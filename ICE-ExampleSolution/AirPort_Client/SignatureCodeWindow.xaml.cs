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
using System.Windows.Shapes;
using Common;
using AirPort_Client.Core;

namespace AirPort_Client
{
    /// <summary>
    /// SignatureCodeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SignatureCodeWindow
    {
        private ClientProxy facePxy = null;
        private string imagefile = "";
        public SignatureCodeWindow(ClientProxy facePxy)
        {
            InitializeComponent();
            this.facePxy = facePxy;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            imagefile = Util.OpenFileDialog();
            if (imagefile.IsEmpty())
                return;

            imageFace.Source = imagefile.ToImageSource();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(imagefile);
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(image1));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("convertSignatureCode", data);
            var content = facePxy.send(xml);

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            var signatureCode = doc.GetNodeText("signatureCode");

            txtfeature.Text = signatureCode;
        }
    }
}
