using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DbHelper;
using System.Configuration;
using LMS.Client.SubmitSF.Model;
using LMS.Services.SF.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitSF.Controller
{
    public class SebController
    {
        private static readonly string _lmsCon = ConfigurationManager.AppSettings["LMSCon"];
        private static readonly string _LithuaniaShippingMethodID =ConfigurationManager.AppSettings["LithuaniaShippingMethodID"];

        private static readonly string _LithuaniaVenderCode =ConfigurationManager.AppSettings["LithuaniaVenderCode"];

        
        /// <summary>
        /// 获取需要提交到顺E宝的运单
        /// --已提交或者已收货
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLithuaniaWayBillNumberList()
        {
            var shippingMethodId = _LithuaniaShippingMethodID.Replace(",", "','");
            string sql = @"SELECT  WayBillNumber FROM    dbo.WayBillInfos w
                            WHERE   InShippingMethodID IN('"+shippingMethodId+@"')
                                AND Status IN(3,4)
	                            AND w.IsHold=0
                                AND NOT EXISTS ( SELECT 1
                                                    FROM   LithuaniaInfos n
                                                    WHERE  w.WayBillNumber = n.WayBillNumber )";
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData(sql);
            var list = new List<string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS获取需要提交到顺E宝的运单总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Columns[j].ColumnName != "WayBillNumber") continue;
                        if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                        {
                            list.Add(dt.Rows[i][j].ToString().Trim());
                        }
                    }
                }
            }
            Log.Info("LMS获取需要提交到顺E宝的运单完毕！");
            return list;
        }
         /// <summary>
         /// 获取需要提交到顺E宝的运单信息
         /// </summary>
         /// <param name="wayBillNumbers">运单号集合</param>
         /// <returns></returns>
         public static List<SebModel> GetWayBillSfModelList(List<string> wayBillNumbers)
         {
             if (!wayBillNumbers.Any()) return null;
             string sql = @"SELECT  w.WayBillNumber ,
		                    s.ShippingCompany ,
		                    ISNULL(s.ShippingFirstName,'') + ' ' + ISNULL(s.ShippingLastName,'') ShippingName ,
		                    s.ShippingPhone ,
		                    ISNULL(s.ShippingAddress,'') + ' ' + ISNULL(s.ShippingAddress1,'') + ' '
		                    + ISNULL(s.ShippingAddress2,'') ShippingAddress ,
		                    ISNULL(c.PackageNumber,1) PackageNumber ,
		                    w.CountryCode ,
		                    s.ShippingZip ,
		                    s.ShippingState ,
		                    s.ShippingCity ,
		                    w.InShippingMethodID ShippingMethodID ,
		                    CONVERT(XML, ( SELECT  a.ApplicationName ,
                                                    a.HSCode HsCode,
								                    a.Qty ,
								                    a.UnitWeight ,
								                    a.UnitPrice
						                    FROM     dbo.ApplicationInfos a
						                    WHERE    a.WayBillNumber = w.WayBillNumber
								                    AND a.CustomerOrderID = c.CustomerOrderID
								                    AND a.IsDelete = 0
						                    FOR
						                    XML PATH('ApplicationInfoSfModel') ,
							                    ROOT('ArrayOfApplicationInfoSfModel')
						                    )) AS ApplicationInfo
	                    FROM    dbo.WayBillInfos w
			                    LEFT JOIN dbo.ShippingInfos s ON w.ShippingInfoID = s.ShippingInfoID
			                    LEFT JOIN dbo.CustomerOrderInfos c ON w.CustomerOrderID = c.CustomerOrderID
	                    WHERE w.WayBillNumber IN('"+string.Join("','",wayBillNumbers)+"')";
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData(sql);
            var list = new List<SebModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS获取需要提交到顺E宝的运单信息总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var model = new SebModel();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "WayBillNumber":
                                model.WayBillNumber = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingCompany":
                                model.ShippingCompany = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingName":
                                model.ShippingName = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingPhone":
                                model.ShippingPhone = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingAddress":
                                model.ShippingAddress = dt.Rows[i][j].ToString();
                                break;
                            case "PackageNumber":
                                model.PackageNumber = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "CountryCode":
                                model.CountryCode = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingZip":
                                model.ShippingZip = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingState":
                                model.ShippingState = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingCity":
                                model.ShippingCity = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingMethodID":
                                model.ShippingMethodId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "ApplicationInfo":
                                if (string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                                    continue;
                                model.ApplicationInfo = SerializeUtil.DeserializeFromXml<List<ApplicationInfoSfModel>>(dt.Rows[i][j].ToString());
                                break;
                        }
                    }
                    list.Add(model);
                }
            }
            Log.Info("LMS获取需要提交到顺E宝的运单信息完毕！");
            return list;
         }
        /// <summary>
        /// 提交顺E宝错误
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <param name="failureMessage">错误原因</param>
        public static bool SubmitFailure(string wayBillNumber, string failureMessage)
        {
            Log.Info("运单号{0}提交顺E宝失败，原因:{1}".FormatWith(wayBillNumber,failureMessage));
            if (string.IsNullOrWhiteSpace(wayBillNumber)) return false;
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj = dbUtility.ExecuteScalar("exec P_CreateLithuaniaAbnormalWayBill {0},{1}",wayBillNumber,failureMessage);
            return obj.ToString() == "1";
        }
        /// <summary>
        /// 提交顺E宝成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SubmitSuccess(SfOrderResponse model)
        {
            Log.Info("运单号{0},提交顺E宝成功".FormatWith(model.OrderId));
            if (model == null) return false;
            if (model.OrderId.IsNullOrWhiteSpace()) return false;
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj = dbUtility.ExecuteScalar("exec P_CreateLithuaniaSuccessWayBill {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                model.OrderId, _LithuaniaVenderCode, model.MailNo, model.OriginCode ?? "", model.DestCode ?? "",
                model.AgentMailNo??"",model.Remark??"",2,model.FilterResult??"",model.TrackNumber??"");
            return obj.ToString() == "1";
        }
        /// <summary>
        /// 修改运单表没有指定发货渠道的俄罗斯
        /// </summary>
        public static void UpdateOutShippingMethod()
        {
            Log.Info("开始指定顺E宝出仓渠道");
            string sql = @"UPDATE w
		                SET w.OutShippingMethodID=w.InShippingMethodID,w.VenderCode='"+_LithuaniaVenderCode+@"',
				                w.LastUpdatedOn=GETDATE(),w.LastUpdatedBy='system'
		                FROM dbo.WayBillInfos w 
		                WHERE w.VenderCode IS NULL AND OutShippingMethodID IS NULL 
		                AND EXISTS( SELECT 1 FROM dbo.LithuaniaInfos n WHERE w.WayBillNumber=n.WayBillNumber AND n.Status=2)";
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            dbUtility.ExecuteNonQuery(sql);
            Log.Info("完成指定顺E宝出仓渠道");
        }
    }
}
