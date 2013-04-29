using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace TweenUp.My
{
    partial class MyApplication : WindowsFormsApplicationBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(UseCompatibleTextRendering);
            MyProject.Application.Run(args);
        }
    }
}