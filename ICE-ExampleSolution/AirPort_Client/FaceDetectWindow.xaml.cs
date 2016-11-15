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
    /// 旷世为提供参数(但是应用层可以处理
    /// </summary>
    public partial class FaceDetectWindow
    {
        private string filepath = "";

        public FaceDetectWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            filepath = Utility.OpenFileDialog();
            if (filepath.IsEmpty())
                return;

            faceImage.Source = filepath.ToImageSource();
        }

        private void Send()
        {
            var buffer1 = System.IO.File.ReadAllBytes(filepath);
            var image1 = buffer1.ToBase64();

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(image1));
            sb.Append("threshold".ElementText(txtThrold.Text));
            sb.Append("maxImageCount".ElementText("56"));
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

            var persons = doc.SelectNodes("/xml/persons/person");
            lblfacecount.Content = persons.Count;

            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();

            BitmapImage bitmap = (BitmapImage)faceImage.Source;
            context.DrawImage(faceImage.Source, new Rect { X = 0, Y = 0, Width = bitmap.Width, Height = bitmap.Height });
            foreach (XmlNode f in persons)
            {
                Item("imgData->" + f.GetNodeText("imgData"));
                Item("imgWidth->" + f.GetNodeText("imgWidth"));
                Item("imgHeight->" + f.GetNodeText("imgHeight"));
                Item("posX->" + f.GetNodeText("posX"));
                Item("posY->" + f.GetNodeText("posY"));
                Item("quality->" + f.GetNodeText("quality"));

                var x = f.GetNodeText("posX").ToInt32();
                var y = f.GetNodeText("posY").ToInt32();
                var w = f.GetNodeText("imgWidth").ToInt32();
                var h = f.GetNodeText("imgHeight").ToInt32();

                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), new Rect
                {
                    X = x,
                    Y = y,
                    Width = w,
                    Height = h
                });
            }

            context.Close();
            var render = new RenderTargetBitmap((int)bitmap.Width, (int)bitmap.Height, 96, 96, PixelFormats.Default);
            render.Render(visual);

            faceImage.Source = render;
        }

        private void btnDetect_click(object sender, RoutedEventArgs e)
        {
            Send();
        }
    }
}