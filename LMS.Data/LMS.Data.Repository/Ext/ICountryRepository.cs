using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface ICountryRepository
    {
        /// <summary>
        /// 获取国家列表包含常用国家
        /// </summary>
        /// <returns></returns>
        List<CountryExt> GetCommonCountryList();

        IPagedList<CountryExt> GetPagedList(CountryParam param);

        /// <summary>
        /// 获取国家的中文名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string GetCountryChineseName(string code);
    }
}
