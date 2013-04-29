using System;
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
            Application.ThreadException += (sender, e) => MessageBox.Show(
                Resources.ExceptionMessage + Environment.NewLine + e.Exception.Message,
                Resources.ExceptionTitle);
            Application.Run(new Form1());
        }
    }
}