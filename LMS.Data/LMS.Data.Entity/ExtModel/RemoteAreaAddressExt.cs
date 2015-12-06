using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class RemoteAreaAddressExt
	{
		public int RemoteAreaAddressId { get; set; }
		public int ShippingMethodId { get; set; }
		public string ShippingMethodName { get; set; }
		public string CountryCode { get; set; }//国家简码
		public string EName { get; set; }//国家英文名
		public string State { get; set; }
		public string StateCode { get; set; }
		public string City { get; set; }
		public string Zip { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedOn { get; set; }
		public string LastUpdatedBy { get; set; }
		public System.DateTime LastUpdatedOn { get; set; }
		public string ZipEnd { get; set; }

		public PageInfo PageInfoModel { get; set; }
	}
	public class PageInfo
	{
		public int PageCount { get; set; }
		public int TotalCount { get; set; }
	}
}
