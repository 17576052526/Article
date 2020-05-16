using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using WindModel;
using Common;

namespace WindDAL
{
    /// <summary>
    /// 文章表
    /// </summary>
    public class ArticleDAL : DbHelper
    {
        #region 增删改
        /// <summary>
        /// 新增，出现异常返回-1
        /// </summary>
        public int Insert(Article model)
        {
            string sql = "Insert Into Article(TID,Title,Content,ImgUrl,downUrl,ClickCount,CreateDate) Values(@TID,@Title,@Content,@ImgUrl,@downUrl,@ClickCount,@CreateDate)";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@Title",model.Title),
                                        Parameter("@Content",model.Content),
                                        Parameter("@ImgUrl",model.ImgUrl),
                                        Parameter("@downUrl",model.downUrl),
                                        Parameter("@ClickCount",model.ClickCount),
                                        Parameter("@CreateDate",model.CreateDate),
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 修改，出现异常返回-1
        /// </summary>
        public int Update(Article model)
        {
            string sql = "Update Article Set TID=@TID,Title=@Title,Content=@Content,ImgUrl=@ImgUrl,downUrl=@downUrl,ClickCount=@ClickCount,CreateDate=@CreateDate Where ID=@ID";
            SqlParameter[] param ={
                                        Parameter("@TID",model.TID),
                                        Parameter("@Title",model.Title),
                                        Parameter("@Content",model.Content),
                                        Parameter("@ImgUrl",model.ImgUrl),
                                        Parameter("@downUrl",model.downUrl),
                                        Parameter("@ClickCount",model.ClickCount),
                                        Parameter("@CreateDate",model.CreateDate),
                                        Parameter("@ID",model.ID)
                                   };
            return ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除，出现异常返回-1
        /// </summary>
        public int Delete(int ID)
        {
            string sql = "Delete From Article Where ID=" + ID;
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
            string sql = "Delete From Article Where ID In(" + str.Remove(str.Length - 1) + ")";
            return ExecuteNonQuery(sql);
        }
        #endregion
        #region 查询
        /// <summary>
        /// 把 DataReader对象转换成 List集合
        /// </summary>
        protected List<Article> ReaderToList(SqlDataReader reader)
        {
            try
            {
                List<Article> list = new List<Article>();
                while (reader.Read())
                {
                    Article model = new Article();
					model.ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID"]);
					model.TID = Convert.ToString(reader["TID"]);
					model.Title = Convert.ToString(reader["Title"]);
					model.Content = Convert.ToString(reader["Content"]);
					model.ImgUrl = Convert.ToString(reader["ImgUrl"]);
					model.downUrl = Convert.ToString(reader["downUrl"]);
					model.ClickCount = reader["ClickCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClickCount"]);
					if (reader["CreateDate"] != DBNull.Value) { model.CreateDate = Convert.ToDateTime(reader["CreateDate"]); }
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
        public List<Article> Select(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select * From Article ";
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
        public List<Article> Select(int top, string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Top " + top + " * From Article ";
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
        public Article SelModel(int ID)
        {
            string sql = "Select * From Article Where ID=" + ID;
            SqlDataReader reader = ExecuteReader(sql);
            List<Article> list = ReaderToList(reader);
            return list != null && list.Count > 0 ? list[0] : null;
        }
        /// <summary>
        /// 分页查询，出现异常返回null
        /// </summary>
        /// <param name="index">分页的页码</param>
        /// <param name="size">每页显示的数据量</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序方式 为null 为默认排序</param>
        public List<Article> SelPage(int index, int size, string where = null, string order = "ID Asc", SqlParameter[] param = null)
        {
            where = where != null ? "Where " + where : null;
            StringBuilder sql = new StringBuilder();
            sql.Append("With frm As(Select *,Row_Number() Over(Order By " + order + ") As Sys_RowNum From Article " + where + ") ");  //多变联查只需改这句代码就行了
            sql.Append("Select * From frm Where Sys_RowNum Between " + ((index - 1) * size + 1) + " And " + (index * size));  //这句代码不用变
            SqlDataReader reader = ExecuteReader(sql.ToString(), param);
            return ReaderToList(reader);
        }

        /// <summary>
        /// 获取总数据量，出现异常返回-1
        /// </summary>
        public int SelCount(string where = null, SqlParameter[] param = null)
        {
            string sql = "Select Count(*) From Article ";
            if (where != null)
            {
                sql += "Where " + where;
            }
            object obj = ExecuteScalar(sql, param);
            return obj != null ? Convert.ToInt32(obj) : -1;
        }
        #endregion
        /*非代码生成器生成的代码*/
        protected List<Article> ReaderToListID(SqlDataReader reader)
        {
            try
            {
                List<Article> list = new List<Article>();
                while (reader.Read())
                {
                    Article model = new Article();
                    model.ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID"]);
                    model.Title = Convert.ToString(reader["Title"]);
                    model.ImgUrl = Convert.ToString(reader["ImgUrl"]);
                    if (reader["CreateDate"] != DBNull.Value) { model.CreateDate = Convert.ToDateTime(reader["CreateDate"]); }
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
        
        public List<Article> SelPageID(int index, int size, string where = null, string order = "ID Asc", SqlParameter[] param = null)
        {
            where = where != null ? "Where " + where : null;
            StringBuilder sql = new StringBuilder();
            sql.Append("With frm As(Select ID,Title,ImgUrl,CreateDate,Row_Number() Over(Order By " + order + ") As Sys_RowNum From Article " + where + ") ");  //多变联查只需改这句代码就行了
            sql.Append("Select * From frm Where Sys_RowNum Between " + ((index - 1) * size + 1) + " And " + (index * size));  //这句代码不用变
            SqlDataReader reader = ExecuteReader(sql.ToString(), param);
            return ReaderToListID(reader);
        }
    }
}

