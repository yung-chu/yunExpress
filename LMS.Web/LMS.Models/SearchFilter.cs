using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class SearchFilter
    {
        private int _page = 1;

        private int _pageSize = 20;

        public int Page { get { return _page; } set { _page = value; } }

        public virtual int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value < 1)
                    return;

                if (value > 200)
                {
                    _pageSize = 200;
                    return; ;
                }
                _pageSize = value;
            }
        }
    }
}
