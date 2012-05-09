using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class TabsDialog : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabsDialog));
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.TabList = new System.Windows.Forms.ListBox();
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
			//TabList
			//
			this.TabList.AccessibleDescription = null;
			this.TabList.AccessibleName = null;
			resources.ApplyResources(this.TabList, "TabList");
			this.TabList.BackgroundImage = null;
			this.TabList.Font = null;
			this.TabList.FormattingEnabled = true;
			this.TabList.Items.AddRange(new object[] { resources.GetString("TabList.Items") });
			this.TabList.Name = "TabList";
			//
			//TabsDialog
			//
			this.AcceptButton = this.OK_Button;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.Cancel_Button;
			this.Controls.Add(this.TabList);
			this.Controls.Add(this.TableLayoutPanel1);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TabsDialog";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.TableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

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
		private System.Windows.Forms.ListBox withEventsField_TabList;
		internal System.Windows.Forms.ListBox TabList {
			get { return withEventsField_TabList; }
			set {
				if (withEventsField_TabList != null) {
					withEventsField_TabList.SelectedIndexChanged -= TabList_SelectedIndexChanged;
					withEventsField_TabList.DoubleClick -= TabList_DoubleClick;
				}
				withEventsField_TabList = value;
				if (withEventsField_TabList != null) {
					withEventsField_TabList.SelectedIndexChanged += TabList_SelectedIndexChanged;
					withEventsField_TabList.DoubleClick += TabList_DoubleClick;
				}
			}

		}
	}
}
