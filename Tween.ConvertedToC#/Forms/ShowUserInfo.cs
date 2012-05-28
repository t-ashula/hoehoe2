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
    using Hoehoe.DataModels;

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
        private bool isEditing = false;
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
                MessageBox.Show(Hoehoe.Properties.Resources.ShowUserInfo1);
                this.Close();
                return;
            }

            // アイコンロード
            this.BackgroundWorkerImageLoader.RunWorkerAsync();

            this.InitPath();
            this.InitTooltip();
            this.Text = this.Text.Insert(0, this.info.ScreenName + " ");
            LabelId.Text = this.info.Id.ToString();
            LabelScreenName.Text = this.info.ScreenName;
            LabelName.Text = this.info.Name;
            LabelLocation.Text = this.info.Location;
            this.SetLinklabelWeb(this.info.Url);
            DescriptionBrowser.Visible = false;
            this.MakeDescriptionBrowserText(this.info.Description);
            RecentPostBrowser.Visible = false;
            if (this.info.RecentPost != null)
            {
                this.recentPostTxt = this.owner.CreateDetailHtml(this.owner.TwitterInstance.CreateHtmlAnchor(ref this.info.RecentPost, this.atidList, this.userInfo.Status.Entities, null)
                    + " Posted at " + this.info.PostCreatedAt.ToString() + " via " + this.info.PostSource);
            }

            LinkLabelFollowing.Text = this.info.FriendsCount.ToString();
            LinkLabelFollowers.Text = this.info.FollowersCount.ToString();
            LinkLabelFav.Text = this.info.FavoriteCount.ToString();
            LinkLabelTweet.Text = this.info.StatusesCount.ToString();
            LabelCreatedAt.Text = this.info.CreatedAt.ToString();
            LabelIsProtected.Text = this.info.Protect ? Hoehoe.Properties.Resources.Yes : Hoehoe.Properties.Resources.No;
            LabelIsVerified.Text = this.info.Verified ? Hoehoe.Properties.Resources.Yes : Hoehoe.Properties.Resources.No;

            if (this.owner.TwitterInstance.Username == this.info.ScreenName)
            {
                ButtonEdit.Enabled = true;
                ChangeIconToolStripMenuItem.Enabled = true;
                ButtonBlock.Enabled = false;
                ButtonReportSpam.Enabled = false;
                ButtonBlockDestroy.Enabled = false;
            }
            else
            {
                ButtonEdit.Enabled = false;
                ChangeIconToolStripMenuItem.Enabled = false;
                ButtonBlock.Enabled = true;
                ButtonReportSpam.Enabled = true;
                ButtonBlockDestroy.Enabled = true;
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
                this.owner.OpenUriAsync(LinkLabelWeb.Text);
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
                MessageBox.Show(Hoehoe.Properties.Resources.FRMessage2 + ret);
            }
            else
            {
                MessageBox.Show(Hoehoe.Properties.Resources.FRMessage3);
                LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo1;
                ButtonFollow.Enabled = false;
                ButtonUnFollow.Enabled = true;
            }
        }

        private void ButtonUnFollow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + Hoehoe.Properties.Resources.ButtonUnFollow_ClickText1, Hoehoe.Properties.Resources.ButtonUnFollow_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string ret = this.owner.TwitterInstance.PostRemoveCommand(this.info.ScreenName);
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.FRMessage2 + ret);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.FRMessage3);
                    LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    ButtonFollow.Enabled = true;
                    ButtonUnFollow.Enabled = false;
                }
            }
        }

        private void ShowUserInfo_Activated(object sender, EventArgs e)
        {
            // 画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
            if (UserPicture.Image != null)
            {
                UserPicture.Invalidate(false);
            }
        }

        private void ShowUserInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserPicture.Image = null;
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
                    UserPicture.Image = this.icondata;
                }
            }
            catch (Exception)
            {
                UserPicture.Image = null;
            }

            if (this.owner.TwitterInstance.Username == this.info.ScreenName)
            {
                // 自分の場合
                LabelIsFollowing.Text = string.Empty;
                LabelIsFollowed.Text = string.Empty;
                ButtonFollow.Enabled = false;
                ButtonUnFollow.Enabled = false;
            }
            else
            {
                if (string.IsNullOrEmpty(this.friendshipResult))
                {
                    LabelIsFollowing.Text = this.info.IsFollowing ? Hoehoe.Properties.Resources.GetFriendshipInfo1 : Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    ButtonFollow.Enabled = !this.info.IsFollowing;
                    LabelIsFollowed.Text = this.info.IsFollowed ? Hoehoe.Properties.Resources.GetFriendshipInfo3 : Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    ButtonUnFollow.Enabled = this.info.IsFollowing;
                }
                else
                {
                    MessageBox.Show(this.friendshipResult);
                    ButtonUnFollow.Enabled = false;
                    ButtonFollow.Enabled = false;
                    LabelIsFollowed.Text = Hoehoe.Properties.Resources.GetFriendshipInfo6;
                    LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo6;
                }
            }
        }

        private void ShowUserInfo_Shown(object sender, EventArgs e)
        {
            DescriptionBrowser.DocumentText = this.descriptionTxt;
            DescriptionBrowser.Visible = true;
            if (this.info.RecentPost != null)
            {
                RecentPostBrowser.DocumentText = this.recentPostTxt;
                RecentPostBrowser.Visible = true;
            }
            else
            {
                LabelRecentPost.Text = Hoehoe.Properties.Resources.ShowUserInfo2;
            }

            ButtonClose.Focus();
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
                    if (AppendSettingDialog.Instance.OpenUserTimeline && m.Success && this.owner.IsTwitterId(m.Result("${ScreenName}")))
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
                ToolTip1.Show(browser.StatusText, this, PointToClient(MousePosition));
            }
            else if (string.IsNullOrEmpty(DescriptionBrowser.StatusText))
            {
                ToolTip1.Hide(this);
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                sc.Document.ExecCommand("SelectAll", false, null);
            }
        }

        private void SelectionCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string selectedText = this.owner.WebBrowser_GetSelectionText(ref sc);
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
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string selectedText = this.owner.WebBrowser_GetSelectionText(ref sc);
                if (selectedText == null)
                {
                    SelectionCopyToolStripMenuItem.Enabled = false;
                }
                else
                {
                    SelectionCopyToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void ShowUserInfo_MouseEnter(object sender, EventArgs e)
        {
            ToolTip1.Hide(this);
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
            if (UserPicture.Image != null)
            {
                string name = this.info.ImageUrl.ToString();
                this.owner.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
            }
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            UserPicture.Cursor = Cursors.Default;
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
                this.buttonEditText = ButtonEdit.Text;
                ButtonEdit.Text = Hoehoe.Properties.Resources.UserInfoButtonEdit_ClickText1;

                // 座標初期化,プロパティ設定
                TextBoxName.Location = LabelName.Location;
                TextBoxName.Height = LabelName.Height;
                TextBoxName.Width = LabelName.Width;
                TextBoxName.BackColor = this.owner.InputBackColor;
                TextBoxName.MaxLength = 20;
                TextBoxName.Text = LabelName.Text;
                TextBoxName.TabStop = true;
                TextBoxName.Visible = true;
                LabelName.Visible = false;

                TextBoxLocation.Location = LabelLocation.Location;
                TextBoxLocation.Height = LabelLocation.Height;
                TextBoxLocation.Width = LabelLocation.Width;
                TextBoxLocation.BackColor = this.owner.InputBackColor;
                TextBoxLocation.MaxLength = 30;
                TextBoxLocation.Text = LabelLocation.Text;
                TextBoxLocation.TabStop = true;
                TextBoxLocation.Visible = true;
                LabelLocation.Visible = false;

                TextBoxWeb.Location = LinkLabelWeb.Location;
                TextBoxWeb.Height = LinkLabelWeb.Height;
                TextBoxWeb.Width = LinkLabelWeb.Width;
                TextBoxWeb.BackColor = this.owner.InputBackColor;
                TextBoxWeb.MaxLength = 100;
                TextBoxWeb.Text = this.info.Url;
                TextBoxWeb.TabStop = true;
                TextBoxWeb.Visible = true;
                LinkLabelWeb.Visible = false;

                TextBoxDescription.Location = DescriptionBrowser.Location;
                TextBoxDescription.Height = DescriptionBrowser.Height;
                TextBoxDescription.Width = DescriptionBrowser.Width;
                TextBoxDescription.BackColor = this.owner.InputBackColor;
                TextBoxDescription.MaxLength = 160;
                TextBoxDescription.Text = this.info.Description;
                TextBoxDescription.Multiline = true;
                TextBoxDescription.ScrollBars = ScrollBars.Vertical;
                TextBoxDescription.TabStop = true;
                TextBoxDescription.Visible = true;
                DescriptionBrowser.Visible = false;

                TextBoxName.Focus();
                TextBoxName.Select(TextBoxName.Text.Length, 0);

                this.isEditing = true;
            }
            else
            {
                UpdateProfileArgs arg = new UpdateProfileArgs();

                if (TextBoxName.Modified || TextBoxLocation.Modified || TextBoxWeb.Modified || TextBoxDescription.Modified)
                {
                    arg.Tw = this.owner.TwitterInstance;
                    arg.Name = TextBoxName.Text.Trim();
                    arg.Url = TextBoxWeb.Text.Trim();
                    arg.Location = TextBoxLocation.Text.Trim();
                    arg.Description = TextBoxDescription.Text.Trim();

                    using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.UserInfoButtonEdit_ClickText2, this.UpdateProfile_Dowork, this.UpddateProfile_RunWorkerCompleted, arg))
                    {
                        dlg.ShowDialog();
                        if (!string.IsNullOrEmpty(dlg.Result.ToString()))
                        {
                            return;
                        }
                    }
                }

                LabelName.Text = TextBoxName.Text;
                this.info.Name = LabelName.Text;
                TextBoxName.TabStop = false;
                TextBoxName.Visible = false;
                LabelName.Visible = true;

                LabelLocation.Text = TextBoxLocation.Text;
                this.info.Location = LabelLocation.Text;
                TextBoxLocation.TabStop = false;
                TextBoxLocation.Visible = false;
                LabelLocation.Visible = true;

                this.SetLinklabelWeb(TextBoxWeb.Text);
                this.info.Url = TextBoxWeb.Text;
                TextBoxWeb.TabStop = false;
                TextBoxWeb.Visible = false;
                LinkLabelWeb.Visible = true;

                this.DescriptionBrowser.DocumentText = this.MakeDescriptionBrowserText(TextBoxDescription.Text);
                this.info.Description = TextBoxDescription.Text;
                TextBoxDescription.TabStop = false;
                TextBoxDescription.Visible = false;
                DescriptionBrowser.Visible = true;

                ButtonEdit.Text = this.buttonEditText;

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
                    UserPicture.Image = img;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialogIcon.Filter = Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText1;
            OpenFileDialogIcon.Title = Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText2;
            OpenFileDialogIcon.FileName = string.Empty;

            DialogResult rslt = OpenFileDialogIcon.ShowDialog();

            if (rslt != DialogResult.OK)
            {
                return;
            }

            string fn = OpenFileDialogIcon.FileName;
            if (this.IsValidIconFile(new FileInfo(fn)))
            {
                this.ChangeIcon(fn);
            }
            else
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText6);
            }
        }

        private void ButtonBlock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + Hoehoe.Properties.Resources.ButtonBlock_ClickText1, Hoehoe.Properties.Resources.ButtonBlock_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostCreateBlock(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Hoehoe.Properties.Resources.ButtonBlock_ClickText3);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ButtonBlock_ClickText4);
                }
            }
        }

        private void ButtonReportSpam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + Hoehoe.Properties.Resources.ButtonReportSpam_ClickText1, Hoehoe.Properties.Resources.ButtonReportSpam_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostReportSpam(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Hoehoe.Properties.Resources.ButtonReportSpam_ClickText3);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ButtonReportSpam_ClickText4);
                }
            }
        }

        private void ButtonBlockDestroy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.info.ScreenName + Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText1, Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = this.owner.TwitterInstance.PostDestroyBlock(this.info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText3);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText4);
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
            ToolTip1.SetToolTip(LinkLabelTweet, this.home);
            ToolTip1.SetToolTip(LinkLabelFollowing, this.following);
            ToolTip1.SetToolTip(LinkLabelFollowers, this.followers);
            ToolTip1.SetToolTip(LinkLabelFav, this.favorites);
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
            ToolTip1.SetToolTip(LinkLabelWeb, jumpto);
            LinkLabelWeb.Tag = jumpto;
            LinkLabelWeb.Text = data;
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

            using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText3, this.UpdateProfileImage_Dowork, this.UpdateProfileImage_RunWorkerCompleted, arg))
            {
                dlg.ShowDialog();
                res = dlg.Result as string;
                if (!string.IsNullOrEmpty(res))
                {
                    // "Err:"が付いたエラーメッセージが返ってくる
                    MessageBox.Show(res + System.Environment.NewLine + Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText4);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText5);
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