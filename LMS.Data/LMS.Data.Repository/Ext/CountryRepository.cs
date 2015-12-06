using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class CountryRepository
    {
        /// <summary>
        /// 获取国家列表不包含国家
        /// </summary>
        /// <returns></returns>
        public List<CountryExt> GetCommonCountryList()
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            var list = from c in ctx.Countries
                       join cc in ctx.CommonCountries on c.CountryCode equals cc.CountryCode into lcc
                       from lc in lcc.DefaultIfEmpty()
                       orderby c.Name
                       select new CountryExt
                       {
                           CountryCode = c.CountryCode,
                           Name = c.Name,
                           AllowsShipping = c.AllowsShipping,
                           ThreeLetterISOCode = c.ThreeLetterISOCode,
                           NumericISOCode = c.NumericISOCode,
                           Status = c.Status,
                           DisplayOrder = c.DisplayOrder,
                           ChineseName = c.ChineseName,
                           IsCommonCountry = lc.IsCommonCountry
                       };

            return list.ToList();
        }

        public IPagedList<CountryExt> GetPagedList(CountryParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            if (ctx == null) return null;
            return (from a in ctx.Countries
                                       .WhereIf(p => p.Name.StartsWith(param.Name), !param.Name.IsNullOrEmpty())
                                       .WhereIf(p => p.ChineseName.StartsWith(param.ChineseName), !param.ChineseName.IsNullOrEmpty())
                                       .WhereIf(p => p.CountryCode.StartsWith(param.CountryCode), !param.CountryCode.IsNullOrEmpty())
                    select new CountryExt()
                    {
                        ChineseName = a.ChineseName,
                        CountryCode = a.CountryCode,
                        Name = a.Name
                    }
                   ).OrderBy(p => p.CountryCode).ToPagedList(param.Page, param.PageSize);
        }
        /// <summary>
        /// 获取国家的中文名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetCountryChineseName(string code)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            return ctx.Countries.Where(t => t.CountryCode == code).Select(t => t.ChineseName).FirstOrDefault();
        }
    }
}
