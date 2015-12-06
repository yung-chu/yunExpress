using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.WayBillEventLogGenerate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var lmsDbContext = new LMS_DbContext();
                WayBillInfoRepository wayBillInfoRepository = new WayBillInfoRepository(lmsDbContext);
                MailPostBagInfoRepository mailPostBagInfoRepository = new MailPostBagInfoRepository(lmsDbContext);
                WayBillEventLogRepository wayBillEventLogRepository = new WayBillEventLogRepository(lmsDbContext);
                MailTotalPackageInfoRepository mailTotalPackageInfoRepository =
                    new MailTotalPackageInfoRepository(lmsDbContext);
                MailExchangeBagLogRepository mailExchangeBagLogRepository =
                    new MailExchangeBagLogRepository(lmsDbContext);
                MailReturnGoodsLogRepository mailReturnGoodsLogRepository =
                    new MailReturnGoodsLogRepository(lmsDbContext);
                MailHoldLogRepository mailHoldLogRepository =new MailHoldLogRepository(lmsDbContext);

                //MailTotalPackageOrPostBagRelationalRepository mailTotalPackageOrPostBagRelationalRepository= new MailTotalPackageOrPostBagRelationalRepository(lmsDbContext);

                List<MailPostBagInfo> listMailPostBagInfo;
                List<MailTotalPackageInfo> listMailTotalPackageInfo;
                List<MailReturnGoodsLog> listMailReturnGoodsLog;

                IEnumerable<Country> listCountry = new CountryRepository(lmsDbContext).GetAll().ToList();

                #region 收寄局

                while ((listMailPostBagInfo =
                        mailPostBagInfoRepository.GetFiltered(t => t.FuPostBagNumber != null && t.TrackStatus == 0)
                                                 .Take(50).ToList())
                    .Any())
                {
                    listMailPostBagInfo.ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                wayBillInfoRepository.GetList(
                                    w =>
                                    w.OutStorageID == p.OutStorageID && (w.Status == (int) WayBill.StatusEnum.Send ||
                                                                         w.Status == (int) WayBill.StatusEnum.WaitOrder)
                                    );

                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers =mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w =>
                                {
                                    //if (!wayBillEventLogRepository.Exists(
                                    //    we => we.WayBillNumber == w.WayBillNumber && we.EventCode == 610))
                                    //{
                                        wayBillEventLogRepository.Add(new WayBillEventLog()
                                            {
                                                WayBillNumber = w.WayBillNumber,
                                                EventCode = 610,
                                                EventDate = p.ScanTime.Value,
                                                Description = "收寄局已收寄",
                                                Operator = p.ScanBy,
                                                LastUpdatedOn = DateTime.Now,
                                                Remarks =
                                                    string.Format("发往：{0}",
                                                                  listCountry.First(c => c.CountryCode == w.CountryCode)
                                                                             .ChineseName)
                                            });
                                    //}
                                });

                            p.TrackStatus = 1;

                            Log.Info(string.Format("收寄局已收寄,更新袋号：{0}", p.PostBagNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailPostBagInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }

                }



                #endregion


                #region 收寄局换袋

                while ((listMailPostBagInfo =
                        mailPostBagInfoRepository.GetFiltered(t => t.FuPostBagNumber != null && t.TrackStatus == 3)
                                                 .Take(50).ToList())
                    .Any())
                {
                    listMailPostBagInfo.ForEach(p =>
                        {
                            var listTrackNumbers =
                                mailExchangeBagLogRepository.GetFiltered(me => me.NewPostBagNumber == p.PostBagNumber)
                                                            .Select(me => me.TrackNumber).ToList();
                            //获取换袋的运单
                            List<WayBillInfo> listWayBillInfo =
                                wayBillInfoRepository.GetList(
                                    w => w.OutStorageID == p.OutStorageID && listTrackNumbers.Contains(w.TrackingNumber)
                                         && (w.Status == (int) WayBill.StatusEnum.Send ||
                                             w.Status == (int) WayBill.StatusEnum.WaitOrder));
                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w =>
                                {
                                    //if (!wayBillEventLogRepository.Exists(
                                    //    we => we.WayBillNumber == w.WayBillNumber && we.EventCode == 610))
                                    //{
                                        wayBillEventLogRepository.Add(new WayBillEventLog()
                                            {
                                                WayBillNumber = w.WayBillNumber,
                                                EventCode = 610,
                                                EventDate = p.ScanTime.Value,
                                                Description = "收寄局已收寄",
                                                Operator = p.ScanBy,
                                                LastUpdatedOn = DateTime.Now,
                                                Remarks =
                                                    string.Format("发往：{0}",
                                                                  listCountry.First(c => c.CountryCode == w.CountryCode)
                                                                             .ChineseName)
                                            });
                                    //}
                                });

                            p.TrackStatus = 1;

                            Log.Info(string.Format("收寄局已收寄,更新袋号：{0}", p.PostBagNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailPostBagInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }
                }


                #endregion


                #region 处理中心扫描

                while ((listMailPostBagInfo = mailPostBagInfoRepository.GetUnTrackingCreated(50).ToList()).Any())
                {
                    listMailPostBagInfo.ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                wayBillInfoRepository.GetList(
                                    w =>
                                    w.OutStorageID == p.OutStorageID && (w.Status == (int) WayBill.StatusEnum.Send ||
                                                                         w.Status == (int) WayBill.StatusEnum.WaitOrder));

                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w => wayBillEventLogRepository.Add(new WayBillEventLog()
                                {
                                    WayBillNumber = w.WayBillNumber,
                                    EventCode = 620,
                                    EventDate =
                                        p.MailTotalPackageOrPostBagRelationals.First().ScanTime,
                                    Description = "邮政处理中心直封封发",
                                    Operator =
                                        p.MailTotalPackageOrPostBagRelationals.First().CreatedBy,
                                    LastUpdatedOn = DateTime.Now,
                                    Remarks =
                                        string.Format("封发总包号：{0}",
                                                      p.MailTotalPackageOrPostBagRelationals
                                                       .First()
                                                       .MailTotalPackageInfo.ShortNumber)
                                }));

                            p.TrackStatus = 2;

                            Log.Info(string.Format("处理中心,更新袋号：{0}", p.PostBagNumber));
                        });

                    using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                                  new TimeSpan(0, 5, 0)))
                    {
                        mailPostBagInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }
                }



                #endregion


                #region 离开福州

                while ((listMailTotalPackageInfo = mailTotalPackageInfoRepository.GetFiltered(
                    t =>
                    t.FZFlightNo != null && t.FuZhouDepartureTime.HasValue && t.TrackStatus == 0 &&
                    t.FuZhouDepartureTime.Value < DateTime.Now).Take(50).ToList()).Any())
                {
                    listMailTotalPackageInfo.ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                mailPostBagInfoRepository.GetWayBillByMailTotalPackageNumber(
                                    p.MailTotalPackageNumber).Where(w => w.Status == (int) WayBill.StatusEnum.Send ||
                                                                         w.Status == (int) WayBill.StatusEnum.WaitOrder)
                                                         .ToList();
                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w =>
                                {
                                    wayBillEventLogRepository.Add(new WayBillEventLog()
                                        {
                                            WayBillNumber = w.WayBillNumber,
                                            EventCode = 630,
                                            EventDate = p.FuZhouDepartureTime.Value,
                                            Description = "货物配载启运，发往台北",
                                            Operator = p.LastUpdatedBy,
                                            LastUpdatedOn = DateTime.Now,
                                            Remarks =
                                                p.FZFlightType == 1
                                                    ? string.Format("航班号：{0}", p.FZFlightNo)
                                                    : string.Format("邮轮号：{0}", p.FZFlightNo),
                                        });
                                });

                            p.TrackStatus = 1;

                            Log.Info(string.Format("离开福州,更新中总包号：{0}", p.MailTotalPackageNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailTotalPackageInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }
                }




                #endregion


                #region 到达台湾

                while (
                    (listMailTotalPackageInfo =
                     mailTotalPackageInfoRepository.GetFiltered(
                         t =>
                         t.TaiWanArrivedTime.HasValue && t.TrackStatus == 1 &&
                         t.TaiWanArrivedTime.Value < DateTime.Now)
                                                   .Take(50).ToList()).Any())
                {
                    listMailTotalPackageInfo.ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                mailPostBagInfoRepository.GetWayBillByMailTotalPackageNumber(
                                    p.MailTotalPackageNumber);

                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w =>
                                {
                                    wayBillEventLogRepository.Add(new WayBillEventLog()
                                        {
                                            WayBillNumber = w.WayBillNumber,
                                            EventCode = 640,
                                            EventDate = p.TaiWanArrivedTime.Value,
                                            Description = string.Format("到达台北分拨中心，等待交航"),
                                            Operator = p.LastUpdatedBy,
                                            LastUpdatedOn = DateTime.Now,
                                        });

                                });

                            p.TrackStatus = 2;

                            Log.Info(string.Format("到达台湾,更新中总包号：{0}", p.MailTotalPackageNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailTotalPackageInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }
                }




                #endregion


                #region 离开台湾

                while (
                    (listMailTotalPackageInfo =
                     mailTotalPackageInfoRepository.GetFiltered(
                         t =>
                         t.TWFlightNo != null && t.TaiWanDepartureTime.HasValue &&
                         t.TrackStatus == 2 && t.TaiWanDepartureTime.Value < DateTime.Now).Take(50).ToList()).Any())
                {
                    listMailTotalPackageInfo.ToList().ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                mailPostBagInfoRepository.GetWayBillByMailTotalPackageNumber(
                                    p.MailTotalPackageNumber);

                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));

                            listWayBillInfo.ForEach(w =>
                                {

                                    wayBillEventLogRepository.Add(new WayBillEventLog()
                                        {
                                            WayBillNumber = w.WayBillNumber,
                                            EventCode = 650,
                                            EventDate = p.TaiWanDepartureTime.Value,
                                            Description = string.Format("货物交航\r\n(此信息由航空公司提供)"),
                                            Operator = p.LastUpdatedBy,
                                            LastUpdatedOn = DateTime.Now,
                                            Remarks = string.Format("航班号：{0}", p.TWFlightNo),
                                        });
                                });

                            p.TrackStatus = 3;

                            Log.Info(string.Format("离开台湾,更新中总包号：{0}", p.MailTotalPackageNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailTotalPackageInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }

                }



                #endregion


                #region 到达目的国

                while (
                    (listMailTotalPackageInfo =
                     mailTotalPackageInfoRepository.GetFiltered(
                         t =>
                         t.ToArrivedTime.HasValue && t.TrackStatus == 3 &&
                         t.ToArrivedTime.Value < DateTime.Now).Take(50).ToList()).Any())
                {
                    listMailTotalPackageInfo.ToList().ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                mailPostBagInfoRepository.GetWayBillByMailTotalPackageNumber(
                                    p.MailTotalPackageNumber);

                            //退件的不生成记录
                            var returnTrackNumbers =
                                mailReturnGoodsLogRepository.GetAll()
                                                            .Select(mr => mr.TrackNumber);
                            //拦截的不生成记录
                            var holdTrackNumbers = mailHoldLogRepository.GetAll().Select(mr => mr.TrackingNumber);
                            listWayBillInfo.RemoveAll(w => returnTrackNumbers.Contains(w.TrackingNumber));
                            listWayBillInfo.RemoveAll(w => holdTrackNumbers.Contains(w.TrackingNumber));


                            //中文国家名
                            var countryChineseName = listCountry.First(
                                c =>
                                c.CountryCode ==
                                mailPostBagInfoRepository.GetCountryCodeByMailTotalPackageNumber(p.MailTotalPackageNumber)).ChineseName;

                            listWayBillInfo.ForEach(w =>
                                {
                                    wayBillEventLogRepository.Add(new WayBillEventLog()
                                        {
                                            WayBillNumber = w.WayBillNumber,
                                            EventCode = 660,
                                            EventDate = p.ToArrivedTime.Value,
                                            Description =
                                                string.Format("到达目的地（{0}），预计送达时间1～2天\r\n(此信息由航空公司提供)",
                                                              countryChineseName),
                                            Operator = p.LastUpdatedBy,
                                            LastUpdatedOn = DateTime.Now,
                                            Remarks = string.Format("航班号：{0}", p.TWFlightNo),
                                        });

                                });

                            p.TrackStatus = 4;

                            Log.Info(string.Format("到达目的国,更新中总包号：{0}", p.MailTotalPackageNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailTotalPackageInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }
                }

                #endregion


                #region 退件

                while ((listMailReturnGoodsLog =
                        mailReturnGoodsLogRepository.GetFiltered(t => t.TrackStatus == 0)
                                                    .Take(50).ToList())
                    .Any())
                {
                    listMailReturnGoodsLog.ForEach(p =>
                        {
                            List<WayBillInfo> listWayBillInfo =
                                wayBillInfoRepository.GetList(
                                    w =>
                                    w.TrackingNumber == p.TrackNumber && (w.Status == (int) WayBill.StatusEnum.Send ||
                                                                          w.Status == (int) WayBill.StatusEnum.WaitOrder)
                                    );

                            listWayBillInfo.ForEach(w => wayBillEventLogRepository.Add(new WayBillEventLog()
                                {
                                    WayBillNumber = w.WayBillNumber,
                                    EventCode = 635,
                                    EventDate = p.ReturnOn,
                                    Description = "退回操作中心",
                                    Operator = p.ReturnBy,
                                    LastUpdatedOn = DateTime.Now,
                                    Remarks = string.Format("已退回操作中心")
                                }));

                            p.TrackStatus = 1;

                            Log.Info(string.Format("收寄局已收寄,更新袋号：{0}", p.PostBagNumber));
                        });

                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        mailPostBagInfoRepository.UnitOfWork.Commit();
                        wayBillEventLogRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }

                }



                #endregion

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
