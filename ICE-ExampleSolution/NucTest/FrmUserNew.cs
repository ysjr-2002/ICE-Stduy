﻿using System;
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
    public partial class FrmUserNew : Form
    {
        public FrmUserNew()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tag = textBox1.Text;
            DialogResult = DialogResult.Yes;
        }
    }
}
