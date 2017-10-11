using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace elanguage.Efile
{
    class XmlDoc
    {
        internal static string xmlStr = string.Empty;
        private XmlNode root = new XmlNode();
        private string remark = string.Empty;
        public static List<XmlNode> AllNodes = new List<XmlNode>();
        internal List<XmlNode> nodes = new List<XmlNode>();
        public XmlNode Root
        {
            get { return root; }
            set { root = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public bool hasChinese(string str)
        {
            if (Regex.IsMatch(str, @"[\u4e00-\u9fa5]"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public XmlDoc(string path)
        {
            xmlStr = string.Empty;
            root = new XmlNode();
            remark = string.Empty;
            AllNodes = new List<XmlNode>();
            nodes = new List<XmlNode>();
            //StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8"));
            //编码使用Default，否则会生成乱码
            StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
            xmlStr = InitialXmlStr(sr.ReadToEnd());
            sr.Close();

            if (!hasChinese(xmlStr))
            {
                sr = new StreamReader(path, System.Text.Encoding.UTF8);
                xmlStr = InitialXmlStr(sr.ReadToEnd());
                sr.Close();
            }


            if (!VerifyXml())
            {
                throw new XmlInvalidException();
            }
            int begin = -1;
            if (xmlStr.Contains("<!"))
            {
                begin = xmlStr.IndexOf("!>");
            }
            else
            {
                begin = xmlStr.IndexOf("<");

            }

            int end = xmlStr.LastIndexOf(">");
            root.InitialXmlNode(begin, end);
            AllNodes.Add(root);

            if (!root.hasChildren())
            {
                AllNodes[0].setContent();
            }
            else
            {
                root.ExtractAllNode();
            }
        }

        public XmlDoc(string path, int coding)
        {
            //StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8"));
            //编码使用Default，否则会生成乱码
            StreamReader sr = null;
            xmlStr = string.Empty;
            root = new XmlNode();
            remark = string.Empty;
            AllNodes = new List<XmlNode>();
            nodes = new List<XmlNode>();

            if (coding == 0)
            {
                sr = new StreamReader(path, System.Text.Encoding.Default);
            }
            else
            {
                sr = new StreamReader(path, System.Text.Encoding.UTF8);
            }
            xmlStr = InitialXmlStr(sr.ReadToEnd());
            sr.Close();
            if (xmlStr != "")
            {
                if (!hasChinese(xmlStr))
                {
                    sr = new StreamReader(path, System.Text.Encoding.UTF8);
                    xmlStr = InitialXmlStr(sr.ReadToEnd());
                    sr.Close();
                }


                if (!VerifyXml())
                {
                    throw new XmlInvalidException();
                }
                int begin = -1;
                if (xmlStr.Contains("<!"))
                {
                    begin = xmlStr.IndexOf("!>");
                }
                else
                {
                    begin = xmlStr.IndexOf("<");

                }

                int end = xmlStr.LastIndexOf(">");
                root.InitialXmlNode(begin, end);
                AllNodes.Add(root);

                if (!root.hasChildren())
                {
                    AllNodes[0].setContent();
                }
                else
                {
                    root.ExtractAllNode();
                }
            }

        }

        public XmlDoc(string path, string tables)
        {
            xmlStr = string.Empty;
            root = new XmlNode();
            remark = string.Empty;
            AllNodes = new List<XmlNode>();
            nodes = new List<XmlNode>();
            //StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8"));
            //编码使用Default，否则会生成乱码
            StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
            xmlStr = InitialXmlStr(sr.ReadToEnd());
            sr.Close();

            if (!VerifyXml())
            {
                throw new XmlInvalidException();
            }
            int begin = xmlStr.IndexOf("!>");
            int end = xmlStr.LastIndexOf(">");
            root.InitialXmlNode(begin, end);

            AllNodes.Add(root);

            if (!root.hasChildren())
            {
                AllNodes[0].setContent();
            }
            else
            {
                root.ExtractAllNode(tables);
            }
        }

        //初始化XML字符串
        private string InitialXmlStr(string str)
        {
            while (str.IndexOf("< ") != -1)
            {
                str = str.Replace("< ", "<");  //"< " --> "<"
            }

            //while (str.IndexOf(" <") != -1)
            //{
            //    str = str.Replace(" <", "<");  //" <" --> "<"
            //}

            while (str.IndexOf(" >") != -1)
            {
                str = str.Replace(" >", ">");  //" >" --> ">"
            }

            //while (str.IndexOf("> ") != -1)
            //{
            //    str = str.Replace("> ", ">");  //"> " --> ">"
            //}

            while (str.IndexOf("/ ") != -1)
            {
                str = str.Replace("/ ", "/");  //"/ " --> "/"
            }

            return str;
        }

        //将XML写入到文本文件中
        public static void WriteToFile(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(XmlDoc.xmlStr);
            sw.Flush();
            sw.Close();
        }

        //验证XML文件的有效性
        private bool VerifyXml()
        {
            int i = xmlStr.IndexOf("<?");
            int j = -1;
            if (i != -1)
            {
                j = xmlStr.IndexOf("?>");
                if (j == -1)
                {
                    return false;
                }
            }
            if (i != -1 && j != -1)
            {
                int v = xmlStr.IndexOf("version");
                int m = -1, n = -1;
                if (v != -1)
                {
                    m = xmlStr.IndexOf("\"", i, j - i + 1);
                    n = xmlStr.IndexOf("\"", m + 1, j - m);
                    if (m == -1 || n == -1)
                    {
                        return false;
                    }
                    string version = xmlStr.Substring(m + 1, n - m - 1);
                    remark += "version:" + version;
                }

                int e = xmlStr.IndexOf("encoding");
                if (e != -1)
                {
                    if (n != -1)
                    {
                        m = xmlStr.IndexOf("\"", n + 1, j - n);
                        n = xmlStr.IndexOf("\"", m + 1, j - m);
                        if (m == -1 || n == -1)
                        {
                            return false;
                        }
                        string encoding = xmlStr.Substring(m + 1, n - m - 1);
                        remark += " encoding:" + encoding;
                    }
                    else
                    {
                        m = xmlStr.IndexOf("\"", e, j - n - 1);
                        n = xmlStr.IndexOf("\"", e, j - m - 1);
                        if (m == -1 || n == -1)
                        {
                            return false;
                        }
                        string encoding = xmlStr.Substring(m + 1, n - m - 1);
                        remark += " encoding:" + encoding;
                    }
                }
            }

            return true;
        }

        //将文档转化为字符串
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < XmlDoc.AllNodes.Count; i++)
            {
                if (XmlDoc.AllNodes[i].Depth() == 2)
                {
                    sb.Append(XmlDoc.AllNodes[i].ToString());
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        //根据元素名称查找元素
        public List<XmlNode> GetElementsByTagName(string name)
        {
            nodes.Clear();
            foreach (XmlNode node in AllNodes)
            {
                if (String.Compare(node.Name, name) == 0)
                {
                    nodes.Add(node);
                }
            }

            return nodes;
        }
    }

    class XmlInvalidException : Exception
    {
        private string message;
        public override string Message
        {
            get { return message; }
        }

        public XmlInvalidException()
        {
            this.message = "XML FILE INVALID!";
        }

        public XmlInvalidException(string str)
        {
            this.message = str;
        }
    }
}