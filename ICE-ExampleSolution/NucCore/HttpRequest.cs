using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NucCore
{
    public class HttpRequest
    {
        private const int timeout = 150000;
        public static HttpWebRequest Get(string url)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "GET";
            wr.Timeout = timeout;
            wr.KeepAlive = false;
            wr.Proxy = null;
            return wr;
        }

        public static HttpWebRequest Post(string url)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.Method = "POST";
            wr.Timeout = timeout;
            wr.KeepAlive = false;
            return wr;
        }

        public static HttpWebRequest Delete(string url)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "Delete";
            wr.Timeout = timeout;
            return wr;
        }

        public static HttpWebRequest PostImage(string url, string boundary)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Timeout = timeout;
            wr.KeepAlive = false;
            return wr;
        }
    }
}
