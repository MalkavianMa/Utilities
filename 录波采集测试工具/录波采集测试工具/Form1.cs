using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace 录波采集测试工具
{
    public partial class Form1 : Form
    {
        public string startdraw = "";
        public string drawStartTime = "";
        public int timeDraw = 10;
        public int monoDebug=0;

        public DateTime spa;
        TimeSpan ts = new TimeSpan(1, 0, 0, 0);
        System.Threading.Timer dataTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Recording();
            //startdraw = Assistant_Form.getCFG("startdraw");
            drawStartTime = Assistant_Form.getCFG("drawStartTime");
            timeDraw = Convert.ToInt32(Assistant_Form.getCFG("caijijiange"));
            monoDebug = Convert.ToInt32(Assistant_Form.getCFG("debug"));
            try
            {

                //System.Timers.Timer aTimer = new System.Timers.Timer();
                //aTimer.Enabled = true;
                //aTimer.Elapsed += new ElapsedEventHandler(TimedEvent);
                //aTimer.Interval = timeDraw * 60 * 1000;    //配置文件中配置的时间（分钟）  
                //aTimer.Start();         

                // outnDrawASmallRectangleTime(SumRectangle,spa-ts,spa);
                spa = Convert.ToDateTime(drawStartTime);

                // do
                //  {
                // spa = spa + ts;
                dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                // ti++; ///throw new NotImplementedException();
                // } while (spa < System.DateTime.Now);

            }
            catch (Exception EX)
            {
                writeLog(EX.ToString(), "");
                // throw;
            }

        }

        

        private void TimedEvent(object source)
        {

            try
            {
                dataTimer.Change(Timeout.Infinite, -1);
                LineQuery(spa, spa + ts);


            }
            catch (Exception ex)
            {
                this.label1.Text = ex.ToString();
                writeLog(ex.ToString(), "TimedEvent异常");
                //throw;
            }
            finally
            {
                dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));

            }
        }
        public static DataTable recordTable;
        List<string> subList = new List<string>();
        private void Recording()
        {
            string filename = Environment.CurrentDirectory + "\\" + "录波器信息.xlsx";
            //string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source = " + filename + ";Extended Properties=Excel 8.0;";
            //using (OleDbDataAdapter myCommand = new OleDbDataAdapter("  select distinct 变电站 from [录波器信息$]  ", strConn))
            //{


            //    // OleDbDataAdapter myCommand = new OleDbDataAdapter("select *from  [录波器信息$] where 变电站 in(   select distinct 变电站 from [录波器信息$] )  ", strConn);
            //    DataSet myDataSet = new DataSet();
            //    myCommand.Fill(myDataSet, "Recording");
            //    recordTable = myDataSet.Tables[0];
            //    foreach (DataRow t in recordTable.Rows)
            //    {
            //        for (int i = 0; i < recordTable.Columns.Count; i++)
            //        {
            //            subList.Add(t[i].ToString());

            //        }
            //    }
            //}
            //CsvStreamReader cs = new CsvStreamReader(filename);
            recordTable = ExcelHelper.ImportExcel(filename, 0);
            foreach (DataRow rt in recordTable.Rows)
            {

                for (int i = 0; i < recordTable.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        subList.Add(rt[i].ToString());

                    }
                    else if (i > 0 && rt[i - 1] != rt[i])
                    {
                        subList.Add(rt[i].ToString());

                    }
                }
            }
            //foreach (DataRow it in recordTable.Rows)
            //{
            //    for (int i = 0; i < recordTable.Columns.Count; i++)
            //    {
            //       string a=  it[i].ToString();
            //        foreach ( DataColumn em in recordTable.Columns)
            //        {
            //            for (int j = 0; j < em.Table.Columns.Count; j++)
            //            {
            //                string b = em.Table.Columns[j].ToString();
            //                string c = em.Caption[j].ToString();
            //            }
            //        }
            //    }
            //}

            //throw new NotImplementedException();
        }

        private static void ConvertTime(FaultRecordService.time startTime, DateTime startTime1)
        {

            startTime.day = startTime1.Day;
            startTime.hour = startTime1.Hour;
            startTime.minute = startTime1.Minute;
            startTime.month = startTime1.Month;
            startTime.msecond = startTime1.Millisecond;
            startTime.second = startTime1.Second;
            startTime.year = startTime1.Year;

        }
        int j = 0;
        private Stopwatch stw = new Stopwatch();
        private void LineQuery(DateTime startTime1, DateTime endTime1)
        {
            stw.Start();

            int FRrtn = -1;
            string FRerror = "";

            try
            {
                录波采集测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();
                录波采集测试工具.FaultRecordService.time startTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
                录波采集测试工具.FaultRecordService.time endTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
                录波采集测试工具.FaultRecordService.faultRecordRtn FRfaultRecordRtn = new FaultRecordService.faultRecordRtn();


                writeLog("采集数据开始", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                try
                {
                    ConvertTime(startTime, startTime1);
                    ConvertTime(endTime, endTime1);
                }
                catch (Exception ex)
                {

                    writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }

               // writeLog("***time类型 startTime" + startTime.ToString() + "endtime" + endTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                if (j < subList.Count)
                {
                    writeLog(j+"行199当前使用的参数值变电站名："+ subList[j] +"时间：starttime：" + startTime.year + startTime.month + startTime.day + startTime.hour + startTime.minute + startTime.second + "endtime::" + endTime.year + endTime.month + endTime.day + endTime.hour + endTime.minute + endTime.second, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                    FRfaultRecordRtn = FRService.getFaultRecordByStationName(subList[j], startTime, endTime);
                    //FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text, startTime, endTime);
                    // FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);

                    //FRrtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).rtn;
                    //FRerror = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).error;
                    //FRfaultRecordRtn.faultRecords = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).faultRecords;

                    FRrtn = FRfaultRecordRtn.rtn;
                    FRerror = FRfaultRecordRtn.error;
                    writeLog("行211rtn值"+FRrtn.ToString() +"fr错误信息"+ FRerror.ToString()+j, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


                    //   writeLog("确定数组个数"+FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                    if (FRfaultRecordRtn.faultRecords!=null)
                    {
                        if (FRfaultRecordRtn.faultRecords.Length != 0)
                        {
                            for (int i = 0; i < FRfaultRecordRtn.faultRecords.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(FRfaultRecordRtn.faultRecords[i].id.ToString()))
                                {
                                    writeLog("faultRecords不为空，返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                                    if (FRfaultRecordRtn.faultRecords[i].recorderType==0)
                                    {//主变不处理
                                     //取编号进行下载文件
                                        RecordDownload(FRfaultRecordRtn.faultRecords[i].id, "_comtrade.cfg");
                                        RecordDownload(FRfaultRecordRtn.faultRecords[i].id, "_comtrade.dat");
                                        this.label1.Text = "工作中》》》》》》》";
                                        stw.Stop();
                                        writeLog("采集数据结束，耗时 " + stw.Elapsed.Minutes + "分 " + stw.Elapsed.Seconds + "秒" + stw.Elapsed.Milliseconds + "毫秒。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                                        stw = new Stopwatch();//需要重新计时？
                                        j++;

                                    }
                                    else
                                    {
                                        j++;
                                        writeLog("主","变");
                                        if (monoDebug==2)
                                        {
                                           
                                            dataTimer.Change(Timeout.Infinite, -1);
                                            LineQuery(spa, spa + ts);
                                            // dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                                            dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000)); 
                                        }
                                        else
                                        {
                                           
                                        }

                                    }
                                }
                                else
                                {
                                    j++;
                                    writeLog("faultRecordsID为空，返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                                    if (monoDebug == 2)
                                    {
                                        dataTimer.Change(Timeout.Infinite, -1);
                                        LineQuery(spa, spa + ts);
                                        // dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                                        dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                                    }
                                }
                            }
                        }
                        else
                        {
                            j++;
                            writeLog(j+"数组Length为空！变电站名:" + subList[j] + "时间：starttime：" + startTime.year + startTime.month + startTime.day + startTime.hour + startTime.minute + startTime.second + "endtime::" + endTime.year + endTime.month + endTime.day + endTime.hour + endTime.minute + endTime.second, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime
                            if (monoDebug == 2)
                            {
                                dataTimer.Change(Timeout.Infinite, -1);
                                LineQuery(spa, spa + ts);
                                // dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                                dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                            }
                            //j++;

                        }
                    }
                    else
                    {
                       
                        writeLog(j+ "FRfaultRecordRtn.faultRecords==null！变电站名:" + subList[j] + "时间：starttime：" + startTime.year+"/" + startTime.month + "/" + startTime.day + "/" + startTime.hour + "/" + startTime.minute + "/" + startTime.second + "endtime::" + endTime.year + "/" + endTime.month + "/" + endTime.day + "/" + endTime.hour + "/" + endTime.minute + "/" + endTime.second, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime
                        j++;
                        if (monoDebug==2)
                        {
                            dataTimer.Change(Timeout.Infinite, -1);
                            LineQuery(spa, spa + ts);
                            // dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                            dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000)); 
                        }
                    }

                }
                else if (j == subList.Count && spa < System.DateTime.Now)
                {
                    spa = spa + ts;
                    j = 0;
                    if (FRfaultRecordRtn.faultRecords != null)
                    {
                        // dataTimer.Dispose();
                        // writeLog(j+ "尾部释放！尝试new立即启动" + j, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime
                        writeLog(j+ "尾部！faultRecords不为空启动" + j, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime

                        dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));

                    }
                    else
                    {
                        if (monoDebug==2)
                        {

                            writeLog("数组尾部faultRecords为空！尝试立即启动" + j, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime
                            dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));

                            // dataTimer.Change(0, 0);
                            //dataTimer.Change(Timeout.Infinite, -1);
                            //LineQuery(spa, spa + ts);
                            //// dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                            //dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000)); 
                        }
                        else
                        {
                            dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            }
            // FRerror = FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text,,).error;
            //FaultRecordServiceService.getFaultRecordByDeviceOneName()
        }

        private void RecordDownload(long waveId, string WaveExtension)
        {
            //  waveId = Convert.ToInt64(tbxWaveId.Text);// Convert.ToUInt32(tbxWaveId);
            int rtn = -1;
            byte[] bytes = new byte[] { };

            string error = "";
            try
            {
                录波采集测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();

                //rtn = FRService.getFile(waveId, tbxWaveExtension.Text).rtn;
                //bytes = FRService.getFile(waveId, tbxWaveExtension.Text).bytes;

                //FileRtn fr = new FileRtn();
                //var fileRtn1 = FRService.getFile(waveId, tbxWaveExtension.Text);
                //fr = GetFileRtn1(fileRtn1);
                录波采集测试工具.FaultRecordService.fileRtn fr = new FaultRecordService.fileRtn();
                fr = FRService.getFile(waveId, WaveExtension);
                rtn = fr.rtn;

                if (rtn != -1)
                {
                    writeLog("返回wenjian** 返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                    bytes = fr.bytes;
                    error = fr.error;
                    writeLog("返回wenjian**" + bytes.Length.ToString() + "返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                    if (bytes.Length != 0)
                    {
                        // FileStream opBytes
                        waveDownload(bytes, waveId, WaveExtension);

                    };
                    //error = FRService.getFile(waveId, tbxWaveExtension.Text).error;
                    //  writeLog("返回rtn:" + rtn + "返回ByteS:" + bytes + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                    writeLog(j+"xiazai返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }
                else
                {
                    writeLog("可能是未找到文件rtn为空！尝试立即启动hou"+j, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));  // subList[j], startTime, endTime
                    j++;
                    if (monoDebug==2)
                    {
                        dataTimer.Change(Timeout.Infinite, -1);
                        LineQuery(spa, spa + ts);
                        dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000)); 
                    }
                   // dataTimer.Change(Timeout.Infinite, 0);
                    //dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, 0);

                    //  dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                    //dataTimer.Change(Timeout.Infinite, -1);
                    //LineQuery(spa, spa + ts);
                    //// dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                    //dataTimer.Change(Timeout.Infinite, 0);

                    //dataTimer.Change(0, 0);

                    // dataTimer.Change(0, 0);
                    //dataTimer.Change(Timeout.Infinite, -1);
                    //LineQuery(spa, spa + ts);
                    //// dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));
                    //dataTimer.Change(Timeout.Infinite, 0);
                }
            }
            catch (Exception ex)
            {
                writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                // throw;
            }
        }


        private void waveDownload(byte[] bytes, long waveId, string Extension)
        {
            string filename = Environment.CurrentDirectory + "\\" + waveId + Extension;
            writeLog(filename, "文件下载中>>>");
            //if (tbxWaveExtension.Text.Contains("m14"))
            //{
            //    //Stream s = File.Open(filename, FileMode.Create);
            //    ////BinaryFormatter b = new BinaryFormatter();
            //    ////  b.Serialize(s, bytes);
            //    //BinaryWriter b = new BinaryWriter(s);
            //    writeLog(filename, "14");

            //    File.WriteAllBytes(filename, bytes);

            //    //s.Close();


            //}
            //else if (tbxWaveExtension.Text.Contains("m13"))
            //{
            //    writeLog(filename, "13");

            //    FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
            //    BinaryWriter bi = new BinaryWriter(fs);
            //    bi.Write(bytes);
            //    bi.Close();
            //    fs.Close();
            //}
            //// FileStream fs = new FileStream(Environment.CurrentDirectory + "\\" + tbxWaveId.Text +"a1"+ tbxWaveExtension.Text, FileMode.Create);
            //else
            //{
            File.WriteAllBytes(filename, bytes);

            //}



            //FileStream fileStream = File.Create(filename);
            //int bufLen = 8196;
            //byte[] buf = new byte[bufLen];
            //int bytesRead = 1;

            //while (bytesRead != 0)
            //{
            //    bytesRead = fileStream.Read(bytes, 0, buf.Length);
            //    fileStream.Write(bytes, 0, buf.Length);
            //}

            //fileStream.Close();

        }
        private void writeLog(string log, string logtime)
        {
            try
            {
                // this.listBox1.Items.Add(log + "  " + logtime);
                string des = Assembly.GetExecutingAssembly().Location;
                des = des.Substring(0, des.LastIndexOf(@"\")) + "\\Log";
                if (!Directory.Exists(des))
                {
                    Directory.CreateDirectory(des);
                }
                string filename = des + "\\leidian_log " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                FileStream myFileStream = new FileStream(filename, FileMode.Append);
                StreamWriter sw = new StreamWriter(myFileStream, System.Text.Encoding.Default);
                sw.WriteLine(log + "  " + logtime);
                myFileStream.Flush();
                sw.Close();
                myFileStream.Close();
            }
            catch (System.Exception ex)
            {
                this.label1.Text = ex.Message.ToString() + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");

            }
        }
    }
}
