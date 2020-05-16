using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 发送邮箱类
    /// </summary>
    public class Email
    {
        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="title">标题</param>
        /// <param name="email">Email</param>
        public static void SendEmail(string Content, string Title, string email)
        {
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.163.com", 25);
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("zhang_sheJi@163.com", "19ZXS9010zc26");
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.Priority = System.Net.Mail.MailPriority.Normal;
                message.From = new System.Net.Mail.MailAddress("zhang_sheJi@163.com", Title, System.Text.Encoding.GetEncoding("gb2312"));
                message.To.Add(email);
                message.Subject = Title;
                message.Body = Content;
                message.SubjectEncoding = System.Text.Encoding.GetEncoding("gb2312");
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.GetEncoding("gb2312");
                client.Send(message);
            }
            catch
            {

            }
        }
    }
}
