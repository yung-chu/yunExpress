using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    public class CustomerException : Exception
    {
        public CustomerException(int status,string message,string code)
        {
            ResultStatus = status;
            ResultMessage = message;
            Code = code;
        }
        public override string Message
        {
            get
            {
                return ResultMessage;
            }
        }

        public int ResultStatus { get; set; }
        public string ResultMessage { get; set; }
        public string Code { get; set; }
    
    }
}
