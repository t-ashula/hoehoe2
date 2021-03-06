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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Hoehoe
{
    public static class Win32Api
    {
        #region "先行起動プロセスをアクティブにする"

        // 外部プロセスのウィンドウを起動する
        public static void WakeupWindow(IntPtr hWnd)
        {
            // メイン・ウィンドウが最小化されていれば元に戻す
            if (IsIconic(hWnd))
            {
                ShowWindowAsync(hWnd, SW_RESTORE);
            }

            // メイン・ウィンドウを最前面に表示する
            SetForegroundWindow(hWnd);
        }

        // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // ウィンドウの表示状態を設定
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        // 指定されたウィンドウが最小化（ アイコン化）されているかどうかを調べる
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        // ShowWindowAsync関数のパラメータに渡す定義値
        // 画面を元の大きさに戻す
        private const int SW_RESTORE = 9;

        // 実行中の同じアプリケーションのプロセスを取得する
        public static Process GetPreviousProcess()
        {
            var curProcess = Process.GetCurrentProcess();
            return Process.GetProcessesByName(curProcess.ProcessName)
                .Where(process => process.Id != curProcess.Id)
                .FirstOrDefault(process => process.MainModule.FileName.ToUpper() == curProcess.MainModule.FileName.ToUpper());
        }

        #endregion

        #region "タスクトレイアイコンのクリック"

        // 指定されたクラス名およびウィンドウ名と一致するトップレベルウィンドウのハンドルを取得します
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // 指定された文字列と一致するクラス名とウィンドウ名文字列を持つウィンドウのハンドルを返します
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        // 指定されたウィンドウへ、指定されたメッセージを送信します
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        // SendMessageで送信するメッセージ
        private enum SM_MESSAGE
        {
            /// <summary>
            /// ユーザー定義メッセージ
            /// </summary>
            WM_USER = 0x400,

            /// <summary>
            /// ツールバーのボタン取得
            /// </summary>
            TB_GETBUTTON = WM_USER + 23,

            /// <summary>
            /// ツールバーのボタン（アイコン）数取得
            /// </summary>
            TB_BUTTONCOUNT = WM_USER + 24,

            /// <summary>
            /// ツールバーのボタン詳細情報取得
            /// </summary>
            TB_GETBUTTONINFO = WM_USER + 65
        }

        // ツールバーボタン構造体
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TBBUTTON
        {
            public int iBitmap;
            public IntPtr idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public int dwData;
            public int iString;
        }

        // ツールバーボタン詳細情報構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct TBBUTTONINFO
        {
            public int cbSize;
            public int dwMask;
            public int idCommand;
            public int iImage;
            public byte fsState;
            public byte fsStyle;
            public short cx;
            public IntPtr lParam;
            public IntPtr pszText;
            public int cchText;
        }

        // TBBUTTONINFOのlParamでポイントされるアイコン情報（PostMessageで使用）
        [StructLayout(LayoutKind.Sequential)]
        private struct TRAYNOTIFY
        {
            public IntPtr hWnd;
            public uint uID;
            public uint uCallbackMessage;
            public uint dwDummy1;
            public uint dwDummy2;
            public IntPtr hIcon;
        }

        // TBBUTTONINFOに指定するマスク情報
        [Flags]
        private enum ToolbarButtonMask
        {
            TBIF_COMMAND = 0x20,
            TBIF_LPARAM = 0x10,
            TBIF_TEXT = 0x2
        }

        // 指定されたウィンドウを作成したスレッドの ID を取得します
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);

        // 指定したプロセスIDに対するプロセスハンドルを取得します
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)]bool bInheritHandle, int dwProcessId);

        // OpenProcessで指定するアクセス権
        [Flags]
        private enum ProcessAccess
        {
            /// <summary>Specifies all possible access flags for the process object.</summary>
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation | Terminate | VMOperation | VMRead | VMWrite | Synchronize,

            /// <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
            CreateThread = 0x2,

            /// <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
            DuplicateHandle = 0x40,

            /// <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
            QueryInformation = 0x400,

            /// <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
            SetInformation = 0x200,

            /// <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
            Terminate = 0x1,

            /// <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
            VMOperation = 0x8,

            /// <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
            VMRead = 0x10,

            /// <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
            VMWrite = 0x20,

            /// <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
            Synchronize = 0x100000
        }

        // 指定したプロセスの仮想アドレス空間にメモリ領域を確保
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationTypes flAllocationType, MemoryProtectionTypes flProtect);

        // アロケート種類
        [Flags]
        private enum AllocationTypes : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        // アロケートしたメモリに対する保護レベル
        [Flags]
        private enum MemoryProtectionTypes : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x1,
            ReadOnly = 0x2,
            ReadWrite = 0x4,
            WriteCopy = 0x8,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        // オープンしているカーネルオブジェクトのハンドルをクローズします
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        // 指定されたプロセスの仮想アドレス空間内のメモリ領域を解放またはコミット解除します
        [DllImport("kernel32.dll")]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);

        // メモリ解放種別
        [Flags]
        private enum MemoryFreeTypes
        {
            Release = 0x8000
        }

        // 指定したプロセスのメモリ領域にデータをコピーする
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref TBBUTTONINFO lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        // 指定したプロセスのメモリ領域のデータを呼び出し側プロセスのバッファにコピーする
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int iSize, ref int lpNumberOfBytesRead);

        // メッセージをウィンドウのメッセージ キューに置き、対応するウィンドウがメッセージを処理するのを待たずに戻ります
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        // PostMessageで送信するメッセージ
        private enum PM_MESSAGE : uint
        {
            // 左マウスボタン押し下げ
            WM_LBUTTONDOWN = 0x201,

            // 左マウスボタン離し
            WM_LBUTTONUP = 0x202
        }

        /// <summary>
        /// タスクトレイアイコンのクリック処理
        /// </summary>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public static bool ClickTasktrayIcon(string tooltip)
        {
            const string TRAY_WINDOW = "Shell_TrayWnd";
            const string TRAY_NOTIFYWINDOW = "TrayNotifyWnd";
            const string TRAY_PAGER = "SysPager";
            const string TOOLBAR_CONTROL = "ToolbarWindow32";

            // タスクバーのハンドル取得
            IntPtr taskbarWin = FindWindow(TRAY_WINDOW, null);
            if (taskbarWin.Equals(IntPtr.Zero))
            {
                return false;
            }

            // 通知領域のハンドル取得
            IntPtr trayWin = FindWindowEx(taskbarWin, IntPtr.Zero, TRAY_NOTIFYWINDOW, null);
            if (trayWin.Equals(IntPtr.Zero))
            {
                return false;
            }

            // SysPagerの有無確認。（XP/2000はSysPagerあり）
            IntPtr tempWin = FindWindowEx(trayWin, IntPtr.Zero, TRAY_PAGER, null);
            if (tempWin.Equals(IntPtr.Zero))
            {
                tempWin = trayWin;
            }

            // タスクトレイがツールバーで出来ているか確認　→　ツールバーでなければ終了
            IntPtr toolWin = FindWindowEx(tempWin, IntPtr.Zero, TOOLBAR_CONTROL, null);
            if (toolWin.Equals(IntPtr.Zero))
            {
                return false;
            }

            // タスクトレイのプロセス（Explorer）を取得し、外部から参照するために開く
            int expPid = 0;
            GetWindowThreadProcessId(toolWin, ref expPid);
            IntPtr hProc = OpenProcess(ProcessAccess.VMOperation | ProcessAccess.VMRead | ProcessAccess.VMWrite, false, expPid);
            if (hProc.Equals(IntPtr.Zero))
            {
                return false;
            }

            // プロセスを閉じるためにTry-Finally
            try
            {
                var tbButtonLocal = new TBBUTTON(); // 本プロセス内のタスクバーボタン情報作成（サイズ特定でのみ使用）

                // Explorer内のタスクバーボタン格納メモリ確保
                IntPtr ptbSysButton = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonLocal), AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                if (ptbSysButton.Equals(IntPtr.Zero))
                {
                    // メモリ確保失敗
                    return false;
                }

                try
                {
                    var tbButtonInfoLocal = new TBBUTTONINFO(); // 本プロセス内ツールバーボタン詳細情報作成

                    // Explorer内のタスクバーボタン詳細情報格納メモリ確保
                    IntPtr ptbSysInfo = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonInfoLocal), AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                    if (ptbSysInfo.Equals(IntPtr.Zero))
                    {
                        // メモリ確保失敗
                        return false;
                    }

                    try
                    {
                        const int titleSize = 256;    // Tooltip文字列長

                        // 共有メモリにTooltip読込メモリ確保
                        IntPtr pszTitle = Marshal.AllocCoTaskMem(titleSize);
                        if (pszTitle.Equals(IntPtr.Zero))
                        {
                            // メモリ確保失敗
                            return false;
                        }

                        try
                        {
                            // Explorer内にTooltip読込メモリ確保
                            IntPtr pszSysTitle = VirtualAllocEx(hProc, IntPtr.Zero, titleSize, AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                            if (pszSysTitle.Equals(IntPtr.Zero))
                            {
                                // メモリ確保失敗
                                return false;
                            }

                            try
                            {
                                // 通知領域ボタン数取得
                                int iCount = SendMessage(toolWin, (int)SM_MESSAGE.TB_BUTTONCOUNT, new IntPtr(0), new IntPtr(0));

                                // 左から順に情報取得
                                for (int i = 0; i < iCount; i++)
                                {
                                    int dwBytes = 0;                                              // 読み書きバイト数
                                    TBBUTTON tbButtonLocal2;                  // ボタン情報
                                    TBBUTTONINFO tbButtonInfoLocal2;      // ボタン詳細情報

                                    // 共有メモリにボタン情報読込メモリ確保
                                    IntPtr ptrLocal = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonLocal));
                                    if (ptrLocal.Equals(IntPtr.Zero))
                                    {
                                        return false;
                                    }

                                    // メモリ確保失敗
                                    try
                                    {
                                        // 共有メモリ初期化
                                        Marshal.StructureToPtr(tbButtonLocal, ptrLocal, true);

                                        // ボタン情報取得（idCommandを取得するため）
                                        SendMessage(toolWin, (int)SM_MESSAGE.TB_GETBUTTON, new IntPtr(i), ptbSysButton);

                                        // Explorer内のメモリを共有メモリに読み込み
                                        ReadProcessMemory(hProc, ptbSysButton, ptrLocal, Marshal.SizeOf(tbButtonLocal), ref dwBytes);

                                        // 共有メモリの内容を構造体に変換
                                        tbButtonLocal2 = (TBBUTTON)Marshal.PtrToStructure(ptrLocal, typeof(TBBUTTON));
                                    }
                                    finally
                                    {
                                        // 共有メモリ解放
                                        Marshal.FreeCoTaskMem(ptrLocal);
                                    }

                                    // ボタン詳細情報を取得するためのマスク等を設定
                                    tbButtonInfoLocal.cbSize = Marshal.SizeOf(tbButtonInfoLocal);
                                    tbButtonInfoLocal.dwMask = (int)(ToolbarButtonMask.TBIF_COMMAND | ToolbarButtonMask.TBIF_LPARAM | ToolbarButtonMask.TBIF_TEXT);

                                    // Tooltip書き込み先領域
                                    tbButtonInfoLocal.pszText = pszSysTitle;
                                    tbButtonInfoLocal.cchText = titleSize;

                                    // マスク設定等をExplorerのメモリへ書き込み
                                    WriteProcessMemory(hProc, ptbSysInfo, ref tbButtonInfoLocal, Marshal.SizeOf(tbButtonInfoLocal), out dwBytes);

                                    // ボタン詳細情報取得
                                    SendMessage(toolWin, (int)SM_MESSAGE.TB_GETBUTTONINFO, tbButtonLocal2.idCommand, ptbSysInfo);

                                    // 共有メモリにボタン詳細情報を読み込む領域確保
                                    IntPtr ptrInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonInfoLocal));
                                    if (ptrInfo.Equals(IntPtr.Zero))
                                    {
                                        // 共有メモリ確保失敗
                                        return false;
                                    }

                                    try
                                    {
                                        Marshal.StructureToPtr(tbButtonInfoLocal, ptrInfo, true); // 共有メモリ初期化

                                        // Explorer内のメモリを共有メモリに読み込み
                                        ReadProcessMemory(hProc, ptbSysInfo, ptrInfo, Marshal.SizeOf(tbButtonInfoLocal), ref dwBytes);

                                        // 共有メモリの内容を構造体に変換
                                        tbButtonInfoLocal2 = (TBBUTTONINFO)Marshal.PtrToStructure(ptrInfo, typeof(TBBUTTONINFO));
                                    }
                                    finally
                                    {
                                        Marshal.FreeCoTaskMem(ptrInfo);
                                    }

                                    // Tooltipの内容をExplorer内のメモリから共有メモリへ読込
                                    ReadProcessMemory(hProc, pszSysTitle, pszTitle, titleSize, ref dwBytes);

                                    // ローカル変数へ変換
                                    string title = Marshal.PtrToStringAnsi(pszTitle, titleSize);  // Tooltip文字列

                                    // Tooltipが指定文字列を含んでいればクリック
                                    if (title.Contains(tooltip))
                                    {
                                        // PostMessageでクリックを送るために、ボタン詳細情報のlParamでポイントされているTRAYNOTIFY情報が必要
                                        var tNotify = new TRAYNOTIFY();
                                        TRAYNOTIFY tNotify2;

                                        // 共有メモリ確保
                                        IntPtr ptNotify = Marshal.AllocCoTaskMem(Marshal.SizeOf(tNotify));
                                        if (ptNotify.Equals(IntPtr.Zero))
                                        {
                                            // メモリ確保失敗
                                            return false;
                                        }

                                        try
                                        {
                                            Marshal.StructureToPtr(tNotify, ptNotify, true);   // 初期化

                                            // lParamのメモリを読込
                                            ReadProcessMemory(hProc, tbButtonInfoLocal2.lParam, ptNotify, Marshal.SizeOf(tNotify), ref dwBytes);

                                            // 構造体へ変換
                                            tNotify2 = (TRAYNOTIFY)Marshal.PtrToStructure(ptNotify, typeof(TRAYNOTIFY));
                                        }
                                        finally
                                        {
                                            Marshal.FreeCoTaskMem(ptNotify);
                                        }

                                        // クリックするためには通知領域がアクティブでなければならない
                                        SetForegroundWindow(tNotify2.hWnd);

                                        // 左クリック
                                        PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, (uint)PM_MESSAGE.WM_LBUTTONDOWN);
                                        PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, (uint)PM_MESSAGE.WM_LBUTTONUP);
                                        return true;
                                    }
                                }

                                return false; // 該当なし
                            }
                            finally
                            {
                                VirtualFreeEx(hProc, pszSysTitle, titleSize, (int)MemoryFreeTypes.Release);
                            }
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(pszTitle);
                        }
                    }
                    finally
                    {
                        VirtualFreeEx(hProc, ptbSysInfo, Marshal.SizeOf(tbButtonInfoLocal), (int)MemoryFreeTypes.Release);
                    }
                }
                finally
                {
                    VirtualFreeEx(hProc, ptbSysButton, Marshal.SizeOf(tbButtonLocal), (int)MemoryFreeTypes.Release);
                }
            }
            finally
            {
                CloseHandle(hProc);    // Explorerのプロセス閉じる
            }
        }

        #endregion

        // 画面をブリンクするためのWin32API。起動時に10ページ読み取りごとに継続確認メッセージを表示する際の通知強調用
        [DllImport("user32.dll")]
        public static extern int FlashWindow(int hwnd, int bInvert);

        #region "画面ブリンク用"

        public static bool FlashMyWindow(IntPtr hwnd, FlashSpecification flashType, int flashCount)
        {
            var fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hwnd;
            fInfo.dwFlags = (int)FlashSpecification.FlashAll;
            fInfo.uCount = flashCount;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        public enum FlashSpecification
        {
            FlashStop = FLASHW_STOP,
            FlashCaption = FLASHW_CAPTION,
            FlashTray = FLASHW_TRAY,
            FlashAll = FLASHW_ALL,
            FlashTimer = FLASHW_TIMER,
            FlashTimerNoForeground = FLASHW_TIMERNOFG
        }

        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/723flashwindow/flashwindow.html
        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FLASHWINFO fwinfo);

        private struct FLASHWINFO
        {
            public int cbSize;    // FLASHWINFO構造体のサイズ
            public IntPtr hwnd;   // 点滅対象のウィンドウ・ハンドル
            public int dwFlags;   // 以下の「FLASHW_XXX」のいずれか
            public int uCount;    // 点滅する回数
            public int dwTimeout; // 点滅する間隔（ミリ秒単位）
        }

        private const int FLASHW_STOP = 0;        // 点滅を止める
        private const int FLASHW_CAPTION = 0x1;   // タイトルバーを点滅させる
        private const int FLASHW_TRAY = 0x2;      // タスクバー・ボタンを点滅させる
        private const int FLASHW_ALL = 0x3;       // タスクバー・ボタンとタイトルバーを点滅させる
        private const int FLASHW_TIMER = 0x4;     // FLASHW_STOPが指定されるまでずっと点滅させる
        private const int FLASHW_TIMERNOFG = 0xc; // ウィンドウが最前面に来るまでずっと点滅させる

        #endregion

        [DllImport("user32.dll")]
        public static extern bool ValidateRect(IntPtr hwnd, IntPtr rect);

        #region "スクリーンセーバー起動中か判定"

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int intAction, int intParam, ref bool bParam, int intWinIniFlag);

        // スクリーンセーバーが起動中かを取得する定数
        private const int SPI_GETSCREENSAVERRUNNING = 0x61;

        public static bool IsScreenSaverRunning()
        {
            bool isRunning = false;
            int ret = SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);
            if (ret != 0)
            {
                return isRunning;
            }

            return false;
        }

        #endregion

        #region "グローバルフック"

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("kernel32", EntryPoint = "GlobalAddAtomA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GlobalAddAtom(string lpString);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GlobalDeleteAtom(int nAtom);

        // register a global hot key use the GlobalAddAtom API to get a unique ID (as suggested by MSDN docs)
        private static int registeredGlobalHotKeyCount;

        public static int RegisterGlobalHotKey(int hotkeyValue, int modifiers, Form targetForm)
        {
            int hotkeyID = 0;
            try
            {
                registeredGlobalHotKeyCount += 1;
                string atomName = string.Format("{0}{1}{2}", Thread.CurrentThread.ManagedThreadId.ToString("X8"), targetForm.Name, registeredGlobalHotKeyCount);
                hotkeyID = GlobalAddAtom(atomName);
                if (hotkeyID == 0)
                {
                    throw new Exception(string.Format("Unable to generate unique hotkey ID. Error code: {0}", Marshal.GetLastWin32Error()));
                }

                // register the hotkey, throw if any error
                if (RegisterHotKey(targetForm.Handle, hotkeyID, modifiers, hotkeyValue) == 0)
                {
                    throw new Exception(string.Format("Unable to register hotkey. Error code: {0}", Marshal.GetLastWin32Error()));
                }

                return hotkeyID;
            }
            catch (Exception)
            {
                // clean up if hotkey registration failed
                UnregisterGlobalHotKey(hotkeyID, targetForm);
                return 0;
            }
        }

        // unregister a global hotkey
        public static void UnregisterGlobalHotKey(int hotkeyID, Form targetForm)
        {
            if (hotkeyID != 0)
            {
                UnregisterHotKey(targetForm.Handle, hotkeyID);
                GlobalDeleteAtom(hotkeyID);

                // hotkeyID = 0;
            }
        }

        #endregion

        #region "プロセスのProxy設定"

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private struct INTERNET_PROXY_INFO
        {
            public int DwAccessType;
            public IntPtr Proxy;
            public IntPtr ProxyBypass;
        }

        private static void RefreshProxySettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;

            // const int INTERNET_OPEN_TYPE_PRECONFIG = 0;  // IE setting
            const int INTERNET_OPEN_TYPE_DIRECT = 1; // Direct
            const int INTERNET_OPEN_TYPE_PROXY = 3;  // Custom

            INTERNET_PROXY_INFO ipi = default(INTERNET_PROXY_INFO);

            // Filling in structure
            if (!string.IsNullOrEmpty(strProxy))
            {
                ipi.DwAccessType = INTERNET_OPEN_TYPE_PROXY;
                ipi.Proxy = Marshal.StringToHGlobalAnsi(strProxy);
                ipi.ProxyBypass = Marshal.StringToHGlobalAnsi("local");
            }
            else if (strProxy == null)
            {
                // IE Default
                IWebProxy p = WebRequest.GetSystemWebProxy();
                if (p.IsBypassed(new Uri("http://www.google.com/")))
                {
                    ipi.DwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                    ipi.Proxy = IntPtr.Zero;
                    ipi.ProxyBypass = IntPtr.Zero;
                }
                else
                {
                    ipi.DwAccessType = INTERNET_OPEN_TYPE_PROXY;
                    ipi.Proxy = Marshal.StringToHGlobalAnsi(p.GetProxy(new Uri("http://www.google.com/")).Authority);
                    ipi.ProxyBypass = Marshal.StringToHGlobalAnsi("local");
                }
            }
            else
            {
                ipi.DwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                ipi.Proxy = IntPtr.Zero;
                ipi.ProxyBypass = IntPtr.Zero;
            }

            try
            {
                // Allocating memory
                IntPtr pIpi = Marshal.AllocCoTaskMem(Marshal.SizeOf(ipi));
                if (pIpi.Equals(IntPtr.Zero))
                {
                    return;
                }

                try
                {
                    // Converting structure to IntPtr
                    Marshal.StructureToPtr(ipi, pIpi, true);
                    bool ret = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, pIpi, Marshal.SizeOf(ipi));
                    if (!ret)
                    {
                        Debug.WriteLine("Proxy set failed.");
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pIpi);
                }
            }
            finally
            {
                if (ipi.Proxy != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ipi.Proxy);
                }

                if (ipi.ProxyBypass != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ipi.ProxyBypass);
                }
            }
        }

        private static void RefreshProxyAccount(string username, string password)
        {
            const int INTERNET_OPTION_PROXY_USERNAME = 43;
            const int INTERNET_OPTION_PROXY_PASSWORD = 44;

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_USERNAME, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_PASSWORD, IntPtr.Zero, 0);
            }
            else
            {
                IntPtr pUser = Marshal.StringToBSTR(username);
                IntPtr pPass = Marshal.StringToBSTR(password);
                try
                {
                    InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_USERNAME, pUser, username.Length + 1);
                    InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_PASSWORD, pPass, password.Length + 1);
                }
                finally
                {
                    Marshal.FreeBSTR(pUser);
                    Marshal.FreeBSTR(pPass);
                }
            }
        }

        public static void SetProxy(HttpConnection.ProxyType pType, string host, int port, string username, string password)
        {
            string proxy = null;
            switch (pType)
            {
                case HttpConnection.ProxyType.IE:
                    break;

                case HttpConnection.ProxyType.None:
                    proxy = string.Empty;
                    break;

                case HttpConnection.ProxyType.Specified:
                    proxy = host + (port > 0 ? string.Format(":{0}", port) : string.Empty);
                    break;
            }

            RefreshProxySettings(proxy);
            RefreshProxyAccount(username, password);
        }

        #endregion
    }
}