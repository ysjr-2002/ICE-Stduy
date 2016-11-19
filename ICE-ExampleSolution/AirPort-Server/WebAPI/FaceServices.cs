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
            if (compareResult != null)
                return compareResult.score;
            else
                return -1;
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
        /// <param name="group">by</param>
        /// <param name="feature"></param>
        /// <param name="limit">返回数量</param>
        /// <param name="filter"></param>
        /// <param name="crop"></param>
        /// <param name="image"></param>
        public SearchResut Search(string group, string feature, int limit, string filter, bool crop, byte[] image)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("group", group);
            param.Add("feature", feature);
            //param.Add("image_maxsize", image_maxsize.ToString());
            param.Add("limit", limit.ToString());
            //param.Add("filter", filter);
            //param.Add("crop", crop.ToString());

            var searchResult = HttpMethod.PostNoImage<SearchResut>(Constrants.url_search, param);
            return searchResult;
        }

        public void GetVideo(string url, float threshold, string rtspId, Action<DynamicFaceResult> callback)
        {
            HttpMethod test = new WebAPI.HttpMethod();
            test.Websocket(url, threshold, callback);
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
