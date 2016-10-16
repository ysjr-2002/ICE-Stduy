using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Demo;
using Ice;
namespace client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        Ice.Communicator ic = null;
        ysj a;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            a = new client.ysj();
            a.main(new string[1] { "ysj" });

            //ic = Ice.Util.initialize();
            //Ice.ObjectPrx obj = ic.stringToProxy("callback:default -p 10000");
            //CallbackSenderPrx senderPxy = CallbackSenderPrxHelper.checkedCast(obj);
            //Console.WriteLine("call ok");

            //Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("xxx", "default");
            //adapter.add(new CallbackI(), ic.stringToIdentity("callbackReceiver"));
            //adapter.activate();
            //CallbackReceiverPrx receiverPxy = CallbackReceiverPrxHelper.uncheckedCast(
            //                              adapter.createProxy(ic.stringToIdentity("callbackReceiver")));

            //senderPxy.initializeCallback(receiverPxy);
        }
    }

    class CallbackI : Demo.CallbackReceiverDisp_
    {
        public override void callback(Current current__)
        {
            Console.Out.WriteLine("call back->" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    class ysj : Ice.Application
    {
        public override int run(string[] args)
        {
            var senderPxy = CallbackSenderPrxHelper.checkedCast(communicator().stringToProxy("callback:default -p 10000"));
            if (senderPxy == null)
            {
                Console.Out.WriteLine("创建对象为空");
                return 1;
            }

            Ice.ObjectAdapter adapter = communicator().createObjectAdapterWithEndpoints("xxx", "default");

            Ice.Object ok = new CallbackI();
            adapter.add(ok, communicator().stringToIdentity("callbackReceiver"));
            adapter.activate();

            var identity = communicator().stringToIdentity("callbackReceiver");
            var objectPtr = adapter.createProxy(identity);
            CallbackReceiverPrx receiverPxy = CallbackReceiverPrxHelper.uncheckedCast(objectPtr);

            senderPxy.initializeCallback(receiverPxy);

            return 0;
        }
    }
}
