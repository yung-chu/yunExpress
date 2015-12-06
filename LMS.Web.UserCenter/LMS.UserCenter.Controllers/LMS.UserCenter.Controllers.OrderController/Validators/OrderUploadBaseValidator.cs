using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FluentValidation;
using LMS.UserCenter.Controllers.OrderController.Models;

namespace LMS.UserCenter.Controllers.OrderController.Validators
{
    /// <summary>
    /// 订单上传验证(第二步)之基础验证(公共字段可空、大小限制)
    /// </summary>
    public class OrderUploadBaseValidator : AbstractValidator<OrderModel>
    {
        public OrderUploadBaseValidator()
        {
            RuleFor(s => s.CustomerOrderNumber).Cascade(CascadeMode.StopOnFirstFailure)
                 .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                 .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                 //.Must(IsExistsOrderID).When(o => !o.IsReturn).WithMessage("[{PropertyName}-{PropertyValue}]已存在")
                 .WithName("客户订单号");            
           
          
            //RuleFor(s => s.TrackingNumber)
            //    .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
            //   //.When(p => !string.IsNullOrWhiteSpace(p.TrackingNumber))
            //    .WithName("跟踪号");
            /* 
            RuleFor(s => s.ShippingMethodCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                //.Must(ShippingMethodIsEnable).WithMessage("[{PropertyName}]不存在或不可用")
                .WithName("运输方式代号"); ;
            RuleFor(s => s.ShippingFirstName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓");
            RuleFor(s => s.ShippingLastName)

                //.NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.ShippingLastName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人名");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("城市");
            RuleFor(s => s.ShippingState)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("省/州");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2).WithMessage("[{PropertyName}]长度必须为2个字符")
               // .Must(CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
                .WithName("国家代码");
            //RuleFor(s => s.ShippingZip)
            //    .NotEmpty().WithMessage("[{PropertyName}]不能为空")
            //    .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
            //    .WithName("邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("电话");
            //RuleFor(s => s.InsuredID)
            //    //.Must(IsExistsInsuredID).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
            //   // .WithName("保险类型");

            ////            RuleFor(s => s.GoodsTypeID)
            ////                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
            ////                .Must(IsPackageType).WithMessage("[{PropertyName}]必须为大于零的整数")
            ////                .WithName("包裹类型");
            //RuleFor(s => s.SensitiveTypeID)
            //    .Must(IsGoodsType).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
            //    .WithName("货品类型");
           */

        }
    }
}