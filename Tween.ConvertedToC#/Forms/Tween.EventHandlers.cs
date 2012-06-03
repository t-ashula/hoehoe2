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
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using Hoehoe.TweenCustomControl;

    partial class TweenMain
    {
        #region done

        private void SetupOperateContextMenu()
        {
            if (this.ListTab.SelectedTab == null)
            {
                return;
            }

            if (this.statuses == null || this.statuses.Tabs == null || !this.statuses.Tabs.ContainsKey(this.ListTab.SelectedTab.Text))
            {
                return;
            }

            bool existCurrentPost = this.ExistCurrentPost;
            this.ReplyStripMenuItem.Enabled = existCurrentPost;
            this.ReplyAllStripMenuItem.Enabled = existCurrentPost;
            this.DMStripMenuItem.Enabled = existCurrentPost;
            this.ShowProfileMenuItem.Enabled = existCurrentPost;
            this.ShowUserTimelineContextMenuItem.Enabled = existCurrentPost;
            this.ListManageUserContextToolStripMenuItem2.Enabled = existCurrentPost;
            this.MoveToFavToolStripMenuItem.Enabled = existCurrentPost;
            this.TabMenuItem.Enabled = existCurrentPost;
            this.IDRuleMenuItem.Enabled = existCurrentPost;
            this.UnreadStripMenuItem.Enabled = existCurrentPost;
            this.ReadedStripMenuItem.Enabled = existCurrentPost;

            TabUsageType selectedTabType = this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType;
            bool dmOrNotExist = selectedTabType == TabUsageType.DirectMessage || !existCurrentPost || this.curPost.IsDm;

            this.FavAddToolStripMenuItem.Enabled = !dmOrNotExist;
            this.FavRemoveToolStripMenuItem.Enabled = !dmOrNotExist;
            this.StatusOpenMenuItem.Enabled = !dmOrNotExist;
            this.FavorareMenuItem.Enabled = !dmOrNotExist;
            this.ShowRelatedStatusesMenuItem.Enabled = !dmOrNotExist;
            this.DeleteStripMenuItem.Text = !dmOrNotExist && this.curPost.IsRetweeted ? Hoehoe.Properties.Resources.DeleteMenuText2 : Hoehoe.Properties.Resources.DeleteMenuText1;
            this.DeleteStripMenuItem.Enabled = !dmOrNotExist ? this.curPost.IsMe : existCurrentPost && this.curPost.IsDm;
            this.ReTweetOriginalStripMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.FavoriteRetweetContextMenu.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.ReTweetStripMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe;
            this.QuoteStripMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe;
            this.FavoriteRetweetUnofficialContextMenu.Enabled = !dmOrNotExist && !this.curPost.IsMe;
            this.RepliedStatusOpenMenuItem.Enabled = existCurrentPost && selectedTabType != TabUsageType.PublicSearch && this.curPost.InReplyToStatusId > 0;
            this.MoveToRTHomeMenuItem.Enabled = existCurrentPost && this.curPost.IsRetweeted;
        }

        private void SetupPostBrowserContextMenu()
        {
            // URLコピーの項目の表示/非表示
            string postBrowserStatusText1 = this.PostBrowser.StatusText;
            bool isHttpUrl = postBrowserStatusText1.StartsWith("http");
            this.postBrowserStatusText = isHttpUrl ? postBrowserStatusText1 : string.Empty;
            this.UrlCopyContextMenuItem.Enabled = isHttpUrl;

            bool enable = isHttpUrl && !string.IsNullOrEmpty(this.GetUserId());
            this.FollowContextMenuItem.Enabled = enable;
            this.RemoveContextMenuItem.Enabled = enable;
            this.FriendshipContextMenuItem.Enabled = enable;
            this.ShowUserStatusContextMenuItem.Enabled = enable;
            this.SearchPostsDetailToolStripMenuItem.Enabled = enable;
            this.IdFilterAddMenuItem.Enabled = enable;
            this.ListManageUserContextToolStripMenuItem.Enabled = enable;
            this.SearchAtPostsDetailToolStripMenuItem.Enabled = enable;

            this.UseHashtagMenuItem.Enabled = isHttpUrl && Regex.IsMatch(this.postBrowserStatusText, "^https?://twitter.com/search\\?q=%23");

            // 文字列選択されてるときは選択文字列関係の項目を表示
            bool hasSelection = !string.IsNullOrEmpty(this.WebBrowser_GetSelectionText(ref this.PostBrowser));
            this.SelectionSearchContextMenuItem.Enabled = hasSelection;
            this.SelectionCopyContextMenuItem.Enabled = hasSelection;
            this.SelectionTranslationToolStripMenuItem.Enabled = hasSelection;

            // 発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            this.FriendshipAllMenuItem.Enabled = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"").Cast<Match>()
                .Select(m => m.Result("${ScreenName}").ToLower())
                .Any(s => s != this.tw.Username.ToLower());
            this.TranslationToolStripMenuItem.Enabled = this.curPost != null;
        }

        private void SetupPostModeContextMenu()
        {
            this.ToolStripMenuItemUrlAutoShorten.Checked = this.settingDialog.UrlConvertAuto;
        }

        private void SetupSourceContextMenu()
        {
            bool dmOrNotExist = this.curPost == null || !this.ExistCurrentPost || this.curPost.IsDm;
            this.SourceCopyMenuItem.Enabled = this.SourceUrlCopyMenuItem.Enabled = !dmOrNotExist;
        }

        private void SetupTabPropertyContextMenu(bool fromMenuBar)
        {
            // 右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (string.IsNullOrEmpty(this.rclickTabName) || fromMenuBar)
            {
                if (this.ListTab != null && this.ListTab.SelectedTab != null)
                {
                    this.rclickTabName = this.ListTab.SelectedTab.Text;
                }
                else
                {
                    return;
                }
            }

            if (this.statuses == null || this.statuses.Tabs == null)
            {
                return;
            }

            TabClass tb = this.statuses.Tabs[this.rclickTabName];
            if (tb == null)
            {
                return;
            }

            this.NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;

            this.soundfileListup = true;
            ReloadSoundSelector(this.SoundFileComboBox.ComboBox, tb.SoundFile);
            ReloadSoundSelector(this.SoundFileTbComboBox.ComboBox, tb.SoundFile);
            this.soundfileListup = false;

            this.UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            this.TabMenuControl(this.rclickTabName);
        }

        private static void ReloadSoundSelector(ComboBox soundFileComboBox, string currentSoundFile)
        {
            soundFileComboBox.Items.Clear();
            soundFileComboBox.Items.Add(string.Empty);
            var names = MyCommon.GetSoundFileNames();
            if (names.Length > 0)
            {
                soundFileComboBox.Items.AddRange(names);
            }

            int idx = soundFileComboBox.Items.IndexOf(currentSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            soundFileComboBox.SelectedIndex = idx;
        }

        /// <summary>
        /// 発言詳細のアイコン右クリック時のメニュー制御
        /// </summary>
        private void SetupUserPictureContextMenu()
        {
            var saveiconmenu = false;
            var iconmenu = false;
            var iconmenutxt = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText1;
            if (this.curList.SelectedIndices.Count <= 0 || this.curPost == null)
            {
                iconmenutxt = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText2;
            }
            else
            {
                string name = this.curPost.ImageUrl;
                if (!string.IsNullOrEmpty(name))
                {
                    saveiconmenu = this.iconDict[this.curPost.ImageUrl] != null;
                    int idx = name.LastIndexOf('/');
                    if (idx != -1)
                    {
                        name = Path.GetFileName(name.Substring(idx));
                        if (name.Contains("_normal.") || name.EndsWith("_normal"))
                        {
                            iconmenu = true;
                            iconmenutxt = name.Replace("_normal", string.Empty);
                        }
                    }
                }
            }

            this.SaveIconPictureToolStripMenuItem.Enabled = saveiconmenu;
            this.IconNameToolStripMenuItem.Enabled = iconmenu;
            this.IconNameToolStripMenuItem.Text = iconmenutxt;

            object tag = this.NameLabel.Tag;
            bool hasName = tag != null;
            this.ShowUserStatusToolStripMenuItem.Enabled = hasName;
            this.SearchPostsDetailNameToolStripMenuItem.Enabled = hasName;
            this.ListManageUserContextToolStripMenuItem3.Enabled = hasName;

            bool enable = hasName && (string)tag != this.tw.Username;
            this.FollowToolStripMenuItem.Enabled = enable;
            this.UnFollowToolStripMenuItem.Enabled = enable;
            this.ShowFriendShipToolStripMenuItem.Enabled = enable;
            this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = enable;
        }

        private void SetupCommandMenu()
        {
            this.RtCountMenuItem.Enabled = this.ExistCurrentPost && !this.curPost.IsDm;
        }

        private void SetupEditMenu()
        {
            this.UndoRemoveTabMenuItem.Enabled = this.statuses.RemovedTab.Count != 0;
            this.PublicSearchQueryMenuItem.Enabled = this.ListTab.SelectedTab != null && this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch;
            this.CopyUserIdStripMenuItem.Enabled = this.ExistCurrentPost;
            this.CopyURLMenuItem.Enabled = this.ExistCurrentPost && !this.curPost.IsDm;
            this.CopySTOTMenuItem.Enabled = this.ExistCurrentPost && !this.curPost.IsProtect;
        }

        private void SetupHelpMenu()
        {
            this.DebugModeToolStripMenuItem.Visible = MyCommon.DebugBuild || this.IsKeyDown(Keys.Control) && this.IsKeyDown(Keys.Shift);
        }

        private void SetupOperateMenu()
        {
            if (this.ListTab.SelectedTab == null
                || this.statuses == null
                || this.statuses.Tabs == null
                || !this.statuses.Tabs.ContainsKey(this.ListTab.SelectedTab.Text))
            {
                return;
            }

            this.ReplyOpMenuItem.Enabled = this.ExistCurrentPost;
            this.ReplyAllOpMenuItem.Enabled = this.ExistCurrentPost;
            this.DmOpMenuItem.Enabled = this.ExistCurrentPost;
            this.ShowProfMenuItem.Enabled = this.ExistCurrentPost;
            this.ShowUserTimelineToolStripMenuItem.Enabled = this.ExistCurrentPost;
            this.ListManageMenuItem.Enabled = this.ExistCurrentPost;
            this.OpenFavOpMenuItem.Enabled = this.ExistCurrentPost;
            this.CreateTabRuleOpMenuItem.Enabled = this.ExistCurrentPost;
            this.CreateIdRuleOpMenuItem.Enabled = this.ExistCurrentPost;
            this.ReadOpMenuItem.Enabled = this.ExistCurrentPost;
            this.UnreadOpMenuItem.Enabled = this.ExistCurrentPost;

            TabUsageType selectedTabType = this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType;
            bool dmOrNotExist = selectedTabType == TabUsageType.DirectMessage || !this.ExistCurrentPost || this.curPost.IsDm;
            this.FavOpMenuItem.Enabled = !dmOrNotExist;
            this.UnFavOpMenuItem.Enabled = !dmOrNotExist;
            this.OpenStatusOpMenuItem.Enabled = !dmOrNotExist;
            this.OpenFavotterOpMenuItem.Enabled = !dmOrNotExist;
            this.ShowRelatedStatusesMenuItem2.Enabled = !dmOrNotExist;

            this.DelOpMenuItem.Enabled = !dmOrNotExist ? this.curPost.IsMe : this.ExistCurrentPost && this.curPost.IsDm;
            this.RtOpMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.RtUnOpMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.QtOpMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.FavoriteRetweetMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;
            this.FavoriteRetweetUnofficialMenuItem.Enabled = !dmOrNotExist && !this.curPost.IsMe && !this.curPost.IsProtect;

            this.RefreshPrevOpMenuItem.Enabled = selectedTabType != TabUsageType.Favorites;
            this.OpenRepSourceOpMenuItem.Enabled = selectedTabType != TabUsageType.PublicSearch && this.ExistCurrentPost && this.curPost.InReplyToStatusId > 0;
            this.OpenRterHomeMenuItem.Enabled = this.ExistCurrentPost && !string.IsNullOrEmpty(this.curPost.RetweetedBy);
        }

        private void ShowAboutBox()
        {
            if (this.aboutBox == null)
            {
                this.aboutBox = new TweenAboutBox();
            }

            this.aboutBox.ShowDialog();
            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void ShowApiInfoBox()
        {
            GetApiInfoArgs args = new GetApiInfoArgs { Tw = this.tw, Info = new ApiInfo() };
            StringBuilder tmp = new StringBuilder();
            using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.ApiInfo6, this.GetApiInfo_Dowork, null, args))
            {
                dlg.ShowDialog();
                if ((bool)dlg.Result)
                {
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo1, args.Info.MaxCount));
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo2, args.Info.RemainCount));
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo3, args.Info.ResetTime));
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo7, (this.tw.UserStreamEnabled ? Hoehoe.Properties.Resources.Enable : Hoehoe.Properties.Resources.Disable)));
                    tmp.AppendLine();
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo8, args.Info.AccessLevel));
                    tmp.AppendLine();
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo9, (args.Info.MediaMaxCount < 0 ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaMaxCount.ToString())));
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo10, (args.Info.MediaRemainCount < 0 ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaRemainCount.ToString())));
                    tmp.AppendLine(string.Format("{0}{1}", Hoehoe.Properties.Resources.ApiInfo11, (args.Info.MediaResetTime == new DateTime() ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaResetTime.ToString())));
                    this.SetStatusLabelUrl();
                }
                else
                {
                    tmp.Append(Hoehoe.Properties.Resources.ApiInfo5);
                }
            }

            MessageBox.Show(tmp.ToString(), Hoehoe.Properties.Resources.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowCacheInfoBox()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine(string.Format("{0, -15} : {1}bytes ({2}MB)", "キャッシュメモリ容量", this.iconDict.CacheMemoryLimit, this.iconDict.CacheMemoryLimit / (1024 * 1024)));
            buf.AppendLine(string.Format("{0, -15} : {1}%", "物理メモリ使用割合", this.iconDict.PhysicalMemoryLimit));
            buf.AppendLine(string.Format("{0, -15} : {1}", "キャッシュエントリ保持数", this.iconDict.CacheCount));
            buf.AppendLine(string.Format("{0, -15} : {1}", "キャッシュエントリ破棄数", this.iconDict.CacheRemoveCount));
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void ShowEventViewerBox()
        {
            if (this.evtDialog == null || this.evtDialog.IsDisposed)
            {
                this.evtDialog = new EventViewerDialog();
                this.evtDialog.Owner = this;

                // 親の中央に表示
                Point pos = this.evtDialog.Location;
                pos.X = Convert.ToInt32(this.Location.X + ((this.Size.Width - this.evtDialog.Size.Width) / 2));
                pos.Y = Convert.ToInt32(this.Location.Y + ((this.Size.Height - this.evtDialog.Size.Height) / 2));
                this.evtDialog.Location = pos;
            }

            this.evtDialog.EventSource = this.tw.StoredEvent;
            if (!this.evtDialog.Visible)
            {
                this.evtDialog.Show(this);
            }
            else
            {
                this.evtDialog.Activate();
            }

            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void ShowPostImageFileSelectBox()
        {
            if (string.IsNullOrEmpty(this.ImageService))
            {
                return;
            }

            this.OpenFileDialog1.Filter = this.pictureServices[this.ImageService].GetFileOpenDialogFilter();
            this.OpenFileDialog1.Title = Hoehoe.Properties.Resources.PickPictureDialog1;
            this.OpenFileDialog1.FileName = string.Empty;

            try
            {
                this.AllowDrop = false;
                if (this.OpenFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
            }
            finally
            {
                this.AllowDrop = true;
            }

            this.ImagefilePathText.Text = this.OpenFileDialog1.FileName;
            this.LoadImageFromSelectedFile();
        }

        private void ShowFilterEditBox()
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                this.rclickTabName = this.statuses.GetTabByType(TabUsageType.Home).TabName;
            }

            this.fltDialog.SetCurrent(this.rclickTabName);
            this.fltDialog.ShowDialog();
            this.TopMost = this.settingDialog.AlwaysTop;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                this.statuses.FilterAll();
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }

                if (!this.settingDialog.TabIconDisp)
                {
                    this.ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            this.SaveConfigsTabs();
        }

        private void ShowFriendshipOfAllUserInCurrentTweet()
        {
            var ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            if (ma.Count > 0)
            {
                this.ShowFriendship(ma.Cast<Match>().Select(m => m.Result("${ScreenName}")).ToArray());
            }
        }

        private void ShowFriendshipOfCurrentTweetUser()
        {
            this.ShowFriendship(this.curPost == null ? string.Empty : this.curPost.ScreenName);
        }

        private void ShowFriendshipOfCurrentLinkUser()
        {
            this.ShowFriendship(this.GetUserId());
        }

        private void ShowFriendshipOfCurrentIconUser()
        {
            if (this.NameLabel.Tag != null)
            {
                this.ShowFriendship((string)this.NameLabel.Tag);
            }
        }

        private void ShowListManageBox()
        {
            using (ListManage form = new ListManage(this.tw))
            {
                form.ShowDialog(this);
            }
        }

        private void ShowCurrentTweetRtCountBox()
        {
            if (this.ExistCurrentPost)
            {
                using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.RtCountMenuItem_ClickText1, this.GetRetweet_DoWork))
                {
                    // ダイアログ表示
                    formInfo.ShowDialog();
                    int retweetCount = (int)formInfo.Result;
                    string msg = retweetCount < 0 ?
                        Hoehoe.Properties.Resources.RtCountText2 :
                        string.Format("{0}{1}", retweetCount, Hoehoe.Properties.Resources.RtCountText1);
                    MessageBox.Show(msg);
                }
            }
        }

        private void ShowtStatusOfCurrentLinkUser()
        {
            this.ShowUserStatus(this.GetUserId(), false);
        }

        private void ShowStatusOfCurrentIconUser()
        {
            if (this.NameLabel.Tag != null)
            {
                this.ShowUserStatus((string)this.NameLabel.Tag, false);
            }
        }

        private void ShowStatusOfCurrentTweetUser()
        {
            if (this.curPost != null)
            {
                this.ShowUserStatus(this.curPost.ScreenName, false);
            }
        }

        private void TryShowStatusOfCurrentTweetUser()
        {
            this.ShowUserStatus(this.curPost == null ? string.Empty : this.curPost.ScreenName);
        }

        private void ShowStatusOfUserSelf()
        {
            this.ShowUserStatus(this.tw.Username, false);
        }

        private void ShowHashManageBox()
        {
            try
            {
                DialogResult rslt = this.HashMgr.ShowDialog();
                this.TopMost = this.settingDialog.AlwaysTop;
                if (rslt == DialogResult.Cancel)
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            this.ChangeUseHashTagSetting(toggle: false);
        }

        private void TryShowSettingsBox()
        {
            DialogResult result = default(DialogResult);
            string uid = this.tw.Username.ToLower();

            try
            {
                result = this.settingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return;
            }

            if (result == DialogResult.OK)
            {
                lock (this.syncObject)
                {
                    this.tw.SetTinyUrlResolve(this.settingDialog.TinyUrlResolve);
                    this.tw.SetRestrictFavCheck(this.settingDialog.RestrictFavCheck);
                    this.tw.ReadOwnPost = this.settingDialog.ReadOwnPost;
                    this.tw.SetUseSsl(this.settingDialog.UseSsl);
                    ShortUrl.IsResolve = this.settingDialog.TinyUrlResolve;
                    ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;
                    ShortUrl.SetBitlyId(this.settingDialog.BitlyUser);
                    ShortUrl.SetBitlyKey(this.settingDialog.BitlyPwd);
                    HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
                    HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);

                    HttpConnection.InitializeConnection(this.settingDialog.DefaultTimeOut, this.settingDialog.SelectedProxyType, this.settingDialog.ProxyAddress, this.settingDialog.ProxyPort, this.settingDialog.ProxyUser, this.settingDialog.ProxyPassword);
                    this.CreatePictureServices();
                    try
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.Normal;
                            this.ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawItem += this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                            this.ListTab.ImageList = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        if (!this.settingDialog.UnreadManage)
                        {
                            this.ReadedStripMenuItem.Enabled = false;
                            this.UnreadStripMenuItem.Enabled = false;
                            if (this.settingDialog.TabIconDisp)
                            {
                                foreach (TabPage myTab in this.ListTab.TabPages)
                                {
                                    myTab.ImageIndex = -1;
                                }
                            }
                        }
                        else
                        {
                            this.ReadedStripMenuItem.Enabled = true;
                            this.UnreadStripMenuItem.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(UnreadManage)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage mytab in this.ListTab.TabPages)
                        {
                            DetailsListView lst = (DetailsListView)mytab.Tag;
                            lst.GridLines = this.settingDialog.ShowGrid;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(ShowGrid)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;
                    this.fntUnread = this.settingDialog.FontUnread;
                    this.clrUnread = this.settingDialog.ColorUnread;
                    this.fntReaded = this.settingDialog.FontReaded;
                    this.clrRead = this.settingDialog.ColorReaded;
                    this.clrFav = this.settingDialog.ColorFav;
                    this.clrOWL = this.settingDialog.ColorOWL;
                    this.clrRetweet = this.settingDialog.ColorRetweet;
                    this.fntDetail = this.settingDialog.FontDetail;
                    this.clrDetail = this.settingDialog.ColorDetail;
                    this.clrDetailLink = this.settingDialog.ColorDetailLink;
                    this.clrDetailBackcolor = this.settingDialog.ColorDetailBackcolor;
                    this.clrSelf = this.settingDialog.ColorSelf;
                    this.clrAtSelf = this.settingDialog.ColorAtSelf;
                    this.clrTarget = this.settingDialog.ColorTarget;
                    this.clrAtTarget = this.settingDialog.ColorAtTarget;
                    this.clrAtFromTarget = this.settingDialog.ColorAtFromTarget;
                    this.clrAtTo = this.settingDialog.ColorAtTo;
                    this.clrListBackcolor = this.settingDialog.ColorListBackcolor;
                    this.InputBackColor = this.settingDialog.ColorInputBackcolor;
                    this.clrInputForecolor = this.settingDialog.ColorInputFont;
                    this.fntInputFont = this.settingDialog.FontInputFont;
                    try
                    {
                        if (this.StatusText.Focused)
                        {
                            this.StatusText.BackColor = this.InputBackColor;
                        }

                        this.StatusText.Font = this.fntInputFont;
                        this.StatusText.ForeColor = this.clrInputForecolor;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    this.brsForeColorUnread.Dispose();
                    this.brsForeColorReaded.Dispose();
                    this.brsForeColorFav.Dispose();
                    this.brsForeColorOWL.Dispose();
                    this.brsForeColorRetweet.Dispose();
                    this.brsForeColorUnread = new SolidBrush(this.clrUnread);
                    this.brsForeColorReaded = new SolidBrush(this.clrRead);
                    this.brsForeColorFav = new SolidBrush(this.clrFav);
                    this.brsForeColorOWL = new SolidBrush(this.clrOWL);
                    this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                    this.brsBackColorMine.Dispose();
                    this.brsBackColorAt.Dispose();
                    this.brsBackColorYou.Dispose();
                    this.brsBackColorAtYou.Dispose();
                    this.brsBackColorAtFromTarget.Dispose();
                    this.brsBackColorAtTo.Dispose();
                    this.brsBackColorNone.Dispose();
                    this.brsBackColorMine = new SolidBrush(this.clrSelf);
                    this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
                    this.brsBackColorYou = new SolidBrush(this.clrTarget);
                    this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                    this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                    this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                    this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);
                    try
                    {
                        if (this.settingDialog.IsMonospace)
                        {
                            this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                            this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
                        }
                        else
                        {
                            this.detailHtmlFormatHeader = DetailHtmlFormat1;
                            this.detailHtmlFormatFooter = DetailHtmlFormat7;
                        }

                        this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                        if (this.settingDialog.IsMonospace)
                        {
                            this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
                        }
                        else
                        {
                            this.detailHtmlFormatHeader += DetailHtmlFormat6;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "Font";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        this.statuses.SetUnreadManage(this.settingDialog.UnreadManage);
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "_statuses";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage tb in this.ListTab.TabPages)
                        {
                            if (this.settingDialog.TabIconDisp)
                            {
                                if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                                {
                                    tb.ImageIndex = -1;
                                }
                                else
                                {
                                    tb.ImageIndex = 0;
                                }
                            }

                            if (tb.Tag != null && tb.Controls.Count > 0)
                            {
                                ((DetailsListView)tb.Tag).Font = this.fntReaded;
                                ((DetailsListView)tb.Tag).BackColor = this.clrListBackcolor;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp no2)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    this.SetMainWindowTitle();
                    this.SetNotifyIconText();

                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    this.ListTab.Refresh();

                    Outputz.Key = this.settingDialog.OutputzKey;
                    Outputz.Enabled = this.settingDialog.OutputzEnabled;
                    switch (this.settingDialog.OutputzUrlmode)
                    {
                        case OutputzUrlmode.twittercom:
                            Outputz.OutUrl = "http://twitter.com/";
                            break;
                        case OutputzUrlmode.twittercomWithUsername:
                            Outputz.OutUrl = "http://twitter.com/" + this.tw.Username;
                            break;
                    }

                    this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (this.settingDialog.HotkeyEnabled)
                    {
                        ///グローバルホットキーの登録。設定で変更可能にするかも
                        HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                        if ((this.settingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                        }

                        this.hookGlobalHotkey.RegisterOriginalHotkey(this.settingDialog.HotkeyKey, this.settingDialog.HotkeyValue, modKey);
                    }

                    if (uid != this.tw.Username)
                    {
                        this.DoGetFollowersMenu();
                    }

                    this.SetImageServiceCombo();
                    if (this.settingDialog.IsNotifyUseGrowl)
                    {
                        this.growlHelper.RegisterGrowl();
                    }

                    try
                    {
                        this.StatusText_TextChangedExtracted();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Twitter.AccountState = AccountState.Valid;

            this.TopMost = this.settingDialog.AlwaysTop;
            this.SaveConfigsAll(false);
        }

        private void SearchSelectedTextAtCurrentTab()
        {
            // 発言詳細の選択文字列で現在のタブを検索
            string txt = this.WebBrowser_GetSelectionText(ref this.PostBrowser);
            if (!string.IsNullOrEmpty(txt))
            {
                this.searchDialog.SWord = txt;
                this.searchDialog.CheckCaseSensitive = false;
                this.searchDialog.CheckRegex = false;
                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void TrySearchWordInTabToBottom()
        {
            // 次を検索
            if (string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                TrySearchWordInTab();
            }
            else
            {
                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void TrySearchWordInTabToTop()
        {
            // 前を検索
            if (string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                if (!TryGetSearchCondition())
                {
                    return;
                }
            }

            this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.PrevSearch);
        }

        private bool TryGetSearchCondition()
        {
            if (this.searchDialog.ShowDialog() == DialogResult.Cancel)
            {
                this.TopMost = this.settingDialog.AlwaysTop;
                return false;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                return false;
            }
            return true;
        }

        private void TrySearchWordInTab()
        {
            // 検索メニュー
            if (!TryGetSearchCondition())
            {
                return;
            }
            this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
        }

        private void TryFollowUserOfCurrentTweet()
        {
            this.FollowCommand(this.curPost != null ? this.curPost.ScreenName : string.Empty);
        }

        private void TryFollowUserOfCurrentLinkUser()
        {
            this.FollowCommand(this.GetUserId());
        }

        private void TryFollowUserOfCurrentIconUser()
        {
            if (this.NameLabel.Tag != null)
            {
                this.FollowCommand((string)this.NameLabel.Tag);
            }
        }
                
        private void DeleteSelectedTab(bool fromMenuBar)
        {
            if (string.IsNullOrEmpty(this.rclickTabName) || fromMenuBar)
            {
                this.rclickTabName = this.ListTab.SelectedTab.Text;
            }

            this.RemoveSpecifiedTab(this.rclickTabName, true);
            this.SaveConfigsTabs();
        }

        private void AddNewTab()
        {
            string tabName = null;
            TabUsageType tabUsage = default(TabUsageType);
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = this.statuses.GetUniqueTabName();
                inputName.SetIsShowUsage(true);
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return;
                }

                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            // List対応
            ListElement list = null;
            if (tabUsage == TabUsageType.Lists)
            {
                using (ListAvailable listAvail = new ListAvailable())
                {
                    if (listAvail.ShowDialog(this) == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (listAvail.SelectedList == null)
                    {
                        return;
                    }

                    list = listAvail.SelectedList;
                }
            }

            if (!this.statuses.AddTab(tabName, tabUsage, list) || !this.AddNewTab(tabName, false, tabUsage, list))
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                MessageBox.Show(tmp, Hoehoe.Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // 成功
            this.SaveConfigsTabs();
            if (tabUsage == TabUsageType.PublicSearch)
            {
                this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }

            if (tabUsage == TabUsageType.Lists)
            {
                this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                this.GetTimeline(WorkerType.List, 1, 0, tabName);
            }
        }

        private void ChangeAllrepliesSetting(bool useAllReply)
        {
            this.tw.AllAtReply = useAllReply;
            this.SetModifySettingCommon(true);
            this.tw.ReconnectUserStream();
        }
        
        private void OpenFavorarePageOfSelectedTweetUser()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[0]);
                this.OpenFavorarePageOfUser(post.ScreenName);
            }
        }

        private void OpenFavorarePageOfSelf()
        {
            this.OpenFavorarePageOfUser(this.tw.Username);
        }

        private void TryOpenFavorarePageOfCurrentTweetUser()
        {
            string id = this.GetUserIdFromCurPostOrInput("Show Favstar");
            if (!string.IsNullOrEmpty(id))
            {
                this.OpenFavorarePageOfUser(id);
            }
        }

        private void OpenFavorarePageOfUser(string id)
        {
            this.OpenUriAsync(string.Format("{0}users/{1}/recent", Hoehoe.Properties.Resources.FavstarUrl, id));
        }

        private void ExitApplication()
        {
            MyCommon.IsEnding = true;
            this.Close();
        }

        private void TryRestartApplication()
        {
            try
            {
                this.ExitApplication();
                Application.Restart();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to restart. Please run Tween manually.");
            }
        }

        private void DisplayTimelineWorkerProgressChanged(int progressPercentage, string msg)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            var progressMessage = this.StatusLabel.Text;
            if (progressPercentage > 100)
            {
                // 発言投稿
                if (progressPercentage == 200)
                {
                    // 開始
                    progressMessage = "Posting...";
                }

                if (progressPercentage == 300)
                {
                    // 終了
                    progressMessage = Hoehoe.Properties.Resources.PostWorker_RunWorkerCompletedText4;
                }
            }
            else
            {
                if (msg.Length > 0)
                {
                    progressMessage = msg;
                }
            }

            this.StatusLabel.Text = progressMessage;
        }

        private void ChangeUseHashTagSetting(bool toggle = true)
        {
            if (toggle)
            {
                this.HashMgr.ToggleHash();
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                this.HashStripSplitButton.Text = "#[-]";
                this.HashToggleMenuItem.Checked = false;
                this.HashToggleToolStripMenuItem.Checked = false;
            }

            this.modifySettingCommon = true;
            this.StatusText_TextChangedExtracted();
        }
        
        private void ChangeWindowState()
        {
            if ((this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
                && this.Visible && object.ReferenceEquals(Form.ActiveForm, this))
            {
                // アイコン化
                this.Visible = false;
            }
            else if (Form.ActiveForm == null)
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }

                this.Activate();
                this.BringToFront();
                this.StatusText.Focus();
            }
        }

        private void AddIdFilteringRuleFromSelectedTweets()
        {
            // 未選択なら処理終了
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            var names = this.curList.SelectedIndices.Cast<int>()
                .Select(idx => this.statuses.Item(this.curTab.Text, idx))
                .Select(pc => pc.IsRetweeted ? pc.RetweetedBy : pc.ScreenName);
            this.TryAddIdsFilter(names);
        }

        private void ApplyNewFilters()
        {
            this.itemCache = null;
            this.postCache = null;
            this.curPost = null;
            this.curItemIndex = -1;
            this.statuses.FilterAll();
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.ContainsTab(tb.Text))
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        private void AddIdFilteringRuleFromCurrentTweet()
        {
            TryAddIdFilter(this.GetUserId());
        }

        private void TryAddIdFilter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            this.TryAddIdsFilter(new[] { name });
        }

        private void TryAddIdsFilter(IEnumerable<string> names)
        {
            var uniNames = names.Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).Distinct();
            if (uniNames.Count() < 1)
            {
                return;
            }

            // タブ選択（or追加）
            string tabName = string.Empty;
            if (!this.SelectTab(ref tabName))
            {
                return;
            }

            bool mv = false;
            bool mk = false;
            this.MoveOrCopy(ref mv, ref mk);

            this.statuses.Tabs[tabName].AddFilters(names.Select(name => new FiltersClass()
            {
                NameFilter = name,
                SearchBoth = true,
                MoveFrom = mv,
                SetMark = mk,
                UseRegex = false,
                SearchUrl = false
            }));
            this.SetModifySettingAtId(this.AtIdSupl.AddRangeItem(names.Select(name => "@" + name)));

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.ApplyNewFilters();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            this.SaveConfigsTabs();
        }
        
        private void TryOpenCurrentTweetIconUrl()
        {
            if (this.curPost == null)
            {
                return;
            }

            this.OpenUriAsync(this.curPost.NormalImageUrl);
        }

        private void CancelPostImageSelecting()
        {
            this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
            this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
            this.ImagefilePathText.CausesValidation = false;
            this.TimelinePanel.Visible = true;
            this.TimelinePanel.Enabled = true;
            this.ImageSelectionPanel.Visible = false;
            this.ImageSelectionPanel.Enabled = false;
            ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
            this.ImagefilePathText.CausesValidation = true;
        }

        private void ToggleImageSelectorView()
        {
            if (this.ImageSelectionPanel.Visible)
            {
                CancelPostImageSelecting();
            }
            else
            {
                this.ImageSelectionPanel.Visible = true;
                this.ImageSelectionPanel.Enabled = true;
                this.TimelinePanel.Visible = false;
                this.TimelinePanel.Enabled = false;
                this.ImagefilePathText.Focus();
            }
        }

        private void TryChangeImageUploadService()
        {
            if (this.ImageSelectedPicture.Tag == null || string.IsNullOrEmpty(this.ImageService))
            {
                return;
            }

            try
            {
                FileInfo fi = new FileInfo(this.ImagefilePathText.Text.Trim());
                if (!this.pictureServices[this.ImageService].CheckValidFilesize(fi.Extension, fi.Length))
                {
                    this.ImagefilePathText.Text = string.Empty;
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                }
            }
            catch (Exception)
            {
            }

            this.modifySettingCommon = true;
            this.SaveConfigsAll(false);
            if (this.ImageService == "Twitter")
            {
                this.StatusText_TextChangedExtracted();
            }
        }

        private void TrySearchAndFocusUnreadTweet()
        {
            if (this.ImageSelectionPanel.Enabled)
            {
                return;
            }

            // 現在タブから最終タブまで探索
            int idx = -1;
            DetailsListView lst = null;
            TabControl.TabPageCollection pages = this.ListTab.TabPages;
            foreach (var i in Enumerable.Range(0, pages.Count).Select(i => (i + pages.IndexOf(this.curTab)) % pages.Count))
            {
                // 未読Index取得
                idx = this.statuses.GetOldestUnreadIndex(pages[i].Text);
                if (idx > -1)
                {
                    this.ListTab.SelectedIndex = i;
                    lst = (DetailsListView)pages[i].Tag;
                    break;
                }
            }

            // 全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                this.ListTab.SelectedIndex = 0;
                lst = (DetailsListView)pages[0].Tag;
                if (this.statuses.SortOrder == SortOrder.Ascending)
                {
                    idx = lst.VirtualListSize - 1;
                }
                else
                {
                    idx = 0;
                }
            }

            if (lst.VirtualListSize > 0 && idx > -1 && lst.VirtualListSize > idx)
            {
                this.SelectListItem(lst, idx);
                if (this.statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if ((this.statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - this.iconSz - 10)
                        || (this.statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < this.iconSz + 10))
                    {
                        this.MoveTop();
                    }
                    else
                    {
                        lst.EnsureVisible(idx);
                    }
                }
                else
                {
                    lst.EnsureVisible(idx);
                }
            }

            lst.Focus();
        }

        private void ChangeListLockSetting(bool locked)
        {
            this.cfgCommon.ListLock = this.LockListFileMenuItem.Checked = this.ListLockMenuItem.Checked = locked;
            this.SetModifySettingCommon(true);
        }

        private void ShowListSelectFormForCurrentTweetUser()
        {
            if (this.curPost != null)
            {
                ShowListSelectForm(this.curPost.ScreenName);
            }
        }

        private void ShowListSelectForm(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            if (this.statuses.SubscribableLists.Count == 0)
            {
                string res = this.tw.GetListsApi();
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Failed to get lists. (" + res + ")");
                    return;
                }
            }

            using (MyLists listSelectForm = new MyLists(user, this.tw))
            {
                listSelectForm.ShowDialog(this);
            }
        }
     
        #endregion done

        #region event handler

        #region cleanuped

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTab();
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeAllrepliesSetting(this.AllrepliesToolStripMenuItem.Checked);
        }

        private void ApiInfoMenuItem_Click(object sender, EventArgs e)
        {
            ShowApiInfoBox();
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Bitly);
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            ShowCacheInfoBox();
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearTab(this.rclickTabName, true);
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            this.SetupOperateContextMenu();
        }

        private void ContextMenuPostBrowser_Opening(object sender, CancelEventArgs e)
        {
            SetupPostBrowserContextMenu();
            e.Cancel = false;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            SetupPostModeContextMenu();
        }

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            SetupSourceContextMenu();
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            SetupTabPropertyContextMenu(fromMenuBar: false);
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            this.SetupUserPictureContextMenu();
        }

        private void CopySTOTMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyStot();
        }

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyIdUri();
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyUserId();
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchSelectedTextAtCurrentTab();
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, false);
        }

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoStatusDelete();
        }
        
        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedTab(fromMenuBar: false);
        }

        private void DeleteTbMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedTab(fromMenuBar: true);
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(this.displayItem))
            {
                this.UserPicture.ReplaceImage(this.displayItem.Image);
            }
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DispSelectedPost(true);
        }
        
        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowEventViewerBox();
        }

        private void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoriteChange(true);
        }

        private void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoriteChange(false);
        }
        
        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            OpenFavorarePageOfSelectedTweetUser();
        }

        private void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetOriginal();
        }

        private void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetUnofficial();
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            this.ShowPostImageFileSelectBox();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFilterEditBox();
        }

        private void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            TryFollowUserOfCurrentTweet();
        }

        private void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            TryFollowUserOfCurrentLinkUser();
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryFollowUserOfCurrentIconUser();
        }

        private void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfAllUserInCurrentTweet();
        }

        private void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfCurrentLinkUser();
        }

        private void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfCurrentTweetUser();
        }

        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoGetFollowersMenu();
        }

        private void GetTimelineWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.DisplayTimelineWorkerProgressChanged(e.ProgressPercentage, (string)e.UserState);
        }

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowHashManageBox();
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            this.ChangeUseHashTagSetting();
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeUseHashTagSetting();
        }

        private void HookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            this.ChangeWindowState();
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            this.AddIdFilteringRuleFromSelectedTweets();
        }

        private void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenCurrentTweetIconUrl();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            this.AddIdFilteringRuleFromCurrentTweet();
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetModifySettingCommon(true);
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
        {
            this.CancelPostImageSelecting();
        }

        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            this.ToggleImageSelectorView();
        }

        private void ImageSelectionPanel_VisibleChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChangedExtracted();
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                CancelPostImageSelecting();
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 0x1b)
            {
                this.ImagefilePathText.CausesValidation = false;
                e.Handled = true;
            }
        }

        private void ImageSelection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ImagefilePathText.CausesValidation = false;
            }
        }

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryChangeImageUploadService();
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (this.ImageCancelButton.Focused)
            {
                this.ImagefilePathText.CausesValidation = false;
                return;
            }

            this.ImagefilePathText.Text = this.ImagefilePathText.Text.Trim();
            if (string.IsNullOrEmpty(this.ImagefilePathText.Text))
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
            }
            else
            {
                this.LoadImageFromSelectedFile();
            }
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Isgd);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Jmp);
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (this.ImageCancelButton.Focused)
            {
                this.ImagefilePathText.CausesValidation = false;
                return;
            }

            this.ImagefilePathText.Text = this.ImagefilePathText.Text.Trim();
            if (string.IsNullOrEmpty(this.ImagefilePathText.Text))
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
            }
            else
            {
                this.LoadImageFromSelectedFile();
            }
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Isgd);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Jmp);
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            TrySearchAndFocusUnreadTweet();
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ChangeListLockSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowListManageBox();
        }

        private void ListManageUserContextToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageMenuItem_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowListSelectForm(this.GetUserId());
        }

        #endregion

        private void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationHelpWebPageUrl);
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            SetupCommandMenu();
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            SetupEditMenu();
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            SetupHelpMenu();
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            SetupOperateMenu();
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            TrySearchWordInTabToBottom();
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            TrySearchWordInTabToTop();
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            TrySearchWordInTab();
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupTabPropertyContextMenu(true);
        }

        private void SetFocusToMainMenu()
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            this.MenuStrip1.Tag = new object();
            this.MenuStrip1.Select();
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            SetFocusToMainMenu();
        }

        private void SetFocusFromMainMenu()
        {
            if (this.Tag != null)
            {
                // 設定された戻り先へ遷移
                if (object.ReferenceEquals(this.Tag, this.ListTab.SelectedTab))
                {
                    ((Control)this.ListTab.SelectedTab.Tag).Select();
                }
                else
                {
                    ((Control)this.Tag).Select();
                }
            }
            else
            {
                // 戻り先が指定されていない (初期状態) 場合はタブに遷移
                if (this.ListTab.SelectedIndex > -1 && this.ListTab.SelectedTab.HasChildren)
                {
                    this.Tag = this.ListTab.SelectedTab.Tag;
                    ((Control)this.Tag).Select();
                }
            }

            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            this.MenuStrip1.Tag = null;
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            SetFocusFromMainMenu();
        }

        private void TryOpenCurListSelectedUserFavorites()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("https://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName + "/favorites");
            }
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenCurListSelectedUserFavorites();
        }

        private void TryOpenCurListSelectedUserHome()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("https://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName);
            }
            else if (this.curList.SelectedIndices.Count == 0)
            {
                this.OpenUriAsync("https://twitter.com/");
            }
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenCurListSelectedUserHome();
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.DoMoveToRTHome();
        }

        private void ChangeStatusTextMultilineState(bool multi)
        {
            // 発言欄複数行
            this.StatusText.Multiline = multi;
            this.cfgLocal.StatusMultiline = multi;
            int baseHeight = this.SplitContainer2.Height - this.SplitContainer2.SplitterWidth;
            baseHeight -= multi ? this.mySpDis2 : this.SplitContainer2.Panel2MinSize;

            this.SplitContainer2.SplitterDistance = baseHeight < 0 ? 0 : baseHeight;
            this.SetModifySettingLocal(true);
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (this.itemCache != null && e.StartIndex >= this.itemCacheIndex && e.EndIndex < this.itemCacheIndex + this.itemCache.Length && this.curList.Equals(sender))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            if (this.curList.Equals(sender))
            {
                this.CreateCache(e.StartIndex, e.EndIndex);
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.settingDialog.SortOrderLock)
            {
                return;
            }

            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            if (this.iconCol)
            {
                mode = IdComparerClass.ComparerMode.Id;
            }
            else
            {
                switch (e.Column)
                {
                    case 0:
                    case 5:
                    case 6:
                        // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                        // ソートしない
                        return;
                    case 1:
                        // ニックネーム
                        mode = IdComparerClass.ComparerMode.Nickname;
                        break;
                    case 2:
                        // 本文
                        mode = IdComparerClass.ComparerMode.Data;
                        break;
                    case 3:
                        // 時刻=発言Id
                        mode = IdComparerClass.ComparerMode.Id;
                        break;
                    case 4:
                        // 名前
                        mode = IdComparerClass.ComparerMode.Name;
                        break;
                    case 7:
                        // Source
                        mode = IdComparerClass.ComparerMode.Source;
                        break;
                }
            }

            this.statuses.ToggleSortOrder(mode);
            this.InitColumnText();

            if (this.iconCol)
            {
                ((DetailsListView)sender).Columns[0].Text = this.columnOrgTexts[0];
                ((DetailsListView)sender).Columns[1].Text = this.columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    ((DetailsListView)sender).Columns[i].Text = this.columnOrgTexts[i];
                }

                ((DetailsListView)sender).Columns[e.Column].Text = this.columnTexts[e.Column];
            }

            this.itemCache = null;
            this.postCache = null;

            if (this.statuses.Tabs[this.curTab.Text].AllCount > 0 && this.curPost != null)
            {
                int idx = this.statuses.Tabs[this.curTab.Text].IndexOf(this.curPost.StatusId);
                if (idx > -1)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                }
            }

            this.curList.Refresh();
            this.modifySettingCommon = true;
        }

        private void MyList_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (this.cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (this.iconCol)
            {
                this.cfgLocal.Width1 = lst.Columns[0].Width;
                this.cfgLocal.Width3 = lst.Columns[1].Width;
            }
            else
            {
                int[] darr = new int[lst.Columns.Count];
                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    darr[lst.Columns[i].DisplayIndex] = i;
                }

                MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    switch (darr[i])
                    {
                        case 0:
                            this.cfgLocal.DisplayIndex1 = i;
                            break;
                        case 1:
                            this.cfgLocal.DisplayIndex2 = i;
                            break;
                        case 2:
                            this.cfgLocal.DisplayIndex3 = i;
                            break;
                        case 3:
                            this.cfgLocal.DisplayIndex4 = i;
                            break;
                        case 4:
                            this.cfgLocal.DisplayIndex5 = i;
                            break;
                        case 5:
                            this.cfgLocal.DisplayIndex6 = i;
                            break;
                        case 6:
                            this.cfgLocal.DisplayIndex7 = i;
                            break;
                        case 7:
                            this.cfgLocal.DisplayIndex8 = i;
                            break;
                    }
                }

                this.cfgLocal.Width1 = lst.Columns[0].Width;
                this.cfgLocal.Width2 = lst.Columns[1].Width;
                this.cfgLocal.Width3 = lst.Columns[2].Width;
                this.cfgLocal.Width4 = lst.Columns[3].Width;
                this.cfgLocal.Width5 = lst.Columns[4].Width;
                this.cfgLocal.Width6 = lst.Columns[5].Width;
                this.cfgLocal.Width7 = lst.Columns[6].Width;
                this.cfgLocal.Width8 = lst.Columns[7].Width;
            }

            this.modifySettingLocal = true;
            this.isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (this.iconCol)
            {
                if (this.cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this.cfgLocal.Width1 = lst.Columns[0].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    this.cfgLocal.Width3 = lst.Columns[1].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }
            }
            else
            {
                if (this.cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this.cfgLocal.Width1 = lst.Columns[0].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    this.cfgLocal.Width2 = lst.Columns[1].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    this.cfgLocal.Width3 = lst.Columns[2].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    this.cfgLocal.Width4 = lst.Columns[3].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    this.cfgLocal.Width5 = lst.Columns[4].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    this.cfgLocal.Width6 = lst.Columns[5].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    this.cfgLocal.Width7 = lst.Columns[6].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    this.cfgLocal.Width8 = lst.Columns[7].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }
            }
        }

        private void MyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void MyList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.State == 0)
            {
                return;
            }
            e.DrawDefault = false;
            if (e.Item.Selected)
            {
                // 選択中の行
                SolidBrush brs1 = ((Control)sender).Focused ? this.brsHighLight : this.brsDeactiveSelection;
                e.Graphics.FillRectangle(brs1, e.Bounds);
            }
            else
            {
                var cl = e.Item.BackColor;
                SolidBrush brs2 = (cl == this.clrSelf) ? this.brsBackColorMine : (cl == this.clrAtSelf) ? this.brsBackColorAt : (cl == this.clrTarget) ? this.brsBackColorYou : (cl == this.clrAtTarget) ? this.brsBackColorAtYou : (cl == this.clrAtFromTarget) ? this.brsBackColorAtFromTarget : (cl == this.clrAtTo) ? this.brsBackColorAtTo : this.brsBackColorNone;
                e.Graphics.FillRectangle(brs2, e.Bounds);
            }
            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                e.DrawFocusRectangle();
            }
            this.DrawListViewItemIcon(e);
        }

        private void MyList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ItemState == 0)
            {
                return;
            }
            if (e.ColumnIndex < 1)
            {
                return;
            }
            // アイコン以外の列
            RectangleF rct = e.Bounds;
            RectangleF rctB = e.Bounds;
            rct.Width = e.Header.Width;
            rctB.Width = e.Header.Width;
            if (this.iconCol)
            {
                rct.Y += e.Item.Font.Height;
                rct.Height -= e.Item.Font.Height;
                rctB.Height = e.Item.Font.Height;
            }
            int heightDiff = 0;
            int drawLineCount = Math.Max(1, Math.DivRem(Convert.ToInt32(rct.Height), e.Item.Font.Height, out heightDiff));
            // フォントの高さの半分を足してるのは保険。無くてもいいかも。
            if (!this.iconCol && drawLineCount <= 1)
            {
            }
            else
                if (heightDiff < e.Item.Font.Height * 0.7)
                {
                    rct.Height = Convert.ToSingle(e.Item.Font.Height * drawLineCount) - 1;
                }
                else
                {
                    drawLineCount += 1;
                }
            if (!e.Item.Selected)
            {
                // 選択されていない行
                // 文字色
                SolidBrush brs = null;
                bool flg = false;
                var cl = e.Item.ForeColor;
                if (cl == this.clrUnread)
                {
                    brs = this.brsForeColorUnread;
                }
                else
                    if (cl == this.clrRead)
                    {
                        brs = this.brsForeColorReaded;
                    }
                    else
                        if (cl == this.clrFav)
                        {
                            brs = this.brsForeColorFav;
                        }
                        else
                            if (cl == this.clrOWL)
                            {
                                brs = this.brsForeColorOWL;
                            }
                            else
                                if (cl == this.clrRetweet)
                                {
                                    brs = this.brsForeColorRetweet;
                                }
                                else
                                {
                                    brs = new SolidBrush(e.Item.ForeColor);
                                    flg = true;
                                }
                if (rct.Width > 0)
                {
                    if (this.iconCol)
                    {
                        using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                        {
                            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), brs.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                        }
                    }
                    else
                        if (drawLineCount == 1)
                        {
                            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                        }
                        else
                        {
                            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                        }
                }
                if (flg)
                {
                    brs.Dispose();
                }
            }
            else
            {
                if (rct.Width > 0)
                {
                    // 選択中の行
                    using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                    {
                        if (((Control)sender).Focused)
                        {
                            if (this.iconCol)
                            {
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this.brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                            }
                            else
                                if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                        }
                        else
                        {
                            if (this.iconCol)
                            {
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this.brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                            }
                            else
                                if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                        }
                    }
                }
            }
        }

        private void MyList_HScrolled(object sender, EventArgs e)
        {
            ((DetailsListView)sender).Refresh();
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            this.anchorFlag = false;
        }

        private void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (this.settingDialog.ListDoubleClickAction)
            {
                case 0:
                    this.MakeReplyOrDirectStatus();
                    break;
                case 1:
                    this.FavoriteChange(true);
                    break;
                case 2:
                    this.ShowStatusOfCurrentTweetUser();
                    break;
                case 3:
                    this.ShowUserTimeline();
                    break;
                case 4:
                    this.AddRelatedStatusesTab();
                    break;
                case 5:
                    this.TryOpenCurListSelectedUserHome();
                    break;
                case 6:
                    this.TryOpenSelectedTweetWebPage();
                    break;
                case 7:
                    // 動作なし
                    break;
            }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this.itemCache != null && e.ItemIndex >= this.itemCacheIndex && e.ItemIndex < this.itemCacheIndex + this.itemCache.Length && this.curList.Equals(sender))
            {
                // A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = this.itemCache[e.ItemIndex - this.itemCacheIndex];
            }
            else
            {
                // A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((Hoehoe.TweenCustomControl.DetailsListView)sender).Parent;
                try
                {
                    e.Item = this.CreateItem(tb, this.statuses.Item(tb.Text, e.ItemIndex), e.ItemIndex);
                }
                catch (Exception)
                {
                    // 不正な要求に対する間に合わせの応答
                    e.Item = new ImageListViewItem(new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }, string.Empty);
                }
            }
        }

        private void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.curList == null || this.curList.SelectedIndices.Count != 1)
            {
                return;
            }

            this.curItemIndex = this.curList.SelectedIndices[0];
            if (this.curItemIndex > this.curList.VirtualListSize - 1)
            {
                return;
            }

            try
            {
                this.curPost = this.GetCurTabPost(this.curItemIndex);
            }
            catch (ArgumentException)
            {
                return;
            }

            this.PushSelectPostChain();

            if (this.settingDialog.UnreadManage)
            {
                this.statuses.SetReadAllTab(true, this.curTab.Text, this.curItemIndex);
            }

            // キャッシュの書き換え
            this.ChangeCacheStyleRead(true, this.curItemIndex, this.curTab);

            // 既読へ（フォント、文字色）
            this.ColorizeList();
            this.colorize = true;
        }

        private void ChangeNewPostPopupSetting(bool popup)
        {
            this.cfgCommon.NewAllPop = this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked = popup;
            this.SetModifySettingCommon(true);
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ChangeNewPostPopupSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void ChangeNotifySetting(bool notify)
        {
            this.NotifyTbMenuItem.Checked = this.NotifyDispMenuItem.Checked = notify;
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.statuses.Tabs[this.rclickTabName].Notify = notify;
            this.SaveConfigsTabs();
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            ChangeNotifySetting(((ToolStripMenuItem)sender).Checked);
        }

        private void ActivateMainForm()
        {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
            this.BringToFront();
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            ActivateMainForm();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ActivateMainForm();
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            this.SetNotifyIconText();
        }

        private void OpenOwnFavedMenuItem_Click(object sender, EventArgs e)
        {
            OpenFavorarePageOfSelf();
        }

        private void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync("https://twitter.com/" + this.tw.Username);
        }

        private void TryOpenUrlInCurrentTweet()
        {
            if (this.PostBrowser.Document.Links.Count > 0)
            {
                this.urlDialog.ClearUrl();
                string openUrlStr = string.Empty;
                if (this.PostBrowser.Document.Links.Count == 1)
                {
                    try
                    {
                        string urlStr = MyCommon.IDNDecode(this.PostBrowser.Document.Links[0].GetAttribute("href"));
                        if (string.IsNullOrEmpty(urlStr))
                        {
                            return;
                        }

                        openUrlStr = MyCommon.GetUrlEncodeMultibyteChar(urlStr);
                    }
                    catch (ArgumentException)
                    {
                        // 変なHTML？
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                else
                {
                    foreach (HtmlElement linkElm in this.PostBrowser.Document.Links)
                    {
                        string urlStr = string.Empty;
                        string linkText = string.Empty;
                        string href = string.Empty;
                        try
                        {
                            urlStr = linkElm.GetAttribute("title");
                            href = MyCommon.IDNDecode(linkElm.GetAttribute("href"));
                            if (string.IsNullOrEmpty(urlStr))
                            {
                                urlStr = href;
                            }

                            linkText = linkElm.InnerText;
                            if (!linkText.StartsWith("http") && !linkText.StartsWith("#") && !linkText.Contains("."))
                            {
                                linkText = "@" + linkText;
                            }
                        }
                        catch (ArgumentException)
                        {
                            // 変なHTML？
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(urlStr))
                        {
                            continue;
                        }

                        this.urlDialog.AddUrl(new OpenUrlItem(linkText, MyCommon.GetUrlEncodeMultibyteChar(urlStr), href));
                    }

                    try
                    {
                        if (this.urlDialog.ShowDialog() == DialogResult.OK)
                        {
                            openUrlStr = this.urlDialog.SelectedUrl;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    this.TopMost = this.settingDialog.AlwaysTop;
                }

                if (string.IsNullOrEmpty(openUrlStr))
                {
                    return;
                }

                if (openUrlStr.StartsWith("http://twitter.com/search?q=") || openUrlStr.StartsWith("https://twitter.com/search?q="))
                {
                    // ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(openUrlStr);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    this.HashSupl.AddItem(hash);
                    this.HashMgr.AddHashToHistory(hash.Trim(), false);
                    this.AddNewTabForSearch(hash);
                    return;
                }

                if (this.settingDialog.OpenUserTimeline)
                {
                    Match m = Regex.Match(openUrlStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
                    {
                        this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                }
                else
                {
                    this.OpenUriAsync(openUrlStr);
                }
            }
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenUrlInCurrentTweet();
        }

        private void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUserAppointUrl();
        }

        private void OwnStatusMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfUserSelf();
        }

        private void ChangePlaySoundSetting(bool play)
        {
            this.settingDialog.PlaySound = this.PlaySoundFileMenuItem.Checked = this.PlaySoundMenuItem.Checked = play;
            this.modifySettingCommon = true;
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ChangePlaySoundSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void PostBrowser_NavigatedExtracted(Uri eUrl)
        {
            if (eUrl.AbsoluteUri != "about:blank")
            {
                this.DispSelectedPost();
                this.OpenUriAsync(eUrl.OriginalString);
            }
        }

        private void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            PostBrowser_NavigatedExtracted(e.Url);
        }

        private bool NavigateNextUrl(Uri url)
        {
            if (url.Scheme == "data")
            {
                this.StatusLabelUrl.Text = this.PostBrowser.StatusText.Replace("&", "&&");
                return true;
            }

            string eUrlAbsoluteUri = url.AbsoluteUri;
            if (eUrlAbsoluteUri == "about:blank")
            {
                return false;
            }

            if (eUrlAbsoluteUri.StartsWith("http://twitter.com/search?q=%23") || eUrlAbsoluteUri.StartsWith("https://twitter.com/search?q=%23"))
            {
                // ハッシュタグの場合は、タブで開く
                string urlStr = HttpUtility.UrlDecode(eUrlAbsoluteUri);
                string hash = urlStr.Substring(urlStr.IndexOf("#"));
                this.HashSupl.AddItem(hash);
                this.HashMgr.AddHashToHistory(hash.Trim(), false);
                this.AddNewTabForSearch(hash);
                return true;
            }

            Match m = Regex.Match(eUrlAbsoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
            string eUrlOriginalString = url.OriginalString;
            if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
            {
                // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
                bool isCtrlKeyDown = this.IsKeyDown(Keys.Control);
                bool isOpenInTab = this.settingDialog.OpenUserTimeline;
                if (isOpenInTab)
                {
                    if (isCtrlKeyDown)
                    {
                        this.OpenUriAsync(eUrlOriginalString);
                    }
                    else
                    {
                        this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                }
                else
                {
                    if (isCtrlKeyDown)
                    {
                        this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                    else
                    {
                        this.OpenUriAsync(eUrlOriginalString);
                    }
                }
            }
            else
            {
                this.OpenUriAsync(eUrlOriginalString);
            }
            return true;
        }

        private void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = NavigateNextUrl(e.Url);
        }

        private void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            bool res = this.CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, modState);
            if (res)
            {
                e.IsInputKey = true;
            }
        }

        private void ChangeStatusLabelUrlTextByPostBrowserStatusText()
        {
            try
            {
                string postBrowserStatusText1 = this.PostBrowser.StatusText;
                if (postBrowserStatusText1.StartsWith("http")
                    || postBrowserStatusText1.StartsWith("ftp")
                    || postBrowserStatusText1.StartsWith("data"))
                {
                    this.StatusLabelUrl.Text = postBrowserStatusText1.Replace("&", "&&");
                }

                if (string.IsNullOrEmpty(postBrowserStatusText1))
                {
                    this.SetStatusLabelUrl();
                }
            }
            catch (Exception)
            {
            }
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            ChangeStatusLabelUrlTextByPostBrowserStatusText();
        }

        private void TryPostTweet()
        {
            if (this.StatusText.Text.Trim().Length == 0)
            {
                if (!this.ImageSelectionPanel.Enabled)
                {
                    this.DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && this.StatusText.Text.Trim() == string.Format("RT @{0}: {1}", this.curPost.ScreenName, this.curPost.TextFromApi))
            {
                DialogResult res = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.PostButton_Click1, Environment.NewLine), "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        this.DoReTweetOfficial(false);
                        this.StatusText.Text = string.Empty;
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            this.postHistory[this.postHistory.Count - 1] = new PostingStatus(this.StatusText.Text.Trim(), this.replyToId, this.replyToName);

            if (this.settingDialog.Nicoms)
            {
                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                this.ConvertUrl(UrlConverter.Nicoms);
            }

            this.StatusText.SelectionStart = this.StatusText.Text.Length;
            GetWorkerArg args = new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = WorkerType.PostMessage };
            this.CheckReplyTo(this.StatusText.Text);

            // 整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = this.StatusText.Text.Trim();
            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !tmpStatus.EndsWith(" ."))
                {
                    adjustCount += 2;
                }
            }

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                adjustCount += Regex.Matches(tmpStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+").Count;
            }

            bool isCutOff = false;
            bool isRemoveFooter = this.IsKeyDown(Keys.Shift);
            if (this.StatusText.Multiline && !this.settingDialog.PostCtrlEnter)
            {
                // 複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = this.IsKeyDown(Keys.Control);
            }

            if (this.settingDialog.PostShiftEnter)
            {
                isRemoveFooter = this.IsKeyDown(Keys.Control);
            }

            if (!isRemoveFooter && (this.StatusText.Text.Contains("RT @") || this.StatusText.Text.Contains("QT @")))
            {
                isRemoveFooter = true;
            }

            if (this.GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
            {
                if (MessageBox.Show(Hoehoe.Properties.Resources.PostLengthOverMessage1, Hoehoe.Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    isCutOff = true;
                    if (this.GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
                    {
                        isRemoveFooter = true;
                    }
                }
                else
                {
                    return;
                }
            }

            string footer = string.Empty;
            string header = string.Empty;
            if (this.StatusText.Text.StartsWith("D ") || this.StatusText.Text.StartsWith("d "))
            {
                // DM時は何もつけない
                footer = string.Empty;
            }
            else
            {
                // ハッシュタグ
                if (this.HashMgr.IsNotAddToAtReply)
                {
                    if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.replyToId == 0 && string.IsNullOrEmpty(this.replyToName))
                    {
                        if (this.HashMgr.IsHead)
                        {
                            header = this.HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + this.HashMgr.UseHash;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
                    {
                        if (this.HashMgr.IsHead)
                        {
                            header = this.HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + this.HashMgr.UseHash;
                        }
                    }
                }

                if (!isRemoveFooter)
                {
                    if (this.settingDialog.UseRecommendStatus)
                    {
                        // 推奨ステータスを使用する
                        footer += this.settingDialog.RecommendStatusText;
                    }
                    else
                    {
                        // テキストボックスに入力されている文字列を使用する
                        footer += " " + this.settingDialog.Status.Trim();
                    }
                }
            }

            args.PStatus.Status = header + this.StatusText.Text.Trim() + footer;

            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(args.PStatus.Status, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !args.PStatus.Status.EndsWith(" ."))
                {
                    args.PStatus.Status += " .";
                }
            }

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+");
                if (mc2.Success)
                {
                    args.PStatus.Status = Regex.Replace(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+", "$& ");
                }
            }

            if (this.IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                args.PStatus.Status = args.PStatus.Status.Replace("　", " ");
            }

            if (isCutOff && args.PStatus.Status.Length > 140)
            {
                args.PStatus.Status = args.PStatus.Status.Substring(0, 140);
                const string AtId = "(@|＠)[a-z0-9_/]+$";
                const string HashTag = "(^|[^0-9A-Z&\\/\\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$";
                const string Url = "https?:\\/\\/[a-z0-9!\\*'\\(\\);:&=\\+\\$\\/%#\\[\\]\\-_\\.,~?]+$";

                // 簡易判定
                string pattern = string.Format("({0})|({1})|({2})", AtId, HashTag, Url);
                Match mc = Regex.Match(args.PStatus.Status, pattern, RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    // さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                    args.PStatus.Status = args.PStatus.Status.Substring(0, 140 - mc.Value.Length);
                }

                if (MessageBox.Show(args.PStatus.Status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            args.PStatus.InReplyToId = this.replyToId;
            args.PStatus.InReplyToName = this.replyToName;
            if (this.ImageSelectionPanel.Visible)
            {
                // 画像投稿
                if (!object.ReferenceEquals(this.ImageSelectedPicture.Image, this.ImageSelectedPicture.InitialImage) && this.ImageServiceCombo.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ImagefilePathText.Text))
                {
                    if (MessageBox.Show(Hoehoe.Properties.Resources.PostPictureConfirm1, Hoehoe.Properties.Resources.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        this.TimelinePanel.Visible = true;
                        this.TimelinePanel.Enabled = true;
                        this.ImageSelectionPanel.Visible = false;
                        this.ImageSelectionPanel.Enabled = false;
                        if (this.curList != null)
                        {
                            this.curList.Focus();
                        }

                        return;
                    }

                    args.PStatus.ImageService = this.ImageServiceCombo.Text;
                    args.PStatus.ImagePath = this.ImagefilePathText.Text;
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImagefilePathText.Text = string.Empty;
                    this.TimelinePanel.Visible = true;
                    this.TimelinePanel.Enabled = true;
                    this.ImageSelectionPanel.Visible = false;
                    this.ImageSelectionPanel.Enabled = false;
                    if (this.curList != null)
                    {
                        this.curList.Focus();
                    }
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.PostPictureWarn1, Hoehoe.Properties.Resources.PostPictureWarn2);
                    return;
                }
            }

            this.RunAsync(args);

            // Google検索（試験実装）
            if (this.StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && this.StatusText.Text.Trim().Length > 7)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.SearchItem2Url, HttpUtility.UrlEncode(this.StatusText.Text.Substring(7)));
                this.OpenUriAsync(tmp);
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
            this.StatusText.Text = string.Empty;
            this.postHistory.Add(new PostingStatus());
            this.postHistoryIndex = this.postHistory.Count - 1;
            if (!this.ToolStripFocusLockMenuItem.Checked)
            {
                ((Control)this.ListTab.SelectedTab.Tag).Focus();
            }

            this.urlUndoBuffer = null;
            this.UrlUndoToolStripMenuItem.Enabled = false; // Undoをできないように設定
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            TryPostTweet();
        }

        private void FocusCurrentPublicSearchTabSearchInput()
        {
            if (this.ListTab.SelectedTab != null)
            {
                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType != TabUsageType.PublicSearch)
                {
                    return;
                }

                this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            FocusCurrentPublicSearchTabSearchInput();
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoQuote();
        }

        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetOfficial(true);
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetUnofficial();
        }

        private void ChangeSelectetdTweetReadStateToRead()
        {
            this.curList.BeginUpdate();
            if (this.settingDialog.UnreadManage)
            {
                foreach (int idx in this.curList.SelectedIndices)
                {
                    this.statuses.SetReadAllTab(true, this.curTab.Text, idx);
                }
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(true, idx, this.curTab);
            }

            this.ColorizeList();
            this.curList.EndUpdate();
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this.settingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSelectetdTweetReadStateToRead();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            // もっと前を取得
            this.DoRefreshMore();
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRefresh();
        }

        private void TryUnfollowCurrentTweetUser()
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            this.RemoveCommand(id, false);
        }

        private void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            TryUnfollowCurrentTweetUser();
        }

        private void TryUnfollowUserInCurrentTweet()
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.RemoveCommand(name, false);
            }
        }

        private void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            TryUnfollowUserInCurrentTweet();
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRepliedStatusOpen();
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true, true);
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true);
        }

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            ShowCurrentTweetRtCountBox();
        }

        private void TrySaveCurrentTweetUserIcon()
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);
            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(this.iconDict[name]))
                    {
                        using (Bitmap bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bmp2))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                            }

                            bmp2.Save(this.SaveFileDialog1.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 処理中にキャッシュアウトする可能性あり
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrySaveCurrentTweetUserIcon();
        }

        private void TrySaveLog()
        {
            DialogResult rslt = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine), Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            this.SaveFileDialog1.FileName = string.Format("HoehoePosts{0:yyMMdd-HHmmss}.tsv", DateTime.Now);
            this.SaveFileDialog1.InitialDirectory = MyCommon.AppDir;
            this.SaveFileDialog1.Filter = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText3;
            this.SaveFileDialog1.FilterIndex = 0;
            this.SaveFileDialog1.Title = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText4;
            this.SaveFileDialog1.RestoreDirectory = true;

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!this.SaveFileDialog1.ValidateNames)
                {
                    return;
                }

                using (StreamWriter sw = new StreamWriter(this.SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    if (rslt == DialogResult.Yes)
                    {
                        // All
                        for (int idx = 0; idx <= this.curList.VirtualListSize - 1; idx++)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }

                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}", post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""), post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl, post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }
                    else
                    {
                        foreach (int idx in this.curList.SelectedIndices)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }

                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}", post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""), post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl, post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }

                    sw.Close();
                }
            }

            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            TrySaveLog();
        }

        private void SaveCurrentTweetUserOriginalSizeIcon()
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));
            this.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8);
            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // STUB
            }
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentTweetUserOriginalSizeIcon();
        }

        private void TryAddSearchTabForAtUserOfCurrentTweet()
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                this.AddNewTabForSearch("@" + id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryAddSearchTabForAtUserOfCurrentTweet();
        }

        private void TryAddSearchTabForAtUserInCurrentTweet()
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.AddNewTabForSearch("@" + name);
            }
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryAddSearchTabForAtUserInCurrentTweet();
        }

        private void SearchButton_ClickExtracted(Control pnl)
        {
            string tabName = pnl.Parent.Text;
            TabClass tb = this.statuses.Tabs[tabName];
            ComboBox cmb = (ComboBox)pnl.Controls["comboSearch"];
            ComboBox cmbLang = (ComboBox)pnl.Controls["comboLang"];
            ComboBox cmbusline = (ComboBox)pnl.Controls["comboUserline"];
            cmb.Text = cmb.Text.Trim();

            // TODO: confirm this-> 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
            bool inQuote = false;
            StringBuilder buf = new StringBuilder();
            char[] c = cmb.Text.ToCharArray();
            for (int cnt = 0; cnt < cmb.Text.Length; cnt++)
            {
                if (cnt > cmb.Text.Length - 4)
                {
                    buf.Append(cmb.Text.Substring(cnt));
                    break;
                }

                if (c[cnt] == Convert.ToChar("\""))
                {
                    inQuote = !inQuote;
                }
                else
                {
                    if (!inQuote && cmb.Text.Substring(cnt, 4).Equals(" or ", StringComparison.OrdinalIgnoreCase))
                    {
                        buf.Append(" OR ");
                        cnt += 3;
                        continue;
                    }
                }

                buf.Append(c[cnt]);
            }

            cmb.Text = buf.ToString();

            tb.SearchWords = cmb.Text;
            tb.SearchLang = cmbLang.Text;
            if (string.IsNullOrEmpty(cmb.Text))
            {
                ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
                this.SaveConfigsTabs();
                return;
            }

            if (tb.IsQueryChanged())
            {
                int idx = ((ComboBox)pnl.Controls["comboSearch"]).Items.IndexOf(tb.SearchWords);
                if (idx > -1)
                {
                    ((ComboBox)pnl.Controls["comboSearch"]).Items.RemoveAt(idx);
                }

                ((ComboBox)pnl.Controls["comboSearch"]).Items.Insert(0, tb.SearchWords);
                cmb.Text = tb.SearchWords;
                cmb.SelectAll();
                DetailsListView lst = (DetailsListView)pnl.Parent.Tag;
                lst.VirtualListSize = 0;
                lst.Items.Clear();
                this.statuses.ClearTabIds(tabName);
                this.SaveConfigsTabs(); // 検索条件の保存
            }

            this.GetTimeline(WorkerType.PublicSearch, 1, 0, tabName);
            ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }

            SearchButton_ClickExtracted(pnl);
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                TabPage relTp = this.ListTab.SelectedTab;
                this.RemoveSpecifiedTab(relTp.Text, false);
                this.SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        private void ChangeSearchPanelControlsTabStop(Control pnl, bool newVariable)
        {
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = newVariable;
            }
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            ChangeSearchPanelControlsTabStop((Control)sender, true);
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            ChangeSearchPanelControlsTabStop((Control)sender, false);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem2Url);
        }

        private void TryAddTimelineTabForCurrentTweetUser()
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                this.AddNewTabForUserTimeline(id);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryAddTimelineTabForCurrentTweetUser();
        }

        private void TryAddTimelineTabForUserInCurrentTweet()
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.AddNewTabForUserTimeline(name);
            }
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryAddTimelineTabForUserInCurrentTweet();
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem4Url);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem1Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem3Url);
        }

        private void SelectAllItemInFocused()
        {
            if (this.StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                this.StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                this.curList.SelectAllItem();
            }
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllItemInFocused();
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細ですべて選択
            this.PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private void TryCopyCurrentTweetSelection()
        {
            try
            {
                Clipboard.SetDataObject(this.WebBrowser_GetSelectionText(ref this.PostBrowser), false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            TryCopyCurrentTweetSelection();
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoTranslation(this.WebBrowser_GetSelectionText(ref this.PostBrowser));
        }

        private void SetStatusLabelApiHandler(object sender, ApiInformationChangedEventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new SetStatusLabelApiDelegate(this.SetStatusLabelApi));
                }
                else
                {
                    this.SetStatusLabelApi();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            TryShowSettingsBox();
        }

        private void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationShortcutKeyHelpWebPageUrl);
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfCurrentIconUser();
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfCurrentTweetUser();
        }

        private void AddRelatedStatusesTab()
        {
            TabClass backToTab = this.curTab == null ? this.statuses.Tabs[this.ListTab.SelectedTab.Text] : this.statuses.Tabs[this.curTab.Text];
            if (this.ExistCurrentPost && !this.curPost.IsDm)
            {
                // PublicSearchも除外した方がよい？
                if (this.statuses.GetTabByType(TabUsageType.Related) == null)
                {
                    const string TabName = "Related Tweets";
                    string newTabName = TabName;
                    if (!this.AddNewTab(newTabName, false, TabUsageType.Related))
                    {
                        for (int i = 2; i <= 100; i++)
                        {
                            newTabName = TabName + i.ToString();
                            if (this.AddNewTab(newTabName, false, TabUsageType.Related))
                            {
                                this.statuses.AddTab(newTabName, TabUsageType.Related, null);
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.statuses.AddTab(newTabName, TabUsageType.Related, null);
                    }

                    this.statuses.GetTabByName(newTabName).UnreadManage = false;
                    this.statuses.GetTabByName(newTabName).Notify = false;
                }

                TabClass tb = this.statuses.GetTabByType(TabUsageType.Related);
                tb.RelationTargetPost = this.curPost;
                this.ClearTab(tb.TabName, false);
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (tb.TabName == this.ListTab.TabPages[i].Text)
                    {
                        this.ListTab.SelectedIndex = i;
                        this.ListTabSelect(this.ListTab.TabPages[i]);
                        break;
                    }
                }

                this.GetTimeline(WorkerType.Related, 1, 1, tb.TabName);
            }
        }

        private void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            AddRelatedStatusesTab();
        }

        private void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            ShowtStatusOfCurrentLinkUser();
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfCurrentIconUser();
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowUserTimeline();
        }

        private void ChangeCurrentTabSoundFile(string soundfile)
        {
            if (this.soundfileListup || string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.statuses.Tabs[this.rclickTabName].SoundFile = soundfile;
            this.SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeCurrentTabSoundFile((string)((ToolStripComboBox)sender).SelectedItem);
        }

        private void TryCopySourceName()
        {
            string selText = this.SourceLinkLabel.Text;
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            TryCopySourceName();
        }

        private void TryOpenSourceLink()
        {
            string link = (string)this.SourceLinkLabel.Tag;
            if (!string.IsNullOrEmpty(link))
            {
                this.OpenUriAsync(link);
            }
        }

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TryOpenSourceLink();
            }
        }

        private void ChangeStatusLabelUrlText(string link, bool updateEmpty = false)
        {
            if (string.IsNullOrEmpty(link))
            {
                if (updateEmpty)
                {
                    this.StatusLabelUrl.Text = string.Empty;
                }
            }
            else
            {
                this.StatusLabelUrl.Text = link;
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            ChangeStatusLabelUrlText((string)this.SourceLinkLabel.Tag);
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            this.SetStatusLabelUrl();
        }

        private void TryCopySourceUrl()
        {
            string selText = Convert.ToString(this.SourceLinkLabel.Tag);
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            TryCopySourceUrl();
        }

        private void SpaceKeyCanceler_SpaceCancel(object sender, EventArgs e)
        {
            this.TrySearchAndFocusUnreadTweet();
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.mySpDis = this.SplitContainer1.SplitterDistance;
                if (this.StatusText.Multiline)
                {
                    this.mySpDis2 = this.StatusText.Height;
                }

                this.modifySettingLocal = true;
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MultiLineMenuItem.PerformClick();
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            this.StatusText.Multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            this.MultiLineMenuItem.Checked = this.StatusText.Multiline;
            this.modifySettingLocal = true;
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.StatusText.Multiline)
            {
                this.mySpDis2 = this.StatusText.Height;
            }

            this.modifySettingLocal = true;
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                this.modifySettingLocal = true;
            }
        }

        private void SplitContainer4_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            if (this.SplitContainer4.Panel2Collapsed)
            {
                return;
            }

            if (this.SplitContainer4.Height < this.SplitContainer4.SplitterWidth + this.SplitContainer4.Panel2MinSize + this.SplitContainer4.SplitterDistance && this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - this.SplitContainer4.Panel2MinSize > 0)
            {
                this.SplitContainer4.SplitterDistance = this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - this.SplitContainer4.Panel2MinSize;
            }

            if (this.SplitContainer4.Panel2.Height > 90 && this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - 90 > 0)
            {
                this.SplitContainer4.SplitterDistance = this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - 90;
            }
        }

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myAdSpDis = this.SplitContainer4.SplitterDistance;
                this.modifySettingLocal = true;
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(this.StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void TryOpenSelectedTweetWebPage()
        {
            if (this.curList.SelectedIndices.Count > 0 && this.statuses.Tabs[this.curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[0]);
                this.OpenUriAsync(string.Format("https://twitter.com/{0}/status/{1}", post.ScreenName, post.OriginalStatusId));
            }
        }

        private void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenSelectedTweetWebPage();
        }

        private void StatusText_EnterExtracted()
        {
            /// フォーカスの戻り先を StatusText に設定
            this.Tag = this.StatusText;
            this.StatusText.BackColor = this.InputBackColor;
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            this.StatusText_EnterExtracted();
        }

        private void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            if (this.CommonKeyDown(e.KeyCode, FocusedControl.StatusText, modState))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            this.StatusText_TextChangedExtracted();
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                // @マーク
                if (!this.settingDialog.UseAtIdSupplement)
                {
                    return;
                }

                int cnt = this.AtIdSupl.ItemCount;
                this.ShowSuplDialog(this.StatusText, this.AtIdSupl);
                if (cnt != this.AtIdSupl.ItemCount)
                {
                    this.modifySettingAtId = true;
                }

                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!this.settingDialog.UseHashSupplement)
                {
                    return;
                }

                this.ShowSuplDialog(this.StatusText, this.HashSupl);
                e.Handled = true;
            }
        }

        private void StatusText_KeyUp(object sender, KeyEventArgs e)
        {
            // スペースキーで未読ジャンプ
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.ProcessKey)
                {
                    bool isSpace = false;
                    foreach (char c in this.StatusText.Text.ToCharArray())
                    {
                        if (c == ' ' || c == '　')
                        {
                            isSpace = true;
                        }
                        else
                        {
                            isSpace = false;
                            break;
                        }
                    }

                    if (isSpace)
                    {
                        e.Handled = true;
                        this.StatusText.Text = string.Empty;
                        this.TrySearchAndFocusUnreadTweet();
                    }
                }
            }

            this.StatusText_TextChangedExtracted();
        }

        private void StatusText_LeaveExtracted()
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (this.ListTab.SelectedTab != null && this.MenuStrip1.Tag == null)
            {
                this.Tag = this.ListTab.SelectedTab.Tag;
            }

            this.StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            StatusText_LeaveExtracted();
        }

        private void ChangeStatusTextMultiline(bool isMultiLine)
        {
            this.StatusText.ScrollBars = isMultiLine ? ScrollBars.Vertical : ScrollBars.None;
            this.modifySettingLocal = true;
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            ChangeStatusTextMultiline(this.StatusText.Multiline);
        }

        private void StatusText_TextChangedExtracted()
        {
            // 文字数カウント
            int len = this.GetRestStatusCount(true, false);
            this.lblLen.Text = len.ToString();
            this.StatusText.ForeColor = len < 0 ? Color.Red : this.clrInputForecolor;
            if (string.IsNullOrEmpty(this.StatusText.Text))
            {
                this.replyToId = 0;
                this.replyToName = string.Empty;
            }
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            StatusText_TextChangedExtracted();
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.TimelineRefreshEnableChange(!this.StopRefreshAllMenuItem.Checked);
        }

        private void ChangeUserStreamStatus()
        {
            this.MenuItemUserStream.Enabled = false;
            if (this.StopRefreshAllMenuItem.Checked)
            {
                this.StopRefreshAllMenuItem.Checked = false;
                return;
            }

            if (this.isActiveUserstream)
            {
                this.tw.StopUserStream();
            }
            else
            {
                this.tw.StartUserStream();
            }
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeUserStreamStatus();
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                this.isOsResumed = true;
            }
        }

        private void AddFilteringRuleFromSelectedTweet()
        {
            // 選択発言を元にフィルタ追加
            foreach (int idx in this.curList.SelectedIndices)
            {
                // タブ選択（or追加）
                string tabName = string.Empty;
                if (!this.SelectTab(ref tabName))
                {
                    return;
                }

                this.fltDialog.SetCurrent(tabName);
                PostClass statusesItem = this.statuses.Item(this.curTab.Text, idx);
                string scname = statusesItem.IsRetweeted ? statusesItem.RetweetedBy : statusesItem.ScreenName;
                this.fltDialog.AddNewFilter(scname, statusesItem.TextFromApi);
                this.fltDialog.ShowDialog();
                this.TopMost = this.settingDialog.AlwaysTop;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                this.statuses.FilterAll();
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }

                if (!this.settingDialog.TabIconDisp)
                {
                    this.ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            this.SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null && ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                this.curPost = this.statuses.Item(this.ListTab.SelectedTab.Text, ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            AddFilteringRuleFromSelectedTweet();
        }

        private void RenameCurrentTabName()
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.TabRename(ref this.rclickTabName);
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            RenameCurrentTabName();
        }

        private void RenameSelectedTabName()
        {
            string tn = this.ListTab.SelectedTab.Text;
            this.TabRename(ref tn);
        }

        private void Tabs_DoubleClick(object sender, MouseEventArgs e)
        {
            RenameSelectedTabName();
        }

        private void Tabs_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TabPage)))
            {
                return;
            }

            this.tabDraging = false;
            string tn = string.Empty;
            bool bef = false;
            Point cpos = new Point(e.X, e.Y);
            Point spos = this.ListTab.PointToClient(cpos);
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Contains(spos))
                {
                    tn = this.ListTab.TabPages[i].Text;
                    bef = spos.X <= (rect.Left + rect.Right) / 2;
                    break;
                }
            }

            // タブのないところにドロップ->最後尾へ移動
            if (string.IsNullOrEmpty(tn))
            {
                tn = this.ListTab.TabPages[this.ListTab.TabPages.Count - 1].Text;
                bef = false;
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn)
            {
                return;
            }

            this.ReOrderTab(tp.Text, tn, bef);
        }

        private void Tabs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TabPage)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Tabs_MouseDownExtracted(MouseEventArgs e)
        {
            if (this.settingDialog.TabMouseLock)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                this.tabDraging = false;
            }
            else
            {
                Point cpos = e.Location;
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(cpos))
                    {
                        this.tabDraging = true;
                        this.tabMouseDownPoint = cpos;
                        break;
                    }
                }
            }
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            Tabs_MouseDownExtracted(e);
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e)
        {
            if (!this.timerTimeline.Enabled)
            {
                return;
            }

            this.resetTimers = e;
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            // 200ms
            this.RefreshTasktrayIcon(false);
        }

        private void TimerTimeline_ElapsedExtracted()
        {
            if (this.timerHomeCounter > 0)
            {
                Interlocked.Decrement(ref this.timerHomeCounter);
            }

            if (this.timerMentionCounter > 0)
            {
                Interlocked.Decrement(ref this.timerMentionCounter);
            }

            if (this.timerDmCounter > 0)
            {
                Interlocked.Decrement(ref this.timerDmCounter);
            }

            if (this.timerPubSearchCounter > 0)
            {
                Interlocked.Decrement(ref this.timerPubSearchCounter);
            }

            if (this.timerUserTimelineCounter > 0)
            {
                Interlocked.Decrement(ref this.timerUserTimelineCounter);
            }

            if (this.timerListsCounter > 0)
            {
                Interlocked.Decrement(ref this.timerListsCounter);
            }

            if (this.timerUsCounter > 0)
            {
                Interlocked.Decrement(ref this.timerUsCounter);
            }

            Interlocked.Increment(ref this.timerRefreshFollowers);

            // 'タイマー初期化
            if (this.resetTimers.Timeline || (this.timerHomeCounter <= 0 && this.settingDialog.TimelinePeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerHomeCounter, this.settingDialog.TimelinePeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.Timeline)
                {
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                }

                this.resetTimers.Timeline = false;
            }

            if (this.resetTimers.Reply || (this.timerMentionCounter <= 0 && this.settingDialog.ReplyPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerMentionCounter, this.settingDialog.ReplyPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.Reply)
                {
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                }

                this.resetTimers.Reply = false;
            }

            if (this.resetTimers.DirectMessage || (this.timerDmCounter <= 0 && this.settingDialog.DMPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerDmCounter, this.settingDialog.DMPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.DirectMessage)
                {
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                }

                this.resetTimers.DirectMessage = false;
            }

            if (this.resetTimers.PublicSearch || (this.timerPubSearchCounter <= 0 && this.settingDialog.PubSearchPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerPubSearchCounter, this.settingDialog.PubSearchPeriodInt);
                if (!this.resetTimers.PublicSearch)
                {
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                }

                this.resetTimers.PublicSearch = false;
            }

            if (this.resetTimers.UserTimeline || (this.timerUserTimelineCounter <= 0 && this.settingDialog.UserTimelinePeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerUserTimelineCounter, this.settingDialog.UserTimelinePeriodInt);
                if (!this.resetTimers.UserTimeline)
                {
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                }

                this.resetTimers.UserTimeline = false;
            }

            if (this.resetTimers.Lists || (this.timerListsCounter <= 0 && this.settingDialog.ListsPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerListsCounter, this.settingDialog.ListsPeriodInt);
                if (!this.resetTimers.Lists)
                {
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                }

                this.resetTimers.Lists = false;
            }

            if (this.resetTimers.UserStream || (this.timerUsCounter <= 0 && this.settingDialog.UserstreamPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerUsCounter, this.settingDialog.UserstreamPeriodInt);
                if (this.isActiveUserstream)
                {
                    this.RefreshTimeline(true);
                }

                this.resetTimers.UserStream = false;
            }

            if (this.timerRefreshFollowers > 6 * 3600)
            {
                Interlocked.Exchange(ref this.timerRefreshFollowers, 0);
                this.DoGetFollowersMenu();
                this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }

            if (this.isOsResumed)
            {
                Interlocked.Increment(ref this.timerResumeWait);
                if (this.timerResumeWait > 30)
                {
                    this.isOsResumed = false;
                    Interlocked.Exchange(ref this.timerResumeWait, 0);
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                    this.DoGetFollowersMenu();
                    this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                    if (InvokeRequired && !IsDisposed)
                    {
                        this.Invoke(new MethodInvoker(this.TrimPostChain));
                    }
                }
            }
        }

        private void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            TimerTimeline_ElapsedExtracted();
        }

        private void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.TinyUrl);
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.SetModifySettingCommon(true);
        }

        private void ChangeAutoUrlConvertFlag(bool autoConvert)
        {
            this.settingDialog.UrlConvertAuto = autoConvert;
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            ChangeAutoUrlConvertFlag(this.ToolStripMenuItemUrlAutoShorten.Checked);
        }

        private static void ChangeTraceFlag(bool trace)
        {
            MyCommon.TraceFlag = trace;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTraceFlag(this.TraceOutToolStripMenuItem.Checked);
        }

        private void ChangeTrackWordStatus()
        {
            if (this.TrackToolStripMenuItem.Checked)
            {
                using (InputTabName inputForm = new InputTabName())
                {
                    inputForm.TabName = this.prevTrackWord;
                    inputForm.SetFormTitle("Input track word");
                    inputForm.SetFormDescription("Track word");
                    if (inputForm.ShowDialog() != DialogResult.OK)
                    {
                        this.TrackToolStripMenuItem.Checked = false;
                        return;
                    }

                    this.prevTrackWord = inputForm.TabName.Trim();
                }

                if (this.prevTrackWord != this.tw.TrackWord)
                {
                    this.tw.TrackWord = this.prevTrackWord;
                    this.modifySettingCommon = true;
                    this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.prevTrackWord);
                    this.tw.ReconnectUserStream();
                }
            }
            else
            {
                this.tw.TrackWord = string.Empty;
                this.tw.ReconnectUserStream();
            }

            this.modifySettingCommon = true;
        }

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTrackWordStatus();
        }

        private void TranslateCurrentTweet()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.DoTranslation(this.curPost.TextFromApi);
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TranslateCurrentTweet();
        }

        private void Tw_NewPostFromStream()
        {
            if (this.settingDialog.ReadOldPosts)
            {
                // 新着時未読クリア
                this.statuses.SetRead();
            }

            int rsltAddCount = this.statuses.DistributePosts();
            lock (this.syncObject)
            {
                DateTime tm = DateTime.Now;
                if (this.timeLineTimestamps.ContainsKey(tm))
                {
                    this.timeLineTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    this.timeLineTimestamps.Add(tm, rsltAddCount);
                }

                DateTime oneHour = tm.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                this.timeLineCount = 0;
                foreach (System.DateTime key in this.timeLineTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                    {
                        keys.Add(key);
                    }
                    else
                    {
                        this.timeLineCount += this.timeLineTimestamps[key];
                    }
                }

                foreach (DateTime key in keys)
                {
                    this.timeLineTimestamps.Remove(key);
                }

                keys.Clear();
            }

            if (this.settingDialog.UserstreamPeriodInt > 0)
            {
                return;
            }

            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<bool>(this.RefreshTimeline), true);
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void Tw_PostDeleted(long id)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action(() =>
                    {
                        this.statuses.RemovePostReserve(id);
                        if (this.curTab != null && this.statuses.Tabs[this.curTab.Text].Contains(id))
                        {
                            this.itemCache = null;
                            this.itemCacheIndex = -1;
                            this.postCache = null;
                            ((DetailsListView)this.curTab.Tag).Update();
                            if (this.curPost != null & this.curPost.StatusId == id)
                            {
                                DispSelectedPost(true);
                            }
                        }
                    }));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void Tw_UserIdChanged()
        {
            this.modifySettingCommon = true;
        }

        private void Tw_UserStreamEventArrived(Twitter.FormattedEvent ev)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<Twitter.FormattedEvent>(this.Tw_UserStreamEventArrived), ev);
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            this.StatusLabel.Text = "Event: " + ev.Event;
            this.NotifyEvent(ev);
            if (ev.Event == "favorite" || ev.Event == "unfavorite")
            {
                if (this.curTab != null && this.statuses.Tabs[this.curTab.Text].Contains(ev.Id))
                {
                    this.itemCache = null;
                    this.itemCacheIndex = -1;
                    this.postCache = null;
                    ((DetailsListView)this.curTab.Tag).Update();
                }

                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(this.tw.Username.ToLower()))
                {
                    this.RemovePostFromFavTab(new long[] { ev.Id });
                }
            }
        }

        private void ChangeUserStreamStatusDisplay(bool start)
        {
            this.MenuItemUserStream.Text = start ? "&UserStream ▶" : "&UserStream ■";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = start ? "&Stop" : "&Start";
            this.StopToolStripMenuItem.Enabled = true;
            this.StatusLabel.Text = start ? "UserStream Started." : "UserStream Stopped.";
        }

        private void Tw_UserStreamStarted()
        {
            this.isActiveUserstream = true;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.Tw_UserStreamStarted));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            ChangeUserStreamStatusDisplay(start: true);
        }

        private void Tw_UserStreamStopped()
        {
            this.isActiveUserstream = false;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.Tw_UserStreamStopped));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            ChangeUserStreamStatusDisplay(start: false);
        }

        private void ActivateMainFormControls()
        {
            /// 画面がアクティブになったら、発言欄の背景色戻す
            if (this.StatusText.Focused)
            {
                this.StatusText_EnterExtracted();
            }
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            this.ActivateMainFormControls();
        }

        private void TweenMain_ClientSizeChangedExtracted()
        {
            if (!this.initialLayout && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.mySize = this.ClientSize;
                    this.mySpDis = this.SplitContainer1.SplitterDistance;
                    this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (this.StatusText.Multiline)
                    {
                        this.mySpDis2 = this.StatusText.Height;
                    }

                    this.myAdSpDis = this.SplitContainer4.SplitterDistance;
                    this.modifySettingLocal = true;
                }
            }
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            this.TweenMain_ClientSizeChangedExtracted();
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_LeaveExtracted();
        }

        private void DisposeAll()
        {
            // 後始末
            this.settingDialog.Dispose();
            this.tabDialog.Dispose();
            this.searchDialog.Dispose();
            this.fltDialog.Dispose();
            this.urlDialog.Dispose();
            this.spaceKeyCanceler.Dispose();
            if (this.iconAt != null)
            {
                this.iconAt.Dispose();
            }

            if (this.iconAtRed != null)
            {
                this.iconAtRed.Dispose();
            }

            if (this.iconAtSmoke != null)
            {
                this.iconAtSmoke.Dispose();
            }

            if (this.iconRefresh[0] != null)
            {
                this.iconRefresh[0].Dispose();
            }

            if (this.iconRefresh[1] != null)
            {
                this.iconRefresh[1].Dispose();
            }

            if (this.iconRefresh[2] != null)
            {
                this.iconRefresh[2].Dispose();
            }

            if (this.iconRefresh[3] != null)
            {
                this.iconRefresh[3].Dispose();
            }

            if (this.tabIcon != null)
            {
                this.tabIcon.Dispose();
            }

            if (this.mainIcon != null)
            {
                this.mainIcon.Dispose();
            }

            if (this.replyIcon != null)
            {
                this.replyIcon.Dispose();
            }

            if (this.replyIconBlink != null)
            {
                this.replyIconBlink.Dispose();
            }

            this.brsHighLight.Dispose();
            this.brsHighLightText.Dispose();
            if (this.brsForeColorUnread != null)
            {
                this.brsForeColorUnread.Dispose();
            }

            if (this.brsForeColorReaded != null)
            {
                this.brsForeColorReaded.Dispose();
            }

            if (this.brsForeColorFav != null)
            {
                this.brsForeColorFav.Dispose();
            }

            if (this.brsForeColorOWL != null)
            {
                this.brsForeColorOWL.Dispose();
            }

            if (this.brsForeColorRetweet != null)
            {
                this.brsForeColorRetweet.Dispose();
            }

            if (this.brsBackColorMine != null)
            {
                this.brsBackColorMine.Dispose();
            }

            if (this.brsBackColorAt != null)
            {
                this.brsBackColorAt.Dispose();
            }

            if (this.brsBackColorYou != null)
            {
                this.brsBackColorYou.Dispose();
            }

            if (this.brsBackColorAtYou != null)
            {
                this.brsBackColorAtYou.Dispose();
            }

            if (this.brsBackColorAtFromTarget != null)
            {
                this.brsBackColorAtFromTarget.Dispose();
            }

            if (this.brsBackColorAtTo != null)
            {
                this.brsBackColorAtTo.Dispose();
            }

            if (this.brsBackColorNone != null)
            {
                this.brsBackColorNone.Dispose();
            }

            if (this.brsDeactiveSelection != null)
            {
                this.brsDeactiveSelection.Dispose();
            }

            this.shield.Dispose();
            this.tabStringFormat.Dispose();
            foreach (BackgroundWorker bw in this.bworkers)
            {
                if (bw != null)
                {
                    bw.Dispose();
                }
            }

            if (this.followerFetchWorker != null)
            {
                this.followerFetchWorker.Dispose();
            }

            this.apiGauge.Dispose();
            if (this.iconDict != null)
            {
                this.iconDict.PauseGetImage = true;
                this.iconDict.Dispose();
            }

            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            this.DisposeAll();
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                this.ImageSelectionPanel.Visible = true;
                this.ImageSelectionPanel.Enabled = true;
                this.TimelinePanel.Visible = false;
                this.TimelinePanel.Enabled = false;
                this.ImagefilePathText.Text = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                this.LoadImageFromSelectedFile();
                this.Activate();
                this.BringToFront();
                this.StatusText.Focus();
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string data = e.Data.GetData(DataFormats.StringFormat, true) as string;
                if (data != null)
                {
                    this.StatusText.Text += data;
                }
            }
        }

        private void TweenMain_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                FileInfo fl = new FileInfo(filename);
                string ext = fl.Extension;

                if (!string.IsNullOrEmpty(this.ImageService) && this.pictureServices[this.ImageService].CheckValidFilesize(ext, fl.Length))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }

                foreach (string svc in this.ImageServiceCombo.Items)
                {
                    if (string.IsNullOrEmpty(svc))
                    {
                        continue;
                    }

                    if (this.pictureServices[svc].CheckValidFilesize(ext, fl.Length))
                    {
                        this.ImageServiceCombo.SelectedItem = svc;
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }

                e.Effect = DragDropEffects.None;
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void TweenMain_FormClosingExtracted(FormClosingEventArgs e)
        {
            if (!this.settingDialog.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon.IsEnding == false)
            {
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                this.ignoreConfigSave = true;
                MyCommon.IsEnding = true;
                this.timerTimeline.Enabled = false;
                this.TimerRefreshIcon.Enabled = false;
            }
        }

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.TweenMain_FormClosingExtracted(e);
        }

        private void TweenMain_LoadExtracted()
        {
            this.ignoreConfigSave = true;
            this.Visible = false;

            this.securityManager = new InternetSecurityManager(this.PostBrowser);
            this.thumbnail = new Thumbnail(this);

            MyCommon.TwitterApiInfo.Changed += this.SetStatusLabelApiHandler;
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;

            this.VerUpMenuItem.Image = this.shield.Icon;
            var cmdArgs = System.Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (cmdArgs.Length != 0 && cmdArgs.Contains("/d"))
            {
                MyCommon.TraceFlag = true;
            }

            this.spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this.spaceKeyCanceler.SpaceCancel += this.SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            this.InitializeTraceFrag();

            // アイコン読み込み
            this.LoadIcons();

            // 発言保持クラス
            this.statuses = TabInformations.GetInstance();

            // アイコン設定
            this.Icon = this.mainIcon;          // メインフォーム（TweenMain）
            this.NotifyIcon1.Icon = this.iconAt;     // タスクトレイ
            this.TabImage.Images.Add(this.tabIcon);  // タブ見出し

            this.settingDialog.Owner = this;
            this.searchDialog.Owner = this;
            this.fltDialog.Owner = this;
            this.tabDialog.Owner = this;
            this.urlDialog.Owner = this;

            this.postHistory.Add(new PostingStatus());
            this.postHistoryIndex = 0;
            this.replyToId = 0;
            this.replyToName = string.Empty;

            // <<<<<<<<<設定関連>>>>>>>>>
            // '設定読み出し
            this.LoadConfig();

            // 新着バルーン通知のチェック状態設定
            this.NewPostPopMenuItem.Checked = this.cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = this.NewPostPopMenuItem.Checked;

            // フォント＆文字色＆背景色保持
            this.fntUnread = this.cfgLocal.FontUnread;
            this.clrUnread = this.cfgLocal.ColorUnread;
            this.fntReaded = this.cfgLocal.FontRead;
            this.clrRead = this.cfgLocal.ColorRead;
            this.clrFav = this.cfgLocal.ColorFav;
            this.clrOWL = this.cfgLocal.ColorOWL;
            this.clrRetweet = this.cfgLocal.ColorRetweet;
            this.fntDetail = this.cfgLocal.FontDetail;
            this.clrDetail = this.cfgLocal.ColorDetail;
            this.clrDetailLink = this.cfgLocal.ColorDetailLink;
            this.clrDetailBackcolor = this.cfgLocal.ColorDetailBackcolor;
            this.clrSelf = this.cfgLocal.ColorSelf;
            this.clrAtSelf = this.cfgLocal.ColorAtSelf;
            this.clrTarget = this.cfgLocal.ColorTarget;
            this.clrAtTarget = this.cfgLocal.ColorAtTarget;
            this.clrAtFromTarget = this.cfgLocal.ColorAtFromTarget;
            this.clrAtTo = this.cfgLocal.ColorAtTo;
            this.clrListBackcolor = this.cfgLocal.ColorListBackcolor;
            this.InputBackColor = this.cfgLocal.ColorInputBackcolor;
            this.clrInputForecolor = this.cfgLocal.ColorInputFont;
            this.fntInputFont = this.cfgLocal.FontInputFont;

            this.brsForeColorUnread = new SolidBrush(this.clrUnread);
            this.brsForeColorReaded = new SolidBrush(this.clrRead);
            this.brsForeColorFav = new SolidBrush(this.clrFav);
            this.brsForeColorOWL = new SolidBrush(this.clrOWL);
            this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
            this.brsBackColorMine = new SolidBrush(this.clrSelf);
            this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
            this.brsBackColorYou = new SolidBrush(this.clrTarget);
            this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
            this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
            this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
            this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);

            // StringFormatオブジェクトへの事前設定
            this.tabStringFormat.Alignment = StringAlignment.Center;
            this.tabStringFormat.LineAlignment = StringAlignment.Center;

            // 設定画面への反映
            HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);
            this.settingDialog.TwitterApiUrl = this.cfgCommon.TwitterUrl;
            this.settingDialog.TwitterSearchApiUrl = this.cfgCommon.TwitterSearchUrl;

            // 認証関連
            if (string.IsNullOrEmpty(this.cfgCommon.Token))
            {
                this.cfgCommon.UserName = string.Empty;
            }

            this.tw.Initialize(this.cfgCommon.Token, this.cfgCommon.TokenSecret, this.cfgCommon.UserName, this.cfgCommon.UserId);

            this.settingDialog.UserAccounts = this.cfgCommon.UserAccounts;
            this.settingDialog.TimelinePeriodInt = this.cfgCommon.TimelinePeriod;
            this.settingDialog.ReplyPeriodInt = this.cfgCommon.ReplyPeriod;
            this.settingDialog.DMPeriodInt = this.cfgCommon.DMPeriod;
            this.settingDialog.PubSearchPeriodInt = this.cfgCommon.PubSearchPeriod;
            this.settingDialog.UserTimelinePeriodInt = this.cfgCommon.UserTimelinePeriod;
            this.settingDialog.ListsPeriodInt = this.cfgCommon.ListsPeriod;

            // 不正値チェック
            if (!cmdArgs.Contains("nolimit"))
            {
                if (this.settingDialog.TimelinePeriodInt < 15 && this.settingDialog.TimelinePeriodInt > 0)
                {
                    this.settingDialog.TimelinePeriodInt = 15;
                }

                if (this.settingDialog.ReplyPeriodInt < 15 && this.settingDialog.ReplyPeriodInt > 0)
                {
                    this.settingDialog.ReplyPeriodInt = 15;
                }

                if (this.settingDialog.DMPeriodInt < 15 && this.settingDialog.DMPeriodInt > 0)
                {
                    this.settingDialog.DMPeriodInt = 15;
                }

                if (this.settingDialog.PubSearchPeriodInt < 30 && this.settingDialog.PubSearchPeriodInt > 0)
                {
                    this.settingDialog.PubSearchPeriodInt = 30;
                }

                if (this.settingDialog.UserTimelinePeriodInt < 15 && this.settingDialog.UserTimelinePeriodInt > 0)
                {
                    this.settingDialog.UserTimelinePeriodInt = 15;
                }

                if (this.settingDialog.ListsPeriodInt < 15 && this.settingDialog.ListsPeriodInt > 0)
                {
                    this.settingDialog.ListsPeriodInt = 15;
                }
            }

            // 起動時読み込み分を既読にするか。Trueなら既読として処理
            this.settingDialog.Readed = this.cfgCommon.Read;

            // 新着取得時のリストスクロールをするか。Trueならスクロールしない
            this.ListLockMenuItem.Checked = this.cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = this.cfgCommon.ListLock;
            this.settingDialog.IconSz = this.cfgCommon.IconSize;

            // 文末ステータス
            this.settingDialog.Status = this.cfgLocal.StatusText;

            // 未読管理。Trueなら未読管理する
            this.settingDialog.UnreadManage = this.cfgCommon.UnreadManage;

            // サウンド再生（タブ別設定より優先）
            this.settingDialog.PlaySound = this.cfgCommon.PlaySound;
            this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;

            // 片思い表示。Trueなら片思い表示する
            this.settingDialog.OneWayLove = this.cfgCommon.OneWayLove;

            // フォント＆文字色＆背景色
            this.settingDialog.FontUnread = this.fntUnread;
            this.settingDialog.ColorUnread = this.clrUnread;
            this.settingDialog.FontReaded = this.fntReaded;
            this.settingDialog.ColorReaded = this.clrRead;
            this.settingDialog.ColorFav = this.clrFav;
            this.settingDialog.ColorOWL = this.clrOWL;
            this.settingDialog.ColorRetweet = this.clrRetweet;
            this.settingDialog.FontDetail = this.fntDetail;
            this.settingDialog.ColorDetail = this.clrDetail;
            this.settingDialog.ColorDetailLink = this.clrDetailLink;
            this.settingDialog.ColorDetailBackcolor = this.clrDetailBackcolor;
            this.settingDialog.ColorSelf = this.clrSelf;
            this.settingDialog.ColorAtSelf = this.clrAtSelf;
            this.settingDialog.ColorTarget = this.clrTarget;
            this.settingDialog.ColorAtTarget = this.clrAtTarget;
            this.settingDialog.ColorAtFromTarget = this.clrAtFromTarget;
            this.settingDialog.ColorAtTo = this.clrAtTo;
            this.settingDialog.ColorListBackcolor = this.clrListBackcolor;
            this.settingDialog.ColorInputBackcolor = this.InputBackColor;
            this.settingDialog.ColorInputFont = this.clrInputForecolor;
            this.settingDialog.FontInputFont = this.fntInputFont;
            this.settingDialog.NameBalloon = this.cfgCommon.NameBalloon;
            this.settingDialog.PostCtrlEnter = this.cfgCommon.PostCtrlEnter;
            this.settingDialog.PostShiftEnter = this.cfgCommon.PostShiftEnter;
            this.settingDialog.CountApi = this.cfgCommon.CountApi;
            this.settingDialog.CountApiReply = this.cfgCommon.CountApiReply;
            if (this.settingDialog.CountApi < 20 || this.settingDialog.CountApi > 200)
            {
                this.settingDialog.CountApi = 60;
            }

            if (this.settingDialog.CountApiReply < 20 || this.settingDialog.CountApiReply > 200)
            {
                this.settingDialog.CountApiReply = 40;
            }

            this.settingDialog.BrowserPath = this.cfgLocal.BrowserPath;
            this.settingDialog.PostAndGet = this.cfgCommon.PostAndGet;
            this.settingDialog.UseRecommendStatus = this.cfgLocal.UseRecommendStatus;
            this.settingDialog.DispUsername = this.cfgCommon.DispUsername;
            this.settingDialog.CloseToExit = this.cfgCommon.CloseToExit;
            this.settingDialog.MinimizeToTray = this.cfgCommon.MinimizeToTray;
            this.settingDialog.DispLatestPost = this.cfgCommon.DispLatestPost;
            this.settingDialog.SortOrderLock = this.cfgCommon.SortOrderLock;
            this.settingDialog.TinyUrlResolve = this.cfgCommon.TinyUrlResolve;
            this.settingDialog.ShortUrlForceResolve = this.cfgCommon.ShortUrlForceResolve;
            this.settingDialog.SelectedProxyType = this.cfgLocal.ProxyType;
            this.settingDialog.ProxyAddress = this.cfgLocal.ProxyAddress;
            this.settingDialog.ProxyPort = this.cfgLocal.ProxyPort;
            this.settingDialog.ProxyUser = this.cfgLocal.ProxyUser;
            this.settingDialog.ProxyPassword = this.cfgLocal.ProxyPassword;
            this.settingDialog.PeriodAdjust = this.cfgCommon.PeriodAdjust;
            this.settingDialog.StartupVersion = this.cfgCommon.StartupVersion;
            this.settingDialog.StartupFollowers = this.cfgCommon.StartupFollowers;
            this.settingDialog.RestrictFavCheck = this.cfgCommon.RestrictFavCheck;
            this.settingDialog.AlwaysTop = this.cfgCommon.AlwaysTop;
            this.settingDialog.UrlConvertAuto = false;
            this.settingDialog.OutputzEnabled = this.cfgCommon.Outputz;
            this.settingDialog.OutputzKey = this.cfgCommon.OutputzKey;
            this.settingDialog.OutputzUrlmode = this.cfgCommon.OutputzUrlMode;
            this.settingDialog.UseUnreadStyle = this.cfgCommon.UseUnreadStyle;
            this.settingDialog.DefaultTimeOut = this.cfgCommon.DefaultTimeOut;
            this.settingDialog.RetweetNoConfirm = this.cfgCommon.RetweetNoConfirm;
            this.settingDialog.PlaySound = this.cfgCommon.PlaySound;
            this.settingDialog.DateTimeFormat = this.cfgCommon.DateTimeFormat;
            this.settingDialog.LimitBalloon = this.cfgCommon.LimitBalloon;
            this.settingDialog.EventNotifyEnabled = this.cfgCommon.EventNotifyEnabled;
            this.settingDialog.EventNotifyFlag = this.cfgCommon.EventNotifyFlag;
            this.settingDialog.IsMyEventNotifyFlag = this.cfgCommon.IsMyEventNotifyFlag;
            this.settingDialog.ForceEventNotify = this.cfgCommon.ForceEventNotify;
            this.settingDialog.FavEventUnread = this.cfgCommon.FavEventUnread;
            this.settingDialog.TranslateLanguage = this.cfgCommon.TranslateLanguage;
            this.settingDialog.EventSoundFile = this.cfgCommon.EventSoundFile;

            // 廃止サービスが選択されていた場合bit.lyへ読み替え
            if (this.cfgCommon.AutoShortUrlFirst < 0)
            {
                this.cfgCommon.AutoShortUrlFirst = UrlConverter.Bitly;
            }

            this.settingDialog.AutoShortUrlFirst = this.cfgCommon.AutoShortUrlFirst;
            this.settingDialog.TabIconDisp = this.cfgCommon.TabIconDisp;
            this.settingDialog.ReplyIconState = this.cfgCommon.ReplyIconState;
            this.settingDialog.ReadOwnPost = this.cfgCommon.ReadOwnPost;
            this.settingDialog.GetFav = this.cfgCommon.GetFav;
            this.settingDialog.ReadOldPosts = this.cfgCommon.ReadOldPosts;
            this.settingDialog.UseSsl = this.cfgCommon.UseSsl;
            this.settingDialog.BitlyUser = this.cfgCommon.BilyUser;
            this.settingDialog.BitlyPwd = this.cfgCommon.BitlyPwd;
            this.settingDialog.ShowGrid = this.cfgCommon.ShowGrid;
            this.settingDialog.Language = this.cfgCommon.Language;
            this.settingDialog.UseAtIdSupplement = this.cfgCommon.UseAtIdSupplement;
            this.settingDialog.UseHashSupplement = this.cfgCommon.UseHashSupplement;
            this.settingDialog.PreviewEnable = this.cfgCommon.PreviewEnable;
            this.AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            this.settingDialog.IsMonospace = this.cfgCommon.IsMonospace;
            if (this.settingDialog.IsMonospace)
            {
                this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
            }
            else
            {
                this.detailHtmlFormatHeader = DetailHtmlFormat1;
                this.detailHtmlFormatFooter = DetailHtmlFormat7;
            }

            this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
            if (this.settingDialog.IsMonospace)
            {
                this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
            }
            else
            {
                this.detailHtmlFormatHeader += DetailHtmlFormat6;
            }

            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = this.cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = this.cfgCommon.FocusLockToStatusText;

            this.settingDialog.RecommendStatusText = " [TWNv" + Regex.Replace(MyCommon.FileVersion.Replace(".", string.Empty), "^0*", string.Empty) + "]";

            // 書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(this.settingDialog.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    this.settingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                this.settingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            this.settingDialog.Nicoms = this.cfgCommon.Nicoms;
            this.settingDialog.HotkeyEnabled = this.cfgCommon.HotkeyEnabled;
            this.settingDialog.HotkeyMod = this.cfgCommon.HotkeyModifier;
            this.settingDialog.HotkeyKey = this.cfgCommon.HotkeyKey;
            this.settingDialog.HotkeyValue = this.cfgCommon.HotkeyValue;
            this.settingDialog.BlinkNewMentions = this.cfgCommon.BlinkNewMentions;
            this.settingDialog.UseAdditionalCount = this.cfgCommon.UseAdditionalCount;
            this.settingDialog.MoreCountApi = this.cfgCommon.MoreCountApi;
            this.settingDialog.FirstCountApi = this.cfgCommon.FirstCountApi;
            this.settingDialog.SearchCountApi = this.cfgCommon.SearchCountApi;
            this.settingDialog.FavoritesCountApi = this.cfgCommon.FavoritesCountApi;
            this.settingDialog.UserTimelineCountApi = this.cfgCommon.UserTimelineCountApi;
            this.settingDialog.ListCountApi = this.cfgCommon.ListCountApi;
            this.settingDialog.UserstreamStartup = this.cfgCommon.UserstreamStartup;
            this.settingDialog.UserstreamPeriodInt = this.cfgCommon.UserstreamPeriod;
            this.settingDialog.OpenUserTimeline = this.cfgCommon.OpenUserTimeline;
            this.settingDialog.ListDoubleClickAction = this.cfgCommon.ListDoubleClickAction;
            this.settingDialog.UserAppointUrl = this.cfgCommon.UserAppointUrl;
            this.settingDialog.HideDuplicatedRetweets = this.cfgCommon.HideDuplicatedRetweets;
            this.settingDialog.IsPreviewFoursquare = this.cfgCommon.IsPreviewFoursquare;
            this.settingDialog.FoursquarePreviewHeight = this.cfgCommon.FoursquarePreviewHeight;
            this.settingDialog.FoursquarePreviewWidth = this.cfgCommon.FoursquarePreviewWidth;
            this.settingDialog.FoursquarePreviewZoom = this.cfgCommon.FoursquarePreviewZoom;
            this.settingDialog.IsListStatusesIncludeRts = this.cfgCommon.IsListsIncludeRts;
            this.settingDialog.TabMouseLock = this.cfgCommon.TabMouseLock;
            this.settingDialog.IsRemoveSameEvent = this.cfgCommon.IsRemoveSameEvent;
            this.settingDialog.IsNotifyUseGrowl = this.cfgCommon.IsUseNotifyGrowl;

            // ハッシュタグ関連
            this.HashSupl = new AtIdSupplement(this.cfgCommon.HashTags, "#");
            this.HashMgr = new HashtagManage(this.HashSupl, this.cfgCommon.HashTags.ToArray(), this.cfgCommon.HashSelected, this.cfgCommon.HashIsPermanent, this.cfgCommon.HashIsHead, this.cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.HashMgr.IsPermanent)
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
            }

            this.isInitializing = true;

            // アイコンリスト作成
            try
            {
                this.iconDict = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }

            this.iconDict.PauseGetImage = false;

            bool saveRequired = false;

            // ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(this.tw.Username))
            {
                saveRequired = true;

                // 設定せずにキャンセルされた場合はプログラム終了
                if (this.settingDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // 設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
                if (string.IsNullOrEmpty(this.tw.Username))
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // フォント＆文字色＆背景色保持
                this.fntUnread = this.settingDialog.FontUnread;
                this.clrUnread = this.settingDialog.ColorUnread;
                this.fntReaded = this.settingDialog.FontReaded;
                this.clrRead = this.settingDialog.ColorReaded;
                this.clrFav = this.settingDialog.ColorFav;
                this.clrOWL = this.settingDialog.ColorOWL;
                this.clrRetweet = this.settingDialog.ColorRetweet;
                this.fntDetail = this.settingDialog.FontDetail;
                this.clrDetail = this.settingDialog.ColorDetail;
                this.clrDetailLink = this.settingDialog.ColorDetailLink;
                this.clrDetailBackcolor = this.settingDialog.ColorDetailBackcolor;
                this.clrSelf = this.settingDialog.ColorSelf;
                this.clrAtSelf = this.settingDialog.ColorAtSelf;
                this.clrTarget = this.settingDialog.ColorTarget;
                this.clrAtTarget = this.settingDialog.ColorAtTarget;
                this.clrAtFromTarget = this.settingDialog.ColorAtFromTarget;
                this.clrAtTo = this.settingDialog.ColorAtTo;
                this.clrListBackcolor = this.settingDialog.ColorListBackcolor;
                this.InputBackColor = this.settingDialog.ColorInputBackcolor;
                this.clrInputForecolor = this.settingDialog.ColorInputFont;
                this.fntInputFont = this.settingDialog.FontInputFont;
                this.brsForeColorUnread.Dispose();
                this.brsForeColorReaded.Dispose();
                this.brsForeColorFav.Dispose();
                this.brsForeColorOWL.Dispose();
                this.brsForeColorRetweet.Dispose();
                this.brsForeColorUnread = new SolidBrush(this.clrUnread);
                this.brsForeColorReaded = new SolidBrush(this.clrRead);
                this.brsForeColorFav = new SolidBrush(this.clrFav);
                this.brsForeColorOWL = new SolidBrush(this.clrOWL);
                this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                this.brsBackColorMine.Dispose();
                this.brsBackColorAt.Dispose();
                this.brsBackColorYou.Dispose();
                this.brsBackColorAtYou.Dispose();
                this.brsBackColorAtFromTarget.Dispose();
                this.brsBackColorAtTo.Dispose();
                this.brsBackColorNone.Dispose();
                this.brsBackColorMine = new SolidBrush(this.clrSelf);
                this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
                this.brsBackColorYou = new SolidBrush(this.clrTarget);
                this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);

                if (this.settingDialog.IsMonospace)
                {
                    this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                    this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
                }
                else
                {
                    this.detailHtmlFormatHeader = DetailHtmlFormat1;
                    this.detailHtmlFormatFooter = DetailHtmlFormat7;
                }

                this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                if (this.settingDialog.IsMonospace)
                {
                    this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
                }
                else
                {
                    this.detailHtmlFormatHeader += DetailHtmlFormat6;
                }
            }

            if (this.settingDialog.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((this.settingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }

                if ((this.settingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }

                if ((this.settingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }

                if ((this.settingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                this.hookGlobalHotkey.RegisterOriginalHotkey(this.settingDialog.HotkeyKey, this.settingDialog.HotkeyValue, modKey);
            }

            // Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(this.settingDialog.DefaultTimeOut, this.settingDialog.SelectedProxyType, this.settingDialog.ProxyAddress, this.settingDialog.ProxyPort, this.settingDialog.ProxyUser, this.settingDialog.ProxyPassword);

            this.tw.SetRestrictFavCheck(this.settingDialog.RestrictFavCheck);
            this.tw.ReadOwnPost = this.settingDialog.ReadOwnPost;
            this.tw.SetUseSsl(this.settingDialog.UseSsl);
            ShortUrl.IsResolve = this.settingDialog.TinyUrlResolve;
            ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(this.settingDialog.BitlyUser);
            ShortUrl.SetBitlyKey(this.settingDialog.BitlyPwd);
            HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);
            this.tw.TrackWord = this.cfgCommon.TrackWord;
            this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.tw.TrackWord);
            this.tw.AllAtReply = this.cfgCommon.AllAtReply;
            this.AllrepliesToolStripMenuItem.Checked = this.tw.AllAtReply;

            Outputz.Key = this.settingDialog.OutputzKey;
            Outputz.Enabled = this.settingDialog.OutputzEnabled;
            switch (this.settingDialog.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    Outputz.OutUrl = "http://twitter.com/";
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    Outputz.OutUrl = "http://twitter.com/" + this.tw.Username;
                    break;
            }

            // 画像投稿サービス
            this.CreatePictureServices();
            this.SetImageServiceCombo();
            this.ImageSelectionPanel.Enabled = false;
            this.ImageServiceCombo.SelectedIndex = this.cfgCommon.UseImageService;

            // ウィンドウ設定
            this.ClientSize = this.cfgLocal.FormSize;
            this.mySize = this.cfgLocal.FormSize;          // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            this.myLoc = this.cfgLocal.FormLocation;       // タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = this.cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(this.mySize.Width, SystemInformation.CaptionHeight));
                bool outOfScreen = true;
                if (Screen.AllScreens.Length == 1)
                {
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (!Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty)
                        {
                            outOfScreen = false;
                            break;
                        }
                    }

                    if (outOfScreen)
                    {
                        this.DesktopLocation = new Point(0, 0);
                        this.myLoc = this.DesktopLocation;
                    }
                }
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            this.mySpDis = this.cfgLocal.SplitterDistance;
            this.mySpDis2 = this.cfgLocal.StatusTextHeight;
            this.mySpDis3 = this.cfgLocal.PreviewDistance;
            if (this.mySpDis3 == -1)
            {
                this.mySpDis3 = this.mySize.Width - 150;
                if (this.mySpDis3 < 1)
                {
                    this.mySpDis3 = 50;
                }

                this.cfgLocal.PreviewDistance = this.mySpDis3;
            }

            this.myAdSpDis = this.cfgLocal.AdSplitterDistance;
            this.MultiLineMenuItem.Checked = this.cfgLocal.StatusMultiline;
            this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;

            // 入力欄
            this.StatusText.Font = this.fntInputFont;
            this.StatusText.ForeColor = this.clrInputForecolor;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this.settingDialog.UnreadManage == false)
            {
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
            }

            if (this.settingDialog.IsNotifyUseGrowl)
            {
                this.growlHelper.RegisterGrowl();
            }

            // タイマー設定
            this.timerTimeline.AutoReset = true;
            this.timerTimeline.SynchronizingObject = this;

            // Recent取得間隔
            this.timerTimeline.Interval = 1000;
            this.timerTimeline.Enabled = true;

            // 更新中アイコンアニメーション間隔
            this.TimerRefreshIcon.Interval = 200;
            this.TimerRefreshIcon.Enabled = true;

            // 状態表示部の初期化（画面右下）
            this.StatusLabel.Text = string.Empty;
            this.StatusLabel.AutoToolTip = false;
            this.StatusLabel.ToolTipText = string.Empty;

            // 文字カウンタ初期化
            this.lblLen.Text = this.GetRestStatusCount(true, false).ToString();

            this.statuses.SortOrder = (SortOrder)this.cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (this.cfgCommon.SortColumn)
            {
                case 0:
                case 5:
                case 6:
                    // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                    // ソートしない Idソートに読み替え
                    mode = IdComparerClass.ComparerMode.Id;
                    break;
                case 1:
                    // ニックネーム
                    mode = IdComparerClass.ComparerMode.Nickname;
                    break;
                case 2:
                    // 本文
                    mode = IdComparerClass.ComparerMode.Data;
                    break;
                case 3:
                    // 時刻=発言Id
                    mode = IdComparerClass.ComparerMode.Id;
                    break;
                case 4:
                    // 名前
                    mode = IdComparerClass.ComparerMode.Name;
                    break;
                case 7:
                    // Source
                    mode = IdComparerClass.ComparerMode.Source;
                    break;
            }

            this.statuses.SortMode = mode;

            switch (this.settingDialog.IconSz)
            {
                case IconSizes.IconNone:
                    this.iconSz = 0;
                    break;
                case IconSizes.Icon16:
                    this.iconSz = 16;
                    break;
                case IconSizes.Icon24:
                    this.iconSz = 26;
                    break;
                case IconSizes.Icon48:
                    this.iconSz = 48;
                    break;
                case IconSizes.Icon48_2:
                    this.iconSz = 48;
                    this.iconCol = true;
                    break;
            }

            if (this.iconSz == 0)
            {
                this.tw.SetGetIcon(false);
            }
            else
            {
                this.tw.SetGetIcon(true);
                this.tw.SetIconSize(this.iconSz);
            }

            this.tw.SetTinyUrlResolve(this.settingDialog.TinyUrlResolve);
            ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;

            // 発言詳細部アイコンをリストアイコンにサイズ変更
            int sz = this.iconSz;
            if (this.iconSz == 0)
            {
                sz = 16;
            }

            this.tw.DetailIcon = this.iconDict;
            this.StatusLabel.Text = Hoehoe.Properties.Resources.Form1_LoadText1;  // 画面右下の状態表示を変更
            this.StatusLabelUrl.Text = string.Empty;  // 画面左下のリンク先URL表示部を初期化
            this.NameLabel.Text = string.Empty;       // 発言詳細部名前ラベル初期化
            this.DateTimeLabel.Text = string.Empty;   // 発言詳細部日時ラベル初期化
            this.SourceLinkLabel.Text = string.Empty; // Source部分初期化

            // <<<<<<<<タブ関連>>>>>>>
            // デフォルトタブの存在チェック、ない場合には追加
            if (this.statuses.GetTabByType(TabUsageType.Home) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.RECENT))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.RECENT].TabType = TabUsageType.Home;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.Mentions) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.REPLY))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.REPLY].TabType = TabUsageType.Mentions;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.DirectMessage) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.DM))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.DM].TabType = TabUsageType.DirectMessage;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.Favorites) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.FAV))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.FAV].TabType = TabUsageType.Favorites;
                }
            }

            foreach (string tn in this.statuses.Tabs.Keys)
            {
                if (this.statuses.Tabs[tn].TabType == TabUsageType.Undefined)
                {
                    this.statuses.Tabs[tn].TabType = TabUsageType.UserDefined;
                }

                if (!this.AddNewTab(tn, true, this.statuses.Tabs[tn].TabType, this.statuses.Tabs[tn].ListInfo))
                {
                    throw new Exception(Hoehoe.Properties.Resources.TweenMain_LoadText1);
                }
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            this.CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            this.CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (this.settingDialog.MinimizeToTray == false || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }

            this.curTab = this.ListTab.SelectedTab;
            this.curItemIndex = -1;
            this.curList = (DetailsListView)this.curTab.Tag;
            this.SetMainWindowTitle();
            this.SetNotifyIconText();

            if (this.settingDialog.TabIconDisp)
            {
                this.ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                this.ListTab.DrawItem += this.ListTab_DrawItem;
                this.ListTab.ImageList = null;
            }

            this.SplitContainer4.Panel2Collapsed = true;
            this.ignoreConfigSave = false;
            this.ResizeMainForm();
            if (saveRequired)
            {
                this.SaveConfigsAll(false);
            }

            if (this.tw.UserId == 0)
            {
                this.tw.VerifyCredentials();
                foreach (var ua in this.cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == this.tw.Username.ToLower())
                    {
                        ua.UserId = this.tw.UserId;
                        break;
                    }
                }
            }

            foreach (var ua in this.settingDialog.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == this.tw.Username.ToLower())
                {
                    ua.UserId = this.tw.UserId;
                    break;
                }
            }
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            TweenMain_LoadExtracted();
        }

        private void TweenMain_LocationChangedExtracted()
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myLoc = this.DesktopLocation;
                this.modifySettingLocal = true;
            }
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            this.TweenMain_LocationChangedExtracted();
        }

        private void ResizeMainForm()
        {
            if (!this.initialLayout && this.settingDialog.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }

            if (this.initialLayout && this.cfgLocal != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                this.ClientSize = this.cfgLocal.FormSize;          // 'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                this.DesktopLocation = this.cfgLocal.FormLocation; // '位置保持（最小化・最大化されたまま終了した場合の対応用）

                if (!this.SplitContainer4.Panel2Collapsed && this.cfgLocal.AdSplitterDistance > this.SplitContainer4.Panel1MinSize)
                {
                    // Splitterの位置設定
                    this.SplitContainer4.SplitterDistance = this.cfgLocal.AdSplitterDistance;
                }

                if (this.cfgLocal.SplitterDistance > this.SplitContainer1.Panel1MinSize && this.cfgLocal.SplitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    // Splitterの位置設定
                    this.SplitContainer1.SplitterDistance = this.cfgLocal.SplitterDistance;
                }

                // 発言欄複数行
                this.StatusText.Multiline = this.cfgLocal.StatusMultiline;
                if (this.StatusText.Multiline)
                {
                    int dis = this.SplitContainer2.Height - this.cfgLocal.StatusTextHeight - this.SplitContainer2.SplitterWidth;
                    if (dis > this.SplitContainer2.Panel1MinSize && dis < this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.cfgLocal.StatusTextHeight - this.SplitContainer2.SplitterWidth;
                    }

                    this.StatusText.Height = this.cfgLocal.StatusTextHeight;
                }
                else
                {
                    if (this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth > 0)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth;
                    }
                }

                if (this.cfgLocal.PreviewDistance > this.SplitContainer3.Panel1MinSize && this.cfgLocal.PreviewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = this.cfgLocal.PreviewDistance;
                }

                this.initialLayout = false;
            }
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            this.ResizeMainForm();
        }

        private void TweenMain_ShownExtracted()
        {
            try
            {
                // 発言詳細部初期化
                this.PostBrowser.Url = new Uri("about:blank");
                this.PostBrowser.DocumentText = string.Empty;
            }
            catch (Exception)
            {
            }

            this.NotifyIcon1.Visible = true;
            this.tw.UserIdChanged += this.Tw_UserIdChanged;

            if (MyCommon.IsNetworkAvailable())
            {
                string tabNameAny = string.Empty;
                this.GetTimeline(WorkerType.BlockIds, 0, 0, tabNameAny);
                if (this.settingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, tabNameAny);
                }

                this.GetTimeline(WorkerType.Configuration, 0, 0, tabNameAny);
                this.StartUserStream();
                this.waitTimeline = true;
                this.GetTimeline(WorkerType.Timeline, 1, 1, tabNameAny);
                this.waitReply = true;
                this.GetTimeline(WorkerType.Reply, 1, 1, tabNameAny);
                this.waitDm = true;
                this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 1, tabNameAny);
                if (this.settingDialog.GetFav)
                {
                    this.waitFav = true;
                    this.GetTimeline(WorkerType.Favorites, 1, 1, tabNameAny);
                }

                this.waitPubSearch = true;
                this.GetTimeline(WorkerType.PublicSearch, 1, 0, tabNameAny);
                this.waitUserTimeline = true;
                this.GetTimeline(WorkerType.UserTimeline, 1, 0, tabNameAny);
                this.waitLists = true;
                this.GetTimeline(WorkerType.List, 1, 0, tabNameAny);
                int i = 0;
                int j = 0;
                while (this.IsInitialRead() && !MyCommon.IsEnding)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    i += 1;
                    j += 1;
                    if (j > 1200)
                    {
                        // 120秒間初期処理が終了しなかったら強制的に打ち切る
                        break;
                    }

                    if (i > 50)
                    {
                        if (MyCommon.IsEnding)
                        {
                            return;
                        }

                        i = 0;
                    }
                }

                if (MyCommon.IsEnding)
                {
                    return;
                }

                // バージョンチェック（引数：起動時チェックの場合はTrue･･･チェック結果のメッセージを表示しない）
                if (this.settingDialog.StartupVersion)
                {
                    this.CheckNewVersion(true);
                }

                // 取得失敗の場合は再試行する
                if (!this.tw.GetFollowersSuccess && this.settingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, tabNameAny);
                }

                // 取得失敗の場合は再試行する
                if (this.settingDialog.TwitterConfiguration.PhotoSizeLimit == 0)
                {
                    this.GetTimeline(WorkerType.Configuration, 0, 0, tabNameAny);
                }

                // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
                if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ReAuthorizeText);
                    this.TryShowSettingsBox();
                }
            }

            this.isInitializing = false;
            this.timerTimeline.Enabled = true;
        }

        private void TweenMain_Shown(object sender, EventArgs e)
        {
            this.TweenMain_ShownExtracted();
        }

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            this.TryRestartApplication();
        }

        private void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Twurl);
        }

        private void TryUnfollowCurrentIconUser()
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.RemoveCommand(id, false);
                }
            }
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryUnfollowCurrentIconUser();
        }

        private void UndoRemoveTab()
        {
            if (this.statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TabClass tb = this.statuses.RemovedTab.Pop();
            string renamed = tb.TabName;
            for (int i = 1; i <= int.MaxValue; i++)
            {
                if (!this.statuses.ContainsTab(renamed))
                {
                    break;
                }
                renamed = string.Format("{0}({1})", tb.TabName, i);
            }
            tb.TabName = renamed;
            this.statuses.Tabs.Add(renamed, tb);
            this.AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
            this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
            this.SaveConfigsTabs();
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            this.UndoRemoveTab();
        }

        private void ChangeSelectedTweetReadSateToUnread()
        {
            this.curList.BeginUpdate();
            if (this.settingDialog.UnreadManage)
            {
                foreach (int idx in this.curList.SelectedIndices)
                {
                    this.statuses.SetReadAllTab(false, this.curTab.Text, idx);
                }
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(false, idx, this.curTab);
            }

            this.ColorizeList();
            this.curList.EndUpdate();
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (this.settingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == -1)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeSelectedTweetReadSateToUnread();
        }

        private void ChangeCurrentTabUnreadManagement(bool isManage)
        {
            this.UreadManageMenuItem.Checked = this.UnreadMngTbMenuItem.Checked = isManage;
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.ChangeTabUnreadManage(this.rclickTabName, isManage);
            this.SaveConfigsTabs();
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeCurrentTabUnreadManagement(((ToolStripMenuItem)sender).Checked);
        }

        private void ConvertUrlByAutoSelectedService()
        {
            if (!this.ConvertUrl(this.settingDialog.AutoShortUrlFirst))
            {
                // 前回使用した短縮URLサービス以外を選択する
                UrlConverter svc = this.settingDialog.AutoShortUrlFirst;
                Random rnd = new Random();
                do
                {
                    svc = (UrlConverter)rnd.Next(System.Enum.GetNames(typeof(UrlConverter)).Length);
                }
                while (!(svc != this.settingDialog.AutoShortUrlFirst && svc != UrlConverter.Nicoms && svc != UrlConverter.Unu));
                this.ConvertUrl(svc);
            }
        }

        private void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrlByAutoSelectedService();
        }

        private void TryCopyUrlInCurrentTweet()
        {
            try
            {
                MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + this.postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?:// [^\"]+)\"", RegexOptions.IgnoreCase);
                foreach (Match m in mc)
                {
                    if (m.Groups["url"].Value == this.postBrowserStatusText)
                    {
                        Clipboard.SetDataObject(m.Groups["title"].Value, false, 5, 100);
                        break;
                    }
                }

                if (mc.Count == 0)
                {
                    Clipboard.SetDataObject(this.postBrowserStatusText, false, 5, 100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            this.TryCopyUrlInCurrentTweet();
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoUrlUndo();
        }

        private void TrySetHashtagFromCurrentTweet()
        {
            Match m = Regex.Match(this.postBrowserStatusText, "^https?://twitter.com/search\\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                // 使用ハッシュタグとして設定
                this.HashMgr.SetPermanentHash("#" + m.Result("${hash}"));
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashToggleToolStripMenuItem.Checked = true;
                this.modifySettingCommon = true;
            }
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            this.TrySetHashtagFromCurrentTweet();
        }

        private void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenFavorarePageOfCurrentTweetUser();
        }

        private void TryOpenCurrentNameLabelUserHome()
        {
            if (this.NameLabel.Tag != null)
            {
                this.OpenUriAsync("https://twitter.com/" + (string)this.NameLabel.Tag);
            }
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            TryOpenCurrentNameLabelUserHome();
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.ChangeUserPictureCursor(Cursors.Hand);
        }

        private void ChangeUserPictureCursor(Cursor cursorsDefault)
        {
            this.UserPicture.Cursor = cursorsDefault;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            ChangeUserPictureCursor(Cursors.Default);
        }

        private void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryShowStatusOfCurrentTweetUser();
        }

        private void TryAddNewUserTimelineTab()
        {
            string id = this.GetUserIdFromCurPostOrInput("Show UserTimeline");
            if (!string.IsNullOrEmpty(id))
            {
                this.AddNewTabForUserTimeline(id);
            }
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryAddNewUserTimelineTab();
        }

        private void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Uxnu);
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            this.CheckNewVersion();
        }

        #region callback

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = this.tw.GetInfoApi(args.Info);
        }

        private void FollowCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostFollowCommand(arg.Id);
        }

        private void RemoveCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostRemoveCommand(arg.Id);
        }

        private void ShowFriendship_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowFriendshipArgs arg = (ShowFriendshipArgs)e.Argument;
            string result = string.Empty;
            foreach (ShowFriendshipArgs.FriendshipInfo fInfo in arg.Ids)
            {
                string rt = arg.Tw.GetFriendshipInfo(fInfo.Id, ref fInfo.IsFollowing, ref fInfo.IsFollowed);
                if (!string.IsNullOrEmpty(rt))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = rt;
                    }

                    fInfo.IsError = true;
                }
            }

            e.Result = result;
        }

        private void GetUserInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            GetUserInfoArgs args = (GetUserInfoArgs)e.Argument;
            e.Result = args.Tw.GetUserInfo(args.Id, ref args.User);
        }

        private void GetRetweet_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            this.tw.GetStatusRetweetedCount(this.CurPost.OriginalStatusId, ref counter);
            e.Result = counter;
        }

        private void GetTimelineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            if (bw.CancellationPending || MyCommon.IsEnding)
            {
                e.Cancel = true;
                return;
            }

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            //// Tween.My.MyProject.Application.InitCulture(); // TODO: Need this here?
            string ret = string.Empty;
            GetWorkerResult rslt = new GetWorkerResult();
            bool read = !this.settingDialog.UnreadManage;
            if (this.isInitializing && this.settingDialog.UnreadManage)
            {
                read = this.settingDialog.Readed;
            }

            GetWorkerArg args = (GetWorkerArg)e.Argument;
            if (!CheckAccountValid())
            {
                // エラー表示のみ行ない、後処理キャンセル
                rslt.RetMsg = "Auth error. Check your account";
                rslt.WorkerType = WorkerType.ErrorState;
                rslt.TabName = args.TabName;
                e.Result = rslt;
                return;
            }

            if (args.WorkerType != WorkerType.OpenUri)
            {
                bw.ReportProgress(0, string.Empty);
            }

            // Notifyアイコンアニメーション開始
            switch (args.WorkerType)
            {
                case WorkerType.Timeline:
                case WorkerType.Reply:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, this.isInitializing);
                    if (string.IsNullOrEmpty(ret) && args.WorkerType == WorkerType.Timeline && this.settingDialog.ReadOldPosts)
                    {
                        // 新着時未読クリア
                        this.statuses.SetRead();
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.DirectMessegeRcv:
                    // 送信分もまとめて取得
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeSnt, args.Page == -1);
                    }

                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.FavAdd:
                    // スレッド処理はしない
                    if (this.statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this.statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = null;
                            if (tbc.IsInnerStorageTabType)
                            {
                                post = tbc.Posts[args.Ids[i]];
                            }
                            else
                            {
                                post = this.statuses.Item(args.Ids[i]);
                            }

                            args.Page = i + 1;
                            bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                            if (!post.IsFav)
                            {
                                ret = this.tw.PostFavAdd(post.OriginalStatusId);
                                if (ret.Length == 0)
                                {
                                    // リスト再描画必要
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
                                    this.favTimestamps.Add(DateTime.Now);
                                    if (string.IsNullOrEmpty(post.RelTabName))
                                    {
                                        // 検索,リストUserTimeline.Relatedタブからのfavは、favタブへ追加せず。それ以外は追加
                                        this.statuses.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                    }
                                    else
                                    {
                                        // 検索,リスト,UserTimeline.Relatedタブからのfavで、TLでも取得済みならfav反映
                                        if (this.statuses.ContainsKey(post.StatusId))
                                        {
                                            PostClass postTl = this.statuses.Item(post.StatusId);
                                            postTl.IsFav = true;
                                            this.statuses.GetTabByType(TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, false);
                                        }
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
                                    {
                                        if (tb.Contains(post.StatusId))
                                        {
                                            tb.Posts[post.StatusId].IsFav = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    rslt.SIds = args.SIds;
                    break;
                case WorkerType.FavRemove:
                    // スレッド処理はしない
                    if (this.statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this.statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = tbc.IsInnerStorageTabType ? tbc.Posts[args.Ids[i]] : this.statuses.Item(args.Ids[i]);
                            args.Page = i + 1;
                            bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                            if (post.IsFav)
                            {
                                ret = this.tw.PostFavRemove(post.OriginalStatusId);
                                if (ret.Length == 0)
                                {
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = false;

                                    // リスト再描画必要
                                    if (this.statuses.ContainsKey(post.StatusId))
                                    {
                                        this.statuses.Item(post.StatusId).IsFav = false;
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
                                    {
                                        if (tb.Contains(post.StatusId))
                                        {
                                            tb.Posts[post.StatusId].IsFav = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    rslt.SIds = args.SIds;
                    break;
                case WorkerType.PostMessage:
                    bw.ReportProgress(200);
                    if (string.IsNullOrEmpty(args.PStatus.ImagePath))
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            ret = this.tw.PostStatus(args.PStatus.Status, args.PStatus.InReplyToId);
                            if (string.IsNullOrEmpty(ret) || ret.StartsWith("OK:") || ret.StartsWith("Outputz:") || ret.StartsWith("Warn:") || ret == "Err:Status is a duplicate." || args.PStatus.Status.StartsWith("D", StringComparison.OrdinalIgnoreCase) || args.PStatus.Status.StartsWith("DM", StringComparison.OrdinalIgnoreCase) || Twitter.AccountState != AccountState.Valid)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        ret = this.pictureServices[args.PStatus.ImageService].Upload(ref args.PStatus.ImagePath, ref args.PStatus.Status, args.PStatus.InReplyToId);
                    }

                    bw.ReportProgress(300);
                    rslt.PStatus = args.PStatus;
                    break;
                case WorkerType.Retweet:
                    bw.ReportProgress(200);
                    for (int i = 0; i < args.Ids.Count; i++)
                    {
                        ret = this.tw.PostRetweet(args.Ids[i], read);
                    }

                    bw.ReportProgress(300);
                    break;
                case WorkerType.Follower:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText1);
                    ret = this.tw.GetFollowersApi();
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this.tw.GetNoRetweetIdsApi();
                    }

                    break;
                case WorkerType.Configuration:
                    ret = this.tw.ConfigurationApi();
                    break;
                case WorkerType.OpenUri:
                    string myPath = Convert.ToString(args.Url);
                    try
                    {
                        if (!string.IsNullOrEmpty(this.settingDialog.BrowserPath))
                        {
                            if (this.settingDialog.BrowserPath.StartsWith("\"") && this.settingDialog.BrowserPath.Length > 2 && this.settingDialog.BrowserPath.IndexOf("\"", 2) > -1)
                            {
                                int sep = this.settingDialog.BrowserPath.IndexOf("\"", 2);
                                string browserPath = this.settingDialog.BrowserPath.Substring(1, sep - 1);
                                string arg = string.Empty;
                                if (sep < this.settingDialog.BrowserPath.Length - 1)
                                {
                                    arg = this.settingDialog.BrowserPath.Substring(sep + 1);
                                }

                                myPath = arg + " " + myPath;
                                Process.Start(browserPath, myPath);
                            }
                            else
                            {
                                Process.Start(this.settingDialog.BrowserPath, myPath);
                            }
                        }
                        else
                        {
                            Process.Start(myPath);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    break;
                case WorkerType.Favorites:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetFavoritesApi(read, args.WorkerType, args.Page == -1);
                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.PublicSearch:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch))
                        {
                            if (!string.IsNullOrEmpty(tb.SearchWords))
                            {
                                ret = this.tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetSearch(read, tb, false);
                            if (string.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = this.tw.GetSearch(read, tb, true);
                            }
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.UserTimeline:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    int count = 20;
                    if (this.settingDialog.UseAdditionalCount)
                    {
                        count = this.settingDialog.UserTimelineCountApi;
                    }

                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.UserTimeline))
                        {
                            if (!string.IsNullOrEmpty(tb.User))
                            {
                                ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                     // 振り分け
                    break;
                case WorkerType.List:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        // 定期更新
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.Lists))
                        {
                            if (tb.ListInfo != null && tb.ListInfo.Id != 0)
                            {
                                ret = this.tw.GetListStatus(read, tb, false, this.isInitializing);
                            }
                        }
                    }
                    else
                    {
                        // 手動更新（特定タブのみ更新）
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetListStatus(read, tb, args.Page == -1, this.isInitializing);
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts(); // 振り分け
                    break;
                case WorkerType.Related:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetRelatedResult(read, this.statuses.GetTabByName(args.TabName));
                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.BlockIds:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateBlockUserText1);
                    ret = this.tw.GetBlockUserIds();
                    if (TabInformations.GetInstance().BlockIds.Count == 0)
                    {
                        this.tw.GetBlockUserIds();
                    }

                    break;
            }

            // キャンセル要求
            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // 時速表示用
            if (args.WorkerType == WorkerType.FavAdd)
            {
                System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                for (int i = this.favTimestamps.Count - 1; i >= 0; i += -1)
                {
                    if (this.favTimestamps[i].CompareTo(oneHour) < 0)
                    {
                        this.favTimestamps.RemoveAt(i);
                    }
                }
            }

            if (args.WorkerType == WorkerType.Timeline && !this.isInitializing)
            {
                lock (this.syncObject)
                {
                    DateTime tm = DateTime.Now;
                    if (this.timeLineTimestamps.ContainsKey(tm))
                    {
                        this.timeLineTimestamps[tm] += rslt.AddCount;
                    }
                    else
                    {
                        this.timeLineTimestamps.Add(tm, rslt.AddCount);
                    }

                    DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                    List<DateTime> keys = new List<DateTime>();
                    this.timeLineCount = 0;
                    foreach (DateTime key in this.timeLineTimestamps.Keys)
                    {
                        if (key.CompareTo(oneHour) < 0)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            this.timeLineCount += this.timeLineTimestamps[key];
                        }
                    }

                    foreach (DateTime key in keys)
                    {
                        this.timeLineTimestamps.Remove(key);
                    }

                    keys.Clear();
                }
            }

            // 終了ステータス
            if (args.WorkerType != WorkerType.OpenUri)
            {
                bw.ReportProgress(100, this.MakeStatusMessage(args, true));
            }

            // ステータス書き換え、Notifyアイコンアニメーション開始
            rslt.RetMsg = ret;
            rslt.WorkerType = args.WorkerType;
            rslt.TabName = args.TabName;
            if (args.WorkerType == WorkerType.DirectMessegeRcv
                || args.WorkerType == WorkerType.DirectMessegeSnt
                || args.WorkerType == WorkerType.Reply
                || args.WorkerType == WorkerType.Timeline
                || args.WorkerType == WorkerType.Favorites)
            {
                // 値が正しいか後でチェック。10ページ毎の継続確認
                rslt.Page = args.Page - 1;
            }

            e.Result = rslt;
        }

        private void GetTimelineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (MyCommon.IsEnding || e.Cancelled)
            {
                // キャンセル
                return;
            }

            if (e.Error != null)
            {
                this.myStatusError = true;
                this.waitTimeline = false;
                this.waitReply = false;
                this.waitDm = false;
                this.waitFav = false;
                this.waitPubSearch = false;
                this.waitUserTimeline = false;
                this.waitLists = false;
                throw new Exception("BackgroundWorker Exception", e.Error);
            }

            GetWorkerResult rslt = (GetWorkerResult)e.Result;
            if (rslt.WorkerType == WorkerType.OpenUri)
            {
                return;
            }

            // エラー
            if (rslt.RetMsg.Length > 0)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = rslt.RetMsg;
            }

            if (rslt.WorkerType == WorkerType.ErrorState)
            {
                return;
            }

            if (rslt.WorkerType == WorkerType.FavRemove)
            {
                this.RemovePostFromFavTab(rslt.SIds.ToArray());
            }

            if (rslt.WorkerType == WorkerType.Timeline
                || rslt.WorkerType == WorkerType.Reply
                || rslt.WorkerType == WorkerType.List
                || rslt.WorkerType == WorkerType.PublicSearch
                || rslt.WorkerType == WorkerType.DirectMessegeRcv
                || rslt.WorkerType == WorkerType.DirectMessegeSnt
                || rslt.WorkerType == WorkerType.Favorites
                || rslt.WorkerType == WorkerType.Follower
                || rslt.WorkerType == WorkerType.FavAdd
                || rslt.WorkerType == WorkerType.FavRemove
                || rslt.WorkerType == WorkerType.Related
                || rslt.WorkerType == WorkerType.UserTimeline
                || rslt.WorkerType == WorkerType.BlockIds
                || rslt.WorkerType == WorkerType.Configuration)
            {
                // リスト反映
                this.RefreshTimeline(false);
            }

            switch (rslt.WorkerType)
            {
                case WorkerType.Timeline:
                    this.waitTimeline = false;
                    if (!this.isInitializing)
                    {
                        // 'API使用時の取得調整は別途考える（カウント調整？）
                    }

                    break;
                case WorkerType.Reply:
                    this.waitReply = false;
                    if (rslt.NewDM && !this.isInitializing)
                    {
                        this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    }

                    break;
                case WorkerType.Favorites:
                    this.waitFav = false;
                    break;
                case WorkerType.DirectMessegeRcv:
                    this.waitDm = false;
                    break;
                case WorkerType.FavAdd:
                case WorkerType.FavRemove:
                    if (this.curList != null && this.curTab != null)
                    {
                        this.curList.BeginUpdate();
                        if (rslt.WorkerType == WorkerType.FavRemove && this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.Favorites)
                        {
                            // 色変えは不要
                        }
                        else
                        {
                            for (int i = 0; i < rslt.SIds.Count; i++)
                            {
                                if (this.curTab.Text.Equals(rslt.TabName))
                                {
                                    int idx = this.statuses.Tabs[rslt.TabName].IndexOf(rslt.SIds[i]);
                                    if (idx > -1)
                                    {
                                        TabClass tb = this.statuses.Tabs[rslt.TabName];
                                        if (tb != null)
                                        {
                                            PostClass post = null;
                                            if (tb.TabType == TabUsageType.Lists || tb.TabType == TabUsageType.PublicSearch)
                                            {
                                                post = tb.Posts[rslt.SIds[i]];
                                            }
                                            else
                                            {
                                                post = this.statuses.Item(rslt.SIds[i]);
                                            }

                                            this.ChangeCacheStyleRead(post.IsRead, idx, this.curTab);
                                        }

                                        if (idx == this.curItemIndex)
                                        {
                                            // 選択アイテム再表示
                                            this.DispSelectedPost(true);
                                        }
                                    }
                                }
                            }
                        }

                        this.curList.EndUpdate();
                    }

                    break;
                case WorkerType.PostMessage:
                    if (string.IsNullOrEmpty(rslt.RetMsg) || rslt.RetMsg.StartsWith("Outputz") || rslt.RetMsg.StartsWith("OK:") || rslt.RetMsg == "Warn:Status is a duplicate.")
                    {
                        this.postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this.postTimestamps.Count - 1; i >= 0; i += -1)
                        {
                            if (this.postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this.postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!this.HashMgr.IsPermanent && !string.IsNullOrEmpty(this.HashMgr.UseHash))
                        {
                            this.HashMgr.ClearHashtag();
                            this.HashStripSplitButton.Text = "#[-]";
                            this.HashToggleMenuItem.Checked = false;
                            this.HashToggleToolStripMenuItem.Checked = false;
                        }

                        this.SetMainWindowTitle();
                        rslt.RetMsg = string.Empty;
                    }
                    else
                    {
                        DialogResult retry = default(DialogResult);
                        try
                        {
                            retry = MessageBox.Show(
                                string.Format("{0}   --->   [ {1} ]{2}\"{3}\"{2}{4}", Hoehoe.Properties.Resources.StatusUpdateFailed1, rslt.RetMsg, Environment.NewLine, rslt.PStatus.Status, Hoehoe.Properties.Resources.StatusUpdateFailed2),
                                "Failed to update status", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
                        }
                        catch (Exception)
                        {
                            retry = DialogResult.Abort;
                        }

                        if (retry == DialogResult.Retry)
                        {
                            RunAsync(new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = WorkerType.PostMessage, PStatus = rslt.PStatus });
                        }
                        else
                        {
                            if (ToolStripFocusLockMenuItem.Checked)
                            {
                                // 連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                                this.StatusText_EnterExtracted();
                            }
                        }
                    }

                    if (rslt.RetMsg.Length == 0 && this.settingDialog.PostAndGet)
                    {
                        if (this.isActiveUserstream)
                        {
                            this.RefreshTimeline(true);
                        }
                        else
                        {
                            this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        }
                    }

                    break;
                case WorkerType.Retweet:
                    if (rslt.RetMsg.Length == 0)
                    {
                        this.postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this.postTimestamps.Count - 1; i >= 0; i--)
                        {
                            if (this.postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this.postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!this.isActiveUserstream && this.settingDialog.PostAndGet)
                        {
                            this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        }
                    }

                    break;
                case WorkerType.Follower:
                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    break;
                case WorkerType.Configuration:
                    // this._waitFollower = False
                    if (this.settingDialog.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        this.pictureServices["Twitter"].Configuration("MaxUploadFilesize", this.settingDialog.TwitterConfiguration.PhotoSizeLimit);
                    }

                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    break;
                case WorkerType.PublicSearch:
                    this.waitPubSearch = false;
                    break;
                case WorkerType.UserTimeline:
                    this.waitUserTimeline = false;
                    break;
                case WorkerType.List:
                    this.waitLists = false;
                    break;
                case WorkerType.Related:
                    {
                        TabClass tb = this.statuses.GetTabByType(TabUsageType.Related);
                        if (tb != null && tb.RelationTargetPost != null && tb.Contains(tb.RelationTargetPost.StatusId))
                        {
                            foreach (TabPage tp in this.ListTab.TabPages)
                            {
                                if (tp.Text == tb.TabName)
                                {
                                    ((DetailsListView)tp.Tag).SelectedIndices.Add(tb.IndexOf(tb.RelationTargetPost.StatusId));
                                    ((DetailsListView)tp.Tag).Items[tb.IndexOf(tb.RelationTargetPost.StatusId)].Focused = true;
                                    break;
                                }
                            }
                        }
                    }

                    break;
            }
        }

        private void GrowlHelper_Callback(object sender, GrowlHelper.NotifyCallbackEventArgs e)
        {
            if (Form.ActiveForm == null)
            {
                this.BeginInvoke(
                    new Action(() =>
                    {
                        this.Visible = true;
                        if (WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }

                        this.Activate();
                        this.BringToFront();
                        if (e.NotifyType == GrowlHelper.NotifyType.DirectMessage)
                        {
                            if (!this.GoDirectMessage(e.StatusId))
                            {
                                this.StatusText.Focus();
                            }
                        }
                        else
                        {
                            if (!this.GoStatus(e.StatusId))
                            {
                                this.StatusText.Focus();
                            }
                        }
                    }));
            }
        }

        #endregion callback

        #region ListTab events

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;
            this.prevSelectedTab = e.TabPage;
        }

        private void ListTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            string txt = null;
            try
            {
                txt = this.ListTab.TabPages[e.Index].Text;
            }
            catch (Exception)
            {
                return;
            }

            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            if (e.State == DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }

            Brush fore = null;
            try
            {
                if (this.statuses.Tabs[txt].UnreadCount > 0)
                {
                    fore = Brushes.Red;
                }
                else
                {
                    fore = SystemBrushes.ControlText;
                }
            }
            catch (Exception)
            {
                fore = SystemBrushes.ControlText;
            }

            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, this.tabStringFormat);
        }

        private void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ListTab.SelectedTab != null)
            {
                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    Control pnl = this.ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused || pnl.Controls["comboLang"].Focused || pnl.Controls["buttonSearch"].Focused)
                    {
                        return;
                    }
                }

                ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
                if (modState == ModifierState.NotFlags)
                {
                    return;
                }

                if (modState != ModifierState.None)
                {
                    this.anchorFlag = false;
                }

                if (this.CommonKeyDown(e.KeyCode, FocusedControl.ListTab, modState))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void ListTab_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this.RemoveSpecifiedTab(this.ListTab.TabPages[i].Text, true);
                        this.SaveConfigsTabs();
                        break;
                    }
                }
            }
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D
            if (!this.settingDialog.TabMouseLock && e.Button == MouseButtons.Left && this.tabDraging)
            {
                string tn = string.Empty;
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(this.tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(this.tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    // タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = this.ListTab.SelectedTab.Text;
                }

                if (string.IsNullOrEmpty(tn))
                {
                    return;
                }

                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    if (tb.Text == tn)
                    {
                        this.ListTab.DoDragDrop(tb, DragDropEffects.All);
                        break;
                    }
                }
            }
            else
            {
                this.tabDraging = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Left <= cpos.X & cpos.X <= rect.Right & rect.Top <= cpos.Y & cpos.Y <= rect.Bottom)
                {
                    this.rclickTabName = this.ListTab.TabPages[i].Text;
                    break;
                }
            }
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            this.tabDraging = false;
        }

        private void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this._curList.Refresh()
            this.DispSelectedPost();
            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (this.ListTab.Focused || ((Control)this.ListTab.SelectedTab.Tag).Focused)
            {
                this.Tag = this.ListTab.Tag;
            }

            this.TabMenuControl(this.ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.ListTabSelect(e.TabPage);
        }

        #endregion

        #endregion event handler
    }
}