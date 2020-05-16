using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class LayoutController : Controller
    {
        //模板页视图的初始化代码放在构造函数，其他控制器继承该控制器
        public LayoutController()
        {

        }

    }
}
