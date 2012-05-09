using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class OpenURL : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenURL));
			this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.UrlList = new System.Windows.Forms.ListBox();
			this.TableLayoutPanel2.SuspendLayout();
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			//TableLayoutPanel2
			//
			resources.ApplyResources(this.TableLayoutPanel2, "TableLayoutPanel2");
			this.TableLayoutPanel2.Controls.Add(this.TableLayoutPanel1, 0, 1);
			this.TableLayoutPanel2.Controls.Add(this.UrlList, 0, 0);
			this.TableLayoutPanel2.Name = "TableLayoutPanel2";
			//
			//TableLayoutPanel1
			//
			resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
			this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			//
			//OK_Button
			//
			resources.ApplyResources(this.OK_Button, "OK_Button");
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.OK_Button.Name = "OK_Button";
			//
			//Cancel_Button
			//
			resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Name = "Cancel_Button";
			//
			//UrlList
			//
			this.UrlList.DisplayMember = "Text";
			resources.ApplyResources(this.UrlList, "UrlList");
			this.UrlList.FormattingEnabled = true;
			this.UrlList.Name = "UrlList";
			this.UrlList.ValueMember = "Url";
			//
			//OpenURL
			//
			this.AcceptButton = this.OK_Button;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel_Button;
			this.ControlBox = false;
			this.Controls.Add(this.TableLayoutPanel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenURL";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.TableLayoutPanel2.ResumeLayout(false);
			this.TableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
		internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		private System.Windows.Forms.Button withEventsField_OK_Button;
		internal System.Windows.Forms.Button OK_Button {
			get { return withEventsField_OK_Button; }
			set {
				if (withEventsField_OK_Button != null) {
					withEventsField_OK_Button.Click -= OK_Button_Click;
				}
				withEventsField_OK_Button = value;
				if (withEventsField_OK_Button != null) {
					withEventsField_OK_Button.Click += OK_Button_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_Cancel_Button;
		internal System.Windows.Forms.Button Cancel_Button {
			get { return withEventsField_Cancel_Button; }
			set {
				if (withEventsField_Cancel_Button != null) {
					withEventsField_Cancel_Button.Click -= Cancel_Button_Click;
				}
				withEventsField_Cancel_Button = value;
				if (withEventsField_Cancel_Button != null) {
					withEventsField_Cancel_Button.Click += Cancel_Button_Click;
				}
			}
		}
		private System.Windows.Forms.ListBox withEventsField_UrlList;
		internal System.Windows.Forms.ListBox UrlList {
			get { return withEventsField_UrlList; }
			set {
				if (withEventsField_UrlList != null) {
					withEventsField_UrlList.DoubleClick -= UrlList_DoubleClick;
					withEventsField_UrlList.KeyDown -= UrlList_KeyDown;
				}
				withEventsField_UrlList = value;
				if (withEventsField_UrlList != null) {
					withEventsField_UrlList.DoubleClick += UrlList_DoubleClick;
					withEventsField_UrlList.KeyDown += UrlList_KeyDown;
				}
			}

		}
	}
}
