using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using LMS.Client.B2CSubmit.Model;
using LMS.Services.B2CServices;
using LMS.Services.B2CServices.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CSubmit.Controller
{
    public class B2CController
    {
        private static readonly string Url = ConfigurationManager.AppSettings["B2CUrl"];
        private static readonly string AuthenticationKey = ConfigurationManager.AppSettings["AuthenticationKey"];
        private static readonly string Email = ConfigurationManager.AppSettings["Email"];

        private static readonly int DDPRegisterShippingMethodId =
            Int32.Parse(ConfigurationManager.AppSettings["DDPRegisterShippingMethodId"]);

        private static readonly int DDPShippingMethodId =
            Int32.Parse(ConfigurationManager.AppSettings["DDPShippingMethodId"]);
        /// <summary>
        /// 提交到B2C
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static B2CPreAlertLog SubmitB2C(WayBillInfoModel model)
        {
            var result = new B2CPreAlertLog
                {
                    WayBillNumber = model.WayBillNumber,
                    PreAlertBatchNo = model.PreAlertBatchNo,
                    Status = 1
                };
            Log.Info("开始预报运单号为{0}到B2C".FormatWith(model.WayBillNumber));
            var prealert = new Prealert();
            prealert.AuthenticationKey = AuthenticationKey;
            prealert.LayoutPlatform = "L01";
            prealert.LayoutType = "P02";
            prealert.LayoutVersion = "2.0";
            prealert.PrealertReference = model.PreAlertBatchNo;
            prealert.PrealertValidation = new PrealertValidation()
                {
                    Timezone = "+8",
                    TotalShipments = 1,
                    MailAddressConfirmation = Email,
                    MailAddressError = Email
                };
            var street = (model.Street ?? "").ToDBC().StringSplitLengthWords(40);  
            var shipment= new Shipment
                {
                    CountryCodeOrigin = "CHN",
                    Currency = "EUR",
                    CustomsService = "DDP",
                    OrderNumber = model.WayBillNumber,
                    PurchaseDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    ShippingMethod =
                        model.ShippingMethodID == DDPRegisterShippingMethodId
                            ? (model.Weight > 2 ? "PPLUSDDP" : "PPAR")
                            : "TMAIL",
                    ShipmentAddress =
                        {
                            ConsigneeName = model.ConsigneeName.ToDBC().StripXML(),
                            AddressType = "DL1",
                            CompanyName = (model.CompanyName??"").ToDBC().StripXML(),
                            Street = street[0].StripXML(),
                            AdditionalAddressInfo = street.Count>1?street[1].StripXML():"",
                            CityOrTown = (model.CityOrTown ?? "").ToDBC().StripXML(),
                            StateOrProvince = (model.StateOrProvince ?? "").ToDBC().StripXML(),
                            ZIPCode = (model.ZIPCode).ToDBC().StripXML(),
                            CountryCode = model.CountryCode
                        },
                    ShipmentContact = {PhoneNumber = (model.PhoneNumber??"").ToDBC().StripXML(), EmailAddress = Email}
                };
            shipment.ShipmentPackages.Add(new ShipmentPackage()
                {
                    DimensionHeight = decimal.ToInt32(Math.Round(model.Height,0)),
                    DimensionLength = decimal.ToInt32(Math.Round(model.Length,0)),
                    DimensionWidth = decimal.ToInt32(Math.Round(model.Width,0)),
                    PackageBarcode = model.WayBillNumber,
                    PackageWeight =  decimal.ToInt32(Math.Round(model.Weight,3)*1000),
                    PackageNumber = 1
                });
            model.ApplicationInfos.ForEach(p => shipment.ShipmentContentCustoms.Add(new ShipmentContentCustoms()
                {
                    SKUCode = (p.SKUCode??"").ToDBC().StripXML(),
                    SKUDescription = (p.SKUDescription??"").ToDBC().StripXML(),
                    HSCode = (p.HSCode??"").ToDBC().StripXML(),
                    ImageUrl = (p.ImageUrl??"").ToDBC().StripXML(),
                    PackageNumber = 1,
                    Price = decimal.Round(p.Price,2),
                    Quantity = p.Quantity
                }));
            prealert.Shipments.Add(shipment);
            try
            {
                result.ShippingMethod = shipment.ShippingMethod;
                var xdoc= PreAlertB2CService.SubmitB2C(prealert, Url);
                Log.Info(xdoc.InnerText);
                XmlNode root = xdoc.SelectSingleNode("/Error");
                if (root != null && root.HasChildNodes)
                {
                    result.Status = 3;
                    var code = xdoc.SelectSingleNode("/Error/Code");
                    if (code != null)
                    {
                        result.ErrorCode = Int32.Parse(code.InnerText);
                        if (result.ErrorCode == 5001)
                        {
                            result.Status = 2;
                        }
                    }
                    var msg = xdoc.SelectSingleNode("/Error/Message");
                    if (msg != null)
                    {
                        result.ErrorMsg = msg.InnerText;
                    }
                    var detail = xdoc.SelectSingleNode("/Error/Details");
                    if (detail != null)
                    {
                        result.ErrorDetails = detail.InnerText;
                    }
                }
                else
                {
                    XmlNode newroot = xdoc.SelectSingleNode("/Prealert");
                    if (newroot != null && newroot.HasChildNodes)
                    {
                        result.Status = 2;
                        var prealterId = xdoc.SelectSingleNode("/Prealert/PreAlertID");
                        if (prealterId != null)
                        {
                            result.PreAlertID = Int32.Parse(prealterId.InnerText);
                        }
                    }
                    else
                    {
                        Log.Error(xdoc.InnerText);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info("完成预报运单号为{0}到B2C".FormatWith(model.WayBillNumber));
            return result;
        }
    }
}
