using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
 namespace elanguage.Util 
 {
	public class StringUtils {
	
		/// <summary>
		/// 分割以space分割，或‘’包装的字符串 例如： "2 发生时间 '2011-11-03 00:00:02.0'";
		/// </summary>
		///
		/// <param name="line"></param>
		/// <returns></returns>
        public static string[] SplitLineWithSpace(string line)
        {

            List<string> list = new List<string>();
			ParseSection(list, line, 0, 0);
			return list.ToArray();
	
		}

        private static void ParseSection(List<string> list, string line, int start,int pos) {
			char chr = '0';
			char wrapper = '\'';
			bool sectionEnd = false;
			bool lineEnd = false;
			bool hasWrapper = false;
	
			// 跳过空白字符
			do {
				// lineEnd=(pos>=line.length());
				if (pos < line.Length)
					chr = line[pos++];
				else
					break;
	
			} while (IsSpace(chr));
	
			if (IsWrapper(chr)) {
				wrapper = chr;
				hasWrapper = true;
				start = pos;
			} else {
				start = --pos;
			}
	
			while (!sectionEnd && !lineEnd) {
				chr = line[pos++];
				if (pos >= line.Length) {
					lineEnd = true;
				} else if (hasWrapper) {
					if (chr == wrapper) {
						sectionEnd = true;
					}
				} else {
					if (IsSpace(chr))
						sectionEnd = true;
				}
			}
			// pos--;
			String sectionStr = "";
			if (hasWrapper) {
				sectionStr = line.Substring(start,(pos - 1)-(start));
			} else {
				sectionStr = line.Substring(start,(pos)-(start)).Trim();
			}
	
			//System.Console.Out.WriteLine(sectionStr);
            list.Add(sectionStr);
			if (!lineEnd) {
				ParseSection(list, line, start, pos);
			}
		}
	
		private static readonly char[] wrappers = new char[] { '\'', '\"' };
	
		/// <summary>
		/// 判断字符是否是单双引号
		/// </summary>
		///
		/// <param name="chr"></param>
		/// <returns></returns>
		private static bool IsWrapper(char chr) {
			/* foreach */
			foreach (char c  in  wrappers) {
				if (c == chr) {
					return true;
				}
			}
			return false;
		}
	
		/// <summary>
		/// 是否是空白字符
		/// </summary>
		///
		/// <param name="chr"></param>
		/// <returns></returns>
		public static bool IsSpace(char chr) {
			return (chr == ' ' || chr == '\n' || chr == '\t' || chr == '\r' || chr == '\f');
		}
	
		public static bool IsInteger(String str) {
			String regx = "\\d+";
            return Regex.IsMatch(str, regx);
		}

	}
}
