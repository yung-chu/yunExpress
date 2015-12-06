using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Data;
using AutoMapper;
using LMS.Data.Entity;
using LMS.Data.Entity.Param;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Filters;
using LMS.Controllers.FinancialController.ViewModels;

namespace LMS.Controllers.FinancialController
{
    public partial class FinancialController
    {
        public ActionResult DeliveryCostDetailsReview(DeliveryCostDetailsModel requestModel)
        {
            HttpContext.Server.ScriptTimeout = 10 * 60;

            DeliveryCostDetailsModel model = new DeliveryCostDetailsModel();
            model.SearchWhereTypes = GetWhereTypes();
            model.Filter = new DeliveryCostDetailsFilterModel();
            model.Filter.StartTime = DateTime.Now.AddMonths(-3);
            model.Filter.EndTime = DateTime.Now;
            //if (requestModel != null)
            //{
            //    model.Filter = requestModel.Filter;

            //    //get data
            //    if (!string.IsNullOrEmpty(model.Filter.VenderCode))
            //    {
            //        model.PagedList = GetSearchData(model.Filter);
            //    }
            //}
            return View(model);
        }

        public ActionResult GetSearchDataAjax(string VenderCode, string ShippingMethodId, DateTime? StartTime, DateTime? EndTime, int? Status, int? SearchWhere, string SearchContext, int page, int ShippingType, int pageSize = 20)
        {
            DeliveryCostDetailsModel model = new DeliveryCostDetailsModel();
            model.SearchWhereTypes = GetWhereTypes();

            model.Filter = GetFilter(VenderCode, ShippingMethodId, StartTime, EndTime, Status, SearchWhere, SearchContext, page, ShippingType, pageSize);

            //get data
            //if (!string.IsNullOrEmpty(model.Filter.VenderCode))
            //{
            model.PagedList = GetSearchData(model.Filter);
            //}
            //else
            //{
            //    model.PagedList = new PagedList<DeliveryFeeModel>();
            //}

            return View("DeliveryPartialView", model);
        }

        private PagedList<DeliveryFeeModel> GetSearchData(DeliveryCostDetailsFilterModel filter, int searchType = 1)
        {
            DeliveryReviewParam para = Mapper.Map<DeliveryCostDetailsFilterModel, DeliveryReviewParam>(filter);
            PagedList<DeliveryFeeExt> result;
            if (searchType == 1)
            {
                var list = GetShippingMethods(para.VenderCode, filter.ShippingType);
                if (list != null && list.Any())
                {
                    if (!para.ShippingMethodIds.Any())
                    {
                        para.ShippingMethodIds = list.Select(p => p.ShippingMethodId).ToArray();
                    }
                    result = _financialService.DeliveryFeeSearch(para);
                }
                else
                {
                    result = new PagedList<DeliveryFeeExt>();
                }
            }
            else
            {
                result = _financialService.ImportExcelWait2Audit(para);
            }
            return Mapper.Map<PagedList<DeliveryFeeExt>, PagedList<DeliveryFeeModel>>(result);
        }

        private DeliveryCostDetailsFilterModel GetFilter(string VenderCode, string ShippingMethodId, DateTime? StartTime, DateTime? EndTime, int? Status, int? SearchWhere, string SearchContext, int page, int ShippingType, int pageSize = 20)
        {
            var filter = new DeliveryCostDetailsFilterModel();
            filter.Page = page > 0 ? page : 1;
            filter.PageSize = pageSize;
            filter.VenderCode = VenderCode;
            if (SearchContext.IsNullOrWhiteSpace())
            {
                filter.ShippingMethodId = ShippingMethodId;
            }
            filter.StartTime = StartTime;
            //filterModel.EndTime.Value.Date.AddDays(1).AddMinutes(-1);
            filter.EndTime = EndTime.HasValue ? EndTime.Value.Date.AddDays(1).AddMinutes(-1) : EndTime;
            filter.Status = Status == 0 ? null : Status; //0 全部
            filter.SearchWhere = SearchWhere;
            filter.SearchContext = SearchContext;
            filter.ShippingType = ShippingType;
            return filter;
        }

        public void ExportExcel(string VenderCode, string ShippingMethodId, DateTime? StartTime, DateTime? EndTime, int? Status, int? SearchWhere, string SearchContext, int ShippingType, int page = 1, int pageSize = 20)
        {
            HttpContext.Server.ScriptTimeout = 100 * 60; //10 minutes
            Log.Info("开始导出小包发货费用");
            var filter = GetFilter(VenderCode, ShippingMethodId, StartTime, EndTime, Status, SearchWhere, SearchContext, page, ShippingType, pageSize);
            var titleList = new List<string>
                    {                        
                        "WayBillNumber-运单号",
                        "CustomerOrderNumber-客户订单号",
                        "CustomerName-客户名称",
                        "Trackingnumber-跟踪号",
                        "VenderName-服务商",
                        "OutStorageCreatedOn-发货时间",
                        "CountryCode-发货国家",
                        "ShippingmethodName-运输方式",
                        "SetWeight-计费重量",
                        "Weight-称重重量",     
                        "AprroveWeight-最终重量",   
                        "Freight-运费",
                        "Register-挂号费",
                        "FuelCharge-燃油费",
                        "Surcharge-附加费",
                        "TariffPrepayFee-关税服务费用",
                        "TotalFee-系统总费用",  
                        "TotalFeeFinal-最终总费用",  
                        "StatusStr-状态",
                        "Auditor-审核人",
                        "AuditorDate-审核时间"
                    };
            ExportFile result = new ExportFile();
            try
            {
                string fileName = "DeliveryCostDetailsReview-" + filter.VenderCode + "-" + DateTime.Now.ToString("yyyy-dd-MM-hh-mm-ss") + "1";
                filter.IsExportExcel = true;//设置获取全部
                Log.Info("开始转化小包查询参数");
                DeliveryReviewParam para = Mapper.Map<DeliveryCostDetailsFilterModel, DeliveryReviewParam>(filter);
                Log.Info("完成转化小包查询参数");
                var exports = new List<DeliveryFeeExportExcel>();
                Log.Info("开始获取小包的运输方式");
                var list = GetShippingMethods(para.VenderCode, ShippingType);
                Log.Info("完成获取小包的运输方式");
                if (list != null && list.Any())
                {
                    Log.Info("开始查询小包发货费用列表");
                    para.ShippingMethodIds = list.Select(p => p.ShippingMethodId).ToArray();
                    var data = _financialService.ExportExcel(para);
                    foreach (var d in data)
                    {
                        var t = new DeliveryFeeExportExcel
                            {
                                AprroveWeight = d.AprroveWeight,
                                Auditor = d.Auditor,
                                AuditorDate = d.AuditorDate,
                                CountryChineseName = d.CountryChineseName,
                                CountryCode = d.CountryCode,
                                CustomerName=d.CustomerName,
                                CreatedBy = d.CreatedBy,
                                CreatedOn = d.CreatedOn,
                                CustomerOrderNumber = d.CustomerOrderNumber,
                                DeliveryFeeID = d.DeliveryFeeID,
                                Freight = d.Freight,
                                FuelCharge = d.FuelCharge,
                                LastUpdatedBy = d.LastUpdatedBy,
                                LastUpdatedOn = d.LastUpdatedOn,
                                OutStorageCreatedOn = d.OutStorageCreatedOn,
                                Register = d.Register,
                                Remark = d.Remark,
                                SetWeight = d.SetWeight,
                                ShippingmethodID = d.ShippingmethodID,
                                ShippingmethodName = d.ShippingmethodName,
                                Status = d.Status,
                                StatusStr = ConvertStatusToString(d.Status),
                                Surcharge = d.Surcharge,
                                TariffPrepayFee = d.TariffPrepayFee,
                                TotalFee = d.TotalFee,
                                TotalFeeFinal = d.TotalFeeFinal,
                                Trackingnumber = d.Trackingnumber,
                                VenderCode = d.VenderCode,
                                VenderData = d.VenderData,
                                VenderId = d.VenderId,
                                VenderName = d.VenderName,
                                WayBillNumber = d.WayBillNumber,
                                Weight = d.Weight
                            };
                        exports.Add(t);
                    }
                    Log.Info("完成查询小包发货费用列表");
                }
                Log.Info("开始生成小包execl");
                ExportExcelByWeb.ListExcel(fileName, exports, titleList);
                Log.Info("完成生成小包execl");
                result.IsSuccess = true;
                result.FilePaths = new string[] { fileName };
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
        }


        private string ConvertStatusToString(int status)
        {
            string result = "";
            switch (status)
            {
                case 1:
                    result = "未审核";
                    break;
                case 2:
                    result = "异常";
                    break;
                case 3:
                    result = "已审核";
                    break;
                default:
                    break;
            }
            return result;
        }

        public JsonResult GetRemarkHistory(int id)
        {
            SingleDataJsonModel model = new SingleDataJsonModel();

            if (id <= 0)
            {
                model.Status = 0;
                model.Info = "无效请求";
            }
            else
            {
                string remark = "";
                try
                {
                    remark = _financialService.GetRemarkHistory(id);
                    model.Status = 1;
                    model.Data = remark;
                }
                catch (Exception ex)
                {
                    model.Status = 0;
                    model.Info = "服务器忙，请稍后再试!";
                    Log.Exception(ex);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryCostDetailsAuditPass(string ids)
        {
            JsonModel model = new JsonModel();

            if (string.IsNullOrEmpty(ids))
            {
                model.Status = 0;
                model.Info = "请选择运单!";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            try
            {
                List<int> idsArray = ids.Split(',').Select(t => int.Parse(t)).ToList();
                var currentUser = this._workContext.User;
                string remark = string.Format("{0}({1})在{2},进行审核通过操作。<hr/>", currentUser.RealName, currentUser.UserUame, DateTime.Now.ToString());
                if (_financialService.DeliveryFeeAuditPass(idsArray, currentUser.UserUame, remark, DateTime.Now))
                {
                    model.Status = 1;
                }
            }
            catch (BusinessLogicException ex)
            {
                model.Status = 0;
                model.Info = ex.Message;
                Log.Exception(ex);
            }
            catch (Exception ex)
            {
                model.Status = 0;
                model.Info = "系统忙,请稍后再试!";
                Log.Exception(ex);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryCostDetailsReverseAudit(string ids)
        {
            ReverseAuditJsonModel model = new ReverseAuditJsonModel();

            if (string.IsNullOrEmpty(ids))
            {
                model.Status = 0;
                model.Info = "请选择运单!";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            try
            {
                List<int> idsArray = ids.Split(',').Select(t => int.Parse(t)).ToList();
                var currentUser = this._workContext.User;
                string remark = string.Format("{0}({1})在{2},进行反审核操作。<hr/>", currentUser.RealName, currentUser.UserUame, DateTime.Now.ToString());
                //idsArray = ids.Split(',').Select(t => Convert.ToInt32(t)).ToList();
                _financialService.ReverseAudit(idsArray, currentUser.UserUame, remark, DateTime.Now);

                model.Status = 1;
            }
            catch (Exception ex)
            {
                model.Status = 0;
                model.Info = "系统忙,请稍后再试!";
                Log.Exception(ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryFeeAuditError(string ids, string error)
        {
            JsonModel model = new JsonModel();
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(error))
            {
                model.Status = 0;
                model.Info = "无效请求";
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var currentUser = this._workContext.User;
                string remark = string.Format("{0}({1})在{2}审核异常操作,备注:“{3}”。<hr/>",
                    currentUser.RealName,
                    currentUser.UserUame,
                    DateTime.Now.ToString(),
                    error
                    );

                List<int> idsArray = ids.Split(',').Select(t => int.Parse(t)).ToList();

                if (_financialService.DeliveryFeeAuditError(idsArray, currentUser.UserUame, remark, DateTime.Now))
                {
                    model.Status = 1;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                model.Info = "系统忙,请稍后再试!";
                Log.Exception(ex);
            }

            model.Status = 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public static List<SelectListItem> GetWhereTypes()
        {
            var list = new List<SelectListItem>
			{
				new SelectListItem{
					Value = ((int)WayBill.SearchFilterEnum.WayBillNumber).ToString(),
					Text = "运单号"					
				},
				new SelectListItem{
					Value =  ((int)WayBill.SearchFilterEnum.TrackingNumber).ToString(),
					Text = "跟踪号"					
				},
                new SelectListItem{
					Value = ((int)WayBill.SearchFilterEnum.CustomerOrderNumber).ToString(),
					Text = "订单号"					
				}
			};
            return list;

        }

        [HttpGet]
        public ActionResult DeliveryFeeVectorExcelUpload(string VenderCode)
        {
            var model = new FileUploadJsonModel { VenderCode = VenderCode, SelectOrderNoList = GetWhereTypes(), Status = 10 };
            return View(model);
        }

        static string[] _Support_Excel_File_Types = new string[] { "xls","xlsx" };

        [HttpPost]
        public ActionResult DeliveryFeeVectorExcelUploadPost(FileUploadJsonModel filter)
        {
            Log.Info("服务器开始运行小包导入账单方法");
            HttpContext.Server.ScriptTimeout = 200 * 60;

            var model = new FileUploadJsonModel
                {
                    VenderCode = filter.VenderCode,
                    SelectOrderNo = filter.SelectOrderNo,
                    SelectOrderNoList = GetWhereTypes()
                };
            if (filter.VenderCode.IsNullOrWhiteSpace())
            {
                model.Status = 0;
                model.Info = "服务商为空!";
                return View("DeliveryFeeVectorExcelUpload", model);
            }
            #region valid the excel
            HttpFileCollectionBase files = Request.Files;
            if (files == null || files.Count == 0)
            {
                model.Status = 0;
                model.Info = "上传文件不能为空!";
                return View("DeliveryFeeVectorExcelUpload", model);
            }
            HttpPostedFileBase excelfile = files[0];
            string fileType = excelfile.FileName.Substring(excelfile.FileName.LastIndexOf(".") + 1);
            if (!_Support_Excel_File_Types.Contains(fileType))
            {
                model.Status = 0;
                model.Info = "上传文件格式不对,目前只支持.xls,.xlsx格式!";
                return View("DeliveryFeeVectorExcelUpload", model);
            }
            #endregion

            try
            {
                Log.Info("开始保存小包上传的Execl文件");
                #region save tempfile
                string setting =  System.Configuration.ConfigurationManager.AppSettings["Upload_Path"];
                string tempFilePath = Path.Combine(setting, string.Format("DeliveryFeeImport_{0}{1}.{2}", _workContext.User.UserUame, DateTime.Now.Ticks.ToString(), fileType));
                excelfile.SaveAs(tempFilePath);
                #endregion
                Log.Info("完成保存小包上传的Execl文件");
                var VenderUploadDeliveryFeeList = new List<DeliveryImportDataModel>();

                #region Get data from Excel
                try
                {
                    //DataTable dataTable = ExcelHelper.ReadToDataTable(excelfile.InputStream);
                    Log.Info("开始读起execl到datatable");
                    DataTable dataTable = ExcelHelper.ReadToDataTable(tempFilePath);
                    Log.Info("完成读起execl到datatable");
                    string venderName = string.Empty;
                    var dorderNumber = new Dictionary<string, string>();
                    Log.Info("开始验证execl数据");
                    foreach (var row in dataTable.AsEnumerable())
                    {
                        var data = new DeliveryImportDataModel();
                        if (!string.IsNullOrWhiteSpace(row.Field<string>("订单号/跟踪号")))
                        {
                            if (dorderNumber.ContainsKey(row.Field<string>("订单号/跟踪号").Trim()))
                            {
                                data.ErrorReason += "订单号/跟踪号,已经存在 上传数据为：" + row.Field<string>("订单号/跟踪号");
                            }
                            else
                            {
                                dorderNumber.Add(row.Field<string>("订单号/跟踪号").Trim(), row.Field<string>("订单号/跟踪号").Trim());
                            }
                            data.OrderNumber = row.Field<string>("订单号/跟踪号").Trim();
                        }
                        else
                        {
                            data.ErrorReason += " [订单号/跟踪号]不能为空; ";
                        }
                        data.ReceivingDateStr = row.Field<string>("收货日期");
                        if (!string.IsNullOrWhiteSpace(row.Field<string>("服务商")))
                        {
                            if (venderName.IsNullOrEmpty())
                            {
                                venderName = row.Field<string>("服务商").Trim();
                            }
                            else
                            {
                                if (venderName != row.Field<string>("服务商").Trim())
                                {
                                    data.ErrorReason += " 一次上传只允许一个服务商的数据; ";
                                }
                            }
                            data.VenderName = row.Field<string>("服务商").Trim();
                        }
                        //data.VenderName = string.IsNullOrEmpty(data.VenderName) ? string.Empty : data.VenderName.Trim();
                        data.ShippingMethodName = row.Field<string>("运输方式");
                        data.ShippingMethodName = string.IsNullOrEmpty(data.ShippingMethodName) ? string.Empty : data.ShippingMethodName.Trim();
                        data.CountryName = row.Field<string>("国家");

                        #region 数字列
                        if (!string.IsNullOrEmpty(row.Field<string>("货物重量kg")))
                        {
                            try
                            {
                                data.Weight = Convert.ToDecimal(row.Field<string>("货物重量kg"));
                            }
                            catch
                            {
                                data.ErrorReason += "货物重量kg,不为数字,上传数据为：" + row.Field<string>("货物重量kg");
                            }
                        }
                        if (!string.IsNullOrEmpty(row.Field<string>("计费重量kg")))
                        {
                            try
                            {
                                data.SettleWeight = Convert.ToDecimal(row.Field<string>("计费重量kg"));
                            }
                            catch
                            {
                                data.ErrorReason += "计费重量kg,不为数字,上传数据为：" + row.Field<string>("计费重量kg");
                            }
                        }
                        else
                        {
                            data.ErrorReason += " [计费重量kg]不能为空;";
                        }
                        if (!string.IsNullOrEmpty(row.Field<string>("总费用")))
                        {
                            try
                            {
                                data.TotalFee = Convert.ToDecimal(row.Field<string>("总费用"));
                            }
                            catch
                            {
                                data.ErrorReason += "总费用,不为数字,上传数据为：" + row.Field<string>("总费用");
                            }
                        }
                        else
                        {
                            data.ErrorReason += " [总费用]不能为空; ";
                        }
                        #endregion

                        data.CreatedOn = DateTime.Now;
                        data.CreatedBy = _workContext.User.UserUame;
                        data.Status = 0;

                        VenderUploadDeliveryFeeList.Add(data);
                    }

                    //VenderUploadDeliveryFeeList = dataTable.AsEnumerable().Select(row => new DeliveryImportDataModel
                    //{
                    //    OrderNumber = row.Field<string>("订单号/跟踪号"),
                    //    ReceivingDateStr = row.Field<string>("收货日期"),
                    //    VenderName = row.Field<string>("服务商"),
                    //    ShippingMethodName = row.Field<string>("运输方式"),
                    //    CountryName = row.Field<string>("国家"),
                    //    Weight = Convert.ToDecimal(row.Field<string>("货物重量kg")),
                    //    SettleWeight = Convert.ToDecimal(row.Field<string>("计费重量kg")),
                    //    TotalFee = Convert.ToDecimal(row.Field<string>("总费用")),
                    //    CreatedOn = DateTime.Now,
                    //    CreatedBy = _workContext.User.UserUame,
                    //    Status = 0
                    //}).ToList();

                    DateTime dtTemp = DateTime.Now;
                    foreach (var i in VenderUploadDeliveryFeeList)
                    {
                        if (!string.IsNullOrEmpty(i.ReceivingDateStr))
                        {
                            if (DateTime.TryParse(i.ReceivingDateStr, out dtTemp))
                            {
                                i.ReceivingDate = dtTemp;
                            }
                            else
                            {
                                i.ErrorReason += " 日期格式不对;请采用excel标准日期上传! ";
                            }

                        }
                    }
                    Log.Info("完成验证execl数据");
                    dataTable = null;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    throw new BusinessLogicException("读取Excel数据失败");
                }
                #endregion

                #region check 3 columns

                //DeliveryImportDataModel first = VenderUploadDeliveryFeeList.FirstOrDefault();
                //foreach (var t in VenderUploadDeliveryFeeList)
                //{
                //    if (first.VenderName != t.VenderName)
                //    {
                //        t.ErrorReason += " 一次上传只允许一个服务商的数据; ";
                //    }
                //    //if (first.ShippingMethodName != t.ShippingMethodName)
                //    //{
                //    //    t.ErrorReason += " 一次上传只允许一种运输方式的数据; ";
                //    //}
                //    if (!t.TotalFee.HasValue)
                //    {
                //        t.ErrorReason += " [总费用]不能为空; ";
                //    }
                //    if (string.IsNullOrEmpty(t.OrderNumber))
                //    {
                //        t.ErrorReason += " [订单号/跟踪号]不能为空; ";
                //    }
                //    if (!t.SettleWeight.HasValue)
                //    {
                //        t.ErrorReason += " [计费重量kg]不能为空; ";
                //    }
                //}

                //var duplicates = VenderUploadDeliveryFeeList.GroupBy(s => s.OrderNumber).SelectMany(grp => grp.Skip(1));
                //foreach (var dup in duplicates)
                //{
                //    foreach (var v in VenderUploadDeliveryFeeList.Where(t => t.OrderNumber == dup.OrderNumber))
                //    {
                //        v.ErrorReason += string.Format(" 单号[{0}]发现多条重复数据; ", dup.OrderNumber);
                //    }
                //}

                #endregion

                #region Save Data
                Log.Info("开始保存execl数据到数据库");
                int countShouldNotBeEmpty = VenderUploadDeliveryFeeList.Count(t => !string.IsNullOrEmpty(t.ErrorReason));
                var dataList = Mapper.Map<List<DeliveryImportDataModel>, List<DeliveryImportAccountCheck>>(
                    VenderUploadDeliveryFeeList.Where(t => string.IsNullOrEmpty(t.ErrorReason)).ToList());
                foreach (var check in dataList)
                {
                    check.UserName = _workContext.User.UserUame; //当前用户
                }
                model.UploadFailCount = 0;
                if (countShouldNotBeEmpty > 0)
                {
                    model.ErrorData.AddRange(VenderUploadDeliveryFeeList.Where(t => !string.IsNullOrEmpty(t.ErrorReason)).ToList());
                    model.UploadFailCount = countShouldNotBeEmpty;
                }
                //写入数据
                if (_financialService.SaveDeliveryImportAccountChecks(dataList, filter.VenderCode, filter.SelectOrderNo) && dataList.Count(t => t.IsTrue == false) > 0)
                {
                    var output = dataList.Where(t => t.IsTrue == false).ToList();
                    model.ErrorData.AddRange(Mapper.Map<List<DeliveryImportAccountCheck>, List<DeliveryImportDataModel>>(output));

                    //统计
                    model.UploadFailCount += output.Count();
                }
                if (model.UploadFailCount == 0)
                {
                    model.Status = 1;
                    model.UploadSuccessCount = VenderUploadDeliveryFeeList.Count();
                }
                else
                {
                    model.Status = 3;
                    model.UploadSuccessCount = VenderUploadDeliveryFeeList.Count() - model.UploadFailCount;
                }
                Log.Info("完成保存execl数据到数据库");
                #endregion
            }
            catch (BusinessLogicException ex)
            {
                Log.Exception(ex);
                model.Status = 0;
                model.Info = ex.Message;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                model.Status = 0;
                model.Info = "保存数据出错";
            }
            Log.Info("服务器完成运行小包导入账单方法");
            return View("DeliveryFeeVectorExcelUpload", model);
        }

        public ActionResult GetVenderImportWaitAduit(string VenderCode, string ShippingMethodId, DateTime? StartTime, DateTime? EndTime, int? Status, int? SearchWhere, string SearchContext, int ShippingType, int page, int pageSize = 20)
        {
            DeliveryCostDetailsModel model = new DeliveryCostDetailsModel();
            model.SearchWhereTypes = GetWhereTypes();

            model.Filter = GetFilter(VenderCode, ShippingMethodId, StartTime, EndTime, Status, SearchWhere, SearchContext, page, ShippingType, pageSize);
            model.Filter.UserName = _workContext.User.UserUame;

            //get data
            //if (!string.IsNullOrEmpty(model.Filter.VenderCode))
            //{
            model.PagedList = GetSearchData(model.Filter, 2);
            //}
            //else
            //{
            //    model.PagedList = new PagedList<DeliveryFeeModel>();
            //}

            return View("DeliveryFeeWaitAudit", model);
        }

        public JsonResult GetTotalFinalSum(string VenderCode, string ShippingMethodId, DateTime? StartTime, DateTime? EndTime, int? Status, int? SearchWhere, string SearchContext, int page, int ShippingType, int pageSize = 20)
        {
            TotalSumJsonModel model = new TotalSumJsonModel();
            try
            {
                var filter = GetFilter(VenderCode, ShippingMethodId, StartTime, EndTime, Status, SearchWhere, SearchContext, page, ShippingType, pageSize);

                DeliveryReviewParam para = Mapper.Map<DeliveryCostDetailsFilterModel, DeliveryReviewParam>(filter);
                var list = GetShippingMethods(filter.VenderCode, ShippingType);
                if (list != null && list.Any())
                {
                    para.ShippingMethodIds = list.Select(p => p.ShippingMethodId).ToArray();
                    model.Data = _financialService.DeliveryFeeGetTotalFinalSum(para);
                    model.Status = 1;
                }
                else
                {
                    model.Data = 0;
                    model.Status = 1;
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                model.Status = 0;
                model.Info = "服务器忙，请稍后再试!";
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<DeliveryDeviation> GetVenderDeliveryDeviation(string venderName)
        {
            var key = string.Format("DeliveryDeviation_{0}", venderName.GetHashCode());

            if (HttpRuntime.Cache[key] != null)
            {
                return HttpRuntime.Cache[key] as List<DeliveryDeviation>;
            }
            else
            {
                var data = _financialService.GetVenderDeliveryDeviation(venderName);
                HttpRuntime.Cache.Add(key, data, null, DateTime.Now.AddMinutes(5), new TimeSpan(), System.Web.Caching.CacheItemPriority.Normal, null);
                return data;
            }
        }

        private void GetLocalOrderInfo(List<VenderUploadDeliveryFee> venderUploadDeliveryFeeList)
        {
            var ids = venderUploadDeliveryFeeList.Select(t => t.OrderOrTrackNumber).ToList();

            List<WayBillNumberExtSimple> localInfos = _financialService.GetLocalOrderInfo(ids);

            foreach (var fee in venderUploadDeliveryFeeList)
            {
                var temp = localInfos.Where(t => t.WayBillNumber == fee.OrderOrTrackNumber || t.Trackingnumber == fee.OrderOrTrackNumber).FirstOrDefault();
                if (temp != null)
                {
                    fee.WayBillNumber = temp.WayBillNumber;
                    fee.LocalWeight = temp.SetWeight;
                    fee.LocalTotalFee = temp.TotalFee;
                }
            }

            var deliveryDeviations = GetVenderDeliveryDeviation(venderUploadDeliveryFeeList.FirstOrDefault().VenderName);
            //bool isWeigtDeviationPercent = 

            var query = venderUploadDeliveryFeeList.Where(t =>
                //重量
                (t.ChargedWeight - t.LocalWeight) / t.LocalWeight >
                deliveryDeviations.Where(d => d.DeviationType == 2).OrderBy(d => d.VenderId).FirstOrDefault().DeviationRate ||
                (t.ChargedWeight - t.LocalWeight) >
                deliveryDeviations.Where(d => d.DeviationType == 2).OrderBy(d => d.VenderId).FirstOrDefault().DeviationValue ||
                    //费用
                (t.TotalFee - t.LocalTotalFee) / t.LocalTotalFee >
                deliveryDeviations.Where(d => d.DeviationType == 1).OrderBy(d => d.VenderId).FirstOrDefault().DeviationRate ||
                (t.TotalFee - t.LocalTotalFee) >
                deliveryDeviations.Where(d => d.DeviationType == 1).OrderBy(d => d.VenderId).FirstOrDefault().DeviationValue
               );

            //save diffs


        }

        //
        /// <summary>
        /// 获取小包还是快递的运输方式
        /// </summary>
        /// <param name="vendercode"></param>
        /// <param name="shippingType">1-小包，2-快递</param>
        /// <returns></returns>
        private List<ShippingMethod> GetShippingMethods(string vendercode, int shippingType)
        {
            //添加缓存机制 胡志平 过期时间5Minutes
            string cacheKey = string.Format("/SelectVender/ShippingMethodList/{0}_" + shippingType.ToString(),
                                            string.IsNullOrWhiteSpace(vendercode)
                                                ? "0"
                                                : vendercode.Trim().ToLower());
            var list = new List<ShippingMethod>();
            if (Cache.Exists(cacheKey))
            {
                list = Cache.Get<List<ShippingMethod>>(cacheKey);
            }
            else
            {
                //1-小包
                list = _freightService.GetShippingMethodListByVenderCode(vendercode, shippingType, true);
                if (list != null && list.Any())
                {
                    Cache.Add(cacheKey, list, DateTime.Now.AddMinutes(5));
                }
            }
            return list;
        }
    }

    #region 服务商导入Excel实体
    public class VenderUploadDeliveryFee
    {
        public virtual string WayBillNumber { get; set; }

        public virtual string OrderOrTrackNumber { get; set; }
        public virtual System.DateTime? OutStorageCreatedOn { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string ShippingmethodName { get; set; }
        public virtual string CountryCode { get; set; }
        /// <summary>
        /// Weight Kg
        /// </summary>
        public virtual decimal? Weight { get; set; }
        /// <summary>
        /// ChargedWeight Kg
        /// </summary>
        public virtual decimal? ChargedWeight { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>
        public virtual decimal? TotalFee { get; set; }

        public virtual decimal? LocalWeight { get; set; }
        public virtual decimal? LocalTotalFee { get; set; }
    }

    public class DeliveryFeeExportExcel
    {
        public virtual int DeliveryFeeID { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public virtual string Trackingnumber { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string VenderCode { get; set; }
        public virtual Nullable<int> VenderId { get; set; }
        public virtual Nullable<int> ShippingmethodID { get; set; }
        public virtual string ShippingmethodName { get; set; }
        public virtual Nullable<decimal> SetWeight { get; set; }
        public virtual Nullable<decimal> AprroveWeight { get; set; }
        public virtual int Status { get; set; }
        public virtual string StatusStr { get; set; }
        public virtual string Remark { get; set; }
        public virtual string Auditor { get; set; }
        public virtual Nullable<System.DateTime> AuditorDate { get; set; }
        public virtual System.DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual System.DateTime LastUpdatedOn { get; set; }
        public virtual string LastUpdatedBy { get; set; }

        /// <summary>
        /// 发货时间（出仓时间）
        /// </summary>
        public virtual System.DateTime? OutStorageCreatedOn { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryChineseName { get; set; }

        /// <summary>
        /// 称重质量
        /// </summary>
        public virtual Nullable<decimal> Weight { get; set; }
        //运费
        public virtual decimal? Freight { get; set; }
        //挂号费
        public virtual decimal? Register { get; set; }
        //燃油费
        public virtual decimal? FuelCharge { get; set; }
        //关税预付服务费
        public virtual decimal? TariffPrepayFee { get; set; }
        //附加费
        public virtual decimal? Surcharge { get; set; }
        //总费用
        public virtual decimal? TotalFee { get; set; }

        //最终总费用
        public virtual decimal? TotalFeeFinal { get; set; }

        //服务商数据
        public virtual DeliveryImportAccountCheck VenderData { get; set; }

        public virtual string CustomerName { get; set; }
    }
    #endregion

    #region JsonModels
    public class JsonModel
    {
        public int Status { get; set; }
        public string Info { get; set; }
        public string VenderCode { get; set; }
        public int SelectOrderNo { get; set; }
        public List<SelectListItem> SelectOrderNoList { get; set; }
    }
    public class ReverseAuditJsonModel : JsonModel
    {
        //public List<string> SuccessIds { get; set; }
        //public List<string> FailIds { get; set; }
    }
    public class SingleDataJsonModel : JsonModel
    {
        public string Data { get; set; }
    }
    public class FileUploadJsonModel : JsonModel
    {
        public FileUploadJsonModel()
        {
            ErrorData = new List<DeliveryImportDataModel>();
        }
        public int UploadSuccessCount { get; set; }
        public int UploadFailCount { get; set; }
        public List<DeliveryImportDataModel> ErrorData { get; set; }
    }
    public class ExpressFileUploadJsonModel : JsonModel
    {
        public ExpressFileUploadJsonModel()
        {
            ErrorData = new List<ExpressDeliveryImportDataModel>();
        }
        public int UploadSuccessCount { get; set; }
        public int UploadFailCount { get; set; }
        public List<ExpressDeliveryImportDataModel> ErrorData { get; set; }
    }
    public class TotalSumJsonModel : JsonModel
    {
        public decimal Data { get; set; }
    }
    #endregion
}