using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class HashtagManage : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashtagManage));
			this.TableLayoutButtons = new System.Windows.Forms.TableLayoutPanel();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.OK_Button = new System.Windows.Forms.Button();
			this.DeleteButton = new System.Windows.Forms.Button();
			this.EditButton = new System.Windows.Forms.Button();
			this.AddButton = new System.Windows.Forms.Button();
			this.HistoryHashList = new System.Windows.Forms.ListBox();
			this.UseHashText = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.CheckPermanent = new System.Windows.Forms.CheckBox();
			this.GroupDetail = new System.Windows.Forms.GroupBox();
			this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.PermOK_Button = new System.Windows.Forms.Button();
			this.PermCancel_Button = new System.Windows.Forms.Button();
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
			//TableLayoutButtons
			//
			resources.ApplyResources(this.TableLayoutButtons, "TableLayoutButtons");
			this.TableLayoutButtons.Controls.Add(this.Cancel_Button, 1, 0);
			this.TableLayoutButtons.Controls.Add(this.OK_Button, 0, 0);
			this.TableLayoutButtons.Name = "TableLayoutButtons";
			//
			//Cancel_Button
			//
			resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Name = "Cancel_Button";
			//
			//OK_Button
			//
			resources.ApplyResources(this.OK_Button, "OK_Button");
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Name = "OK_Button";
			//
			//DeleteButton
			//
			resources.ApplyResources(this.DeleteButton, "DeleteButton");
			this.DeleteButton.Name = "DeleteButton";
			this.DeleteButton.UseVisualStyleBackColor = true;
			//
			//EditButton
			//
			resources.ApplyResources(this.EditButton, "EditButton");
			this.EditButton.Name = "EditButton";
			this.EditButton.UseVisualStyleBackColor = true;
			//
			//AddButton
			//
			resources.ApplyResources(this.AddButton, "AddButton");
			this.AddButton.Name = "AddButton";
			this.AddButton.UseVisualStyleBackColor = true;
			//
			//HistoryHashList
			//
			this.HistoryHashList.FormattingEnabled = true;
			resources.ApplyResources(this.HistoryHashList, "HistoryHashList");
			this.HistoryHashList.Name = "HistoryHashList";
			this.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			//
			//UseHashText
			//
			resources.ApplyResources(this.UseHashText, "UseHashText");
			this.UseHashText.Name = "UseHashText";
			//
			//Label1
			//
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Name = "Label1";
			//
			//CheckPermanent
			//
			resources.ApplyResources(this.CheckPermanent, "CheckPermanent");
			this.CheckPermanent.Name = "CheckPermanent";
			this.CheckPermanent.UseVisualStyleBackColor = true;
			//
			//GroupDetail
			//
			this.GroupDetail.Controls.Add(this.TableLayoutPanel2);
			this.GroupDetail.Controls.Add(this.UseHashText);
			this.GroupDetail.Controls.Add(this.Label1);
			resources.ApplyResources(this.GroupDetail, "GroupDetail");
			this.GroupDetail.Name = "GroupDetail";
			this.GroupDetail.TabStop = false;
			//
			//TableLayoutPanel2
			//
			resources.ApplyResources(this.TableLayoutPanel2, "TableLayoutPanel2");
			this.TableLayoutPanel2.Controls.Add(this.PermOK_Button, 0, 0);
			this.TableLayoutPanel2.Controls.Add(this.PermCancel_Button, 1, 0);
			this.TableLayoutPanel2.Name = "TableLayoutPanel2";
			//
			//PermOK_Button
			//
			resources.ApplyResources(this.PermOK_Button, "PermOK_Button");
			this.PermOK_Button.Name = "PermOK_Button";
			//
			//PermCancel_Button
			//
			resources.ApplyResources(this.PermCancel_Button, "PermCancel_Button");
			this.PermCancel_Button.Name = "PermCancel_Button";
			//
			//Label3
			//
			resources.ApplyResources(this.Label3, "Label3");
			this.Label3.Name = "Label3";
			//
			//RadioLast
			//
			resources.ApplyResources(this.RadioLast, "RadioLast");
			this.RadioLast.Name = "RadioLast";
			this.RadioLast.TabStop = true;
			this.RadioLast.UseVisualStyleBackColor = true;
			//
			//RadioHead
			//
			resources.ApplyResources(this.RadioHead, "RadioHead");
			this.RadioHead.Name = "RadioHead";
			this.RadioHead.TabStop = true;
			this.RadioHead.UseVisualStyleBackColor = true;
			//
			//UnSelectButton
			//
			resources.ApplyResources(this.UnSelectButton, "UnSelectButton");
			this.UnSelectButton.Name = "UnSelectButton";
			this.UnSelectButton.UseVisualStyleBackColor = true;
			//
			//GroupHashtag
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
			//CheckNotAddToAtReply
			//
			resources.ApplyResources(this.CheckNotAddToAtReply, "CheckNotAddToAtReply");
			this.CheckNotAddToAtReply.Checked = true;
			this.CheckNotAddToAtReply.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckNotAddToAtReply.Name = "CheckNotAddToAtReply";
			this.CheckNotAddToAtReply.UseVisualStyleBackColor = true;
			//
			//HashtagManage
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
		private System.Windows.Forms.Button withEventsField_DeleteButton;
		internal System.Windows.Forms.Button DeleteButton {
			get { return withEventsField_DeleteButton; }
			set {
				if (withEventsField_DeleteButton != null) {
					withEventsField_DeleteButton.Click -= DeleteButton_Click;
				}
				withEventsField_DeleteButton = value;
				if (withEventsField_DeleteButton != null) {
					withEventsField_DeleteButton.Click += DeleteButton_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_EditButton;
		internal System.Windows.Forms.Button EditButton {
			get { return withEventsField_EditButton; }
			set {
				if (withEventsField_EditButton != null) {
					withEventsField_EditButton.Click -= EditButton_Click;
				}
				withEventsField_EditButton = value;
				if (withEventsField_EditButton != null) {
					withEventsField_EditButton.Click += EditButton_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_AddButton;
		internal System.Windows.Forms.Button AddButton {
			get { return withEventsField_AddButton; }
			set {
				if (withEventsField_AddButton != null) {
					withEventsField_AddButton.Click -= AddButton_Click;
				}
				withEventsField_AddButton = value;
				if (withEventsField_AddButton != null) {
					withEventsField_AddButton.Click += AddButton_Click;
				}
			}
		}
		private System.Windows.Forms.ListBox withEventsField_HistoryHashList;
		internal System.Windows.Forms.ListBox HistoryHashList {
			get { return withEventsField_HistoryHashList; }
			set {
				if (withEventsField_HistoryHashList != null) {
					withEventsField_HistoryHashList.DoubleClick -= HistoryHashList_DoubleClick;
					withEventsField_HistoryHashList.KeyDown -= HistoryHashList_KeyDown;
				}
				withEventsField_HistoryHashList = value;
				if (withEventsField_HistoryHashList != null) {
					withEventsField_HistoryHashList.DoubleClick += HistoryHashList_DoubleClick;
					withEventsField_HistoryHashList.KeyDown += HistoryHashList_KeyDown;
				}
			}
		}
		private System.Windows.Forms.TextBox withEventsField_UseHashText;
		internal System.Windows.Forms.TextBox UseHashText {
			get { return withEventsField_UseHashText; }
			set {
				if (withEventsField_UseHashText != null) {
					withEventsField_UseHashText.KeyPress -= UseHashText_KeyPress;
				}
				withEventsField_UseHashText = value;
				if (withEventsField_UseHashText != null) {
					withEventsField_UseHashText.KeyPress += UseHashText_KeyPress;
				}
			}
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.CheckBox CheckPermanent;
		internal System.Windows.Forms.GroupBox GroupDetail;
		internal System.Windows.Forms.RadioButton RadioLast;
		internal System.Windows.Forms.RadioButton RadioHead;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
		private System.Windows.Forms.Button withEventsField_PermOK_Button;
		internal System.Windows.Forms.Button PermOK_Button {
			get { return withEventsField_PermOK_Button; }
			set {
				if (withEventsField_PermOK_Button != null) {
					withEventsField_PermOK_Button.Click -= PermOK_Button_Click;
				}
				withEventsField_PermOK_Button = value;
				if (withEventsField_PermOK_Button != null) {
					withEventsField_PermOK_Button.Click += PermOK_Button_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_PermCancel_Button;
		internal System.Windows.Forms.Button PermCancel_Button {
			get { return withEventsField_PermCancel_Button; }
			set {
				if (withEventsField_PermCancel_Button != null) {
					withEventsField_PermCancel_Button.Click -= PermCancel_Button_Click;
				}
				withEventsField_PermCancel_Button = value;
				if (withEventsField_PermCancel_Button != null) {
					withEventsField_PermCancel_Button.Click += PermCancel_Button_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_UnSelectButton;
		internal System.Windows.Forms.Button UnSelectButton {
			get { return withEventsField_UnSelectButton; }
			set {
				if (withEventsField_UnSelectButton != null) {
					withEventsField_UnSelectButton.Click -= UnSelectButton_Click;
				}
				withEventsField_UnSelectButton = value;
				if (withEventsField_UnSelectButton != null) {
					withEventsField_UnSelectButton.Click += UnSelectButton_Click;
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
		internal System.Windows.Forms.GroupBox GroupHashtag;
		private System.Windows.Forms.CheckBox withEventsField_CheckNotAddToAtReply;
		internal System.Windows.Forms.CheckBox CheckNotAddToAtReply {
			get { return withEventsField_CheckNotAddToAtReply; }
			set {
				if (withEventsField_CheckNotAddToAtReply != null) {
					withEventsField_CheckNotAddToAtReply.CheckedChanged -= CheckNotAddToAtReply_CheckedChanged;
				}
				withEventsField_CheckNotAddToAtReply = value;
				if (withEventsField_CheckNotAddToAtReply != null) {
					withEventsField_CheckNotAddToAtReply.CheckedChanged += CheckNotAddToAtReply_CheckedChanged;
				}
			}

		}
	}
}
