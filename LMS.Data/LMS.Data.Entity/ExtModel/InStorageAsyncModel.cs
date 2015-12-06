using LMS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class InStorageAsyncModel
    {
        
        public Process Process { get; set; } //已经禁用
        public DateTime InStorageCreateOn { get; set; }
        public Entity.WayBillInfoExt WayBillInfoExt { get; set; }

        public string UserUame { get; set; }
        public string InStorageID { get; set; }
        public string ReceivingBillID { get; set; }

        public DateTime BusinessDate { get; set; }
    }
}
