using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class ListAvailable : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListAvailable));
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.UsernameLabel = new System.Windows.Forms.Label();
			this.NameLabel = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.StatusLabel = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.MemberCountLabel = new System.Windows.Forms.Label();
			this.Label10 = new System.Windows.Forms.Label();
			this.SubscriberCountLabel = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.DescriptionText = new System.Windows.Forms.TextBox();
			this.RefreshButton = new System.Windows.Forms.Button();
			this.ListsList = new System.Windows.Forms.ListBox();
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
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
			this.OK_Button.Name = "OK_Button";
			//
			//Cancel_Button
			//
			resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Name = "Cancel_Button";
			//
			//Label1
			//
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Name = "Label1";
			//
			//UsernameLabel
			//
			this.UsernameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.UsernameLabel, "UsernameLabel");
			this.UsernameLabel.Name = "UsernameLabel";
			//
			//NameLabel
			//
			this.NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.NameLabel, "NameLabel");
			this.NameLabel.Name = "NameLabel";
			//
			//Label4
			//
			resources.ApplyResources(this.Label4, "Label4");
			this.Label4.Name = "Label4";
			//
			//Label6
			//
			resources.ApplyResources(this.Label6, "Label6");
			this.Label6.Name = "Label6";
			//
			//StatusLabel
			//
			this.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.StatusLabel, "StatusLabel");
			this.StatusLabel.Name = "StatusLabel";
			//
			//Label8
			//
			resources.ApplyResources(this.Label8, "Label8");
			this.Label8.Name = "Label8";
			//
			//MemberCountLabel
			//
			this.MemberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.MemberCountLabel, "MemberCountLabel");
			this.MemberCountLabel.Name = "MemberCountLabel";
			//
			//Label10
			//
			resources.ApplyResources(this.Label10, "Label10");
			this.Label10.Name = "Label10";
			//
			//SubscriberCountLabel
			//
			this.SubscriberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.SubscriberCountLabel, "SubscriberCountLabel");
			this.SubscriberCountLabel.Name = "SubscriberCountLabel";
			//
			//Label12
			//
			resources.ApplyResources(this.Label12, "Label12");
			this.Label12.Name = "Label12";
			//
			//DescriptionText
			//
			resources.ApplyResources(this.DescriptionText, "DescriptionText");
			this.DescriptionText.Name = "DescriptionText";
			this.DescriptionText.ReadOnly = true;
			this.DescriptionText.TabStop = false;
			//
			//RefreshButton
			//
			resources.ApplyResources(this.RefreshButton, "RefreshButton");
			this.RefreshButton.Name = "RefreshButton";
			//
			//ListsList
			//
			this.ListsList.FormattingEnabled = true;
			resources.ApplyResources(this.ListsList, "ListsList");
			this.ListsList.Name = "ListsList";
			//
			//ListAvailable
			//
			this.AcceptButton = this.OK_Button;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel_Button;
			this.Controls.Add(this.ListsList);
			this.Controls.Add(this.RefreshButton);
			this.Controls.Add(this.DescriptionText);
			this.Controls.Add(this.SubscriberCountLabel);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.MemberCountLabel);
			this.Controls.Add(this.Label10);
			this.Controls.Add(this.StatusLabel);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.Label6);
			this.Controls.Add(this.NameLabel);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.UsernameLabel);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.TableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ListAvailable";
			this.ShowInTaskbar = false;
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
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label UsernameLabel;
		internal System.Windows.Forms.Label NameLabel;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label StatusLabel;
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.Label MemberCountLabel;
		internal System.Windows.Forms.Label Label10;
		internal System.Windows.Forms.Label SubscriberCountLabel;
		internal System.Windows.Forms.Label Label12;
		internal System.Windows.Forms.TextBox DescriptionText;
		private System.Windows.Forms.Button withEventsField_RefreshButton;
		internal System.Windows.Forms.Button RefreshButton {
			get { return withEventsField_RefreshButton; }
			set {
				if (withEventsField_RefreshButton != null) {
					withEventsField_RefreshButton.Click -= RefreshButton_Click;
				}
				withEventsField_RefreshButton = value;
				if (withEventsField_RefreshButton != null) {
					withEventsField_RefreshButton.Click += RefreshButton_Click;
				}
			}
		}
		private System.Windows.Forms.ListBox withEventsField_ListsList;
		internal System.Windows.Forms.ListBox ListsList {
			get { return withEventsField_ListsList; }
			set {
				if (withEventsField_ListsList != null) {
					withEventsField_ListsList.SelectedIndexChanged -= ListsList_SelectedIndexChanged;
				}
				withEventsField_ListsList = value;
				if (withEventsField_ListsList != null) {
					withEventsField_ListsList.SelectedIndexChanged += ListsList_SelectedIndexChanged;
				}
			}

		}
	}
}
