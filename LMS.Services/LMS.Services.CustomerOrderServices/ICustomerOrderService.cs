using System.Collections.Generic;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.CustomerOrderServices
{
    public interface ICustomerOrderService
    {
        /// <summary>
        /// 添加客户订单
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        bool Add(CustomerOrderInfo orderInfo);

        /// <summary>
        /// 批量添加客户订单 
        /// </summary>
        /// <param name="orderInfos"></param>
        /// <returns></returns>
        bool BatchAdd(List<CustomerOrderInfo> orderInfos);

        /// <summary>
        /// 修改客户订单
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        bool Moditfy(CustomerOrderInfo orderInfo);

        /// <summary>
        /// 删除客户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// 取肖客户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Cancel(int id);
        /// <summary>
        /// 取肖客户订单(批量)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool BatchCancel(List<int> ids);

		/// <summary>
		/// 删除客户订单(批量)
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
	    bool BatchDelete(List<int> ids);

        /// <summary>
        /// 根据CustomerOrderId列表获取CustomerOrderList
        /// </summary>
        /// <param name="ids">CustomerOrderId</param>
        /// <returns></returns>
        List<CustomerOrderInfo> GetListByCustomerOrderId(List<int> ids,int status);

        /// <summary>
        /// 获取客户订单信息
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        CustomerOrderInfo Get(int customerOrderId);

        /// <summary>
        /// 判断客户订单号是否已经存在
        /// </summary>
        /// <param name="customerCode">客户编号</param>
        /// <param name="customerOrderNumber">客户订单号</param>
        /// <returns></returns>
        bool IsExists(string customerCode, string customerOrderNumber);

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="maxCustomerOrderId"></param>
        /// <returns></returns>
        IPagedList<CustomerOrderInfoExt> GetList(CustomerOrderParam param,int maxCustomerOrderId=0);


        /// <summary>
        /// 根据订单号获取订单信息
        /// </summary>
        /// <param name="customerOrderNumber">订单数组</param>
        /// <returns></returns>
        List<CustomerOrderInfoExt> GetCustomerOrderList(string[] customerOrderNumber, string customerCode);

        /// <summary>
        /// 客户订单确认
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        bool CustomerOrderConfirm(int customerOrderId);

        /// <summary>
        /// 客户订单确认(批量)
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        bool CustomerOrderConfirmBatch(List<int> customerOrderIds);

        /// <summary>
        /// 客户订单提交
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        bool CustomerOrderSubmit(int customerOrderId);

        /// <summary>
        /// 客户订单提交(批量)
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        //ResultExt CustomerOrderSubmitBatch(List<int> customerOrderIds);

        /// <summary>
        /// 客户订单提交(批量)
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        //ResultExt CustomerOrderSubmitBatch(object customerOrderIds);

        /// <summary>
        /// 拦截客户订单
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <param name="message">拦截订单原因</param>
        /// <returns></returns>
        bool IsHold(int customerOrderId, string message);

        /// <summary>
        /// 拦截客户订单(批量)
        /// </summary>
        /// <param name="customerOrderIds">订单ID列表</param>
        /// <param name="message">拦截订单原因</param>
        /// <returns></returns>
        bool BatchHold(List<int> customerOrderIds,string message);

        IPagedList<EubWayBillApplicationInfoExt> GetEubWayBillList(EubWayBillApplicationInfoParam param, int maxCustomerOrderId = 0);

        /// <summary>
        /// 获取客户拦截订单列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="maxCustomerOrderId"></param>
        /// <returns></returns>
        PagedList<CustomerOrderInfoExt> GetCustomerOrderByBlockedList(CustomerOrderParam param, int maxCustomerOrderId = 0);

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        CustomerOrderInfo Print(int customerOrderId);

        /// <summary>
        ///  根据customerOrderId获取要打印的订单，并更改该订单的打印状态
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        CustomerOrderInfo PrintByCustomerCode(int customerOrderId);

        /// <summary>
        /// 根据customerOrderNumber获取要打印的订单，并更改该订单的打印状态
        /// </summary>
        /// <param name="customerOrderNumber"></param>
        /// <returns></returns>
        CustomerOrderInfo PrintByCustomerOrderNumber(string customerOrderNumber);

        List<CustomerOrderInfo> PrintByCustomerOrderIds(IEnumerable<int> customerOrderIds);

        IList<WayBillInfo> GetWayBillList(string[] wayBillNumber,string customerCode);

        WayBillInfo GetWayBill(string wayBillNumber, string customerCode);

		WayBillInfo GetWayBill(string number);

        /// <summary>
        /// Eub运单申请 ，并返回未成功的运单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <isNologin></isNologin>
        List<string> ApplyEubWayBillInfo(EubWayBillParam param);

        List<EubWayBillApplicationInfo> GetEubWayBillInfoList(List<string> wayBillNumbers);
        void UpdateEubWayBillInfoStatus(List<string> wayBillNumbers, int status);
        /// <summary>
        /// 获取所有EUB运单
        /// </summary>
        /// <param name="shippingMethodIds">EUB运输方式列表</param>
        /// <returns></returns>
        List<WayBillInfo> GetEubWayBillList(List<int> shippingMethodIds);

        /// <summary>
        /// 获取客户EUB订单数量  add huahiyou 2014-07-03
        /// </summary>
        /// <param name="shippingMethodIds"></param>
        /// <returns></returns>
        int GetEubWayBillCount(List<int> shippingMethodIds);

        /// <summary>
        /// 更新Eub订单的已打印状态
        /// </summary>
        /// <param name="eubOrderId"></param>
        void UpdateEubWayBillStatus(int eubOrderId);

        List<CustomerOrderInfo> GetCustomerOrderInfos(CustomerOrderParam param);

        List<CustomerOrderInfoExportExt> GetCustomerOrderInfoExport(CustomerOrderParam param);
        List<ApplicationInfo> GetApplicationInfoList(List<int> ids);

        bool DeleteCustomerOrderInfoList(List<int> selected);
        List<string> GetCustomerOrderInfos(List<string> customerOrderNumber);
        List<string> GetCustomerOrderInfoByTrack(List<string> trackingNumbers);
        List<string> GetCustomerOrderIdByWayBillNumber(List<string> wayBillNumbers);

		//客户中心--已提交运单删除 yungchu
	    bool UpdateWaybillStatus(List<int> customerOrderIds);
        /// <summary>
        /// 获取申请EUB运单列表
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        IList<WayBillInfo> GetEUBWayBillList(string[] wayBillNumber, string customerCode);

        /// <summary>
        /// 把订单改为提交中状态
        /// </summary>
        /// <param name="customerOrderIds"></param>
        void CustomerOrderSubmitQuick(List<int> customerOrderIds);

        CustomerOrderInfo GetCustomerOrderInfo(string customerOrderNumber);
        /// <summary>
        /// 获取当前最大的客户订单ID
        /// </summary>
        /// <returns></returns>
        int GetMaxCustomerOrderID();

        /// <summary>
        /// 检查是否属偏远地址
        /// </summary>
        List<string> CheckRemoteArea(List<int> customerOrderIds);

        /// <summary>
        /// Eub运单Api申请
        /// Add By zhengsong
        /// Time:2015-01-22
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<CustomerOrderService.ResultEUB> ApplyApiEubWayBillInfo(EubWayBillParam param);

        /// <summary>
        /// 添加 EubWayBillApplicationInf表信息
        /// Add By zhengsong
        /// Time:2014-01-22
        /// </summary>
        /// <param name="wayBillInfos"></param>
        void AddEubWayBillApplicationInfo(List<WayBillInfo> wayBillInfos);

        /// <summary>
        /// 后台job 执行预报
        /// Add By zhengsong
        /// Time:2015-02-02
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isNoLogin"></param>
        /// <returns></returns>
        Dictionary<string,string> ForecastEubWayBillInfo(EubWayBillParam param, bool isNoLogin = false);
    }
}