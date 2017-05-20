using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ice;
using Common;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;

namespace FaceDetectAndCompare
{
    public partial class FrmMain : Form
    {
        private string threshold = "0.78";
        private string facemax = "5";
        private int status_ok = 0;
        private const string logcontent = "detect[{0}] found {1} person(s) ({2}ms)";
        private static log4net.ILog log = log4net.LogManager.GetLogger("face");
        public FrmMain()
        {
            InitializeComponent();
        }

        private int key(string path)
        {
            var filename = Path.GetFileName(path);
            var idpos = filename.IndexOf('_');
            var id = filename.Substring(0, idpos);
            return Int32.Parse(id);
        }

        private List<FileItem> fileItems = new List<FileItem>();

        private async void button1_Click(object sender, EventArgs e)
        {
            File.Delete("data.json");
            Stopwatch sw = Stopwatch.StartNew();
            var files = Directory.GetFiles(@"d:\data1", "*.jpg").ToList().
                OrderBy(s => key(s)).ToArray();
            sw.Stop();

            var first = files.Take(files.Length / 2).ToArray();
            var second = files.Skip(files.Length / 2).ToArray();

            sw.Restart();
            var t1 = Task.Factory.StartNew(() =>
            {
                Work(files);
            });
            //var t2 = Task.Factory.StartNew(() =>
            //{
            //    Work(second);
            //});
            await t1;
            //await t2;
            Console.WriteLine("耗时->" + sw.ElapsedMilliseconds);
            Console.WriteLine("结束->" + fileItems.Count);

            JavaScriptSerializer js = new JavaScriptSerializer();
            var json = js.Serialize(fileItems);
            File.WriteAllText("data.json", json);
        }

        private static readonly object sync = new object();
        private void Work(string[] files)
        {
            foreach (var file in files)
            {
                var filegroupId = GetFileId(file);
                FileItem exist = null;
                lock (sync)
                {
                    exist = fileItems.SingleOrDefault(s => s.ID == filegroupId);
                }
                if (exist == null)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.label1.Text = filegroupId;
                    }));
                    var fi = GetFileItem(filegroupId, file, files);
                    if (string.IsNullOrEmpty(fi.CardFile))
                    {
                        continue;
                    }
                    lock (sync)
                    {
                        fileItems.Add(fi);
                    }
                }
            }
        }

        private string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }

        private string GetFileId(string filepath)
        {
            var filename = Path.GetFileName(filepath);
            var start = filename.IndexOf('_');
            var id = filename.Substring(0, start);
            return id;
        }

        private FileItem GetFileItem(string id, string filepath, IEnumerable<string> files)
        {
            var idfiles = files.Where(s => Path.GetFileName(s).StartsWith(id + "_"));
            FileItem fi = new FileItem();
            fi.ID = id;
            fi.CardFile = idfiles.FirstOrDefault(s => s.Contains("card"));
            fi.OtherFiles = idfiles.Where(s => !s.Contains("card")).ToList();
            return fi;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(new Action(() =>
            {
                IceApp app = new IceApp();
                app.main(new string[1] { "" }, "config.client");
            }));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            fileItems = js.Deserialize<List<FileItem>>(File.ReadAllText("data.json"));
            Stopwatch sw = Stopwatch.StartNew();
            fileItems = fileItems.Where(s => Int32.Parse(s.ID) >= 1507).ToList();
            await Task.Run(() =>
            {
                foreach (var fi in fileItems)
                {
                    foreach (var file in fi.OtherFiles)
                    {
                        Detect(file, fi.CardFile);
                    }
                }
            });
            sw.Stop();
            log.Info("总比对耗时->" + sw.ElapsedMilliseconds);
        }

        private void Detect(string imagefile, string cardfile)
        {
            var filename = GetFileName(imagefile);
            var base64Image = imagefile.FileToBase64();
            var sb = new StringBuilder();
            sb.Append("imgData".ElementImage(base64Image));
            sb.Append("threshold".ElementText(threshold));
            sb.Append("maxImageCount".ElementText(facemax));
            var sendcontent = sb.ToString();

            var xml = XmlParse.GetXml("staticDetect", sendcontent);
            Stopwatch sw = Stopwatch.StartNew();
            var content = IceApp.facePxy.send(xml);
            sw.Stop();

            if (content.IsEmpty())
            {
                return;
            }
            var facecount = 0;
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != status_ok)
            {
                var data = string.Format(logcontent, filename, facecount, sw.ElapsedMilliseconds);
                log.Info(data);
                return;
            }
            var persons = doc.SelectNodes("/xml/persons/person");
            facecount = persons.Count;

            var temp = string.Format(logcontent, filename, facecount, sw.ElapsedMilliseconds);
            log.Info(temp);

            DrawFace(persons, filename, cardfile);
        }

        private void DrawFace(XmlNodeList faces, string filename, string cardfile)
        {
            var index = 0;
            var cardfilebase = cardfile.FileToBase64();
            foreach (XmlNode face in faces)
            {
                var quality = face.GetNodeText("quality").ToFloat();
                var x = face.GetNodeText("posX").ToInt32();
                var y = face.GetNodeText("posY").ToInt32();
                var h = face.GetNodeText("imgHeight").ToInt32();
                var w = face.GetNodeText("imgWidth").ToInt32();
                var faceimage = face.GetNodeText("imgData");

                var similarity = "";
                long elapseillseconds = 0;
                compare(faceimage, cardfilebase, ref similarity, ref elapseillseconds);

                var content = "img[{0}]-{1} similarity:{2} ({3}ms)";
                content = string.Format(content, filename, index, similarity, elapseillseconds);
                log.Info(content);
                index++;
            }
        }

        private void compare(string imagebaseA, string imagebaseB, ref string similarity, ref long elapsemillseconds)
        {
            var sb = new StringBuilder();
            sb.Append("srcImgData".ElementImage(imagebaseA));
            sb.Append("destImgData".ElementImage(imagebaseB));
            var xml = XmlParse.GetXml("compare", sb.ToString());
            Stopwatch sw = Stopwatch.StartNew();
            var content = IceApp.facePxy.send(xml);
            sw.Stop();
            elapsemillseconds = sw.ElapsedMilliseconds;

            if (content.IsEmpty())
            {
                return;
            }
            var doc = XmlParse.LoadXml(content);
            var code = doc.GetNodeText("code");
            if (code.ToInt32() != status_ok)
            {
                return;
            }
            similarity = doc.GetNodeText("similarity");
            sb.Clear();
        }
    }
}
