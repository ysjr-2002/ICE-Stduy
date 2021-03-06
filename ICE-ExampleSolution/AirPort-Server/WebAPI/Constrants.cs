﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.WebAPI
{
    public class Constrants
    {
        private const string domain = "http://192.168.1.40:8000";

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

        public static string url_video = "ws://192.168.1.40:8000/video";
    }
}
