// Hoehoe - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
//
// All rights reserved.
// This file is part of Hoehoe.
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

namespace Hoehoe
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public partial class DialogAsShieldIcon
    {
        #region privates

        private DialogResult _result;

        #endregion privates

        #region constructor

        public DialogAsShieldIcon()
        {
            InitializeComponent();
            _result = DialogResult.None;
        }

        #endregion constructor

        #region public methods

        public DialogResult ShowDialog(string text, string detail = "", string caption = "DialogAsShieldIcon", MessageBoxButtons buttons = MessageBoxButtons.OKCancel, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            Label1.Text = text;
            Text = caption;
            TextDetail.Text = detail;
            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    okButton.Text = "OK";
                    cancelButton.Text = "Cancel";
                    break;
                case MessageBoxButtons.YesNo:
                    okButton.Text = "Yes";
                    cancelButton.Text = "No";
                    break;
                default:
                    okButton.Text = "OK";
                    cancelButton.Text = "Cancel";
                    break;
            }

            // とりあえずアイコンは処理しない（互換性のためパラメータだけ指定できる）
            ShowDialog(Owner);
            while (_result == DialogResult.None)
            {
                Thread.Sleep(200);
                Application.DoEvents();
            }

            if (buttons == MessageBoxButtons.YesNo)
            {
                switch (_result)
                {
                    case DialogResult.OK:
                        return DialogResult.Yes;
                    case DialogResult.Cancel:
                        return DialogResult.No;
                }
            }

            return _result;
        }

        #endregion public methods

        #region event handler

        private void OkButton_Click(object sender, EventArgs e)
        {
            _result = DialogResult.OK;
            Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            _result = DialogResult.Cancel;
            Hide();
        }

        private void DialogAsShieldIcon_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_result == DialogResult.None)
            {
                e.Cancel = true;
                _result = DialogResult.Cancel;
                Hide();
            }
        }

        private void DialogAsShieldIcon_Load(object sender, EventArgs e)
        {
            PictureBox1.Image = SystemIcons.Question.ToBitmap();
        }

        #endregion event handler
    }
}