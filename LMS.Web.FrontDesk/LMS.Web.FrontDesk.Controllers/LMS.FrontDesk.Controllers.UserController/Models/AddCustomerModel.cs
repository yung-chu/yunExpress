using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LMS.FrontDesk.Controllers.UserController.Models
{
    //用户注册
    public class AddCustomerModel
    {
        public AddCustomerModel()
        {
            ShowCategoryListModel = new ShowCategoryListModel();
        }

	    public int GetId { get; set; }
        public ShowCategoryListModel ShowCategoryListModel { get; set; }


        [RegularExpression(@"^[A-z\d]{5,18}$",ErrorMessage = "用户名格式为:字母a~z,数字0~9,且不能小于5位数")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public string AccountID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        [RegularExpression(@"^[A-z\d]{6,}$", ErrorMessage = "密码格式为:字母a~z,数字0~9,且不能小于6位数")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        [Compare("Password", ErrorMessage = "两次输入的密码不相同")]
        public string NextPassword { get; set; }

	    [Required(ErrorMessage = "客户名称不能为空")]
        public string Name { get; set; }
	    public string CustomerCode { get; set; }

	    //[RegularExpression(@"\d{11}$", ErrorMessage = "手机号需要11位数字组成")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "手机号不能为空")]
        //public string Mobile { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "联系电话不能为空")]
        //[RegularExpression(@"(\d{3}-\d{8}|\d{4}-\d{7})|^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$", ErrorMessage = "请输入正确的手机和电话")]
        public string Mobile { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "地址不能为空")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "联系人不能为空")]
        public string LinkMan { get; set; }
        //[Required(ErrorMessage = "Email不能为空")]
       // [RegularExpression(@"^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$", ErrorMessage = "请输入正确的Email")]
        public string Email { get; set; }

        public string QQ { get; set; }

	    public int? CustomerRule { get; set; }

    }

    public class CategoryModel
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string ParentPath { get; set; }
        public int Level { get; set; }
        public int Sort { get; set; }
        public string Pic { get; set; }
        public string Description { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string EnglishName { get; set; }
    }
}
