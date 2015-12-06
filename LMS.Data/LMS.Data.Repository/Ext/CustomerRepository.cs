using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class CustomerRepository
    {
        public CustomerStatisticsInfoExt GetCustomerStatisticsInfo(string customerCode, out decimal recharge, out decimal takeOffMoney, out decimal balance, out int unconfirmOrder, out int confirmOrder, out int submitOrder, out int haveOrder, out int sendOrder, out int holdOrder, out int totalOrder, out int submitingOrder, out int submitFailOrder)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            //recharge = 0; takeOffMoney = 0; balance = 0; unconfirmOrder = 0; confirmOrder = 0; submitOrder = 0; holdOrder = 0;
            var _customerCode = new SqlParameter { ParameterName = "CustomerCode", Value = customerCode, DbType = DbType.String };
            var _recharge = new SqlParameter { ParameterName = "Recharge", Value = 0, DbType = DbType.Decimal, Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
            var _takeOffMoney = new SqlParameter { ParameterName = "TakeOffMoney", Value = 0, DbType = DbType.Decimal, Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
            var _balance = new SqlParameter { ParameterName = "Balance", Value = 0, DbType = DbType.Decimal, Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
            var _unconfirmOrder = new SqlParameter { ParameterName = "UnconfirmOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _confirmOrder = new SqlParameter { ParameterName = "ConfirmOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _submitOrder = new SqlParameter { ParameterName = "SubmitOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _haveOrder = new SqlParameter { ParameterName = "HaveOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _sendOrder = new SqlParameter { ParameterName = "SendOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _holdOrder = new SqlParameter { ParameterName = "HoldOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _totalOrder = new SqlParameter { ParameterName = "TotalOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _submitingOrder = new SqlParameter { ParameterName = "SubmitingOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            var _submitFailOrder = new SqlParameter { ParameterName = "SubmitFailOrder", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<CustomerStatisticsInfoExt>("P_CustomerStatisticsInfo"
                        , _customerCode, _recharge, _takeOffMoney, _balance, _unconfirmOrder, _confirmOrder, _submitOrder, _haveOrder, _sendOrder, _holdOrder, _totalOrder, _submitingOrder, _submitFailOrder);
            }

            return new CustomerStatisticsInfoExt
            {
                Recharge = recharge = decimal.Parse(String.IsNullOrEmpty(_recharge.Value.ToString()) ? "0" : _recharge.Value.ToString()),
                TakeOffMoney = takeOffMoney = decimal.Parse(String.IsNullOrEmpty(_takeOffMoney.Value.ToString()) ? "0" : _takeOffMoney.Value.ToString()),
                Balance = balance = decimal.Parse(String.IsNullOrEmpty(_balance.Value.ToString()) ? "0" : _balance.Value.ToString()),
                UnconfirmOrder = unconfirmOrder = int.Parse(String.IsNullOrEmpty(_unconfirmOrder.Value.ToString()) ? "0" : _unconfirmOrder.Value.ToString()),
                ConfirmOrder = confirmOrder = int.Parse(String.IsNullOrEmpty(_confirmOrder.Value.ToString()) ? "0" : _confirmOrder.Value.ToString()),
                SubmitOrder = submitOrder = int.Parse(String.IsNullOrEmpty(_submitOrder.Value.ToString()) ? "0" : _submitOrder.Value.ToString()),

                HaveOrder = haveOrder = int.Parse(String.IsNullOrEmpty(_haveOrder.Value.ToString()) ? "0" : _haveOrder.Value.ToString()),
                SendOrder = sendOrder = int.Parse(String.IsNullOrEmpty(_sendOrder.Value.ToString()) ? "0" : _sendOrder.Value.ToString()),
                HoldOrder = holdOrder = int.Parse(String.IsNullOrEmpty(_holdOrder.Value.ToString()) ? "0" : _holdOrder.Value.ToString()),
                TotalOrder = totalOrder = int.Parse(String.IsNullOrEmpty(_totalOrder.Value.ToString()) ? "0" : _totalOrder.Value.ToString()),
                SubmitingOrder = submitingOrder = int.Parse(String.IsNullOrEmpty(_submitingOrder.Value.ToString()) ? "0" : _submitingOrder.Value.ToString()),
                SubmitFailOrder = submitFailOrder = int.Parse(String.IsNullOrEmpty(_submitFailOrder.Value.ToString()) ? "0" : _submitFailOrder.Value.ToString()),
            };

        }

        public List<Customer> GetCustomerByReceivingExpenseList(string keyword, DateTime? StartTime, DateTime? EndTime)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Expression<Func<Customer, bool>> filter = o => true;
            filter = filter.AndIf(p=>(p.CustomerCode.Contains(keyword) || p.Name.Contains(keyword)),!string.IsNullOrWhiteSpace(keyword));
            Expression<Func<ReceivingExpens, bool>> rExpression = o => true;
            rExpression = rExpression.AndIf(p => p.AcceptanceDate >= StartTime, StartTime.HasValue)
                                     .AndIf(p => p.AcceptanceDate <= EndTime, EndTime.HasValue);
            var list = (from c in ctx.Customers.Where(filter)
                       where
                           (from w in ctx.ReceivingExpenses.Where(rExpression) select w.WayBillInfo.CustomerCode)
                           .Contains(c.CustomerCode)
                        orderby c.CustomerCode
                       select c).ToList();
            return list;
        }

		public IPagedList<CustomerExt> CustomerList(SearchCustomerParam param)
		{
			var ctx = this.UnitOfWork as LMS_DbContext;
			ctx.Configuration.AutoDetectChangesEnabled = false;
			ctx.Configuration.LazyLoadingEnabled = false;//不延迟加载
			Check.Argument.IsNotNull(ctx, "数据库对象");
			Expression<Func<Customer, bool>> filter = o => true;

			filter = filter.AndIf(a => a.Name.Contains(param.CustomerCode) || a.CustomerCode.Contains(param.CustomerCode),
				!string.IsNullOrEmpty(param.CustomerCode))
				.AndIf(a => a.Status == param.Status, param.Status != null);

			var result = from a in ctx.Customers.Where(filter)
						 orderby a.CreatedOn descending
						 select new CustomerExt
						 {
							 CustomerID=a.CustomerID,
							 CustomerCode=a.CustomerCode,
							 Name=a.Name,
							 Status=a.Status,
							 LinkMan=a.LinkMan,
							 Tele=a.Tele,
							 CustomerManager=a.CustomerManager,
							 CreatedOn=a.CreatedOn,
							 Balance=a.CustomerBalance.Balance.HasValue?a.CustomerBalance.Balance.Value:0
						 };

			return result.ToPagedList(param.Page,param.PageSize);

		}
    }
}
