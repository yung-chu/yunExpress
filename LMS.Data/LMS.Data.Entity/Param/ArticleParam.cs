using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class ArticleParam : SearchParam
    {
        public string Sort { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
        public string Author { get; set; }
        public int? CategoryID { get; set; }
        public int? Status { get; set; }
        public DateTime? StartPublishedTime { get; set; }
        public DateTime? EndPublishedTime { get; set; }
        public DateTime PublishedTime { get; set; }
        public DateTime PublishedEndTime { get; set; }
    }

    public  class WebsiteArticleParam : SearchParam
    {

        public override int PageSize
        {
            get
            {
                return 10;
            }
         
        }
        public int CategoryID { get; set; }

        public int? Status
        {
            get { return 1; }
        }
    }
}
