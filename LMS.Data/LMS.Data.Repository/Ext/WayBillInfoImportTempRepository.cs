using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class WayBillInfoImportTempRepository
    {
        /// <summary>
        /// 查询客户订单号是否存在
        /// </summary>
        /// <param name="customerOrderNumber"></param>
        /// <returns>返回存在的客户订单号</returns>
        public List<string> GetIsEixtCustomerOrderNumber(List<string> customerOrderNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
             
            if (customerOrderNumber.Any())
            {
                var CustomerOrderNumbers = new SqlParameter { ParameterName = "CustomerOrderNumbers", Value = string.Join(",", customerOrderNumber), DbType = DbType.String };
                if (ctx != null)
                {
                    var obj = ctx.ExecuteStoredProcedureList<string>("P_IsExitCustomerOrderNumber @CustomerOrderNumbers", CustomerOrderNumbers);

                    if (obj != null && obj.Count > 0)
                    {
                        return obj.ToList();
                    }
                }
            }
            return new List<string>();
        }
        /// <summary>
        /// 从运单临时表把数据插入运单表
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public bool ImportWayBillInfo(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int isSuccess = 0;
            if (wayBillNumbers.Any())
            {
                var WayBillNumber = new SqlParameter { ParameterName = "WayBillNumbers", Value = string.Join(",", wayBillNumbers), DbType = DbType.String };
                var isReturn = new SqlParameter { ParameterName = "isReturn", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
                if (ctx != null)
                {
                    var obj = ctx.ExecuteCommand("exec P_ImportWayBillInfo @WayBillNumbers,@isReturn output", WayBillNumber, isReturn);
                    Int32.TryParse(isReturn.Value.ToString(), out isSuccess);
                }
            }
            return isSuccess==1;
        }
    }
}
