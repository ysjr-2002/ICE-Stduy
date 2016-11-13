﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort_Server.FaceResult
{
    /// <summary>
    /// 特征提取
    /// </summary>
    public class FeatureResult
    {
        public Face Face { get; set; }

        public string Feature { get; set; }

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
}
