using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Web.Filters;
using LMS.Controllers.FubController.Models;
using LMS.Data.Entity;
using LMS.Services.CountryServices;
using LMS.Services.FubServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.Controllers.FubController
{
    public partial class FubController : BaseController
    {
        private readonly IFubService _fubService;
        private readonly ICountryService _countryService;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IWorkContext _workContext;
        private readonly IFreightService _freightService;

        public FubController(IFubService fubService, ICountryService countryService,
            IWayBillInfoRepository wayBillInfoRepository, IWorkContext workContext, IFreightService freightService)
        {
            _fubService = fubService;
            _countryService = countryService;
            _wayBillInfoRepository = wayBillInfoRepository;
            _workContext = workContext;
            _freightService = freightService;
        }

        /// <summary>
        /// 国际小包优+ 查询
        /// Add By zhengsong
        /// Time:2014-12-30
        /// </summary>
        /// <returns></returns>
        public ActionResult FubSelectList(FubListFilterModel filter)
        {
            if (filter.DateWhere == 0)
            {
                if (filter.StartTime == null)
                {
                    filter.StartTime = DateTime.Now.AddDays(-1);
                }
                if (filter.EndTime == null)
                {
                    filter.EndTime = DateTime.Now;
                }
                filter.DateWhere = 1;
            }
            return View(BindListViewModel(filter));
        }

        [HttpPost]
        public ActionResult FubSelectList(FubListViewModel model)
        {
            return View(BindListViewModel(model.FubListFilterModel));
        }


        /// <summary>
        /// 福邮邮袋扫描查询
        /// Add By zhengsong
        /// Time:2014-12-31
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ActionResult FuzhouSelectList(FubListFilterModel filter)
        {
            if (filter.DateWhere == 0)
            {
                if (filter.StartTime == null)
                {
                    filter.StartTime = DateTime.Now.AddDays(-1);
                }
                if (filter.EndTime == null)
                {
                    filter.EndTime = DateTime.Now;
                }
            }
            filter.DateWhere = 1;
            return View(BindListViewModel(filter));
        }

        [HttpPost]
        public ActionResult FuzhouSelectList(FubListViewModel model)
        {
            model.FubListFilterModel.DateWhere = 1;
            return View(BindListViewModel(model.FubListFilterModel));
        }

        /// <summary>
        /// 中心局邮袋扫描查询
        /// Add By zhengsong
        /// Time:2014-12-31
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ActionResult CenterSelectList(FubListFilterModel filter)
        {
            if (filter.DateWhere == 0)
            {
                if (filter.StartTime == null)
                {
                    filter.StartTime = DateTime.Now.AddDays(-1);
                }
                if (filter.EndTime == null)
                {
                    filter.EndTime = DateTime.Now;
                }
            }
            filter.DateWhere = 2;
            return View(BindListCenterViewModel(filter));
        }

        [HttpPost]
        public ActionResult CenterSelectList(FubListViewModel model)
        {
            model.FubListFilterModel.DateWhere = 2;
            return View(BindListCenterViewModel(model.FubListFilterModel));
        }

        public FubListViewModel BindListViewModel(FubListFilterModel filter)
        {
            FubListViewModel model = new FubListViewModel();
            model.SearchWheres.Add(new SelectListItem()
                {
                    Text = "请选择",
                    Value = "",
                    Selected = !filter.SearchWhere.HasValue
                });
            model.SearchWheres.Add(new SelectListItem()
                {
                    Text = "客户邮袋号",
                    Value = "1",
                    Selected = "1" == filter.SearchWhere.ToString()
                });
            model.SearchWheres.Add(new SelectListItem()
                {
                    Text = "福邮邮袋号",
                    Value = "2",
                    Selected = "2" == filter.SearchWhere.ToString()
                });
            model.SearchWheres.Add(new SelectListItem()
                {
                    Text = "总包号",
                    Value = "3",
                    Selected = "3" == filter.SearchWhere.ToString()
                });
            model.DateWheres.Add(new SelectListItem
                {
                    Text = "福州扫描日期",
                    Value = "1",
                    Selected = "1" == filter.DateWhere.ToString()
                });
            model.DateWheres.Add(new SelectListItem
                {
                    Text = "中心局扫描日期",
                    Value = "2",
                    Selected = "2" == filter.DateWhere.ToString()
                });
            model.FubListFilterModel = filter;
            model.CountryList = GetCountryList("");
            FubListParam param = new FubListParam();
            param.DateWhere = filter.DateWhere;
            param.SearchWhere = filter.SearchWhere;
            param.SearchContext = filter.SearchContext;
            param.StartTime = filter.StartTime;
            param.EndTime = filter.EndTime;
            param.CreatedBy = filter.CreatedBy;
            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.PagedList =
                _fubService.GetFubPagedList(param).ToModelAsPageCollection<FubListModelExt, FubListModel>();
            return model;
        }

        public FubListViewModel BindListCenterViewModel(FubListFilterModel filter)
        {
            FubListViewModel model = new FubListViewModel();
            //model.SearchWheres.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.SearchWhere.HasValue });
            //model.SearchWheres.Add(new SelectListItem() { Text = "客户邮袋号", Value = "1", Selected = "1" == filter.DateWhere.ToString() });
            //model.SearchWheres.Add(new SelectListItem() { Text = "福邮邮袋号", Value = "2", Selected = "2" == filter.DateWhere.ToString() });
            //model.SearchWheres.Add(new SelectListItem() { Text = "总包号", Value = "3", Selected = "3" == filter.DateWhere.ToString() });
            //model.DateWheres.Add(new SelectListItem { Text = "福州扫描日期", Value = "1", Selected = "1" == filter.DateWhere.ToString() });
            //model.DateWheres.Add(new SelectListItem { Text = "中心局扫描日期", Value = "2", Selected = "2" == filter.DateWhere.ToString() });
            model.FubListFilterModel = filter;
            model.CountryList = GetCountryList("");
            FubListParam param = new FubListParam();
            param.DateWhere = filter.DateWhere;
            //param.SearchWhere = filter.SearchWhere;
            //param.SearchContext = filter.SearchContext;
            param.StartTime = filter.StartTime;
            param.EndTime = filter.EndTime;
            param.CreatedBy = filter.CreatedBy;
            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.PagedList =
                _fubService.GetFubCenterPagedList(param).ToModelAsPageCollection<FubListModelExt, FubListModel>();
            return model;
        }

        public List<SelectListItem> GetCountryList(string keyword)
        {
            var list = new List<SelectListItem> {new SelectListItem {Value = "", Text = ""}};
            _countryService.GetCountryList(keyword).ForEach(c => list.Add(new SelectListItem
            {
                Value = c.CountryCode,
                Text = c.ChineseName
            }));
            return list;
        }



        /// <summary>
        /// 退件记录查询
        /// add by yungchu
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnBagLogList(MailReturnGoodsLogsParam filterModel)
        {
            return View(ReturnBagLogDataBind(filterModel));
        }

        [HttpPost]
        [FormValueRequired("Search")]
        public ActionResult ReturnBagLogList(ReturnBagLogListModel model)
        {
            return View(ReturnBagLogDataBind(model.FilterModel));
        }

        public ReturnBagLogListModel ReturnBagLogDataBind(MailReturnGoodsLogsParam filterModel)
        {
            filterModel.StartTime = filterModel.StartTime ?? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + "00:00");
            filterModel.EndTime = filterModel.EndTime ?? DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + "00:00");


            var model = new ReturnBagLogListModel()
            {
               FilterModel=  filterModel,
               PagedList = _fubService.GetMailReturnGoodsLogsList(new MailReturnGoodsLogsParam
                {
                    Page=filterModel.Page,
                    PageSize=filterModel.PageSize,
                    TrackNumber =filterModel.TrackNumber,
                    StartTime=filterModel.StartTime,
                    EndTime=filterModel.EndTime,
                    ReasonType=filterModel.ReasonType,
                    ReturnBy =filterModel.ReturnBy
                })
            };


            if (model.PagedList.InnerList != null && model.PagedList.InnerList.Any())
            {
                model.PagedList.InnerList.ForEach(a =>
                {
                    a.ReturnReason = MailReturnGoodsLogs.GetReasonTypeDescription(a.ReasonType);
                });
            }


            //下拉框
            model.SelectListItem.Add(new SelectListItem() {Text="全部",Value=""});
            MailReturnGoodsLogs.GetReasonTypeList().ForEach(a => model.SelectListItem.Add(new SelectListItem()
            {
                Text = a.TextField,
                Value = a.ValueField,
                Selected = filterModel.ReasonType.HasValue&&a.ValueField==filterModel.ReasonType.Value.ToString()
            }));

            return model;
        }

        [HttpPost]
        [FormValueRequired("btnExport")]
        [System.Web.Mvc.ActionName("ReturnBagLogList")]
        public ActionResult ExportMailReturnGoods(MailReturnGoodsLogsParam filterModel)
        {

             var titleList = new List<string>
                    {
                        "TrackNumber-包裹单号",
                        "InShippingMethodName-运输方式",
                        "CountryCode-发货国家",
                        "Weight-重量kg",
                        "ReturnReason-退回原因",
                        "ReturnOn-退回时间",
                        "ReturnBy-退回操作人"
                    };

            var getModel = ReturnBagLogDataBind(filterModel);
            var getList = getModel.PagedList.InnerList;


            string fileName = "退件记录查询导出" + DateTime.Now.ToString("yyyy-dd-MM-hh-mm-ss");
            ExportExcelByWeb.ListExcel(fileName, getList, titleList);

            return View(getModel);
        }

        /// <summary>
        /// 换袋记录查询
        /// add by yungchu
        /// </summary>
        /// <returns></returns>
        public ActionResult ExchangeBagLogList(MailExchangeBagLogsParam filterModel)
        {
            return View(ExchangeBagLogDataBind(filterModel));
        }

        [HttpPost]
        [FormValueRequired("Search")]
        public ActionResult ExchangeBagLogList(ExchangeBagLogModel model)
        {
            return View(ExchangeBagLogDataBind(model.FilterModel));
        }

        public ExchangeBagLogModel ExchangeBagLogDataBind(MailExchangeBagLogsParam filterModel)
        {
            filterModel.StartTime = filterModel.StartTime ?? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + "00:00"); ;
            filterModel.EndTime = filterModel.EndTime ?? DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + "00:00"); ;

            var model = new ExchangeBagLogModel()
            {
                FilterModel = filterModel,
                PagedList=_fubService.GetMailExchangeBagLogsList(new  MailExchangeBagLogsParam()
                {
                    Page=filterModel.Page,
                    PageSize=filterModel.PageSize,
                    TrackNumber=filterModel.TrackNumber,
                    StartTime=filterModel.StartTime,
                    EndTime=filterModel.EndTime
                })
            };
            return model;
        }

        public ActionResult ReturnGoods()
        {
            return View(new ReturnGoodsViewModel());
        }

        [HttpPost]
        public ActionResult AddReturnGoods(List<ReturnGoodsModel> returnGoodsModels)
        {
            try
            {
                _fubService.AddMailReturnGoodsLogs(returnGoodsModels);
                return Json(new { Result = true});
            }
            catch (Exception ex)
            {
                return Json(new {Result = false, Message = ex.Message});
            }
        }

        [HttpPost]
        public ActionResult CanReturnGoods(string trackNumber, int reasonType)
        {
            try
            {
                _fubService.CanAddMailReturnGoodsLogs(trackNumber, reasonType);
                var wayBill=_wayBillInfoRepository.GetWayBillInfoExtSilm(trackNumber);
                return Json(new 
                { 
                    Result = true,
                    TrackNumber = trackNumber,
                    ReasonType=reasonType,
                    Weight = wayBill.Weight,
                    Country = GetGetCountryListFromCache().FirstOrDefault(p => p.CountryCode == wayBill.CountryCode).ChineseName,
                    ReturnBy = _workContext.User.UserUame,
                });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }

        private List<Country> GetGetCountryListFromCache()
        {
            const string key = "List_Country";

            object inCache = Cache.Get(key);

            if (inCache != null)
            {
                var listCountry = inCache as List<Country>;

                if (listCountry != null) return listCountry;
            }

            var listCountryNewest = _freightService.GetCountryList();

            if (listCountryNewest != null)
            {
                Cache.Add(key, listCountryNewest, 60);
            }

            return listCountryNewest;
        }


        public ActionResult HoldList(HoldLogModel model)
        {
            return View(HoldListDataBind(model.FilterModel));
        }

        public HoldLogModel HoldListDataBind(HoldLogFilterModel filterModel)
        {
            var viewModel = new HoldLogModel()
            {
                FilterModel = filterModel,
                PagedList = _fubService.GetMailHoldLogsList(new MailHoldLogsParam()
                {
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                    TrackNumbers = string.IsNullOrWhiteSpace(filterModel.TrackNumbers)?new string[]{}: filterModel.TrackNumbers.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                    StartTime = filterModel.StartTime,
                    EndTime = filterModel.EndTime
                })
            };
            return viewModel;
        }

        [HttpPost]
        public ActionResult Hold(string trackNumbers)
        {
            try
            {
                _fubService.AddMailHoldLogs(trackNumbers.Split(','));

                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }

    }
}
