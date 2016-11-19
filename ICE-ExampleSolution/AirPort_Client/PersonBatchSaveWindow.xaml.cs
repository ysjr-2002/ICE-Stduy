using Common.NotifyBase;
using IceInternal;
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
    /// PersonBatchSaveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PersonBatchSaveWindow
    {
        private string imageFile1 = @"C:\Users\Shaojie\Desktop\face\zp.bmp";
        Test test = new Test();
        public PersonBatchSaveWindow()
        {
            InitializeComponent();
            test.ExecuteCount = "1";
            test.ExecuteTime = "20";
            lblcount.DataContext = test;
            lbltime.DataContext = test;
            lbltotaltime.DataContext = test;

            txtfolder.Text = @"D:\300";
        }

        private string getFileName(string filepath)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(filepath);
            var start = name.IndexOf('(') + 1;
            var end = name.IndexOf(')');
            var len = end - start;
            name = name.Substring(start, len);
            return name;
        }

        private void btnExecute_click(object sender, RoutedEventArgs e)
        {
            var files = System.IO.Directory.GetFiles(txtfolder.Text, "*.jpg");
            files = files.OrderBy(s => getFileName(s).ToInt32()).ToArray();

            System.Threading.ThreadPool.QueueUserWorkItem((s) =>
            {
                Stopwatch totaltime = Stopwatch.StartNew();
                foreach (var file in files)
                {
                    var index = getFileName(file);
                    Stopwatch sw = Stopwatch.StartNew();
                    send(file);
                    sw.Stop();
                    test.ExecuteCount = index;
                    test.ExecuteTime = sw.ElapsedMilliseconds.ToString();
                    test.ExecuteTotalTime = totaltime.ElapsedMilliseconds.ToString();

                    if (index.ToInt32() > 200)
                        break;
                }
                totaltime.Stop();
                //for (int i = 1; i <= 5000; i++)
                //{
                //    Stopwatch sw = Stopwatch.StartNew();
                //    send(i.ToString());
                //    sw.Stop();
                //    test.ExecuteCount = i.ToString();
                //    test.ExecuteTime = sw.ElapsedMilliseconds.ToString();
                //}
            });
        }

        private void send(string filepath)
        {
            var index = getFileName(filepath);

            var buffer1 = filepath.FileToByte();
            var image1 = Convert.ToBase64String(buffer1);

            var sb = new StringBuilder();
            sb.Append("uuid".ElementText(index));
            sb.Append("code".ElementText(index));
            sb.Append("name".ElementText(index));
            sb.Append("descrption".ElementText(""));
            sb.Append("imgData1".ElementText(image1));
            sb.Append("signatureCode1".ElementText(image1));
            sb.Append("imgData2".ElementText(""));
            sb.Append("signatureCode2".ElementText(""));
            sb.Append("imgData3".ElementText(""));
            sb.Append("signatureCode3".ElementText(""));
            sb.Append("tags".ElementBegin());

            List<string> tagList = new List<string>();
            tagList.Add("国内旅客");
            tagList.Add("国际旅客");
            foreach (var tagContent in tagList)
            {
                sb.Append("tag".ElementText(tagContent));
            }
            sb.Append("tags".ElementEnd());
            var data = sb.ToString();

            var xml = XmlParse.GetXml("createOrUpdatePerson", data);
            var content = FaceServices.FaceProxy.send(xml);
            //处理返回数据
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            var faceId = doc.GetNodeText("faceId");
        }

        private void btnSelect_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            txtfolder.Text = fbd.SelectedPath;
        }
    }

    class Test : PropertyNotifyObject
    {
        public string ExecuteCount
        {
            get { return this.GetValue(s => s.ExecuteCount); }
            set { this.SetValue(s => s.ExecuteCount, value); }
        }

        public string ExecuteTime
        {
            get { return this.GetValue(s => s.ExecuteTime); }
            set { this.SetValue(s => s.ExecuteTime, value); }
        }

        public string ExecuteTotalTime
        {
            get { return this.GetValue(s => s.ExecuteTotalTime); }
            set { this.SetValue(s => s.ExecuteTotalTime, value); }
        }
    }
}
