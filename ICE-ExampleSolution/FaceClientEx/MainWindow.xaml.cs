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

        private void btnOneCompareOne_Click(object sender, RoutedEventArgs e)
        {
            var xml = "<xml><type>compare</type><srcImgData>{0}</srcImgData><destImgData>{1}</destImgData></xml>";
            var content = facePxy.send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.SelectSingleNode("/xml/code");
            var similarity = doc.SelectSingleNode("/xml/similarity");

            Item("code->" + code.InnerText);
            Item("similarity->" + similarity.InnerText);            
        }
    }

    public class compareResult
    {
        public int code { get; set; }
        public float similarity { get; set; }
    }
}
