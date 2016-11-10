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

namespace FaceClientEx
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FaceDetectWindow face = new FaceClientEx.FaceDetectWindow();
            face.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CompareWindow compare = new CompareWindow();
            compare.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RtspWindow rtsp = new FaceClientEx.RtspWindow();
            rtsp.ShowDialog();
        }
    }
}
