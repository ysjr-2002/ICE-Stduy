using AirPort.Client.Core;
using AirPort.Client.Model;
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
    /// 特征码1:N比对
    /// </summary>
    public partial class SignatureSearchWindow
    {
        private string filepath = "";
        public SignatureSearchWindow()
        {
            InitializeComponent();
            this.Loaded += SignatureSearchWindow_Loaded;
        }

        private void LoadTags()
        {
            foreach (var tag in ContextData.Tags())
            {
                if (tag == "无")
                    continue;
                CheckBox tagBox = new CheckBox
                {
                    Content = tag,
                    Margin = new Thickness(0, 10, 5, 0)
                };
                tagContainer.Children.Add(tagBox);
            }
        }

        private void SignatureSearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTags();
        }

        private void btnChoice_click(object sender, RoutedEventArgs e)
        {
            filepath = Utility.OpenFileDialog();
            if (filepath.IsEmpty())
                return;

            imgSource.Source = filepath.ToImageSource();
        }

        private void btnQuery_click(object sender, RoutedEventArgs e)
        {
            if (filepath.IsEmpty())
            {
                WarnDialog("请选择图像！");
                return;
            }
            query();
        }

        private string GetImageFeature(string imagefile)
        {
            var buffer = imagefile.FileToByte();
            var base64Image = buffer.ToBase64();

            var sb = new StringBuilder();
            sb.Append("imgData".ElementText(base64Image));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("convertSignatureCode", data);
            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return string.Empty;
            }

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            var signatureCode = doc.GetNodeText("signatureCode");
            return signatureCode;
        }

        private string[] GetTags()
        {
            var controls = tagContainer.Children.OfType<CheckBox>();
            var tags = controls.Where(s => s.IsChecked == true).Select(s => s.Content.ToString()).ToArray();
            return tags;
        }

        private void query()
        {
            var sb = new StringBuilder();

            var feature = GetImageFeature(filepath);
            sb.Append("signatureCode".ElementText(feature));
            sb.Append("threshold".ElementText(txtthrold.Text));
            sb.Append("size".ElementText(txtsize.Text));

            var tags = GetTags();
            if (tags.Length > 0)
            {
                sb.Append("tags".ElementBegin());
                foreach (var tag in tags)
                {
                    sb.Append("tag".ElementText(tag));
                }
                sb.Append("tags".ElementEnd());
            }

            sb.Append("validTime".ElementText(txtValidTime.Text.ToInt32().ToString()));

            var data = sb.ToString();
            var xml = XmlParse.GetXml("verifySignatureCode", data);
            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();

            lbltime.Content = sw.ElapsedMilliseconds;

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != status_ok)
            {
                WarnDialog("查询结果返回失败！");
                return;
            }

            Item("code->" + code);
            var personNodes = doc.SelectNodes("/xml/result/matchPerson");
            Item("匹配人物数量->" + personNodes.Count);
            lblpersoncount.Content = personNodes.Count.ToString();

            List<PersonInfo> persons = new List<PersonInfo>();
            foreach (XmlNode n in personNodes)
            {
                var p = new PersonInfo();
                p.similarity = n.SelectSingleNode("similarity").InnerText.ToFloat();
                p.faceId = n.SelectSingleNode("faceId").InnerText;
                p.uuid = n.SelectSingleNode("uuid").InnerText;
                p.code = n.SelectSingleNode("code").InnerText;
                p.Name = n.SelectSingleNode("name").InnerText;
                p.Description = n.SelectSingleNode("description").InnerText;
                p.FaceImage1 = n.SelectSingleNode("imgData1").InnerText;
                p.FaceImage2 = n.SelectSingleNode("imgData2").InnerText;
                p.FaceImage3 = n.SelectSingleNode("imgData3").InnerText;

                persons.Add(p);
            }
            persons = persons.OrderByDescending(s => s.similarity).ToList();
            dgPersons.ItemsSource = persons;
        }

        private void btnReset_click(object sender, RoutedEventArgs e)
        {
            filepath = string.Empty;
            imgSource.Source = null;

            var tags = tagContainer.Children.OfType<CheckBox>();
            foreach (var tag in tags)
            {
                tag.IsChecked = false;
            }
        }

        private void dgPersons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgPersons.SelectedItem == null)
                return;

            PersonInfo person = (PersonInfo)dgPersons.SelectedItem;
            PersonViewWindow window = new PersonViewWindow(person);
            window.ShowDialog();
        }
    }
}
