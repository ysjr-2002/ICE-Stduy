using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AirPort_Server.WebAPI
{
    class HttpMethod
    {
        public static void Get(string url)
        {
            var wr = MyWebRequest.CreateGet(url);
            var response = wr.GetResponse();
            var stream = response.GetResponseStream();

            StreamReader sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            sr.Close();
            Console.WriteLine(content);
            Trace.WriteLine(content);
        }

        public static T Post<T>(string url, byte[] data, Dictionary<string, string> param)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)MyWebRequest.CreateImage(url, boundary);
            WebResponse response = null;

            StringBuilder sb = new StringBuilder();
            try
            {
                var rs = request.GetRequestStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in param.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, param[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }

                //文件开始
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                //图片
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "image", "image.jpg", "text/plain");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                rs.Write(data, 0, data.Length);
                //文件结束
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    var content = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine(content);

                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    return serialize.Deserialize<T>(content);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private static void Write(byte[] buffer, Stream output)
        {
            using (var input = new MemoryStream(buffer))
            {
                var len = 0;
                var data = new byte[1024];
                while ((len = input.Read(data, 0, data.Length)) > 0)
                {
                    output.Write(data, 0, len);
                }
            }
        }

        public static T Post<T>(string url, byte[] data1, byte[] data2, Dictionary<string, string> param)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)MyWebRequest.CreateImage(url, boundary);
            WebResponse response = null;

            StringBuilder sb = new StringBuilder();
            try
            {
                var rs = request.GetRequestStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in param.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, param[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }

                //文件开始
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                //图片1
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "image1", "image1.jpg", "text/plain");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                rs.Write(data1, 0, data1.Length);
                //文件结束1
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                rs.Write(trailer, 0, trailer.Length);

                //图片2
                string header2 = string.Format(headerTemplate, "image2", "image2.jpg", "text/plain");
                byte[] headerbytes2 = System.Text.Encoding.UTF8.GetBytes(header2);
                rs.Write(headerbytes2, 0, headerbytes.Length);
                rs.Write(data2, 0, data2.Length);

                //文件结束
                rs.Write(trailer, 0, trailer.Length);

                rs.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    var content = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine(content);

                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    return serialize.Deserialize<T>(content);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
