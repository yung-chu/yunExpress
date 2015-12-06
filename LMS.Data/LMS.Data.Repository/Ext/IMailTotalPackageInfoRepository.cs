using System;
using System.Linq;
using System.Collections.Generic;

using System.Linq.Expressions;

using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LMS.Data.Entity;

// This file is auto generated and will be overwritten as soon as the template is executed
// Do not modify this file...
	
namespace LMS.Data.Repository
{   
	public partial interface IMailTotalPackageInfoRepository 
	{
        /// <summary>
        /// 保存总包号 和 总包与福邮的关系
        /// </summary>
        /// <param name="mainPostTag">总局袋牌</param>
        /// <param name="re">关系数据</param>
        /// <returns></returns>
        bool SaveMainPostBagTag(MailTotalPackageInfo mainPostTag, MailTotalPackageOrPostBagRelational re);


	}
}
