using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace naviLink
{
    public class AssGetCFG
    {

        //public static string AnsiToUnicode(string ansiText)
        //{
        //    byte[] gb = System.Text.Encoding.GetEncoding("GB2312").GetBytes(ansiText);
        //    string un = System.Text.Encoding.GetEncoding("Unicode").GetString(gb);
        //    return un;
        //}

        /// <summary>
        /// <summary>
        /// 字符串转Unicode
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string String2Unicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }


        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="cfg">需要获取配置</param>
        /// <returns>配置参数</returns>
        public static string getCFG(string cfg)
        {
            //  string path = Environment.CurrentDirectory;
            // cfg = String2Unicode(cfg);
            //   cfg  = Encoding.UTF8.GetString(Encoding.Default.GetBytes(cfg));
            //cfg = AnsiToUnicode(cfg);
            //byte [] gb2312=Encoding.Convert()
            //  cfg = cfg.Replace("UTF-8", "GBK").Trim();
            //cfg.Encoding.UTF8
            cfg = cfg.Replace("&", "");
            string cfgstr = "";
            try
            {
                //string path = @"C:\Users\Administrator.WIN7-1607281528\Desktop\test.xml";
                XmlDocument doc = new XmlDocument();
                
                doc.LoadXml(cfg);
                //doc.Load(path);
                XmlNode root = doc.DocumentElement;

                XmlNodeList node = doc.GetElementsByTagName("url");

                for (int i = 0; i < node.Count; i++)
                {
                    foreach (XmlNode nodechild in node[i].ChildNodes)
                    {
                        if (nodechild.Name == "url_short")
                        {
                            cfgstr = nodechild.InnerText;

                            //ldTime=token.
                        }
                    }
                }
              
            }
            catch (System.Exception ex)
            {
                cfgstr = "";
            }
            return cfgstr;

        }

    }
}
