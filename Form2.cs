using System;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.RegularExpressions;

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
            //获取姓名
            Ajax ajax = new Ajax();
            ajax.Method = "POST";
            ajax.Url = "http://hebgb.gwypx.com.cn/portal/checkIsLogin.do";
            ajax.cookieString = session;
            string result = ajax.MakeRequest();
            dynamic json1 = JsonConvert.DeserializeObject(result);
            string realname = json1.realname;
            label1.Text = realname;

            //获取未完成课程
            Ajax ajax2 = new Ajax();
            ajax2.cookieString = session;
            ajax2.Url = "http://hebgb.gwypx.com.cn/student/class_myClassList.do?type=1&menu=myclass";
            string html = ajax2.MakeRequest();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
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
                button.Location = new System.Drawing.Point(40, 100 + courses.IndexOf(tuple) * 30);
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

    }
}