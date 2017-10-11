using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace 录波采集测试工具
{
    public class Assistant_Form
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="cfg">需要获取配置</param>
        /// <returns>配置参数</returns>
        public static string getCFG(string cfg)
        {
            string path = Environment.CurrentDirectory;

            string cfgstr = "";
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(path + "\\leidian.xml");
                XmlNode Root = doc.SelectSingleNode("leidian");
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

    }
}
