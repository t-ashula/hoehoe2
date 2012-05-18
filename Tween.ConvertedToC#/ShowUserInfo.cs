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
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace Hoehoe
{
    public partial class ShowUserInfo
    {
        private const string Mainpath = "http://twitter.com/";
        private const string Followingpath = "/following";
        private const string Followerspath = "/followers";
        private const string Favpath = "/favorites";

        private TwitterDataModel.User _userInfo;
        private UserInfo _info = new UserInfo();
        private Image _icondata;
        private List<string> _atList = new List<string>();
        private string _descriptionTxt;
        private string _recentPostTxt;
        private string _home;
        private string _following;
        private string _followers;
        private string _favorites;
        private TweenMain _owner;
        private string _friendshipResult = "";

        private void InitPath()
        {
            _home = Mainpath + _info.ScreenName;
            _following = _home + Followingpath;
            _followers = _home + Followerspath;
            _favorites = _home + Favpath;
        }

        private void InitTooltip()
        {
            ToolTip1.SetToolTip(LinkLabelTweet, _home);
            ToolTip1.SetToolTip(LinkLabelFollowing, _following);
            ToolTip1.SetToolTip(LinkLabelFollowers, _followers);
            ToolTip1.SetToolTip(LinkLabelFav, _favorites);
        }

        private bool AnalizeUserInfo(TwitterDataModel.User user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                _info.Id = user.Id;
                _info.Name = user.Name.Trim();
                _info.ScreenName = user.ScreenName;
                _info.Location = user.Location;
                _info.Description = user.Description;
                _info.ImageUrl = new Uri(user.ProfileImageUrl);
                _info.Url = user.Url;
                _info.Protect = user.Protected;
                _info.FriendsCount = user.FriendsCount;
                _info.FollowersCount = user.FollowersCount;
                _info.FavoriteCount = user.FavouritesCount;
                _info.CreatedAt = MyCommon.DateTimeParse(user.CreatedAt);
                _info.StatusesCount = user.StatusesCount;
                _info.Verified = user.Verified;
                try
                {
                    _info.RecentPost = user.Status.Text;
                    _info.PostCreatedAt = MyCommon.DateTimeParse(user.Status.CreatedAt);
                    _info.PostSource = user.Status.Source;
                    if (!_info.PostSource.Contains("</a>"))
                    {
                        _info.PostSource += "</a>";
                    }
                }
                catch (Exception)
                {
                    _info.RecentPost = null;
                    _info.PostSource = null;
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
            string webtext = _owner.TwitterInstance.PreProcessUrl("<a href=\"" + data + "\">Dummy</a>");
            webtext = ShortUrl.Resolve(webtext, false);
            string jumpto = Regex.Match(webtext, "<a href=\"(?<url>.*?)\"").Groups["url"].Value;
            ToolTip1.SetToolTip(LinkLabelWeb, jumpto);
            LinkLabelWeb.Tag = jumpto;
            LinkLabelWeb.Text = data;
        }

        private string MakeDescriptionBrowserText(string data)
        {
            _descriptionTxt = _owner.createDetailHtml(_owner.TwitterInstance.CreateHtmlAnchor(data, _atList, null));
            return _descriptionTxt;
        }

        private void ShowUserInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void ShowUserInfo_Load(object sender, EventArgs e)
        {
            _owner = (TweenMain)this.Owner;
            if (!AnalizeUserInfo(_userInfo))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ShowUserInfo1);
                this.Close();
                return;
            }

            //アイコンロード
            BackgroundWorkerImageLoader.RunWorkerAsync();

            InitPath();
            InitTooltip();
            this.Text = this.Text.Insert(0, _info.ScreenName + " ");
            LabelId.Text = _info.Id.ToString();
            LabelScreenName.Text = _info.ScreenName;
            LabelName.Text = _info.Name;

            LabelLocation.Text = _info.Location;

            SetLinklabelWeb(_info.Url);

            DescriptionBrowser.Visible = false;
            MakeDescriptionBrowserText(_info.Description);

            RecentPostBrowser.Visible = false;
            if (_info.RecentPost != null)
            {
                _recentPostTxt = _owner.createDetailHtml(_owner.TwitterInstance.CreateHtmlAnchor(ref _info.RecentPost, _atList, _userInfo.Status.Entities, null) + " Posted at " + _info.PostCreatedAt.ToString() + " via " + _info.PostSource);
            }

            LinkLabelFollowing.Text = _info.FriendsCount.ToString();
            LinkLabelFollowers.Text = _info.FollowersCount.ToString();
            LinkLabelFav.Text = _info.FavoriteCount.ToString();
            LinkLabelTweet.Text = _info.StatusesCount.ToString();

            LabelCreatedAt.Text = _info.CreatedAt.ToString();

            if (_info.Protect)
            {
                LabelIsProtected.Text = Hoehoe.Properties.Resources.Yes;
            }
            else
            {
                LabelIsProtected.Text = Hoehoe.Properties.Resources.No;
            }

            if (_info.Verified)
            {
                LabelIsVerified.Text = Hoehoe.Properties.Resources.Yes;
            }
            else
            {
                LabelIsVerified.Text = Hoehoe.Properties.Resources.No;
            }

            if (_owner.TwitterInstance.Username == _info.ScreenName)
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

        public void SetUser(TwitterDataModel.User value)
        {
            this._userInfo = value;
        }

        private void LinkLabelWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_info.Url != null)
            {
                _owner.OpenUriAsync(LinkLabelWeb.Text);
            }
        }

        private void LinkLabelFollowing_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _owner.OpenUriAsync(_following);
        }

        private void LinkLabelFollowers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _owner.OpenUriAsync(_followers);
        }

        private void LinkLabelTweet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _owner.OpenUriAsync(_home);
        }

        private void LinkLabelFav_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _owner.OpenUriAsync(_favorites);
        }

        private void ButtonFollow_Click(object sender, EventArgs e)
        {
            string ret = _owner.TwitterInstance.PostFollowCommand(_info.ScreenName);
            if (!String.IsNullOrEmpty(ret))
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
            if (MessageBox.Show(_info.ScreenName + Hoehoe.Properties.Resources.ButtonUnFollow_ClickText1, Hoehoe.Properties.Resources.ButtonUnFollow_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string ret = _owner.TwitterInstance.PostRemoveCommand(_info.ScreenName);
                if (!String.IsNullOrEmpty(ret))
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
            //画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
            if (UserPicture.Image != null)
            {
                UserPicture.Invalidate(false);
            }
        }

        private void ShowUserInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserPicture.Image = null;
            if (_icondata != null)
            {
                _icondata.Dispose();
            }
        }

        private void BackgroundWorkerImageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            string name = _info.ImageUrl.ToString();
            _icondata = (new HttpVarious()).GetImage(name.Replace("_normal", "_bigger"));
            if (_owner.TwitterInstance.Username == _info.ScreenName)
            {
                return;
            }

            _info.IsFollowing = false;
            _info.IsFollowed = false;
            _friendshipResult = _owner.TwitterInstance.GetFriendshipInfo(_info.ScreenName, ref _info.IsFollowing, ref _info.IsFollowed);
        }

        private void BackgroundWorkerImageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (_icondata != null)
                {
                    UserPicture.Image = _icondata;
                }
            }
            catch (Exception)
            {
                UserPicture.Image = null;
            }

            if (_owner.TwitterInstance.Username == _info.ScreenName)
            {
                // 自分の場合
                LabelIsFollowing.Text = "";
                LabelIsFollowed.Text = "";
                ButtonFollow.Enabled = false;
                ButtonUnFollow.Enabled = false;
            }
            else
            {
                if (String.IsNullOrEmpty(_friendshipResult))
                {
                    if (_info.IsFollowing)
                    {
                        LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo1;
                    }
                    else
                    {
                        LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    }
                    ButtonFollow.Enabled = !_info.IsFollowing;
                    if (_info.IsFollowed)
                    {
                        LabelIsFollowed.Text = Hoehoe.Properties.Resources.GetFriendshipInfo3;
                    }
                    else
                    {
                        LabelIsFollowed.Text = Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    }
                    ButtonUnFollow.Enabled = _info.IsFollowing;
                }
                else
                {
                    MessageBox.Show(_friendshipResult);
                    ButtonUnFollow.Enabled = false;
                    ButtonFollow.Enabled = false;
                    LabelIsFollowed.Text = Hoehoe.Properties.Resources.GetFriendshipInfo6;
                    LabelIsFollowing.Text = Hoehoe.Properties.Resources.GetFriendshipInfo6;
                }
            }
        }

        private void ShowUserInfo_Shown(object sender, EventArgs e)
        {
            DescriptionBrowser.DocumentText = _descriptionTxt;
            DescriptionBrowser.Visible = true;
            if (_info.RecentPost != null)
            {
                RecentPostBrowser.DocumentText = _recentPostTxt;
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
                    //ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(e.Url.AbsoluteUri);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    _owner.HashSupl.AddItem(hash);
                    _owner.HashMgr.AddHashToHistory(hash.Trim(), false);
                    _owner.AddNewTabForSearch(hash);
                    return;
                }
                else
                {
                    Match m = Regex.Match(e.Url.AbsoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (AppendSettingDialog.Instance.OpenUserTimeline && m.Success && _owner.IsTwitterId(m.Result("${ScreenName}")))
                    {
                        _owner.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                    else
                    {
                        _owner.OpenUriAsync(e.Url.OriginalString);
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
            else if (String.IsNullOrEmpty(DescriptionBrowser.StatusText))
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
                string _selText = _owner.WebBrowser_GetSelectionText(ref sc);
                if (_selText != null)
                {
                    try
                    {
                        Clipboard.SetDataObject(_selText, false, 5, 100);
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
                string _selText = _owner.WebBrowser_GetSelectionText(ref sc);
                if (_selText == null)
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
            _owner.OpenUriAsync("http://support.twitter.com/groups/31-twitter-basics/topics/111-features/articles/268350-x8a8d-x8a3c-x6e08-x307f-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _owner.OpenUriAsync("http://support.twitter.com/groups/31-twitter-basics/topics/107-my-profile-account-settings/articles/243055-x516c-x958b-x3001-x975e-x516c-x958b-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void ButtonSearchPosts_Click(object sender, EventArgs e)
        {
            _owner.AddNewTabForUserTimeline(_info.ScreenName);
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (UserPicture.Image != null)
            {
                string name = _info.ImageUrl.ToString();
                _owner.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
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

        private class UpdateProfileArgs
        {
            public Twitter Tw;
            public string Name;
            public string Location;
            public string Url;
            public string Description;
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

        bool isEditing = false;
        string buttonEditText = "";
        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            // 自分以外のプロフィールは変更できない
            if (_owner.TwitterInstance.Username != _info.ScreenName)
            {
                return;
            }

            if (!isEditing)
            {
                buttonEditText = ButtonEdit.Text;
                ButtonEdit.Text = Hoehoe.Properties.Resources.UserInfoButtonEdit_ClickText1;

                //座標初期化,プロパティ設定
                TextBoxName.Location = LabelName.Location;
                TextBoxName.Height = LabelName.Height;
                TextBoxName.Width = LabelName.Width;
                TextBoxName.BackColor = _owner.InputBackColor;
                TextBoxName.MaxLength = 20;
                TextBoxName.Text = LabelName.Text;
                TextBoxName.TabStop = true;
                TextBoxName.Visible = true;
                LabelName.Visible = false;

                TextBoxLocation.Location = LabelLocation.Location;
                TextBoxLocation.Height = LabelLocation.Height;
                TextBoxLocation.Width = LabelLocation.Width;
                TextBoxLocation.BackColor = _owner.InputBackColor;
                TextBoxLocation.MaxLength = 30;
                TextBoxLocation.Text = LabelLocation.Text;
                TextBoxLocation.TabStop = true;
                TextBoxLocation.Visible = true;
                LabelLocation.Visible = false;

                TextBoxWeb.Location = LinkLabelWeb.Location;
                TextBoxWeb.Height = LinkLabelWeb.Height;
                TextBoxWeb.Width = LinkLabelWeb.Width;
                TextBoxWeb.BackColor = _owner.InputBackColor;
                TextBoxWeb.MaxLength = 100;
                TextBoxWeb.Text = _info.Url;
                TextBoxWeb.TabStop = true;
                TextBoxWeb.Visible = true;
                LinkLabelWeb.Visible = false;

                TextBoxDescription.Location = DescriptionBrowser.Location;
                TextBoxDescription.Height = DescriptionBrowser.Height;
                TextBoxDescription.Width = DescriptionBrowser.Width;
                TextBoxDescription.BackColor = _owner.InputBackColor;
                TextBoxDescription.MaxLength = 160;
                TextBoxDescription.Text = _info.Description;
                TextBoxDescription.Multiline = true;
                TextBoxDescription.ScrollBars = ScrollBars.Vertical;
                TextBoxDescription.TabStop = true;
                TextBoxDescription.Visible = true;
                DescriptionBrowser.Visible = false;

                TextBoxName.Focus();
                TextBoxName.Select(TextBoxName.Text.Length, 0);

                isEditing = true;
            }
            else
            {
                UpdateProfileArgs arg = new UpdateProfileArgs();

                if (TextBoxName.Modified || TextBoxLocation.Modified || TextBoxWeb.Modified || TextBoxDescription.Modified)
                {
                    arg.Tw = _owner.TwitterInstance;
                    arg.Name = TextBoxName.Text.Trim();
                    arg.Url = TextBoxWeb.Text.Trim();
                    arg.Location = TextBoxLocation.Text.Trim();
                    arg.Description = TextBoxDescription.Text.Trim();

                    using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.UserInfoButtonEdit_ClickText2, UpdateProfile_Dowork, UpddateProfile_RunWorkerCompleted, arg))
                    {
                        dlg.ShowDialog();
                        if (!String.IsNullOrEmpty(dlg.Result.ToString()))
                        {
                            return;
                        }
                    }
                }

                LabelName.Text = TextBoxName.Text;
                _info.Name = LabelName.Text;
                TextBoxName.TabStop = false;
                TextBoxName.Visible = false;
                LabelName.Visible = true;

                LabelLocation.Text = TextBoxLocation.Text;
                _info.Location = LabelLocation.Text;
                TextBoxLocation.TabStop = false;
                TextBoxLocation.Visible = false;
                LabelLocation.Visible = true;

                SetLinklabelWeb(TextBoxWeb.Text);
                _info.Url = TextBoxWeb.Text;
                TextBoxWeb.TabStop = false;
                TextBoxWeb.Visible = false;
                LinkLabelWeb.Visible = true;

                DescriptionBrowser.DocumentText = MakeDescriptionBrowserText(TextBoxDescription.Text);
                _info.Description = TextBoxDescription.Text;
                TextBoxDescription.TabStop = false;
                TextBoxDescription.Visible = false;
                DescriptionBrowser.Visible = true;

                ButtonEdit.Text = buttonEditText;

                isEditing = false;
            }
        }

        public class UpdateProfileImageArgs
        {
            public Twitter Tw;
            public string FileName;
        }

        private void UpdateProfileImage_Dowork(object sender, DoWorkEventArgs e)
        {
            UpdateProfileImageArgs arg = (UpdateProfileImageArgs)e.Argument;
            e.Result = arg.Tw.PostUpdateProfileImage(arg.FileName);
        }

        private void UpdateProfileImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string res = "";
            TwitterDataModel.User user = null;

            if (e.Result == null)
            {
                return;
            }

            // アイコンを取得してみる
            // が、古いアイコンのユーザーデータが返ってくるため反映/判断できない

            try
            {
                res = _owner.TwitterInstance.GetUserInfo(_info.ScreenName, ref user);
                Image img = (new HttpVarious()).GetImage(user.ProfileImageUrl);
                if (img != null)
                {
                    UserPicture.Image = img;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void doChangeIcon(string filename)
        {
            string res = "";
            UpdateProfileImageArgs arg = new UpdateProfileImageArgs
            {
                Tw = _owner.TwitterInstance,
                FileName = filename
            };

            using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText3, UpdateProfileImage_Dowork, UpdateProfileImage_RunWorkerCompleted, arg))
            {
                dlg.ShowDialog();
                res = dlg.Result as string;
                if (!String.IsNullOrEmpty(res))
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

        private void ChangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialogIcon.Filter = Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText1;
            OpenFileDialogIcon.Title = Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText2;
            OpenFileDialogIcon.FileName = "";

            DialogResult rslt = OpenFileDialogIcon.ShowDialog();

            if (rslt != DialogResult.OK)
            {
                return;
            }

            string fn = OpenFileDialogIcon.FileName;
            if (isValidIconFile(new FileInfo(fn)))
            {
                doChangeIcon(fn);
            }
            else
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ChangeIconToolStripMenuItem_ClickText6);
            }
        }

        private void ButtonBlock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(_info.ScreenName + Hoehoe.Properties.Resources.ButtonBlock_ClickText1, Hoehoe.Properties.Resources.ButtonBlock_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = _owner.TwitterInstance.PostCreateBlock(_info.ScreenName);
                if (!String.IsNullOrEmpty(res))
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
            if (MessageBox.Show(_info.ScreenName + Hoehoe.Properties.Resources.ButtonReportSpam_ClickText1, Hoehoe.Properties.Resources.ButtonReportSpam_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = _owner.TwitterInstance.PostReportSpam(_info.ScreenName);
                if (!String.IsNullOrEmpty(res))
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
            if (MessageBox.Show(_info.ScreenName + Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText1, Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText2, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = _owner.TwitterInstance.PostDestroyBlock(_info.ScreenName);
                if (!String.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText3);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ButtonBlockDestroy_ClickText4);
                }
            }
        }

        private bool isValidExtension(string ext)
        {
            return ext.Equals(".jpg") || ext.Equals(".jpeg") || ext.Equals(".png") || ext.Equals(".gif");
        }

        private bool isValidIconFile(FileInfo info)
        {
            string ext = info.Extension.ToLower();
            return isValidExtension(ext) && info.Length < 700 * 1024 && !MyCommon.IsAnimatedGif(info.FullName);
        }

        private void ShowUserInfo_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])(e.Data.GetData(DataFormats.FileDrop, false)))[0];
                FileInfo fl = new FileInfo(filename);

                e.Effect = DragDropEffects.None;
                if (isValidIconFile(fl))
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
                string filename = ((string[])(e.Data.GetData(DataFormats.FileDrop, false)))[0];
                doChangeIcon(filename);
            }
        }

        public ShowUserInfo()
        {
            InitializeComponent();
        }
    }
}