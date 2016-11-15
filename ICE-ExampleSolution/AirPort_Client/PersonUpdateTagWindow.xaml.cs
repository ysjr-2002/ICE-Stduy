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

namespace AirPort.Client
{
    /// <summary>
    /// 更新人像标签
    /// </summary>
    public partial class PersonUpdateTagWindow
    {
        private string uuid = "";
        public PersonUpdateTagWindow(string uuid)
        {
            InitializeComponent();

            this.uuid = uuid;
            txtuuid.Text = uuid;
            this.Loaded += PersonUpdateTagWindow_Loaded;
        }

        private void PersonUpdateTagWindow_Loaded(object sender, RoutedEventArgs e)
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

        private List<string> GetSelectedTags()
        {
            List<string> tags = new List<string>();
            foreach (CheckBox item in tagContainer.Children)
            {
                if (item.IsChecked.Value)
                {
                    tags.Add(item.Content.ToString());
                }
            }
            return tags;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtuuid.Text.IsEmpty())
            {
                WarnDialog("请输入用户业务标识！");
                return;
            }
            if (GetSelectedTags().Count == 0)
            {
                WarnDialog("请选择标签！");
                return;
            }
            deletePersonTag();

            this.DialogResult = true;
        }

        private void deletePersonTag()
        {
            var sb = new StringBuilder();
            sb.Append("uuid".ElementText(txtuuid.Text));

            var tags = GetSelectedTags();
            if (tags.Count > 0)
            {
                sb.Append("tags".ElementBegin());
                foreach (var tag in tags)
                {
                    sb.Append("tag".ElementText(tag));
                }
                sb.Append("tags".ElementEnd());
            }

            var data = sb.ToString();
            var xml = XmlParse.GetXml("updatePersonTags", data);
            var content = FaceServices.FaceProxy.send(xml);

            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            Item("code->" + code);

            if (code.ToInt32() == status_ok)
            {
                TipDialog("操作成功！");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
