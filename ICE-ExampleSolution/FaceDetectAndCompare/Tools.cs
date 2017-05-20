using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectAndCompare
{
    class Tools
    {
        public static List<T[]> DivideArray<T>(T[] array, int i)
        {
            var len = array.Length / i;
            var mod = array.Length % i;
            List<T[]> list = new List<T[]>();
            for (int n = 0; n < i; n++)
            {
                if (n < i - 1)
                {
                    T[] sub = array.Skip(n * len).Take(len).ToArray();
                    list.Add(sub);
                }
                else
                {
                    T[] sub = array.Skip(n * len).Take(len + mod).ToArray();
                    list.Add(sub);
                }
            }
            return list;
        }

        public static string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }

        public static string GetFileGroupId(string filepath)
        {
            var filename = Path.GetFileName(filepath);
            var start = filename.IndexOf('_');
            var id = filename.Substring(0, start);
            return id;
        }

        public static FileItem GetFileItem(string groupId, string filepath, IEnumerable<string> files)
        {
            var idfiles = files.Where(s => Path.GetFileName(s).StartsWith(groupId + "_"));
            FileItem fi = new FileItem();
            fi.FileGroupId = groupId;
            fi.CardFile = idfiles.FirstOrDefault(s => s.Contains("card"));
            fi.OtherFiles = idfiles.Where(s => !s.Contains("card")).ToList();
            return fi;
        }
    }
}
