using AirPort_Client.Core;
using Common;
using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Xml;

namespace AirPort_Client
{
    /// <summary>
    /// FaceDetectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FaceDetectWindow : Window
    {
        private string filepath = "";
        private ClientProxy proxy;
        public FaceDetectWindow(ClientProxy proxy)
        {
            InitializeComponent();
            this.proxy = proxy;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            filepath = Util.OpenFileDialog();
            if (filepath.IsEmpty())
                return;

            faceImaeg.Source = filepath.ToImageSource();
        }

        private void Send()
        {
            var buffer1 = System.IO.File.ReadAllBytes(filepath);
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(image1));
            sb.Append("threshold".ElementText(txtThrold.Text));
            sb.Append("maxImageCount".ElementText("56"));
            var data = sb.ToString();

            var xml = GetXml("staticDetect", data);
            var content = proxy.Send(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var code = doc.GetNodeText("code");
            //Item("code->" + code);

            //var persons = doc.SelectNodes("/xml/persons/person");
            //Item("人脸数量->" + persons.Count);
            //foreach (XmlNode f in persons)
            //{
            //    Item("imgData->" + f.GetNodeText("imgData"));
            //    Item("imgWidth->" + f.GetNodeText("imgWidth"));
            //    Item("imgHeight->" + f.GetNodeText("imgHeight"));
            //    Item("posX->" + f.GetNodeText("posX"));
            //    Item("posY->" + f.GetNodeText("posY"));
            //    Item("quality->" + f.GetNodeText("quality"));
            //}
        }

        private string GetXml(string type, string data)
        {
            var content = "<xml><type>" + type + "</type>" + data + "</xml>";
            return content;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Send();
        }
    }
}