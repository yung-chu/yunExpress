using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.FubController
{
    public class InTrackingLogInfoModel
    {
        public InTrackingLogInfoModel()
        {
            ListModel = new List<InTrackingLogInfoExt>();
        }

        public List<LMS.Data.Entity.InTrackingLogInfoExt> ListModel { get; set; }
        public string Number { get; set; }
        public int? Error { get; set; }
        public string NoQueryNumber { get; set; }//查询不到的单号
    }


}  
