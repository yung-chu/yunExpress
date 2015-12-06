using LMS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Services.InStorageServices
{
    public class InStorageAsyncModel
    {
        public Process Process { get; set; }
        public DateTime InStorageCreateOn { get; set; }
        public WayBillInfoExt WayBillInfoExt { get; set; }

        public string UserUame { get; set; }
        public string InStorageID { get; set; }
    }
}
