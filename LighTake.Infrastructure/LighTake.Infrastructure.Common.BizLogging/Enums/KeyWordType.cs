using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LighTake.Infrastructure.Common.BizLogging
{
    /// <summary>
    /// 关键词类型
    /// </summary>
    public enum KeywordType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
		/// lms-- WayBillNumber
        /// </summary>
        WayBillNumber = 1,

		/// <summary>
		///lms-CustomerOrderNumber
		/// </summary>
		CustomerOrderNumber = 2,


		/// <summary>
		///lms lis CustomerCode
		/// </summary>
		CustomerCode = 3,

		/// <summary>
		/// Lms WayBillTemplateId
		/// </summary>
		Lms_WayBillTemplateId = 4,


		/// <summary>
		/// Lis TemplateId
		/// </summary>
		Lis_TemplateId=5,

		/// <summary>
		///lis lms- ShippingMethodId
		/// </summary>
		ShippingMethodId = 6,



		/// <summary>
		///lms- CategoryID
		/// </summary>
		CategoryId = 7,

		/// <summary>
		///lms- ArticleID
		/// </summary>
		ArticleId = 8,


		/// <summary>
		///lis- VenderId
		/// </summary>
		VenderId = 9




    }
}
