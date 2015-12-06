using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public partial class DeliveryImportAccountCheck
    {
        public virtual bool IsTrue { get; set; }
        public virtual string ErrorReason { get; set; } 
    }
    public partial class ExpressDeliveryImportAccountCheck
    {
        public virtual string ErrorReason { get; set; }
        public virtual bool IsTrue { get; set; }
    }
}
