using Common;
using AirPort.Client.Core;
using AirPort.Client.Model;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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
using System.Diagnostics;

namespace AirPort.Client
{
    /// <summary>
    /// 人像库
    /// </summary>
    public partial class PersonRepositoryWindow
    {
        private int offset = 0;
        private int pagesize = 30;
        public PersonRepositoryWindow()
        {
            InitializeComponent();
            this.Loaded += PersonWareHouseWindow_Loaded;
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

        private void PersonWareHouseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTags();
        }

        private void Button_click(object sender, RoutedEventArgs e)
        {
            PersonSaveWindow person = new PersonSaveWindow();
            var result = person.ShowDialog().Value;
            if (result)
                queryPersons();
        }

        private string GetSelectPersonId()
        {
            var faceId = "";
            if (dgPersons.SelectedItem != null)
            {
                faceId = ((PersonInfo)dgPersons.SelectedItem).faceId;
            }
            return faceId;
        }

        //删除(单个)
        private void btnDeletePerson_click(object sender, RoutedEventArgs e)
        {
            var faceId = GetSelectPersonId();
            PersonDeleteWindow person = new PersonDeleteWindow(faceId);
            var result = person.ShowDialog().Value;
            if (result)
                queryPersons();
        }

        //删除(批量)
        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            BatchDeleteWindow batch = new BatchDeleteWindow();
            var result = batch.ShowDialog().Value;
            if (result)
                queryPersons();
        }

        //更新
        private void btnUpdateTag_click(object sender, RoutedEventArgs e)
        {
            var faceId = GetSelectPersonId();
            PersonUpdateTagWindow updateTag = new PersonUpdateTagWindow(faceId);
            var result = updateTag.ShowDialog().Value;
            if (result)
                queryPersons();
        }

        //删除人像标签
        private void btnDeleteTag_click(object sender, RoutedEventArgs e)
        {
            var faceId = GetSelectPersonId();
            PersonDeleteTagWindow tag = new PersonDeleteTagWindow(faceId);
            var result = tag.ShowDialog().Value;
            if (result)
                queryPersons();
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

        private void btnQueryPerson_click(object sender, RoutedEventArgs e)
        {
            if (GetSelectedTags().Count == 0)
            {
                WarnDialog("请选择标签！");
                return;
            }
            queryPersons();
        }

        private void queryPersons()
        {
            var sb = new StringBuilder();
            sb.Append("id".ElementText(txt1.Text));
            sb.Append("uuid".ElementText(txt2.Text));
            sb.Append("code".ElementText(txt3.Text));

            sb.Append("tags".ElementBegin());
            var tags = GetSelectedTags();
            foreach (var tag in tags)
            {
                sb.Append("tag".ElementText(tag));
            }
            sb.Append("tags".ElementEnd());

            sb.Append("offset".ElementText(offset.ToString()));
            sb.Append("size".ElementText(pagesize.ToString()));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("queryPersons", data);

            Stopwatch sw = Stopwatch.StartNew();
            var content = FaceServices.FaceProxy.send(xml);
            sw.Stop();
            lbltime.Content = sw.ElapsedMilliseconds;
            if (content.IsEmpty())
            {
                WarnDialog(community_error);
                return;
            }
            ShowQueryResult(content);
        }

        private void ShowQueryResult(string content)
        {
            var doc = XmlParse.LoadXml(content);

            var code = doc.GetNodeText("code");
            var totalCount = doc.GetNodeText("totalCount");

            Item("code->" + code);
            Item("totalCount->" + totalCount);
            lblpersoncount.Content = totalCount;
            var personNodes = doc.SelectNodes("/xml/persons/person");

            List<PersonInfo> persons = new List<PersonInfo>();
            foreach (XmlNode n in personNodes)
            {
                var p = new PersonInfo();
                p.faceId = n.SelectSingleNode("faceId").InnerText;
                p.uuid = n.SelectSingleNode("uuid").InnerText;
                p.code = n.SelectSingleNode("code").InnerText;
                p.Name = n.SelectSingleNode("name").InnerText;
                p.Description = n.SelectSingleNode("descrption").InnerText;
                p.FaceImage1 = n.SelectSingleNode("imgData1").InnerText;
                p.HasSignatureCode1 = n.SelectSingleNode("hasSignatureCode1").InnerText.ToInt32() == 1;
                p.FaceImage2 = n.SelectSingleNode("imgData2").InnerText;
                p.HasSignatureCode2 = n.SelectSingleNode("hasSignatureCode2").InnerText.ToInt32() == 1;
                p.FaceImage3 = n.SelectSingleNode("imgData3").InnerText;
                p.HasSignatureCode3 = n.SelectSingleNode("hasSignatureCode3").InnerText.ToInt32() == 1;

                persons.Add(p);
            }

            dgPersons.ItemsSource = persons;
        }

        private void btnReset_click(object sender, RoutedEventArgs e)
        {
            txt1.Clear();
            txt2.Clear();
            txt3.Clear();
            foreach (CheckBox item in tagContainer.Children)
            {
                item.IsChecked = false;
            }
        }

        private void btnBatchSave_click(object sender, RoutedEventArgs e)
        {
            PersonBatchSaveWindow pb = new PersonBatchSaveWindow();
            pb.ShowDialog();
        }

        private void btnPre_click(object sender, RoutedEventArgs e)
        {
            offset -= pagesize;
            if (offset < 0)
                offset = 0;
            queryPersons();
        }

        private void btnNext_click(object sender, RoutedEventArgs e)
        {
            offset += pagesize;
            queryPersons();
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
