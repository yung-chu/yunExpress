using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using System.Xml.Serialization;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Express.DHL.Request;
using LMS.Data.Express.DHL.Response.Error;
using LMS.Data.Express.DHL.Response;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;

namespace LMS.Services.ExpressServices
{
    public class ExpressService : IExpressService
    {
        private readonly IExpressAccountInfoRepository _expressAccountInfoRepository;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IExpressResponsRepository _expressResponsRepository;
        private readonly IWorkContext _workContext;
        private readonly INetherlandsParcelResponsRepository _netherlandsParcelResponsRepository;
        private readonly IDeliveryChannelChangeLogRepository _deliveryChannelChangeLogRepository;

        public ExpressService(IExpressAccountInfoRepository expressAccountInfoRepository, 
            IExpressResponsRepository expressResponsRepository,
            IWorkContext workContext,
            IWayBillInfoRepository wayBillInfoRepository,
            INetherlandsParcelResponsRepository netherlandsParcelResponsRepository,
            IDeliveryChannelChangeLogRepository deliveryChannelChangeLogRepository)
        {
            _expressAccountInfoRepository = expressAccountInfoRepository;
            _expressResponsRepository = expressResponsRepository;
            _workContext = workContext;
            _wayBillInfoRepository = wayBillInfoRepository;
            _netherlandsParcelResponsRepository = netherlandsParcelResponsRepository;
            _deliveryChannelChangeLogRepository = deliveryChannelChangeLogRepository;
        }


        public IEnumerable<ExpressAccountInfo> GetExpressAccountInfos()
        {
            return _expressAccountInfoRepository.GetAll();
        }

        public bool IsExistNLPOST(string wayBillNumber)
        {
            return _netherlandsParcelResponsRepository.Exists(p => p.WayBillNumber == wayBillNumber);
        }

        public string GetNetherlandsParcelSfNumber(string wayBillNumber)
        {
            var model = _netherlandsParcelResponsRepository.Single(p => p.WayBillNumber == wayBillNumber);
            if (model != null)
            {
                return model.MailNo;
            }
            return "";
        }

        public void AddNLPOST(NetherlandsParcelRespons netherlandsParcelRespons, WayBillInfo wayBillInfo)
        {
            if (netherlandsParcelRespons == null)
            {
                throw new BusinessLogicException("数据不能为空！");
            }
            var result = _netherlandsParcelResponsRepository.Exists(p => p.WayBillNumber == netherlandsParcelRespons.WayBillNumber);
            if (result)
                throw new BusinessLogicException("该数据已存在！");
            netherlandsParcelRespons.LastUpdatedBy = _workContext.User.UserUame;
            netherlandsParcelRespons.LastUpdatedOn = DateTime.Now;
            netherlandsParcelRespons.CreatedBy = _workContext.User.UserUame;
            netherlandsParcelRespons.CreatedOn = DateTime.Now;
            _netherlandsParcelResponsRepository.Add(netherlandsParcelRespons);

            wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
            wayBillInfo.LastUpdatedOn = DateTime.Now;
            wayBillInfo.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
            wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
            _wayBillInfoRepository.Modify(wayBillInfo);

            using (var tran = new TransactionScope())
            {
                _netherlandsParcelResponsRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                AddDeliveryChannelChangeLog(wayBillInfo.WayBillNumber, wayBillInfo.OutShippingMethodID.Value, wayBillInfo.VenderCode);
                tran.Complete();
            }
        }

        public void UpdateNLPOST(NetherlandsParcelRespons netherlandsParcelRespons)
        {
            if (netherlandsParcelRespons == null)
            {
                throw new BusinessLogicException("数据不能为空！");
            }
            var result = _netherlandsParcelResponsRepository.Single(p => p.WayBillNumber == netherlandsParcelRespons.WayBillNumber);
            if (result==null)
                throw new BusinessLogicException("该数据不存在！");
            result.LastUpdatedBy = _workContext.User.UserUame;
            result.LastUpdatedOn = DateTime.Now;
            result.Status = netherlandsParcelRespons.Status;
            _netherlandsParcelResponsRepository.Modify(result);
            _netherlandsParcelResponsRepository.UnitOfWork.Commit();
        }

        /// <summary>
        /// 是否存在运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public bool IsExistExpressResponse(string wayBillNumber)
        {
            return _expressResponsRepository.Exists(p => p.WayBillNumber==wayBillNumber);
        }
        /// <summary>
        /// 取消荷兰小包
        /// </summary>
        /// <param name="wayBillNumber"></param>
        public void DeleteNLPOST(string wayBillNumber)
        {
            _netherlandsParcelResponsRepository.Remove(p=>p.WayBillNumber==wayBillNumber);
        }

        public List<AgentNumberInfo> GetAgentNumbers(List<string> orderIds, string customerCode)
        {
            return _netherlandsParcelResponsRepository.GetAgentNumbers(orderIds, customerCode);
        }

        /// <summary>
        /// Post请求DHL接口
        /// </summary>
        /// <param name="ap">ShipmentValidateRequestAP实例</param>
        /// <param name="serverUrl">请求DHL接口的服务地址</param>
        /// <returns></returns>
        public ShipmentValidateResponse PostDHLShipment(ShipmentValidateRequestAP ap,string serverUrl)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("req", "http://www.dhl.com");
            var responseResult = HttpHelper.PostSendRequest(ap, serverUrl, ns);
            try
            {
                var response = SerializeUtil.DeserializeFromXml<ShipmentValidateResponse>(responseResult);
                return response;
            }
            catch (InvalidOperationException ex)
            {
                try
                {
                    var result = SerializeUtil.DeserializeFromXml<ShipmentValidateErrorResponse>(responseResult);
                    if (result != null)
                    {
                        throw new BusinessLogicException(result.Response.Status.Condition[0].ConditionData);
                    }
                }
                catch (InvalidOperationException ioex)
                {
                    var errorResult = SerializeUtil.DeserializeFromXml<LMS.Data.Express.DHL.Response.ErrorResponse>(responseResult);
                    if (errorResult != null)
                    {
                        throw new BusinessLogicException(errorResult.Response.Status.Condition[0].ConditionData);
                    }
                }
            }
            return null;
        }


        public ExpressAccountInfo  GetExpressAccountInfo(string venderCode,int shippingMethodId)
        {
            return _expressAccountInfoRepository.GetAll().FirstOrDefault(p=>p.VenderCode==venderCode&&p.ShippingMethodId==shippingMethodId);
        }


        public ExpressRespons GetExpressResponse(string wayBillNumber)
        {
            return !string.IsNullOrWhiteSpace(wayBillNumber) ? _expressResponsRepository.GetList(p => p.WayBillNumber == wayBillNumber).FirstOrDefault() : null;
        }

        /// <summary>
        /// 添加快递服务响应的数据，并更新运单的物流商、发货运输方式ID和跟踪号
        /// </summary>
        /// <param name="expressResponse">快递响应实例</param>
        /// <param name="wayBillInfo">运单实例</param>
        public void AddExpressResponse(ExpressRespons expressResponse, WayBillInfo wayBillInfo,bool isNologin=false)
        {
            if (null == expressResponse)
            {
                throw new BusinessLogicException("数据不能为空！");
            }
            var result = _expressResponsRepository.Exists(p => p.WayBillNumber == expressResponse.WayBillNumber);
            if (result)
                throw new BusinessLogicException("该数据已存在！");
            var wayBill = _wayBillInfoRepository.Get(wayBillInfo.WayBillNumber);
            expressResponse.LastUpdatedBy = isNologin ? "System" : _workContext.User.UserUame;
            expressResponse.LastUpdatedOn = DateTime.Now;
            expressResponse.CreatedBy = isNologin ? "System" : _workContext.User.UserUame;
            expressResponse.CreatedOn = DateTime.Now;
            _expressResponsRepository.Add(expressResponse);
            wayBill.VenderCode = wayBillInfo.VenderCode;
            wayBill.OutShippingMethodID = wayBillInfo.InShippingMethodID;
            wayBill.TrackingNumber = wayBillInfo.TrackingNumber;
            wayBill.CustomerOrderInfo.TrackingNumber = wayBillInfo.TrackingNumber;
            wayBill.LastUpdatedBy = isNologin ? "System" : _workContext.User.UserUame;
            wayBill.LastUpdatedOn = DateTime.Now;
            wayBill.CustomerOrderInfo.LastUpdatedBy = isNologin ? "System" : _workContext.User.UserUame;
            wayBill.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
            _wayBillInfoRepository.Modify(wayBill);
           
            using (var tran = new TransactionScope())
            {
                _expressResponsRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                AddDeliveryChannelChangeLog(wayBillInfo.WayBillNumber, wayBillInfo.OutShippingMethodID.Value, wayBillInfo.VenderCode,isNologin);
                tran.Complete();
            }
        }

        //DHL Api 预报信息
        //Add By zhengsong
        public ExpressRespons AddExpressResponseToAPI(ExpressRespons expressResponse, WayBillInfo wayBillInfo)
        {
            if (null == expressResponse)
            {
                throw new BusinessLogicException("数据不能为空！");
            }
            var result = _expressResponsRepository.Exists(p => p.WayBillNumber == expressResponse.WayBillNumber);
            if (result)
                throw new BusinessLogicException("该数据已存在！");

            expressResponse.LastUpdatedBy = wayBillInfo.CreatedBy;
            expressResponse.LastUpdatedOn = DateTime.Now;
            expressResponse.CreatedBy = wayBillInfo.CreatedBy;
            expressResponse.CreatedOn = DateTime.Now;
            //_expressResponsRepository.Add(expressResponse);

            //wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
            //wayBillInfo.LastUpdatedOn = DateTime.Now;
            //wayBillInfo.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
            //wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
            //_wayBillInfoRepository.Modify(wayBillInfo);
            //using (var tran = new TransactionScope())
            //{
            //    //_expressResponsRepository.UnitOfWork.Commit();
            //    //_wayBillInfoRepository.UnitOfWork.Commit();
            //    AddDeliveryChannelChangeLog(wayBillInfo.WayBillNumber, wayBillInfo.OutShippingMethodID.Value, wayBillInfo.VenderCode, wayBillInfo.CreatedBy);
            //    tran.Complete();
            //}
            return expressResponse;
        }

        //添加 DHL ExpressResponse表信息
        //Add By zhengsong
        public void AddExpressResponseToDHL(List<ExpressRespons> expressResponses)
        {
            try
            {
                expressResponses.ForEach(p => _expressResponsRepository.Add(p));
                _expressResponsRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //添加DHL DeliveryChannelChangeLog日志
        //Add By zhengsong
        public void AddApiDHLDeliveryChannelChangeLog(List<WayBillInfo> wayBillInfos)
        {
            try
            {
                wayBillInfos.ForEach(p =>
                {
                    DeliveryChannelChangeLog deliveryChannelChangeLog = new DeliveryChannelChangeLog();
                    deliveryChannelChangeLog.WayBillNumber = p.WayBillNumber;
                    deliveryChannelChangeLog.ShippingMethodId = p.InShippingMethodID.Value;
                    deliveryChannelChangeLog.VenderCode = p.VenderCode;
                    deliveryChannelChangeLog.CreatedBy = p.CreatedBy;
                    deliveryChannelChangeLog.LastUpdatedBy = p.CreatedBy;
                    deliveryChannelChangeLog.LastUpdatedOn = DateTime.Now;
                    deliveryChannelChangeLog.CreatedOn = DateTime.Now;
                    _deliveryChannelChangeLogRepository.Add(deliveryChannelChangeLog);
                });
                _deliveryChannelChangeLogRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public void AddDeliveryChannelChangeLog(string wayBillNumber, int shippingMethodId, string venderCode,bool isNologin=false)
       {
           DeliveryChannelChangeLog deliveryChannelChangeLog =new DeliveryChannelChangeLog();
           deliveryChannelChangeLog.WayBillNumber = wayBillNumber;
           deliveryChannelChangeLog.ShippingMethodId = shippingMethodId;
           deliveryChannelChangeLog.VenderCode = venderCode;
           deliveryChannelChangeLog.CreatedBy = isNologin ? "System" : _workContext.User.UserUame;
           deliveryChannelChangeLog.LastUpdatedBy = isNologin ? "System" : _workContext.User.UserUame;
           deliveryChannelChangeLog.LastUpdatedOn = DateTime.Now;
           deliveryChannelChangeLog.CreatedOn = DateTime.Now;
           _deliveryChannelChangeLogRepository.Add(deliveryChannelChangeLog);
           _deliveryChannelChangeLogRepository.UnitOfWork.Commit();
       }

        /// <summary>
        /// 更新快递服务响应的数据
        /// </summary>
        /// <param name="expressResponse"></param>
        public void UpdateExpressResponse(ExpressRespons expressResponse)
        {
            if (null == expressResponse)
            {
                throw new BusinessLogicException("数据不能为空！");

            }
            var responseEntity = _expressResponsRepository.GetList(p => p.WayBillNumber == expressResponse.WayBillNumber).FirstOrDefault();
            if(null==responseEntity)
                throw new BusinessLogicException("该数据不存在,更新失败！");
            expressResponse.LastUpdatedBy = _workContext.User.UserUame;
            expressResponse.LastUpdatedOn = DateTime.Now;
            expressResponse.CreatedBy = _workContext.User.UserUame;
            expressResponse.CreatedOn = DateTime.Now;

            _expressResponsRepository.Modify(expressResponse);
            _expressResponsRepository.UnitOfWork.Commit();
        }

        /// <summary>
        /// 添加快递服务响应的数据
        /// </summary>
        /// <param name="expressResponses"></param>
        public void AddExpressResponse(IEnumerable<ExpressRespons> expressResponses)
        {
            string errorWayBillNumber = string.Empty;
            var enumerable = expressResponses as ExpressRespons[] ?? expressResponses.ToArray();
            if (!enumerable.Any())
            {
                throw new BusinessLogicException("数据不能为空！");
            }
            foreach (var expressResponse in enumerable)
            {
                ExpressRespons response = expressResponse;
                var result = _expressResponsRepository.Exists(p => p.WayBillNumber == response.WayBillNumber);
                if (result) continue;
                expressResponse.LastUpdatedBy = _workContext.User.UserUame;
                expressResponse.LastUpdatedOn = DateTime.Now;
                expressResponse.CreatedBy = _workContext.User.UserUame;
                expressResponse.CreatedOn = DateTime.Now;
                try
                {
                    _expressResponsRepository.Add(expressResponse);
                    _expressResponsRepository.UnitOfWork.Commit();
                }
                catch (Exception)
                {
                    errorWayBillNumber = expressResponse + ",";
                }
            }
            if (!string.IsNullOrWhiteSpace(errorWayBillNumber))
            {
                throw new BusinessLogicException("提交失败的运单有：{0}！".FormatWith(errorWayBillNumber));
            }
        }

        public NetherlandsParcelRespons  GetNetherlandsParcelRespons(string wayBillNumber, int? status)
        {
              Expression<Func<NetherlandsParcelRespons, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Status == status, status.HasValue)
                           .AndIf(p =>p.WayBillNumber==wayBillNumber,!string.IsNullOrWhiteSpace(wayBillNumber));
           return _netherlandsParcelResponsRepository.GetFiltered(filter).FirstOrDefault();
        }
    }
}
