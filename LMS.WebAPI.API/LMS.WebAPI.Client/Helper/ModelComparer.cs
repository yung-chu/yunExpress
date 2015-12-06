using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.WebAPI.Client.Models;

namespace LMS.WebAPI.Client.Helper
{
    public class WayBillModelComparer : IEqualityComparer<WayBillModel>
    {
        public bool Equals(WayBillModel x, WayBillModel y)
        {
            return x.OrderNumber.ToUpper() == y.OrderNumber.ToUpper();
        }

        public int GetHashCode(WayBillModel obj)
        {
            return obj.OrderNumber.ToUpper().GetHashCode();
        }
    }
}