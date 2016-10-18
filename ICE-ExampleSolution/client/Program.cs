using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    
    class Program
    {
        [STAThread]
        static int Main(string[] arg)
        {
            App app = new App();
            return app.main(arg, "config.client");
        }
    }
}
