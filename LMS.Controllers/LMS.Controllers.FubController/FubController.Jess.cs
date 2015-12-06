using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Controllers.FubController.Models;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Filters;

namespace LMS.Controllers.FubController
{
    public partial class FubController
    {
        public ActionResult ExchangeBag()
        {
            return View();
        }
        public JsonResult GetMailPostBagInfoExt(string postBagNumber)
        {
            var model = _fubService.GetMailPostBagInfoExt(postBagNumber);
            var result = "0";//无对应客户袋牌
            if (model != null)
            {
                result = !string.IsNullOrWhiteSpace(model.FuPostBagNumber) ? "1" : "2";
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMailPostBagInfoByFu(string fuPostBagNumber)
        {
            var model = _fubService.GetMailPostBagInfoByFu(fuPostBagNumber);
            var result = "1";//无对应邮政袋牌
            if (model != null)
            {
                result = "0";//邮政袋牌已扫描
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SacnMailPostBagInfo(string postBagNumber, string fuPostBagNumber)
        {
            var result = "0";
            if (!string.IsNullOrWhiteSpace(postBagNumber) && !string.IsNullOrWhiteSpace(fuPostBagNumber))
            {

                var model = _fubService.GetMailPostBagInfoExt(postBagNumber);
                if (model == null)
                {
                    result = "无效客户袋牌";
                }
                else
                {
                    if (!model.FuPostBagNumber.IsNullOrWhiteSpace())
                    {
                        result = "客户袋牌已扫描";
                    }
                    else
                    {
                        if (_fubService.GetMailPostBagInfoByFu(fuPostBagNumber) != null)
                        {
                            result = "邮政袋牌已扫描";
                        }
                        else
                        {
                            if (_fubService.SacnMailPostBagInfo(postBagNumber.Trim(), fuPostBagNumber.Trim()))
                            {
                                result = "1";
                            }
                        }
                    }
                    
                }
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LogFlightNumberList(LogFlightNumberListFilter param)
        {
            if (param.StartTime == null)
            {
                param.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + "00:00");
            }
            if (param.EndTime == null)
            {
                param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
            }
            return View(TotalPackageListDataBind(param));
        }
        [HttpPost]
        [ActionName("LogFlightNumberList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchLogFlightNumberList(LogFlightNumberListViewModel filter)
        {
            filter.PagedList.PageIndex = 1;
            return View(TotalPackageListDataBind(filter.FilterModel));
        }
        public ActionResult EditTime(string totalPackageNumber)
        {
            var model = _fubService.GetMailTotalPackageInfoExt(totalPackageNumber).ToModel<MailTotalPackageInfoModel>();
            return View(model);
        }
        public LogFlightNumberListViewModel TotalPackageListDataBind(LogFlightNumberListFilter param)
        {
            var model = new LogFlightNumberListViewModel()
                {
                    FilterModel = param,
                    CountryList = GetCountryList(""),
                    PagedList = _fubService.GetailTotalPackageList(new LogFlightNumberListParam()
                        {
                            MailTotalPackageNumber = param.MailTotalPackageNumber,
                            EndTime = param.EndTime,
                            StartTime = param.StartTime,
                            Page = param.Page,
                            PageSize = param.PageSize,
                        }).ToModelAsPageCollection<MailTotalPackageInfoExt, MailTotalPackageInfoModel>()
                };
            return model;
        }
        public JsonResult PostEditTime(MailTotalPackageInfoModel model)
        {
            var result = new ResponseResult {Result = true};
            if (model.MailTotalPackageNumber.IsNullOrWhiteSpace())
            {
                result.Message = "无效ID";
                result.Result = false;
            }
            else
            {
                var isnullcount = 0;
                if (model.FZFlightNo.IsNullOrWhiteSpace())
                {
                    isnullcount++;
                }
                if (!model.FuZhouDepartureTime.HasValue)
                {
                    isnullcount++;
                }
                if (!model.TaiWanArrivedTime.HasValue)
                {
                    isnullcount++;
                }
                if (isnullcount > 0)
                {
                    result.Message = "保存失败！";
                    result.Result = false;
                }
                else
                {
                    if (model.TaiWanArrivedTime.Value <= model.FuZhouDepartureTime.Value)
                    {
                        result.Message = "福州 - 台湾 到达时间必须大于起航时间！";
                        result.Result = false;
                    }
                    else
                    {
                        isnullcount = 0;
                        if (model.TWFlightNo.IsNullOrWhiteSpace())
                        {
                            isnullcount++;
                        }
                        if (!model.TaiWanDepartureTime.HasValue)
                        {
                            isnullcount++;
                        }
                        if (!model.ToArrivedTime.HasValue)
                        {
                            isnullcount++;
                        }
                        if (isnullcount > 0 && isnullcount < 3)
                        {
                            result.Message = "台湾 - 目的地 航班号，起航时间，到达时间必须全部填写！";
                            result.Result = false;
                        }
                        else
                        {
                            if (isnullcount == 0 && model.ToArrivedTime.Value <= model.TaiWanDepartureTime.Value)
                            {
                                result.Message = "台湾 - 目的地 到达时间必须大于起航时间！";
                                result.Result = false;
                            }else if (isnullcount == 0 &&
                                      model.TaiWanDepartureTime.Value <= model.TaiWanArrivedTime.Value)
                            {
                                result.Message = "到台湾时间必须小于从台湾起航时间！";
                                result.Result = false;
                            }
                            else
                            {
                                if (!_fubService.LogFlightNumber(new MailTotalPackageInfoExt()
                                {
                                    FZFlightType = model.FZFlightType,
                                    FZFlightNo = model.FZFlightNo,
                                    FuZhouDepartureTime = model.FuZhouDepartureTime,
                                    TaiWanArrivedTime = model.TaiWanArrivedTime,
                                    TWFlightNo = model.TWFlightNo??"",
                                    TaiWanDepartureTime = model.TaiWanDepartureTime,
                                    ToArrivedTime = model.ToArrivedTime,
                                    MailTotalPackageNumber = model.MailTotalPackageNumber
                                }))
                                {
                                    result.Message = "保存失败！";
                                    result.Result = false;
                                }
                            }
                        }
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PackageExchangeBag()
        {
            return View();
        }

        public JsonResult CheckTrackNumber(string trackNumber)
        {
            var result = "1";
            if (!trackNumber.IsNullOrWhiteSpace())
            {
                result = _fubService.CheckTrackNumber(trackNumber);
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckBagNumber(string bagNumber, string trackNumber)
        {
            string result;
            if (bagNumber.IsNullOrWhiteSpace())
            {
                result = "2";
            }
            else
            {
                result = trackNumber.IsNullOrWhiteSpace() ? "0" : _fubService.CheckBagNumber(bagNumber, trackNumber);
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SacnPackageExchangeBag(string bagNumber, string trackNumber)
        {
            string result;
            if (bagNumber.IsNullOrWhiteSpace())
            {
                result = "2";
            }
            else
            {
                result = trackNumber.IsNullOrWhiteSpace() ? "0" : _fubService.SacnPackageExchangeBag(bagNumber, trackNumber).ToString();
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
