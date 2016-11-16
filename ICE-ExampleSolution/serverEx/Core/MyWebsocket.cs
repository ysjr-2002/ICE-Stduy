using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace serverEx.Core
{
    class MyWebsocket
    {
        private WebSocket ws = null;

        public void Test()
        {
            ws = new WebSocket("ws://localhost:4649/Echo?name=nobita");
            ws.Log.Level = LogLevel.Error;

            ws.OnOpen += (a, b) =>
            {
                Trace.WriteLine("opened");
            };

            ws.OnClose += (a, b) =>
            {
                Trace.WriteLine("close");
            };

            ws.OnMessage += (a, m) =>
            {
                Trace.WriteLine("message coming...");
                if (m.IsText)
                    Trace.WriteLine("is text");

                if (m.IsBinary)
                    Trace.WriteLine("is binary");

                if (m.IsPing)
                    Trace.WriteLine("is ping");

                Trace.WriteLine(m.Data);
            };
            ws.Connect();
        }

        public void SendText(string data)
        {
            ws.Send(data);
        }

        public void SendByte(byte[] data)
        {
            ws.Send(data);
        }

        public void SetCookie()
        {
            WebSocketSharp.Net.Cookie coolie = new WebSocketSharp.Net.Cookie();
            ws.SetCookie(coolie);
        }
    }
}
