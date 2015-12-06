using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class Menu
    {
        public Menu()
        {
            Items = new List<MenuItem>();
        }

        public IList<MenuItem> Items { get; set; }
    }

    public class MenuItem
    {
        public MenuItem()
        {
            SubItems = new List<MenuItem>();
        }

        public IList<MenuItem> SubItems { get; set; }

        public string Name { get; set; }

        public string NavigateUrl { get; set; }

        public string PermissionCode { get; set; }

        public bool IsActive { get; set; }
    }
}
