using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.TrackingNumberServices
{
    public class TrackingNumberService : ServiceBase, ITrackingNumberService
    {
        private readonly ITrackingNumberInfoRepository _trackingNumberInfoRepository;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly ITrackingNumberDetailInfoRepository _trackingNumberDetailInfoRepository;
        private readonly IWorkContext _workContext;
        private readonly ICountryRepository _countryRepository;
        private readonly IShippingInfoRepository _shippingInfoRepository;

        public TrackingNumberService(ITrackingNumberInfoRepository trackingNumberInfoRepository,
                                     ITrackingNumberDetailInfoRepository trackingNumberDetailInfoRepository,
                                     IWayBillInfoRepository wayBillInfoRepository,
                                     IWorkContext workContext,
                                     ICountryRepository countryRepository, IShippingInfoRepository shippingInfoRepository)
        {
            _trackingNumberInfoRepository = trackingNumberInfoRepository;
            _trackingNumberDetailInfoRepository = trackingNumberDetailInfoRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _workContext = workContext;
            _countryRepository = countryRepository;
            _shippingInfoRepository = shippingInfoRepository;
            AddDisposableObject(trackingNumberInfoRepository, trackingNumberDetailInfoRepository, wayBillInfoRepository, workContext);

        }

        public void AddTrackingNumber(TrackingNumberInfo trackingNumberInfo)
        {
            _trackingNumberInfoRepository.Add(trackingNumberInfo);
            _trackingNumberInfoRepository.UnitOfWork.Commit();
        }
        public void UpdateTrackingNumber(TrackingNumberInfo trackingNumberInfo)
        {
            throw new NotImplementedException();
        }

        public bool DisableTrackingNumber(string id)
        {
            TrackingNumberInfo model = new TrackingNumberInfo();
            List<TrackingNumberDetailInfo> trackingNumberDetailInfo = new List<TrackingNumberDetailInfo>();
            trackingNumberDetailInfo = _trackingNumberDetailInfoRepository.GetList(p => p.TrackingNumberID == id);
            model = _trackingNumberInfoRepository.Get(id);
            if (model != null)
            {
                if (trackingNumberDetailInfo.Count > 0)
                {
                    foreach (var row in trackingNumberDetailInfo)
                    {
                        if (row.Status == 1)
                        {
                            row.Status = 3;
                            _trackingNumberDetailInfoRepository.Modify(row);
                        }
                    }
                }
                model.Status = 2;
                _trackingNumberInfoRepository.Modify(model);
                _trackingNumberInfoRepository.UnitOfWork.Commit();
                _trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                return true;
            }
            return false;
        }

        public bool UpdateTrackingNumberInfoDetailAndWayBillInfo(WayBillInfo wayBillInfo)
        {
            bool bResult = false;
            if (wayBillInfo == null)
                return bResult;

            if (wayBillInfo.InShippingMethodID.HasValue)
            {
                var trackingNumberDetail = TrackNumberAssignStandard(wayBillInfo.InShippingMethodID.Value, 1, wayBillInfo.CountryCode);

                //_trackingNumberDetailInfoRepository.GetTrackingNumberDetailInfo(wayBillInfo.InShippingMethodID.Value, wayBillInfo.CountryCode);
                if (null != trackingNumberDetail && trackingNumberDetail.Any())
                {
                    //trackingNumberDetail.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
                    //trackingNumberDetail.WayBillNumber = wayBillInfo.WayBillNumber;
                    // _trackingNumberDetailInfoRepository.Modify(trackingNumberDetail);

                    wayBillInfo.TrackingNumber = trackingNumberDetail[0];
                    wayBillInfo.LastUpdatedOn = DateTime.Now;
                    wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
                    wayBillInfo.LastUpdatedOn = DateTime.Now;

                    using (var transaction = new TransactionScope())
                    {
                        _wayBillInfoRepository.Modify(wayBillInfo);

                        //_trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                        _wayBillInfoRepository.UnitOfWork.Commit();
                        transaction.Complete();
                        bResult = true;
                    }

                }
            }

            return bResult;
        }

        /// <summary>
        /// 验证是否重复跟踪号,采用分批验证机制
        /// </summary>
        /// <param name="trackNumbers"></param>
        /// <returns></returns>
        private List<string> GetRepeatedTrackNumbers(List<TrackingNumberDetailInfo> trackNumbers)
        {
            //计算分批大小
            var avgLength = Math.Ceiling((double)(trackNumbers.Sum(t => t.TrackingNumber.Length) / trackNumbers.Count));
            int batchSize =(int)(10000 / (avgLength + 3));// 3表示两个单引号和一个逗号的长度,eg.'asdasd', 

            List<string> existTrackNumbers = new List<string>();

            //验证
            int index = 0;
            do
            {
                var current = trackNumbers.Skip(index * batchSize).Take(batchSize).Select(t => t.TrackingNumber);
                existTrackNumbers.AddRange(CheckRepeatedTrackNumberFormDb(current));

                index++;
            }
            while (trackNumbers.Count > index * batchSize);//总数比当前页多就取下一页

            return existTrackNumbers;
        }

        private List<string> CheckRepeatedTrackNumberFormDb(IEnumerable<string> trackNumbers)
        {           
            return _trackingNumberDetailInfoRepository.CheckRepeatedTrackNumbers(trackNumbers);
        }


        /// <summary>
        /// 上传跟踪号
        /// Add by zhengsong
        /// 修复不同运输方式可能重复的问题 ,
        /// 对跟踪号分批进行全表对比 ,
        /// 同时加入并行运算提升性能
        /// by daniel 2014-11-1
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TrackingNumberDetailInfo> UploadTrackingNumberList(TrackingNumberInfo model)
        {
            Log.Info("跟踪号"+model.TrackingNumberDetailInfos.Count);
            TrackingNumberInfo tn = new TrackingNumberInfo();
            ConcurrentBag<TrackingNumberDetailInfo> errors = new ConcurrentBag<TrackingNumberDetailInfo>();

           
            Log.Info("开始查找重复的跟踪号...");
            var duplicateTrackNumbers = GetRepeatedTrackNumbers(model.TrackingNumberDetailInfos.ToList());
            Log.Info("查找重复的跟踪号OK.");
            Parallel.ForEach(model.TrackingNumberDetailInfos, t =>
            //foreach (var t in model.TrackingNumberDetailInfos)
            {
                if (duplicateTrackNumbers.Contains(t.TrackingNumber))
                {
                    errors.Add(t);
                }
            });
          
            if (errors.Count < 1)
            {
                tn.TrackingNumberID = model.TrackingNumberID;
                tn.ShippingMethodID = model.ShippingMethodID;
                tn.ApplianceCountry = model.ApplianceCountry;
                tn.Status = model.Status;
                tn.CreatedBy = model.CreatedBy;
                tn.CreatedNo = model.CreatedNo;
                tn.LastUpdateOn = model.LastUpdateOn;
                tn.LastUpdatedBy = model.LastUpdatedBy;
                tn.ApplianceCountry = model.ApplianceCountry;
               
                ConcurrentBag<TrackingNumberDetailInfo> dataList = new ConcurrentBag<TrackingNumberDetailInfo>();

                //排除有问题的跟踪号
                var good = model.TrackingNumberDetailInfos.Except(errors);

                Log.Info("准备插入的数据..."+good.Count());
                //插入的数据
                Parallel.ForEach(good, row =>
                {
                    TrackingNumberDetailInfo detail = new TrackingNumberDetailInfo();
                    detail.TrackingNumber = row.TrackingNumber.ToUpperInvariant();
                    detail.TrackingNumberID = model.TrackingNumberID;
                    detail.Status = (short)TrackingNumberDetailInfo.StatusEnum.NotUsed;
                    detail.WayBillNumber = "";
                    dataList.Add(detail);
                });
                Log.Info("准备的数据OK." + dataList.Count);

                if (dataList.Count > 0)
                {
                    Log.Info("开始保存数据...");
                    using (var trans = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 10, 0)))
                    {
                        AddTrackingNumber(tn);
                        _wayBillInfoRepository.BulkInsert("TrackingNumberDetailInfos", dataList.ToList());
                        trans.Complete();
                    }
                    Log.Info("保存数据完成.");
                }

                return dataList.ToList();
            }
            else
            {
                return errors.ToList();
            }
        }

        public List<TrackingNumberInfo> GetTrackingNumbers(List<int> shippingMethodIds)
        {
            return _trackingNumberInfoRepository.GetList(p => shippingMethodIds.Contains(p.ShippingMethodID) && p.Status == (short)TrackingNumberInfo.StatusEnum.Enable);
        }

        public IPagedList<CountryExt> GetPagedList(CountryParam param = null)
        {
            param = param ?? new CountryParam();
            return _countryRepository.GetPagedList(param);
        }

        public TrackingNumberInfo GetTrackingNumberInfo(string id)
        {
            return _trackingNumberInfoRepository.Get(id);
        }

        public List<TrackingNumberDetailInfo> GetTrackingNumberDetailById(string id)
        {
            return _trackingNumberDetailInfoRepository.GetList(p => p.TrackingNumberID == id);
        }


        public IPagedList<TrackingNumberDetailInfo> GetTrackingNumberPagedList(int page, int pageSize, string trackingNumberID)
        {
            Expression<Func<TrackingNumberDetailInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.TrackingNumberID == trackingNumberID, !string.IsNullOrWhiteSpace(trackingNumberID));
            Func<IQueryable<TrackingNumberDetailInfo>, IOrderedQueryable<TrackingNumberDetailInfo>>
            orderBy = o => o.OrderByDescending(p => p.WayBillNumber);
            return _trackingNumberDetailInfoRepository.FindPagedList(page, pageSize, filter, orderBy);
        }


        //{被废弃 , 获取跟踪号请使用标准接口GetListByShippingMethodId , by daniel , 2014-10-23}
        //public TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode,
        //                                                            List<int> detailIds)
        //{
        //    return _trackingNumberDetailInfoRepository.GetTrackingNumberDetailInfo(shippingMethodId, countryCode,
        //                                                                           detailIds);
        //}



        /// <summary>
        /// 分配跟踪号[标准接口]
        /// 2014-10-29 by daniel
        /// </summary>
        /// <param name="shippingMethodId">运输方式Id</param>
        /// <param name="count">数据量</param>
        /// <param name="countryCode">国家代码</param>
        /// <returns></returns>
        public IList<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode)
        {
            using (RefAPI.TrackNumber.TrackNumberAssignClient client = new RefAPI.TrackNumber.TrackNumberAssignClient())
            {
                var re = client.TrackNumberAssignStandard(shippingMethodId, count, countryCode);

                if (re.IsSuccess)
                {
                    return re.Data.ToList();
                }
                else
                {
                    Log.Error(re.ErrorMessage);
                    return null;
                    //throw new BusinessLogicException(re.ErrorMessage);
                }
            }
        }

        //{无人使用被废弃,by daniel , 2014-10-23}
        //public List<TrackingNumberDetailInfo> GetTrackingNumberDetailList()
        //{
        //    return
        //        _trackingNumberDetailInfoRepository.GetList(
        //            p => p.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed);
        //}



        public ShippingInfo GetShippingInfo(string waybillnumber)
        {
            var wayBillInfo = _wayBillInfoRepository.Get(waybillnumber);

            if (wayBillInfo == null) return null;

            return _shippingInfoRepository.Get(wayBillInfo.ShippingInfoID);

        }

    }
}
