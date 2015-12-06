using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web.Controllers
{
    public class ValidationCodeController : BaseController
    {
        private const string _contentType = @"image/JPEG";

        public FileResult GetLoginValidationImageCode()
        {
            ValidationImageCodeGenerator.ValidationImageCode validationImageCode = ValidationImageCodeGenerator.Generate();
            Personal["LoginImageCode"] = validationImageCode.Code;
            return File(validationImageCode.FileContent, _contentType);
        }

        public FileResult GetRegisterValidationImageCode()
        {
            ValidationImageCodeGenerator.ValidationImageCode validationImageCode = ValidationImageCodeGenerator.Generate();
            Personal["RegisterImageCode"] = validationImageCode.Code;
            return File(validationImageCode.FileContent, _contentType);
        }
    }
}
