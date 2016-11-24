using Common;
using AirPort.Client.Core;
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

namespace AirPort.Client
{
    /// <summary>
    /// 批量删除
    /// </summary>
    public partial class BatchDeleteWindow
    {
        private List<string> delTags = new List<string>();

        public BatchDeleteWindow()
        {
            InitializeComponent();
            this.Loaded += BatchDeleteWindow_Loaded;
        }

        private void BatchDeleteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTags();
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
                    Width = 300,
                    Margin = new Thickness(0, 10, 5, 0)
                };
                tagContainer.Children.Add(control);
            }
        }

        private void btnSave_click(object sender, RoutedEventArgs e)
        {
            delTags.Clear();
            foreach (CheckBox item in tagContainer.Children)
            {
                if (item.IsChecked.Value)
                {
                    delTags.Add(item.Content.ToString());
                }
            }
            delete();
            this.DialogResult = true;
        }

        private void delete()
        {
            var sb = new StringBuilder();
            sb.Append("tags".ElementBegin());
            foreach (var tag in delTags)
            {
                sb.Append("tag".ElementText(tag));
            }
            sb.Append("tags".ElementEnd());
            var data = sb.ToString();

            var xml = XmlParse.GetXml("deletePersonsByTags", data);
            var content = FaceServices.FaceProxy.send(xml);
            if (content.IsEmpty())
            {
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            var affectCount = doc.GetNodeText("affectCount");

            Item("code->" + code);
            Item("affectCount->" + affectCount);
            TipDialog("删除记录数->" + affectCount);
        }

        private void btnClose_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
