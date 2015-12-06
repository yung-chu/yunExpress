using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Services.SF.Model
{
    public class OrderSfModel
    {
        public OrderSfModel()
        {
            Applications=new List<ApplicationSfModel>();
        }
        public string OrderId { get; set; }
        public int ExpressType { get; set; }
        public string ShippingName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingTel { get; set; }
        public string ShippingPhone { get; set; }
        public int ParcelQuantity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCity { get; set; }
        public string CountryCode { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingAddress { get; set; }
        public decimal ApplicationTotalPrice { get; set; }
        public decimal ApplicationTotalWeight { get; set; }
        public string Remark { get; set; }
        public List<ApplicationSfModel> Applications { get; set; }
    }
    public class ApplicationSfModel
    {
        public string ApplicationName { get; set; }
        public int Qty { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class SfOrderResponse
    {
        public string OrderId { get; set; }
        public string MailNo { get; set; }
        public string OriginCode { get; set; }
        public string DestCode { get; set; }
        public string Remark { get; set; }
        public string AgentMailNo { get; set; }
        public string FilterResult { get; set; }
        public string TrackNumber { get; set; }
    }
    public class SfResponse
    {
        public SfResponse()
        {
            Model = new SfOrderResponse();
        }
        public string ErrorMsg { get; set; }
        public SfOrderResponse Model { get; set; }
    }
}
