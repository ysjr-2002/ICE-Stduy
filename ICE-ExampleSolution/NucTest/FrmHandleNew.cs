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
    public partial class FrmHandleNew : Form
    {
        public FrmHandleNew()
        {
            InitializeComponent();
        }

        public string camera_id { get; set; }

        public string group_id { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            camera_id = textBox1.Text;
            group_id = textBox2.Text;
            DialogResult = DialogResult.Yes;
        }
    }
}
