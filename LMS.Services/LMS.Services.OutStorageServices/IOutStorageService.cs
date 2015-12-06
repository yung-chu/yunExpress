using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.OutStorageServices
{
    public interface IOutStorageService
    {
        /// <summary>
        /// 创建出仓单
        /// </summary>
        /// <param name="createOutStorageExt"></param>
        void CreateOutStorage(CreateOutStorageExt createOutStorageExt);
        /// <summary>
        /// 获取出仓单信息
        /// </summary>
        /// <param name="outStorageId"></param>
        /// <returns></returns>
        OutStorageInfo GetOutStorageInfo(string outStorageId);
        /// <summary>
        /// 查询出仓信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<OutStorageInfoExt> GetOutStoragePagedList(OutStorageListSearchParam param);
        /// <summary>
        /// 根据运单号列表查询运单信息列表
        /// </summary>
        /// <param name="WayBillNumbers"></param>
        /// <returns></returns>
        List<WayBillInfo> GetWayBillInfoListByWayBillNumber(List<string> wayBillNumbers);
        /// <summary>
        /// 根据运单号查询运单信息
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        WayBillInfo GetWayBillInfoByWayBillNumber(string wayBillNumber);
        /// <summary>
        /// 获取没有生成出仓成本的运单号列表
        /// </summary>
        /// <returns></returns>
        List<VenderFeeLog> GetNoPriceOutStorageWayBillList();
        /// <summary>
        /// 获取服务商价格出现错误，并记录
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="message"></param>
        void UpdateErrorRemark(string wayBillNumber, string message);
        /// <summary>
        /// 更新服务商价格
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="result"></param>
        void UpdateVenderPrice(string wayBillNumber, PriceProviderResult result);
        /// <summary>
        /// 获取服务商价格参数
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        VenderPackageModel GetVenderPriceModel(string wayBillNumber);

        /// <summary>
        /// 导出出仓运单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
       IList<ExportOutStorageInfo> GetExportOutStorageInfo(OutStorageListParam param);
        
        /// <summary>
        /// 更新出仓汇总服务商价格
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        bool UpdateOutStoragePrice(List<string> wayBillNumbers);

        /// <summary>
        /// 查询渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <returns></returns>
        List<DeliveryChannelConfiguration> GetDeliveryChannelConfigurations(int inShippingMethodId);

        /// <summary>
        /// 删除渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="deliveryChannelConfigurationId"></param>
        /// <returns></returns>
        bool DeleteDeliveryChannelConfiguration(int deliveryChannelConfigurationId);

        /// <summary>
        /// 添加渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddDeliveryChannelConfiguration(DeliveryChannelConfiguration model);

        /// <summary>
        /// 查询单个渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <param name="venderId"></param>
        /// <param name="outShippingMethodId"></param>
        /// <returns></returns>
        DeliveryChannelConfiguration GetDeliveryChannelConfiguration(int inShippingMethodId, int venderId, int outShippingMethodId);

        /// <summary>
        /// 是否可以整批的修改出仓渠道
        /// </summary>
        /// <param name="outStorageID"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        bool IsUpdateOutStorage(string outStorageID,int qty);

        List<DeliveryChannelConfiguration> GetOutStorageShippingMethods(int venderId);
        ResponseResult UpdateOutStorageInfo(string outStorageId, int outshippingMethodId, string outshippingMethodName, string venderCode, string remark);
        ResponseResult UpdateOutStorageInfoAll(string wayBillLists, int outshippingMethodId, string outshippingMethodName, string venderCode, string remark, out List<string> outStorageId);
        bool UpdateOldOutStorageInfo(List<string> outStorageIdList);
        /// <summary>
        /// 生成总包号
        /// </summary>
        /// <returns></returns>
        string CreateTotalPackageNumber();
        /// <summary>
        /// 获取运单集合的总重量，总件数
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="totalQty">总件数</param>
        /// <param name="totalWeight">总重量</param>
        void GetWayBillSummary(List<string> wayBillNumbers, out int totalQty, out decimal totalWeight);
        /// <summary>
        /// 获取运单集合总件数（包裹明细数量）
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="totalQty"></param>
        void GetWayBillTotalQty(List<string> wayBillNumbers, out int totalQty);
        /// <summary>
        /// 获取运单集合总重量（称重加上包材重量）
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="totalWeight"></param>
        void GetWayBillTotalWeight(List<string> wayBillNumbers, out decimal totalWeight);
        /// <summary>
        /// 根据服务商查询未离港的总包号
        /// </summary>
        /// <param name="venderCode"></param>
        /// <returns></returns>
        List<string> GetTotalPackageNumberList(string venderCode);
        /// <summary>
        /// 获取总包列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<TotalPackageInfoExt> GetTotalPackageList(EditTotalPackageTimeParam param);
        /// <summary>
        /// 获取通用地点
        /// </summary>
        /// <returns></returns>
        TotalPackageAddressExt GetTotalPackageAddress();
        /// <summary>
        /// 获取跟踪记录时间
        /// </summary>
        /// <param name="totalPackageNumber">总包号</param>
        /// <returns></returns>
        List<TotalPackageTraceInfo> GetTotalPackageTraceInfo(string totalPackageNumber);
        /// <summary>
        /// 编辑总包号跟踪时间
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        void EditTotalPackageTraceTime(List<TotalPackageTraceInfo> model);
        /// <summary>
        /// 获取该总包号最后一个出仓单追加进来的时间
        /// </summary>
        /// <param name="totalPackageNumber"></param>
        /// <returns></returns>
        DateTime GetTotalPackageTraceLastTime(string totalPackageNumber);
        /// <summary>
        /// 清除总包号跟踪时间记录
        /// </summary>
        /// <param name="model"></param>
        void DeleteTotalPackageTraceTime(List<TotalPackageTraceInfo> model);
        /// <summary>
        /// 获取B2C预报信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<B2CPreAlterExt> GetB2CPreAlterExtList(B2CPreAlterListParam param);

        bool PreAlterB2CBySearch(B2CPreAlterListParam param);
        bool PreAlterB2CByWayBillNumber(List<string> wayBillNumbers);
    }
}
