using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Log = LighTake.Infrastructure.Common.Logging.Log;

namespace LighTake.Infrastructure.Common.Excel
{
    public class ExportExcelByWeb
    {

        /// <summary>
        /// List写入Excel
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="lst">要写入的List</param>
        /// <param name="listTile">实体中需要的列名（默认为所有）</param>
        public static void WriteToDownLoad<T>(string fileName, string sheetName, List<T> lst, List<string> listTile, Dictionary<string, Dictionary<int, string>> convertDataColumns,int execlType=1)
        {
            WriteListDataToExcel(fileName, sheetName, lst, listTile, convertDataColumns, execlType);
        }

        /// <summary>
        /// List写入Excel
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="lst">要写入的List</param>
        /// <param name="listTile">实体中需要的列名（默认为所有）</param>
        public static void WriteToDownLoad<T>(List<T> lst, List<string> listTile, Dictionary<string, Dictionary<int, string>> convertDataColumns,int execlType=1)
        {
            WriteListDataToExcel(DateTime.Now.ToString("yyyyMMddHH:mm:ss"), "sheet1", lst, listTile, convertDataColumns, execlType);
        }

        /// <summary>
        /// List写入Excel
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="lst">要写入的List</param>
        /// <param name="listTile">实体中需要的列名（默认为所有）</param>
        public static void WriteToDownLoad<T>(List<T> lst, List<string> listTile, int execlType = 1)
        {
            WriteListDataToExcel(DateTime.Now.ToString("yyyyMMddHH:mm:ss"), "sheet1", lst, listTile, null,execlType);
        }


		/// <summary>
		/// List写入Excel(导出多个sheet)
		/// add by yungchu
		/// 2014/7/1
		/// </summary>
		/// <typeparam name="T">实体</typeparam>
		/// <param name="fileName">要保存的文件名称 eg:test.xls</param>
		/// <param name="sheetName">工作薄名称</param>
		/// <param name="lst">要写入的List</param>
		/// <param name="listTile">实体中需要的列名（默认为所有）</param>
		public static void WriteToDownLoad<T>(IWorkbook workbook, List<T> lst, List<string> listTitle, Dictionary<string, Dictionary<int, string>> convertDataColumns,int flag, string customerOrVender,DateTime startTime,DateTime endTime,decimal receivingAmount,decimal deliveryAmount, string sheetName)
		{
			//InitExcel(DateTime.Now.ToString("yyyyMMddHH:mm:ss"));
			//  HSSFWorkbook workbook = InitializeWorkbook();
			//对第一lst进行导出数据 写入到sheet1里面去
			ListExcel_chragePay(workbook, lst, listTitle, convertDataColumns, 2, true, flag, customerOrVender, startTime, endTime, receivingAmount, deliveryAmount, sheetName);

		}



		/// <summary>
		/// 应收应付数据导出
		/// add by yungchu
		/// 2014/7/1
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="workbook"></param>
		/// <param name="lst"></param>
		/// <param name="lstTitle"></param>
		/// <param name="convertDataColumns"></param>
		/// <param name="startIndex"></param>
		/// <param name="isTitle"></param>
		/// <param name="sheetName"></param>
		private static void ListExcel_chragePay<T>(IWorkbook workbook, List<T> lst, List<string> lstTitle, Dictionary<string, Dictionary<int, string>> convertDataColumns, int startIndex, bool isTitle,int flag, string customerOrVender,DateTime startTime,DateTime endTime,decimal receivingAmount,decimal deliveryAmount, string sheetName)
		{
			ISheet sheet1 = null;
			T _t = (T)Activator.CreateInstance(typeof(T));
			PropertyInfo[] propertys = _t.GetType().GetProperties();
			if (lstTitle == null)
			{
				lstTitle = propertys.Select(t => t.Name).ToList();
			}
			if (lstTitle != null && isTitle)
			{

				sheet1 = workbook.CreateSheet(sheetName);


				//表头第一行
				IRow firstRow = sheet1.CreateRow(0);
				ICell cell = firstRow.CreateCell(0);
				firstRow.HeightInPoints = 20;

			
				ICellStyle style = workbook.CreateCellStyle();

				//第一行两端对齐 加边框
				style.VerticalAlignment=VerticalAlignment.Justify;
				cell.CellStyle = style;

			
				//插入空格
				string empty = "                                                                       ";

				//按客户导出
				if(flag==1)
				{
					string result = string.Format("客户：{0}{1}日期:{2}  至 {3}", customerOrVender,empty, startTime, endTime);
					cell.SetCellValue(result);

				}//按服务商导出
				else if(flag==2)
				{
					string result = string.Format("服务商：{0}{1}日期:{2}  至 {3}", customerOrVender,empty, startTime, endTime);
					cell.SetCellValue(result);
				}



				//设置总体列宽
				for (int i = 0; i < lstTitle.Count; i++)
				{
					sheet1.SetColumnWidth(i, 20 * 256);
				}
	

				//第一行合并单元格
				sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

			




				//给定的标题为空,赋值T默认的列名
				IRow hsTitleRow = sheet1.CreateRow(1);
				//标题赋值
				for (int yt = 0; yt < lstTitle.Count; yt++)
				{
					if (lstTitle[yt].IndexOf("-") != -1)
					{
						hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
					}
					else
					{
						hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
					}
				}
			}

			else
				sheet1 = workbook.GetSheet(sheetName);

			for (int xcount = 0; xcount < lst.Count; xcount++)
			{
				IRow hsBodyRow = sheet1.CreateRow(startIndex);

				for (int ycBody = 0; ycBody < lstTitle.Count; ycBody++)
				{
					string title = string.Empty;
					if (lstTitle[ycBody].IndexOf("-") != -1)
					{
						title = lstTitle[ycBody].Split('-')[0];
					}
					else
					{
						title = lstTitle[ycBody];
					}

					PropertyInfo pi = propertys.First(p => p.Name == title);
					//if (!lstTitle.Any(t => t == pi.Name))
					//    continue;
					if (pi == null) continue;
					object obj = pi.GetValue(lst[xcount], null);

					if (obj != null)        //geying
					{
						var value = obj.ToString();
						if (obj.ToString() == "False")
							value = "0";
						else if (obj.ToString() == "True")
							value = "1";
						if (convertDataColumns == null)
						{
							hsBodyRow.CreateCell(ycBody).SetCellValue(value);
							continue;
						}
						if (convertDataColumns.ContainsKey(pi.Name))
						{
							foreach (KeyValuePair<string, Dictionary<int, string>> columnDictItem in convertDataColumns)
							{
								bool bl = false;
								if (pi.Name == columnDictItem.Key)
								{
									try
									{
										if (columnDictItem.Value.ContainsKey(int.Parse(value)))
										{
											foreach (KeyValuePair<int, string> dictValueItem in columnDictItem.Value)
											{
												if (dictValueItem.Key == int.Parse(value))
												{
													hsBodyRow.CreateCell(ycBody).SetCellValue(dictValueItem.Value);
													bl = true;
													break;
												}
											}
										}
									}
									catch
									{
										throw new Exception("获取value转换出错了,详细value为:" + value);
									}
								}
								if (bl)
									break;
							}
						}
						else
							hsBodyRow.CreateCell(ycBody).SetCellValue(value);
					}
					else
					{
						hsBodyRow.CreateCell(ycBody).SetCellValue("");
					}
				}
				startIndex++;
			}


		
			IRow countRow = sheet1.CreateRow(lst.Count + 3);
			//应收合计
			ICell cellReceivingAmount = countRow.CreateCell(2);
			cellReceivingAmount.SetCellValue("应收合计:"+Convert.ToString(receivingAmount));

			//应付合计
			ICell cellDeliveryAmount = countRow.CreateCell(3);
			cellDeliveryAmount.SetCellValue("应付合计:" + Convert.ToString(deliveryAmount));

			//利率统计
			ICell cellRate = countRow.CreateCell(4);
			decimal getRate = ((receivingAmount - deliveryAmount)/deliveryAmount)*100;
			cellRate.SetCellValue("总毛利率:" + getRate.ToString("#0.0000")+"%");

		}


        private static void ListExcel<T>(IWorkbook workbook, List<T> lst, List<string> lstTitle, Dictionary<string, Dictionary<int, string>> convertDataColumns, int startIndex, bool isTitle, string sheetName = "sheet1")
        {
            ISheet sheet1 = null;
            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();
            if (lstTitle == null)
            {
                lstTitle = propertys.Select(t => t.Name).ToList();
            }
            if (lstTitle != null && isTitle)
            {
                sheet1 = workbook.CreateSheet(sheetName);
                //给定的标题为空,赋值T默认的列名

                IRow hsTitleRow = sheet1.CreateRow(0);
                //标题赋值
                for (int yt = 0; yt < lstTitle.Count; yt++)
                {
                    if (lstTitle[yt].IndexOf("-") != -1)
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    }
                    else
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    }
                }
            }

            else
                sheet1 = workbook.GetSheet(sheetName);
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Red.Index;//设置背景颜色为红色
            cellStyle.FillForegroundColor = HSSFColor.Red.Index;
            bool isSetRed = false;
            if (propertys.FirstOrDefault(p => p.Name == "IsRed") != null)
            {
                isSetRed = propertys.FirstOrDefault(p => p.Name == "IsRed") != null;
            }
            //bool isSetRed = propertys.FirstOrDefault(p => p.Name == "IsRed") != null;
            for (int xcount = 0; xcount < lst.Count; xcount++)
            {
                IRow hsBodyRow = sheet1.CreateRow(startIndex);
                
                for (int ycBody = 0; ycBody < lstTitle.Count; ycBody++)
                {
                    string title = string.Empty;
                    if (lstTitle[ycBody].IndexOf("-") != -1)
                    {
                        title = lstTitle[ycBody].Split('-')[0];
                    }
                    else
                    {
                        title = lstTitle[ycBody];
                    }

                    PropertyInfo pi = propertys.First(p => p.Name == title);
                    //if (!lstTitle.Any(t => t == pi.Name))
                    //    continue;
                    if (pi == null) continue;
                    object obj = pi.GetValue(lst[xcount], null);

                    if (obj != null)        //geying
                    {
                        var value = obj.ToString();
                        if (obj.ToString() == "False")
                            value = "0";
                        else if (obj.ToString() == "True")
                            value = "1";
                        if (convertDataColumns == null)
                        {
                            hsBodyRow.CreateCell(ycBody).SetCellValue(value);
                            if (isSetRed)
                            {
                                object color= propertys.First(p => p.Name == "IsRed").GetValue(lst[xcount], null);
                                if (color != null && color.ToString() == "1")
                                {
                                    hsBodyRow.GetCell(ycBody).CellStyle = cellStyle;
                                }
                            }
                            continue;
                        }
                        if (convertDataColumns.ContainsKey(pi.Name))
                        {
                            foreach (KeyValuePair<string, Dictionary<int, string>> columnDictItem in convertDataColumns)
                            {
                                bool bl = false;
                                if (pi.Name == columnDictItem.Key)
                                {
                                    try
                                    {
                                        if (columnDictItem.Value.ContainsKey(int.Parse(value)))
                                        {
                                            foreach (KeyValuePair<int, string> dictValueItem in columnDictItem.Value)
                                            {
                                                if (dictValueItem.Key == int.Parse(value))
                                                {
                                                    hsBodyRow.CreateCell(ycBody).SetCellValue(dictValueItem.Value);
                                                    bl = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        throw new Exception("获取value转换出错了,详细value为:" + value);
                                    }
                                }
                                if (bl)
                                    break;
                            }
                        }
                        else
                            hsBodyRow.CreateCell(ycBody).SetCellValue(value);
                    }
                    else
                    {
                        hsBodyRow.CreateCell(ycBody).SetCellValue("");
                    }
                    if (isSetRed)
                    {
                        object color = propertys.First(p => p.Name == "IsRed").GetValue(lst[xcount], null);
                        if (color != null && color.ToString() == "1")
                        {
                            hsBodyRow.GetCell(ycBody).CellStyle = cellStyle;
                        }
                    }
                }
                startIndex++;
            }
        }





        /// <summary>
        /// 运单导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lst"></param>
        /// <param name="lstTitle"></param>
        public static void ListWayBillExcel<T>(string fileName, List<T> lst, List<string> lstTitle,int execlType=1)
        {
            IWorkbook workbook;
            if (execlType == 1)
            {
                InitExcel(fileName);
                workbook = InitializeWorkbook();
            }
            else
            {
                InitExcelx("WayBillInfoList_" + DateTime.Now.Ticks.ToString());
                workbook = InitializeXWorkbook();
            }

            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();

            #region header set value
            ISheet sheet1 = null;
            if (lstTitle != null)
            {
                sheet1 = workbook.CreateSheet("sheet1");
                //给定的标题为空,赋值T默认的列名

                IRow hsTitleRow = sheet1.CreateRow(0);
                //标题赋值
                for (int yt = 0; yt < lstTitle.Count; yt++)
                {
                    if (lstTitle[yt].IndexOf("-") != -1)
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    }
                    else
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    }
                }
            }
            else
                sheet1 = workbook.GetSheet("sheet1");
            #endregion

            for (int xcount = 0; xcount < lst.Count; xcount++)//行
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount+1);
                for (int ycBody = 0; ycBody < 42; ycBody++) //列
                {
                        string title = string.Empty;
                        if (ycBody == 0)
                        {
                            title = "CustomerOrderNumber";
                        }
                        else if (ycBody == 1)
                        {
                            title = "CustomerCode";
                        }else if (ycBody == 2)
                        {
                            title = "Name";
                        }
                        else if (ycBody == 3)
                        {
                            title = "WayBillNumber";
                        }
                        else if (ycBody == 4)
                        {
                            title = "InShippingMethodName";
                        }
                        else if (ycBody == 5)
                        {
                            title = "TrackingNumber";
                        }
                        else if (ycBody == 6)
                        {
                            title = "CountryCode";
                        }else if (ycBody == 7)
                        {
                            title = "ChineseName";
                        }
                        else if (ycBody == 8)
                        {
                            title = "ShippingFirstName";
                        }
                        else if (ycBody == 9)
                        {
                            title = "ShippingLastName";
                        } else if (ycBody == 10)
                        {
                            title = "ShippingCompany";
                        }
                        else if (ycBody == 11)
                        {
                            title = "ShippingAddress";
                        }
                        else if (ycBody == 12)
                        {
                            title = "ShippingCity";
                        }
                        else if (ycBody == 13)
                        {
                            title = "ShippingState";
                        }
                        else if (ycBody == 14)
                        {
                            title = "ShippingZip";
                        }
                        else if (ycBody == 15)
                        {
                            title = "ShippingPhone";
                        }
                        else if (ycBody == 16)
                        {
                            title = "WayCreatedOn";
                        }
                        else if (ycBody == 17)
                        {
                            title = "ShiCreatedOn";
                        }
                        else if (ycBody == 18)
                        {
                            title = "SenCreatedOn";
                        }
                        else if (ycBody == 19)
                        {
                            title = "Status";
                        }
                        else if (ycBody == 20)
                        {
                            title = "ShippingTaxId";
                        }
                        else if (ycBody == 21)
                        {
                            title = "SenderFirstName";
                        }
                        else if (ycBody == 22)
                        {
                            title = "SenderLastName";
                        }
                        else if (ycBody == 23)
                        {
                            title = "SenderCompany";
                        }
                        else if (ycBody == 24)
                        {
                            title = "SenderAddress";
                        }
                        else if (ycBody == 25)
                        {
                            title = "SenderCity";
                        }
                        else if (ycBody == 26)
                        {
                            title = "SenderState";
                        }
                        else if (ycBody == 27)
                        {
                            title = "SenderZip";
                        }
                        else if (ycBody == 28)
                        {
                            title = "SenderPhone";
                        }
                        else if (ycBody == 29)
                        {
                            title = "IsReturn";
                        }
                        else if (ycBody == 30)
                        {
                            title = "InsuredName";
                        }
                        else if (ycBody == 31)
                        {
                            title = "InsureAmount";
                        }
                        else if (ycBody == 32)
                        {
                            title = "SensitiveTypeName";
                        }
                        else if (ycBody == 33)
                        {
                            title = "AppLicationType";
                        }else if (ycBody == 34)
                        {
                            title = "PackageNumber";
                        }
                        else if (ycBody == 35)
                        {
                            title = "Length";
                        }
                        else if (ycBody == 36)
                        {
                            title = "Width";
                        }
                        else if (ycBody == 37)
                        {
                            title = "Height";
                        }

                        else if (ycBody == 38)
                        {
                            title = "Weight";
                        }
                        else if (ycBody == 39)
                        {
                            title = "SettleWeight";
                        }
						else if (ycBody == 40)//是否关税预付 yungchu
						{
							title = "EnableTariffPrepay";
						}
                        else if (ycBody == 41)
                        {
                            title = "ApplicationInfoModels";
                        }
                    PropertyInfo pi = propertys.FirstOrDefault(p => p.Name == title);
                       
                        object obj = pi.GetValue(lst[xcount], null); //取对应行与列的值

                        if (ycBody>40&&obj!=null&&pi.PropertyType.IsGenericType)
                        {
                            #region
                                Type objType = obj.GetType();
                                int count = Convert.ToInt32(objType.GetProperty("Count").GetValue(obj, null));
                                int z = 41;

                                for (int i = 0; i < count; i++)
                                {
                                    object listItem = objType.GetProperty("Item").GetValue(obj, new object[] { i });

                                    PropertyInfo[] propertysList = listItem.GetType().GetProperties();
                                    for(int j=0;j<8;j++)
                                    {
                                        string titles = null;
                                        //object subObj = pi2.GetValue(listItem, null);
                                        if ((z - 41) % 8 == 0)
                                        {
                                            titles = "ApplicationName";
                                        }
                                        else if ((z - 41) % 8 == 1)
                                        {
                                            titles = "PickingName";
                                        }
                                        else if ((z - 41) % 8 == 2)
                                        {
                                            titles = "HSCode";
                                        }
                                        else if ((z - 41) % 8 == 3)
                                        {
                                            titles = "Qty";
                                        }
                                        else if ((z - 41) % 8 == 4)
                                        {
                                            titles = "UnitPrice";
                                        }
                                        else if ((z - 41) % 8 == 5)
                                        {
                                            titles = "UnitWeight";
                                        }
                                        else if ((z - 41) % 8 == 6)
                                        {
                                            titles = "ProductUrl";
                                        }else if ((z - 41) % 8 == 7)
                                        {
                                            titles = "Remark";
                                        }

                                        PropertyInfo pi2 = propertysList.FirstOrDefault(p => p.Name == titles);

                                        object subObj = pi2.GetValue(listItem, null); //取对应行与列的值
                                        if (subObj != null)
                                        {
                                            hsBodyRow.CreateCell(z).SetCellValue(subObj.ToString());
                                        }
                                        else
                                        {
                                            hsBodyRow.CreateCell(z).SetCellValue("");
                                        }
                                        z++;
                                    }
                                }
                            #endregion
                        }
                        else
                        {
                            if (obj != null) //geying
                            {
                                var value = obj.ToString();
                                if (obj.ToString() == "False")
                                    value = "否";
                                else if (obj.ToString() == "True")
                                    value = "是";
                                hsBodyRow.CreateCell(ycBody).SetCellValue(value);
                                continue;
                            }
                            else //
                            {
                                hsBodyRow.CreateCell(ycBody).SetCellValue("");
                            }
                        }
                }
            }
            if (execlType == 2)
            {
                WriteToStream(workbook, fileName);
            }
            else
            {
                WriteToStream(workbook);
            }
            
        }


        /// <summary>
        /// 真实跟踪号查询导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lst"></param>
        /// <param name="lstTitle"></param>
        public static void TrackWayBillExcel<T>(string fileName, List<T> lst, List<string> lstTitle)
        {
            InitExcel(fileName);

            HSSFWorkbook workbook = InitializeWorkbook();

            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();

            #region header set value
            ISheet sheet1 = null;
            if (lstTitle != null)
            {
                sheet1 = workbook.CreateSheet("sheet1");
                //给定的标题为空,赋值T默认的列名

                IRow hsTitleRow = sheet1.CreateRow(0);
                //标题赋值
                for (int yt = 0; yt < lstTitle.Count; yt++)
                {
                    if (lstTitle[yt].IndexOf("-") != -1)
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    }
                    else
                    {
                        hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    }
                }
            }
            else
                sheet1 = workbook.GetSheet("sheet1");
            #endregion

            for (int xcount = 0; xcount < lst.Count; xcount++)//行
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount + 1);
                for (int ycBody = 0; ycBody < 43; ycBody++) //列
                {
                    string title = string.Empty;
                    if (ycBody == 0)
                    {
                        title = "CustomerOrderNumber";
                    }
                    else if (ycBody == 1)
                    {
                        title = "CustomerCode";
                    }
                    else if (ycBody == 2)
                    {
                        title = "Name";
                    }
                    else if (ycBody == 3)
                    {
                        title = "WayBillNumber";
                    }
                    else if (ycBody == 4)
                    {
                        title = "InShippingMethodName";
                    }
                    else if (ycBody == 5)
                    {
                        title = "TrackingNumber";
                    }
                    else if (ycBody == 6)
                    {
                        title = "TrueTrackingNumber";
                    }
                    else if (ycBody == 7)
                    {
                        title = "CountryCode";
                    }
                    else if (ycBody == 8)
                    {
                        title = "ChineseName";
                    }
                    else if (ycBody == 9)
                    {
                        title = "ShippingFirstName";
                    }
                    else if (ycBody == 10)
                    {
                        title = "ShippingLastName";
                    }
                    else if (ycBody == 11)
                    {
                        title = "ShippingCompany";
                    }
                    else if (ycBody == 12)
                    {
                        title = "ShippingAddress";
                    }
                    else if (ycBody == 13)
                    {
                        title = "ShippingCity";
                    }
                    else if (ycBody == 14)
                    {
                        title = "ShippingState";
                    }
                    else if (ycBody == 15)
                    {
                        title = "ShippingZip";
                    }
                    else if (ycBody == 16)
                    {
                        title = "ShippingPhone";
                    }
                    else if (ycBody == 17)
                    {
                        title = "WayCreatedOn";
                    }
                    else if (ycBody == 18)
                    {
                        title = "ShiCreatedOn";
                    }
                    else if (ycBody == 19)
                    {
                        title = "SenCreatedOn";
                    }
                    else if (ycBody == 20)
                    {
                        title = "Status";
                    }
                    else if (ycBody == 21)
                    {
                        title = "ShippingTaxId";
                    }
                    else if (ycBody == 22)
                    {
                        title = "SenderFirstName";
                    }
                    else if (ycBody == 23)
                    {
                        title = "SenderLastName";
                    }
                    else if (ycBody == 24)
                    {
                        title = "SenderCompany";
                    }
                    else if (ycBody == 25)
                    {
                        title = "SenderAddress";
                    }
                    else if (ycBody == 26)
                    {
                        title = "SenderCity";
                    }
                    else if (ycBody == 27)
                    {
                        title = "SenderState";
                    }
                    else if (ycBody == 28)
                    {
                        title = "SenderZip";
                    }
                    else if (ycBody == 29)
                    {
                        title = "SenderPhone";
                    }
                    else if (ycBody == 30)
                    {
                        title = "IsReturn";
                    }
                    else if (ycBody == 31)
                    {
                        title = "InsuredName";
                    }
                    else if (ycBody == 32)
                    {
                        title = "InsureAmount";
                    }
                    else if (ycBody == 33)
                    {
                        title = "SensitiveTypeName";
                    }
                    else if (ycBody == 34)
                    {
                        title = "AppLicationType";
                    }
                    else if (ycBody == 35)
                    {
                        title = "PackageNumber";
                    }
                    else if (ycBody == 36)
                    {
                        title = "Length";
                    }
                    else if (ycBody == 37)
                    {
                        title = "Width";
                    }
                    else if (ycBody == 38)
                    {
                        title = "Height";
                    }

                    else if (ycBody == 39)
                    {
                        title = "Weight";
                    }
                    else if (ycBody == 40)
                    {
                        title = "SettleWeight";
                    }
                    else if (ycBody == 41)//是否关税预付 yungchu
                    {
                        title = "EnableTariffPrepay";
                    }
                    else if (ycBody == 42)
                    {
                        title = "ApplicationInfoModels";
                    }
                    PropertyInfo pi = propertys.FirstOrDefault(p => p.Name == title);

                    object obj = pi.GetValue(lst[xcount], null); //取对应行与列的值

                    if (ycBody > 41 && obj != null && pi.PropertyType.IsGenericType)
                    {
                        #region
                        Type objType = obj.GetType();
                        int count = Convert.ToInt32(objType.GetProperty("Count").GetValue(obj, null));
                        int z = 42;

                        for (int i = 0; i < count; i++)
                        {
                            object listItem = objType.GetProperty("Item").GetValue(obj, new object[] { i });

                            PropertyInfo[] propertysList = listItem.GetType().GetProperties();
                            for (int j = 0; j < 8; j++)
                            {
                                string titles = null;
                                //object subObj = pi2.GetValue(listItem, null);
                                if ((z - 42) % 8 == 0)
                                {
                                    titles = "ApplicationName";
                                }
                                else if ((z - 42) % 8 == 1)
                                {
                                    titles = "PickingName";
                                }
                                else if ((z - 42) % 8 == 2)
                                {
                                    titles = "HSCode";
                                }
                                else if ((z - 42) % 8 == 3)
                                {
                                    titles = "Qty";
                                }
                                else if ((z - 42) % 8 == 4)
                                {
                                    titles = "UnitPrice";
                                }
                                else if ((z - 42) % 8 == 5)
                                {
                                    titles = "UnitWeight";
                                }
                                else if ((z - 42) % 8 == 6)
                                {
                                    titles = "ProductUrl";
                                }
                                else if ((z - 42) % 8 == 7)
                                {
                                    titles = "Remark";
                                }

                                PropertyInfo pi2 = propertysList.FirstOrDefault(p => p.Name == titles);

                                object subObj = pi2.GetValue(listItem, null); //取对应行与列的值
                                if (subObj != null)
                                {
                                    hsBodyRow.CreateCell(z).SetCellValue(subObj.ToString());
                                }
                                else
                                {
                                    hsBodyRow.CreateCell(z).SetCellValue("");
                                }
                                z++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (obj != null) //geying
                        {
                            var value = obj.ToString();
                            if (obj.ToString() == "False")
                                value = "否";
                            else if (obj.ToString() == "True")
                                value = "是";
                            hsBodyRow.CreateCell(ycBody).SetCellValue(value);
                            continue;
                        }
                        else //
                        {
                            hsBodyRow.CreateCell(ycBody).SetCellValue("");
                        }
                    }
                }
            }

            WriteToStream(workbook);
        }


	    /// <summary>
        /// 将List数据导入到EXCEL中  
        /// by:bookers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lst"></param>
        /// <param name="lstTitle"></param>
        /// <param name="isContainGenericList">是否导出包含泛型列表的数据 默认开启</param>
        public static void ListExcel<T>(string fileName, List<T> lst, List<string> lstTitle, bool isContainGenericList = true,int execlType=1)
	    {
	        IWorkbook workbook;
	        if (execlType == 1)
	        {
	            InitExcel(fileName);
	            workbook = InitializeWorkbook();
	        }
	        else
	        {
	            InitExcelx(fileName);
	            workbook = InitializeXWorkbook();
	        }

	        T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();
            #region header set value
            ISheet sheet1 = workbook.CreateSheet("sheet1");
            if (lstTitle==null)
            {
                WriteToStream(workbook);
                return;
            }
             List<string> lstFieldName=new List<string>();
                //给定的标题为空,赋值T默认的列名
                IRow hsTitleRow = sheet1.CreateRow(0);
                //标题赋值
            for (int yt = 0; yt < lstTitle.Count; yt++)
            {
                if (lstTitle[yt].IndexOf("-", StringComparison.Ordinal) != -1)
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    lstFieldName.Add(lstTitle[yt].Split('-')[0].ToUpperInvariant());
                }
                else
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    lstFieldName.Add(lstTitle[yt].ToUpperInvariant());
                }
            }

            #endregion

            for (int xcount = 0; xcount < lst.Count; xcount++) //读取List里的记录并填充到Excel的行中
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount + 1); //创建Excel行
                int rawPropertyCount = propertys.Count();
                for (int ycBody = 0; ycBody < rawPropertyCount; ycBody++)
                {
                    string fieldName = propertys[ycBody].Name;
                    object obj = propertys[ycBody].GetValue(lst[xcount], null);
                    if (obj == null)
                        continue;
                    if (obj.GetType().IsGenericType && isContainGenericList &&
                        propertys[ycBody].PropertyType.IsGenericType)
                    {
                        Type objType = obj.GetType();
                        int count = Convert.ToInt32(objType.GetProperty("Count").GetValue(obj, null));
                        for (int i = 0; i < count; i++)
                        {
                            object listItem = objType.GetProperty("Item").GetValue(obj, new object[] {i});
                            PropertyInfo[] propertyInfos = listItem.GetType().GetProperties();

                            for (int j = 0; j < propertyInfos.Count(); j++)
                            {
                                var objValue = propertyInfos[j].GetValue(listItem, null);
                                var index =lstFieldName.FindIndex( s => s.Equals((propertyInfos[j].Name + (i + 1).ToString()).ToUpperInvariant(),StringComparison.Ordinal));
                                if (index < 0) continue;
                                hsBodyRow.CreateCell(index).SetCellValue(objValue != null ? objValue.ToString() : "");
                            }
                        }
                    }
                    else
                    {
                        var value = obj.ToString();
                        if (obj.ToString() == "False")
                            value = "否";
                        else if (obj.ToString() == "True")
                            value = "是";

                        var index = lstFieldName.FindIndex(s => s.Equals(fieldName.ToUpperInvariant(), StringComparison.Ordinal));
                        if (index < 0) continue;
                        hsBodyRow.CreateCell(index).SetCellValue(value); //写入数据到Excel行中
                    }
                }

            }
            WriteToStream(workbook);
        }


        /// <summary>
        /// EUB模板导出
        /// Add By zhengsong
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lst"></param>
        /// <param name="lstTitle"></param>
        /// <param name="isContainGenericList"></param>
        public static void EUBListExcel<T>(string fileName, List<T> lst, List<string> lstTitle, bool isContainGenericList = true)
        {
            InitExcel(fileName);

            HSSFWorkbook workbook = InitializeWorkbook();

            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();
            #region header set value
            ISheet sheet1 = workbook.CreateSheet("sheet1");
            sheet1.SetColumnWidth(1, 30*256);
            if (lstTitle == null)
            {
                WriteToStream(workbook);
                return;
            }
            List<string> lstFieldName = new List<string>();
            //给定的标题为空,赋值T默认的列名
            IRow hsTitleRow = sheet1.CreateRow(0);
            //标题赋值
            for (int yt = 0; yt < lstTitle.Count; yt++)
            {
                if (lstTitle[yt].IndexOf("-", StringComparison.Ordinal) != -1)
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    lstFieldName.Add(lstTitle[yt].Split('-')[0].ToUpperInvariant());
                }
                else
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    lstFieldName.Add(lstTitle[yt].ToUpperInvariant());
                }
            }

            #endregion
            ICellStyle style = workbook.CreateCellStyle();
            for (int xcount = 0; xcount < lst.Count; xcount++) //读取List里的记录并填充到Excel的行中
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount + 1); //创建Excel行
                int rawPropertyCount = propertys.Count();
                for (int ycBody = 0; ycBody < rawPropertyCount; ycBody++)
                {
                    string fieldName = propertys[ycBody].Name;
                    object obj = propertys[ycBody].GetValue(lst[xcount], null);
                    if (obj == null)
                        continue;
                    if (obj.GetType().IsGenericType && isContainGenericList &&
                        propertys[ycBody].PropertyType.IsGenericType)
                    {
                        Type objType = obj.GetType();
                        int count = Convert.ToInt32(objType.GetProperty("Count").GetValue(obj, null));
                        for (int i = 0; i < count; i++)
                        {
                            object listItem = objType.GetProperty("Item").GetValue(obj, new object[] {i});
                            PropertyInfo[] propertyInfos = listItem.GetType().GetProperties();

                            for (int j = 0; j < propertyInfos.Count(); j++)
                            {
                                var objValue = propertyInfos[j].GetValue(listItem, null);
                                var index =
                                    lstFieldName.FindIndex(
                                        s =>
                                        s.Equals((propertyInfos[j].Name + (i + 1).ToString()).ToUpperInvariant(),
                                                 StringComparison.Ordinal));
                                if (index < 0) continue;
                                hsBodyRow.CreateCell(index).SetCellValue(objValue != null ? objValue.ToString() : "");
                            }
                        }
                    }
                    else
                    {
                        var value = obj.ToString();
                        if (obj.ToString() == "False")
                            value = "否";
                        else if (obj.ToString() == "True")
                            value = "是";

                        var index =
                            lstFieldName.FindIndex(s => s.Equals(fieldName.ToUpperInvariant(), StringComparison.Ordinal));
                        if (index < 0) continue;
                        hsBodyRow.CreateCell(index).SetCellValue(value); //写入数据到Excel行中
                    }
                }
                Type objFile = lst[xcount].GetType();
                byte[] file = objFile.GetProperty("TrackingNumberFile").GetValue(lst[xcount], null) as byte[];
                //给execl 赋一个行高值
                if (file != null)
                {
                    hsBodyRow.HeightInPoints = 63;
                    AddPieChart(sheet1, workbook, file, (xcount + 1), 1);
                }
            }
            WriteToStream(workbook);
        }

        public static void AddPieChart(ISheet sheet, HSSFWorkbook workbook, byte[] bytes, int row, int col)
        {
            try
            {
                //add picture data to this workbook.
                //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/html/");
                //if (fileurl.Contains("/"))
                //{
                //    path += fileurl.Substring(fileurl.IndexOf('/'));
                //}
                //string FileName = path;
                //byte[] bytes = File.ReadAllBytes(FileName);

                if ( bytes != null && bytes.Length>0)
                {
                    int pictureIdx = workbook.AddPicture(bytes, PictureType.PNG);
                    HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                    HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, col, row, col + 1 , row + 1);
                    //##处理照片位置，【图片左上角为（col, row）第row+1行col+1列，右下角为（ col +1, row +1）第 col +1+1行row +1+1列，宽为100，高为50

                    HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    //这句话一定不要，这是用图片原始大小来显示
                    //pict.Resize();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 导出入仓列表
        /// Add by zhengsong
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lst"></param>
        /// <param name="lstTitle"></param>
        /// <param name="inStorageId"></param>
        /// <param name="shippingMehtodName"></param>
        /// <param name="toltaWeight"></param>
        /// <param name="tolta"></param>
        /// <param name="isContainGenericList"></param>
        public static void InStorageInfExcel<T>(string fileName, List<T> lst, List<string> lstTitle,string inStorageId,string shippingMehtodName,decimal toltaWeight,int tolta, bool isContainGenericList = true)
        {
            InitExcel(fileName);

            HSSFWorkbook workbook = InitializeWorkbook();

            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();
            #region header set value
            ISheet sheet1 = workbook.CreateSheet("sheet1");
            if (lstTitle == null)
            {
                WriteToStream(workbook);
                return;
            }


            //sheet1.AddMergedRegion(new Region(0, 2, 0, 3));
            IRow NumberRow = sheet1.CreateRow(0);
            IRow ShippingRow = sheet1.CreateRow(1);
            NumberRow.CreateCell(0).SetCellValue("编号:");
            NumberRow.CreateCell(1).SetCellValue(inStorageId);
            NumberRow.CreateCell(2).SetCellValue("*"+inStorageId+"*");
            NumberRow.CreateCell(4).SetCellValue("账号:");
            NumberRow.CreateCell(5).SetCellValue("");
            ShippingRow.CreateCell(0).SetCellValue("渠道:");
            ShippingRow.CreateCell(1).SetCellValue(shippingMehtodName);
            ShippingRow.CreateCell(2).SetCellValue("总重量Kg:");
            ShippingRow.CreateCell(3).SetCellValue(toltaWeight.ToString("F2"));
            ShippingRow.CreateCell(4).SetCellValue("总件数:");
            ShippingRow.CreateCell(5).SetCellValue(tolta);
            List<string> lstFieldName = new List<string>();
            //给定的标题为空,赋值T默认的列名
            IRow hsTitleRow = sheet1.CreateRow(3);
            //标题赋值
            for (int yt = 0; yt < lstTitle.Count; yt++)
            {
                if (lstTitle[yt].IndexOf("-", StringComparison.Ordinal) != -1)
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt].Split('-')[1]);
                    lstFieldName.Add(lstTitle[yt].Split('-')[0].ToUpperInvariant());
                }
                else
                {
                    hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
                    lstFieldName.Add(lstTitle[yt].ToUpperInvariant());
                }
            }

            #endregion

            for (int xcount = 0; xcount < lst.Count; xcount++) //读取List里的记录并填充到Excel的行中
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount + 4); //创建Excel行
                int rawPropertyCount = propertys.Count();
                for (int ycBody = 0; ycBody < rawPropertyCount; ycBody++)
                {
                    string fieldName = propertys[ycBody].Name;
                    object obj = propertys[ycBody].GetValue(lst[xcount], null);
                    if (obj == null)
                        continue;
                    if (obj.GetType().IsGenericType && isContainGenericList &&
                        propertys[ycBody].PropertyType.IsGenericType)
                    {
                        Type objType = obj.GetType();
                        int count = Convert.ToInt32(objType.GetProperty("Count").GetValue(obj, null));
                        for (int i = 0; i < count; i++)
                        {
                            object listItem = objType.GetProperty("Item").GetValue(obj, new object[] { i });
                            PropertyInfo[] propertyInfos = listItem.GetType().GetProperties();

                            for (int j = 0; j < propertyInfos.Count(); j++)
                            {
                                var objValue = propertyInfos[j].GetValue(listItem, null);
                                var index = lstFieldName.FindIndex(s => s.Equals((propertyInfos[j].Name + (i + 1).ToString()).ToUpperInvariant(), StringComparison.Ordinal));
                                if (index < 0) continue;
                                hsBodyRow.CreateCell(index).SetCellValue(objValue != null ? objValue.ToString() : "");
                            }
                        }
                    }
                    else
                    {
                        var value = obj.ToString();
                        if (obj.ToString() == "False")
                            value = "否";
                        else if (obj.ToString() == "True")
                            value = "是";

                        var index = lstFieldName.FindIndex(s => s.Equals(fieldName.ToUpperInvariant(), StringComparison.Ordinal));
                        if (index < 0) continue;
                        hsBodyRow.CreateCell(index).SetCellValue(value); //写入数据到Excel行中
                    }
                }

            }
            WriteToStream(workbook);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <param name="list"></param>
        /// <param name="listTile"></param>
        /// <param name="ConvertDataColumns"></param>
        /// <param name="execlType"> 1-xls,2-xlsx</param>
        private static void WriteListDataToExcel<T>(string fileName, string sheetName, List<T> list, List<string> listTile, Dictionary<string, Dictionary<int, string>> ConvertDataColumns,int execlType=1)
        {
            IWorkbook workbook;
            if (execlType == 1)
            {
                InitExcel(fileName);
                workbook = InitializeWorkbook();
            }
            else
            {
                InitExcelx(fileName);
                workbook = InitializeXWorkbook();
            }

            //ICellStyle style=  workbook.GetSheet("Sheet1").GetRow(3).RowStyle;

            //填充数据
            if (sheetName != null) ListExcel(workbook, list, listTile, ConvertDataColumns, 1, true);

            WriteToStream(workbook);


        }


        public static void WriteToStream(IWorkbook workbook,string savePath="")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(savePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {

                        workbook.Write(ms);
                        if (workbook as XSSFWorkbook != null)
                        {
                            using (MemoryStream newms = new MemoryStream(ms.GetBuffer()))
                            {
                                long fileSize = newms.Length;
                                //加上设置大小下来的.xlsx文件打开时才没有错误
                                HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
                            }
                        }
                        HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
                        
                    }
                }
                else
                {
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fileStream);
                        HttpContext.Current.Response.WriteFile(savePath);
                    }
                }
                if (HttpContext.Current.Response.IsClientConnected)
                {
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
            catch (ThreadAbortException ex)
            {
                Log.Exception(new Exception("NPOI ExcelHelper Export Exception!", ex));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static HSSFWorkbook InitializeWorkbook()
        {
            HSSFWorkbook _hssfworkbook = new HSSFWorkbook();
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "";
            _hssfworkbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "";
            _hssfworkbook.SummaryInformation = si;
            return _hssfworkbook;
        }
        /// <summary>
        /// xlsx初始化
        /// </summary>
        /// <returns></returns>
        static XSSFWorkbook InitializeXWorkbook()
        {
            XSSFWorkbook _xssfWorkbook = new XSSFWorkbook();
            _xssfWorkbook.GetProperties().CoreProperties.Subject = "";
            _xssfWorkbook.GetProperties().CoreProperties.Revision = "";
            return _xssfWorkbook;
        }

        /// <summary>
        /// 写入数据到excel指定模板
        /// </summary>
        /// <param name="filePath">模板路径</param>
        /// <param name="newFilePath">生成新的文件路径</param>
        /// <param name="fileName">创建的excel文件名字</param>
        /// <param name="list">需要填充的数据源</param>
        /// <param name="startIndex">开始填充的索引值</param>
        public static void WriteExcelTemplateData<T>(string filePath, List<T> list, List<string> columnsList, Dictionary<string, Dictionary<int, string>> ConvertDataColumns, int startIndex, bool isTitle,string sheetName="sheet1")
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHH:mm:ss");
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(fs);


                ListExcel(workbook, list, columnsList, null, startIndex, false, sheetName);


                InitExcel(fileName);

                WriteToStream(workbook);
            }
        }

        public static void InitExcel(string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xls"));
            HttpContext.Current.Response.Clear();
        }
        public static void InitExcelx(string fileName)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}",fileName + ".xlsx"));
        }

        public static void WriteDataToReturnOrderTemplate<T>(string filePath, List<T> lst, List<string> lstTitle, int startIndex, string businessModel, string processMethod)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheet("Sheet1");
                sheet.GetRow(1).GetCell(1).SetCellValue(businessModel);
                sheet.GetRow(1).GetCell(5).SetCellValue(processMethod);

                IRow lastRow = sheet.GetRow(4);


                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();

                for (int xcount = 0; xcount < lst.Count; xcount++)
                {
                    IRow hsBodyRow = sheet.CreateRow(startIndex);

                    ICellStyle style = workbook.CreateCellStyle();
                    style.BorderBottom = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderTop = BorderStyle.Thin;

                    for (int ycBody = 0; ycBody < lstTitle.Count; ycBody++)
                    {
                        string title = string.Empty;
                        if (lstTitle[ycBody].IndexOf("-") != -1)
                        {
                            title = lstTitle[ycBody].Split('-')[0];
                        }
                        else
                        {
                            title = lstTitle[ycBody];
                        }
                        PropertyInfo pi = propertys.FirstOrDefault(p => p.Name == title);
                        if (pi == null)
                        {
                            if (title == "TotalPrice")
                            {
                                object quantity = 0;
                                object purchaserPrice = 0;
                                foreach (var pty in propertys)
                                {
                                    if (pty.Name == "Quantity")
                                    {

                                        quantity = (int)pty.GetValue(lst[xcount], null);
                                    }
                                    if (pty.Name == "PurchasePrice")
                                    {
                                        purchaserPrice = (decimal)pty.GetValue(lst[xcount], null);
                                    }
                                }
                                var total = decimal.Round(int.Parse(quantity.ToString()) * decimal.Parse(purchaserPrice.ToString()), 2);

                                hsBodyRow.CreateCell(ycBody).SetCellValue(total.ToString());
                                hsBodyRow.GetCell(ycBody).CellStyle = style;
                                continue;
                            }
                            else
                                continue;

                        }
                        object obj = pi.GetValue(lst[xcount], null);
                        if (obj != null)
                        {
                            var value = obj.ToString();
                            if (obj.ToString() == "False")
                                value = "0";
                            else if (obj.ToString() == "True")
                                value = "1";
                            else
                                hsBodyRow.CreateCell(ycBody).SetCellValue(value);
                            hsBodyRow.GetCell(ycBody).CellStyle = style;
                        }
                        else
                        {
                            hsBodyRow.CreateCell(ycBody).SetCellValue("");
                        }
                    }
                    startIndex++;
                }


                IRow row1 = sheet.CreateRow(lst.Count + 3);
                for (int i = 0; i < lastRow.Cells.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(lastRow.GetCell(i).StringCellValue);
                    sheet.SetColumnWidth(i, 15 * 256);
                }
                IFont fontcolorblue = workbook.CreateFont();
                fontcolorblue.IsItalic = true;//下划线  
                fontcolorblue.FontHeightInPoints = 15;
                fontcolorblue.Underline = FontUnderlineType.Single;
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.SetFont(fontcolorblue);
                row1.CreateCell(1).SetCellValue("               ");
                row1.GetCell(1).CellStyle = cellStyle;
                row1.CreateCell(3).SetCellValue("               ");
                row1.GetCell(3).CellStyle = cellStyle;

                row1.CreateCell(6).SetCellValue("               ");
                row1.GetCell(6).CellStyle = cellStyle;

                //sheet.AddMergedRegion(new CellRangeAddress(10, 1, 2, 2));
                InitExcel(DateTime.Now.ToString("yyyymmddHH:mm:ss"));
                WriteToStream(workbook);
            }
        }
    }
}
