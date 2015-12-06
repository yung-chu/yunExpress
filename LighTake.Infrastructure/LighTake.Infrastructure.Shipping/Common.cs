using System;
using System.Collections.Generic;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    public class Common
    {
        ///<summary>
        ///向上取整（逢0.5kg向上取整--如1.1KG按1.5算，1.6KG，按2计算）
        ///</summary>
        ///<param name="source"></param>
        ///<returns></returns>
        public static decimal RoundUp05(decimal source)
        {
            var integerNum = (int)source;
            decimal decimalNum = source - integerNum;
            decimal result;


            if (decimalNum < 0.5m)
                return integerNum + 0.5m;
            if (decimalNum > 0.5m)
                result = integerNum + 1m;
            else
                result = source;

            return result;

        }

    }
}
