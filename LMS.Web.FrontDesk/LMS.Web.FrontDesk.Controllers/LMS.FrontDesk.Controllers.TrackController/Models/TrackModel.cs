using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.FrontDesk.Controllers.TrackController.Models
{
    public class TrackModel
    {
        public TrackModel()
        {
            ListContactTable = new List<ContactTable>();
            ListHandleing = new List<ContactTable>();
            ListSendPackage = new List<ContactTable>();
            ListSuccessSign = new List<ContactTable>();
            ListIsHold = new List<ContactTable>();
            NoQueryWaybillnumbers = new List<string>();
        }

        public List<ContactTable> ListContactTable { get; set; }
        public string WayBillNumber { get; set; }
     
        //处理中
        public List<ContactTable> ListHandleing { get; set; }

        //运输中
        public List<ContactTable> ListSendPackage { get; set; }

        //成功签收
        public List<ContactTable> ListSuccessSign { get; set; }

        //可能异常
        public List<ContactTable> ListIsHold { get; set; }

        //查询不到
        public List<string> NoQueryWaybillnumbers = new List<string>();

    }
}
