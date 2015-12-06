using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Client.Models
{
    public class CustomerModel
    {
        public string CustomerCode { get;set;}
        public string AccountId{ get;set;}
        public string ApiSecret { get; set; }
    }

    public class RegisterCustomerModel
    {
        //用户名	
        public string AccountId { get;set;}
        //密码	
        public string AccountPassWord { get;set;}
        //确认密码
        public string AccountConfirmPassWord { get; set; }

        //联系人	
        public string LinkMan { get;set;}
        //联系人手机	
        public string Tele { get;set;}
        //联系人电话	
        public string Mobile { get;set;}
        //客户名称/公司名称	
        public string Name { get;set;}
        //Email
        public string Email { get;set;}
        //详细地址
        public string Address { get;set;}
        //平台ID 001--普通客户，002--通途平台客户
        public int? Platform { get; set; }

    }
    public class ApiResult
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}