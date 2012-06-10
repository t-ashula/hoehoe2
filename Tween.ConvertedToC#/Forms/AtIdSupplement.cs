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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public partial class AtIdSupplement
    {
        #region private fields
        private string startChar = string.Empty;
        #endregion

        #region constructor

        public AtIdSupplement(List<string> itemList, string startCharacter)
        {
            this.InitializeComponent();
            this.startChar = startCharacter;
            this.TextId.AutoCompleteCustomSource.AddRange(itemList.ToArray());
        }
        #endregion

        #region properties

        public string StartsWith { get; set; }

        public int ItemCount
        {
            get { return this.TextId.AutoCompleteCustomSource.Count; }
        }

        public string InputText { get; private set; }

        public bool IsBack { get; private set; }

        #endregion

        #region public
        public bool AddItem(string id)
        {
            if (!this.TextId.AutoCompleteCustomSource.Contains(id))
            {
                this.TextId.AutoCompleteCustomSource.Add(id);
                return true;
            }

            return false;
        }

        public bool AddRangeItem(IEnumerable<string> ids)
        {
            var cnt = this.ItemCount;
            var q = ids.Distinct();
            foreach (string id in q)
            {
                this.AddItem(id);
            }

            return cnt != this.ItemCount;
        }

        public List<string> GetItemList()
        {
            return this.TextId.AutoCompleteCustomSource.Cast<string>().ToList();
        }
        #endregion
        #region event handler

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.InputText = this.TextId.Text;
            this.IsBack = false;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.InputText = string.Empty;
            this.IsBack = false;
        }

        private void TextId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(this.TextId.Text))
            {
                this.InputText = string.Empty;
                this.IsBack = true;
                this.Close();
            }

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Tab)
            {
                this.InputText = this.TextId.Text + " ";
                this.IsBack = false;
                this.Close();
            }

            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!string.IsNullOrEmpty(this.TextId.Text))
                {
                    int idx = this.TextId.AutoCompleteCustomSource.IndexOf(this.TextId.Text);
                    if (idx > -1)
                    {
                        this.TextId.Text = string.Empty;
                        this.TextId.AutoCompleteCustomSource.RemoveAt(idx);
                    }
                }
            }
        }

        private void AtIdSupplement_Load(object sender, EventArgs e)
        {
            if (this.startChar == "#")
            {
                this.ClientSize = new Size(this.TextId.Width, this.TextId.Height);
                this.TextId.ImeMode = ImeMode.Inherit;
            }
        }

        private void AtIdSupplement_Shown(object sender, EventArgs e)
        {
            this.TextId.Text = this.startChar;
            if (!string.IsNullOrEmpty(this.StartsWith))
            {
                this.TextId.Text += this.StartsWith.Substring(0, this.StartsWith.Length);
            }

            this.TextId.SelectionStart = this.TextId.Text.Length;
            this.TextId.Focus();
        }

        private void TextId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                this.InputText = this.TextId.Text + " ";
                this.IsBack = false;
                this.Close();
            }
        }

        private void AtIdSupplement_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.StartsWith = string.Empty;
            this.DialogResult = this.IsBack ? DialogResult.Cancel : DialogResult.OK;
        }

        #endregion
    }
}