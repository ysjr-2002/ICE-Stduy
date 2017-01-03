using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NucTest
{
    public partial class FrmCameraNew : Form
    {
        public FrmCameraNew()
        {
            InitializeComponent();
        }

        public string CameraName { get; set; }

        public string Rtsp { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            CameraName = textBox1.Text;
            Rtsp = textBox2.Text;
            DialogResult = DialogResult.Yes;
        }
    }
}
