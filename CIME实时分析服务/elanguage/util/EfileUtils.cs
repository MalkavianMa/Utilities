using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
namespace elanguage.Util
{
    /// <summary>
    /// <p>
    /// Title: // TODO
    /// </p>
    /// <p>
    /// Description: // TODO
    /// </p>
    /// </summary>
    ///

    public class EfileUtils
    {
        /// <summary>
        /// 打开文件，返回String格式的文件内容
        /// </summary>
        ///
        /// <param name="path">文件路径</param>
        /// <exception cref="IOException"></exception>
        public String OpenFile(String path)
        {
            // 判断文件格式是否正确
            if (this.CheckFile(path) != 1)
            {
                throw new Exception("文件格式不正确");
            }

            // 将文件转换为UTF-8格式
            try
            {
                EfileUtils.TransferFile(path);
            }
            catch (IOException e)
            {
                throw new Exception("文件转换为UTF-8格式中出错");
            }
            // 解析文件为String类型
            String content_temp = null;
            try
            {
                content_temp = ChangeFileToString(path);
            }
            catch (IOException e_0)
            {
                throw new Exception("文件内容转换为String中出错");
            }
            // 去除文件头信息
            String fileStr = this.RemoveHeadFormat(content_temp);
            return fileStr;
        }

        private String ChangeFileToString(String path)
        {
            TextReader read = null;
            StringBuilder content_temp = null;
            StreamReader sr = null;
            try
            {
                content_temp = new StringBuilder();
                sr = new StreamReader(new FileInfo(path).OpenRead(), System.Text.Encoding.GetEncoding("UTF-8"));
                read = sr;
                String temp = "";
                int i = 0;
                // 将文件解析为字符串格式
                while ((temp = read.ReadLine()) != null)
                {
                    if (temp.Contains("\\") || temp.Contains("//"))
                    {
                        content_temp.Append(temp.Replace("\\\\", "").Replace(
                                "////", "")
                                + "\n");// 增加解析效率
                    }
                    else
                    {
                        content_temp.Append(temp.Trim() + "\n");// 增加解析效率
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("解析文件为String时出错");
            }
            finally
            {
                if (null != read)
                    read.Close();
                if (null != sr)
                    sr.Close();
            }
            return content_temp.ToString();
        }

        /// <summary>
        /// 读取字符串并去掉模块表头；如：<!System=SGTMS type='缺陷影响业务情况' Version=1.0 Code=UTF-8
        /// Data=1.0!>
        /// </summary>
        ///
        public String RemoveHeadFormat(String fileStr)
        {
            StringBuilder eBuff = new StringBuilder("");
            while (fileStr.Contains("<!") && fileStr.Contains("!>"))
            {
                int start = fileStr.IndexOf("<!");
                int end = fileStr.IndexOf("!>");
                if (start > 0)
                {
                    int headendpos = start - 1;
                    eBuff.Append(fileStr.Substring(0, (headendpos) - (0)));
                }
                eBuff.Append(fileStr.Substring(end + 2));
                fileStr = eBuff.ToString().Trim();
                eBuff = new StringBuilder("");
            }
            return fileStr;
        }

        /// <summary>
        /// 判断源文件编码是否为utf-8,不是的话将源文件编码转换成utf-8
        /// </summary>
        ///
        /// <param name="srcFileName">文件路径+文件名</param>
        /// <exception cref="IOException"></exception>
        public static void TransferFile(String srcFileName) {
			FileInfo file = new FileInfo(srcFileName);
			// 获取源文件编码
			String charset = GetCharset(file);
			if (null != charset && !charset.Equals("UTF-8")) {
				String line_separator = Environment.GetEnvironmentVariable("line.separator");
				FileStream fis = File.OpenRead(srcFileName);
				StringBuilder content = new StringBuilder();
                TextReader d = new StreamReader(fis, System.Text.Encoding.GetEncoding(charset));// "UTF-8"
				String line = null;
				while ((line = d.ReadLine()) != null)
					content.Append(line + line_separator);
				d.Close();
				fis.Close();
				StreamWriter sw = new StreamWriter(srcFileName,false,System.Text.Encoding.GetEncoding("utf-8"));
                sw.Write(content.ToString());
                sw.Close();
			}
		}

        /// <summary>
        /// 根据文件得到该文件中文本内容的编码
        /// </summary>
        ///
        /// <param name="file">要分析的文件</param>
        public static String GetCharset(FileInfo file)
        {

            BufferedStream bis = new BufferedStream(file.OpenRead());
            System.IO.BinaryReader br = new System.IO.BinaryReader(bis);
            Byte[] buffer = br.ReadBytes(2);
            br.Close();
            bis.Close();

            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return "UTF-8";
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return "UTF-16BE";
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return "UTF-16LE";

                }
                else
                {
                    return "GBK";
                }
            }
            else
            {
                return "GBK";
            }
        }

        /// <summary>
        /// 判断文件是否符合解析条件
        /// </summary>
        ///
        /// <param name="file">要分析的文件路径 该方法为空，后续如需判断文件正确性在此方法添加即可，返回1合格，返回0错误</param>
        public int CheckFile(String path)
        {

            return 1;
        }

    }
}
