using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elanguage.Efile
{
    public class NodeAttribute
    {
        public string Name = string.Empty;
        public string Content = string.Empty;
        public NodeAttribute()
        {

        }
        public NodeAttribute(string name, string content)
        {
            this.Name = name;
            this.Content = content;
        }
    }

    class XmlNode
    {
        private string name = string.Empty;
        private string content = string.Empty;
        private NodeAttribute attribute = new NodeAttribute();
        internal int startindex1 = -1;
        internal int startindex2 = -1;
        internal int endindex1 = -1;
        internal int endindex2 = -1;
        internal string startMarker = string.Empty;
        internal string endMarker = string.Empty;
        internal bool hasChild = false; //是否有子元素
        private bool hasNext = false; //是否有兄弟元素
        private List<XmlNode> nodes = new List<XmlNode>();
        internal XmlNode father;

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
                this.startMarker = "<" + this.name + ">";
                this.endMarker = "</" + this.name + ">";
            }
        }

        public NodeAttribute Attribute
        {
            get { return attribute; }
            set { attribute = value; }
        }

        //获取元素的内容
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        //获取元素的子元素
        public List<XmlNode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        //提取该元素的所有子元素
        internal void ExtractAllNode()
        {
            XmlNode node = this;

            if (node.hasChildren())
            {
                node.GetChildren();
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    node.Nodes[i].ExtractAllNode();
                }
            }
            else
            {
                node.setContent();
            }
        }

        //提取该元素的指定子元素
        internal void ExtractAllNode(string tables)
        {
            XmlNode node = this;
            if (node.hasChildren())
            {
                node.GetChildren();
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    bool mytable = false;
                    if (!tables.Contains(","))
                    {

                        if (node.Nodes[i].Name==tables)
                            mytable = true;
                    } 
                    else
                    {
                        for (int j = 0; j < tables.Split(',').Length; j++)
                        {
                            if (node.Nodes[i].Name==tables.Split(',')[j])
                            {
                                mytable = true;
                                break;
                            }
                        }
                    }
                  
                    if (mytable)
                    {

                        node.Nodes[i].ExtractAllNode();
                    }


                }
            }
            else
            {
                node.setContent();
            }
        }



        //提取元素的下一层元素
        private void GetChildren()
        {
            XmlNode node = this;
            int begin = node.startindex2 + 1;
            int end = node.endindex1 - 1;
            XmlNode newnode = new XmlNode();
            newnode.father = node;
            newnode.InitialXmlNode(begin, end);
            node.nodes.Add(newnode);
            XmlDoc.AllNodes.Add(newnode);

            newnode.GetBrothers();
        }

        //提取该元素之后的所有兄弟元素
        private void GetBrothers()
        {
            XmlNode node = this;
            while (node.hasNextBrother())
            {
                int begin = node.endindex2 + 1;
                int end = node.father.endindex1 - 1;
                XmlNode newnode = new XmlNode();
                newnode.father = node.father;
                newnode.InitialXmlNode(begin, end);
                node.father.nodes.Add(newnode);
                XmlDoc.AllNodes.Add(newnode);
                node = newnode;
            }
        }

        //判断元素是否有子元素
        internal bool hasChildren()
        {
            int begin = this.startindex2 + 1;
            int end = this.endindex1 - 1;
            int index = XmlDoc.xmlStr.IndexOf("<", begin, end - begin + 1);
            if (index != -1)
            {
                this.hasChild = true;
                return true;
            }

            return false;
        }

        //判断元素是否还有下一个兄弟元素
        internal bool hasNextBrother()
        {
            int begin = this.endindex2 + 1;
            if (this.father == null)
            {
                return false;
            }
            int end = this.father.endindex1 - 1;
            int index = XmlDoc.xmlStr.IndexOf("<", begin, end - begin + 1);
            if (index != -1)
            {
                this.hasNext = true;
                return true;
            }

            return false;
        }

        //设置元素的内容
        internal void setContent()
        {
            XmlNode node = this;
            //if (this.hasChildren())
            if (node.hasChildren())
            {
                return;
            }

            this.Content = XmlDoc.xmlStr.Substring(this.startindex2 + 1, this.endindex1 - this.startindex2 - 1).Trim();

        }


        /// <summary>
        /// 根据元素在字符串中的起始索引和结束索引设置元素的字段信息
        /// </summary>
        /// <param name="begin">初始索引</param>
        /// <param name="end">结束索引</param>
        internal void InitialXmlNode(int begin, int end)
        {
            this.startindex1 = XmlDoc.xmlStr.IndexOf("<", begin, end - begin + 1);
            this.startindex2 = XmlDoc.xmlStr.IndexOf(">", this.startindex1 + 1, end - startindex1);
            if (this.startindex1 == -1 || this.startindex2 == -1)
            {
                throw new XmlInvalidException();
            }
            int temp = XmlDoc.xmlStr.IndexOf(" ", this.startindex1, this.startindex2 - this.startindex1 + 1);
            if (temp != -1)
            {
                this.startMarker = XmlDoc.xmlStr.Substring(this.startindex1, temp - this.startindex1 + 1).Trim() + ">";
                this.InitialNodeAttribute(this.startindex1, this.startindex2);
            }
            else
            {
                this.startMarker = XmlDoc.xmlStr.Substring(this.startindex1, this.startindex2 - this.startindex1 + 1);
            }

            this.name = this.startMarker.Substring(1, this.startMarker.Length - 2).Trim();
            //this.endMarker = this.startMarker.Replace("<", "</");
            this.endMarker = "</" + this.name + ">";
            this.endindex1 = XmlDoc.xmlStr.IndexOf(this.endMarker, this.startindex2 + 1);
            if (this.endindex1 == -1)
            {
                throw new XmlInvalidException(string.Format("没有匹配的元素尾:{0}", this.endMarker));
            }
            this.endindex2 = this.endindex1 + this.endMarker.Length - 1;
        }

        /// <summary>
        /// 根据元素头在字符串中的起始索引和终止索引设置元素的属性
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        private void InitialNodeAttribute(int begin, int end)
        {
            int cbegin = XmlDoc.xmlStr.IndexOf("'", begin, end - begin + 1) + 1;
            int cend = XmlDoc.xmlStr.IndexOf("'", cbegin, end - cbegin + 1) - 1;
            int equal = XmlDoc.xmlStr.LastIndexOf("=", end, end - begin + 1);
            int nbegin = XmlDoc.xmlStr.IndexOf(" ", begin, equal - begin + 1);
            int nend = XmlDoc.xmlStr.IndexOf("=", nbegin + 1, equal - nbegin);
            nend = nend == -1 ? equal - 1 : nend; //属性等号之前无空格
            this.attribute.Name = XmlDoc.xmlStr.Substring(nbegin, nend - nbegin).Trim();
            this.attribute.Content = XmlDoc.xmlStr.Substring(cbegin, cend - cbegin + 1).Trim();
        }

        //用于输出格式化结点
        public override string ToString()
        {
            string str = "";
            int deep = this.Depth();

            //深度大于2的元素，只显示子元素名称，不显示具体内容
            if (deep > 2)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    str += "{" + this.Nodes[i].Name + "}";
                }
            }
            else if (deep == 2) //深度等于2的元素，显示所有子元素的名称及其内容
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    str += this.Nodes[i].Name + ":" + this.Nodes[i].Content + "\t";
                }
                if (this.Attribute.Name != "")
                {
                    str += this.Attribute.Name + ":" + this.Attribute.Content;
                }
            }
            else  //深度等于1的元素，显元素的名称及其内容
            {
                return this.Name + ":" + this.Content;
            }

            return str;
        }

        //获取元素的深度，没有子元素的深度为1
        internal int Depth()
        {
            if (!this.hasChild)
            {
                return 1;
            }
            else
            {
                int max = 1;
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    int deep = this.Nodes[i].Depth() + 1;
                    max = max > deep ? max : deep;
                }

                return max;
            }
        }

        //添加子元素
        public void AppendChild(XmlNode node)
        {
            string str = node.StringXML();
            node.father = this;
            this.Nodes.Add(node);
            XmlDoc.xmlStr = XmlDoc.xmlStr.Insert(this.startindex2 + 1, "\r\n" + str);
        }

        //获取结点的字符串表示形式
        private string StringXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.startMarker + this.Content + this.endMarker);
            if (this.Attribute.Name != "")
            {
                string temp = string.Format("{0} {1}=\"{2}\">",
                    this.startMarker.Substring(0, this.startMarker.Length - 1),
                    this.Attribute.Name,
                    this.Attribute.Content);
                sb.Replace(this.startMarker, temp);
            }

            return sb.ToString();
        }
    }
}