using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //详情页路由配置
            routes.MapRoute(
                name: "Xq",
                url: "{id}",
                defaults: new { controller = "Home", action = "Xq" },
                constraints: new { id = @"\d+" }
            );

            //列表页路由配置
            routes.MapRoute(
                name: "List",
                url: "{id}/{page}",
                defaults: new { controller = "Home", action = "List", page = 1 },
                constraints: new { id = @"\w+", page = @"\d*" }
            );

            //其他页面路由配置
            routes.MapRoute(
                name: "Other",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}