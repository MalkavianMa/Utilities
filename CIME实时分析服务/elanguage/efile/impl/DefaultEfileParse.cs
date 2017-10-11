using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
namespace elanguage.Efile.Impl
{
    public class DefaultEfileParse : EFileParse
    {

        public virtual List<ETable> ParseFile(String path, int coding)
        {

            List<ETable> tableList = new List<ETable>();
            XmlDoc doc = new XmlDoc(path, coding);

            for (int i = 0; i < XmlDoc.AllNodes.Count; i++)
            {
                XmlNode ele = XmlDoc.AllNodes[i];
                ETable table = new ETable();
                String tagName = ele.Name;

                if (tagName.Contains("::"))
                {
                    table.SetTableName(Regex.Split(tagName, "::")[1]);
                }
                else
                {
                    table.SetTableName(tagName);
                }
                table.SetDate(ele.Attribute.Content);


                //需判断Content是否为空
                if (ele.Content != "")
                {
                    ParseTableData(table, ele.Content);
                    tableList.Add(table);
                }


            }
            return tableList;
        }


        public virtual List<ETable> ParseFile(String path, string tables)
        {

            List<ETable> tableList = new List<ETable>();
            XmlDoc doc = new XmlDoc(path, tables);

            for (int i = 0; i < XmlDoc.AllNodes.Count; i++)
            {
                XmlNode ele = XmlDoc.AllNodes[i];
                ETable table = new ETable();
                String tagName = ele.Name;

                if (tagName.Contains("::"))
                {
                    table.SetTableName(Regex.Split(tagName, "::")[1]);
                }
                else
                {
                    table.SetTableName(tagName);
                }
                table.SetDate(ele.Attribute.Content);


                //需判断Content是否为空
                if (ele.Content != "")
                {
                    ParseTableData(table, ele.Content);
                    tableList.Add(table);
                }


            }
            return tableList;
        }


        public virtual List<ETable> ParseFile(String path)
        {

            List<ETable> tableList = new List<ETable>();
            XmlDoc doc = new XmlDoc(path);

            for (int i = 0; i < XmlDoc.AllNodes.Count; i++)
            {
                XmlNode ele = XmlDoc.AllNodes[i];
                ETable table = new ETable();
                String tagName = ele.Name;
                if (tagName.Contains("::"))
                {
                    table.SetTableName(Regex.Split(tagName, "::")[1]);
                }
                else
                {
                    table.SetTableName(tagName);
                }
                table.SetDate(ele.Attribute.Content);


                //需判断Content是否为空
                if (ele.Content != "")
                {
                    ParseTableData(table, ele.Content);
                    tableList.Add(table);
                }



            }
            return tableList;
        }

        public virtual void CreateEFile(ETable etb, string startsWith, string path)
        {
            XmlDoc.xmlStr = "<!System=FAULTVISTA Version=1.0 Code=UTF-8 Data=1.0!>\n";
            XmlDoc.xmlStr += etb.ToString(startsWith);
            XmlDoc.WriteToFile(path);
        }



        private void ParseTableData(ETable table, String content)
        {
            // System.out.println("==context=="+content);
            String[] contentArr = Regex.Split(content, "\n");
            String headStr = contentArr[0].Trim();
            if (headStr.StartsWith("@@"))
            {
                // 单列式
                ParseSingleColType(table, contentArr);
            }
            else if (headStr.StartsWith("@#"))
            {
                // 多列式
                ParseMultiColType(table, contentArr);
            }
            else if (headStr.StartsWith("@"))
            {
                // 横表式
                ParseRowType(table, contentArr);
            }
            string stable = table.ToString("@#");
        }

        /// <summary>
        /// 解析单列式
        /// </summary>
        ///
        /// @@
        /// 
        /// @param table
        /// <param name="contentArr"></param>
        private void ParseSingleColType(ETable table, String[] contentArr)
        {
            String headStr = contentArr[0].Trim();
            String[] headerArr = SplitString(headStr.Substring("@@".Length));
            if (contentArr.Length > 1)
            {
                int propNo = 0;
                /**
                 * 存储属性name--value的Map
                 */
                Dictionary<string, string> propMap = new Dictionary<string, string>();
                int loopTime = 0;

                for (int i = 1; i < contentArr.Length; i++)
                {
                    String linestr = contentArr[i].Trim();
                    // System.out.println("linestr="+linestr);

                    if (linestr.StartsWith("#"))
                    {
                        linestr = linestr.Substring(1);
                        String[] lineArr = SplitString(linestr);
                        String[] popArr = new String[] { "", "" };
                        if (headerArr.Length == 2)
                        {
                            System.Array.Copy((Array)(lineArr), 0, (Array)(popArr), 0, lineArr.Length);
                        }
                        else if (headerArr.Length == 3)
                        {
                            System.Array.Copy((Array)(lineArr), 1, (Array)(popArr), 0, lineArr.Length - 1);
                        }
                        if (propMap.ContainsKey(popArr[0]))
                        {
                            // 把属性值 加入到table的数据列表
                            String[] data = new String[propMap.Values.Count];
                            propMap.Values.CopyTo(data, 0);
                            table.GetDatas().Add(data);
                            propMap.Clear();

                        }
                        propMap.Add(popArr[0], popArr[1]);

                    }

                }
                // 设置列名
                String[] columns = new String[propMap.Keys.Count];
                propMap.Keys.CopyTo(columns, 0);
                table.SetColumnNames(columns);
                // 把属性值 加入到table的数据列表
                String[] data_0 = new String[propMap.Values.Count];
                propMap.Values.CopyTo(data_0, 0);
                table.GetDatas().Add(data_0);
                propMap.Clear();
            }
        }

        /// <summary>
        /// 解析多列式
        /// </summary>
        ///
        /// <param name="table"></param>
        /// <param name="contentArr"></param>
        private void ParseMultiColType(ETable table, String[] contentArr)
        {
            if (contentArr.Length > 1)
            {
                List<string[]> porpList = new List<string[]>();
                for (int i = 1; i < contentArr.Length; i++)
                {
                    String linestr = contentArr[i].Trim();
                    // System.out.println("linestr="+linestr);
                    if (linestr.StartsWith("#"))
                    {
                        linestr = linestr.Substring(1);
                        String[] lineArr = SplitString(linestr);
                        porpList.Add(lineArr);
                    }

                }
                String[] columnNames = new String[porpList.Count];
                for (int col = 0; col < columnNames.Length; col++)
                {
                    columnNames[col] = porpList[col][1];
                }
                table.SetColumnNames(columnNames);

                for (int row = 2; row < porpList[0].Length; row++)
                {
                    String[] data = new String[columnNames.Length];
                    for (int col_0 = 0; col_0 < data.Length; col_0++)
                    {
                        data[col_0] = porpList[col_0][row];
                    }
                    table.GetDatas().Add(data);
                }

            }
        }

        /// <summary>
        /// 解析横表式
        /// </summary>
        ///
        /// <param name="table"></param>
        /// <param name="contentArr"></param>
        private void ParseRowType(ETable table, String[] contentArr)
        {
            String headStr = contentArr[0].Trim();
            String[] headerArr = SplitString(headStr.Substring("@".Length));
            String[] columnNames = new String[headerArr.Length];
            System.Array.Copy((Array)(headerArr), 0, (Array)(columnNames), 0, columnNames.Length);
            table.SetColumnNames(columnNames);
            if (contentArr.Length > 1)
            {
                for (int i = 1; i < contentArr.Length; i++)
                {
                    String linestr = contentArr[i].Trim();
                    System.Console.Out.WriteLine("datastr=" + linestr);

                    if (linestr.StartsWith("%"))
                    {
                        // 类型
                        String[] lineArr = SplitString(linestr.Substring(1));
                        String[] type = new String[lineArr.Length];
                        System.Array.Copy((Array)(lineArr), 0, (Array)(type), 0, type.Length);
                        table.SetTypes(type);
                    }
                    else if (linestr.StartsWith("$"))
                    {
                        // 梁纲引导符 单位
                        String[] lineArr_0 = SplitString(linestr.Substring(1));
                        String[] unites = new String[lineArr_0.Length];
                        System.Array.Copy((Array)(lineArr_0), 0, (Array)(unites), 0, unites.Length);
                        table.SetUnites(unites);

                    }
                    else if (linestr.StartsWith(":"))
                    {
                        // 限值引导符
                        String[] lineArr_1 = SplitString(linestr.Substring(1));
                        String[] limitValues = new String[lineArr_1.Length];
                        System.Array.Copy((Array)(lineArr_1), 0, (Array)(limitValues), 0, limitValues.Length);
                        table.SetLimitValues(limitValues);

                    }
                    else if (linestr.StartsWith("#"))
                    {
                        // 数据值

                        String[] lineArr_2 = SplitString(linestr.Substring(1));
                        String[] data = new String[lineArr_2.Length];
                        System.Array.Copy((Array)(lineArr_2), 0, (Array)(data), 0, data.Length);
                        table.GetDatas().Add(data);
                    }
                    //else if (linestr.StartsWith("//"))
                    //{

                    //}

                }
            }
        }

        /// <summary>
        /// 分割字符串为数组
        /// </summary>
        ///
        /// <param name="str"></param>
        /// <returns></returns>
        private String[] SplitString(String str)
        {
            return elanguage.Util.StringUtils.SplitLineWithSpace(str);
        }

    }
}
