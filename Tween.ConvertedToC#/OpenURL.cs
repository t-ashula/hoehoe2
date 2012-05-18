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

using System;
using System.Windows.Forms;

namespace Hoehoe
{
    public partial class OpenURL
    {
        private string _selUrl;

        private void OkButton_Click(object sender, EventArgs e)
        {
            SelectUrlOrCancelDialog();
        }

        private void SelectUrlOrCancelDialog()
        {
            if (UrlList.SelectedItems.Count == 0)
            {
                CloseWithCancel();
            }
            else
            {
                _selUrl = UrlList.SelectedItem.ToString();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CloseWithCancel();
        }

        private void CloseWithCancel()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public void ClearUrl()
        {
            UrlList.Items.Clear();
        }

        public void AddUrl(OpenUrlItem openUrlItem)
        {
            UrlList.Items.Add(openUrlItem);
        }

        public string SelectedUrl
        {
            get { return UrlList.SelectedItems.Count == 1 ? _selUrl : ""; }
        }

        private void OpenURL_Shown(object sender, EventArgs e)
        {
            UrlList.Focus();
            if (UrlList.Items.Count > 0)
            {
                UrlList.SelectedIndex = 0;
            }
        }

        private void UrlList_DoubleClick(object sender, EventArgs e)
        {
            if (UrlList.SelectedItem == null)
            {
                return;
            }

            if (UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            if (UrlList.Items[UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition))] == null)
            {
                return;
            }
            SelectUrlOrCancelDialog();
        }

        private void UrlList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.J && UrlList.SelectedIndex < UrlList.Items.Count - 1)
            {
                e.SuppressKeyPress = true;
                UrlList.SelectedIndex += 1;
            }
            if (e.KeyCode == Keys.K && UrlList.SelectedIndex > 0)
            {
                e.SuppressKeyPress = true;
                UrlList.SelectedIndex -= 1;
            }
            if (e.Control && e.KeyCode == Keys.Oem4)
            {
                e.SuppressKeyPress = true;
                CloseWithCancel();
            }
        }

        public OpenURL()
        {
            InitializeComponent();
        }
    }
}