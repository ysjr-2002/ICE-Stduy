using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client553
{
    class Program
    {
        static void Main(string[] args)
        {
            var ic = Ice.Util.initialize(ref args);
            Ice.ObjectPrx prx = ic.stringToProxy("ysj:default -p 9872");
            demo.PrinterPrx proxy = demo.PrinterPrxHelper.checkedCast(prx);
            proxy.printString("ss");
            proxy.sendImage(new byte[1] { 1 }, "tt");

            var adapter = ic.createObjectAdapterWithEndpoints("kk", "default -p 9871");
            var identity = ic.stringToIdentity("jj");
            var objectpxy = adapter.add(new MyCallback(), identity);
            adapter.activate();

            //方式一
            //objectpxy = ic.stringToProxy("jj:default -p 9871");
            //方式二
            //demo.ICallbackPrx callbackproxy = demo.ICallbackPrxHelper.uncheckedCast(objectpxy);
            //方式三
            objectpxy = adapter.createProxy(identity);
            demo.ICallbackPrx callbackproxy = demo.ICallbackPrxHelper.uncheckedCast(objectpxy);

            proxy.InitCallback(callbackproxy);

            ic.waitForShutdown();
        }
    }
}
