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
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

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

        private Connect connect = null;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region code run
            //try
            //{
            //    var args = new string[] { "" };
            //    Ice.Properties properties = Ice.Util.createProperties();
            //    //单位KB
            //    properties.setProperty("Ice.MessageSizeMax", "2048");
            //    Ice.InitializationData initData = new Ice.InitializationData();
            //    initData.properties = properties;
            //    ic = Ice.Util.initialize(initData);

            //    if (ic == null)
            //    {
            //        Debug.Assert(false, "初始化失败");
            //        return;
            //    }
            //    Ice.ObjectPrx pxy = ic.stringToProxy("myface:tcp -h localhost -p 9996");
            //    facePxy = FaceRecognitionPrxHelper.checkedCast(pxy);
            //    if (facePxy == null)
            //    {
            //        Debug.Assert(false, "代理为空");
            //        return;
            //    }
            //}
            //catch (System.Exception)
            //{
            //    Debug.Assert(false, "初始化失败");
            //    return;
            //} 
            #endregion

            #region config run
            Task.Factory.StartNew(() =>
            {
                connect = new FaceClientEx.Connect();
                var args = new string[] { "ysj" };
                connect.main(args, "config.client");
            });

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                facePxy = connect.pxy;
            });

            #endregion
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
            try
            {
                var xml = GetXml("compare", "<srcImgData><![CDATA[{0}]]></srcImgData><destImgData><![CDATA[{1}]]></destImgData>");

                var buffer1 = System.IO.File.ReadAllBytes(@"f:\face.jpg");
                //var buffer1 = new byte[298250];
                var image1 = Convert.ToBase64String(buffer1);

                var buffer2 = System.IO.File.ReadAllBytes(@"f:\face.jpg");
                var image2 = Convert.ToBase64String(buffer2);

                xml = string.Format(xml, image1, image2);

                var temp = System.Text.Encoding.UTF8.GetBytes(xml);

                var x = 1024 * 1024 - temp.Length;

                //var content = facePxy.send(xml);

                Stopwatch sw = Stopwatch.StartNew();
                Ice.AsyncResult result = facePxy.begin_send(xml).whenCompleted((arg) =>
                {
                    sw.Stop();
                    var ss = sw.ElapsedMilliseconds;
                    var content = arg;
                    Item("back->" + ss);
                },
                (ex) =>
                {
                    var msg = ex.StackTrace;
                    Item("error->" + msg);
                });

                Item("image len->" + buffer1.Length);

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(content);
                //var code = doc.GetNodeText("code");
                //var similarity = doc.GetNodeText("similarity");
                //Item("code->" + code);
                //Item("similarity->" + similarity);
            }
            catch (Exception)
            {
                MessageBox.Show("error");
            }
        }

        private void btnstaticDetect_Click(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(@"C:\Users\ysj\Desktop\Face\face.jpg");
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(image1));
            sb.Append("threshold".ElementText("0.5"));
            sb.Append("maxImageCount".ElementText("56"));
            var data = sb.ToString();

            var xml = GetXml("staticDetect", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.GetNodeText("code");
            Item("code->" + code);

            var persons = doc.SelectNodes("/xml/persons/person");
            Item("人脸数量->" + persons.Count);
            foreach (XmlNode f in persons)
            {
                Item("imgData->" + f.GetNodeText("imgData"));
                Item("imgWidth->" + f.GetNodeText("imgWidth"));
                Item("imgHeight->" + f.GetNodeText("imgHeight"));
                Item("posX->" + f.GetNodeText("posX"));
                Item("posY->" + f.GetNodeText("posY"));
                Item("quality->" + f.GetNodeText("quality"));
            }
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
            var code = doc.GetNodeText("code");
            var signatureCode = doc.GetNodeText("signatureCode");
            var buffer = Convert.FromBase64String(signatureCode);

            Item("code->" + code);
            Item("signatureCode->" + buffer.Length);
        }

        private void btnQueryPerson_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("id".ElementText("1113"));
            sb.Append("uuid".ElementText("72297c8842604c059b05d28bfb11d10b"));
            sb.Append("code".ElementText("350321198003212221"));
            sb.Append("tags".ElementBegin());
            sb.Append("tag".ElementText("WAY1"));
            sb.Append("tag".ElementText("黄种"));
            sb.Append("tag".ElementText("BLACK"));
            sb.Append("tags".ElementEnd());
            sb.Append("offset".ElementText("0"));
            sb.Append("size".ElementText("10"));
            var data = sb.ToString();
            var xml = GetXml("queryPersons", data);
            var content = facePxy.send(xml);

            ShowQueryResult(content);
        }

        private void ShowQueryResult(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var code = doc.GetNodeText("code");
            var totalCount = doc.GetNodeText("totalCount");

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
            sb.Append("uuid".ElementText("72297c8842604c059b05d28bfb11d10b"));
            sb.Append("code".ElementText("350321198003212221"));
            sb.Append("name".ElementText("黄测试"));
            sb.Append("descrption".ElementText("{'race':'白人','gender':'男'}"));
            sb.Append("imgData1".ElementText(image1));
            sb.Append("signatureCode1".ElementText(image1));
            sb.Append("imgData2".ElementText(image2));
            sb.Append("signatureCode2".ElementText(image2));
            sb.Append("imgData3".ElementText(image3));
            sb.Append("signatureCode3".ElementText(image3));
            sb.Append("tags".ElementBegin());
            sb.Append("tag".ElementText("VIP"));
            sb.Append("tag".ElementText("国内旅客"));
            sb.Append("tag".ElementText("20161024-CZ3108"));
            sb.Append("tags".ElementEnd());
            var data = sb.ToString();

            var xml = GetXml("createOrUpdatePerson", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.GetNodeText("code");
            var faceId = doc.GetNodeText("faceId");

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

            var code = doc.GetNodeText("code");
            Item("code->" + code);
        }

        private void btndeletePersonTags_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("uuid".ElementText("72297c8842604c059b05d28bfb11d10b"));
            if (!ckbClearPersonTag.IsChecked.Value)
            {
                sb.Append("tags".ElementBegin());
                sb.Append("tag".ElementText("1"));
                sb.Append("tag".ElementText("2"));
                sb.Append("tag".ElementText("3"));
                sb.Append("tag".ElementText("4"));
                sb.Append("tag".ElementText("5"));
                sb.Append("tag".ElementText("6"));
                sb.Append("tags".ElementEnd());
            }
            var data = sb.ToString();
            var xml = GetXml("deletePersonTags", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.GetNodeText("code");
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
            sb.Append("tags".ElementBegin());
            sb.Append("tag".ElementText("VIP"));
            sb.Append("tag".ElementText("国内旅客"));
            sb.Append("tag".ElementText("国际旅客"));
            sb.Append("tag".ElementText("黑人"));
            sb.Append("tags".ElementEnd());
            var data = sb.ToString();

            var xml = GetXml("deletePersonsByTags", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var code = doc.GetNodeText("code");
            var affectCount = doc.GetNodeText("affectCount");
            Item("code->" + code);
            Item("affectCount->" + affectCount);
        }

        Ice.ObjectAdapter callbackAdapter = null;
        private void btndynamicDetect_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("threshold".ElementText("0.5"));
            sb.Append("rtspId".ElementText("1"));
            sb.Append("rtspPath".ElementText("rtsp://admin:12345@192.0.0.64:554/h264/ch1/main/av_stream"));
            sb.Append("responseType".ElementBegin());
            if (ckbCallback.IsChecked.Value)
                sb.Append("type".ElementText("callback"));
            else
                sb.Append("type".ElementText("messageQueue"));

            sb.Append("size".ElementText("10"));
            sb.Append("responseType".ElementEnd());
            sb.Append("maxImageCount".ElementText("3"));
            sb.Append("frames".ElementText("5"));
            var data = sb.ToString();
            var xml = GetXml("dynamicDetect", data);

            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.GetNodeText("code");
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

                    Item("query next...");
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
            sb.Append("rtspId".ElementText("9999"));
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

            sb.Append("signatureCode".ElementText("signatureCode"));
            sb.Append("threshold".ElementText("0.56"));
            sb.Append("size".ElementText("100"));

            sb.Append("tags".ElementBegin());
            sb.Append("tag".ElementText("1"));
            sb.Append("tag".ElementText("2"));
            sb.Append("tags".ElementEnd());

            var data = sb.ToString();
            var xml = GetXml("verifySignatureCode", data);
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.GetNodeText("code");
            Item("code->" + code);


            var persons = doc.SelectNodes("/xml/result/matchPerson");
            Item("匹配人物数量->" + persons.Count);

            foreach (XmlNode p in persons)
            {
                Item("similarity->" + p.GetNodeText("similarity"));
                Item("faceId->" + p.GetNodeText("faceId"));
                Item("uuid->" + p.GetNodeText("uuid"));
                Item("name->" + p.GetNodeText("name"));
                Item("descrption->" + p.GetNodeText("descrption"));

                var tags = p.SelectNodes("tags/tag");
                Item("人物标签数量->" + tags.Count);
                foreach (XmlNode tag in tags)
                {
                    Item("tag->" + tag.InnerText);
                }

                Item("人脸1->" + p.GetNodeText("imgData1"));
                Item("人脸2->" + p.GetNodeText("imgData2"));
                Item("人脸3->" + p.GetNodeText("imgData3"));
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lbResult.Items.Clear();
        }
    }
}
