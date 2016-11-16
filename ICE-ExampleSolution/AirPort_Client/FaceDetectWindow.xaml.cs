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

            var imageSource = DrawFace(persons);
            faceImage.Source = imageSource;
        }

        private void btnDetect_click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private ImageSource WpfDraw(XmlNodeList faces)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();

            BitmapImage bitmap = (BitmapImage)faceImage.Source;
            context.DrawImage(faceImage.Source, new Rect { X = 0, Y = 0, Width = bitmap.Width, Height = bitmap.Height });
            foreach (XmlNode f in faces)
            {
                Item("imgData->" + f.GetNodeText("imgData"));
                var quality = f.GetNodeText("quality").ToFloat();
                var x = f.GetNodeText("posX").ToInt32();
                var y = f.GetNodeText("posY").ToInt32();
                var w = f.GetNodeText("imgWidth").ToInt32();
                var h = f.GetNodeText("imgHeight").ToInt32();

                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 5), new Rect
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

            return render;
        }

        private ImageSource DrawFace(XmlNodeList faces)
        {
            System.Drawing.Image source = System.Drawing.Image.FromFile(imagefile);
            var g = System.Drawing.Graphics.FromImage(source);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Red, 5);
            foreach (XmlNode f in faces)
            {
                Item("imgData->" + f.GetNodeText("imgData"));
                var quality = f.GetNodeText("quality").ToFloat();
                var x = f.GetNodeText("posX").ToInt32();
                var y = f.GetNodeText("posY").ToInt32();
                var w = f.GetNodeText("imgWidth").ToInt32();
                var h = f.GetNodeText("imgHeight").ToInt32();

                g.DrawRectangle(pen, new System.Drawing.Rectangle { X = x, Y = y, Width = w, Height = h });
            }
            g.Save();
            g.Dispose();

            var imageSource = Utility.BitmapToBitmapSource((System.Drawing.Bitmap)source);
            return imageSource;
        }
    }
}