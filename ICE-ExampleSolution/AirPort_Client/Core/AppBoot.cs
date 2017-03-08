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
            var window = new MainWindow();
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
                //Ice.Properties properties = Ice.Util.createProperties();
                //properties.setProperty("Ice.MessageSizeMax", "2097152");//2gb in kb 2097152
                //properties.setProperty("Face.Proxy", "myface:tcp -h 192.168.3.77 -p 9996");
                //Ice.InitializationData data = new Ice.InitializationData();
                //data.properties = properties;
                //proxy.main(args, data);
            }).Start();
        }
    }
}
