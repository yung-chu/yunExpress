using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;

namespace LMS.Services.SequenceNumber
{
    public class SequenceNumberService
    {
        /// <summary>
        /// 获取单号
        /// </summary>
        /// <param name="prefixCode">PrefixCode.InquiryID</param>
        /// <returns></returns>
        public static string GetSequenceNumber(string prefixCode)
        {
            return GetSequenceNumber(prefixCode, 1);
        }

        //获取流水号锁
        private static readonly object lockGetSequenceNumber=new object();

        public static string GetSequenceNumber(string prefixCode, int count)
        {
            ////防止并发时生成混乱
            lock (lockGetSequenceNumber)
            {
                using (var ctx = new LMS_DbContext())
                {
                    //return ctx.
                    return ctx.P_CreateSequenceNumber(prefixCode, count).SingleOrDefault();
                }
            }
        }

        private static readonly object lockGetWayBillNumber = new object();
        /// <summary>
        /// 新生成运单号规则
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        public static string GetWayBillNumber(string customerCode)
        {
            return GetWayBillNumber(customerCode, "1", 1);
        }
        public static string GetWayBillNumber(string customerCode, string area, int count)
        {
            lock (lockGetWayBillNumber)
            {
                using (var ctx=new LMS_DbContext())
                {
                    return ctx.P_CreateWayBillNumber(customerCode, area, count).SingleOrDefault();
                }
            }
        }
    }
}
