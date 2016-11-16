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
    /// 入库
    /// </summary>
    public partial class PersonSaveWindow
    {
        private int tagIndex = 1;
        private string imageFile1 = "";
        private string imageFile2 = "";
        private string imageFile3 = "";
        private const string tagTemplate = "标签{0}：{1}";
        private List<string> tagList = new List<string>();

        public PersonSaveWindow()
        {
            InitializeComponent();
            cmbTags.ItemsSource = ContextData.Tags();
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            var content = ((ListBoxItem)cmbTags.SelectedItem).Content.ToString();
            if (content == "无")
            {
                WarnDialog("请选择一个标签！");
                return;
            }
            tagList.Add(content);
            Label lblTag = new Label
            {
                Margin = new Thickness(0, 5, 0, 0),
                Content = string.Format(tagTemplate, tagIndex, content),
                Foreground = Brushes.Red,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            spTags.Children.Add(lblTag);
            tagIndex++;
        }

        private void btnSave_click(object sender, RoutedEventArgs e)
        {
            if (txtuuid.Text.IsEmpty())
            {
                WarnDialog("请输入业务编号！");
                txtuuid.Focus();
                return;
            }
            if (txtCode.Text.IsEmpty())
            {
                WarnDialog("请输入唯一标识！");
                txtCode.Focus();
                return;
            }
            if (txtName.Text.IsEmpty())
            {
                WarnDialog("请输入名称！");
                txtName.Focus();
                return;
            }
            send();
            this.DialogResult = true;
        }

        private void send()
        {
            var buffer1 = imageFile1.FileToByte();
            var image1 = Convert.ToBase64String(buffer1);

            var buffer2 = imageFile2.FileToByte();
            var image2 = Convert.ToBase64String(buffer2);

            var buffer3 = imageFile3.FileToByte();
            var image3 = Convert.ToBase64String(buffer3);

            var sb = new StringBuilder();
            sb.Append("uuid".ElementText(txtuuid.Text));
            sb.Append("code".ElementText(txtCode.Text));
            sb.Append("name".ElementText(txtName.Text));
            sb.Append("descrption".ElementText(""));
            sb.Append("imgData1".ElementText(image1));
            sb.Append("signatureCode1".ElementText(image1));
            sb.Append("imgData2".ElementText(image2));
            sb.Append("signatureCode2".ElementText(image2));
            sb.Append("imgData3".ElementText(image3));
            sb.Append("signatureCode3".ElementText(image3));
            sb.Append("tags".ElementBegin());
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
            Item("code->" + code);
            Item("faceId->" + faceId);
            MessageBox.Show(faceId);
        }

        private void btnClose_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnImage1_Click(object sender, RoutedEventArgs e)
        {
            imageFile1 = Utility.OpenFileDialog();
            if (imageFile1.IsEmpty())
                return;

            txtImage1.Text = imageFile1;
        }

        private void btnImage2_Click(object sender, RoutedEventArgs e)
        {
            imageFile2 = Utility.OpenFileDialog();
            if (imageFile2.IsEmpty())
                return;

            txtImage2.Text = imageFile2;
        }

        private void btnImage3_Click(object sender, RoutedEventArgs e)
        {
            imageFile3 = Utility.OpenFileDialog();
            if (imageFile3.IsEmpty())
                return;

            txtImage3.Text = imageFile3;
        }
    }
}
