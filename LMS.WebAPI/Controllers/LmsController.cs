using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Services.FeeManageServices;
using LMS.Services.FinancialServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.UserServices;
using LMS.WebAPI.Model;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;

namespace LMS.WebAPI.Controllers
{
    public class LmsController : BaseApiController
    {
        private readonly IReturnGoodsService _returnGoodsService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IFeeManageService _feeManageService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IFinancialService _financialService;

        public LmsController(IReturnGoodsService returnGoodsService,
            IUserService userService,
            IWorkContext workContext,
            IFeeManageService feeManageService,
            IAuthenticationService authenticationService,
            IFinancialService financialService)
        {
            _returnGoodsService = returnGoodsService;
            _userService = userService;
            _workContext = workContext;
            _feeManageService = feeManageService;
            _authenticationService = authenticationService;
            _financialService = financialService;
        }

        //Post: api/lms/PostLogin/
        public UserModel PostLogin(LoginModel loginModel)
        {
            if (loginModel == null)
                return null;
            string msg = string.Empty;
            User user = _userService.ValidateUser(loginModel.UserName, loginModel.Pwd, out msg);
            if(user==null)
                return null;
            UserModel userModel =new UserModel()
                {
                    CustomerId=user.CustomerId,
                    UserUame = user.UserUame,
                    RealName = user.RealName
                };
            return userModel;
        }

        //Post: api/lms/PostBatchAddReturnGoods/
        public ResponseResultModel PostBatchAddReturnGoods(List<ReturnGoodsExt> list)
        {
            ResponseResultModel result = new ResponseResultModel() { IsSuccess = false, Message = "提交失败" };
            try
            {
                if (list != null && list.Count > 0)
                {
                    _returnGoodsService.BatchAddReturnGoods(list);
                    result.IsSuccess = true;
                    result.Message = "提交成功";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "数据不可用或没有数据";
                }
              
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return result;
        }

        //// Get: api/lms/GetInFeeTotalInfo/
        //public InFeeTotalInfoExtModel GetInFeeTotalInfo(string number)
        //{
        //    decimal totalfee = 0;
        //    var list =
        //        _feeManageService.GetInFeeTotalInfoList(
        //            new InFeeTotalListParam() {Number = number.Trim()},
        //            out totalfee);
        //    if (list != null && list.Count > 0)
        //        return list.FirstOrDefault().ToModel<InFeeTotalInfoExtModel>();
        //    return null;
        //}

        // Get: api/lms/GetInFeeTotalInfo/
        public InFeeTotalInfoExtModel GetInFeeTotalInfo(string number)
        {
            var list =
                _financialService.GetInFeeTotalInfo(number);
            if (list != null )
                return list.ToModel<InFeeTotalInfoExtModel>();
            return null;
        }

        public string Get(string id)
        {
            return id;
        }

    }
}
