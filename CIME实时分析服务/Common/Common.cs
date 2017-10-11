using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Hisingpower
{
    public class Common
    {

        public static string Now()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string Timespan()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    
      

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="cfg">需要获取配置</param>
        /// <returns>配置参数</returns>
        public static string getCFG(string cfg)
        {
            string cfgstr = "";
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(@"C:\Program Files\Haixing\SDFLdatas\SDFLcfg.xml");
                XmlNode Root = doc.SelectSingleNode("AppSettings");
                foreach (XmlNode child in Root)
                {
                    if (child.Name == cfg)
                    {
                        if (child.InnerText != "")
                        {
                            cfgstr = child.InnerText;
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


        /// <summary>
        /// 获取本机ipv4
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logName">日志文件名</param>
        /// <param name="log">日志内容</param>
        /// <param name="logTime">日志时间</param>
        public static void WriteLog(string logName, string log, string logTime)
        {
            try
            {
                string des = Environment.CurrentDirectory +"\\Log";
                if (!Directory.Exists(des))
                {
                    Directory.CreateDirectory(des);
                }
                string filename = des + "\\" + logName + " " + logTime.Split(' ')[0] + ".txt";
                FileStream myFileStream = new FileStream(filename, FileMode.Append);
                StreamWriter sw = new StreamWriter(myFileStream, System.Text.Encoding.Default);
                sw.WriteLine(log + "  " + logTime);
                myFileStream.Flush();
                sw.Close();
                myFileStream.Close();

            }
            catch (System.Exception ex)
            {

            }
        }

        public static string hex2str(string hex)
        {

            StringBuilder sb = new StringBuilder(hex);
            sb.Replace("0x", "");
            return sb.ToString();

        }

        #region 该方法用于生成指定位数的随机字符串
        /// <summary>
        /// 该方法用于生成指定位数的随机字符串
        /// </summary>
        /// <param name="VcodeNum">参数是随机数的位数</param>
        /// <returns>返回一个随机数字符串</returns>
        public static string RndNumStr(int VcodeNum)
        {
            string[] source = { "0", "1", "1", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            string checkCode = String.Empty;
            Random random = new Random();
            for (int i = 0; i < VcodeNum; i++)
            {
                checkCode += source[random.Next(0, source.Length)];

            }
            return checkCode;
        }
        #endregion

      

    }
}
