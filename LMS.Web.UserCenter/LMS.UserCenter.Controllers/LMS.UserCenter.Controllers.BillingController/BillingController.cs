using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.BillingServices;
using LMS.Services.CountryServices;
using LMS.Services.FeeManageServices;
using LMS.Services.CommonServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.UserCenter.Controllers.BillingController.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using LighTake.Infrastructure.Web.Utities;
using SelectListModel = LMS.UserCenter.Controllers.BillingController.Models.SelectListModel;

namespace LMS.UserCenter.Controllers.BillingController
{
    [MemberOnly]
    public partial class BillingController : BaseController
    {

        private readonly IBillingService _billingService;
        private readonly IFeeManageService _feeManageService;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IGoodsTypeService _goodsTypeService;
        private readonly IFreightService _freightService;
        private readonly ICustomerService _customerService;

        public BillingController(IBillingService billingService,
            ICustomerService customerService,
            IFreightService freightService,
            IFeeManageService feeManageService,
            ICountryService countryService,
            IGoodsTypeService goodsTypeService,
            IWorkContext workContext)
        {
            _billingService = billingService;
            _feeManageService = feeManageService;
            _goodsTypeService = goodsTypeService;
            _countryService = countryService;
            _workContext = workContext;
            _freightService = freightService;
            _customerService = customerService;
        }

        #region 账户异动记录

        //账户异动首页列表
        public ActionResult ChangeRecords(BillingFilterModel filter)
        {
            return View(List(filter));
        }

        [HttpPost]
        [ActionName("ChangeRecords")]
        [FormValueRequired("btnToExcel")]
        public ActionResult ToExecl(BillingViewModels param)
        {
            var viewModel = List(param.Filter);
            var list = new List<BillingExecModel>();
            viewModel.BillingList.InnerList.ForEach(p =>
            {
                var m = new BillingExecModel()
                {
                    Balance = p.Balance,
                    CreatedOn = p.CreatedOn,
                    CustomerCode = p.CustomerCode,
                    MoneyChangeTypeShortName = p.MoneyChangeTypeShortName,
                    Remark = p.Remark,
                    SerialNumber = p.SerialNumber,
                    InCash = 0,
                    OutCash = 0
                };
                if (p.Amount.HasValue)
                {
                    if (p.Amount.Value > 0)
                    {
                        m.InCash = p.Amount.Value;
                    }
                    else
                    {
                        m.OutCash = p.Amount.Value;
                    }
                }
                list.Add(m);
            });
            var titleList = new List<string> { "CustomerCode-客户代码", "CreatedOn-日期", "MoneyChangeTypeShortName-费用类型", "Remark-费用说明", "InCash-进账金额", "OutCash-出账金额", "Balance-帐户结余" };
            ExportExcelByWeb.WriteToDownLoad(list, titleList, null);
            return View(viewModel);
        }

        //账户异动记录列表封装方法
        private BillingViewModels List(BillingFilterModel filter)
        {
            if (IsPostRequest)
                filter.Page = 1;

            var viewModels = new BillingViewModels { Filter = filter };

            var param = new AmountRecordSearchParam //B_LMS.Data
            {
                CustomerCode = _workContext.User.UserUame,
                StartDateTime = filter.StartDateTime,
                EndDateTime = filter.EndDateTime,
                Page = filter.Page,
                PageSize = filter.PageSize
            };

            decimal totalInFee = 0;
            decimal totalOutFee = 0;

            var list = _billingService.GetCustomerAmountRecordPagedList(param, out totalInFee, out totalOutFee);
            viewModels.BillingList = list.ToModelAsPageCollection<CustomerAmountRecordExt, BillingModel>();//LMS_Db.Entities与LMS.UserCenter.Controllers.BillingController映射

            viewModels.TotalInFee = totalInFee;
            viewModels.TotalOutFee = totalOutFee;

            return viewModels;
        }

        #endregion

        #region 订单扣费明细

        //首页列表方法
        public ActionResult ChargebackDetail(InFeeListFilterModel filter)
        {
            return View(List(filter));
        }

        /// <summary>
        /// 订单扣费查询
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChargebackDetail")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearachChargebackDetail(InFeeListFilterModel filterModel)
        {
            return View(List(filterModel));
        }

        /// <summary>
        /// 导出订单扣费明细
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChargebackDetail")]
        [FormValueRequired("btnToExcel")]
        public ActionResult OutFeeInfoToExecl(InFeeListFilterModel filterModel)
        {
            var model = ListExport(filterModel);
            var titleList = new List<string> { "WayBillNumber-运单号", "CustomerOrderNumber-客户订单号", "CustomerCode-客户代码", "InDateTime-收货时间",
                "TrackingNumber-跟踪号", "ChineseName-发货国家", "InShippingName-运输方式", "SettleWeight-计费重量" ,
            "Freight-运费","Register-挂号费","FuelCharge-燃油费","Surcharge-附加费","TotalFee-总费用"};
            ExportExcelByWeb.WriteToDownLoad(model.List, titleList, null);
            return View(model);
        }

        //订单扣费明细封装方法
        private InFeeInfoListViewModel List(InFeeListFilterModel filter)
        {
            if (IsPostRequest)
                filter.Page = 1;

            var viewModels = new InFeeInfoListViewModel { FilterModel = filter };
            var param = new InFeeListParam //B_LMS.Data
            {
                CustomerCode = _workContext.User.UserUame,
                CountryCode = filter.CountryCode,
                EndTime = (filter.EndTime.HasValue ? filter.EndTime.Value.ToString("yyyy-MM-dd 23:59:59") : null).ConvertTo<DateTime?>(),
                SearchContext = filter.SearchContext,
                ShippingMethodId = filter.ShippingMethodId,
                SearchWhere = filter.SearchWhere,
                StartTime = filter.StartTime,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
            decimal alltotalfee = 0;

            var list = _feeManageService.GetInFeeInfoPagedList(param, out alltotalfee);
            viewModels.PagedList = list.ToModelAsPageCollection<InFeeInfoExt, InFeeInfoModel>();//LMS_Db.Entities与LMS.UserCenter.Controllers.BillingController映射

            viewModels.AllTotalFee = alltotalfee;

            InitAdd();

            return viewModels;
        }

        //订单扣费明细封装方法
        private InFeeInfoListViewModel ListExport(InFeeListFilterModel filter)
        {
            if (IsPostRequest)
                filter.Page = 1;

            var viewModels = new InFeeInfoListViewModel { FilterModel = filter };

            var param = new InFeeListParam //B_LMS.Data
            {
                CustomerCode = _workContext.User.UserUame,
                CountryCode = filter.CountryCode,
                EndTime = (filter.EndTime.HasValue ? filter.EndTime.Value.ToString("yyyy-MM-dd 23:59:59") : null).ConvertTo<DateTime?>(),
                SearchContext = filter.SearchContext,
                ShippingMethodId = filter.ShippingMethodId,
                SearchWhere = filter.SearchWhere,
                StartTime = filter.StartTime
            };

            decimal alltotalfee = 0;

            var list = _feeManageService.GetInFeeInfoList(param, out alltotalfee);
            viewModels.List = list.ToModelAsCollection<InFeeInfoExt, InFeeInfoModel>();//LMS_Db.Entities与LMS.UserCenter.Controllers.BillingController映射

            viewModels.AllTotalFee = alltotalfee;

            InitAdd();

            return viewModels;
        }

        #endregion

        #region 账户充值

        public ActionResult Recharge()
        {
            var model = new RechargeViewModel
            {
                CustomerBalances = GetCustomerBalance(),
                RechargeWayList = RechargeWayList()
            };
            return View(model);
        }

        /// <summary>
        /// 客户余额实体
        /// </summary>
        /// <returns></returns>
        private CustomerBalances GetCustomerBalance()
        {
            return _billingService.GetCustomerBalance(_workContext.User.UserUame).ToModel<CustomerBalances>();
        }

        /// <summary>
        /// 充值类型集合
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> RechargeWayList()
        {
            var listModel = new List<SelectListModel>();
            var list = _billingService.GetRechargeTypeList(1);
            list.Each(p =>
            {
                listModel.Add(new SelectListModel()
                {
                    SelectValue = p.RechargeType1.ToString(),
                    SelectName = p.RechargeTypeName
                });
            });
            return listModel;
        }

        //用户充值保存
        [HttpPost]
        [ActionName("Recharge")]
        [FormValueRequired("btnSave")]
        public ActionResult btnSave(FormCollection form)
        {
            var param = new CustomerCreditInfo()
            {
                CustomerCode = _workContext.User.UserUame,
                Amount = Convert.ToDecimal(form["Amount"]),
                RechargeType = Convert.ToInt32(form["RechargeType"]),
                TransactionNo = form["TransactionNo"],
                Remark = form["Remark"],
                CreatedBy = _workContext.User.UserUame,
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                Status = 1
            };

            try
            {
                string filePath = sysConfig.VoucherPath+ _workContext.User.UserUame+@"\";
                var fileName = SaveFileToService(filePath);

                param.VoucherPath = fileName;
                _billingService.CreateRechargeRecord(param);
                SuccessNotification("保存成功！");
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.Message);
            }
            return RedirectToAction("Recharge");
        }

        /// <summary>
        /// 保存文件到服务器上
        /// </summary>
        /// <returns>返回当前上传成功后的文件名</returns>
        private string SaveFileToService(string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                string tempName = string.Empty;
                HttpFileCollectionBase files = HttpContext.Request.Files;
                for (int iFile = 0; iFile < files.Count; iFile++)
                {
                    HttpPostedFileBase postedFile = files[iFile];
                    tempName = Path.GetFileName(postedFile.FileName);
                    if (string.IsNullOrWhiteSpace(tempName))
                        throw new Exception("请选择需要上传的文件");
                    string fileExtension = Path.GetExtension(tempName);
                    //if (fileExtension != ".xls")
                    //    throw new Exception("只能上传xls类型的文件");
                    if (!string.IsNullOrEmpty(tempName))
                    {
                        tempName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;
                        postedFile.SaveAs(filePath + tempName);
                    }
                }
                return tempName;
            }
            catch (Exception ex)
            {
                throw new Exception("上传文件保存出错:" + ex.Message);
            }
        }

        #endregion

        #region 用户密码修改

        public ActionResult ModifyPassword()
        {
            return View(GetCustomerModel());
        }

        //修改用户密码 0失败 1成功 其他
        [HttpPost]
        public string UpdateCustomer(CustomerModel model)
        {
            string isSuccess = "0";
            if (model != null && !String.IsNullOrEmpty(model.CustomerCode))
            {
                Customer param = _billingService.GetCustomer(_workContext.User.UserUame);
                param.AccountPassWord = model.SureNewPassWord.Trim().ToMD5();
                isSuccess = _billingService.UpdateCustomer(param);
            }
            return isSuccess;
        }

        //根据客户代码获取用户实体
        public CustomerModel GetCustomerModel()
        {
            Customer model = _billingService.GetCustomer(_workContext.User.UserUame);
            return new CustomerModel
            {
                CustomerCode = model.CustomerCode,
                AccountID = model.AccountID,
                AccountPassWord = model.AccountPassWord
            };
        }

        //旧密码是否正确
        [HttpPost]
        public string ComparePassword(FormCollection form)
        {
            return form["AccountPassWord"].ToMD5() == GetCustomerModel().AccountPassWord ? "1" : "0";
        }

        #endregion

        #region 下拉公用

        //下拉公共
        public void InitAdd(bool isEnabled = false)
        {
            ViewBag.CountryList = GetCountryList("");//国家
            ViewBag.ShippingMethods = GetShipingMethods(isEnabled);//运输方式
            ViewBag.SearchWhere = GetSearchWhere();//查询条件
        }

        //查询条件
        public List<SelectListItem> GetSearchWhere()
        {
            var listItem = new List<SelectListItem>();
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                listItem.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField });
            });
            return listItem;
        }

        //运输方式
        public List<SelectListItem> GetShipingMethods(bool isEnabled = false)
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            _freightService.GetShippingMethodListByCustomerCode(_workContext.User.UserUame, isEnabled).ForEach(s => list.Add(new SelectListItem
            {
                Value = s.ShippingMethodId.ToString(CultureInfo.InvariantCulture),
                Text = s.ShippingMethodName
            }));

            return list;
        }

        #endregion

        public ActionResult FreightTrial()
        {
            ViewBag.CountryList = GetCountryList("");
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            return View(new FreightTrialViewModels());
        }

        [HttpPost]
        [FormValueRequired("Trial")]
        public ActionResult FreightTrial(FreightTrialFilterModel filter)
        {
            ViewBag.CountryList = GetCountryList("");
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            var viewModels = new FreightTrialViewModels { Filter = filter };

            try
            {
                viewModels = GetFreightList(filter);
            }
            catch (Exception e)
            {
                ErrorNotification(e.Message);
            }

            return View(viewModels);
        }

        [HttpPost]
        [FormValueRequired("Import")]
        public ActionResult FreightTrial(FreightTrialFilterModel filter, bool isImport = true)
        {

            var viewModels = new FreightTrialViewModels { Filter = filter };

            try
            {
                viewModels = GetFreightList(filter);
                var listTile = new Dictionary<string, string>()
                    {
                        {"ShippingMethodName","运输方式"},
                        {"Weight","计算重量"},
                        {"ShippingFee","运费"},
                        {"RegistrationFee","挂号费"},
                        {"FuelFee","燃油费"},
                        {"SundryFee","杂费"},
                        {"TotalFee","总费用"},
                        {"DeliveryTime","时效"},
                        {"Remarks","备注"},
                    };
                ExcelHelper.WriteToDownLoad("ShippingFreight.xls", "sheet1", viewModels.FreightList, listTile);
            }
            catch (Exception e)
            {
                ViewBag.CountryList = GetCountryList("");
                ViewBag.GoodsTypeList = GetGoodsTypeList();
                ErrorNotification(e.Message);
            }

            return View(viewModels);
        }

        private FreightTrialViewModels GetFreightList(FreightTrialFilterModel filter)
        {
            var viewModels = new FreightTrialViewModels { Filter = filter };

            if (!filter.Weight.HasValue &&
                (!filter.Length.HasValue && !filter.Width.HasValue && !filter.Height.HasValue))
            {
                throw new Exception("重量或长宽高必填其中一项");
            }

            if (string.IsNullOrWhiteSpace(filter.CountryCode))
            {
                throw new Exception("请选择发货国家");
            }
            var customerCode = _workContext.User.UserUame;
            var customer = _customerService.GetCustomer(customerCode);

            if (customer == null || !customer.CustomerTypeID.HasValue)
            {
                throw new Exception("客户类型不存在");
            }
            var list = _freightService.GetCustomerShippingPrices(new FreightPackageModel()
            {
                Weight = filter.Weight ?? 0,
                Length = filter.Length ?? 0,
                Width = filter.Width ?? 0,
                Height = filter.Height ?? 0,
                CountryCode = filter.CountryCode,
                ShippingTypeId = filter.PackageType,
                CustomerTypeId = customer.CustomerTypeID.Value,
                CustomerId = customer.CustomerID
            });

            var shippingList = _freightService.GetShippingMethodListByCustomerCode(customerCode, true);
            foreach (var item in list)
            {
                if (!item.CanShipping) continue;
                if (item.ShippingMethodId == null) throw new Exception(string.Format("没有运输方式"));
                var shippingMethod =
                    shippingList.First(
                        s => s.ShippingMethodId == item.ShippingMethodId.Value);
                if (shippingMethod == null) throw new Exception(string.Format("运输方式【{0}】不存在", item.ShippingMethodId.Value));

                viewModels.FreightList.Add(new FreightModel
                {
                    ShippingMethodName = shippingList.First(s => item.ShippingMethodId != null && s.ShippingMethodId == item.ShippingMethodId.Value).ShippingMethodName,
                    Weight = item.Weight,
                    ShippingFee = item.ShippingFee,
                    RegistrationFee = item.RegistrationFee,
                    RemoteAreaFee = item.RemoteAreaFee,
                    FuelFee = item.FuelFee,
                    OtherFee = item.OtherFee,
                    DeliveryTime = item.DeliveryTime,
                    Remarks = item.Remark
                });
            }
            viewModels.FreightList = viewModels.FreightList.OrderBy(o => o.TotalFee).ToList();
            return viewModels;
        }

        public List<SelectListItem> GetCountryList(string keyword)
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            _countryService.GetCountryList(keyword).ForEach(c => list.Add(new SelectListItem
            {
                Value = c.CountryCode,
                Text = string.Format("{0}|{1}", c.CountryCode, c.ChineseName)
            }));

            return list;
        }

        public List<SelectListItem> GetGoodsTypeList()
        {
            var list = new List<SelectListItem>();
            _goodsTypeService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.GoodsTypeID.ToString(CultureInfo.InvariantCulture),
                Text = c.GoodsTypeName
            }));

            return list;
        }
    }
}
