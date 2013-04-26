using System.Diagnostics;
using Microsoft.VisualBasic;

namespace TweenUp.My
{
    /// <summary>
    /// 次のイベントは MyApplication に対して利用できます:
    /// Startup: アプリケーションが開始されたとき、スタートアップ フォームが作成される前に発生します。
    /// Shutdown: アプリケーション フォームがすべて閉じられた後に発生します。このイベントは、通常の終了以外の方法でアプリケーションが終了されたときには発生しません。
    /// UnhandledException: ハンドルされていない例外がアプリケーションで発生したときに発生するイベントです。
    /// StartupNextInstance: 単一インスタンス アプリケーションが起動され、それが既にアクティブであるときに発生します。
    /// NetworkAvailabilityChanged: ネットワーク接続が接続されたとき、または切断されたときに発生します。
    /// </summary>
    internal partial class MyApplication
    {
        private void MyApplication_UnhandledException(object sender, Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            MyProject.Application.Log.DefaultFileLogWriter.Location = Microsoft.VisualBasic.Logging.LogFileLocation.ExecutableDirectory;
            MyProject.Application.Log.DefaultFileLogWriter.MaxFileSize = 16384;
            MyProject.Application.Log.DefaultFileLogWriter.AutoFlush = true;
            MyProject.Application.Log.DefaultFileLogWriter.Append = false;
            MyProject.Application.Log.WriteException(e.Exception, TraceEventType.Error, DateAndTime.Now.ToString());
            Interaction.MsgBox(Resources.ExceptionMessage + Constants.vbCrLf + e.Exception.Message, 0, Resources.ExceptionTitle);
        }
    }
}