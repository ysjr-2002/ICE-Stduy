using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Client.Core
{
    public static class FaceServices
    {
        public static FaceProxy FaceProxy
        {
            get
            {
                var obj = ServiceLocator.Current.GetInstance(typeof(FaceProxy));
                return (FaceProxy)obj;
            }
        }
    }
}
