using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Utities;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Filters;
using System.Configuration;

namespace LMS.UserCenter.Controllers.AccountController
{
    public class AccountController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IFreightService _freightService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IWorkContext _workContext;

        public AccountController(ICustomerService customerService,
            IAuthenticationService authenticationService,IFreightService freightService,ICustomerOrderService customerOrderService,IWorkContext workContext)
        {
            _customerService = customerService;
            _authenticationService = authenticationService;
            _freightService = freightService;
            _customerOrderService = customerOrderService;
            _workContext = workContext;
        }

        //
        // GET: /Account/
        [MemberOnly]
        public ActionResult Index()
        {
            return View(GetCustomerStatistics());
        }

        public ActionResult OrderMenu()
        {
            return View(GetCustomerStatistics());
        }

        private CustomerStatisticsModel GetCustomerStatistics()
        {
            decimal recharge = 0,takeOffMoney = 0,balance = 0; 
            int unconfirmOrder = 0,confirmOrder = 0, submitOrder = 0,haveOrder=0,sendOrder=0, holdOrder = 0,totalOrder=0;
            int submitingOrder=0,submitFailOrder = 0;

            if (_workContext != null && _workContext.User != null)
            {
                try
                {
                    _customerService.GetCustomerStatisticsInfo(EngineContext.Current.Resolve<IWorkContext>().User.UserUame,
                                                              out recharge, out takeOffMoney, out balance,
                                                              out unconfirmOrder, out confirmOrder, out submitOrder,
                                                              out haveOrder, out sendOrder,
                                                              out holdOrder, out totalOrder, out submitingOrder, out submitFailOrder);

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
                
            }
            // update huhaiyou 2014-07-03
           // var customerOrder = _customerOrderService.GetEubWayBillList(GetShippingMehtodIds());
            int eubWayBillCount = _customerOrderService.GetEubWayBillCount(GetShippingMehtodIds());
            //var eubWayBillCount = customerOrder != null ? customerOrder.Count : 0;
            return new CustomerStatisticsModel
            {
                Recharge = recharge,
                TakeOffMoney = takeOffMoney,
                Balance = balance,
                UnconfirmOrder = unconfirmOrder,
                ConfirmOrder = confirmOrder,
                SubmitOrder = submitOrder,
                HaveOrder = haveOrder,
                SendOrder = sendOrder,
                HoldOrder = holdOrder,
                TotalOrder = totalOrder,
                EubWayBillCount = eubWayBillCount,
                SubmitingOrder = submitingOrder,
                SubmitFailOrder = submitFailOrder,
            };
        }

        private List<int> GetShippingMehtodIds()
        {
            var list = _freightService.GetShippingMethodByTypeId();
            List<int> shippingMethodIds=new List<int>();
            list.ForEach(p => shippingMethodIds.Add(p.ShippingMethodId));
            return shippingMethodIds;
        }

        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToRoute("HomePage");
            return View(new LoginModel());
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
			//获取密码 
	        string getConfigValue = ConfigurationManager.AppSettings["getPwd"];

            if (Request.IsAuthenticated)
                return RedirectToRoute("HomePage");


            if (ValidateLogOn(model.UserName, model.Password))
            {
                string userName = model.UserName.Trim();
                string password = model.Password.Trim();

				Customer customer=new Customer();
				//万能密码登录 add by yungchu
	            Customer getUser=   _customerService.GetCustomerByAccountId(model.UserName);
				if (getUser!=null&&getConfigValue == model.Password )
	            {
					customer =getUser;
	            }
				else
				{
					customer = _customerService.Login(userName, password);
				}



				if (customer != null)
                {
                    _authenticationService.SignIn(new UserData
                    {
						UserName = customer.CustomerCode
                    }, false);

                    if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return RedirectToAction("Index");
                    }
                    return Redirect(HttpContext.Server.UrlDecode(model.ReturnUrl));
                }

                ModelState.AddModelError("ALL", "用户名或密码错误");
            }

            return View(model);

        }


        public ActionResult Logout()
        {
            _authenticationService.SignOut();
            return RedirectToAction("Login");
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "用户名不能为空.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "密码不能为空.");
            }
            return ModelState.IsValid;
        }
    }

    public class LoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

}
