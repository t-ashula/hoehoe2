using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.1
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------



namespace Tween.My
{

	[System.Runtime.CompilerServices.CompilerGeneratedAttribute(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : global::System.Configuration.ApplicationSettingsBase
	{

		private static MySettings defaultInstance = (MySettings)global::System.Configuration.ApplicationSettingsBase.Synchronized(new MySettings());

		#region "My.Settings 自動保存機能"

		private static bool addedHandler;

		private static object addedHandlerLockObject = new object();
		[System.Diagnostics.DebuggerNonUserCodeAttribute(), System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(global::System.Object sender, global::System.EventArgs e)
		{
			if (MyProject.Application.SaveMySettingsOnExit) {
				Tween.My.Settings.Save();
			}
		}
		#endregion

		public static MySettings Default {
			get {

				if (!addedHandler) {
					lock (addedHandlerLockObject) {
						if (!addedHandler) {
							MyProject.Application.Shutdown += AutoSaveSettings;
							addedHandler = true;
						}
					}
				}
				return defaultInstance;
			}
		}
	}
}

namespace Tween.My
{

	[Microsoft.VisualBasic.HideModuleNameAttribute(), System.Diagnostics.DebuggerNonUserCodeAttribute(), System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	static internal class MySettingsProperty
	{

		[System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")]
		static internal global::Tween.My.MySettings Settings {
			get { return global::Tween.My.MySettings.Default; }
		}
	}
}
