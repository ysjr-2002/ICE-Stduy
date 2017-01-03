using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NucCore
{
    public class User
    {
        public string Id { get; set; }

        public string Tag { get; set; }

        public string[] images { get; set; }

        public string[] groups { get; set; }

        public bool disabled { get; set; }
    }
}
