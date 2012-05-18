using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Hoehoe
{
    partial class InputTabName : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputTabName));
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.LabelDescription = new System.Windows.Forms.Label();
            this.TextTabName = new System.Windows.Forms.TextBox();
            this.LabelUsage = new System.Windows.Forms.Label();
            this.ComboUsage = new System.Windows.Forms.ComboBox();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.okButton, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.cancelButton, 1, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // LabelDescription
            // 
            resources.ApplyResources(this.LabelDescription, "LabelDescription");
            this.LabelDescription.Name = "LabelDescription";
            // 
            // TextTabName
            // 
            resources.ApplyResources(this.TextTabName, "TextTabName");
            this.TextTabName.Name = "TextTabName";
            // 
            // LabelUsage
            // 
            resources.ApplyResources(this.LabelUsage, "LabelUsage");
            this.LabelUsage.Name = "LabelUsage";
            // 
            // ComboUsage
            // 
            this.ComboUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboUsage.FormattingEnabled = true;
            resources.ApplyResources(this.ComboUsage, "ComboUsage");
            this.ComboUsage.Name = "ComboUsage";
            this.ComboUsage.SelectedIndexChanged += new System.EventHandler(this.ComboUsage_SelectedIndexChanged);
            // 
            // InputTabName
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.ComboUsage);
            this.Controls.Add(this.LabelUsage);
            this.Controls.Add(this.TextTabName);
            this.Controls.Add(this.LabelDescription);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputTabName";
            this.Shown += new System.EventHandler(this.InputTabName_Shown);
            this.Load += new System.EventHandler(this.InputTabName_Load);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button okButton;
        internal System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.Label LabelDescription;
        internal System.Windows.Forms.TextBox TextTabName;
        internal System.Windows.Forms.Label LabelUsage;
        internal System.Windows.Forms.ComboBox ComboUsage;
    }
}
