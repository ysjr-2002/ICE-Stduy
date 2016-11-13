using AirPort_Client.Core;
using FaceRecognitionModule;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirPort_Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientProxy clientProxy = null;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            clientProxy = new ClientProxy();
            clientProxy.Connect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FaceDetectWindow face = new FaceDetectWindow(clientProxy);
            face.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CompareWindow compare = new CompareWindow(clientProxy);
            compare.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RtspWindow rtsp = new RtspWindow();
            rtsp.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            GroupWindow group = new GroupWindow();
            group.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SignatureCodeWindow sign = new AirPort_Client.SignatureCodeWindow(clientProxy);
            sign.ShowDialog();
        }
    }
}
