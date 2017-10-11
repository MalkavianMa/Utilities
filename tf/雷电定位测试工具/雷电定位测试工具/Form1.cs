using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Hisingpower.DButility;
using System.Data.SqlClient;

namespace 雷电定位测试工具
{
    public partial class Form1 : Form
    {
        public string strLeftLocation = "";
        public string strRightLocation = "";
        public string xiaojuxingbanjing = "";
        public int timeDraw = 10;
        public string startdraw = "";
        public string drawStartTime = "";
        public string drawEndTime = "";
        List<String> SumRectangle = new List<string>();
        public int monoDebug = 0;

        string _Connstr;


        public string Connstr
        {
            get
            {
                return Assistant_Form.getCFG("sqlConnection");//"Data Source=.;Initial Catalog=test;Integrated Security=true;";
                //return Hisingpower.DESEncrypt.Decrypt(Common.getCFG("ConnectString"));
            }
            set { _Connstr = value; }
        }
        //  public string
        public Form1()
        {
            InitializeComponent();
            writeLog("leidian服务启动。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


        }


        private void btnDrawASmallRectangleTime()
        {
            // if (!string.IsNullOrEmpty(DrawLargeRectangle))
            {
                string RectangleStart, RectangleStartTwo, RectangleStartThree, RectangleStop = "";
                float smallR;
                RectangleStart = startdraw; // = DrawLargeRectangle.MinLng.ToString() + "_" + DrawLargeRectangle.MaxLat.ToString();//第一个点 最小经度 最大维度
                RectangleStartTwo = strRightLocation;// = DrawLargeRectangle.MaxLng.ToString() + "_" + DrawLargeRectangle.MaxLat.ToString();//最大
                RectangleStartThree = strLeftLocation;// = DrawLargeRectangle.MinLng.ToString() + "_" + DrawLargeRectangle.MinLat.ToString();//最小
                RectangleStop = "";//未使用 //= DrawLargeRectangle.MaxLng.ToString() + "_" + DrawLargeRectangle.MinLat.ToString();//最大经度 最小维度

                mark = 0;
                markLng = 0;
                smallR = Convert.ToSingle(xiaojuxingbanjing);//= Convert.ToSingle(tbxxiaojuxing.Text);
                                                             //  DrawASmallTime(RectangleStart, RectangleStop, DrawLargeRectangle.MaxLng, DrawLargeRectangle.MinLat, smallR);
                DrawASmallTime(RectangleStart, RectangleStop, Convert.ToDouble(RectangleStartTwo.Split('_')[0]), Convert.ToDouble(RectangleStartThree.Split('_')[1]), smallR);

            }
        }
        public DateTime spa;
        TimeSpan ts = new TimeSpan(1, 0, 0, 0);
        System.Threading.Timer dataTimer;

        private void Form1_Load(object sender, EventArgs e)
        {
            monoDebug = Convert.ToInt32(Assistant_Form.getCFG("debug"));

            label1.Text = "工作中》》》";
            strLeftLocation = Assistant_Form.getCFG("dajuxingzuoxia");//雷电定位测试工具.LDService.GetFlashReadService LDService = new LDService.GetFlashReadService();
            strRightLocation = Assistant_Form.getCFG("dajuxingyoushang");
            xiaojuxingbanjing = Assistant_Form.getCFG("xiaojuxingbanjing");
            timeDraw = Convert.ToInt32(Assistant_Form.getCFG("caijijiange"));
            startdraw = Assistant_Form.getCFG("startdraw");
            drawStartTime = Assistant_Form.getCFG("drawStartTime");
            drawEndTime = Assistant_Form.getCFG("drawEndTime");
            writeLog("大巨星左下经纬度：" + strLeftLocation + " 大巨星you下经纬度:" + strRightLocation + "小矩形半径：" + xiaojuxingbanjing + "（公里）" + "采集时间间隔：" + timeDraw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            btnDrawASmallRectangleTime();
            writeLog("系统当前时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

            // string middrawEndTime;
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
            writeLog("T跳过数记录总记录", dinsT.ToString());

            //=  Convert.ToDateTime(drawStartTime)
            // outnDrawASmallRectangleTime(SumRectangle);
            //雷电定位测试工具.FaultRecordService.FaultRecordServiceService FaultRecordServiceService = new FaultRecordService.FaultRecordServiceService();
            //FaultRecordServiceService.getFaultRecordByDeviceOneName()
        }
        public int ti = 0;
        private void outnDrawASmallRectangleTime(List<string> SumRectangle, DateTime st, DateTime et)
        {
            string stime, etime = "";
            if (ti < SumRectangle.Count) //  for (int i=0; i < SumRectangle.Count; i++ )
            {

                //  writeLeidian(SumRectangle[i], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                string lf, rl = "";
                lf = SumRectangle[ti].Split('|')[0];
                rl = SumRectangle[ti].Split('|')[1];
                stime = st.ToString("yyyy-MM-dd HH:mm:ss");
                etime = et.ToString("yyyy-MM-dd HH:mm:ss");
                leidianCaiji(lf, rl, stime, etime);

                ti++;
                /// Thread.Sleep(timeDraw * 60 * 1000);//1秒
                if (monoDebug==2)
                {
                    if (innum > Convert.ToInt32(Assistant_Form.getCFG("innum")))
                    //  if (true)

                    {
                        dinsT++;
                        // if (dinsT<100)
                        {
                          
                            writeLog("重复记录总记录", innum.ToString());
                            if (ti+1!= SumRectangle.Count)
                            {
                                innum = 0;

                            }
                            else
                            {

                            }
                            //  dataTimer.Change(Timeout.Infinite, -1);
                            outnDrawASmallRectangleTime(SumRectangle, spa, spa + ts);
                            //  dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                         //   dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));

                        }

                    } 
                }
                if (monoDebug == 1)
                {
                    if (innum > Convert.ToInt32(Assistant_Form.getCFG("innum")))
                        //  if (true)

                    {
                        dinsT++;
                        // if (dinsT<100)
                        {
                            writeLog("重复记录总记录", innum.ToString()); innum = 0;
                            // dataTimer.Change(Timeout.Infinite, -1);
                            outnDrawASmallRectangleTime(SumRectangle, spa, spa + ts);
                            //  dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                           // dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));

                        }

                    }
                }

            }
            else if (ti == SumRectangle.Count && spa < System.DateTime.Now)
            {
                spa = spa + ts;
                ti = 0;
                if (monoDebug == 2)
                {
                   if (innum > Convert.ToInt32(Assistant_Form.getCFG("innum")))
                    //  if (true)

                    {
                        dinsT++;
                        // if (dinsT<100)
                      
                            //innum = 0;
                            writeLog("矩阵数组尾部", innum.ToString());
                            //dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 5 * 60 * 1000, (timeDraw * 60 * 1000));

                            //  dataTimer.Change(Timeout.Infinite, -1);
                           outnDrawASmallRectangleTime(SumRectangle, spa, spa + ts);
                            //  dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 0, (timeDraw * 60 * 1000));
                            //   dataTimer.Change((timeDraw * 60 * 1000), (timeDraw * 60 * 1000));

                      

                    }
                    else
                    {
                        dataTimer = new System.Threading.Timer(new TimerCallback(TimedEvent), null, 5 * 60 * 1000, (timeDraw * 60 * 1000));

                    }


                }
                else
                {
                    dataTimer= new System.Threading.Timer(new TimerCallback(TimedEvent), null, 5*60 * 1000, (timeDraw * 60 * 1000));
                }
            }

            // throw new NotImplementedException();
        }


       

        private void TimedEvent(object source)
        {

            try
            {
                dataTimer.Change(Timeout.Infinite, -1);
                outnDrawASmallRectangleTime(SumRectangle, spa, spa + ts);


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

        private void DrawASmallTime(string RectangleStart, string RectangleStop, double RectangleStartTwoLng, double RectangleStartThreeLat, float r)
        {
            double btnLng;
            double btLat;
            //小矩形暂定2km的长和宽//中心点变为左上,依次推左下 和右上//实际画取2kM的正方形
            double poWave = Convert.ToDouble(RectangleStart.Split('_')[0]);
            double pwWave = Convert.ToDouble(RectangleStart.Split('_')[1]);
            //  float prWave = 2;

            double pwtWave = Convert.ToDouble(RectangleStart.Split('_')[1]);

            //btnLng = btm.MaxLng;

            // for (; btnLng < RectangleStartTwoLng; DrawASmall(RectangleStart, "", RectangleStartTwoLng, RectangleStartThreeLat))//经度缩减 纬度不变
            for (double j = poWave; j <= RectangleStartTwoLng; j = DrawASmallLngTime(poWave, pwtWave, RectangleStartTwoLng, false, r).MaxLng)
            {
                pwWave = pwtWave;
                // PositionModel bta = new 雷电定位测试工具.PositionModel();
                // bta = DistanceHelper.FindNeighPosition(poWave, pwWave, prWave);
                //writeLog("返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + pwWave, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                //  RectangleStart = btm.MaxLng.ToString() + "_" + pwWave.ToString();
                if (j != poWave)
                {
                    poWave = DrawASmallLngTime(j, pwtWave, RectangleStartTwoLng, false, r).MaxLng;

                }


                for (double i = DrawASmallLatTime(poWave, pwWave, RectangleStartThreeLat, true, r).MinLat; RectangleStartThreeLat < i; i = DrawASmallLatTime(poWave, pwWave, RectangleStartThreeLat, false, r).MinLat)////--纬度追加递减
                {
                    // PositionModel btnLat=new 雷电定位测试工具.PositionModel ();
                    // btnLat = DrawASmallLat(poWave, btm.MinLat, RectangleStartThreeLat):

                    pwWave = DrawASmallLatTime(poWave, i, RectangleStartThreeLat, true, r).MinLat;//少画了两个最初的小矩形//bta.MinLat//work out
                }                //DrawASmall(RectangleStart, "", RectangleStartTwoLng);

            }
        }

        private PositionModel DrawASmallLngTime(double poWave, double twolat, double RectangleStartThreeLat, bool p, float r)
        {
            float prWave = r;//暂定2km
            PositionModel btm = new 雷电定位测试工具.PositionModel();
            btm = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
            // btnLat = btm.MinLat;
            if (p)//判断仅是自减纬度 还是需要输出小矩形 0为自减
            {
                // btnLng = btm.MaxLng;//纬度递减引起的经度误差 可利用Mark
                markLng++;
                if (markLng > 1)
                {
                    PositionModel btI = new 雷电定位测试工具.PositionModel();
                    btI = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
                    SumRectangle.Add(poWave + "_" + btI.MinLat + "|" + btI.MaxLng + "_" + twolat + "|" + drawStartTime + "|" + drawEndTime);
                    // leidianCaiji(poWave+"_"+btI.MinLat,btI.MaxLng+"_"+twolat,drawStartTime,drawEndTime);
                    // writeLog(markLng + "返回小矩形的左下经度" + poWave + "纬度" + btI.MinLat + "右上经度:" + btI.MaxLng + "右上纬度" + twolat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }
                else
                {
                    // PositionModel btm = new 雷电定位测试工具.PositionModel();
                    btm = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
                    // markLng = btm.MaxLng;
                    // writeLog(markLng + "返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + twolat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                    // leidianCaiji(poWave + "_" + btm.MinLat, btm.MaxLng + "_" + twolat, drawStartTime, drawEndTime);
                    SumRectangle.Add(poWave + "_" + btm.MinLat + "|" + btm.MaxLng + "_" + twolat + "|" + drawStartTime + "|" + drawEndTime);
                }
                // throw new NotImplementedException();
            }
            return btm;

        }
        private PositionModel DrawASmallLatTime(double poWave, double minLat, double rectangleStartThreeLat, Boolean w, float r)
        {
            double btnLng;
            float prWave = r;//暂定2km
            PositionModel btm = new 雷电定位测试工具.PositionModel();
            btm = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
            // btnLat = btm.MinLat;
            if (w)//判断仅是自减纬度 还是需要输出小矩形 0为自减
            {
                // btnLng = btm.MaxLng;//纬度递减引起的经度误差 可利用Mark
                mark++;
                if (mark > 1)
                {
                    PositionModel btI = new 雷电定位测试工具.PositionModel();
                    btI = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
                    //  writeLog(mark + "返回小矩形的左下经度" + poWave + "纬度" + btI.MinLat + "右上经度:" + markLat + "右上纬度" + minLat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                    //leidianCaiji(poWave + "_" + btI.MinLat, markLat + "_" + minLat, drawStartTime, drawEndTime);
                    SumRectangle.Add(poWave + "_" + btI.MinLat + "|" + markLat + "_" + minLat + "|" + drawStartTime + "|" + drawEndTime);
                }
                else
                {
                    // PositionModel btm = new 雷电定位测试工具.PositionModel();
                    btm = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
                    markLat = btm.MaxLng;
                    // writeLog(mark + "返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + minLat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                    //leidianCaiji(poWave + "_" + btm.MinLat, btm.MaxLng + "_" + minLat, drawStartTime, drawEndTime);
                    SumRectangle.Add(poWave + "_" + btm.MinLat + "|" + btm.MaxLng + "_" + minLat + "|" + drawStartTime + "|" + drawEndTime);

                }

            }

            return btm;
            // throw new NotImplementedException();
        }

        private void writeLeidian(string log, string logtime)
        {
            try
            {
                //this.listBox1.Items.Add(log + "  " + logtime);
                string des = Assembly.GetExecutingAssembly().Location;
                des = des.Substring(0, des.LastIndexOf(@"\")) + "\\Leidian";
                if (!Directory.Exists(des))
                {
                    Directory.CreateDirectory(des);
                }
                string filename = des + "\\leidian_result" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                FileStream myFileStream = new FileStream(filename, FileMode.Append);
                StreamWriter sw = new StreamWriter(myFileStream, System.Text.Encoding.Default);
                sw.WriteLine(log + "  ");
                myFileStream.Flush();
                sw.Close();
                myFileStream.Close();
            }
            catch (System.Exception ex)
            {
                // this.listBox1.Items.Add(ex.Message.ToString() + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            }
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
                //  this.listBox1.Items.Add(ex.Message.ToString() + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            }
        }
        int innum=0;
        int dinsT = 0;

        private Stopwatch stw = new Stopwatch();
        private void leidianCaiji(string lf, string rl, string stime, string etime)
        {
            stw.Start();
            writeLog("采集数据开始leftLocation =" + lf + "rightLocation=" + rl + "startTime =" + stime + "endTime =" + etime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

            雷电定位测试工具.LDService.GetFlashReadService LDService = new LDService.GetFlashReadService();
            XmlDocument doc = new XmlDocument();
            string result = "";
            // string getFlash = "";

            if (true)
            {
               //result = LDService.getFlashByRect(tbxleftLocation.Text, tbxRightLocation.Text, tbxstarttime.Text, tbxEndtime.Text, tbxStartline.Text, tbxLineCount.Text);

                // getFlash =  LDService.getFlashByRect(tbxleftLocation.Text, tbxRightLocation.Text, tbxstarttime.Text, tbxEndtime.Text, tbxStartline.Text, tbxLineCount.Text);
                // XmlNode root = doc.DocumentElement;
                // doc.LoadXml(getFlash);
                try
                {
                    // 雷电定位测试工具.QueryService queryservice = new 雷电定位测试工具.QueryService.QueryService();

                    //doc.LoadXml(LDService.getFlashByRect(lf, rl, stime, etime, 1.ToString(), 2000.ToString()));
                    // string path = @"C:\Users\Administrator.WIN7-1607281528\Desktop\leidian_result2017-08-32.txt";

                    // doc.Load(path);
                    //  result = doc.InnerXml.ToString();
                    if (monoDebug==1)
                    {
                        result = "<?xml version='1.0' encoding='UTF - 8'?><Tables><Table name='矩形查询结果' count='27'><Columns>序号|时间|微秒|经度|纬度|电流(kA)|回击</Columns><Datas>1|2016-12-07 19:20:16|9399855|120.928096|36.923691|-16.6|1</Datas><Datas>2|2016-12-07 19:22:05|3456858|121.690354|36.874247|48.8|1</Datas><Datas>3|2016-12-07 19:28:07|7048159|119.100012|36.762637|-63.9|1</Datas><Datas>4|2016-12-07 19:30:26|7943446|122.303961|36.830026|62.9|1</Datas><Datas>5|2016-12-07 19:36:39|4182358|120.562444|37.032474|-20.1|1</Datas><Datas>6|2016-12-07 20:06:37|6180188|122.400095|36.754604|49.9|1</Datas><Datas>7|2016-12-07 20:35:04|9711737|121.451268|36.899026|-12.4|1</Datas><Datas>8|2016-12-07 20:39:21|1918709|120.847130|36.929992|-7.5|1</Datas><Datas>9|2016-12-07 20:45:08|1577671|119.091878|36.903076|-34.6|1</Datas><Datas>10|2016-12-07 20:57:45|7215049|121.058645|36.918823|-9.2|1</Datas><Datas>11|2016-12-07 22:07:35|7323181|122.246206|37.176931|-62.1|1</Datas><Datas>12|2016-12-07 22:32:20|8278012|120.795714|36.982487|-8.0|1</Datas><Datas>13|2016-12-07 23:05:24|6314655|122.390078|36.636743|-53.4|1</Datas><Datas>14|2016-12-07 23:38:49|4809736|120.534559|37.402409|-9.6|1</Datas><Datas>15|2016-12-07 23:48:33|2236136|118.107639|35.340633|-20.1|1</Datas><Datas>16|2016-12-08 13:29:09|6191841|122.551891|37.369738|19.6|1</Datas><Datas>17|2016-12-08 16:10:21|557222|122.499007|37.395018|-7.2|1</Datas><Datas>18|2016-12-08 16:10:39|2216033|122.642962|38.108956|22.0|1</Datas><Datas>19|2016-12-08 16:12:35|2493331|122.571761|37.460018|-6.7|1</Datas><Datas>20|2016-12-08 17:04:42|6535936|122.075501|37.201688|-15.7|1</Datas><Datas>21|2016-12-08 17:27:17|7229368|122.706690|37.681092|13.5|1</Datas><Datas>22|2016-12-08 17:28:38|8908955|119.356806|34.524748|17.3|1</Datas><Datas>23|2016-12-08 18:00:09|8959002|120.529773|37.958614|-15.3|1</Datas><Datas>24|2016-12-08 18:32:34|8402634|120.503744|36.317449|-43.6|1</Datas><Datas>25|2016-12-08 18:44:48|6627527|122.605034|37.481650|17.2|1</Datas><Datas>26|2016-12-08 19:14:00|2903033|122.704036|37.583866|-12.6|1</Datas><Datas>27|2016-12-08 19:18:03|750511|122.670722|37.125614|-52.2|1</Datas></Table></Tables> ";

                    }
                    else if (Convert.ToInt32(Assistant_Form.getCFG("isTXT"))==1)
                    {
                        result = "<?xml version='1.0' encoding='UTF - 8'?><Tables><Table name='矩形查询结果' count='27'><Columns>序号|时间|微秒|经度|纬度|电流(kA)|回击</Columns><Datas>1|2016-12-07 19:20:16|9399855|120.928096|36.923691|-16.6|1</Datas><Datas>2|2016-12-07 19:22:05|3456858|121.690354|36.874247|48.8|1</Datas><Datas>3|2016-12-07 19:28:07|7048159|119.100012|36.762637|-63.9|1</Datas><Datas>4|2016-12-07 19:30:26|7943446|122.303961|36.830026|62.9|1</Datas><Datas>5|2016-12-07 19:36:39|4182358|120.562444|37.032474|-20.1|1</Datas><Datas>6|2016-12-07 20:06:37|6180188|122.400095|36.754604|49.9|1</Datas><Datas>7|2016-12-07 20:35:04|9711737|121.451268|36.899026|-12.4|1</Datas><Datas>8|2016-12-07 20:39:21|1918709|120.847130|36.929992|-7.5|1</Datas><Datas>9|2016-12-07 20:45:08|1577671|119.091878|36.903076|-34.6|1</Datas><Datas>10|2016-12-07 20:57:45|7215049|121.058645|36.918823|-9.2|1</Datas><Datas>11|2016-12-07 22:07:35|7323181|122.246206|37.176931|-62.1|1</Datas><Datas>12|2016-12-07 22:32:20|8278012|120.795714|36.982487|-8.0|1</Datas><Datas>13|2016-12-07 23:05:24|6314655|122.390078|36.636743|-53.4|1</Datas><Datas>14|2016-12-07 23:38:49|4809736|120.534559|37.402409|-9.6|1</Datas><Datas>15|2016-12-07 23:48:33|2236136|118.107639|35.340633|-20.1|1</Datas><Datas>16|2016-12-08 13:29:09|6191841|122.551891|37.369738|19.6|1</Datas><Datas>17|2016-12-08 16:10:21|557222|122.499007|37.395018|-7.2|1</Datas><Datas>18|2016-12-08 16:10:39|2216033|122.642962|38.108956|22.0|1</Datas><Datas>19|2016-12-08 16:12:35|2493331|122.571761|37.460018|-6.7|1</Datas><Datas>20|2016-12-08 17:04:42|6535936|122.075501|37.201688|-15.7|1</Datas><Datas>21|2016-12-08 17:27:17|7229368|122.706690|37.681092|13.5|1</Datas><Datas>22|2016-12-08 17:28:38|8908955|119.356806|34.524748|17.3|1</Datas><Datas>23|2016-12-08 18:00:09|8959002|120.529773|37.958614|-15.3|1</Datas><Datas>24|2016-12-08 18:32:34|8402634|120.503744|36.317449|-43.6|1</Datas><Datas>25|2016-12-08 18:44:48|6627527|122.605034|37.481650|17.2|1</Datas><Datas>26|2016-12-08 19:14:00|2903033|122.704036|37.583866|-12.6|1</Datas><Datas>27|2016-12-08 19:18:03|750511|122.670722|37.125614|-52.2|1</Datas></Table></Tables> ";
                    }
                    else
                    {
                        result = LDService.getFlashByRect(lf, rl, stime, etime, 1.ToString(), 2000.ToString());
                    }
                    InsertList(ResultList(result));
                    writeLeidian(result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                  
                   // innum = 0;
                    // ResultXml();
                    // SumRectangle.Add(result);
                }
                catch (Exception ex)
                {
                    writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                }
            }

            //   private Stopwatch stw = new Stopwatch();
            stw.Stop();
            writeLog("采集数据结束，耗时 " + stw.Elapsed.Minutes + "分 " + stw.Elapsed.Seconds + "秒" + stw.Elapsed.Milliseconds + "毫秒。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        }

        private void InsertList(List<string> toke)
        {
            string ldTime;
            DateTime saveTime, ldTime2;
            int ldMillisecond;
            //DateTime ldTime = new DateTime(year,month,day,hour,f);
            double ldLng, ldLat, tdCurrent;
            int isback;
            for (int i = 0; i < toke.Count; i++)
            {
                ldTime = toke[i].Split('|')[1]; //+ toke[i].Split('|')[2];
                ldTime2 = Convert.ToDateTime(toke[i].Split('|')[1]);// + toke[i].Split('|')[2]);
                ldMillisecond = Convert.ToInt32(toke[i].Split('|')[2]);
                ldLng = Convert.ToDouble(toke[i].Split('|')[3]);
                ldLat = Convert.ToDouble(toke[i].Split('|')[4]);
                tdCurrent = Convert.ToDouble(toke[i].Split('|')[5]);
                isback = Convert.ToInt32(toke[i].Split('|')[6]);

                //int j = i + 1;
                //  string a = toke[i].Substring(j.ToString().Length + 1, 27).Trim('|');
                //  ///string ijtime = toke[i].Substring(j.ToString().Length + 1, 27).Replace("|", ".");
                //  //  ldTime = toke[i].Substring(j.ToString().Length + 1, 27).Replace("|", "");
                //string ijtime = toke[i].Substring(a.IndexOf("|"), 27).Replace("|", ".");

                //  ldTime = Convert.ToDateTime(ijtime).ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                //  //yyyy-MM-dd HH:mm:ss
                //  //yyyy-MM-dd HH:mm:ss.ffffff
                //  ldLng = Convert.ToSingle(toke[i].Substring(a.IndexOf("|") + 28, 10));
                //  ldLat = Convert.ToSingle(a.IndexOf("|") + 38,9);

                saveTime = DateTime.Now;
               string sql = "SELECT * FROM[test].[dbo].[ThunderDatasHistory] where time ='"+ ldTime + "'and millisecond="+ldMillisecond+ "and lng =" + ldLng + "  and lat =" + ldLat + "  and TDcurrent=" + tdCurrent + " and [isBack]=" + isback;
                //string sql = "SELECT[id],[time] ,[lng]  ,[lat] ,[TDcurrent] ,[isBack]  ,[saveTime] FROM[test].[dbo].[ThunderDatasHistory] where time ='" + ldTime2.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'and millisecond=" + ldMillisecond + "and lng =" + ldLng + "  and lat =" + ldLat + "  and TDcurrent=" + tdCurrent + " and [isBack]=" + isback;

                try
                {
                    using (SqlDataReader sdr = SqlHelper.ExecuteDataReader(Connstr, sql, null))
                    {
                        if (sdr.HasRows)
                        {
                            writeLog("已存在", sql);

                            while (sdr.Read())
                            {
                                innum++;

                            }

                        }
                        else
                        {
                            string sql1 = "INSERT INTO [test].[dbo].[ThunderDatasHistory] ([time],[millisecond],[lng]  ,[lat] ,[TDcurrent] ,[isBack] ,[saveTime])VALUES('" + ldTime + "'," + ldMillisecond + "," + ldLng + "," + ldLat + "," + tdCurrent + "," + isback + ",'" + saveTime + "')";

                            if (SqlHelper.ExecuteNonQuery(Connstr, sql1, null) > 0)
                            {
                                writeLog("添加成功", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                            }
                            else
                            {
                                writeLog("添加失败", sql1);

                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                    writeLog("sql", ex.ToString());
                   
                }
               
            }
            //throw new NotImplementedException();
        }
        private static List<string> ResultList(string result)
        {
            List<string> toke = new List<string>();
            //DateTime ldTime, saveTime;
            //float ldLng, ldLat, tdCurrent;
            //int isback;
            string token = "";
            //string path = @"C:\Users\Administrator.WIN7-1607281528\Desktop\test.xml";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            //doc.Load(path);
            XmlNode root = doc.DocumentElement;

            XmlNodeList node = doc.GetElementsByTagName("Table");

            for (int i = 0; i < node.Count; i++)
            {
                foreach (XmlNode nodechild in node[i].ChildNodes)
                {
                    if (nodechild.Name == "Datas")
                    {
                        token = nodechild.InnerText;
                        toke.Add(token);

                        //ldTime=token.
                    }
                }
            }
            return toke;
            //  writeLog("token=" + token, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            // return token;// throw new NotImplementedException();
        }
        private void ResultXml()
        {
            throw new NotImplementedException();
        }

        //private void btntest_Click(object sender, EventArgs e)
        //{
        //    stw.Start();
        //    writeLog("采集数据开始leftLocation =" + tbxleftLocation.Text + "rightLocation=" + tbxRightLocation.Text + "startTime =" + tbxstarttime.Text + "endTime =" + tbxEndtime.Text, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //    雷电定位测试工具.LDService.GetFlashReadService LDService = new LDService.GetFlashReadService();
        //    XmlDocument doc = new XmlDocument();
        //    string result = "";
        //    // string getFlash = "";

        //    if (true)
        //    {
        //        //result = LDService.getFlashByRect(tbxleftLocation.Text, tbxRightLocation.Text, tbxstarttime.Text, tbxEndtime.Text, tbxStartline.Text, tbxLineCount.Text);

        //        // getFlash =  LDService.getFlashByRect(tbxleftLocation.Text, tbxRightLocation.Text, tbxstarttime.Text, tbxEndtime.Text, tbxStartline.Text, tbxLineCount.Text);
        //        // XmlNode root = doc.DocumentElement;
        //        // doc.LoadXml(getFlash);
        //        try
        //        {
        //            // 雷电定位测试工具.QueryService queryservice = new 雷电定位测试工具.QueryService.QueryService();

        //            doc.LoadXml(LDService.getFlashByRect(tbxleftLocation.Text, tbxRightLocation.Text, tbxstarttime.Text, tbxEndtime.Text, tbxStartline.Text, tbxLineCount.Text));
        //            result = doc.InnerXml.ToString();

        //            writeLog("返回" + result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //        }
        //        catch (Exception ex)
        //        {
        //            writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //        }
        //    }

        //    //   private Stopwatch stw = new Stopwatch();
        //    stw.Stop();
        //    writeLog("采集数据结束，耗时 " + stw.Elapsed.Minutes + "分 " + stw.Elapsed.Seconds + "秒" + stw.Elapsed.Milliseconds + "毫秒。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));




        //}

        //private void btnLineQuery_Click(object sender, EventArgs e)
        //{
        //    int FRrtn = -1;
        //    string FRerror = "";

        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();
        //        雷电定位测试工具.FaultRecordService.time startTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.time endTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.faultRecordRtn FRfaultRecordRtn = new FaultRecordService.faultRecordRtn();

        //        DateTime startTime1 = Convert.ToDateTime(tbxWaveStartTime.Text);
        //        DateTime endTime1 = Convert.ToDateTime(tbxWaveEndTime.Text);
        //        writeLog("datetime类型 startTime1" + startTime1.ToString() + "endTime1" + endTime1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        try
        //        {
        //            ConvertTime(startTime, startTime1);
        //            ConvertTime(endTime, endTime1);
        //        }
        //        catch (Exception ex)
        //        {

        //            writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }

        //        writeLog("***time类型 startTime" + startTime.ToString() + "endtime" + endTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        FRfaultRecordRtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);
        //        //FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text, startTime, endTime);
        //        // FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);

        //        //FRrtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).rtn;
        //        //FRerror = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).error;
        //        //FRfaultRecordRtn.faultRecords = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).faultRecords;

        //        FRrtn = FRfaultRecordRtn.rtn;
        //        FRerror = FRfaultRecordRtn.error;
        //        writeLog(FRrtn.ToString() + FRerror.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


        //        writeLog(FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        for (int i = 0; i < FRfaultRecordRtn.faultRecords.Length; i++)
        //        {
        //            writeLog("返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //    // FRerror = FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text,,).error;
        //    //FaultRecordServiceService.getFaultRecordByDeviceOneName()
        //}

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

        //private void btnWave_Click(object sender, EventArgs e)
        //{
        //    long waveId = Convert.ToInt64(tbxWaveId.Text);// Convert.ToUInt32(tbxWaveId);
        //    int rtn = -1;
        //    byte[] bytes = new byte[] { };
        //    string error = "";
        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();

        //        //rtn = FRService.getFile(waveId, tbxWaveExtension.Text).rtn;
        //        //bytes = FRService.getFile(waveId, tbxWaveExtension.Text).bytes;

        //        //FileRtn fr = new FileRtn();
        //        //var fileRtn1 = FRService.getFile(waveId, tbxWaveExtension.Text);
        //        //fr = GetFileRtn1(fileRtn1);
        //        雷电定位测试工具.FaultRecordService.fileRtn fr = new FaultRecordService.fileRtn();
        //        fr = FRService.getFile(waveId, tbxWaveExtension.Text);
        //        rtn = fr.rtn;
        //        writeLog("返回wenjian** 返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        bytes = fr.bytes;
        //        error = fr.error;
        //        writeLog("返回wenjian**" + bytes.Length.ToString() + "返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        if (bytes.Length != 0)
        //        {
        //            // FileStream opBytes
        //         // waveDownload(bytes);
        //            waveDownload1(bytes);
        //        };
        //        //error = FRService.getFile(waveId, tbxWaveExtension.Text).error;
        //        //  writeLog("返回rtn:" + rtn + "返回ByteS:" + bytes + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        writeLog("返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //    catch (Exception ex)
        //    {
        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        // throw;
        //    }


        //}

        //private void waveDownload1(byte[] bytes)
        //{
        //    //string des = Assembly.GetExecutingAssembly().Location;
        //    //des = des.Substring(0, des.LastIndexOf(@"\")) + "\\Log";
        //    //if (!Directory.Exists(des))
        //    //{
        //    //    Directory.CreateDirectory(des);
        //    //}
        //    //string filename = des + "\\download " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")+"oo" + tbxWaveExtension.Text;
        //    ////File.WriteAllBytes(filename, bytes);

        //    string filename = Environment.CurrentDirectory+ "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffffff")+ tbxWaveExtension.Text; ;
        //    writeLog(filename, "11");
        //    FileStream fileStream = File.Create(filename);
        //    int bufLen = 8196;
        //    byte[] buf = new byte[bufLen];
        //    int bytesRead = 1;

        //    while (bytesRead != 0)
        //    {
        //        bytesRead = fileStream.Read(bytes, 0, buf.Length);
        //        fileStream.Write(bytes, 0, buf.Length);
        //    }

        //    fileStream.Close();
        //}

        //private void waveDownload(byte[] bytes)
        //{
        //    string filename = Environment.CurrentDirectory + "\\" +tbxWaveId.Text + tbxWaveExtension.Text; 
        //    writeLog(filename, "12");
        //    if (tbxWaveExtension.Text.Contains("m14"))
        //    {
        //        //Stream s = File.Open(filename, FileMode.Create);
        //        ////BinaryFormatter b = new BinaryFormatter();
        //        ////  b.Serialize(s, bytes);
        //        //BinaryWriter b = new BinaryWriter(s);
        //        writeLog(filename, "14");

        //        File.WriteAllBytes(filename, bytes);

        //        //s.Close();


        //    }
        //    else if(tbxWaveExtension.Text.Contains("m13"))
        //    {
        //        writeLog(filename, "13");

        //        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
        //        BinaryWriter bi = new BinaryWriter(fs);
        //        bi.Write(bytes);
        //        bi.Close();
        //        fs.Close();
        //    }
        //    // FileStream fs = new FileStream(Environment.CurrentDirectory + "\\" + tbxWaveId.Text +"a1"+ tbxWaveExtension.Text, FileMode.Create);
        //    else
        //    {
        //        File.WriteAllBytes(filename, bytes);

        //    }



        //    //FileStream fileStream = File.Create(filename);
        //    //int bufLen = 8196;
        //    //byte[] buf = new byte[bufLen];
        //    //int bytesRead = 1;

        //    //while (bytesRead != 0)
        //    //{
        //    //    bytesRead = fileStream.Read(bytes, 0, buf.Length);
        //    //    fileStream.Write(bytes, 0, buf.Length);
        //    //}

        //    //fileStream.Close();

        //}

        //private void btnCalculator_Click(object sender, EventArgs e)
        //{
        //    PositionModel po = new PositionModel();
        //    Double poWave = Convert.ToDouble(tbxleftLocation.Text.Split('_')[0]);
        //    Double pwWave = Convert.ToDouble(tbxleftLocation.Text.Split('_')[1]);
        //    po = DistanceHelper.FindNeighPosition(poWave, pwWave, Convert.ToDouble(tbxRange.Text));//(Convert.ToDouble(tbxleftLocation.Text),Convert.ToDouble(tbxRightLocation.Text), rangWave);
        //    richTextBox1.Text = "返回" + po.MaxLat + "|" + po.MaxLng + "|" + po.MinLat + "|" + po.MinLng;
        //    writeLog("返回MAX " + po.MaxLat + "|" + po.MaxLng + "|MIN" + po.MinLat + "|" + po.MinLng, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //}

        //private void btnKMcal_Click(object sender, EventArgs e)
        //{
        //    double result = DistanceHelper.GetDistance(Convert.ToDouble(tbxlat.Text), Convert.ToDouble(tbxlng.Text), Convert.ToDouble(tbxlat2.Text), Convert.ToDouble(tbxlng2.Text));
        //    writeLog("返回计算距离" + result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //}


        //参数（起点坐标，角度，斜边长（距离）） 这是一个基本的三角函数应用
        //private double[] getNewPoint(double[] point, double angle, double bevel)
        //{
        //    //在Flash中顺时针角度为正，逆时针角度为负
        //    //换算过程中先将角度转为弧度
        //    var radian = angle * Math.PI / 180;
        //    var xMargin = Math.Cos(radian) * bevel;
        //    var yMargin = Math.Sin(radian) * bevel;
        //    return new double[] { point[0] + xMargin, point[1] + yMargin };

        //}



        //private void button1_Click(object sender, EventArgs e)
        //{ //
        //    if (!string.IsNullOrEmpty(textBox2.Text))
        //    {

        //        double poWave = Convert.ToSingle(textBox1.Text.Split('_')[0]);
        //        double pwWave = Convert.ToSingle(textBox1.Text.Split('_')[1]);
        //        float prWave = Convert.ToSingle(textBox2.Text);

        //        PositionModel bto = new 雷电定位测试工具.PositionModel();
        //        bto = DistanceHelper.FindNeighPosition(poWave, pwWave, prWave);

        //        richTextBox2.Text = "左下:经度\r" + bto.MinLng + "\r纬度\r" + bto.MinLat + "\r" + "右上:经度\r" + bto.MaxLng + "\r纬度:\r" + bto.MaxLat;


        //        // stw.Start();
        //        //writeLog("采集数据开始(leftLocation =rightLocation =startTime =endTime =)...",DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.ffffff"));




        //        //stw.Stop(); 
        //        /// writeLog("采集数据结束，耗时 " + stw.Elapsed.Minutes + "分 " +stw.Elapsed.Seconds+"秒"+stw.Elapsed.Milliseconds+"毫秒。")
        //        //      Double poWave = Convert.ToDouble(tbxleftLocation.Text.Split('_')[0]);
        //        //     Double pwWave = Convert.ToDouble(tbxleftLocation.Text.Split('_')[1]);
        //        //  double[] point = { poWave, pwWave };
        //        //  double bevel = 5 * Math.Sqrt(2);
        //        ////double [] result=   getNewPoint(point, 45, bevel);
        //        //PointF fo = new PointF();
        //        //fo.X =poWave;
        //        //fo.Y = pwWave;
        //        //richTextBox2.Text = getNewPoint(fo, 45, bevel).X.ToString()+"\n" + getNewPoint(fo, 45, bevel).Y.ToString();
        //        // richTextBox2.Text = result[0].ToString() +"\n"+ result[1].ToString (); 
        //    }
        //}

        //private void label9_Click(object sender, EventArgs e)
        //{

        //}

        //private void label8_Click(object sender, EventArgs e)
        //{

        //}

        //private void label7_Click(object sender, EventArgs e)
        //{

        //}

        //private void tbxWaveEndTime_TextChanged(object sender, EventArgs e)
        //{

        //}

        //private void tbxWaveStartTime_TextChanged(object sender, EventArgs e)
        //{

        //}

        //private void tbxdeviceOneName_TextChanged(object sender, EventArgs e)
        //{

        //}

        //private void btnZhuzhan_Click(object sender, EventArgs e)
        //{
        //    int FRrtn = -1;
        //    string FRerror = "";

        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();
        //        雷电定位测试工具.FaultRecordService.time startTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.time endTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.faultRecordRtn FRfaultRecordRtn = new FaultRecordService.faultRecordRtn();

        //        DateTime startTime1 = Convert.ToDateTime(tbxZhuzhanSta.Text);
        //        DateTime endTime1 = Convert.ToDateTime(tbxZhuzhanStop.Text);
        //        writeLog("datetime类型 startTime1" + startTime1.ToString() + "endTime1" + endTime1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        try
        //        {
        //            ConvertTime(startTime, startTime1);
        //            ConvertTime(endTime, endTime1);
        //        }
        //        catch (Exception ex)
        //        {

        //            writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }

        //        writeLog("zhuzhantime类型 startTime" + startTime.ToString() + "endtime" + endTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        FRfaultRecordRtn = FRService.getFaultRecordByMainstationName(tbxZhuzhan.Text, startTime, endTime);
        //        //FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text, startTime, endTime);
        //        // FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);

        //        //FRrtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).rtn;
        //        //FRerror = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).error;
        //        //FRfaultRecordRtn.faultRecords = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).faultRecords;

        //        FRrtn = FRfaultRecordRtn.rtn;
        //        FRerror = FRfaultRecordRtn.error;
        //        writeLog(FRrtn.ToString() + FRerror.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //        writeLog(FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


        //        writeLog(FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        for (int i = 0; i < FRfaultRecordRtn.faultRecords.Length; i++)
        //        {
        //            writeLog("返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //}

        //private void btnYci_Click(object sender, EventArgs e)
        //{
        //    int FRrtn = -1;
        //    string FRerror = "";

        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();
        //        雷电定位测试工具.FaultRecordService.time startTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.time endTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.faultRecordRtn FRfaultRecordRtn = new FaultRecordService.faultRecordRtn();

        //        DateTime startTime1 = Convert.ToDateTime(tbxYsta.Text);
        //        DateTime endTime1 = Convert.ToDateTime(tbxYstop.Text);
        //        writeLog("datetime类型 startTime1" + startTime1.ToString() + "endTime1" + endTime1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        try
        //        {
        //            ConvertTime(startTime, startTime1);
        //            ConvertTime(endTime, endTime1);
        //        }
        //        catch (Exception ex)
        //        {

        //            writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }

        //        writeLog("*yici**time类型 startTime" + startTime.ToString() + "endtime" + endTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        FRfaultRecordRtn = FRService.getFaultRecordByDeviceOneName(tbxYiciName.Text, startTime, endTime);
        //        //FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text, startTime, endTime);
        //        // FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);

        //        //FRrtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).rtn;
        //        //FRerror = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).error;
        //        //FRfaultRecordRtn.faultRecords = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).faultRecords;

        //        FRrtn = FRfaultRecordRtn.rtn;
        //        FRerror = FRfaultRecordRtn.error;
        //        writeLog(FRrtn.ToString() + FRerror.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


        //        writeLog(FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        for (int i = 0; i < FRfaultRecordRtn.faultRecords.Length; i++)
        //        {
        //            writeLog("返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    int FRrtn = -1;
        //    string FRerror = "";

        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();
        //        雷电定位测试工具.FaultRecordService.time startTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.time endTime = new FaultRecordService.time();// tbxWaveStartTime.Text;
        //        雷电定位测试工具.FaultRecordService.faultRecordRtn FRfaultRecordRtn = new FaultRecordService.faultRecordRtn();

        //        DateTime startTime1 = Convert.ToDateTime(tbxRsta.Text);
        //        DateTime endTime1 = Convert.ToDateTime(tbxRstop.Text);
        //        writeLog("datetime类型 startTime1" + startTime1.ToString() + "endTime1" + endTime1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        try
        //        {
        //            ConvertTime(startTime, startTime1);
        //            ConvertTime(endTime, endTime1);
        //        }
        //        catch (Exception ex)
        //        {

        //            writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }

        //        writeLog("luboming***time类型 startTime" + startTime.ToString() + "endtime" + endTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        FRfaultRecordRtn = FRService.getFaultRecordByRecorderName(tbxRecorderName.Text, startTime, endTime);
        //        //FRService.getFaultRecordByDeviceOneName(tbxdeviceOneName.Text, startTime, endTime);
        //        // FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime);

        //        //FRrtn = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).rtn;
        //        //FRerror = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).error;
        //        //FRfaultRecordRtn.faultRecords = FRService.getFaultRecordByStationName(tbxdeviceOneName.Text, startTime, endTime).faultRecords;

        //        FRrtn = FRfaultRecordRtn.rtn;
        //        FRerror = FRfaultRecordRtn.error;
        //        writeLog(FRrtn.ToString() + FRerror.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));


        //        writeLog(FRfaultRecordRtn.faultRecords.Length.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        for (int i = 0; i < FRfaultRecordRtn.faultRecords.Length; i++)
        //        {
        //            writeLog("返回rtn:" + FRrtn.ToString() + "返回error:" + FRerror + "返回faultRecordRtn" + "编号" + FRfaultRecordRtn.faultRecords[i].id.ToString() + "一次设备名" + FRfaultRecordRtn.faultRecords[i].deviceOneName.ToString() + "故障时间毫秒值" + FRfaultRecordRtn.faultRecords[i].faultTime.month.ToString() + "主站名" + FRfaultRecordRtn.faultRecords[i].stationName.ToString() + "录波器名" + FRfaultRecordRtn.faultRecords[i].recorderName.ToString() + "故障相别" + FRfaultRecordRtn.faultRecords[i].phase.ToString() + "故障测距" + FRfaultRecordRtn.faultRecords[i].location.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //}


        // string DrawLargeRectangle = "";
        PositionModel DrawLargeRectangle = new 雷电定位测试工具.PositionModel();

        //private void btnDrawALargeRectangle_Click(object sender, EventArgs e)
        //{
        //    double poWave = Convert.ToDouble(tbxCenterPoint.Text.Split('_')[0]);
        //    double pwWave = Convert.ToDouble(tbxCenterPoint.Text.Split('_')[1]);
        //    float prWave = Convert.ToSingle(tbxAcquisitionRange.Text);

        //    PositionModel bto = new 雷电定位测试工具.PositionModel();

        //    DrawLargeRectangle = DistanceHelper.FindNeighPosition(poWave, pwWave, prWave);
        //    bto = DistanceHelper.FindNeighPosition(poWave, pwWave, prWave);
        //    // DrawLargeRectangle= "左下:经度\r" + bto.MinLng + "\r纬度\r" + bto.MinLat + "\r" + "右上:经度\r" + bto.MaxLng + "\r纬度:\r" + bto.MaxLat;
        //    richTextBox2.Text = "左下:经度\r" + bto.MinLng + "\r纬度\r" + bto.MinLat + "\r" + "右上:经度\r" + bto.MaxLng + "\r纬度:\r" + bto.MaxLat;

        //}


        //private void btnDrawASmallRectangle_Click(object sender, EventArgs e)
        //{
        //    // if (!string.IsNullOrEmpty(DrawLargeRectangle))
        //    {
        //        string RectangleStart, RectangleStartTwo, RectangleStartThree, RectangleStop = "";
        //        float smallR;
        //        RectangleStart = DrawLargeRectangle.MinLng.ToString() + "_" + DrawLargeRectangle.MaxLat.ToString();//第一个点 最小经度 最大维度
        //        RectangleStartTwo = DrawLargeRectangle.MaxLng.ToString() + "_" + DrawLargeRectangle.MaxLat.ToString();//最大
        //        RectangleStartThree = DrawLargeRectangle.MinLng.ToString() + "_" + DrawLargeRectangle.MinLat.ToString();//最小
        //        RectangleStop = DrawLargeRectangle.MaxLng.ToString() + "_" + DrawLargeRectangle.MinLat.ToString();//最大经度 最小维度
        //        mark = 0;
        //        markLng = 0;
        //        smallR = Convert.ToSingle(tbxxiaojuxing.Text);
        //        DrawASmall(RectangleStart, RectangleStop, DrawLargeRectangle.MaxLng, DrawLargeRectangle.MinLat,smallR);

        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RectangleStart"></param>
        /// <param name="RectangleStop"></param>
        /// <param name="RectangleStartTwoLng"></param>
        /// <param name="RectangleStartThreeLat">指示纬度缩减的停止值</param>
        //private void DrawASmall(string RectangleStart, string RectangleStop, double RectangleStartTwoLng, double RectangleStartThreeLat, float r)
        //{
        //    double btnLng;
        //    double btLat;
        //    //小矩形暂定2km的长和宽//中心点变为左上,依次推左下 和右上//实际画取2kM的正方形
        //    double poWave = Convert.ToDouble(RectangleStart.Split('_')[0]);
        //    double pwWave = Convert.ToDouble(RectangleStart.Split('_')[1]);
        //  //  float prWave = 2;

        //    double pwtWave = Convert.ToDouble(RectangleStart.Split('_')[1]);

        //    //btnLng = btm.MaxLng;

        //    // for (; btnLng < RectangleStartTwoLng; DrawASmall(RectangleStart, "", RectangleStartTwoLng, RectangleStartThreeLat))//经度缩减 纬度不变
        //    for (double j = poWave; j <= RectangleStartTwoLng; j = DrawASmallLng(poWave, pwtWave, RectangleStartTwoLng, false,r).MaxLng)
        //    {
        //        pwWave = pwtWave;
        //       // PositionModel bta = new 雷电定位测试工具.PositionModel();
        //       // bta = DistanceHelper.FindNeighPosition(poWave, pwWave, prWave);
        //        //writeLog("返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + pwWave, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //        //  RectangleStart = btm.MaxLng.ToString() + "_" + pwWave.ToString();
        //        if (j != poWave)
        //        {
        //            poWave = DrawASmallLng(j, pwtWave, RectangleStartTwoLng, false,r).MaxLng;

        //        }


        //        for (double i = DrawASmallLat(poWave, pwWave, RectangleStartThreeLat, true,r).MinLat; RectangleStartThreeLat < i; i = DrawASmallLat(poWave, pwWave, RectangleStartThreeLat, false,r).MinLat)////--纬度追加递减
        //        {
        //            // PositionModel btnLat=new 雷电定位测试工具.PositionModel ();
        //            // btnLat = DrawASmallLat(poWave, btm.MinLat, RectangleStartThreeLat):

        //            pwWave = DrawASmallLat(poWave, i, RectangleStartThreeLat, true,r).MinLat;//少画了两个最初的小矩形//bta.MinLat//work out
        //        }                //DrawASmall(RectangleStart, "", RectangleStartTwoLng);

        //    }
        //}

        int markLng = 0;
        private PositionModel DrawASmallLng(double poWave, double twolat, double RectangleStartThreeLat, bool p, float r)
        {
            float prWave = r;//暂定2km
            PositionModel btm = new 雷电定位测试工具.PositionModel();
            btm = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
            // btnLat = btm.MinLat;
            if (p)//判断仅是自减纬度 还是需要输出小矩形 0为自减
            {
                // btnLng = btm.MaxLng;//纬度递减引起的经度误差 可利用Mark
                markLng++;
                if (markLng > 1)
                {
                    PositionModel btI = new 雷电定位测试工具.PositionModel();
                    btI = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
                    writeLog(markLng + "返回小矩形的左下经度" + poWave + "纬度" + btI.MinLat + "右上经度:" + btI.MaxLng + "右上纬度" + twolat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }
                else
                {
                    // PositionModel btm = new 雷电定位测试工具.PositionModel();
                    btm = DistanceHelper.FindNeighPosition(poWave, twolat, prWave);
                    // markLng = btm.MaxLng;
                    writeLog(markLng + "返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + twolat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }
                // throw new NotImplementedException();
            }
            return btm;

        }
        int mark = 0;//用来标记小矩形纬度递减的个数
        double markLat;
        private PositionModel DrawASmallLat(double poWave, double minLat, double rectangleStartThreeLat, Boolean w, float r)
        {
            double btnLng;
            float prWave = r;//暂定2km
            PositionModel btm = new 雷电定位测试工具.PositionModel();
            btm = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
            // btnLat = btm.MinLat;
            if (w)//判断仅是自减纬度 还是需要输出小矩形 0为自减
            {
                // btnLng = btm.MaxLng;//纬度递减引起的经度误差 可利用Mark
                mark++;
                if (mark > 1)
                {
                    PositionModel btI = new 雷电定位测试工具.PositionModel();
                    btI = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
                    writeLog(mark + "返回小矩形的左下经度" + poWave + "纬度" + btI.MinLat + "右上经度:" + markLat + "右上纬度" + minLat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }
                else
                {
                    // PositionModel btm = new 雷电定位测试工具.PositionModel();
                    btm = DistanceHelper.FindNeighPosition(poWave, minLat, prWave);
                    markLat = btm.MaxLng;
                    writeLog(mark + "返回小矩形的左下经度" + poWave + "纬度" + btm.MinLat + "右上经度:" + btm.MaxLng + "右上纬度" + minLat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

                }

            }

            return btm;
            // throw new NotImplementedException();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        //private void button3_Click(object sender, EventArgs e)
        //{


        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    long waveId = Convert.ToInt64(tbxWaveId.Text);// Convert.ToUInt32(tbxWaveId);
        //    int rtn = -1;
        //    byte[] bytes = new byte[] { };

        //    string error = "";
        //    try
        //    {
        //        雷电定位测试工具.FaultRecordService.FaultRecordServiceService FRService = new FaultRecordService.FaultRecordServiceService();

        //        //rtn = FRService.getFile(waveId, tbxWaveExtension.Text).rtn;
        //        //bytes = FRService.getFile(waveId, tbxWaveExtension.Text).bytes;

        //        //FileRtn fr = new FileRtn();
        //        //var fileRtn1 = FRService.getFile(waveId, tbxWaveExtension.Text);
        //        //fr = GetFileRtn1(fileRtn1);
        //        雷电定位测试工具.FaultRecordService.fileRtn fr = new FaultRecordService.fileRtn();
        //        fr = FRService.getFile(waveId, tbxWaveExtension.Text);
        //        rtn = fr.rtn;
        //        writeLog("返回wenjian** 返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        bytes = fr.bytes;
        //        error = fr.error;
        //        writeLog("返回wenjian**" + bytes.Length.ToString() + "返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        if (bytes.Length != 0)
        //        {
        //            // FileStream opBytes
        //            waveDownload(bytes);

        //        };
        //        //error = FRService.getFile(waveId, tbxWaveExtension.Text).error;
        //        //  writeLog("返回rtn:" + rtn + "返回ByteS:" + bytes + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        writeLog("返回rtn:" + rtn.ToString() + "返回error：" + error, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        //    }
        //    catch (Exception ex)
        //    {
        //        writeLog("异常" + ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));

        //        // throw;
        //    }
        //}




        //参数（起点坐标，角度，斜边长（距离）） 这是一个基本的三角函数应用
        //private PointF getNewPoint(PointF point, double angle, double bevel)
        //{
        //    //在Flash中顺时针角度为正，逆时针角度为负
        //    //换算过程中先将角度转为弧度
        //    var radian = angle * Math.PI / 180;
        //    var xMargin = Math.Cos(radian) * bevel;
        //    var yMargin = Math.Sin(radian) * bevel;
        //    return new PointF(point.X + (float)xMargin, point.Y + (float)yMargin);

        //}






        ///// <remarks/>
        //[System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace = "http://www.sgcc.com.cn/sggis/service/gisservice", ResponseNamespace = "http://www.sgcc.com.cn/sggis/service/gisservice", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        //[return: System.Xml.Serialization.XmlElementAttribute("out")]
        //public string queryPSR(string inputXML)
        //{
        //    object[] results = this.Invoke("queryPSR", new object[] {
        //                inputXML});
        //    return ((string)(results[0]));
        //}
    }
}
