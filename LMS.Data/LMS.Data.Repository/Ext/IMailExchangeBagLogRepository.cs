using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial interface IMailExchangeBagLogRepository
    {
        /// <summary>
        /// 验证包裹单号
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns></returns>
        string CheckTrackNumber(string trackNumber);

        /// <summary>
        /// 换袋记录查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<MailExchangeBagLogsExt> GetMailExchangeBagLogsList(MailExchangeBagLogsParam param);

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
        /// <param name="recordBy">操作人</param>
        /// <returns>-1-失败， 0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，3-该单号与目标袋牌国家不匹配,
        /// 4-该目标袋牌重量已超重不能放入,5-该单号发货渠道错误，100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        int SacnPackageExchangeBag(string bagNumber, string trackNumber, string recordBy);
    }
}
