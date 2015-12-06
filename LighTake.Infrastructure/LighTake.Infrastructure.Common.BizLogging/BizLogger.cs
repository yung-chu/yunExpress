using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.BizLogging.BizLogWcf;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LighTake.Infrastructure.Common.Logging;
using Newtonsoft.Json;
using System.Web;
using System.Net;

namespace LighTake.Infrastructure.Common.BizLogging
{
    /// <summary>
    /// 业务日志
    /// </summary>
    public class BizLogger
    {
        /// <summary>
        /// 写业务日志
        /// </summary>
        public static void WriteLog(BizLog log)
        {
            if (log == null) return;
            string info = string.Empty;
            try
            {
                BizLogModel model = Map(log);
                info = Check(model);
                if (string.IsNullOrEmpty(info))
                {
                    WriteLog2WCF(model);
                    return;
                }
            }
            catch (Exception ex)
            {
                var exReal = ex.InnerException == null ? ex : ex.InnerException;
                info = exReal.ToString();
            }

            if (string.IsNullOrEmpty(info))
            {
#if DEBUG
                throw new BusinessLogicException(info);
#else
                Log.Error(string.Format("写[业务日志]出错,<br/>原因:{0},<br/>日志数据:{1}."
                    , info
                    , JsonConvert.SerializeObject(log)));
#endif
            }

        }

        /// <summary>
        /// B/S系统写业务日志
        /// </summary>
        /// <typeparam name="T">日志Details实体类型</typeparam>
        /// <param name="log">日志</param>
        /// <param name="details">日志Details实体</param>
        public static void WriteLogBS<T>(BizLog log, T details)
        {
            if (log == null) return;

            if (details != null)
            {
                try
                {
                    log.Details = JsonConvert.SerializeObject(details);
                }
                catch(Exception ex)
                {
                    string error =string.Concat("写业务日志失败,原因序列化失败,详细错误信息:" , ex.Message);
#if DEBUG
	                Log.Exception(ex);
	                // throw new BusinessLogicException(error);                    
#else
                    Log.Error(error);
                    return; //跳出
#endif

                }
            }
            
            if (HttpContext.Current != null)
            {
                if (string.IsNullOrEmpty(log.IP))
                {
                    try
                    {
                        log.IP = HttpContext.Current.Request.UserHostAddress;
                    }
                    catch { }
                }

                if (string.IsNullOrEmpty(log.URL))
                {

                    try
                    {
                        log.URL = HttpContext.Current.Request.Url.AbsolutePath;
                    }
                    catch { }

                }
            }            

            WriteLog(log);
        }

        /// <summary>
        /// C/S系统写业务日志
        /// </summary>
        /// <typeparam name="T">日志Details实体类型</typeparam>
        /// <param name="log">日志</param>
        /// <param name="details">日志Details实体</param>
        public static void WriteLogCS<T>(BizLog log, T details)
        {
            if (log == null) return;

            if (details != null)
            {
                try
                {
                    log.Details = JsonConvert.SerializeObject(details);
                }
                catch (Exception ex)
                {
                    string error = string.Concat("写业务日志失败,原因序列化失败,详细错误信息:", ex.Message);
#if DEBUG
                    throw new BusinessLogicException(error);
#else
                    Log.Error(error);
                    return; //跳出
#endif

                }
            }

            if (string.IsNullOrEmpty(log.IP))
            {
                try
                {
                    log.IP = GetIP();
                }
                catch { }
            }
            if (string.IsNullOrEmpty(log.Mac))
            {

                try
                {
                    log.Mac = GetMac();
                }
                catch { }
            }

            WriteLog(log);
        }


		public static PagedList<BizLogModel> QueryBizLogInfo(BizLog log, int pageIndex, int pageSize,DateTime?startTime,DateTime?endTime, out int totalRecords)
		{
			BizLogModel[] getData=new BizLogModel[]{};
			using (APIClient client = new APIClient())
			{
				getData = client.QueryLog(out totalRecords, new BizLogFilter()
				{
					PageIndex = pageIndex,
					PageSize = pageSize,
					Keyword = log.Keyword,
					KeywordTypeID =(int)log.KeywordType, 
			     	Summary=log.Summary,
					SystemCode = GetSystemType.GetSystemTypeCode(log.SystemCode),
				    ModuleName=log.ModuleName, 
				    UserRealName = log.UserRealName,
				    CreateOnStart = startTime,
					CreateOnEnd = endTime
				});
			}
			return getData.ToPagedList(pageIndex, pageSize);
	    }

		
		public static List<BizLogModel> ShowBizLogInfo(BizLog log)
		{
			int totalRecords = 99;
			int pageIndex = 1;
			int pageSize = 99;

			using (APIClient client = new APIClient())
			{
				var getData = client.QueryLog(out totalRecords, new BizLogFilter()
				{
					PageIndex = pageIndex,
					PageSize = pageSize,
					SystemCode = GetSystemType.GetSystemTypeCode(log.SystemCode)
				});

				return getData.ToList();
			}
		}


	    #region C/S系统获取IP,MAC

        static string GetIP()
        {
            //winform get local computer ip
            string address = string.Empty;
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    address = ip.ToString();
                    break;
                }
            }
            if (!string.IsNullOrEmpty(address))
            {
                return address;
            }


            return null;
        }

        static string GetMac()
        {
            //winform get local computer mac
            var macs = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(t => t.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                .Select(t => t.GetPhysicalAddress().ToString())
                .Where(t => t.Length == 12);

            return macs.FirstOrDefault();
        }

        #endregion

        #region private member

        /// <summary>
        /// 验证日志是否合法
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        static string Check(BizLogModel log)
        {
            if (log == null) return "[业务日志]不能为空!";

            if (!string.IsNullOrWhiteSpace(log.Keyword) && log.KeywordTypeID <= 0)
            {
                return "如果您输入[关键词],需要同时输入[关键词类型]!";
            }

            if (!string.IsNullOrWhiteSpace(log.UserCode) && log.UserType <= 0)
            {
                return "如果您输入[用户编码],需要同时输入[用户类型]!";
            }

            if (string.IsNullOrWhiteSpace(log.SystemCode))
            {
                return "[系统编码]不能为空!";
            }

            if (string.IsNullOrWhiteSpace(log.ModuleName))
            {
                return "[模块名称]不能为空!";
            }

            if (string.IsNullOrWhiteSpace(log.Summary))
            {
                return "[日志摘]要不能为空!";
            }

            return string.Empty;
        }

        /// <summary>
        /// Mapping
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        static BizLogModel Map(BizLog log)
        {
            if (log == null) return null;
            BizLogModel data = new BizLogModel();
            data.CreateOn = DateTime.Now; //统一处理
            data.Details = log.Details;
            data.IP = log.IP;
            data.Keyword = log.Keyword;
            data.KeywordTypeID = (int)log.KeywordType;
            data.Mac = log.Mac;
            data.ModuleName = log.ModuleName;
            data.Summary = log.Summary;
			data.SystemCode = GetSystemType.GetSystemTypeCode(log.SystemCode);//系统
            data.URL = log.URL;
            data.UserCode = log.UserCode;
            data.UserRealName = log.UserRealName;
            data.UserType = (int)log.UserType;
            return data;
        }

        /// <summary>
        /// 调用WCF
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static void WriteLog2WCF(BizLogModel data)
        {
            using (APIClient client = new APIClient())
            {
                client.WriteLog(data);
            }
        }

        #endregion



    }
}
