using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class Vender
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public int VenderId { get; set; }
    }
    public class VenderModel
    {
        public int VenderId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Enabled { get; set; }
    }
}
