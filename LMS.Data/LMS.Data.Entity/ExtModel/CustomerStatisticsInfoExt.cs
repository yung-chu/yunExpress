using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CustomerStatisticsInfoExt
    {
        public decimal Recharge { get; set; }
        public decimal TakeOffMoney { get; set; }
        public decimal Balance { get; set; }
        public int UnconfirmOrder { get; set; }
        public int ConfirmOrder { get; set; }
        public int SubmitOrder { get; set; }
        public int HaveOrder { get; set; }
        public int SendOrder { get; set; }
        public int HoldOrder { get; set; }
        public int TotalOrder { get; set; }
        public int SubmitingOrder { get; set; }
        public int SubmitFailOrder { get; set; }
    }
}
