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
        /// ����
        /// </summary>
        ///
        private String tableName;

        private String date;
        /// <summary>
        /// ����
        /// </summary>
        ///
        private String[] columnNames;
        /// <summary>
        /// ����
        /// </summary>
        ///
        private String[] types;
        /// <summary>
        /// ��λ
        /// </summary>
        ///
        private String[] unites;
        /// <summary>
        /// �޶�ֵ
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
        /// ����E��ʽ�ļ�
        /// </summary>
        /// <param name="startsWith">"@@"Ϊ����ʽ��"@#"Ϊ����ʽ,"@"Ϊ���ʽ</param>
        /// <returns></returns>
        public string ToString(string startsWith)
        {
            string result = string.Format("<���϶�λ::{0} date='{1}'>\n", tableName, date);
            string[] str;

            switch (startsWith)
            {
                case "@@": //����ʽ
                    result += startsWith +"˳�� ������ ����ֵ\n";
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
                case "@#": //����ʽ
                    result += startsWith + "˳�� ������";
                    for (int n = 0; n < datas.Count; n++)
                    {
                        result += string.Format(" ֵ{0}",n+1);
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
                case "@": //���ʽ
                    result += startsWith + string.Join(" ", columnNames) + "\n";
                    for (int i = 0; i < datas.Count; i++)
                    {
                        str = new string[datas[i].Length];
                        datas[i].CopyTo(str, 0);
                        result += string.Format("#{0}\n",  string.Join(" ", str));
                    }
                    break;
            }
            result += string.Format("</���϶�λ::{0}>\n", tableName);
            return result;
        }
    }
}
