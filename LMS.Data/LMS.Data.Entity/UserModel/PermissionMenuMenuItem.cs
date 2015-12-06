using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class PermissionMenuMenuItem : Permission
    {
        public PermissionMenuMenuItem()
        {
            SubItems = new List<PermissionMenuMenuItem>();
        }

        public IList<PermissionMenuMenuItem> SubItems { get; set; }
    }
}
