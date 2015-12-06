using LMS.Controllers.FubController.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LMS.Controllers.FubController
{
    public partial class FubController
    {
        public ActionResult BagTagPrint(string outStorageId)
        {
            if (string.IsNullOrWhiteSpace(outStorageId)) return View();

            BagTagPrintModel model = GetBagTagPrint(outStorageId);

            //ui test data
            //model.BagTagNumber = "A-US-201412300001";
            //model.CountryName = "美国";
            //model.HasBattery = false;
            //model.Qty = 55;            
            //model.TotalWeight = 24.5M;

            return View(model);
        }

        private BagTagPrintModel GetBagTagPrint(string outStorageId)
        {
            var e = _fubService.GetBagTagPrint(outStorageId);
            if (e == null) return null;

            BagTagPrintModel model = new BagTagPrintModel();
            model.BagTagNumber = e.BagTagNumber;
            model.CountryName = e.CountryName;
            model.HasBattery = e.HasBattery;
            model.Qty = e.Qty;
            model.TotalWeight = e.TotalWeight;

            return model;
        }

        public ActionResult ExchangeBagMainPost()
        {
            return View();
        }

        public JsonResult IsValidFuPostBagNumber(string fuPostNumber)
        {
            JsonResultModel re = new JsonResultModel();

            var result = _fubService.IsValidFuPostBagNumber(fuPostNumber);
            if (result.Status)
            {
                re.Status = 5;
            }
            else
            {
                re.Status = 1;
                re.Message = result.Message;
            }

            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MainPostNumberSave(string fuPostNumber, string mainPostNumber)
        {
            JsonResultModel re = new JsonResultModel();

            if (string.IsNullOrWhiteSpace(fuPostNumber) || string.IsNullOrWhiteSpace(mainPostNumber))
            {
                re.Status = 1;
                re.Message = "[福邮袋牌]或者[总包号]都不能为空.";
            }
            else
            {
                var resultInfo = _fubService.MainPostNumberSave(fuPostNumber, mainPostNumber);
                if (resultInfo.Status)
                {
                    re.Status = 5;
                }
                else
                {
                    re.Status = 1;
                    re.Message = resultInfo.Message;
                }
            }
            return Json(re, JsonRequestBehavior.AllowGet);
        }
    }

    public class JsonResultModel
    {
        /// <summary>
        /// 1 faile, 5 success ,
        /// </summary>
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class JsonResultModel<T> : JsonResultModel
    {
        public List<T> Data { get; set; }
    }
}
