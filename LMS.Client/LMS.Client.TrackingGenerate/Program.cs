using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Client.TrackingGenerate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                WayBillInfoRepository wayBillInfoRepository = new WayBillInfoRepository(new LMS_DbContext());
                TotalPackageOutStorageRelationalInfoRepository totalPackageOutStorageRelational =
                    new TotalPackageOutStorageRelationalInfoRepository(new LMS_DbContext());

                InTrackingLogInfoRepository inTrackingLogInfoRepository = new InTrackingLogInfoRepository(new LMS_DbContext());

                //最后的序号
                int lastWayBillEventLogId = 0;

                //获取所有需要处理的,每次取一定数量，往前移动
                List<WayBillEventLogExt> wayBillEventLogExtList;

                while ((wayBillEventLogExtList = wayBillInfoRepository.GetWayBillEventLogExtList(lastWayBillEventLogId, 50)).Any())
                {
                    Console.WriteLine("获取到条数：{0}", wayBillEventLogExtList.Count);
                    Log.Info(string.Format("获取到条数：{0}", wayBillEventLogExtList.Count));

                    //记录最后的序号
                    lastWayBillEventLogId = wayBillEventLogExtList.Last().WayBillEventLogId;

                    try
                    {
                        //所有运输方式ID
                        var shippingMethodIds =
                            wayBillEventLogExtList.Where(p => p.ShippingMethodId.HasValue).GroupBy(p => p.ShippingMethodId.Value).Select(p => p.Key).ToList();

                        //获取运输方式信息
                        var shippingMethodList = GetShippingMethodsByIds(shippingMethodIds);

                        List<InTrackingLogInfo> inTrackingLogInfoList = new List<InTrackingLogInfo>();
                        List<WayBillEventLogExt> wayBillEventLogExtChangedList = new List<WayBillEventLogExt>();
                        wayBillEventLogExtList.ForEach(p =>
                            {

                                if (p.EventCode == (int) WayBillEvent.EventCodeEnum.InStorage)
                                {
                                    //收货后，删除预报记录
                                    inTrackingLogInfoRepository.Remove(i => i.WayBillNumber == p.WayBillNumber);
                                    inTrackingLogInfoList.Remove(i => i.WayBillNumber == p.WayBillNumber);
                                }

                                InTrackingLogInfo inTrackingLogInfo = null;

                                var shippingMethod = shippingMethodList.FirstOrDefault(pp => pp.ShippingMethodId == p.ShippingMethodId);
                                if (shippingMethod != null&&!shippingMethod.Code.IsNullOrWhiteSpace())
                                {
                                    switch (shippingMethod.Code)
                                    { 
                                        case "SPLUS":
                                        case "SPLUSZ":
                                            // 中美专线shippingMethod.TrackingUrl.Contains("worldtrack.dhlglobalmail.com")
                                            inTrackingLogInfo = GenerateInTrackingLogInfoGdm(p);
                                            break;
                                        case "EUDDPG":
                                        case "EUDDP":
                                            inTrackingLogInfo = GenerateInTrackingLogInfoEUD(p,totalPackageOutStorageRelational);
                                            break;
                                        case "CNPOST-FYB":
                                            inTrackingLogInfo = GenerateInTrackingLogInfoFUB(p);
                                            break;
                                        default:
                                            inTrackingLogInfo = GenerateInTrackingLogInfoCommon(p);
                                            break;
                                    }
                                }
                                else
                                {
                                    inTrackingLogInfo = GenerateInTrackingLogInfoCommon(p);
                                }

                                if (inTrackingLogInfo != null)
                                {
                                    Console.WriteLine("需要更新运单：{0}", inTrackingLogInfo.WayBillNumber);

                                    inTrackingLogInfoList.Add(inTrackingLogInfo);
                                    wayBillEventLogExtChangedList.Add(p);
                                }

                            });

                        Log.Info(string.Format("需要更新运单数：{0}", inTrackingLogInfoList.Count));

                        if (inTrackingLogInfoList.Any())
                        {
                            Log.Info(string.Format("需要更新运单：{0}", string.Join(",", inTrackingLogInfoList.Select(p => p.WayBillNumber))));

                            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                            {
                                wayBillInfoRepository.BulkInsert("InTrackingLogInfos", inTrackingLogInfoList);
                                UpdateWayBillEventLogExt(wayBillEventLogExtChangedList);
                                inTrackingLogInfoRepository.UnitOfWork.Commit();

                                transaction.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Log.Exception(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log.Exception(ex);
            }

#if DEBUG
            Console.WriteLine("执行完成，任意键退出...");
            Console.ReadLine();
#endif
        }

        public static InTrackingLogInfo GenerateInTrackingLogInfoEUD(WayBillEventLogExt wayBillEventLogExt, TotalPackageOutStorageRelationalInfoRepository totalPackageOutStorageRelational)
        {
            wayBillEventLogExt.LastUpdatedOn = DateTime.Now;

            InTrackingLogInfo inTrackingLogInfo = new InTrackingLogInfo()
            {
                WayBillNumber = wayBillEventLogExt.WayBillNumber,
                ProcessDate = wayBillEventLogExt.EventDate,
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                CreatedBy = wayBillEventLogExt.Operator,
                LastUpdatedBy = wayBillEventLogExt.Operator,
            };

            if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.Submit)
            {
                inTrackingLogInfo.ProcessContent = "Shipping information received";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";

                wayBillEventLogExt.TrackingLogCreated = true;
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.InStorage)
            {
                inTrackingLogInfo.ProcessContent = "Arrived at Sort Facility in SHENZHEN";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";

                wayBillEventLogExt.TrackingLogCreated = true;
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.OutStorage)
            {
                if (!wayBillEventLogExt.TrackingLogProgress.HasValue)
                {
                    inTrackingLogInfo.ProcessContent = "Departed Facility in SHENZHEN";
                    inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";
                    if (totalPackageOutStorageRelational.Exists(r => r.OutStorageID == wayBillEventLogExt.OutStorageID))
                    {
                        wayBillEventLogExt.TrackingLogCreated = true;
                    }
                    else
                    {
                        wayBillEventLogExt.TrackingLogProgress = 1;
                    }
                }
                else if (wayBillEventLogExt.TrackingLogProgress.Value == 1 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 1)
                {
                    inTrackingLogInfo.ProcessContent = "Processed at HONG KONG";
                    inTrackingLogInfo.ProcessLocation = "HONGKONG - HONGKONG";
                    inTrackingLogInfo.ProcessDate = DateTime.Now.AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogProgress = 2;
                }
                else if (wayBillEventLogExt.TrackingLogProgress.Value == 2 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 2)
                {
                    inTrackingLogInfo.ProcessContent = "Ready for boarding";
                    inTrackingLogInfo.ProcessLocation = "HONGKONG - HONGKONG";
                    inTrackingLogInfo.ProcessDate = DateTime.Now.AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogProgress = 3;
                }
                else if (wayBillEventLogExt.TrackingLogProgress.Value == 3 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 3)
                {
                    inTrackingLogInfo.ProcessContent = "Arrived at Sort Facility AM, Clearance processing";
                    inTrackingLogInfo.ProcessLocation = "Amsterdam - Netherlands";
                    inTrackingLogInfo.ProcessDate = DateTime.Now.AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogProgress = 4;
                }
                else if (wayBillEventLogExt.TrackingLogProgress.Value == 4 &&
                         (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 4 && LocalTimeToEud(DateTime.Now).DayOfWeek != DayOfWeek.Sunday && LocalTimeToEud(DateTime.Now).DayOfWeek!=DayOfWeek.Saturday)
                {
                    if (wayBillEventLogExt.CountryCode == "DE")
                    {
                        inTrackingLogInfo.ProcessContent = "Processed for clearance, To DPWN with delivery courier ";
                        inTrackingLogInfo.ProcessLocation = "Amsterdam - Netherlands";                    
                    }
                    else
                    {
                        inTrackingLogInfo.ProcessContent = "Processed for clearance, To PostNL with delivery courier";
                        inTrackingLogInfo.ProcessLocation = "Amsterdam - Netherlands";
                    }
                    inTrackingLogInfo.ProcessDate = DateTime.Now.AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogCreated = true;
                    
                }
                else
                {
                    inTrackingLogInfo = null;
                }
            }
            else
            {
                inTrackingLogInfo = null;
            }

            return inTrackingLogInfo;
        }

        public static InTrackingLogInfo GenerateInTrackingLogInfoGdm(WayBillEventLogExt wayBillEventLogExt)
        {
            wayBillEventLogExt.LastUpdatedOn = DateTime.Now;

            InTrackingLogInfo inTrackingLogInfo = new InTrackingLogInfo()
                {
                    WayBillNumber = wayBillEventLogExt.WayBillNumber,
                    ProcessDate = wayBillEventLogExt.EventDate,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now,
                    CreatedBy = wayBillEventLogExt.Operator,
                    LastUpdatedBy = wayBillEventLogExt.Operator,
                };

            if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.Submit)
            {
                inTrackingLogInfo.ProcessContent = "Shipment information received";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";

                wayBillEventLogExt.TrackingLogCreated = true;
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.InStorage)
            {
                inTrackingLogInfo.ProcessContent = "Arrived at Sort Facility";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";

                wayBillEventLogExt.TrackingLogCreated = true;
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.OutStorage)
            {
                if (!wayBillEventLogExt.TrackingLogProgress.HasValue)
                {
                    inTrackingLogInfo.ProcessContent = "Shipment had sent to HONG KONG";
                    inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";

                    wayBillEventLogExt.TrackingLogProgress = 1;
                }
                else if ((wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Send || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.WaitOrder || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Delivered) && wayBillEventLogExt.TrackingLogProgress.Value == 1 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 1)
                {
                    inTrackingLogInfo.ProcessContent = "Processed at HONG KONG";
                    inTrackingLogInfo.ProcessLocation = "HONGKONG - HONGKONG";
                    inTrackingLogInfo.ProcessDate = wayBillEventLogExt.EventDate.AddDays(1).AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogProgress = 2;
                }
                else if ((wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Send || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.WaitOrder || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Delivered) && wayBillEventLogExt.TrackingLogProgress.Value == 2 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 2)
                {
                    inTrackingLogInfo.ProcessContent = "Ready for boarding";
                    inTrackingLogInfo.ProcessLocation = "HONGKONG - HONGKONG";
                    inTrackingLogInfo.ProcessDate = wayBillEventLogExt.EventDate.AddDays(2).AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                    wayBillEventLogExt.TrackingLogProgress = 3;
                }
                else if ((wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Send || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.WaitOrder || wayBillEventLogExt.Status == (int)WayBill.StatusEnum.Delivered) && wayBillEventLogExt.TrackingLogProgress.Value == 3 && (DateTime.Now - wayBillEventLogExt.EventDate).TotalDays > 3)
                {
                    inTrackingLogInfo.ProcessContent = "Arrived at US waiting customs clearance";
                    inTrackingLogInfo.ProcessLocation = "CVG - USA";
                    inTrackingLogInfo.ProcessDate = wayBillEventLogExt.EventDate.AddDays(3).AddMilliseconds(new Random().Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000)).AddHours(-13);//返回CVG当地时间，-5，+8
                    wayBillEventLogExt.TrackingLogCreated = true;
                }
                else
                {
                    inTrackingLogInfo = null;
                }
            }
            else
            {
                inTrackingLogInfo = null;
            }

            return inTrackingLogInfo;
        }

        public static InTrackingLogInfo GenerateInTrackingLogInfoCommon(WayBillEventLogExt wayBillEventLogExt)
        {
            wayBillEventLogExt.LastUpdatedOn = DateTime.Now;
            wayBillEventLogExt.TrackingLogCreated = true;

            InTrackingLogInfo inTrackingLogInfo = new InTrackingLogInfo()
                {
                    WayBillNumber = wayBillEventLogExt.WayBillNumber,
                    ProcessDate = wayBillEventLogExt.EventDate,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now,
                    CreatedBy = wayBillEventLogExt.Operator,
                    LastUpdatedBy = wayBillEventLogExt.Operator,
                };

            if (wayBillEventLogExt.EventCode == (int) WayBillEvent.EventCodeEnum.Submit)
            {
                inTrackingLogInfo.ProcessContent = "Order Processing";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";
            }
            else if (wayBillEventLogExt.EventCode == (int) WayBillEvent.EventCodeEnum.InStorage)
            {
                inTrackingLogInfo.ProcessContent = "Shipment picked up";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";
            }
            else if (wayBillEventLogExt.EventCode == (int) WayBillEvent.EventCodeEnum.OutStorage)
            {
                inTrackingLogInfo.ProcessContent = "OutStorage Scan";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.ReturnGood)
            {
                inTrackingLogInfo.ProcessContent = "Returned to Sender";
                inTrackingLogInfo.ProcessLocation = "SHENZHEN - CHINA";
            }
            else
            {
                inTrackingLogInfo = null;
            }

            return inTrackingLogInfo;
        }

        public static InTrackingLogInfo GenerateInTrackingLogInfoFUB(WayBillEventLogExt wayBillEventLogExt)
        {
            wayBillEventLogExt.LastUpdatedOn = DateTime.Now;
            wayBillEventLogExt.TrackingLogCreated = true;

            InTrackingLogInfo inTrackingLogInfo = new InTrackingLogInfo()
            {
                WayBillNumber = wayBillEventLogExt.WayBillNumber,
                ProcessDate = wayBillEventLogExt.EventDate,
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                CreatedBy = wayBillEventLogExt.Operator,
                LastUpdatedBy = wayBillEventLogExt.Operator,
                Remarks = wayBillEventLogExt.Remarks,
            };

            if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.Submit)
            {
                inTrackingLogInfo.ProcessContent = "运单已生成";
                inTrackingLogInfo.ProcessLocation = "深圳 - 中国";
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.InStorage)
            {
                inTrackingLogInfo.ProcessContent = "货物入库扫描 ";
                inTrackingLogInfo.ProcessLocation = "深圳 - 中国";
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.OutStorage)
            {
                inTrackingLogInfo.ProcessContent = "货物出库扫描";
                inTrackingLogInfo.ProcessLocation = "深圳 - 中国";
            }
            else if (wayBillEventLogExt.EventCode == (int)WayBillEvent.EventCodeEnum.ReturnGood)
            {
                inTrackingLogInfo.ProcessContent = "已退货";
                inTrackingLogInfo.ProcessLocation = "深圳 - 中国";
            }
            else if (wayBillEventLogExt.EventCode == 610 || wayBillEventLogExt.EventCode == 620
                || wayBillEventLogExt.EventCode == 630 || wayBillEventLogExt.EventCode == 635
                || wayBillEventLogExt.EventCode == 636)
            {
                inTrackingLogInfo.ProcessContent = wayBillEventLogExt.Description;
                inTrackingLogInfo.ProcessLocation = "福州 - 中国";
            }
            else if (wayBillEventLogExt.EventCode == 640 || wayBillEventLogExt.EventCode == 650)
            {
                inTrackingLogInfo.ProcessContent = wayBillEventLogExt.Description;
                inTrackingLogInfo.ProcessLocation = "台湾";
            }
            else if (wayBillEventLogExt.EventCode == 660)
            {
                inTrackingLogInfo.ProcessContent = wayBillEventLogExt.Description;
                inTrackingLogInfo.ProcessLocation = Regex.Match(wayBillEventLogExt.Description, "（(.*?)）").Groups[1].Value;
            }
            else
            {
                inTrackingLogInfo = null;
            }

            return inTrackingLogInfo;
        }

        public static void UpdateWayBillEventLogExt(IEnumerable<WayBillEventLogExt> wayBillEventLogExts)
        {
            WayBillEventLogRepository wayBillEventLogRepository = new WayBillEventLogRepository(new LMS_DbContext());
            wayBillEventLogExts.ToList().ForEach(p =>
                {
                    WayBillEventLog wayBillEventLog=new WayBillEventLog();
                    p.CopyTo(wayBillEventLog);
                    wayBillEventLogRepository.Modify(wayBillEventLog);
                });
            wayBillEventLogRepository.UnitOfWork.Commit();
        }

        public static void DeleteInTrackingLogInfo(string wayBillNumber)
        {
            InTrackingLogInfoRepository inTrackingLogInfoRepository = new InTrackingLogInfoRepository(new LMS_DbContext());
            inTrackingLogInfoRepository.Remove(i => i.WayBillNumber == wayBillNumber);
            inTrackingLogInfoRepository.UnitOfWork.Commit();
        }

        public static List<ShippingMethodModel> GetShippingMethodsByIds(List<int> shippingMethodIds)
        {
            shippingMethodIds.Add(126);
            var url = sysConfig.LISAPIPath + "API/LIS/PostShippingMethodsByIds";
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.POST,
                                                                           EnumContentType.Json, shippingMethodIds);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }
        /// <summary>
        /// 当地时间转换成荷兰时间
        /// (UTC+01:00)阿姆斯特丹，柏林，伯尔尼，罗马，斯德哥尔摩，维也纳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime LocalTimeToEud(DateTime dt)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local,
                                                   easternZone);
        }

    }
}
