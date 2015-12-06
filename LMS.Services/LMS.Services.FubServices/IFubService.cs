using LMS.Data.Entity.ExtModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.FubServices
{
    public interface IFubService
    {
       IPagedList<FubListModelExt> GetFubPagedList(FubListParam param);

        /// <summary>
        /// 获取打印袋牌数据
        /// </summary>
        /// <param name="outStorageId">出仓ID</param>
        /// <returns></returns>
        BagTagPrintExt GetBagTagPrint(string outStorageId);
        /// <summary>
        /// 获取云途袋牌号信息
        /// </summary>
        /// <param name="postBagNumber">云途袋牌号</param>
        /// <returns></returns>
        MailPostBagInfoExt GetMailPostBagInfoExt(string postBagNumber);
        /// <summary>
        /// 获取云途袋牌号信息
        /// </summary>
        /// <param name="fuPostBagNumber">邮政袋牌号</param>
        /// <returns></returns>
        MailPostBagInfoExt GetMailPostBagInfoByFu(string fuPostBagNumber);
        /// <summary>
        /// 换袋扫描
        /// </summary>
        /// <param name="postBagNumber">云途袋牌号</param>
        /// <param name="fuPostBagNumber">邮政袋牌号</param>
        /// <returns></returns>
        bool SacnMailPostBagInfo(string postBagNumber, string fuPostBagNumber);
        /// <summary>
        /// 录入航班号查询列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<MailTotalPackageInfoExt> GetailTotalPackageList(LogFlightNumberListParam param);

        /// <summary>
        /// 根据总包号主键查询总包号信息
        /// </summary>
        /// <param name="mailTotalPackageNumber"></param>
        /// <returns></returns>
        MailTotalPackageInfoExt GetMailTotalPackageInfoExt(string mailTotalPackageNumber);



        /// <summary>
        /// 保存总包号与关系
        /// </summary>
        /// <param name="fuPostNumber">福邮袋牌</param>
        /// <param name="mainPostNumber">总包号</param>
        /// <returns></returns>
        ResultInfo MainPostNumberSave(string fuPostNumber, string mainPostNumber);

        /// <summary>
        /// 是否合法福邮袋牌
        /// </summary>
        /// <param name="fuPostNumber">福邮袋牌</param>
        /// <returns></returns>
        ResultInfo IsValidFuPostBagNumber(string fuPostNumber);


        IPagedList<FubListModelExt> GetFubCenterPagedList(FubListParam param);
        /// <summary>
        /// 录入航班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool LogFlightNumber(MailTotalPackageInfoExt model);
        /// <summary>
        /// 验证包裹单号
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns>0-验证成功,1-不存在，2-已退件</returns>
        string CheckTrackNumber(string trackNumber);

        /// <summary>
        /// 验证目的袋牌
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <returns>0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，3-该单号与目标袋牌国家不匹配,
        /// 4-该目标袋牌重量已超重不能放入,100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        string CheckBagNumber(string bagNumber, string trackNumber);
        /// <summary>
        /// 保存换袋记录
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <returns>-1-失败， 0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，3-该单号与目标袋牌国家不匹配,
        /// 4-该目标袋牌重量已超重不能放入,5-该单号发货渠道错误，100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        int SacnPackageExchangeBag(string bagNumber, string trackNumber);
        /// <summary>
        /// 退回记录查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<MailReturnGoodsLogsExt> GetMailReturnGoodsLogsList(MailReturnGoodsLogsParam param);

        /// <summary>
        /// 包裹换袋记录查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<MailExchangeBagLogsExt> GetMailExchangeBagLogsList(MailExchangeBagLogsParam param);



        void AddMailReturnGoodsLogs(List<ReturnGoodsModel> returnGoodsModels);

        void CanAddMailReturnGoodsLogs(string trackNumber, int reasonType);

        /// <summary>
        /// 包裹拦截记录查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<MailHoldLogsExt> GetMailHoldLogsList(MailHoldLogsParam param);

        void AddMailHoldLogs(string[] trackNumbers);
    }
}
