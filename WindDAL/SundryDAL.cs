using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using WindModel;
using Common;

namespace WindDAL
{
    /// <summary>
    /// 杂项表
    /// </summary>
    public class SundryDAL : DbHelper
    {
        #region 增删改
        /// <summary>
        /// 新增，出现异常返回-1
        /// </summary>
        public int Insert(Sundry model)
        {
            string sql = "Insert Into Sundry(TID,str1,str2,str3,str4,str5) Values(@TID,@str1,@str2,@str3,@str4,@str5)";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@str1",model.str1),
                                        Parameter("@str2",model.str2),
                                        Parameter("@str3",model.str3),
                                        Parameter("@str4",model.str4),
                                        Parameter("@str5",model.str5),
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 修改，出现异常返回-1
        /// </summary>
        public int Update(Sundry model)
        {
            string sql = "Update Sundry Set TID=@TID,str1=@str1,str2=@str2,str3=@str3,str4=@str4,str5=@str5 Where ID=@ID";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@str1",model.str1),
                                        Parameter("@str2",model.str2),
                                        Parameter("@str3",model.str3),
                                        Parameter("@str4",model.str4),
                                        Parameter("@str5",model.str5),
                                        Parameter("@ID",model.ID)
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除，出现异常返回-1
        /// </summary>
        public int Delete(int ID)
        {
            string sql = "Delete From Sundry Where ID=" + ID;
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
            string sql = "Delete From Sundry Where ID In(" + str.Remove(str.Length - 1) + ")";
            return ExecuteNonQuery(sql);
        }
        #endregion
        #region 查询
        /// <summary>
        /// 把 DataReader对象转换成 List集合
        /// </summary>
        protected List<Sundry> ReaderToList(SqlDataReader reader)
        {
            try
            {
                List<Sundry> list = new List<Sundry>();
                while (reader.Read())
                {
                    Sundry model = new Sundry();
					model.ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID"]);
					model.TID = reader["TID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TID"]);
					model.str1 = Convert.ToString(reader["str1"]);
					model.str2 = Convert.ToString(reader["str2"]);
					model.str3 = Convert.ToString(reader["str3"]);
					model.str4 = Convert.ToString(reader["str4"]);
					model.str5 = Convert.ToString(reader["str5"]);
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
        public List<Sundry> Select(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select * From Sundry ";
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
        public List<Sundry> Select(int top, string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Top " + top + " * From Sundry ";
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
        public Sundry SelModel(int ID)
        {
            string sql = "Select * From Sundry Where ID=" + ID;
            SqlDataReader reader = ExecuteReader(sql);
            List<Sundry> list = ReaderToList(reader);
            return list != null && list.Count > 0 ? list[0] : null;
        }
        /// <summary>
        /// 分页查询，出现异常返回null
        /// </summary>
        /// <param name="index">分页的页码</param>
        /// <param name="size">每页显示的数据量</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序方式 为null 为默认排序</param>
        public List<Sundry> SelPage(int index, int size, string where = null, string order = "ID Asc", SqlParameter[] param = null)
        {
            where = where != null ? "Where " + where : null;
            StringBuilder sql = new StringBuilder();
            sql.Append("With frm As(Select *,Row_Number() Over(Order By " + order + ") As Sys_RowNum From Sundry " + where + ") ");  //多变联查只需改这句代码就行了
            sql.Append("Select * From frm Where Sys_RowNum Between " + ((index - 1) * size + 1) + " And " + (index * size));  //这句代码不用变
            SqlDataReader reader = ExecuteReader(sql.ToString(), param);
            return ReaderToList(reader);
        }

        /// <summary>
        /// 获取总数据量，出现异常返回-1
        /// </summary>
        public int SelCount(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Count(*) From Sundry ";
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

