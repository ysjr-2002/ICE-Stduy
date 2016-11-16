using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.FaceResult
{
    /// <summary>
    /// 特征提取
    /// </summary>
    public class FeatureResult
    {
        //特征提取人脸
        public Face Face { get; set; }
        //特征码
        public string Feature { get; set; }
        //人脸位置信息
        public Rect Rect { get; set; }
    }
    /// <summary>
    /// 人脸检测
    /// </summary>
    public class DetectResult
    {
        public DetectFace[] Faces { get; set; }
        public Rect ImageRect { get; set; }
    }

    /// <summary>
    /// 比对
    /// </summary>
    public class CompareResult
    {
        public Face face1 { get; set; }

        public Face face2 { get; set; }

        public float score { get; set; }
    }

    /// <summary>
    /// 入库结果
    /// </summary>
    public class PostResult
    {
        public GroupFace face { get; set; }

        public int id { get; set; }

        public Rect image_rect { get; set; }
    }

    /// <summary>
    /// 搜索结果
    /// </summary>
    public class SearchResut
    {
        public GroupFace face { get; set; }

        public Group[] groups { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VideoResult
    {

    }
}
