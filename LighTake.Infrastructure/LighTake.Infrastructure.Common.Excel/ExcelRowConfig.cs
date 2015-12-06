using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common.Excel
{
    public class ExcelRowConfig
    {
        public string FieldName { get; set; }

        private string _headText;
        public string HeadText
        {
            get
            {
                if (_headText.IsNullOrWhiteSpace())
                {
                    HeadText = FieldName;
                }
                return _headText;
            }
            set { _headText = value; }
        }
    }
}
