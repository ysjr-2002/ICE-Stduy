using AirPort.Server.Core;
using AirPort.Server.FaceResult;
using AirPort.Server.Repository;
using AirPort.Server.WebAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace AirPort.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //FaceServices fs = new FaceServices();
            //fs.GetVideo("", 0.87f, "1", WebSocketcallback);

            Console.Title = "Server";
            PersonDB db = new PersonDB();
            db.Test();

            Ice.Properties properties = Ice.Util.createProperties();
            //单位KB
            properties.setProperty("Ice.MessageSizeMax", "2048");
            Ice.InitializationData initData = new Ice.InitializationData();
            initData.properties = properties;
            var ic = Ice.Util.initialize(initData);

            //var ic = Ice.Util.initialize(ref args);
            Ice.Object servant = new MyFace(db);
            Ice.ObjectAdapter adapter = ic.createObjectAdapterWithEndpoints("FaceAdapter", "default -p 9996");
            adapter.add(servant, ic.stringToIdentity("myface"));
            adapter.activate();
            Console.Out.WriteLine("server start...");
            Console.Out.WriteLine("wait to shutdown...");
            ic.waitForShutdown();
        }

        private static void WebSocketcallback(DynamicFaceResult face)
        {
            var sb = new StringBuilder();

            sb.Append("xml".ElementBegin());
            sb.Append("type".ElementText("dynamicDetectResult"));
            sb.Append("rtspId".ElementBegin());
            sb.Append("persons".ElementBegin());
            sb.Append("person".ElementBegin());
            sb.Append("imgData".ElementText(""));
            sb.Append("posX".ElementText(face.Result.Face.Rect.Left.ToString()));
            sb.Append("posY".ElementText(face.Result.Face.Rect.Top.ToString()));
            sb.Append("imgWidth".ElementText(face.Result.Face.Rect.Width.ToString()));
            sb.Append("imgHeight".ElementText(face.Result.Face.Rect.Height.ToString()));
            sb.Append("quality".ElementText(face.Result.Face.Quality.ToString()));
            sb.Append("person".ElementEnd());
            sb.Append("persons".ElementEnd());
            sb.Append("xml".ElementEnd());

            var data = sb.ToString();

            Console.WriteLine(data);
        }
    }
}
