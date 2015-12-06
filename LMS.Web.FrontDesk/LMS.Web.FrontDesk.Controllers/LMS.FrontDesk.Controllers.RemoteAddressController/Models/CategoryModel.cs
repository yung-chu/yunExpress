using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.FrontDesk.Controllers.RemoteAddressController
{
	public class CategoryModel
	{
		public int CategoryID { get; set; }
		public string Name { get; set; }
		public int ParentID { get; set; }
		public string ParentPath { get; set; }
		public int Level { get; set; }
		public int Sort { get; set; }
		public string Pic { get; set; }
		public string Description { get; set; }
		public string SeoTitle { get; set; }
		public string SeoKeywords { get; set; }
		public string SeoDescription { get; set; }
		public int Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedOn { get; set; }
		public string LastUpdatedBy { get; set; }
		public DateTime? LastUpdatedOn { get; set; }
		public string EnglishName { get; set; }
	}
}