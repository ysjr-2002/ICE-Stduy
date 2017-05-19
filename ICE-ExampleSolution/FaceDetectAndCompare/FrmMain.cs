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

namespace FaceDetectAndCompare
{
    public partial class FrmMain : Form
    {
        private string threshold = "0.78";
        private string facemax = "8";
        private int status_ok = 0;
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

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var files = Directory.GetFiles(@"F:\data1", "*.jpg").ToList().
                OrderBy(s => key(s)).
                GroupBy(s => key(s)).
                Select(g => new
                {
                    key = g.Key,
                    c = g.Count()
                });

            sw.Stop();
            Console.WriteLine("耗时->" + sw.ElapsedMilliseconds);

            //Task.Factory.StartNew(() =>
            //{
            //    foreach (var file in files)
            //    {
            //        //Detect(file, Path.GetFileName(file));
            //        Console.WriteLine(Path.GetFileName(file));
            //    }
            //});

            foreach (var item in files)
            {
                log.Info(item.key + "->" + item.c);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            IceApp app = new IceApp();
            app.main(null, "config.client");
        }

        private void Detect(string imagefile, string filename)
        {
            var base64Image = imagefile.FileToBase64();

            var sb = new StringBuilder();
            sb.Append("imgData".ElementImage(base64Image));
            sb.Append("threshold".ElementText(threshold));
            sb.Append("maxImageCount".ElementText(facemax));
            var data = sb.ToString();

            var xml = XmlParse.GetXml("staticDetect", data);
            Stopwatch sw = Stopwatch.StartNew();
            var content = IceApp.facePxy.send(xml);
            sw.Stop();

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
            var persons = doc.SelectNodes("/xml/persons/person");
            var facecount = persons.Count;

            var logcontent = "detect[{0}] found {1} person(s) ({2}ms)";
            logcontent = string.Format(logcontent, filename, facecount, sw.ElapsedMilliseconds);
            log.Info(logcontent);

            DrawFace(persons, filename);
        }

        private void DrawFace(XmlNodeList faces, string filename)
        {
            var index = 0;
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
                compare(faceimage, "", ref similarity, ref elapseillseconds);

                var content = "img[(0)]-{1} similarity:{2} ({3}ms)";
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
