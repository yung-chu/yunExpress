using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Services.TrackServices;

namespace LMS.FrontDesk.Controllers.FubController
{
    public class HomeController : BaseController
    {
        private readonly ITrackingService _trackingService;

        public HomeController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }


        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Index(string Number)
        {
            return View(DataBind(Number));
        }


        public ActionResult Index2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index2(string number)
        {
            return View(DataBind(number));
        }




        public InTrackingLogInfoModel DataBind(string Number)
        {

            var model = new InTrackingLogInfoModel();
            //所有单号
            var allNumberList = new List<string>();
            //能查的单号
            var newNumberList = new List<string>();
            //不能查的单号
            var noQueryNumber = new List<string>();

            if (!string.IsNullOrEmpty(Number))
            {
                if (Number.Contains("/t"))
                {
                    Number = Number.Replace("/t", "");
                }

                var numberList = Number.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                if (numberList.Count() > 50) //超过50个取50个
                {
                    numberList = numberList.Take(50).ToList();
                }


                numberList.ForEach(p =>
                {
                    p = p.Trim();

                    allNumberList.Add(p);
                    //单号长度11位，591开头
                    if (p.Length != 11 || !p.StartsWith("591"))
                    {
                        return;
                    }

                    if (!newNumberList.Contains(p))
                    {
                        newNumberList.Add(p);
                    }

                });

            }



            //查询数据
            if (!string.IsNullOrEmpty(Number) && newNumberList.Any())
            {
                //查询到的单号
                var getNumberList = new List<string>();

                //查询到的数据
                var getSearchNumber = _trackingService.GetInTrackingLogInfoList(newNumberList);
                getSearchNumber.ForEach(a =>
                {
                    if (!getNumberList.Contains(a.TrackingNumber))
                    {
                        getNumberList.Add(a.TrackingNumber);
                    }
                });

                //获取查询不到的单号
                noQueryNumber = allNumberList.Except(getNumberList).ToList();

                model.NoQueryNumber = string.Join(",", noQueryNumber);
                model.ListModel = getSearchNumber;
            }

            return model;
        }



        public JsonResult ReturnModel(string number)
        {
            var result = new ResposeInfo() { Result = false };

            //请求数据
            var getModel = DataBind(number);


            var sb = new StringBuilder();

            #region 返回结果
            if (getModel != null && getModel.ListModel != null)
            {
                //按跟踪号分组
                foreach (var item in getModel.ListModel.ToLookup(p => p.TrackingNumber))
                {
                    //跟踪号
                    var getTrackingNumber = item.Key;

                    sb.Append("<table class='Tb mt30'>");
                    sb.Append("<tr>");
                    sb.Append("<th colspan='3' style='background:#006fbd; color:#fff; text-align:left; padding-left:20px;'>");
                    sb.AppendFormat("单号:{0}", getTrackingNumber + "（<span style='font-size:18px;font-weight:bold'>该邮件属于平常邮件,不接受查询,只提供邮件所在的总包发运信息如下:</span>）");
                    sb.Append("</th>");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    sb.Append("<th>");
                    sb.Append("日期");
                    sb.Append("</th>");

                    sb.Append("<th>");
                    sb.Append("动态");
                    sb.Append("</th>");

                    sb.Append("<th>");
                    sb.Append("备注");
                    sb.Append("</th>");
                    sb.Append("</tr>");


                    foreach (var item1 in item.OrderBy(p=>p.ProcessDate))
                    {
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>");
                        sb.AppendFormat("{0}", item1.ProcessDate.Value.ToString("yyyy-MM-dd"));
                        sb.Append("</td>");


                        sb.Append("<td align='center'>");
                        var getProcessContent = item1.ProcessContent.Contains("\r\n") ? item1.ProcessContent.Replace("\r\n", "<br/>") : item1.ProcessContent;
                        sb.AppendFormat("{0}", getProcessContent);
                        sb.Append(" </td>");

                        sb.Append("<td align='center'>");
                        sb.AppendFormat("{0}", item1.Remarks);
                        sb.Append(" </td>");
                        sb.Append("</tr>");
                    }

                    sb.Append("</table>");

                }
            }
            #endregion


            result.TrackData = sb.ToString();//返回数据
            result.NumberData = number;//返回查询单号
            result.NoQueryNumber = getModel != null ? getModel.NoQueryNumber : "";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class ResposeInfo
        {
            public bool Result { get; set; }
            public int ErrorInfo { get; set; }
            public string TrackData { get; set; }
            public string NumberData { get; set; }
            public string NoQueryNumber { get; set; }
        }

    }
}
