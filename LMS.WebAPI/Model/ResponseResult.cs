using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    [Serializable]
    public class ResponseResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}