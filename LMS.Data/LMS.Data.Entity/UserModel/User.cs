using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class User
    {
        public User()
        {
            Permissions = new List<Permission>();
            Menu = new PermissionMenu();
        }

        public System.Guid CustomerId { get; set; }

        /// <summary>
        /// 登陆名称
        /// </summary>
        public string UserUame { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName
        {
            get;
            set;
        }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName
        {
            get;
            set;
        }

        /// <summary>
        /// 默认角色
        /// </summary>
        public Nullable<int> RoleID
        {
            get;
            set;
        }

        /// <summary>
        ///  性别
        /// </summary>
        public string Sex
        {
            get;
            set;
        }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday
        {
            get;
            set;
        }

        /// <summary>
        /// 岗位
        /// </summary>
        public string Duty
        {
            get;
            set;
        }

        /// <summary>
        /// 职称
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 网络即时通讯ID
        /// </summary>
        public string OICQ
        {
            get;
            set;
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 状态 
        /// </summary>
        public int Enabled { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 系统代码
        /// </summary>
        public string SystemCode { get; set; }

        public IList<Permission> Permissions { get; set; }

        public PermissionMenu Menu { get; set; }
    }

    //已经禁用
    public class Process
    {
        public string ProcessSubmit = "Order Processing";

        public string ProcessInScann = "Shipment picked up";

        public string ProcessOutScann = "outStorage Scan";

        public string ProcessReturn = "Returned to sender";

        public string ProcessAdderss = "ShenZhen - China";
    }
}
