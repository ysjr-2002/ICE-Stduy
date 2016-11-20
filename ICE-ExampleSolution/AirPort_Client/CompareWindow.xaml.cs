using AirPort.Client.Core;
using Common;
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

namespace AirPort.Client
{
    /// <summary>
    /// 1:1比对
    /// </summary>
    public partial class CompareWindow
    {
        private string imagefile1 = "";
        private string imagefile2 = "";

        public CompareWindow()
        {
            InitializeComponent();
        }

        private void btnImage1_Click(object sender, RoutedEventArgs e)
        {
            imagefile1 = Utility.OpenFileDialog();
            if (imagefile1.IsEmpty())
                return;

            image1.Source = imagefile1.ToImageSource();
        }

        private void btnImage2_Click(object sender, RoutedEventArgs e)
        {
            imagefile2 = Utility.OpenFileDialog();
            if (imagefile2.IsEmpty())
                return;

            image2.Source = imagefile2.ToImageSource();
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            if (imagefile1.IsEmpty() || imagefile2.IsEmpty())
            {
                MessageBox.Show("请选择两张图片进行比对！");
                return;
            }

            var image1 = imagefile1.FileToBase64();
            var image2 = imagefile2.FileToBase64();

            var sb = new StringBuilder();
            sb.Append("srcImgData".ElementImage(image1));
            sb.Append("destImgData".ElementImage(image2));
            var xml = XmlParse.GetXml("compare", sb.ToString());
            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();

            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != status_ok)
            {
                WarnDialog("比对失败！");
                return;
            }
            var similarity = doc.GetNodeText("similarity");
            lblSmilary.Content = similarity.ToString().FormatFloat();
            lbltime.Content = sw.ElapsedMilliseconds;
        }
    }
}
