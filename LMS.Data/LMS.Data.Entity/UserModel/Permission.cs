using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class Permission
    {
        public int ModuleId { get; set; }

        public string Name { get; set; }

        public string NavigateUrl { get; set; }

        public string PermissionCode { get; set; }

        public int ParentID { get; set; }
    }
}
