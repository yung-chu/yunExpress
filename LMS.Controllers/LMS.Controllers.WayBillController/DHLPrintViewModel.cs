using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.Data.Entity;

namespace LMS.Controllers.WayBillController
{
    public class DHLPrintViewModel
    {
        public DHLPrintViewModel()
        {
            WayBillInfos=new List<WayBillInfo>();
            ExpressAccountInfos=new List<ExpressAccountInfo>();
        }

        public List<WayBillInfo> WayBillInfos { get; set; }
        public List<ExpressAccountInfo> ExpressAccountInfos { get; set; }
        public Dictionary<int, string> ShippingMethodCodes { get; set; }
    }
    public class NLPOSTViewModel
    {
        public NLPOSTViewModel()
        {
            WayBillInfos = new List<WayBillInfo>();
        }
        public List<WayBillInfo> WayBillInfos { get; set; }
    }

    public class LithuaniaViewModel
    {
        public LithuaniaViewModel()
        {
            WayBillInfos=new List<WayBillInfo>();
        }

        public string MailNo { get; set; }
        public string AgentMailNo { get; set; }

        public List<WayBillInfo> WayBillInfos { get; set; }
    }
}