using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;

namespace LMS.Controllers.OrderController
{
    public class OrderModel
    {
        public OrderModel ()
        {
            SenderInfoModel=new SenderInfoModel();
            ShippingInfoModel=new ShippingInfoModel();
        }

        public string CustomerOrderID { get; set; }
        public string TrackingNumber { get; set; }
        public int ShippingMethodID { get; set; }
        public string ShippingMethodCode { get; set; }
        public string ShippingMethodName { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public int? PackageNumber { get; set; }
        public int? AppLicationType { get; set; }
        public decimal? InsureAmount { get; set; }
        public bool IsReturn { get; set; }
        public int? SafetyType { get; set; }
        public int? SensitiveTypeID { get; set; }
        public string Message { get; set; }
        public int Number { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public string InsuredID { get; set; }
        public string InsureAmountNumber { get; set; }
        public List<ProductDetailModel> ProductDetailModels { get; set; }

        public SenderInfoModel SenderInfoModel { get; set; }
        public ShippingInfoModel ShippingInfoModel { get; set; }

    }
    public class InsuredModel
    {
        public int InsuredID { get; set; }
        public string InsuredName { get; set; }
    }
    public class SensitiveTypeInfoModel
    {
        public int SensitiveTypeID { get; set; }
        public string SensitiveTypeName { get; set; }
    }

    //收件人信息
    public class ShippingInfoModel
    {
        public string CountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTaxId { get; set; }
    }
    //发件人信息
    public class SenderInfoModel
    {
        public string CountryCode { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
    }

    public class ShippingInfoNlpostModelValidator : AbstractValidator<ShippingInfoModel>
    {
        public ShippingInfoNlpostModelValidator()
        {
            RuleFor(s => s.ShippingFirstName)
                .NotEmpty().When(s => string.IsNullOrWhiteSpace(s.ShippingLastName)).WithMessage("{PropertyName}]不能为空")
                .WithName("收件人姓名");
            RuleFor(s => s.ShippingFirstName + " " + s.ShippingLastName)
                .Length(1, 60).When(s => !string.IsNullOrWhiteSpace(s.ShippingFirstName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓名");
            RuleFor(s => s.ShippingCompany)
                .Length(1, 30).When(s => !string.IsNullOrWhiteSpace(s.ShippingCompany)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人公司");
            RuleFor(s => s.ShippingState)
                .Length(1, 30).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人省/州");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 30).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人城市");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人地址");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 2).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人国家代码");
            RuleFor(s => s.ShippingZip)
                .Length(1, 15).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingZip))
                .WithName("收件人邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").WithMessage("[{PropertyName}]只能是数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("收件人电话");


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

    public class ProductDetailModelValidator : AbstractValidator<ProductDetailModel>
    {
        public ProductDetailModelValidator()
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

            RuleFor(s => s.Quantity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s> 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s.HasValue && s.Value > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
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
}