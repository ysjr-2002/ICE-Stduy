using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace serverEx
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Ice.Properties properties = Ice.Util.createProperties();
    //        //单位KB
    //        properties.setProperty("Ice.MessageSizeMax", "2048");
    //        Ice.InitializationData initData = new Ice.InitializationData();
    //        initData.properties = properties;
    //        var ic = Ice.Util.initialize(initData);

    //        //var ic = Ice.Util.initialize(ref args);
    //        Ice.Object servant = new MyFace();
    //        Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("FaceAdapter", "default -p 9996");
    //        adapter.add(servant, ic.stringToIdentity("myface"));
    //        adapter.activate();

    //        Console.Out.WriteLine("server start...");
    //        var task = CloudAPI.MegviiCloud.Login();
    //        if (task.Result)
    //        {
    //            var taskSatus = CloudAPI.MegviiCloud.GetAccountStatus();
    //            var status = taskSatus.Result;
    //            Console.Out.WriteLine("剩余调用次数:" + status.data.limitation.quota);
    //            Console.Out.WriteLine("cloud compare service start...");
    //        }
    //        Console.Out.WriteLine("wait to shutdown...");
    //        ic.waitForShutdown();
    //    }
    //}

    class App : Ice.Application
    {
        public override int run(string[] args)
        {
            Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Face1");
            Ice.Object faceServant = new MyFace();
            adapter.add(faceServant, communicator().stringToIdentity("myface"));
            adapter.activate();

            Ice.ObjectAdapter adapter2 = communicator().createObjectAdapter("Face2");
            Ice.Object faceServant2 = new MyFace();
            adapter2.add(faceServant2, communicator().stringToIdentity("myface2"));
            adapter2.activate();

            Console.Out.WriteLine("server start...");
            communicator().waitForShutdown();
            return 0;
        }

        static int Main(string[] args)
        {
            App app = new serverEx.App();
            return app.main(args, "config.server");
        }
    }
}
