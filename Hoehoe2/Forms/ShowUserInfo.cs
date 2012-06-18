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
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Windows.Forms;
    using R = Hoehoe.Properties.Resources;

    public partial class ShowUserInfo
    {
        #region private

        private const string Mainpath = "http://twitter.com/";
        private const string Followingpath = "/following";
        private const string Followerspath = "/followers";
        private const string Favpath = "/favorites";

        private DataModels.Twitter.User userInfo;
        private UserInfo info = new UserInfo();
        private Image icondata;
        private List<string> atidList = new List<string>();
        private string descriptionTxt;
        private string recentPostTxt;
        private string home;
        private string following;
        private string followers;
        private string favorites;
        private string friendshipResult = string.Empty;
        private TweenMain owner;
        private bool isEditing;
        private string buttonEditText = string.Empty;

        #endregion private

        #region constructor

        public ShowUserInfo()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region public methods

        public void SetUser(DataModels.Twitter.User value)
        {
            this.userInfo = value;
        }

        #endregion public methods

        #region event handler

        private void ShowUserInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void ShowUserInfo_Load(object sender, EventArgs e)
        {
            this.owner = (TweenMain)this.Owner;
            if (!this.AnalizeUserInfo(this.userInfo))
            {
                MessageBox.Show(R.ShowUserInfo1);
                this.Close();
                return;
            }

            // アイコンロード
            this.BackgroundWorkerImageLoader.RunWorkerAsync();

            this.InitPath();
            this.InitTooltip();
            this.Text = this.Text.Insert(0, this.info.ScreenName + " ");
            this.LabelId.Text = this.info.Id.ToString();
            this.LabelScreenName.Text = this.info.ScreenName;
            this.LabelName.Text = this.info.Name;
            this.LabelLocation.Text = this.info.Location;
            this.SetLinklabelWeb(this.info.Url);
            this.DescriptionBrowser.Visible = false;
            this.MakeDescriptionBrowserText(this.info.Description);
            this.RecentPostBrowser.Visible = false;
            if (this.info.RecentPost != null)
            {
                this.recentPostTxt = this.owner.CreateDetailHtml(
                    this.owner.TwitterInstance.CreateHtmlAnchor(ref this.info.RecentPost, this.atidList, this.userInfo.Status.Entities, null)
                    + string.Format(" Posted at {0} via {1}", this.info.PostCreatedAt, this.info.PostSource));
            }

            this.LinkLabelFollowing.Text = this.info.FriendsCount.ToString();
            this.LinkLabelFollowers.Text = this.info.FollowersCount.ToString();
            this.LinkLabelFav.Text = this.info.FavoriteCount.ToString();
            this.LinkLabelTweet.Text = this.info.StatusesCount.ToString();
            this.LabelCreatedAt.Text = this.info.CreatedAt.ToString();
            this.LabelIsProtected.Text = this.info.Protect ? R.Yes : R.No;
            this.LabelIsVerified.Text = this.info.Verified ? R.Yes : R.No;

            if (this.owner.TwitterInstance.Username == this.info.ScreenName)
            {
                this.ButtonEdit.Enabled = true;
                this.ChangeIconToolStripMenuItem.Enabled = true;
                this.ButtonBlock.Enabled = false;
                this.ButtonReportSpam.Enabled = false;
                this.ButtonBlockDestroy.Enabled = false;
            }
            else
            {
                this.ButtonEdit.Enabled = false;
                this.ChangeIconToolStripMenuItem.Enabled = false;
                this.ButtonBlock.Enabled = true;
                this.ButtonReportSpam.Enabled = true;
                this.ButtonBlockDestroy.Enabled = true;
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LinkLabelWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.info.Url != null)
            {
                this.owner.OpenUriAsync(this.LinkLabelWeb.Text);
            }
        }

        private void LinkLabelFollowing_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync(this.following);
        }

        private void LinkLabelFollowers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync(this.followers);
        }

        private void LinkLabelTweet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync(this.home);
        }

        private void LinkLabelFav_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync(this.favorites);
        }

        private void ButtonFollow_Click(object sender, EventArgs e)
        {
            string ret = this.owner.TwitterInstance.PostFollowCommand(this.info.ScreenName);
            if (!string.IsNullOrEmpty(ret))
            {
                MessageBox.Show(R.FRMessage2 + ret);
            }
            else
            {
                MessageBox.Show(R.FRMessage3);
                this.LabelIsFollowing.Text = R.GetFriendshipInfo1;
                this.ButtonFollow.Enabled = false;
                this.ButtonUnFollow.Enabled = true;
            }
        }

        private void ButtonUnFollow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + R.ButtonUnFollow_ClickText1, R.ButtonUnFollow_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string ret = this.owner.TwitterInstance.PostRemoveCommand(this.info.ScreenName);
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(R.FRMessage2 + ret);
                }
                else
                {
                    MessageBox.Show(R.FRMessage3);
                    this.LabelIsFollowing.Text = R.GetFriendshipInfo2;
                    this.ButtonFollow.Enabled = true;
                    this.ButtonUnFollow.Enabled = false;
                }
            }
        }

        private void ShowUserInfo_Activated(object sender, EventArgs e)
        {
            // 画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
            if (this.UserPicture.Image != null)
            {
                this.UserPicture.Invalidate(false);
            }
        }

        private void ShowUserInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.UserPicture.Image = null;
            if (this.icondata != null)
            {
                this.icondata.Dispose();
            }
        }

        private void BackgroundWorkerImageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            string name = this.info.ImageUrl.ToString();
            this.icondata = (new HttpVarious()).GetImage(name.Replace("_normal", "_bigger"));
            if (this.owner.TwitterInstance.Username == this.info.ScreenName)
            {
                return;
            }

            this.info.IsFollowing = false;
            this.info.IsFollowed = false;
            this.friendshipResult = this.owner.TwitterInstance.GetFriendshipInfo(this.info.ScreenName, ref this.info.IsFollowing, ref this.info.IsFollowed);
        }

        private void BackgroundWorkerImageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (this.icondata != null)
                {
                    this.UserPicture.Image = this.icondata;
                }
            }
            catch (Exception)
            {
                this.UserPicture.Image = null;
            }

            if (this.owner.TwitterInstance.Username == this.info.ScreenName)
            {
                // 自分の場合
                this.LabelIsFollowing.Text = string.Empty;
                this.LabelIsFollowed.Text = string.Empty;
                this.ButtonFollow.Enabled = false;
                this.ButtonUnFollow.Enabled = false;
            }
            else
            {
                if (string.IsNullOrEmpty(this.friendshipResult))
                {
                    this.LabelIsFollowing.Text = this.info.IsFollowing ? R.GetFriendshipInfo1 : R.GetFriendshipInfo2;
                    this.ButtonFollow.Enabled = !this.info.IsFollowing;
                    this.LabelIsFollowed.Text = this.info.IsFollowed ? R.GetFriendshipInfo3 : R.GetFriendshipInfo4;
                    this.ButtonUnFollow.Enabled = this.info.IsFollowing;
                }
                else
                {
                    MessageBox.Show(this.friendshipResult);
                    this.ButtonUnFollow.Enabled = false;
                    this.ButtonFollow.Enabled = false;
                    this.LabelIsFollowed.Text = R.GetFriendshipInfo6;
                    this.LabelIsFollowing.Text = R.GetFriendshipInfo6;
                }
            }
        }

        private void ShowUserInfo_Shown(object sender, EventArgs e)
        {
            this.DescriptionBrowser.DocumentText = this.descriptionTxt;
            this.DescriptionBrowser.Visible = true;
            if (this.info.RecentPost != null)
            {
                this.RecentPostBrowser.DocumentText = this.recentPostTxt;
                this.RecentPostBrowser.Visible = true;
            }
            else
            {
                this.LabelRecentPost.Text = R.ShowUserInfo2;
            }

            this.ButtonClose.Focus();
        }

        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;

                if (e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") || e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23"))
                {
                    // ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(e.Url.AbsoluteUri);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    this.owner.HashSupl.AddItem(hash);
                    this.owner.HashMgr.AddHashToHistory(hash.Trim(), false);
                    this.owner.AddNewTabForSearch(hash);
                    return;
                }
                else
                {
                    Match m = Regex.Match(e.Url.AbsoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (Configs.Instance.OpenUserTimeline && m.Success && this.owner.IsTwitterId(m.Result("${ScreenName}")))
                    {
                        this.owner.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                    else
                    {
                        this.owner.OpenUriAsync(e.Url.OriginalString);
                    }
                }
            }
        }

        private void WebBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            WebBrowser browser = (WebBrowser)sender;
            if (browser.StatusText.StartsWith("http"))
            {
                this.ToolTip1.Show(browser.StatusText, this, PointToClient(MousePosition));
            }
            else if (string.IsNullOrEmpty(this.DescriptionBrowser.StatusText))
            {
                this.ToolTip1.Hide(this);
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = this.ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                sc.Document.ExecCommand("SelectAll", false, null);
            }
        }

        private void SelectionCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = this.ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string selectedText = this.owner.WebBrowser_GetSelectionText( sc);
                if (selectedText != null)
                {
                    try
                    {
                        Clipboard.SetDataObject(selectedText, false, 5, 100);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            WebBrowser sc = this.ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string selectedText = this.owner.WebBrowser_GetSelectionText( sc);
                if (selectedText == null)
                {
                    this.SelectionCopyToolStripMenuItem.Enabled = false;
                }
                else
                {
                    this.SelectionCopyToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void ShowUserInfo_MouseEnter(object sender, EventArgs e)
        {
            this.ToolTip1.Hide(this);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync("http://support.twitter.com/groups/31-twitter-basics/topics/111-features/articles/268350-x8a8d-x8a3c-x6e08-x307f-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.owner.OpenUriAsync("http://support.twitter.com/groups/31-twitter-basics/topics/107-my-profile-account-settings/articles/243055-x516c-x958b-x3001-x975e-x516c-x958b-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void ButtonSearchPosts_Click(object sender, EventArgs e)
        {
            this.owner.AddNewTabForUserTimeline(this.info.ScreenName);
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (this.UserPicture.Image != null)
            {
                string name = this.info.ImageUrl.ToString();
                this.owner.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
            }
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Default;
        }

        private void UpdateProfile_Dowork(object sender, DoWorkEventArgs e)
        {
            UpdateProfileArgs arg = (UpdateProfileArgs)e.Argument;
            e.Result = arg.Tw.PostUpdateProfile(arg.Name, arg.Url, arg.Location, arg.Description);
        }

        private void UpddateProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string res = (string)e.Result;
            if (res.StartsWith("err:", StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show(res);
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            // 自分以外のプロフィールは変更できない
            if (this.owner.TwitterInstance.Username != this.info.ScreenName)
            {
                return;
            }

            if (!this.isEditing)
            {
                this.buttonEditText = this.ButtonEdit.Text;
                this.ButtonEdit.Text = R.UserInfoButtonEdit_ClickText1;

                // 座標初期化,プロパティ設定
                this.TextBoxName.Location = this.LabelName.Location;
                this.TextBoxName.Height = this.LabelName.Height;
                this.TextBoxName.Width = this.LabelName.Width;
                this.TextBoxName.BackColor = this.owner.InputBackColor;
                this.TextBoxName.MaxLength = 20;
                this.TextBoxName.Text = this.LabelName.Text;
                this.TextBoxName.TabStop = true;
                this.TextBoxName.Visible = true;
                this.LabelName.Visible = false;

                this.TextBoxLocation.Location = this.LabelLocation.Location;
                this.TextBoxLocation.Height = this.LabelLocation.Height;
                this.TextBoxLocation.Width = this.LabelLocation.Width;
                this.TextBoxLocation.BackColor = this.owner.InputBackColor;
                this.TextBoxLocation.MaxLength = 30;
                this.TextBoxLocation.Text = this.LabelLocation.Text;
                this.TextBoxLocation.TabStop = true;
                this.TextBoxLocation.Visible = true;
                this.LabelLocation.Visible = false;

                this.TextBoxWeb.Location = this.LinkLabelWeb.Location;
                this.TextBoxWeb.Height = this.LinkLabelWeb.Height;
                this.TextBoxWeb.Width = this.LinkLabelWeb.Width;
                this.TextBoxWeb.BackColor = this.owner.InputBackColor;
                this.TextBoxWeb.MaxLength = 100;
                this.TextBoxWeb.Text = this.info.Url;
                this.TextBoxWeb.TabStop = true;
                this.TextBoxWeb.Visible = true;
                this.LinkLabelWeb.Visible = false;

                this.TextBoxDescription.Location = this.DescriptionBrowser.Location;
                this.TextBoxDescription.Height = this.DescriptionBrowser.Height;
                this.TextBoxDescription.Width = this.DescriptionBrowser.Width;
                this.TextBoxDescription.BackColor = this.owner.InputBackColor;
                this.TextBoxDescription.MaxLength = 160;
                this.TextBoxDescription.Text = this.info.Description;
                this.TextBoxDescription.Multiline = true;
                this.TextBoxDescription.ScrollBars = ScrollBars.Vertical;
                this.TextBoxDescription.TabStop = true;
                this.TextBoxDescription.Visible = true;
                this.DescriptionBrowser.Visible = false;

                this.TextBoxName.Focus();
                this.TextBoxName.Select(this.TextBoxName.Text.Length, 0);

                this.isEditing = true;
            }
            else
            {
                if (this.TextBoxName.Modified || this.TextBoxLocation.Modified || this.TextBoxWeb.Modified || this.TextBoxDescription.Modified)
                {
                    UpdateProfileArgs arg = new UpdateProfileArgs()
                    {
                        Tw = this.owner.TwitterInstance,
                        Name = this.TextBoxName.Text.Trim(),
                        Url = this.TextBoxWeb.Text.Trim(),
                        Location = this.TextBoxLocation.Text.Trim(),
                        Description = this.TextBoxDescription.Text.Trim()
                    };

                    using (FormInfo dlg = new FormInfo(this, R.UserInfoButtonEdit_ClickText2, this.UpdateProfile_Dowork, this.UpddateProfile_RunWorkerCompleted, arg))
                    {
                        dlg.ShowDialog();
                        if (!string.IsNullOrEmpty(dlg.Result.ToString()))
                        {
                            return;
                        }
                    }
                }

                this.LabelName.Text = this.TextBoxName.Text;
                this.info.Name = this.LabelName.Text;
                this.TextBoxName.TabStop = false;
                this.TextBoxName.Visible = false;
                this.LabelName.Visible = true;

                this.LabelLocation.Text = this.TextBoxLocation.Text;
                this.info.Location = this.LabelLocation.Text;
                this.TextBoxLocation.TabStop = false;
                this.TextBoxLocation.Visible = false;
                this.LabelLocation.Visible = true;

                this.SetLinklabelWeb(this.TextBoxWeb.Text);
                this.info.Url = this.TextBoxWeb.Text;
                this.TextBoxWeb.TabStop = false;
                this.TextBoxWeb.Visible = false;
                this.LinkLabelWeb.Visible = true;

                this.DescriptionBrowser.DocumentText = this.MakeDescriptionBrowserText(this.TextBoxDescription.Text);
                this.info.Description = this.TextBoxDescription.Text;
                this.TextBoxDescription.TabStop = false;
                this.TextBoxDescription.Visible = false;
                this.DescriptionBrowser.Visible = true;

                this.ButtonEdit.Text = this.buttonEditText;

                this.isEditing = false;
            }
        }

        private void UpdateProfileImage_Dowork(object sender, DoWorkEventArgs e)
        {
            UpdateProfileImageArgs arg = (UpdateProfileImageArgs)e.Argument;
            e.Result = arg.Tw.PostUpdateProfileImage(arg.FileName);
        }

        private void UpdateProfileImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string res = string.Empty;
            DataModels.Twitter.User user = null;

            if (e.Result == null)
            {
                return;
            }

            // アイコンを取得してみるが、古いアイコンのユーザーデータが返ってくるため反映/判断できない
            try
            {
                res = this.owner.TwitterInstance.GetUserInfo(this.info.ScreenName, ref user);
                Image img = (new HttpVarious()).GetImage(user.ProfileImageUrl);
                if (img != null)
                {
                    this.UserPicture.Image = img;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenFileDialogIcon.Filter = R.ChangeIconToolStripMenuItem_ClickText1;
            this.OpenFileDialogIcon.Title = R.ChangeIconToolStripMenuItem_ClickText2;
            this.OpenFileDialogIcon.FileName = string.Empty;

            DialogResult rslt = this.OpenFileDialogIcon.ShowDialog();
            if (rslt != DialogResult.OK)
            {
                return;
            }

            string fn = this.OpenFileDialogIcon.FileName;
            if (this.IsValidIconFile(new FileInfo(fn)))
            {
                this.ChangeIcon(fn);
            }
            else
            {
                MessageBox.Show(R.ChangeIconToolStripMenuItem_ClickText6);
            }
        }

        private void ButtonBlock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + R.ButtonBlock_ClickText1, R.ButtonBlock_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostCreateBlock(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + R.ButtonBlock_ClickText3);
                }
                else
                {
                    MessageBox.Show(R.ButtonBlock_ClickText4);
                }
            }
        }

        private void ButtonReportSpam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + R.ButtonReportSpam_ClickText1, R.ButtonReportSpam_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostReportSpam(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + R.ButtonReportSpam_ClickText3);
                }
                else
                {
                    MessageBox.Show(R.ButtonReportSpam_ClickText4);
                }
            }
        }

        private void ButtonBlockDestroy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + R.ButtonBlockDestroy_ClickText1, R.ButtonBlockDestroy_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostDestroyBlock(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + R.ButtonBlockDestroy_ClickText3);
                }
                else
                {
                    MessageBox.Show(R.ButtonBlockDestroy_ClickText4);
                }
            }
        }

        private void ShowUserInfo_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                FileInfo fl = new FileInfo(filename);

                e.Effect = DragDropEffects.None;
                if (this.IsValidIconFile(fl))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ShowUserInfo_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                this.ChangeIcon(filename);
            }
        }

        #endregion event handler

        #region private methods

        private void InitPath()
        {
            this.home = Mainpath + this.info.ScreenName;
            this.following = this.home + Followingpath;
            this.followers = this.home + Followerspath;
            this.favorites = this.home + Favpath;
        }

        private void InitTooltip()
        {
            this.ToolTip1.SetToolTip(this.LinkLabelTweet, this.home);
            this.ToolTip1.SetToolTip(this.LinkLabelFollowing, this.following);
            this.ToolTip1.SetToolTip(this.LinkLabelFollowers, this.followers);
            this.ToolTip1.SetToolTip(this.LinkLabelFav, this.favorites);
        }

        private bool AnalizeUserInfo(DataModels.Twitter.User user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                this.info.Id = user.Id;
                this.info.Name = user.Name.Trim();
                this.info.ScreenName = user.ScreenName;
                this.info.Location = user.Location;
                this.info.Description = user.Description;
                this.info.ImageUrl = new Uri(user.ProfileImageUrl);
                this.info.Url = user.Url;
                this.info.Protect = user.Protected;
                this.info.FriendsCount = user.FriendsCount;
                this.info.FollowersCount = user.FollowersCount;
                this.info.FavoriteCount = user.FavouritesCount;
                this.info.CreatedAt = MyCommon.DateTimeParse(user.CreatedAt);
                this.info.StatusesCount = user.StatusesCount;
                this.info.Verified = user.Verified;
                try
                {
                    this.info.RecentPost = user.Status.Text;
                    this.info.PostCreatedAt = MyCommon.DateTimeParse(user.Status.CreatedAt);
                    this.info.PostSource = user.Status.Source;
                    if (!this.info.PostSource.Contains("</a>"))
                    {
                        this.info.PostSource += "</a>";
                    }
                }
                catch (Exception)
                {
                    this.info.RecentPost = null;
                    this.info.PostSource = null;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void SetLinklabelWeb(string data)
        {
            string webtext = this.owner.TwitterInstance.PreProcessUrl("<a href=\"" + data + "\">Dummy</a>");
            webtext = ShortUrl.Resolve(webtext, false);
            string jumpto = Regex.Match(webtext, "<a href=\"(?<url>.*?)\"").Groups["url"].Value;
            this.ToolTip1.SetToolTip(this.LinkLabelWeb, jumpto);
            this.LinkLabelWeb.Tag = jumpto;
            this.LinkLabelWeb.Text = data;
        }

        private string MakeDescriptionBrowserText(string data)
        {
            this.descriptionTxt = this.owner.CreateDetailHtml(this.owner.TwitterInstance.CreateHtmlAnchor(data, this.atidList, null));
            return this.descriptionTxt;
        }

        private void ChangeIcon(string filename)
        {
            string res = string.Empty;
            UpdateProfileImageArgs arg = new UpdateProfileImageArgs
            {
                Tw = this.owner.TwitterInstance,
                FileName = filename
            };

            using (FormInfo dlg = new FormInfo(this, R.ChangeIconToolStripMenuItem_ClickText3, this.UpdateProfileImage_Dowork, this.UpdateProfileImage_RunWorkerCompleted, arg))
            {
                dlg.ShowDialog();
                res = dlg.Result as string;
                if (!string.IsNullOrEmpty(res))
                {
                    // "Err:"が付いたエラーメッセージが返ってくる
                    MessageBox.Show(res + System.Environment.NewLine + R.ChangeIconToolStripMenuItem_ClickText4);
                }
                else
                {
                    MessageBox.Show(R.ChangeIconToolStripMenuItem_ClickText5);
                }
            }
        }

        private bool IsValidExtension(string ext)
        {
            return ext.Equals(".jpg") || ext.Equals(".jpeg") || ext.Equals(".png") || ext.Equals(".gif");
        }

        private bool IsValidIconFile(FileInfo fileInfo)
        {
            string ext = fileInfo.Extension.ToLower();
            return this.IsValidExtension(ext) && fileInfo.Length < 700 * 1024 && !MyCommon.IsAnimatedGif(fileInfo.FullName);
        }

        #endregion private methods

        #region inner class

        public class UpdateProfileImageArgs
        {
            public Twitter Tw { get; set; }

            public string FileName { get; set; }
        }

        private class UpdateProfileArgs
        {
            public Twitter Tw { get; set; }

            public string Name { get; set; }

            public string Location { get; set; }

            public string Url { get; set; }

            public string Description { get; set; }
        }

        #endregion inner class
    }
}