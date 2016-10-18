using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionModule;
using Ice;

namespace FaceClient
{
    public class ClientCallbackI : FaceRecognitionModule.ClientCallbackReceiverDisp_
    {
        private Action<string> log;
        public ClientCallbackI(Action<string> act)
        {
            this.log = act;
        }
        public override void detectCallback(PersonInfo[] faceInfoSeq, Current current__)
        {
            var length = faceInfoSeq?.Length;
            if (length == 0)
            {
                Console.Out.WriteLine("返回数据为空");
                return;
            }

            log("detect face count:" + length);
            foreach (var p in faceInfoSeq)
            {
                log("name:" + p.name);
                log("code:" + p.code);
                log("race:" + p.race);
                log("nationality:" + p.nationality);
                log("datetime:" + DateTime.Now.ToString());
            }
        }
    }
}
