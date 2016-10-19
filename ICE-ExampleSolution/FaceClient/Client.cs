using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceClient
{
    class Client : Ice.Application
    {
        private Action<string> Item = null;
        public Ice.Communicator ic = null;
        public FaceRecognitionModule.FaceRecognitionPrx facePxy = null;

        public Client(Action<string> log)
        {
            this.Item = log;
        }

        public override int run(string[] args)
        {
            Ice.ObjectPrx pxy = communicator().propertyToProxy("Face.Proxy");
            this.facePxy = FaceRecognitionModule.FaceRecognitionPrxHelper.checkedCast(pxy);
            this.ic = communicator();
            //一定要阻塞
            communicator().waitForShutdown();
            return 0;
        }

        public void Compare()
        {
            var data = System.IO.File.ReadAllBytes("F:\\girl.jpg");
            var comepareResult = facePxy.compare(data, data);
            if (comepareResult == null)
            {
                Debug.Assert(false, "比对异常");
                return;
            }

            Item("code:" + comepareResult.code);
            Item("message:" + comepareResult.message);
            Item("similarity:" + comepareResult.similarity);
        }
    }
}
