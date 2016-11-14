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

namespace AirPort.Client
{
    class AppBoot : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            MainWindow window = new AirPort.Client.MainWindow();
            return window;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            FaceProxy proxy = new Core.FaceProxy();
            proxy.Connect();
            this.Container.RegisterInstance(typeof(FaceProxy), proxy);

            var window = (Window)this.Shell;
            window.ShowDialog();
        }
    }
}
