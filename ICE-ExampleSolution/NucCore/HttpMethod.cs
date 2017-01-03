using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common;
using System.Web;

namespace NucCore
{
    class HttpMethod
    {
        public static void Delete(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpRequest.Delete(url);
            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();
            }
        }

        public static T Get<T>(string url)
        {
            HttpWebRequest request = HttpRequest.Get(url);
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    return serialize.Deserialize<T>(content);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T Post<T>(string url, byte[] data, Dictionary<string, string> param)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)HttpRequest.PostImage(url, boundary);
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

        public static T PostNoImage<T>(string url, Dictionary<string, string> param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpRequest.Post(url);
            WebResponse response = null;

            StringBuilder sb = new StringBuilder();
            try
            {
                var preSignStr = HttpCore.CreateLinkString(param);
                var postdata = Encoding.UTF8.GetBytes(preSignStr);

                var rs = request.GetRequestStream();
                rs.Write(postdata, 0, postdata.Length);
                rs.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    var content = sr.ReadToEnd();
                    sr.Close();

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
    }


    public static class HttpCore
    {
        /// <summary>
        ///  把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            if (dicArray == null || dicArray.Count == 0)
                return string.Empty;

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
    }
}
