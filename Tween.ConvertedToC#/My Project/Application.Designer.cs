using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.235
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------



namespace Tween.My
{

	//メモ: このファイルは自動生成されました。直接変更しないでください。変更したり、
	// ビルド エラーが発生した場合は、プロジェクト デザイナー へ移動し (プロジェクト
	// プロパティに移動するか、またはソリューション エクスプローラーのマイ プロジェクト
	// ノード上でダブルクリック)、アプリケーション タブ上で変更を行います。
	//
	internal partial class MyApplication
	{

		[System.Diagnostics.DebuggerStepThroughAttribute()]
		public MyApplication() : base(global::Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.ApplicationDefined)
		{
			UnhandledException += MyApplication_UnhandledException;
			Startup += MyApplication_Startup;
			Shutdown += MyApplication_Shutdown;
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = false;
			this.ShutdownStyle = global::Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
		}

		[System.Diagnostics.DebuggerStepThroughAttribute()]
		protected override void OnCreateMainForm()
		{
			this.MainForm = My.MyProject.Forms.TweenMain;
		}
	}
}
