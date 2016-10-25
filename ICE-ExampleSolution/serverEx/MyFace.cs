using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionModule;
using Ice;
using System.IO;

namespace serverEx
{
    class MyFace : FaceRecognitionModule.FaceRecognitionDisp_
    {
        public MyFace()
        {
            Console.Out.WriteLine("create a server object");
        }

        public override void initConnectionListener(ConnectionListenerPrx listener, Current current__)
        {

        }

        public override string send(string xml, Current current__)
        {
            var content = "";

            StringWriter sw = new StringWriter();
            //compareResult result = new compareResult
            //{
            //    code = 0,
            //    similarity = 0.95f
            //};
            //System.Xml.Serialization.XmlSerializer serialize = new System.Xml.Serialization.XmlSerializer(typeof(compareResult));
            //serialize.Serialize(sw, result);

            //content = sw.ToString();

            content = "<xml>"
                + "<code>0</code>"
                    + "<similarity>0.812</similarity>"
                    + "</xml>";

            return content;
        }
    }

    public class compareResult
    {
        public int code { get; set; }

        public float similarity { get; set; }
    }
}
