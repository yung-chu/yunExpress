using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class WayBillTemplateExt : WayBillTemplate
    {
        public virtual string TemplateHead { get; set; }
        public virtual string TemplateBodyContent { get; set; }
    }
}
