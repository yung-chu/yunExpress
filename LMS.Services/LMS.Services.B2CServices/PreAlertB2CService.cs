using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using LMS.Services.B2CServices.Model;
using LighTake.Infrastructure.Common;

namespace LMS.Services.B2CServices
{
    public class PreAlertB2CService
    {
        public static XmlDocument SubmitB2C(Prealert model,string url)
        {

            var xml = new XmlDocument();
            var xn = new XmlSerializerNamespaces();
            xn.Add("", "");
            var result = WebUtil.PostDataToUrl(SerializeUtil.SerializeToXml(model, xn), url);
            //xml.LoadXml(SerializeUtil.SerializeToXml(model, xn));
            //var result = HttpRequestsFunctions.PostRetString(url, xml);
            var len = result.LastIndexOf('>') + 1;
            var starat = result.IndexOf('<');
            result = result.Substring(starat, len - starat);
            var x = new XmlDocument();
            x.LoadXml(result);
            return x;
            //return HttpRequestsFunctions.PostRetXml(url,xml);
        }
    }
}
