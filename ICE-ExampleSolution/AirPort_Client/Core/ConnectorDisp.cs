using Ice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Client.Core
{
    class ConnectorDisp : FaceRecognitionModule.ConnectionListenerDisp_
    {
        private Action<string> log;

        public ConnectorDisp(Action<string> log)
        {
            this.log = log;
        }

        public override void onRecv(string xmlContent, Current current__)
        {
            log(xmlContent);
        }
    }
}
