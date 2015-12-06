using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;

namespace LMS.Services.SF.Model
{
    public class OrderSfModelValidator : AbstractValidator<OrderSfModel>
    {
        public OrderSfModelValidator()
        {
            RuleFor(s => s.ShippingName)
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
            RuleFor(s => s.ShippingTel)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Matches(@"^[0-9]*$").WithMessage("[{PropertyName}]只能是数字")
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人电话");
            RuleFor(s => s.ShippingPhone)
                //.NotEmpty().WithMessage("[{PropertyName}]不能为空")
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
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
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
    public class ApplicationSfModelValidator : AbstractValidator<ApplicationSfModel>
    {
        public ApplicationSfModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("申报名称");
            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s => s> 0).WithMessage("[{PropertyName}]必须为大于零的整数")
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
}
