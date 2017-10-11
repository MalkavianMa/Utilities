using elanguage.Efile;
using elanguage.Efile.Impl;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
 namespace elanguage 
 {
	public class MainTest {
	
		public static void Main(String[] args) {
	
			try {
				DefaultEfileParse parse = new DefaultEfileParse();
                String file = "E:\\6.������Ŀ\\elanguage\\data\\���ʽ.txt";
				List<ETable> list = parse.ParseFile(file);
				elanguage.Util.Debug.Debugwrite(list);
                ETable et = new ETable();
                et.SetColumnNames(new string[] {"˳��", "�ֶ�1", "�ֶ�2", "�ֶ�3" });
                object[] o1 = new object[] { "1","111","333","222"};
                object[] o2 = new object[] { "2", "411", "343", "222" };
                object[] o3 = new object[] { "3", "141", "333", "242" };
                List<object[]> listo = new List<object[]>();
                listo.Add(o1);
                listo.Add(o2);
                listo.Add(o3);
                et.SetDatas(listo);
                et.SetTableName("��Ͻ��");
                et.SetDate(DateTime.Now.ToString());
                parse.CreateEFile(et,"@" ,"D:\\test.txt");
                
			} catch (Exception e) {
				Console.Error.WriteLine(e.StackTrace);
			}

		}
	}
}
