using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FaceRecognitionModule;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace FaceClientEx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ice.Communicator ic;
        private FaceRecognitionPrx facePxy = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void Item(string content)
        {
            Action act = () =>
            {
                lbResult.Items.Add(content);
            };

            if (Dispatcher.CheckAccess())
            {
                act();
            }
            else
            {
                this.Dispatcher.Invoke(act);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var args = new string[] { "" };
                ic = Ice.Util.initialize(ref args);
                if (ic == null)
                {
                    Debug.Assert(false, "初始化失败");
                    return;
                }
                Ice.ObjectPrx pxy = ic.stringToProxy("myface:default -p 9996");
                facePxy = FaceRecognitionPrxHelper.checkedCast(pxy);
                if (facePxy == null)
                {
                    Debug.Assert(false, "代理为空");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Assert(false, "初始化失败");
                return;
            }
        }

        private string GetXml(string type, string data)
        {
            var content = "<xml><type>" + type + "</type>" + data + "</xml>";
            return content;
        }

        private string Element(string name, string value)
        {
            var content = "<{0}>{1}</{0}>";
            content = string.Format(content, name, value);
            return content;
        }

        private void btnOneCompareOne_Click(object sender, RoutedEventArgs e)
        {
            var xml = GetXml("compare", "<srcImgData><![CDATA[{0}]]></srcImgData><destImgData><![CDATA[{1}]]></destImgData>");

            var buffer1 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\face.jpg");
            var image1 = Convert.ToBase64String(buffer1);

            var buffer2 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\face.jpg");
            var image2 = Convert.ToBase64String(buffer2);

            xml = string.Format(xml, image1, image2);

            var content = facePxy.send(xml);

            Item("image len->" + buffer1.Length);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.SelectSingleNode("/xml/code");
            var similarity = doc.SelectSingleNode("/xml/similarity");
            Item("code->" + code.InnerText);
            Item("similarity->" + similarity.InnerText);
        }

        private void btnstaticDetect_Click(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\face.jpg");
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append(Element("imgData", image1));
            sb.Append(Element("threshold", "0.5"));
            sb.Append(Element("maxImageCount", "56"));
            var data = sb.ToString();
            var xml = GetXml("staticDetect", data);
            var content = facePxy.send(xml);
        }

        private void btnconvertSignatureCode_Click(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\face.jpg");
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append(Element("imgData", image1));
            var data = sb.ToString();

            var xml = GetXml("convertSignatureCode", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.SelectSingleNode("/xml/code");
            var signatureCode = doc.SelectSingleNode("/xml/signatureCode");
            Item("code->" + code.InnerText);

            var buffer = Convert.FromBase64String(signatureCode.InnerText);
            Item("signatureCode->" + buffer.Length);
        }

        private void btnQueryPerson_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append(Element("id", "1113"));
            sb.Append(Element("uuid", "72297c8842604c059b05d28bfb11d10b"));
            sb.Append(Element("code", "350321198003212221"));
            sb.Append("<tags>");
            sb.Append(Element("tag", "WAY1"));
            sb.Append(Element("tag", "黄种"));
            sb.Append(Element("tag", "BLACK"));
            sb.Append("</tags>");
            sb.Append(Element("offset", "0"));
            sb.Append(Element("size", "10"));
            var data = sb.ToString();
            var xml = GetXml("queryPersons", data);
            var content = facePxy.send(xml);

            ShowQueryResult(content);
        }

        private void ShowQueryResult(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var code = doc.SelectSingleNode("/xml/code").InnerText;
            var totalCount = doc.SelectSingleNode("/xml/totalCount").InnerText;

            Item("code->" + code);
            Item("totalCount->" + totalCount);

            var persons = doc.SelectNodes("/xml/persons/person");
            foreach (XmlNode p in persons)
            {
                Item("人物信息");
                Item("faceId->" + p.SelectSingleNode("faceId").InnerText);
                Item("uuid->" + p.SelectSingleNode("uuid").InnerText);
                Item("code->" + p.SelectSingleNode("code").InnerText);
                Item("name->" + p.SelectSingleNode("name").InnerText);
                Item("descrption->" + p.SelectSingleNode("descrption").InnerText);
                Item("imgData1->" + p.SelectSingleNode("imgData1").InnerText);
                Item("imgData2->" + p.SelectSingleNode("imgData2").InnerText);
                Item("imgData3->" + p.SelectSingleNode("imgData3").InnerText);
            }
        }

        private void btncreateorUpdatePerson_Click(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\girl.jpg");
            var image1 = Convert.ToBase64String(buffer1);

            var buffer2 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\girl.jpg");
            var image2 = Convert.ToBase64String(buffer2);

            var buffer3 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\girl.jpg");
            var image3 = Convert.ToBase64String(buffer3);

            image1 = "";
            image2 = "";
            image3 = "";

            var sb = new StringBuilder();
            sb.Append(Element("uuid", "72297c8842604c059b05d28bfb11d10b"));
            sb.Append(Element("code", "350321198003212221"));
            sb.Append(Element("name", "黄测试"));
            sb.Append(Element("descrption", "{'race':'白人','gender':'男'}"));
            sb.Append(Element("imgData1", image1));
            sb.Append(Element("signatureCode1", image1));
            sb.Append(Element("imgData2", image2));
            sb.Append(Element("signatureCode2", image2));
            sb.Append(Element("imgData3", image3));
            sb.Append(Element("signatureCode3", image3));
            sb.Append("<tags>");
            sb.Append(Element("tag", "VIP"));
            sb.Append(Element("tag", "国内旅客"));
            sb.Append(Element("tag", "20161024-CZ3108"));
            sb.Append("</tags>");
            var data = sb.ToString();

            var xml = GetXml("createOrUpdatePerson", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.SelectSingleNode("/xml/code").InnerText;
            var faceId = doc.SelectSingleNode("/xml/faceId").InnerText;

            Item("code->" + code);
            Item("faceId->" + faceId);
        }

        private void btndeletePerson_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append((Element("uuid", "72297c8842604c059b05d28bfb11d10b")));
            var data = sb.ToString();
            var xml = GetXml("deletePerson", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.SelectSingleNode("/xml/code").InnerText;
            Item("code->" + code);
        }

        private void btndeletePersonTags_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append(Element("uuid", "72297c8842604c059b05d28bfb11d10b"));
            if (!ckbClearPersonTag.IsChecked.Value)
            {
                sb.Append("<tags>");
                sb.Append(Element("tag", "1"));
                sb.Append(Element("tag", "2"));
                sb.Append(Element("tag", "3"));
                sb.Append(Element("tag", "4"));
                sb.Append(Element("tag", "5"));
                sb.Append(Element("tag", "6"));
                sb.Append("</tags>");
            }
            var data = sb.ToString();
            var xml = GetXml("deletePersonTags", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.SelectSingleNode("/xml/code").InnerText;
            Item("code->" + code);
        }

        /// <summary>
        /// 按标签批量删除人像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btndeletePersonsByTags_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("<tags>");
            sb.Append(Element("tag", "VIP"));
            sb.Append(Element("tag", "国内旅客"));
            sb.Append(Element("tag", "国际旅客"));
            sb.Append(Element("tag", "黑人"));
            sb.Append("</tags>");
            var data = sb.ToString();
            var xml = GetXml("deletePersonsByTags", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.SelectSingleNode("/xml/code").InnerText;
            var affectCount = doc.SelectSingleNode("/xml/affectCount").InnerText;
            Item("code->" + code);
            Item("affectCount->" + affectCount);
        }

        Ice.ObjectAdapter callbackAdapter = null;
        private void btndynamicDetect_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append(Element("threshold", "0.5"));
            sb.Append(Element("rtspId", "1"));
            sb.Append(Element("rtspPath", "rtsp://admin:12345@192.0.0.64:554/h264/ch1/main/av_stream"));
            sb.Append("<responseType>");
            if (ckbCallback.IsChecked.Value)
                sb.Append(Element("type", "callback"));
            else
                sb.Append(Element("type", "messageQueue"));

            sb.Append(Element("size", "10"));
            sb.Append("</responseType>");
            sb.Append(Element("maxImageCount", "3"));
            sb.Append(Element("frames", "5"));
            var data = sb.ToString();
            var xml = GetXml("dynamicDetect", data);

            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.SelectSingleNode("/xml/code").InnerText;

            Item("code->" + code);

            if (ckbCallback.IsChecked.Value)
            {
                callbackAdapter = ic.createObjectAdapterWithEndpoints("callback-receiver", "default");
                Ice.Object callbackServant = new ConnectorDisp(Item);

                callbackAdapter.add(callbackServant, ic.stringToIdentity("callbackReceiver"));
                callbackAdapter.activate();

                ConnectionListenerPrx listenerPxy = null;

                var objectPxy = callbackAdapter.createProxy(ic.stringToIdentity("callbackReceiver"));
                listenerPxy = ConnectionListenerPrxHelper.checkedCast(objectPxy);

                facePxy.initConnectionListener(listenerPxy);

                Item("set callback ok");
            }
        }

        private bool stopPool = false;
        private void btnqueryQueue_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                while (!stopPool)
                {
                    var xml = GetXml("recvPerson", "");
                    var content = facePxy.send(xml);
                    Item("query queue back->" + content);

                    var wait = Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                    });
                    wait.Wait();
                }
            });
        }

        private void btnstopDynamicDetect_Click(object sender, RoutedEventArgs e)
        {
            stopPool = true;
            if (callbackAdapter != null)
            {
                callbackAdapter.destroy();
                callbackAdapter = null;
            }

            var sb = new StringBuilder();
            sb.Append(Element("rtspId", "9999"));
            var data = sb.ToString();
            var xml = GetXml("shutdownDynamicDetect", data);
            var content = facePxy.send(xml);
        }
        /// <summary>
        /// 特征码1:N比对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnverifySignaturecode_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            var data = sb.ToString();
            var xml = GetXml("verifySignatureCode", data);
            var content = facePxy.send(xml);
        }
    }

    public class compareResult
    {
        public int code { get; set; }
        public float similarity { get; set; }
    }
}
