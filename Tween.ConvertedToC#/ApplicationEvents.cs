using System;
using System.Diagnostics;

// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
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

using System.IO;
using System.Windows.Forms;

namespace Tween.My
{
    // 次のイベントは MyApplication に対して利用できます:
    //
    // Startup: アプリケーションが開始されたとき、スタートアップ フォームが作成される前に発生します。
    // Shutdown: アプリケーション フォームがすべて閉じられた後に発生します。このイベントは、通常の終了以外の方法でアプリケーションが終了されたときには発生しません。
    // UnhandledException: ハンドルされていない例外がアプリケーションで発生したときに発生するイベントです。
    // StartupNextInstance: 単一インスタンス アプリケーションが起動され、それが既にアクティブであるときに発生します。
    // NetworkAvailabilityChanged: ネットワーク接続が接続されたとき、または切断されたときに発生します。
    internal partial class MyApplication
    {
        private static System.Threading.Mutex mt;

        private void MyApplication_Shutdown(object sender, System.EventArgs e)
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
            catch (Exception ex)
            {
            }
        }

        private void MyApplication_Startup(object sender, Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            CheckSettingFilePath();
            InitCulture();

            //string pt = Application.Info.DirectoryPath.Replace("\\", "/") + "/" + Application.Info.ProductName;
            string pt = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "/") + "/" + Application.ProductName;
            mt = new System.Threading.Mutex(false, pt);
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
                            bool rslt = Win32Api.ClickTasktrayIcon("Tween");
                            if (!rslt)
                            {
                                // 警告を表示（見つからない、またはその他の原因で失敗）
                                MessageBox.Show(Tween.My.Resources.Resources.StartupText1, Tween.My.Resources.Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            // 警告を表示（プロセス見つからない場合）
                            MessageBox.Show(Tween.My.Resources.Resources.StartupText1, Tween.My.Resources.Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    //起動キャンセル
                    e.Cancel = true;
                    try
                    {
                        mt.ReleaseMutex();
                        mt.Close();
                        mt = null;
                    }
                    catch (Exception ex)
                    {
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void MyApplication_UnhandledException(object sender, Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            //GDI+のエラー原因を特定したい
            if (e.Exception.Message != "A generic error occurred in GDI+." && e.Exception.Message != "GDI+ で汎用エラーが発生しました。")
            {
                e.ExitApplication = MyCommon.ExceptionOut(e.Exception);
            }
            else
            {
                e.ExitApplication = false;
            }
        }

        private bool IsEqualCurrentCulture(string CultureName)
        {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith(CultureName);
        }

        public string CultureCode
        {
            get
            {
                if (MyCommon.cultureStr == null)
                {
                    SettingCommon cfgCommon = SettingCommon.Load();
                    MyCommon.cultureStr = cfgCommon.Language;
                    if (MyCommon.cultureStr == "OS")
                    {
                        if (!IsEqualCurrentCulture("ja") && !IsEqualCurrentCulture("en") && !IsEqualCurrentCulture("zh-CN"))
                        {
                            MyCommon.cultureStr = "en";
                        }
                    }
                }
                return MyCommon.cultureStr;
            }
        }

        public void InitCulture(string code)
        {
            try
            {
                ChangeUICulture(code);
            }
            catch (Exception ex)
            {
            }
        }

        public void InitCulture()
        {
            try
            {
                if (this.CultureCode != "OS")
                    ChangeUICulture(this.CultureCode);
            }
            catch (Exception ex)
            {
            }
        }

        private void CheckSettingFilePath()
        {
            if (File.Exists(Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, "roaming")))
            {
                MyCommon.settingPath = MySpecialPath.UserAppDataPath();
            }
            else
            {
                MyCommon.settingPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            }
        }
    }
}