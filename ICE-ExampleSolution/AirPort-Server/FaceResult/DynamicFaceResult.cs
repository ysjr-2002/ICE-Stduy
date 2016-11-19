using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.FaceResult
{
    public class Body
    {
        public string Image { get; set; }

        public Rect Rect { get; set; }
    }

    public class Clip
    {
        public int ExpiredFrom { get; set; }

        public string Name { get; set; }

        public int Now { get; set; }

        public int Seq { get; set; }

        public int ValidFrom { get; set; }
    }

    public class DynamicFace
    {
        public string Image { get; set; }

        public Rect Rect { get; set; }
    }

    public class FaceQuality
    {
        public float Blurness { get; set; }

        public float Expression { get; set; }

        public float Illumination { get; set; }

        public float Occlusion { get; set; }

        public float Pose { get; set; }
    }

    public class LeftEyeStatus
    {
        public float Darkglasses { get; set; }

        public float NoglassEyeclose { get; set; }

        public float NoglassEyeopen { get; set; }

        public float NormalglassEyeclose { get; set; }

        public float NormalglassEyeopen { get; set; }

        public float OtherOcclusion { get; set; }
    }

    public class Pose
    {
        public float Pitch { get; set; }

        public float Roll { get; set; }

        public float Yaw { get; set; }
    }

    public class RightEyeStatus
    {
        public float Darkglasses { get; set; }

        public float NoglassEyeclose { get; set; }

        public float NoglassEyeopen { get; set; }

        public float NormalglassEyeclose { get; set; }

        public float NormalglassEyeopen { get; set; }

        public float OtherOcclusion { get; set; }
    }

    public class Attrs
    {
        public float Age { get; set; }

        public FaceQuality FaceQuality { get; set; }

        public Gender Gender { get; set; }

        public LeftEyeStatus LeftEyeStatus { get; set; }

        public Pose Pose { get; set; }

        public RightEyeStatus RightEyeStatus { get; set; }
    }

    //public class Face2
    //{
    //    public Attrs Attrs { get; set; }

    //    public int Quality { get; set; }

    //    public Rect Rect { get; set; }
    //}

    public class Result
    {
        public Face Face { get; set; }

        public Group[] Groups { get; set; }
    }

    public class DynamicFaceResult
    {
        //public Body Body { get; set; }
       
        public string Type { get; set; }

        public int Track { get; set; }

        public int pts { get; set; }

        public long Timestamp { get; set; }

        public DynamicFace Face { get; set; }

        //public Full Full { get; set; }

        public Result Result { get; set; }

        public Clip Clip { get; set; }
    }
}
