using System;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public string session { get; set; }
        public Form2()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button1, "频繁使用本程序，会导致总看课时间过长。可点击这里让数据看起来更合理。");
            //获取姓名
            Ajax ajax = new Ajax();
            ajax.Method = "POST";
            ajax.Url = "http://hebgb.gwypx.com.cn/portal/checkIsLogin.do";
            ajax.cookieString = session;
            string result = ajax.MakeRequest();
            dynamic json1 = JsonConvert.DeserializeObject(result);
            string realname = json1.realname;
            label1.Text = realname;

            //获取课程列表页面
            Ajax ajax2 = new Ajax();
            ajax2.cookieString = session;
            ajax2.Url = "http://hebgb.gwypx.com.cn/student/class_myClassList.do?type=1&menu=myclass";
            string html = ajax2.MakeRequest();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //获取已完成时长
            var doc1 = doc;
            var center_xs = doc1.DocumentNode.SelectNodes("//div[@class='center_course_data']");
            var pattern = @"(?<=<div class=""center_xs""><h5>)[\d\.]+小时(?=</h5>已完成时长</div>)";
            var match = Regex.Match(center_xs[0].InnerHtml, pattern);
            if (match.Success)
            {
                label6.Text = match.Value;
            }

            //获取未完成课程
            var courseNodes = doc.DocumentNode.SelectNodes("//div[@class='learn_list']//div[@class='join_special_list']");
            var courses = new List<Tuple<string, string>>();
            foreach (var courseNode in courseNodes)
            {
                var courseNameNode = courseNode.SelectSingleNode(".//h2[@class='join_course_name']/a");
                var courseLinkNode = courseNode.SelectSingleNode(".//div[@class='join_btn_enter']/a[not(@style='background-color:grey;')]");
                if (courseNameNode != null && courseLinkNode != null)
                {
                    string courseName = courseNameNode.InnerText.Trim();
                    string courseLink = courseLinkNode.GetAttributeValue("href", "");
                    courses.Add(new Tuple<string, string>(courseName, courseLink));
                    courseUrl = courseLink;//最后一个链接存起来，用来减时长刷课
                }
            }

            Form3 form3 = new Form3();
            form3.session = this.session;
            foreach (var tuple in courses)
            {
                // 在窗体上添加 Button 控件，显示 Tuple 的 name，点击按钮打开 Tuple 的 url
                var button = new Button();
                button.Text = tuple.Item1;
                button.Width = 500;
                button.Location = new System.Drawing.Point(100, 130 + courses.IndexOf(tuple) * 30);
                button.Click += (sender1, e1) =>
                {
                    form3.courseUrl = tuple.Item2;
                    this.Hide();
                    form3.Show();
                };
                this.Controls.Add(button);
            }
        }


        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭应用程序
            Application.Exit();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        //随意拿到一个课程链接
        private static string courseUrl = "";
        //减少观看时长
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("有bug，暂不开放。");

            /* 
             MessageBox.Show("请稍等。。。");
             //获取全部课程信息
             Ajax ajax = new Ajax();
             ajax.cookieString = session;
             ajax.Url = "http://hebgb.gwypx.com.cn" + courseUrl;
             string html = ajax.MakeRequest();
             HtmlDocument doc = new HtmlDocument();
             doc.LoadHtml(html);
             List<Tuple<string, string>> courseIdAndAddUrlList = new List<Tuple<string, string>>();
             HtmlNodeCollection courseNodes = doc.DocumentNode.SelectNodes("//div[@class='hoz_course_row']");
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
             //用负倍速播放一遍
             foreach (Tuple<string, string> tuple in courseIdAndAddUrlList)
             {
                 // tuple.Item1, tuple.Item2);
                 //打开一次播放页面
                 Ajax ajax1 = new Ajax();
                 ajax1.cookieString = session;
                 ajax1.Url = "http://hebgb.gwypx.com.cn/portal/study_play.do?id=" + tuple.Item2;
                 ajax1.MakeRequest();

                 //负速度播放
                 Ajax ajax3 = new Ajax();
                 ajax3.cookieString = session;
                 ajax3.Method = "POST";
                 ajax3.Url = "http://hebgb.gwypx.com.cn/portal/seekNew.do";
                 ajax3.Body = "id=" + tuple.Item2 + "&serializeSco={\"res01\":{\"lesson_location\":9,\"session_time\":10,\"last_learn_time\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"},\"last_study_sco\":\"res01\"}&duration=-2000&study_course=" + tuple.Item1;
                 ajax3.MakeRequest();
                // break;
             }

             //刷新显示时长
             Ajax ajax4 = new Ajax();
             ajax4.cookieString = session;
             ajax4.Url = "http://hebgb.gwypx.com.cn/student/class_myClassList.do?type=1&menu=myclass";
             string html2 = ajax4.MakeRequest();
             var doc2 = new HtmlDocument();
             doc2.LoadHtml(html2);

             //获取已完成时长
             var center_xs = doc2.DocumentNode.SelectNodes("//div[@class='center_course_data']");
             var pattern1 = @"(?<=<div class=""center_xs""><h5>)[\d\.]+小时(?=</h5>已完成时长</div>)";
             var match1 = Regex.Match(center_xs[0].InnerHtml, pattern1);
             if (match1.Success)
             {
                 label6.Text = match1.Value;
                 MessageBox.Show("已完成。");
             }
 */
        }
    }
}