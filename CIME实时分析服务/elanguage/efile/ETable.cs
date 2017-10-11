using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
namespace elanguage.Efile
{
    public class ETable
    {
        /// <summary>
        /// 表名
        /// </summary>
        ///
        private String tableName;

        private String date;
        /// <summary>
        /// 列名
        /// </summary>
        ///
        private String[] columnNames;
        /// <summary>
        /// 类型
        /// </summary>
        ///
        private String[] types;
        /// <summary>
        /// 单位
        /// </summary>
        ///
        private String[] unites;
        /// <summary>
        /// 限定值
        /// </summary>
        ///
        private String[] limitValues;

        private List<object[]> datas;

        public ETable()
        {
            this.datas = null;
            datas = new List<object[]>();
        }

        public String GetTableName()
        {
            return tableName;
        }

        public void SetTableName(String tableName_0)
        {
            this.tableName = tableName_0;
        }

        public String[] GetColumnNames()
        {
            return columnNames;
        }

        public void SetColumnNames(String[] columnNames_0)
        {
            this.columnNames = columnNames_0;
        }

        public List<object[]> GetDatas()
        {
            return datas;
        }

        public void SetDatas(List<object[]> datas_0)
        {
            this.datas = datas_0;
        }

        public String GetDate()
        {
            return date;
        }

        public void SetDate(String date_0)
        {
            this.date = date_0;
        }

        public String[] GetTypes()
        {
            return types;
        }

        public void SetTypes(String[] types_0)
        {
            this.types = types_0;
        }

        public String[] GetUnites()
        {
            return unites;
        }

        public void SetUnites(String[] unites_0)
        {
            this.unites = unites_0;
        }

        public String[] GetLimitValues()
        {
            return limitValues;
        }

        public void SetLimitValues(String[] limitValues_0)
        {
            this.limitValues = limitValues_0;
        }
        /// <summary>
        /// 生成E格式文件
        /// </summary>
        /// <param name="startsWith">"@@"为单列式，"@#"为多列式,"@"为横表式</param>
        /// <returns></returns>
        public string ToString(string startsWith)
        {
            string result = string.Format("<故障定位::{0} date='{1}'>\n", tableName, date);
            string[] str;

            switch (startsWith)
            {
                case "@@": //单列式
                    result += startsWith +"顺序 属性名 属性值\n";
                    for (int i = 0; i < datas.Count; i++)
                    {
                        str = new string[datas[i].Length];
                        datas[i].CopyTo(str, 0);
                        
                        for (int j = 0; j < columnNames.Length; j++)
                        {
                            result += string.Format("#{0} {1} {2}\n", j + 1, columnNames[j], str[j]);
                        }
                    }
                    break;
                case "@#": //多列式
                    result += startsWith + "顺序 属性名";
                    for (int n = 0; n < datas.Count; n++)
                    {
                        result += string.Format(" 值{0}",n+1);
                    }
                    result += "\n";
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            result += string.Format("#{0} {1}", i + 1, columnNames[i]);
                            for (int j = 0; j < datas.Count; j++)
                            {
                                str = new string[datas[j].Length];
                                datas[j].CopyTo(str, 0);
                                result += " " + str[i];
                            }
                            result += "\n";
                        }
                    break;
                case "@": //横表式
                    result += startsWith + string.Join(" ", columnNames) + "\n";
                    for (int i = 0; i < datas.Count; i++)
                    {
                        str = new string[datas[i].Length];
                        datas[i].CopyTo(str, 0);
                        result += string.Format("#{0}\n",  string.Join(" ", str));
                    }
                    break;
            }
            result += string.Format("</故障定位::{0}>\n", tableName);
            return result;
        }
    }
}
