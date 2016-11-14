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

namespace AirPort.Client
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PersonWareHouseWindow
    {
        public PersonWareHouseWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PersonSaveWindow person = new PersonSaveWindow();
            person.ShowDialog();
        }

        //删除(单个)
        private void btnDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            PersonDeleteWindow person = new Client.PersonDeleteWindow();
            person.ShowDialog();
        }

        //删除(批量)
        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            BatchDeleteWindow batch = new BatchDeleteWindow();
            batch.ShowDialog();
        }

        //更新
        private void btnUpdateTag_Click(object sender, RoutedEventArgs e)
        {

        }

        //删除
        private void btnDeleteTag_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
