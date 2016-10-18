using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionModule;
using Ice;
namespace server
{
    //class Program
    //{
    //    /// <summary>
    //    /// endpoint
    //    /// 1.default -p 9996
    //    /// 2.default -h localhost -p 9996
    //    /// </summary>
    //    /// <param name="args"></param>
    //    static void Main(string[] args)
    //    {
    //        Ice.Communicator ic = null;
    //        ic = Ice.Util.initialize(ref args);
    //        Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("faceAdapter", "default -h 192.168.1.116 -p 9996");
    //        Ice.Object face = new MyFace();
    //        adapter.add(face, ic.stringToIdentity("myface"));
    //        adapter.activate();
    //        Console.Out.WriteLine("server start...");
    //        ic.waitForShutdown();
    //    }
    //}

    public class App : Ice.Application
    {
        static int Main(string[] args)
        {
            App app = new App();
            return app.main(args, "config.server");
        }

        public override int run(string[] args)
        {
            Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Face");
            Ice.Object servant = new MyFace();
            adapter.add(servant, communicator().stringToIdentity("myface"));
            adapter.activate();

            Console.Out.WriteLine("server start...");
            var task = CloudAPI.MegviiCloud.Login();
            if (task.Result)
            {
                var taskSatus = CloudAPI.MegviiCloud.GetAccountStatus();
                var status = taskSatus.Result;
                Console.Out.WriteLine("剩余调用次数:" + status.data.limitation.quota);
                Console.Out.WriteLine("cloud compare service start...");
            }
            communicator().waitForShutdown();
            Console.Out.WriteLine("ending...");
            return 0;
        }
    }
}
