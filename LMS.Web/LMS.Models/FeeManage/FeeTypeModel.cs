using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class FeeTypeModel
    {
        public int FeeTypeID { get; set; }
        public string FeeTypeName { get; set; }
        public string CalculateExpression { get; set; }
        public bool IsEnable { get; set; }
        public string FeeTypeRemark { get; set; }
    }
}
