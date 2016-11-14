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
using AirPort.Client.Core;
using System.Diagnostics;

namespace AirPort.Client
{
    /// <summary>
    /// 特征码提取
    /// </summary>
    public partial class SignatureCodeWindow
    {
        private string imagefile = "";
        public SignatureCodeWindow()
        {
            InitializeComponent();
        }

        private void btnChoiceImage_Click(object sender, RoutedEventArgs e)
        {
            imagefile = Utility.OpenFileDialog();
            if (imagefile.IsEmpty())
                return;

            imageFace.Source = imagefile.ToImageSource();
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            var buffer1 = System.IO.File.ReadAllBytes(imagefile);
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(image1));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("convertSignatureCode", data);

            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            var signatureCode = doc.GetNodeText("signatureCode");

            txtfeature.Text = signatureCode;
            lbltimeInfo.Content = sw.ElapsedMilliseconds + "毫秒";
        }
    }
}
