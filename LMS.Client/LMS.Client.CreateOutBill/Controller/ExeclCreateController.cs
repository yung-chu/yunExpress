using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Client.CreateOutBill.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace LMS.Client.CreateOutBill.Controller
{
    public class ExeclCreateController
    {
        private static readonly string path = System.Configuration.ConfigurationManager.AppSettings["ExportFileXlsPath"].ToString();

        /// <summary>
        /// 根据收货出账单号生成Execl
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="list"></param>
        public static void ExeclCreate(ReceivingBill bill, List<BillModel> list)
        {
            Log.Info("LMS开始生成账单号为：{0}execl表格".FormatWith(bill.ReceivingBillID));
            var groupShipping = list.GroupBy(p => p.InShippingMethodName).Select(g => new
                {
                    ShippingMethodName = g.Key,
                    TotalFee = g.Sum(p => p.Freight + p.FuelCharge +
                                        p.Register + p.Surcharge +
                                        p.TariffPrepayFee +
                                        p.SpecialFee+p.RemoteAreaFee)
                }).ToList();
            using (
                var file = new FileStream(
                    AppDomain.CurrentDomain.BaseDirectory + "ExcelTemplate/FinancialCreditBill.xls", FileMode.Open,
                    FileAccess.Read))
            {
                IWorkbook hssfworkbook = new HSSFWorkbook(file);
                ISheet sheetNew = hssfworkbook.CloneSheet(0);
                sheetNew.GetRow(1).GetCell(0).SetCellValue("客户名称：{0}".FormatWith(bill.CustomerName));
                sheetNew.GetRow(1).GetCell(1).SetCellValue("账单结算期间：{0}至{1}".FormatWith(DateTime.Parse(bill.BillStartTime).ToString("yyyy-MM-dd"), DateTime.Parse(bill.BillEndTime).ToString("yyyy-MM-dd")));
                IRow sourceRow = sheetNew.GetRow(2);
                int startRow = 2;
                foreach (var t in groupShipping)
                {
                    startRow++;
                    sheetNew.ShiftRows(startRow, sheetNew.LastRowNum, 1, true, true);
                    IRow row = sheetNew.CreateRow(startRow);
                    CopyRow(sourceRow,ref row);
                    row.GetCell(0).SetCellValue(t.ShippingMethodName);
                    row.GetCell(1).SetCellValue("总费用：{0}".FormatWith(t.TotalFee.ToString("F2")));
                }
                startRow++;
                sheetNew.ShiftRows(startRow, sheetNew.LastRowNum, 1, true, true);
                IRow srow = sheetNew.CreateRow(startRow);
                CopyRow(sourceRow, ref srow);
                srow.GetCell(0).SetCellValue("结算人：{0}".FormatWith(bill.ReceivingBillAuditor));

                startRow = startRow + 3;
                int startR = startRow;
                foreach (var b in list)
                {
                    IRow row = sheetNew.CreateRow(startRow);
                    row.CreateCell(0).SetCellValue(b.WayBillNumber);
                    row.CreateCell(1).SetCellValue(b.CustomerOrderNumber??"");
                    row.CreateCell(2).SetCellValue(b.CreatedOn.ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(3).SetCellValue(b.InStorageCreatedOn.ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(4).SetCellValue(b.TrackingNumber??"");
                    row.CreateCell(5).SetCellValue(b.ChineseName);
                    row.CreateCell(6).SetCellValue(b.InShippingMethodName);
                    row.CreateCell(7).SetCellValue(b.SettleWeight.ToString("F4"));
                    row.CreateCell(8).SetCellValue(b.Weight.ToString("F4"));
                    row.CreateCell(9).SetCellValue(b.CountNumber);
                    row.CreateCell(10).SetCellValue(double.Parse(b.Freight.ToString("F2")));
                    row.CreateCell(11).SetCellValue(double.Parse(b.Register.ToString("F2")));
                    row.CreateCell(12).SetCellValue(double.Parse(b.FuelCharge.ToString("F2")));
                    row.CreateCell(13).SetCellValue(double.Parse(b.Surcharge.ToString("F2")));
                    row.CreateCell(14).SetCellValue(double.Parse(b.TariffPrepayFee.ToString("F2")));
                    row.CreateCell(15).SetCellValue(double.Parse(b.SpecialFee.ToString("F2")));
                    row.CreateCell(16).SetCellValue(double.Parse(b.RemoteAreaFee.ToString("F2")));
                    startRow++;
                    row.CreateCell(17).SetCellFormula("sum(K" + startRow + ":Q" + startRow + ")");
                }
                IRow totalRow = sheetNew.CreateRow(startRow);
                for (int i = 0; i < 18; i++)
                {
                    totalRow.CreateCell(i).SetCellValue("");
                }
                startR++;
                totalRow.Cells[9].SetCellValue("各项计费小计：");
                totalRow.Cells[10].SetCellFormula("sum(K" + startR + ":K" + startRow + ")");
                totalRow.Cells[11].SetCellFormula("sum(L" + startR + ":L" + startRow + ")");
                totalRow.Cells[12].SetCellFormula("sum(M" + startR + ":M" + startRow + ")");
                totalRow.Cells[13].SetCellFormula("sum(N" + startR + ":N" + startRow + ")");
                totalRow.Cells[14].SetCellFormula("sum(O" + startR + ":O" + startRow + ")");
                totalRow.Cells[15].SetCellFormula("sum(P" + startR + ":P" + startRow + ")");
                totalRow.Cells[16].SetCellFormula("sum(Q" + startR + ":Q" + startRow + ")");
                totalRow.Cells[17].SetCellFormula("sum(R" + startR + ":R" + startRow + ")");
                hssfworkbook.SetSheetName(hssfworkbook.GetSheetIndex(sheetNew), bill.ReceivingBillID);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(Path.Combine(path, bill.ReceivingBillID + ".xls")))
                {
                    File.Delete(Path.Combine(path, bill.ReceivingBillID + ".xls"));
                }
                var savefile = new FileStream(Path.Combine(path, bill.ReceivingBillID + ".xls"), FileMode.Create);
                hssfworkbook.RemoveSheetAt(0);
                hssfworkbook.Write(savefile);
                file.Close();
            }
            Log.Info("LMS完成生成账单号为：{0}execl表格".FormatWith(bill.ReceivingBillID));
        }
        /// <summary>
        /// 复制行样式
        /// </summary>
        /// <param name="sourceRow"></param>
        /// <param name="toRow"></param>
        public static void CopyRow(IRow sourceRow, ref IRow toRow)
        {
            toRow.Height = sourceRow.Height;
            for (int m = sourceRow.FirstCellNum; m < sourceRow.LastCellNum; m++)
            {
                ICell cell = toRow.CreateCell(m);
                ICell sourcecell = sourceRow.GetCell(m);
                if (sourcecell != null)
                {
                    cell.CellStyle = sourcecell.CellStyle;
                    cell.CellStyle.Alignment = HorizontalAlignment.LEFT;
                    cell.SetCellType(sourcecell.CellType);
                }
            }
        }
    }
}
