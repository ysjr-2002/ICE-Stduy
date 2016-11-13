using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System
{
    public static class XmlParse
    {
        public static string GetXml(string type, string data)
        {
            var content = "<xml><type>" + type + "</type>" + data + "</xml>";
            return content;
        }

        public static XmlDocument LoadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
    }
}
