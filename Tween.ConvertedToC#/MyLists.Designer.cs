using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class MyLists : System.Windows.Forms.Form
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyLists));
			this.ListsCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.追加AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.削除DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.更新RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListRefreshButton = new System.Windows.Forms.Button();
			this.CloseButton = new System.Windows.Forms.Button();
			this.ContextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			//ListsCheckedListBox
			//
			resources.ApplyResources(this.ListsCheckedListBox, "ListsCheckedListBox");
			this.ListsCheckedListBox.CheckOnClick = true;
			this.ListsCheckedListBox.ContextMenuStrip = this.ContextMenuStrip1;
			this.ListsCheckedListBox.FormattingEnabled = true;
			this.ListsCheckedListBox.Name = "ListsCheckedListBox";
			//
			//ContextMenuStrip1
			//
			this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.追加AToolStripMenuItem,
				this.削除DToolStripMenuItem,
				this.ToolStripMenuItem1,
				this.更新RToolStripMenuItem
			});
			this.ContextMenuStrip1.Name = "ContextMenuStrip1";
			resources.ApplyResources(this.ContextMenuStrip1, "ContextMenuStrip1");
			//
			//追加AToolStripMenuItem
			//
			this.追加AToolStripMenuItem.Name = "追加AToolStripMenuItem";
			resources.ApplyResources(this.追加AToolStripMenuItem, "追加AToolStripMenuItem");
			//
			//削除DToolStripMenuItem
			//
			this.削除DToolStripMenuItem.Name = "削除DToolStripMenuItem";
			resources.ApplyResources(this.削除DToolStripMenuItem, "削除DToolStripMenuItem");
			//
			//ToolStripMenuItem1
			//
			this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
			resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
			//
			//更新RToolStripMenuItem
			//
			this.更新RToolStripMenuItem.Name = "更新RToolStripMenuItem";
			resources.ApplyResources(this.更新RToolStripMenuItem, "更新RToolStripMenuItem");
			//
			//ListRefreshButton
			//
			resources.ApplyResources(this.ListRefreshButton, "ListRefreshButton");
			this.ListRefreshButton.Name = "ListRefreshButton";
			this.ListRefreshButton.UseVisualStyleBackColor = true;
			//
			//CloseButton
			//
			resources.ApplyResources(this.CloseButton, "CloseButton");
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.UseVisualStyleBackColor = true;
			//
			//MyLists
			//
			this.AcceptButton = this.CloseButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CloseButton;
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.ListRefreshButton);
			this.Controls.Add(this.ListsCheckedListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MyLists";
			this.ShowInTaskbar = false;
			this.ContextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.CheckedListBox withEventsField_ListsCheckedListBox;
		internal System.Windows.Forms.CheckedListBox ListsCheckedListBox {
			get { return withEventsField_ListsCheckedListBox; }
			set {
				if (withEventsField_ListsCheckedListBox != null) {
					withEventsField_ListsCheckedListBox.ItemCheck -= ListsCheckedListBox_ItemCheck;
					withEventsField_ListsCheckedListBox.MouseDown -= ListsCheckedListBox_MouseDown;
				}
				withEventsField_ListsCheckedListBox = value;
				if (withEventsField_ListsCheckedListBox != null) {
					withEventsField_ListsCheckedListBox.ItemCheck += ListsCheckedListBox_ItemCheck;
					withEventsField_ListsCheckedListBox.MouseDown += ListsCheckedListBox_MouseDown;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ListRefreshButton;
		internal System.Windows.Forms.Button ListRefreshButton {
			get { return withEventsField_ListRefreshButton; }
			set {
				if (withEventsField_ListRefreshButton != null) {
					withEventsField_ListRefreshButton.Click -= ListRefreshButton_Click;
				}
				withEventsField_ListRefreshButton = value;
				if (withEventsField_ListRefreshButton != null) {
					withEventsField_ListRefreshButton.Click += ListRefreshButton_Click;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuStrip1;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1 {
			get { return withEventsField_ContextMenuStrip1; }
			set {
				if (withEventsField_ContextMenuStrip1 != null) {
					withEventsField_ContextMenuStrip1.Opening -= ContextMenuStrip1_Opening;
				}
				withEventsField_ContextMenuStrip1 = value;
				if (withEventsField_ContextMenuStrip1 != null) {
					withEventsField_ContextMenuStrip1.Opening += ContextMenuStrip1_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_追加AToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem 追加AToolStripMenuItem {
			get { return withEventsField_追加AToolStripMenuItem; }
			set {
				if (withEventsField_追加AToolStripMenuItem != null) {
					withEventsField_追加AToolStripMenuItem.Click -= 追加AToolStripMenuItem_Click;
				}
				withEventsField_追加AToolStripMenuItem = value;
				if (withEventsField_追加AToolStripMenuItem != null) {
					withEventsField_追加AToolStripMenuItem.Click += 追加AToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_削除DToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem 削除DToolStripMenuItem {
			get { return withEventsField_削除DToolStripMenuItem; }
			set {
				if (withEventsField_削除DToolStripMenuItem != null) {
					withEventsField_削除DToolStripMenuItem.Click -= 削除DToolStripMenuItem_Click;
				}
				withEventsField_削除DToolStripMenuItem = value;
				if (withEventsField_削除DToolStripMenuItem != null) {
					withEventsField_削除DToolStripMenuItem.Click += 削除DToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_更新RToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem 更新RToolStripMenuItem {
			get { return withEventsField_更新RToolStripMenuItem; }
			set {
				if (withEventsField_更新RToolStripMenuItem != null) {
					withEventsField_更新RToolStripMenuItem.Click -= 更新RToolStripMenuItem_Click;
				}
				withEventsField_更新RToolStripMenuItem = value;
				if (withEventsField_更新RToolStripMenuItem != null) {
					withEventsField_更新RToolStripMenuItem.Click += 更新RToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_CloseButton;
		internal System.Windows.Forms.Button CloseButton {
			get { return withEventsField_CloseButton; }
			set {
				if (withEventsField_CloseButton != null) {
					withEventsField_CloseButton.Click -= CloseButton_Click;
				}
				withEventsField_CloseButton = value;
				if (withEventsField_CloseButton != null) {
					withEventsField_CloseButton.Click += CloseButton_Click;
				}
			}
		}
	}
}
