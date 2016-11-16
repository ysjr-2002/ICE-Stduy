using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.Repository
{
    public interface IRepository<T>
    {
        void Test();

        void Add(T t);

        void Update(T t);

        void Delete(T t);

        IEnumerable<T> Search();
    }
}
