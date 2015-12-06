using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web
{
    public class ExcelXlsFileResult : FileContentResult
    {
        protected virtual string Extension
        {
            get { return ".xls"; }
        }

        protected ExcelXlsFileResult(byte[] fileContents, string contentType, string fileNameWithoutExtionsion)
            : base(fileContents, contentType)
        {
            FileDownloadName = getFileName(fileNameWithoutExtionsion);
        }

        public ExcelXlsFileResult(byte[] fileContents, string fileNameWithoutExtionsion)
            : this(fileContents, "application/vnd.ms-excel", fileNameWithoutExtionsion)
        { }

        protected string getFileName(string fileNameWithoutExtionsion)
        {
            return string.Concat(fileNameWithoutExtionsion, Extension);
        }
    }
}
