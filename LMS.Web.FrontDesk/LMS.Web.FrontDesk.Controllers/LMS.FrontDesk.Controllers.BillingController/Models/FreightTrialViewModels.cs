using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LighTake.Infrastructure.Common;

namespace LMS.FrontDesk.Controllers.BillingController.Models
{

	public class FreightTrialViewModels
	{
		public FreightTrialViewModels()
		{
			Filter = new FreightTrialFilterModel();
			FreightList = new List<FreightModel>();
		    ShowCategoryListModel = new ShowCategoryListModel();
		}

		public int GetId { get; set; }
		public List<FreightModel> FreightList { get; set; }
		public FreightTrialFilterModel Filter { get; set; }
        //产品服务, 新闻中心, 关于我们, 帮助中心 顶部列表
        public ShowCategoryListModel ShowCategoryListModel { get; set; }

	}





    public class FreightTrialFilterModel
    {
        public string Length { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public decimal? Weight { get; set; }

        public string CountryCode { get; set; }

        public int PackageType { get; set; }
    }

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
    public class FreightModel
    {
        public int ShippingMethodId { get; set; }

        public string ShippingMethodName { get; set; }

        public decimal Weight { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal RemoteAreaFee { get; set; }

        public decimal RegistrationFee { get; set; }

        public decimal SundryFee
        {
            get { return (OtherFee + RemoteAreaFee).NewRound(2); }
        }

        public decimal FuelFee { get; set; }

        public decimal OtherFee { get; set; }

        public decimal TariffPrepayFee { get; set; }

        public decimal TotalFee
        {
            get
            {
                return
                       (ShippingFee + RegistrationFee + FuelFee + OtherFee + RemoteAreaFee + TariffPrepayFee)
                           .NewRound(2);
            }
        }

        public string Remarks { get; set; }

        public string DeliveryTime { get; set; }
    }
}
