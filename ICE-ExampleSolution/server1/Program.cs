using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int status = 0;
            Ice.Communicator ic = null;

            ic = Ice.Util.initialize(ref args);

            Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("SimplePrinterAdapter", "default -p 10000");

            Ice.Object obj = new PrinterI();

            adapter.add(obj, ic.stringToIdentity("SimplePrinter"));

            adapter.activate();

            Console.WriteLine("started");

            ic.waitForShutdown();

            if (ic != null)
            {
                ic.destroy();
                status = 1;
            }

            Environment.Exit(status);
        }

    }
}
