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

        private readonly string _startChar = string.Empty;

        #endregion private fields

        #region constructor

        public AtIdSupplement(List<string> itemList, string startCharacter)
        {
            InitializeComponent();
            _startChar = startCharacter;
            TextId.AutoCompleteCustomSource.AddRange(itemList.ToArray());
        }

        #endregion constructor

        #region properties

        public string StartsWith { get; set; }

        public int ItemCount
        {
            get { return TextId.AutoCompleteCustomSource.Count; }
        }

        public string InputText { get; private set; }

        public bool IsBack { get; private set; }

        #endregion properties

        #region public

        public bool AddItem(string id)
        {
            if (!TextId.AutoCompleteCustomSource.Contains(id))
            {
                TextId.AutoCompleteCustomSource.Add(id);
                return true;
            }

            return false;
        }

        public bool AddRangeItem(IEnumerable<string> ids)
        {
            var cnt = ItemCount;
            var q = ids.Distinct();
            foreach (var id in q)
            {
                AddItem(id);
            }

            return cnt != ItemCount;
        }

        public List<string> GetItemList()
        {
            return TextId.AutoCompleteCustomSource.Cast<string>().ToList();
        }

        #endregion public

        #region event handler

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            InputText = TextId.Text;
            IsBack = false;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            InputText = string.Empty;
            IsBack = false;
        }

        private void TextId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(TextId.Text))
            {
                InputText = string.Empty;
                IsBack = true;
                Close();
            }

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Tab)
            {
                InputText = TextId.Text + " ";
                IsBack = false;
                Close();
            }

            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!string.IsNullOrEmpty(TextId.Text))
                {
                    int idx = TextId.AutoCompleteCustomSource.IndexOf(TextId.Text);
                    if (idx > -1)
                    {
                        TextId.Text = string.Empty;
                        TextId.AutoCompleteCustomSource.RemoveAt(idx);
                    }
                }
            }
        }

        private void AtIdSupplement_Load(object sender, EventArgs e)
        {
            if (_startChar == "#")
            {
                ClientSize = new Size(TextId.Width, TextId.Height);
                TextId.ImeMode = ImeMode.Inherit;
            }
        }

        private void AtIdSupplement_Shown(object sender, EventArgs e)
        {
            TextId.Text = _startChar;
            if (!string.IsNullOrEmpty(StartsWith))
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
                InputText = TextId.Text + " ";
                IsBack = false;
                Close();
            }
        }

        private void AtIdSupplement_FormClosed(object sender, FormClosedEventArgs e)
        {
            StartsWith = string.Empty;
            DialogResult = IsBack ? DialogResult.Cancel : DialogResult.OK;
        }

        #endregion event handler
    }
}