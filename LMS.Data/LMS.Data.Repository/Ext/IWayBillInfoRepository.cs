using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IWayBillInfoRepository
    {
        /// <summary>
        /// 收货费用明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IPagedList<InFeeInfoExt> GetInFeeInfoExtPagedList(InFeeListParam param, out decimal totalFee);
        /// <summary>
        /// 发货成本明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IPagedList<OutFeeInfoExt> GetOutFeeInfoExtPagedList(OutFeeListParam param, out decimal totalFee);

        /// <summary>
        /// 收货费用明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<InFeeInfoExt> GetInFeeInfoExtList(InFeeListParam param, out decimal totalFee);

        /// <summary>
        /// 收货费用明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<InFeeTotalInfoExt> GetInFeeTotalInfoExtList(InFeeTotalListParam param, out decimal totalFee);
        /// <summary>
        /// 发货成本明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<OutFeeInfoExt> GetOutFeeInfoExtList(OutFeeListParam param, out decimal totalFee);
        /// <summary>
        /// 导出出仓运单信息(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IList<ExportOutStorageInfo> GetExportOutStorageInfo(OutStorageListParam param);

        /// <summary>
        /// 打印快递入仓交接单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IList<PrintInStorageInvoiceExt> GetPrintInStorageInvoice(PrintInStorageInvoiceParam param);

        /// <summary>
        /// 更新服务商价格汇总
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        bool UpdateOutStoragePrice(List<string> wayBillNumbers);

        /// <summary>
        ///  Add by zhengsong
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        List<int> GetCustomerId(List<string> wayBillNumbers);


        IList<WayBillInfo> GetWayBillList(string[] wayBillNumber,string customerCode);

        WayBillInfo GetWayBill(string wayBillNumber, string customerCode);


        /// <summary>
        /// 获取所有EUB运单
        /// </summary>
        /// <param name="shippingMethodIds">EUB运输方式列表</param>
        /// <param name="customerCode">客户代码</param>
        /// <returns></returns>
        IList<WayBillInfo> GetEubWayBillList(List<int> shippingMethodIds, string customerCode);

        /// <summary>
        /// add huhaiyou 2014-07-03
        /// </summary>
        /// <param name="shippingMethodIds"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        int GetEubWayBillCount(List<int> shippingMethodIds, string customerCode);

        WayBillInfo GetWayBillInfo(string numberStr, string customerCode);

		//获取称重 yungchu
		decimal GetWayBillWeight(string wayBillNumber);
        /// <summary>
        /// 快递货物信息查询
        /// Add By zhengsong
        /// Time:2014-06-19
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<ExpressWayBillExt> GetExpressWayBillDetailList(ExpressWayBillParam param);

        IPagedList<ExpressWayBillViewExt> GetPagedWayBillDetailList(ExpressWayBillParam param);
        /// <summary>
        /// 获取标签打印订单信息
        /// add by Jess
        /// date: 20140711
        /// </summary>
        /// <param name="orderNumbers"></param>
        /// <param name="customercode"></param>
        /// <returns></returns>
        List<LabelPrintExt> GetLabelPrintExtList(IEnumerable<string> orderNumbers, string customercode);

        WayBillInfo GetWayBill(string number);

        /// <summary>
        /// 大量数据插入
        /// </summary>
        void BulkInsert<T>(string tableName, IList<T> list);
        /// <summary>
        /// 根据运单号查询符合申请的EUB运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        IList<WayBillInfo> GetWayBillListByWayBillNumbers(string[] wayBillNumber, string customerCode);

        /// <summary>
        /// 获取运单的订单号
        /// </summary>
        /// <param name="customerOrderNumbers"></param>
        /// <returns></returns>
        List<int?> GetExistCustomerOrderNumber(List<string> customerOrderNumbers);

        WayBillInfo GetWayBillByTrackingNumber(string trackingNumber);

        WayBillInfoExtSilm GetWayBillInfoExtSilm(string number);

        IPagedList<WayBillInfo> FindPagedListExt(int pageIndex, int pageSize, Expression<Func<WayBillInfo, bool>> expression,
                                                 Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>> orderByExpression);

        IPagedList<WayBillInfoListSilm> FindPagedListSilm(int pageIndex, int pageSize, Expression<Func<WayBillInfo, bool>> expression,
                                         Func<IQueryable<WayBillInfoListSilm>, IOrderedQueryable<WayBillInfoListSilm>> orderByExpression);

        IPagedList<AbnormalWayBillModel> FindPagedListAbnormalWayBillModel(int pageIndex, int pageSize,
                                                                                  Expression<Func<WayBillInfo, bool>>
                                                                                      expression,
                                                                                  Func
                                                                                      <IQueryable<AbnormalWayBillModel>,
                                                                                      IOrderedQueryable
                                                                                      <AbnormalWayBillModel>>
                                                                                      orderByExpression);

        List<AbnormalWayBillModel> FindListAbnormalWayBillModel(
                                                                          Expression<Func<WayBillInfo, bool>>
                                                                              expression,
                                                                          Func
                                                                              <IQueryable<AbnormalWayBillModel>,
                                                                              IOrderedQueryable
                                                                              <AbnormalWayBillModel>>
                                                                              orderByExpression);

        /// <summary>
        /// 已发货运单列表
        /// Add By zhengsong
        /// Time:2014-09-13
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<ShippingWayBillExt> GetShippingWayBillPagedList(ShippingWayBillParam param);

        string GetAllShippingWayBillList(ShippingWayBillParam param);

        /// <summary>
        /// 已发货运单导出
        /// Add By zhengsong
        /// Time:2014-09-13
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<ShippingWayBillExt> GetShippingWayBillList(ShippingWayBillParam param);

        List<WayBillEventLogExt> GetWayBillEventLogExtList(int maxCount);
        string GetIsUpdateShippingWayBillList(List<string> wayBillList);
        //运单列表导出
        IList<WayBillListExportModel> GetWayBillListExport(WayBillListExportParam param);
        //运单列表申报信息导出
        IList<ApplicationInfoExportModel> GetApplicationInfoExport(WayBillListExportParam param);

		//运单修改 yungchu 
		IPagedList<WaybillInfoUpdateExt> GetWaybillInfoUpdatePagedList(WaybillInfoUpdateParam param);
		//运单是否出费用
		List<string> IsWayBillnumberInFeeInfo(List<string> waybillNumber);


		//入仓重量对比异常运单
		IPagedList<InStorageWeightAbnormalExt> GetInStorageWeightAbnormaPagedList(InStorageWeightAbnormalParam param);
		List<ExportInStorageWeightAbnormalExt> GetExportInStorageWeightAbnormal(InStorageWeightAbnormalParam param);

        //运单汇总查询
        List<WaybillSummary> GetWaybillSummaryList(WaybillSummaryParam param);

        ///// <summary>
        ///// 查出福州邮政的运单
        ///// Add By zhengsong
        ///// Time:2014-11-05
        ///// </summary>
        ///// <param name="outShippingMethodId"></param>
        ///// <param name="wayBillNumbers"></param>
        ///// <returns></returns>
        //List<FZWayBillInfoExt> GetFuZhouWayBillList(List<int> outShippingMethodId, List<string> wayBillNumbers);

        List<FZWayBillInfoExt> GetFuZhouWayBillNumbers(List<int> outShippingMethodId,int number);

        /// <summary>
        /// 查询入仓单包括的运单入仓状态
        /// </summary>
        /// <param name="inStorageIDs">入仓单IDs</param>
        /// <returns></returns>
        List<InStorageProcess> GetInStorageProcess(List<string> inStorageIDs);

        //根据跟踪号获取运单号--FUBTrack add by yunchu
        List<string> GetWaybillNumberList(List<string> trackingNumberList);

        /// <summary>
        /// 获取需要预报信息的DHL和EUB运单
        /// </summary>
        /// <param name="intShippingMethodIds"></param>
        /// <param name="number">每次取number之后的运单</param>
        /// <returns></returns>
        List<WayBillInfo> GetDHLandEUBWayBillInfos(List<int> intShippingMethodIds, int number);
    }
}
