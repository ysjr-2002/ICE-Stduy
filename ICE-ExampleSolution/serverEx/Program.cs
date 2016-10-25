using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace serverEx
{
    class Program
    {
        static void Main(string[] args)
        {
            var ic = Ice.Util.initialize(ref args);

            Ice.Object servant = new MyFace();

            Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("FaceAdapter", "default -p 9996");
            adapter.add(servant, ic.stringToIdentity("myface"));
            adapter.activate();

            Console.Out.WriteLine("server start...");

            ic.waitForShutdown();
            Console.Out.WriteLine("wait to shutdown...");
        }
    }
}
