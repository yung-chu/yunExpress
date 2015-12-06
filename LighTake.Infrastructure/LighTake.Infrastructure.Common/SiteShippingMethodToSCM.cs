using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
  public   class SiteShippingMethodToSCM
    {
       public static int ConvertShippingMethod(int shippingMethod)
       {
           if (shippingMethod <= 3 || shippingMethod == 5) return 1;
           else if (shippingMethod == 4) return 2;
           else if (shippingMethod == 6) return 3;
           else if (shippingMethod == 7) return 4;
           else return 0;
       }
    }
}
