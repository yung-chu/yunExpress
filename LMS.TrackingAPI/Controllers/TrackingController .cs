using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.TrackingAPI.Infrastructure;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;

namespace LMS.TrackingAPI.Controllers
{
    public class TrackingController : ApiController
    {
        private static readonly string S_LastUpdatedOnFilePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "LastSelectTime.txt");
        private readonly IFreightService _freightService;
        private readonly IOrderService _orderService;
        //public static DateTime endTime = DateTime.Parse("2000-01-01"); //DateTime.TryParse(2010-01-01);
        public TrackingController(IOrderService orderService,IFreightService freightService)
        {
            _freightService = freightService;
            _orderService = orderService;
        }

        #region 提交跟踪号信息查询跟踪信息
        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <param name="orderTrackingRequests"></param>
        /// <returns></returns>
        [HttpPost]
        public int AddOutTrackingInfoList(List<OrderTrackingRequestModel> orderTrackingRequests)
        {
            return _freightService.AddOutTrackingInfo(orderTrackingRequests);
        }

        [HttpPost]
        public int AddOutTrackingInfo(OrderTrackingRequestModel orderTrackingRequests)
        {
            List<OrderTrackingRequestModel> models=new List<OrderTrackingRequestModel>();
            models.Add(orderTrackingRequests);
            return _freightService.AddOutTrackingInfo(models);
        }
        #endregion 


        #region 提交跟踪号，获取外部跟踪信息
        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <param name="TrackingNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderTrackingModel GetOutTrackingInfo(string TrackingNumber)
        {
            return _freightService.GetOutTrackingInfo(TrackingNumber);
        }

        //[HttpPost]
        public List<OrderTrackingModel> GetOutTrackingInfoList(string TrackingNumber)
        {
            TrackingNumber = TrackingNumber.Substring(0, TrackingNumber.Length - 1);
            return _freightService.GetOutTrackingInfoList(TrackingNumber);
            
        }

        #endregion

        #region 提交跟踪号服务

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        [HttpGet]
        public int Add()
        {
            var shippingMehtod = _freightService.GetShippingMethods("", true);
            const string dhlUrl = "WWW.DHL.COM";
            const string upsUrl = "WWW.UPS.COM";
            const string equickUrl = "WWW.EQUICK.CN";
            const string dhlGlobalMailUrl = "WORLDTRACK.DHLGLOBALMAIL.COM";
            List<int> dhlshippingMehtods = new List<int>();
            List<int> upsshippingMehtods = new List<int>();
            List<int> equickshippingMehtods = new List<int>();
            List<int> dhlGlobalMailMehtods = new List<int>();
            List<int> shippingMehtods = new List<int>();
            string result = "";
            if (shippingMehtod.Count > 0)
            {
                shippingMehtod.ForEach(p =>
                    {
                        if (p.TrackingUrl != null)
                        {
                            bool need = false;

                            if (dhlUrl.Contains(p.TrackingUrl.ToUpperInvariant()))
                            {
                                dhlshippingMehtods.Add(p.ShippingMethodId);
                                need = true;
                            }
                            else if (upsUrl.Contains(p.TrackingUrl.ToUpperInvariant()))
                            {
                                upsshippingMehtods.Add(p.ShippingMethodId);
                                need = true;
                            }
                            else if (equickUrl.Contains(p.TrackingUrl.ToUpperInvariant()))
                            {
                                equickshippingMehtods.Add(p.ShippingMethodId);
                                need = true;
                            }
                            else if (p.TrackingUrl.ToUpperInvariant().Contains(dhlGlobalMailUrl))
                            {
                                dhlGlobalMailMehtods.Add(p.ShippingMethodId);
                                need = true;
                            }

                            if (need)
                            {
                                shippingMehtods.Add(p.ShippingMethodId);
                                result += "抓取的运输方式" + "[ID:" + p.ShippingMethodId + ",中文名" + p.FullName + "]";
                            }

                        }
                    });
            }
            //获取时间
            DateTime dtLastUpdatedOn = GetLastUpdatedOn();
            var wayBillList = _orderService.GetWayBillTakeList(shippingMehtods, dtLastUpdatedOn);
            var wayBillInfos = wayBillList as IList<WayBillInfo> ?? wayBillList.ToList();
            result += "抓取的运单数(" + wayBillInfos.Count().ToString() + ")";
            List<OrderTrackingRequestModel> trackingNubmerList = new List<OrderTrackingRequestModel>();
            //string temp = string.Join(",", wayBillInfos.Select(p => p.WayBillNumber));

            wayBillInfos.ToList().ForEach(p =>
                {
                    OrderTrackingRequestModel model = new OrderTrackingRequestModel();
                    if (dhlshippingMehtods.Contains(p.InShippingMethodID ?? -1))
                    {
                        model.ShipmentID = 1;
                    }
                    else if (upsshippingMehtods.Contains(p.InShippingMethodID ?? -1))
                    {
                        model.ShipmentID = 2;
                    }
                    else if (equickshippingMehtods.Contains(p.InShippingMethodID ?? -1))
                    {
                        model.ShipmentID = 3;
                    }
                    else if (dhlGlobalMailMehtods.Contains(p.InShippingMethodID ?? -1))
                    {
                        model.ShipmentID = 4;
                    }

                    model.CustomerCode = "admin";

                    var sm = shippingMehtod.Find(s => s.ShippingMethodId == (p.InShippingMethodID ?? -1));
                    if (sm != null && sm.IsHideTrackingNumber)
                    {
                        model.TrackingNumber = p.TrueTrackingNumber;
                    }
                    else
                    {
                        model.TrackingNumber = p.TrackingNumber;
                    }

                    if (model.ShipmentID != 0 && !string.IsNullOrWhiteSpace(model.TrackingNumber))
                    {
                        trackingNubmerList.Add(model);
                    }
                });
            try
            {
                //string temp2 = string.Join(",", trackingNubmerList.Select(p => p.TrackingNumber));

                var total = AddOutTrackingInfoList(trackingNubmerList);
                result += "请求的运单数量:" + trackingNubmerList.Count.ToString() + "," + "返回的成功数量:" + total.ToString();
                if (trackingNubmerList.Any() && total == trackingNubmerList.Count())
                {
                    //记录最后时间
                    DateTime lastLastUpdatedOn = wayBillInfos.OrderByDescending(p => p.LastUpdatedOn).FirstOrDefault().LastUpdatedOn;

                    Log.Info(string.Format("记录最后时间：{0},在{1}", lastLastUpdatedOn.ToString("yyyy-MM-dd hh:mm:ss.fff"), S_LastUpdatedOnFilePath));
                    WriteLastUpdatedOn(lastLastUpdatedOn);
                }

                Log.Info(result);
                return total;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }
        }

        /// <summary> 获取跟踪信息状态，并更改运单，订单状态
        /// 获取跟踪信息状态，并更改运单，订单状态
        /// </summary>
        [HttpGet]
        public int GetStatus()
        {
            string result = "";
            int number = 0;
            var trackingNubmerList = _orderService.GetTrueTrackingNumber();
            trackingNubmerList = trackingNubmerList.Substring(0, trackingNubmerList.Length - 1);
            string[] trackingNubmers = trackingNubmerList.Split(',');
            int index = 0;
            string trackingNubmer = "";
            string errortrackingNubmers = "";
            string ReturnTrackNumber = "";
            string SuccessTrackNumber = "";
                try
                {
                    trackingNubmers.ToList().ForEach(p =>
                        {
                            trackingNubmer += p+",";
                            index++;
                            if (index%1000 == 0)
                            {
                                if (!string.IsNullOrWhiteSpace(trackingNubmer))
                                {
                                    GetOutTrackingInfoList(trackingNubmer).ForEach(z =>
                                    {

                                        ReturnTrackNumber += z.TrackingNumber + ",";
                                        if (z.PackageState == 3)
                                        {
                                           bool r= _orderService.UpdateWayBillAndOrderStust(z.TrackingNumber);
                                           if (r)
                                           {
                                               SuccessTrackNumber += z.TrackingNumber + ",";
                                           }else
                                           {
                                               errortrackingNubmers += "[" + z.TrackingNumber + "(" + z.PackageState + ")" + "]";
                                           }
                                        }
                                    });
                                    index = 0;
                                }
                                trackingNubmer = "";
                            }
                        });
                    if (!string.IsNullOrWhiteSpace(trackingNubmer))
                    {
                        GetOutTrackingInfoList(trackingNubmer).ForEach(z =>
                            {
                                ReturnTrackNumber += z.TrackingNumber + ",";
                                if (z.PackageState == 3)
                                {
                                    var r = _orderService.UpdateWayBillAndOrderStust(z.TrackingNumber);
                                    if (r)
                                    {
                                        SuccessTrackNumber += z.TrackingNumber + ",";
                                        number++;
                                    }
                                    else
                                    {
                                        errortrackingNubmers += "[" + z.TrackingNumber + "(" + z.PackageState + ")" +
                                                                "]";
                                    }
                                }
                            });
                    }
                    //result = number.ToString();
                    Log.Info("失败的运单跟踪号" + errortrackingNubmers);
                    Log.Info("获取信息的运单跟踪号" + ReturnTrackNumber);
                    Log.Info("成功修改状态的运单跟踪号" + SuccessTrackNumber);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            return number;
        }

        #endregion

        private void WriteLastUpdatedOn(DateTime date)
        {
            File.WriteAllText(S_LastUpdatedOnFilePath, date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        private DateTime GetLastUpdatedOn()
        {
            if (File.Exists(S_LastUpdatedOnFilePath))
            {
                string strLastUpdatedOn = File.ReadAllText(S_LastUpdatedOnFilePath);
                DateTime dtLastUpdatedOn;
                if (DateTime.TryParse(strLastUpdatedOn, out dtLastUpdatedOn))
                    return dtLastUpdatedOn;
            }else
            {
                File.WriteAllText(S_LastUpdatedOnFilePath, "2000-01-01");
            }
            return DateTime.MinValue;
        }
    }
}