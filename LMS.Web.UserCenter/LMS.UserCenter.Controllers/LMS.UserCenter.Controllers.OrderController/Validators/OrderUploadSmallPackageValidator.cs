using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace LMS.UserCenter.Controllers.OrderController.Validators
{
    /// <summary>
    /// 小包业务验证逻辑
    /// </summary>
    public class OrderUploadSmallPackageValidator : OrderUploadBaseValidator
    {
        public OrderUploadSmallPackageValidator()
            : base()
        {
            RuleFor(s => s.CustomerOrderNumber)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(15, 30).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("客户订单号");
        }
    }
}