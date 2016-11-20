using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace System
{
    public static class XmlExt
    {
        public static XmlNodeList GetSelecteNodes(this XmlDocument doc, string path)
        {
            var nodes = doc.SelectNodes("/xml/" + path);
            return nodes;
        }

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

        public static string ElementImage(this string name, string value)
        {
            return string.Format("<{0}><![CDATA[{1}]]></{0}>", name, value);
        }

        public static string FormatFloat(this string val)
        {
            if (val.Length > 4)
                return val.Substring(0, 4);
            else
                return val;
        }
    }
}
