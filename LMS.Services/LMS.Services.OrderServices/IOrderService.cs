using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Services.OrderServices
{
    public interface IOrderService
    {
        /// <summary>
        /// 后台批量上传运单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool BatchCreateWayBillInfo(List<WayBillInfo> list);

        /// <summary>
        /// 批量更新上传运单(API)
        /// </summary>
        /// <param name="list">运单列表</param>
        /// <param name="trackingNumberDetailInfos">跟踪号列表</param>
        /// <returns></returns>
        bool BatchCreateWayBillInfoAPI(List<WayBillInfo> list);

        List<TrackingNumberExt> GetTrackingNumberExtList(TrackingNumberParam param);

        /// <summary>
        /// 创建运单，并反回跟踪号
        /// </summary>
        /// <param name="wayBillInfo"></param>
        /// <returns></returns>
        //TrackingNumberDetailInfo CreateWayBillInfoBySysTrackNumber(WayBillInfo wayBillInfo);

        /// <summary>
        /// 获取运单列表信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<WayBillInfo> GetWayBillInfoPagedList(OrderListParam param);
        /// <summary>
        /// 获取异常运单列表信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<AbnormalWayBillModel> GetAbnormalWayBillPagedList(AbnormalWayBillParam param);
        /// <summary>
        /// Hold运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        void HoldWayBillInfo(string wayBillNumber);

        /// <summary>
        /// 批量拦截（运单管理）
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        void BatchHoldWayBillInfo(List<string> wayBillNumberList);

        /// <summary>
        /// 修改运单
        /// </summary>
        /// <param name="wayBillInfo"></param>
        void UpdateWayBillInfo(WayBillInfo wayBillInfo);


	    /// <summary>
		/// 修改运单部分信息()
	    /// add by yungchu
	    /// </summary>
	    /// <param name="wayBillInfo"></param>
	    void UpdateWayBillInfos(WayBillInfo wayBillInfo);

	    /// <summary>
        /// 批量修改运输方式
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        /// <param name="shippingMethodId">改运输方式ID</param>
        /// <param name="shippingMethodName">改运输方式名称</param>
        void BatchUpdateWayBillInfo(List<string> wayBillNumberList, int? shippingMethodId, string shippingMethodName);


        /// <summary>
        /// 判断同一客户的订单号是否相同
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        bool IsExitOrderNUmber(string orderNumber, string customerCode);
        /// <summary>
        /// 判断订单的跟踪号是否相同
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        bool IsExitTrackingNumber(string trackingNumber);
        /// <summary>
        /// 取消拦截运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        void CancelHoldWayBillInfo(string wayBillNumber);

        /// <summary>
        /// 取消拦截运单(批量)
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        void BatchCancelHoldWayBillInfo(List<string> wayBillNumberList);
        /// <summary>
        /// 删除拦截运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        void DeleteWayBillInfo(string wayBillNumber);

		/// <summary>
		/// 批量退回异常运单
		/// yungchu
		/// </summary>
		/// <param name="wayBillNumberList"></param>
		void BatchReturnWayBillInfo(List<string> wayBillNumberList);



        /// <summary>
        /// 删除拦截运单(批量)
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        void BatchDeleteWayBillInfo(List<string> wayBillNumberList);
        /// <summary>
        /// 增加异常运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="typeEnum"></param>
        /// <param name="desction"></param>
        void AddAbnormalWayBill(string wayBillNumber, WayBill.AbnormalTypeEnum typeEnum, string description);

        /// <summary>
        /// 增加异常运单
        /// </summary>
        void AddAbnormalWayBill(string wayBillNumber, WayBill.AbnormalTypeEnum typeEnum, string description, string userUame);
        /// <summary>
        /// 获取所有保险类型
        /// </summary>
        /// <returns></returns>
        List<InsuredCalculation> GetInsuredCalculationListAll();
        /// <summary>
        /// 获取所有敏感货品类型
        /// </summary>
        /// <returns></returns>
        List<SensitiveTypeInfo> GetSensitiveTypeInfoListAll();

        /// <summary>
        /// 条件查询所有运单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<WayBillInfo> GetWayBillInfo(OrderListParam param);

        ShippingInfo GetshippingInfoById(int? id=0);
        InsuredCalculation GetInsuredCalculationById(int? id=0);
        SensitiveTypeInfo GetSensitiveTypeInfoById(int? id=0);
        CustomerOrderInfo GetCustomerOrderInfoById(int? id=0);
        IEnumerable<ApplicationInfo> GetApplicationInfoByWayBillNumber(string name);

        /// <summary>
        /// 从TrackingNumberDetail表中更新wayBillNumber表和CustomerOrderInfo表的跟踪号
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <param name="wayBillNumber">运单ID</param>
        /// <param name="shippingMethodId">运输方式ID</param>
        /// <param name="isSystemGenerate">是否需要系统自动生成跟踪号</param>
        /// <returns>返回跟踪号</returns>
        string UpdateTrackingNumberByTrackingNumberDetail(int customerOrderId, string wayBillNumber,
                                                          int shippingMethodId,string countryCode, bool isSystemGenerate = true);

        /// <summary>
        /// 运单跟踪号更改
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        void ChangeWayBillTrackingNumber(List<WayBillInfo> wayBillInfos);

        SelectTrackingNumberExt GetTrackingNumberDetails(TrackingNumberParam param);
        List<GoodsTypeInfo> GetGoodsTypes();
        SenderInfo GetSenderInfoById(int? id);


        IEnumerable<WayBillInfo> GetWayBillInfos(IEnumerable<string> wayBillOrTranckingNumers);

        /// <summary>
        /// 运单转单逻辑
        /// </summary>
        /// <param name="wayBillInfo">运单</param>
        /// <param name="trackingNumber">跟踪号</param>
        /// <returns></returns>
        WayBillInfo UpdataWayBillTrackingNumber(WayBillInfo wayBillInfo, string trackingNumber);

        /// <summary>
        /// 判断运单的跟踪号是否存在
        /// Add by zhengsong
        /// Time:2014-05-28
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        bool IsExitTrackingNumber(string trackingNumber, string wayBillNumber);

        ///// <summary>
        ///// Add By zhengsong
        ///// Time:2014-06-09
        ///// </summary>
        ///// <param name="inTackingLogInfo"></param>
        ///// <returns></returns>
        //bool CreateInTackingLogInfo(InTrackingLogInfo inTackingLogInfo);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <param name="trueTrackeingNumber"></param>
        bool UpdateWayBillAndOrderStust(string trueTrackeingNumber);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        IEnumerable<WayBillInfo> GetWayBillTakeList(List<int> shippingMehotdId, DateTime endTime);

        /// <summary>
        /// 获取需要跟踪信息的跟踪号
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        string GetTrueTrackingNumber();

        /// <summary>
        /// 获取未使用的跟踪号
        /// Add by zhengsong
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        //TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode, List<int> detailIds);

        /// <summary>
        /// 保存使用的跟踪号
        /// Add by zhengsong
        /// </summary>
        /// <param name="trackingNumberDetailInfos"></param>
        /// <returns></returns>
        //bool UpdateTrackingNumberDetail(List<TrackingNumberDetailInfo> trackingNumberDetailInfos );

        /// <summary>
        /// 重写转单逻辑
        /// Add By zhengsong
        /// Time:2014-06-16
        /// </summary>
        /// <param name="wayBillInfo"></param>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        WayBillInfo InStorageWayBillTrackingNumber(WayBillInfo wayBillInfo, string trackingNumber);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-20
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<ExpressWayBillViewExt> GetPagedExpressWayBillList(ExpressWayBillParam param);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-23
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<ExpressWayBillExt> GetExprotWayBillList(ExpressWayBillParam param);

        /// <summary>
        /// 导出异常运单
        /// Add By zhengsong
        /// Time:2014-07-09
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<AbnormalWayBillModel> GetAbnormalWayBillList(AbnormalWayBillParam param);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<WayBillInfo> GetExportWayBillInfoPagedList(OrderListParam param);

        /// <summary>
        /// 中美专线导出模板导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<WayBillInfo> GetExportWayBillInfo(OrderListParam param);

        /// <summary>
        /// 获取运单列表，（仅供运单列表使用）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<WayBillInfoListSilm> GetWayBillInfoPagedListSilm(OrderListParam param);

        IPagedList<ShippingWayBillExt> GetShippingWayBillPagedList(ShippingWayBillParam param);
        List<ShippingWayBillExt> GetShippingWayBillList(ShippingWayBillParam param);
        string GetAllShippingWayBillList(ShippingWayBillParam param);
        string GetIsUpdateShippingWayBillList(List<string> wayBillList);
        //运单列表导出
        IList<WayBillListExportModel> GetWayBillListExport(WayBillListExportParam param);
        //运单列表导出，查询申报信息
        IList<ApplicationInfoExportModel> GetApplicationInfoExport(WayBillListExportParam param);

		//查询运单改单列表 yungchu
	    IPagedList<WaybillInfoUpdateExt> GetWaybillInfoUpdatePagedList(WaybillInfoUpdateParam param);

		//“已收货”，判断运单是否已出收货费用进行不同逻辑 yungchu
        List<WayBillInfo> OperateWaybillByFee(List<string> waybillNumberList, int? shippingMethodId, string shippingMethodName, string trackNumber = null);
		//是否运单出费用
	    List<string> IsWayBillnumberInFeeInfo(List<string> waybillNumber);

		//返回新运单列表 yungchu
        List<WayBillInfo> CopyWayBillInfoList(List<string> waybillNumberList, int? flag, int? shippingMethodId, string shippingMethodName = null, string trackNumber = null);


		//返回入仓重量对比异常运单列表 yungchu
		IPagedList<InStorageWeightAbnormalExt> GetInStorageWeightAbnormalPagedList(InStorageWeightAbnormalParam param);
		List<ExportInStorageWeightAbnormalExt> GetExportInStorageWeightAbnormalExt(InStorageWeightAbnormalParam param);

		//增加入仓重量对比异常运单
		void AddInStorageWeightAbnormal(WeightAbnormalLog model);
		void DeleteInStorageWeightAbnormal(List<string> waybillNumbers);
		void UpdateInStorageWeightAbnormal(string waybillNumber,decimal weight);
		bool IsExistInStorageWeightAbnormal(string waybillNumber);

        void UpdateWayBillChangeLog(WayBillInfo wayBillInfo, string originalWayBillNumber, int changeType, string changeReason);

        /// <summary>
        /// 无预报入仓异常
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<NoForecastAbnormalExt> GetNoForecastAbnormalExtPagedList(NoForecastAbnormalParam param);

        void UpdateNoForecastAbnormal(NoForecastAbnormal noForecastAbnormalExt);

        void DeleteNoForecastAbnormal(int[] noForecastAbnormalId);

        void ReturnNoForecastAbnormal(int[] noForecastAbnormalIds);
        /// <summary>
        /// 获取运单汇总
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<WaybillSummary> GetWaybillSummaryList(WaybillSummaryParam param);
        /// <summary>
        /// 获取存在的客户订单号
        /// </summary>
        /// <param name="customerOrderNumbers">客户订单号集合</param>
        /// <returns></returns>
        List<string> GetIsEixtCustomerOrderNumber(List<string> customerOrderNumbers);
        /// <summary>
        /// 批量插入数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        void BulkInsert<T>(string tableName, IList<T> list);

        ///// <summary>
        ///// Add By zhengsong
        ///// Time:2014-09-25
        ///// </summary>
        ///// <param name="outShippingMethodId"></param>
        ///// <param name="wayBillNumbers"></param>
        ///// <returns></returns>
        //List<FZWayBillInfoExt> GetPostalWayBillInfo(List<int> outShippingMethodId,List<string> wayBillNumbers);

        /// <summary>
        /// 查询是否已经记录
        /// Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        List<FuzhouPostLog> GetFuzhouPostLogByWayBill(List<string> wayBillNumbers);

        /// <summary>
        /// 更申请记录
        ///  Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
        void AddorUpdateFuzhouPostLog(List<ErrorWayBillExt> errorWayBills);

        /// <summary>
        /// 查出福州邮政的运单
        /// Add By zhengsong
        /// Time:2014-11-05
        /// </summary>
        /// <param name="outShippingMethodId"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        List<FZWayBillInfoExt> GetFZWayBillNumbers(List<int> outShippingMethodId, int numbers);


        /// <summary>
        /// Add By zhengsong
        /// Time:2015-01-29
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        List<WayBillInfo> GetDHLandEUBWayBillInfos(List<int> inShippingMethodId, int numbers);

        /// <summary>
        /// hold 预报失败运单
        /// Add By zhengsong
        /// </summary>
        /// <param name="errorWayBills"></param>
        void UpdateWayBillInfo(Dictionary<string, string> errorWayBills);
    }
}
