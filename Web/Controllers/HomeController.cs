using Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using WindDAL;
using WindModel;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [OutputCache(Duration = 3600, VaryByParam = "dwbc")]
        public ActionResult Index(string id)
        {
            if (id != null) { return Redirect("/Sys/Error.html?ID=404"); } //防止根据不同参数缓存页面

            IndexModel model = new IndexModel();
            model.Banner = new SundryDAL().Select("TID=1");
            model.Article = new ArticleDAL().Select(30, "1=1 order by ID DESC");
            //标题关键词描述
            ViewBag.Title = "新闻中心_易风网";
            ViewBag.Key = "新闻,资讯,易风";
            ViewBag.Desc = "易风网是为用户24小时提供全面及时的新闻、问答、社会、历史、奇闻趣事、科技、文化、教育、健康、以及其他各种知识分享的门户网站。";
            return View(model);
        }
        public ActionResult List(string id, int page, string title = null)  //list这个类型是不存在的，是为了满足路由器而写的，查询全部和按标题查询就用list /list/1 或/list?title=
        {
            string where = null;
            SqlParameter[] param = null;
            string hrefParam = null;
            if (title != null)
            {
                where = "title like @title";
                param = new SqlParameter[] { new SqlParameter("@title", "%" + title + "%") };
                hrefParam = "?title=" + title;
            }
            else if (id != "list")
            {
                where = "tid=@tid";
                param = new SqlParameter[] { new SqlParameter("@tid", id) };
            }
            ViewBag.topPage = page == 1 ? "javascript:void(0)" : "/" + id + "/" + (page - 1) + hrefParam;
            ViewBag.bottomPage = "/" + id + "/" + (page + 1) + hrefParam;
            //标题关键词描述
            string typeName = Convert.ToString(DbHelper.ExecuteScalar("select Title from Types where TID=@TID", new SqlParameter[] { new SqlParameter("@TID", id) }));
            typeName = typeName.Length == 0 ? "全部" : typeName;
            ViewBag.Title = typeName + "_易风网";
            ViewBag.Key = typeName + ",易风";
            //ViewBag.Desc = "";

            List<Article> model = new ArticleDAL().SelPageID(page, 30, where, "ID DESC", param);
            return View(model);
        }
        public ActionResult Xq(int id)    //若要静态化,保存路径就是 /html/id前三位/id后几位.html
        {
            ArticleDAL dal = new ArticleDAL();
            Article model = dal.SelModel(id);
            return View(model);
        }
        [OutputCache(Duration = 3600, VaryByParam = "dwbc")]
        public ActionResult _PartialRight()
        {
            ArticleDAL artDal= new ArticleDAL();
            _PartialRightModel model = new _PartialRightModel();
            model.ermen = artDal.SelPageID(10, 5, "ImgUrl is not null", "ID DESC");
            model.jingxuan = artDal.SelPageID(10, 6, "ImgUrl is not null", "ID DESC");
            return PartialView(model);
        }
    }
}
