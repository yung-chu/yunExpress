using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ArticleExt : Article
    {
        public virtual string CategoryName { get; set; }
        public virtual string UserName { get; set; }
    }
}
