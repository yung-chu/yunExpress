using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Controllers.SettlementController
{
    public partial class SettlementController
    {
        public ActionResult CheckStand()
        {
            return View();
        }
        public JsonResult GetSettlementList(string customerCode)
        {
            var result = new JsonResponseResult<SettlementJson>();
            try
            {
                var model = new SettlementJson();
                int totalNumber = 0;
                decimal totalFee = 0;
                decimal totalSettleWeight = 0;
                decimal totalWeight = 0;
                _settlementService.GetSettlementByCustomerCode(customerCode, Settlement.StatusEnum.Outstanding).ForEach(
                    p =>
                    {
                        model.Data.Add(new SettlementJsonModel
                            {
                                CreatedBy = p.CreatedBy,
                                CreatedOn = p.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                                CustomerCode = p.CustomerCode,
                                SettlementNumber = p.SettlementNumber,
                                TotalNumber = p.TotalNumber,
                                TotalFee = p.TotalFee.ToString("F2"),
                                TotalSettleWeight = p.TotalSettleWeight.ToString("F3"),
                                TotalWeight = p.TotalWeight.ToString("F3")
                            });
                        totalNumber = totalNumber + p.TotalNumber;
                        totalFee = totalFee + p.TotalFee;
                        totalSettleWeight = totalSettleWeight + p.TotalSettleWeight;
                        totalWeight = totalWeight + p.TotalWeight;
                    });

                model.TotalFee = totalFee.ToString("F2");
                model.TotalNumber = totalNumber;
                model.TotalSettleWeight = totalSettleWeight.ToString("F3");
                model.TotalWeight = totalWeight.ToString("F3");
                result.Result = "1";
                result.ResultDesc = "";
                result.Item = model;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = "0";
                result.ResultDesc = "";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserList(string keyword)
        {
            var result = new JsonResponseResult<List<CustomerSmallExt>>();
            try
            {
                var model = _settlementService.GetOutstandingPaymentCustomer(keyword);
                result.Result = "1";
                result.ResultDesc = "";
                result.Item = model;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = "0";
                result.ResultDesc = "";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RechargeWayList()
        {
            var result = new List<RechargeTypeModel>();
            var list = _billingService.GetRechargeTypeList(1);
            list.ForEach(p => result.Add(new RechargeTypeModel()
                {
                    RechargeType = p.RechargeType1,
                    RechargeTypeName = p.RechargeTypeName
                }));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserBalance(string customerCode)
        {
            var result = _billingService.GetCustomerBalance(customerCode);
            return Json(new { CustomerCode = result.CustomerCode, Balance = result.Balance }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckTransactionNo(string transactionNo)
        {
            var result = _billingService.CheckTransactionNo(transactionNo);
            return Json(new {Result = result ? "1" : "0"}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFile(FormCollection form)
        {
            var result = new JsonResponseResult<UpLoadFile>();
            var customerCode = form["CustomerCode"];
            try
            {
                if (!string.IsNullOrWhiteSpace(customerCode))
                {
                    string filePath = sysConfig.VoucherPath + customerCode + @"\";
                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                    HttpFileCollectionBase files = HttpContext.Request.Files;
                    string tempName = string.Empty;
                    for (int iFile = 0; iFile < files.Count; iFile++)
                    {
                        HttpPostedFileBase postedFile = files[iFile];
                        tempName = Path.GetFileName(postedFile.FileName);
                        if (string.IsNullOrWhiteSpace(tempName))
                            throw new Exception("请选择需要上传的文件");
                        string fileExtension = Path.GetExtension(tempName);
                        //if (fileExtension != ".xls")
                        //    throw new Exception("只能上传xls类型的文件");
                        if (!string.IsNullOrEmpty(tempName))
                        {
                            tempName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;
                            postedFile.SaveAs(filePath + tempName);
                        }
                    }
                    result.Result = "1";
                    result.ResultDesc = "";
                    result.Item=new UpLoadFile(){Address = tempName};
                }
                else
                {
                    result.Result = "0";
                    result.ResultDesc = "没有选择客户";
                }
            }
            catch (Exception ex)
            {
                result.Result = "0";
                result.ResultDesc = ex.Message;
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult SaveCheckStand(FormCollection form)
        {
            var param = new CustomerCreditInfo()
            {
                CustomerCode = form["CustomerCode"],
                Amount = Convert.ToDecimal(form["Amount"]),
                RechargeType = Convert.ToInt32(form["RechargeType"]),
                TransactionNo = form["TransactionNo"],
                Remark = form["Remark"],
                VoucherPath = form["flUploadAddress"],
                CreatedOn = DateTime.Now,
                CreatedBy = _workContext.User.UserUame,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = _workContext.User.UserUame,
                Status = 2
            };
            var settlementList = form["SettlementList"];
            var balance = Convert.ToDecimal(form["Balance"]);
            var result = "0";
            try
            {
                using (var tran = new TransactionScope())
                {
                    _billingService.CreateRechargeRecord(param);
                    var amountrecord = new CustomerAmountRecordParam()
                    {
                        Amount = param.Amount.Value,
                        CustomerCode = param.CustomerCode,
                        MoneyChangeTypeId = 1,
                        Remark = param.Remark,
                        FeeTypeId = 1,
                        TransactionNo = param.TransactionNo
                    };
                    if (amountrecord.Amount == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(settlementList) && balance  >= 0)
                        {
                            _settlementService.CheckOkSettlement(settlementList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());
                            result = "1";
                        }
                    }
                    else
                    {
                        if (_customerService.CreateCustomerAmountRecord(amountrecord) == 1)
                        {
                            if (!string.IsNullOrWhiteSpace(settlementList) && balance + param.Amount >= 0)
                            {
                                _settlementService.CheckOkSettlement(
                                    settlementList.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                                  .ToList());
                            }
                            result = "1";
                        }
                        else
                        {
                            throw new Exception("现结收款充值失败！");
                        }
                    }
                    
                    tran.Complete();
                }
                
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return Json(new { Result = result }, JsonRequestBehavior.DenyGet);
        }
    }
}
