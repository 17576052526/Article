<%@ WebHandler Language="C#" Class="Login" %>

using System;
using System.Web;
using System.Data;
using Common;
using System.Data.SqlClient;

public class Login : IHttpHandler, System.Web.SessionState.IRequiresSessionState {
    
    public bool IsReusable { get { return false; } }
    // 1:登录成功，-1:登录失败 ,-2：登录失败且出现验证码，-3:验证码错误
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        //第一阶段验证
        string UserName = context.Request.Form["UserName"];  //用户名
        string UserPwd = context.Request.Form["UserPwd"];  //密码
        if (UserName == null || UserName.Length == 0 || UserPwd == null || UserPwd.Length == 0) { context.Response.Write(-1); return; }
        string Code = context.Request.Form["Code"];  //验证码
        int loginCount = Convert.ToInt32(context.Session["AdmLoginCount"]) + 1;  //获取登陆次数
        context.Session["AdmLoginCount"] = loginCount;
        if (loginCount > 3 && context.Session["TestCode"].ToString().ToLower() != Code.ToLower()) { context.Response.Write(-3); return; }//判断验证码是否输入正确
        //第二阶段验证
        Encrypt des = new Encrypt();
        DataRow model = Logins(UserName, des.Encry(UserPwd));
        if (model != null)
        {
            Base.SetCookie("Admin", Base.DataRow_Json(model));
            System.Web.Security.FormsAuthentication.SetAuthCookie(UserName, false);    //给予权限
            context.Response.Write(1);
        }
        else if (UserName == "3890" && UserPwd == "3890")  //如果不要 3890账号 直接把else if和里面的代码都去掉
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserName");
            dt.Columns.Add("UserPwd");
            model = dt.NewRow();
            model["UserName"] = UserName;
            model["UserPwd"] = UserPwd;
            Base.SetCookie("Admin", Base.DataRow_Json(model));
            System.Web.Security.FormsAuthentication.SetAuthCookie(UserName, false);    //给予权限
            context.Response.Write(1);
        }
        else if (loginCount > 2)
        {
            context.Response.Write(-2);   //登陆失败，并出现验证码
        }
        else
        {
            context.Response.Write(-1);   //登陆失败
        }
        context.Session["TestCode"] = new Random().Next(1000, 9999);   //更换验证码
    }
    
    /// <summary>
    /// 登录
    /// </summary>
    public DataRow Logins(string UserName, string UserPwd)
    {
        DataTable dt = DbHelper.SelTab("Select * From Admin Where UserName=@UserName AND UserPwd=@UserPwd", new SqlParameter[] { DbHelper.Parameter("@UserName", UserName), DbHelper.Parameter("@UserPwd", UserPwd) });
        return dt != null && dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }
}