using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class B2CPreAlertLogsRepository
    {
        public PagedList<B2CPreAlterExt> GetB2CPreAlterExtList(B2CPreAlterListParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var wayBillStatus = new List<int>
                {
                    WayBill.StatusEnum.WaitOrder.GetStatusValue(),
                    WayBill.StatusEnum.Send.GetStatusValue()
                };
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => wayBillStatus.Contains(o.Status);
            Expression<Func<B2CPreAlertLogs, bool>> filterB2C = o => true;

            filterWayBill = filterWayBill.And(p => p.OutStorageCreatedOn >= param.OutStartTime);
            int isNotExist = 0;
            if (!param.SearchContext.IsNullOrWhiteSpace())
            {
                var wayBillList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                switch (WayBill.ParseToSearchFilter(param.SearchWhere))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.CustomerOrderNumber));
                        break;
                }
                filterWayBill = filterWayBill.And(p => param.ShippingMethodIds.Contains(p.OutShippingMethodID.Value));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(param.CountryCode))
                {
                    var codes = param.CountryCode.Split(',').ToList();
                    if (codes.Count > 0)
                    {
                        filterWayBill = filterWayBill.And(p => codes.Contains(p.CountryCode));
                    }
                }
                filterWayBill =
                    filterWayBill.AndIf(p => p.CustomerCode == param.CustomerCode,
                                        !string.IsNullOrWhiteSpace(param.CustomerCode))
                                 .AndIf(p => p.SettleWeight >= param.StartWeight, param.StartWeight.HasValue)
                                 .AndIf(p => p.SettleWeight <= param.EndWeight, param.EndWeight.HasValue)
                                 .And(p => p.OutShippingMethodID == param.ShippingMethodId)
                    ;
                switch (WayBill.ParseToDateFilter(param.SearchTime))
                {
                    case WayBill.DateFilterEnum.TakeOverOn:
                        filterWayBill = filterWayBill.AndIf(p => p.InStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(p => p.InStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.DeliverOn:
                        filterWayBill = filterWayBill.AndIf(p => p.OutStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(p => p.OutStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                }
                if (param.Status.HasValue)
                {
                    if (B2CPreAlter.ParseToStatus(param.Status.Value) == B2CPreAlter.StatusEnum.None)
                    {
                        isNotExist = 1;
                    }
                    else
                    {
                        isNotExist = 2;
                        filterB2C = filterB2C.And(p => p.Status == param.Status.Value);
                    }

                }
            }
            var okStatus = B2CPreAlter.StatusToValue(B2CPreAlter.StatusEnum.OK);
            var noneStatus = B2CPreAlter.StatusToValue(B2CPreAlter.StatusEnum.None);
            if (isNotExist == 0)
            {
                return (from w in ctx.WayBillInfos.Where(filterWayBill)
                        join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                        join b in ctx.B2CPreAlertLogs on w.WayBillNumber equals b.WayBillNumber into B2C
                        from c in B2C.DefaultIfEmpty()
                        orderby w.OutStorageCreatedOn descending
                        select new B2CPreAlterExt
                            {
                                CountryCode = w.CountryCode,
                                CustomerCode = w.CustomerCode,
                                ErrorMsg = c != null ? (c.Status == okStatus ? "" : c.ErrorMsg) : "",
                                InStorageCreatedOn = w.InStorageCreatedOn,
                                OutShippingMethodName = w.OutShippingMethodName,
                                SettleWeight = w.SettleWeight ?? 0,
                                TrackingNumber = w.TrackingNumber,
                                VenderName = o.VenderName,
                                WayBillNumber = w.WayBillNumber,
                                Status = c != null ? c.Status : 0
                            }).ToPagedList(param.Page, param.PageSize);
            }
            else if (isNotExist == 1)
            {
                return (from w in ctx.WayBillInfos.Where(filterWayBill)
                        join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                        where !(from c in ctx.B2CPreAlertLogs select c.WayBillNumber).Contains(w.WayBillNumber)
                        orderby w.OutStorageCreatedOn descending
                        select new B2CPreAlterExt
                            {
                                CountryCode = w.CountryCode,
                                CustomerCode = w.CustomerCode,
                                ErrorMsg = "",
                                InStorageCreatedOn = w.InStorageCreatedOn,
                                OutShippingMethodName = w.OutShippingMethodName,
                                SettleWeight = w.SettleWeight ?? 0,
                                TrackingNumber = w.TrackingNumber,
                                VenderName = o.VenderName,
                                WayBillNumber = w.WayBillNumber,
                                Status = noneStatus
                            }).ToPagedList(param.Page, param.PageSize);
            }
            else
            {
                return (from w in ctx.WayBillInfos.Where(filterWayBill)
                        join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                        join b in ctx.B2CPreAlertLogs.Where(filterB2C) on w.WayBillNumber equals b.WayBillNumber
                        orderby w.OutStorageCreatedOn descending
                        select new B2CPreAlterExt
                        {
                            CountryCode = w.CountryCode,
                            CustomerCode = w.CustomerCode,
                            ErrorMsg = b.Status == okStatus ? "" : b.ErrorMsg,
                            InStorageCreatedOn = w.InStorageCreatedOn,
                            OutShippingMethodName = w.OutShippingMethodName,
                            SettleWeight = w.SettleWeight ?? 0,
                            TrackingNumber = w.TrackingNumber,
                            VenderName = o.VenderName,
                            WayBillNumber = w.WayBillNumber,
                            Status = b.Status
                        }).ToPagedList(param.Page, param.PageSize);
            }
        }

        public bool PreAlterB2CBySearch(B2CPreAlterListParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var wayBillStatus = new List<int>
                {
                    WayBill.StatusEnum.WaitOrder.GetStatusValue(),
                    WayBill.StatusEnum.Send.GetStatusValue()
                };
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => wayBillStatus.Contains(o.Status);
            filterWayBill = filterWayBill.And(p => p.OutStorageCreatedOn >= param.OutStartTime);
            if (!param.SearchContext.IsNullOrWhiteSpace())
            {
                var wayBillList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                switch (WayBill.ParseToSearchFilter(param.SearchWhere))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.CustomerOrderNumber));
                        break;
                }
                filterWayBill = filterWayBill.And(p => param.ShippingMethodIds.Contains(p.OutShippingMethodID.Value));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(param.CountryCode))
                {
                    var codes = param.CountryCode.Split(',').ToList();
                    if (codes.Count > 0)
                    {
                        filterWayBill = filterWayBill.And(p => codes.Contains(p.CountryCode));
                    }
                }
                filterWayBill =
                    filterWayBill.AndIf(p => p.CustomerCode == param.CustomerCode,
                                        !string.IsNullOrWhiteSpace(param.CustomerCode))
                                 .AndIf(p => p.SettleWeight >= param.StartWeight, param.StartWeight.HasValue)
                                 .AndIf(p => p.SettleWeight <= param.EndWeight, param.EndWeight.HasValue)
                                 .And(p => p.OutShippingMethodID == param.ShippingMethodId)
                    ;
                switch (WayBill.ParseToDateFilter(param.SearchTime))
                {
                    case WayBill.DateFilterEnum.TakeOverOn:
                        filterWayBill = filterWayBill.AndIf(p => p.InStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(p => p.InStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.DeliverOn:
                        filterWayBill = filterWayBill.AndIf(p => p.OutStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(p => p.OutStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                }
            }
            var list = (from w in ctx.WayBillInfos.Where(filterWayBill)
                        join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                        where !(from c in ctx.B2CPreAlertLogs select c.WayBillNumber).Contains(w.WayBillNumber)
                        select w.WayBillNumber).ToList();

            return PreAlterB2CByWayBillNumber(list);
        }

        public bool PreAlterB2CByWayBillNumber(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            wayBillNumbers.ForEach(p =>
                {
                    if (!ctx.B2CPreAlertLogs.Any(o => o.WayBillNumber == p))
                    {
                        ctx.B2CPreAlertLogs.Add(new B2CPreAlertLogs()
                            {
                                CreatedBy = "system",
                                CreatedOn = DateTime.Now,
                                LastUpdatedBy = "system",
                                LastUpdatedOn = DateTime.Now,
                                WayBillNumber = p,
                                Status = 1
                            });
                    }
                });
            using (var scope = new TransactionScope())
            {
                ctx.SaveChanges();
                scope.Complete();
            }
            return true;
        }
    }
}
