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
    using System.Drawing;
    using System.Windows.Forms;
    using R = Properties.Resources;

    public partial class ListManage
    {
        #region private

        private readonly Twitter _twitter;

        #endregion

        #region constructor

        public ListManage(Twitter tw)
        {
            InitializeComponent();
            _twitter = tw;
        }

        #endregion

        #region eventhandler

        private void ListManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && EditCheckBox.Checked)
            {
                OKEditButton.PerformClick();
            }
        }

        private void ListManage_Load(object sender, EventArgs e)
        {
            UserList_SelectedIndexChanged(null, EventArgs.Empty);
            if (TabInformations.Instance.SubscribableLists.Count == 0)
            {
                RefreshLists();
            }

            foreach (var listItem in TabInformations.Instance.SubscribableLists.FindAll(i => i.Username == _twitter.Username))
            {
                ListsList.Items.Add(listItem);
            }

            if (ListsList.Items.Count > 0)
            {
                ListsList.SelectedIndex = 0;
            }

            ListsList.Focus();
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null)
            {
                return;
            }

            var list = (ListElement)ListsList.SelectedItem;
            UsernameTextBox.Text = list.Username;
            NameTextBox.Text = list.Name;
            PublicRadioButton.Checked = list.IsPublic;
            PrivateRadioButton.Checked = !list.IsPublic;
            MemberCountTextBox.Text = list.MemberCount.ToString();
            SubscriberCountTextBox.Text = list.SubscriberCount.ToString();
            DescriptionText.Text = list.Description;
            UserList.Items.Clear();
            foreach (UserInfo user in list.Members)
            {
                UserList.Items.Add(user);
            }

            GetMoreUsersButton.Text = (UserList.Items.Count > 0 ? R.ListManageGetMoreUsers2 : R.ListManageGetMoreUsers1);
        }

        private void EditCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var state = EditCheckBox.Checked;
            AddListButton.Enabled = !state;
            EditCheckBox.Enabled = !state;
            DeleteListButton.Enabled = !state;
            NameTextBox.ReadOnly = !state;
            PublicRadioButton.Enabled = state;
            PrivateRadioButton.Enabled = state;
            DescriptionText.ReadOnly = !state;
            ListsList.Enabled = !state;
            OKEditButton.Enabled = state;
            CancelEditButton.Enabled = state;
            EditCheckBox.AutoCheck = !state;
            MemberGroup.Enabled = !state;
            UserGroup.Enabled = !state;
            CloseButton.Enabled = !state;
            UsernameTextBox.TabStop = !state;
            MemberCountTextBox.TabStop = !state;
            SubscriberCountTextBox.TabStop = !state;
            if (state)
            {
                NameTextBox.Focus();
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null)
            {
                return;
            }

            var listItem = (ListElement)ListsList.SelectedItem;

            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                MessageBox.Show(R.ListManageOKButton1);
                return;
            }

            listItem.Name = NameTextBox.Text;
            listItem.IsPublic = PublicRadioButton.Checked;
            listItem.Description = DescriptionText.Text;

            string rslt = listItem.Refresh();

            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(string.Format(R.ListManageOKButton2, rslt));
                return;
            }

            ListsList.Items.Clear();
            ListManage_Load(null, EventArgs.Empty);

            EditCheckBox.AutoCheck = true;
            EditCheckBox.Checked = false;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            EditCheckBox.AutoCheck = true;
            EditCheckBox.Checked = false;

            for (int i = ListsList.Items.Count - 1; i >= 0; i--)
            {
                if (ListsList.Items[i] is NewListElement)
                {
                    ListsList.Items.RemoveAt(i);
                }
            }

            ListsList_SelectedIndexChanged(ListsList, EventArgs.Empty);
        }

        private void RefreshUsersButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null)
            {
                return;
            }

            UserList.Items.Clear();
            Action<ListElement> dlgt = lElement => Invoke(new Action<string>(GetListMembersCallback), lElement.RefreshMembers());
            dlgt.BeginInvoke((ListElement)ListsList.SelectedItem, null, null);
        }

        private void GetMoreUsersButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null)
            {
                return;
            }

            Action<ListElement> dlgt = lElement => Invoke(new Action<string>(GetListMembersCallback), lElement.GetMoreMembers());
            dlgt.BeginInvoke((ListElement)ListsList.SelectedItem, null, null);
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null || UserList.SelectedItem == null)
            {
                return;
            }

            var list = (ListElement)ListsList.SelectedItem;
            var user = (UserInfo)UserList.SelectedItem;
            if (MessageBox.Show(R.ListManageDeleteUser1, "Hoehoe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = _twitter.RemoveUserToList(list.Id.ToString(), user.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(string.Format(R.ListManageDeleteUser2, rslt));
                    return;
                }

                int idx = ListsList.SelectedIndex;
                list.Members.Remove(user);
                ListsList_SelectedIndexChanged(ListsList, EventArgs.Empty);
                if (idx < ListsList.Items.Count)
                {
                    ListsList.SelectedIndex = idx;
                }
            }
        }

        private void DeleteListButton_Click(object sender, EventArgs e)
        {
            if (ListsList.SelectedItem == null)
            {
                return;
            }

            var list = (ListElement)ListsList.SelectedItem;
            if (MessageBox.Show(R.ListManageDeleteLists1, "Hoehoe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = _twitter.DeleteList(list.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(R.ListManageOKButton2, rslt);
                    return;
                }

                rslt = _twitter.GetListsApi();

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(R.ListsDeleteFailed, rslt);
                    return;
                }

                ListsList.Items.Clear();
                ListManage_Load(this, EventArgs.Empty);
            }
        }

        private void AddListButton_Click(object sender, EventArgs e)
        {
            var newList = new NewListElement(_twitter);
            ListsList.Items.Add(newList);
            ListsList.SelectedItem = newList;
            EditCheckBox.Checked = true;
            EditCheckBox_CheckedChanged(EditCheckBox, EventArgs.Empty);
        }

        private void UserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserList.SelectedItem == null)
            {
                if (UserIcon.Image != null)
                {
                    UserIcon.Image.Dispose();
                    UserIcon.Image = null;
                }

                UserLocation.Text = string.Empty;
                UserWeb.Text = string.Empty;
                UserFollowNum.Text = "0";
                UserFollowerNum.Text = "0";
                UserPostsNum.Text = "0";
                UserProfile.Text = string.Empty;
                UserTweetDateTime.Text = string.Empty;
                UserTweet.Text = string.Empty;
                DeleteUserButton.Enabled = false;
            }
            else
            {
                var user = (UserInfo)UserList.SelectedItem;
                UserLocation.Text = user.Location;
                UserWeb.Text = user.Url;
                UserFollowNum.Text = user.FriendsCount.ToString("#,###,##0");
                UserFollowerNum.Text = user.FollowersCount.ToString("#,###,##0");
                UserPostsNum.Text = user.StatusesCount.ToString("#,###,##0");
                UserProfile.Text = user.Description;
                if (!string.IsNullOrEmpty(user.RecentPost))
                {
                    UserTweetDateTime.Text = user.PostCreatedAt.ToString("yy/MM/dd HH:mm");
                    UserTweet.Text = user.RecentPost;
                }
                else
                {
                    UserTweetDateTime.Text = string.Empty;
                    UserTweet.Text = string.Empty;
                }

                DeleteUserButton.Enabled = true;
                Action<Uri> a = url => Invoke(new Action<Image>(DisplayIcon), (new HttpVarious()).GetImage(url));
                a.BeginInvoke(user.ImageUrl, null, null);
            }
        }

        private void RefreshListsButton_Click(object sender, EventArgs e)
        {
            RefreshLists();
            ListsList.Items.Clear();
            ListManage_Load(null, EventArgs.Empty);
        }

        private void RefreshLists_Dowork(object sender, DoWorkEventArgs e)
        {
            e.Result = _twitter.GetListsApi();
        }

        private void UserWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Owner != null)
            {
                ((TweenMain)Owner).OpenUriAsync(UserWeb.Text);
            }
        }

        private void ListManage_Validating(object sender, CancelEventArgs e)
        {
            if (EditCheckBox.Checked)
            {
                e.Cancel = true;
                CancelButton.PerformClick();
            }
        }

        #endregion

        #region private methods

        private void GetListMembersCallback(string result)
        {
            if (result == ListsList.SelectedItem.ToString())
            {
                ListsList_SelectedIndexChanged(ListsList, EventArgs.Empty);
                GetMoreUsersButton.Text = R.ListManageGetMoreUsers1;
            }
            else
            {
                MessageBox.Show(string.Format(R.ListManageGetListMembersCallback1, result));
            }
        }

        private void DisplayIcon(Image img)
        {
            if (img == null || UserList.SelectedItem == null)
            {
                return;
            }

            if (((UserInfo)UserList.SelectedItem).ImageUrl.ToString() == (string)img.Tag)
            {
                UserIcon.Image = img;
            }
        }

        private void RefreshLists()
        {
            using (var dlg = new FormInfo(this, R.ListsGetting, RefreshLists_Dowork))
            {
                dlg.ShowDialog();
                var result = (string)dlg.Result;
                if (string.IsNullOrEmpty(result))
                {
                    return;
                }

                MessageBox.Show(string.Format(R.ListsDeleteFailed, result));
            }
        }

        #endregion

        #region inner class

        private class NewListElement : ListElement
        {
            public NewListElement(Twitter tw)
            {
                Tw = tw;
                IsCreated = false;
            }

            public bool IsCreated { get; private set; }

            public override string Refresh()
            {
                if (IsCreated)
                {
                    return base.Refresh();
                }

                string rslt = Tw.CreateListApi(Name, !IsPublic, Description);
                IsCreated = string.IsNullOrEmpty(rslt);
                return rslt;
            }

            public override string ToString()
            {
                return IsCreated ? base.ToString() : "NewList";
            }
        }

        #endregion
    }
}