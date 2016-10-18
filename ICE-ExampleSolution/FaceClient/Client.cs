using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceClient
{
    class Client : Ice.Application
    {
        public Ice.Communicator ic;
        public FaceRecognitionModule.FaceRecognitionPrx proxy = null;
        public override int run(string[] args)
        {
            Ice.ObjectPrx pxy = communicator().propertyToProxy("Face.Proxy");
            proxy = FaceRecognitionModule.FaceRecognitionPrxHelper.checkedCast(pxy);
            ic = communicator();

            proxy.removePerson("dfdfdf");
            return 0;
        }
    }
}
