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

        [ThreadStatic]
        static User user;

        public static User User
        {
            [DebuggerStepThrough]
            get
            {
                if (user == null)
                    user = new User();
                return user;
            }
        }

        [ThreadStatic]
        static MyForms forms;

        public static MyForms Forms
        {
            [DebuggerStepThrough]
            get
            {
                if (forms == null)
                    forms = new MyForms();
                return forms;
            }
        }

        internal sealed class MyForms
        {
            global::Tween.TweenMain TweenMain_instance;
            bool TweenMain_isCreating;

            public global::Tween.TweenMain TweenMain
            {
                [DebuggerStepThrough]
                get { return GetForm(ref TweenMain_instance, ref TweenMain_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref TweenMain_instance, value); }
            }

            global::Tween.ListManage ListManage_instance;
            bool ListManage_isCreating;

            public global::Tween.ListManage ListManage
            {
                [DebuggerStepThrough]
                get { return GetForm(ref ListManage_instance, ref ListManage_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref ListManage_instance, value); }
            }

            global::Tween.MessageForm MessageForm_instance;
            bool MessageForm_isCreating;

            public global::Tween.MessageForm MessageForm
            {
                [DebuggerStepThrough]
                get { return GetForm(ref MessageForm_instance, ref MessageForm_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref MessageForm_instance, value); }
            }

            global::Tween.MyLists MyLists_instance;
            bool MyLists_isCreating;

            public global::Tween.MyLists MyLists
            {
                [DebuggerStepThrough]
                get { return GetForm(ref MyLists_instance, ref MyLists_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref MyLists_instance, value); }
            }

            global::Tween.OpenURL OpenURL_instance;
            bool OpenURL_isCreating;

            public global::Tween.OpenURL OpenURL
            {
                [DebuggerStepThrough]
                get { return GetForm(ref OpenURL_instance, ref OpenURL_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref OpenURL_instance, value); }
            }

            global::Tween.SearchWord SearchWord_instance;
            bool SearchWord_isCreating;

            public global::Tween.SearchWord SearchWord
            {
                [DebuggerStepThrough]
                get { return GetForm(ref SearchWord_instance, ref SearchWord_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref SearchWord_instance, value); }
            }

            global::Tween.AppendSettingDialog AppendSettingDialog_instance;
            bool AppendSettingDialog_isCreating;

            public global::Tween.AppendSettingDialog AppendSettingDialog
            {
                [DebuggerStepThrough]
                get { return GetForm(ref AppendSettingDialog_instance, ref AppendSettingDialog_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref AppendSettingDialog_instance, value); }
            }

            global::Tween.AtIdSupplement AtIdSupplement_instance;
            bool AtIdSupplement_isCreating;

            public global::Tween.AtIdSupplement AtIdSupplement
            {
                [DebuggerStepThrough]
                get { return GetForm(ref AtIdSupplement_instance, ref AtIdSupplement_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref AtIdSupplement_instance, value); }
            }

            global::Tween.AuthBrowser AuthBrowser_instance;
            bool AuthBrowser_isCreating;

            public global::Tween.AuthBrowser AuthBrowser
            {
                [DebuggerStepThrough]
                get { return GetForm(ref AuthBrowser_instance, ref AuthBrowser_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref AuthBrowser_instance, value); }
            }

            global::Tween.ShowUserInfo ShowUserInfo_instance;
            bool ShowUserInfo_isCreating;

            public global::Tween.ShowUserInfo ShowUserInfo
            {
                [DebuggerStepThrough]
                get { return GetForm(ref ShowUserInfo_instance, ref ShowUserInfo_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref ShowUserInfo_instance, value); }
            }

            global::Tween.HashtagManage HashtagManage_instance;
            bool HashtagManage_isCreating;

            public global::Tween.HashtagManage HashtagManage
            {
                [DebuggerStepThrough]
                get { return GetForm(ref HashtagManage_instance, ref HashtagManage_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref HashtagManage_instance, value); }
            }

            global::Tween.InputTabName InputTabName_instance;
            bool InputTabName_isCreating;

            public global::Tween.InputTabName InputTabName
            {
                [DebuggerStepThrough]
                get { return GetForm(ref InputTabName_instance, ref InputTabName_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref InputTabName_instance, value); }
            }

            global::Tween.ListAvailable ListAvailable_instance;
            bool ListAvailable_isCreating;

            public global::Tween.ListAvailable ListAvailable
            {
                [DebuggerStepThrough]
                get { return GetForm(ref ListAvailable_instance, ref ListAvailable_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref ListAvailable_instance, value); }
            }

            global::Tween.EventViewerDialog EventViewerDialog_instance;
            bool EventViewerDialog_isCreating;

            public global::Tween.EventViewerDialog EventViewerDialog
            {
                [DebuggerStepThrough]
                get { return GetForm(ref EventViewerDialog_instance, ref EventViewerDialog_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref EventViewerDialog_instance, value); }
            }

            global::Tween.DialogAsShieldIcon DialogAsShieldIcon_instance;
            bool DialogAsShieldIcon_isCreating;

            public global::Tween.DialogAsShieldIcon DialogAsShieldIcon
            {
                [DebuggerStepThrough]
                get { return GetForm(ref DialogAsShieldIcon_instance, ref DialogAsShieldIcon_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref DialogAsShieldIcon_instance, value); }
            }

            global::Tween.FormInfo FormInfo_instance;
            bool FormInfo_isCreating;

            public global::Tween.FormInfo FormInfo
            {
                [DebuggerStepThrough]
                get { return GetForm(ref FormInfo_instance, ref FormInfo_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref FormInfo_instance, value); }
            }

            global::Tween.FilterDialog FilterDialog_instance;
            bool FilterDialog_isCreating;

            public global::Tween.FilterDialog FilterDialog
            {
                [DebuggerStepThrough]
                get { return GetForm(ref FilterDialog_instance, ref FilterDialog_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref FilterDialog_instance, value); }
            }

            global::Tween.TabsDialog TabsDialog_instance;
            bool TabsDialog_isCreating;

            public global::Tween.TabsDialog TabsDialog
            {
                [DebuggerStepThrough]
                get { return GetForm(ref TabsDialog_instance, ref TabsDialog_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref TabsDialog_instance, value); }
            }

            global::Tween.TweenAboutBox TweenAboutBox_instance;
            bool TweenAboutBox_isCreating;

            public global::Tween.TweenAboutBox TweenAboutBox
            {
                [DebuggerStepThrough]
                get { return GetForm(ref TweenAboutBox_instance, ref TweenAboutBox_isCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref TweenAboutBox_instance, value); }
            }

            [DebuggerStepThrough]
            private static T GetForm<T>(ref T instance, ref bool isCreating) where T : Form, new()
            {
                if (instance == null || instance.IsDisposed)
                {
                    if (isCreating)
                    {
                        throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
                    }
                    isCreating = true;
                    try
                    {
                        instance = new T();
                    }
                    catch (System.Reflection.TargetInvocationException ex)
                    {
                        throw new InvalidOperationException(Utils.GetResourceString("WinForms_SeeInnerException", new string[] { ex.InnerException.Message }), ex.InnerException);
                    }
                    finally
                    {
                        isCreating = false;
                    }
                }
                return instance;
            }

            [DebuggerStepThrough]
            private static void SetForm<T>(ref T instance, T value) where T : Form
            {
                if (instance != value)
                {
                    if (value == null)
                    {
                        instance.Dispose();
                        instance = null;
                    }
                    else
                    {
                        throw new ArgumentException("Property can only be set to null");
                    }
                }
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