using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// DES 加密和解密
    /// </summary>
    public class Encrypt
    {
        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        public string Encry(string value) { return Encry(value, "WindKeys"); }
        /// <summary>
        /// 加密，key必须是8个字符
        /// </summary>
        public string Encry(string value, string key)
        {
            byte[] val = Encoding.UTF8.GetBytes(value);
            byte[] KeyByte = Encoding.UTF8.GetBytes(key);
            byte[] IVByte = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(KeyByte, IVByte), CryptoStreamMode.Write);
            cs.Write(val, 0, val.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion
        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string value) { return Decrypt(value, "WindKeys"); }
        /// <summary>
        /// 解密，key必须是8个字符，且必须与加密时key一致
        /// </summary>
        public string Decrypt(string value, string key)
        {
            byte[] val = Convert.FromBase64String(value);
            byte[] KeyByte = Encoding.UTF8.GetBytes(key);
            byte[] IVByte = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(KeyByte, IVByte), CryptoStreamMode.Write);
            cs.Write(val, 0, val.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion
    }
}
