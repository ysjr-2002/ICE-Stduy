using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Client.Core
{
    class ContextData
    {
        public static List<string> Tags()
        {
            List<string> tags = new List<string>
            {
                "无",
                "国内旅客",
                "国际旅客",
                "黄种人",
                "白种人",
                "国航VIP" ,
                "南航VIP" ,
                "东航VIP",
                "深航VIP"
            };
            return tags;
        }
    }
}
