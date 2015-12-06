using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class CategoryModel
    {
        public virtual int CategoryID { get; set; }
        public virtual string Name { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string ParentPath { get; set; }
        public virtual int Level { get; set; }
        public virtual int Sort { get; set; }
        public virtual string Pic { get; set; }
        public virtual string Description { get; set; }
        public virtual string SeoTitle { get; set; }
        public virtual string SeoKeywords { get; set; }
        public virtual string SeoDescription { get; set; }
        public virtual int Status { get; set; }
        public string _parentId { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}
