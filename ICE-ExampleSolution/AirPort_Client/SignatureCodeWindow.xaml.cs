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

            txtfeature.Clear();
            lbltimeInfo.Content = "";
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            if (imagefile.IsEmpty())
            {
                WarnDialog("请选择一张图片！ ");
                return;
            }
            var base64Image = imagefile.FileToBase64();
            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(base64Image));
            var data = sb.ToString();
            var xml = XmlParse.GetXml("convertSignatureCode", data);

            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();
            lbltimeInfo.Content = string.Format(callElapsed_Template, sw.ElapsedMilliseconds);
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != status_ok)
            {
                WarnDialog("提取特征码失败！");
                return;
            }
            var signatureCode = doc.GetNodeText("signatureCode");
            txtfeature.Text = signatureCode;
        }
    }
}
