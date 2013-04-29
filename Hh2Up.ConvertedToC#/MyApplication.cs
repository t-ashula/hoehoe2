using System;
using System.Threading;
using System.Windows.Forms;

namespace TweenUp
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.ThreadException += MyApplication_UnhandledException;
            Application.ApplicationExit += (_, __) => { };
            Application.Run(new Form1());
        }

        private static void MyApplication_UnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                My.Resources.ExceptionMessage + Environment.NewLine + e.Exception.Message,
                My.Resources.ExceptionTitle);
        }
    }
}