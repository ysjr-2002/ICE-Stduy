using Common;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirPort.Client
{
    public class PopupWindowBase : Window
    {
        protected const int status_ok = 0;
        protected const string community_error = "通讯异常！";
        protected const string callElapsed_Template = "{0}ms";

        public PopupWindowBase()
        {
            this.Width = 1024;
            this.Height = 768;
            //this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ShowInTaskbar = false;
        }

        protected virtual void Item(string content)
        {
            LogHelper.Info(content);
        }

        protected virtual void TipDialog(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected virtual void WarnDialog(string message)
        {
            MessageBox.Show(message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected MessageBoxResult ConfirmDialog(string message)
        {
            return MessageBox.Show(message, "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
