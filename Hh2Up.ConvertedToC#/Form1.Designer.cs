using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace TweenUp
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class Form1 : System.Windows.Forms.Form
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

		//Windows フォーム デザイナで必要です。

		private System.ComponentModel.IContainer components;
		//メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
		//Windows フォーム デザイナを使用して変更できます。  
		//コード エディタを使って変更しないでください。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.LabelProgress = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("MS UI Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(128));
			this.Label1.Location = new System.Drawing.Point(102, 35);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(152, 13);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Tweenを更新しています・・・";
			this.Label1.UseMnemonic = false;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Font = new System.Drawing.Font("MS UI Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(128));
			this.Label2.Location = new System.Drawing.Point(97, 70);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(163, 13);
			this.Label2.TabIndex = 1;
			this.Label2.Text = "このまましばらくお待ちください。";
			this.Label2.UseMnemonic = false;
			//
			//BackgroundWorker1
			//
			//
			//LabelProgress
			//
			this.LabelProgress.AutoSize = true;
			this.LabelProgress.Font = new System.Drawing.Font("MS UI Gothic", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(128));
			this.LabelProgress.Location = new System.Drawing.Point(149, 135);
			this.LabelProgress.Name = "LabelProgress";
			this.LabelProgress.Size = new System.Drawing.Size(59, 13);
			this.LabelProgress.TabIndex = 2;
			this.LabelProgress.Text = "進行状況";
			this.LabelProgress.UseMnemonic = false;
			//
			//Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(357, 219);
			this.Controls.Add(this.LabelProgress);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label2;
		private System.ComponentModel.BackgroundWorker withEventsField_BackgroundWorker1;
		internal System.ComponentModel.BackgroundWorker BackgroundWorker1 {
			get { return withEventsField_BackgroundWorker1; }
			set {
				if (withEventsField_BackgroundWorker1 != null) {
					withEventsField_BackgroundWorker1.DoWork -= BackgroundWorker1_DoWork;
					withEventsField_BackgroundWorker1.RunWorkerCompleted -= BackgroundWorker1_RunWorkerCompleted;
					withEventsField_BackgroundWorker1.ProgressChanged -= BackgroundWorker1_ProgressChanged;
				}
				withEventsField_BackgroundWorker1 = value;
				if (withEventsField_BackgroundWorker1 != null) {
					withEventsField_BackgroundWorker1.DoWork += BackgroundWorker1_DoWork;
					withEventsField_BackgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
					withEventsField_BackgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
				}
			}
		}

		internal System.Windows.Forms.Label LabelProgress;
	}
}
