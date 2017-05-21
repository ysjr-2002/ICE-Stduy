using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectAndCompare
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args == null || args.Length >= 2)
            {
                var index = args[0];
                var dataroot = args[1];
                var smallpic = args[2];
                Application.Run(new FrmMain(index, dataroot, smallpic));
            }
            else
            {
                Application.Run(new FrmMain());
            }
        }
    }
}
