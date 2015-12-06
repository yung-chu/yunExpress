using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LMS.WebApi.Client.Test.ServiceHttpResponse
{
    public class HttpResponseTests
    {
        private HttpClient client;

        private HttpResponseMessage response;

        [SetUp]
        public void SetUP()
        {
            client = new HttpClient();

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["serviceBaseUri"]);
            //response = client.GetAsync("contacts/get").Result;
        }

        public void GetWayBill()
        {

            response = client.GetAsync("WayBill/GetWayBill?wayBillNumber=GO1308100021").Result;

            //Task.WaitAll(client.GetAsync("WayBill/GetWayBill?wayBillNumber=GO1308100021"));
            //Task.WaitAll(client.PostAsync("",))
        }


        [Test]
        public void GetResponseIsSuccess()
        {
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        [Test]
        public void GetResponseIsJson()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Test]
        public void GetAuthenticationStatus()
        {
            Assert.AreNotEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        }
    }
}
