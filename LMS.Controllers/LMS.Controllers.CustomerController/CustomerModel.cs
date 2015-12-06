using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Controllers.CustomerController
{
    public class CustomerModel
    {
        public Guid CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public int CustomerTypeID { get; set; }
        public int PaymentTypeID { get; set; }
        public string PaymentName { get; set; }
    }
    public class CustomerListModel
    {
        public Guid CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string AccountID { get; set; }
        public string AccountPassWord { get; set; }
        public Nullable<int> CustomerTypeID { get; set; }
        public string Name { get; set; }
        //public string EnName { get; set; }
        public Nullable<int> PaymentTypeID { get; set; }
        public string Address { get; set; }
        //public string EnAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Tele { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string PostCode { get; set; }
        public string QQ { get; set; }
        public string MSN { get; set; }
        public string Skype { get; set; }
        public int Status { get; set; }
        //public string LastLoginIP { get; set; }
        public Nullable<int> LoginCount { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LinkMan { get; set; }
        public string WebSite { get; set; }
        public decimal Balance { get; set; }
        public bool EnableCredit { get; set; }
        public decimal MaxDelinquentAmounts { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
		public string CustomerManager { get; set; }
		public string CurrentStatus { get; set; }

    }
    public class CustomerRechargeModel
    {
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public int MoneyChangeTypeID { get; set; }
        public decimal Amount { get; set; }
        public int FeeTypeID { get; set; }
        public int SelectNumberTypeID { get; set; }
        public string NumberValue { get; set; }
        public string Remark { get; set; }
    }
    public class CustomerCreditModel
    {
        public int ID { get; set; }
        public string CustomerCode { get; set; }
        public Nullable<int> RechargeType { get; set; }
        public string VoucherPath { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string TransactionNo { get; set; }
        public string Remark { get; set; }
    }
    public class CustomerAmountRecordModel
    {
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
        public string WayBillNumber { get; set; }
        public string TransactionNo { get; set; }
        public Nullable<int> MoneyChangeTypeID { get; set; }
        public string MoneyChangeTypeShortName { get; set; }
        public Nullable<int> FeeTypeID { get; set; }
        public string FeeTypeName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Remark { get; set; }
    }
    public class CustomerAmountRecordListModel
    {
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TransactionNo { get; set; }
        public Nullable<int> MoneyChangeTypeID { get; set; }
        public Nullable<int> FeeTypeID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string Remark { get; set; }
        public string MoneyChangeTypeShortName { get; set; }
    }
    public class CustomerAmountRecordToExecl
    {
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
        //public string CustomerOrderNumber { get; set; }
        //public string TransactionNo { get; set; }
        //public Nullable<int> MoneyChangeTypeID { get; set; }
        public string MoneyChangeTypeShortName { get; set; }
        public decimal InCash { get; set; }
        public decimal OutCash { get; set; }
        //public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public System.DateTime CreatedOn { get; set; }
        //public string CreatedBy { get; set; }
        public string Remark { get; set; }
    }
}