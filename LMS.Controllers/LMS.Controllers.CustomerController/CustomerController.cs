using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.CustomerServices;
using LMS.Services.FeeManageServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Web.Controllers;

using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Filters;
using LighTake.Infrastructure.Web.Utities;
using LMS.Services.OperateLogServices;

namespace LMS.Controllers.CustomerController
{
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/
        private ICustomerService _customerService;
        private IFeeManageService _feeManageService;
        private IFreightService _freightService;
		private IOperateLogServices _operateLogServices;
		private IWorkContext _workContext;
        private ICustomerSourceInfoRepository _customerSourceInfoRepository;

        public CustomerController(ICustomerService customerService, IFeeManageService feeManageService, IFreightService freightService,
            IOperateLogServices operateLogServices, IWorkContext workContext, ICustomerSourceInfoRepository customerSourceInfoRepository
			)
        {
            _customerService = customerService;
            _feeManageService = feeManageService;
            _freightService = freightService;
	        _operateLogServices = operateLogServices;
	        _workContext = workContext;
            _customerSourceInfoRepository = customerSourceInfoRepository;
        }


        public ActionResult Add()
		{
			var listSelectListItem = new List<SelectListItem>();
			GetListCustomerManagerInfo().ForEach(p => listSelectListItem.Add(new SelectListItem
			{
				Text = p.SelectName,
				Value = p.SelectValue

			}));

            var model = new CustomerAddViewModel
                {
                    CustomerTypeList = GetCustomerTypeList(),
                    PaymentTypeList = GetPaymentTypeList(),
                    CustomerStatus = GetCustomerStatusList(),
					CustomerManagerList = listSelectListItem
                };

            return View(model);
        }

        public ActionResult CustomerRecharge()
        {
            var model = new CustomerRechargeViewModel
                {
                    FeeTypeList = GetFeeTypeList(),
                    MoneyChangeTypeList = GetMoneyChangeTypeList()
                    
                };
            return View(model);
        }
        [HttpPost]
        public string CreateCustomerRecharge(CustomerRechargeModel model)
        {
            string isSuccess = "0";
            if (model != null)
            {
                var param = new CustomerAmountRecordParam()
                    {
                        Amount = model.Amount,
                        CustomerCode = model.CustomerCode,
                        MoneyChangeTypeId = model.MoneyChangeTypeID,
                        Remark = model.Remark,
                        FeeTypeId = model.FeeTypeID
                    };
                switch (Customer.ParseToNumberType(model.SelectNumberTypeID))
                {
                    case Customer.NumberTypeEnum.TransactionNo:
                        param.TransactionNo = model.NumberValue;
                        break;
                    case Customer.NumberTypeEnum.WayBill:
                        param.WayBillNumber = model.NumberValue;
                        break;
                }
                try
                {
                    isSuccess = _customerService.CreateCustomerAmountRecord(param).ToString();
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    isSuccess = ex.Message;
                }

            }
            return isSuccess;
        }

        public string CheckCustomerCreditInfo(int id)
        {
            string isSuccess = "0";
            try
            {
                var model = _customerService.GetCustomerCreditInfo(id);
                if (model != null && model.Amount.HasValue && model.Status == CustomerCreditInfo.StatusToValue(CustomerCreditInfo.StatusEnum.NoCheck))
                {
                    var param = new CustomerAmountRecordParam()
                    {
                        Amount = model.Amount.Value,
                        CustomerCode = model.CustomerCode,
                        MoneyChangeTypeId = 1,
                        Remark = model.Remark,
                        FeeTypeId = 1,
                        TransactionNo = model.TransactionNo
                    };
                    if (_customerService.CreateCustomerAmountRecord(param) == 1)
                    {
                        _customerService.VerifyCustomerCreditInfo(id, CustomerCreditInfo.StatusEnum.Checked);
                    }
                    isSuccess = "1";
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                isSuccess = ex.Message;
            }
            return isSuccess;
        }

        public ActionResult CustomerRechargeList(CustomerRechargeListFilterModel param)
        {
            return View(Data_Bind(param));
        }
        [HttpPost]
        [ActionName("CustomerRechargeList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchCustomerRechargeList(CustomerRechargeListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(Data_Bind(param.FilterModel));
        }
        public ActionResult CustomerAmountRecordList(CustomerAmountRecordListFilterModel param)
        {
            return View(AmountRecordDataBind(param));
        }
        [HttpPost]
        [ActionName("CustomerAmountRecordList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchCustomerAmountRecordList(CustomerAmountRecordListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(AmountRecordDataBind(param.FilterModel));
        }
        [HttpPost]
        [ActionName("CustomerAmountRecordList")]
        [FormValueRequired("btnToExcel")]
        public ActionResult ToExecl(CustomerAmountRecordListViewModel param)
        {
            var model = AmountRecordDataBind(param.FilterModel);
            var list = new List<CustomerAmountRecordToExecl>();
            model.PagedList.InnerList.ForEach(p =>
                {
                    var m = new CustomerAmountRecordToExecl()
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
            return View(model);
        }

        public string GetCustomerRechargeList(FormCollection from)
        {
            var param = JsonHelper.JsonToEntity(from["params"], new CustomerListFilterModel()) as CustomerListFilterModel;
            var list =
                _customerService.GetCustomerCreditInfoList(param.CustomerCode, param.Status)
                                .ToModelAsCollection<CustomerCreditInfo, CustomerCreditModel>();
            return JsonHelper.CreateJsonParameters(list, true, list.Count);
        }

        public ActionResult Edit(string CustomerID)
        {
            var model = new CustomerEditViewModel();
            if (!string.IsNullOrWhiteSpace(CustomerID))
            {
                model.CustomerModel = _customerService.GetCustomerById(CustomerID).ToModel<CustomerListModel>();
                model.CustomerModel.AccountPassWord = "";
                model.CustomerStatus = GetCustomerStatusList();
                GetCustomerTypeList()
                .ForEach(
                    p =>
                    model.CustomerTypeList.Add(new SelectListItem()
                    {
                        Text = p.SelectName,
                        Value = p.SelectValue,
                        Selected = (model.CustomerModel.CustomerTypeID.HasValue && p.SelectValue == model.CustomerModel.CustomerTypeID.Value.ToString())
                    }));
                GetPaymentTypeList().ForEach(p => model.PaymentTypeList.Add(new SelectListItem()
                {
                    Text = p.SelectName,
                    Value = p.SelectValue,
                    Selected = (model.CustomerModel.PaymentTypeID.HasValue && p.SelectValue == model.CustomerModel.PaymentTypeID.Value.ToString())
                }));

				//业务经理
				GetListCustomerManagerInfo().ForEach(p => model.CustomerManagerList.Add(new SelectListItem
		            {
			            Text = p.SelectName,
			            Value = p.SelectValue,
						Selected = (!string.IsNullOrEmpty(model.CustomerModel.CustomerManager) && p.SelectValue == model.CustomerModel.CustomerManager)
		            }));



                //显示客户来源平台信息
                var getModel =_customerSourceInfoRepository.First(a => a.CustomerCode == model.CustomerModel.CustomerCode);
                model.SourcePlatform = getModel == null? "普通客户": Customer.GetCustomerSourceTypeDescription(getModel.SourceType);
             

	            model.ReturnUrl = Request.Form["returnUrl"] ?? Url.Action("List", "Customer");


            }
            return View(model);
        }



	    public JsonResult CreateCustomer(CustomerListModel model)
        {
            var result = new ResponseResult();

            if (model != null)
            {
                try
                {
                    model.ApiKey = Constants.ENCRYPT_KEY;
                    model.ApiSecret = model.CustomerCode.EncryptDES();
                    _customerService.CreateCustomer(model.ToEntity<Customer>());

					//同步到lis 客户
	                if (model.Status!=1)
	                {
		                Customer getCustomer=   _customerService.GetCustomerByAccountId(model.AccountID);
						UpdateCustomerInfoToLis(getCustomer.CustomerCode);
	                }

	                result.Result = true;
                    result.Message = "Success";
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    result.Result = false;
                    result.Message = ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditCustomer(CustomerEditViewModel model)
        {
            var result = new ResponseResult();
            if (model != null && model.CustomerModel != null)
            {
                try
                {

					//#region 操作日志
					////yungchu
					////敏感字-密码,登录帐号

					//StringBuilder  sb=new StringBuilder();
					//sb.Append("");
					//Customer getCustomer=  _customerService.GetCustomerById(model.CustomerModel.CustomerID.ToString());

					//if (getCustomer.Name!=model.CustomerModel.Name)
					//{
					//	sb.AppendFormat(" 名称从{0}更改为{1}", getCustomer.Name, model.CustomerModel.Name);
					//}
					//if (getCustomer.AccountID != model.CustomerModel.AccountID)
					//{
					//	sb.AppendFormat(" 登陆账号从{0}更改为{1}", getCustomer.AccountID, model.CustomerModel.AccountID);
					//}


					//BizLog bizlog = new BizLog()
					//{
					//	Summary =sb.ToString()!=""? "[客户编辑]"+sb:"客户编辑",
					//	KeywordType = KeywordType.CustomerCode,
					//	Keyword = model.CustomerModel.CustomerCode,
					//	UserCode = _workContext.User.UserUame,
					//	UserRealName = _workContext.User.UserUame,
					//	UserType = UserType.LMS_User,
					//	SystemCode = SystemType.LMS,
					//	ModuleName = "客户编辑"
					//};

					//_operateLogServices.WriteLog(bizlog, model);
					//#endregion

                    if (!string.IsNullOrWhiteSpace(model.CustomerModel.AccountID))
                    {
                        model.CustomerModel.AccountID = model.CustomerModel.AccountID.Trim();
                    }
                    if (!string.IsNullOrWhiteSpace(model.CustomerModel.AccountPassWord))
                    {
                        model.CustomerModel.AccountPassWord = model.CustomerModel.AccountPassWord.Trim();
                    }

                    _customerService.UpdateCustomer(model.CustomerModel.ToEntity<Customer>());
					//更新客户名称到lis客户表 yungchu
	                if (model.CustomerModel.Status != 1)
	                {
						UpdateCustomerInfoToLis(model.CustomerModel.CustomerCode);
	                }

                    result.Result = true;
                    result.Message = "Success";
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    result.Result = false;
                    result.Message = ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


		//(排除未审核)更新客户名称到lis客户表 yungchu
		public void UpdateCustomerInfoToLis(string customerCode)
	    {
			Customer getCustomers = _customerService.GetCustomer(customerCode);
			int getStatus = getCustomers.Status == 2 ? 1 : 2;

			var param = new CustomerInfoParam()
			{
				CustomerId=getCustomers.CustomerID,
				CustomerTypeId=getCustomers.CustomerTypeID,
				Status=getStatus,
				CustomerCode = getCustomers.CustomerCode,
				Name = getCustomers.Name,
				EnName = getCustomers.EnName,
				Address = getCustomers.Address,
				Phone = getCustomers.Tele,
				QQ = getCustomers.QQ,
				MSN = getCustomers.MSN,
				Skype = getCustomers.Skype,
				Country = getCustomers.Country,
				Province = getCustomers.Province,
				Fax = getCustomers.Fax,
				PostCode = getCustomers.PostCode,
				LastUpdatedBy = _workContext.User.UserUame

			};
			_freightService.UpdateCustomerInfoToLis(param);

	    }






	    public JsonResult GetSecret(string code)
        {
            var result = new ApiResult();
            try
            {
                result.ApiKey = GetRandom(24);
                result.ApiSecret = SecurityUtil.EncryptDES(code, result.ApiKey);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.ApiKey = "";
                result.ApiSecret = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //生成字母和数字随机数
        private string GetRandom(int length)
        {
            char[] Pattern = new char[]
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'
                    , 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int) DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

		//[OutputCache(Duration = 60 * 60, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
		//public string GetCustomerList(FormCollection from)
		//{
		//	var param = JsonHelper.JsonToEntity(from["params"], new CustomerListFilterModel()) as CustomerListFilterModel;
		//	var list = _customerService.GetCustomerList(param.CustomerCode, param.Status).ToModelAsCollection<Customer, CustomerListModel>();
		//	return JsonHelper.CreateJsonParameters(list, true, list.Count);
		//}


		[OutputCache(Duration = 5*60 * 60, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
		public ActionResult List(CustomerListFilterModel filterModel)
		{
			return View(CustomerListDataBind(filterModel));
		}


		[ActionName("List")]
		[HttpPost]
		[FormValueRequired("btnSearch")]
		public ActionResult GetList(CustomerListViewModel model)
		{
			return View(CustomerListDataBind(model.FilterModel));
	    }

	    public CustomerListViewModel CustomerListDataBind(CustomerListFilterModel filterModel)
		{
		    var model = new CustomerListViewModel()
		    {
			    FilterModel = filterModel,
			    PagedList = _customerService.GetCustomerList(new SearchCustomerParam
			    {
				    Page = filterModel.Page,
				    PageSize = filterModel.PageSize,
				    CustomerCode = filterModel.CustomerCode,
				    Status = filterModel.Status
				})
		    };

			model.CustomerStatus.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !model.FilterModel.Status.HasValue });
			GetCustomerStatusList().ForEach(i =>
			{
				model.CustomerStatus.Add(new SelectListItem() { Text = i.Text, Value = i.Value, Selected = model.FilterModel.Status.HasValue && model.FilterModel.Status.Value.ToString() == i.Value });
			});
		
		    model.PagedList.InnerList.ForEach(a => a.CurrentStatus= Customer.GetStatusDescription(a.Status));

		    return model;
	    }


		//业务经理下拉框
		public List<SelectListModel> GetListCustomerManagerInfo()
		{
			var listSelectListModel = new List<SelectListModel>();
			_customerService.GetListCustomerManagerInfo("").ForEach(p => listSelectListModel.Add(new SelectListModel
			{
				SelectName = p.Name,
				SelectValue = p.Name
			}));

			return listSelectListModel;
		}


	    //获取运费计算系统客户信息 yungchu
	    public JsonResult GetCustomerType(string customerCode)
	    {
		    var result= new ResponseResult();
		    List<Customer> getListCustomer = _freightService.GetListCustomers(customerCode);

			if (getListCustomer != null &&
				getListCustomer.Count != 0)
		    {
				Guid customerId = getListCustomer[0].CustomerID;
				result.Message = customerId.ToString();
			    result.Result = true;
		    }
		    else
		    {
				result.Result = false;//取当前类别
		    }

		    return Json(result, JsonRequestBehavior.AllowGet);
	    }



        #region 收货费用用户选择列表
        public ActionResult SelectReceivingExpenseList(DateTime? startTime, DateTime? endTime)
        {
            var model = new CustomerViewModel
                {
                    FilterModel = { StartTime = startTime, EndTime = endTime },
                    CustomerModels =
                        _customerService.GetCustomerByReceivingExpenseList(string.Empty, startTime, endTime)
                                        .ToModelAsCollection<Customer, CustomerModel>()
                };
            return View(model);
        } 

        public JsonResult GetSelectReceivingExpenseList(string keyword, DateTime? startTime, DateTime? endTime)
        {
            var list = _customerService.GetCustomerByReceivingExpenseList(keyword, startTime, endTime)
                                       .ToModelAsCollection<Customer, CustomerModel>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult SelectList(bool? IsAll, bool? showPaymentType, bool? onlyShowCash)
        {
            IsAll = IsAll ?? false;
            var model = CustomerDataBind(new CustomerFilterModel() { IsAll = IsAll.Value, OnlyShowCash = onlyShowCash });
            model.ShowPaymentType = showPaymentType ?? false;
            model.OnlyShowCash = onlyShowCash ?? false;
            return View(model);
        }

        [HttpPost]
        public ActionResult SelectList(CustomerViewModel paramModel)
        {
            return View(CustomerDataBind(paramModel.FilterModel));
        }


		#region 避免弹出框多层选不到
		public ActionResult SelectListInfo(bool? IsAll)
		{
			IsAll = IsAll ?? false;
			return View(CustomerDataBind(new CustomerFilterModel() { IsAll = IsAll.Value }));
		}

		[HttpPost]
		public ActionResult SelectListInfo(CustomerViewModel paramModel)
		{
			return View(CustomerDataBind(paramModel.FilterModel));
		}
		#endregion




        private CustomerViewModel CustomerDataBind(CustomerFilterModel filterModel)
        {
            CustomerViewModel viewModel = new CustomerViewModel();
            CustomerParam param = new CustomerParam();
            viewModel.FilterModel = filterModel;
            if (!string.IsNullOrWhiteSpace(filterModel.CustomerCode))
                param.CustomerCode = filterModel.CustomerCode;
            viewModel.CustomerModels =
                _customerService.GetCustomerList(param.CustomerCode).ToModelAsCollection<Customer, CustomerModel>();
            var listPaymentType = GetPaymentTypeList();

            if (filterModel.OnlyShowCash == true)
            {
                viewModel.CustomerModels.RemoveAll(p => p.PaymentTypeID != 3 && p.PaymentTypeID != 4);
            }
            viewModel.CustomerModels.ForEach(p => p.PaymentName = listPaymentType.Find(r => r.SelectValue == p.PaymentTypeID.ToString())==null?"":listPaymentType.Find(r => r.SelectValue == p.PaymentTypeID.ToString()).SelectName);
            return viewModel;
        }



        public JsonResult GetSelectCustomerByParam(string keyword, bool IsAll, bool? onlyShowCash)
        {
            CustomerParam param = new CustomerParam();
            if (!string.IsNullOrWhiteSpace(keyword))
                param.CustomerCode = keyword;
            var list = _customerService.GetCustomerList(param.CustomerCode, IsAll).ToModelAsCollection<Customer, CustomerModel>();
            var listPaymentType = GetPaymentTypeList();
            if (onlyShowCash == true)
            {
                list.RemoveAll(p => p.PaymentTypeID != 3 && p.PaymentTypeID != 4);
            }
            list.ForEach(p => p.PaymentName = listPaymentType.Find(r => r.SelectValue == p.PaymentTypeID.ToString()) == null ? "" : listPaymentType.Find(r => r.SelectValue == p.PaymentTypeID.ToString()).SelectName);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> GetCustomerStatusList()
        {
            var List = new List<SelectListItem>();
            Customer.GetStatusList().Each(p =>
                List.Add(new SelectListItem()
                {
                    Value = p.ValueField,
                    Text = p.TextField_EN
                }));
            return List;
        }

        /// <summary>
        /// 获取客户类型
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> GetCustomerTypeList()
        {
            var list = new List<SelectListModel>();
            _freightService.GetCustomerTypeList().ForEach(p =>
                {
                    list.Add(new SelectListModel()
                        {
                            SelectName = p.CustomerTypeName,
                            SelectValue = p.CustomerTypeId.ToString()
                        });
                });
            return list;
        }
        /// <summary>
        /// 获取结算类型
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> GetPaymentTypeList()
        {
            var model = new List<SelectListModel>();
            //获取结算类型，状态1代表启用
            var list = _customerService.GetPaymentTypeList(1);
            list.Each(p =>
            {
                model.Add(new SelectListModel()
                {
                    SelectValue = p.PaymentTypeID.ToString(),
                    SelectName = p.PaymentName
                });
            });
            return model;
        }
        /// <summary>
        /// 获取客户资金变动类型
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> GetMoneyChangeTypeList()
        {
            var model = new List<SelectListModel>();
            var list = _customerService.GetMoneyChangeTypeInfo(1);
            list.Each(p =>
            {
                model.Add(new SelectListModel()
                {
                    SelectValue = p.MoneyChangeTypeID.ToString(),
                    SelectName = p.MoneyChangeTypeShortName
                });
            });
            return model;
        }
        /// <summary>
        /// 获取费用类型
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> GetFeeTypeList()
        {
            var model = new List<SelectListModel>();
            var list = _feeManageService.GetFeeTypeList(string.Empty, true);
            list.Each(p =>
            {
                model.Add(new SelectListModel()
                {
                    SelectValue = p.FeeTypeID.ToString(),
                    SelectName = p.FeeTypeName
                });
            });
            return model;
        }

        private CustomerRechargeListViewModel Data_Bind(CustomerRechargeListFilterModel filter)
        {
            var model = new CustomerRechargeListViewModel
                {
                    FilterModel = filter,
                    PagedList = _customerService.GetCustomerCreditPagedList(new CustomerCreditParam()
                        {
                            CustomerCode = filter.CustomerCode,
                            Page = filter.Page,
                            PageSize = filter.PageSize,
                            Status = filter.Status
                        }).ToModelAsPageCollection<CustomerCreditInfo, CustomerCreditModel>()
                };
            //状态
            model.StatusModels.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !model.FilterModel.Status.HasValue });
            CustomerCreditInfo.GetStatusList().ForEach(i =>
            {
                model.StatusModels.Add(new SelectListItem() { Text = i.TextField, Value = i.ValueField, Selected = model.FilterModel.Status.HasValue && model.FilterModel.Status.Value.ToString() == i.ValueField });
            });
            return model;
        }
        private CustomerAmountRecordListViewModel AmountRecordDataBind(CustomerAmountRecordListFilterModel filter)
        {
            var model = new CustomerAmountRecordListViewModel
                {
                    FilterModel = filter
                };
            decimal totalInFee = 0;
            decimal totalOutFee = 0;
            if (!string.IsNullOrWhiteSpace(filter.CustomerCode))
            {
                model.PagedList = _customerService.GetCustomerAmountRecordPagedList(new AmountRecordSearchParam()
                    {
                        CustomerCode = filter.CustomerCode,
                        EndDateTime = filter.EndDateTime,
                        StartDateTime = filter.StartDateTime,
                        Page = filter.Page,
                        PageSize = filter.PageSize
                    }, out totalInFee, out totalOutFee).ToModelAsPageCollection<CustomerAmountRecordExt, CustomerAmountRecordListModel>();
            }
            model.TotalInFee = totalInFee;
            model.TotalOutFee = totalOutFee;
            return model;
        }
    }

    public class ApiResult
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }


}
