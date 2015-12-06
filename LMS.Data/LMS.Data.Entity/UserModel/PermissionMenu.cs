using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class PermissionMenu
    {
        public PermissionMenu()
        {
            Items = new List<PermissionMenuMenuItem>();
        }

        public IList<PermissionMenuMenuItem> Items { get; set; }
    }

}
