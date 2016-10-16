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
    /// <summary>
    /// 
    /// </summary>
    class callbacksender : Demo.CallbackSenderDisp_
    {
        public override void initializeCallback(CallbackReceiverPrx proxy, Current current__)
        {
            Console.Out.WriteLine("initialize callback");
            try
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        proxy.callback();
                        Thread.Sleep(1000);
                        Console.Out.WriteLine("call back");
                    }
                });
                
            }
            catch(System.Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        public override void shutdownDynamicDetect(Current current__)
        {
            Console.Out.WriteLine("shutting down...");
            current__.adapter.getCommunicator().shutdown();
        }
    }
}
