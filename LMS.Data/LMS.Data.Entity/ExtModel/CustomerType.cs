using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CustomerType
    {
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
    }
    public class CustomerTypeModel
    {
        public int CustomerTypeId { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }
    }
}
