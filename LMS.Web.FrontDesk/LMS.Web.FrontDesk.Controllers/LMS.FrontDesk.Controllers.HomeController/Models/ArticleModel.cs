using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.FrontDesk.Controllers.HomeController.Models
{
    /// <summary>
    ///  新闻表
    /// </summary>
    public class ArticleModel
    {
        public int ArticleID { get; set; }
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Source { get; set; }
        public string Author { get; set; }
        public DateTime? PublishedTime { get; set; }
        public string Pic { get; set; }
        public int Sort { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int? Status { get; set; }
    }
}
