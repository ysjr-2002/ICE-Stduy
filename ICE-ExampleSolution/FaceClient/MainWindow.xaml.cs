using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ice;
using FaceRecognitionModule;
using System.Diagnostics;
using System.Threading;

namespace FaceClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ice.Communicator ic = null;
        private FaceRecognitionPrx facePxy = null;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private Client client = null;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = new string[1] { "arg" };
            //try
            //{
            //    ic = Ice.Util.initialize(ref args);
            //    if (ic == null)
            //    {
            //        Debug.Assert(false, "初始化失败");
            //        return;
            //    }

            //    Ice.ObjectPrx pxy = ic.stringToProxy("myface:default -p 9996");
            //    facePxy = FaceRecognitionPrxHelper.checkedCast(pxy);
            //    if (facePxy == null)
            //    {
            //        Debug.Assert(false, "代理为空");
            //        return;
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.Assert(false, "初始化失败");
            //    return;
            //}

            Task.Factory.StartNew(() =>
            {
                client = new Client(Item);
                client.main(args, "config.client");
            });

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                this.ic = client.ic;
                this.facePxy = client.facePxy;
            });
        }

        private string GetUUID()
        {
            return Guid.NewGuid().ToString("N");
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

        private void btnOneCompareOne_Click(object sender, RoutedEventArgs e)
        {
            var path = "f:\\face.jpg";
            var data = System.IO.File.ReadAllBytes(path);
            //var comepareResult = facePxy.compare(data, data);
            //if (comepareResult == null)
            //{
            //    Debug.Assert(false, "比对异常");
            //    return;
            //}

            //lbResult.Items.Clear();
            //Item("code:" + comepareResult.code);
            //Item("message:" + comepareResult.message);
            //Item("similarity:" + comepareResult.similarity);

            #region 比对
            facePxy.begin_compare(data, data).whenCompleted((comepareResult) =>
            {
                if (comepareResult == null)
                {
                    Debug.Assert(false, "比对异常");
                    return;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    lbResult.Items.Clear();
                    Item("code:" + comepareResult.code);
                    Item("message:" + comepareResult.message);
                    Item("similarity:" + comepareResult.similarity);
                }));
            },
            (ex) =>
            {
                Item("调用异常:" + ex.Message);
            });
            Item("调用结束");
            #endregion

            client.Compare();
        }

        private void btnFaceCapture_Click(object sender, RoutedEventArgs e)
        {
            var threshold = 0.8f;
            var maxImageCount = 6;
            var data = System.IO.File.ReadAllBytes("F:\\girl.jpg");
            var faceResult = facePxy.staticDetect(data, threshold, maxImageCount);
            if (faceResult == null)
            {
                Debug.Assert(false, "人脸捕获异常");
                return;
            }

            lbResult.Items.Clear();
            Item("code:" + faceResult.code);
            Item("message:" + faceResult.message);
            Item("faces length:" + faceResult.faceInfoList.Length);

            int index = 1;
            foreach (var face in faceResult.faceInfoList)
            {
                Item("person:" + index);
                Item("name:" + face.name + " nation:" + face.nationality);
                index++;
            }
        }

        private void btnSignaturecode_Click(object sender, RoutedEventArgs e)
        {
            var data = System.IO.File.ReadAllBytes("F:\\girl.jpg");
            var signature = facePxy.convertSignatureCode(data);
            if (signature == null)
            {
                Debug.Assert(false, "提取特征失败");
                return;
            }

            lbResult.Items.Clear();
            Item("signature code length:" + signature.Length);
        }

        private void btnQueryPerson_Click(object sender, RoutedEventArgs e)
        {
            List<string> tagList = new List<string> { "1", "2", "3" };
            var result = facePxy.queryPerson("id", "uuid", "code", tagList.ToArray(), 0, 30);

            if (result == null)
            {
                Debug.Assert(false, "人物库查询失败");
                return;
            }

            lbResult.Items.Clear();
            Item("记录数:" + result.totalCount);
            Item("查询人物库数量:" + result.personInfoList.Length);
        }

        private void btncreateorUpdatePerson_Click(object sender, RoutedEventArgs e)
        {
            var uuid = GetUUID();
            var name = "name";
            var code = "code";

            var result = facePxy.createOrUpdatePerson(uuid, name, code, null, null, null, null, null, null);
            lbResult.Items.Clear();
            Item("code:" + result.code);
            Item("message:" + result.message);
        }

        private void btnremovePerson_Click(object sender, RoutedEventArgs e)
        {
            var uuid = GetUUID();
            var result = facePxy.removePerson(uuid);
            if (result == null)
            {
                Debug.Assert(false, "删除操作异常");
                return;
            }
            lbResult.Items.Clear();
            Item("删除:" + uuid);
        }

        private void btnupdatePersonTag_Click(object sender, RoutedEventArgs e)
        {
            var uuid = GetUUID();
            var result = facePxy.updatePersonTags(uuid, new List<string> { "1", "2" }.ToArray());

            lbResult.Items.Clear();
            Item("code:" + result.code);
            Item("message:" + result.message);
        }

        private void btnremovePersonTag_Click(object sender, RoutedEventArgs e)
        {
            var uuid = GetUUID();
            var result = facePxy.deletePersonTags(uuid, null);

            lbResult.Items.Clear();
            Item("code:" + result.code);
            Item("message:" + result.message);
        }


        private Ice.ObjectAdapter callbackAdapter = null;
        private void btndynamicDetect_Click(object sender, RoutedEventArgs e)
        {
            //回调EndPoint的写法
            //1."default -h 192.168.1.116 -p 9991"
            //2."default"
            string endpoints = "default -h 192.168.1.116 -p 9991";
            callbackAdapter = ic.createObjectAdapterWithEndpoints("callback-client", endpoints);

            Ice.Object servant = new ClientCallbackI(Item);
            callbackAdapter.add(servant, ic.stringToIdentity("callbackReceiver"));
            callbackAdapter.activate();

            ClientCallbackReceiverPrx receiverPrx = null;

            Ice.Identity identity = ic.stringToIdentity("callbackReceiver");
            //代理一定要通过adapter对象创建
            Ice.ObjectPrx pxy = callbackAdapter.createProxy(identity);
            receiverPrx = ClientCallbackReceiverPrxHelper.uncheckedCast(pxy);


            var result = facePxy.dynamicDetect("rtspPath", receiverPrx, 0.4f, 100, 4);
            lbResult.Items.Clear();
            Item("code:" + result.code);
            Item("message:" + result.message);
        }

        private void btnstopDynamicDetect_Click(object sender, RoutedEventArgs e)
        {
            var result = facePxy.shutdownDynamicDetect();
            lbResult.Items.Clear();
            Item("code:" + result.code);
            Item("message:" + result.message);

            if (callbackAdapter != null)
            {
                Item("destroy local proxy");
                callbackAdapter.destroy();
            }
        }

        private void btnverifySignaturecode_Click(object sender, RoutedEventArgs e)
        {
            var result = facePxy.verifySignatureCode(null, 0.8f, 0, 50);
            lbResult.Items.Clear();
            Item("totalCount:" + result.totalCount);
            Item("verifySignatureCodeInfoList:" + result?.verifySignatureCodeInfoList?.Length);

            var index = 1;
            foreach (var p in result.verifySignatureCodeInfoList)
            {
                Item("index:" + index);
                Item("similarity:" + p.similarity);
                Item("name:" + p.personInfo.name);
                Item("code:" + p.personInfo.code);
                index++;
            }
        }
    }
}
