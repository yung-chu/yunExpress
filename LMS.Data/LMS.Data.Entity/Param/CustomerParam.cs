using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public  class CustomerParam
    {
        public string CustomerCode { get; set; }
    }

	public class SearchCustomerParam : SearchParam
	{
		public string CustomerCode { get; set; }
		public int? Status { get; set; }
	}


	public class CustomerInfoParam
	{
		public Guid CustomerId { get; set; }
		public int? CustomerTypeId { get; set; }
		public string CustomerCode { get; set; }
		public string Name { get; set; }
		public string EnName { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string QQ { get; set; }
		public string MSN { get; set; }
		public string Skype { get; set; }
		public string Country { get; set; }
		public string Province { get; set; }
		public string Fax { get; set; }
		public string PostCode { get; set; }
		public string LastUpdatedBy { get; set; }//lms 创建,编辑人
		public int Status { get; set; }
	}
}
