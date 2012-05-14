using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tween
{
    partial class SearchWord : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWord));
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SWordText = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.CheckSearchCaseSensitive = new System.Windows.Forms.CheckBox();
            this.CheckSearchRegex = new System.Windows.Forms.CheckBox();
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
            this.okButton.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // SWordText
            // 
            resources.ApplyResources(this.SWordText, "SWordText");
            this.SWordText.Name = "SWordText";
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // CheckSearchCaseSensitive
            // 
            resources.ApplyResources(this.CheckSearchCaseSensitive, "CheckSearchCaseSensitive");
            this.CheckSearchCaseSensitive.Name = "CheckSearchCaseSensitive";
            this.CheckSearchCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // CheckSearchRegex
            // 
            resources.ApplyResources(this.CheckSearchRegex, "CheckSearchRegex");
            this.CheckSearchRegex.Name = "CheckSearchRegex";
            this.CheckSearchRegex.UseVisualStyleBackColor = true;
            // 
            // SearchWord
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.CheckSearchRegex);
            this.Controls.Add(this.CheckSearchCaseSensitive);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.SWordText);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWord";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.SearchWord_Shown);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;        
        internal System.Windows.Forms.Button okButton;
        internal System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.TextBox SWordText;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.CheckBox CheckSearchCaseSensitive;
        internal System.Windows.Forms.CheckBox CheckSearchRegex;
    }
}
