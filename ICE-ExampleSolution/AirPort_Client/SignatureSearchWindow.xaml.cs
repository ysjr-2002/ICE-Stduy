using AirPort.Client.Core;
using Common;
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
                CheckBox control = new CheckBox
                {
                    Content = tag,
                    Margin = new Thickness(0, 10, 5, 0)
                };
                tagContainer.Children.Add(control);
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
                WarnDialog("请选择源图像！");
                return;
            }
            query();
        }

        private void query()
        {
            var sb = new StringBuilder();

            sb.Append("signatureCode".ElementText("signatureCode"));
            sb.Append("threshold".ElementText("0.56"));
            sb.Append("size".ElementText("100"));

            sb.Append("tags".ElementBegin());
            sb.Append("tag".ElementText("1"));
            sb.Append("tag".ElementText("2"));
            sb.Append("tags".ElementEnd());

            var data = sb.ToString();
            var xml = XmlParse.GetXml("verifySignatureCode", data);
            var content = FaceServices.FaceProxy.send(xml);

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != 200)
            {
                WarnDialog("查询结果返回失败！");
                return;
            }

            Item("code->" + code);
            var persons = doc.SelectNodes("/xml/result/matchPerson");
            Item("匹配人物数量->" + persons.Count);

            foreach (XmlNode n in persons)
            {
                Item("similarity->" + n.GetNodeText("similarity"));
                Item("faceId->" + n.GetNodeText("faceId"));
                Item("uuid->" + n.GetNodeText("uuid"));
                Item("name->" + n.GetNodeText("name"));
                Item("descrption->" + n.GetNodeText("descrption"));

                var tags = n.SelectNodes("tags/tag");
                Item("人物标签数量->" + tags.Count);
                foreach (XmlNode tag in tags)
                {
                    Item("tag->" + tag.InnerText);
                }

                Item("人脸1->" + n.GetNodeText("imgData1"));
                Item("人脸2->" + n.GetNodeText("imgData2"));
                Item("人脸3->" + n.GetNodeText("imgData3"));
            }
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
    }
}
