using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DbHelper;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CTracking.Controller
{
    public class TrackController
    {
        /// <summary>
        /// 间隔几小时运行一次
        /// </summary>
        private static readonly int IntervalHours = Int32.Parse(ConfigurationManager.AppSettings["IntervalHours"]);
        /// <summary>
        /// 跟踪信息数据库连接字符串
        /// </summary>
        private static readonly string TrackCon = ConfigurationManager.AppSettings["TrackCon"];
        /// <summary>
        /// 获取B2C跟新信息插入数据库按时间点
        /// </summary>
        public static bool GetTrackInfo()
        {
            var intervalHours = -1 * (IntervalHours + 4);//加4小时，是防止漏抓取B2C跟踪信息
            var datetime = long.Parse(DateTime.UtcNow.AddHours(intervalHours).ToString("yyyyMMddHH") + "0000");
            Log.Info("本次获取UTC时间点为：{0}".FormatWith(datetime));
            var result = B2CController.GetTrackInfoXmlByTime(datetime);
            try
            {
                DbUtility dbUtility = new SqlDbUtility(TrackCon);
                string obj = dbUtility.ExecuteScalar(
                            "exec P_ImportB2CTrackInfo {0},{1}",
                            datetime.ToString(), result).ToString();
                var issuccess = obj == "1";
                if (issuccess)
                {
                    Log.Info("本次获取UTC时间点为：{0} 插入数据库成功！".FormatWith(datetime));
                }
                else
                {
                    Log.Error("本次获取UTC时间点为：{0} 插入数据库失败！".FormatWith(datetime));
                }
                return issuccess;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        /// <summary>
        /// 根据单号获取跟踪信息
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public static bool GetTrackInfo(string orderNumber)
        {
            Log.Info("本次获取单号为：{0}".FormatWith(orderNumber));
            var result = B2CController.GetTrackInfoXmlByNumber(orderNumber);
            try
            {
                DbUtility dbUtility = new SqlDbUtility(TrackCon);
                string obj = dbUtility.ExecuteScalar(
                            "exec P_ImportB2CTrackInfo {0},{1}",
                            orderNumber, result).ToString();
                var issuccess = obj == "1";
                if (issuccess)
                {
                    Log.Info("本次获取单号为：{0} 插入数据库成功！".FormatWith(orderNumber));
                }
                else
                {
                    Log.Error("本次获取单号为：{0} 插入数据库失败！".FormatWith(orderNumber));
                }
                return issuccess;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }

        private static DataTable GetB2CTrackInfosSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("KeyCode", typeof (string)),
                    new DataColumn("AbcCode", typeof (string)),
                    new DataColumn("SupplierCode", typeof (string)),
                    new DataColumn("ProductName", typeof (string)),
                    new DataColumn("Weight", typeof (int)),
                    new DataColumn("Supplier", typeof (string)),
                    new DataColumn("Email", typeof (string)),
                    new DataColumn("OrderId", typeof (int)),
                    new DataColumn("CodAmount", typeof (decimal)),
                    new DataColumn("Price", typeof (decimal)),
                    new DataColumn("TrackYourParcel", typeof (string)),
                    new DataColumn("CreatedBy", typeof (string)),
                    new DataColumn("LastUpdatedBy", typeof (string))
                });
            return dt;
        }

        private static DataTable GetB2CTrackDetailsSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("KeyCode", typeof (string)),
                    new DataColumn("StatusId", typeof (int)),
                    new DataColumn("PodStatus", typeof (string)),
                    new DataColumn("StatusDate", typeof (DateTime)),
                    new DataColumn("Comment", typeof (string)),
                    new DataColumn("CreatedBy", typeof (string)),
                    new DataColumn("LastUpdatedBy", typeof (string))
                });
            return dt;
        }

        public static DataTable GetB2CForeCastLogsSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("ID", typeof (long))
                {
                    AutoIncrement = true,
                    AutoIncrementStep = 1
                },
                    new DataColumn("WayBillNumber", typeof (string)),
                    new DataColumn("EventCode", typeof (int)),
                    new DataColumn("EventDate", typeof (DateTime)),
                    new DataColumn("EventContent", typeof (string)),
                    new DataColumn("EventLocation", typeof (string)),
                    new DataColumn("IsOkJob", typeof (int)),
                    new DataColumn("CreatedOn", typeof (DateTime)),
                    new DataColumn("LastUpdatedOn", typeof (DateTime))
                });
            return dt;
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        public static void BulkToDb(DataTable dt, string tableName)
        {
            using (var bulkCopy = new SqlBulkCopy(TrackCon) { DestinationTableName = tableName, BatchSize = dt.Rows.Count, BulkCopyTimeout = 10000 })
            {
                try
                {
                    if (dt.Rows.Count != 0)
                    {
                        bulkCopy.ColumnMappings.Clear();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            bulkCopy.ColumnMappings.Add((string)dt.Columns[j].ColumnName, (string)dt.Columns[j].ColumnName);//匹配列名  
                        }
                        bulkCopy.WriteToServer(dt);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }
        /// <summary>
        /// B2CForeCastLogs外部信息插入LMS的InTrackingLogInfos
        /// </summary>
        public static void TrackInfoInsertLms()
        {
            Log.Info("开始插入LMS的InTrackingLogInfos");
            try
            {
                DbUtility dbUtility = new SqlDbUtility(TrackCon);
                dbUtility.ExecuteNonQuery("P_B2CInsertLMSInTrackingLogInfos");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
            Log.Info("完成插入LMS的InTrackingLogInfos");
        }
    }
}
