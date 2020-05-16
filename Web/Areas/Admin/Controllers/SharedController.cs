using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    public class SharedController : LayoutController
    {
        //
        // GET: /Admin/Shared/

        public ActionResult Index()
        {
            return View();
        }

        //文件上传
        [HttpPost]
        public ActionResult UpFile(string HandID, string path)
        {
            string str = null;
            if (HandID == "Upload")  //上传文件
            {
                str = Base.Upload("fileUpFile");  //获取保存在服务器的文件路径
            }
            else if (HandID == "DelFile")  //删除文件
            {
                path = Server.MapPath(path);
                if (System.IO.File.Exists(path))
                {
                    try { System.IO.File.Delete(path); str = "1"; }  //删除成功
                    catch { str = "-1"; }  //删除失败
                }
                else { str = "2"; }  //文件不存在
            }
            return Content(str);
        }

    }
}
