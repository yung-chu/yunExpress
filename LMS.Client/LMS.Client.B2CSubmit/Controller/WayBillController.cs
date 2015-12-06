using System;
using System.Data;
using DbHelper;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LMS.Client.B2CSubmit.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CSubmit.Controller
{
    public class WayBillController
    {
        private static readonly string _lmsCon = ConfigurationManager.AppSettings["LMSCon"];
        /// <summary>
        /// 获取需要提交到B2C的运单号
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSubmitWayBillNumber()
        {
            var list = new List<string>();
            try
            {
                string sql = @"SELECT WayBillNumber FROM dbo.B2CPreAlertLogs WHERE Status=1";
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                DataTable dt = dbUtility.ExecuteData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Log.Info("LMS获取需要提交到B2C的运单总数是" + dt.Rows.Count);
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
                Log.Info("LMS获取需要提交到B2C的运单完毕！");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return list;
        }
        /// <summary>
        /// 查询该运单号的数据
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public static WayBillInfoModel GetWayBillInfoModel(string wayBillNumber)
        {
            Log.Info("开始获取运单号为{0}的数据".FormatWith(wayBillNumber));
            var model = new WayBillInfoModel();
            string sql =
                @"SELECT w.WayBillNumber,w.CustomerOrderNumber, ISNULL(s.ShippingFirstName,'')+' '+ ISNULL(s.ShippingLastName,'') ConsigneeName,w.OutShippingMethodID ShippingMethodID
                    ,ISNULL(s.ShippingCompany,'') ShippingCompany,s.ShippingAddress,s.ShippingCity,s.ShippingState,s.ShippingZip,s.ShippingPhone,c.ThreeLetterISOCode CountryCode,
                    ISNULL(w.Weight,0) Weight,ISNULL(w.Length,1) Length,ISNULL(w.Width,1) Width,ISNULL(w.Height,1) Height
                FROM dbo.WayBillInfos w LEFT JOIN dbo.ShippingInfos s ON s.ShippingInfoID = w.ShippingInfoID
                    LEFT JOIN dbo.Countries c ON c.CountryCode = w.CountryCode 
                WHERE w.WayBillNumber='" + wayBillNumber + "'";
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                DataTable dt = dbUtility.ExecuteData(sql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    switch (dt.Columns[j].ColumnName)
                    {
                        case "WayBillNumber":
                            model.WayBillNumber = dt.Rows[0][j].ToString();
                            break;
                        case "CustomerOrderNumber":
                            model.CustomerOrderNumber = dt.Rows[0][j].ToString();
                            break;
                        case "ConsigneeName":
                            model.ConsigneeName = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingCompany":
                            model.CompanyName = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingAddress":
                            model.Street = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingCity":
                            model.CityOrTown = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingState":
                            model.StateOrProvince = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingZip":
                            model.ZIPCode = dt.Rows[0][j].ToString();
                            break;
                        case "ShippingPhone":
                            model.PhoneNumber = dt.Rows[0][j].ToString();
                            break;
                        case "CountryCode":
                            model.CountryCode = dt.Rows[0][j].ToString();
                            break;
                        case "Weight":
                            model.Weight = decimal.Parse(dt.Rows[0][j].ToString());
                            break;
                        case "Length":
                            model.Length = decimal.Parse(dt.Rows[0][j].ToString());
                            break;
                        case "Width":
                            model.Width = decimal.Parse(dt.Rows[0][j].ToString());
                            break;
                        case "Height":
                            model.Height = decimal.Parse(dt.Rows[0][j].ToString());
                            break;
                        case "ShippingMethodID":
                            model.ShippingMethodID = Int32.Parse(dt.Rows[0][j].ToString());
                            break;
                        //case "ApplicationInfos":
                        //    model.ApplicationInfos = SerializeUtil.DeserializeFromXml<List<ApplicationInfoModel>>(dt.Rows[0][j].ToString());
                        //    break;
                    }
                }
                sql =
                    @"SELECT ISNULL(a.Remark,'') SKUCode,a.ApplicationName SKUDescription,a.HSCode,
                    ISNULL(a.Qty,1) Quantity,ISNULL(a.UnitPrice,0)*ISNULL(a.Qty,1) Price,a.ProductUrl ImageUrl
                                    FROM dbo.ApplicationInfos a
                                    WHERE a.WayBillNumber='" + wayBillNumber + "' AND a.IsDelete=0";
                DataTable adt = dbUtility.ExecuteData(sql);
                if (adt != null || adt.Rows.Count > 0)
                {
                    for (int i = 0; i < adt.Rows.Count; i++)
                    {
                        var app = new ApplicationInfoModel();
                        for (int j = 0; j < adt.Columns.Count; j++)
                        {
                            switch (adt.Columns[j].ColumnName)
                            {
                                case "SKUCode":
                                    app.SKUCode = adt.Rows[i][j].ToString();
                                    break;
                                case "SKUDescription":
                                    app.SKUDescription = adt.Rows[i][j].ToString();
                                    break;
                                case "HSCode":
                                    app.HSCode = adt.Rows[i][j].ToString();
                                    break;
                                case "Quantity":
                                    app.Quantity = Int32.Parse(adt.Rows[i][j].ToString());
                                    break;
                                case "Price":
                                    app.Price =decimal.Parse(adt.Rows[i][j].ToString());
                                    break;
                                case "ImageUrl":
                                    app.ImageUrl = adt.Rows[i][j].ToString();
                                    break;
                            }
                        }
                        model.ApplicationInfos.Add(app);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info("完成获取运单号为{0}的数据".FormatWith(wayBillNumber));
            return model;
        }
        /// <summary>
        /// 修改B2CPreAlertLogs
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateB2CPreAlertLogs(B2CPreAlertLog model)
        {
            string sql = "";
            if (model.Status == 2)
            {
                sql = @"UPDATE dbo.B2CPreAlertLogs
                    SET ShippingMethod='"+model.ShippingMethod+"',PreAlertID='"+model.PreAlertID+@"',Status=2
                    ,PreAlertBatchNo='"+model.PreAlertBatchNo+@"',LastUpdatedOn=GETDATE(),LastUpdatedBy='system'
                    WHERE WayBillNumber='" + model.WayBillNumber + "' AND PreAlertBatchNo IS NULL";
            }
            else if (model.Status == 3)
            {
                sql = @"UPDATE dbo.B2CPreAlertLogs
                    SET ShippingMethod='" + model.ShippingMethod + "',ErrorMsg=N'" + model.ErrorMsg.Replace("/r/n","").StripXML() + "',ErrorCode='" + model.ErrorCode + "',ErrorDetails=N'" + model.ErrorDetails.Replace("/r/n","").StripXML() + @"',Status=3
                    ,PreAlertBatchNo='" + model.PreAlertBatchNo + @"',LastUpdatedOn=GETDATE(),LastUpdatedBy='system'
                    WHERE WayBillNumber='" + model.WayBillNumber + "'";
            }
            if (sql.Length>0)
            {
                try
                {
                    DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                    dbUtility.ExecuteNonQuery(sql);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }
        /// <summary>
        /// 获取批次号
        /// </summary>
        /// <returns></returns>
        public static string GetSequenceNumber()
        {
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                object obj = dbUtility.ExecuteScalar("exec P_CreateSequenceNumber N'EU',N'1'");
                if (obj != null)
                {
                    return obj.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return string.Empty;
        }
    }
}
