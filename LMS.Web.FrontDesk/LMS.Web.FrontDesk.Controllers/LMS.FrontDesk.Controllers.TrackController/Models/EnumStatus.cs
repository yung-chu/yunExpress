using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.FrontDesk.Controllers.TrackController.Models
{
    public class EnumStatus
    {


        public enum InfoStateEnum
        {
            /// <summary>
            /// 未知
            /// </summary>
            UnKnow = 0,

            /// <summary>
            /// 未成功请求
            /// </summary>
            UnSpidered = 1,

            /// <summary>
            /// 已成功请求
            /// </summary>
            Spidered = 2
        }


        public enum PackageStateEnum
        {
            /// <summary>
            /// 未知
            /// </summary>
            UnKnow = 0,

            /// <summary>
            /// 不存在
            /// </summary>
            NotExist = 1,

            /// <summary>
            /// 运输中
            /// </summary>
            InTransit = 2,

            /// <summary>
            /// 已签收
            /// </summary>
            Delivered = 3
        }

        public enum WaybillEnum
        {
            /// <summary>
            /// 已提交
            /// </summary>
            Submitted = 3,

            /// <summary>
            /// 已收货
            /// </summary>
            Have = 4,

            /// <summary>
            /// 待转单
            /// </summary>
            WaitOrder = 8,

            /// <summary>
            /// 发货运输中
            /// </summary>
            Send = 5,

            /// <summary>
            /// 已删除
            /// </summary>
            Delete = 6,

            /// <summary>
            /// 退货在仓
            /// </summary>
            ReGoodsInStorage = 9,

            /// <summary>
            /// 已退回
            /// </summary>
            Return = 7,

            /// <summary>
            /// 已签收
            /// </summary>
            Delivered = 10,

        }



        public string PackageState(int status)
        {
            string packageState = string.Empty;
            switch (status)
            {
                case 0:
                    packageState = "未知";
                    break;
                case 1:
                    // packageState = "不存在";
                    packageState = "已发货";
                    break;
                case 2:
                    packageState = "运输中";
                    break;
                case 3:
                    packageState = "已签收";
                    break;
            }

            return packageState;
        }



        public string InfoState(int status)
        {
            string packageState = string.Empty;
            switch (status)
            {
                case 0:
                    packageState = "未知";
                    break;
                case 1:
                    packageState = "未成功请求";
                    break;
                case 2:
                    packageState = "已成功请求";
                    break;
            }

            return packageState;
        }

        public string WaybillState(int status)
        {
            string waybillState = string.Empty;
            switch (status)
            {
                case 3:
                    waybillState = "已提交";
                    break;
                case 4:
                    waybillState = "已收货";
                    break;
                case 8:
                    waybillState = "待转单";
                    break;
                case 5:
                    waybillState = "发货运输中";
                    break;
                case 6:
                    waybillState = "已删除";
                    break;
                case 9:
                    waybillState = "退货在仓";
                    break;
                case 7:
                    waybillState = "已退回";
                    break;
                case 10:
                    waybillState = "已签收";
                    break;
            }

            return waybillState;
        }


    }
}
