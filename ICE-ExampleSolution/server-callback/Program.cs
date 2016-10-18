using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ice;
namespace server_callback
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ice.Communicator ic = null;
            //ic = Ice.Util.initialize(ref args);
            ////创建一个adapter
            //Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("Callbackadapter", "default -p 10000");
            ////创建一个服务端对象
            //Ice.Object obj = new callbacksender();
            ////将服务端对象和adapter进行关联
            //adapter.add(obj, ic.stringToIdentity("callbackSender"));
            ////激活adapter
            //adapter.activate();

            //Console.Out.WriteLine("started");

            //ic.waitForShutdown();

            App app = new server_callback.App();

            app.main(args, "config.server");
        }
    }
}
