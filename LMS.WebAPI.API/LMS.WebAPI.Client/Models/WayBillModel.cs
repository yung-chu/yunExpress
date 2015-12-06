using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FluentValidation;
using FluentValidation.Attributes;
using LMS.Data.Entity;
using LMS.Services.FreightServices;
using LMS.WebAPI.Client.Helper;
using LighTake.Infrastructure.Common.InversionOfControl;
using Newtonsoft.Json;

namespace LMS.WebAPI.Client.Models
{
    [Validator(typeof (WayBillModelValidator))]
    public class WayBillModel
    {
        public WayBillModel()
        {
            ErrorMessage = new StringBuilder();
            InsuranceType = 0;
            IsValid = true;
            Length = 1;
            Width = 1;
            Height = 1;
            Weight = 0;
            ApplicationType = 4;
            IsReturn = false;
            InsuranceType = 0;
        }

        [JsonIgnore]
        [XmlIgnore]
        public int? InShippingMethodId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string InShippingMethodName { get; set; }

        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingMethodCode { get; set; }

        //是否关税预付
        public bool? EnableTariffPrepay { get; set; }
        //public int Status { get; set; }
        /// <summary>
        /// 包裹称重重量
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 总费用
        /// </summary>
        public decimal? TotalFee { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 包裹结算重量
        /// </summary>
        public decimal? SettleWeight { get; set; }

        /// <summary>
        /// 包裹体积
        /// </summary>
        public decimal PackageVolume
        {
            get { return (Length ?? 1)*(Width ?? 1)*(Height ?? 1); }
        }

        public int PackageNumber { get; set; }


        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }



        [XmlElement("ShippingInfo")]
        public ShippingInfoModel ShippingInfo { get; set; }

        [XmlElement("SenderInfo", IsNullable=true)]
        public SenderInfoModel SenderInfo { get; set; }

        public bool IsReturn { get; set; }

        /// <summary>
        ///  申报类型
        /// </summary>
        public int ApplicationType { get; set; }

        public int InsuranceType { get; set; }
        public decimal? InsureAmount { get; set; }
        public int? SensitiveTypeID { get; set; }

        private List<ApplicationInfoModel> _applicationInfos;

        [XmlArrayItem("ApplicationInfo")]
        public List<ApplicationInfoModel> ApplicationInfos
        {
            get { return _applicationInfos ?? (_applicationInfos = new List<ApplicationInfoModel>()); }
            set { _applicationInfos = value; }
        }

        [JsonIgnore]
        [XmlIgnore]
        public bool IsValid { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public StringBuilder ErrorMessage { get; set; }
    }

    /// <summary>
    /// 收件人信息
    /// </summary>
    [Validator(typeof (ShippingInfoModelValidator))]
    public class ShippingInfoModel
    {
        /// <summary>
        /// 收件人税号
        /// </summary>

        public string ShippingTaxId { get; set; }

        /// <summary>
        /// 收件人国家简码
        /// </summary>
        public string CountryCode { get; set; }

        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
    }

    /// <summary>
    /// 发件人信息
    /// </summary>
    [Validator(typeof (SenderInfoModelValidator))]
    public class SenderInfoModel
    {
        public SenderInfoModel()
        {
            CountryCode = "CN";
        }

        /// <summary>
        /// 发件人国家简码
        /// </summary>
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

    public class OrderInfo
    {
        public string OrderNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string WayBillNumber { get; set; }
    }
    
    public class TrackNumberInfo
    {
        public string TrackingNumber { get; set; }
    }

    [Validator(typeof (ApplicationInfoModelValidator))]
    public class ApplicationInfoModel
    {
        public string ApplicationName { get; set; }
        public string HSCode { get; set; }
        public int? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitWeight { get; set; }

        /// <summary>
        /// 配货名称
        /// </summary>
        public string PickingName { get; set; }

        public string Remark { get; set; }

        public string ProductUrl { get; set; }

    }


    public class OrderResponseResult
    {
        public string CustomerOrderId { get; set; }

        public int Status { get; set; }

        public string OrderId { get; set; }

        public string TrackStatus { get; set; }

        public string Feedback { get; set; }

        public string AgentNumber { get; set; }
    }


    public class WayBillModelValidator : AbstractValidator<WayBillModel>
    {
        public WayBillModelValidator()
        {
            RuleFor(s => s.OrderNumber).Cascade(CascadeMode.StopOnFirstFailure)
                                       .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                                       .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                                       .WithName("客户订单号");
            RuleFor(s => s.ShippingMethodCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(CommonMethodHelper.ShippingMethodIsEnable).WithMessage("[{PropertyName}]不存在或不可用")
                .WithName("运输方式代号");


            RuleFor(s => s.ApplicationType)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .WithName("申报类型");
            // RuleFor(s => s.InsuranceType).NotEmpty().WithMessage("[{PropertyName}]不能为空")
            
        }

    }

    public class ShippingInfoModelValidator : AbstractValidator<ShippingInfoModel>
    {
        public ShippingInfoModelValidator()
        {

            RuleFor(s => s.ShippingFirstName)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.ShippingFirstName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓");
            RuleFor(s => s.ShippingLastName)
                .Length(1, 50)
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingLastName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人名");
            RuleFor(s => s.ShippingCompany)
                .Length(1, 200).When(s => !string.IsNullOrWhiteSpace(s.ShippingCompany)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人公司");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人城市");
            RuleFor(s => s.ShippingState)
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人省/州");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 5).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(CommonMethodHelper.CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
                .WithName("收件人国家代码");
            RuleFor(s => s.ShippingZip)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("收件人电话");
        }

    }

    //中美专线验证
    public class ShippingInfoSinoUSModelValidator : AbstractValidator<ShippingInfoModel>
    {
        public ShippingInfoSinoUSModelValidator()
        {

            RuleFor(s => s.ShippingFirstName + s.ShippingLastName)
                .Length(1, 35).When(s => !string.IsNullOrWhiteSpace(s.ShippingFirstName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓名");
            RuleFor(s => s.ShippingCompany)
                .Length(1, 35).When(s => !string.IsNullOrWhiteSpace(s.ShippingCompany)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人公司");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 35).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址1");
            RuleFor(s => s.ShippingAddress1)
                .Length(0, 35).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址2");
            RuleFor(s => s.ShippingAddress2)
                .Length(0, 35).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址3");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 35).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人城市");
            RuleFor(s => s.ShippingState)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 2).WithMessage("[{PropertyName}]长度必须为[2]")
                .WithName("收件人省/州");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 2).WithMessage("[{PropertyName}]长度必须为[2]")
                .Must(CommonMethodHelper.CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
                .WithName("收件人国家代码");
            RuleFor(s => s.ShippingZip)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(5, 9).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("收件人电话");
        }
    }

    public class ShippingInfoNlpostModelValidator : AbstractValidator<ShippingInfoModel>
    {
        public ShippingInfoNlpostModelValidator()
        {
            RuleFor(s => s.ShippingFirstName + s.ShippingLastName)
                .NotEmpty().WithMessage("{PropertyName}]不能为空")
                .WithName("收件人姓名");
            RuleFor(s => s.ShippingFirstName +" "+ s.ShippingLastName)
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
                .NotEmpty().When(s=>string.IsNullOrWhiteSpace(s.ShippingAddress1)&&string.IsNullOrWhiteSpace(s.ShippingAddress2)).WithMessage("[{PropertyName}]不能为空")
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
            RuleFor(s=>s.ShippingAddress+" "+ s.ShippingAddress1+" "+s.ShippingAddress2)
                .Length(1, 200).When(s => !string.IsNullOrWhiteSpace(s.ShippingAddress)|| !string.IsNullOrWhiteSpace(s.ShippingAddress1)|| !string.IsNullOrWhiteSpace(s.ShippingAddress2))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 2).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(CommonMethodHelper.CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
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

    public class ShippingInfoSfModelValidator:AbstractValidator<ShippingInfoModel>
    {
        public ShippingInfoSfModelValidator()
        {
            RuleFor(s => s.ShippingFirstName + s.ShippingLastName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓名");
            RuleFor(s => s.ShippingCompany)
                //.NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s=>!string.IsNullOrWhiteSpace(s.ShippingCompany))
                .WithName("收件人公司");
            RuleFor(s => s.ShippingPhone)
                .Matches(@"^[0-9]*$").WithMessage("[{PropertyName}]只能是数字")
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("收件人手机");
            RuleFor(s => s.ShippingState)
                .Length(1, 30).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人省/州");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人城市");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2, 2).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人国家代码");
            RuleFor(s => s.ShippingZip)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 25).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("收件人邮编");
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

    public class SenderInfoModelValidator : AbstractValidator<SenderInfoModel>
    {
        public SenderInfoModelValidator()
        {

            RuleFor(s => s.SenderFirstName)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.SenderFirstName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人姓");
            RuleFor(s => s.SenderLastName)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.SenderLastName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人名");
            RuleFor(s => s.SenderCompany)
                .Length(1, 200).When(s => !string.IsNullOrWhiteSpace(s.SenderCompany)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人公司");
            RuleFor(s => s.SenderAddress)
                .Length(1, 200).When(s => !string.IsNullOrWhiteSpace(s.SenderAddress)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人地址");
            RuleFor(s => s.SenderCity)
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.SenderCity)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人城市");
            RuleFor(s => s.SenderState)
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.SenderState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人省/州");
            RuleFor(s => s.CountryCode)
                .Length(2, 5).When(s => !string.IsNullOrWhiteSpace(s.CountryCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人国家代码");
            RuleFor(s => s.SenderZip)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.SenderZip)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人邮编");
            RuleFor(s => s.SenderPhone)
                .Length(1, 20).When(s => !string.IsNullOrWhiteSpace(s.SenderPhone)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("发件人电话");

        }
    }


    public class ApplicationInfoModelValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.PickingName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.Remark)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");



        }
    }

    public class ApplicationInfoFuZhouModelValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoFuZhouModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 60).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报名称");

            RuleFor(s => s.PickingName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 60).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报中文名称");

            RuleFor(s => s.HSCode)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.PickingName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.Remark)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");



        }
    }

    public class ApplicationInfoModelEudValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoModelEudValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.PickingName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 500).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                //.Must(CheckRemarks).WithMessage("[{PropertyName}]格式不正确")
                .WithName("备注");

            RuleFor(s => s.ProductUrl)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 500).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("销售链接");
        }


        //private bool CheckRemarks(ApplicationInfoModel model, string arg)
        //{   
        //    return CheckRemark(model.Remark);
        //}

        //private bool CheckRemark(string remark)
        //{
        //    //SKUCode1  上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于30
        //    Regex z = new Regex(@"^[A-Za-z0-9]{0,30}$");
        //     MatchCollection mc = z.Matches(remark);
        //     if (mc.Count == 1 && mc[0].Value == remark)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

    }

    public class ApplicationInfoModelNlpostValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoModelNlpostValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .Length(1, 10).When(s=>!string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]只能是数字")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s.HasValue&&s.Value > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s.HasValue&&s.Value > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s.HasValue&&s.Value > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
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

    public class ApplicationInfoModelSfModelValidator:AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoModelSfModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("申报名称");
            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");

            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");
        }
    }

    public class ApplicationInfoEUBModelValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoEUBModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 128).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报名称");

            RuleFor(s => s.PickingName)
                .Must(CheckPickingNames).WithMessage("[{PropertyName}]必须含有2个以上的中文字符")
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 64).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报中文名称");

            RuleFor(s => s.HSCode)
                .Length(1, 50).When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.Remark)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.Remark)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");
        }

        private bool CheckPickingNames(ApplicationInfoModel model, string arg)
        {
            return CheckPickingName(model.PickingName);
        }

        private bool CheckPickingName(string pickingName)
        {
            //pickingName  上传系统时申报中文名必须含有两个以上的中文字符
            return Regex.IsMatch(pickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+");
        }
    }

    public class ApplicationInfoDHLModelValidator : AbstractValidator<ApplicationInfoModel>
    {
        public ApplicationInfoDHLModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(CheckApplicationName).WithMessage("[{PropertyName}]不能包含特殊字符和汉字")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .Length(1, 50)
                .When(s => !string.IsNullOrWhiteSpace(s.HSCode))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => (s ?? 1) > 0).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500)
                .When(s => !string.IsNullOrWhiteSpace(s.PickingName))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .Length(1, 500)
                .When(s => !string.IsNullOrWhiteSpace(s.Remark))
                .WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");
        }

        private bool CheckApplicationName(ApplicationInfoModel model, string arg)
        {
            return ApplicationName(model.ApplicationName);
        }

        private bool ApplicationName(string applicationName)
        {
            //pickingName  上传系统时申报英文名不能包含特殊字符和汉字
            return !Regex.IsMatch(applicationName, @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+");
        }

    }
    public class ResultResponse
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
    }
}