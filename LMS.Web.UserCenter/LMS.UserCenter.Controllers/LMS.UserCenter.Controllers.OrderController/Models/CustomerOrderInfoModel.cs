using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FluentValidation.Attributes;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using FluentValidation;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    [Validator(typeof(CustomerOrderInfoModelValidator))]
    public class CustomerOrderInfoModel
    {
        public CustomerOrderInfoModel()
        {
            this.ApplicationInfoList = new List<ApplicationInfoModel>();
            PackageNumberValue = "1";

        }

        public int GoodsTypeID { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingFirstLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingTaxId { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderFirstLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }

        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }

        public decimal? Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public decimal? SettleWeight { get; set; }
        public int? PackageNumber { get; set; }
        public string PackageNumberValue { get; set; }


        public bool IsReturn { get; set; }
        public bool IsInsured { get; set; }
        public bool IsBattery { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsHold { get; set; }

        public int InsuredID { get; set; }
        public int SensitiveTypeID { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string ProductDetail { get; set; }
        public string AbnormalDescription { get; set; }
        public string TrackingNumber { get; set; }
        public string RawTrackingNumber { get; set; }
        public Nullable<System.DateTime> TransferOrderDate { get; set; }
        public string CountryName { get; set; }

        public string CountryChineseName { get; set; }

        public string BarCode { get; set; }

        public string BarCode128 { get; set; }

        public string CustomerOrderNumberCode39 { get; set; }
        public string CustomerOrderNumberCode128 { get; set; }
        public string CustomerOrderNumberCode128L { get; set; }
        public string TrackingNumberCode39 { get; set; }
        public string TrackingNumberCode128 { get; set; }
        public string WayBillNumberCode39 { get; set; }
        public string WayBillNumberCode128 { get; set; }
        public string CustomerOrderNumberCode128Lh { get; set; }

        public int? ShippingZone { get; set; }

        public string ReturnUrl { get; set; }
        public string InsureAmountValue { get; set; }
        public string InsuredName { get; set; }
        public string SensitiveTypeName { get; set; }
        public decimal? InsureAmount { get; set; }
        public string AppLicationTypeId { get; set; }
        public int AppLicationType { get; set; }
        public string InsuredValue { get; set; }
        public string InsuredCalculationId { get; set; }
        public List<SelectListItem> AppLicationTypes { get; set; }
        public List<SelectListItem> InsuredCalculationsTypes { get; set; }
        public List<ApplicationInfoModel> ApplicationInfoList { get; set; }
        public List<WayBillInfo> WayBillInfos { get; set; }

        public bool EnableTariffPrepay { get; set; }

        public string Remark { get; set; }

        //提交失败订单编辑时可保存
        public int? SubmitFailFlag { get; set; }

        /// <summary>
        /// 格口号
        /// </summary>
        public int MouthNumber { get; set; }

        //广州小包专用发货地址
        public string Address { get; set; }

        //广州小包专用发货人
        public string Name { get; set; }

        //分拣标识
        public string SortingIdentity { get; set; }

        //带电标识
        public string BatteryIdentity { get; set; }
    }



    public class CustomerOrderInfoModelValidator : AbstractValidator<CustomerOrderInfoModel>
    {
        public CustomerOrderInfoModelValidator()
        {
            RuleFor(s => s.GoodsTypeID)
                .NotEmpty().WithMessage("[{PropertyName}]必须为大于零的整数")
                .GreaterThan(0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("包裹类型");
            RuleFor(s => s.Weight).NotEmpty().WithMessage(string.Format("[包裹重量]必须大于零"))
                                  .GreaterThan(0).WithMessage("[包裹重量]必须大于零");
            RuleFor(s => s.GoodsTypeID).GreaterThan(0).WithMessage(string.Format("[包裹类型]必须为大于零的整数"));
            RuleFor(s => s.CustomerOrderNumber).NotEmpty().WithMessage(string.Format("[客户订单号]不能为空"));
            RuleFor(s => (s.ShippingFirstName + s.ShippingLastName))
                .NotEmpty()
                .WithMessage(string.Format("[收件人姓名]不能为空"))
                .WithName("ShippingFirstName");
            //RuleFor(s => s.ShippingLastName).NotEmpty().WithMessage(string.Format("[收件人名]不能为空"));
            RuleFor(s => s.CountryCode).NotEmpty().WithMessage(string.Format("请选择[收件人国家]"));
            RuleFor(s => s.ShippingAddress).NotEmpty().WithMessage("[{PropertyName}]不能为空")
                                           .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                                           .WithName("收件人地址");
            RuleFor(s => s.ShippingState)
                .NotEmpty().When(t => t.ShippingMethodId != 34 && t.ShippingMethodId != 1095)
                .WithMessage(string.Format("[州/省]不能为空"));
            RuleFor(s => s.ShippingCity)
                 .NotEmpty().WithMessage(string.Format("[城市]不能为空"));
            RuleFor(s => s.ShippingZip)
                .NotEmpty().When(t => t.ShippingMethodId != 34 && t.ShippingMethodId != 1095)
                .WithMessage(string.Format("[邮编]不能为空"));
            RuleFor(s => s.ShippingMethodId).NotEmpty().WithMessage(string.Format("请选择[运输方式]"));
            RuleFor(s => s.ShippingMethodId).NotEqual(0).WithMessage(string.Format("请选择[运输方式]"));

        }


    }

    public class CustomerOrderInfoModelNlPostValidator : AbstractValidator<CustomerOrderInfoModel>
    {
        public CustomerOrderInfoModelNlPostValidator()
        {
            RuleFor(s => s.CustomerOrderNumber).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                 .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                 .WithName("客户订单号");
            RuleFor(s => s.ShippingFirstName)
                .NotEmpty().WithMessage("{PropertyName}]不能为空")
                .Length(1, 60).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓");
            RuleFor(s => s.ShippingFirstName + " " + s.ShippingLastName)
                .Length(1, 60).When(s => !string.IsNullOrWhiteSpace(s.ShippingFirstName))
                .WithMessage("[{PropertyName}]姓名总和长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人名");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().When(s => string.IsNullOrWhiteSpace(s.ShippingAddress1) && string.IsNullOrWhiteSpace(s.ShippingAddress2)).WithMessage("[{PropertyName}]不能为空")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingAddress1)
                .NotEmpty().When(s => string.IsNullOrWhiteSpace(s.ShippingAddress) && string.IsNullOrWhiteSpace(s.ShippingAddress2)).WithMessage("[{PropertyName}]不能为空")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingAddress2)
                .NotEmpty().When(s => string.IsNullOrWhiteSpace(s.ShippingAddress1) && string.IsNullOrWhiteSpace(s.ShippingAddress)).WithMessage("[{PropertyName}]不能为空")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingAddress + " " + s.ShippingAddress1 + " " + s.ShippingAddress2)
                .Length(1, 200).When(s => !string.IsNullOrWhiteSpace(s.ShippingAddress) || !string.IsNullOrWhiteSpace(s.ShippingAddress1) || !string.IsNullOrWhiteSpace(s.ShippingAddress2))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 30).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("城市");
            RuleFor(s => s.ShippingState)
                .Length(1, 30).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("省/州");
            RuleFor(s => s.ShippingZip)
                .Length(1, 15).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingZip))
                .WithName("邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").WithMessage("[{PropertyName}]只能是数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("电话");
        }
        private bool IsNoChinese(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var regex = new Regex(@"([\u4E00-\u9FA5]|[\uFE30-\uFFA0])+");
                return !regex.IsMatch(str);
            }
            else
            {
                return true;
            }
        }
    }

    public class ApplicationInfoModelValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .Length(1, 10).When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]只能是数字")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.PickingName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.Remark)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");
            RuleFor(s => s.ProductUrl)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.ProductUrl)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("销售链接");
        }
    }



    public class CustomerOrderViewModels
    {
        public CustomerOrderViewModels()
        {
            OrderList = new PagedList<CustomerOrderInfoModel>();
            Filter = new CustomerOrderFilter();
            AddressLabel = new List<SelectListItem>();
            PrintTemplate = new List<SelectListItem>();
            SearchWheres = new List<SelectListItem>();
            FieldItems = GetFieldItems();
        }

        public PagedList<CustomerOrderInfoModel> OrderList { get; set; }

        public CustomerOrderFilter Filter { get; set; }

        public List<SelectListItem> AddressLabel { get; set; }

        public List<SelectListItem> PrintTemplate { get; set; }

        public List<ExportFieldItem> FieldItems { get; set; }

        public List<SelectListItem> SearchWheres { get; set; }

        public List<ExportFieldItem> GetFieldItems()
        {
            var fieldItems = new List<ExportFieldItem>() { 
                new ExportFieldItem() {Id =1, GroupName = "运单信息", Value = "WayBillNumber", Text = "运单号", Select = false },
                new ExportFieldItem() {Id =2,GroupName = "运单信息", Value = "CustomerOrderNumber", Text = "客户订单号", Select = false },
                new ExportFieldItem() {Id =3,GroupName = "运单信息", Value = "ShippingMethodName", Text = "运输方式", Select = false },
                new ExportFieldItem() {Id =4,GroupName = "运单信息", Value = "TrackingNumber", Text = "新跟踪号", Select = false },
                new ExportFieldItem() {Id =5,GroupName = "运单信息", Value = "RawTrackingNumber", Text = "原踪号", Select = false },
                new ExportFieldItem() {Id =6,GroupName = "运单信息", Value = "TransferOrderDate", Text = "转单时间", Select = false },
                new ExportFieldItem() {Id =26,GroupName = "运单信息", Value = "InsuredName", Text = "保险类型", Select = false },
                new ExportFieldItem() {Id =27,GroupName = "运单信息", Value = "InsureAmount", Text = "保险价值", Select = false },
                new ExportFieldItem() {Id =15,GroupName = "运单信息", Value = "CreatedOn", Text = "订单提交时间", Select = false },
                new ExportFieldItem() {Id =16,GroupName = "运单信息", Value = "DeliveryDate", Text = "订单发货时间", Select = false },
                new ExportFieldItem() {Id =30,GroupName = "包裹信息", Value = "PackageNumber", Text = "件数", Select = false },
                new ExportFieldItem() {Id =32,GroupName = "包裹信息", Value = "Weight", Text = "包裹重量Kg", Select = false },
                new ExportFieldItem() {Id =33,GroupName = "包裹信息", Value = "SettleWeight", Text = "计费重量Kg", Select = false },
                new ExportFieldItem() {Id =31,GroupName = "包裹信息", Value = "Length&Width&Height", Text = "长*宽*高 CM", Select = false },
                new ExportFieldItem() {Id =25,GroupName = "包裹信息", Value = "IsReturn", Text = "是否退回", Select = false },
                new ExportFieldItem() {Id =28,GroupName = "包裹信息", Value = "SensitiveTypeName", Text = "敏感货物", Select = false },
                new ExportFieldItem() {Id =29,GroupName = "包裹信息", Value = "AppLicationTypeId", Text = "申报类型", Select = false },
                new ExportFieldItem() {Id =8,GroupName = "收货人信息", Value = "ShippingFirstLastName", Text = "收货人姓名", Select = false },
                new ExportFieldItem() {Id =7,GroupName = "收货人信息", Value = "CountryCode", Text = "收货人国家", Select = false },
                new ExportFieldItem() {Id =9,GroupName = "收货人信息", Value = "ShippingCompany", Text = "收件人公司", Select = false },
                new ExportFieldItem() {Id =11,GroupName = "收货人信息", Value = "ShippingCity", Text = "收货城市", Select = false },
                new ExportFieldItem() {Id =12,GroupName = "收货人信息", Value = "ShippingState", Text = "收货人省/州",Select = false},
                new ExportFieldItem() {Id =10,GroupName = "收货人信息", Value = "ShippingAddress", Text = "收件地址", Select = false },
                new ExportFieldItem() {Id =13,GroupName = "收货人信息", Value = "ShippingZip", Text = "邮编", Select = false },
                new ExportFieldItem() {Id =14,GroupName = "收货人信息", Value = "ShippingPhone", Text = "电话", Select = false },
                new ExportFieldItem() {Id =17,GroupName = "收货人信息", Value = "ShippingTaxId", Text = "收货人税号", Select = false },
                new ExportFieldItem() {Id =18,GroupName = "发货人信息", Value = "SenderFirstLastName", Text = "发件人姓名", Select = false },
                new ExportFieldItem() {Id =19,GroupName = "发货人信息", Value = "SenderCompany", Text = "发件人公司", Select = false },
                new ExportFieldItem() {Id =22,GroupName = "发货人信息", Value = "SenderState", Text = "省/州", Select = false },
                new ExportFieldItem() {Id =21,GroupName = "发货人信息", Value = "SenderCity", Text = "城市", Select = false },
                new ExportFieldItem() {Id =20,GroupName = "发货人信息", Value = "SenderAddress", Text = "收件地址", Select = false },
                new ExportFieldItem() {Id =23,GroupName = "发货人信息", Value = "SenderZip", Text = "邮编", Select = false },
                new ExportFieldItem() {Id =24,GroupName = "发货人信息", Value = "SenderPhone", Text = "电话", Select = false },
                new ExportFieldItem() {Id =34,GroupName = "申报信息", Value = "ApplicationName", Text = "申报名称", Select = false },
                new ExportFieldItem() {Id =35,GroupName = "申报信息", Value = "PickingName", Text = "申报中文名称", Select = false },
                new ExportFieldItem() {Id =36,GroupName = "申报信息", Value = "HSCode", Text = "海关编码", Select = false },
                new ExportFieldItem() {Id =37,GroupName = "申报信息", Value = "Qty", Text = "数量", Select = false },
                new ExportFieldItem() {Id =38,GroupName = "申报信息", Value = "UnitPrice", Text = "单价", Select = false },
                new ExportFieldItem() {Id =39,GroupName = "申报信息", Value = "UnitWeight", Text = "净重量KG", Select = false },
                new ExportFieldItem() {Id =40,GroupName = "申报信息", Value = "Remark", Text = "备注", Select = false }
            };
            return fieldItems;

        }

    }


}