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
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class ListAvailable
    {
        #region constructor

        public ListAvailable()
        {
            this.SelectedList = null;
            this.InitializeComponent();
        }

        #endregion constructor

        #region properties

        public ListElement SelectedList { get; private set; }

        #endregion properties

        #region event handler

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedIndex > -1)
            {
                this.SelectedList = (ListElement)this.ListsList.SelectedItem;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.SelectedList = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ListAvailable_Shown(object sender, EventArgs e)
        {
            if (TabInformations.GetInstance().SubscribableLists.Count == 0)
            {
                this.RefreshLists();
            }

            this.ListsList.Items.AddRange(TabInformations.GetInstance().SubscribableLists.ToArray());
            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }
            else
            {
                this.ClearListInfo();
            }
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedIndex > -1)
            {
                this.SetListInfo((ListElement)this.ListsList.SelectedItem);
            }
            else
            {
                this.ClearListInfo();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            this.RefreshLists();
            this.ListsList.Items.Clear();
            this.ListsList.Items.AddRange(TabInformations.GetInstance().SubscribableLists.ToArray());
            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }
        }

        private void RefreshLists_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
            }
            catch (InvalidCastException)
            {
                return;
            }
        }

        #endregion event handler

        #region private methods

        private void ClearListInfo()
        {
            this.UsernameLabel.Text = string.Empty;
            this.NameLabel.Text = string.Empty;
            this.StatusLabel.Text = string.Empty;
            this.MemberCountLabel.Text = "0";
            this.SubscriberCountLabel.Text = "0";
            this.DescriptionText.Text = string.Empty;
        }

        private void SetListInfo(ListElement lst)
        {
            this.UsernameLabel.Text = string.Format("{0} / {1}", lst.Username, lst.Nickname);
            this.NameLabel.Text = lst.Name;
            this.StatusLabel.Text = lst.IsPublic ? "Public" : "Private";
            this.MemberCountLabel.Text = lst.MemberCount.ToString("#,##0");
            this.SubscriberCountLabel.Text = lst.SubscriberCount.ToString("#,##0");
            this.DescriptionText.Text = lst.Description;
        }

        private void RefreshLists()
        {
            using (FormInfo dlg = new FormInfo(this, "Getting Lists...", this.RefreshLists_DoWork))
            {
                dlg.ShowDialog();
                if (!string.IsNullOrEmpty((string)dlg.Result))
                {
                    MessageBox.Show("Failed to get lists. (" + (string)dlg.Result + ")");
                    return;
                }
            }
        }

        #endregion private methods
    }
}