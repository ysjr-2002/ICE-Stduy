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

        IEnumerable<T> Search(Pagequery page, string faceId, string uuid, string code, string[] tags);

        IEnumerable<person> Search1VN(Pagequery page, string[] faceId, string[] tags, long validTime);

        void AddPersonTag(string faceId, string[] tags);

        void UpdatePersonTag(string faceId, string[] tags);

        int DeleteByTags(string[] tags);

        string[] GetPersonTags(string faceId);
    }

    public class Pagequery
    {
        public int Offset { get; set; }
        public int Pagesize { get; set; }
        public int TotalCount { get; set; }
    }
}
