using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMS.Data.Repository;
using LMS.Data.Context;
using LMS.Data.Entity;
using System.Collections.Generic;

namespace LMS.Data.Repository.Test
{
    /// <summary>
    /// 跟踪号分配机制 单元测试 
    /// by daniel 2014-10-30
    /// </summary>
    [TestClass]
    public class TrackingNumberDetailInfoRepositoryTest
    {
        LMS_DbContext _db = new LMS_DbContext();
        TrackingNumberDetailInfoRepository _context = new TrackingNumberDetailInfoRepository(new LMS_DbContext());

        //内部方法:

        #region IsUsedTrackNumber
        [TestMethod]
        public void IsUsedTrackNumberTestFalse()
        {
            PrivateObject obj = new PrivateObject(_context);
            var result = obj.Invoke("IsUsedTrackNumber", _db, "aa");
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void IsUsedTrackNumberTestTrue()
        {
            PrivateObject obj = new PrivateObject(_context);
            var result = obj.Invoke("IsUsedTrackNumber", _db, "aaatk0821003");
            Assert.AreEqual(result, true);
        }
        #endregion

        #region GetTrackNumber
        [TestMethod]
        public void GetTrackNumberTest()
        {
            PrivateObject obj = new PrivateObject(_context);
            var result = obj.Invoke("GetTrackNumber", _db, 15, 1, string.Empty) as IList<TrackingNumberDetailInfo>;
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void GetTrackNumberTestWithCountryCode()
        {
            PrivateObject obj = new PrivateObject(_context);
            var result = obj.Invoke("GetTrackNumber", _db, 15, 1, "AD") as IList<TrackingNumberDetailInfo>;
            Assert.AreEqual(result.Count, 1);
        }
        #endregion


        //对外接口:
        [TestMethod]
        public void TrackNumberAssignStandardTest()
        {
            int requiredCount = 5;
            var result = _context.TrackNumberAssignStandard(15, requiredCount, "AD");
            Assert.AreEqual(result.Count, requiredCount);
        }


    }
}
