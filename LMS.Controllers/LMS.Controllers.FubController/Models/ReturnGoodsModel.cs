using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;

namespace LMS.Controllers.FubController.Models
{
    public class ReturnGoodsViewModel
    {
        public ReturnGoodsViewModel()
        {
            ReasonTypeList = MailReturnGoodsLogs.GetReasonTypeList().Select(p => new SelectListItem()
                {
                    Value = p.ValueField,
                    Text = p.TextField,
                }).ToList();
        }

        public List<SelectListItem> ReasonTypeList { get; set; }
    }
}