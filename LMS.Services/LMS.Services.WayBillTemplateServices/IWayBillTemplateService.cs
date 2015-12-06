using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.WayBillTemplateServices
{
    public interface IWayBillTemplateService
    {

        List<WayBillTemplate> GetList();
        /// <summary>
        /// 获取wayBillTempLate
        /// </summary>
        /// <param name="wayBillTempLateId"></param>
        /// <returns></returns>
        WayBillTemplate GetWayBillTemplate(int wayBillTempLateId);

        IEnumerable<WayBillTemplateExt> GetGetWayBillTemplateExtByName(string templateName);
        

        List<WayBillTemplate> GetWayBillTemplateByShippingMethod(int shippingMethodId);

        List<WayBillTemplate> GetWayBillTemplateByNameAndShippingMethod(string templateName, int shippingMethodId);

        /// <summary>
        /// 获取wayBillTempLate列表
        /// </summary>
        /// <param name="shippingMethodIds">运输方式ID列表</param>
        /// <param name="templateTypeId">类型ID</param>
        /// <returns></returns>
        IEnumerable<WayBillTemplateExt> GetWayBillTemplateList(IEnumerable<int> shippingMethodIds, string templateTypeId);

        WayBillTemplateExt GetWayBillTemplate(int shippingMethodId, string templateName);
     
        /// <summary>
        /// 添加运单模板信息
        /// </summary>
        /// <param name="wayBillTemplate"></param>
        void AddWayBillTemplate(WayBillTemplate wayBillTemplate);


        /// <summary>
        /// 更新运单模板信息
        /// </summary>
        /// <param name="wayBillTemplate"></param>
        void UpdateWayBillTemplate(WayBillTemplate wayBillTemplate);

        /// <summary>
        /// 获取运单模板列表信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<WayBillTemplate> GetWayBillTemplatePagedList(WayBillTemplateListParam param);


        /// <summary>
        /// 获取新建模板列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<WayBillTemplateInfo> GetWayBillTemplateInfoList(WayBillTemplateInfoParam param);

        WayBillTemplateInfo GetWayBillTemplateInfoByID(int templateModelId);

        /// <summary> 添加新增模板
        /// Add by zhengsong
        /// </summary>
        /// <param name="wayBillTemplateInfo"></param>
        /// <returns></returns>
        bool AddWayBillTemplateInfo(WayBillTemplateInfo wayBillTemplateInfo);

        /// <summary>
        /// 能否打印
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="number">运单号，订单号，跟踪号</param>
        /// <returns></returns>
        bool GetCanPrint(string templateName, string number);

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-11-11
        /// </summary>
        /// <returns></returns>
        List<GZPacketAddressInfo> GetGZPacketAddressInfo();

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-11-11
        /// </summary>
        /// <param name="gzPacketAddressInfo"></param>
        /// <returns></returns>
        void UpdateGZPacketAddressInfo(GZPacketAddressInfo gzPacketAddressInfo);

        /// <summary>
        /// 清空广州小包地址信息使用次数
        /// </summary>
        void UpdateGZPacketAddressNumber();
        /// <summary>
        /// 根据客户订单号获取客户订单信息（对外打印标签接口）
        /// </summary>
        /// <param name="orderNumbers"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        List<CustomerOrderInfo> PrintByCustomerOrderNumbers(List<string> orderNumbers, string customerCode);
    }
}
