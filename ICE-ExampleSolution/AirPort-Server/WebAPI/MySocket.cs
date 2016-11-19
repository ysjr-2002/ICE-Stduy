using AirPort.Server.FaceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AirPort.Server.WebAPI
{
    public class MySocket
    {
        public string rtstpId
        {
            get; set;
        }
        private WebSocketSharp.WebSocket ws = null;
        public delegate void VideoFaceDetectEventHandler(string rtspId, DynamicFaceResult face);
        public event VideoFaceDetectEventHandler OnFaceDetect;

        public void Run(string rtstpId, string videourl, float threshold)
        {
            this.rtstpId = rtstpId;
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

            ws = new WebSocketSharp.WebSocket(url);
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
                            Console.WriteLine(json);
                            if (OnFaceDetect != null)
                            {
                                OnFaceDetect(this.rtstpId, result);
                            }
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

        public void Stop()
        {
            ws.Close();
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
