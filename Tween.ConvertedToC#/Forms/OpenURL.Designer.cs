namespace Hoehoe
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    partial class OpenURL : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenURL));
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.UrlList = new System.Windows.Forms.ListBox();
            this.TableLayoutPanel2.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel2
            // 
            resources.ApplyResources(this.TableLayoutPanel2, "TableLayoutPanel2");
            this.TableLayoutPanel2.Controls.Add(this.TableLayoutPanel1, 0, 1);
            this.TableLayoutPanel2.Controls.Add(this.UrlList, 0, 0);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
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
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // UrlList
            // 
            this.UrlList.DisplayMember = "Text";
            resources.ApplyResources(this.UrlList, "UrlList");
            this.UrlList.FormattingEnabled = true;
            this.UrlList.Name = "UrlList";
            this.UrlList.ValueMember = "Url";
            this.UrlList.DoubleClick += new System.EventHandler(this.UrlList_DoubleClick);
            this.UrlList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UrlList_KeyDown);
            // 
            // OpenURL
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.TableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenURL";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new EventHandler(this.OpenURL_Shown);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button okButton;
        internal System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.ListBox UrlList;
    }
}