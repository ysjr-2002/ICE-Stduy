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
using System.Threading;

namespace FaceDetectAndCompare
{
    public partial class FrmMain : Form
    {
        private string threshold = "0.01";
        private string facemax = "4";
        private int status_ok = 0;
        private const string detect_logtemplate = "detect[{0}] found {1} person(s) ({2}ms) best({3})";
        private const string compare_logtemplate = "img[{0}]-{1} similarity:{2} ({3}ms)";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("face");

        private string root = "";
        private List<FileItem> fileItems = new List<FileItem>();
        private static readonly object sync = new object();

        public FrmMain()
        {
            InitializeComponent();
        }

        public FrmMain(string index, string root, string smallpic)
        {
            InitializeComponent();
            this.Text = this.Text + "-" + index;
            this.root = root;
            this.smallpicPath = smallpic;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(new Action(() =>
            {
                IceApp app = new IceApp();
                var startup = System.Windows.Forms.Application.StartupPath;
                startup = Path.Combine(startup, "config.client");
                app.main(new string[1] { "" }, startup);
            }));

            if (!root.IsEmpty())
            {
                Thread.Sleep(2000);
                if (!Directory.Exists(smallpicPath))
                {
                    Directory.CreateDirectory(smallpicPath);
                }
                btnDetectAndCompare.PerformClick();
            }
        }

        //文件分组
        private async void btnGrouping_Click(object sender, EventArgs e)
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

            log.Info("总文件数量->" + files.Length);

            int threadcount = Int32.Parse(txtThreadcount.Text);
            List<string[]> list = Tools.DivideArray(files, threadcount);
            Task[] tasks = new Task[threadcount];

            sw.Restart();
            tasks[0] = Task.Factory.StartNew(() =>
            {
                FileGroup(list[0]);
            });
            tasks[1] = Task.Factory.StartNew(() =>
            {
                FileGroup(list[1]);
            });

            for (int i = 0; i < threadcount; i++)
            {
                await tasks[i];
            }
            sw.Stop();
            log.Info("文件分组耗时->" + sw.ElapsedMilliseconds);
            log.Info("总文件组数量->" + fileItems.Count);

            JavaScriptSerializer js = new JavaScriptSerializer();
            var json = js.Serialize(fileItems);
            File.WriteAllText(datafile, json);
        }

        private void FileGroup(string[] files)
        {
            foreach (var file in files)
            {
                var filegroupId = Tools.GetFileGroupId(file);
                FileItem exist = null;
                lock (sync)
                {
                    exist = fileItems.SingleOrDefault(s => s.FileGroupId == filegroupId);
                }
                if (exist == null)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.label1.Text = filegroupId.ToString();
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

        int count = 1;
        /// <summary>
        /// 分组和检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDetectAndComapare_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(root))
            {
                var folder = new FolderBrowserDialog();
                folder.SelectedPath = root;
                var dialog = folder.ShowDialog();
                if (dialog != DialogResult.OK)
                {
                    return;
                }
                root = folder.SelectedPath;
            }

            if (smallpicPath.IsEmpty())
            {
                MessageBox.Show("请设置小图保存路径！");
                return;
            }
            facemax = txtMaxcount.Text;

            var datafile = Path.Combine(root, "data.json");
            var content = File.ReadAllText(datafile);
            JavaScriptSerializer js = new JavaScriptSerializer();
            fileItems = js.Deserialize<List<FileItem>>(content).OrderBy(s => s.FileGroupId).ToList();
            Stopwatch sw = Stopwatch.StartNew();
            label3.Text = "开始工作...";
            log.Info("开始检测和分析->" + DateTime.Now.ToStandard());
            await Task.Run(() =>
            {
                foreach (var fi in fileItems)
                {
                    var groupInfo = string.Format("第{0:d4}组", count);
                    log.Info(groupInfo);
                    this.Invoke(new Action(() =>
                    {
                        this.label1.Text = groupInfo;
                    }));
                    foreach (var file in fi.OtherFiles)
                    {
                        DetectFace(file, fi.CardFile);
                    }
                    count++;
                }
            });
            sw.Stop();
            log.Info("总比对耗时->" + (int)(sw.ElapsedMilliseconds / 1000) + "秒");
            log.Info("结束检测和分析->" + DateTime.Now.ToStandard());
            MessageBox.Show("分析结束！");
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
                var data = string.Format(detect_logtemplate, filename, facecount, sw.ElapsedMilliseconds, "0.00f");
                log.Info(data);
                return;
            }
            var persons = doc.SelectNodes("/xml/persons/person");
            facecount = persons.Count;

            List<string> logs = new List<string>();
            var bestSimilarity = CompareFaces(persons, filename, cardfile, logs);

            //检测日志
            var temp = string.Format(detect_logtemplate, filename, facecount, sw.ElapsedMilliseconds, bestSimilarity);
            log.Info(temp);

            foreach (var piccomparelog in logs)
            {
                log.Info(piccomparelog);
            }
        }

        private float CompareFaces(XmlNodeList faces, string filename, string cardfile, List<string> logs)
        {
            var index = 0;
            var cardfilebase = cardfile.FileToBase64();
            var best = 0.0f;
            foreach (XmlNode face in faces)
            {
                //var quality = face.GetNodeText("quality").ToFloat();
                var x = face.GetNodeText("posX").ToInt32();
                var y = face.GetNodeText("posY").ToInt32();
                var h = face.GetNodeText("imgHeight").ToInt32();
                var w = face.GetNodeText("imgWidth").ToInt32();
                var faceimage = face.GetNodeText("imgData");

                var similarity = "";
                var elapseillseconds = (long)0;
                Compare(faceimage, cardfilebase, ref similarity, ref elapseillseconds);

                var fn = Path.GetFileNameWithoutExtension(filename);

                var smallpicname = string.Format("{0}_{1}_{2}_{3}", fn, index, w, h);
                smallpicname = string.Concat(smallpicname, ".jpg");
                var temp = string.Format(compare_logtemplate, smallpicname, index, similarity, elapseillseconds);
                logs.Add(temp);
                //log.Info(temp);

                SaveImage(faceimage.Base64ToByte(), smallpicname);
                index++;

                if (similarity.ToFloat() > best)
                    best = similarity.ToFloat();
            }
            return best;
        }

        private void SaveImage(byte[] buffer, string filename)
        {
            var filepath = Path.Combine(smallpicPath, filename);
            File.WriteAllBytes(filepath, buffer);
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

        private void button1_Click(object sender, EventArgs e)
        {
            var folder = new OpenFileDialog();
            folder.Multiselect = false;
            var dialog = folder.ShowDialog();
            if (dialog != DialogResult.OK)
            {
                return;
            }
            var filepath = folder.FileName;

            DetectFace(filepath, "");
        }

        string smallpicPath = "";
        private void button2_Click(object sender, EventArgs e)
        {
            var folder = new FolderBrowserDialog();
            folder.SelectedPath = root;
            var dialog = folder.ShowDialog();
            if (dialog != DialogResult.OK)
            {
                return;
            }
            smallpicPath = folder.SelectedPath;
        }
    }
}
