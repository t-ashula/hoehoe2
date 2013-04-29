using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace TweenUp.My
{
    /// <summary>
    /// The my project.
    /// </summary>
    internal sealed class MyProject
    {
        [ThreadStatic]
        private static MyApplication _application;

        /// <summary>
        /// Gets the application.
        /// </summary>
        public static MyApplication Application
        {
            [DebuggerStepThrough]
            get { return _application ?? (_application = new MyApplication()); }
        }

        [ThreadStatic]
        private static MyForms _forms;

        /// <summary>
        /// Gets the forms.
        /// </summary>
        public static MyForms Forms
        {
            [DebuggerStepThrough]
            get { return _forms ?? (_forms = new MyForms()); }
        }

        /// <summary>
        /// The my forms.
        /// </summary>
        internal sealed class MyForms
        {
            private Form1 _form1Instance;
            private bool _form1IsCreating;

            /// <summary>
            /// Gets or sets the form 1.
            /// </summary>
            public Form1 Form1
            {
                [DebuggerStepThrough]
                get { return GetForm(ref _form1Instance, ref _form1IsCreating); }
                [DebuggerStepThrough]
                set { SetForm(ref _form1Instance, value); }
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
}