using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Results;
using LMS.Services.SF.Model;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.CommonServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.OperateLogServices;
using LMS.Services.OrderServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using LMS.Services.TrackingNumberServices;

namespace LMS.Controllers.OrderController
{
    public class OrderController : BaseController
    {
        //
        // GET: /Order/

        private IWorkContext _workContext;
        private IOrderService _orderService;
        private IFreightService _freightService;
        private ICustomerService _customerService;
        private IOperateLogServices _operateLogServices;
        private ITrackingNumberService _trackingNumberService;
        private readonly IInsuredCalculationService _insuredCalculationService;

        public OrderController(IWorkContext workContext, 
            IOrderService orderService, 
            IFreightService freightService, 
            IInsuredCalculationService insuredCalculationService,
            ICustomerService customerService,
            IOperateLogServices operateLogServices,
            ITrackingNumberService trackingNumberService
            )
        {
            _workContext = workContext;
            _orderService = orderService;
            _freightService = freightService;
            _insuredCalculationService = insuredCalculationService;
            _customerService = customerService;
            _operateLogServices = operateLogServices;
            _trackingNumberService = trackingNumberService;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View(OrderDataBind(new OrderFilterModel()));
        }


        private OrderFilterModel GetFormValue(FormCollection form)
        {
            var code = form["CustomerID"];
            var nickName = form["NickName"];
            OrderFilterModel filterModel = new OrderFilterModel();

            if (string.IsNullOrWhiteSpace(code))
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    filterModel.NickName = nickName;
                    filterModel.CustomerCode = code;
                }

                SetViewMessage(ShowMessageType.Error, "必须选择一个客户以及运输方式", true, false);
                return null;
            }

            return new OrderFilterModel
            {
                CustomerCode = code,
                NickName = nickName,
                FilePath = form["FilterModel.FilePath"]
            };
        }


        private OrderViewModel OrderDataBind(OrderFilterModel filterModel)
        {
            return new OrderViewModel { FilterModel = filterModel };
        }

        /// <summary>
        /// 保存文件到服务器上
        /// </summary>
        /// <returns>返回当前上传成功后的文件名</returns>
        private string SaveFileToService(string filePath)
        {
            try
            {
                string tempName = string.Empty;
                HttpFileCollectionBase files = HttpContext.Request.Files;
                for (int iFile = 0; iFile < files.Count; iFile++)
                {
                    HttpPostedFileBase postedFile = files[iFile];
                    tempName = Path.GetFileName(postedFile.FileName);
                    if (string.IsNullOrWhiteSpace(tempName))
                        throw new Exception("请选择需要上传的文件");
                    string fileExtension = Path.GetExtension(tempName);
                    if (fileExtension != ".xls" && fileExtension != ".xlsx" && fileExtension != ".et")
                    {
                        throw new Exception("只能上传xls类型的文件");
                    }
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
                Log.Exception(ex);
                throw new Exception("上传文件保存出错:" + ex.Message);
            }
        }


        private OrderModel ReadRowDataToModel(DataRow dr)
        {
            int index = 0;
            bool isEmpty = true;
            var orderModel = new OrderModel();
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.CustomerOrderID = dr[index].ToString().Trim();
                isEmpty = false;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingMethodCode = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.TrackingNumber = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;

            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.CountryCode = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingFirstName = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingLastName = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingCompany = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingAddress = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingCity = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingState = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingZip = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingPhone = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.ShippingInfoModel.ShippingTaxId = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderFirstName = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderLastName = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderCompany = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderAddress = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderCity = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderState = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderZip = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                orderModel.SenderInfoModel.SenderPhone = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            orderModel.IsReturn = false;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString()))
            {
                if (isEmpty) { isEmpty = false; }
                if (dr[index].ToString() == "Y".ToUpper())
                    orderModel.IsReturn = true;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString()))
            {
                orderModel.InsuredID = dr[index].ToString();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {

                orderModel.InsureAmountNumber = dr[index].ToString().Trim();
                if (isEmpty) { isEmpty = false; }
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString()))
            {
                if (isEmpty) { isEmpty = false; }
                int type;
                if (int.TryParse(dr[index].ToString(), out type))
                    orderModel.SensitiveTypeID = type;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                int type;
                switch (dr[index].ToString().Trim().ToUpper())
                {
                    case "GIFT":
                        type = CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        break;
                    case "SAMEPLE":
                        type = CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        break;
                    case "DOCUMENTS":
                        type = CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        break;
                    case "OTHERS":
                        type = CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        break;
                    default:
                        type = CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        break;
                }
                orderModel.AppLicationType = type;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                int type;
                if (int.TryParse(dr[index].ToString().Trim(), out type))
                    orderModel.PackageNumber = type;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                decimal length;
                if (decimal.TryParse(dr[index].ToString().Trim(), out length))
                    orderModel.Length = length;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                decimal width;
                if (decimal.TryParse(dr[index].ToString().Trim(), out width))
                    orderModel.Width = width;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                decimal heigth;
                if (decimal.TryParse(dr[index].ToString().Trim(), out heigth))
                    orderModel.Height = heigth;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }
                decimal weight;
                if (decimal.TryParse(dr[index].ToString().Trim(), out weight))
                    orderModel.Weight = weight;
            }
            index++;
            if (!string.IsNullOrWhiteSpace(dr[index].ToString().Trim()))
            {
                if (isEmpty) { isEmpty = false; }

                if (dr[index].ToString().Trim().ToUpper() == "Y")
                    orderModel.EnableTariffPrepay = true;
            }
            if (isEmpty) return null;
            return orderModel;
        }

        private List<OrderModel> ValidateExcelDataToList(DataTable dt, string customerCode, out int productColumnsCount)
        {
            //添加基本显示列
            productColumnsCount = (dt.Columns.Count - 32) / 8;
            var countryList = _freightService.GetCountrys();
            var orderModels = new List<OrderModel>();
            var InsureList = _insuredCalculationService.GetList();
            //客户对应的运输方式
            var shippingMethods = _freightService.GetShippingMethodListByCustomerCode(customerCode, false);
            //保险类型
            var insuredTypes = _orderService.GetInsuredCalculationListAll().ToModelAsCollection<InsuredCalculation, InsuredModel>();
            var sensitiveTypeInfos = _orderService.GetSensitiveTypeInfoListAll().ToModelAsCollection<SensitiveTypeInfo, SensitiveTypeInfoModel>();

            var listTariffPrepayFee =
                        _freightService.GetShippingMethodsTariffPrepay(customerCode);
            var customerOrderNumbers = new List<string>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {

                var orderModel = ReadRowDataToModel(dt.Rows[i]);
                if (orderModel == null) continue;
                //对以上相关不能为空字段进行数据验证
                if (string.IsNullOrWhiteSpace(orderModel.CustomerOrderID))
                {
                    orderModel.Message += "订单号不能为空<br/>";
                }
                else
                {
                    customerOrderNumbers.Add(orderModel.CustomerOrderID);
                }

                if (orderModel.Width == null || orderModel.Width <= 0)
                {
                    orderModel.Width = 1;
                }
                if (orderModel.Height == null || orderModel.Height <= 0)
                {
                    orderModel.Height = 1;
                }
                if (orderModel.Length == null || orderModel.Length <= 0)
                {
                    orderModel.Length = 1;
                }
                //else if (!string.IsNullOrWhiteSpace(orderModel.CustomerOrderID) && _orderService.IsExitOrderNUmber(orderModel.CustomerOrderID.ToUpper(), customerCode.ToUpper()))
                //    orderModel.Message += "订单号在系统中已经存在<br/>";
                if (orderModel.Weight == null || orderModel.Weight <= 0)
                {
                    orderModel.Message += "订单重量不能为零<br/>";
                }
                if (string.IsNullOrWhiteSpace((orderModel.ShippingInfoModel.ShippingFirstName + orderModel.ShippingInfoModel.ShippingLastName)))
                {
                    orderModel.Message += "收件人姓名不能为空<br/>";
                }
                if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingAddress))
                {
                    orderModel.Message += "收件人地址不能为空<br/>";
                }
                if (orderModel.ShippingInfoModel.ShippingAddress != null && orderModel.ShippingInfoModel.ShippingAddress.Length > 200)
                {
                    orderModel.Message += "收件人地址不能大于200字符<br/>";
                }
                if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingCity))
                {
                    orderModel.Message += "收件人城市不能为空<br/>";
                }
                if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingPhone))
                {
                    orderModel.Message += "收件人电话不能为空<br/>";
                }
                
                //解决运输方式不填 报空引用 yungchu
                ShippingMethod shipping = new ShippingMethod();
                if (orderModel.ShippingMethodCode != null)
                {
                    shipping = shippingMethods.FirstOrDefault(p => p.ShippingMethodCode.ToUpperInvariant() == orderModel.ShippingMethodCode.ToUpperInvariant());
                }

                if (shipping == null || orderModel.ShippingMethodCode == null)
                {
                    orderModel.Message += "运输方式不存在<br/>";
                }

                if (orderModel.ShippingMethodCode.Trim() == "LTPOST")
                {
                    //收货国家俄罗斯，邮编不能为空
                    if (orderModel.ShippingInfoModel.CountryCode.ToUpperInvariant() == "RU")
                    {
                        if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingZip))
                        {
                            orderModel.Message += "收件人邮编不能为空<br/>";
                        }
                    }
                }
                #region 中邮挂号福州
                //中邮挂号福州
                if (shipping != null && (shipping.ShippingMethodCode.Trim() == "CNPOST-FZ" || shipping.ShippingMethodCode.Trim() == "CNPOST-FYB"))
                {
                    if (orderModel.CustomerOrderID != null && orderModel.CustomerOrderID.Length > 30)
                    {
                        orderModel.Message += "订单号长度必须小于等于30<br/>";
                    }
                    //国家两位
                    if (orderModel.ShippingInfoModel.CountryCode != null && orderModel.ShippingInfoModel.CountryCode.Length != 2)
                    {
                        orderModel.Message += "国家简码必须是两位<br/>";
                    }
                    //收件人州或省
                    if (orderModel.ShippingInfoModel.ShippingState != null && orderModel.ShippingInfoModel.ShippingState.Length > 50)
                    {
                        orderModel.Message += "收件人省或州长度不能超过50<br/>";
                    }
                    //收件人城市
                    if (orderModel.ShippingInfoModel.ShippingCity != null && orderModel.ShippingInfoModel.ShippingCity.Length > 50)
                    {
                        orderModel.Message += "收件人城市长度不能超过50<br/>";
                    }
                    //收件人地址
                    if (orderModel.ShippingInfoModel.ShippingAddress != null && orderModel.ShippingInfoModel.ShippingAddress.Length > 120)
                    {
                        orderModel.Message += "收件人地址长度不能超过120<br/>";
                    }
                    //收件人邮编
                    if (orderModel.ShippingInfoModel.ShippingZip != null && orderModel.ShippingInfoModel.ShippingZip.Length > 12)
                    {
                        orderModel.Message += "收件人邮编长度不能超过12<br/>";
                    }
                    //收件人名字
                    string name = "";
                    if (orderModel.ShippingInfoModel.ShippingFirstName != null)
                    {
                        name += orderModel.ShippingInfoModel.ShippingFirstName;
                    }
                    if (orderModel.ShippingInfoModel.ShippingLastName != null)
                    {
                        name += orderModel.ShippingInfoModel.ShippingLastName;
                    }
                    if (name.Length > 64)
                    {
                        orderModel.Message += "收件人名字长度不能超过64<br/>";
                    }
                    //收件人电话
                    if (orderModel.ShippingInfoModel.ShippingPhone != null && orderModel.ShippingInfoModel.ShippingPhone.Length > 20)
                    {
                        orderModel.Message += "收件人电话长度不能超过20<br/>";
                    }
                    //发件人省份
                    if (orderModel.SenderInfoModel.SenderState != null && orderModel.SenderInfoModel.SenderState.Length > 20)
                    {
                        orderModel.Message += "发件人州省长度不能超过20<br/>";
                    }
                    //发件人城市
                    if (orderModel.SenderInfoModel.SenderCity != null && orderModel.SenderInfoModel.SenderCity.Length > 64)
                    {
                        orderModel.Message += "发件人城市长度不能超过64<br/>";
                    }
                    //发件人街道
                    if (orderModel.SenderInfoModel.SenderAddress != null && orderModel.SenderInfoModel.SenderAddress.Length > 120)
                    {
                        orderModel.Message += "发件人地址长度不能超过120<br/>";
                    }
                    //发件人邮编
                    if (orderModel.SenderInfoModel.SenderZip != null && orderModel.SenderInfoModel.SenderZip.Length > 6)
                    {
                        orderModel.Message += "发件人邮编长度不能超过6<br/>";
                    }
                    //发件人名字
                    string senderName = "";
                    if (orderModel.SenderInfoModel.SenderFirstName != null)
                    {
                        senderName += orderModel.SenderInfoModel.SenderFirstName;
                    }
                    if (orderModel.SenderInfoModel.SenderLastName != null)
                    {
                        senderName += orderModel.SenderInfoModel.SenderLastName;
                    }
                    if (senderName.Length > 20)
                    {
                        orderModel.Message += "发件人名字长度不能超过20<br/>";
                    }
                    //发件人电话
                    if (orderModel.SenderInfoModel.SenderPhone != null && orderModel.SenderInfoModel.SenderPhone.Length > 20)
                    {
                        orderModel.Message += "发件人电话长度不能超过20<br/>";
                    }
                }
                #endregion

                #region DHL 上传验证

                if (shipping != null && (shipping.ShippingMethodCode.Trim() == "HKDHL" || shipping.ShippingMethodCode.Trim() == "DHLCN" || shipping.ShippingMethodCode.Trim() == "DHLSG"))
                {
                    if (orderModel.InsureAmount != null && orderModel.InsureAmount.ToString().Length>14)
                    {
                        orderModel.Message += "保险金额长度不能超过14个字符<br/>";
                    }
                    else if (orderModel.InsureAmount != null && !Regex.IsMatch(orderModel.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                    {
                        orderModel.Message += "保险金额有数字组成，最多保留两位小数<br/>";
                    }

                    if (orderModel.ShippingInfoModel.ShippingCompany !=null && orderModel.ShippingInfoModel.ShippingCompany.Length > 35)
                    {
                        orderModel.Message += "收件人公司长度为0-35个字符<br/>";
                    }
                    try
                    {
                        if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingAddress) ||
                            orderModel.ShippingInfoModel.ShippingAddress.Length > 70 ||
                            orderModel.ShippingInfoModel.ShippingAddress.StringSplitLengthWords(35).Count > 2)
                        {
                            orderModel.Message += "收件人地址不能为空或者超长<br/>";
                        }
                    }
                    catch (Exception  ex)
                    {
                        orderModel.Message += "收件人地址格式不对";
                    }


                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingCity) || orderModel.ShippingInfoModel.ShippingCity.Length > 35)
                    {
                        orderModel.Message += "收件人城市长度为1-35个字符<br/>";
                    }

                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingState) || orderModel.ShippingInfoModel.ShippingState.Length > 35)
                    {
                        orderModel.Message += "收件人州/省长度为1-35个字符<br/>";
                    }
                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingZip) || orderModel.ShippingInfoModel.ShippingZip.Length > 12)
                    {
                        orderModel.Message += "收件人邮编长度为1-12个字符<br/>";
                    }

                    if (!string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingTaxId) &&
                        orderModel.ShippingInfoModel.ShippingTaxId.Length > 20)
                    {
                        orderModel.Message += "收件人税号不能超过20字符<br/>";
                    }
                    if ((orderModel.ShippingInfoModel.ShippingFirstName + orderModel.ShippingInfoModel.ShippingLastName).Length>35)
                    {
                        orderModel.Message += "收件人姓名不能超过35个字符<br/>";
                    }

                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingPhone))
                    {
                        orderModel.Message += "收件人电话不能为空<br/>";
                    }
                    else if(orderModel.ShippingInfoModel.ShippingPhone.Length>25)
                    {
                        orderModel.Message += "收件人电话不能超过25个字符<br/>";
                    }
                }

                #endregion

                #region EUB 上传验证

                if (shipping != null &&
                    (shipping.ShippingMethodCode.Trim() == "EUB_CS" || shipping.ShippingMethodCode.Trim() == "EUB-SZ" ||
                     shipping.ShippingMethodCode.Trim() == "EUB-FZ"))
                {
                    if (orderModel.CustomerOrderID != null && (orderModel.CustomerOrderID.Length > 32 || orderModel.CustomerOrderID.Length < 4))
                    {
                        orderModel.Message += "订单号长度必须为4-32个字符<br/>";
                    }
                    if (orderModel.Weight == null || orderModel.Height == null || orderModel.Length == null)
                    {
                        orderModel.Message += "体积(长*宽*高)为必填<br/>";
                    }

                    if (
                        (orderModel.ShippingInfoModel.ShippingFirstName + orderModel.ShippingInfoModel.ShippingLastName)
                            .Length > 256)
                    {
                        orderModel.Message += "收件人姓名不能超过256个字符<br/>";
                    }
                    if (orderModel.ShippingInfoModel.ShippingCity != null &&
                        orderModel.ShippingInfoModel.ShippingCity.Length > 128)
                    {
                        orderModel.Message += "收件人城市不能超过128个字符<br/>";
                    }

                    if (orderModel.ShippingInfoModel.ShippingState != null &&
                        orderModel.ShippingInfoModel.ShippingState.Length > 128)
                    {
                        orderModel.Message += "收件人州不能超过128个字符<br/>";
                    }

                    if (orderModel.ShippingInfoModel.ShippingZip != null)
                    {
                        if (orderModel.ShippingInfoModel.ShippingZip.Length > 16)
                        {
                            orderModel.Message += "收件人邮编不能超过16个字符<br/>";
                        }
                        else
                        {
                            switch (orderModel.ShippingInfoModel.CountryCode.ToUpperInvariant())
                            {
                                case "US":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip,"^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5}$)$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                                case "AU":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip, "^[0-9]{4}$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                                case "CA":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                                case "GB":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                                case "FR":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip, "^[0-9]{5}$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                                case "RU":
                                    if (!Regex.IsMatch(orderModel.ShippingInfoModel.ShippingZip, "^[0-9]{6}$"))
                                    {
                                        orderModel.Message += "邮编不合法";
                                    }
                                    break;
                            }
                        }
                    }
                    else if (orderModel.ShippingInfoModel.CountryCode.ToUpperInvariant() != "HK")
                    {
                        orderModel.Message += "邮编不能为空";
                    }

                }

                #endregion
                //Add By zhengsong
                //是否是需要计算偏远附加费 ，需要验证省/州，城市，邮编
                if (shipping != null && shipping.FuelRelateRAF)
                {
                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingZip))
                    {
                        orderModel.Message += "邮编不能为空<br/>";
                    }
                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingState))
                    {
                        orderModel.Message += "州/省不能为空<br/>";
                    }
                    if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingCity))
                    {
                        orderModel.Message += "城市不能为空<br/>";
                    }
                }

                //if (string.IsNullOrWhiteSpace(orderModel.ShippingMethodCode))
                //{
                //	orderModel.Message += "入仓运输代码为空<br/>";
                //}

                if (shipping != null && orderModel.ShippingMethodCode != null&&orderModel.Message.IsNullOrWhiteSpace())
                {
                    //顺丰荷兰小包
                    if (sysConfig.NLPOSTShippingMethodID.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries).Contains(shipping.ShippingMethodId.ToString()))
                    {
                        ValidationResult customerOrderResult = new ShippingInfoNlpostModelValidator().Validate(orderModel.ShippingInfoModel);
                        if (!customerOrderResult.IsValid)
                        {
                            foreach (var err in customerOrderResult.Errors)
                            {
                                orderModel.Message += err.ErrorMessage + "<br/>";
                            }
                        }
                    }
                    else if (sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(shipping.ShippingMethodId.ToString()))
                    {
                        //俄罗斯挂号、平邮运输方式ID
                        ValidationResult customerOrderResult = new OrderSfModelValidator().Validate(new OrderSfModel()
                            {
                                ShippingName =
                                    orderModel.ShippingInfoModel.ShippingFirstName +
                                    orderModel.ShippingInfoModel.ShippingLastName,
                                ShippingAddress = orderModel.ShippingInfoModel.ShippingAddress,
                                ShippingCity = orderModel.ShippingInfoModel.ShippingCity,
                                ShippingCompany = orderModel.ShippingInfoModel.ShippingCompany,
                                ShippingPhone = orderModel.ShippingInfoModel.ShippingPhone,
                                ShippingState = orderModel.ShippingInfoModel.ShippingState,
                                ShippingTel = orderModel.ShippingInfoModel.ShippingPhone,
                                ShippingZip = orderModel.ShippingInfoModel.ShippingZip,
                                CountryCode = orderModel.ShippingInfoModel.CountryCode
                            });
                        if (!customerOrderResult.IsValid)
                        {
                            foreach (var err in customerOrderResult.Errors)
                            {
                                orderModel.Message += err.ErrorMessage + "<br/>";
                            }
                        }
                    }
                }


                //客户是否开启关税预付 yungchu
                if (shipping != null)
                {
                    //List<TariffPrepayFeeShippingMethod> listTariffPrepayFee =
                    //    _freightService.GetShippingMethodsTariffPrepay(customerCode);
                    if (orderModel != null && orderModel.EnableTariffPrepay)
                    {
                        if (listTariffPrepayFee == null || listTariffPrepayFee.Count == 0)
                        {
                            orderModel.Message += "未开通关税预付权限，请联系业务";
                        }
                        else //客户是否开启该运输方式关税预付
                        {
                            List<int> listStr = new List<int>();
                            listTariffPrepayFee.ForEach(a => listStr.Add(a.ShippingMethodId));
                            if (listStr == null || !listStr.Contains(shipping.ShippingMethodId))
                            {
                                orderModel.Message += "未开通关税预付权限，请联系业务";
                            }
                        }
                    }
                }

                if (orderModel.ShippingMethodCode != null && shipping != null && shipping.ShippingMethodId == sysConfig.SpecialShippingMethodId)
                {
                    if (!string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingZip))
                    {
                        if (!Tools.CheckPostCode(orderModel.ShippingInfoModel.ShippingZip))
                        {
                            orderModel.Message += "邮编为6位纯数字且只能以1 2 3 4 6开头";
                        }
                    }
                    else
                    {
                        orderModel.Message += "邮编不能为空";
                    }
                    if (!string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.ShippingPhone))
                    {
                        if (!Tools.CheckShippingPhone(orderModel.ShippingInfoModel.ShippingPhone))
                        {
                            orderModel.Message += "电话号码最长不能超过11位数字";
                        }
                    }
                }
                if (shipping != null)
                {
                    orderModel.ShippingMethodName = shipping.ShippingMethodName;
                    orderModel.ShippingMethodID = shipping.ShippingMethodId;
                }
                else
                {
                    orderModel.ShippingMethodName = orderModel.ShippingMethodCode;
                }
                if (!string.IsNullOrWhiteSpace(orderModel.TrackingNumber) &&
                    _orderService.IsExitTrackingNumber(orderModel.TrackingNumber))
                {
                    orderModel.Message += "跟踪号在系统中已经存在<br/>";
                }
                if (string.IsNullOrWhiteSpace(orderModel.ShippingInfoModel.CountryCode))
                {
                    orderModel.Message += "收件人国家编码不能为空<br/>";
                }
                else
                {
                    if (countryList.FirstOrDefault(p => p.CountryCode.ToUpperInvariant() == orderModel.ShippingInfoModel.CountryCode.ToUpperInvariant()) ==
                        null)
                    {
                        orderModel.Message += "收件人国家编码不存在<br/>";
                    }
                }


                //if (!orderModel.SafetyType.HasValue)
                //    orderModel.Message += "必须要选择一个保险类型";
                if (!string.IsNullOrWhiteSpace(orderModel.InsuredID))
                {
                    string[] insuredID = orderModel.InsuredID.Split('_');
                    string safetyTypeid = insuredID[0];
                    int safetyType;

                    if (int.TryParse(safetyTypeid, out safetyType))
                    {
                        orderModel.SafetyType = safetyType;
                        if (!insuredTypes.Exists(p => p.InsuredID == safetyType))
                        {
                            orderModel.Message += "保险类型不存在<br/>";
                        }
                        else
                        {
                            if (orderModel.SafetyType != 2)
                            {
                                orderModel.InsureAmount = decimal.Parse(InsureList.Find(p => p.InsuredID == orderModel.SafetyType).InsuredCalculation1);
                            }
                            else
                            {
                                decimal insureAmount = 0;
                                if (decimal.TryParse(orderModel.InsureAmountNumber, out insureAmount))
                                {
                                    orderModel.InsureAmount = insureAmount;
                                    if (orderModel.InsureAmount <= 0)
                                    {
                                        orderModel.Message += "保险金额必须是大于零<br/>";
                                    }
                                }
                                else
                                {
                                    orderModel.Message += "保险金额必须为数字<br/>";
                                }
                            }
                        }

                    }
                    else
                    {
                        orderModel.Message += "保险类型不存在<br/>";
                    }
                }
                if (orderModel.SensitiveTypeID.HasValue &&
                    !sensitiveTypeInfos.Exists(p => p.SensitiveTypeID == orderModel.SensitiveTypeID.Value))
                {
                    orderModel.Message += "敏感货品类型不存在<br/>";
                }
                orderModel.ProductDetailModels = new List<ProductDetailModel>();
                orderModel.Number = i + 2;
                var productDetailModelValidator = new ProductDetailModelValidator();
                for (var j = 32; j + 8 <= dt.Columns.Count; j = j + 8)
                {
                    int n = 1;
                    var detailModel = new ProductDetailModel();
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString())
                        || !string.IsNullOrWhiteSpace(dt.Rows[i][j + 3].ToString())
                        || !string.IsNullOrWhiteSpace(dt.Rows[i][j + 4].ToString())
                        || !string.IsNullOrWhiteSpace(dt.Rows[i][j + 5].ToString()))
                    {
                        if (string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                        {
                            orderModel.Message += "第" + n + "列申报名称不能为空<br/>";
                        }

                        detailModel.ApplicationName = dt.Rows[i][j].ToString();
                        detailModel.PickingName = dt.Rows[i][j + 1].ToString();

                        if (orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FZ" || orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB" || orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOSTP_FZ")
                        {
                            //福州邮政申报信息判断

                            if (dt.Rows[i][j].ToString().Length > 60)
                            {
                                orderModel.Message += "第" + n + "列申报名称超长<br/>";
                            }
                            if (string.IsNullOrWhiteSpace(detailModel.PickingName) || detailModel.PickingName.Length>60)
                            {
                                orderModel.Message += "第" + n + "列申报中文名称不能为空或超长<br/>";
                            }
                        }

                        //DHL验证申报信息
                        if (orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" ||
                             orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" ||
                             orderModel.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG")
                        {
                            if (string.IsNullOrWhiteSpace(detailModel.ApplicationName) || detailModel.ApplicationName.Length > 60)
                            {
                                orderModel.Message += "第" + n + "列申报英文名称不能为空或超长<br/>";
                            }
                            else
                            {
                                if (Regex.IsMatch(detailModel.ApplicationName,
                                                  @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+"))
                                {
                                    orderModel.Message += "第" + n + "列申报英文名称不能包含特殊字符和汉字<br/>";
                                }
                            }
                        }

                        detailModel.HSCode = dt.Rows[i][j + 2].ToString();

                        if (string.IsNullOrWhiteSpace(dt.Rows[i][j + 3].ToString()))
                            orderModel.Message += "第" + n + "列申报数量不能为空<br/>";
                        else
                        {
                            int qty;
                            if (int.TryParse(dt.Rows[i][j + 3].ToString(), out qty))
                                detailModel.Quantity = qty;
                            else
                                orderModel.Message += "第" + n + "列申报数量格式不正确<br/>";
                        }

                        if (string.IsNullOrWhiteSpace(dt.Rows[i][j + 4].ToString()))
                            orderModel.Message += "第" + n + "列申报价格不能为空<br/>";
                        else
                        {
                            decimal price;
                            if (decimal.TryParse(dt.Rows[i][j + 4].ToString(), out price))
                                detailModel.UnitPrice = price;
                            else
                                orderModel.Message += "第" + n + "列申报价格格式不正确<br/>";
                        }
                        if (string.IsNullOrWhiteSpace(dt.Rows[i][j + 5].ToString()))
                            orderModel.Message += "第" + n + "列申报净重量不能为空<br/>";
                        else
                        {
                            decimal weigth;
                            if (decimal.TryParse(dt.Rows[i][j + 5].ToString(), out weigth))
                                detailModel.UnitWeight = weigth;
                            else
                                orderModel.Message += "第" + n + "列申报净重量格式不正确<br/>";
                        }

                        detailModel.ProductUrl = dt.Rows[i][j + 6].ToString();
                        detailModel.Remark = dt.Rows[i][j + 7].ToString();

                        //EUB 验证
                        if (shipping != null &&
                            (shipping.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" ||
                             shipping.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" ||
                             shipping.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                        {
                            if (detailModel.PickingName.Length > 64 || !Regex.IsMatch(detailModel.PickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+"))
                            {
                                orderModel.Message += "第" + n + "列申报中文名称超长或者必须包含两个中文字符<br/>";
                            }
                            if (detailModel.ApplicationName.Length > 128)
                            {
                                orderModel.Message += "第" + n + "列申报名称不能超过128个字符<br/>";
                            }
                        }

                        if ((orderModel.ShippingMethodCode == sysConfig.DDPRegisterShippingMethodCode ||
                             orderModel.ShippingMethodCode == sysConfig.DDPShippingMethodCode))
                        {
                            if (detailModel.HSCode == null || string.IsNullOrWhiteSpace(detailModel.HSCode))
                            {
                                orderModel.Message += "第" + n + "行申报信息海关编码不能为空<br/>";
                            }
                            if (detailModel.ProductUrl == null || string.IsNullOrWhiteSpace(detailModel.ProductUrl))
                            {
                                orderModel.Message += "第" + n + "行申报信息销售链接不能为空<br/>";
                            }
                            if (detailModel.Remark == null || string.IsNullOrWhiteSpace(detailModel.Remark))
                            {
                                orderModel.Message += "第" + n + "行申报信息备注不能为空<br/>";
                            }
                        }
                        else if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(orderModel.ShippingMethodID.ToString()))
                        {
                            var result= productDetailModelValidator.Validate(detailModel);
                            if (!result.IsValid)
                            {
                                foreach (var err in result.Errors)
                                {
                                    orderModel.Message += "第" + n + "行" + err.ErrorMessage + "<br/>";
                                }
                            }
                        }
                        else if (sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(orderModel.ShippingMethodID.ToString()))
                        {
                            var result =new ApplicationSfModelValidator().Validate(new ApplicationSfModel()
                                {
                                    ApplicationName = detailModel.ApplicationName,
                                    Qty = detailModel.Quantity,
                                    UnitPrice = detailModel.UnitPrice,
                                    UnitWeight = detailModel.UnitWeight??0
                                });
                            if (!result.IsValid)
                            {
                                foreach (var err in result.Errors)
                                {
                                    orderModel.Message += "第" + n + "行" + err.ErrorMessage + "<br/>";
                                }
                            }
                        }
                        orderModel.ProductDetailModels.Add(detailModel);
                    }
                    n++;
                }

                //欧洲专线上传 限制
                //Add By zhengsong
                if (sysConfig.DDPShippingMethodCode == orderModel.ShippingMethodCode.ToUpper() ||
                    sysConfig.DDPRegisterShippingMethodCode == orderModel.ShippingMethodCode.ToUpper() ||
                    sysConfig.EuropeShippingMethodCode == orderModel.ShippingMethodCode.ToUpper())
                {
                    // OrderNumber 上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于25
                    //Regex r = new Regex(@"^[A-Za-z0-9]{0,25}$");
                    //MatchCollection customerOrderNumber = r.Matches(orderModel.CustomerOrderID);
                    //if (customerOrderNumber.Count < 1 || customerOrderNumber[0].Value != orderModel.CustomerOrderID)
                    //{
                    //    orderModel.Message += "订单号[{0}]格式不符合要求<br/>".FormatWith(orderModel.CustomerOrderID);
                    //}

                    if (orderModel.CustomerOrderID.Length >25)
                    {
                       
                        orderModel.Message += "订单号[{0}]超过25个字符<br/>".FormatWith(orderModel.CustomerOrderID);
                    }

                    ////SKUCode1  上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于30
                    //Regex z = new Regex(@"^[A-Za-z0-9]{0,30}$");
                    //foreach (var row in orderModel.ProductDetailModels)
                    //{
                    //    MatchCollection remark = z.Matches(row.Remark);
                    //    if (remark.Count < 1 || remark[0].Value != row.Remark)
                    //    {
                    //        orderModel.Message += "申报信息[{0}]格式不符合要求<br/>".FormatWith(row.Remark);
                    //        break;
                    //    }
                    //}

                    ////PhoneNumber   只能是数字，不能出现其他字符，比如：&#43; &amp;
                    //Regex c = new Regex(@"^[0-9]{0,}$");
                    //MatchCollection shippingPhone = c.Matches(orderModel.ShippingInfoModel.ShippingPhone);
                    //if (shippingPhone.Count < 1 || shippingPhone[0].Value != orderModel.ShippingInfoModel.ShippingPhone)
                    //{
                    //    orderModel.Message += "件人电话[{0}]格式不符合要求<br/>".FormatWith(orderModel.ShippingInfoModel.ShippingPhone);
                    //}
                }


                // if (string.IsNullOrWhiteSpace(orderModel.Message))
                if (!string.IsNullOrWhiteSpace(orderModel.CustomerOrderID) && orderModels.Exists(p => p.CustomerOrderID == orderModel.CustomerOrderID))
                {
                    orderModel.Message += "订单号：{0}有相同<br/>".FormatWith(orderModel.CustomerOrderID);
                }
                if (!string.IsNullOrWhiteSpace(orderModel.TrackingNumber) &&
                    orderModels.Exists(p => p.TrackingNumber == orderModel.TrackingNumber))
                {
                    orderModel.Message += "跟踪号:{0}有相同<br/>".FormatWith(orderModel.TrackingNumber);
                }
                orderModels.Add(orderModel);
            }
            //Jess 2014-11-06 优化
            int pagesize = 100;
            int pageindex = 1;
            do
            {
                var list= _orderService.GetIsEixtCustomerOrderNumber(customerOrderNumbers.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList());
                if (list.Any())
                {
                    foreach (var model in orderModels.Where(p => list.Contains(p.CustomerOrderID)).ToList())
                    {
                        model.Message += "订单号在系统中已经存在<br/>";
                    }
                }
                pageindex++;
            } while (customerOrderNumbers.Count > (pageindex - 1) * pagesize);
            
            return orderModels;
        }

        private DataTable ReadExcelToModelList(string filePath)
        {
            var dt = ExcelHelper.ReadRepeatHeadToDataTable(filePath);
            if (dt == null || dt.Rows.Count == 0) throw new Exception("没有获取excel里面的数据");
            return dt;
        }


        [HttpPost]
        [ActionName("Upload")]
        [FormValueRequired("btnUpload")]
        public ActionResult btnUpload(FormCollection form)
        {

            OrderFilterModel filterModel = GetFormValue(form);
            if (filterModel == null)
                return View(OrderDataBind(new OrderFilterModel()));
            string filePath = sysConfig.TemporaryPath;
            try
            {
                //第一步先验证上传文件 ，成功后保存到指定路径并返回当前上传的文件名称
                //第二步对上传的文件进行读取，验证数据合法性(只针对于excel文件)
                //第三步根据上传的数据进行表的逻辑关联验证
                //第四步全部验证成功拼接结果集返回到列表展示
                //第五步确定上传保存当前数据并根据每一个地址生成一个订单
                var fileName = SaveFileToService(filePath);
                bool error = false;
                int productColumnsCount;
                var list = ValidateExcelDataToList(ReadExcelToModelList(filePath + fileName), filterModel.CustomerCode, out productColumnsCount);

                OrderViewModel viewModel = new OrderViewModel();
                filterModel.FilePath = filePath + fileName;
                viewModel.FilterModel = filterModel;
                viewModel.OrderModels = list;
                viewModel.ProductColumnsCount = productColumnsCount;


                var gorupbyCstmID = from a in list
                                    group a by a.CustomerOrderID
                                        into c
                                        select new { cid = c.Key, count = c.Count() };

                if (list.Any(p => !string.IsNullOrWhiteSpace(p.Message)))
                {
                    //SetViewMessage(ShowMessageType.Error, "出现错误不允许保存", true, false);
                    //viewModel.Error = true;
                    error = true;
                }

                foreach (var item in gorupbyCstmID)
                {
                    if (item.count > 1)
                    {
                        error = true;
                        var model = viewModel.OrderModels.First(p => p.CustomerOrderID == item.cid);
                        model.Message += model.CustomerOrderID + "出现重复";
                    }
                }
                if (error)
                {
                    SetViewMessage(ShowMessageType.Error, "出现错误不允许保存", true, false);
                    viewModel.Error = true;
                }
                else
                {
                    viewModel.Error = error;
                }
                viewModel.OrderModels = viewModel.OrderModels.OrderByDescending(p => p.Message).ThenBy(p => p.Number).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                SetViewMessage(ShowMessageType.Error, ex.Message, true, false);
                OrderViewModel viewModel = new OrderViewModel();
                viewModel.FilterModel = filterModel;
                return View(viewModel);
            }
        }

        [HttpPost]
        [ActionName("Upload")]
        [FormValueRequired("btnConfirm")]
        public ActionResult btnConfirm(FormCollection form)
        {

            OrderFilterModel filterModel = GetFormValue(form);
            if (filterModel == null)
                return View(OrderDataBind(new OrderFilterModel()));
            List<int> detailIds = new List<int>();
            Dictionary<string, string> error = new Dictionary<string, string>();
            string outValue;
            string errorShippingMethod = "";
            List<TrackingNumberDetailInfo> trackingNumberDetailInfos = new List<TrackingNumberDetailInfo>();
            // var dt = ReadExcelToModelList(filterModel.FilePath);
            int productColumnsCount = 0;
            var list = ValidateExcelDataToList(ReadExcelToModelList(filterModel.FilePath), filterModel.CustomerCode, out productColumnsCount);
            // var list = DisplayTopResult(dt, dt.Rows.Count, filterModel.ShippingMethodID.Value, filterModel.CustomerID.Value);
            List<WayBillInfo> wayBillList = new List<WayBillInfo>();
            //  Dictionary<string, string> skuGroupList = new Dictionary<string, string>();
            var shippingMethods = _freightService.GetShippingMethodListByCustomerCode(filterModel.CustomerCode, true);
            list.ForEach(p =>
            {
                WayBillInfo wayBill = new WayBillInfo();
                // wayBill.CustomerCode
                wayBill.CustomerCode = filterModel.CustomerCode.ToUpperInvariant();
                wayBill.IsReturn = false;
                wayBill.CountryCode = p.ShippingInfoModel.CountryCode.ToUpperInvariant();
                wayBill.CustomerOrderNumber = p.CustomerOrderID.ToUpperInvariant();
                wayBill.InShippingMethodID = p.ShippingMethodID;
                var shippingMethod = shippingMethods.FirstOrDefault(s => s.ShippingMethodId == p.ShippingMethodID);
                wayBill.InShippingMethodName = shippingMethod != null ? shippingMethod.ShippingMethodName : "";
                wayBill.CreatedBy = _workContext.User.UserUame;
                wayBill.CreatedOn = DateTime.Now;
                wayBill.LastUpdatedBy = _workContext.User.UserUame;
                wayBill.LastUpdatedOn = DateTime.Now;
                wayBill.InsuredID = p.SafetyType;
                wayBill.EnableTariffPrepay = p.EnableTariffPrepay;
                if (p.TrackingNumber != null)
                {
                    wayBill.TrackingNumber = p.TrackingNumber.ToUpperInvariant();
                }
                else
                {
                    wayBill.TrackingNumber = p.TrackingNumber;
                }
                //判断是否是需要系统生成跟踪号
                //if (item.InShippingMethodID.HasValue)
                //{
                if (string.IsNullOrWhiteSpace(wayBill.TrackingNumber) && _freightService.GetShippingMethod(p.ShippingMethodID).IsSysTrackNumber)
                {
                    //改成统一跟踪号分配机制 2014-10-23 daniel
                    var trackingNumberList = GetTrackNumber(p.ShippingMethodID, wayBill.CountryCode.ToUpperInvariant(), list);                     

                    if (trackingNumberList != null && trackingNumberList.Any())
                    {
                        wayBill.TrackingNumber = trackingNumberList;                      
                    }
                    else
                    {
                        if (error.TryGetValue(wayBill.InShippingMethodName, out outValue))
                        {
                            if (outValue == wayBill.CountryCode)
                            {
                            }
                        }
                        errorShippingMethod += "运输方式[" + wayBill.InShippingMethodName + "]国家[" +
                                               wayBill.CountryCode + "]";
                    }
                }
                wayBill.GoodsTypeID = 1;//默认是包裹
                wayBill.Status = WayBill.StatusToValue(WayBill.StatusEnum.Submitted);

                wayBill.Weight = p.Weight ?? 0;
                wayBill.Length = p.Length ?? 1;
                wayBill.Width = p.Width ?? 1;
                wayBill.Height = p.Height ?? 1;

                var shippingInfo = new ShippingInfo
                {
                    CountryCode = p.ShippingInfoModel.CountryCode.ToUpperInvariant(),
                    ShippingCompany = p.ShippingInfoModel.ShippingCompany,
                    ShippingCity = p.ShippingInfoModel.ShippingCity,
                    ShippingAddress = p.ShippingInfoModel.ShippingAddress,
                    ShippingFirstName = p.ShippingInfoModel.ShippingFirstName,
                    ShippingLastName = p.ShippingInfoModel.ShippingLastName,
                    ShippingPhone = p.ShippingInfoModel.ShippingPhone,
                    ShippingState = p.ShippingInfoModel.ShippingState,
                    ShippingZip = p.ShippingInfoModel.ShippingZip,
                    ShippingTaxId = p.ShippingInfoModel.ShippingTaxId
                };
                wayBill.ShippingInfo = shippingInfo;
                var senderInfo = new SenderInfo
                    {
                        CountryCode = "CN",
                        SenderFirstName = p.SenderInfoModel.SenderFirstName,
                        SenderLastName = p.SenderInfoModel.SenderLastName,
                        SenderCompany = p.SenderInfoModel.SenderCompany,
                        SenderAddress = p.SenderInfoModel.SenderAddress,
                        SenderCity = p.SenderInfoModel.SenderCity,
                        SenderState = p.SenderInfoModel.SenderState,
                        SenderZip = p.SenderInfoModel.SenderZip,
                        SenderPhone = p.SenderInfoModel.SenderPhone,
                    };
                wayBill.SenderInfo = senderInfo;
                var customerOrderInfo = new CustomerOrderInfo();
                if (!string.IsNullOrWhiteSpace(p.CustomerOrderID))
                {

                    customerOrderInfo.LastUpdatedBy = customerOrderInfo.CreatedBy = _workContext.User.UserUame;
                    customerOrderInfo.CustomerCode = filterModel.CustomerCode.ToUpperInvariant();
                    customerOrderInfo.LastUpdatedOn = customerOrderInfo.CreatedOn = DateTime.Now;
                    customerOrderInfo.CustomerOrderNumber = p.CustomerOrderID.ToUpperInvariant();
                    customerOrderInfo.ShippingMethodId = p.ShippingMethodID;
                    customerOrderInfo.ShippingMethodName = shippingMethod != null
                                                               ? shippingMethod.ShippingMethodName
                                                               : "";
                    customerOrderInfo.ShippingInfo = shippingInfo;
                    customerOrderInfo.SenderInfo = senderInfo;
                    customerOrderInfo.IsInsured = p.SafetyType.HasValue;
                    customerOrderInfo.InsuredID = p.SafetyType;
                    if (p.TrackingNumber != null)
                    {
                        customerOrderInfo.TrackingNumber = p.TrackingNumber.ToUpperInvariant();
                    }
                    else
                    {
                        customerOrderInfo.TrackingNumber = wayBill.TrackingNumber;
                    }
                    customerOrderInfo.GoodsTypeID = 1;//默认是包裹
                    customerOrderInfo.Weight = p.Weight ?? 0;
                    customerOrderInfo.Width = p.Width ?? 1;
                    customerOrderInfo.Height = p.Height ?? 1;
                    customerOrderInfo.Length = p.Length ?? 1;
                    customerOrderInfo.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Submitted);
                    customerOrderInfo.InsureAmount = p.InsureAmount == null ? 0 : p.InsureAmount.Value;
                    customerOrderInfo.AppLicationType = p.AppLicationType == null ? CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others) : p.AppLicationType.Value;
                    customerOrderInfo.PackageNumber = p.PackageNumber == null ? 1 : p.PackageNumber.Value;
                    customerOrderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus()
                        {
                            CreatedOn = DateTime.Now,
                            Remark = "后台批量导入",
                            Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Submitted)
                        });
                    wayBill.CustomerOrderInfo = customerOrderInfo;

                }
                p.ProductDetailModels.Each(d =>
                {
                    var applicationInfo = new ApplicationInfo
                        {
                            ApplicationName = d.ApplicationName,
                            Qty = d.Quantity,
                            HSCode = d.HSCode,
                            UnitPrice = d.UnitPrice,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            UnitWeight = d.UnitWeight,
                            PickingName = d.PickingName,
                            Remark = d.Remark,
                            ProductUrl = d.ProductUrl,
                        };
                    if (!string.IsNullOrWhiteSpace(p.CustomerOrderID))
                    {
                        applicationInfo.CustomerOrderInfo = customerOrderInfo;
                    }
                    wayBill.ApplicationInfos.Add(applicationInfo);
                });
                wayBillList.Add(wayBill);


            });
            try
            {

                if (!string.IsNullOrWhiteSpace(errorShippingMethod))
                {
                    SetViewMessage(ShowMessageType.Error, string.Format("保存失败，{0}的跟踪号数量不足。", errorShippingMethod), false, false);
                    return View(OrderDataBind(filterModel));
                }
                if (_orderService.BatchCreateWayBillInfo(wayBillList) 
                    //&& 
                    //_orderService.UpdateTrackingNumberDetail(trackingNumberDetailInfos) 
                    //不用在维护跟踪号状态 2014-10-23 daniel
                    )
                {
                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //foreach (var item in wayBillList)
                    //{
                    //	BizLog bizlog = new BizLog()
                    //	{
                    //		Summary = "[运单上传]",
                    //		KeywordType = KeywordType.WayBillNumber,
                    //		Keyword = item.WayBillNumber,
                    //		UserCode = _workContext.User.UserUame,
                    //		UserRealName = _customerService.GetCustomer(_workContext.User.UserUame).Name ?? _workContext.User.UserUame,
                    //		UserType = UserType.LMS_User,
                    //		SystemCode = SystemType.LMS,
                    //		ModuleName = "运单上传"
                    //	};

                    //	_operateLogServices.WriteLog(bizlog, item);

                    //}
                    //#endregion

                    SetViewMessage(ShowMessageType.Success, "上传成功", false, false);
                }
                else
                {
                    SetViewMessage(ShowMessageType.Error, "上传失败", false, false);
                }

                return View(OrderDataBind(filterModel));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                SetViewMessage(ShowMessageType.Error, ex.Message, false, false);
                return View(OrderDataBind(filterModel));
            }

            return View();
        }

        private string GetTrackNumber(int shippingMethodId, string countryCode, List<OrderModel> list)
        {
            try
            {
                while (true)
                {
                    var trackingNumbers = _trackingNumberService.TrackNumberAssignStandard(shippingMethodId, 1, countryCode);
                    if (trackingNumbers != null && trackingNumbers.Any())
                    {
                        var tn = trackingNumbers.FirstOrDefault();
                        if (list.Any(t => t.TrackingNumber == tn))
                        {
                            //重复
                        }
                        else
                        {
                            return trackingNumbers[0];//不重复
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }
    }
}



