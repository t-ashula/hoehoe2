// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System.Windows.Forms;

namespace Tween
{
    public partial class DialogAsShieldIcon
    {
        //Private shield As New ShieldIcon

        private DialogResult dResult = System.Windows.Forms.DialogResult.None;

        private void OK_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.dResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.dResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void DialogAsShieldIcon_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (dResult == System.Windows.Forms.DialogResult.None)
            {
                e.Cancel = true;
                dResult = System.Windows.Forms.DialogResult.Cancel;
                this.Hide();
            }
        }

        private void DialogAsShieldIcon_Load(System.Object sender, System.EventArgs e)
        {
            //OK_Button.Image = shield.Icon
            PictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
        }

        public new System.Windows.Forms.DialogResult ShowDialog(string text, string detail = "", string caption = "DialogAsShieldIcon", System.Windows.Forms.MessageBoxButtons Buttons = MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            Label1.Text = text;
            this.Text = caption;
            this.TextDetail.Text = detail;
            switch (Buttons)
            {
                case MessageBoxButtons.OKCancel:
                    OK_Button.Text = "OK";
                    Cancel_Button.Text = "Cancel";
                    break;
                case MessageBoxButtons.YesNo:
                    OK_Button.Text = "Yes";
                    Cancel_Button.Text = "No";
                    break;
                default:
                    OK_Button.Text = "OK";
                    Cancel_Button.Text = "Cancel";
                    break;
            }
            // とりあえずアイコンは処理しない（互換性のためパラメータだけ指定できる）

            base.ShowDialog(this.Owner);
            while (this.dResult == System.Windows.Forms.DialogResult.None)
            {
                System.Threading.Thread.Sleep(200);
                Application.DoEvents();
            }
            if (Buttons == MessageBoxButtons.YesNo)
            {
                switch (dResult)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return System.Windows.Forms.DialogResult.Yes;
                    case System.Windows.Forms.DialogResult.Cancel:
                        return System.Windows.Forms.DialogResult.No;
                }
            }
            //else
            {
                return dResult;
            }
        }

        public DialogAsShieldIcon()
        {
            Load += DialogAsShieldIcon_Load;
            FormClosing += DialogAsShieldIcon_FormClosing;
            InitializeComponent();
        }
    }
}