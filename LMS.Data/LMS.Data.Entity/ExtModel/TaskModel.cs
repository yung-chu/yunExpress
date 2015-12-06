using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class TaskModel
    {
        public long TaskID { get; set; }
        public int TaskType { get; set; }
        public string TaskKey { get; set; }
        public int Status { get; set; }
        public InStorageAsyncModel Body { get; set; }
        public int Times { get; set; }
        public string Error { get; set; }
        public System.DateTime CreateOn { get; set; }
    }

}
