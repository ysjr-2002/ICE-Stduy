using AirPort.Server.Core;
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
            //fs.QueryGroupPhotoes();

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
    }
}
