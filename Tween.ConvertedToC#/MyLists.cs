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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tween
{
    public partial class MyLists
    {
        private string _contextUserName;
        private Twitter _tw;

        public MyLists(string userName, Twitter tw)
        {
            InitializeComponent();

            this._contextUserName = userName;
            this._tw = tw;
            this.Text = this._contextUserName + Tween.Properties.Resources.MyLists1;
        }

        private void MyLists_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        private void LoadList()
        {
            this.ListsCheckedListBox.ItemCheck -= this.ListsCheckedListBox_ItemCheck;

            this.ListsCheckedListBox.Items.AddRange(TabInformations.GetInstance().SubscribableLists.FindAll(item => item.Username == this._tw.Username).ToArray());

            for (int i = 0; i < this.ListsCheckedListBox.Items.Count; i++)
            {
                ListElement listItem = (ListElement)this.ListsCheckedListBox.Items[i];

                List<PostClass> listPost = new List<PostClass>();
                List<PostClass> otherPost = new List<PostClass>();

                foreach (TabClass tab in TabInformations.GetInstance().Tabs.Values)
                {
                    if (tab.TabType == MyCommon.TabUsageType.Lists)
                    {
                        if (listItem.Id == tab.ListInfo.Id)
                        {
                            listPost.AddRange(tab.Posts.Values);
                        }
                        else
                        {
                            otherPost.AddRange(tab.Posts.Values);
                        }
                    }
                }

                //リストが空の場合は推定不能
                if (listPost.Count == 0)
                {
                    this.ListsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate);
                    continue;
                }

                //リストに該当ユーザーのポストが含まれていれば、リストにユーザーが含まれているとする。
                if (listPost.Exists(item => item.ScreenName == _contextUserName))
                {
                    this.ListsCheckedListBox.SetItemChecked(i, true);
                    continue;
                }

                List<long> listPostUserIDs = new List<long>();
                List<string> listPostUserNames = new List<string>();
                DateTime listOlderPostCreatedAt = DateTime.MaxValue;
                DateTime listNewistPostCreatedAt = DateTime.MinValue;

                foreach (PostClass post in listPost)
                {
                    if (post.UserId > 0 && !listPostUserIDs.Contains(post.UserId))
                    {
                        listPostUserIDs.Add(post.UserId);
                    }
                    if (post.ScreenName != null && !listPostUserNames.Contains(post.ScreenName))
                    {
                        listPostUserNames.Add(post.ScreenName);
                    }
                    if (post.CreatedAt < listOlderPostCreatedAt)
                    {
                        listOlderPostCreatedAt = post.CreatedAt;
                    }
                    if (post.CreatedAt > listNewistPostCreatedAt)
                    {
                        listNewistPostCreatedAt = post.CreatedAt;
                    }
                }

                //リスト中のユーザーの人数がlistItem.MemberCount以上で、かつ該当のユーザーが含まれていなければ、リストにユーザーは含まれていないとする。
                if (listItem.MemberCount > 0 && listItem.MemberCount <= listPostUserIDs.Count && (!listPostUserNames.Contains(_contextUserName)))
                {
                    this.ListsCheckedListBox.SetItemChecked(i, false);
                    continue;
                }

                otherPost.AddRange(TabInformations.GetInstance().Posts.Values);

                //リストに該当ユーザーのポストが含まれていないのにリスト以外で取得したポストの中にリストに含まれるべきポストがある場合は、リストにユーザーは含まれていないとする。
                if (otherPost.Exists(item => (item.ScreenName == this._contextUserName) && (item.CreatedAt > listOlderPostCreatedAt) && (item.CreatedAt < listNewistPostCreatedAt) && ((!item.IsReply) || listPostUserNames.Contains(item.InReplyToUser))))
                {
                    this.ListsCheckedListBox.SetItemChecked(i, false);
                    continue;
                }

                this.ListsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate);
            }

            this.ListsCheckedListBox.ItemCheck += this.ListsCheckedListBox_ItemCheck;
        }

        private void ListRefreshButton_Click(object sender, EventArgs e)
        {
            string rslt = this._tw.GetListsApi();
            if (!String.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(string.Format(Tween.Properties.Resources.ListsDeleteFailed, rslt));
            }
            else
            {
                this.ListsCheckedListBox.Items.Clear();
                LoadList();
            }
        }

        private void ListsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            switch (e.CurrentValue)
            {
                case CheckState.Indeterminate:
                    {
                        ListElement listItem = (ListElement)this.ListsCheckedListBox.Items[e.Index];

                        bool ret = false;
                        string rslt = this._tw.ContainsUserAtList(listItem.Id.ToString(), _contextUserName, ref ret);
                        if (!string.IsNullOrEmpty(rslt))
                        {
                            MessageBox.Show(string.Format(Tween.Properties.Resources.ListManageOKButton2, rslt));
                            e.NewValue = CheckState.Indeterminate;
                        }
                        else
                        {
                            if (ret)
                            {
                                e.NewValue = CheckState.Checked;
                            }
                            else
                            {
                                e.NewValue = CheckState.Unchecked;
                            }
                        }
                    }
                    break;
                case CheckState.Unchecked:
                    {
                        ListElement list = (ListElement)this.ListsCheckedListBox.Items[e.Index];
                        string rslt = this._tw.AddUserToList(list.Id.ToString(), this._contextUserName.ToString());
                        if (!string.IsNullOrEmpty(rslt))
                        {
                            MessageBox.Show(string.Format(Tween.Properties.Resources.ListManageOKButton2, rslt));
                            e.NewValue = CheckState.Indeterminate;
                        }
                    }
                    break;
                case CheckState.Checked:
                    {
                        ListElement list = (ListElement)this.ListsCheckedListBox.Items[e.Index];
                        string rslt = this._tw.RemoveUserToList(list.Id.ToString(), this._contextUserName.ToString());
                        if (!string.IsNullOrEmpty(rslt))
                        {
                            MessageBox.Show(string.Format(Tween.Properties.Resources.ListManageOKButton2, rslt));
                            e.NewValue = CheckState.Indeterminate;
                        }
                    }
                    break;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = this.ListsCheckedListBox.SelectedItem == null;
        }

        private void AddListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListsCheckedListBox.ItemCheck -= this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Unchecked);
            this.ListsCheckedListBox.ItemCheck += this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Checked);
        }

        private void DeleteListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListsCheckedListBox.ItemCheck -= this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Checked);
            this.ListsCheckedListBox.ItemCheck += this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Unchecked);
        }

        private void ReloadListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ListsCheckedListBox.ItemCheck -= this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Indeterminate);
            this.ListsCheckedListBox.ItemCheck += this.ListsCheckedListBox_ItemCheck;
            this.ListsCheckedListBox.SetItemCheckState(this.ListsCheckedListBox.SelectedIndex, CheckState.Checked);
        }

        private void ListsCheckedListBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //項目が無い部分をクリックしても、選択されている項目のチェック状態が変更されてしまうので、その対策
                    for (int index = 0; index < ListsCheckedListBox.Items.Count; index++)
                    {
                        if (this.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location))
                        {
                            return;
                        }
                    }

                    this.ListsCheckedListBox.SelectedItem = null;
                    break;
                case MouseButtons.Right:
                    //コンテキストメニューの項目実行時にSelectedItemプロパティを利用出来るように
                    for (int index = 0; index < ListsCheckedListBox.Items.Count; index++)
                    {
                        if (this.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location))
                        {
                            this.ListsCheckedListBox.SetSelected(index, true);
                            return;
                        }
                    }

                    this.ListsCheckedListBox.SelectedItem = null;
                    break;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}