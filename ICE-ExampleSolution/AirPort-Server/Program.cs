using AirPort.Server.Core;
using AirPort.Server.FaceResult;
using AirPort.Server.Repository;
using AirPort.Server.WebAPI;
using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AirPort.Server
{
    #region console start
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Console.Title = "Server";
    //        PersonDB db = new PersonDB();
    //        db.Test();

    //        Ice.Properties properties = Ice.Util.createProperties();
    //        //单位KB
    //        properties.setProperty("Ice.MessageSizeMax", "2048");
    //        Ice.InitializationData initData = new Ice.InitializationData();
    //        initData.properties = properties;
    //        var ic = Ice.Util.initialize(initData);

    //        //var ic = Ice.Util.initialize(ref args);
    //        Ice.Object servant = new MyFace(db);
    //        Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("FaceAdapter", "default -p 9996");
    //        adapter.add(servant, ic.stringToIdentity("myface"));
    //        adapter.activate();
    //        Console.Out.WriteLine("server start...");
    //        Console.Out.WriteLine("wait to shutdown...");
    //        ic.waitForShutdown();
    //    }
    //} 
    #endregion

    class App : Ice.Application
    {
        private void AutoRun()
        {
            var appname = "byiceserver";
            var auto = System.Configuration.ConfigurationManager.AppSettings["autoRun"];
            var apppath = System.Windows.Forms.Application.ExecutablePath;
            Utility.runWhenStart(auto == "1", appname, apppath);
        }

        public override int run(string[] args)
        {
            AutoRun();

            Console.Title = "ICE-Server";
            PersonMySql db = new PersonMySql();
            db.Connect();

            try
            {
                Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Face");
                Ice.Object faceServant = new MyFace(db);
                adapter.add(faceServant, communicator().stringToIdentity("myface"));
                adapter.activate();

                print("server start...");
                communicator().waitForShutdown();
                return 0;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;

                Console.ForegroundColor = ConsoleColor.Red;
                print("启动服务失败->" + ex.Message);
                Console.ReadLine();
                return 0;
            }
        }

        static int Main(string[] args)
        {
            App app = new App();

            Task.Factory.StartNew(() =>
            {
                GetIp();
            });
            auto.WaitOne();
            Console.Clear();

            //自定义方式
            Ice.Properties properties = Ice.Util.createProperties();
            properties.load("config.server");
            properties.setProperty("Ice.ThreadPool.Server.Size", "16");
            properties.setProperty("Ice.ThreadPool.Server.SizeMax", "100");
            properties.setProperty("Ice.ThreadPool.Server.SizeWarn", "0");
            Ice.InitializationData data = new Ice.InitializationData();
            data.properties = properties;
            return app.main(args, data);

            //配置文件方式
            //return app.main(args, "config.server");
        }

        static AutoResetEvent auto = new AutoResetEvent(false);
        private static void GetIp()
        {
            var config = System.IO.File.ReadAllText("config.server");
            var available = false;
            while (!available)
            {
                var ip = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (var item in ip)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (config.IndexOf(item.ToString()) >= 0)
                        {
                            auto.Set();
                            available = true;
                            break;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private static void print(string content)
        {
            //https://github.com/dengly/Ice-demo/blob/master/Ice%203.6.1%E9%85%8D%E7%BD%AE%E5%8F%82%E6%95%B0%E8%AF%B4%E6%98%8E.md
            Console.WriteLine(string.Format("iceserver:{0}", content));
        }
    }
}
