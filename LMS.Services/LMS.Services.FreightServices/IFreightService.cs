using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;

namespace LMS.Services.FreightServices
{
    public interface IFreightService
    {
        /// <summary>
        /// 获取客户类型列表
        /// </summary>
        /// <returns></returns>
        List<CustomerType> GetCustomerTypeList();

        List<ShippingMethodModel> GetShippingMethodByTypeId();

        List<ShippingMethodModel> GetShippingMethodByTypeId(int shippingMethodTypeId);

        /// <summary>
        /// 获取运输方式
        /// </summary>
        /// <param name="venderCode"></param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        List<ShippingMethodModel> GetShippingMethods(string venderCode, bool IsAll, int shippingMethodType);

        /// <summary>
        /// 获取运输方式
        /// </summary>
        /// <param name="venderCode"></param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        List<ShippingMethodModel> GetShippingMethods(string venderCode, bool IsAll);
        /// <summary>
        /// 获取服务商列表
        /// </summary>
        /// <returns></returns>
        List<Vender> GetVenderList(bool IsAll);

        /// <summary>
        /// 根据客户编码获取运输方式列表
        /// </summary>
        /// <returns></returns>
        List<ShippingMethod> GetShippingMethodListByCustomerCode(string customerCode, bool IsAll);

        /// <summary>
        /// 根据客户类型ID获取运输方式列表
        /// </summary>
        /// <param name="customerTypeId"></param>
        /// <returns></returns>
        List<ShippingMethod> GetShippingMethodListByCustomerTypeId(int? customerTypeId, bool IsAll);

        /// <summary>
        /// 根据服务商代码获取运输方式列表
        /// </summary>
        /// <param name="venderCode"></param>
        /// <returns></returns>
        List<ShippingMethod> GetShippingMethodListByVenderCode(string venderCode, int shippingMethodType,bool IsAll);

        /// <summary>
        /// 根据服务商代码获取运输方式列表
        /// </summary>
        /// <param name="venderCode"></param>
        /// <returns></returns>
        List<ShippingMethod> GetShippingMethodListByVenderCode(string venderCode, bool IsAll);

        /// <summary>
        /// 计算客户运费
        /// </summary>
        /// <param name="customerPackageModel"></param>
        /// <returns></returns>
        PriceProviderResult GetCustomerShippingPrice(CustomerPackageModel customerPackageModel);

        /// <summary>
        /// 获取计算客户运费可用运输方式
        /// </summary>
        /// <param name="customerPackageModel"></param>
        /// <returns></returns>
        List<PriceProviderResult> GetCustomerShippingPrices(FreightPackageModel customerPackageModel);

        /// <summary>
        /// 计算服务商运费
        /// </summary>
        /// <param name="venderPackageModel"></param>
        /// <returns></returns>
        PriceProviderResult GetVenderShippingPrice(VenderPackageModel venderPackageModel);

        /// <summary>
        /// 获取国家所属的区域
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        List<ShippingMethodCountryModel> GetCountryArea(int shippingMethodId, string countryCode);

        /// <summary>
        /// 获取运输方式支持的国家区域
        /// </summary>
        List<ShippingMethodCountryModel> GetCountryArea(int shippingMethodId);

        /// <summary>
        /// 获取国家中文名字
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        string GetChineseName(string countryCode);

        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <returns></returns>
        List<Country> GetCountryList();

        ShippingMethodModel GetShippingMethod(int shippingMethodId);

        /// <summary>
        /// 获取所有国家
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        List<ShippingMethodCountryModel> GetCountrys();

        /// <summary>
        /// 按运输方式ID获取运输方式列表
        /// </summary>
        /// <param name="shippingMethodIds">运输方式ID列表</param>
        /// <returns></returns>
        List<ShippingMethodModel> GetShippingMethodsByIds(List<int> shippingMethodIds);

        ShippingMethodModel GetShippingMethodByCode(string code);

        /// <summary>
        /// 获取客户运输方式对应运费计算接口
        /// </summary>
        /// <param name="customerPackageModel"></param>
        /// <returns></returns>
        PriceProviderResult GetCustomerShippingPrice(CustomerInfoPackageModel customerPackageModel);

        /// <summary>
        ///  获取商业快递类型运输方式
        /// add by zhengsong
        /// </summary>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        List<ShippingMethodModel> GetShippingMethodList(bool IsAll);

        List<ShippingMethod> GetShippingMethodListByCustomerTypeId(string customerId, int? customerTypeId, bool IsAll);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-09
        /// 提交跟踪号请求服务抓取跟踪信息
        /// </summary>
        /// <param name="orderTrackingRequests"></param>
        /// <returns></returns>
        int AddOutTrackingInfo(List<OrderTrackingRequestModel> orderTrackingRequests);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-10
        /// 根据跟踪号获取跟踪信息
        /// </summary>
        /// <param name="trackingNumberList"></param>
        /// <returns></returns>
        List<OrderTrackingModel> GetOutTrackingInfoList(string trackingNumberList);

        OrderTrackingModel GetOutTrackingInfo(string trackingNumber);

        /// <summary>
        /// 获取需要隐藏跟踪号的运输方式
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        List<ShippingMethodModel> GetShippingMethodsByHide();

        /// <summary>
        /// 是否超周长
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        ResponseResult IsOverMaxGirthSingle(CustomerInfoPackageModel package);

		/// <summary>
		/// 获取客户可用的关税预付运输方式信息
		/// yungchu
		/// </summary>
		/// <param name="customerCode"></param>
		/// <returns></returns>
	    List<TariffPrepayFeeShippingMethod> GetShippingMethodsTariffPrepay(string customerCode);

		///根据customerCode获取lis客户信息
	    List<Customer> GetListCustomers(string customerCode);
		///更新的用户同步到lis客户表 yungchu
		bool UpdateCustomerInfoToLis(CustomerInfoParam customerInfoParam);

        VenderModel GetVender(string venderCode);
        ResponseResult AddWayBillZONGTENG(string wayBill);

        /// <summary>
        /// Add By zhengsong
        /// 判读是否是偏远地址
        /// Time:2014-11-27
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <param name="shippingCity"></param>
        /// <param name="shippingZip"></param>
        /// <returns></returns>
        bool IsRemoteArea(int? shippingMethodId, string countryCode, string shippingCity,
                                          string shippingZip);

		//获取偏远地址 add by yungchu
	    List<RemoteAreaAddressExt> GetPagedListRemoteAreaAddress(RemoteAreaAddressParam param);
    }
}
