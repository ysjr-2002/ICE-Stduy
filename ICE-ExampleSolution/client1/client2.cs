    using System;
using demo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ice;
using System.IO;
using System.Diagnostics;

namespace client1
{
    class client2 : Ice.Application
    {
        private static void menu()
        {
            Console.Out.WriteLine(
                "usage:\n" +
                "i: send immediate greeting\n" +
                "d: send delayed greeting\n" +
                "s: shutdown server\n" +
                "x: exit\n" +
                "?: help\n");
        }

        public override int run(string[] args)
        {
            var proxy = PrinterPrxHelper.checkedCast(communicator().propertyToProxy("Print.Proxy"));
            if (proxy == null)
            {
                Console.Error.WriteLine("invalid proxy");
                return 1;
            }

            menu();

            string line = null;
            do
            {
                line = Console.In.ReadLine();
                if (line == "i")
                {
                    proxy.printString("马上发");
                    Console.WriteLine("马上发");
                }
                else if (line == "d")
                {
                    proxy.begin_printString("延时发").whenCompleted(() =>
                    {
                        Console.WriteLine("异步发");
                    },
                    (Ice.Exception ex) =>
                    {

                    });
                }
                else if (line == "p")
                {
                    var buffer = File.ReadAllBytes("f:\\a.jpg");
                    Stopwatch sw = Stopwatch.StartNew();
                    proxy.sendImage(buffer, "car");
                    sw.Stop();
                    Console.WriteLine("发送成功->" + sw.ElapsedMilliseconds);
                }
                else if (line == "s")
                {
                    proxy.printString("s");
                    Console.Out.WriteLine("s");
                }
            } while (!(line == "x"));

            proxy.printString("app exuecte");

            return 0;
        }
    }
}
