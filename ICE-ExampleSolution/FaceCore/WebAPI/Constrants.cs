using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.WebAPI
{
    public class Constrants
    {
        public static string domain = "";
        /// <summary>
        /// 特征码提取
        /// </summary>
        public static string url_extract = domain + "/_extract";
        /// <summary>
        /// 图片比对
        /// </summary>
        public static string url_compare = domain + "/compare";
        /// <summary>
        /// 人脸检测
        /// </summary>
        public static string url_detect = domain + "/detect";
        public static string url_g = domain + "/g";
        /// <summary>
        /// 入库
        /// </summary>
        public static string url_gpost = domain + "/g/group";

        public static string url_gdelete = domain + "/g/group";

        public static string url_version = domain + "/version";

        public static string url_search = domain + "/search";

        public static string url_video = "";

        public static void Init(string serverIp)
        {
            domain = "http://" + serverIp + ":8000";
            url_extract = domain + "/_extract";
            url_compare = domain + "/compare";
            url_detect = domain + "/detect";
            url_g = domain + "/g";
            url_gpost = domain + "/g/group";
            url_gdelete = domain + "/g/group";
            url_version = domain + "/version";
            url_search = domain + "/search";
            url_video = "ws://" + serverIp + ":8000/video";
        }
    }
}
