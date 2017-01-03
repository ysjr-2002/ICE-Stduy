using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NucCore
{
    public class Rect
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Face
    {
        public Rect rect { get; set; }
        public string image { get; set; }
    }

    public class Person
    {
        public string id { get; set; }
        public string tag { get; set; }
        public double confidence { get; set; }
        public int feature_id { get; set; }
    }

    public class Attr
    {
        public double age { get; set; }
        public double male { get; set; }
        public double female { get; set; }
    }

    public class Data
    {
        public int track { get; set; }
        public int timestamp { get; set; }
        public string status { get; set; }
        public double quality { get; set; }
        public Face face { get; set; }
        public Person person { get; set; }
        public Attr attr { get; set; }
    }

    public class RecognizeResult
    {
        public string type { get; set; }
        public Data data { get; set; }
    }

    /// <summary>
    /// 跟踪状态
    /// </summary>
    public enum TractStatus
    {
        /// <summary>
        /// 多次
        /// </summary>
        recognizing,
        /// <summary>
        /// 一次输出
        /// </summary>
        recognized,
        /// <summary>
        /// 一次输出
        /// </summary>
        unrecognized,
        /// <summary>
        /// 一次消失
        /// </summary>
        gone,
    }
}
