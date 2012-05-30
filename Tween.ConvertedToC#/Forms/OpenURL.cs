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

    public partial class OpenURL
    {
        #region private

        private string selectedUrl;

        #endregion private

        #region constructor

        public OpenURL()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region properties

        public string SelectedUrl
        {
            get { return this.UrlList.SelectedItems.Count == 1 ? this.selectedUrl : string.Empty; }
        }

        #endregion properties

        #region public methods

        public void ClearUrl()
        {
            this.UrlList.Items.Clear();
        }

        public void AddUrl(OpenUrlItem openUrlItem)
        {
            this.UrlList.Items.Add(openUrlItem);
        }

        #endregion public methods

        #region event handler

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.SelectUrlOrCancelDialog();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.CloseWithCancel();
        }

        private void OpenURL_Shown(object sender, EventArgs e)
        {
            this.UrlList.Focus();
            if (this.UrlList.Items.Count > 0)
            {
                this.UrlList.SelectedIndex = 0;
            }
        }

        private void UrlList_DoubleClick(object sender, EventArgs e)
        {
            if (this.UrlList.SelectedItem == null)
            {
                return;
            }

            if (this.UrlList.IndexFromPoint(this.UrlList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            if (this.UrlList.Items[this.UrlList.IndexFromPoint(this.UrlList.PointToClient(Control.MousePosition))] == null)
            {
                return;
            }

            this.SelectUrlOrCancelDialog();
        }

        private void UrlList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.J && this.UrlList.SelectedIndex < this.UrlList.Items.Count - 1)
            {
                e.SuppressKeyPress = true;
                this.UrlList.SelectedIndex += 1;
            }

            if (e.KeyCode == Keys.K && this.UrlList.SelectedIndex > 0)
            {
                e.SuppressKeyPress = true;
                this.UrlList.SelectedIndex -= 1;
            }

            if (e.Control && e.KeyCode == Keys.Oem4)
            {
                e.SuppressKeyPress = true;
                this.CloseWithCancel();
            }
        }

        #endregion event handler

        #region private methods

        private void SelectUrlOrCancelDialog()
        {
            if (this.UrlList.SelectedItems.Count == 0)
            {
                this.CloseWithCancel();
            }
            else
            {
                this.selectedUrl = this.UrlList.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void CloseWithCancel()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion private methods
    }
}