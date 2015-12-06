using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Client.UpdateReceivingPrice.Model;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Client.UpdateReceivingPrice.Controller
{
    public class FreightController
    {
        private static readonly string _baseLISUrl = System.Configuration.ConfigurationManager.AppSettings["LISUrl"].ToString();
        public static PriceProviderResult GetFreightPrice(WayBillImportModel model)
        {
            var result = new PriceProviderResult();
            try
            {
                var list = HttpHelper.DoRequest<PriceProviderResult>(_baseLISUrl + "API/LIS/PostCustomerPriceAuto",
                                                                     EnumHttpMethod.POST, EnumContentType.Json, model);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + _baseLISUrl + "API/LIS/PostCustomerPriceAuto");
                Log.Exception(ex);
                result.CanShipping = false;
                result.Message = "";
            }
            return result;
        }
    }
}
