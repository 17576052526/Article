using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    public class ExcelHelper
    {
        #region Excel 与数据库的相互导入和导出
        /// <summary>
        /// DataTable导入Excel并下载到浏览器
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="fileName">要下载到浏览器时的文件名称，不用带后缀</param>
        public void ImportExcel(System.Data.DataTable dt, string fileName)
        {
            StringBuilder str = new StringBuilder();
            //列名
            foreach (DataColumn c in dt.Columns)
            {
                str.Append(c.ColumnName + "\t");
            }
            str.Append("\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    str.Append(Convert.ToString(dr[i]).Replace('\t', ' ').Replace('\n', ' ') + "\t");
                }
                str.Append("\n");
            }
            //下载
            HttpResponse resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            resp.Write(str.ToString());
            resp.End();
        }

        /// <summary>
        /// Excel导入到DataTable
        /// </summary>
        /// <param name="path">Excel路径</param>
        /// <param name="sheetName">excel的sheet名称</param>
        public System.Data.DataTable ExportExcel(string path, string sheetName)
        {
            //连接字符串
            string connString = null;
            string Suffix = path.Substring(path.LastIndexOf('.') + 1).ToLower();    //获取后缀
            if (Suffix == "xls")
            {
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=\"Excel 8.0;\"";
            }
            else if (Suffix == "xlsx")
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + "Extended Properties=\"Excel 8.0;\"";
            }
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                conn.Open();
                string sql = "select * from [" + sheetName + "$]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch { return null; }
            finally { conn.Close(); }
        }

        /// <summary>
        /// DataTable导入到数据库，导入成功返回True字符串，失败则返回异常信息
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="cols">数据库所对应excel中的列名</param>
        /// <param name="test">验证列的值的类型，验证失败则抛出异常（数字或小数类型：number,字符串类型：string,日期时间类型:datetime）</param>
        /// <param name="TabName">要导入的数据库表名</param>
        public string DataTableImportSql(System.Data.DataTable dt, string[] cols, string[] test, string TabName)
        {
            try
            {
                //构造新增sql字符串模板
                string temSql = "Insert Into " + TabName + "(";
                foreach (string s in cols)
                {
                    temSql += s + ",";
                }
                temSql = temSql.Remove(temSql.Length - 1, 1) + ") Values({0})\n";
                //构造sql语句
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string str = null;
                    for (int j = 0; j < cols.Length; j++)
                    {
                        string val = dr[j].ToString().Trim();
                        if (val.Length == 0)
                        {
                            str += "NULL,";
                        }
                        else if (test[j] == "string")
                        {
                            str += "'" + val.Replace("'", "''") + "',";
                        }
                        else if (test[j] == "number")
                        {
                            if (Regex.IsMatch(val, @"^-?\d+(\.\d+)?$"))
                            {
                                str += "'" + val + "',";
                            }
                            else
                            {
                                throw new Exception("DataTable第" + (i + 1) + "行" + "第" + (j + 1) + "列的值：“" + val + "”不是有效数字，Excel是第" + (i + 2) + "行");
                            }
                        }
                        else if (test[j] == "datetime")
                        {
                            DateTime d;
                            if (DateTime.TryParse(val, out d))
                            {
                                str += "'" + val + "',";
                            }
                            else
                            {
                                throw new Exception("DataTable第" + (i + 1) + "行" + "第" + (j + 1) + "列的值：“" + val + "”不是有效日期或时间，Excel是第" + (i + 2) + "行");
                            }
                        }
                        else
                        {
                            throw new Exception("test参数的值" + test[j] + "不是在指定的值之中");
                        }
                    }
                    sql.Append(String.Format(temSql, str.Remove(str.Length - 1, 1)));
                }
                return DbHelper.ExecuteTran(sql.ToString()).ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        #endregion
        /*
        #region C#操作 Excel（针对已有的Excel模板做操作）,如果只是简单的Excel数据导入导出，请调用上面的方法
        
        // 使用说明：
        // 1.先调用Open()
        // 2.其他操作
        // 3.调用 SaveAs()保存文件，并释放资源
         
        private Application app;
        private _Workbook wbk;
        private _Worksheet sheet; //当前操作的sheet
        private object missing = System.Reflection.Missing.Value;
        public int RowCount { get { return sheet.UsedRange.Rows.Count; } }  //总行数
        public int ColCount { get { return sheet.UsedRange.Columns.Count; } }  //总列数
        /// <summary>
        /// 打开Excel
        /// </summary>
        /// <param name="path">Excel的路径</param>
        public void Open(string path, string sheetName)
        {
            app = new Application();
            Workbooks wbks = app.Workbooks;
            app.DisplayAlerts = false;  //设置是否显示警告窗体
            app.Visible = false;    //设置是否显示Excel
            app.ScreenUpdating = false; //禁止刷新屏幕
            wbk = wbks.Add(path);  //打开已有的Excel
            Sheets sheets = wbk.Sheets;  //获取sheet集合
            sheet = (_Worksheet)sheets.get_Item(sheetName); //当前操作的sheet
        }

        /// <summary>
        /// 插入行，index:索引，从1开始
        /// </summary>
        public void AddRow(int index)
        {
            ((Range)sheet.Rows[index, missing]).Insert(missing, XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
        }
        /// <summary>
        /// 插入列，index:索引，从1开始
        /// </summary>
        public void AddCol(int index)
        {
            Range xlsColumns = (Range)sheet.Columns[index, System.Type.Missing];
            xlsColumns.Insert(XlInsertShiftDirection.xlShiftToRight, Type.Missing);
        }
        public void RowHeight(float height, int row1, int row2)
        {
            for (int i = row1; i <= row2; i++)
            {
                Range range = (Range)sheet.Rows[i, missing];
                range.RowHeight = height;
            }

        }
        /// <summary>
        /// 设置单元格的值
        /// </summary>
        public void SetCell(int row, int cell, string val)
        {
            Range r = (Range)sheet.Cells[row, cell];
            r.Value = val;
            r.Borders.LineStyle = 1;
        }
        /// <summary>
        /// 获取单元格的值
        /// </summary>
        public string GetCell(int row, int cell)
        {
            return Convert.ToString(sheet.Cells[row, cell].Value);
        }
        //该方法注释掉 因为要引用 COM：Microsoft Office 14.0 Object Library
        /// <summary>
        /// 向Excel中插入图片  RangeName：单元格名称，例如：B4 ,其他参数分别为图片的路径,图片宽度和高度
        /// </summary>
        //public void InsertImg(string RangeName, string imgPath, float width, float height)
        //{
        //    Range range = sheet.get_Range(RangeName, missing);
        //    float left = Convert.ToSingle(range.Left);
        //    float top = Convert.ToSingle(range.Top);
        //    sheet.Shapes.AddPicture(imgPath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, left, top, width, height);
        //}
        /// <summary>
        /// 保存文件并释放资源
        /// </summary>
        /// <param name="path"></param>
        public void SaveAs(string path)
        {
            //屏蔽掉系统跳出的Alert
            app.AlertBeforeOverwriting = false;
            //保存到指定目录
            wbk.SaveAs(path, missing, missing, missing, missing, missing, XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
            //释放资源
            wbk.Close();
            app.Quit();
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("excel");
            foreach (System.Diagnostics.Process pro in procs) { pro.Kill(); }  //杀死进程
        }
        #endregion
        */
    }
}
