using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DbHelper;
using LMS.Client.SubmitSF.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitSF.Controller
{
    public class WayBillController
    {
        private static readonly string _lmsCon = System.Configuration.ConfigurationManager.AppSettings["LMSCon"];

        private static readonly string _NLPOSTShippingMethodID =
            System.Configuration.ConfigurationManager.AppSettings["NLPOSTShippingMethodID"];

        private static readonly string _NLPOSTVenderCode =
            System.Configuration.ConfigurationManager.AppSettings["NLPOSTVenderCode"];

        /// <summary>
        /// 运单状态是已提交状态
        /// </summary>
        private const string SubmitStatus = "3";
        /// <summary>
        /// 提交顺丰异常操作类型
        /// </summary>
        private const string SubmitSFAbnormal = "6";

        /// <summary>
        /// 获取需要提交到顺丰的运单
        /// </summary>
        /// <returns></returns>
        public static List<string> GetNlPostWayBillNumberList()
        {
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData("exec P_GetNlPostWayBillNumberList N'" + _NLPOSTShippingMethodID + "',N'" + SubmitStatus + "'");
            var list = new List<string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS获取需要提交到顺丰的运单总数是" + dt.Rows.Count);
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
            Log.Info("LMS获取需要提交到顺丰的运单完毕！");
            return list;
        }
        /// <summary>
        /// 获取需要提交到顺丰的运单信息
        /// </summary>
        /// <param name="wayBillNumbers">运单号集合</param>
        /// <returns></returns>
        public static List<WayBillSfModel> GetWayBillSfModelList(List<string> wayBillNumbers)
        {
            if (!wayBillNumbers.Any()) return null;
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData("exec P_GetNlPostWayBillNumberInfoList N'" + string.Join(",",wayBillNumbers) + "'");
            var list = new List<WayBillSfModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS获取需要提交到顺丰的运单信息总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var model = new WayBillSfModel();
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
                            case "IsReturn":
                                model.IsReturn = bool.Parse(dt.Rows[i][j].ToString());
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
            Log.Info("LMS获取需要提交到顺丰的运单信息完毕！");
            return list;
        }
        /// <summary>
        /// 提交顺丰错误
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <param name="failureMessage">错误原因</param>
        public static bool SubmitFailure(string wayBillNumber, string failureMessage)
        {
            Log.Info("运单号{0}提交顺丰失败，原因:{1}".FormatWith(wayBillNumber,failureMessage));
            if (string.IsNullOrWhiteSpace(wayBillNumber)) return false;
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj = dbUtility.ExecuteScalar("exec P_CreateNlPostAbnormalWayBill {0},{1}",wayBillNumber,failureMessage);
            return obj.ToString() == "1";
        }
        /// <summary>
        /// 提交顺丰成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SubmitSuccess(NetherlandsParcelModel model)
        {
            Log.Info("运单号{0},提交顺丰成功".FormatWith(model.WayBillNumber));
            if (model == null) return false;
            if (model.WayBillNumber.IsNullOrWhiteSpace()) return false;
            model.Status = 2;
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj = dbUtility.ExecuteScalar("exec P_CreateNlPostSuccessWayBill {0},{1},{2},{3},{4},{5},{6},{7},{8}",
                model.WayBillNumber, Int32.Parse(_NLPOSTShippingMethodID), _NLPOSTVenderCode,model.MailNo,model.OriginCode??"",model.DestCode??"",
                model.AgentMailNo,model.Remark??"",model.Status);
            return obj.ToString() == "1";
        }
        /// <summary>
        /// 修改运单表没有指定发货渠道的荷兰小包
        /// </summary>
        /// <returns></returns>
        public static void UpdateOutShippingMethod()
        {
            Log.Info("开始指定顺丰荷兰小包出仓渠道");
            string sql = @"UPDATE w
		                SET w.OutShippingMethodID='" + _NLPOSTShippingMethodID + @"',w.VenderCode='" + _NLPOSTVenderCode + @"',
				                w.LastUpdatedOn=GETDATE(),w.LastUpdatedBy='system'
		                FROM dbo.WayBillInfos w 
		                WHERE w.VenderCode IS NULL AND OutShippingMethodID IS NULL 
		                AND EXISTS( SELECT 1 FROM dbo.NetherlandsParcelResponses n WHERE w.WayBillNumber=n.WayBillNumber AND n.Status=2)";
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            dbUtility.ExecuteNonQuery(sql);
            Log.Info("完成指定顺丰荷兰小包出仓渠道");
        }
    }
}
