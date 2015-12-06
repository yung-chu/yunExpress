using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LMS.Client.SubmitSF.Controller;
using LMS.Services.SF;
using LMS.Services.SF.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitSF
{
    public class SubmitSfJob
    {
        private static readonly string _LithuaniaShippingMethodID = ConfigurationManager.AppSettings["LithuaniaShippingMethodID"];
        private static readonly string _authorization = ConfigurationManager.AppSettings["LithuaniaAuthorization"];

        public static void SubmitSfOrder()
        {
            Log.Info("开始预报荷兰小包");
            //预报荷兰小包
            var list = WayBillController.GetNlPostWayBillNumberList();
            if (list.Any())
            {
                const int pagesize = 50;//分批查询
                int pageindex = 1;
                do
                {
                    var waybillList=WayBillController.GetWayBillSfModelList(list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList());
                    if (waybillList.Any())
                    {
                        foreach (var wayBillSfModel in waybillList)
                        {
                            try
                            {
                                if (!wayBillSfModel.ApplicationInfo.Any())
                                {
                                    WayBillController.SubmitFailure(wayBillSfModel.WayBillNumber, "没有申报信息");
                                    continue;
                                }
                                var parcel = SFController.NlPost(wayBillSfModel);
                                if (parcel != null && !parcel.WayBillNumber.IsNullOrWhiteSpace() && !parcel.MailNo.IsNullOrWhiteSpace())
                                {
                                    if (SFController.NlPostConfirm(parcel.WayBillNumber, parcel.MailNo))
                                    {
                                        //记录成功
                                        WayBillController.SubmitSuccess(parcel);
                                    }
                                    else
                                    {
                                        //取消订单
                                        SFController.NlPostConfirm(parcel.WayBillNumber, parcel.MailNo, 2);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("运单号：{0} 提交顺丰API异常信息：{1}".FormatWith(wayBillSfModel.WayBillNumber, ex.Message));
                            }
                            
                        }
                    }
                    pageindex++;
                } while (list.Count > (pageindex - 1) * pagesize);
            }
            Log.Info("完成预报荷兰小包");
            WayBillController.UpdateOutShippingMethod();
        }
        //预报顺E宝
        public static void SubmitSebOrder()
        {
            Log.Info("开始预报顺E宝俄罗斯挂号，平邮");
            var list = SebController.GetLithuaniaWayBillNumberList();
            if (list.Any())
            {
                const int pagesize = 50;//分批查询
                int pageindex = 1;
                do
                {
                    var waybillList = SebController.GetWayBillSfModelList(list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList());
                    if (waybillList.Any())
                    {
                        foreach (var wayBillSfModel in waybillList)
                        {
                            try
                            {
                                var model = new OrderSfModel()
                                    {
                                        OrderId = wayBillSfModel.WayBillNumber,
                                        ExpressType =
                                            _LithuaniaShippingMethodID.Split(new string[] {","},
                                                                             StringSplitOptions.RemoveEmptyEntries)
                                                [0] == wayBillSfModel.ShippingMethodId.ToString()
                                                ? 10
                                                : 9,
                                        ShippingName = wayBillSfModel.ShippingName,
                                        ShippingCompany = wayBillSfModel.ShippingCompany,
                                        ShippingTel = wayBillSfModel.ShippingPhone,
                                        ShippingPhone = wayBillSfModel.ShippingPhone,
                                        ShippingAddress = wayBillSfModel.ShippingAddress,
                                        ParcelQuantity = wayBillSfModel.PackageNumber,
                                        ShippingState = wayBillSfModel.ShippingState,
                                        ShippingCity = wayBillSfModel.ShippingCity,
                                        CountryCode = wayBillSfModel.CountryCode,
                                        ShippingZip = wayBillSfModel.ShippingZip,
                                        ApplicationTotalPrice =
                                            wayBillSfModel.ApplicationInfo.Sum(p => p.Qty*p.UnitPrice),
                                        ApplicationTotalWeight =
                                            wayBillSfModel.ApplicationInfo.Sum(p => p.Qty*p.UnitWeight)
                                    };

                                if (!wayBillSfModel.ApplicationInfo.Any())
                                {
                                    SebController.SubmitFailure(wayBillSfModel.WayBillNumber, "没有申报信息");
                                    continue;
                                }
                                else
                                {
                                    wayBillSfModel.ApplicationInfo.ForEach(p => model.Applications.Add(new ApplicationSfModel()
                                        {
                                            ApplicationName = p.ApplicationName,
                                            Qty = p.Qty,
                                            UnitPrice = p.UnitPrice,
                                            UnitWeight = p.UnitWeight
                                        }));
                                }
                                var parcel = LMSSFCommon.SubmitSf(model,_authorization);
                                if (parcel.ErrorMsg.IsNullOrWhiteSpace() && parcel.Model != null &&
                                    !parcel.Model.OrderId.IsNullOrWhiteSpace() &&
                                    !parcel.Model.MailNo.IsNullOrWhiteSpace())
                                {
                                    //if (LMSSFCommon.SfConfirm(parcel.Model.OrderId, parcel.Model.MailNo, _authorization))
                                    //{
                                        //记录成功
                                        SebController.SubmitSuccess(parcel.Model);
                                    //}
                                    //else
                                    //{
                                    //    //取消订单
                                    //    LMSSFCommon.SfConfirm(parcel.Model.OrderId, parcel.Model.MailNo, _authorization,
                                    //                          2);
                                    //}
                                }
                                else
                                {
                                    SebController.SubmitFailure(wayBillSfModel.WayBillNumber, parcel.ErrorMsg);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("运单号：{0} 提交顺E宝API异常信息：{1}".FormatWith(wayBillSfModel.WayBillNumber, ex.Message));
                            }

                        }
                    }
                    pageindex++;
                } while (list.Count > (pageindex - 1) * pagesize);
            }
            Log.Info("完成预报顺E宝俄罗斯挂号，平邮");
            SebController.UpdateOutShippingMethod();
        }
    }
}
