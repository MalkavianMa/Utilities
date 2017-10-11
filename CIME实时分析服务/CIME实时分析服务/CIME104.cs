using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RealtimeAnalysis;
using Hisingpower;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using Hisingpower.DButility;
using System.Net;
using elanguage.Efile.Impl;
using elanguage.Efile;

namespace CIME实时分析服务
{
    public partial class CIME104 : Form
    {
        public CIME104()
        {
            InitializeComponent();
        }

        private void CIME104_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消关闭逻辑
            //e.Cancel = true;
            //this.Hide();

            e.Cancel = (new CloseForm(this)).ShowDialog() == DialogResult.OK ? false : true;
        }

        string _coding;

        public string Coding
        {
            get { return Common.getCFG("Ecoding"); }
            set { _coding = value; }
        }


        string _EFTP;

        public string EFTP
        {
            get { return Common.getCFG("EFTP"); }
            set { _EFTP = value; }
        }

        string _EMode;


        public string EMode
        {
            get { return Common.getCFG("EMode"); }
            set { _EMode = value; }
        }

        string _Ext;

        public string Ext
        {
            get { return "*." + Common.getCFG("Ext"); }
            set { _Ext = value; }
        }

        string _Connstr;

        public string Connstr
        {
            get
            {
                //return "Data Source=.;Initial Catalog=FAULTVISTA;Integrated Security=true;";
                return Hisingpower.DESEncrypt.Decrypt(Common.getCFG("ConnectString"));
            }
            set { _Connstr = value; }
        }

        string _Epath;

        public string Epath
        {
            get { return Common.getCFG("Epath"); }
            set { _Epath = value; }
        }

        int _Espan;

        public int Espan
        {
            get { return Convert.ToInt32(Common.getCFG("Espan")); }
            set { _Espan = value; }
        }
        int _ESavespan;

        public int ESavespan
        {
            get { return Convert.ToInt32(Common.getCFG("ESavespan")); }
            set { _ESavespan = value; }
        }


        private static string ftpuser = ""; //ftp用户名

        public static string Ftpuser
        {
            get
            {

                return Common.getCFG("user");
            }
            set
            {

                ftpuser = value;

            }
        }

        private static string ftppwd = "";  //ftp密码

        public static string Ftppwd
        {
            get
            {
                return DESEncrypt.Decrypt(Common.getCFG("pwd"));

            }
            set
            {

                ftppwd = value;

            }
        }

        System.Threading.Timer dataTimer;//抓取E文件timer，FTP抓取和下载E文件，下载后清空ftp目录
        System.Threading.Timer dataSaveTimer;//处理E文件timer，处理下载后的E文件，存入数据库
        #region ftp参数

        private static string currentDir = "/";
        private static FtpWebRequest request;

        #endregion
        string saveEpath = "";
        string oldDatapath = "";
        private void CIME104_Load(object sender, EventArgs e)
        {
            try
            {
                Common.WriteLog("CIME实时分析服务", "CIME实时分析服务启动。", Common.Now());
                saveEpath = Environment.CurrentDirectory + "\\TempFiles";
                if (!Directory.Exists(saveEpath))
                {
                    Directory.CreateDirectory(saveEpath);
                }
                oldDatapath = Environment.CurrentDirectory + "\\OldDataFiles";
                if (!Directory.Exists(oldDatapath))
                {
                    Directory.CreateDirectory(oldDatapath);
                }
                if (EFTP == "1")
                {
                    Common.WriteLog("CIME实时分析服务", "开启FTP抓取E文件功能。", Common.Now());
                    dataTimer = new System.Threading.Timer(new TimerCallback(dataTimerEvent), null, 0, (Espan * 1000));
                }
                else
                {
                    Common.WriteLog("CIME实时分析服务", "未开启FTP抓取E文件功能。", Common.Now());
                }
                dataSaveTimer = new System.Threading.Timer(new TimerCallback(dataSaveTimerEvent), null, 0, (ESavespan * 1000));
            }
            catch (System.Exception ex)
            {
                Common.WriteLog("CIME实时分析服务", "CIME实时分析服务出错！" + ex.Message.ToString(), Common.Now());
                this.lblstate.Text = "CIME实时分析服务出错！" + ex.Message.ToString();
            }
        }

        //ftp抓取
        private void dataTimerEvent(object source)
        {
            try
            {
                dataTimer.Change(Timeout.Infinite, -1);
                ScaenEfiles();
            }
            catch (System.Exception ex)
            {
                Common.WriteLog("CIME实时分析服务", "CIME实时分析服务[FTP抓取]出错！" + ex.Message.ToString(), Common.Now());
                this.lblstate.Text = "CIME实时分析服务[FTP抓取]出错！" + ex.Message.ToString();

            }
            finally
            {
                dataTimer.Change((Espan * 1000), (Espan * 1000));
            }

        }

        //E文件处理
        private void dataSaveTimerEvent(object source)
        {
            try
            {
                dataSaveTimer.Change(Timeout.Infinite, -1);
                saveFiles();
            }
            catch (System.Exception ex)
            {
                Common.WriteLog("CIME实时分析服务", "CIME实时分析服务[E文件处理]出错！" + ex.Message.ToString(), Common.Now());
                this.lblstate.Text = "CIME实时分析服务[E文件处理]出错！" + ex.Message.ToString();

            }
            finally
            {
                dataSaveTimer.Change((ESavespan * 1000), (ESavespan * 1000));
            }
        }
        private void ScaenEfiles()
        {
            //------ ftp抓取E文件开始 [2016/12/8 HISINGPOWER]
            if (!Epath.EndsWith("/"))
            {
                Epath += "/";
            }
            string[] FileList = null;
            try
            {
                FileList = List(Epath);
            }
            catch (System.Exception ex)
            {
                Common.WriteLog("CIME实时分析服务", "获取ftp文件列表失败（path=" + Epath + "）： " + ex.Message.ToString(), Common.Now());
                Thread.Sleep(5 * 1000);
                Common.WriteLog("CIME实时分析服务", "尝试再次获取文件列表", Common.Now());
                try
                {
                    FileList = List(Epath);
                }
                catch (System.Exception ex2)
                {
                    Common.WriteLog("CIME实时分析服务", "再次获取文件列表失败" + ex2.Message.ToString(), Common.Now());
                }


            }

            if (FileList != null)
            {
                if (FileList.Length > 0)
                {

                    for (int i = 0; i < FileList.Length; i++)
                    {
                        try
                        {
                            try
                            {
                                if (FileList[i] != "" && FileList[i].Contains("ALARM_MESSAGE") && FileList[i].Contains(".RB"))
                                {
                                    try
                                    {
                                        Download(saveEpath + "\\" + FileList[i], Epath + FileList[i]);
                                        Common.WriteLog("CIME实时分析服务", "下载文件" + Epath + FileList[i] + "到" + saveEpath + "\\" + FileList[i] + "。", Common.Now());
                                        //------ 下载后删除开始 [2016/12/8 HISINGPOWER]
                                        string uri = Epath + FileList[i];
                                        try
                                        {

                                            FtpWebRequest request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.DeleteFile);
                                            FtpWebResponse response = GetFtpResponse(request);
                                            if (response == null)
                                            {
                                                Common.WriteLog("CIME实时分析服务", "删除文件" + Epath + FileList[i] + "失败！...服务端未响应!", Common.Now());
                                                return;
                                            }
                                            Common.WriteLog("CIME实时分析服务", "删除文件" + Epath + FileList[i] + ",服务器返回:[" + response.StatusCode + "];[" + response.StatusDescription + "]", Common.Now());
                                        }
                                        catch (WebException err)
                                        {
                                            Common.WriteLog("CIME实时分析服务", "删除文件" + Epath + FileList[i] + "出错：" + err.Message.ToString(), Common.Now());
                                        }
                                        //------ 下载后删除结束 [2016/12/8 HISINGPOWER]
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Common.WriteLog("CIME实时分析服务", "下载文件" + Epath + FileList[i] + "到" + saveEpath + "\\" + FileList[i] + "。时出错：" + ex.Message.ToString(), Common.Now());
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {

                            }
                        }
                        catch (System.Exception ex)
                        {

                        }

                    }
                }
            }
            //------ ftp抓取E文件结束 [2016/12/8 HISINGPOWER]


        }

        private void saveFiles()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(saveEpath);
            FileInfo[] allFiles = dirinfo.GetFiles(Ext);

            foreach (FileInfo FileNm in allFiles)
            {
                if (FileNm.Extension == Ext.Substring(Ext.IndexOf(".")))
                {
                    DefaultEfileParse parse = new DefaultEfileParse();
                    String file = FileNm.FullName;
                    List<ETable> list = parse.ParseFile(file, Convert.ToInt32(Coding));
                    List<object[]> datas;

                    int success = 0;
                    //Etalbe集合
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            datas = list[i].GetDatas();

                            //D5000
                            //行
                            //@顺序    厂站名称    发生时间    短信内容

                            //#1 '华中.罗坊站'	'2016-12-08 14:56:17'	'2016-12-08 14:56:17      华中.罗坊站 华中.罗坊站/35kV.302开关      分闸'

                            //积成电子
                            //@Num    OccurTime    RDFID       status					content       
                            //序号  发生时间    开关编号     变位动作(0表示合转分，1表示分转合)     事项内容

                            if (datas.Count > 0)
                            {
                                string str = "";
                                string breaker = "";
                                string rtime = "";

                                switch (EMode)
                                {
                                    case "0":
                                        {
                                            //列
                                            rtime = datas[0][2].ToString();
                                            str = datas[0][3].ToString();
                                            //华中.罗坊站/35kV.302开关      分闸(停电)
                                            //江西.昌东站/220kV.211开关      分闸(封锁)
                                            //江西.罗湾厂/6kV.610开关      分闸

                                            string state = str.Substring(datas[0][3].ToString().LastIndexOf("      ") + 6);
                                            string statestr = state;
                                            if (state == "分闸" || state.Contains("合闸"))
                                            {


                                                breaker = str.Substring(datas[0][3].ToString().IndexOf("      ") + 6).Split(' ')[1];
                                                string sql = "select * from Relations_yaoxin where breakername='" + breaker + "'";
                                                SqlDataReader sdr = SqlHelper.ExecuteDataReader(Connstr, sql, null);
                                                if (sdr.HasRows)
                                                {


                                                    //[groupid],[sub],[line],[yaoxinid],[info],[yaoxintype],[breakername]
                                                    while (sdr.Read())
                                                    {
                                                        if (state == "分闸")
                                                        {
                                                            state = "0";
                                                        }
                                                        else if ((state.Contains("合闸")))
                                                        {
                                                            if (state == "合闸")
                                                            {
                                                                state = "1";
                                                            }
                                                            else if (state.Contains("相合闸"))
                                                            {
                                                                //如果同意时间（秒）中没有合成的线路合闸信号而有A/B/C相合闸，则解析为线路合闸
                                                                string sql0 = "select id from Datas_yaoxin where groupid=" + sdr["groupid"] + " and yaoxinid=" + sdr["yaoxinid"] + " and rtime='" + rtime + "' and state='1'";
                                                                if (SqlHelper.ExecuteScalar(Connstr, sql0, null) == null)
                                                                {
                                                                    state = "1";
                                                                }
                                                            }

                                                        }
                                                        if (state == "0" || state == "1")
                                                        {
                                                            string sql1 = "INSERT INTO [Datas_yaoxin]([groupid],[yaoxinid],[state],[yaoxintype],[rtime],[datatype])VALUES"
                                                                   + "(" + sdr["groupid"] + "," + sdr["yaoxinid"] + ",'" + state + "','开关','" + rtime + "','z')";
                                                            if (SqlHelper.ExecuteNonQuery(Connstr, sql1, null) > 0)
                                                            {
                                                                Common.WriteLog("CIME实时分析服务", sdr["sub"] + " " + sdr["line"] + "(" + breaker + ")" + statestr + "，接收时间 " + rtime, Common.Now());
                                                                Common.WriteLog("CIME实时分析服务[变位]", sdr["sub"] + " " + sdr["line"] + "(" + breaker + ")" + statestr + "(state=" + state + ")，接收时间 " + rtime, Common.Now());

                                                            }
                                                        }
                                                        else
                                                        {
                                                            Common.WriteLog("CIME实时分析服务[变位]", sdr["sub"] + " " + sdr["line"] + "(" + breaker + ")" + statestr + "，接收时间 " + rtime, Common.Now());
                                                        }

                                                    }
                                                    sdr.Close();
                                                }
                                                else
                                                {
                                                    sdr.Close();
                                                }
                                            }

                                            break;
                                        }
                                    case "1":
                                        {
                                            //列
                                            for (int j = 0; j < datas.Count; j++)
                                            {
                                                try
                                                {
                                                    rtime = datas[j][1].ToString().Replace("_", " ");
                                                    str = datas[j][2].ToString();
                                                    string state = datas[j][3].ToString();
                                                    string sql = "select * from Relations_yaoxin where breakername like '%." + str + "'";
                                                    SqlDataReader sdr = SqlHelper.ExecuteDataReader(Connstr, sql, null);
                                                    if (sdr.HasRows)
                                                    {
                                                        //[groupid],[sub],[line],[yaoxinid],[info],[yaoxintype],[breakername]
                                                        while (sdr.Read())
                                                        {
                                                            breaker = sdr["info"].ToString() + "@" + str;

                                                            string sql1 = "INSERT INTO [Datas_yaoxin]([groupid],[yaoxinid],[state],[yaoxintype],[rtime],[datatype])VALUES"
                                                                        + "(" + sdr["groupid"] + "," + sdr["yaoxinid"] + ",'" + state + "','开关','" + rtime + "','z')";
                                                            if (SqlHelper.ExecuteNonQuery(Connstr, sql1, null) > 0)
                                                            {
                                                                Common.WriteLog("CIME实时分析服务", sdr["sub"] + " " + sdr["line"] + "(" + breaker + ")跳闸，接收时间 " + rtime, Common.Now());
                                                                Common.WriteLog("CIME实时分析服务[跳闸]", sdr["sub"] + " " + sdr["line"] + "(" + breaker + ")跳闸，接收时间 " + rtime, Common.Now());

                                                            }
                                                        }
                                                        sdr.Close();
                                                    }
                                                    else
                                                    {
                                                        sdr.Close();
                                                    }
                                                }
                                                catch (System.Exception ex)
                                                {

                                                }
                                            }

                                            break;
                                        }
                                }




                            }

                            string oldpath = oldDatapath + "\\" + DateTime.Now.ToString("yyyyMMdd");
                            if (!Directory.Exists(oldpath))
                            {
                                Directory.CreateDirectory(oldpath);
                            }
                            oldpath += "\\" + FileNm.Name;
                            try
                            {

                                File.Copy(FileNm.FullName, oldpath, true);
                                File.Delete(FileNm.FullName);
                                Common.WriteLog("CIME实时分析服务", "将文件" + FileNm.FullName + "移动到" + oldpath + "。", Common.Now());
                            }
                            catch (System.Exception ex)
                            {
                                Common.WriteLog("CIME实时分析服务", "将文件" + FileNm.FullName + "移动到" + oldpath + "。时出错" + ex.Message.ToString(), Common.Now());
                            }


                        }
                        catch (System.Exception ex)
                        {
                            Common.WriteLog("CIME实时分析服务", "CIME实时分析服务[读取开关信息]出错！" + ex.Message.ToString(), Common.Now());
                        }



                    }
                }


            }
        }

        /// <summary>
        /// 获取服务器路径下的文件
        /// </summary>
        /// <param name="Url">服务器路径</param>
        /// <returns></returns>
        public static string[] List(string Url)
        {
            List<string> rs = new List<string>();
            request = (FtpWebRequest)FtpWebRequest.Create(Url);
            request.KeepAlive = false;
            request.Credentials = new NetworkCredential(Ftpuser, Ftppwd);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse rep = (FtpWebResponse)request.GetResponse();
            long len = rep.ContentLength;
            using (Stream s = rep.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                int readcount = 0;
                long totalcount = 0;
                byte[] buffer = new byte[20480];
                //缓冲区为1024时大约能获取41个文件名，因此需要增大。设置为20480是1024的20倍，经测试可以读取500个左右文件名。而且再增大缓存区也不会增加存储文件名的空间。2015年6月10日 10:30:33 bing
                while (true)
                {
                    readcount = s.Read(buffer, 0, 20480);
                    totalcount += readcount;
                    ms.Write(buffer, 0, readcount);
                    if (totalcount >= len)
                    {
                        break;
                    }
                }
                s.Close();
                rep.Close();
                string str = Encoding.UTF8.GetString(ms.ToArray());
                StringReader sr = new StringReader(str);
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    rs.Add(line);
                }
            }
            return rs.ToArray();
        }

        /// <summary>
        /// 从FTP服务器下载到本地文件
        /// </summary>
        /// <param name="filePath">本地文件存储路径</param>
        /// <param name="fileName">带路径的文件全名</param>
        public static string Download(string filePath, string fileName)
        {

            try
            {
                request = CreateFtpWebRequest(fileName, WebRequestMethods.Ftp.DownloadFile);
                request.KeepAlive = false;
                FtpWebResponse response = GetFtpResponse(request);
                if (response == null)
                {

                    return "0";
                }
                Stream responseStream = response.GetResponseStream();
                FileStream fileStream = File.Create(filePath);
                int bufLen = 8196;
                byte[] buf = new byte[bufLen];
                int bytesRead = 1;

                while (bytesRead != 0)
                {
                    bytesRead = responseStream.Read(buf, 0, buf.Length);
                    fileStream.Write(buf, 0, bytesRead);
                }
                responseStream.Close();
                fileStream.Close();
                return "1";

            }
            catch (WebException err)
            {
                return err.Message.ToString();
            }
        }

        //创建FtpWebRequest对象
        private static FtpWebRequest CreateFtpWebRequest(string uri, string requestMethod)
        {
            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            NetworkCredential networkCredential = new NetworkCredential(Ftpuser, Ftppwd); ;
            request.Credentials = networkCredential;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = requestMethod;
            return request;
        }

        //获取服务器返回的响应体
        private static FtpWebResponse GetFtpResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();

                return response;
            }
            catch (WebException err)
            {
                return null;
            }
        }
    }
}
