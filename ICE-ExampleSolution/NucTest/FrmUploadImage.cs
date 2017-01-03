using Common;
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
    public partial class FrmUploadImage : Form
    {
        public FrmUploadImage()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.jpg|*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.IsEmpty() || textBox2.Text.IsEmpty())
            {
                return;
            }

            userid = textBox1.Text;
            filepath = textBox2.Text;
            DialogResult = DialogResult.Yes;
        }

        public string userid { get; set; }

        public string filepath { get; set; }
    }
}
