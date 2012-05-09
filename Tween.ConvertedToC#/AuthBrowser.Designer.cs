using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class AuthBrowser : System.Windows.Forms.Form
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
			this.Panel1 = new System.Windows.Forms.Panel();
			this.AddressLabel = new System.Windows.Forms.Label();
			this.AuthWebBrowser = new System.Windows.Forms.WebBrowser();
			this.Panel2 = new System.Windows.Forms.Panel();
			this.Cancel = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.PinText = new System.Windows.Forms.TextBox();
			this.NextButton = new System.Windows.Forms.Button();
			this.Panel2.SuspendLayout();
			this.SuspendLayout();
			//
			//Panel1
			//
			this.Panel1.AutoSize = true;
			this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.Panel1.Location = new System.Drawing.Point(0, 22);
			this.Panel1.Name = "Panel1";
			this.Panel1.Size = new System.Drawing.Size(773, 0);
			this.Panel1.TabIndex = 0;
			//
			//AddressLabel
			//
			this.AddressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AddressLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AddressLabel.Location = new System.Drawing.Point(0, 0);
			this.AddressLabel.Name = "AddressLabel";
			this.AddressLabel.Size = new System.Drawing.Size(531, 22);
			this.AddressLabel.TabIndex = 0;
			this.AddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//AuthWebBrowser
			//
			this.AuthWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AuthWebBrowser.Location = new System.Drawing.Point(0, 22);
			this.AuthWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.AuthWebBrowser.Name = "AuthWebBrowser";
			this.AuthWebBrowser.Size = new System.Drawing.Size(773, 540);
			this.AuthWebBrowser.TabIndex = 1;
			//
			//Panel2
			//
			this.Panel2.Controls.Add(this.Cancel);
			this.Panel2.Controls.Add(this.AddressLabel);
			this.Panel2.Controls.Add(this.Label1);
			this.Panel2.Controls.Add(this.PinText);
			this.Panel2.Controls.Add(this.NextButton);
			this.Panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.Panel2.Location = new System.Drawing.Point(0, 0);
			this.Panel2.Name = "Panel2";
			this.Panel2.Size = new System.Drawing.Size(773, 22);
			this.Panel2.TabIndex = 2;
			//
			//Cancel
			//
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(536, 32);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 15);
			this.Cancel.TabIndex = 3;
			this.Cancel.TabStop = false;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Dock = System.Windows.Forms.DockStyle.Right;
			this.Label1.Location = new System.Drawing.Point(531, 0);
			this.Label1.Name = "Label1";
			this.Label1.Padding = new System.Windows.Forms.Padding(3);
			this.Label1.Size = new System.Drawing.Size(29, 18);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "PIN";
			//
			//PinText
			//
			this.PinText.Dock = System.Windows.Forms.DockStyle.Right;
			this.PinText.Location = new System.Drawing.Point(560, 0);
			this.PinText.Name = "PinText";
			this.PinText.Size = new System.Drawing.Size(138, 19);
			this.PinText.TabIndex = 1;
			//
			//NextButton
			//
			this.NextButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.NextButton.Location = new System.Drawing.Point(698, 0);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(75, 22);
			this.NextButton.TabIndex = 2;
			this.NextButton.Text = "Finish";
			this.NextButton.UseVisualStyleBackColor = true;
			//
			//AuthBrowser
			//
			this.AcceptButton = this.NextButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(773, 562);
			this.Controls.Add(this.AuthWebBrowser);
			this.Controls.Add(this.Panel1);
			this.Controls.Add(this.Panel2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AuthBrowser";
			this.ShowIcon = false;
			this.Text = "Browser";
			this.Panel2.ResumeLayout(false);
			this.Panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		internal System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.WebBrowser withEventsField_AuthWebBrowser;
		internal System.Windows.Forms.WebBrowser AuthWebBrowser {
			get { return withEventsField_AuthWebBrowser; }
			set {
				if (withEventsField_AuthWebBrowser != null) {
					withEventsField_AuthWebBrowser.DocumentCompleted -= AuthWebBrowser_DocumentCompleted;
					withEventsField_AuthWebBrowser.Navigating -= AuthWebBrowser_Navigating;
				}
				withEventsField_AuthWebBrowser = value;
				if (withEventsField_AuthWebBrowser != null) {
					withEventsField_AuthWebBrowser.DocumentCompleted += AuthWebBrowser_DocumentCompleted;
					withEventsField_AuthWebBrowser.Navigating += AuthWebBrowser_Navigating;
				}
			}
		}
		internal System.Windows.Forms.Label AddressLabel;
		internal System.Windows.Forms.Panel Panel2;
		private System.Windows.Forms.Button withEventsField_NextButton;
		internal System.Windows.Forms.Button NextButton {
			get { return withEventsField_NextButton; }
			set {
				if (withEventsField_NextButton != null) {
					withEventsField_NextButton.Click -= NextButton_Click;
				}
				withEventsField_NextButton = value;
				if (withEventsField_NextButton != null) {
					withEventsField_NextButton.Click += NextButton_Click;
				}
			}
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.TextBox PinText;
		private System.Windows.Forms.Button withEventsField_Cancel;
		internal System.Windows.Forms.Button Cancel {
			get { return withEventsField_Cancel; }
			set {
				if (withEventsField_Cancel != null) {
					withEventsField_Cancel.Click -= Cancel_Click;
				}
				withEventsField_Cancel = value;
				if (withEventsField_Cancel != null) {
					withEventsField_Cancel.Click += Cancel_Click;
				}
			}
		}
	}
}
