using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionModule;
using Ice;
using System.IO;
using System.Xml;
using System.Threading;

namespace serverEx
{
    class MyFace : FaceRecognitionDisp_
    {
        private ConnectionListenerPrx clientPxy = null;
        private Queue<string> queue = new Queue<string>();

        private bool stopCallback = false;

        public MyFace()
        {
            Out("create a server object");
        }

        private string ElementBegin(string name)
        {
            return "<" + name + ">";
        }

        private string ElementEnd(string name)
        {
            return "</" + name + ">";
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
            stopCallback = false;
            Out("receive client callback listener");

            Task.Factory.StartNew(() =>
            {
                while (!stopCallback)
                {
                    listener.onRecv("终于等到你->" + DateTime.Now);
                    Thread.Sleep(1000);
                }
            });
        }

        public override string send(string xml, Current current__)
        {
            var content = "";
            content = ParseXml(xml);
            return content;
        }

        private string ParseXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var type = doc.SelectSingleNode("/xml/type");
            var typename = type.InnerText;

            Out(typename);
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

        private void Out(string str)
        {
            Console.Out.WriteLine(str);
        }

        private string GetNodeValue(XmlDocument doc, string path)
        {
            var node = doc.SelectSingleNode("/xml/" + path);
            if (node == null)
            {
                Out("节点查询为null->" + path);
                return string.Empty;
            }
            else
            {
                return node.InnerText;
            }
        }

        private XmlNodeList GetNodes(XmlDocument doc, string path)
        {
            var nodes = doc.SelectNodes("/xml/" + path);
            return nodes;
        }

        private string staticDetect(XmlDocument doc)
        {
            var imgData = GetNodeValue(doc, "imgData");
            var threshold = GetNodeValue(doc, "threshold");
            var maxImageCount = GetNodeValue(doc, "maxImageCount");
            var image = Convert.FromBase64String(imgData);

            Out("imgData->" + image.Length);
            Out("threshold->" + threshold);
            Out("maxImageCount->" + maxImageCount);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string dynamicDetect(XmlDocument doc)
        {
            var threshold = GetNodeValue(doc, "threshold");
            var rtspId = GetNodeValue(doc, "rtspId");
            var rtspPath = GetNodeValue(doc, "rtspPath");
            var type = GetNodeValue(doc, "responseType/type");
            var size = GetNodeValue(doc, "responseType/size");
            var maxImageCount = GetNodeValue(doc, "maxImageCount");
            var frames = GetNodeValue(doc, "frames");

            Out("threshold->" + threshold);
            Out("rtspId->" + rtspId);
            Out("rtspPath->" + rtspPath);
            Out("type->" + type);
            Out("size->" + size);
            Out("maxImageCount->" + maxImageCount);
            Out("frames->" + frames);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string recvPerson(XmlDocument doc)
        {
            var sb = new StringBuilder();
            sb.Append("recvPerson->" + DateTime.Now.ToString());
            return sb.ToString();
        }

        private string shutdownDynamicDetect(XmlDocument doc)
        {
            stopCallback = true;

            var rtspId = GetNodeValue(doc, "rtspId");

            Out("rtspId->" + rtspId);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string compare(XmlDocument doc)
        {
            var sourceContent = GetNodeValue(doc, "srcImgData");
            var destContent = GetNodeValue(doc, "destImgData");

            var image1 = Convert.FromBase64String(sourceContent);
            var image2 = Convert.FromBase64String(destContent);

            Out("image1 length=" + image1.Length);
            Out("image2 length=" + image2.Length);

            var similarity = CloudAPI.MegviiCloud.Compare(image1, image2);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<similarity>" + similarity + "</similarity>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string convertSignatureCode(XmlDocument doc)
        {
            var imgData = GetNodeValue(doc, "imgData");
            var buffer = Convert.FromBase64String(imgData);
            Out("Image Length->" + buffer.Length);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<signatureCode>" + imgData + "</signatureCode>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string createOrUpdatePerson(XmlDocument doc)
        {
            var uuid = GetNodeValue(doc, "uuid");
            var code = GetNodeValue(doc, "code");
            var name = GetNodeValue(doc, "name");
            var descrption = GetNodeValue(doc, "descrption");
            var imgData1 = GetNodeValue(doc, "imgData1");
            var signatureCode1 = GetNodeValue(doc, "signatureCode1");
            var imgData2 = GetNodeValue(doc, "imgData2");
            var signatureCode2 = GetNodeValue(doc, "signatureCode2");
            var imgData3 = GetNodeValue(doc, "imgData3");
            var signatureCode3 = GetNodeValue(doc, "signatureCode3");

            var tag1 = GetNodeValue(doc, "tags/tag[1]");
            var tag2 = GetNodeValue(doc, "tags/tag[2]");
            var tag3 = GetNodeValue(doc, "tags/tag[3]");

            Out("uuid->" + uuid);
            Out("code->" + code);
            Out("name->" + name);
            Out("descrption->" + descrption);
            Out("tag1->" + tag1);
            Out("tag2->" + tag2);
            Out("tag3->" + tag3);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("<faceId>are you ok?</faceId>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string updatePersonTags(XmlDocument doc)
        {
            var uuid = GetNodeValue(doc, "uuid");
            Out("uuid->" + uuid);
            var tags = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tags)
            {
                Out("tag->" + tag.InnerText);
            }
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string deletePersonTags(XmlDocument doc)
        {
            var uuid = GetNodeValue(doc, "uuid");
            Out("uuid->" + uuid);
            var tags = GetNodes(doc, "tags/tag");
            if (tags.Count > 0)
            {
                Out("删除人物的指定标签");
                foreach (XmlNode tag in tags)
                {
                    Out("tag->" + tag.InnerText);
                }
            }
            else
            {
                Out("删除人物的全部标签");
            }
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string deletePerson(XmlDocument doc)
        {
            var uuid = GetNodeValue(doc, "uuid");
            Out("uuid->" + uuid);

            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<code>0</code>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        private string deletePersonsByTags(XmlDocument doc)
        {
            var tags = GetNodes(doc, "tags/tag");
            Out("批量删除以下标签对应的人物");
            foreach (XmlNode tag in tags)
            {
                Out("tag->" + tag.InnerText);
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
            var id = GetNodeValue(doc, "id");
            var uuid = GetNodeValue(doc, "uuid");
            var code = GetNodeValue(doc, "code");

            Out("id->" + id);
            Out("uuid->" + uuid);
            Out("code->" + code);

            Out("人物标签");
            var tags = GetNodes(doc, "tags/tag");
            foreach (XmlNode tag in tags)
            {
                Out("tag->" + tag.InnerText);
            }

            var offset = GetNodeValue(doc, "offset");
            var size = GetNodeValue(doc, "size");

            Out("offset->" + offset);
            Out("size->" + size);


            Out("返回10条记录");
            var sb = new StringBuilder();
            sb.Append(ElementBegin("xml"));
            sb.Append(Element("code", "0"));
            sb.Append(Element("totalCount", "5689"));
            sb.Append(ElementBegin("persons"));
            for (int i = 0; i < 10; i++)
            {
                sb.Append(ElementBegin("person"));
                sb.Append(Element("faceId", "1112"));
                sb.Append(Element("uuid", "72297c8842604c059b05d28bfb11d10b"));
                sb.Append(Element("code", "350321198003212221"));
                sb.Append(Element("name", "杨绍杰"));
                sb.Append(Element("descrption", "{'race:白人','gender':'男'}"));
                sb.Append(Element("imgData1", "imgData1"));
                sb.Append(Element("imgData2", "imgData2"));
                sb.Append(Element("imgData3", "imgData3"));
                sb.Append(ElementEnd("person"));
            }
            sb.Append(ElementEnd("persons"));
            sb.Append(ElementEnd("xml"));
            return sb.ToString();
        }

        private string verifySignatureCode(XmlDocument doc)
        {
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
