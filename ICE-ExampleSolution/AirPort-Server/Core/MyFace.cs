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
namespace AirPort.Server.Core
{
    class MyFace : FaceRecognitionDisp_
    {
        private bool _stopCallback = false;

        private ConnectionListenerPrx clientPxy = null;
        private Queue<string> queue = new Queue<string>();

        private FaceServices fs = null;

        public MyFace()
        {
            fs = new FaceServices();
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
            clientPxy = listener;
            _stopCallback = false;
            print("receive client callback listener");

            Task.Factory.StartNew(() =>
            {
                while (!_stopCallback)
                {
                    listener.onRecv("终于等到你->" + DateTime.Now);
                    Thread.Sleep(1000);
                }
            });
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

            var count = result.Faces.Length;
            print("检测数量->" + count);
            foreach (var face in result.Faces)
            {
                //for (int i = 0; i < count; i++)
                //{
                sb.Append("person".ElementBegin());

                sb.Append("imgData".ElementText(face.crop.image));
                sb.Append("posX".ElementText(face.Rect.Left.ToString()));
                sb.Append("posY".ElementText(face.Rect.Top.ToString()));
                sb.Append("imgWidth".ElementText(face.Rect.Width.ToString()));
                sb.Append("imgHeight".ElementText(face.Rect.Height.ToString()));
                sb.Append("quality".ElementText(face.Quality.ToString()));

                sb.Append("person".ElementEnd());
                //}
            }

            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }

        private string dynamicDetect(XmlDocument doc)
        {
            var threshold = doc.GetNodeText("threshold");
            var rtspId = doc.GetNodeText("rtspId");
            var rtspPath = doc.GetNodeText("rtspPath");
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

            return ResponseOk();
        }

        private string recvPerson(XmlDocument doc)
        {
            return ResponseOk();
        }

        private string shutdownDynamicDetect(XmlDocument doc)
        {
            _stopCallback = true;
            var rtspId = doc.GetNodeText("rtspId");
            print("rtspId->" + rtspId);

            return ResponseOk();
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

            var tags = GetNodes(doc, "tags/tag");
            print("人物标签");
            foreach (XmlNode tag in tags)
            {
                print("tag->" + tag.InnerText);
            }

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<faceId>are you ok?</faceId>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string updatePersonTags(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);

            print("更新人物标签");
            var tags = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tags)
            {
                print("tag->" + tag.InnerText);
            }

            return ResponseOk();
        }

        private string deletePersonTags(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);
            var tags = GetNodes(doc, "tags/tag");
            if (tags.Count > 0)
            {
                print("删除人物的指定标签");
                foreach (XmlNode tag in tags)
                {
                    print("tag->" + tag.InnerText);
                }
            }
            else
            {
                print("删除人物的全部标签");
            }

            return ResponseOk();
        }

        private string deletePerson(XmlDocument doc)
        {
            var uuid = doc.GetNodeText("uuid");
            print("uuid->" + uuid);

            return ResponseOk();
        }

        private string deletePersonsByTags(XmlDocument doc)
        {
            var tags = GetNodes(doc, "tags/tag");
            print("批量删除以下标签对应的人物");
            foreach (XmlNode tag in tags)
            {
                print("tag->" + tag.InnerText);
            }

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<affectCount>" + new Random().Next(100, 900) + "</affectCount>");
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
            var tags = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tags)
            {
                print("tag->" + tag.InnerText);
            }

            var offset = doc.GetNodeText("offset");
            var size = doc.GetNodeText("size");

            print("offset->" + offset);
            print("size->" + size);


            print("返回10条记录");
            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append(Element("code", "0"));
            sb.Append(Element("totalCount", "5689"));
            sb.Append("persons".ElementBegin());
            for (int i = 0; i < 10; i++)
            {
                sb.Append("person".ElementBegin());
                sb.Append(Element("faceId", "1112"));
                sb.Append(Element("uuid", "72297c8842604c059b05d28bfb11d10b"));
                sb.Append(Element("code", "350321198003212221"));
                sb.Append(Element("name", "杨绍杰"));
                sb.Append(Element("descrption", "{'race:白人','gender':'男'}"));
                sb.Append(Element("imgData1", "imgData1"));
                sb.Append(Element("imgData2", "imgData2"));
                sb.Append(Element("imgData3", "imgData3"));
                sb.Append("person".ElementEnd());
            }
            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }

        private string verifySignatureCode(XmlDocument doc)
        {
            var signatureCode = doc.GetNodeText("signatureCode");
            var threshold = doc.GetNodeText("threshold");
            var size = doc.GetNodeText("size");

            print("signatureCode->" + signatureCode);
            print("threshold->" + threshold);
            print("size->" + size);

            print("匹配标签");
            var tags = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tags)
            {
                print("tag->" + tag.InnerText);
            }

            var sb = new StringBuilder();
            sb.Append("xml".ElementBegin());
            sb.Append("code".ElementText("0"));
            sb.Append("result".ElementBegin());

            for (int i = 0; i < 10; i++)
            {
                sb.Append("matchPerson".ElementBegin());
                sb.Append("similarity".ElementText("0.81"));
                sb.Append("faceId".ElementText("123"));
                sb.Append("uuid".ElementText("72297c8842604c059b05d28bfb11d10b"));
                sb.Append("code".ElementText("350321198003212221"));
                sb.Append("name".ElementText("杨绍杰"));
                sb.Append("descrption".ElementText("{\"race:\":\"黄种\"}"));

                sb.Append("tags".ElementBegin());
                var temp = "国内旅客,国际旅客,黄种人".Split(',');
                foreach (var t in temp)
                {
                    sb.Append("tag".ElementText(t));
                }
                sb.Append("tags".ElementEnd());

                sb.Append("imgData1".ElementText("imgData1"));
                sb.Append("imgData2".ElementText("imgData2"));
                sb.Append("imgData3".ElementText("imgData3"));

                sb.Append("matchPerson".ElementEnd());
            }
            sb.Append("result".ElementEnd());
            sb.Append("xml".ElementEnd());
            return sb.ToString();
        }
    }
}
