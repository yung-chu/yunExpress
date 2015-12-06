using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FluentValidation;
using LMS.UserCenter.Controllers.OrderController.Models;

namespace LMS.UserCenter.Controllers.OrderController.Validators
{
    /// <summary>
    /// 订单上传简单验证(上传第一步验证)
    /// </summary>
    public class OrderUploadSimpleValidator : AbstractValidator<OrderModel>
    {
        public OrderUploadSimpleValidator()
        {

        }

    }
}