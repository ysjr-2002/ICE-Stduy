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
using System.Text;
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
            PersonDB db = new PersonDB();
            db.Test();

            Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Face");
            Ice.Object faceServant = new MyFace(db);
            adapter.add(faceServant, communicator().stringToIdentity("myface"));
            adapter.activate();

            print("server start...");
            communicator().waitForShutdown();
            return 0;
        }

        static int Main(string[] args)
        {
            App app = new App();

            //Ice.Properties properties = Ice.Util.createProperties();
            //properties.setProperty("Ice.MessageSizeMax", "2097152");//2gb in kb
            //properties.setProperty("Face.Endpoints", "tcp -h 127.0.0.1 -p 9996");
            //Ice.InitializationData data = new Ice.InitializationData();
            //data.properties = properties;
            //return app.main(args, data);

            return app.main(args, "config.server");
        }

        private static void print(string content)
        {
            Console.WriteLine(string.Format("iceserver:{0}", content));
        }
    }
}
