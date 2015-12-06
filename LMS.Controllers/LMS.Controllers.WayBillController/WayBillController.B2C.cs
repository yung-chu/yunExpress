using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Web.Filters;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.WayBillController
{
    public partial class WayBillController
    {
        public ActionResult B2CPreAlterList(B2CPreAlterListFilte param = null)
        {
            var model=new B2CPreAlterViewModel();
            if (param != null)
            {
                model.Param = param;
                model.Param.PageSize = param.PageSize;
                model = GetB2CPreAlterViewModel(model);
                model.PagedList = GetPageList(model);
            }
            else
            {
                model = GetB2CPreAlterViewModel(model);
            }
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("btnSearch")]
        [ActionName("B2CPreAlterList")]
        public ActionResult SeachB2CPreAlterList(B2CPreAlterListFilte param)
        {
            var model=new B2CPreAlterViewModel {Param = param};
            
            model = GetB2CPreAlterViewModel(model);
            model.PagedList = GetPageList(model);
            return View(model);
        }
        private B2CPreAlterViewModel GetB2CPreAlterViewModel(B2CPreAlterViewModel model)
        {
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (Int32.Parse(p.ValueField) < 4)
                {
                    model.SearchWheres.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField,
                        Selected = p.ValueField == model.Param.SearchWhere.ToString()
                    });
                }
            });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                if (p.ValueField == "2" || p.ValueField == "3")
                {
                    model.DateTimeWheres.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField,
                        Selected = p.ValueField == model.Param.SearchTime.ToString()
                    });
                }
            });
            model.StatusList.Add(new SelectListItem()
            {
                Text = "全部",
                Value = ""
            });
            B2CPreAlter.GetStatusList().ForEach(p =>
                {
                    model.StatusList.Add(new SelectListItem()
                        {
                            Text = p.TextField,
                            Value = p.ValueField,
                            Selected =model.Param.Status.HasValue&& p.ValueField==model.Param.Status.Value.ToString()
                        });
                });
            var list = new List<int>();
            sysConfig.DDPShippingMethodId.Split(',').ToList().ForEach(p=>list.Add(Int32.Parse(p)));
            _freightService.GetShippingMethodsByIds(list).ForEach(p =>
                {
                    model.ShippingMethods.Add(new SelectListItem()
                        {
                            Text = p.FullName,
                            Value = p.ShippingMethodId.ToString()
                        });
                });
            model.Param.OutStartTime = DateTime.Parse(sysConfig.B2CPreAlterStartTime);
            return model;
        }
        private PagedList<B2CPreAlterExt> GetPageList(B2CPreAlterViewModel model)
        {
            return _outStorageService.GetB2CPreAlterExtList(new B2CPreAlterListParam()
            {
                CountryCode = model.Param.CountryCode,
                CustomerCode = model.Param.CustomerCode,
                EndTime = model.Param.EndTime,
                EndWeight = model.Param.EndWeight,
                IsSelectAll = model.Param.IsSelectAll,
                ShippingMethodId = model.Param.ShippingMethodId,
                ShippingMethodIds = model.ShippingMethods.Select(p => Int32.Parse(p.Value)).ToList(),
                SearchTime = model.Param.SearchTime,
                SearchWhere = model.Param.SearchWhere,
                StartTime = model.Param.StartTime,
                StartWeight = model.Param.StartWeight,
                Status = model.Param.Status,
                OutStartTime = model.Param.OutStartTime,
                PageSize = model.Param.PageSize,
                Page = model.Param.Page,
                SearchContext = model.Param.SearchContext
            });
        }
        [HttpPost]
        [FormValueRequired("btnPreAlter")]
        [ActionName("B2CPreAlterList")]
        public ActionResult SubmitB2CPreAlter(B2CPreAlterListFilte param)
        {
            var model = new B2CPreAlterViewModel { Param = param };
            model = GetB2CPreAlterViewModel(model);
            var result = false;
            try
            {
                if (param.IsSelectAll)
                {
                    result=_outStorageService.PreAlterB2CBySearch(new B2CPreAlterListParam()
                    {
                        CountryCode = model.Param.CountryCode,
                        CustomerCode = model.Param.CustomerCode,
                        EndTime = model.Param.EndTime,
                        EndWeight = model.Param.EndWeight,
                        IsSelectAll = model.Param.IsSelectAll,
                        ShippingMethodId = model.Param.ShippingMethodId,
                        ShippingMethodIds = model.ShippingMethods.Select(p => Int32.Parse(p.Value)).ToList(),
                        SearchTime = model.Param.SearchTime,
                        SearchWhere = model.Param.SearchWhere,
                        StartTime = model.Param.StartTime,
                        StartWeight = model.Param.StartWeight,
                        Status = model.Param.Status,
                        OutStartTime = model.Param.OutStartTime,
                        PageSize = model.Param.PageSize,
                        Page = model.Param.Page,
                        SearchContext = model.Param.SearchContext
                    });
                }
                else if (!param.SelectWayBillNumber.IsNullOrWhiteSpace())
                {
                    result=_outStorageService.PreAlterB2CByWayBillNumber(param.SelectWayBillNumber.Split(',').ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            if (result)
            {
                SetViewMessage(ShowMessageType.Success, "操作成功！");
            }
            else
            {
                SetViewMessage(ShowMessageType.Error, "操作失败！");
            }
            
            model.PagedList = GetPageList(model);
            return View(model);
        }
    }
}