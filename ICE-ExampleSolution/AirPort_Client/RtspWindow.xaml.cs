using AirPort.Client.Core;
using Common;
using FaceRecognitionModule;
using System;
using System.Collections.Generic;
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

namespace AirPort.Client
{
    /// <summary>
    /// 人脸动态监测
    /// </summary>
    public partial class RtspWindow
    {
        private const string callback = "回调";

        public RtspWindow()
        {
            InitializeComponent();
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

        private bool stopPool = false;
        private Ice.ObjectAdapter callbackAdapter = null;

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
                    Loopquery();
                }
                btnStart.IsEnabled = false;
            }
        }

        private void Setcallback()
        {
            callbackAdapter = FaceServices.FaceProxy.Ic.createObjectAdapterWithEndpoints("callback-receiver", "default");
            Ice.Object callbackServant = new ConnectorDisp(Item);

            callbackAdapter.add(callbackServant, FaceServices.FaceProxy.Ic.stringToIdentity("callbackReceiver"));
            callbackAdapter.activate();

            ConnectionListenerPrx listenerPxy = null;

            var objectPxy = callbackAdapter.createProxy(FaceServices.FaceProxy.Ic.stringToIdentity("callbackReceiver"));
            listenerPxy = ConnectionListenerPrxHelper.checkedCast(objectPxy);

            FaceServices.FaceProxy.initConnectionListener(listenerPxy);

            Item("set callback ok");
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
                    Item("query queue back->" + content);

                    Thread.Sleep(1000);
                    Item("query next...");
                }
            });
        }

        private void btnStop_click(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
