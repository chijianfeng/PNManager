using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using ExcelOper.Data;
using System.Windows;

namespace ExcelOper
{
    class ExcelReader
    {

        private string excelpath = null;

        public ExcelReader(string path)
        {
            excelpath = path;
        }

        public List<SheetInfo> GetSheetlist()
        {
            if (excelpath == null||excelpath.Length<=0)
                return null;
            List<SheetInfo> list = new List<SheetInfo>();
            string connStr = "";
            string sql_F = "Select * FROM [{0}]";
            string fileType = System.IO.Path.GetExtension(excelpath);
            if (fileType == ".xls")
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excelpath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + excelpath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

            OleDbConnection conn = new OleDbConnection(connStr);
            OleDbDataAdapter da = null;
            try
            {
                conn.Open();
                string sheetname = "";
                DataTable dtSheetName = 
                    conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                da = new OleDbDataAdapter();
                for (int i = 0; i < dtSheetName.Rows.Count;i++ )
                {
                    sheetname = (string)dtSheetName.Rows[i]["TABLE_NAME"];
                    if (sheetname.Contains("$") )
                    {
                        SheetInfo info = new SheetInfo();
                        info.SheetName = sheetname.Replace("$", "");

                        da.SelectCommand = new OleDbCommand(String.Format(sql_F, sheetname), conn);
                        DataSet dsItem = new DataSet();
                        da.Fill(dsItem, sheetname);
                        int cnum = dsItem.Tables[0].Columns.Count;
                        int rnum = dsItem.Tables[0].Rows.Count;
                        info.StartRange = "A1";
                        char c = (char)('A' + cnum - 1);
                        info.EndRange = c + Convert.ToString(rnum);
                        list.Add(info);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误消息");
                return null; 
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    if(da!=null)
                        da.Dispose();
                    conn.Dispose();
                }
            }

            return list;
        }

        public DataSet Read(string sheetname)       //获取Excel的数据
        {
            string connStr = "";
            string fileType = System.IO.Path.GetExtension(excelpath);
            if (string.IsNullOrEmpty(fileType)) return null;

            if (fileType == ".xls")
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excelpath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + excelpath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            string sql_F = "Select * FROM [{0}]";

            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            DataTable dtSheetName = null;

            DataSet ds = new DataSet();

            try
            {
                conn = new OleDbConnection(connStr);
                conn.Open();

                // 获取数据源的表定义元数据                         
                string SheetName = "";
                dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                da = new OleDbDataAdapter();
                sheetname += "$";
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    SheetName = (string)dtSheetName.Rows[i]["TABLE_NAME"];

                    if (SheetName.Contains("$") && !SheetName.Replace("'", "").EndsWith("$"))
                    {
                        continue;
                    }

                    if (!sheetname.Equals(SheetName))
                        continue;
                    da.SelectCommand = new OleDbCommand(String.Format(sql_F, SheetName), conn);
                    DataSet dsItem = new DataSet();
                    da.Fill(dsItem, SheetName);

                    ds.Tables.Add(dsItem.Tables[0].Copy());
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString() , "错误消息" );
                return null; 
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    if(da!=null)
                        da.Dispose();
                    conn.Dispose();
                }
            }
            return ds;
        }
    }
}
