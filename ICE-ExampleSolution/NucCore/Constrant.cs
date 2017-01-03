using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NucCore
{
    public class Constrant
    {
        public string camera { get; set; }

        public string group { get; set; }

        public string user { get; set; }

        public string handle { get; set; }

        public string handle_websocket { get; set; }

        public void Init(string nucIP)
        {
            camera = string.Concat("http://", nucIP, "/camera");

            group = string.Concat("http://", nucIP, "/group");

            user = string.Concat("http://", nucIP, "/user");

            handle = string.Concat("http://", nucIP, "/handle");

            handle_websocket = string.Concat("ws://", nucIP, "/handle");
        }
    }
}
