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
    public partial class FrmGroupUser : Form
    {
        public FrmGroupUser()
        {
            InitializeComponent();
        }

        public string group_id { get; set; }

        public string user_id { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            group_id = textBox2.Text;
            user_id = textBox1.Text;

            DialogResult = DialogResult.Yes;
        }
    }
}
