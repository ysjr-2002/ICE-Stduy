using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectAndCompare
{
    class FileItem
    {
        public int FileGroupId { get; set; }

        public string CardFile { get; set; }

        public List<string> OtherFiles { get; set; }
    }
}
