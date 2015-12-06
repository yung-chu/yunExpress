using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class ReceivingExpensRepository
    {
        #region 老版
        
        ///// <summary>
        ///// 收货费用明细
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public IPagedList<ReceivingExpenseExt> GetInFeeInfoExtPagedList(FinancialParam param)
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");

        //    var numberList = new List<string>();
        //    if (!string.IsNullOrWhiteSpace(param.SearchContext))
        //    {
        //        numberList =
        //            param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        //                 .ToList();
        //    }

        //    Expression<Func<ReceivingExpens, bool>> filter = o => true;

        //    filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue)
        //                   .AndIf(r => r.WayBillInfo.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
        //                   .AndIf(r => r.WayBillInfo.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode));

        //    if (param.SearchWhere.HasValue && numberList.Count > 0)
        //    {
        //        switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
        //        {
        //            case WayBill.SearchFilterEnum.WayBillNumber:
        //                filter = filter.And(p => numberList.Contains(p.WayBillNumber));
        //                break;
        //            case WayBill.SearchFilterEnum.TrackingNumber:
        //                filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
        //                break;
        //            case WayBill.SearchFilterEnum.CustomerOrderNumber:
        //                filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
        //                break;

        //        }
        //    }

        //    var list = from w in ctx.ReceivingExpenses.Where(filter)
        //               join c in ctx.Customers on w.WayBillInfo.CustomerCode equals c.CustomerCode
        //               orderby w.AuditorDate descending 

        //               select new ReceivingExpenseExt()
        //                   {
        //                       CustomerCode = w.WayBillInfo.CustomerCode,
        //                       CustomerName = c.Name,

        //                       ShippingMethodName = w.WayBillInfo.InShippingMethodName,

        //                       WayBillNumber = w.WayBillNumber,

        //                       CustomerOrderNumber = w.CustomerOrderNumber,

        //                       TrackingNumber = w.WayBillInfo.TrackingNumber,

        //                       CountryCode = w.WayBillInfo.CountryCode,

        //                       Weight = w.WayBillInfo.Weight.Value,

        //                       SettleWeight = w.WayBillInfo.SettleWeight.Value,

        //                       //运费
        //                       Freight = w.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),

        //                       //挂号费
        //                       Register = w.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),

        //                       //燃油费
        //                       FuelCharge = w.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),
                               
        //                       //关税预付服务费
        //                       TariffPrepayFee = w.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),
                              
        //                       //附加费
        //                       Surcharge = w.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),

        //                       //总费用
        //                       TotalFee = w.ReceivingExpenseInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

        //                       FinancialNote = w.FinancialNote
        //                   };

        //    return list.ToPagedList(param.Page, param.PageSize);
        //}
        #endregion

        /// <summary>
        /// 收货费用明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<ReceivingExpenseExt> GetInFeeInfoExtPagedList(FinancialParam param)
        {

            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var numberlist = new List<string>();
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist = param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId, DbType = DbType.Int32 };
            var SearchWhere = new SqlParameter { ParameterName = "SearchWhere", Value = param.SearchWhere, DbType = DbType.Int32 };
            var SearchContext = new SqlParameter { ParameterName = "SearchContext", Value = list, DbType = DbType.String };
            var TotalRecord = new SqlParameter { ParameterName = "TotalRecord", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var PageSize = new SqlParameter { ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32 };
            var PageIndex = new SqlParameter { ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32 };
            var TotalPage = new SqlParameter { ParameterName = "TotalPage", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };

            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<ReceivingExpenseExt>("P_GetReceivingExpensesAbnormalPagedList"
                                                                                , CustomerCode, ShippingMethodId, SearchWhere,
                                                                                SearchContext, TotalRecord,
                                                                                PageSize, PageIndex, TotalPage);
                if (obj != null && obj.Count > 0)
                {
                    return new PagedList<ReceivingExpenseExt>() { InnerList = obj.ToList(), PageIndex = param.Page, PageSize = param.PageSize, TotalCount = Int32.Parse(TotalRecord.Value.ToString()), TotalPages = Int32.Parse(TotalPage.Value.ToString()) };
                }
            }

            return new PagedList<ReceivingExpenseExt>() { InnerList = null, PageIndex = param.Page, PageSize = param.PageSize, TotalCount = 0, TotalPages = 0 };
           

        }

        public ReceivingExpensesEditExt GetReceivingExpensesEditEx(string wayBillNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var list = from r in ctx.ReceivingExpenses.Where(r => r.WayBillNumber == wayBillNumber)
                       join c in ctx.Customers on r.WayBillInfo.CustomerCode equals c.CustomerCode
                       select new ReceivingExpensesEditExt()
                           {
                               CustomerName = c.Name,
                               CustomerOrderNumber = r.WayBillInfo.CustomerOrderNumber,
                               FinancialNote = r.FinancialNote,
                               WayBillNumber=wayBillNumber,

                               FreightOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),
                               RegisterOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),
                               FuelChargeOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),
                               TariffPrepayFeeOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),
                               SurchargeOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),
                               RemoteAreaFeeOriginal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 12 && e.OperationType == 1).Sum(e => e.Amount),
                               TotalFeeOriginal = r.ReceivingExpenseInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                               FreightFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 3).Sum(e => e.Amount),
                               RegisterFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 3).Sum(e => e.Amount),
                               FuelChargeFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 3).Sum(e => e.Amount),
                               TariffPrepayFeeFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 3).Sum(e => e.Amount),
                               SurchargeFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 3).Sum(e => e.Amount),
                               RemoteAreaFeeFinal = r.ReceivingExpenseInfos.Where(e => e.FeeTypeID == 12 && e.OperationType == 3).Sum(e => e.Amount),
                               TotalFeeFinal = r.ReceivingExpenseInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),
                           };

            return list.FirstOrDefault();

        }

        /// <summary>
        /// 收货费用
        /// Add By zhengsong
        /// Time:2014-06-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<InFeeInfoAuditListExt> GetAuditPagedList(InFeeInfoAuditParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var numberlist = new List<string>();
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist = param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId, DbType = DbType.Int32 };
            var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
            var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };
            //var DateWhere = new SqlParameter { ParameterName = "DateWhere", Value = param.DateWhere, DbType = DbType.Int32 };
            var CountryCode = new SqlParameter { ParameterName = "CountryCode", Value = param.CountryCode, DbType = DbType.String };
            var SearchWhere = new SqlParameter { ParameterName = "SearchWhere", Value = param.SearchWhere, DbType = DbType.Int32 };
            var SearchContext = new SqlParameter { ParameterName = "SearchContext", Value = list, DbType = DbType.String };
            var TotalRecord = new SqlParameter { ParameterName = "TotalRecord", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var PageSize = new SqlParameter { ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32 };
            var PageIndex = new SqlParameter { ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32 };
            var TotalPage = new SqlParameter { ParameterName = "TotalPage", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var Status = new SqlParameter { ParameterName = "Status", Value = param.Status, DbType = DbType.Int32 };
            
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeInfoAuditListExt>("P_GetReceivingExpensesPagedList"
                                                                                , CustomerCode, ShippingMethodId,
                                                                                StartTime, EndTime, CountryCode, SearchWhere,
                                                                                SearchContext, Status, TotalRecord,
                                                                                PageSize, PageIndex, TotalPage);
                if (obj != null && obj.Count > 0)
                {
                    return new PagedList<InFeeInfoAuditListExt>() { InnerList = obj.ToList(), PageIndex = param.Page, PageSize = param.PageSize, TotalCount = Int32.Parse(TotalRecord.Value.ToString()), TotalPages = Int32.Parse(TotalPage.Value.ToString()) };
                }
            }

            return new PagedList<InFeeInfoAuditListExt>() { InnerList = null, PageIndex = param.Page, PageSize = param.PageSize, TotalCount = 0, TotalPages = 0 };
           
        }

        public IList<InFeeInfoAuditListExt> GetAuditList(InFeeInfoAuditParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var numberlist = new List<string>();
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist = param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId, DbType = DbType.Int32 };
            var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
            var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };
            var CountryCode = new SqlParameter { ParameterName = "CountryCode", Value = param.CountryCode, DbType = DbType.String };
            var SearchWhere = new SqlParameter { ParameterName = "SearchWhere", Value = param.SearchWhere, DbType = DbType.Int32 };
            var SearchContext = new SqlParameter { ParameterName = "SearchContext", Value = list, DbType = DbType.String };
            var Status = new SqlParameter { ParameterName = "Status", Value = param.Status, DbType = DbType.Int32 };
            //var DateWhere = new SqlParameter { ParameterName = "DateWhere", Value = param.DateWhere, DbType = DbType.Int32 };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeInfoAuditListExt>("P_GetReceivingExpensesList @CustomerCode,@ShippingMethodId,@CountryCode,@StartTime,@EndTime,@SearchWhere,@SearchContext,@Status"
                                                        , CustomerCode, ShippingMethodId, CountryCode, StartTime, EndTime, SearchWhere, SearchContext, Status);
                if (obj != null && obj.Count > 0)
                {
                    return obj;
                }
            }
            return null;
            
        }

        public IList<InFeeInfoAuditListExt> GetInFeeInfoExport(InFeeInfoAuditParam param)
        {
            //var ctx = this.UnitOfWork as LMS_DbContext;
            //Check.Argument.IsNotNull(ctx, "数据库对象");
            var ctx = new LMS_DbContext();
            var numberlist = new List<string>();
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist = param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId, DbType = DbType.Int32 };
            var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
            var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };
            var CountryCode = new SqlParameter { ParameterName = "CountryCode", Value = param.CountryCode, DbType = DbType.String };
            var SearchWhere = new SqlParameter { ParameterName = "SearchWhere", Value = param.SearchWhere, DbType = DbType.Int32 };
            var SearchContext = new SqlParameter { ParameterName = "SearchContext", Value = list, DbType = DbType.String };
            var Status = new SqlParameter { ParameterName = "Status", Value = param.Status, DbType = DbType.Int32 };
            var PageSize = new SqlParameter { ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32 };
            var PageIndex = new SqlParameter { ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32 };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeInfoAuditListExt>("P_GetReceivingExpensesList_Export  @CustomerCode,@ShippingMethodId,@CountryCode,@StartTime,@EndTime,@SearchWhere,@SearchContext,@Status,@PageSize,@PageIndex"
                                                        , CustomerCode, ShippingMethodId, CountryCode, StartTime, EndTime, SearchWhere, SearchContext, Status,PageSize,PageIndex);
                if (obj != null && obj.Count > 0)
                {
                    return obj;
                }
            }
            return null;
        }
        public int GetInFeeInfoExportTotalCount(InFeeInfoAuditParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int isSuccess = 0;
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId ?? (object)DBNull.Value, DbType = DbType.Int32 };
            var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
            var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };
            var CountryCode = new SqlParameter { ParameterName = "CountryCode", Value = param.CountryCode ?? (object)DBNull.Value, DbType = DbType.String };
            var Status = new SqlParameter { ParameterName = "Status", Value = param.Status ?? (object)DBNull.Value, DbType = DbType.Int32 };
            var TotalRecord = new SqlParameter { ParameterName = "TotalRecord", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            if (ctx != null)
            {
                ctx.ExecuteCommand("Exec P_GetReceivingExpensesList_Export_TotalCount  @CustomerCode,@ShippingMethodId,@CountryCode,@StartTime,@EndTime,@Status,@TotalRecord output"
                                                        , CustomerCode, ShippingMethodId, CountryCode, StartTime, EndTime, Status, TotalRecord);
                Int32.TryParse(TotalRecord.Value.ToString(), out isSuccess);
            }
            return isSuccess;
        }
    }
}
