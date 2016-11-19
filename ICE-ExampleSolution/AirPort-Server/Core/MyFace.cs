using AirPort.Server.WebAPI;
using FaceRecognitionModule;
using Ice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Common;
using AirPort.Server.Repository;
using AirPort.Server.FaceResult;

namespace AirPort.Server.Core
{
    class MyFace : FaceRecognitionDisp_
    {
        private bool _stopCallback = false;
        private Queue<string> queue = new Queue<string>();
        private FaceServices fs = null;
        private const string group = "by";
        private PersonDB db = null;

        Dictionary<string, ClientData> faceResultList = new Dictionary<string, ClientData>();

        public MyFace(PersonDB db)
        {
            fs = new FaceServices();
            this.db = db;
            print("create a server object");
        }

        private string Element(string name, string value)
        {
            var content = "<{0}>{1}</{0}>";
            content = string.Format(content, name, value);
            return content;
        }

        public override void initConnectionListener(ConnectionListenerPrx listener, Current current__)
        {
            _stopCallback = false;
            print("receive client callback listener");

            if (faceResultList.ContainsKey("1"))
            {
                faceResultList["1"].proxy = listener;
            }

            #region test
            //Task.Factory.StartNew(() =>
            //{
            //    while (!_stopCallback)
            //    {
            //        var sb = new StringBuilder();

            //        sb.Append("xml".ElementBegin());
            //        sb.Append("type".ElementText("dynamicDetectResult"));
            //        sb.Append("rtspId".ElementText("1"));
            //        sb.Append("persons".ElementBegin());
            //        sb.Append("person".ElementBegin());
            //        sb.Append("imgData".ElementText(""));
            //        sb.Append("posX".ElementText("1"));
            //        sb.Append("posY".ElementText("2"));
            //        sb.Append("imgWidth".ElementText("3"));
            //        sb.Append("imgHeight".ElementText("4"));
            //        sb.Append("quality".ElementText("5"));
            //        sb.Append("person".ElementEnd());
            //        sb.Append("persons".ElementEnd());
            //        sb.Append("xml".ElementEnd());

            //        var data = sb.ToString();
            //        clientPxy.onRecv(data);
            //        Thread.Sleep(1000);
            //    }
            //});
            #endregion
        }

        public override string send(string xml, Current current__)
        {
            var content = "";
            var conn = current__.con;
            var endPoint = conn.getEndpoint();
            var ip = current__.con.ToString();

            print("ip->" + ip);
            content = ParseXml(xml);
            return content;
        }

        private string ParseXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var typename = doc.GetNodeText("type");

            print(typename);
            var content = "";
            switch (typename)
            {
                case "staticDetect": //静态人脸识别，返回图片内的人脸数据
                    content = staticDetect(doc);
                    break;
                case "dynamicDetect": //人脸动态识别接口
                    content = dynamicDetect(doc);
                    break;
                case "recvPerson":
                    content = recvPerson(doc);
                    break;
                case "shutdownDynamicDetect": //停止动态人脸识别
                    content = shutdownDynamicDetect(doc);
                    break;
                case "compare": //图片1：1比对
                    content = compare(doc);
                    break;
                case "convertSignatureCode": //特征码提取接口
                    content = convertSignatureCode(doc);
                    break;
                case "createOrUpdatePerson": //新增或更新人像库
                    content = createOrUpdatePerson(doc);
                    break;
                case "updatePersonTags": //更新人像库标签
                    content = updatePersonTags(doc);
                    break;
                case "deletePersonTags": //删除人像库标签
                    content = deletePersonTags(doc);
                    break;
                case "deletePerson": //删除单个人像
                    content = deletePerson(doc);
                    break;
                case "deletePersonsByTags": //批量删除人像
                    content = deletePersonsByTags(doc);
                    break;
                case "queryPersons": //查询人像
                    content = queryPersons(doc);
                    break;
                case "verifySignatureCode": //特征码1:N比对
                    content = verifySignatureCode(doc);
                    break;
                case "":
                    break;
            }
            return content;
        }

        private void print(string str)
        {
            Console.Out.WriteLine(str);
        }

        private XmlNodeList GetNodes(XmlDocument doc, string path)
        {
            var nodes = doc.SelectNodes("/xml/" + path);
            return nodes;
        }

        private string ResponseOk()
        {
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string staticDetect(XmlDocument doc)
        {
            var imgData = doc.GetNodeText("imgData");
            var threshold = doc.GetNodeText("threshold");
            var maxImageCount = doc.GetNodeText("maxImageCount");
            var image = Convert.FromBase64String(imgData);

            print("imgData->" + image.Length);
            print("threshold->" + threshold);
            print("maxImageCount->" + maxImageCount);

            var result = fs.Detect(image, false, false);
            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("code".ElementText("0"));
            sb.Append("persons".ElementBegin());

            var count = 0;
            if (result.Faces != null)
            {
                count = result.Faces.Length;
                var topFaces = result.Faces.OrderByDescending(s => s.Quality).Take(maxImageCount.ToInt32());
                foreach (var face in topFaces)
                {
                    sb.Append("person".ElementBegin());
                    sb.Append("imgData".ElementText(face.crop.image));
                    sb.Append("posX".ElementText(face.Rect.Left.ToString()));
                    sb.Append("posY".ElementText(face.Rect.Top.ToString()));
                    sb.Append("imgWidth".ElementText(face.Rect.Width.ToString()));
                    sb.Append("imgHeight".ElementText(face.Rect.Height.ToString()));
                    sb.Append("quality".ElementText(face.Quality.ToString()));
                    sb.Append("person".ElementEnd());
                }
            }
            print("检测数量->" + count);

            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }

        private string dynamicDetect(XmlDocument doc)
        {
            var threshold = doc.GetNodeText("threshold");
            var rtspId = doc.GetNodeText("rtspId");
            var rtspPath = doc.GetNodeText("rtspPath") + "user=admin&password=&channel=1&stream=0.sdp?";
            var type = doc.GetNodeText("responseType/type");
            var size = doc.GetNodeText("responseType/size");
            var maxImageCount = doc.GetNodeText("maxImageCount");
            var frames = doc.GetNodeText("frames");

            print("threshold->" + threshold);
            print("rtspId->" + rtspId);
            print("rtspPath->" + rtspPath);
            print("type->" + type);
            print("size->" + size);
            print("maxImageCount->" + maxImageCount);
            print("frames->" + frames);

            //fs.GetVideo(rtspPath, threshold.ToFloat(), rtspId, WebSocketcallback);

            MySocket websocket = new WebAPI.MySocket();
            websocket.OnFaceDetect += Websocket_OnFaceDetect;
            websocket.Run(rtspId, rtspPath, threshold.ToFloat());

            ClientData client = new ClientData()
            {
                messageType = type,
                socket = websocket,
                queue = new Queue<DynamicFaceResult>(size.ToInt32())
            };

            if (type == "messageQueue")
            {
                faceResultList.Add(rtspId, client);
            }

            return ResponseOk();
        }

        private void Websocket_OnFaceDetect(string rtspId, DynamicFaceResult face)
        {

        }

        private string recvPerson(XmlDocument doc)
        {
            lock (this)
            {
                //var queue = faceResultList["1"];
                //if (queue.Count > 0)
                //{
                //    var face = queue.Dequeue();
                //    var data = GetDynamicResutl(face);
                //    return data;
                //}
                //else
                //{
                //    Console.WriteLine("返回空数据");
                //    return GetEmpty();
                //}
            }
            return string.Empty;
        }

        private string shutdownDynamicDetect(XmlDocument doc)
        {
            lock (this)
            {
                _stopCallback = true;
                var rtspId = doc.GetNodeText("rtspId");
                print("rtspId->" + rtspId);
                if (faceResultList.ContainsKey(rtspId))
                {
                    faceResultList.Remove(rtspId);
                }
                return ResponseOk();
            }
        }

        private string compare(XmlDocument doc)
        {
            var sourceContent = doc.GetNodeText("srcImgData");
            var destContent = doc.GetNodeText("destImgData");

            var image1 = sourceContent.Base64ToByte();
            var image2 = destContent.Base64ToByte();

            print("image1 length=" + image1.Length);
            print("image2 length=" + image2.Length);

            var similarity = 0.8d;
            similarity = fs.Compare(image1, image2);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<similarity>" + similarity + "</similarity>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string convertSignatureCode(XmlDocument doc)
        {
            var imgData = doc.GetNodeText("imgData");
            var buffer = imgData.Base64ToByte();
            print("Image Length->" + buffer.Length);

            var feature = fs.Feature(buffer, 0.9f, false);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<signatureCode>" + feature.Feature + "</signatureCode>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string createOrUpdatePerson(XmlDocument doc)
        {
            var faceId = SaveToRepository(doc);
            SaveTag(faceId, doc);

            var imgData1 = doc.GetNodeText("imgData1");
            var signatureCode1 = doc.GetNodeText("signatureCode1");
            Post(faceId, signatureCode1, imgData1.Base64ToByte());

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<faceId>" + faceId + "</faceId>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string SaveToRepository(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            var code = doc.GetNodeText("code");
            var name = doc.GetNodeText("name");
            var descrption = doc.GetNodeText("descrption");
            var imgData1 = doc.GetNodeText("imgData1");
            var signatureCode1 = doc.GetNodeText("signatureCode1");
            var imgData2 = doc.GetNodeText("imgData2");
            var signatureCode2 = doc.GetNodeText("signatureCode2");
            var imgData3 = doc.GetNodeText("imgData3");
            var signatureCode3 = doc.GetNodeText("signatureCode3");

            print("uuid->" + uuid);
            print("code->" + code);
            print("name->" + name);
            print("descrption->" + descrption);

            person person = new person();
            person.FaceID = uuid; //Guid.NewGuid().ToString("N");
            person.UUID = uuid;
            person.Code = code;
            person.Name = name;
            person.Description = descrption;
            person.ImageData1 = imgData1;
            person.SignatureCode1 = signatureCode1;
            person.HasSignatureCode1 = signatureCode1.Length > 0;
            person.ImageData2 = imgData2;
            person.SignatureCode2 = signatureCode2;
            person.HasSignatureCode2 = signatureCode2.Length > 0;
            person.ImageData3 = imgData3;
            person.SignatureCode3 = signatureCode3;
            person.HasSignatureCode3 = signatureCode3.Length > 0;
            person.CreateTime = DateTime.Now;

            this.db.Add(person);

            return person.FaceID;
        }

        private void SaveTag(string faceId, XmlDocument doc)
        {
            var tagNodes = GetNodes(doc, "tags/tag");
            print("人物标签");

            List<string> tags = new List<string>();
            foreach (XmlNode t in tagNodes)
            {
                print("tag->" + t.InnerText);
                tags.Add(t.InnerText);
            }
            db.AddPersonTag(faceId, tags.ToArray());
        }

        private void Post(string tag, string feature, byte[] data)
        {
            fs.GroupPost(group, tag, "", 0, false, data);
        }

        private string updatePersonTags(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);

            print("更新人物标签");
            var tagNodes = GetNodes(doc, "tags/tag");

            List<string> tags = new List<string>();
            foreach (XmlNode tag in tagNodes)
            {
                print("tag->" + tag.InnerText);
                tags.Add(tag.InnerText);
            }
            db.UpdatePersonTag(uuid, tags.ToArray());
            return ResponseOk();
        }

        private string deletePersonTags(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);
            var tagNodes = GetNodes(doc, "tags/tag");

            List<string> tags = new List<string>();
            if (tagNodes.Count > 0)
            {
                print("删除人物的指定标签");
                foreach (XmlNode tag in tagNodes)
                {
                    print("tag->" + tag.InnerText);
                    tags.Add(tag.InnerText);
                }
            }
            else
            {
                print("删除人物的全部标签");
            }

            db.DeletePersonTag(uuid, tags.ToArray());
            return ResponseOk();
        }

        private string deletePerson(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);

            person p = new Repository.person { FaceID = uuid };
            db.Delete(p);

            return ResponseOk();
        }

        private string deletePersonsByTags(XmlDocument doc)
        {
            var tagNodes = GetNodes(doc, "tags/tag");
            print("批量删除以下标签对应的人物");

            List<string> tags = new List<string>();
            foreach (XmlNode tag in tagNodes)
            {
                print("tag->" + tag.InnerText);
                tags.Add(tag.InnerText);
            }

            var affectcount = db.DeleteByTags(tags.ToArray());

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<affectCount>" + affectcount + "</affectCount>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string queryPersons(XmlDocument doc)
        {
            var id = doc.GetNodeText("id");
            var uuid = doc.GetNodeText("uuid");
            var code = doc.GetNodeText("code");

            print("id->" + id);
            print("uuid->" + uuid);
            print("code->" + code);

            print("人物标签");
            List<string> tags = new List<string>();
            var tagNodes = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tagNodes)
            {
                print("tag->" + tag.InnerText);
                tags.Add(tag.InnerText);
            }

            var offset = doc.GetNodeText("offset");
            var size = doc.GetNodeText("size");

            print("offset->" + offset);
            print("size->" + size);

            Pagequery page = new Pagequery
            {
                Offset = offset.ToInt32(),
                Pagesize = size.ToInt32(),
            };

            var persons = db.Search(page, tags.ToArray());
            var count = page.TotalCount.ToString();
            print("返回" + count + "条记录");
            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("code".ElementText("0"));
            sb.Append("totalCount".ElementText(count));
            sb.Append("persons".ElementBegin());

            foreach (var p in persons)
            {
                sb.Append("person".ElementBegin());
                sb.Append("faceId".ElementText(p.FaceID));
                sb.Append("uuid".ElementText(p.UUID));
                sb.Append("code".ElementText(p.Code));
                sb.Append("name".ElementText(p.Name));
                sb.Append("descrption".ElementText(p.Description));
                sb.Append("imgData1".ElementText(p.ImageData1));
                sb.Append("hasSignatureCode1".ElementText(hasSignaturecode(p.SignatureCode1)));
                sb.Append("imgData2".ElementText(p.ImageData2));
                sb.Append("hasSignatureCode2".ElementText(hasSignaturecode(p.SignatureCode2)));
                sb.Append("imgData3".ElementText(p.ImageData3));
                sb.Append("hasSignatureCode3".ElementText(hasSignaturecode(p.SignatureCode3)));
                sb.Append("person".ElementEnd());
            }

            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }

        private string hasSignaturecode(string val)
        {
            if (val.IsEmpty())
                return "0";
            else
                return val.Length > 0 ? "1" : "0";
        }

        private string verifySignatureCode(XmlDocument doc)
        {
            var signatureCode = doc.GetNodeText("signatureCode");
            var threshold = doc.GetNodeText("threshold").ToFloat();
            var size = doc.GetNodeText("size").ToInt32();

            print("signatureCode->" + signatureCode);
            print("threshold->" + threshold);
            print("size->" + size);

            print("匹配标签");
            var tagNodes = GetNodes(doc, "tags/tag");
            List<string> tags = new List<string>();
            foreach (XmlNode tag in tagNodes)
            {
                print("tag->" + tag.InnerText);
                tags.Add(tag.InnerText);
            }

            var result = fs.Search(group, signatureCode, size, "", false, null);
            var filterResult = GetfilterID(result, threshold);

            var filterfaceID = filterResult.Select(s => s.faceId).ToArray();
            Pagequery page = new Pagequery()
            {
                Offset = 0,
                Pagesize = size,
            };
            //查询数据库
            var persons = db.Search1VN(page, filterfaceID, tags.ToArray());

            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("code".ElementText("0"));
            sb.Append("result".ElementBegin());

            foreach (var p in persons)
            {
                sb.Append("matchPerson".ElementBegin());
                sb.Append("similarity".ElementText(Getsimilarity(p.FaceID, filterResult).ToString()));
                sb.Append("faceId".ElementText(p.FaceID));
                sb.Append("uuid".ElementText(p.UUID));
                sb.Append("code".ElementText(p.Code));
                sb.Append("name".ElementText(p.Name));
                sb.Append("descrption".ElementText(p.Description));

                sb.Append("tags".ElementBegin());
                var temp = "国内旅客,国际旅客,黄种人".Split(',');
                foreach (var t in temp)
                {
                    sb.Append("tag".ElementText(t));
                }
                sb.Append("tags".ElementEnd());

                sb.Append("imgData1".ElementText(p.ImageData1));
                sb.Append("imgData2".ElementText("imgData2"));
                sb.Append("imgData3".ElementText("imgData3"));

                sb.Append("matchPerson".ElementEnd());
            }

            sb.Append("result".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }

        private float Getsimilarity(string faceId, List<FaceScore> list)
        {
            var face = list.FirstOrDefault(s => s.faceId == faceId);
            if (face != null)
                return face.score;
            else
                return 0.0f;
        }

        private List<FaceScore> GetfilterID(SearchResut result, float throshold)
        {
            List<FaceScore> list = new List<FaceScore>();
            throshold = throshold * 100;
            if (result.groups != null)
            {
                var group = result.groups.FirstOrDefault();
                if (group != null)
                    list = group.photos.Where(f => f.Score >= throshold).OrderByDescending(s => s.Score).
                        Select(s => new FaceScore
                        {
                            faceId = s.Tag,
                            score = s.Score
                        }).ToList();
            }
            return list;
        }

        private void WebSocketcallback(DynamicFaceResult face)
        {
            lock (this)
            {
                //var data = GetDynamicResutl(face);
                //faceResultList["1"].Enqueue(face);
                //this.clientPxy.onRecv(data);
            }
        }

        private string GetDynamicResutl(DynamicFaceResult face)
        {
            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("type".ElementText("dynamicDetectResult"));
            sb.Append("rtspId".ElementText("1"));
            sb.Append("persons".ElementBegin());
            sb.Append("person".ElementBegin());
            sb.Append("imgData".ElementText(face.Face.Image));
            sb.Append("posX".ElementText(face.Result.Face.Rect.Left.ToString()));
            sb.Append("posY".ElementText(face.Result.Face.Rect.Top.ToString()));
            sb.Append("imgWidth".ElementText(face.Result.Face.Rect.Width.ToString()));
            sb.Append("imgHeight".ElementText(face.Result.Face.Rect.Height.ToString()));
            sb.Append("quality".ElementText(face.Result.Face.Quality.ToString()));
            sb.Append("person".ElementEnd());
            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            var data = sb.ToString();
            return data;
        }

        /// <summary>
        /// 返回无人脸数据
        /// </summary>
        /// <returns></returns>
        private string GetEmpty()
        {
            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("type".ElementText("dynamicDetectResult"));
            sb.Append("rtspId".ElementText("1"));
            sb.Append("persons".ElementBegin());
            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            var data = sb.ToString();
            return data;
        }
    }

    public class FaceScore
    {
        public string faceId { get; set; }

        public float score { get; set; }
    }

    public class ClientData
    {
        public string messageType { get; set; }
        public MySocket socket { get; set; }
        public Queue<DynamicFaceResult> queue { get; set; }
        public ConnectionListenerPrx proxy { get; set; }
    }
}
