using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace LighTake.Infrastructure.Common.Excel
{
    public class ExcelOperation
    {

        /// <summary>
        /// 根据完整的文件路径名获取当前excel的sheetName
        /// </summary>
        /// <param name="filepath">完整excel文件路径名</param>
        /// <returns>所有sheetName名称</returns>
        public ArrayList ExcelSheetName(string fileName)
        {
            ArrayList al = new ArrayList();
            string strConn;
            string fileExt = Path.GetExtension(fileName).ToLower();
            if (fileExt == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=Excel 8.0;";
            }
            else
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=Excel 12.0;";
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable sheetNames = conn.GetOleDbSchemaTable
            (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn.Close();
            foreach (DataRow dr in sheetNames.Rows)
            {
                al.Add(dr[2]);
            }
            return al;
        }

        /// <summary>
        /// 根据fileName及sheetName加载excel内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetname"></param>
        /// <returns></returns>
        public DataSet ExcelDataSource(string fileName, string sheetname)
        {
            string strConn;
            string fileExt = Path.GetExtension(fileName);
            if (fileExt == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName +
                          ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
            }
            else
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName +
                          ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
            }
            DataSet ds = new DataSet();
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                string str = "select * from [" + sheetname + "]";
                OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
                da.Fill(ds, sheetname);
                conn.Close();
            }
            return ds;
        }
    }
}
