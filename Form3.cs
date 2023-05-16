using HtmlAgilityPack;
using System;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Net;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public string  session { get; set; }
        public string courseUrl { get; set; }

        List<Tuple<string, string>> courseIdAndAddUrlList = new List<Tuple<string, string>>();
        public Form3()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form3_FormClosing);
        }


        
        private void Form3_Load(object sender, EventArgs e)
        {
            //默认不显示打开网页端按钮
            button2.Visible = false;

            //获取未完成课程
            if (getNotcompleteClazz() == 0)
            {
                button1.Visible = false;
                button2.Visible = true;
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭应用程序
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //隐藏按钮
            Button button = (Button)sender;
            button.Hide();
            label1.Text = "正在注入攻击，请保持网络畅通，不要关闭本页面。。。";
            label1.Refresh();


            //执行1000倍速播放
           while(getNotcompleteClazz()!=0)
            {
                for (int i = 0; i < 2; i++)
                {
                    foreach (Tuple<string, string> tuple in courseIdAndAddUrlList)
                    {
                        // tuple.Item1, tuple.Item2);
                        //打开一次播放页面
                        Ajax ajax1 = new Ajax();
                        ajax1.cookieString = session;
                        ajax1.Url = "http://hebgb.gwypx.com.cn/portal/study_play.do?id=" + tuple.Item2;
                        ajax1.MakeRequest();

                        //500倍速度播放
                        Ajax ajax2 = new Ajax();
                        ajax2.cookieString = session;
                        ajax2.Method = "POST";
                        ajax2.Url = "http://hebgb.gwypx.com.cn/portal/seekNew.do";
                        ajax2.Body = "id=" + tuple.Item2 + "&serializeSco={\"res01\":{\"lesson_location\":9,\"session_time\":10,\"last_learn_time\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"},\"last_study_sco\":\"res01\"}&duration=1000&study_course=" + tuple.Item1;
                        ajax2.MakeRequest();
                    }
                }
            }

            Thread.Sleep(1000);
            
            label1.Text = "";
            label3.Text = "";
            label2.Text = "已看完全部课程！请打开网页端查看。";
            label2.ForeColor = Color.Red;
            button2.Visible = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //去网页查看
            ProcessStartInfo startInfo = new ProcessStartInfo("http://hebgb.gwypx.com.cn/");
            Process.Start(startInfo);
            // 关闭应用程序
            Application.Exit();
        }

        //获取未看完的课程
        private int getNotcompleteClazz()
        {
            //获取全部课程信息
            Ajax ajax = new Ajax();
            ajax.cookieString = session;
            ajax.Url = "http://hebgb.gwypx.com.cn" + this.courseUrl;
            string html = ajax.MakeRequest();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNodeCollection notcompleteClazz = doc.DocumentNode.SelectNodes("//div[@id='notcompleteClazz']");
            if(notcompleteClazz[0].InnerText.Trim()==""|| notcompleteClazz[0].InnerText.Trim() == null)
            {
                label3.Text = "未看完的课程还有：0节";
                return 0;
            }
            else
            {
                HtmlNodeCollection courseNodes = doc.DocumentNode.SelectNodes("//div[@id='notcompleteClazz']//div[@class='hoz_course_row']");
                if (courseNodes != null)
                {
                    foreach (HtmlNode courseNode in courseNodes)
                    {
                        string href = courseNode.SelectSingleNode(".//h2[@class='hoz_course_name']/a")
                            .GetAttributeValue("href", "");
                        int startIndex = href.IndexOf("courseId=") + 9;
                        int endIndex = href.IndexOf("&", startIndex);
                        if (endIndex == -1)
                        {
                            endIndex = href.Length;
                        }
                        string courseId = href.Substring(startIndex, endIndex - startIndex);
                        string addUrl = courseNode.SelectSingleNode(".//input[@type='button']")
                            .GetAttributeValue("onclick", "");
                        startIndex = addUrl.IndexOf("(") + 1;
                        endIndex = addUrl.IndexOf(")", startIndex);
                        string addUrlValue = addUrl.Substring(startIndex, endIndex - startIndex);
                        courseIdAndAddUrlList.Add(new Tuple<string, string>(courseId, addUrlValue));
                    }
                }
                int s = courseIdAndAddUrlList.Count;
                label3.Text = "未看完的课程还有：" + s + "节";
                return s;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.session = session;
            this.Hide();
            form2.Show();
        }
    }
}
