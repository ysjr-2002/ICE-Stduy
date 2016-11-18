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
            //HttpMethod.Get(Constrants.url_version);
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

            var featureResult = HttpMethod.Post<FeatureResult>(Constrants.url_extract, image, param);
            return featureResult;
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

            var compareResult = HttpMethod.Post<CompareResult>(Constrants.url_compare, image1, image2, param);
            return compareResult.score;
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

            var detectResult = HttpMethod.Post<DetectResult>(Constrants.url_detect, image, param);
            return detectResult;
        }

        public PostResult GroupPost(string group, string tag, string feature, int image_maxsize, bool crop, byte[] image)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", group);
            param.Add("tag", tag);
            param.Add("feature", feature);
            //param.Add("image_maxsize", image_maxsize.ToString());
            //param.Add("crop", crop.ToString());
            var postResult = HttpMethod.Post<PostResult>(Constrants.url_g + "/" + group, image, param);
            return postResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">byairport</param>
        /// <param name="feature"></param>
        /// <param name="image_maxsize"></param>
        /// <param name="limit">返回数量</param>
        /// <param name="filter"></param>
        /// <param name="crop"></param>
        /// <param name="image"></param>
        public SearchResut Search(string group, string feature, int image_maxsize, int limit, string filter, bool crop, byte[] image)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", group);
            //param.Add("feature", feature);
            //param.Add("image_maxsize", image_maxsize.ToString());
            param.Add("limit", limit.ToString());
            //param.Add("filter", filter);
            //param.Add("crop", crop.ToString());

            var searchResult = HttpMethod.Post<SearchResut>(Constrants.url_search, image, param);
            return searchResult;
        }

        public VideoResult GetVideo(string url, float threshold)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("url", url);
            //抓拍的人脸张数（根据业务传）
            param.Add("limit", "10000");
            param.Add("crop", "face");
            //不转只进行抓拍
            //param.Add("group", "");
            //param.Add("threshold", threshold.ToString());
            //抓图间隔
            param.Add("interval", "1000");
            //抓拍人脸的最小大小
            param.Add("facemin", "100");
            //websocket名称
            param.Add("name", "snap");

            var videoResult = HttpMethod.Get<VideoResult>(Constrants.url_search, param);
            return videoResult;
        }

        public void Search()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            var videoResult = HttpMethod.Get<VideoResult>(Constrants.url_g, param);
        }

        public void QueryGroupPhotoes()
        {
            var groupname = "by";
            Dictionary<string, string> param = new Dictionary<string, string>();
            var result = HttpMethod.Get<GroupPhotoResult>(Constrants.url_g + "/" + groupname, param);

            if (result == null)
                return;

            Console.WriteLine(result.name);
            Console.WriteLine(result.total_photos);
            Console.WriteLine(result.next_cursor);
            foreach (var item in result.photos)
            {
                Console.WriteLine(string.Format("{0} {1}", item.Id, item.Tag));
            }
        }
    }
}
