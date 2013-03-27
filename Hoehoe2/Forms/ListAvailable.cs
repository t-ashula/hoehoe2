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
            SelectedList = null;
            InitializeComponent();
        }

        #endregion

        #region properties

        public ListElement SelectedList { get; private set; }

        #endregion

        #region event handler

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedIndex < 0)
            {
                return;
            }

            SelectedList = (ListElement)ListsList.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            SelectedList = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ListAvailable_Shown(object sender, EventArgs e)
        {
            if (TabInformations.Instance.SubscribableLists.Count == 0)
            {
                RefreshLists();
            }

            ListsList.Items.AddRange(TabInformations.Instance.SubscribableLists.ToArray());
            if (ListsList.Items.Count > 0)
            {
                ListsList.SelectedIndex = 0;
            }
            else
            {
                ClearListInfo();
            }
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListsList.SelectedIndex > -1)
            {
                SetListInfo((ListElement)ListsList.SelectedItem);
            }
            else
            {
                ClearListInfo();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshLists();
            ListsList.Items.Clear();
            ListsList.Items.AddRange(TabInformations.Instance.SubscribableLists.ToArray());
            if (ListsList.Items.Count > 0)
            {
                ListsList.SelectedIndex = 0;
            }
        }

        private void RefreshLists_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = ((TweenMain)Owner).TwitterInstance.GetListsApi();
            }
            catch (InvalidCastException)
            {
                // todo: do something?
            }
        }

        #endregion

        #region private methods

        private void ClearListInfo()
        {
            UsernameLabel.Text = string.Empty;
            NameLabel.Text = string.Empty;
            StatusLabel.Text = string.Empty;
            MemberCountLabel.Text = "0";
            SubscriberCountLabel.Text = "0";
            DescriptionText.Text = string.Empty;
        }

        private void SetListInfo(ListElement lst)
        {
            UsernameLabel.Text = string.Format("{0} / {1}", lst.Username, lst.Nickname);
            NameLabel.Text = lst.Name;
            StatusLabel.Text = lst.IsPublic ? "Public" : "Private";
            MemberCountLabel.Text = lst.MemberCount.ToString("#,##0");
            SubscriberCountLabel.Text = lst.SubscriberCount.ToString("#,##0");
            DescriptionText.Text = lst.Description;
        }

        private void RefreshLists()
        {
            using (var dlg = new FormInfo(this, "Getting Lists...", RefreshLists_DoWork))
            {
                dlg.ShowDialog();
                var ret = (string)dlg.Result;
                if (string.IsNullOrEmpty(ret))
                {
                    return;
                }

                MessageBox.Show(string.Format("Failed to get lists. ({0})", ret));
            }
        }

        #endregion
    }
}