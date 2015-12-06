using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial interface IWayBillTemplateRepository
    {
        IEnumerable<WayBillTemplateExt> GetGetWayBillTemplateExtByName(string templateName);

        IEnumerable<WayBillTemplateExt> GetWayBillTemplateList(IEnumerable<int> shippingMethodIds,
                                                               string templateTypeId);

        WayBillTemplateExt GetWayBillTemplate(int shippingMethodId, string templateName);

        bool GetCanPrint(string templateName, string number);

        List<LabelPrintModel> GetLabelPrint(List<string> orderNumbers, string customerCode, string templateTypeId);
    }
}
