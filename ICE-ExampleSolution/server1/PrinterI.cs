using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ice;
using System.IO;
using System.Drawing;

namespace server1
{
    class PrinterI : demo.PrinterDisp_
    {
        public override void printString(string s, Current current__)
        {
            Console.WriteLine("welcome to ice:" + s);
            if (s == "s")
            {
                Console.WriteLine("关闭了");
                current__.adapter.getCommunicator().shutdown();
            }
        }

        public override void sendImage(byte[] seq, string name, Current current__)
        {
            MemoryStream ms = new MemoryStream(seq);
            Bitmap image = new Bitmap(ms);

            var filename = @"f:\" + name + ".jpg";
            image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
            Console.Out.WriteLine("保存成功->" + filename);
        }
    }
}
