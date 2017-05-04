using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ice;

namespace server53
{
    class DglI : demo.DglDisp_
    {
        public override void Where(string name, Current current__)
        {
            Console.Out.WriteLine("你也来了->" + name);
        }
    }
}
