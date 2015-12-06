using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    public static class QueryableExtension
    {
        /// <summary>
        /// 转换为强类型分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <returns>强类型分页列表</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source,int pageIndex,int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// 转换为强类型分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <returns>强类型分页列表</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }
    }
}
