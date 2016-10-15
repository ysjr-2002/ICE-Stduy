using System;
using demo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client1
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            client2 app = new client2();
            app.main(args, "config.client");

            Console.Read();
            return 1;
        }
    }
}
