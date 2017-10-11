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
        /// ���ļ�������String��ʽ���ļ�����
        /// </summary>
        ///
        /// <param name="path">�ļ�·��</param>
        /// <exception cref="IOException"></exception>
        public String OpenFile(String path)
        {
            // �ж��ļ���ʽ�Ƿ���ȷ
            if (this.CheckFile(path) != 1)
            {
                throw new Exception("�ļ���ʽ����ȷ");
            }

            // ���ļ�ת��ΪUTF-8��ʽ
            try
            {
                EfileUtils.TransferFile(path);
            }
            catch (IOException e)
            {
                throw new Exception("�ļ�ת��ΪUTF-8��ʽ�г���");
            }
            // �����ļ�ΪString����
            String content_temp = null;
            try
            {
                content_temp = ChangeFileToString(path);
            }
            catch (IOException e_0)
            {
                throw new Exception("�ļ�����ת��ΪString�г���");
            }
            // ȥ���ļ�ͷ��Ϣ
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
                // ���ļ�����Ϊ�ַ�����ʽ
                while ((temp = read.ReadLine()) != null)
                {
                    if (temp.Contains("\\") || temp.Contains("//"))
                    {
                        content_temp.Append(temp.Replace("\\\\", "").Replace(
                                "////", "")
                                + "\n");// ���ӽ���Ч��
                    }
                    else
                    {
                        content_temp.Append(temp.Trim() + "\n");// ���ӽ���Ч��
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("�����ļ�ΪStringʱ����");
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
        /// ��ȡ�ַ�����ȥ��ģ���ͷ���磺<!System=SGTMS type='ȱ��Ӱ��ҵ�����' Version=1.0 Code=UTF-8
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
        /// �ж�Դ�ļ������Ƿ�Ϊutf-8,���ǵĻ���Դ�ļ�����ת����utf-8
        /// </summary>
        ///
        /// <param name="srcFileName">�ļ�·��+�ļ���</param>
        /// <exception cref="IOException"></exception>
        public static void TransferFile(String srcFileName) {
			FileInfo file = new FileInfo(srcFileName);
			// ��ȡԴ�ļ�����
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
        /// �����ļ��õ����ļ����ı����ݵı���
        /// </summary>
        ///
        /// <param name="file">Ҫ�������ļ�</param>
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
        /// �ж��ļ��Ƿ���Ͻ�������
        /// </summary>
        ///
        /// <param name="file">Ҫ�������ļ�·�� �÷���Ϊ�գ����������ж��ļ���ȷ���ڴ˷�����Ӽ��ɣ�����1�ϸ񣬷���0����</param>
        public int CheckFile(String path)
        {

            return 1;
        }

    }
}
