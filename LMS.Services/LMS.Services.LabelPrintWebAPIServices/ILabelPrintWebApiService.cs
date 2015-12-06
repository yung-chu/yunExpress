using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Services.LabelPrintWebAPIServices
{
    public interface ILabelPrintWebApiService
    {
        List<LabelPrintExt> GetLabelPrintExtList(IEnumerable<string> orderNumbers, string customercode);
    }
}
