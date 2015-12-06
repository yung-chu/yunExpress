using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Web.Models
{
    public class SearchFilter
    {
        private int _page = 1;

        private int _pageSize = 20;

        public int Page { get { return _page; } set { _page = value; } }

        public virtual int PageSize { get { return _pageSize; } set { _pageSize = value; } }

    }
	public class FinancialSearchFilter
	{
		private int _page = 1;

		private int _pageSize = 300;

		public int Page { get { return _page; } set { _page = value; } }

		public virtual int PageSize { get { return _pageSize; } set { _pageSize = value; } }

	}
}
