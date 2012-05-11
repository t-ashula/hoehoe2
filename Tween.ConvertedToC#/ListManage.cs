using System;

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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tween
{
    public partial class ListManage
    {
        private Twitter tw;

        public ListManage(Twitter tw)
        {
            Validating += ListManage_Validating;
            Load += ListManage_Load;
            KeyDown += ListManage_KeyDown;
            this.InitializeComponent();

            this.tw = tw;
        }

        private void ListManage_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.EditCheckBox.Checked)
            {
                this.OKEditButton.PerformClick();
            }
        }

        private void ListManage_Load(System.Object sender, System.EventArgs e)
        {
            this.UserList_SelectedIndexChanged(null, EventArgs.Empty);
            if (TabInformations.GetInstance().SubscribableLists.Count == 0)
                this.RefreshLists();
            foreach (ListElement listItem in TabInformations.GetInstance().SubscribableLists.FindAll(i => i.Username == this.tw.Username))
            {
                this.ListsList.Items.Add(listItem);
            }
            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }
            this.ListsList.Focus();
        }

        private void ListsList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
                return;

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

            this.GetMoreUsersButton.Text = (this.UserList.Items.Count > 0 ? Tween.My_Project.Resources.ListManageGetMoreUsers2 : Tween.My_Project.Resources.ListManageGetMoreUsers1).ToString();
        }

        private void EditCheckBox_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            this.AddListButton.Enabled = !this.EditCheckBox.Checked;
            this.EditCheckBox.Enabled = !this.EditCheckBox.Checked;
            this.DeleteListButton.Enabled = !this.EditCheckBox.Checked;

            this.NameTextBox.ReadOnly = !this.EditCheckBox.Checked;
            this.PublicRadioButton.Enabled = this.EditCheckBox.Checked;
            this.PrivateRadioButton.Enabled = this.EditCheckBox.Checked;
            this.DescriptionText.ReadOnly = !this.EditCheckBox.Checked;
            this.ListsList.Enabled = !this.EditCheckBox.Checked;

            this.OKEditButton.Enabled = this.EditCheckBox.Checked;
            this.CancelEditButton.Enabled = this.EditCheckBox.Checked;
            this.EditCheckBox.AutoCheck = !this.EditCheckBox.Checked;

            this.MemberGroup.Enabled = !this.EditCheckBox.Checked;
            this.UserGroup.Enabled = !this.EditCheckBox.Checked;
            this.CloseButton.Enabled = !this.EditCheckBox.Checked;

            this.UsernameTextBox.TabStop = !this.EditCheckBox.Checked;
            this.MemberCountTextBox.TabStop = !this.EditCheckBox.Checked;
            this.SubscriberCountTextBox.TabStop = !this.EditCheckBox.Checked;
            if (this.EditCheckBox.Checked == true)
                this.NameTextBox.Focus();
        }

        private void OKButton_Click(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
                return;
            ListElement listItem = (ListElement)this.ListsList.SelectedItem;

            if (string.IsNullOrEmpty(this.NameTextBox.Text))
            {
                MessageBox.Show(Tween.My_Project.Resources.ListManageOKButton1);
                return;
            }

            listItem.Name = this.NameTextBox.Text;
            listItem.IsPublic = this.PublicRadioButton.Checked;
            listItem.Description = this.DescriptionText.Text;

            string rslt = listItem.Refresh();

            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(string.Format(Tween.My_Project.Resources.ListManageOKButton2, rslt));
                return;
            }

            this.ListsList.Items.Clear();
            this.ListManage_Load(null, EventArgs.Empty);

            this.EditCheckBox.AutoCheck = true;
            this.EditCheckBox.Checked = false;
        }

        private void CancelButton_Click(System.Object sender, System.EventArgs e)
        {
            this.EditCheckBox.AutoCheck = true;
            this.EditCheckBox.Checked = false;

            for (int i = this.ListsList.Items.Count - 1; i >= 0; i += -1)
            {
                if (this.ListsList.Items[i] is NewListElement)
                {
                    this.ListsList.Items.RemoveAt(i);
                }
            }

            this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
        }

        private void RefreshUsersButton_Click(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
                return;
            this.UserList.Items.Clear();
            Action<ListElement> dlgt = new Action<ListElement>(lElement => { this.Invoke(new Action<string>(GetListMembersCallback), lElement.RefreshMembers()); });
            dlgt.BeginInvoke((ListElement)this.ListsList.SelectedItem, null, null);
        }

        private void GetMoreUsersButton_Click(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
                return;
            Action<ListElement> dlgt = new Action<ListElement>(lElement => { this.Invoke(new Action<string>(GetListMembersCallback), lElement.GetMoreMembers()); });
            dlgt.BeginInvoke((ListElement)this.ListsList.SelectedItem, null, null);
        }

        private void GetListMembersCallback(string result)
        {
            if (result == this.ListsList.SelectedItem.ToString())
            {
                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                this.GetMoreUsersButton.Text = Tween.My_Project.Resources.ListManageGetMoreUsers1;
            }
            else
            {
                MessageBox.Show(string.Format(Tween.My_Project.Resources.ListManageGetListMembersCallback1, result));
            }
        }

        private void DeleteUserButton_Click(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null || this.UserList.SelectedItem == null)
            {
                return;
            }

            ListElement list = (ListElement)this.ListsList.SelectedItem;
            UserInfo user = (UserInfo)this.UserList.SelectedItem;
            if (MessageBox.Show(Tween.My_Project.Resources.ListManageDeleteUser1, "Tween", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = this.tw.RemoveUserToList(list.Id.ToString(), user.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(string.Format(Tween.My_Project.Resources.ListManageDeleteUser2, rslt));
                    return;
                }
                int idx = ListsList.SelectedIndex;
                list.Members.Remove(user);
                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                if (idx < ListsList.Items.Count)
                    ListsList.SelectedIndex = idx;
            }
        }

        private void DeleteListButton_Click(System.Object sender, System.EventArgs e)
        {
            if (this.ListsList.SelectedItem == null)
                return;
            ListElement list = (ListElement)this.ListsList.SelectedItem;

            if (MessageBox.Show(Tween.My_Project.Resources.ListManageDeleteLists1, "Tween", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string rslt = "";

                rslt = this.tw.DeleteList(list.Id.ToString());

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(Tween.My_Project.Resources.ListManageOKButton2, rslt);
                    return;
                }

                rslt = this.tw.GetListsApi();

                if (!string.IsNullOrEmpty(rslt))
                {
                    MessageBox.Show(Tween.My_Project.Resources.ListsDeleteFailed, rslt);
                    return;
                }

                this.ListsList.Items.Clear();
                this.ListManage_Load(this, EventArgs.Empty);
            }
        }

        private void AddListButton_Click(System.Object sender, System.EventArgs e)
        {
            NewListElement newList = new NewListElement(this.tw);
            this.ListsList.Items.Add(newList);
            this.ListsList.SelectedItem = newList;
            this.EditCheckBox.Checked = true;
            this.EditCheckBox_CheckedChanged(this.EditCheckBox, EventArgs.Empty);
        }

        private void UserList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (UserList.SelectedItem == null)
            {
                if (this.UserIcon.Image != null)
                {
                    this.UserIcon.Image.Dispose();
                    this.UserIcon.Image = null;
                }
                this.UserLocation.Text = "";
                this.UserWeb.Text = "";
                this.UserFollowNum.Text = "0";
                this.UserFollowerNum.Text = "0";
                this.UserPostsNum.Text = "0";
                this.UserProfile.Text = "";
                this.UserTweetDateTime.Text = "";
                this.UserTweet.Text = "";
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
                    this.UserTweetDateTime.Text = "";
                    this.UserTweet.Text = "";
                }
                this.DeleteUserButton.Enabled = true;

                Action<Uri> a = new Action<Uri>(url => { this.Invoke(new Action<Image>(DisplayIcon), (new HttpVarious()).GetImage(url)); });
                a.BeginInvoke(user.ImageUrl, null, null);
            }
        }

        private void DisplayIcon(Image img)
        {
            if (img == null || this.UserList.SelectedItem == null)
                return;
            if (((UserInfo)this.UserList.SelectedItem).ImageUrl.ToString() == (string)img.Tag)
            {
                this.UserIcon.Image = img;
            }
        }

        private void RefreshListsButton_Click(System.Object sender, System.EventArgs e)
        {
            this.RefreshLists();
            this.ListsList.Items.Clear();
            this.ListManage_Load(null, EventArgs.Empty);
        }

        private void RefreshLists()
        {
            using (FormInfo dlg = new FormInfo(this, Tween.My_Project.Resources.ListsGetting, RefreshLists_Dowork))
            {
                dlg.ShowDialog();
                if (!string.IsNullOrEmpty((string)dlg.Result))
                {
                    MessageBox.Show(string.Format(Tween.My_Project.Resources.ListsDeleteFailed, (string)dlg.Result));
                    return;
                }
            }
        }

        private void RefreshLists_Dowork(object sender, DoWorkEventArgs e)
        {
            e.Result = tw.GetListsApi();
        }

        private void UserWeb_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (this.Owner != null)
            {
                ((TweenMain)this.Owner).OpenUriAsync(UserWeb.Text);
            }
        }

        private class NewListElement : ListElement
        {
            private bool _isCreated = false;

            public NewListElement(Twitter tw)
            {
                this._tw = tw;
            }

            public override string Refresh()
            {
                if (this.IsCreated)
                {
                    return base.Refresh();
                }
                else
                {
                    string rslt = this._tw.CreateListApi(this.Name, !this.IsPublic, this.Description);
                    this._isCreated = (string.IsNullOrEmpty(rslt));
                    return rslt;
                }
            }

            public bool IsCreated
            {
                get { return this._isCreated; }
            }

            public override string ToString()
            {
                if (IsCreated)
                {
                    return base.ToString();
                }
                else
                {
                    return "NewList";
                }
            }
        }

        private void ListManage_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.EditCheckBox.Checked)
            {
                e.Cancel = true;
                this.CancelButton.PerformClick();
            }
        }
    }
}