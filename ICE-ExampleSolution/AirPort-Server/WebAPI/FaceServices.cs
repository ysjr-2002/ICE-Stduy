using AirPort.Server.FaceResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.WebAPI
{
    class FaceServices
    {
        public void GetVersion()
        {
            HttpMethod.Get(Constrants.url_version);
        }
        /// <summary>
        /// 提取特征码
        /// </summary>
        /// <param name="image"></param>
        /// <param name="threshold"></param>
        /// <param name="crop"></param>
        /// <returns></returns>
        public FeatureResult Feature(byte[] image, float threshold, bool crop)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("image_maxsize", "0");
            param.Add("crop", crop.ToString());

            var result = HttpMethod.Post<FeatureResult>(Constrants.url_extract, image, param);
            Console.WriteLine("特征码:" + result.Feature.Length);
            Console.WriteLine(result.Feature);
            return result;
        }
        /// <summary>
        /// 1:1比对
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        public double Compare(byte[] image1, byte[] image2)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("image_maxsize1", "0");
            param.Add("image_maxsize2", "0");

            var result = HttpMethod.Post<CompareResult>(Constrants.url_compare, image1, image2, param);
            return result.score;
        }
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="image"></param>
        /// <param name="analyze"></param>
        /// <param name="crop"></param>
        public DetectResult Detect(byte[] image, bool analyze, bool crop)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("image_maxsize", "0");
            param.Add("analyze", "true");
            param.Add("crop", "true");

            Stopwatch sw = Stopwatch.StartNew();
            var result = HttpMethod.Post<DetectResult>(Constrants.url_detect, image, param);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            return result;
        }

        public void Group_Delete()
        {

        }

        public void Group_Get(string group, string cursor)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", "5000");
            param.Add("cursor", cursor);
        }

        public void GroupPost(string group, string tag, string feature, int image_maxsize, string crop, byte[] image)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", group);
            param.Add("tag", tag);
            param.Add("feature", feature);
            param.Add("image_maxsize", image_maxsize.ToString());
            param.Add("crop", crop);
        }

        public void GGroupDelete(string group, string photo)
        {

        }

        public void GGroupGet(string group, string photo)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">可多个</param>
        /// <param name="feature"></param>
        /// <param name="image_maxsize"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="crop"></param>
        /// <param name="image"></param>
        public void Search(string group, string feature, int image_maxsize, int limit, string filter, bool crop, byte[] image)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", group);
            param.Add("feature", feature);
            param.Add("image_maxsize", image_maxsize.ToString());
            param.Add("limit", limit.ToString());
            param.Add("filter", filter);
            param.Add("crop", crop.ToString());
        }
    }
}
