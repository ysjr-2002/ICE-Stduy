﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort_Server
{
    public class face_quality
    {
        public decimal blurness { get; set; }

        public decimal expression { get; set; }

        public decimal illumination { get; set; }

        public decimal occlusion { get; set; }

        public decimal pose { get; set; }
    }

    public class Gender
    {
        public float Female { get; set; }

        public float Male { get; set; }
    }

    public class EyeStatus
    {
        public int Darkglasses { get; set; }

        public int NoglassEyeclose { get; set; }

        public int NoglassEyeopen { get; set; }

        public int NormalglassEyeclose { get; set; }

        public int NormalglassEyeopen { get; set; }

        public int OtherOcclusion { get; set; }
    }

    public class Pose
    {
        public float Pitch { get; set; }

        public float Roll { get; set; }

        public float Yaw { get; set; }
    }

    public class Attrs
    {
        public Pose Pose { get; set; }

        public float Age { get; set; }

        public Gender Gender { get; set; }

        public face_quality Face_quality { get; set; }
    }

    public class Face
    {
        public Rect Rect { get; set; }

        public Attrs Attrs { get; set; }

        public float Quality { get; set; }
    }

    public class DetectFace
    {
        public Rect Rect { get; set; }
        public Attrs Attrs { get; set; }
        public float Quality { get; set; }
        public crop crop { get; set; }
    }

    public class crop
    {
        public Rect rect { get; set; }
        public string image { get; set; }
    }

    public class Rect
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
