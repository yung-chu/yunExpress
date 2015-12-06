using System.Transactions;
using LMS.Core;
using LighTake.Infrastructure.Common.Logging;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using System.Text.RegularExpressions;

namespace LMS.Services.FubServices
{
    public class FubService : IFubService
    {

        private readonly IMailPostBagInfoRepository _mailPostBagInfoRepository;
        private readonly IMailTotalPackageInfoRepository _mailTotalPackageInfoRepository;
        private readonly IMailReturnGoodsLogRepository _mailReturnGoodsLogRepository;
        private readonly IMailExchangeBagLogRepository _mailExchangeBagLogRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IWorkContext _workContext;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IMailHoldLogRepository _mailHoldLogRepository;
        private readonly IWayBillEventLogRepository _wayBillEventLogRepository;

        public FubService(IMailPostBagInfoRepository mailPostBagInfoRepository,
                          IWorkContext workContext,
                          IMailTotalPackageInfoRepository mailTotalPackageInfoRepository,
                          ICountryRepository countryRepository,
                          IMailExchangeBagLogRepository mailExchangeBagLogRepository,
                          IMailReturnGoodsLogRepository mailReturnGoodsLogRepository,
                          IWayBillInfoRepository wayBillInfoRepository,
                          IMailHoldLogRepository mailHoldLogRepository,
                          IWayBillEventLogRepository wayBillEventLogRepository
            )
        {
            _mailPostBagInfoRepository = mailPostBagInfoRepository;
            _workContext = workContext;
            _mailTotalPackageInfoRepository = mailTotalPackageInfoRepository;
            _countryRepository = countryRepository;
            _mailExchangeBagLogRepository = mailExchangeBagLogRepository;
            _mailReturnGoodsLogRepository = mailReturnGoodsLogRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _mailHoldLogRepository = mailHoldLogRepository;
            _wayBillEventLogRepository = wayBillEventLogRepository;
        }


        public IPagedList<FubListModelExt> GetFubPagedList(FubListParam param)
        {
            return _mailPostBagInfoRepository.GetFubPagedList(param);
        }

        public IPagedList<FubListModelExt> GetFubCenterPagedList(FubListParam param)
        {
            return _mailPostBagInfoRepository.GetFubCenterPagedList(param);
        }

        public bool LogFlightNumber(MailTotalPackageInfoExt model)
        {
            var isupdate = false;
            var info =
                _mailTotalPackageInfoRepository.Single(p => p.MailTotalPackageNumber == model.MailTotalPackageNumber);
            if (model != null && info != null)
            {
                if (model.FZFlightType != null && !model.FZFlightNo.IsNullOrWhiteSpace() &&
                    model.FuZhouDepartureTime.HasValue && model.TaiWanArrivedTime.HasValue)
                {
                    info.FZFlightType = model.FZFlightType;
                    info.FZFlightNo = model.FZFlightNo;
                    info.FuZhouDepartureTime = model.FuZhouDepartureTime;
                    info.TaiWanArrivedTime = model.TaiWanArrivedTime;
                    isupdate = true;
                }
                if (!model.TWFlightNo.IsNullOrWhiteSpace() && model.TaiWanDepartureTime.HasValue &&
                    model.ToArrivedTime.HasValue)
                {
                    info.TWFlightNo = model.TWFlightNo;
                    info.TaiWanDepartureTime = model.TaiWanDepartureTime;
                    info.ToArrivedTime = model.ToArrivedTime;
                    isupdate = true;
                }
            }
            if (isupdate)
            {
                info.LastUpdatedBy = _workContext.User.UserUame;
                info.LastUpdatedOn = DateTime.Now;
                try
                {
                    _mailTotalPackageInfoRepository.Modify(info);
                    _mailTotalPackageInfoRepository.UnitOfWork.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
            return false;

        }

        /// <summary>
        /// 验证包裹单号
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns>0-验证成功,1-不存在，2-已退件</returns>
        public string CheckTrackNumber(string trackNumber)
        {
            return _mailExchangeBagLogRepository.CheckTrackNumber(trackNumber);
        }

        /// <summary>
        /// 验证目的袋牌
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <returns>0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，3-该单号与目标袋牌国家不匹配,
        /// 4-该目标袋牌重量已超重不能放入,100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        public string CheckBagNumber(string bagNumber, string trackNumber)
        {
            return _mailExchangeBagLogRepository.CheckBagNumber(bagNumber, trackNumber);
        }

        /// <summary>
        /// 保存换袋记录
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <returns>-1-失败， 0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，3-该单号与目标袋牌国家不匹配,
        /// 4-该目标袋牌重量已超重不能放入,5-该单号发货渠道错误，100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        public int SacnPackageExchangeBag(string bagNumber, string trackNumber)
        {
            return _mailExchangeBagLogRepository.SacnPackageExchangeBag(bagNumber, trackNumber,
                                                                        _workContext.User.UserUame);
        }

        /// <summary>
        /// 获取打印袋牌数据
        /// </summary>
        /// <param name="outStorageId">出仓ID</param>
        /// <returns></returns>
        public BagTagPrintExt GetBagTagPrint(string outStorageId)
        {
            try
            {
                var tag = _mailPostBagInfoRepository.GetBagTagPrint(outStorageId);
                if (tag != null)
                {
                    var chineseCountryName = _countryRepository.GetCountryChineseName(tag.CountryName);
                    tag.CountryName = string.Format("{0}({1})", chineseCountryName, tag.CountryName);
                }
                return tag;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return null;
        }

        /// <summary>
        /// 获取云途袋牌号信息
        /// </summary>
        /// <param name="postBagNumber">云途袋牌号</param>
        /// <returns></returns>
        public MailPostBagInfoExt GetMailPostBagInfoExt(string postBagNumber)
        {
            try
            {
                var model = _mailPostBagInfoRepository.Single(p => p.PostBagNumber == postBagNumber);
                if (model != null)
                {
                    return new MailPostBagInfoExt()
                        {
                            CountryCode = model.CountryCode,
                            CreatedBy = model.CreatedBy,
                            CreatedOn = model.CreatedOn,
                            FuPostBagNumber = model.FuPostBagNumber,
                            IsBattery = model.IsBattery,
                            LastUpdatedBy = model.LastUpdatedBy,
                            LastUpdatedOn = model.LastUpdatedOn,
                            OutStorageID = model.OutStorageID,
                            PostBagNumber = model.PostBagNumber,
                            ScanTime = model.ScanTime
                        };
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取云途袋牌号信息
        /// </summary>
        /// <param name="fuPostBagNumber">邮政袋牌号</param>
        /// <returns></returns>
        public MailPostBagInfoExt GetMailPostBagInfoByFu(string fuPostBagNumber)
        {
            try
            {
                var model = _mailPostBagInfoRepository.Single(p => p.FuPostBagNumber == fuPostBagNumber);
                if (model != null)
                {
                    return new MailPostBagInfoExt()
                        {
                            CountryCode = model.CountryCode,
                            CreatedBy = model.CreatedBy,
                            CreatedOn = model.CreatedOn,
                            FuPostBagNumber = model.FuPostBagNumber,
                            IsBattery = model.IsBattery,
                            LastUpdatedBy = model.LastUpdatedBy,
                            LastUpdatedOn = model.LastUpdatedOn,
                            OutStorageID = model.OutStorageID,
                            PostBagNumber = model.PostBagNumber,
                            ScanTime = model.ScanTime
                        };
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 换袋扫描
        /// </summary>
        /// <param name="postBagNumber">云途袋牌号</param>
        /// <param name="fuPostBagNumber">邮政袋牌号</param>
        /// <returns></returns>
        public bool SacnMailPostBagInfo(string postBagNumber, string fuPostBagNumber)
        {
            try
            {
                _mailPostBagInfoRepository.Modify(m => new MailPostBagInfo()
                    {
                        FuPostBagNumber = fuPostBagNumber,
                        LastUpdatedBy = _workContext.User.UserUame,
                        LastUpdatedOn = DateTime.Now,
                        ScanTime = DateTime.Now,
                        ScanBy = _workContext.User.UserUame,
                    }, t => t.PostBagNumber == postBagNumber);
                _mailPostBagInfoRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }

        /// <summary>
        /// 录入航班号查询列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<MailTotalPackageInfoExt> GetailTotalPackageList(LogFlightNumberListParam param)
        {
            var result = new PagedList<MailTotalPackageInfoExt>
                {
                    PageIndex = param.Page,
                    PageSize = param.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    InnerList = new List<MailTotalPackageInfoExt>()
                };
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);


            Expression<Func<MailTotalPackageInfo, bool>> filter = p => true;
            int shortNumber;
            if (!param.MailTotalPackageNumber.IsNullOrWhiteSpace() &&
                Int32.TryParse(param.MailTotalPackageNumber, out shortNumber))
            {
                filter = filter.And(p => p.ShortNumber == shortNumber);
            }
            filter = filter.And(p => p.ScanTime >= startTime && p.ScanTime <= endTime);
            Func<IQueryable<MailTotalPackageInfo>, IOrderedQueryable<MailTotalPackageInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            var list = _mailTotalPackageInfoRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
            if (list.Any())
            {
                result.PageIndex = list.PageIndex;
                result.PageSize = list.PageSize;
                result.TotalCount = list.TotalCount;
                result.TotalPages = list.TotalPages;
                var numbers = new List<string>();
                list.InnerList.ForEach(p =>
                    {
                        numbers.Add(p.MailTotalPackageNumber);
                        result.InnerList.Add(new MailTotalPackageInfoExt()
                            {
                                MailTotalPackageNumber = p.MailTotalPackageNumber,
                                CreatedBy = p.CreatedBy,
                                CreatedOn = p.CreatedOn,
                                FuZhouDepartureTime = p.FuZhouDepartureTime,
                                FZFlightNo = p.FZFlightNo,
                                LastUpdatedBy = p.LastUpdatedBy,
                                LastUpdatedOn = p.LastUpdatedOn,
                                ScanTime = p.ScanTime,
                                TaiWanArrivedTime = p.TaiWanArrivedTime,
                                TWFlightNo = p.TWFlightNo,
                                ToArrivedTime = p.ToArrivedTime,
                                TaiWanDepartureTime = p.TaiWanDepartureTime,
                                TotalPackageNumber = p.TotalPackageNumber,
                                TrackStatus = p.TrackStatus,
                                ShortNumber = p.ShortNumber
                            });
                    });
                var countrys = _mailPostBagInfoRepository.GetMailTotalPackageInfoCountry(numbers);
                result.InnerList.ForEach(p =>
                    {
                        p.CountryCode = countrys[p.MailTotalPackageNumber].CountryCode;
                    });
            }
            return result;
        }

        /// <summary>
        /// 根据总包号主键查询总包号信息
        /// </summary>
        /// <param name="mailTotalPackageNumber"></param>
        /// <returns></returns>
        public MailTotalPackageInfoExt GetMailTotalPackageInfoExt(string mailTotalPackageNumber)
        {
            try
            {
                var model =
                    _mailTotalPackageInfoRepository.Single(p => p.MailTotalPackageNumber == mailTotalPackageNumber);
                if (model != null)
                {
                    return new MailTotalPackageInfoExt()
                        {
                            MailTotalPackageNumber = model.MailTotalPackageNumber,
                            CreatedBy = model.CreatedBy,
                            CreatedOn = model.CreatedOn,
                            FuZhouDepartureTime = model.FuZhouDepartureTime,
                            FZFlightNo = model.FZFlightNo,
                            LastUpdatedBy = model.LastUpdatedBy,
                            LastUpdatedOn = model.LastUpdatedOn,
                            ScanTime = model.ScanTime,
                            TaiWanArrivedTime = model.TaiWanArrivedTime,
                            TWFlightNo = model.TWFlightNo,
                            ToArrivedTime = model.ToArrivedTime,
                            TaiWanDepartureTime = model.TaiWanDepartureTime,
                            TotalPackageNumber = model.TotalPackageNumber,
                            TrackStatus = model.TrackStatus,
                            ShortNumber = model.ShortNumber,
                            FZFlightType = model.FZFlightType
                        };
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }


        public ResultInfo MainPostNumberSave(string fuPostNumber, string mainPostNumber)
        {
            ResultInfo result = new ResultInfo();

            MailTotalPackageInfo m = new MailTotalPackageInfo();
            string mainCountryCode = "";
            string fuCountryCode = "";
            string fourBagNumberStr = GetMainPackageNumber(mainPostNumber);
            if (string.IsNullOrWhiteSpace(fourBagNumberStr))
            {
                result.Status = false;
                result.Message = "[总包号]不合法.";
                return result;
            }

            int fourBagNumber;
            int.TryParse(fourBagNumberStr, out fourBagNumber);

            m.MailTotalPackageNumber = DateTime.Now.ToString("yyyyMMdd") + fourBagNumberStr;
            m.TotalPackageNumber = mainPostNumber;
            m.ShortNumber = fourBagNumber;
            m.ScanTime = DateTime.Now;
            m.TrackStatus = 0;
            m.CreatedBy = _workContext.User.UserUame;
            m.CreatedOn = DateTime.Now;

            DateTime? dt = null;
            m.TaiWanArrivedTime = dt;
            m.TaiWanDepartureTime = dt;
            m.ToArrivedTime = dt;
            m.FuZhouDepartureTime = dt;
            m.LastUpdatedOn = DateTime.Now;
            m.LastUpdatedBy = _workContext.User.UserUame;

            MailTotalPackageOrPostBagRelational re = new MailTotalPackageOrPostBagRelational();
            re.CreatedBy = _workContext.User.UserUame;
            re.CreatedOn = DateTime.Now;
            //re.MailTotalPackageNumber
            //获取客袋带牌号信息
            var mailPostBagInfo = _mailPostBagInfoRepository.GetYunExpressBagNumber(fuPostNumber);
            if (mailPostBagInfo != null)
            {
                re.PostBagNumber = mailPostBagInfo.PostBagNumber;
                fuCountryCode = mailPostBagInfo.CountryCode.ToUpperInvariant();
            }
            re.ScanTime = DateTime.Now;
            re.LastUpdatedOn = DateTime.Now;
            re.LastUpdatedBy = _workContext.User.UserUame;

            
            if (mainPostNumber.Length > 8)
            {
                mainCountryCode = mainPostNumber.Substring(6, 2).ToUpperInvariant();
            }

            // 国际小包U+ 总包号扫描校验客户袋牌与总包号对应国家是否一致。
            // Add by zhengsong
            if (string.IsNullOrWhiteSpace(mainCountryCode) || string.IsNullOrWhiteSpace(fuCountryCode) ||
                mainCountryCode != fuCountryCode)
            {
                result.Status = false;
                result.Message = "客户袋牌与总包号对应国家不一致，请检查是否贴错！关联不成功！";
                return result;
            }

            if (string.IsNullOrWhiteSpace(re.PostBagNumber))
            {
                result.Status = false;
                result.Message = "未找到[福邮袋牌]或者[总包号].";
                return result;
            }

            try
            {
                if (_mailTotalPackageInfoRepository.SaveMainPostBagTag(m, re))
                {
                    result.Status = true;
                    return result;
                }
            }
            catch (BusinessLogicException ex)
            {
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            result.Status = false;

            if (string.IsNullOrWhiteSpace(result.Message))
            {
                result.Message = "服务器内部错误.";
            }

            return result;
        }

        private string GetMainPackageNumber(string mainPostNumber)
        {
            try
            {
                //^[A-Z]{6}-[A-Z]{6}-[A-Z]{1}-[A-Z]{2}-[0-9]{1}-([0-9]{4})-[0-9]{3}-[0-9]{1}-[0-9]{1}-[0-9]{4}$
                Regex re =
                    new Regex(@"^[A-Z]{6}[A-Z]{6}[A-Z]{1}[A-Z]{2}[0-9]{1}([0-9]{4})[0-9]{3}[0-9]{1}[0-9]{1}[0-9]{4}$",
                              RegexOptions.None);
                MatchCollection mc = re.Matches(mainPostNumber);
                return mc[0].Groups[1].Value;
            }
            catch (Exception ex)
            {
                Log.Error(mainPostNumber + "" + ex);
            }
            return null;
        }

        public ResultInfo IsValidFuPostBagNumber(string fuPostNumber)
        {
            return _mailPostBagInfoRepository.IsValidFuPostBagNumber(fuPostNumber);
        }

        public void AddMailReturnGoodsLogs(List<ReturnGoodsModel> returnGoodsModels)
        {
            returnGoodsModels.ForEach(r =>
                {
                    CanAddMailReturnGoodsLogs(r.TrackNumber, r.ReasonType);

                    string postBagNumber = _mailPostBagInfoRepository.GetPostBagNumber(r.TrackNumber);

                    _mailHoldLogRepository.Remove(h => h.TrackingNumber == r.TrackNumber);

                    _mailReturnGoodsLogRepository.Add(new MailReturnGoodsLog()
                        {
                            TrackNumber = r.TrackNumber,
                            ReasonType = r.ReasonType,
                            ReturnBy = _workContext.User.UserUame,
                            ReturnOn = DateTime.Now,
                            CreatedOn = DateTime.Now,
                            CreatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = _workContext.User.UserUame,
                            PostBagNumber = postBagNumber,
                        });
                });


            using (
                var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                _mailHoldLogRepository.UnitOfWork.Commit();
                _mailReturnGoodsLogRepository.UnitOfWork.Commit();

                transaction.Complete();
            }

        }

        public void CanAddMailReturnGoodsLogs(string trackNumber, int reasonType)
        {
            if (!_wayBillInfoRepository.Exists(p => p.TrackingNumber == trackNumber)) throw new Exception("该单不存在");
            if (_mailReturnGoodsLogRepository.Exists(p => p.TrackNumber == trackNumber)) throw new Exception("该单已是退件");
            //string postBagNumber = _mailPostBagInfoRepository.GetPostBagNumber(trackNumber);
            //if (postBagNumber == null) throw new Exception("该单不为国际小包优+");
        }

        public IPagedList<MailReturnGoodsLogsExt> GetMailReturnGoodsLogsList(MailReturnGoodsLogsParam param)
        {
            return _mailReturnGoodsLogRepository.GetMailReturnGoodsLogsList(param);
        }

        public IPagedList<MailExchangeBagLogsExt> GetMailExchangeBagLogsList(MailExchangeBagLogsParam param)
        {
            return _mailExchangeBagLogRepository.GetMailExchangeBagLogsList(param);
        }


        public IPagedList<MailHoldLogsExt> GetMailHoldLogsList(MailHoldLogsParam param)
        {
            return _mailPostBagInfoRepository.GetMailHoldLogsList(param);
        }

        public void AddMailHoldLogs(string[] trackNumbers)
        {

            trackNumbers.ToList().ForEach(p =>
                {
                    var wayBill = _wayBillInfoRepository.GetFiltered(w => w.TrackingNumber == p).First();

                    if (wayBill.Status != (int) WayBill.StatusEnum.Send &&
                        wayBill.Status == (int) WayBill.StatusEnum.WaitOrder)
                    {
                        throw new Exception(string.Format("单:{0}不为已发货或待转单状态",p));
                    }

                    _mailHoldLogRepository.Add(new MailHoldLog()
                        {
                            TrackingNumber = p,
                            CreatedBy = _workContext.User.UserUame,
                            HoldBy = _workContext.User.UserUame,
                            LastUpdatedBy = _workContext.User.UserUame,
                            HoldOn = DateTime.Now,
                            CreatedOn = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                        });

                    _wayBillEventLogRepository.Add(new WayBillEventLog()
                        {
                            WayBillNumber = wayBill.WayBillNumber,
                            EventCode = 636,
                            EventDate = DateTime.Now,
                            Description = "拦截",
                            Operator = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            Remarks = string.Format("包裹异常处理中")
                        });

                });

            using (
                var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                _mailHoldLogRepository.UnitOfWork.Commit();
                _wayBillEventLogRepository.UnitOfWork.Commit();

                transaction.Complete();
            }
        }
    }

}

