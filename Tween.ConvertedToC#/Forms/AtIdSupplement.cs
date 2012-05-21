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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Hoehoe
{
    public partial class AtIdSupplement
    {
        #region constructor
        public AtIdSupplement()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            // InitializeComponent() 呼び出しの後で初期化を追加します。
        }

        public AtIdSupplement(List<string> ItemList, string startCharacter)
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            for (int i = 0; i < ItemList.Count; i++)
            {
                this.TextId.AutoCompleteCustomSource.Add(ItemList[i]);
            }
            _startChar = startCharacter;
        }
        #endregion

        #region event handler

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            InputText = this.TextId.Text;
            IsBack = false;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            InputText = "";
            IsBack = false;
        }

        private void TextId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && String.IsNullOrEmpty(this.TextId.Text))
            {
                InputText = "";
                IsBack = true;
                this.Close();
            }
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Tab)
            {
                InputText = this.TextId.Text + " ";
                IsBack = false;
                this.Close();
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!String.IsNullOrEmpty(this.TextId.Text))
                {
                    int idx = this.TextId.AutoCompleteCustomSource.IndexOf(this.TextId.Text);
                    if (idx > -1)
                    {
                        this.TextId.Text = "";
                        this.TextId.AutoCompleteCustomSource.RemoveAt(idx);
                    }
                }
            }
        }

        private void AtIdSupplement_Load(object sender, EventArgs e)
        {
            if (_startChar == "#")
            {
                this.ClientSize = new Size(this.TextId.Width, this.TextId.Height);
                //プロパティで切り替えできるように
                this.TextId.ImeMode = ImeMode.Inherit;
            }
        }

        private void AtIdSupplement_Shown(object sender, EventArgs e)
        {
            TextId.Text = _startChar;
            if (!String.IsNullOrEmpty(StartsWith))
            {
                TextId.Text += StartsWith.Substring(0, StartsWith.Length);
            }
            TextId.SelectionStart = TextId.Text.Length;
            TextId.Focus();
        }

        private void TextId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                InputText = this.TextId.Text + " ";
                IsBack = false;
                this.Close();
            }
        }

        private void AtIdSupplement_FormClosed(object sender, FormClosedEventArgs e)
        {
            StartsWith = "";
            this.DialogResult = IsBack ? DialogResult.Cancel : DialogResult.OK;
        }

        #endregion
        #region properties

        public string StartsWith { get; set; }

        public int ItemCount
        {
            get { return this.TextId.AutoCompleteCustomSource.Count; }
        }
        #endregion

        #region public
        public string InputText = "";
        public bool IsBack = false;

        public void AddItem(string id)
        {
            if (!this.TextId.AutoCompleteCustomSource.Contains(id))
            {
                this.TextId.AutoCompleteCustomSource.Add(id);
            }
        }

        public void AddRangeItem(string[] ids)
        {
            foreach (string id in ids)
            {
                this.AddItem(id);
            }
        }

        public List<string> GetItemList()
        {
            List<string> ids = new List<string>();
            for (int i = 0; i < this.TextId.AutoCompleteCustomSource.Count; i++)
            {
                ids.Add(this.TextId.AutoCompleteCustomSource[i]);
            }
            return ids;
        }
        #endregion

        #region private
        private string _startChar = "";
        #endregion
    }
}