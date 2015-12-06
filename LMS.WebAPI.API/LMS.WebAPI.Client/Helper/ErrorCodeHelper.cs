using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Client.Helper
{
    public enum ErrorCode
    {
        Error0000 = 0000,
        Error10000 = 10000,
        Error1001 = 1001,
        Error1002 = 1002,
        Error1003 = 1003,
        Error1004 = 1004,
        Error1005 = 1005,
        Error1006 = 1006,
        Error1011 = 1011,
        Error2001 = 2001,
        Error2002 = 2002,
        Error2003 = 2003,
        Error5001 = 5001,
        Error9999 = 9999,
        Error2004=2004,
        Error2005=2005,
        Error2006=2006,
        Error2007=2007,
        Error2008=2008,
        Error2009=2009,
        Error2010=2010,
        Error2011=2011,
        Error2012=2012,
        Error2013=2013,
        Error2019=2019,
        Error2020=2020,
        Error2021=2021,
        Error2022=2022,
        Error2023=2023,
        Error2024=2024,
        Error2025=2025,
        Error2026=2026,
        Error2027=2027,
        Error2028=2028,
        Error2029=2029
    }

    public enum TrackStatus
    {
        /// <summary>
        ///已出单
        /// </summary>
        Send=1,
        /// <summary>
        /// 待转单
        /// </summary>
        WaitOrder=2,
        /// <summary>
        /// 无跟踪号
        /// </summary>
        None=3
    }

    public enum Status
    {
        Fail = 0,
        Success = 1

    }

    public static class ErrorCodeHelper
    {
        public static string GetErrorCode(ErrorCode errorCode)
        {
            return (int) errorCode == 0 ? "0000" : ((int) errorCode).ToString();
        }
    }

}