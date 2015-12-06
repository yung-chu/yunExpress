using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity.ExtModel;

namespace LMS.Services.InStorageServices
{
    public interface IInStorageService
    {
        /// <summary>
        /// 获取货物类型列表
        /// </summary>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        List<GoodsTypeInfo> GetGoodsTypeList(bool? isDelete);
        /// <summary>
        /// 通过运单号和跟踪号和真实跟踪号和客户订单号查询运单信息
        /// </summary>
        /// <param name="WayBillNumber"></param>
        /// <returns></returns>
        WayBillInfo GetWayBillInfo(string WayBillNumber);

        /// <summary>
        /// 通过运单号和跟踪号与客户编码与真实跟踪号查询运单信息
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="CustomerCode"></param>
        /// <returns></returns>
        WayBillInfo GetWayBillInfo(string numberStr, string CustomerCode);


	    /// <summary>
        /// 创建入仓单
        /// </summary>
        /// <param name="createInStorageExt"></param>
        void CreateInStorage(CreateInStorageExt createInStorageExt);

        /// <summary>
        /// 异步入仓保存
        /// </summary>
        /// <param name="task"></param>
        void CreateInStorageAsync(Task task);

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="taskType">类型</param>
        /// <param name="status">状态</param>
        /// <param name="taskKey">key</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        IPagedList<TaskModel> GetTaskList(int taskType, int status, string taskKey, int pageIndex, int pageSize = 50);

        List<InStorageTotalModel> GetInStorageTotals(string InStorageId);
        string GetShippingMethodName(string InStorageId);

        bool Retry(long[] ids);

        /// <summary>
        /// CS版创建入仓单，和包裹明细 add by huhaiyou 2014-4-28
        /// </summary>
        /// <param name="createInStorageExtCS"></param>
        void CreateInStorageCS(CreateInStorageExtCS createInStorageExtCS);
        /// <summary>
        /// 获取入仓单信息
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        InStorageInfo GetInStorageInfo(string InStorageId);
        /// <summary>
        /// 查询入仓信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<InStorageInfo> GetInStoragePagedList(InStorageListSearchParam param);


        /// <summary>
        /// 获取现结客户的所有未生成结算单的入仓信息
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        List<InStorageInfo> GetInStorageNoSettlementList(string customerCode);

        /// <summary>
        /// 打印快递入仓发票
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IList<PrintInStorageInvoiceExt> GetPrintInStorageInvoice(PrintInStorageInvoiceParam param);

        /// <summary>
        /// 获取物流快递打单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<ExpressPrintWayBillExt> GetExpressPrintWayBillList(ExpressPrintWayBillParam param);

        /// <summary>
        /// 获取入仓列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<InStorageInfoExt> GetInStorageInfoExtPagedList(InStorageListSearchParam param);

	    /// <summary>
	    /// 获取快递打单日志列表
	    /// add by yungchu
	    /// </summary>
	    /// <param name="wayBillNumbers"></param>
	    /// <returns></returns>
	    List<WayBillPrintLog> GetWayBillPrintLogList(string wayBillNumber);

	    /// <summary>
	    /// 增加快递打单日志数据
	    /// add by yungchu
	    /// </summary>
		/// <param name="AddWayBillPrintLog"></param>
	    /// <returns></returns>
	    bool AddWayBillPrintLog(WayBillPrintLog param);
	


		IEnumerable<WayBillInfo> GetWayBillByWayBillNumbers(IEnumerable<string> wayBillNumbers);
        void BatchUpdateWayBillByVenderCode(IEnumerable<WayBillInfo> wayBillNumbers);
        void UpdateWayBillTrackingNumber(WayBillInfo wayBillInfo);
	    bool IsExitTrackingNumber(string trackingNumber, string wayBillNumber);
		//是否存在重复真实跟踪号
		bool IsExitTrueTrackingNumber(string trueTrackingNumber, string wayBillNumber);



        /// <summary>
        /// 获取运单实际入仓件数 zxq
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int GetWaybillPackageDetailCount(WaybillPackageDetail param);

	    /// <summary>
	    /// 新增,编辑,删除查询入库重量对比配置 yungchu
	    /// </summary>
	    /// <param name="entity"></param>
	    /// <returns></returns>
		InStorageWeightDeviation GetInStorageWeightCompareDeviationValue(string customerCode, int shippingMethodId);
	    bool AddInStorageWeightDeviations(InStorageWeightDeviation entity);
		bool EditInStorageWeightDeviations(InStorageWeightDeviation entity);
		bool DeleteInStorageWeightDeviations(int id);
		InStorageWeightDeviation GetInStorageWeightDeviation(int id);
	    IPagedList<InStorageWeightDeviation> GetInStorageWeightDeviationPagedList(WeightDeviationParam param);

        /// <summary>
        /// 获取入仓进度
        /// </summary>
        /// <param name="inStorageIDs"></param>
        /// <returns></returns>
        List<InStorageProcess> GetInStorageProcess(List<string> inStorageIDs);

        /// <summary>
        ///修改,查询运单业务日期信息 add by yungchu
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AddWayBillBusinessDateInfos(WayBillBusinessDateInfo entity);
        WayBillBusinessDateInfo GetWayBillBusinessDateInfo(string wayBillNumber);


        void FixInStoraging();

        LithuaniaInfo GetLithuaniaInfoByWayBillNumber(string wayBillNumber);
    }
}
