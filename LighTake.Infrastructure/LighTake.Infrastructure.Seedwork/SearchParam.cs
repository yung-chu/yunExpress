using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace LighTake.Infrastructure.Seedwork
{
    public class SearchParam
    {
        int _pageIndex = 1;

        int _pageSize = 20;

        string _orderby = "1=1";

        /// <summary>
        /// 当前页码
        /// </summary>
        public virtual int Page
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (value > 0)
                    _pageIndex = value;
            }
        }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public virtual int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value > 0)
                    _pageSize = value;
            }
        }

        /// <summary>
        /// 排序字符串
        /// </summary>
        public string OrderBy
        {
            get
            {
                return _orderby;
            }
            set
            {
                if (value != null)
                    _orderby = value;
            }
        }

        private IList<OrderByParam> _orderByParams;

        /// <summary>
        /// 排序参数
        /// </summary>
        public IList<OrderByParam> OrderByParams
        {
            get { return _orderByParams ?? (_orderByParams = new List<OrderByParam>()); }

            set { _orderByParams = value; }
        }

        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode { get; set; }

    }
}
