using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Security;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.EubWayBillServices;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.Eub
{
    public class Handler
    {
        private readonly IEubWayBillService _eubWayBillService;
        public Handler()
        {
            var dbContext = new LMS_DbContext();
            _eubWayBillService = new EubWayBillService(new EubWayBillApplicationInfoRepository(dbContext), new EubAccountInfoRepository(dbContext));
        }
        //public void StaticLabelDowLoad()
        //{
        //    var list = _eubWayBillService.GetApplyList().ToList();
        //    foreach (var item in list)
        //    {
        //        WebClient client = new WebClient();
        //        string trackNumber = item.WayBillInfo.TrackingNumber;
        //        if (_eubWayBillService.GetEubAccountInfo(item.ShippingMethodID) == null)
        //        {;
        //            Log.Info(string.Format("运输方式ID为{0}没有配置Eub帐户信息！", item.ShippingMethodID));
        //            continue;
        //        }
        //        string authenticate = _eubWayBillService.GetEubAccountInfo(item.ShippingMethodID).AuthorizationCode;
        //        string md5str = GetMD5(authenticate + trackNumber).ToLower();
        //        string url = string.Format("{0}/partner/api/public/p/static/label/download/{1}/{2}.pdf ", sysConfig.LabelDowLoadPath, md5str, trackNumber);
        //        string fileName = sysConfig.PdfTemplatePath + item.WayBillInfo.WayBillNumber + ".pdf";
        //        if (!Directory.Exists(sysConfig.PdfTemplatePath))
        //            Directory.CreateDirectory(sysConfig.PdfTemplatePath);
        //        string localUrl = sysConfig.PdfTemplateWebPath + item.WayBillInfo.WayBillNumber + ".pdf";
        //        try
        //        {
        //            client.DownloadFile(url, fileName);
        //            item.Status = EubWayBillApplicationInfo.StatusToValue(EubWayBillApplicationInfo.StatusEnum.DownLoad);
        //            item.EubDownLoad = url;
        //            item.LocalDownLoad = localUrl;
        //            _eubWayBillService.UpdateEubWayBillInfo(item);
        //            Log.Info("Eub下载地址：" + url);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Exception(ex);
        //        }
        //    }
          
        //}


        public void StaticLabelDowLoad()
        {
            _eubWayBillService.StaticLabelDowLoad();

        }

    
        public static void Start()
        {
            try
            {
                var Handler = new Handler();
                Console.WriteLine("*********标签开始下载*********");
                Handler.StaticLabelDowLoad();
                Console.WriteLine("*********标签下载完毕*********");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }




    }
}
