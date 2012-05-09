using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class FilterDialog : System.Windows.Forms.Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterDialog));
			this.ButtonClose = new System.Windows.Forms.Button();
			this.ListFilters = new System.Windows.Forms.ListBox();
			this.EditFilterGroup = new System.Windows.Forms.GroupBox();
			this.Label11 = new System.Windows.Forms.Label();
			this.GroupExclude = new System.Windows.Forms.GroupBox();
			this.CheckExLambDa = new System.Windows.Forms.CheckBox();
			this.TextExSource = new System.Windows.Forms.TextBox();
			this.Label12 = new System.Windows.Forms.Label();
			this.CheckExRetweet = new System.Windows.Forms.CheckBox();
			this.CheckExCaseSensitive = new System.Windows.Forms.CheckBox();
			this.RadioExAnd = new System.Windows.Forms.RadioButton();
			this.Label1 = new System.Windows.Forms.Label();
			this.CheckExURL = new System.Windows.Forms.CheckBox();
			this.RadioExPLUS = new System.Windows.Forms.RadioButton();
			this.CheckExRegex = new System.Windows.Forms.CheckBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.ExUID = new System.Windows.Forms.TextBox();
			this.ExMSG1 = new System.Windows.Forms.TextBox();
			this.ExMSG2 = new System.Windows.Forms.TextBox();
			this.GroupMatch = new System.Windows.Forms.GroupBox();
			this.CheckLambda = new System.Windows.Forms.CheckBox();
			this.TextSource = new System.Windows.Forms.TextBox();
			this.Label5 = new System.Windows.Forms.Label();
			this.CheckRetweet = new System.Windows.Forms.CheckBox();
			this.CheckCaseSensitive = new System.Windows.Forms.CheckBox();
			this.RadioAND = new System.Windows.Forms.RadioButton();
			this.Label8 = new System.Windows.Forms.Label();
			this.CheckURL = new System.Windows.Forms.CheckBox();
			this.RadioPLUS = new System.Windows.Forms.RadioButton();
			this.CheckRegex = new System.Windows.Forms.CheckBox();
			this.Label9 = new System.Windows.Forms.Label();
			this.Label7 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.UID = new System.Windows.Forms.TextBox();
			this.MSG1 = new System.Windows.Forms.TextBox();
			this.MSG2 = new System.Windows.Forms.TextBox();
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.CheckMark = new System.Windows.Forms.CheckBox();
			this.OptCopy = new System.Windows.Forms.RadioButton();
			this.OptMove = new System.Windows.Forms.RadioButton();
			this.ButtonCancel = new System.Windows.Forms.Button();
			this.ButtonOK = new System.Windows.Forms.Button();
			this.ButtonNew = new System.Windows.Forms.Button();
			this.ButtonDelete = new System.Windows.Forms.Button();
			this.ButtonEdit = new System.Windows.Forms.Button();
			this.GroupBox2 = new System.Windows.Forms.GroupBox();
			this.ButtonRuleMove = new System.Windows.Forms.Button();
			this.ButtonRuleCopy = new System.Windows.Forms.Button();
			this.ButtonRuleDown = new System.Windows.Forms.Button();
			this.ButtonRuleUp = new System.Windows.Forms.Button();
			this.ListTabs = new System.Windows.Forms.ListBox();
			this.ButtonAddTab = new System.Windows.Forms.Button();
			this.ButtonDeleteTab = new System.Windows.Forms.Button();
			this.ButtonRenameTab = new System.Windows.Forms.Button();
			this.CheckManageRead = new System.Windows.Forms.CheckBox();
			this.CheckNotifyNew = new System.Windows.Forms.CheckBox();
			this.ComboSound = new System.Windows.Forms.ComboBox();
			this.Label10 = new System.Windows.Forms.Label();
			this.ButtonUp = new System.Windows.Forms.Button();
			this.ButtonDown = new System.Windows.Forms.Button();
			this.GroupTab = new System.Windows.Forms.GroupBox();
			this.LabelTabType = new System.Windows.Forms.Label();
			this.Label13 = new System.Windows.Forms.Label();
			this.EditFilterGroup.SuspendLayout();
			this.GroupExclude.SuspendLayout();
			this.GroupMatch.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.GroupBox2.SuspendLayout();
			this.GroupTab.SuspendLayout();
			this.SuspendLayout();
			//
			//ButtonClose
			//
			resources.ApplyResources(this.ButtonClose, "ButtonClose");
			this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ButtonClose.Name = "ButtonClose";
			this.ButtonClose.UseVisualStyleBackColor = true;
			//
			//ListFilters
			//
			resources.ApplyResources(this.ListFilters, "ListFilters");
			this.ListFilters.FormattingEnabled = true;
			this.ListFilters.Name = "ListFilters";
			this.ListFilters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			//
			//EditFilterGroup
			//
			resources.ApplyResources(this.EditFilterGroup, "EditFilterGroup");
			this.EditFilterGroup.Controls.Add(this.Label11);
			this.EditFilterGroup.Controls.Add(this.GroupExclude);
			this.EditFilterGroup.Controls.Add(this.GroupMatch);
			this.EditFilterGroup.Controls.Add(this.GroupBox1);
			this.EditFilterGroup.Controls.Add(this.ButtonCancel);
			this.EditFilterGroup.Controls.Add(this.ButtonOK);
			this.EditFilterGroup.Name = "EditFilterGroup";
			this.EditFilterGroup.TabStop = false;
			//
			//Label11
			//
			resources.ApplyResources(this.Label11, "Label11");
			this.Label11.Name = "Label11";
			//
			//GroupExclude
			//
			resources.ApplyResources(this.GroupExclude, "GroupExclude");
			this.GroupExclude.Controls.Add(this.CheckExLambDa);
			this.GroupExclude.Controls.Add(this.TextExSource);
			this.GroupExclude.Controls.Add(this.Label12);
			this.GroupExclude.Controls.Add(this.CheckExRetweet);
			this.GroupExclude.Controls.Add(this.CheckExCaseSensitive);
			this.GroupExclude.Controls.Add(this.RadioExAnd);
			this.GroupExclude.Controls.Add(this.Label1);
			this.GroupExclude.Controls.Add(this.CheckExURL);
			this.GroupExclude.Controls.Add(this.RadioExPLUS);
			this.GroupExclude.Controls.Add(this.CheckExRegex);
			this.GroupExclude.Controls.Add(this.Label2);
			this.GroupExclude.Controls.Add(this.Label3);
			this.GroupExclude.Controls.Add(this.Label4);
			this.GroupExclude.Controls.Add(this.ExUID);
			this.GroupExclude.Controls.Add(this.ExMSG1);
			this.GroupExclude.Controls.Add(this.ExMSG2);
			this.GroupExclude.Name = "GroupExclude";
			this.GroupExclude.TabStop = false;
			//
			//CheckExLambDa
			//
			resources.ApplyResources(this.CheckExLambDa, "CheckExLambDa");
			this.CheckExLambDa.Name = "CheckExLambDa";
			this.CheckExLambDa.UseVisualStyleBackColor = true;
			//
			//TextExSource
			//
			resources.ApplyResources(this.TextExSource, "TextExSource");
			this.TextExSource.Name = "TextExSource";
			//
			//Label12
			//
			resources.ApplyResources(this.Label12, "Label12");
			this.Label12.Name = "Label12";
			//
			//CheckExRetweet
			//
			resources.ApplyResources(this.CheckExRetweet, "CheckExRetweet");
			this.CheckExRetweet.Name = "CheckExRetweet";
			this.CheckExRetweet.UseVisualStyleBackColor = true;
			//
			//CheckExCaseSensitive
			//
			resources.ApplyResources(this.CheckExCaseSensitive, "CheckExCaseSensitive");
			this.CheckExCaseSensitive.Name = "CheckExCaseSensitive";
			this.CheckExCaseSensitive.UseVisualStyleBackColor = true;
			//
			//RadioExAnd
			//
			resources.ApplyResources(this.RadioExAnd, "RadioExAnd");
			this.RadioExAnd.Checked = true;
			this.RadioExAnd.Name = "RadioExAnd";
			this.RadioExAnd.TabStop = true;
			this.RadioExAnd.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Name = "Label1";
			//
			//CheckExURL
			//
			resources.ApplyResources(this.CheckExURL, "CheckExURL");
			this.CheckExURL.Name = "CheckExURL";
			this.CheckExURL.UseVisualStyleBackColor = true;
			//
			//RadioExPLUS
			//
			resources.ApplyResources(this.RadioExPLUS, "RadioExPLUS");
			this.RadioExPLUS.Name = "RadioExPLUS";
			this.RadioExPLUS.UseVisualStyleBackColor = true;
			//
			//CheckExRegex
			//
			resources.ApplyResources(this.CheckExRegex, "CheckExRegex");
			this.CheckExRegex.Name = "CheckExRegex";
			this.CheckExRegex.UseVisualStyleBackColor = true;
			//
			//Label2
			//
			resources.ApplyResources(this.Label2, "Label2");
			this.Label2.Name = "Label2";
			//
			//Label3
			//
			resources.ApplyResources(this.Label3, "Label3");
			this.Label3.Name = "Label3";
			//
			//Label4
			//
			resources.ApplyResources(this.Label4, "Label4");
			this.Label4.Name = "Label4";
			//
			//ExUID
			//
			resources.ApplyResources(this.ExUID, "ExUID");
			this.ExUID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ExUID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.ExUID.Name = "ExUID";
			//
			//ExMSG1
			//
			resources.ApplyResources(this.ExMSG1, "ExMSG1");
			this.ExMSG1.Name = "ExMSG1";
			//
			//ExMSG2
			//
			resources.ApplyResources(this.ExMSG2, "ExMSG2");
			this.ExMSG2.Name = "ExMSG2";
			//
			//GroupMatch
			//
			resources.ApplyResources(this.GroupMatch, "GroupMatch");
			this.GroupMatch.Controls.Add(this.CheckLambda);
			this.GroupMatch.Controls.Add(this.TextSource);
			this.GroupMatch.Controls.Add(this.Label5);
			this.GroupMatch.Controls.Add(this.CheckRetweet);
			this.GroupMatch.Controls.Add(this.CheckCaseSensitive);
			this.GroupMatch.Controls.Add(this.RadioAND);
			this.GroupMatch.Controls.Add(this.Label8);
			this.GroupMatch.Controls.Add(this.CheckURL);
			this.GroupMatch.Controls.Add(this.RadioPLUS);
			this.GroupMatch.Controls.Add(this.CheckRegex);
			this.GroupMatch.Controls.Add(this.Label9);
			this.GroupMatch.Controls.Add(this.Label7);
			this.GroupMatch.Controls.Add(this.Label6);
			this.GroupMatch.Controls.Add(this.UID);
			this.GroupMatch.Controls.Add(this.MSG1);
			this.GroupMatch.Controls.Add(this.MSG2);
			this.GroupMatch.Name = "GroupMatch";
			this.GroupMatch.TabStop = false;
			//
			//CheckLambda
			//
			resources.ApplyResources(this.CheckLambda, "CheckLambda");
			this.CheckLambda.Name = "CheckLambda";
			this.CheckLambda.UseVisualStyleBackColor = true;
			//
			//TextSource
			//
			resources.ApplyResources(this.TextSource, "TextSource");
			this.TextSource.Name = "TextSource";
			//
			//Label5
			//
			resources.ApplyResources(this.Label5, "Label5");
			this.Label5.Name = "Label5";
			//
			//CheckRetweet
			//
			resources.ApplyResources(this.CheckRetweet, "CheckRetweet");
			this.CheckRetweet.Name = "CheckRetweet";
			this.CheckRetweet.UseVisualStyleBackColor = true;
			//
			//CheckCaseSensitive
			//
			resources.ApplyResources(this.CheckCaseSensitive, "CheckCaseSensitive");
			this.CheckCaseSensitive.Name = "CheckCaseSensitive";
			this.CheckCaseSensitive.UseVisualStyleBackColor = true;
			//
			//RadioAND
			//
			resources.ApplyResources(this.RadioAND, "RadioAND");
			this.RadioAND.Checked = true;
			this.RadioAND.Name = "RadioAND";
			this.RadioAND.TabStop = true;
			this.RadioAND.UseVisualStyleBackColor = true;
			//
			//Label8
			//
			resources.ApplyResources(this.Label8, "Label8");
			this.Label8.Name = "Label8";
			//
			//CheckURL
			//
			resources.ApplyResources(this.CheckURL, "CheckURL");
			this.CheckURL.Name = "CheckURL";
			this.CheckURL.UseVisualStyleBackColor = true;
			//
			//RadioPLUS
			//
			resources.ApplyResources(this.RadioPLUS, "RadioPLUS");
			this.RadioPLUS.Name = "RadioPLUS";
			this.RadioPLUS.UseVisualStyleBackColor = true;
			//
			//CheckRegex
			//
			resources.ApplyResources(this.CheckRegex, "CheckRegex");
			this.CheckRegex.Name = "CheckRegex";
			this.CheckRegex.UseVisualStyleBackColor = true;
			//
			//Label9
			//
			resources.ApplyResources(this.Label9, "Label9");
			this.Label9.Name = "Label9";
			//
			//Label7
			//
			resources.ApplyResources(this.Label7, "Label7");
			this.Label7.Name = "Label7";
			//
			//Label6
			//
			resources.ApplyResources(this.Label6, "Label6");
			this.Label6.Name = "Label6";
			//
			//UID
			//
			resources.ApplyResources(this.UID, "UID");
			this.UID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.UID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.UID.Name = "UID";
			//
			//MSG1
			//
			resources.ApplyResources(this.MSG1, "MSG1");
			this.MSG1.Name = "MSG1";
			//
			//MSG2
			//
			resources.ApplyResources(this.MSG2, "MSG2");
			this.MSG2.Name = "MSG2";
			//
			//GroupBox1
			//
			resources.ApplyResources(this.GroupBox1, "GroupBox1");
			this.GroupBox1.Controls.Add(this.CheckMark);
			this.GroupBox1.Controls.Add(this.OptCopy);
			this.GroupBox1.Controls.Add(this.OptMove);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.TabStop = false;
			//
			//CheckMark
			//
			resources.ApplyResources(this.CheckMark, "CheckMark");
			this.CheckMark.Name = "CheckMark";
			this.CheckMark.UseVisualStyleBackColor = true;
			//
			//OptCopy
			//
			resources.ApplyResources(this.OptCopy, "OptCopy");
			this.OptCopy.Name = "OptCopy";
			this.OptCopy.TabStop = true;
			this.OptCopy.UseVisualStyleBackColor = true;
			//
			//OptMove
			//
			resources.ApplyResources(this.OptMove, "OptMove");
			this.OptMove.Name = "OptMove";
			this.OptMove.TabStop = true;
			this.OptMove.UseVisualStyleBackColor = true;
			//
			//ButtonCancel
			//
			resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
			this.ButtonCancel.Name = "ButtonCancel";
			this.ButtonCancel.UseVisualStyleBackColor = true;
			//
			//ButtonOK
			//
			resources.ApplyResources(this.ButtonOK, "ButtonOK");
			this.ButtonOK.Name = "ButtonOK";
			this.ButtonOK.UseVisualStyleBackColor = true;
			//
			//ButtonNew
			//
			resources.ApplyResources(this.ButtonNew, "ButtonNew");
			this.ButtonNew.Name = "ButtonNew";
			this.ButtonNew.UseVisualStyleBackColor = true;
			//
			//ButtonDelete
			//
			resources.ApplyResources(this.ButtonDelete, "ButtonDelete");
			this.ButtonDelete.Name = "ButtonDelete";
			this.ButtonDelete.UseVisualStyleBackColor = true;
			//
			//ButtonEdit
			//
			resources.ApplyResources(this.ButtonEdit, "ButtonEdit");
			this.ButtonEdit.Name = "ButtonEdit";
			this.ButtonEdit.UseVisualStyleBackColor = true;
			//
			//GroupBox2
			//
			resources.ApplyResources(this.GroupBox2, "GroupBox2");
			this.GroupBox2.Controls.Add(this.ButtonRuleMove);
			this.GroupBox2.Controls.Add(this.ButtonRuleCopy);
			this.GroupBox2.Controls.Add(this.ButtonRuleDown);
			this.GroupBox2.Controls.Add(this.ButtonRuleUp);
			this.GroupBox2.Controls.Add(this.ListFilters);
			this.GroupBox2.Controls.Add(this.ButtonEdit);
			this.GroupBox2.Controls.Add(this.ButtonDelete);
			this.GroupBox2.Controls.Add(this.ButtonNew);
			this.GroupBox2.Controls.Add(this.EditFilterGroup);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.TabStop = false;
			//
			//ButtonRuleMove
			//
			resources.ApplyResources(this.ButtonRuleMove, "ButtonRuleMove");
			this.ButtonRuleMove.Name = "ButtonRuleMove";
			this.ButtonRuleMove.UseVisualStyleBackColor = true;
			//
			//ButtonRuleCopy
			//
			resources.ApplyResources(this.ButtonRuleCopy, "ButtonRuleCopy");
			this.ButtonRuleCopy.Name = "ButtonRuleCopy";
			this.ButtonRuleCopy.UseVisualStyleBackColor = true;
			//
			//ButtonRuleDown
			//
			resources.ApplyResources(this.ButtonRuleDown, "ButtonRuleDown");
			this.ButtonRuleDown.Name = "ButtonRuleDown";
			this.ButtonRuleDown.UseVisualStyleBackColor = true;
			//
			//ButtonRuleUp
			//
			resources.ApplyResources(this.ButtonRuleUp, "ButtonRuleUp");
			this.ButtonRuleUp.Name = "ButtonRuleUp";
			this.ButtonRuleUp.UseVisualStyleBackColor = true;
			//
			//ListTabs
			//
			resources.ApplyResources(this.ListTabs, "ListTabs");
			this.ListTabs.FormattingEnabled = true;
			this.ListTabs.Name = "ListTabs";
			//
			//ButtonAddTab
			//
			resources.ApplyResources(this.ButtonAddTab, "ButtonAddTab");
			this.ButtonAddTab.Name = "ButtonAddTab";
			this.ButtonAddTab.UseVisualStyleBackColor = true;
			//
			//ButtonDeleteTab
			//
			resources.ApplyResources(this.ButtonDeleteTab, "ButtonDeleteTab");
			this.ButtonDeleteTab.Name = "ButtonDeleteTab";
			this.ButtonDeleteTab.UseVisualStyleBackColor = true;
			//
			//ButtonRenameTab
			//
			resources.ApplyResources(this.ButtonRenameTab, "ButtonRenameTab");
			this.ButtonRenameTab.Name = "ButtonRenameTab";
			this.ButtonRenameTab.UseVisualStyleBackColor = true;
			//
			//CheckManageRead
			//
			resources.ApplyResources(this.CheckManageRead, "CheckManageRead");
			this.CheckManageRead.Name = "CheckManageRead";
			this.CheckManageRead.UseVisualStyleBackColor = true;
			//
			//CheckNotifyNew
			//
			resources.ApplyResources(this.CheckNotifyNew, "CheckNotifyNew");
			this.CheckNotifyNew.Name = "CheckNotifyNew";
			this.CheckNotifyNew.UseVisualStyleBackColor = true;
			//
			//ComboSound
			//
			resources.ApplyResources(this.ComboSound, "ComboSound");
			this.ComboSound.FormattingEnabled = true;
			this.ComboSound.Name = "ComboSound";
			//
			//Label10
			//
			resources.ApplyResources(this.Label10, "Label10");
			this.Label10.Name = "Label10";
			//
			//ButtonUp
			//
			resources.ApplyResources(this.ButtonUp, "ButtonUp");
			this.ButtonUp.Name = "ButtonUp";
			this.ButtonUp.UseVisualStyleBackColor = true;
			//
			//ButtonDown
			//
			resources.ApplyResources(this.ButtonDown, "ButtonDown");
			this.ButtonDown.Name = "ButtonDown";
			this.ButtonDown.UseVisualStyleBackColor = true;
			//
			//GroupTab
			//
			resources.ApplyResources(this.GroupTab, "GroupTab");
			this.GroupTab.Controls.Add(this.LabelTabType);
			this.GroupTab.Controls.Add(this.Label13);
			this.GroupTab.Controls.Add(this.ListTabs);
			this.GroupTab.Controls.Add(this.ButtonDown);
			this.GroupTab.Controls.Add(this.ButtonAddTab);
			this.GroupTab.Controls.Add(this.ButtonUp);
			this.GroupTab.Controls.Add(this.ButtonDeleteTab);
			this.GroupTab.Controls.Add(this.Label10);
			this.GroupTab.Controls.Add(this.ButtonRenameTab);
			this.GroupTab.Controls.Add(this.ComboSound);
			this.GroupTab.Controls.Add(this.CheckManageRead);
			this.GroupTab.Controls.Add(this.CheckNotifyNew);
			this.GroupTab.Name = "GroupTab";
			this.GroupTab.TabStop = false;
			//
			//LabelTabType
			//
			resources.ApplyResources(this.LabelTabType, "LabelTabType");
			this.LabelTabType.Name = "LabelTabType";
			//
			//Label13
			//
			resources.ApplyResources(this.Label13, "Label13");
			this.Label13.Name = "Label13";
			//
			//FilterDialog
			//
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.ButtonClose;
			this.ControlBox = false;
			this.Controls.Add(this.GroupTab);
			this.Controls.Add(this.GroupBox2);
			this.Controls.Add(this.ButtonClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterDialog";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.EditFilterGroup.ResumeLayout(false);
			this.GroupExclude.ResumeLayout(false);
			this.GroupExclude.PerformLayout();
			this.GroupMatch.ResumeLayout(false);
			this.GroupMatch.PerformLayout();
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			this.GroupBox2.ResumeLayout(false);
			this.GroupTab.ResumeLayout(false);
			this.GroupTab.PerformLayout();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button withEventsField_ButtonClose;
		internal System.Windows.Forms.Button ButtonClose {
			get { return withEventsField_ButtonClose; }
			set {
				if (withEventsField_ButtonClose != null) {
					withEventsField_ButtonClose.Click -= ButtonClose_Click;
				}
				withEventsField_ButtonClose = value;
				if (withEventsField_ButtonClose != null) {
					withEventsField_ButtonClose.Click += ButtonClose_Click;
				}
			}
		}
		private System.Windows.Forms.ListBox withEventsField_ListFilters;
		internal System.Windows.Forms.ListBox ListFilters {
			get { return withEventsField_ListFilters; }
			set {
				if (withEventsField_ListFilters != null) {
					withEventsField_ListFilters.SelectedIndexChanged -= ListFilters_SelectedIndexChanged;
					withEventsField_ListFilters.DoubleClick -= ListFilters_DoubleClick;
				}
				withEventsField_ListFilters = value;
				if (withEventsField_ListFilters != null) {
					withEventsField_ListFilters.SelectedIndexChanged += ListFilters_SelectedIndexChanged;
					withEventsField_ListFilters.DoubleClick += ListFilters_DoubleClick;
				}
			}
		}
		internal System.Windows.Forms.GroupBox EditFilterGroup;
		internal System.Windows.Forms.RadioButton RadioPLUS;
		private System.Windows.Forms.RadioButton withEventsField_RadioAND;
		internal System.Windows.Forms.RadioButton RadioAND {
			get { return withEventsField_RadioAND; }
			set {
				if (withEventsField_RadioAND != null) {
					withEventsField_RadioAND.CheckedChanged -= RadioAND_CheckedChanged;
				}
				withEventsField_RadioAND = value;
				if (withEventsField_RadioAND != null) {
					withEventsField_RadioAND.CheckedChanged += RadioAND_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.TextBox withEventsField_MSG2;
		internal System.Windows.Forms.TextBox MSG2 {
			get { return withEventsField_MSG2; }
			set {
				if (withEventsField_MSG2 != null) {
					withEventsField_MSG2.KeyDown -= FilterTextBox_KeyDown;
					withEventsField_MSG2.KeyPress -= FilterTextBox_KeyPress;
				}
				withEventsField_MSG2 = value;
				if (withEventsField_MSG2 != null) {
					withEventsField_MSG2.KeyDown += FilterTextBox_KeyDown;
					withEventsField_MSG2.KeyPress += FilterTextBox_KeyPress;
				}
			}
		}
		internal System.Windows.Forms.Label Label9;
		private System.Windows.Forms.TextBox withEventsField_MSG1;
		internal System.Windows.Forms.TextBox MSG1 {
			get { return withEventsField_MSG1; }
			set {
				if (withEventsField_MSG1 != null) {
					withEventsField_MSG1.KeyDown -= FilterTextBox_KeyDown;
					withEventsField_MSG1.KeyPress -= FilterTextBox_KeyPress;
				}
				withEventsField_MSG1 = value;
				if (withEventsField_MSG1 != null) {
					withEventsField_MSG1.KeyDown += FilterTextBox_KeyDown;
					withEventsField_MSG1.KeyPress += FilterTextBox_KeyPress;
				}
			}
		}
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Button withEventsField_ButtonCancel;
		internal System.Windows.Forms.Button ButtonCancel {
			get { return withEventsField_ButtonCancel; }
			set {
				if (withEventsField_ButtonCancel != null) {
					withEventsField_ButtonCancel.Click -= ButtonCancel_Click;
				}
				withEventsField_ButtonCancel = value;
				if (withEventsField_ButtonCancel != null) {
					withEventsField_ButtonCancel.Click += ButtonCancel_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonOK;
		internal System.Windows.Forms.Button ButtonOK {
			get { return withEventsField_ButtonOK; }
			set {
				if (withEventsField_ButtonOK != null) {
					withEventsField_ButtonOK.Click -= ButtonOK_Click;
				}
				withEventsField_ButtonOK = value;
				if (withEventsField_ButtonOK != null) {
					withEventsField_ButtonOK.Click += ButtonOK_Click;
				}
			}
		}
		internal System.Windows.Forms.TextBox UID;
		internal System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Button withEventsField_ButtonNew;
		internal System.Windows.Forms.Button ButtonNew {
			get { return withEventsField_ButtonNew; }
			set {
				if (withEventsField_ButtonNew != null) {
					withEventsField_ButtonNew.Click -= ButtonNew_Click;
				}
				withEventsField_ButtonNew = value;
				if (withEventsField_ButtonNew != null) {
					withEventsField_ButtonNew.Click += ButtonNew_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonDelete;
		internal System.Windows.Forms.Button ButtonDelete {
			get { return withEventsField_ButtonDelete; }
			set {
				if (withEventsField_ButtonDelete != null) {
					withEventsField_ButtonDelete.Click -= ButtonDelete_Click;
				}
				withEventsField_ButtonDelete = value;
				if (withEventsField_ButtonDelete != null) {
					withEventsField_ButtonDelete.Click += ButtonDelete_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonEdit;
		internal System.Windows.Forms.Button ButtonEdit {
			get { return withEventsField_ButtonEdit; }
			set {
				if (withEventsField_ButtonEdit != null) {
					withEventsField_ButtonEdit.Click -= ButtonEdit_Click;
				}
				withEventsField_ButtonEdit = value;
				if (withEventsField_ButtonEdit != null) {
					withEventsField_ButtonEdit.Click += ButtonEdit_Click;
				}
			}
		}
		internal System.Windows.Forms.CheckBox CheckURL;
		internal System.Windows.Forms.CheckBox CheckRegex;
		internal System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton withEventsField_OptMove;
		internal System.Windows.Forms.RadioButton OptMove {
			get { return withEventsField_OptMove; }
			set {
				if (withEventsField_OptMove != null) {
					withEventsField_OptMove.CheckedChanged -= OptMove_CheckedChanged;
				}
				withEventsField_OptMove = value;
				if (withEventsField_OptMove != null) {
					withEventsField_OptMove.CheckedChanged += OptMove_CheckedChanged;
				}
			}
		}
		internal System.Windows.Forms.RadioButton OptCopy;
		internal System.Windows.Forms.GroupBox GroupBox2;
		private System.Windows.Forms.ListBox withEventsField_ListTabs;
		internal System.Windows.Forms.ListBox ListTabs {
			get { return withEventsField_ListTabs; }
			set {
				if (withEventsField_ListTabs != null) {
					withEventsField_ListTabs.SelectedIndexChanged -= ListTabs_SelectedIndexChanged;
				}
				withEventsField_ListTabs = value;
				if (withEventsField_ListTabs != null) {
					withEventsField_ListTabs.SelectedIndexChanged += ListTabs_SelectedIndexChanged;
				}
			}
		}
		internal System.Windows.Forms.GroupBox GroupMatch;
		internal System.Windows.Forms.GroupBox GroupExclude;
		private System.Windows.Forms.RadioButton withEventsField_RadioExAnd;
		internal System.Windows.Forms.RadioButton RadioExAnd {
			get { return withEventsField_RadioExAnd; }
			set {
				if (withEventsField_RadioExAnd != null) {
					withEventsField_RadioExAnd.CheckedChanged -= RadioExAnd_CheckedChanged;
				}
				withEventsField_RadioExAnd = value;
				if (withEventsField_RadioExAnd != null) {
					withEventsField_RadioExAnd.CheckedChanged += RadioExAnd_CheckedChanged;
				}
			}
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.CheckBox CheckExURL;
		internal System.Windows.Forms.RadioButton RadioExPLUS;
		internal System.Windows.Forms.CheckBox CheckExRegex;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.TextBox ExUID;
		private System.Windows.Forms.TextBox withEventsField_ExMSG1;
		internal System.Windows.Forms.TextBox ExMSG1 {
			get { return withEventsField_ExMSG1; }
			set {
				if (withEventsField_ExMSG1 != null) {
					withEventsField_ExMSG1.KeyDown -= FilterTextBox_KeyDown;
					withEventsField_ExMSG1.KeyPress -= FilterTextBox_KeyPress;
				}
				withEventsField_ExMSG1 = value;
				if (withEventsField_ExMSG1 != null) {
					withEventsField_ExMSG1.KeyDown += FilterTextBox_KeyDown;
					withEventsField_ExMSG1.KeyPress += FilterTextBox_KeyPress;
				}
			}
		}
		private System.Windows.Forms.TextBox withEventsField_ExMSG2;
		internal System.Windows.Forms.TextBox ExMSG2 {
			get { return withEventsField_ExMSG2; }
			set {
				if (withEventsField_ExMSG2 != null) {
					withEventsField_ExMSG2.KeyDown -= FilterTextBox_KeyDown;
					withEventsField_ExMSG2.KeyPress -= FilterTextBox_KeyPress;
				}
				withEventsField_ExMSG2 = value;
				if (withEventsField_ExMSG2 != null) {
					withEventsField_ExMSG2.KeyDown += FilterTextBox_KeyDown;
					withEventsField_ExMSG2.KeyPress += FilterTextBox_KeyPress;
				}
			}
		}
		internal System.Windows.Forms.CheckBox CheckMark;
		private System.Windows.Forms.Button withEventsField_ButtonAddTab;
		internal System.Windows.Forms.Button ButtonAddTab {
			get { return withEventsField_ButtonAddTab; }
			set {
				if (withEventsField_ButtonAddTab != null) {
					withEventsField_ButtonAddTab.Click -= ButtonAddTab_Click;
				}
				withEventsField_ButtonAddTab = value;
				if (withEventsField_ButtonAddTab != null) {
					withEventsField_ButtonAddTab.Click += ButtonAddTab_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonDeleteTab;
		internal System.Windows.Forms.Button ButtonDeleteTab {
			get { return withEventsField_ButtonDeleteTab; }
			set {
				if (withEventsField_ButtonDeleteTab != null) {
					withEventsField_ButtonDeleteTab.Click -= ButtonDeleteTab_Click;
				}
				withEventsField_ButtonDeleteTab = value;
				if (withEventsField_ButtonDeleteTab != null) {
					withEventsField_ButtonDeleteTab.Click += ButtonDeleteTab_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonRenameTab;
		internal System.Windows.Forms.Button ButtonRenameTab {
			get { return withEventsField_ButtonRenameTab; }
			set {
				if (withEventsField_ButtonRenameTab != null) {
					withEventsField_ButtonRenameTab.Click -= ButtonRenameTab_Click;
				}
				withEventsField_ButtonRenameTab = value;
				if (withEventsField_ButtonRenameTab != null) {
					withEventsField_ButtonRenameTab.Click += ButtonRenameTab_Click;
				}
			}
		}
		private System.Windows.Forms.CheckBox withEventsField_CheckManageRead;
		internal System.Windows.Forms.CheckBox CheckManageRead {
			get { return withEventsField_CheckManageRead; }
			set {
				if (withEventsField_CheckManageRead != null) {
					withEventsField_CheckManageRead.CheckedChanged -= CheckManageRead_CheckedChanged;
				}
				withEventsField_CheckManageRead = value;
				if (withEventsField_CheckManageRead != null) {
					withEventsField_CheckManageRead.CheckedChanged += CheckManageRead_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.CheckBox withEventsField_CheckNotifyNew;
		internal System.Windows.Forms.CheckBox CheckNotifyNew {
			get { return withEventsField_CheckNotifyNew; }
			set {
				if (withEventsField_CheckNotifyNew != null) {
					withEventsField_CheckNotifyNew.CheckedChanged -= CheckNotifyNew_CheckedChanged;
				}
				withEventsField_CheckNotifyNew = value;
				if (withEventsField_CheckNotifyNew != null) {
					withEventsField_CheckNotifyNew.CheckedChanged += CheckNotifyNew_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.ComboBox withEventsField_ComboSound;
		internal System.Windows.Forms.ComboBox ComboSound {
			get { return withEventsField_ComboSound; }
			set {
				if (withEventsField_ComboSound != null) {
					withEventsField_ComboSound.SelectedIndexChanged -= ComboSound_SelectedIndexChanged;
				}
				withEventsField_ComboSound = value;
				if (withEventsField_ComboSound != null) {
					withEventsField_ComboSound.SelectedIndexChanged += ComboSound_SelectedIndexChanged;
				}
			}
		}
		internal System.Windows.Forms.Label Label10;
		internal System.Windows.Forms.Label Label11;
		private System.Windows.Forms.Button withEventsField_ButtonUp;
		internal System.Windows.Forms.Button ButtonUp {
			get { return withEventsField_ButtonUp; }
			set {
				if (withEventsField_ButtonUp != null) {
					withEventsField_ButtonUp.Click -= ButtonUp_Click;
				}
				withEventsField_ButtonUp = value;
				if (withEventsField_ButtonUp != null) {
					withEventsField_ButtonUp.Click += ButtonUp_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonDown;
		internal System.Windows.Forms.Button ButtonDown {
			get { return withEventsField_ButtonDown; }
			set {
				if (withEventsField_ButtonDown != null) {
					withEventsField_ButtonDown.Click -= ButtonDown_Click;
				}
				withEventsField_ButtonDown = value;
				if (withEventsField_ButtonDown != null) {
					withEventsField_ButtonDown.Click += ButtonDown_Click;
				}
			}
		}
		internal System.Windows.Forms.GroupBox GroupTab;
		internal System.Windows.Forms.CheckBox CheckExCaseSensitive;
		internal System.Windows.Forms.CheckBox CheckCaseSensitive;
		internal System.Windows.Forms.CheckBox CheckRetweet;
		internal System.Windows.Forms.TextBox TextExSource;
		internal System.Windows.Forms.Label Label12;
		internal System.Windows.Forms.CheckBox CheckExRetweet;
		internal System.Windows.Forms.TextBox TextSource;
		internal System.Windows.Forms.Label Label5;
		private System.Windows.Forms.Button withEventsField_ButtonRuleDown;
		internal System.Windows.Forms.Button ButtonRuleDown {
			get { return withEventsField_ButtonRuleDown; }
			set {
				if (withEventsField_ButtonRuleDown != null) {
					withEventsField_ButtonRuleDown.Click -= ButtonRuleDown_Click;
				}
				withEventsField_ButtonRuleDown = value;
				if (withEventsField_ButtonRuleDown != null) {
					withEventsField_ButtonRuleDown.Click += ButtonRuleDown_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonRuleUp;
		internal System.Windows.Forms.Button ButtonRuleUp {
			get { return withEventsField_ButtonRuleUp; }
			set {
				if (withEventsField_ButtonRuleUp != null) {
					withEventsField_ButtonRuleUp.Click -= ButtonRuleUp_Click;
				}
				withEventsField_ButtonRuleUp = value;
				if (withEventsField_ButtonRuleUp != null) {
					withEventsField_ButtonRuleUp.Click += ButtonRuleUp_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonRuleMove;
		internal System.Windows.Forms.Button ButtonRuleMove {
			get { return withEventsField_ButtonRuleMove; }
			set {
				if (withEventsField_ButtonRuleMove != null) {
					withEventsField_ButtonRuleMove.Click -= ButtonRuleMove_Click;
				}
				withEventsField_ButtonRuleMove = value;
				if (withEventsField_ButtonRuleMove != null) {
					withEventsField_ButtonRuleMove.Click += ButtonRuleMove_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonRuleCopy;
		internal System.Windows.Forms.Button ButtonRuleCopy {
			get { return withEventsField_ButtonRuleCopy; }
			set {
				if (withEventsField_ButtonRuleCopy != null) {
					withEventsField_ButtonRuleCopy.Click -= ButtonRuleCopy_Click;
				}
				withEventsField_ButtonRuleCopy = value;
				if (withEventsField_ButtonRuleCopy != null) {
					withEventsField_ButtonRuleCopy.Click += ButtonRuleCopy_Click;
				}
			}
		}
		internal System.Windows.Forms.Label LabelTabType;
		internal System.Windows.Forms.Label Label13;
		internal System.Windows.Forms.CheckBox CheckExLambDa;

		internal System.Windows.Forms.CheckBox CheckLambda;
	}
}
