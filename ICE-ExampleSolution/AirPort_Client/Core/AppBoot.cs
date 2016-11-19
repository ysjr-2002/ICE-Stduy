using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using AirPort.Client.Core;
using System.Threading;

namespace AirPort.Client
{
    class AppBoot : UnityBootstrapper
    {
        private FaceProxy proxy = new FaceProxy();

        protected override DependencyObject CreateShell()
        {
            MainWindow window = new AirPort.Client.MainWindow();
            return window;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            //FaceProxy proxy = new FaceProxy();
            //proxy.Connect();

            RunIce();
            Thread.Sleep(500);

            this.Container.RegisterInstance(typeof(FaceProxy), proxy);

            var window = (Window)this.Shell;
            window.ShowDialog();
        }

        private void RunIce()
        {
            new Thread(() =>
            {
                string[] args = new string[0];

                proxy.main(args, "config.client");
            }).Start();
        }
    }
}
