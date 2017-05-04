using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server53
{
    class Application : Ice.Application
    {
        public override int run(string[] args)
        {
            var adapter = communicator().createObjectAdapterWithEndpoints("tt", "default -p 9872");
            adapter.add(new PrinterI(), communicator().stringToIdentity("ysj"));
            adapter.add(new DglI(), communicator().stringToIdentity("dgl"));
            adapter.activate();

            Console.Out.WriteLine("server start");
            communicator().waitForShutdown();
            return 0;
        }
    }
}
