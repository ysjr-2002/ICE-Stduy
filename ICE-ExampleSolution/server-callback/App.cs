using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server_callback
{
    public class App : Ice.Application
    {
        public override int run(string[] args)
        {
            if (args.Length > 0)
            {
                System.Console.Error.WriteLine(appName() + ": too many arguments");
                return 1;
            }

            Ice.ObjectAdapter adapter = communicator().createObjectAdapter("Callback.Server");
            adapter.add(new CallbackSenderI(), communicator().stringToIdentity("callbackSender"));
            adapter.activate();

            Console.Out.WriteLine("start");

            communicator().waitForShutdown();
            return 0;
        }
    }
}
