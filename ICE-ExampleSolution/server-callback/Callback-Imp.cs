using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo;
using Ice;
using System.Threading;

namespace server_callback
{
    public sealed class CallbackSenderI : CallbackSenderDisp_
    {
        public override void initiateCallback(CallbackReceiverPrx proxy, Ice.Current current)
        {
            System.Console.Out.WriteLine("initiating callback");
            try
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        proxy.callback();
                        Console.Out.WriteLine("call back");
                        Thread.Sleep(1000);
                    }
                });
            }
            catch (System.Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
        }

        public override void shutdown(Ice.Current current)
        {
            System.Console.Out.WriteLine("Shutting down...");
            try
            {
                current.adapter.getCommunicator().shutdown();
            }
            catch (System.Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
        }
    }
}
