using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceClientEx
{
    class Connect : Ice.Application
    {
        public FaceRecognitionModule.FaceRecognitionPrx pxy = null;

        public override int run(string[] args)
        {
            Ice.ObjectPrx temp = communicator().propertyToProxy("Face.Proxy");
            pxy = FaceRecognitionModule.FaceRecognitionPrxHelper.checkedCast(temp);
            communicator().waitForShutdown();
            return 0;
        }
    }
}
