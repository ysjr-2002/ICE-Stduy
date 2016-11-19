using AirPort.Client.Model;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class PersonViewWindow
    {
        private PersonInfo person = null;
        public PersonViewWindow(PersonInfo person)
        {
            InitializeComponent();

            this.person = person;
            this.DataContext = person;
            this.Loaded += PersonViewWindow_Loaded;
        }

        private void PersonViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var byteArray = person.FaceImage1.Base64ToByte();
            var imagesource = ByteArrayToBitmapImage(byteArray);
            imageFace.Source = imagesource;
        }

        private void btnClose_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }
    }
}
