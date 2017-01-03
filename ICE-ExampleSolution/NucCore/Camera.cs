using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NucCore
{
    public class Camera
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public attached_params attached_params { get; set; }
    }

    public class attached_params
    {

    }
}
