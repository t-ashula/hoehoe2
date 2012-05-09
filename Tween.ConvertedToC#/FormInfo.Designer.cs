using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class FormInfo : System.Windows.Forms.Form
	{

		//フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing && components != null) {
					components.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		//Windows フォーム デザイナーで必要です。

		private System.ComponentModel.IContainer components;
		//メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
		//Windows フォーム デザイナーを使用して変更できます。  
		//コード エディターを使って変更しないでください。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfo));
			this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
			this.LabelInformation = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			//ProgressBar1
			//
			resources.ApplyResources(this.ProgressBar1, "ProgressBar1");
			this.ProgressBar1.Name = "ProgressBar1";
			this.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			//
			//LabelInformation
			//
			resources.ApplyResources(this.LabelInformation, "LabelInformation");
			this.LabelInformation.Name = "LabelInformation";
			//
			//FormInfo
			//
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.LabelInformation);
			this.Controls.Add(this.ProgressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInfo";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		internal System.Windows.Forms.ProgressBar ProgressBar1;
		private System.Windows.Forms.Label withEventsField_LabelInformation;
		internal System.Windows.Forms.Label LabelInformation {
			get { return withEventsField_LabelInformation; }
			set {
				if (withEventsField_LabelInformation != null) {
					withEventsField_LabelInformation.TextChanged -= LabelInformation_TextChanged;
				}
				withEventsField_LabelInformation = value;
				if (withEventsField_LabelInformation != null) {
					withEventsField_LabelInformation.TextChanged += LabelInformation_TextChanged;
				}
			}
		}
	}
}
