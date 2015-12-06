using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using LMS.Data.Entity;
using LMS.FrontDesk.Controllers.TrackController.Models;
using LMS.Services.FreightServices;
using LMS.Services.TrackServices;
using LMS.Services.TrackingNumberServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Common;

namespace LMS.FrontDesk.Controllers.TrackController
{
    public class TrackController : BaseController
    {

        private readonly ITrackingService _trackingService;
        private readonly IFreightService _iFreightService;
        private readonly ITrackingNumberService _trackingNumberService;



        public TrackController(ITrackingService trackingService, IFreightService iFreightService, ITrackingNumberService iTrackingNumberService)
        {
            _trackingService = trackingService;
            _iFreightService = iFreightService;
            _trackingNumberService = iTrackingNumberService;
        }


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Detail(FilterModel param)
        {
            return View(SearchDataBind(param));
        }


        [HttpPost]
        [HttpGet]
        [ActionName("Detail")]
        public ActionResult SearchTrackingInfo(FilterModel param)
        {
            return View(SearchDataBind(param));
        }


        public TrackModel SearchDataBind(FilterModel param)
        {
            var model = new TrackModel();
            var wayBillNumberList = new List<string>();
            var noQueryWaybillnumbers = new List<string>();


            if (!string.IsNullOrEmpty(param.WayBillNumber))
            {



                wayBillNumberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                
                
                //获取该运单信息(运单号,跟踪号,订单号)
                List<WayBillInfo> listwayBillInfo = _trackingService.GetWayBillInfoList(wayBillNumberList);
                List<ContactTable> getTrackData = GetContactTables(listwayBillInfo);

                model.ListContactTable = getTrackData;

                //处理中--提交中,入仓中,已收货
                int submittedStatus= (int) WayBill.StatusEnum.Submitted;
                int inStoragingStatus = (int) WayBill.StatusEnum.InStoraging;
                int haveStatus = (int) WayBill.StatusEnum.Have;

                model.ListHandleing =
                    getTrackData.Where(
                        p =>
                            (p.WaybillStatus == submittedStatus || p.WaybillStatus == inStoragingStatus ||
                            p.WaybillStatus == haveStatus)&&!p.IsHold).ToList();

                //运输中
                model.ListSendPackage = getTrackData.Where(p => p.WaybillStatus == (int)WayBill.StatusEnum.Send && !p.IsHold).ToList();

                //成功签收
                model.ListSuccessSign = getTrackData.Where(p => p.WaybillStatus == (int)WayBill.StatusEnum.Delivered && !p.IsHold).ToList();

                //异常
                model.ListIsHold = getTrackData.Where(p => p.IsHold).ToList();

                //查询不到
                wayBillNumberList.ForEach(p =>
                {
                    if (_trackingService.IsGetWayBillInfo(p))
                    {
                        noQueryWaybillnumbers.Add(p);
                    }
                });
                getTrackData.ToList().ForEach(p =>
                {
                   if (!string.IsNullOrWhiteSpace(p.NoQueryWaybillnumber) )
                   {
                       noQueryWaybillnumbers.Add(p.NoQueryWaybillnumber);
                   }
                });
                model.NoQueryWaybillnumbers = noQueryWaybillnumbers;
            }
          

            return model;
        }

        public class FilterModel
        {
            public string WayBillNumber { get; set; }
        }


        //数据源
        public List<ContactTable> GetContactTables(List<WayBillInfo> waybillInfoList)
        {

            var listContactTable = new List<ContactTable>();
            var contactTable = new ContactTable();
            var enumStatus = new EnumStatus();


            foreach (var wayBillInfo in waybillInfoList)
            {

                ////判断运单状态,是否是退回,退货在仓
                //int returnStatus = WayBill.StatusToValue(WayBill.StatusEnum.Return);
                //int reGoodsInStorageStatus = WayBill.StatusToValue(WayBill.StatusEnum.ReGoodsInStorage);

                //if (wayBillInfo.Status == returnStatus || wayBillInfo.Status == reGoodsInStorageStatus)
                //{
                    
                //    contactTable.WaybillReurnStatus = true;
                //}


                #region 获取数据
                //获取外部数据
                OrderTrackingModel getOuterOrderTrackingModel = _iFreightService.GetOutTrackingInfo(wayBillInfo.TrueTrackingNumber) ??
                _iFreightService.GetOutTrackingInfo(wayBillInfo.TrackingNumber);


                //外部信息已签收--反写该运单已签收
                if (getOuterOrderTrackingModel != null && getOuterOrderTrackingModel.PackageState == (int)EnumStatus.PackageStateEnum.Delivered)
                {
                    _trackingService.UpdateWayBillInfo(wayBillInfo.WayBillNumber);
                }

                //去除外部轨迹抓取第一条
                if (getOuterOrderTrackingModel != null && getOuterOrderTrackingModel.OrderTrackingDetails.Any())
                {
                    getOuterOrderTrackingModel.OrderTrackingDetails.RemoveAt(getOuterOrderTrackingModel.OrderTrackingDetails.Count - 1);
                }

                //获取内部数据列表
                List<InTrackingLogInfo> getInTackingLogInfo = _trackingService.GetInTrackingLogInfos(wayBillInfo.WayBillNumber).OrderByDescending(a => a.ProcessDate).ToList();
                List<InTrackingLogInfo> getInTackingLogInfoList = _trackingService.GetInTrackingLogInfos(wayBillInfo.WayBillNumber).ToList();

                #endregion


                var orderTrackingDetailModelInfo = new List<OrderTrackingDetailModelInfo>();

                //都有数据
                if (getInTackingLogInfo.Any() && getOuterOrderTrackingModel != null)
                {
                    int? getSpanDay = 0;
                    int getCount = 0;

                    if (getOuterOrderTrackingModel.OrderTrackingDetails.Any())
                    {
                        //获取内部数据收货时间(第二条)
                        DateTime getSecondInnerDate = getInTackingLogInfoList.Count >= 2 ? getInTackingLogInfoList.Skip(1).Take(1).ToList()[0].ProcessDate.Value : getInTackingLogInfo[0].ProcessDate.Value;

                        //签收天数
                        TimeSpan ts = getOuterOrderTrackingModel.OrderTrackingDetails.First().ProcessDate.Value - getSecondInnerDate;
                        getSpanDay = Convert.ToInt32(Math.Ceiling(Math.Abs(ts.TotalDays)));
                        //外部明细
                        getCount = getOuterOrderTrackingModel.OrderTrackingDetails.Count();

                        //添加外部轨迹信息
                        getOuterOrderTrackingModel.OrderTrackingDetails.ForEach(
                        p => orderTrackingDetailModelInfo.Add(new OrderTrackingDetailModelInfo
                        {
                            WaybillNumber = wayBillInfo.WayBillNumber,
                            ProcessDate = p.ProcessDate,
                            ProcessContent = p.ProcessContent,
                            ProcessLocation = p.ProcessLocation
                        }));
                    }


                    //添加内部轨迹信息
                    getInTackingLogInfo.ForEach(p => orderTrackingDetailModelInfo.Add(new OrderTrackingDetailModelInfo
                    {
                        WaybillNumber = wayBillInfo.WayBillNumber,
                        ProcessDate = p.ProcessDate,
                        ProcessContent = p.ProcessContent,
                        ProcessLocation = p.ProcessLocation
                    }));


                    listContactTable.Add(new ContactTable()
                    {
                        WaybillNumber = wayBillInfo.WayBillNumber,
                        FalseTrackNumber = wayBillInfo.TrackingNumber,
                        CurrentLocation = "China",
                        CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                        InShippingMethodName = wayBillInfo.InShippingMethodName,

                        //运单退回情况
                        WaybillReurnStatus = contactTable.WaybillReurnStatus,
                        //目的地
                        Destination = _trackingNumberService.GetShippingInfo(wayBillInfo.WayBillNumber).CountryCode,

                        //最新事件时间
                        LastEventDate = getCount == 0 ? getInTackingLogInfo.First().ProcessDate : getOuterOrderTrackingModel.OrderTrackingDetails.First().ProcessDate,
                        LastEventContent = getCount == 0 ? getInTackingLogInfo.First().ProcessContent : getOuterOrderTrackingModel.OrderTrackingDetails.First().ProcessContent,

                        //包裹状态
                        EnumStingStatus = getCount != 0 ? enumStatus.PackageState(getOuterOrderTrackingModel.PackageState.Value) : WayBill.GetStatusDescription(wayBillInfo.Status),

                        //运单状态
                        WaybillStatus = wayBillInfo.Status,
                        //是否hold
                        IsHold = wayBillInfo.IsHold,

                        //收货天数
                        IntervalDays = getCount != 0 ? getSpanDay : Convert.ToInt32((System.DateTime.Now - getInTackingLogInfo.Min(a => a.ProcessDate).Value).TotalDays),
                        //真实跟踪号时
                        TrackNumber = wayBillInfo.TrueTrackingNumber,
                        //显示轨迹信息
                        OrderTrackingDetailModelInfo = orderTrackingDetailModelInfo

                    });

                }




                //内部表有数据,外部表没有数据
                if (getInTackingLogInfo.Any() && getOuterOrderTrackingModel == null)
                {

                    //添加内部轨迹信息
                    getInTackingLogInfo.ForEach(p => orderTrackingDetailModelInfo.Add(new OrderTrackingDetailModelInfo
                    {
                        WaybillNumber = wayBillInfo.WayBillNumber,
                        ProcessDate = p.ProcessDate,
                        ProcessContent = p.ProcessContent,
                        ProcessLocation = p.ProcessLocation
                    }));


                    listContactTable.Add(new ContactTable()
                    {
                        WaybillNumber = wayBillInfo.WayBillNumber,
                        FalseTrackNumber = wayBillInfo.TrackingNumber,
                        CurrentLocation = "中国",
                        CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                        InShippingMethodName = wayBillInfo.InShippingMethodName,

                        //目的地
                        Destination = _trackingNumberService.GetShippingInfo(wayBillInfo.WayBillNumber).CountryCode,
                        //最新事件时间
                        LastEventDate = getInTackingLogInfo.First().ProcessDate,
                        LastEventContent = getInTackingLogInfo.First().ProcessContent,

                        //包裹状态
                        EnumStingStatus = WayBill.GetStatusDescription(wayBillInfo.Status),

                        //运单状态
                        WaybillStatus = wayBillInfo.Status,
                        //是否hold
                        IsHold = wayBillInfo.IsHold,

                        //显示轨迹信息
                        OrderTrackingDetailModelInfo = orderTrackingDetailModelInfo

                    });

                }


                //都没有轨迹数据
                if (!getInTackingLogInfo.Any() && getOuterOrderTrackingModel == null)
                {
                    listContactTable.Add(new ContactTable()
                    {
                        NoQueryWaybillnumber = wayBillInfo.WayBillNumber
                    });
                }
            }


            return listContactTable;

        }

    }

}
