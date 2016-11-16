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

namespace AirPort.Client
{
    /// <summary>
    /// 删除单个人物
    /// </summary>
    public partial class PersonDeleteWindow
    {
        private string uuid = "";
        public PersonDeleteWindow(string uuid)
        {
            InitializeComponent();
            this.uuid = uuid;
            this.txtuuid.Text = uuid;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtuuid.Text.IsEmpty())
            {
                WarnDialog("请输入用户业务标识！");
                return;
            }
            delete();
            this.DialogResult = true;
        }

        private void delete()
        {
            var sb = new StringBuilder();
            sb.Append(("uuid".ElementText(txtuuid.Text)));
            var data = sb.ToString();
            var xml = XmlParse.GetXml("deletePerson", data);
            var content = FaceServices.FaceProxy.send(xml);
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            Item("code->" + code);
            if (code.ToInt32() == status_ok)
            {
                TipDialog("删除成功！");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
