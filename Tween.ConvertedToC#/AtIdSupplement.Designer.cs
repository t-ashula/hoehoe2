using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
    partial class AtIdSupplement : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AtIdSupplement));
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.TextId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            //ButtonOK
            //
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ButtonOK, "ButtonOK");
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.TabStop = false;
            this.ButtonOK.UseVisualStyleBackColor = true;
            //
            //ButtonCancel
            //
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.TabStop = false;
            this.ButtonCancel.UseVisualStyleBackColor = true;
            //
            //TextId
            //
            this.TextId.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TextId.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            resources.ApplyResources(this.TextId, "TextId");
            this.TextId.Name = "TextId";
            //
            //Label1
            //
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            //
            //AtIdSupplement
            //
            this.AcceptButton = this.ButtonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.TextId);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AtIdSupplement";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosed += new FormClosedEventHandler(this.AtIdSupplement_FormClosed);
            this.Shown += new EventHandler(this.AtIdSupplement_Shown);
            this.Load += new EventHandler(this.AtIdSupplement_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

            this.ButtonOK.Click += new EventHandler(this.ButtonOK_Click);
            this.ButtonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            this.TextId.KeyDown += new KeyEventHandler(this.TextId_KeyDown);
            this.TextId.PreviewKeyDown += new PreviewKeyDownEventHandler(this.TextId_PreviewKeyDown);
        }        
        internal System.Windows.Forms.Button ButtonOK;
        internal System.Windows.Forms.Button ButtonCancel;
        internal System.Windows.Forms.TextBox TextId;
        internal System.Windows.Forms.Label Label1;
    }
}