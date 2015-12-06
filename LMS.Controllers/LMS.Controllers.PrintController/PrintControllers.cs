using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Core;

namespace LMS.Controllers.PrintController
{
    public class PrintController : Controller
    {
        public ActionResult DownLoadPdf(string wayBillNumber)
        {
            return File(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf", "application/pdf");
        }
    }
}