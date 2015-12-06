using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Repository
{
    public partial interface IDeliveryFeeRepository
    {
        PagedList<DeliveryFeeExt> Search(DeliveryReviewParam param, bool enableStatusFilter = false, bool isExpress=false);

        List<DeliveryFeeExt> Export(DeliveryReviewParam param, bool enableStatusFilter = false, bool isExpress = false);


        bool ReverseAudit(List<int> ids, string userName, string remark, DateTime dt);
        bool DeliveryFeeAuditError(List<int> ids, string userName, string error, DateTime dt);
        bool DeliveryFeeAuditPass(List<int> ids, string userName, string remark, DateTime dt);

        //bool ReverseAudit(List<int> ids,string remark);


        DeliveryFeeAnomalyEditExt GetDeliveryFeeAnomalyEditExt(string wayBillNumber);

        DeliveryFeeAnomalyEditExt GetDeliveryFeeExpressAnomalyEditExt(string wayBillNumber);

        string GetRemarkHistory(int id);
        //bool DeliveryFeeAuditError(List<int> ids, string error);

        List<DeliveryDeviation> GetVenderDeliveryDeviation(string venderName);
        List<DeliveryDeviation> GetVenderDeliveryDeviationByVenderCode(string venderCode);
        List<WayBillNumberExtSimple> GetLocalOrderInfo(List<string> orderOrTrackNumbers);

        bool SaveDeliveryImportAccountChecks(List<DeliveryImportAccountCheck> importData,string venderCode, int selectOrderNo);

        bool SaveDeliveryImportAccountChecks(List<ExpressDeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo);
        PagedList<DeliveryFeeExt> ImportExcelWait2Audit(DeliveryReviewParam param);
        PagedList<DeliveryFeeExpressExt> ExpressImportWait2Audit(DeliveryReviewParam param);

        //bool DeliveryFeeAuditPass(List<int> ids, string userName, string remark);

        decimal DeliveryFeeGetTotalFinalSum(DeliveryReviewParam para);

        bool ExpressDeliveryFeeAuditError(List<AuditParam> dataParams, string userName);
        bool ExpressDeliveryFeeAuditPass(List<AuditParam> dataParams, string userName);
        bool DeleteDeliveryImportAccountChecksTemp(string userName);
        bool DeleteExpressDeliveryImportAccountChecksTemp(string userName);
    }
}
