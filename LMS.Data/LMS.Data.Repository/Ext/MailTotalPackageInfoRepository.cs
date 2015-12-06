using System;
using System.Linq;
using System.Collections.Generic;

using System.Linq.Expressions;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Seedwork.EF;
using System.Transactions;
using LighTake.Infrastructure.Common;

// This file is auto generated and will be overwritten as soon as the template is executed
// Do not modify this file...

namespace LMS.Data.Repository
{
    public partial class MailTotalPackageInfoRepository
    {
        static object _lockSaveMainPostBagTag = new object();

        /// <summary>
        /// 保存总包号 和 总包与福邮的关系
        /// </summary>
        /// <param name="mainPostTag">总局袋牌</param>
        /// <param name="re">关系数据</param>
        /// <returns></returns>
        public bool SaveMainPostBagTag(MailTotalPackageInfo mainPostTag, MailTotalPackageOrPostBagRelational re)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;  
           
            lock (_lockSaveMainPostBagTag)
            {
                if (ctx.MailTotalPackageOrPostBagRelationals.Any(t => t.PostBagNumber == re.PostBagNumber))
                {
                    throw new BusinessLogicException("[福邮袋牌号]已经扫描过了.");
                }
                
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (!ctx.MailTotalPackageInfos.Any(t => t.MailTotalPackageNumber == mainPostTag.MailTotalPackageNumber)) //不存在
                    {
                        DateTime fiveDaysAgo = DateTime.Now.AddDays(-5);

                        //5天之前是否存在
                        if (ctx.MailTotalPackageInfos.Any(t => t.TotalPackageNumber == mainPostTag.TotalPackageNumber && t.CreatedOn >= fiveDaysAgo))
                        {
                            //替换MailTotalPackageNumber
                            mainPostTag.MailTotalPackageNumber = ctx.MailTotalPackageInfos
                                  .Where(t => t.TotalPackageNumber == mainPostTag.TotalPackageNumber && t.CreatedOn >= fiveDaysAgo)
                                  .OrderByDescending(p => p.CreatedOn)
                                  .Select(t => t.MailTotalPackageNumber)
                                  .FirstOrDefault();
                        }
                        else
                        {
                            //insert MailTotalPackageInfo
                            ctx.MailTotalPackageInfos.Add(mainPostTag);
                        }
                    }

                    //insert MailTotalPackageOrPostBagRelational
                    re.MailTotalPackageNumber = mainPostTag.MailTotalPackageNumber;
                    ctx.MailTotalPackageOrPostBagRelationals.Add(re);

                    ctx.SaveChanges();
                    tran.Complete();
                }

                return true;
            }
            // throw new NotImplementedException();
        }
    }
}
