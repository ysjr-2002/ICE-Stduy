using FaceRecognitionModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort_Client.Core
{
    public class ClientProxy
    {
        private Ice.Communicator ic;
        private FaceRecognitionPrx facePxy = null;

        public void Connect()
        {
            try
            {
                var args = new string[] { "" };
                Ice.Properties properties = Ice.Util.createProperties();
                //单位KB
                properties.setProperty("Ice.MessageSizeMax", "2048");
                Ice.InitializationData initData = new Ice.InitializationData();
                initData.properties = properties;
                ic = Ice.Util.initialize(initData);

                if (ic == null)
                {
                    Debug.Assert(false, "初始化失败");
                    return;
                }
                Ice.ObjectPrx pxy = ic.stringToProxy("myface:tcp -h localhost -p 9996");
                facePxy = FaceRecognitionPrxHelper.checkedCast(pxy);
                if (facePxy == null)
                {
                    Debug.Assert(false, "代理为空");
                    return;
                }
            }
            catch (System.Exception)
            {
                Debug.Assert(false, "初始化失败");
                return;
            }
        }

        public string Send(string xml)
        {
            return facePxy.send(xml);
        }
    }
}
