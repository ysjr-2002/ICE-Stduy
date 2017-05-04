using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demo;
using Ice;

namespace server53
{
    class PrinterI : demo.PrinterDisp_
    {
        private ICallbackPrx callbackpxy = null;
        public override void InitCallback(ICallbackPrx callback, Current current__)
        {
            callbackpxy = callback;
            callback.doback("ttt");
        }

        public override void printString(string input, Current current__)
        {
            Console.WriteLine("coming->" + input);
        }

        public override void sendImage(byte[] seq, string name, Current current__)
        {
            Console.Out.WriteLine("coming->" + seq?.Length);
        }
    }
}
