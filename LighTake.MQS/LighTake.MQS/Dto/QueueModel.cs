using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LighTake.MQS.Dto
{
    public class QueueModel
    {
        public int Status { get; set; }

        public string ErrorMessage { get; set; }

        public object Value { get; set; }

    }
}