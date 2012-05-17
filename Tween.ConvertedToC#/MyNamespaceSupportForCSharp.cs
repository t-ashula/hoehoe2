using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Devices;

// This file was created by the VB to C# converter (SharpDevelop 4.2.0.8774-RC).
// It contains classes for supporting the VB "My" namespace in C#.
// If the VB application does not use the "My" namespace, or if you removed the usage
// after the conversion to C#, you can delete this file.

namespace Tween.My
{
    sealed partial class MyProject
    {
        [ThreadStatic]
        static MyApplication application;

        public static MyApplication Application
        {
            [DebuggerStepThrough]
            get
            {
                if (application == null)
                    application = new MyApplication();
                return application;
            }
        }

        [ThreadStatic]
        static MyComputer computer;

        public static MyComputer Computer
        {
            [DebuggerStepThrough]
            get
            {
                if (computer == null)
                    computer = new MyComputer();
                return computer;
            }
        }
    }

    partial class MyApplication : WindowsFormsApplicationBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(UseCompatibleTextRendering);
            MyProject.Application.Run(args);
        }
    }

    partial class MyComputer : Computer
    {
    }
}