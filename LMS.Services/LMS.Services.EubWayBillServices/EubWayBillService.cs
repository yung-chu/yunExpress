using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Security;
using Amib.Threading;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.EubWayBillServices
{
    public class EubWayBillService:IEubWayBillService
    {

        private readonly IEubWayBillApplicationInfoRepository _wayBillApplicationInfoRepository;
        private readonly IEubAccountInfoRepository _eubAccountInfoRepository;

        public EubWayBillService(IEubWayBillApplicationInfoRepository wayBillApplicationInfoRepository,IEubAccountInfoRepository eubAccountInfoRepository)
        {
            _wayBillApplicationInfoRepository = wayBillApplicationInfoRepository;
            _eubAccountInfoRepository = eubAccountInfoRepository;
        }
        public EubAccountInfo GetEubAccountInfo(int shippingMethodId)
        {  
            return _eubAccountInfoRepository.Single(p => p.ShippingMethodId == shippingMethodId&&p.Status==1);
        }

        public EubWayBillApplicationInfo GetEubWayBillApplicationInfo(int id)
        {
            return _wayBillApplicationInfoRepository.Get(id);
        }

       public EubWayBillApplicationInfo GetEubWayBillApplicationInfo(string wayBillNumber)
       {
           return _wayBillApplicationInfoRepository.GetFiltered(p => p.WayBillNumber == wayBillNumber).FirstOrDefault();
       }

        /// <summary>
        /// 获取已申请的运单
        /// </summary>
        /// <returns></returns>
        public IList<EubWayBillApplicationInfo> GetApplyList()
        {
            return _wayBillApplicationInfoRepository.GetList(p => p.Status == (int)EubWayBillApplicationInfo.StatusEnum.Apply).ToList();
        }

        public void UpdateEubWayBillInfo(EubWayBillApplicationInfo eubWay)
        {
            eubWay.LastUpdatedOn = DateTime.Now;
            eubWay.LastUpdatedBy = "admin";
            IEubWayBillApplicationInfoRepository wayBillApplicationInfoRepository = new EubWayBillApplicationInfoRepository(new LMS_DbContext());
            wayBillApplicationInfoRepository.Modify(eubWay);
            wayBillApplicationInfoRepository.UnitOfWork.Commit();
        }


        public void StaticLabelDowLoad()
        {
            var list = GetApplyList().ToList();

            if (list.Count > 0)
            {
                ////多线程下载
                //var threadPool = new SmartThreadPool {MaxThreads = 10};

                //list.ForEach(p => threadPool.QueueWorkItem(EubLabelDowLoad,p));

                //threadPool.WaitForIdle();

                list.ForEach(EubLabelDowLoad);
            }

        }

        private void EubLabelDowLoad(object value)
        {
            EubWayBillApplicationInfo item = value as EubWayBillApplicationInfo;

            if (item == null)
            {
                Log.Info(string.Format("下载EUB标签错误！"));
                return;
            }

            WebClient client = new WebClient();
            string trackNumber = item.WayBillInfo.TrackingNumber;
            if (GetEubAccountInfo(item.ShippingMethodID) == null)
            {

                Log.Info(string.Format("运输方式ID为{0}没有配置Eub帐户信息！", item.ShippingMethodID));
                return;
            }
            try
            {
                string authenticate = GetEubAccountInfo(item.ShippingMethodID).AuthorizationCode;
                string md5str = GetMD5(authenticate + trackNumber).ToLower();
                string url = string.Format("{0}/partner/api/public/p/static/label/download/{1}/{2}.pdf ", sysConfig.LabelDowLoadPath, md5str, trackNumber);
                string fileName = sysConfig.PdfTemplatePath + item.WayBillInfo.WayBillNumber + ".pdf";
                if (!Directory.Exists(sysConfig.PdfTemplatePath))
                    Directory.CreateDirectory(sysConfig.PdfTemplatePath);
                string localUrl = sysConfig.PdfTemplateWebPath + item.WayBillInfo.WayBillNumber + ".pdf";
                Log.Info("Eub下载地址：" + url);
                client.DownloadFile(url, fileName);
                client.Dispose();
                item.Status = EubWayBillApplicationInfo.StatusToValue(EubWayBillApplicationInfo.StatusEnum.DownLoad);
                item.EubDownLoad = url;
                item.LocalDownLoad = localUrl;
                UpdateEubWayBillInfo(item);
                
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private string GetMD5(string str)
        {
            string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            return md5;
        } 
    }
}
