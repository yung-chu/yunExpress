using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Express.DHL.Request;
using LMS.Data.Express.DHL.Response;
using LMS.Data.Express.DHL.Response;

namespace LMS.Services.ExpressServices
{
    public interface IExpressService
    {
        /// <summary>
        /// Post请求DHL接口
        /// </summary>
        /// <param name="ap">ShipmentValidateRequestAP对象实例</param>
        /// <param name="serverUrl">请求的URL地址</param>
        /// <returns>ShipmentValidateResponse对象</returns>
        ShipmentValidateResponse PostDHLShipment(ShipmentValidateRequestAP ap, string serverUrl);

        ExpressAccountInfo GetExpressAccountInfo(string venderCode, int shippingMethodId);
        ExpressRespons GetExpressResponse(string wayBillNumber);
        void AddExpressResponse(ExpressRespons expressResponse, WayBillInfo wayBillInfo,bool isNologin=false);
        void UpdateExpressResponse(ExpressRespons expressResponse);
        void AddExpressResponse(IEnumerable<ExpressRespons> expressResponses);

        /// <summary>
        /// 是否存在运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        bool IsExistExpressResponse(string wayBillNumber);

        IEnumerable<ExpressAccountInfo> GetExpressAccountInfos();
        /// <summary>
        /// 荷兰小包
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        bool IsExistNLPOST(string wayBillNumber);

        void AddNLPOST(NetherlandsParcelRespons netherlandsParcelRespons, WayBillInfo wayBillInfo);
        void UpdateNLPOST(NetherlandsParcelRespons netherlandsParcelRespons);

        /// <summary>
        /// 顺丰快递
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        NetherlandsParcelRespons GetNetherlandsParcelRespons(string wayBillNumber, int? status);
        /// <summary>
        /// 查询顺丰单号
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        string GetNetherlandsParcelSfNumber(string wayBillNumber);
        /// <summary>
        /// 取消荷兰小包
        /// </summary>
        /// <param name="wayBillNumber"></param>
        void DeleteNLPOST(string wayBillNumber);
        /// <summary>
        /// 根据客户订单号获取荷兰小包的顺丰单号
        /// </summary>
        /// <param name="orderIds"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        List<AgentNumberInfo> GetAgentNumbers(List<string> orderIds, string customerCode);

        /// <summary>
        /// 添加指定线下发货渠道记录
        /// </summary>
        void AddDeliveryChannelChangeLog(string wayBillNumber, int shippingMethodId, string venderCode,bool isNologin=false);

        ExpressRespons AddExpressResponseToAPI(ExpressRespons expressResponse, WayBillInfo wayBillInfo);
        void AddExpressResponseToDHL(List<ExpressRespons> expressResponses);
        void AddApiDHLDeliveryChannelChangeLog(List<WayBillInfo> wayBillInfos);
    }
}
