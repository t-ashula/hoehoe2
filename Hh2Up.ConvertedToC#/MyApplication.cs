using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;

namespace TweenUp.My
{
    internal class MyApplication : WindowsFormsApplicationBase
    {
    }

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
            MyProject.Application.Log.DefaultFileLogWriter.Location = Microsoft.VisualBasic.Logging.LogFileLocation.ExecutableDirectory;
            MyProject.Application.Log.DefaultFileLogWriter.MaxFileSize = 16384;
            MyProject.Application.Log.DefaultFileLogWriter.AutoFlush = true;
            MyProject.Application.Log.DefaultFileLogWriter.Append = false;
            MyProject.Application.Log.WriteException(e.Exception, TraceEventType.Error, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            Interaction.MsgBox(Resources.ExceptionMessage + Constants.vbCrLf + e.Exception.Message, 0, Resources.ExceptionTitle);
        }
    }
}