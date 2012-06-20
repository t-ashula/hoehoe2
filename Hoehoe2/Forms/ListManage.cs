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
    using R = Hoehoe.Properties.Resources;

    public partial class ListManage
    {
        #region private

        private Twitter twitter;

        #endregion private

        #region constructor

        public ListManage(Twitter tw)
        {
            this.InitializeComponent();
            this.twitter = tw;
        }

        #endregion constructor

        #region eventhandler

        private void ListManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.EditCheckBox.Checked)
            {
                this.OKEditButton.PerformClick();
            }
        }

        private void ListManage_Load(object sender, EventArgs e)
        {
            this.UserList_SelectedIndexChanged(null, EventArgs.Empty);
            if (TabInformations.Instance.SubscribableLists.Count == 0)
            {
                this.RefreshLists();
            }

            foreach (ListElement listItem in TabInformations.Instance.SubscribableLists.FindAll(i => i.Username == this.twitter.Username))
            {
                this.ListsList.Items.Add(listItem);
            }

            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }

            this.ListsList.Focus();
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
            {
                return;
            }

            ListElement list = (ListElement)this.ListsList.SelectedItem;
            this.UsernameTextBox.Text = list.Username;
            this.NameTextBox.Text = list.Name;
            this.PublicRadioButton.Checked = list.IsPublic;
            this.PrivateRadioButton.Checked = !list.IsPublic;
            this.MemberCountTextBox.Text = list.MemberCount.ToString();
            this.SubscriberCountTextBox.Text = list.SubscriberCount.ToString();
            this.DescriptionText.Text = list.Description;
            this.UserList.Items.Clear();
            foreach (UserInfo user in list.Members)
            {
                this.UserList.Items.Add(user);
            }

            this.GetMoreUsersButton.Text = (this.UserList.Items.Count > 0 ? R.ListManageGetMoreUsers2 : R.ListManageGetMoreUsers1).ToString();
        }

        private void EditCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var state = this.EditCheckBox.Checked;
            this.AddListButton.Enabled = !state;
            this.EditCheckBox.Enabled = !state;
            this.DeleteListButton.Enabled = !state;
            this.NameTextBox.ReadOnly = !state;
            this.PublicRadioButton.Enabled = state;
            this.PrivateRadioButton.Enabled = state;
            this.DescriptionText.ReadOnly = !state;
            this.ListsList.Enabled = !state;
            this.OKEditButton.Enabled = state;
            this.CancelEditButton.Enabled = state;
            this.EditCheckBox.AutoCheck = !state;
            this.MemberGroup.Enabled = !state;
            this.UserGroup.Enabled = !state;
            this.CloseButton.Enabled = !state;
            this.UsernameTextBox.TabStop = !state;
            this.MemberCountTextBox.TabStop = !state;
            this.SubscriberCountTextBox.TabStop = !state;
            if (state)
            {
                this.NameTextBox.Focus();
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
            {
                return;
            }

            ListElement listItem = (ListElement)this.ListsList.SelectedItem;

            if (string.IsNullOrEmpty(this.NameTextBox.Text))
            {
                MessageBox.Show(R.ListManageOKButton1);
                return;
            }

            listItem.Name = this.NameTextBox.Text;
            listItem.IsPublic = this.PublicRadioButton.Checked;
            listItem.Description = this.DescriptionText.Text;

            string rslt = listItem.Refresh();

            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(string.Format(R.ListManageOKButton2, rslt));
                return;
            }

            this.ListsList.Items.Clear();
            this.ListManage_Load(null, EventArgs.Empty);

            this.EditCheckBox.AutoCheck = true;
            this.EditCheckBox.Checked = false;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.EditCheckBox.AutoCheck = true;
            this.EditCheckBox.Checked = false;

            for (int i = this.ListsList.Items.Count - 1; i >= 0; i--)
            {
                if (this.ListsList.Items[i] is NewListElement)
                {
                    this.ListsList.Items.RemoveAt(i);
                }
            }

            this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
        }

        private void RefreshUsersButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
            {
                return;
            }

            this.UserList.Items.Clear();
            Action<ListElement> dlgt = new Action<ListElement>(lElement => { this.Invoke(new Action<string>(GetListMembersCallback), lElement.RefreshMembers()); });
            dlgt.BeginInvoke((ListElement)this.ListsList.SelectedItem, null, null);
        }

        private void GetMoreUsersButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
            {
                return;
            }

            Action<ListElement> dlgt = new Action<ListElement>(lElement => { this.Invoke(new Action<string>(GetListMembersCallback), lElement.GetMoreMembers()); });
            dlgt.BeginInvoke((ListElement)this.ListsList.SelectedItem, null, null);
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null || this.UserList.SelectedItem == null)
            {
                return;
            }

            ListElement list = (ListElement)this.ListsList.SelectedItem;
            UserInfo user = (UserInfo)this.UserList.SelectedItem;
            if (MessageBox.Show(R.ListManageDeleteUser1, "Hoehoe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = this.twitter.RemoveUserToList(list.Id.ToString(), user.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(string.Format(R.ListManageDeleteUser2, rslt));
                    return;
                }

                int idx = this.ListsList.SelectedIndex;
                list.Members.Remove(user);
                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                if (idx < this.ListsList.Items.Count)
                {
                    this.ListsList.SelectedIndex = idx;
                }
            }
        }

        private void DeleteListButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
            {
                return;
            }

            ListElement list = (ListElement)this.ListsList.SelectedItem;
            if (MessageBox.Show(R.ListManageDeleteLists1, "Hoehoe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = this.twitter.DeleteList(list.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(R.ListManageOKButton2, rslt);
                    return;
                }

                rslt = this.twitter.GetListsApi();

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(R.ListsDeleteFailed, rslt);
                    return;
                }

                this.ListsList.Items.Clear();
                this.ListManage_Load(this, EventArgs.Empty);
            }
        }

        private void AddListButton_Click(object sender, EventArgs e)
        {
            NewListElement newList = new NewListElement(this.twitter);
            this.ListsList.Items.Add(newList);
            this.ListsList.SelectedItem = newList;
            this.EditCheckBox.Checked = true;
            this.EditCheckBox_CheckedChanged(this.EditCheckBox, EventArgs.Empty);
        }

        private void UserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.UserList.SelectedItem == null)
            {
                if (this.UserIcon.Image != null)
                {
                    this.UserIcon.Image.Dispose();
                    this.UserIcon.Image = null;
                }

                this.UserLocation.Text = string.Empty;
                this.UserWeb.Text = string.Empty;
                this.UserFollowNum.Text = "0";
                this.UserFollowerNum.Text = "0";
                this.UserPostsNum.Text = "0";
                this.UserProfile.Text = string.Empty;
                this.UserTweetDateTime.Text = string.Empty;
                this.UserTweet.Text = string.Empty;
                this.DeleteUserButton.Enabled = false;
            }
            else
            {
                UserInfo user = (UserInfo)this.UserList.SelectedItem;
                this.UserLocation.Text = user.Location;
                this.UserWeb.Text = user.Url;
                this.UserFollowNum.Text = user.FriendsCount.ToString("#,###,##0");
                this.UserFollowerNum.Text = user.FollowersCount.ToString("#,###,##0");
                this.UserPostsNum.Text = user.StatusesCount.ToString("#,###,##0");
                this.UserProfile.Text = user.Description;
                if (!string.IsNullOrEmpty(user.RecentPost))
                {
                    this.UserTweetDateTime.Text = user.PostCreatedAt.ToString("yy/MM/dd HH:mm");
                    this.UserTweet.Text = user.RecentPost;
                }
                else
                {
                    this.UserTweetDateTime.Text = string.Empty;
                    this.UserTweet.Text = string.Empty;
                }

                this.DeleteUserButton.Enabled = true;
                Action<Uri> a = new Action<Uri>(url => { this.Invoke(new Action<Image>(DisplayIcon), (new HttpVarious()).GetImage(url)); });
                a.BeginInvoke(user.ImageUrl, null, null);
            }
        }

        private void RefreshListsButton_Click(object sender, EventArgs e)
        {
            this.RefreshLists();
            this.ListsList.Items.Clear();
            this.ListManage_Load(null, EventArgs.Empty);
        }

        private void RefreshLists_Dowork(object sender, DoWorkEventArgs e)
        {
            e.Result = this.twitter.GetListsApi();
        }

        private void UserWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.Owner != null)
            {
                ((TweenMain)this.Owner).OpenUriAsync(this.UserWeb.Text);
            }
        }

        private void ListManage_Validating(object sender, CancelEventArgs e)
        {
            if (this.EditCheckBox.Checked)
            {
                e.Cancel = true;
                this.CancelButton.PerformClick();
            }
        }

        #endregion eventhandler

        #region private methods

        private void GetListMembersCallback(string result)
        {
            if (result == this.ListsList.SelectedItem.ToString())
            {
                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                this.GetMoreUsersButton.Text = R.ListManageGetMoreUsers1;
            }
            else
            {
                MessageBox.Show(string.Format(R.ListManageGetListMembersCallback1, result));
            }
        }

        private void DisplayIcon(Image img)
        {
            if (img == null || this.UserList.SelectedItem == null)
            {
                return;
            }

            if (((UserInfo)this.UserList.SelectedItem).ImageUrl.ToString() == (string)img.Tag)
            {
                this.UserIcon.Image = img;
            }
        }

        private void RefreshLists()
        {
            using (FormInfo dlg = new FormInfo(this, R.ListsGetting, this.RefreshLists_Dowork))
            {
                dlg.ShowDialog();
                if (!string.IsNullOrEmpty((string)dlg.Result))
                {
                    MessageBox.Show(string.Format(R.ListsDeleteFailed, (string)dlg.Result));
                    return;
                }
            }
        }

        #endregion private methods

        #region inner class

        private class NewListElement : ListElement
        {
            public NewListElement(Twitter tw)
            {
                this.Tw = tw;
                this.IsCreated = false;
            }

            public bool IsCreated { get; private set; }

            public override string Refresh()
            {
                if (this.IsCreated)
                {
                    return base.Refresh();
                }

                string rslt = this.Tw.CreateListApi(this.Name, !this.IsPublic, this.Description);
                this.IsCreated = string.IsNullOrEmpty(rslt);
                return rslt;
            }

            public override string ToString()
            {
                return this.IsCreated ? base.ToString() : "NewList";
            }
        }

        #endregion inner class
    }
}