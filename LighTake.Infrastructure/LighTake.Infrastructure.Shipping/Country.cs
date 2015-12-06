using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    public class Country 
    {
        #region Properties

        /// <summary>
        /// 国家代码（two letter ISO code）
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序（升序）
        /// </summary>
        public int DisplayOrder {get; set;}

        #endregion
    }
}
