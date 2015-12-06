using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Repository
{
    public partial interface IWayBillInfoImportTempRepository
    {
        List<string> GetIsEixtCustomerOrderNumber(List<string> customerOrderNumber );
        bool ImportWayBillInfo(List<string> wayBillNumbers);
    }
}
