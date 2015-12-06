using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;


namespace LMS.Services.FinancialServices
{
    public interface IFinancialService
    {
        IPagedList<ReceivingExpenseExt> GetReceivingExpensePagedList(FinancialParam financialParam);

        /// <summary>
        /// 财务收货费用
        /// Add By zhengsong
        /// Time:2014-06-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<InFeeInfoAuditListExt> GetAuditPagedList(InFeeInfoAuditParam param);

        IList<InFeeInfoAuditListExt> GetAuditList(InFeeInfoAuditParam param);

        IList<InFeeInfoAuditListExt> GetInFeeInfoExport(InFeeInfoAuditParam param);
        int GetInFeeInfoExportTotalCount(InFeeInfoAuditParam param);
        /// <summary>
        /// 批量审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        void BatchAudited(List<string> wayBillNumberList);

        /// <summary>
        /// 反审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        void BatchAntiAudit(List<string> wayBillNumberList);

        #region 发货明细审核

        PagedList<DeliveryFeeExt> DeliveryFeeSearch(DeliveryReviewParam param);

        PagedList<DeliveryFeeExt> GetDeliveryFeeAnomaly(DeliveryReviewParam param);

        PagedList<DeliveryFeeExt> GetDeliveryFeeExpressAnomaly(DeliveryReviewParam param);

        PagedList<DeliveryFeeExt> ExpressDeliveryFeeSearch(DeliveryReviewParam param);

        List<DeliveryFeeExt> ExportExcel(DeliveryReviewParam param);

        List<DeliveryFeeExt> ExportExcel(DeliveryReviewParam param, bool enableStatusFilter, bool isExpress);      
        string GetRemarkHistory(int id);

        bool ReverseAudit(List<int> ids, string userName, string remark,DateTime dt);
        bool DeliveryFeeAuditError(List<int> ids, string userName, string error, DateTime dt);
        bool DeliveryFeeAuditPass(List<int> ids, string userName, string remark, DateTime dt);

        List<DeliveryDeviation> GetVenderDeliveryDeviation(string venderName);

        List<WayBillNumberExtSimple> GetLocalOrderInfo(List<string> orderOrTrackNumbers);

        bool SaveDeliveryImportAccountChecks(List<DeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo);

        bool SaveDeliveryImportAccountChecks(List<ExpressDeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo);
        PagedList<DeliveryFeeExt> ImportExcelWait2Audit(DeliveryReviewParam param);
        PagedList<DeliveryFeeExpressExt> ExpressImportWait2Audit(DeliveryReviewParam param);
        decimal DeliveryFeeGetTotalFinalSum(DeliveryReviewParam para);
        #endregion

        ReceivingExpensesEditExt GetReceivingExpensesEditExt(string wayBillNumber);

        void EditReceivingExpensesEditExt(ReceivingExpensesEditExt receivingExpensesEditExt);

        /// <summary>
        /// 审核异常
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="auditAnomalyList"></param>
        /// <returns></returns>
        bool UpdateAuditAnomaly(List<AuditAnomalyExt> auditAnomalyList);

        /// <summary>
        /// 出账单
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateOutBilled(List<string> wayBillNumberList, ReceivingBillExt model);

        void EditDeliveryFeeAnomalyEditExt(DeliveryFeeAnomalyEditExt deliveryFeeAnomalyEditExt);

        /// <summary>
        /// 计算运单费用
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        ReceivingExpenseExt GetWayBillPrice(string wayBillNumber);

        /// <summary>
        /// 计算服务商价格
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        DeliveryFeeExt GetWayBillVenderPrice(string wayBillNumber);


		//查询应收应付分析报表 add bu yungchu
		IPagedList<ChargePayAnalysesExt> GetChragePayAnayeseRecordPagedList(ChragePayAnalyeseParam param, out int TotalRecord, out int TotalPage);

		//导出应收应付分析报表 add by yungchu
		List<ChargePayAnalysesExt> GetExportChargePayAnalysesList(ChragePayAnalyeseParam param);


        /// <summary>
        /// 取消异常
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        void WayBillCancelAnomaly(string[] wayBillNumbers);

        DeliveryFeeAnomalyEditExt GetDeliveryFeeAnomalyEditExt(string wayBillNumber);

        DeliveryFeeAnomalyEditExt GetDeliveryFeeExpressAnomalyEditExt(string wayBillNumber);

        IPagedList<ReceivingBill> GetReceivingBillPagedList(ReceivingBillParam param);

        /// <summary>
        /// Add By zhengsong
        /// 判断是否生成运费
        /// Time:2014-0712
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        decimal GetCountFeel(string wayBillNumber);

        /// <summary>
        /// 获取ReceivingExpenseID
        /// Add By zhengsong
        /// Time:2014-07-12
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        int GetReceivingExpenseID(string wayBillNumber);

        /// <summary>
        /// 修改收费费用审核详细表
        /// Add By zhengsong
        /// Time:2014-07-14
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="userUame"></param>
        /// <param name="returnFee"></param>
        void UpdateReceivingExpenseInfo(string wayBillNumber, string userUame, bool returnFee = true);


        /// <summary>
        /// 获取总共收费费用
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        InFeeTotalInfoExt GetInFeeTotalInfo(string number);

		/// <summary>
		/// 获取财务job错误日志记录
		/// yungchu
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		IPagedList<JobErrorLogExt> GetJobErrorLogsPagedList(JobErrorLogsParam param);

		/// <summary>
		/// 增删修查发货审核偏差率
		/// yungchu
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool AddDeliveryDeviations(List<DeliveryDeviation> listEntity);
		bool DeleteDeliveryDeviations(int id);
		bool UpdateDeliveryDeviations(List<DeliveryDeviation> listEntity,int id);
	    DeliveryDeviationExt GetDeliveryDeviationInfo(int id);


		IPagedList<DeliveryDeviationExt> GetDeliveryDeviationPagedList(DeliveryDeviationParam param);
        CustomerOrderInfo GetCustomerOrderInfoByNumber(string customerOrderInfoNumber);
        Customer GetCustomerByCode(string customerCode);

        /// <summary>
        /// 计算运单费用
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        List<PriceProviderResult> GetPriceProviderResult(string[] wayBillNumbers);

        /// <summary>
        /// 计算运单费用
        /// </summary>
        PriceProviderExtResult GetPriceProviderResult(string wayBillNumber);
        /// <summary>
        /// 快递批量审核异常
        /// </summary>
        /// <param name="dataParams"></param>
        /// <returns></returns>
        bool ExpressDeliveryFeeAuditError(List<AuditParam> dataParams, string userName);
        bool ExpressDeliveryFeeAuditPass(List<AuditParam> dataParams, string userName);
    }
}
