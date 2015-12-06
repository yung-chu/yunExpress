using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using LMS.Data.Entity;
using LMS.Services.FeeManageServices;
using LMS.Services.OrderServices;
using LMS.Services.CustomerServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;

namespace LMS.WebAPI.Controllers
{
    public class OrdersController : BaseApiController
    {
        //private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IFeeManageService _feeManageService;

        public OrdersController(ICustomerService customerService, IFeeManageService feeManageService)
        {
            _feeManageService = feeManageService;
            _customerService = customerService;
        }
        //public ValuesController(){} 
        // GET api/orders/5
        public FeeInfoModel Get(string id)
        {

            decimal alltotalfee = 0;
            var lst = _feeManageService.GetInFeeInfoPagedList(new InFeeListParam()
                 {
                     CustomerCode = "C14169",    //代购帐号  DG
                     SearchWhere = 2,
                     SearchContext = id,
                     CountryCode = null,
                     Page = 1,
                     PageSize = 20
                 }, out alltotalfee);

            if (lst.InnerList != null)
            {
                if (lst.InnerList.Count > 0)
                {
                    var obj = new FeeInfoModel()
                        {
                            CustomerOrderNumber = id,
                            SettleWeight = lst.InnerList[0].SettleWeight??0,
                            TotalFee = lst.InnerList[0].TotalFee??0,
                            TrackingNumber = lst.InnerList[0].TrackingNumber
                        };
                    return obj;
                }
            }

            return null;
            // return  _orderService.
        }  

        // GET api/values/5
        //public string Get(int id)
        //{
        //    return id.ToString();
        //}        
        // POST api/values
        public void Post([FromBody]string value)
        {
        }        
        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }        
        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }


}
