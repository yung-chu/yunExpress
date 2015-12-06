using LMS.Data.Entity.ExtModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IMailPostBagInfoRepository
    {

        /// <summary>
        /// 国际小包优+ 查询
        /// Add By zhengsong
        /// Time:2014-12-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<FubListModelExt> GetFubPagedList(FubListParam param);

        /// <summary>
        /// 中心局邮袋扫描查询
        /// Add By zhengsong
        /// Time:2014-12-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<FubListModelExt> GetFubCenterPagedList(FubListParam param);

        /// <summary>
        /// 获取打印袋牌数据
        /// </summary>
        /// <param name="outStorageId">出仓ID</param>
        /// <returns></returns>
        BagTagPrintExt GetBagTagPrint(string outStorageId);


        /// <summary>
        /// 是否有效的福邮袋牌
        /// </summary>
        /// <param name="fuPostBagNumber"></param>
        /// <returns></returns>
        ResultInfo IsValidFuPostBagNumber(string fuPostBagNumber);



        List<MailPostBagInfo> GetUnTrackingCreated(int count);


        List<WayBillInfo> GetWayBillByMailTotalPackageNumber(string mailTotalPackageNumber);

        string GetCountryCodeByMailTotalPackageNumber(string mailTotalPackageNumber);
        /// <summary>
        /// 获取云途袋牌号
        /// </summary>
        /// <param name="fuPostBagNumber">福邮袋牌号</param>
        /// <returns></returns>
        MailPostBagInfo GetYunExpressBagNumber(string fuPostBagNumber);
        /// <summary>
        /// 获取总包号发货国家
        /// </summary>
        /// <param name="mailTotalPackageNumbers"></param>
        /// <returns></returns>
        Dictionary<string, MailTotalPackageCountryExt> GetMailTotalPackageInfoCountry(List<string> mailTotalPackageNumbers);

        /// <summary>
        /// 通过跟踪号获取客户邮包号
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns></returns>
        string GetPostBagNumber(string trackNumber);

        IPagedList<MailHoldLogsExt> GetMailHoldLogsList(MailHoldLogsParam param);
    }
}
