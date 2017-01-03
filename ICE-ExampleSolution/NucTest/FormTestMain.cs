using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NucCore;
using Common;
using System.IO;
using System.Drawing.Imaging;

namespace NucTest
{
    public partial class FormTestMain : Form
    {
        private Nuc nuc = null;
        private Constrant constrant = null;

        public FormTestMain()
        {
            InitializeComponent();
        }

        private void Log(string content)
        {
            richTextBox1.AppendText(content + "\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.IsEmpty())
            {
                Log("请输入设备IP");
                return;
            }

            constrant = new Constrant();
            constrant.Init(textBox1.Text);
            nuc = new Nuc(constrant);

            button1.Enabled = false;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var cameras = nuc.GetCameras();
            Log("摄像机数量->" + cameras.Count);
            foreach (var c in cameras)
            {
                Log("Id:" + c.Id + " 名称:" + c.Name + " 地址:" + c.Url);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmCameraNew form = new FrmCameraNew();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                var camera = nuc.CreateCamera(form.CameraName, form.Rtsp);
                if (camera != null)
                {
                    Log("创建摄像机成功->" + camera.Id);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tbCameraid.Text.IsEmpty())
            {
                return;
            }

            var camera_id = tbCameraid.Text;
            nuc.DeleteCamera(camera_id);
            Log("删除摄像机成功");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var users = nuc.GetUsers();
            Log("用户数量->" + users.Count);
            foreach (var c in users)
            {
                Log("Id:" + c.Id + " tag:" + c.Tag + " 图像:" + (c.images?.Length ?? 0) + " 用户组:" + (c.groups?.Length ?? 0));
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmUserNew form = new NucTest.FrmUserNew();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                var user = nuc.CreatUser(form.Tag.ToString());
                Log("创建用户成功->" + user.Id);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tbUserid.Text.IsEmpty())
            {
                return;
            }

            var user_id = tbUserid.Text;
            nuc.DeleteUser(user_id);
            Log("删除用户成功");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            FrmUploadImage form = new NucTest.FrmUploadImage();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                nuc.PostUserImage(form.userid, form.filepath.ToImageByte());
                Log("加入图片成功");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var groups = nuc.GetGroups();
            Log("组数量->" + groups.Count);
            foreach (var c in groups)
            {
                Log("Id:" + c.Id + " tag:" + c.Tag + " 用户:" + (c.users?.Length ?? 0));
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FrmGroupNew form = new NucTest.FrmGroupNew();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                nuc.CreateGroup(form.Tag.ToString());
                Log("创建用户组成功");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (tbGroupid.Text.IsEmpty())
            {
                return;
            }

            var group_id = tbGroupid.Text;
            nuc.DeleteGroup(group_id);
            Log("删除组成功");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            FrmGroupUser form = new NucTest.FrmGroupUser();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                nuc.AddUserToGroup(form.group_id, form.user_id);
                Log("添加用户到组成功");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FrmGroupUser form = new NucTest.FrmGroupUser();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                nuc.DeleteGroupUser(form.group_id, form.user_id);
                Log("添加用户到组成功");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var handles = nuc.GetHandles();
            Log("数量->" + handles.Count);
            foreach (var c in handles)
            {
                Log("Id:" + c.id + " 摄像机id:" + c.camera_id + " 组id:" + c.group_id + " 工作状态:" + c.working);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            FrmHandleNew form = new NucTest.FrmHandleNew();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                nuc.CreateHandle(form.camera_id, form.group_id);
                Log("创建Handle成功");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (tbHandleid.Text.IsEmpty())
            {
                return;
            }
            nuc.DeleteHandle(tbHandleid.Text);
            Log("删除Handle成功");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (tbHandleid.Text.IsEmpty())
            {
                var msg = "请输入handleId";
                richTextBox1.AppendText(msg + "\n");
                richTextBox1.Select(richTextBox1.Text.Length - msg.Length, msg.Length);
                richTextBox1.SelectionColor = Color.Red;
                return;
            }

            nuc.Live(tbHandleid.Text, LiveCallback);
            Log("start live");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            nuc?.LiveStop();
        }


        private void LiveCallback(RecognizeResult result)
        {
            var base64Image = result.data.face.image;
            if (base64Image.IsEmpty())
                return;

            var buffer = Convert.FromBase64String(base64Image);
            MemoryStream ms = new MemoryStream(buffer);
            Bitmap bitmap = new Bitmap(ms);
            bitmap.Save("c:\\nuc.jpg", ImageFormat.Jpeg);
            bitmap.Dispose();
            ms.Close();
        }
    }
}
