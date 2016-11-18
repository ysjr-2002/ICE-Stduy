using AirPort.Client.Core;
using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AirPort.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FaceDetectWindow face = new FaceDetectWindow();
            face.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CompareWindow compare = new CompareWindow();
            compare.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RtspWindow rtsp = new RtspWindow();
            rtsp.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PersonRepositoryWindow repository = new PersonRepositoryWindow();
            repository.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SignatureCodeWindow sign = new SignatureCodeWindow();
            sign.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            SignatureSearchWindow search = new SignatureSearchWindow();
            search.ShowDialog();
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public class User
    {
        private System.Threading.Semaphore semahore = new System.Threading.Semaphore(2, 2);

        public User(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void Test()
        {
            Console.WriteLine("排队:" + Thread.CurrentThread.Name);
            semahore.WaitOne();
            Console.WriteLine(Thread.CurrentThread.Name);
            Thread.Sleep(5000);
            semahore.Release();
        }
    }
}
