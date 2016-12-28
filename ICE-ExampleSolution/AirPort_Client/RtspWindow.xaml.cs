using AirPort.Client.Core;
using AirPort.Client.Model;
using Common;
using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.ComponentModel;

namespace AirPort.Client
{
    /// <summary>
    /// 人脸动态监测
    /// </summary>
    public partial class RtspWindow
    {
        private const string callback = "回调";

        private int facecount = 0;
        private bool stopPool = false;
        private Ice.ObjectAdapter callbackAdapter = null;
        public RtspWindow()
        {
            InitializeComponent();
            this.txtrtsp.Text = "rtsp://192.168.1.151/live1.sdp";
        }

        private bool CheckInput()
        {
            if (txtrtspId.Text.IsEmpty())
            {
                WarnDialog("请输入标识！");
                return false;
            }
            if (txtrtsp.Text.IsEmpty())
            {
                WarnDialog("请输入地址！");
                return false;
            }
            return true;
        }

        private void btnStart_click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append("threshold".ElementText(txtthreshold.Text));
            sb.Append("rtspId".ElementText(txtrtspId.Text));
            sb.Append("rtspPath".ElementText(txtrtsp.Text));
            sb.Append("responseType".ElementBegin());

            var messageType = ((ComboBoxItem)cmbMessageType.SelectedItem).Content.ToString();
            if (messageType == callback)
                sb.Append("type".ElementText("callback"));
            else
                sb.Append("type".ElementText("messageQueue"));

            sb.Append("size".ElementText(txtsize.Text));
            sb.Append("responseType".ElementEnd());
            sb.Append("maxImageCount".ElementText("3"));
            sb.Append("frames".ElementText(txtframe.Text));
            var data = sb.ToString();
            var xml = XmlParse.GetXml("dynamicDetect", data);
            var content = FaceServices.FaceProxy.send(xml);
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            Item("code->" + code);
            if (code.ToInt32() == status_ok)
            {
                if (messageType == callback)
                {
                    Setcallback();
                }
                else
                {
                    stopPool = false;
                    Loopquery();
                }
                btnStart.IsEnabled = false;
            }
        }

        private void Setcallback()
        {
            callbackAdapter = FaceServices.FaceProxy.Ic.createObjectAdapterWithEndpoints("callback-receiver", "default");
            Ice.Object callbackServant = new ConnectorDisp(FaceBack);

            callbackAdapter.add(callbackServant, FaceServices.FaceProxy.Ic.stringToIdentity("callbackReceiver"));
            callbackAdapter.activate();

            ConnectionListenerPrx listenerPxy = null;

            var objectPxy = callbackAdapter.createProxy(FaceServices.FaceProxy.Ic.stringToIdentity("callbackReceiver"));
            listenerPxy = ConnectionListenerPrxHelper.checkedCast(objectPxy);

            FaceServices.FaceProxy.initConnectionListener(listenerPxy);

            Item("set callback ok");
        }

        private void FaceBack(string content)
        {
            try
            {
                var doc = XmlParse.LoadXml(content);
                var rtspId = doc.GetNodeText("rtspId");
                var personNodes = doc.SelectNodes("/xml/persons/person");
                if (personNodes.Count > 0)
                {
                    facecount++;
                    foreach (XmlNode n in personNodes)
                    {
                        var data = n.SelectSingleNode("imgData").InnerText;
                        var width = n.SelectSingleNode("imgWidth").InnerText;
                        var height = n.SelectSingleNode("imgHeight").InnerText;
                        var posX = n.SelectSingleNode("posX").InnerText;
                        var posY = n.SelectSingleNode("posY").InnerText;
                        var quality = n.SelectSingleNode("quality").InnerText;

                        this.Dispatcher.Invoke(() =>
                        {
                            lblfacecount.Content = "数量:" + facecount;
                            lblquality.Content = "质量:" + quality.FormatFloat();
                            lblrect.Content = string.Format("大小:left={0},top={1},width={2},height={3}", posX, posY, width, height);
                            var imagesource = ByteArrayToBitmapImage(data.Base64ToByte());
                            faceImage.Source = imagesource;
                        });
                        break;
                    }
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lblquality.Content = "没有人脸数据";
                        lblrect.Content = "";
                        faceImage.Source = null;
                    });
                }
            }
            catch
            {
            }
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }

        private void Loopquery()
        {
            Task.Factory.StartNew(() =>
            {
                while (!stopPool)
                {
                    var xml = XmlParse.GetXml("recvPerson", "");
                    var content = FaceServices.FaceProxy.send(xml);
                    if (content.IsEmpty())
                    {
                        continue;
                    }
                    Item("query queue back->");
                    FaceBack(content);
                    Thread.Sleep(500);
                    Item("query next...");
                }
            });
        }

        private void btnStop_click(object sender, RoutedEventArgs e)
        {
            ShutdownDynamicDetect();
        }

        /// <summary>
        /// 关闭窗口时
        /// </summary>
        private void ShutdownDynamicDetect()
        {
            facecount = 0;
            stopPool = true;
            if (callbackAdapter != null)
            {
                callbackAdapter.destroy();
                callbackAdapter = null;
            }

            var sb = new StringBuilder();
            sb.Append("rtspId".ElementText(txtrtspId.Text));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("shutdownDynamicDetect", data);
            var content = FaceServices.FaceProxy.send(xml);
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() == status_ok)
            {
                btnStart.IsEnabled = true;
            }

            lblfacecount.Content = "";
            lblquality.Content = "";
            lblrect.Content = "";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ShutdownDynamicDetect();
            base.OnClosing(e);
        }
    }
}
