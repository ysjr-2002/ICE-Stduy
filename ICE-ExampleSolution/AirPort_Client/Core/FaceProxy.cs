using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Client.Core
{
    public class FaceProxy : Ice.Application
    {
        public FaceRecognitionModule.FaceRecognitionPrx facePxy = null;

        public override int run(string[] args)
        {
            Ice.ObjectPrx temp = communicator().propertyToProxy("Face.Proxy");
            facePxy = FaceRecognitionModule.FaceRecognitionPrxHelper.checkedCast(temp);
            communicator().waitForShutdown();
            return 0;
        }

        public Ice.Communicator Ic
        {
            get
            {
                return Ice.Application.communicator();
            }
        }

        public string send(string xml)
        {
            try
            {
                return facePxy.send(xml);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void initConnectionListener(ConnectionListenerPrx listener)
        {
            facePxy.initConnectionListener(listener);
        }
    }
}
