using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class OtherController : Controller
    {
        //
        // GET: /Other/

        public ActionResult Index()
        {
            return View();
        }
        //意见反馈 发送邮件到qq邮箱
        [HttpPost]
        public ActionResult feedback(string phone,string content)
        {
            Email.SendEmail(content + "<br/>联系电话" + phone, "意见反馈_易风网", "727863984@qq.com");
            return Content("提交成功");
        }
    }
}
