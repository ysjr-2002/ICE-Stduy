using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.Core
{
    public static class FileManager
    {
        private static string root = "";

        static FileManager()
        {
            root = System.Configuration.ConfigurationManager.AppSettings["root"];
            if (Directory.Exists(root) == false)
                Directory.CreateDirectory(root);
        }

        public static string SaveFile(string content, string faceId, string prefix)
        {
            if (!content.IsEmpty())
            {
                var folder = GetFolder();
                var filename = string.Concat(faceId, prefix, ".bin");
                var filepath = Path.Combine(folder, filename);
                File.WriteAllText(filepath, content);
                return filepath;
            }
            else
                return string.Empty;
        }

        public static string ReadFile(string filepath)
        {
            if (File.Exists(filepath))
                return File.ReadAllText(filepath);
            else
                return string.Empty;
        }

        private static string GetFolder()
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var folder = Path.Combine(root, day);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }
    }
}
