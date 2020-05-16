using System;

namespace WindModel
{
    /// <summary>
    /// 文章表
    /// </summary>
    public class Article
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 所属类型
        /// </summary>
        public string TID { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImgUrl { set; get; }
        /// <summary>
        /// 原链接地址
        /// </summary>
        public string downUrl { set; get; }
        /// <summary>
        /// 点击次数
        /// </summary>
        public int ClickCount { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { set; get; }
    }
}
