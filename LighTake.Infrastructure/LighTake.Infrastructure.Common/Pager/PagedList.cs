using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 表示具有分页信息的数据列表
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年11月02日
    /// 修改历史 : 无
    /// </remarks>
    public interface IPagedList<T> : IEnumerable<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPages { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// 数据列表
        /// </summary>
        List<T> InnerList { get; set; }
    }

    /// <summary>
    /// 具有分页信息的强类型数据列表
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年11月02日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            this.PageSize = 15;
            this.PageIndex = 1;
            this.InnerList = new List<T>();
        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">未分页的对象列表</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source.AsQueryable(), pageIndex, pageSize)
        {

        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">已分页的对象列表</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IEnumerable<T> source, int totalRecords, int pageIndex, int pageSize)
            : this(source.AsQueryable(), totalRecords, pageIndex, pageSize)
        {

        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">未分页的对象列表</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 15 : pageSize;

            int total = source.Count();

            this.TotalCount = total;

            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                this.TotalPages++;

            this.PageSize = pageSize;

            this.PageIndex = pageIndex;

            int skipCount = 0;

            if (pageIndex > 1)
            {
                skipCount = pageSize * pageIndex - pageSize;
            }

            this.InnerList = new List<T>();
            var data = source.Skip(skipCount).Take(pageSize).ToList();
            if (data != null)
            {
                this.InnerList.AddRange(data);
            }
        
        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">已分页的对象列表</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IQueryable<T> source, int totalRecords, int pageIndex, int pageSize)
        {
            this.TotalCount = totalRecords;

            this.TotalPages = totalRecords / pageSize;

            if (totalRecords % pageSize > 0)
                this.TotalPages++;

            this.PageSize = pageSize;

            this.PageIndex = pageIndex;

            this.InnerList = new List<T>();

            this.InnerList.AddRange(source.ToList());
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> InnerList { get; set; }

        #region IPagedList Members

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get { return (this.PageIndex * this.PageSize) <= this.TotalCount; }
        }

        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
