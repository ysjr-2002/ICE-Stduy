using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectAndCompare
{

    class IceApp : Ice.Application
    {
        public static FaceRecognitionModule.FaceRecognitionPrx facePxy = null;

        public override int run(string[] args)
        {
            Ice.ObjectPrx temp = communicator().propertyToProxy("Face.Proxy");
            facePxy = FaceRecognitionModule.FaceRecognitionPrxHelper.checkedCast(temp);
            communicator().waitForShutdown();
            return 0;
        }
    }
}
