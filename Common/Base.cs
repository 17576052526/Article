using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

//该类没有命名空间，访问时不用引用命名空间
/// <summary>
/// 常用的静态方法类
/// </summary>
public class Base
{
    #region 字符串操作
    /// <summary>
    /// 去掉html标签，返回去掉之后的字符串
    /// </summary>
    public static string NoHTML(object objStr)
    {
        string htmlString = objStr.ToString();
        //删除脚本   
        htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
        //删除HTML   
        htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"-->", "", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"<!--.*", "", RegexOptions.IgnoreCase);

        htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
        htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);

        htmlString.Replace("<", "");
        htmlString.Replace(">", "");
        htmlString.Replace("\r\n", "");
        htmlString = HttpContext.Current.Server.HtmlEncode(htmlString).Trim();

        return htmlString;
    }
    /// <summary>
    /// 截取字符串，一个汉字两个字节长度
    /// </summary>
    /// <param name="Str">要被截取的字符串</param>
    /// <param name="length">截取的长度，一个汉字两个长度</param>
    /// <param name="repl">剩余部分</param>
    public static string SubStr(string str, int length, string repl = "...")
    {
        byte[] by = System.Text.Encoding.GetEncoding("GB2312").GetBytes(str);    //一个中文两个字节
        if (by.Length > length)
        {
            str = System.Text.Encoding.GetEncoding("GB2312").GetString(by, 0, length - 2);
            int index = str.LastIndexOf('?');    //如果最后一个字为汉字，就有可能为‘？’所以要删除‘？’
            if (index > 0)
            {
                str = str.Remove(index, 1);
            }
            str += repl;
        }
        return str;
    }
    #endregion
    #region 日期格式化
    /// <summary>
    /// DateTime格式化，格式：yyyy-MM-dd
    /// </summary>
    public static string Date(object obj) { return String.Format("{0:yyyy-MM-dd}", obj); }

    /// <summary>
    /// DateTime格式化，格式：yyyy-MM-dd HH:mm
    /// </summary>
    public static string Time(object obj) { return String.Format("{0:yyyy-MM-dd HH:mm}", obj); }
    /// <summary>
    /// 传递年月日转换成星期几，例如：传2014-06-06 返回星期三
    /// </summary>
    public static string Week(object obj) { return String.Format("{0:dddd}", obj); }
    #endregion
    #region Json与字符串的互换
    /// <summary>
    /// DataTable 转json字符串
    /// </summary>
    public static string DataTable_Json(DataTable dt)
    {
        StringBuilder JsonStr = new StringBuilder();     //  [{"ID":1,"Title":"a{a"},{"ID":2,"Title":"b]b"},{"ID":3,"Title":"c\"c"}]
        JsonStr.Append("[");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            JsonStr.Append("{");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                JsonStr.Append("\"");
                JsonStr.Append(dt.Columns[j].ColumnName);
                JsonStr.Append("\":\"");
                JsonStr.Append(dt.Rows[i][j].ToString());
                JsonStr.Append("\",");
            }
            JsonStr.Remove(JsonStr.Length - 1, 1);
            JsonStr.Append("},");
        }
        JsonStr.Remove(JsonStr.Length - 1, 1);   //删除最后一个字符串
        JsonStr.Append("]");
        return JsonStr.ToString();
    }
    /// <summary>
    /// DataRow 转json字符串
    /// </summary>
    public static string DataRow_Json(DataRow dr)
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder("{");
        for (int i = 0; i < dr.Table.Columns.Count; i++)
        {
            str.Append("\"");
            str.Append(dr.Table.Columns[i].ColumnName);
            str.Append("\":");
            str.Append("\"");
            str.Append(dr[i]);
            str.Append("\",");
        }
        str.Remove(str.Length - 1, 1);
        str.Append("}");
        return str.ToString();
    }

    /// <summary>
    /// 反序列化（json字符串转对象）
    /// </summary>
    public static T Deserialize<T>(string jsonStr)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        return jss.Deserialize<T>(jsonStr);  //反序列化
    }
    /// <summary>
    /// 序列化（对象转json字符串）
    /// </summary>
    public static string Serialize(object obj)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        return jss.Serialize(obj);    //序列化
    }
    #endregion
    #region Cookies操作
    /// <summary>
    /// 设置Cookie  中文用HttpUtility.UrlEncode先编码，在调用方法，取数据时在解码
    /// </summary>
    /// <param name="day">设置过期天数，0表示关闭浏览器就过期,小于0表示删除cookie</param>
    public static void SetCookie(string key, string value, int day = 0)
    {
        HttpContext.Current.Response.Cookies[key].Value = value;
        if (day != 0) { HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(day); }
    }
    /// <summary>
    /// 获取Cookie，key所对应的值不存在返回null
    /// </summary>
    public static string GetCookie(string key)
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
        return cookie == null ? null : cookie.Value;
    }
    #endregion
    #region 文件上传和下载
    /// <summary>
    /// 文件上传，返回上传之后的文件路径
    /// </summary>
    /// <param name="name">浏览器提交过来的name值</param>
    /// <returns></returns>
    public static string Upload(string name)
    {
        string str = null;
        HttpContext context = HttpContext.Current;
        HttpPostedFile File = context.Request.Files[name];
        if (File != null && File.FileName.Length > 0)
        {
            //获取上传的目录
            string Direct = "/Upload/" + DateTime.Now.ToString("yyyyMM/dd/");    //相对路径
            string dir = context.Server.MapPath(Direct);   //获取绝对路径
            if (!System.IO.Directory.Exists(dir)) { System.IO.Directory.CreateDirectory(dir); } //不存在目录就创建目录
            //获取上传的文件名
            string suffix = File.FileName.Substring(File.FileName.LastIndexOf('.'));  //获取后缀名
            string NewName = DateTime.Now.ToString("HHmmssffff") + suffix;  //获取文件名
            string url = dir + NewName;    //上传的文件路径（绝对路径）
            File.SaveAs(url);    //上传文件
            str = Direct + NewName;
        }
        return str;
    }
    /// <summary>
    /// 文件下载，下载成功返回true，文件不存在返回false
    /// </summary>
    /// <param name="path">文件在服务器的所在路径，绝对路径</param>
    /// <param name="fileName">下载到浏览器的文件名称</param>
    /// <param name="encoding">编码,例如：UTF-8  GB2312</param>
    /// <returns></returns>
    public static bool Download(string path, string fileName, string encoding = "UTF-8")
    {
        if (System.IO.File.Exists(path))
        {
            HttpResponse resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding(encoding);
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            resp.WriteFile(path);
            resp.End();
            return true;
        }
        return false;
    }
    #endregion
}
