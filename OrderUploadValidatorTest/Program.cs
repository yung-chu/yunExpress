
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using System.Diagnostics;
using LMS.UserCenter.Controllers.OrderController.Models;
using LMS.UserCenter.Controllers.OrderController.Validators;

namespace OrderUploadValidatorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidatorOptions.CascadeMode = CascadeMode.Continue;

            OrderModel model = new OrderModel();
            model.CustomerOrderNumber = "1234567890sabdkjasbhdjkahbs;ldha;oshdo ashdouhasodhaoshdop ahsodp hasohdoa hs dpoahsdpohasophdoashdoa hsodhaosdhaoshdoashdoahsdohasodh oashdoashd";
            OrderUploadStandardValidator validator = new OrderUploadStandardValidator();
            var results = validator.Validate(model);

            if (results.IsValid == false)
                results.Errors.ToList().ForEach(t => Console.WriteLine(t.ErrorMessage));
            else
                Console.WriteLine("验证通过!");

            Console.ReadKey();
        }
    }
}
