using AirPort.Server.FaceResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common;

namespace AirPort.Server.WebAPI
{
    public class HttpMethod
    {
        public static T Get<T>(string url, Dictionary<string, string> param)
        {
            var preSignStr = HttpCore.CreateLinkString(param);
            if (preSignStr.Length > 0)
                preSignStr = "?" + preSignStr;
            var httpGetUrl = string.Concat(url, preSignStr);
            var wr = HttpRequest.Get(httpGetUrl);
            try
            {
                var response = wr.GetResponse();
                var stream = response.GetResponseStream();

                StreamReader sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                sr.Close();
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                return serialize.Deserialize<T>(content);
            }
            catch (Exception ex)
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
            finally
            {
                response?.Close();
            }
        }

        public static T PostNoImage<T>(string url, Dictionary<string, string> param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpRequest.Post(url);
            WebResponse response = null;

            StringBuilder sb = new StringBuilder();
            try
            {
                var rs = request.GetRequestStream();
                var preSignStr = HttpCore.CreateLinkString(param);
                var postdata = Encoding.UTF8.GetBytes(preSignStr);
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
            finally
            {
                response?.Close();
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

                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    return serialize.Deserialize<T>(content);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
            finally
            {
                response?.Close();
            }
        }

        public void Websocket(string videourl, float threshold, Action<DynamicFaceResult> callback)
        {
            //videourl = "rtsp://192.168.1.151/user=admin&password=&channel=1&stream=0.sdp?";
            Console.WriteLine(videourl);
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("url", videourl);
            //抓拍的人脸张数（根据业务传）
            param.Add("limit", "10000");
            param.Add("crop", "face");
            //不转只进行抓拍
            //param.Add("group", "");
            //param.Add("threshold", threshold.ToString());
            //抓图间隔
            param.Add("interval", "3000");
            //抓拍人脸的最小大小
            param.Add("facemin", "100");
            //websocket名称
            param.Add("name", "snap");

            var url = Constrants.url_video;

            var ext = HttpCore.CreateLinkString(param);
            url = url + "?" + ext;

            WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(url);
            ws.OnOpen += (a, b) =>
            {
                Console.WriteLine("opened");
            };
            ws.OnClose += (a, b) =>
            {
                Console.WriteLine("close");
            };
            ws.OnMessage += (a, m) =>
            {
                Console.WriteLine("message coming...");
                if (m.IsText)
                {
                    Console.WriteLine("is text");
                    var result = GetFaceResult(m.Data);
                    if (result.Type == "recognize")
                    {
                        if (result.Result.Face.Quality > threshold)
                        {
                            var temp = GetFaceResult(m.Data);
                            temp.Face.Image = "";
                            var json = ToJson(temp);
                            callback(result);
                        }
                        else
                        {
                            Console.WriteLine("低于阈值->" + result.Result.Face.Quality);
                        }
                    }
                    else
                    {
                        Console.WriteLine("type=" + result.Type);
                    }
                }
                if (m.IsBinary)
                    Console.WriteLine("is binary");

                if (m.IsPing)
                    Console.WriteLine("is ping");

            };
            ws.Connect();
        }

        private DynamicFaceResult GetFaceResult(string content)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            var result = serialize.Deserialize<DynamicFaceResult>(content);
            return result;
        }

        private string ToJson(DynamicFaceResult content)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            var result = serialize.Serialize(content);
            return result;
        }
    }
}
