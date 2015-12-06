using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.BizLogWcf;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LighTake.Infrastructure.Web.Filters;
using LMS.Services.CountryServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Models;
using LMS.Services.OperateLogServices;

namespace LMS.Controllers
{
	public class OperationLogController : BaseController
	{
		private IOperateLogServices _operateLogServices;

		public OperationLogController(IOperateLogServices operateLogServices)
		{
			_operateLogServices = operateLogServices;
		}


		public ActionResult Index(OperationLogModelFilter filter)
		{
			if (filter.StartTime == null)
			{
				filter.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
			}
			if (filter.EndTime == null)
			{
				filter.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + "23:59");
			}

			return View(DataBind(filter));
		}

		[HttpPost]
		[FormValueRequired("Search")]
		public ActionResult Index(OperationLogViewModel model)
		{
			return View(DataBind(model.FilterModel));
		}


		public OperationLogViewModel DataBind(OperationLogModelFilter filter)
		{

			int totalCount = 0;
			if ((int)filter.SystemCode == 0)
			{
				filter.SystemCode = SystemType.LMS;
			}

			var model = new OperationLogViewModel()
			{
				FilterModel = filter,
				PagedList = _operateLogServices.QueryBizLogInfo(new BizLog()
				{
					SystemCode = filter.SystemCode,
					ModuleName = filter.ModuleName,
					KeywordType = filter.KeywordTypeID,
					Keyword = filter.Keyword,
					Summary = filter.Summary,
					UserRealName = filter.UserRealName

				}, filter.Page, filter.PageSize, filter.StartTime,filter.EndTime,out totalCount)
			};

			var getSystemTypeList = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "S012",
					TextField = "LMS",
					TextField_EN = "LMS"
				},
				new DataSourceBinder{
					ValueField = "S010",
					TextField = "LIS",
					TextField_EN = "LIS"
				}
			};

			//系统模块
			getSystemTypeList.ForEach(p => model.SystemTypeList.Add(
				new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.SystemCode.ToString() })
			);
			//关键字类别
			OperateLog.GetKeyWordTypeList().ForEach(p=>model.KeywordTypeList.Add(
				new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.KeywordTypeID.ToString() }
			));


			return model;
		}



		//选择操作功能模块
		public ActionResult SelectModuleName(string systemCode)
		{
			ModuleViewModel model = new ModuleViewModel();

			model.ListModule = GetListBizLogModel(systemCode);
			return View(model);
		}

		//搜索操作功能模块
		public JsonResult SearchModuleName(string keyword)
		{
			List<string> getList = GetListBizLogModel(keyword).Where(a => a.Contains(keyword)).ToList();
			return Json(getList, JsonRequestBehavior.AllowGet);
		}

		//取系统的功能模块
		public List<string> GetListBizLogModel(string systemCode)
		{
			List<string> getList = new List<string>() {};

			SystemType stType = SystemType.LMS;
			if (systemCode == "S010")//运费
			{
				stType = SystemType.LIS;
			}
			List<BizLogModel>  listBizLogModel= _operateLogServices.ShowBizLogInfomation(new BizLog() { SystemCode = stType });

			foreach (var bizLogModel in listBizLogModel)
			{
				if (bizLogModel.ModuleName=="BizLog日志系统")
				{
					continue;
				}
				if (!getList.Contains(bizLogModel.ModuleName))
				{
					getList.Add(bizLogModel.ModuleName);
				}
			}

			return getList;
		}






		public class OperationLogModelFilter:SearchFilter
		{
			public SystemType SystemCode { get; set; }
			public string ModuleName { get; set; }
			public KeywordType KeywordTypeID { get; set; }
			public string Keyword { get; set; }
			public string Summary { get; set; }
			public DateTime? StartTime { get; set; }
			public DateTime? EndTime { get; set; }
			public string UserRealName { get; set; }
		}

		public class OperationLogViewModel
		{
			public OperationLogViewModel()
			{
				FilterModel=new OperationLogModelFilter();
				PagedList = new PagedList<BizLogModel>();
				SystemTypeList=new List<SelectListItem>();
				ModuleNameList=new List<SelectListItem>();
				KeywordTypeList=new List<SelectListItem>();
			}
			public IList<SelectListItem> SystemTypeList { get; set; }
			public IList<SelectListItem> ModuleNameList { get; set; }
			public IList<SelectListItem> KeywordTypeList { get; set; }
			public OperationLogModelFilter FilterModel { get; set; }
			public IPagedList<BizLogModel> PagedList { get; set; }

		}

		public class Module
		{
			public string ModuleName { get; set; }
		}


		public class ModuleViewModel
		{
			public ModuleViewModel()
			{
				ListModule = new List<string>();
			}
			public List<string> ListModule { get; set; }
		}



	}
}