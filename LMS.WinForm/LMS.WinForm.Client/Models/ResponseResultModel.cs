using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    [Serializable]
    public class ResponseResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
