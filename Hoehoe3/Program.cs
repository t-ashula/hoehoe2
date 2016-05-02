using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Hoehoe3.Properties;

namespace Hoehoe3
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;

            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = $"Global\\{{{appGuid}}}";

            // Need a place to store a return value in Mutex() constructor call
            bool createdNew;

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone")
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, mutexId, out createdNew, securitySettings))
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        // edited by acidzombie24
                        // mutex.WaitOne(Timeout.Infinite, false);
                        hasHandle = mutex.WaitOne(1000, false);
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact the mutex was abandoned in another process, it will still get aquired
                        hasHandle = true;
                    }

                    // Perform your work here.
                    if (!hasHandle)
                    {
                        TryBringUpPreviousApp();
                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
                    Application.ThreadException += ApplicationOnThreadException;
                    Application.ApplicationExit += ApplicationOnApplicationExit;

                    Application.Run(new MainForm());
                }
                finally
                {
                    // edited by acidzombie24, added if statemnet
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
        }

        private static void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            if (args.Exception is NotImplementedException)
            {
                MessageBox.Show(Resources.Program_ApplicationOnThreadException_Not_Implemented_);
            }
        }

        private static void TryBringUpPreviousApp()
        {
            MessageBox.Show(Resources.Program_TryBringUpPreviousApp_Another_instance_already_executed_);
            var p = Process.GetCurrentProcess();
            var candidate = Process.GetProcessesByName(p.ProcessName)
                .Where(process => process.Id != p.Id)
                .FirstOrDefault(process => string.Equals(process.MainModule.FileName, p.MainModule.FileName, StringComparison.CurrentCultureIgnoreCase));
            if (candidate == null || candidate.MainWindowHandle == IntPtr.Zero)
            {
                return;
            }

            var form = Control.FromHandle(candidate.MainWindowHandle) as Form;
            // TODO: Maximize
            form?.Activate();
        }
    }
}