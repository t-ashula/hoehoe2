using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TweenUp
{
    /// <summary>
    /// The form 1.
    /// </summary>
    public partial class Form1
    {
        private string _tweenexepath = Application.StartupPath;

        private readonly string _sourcepath = Application.StartupPath;

        private void Form1_Load(object sender, EventArgs e)
        {
            // 文字列リソースから設定
            Text = Resources.FormTitle;
            Label1.Text = Resources.TweenUpdating;
            Label2.Text = Resources.PleaseWait;

            var args = GetCommandLineArgs();
            if (args.Count > 0)
            {
                _tweenexepath = args[0];
            }

            if (args.Count == 1 && IsUACRequired())
            {
                Visible = false;
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(Application.StartupPath, Assembly.GetExecutingAssembly().GetName().Name + ".exe"),
                        UseShellExecute = true,
                        WorkingDirectory = Application.StartupPath,
                        Arguments = "\"" + _tweenexepath + "\" up",
                        Verb = "RunAs"
                    }
                };
                try
                {
                    p.Start();
                    p.WaitForExit();
                }
                catch (Win32Exception ex)
                {
                    Debug.Write(ex);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
                finally
                {
                    p.Close();
                }

                Process.Start(Path.Combine(_tweenexepath, Resources.FilenameTweenExe));
                Application.Exit();
                return;
            }

            // exe自身からフォームのアイコンを取得
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

#if NONDEFINED
			if (Environment.GetCommandLineArgs().Length != 1 && Directory.Exists(Environment.GetCommandLineArgs()[1])) {
				TWEENEXEPATH = Environment.GetCommandLineArgs()[1];
				SOURCEPATH = Path.GetTempPath();
			}
#endif
        }

        private static bool IsUACRequired()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major >= 6;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void BackupConfigurationFile()
        {
            try
            {
                var dir2 = Path.Combine(_sourcepath, "TweenBackup2nd");
                if (Directory.Exists(dir2))
                {
                    Directory.Delete(dir2, true);
                }

                var dir1 = Path.Combine(_sourcepath, "TweenBackup1st");
                if (Directory.Exists(dir1))
                {
                    Directory.Move(dir1, dir2);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

            try
            {
                var dir = Path.Combine(_sourcepath, "TweenBackup1st");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                foreach (var file in (new DirectoryInfo(_sourcepath + Path.DirectorySeparatorChar)).GetFiles("*.xml"))
                {
                    file.CopyTo(Path.Combine(dir, file.Name), true);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private void DeleteOldFiles()
        {
            try
            {
                var backupDir = Path.Combine(_sourcepath, "TweenOldFiles");
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                // ログファイルの削除
                var currentDir = new DirectoryInfo(_sourcepath + Path.DirectorySeparatorChar);
                foreach (var file in currentDir.GetFiles("Tween*.log"))
                {
                    file.MoveTo(Path.Combine(backupDir, file.Name));
                }

                // 旧設定ファイルの削除
                foreach (var file in currentDir.GetFiles("Tween.exe.config.Backup*"))
                {
                    file.MoveTo(Path.Combine(backupDir, file.Name));
                }

                // 旧設定XMLファイルの削除
                foreach (var file in currentDir.GetFiles("TweenConf.xml.Backup*"))
                {
                    file.MoveTo(Path.Combine(backupDir, file.Name));
                }

                // 旧設定XMLファイルの削除
                foreach (var file in currentDir.GetFiles("Setting*.xml.Backup*"))
                {
                    file.MoveTo(Path.Combine(backupDir, file.Name));
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            const int WaitTime = 5000;

            // スリープ時間
            var cultures = new List<string>();

            // リソースを配置するフォルダ名（カルチャ名）
            cultures.AddRange(new[] { "en" });
            string curCul;
            if (!Thread.CurrentThread.CurrentUICulture.IsNeutralCulture)
            {
                var idx = Thread.CurrentThread.CurrentUICulture.Name.LastIndexOf('-');
                curCul = idx > -1
                    ? Thread.CurrentThread.CurrentUICulture.Name.Substring(0, idx)
                    : Thread.CurrentThread.CurrentUICulture.Name;
            }
            else
            {
                curCul = Thread.CurrentThread.CurrentUICulture.Name;
            }

            if (string.IsNullOrEmpty(curCul) && curCul != "en")
            {
                cultures.Add(curCul);
            }

            BackgroundWorker1.ReportProgress(0, Resources.ProgressWaitForTweenExit);
            Thread.Sleep(WaitTime); // スリープ

            var imagePath = new ArrayList { Path.Combine(_tweenexepath, Resources.FilenameTweenExe) };

            // TweenUp.exeと同じフォルダのTween.exeは無条件に対象

            // Tween という名前のプロセスを取得
            var ps = Process.GetProcessesByName(Resources.WaitProcessName);

            // Console.WriteLine("取得開始")

            // コレクションをスキャン
            foreach (var p in ps)
            {
                if (imagePath.Contains(p.MainModule.FileName))
                {
                    // ' 終了指示
                    // If Not p.CloseMainWindow() Then
                    // 'アイコン化、ダイアログ表示など、終了を受け付けられる状態ではない。
                    // Throw New ApplicationException(Resources.TimeOutException)
                    // End If
                    if (!p.WaitForExit(60000))
                    {
                        // 強制終了
                        // p.Kill()
                        // If Not p.WaitForExit(10000) Then
                        // だいたい30秒ぐらい（適当）たってもだめなら例外を発生させる
                        throw new ApplicationException(Resources.TimeOutException);

                        // End If
                    }

                    break;
                }
            }

            // BackgroundWorker1.ReportProgress(0, userState:=Resources.ProgressProcessKill)
            // Thread.Sleep(WaitTime) ' スリープ

            // 「Tweenの終了を検出しました」
            BackgroundWorker1.ReportProgress(0, Resources.ProgressDetectTweenExit);

            Thread.Sleep(WaitTime);

            // スリープ

            // 設定ファイルのバックアップ
            BackgroundWorker1.ReportProgress(0, Resources.ProgressBackup);
            BackupConfigurationFile();
            DeleteOldFiles();
            Thread.Sleep(WaitTime);

            BackgroundWorker1.ReportProgress(0, Resources.ProgressCopying);
            foreach (object dstFileLoopVariable in imagePath)
            {
                var dstFile = dstFileLoopVariable;

                // 本体
                var srcFile = Path.Combine(_sourcepath, Resources.FilenameNew);
                if (File.Exists(srcFile))
                {
                    // ImagePathに格納されているファイルにTweenNew.exeを上書き
                    File.Copy(srcFile, dstFile.ToString(), true);

                    // TweenNew.exeを削除
                    File.Delete(Path.Combine(_sourcepath, Resources.FilenameNew));
                }

                // リソース
                // Dim resDirs As String() = Directory.GetDirectories(SOURCEPATH, "*", SearchOption.TopDirectoryOnly)
                // ディレクトリ探索
                foreach (var spath in Directory.GetDirectories(_sourcepath, "*", SearchOption.TopDirectoryOnly))
                {
                    var cul = spath.Substring(spath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    var srcFileRes = Path.Combine(spath, Resources.FilenameResourceNew);
                    var dstFileRes = Path.Combine(_tweenexepath, cul, Resources.FilenameResourceDll);

                    if (File.Exists(srcFileRes))
                    {
                        // リソースフォルダが更新先に存在しない場合は作成する
                        if (!Directory.Exists(Path.Combine(_tweenexepath, cul)))
                        {
                            Directory.CreateDirectory(Path.Combine(_tweenexepath, cul));
                        }

                        // リソースファイルの上書き
                        File.Copy(srcFileRes, dstFileRes, true);

                        // リソースファイル削除
                        File.Delete(srcFileRes);
                    }
                }

                // シリアライザDLL
                var srcFileDll = Path.Combine(_sourcepath, Resources.FilenameDllNew);
                var dstFileDll = Path.Combine(_tweenexepath, Resources.FilenameDll);
                if (File.Exists(srcFileDll))
                {
                    File.Copy(srcFileDll, dstFileDll, true);
                    File.Delete(srcFileDll);
                }
            }

            Thread.Sleep(WaitTime);

            // スリープ

            // ネイティブイメージにコンパイル
            // Call GenerateNativeImage()

            // 「新しいTweenを起動しています」
            BackgroundWorker1.ReportProgress(0, Resources.ProgressExecuteTween);

            if (GetCommandLineArgsCount() == 1)
            {
                Process.Start(Path.Combine(_tweenexepath, Resources.FilenameTweenExe));
            }

            // Process.Start(Path.Combine(TWEENEXEPATH, Resources.FilenameTweenExe))
        }

        private static int GetCommandLineArgsCount()
        {
            return GetCommandLineArgs().Count;
        }

        private static ReadOnlyCollection<string> GetCommandLineArgs()
        {
            return new ReadOnlyCollection<string>(Environment.GetCommandLineArgs().Skip(1).ToArray());
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // 例外が発生していた場合はthrowする
                throw e.Error;
            }

            Close();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進行状況を書き直す　同時にVisibleにする
            LabelProgress.Text = e.UserState.ToString();

            // ラベルコントロールをセンタリング
            LabelProgress.Left = (ClientSize.Width - LabelProgress.Size.Width) / 2;

            LabelProgress.Refresh();
            LabelProgress.Visible = true;
        }

#if NONDEFINED

		private void GenerateNativeImage()
		{
			// Tween.exeをプリコンパイル
			try {
				var psi = new ProcessStartInfo();

				psi.Arguments = "/nologo /silent " + Strings.Chr(34) + Path.Combine(TWEENEXEPATH, TweenUp.Resources.FilenameTweenExe) + Strings.Chr(34);
				psi.FileName = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");
				psi.WindowStyle = ProcessWindowStyle.Hidden;
				Process.Start(psi).WaitForExit();
			} catch {
			}
		}
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
    }
}