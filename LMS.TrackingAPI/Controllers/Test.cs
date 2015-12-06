using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.Data.Entity;
using LMS.WinForm.Client.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMS.TrackingAPI.Controllers
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestTrackingInfo()
        {
            var aa = 1;
            var bb = 2;
            aa = bb;
            //var orderTrackingRequest = new OrderTrackingRequestModel
            //{
            //    ShipmentID = 1,
            //    CustomerCode = "admin",
            //    TrackingNumber = "4329091572"
            //};
            //var success = HttpHelper.DoRequest<int>("http://localhost:18736/api/Tracking/AddOutTrackingInfo", EnumHttpMethod.POST, EnumContentType.Xml, orderTrackingRequest);

            //Assert.IsTrue(success.Value != 0);
        }
    }
}