using demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client1
{
    class client1
    {
        public static void Run(string[] args)
        {
            int status = 0;
            try
            {
                using (Ice.Communicator ic = Ice.Util.initialize(ref args))
                {
                    Ice.ObjectPrx obj = ic.stringToProxy("SimplePrinter:default -p 10000");
                    PrinterPrx printer = PrinterPrxHelper.checkedCast(obj);
                    if (printer == null)
                    {
                        Console.WriteLine("Invalid proxy");
                    }

                    printer.printString("Hello ice");
                    Console.WriteLine("send ok");
                    Console.WriteLine("press any key to continue");
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                status = 1;
            }
            Environment.Exit(status);
        }
    }
}
