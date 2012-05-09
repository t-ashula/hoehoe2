using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class SearchWord : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWord));
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.SWordText = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.CheckSearchCaseSensitive = new System.Windows.Forms.CheckBox();
			this.CheckSearchRegex = new System.Windows.Forms.CheckBox();
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.AccessibleDescription = null;
			this.TableLayoutPanel1.AccessibleName = null;
			resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
			this.TableLayoutPanel1.BackgroundImage = null;
			this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
			this.TableLayoutPanel1.Font = null;
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			//
			//OK_Button
			//
			this.OK_Button.AccessibleDescription = null;
			this.OK_Button.AccessibleName = null;
			resources.ApplyResources(this.OK_Button, "OK_Button");
			this.OK_Button.BackgroundImage = null;
			this.OK_Button.Font = null;
			this.OK_Button.Name = "OK_Button";
			//
			//Cancel_Button
			//
			this.Cancel_Button.AccessibleDescription = null;
			this.Cancel_Button.AccessibleName = null;
			resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
			this.Cancel_Button.BackgroundImage = null;
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Font = null;
			this.Cancel_Button.Name = "Cancel_Button";
			//
			//SWordText
			//
			this.SWordText.AccessibleDescription = null;
			this.SWordText.AccessibleName = null;
			resources.ApplyResources(this.SWordText, "SWordText");
			this.SWordText.BackgroundImage = null;
			this.SWordText.Font = null;
			this.SWordText.Name = "SWordText";
			//
			//Label1
			//
			this.Label1.AccessibleDescription = null;
			this.Label1.AccessibleName = null;
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Font = null;
			this.Label1.Name = "Label1";
			//
			//CheckSearchCaseSensitive
			//
			this.CheckSearchCaseSensitive.AccessibleDescription = null;
			this.CheckSearchCaseSensitive.AccessibleName = null;
			resources.ApplyResources(this.CheckSearchCaseSensitive, "CheckSearchCaseSensitive");
			this.CheckSearchCaseSensitive.BackgroundImage = null;
			this.CheckSearchCaseSensitive.Font = null;
			this.CheckSearchCaseSensitive.Name = "CheckSearchCaseSensitive";
			this.CheckSearchCaseSensitive.UseVisualStyleBackColor = true;
			//
			//CheckSearchRegex
			//
			this.CheckSearchRegex.AccessibleDescription = null;
			this.CheckSearchRegex.AccessibleName = null;
			resources.ApplyResources(this.CheckSearchRegex, "CheckSearchRegex");
			this.CheckSearchRegex.BackgroundImage = null;
			this.CheckSearchRegex.Font = null;
			this.CheckSearchRegex.Name = "CheckSearchRegex";
			this.CheckSearchRegex.UseVisualStyleBackColor = true;
			//
			//SearchWord
			//
			this.AcceptButton = this.OK_Button;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.Cancel_Button;
			this.Controls.Add(this.CheckSearchRegex);
			this.Controls.Add(this.CheckSearchCaseSensitive);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.SWordText);
			this.Controls.Add(this.TableLayoutPanel1);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchWord";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.TableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
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
		internal System.Windows.Forms.TextBox SWordText;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.CheckBox CheckSearchCaseSensitive;

		internal System.Windows.Forms.CheckBox CheckSearchRegex;
	}
}
