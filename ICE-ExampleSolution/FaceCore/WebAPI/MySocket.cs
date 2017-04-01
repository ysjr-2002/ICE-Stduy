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
        private WebSocketSharp.WebSocket ws = null;
        public event VideoFaceDetectEventHandler OnFaceDetect;
        public delegate void VideoFaceDetectEventHandler(string rtspId, DynamicFaceResult face);

        public MySocket()
        {
            IsConnected = false;
        }

        public string rtstpId
        {
            get; set;
        }

        public bool IsConnected
        {
            get; set;
        }

        public void Run(string rtstpId, string videourl, float threshold)
        {
            this.rtstpId = rtstpId;
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
                print("webscoket opened");
                IsConnected = true;
            };
            ws.OnClose += (a, b) =>
            {
                print("webscoket close");
            };
            ws.OnMessage += (a, m) =>
            {
                if (m.IsText)
                {
                    var result = GetFaceResult(m.Data);
                    if (result.Type == "recognize")
                    {
                        print("recognize");
                        if (result.Result.Face.Quality > threshold)
                        {
                            if (OnFaceDetect != null)
                            {
                                OnFaceDetect(this.rtstpId, result);
                            }
                        }
                        else
                        {
                            print("低于阈值->" + result.Result.Face.Quality);
                        }
                    }
                    else
                    {
                        print("type=" + result.Type);
                    }
                }
                if (m.IsBinary)
                    print("is binary");

                if (m.IsPing)
                    print("is ping");
            };
            ws.Connect();
        }

        public void Stop()
        {
            ws.Close();
        }

        private static DynamicFaceResult GetFaceResult(string content)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            var result = serialize.Deserialize<DynamicFaceResult>(content);
            return result;
        }

        private static string ToJson(DynamicFaceResult content)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            var result = serialize.Serialize(content);
            return result;
        }

        private void print(string content)
        {
            Console.WriteLine(string.Format("iceserver:{0}", content));
        }
    }
}
