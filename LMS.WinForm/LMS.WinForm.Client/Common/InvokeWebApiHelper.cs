using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Core;
using LMS.Data.Entity.ExtModel;
using LMS.WinForm.Client.Models;
using LighTake.Infrastructure.Common.Logging;
using LMS.Data.Entity;
using CustomerInfoPackageRequest = LMS.Data.Entity.CustomerInfoPackageRequest;
using PriceProviderExtResult = LMS.Data.Entity.PriceProviderExtResult;

namespace LMS.WinForm.Client.Common
{
    public class InvokeWebApiHelper
    {

        #region 调用接口代码
        /// <summary>
        /// 获取收货信息
        /// </summary>
        /// <param name="number">订单号、运单号、跟踪号</param>
        /// <returns></returns>
        public static InFeeTotalInfoExtModel GetInFeeTotalInfo(string number)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<InFeeTotalInfoExtModel>(sysConfig.LMSAPIPath + "api/Lms/GetInFeeTotalInfo?number="+number, EnumHttpMethod.GET);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/lms/GetInFeeTotalInfo?number="+number);
                Log.Exception(ex);
            }
            return null;
        }
        /// <summary>
        /// 提交退货信息
        /// </summary>
        /// <param name="returnGoodsList">退货信息列表</param>
        /// <returns></returns>
        public static ResponseResultModel PostReturnGoodsResult(List<ReturnGoodsExt> returnGoodsList)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<ResponseResultModel>(sysConfig.LMSAPIPath + "api/Lms/PostBatchAddReturnGoods", EnumHttpMethod.POST, EnumContentType.Json, returnGoodsList);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/lms/PostBatchAddReturnGoods/");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 提交退货信息
        /// </summary>
        /// <param name="returnGoodsList">退货信息列表</param>
        /// <param name="loginModel">登录信息</param>
        /// <returns></returns>
        public static UserModel PostLogin(LoginModel loginModel)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<UserModel>(sysConfig.LMSAPIPath + "api/Lms/PostLogin", EnumHttpMethod.POST, EnumContentType.Json, loginModel);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/lms/PostLogin/");
                Log.Exception(ex);
            }
            return null;
        }

        public static UpdateResponse GetLatestVersion(string appName)
        {
            string url = sysConfig.LMSAPIPath + "api/ClientUpdate/LatestVersion?appName=" + appName;
            try
            {

                var resultModel = HttpHelper.DoRequest<UpdateResponse>(url, EnumHttpMethod.GET);

                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取客户信息 add by huhaiyou 2014-4-10
        /// </summary>
        /// <param name="keyWord">客户编码，客户名称</param>
        /// <returns></returns>
        public static List<CustomerModel> GetCustomerListCS(string keyWord)
        {
            try
            {
                string url = sysConfig.LMSAPIPath + "api/WayBill/GetCustomerListCS";
                if (!string.IsNullOrWhiteSpace(keyWord))
                {
                   url+="?keyWord=" + keyWord;
                }
                var resultModel = HttpHelper.DoRequest<List<CustomerModel>>(url, EnumHttpMethod.GET);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/WayBill/GetCustomerList?keyWord/");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取客户运输方式 add by huhaiyou 2014-4-23
        /// </summary>
        /// <param name="customerTypeId">获取客户运输方式</param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public static List<ShippingMethodModel> GetCustomerShippingMethodsByCustomerId(string customerId,string customerTypeId, bool IsAll)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodsByCustomerId";
            if (!IsAll) url = url + "?enabled=true";
            if (!string.IsNullOrWhiteSpace(customerTypeId))
            {
                if (url.IndexOf('?') > 0)
                {
                    url = url + "&customerTypeId=" + customerTypeId + "&customerId=" + customerId;
                }
                else
                {
                    url = url + "?customerTypeId=" + customerTypeId + "&customerId=" + customerId; ;
                }
            }
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }

        /// <summary>
        /// 获取物流列表 add by huhaiyou 201-4-24
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static List<VendersModel> GetVenders(bool? enabled = null)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<List<VendersModel>>(sysConfig.LISAPIPath + "api/Lis/GetVenders?enabled=" + enabled, EnumHttpMethod.GET);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/Lis/GetVenders?enabled/");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取运单信息 add by huhaiyou 2014-4-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static InStorageWayBillModel CheckOnInStorage(InStorageRequestFormModel model)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<InStorageWayBillModel>(sysConfig.LMSAPIPath + "api/WayBill/CheckOnInStorageSingele", EnumHttpMethod.POST, EnumContentType.Json, model);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/WayBill/CheckOnInStorageSingele/");
                Log.Exception(ex);
            }
            return null;
 
        }

        /// <summary>
        /// 检查运单是否正确 add by huhaiyou 2014-4-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static CheckOnWayBillResponseResult CheckOnWayBill(InStorageFormModel filter)
        {
            try
            {
                // var resultModel = HttpHelper.DoRequest<string>(sysConfig.LMSAPIPath + "api/LMS/CheckOnInStorage?model=" + model, EnumHttpMethod.GET);
                // var resultModel = HttpHelper.DoRequest<ResponseResultModel>(sysConfig.LMSAPIPath + "api/Lms/PostBatchAddReturnGoods", EnumHttpMethod.POST, EnumContentType.Json, returnGoodsList);
                var resultModel = HttpHelper.DoRequest<CheckOnWayBillResponseResult>(sysConfig.LMSAPIPath + "api/WayBill/CheckOnWayBillCS", EnumHttpMethod.POST, EnumContentType.Json, filter);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/WayBill/CheckOnWayBillCS/");
                Log.Exception(ex);
            }
            return null;

        }


        /// <summary>
        /// 创建入库单 add by huhaiyou 2014-4-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ResponseResult CreateInStorageCS(InStorageSaveModel model)
        {
            try
            {
                // var resultModel = HttpHelper.DoRequest<string>(sysConfig.LMSAPIPath + "api/LMS/CheckOnInStorage?model=" + model, EnumHttpMethod.GET);
                // var resultModel = HttpHelper.DoRequest<ResponseResultModel>(sysConfig.LMSAPIPath + "api/Lms/PostBatchAddReturnGoods", EnumHttpMethod.POST, EnumContentType.Json, returnGoodsList);
                var resultModel = HttpHelper.DoRequest<ResponseResult>(sysConfig.LMSAPIPath + "api/WayBill/CreateInStorageCS", EnumHttpMethod.POST, EnumContentType.Json, model);

                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "api/WayBill/CreateInStorageCS/");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取运费计算费用 add by huhaiyou 2014-4-30
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static PriceProviderExtResult PostCustomerPriceAuto(CustomerInfoPackageRequest packageRequest)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<PriceProviderExtResult>(sysConfig.LISAPIPath + "API/LIS/PostCustomerPriceAuto", EnumHttpMethod.POST, EnumContentType.Json, packageRequest);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "/API/LIS/PostCustomerPriceAuto");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取入库单信息 add by huhaiyou 2014-5-7
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        public static InStorageInfoModel GetInStorageInfo(string InStorageId)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<InStorageInfoModel>(sysConfig.LMSAPIPath + "api/WayBill/GetInStorageInfo?InStorageId=" + InStorageId, EnumHttpMethod.POST, EnumContentType.Json);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "/api/WayBill/GetInStorageInfo");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取入库单信息 add by huhaiyou 2014-5-7
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        public static List<InstorageInfoViewModel> GetPrintInStorageInvoice(string InStorageId)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<List<InstorageInfoViewModel>>(sysConfig.LMSAPIPath + "api/WayBill/GetPrintInStorageInvoice?InStorageId=" + InStorageId, EnumHttpMethod.GET, EnumContentType.Json);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "/api/WayBill/GetPrintInStorageInvoice");
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 打印入仓发票
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        public static InStorageInfoModelDetailViewModel InStorageDetailCS(string InStorageId)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<InStorageInfoModelDetailViewModel>(sysConfig.LMSAPIPath + "api/WayBill/InStorageDetailCS?InStorageId=" + InStorageId, EnumHttpMethod.POST, EnumContentType.Json);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LMSAPIPath + "/api/WayBill/InStorageDetailCS");
                Log.Exception(ex);
            }
            return null;
        }

        #endregion
    }

    public class UpdateResponse
    {
        public bool Success { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
    }
}
