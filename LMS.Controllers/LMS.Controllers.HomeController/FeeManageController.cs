using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Models;
using LMS.Services.FeeManageServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.Controllers
{
    public class FeeManageController : BaseController
    {
        private IFeeManageService _feeManageService;
        public FeeManageController(IFeeManageService feeManageService)
        {
            _feeManageService = feeManageService;
        }
        public ActionResult List()
        {
            return View();
        }
        public string GetFeeList(FormCollection from)
        {
            var param = JsonHelper.JsonToEntity(from["params"], new FeeManageParamModel()) as FeeManageParamModel;
            var list = _feeManageService.GetFeeTypeList(param.feeTypeName, param.Status).ToModelAsCollection<FeeType, FeeTypeModel>();
            return JsonHelper.CreateJsonParameters(list, true, list.Count);
        }
        public ActionResult Add()
        {
            return View();
        }
        public JsonResult CreateFeeType(FeeTypeModel model)
        {
            var result = new ResponseResult();
            if (model != null)
            {
                try
                {
                    _feeManageService.CreateFeeType(model.ToEntity<FeeType>());
                    result.Result = true;
                    result.Message = "Success";
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.Message = ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int FeeTypeID)
        {
            var model = _feeManageService.GetFeeType(FeeTypeID).ToModel<FeeTypeModel>();
            return View(model);
        }

        public JsonResult EditFeeType(FeeTypeModel model)
        {
            string result = "0";
            if (model != null)
            {
                try
                {
                    _feeManageService.UpdateFeeType(model.ToEntity<FeeType>());
                    result = "1";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
