namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class ApplicationInfoModel
    {
        public int? CustomerOrderID { get; set; }
        public string WayBillNumber { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public int Qty { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string HSCode { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
        public string ProductUrl { get; set; }
    }
}
