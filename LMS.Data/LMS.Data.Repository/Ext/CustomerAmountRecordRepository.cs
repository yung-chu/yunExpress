using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class CustomerAmountRecordRepository
    {
        public int CreateCustomerAmountRecord(CustomerAmountRecordParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int isSuccess = 0;
            var CustomerCode = new SqlParameter { ParameterName = "customerCode", Value = param.CustomerCode, DbType = DbType.String };
            var WayBillNumber = new SqlParameter { ParameterName = "wayBillNumber", Value = param.WayBillNumber ?? "", DbType = DbType.String };
            var TransactionNo = new SqlParameter { ParameterName = "transactionNo", Value = param.TransactionNo ?? "", DbType = DbType.String };
            var MoneyChangeTypeId = new SqlParameter { ParameterName = "moneyChangeTypeID", Value = param.MoneyChangeTypeId, DbType = DbType.Int32 };
            var FeeTypeID = new SqlParameter { ParameterName = "feeTypeID", Value = param.FeeTypeId, DbType = DbType.Int32 };
            var Amount = new SqlParameter { ParameterName = "amount", Value = param.Amount, DbType = DbType.Decimal };
            var Remark = new SqlParameter { ParameterName = "remark", Value = param.Remark ?? "", DbType = DbType.String };
            var CreatedBy = new SqlParameter { ParameterName = "createdBy", Value = param.CreatedBy, DbType = DbType.String };
            var isReturn = new SqlParameter { ParameterName = "isReturn", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            if (ctx != null)
            {
                ctx.ExecuteCommand(
                    "Exec P_CustomerAmountRecord @customerCode,@wayBillNumber,@transactionNo,@moneyChangeTypeID,@feeTypeID,@amount,@remark,@createdBy,@isReturn output",
                    CustomerCode, WayBillNumber, TransactionNo, MoneyChangeTypeId, FeeTypeID, Amount, Remark, CreatedBy, isReturn);
                Int32.TryParse(isReturn.Value.ToString(), out isSuccess);
            }
            return isSuccess;
        }

        public IPagedList<CustomerAmountRecordExt> GetCustomerAmountList(AmountRecordSearchParam param, out decimal TotalInFee, out decimal TotalOutFee)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            TotalInFee = 0;
            TotalOutFee = 0;
            var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartDateTime, DbType = DbType.Time };
            var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndDateTime, DbType = DbType.Time };
            var TotalRecord = new SqlParameter { ParameterName = "TotalRecord", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var PageSize = new SqlParameter { ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32 };
            var PageIndex = new SqlParameter { ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32 };
            var TotalPage = new SqlParameter { ParameterName = "TotalPage", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var TotalInFeeParam = new SqlParameter { ParameterName = "TotalInFee", Value = 0.00, DbType = DbType.Decimal, Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
            var TotalOutFeeParam = new SqlParameter { ParameterName = "TotalOutFee", Value = 0.00, DbType = DbType.Decimal, Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<CustomerAmountRecordExt>("P_GetCustomerAmountRecordList"
                                                        , CustomerCode, StartTime, EndTime, TotalRecord, PageSize, PageIndex, TotalPage, TotalInFeeParam, TotalOutFeeParam);
                if (obj != null && obj.Count > 0)
                {
                    TotalInFee = decimal.Parse(TotalInFeeParam.Value.ToString());
                    TotalOutFee = decimal.Parse(TotalOutFeeParam.Value.ToString());
                    return new PagedList<CustomerAmountRecordExt>() { InnerList = obj.ToList(), PageIndex = param.Page, PageSize = param.PageSize, TotalCount = Int32.Parse(TotalRecord.Value.ToString()), TotalPages = Int32.Parse(TotalPage.Value.ToString()) };
                }
            }
            return new PagedList<CustomerAmountRecordExt>() { InnerList = null, PageIndex = param.Page, PageSize = param.PageSize, TotalCount = 0, TotalPages = 0 };

        }
    }
}
