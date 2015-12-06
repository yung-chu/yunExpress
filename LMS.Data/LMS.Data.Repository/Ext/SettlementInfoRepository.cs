using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class SettlementInfoRepository
    {
        public List<CustomerSmallExt> GetOutstandingPaymentCustomer(string keyword)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            int status = Settlement.StatusToValue(Settlement.StatusEnum.Outstanding);
            return keyword.IsNullOrWhiteSpace()
                       ? (from c in ctx.Customers
                          where
                              (from s in
                                   ctx.SettlementInfos.Where(
                                       w => w.Status == status)
                               select s.CustomerCode).Contains(c.CustomerCode)
                          select new CustomerSmallExt()
                              {
                                  CustomerCode = c.CustomerCode,
                                  CustomerID = c.CustomerID,
                                  Name = c.Name
                              }).ToList()
                       : (from c in
                              ctx.Customers.Where(c => c.CustomerCode.Contains(keyword) || c.Name.Contains(keyword))
                          where
                              (from s in
                                   ctx.SettlementInfos.Where(
                                       w => w.Status == status)
                               select s.CustomerCode).Contains(c.CustomerCode)
                          select new CustomerSmallExt()
                              {
                                  CustomerCode = c.CustomerCode,
                                  CustomerID = c.CustomerID,
                                  Name = c.Name
                              }).ToList();
        }


        public IPagedList<SettlementInfoExt> GetSettlementInfoList(SettlementInfoParam param)
        {
            var ctx = new LMS_DbContext();
            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = false;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var numberList = new List<string>();
            if (!string.IsNullOrEmpty(param.SettlementNumber))
            {
                numberList =
                    param.SettlementNumber.Split(Environment.NewLine.ToCharArray(),
                                                 StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            Expression<Func<SettlementInfo, bool>> filter = o => true;

            if (numberList.Any())
            {
                filter = filter.And(a => numberList.Contains(a.SettlementNumber));
            }
            else
            {
                //取当天记录
                if (param.StartTime.HasValue && param.EndTime.HasValue)
                {
                    if (param.StartTime == param.EndTime)
                    {
                        param.EndTime = param.EndTime.Value.AddHours(23).AddMinutes(59);
                    }
                }

                filter = filter.AndIf(a => a.CustomerCode.Contains(param.CustomerCode),
                                      !string.IsNullOrEmpty(param.CustomerCode))
                               .AndIf(a => param.StartTime <= a.CreatedOn && a.CreatedOn <= param.EndTime,
                                      param.StartTime.HasValue && param.EndTime.HasValue)
                               .AndIf(a => a.Status == param.Status, param.Status.HasValue)
                               .AndIf(a => a.CreatedBy.Contains(param.CreatedBy), !string.IsNullOrEmpty(param.CreatedBy))
                               .AndIf(a => a.SettlementBy.Contains(param.SettlementBy),
                                      !string.IsNullOrEmpty(param.SettlementBy));
            }



            var result = from a in ctx.SettlementInfos.Where(filter)
                         join b in ctx.Customers on a.CustomerCode equals b.CustomerCode into g
                         from c in g.DefaultIfEmpty()
                         orderby a.CreatedOn descending
                         select new SettlementInfoExt
                             {
                                 SettlementNumber = a.SettlementNumber,
                                 CustomerName = c.Name,
                                 CustomerCode = a.CustomerCode,
                                 TotalNumber = a.TotalNumber,
                                 TotalSettleWeight = a.TotalSettleWeight,
                                 TotalFee = a.TotalFee,
                                 CreatedBy = a.CreatedBy,
                                 CreatedOn = a.CreatedOn,
                                 Status = a.Status,
                                 SettlementBy = a.SettlementBy,
                                 SettlementOn = a.SettlementOn
                             };

            return result.ToPagedList(param.Page, param.PageSize);


        }

        public IPagedList<SettlementSummaryExt> GetSettlementSummaryExtPagedList(SettlementSummaryParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            Check.Argument.IsNotNull(ctx, "数据库对象");

            var query = from w in ctx.WayBillInfos
                        join c in ctx.Customers on w.CustomerCode equals c.CustomerCode
                        join cb in ctx.CustomerBalances on w.CustomerCode equals cb.CustomerCode
                        where
                            w.Status == (int) WayBill.StatusEnum.Have && (c.PaymentTypeID == 3 || c.PaymentTypeID == 4)
                        select new
                            {
                                w.CustomerCode,
                                c.Name,
                                SalesMan = c.CustomerManager,
                                cb.Balance,
                            };



            var querySummary = from w in
                                   (from rx in ctx.ReceivingExpenses
                                    join wb in ctx.WayBillInfos on rx.WayBillNumber equals wb.WayBillNumber
                                    where wb.Status == (int) WayBill.StatusEnum.Have
                                    select new
                                        {
                                            wb.CustomerCode,
                                            ShippingMethodName = wb.InShippingMethodName,
                                            wb.Weight,
                                            wb.SettleWeight,
                                            Fee =
                                        ctx.ReceivingExpenseInfos.Where(
                                            e => e.OperationType == 1 && e.ReceivingExpenseID == rx.ReceivingExpenseID)
                                           .Sum(e => e.Amount),
                                        })
                               group w by new {w.CustomerCode, w.ShippingMethodName} into g
                               select new SettlementShippingMethodSummaryExt
                                   {
                                       CustomerCode = g.Key.CustomerCode,
                                       ShippingMethodName = g.Key.ShippingMethodName,
                                       HaveWaybillCount = g.Count(),
                                       TotalWeight = g.Sum(s => s.Weight),
                                       TotalSettleWeight = g.Sum(s => s.SettleWeight),
                                       TotalFee = g.Sum(s => s.Fee),
                                   };


            var list = query.GroupBy(g => new {g.Balance, g.CustomerCode, g.Name, g.SalesMan})
                            .WhereIf(g => g.Key.Balance >= 0, param.Status == 2)
                            .WhereIf(g => g.Key.Balance < 0, param.Status == 1)
                            .WhereIf(g => g.Key.CustomerCode == param.CustomerCode,
                                     !string.IsNullOrWhiteSpace(param.CustomerCode))
                            .OrderBy(g => g.Key.CustomerCode)
                            .Select(g => new SettlementSummaryExt
                                {
                                    Balance = g.Key.Balance,
                                    CustomerCode = g.Key.CustomerCode,
                                    CustomerName = g.Key.Name,
                                    HaveWaybillCount = g.Count(),
                                    SalesMan = g.Key.SalesMan,
                                }).ToPagedList(param.Page, param.PageSize);

            list.InnerList.ForEach(p =>
                {
                    p.SettlementShippingMethodSummaryExts =
                        querySummary.Where(s => s.CustomerCode == p.CustomerCode).ToList();
                });

            return list;

        }
    }
}
