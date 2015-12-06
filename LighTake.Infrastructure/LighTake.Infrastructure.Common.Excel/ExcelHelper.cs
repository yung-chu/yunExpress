using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Web;
using LighTake.Infrastructure.Common.Logging;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using System.Data;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LighTake.Infrastructure.Common.Excel
{

    /// <summary>
    /// Excel操作类（NPOI）
    /// </summary>
    /// <author>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2012/4/17 14:19
    /// 修改历史 : 无
    /// </author>
    public partial class ExcelHelper
    {

        /// <summary>
        /// 初始化
        /// </summary>
        static HSSFWorkbook InitializeWorkbook()
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
        static XSSFWorkbook InitializeXWorkbook()
        {
            XSSFWorkbook _xssfWorkbook = new XSSFWorkbook();
            _xssfWorkbook.GetProperties().CoreProperties.Subject = "";
            _xssfWorkbook.GetProperties().CoreProperties.Revision = "";
            return _xssfWorkbook;
        }

        /// <summary>
        /// DataTable写入Excel
        /// </summary>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="dt">要写入的DataTable </param>
        public static void WriteToDownLoad(string fileName, string sheetName, DataTable dt)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            string filename = fileName;
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            HttpContext.Current.Response.Clear();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(fileName).ToLower();
            //初始化Excel信息
            if (fileExt == ".xls")
            {
                workbook = InitializeWorkbook();
            }
            else
            {
                workbook = InitializeXWorkbook();
            }
            //填充数据
            DTExcel(sheetName, dt, null, workbook);

            HttpContext.Current.Response.BinaryWrite(WriteToStream(workbook).GetBuffer());
            if (HttpContext.Current.Response.IsClientConnected)
            {
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }

 

        /// <summary>
        /// List写入Excel
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="lst">要写入的List</param>
        /// <param name="listTile">实体中需要的列名（默认为所有）</param>
        public static void WriteToDownLoad<T>(string fileName, string sheetName, List<T> lst, Dictionary<string, string> listTile)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            string filename = fileName;
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            HttpContext.Current.Response.Clear();

            IWorkbook workbook;
            string fileExt = Path.GetExtension(fileName).ToLower();
            //初始化Excel信息
            if (fileExt == ".xls")
            {
                workbook = InitializeWorkbook();
            }
            else
            {
                workbook = InitializeXWorkbook();
            }

            //填充数据
            if (sheetName != null) ListExcel(sheetName, lst, listTile, workbook);
            try
            {
                HttpContext.Current.Response.BinaryWrite(WriteToStream(workbook).GetBuffer());
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
        /// List写入Excel
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="fileName">要保存的文件名称 eg:test.xls</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="lst">要写入的List</param>
        /// <param name="listTile">实体中需要的列名（默认为所有）</param>
        public static void WriteToDownLoad<T>(string fileName, string sheetName, List<T> lst, List<string> listTile = null)
        {
            Dictionary<string, string> lstTitle = null;

            if (listTile != null)
            {
                lstTitle = listTile.ToDictionary(p => p, p => p);
            }

            WriteToDownLoad(fileName, sheetName, lst, lstTitle);
        }

        public static DataTable ReadToDataTable(string filePath)
        {
            string fileExt = Path.GetExtension(filePath).ToLower();
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return ReadToDataTable(stream,fileExt);
            }
        }

        public static DataTable ReadToDataTable(Stream stream, string fileExt)
        {
            DataTable dtResult = new DataTable();
            IWorkbook workbook;
            if (fileExt == ".xls")
            {
                workbook = new HSSFWorkbook(stream);
            }
            else
            {
                workbook=new XSSFWorkbook(stream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            //获取sheet的首行
            IRow headerRow = sheet.GetRow(0);
            for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dtResult.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i < (sheet.LastRowNum + 1); i++)
            {

                IRow row = sheet.GetRow(i);
                if (row == null) throw new Exception("读取excel出错了没有获取到数据");

                //设置为字符串格式 周小强 5.19
                //row.Cells.ForEach(p =>
                //{
                //    if (p.CellType == CellType.NUMERIC)
                //        p.SetCellType(CellType.STRING);
                //    else if (p.CellType == CellType.FORMULA)
                //    {
                //        p.SetCellValue(p.NumericCellValue);
                //        p.SetCellType(CellType.STRING);
                //    }                    
                //});

                //读取支持多种类型包括时间，数字，字符串, 周建春 2014-7-10
                if (row.Cells.Any(p => p.CellType !=  CellType.Blank))
                {
                    DataRow dataRow = dtResult.NewRow();
                    for (int j = row.FirstCellNum; j < headerRow.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell != null)
                        {
                            if (cell.CellType == CellType.Numeric) 
                            {
                                if (DateUtil.IsCellDateFormatted(row.GetCell(j)))//datetime
                                {
                                    dataRow[j] = DateUtil.GetJavaDate(row.GetCell(j).NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).NumericCellValue;
                                }
                            }
                            else if (cell.CellType == CellType.Formula)
                            {
                                dataRow[j] = row.GetCell(j).NumericCellValue;
                            }
                            else
                            {
                                dataRow[j] = row.GetCell(j).StringCellValue;
                            }
                        }
                    }
                    dtResult.Rows.Add(dataRow);
                }
                else
                {
                    break;
                }
            }

            return dtResult;
        }

        public static byte[] ConvertToExcelByte<T>(string sheetName, List<T> data, List<ExcelRowConfig> configs)
        {
            IWorkbook workbook = InitializeWorkbook();
            if (sheetName != null)
                ListExcel(sheetName, data, configs, workbook);

            return WriteToStream(workbook).GetBuffer();
        }

        static MemoryStream WriteToStream(IWorkbook workbook)
        {
            var file = new MemoryStream();
            workbook.Write(file);
            file.Flush();
            return file;
        }

        #region 数据填充部分

        /// <summary>
        /// 将DataTable数据写入到Excel
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="dt"></param>
        /// <param name="lstTitle"></param>
        /// <param name="workbook"></param>
        static void DTExcel(string sheetName, DataTable dt, List<string> lstTitle, IWorkbook workbook)
        {
            IWorkbook _hssfworkbook = workbook;

            ISheet sheet1 = _hssfworkbook.CreateSheet(sheetName);
            int y = dt.Columns.Count;
            int x = dt.Rows.Count;

            //给定的标题为空,赋值datatable默认的列名
            if (lstTitle == null)
            {
                lstTitle = new List<string>();
                for (int ycount = 0; ycount < y; ycount++)
                {
                    lstTitle.Add(dt.Columns[ycount].ColumnName);
                }
            }

            IRow hsTitleRow = sheet1.CreateRow(0);
            //标题赋值
            for (int yt = 0; yt < lstTitle.Count; yt++)
            {
                hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[yt]);
            }

            //填充数据项
            for (int xcount = 1; xcount < x; xcount++)
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount);

                for (int ycBody = 0; ycBody < y; ycBody++)
                {
                    hsBodyRow.CreateCell(ycBody).SetCellValue(dt.DefaultView[xcount - 1][ycBody].ToString());
                }
            }

        }

        static void ListExcel<T>(string sheetName, List<T> lst, Dictionary<string, string> lstTitle, IWorkbook workbook)
        {
            IWorkbook _hssfworkbook = workbook;

            ISheet sheet1 = _hssfworkbook.CreateSheet(sheetName);

            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();

            //给定的标题为空,赋值T默认的列名
            var listTitle = new List<string>();
            if (lstTitle == null)
            {
                listTitle = propertys.Select(p => p.Name).ToList();
                lstTitle = propertys.ToDictionary(p => p.Name, p => p.Name);
            }
            else
            {
                listTitle = lstTitle.Select(p => p.Key).ToList();
            }

            IRow hsTitleRow = sheet1.CreateRow(0);
            //标题赋值

            int yt = 0;
            foreach (string key in lstTitle.Keys)
            {
                hsTitleRow.CreateCell(yt).SetCellValue(lstTitle[key]);
                yt++;
            }

            //填充数据项
            for (int xcount = 0; xcount < lst.Count; xcount++)
            {
                IRow hsBodyRow = sheet1.CreateRow(xcount + 1);

                for (int ycBody = 0; ycBody < lstTitle.Count; ycBody++)
                {
                    PropertyInfo pi = propertys.First(p => p.Name == listTitle[ycBody]);
                    if (listTitle.All(t => t != pi.Name))
                        continue;
                    object obj = pi.GetValue(lst[xcount], null);
                    hsBodyRow.CreateCell(ycBody).SetCellValue(obj != null ? obj.ToString() : "");
                }
            }

        }

        static void ListExcel<T>(string sheetName, List<T> data, IList<ExcelRowConfig> configs, IWorkbook workbook)
        {
            if (configs == null || configs.Count < 1)
                throw new ArgumentNullException("configs", "必须配置导出的列");

            var sheet = workbook.CreateSheet(sheetName);

            #region 创建标题行

            IRow hsTitleRow = sheet.CreateRow(0);
            //标题赋值
            for (int i = 0; i < configs.Count; i++)
                hsTitleRow.CreateCell(i).SetCellValue(configs[i].HeadText);

            #endregion

            //填充数据项
            PropertyInfo[] arrProperty = Activator.CreateInstance(typeof(T)).GetType().GetProperties();
            for (int xcount = 0; xcount < data.Count; xcount++)
            {
                IRow hsBodyRow = sheet.CreateRow(xcount + 1);
                for (int ycBody = 0; ycBody < configs.Count; ycBody++)
                {
                    PropertyInfo property = arrProperty.First(p => p.Name == configs[ycBody].FieldName);
                    if (property == null)
                        continue;

                    object obj = property.GetValue(data[xcount], null);
                    if (obj != null)        //geying
                    {
                        hsBodyRow.CreateCell(ycBody).SetCellValue(obj.ToString());
                    }
                    else
                    {
                        hsBodyRow.CreateCell(ycBody).SetCellValue("");
                    }
                }
            }
        }

        #endregion

        public static DataTable ReadRepeatHeadToDataTable(string filePath)
        {
            string fileExt = Path.GetExtension(filePath).ToLower();
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return ReadRepeatHeadToDataTable(stream,fileExt);
            }
        }

        public static DataTable ReadRepeatHeadToDataTable(Stream stream, string fileExt)
        {
            DataTable dtResult = new DataTable();
            IWorkbook workbook;
            if (fileExt == ".xls" || fileExt == ".et")
            {
                workbook = new HSSFWorkbook(stream);
            }
            else
            {
                workbook = new XSSFWorkbook(stream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            //获取sheet的首行
            IRow headerRow = sheet.GetRow(0);
            for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue + i);
                dtResult.Columns.Add(column);
            }
            int emptyRow = 0;
            for (int i = (sheet.FirstRowNum + 1); i < (sheet.LastRowNum + 1); i++)
            {
                IRow row = sheet.GetRow(i);
                //if (row == null) throw new Exception("读取excel出错了没有获取到数据");
                if (row == null)
                {
                    if (emptyRow >= 4)
                        break;
                    emptyRow++;
                    continue;
                }

                //设置为字符串格式 周小强 5.19
                row.Cells.ForEach(p =>
                {
                    if (p.CellType == CellType.Numeric)
                        p.SetCellType(CellType.String);
                    else if (p.CellType == CellType.Formula)
                    {
                        p.SetCellValue(p.NumericCellValue);
                        p.SetCellType(CellType.String);
                    }
                });
                if (row.Cells.Any(p => !string.IsNullOrWhiteSpace(p.StringCellValue)))
                {
                    DataRow dataRow = dtResult.NewRow();
                    for (int j = row.FirstCellNum; j < headerRow.LastCellNum; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }

                    dtResult.Rows.Add(dataRow);
                }
                else
                {
                    break;
                }
            }
            return dtResult;
        }
    }
}
