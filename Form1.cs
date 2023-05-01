using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nocease");
        }

        //点击登录
        private void button1_Click(object sender, EventArgs e)
        {
           string loginUrl = "http://hebgb.gwypx.com.cn/portal/login_ajax.do";
           string data = "username="+this.textBox1.Text+"&passwd="+this.textBox2.Text;
           string contentType = "application/x-www-form-urlencoded";
           HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64";

            byte[] postData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = postData.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                Console.WriteLine(responseString);
                label4.Text = responseString;

                if (response.Headers.Get("Set-Cookie") != null)
                {
                    string cookies = response.Headers.Get("Set-Cookie");
                    if (responseString == "xt|邢台分院")
                    {
                        Form2 form2 = new Form2();
                        form2.session = cookies;
                        this.Hide();
                        form2.Show();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string errorResponseString = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    Console.WriteLine(errorResponseString);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}