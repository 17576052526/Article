using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Common
{
    public class DbHelper
    {
        private static readonly string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnString"].ToString();    //获取链接字符串，需要添加引用
        private SqlConnection _conn;
        /// <summary>
        /// 数据库连接对象,不需要 Open(),  但需要 Close();
        /// </summary>
        protected SqlConnection Connection
        {
            get
            {
                if (_conn == null)
                {
                    _conn = new SqlConnection(ConnString);
                    _conn.Open();
                }
                else if (_conn.State == ConnectionState.Closed)
                {
                    _conn.Open();
                }
                else if (_conn.State == ConnectionState.Broken)
                {
                    _conn.Close();
                    _conn.Open();
                }
                return _conn;
            }
        }
        /// <summary>
        /// 执行sql语句返回 DataReader对象，出现异常返回null
        /// </summary>
        protected SqlDataReader ExecuteReader(string sql, SqlParameter[] param = null, CommandType type = CommandType.Text)
        {
            try
            {
                SqlCommand comm = new SqlCommand(sql, Connection);
                comm.CommandType = type;
                if (param != null) { comm.Parameters.AddRange(param); }
                return comm.ExecuteReader();
            }
            catch
            {
                Connection.Close();
                return null;
            }
        }
        /// <summary>
        /// 执行sql语句，返回受影响的行，出现异常返回-1
        /// </summary>
        public static int ExecuteNonQuery(string sql, SqlParameter[] param = null, CommandType type = CommandType.Text)
        {
            SqlConnection conn = new SqlConnection(ConnString);
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandType = type;
                if (param != null) { comm.Parameters.AddRange(param); }
                return comm.ExecuteNonQuery();
            }
            catch { return -1; }
            finally { conn.Close(); }
        }

        /// <summary>
        /// 执行Select查询语句，返回第一行第一列，出现异常返回null
        /// </summary>
        public static object ExecuteScalar(string sql, SqlParameter[] param = null, CommandType type = CommandType.Text)
        {
            SqlConnection conn = new SqlConnection(ConnString);
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandType = type;
                if (param != null) { comm.Parameters.AddRange(param); }
                return comm.ExecuteScalar();
            }
            catch { return null; }
            finally { conn.Close(); }
        }
        /// <summary>
        /// 执行事务，成功返回true,失败返回false
        /// </summary>
        public static bool ExecuteTran(string sql, SqlParameter[] param = null)
        {
            string str = "begin try begin tran \n" + sql + "\ncommit tran select 1 end try begin catch rollback tran select 0 end catch";
            return Convert.ToBoolean(ExecuteScalar(str, param));
        }
        /// <summary>
        /// 执行Select查询语句，并以DataTable返回，出现异常返回null
        /// </summary>
        public static DataTable SelTab(string sql, SqlParameter[] param = null, CommandType type = CommandType.Text)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(ConnString);
            try
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.SelectCommand.CommandType = type;
                if (param != null) { adapter.SelectCommand.Parameters.AddRange(param); }
                adapter.Fill(dt);
                return dt;
            }
            catch { return null; }
            finally { conn.Close(); }
        }
        /// <summary>
        /// 执行Select查询语句，返回List集合
        /// </summary>
        public static List<T> SelList<T>(string sql, SqlParameter[] param = null, CommandType type = CommandType.Text) where T : new()
        {
            SqlConnection conn = new SqlConnection(ConnString);
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandType = type;
                if (param != null) { comm.Parameters.AddRange(param); }
                reader = comm.ExecuteReader();
                List<T> list = new List<T>();
                Type objType = typeof(T);    //获取元素的类型
                while (reader.Read())
                {
                    T obj = new T();     //创建集合项元素的对象
                    //遍历该行的每一列
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);           //获取该列的名称
                        PropertyInfo p = objType.GetProperty(fieldName);//获取该列的属性信息
                        //若列名与类属性名不一致，则继续，以防止 select *  后期加个字段就报错
                        if (p == null) { continue; }
                        if (!reader.IsDBNull(i))
                        {
                            Type propType = p.PropertyType;                 //获取类中该列的类型，用于处理类型不一致的情况
                            Type fieldType = reader.GetFieldType(i);        //获取数据库中该列的类型，用于处理类型不一致的情况
                            object objValue = propType != fieldType && propType.Name != "Nullable`1" ? Convert.ChangeType(reader.GetValue(i), propType) : reader.GetValue(i);  //防止赋值时出错
                            p.SetValue(obj, objValue, null);              //为集合元素对象的当前列赋值
                        }
                    }
                    list.Add(obj);  //将集合项元素添加到集合中
                }
                return list;
            }
            catch { return null; }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
        }
        #region 获取Parameter对象  C#保存时间类型必须用 DataTime?
        public static SqlParameter Parameter(string name, object value)
        {
            SqlParameter param = new SqlParameter() { ParameterName = name };
            param.Value = value == null ? DBNull.Value : value;
            return param;
        }
        public static SqlParameter Parameter(string name, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter(name, type);
            param.Value = value == null ? DBNull.Value : value;
            return param;
        }
        public static SqlParameter Parameter(string name, object value, SqlDbType type, int size)
        {
            SqlParameter param = new SqlParameter(name, type, size);
            param.Value = value == null ? DBNull.Value : value;
            return param;
        }
        #endregion
    }
}