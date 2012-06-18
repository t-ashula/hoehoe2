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
    using System.Windows.Forms;

    public partial class SearchWord
    {
        #region constructor

        public SearchWord()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region properties

        public string SWord
        {
            get { return this.SWordText.Text; }
            set { this.SWordText.Text = value; }
        }

        public bool CheckCaseSensitive
        {
            get { return this.CheckSearchCaseSensitive.Checked; }
            set { this.CheckSearchCaseSensitive.Checked = value; }
        }

        public bool CheckRegex
        {
            get { return this.CheckSearchRegex.Checked; }
            set { this.CheckSearchRegex.Checked = value; }
        }

        #endregion properties

        #region event handler

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SearchWord_Shown(object sender, EventArgs e)
        {
            this.SWordText.SelectAll();
            this.SWordText.Focus();
        }

        #endregion event handler
    }
}