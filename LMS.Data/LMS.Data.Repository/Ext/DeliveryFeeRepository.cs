
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using LMS.Data.Entity.Param;
using LMS.Data.Entity.ExtModel;
using System.Data;
using System.Transactions;
using System.Data.SqlClient;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Data.Repository
{
    public partial class DeliveryFeeRepository
    {
        public PagedList<DeliveryFeeExt> Search(DeliveryReviewParam param, bool enableStatusFilter, bool isExpress)
        {
            return QueryViaSql(param, enableStatusFilter, isExpress);
            //return Query(param, status).ToPagedList(param.Page, param.PageSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="status">是否状态过滤</param>
        /// <returns></returns>
        private IQueryable<DeliveryFeeExt> Query(DeliveryReviewParam param, bool enableStatusFilter = false)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;

            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            Expression<Func<DeliveryFee, bool>> filter = o => true;

            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue && enableStatusFilter);
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
                        filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue && enableStatusFilter);
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue && enableStatusFilter);
                        break;
                }
            }
            else
            {
                filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue)
                               .AndIf(r => param.ShippingMethodIds.Contains(r.ShippingmethodID), param.ShippingMethodIds != null && param.ShippingMethodIds.Length > 0)
                               .AndIf(r => r.VenderCode == param.VenderCode, !string.IsNullOrWhiteSpace(param.VenderCode))
                               .AndIf(r => r.WayBillInfo.OutStorageCreatedOn >= param.StartTime, param.StartTime.HasValue)
                               .AndIf(r => r.WayBillInfo.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue);
            }
            //if (param.SearchWhere.HasValue && numberList.Count > 0)
            //{
            //    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
            //    {
            //        case WayBill.SearchFilterEnum.WayBillNumber:
            //            filter = filter.And(p => numberList.Contains(p.WayBillNumber));
            //            break;
            //        case WayBill.SearchFilterEnum.TrackingNumber:
            //            filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
            //            break;
            //        case WayBill.SearchFilterEnum.CustomerOrderNumber:
            //            filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
            //            break;
            //    }
            //}

            var list = from w in ctx.DeliveryFees.Where(filter)
                       //join c in ctx.WayBillInfos on w.WayBillNumber equals c.WayBillNumber
                       orderby w.AuditorDate descending
                       select new DeliveryFeeExt()
                       {
                           VenderCode = w.VenderCode,
                           //AprroveWeight
                           Auditor = w.Auditor,
                           AprroveWeight = w.AprroveWeight,
                           AuditorDate = w.AuditorDate,
                           CreatedBy = w.CreatedBy,
                           CreatedOn = w.CreatedOn,
                           CustomerOrderNumber = w.CustomerOrderNumber,
                           DeliveryFeeID = w.DeliveryFeeID,
                           LastUpdatedBy = w.LastUpdatedBy,
                           LastUpdatedOn = w.LastUpdatedOn,
                           Remark = w.Remark,
                           SetWeight = w.WayBillInfo.SettleWeight,
                           ShippingmethodID = w.ShippingmethodID,
                           ShippingmethodName = w.ShippingmethodName,
                           Status = w.Status,
                           Trackingnumber = w.Trackingnumber,
                           VenderName = w.VenderName,
                           VenderId = w.VenderId,
                           WayBillNumber = w.WayBillNumber,

                           Weight = w.WayBillInfo.Weight,
                           OutStorageCreatedOn = w.WayBillInfo.OutStorageCreatedOn,
                           CountryCode = w.WayBillInfo.CountryCode,

                           //运费
                           Freight = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),

                           //挂号费
                           Register = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),

                           //燃油费
                           FuelCharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),

                           //关税预付服务费
                           TariffPrepayFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),

                           //附加费
                           Surcharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),

                           //总费用
                           TotalFee = w.DeliveryFeeInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                           //最终总费用
                           TotalFeeFinal = w.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),

                       };

            return list;
        }

        private PagedList<DeliveryFeeExt> QueryViaSql(DeliveryReviewParam param, bool enableStatusFilter, bool isExpress)
        {
            StringBuilder sbWhere = new StringBuilder();
            //sbWhere.Append(" WHERE 1=1 ");
            List<SqlParameter> myParams = new List<SqlParameter>();

            #region 条件

            string numberListstr = string.Empty;

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                   param.SearchContext
                   .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                   .ToList();
                numberListstr = string.Join("','", numberList);
            }

            if (param.SearchWhere.HasValue && !string.IsNullOrEmpty(numberListstr))
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        sbWhere.AppendFormat(" And df.WayBillNumber IN ('{0}') ", numberListstr);
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        sbWhere.AppendFormat(" And wbi.TrackingNumber IN ('{0}') ", numberListstr);
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        sbWhere.AppendFormat(" And df.CustomerOrderNumber IN ('{0}') ", numberListstr);
                        break;
                    default:
                        break;
                }

                if (param.Status.HasValue && enableStatusFilter)
                {
                    sbWhere.Append(" And df.Status = @Status ");
                    myParams.Add(new SqlParameter("Status", param.Status.Value));
                }
            }
            else
            {
                if (param.Status.HasValue)
                {
                    sbWhere.Append(" And df.Status = @Status ");
                    myParams.Add(new SqlParameter("Status", param.Status.Value));
                }
                if (!string.IsNullOrWhiteSpace(param.VenderCode))
                {
                    sbWhere.Append(" And df.VenderCode = @VenderCode ");
                    myParams.Add(new SqlParameter("VenderCode", param.VenderCode.Trim()));
                }
                if (param.StartTime.HasValue)
                {
                    sbWhere.Append(" And DATEDIFF(DAY ,@StartTime, wbi.OutStorageCreatedOn)>=0 ");
                    myParams.Add(new SqlParameter("StartTime", param.StartTime.Value));
                }
                if (param.EndTime.HasValue)
                {
                    sbWhere.Append(" And DATEDIFF(DAY ,wbi.OutStorageCreatedOn, @EndTime)>=0  ");
                    myParams.Add(new SqlParameter("EndTime", param.EndTime.Value));
                }
            }
            if (param.ShippingMethodIds != null && param.ShippingMethodIds.Length > 0)
            {
                string shippingMethodIdString = string.Join(",", param.ShippingMethodIds);
                sbWhere.Append(string.Format(" And df.ShippingMethodId in ({0}) ", shippingMethodIdString));
            }
            #endregion

            #region 字段
            string fileds = @"
    df.VenderCode ,
    df.Auditor ,
    df.AprroveWeight ,
    df.AuditorDate ,
    df.CreatedBy ,
    df.CreatedOn ,
    df.CustomerOrderNumber ,
    df.DeliveryFeeID ,
    df.LastUpdatedBy ,
    df.LastUpdatedOn ,
    df.Remark ,
    df.SetWeight ,
    df.ShippingmethodID ,
    df.ShippingmethodName ,
    df.Status ,
    wbi.Trackingnumber ,
    df.VenderName ,
    df.VenderId ,
    df.WayBillNumber ,
    wbi.Weight ,
    wbi.OutStorageCreatedOn ,
    wbi.CountryCode,
    c.Name as CustomerName";
//,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 3 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Freight,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 4 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Register,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 5 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) FuelCharge,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 6 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TariffPrepayFee,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 2 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Surcharge, 
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TotalFee,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE OperationType=3 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TotalFeeFinal      
//";
//            if (isExpress)
//            {
//                fileds +=
//                    @",(SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 8 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) SecurityAppendFee,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 9 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OverWeightLengthGirthFee,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 10 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) AddedTaxFee,
//    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 11 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OtherFee,
//    (SELECT TOP 1 Remark FROM DeliveryFeeInfos WHERE FeeTypeID = 11 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OtherFeeRemark";
//            }
            #endregion

            string tables = @" dbo.DeliveryFees df LEFT JOIN dbo.WayBillInfos wbi ON df.WayBillNumber = wbi.WayBillNumber INNER JOIN [dbo].[Customers] c ON wbi.CustomerCode=c.CustomerCode";
            string orderBy = " order by df.DeliveryFeeID desc ";
            string sqlCount = string.Format(" select count(0) from {0}  WHERE 1=1  {1} ", tables, sbWhere.ToString());
            string sqlPaging = ((!param.IsExportExcel.HasValue) || (!param.IsExportExcel.Value))
                                   ? string.Format(@"
SELECT * FROM (
SELECT ROW_NUMBER() OVER (ORDER BY DeliveryFeeID DESC) as RowNum,{2} 
FROM {0} WHERE 1=1 {1}
) t Where RowNum>@RowStart and RowNum<=@RowEnd 
Order by DeliveryFeeID desc",
                                                   tables, sbWhere.ToString(), fileds)
                                   : string.Format(@"SELECT {2} FROM {0} WHERE 1=1 {1} {3} ", tables, sbWhere.ToString(),
                                                   fileds, orderBy);

            var pagingParams = new List<SqlParameter>(CollectionHelper.Clone(myParams));
            int total = 0;
            List<DeliveryFeeExt> list = new List<DeliveryFeeExt>();

            string sumfileds = @"DeliveryFeeID,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 3 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Freight,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 4 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Register,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 5 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) FuelCharge,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 6 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TariffPrepayFee,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 2 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) Surcharge, 
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TotalFee,
                (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE OperationType=3 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) TotalFeeFinal ";
            if (isExpress)
            {
                sumfileds +=
                    @",(SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 8 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) SecurityAppendFee,
                    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 9 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OverWeightLengthGirthFee,
                    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 10 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) AddedTaxFee,
                    (SELECT SUM(Amount) FROM DeliveryFeeInfos WHERE FeeTypeID = 11 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OtherFee,
                    (SELECT TOP 1 Remark FROM DeliveryFeeInfos WHERE FeeTypeID = 11 AND OperationType=1 AND DeliveryFeeInfos.DeliveryFeeID = df.DeliveryFeeID) OtherFeeRemark";
            }
            using (LMS_DbContext ctx = new LMS_DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;

                total = ctx.Database.SqlQuery<int>(sqlCount, myParams.ToArray()).FirstOrDefault();

                if (!param.IsExportExcel.HasValue || !param.IsExportExcel.Value)
                {
                    pagingParams.Add(new SqlParameter("RowStart", param.PageSize * (param.Page - 1)));
                    pagingParams.Add(new SqlParameter("RowEnd", param.PageSize * param.Page));

                    list = ctx.Database.SqlQuery<DeliveryFeeExt>(sqlPaging, pagingParams.ToArray()).ToList();
                }
                else
                {
                    //导出Execl使用
                    list = ctx.Database.SqlQuery<DeliveryFeeExt>(sqlPaging, pagingParams.ToArray()).ToList();
                }
                if (list.Any())
                    {
                        string deliveryFeeID = string.Join(",", list.Select(p => p.DeliveryFeeID).ToList());
                        var feesql =
                            string.Format(@"SELECT {0} FROM dbo.DeliveryFees df WHERE df.DeliveryFeeID in ({1})",
                                          sumfileds, deliveryFeeID);
                        var feelist = ctx.Database.SqlQuery<DeliveryFeeExt>(feesql).ToDictionary(p=>p.DeliveryFeeID);
                        if (feelist.Any())
                        {
                            foreach (var deliveryFeeExt in list)
                            {
                                deliveryFeeExt.Freight = feelist[deliveryFeeExt.DeliveryFeeID].Freight;
                                deliveryFeeExt.Register = feelist[deliveryFeeExt.DeliveryFeeID].Register;
                                deliveryFeeExt.FuelCharge = feelist[deliveryFeeExt.DeliveryFeeID].FuelCharge;
                                deliveryFeeExt.TariffPrepayFee = feelist[deliveryFeeExt.DeliveryFeeID].TariffPrepayFee;
                                deliveryFeeExt.Surcharge = feelist[deliveryFeeExt.DeliveryFeeID].Surcharge;
                                deliveryFeeExt.TotalFee = feelist[deliveryFeeExt.DeliveryFeeID].TotalFee;
                                deliveryFeeExt.TotalFeeFinal = feelist[deliveryFeeExt.DeliveryFeeID].TotalFeeFinal;
                                if (isExpress)
                                {
                                    deliveryFeeExt.SecurityAppendFee =
                                        feelist[deliveryFeeExt.DeliveryFeeID].SecurityAppendFee;
                                    deliveryFeeExt.OverWeightLengthGirthFee =
                                        feelist[deliveryFeeExt.DeliveryFeeID].OverWeightLengthGirthFee;
                                    deliveryFeeExt.AddedTaxFee = feelist[deliveryFeeExt.DeliveryFeeID].AddedTaxFee;
                                    deliveryFeeExt.OtherFee = feelist[deliveryFeeExt.DeliveryFeeID].OtherFee;
                                    deliveryFeeExt.OtherFeeRemark = feelist[deliveryFeeExt.DeliveryFeeID].OtherFeeRemark;
                                }
                            }
                        }
                    }
            }

            var result = new PagedList<DeliveryFeeExt>();
            result.PageIndex = param.Page;
            result.PageSize = param.PageSize;
            result.TotalCount = total;
            result.TotalPages = (int)(Math.Ceiling((decimal)(total) / param.PageSize));
            result.InnerList = list;
            return result;
        }

        public List<DeliveryFeeExt> Export(DeliveryReviewParam param, bool enableStatusFilter, bool isExpress)
        {
            return QueryViaSql(param, enableStatusFilter, isExpress).ToList();
        }

        public bool ReverseAudit(List<int> ids, string userName, string remark, DateTime dt)
        {
            return DeliveryFeeUpdateStatus(ids, userName, remark, 1, dt, 3);
        }

        public bool DeliveryFeeAuditError(List<int> ids, string userName, string error, DateTime dt)
        {
            return DeliveryFeeUpdateStatus(ids, userName, error, 2, dt, 1);
        }

        public bool ExpressDeliveryFeeAuditPass(List<AuditParam> dataParams, string userName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            foreach (var auditParam in dataParams)
            {
                if (!ctx.DeliveryFees.Any(t => t.DeliveryFeeID == auditParam.DeliveryFeeId && t.Status == 1))
                {
                    throw new BusinessLogicException("只有状态为[未审核]的单才能被批量审核!");
                }
                var fee = ctx.DeliveryFees.FirstOrDefault(t => t.DeliveryFeeID == auditParam.DeliveryFeeId && t.Status == 1);
                fee.Auditor = userName;
                fee.AuditorDate = DateTime.Now;
                fee.Status = 3;//已审核
                fee.LastUpdatedOn = DateTime.Now;
                fee.LastUpdatedBy = userName;
                fee.Remark = auditParam.ErrorRemark + fee.Remark;
                //修改杂费
                if (auditParam.OtherFee > 0)
                {
                    var deliveryFeeInfo =
                        ctx.DeliveryFeeInfos.FirstOrDefault(
                            t => t.DeliveryFeeID == auditParam.DeliveryFeeId && t.FeeTypeID == 11);
                    if (deliveryFeeInfo == null)
                    {
                        ctx.DeliveryFeeInfos.Add(new DeliveryFeeInfo()
                        {
                            DeliveryFeeID = auditParam.DeliveryFeeId,
                            Amount = auditParam.OtherFee,
                            CreatedBy = userName,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = 11,
                            LastUpdatedBy = userName,
                            LastUpdatedOn = DateTime.Now,
                            MoneyChangeTypeID = 2,
                            OperationType = 1,
                            Remark = auditParam.OtherRemark
                        });
                    }
                    else
                    {
                        deliveryFeeInfo.Amount = auditParam.OtherFee;
                        deliveryFeeInfo.Remark = auditParam.OtherRemark;
                        deliveryFeeInfo.LastUpdatedBy = userName;
                        deliveryFeeInfo.LastUpdatedOn = DateTime.Now;
                        //ctx.DeliveryFeeInfos.Attach(deliveryFeeInfo);
                    }
                }
                //插入最终表
                //var fee = ctx.DeliveryFees.FirstOrDefault(t => t.DeliveryFeeID == auditParam.DeliveryFeeId);
                var diac = ctx.ExpressDeliveryImportAccountChecks.FirstOrDefault(t => t.UserName == userName.Trim() &&
                    t.WayBillNumber == fee.WayBillNumber);
                if (diac != null)
                {
                    var final =
                        ctx.FinalExpressDeliveryImportAccountChecks.FirstOrDefault(
                            t => t.WayBillNumber == diac.WayBillNumber) ??
                        new FinalExpressDeliveryImportAccountCheck();
                    final.AddedTaxFee = diac.AddedTaxFee;
                    final.CountryName = diac.CountryName;
                    final.CreatedBy = diac.CreatedBy;
                    final.CreatedOn = diac.CreatedOn;
                    final.Freight = diac.Freight;
                    final.FuelCharge = diac.FuelCharge;
                    final.IncidentalRemark = diac.IncidentalRemark;
                    final.Incidentals = diac.Incidentals;
                    final.OrderNumber = diac.OrderNumber;
                    final.OverWeightLengthGirthFee = diac.OverWeightLengthGirthFee;
                    final.ReceivingDate = diac.ReceivingDate;
                    final.Register = diac.Register;
                    final.SecurityAppendFee = diac.SecurityAppendFee;
                    final.SettleWeight = diac.SettleWeight;
                    final.ShippingMethodName = diac.ShippingMethodName;
                    final.Surcharge = diac.Surcharge;
                    final.TariffPrepayFee = diac.TariffPrepayFee;
                    final.TotalFee = diac.TotalFee;
                    final.UserName = diac.UserName;
                    final.VenderName = diac.VenderName;
                    final.WayBillNumber = diac.WayBillNumber;
                    final.Weight = diac.Weight;
                    if (final.ExpressDeliveryImportAccountCheckFinalID == 0)
                    {
                        ctx.FinalExpressDeliveryImportAccountChecks.Add(final);
                    }
                }
                else
                {
                    throw new BusinessLogicException("请先导入服务商数据然后在审核!");
                }
                //删除之前的最终记录
                ctx.ExecuteCommand("delete from DeliveryFeeInfos where [DeliveryFeeID]=@DeliveryFeeID And OperationType=@OperationType ",
                    new System.Data.SqlClient.SqlParameter("DeliveryFeeID", fee.DeliveryFeeID),
                    new System.Data.SqlClient.SqlParameter("OperationType", 3)
                    );
                //插入新的最终记录
                var info = new DeliveryFeeInfo
                    {
                        DeliveryFeeID = auditParam.DeliveryFeeId,
                        MoneyChangeTypeID = 2,
                        FeeTypeID = 7,
                        OperationType = 3,
                        Amount = diac.TotalFee,
                        CreatedBy = userName,
                        CreatedOn = DateTime.Now,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = userName
                    };

                ctx.DeliveryFeeInfos.Add(info);
            }
            using (var scope = new TransactionScope())
            {
                ctx.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public bool ExpressDeliveryFeeAuditError(List<AuditParam> dataParams, string userName)
        {
            if (dataParams.Count == 0) { return true; }
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.ExecuteCommand(
                string.Format(@" update [DeliveryFees] set [Status] = @Status , Remark = @Remark + Remark ,Auditor = @Auditor ,AuditorDate = @AuditorDate , LastUpdatedBy = @Auditor , LastUpdatedOn = @AuditorDate  where [DeliveryFeeID] in ({0}) and Status={1} ", string.Join(",", dataParams.Select(p => p.DeliveryFeeId).ToList()), 1),
                new SqlParameter("Status", 2),
                new SqlParameter("Remark", dataParams[0].ErrorRemark),
                new SqlParameter("Auditor", userName),
                new SqlParameter("AuditorDate", DateTime.Now)
                );
            dataParams.ForEach(p =>
                {
                    //修改杂费
                    if (p.OtherFee > 0)
                    {
                        var deliveryFeeInfo =
                            ctx.DeliveryFeeInfos.FirstOrDefault(
                                t => t.DeliveryFeeID == p.DeliveryFeeId && t.FeeTypeID == 11);
                        if (deliveryFeeInfo == null)
                        {
                            ctx.DeliveryFeeInfos.Add(new DeliveryFeeInfo()
                                {
                                    DeliveryFeeID = p.DeliveryFeeId,
                                    Amount = p.OtherFee,
                                    CreatedBy = userName,
                                    CreatedOn = DateTime.Now,
                                    FeeTypeID = 11,
                                    LastUpdatedBy = userName,
                                    LastUpdatedOn = DateTime.Now,
                                    MoneyChangeTypeID = 2,
                                    OperationType = 1,
                                    Remark = p.OtherRemark
                                });
                        }
                        else
                        {
                            deliveryFeeInfo.Amount = p.OtherFee;
                            deliveryFeeInfo.Remark = p.OtherRemark;
                            deliveryFeeInfo.LastUpdatedBy = userName;
                            deliveryFeeInfo.LastUpdatedOn = DateTime.Now;
                            //ctx.DeliveryFeeInfos.Attach(deliveryFeeInfo);
                        }
                    }
                    //插入最终表
                    var fee = ctx.DeliveryFees.FirstOrDefault(t => t.DeliveryFeeID == p.DeliveryFeeId);
                    var diac = ctx.ExpressDeliveryImportAccountChecks.FirstOrDefault(t => t.UserName == userName.Trim() &&
                        t.WayBillNumber == fee.WayBillNumber);
                    if (diac != null)
                    {
                        var final = ctx.FinalExpressDeliveryImportAccountChecks.FirstOrDefault(t => t.WayBillNumber == diac.WayBillNumber) ??
                                    new FinalExpressDeliveryImportAccountCheck();
                        final.AddedTaxFee = diac.AddedTaxFee;
                        final.CountryName = diac.CountryName;
                        final.CreatedBy = diac.CreatedBy;
                        final.CreatedOn = diac.CreatedOn;
                        final.Freight = diac.Freight;
                        final.FuelCharge = diac.FuelCharge;
                        final.IncidentalRemark = diac.IncidentalRemark;
                        final.Incidentals = diac.Incidentals;
                        final.OrderNumber = diac.OrderNumber;
                        final.OverWeightLengthGirthFee = diac.OverWeightLengthGirthFee;
                        final.ReceivingDate = diac.ReceivingDate;
                        final.Register = diac.Register;
                        final.SecurityAppendFee = diac.SecurityAppendFee;
                        final.SettleWeight = diac.SettleWeight;
                        final.ShippingMethodName = diac.ShippingMethodName;
                        final.Surcharge = diac.Surcharge;
                        final.TariffPrepayFee = diac.TariffPrepayFee;
                        final.TotalFee = diac.TotalFee;
                        final.UserName = diac.UserName;
                        final.VenderName = diac.VenderName;
                        final.WayBillNumber = diac.WayBillNumber;
                        final.Weight = diac.Weight;
                        if (final.ExpressDeliveryImportAccountCheckFinalID == 0)
                        {
                            ctx.FinalExpressDeliveryImportAccountChecks.Add(final);
                        }
                    }
                });
            ctx.SaveChanges();
            return true;
        }

        public bool DeliveryFeeAuditPass(List<int> ids, string userName, string remark, DateTime dt)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            List<DeliveryFee> fees = new List<DeliveryFee>();
            List<DeliveryFeeInfo> infos = new List<DeliveryFeeInfo>();
            List<DeliveryImportAccountCheck> imports = new List<DeliveryImportAccountCheck>();

            foreach (var id in ids)
            {
                if (!ctx.DeliveryFees.Any(t => t.DeliveryFeeID == id && t.Status == 1))
                {
                    throw new BusinessLogicException("只有状态为[未审核]的单才能被批量审核!");
                }
                var fee = ctx.DeliveryFees.FirstOrDefault(t => t.DeliveryFeeID == id && t.Status == 1);
                fee.Auditor = userName;
                fee.AuditorDate = DateTime.Now;
                fee.Status = 3;//已审核
                fee.LastUpdatedOn = DateTime.Now;
                fee.LastUpdatedBy = userName;
                fee.Remark = remark + fee.Remark;
                //ctx.DeliveryFees.(fee);
                //ctx.Entry(fee).State = System.Data.EntityState.Modified;
                //ctx.ObjectStateManager.ChangeObjectState(fee, System.Data.EntityState.Modified);

                var diac = new DeliveryImportAccountCheck();
                if (ctx.DeliveryImportAccountChecks.Any(t => t.UserName == userName.Trim() && t.WayBillNumber == fee.WayBillNumber && (t.Status == 0 || t.Status == 1)))
                {
                    //import data from vender
                    diac = ctx.DeliveryImportAccountChecks.FirstOrDefault(t => t.UserName == userName.Trim() && t.WayBillNumber == fee.WayBillNumber && (t.Status == 0 || t.Status == 1));
                    //diac.Status = 5;//已审核                    

                    var final = ctx.DeliveryImportAccountChecksFinal.FirstOrDefault(t => t.WayBillNumber == diac.WayBillNumber);
                    if (final == null)
                    {
                        final = new DeliveryImportAccountChecksFinal();
                    }

                    final.CountryName = diac.CountryName;
                    final.CreatedBy = diac.CreatedBy;
                    final.CreatedOn = diac.CreatedOn;
                    final.OrderNumber = diac.OrderNumber;
                    final.ReceivingDate = diac.ReceivingDate;
                    final.SettleWeight = diac.SettleWeight;
                    final.ShippingMethodName = diac.ShippingMethodName;
                    final.TotalFee = diac.TotalFee;
                    final.VenderName = diac.VenderName;
                    final.WayBillNumber = diac.WayBillNumber;
                    final.Weight = diac.Weight;

                    if (final.DeliveryImportAccountCheckFinalID == 0)
                    {
                        ctx.DeliveryImportAccountChecksFinal.Add(final);
                    }

                }
                else
                {
                    throw new BusinessLogicException("请先导入服务商数据然后在审核!");
                }

                //删除之前的最终记录
                ctx.ExecuteCommand("delete from DeliveryFeeInfos where [DeliveryFeeID]=@DeliveryFeeID And OperationType=@OperationType ",
                    new System.Data.SqlClient.SqlParameter("DeliveryFeeID", fee.DeliveryFeeID),
                    new System.Data.SqlClient.SqlParameter("OperationType", 3)
                    );
                //插入新的最终记录
                var info = new DeliveryFeeInfo();
                info.DeliveryFeeID = id;
                info.MoneyChangeTypeID = 2;
                info.FeeTypeID = 7;
                info.OperationType = 3;
                info.Amount = diac.TotalFee;//get from vender
                info.CreatedBy = userName;
                info.CreatedOn = DateTime.Now;
                info.LastUpdatedOn = DateTime.Now;
                info.LastUpdatedBy = userName;

                ctx.DeliveryFeeInfos.Add(info);
            }

            using (var scope = new TransactionScope())
            {
                ctx.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public string GetRemarkHistory(int id)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.Configuration.AutoDetectChangesEnabled = false;
            var query = from w in ctx.DeliveryFees
                        where w.DeliveryFeeID == id
                        select w.Remark;
            return query.FirstOrDefault();
        }

        private bool DeliveryFeeUpdateStatus(List<int> ids, string userName, string remark, int status, DateTime dt, int originalStatus)
        {
            if (ids.Count == 0) return true;

            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            string idsStr = string.Join(",", ids);
            try
            {
                ctx.ExecuteCommand(
                    string.Format(@" update [DeliveryFees] set [Status] = @Status , Remark = @Remark + Remark ,Auditor = @Auditor ,AuditorDate = @AuditorDate , LastUpdatedBy = @Auditor , LastUpdatedOn = @AuditorDate  where [DeliveryFeeID] in ({0}) and Status={1} ", idsStr, originalStatus.ToString()),
                    new SqlParameter("Status", status),
                    new SqlParameter("Remark", remark),
                    new SqlParameter("Auditor", userName),
                    new SqlParameter("AuditorDate", dt)
                    );

                //审核异常
                if (status == 2)
                {
                    foreach (var id in ids)
                    {
                        var fee = ctx.DeliveryFees.FirstOrDefault(t => t.DeliveryFeeID == id);

                        //import data from vender
                        var diac = ctx.DeliveryImportAccountChecks.FirstOrDefault(t => t.UserName == userName.Trim() &&
                            t.WayBillNumber == fee.WayBillNumber);
                        if (diac == null)
                        {
                            throw new BusinessLogicException("无法找到对应的服务商数据!无法审核异常!");
                        }
                        //diac.Status = 5;//已审核

                        var final = ctx.DeliveryImportAccountChecksFinal.FirstOrDefault(t => t.WayBillNumber == diac.WayBillNumber);
                        if (final == null)
                        {
                            final = new DeliveryImportAccountChecksFinal();
                        }

                        final.CountryName = diac.CountryName;
                        final.CreatedBy = diac.CreatedBy;
                        final.CreatedOn = diac.CreatedOn;
                        final.OrderNumber = diac.OrderNumber;
                        final.ReceivingDate = diac.ReceivingDate;
                        final.SettleWeight = diac.SettleWeight;
                        final.ShippingMethodName = diac.ShippingMethodName;
                        final.TotalFee = diac.TotalFee;
                        final.VenderName = diac.VenderName;
                        final.WayBillNumber = diac.WayBillNumber;
                        final.Weight = diac.Weight;

                        if (final.DeliveryImportAccountCheckFinalID == 0)
                        {
                            ctx.DeliveryImportAccountChecksFinal.Add(final);
                        }
                    }

                    ctx.SaveChanges();
                }


                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return false;
        }

        public DeliveryFeeAnomalyEditExt GetDeliveryFeeAnomalyEditExt(string wayBillNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var list = from r in ctx.DeliveryFees.Where(r => r.WayBillNumber == wayBillNumber)
                       join c in ctx.Customers on r.WayBillInfo.CustomerCode equals c.CustomerCode
                       join d in ctx.DeliveryImportAccountChecksFinal on r.WayBillNumber equals d.WayBillNumber into g
                       from l in g.DefaultIfEmpty()
                       select new DeliveryFeeAnomalyEditExt()
                       {
                           CustomerName = c.Name,
                           CustomerOrderNumber = r.WayBillInfo.CustomerOrderNumber,
                           Remark = r.Remark,
                           WayBillNumber = wayBillNumber,
                           SetWeightOriginal = r.SetWeight,

                           SetWeightVender = l.SettleWeight,

                           TotalFeeVender = l.TotalFee,

                           OtherFeeRemark = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 1).FirstOrDefault().Remark,

                           FreightOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),
                           RegisterOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),
                           FuelChargeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),
                           TariffPrepayFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),
                           SurchargeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),
                           OverWeightLengthGirthFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 9 && e.OperationType == 1).Sum(e => e.Amount),
                           SecurityAppendFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 8 && e.OperationType == 1).Sum(e => e.Amount),
                           AddedTaxFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 10 && e.OperationType == 1).Sum(e => e.Amount),
                           OtherFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 1).Sum(e => e.Amount),
                           TotalFeeOriginal = r.DeliveryFeeInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                           FreightFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 3).Sum(e => e.Amount),
                           RegisterFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 3).Sum(e => e.Amount),
                           FuelChargeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 3).Sum(e => e.Amount),
                           TariffPrepayFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 3).Sum(e => e.Amount),
                           SurchargeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 3).Sum(e => e.Amount),
                           OverWeightLengthGirthFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 9 && e.OperationType == 3).Sum(e => e.Amount),
                           SecurityAppendFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 8 && e.OperationType == 3).Sum(e => e.Amount),
                           AddedTaxFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 10 && e.OperationType == 3).Sum(e => e.Amount),
                           OtherFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 3).Sum(e => e.Amount),
                           TotalFeeFinal = r.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),

                       };

            return list.FirstOrDefault();
        }

        public DeliveryFeeAnomalyEditExt GetDeliveryFeeExpressAnomalyEditExt(string wayBillNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var list = from r in ctx.DeliveryFees.Where(r => r.WayBillNumber == wayBillNumber)
                       join c in ctx.Customers on r.WayBillInfo.CustomerCode equals c.CustomerCode
                       join d in ctx.FinalExpressDeliveryImportAccountChecks on r.WayBillNumber equals d.WayBillNumber into g
                       from l in g.DefaultIfEmpty()
                       select new DeliveryFeeAnomalyEditExt()
                       {
                           CustomerName = c.Name,
                           CustomerOrderNumber = r.WayBillInfo.CustomerOrderNumber,
                           Remark = r.Remark,
                           WayBillNumber = wayBillNumber,
                           SetWeightOriginal = r.SetWeight,

                           SetWeightVender = l.SettleWeight,

                           FreightVender = l.Freight,
                           TotalFeeVender = l.TotalFee,
                           RegisterVender = l.Register,
                           FuelChargeVender = l.FuelCharge,
                           TariffPrepayFeeVender = l.TariffPrepayFee,
                           SurchargeVender = l.Surcharge,
                           OverWeightLengthGirthFeeVender = l.OverWeightLengthGirthFee,
                           AddedTaxFeeVender = l.AddedTaxFee,
                           SecurityAppendFeeVender = l.SecurityAppendFee,
                           OtherFeeVender = l.Incidentals,

                           OtherFeeRemarkVender = l.IncidentalRemark,
                           OtherFeeRemark = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 1).FirstOrDefault().Remark,

                           FreightOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),
                           RegisterOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),
                           FuelChargeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),
                           TariffPrepayFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),
                           SurchargeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),
                           OverWeightLengthGirthFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 9 && e.OperationType == 1).Sum(e => e.Amount),
                           SecurityAppendFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 8 && e.OperationType == 1).Sum(e => e.Amount),
                           AddedTaxFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 10 && e.OperationType == 1).Sum(e => e.Amount),
                           OtherFeeOriginal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 1).Sum(e => e.Amount),
                           TotalFeeOriginal = r.DeliveryFeeInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                           FreightFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 3).Sum(e => e.Amount),
                           RegisterFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 3).Sum(e => e.Amount),
                           FuelChargeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 3).Sum(e => e.Amount),
                           TariffPrepayFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 3).Sum(e => e.Amount),
                           SurchargeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 3).Sum(e => e.Amount),
                           OverWeightLengthGirthFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 9 && e.OperationType == 3).Sum(e => e.Amount),
                           SecurityAppendFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 8 && e.OperationType == 3).Sum(e => e.Amount),
                           AddedTaxFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 10 && e.OperationType == 3).Sum(e => e.Amount),
                           OtherFeeFinal = r.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 3).Sum(e => e.Amount),
                           TotalFeeFinal = r.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),

                       };

            return list.FirstOrDefault();
        }

        public List<DeliveryDeviation> GetVenderDeliveryDeviation(string venderName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.DeliveryDeviations.AsNoTracking();

            //特殊和默认的
            return ctx.DeliveryDeviations.Where(t => t.VenderName == venderName).ToList();
        }

        public List<DeliveryDeviation> GetVenderDeliveryDeviationByVenderCode(string venderCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.DeliveryDeviations.AsNoTracking();

            //特殊和默认的
            return ctx.DeliveryDeviations.Where(t => t.VenderCode == venderCode).ToList();
        }

        public List<WayBillNumberExtSimple> GetLocalOrderInfo(List<string> orderOrTrackNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;

            Expression<Func<DeliveryFee, bool>> filter = o => true;
            filter = filter.And(
                p => orderOrTrackNumbers.Contains(p.WayBillNumber) ||
                    orderOrTrackNumbers.Contains(p.Trackingnumber));

            var list = from w in ctx.DeliveryFees.Where(filter)
                       join c in ctx.WayBillInfos on w.WayBillInfo.WayBillNumber equals c.WayBillNumber
                       orderby w.AuditorDate descending
                       select new WayBillNumberExtSimple()
                       {
                           SetWeight = w.SetWeight,
                           WayBillNumber = w.WayBillNumber,
                           Trackingnumber = w.Trackingnumber,
                           TotalFee = w.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount)
                       };

            return list.ToList();
        }
        public bool DeleteDeliveryImportAccountChecksTemp(string userName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.ExecuteCommand("delete from DeliveryImportAccountChecksTemp where CreatedBy=@UserName ", new SqlParameter("UserName", userName.Trim()));
            ctx.SaveChanges();
            return true;
        }
        public bool DeleteExpressDeliveryImportAccountChecksTemp(string userName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.ExecuteCommand("delete from ExpressDeliveryImportAccountChecksTemp where CreatedBy=@UserName ", new SqlParameter("UserName", userName.Trim()));
            ctx.SaveChanges();
            return true;
        }
        //小包
        public bool SaveDeliveryImportAccountChecks(List<DeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var userName = importData.FirstOrDefault().CreatedBy;
            var sqlvenderCode = new SqlParameter { ParameterName = "venderCode", Value = venderCode, DbType = DbType.String };
            var sqlselectOrderNo = new SqlParameter { ParameterName = "selectOrderNo", Value = selectOrderNo, DbType = DbType.Int32 };
            var sqluserName = new SqlParameter { ParameterName = "userName", Value = userName, DbType = DbType.String };
            if (ctx != null)
            {
                Log.Info("开始验证偏差率");
                var obj = ctx.ExecuteStoredProcedureList<DeliveryFeeCheckModel>("P_DeliveryDetailAuditVerificationDeviationRate @venderCode,@selectOrderNo,@userName"
                                                        , sqlvenderCode, sqlselectOrderNo, sqluserName);
                if (obj != null && obj.Count > 0)
                {
                    foreach (var p in importData)
                    {
                        var l = obj.FirstOrDefault(k => k.OrderNumber == p.OrderNumber);
                        if (l != null)
                        {
                            if (l.Status != 2)
                            {
                                p.IsTrue = true;
                            }
                            else
                            {
                                p.IsTrue = false;
                                p.ErrorReason += l.Remark;
                            }
                        }
                        else
                        {
                            p.IsTrue = false;
                            p.ErrorReason += "上传保存失败";
                        }
                    }
                }
                else
                {
                    foreach (var p in importData)
                    {
                        p.IsTrue = false;
                        p.ErrorReason += "上传保存失败";
                    }
                }
                Log.Info("完成验证偏差率");
            }

            #region 老方法
            ////清空所有上次数据
            //var userName = importData.FirstOrDefault().UserName;
            //ctx.ExecuteCommand("delete from DeliveryImportAccountChecks where [UserName]=@UserName ", new SqlParameter("UserName", userName.Trim()));
            //ctx.SaveChanges();
            ////偏差率
            //var deliveryDeviations = new List<DeliveryDeviation>();
            ////判断是否是同一服务商
            //var venderCode = string.Empty;
            //foreach (var p in importData)
            //{
            //    var obj =
            //        ctx.DeliveryFees.FirstOrDefault(
            //            t =>
            //            t.Trackingnumber == p.OrderNumber || t.CustomerOrderNumber == p.OrderNumber ||
            //            t.WayBillNumber == p.OrderNumber);
            //    if (obj != null)
            //    {
            //        p.WayBillNumber = obj.WayBillNumber;
            //        p.IsTrue = true;
            //        if (!deliveryDeviations.Any())
            //        {
            //            deliveryDeviations = GetVenderDeliveryDeviationByVenderCode(obj.VenderCode);
            //            if (!deliveryDeviations.Any())
            //            {
            //                throw new BusinessLogicException(string.Format("服务商[{0}]未配置对应的偏差率,无法进行审核操作.", p.VenderName));
            //            }
            //        }
            //        if (venderCode.IsNullOrWhiteSpace())
            //        {
            //            venderCode = obj.VenderCode;
            //        }
            //        else
            //        {
            //            if (venderCode != obj.VenderCode)
            //            {
            //                p.IsTrue = false;
            //                p.ErrorReason += " 不是同一个服务商；";
            //            }
            //        }
            //        if (p.IsTrue)
            //        {
            //            if (!CheckDeviationRate(obj, deliveryDeviations, p.SettleWeight.Value, p.TotalFee.Value))
            //            {
            //                p.Status = 1;
            //            }
            //            ctx.DeliveryImportAccountChecks.Add(p);
            //        }
            //    }
            //    else
            //    {
            //        p.IsTrue = false;
            //        p.ErrorReason += " 运单不存在;";
            //    }
            //}
            //if (importData.Any(t => t.IsTrue == true))
            //{
            //    ctx.SaveChanges();
            //} 
            #endregion
            Log.Info("完成保存");
            return true;
        }
        //快递
        public bool SaveDeliveryImportAccountChecks(List<ExpressDeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var userName = importData.FirstOrDefault().CreatedBy;
            var sqlvenderCode = new SqlParameter { ParameterName = "venderCode", Value = venderCode, DbType = DbType.String };
            var sqlselectOrderNo = new SqlParameter { ParameterName = "selectOrderNo", Value = selectOrderNo, DbType = DbType.Int32 };
            var sqluserName = new SqlParameter { ParameterName = "userName", Value = userName, DbType = DbType.String };

            if (ctx != null)
            {
                Log.Info("开始验证偏差率");
                var obj = ctx.ExecuteStoredProcedureList<DeliveryFeeCheckModel>("P_ExpressDeliveryDetailAuditVerificationDeviationRate @venderCode,@selectOrderNo,@userName"
                                                        , sqlvenderCode, sqlselectOrderNo, sqluserName);
                if (obj != null && obj.Count > 0)
                {
                    foreach (var p in importData)
                    {
                        var l = obj.FirstOrDefault(k => k.OrderNumber == p.OrderNumber);
                        if (l != null)
                        {
                            if (l.Status != 2)
                            {
                                p.IsTrue = true;
                            }
                            else
                            {
                                p.IsTrue = false;
                                p.ErrorReason += l.Remark;
                            }
                        }
                        else
                        {
                            p.IsTrue = false;
                            p.ErrorReason += "上传保存失败";
                        }
                    }
                }
                else
                {
                    foreach (var p in importData)
                    {
                        p.IsTrue = false;
                        p.ErrorReason += "上传保存失败";
                    }
                }
                Log.Info("完成验证偏差率");
            }
            #region 老方法
            ////清空所有上次数据
            //Log.Info("开始删除以前导入数据");
            //var userName = importData.FirstOrDefault().UserName;
            //ctx.ExecuteCommand("delete from ExpressDeliveryImportAccountChecks where [UserName]=@UserName ", new SqlParameter("UserName", userName.Trim()));
            //ctx.SaveChanges();
            //Log.Info("完成删除以前导入数据");
            ////偏差率
            //var deliveryDeviations = new List<DeliveryDeviation>();
            ////判断是否是同一服务商
            //var venderCode = string.Empty;
            //Log.Info("开始验证偏差率");
            //foreach (var p in importData)
            //{
            //    var obj =
            //        ctx.DeliveryFees.FirstOrDefault(
            //            t =>
            //            t.Trackingnumber == p.OrderNumber || t.CustomerOrderNumber == p.OrderNumber ||
            //            t.WayBillNumber == p.OrderNumber);
            //    if (obj != null)
            //    {
            //        p.WayBillNumber = obj.WayBillNumber;
            //        p.IsTrue = true;
            //        if (!deliveryDeviations.Any())
            //        {
            //            deliveryDeviations = GetVenderDeliveryDeviationByVenderCode(obj.VenderCode);
            //            if (!deliveryDeviations.Any())
            //            {
            //                throw new BusinessLogicException(string.Format("服务商[{0}]未配置对应的偏差率,无法进行审核操作.", p.VenderName));
            //            }
            //        }
            //        if (venderCode.IsNullOrWhiteSpace())
            //        {
            //            venderCode = obj.VenderCode;
            //        }
            //        else
            //        {
            //            if (venderCode != obj.VenderCode)
            //            {
            //                p.IsTrue = false;
            //                p.ErrorReason += " 不是同一个服务商；";
            //            }
            //        }
            //        if (p.IsTrue)
            //        {
            //            if (!CheckDeviationRate(obj, deliveryDeviations, p.SettleWeight.Value, p.TotalFee))
            //            {
            //                p.Status = 1;
            //            }
            //            ctx.ExpressDeliveryImportAccountChecks.Add(p);
            //        }
            //    }
            //    else
            //    {
            //        p.IsTrue = false;
            //        p.ErrorReason += " 运单不存在;";
            //    }
            //}
            //Log.Info("完成验证偏差率");
            //Log.Info("开始保存");
            //if (importData.Any(t => t.IsTrue == true))
            //{
            //    ctx.SaveChanges();
            //}
            //Log.Info("完成保存"); 
            #endregion
            return true;
        }
        //检查偏差率
        public bool CheckDeviationRate(DeliveryFee deliveryFee, List<DeliveryDeviation> deliveryDeviations, decimal settleWeight, decimal totalFee)
        {
            var deliveryDeviationsCount = 0;
            #region 重量偏差
            //重量  偏差率
            if (deliveryDeviations.Any(t => t.DeviationType == 2 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationRate.HasValue))
            {
                var deviationRate = deliveryDeviations.FirstOrDefault(t => t.DeviationType == 2 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationRate.HasValue)
                    .DeviationRate.Value;
                deliveryDeviationsCount++;
                if (Math.Abs((decimal)((settleWeight - deliveryFee.SetWeight) / deliveryFee.SetWeight)) > deviationRate)
                {
                    return false;
                }
            }
            //重量  偏差值
            if (deliveryDeviations.Any(t => t.DeviationType == 2 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationValue.HasValue))
            {
                var deviationValue = deliveryDeviations.FirstOrDefault(t => t.DeviationType == 2 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationValue.HasValue)
                    .DeviationValue.Value;
                deliveryDeviationsCount++;
                if (Math.Abs((decimal)(settleWeight - deliveryFee.SetWeight)) > deviationValue)
                {
                    return false;
                }
            }
            if (deliveryDeviationsCount == 0)
            {
                throw new BusinessLogicException(string.Format("服务商[{0}]未配置[{1}]的重量偏差率/偏差值,无法进行审核操作.", deliveryFee.VenderName, deliveryFee.ShippingmethodName));
            }
            else
            {
                deliveryDeviationsCount = 0;
            }
            #endregion

            #region 总费用偏差
            var localTotalFee = deliveryFee.DeliveryFeeInfos.Where(t => t.DeliveryFeeID == deliveryFee.DeliveryFeeID && t.OperationType == 1).Sum(t => t.Amount);

            if (deliveryDeviations.Any(t => t.DeviationType == 1 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationRate.HasValue))
            {
                var deviationRate = deliveryDeviations.FirstOrDefault(t => t.DeviationType == 1 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationRate.HasValue)
                    .DeviationRate.Value;
                deliveryDeviationsCount++;
                if (Math.Abs((decimal)((totalFee - localTotalFee) / localTotalFee)) > deviationRate)
                {
                    return false;
                }
            }
            if (deliveryDeviations.Any(t => t.DeviationType == 1 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationValue.HasValue))
            {
                var deviationValue = deliveryDeviations.FirstOrDefault(t => t.DeviationType == 1 && t.ShippingmethodID == deliveryFee.ShippingmethodID && t.DeviationValue.HasValue)
                    .DeviationValue.Value;
                deliveryDeviationsCount++;
                if (Math.Abs((decimal)(totalFee - localTotalFee)) > deviationValue)
                {
                    return false;
                }
            }

            if (deliveryDeviationsCount == 0)
            {
                throw new BusinessLogicException(string.Format("服务商[{0}]未配置[{1}]的运费偏差率/偏差值,无法进行审核操作.", deliveryFee.VenderName, deliveryFee.ShippingmethodName));
            }
            #endregion
            return true;
        }
        //小包
        public PagedList<DeliveryFeeExt> ImportExcelWait2Audit(DeliveryReviewParam param)
        {
            return GetWait2Audit(param).ToPagedList(param.Page, param.PageSize);
        }

        private IQueryable<DeliveryFeeExt> GetWait2Audit(DeliveryReviewParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;

            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            Expression<Func<DeliveryFee, bool>> filter = o => true;
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                //int shippingMethodId = 0;
                //if (!string.IsNullOrWhiteSpace(param.ShippingMethodId))
                //{
                //    if (!Int32.TryParse(param.ShippingMethodId, out shippingMethodId))
                //    {
                //        shippingMethodId = 0;
                //    }
                //}
                filter = filter.AndIf(r => r.Status == param.Status.Value, param.Status.HasValue && param.Status.Value != 0) //只显示发货未审核状态
                    //.AndIf(r => r.ShippingmethodID == shippingMethodId, shippingMethodId!=0)
                    //.AndIf(r => param.ShippingMethodIds.Contains(r.ShippingmethodID), param.ShippingMethodIds != null && param.ShippingMethodIds.Length > 0)
                    //.AndIf(r => r.VenderCode == param.VenderCode, !string.IsNullOrWhiteSpace(param.VenderCode))
                    //.AndIf(r => r.WayBillInfo.OutStorageCreatedOn >= param.StartTime, param.StartTime.HasValue)
                    //.AndIf(r => r.WayBillInfo.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue)
                               ;
            }

            var list = from w in ctx.DeliveryFees.Where(filter)
                       join d in ctx.DeliveryImportAccountChecks on w.WayBillNumber equals d.WayBillNumber
                       //join c in ctx.WayBillInfos on d.WayBillNumber equals c.WayBillNumber        
                       where //(new int?[] { 0, 1 }).Contains(d.Status) && //待审核
                        d.UserName == param.UserName //上传用户过滤                       
                       orderby d.Status descending //异常的前面 d.CreatedOn
                       select new DeliveryFeeExt()
                       {
                           VenderCode = w.VenderCode,
                           //AprroveWeight
                           Auditor = w.Auditor,
                           AprroveWeight = w.AprroveWeight,
                           AuditorDate = w.AuditorDate,
                           CreatedBy = w.CreatedBy,
                           CreatedOn = w.CreatedOn,
                           CustomerOrderNumber = w.CustomerOrderNumber,
                           DeliveryFeeID = w.DeliveryFeeID,
                           LastUpdatedBy = w.LastUpdatedBy,
                           LastUpdatedOn = w.LastUpdatedOn,
                           Remark = w.Remark,
                           SetWeight = w.WayBillInfo.SettleWeight,
                           ShippingmethodID = w.ShippingmethodID,
                           ShippingmethodName = w.ShippingmethodName,
                           Status = w.Status,
                           Trackingnumber = w.Trackingnumber,
                           VenderName = w.VenderName,
                           VenderId = w.VenderId,
                           WayBillNumber = w.WayBillNumber,

                           Weight = w.WayBillInfo.Weight,
                           OutStorageCreatedOn = w.WayBillInfo.OutStorageCreatedOn,
                           CountryCode = w.WayBillInfo.CountryCode,

                           //运费
                           Freight = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),

                           //挂号费
                           Register = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),

                           //燃油费
                           FuelCharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),

                           //关税预付服务费
                           TariffPrepayFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),

                           //附加费
                           Surcharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),

                           //总费用
                           TotalFee = w.DeliveryFeeInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                           //最终总费用
                           TotalFeeFinal = w.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),

                           //服务商数据
                           VenderData = d

                       };

            return list;
        }
        //快递
        public PagedList<DeliveryFeeExpressExt> ExpressImportWait2Audit(DeliveryReviewParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            Expression<Func<DeliveryFee, bool>> filter = o => true;
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                //int shippingMethodId = 0;
                //if (!string.IsNullOrWhiteSpace(param.ShippingMethodId))
                //{
                //    if (!Int32.TryParse(param.ShippingMethodId, out shippingMethodId))
                //    {
                //        shippingMethodId = 0;
                //    }
                //}
                filter = filter.AndIf(r => r.Status == param.Status.Value, param.Status.HasValue && param.Status.Value != 0) //只显示发货未审核状态
                    //.AndIf(r => r.ShippingmethodID == shippingMethodId, shippingMethodId != 0)
                    //.AndIf(r => param.ShippingMethodIds.Contains(r.ShippingmethodID), param.ShippingMethodIds != null && param.ShippingMethodIds.Length > 0)
                    //.AndIf(r => r.VenderCode == param.VenderCode, !string.IsNullOrWhiteSpace(param.VenderCode))
                    //.AndIf(r => r.WayBillInfo.OutStorageCreatedOn >= param.StartTime, param.StartTime.HasValue)
                    //.AndIf(r => r.WayBillInfo.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue)
                               ;
            }

            var list = (from w in ctx.DeliveryFees.Where(filter)
                        join d in ctx.ExpressDeliveryImportAccountChecks on w.WayBillNumber equals d.WayBillNumber
                        //join c in ctx.WayBillInfos on d.WayBillNumber equals c.WayBillNumber        
                        where //(new int?[] { 0, 1 }).Contains(d.Status) && //待审核 
                        d.UserName == param.UserName //上传用户过滤                       
                        orderby d.Status descending //异常的前面 d.CreatedOn
                        select new DeliveryFeeExpressExt()
                        {
                            VenderCode = w.VenderCode,
                            Auditor = w.Auditor,
                            AprroveWeight = w.AprroveWeight,
                            AuditorDate = w.AuditorDate,
                            CreatedBy = w.CreatedBy,
                            CreatedOn = w.CreatedOn,
                            CustomerOrderNumber = w.CustomerOrderNumber,
                            DeliveryFeeID = w.DeliveryFeeID,
                            LastUpdatedBy = w.LastUpdatedBy,
                            LastUpdatedOn = w.LastUpdatedOn,
                            Remark = w.Remark,
                            SetWeight = w.WayBillInfo.SettleWeight,
                            ShippingmethodID = w.ShippingmethodID,
                            ShippingmethodName = w.ShippingmethodName,
                            Status = w.Status,
                            Trackingnumber = w.Trackingnumber,
                            VenderName = w.VenderName,
                            VenderId = w.VenderId,
                            WayBillNumber = w.WayBillNumber,

                            Weight = w.WayBillInfo.Weight,
                            OutStorageCreatedOn = w.WayBillInfo.OutStorageCreatedOn,
                            CountryCode = w.WayBillInfo.CountryCode,

                            //运费
                            Freight = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 3 && e.OperationType == 1).Sum(e => e.Amount),

                            //挂号费
                            Register = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 4 && e.OperationType == 1).Sum(e => e.Amount),

                            //燃油费
                            FuelCharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 5 && e.OperationType == 1).Sum(e => e.Amount),

                            //关税预付服务费
                            TariffPrepayFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 6 && e.OperationType == 1).Sum(e => e.Amount),

                            //附加费
                            Surcharge = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 2 && e.OperationType == 1).Sum(e => e.Amount),
                            //超长超重超周长费
                            OverWeightLengthGirthFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 9 && e.OperationType == 1).Sum(e => e.Amount),
                            //安全附加费
                            SecurityAppendFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 8 && e.OperationType == 1).Sum(e => e.Amount),
                            //增值税费
                            AddedTaxFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 10 && e.OperationType == 1).Sum(e => e.Amount),
                            //杂费
                            OtherFee = w.DeliveryFeeInfos.Where(e => e.FeeTypeID == 11 && e.OperationType == 1).Sum(e => e.Amount),

                            //总费用
                            TotalFee = w.DeliveryFeeInfos.Where(e => e.OperationType == 1).Sum(e => e.Amount),

                            //最终总费用
                            TotalFeeFinal = w.DeliveryFeeInfos.Where(e => e.OperationType == 3).Sum(e => e.Amount),
                            //杂费备注
                            OtherFeeRemark = w.DeliveryFeeInfos.FirstOrDefault(e => e.FeeTypeID == 11 && e.OperationType == 1).Remark,
                            //服务商数据
                            VenderData = d

                        }).ToPagedList(param.Page, param.PageSize);
            return list;
        }

        public decimal DeliveryFeeGetTotalFinalSum(DeliveryReviewParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;

            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            Expression<Func<DeliveryFee, bool>> filter = o => true;

            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillInfo.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue)
                    //.AndIf(r => r.ShippingmethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                                .AndIf(r => param.ShippingMethodIds.Contains(r.ShippingmethodID), param.ShippingMethodIds != null && param.ShippingMethodIds.Length > 0)
                               .AndIf(r => r.VenderCode == param.VenderCode, !string.IsNullOrWhiteSpace(param.VenderCode))
                               .AndIf(r => r.WayBillInfo.OutStorageCreatedOn >= param.StartTime, param.StartTime.HasValue)
                               .AndIf(r => r.WayBillInfo.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue);
            }

            var list = from w in ctx.DeliveryFees.Where(filter)
                       //orderby w.AuditorDate descending
                       select w.DeliveryFeeInfos
                            .Where(e => e.OperationType == 3)
                            .Sum(e => e.Amount);
            decimal? sum = list.Sum(s => s);
            return sum != null ? sum.Value : 0;
        }
    }
}
