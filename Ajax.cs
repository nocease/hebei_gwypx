using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System;

namespace WindowsFormsApp1
{
    public class Ajax
    {
        public string Url { get; set; }
        public string Method { get; set; } = "GET";
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        public string cookieString { get; set; }
        public string Body { get; set; }

        public string MakeRequest()
        {
            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = Method;
            request.ContentType = ContentType;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64";
           
            //cookie操作
            Cookie cookie = new Cookie();
            string[] cookieParts = cookieString.Split(';');
            foreach (string cookiePart in cookieParts)
            {
                string[] part = cookiePart.Trim().Split('=');
                if (part.Length == 2)
                {
                    string key = part[0].Trim();
                    string value = part[1].Trim();

                    switch (key.ToLower())
                    {
                        case "session":
                            cookie.Name = key;
                            cookie.Value = value;
                            break;
                        case "path":
                            cookie.Path = value;
                            break;
                        case "httponly":
                            cookie.HttpOnly = true;
                            break;
                        default:
                            cookie.Name = key;
                            cookie.Value = value;
                            break;
                    }
                }
            }
            CookieContainer cookieContainer = new CookieContainer();
            Uri uri = new Uri(Url);
            Cookie cookie2 = new Cookie("SESSION", cookie.Value, "/");
            cookieContainer.Add(uri, cookie);
            request.CookieContainer = cookieContainer;
            
            if (!string.IsNullOrEmpty(Body))
            {
                byte[] data = Encoding.UTF8.GetBytes(Body);
                request.ContentLength = data.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

    }
}
