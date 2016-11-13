using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AirPort_Server.WebAPI
{
    class MyWebRequest
    {
        private const int timeout = 150000;
        public static HttpWebRequest CreateGet(string url)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "GET";
            wr.Timeout = timeout;
            return wr;
        }

        public static HttpWebRequest Create(string url)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.Method = "POST";
            wr.Timeout = 10000;
            return wr;
        }

        public static HttpWebRequest CreateImage(string url, string boundary)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            return wr;
        }

        //public static HttpWebRequest Create(string url)
        //{
        //    var wr = (HttpWebRequest)WebRequest.Create(url);
        //    wr.ContentType = "application/x-www-form-urlencoded";
        //    wr.Method = "POST";
        //    return wr;
        //}
    }
}
