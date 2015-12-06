using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CountryExt:Country
    {
        /// <summary>
        /// 是否常用国家
        /// </summary>
        public bool? IsCommonCountry { get; set; }

        /// <summary>
        /// 国家对应的拼音
        /// </summary>
        public string CountryPinyin { get; set; }


        /// <summary>
        /// 能否使用
        /// </summary>
        public bool CanSelect { get; set; }
    }
}
