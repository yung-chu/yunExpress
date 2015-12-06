using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.Param;
using LMS.Services.BillingServices;
using LMS.Services.CustomerServices;
using LMS.Services.InStorageServices;
using LMS.Services.SequenceNumber;
using LMS.Services.SettlementServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.Controllers.SettlementController
{
    public partial class SettlementController : BaseController
    {
        private readonly ISettlementService _settlementService;
        private readonly ICustomerService _customerService;
        private readonly IInStorageService _inStorageService;
        private readonly IBillingService _billingService;
        private IWorkContext _workContext;
        public SettlementController(ISettlementService settlementService, ICustomerService customerService
            , IInStorageService inStorageService, IBillingService billingService, IWorkContext workContext)
        {
            _settlementService = settlementService;
            _customerService = customerService;
            _inStorageService = inStorageService;
            _billingService = billingService;
            _workContext = workContext;
        }

        public ActionResult SettlementDetail(string settlementNumber)
        {
            var settlementInfo = _settlementService.GetSettlementInfo(settlementNumber);

            if (settlementInfo == null) return Content("该结算单号不存在");

            var model = settlementInfo.ToModel<SettlementDetailViewModel>();
            model.CustomerName = _customerService.GetCustomer(model.CustomerCode).Name;

            return View(model);
        }

        public ActionResult NoSettlementList(string customerCode)
        {
            var model = new NoSettlementListViewModel()
                {
                    CustomerCode = customerCode
                };
            
            if (string.IsNullOrWhiteSpace(customerCode)) return View(model);

            model.InStorageInfos = _inStorageService.GetInStorageNoSettlementList(customerCode);
            model.CustomerName = _customerService.GetCustomer(customerCode).Name;
            model.InStorageProcesses = _inStorageService.GetInStorageProcess(model.InStorageInfos.Select(w => w.InStorageID).ToList());
            return View(model);
        }

        public ActionResult CreateSettlement(string customerCode, string inStorageIDs)
        {
            try
            {
                string settlementNumber = _settlementService.CreateSettlement(customerCode, inStorageIDs.Split(','));

                return Json(new { Result = true, Message = settlementNumber });
            }
            catch (Exception ex)
            {
               Log.Exception(ex);
               return Json(new { Result = false, Message = ex.Message });
            }
        }

        public ActionResult QueryInStorageProcess(string inStorageIDs)
       {
           return Json(_inStorageService.GetInStorageProcess(inStorageIDs.Split(',').ToList()));
       }

        public ActionResult SettlementSummary(SettlementSummaryModelFilterModel filterModel)
       {
           var param = new SettlementSummaryParam();
           filterModel.CopyTo(param);
           var model = new SettlementSummaryModelViewModel()
               {
                   FilterModel = filterModel,
                   PagedList = _settlementService.GetSettlementSummaryExtPagedList(param),
               };
           return View(model);
       }

        /// <summary>
        /// 查看结算清单
        /// add bu yungchu
        /// </summary>
        /// <returns></returns>
        public ActionResult SettlementInfoList(SettlementInfoParam param)
        {
            return View(SettlementInfoDataBind(param));
        }

        [HttpPost]
		public ActionResult SettlementInfoList(SettlementInfoList model)
		{
			return View(SettlementInfoDataBind(model.FilterModel));
		}

		public SettlementInfoList SettlementInfoDataBind(SettlementInfoParam param)
		{
			if (param.StartTime == null)
			{
				param.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
			}
			if (param.EndTime == null)
			{
				param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
			}

			var model = new SettlementInfoList()
			{
				FilterModel = new SettlementInfoParam
				{
					Page = param.Page,
					PageSize = param.PageSize,
					CustomerCode = param.CustomerCode,
					StartTime = param.StartTime,
					EndTime = param.EndTime,
					CreatedBy = param.CreatedBy,
					Status = param.Status,
					SettlementBy = param.SettlementBy,
					SettlementNumber = param.SettlementNumber
				},
				PagedList = _settlementService.GetSettlementInfoList(param)
			};


			//结清状态
			model.PagedList.InnerList.ForEach(a => a.StatusDesc = Settlement.GetStatusDescription(a.Status));



			//状态下拉框
			var statusList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "全部", Selected = !model.FilterModel.Status.HasValue } };
			Settlement.GetStatusList().ForEach(a => statusList.Add(
				new SelectListItem { Text = a.TextField, Value = a.ValueField, Selected = param.Status.HasValue && a.ValueField == param.Status.Value.ToString() }
				));

			model.StatusList = statusList;


			return model;
		}



    }
}
