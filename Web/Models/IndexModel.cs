using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WindModel;

namespace Web.Models
{
    public class IndexModel
    {
        /// <summary>
        /// 首页Banner图
        /// </summary>
        public List<Sundry> Banner { set; get; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<Article> Article { set; get; }
    }
}