using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Hoehoe
{
    partial class HashtagManage : System.Windows.Forms.Form
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

        //Windows フォーム デザイナで必要です。
        private System.ComponentModel.IContainer components;

        //メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        //Windows フォーム デザイナを使用して変更できます。  
        //コード エディタを使って変更しないでください。
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashtagManage));
            this.TableLayoutButtons = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.HistoryHashList = new System.Windows.Forms.ListBox();
            this.UseHashText = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.CheckPermanent = new System.Windows.Forms.CheckBox();
            this.GroupDetail = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.PermOkButton = new System.Windows.Forms.Button();
            this.PermCancelButton = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.RadioLast = new System.Windows.Forms.RadioButton();
            this.RadioHead = new System.Windows.Forms.RadioButton();
            this.UnSelectButton = new System.Windows.Forms.Button();
            this.GroupHashtag = new System.Windows.Forms.GroupBox();
            this.CheckNotAddToAtReply = new System.Windows.Forms.CheckBox();
            this.TableLayoutButtons.SuspendLayout();
            this.GroupDetail.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.GroupHashtag.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutButtons
            // 
            resources.ApplyResources(this.TableLayoutButtons, "TableLayoutButtons");
            this.TableLayoutButtons.Controls.Add(this.cancelButton, 1, 0);
            this.TableLayoutButtons.Controls.Add(this.okButton, 0, 0);
            this.TableLayoutButtons.Name = "TableLayoutButtons";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // HistoryHashList
            // 
            this.HistoryHashList.FormattingEnabled = true;
            resources.ApplyResources(this.HistoryHashList, "HistoryHashList");
            this.HistoryHashList.Name = "HistoryHashList";
            this.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.HistoryHashList.DoubleClick += new System.EventHandler(this.HistoryHashList_DoubleClick);
            this.HistoryHashList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryHashList_KeyDown);
            // 
            // UseHashText
            // 
            resources.ApplyResources(this.UseHashText, "UseHashText");
            this.UseHashText.Name = "UseHashText";
            this.UseHashText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UseHashText_KeyPress);
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // CheckPermanent
            // 
            resources.ApplyResources(this.CheckPermanent, "CheckPermanent");
            this.CheckPermanent.Name = "CheckPermanent";
            this.CheckPermanent.UseVisualStyleBackColor = true;
            // 
            // GroupDetail
            // 
            this.GroupDetail.Controls.Add(this.TableLayoutPanel2);
            this.GroupDetail.Controls.Add(this.UseHashText);
            this.GroupDetail.Controls.Add(this.Label1);
            resources.ApplyResources(this.GroupDetail, "GroupDetail");
            this.GroupDetail.Name = "GroupDetail";
            this.GroupDetail.TabStop = false;
            // 
            // TableLayoutPanel2
            // 
            resources.ApplyResources(this.TableLayoutPanel2, "TableLayoutPanel2");
            this.TableLayoutPanel2.Controls.Add(this.PermOkButton, 0, 0);
            this.TableLayoutPanel2.Controls.Add(this.PermCancelButton, 1, 0);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            // 
            // PermOkButton
            // 
            resources.ApplyResources(this.PermOkButton, "PermOkButton");
            this.PermOkButton.Name = "PermOkButton";
            this.PermOkButton.Click += new System.EventHandler(this.PermOkButton_Click);
            // 
            // PermCancelButton
            // 
            resources.ApplyResources(this.PermCancelButton, "PermCancelButton");
            this.PermCancelButton.Name = "PermCancelButton";
            this.PermCancelButton.Click += new System.EventHandler(this.PermCancelButton_Click);
            // 
            // Label3
            // 
            resources.ApplyResources(this.Label3, "Label3");
            this.Label3.Name = "Label3";
            // 
            // RadioLast
            // 
            resources.ApplyResources(this.RadioLast, "RadioLast");
            this.RadioLast.Name = "RadioLast";
            this.RadioLast.TabStop = true;
            this.RadioLast.UseVisualStyleBackColor = true;
            // 
            // RadioHead
            // 
            resources.ApplyResources(this.RadioHead, "RadioHead");
            this.RadioHead.Name = "RadioHead";
            this.RadioHead.TabStop = true;
            this.RadioHead.UseVisualStyleBackColor = true;
            // 
            // UnSelectButton
            // 
            resources.ApplyResources(this.UnSelectButton, "UnSelectButton");
            this.UnSelectButton.Name = "UnSelectButton";
            this.UnSelectButton.UseVisualStyleBackColor = true;
            this.UnSelectButton.Click += new System.EventHandler(this.UnSelectButton_Click);
            // 
            // GroupHashtag
            // 
            this.GroupHashtag.Controls.Add(this.HistoryHashList);
            this.GroupHashtag.Controls.Add(this.Label3);
            this.GroupHashtag.Controls.Add(this.UnSelectButton);
            this.GroupHashtag.Controls.Add(this.RadioLast);
            this.GroupHashtag.Controls.Add(this.DeleteButton);
            this.GroupHashtag.Controls.Add(this.RadioHead);
            this.GroupHashtag.Controls.Add(this.EditButton);
            this.GroupHashtag.Controls.Add(this.AddButton);
            this.GroupHashtag.Controls.Add(this.CheckPermanent);
            resources.ApplyResources(this.GroupHashtag, "GroupHashtag");
            this.GroupHashtag.Name = "GroupHashtag";
            this.GroupHashtag.TabStop = false;
            // 
            // CheckNotAddToAtReply
            // 
            resources.ApplyResources(this.CheckNotAddToAtReply, "CheckNotAddToAtReply");
            this.CheckNotAddToAtReply.Checked = true;
            this.CheckNotAddToAtReply.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckNotAddToAtReply.Name = "CheckNotAddToAtReply";
            this.CheckNotAddToAtReply.UseVisualStyleBackColor = true;
            this.CheckNotAddToAtReply.CheckedChanged += new System.EventHandler(this.CheckNotAddToAtReply_CheckedChanged);
            // 
            // HashtagManage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CheckNotAddToAtReply);
            this.Controls.Add(this.GroupHashtag);
            this.Controls.Add(this.GroupDetail);
            this.Controls.Add(this.TableLayoutButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HashtagManage";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.HashtagManage_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HashtagManage_KeyDown);
            this.TableLayoutButtons.ResumeLayout(false);
            this.GroupDetail.ResumeLayout(false);
            this.GroupDetail.PerformLayout();
            this.TableLayoutPanel2.ResumeLayout(false);
            this.GroupHashtag.ResumeLayout(false);
            this.GroupHashtag.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.TableLayoutPanel TableLayoutButtons;
        internal System.Windows.Forms.Button DeleteButton;
        internal System.Windows.Forms.Button EditButton;
        internal System.Windows.Forms.Button AddButton;
        internal System.Windows.Forms.ListBox HistoryHashList;
        internal System.Windows.Forms.TextBox UseHashText;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.CheckBox CheckPermanent;
        internal System.Windows.Forms.GroupBox GroupDetail;
        internal System.Windows.Forms.RadioButton RadioLast;
        internal System.Windows.Forms.RadioButton RadioHead;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        internal System.Windows.Forms.Button PermOkButton;
        internal System.Windows.Forms.Button PermCancelButton;
        internal System.Windows.Forms.Button UnSelectButton;
        internal System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.Button okButton;
        internal System.Windows.Forms.GroupBox GroupHashtag;
        internal System.Windows.Forms.CheckBox CheckNotAddToAtReply;
    }
}