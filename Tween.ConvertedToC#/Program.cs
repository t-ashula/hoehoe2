// Hoehoe - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
//
// All rights reserved.
// This file is part of Hoehoe.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Hoehoe.Properties;

namespace Hoehoe
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);//UseCompatibleTextRendering);
            Application.ThreadException += MyApplication_UnhandledException;
            Application.ApplicationExit += (_, __) => { RelaseMutex(); Settings.Default.Save(); };
            if (!MyApplication_Startup())
            {
                return;
            }
            //this.ShutdownStyle = global::Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
            Application.Run(new TweenMain());
        }
        private static Mutex mt;

        private static void RelaseMutex()
        {
            try
            {
                if (mt != null)
                {
                    mt.ReleaseMutex();
                    mt.Close();
                    mt = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private static bool MyApplication_Startup()
        {
            CheckSettingFilePath();
            //InitCulture(Application.CurrentCulture.Name);

            string pt = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/" + Application.ProductName;
            mt = new Mutex(false, pt);
            try
            {
                if (!mt.WaitOne(0, false))
                {
                    // 実行中の同じアプリケーションのウィンドウ・ハンドルの取得
                    Process prevProcess = Win32Api.GetPreviousProcess();
                    if (prevProcess != null && prevProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        // 起動中のアプリケーションを最前面に表示
                        Win32Api.WakeupWindow(prevProcess.MainWindowHandle);
                    }
                    else
                    {
                        if (prevProcess != null)
                        {
                            //プロセス特定は出来たが、ウィンドウハンドルが取得できなかった（アイコン化されている）
                            //タスクトレイアイコンのクリックをエミュレート
                            //注：アイコン特定はTooltipの文字列で行うため、多重起動時は先に見つけた物がアクティブになる
                            bool rslt = Win32Api.ClickTasktrayIcon("Hoehoe");
                            if (!rslt)
                            {
                                // 警告を表示（見つからない、またはその他の原因で失敗）
                                MessageBox.Show(Resources.StartupText1, Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            // 警告を表示（プロセス見つからない場合）
                            MessageBox.Show(Resources.StartupText1, Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    //起動キャンセル
                    RelaseMutex();
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("application startup:" + ex);
                return false;
            }
            return true;
        }

        private static void MyApplication_UnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            System.Diagnostics.Debug.Write("UnhandledException:" + e);
            //GDI+のエラー原因を特定したい
            if (e.Exception.Message != "A generic error occurred in GDI+." && e.Exception.Message != "GDI+ で汎用エラーが発生しました。")
            {
                if (MyCommon.ExceptionOut(e.Exception))
                {
                    Application.Exit();
                }
            }
        }

        private static bool IsEqualCurrentCulture(string CultureName)
        {
            return Thread.CurrentThread.CurrentUICulture.Name.StartsWith(CultureName);
        }

        public static string CultureCode
        {
            get
            {
                if (MyCommon.CultureStr == null)
                {
                    SettingCommon cfgCommon = SettingCommon.Load();
                    MyCommon.CultureStr = cfgCommon.Language;
                    if (MyCommon.CultureStr == "OS")
                    {
                        if (!IsEqualCurrentCulture("ja") && !IsEqualCurrentCulture("en") && !IsEqualCurrentCulture("zh-CN"))
                        {
                            MyCommon.CultureStr = "en";
                        }
                    }
                }
                return MyCommon.CultureStr;
            }
        }

        private static void CheckSettingFilePath()
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "roaming")))
            {
                MyCommon.SettingPath = MySpecialPath.UserAppDataPath();
            }
            else
            {
                MyCommon.SettingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

#if not
        public static void InitCulture(string code)
        {
            try
            {
                ChangeUICulture(code);
            }
            catch (Exception)
            {
            }
        }

        public static void InitCulture()
        {
            try
            {
                if (this.CultureCode != "OS")
                {
                    ChangeUICulture(this.CultureCode);
                }
            }
            catch (Exception)
            {
            }
        }
        private static void ChangeUICulture(string code)
        {
            //
        }
#endif
    }
}
