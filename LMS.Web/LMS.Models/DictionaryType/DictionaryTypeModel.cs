using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class DictionaryTypeModel
    {
        public string DicTypeId { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string ParentId { get; set; }
        public string SpecialName { get; set; }
        public string Remark { get; set; }
        public int Sort { get; set; }
        public bool IsParent { get; set; }
        public bool IsEnable { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }


}
