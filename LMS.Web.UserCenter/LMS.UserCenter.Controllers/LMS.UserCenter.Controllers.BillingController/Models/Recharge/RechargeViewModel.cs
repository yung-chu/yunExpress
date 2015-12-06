using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{

    public class SelectListModel
    {
        public string SelectValue { get; set; }
        public string SelectName { get; set; }
    }

    public class RechargeViewModel
    {
        public RechargeViewModel()
        {
            CustomerBalances = new CustomerBalances();
            RechargeWayList = new List<SelectListModel>();
        }
        public CustomerBalances CustomerBalances { get; set; } //账户余额实体
        public List<SelectListModel> RechargeWayList { get; set; } //支付方式集合
    }

}