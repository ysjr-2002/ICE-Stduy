using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Client.Model
{
    class PersonInfo
    {
        public string faceId { get; set; }

        public string uuid { get; set; }

        public string code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public string FaceImage1 { get; set; }

        public bool HasSignatureCode1 { get; set; }

        public string FaceImage2 { get; set; }

        public bool HasSignatureCode2 { get; set; }

        public string FaceImage3 { get; set; }

        public bool HasSignatureCode3 { get; set; }
    }
}
