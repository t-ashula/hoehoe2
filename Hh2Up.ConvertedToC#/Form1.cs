using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace TweenUp
{
    public partial class Form1
    {
        private string TWEENEXEPATH = Application.StartupPath;

        private string SOURCEPATH = Application.StartupPath;

        private void Form1_Load(System.Object sender, System.EventArgs e)
        {
            // 文字列リソースから設定
            this.Text = TweenUp.My.Resources.FormTitle;
            Label1.Text = TweenUp.My.Resources.TweenUpdating;
            Label2.Text = TweenUp.My.Resources.PleaseWait;

            if (TweenUp.My.MyProject.Application.CommandLineArgs.Count > 0)
                TWEENEXEPATH = TweenUp.My.MyProject.Application.CommandLineArgs[0];

            if (TweenUp.My.MyProject.Application.CommandLineArgs.Count == 1 && IsRequiredUAC())
            {
                this.Visible = false;
                Process p = new Process();
                p.StartInfo.FileName = Path.Combine(Application.StartupPath, TweenUp.My.MyProject.Application.Info.AssemblyName + ".exe");
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WorkingDirectory = Application.StartupPath;
                p.StartInfo.Arguments = "\"" + TWEENEXEPATH + "\" up";
                p.StartInfo.Verb = "RunAs";
                try
                {
                    p.Start();
                    p.WaitForExit();
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    Application.Exit();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    p.Close();
                }

                Process.Start(Path.Combine(TWEENEXEPATH, TweenUp.My.Resources.FilenameTweenExe));
                Application.Exit();
                return;
            }

            // exe自身からフォームのアイコンを取得
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

#if NONDEFINED
			if (Environment.GetCommandLineArgs().Length != 1 && Directory.Exists(Environment.GetCommandLineArgs()[1])) {
				TWEENEXEPATH = Environment.GetCommandLineArgs()[1];
				SOURCEPATH = Path.GetTempPath();
			}
#endif
        }

        private bool IsRequiredUAC()
        {
            OperatingSystem os = System.Environment.OSVersion;
            if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
                return true;
            return false;
        }

        private void Form1_Shown(System.Object sender, System.EventArgs e)
        {
            this.BackgroundWorker1.WorkerReportsProgress = true;
            this.BackgroundWorker1.RunWorkerAsync();
        }

        private void BackupConfigurationFile()
        {
            //Dim SrcFile As String
            //Dim DstFile As String

            //Try
            //    SrcFile = Path.Combine(TWEENEXEPATH, "Tween.exe.Config")
            //    DstFile = Path.Combine(TWEENEXEPATH, "Tween.exe.Config.Backup" + DateTime.Now.ToString("yyyyMMddHHmmss"))

            //    File.Copy(SrcFile, DstFile, True)
            //Catch ex As Exception

            //End Try

            //Try
            //    SrcFile = Path.Combine(TWEENEXEPATH, "TweenConf.xml")
            //    DstFile = Path.Combine(TWEENEXEPATH, "TweenConf.xml.Backup" + DateTime.Now.ToString("yyyyMMddHHmmss"))

            //    File.Copy(SrcFile, DstFile, True)
            //Catch ex As Exception

            //End Try

            try
            {
                string bkDir2 = Path.Combine(SOURCEPATH, "TweenBackup2nd");
                if (Directory.Exists(bkDir2))
                {
                    Directory.Delete(bkDir2, true);
                }
                string bkDir = Path.Combine(SOURCEPATH, "TweenBackup1st");
                if (Directory.Exists(bkDir))
                {
                    Directory.Move(bkDir, bkDir2);
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                string bkDir = Path.Combine(SOURCEPATH, "TweenBackup1st");
                if (!Directory.Exists(bkDir))
                {
                    Directory.CreateDirectory(bkDir);
                }
                foreach (FileInfo file in (new DirectoryInfo(SOURCEPATH + Path.DirectorySeparatorChar)).GetFiles("*.xml"))
                {
                    file.CopyTo(Path.Combine(bkDir, file.Name), true);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DeleteOldFiles()
        {
            try
            {
                string bkDir = Path.Combine(SOURCEPATH, "TweenOldFiles");
                if (!Directory.Exists(bkDir))
                {
                    Directory.CreateDirectory(bkDir);
                }

                //ログファイルの削除
                DirectoryInfo cDir = new DirectoryInfo(SOURCEPATH + Path.DirectorySeparatorChar);
                foreach (FileInfo file in cDir.GetFiles("Tween*.log"))
                {
                    file.MoveTo(Path.Combine(bkDir, file.Name));
                }

                //旧設定ファイルの削除
                foreach (FileInfo file in cDir.GetFiles("Tween.exe.config.Backup*"))
                {
                    file.MoveTo(Path.Combine(bkDir, file.Name));
                }

                //旧設定XMLファイルの削除
                foreach (FileInfo file in cDir.GetFiles("TweenConf.xml.Backup*"))
                {
                    file.MoveTo(Path.Combine(bkDir, file.Name));
                }

                //旧設定XMLファイルの削除
                foreach (FileInfo file in cDir.GetFiles("Setting*.xml.Backup*"))
                {
                    file.MoveTo(Path.Combine(bkDir, file.Name));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void BackgroundWorker1_DoWork(System.Object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            const int WaitTime = 5000;

            // スリープ時間
            List<string> cultures = new List<string>();

            //リソースを配置するフォルダ名（カルチャ名）

            cultures.AddRange(new string[] { "en" });
            string curCul = "";
            if (!Thread.CurrentThread.CurrentUICulture.IsNeutralCulture)
            {
                int idx = Thread.CurrentThread.CurrentUICulture.Name.LastIndexOf('-');
                if (idx > -1)
                {
                    curCul = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, idx);
                }
                else
                {
                    curCul = Thread.CurrentThread.CurrentUICulture.Name;
                }
            }
            else
            {
                curCul = Thread.CurrentThread.CurrentUICulture.Name;
            }
            if (string.IsNullOrEmpty(curCul) && curCul != "en")
                cultures.Add(curCul);

            BackgroundWorker1.ReportProgress(0, userState: TweenUp.My.Resources.ProgressWaitForTweenExit);
            System.Threading.Thread.Sleep(WaitTime);

            // スリープ

            ArrayList ImagePath = new ArrayList();

            //TweenUp.exeと同じフォルダのTween.exeは無条件に対象
            ImagePath.Add(Path.Combine(TWEENEXEPATH, TweenUp.My.Resources.FilenameTweenExe));

            // Tween という名前のプロセスを取得
            Process[] ps = Process.GetProcessesByName(TweenUp.My.Resources.WaitProcessName);
            Process p = null;

            //       Console.WriteLine("取得開始")

            // コレクションをスキャン
            foreach (Process p_loopVariable in ps)
            {
                p = p_loopVariable;
                if (ImagePath.Contains(p.MainModule.FileName) == true)
                {
                    //' 終了指示
                    //If Not p.CloseMainWindow() Then
                    //    'アイコン化、ダイアログ表示など、終了を受け付けられる状態ではない。
                    //    Throw New ApplicationException(My.Resources.TimeOutException)
                    //End If
                    if (!p.WaitForExit(60000))
                    {
                        // 強制終了
                        //p.Kill()
                        //If Not p.WaitForExit(10000) Then
                        // だいたい30秒ぐらい（適当）たってもだめなら例外を発生させる
                        throw new ApplicationException(TweenUp.My.Resources.TimeOutException);

                        //End If
                    }
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            //BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressProcessKill)
            //Thread.Sleep(WaitTime) ' スリープ

            // 「Tweenの終了を検出しました」
            BackgroundWorker1.ReportProgress(0, userState: TweenUp.My.Resources.ProgressDetectTweenExit);

            Thread.Sleep(WaitTime);

            // スリープ

            // 設定ファイルのバックアップ
            BackgroundWorker1.ReportProgress(0, userState: TweenUp.My.Resources.ProgressBackup);
            BackupConfigurationFile();
            DeleteOldFiles();
            Thread.Sleep(WaitTime);

            BackgroundWorker1.ReportProgress(0, userState: TweenUp.My.Resources.ProgressCopying);
            foreach (object DstFile_loopVariable in ImagePath)
            {
                var DstFile = DstFile_loopVariable;

                //本体
                string SrcFile = Path.Combine(SOURCEPATH, TweenUp.My.Resources.FilenameNew);
                if (System.IO.File.Exists(SrcFile))
                {
                    // ImagePathに格納されているファイルにTweenNew.exeを上書き
                    File.Copy(SrcFile, DstFile.ToString(), true);

                    // TweenNew.exeを削除
                    File.Delete(Path.Combine(SOURCEPATH, TweenUp.My.Resources.FilenameNew));
                }

                //リソース
                //Dim resDirs As String() = Directory.GetDirectories(SOURCEPATH, "*", SearchOption.TopDirectoryOnly)
                //ディレクトリ探索
                foreach (string spath in Directory.GetDirectories(SOURCEPATH, "*", SearchOption.TopDirectoryOnly))
                {
                    string cul = spath.Substring(spath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    string SrcFileRes = Path.Combine(spath, TweenUp.My.Resources.FilenameResourceNew);
                    string DstFileRes = Path.Combine(Path.Combine(TWEENEXEPATH, cul), TweenUp.My.Resources.FilenameResourceDll);

                    if (System.IO.File.Exists(SrcFileRes))
                    {
                        // リソースフォルダが更新先に存在しない場合は作成する
                        if (!Directory.Exists(Path.Combine(TWEENEXEPATH, cul)))
                        {
                            Directory.CreateDirectory(Path.Combine(TWEENEXEPATH, cul));
                        }

                        // リソースファイルの上書き
                        File.Copy(SrcFileRes, DstFileRes, true);

                        // リソースファイル削除
                        File.Delete(SrcFileRes);
                    }
                }

                //シリアライザDLL
                string SrcFileDll = Path.Combine(SOURCEPATH, TweenUp.My.Resources.FilenameDllNew);
                string DstFileDll = Path.Combine(TWEENEXEPATH, TweenUp.My.Resources.FilenameDll);
                if (System.IO.File.Exists(SrcFileDll))
                {
                    File.Copy(SrcFileDll, DstFileDll, true);
                    File.Delete(SrcFileDll);
                }
            }

            Thread.Sleep(WaitTime);

            // スリープ

            // ネイティブイメージにコンパイル
            //Call GenerateNativeImage()

            // 「新しいTweenを起動しています」
            BackgroundWorker1.ReportProgress(0, userState: TweenUp.My.Resources.ProgressExecuteTween);

            if (TweenUp.My.MyProject.Application.CommandLineArgs.Count == 1)
            {
                Process.Start(Path.Combine(TWEENEXEPATH, TweenUp.My.Resources.FilenameTweenExe));
            }

            //Process.Start(Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe))
        }

        private void BackgroundWorker1_RunWorkerCompleted(System.Object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if ((e.Error != null))
            {
                // 例外が発生していた場合はthrowする
                throw e.Error;
            }

            this.Close();
        }

        private void BackgroundWorker1_ProgressChanged(System.Object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            // 進行状況を書き直す　同時にVisibleにする
            LabelProgress.Text = e.UserState.ToString();

            // ラベルコントロールをセンタリング
            LabelProgress.Left = (this.ClientSize.Width - LabelProgress.Size.Width) / 2;

            LabelProgress.Refresh();
            LabelProgress.Visible = true;
        }

#if NONDEFINED

		private void GenerateNativeImage()
		{
			// Tween.exeをプリコンパイル
			try {
				var psi = new ProcessStartInfo();

				psi.Arguments = "/nologo /silent " + Strings.Chr(34) + Path.Combine(TWEENEXEPATH, TweenUp.My.Resources.FilenameTweenExe) + Strings.Chr(34);
				psi.FileName = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");
				psi.WindowStyle = ProcessWindowStyle.Hidden;
				Process.Start(psi).WaitForExit();
			} catch {
			}
		}

		public Form1()
		{
			Shown += Form1_Shown;
			Load += Form1_Load;
			InitializeComponent();
		}

#endif
    }
}