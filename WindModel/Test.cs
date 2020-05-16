using System;

namespace WindModel
{
    /// <summary>
    /// 测试表
    /// </summary>
    public class Test
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 所属类型
        /// </summary>
        public int TID { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? Dates { set; get; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Img { set; get; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Num { set; get; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Che { set; get; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 文件
        /// </summary>
        public string Files { set; get; }
    }
}
