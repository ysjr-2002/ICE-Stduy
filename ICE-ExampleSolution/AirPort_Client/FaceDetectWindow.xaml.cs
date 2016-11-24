using AirPort.Client.Core;
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

namespace AirPort.Client
{
    /// <summary>
    /// 旷世为提供参数(应用层可以处理)
    /// </summary>
    public partial class FaceDetectWindow
    {
        private string imagefile = "";
        private int imgPixelWidth = 0;
        private int imgPixelHeight = 0;

        public FaceDetectWindow()
        {
            InitializeComponent();
        }

        private void btnChoice_click(object sender, RoutedEventArgs e)
        {
            imagefile = Utility.OpenFileDialog();
            if (imagefile.IsEmpty())
                return;

            faceImage.Source = imagefile.ToImageSource();
            ClearResult();
        }

        private void ClearResult()
        {
            this.canvas1.Children.Clear();
            lbltimeinfo.Content = "";
            lblfacecount.Content = "";
        }

        private void Send()
        {
            if (imagefile.IsEmpty())
            {
                WarnDialog("请选择一张图片！ ");
                return;
            }

            var buffer = imagefile.FileToByte();
            var base64Image = buffer.ToBase64();

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(base64Image));
            sb.Append("threshold".ElementText(txtThrold.Text));
            sb.Append("maxImageCount".ElementText(txtFaceMax.Text));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("staticDetect", data);
            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();
            lbltimeinfo.Content = string.Format(callElapsed_Template, sw.ElapsedMilliseconds);

            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            Item("code->" + code);
            if (code.ToInt32() != status_ok)
            {
                WarnDialog("执行失败！");
                return;
            }

            var persons = doc.SelectNodes("/xml/persons/person");
            lblfacecount.Content = persons.Count;
            DrawFace(persons);
        }

        private void btnDetect_click(object sender, RoutedEventArgs e)
        {
            ClearResult();
            Send();
        }

        private void DrawFace(XmlNodeList faces)
        {
            imgPixelWidth = ((BitmapSource)faceImage.Source).PixelWidth;
            imgPixelHeight = ((BitmapSource)faceImage.Source).PixelHeight;

            this.canvas1.Width = faceImage.ActualWidth;
            this.canvas1.Height = faceImage.ActualHeight;

            var factorx = faceImage.ActualWidth / imgPixelWidth;
            var factory = faceImage.ActualHeight / imgPixelHeight;

            foreach (XmlNode face in faces)
            {
                var quality = face.GetNodeText("quality").ToFloat();
                if (quality <= txtThrold.Text.ToFloat())
                {
                    continue;
                }
                var x = face.GetNodeText("posX").ToInt32();
                var y = face.GetNodeText("posY").ToInt32();
                var w = face.GetNodeText("imgWidth").ToInt32();
                var h = face.GetNodeText("imgHeight").ToInt32();

                RectangleGeometry rect = new RectangleGeometry
                {
                    Rect = new Rect
                    {
                        X = x * factorx,
                        Y = y * factory,
                        Width = w * factorx,
                        Height = h * factory
                    }
                };

                Path myPath = new Path();
                myPath.StrokeThickness = 3;
                myPath.Stroke = Brushes.Red;
                myPath.Data = rect;

                var qInfo = quality.ToString();
                if (qInfo.Length > 4)
                    qInfo = qInfo.Substring(0, 4);

                Label lblfacequality = new Label { Content = "质量=" + qInfo, Foreground = Brushes.Red };
                Canvas.SetLeft(lblfacequality, rect.Rect.X + +rect.Rect.Width + 5);
                Canvas.SetTop(lblfacequality, rect.Rect.Y);
                this.canvas1.Children.Add(myPath);
                this.canvas1.Children.Add(lblfacequality);
            }
        }
    }
}