using Hisingpower;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hisingpower.DButility;
using System.Web;

namespace naviLink
{
    public partial class Form1 : Form
    {
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
        public Form1()
        {
           

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double lat = 0;
            double lng = 0;
            string towerName = "";
            string url = "";
            string sql = "select *from [FAULTVISTA].[dbo].[Tower]";
            string shotLink = "";

            try
            {

                using (SqlDataReader sdr = SqlHelper.ExecuteDataReader(Connstr, sql, null))
                {
                    if (sdr.HasRows)
                    {
                        string lineName = "";

                        while (sdr.Read())
                        {
                            lat = Convert.ToDouble(sdr["Latitude"]);
                            lng = Convert.ToDouble(sdr["Longitude"]);
                            towerName = sdr["TowerName"].ToString().Replace("#", "");//[] 
                            lineName = sdr["LineKey"].ToString();
                            url = "http://api.t.sina.com.cn/short_url/shorten.xml?source=3271760578&url_long=";
                           string tUrl="http://apis.map.qq.com/tools/poimarker?type=0&marker=coord:"+lat+","+lng+";title:故障杆塔;addr:"+towerName + "&key=OB4BZ-D4W3U-B7VVO-4PJWW-6TKDJ-WPB77&referer=myapp";
                            url = url+ HttpUtility.UrlEncode(tUrl, System.Text.Encoding.GetEncoding(65001));
                            shotLink = GetshotUrl(url);
                            shotLink = AssGetCFG.getCFG(shotLink);
                            
                            string sqlInsert = "update  [FAULTVISTA].[dbo].[Tower] set [naviLink] ='"+shotLink+"' where "+"[Latitude]= "+ lat+ " and [LineKey]='"+lineName+"'";
                            if (SqlHelper.ExecuteNonQuery(Connstr, sqlInsert, null) > 0)
                            {
                                Common.WriteLog("添加成功", Common.Now(), sqlInsert);
                            }
                            else
                            {
                                Common.WriteLog("添加失败", Common.Now(), sqlInsert);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLog("异常信息",ex.ToString (), Common.Now() );

                //  throw;
            }


        }

        private  string  GetshotUrl(string  uri)
        {
            string content = "";
            //请求  
            // string uri = " http://www.baidu.com";
            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";                            //请求方法  
            request.ProtocolVersion = new Version(1, 1);   //Http/1.1版本  
                                                           //Add Other ...  
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //Header  
            foreach (var item in response.Headers)
            {
               this. txt_Header.Text += item.ToString() + ": " +
                response.GetResponseHeader(item.ToString())
                + System.Environment.NewLine;
            }

            //如果主体信息不为空，则接收主体信息内容  
            if (response.ContentLength <= 0)
                return "response null";
            //接收响应主体信息  
            using (Stream stream = response.GetResponseStream())
            {
                //byte[] bytes = new byte[stream.Length];
                //stream.Read(bytes, 0, bytes.Length);
                //stream. Seek(0, SeekOrigin.Begin);
                int totalLength = (int)response.ContentLength;
                int numBytesRead = 0;
                byte[] bytes = new byte[totalLength + 1024];
                //通过一个循环读取流中的数据，读取完毕，跳出循环  
                while (numBytesRead < totalLength)
                {
                    int num = stream.Read(bytes, numBytesRead, 1024);  //每次希望读取1024字节  
                    if (num == 0)   //说明流中数据读取完毕  
                        break;
                    numBytesRead += num;
                }
                content = Encoding.UTF8.GetString(bytes);
                //将接收到的主体数据显示到界面  
                this.txt_Content.Text = content;
            }

            return content;
        }
    }
}
