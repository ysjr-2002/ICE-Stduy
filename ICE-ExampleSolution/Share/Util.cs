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
            var filter = "jpg文件|*.jpg|png文件|*.png";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = filter;
            dialog.Title = "选择文件";
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                return dialog.FileName;
            else
                return string.Empty;
        }
    }
}
