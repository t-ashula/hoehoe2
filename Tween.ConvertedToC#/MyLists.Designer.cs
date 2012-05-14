
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
    partial class MyLists : System.Windows.Forms.Form
    {

        //フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
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
            this.AddListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ReloadListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListRefreshButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListsCheckedListBox
            // 
            resources.ApplyResources(this.ListsCheckedListBox, "ListsCheckedListBox");
            this.ListsCheckedListBox.CheckOnClick = true;
            this.ListsCheckedListBox.ContextMenuStrip = this.ContextMenuStrip1;
            this.ListsCheckedListBox.FormattingEnabled = true;
            this.ListsCheckedListBox.Name = "ListsCheckedListBox";
            this.ListsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListsCheckedListBox_ItemCheck);
            this.ListsCheckedListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListsCheckedListBox_MouseDown);
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddListToolStripMenuItem,
            this.DeleteListToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.ReloadListToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            resources.ApplyResources(this.ContextMenuStrip1, "ContextMenuStrip1");
            this.ContextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // AddListToolStripMenuItem
            // 
            this.AddListToolStripMenuItem.Name = "AddListToolStripMenuItem";
            resources.ApplyResources(this.AddListToolStripMenuItem, "AddListToolStripMenuItem");
            this.AddListToolStripMenuItem.Click += new System.EventHandler(this.AddListToolStripMenuItem_Click);
            // 
            // DeleteListToolStripMenuItem
            // 
            this.DeleteListToolStripMenuItem.Name = "DeleteListToolStripMenuItem";
            resources.ApplyResources(this.DeleteListToolStripMenuItem, "DeleteListToolStripMenuItem");
            this.DeleteListToolStripMenuItem.Click += new System.EventHandler(this.DeleteListToolStripMenuItem_Click);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
            // 
            // ReloadListToolStripMenuItem
            // 
            this.ReloadListToolStripMenuItem.Name = "ReloadListToolStripMenuItem";
            resources.ApplyResources(this.ReloadListToolStripMenuItem, "ReloadListToolStripMenuItem");
            this.ReloadListToolStripMenuItem.Click += new System.EventHandler(this.ReloadListToolStripMenuItem_Click);
            // 
            // ListRefreshButton
            // 
            resources.ApplyResources(this.ListRefreshButton, "ListRefreshButton");
            this.ListRefreshButton.Name = "ListRefreshButton";
            this.ListRefreshButton.UseVisualStyleBackColor = true;
            this.ListRefreshButton.Click += new System.EventHandler(this.ListRefreshButton_Click);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MyLists
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
            this.Load += new System.EventHandler(this.MyLists_Load);
            this.ContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal System.Windows.Forms.CheckedListBox ListsCheckedListBox;
        internal System.Windows.Forms.Button ListRefreshButton;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem AddListToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem DeleteListToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ReloadListToolStripMenuItem;
        internal System.Windows.Forms.Button CloseButton;
    }
}