using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using WindModel;
using Common;

namespace WindDAL
{
    /// <summary>
    /// 测试表
    /// </summary>
    public class TestDAL : DbHelper
    {
        #region 增删改
        /// <summary>
        /// 新增，出现异常返回-1
        /// </summary>
        public int Insert(Test model)
        {
            string sql = "Insert Into Test(TID,Title,Dates,Img,Num,Price,Che,Content,Files) Values(@TID,@Title,@Dates,@Img,@Num,@Price,@Che,@Content,@Files)";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@Title",model.Title),
                                        Parameter("@Dates",model.Dates),
                                        Parameter("@Img",model.Img),
                                        Parameter("@Num",model.Num),
                                        Parameter("@Price",model.Price),
                                        Parameter("@Che",model.Che),
                                        Parameter("@Content",model.Content),
                                        Parameter("@Files",model.Files)
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 修改，出现异常返回-1
        /// </summary>
        public int Update(Test model)
        {
            string sql = "Update Test Set TID=@TID,Title=@Title,Dates=@Dates,Img=@Img,Num=@Num,Price=@Price,Che=@Che,Content=@Content,Files=@Files Where ID=@ID";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@Title",model.Title),
                                        Parameter("@Dates",model.Dates),
                                        Parameter("@Img",model.Img),
                                        Parameter("@Num",model.Num),
                                        Parameter("@Price",model.Price),
                                        Parameter("@Che",model.Che),
                                        Parameter("@Content",model.Content),
                                        Parameter("@Files",model.Files),
                                        Parameter("@ID",model.ID)
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除，出现异常返回-1
        /// </summary>
        public int Delete(int ID)
        {
            string sql = "Delete From Test Where ID=" + ID;
            return ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 批量删除，出现异常返回-1
        /// </summary>
        /// <param name="ID">ID的集合</param>
        public int Delete(List<int> ID)
        {
            string str = "";
            foreach (int id in ID) { str += id + ","; }
            string sql = "Delete From Test Where ID In(" + str.Remove(str.Length - 1) + ")";
            return ExecuteNonQuery(sql);
        }
        #endregion
        #region 查询
        /// <summary>
        /// 把 DataReader对象转换成 List集合
        /// </summary>
        protected List<Test> ReaderToList(SqlDataReader reader)
        {
            try
            {
                List<Test> list = new List<Test>();
                while (reader.Read())
                {
                    Test model = new Test();
                    model.ID = Convert.ToInt32(reader["ID"]);
                    model.TID = reader["TID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TID"]);
                    model.Title = Convert.ToString(reader["Title"]);
                    if (reader["Dates"] != DBNull.Value) { model.Dates = Convert.ToDateTime(reader["Dates"]); }
                    model.Img = Convert.ToString(reader["Img"]);
                    model.Num = reader["Num"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Num"]);
                    model.Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]);
                    model.Che = reader["Che"] == DBNull.Value ? false : Convert.ToBoolean(reader["Che"]);
                    model.Content = Convert.ToString(reader["Content"]);
                    model.Files = Convert.ToString(reader["Files"]);
                    list.Add(model);
                }
                return list;
            }
            catch { return null; }
            finally
            {
                if (reader != null) { reader.Close(); }
                Connection.Close();
            }
            
        }
        /// <summary>
        /// 查询，出现异常返回null
        /// </summary>
        public List<Test> Select(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select * From Test ";
            if (where != null)
            {
                sql += "Where " + where;
            }
            SqlDataReader reader = ExecuteReader(sql, param);
            return ReaderToList(reader);
        }
        /// <summary>
        /// 查询前n条，出现异常返回null
        /// </summary>
        public List<Test> Select(int top, string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Top " + top + " * From Test ";
            if (where != null)
            {
                sql += "Where " + where;
            }
            SqlDataReader reader = ExecuteReader(sql, param);
            return ReaderToList(reader);
        }
        /// <summary>
        /// 查询实体，出现异常返回null
        /// </summary>
        public Test SelModel(int ID)
        {
            string sql = "Select * From Test Where ID=" + ID;
            SqlDataReader reader = ExecuteReader(sql);
            List<Test> list = ReaderToList(reader);
            return list != null && list.Count > 0 ? list[0] : null;
        }
        /// <summary>
        /// 分页查询，出现异常返回null
        /// </summary>
        /// <param name="index">分页的页码</param>
        /// <param name="size">每页显示的数据量</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序方式 为null 为默认排序</param>
        public List<Test> SelPage(int index, int size, string where = null, string order = "ID Asc", SqlParameter[] param = null)
        {
            where = where != null ? "Where " + where : null;
            StringBuilder sql = new StringBuilder();
            sql.Append("With frm As(Select *,Row_Number() Over(Order By " + order + ") As Sys_RowNum From Test " + where + ") ");  //多变联查只需改这句代码就行了
            sql.Append("Select * From frm Where Sys_RowNum Between " + ((index - 1) * size + 1) + " And " + (index * size));  //这句代码不用变
            SqlDataReader reader = ExecuteReader(sql.ToString(), param);
            return ReaderToList(reader);
        }

        /// <summary>
        /// 获取总数据量，出现异常返回-1
        /// </summary>
        public int SelCount(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Count(*) From Test ";
            if (where != null)
            {
                sql += "Where " + where;
            }
            object obj = ExecuteScalar(sql, param);
            return obj != null ? Convert.ToInt32(obj) : -1;
        }
        #endregion

    }
}
