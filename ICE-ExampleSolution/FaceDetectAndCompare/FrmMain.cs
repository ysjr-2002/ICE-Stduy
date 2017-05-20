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
        private const string detect_logtemplate = "detect[{0}] found {1} person(s) ({2}ms)";
        private const string compare_logtemplate = "img[{0}]-{1} quality:{2} similarity:{3} ({4}ms)";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("face");

        private string root = @"F:\airdata\wrong\wrong";
        private List<FileItem> fileItems = new List<FileItem>();
        private static readonly object sync = new object();
        public FrmMain()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var folder = new FolderBrowserDialog();
            folder.SelectedPath = root;
            var dialog = folder.ShowDialog();
            if (dialog != DialogResult.OK)
            {
                return;
            }
            root = folder.SelectedPath;

            var datafile = Path.Combine(root, "data.json");
            File.Delete(datafile);

            Stopwatch sw = Stopwatch.StartNew();
            var files = Directory.GetFiles(root, "*.jpg").ToList().
                OrderBy(s => Tools.GetFileGroupId(s)).ToArray();
            sw.Stop();

            int threadcount = Int32.Parse(txtThreadcount.Text);
            List<string[]> list = Tools.DivideArray(files, threadcount);
            Task[] tasks = new Task[threadcount];

            sw.Restart();
            tasks[0] = Task.Factory.StartNew(() =>
            {
                Work(list[0]);
            });
            tasks[1] = Task.Factory.StartNew(() =>
            {
                Work(list[1]);
            });

            for (int i = 0; i < threadcount; i++)
            {
                await tasks[i];
            }
            sw.Stop();
            Console.WriteLine("耗时->" + sw.ElapsedMilliseconds);
            Console.WriteLine("文件数量->" + fileItems.Count);

            JavaScriptSerializer js = new JavaScriptSerializer();
            var json = js.Serialize(fileItems);
            File.WriteAllText(datafile, json);
        }

        private void Work(string[] files)
        {
            foreach (var file in files)
            {
                var filegroupId = Tools.GetFileGroupId(file);
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
                    var fi = Tools.GetFileItem(filegroupId, file, files);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //Task.Run(new Action(() =>
            //{
            //    IceApp app = new IceApp();
            //    app.main(new string[1] { "" }, "config.client");
            //}));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var datafile = Path.Combine(root, "data.json");
            JavaScriptSerializer js = new JavaScriptSerializer();
            fileItems = js.Deserialize<List<FileItem>>(datafile);
            Stopwatch sw = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                foreach (var fi in fileItems)
                {
                    foreach (var file in fi.OtherFiles)
                    {
                        DetectFace(file, fi.CardFile);
                    }
                }
            });
            sw.Stop();
            log.Info("总比对耗时->" + sw.ElapsedMilliseconds);
        }

        private void DetectFace(string filepath, string cardfile)
        {
            var filename = Tools.GetFileName(filepath);
            var base64Image = filepath.FileToBase64();
            var sb = new StringBuilder();
            sb.Append("imgData".ElementImage(base64Image));
            sb.Append("threshold".ElementText(threshold));
            sb.Append("maxImageCount".ElementText(facemax));
            var xmlcontent = sb.ToString();

            var sendxml = XmlParse.GetXml("staticDetect", xmlcontent);
            Stopwatch sw = Stopwatch.StartNew();
            var content = IceApp.facePxy.send(sendxml);
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
                var data = string.Format(detect_logtemplate, filename, facecount, sw.ElapsedMilliseconds);
                log.Info(data);
                return;
            }
            var persons = doc.SelectNodes("/xml/persons/person");
            facecount = persons.Count;
            var temp = string.Format(detect_logtemplate, filename, facecount, sw.ElapsedMilliseconds);
            log.Info(temp);
            CompareFaces(persons, filename, cardfile);
        }

        private void CompareFaces(XmlNodeList faces, string filename, string cardfile)
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
                var elapseillseconds = (long)0;
                Compare(faceimage, cardfilebase, ref similarity, ref elapseillseconds);

                var temp = string.Format(compare_logtemplate, filename, index, quality, similarity, elapseillseconds);
                log.Info(temp);
                index++;
            }
        }

        private void Compare(string imagebaseA, string imagebaseB, ref string similarity, ref long elapsemillseconds)
        {
            var sb = new StringBuilder();
            sb.Append("srcImgData".ElementImage(imagebaseA));
            sb.Append("destImgData".ElementImage(imagebaseB));
            var sendxml = XmlParse.GetXml("compare", sb.ToString());
            Stopwatch sw = Stopwatch.StartNew();
            var content = IceApp.facePxy.send(sendxml);
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
