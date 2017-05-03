using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ice;

namespace client553
{
    class MyCallback : demo.ICallbackDisp_
    {
        public override void doback(string date, Current current__)
        {
            Console.Out.WriteLine("server callback->" + date);
        }
    }
}
