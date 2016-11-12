using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace System
{
    public static class Util
    {
        public static string OpenFileDialog()
        {
            var filter = "Jpg|*.jpg|Png|*.png";
            OpenFileDialog dlgFile = new OpenFileDialog();
            dlgFile.Multiselect = false;
            dlgFile.Filter = filter;
            var dialogResult = dlgFile.ShowDialog();
            if (dialogResult == DialogResult.OK)
                return dlgFile.FileName;
            else
                return string.Empty;
        }
    }
}
