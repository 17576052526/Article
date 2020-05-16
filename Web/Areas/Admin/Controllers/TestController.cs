using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WindDAL;
using WindModel;
using WindPager;

namespace Web.Areas.Admin.Controllers
{
    public class TestController : LayoutController
    {
        TestDAL Dal = new TestDAL();

        #region List.cshtml的代码
        // 分页查询 id为页码,按条件查询也写在这里面
        public ActionResult List(string TID, string title, int id = 1)
        {
            string where = null;
            where = TID != null ? "TID=" + TID : null;
            if (title != null && title.Length > 0)
            {
                where += (where != null ? " and " : "") + "title like '%" + title + "%'";
            }

            ViewModel<Test> model = new ViewModel<Test>();
            model.PageSize = 15;
            model.PageIndex = id;
            model.List = Dal.SelPage(id, model.PageSize, where, "ID DESC");
            model.DataCount = Dal.SelCount(where);
            return View(model);
        }
        // 删除
        public ActionResult Delete(int id)
        {
            int count = Dal.Delete(id);
            if (count == -1)
            {
                return Content("出现错误，请重试");
            }
            return Redirect(@Url.Action("List") + Request.Url.Query);
        }
        //多选删除
        public ActionResult CheDel(string id)
        {
            List<int> list = new List<int>();
            foreach (string str in id.Split(','))
            {
                list.Add(Convert.ToInt32(str));
            }
            int count = Dal.Delete(list);
            if (count == -1)
            {
                return Content("出现错误，请重试");
            }
            return Redirect(@Url.Action("List") + Request.Url.Query);
        }
        #endregion

        #region Edit.cshtml的代码----------------------------------------------------------------
        public ActionResult Edit(int id = -1)
        {
            if (id != -1)
            {
                return View(Dal.SelModel(id));
            }
            else
            {
                return View();
            }
        }
        //新增或修改 id用来标示是新增还是修改
        [HttpPost]
        public ActionResult Edit(Test model, int id = -1)
        {
            int count = id == -1 ? Dal.Insert(model) : Dal.Update(model);
            if (count == -1)
            {
                return Content("操作失败");
            }
            else
            {
                return Redirect(@Url.Action("List") + Request.Url.Query);
            }
        }
        #endregion

    }
}
