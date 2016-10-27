using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    public static class Ext
    {
        public static string GetNodeText(this XmlDocument doc, string path)
        {
            var node = doc.SelectSingleNode("/xml/" + path);
            if (node == null)
                throw new ArgumentException("参数错误", path);
            else
                return node.InnerText;
        }

        public static string GetNodeText(this XmlNode node, string name)
        {
            var element = node.SelectSingleNode(name);
            if (element == null)
            {
                throw new ArgumentException("参数错误", name);
            }
            else
            {
                return element.InnerText;
            }
        }

        public static byte[] ToByteBuffer(this string str)
        {
            return Convert.FromBase64String(str);
        }

        public static string ElementBegin(this string name)
        {
            return string.Format("<{0}>", name);
        }

        public static string ElementEnd(this string name)
        {
            return string.Format("</{0}>", name);
        }

        public static string ElementText(this string name, string value)
        {
            return string.Format("<{0}>{1}</{0}>", name, value);
        }
    }
}
