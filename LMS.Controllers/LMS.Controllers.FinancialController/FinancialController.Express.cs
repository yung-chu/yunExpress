using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Controllers.FinancialController
{
    public partial class FinancialController
    {
        public ActionResult ExpressDeliveryFeeList()
        {
            var model = ExpressDeliveryFeeListInit();
            return View(model);
        }
        public ActionResult SearchExpressAjax(ExpressDeliveryFeeListFilterModel filter)
        {
            var model = ExpressDeliveryFeeListInit();

            model.Filter = filter;

            var list = GetShippingMethods(filter.VenderCode, filter.ShippingType);
            if (list != null && list.Any())
            {
                string _shippingmethodid = filter.SearchContext.IsNullOrWhiteSpace()
                                               ? filter.ShippingMethodId
                                               : string.Empty;
                model.PagedList = _financialService.ExpressDeliveryFeeSearch(new DeliveryReviewParam()
                    {
                        EndTime = filter.EndTime,
                        Page = filter.Page,
                        PageSize = filter.PageSize,
                        SearchContext = filter.SearchContext,
                        SearchWhere = filter.SearchWhere,
                        ShippingMethodId = _shippingmethodid,
                        StartTime = filter.StartTime,
                        Status = filter.Status,
                        VenderCode = filter.VenderCode,
                        ShippingMethodIds = _shippingmethodid.IsNullOrWhiteSpace()? list.Select(p => p.ShippingMethodId).ToArray():new int[1]{ Int32.Parse(_shippingmethodid)}
                    }).ToModelAsPageCollection<DeliveryFeeExt, ExpressDeliveryFeeModel>();
            }
            else
            {
                model.PagedList = new PagedList<ExpressDeliveryFeeModel>();
            }


            return View("ExpressDeliveryFeePartialList", model);
        }

        public ActionResult ExpressExportExcel(ExpressDeliveryFeeListFilterModel filter)
        {
            HttpContext.Server.ScriptTimeout = 100 * 60; //10 minutes
            Log.Info("开始导出快递发货费用");
            string fileName = "ExpressDeliveryFeeList-" + filter.VenderCode + "-" + DateTime.Now.ToString("yyyy-dd-MM-hh-mm-ss") + "1";
            var list = new List<ExpressDeliveryFeeModel>();
            Log.Info("开始获取快递的运输方式");
            var shippingMethods = GetShippingMethods(filter.VenderCode, filter.ShippingType);
            Log.Info("完成获取快递的运输方式");
            if (shippingMethods != null && shippingMethods.Any())
            {
                Log.Info("开始查询快递发货费用列表");
                _financialService.ExportExcel(new DeliveryReviewParam()
                    {
                        EndTime = filter.EndTime,
                        Page = filter.Page,
                        PageSize = filter.PageSize,
                        SearchContext = filter.SearchContext,
                        SearchWhere = filter.SearchWhere,
                        ShippingMethodId = filter.ShippingMethodId,
                        StartTime = filter.StartTime,
                        Status = filter.Status,
                        VenderCode = filter.VenderCode,
                        IsExportExcel = true,
                        ShippingMethodIds = shippingMethods.Select(p => p.ShippingMethodId).ToArray(),
                    }, false, true).ForEach(d => list.Add(new ExpressDeliveryFeeModel()
                        {
                            AprroveWeight = d.AprroveWeight,
                            Auditor = d.Auditor,
                            AuditorDate = d.AuditorDate,
                            CountryChineseName = d.CountryChineseName,
                            CountryCode = d.CountryCode,
                            CustomerName = d.CustomerName,
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
                            OverWeightLengthGirthFee = d.OverWeightLengthGirthFee,
                            SecurityAppendFee = d.SecurityAppendFee,
                            AddedTaxFee = d.AddedTaxFee,
                            OtherFee = d.OtherFee,
                            OtherFeeRemark = d.OtherFeeRemark,
                            TotalFee = d.TotalFee,
                            TotalFeeFinal = d.TotalFeeFinal,
                            Trackingnumber = d.Trackingnumber,
                            VenderCode = d.VenderCode,
                            VenderId = d.VenderId,
                            VenderName = d.VenderName,
                            WayBillNumber = d.WayBillNumber,
                            Weight = d.Weight,
                        }));
                Log.Info("完成查询快递发货费用列表");
            }
            Log.Info("开始生成快递execl");
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
                        "OverWeightLengthGirthFee-超长超重超周长费",
                        "SecurityAppendFee-安全附加费",
                        "AddedTaxFee-增值税费",
                        "OtherFee-杂费",
                        "OtherFeeRemark-杂费备注",
                        "TotalFee-系统总费用",  
                        "TotalFeeFinal-最终总费用",  
                        "StatusStr-状态",
                        "Auditor-审核人",
                        "AuditorDate-审核时间"
                    };
            ExportExcelByWeb.ListExcel(fileName, list, titleList);
            Log.Info("完成生成快递execl");
            Log.Info("完成导出快递发货费用");
            return new EmptyResult();
        }

        public ActionResult ExpressImportWaitAduit(ExpressDeliveryFeeListFilterModel filter)
        {
            var model = ExpressDeliveryFeeListInit();

            model.Filter = filter;

            model.PagedList = _financialService.ExpressImportWait2Audit(new DeliveryReviewParam()
                {
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    UserName = _workContext.User.UserUame,
                    Status = filter.Status
                }).ToModelAsPageCollection<DeliveryFeeExpressExt, ExpressDeliveryFeeModel>();

            return View("ExpressDeliveryImportWaitAduitPartList", model);
        }

        public ActionResult ExpressDeliveryExeclUpload(string VenderCode)
        {
            var model = new ExpressFileUploadJsonModel { VenderCode = VenderCode, SelectOrderNoList = GetWhereTypes(), Status = 10 };
            return View(model);
        }
        [HttpPost]
        public ActionResult ExpressDeliveryExeclUploadPost(ExpressFileUploadJsonModel filter)
        {
            Log.Info("服务器开始运行快递导入账单方法");
            HttpContext.Server.ScriptTimeout = 200 * 60;
            var model = new ExpressFileUploadJsonModel
                {
                    VenderCode = filter.VenderCode,
                    SelectOrderNo = filter.SelectOrderNo,
                    SelectOrderNoList = GetWhereTypes()
                };

            #region valid the excel
            HttpFileCollectionBase files = Request.Files;
            if (files == null || files.Count == 0)
            {
                model.Status = 0;
                model.Info = "上传文件不能为空!";
                return View("ExpressDeliveryExeclUpload", model);
            }
            HttpPostedFileBase excelfile = files[0];
            string fileType = excelfile.FileName.Substring(excelfile.FileName.LastIndexOf(".") + 1);
            if (!_Support_Excel_File_Types.Contains(fileType))
            {
                model.Status = 0;
                model.Info = "上传文件格式不对,目前只支持.xls,.xlsx格式!";
                return View("ExpressDeliveryExeclUpload", model);
            }
            #endregion
            var VenderUploadDeliveryFeeList = new List<ExpressDeliveryImportDataModel>();
            var passList = new List<ExpressDeliveryImportAccountCheck>();

            #region Get data from Excel
            try
            {
                Log.Info("开始保存快递上传的Execl文件");
                #region save tempfile
                string setting = System.Configuration.ConfigurationManager.AppSettings["Upload_Path"];
                string tempFilePath = Path.Combine(setting, string.Format("ExpressDeliveryFeeImport_{0}{1}.{2}", _workContext.User.UserUame, DateTime.Now.Ticks.ToString(), fileType));
                excelfile.SaveAs(tempFilePath);
                #endregion
                Log.Info("完成保存快递上传的Execl文件");
                //DataTable dataTable = ExcelHelper.ReadToDataTable(excelfile.InputStream);
                Log.Info("开始读起execl到datatable");
                DataTable dataTable = ExcelHelper.ReadToDataTable(tempFilePath);
                Log.Info("完成读起execl到datatable");
                string venderName = string.Empty;
                var dorderNumber = new Dictionary<string, string>();
                Log.Info("开始验证execl数据");
                foreach (var row in dataTable.AsEnumerable())
                {
                    var data = new ExpressDeliveryImportDataModel();
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
                    data.ShippingMethodName = row.Field<string>("运输方式");
                    data.ShippingMethodName = string.IsNullOrEmpty(data.ShippingMethodName) ? string.Empty : data.ShippingMethodName.Trim();
                    data.CountryName = row.Field<string>("国家");
                    data.IncidentalRemark = row.Field<string>("杂费备注");
                    var receivingDate = row.Field<string>("收货日期");
                    #region 收货日期
                    var dtTemp = DateTime.Now;
                    if (!string.IsNullOrEmpty(receivingDate))
                    {
                        if (DateTime.TryParse(receivingDate, out dtTemp))
                        {
                            data.ReceivingDate = dtTemp;
                        }
                        else
                        {
                            data.ErrorReason += " 日期格式不对;请采用excel标准日期上传! ";
                        }
                    }
                    #endregion

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
                    if (!string.IsNullOrEmpty(row.Field<string>("运费")))
                    {
                        try
                        {
                            data.Freight = Convert.ToDecimal(row.Field<string>("运费"));
                        }
                        catch
                        {
                            data.ErrorReason += "运费,不为数字,上传数据为：" + row.Field<string>("运费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("挂号费")))
                    {
                        try
                        {
                            data.Register = Convert.ToDecimal(row.Field<string>("挂号费"));
                        }
                        catch
                        {
                            data.ErrorReason += "挂号费,不为数字,上传数据为：" + row.Field<string>("挂号费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("燃油费")))
                    {
                        try
                        {
                            data.FuelCharge = Convert.ToDecimal(row.Field<string>("燃油费"));
                        }
                        catch
                        {
                            data.ErrorReason += "燃油费,不为数字,上传数据为：" + row.Field<string>("燃油费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("附加费")))
                    {
                        try
                        {
                            data.Surcharge = Convert.ToDecimal(row.Field<string>("附加费"));
                        }
                        catch
                        {
                            data.ErrorReason += "附加费,不为数字,上传数据为：" + row.Field<string>("附加费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("关税预付服务费")))
                    {
                        try
                        {
                            data.TariffPrepayFee = Convert.ToDecimal(row.Field<string>("关税预付服务费"));
                        }
                        catch
                        {
                            data.ErrorReason += "关税预付服务费,不为数字,上传数据为：" + row.Field<string>("关税预付服务费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("超长超重超周围长费（每件总和）")))
                    {
                        try
                        {
                            data.OverWeightLengthGirthFee = Convert.ToDecimal(row.Field<string>("超长超重超周围长费（每件总和）"));
                        }
                        catch
                        {
                            data.ErrorReason += "超长超重超周围长费（每件总和）,不为数字,上传数据为：" + row.Field<string>("超长超重超周围长费（每件总和）");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("安全附加费")))
                    {
                        try
                        {
                            data.SecurityAppendFee = Convert.ToDecimal(row.Field<string>("安全附加费"));
                        }
                        catch
                        {
                            data.ErrorReason += "安全附加费,不为数字,上传数据为：" + row.Field<string>("安全附加费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("增值税率费")))
                    {
                        try
                        {
                            data.AddedTaxFee = Convert.ToDecimal(row.Field<string>("增值税率费"));
                        }
                        catch
                        {
                            data.ErrorReason += "增值税率费,不为数字,上传数据为：" + row.Field<string>("增值税率费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("杂费")))
                    {
                        try
                        {
                            data.Incidentals = Convert.ToDecimal(row.Field<string>("杂费"));
                        }
                        catch
                        {
                            data.ErrorReason += "杂费,不为数字,上传数据为：" + row.Field<string>("杂费");
                        }
                    }
                    if (!string.IsNullOrEmpty(row.Field<string>("总费用")))
                    {
                        try
                        {
                            data.TotalFee = Convert.ToDecimal(row.Field<string>("总费用"));
                        }
                        catch
                        {
                            data.ErrorReason += "总费用,不为数字,上传数据为：" + data.TotalFee;
                        }
                    }
                    else
                    {
                        data.ErrorReason += " [总费用]不能为空; ";
                    }
                    #endregion


                    VenderUploadDeliveryFeeList.Add(data);
                    //验证通过
                    if (data.ErrorReason.IsNullOrWhiteSpace())
                    {
                        passList.Add(new ExpressDeliveryImportAccountCheck()
                            {
                                AddedTaxFee = data.AddedTaxFee,
                                CountryName = data.CountryName,
                                CreatedBy = _workContext.User.UserUame,
                                CreatedOn = DateTime.Now,
                                Freight = data.Freight,
                                FuelCharge = data.FuelCharge,
                                Incidentals = data.Incidentals,
                                IncidentalRemark = data.IncidentalRemark,
                                OrderNumber = data.OrderNumber,
                                OverWeightLengthGirthFee = data.OverWeightLengthGirthFee,
                                TariffPrepayFee = data.TariffPrepayFee,
                                ReceivingDate = data.ReceivingDate,
                                Register = data.Register,
                                SecurityAppendFee = data.SecurityAppendFee,
                                SettleWeight = data.SettleWeight,
                                ShippingMethodName = data.ShippingMethodName,
                                Status = 0,
                                Surcharge = data.Surcharge,
                                TotalFee = data.TotalFee,
                                UserName = _workContext.User.UserUame,
                                VenderName = data.VenderName,
                                WayBillNumber = data.WayBillNumber,
                                Weight = data.Weight
                            });
                    }
                }

                dorderNumber.Clear();
                dataTable.Clear();
                Log.Info("完成验证execl数据");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                model.Status = 0;
                model.Info = "读取Excel数据失败";
            }
            #endregion
            Log.Info("开始保存execl数据到数据库");
            model.UploadFailCount = 0;
            if (VenderUploadDeliveryFeeList.Any(t => !string.IsNullOrEmpty(t.ErrorReason)))
            {
                model.ErrorData.AddRange(VenderUploadDeliveryFeeList.Where(t => !string.IsNullOrEmpty(t.ErrorReason)).ToList());
                model.UploadFailCount = VenderUploadDeliveryFeeList.Count(t => !string.IsNullOrEmpty(t.ErrorReason));
            }
            try
            {
                //写入数据
                if (_financialService.SaveDeliveryImportAccountChecks(passList, filter.VenderCode, filter.SelectOrderNo) && passList.Any(t => t.IsTrue == false))
                {
                    passList.Where(t => t.IsTrue == false).ToList().ForEach(p =>
                        {
                            var errormodel = VenderUploadDeliveryFeeList.FirstOrDefault(e => e.OrderNumber == p.OrderNumber);
                            errormodel.ErrorReason = p.ErrorReason;
                            model.ErrorData.Add(errormodel);
                        });
                    model.UploadFailCount += passList.Count(t => t.IsTrue == false);
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
            Log.Info("完成保存execl数据到数据库");
            Log.Info("服务器完成运行快递导入账单方法");
            return View("ExpressDeliveryExeclUpload", model);
        }

        public JsonResult ExpressDeliveryCostDetailsAuditPass(List<AuditParam> dataParams)
        {
            JsonModel model = new JsonModel();

            if (dataParams == null)
            {
                model.Status = 0;
                model.Info = "请选择运单!";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var currentUser = this._workContext.User;
                dataParams.ForEach(p =>
                    {
                        p.ErrorRemark = string.Format("{0}({1})在{2},进行审核通过操作。<hr/>", currentUser.RealName,
                                                      currentUser.UserUame, DateTime.Now.ToString());
                    });
                if (_financialService.ExpressDeliveryFeeAuditPass(dataParams, currentUser.UserUame))
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

        public JsonResult ExpressDeliveryFeeAuditError(List<AuditParam> dataParams)
        {
            JsonModel model = new JsonModel();
            if (dataParams == null)
            {
                model.Status = 0;
                model.Info = "无效请求";
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var currentUser = this._workContext.User;
                dataParams.ForEach(p =>
                   {
                       p.ErrorRemark = string.Format("{0}({1})在{2}审核异常操作,备注:“{3}”。<hr/>",
                                                     currentUser.RealName,
                                                     currentUser.UserUame,
                                                     DateTime.Now.ToString(),
                                                     p.ErrorRemark
                           );
                   });
                if (_financialService.ExpressDeliveryFeeAuditError(dataParams, currentUser.UserUame))
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

        //初始化查询条件
        private ExpressDeliveryFeeListModel ExpressDeliveryFeeListInit()
        {
            var model = new ExpressDeliveryFeeListModel
            {
                SearchWhereTypes = GetWhereTypes(),
                Filter =
                    new ExpressDeliveryFeeListFilterModel
                    {
                        StartTime = DateTime.Now.AddMonths(-3),
                        EndTime = DateTime.Now
                    },
                StatusList = Financial.GetDeliveryFeeStatusList().ConvertAll(p => new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField
                })
            };
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "" });
            return model;
        }
    }


}
