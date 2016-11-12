using AirPort_Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirPort_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                Ice.Properties properties = Ice.Util.createProperties();
                //单位KB
                properties.setProperty("Ice.MessageSizeMax", "2048");
                Ice.InitializationData initData = new Ice.InitializationData();
                initData.properties = properties;
                var ic = Ice.Util.initialize(initData);

                //var ic = Ice.Util.initialize(ref args);
                Ice.Object servant = new MyFace();
                Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("FaceAdapter", "default -p 9996");
                adapter.add(servant, ic.stringToIdentity("myface"));
                adapter.activate();
                Console.Out.WriteLine("server start...");
                Console.Out.WriteLine("wait to shutdown...");
                ic.waitForShutdown();
            }
        }
    }
}
