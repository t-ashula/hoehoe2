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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using Hoehoe.TweenCustomControl;

    public partial class TweenMain
    {
        #region private methods

        private static bool CheckAccountValid()
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                accountCheckErrorCount += 1;
                if (accountCheckErrorCount > 5)
                {
                    accountCheckErrorCount = 0;
                    Twitter.AccountState = AccountState.Valid;
                    return true;
                }

                return false;
            }

            accountCheckErrorCount = 0;
            return true;
        }

        private static void MoveArrayItem(int[] values, int fromIndex, int toIndex)
        {
            int movedValue = values[fromIndex];
            int numMoved = Math.Abs(fromIndex - toIndex);
            if (toIndex < fromIndex)
            {
                Array.Copy(values, toIndex, values, toIndex + 1, numMoved);
            }
            else
            {
                Array.Copy(values, fromIndex + 1, values, fromIndex, numMoved);
            }

            values[toIndex] = movedValue;
        }

        private static void ChangeTraceFlag(bool trace)
        {
            MyCommon.TraceFlag = trace;
        }
        
        private void CheckReplyTo(string statusText)
        {
            // ハッシュタグの保存
            MatchCollection m = Regex.Matches(statusText, Twitter.HashtagRegexPattern, RegexOptions.IgnoreCase);
            string hstr = string.Empty;
            foreach (Match hm in m)
            {
                if (!hstr.Contains("#" + hm.Result("$3") + " "))
                {
                    hstr += "#" + hm.Result("$3") + " ";
                    this.HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && !hstr.Contains(this.HashMgr.UseHash + " "))
            {
                hstr += this.HashMgr.UseHash;
            }

            if (!string.IsNullOrEmpty(hstr))
            {
                this.HashMgr.AddHashToHistory(hstr.Trim(), false);
            }

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(statusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");
            if (this.settingDialog.UseAtIdSupplement)
            {
                if (this.AtIdSupl.AddRangeItem(m.Cast<Match>().Select(mid => mid.Result("${id}"))))
                {
                    this.SetModifySettingAtId(true);
                }
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (this.replyToId == 0)
            {
                return;
            }

            // リプライ先ユーザー名がない場合も指定しない
            if (string.IsNullOrEmpty(this.replyToName))
            {
                this.replyToId = 0;
                return;
            }

            // 通常Reply
            // 次の条件を満たす場合に in_reply_to_status_id 指定
            // 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
            // 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
            // 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する
            if (m != null)
            {
                if (statusText.StartsWith("@"))
                {
                    if (statusText.StartsWith("@" + this.replyToName))
                    {
                        return;
                    }
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (statusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + this.replyToName)
                        {
                            return;
                        }
                    }
                }
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
        }

        private Icon LoadIcon(string dir, string fileName, Icon defIcon)
        {
            if (!Directory.Exists(dir))
            {
                return defIcon;
            }

            var fullname = Path.Combine(dir, fileName);
            if (File.Exists(fullname))
            {
                return defIcon;
            }

            try
            {
                return new Icon(fullname);
            }
            catch
            {
                return defIcon;
            }
        }

        private void LoadIcons()
        {
            // 着せ替えアイコン対応
            string iconDir = Path.Combine(Application.StartupPath, "Icons");
            this.iconAt = LoadIcon(iconDir, "At.ico", Hoehoe.Properties.Resources.At);                 // タスクトレイ通常時アイコン
            this.iconAtRed = LoadIcon(iconDir, "AtRed.ico", Hoehoe.Properties.Resources.AtRed);        // タスクトレイエラー時アイコン
            this.iconAtSmoke = LoadIcon(iconDir, "AtSmoke.ico", Hoehoe.Properties.Resources.AtSmoke);  // タスクトレイオフライン時アイコン
            this.tabIcon = LoadIcon(iconDir, "Tab.ico", Hoehoe.Properties.Resources.TabIcon);          // タブ見出し未読表示アイコン
            this.mainIcon = LoadIcon(iconDir, "MIcon.ico", Hoehoe.Properties.Resources.MIcon);         // 画面のアイコン
            this.replyIcon = LoadIcon(iconDir, "Reply.ico", Hoehoe.Properties.Resources.Reply);         // Replyのアイコン
            this.replyIconBlink = LoadIcon(iconDir, "ReplyBlink.ico", Hoehoe.Properties.Resources.ReplyBlink);            // Reply点滅のアイコン

            // タスクトレイ更新中アイコン アニメーション対応により4種類読み込み
            this.iconRefresh[0] = LoadIcon(iconDir, "Refresh.ico", Hoehoe.Properties.Resources.Refresh);
            this.iconRefresh[1] = LoadIcon(iconDir, "Refresh2.ico", Hoehoe.Properties.Resources.Refresh2);
            this.iconRefresh[2] = LoadIcon(iconDir, "Refresh3.ico", Hoehoe.Properties.Resources.Refresh3);
            this.iconRefresh[3] = LoadIcon(iconDir, "Refresh4.ico", Hoehoe.Properties.Resources.Refresh4);
        }

        private int GetSortColumnIndex(IdComparerClass.ComparerMode sortMode)
        {
            int c = 0;
            switch (sortMode)
            {
                case IdComparerClass.ComparerMode.Nickname: // ニックネーム
                    c = 1;
                    break;
                case IdComparerClass.ComparerMode.Data:     // 本文
                    c = 2;
                    break;
                case IdComparerClass.ComparerMode.Id:       // 時刻=発言Id
                    c = 3;
                    break;
                case IdComparerClass.ComparerMode.Name:     // 名前
                    c = 4;
                    break;
                case IdComparerClass.ComparerMode.Source:   // Source
                    c = 7;
                    break;
            }
            return c;
        }

        private void InitColumnText()
        {
            var columns = new[] { 
                string.Empty, 
                Hoehoe.Properties.Resources.AddNewTabText2, 
                Hoehoe.Properties.Resources.AddNewTabText3, 
                Hoehoe.Properties.Resources.AddNewTabText4_2, 
                Hoehoe.Properties.Resources.AddNewTabText5, 
                string.Empty, 
                string.Empty, 
                "Source" 
            };
            for (var i = 0; i < columns.Length; ++i)
            {
                this.columnTexts[i] = this.columnOrgTexts[i] = columns[i];
            }

            // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE, U+25B4 BLACK UP-POINTING SMALL TRIANGLE
            string mark = this.statuses.SortOrder == SortOrder.Descending ? "▾" : "▴";
            int c = this.iconCol ? 2 : GetSortColumnIndex(this.statuses.SortMode);
            this.columnTexts[c] = this.columnOrgTexts[c] + mark;
        }

        private void InitializeTraceFrag()
        {
#if DEBUG
            TraceOutToolStripMenuItem.Checked = true;
            MyCommon.TraceFlag = true;
#endif
            if (!MyCommon.FileVersion.EndsWith("0"))
            {
                this.TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void CreatePictureServices()
        {
            if (this.pictureServices != null)
            {
                this.pictureServices.Clear();
            }

            this.pictureServices = new Dictionary<string, IMultimediaShareService>
            {
                { "TwitPic", new TwitPic(this.tw) },
                { "img.ly", new Imgly(this.tw) },
                { "yfrog", new Yfrog(this.tw) },
                { "lockerz", new Plixi(this.tw) },
                { "Twitter", new TwitterPhoto(this.tw) }
            };
        }

        private void LoadConfig()
        {
            this.cfgCommon = SettingCommon.Load();
            if (this.cfgCommon.UserAccounts == null || this.cfgCommon.UserAccounts.Count == 0)
            {
                this.cfgCommon.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(this.cfgCommon.UserName))
                {
                    this.cfgCommon.UserAccounts.Add(new UserAccount
                    {
                        Username = this.cfgCommon.UserName,
                        UserId = this.cfgCommon.UserId,
                        Token = this.cfgCommon.Token,
                        TokenSecret = this.cfgCommon.TokenSecret
                    });
                }
            }

            this.cfgLocal = SettingLocal.Load();
            var tabs = SettingTabs.Load().Tabs;
            foreach (var tb in tabs)
            {
                try
                {
                    this.statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = this.statuses.GetUniqueTabName();
                    this.statuses.Tabs.Add(tb.TabName, tb);
                }
            }

            if (this.statuses.Tabs.Count == 0)
            {
                this.statuses.AddTab(MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                this.statuses.AddTab(MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                this.statuses.AddTab(MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                this.statuses.AddTab(MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
            }
        }

        private void RefreshTimeline(bool isUserStream)
        {
            if (isUserStream)
            {
                this.RefreshTasktrayIcon(true);
            }

            // スクロール制御準備
            int smode = -1; // -1:制御しない,-2:最新へ,その他:topitem使用
            long topId = this.GetScrollPos(ref smode);
            int befCnt = this.curList.VirtualListSize;

            // 現在の選択状態を退避
            var selId = new Dictionary<string, long[]>();
            var focusedId = new Dictionary<string, long>();
            this.SaveSelectedStatus(selId, focusedId);

            // mentionsの更新前件数を保持
            int dmessageCount = this.statuses.GetTabByType(TabUsageType.DirectMessage).AllCount;

            // 更新確定
            PostClass[] notifyPosts = null;
            string soundFile = string.Empty;
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = this.statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon.IsEnding)
            {
                return;
            }

            // リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in this.ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = this.statuses.Tabs[tab.Text];
                    lst.BeginUpdate();
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        if (lst.Equals(this.curList))
                        {
                            this.itemCache = null;
                            this.postCache = null;
                        }

                        try
                        {
                            // リスト件数更新
                            lst.VirtualListSize = tabInfo.AllCount;
                        }
                        catch (Exception)
                        {
                            // アイコン描画不具合あり？
                        }

                        this.SelectListItem(lst, this.statuses.IndexOf(tab.Text, selId[tab.Text]), this.statuses.IndexOf(tab.Text, focusedId[tab.Text]));
                    }

                    lst.EndUpdate();
                    if (tabInfo.UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            if (tab.ImageIndex == -1)
                            {
                                // タブアイコン
                                tab.ImageIndex = 0;
                            }
                        }
                    }
                }

                if (!this.settingDialog.TabIconDisp)
                {
                    this.ListTab.Refresh();
                }
            }
            catch (Exception)
            {
            }

            // スクロール制御後処理
            if (smode != -1)
            {
                try
                {
                    if (befCnt != this.curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                // 最上行
                                if (this.curList.VirtualListSize > 0)
                                {
                                    this.curList.EnsureVisible(0);
                                }

                                break;
                            case -2:
                                // 最下行へ
                                if (this.curList.VirtualListSize > 0)
                                {
                                    this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
                                }

                                break;
                            case -1:
                                // 制御しない
                                break;
                            default:
                                // 表示位置キープ
                                if (this.curList.VirtualListSize > 0 && this.statuses.IndexOf(this.curTab.Text, topId) > -1)
                                {
                                    this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
                                    this.curList.EnsureVisible(this.statuses.IndexOf(this.curTab.Text, topId));
                                }

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Msg"] = "Ref2";
                    throw;
                }
            }

            // 新着通知
            this.NotifyNewPosts(notifyPosts, soundFile, addCount, isMention || dmessageCount != this.statuses.GetTabByType(TabUsageType.DirectMessage).AllCount);

            this.SetMainWindowTitle();
            if (!this.StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }

            this.HashSupl.AddRangeItem(this.tw.GetHashList());
        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (this.curList == null || this.curTab == null || this.curList.VirtualListSize <= 0)
            {
                smode = -1;
                return topId;
            }

            if (this.statuses.SortMode != IdComparerClass.ComparerMode.Id)
            {
                // 現在表示位置へ強制スクロール
                if (this.curList.TopItem != null)
                {
                    topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                }

                smode = 0;
                return topId;
            }

            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                // Id昇順
                if (this.ListLockMenuItem.Checked)
                {
                    // 制御しない(現在表示位置へ強制スクロール)
                    smode = -1;
                }
                else
                {
                    // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                    ListViewItem item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
                    if (item == null)
                    {
                        // 一番下
                        item = this.curList.Items[this.curList.Items.Count - 1];
                    }

                    if (item.Index == this.curList.Items.Count - 1)
                    {
                        smode = -2;
                    }
                    else
                    {
                        smode = -1;
                    }
                }
            }
            else
            {
                // Id降順
                if (this.ListLockMenuItem.Checked)
                {
                    // 現在表示位置へ強制スクロール
                    if (this.curList.TopItem != null)
                    {
                        topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                    }

                    smode = 0;
                }
                else
                {
                    // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                    ListViewItem item = this.curList.GetItemAt(0, 10);
                    if (item == null)
                    {
                        // 一番上
                        item = this.curList.Items[0];
                    }

                    if (item.Index == 0)
                    {
                        // 最上行
                        smode = -3;
                    }
                    else
                    {
                        if (this.curList.TopItem != null)
                        {
                            topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                        }
                        smode = 0;
                    }
                }
            }

            return topId;
        }

        private void SaveSelectedStatus(Dictionary<string, long[]> selId, Dictionary<string, long> focusedId)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            foreach (TabPage tab in this.ListTab.TabPages)
            {
                var lst = (DetailsListView)tab.Tag;
                if (lst.SelectedIndices.Count > 0 && lst.SelectedIndices.Count < 61)
                {
                    selId.Add(tab.Text, this.statuses.GetId(tab.Text, lst.SelectedIndices.Cast<int>()));
                }
                else
                {
                    selId.Add(tab.Text, new long[1] { -2 });
                }

                if (lst.FocusedItem != null)
                {
                    focusedId.Add(tab.Text, this.statuses.GetId(tab.Text, lst.FocusedItem.Index));
                }
                else
                {
                    focusedId.Add(tab.Text, -2);
                }
            }
        }

        private bool IsBalloonRequired()
        {
            return this.IsBalloonRequired(new Twitter.FormattedEvent { Eventtype = EventType.None });
        }

        private bool IsBalloonRequired(Twitter.FormattedEvent ev)
        {
            return this.IsEventNotifyAsEventType(ev.Eventtype)
                && this.IsMyEventNotityAsEventType(ev)
                && (this.NewPostPopMenuItem.Checked || (this.settingDialog.ForceEventNotify && ev.Eventtype != EventType.None))
                && !this.isInitializing
                && ((this.settingDialog.LimitBalloon && (this.WindowState == FormWindowState.Minimized || !this.Visible || Form.ActiveForm == null)) || !this.settingDialog.LimitBalloon)
                && !Win32Api.IsScreenSaverRunning();
        }

        private bool IsEventNotifyAsEventType(EventType type)
        {
            return (this.settingDialog.EventNotifyEnabled && Convert.ToBoolean(type & this.settingDialog.EventNotifyFlag)) || type == EventType.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return Convert.ToBoolean(ev.Eventtype & this.settingDialog.IsMyEventNotifyFlag) ? true : !ev.IsMe;
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null
                && notifyPosts.Count() > 0
                && this.settingDialog.ReadOwnPost
                && notifyPosts.All(post => post.UserId == this.tw.UserId || post.ScreenName == this.tw.Username))
            {
                return;
            }

            // 新着通知
            if (this.IsBalloonRequired() && notifyPosts != null && notifyPosts.Length > 0)
            {
                // Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                if (this.settingDialog.IsNotifyUseGrowl)
                {
                    StringBuilder sb = new StringBuilder();
                    bool reply = false;
                    bool dm = false;
                    foreach (var post in notifyPosts)
                    {
                        if (!(notifyPosts.Count() > 3))
                        {
                            sb.Clear();
                            reply = false;
                            dm = false;
                        }

                        if (post.IsReply && !post.IsExcludeReply)
                        {
                            reply = true;
                        }

                        if (post.IsDm)
                        {
                            dm = true;
                        }

                        if (sb.Length > 0)
                        {
                            sb.Append(Environment.NewLine);
                        }

                        switch (this.settingDialog.NameBalloon)
                        {
                            case NameBalloonEnum.UserID:
                                sb.Append(post.ScreenName).Append(" : ");
                                break;
                            case NameBalloonEnum.NickName:
                                sb.Append(post.Nickname).Append(" : ");
                                break;
                        }

                        sb.Append(post.TextFromApi);
                        if (notifyPosts.Count() > 3)
                        {
                            if (!object.ReferenceEquals(notifyPosts.Last(), post))
                            {
                                continue;
                            }
                        }

                        string notifyText = sb.ToString();
                        if (string.IsNullOrEmpty(notifyText))
                        {
                            return;
                        }

                        var titleStr = GetNotifyTitlteText(addCount, reply, dm);
                        var nt = dm ? GrowlHelper.NotifyType.DirectMessage :
                            reply ? GrowlHelper.NotifyType.Reply :
                            GrowlHelper.NotifyType.Notify;
                        this.growlHelper.Notify(nt, post.StatusId.ToString(), titleStr, notifyText, this.iconDict[post.ImageUrl], post.ImageUrl);
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    bool reply = false;
                    bool dm = false;
                    foreach (var post in notifyPosts)
                    {
                        if (post.IsReply && !post.IsExcludeReply)
                        {
                            reply = true;
                        }

                        if (post.IsDm)
                        {
                            dm = true;
                        }

                        if (sb.Length > 0)
                        {
                            sb.Append(Environment.NewLine);
                        }

                        switch (this.settingDialog.NameBalloon)
                        {
                            case NameBalloonEnum.UserID:
                                sb.Append(post.ScreenName).Append(" : ");
                                break;
                            case NameBalloonEnum.NickName:
                                sb.Append(post.Nickname).Append(" : ");
                                break;
                        }

                        sb.Append(post.TextFromApi);
                    }

                    string notifyText = sb.ToString();
                    if (string.IsNullOrEmpty(notifyText))
                    {
                        return;
                    }

                    var titleStr = this.GetNotifyTitlteText(addCount, reply, dm);
                    var notifyIcon = dm ? ToolTipIcon.Warning :
                        reply ? ToolTipIcon.Warning :
                        ToolTipIcon.Info;
                    this.NotifyIcon1.BalloonTipTitle = titleStr;
                    this.NotifyIcon1.BalloonTipText = notifyText;
                    this.NotifyIcon1.BalloonTipIcon = notifyIcon;
                    this.NotifyIcon1.ShowBalloonTip(500);
                }
            }

            // サウンド再生
            if (!this.isInitializing && this.settingDialog.PlaySound)
            {
                MyCommon.PlaySound(soundFile);
            }

            // mentions新着時に画面ブリンク
            if (!this.isInitializing && this.settingDialog.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                Win32Api.FlashMyWindow(this.Handle, Hoehoe.Win32Api.FlashSpecification.FlashTray, 3);
            }
        }

        private string GetNotifyTitlteText(int addCount, bool reply, bool dm)
        {
            var title = new StringBuilder();
            if (this.settingDialog.DispUsername)
            {
                title.AppendFormat("{0} - ", this.tw.Username);
            }

            string notifyType = string.Empty,
                msg1 = Hoehoe.Properties.Resources.RefreshTimelineText1,
                msg2 = Hoehoe.Properties.Resources.RefreshTimelineText2;
            if (dm)
            {
                notifyType = "[DM]";
                msg1 = Hoehoe.Properties.Resources.RefreshDirectMessageText1;
                msg2 = Hoehoe.Properties.Resources.RefreshDirectMessageText2;
            }
            else if (reply)
            {
                notifyType = "[Reply!]";
                msg1 = Hoehoe.Properties.Resources.RefreshTimelineText1;
                msg2 = Hoehoe.Properties.Resources.RefreshTimelineText2;
            }

            title.AppendFormat("Hoehoe {0} {1} {2} {3}", notifyType, msg1, addCount, msg2);
            return title.ToString();
        }

        private void ChangeCacheStyleRead(bool read, int index, TabPage tab)
        {
            // Read_:True=既読 False=未読
            // 未読管理していなかったら既読として扱う
            if (!this.statuses.Tabs[this.curTab.Text].UnreadManage || !this.settingDialog.UnreadManage)
            {
                read = true;
            }

            // 対象の特定
            ListViewItem itm = null;
            PostClass post = null;
            if (tab.Equals(this.curTab) && this.itemCache != null && index >= this.itemCacheIndex && index < this.itemCacheIndex + this.itemCache.Length)
            {
                itm = this.itemCache[index - this.itemCacheIndex];
                post = this.postCache[index - this.itemCacheIndex];
            }
            else
            {
                itm = ((DetailsListView)tab.Tag).Items[index];
                post = this.statuses.Item(tab.Text, index);
            }

            this.ChangeItemStyleRead(read, itm, post, (DetailsListView)tab.Tag);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem item, PostClass post, DetailsListView listView)
        {
            bool useUnreadStyle = this.settingDialog.UseUnreadStyle;

            // フォント
            Font fnt = read ? this.fntReaded : this.fntUnread;

            // 文字色
            Color cl = this.clrUnread;
            if (post.IsFav)
            {
                cl = this.clrFav;
            }
            else if (post.IsRetweeted)
            {
                cl = this.clrRetweet;
            }
            else if (post.IsOwl && (post.IsDm || this.settingDialog.OneWayLove))
            {
                cl = this.clrOWL;
            }
            else if (read || !useUnreadStyle)
            {
                cl = this.clrRead;
            }

            item.SubItems[5].Text = read ? string.Empty : "★";

            if (listView == null || item.Index == -1)
            {
                item.ForeColor = cl;
                if (useUnreadStyle)
                {
                    item.Font = fnt;
                }
            }
            else
            {
                listView.Update();
                if (useUnreadStyle)
                {
                    listView.ChangeItemFontAndColor(item.Index, cl, fnt);
                }
                else
                {
                    listView.ChangeItemForeColor(item.Index, cl);
                }
            }
        }

        private void ColorizeList()
        {
            if (this.itemCache == null)
            {
                return;
            }

            var post = this.anchorFlag ? this.anchorPost : this.curPost; 
            if (post == null)
            {
                return;
            }

            try
            {
                for (var cnt = 0; cnt < this.itemCache.Length; ++cnt)
                {
                    this.curList.ChangeItemBackColor(this.itemCacheIndex + cnt, this.JudgeColor(post, this.postCache[cnt]));
                }
            }
            catch (Exception)
            {
            }
        }

        private void ColorizeList(ListViewItem item, int index)
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。-1は全キャッシュ。Colorは返さない（ダミーを戻す）    
            var post = this.anchorFlag ? this.anchorPost : this.curPost;
            if (post == null)
            {
                return;
            }

            var target = this.GetCurTabPost(index);
            if (item.Index == -1)
            {
                item.BackColor = this.JudgeColor(post, target);
            }
            else
            {
                this.curList.ChangeItemBackColor(item.Index, this.JudgeColor(post, target));
            }
        }

        private Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl = this.clrListBackcolor;  // その他
            if (targetPost.StatusId == basePost.InReplyToStatusId)
            {
                cl = this.clrAtTo;             // @先
            }
            else if (targetPost.IsMe)
            {
                cl = this.clrSelf;            // 自分=発言者
            }
            else if (targetPost.IsReply)
            {
                cl = this.clrAtSelf;          // 自分宛返信
            }
            else if (basePost.ReplyToList.Contains(targetPost.ScreenName.ToLower()))
            {
                cl = this.clrAtFromTarget;    // 返信先
            }
            else if (targetPost.ReplyToList.Contains(basePost.ScreenName.ToLower()))
            {
                cl = this.clrAtTarget;        // その人への返信
            }
            else if (targetPost.ScreenName.Equals(basePost.ScreenName, StringComparison.OrdinalIgnoreCase))
            {
                cl = this.clrTarget;          // 発言者
            }
            return cl;
        }

        private string MakeStatusMessage(GetWorkerArg asyncArg, bool isFinish)
        {
            string smsg = string.Empty;
            // 継続中メッセージ
            switch (asyncArg.WorkerType)
            {
                case WorkerType.Timeline:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText1 :
                        string.Format("{0}{1}{2}", Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5, asyncArg.Page, Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.Reply:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText9 :
                        string.Format("{0}{1}{2}", Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText4, asyncArg.Page, Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.DirectMessegeRcv:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText11 :
                        string.Format("{0}{1}{2}", Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText8, asyncArg.Page, Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.FavAdd:
                    // 進捗メッセージ残す
                    smsg = isFinish ?
                        string.Empty :
                        string.Format("{0}{1}/{2}{3}{4}", Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15, asyncArg.Page, asyncArg.Ids.Count, Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText16, asyncArg.Page - asyncArg.SIds.Count - 1);
                    break;
                case WorkerType.FavRemove:
                    // 進捗メッセージ残す
                    smsg = isFinish ?
                        string.Empty :
                        string.Format("{0}{1}/{2}{3}{4}", Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17, asyncArg.Page, asyncArg.Ids.Count, Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText18, asyncArg.Page - asyncArg.SIds.Count - 1);
                    break;
                case WorkerType.Favorites:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText20 :
                        Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText19;
                    break;
                case WorkerType.PublicSearch:
                    smsg = isFinish ?
                        "Search refreshed" :
                        "Search refreshing...";
                    break;
                case WorkerType.List:
                    smsg = isFinish ?
                        "List refreshed" :
                        "List refreshing...";
                    break;
                case WorkerType.Related:
                    smsg = isFinish ?
                        "Related refreshed" :
                        "Related refreshing...";
                    break;
                case WorkerType.UserTimeline:
                    smsg = isFinish ?
                        "UserTimeline refreshed" :
                        "UserTimeline refreshing...";
                    break;
                case WorkerType.Follower:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText3 :
                        string.Empty;
                    break;
                case WorkerType.Configuration:
                    // 進捗メッセージ残す
                    break;
                case WorkerType.BlockIds:
                    smsg = isFinish ?
                        Hoehoe.Properties.Resources.UpdateBlockUserText3 :
                        string.Empty;
                    break;
            }

            return smsg;
        }

        private void RemovePostFromFavTab(long[] ids)
        {
            string favTabName = this.statuses.GetTabByType(TabUsageType.Favorites).TabName;
            bool isCurFavTab = this.curTab.Text.Equals(favTabName);
            int fidx = 0;
            if (isCurFavTab)
            {
                if (this.curList.FocusedItem != null)
                {
                    fidx = this.curList.FocusedItem.Index;
                }
                else if (this.curList.TopItem != null)
                {
                    fidx = this.curList.TopItem.Index;
                }
            }

            foreach (long i in ids)
            {
                try
                {
                    this.statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                }
            }

            if (this.curTab != null && isCurFavTab)
            {
                // キャッシュ破棄
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                //// this._curItemIndex = -1
            }

            foreach (TabPage tp in this.ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = this.statuses.Tabs[favTabName].AllCount;
                    break;
                }
            }

            if (isCurFavTab)
            {
                this.ResetFocusedItem(favTabName, fidx);
            }
        }

        private void GetTimeline(WorkerType workerType, int fromPage = 1, int toPage = 0, string tabName = "")
        {
            if (!MyCommon.IsNetworkAvailable())
            {
                return;
            }

            if (this.lastTimeWork == null)
            {
                this.lastTimeWork = new Dictionary<WorkerType, DateTime>();
            }

            // 非同期実行引数設定
            if (!this.lastTimeWork.ContainsKey(workerType))
            {
                this.lastTimeWork.Add(workerType, new DateTime());
            }

            double period = DateTime.Now.Subtract(this.lastTimeWork[workerType]).TotalSeconds;
            if (period > 1 || period < -1)
            {
                this.lastTimeWork[workerType] = DateTime.Now;
                this.RunAsync(new GetWorkerArg() { Page = fromPage, EndPage = toPage, WorkerType = workerType, TabName = tabName });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isFavAdd">TrueでFavAdd,FalseでFavRemove</param>
        /// <param name="multiFavoriteChangeDialogEnable"></param>
        private void ChangeSelectedFavStatus(bool isFavAdd, bool multiFavoriteChangeDialogEnable = true)
        {            
            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage || this.curList.SelectedIndices.Count == 0 || !this.ExistCurrentPost)
            {
                return;
            }

            // 複数fav確認msg
            if (this.curList.SelectedIndices.Count > 250 && isFavAdd)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.FavoriteLimitCountText);
                this.doFavRetweetFlags = false;
                return;
            }

            if (multiFavoriteChangeDialogEnable && this.curList.SelectedIndices.Count > 1)
            {
                if (isFavAdd)
                {
                    string confirmMessage = this.doFavRetweetFlags ? 
                        Hoehoe.Properties.Resources.FavoriteRetweetQuestionText3 :
                        Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText1;
                    var result = MessageBox.Show(confirmMessage, Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        this.doFavRetweetFlags = false;
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show(Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText1, Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            var selcteds = this.curList.SelectedIndices.Cast<int>().Select(i => this.GetCurTabPost(i));
            var ids = isFavAdd ? selcteds.Where(p => !p.IsFav) : selcteds.Where(p => p.IsFav);
            if (ids.Count() == 0)
            {
                this.StatusLabel.Text = isFavAdd ? 
                    Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText4 : 
                    Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;
                return;
            }

            this.RunAsync(new GetWorkerArg()
            {
                Ids = ids.Select(p => p.StatusId).ToList(),
                SIds = new List<long>(),
                TabName = this.curTab.Text,
                WorkerType = isFavAdd ? WorkerType.FavAdd : WorkerType.FavRemove
            });
        }

        private PostClass GetCurTabPost(int index)
        {
            if (this.postCache != null && index >= this.itemCacheIndex && index < this.itemCacheIndex + this.postCache.Length)
            {
                return this.postCache[index - this.itemCacheIndex];
            }
            return this.statuses.Item(this.curTab.Text, index);
        }

        private void DeleteSelected()
        {
            if (this.curTab == null || this.curList == null)
            {
                return;
            }

            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            bool isDmTab = this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage;
            if (!isDmTab)
            {
                if (!this.curList.SelectedIndices.Cast<int>().Select(i => this.GetCurTabPost(i)).Any(p => IsPostMine(p)))
                {
                    return;
                }
            }

            var tmp = string.Format(Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText1, Environment.NewLine);
            var rslt = MessageBox.Show(tmp, Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            int prevFocused = 0;
            if (this.curList.FocusedItem != null)
            {
                prevFocused = this.curList.FocusedItem.Index;
            }
            else if (this.curList.TopItem != null)
            {
                prevFocused = this.curList.TopItem.Index;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                bool deleted = true;
                var statusIds = this.curList.SelectedIndices.Cast<int>().Select(i => this.statuses.GetId(this.curTab.Text, i));
                foreach (var statusId in statusIds)
                {
                    string ret = string.Empty;
                    var post = this.statuses.Item(statusId);
                    if (isDmTab)
                    {
                        ret = this.tw.RemoveDirectMessage(statusId, post);
                    }
                    else
                    {
                        if (this.IsPostMine(post))
                        {
                            ret = this.tw.RemoveStatus(statusId);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    
                    if (string.IsNullOrEmpty(ret))
                    {
                        this.statuses.RemovePost(statusId);
                    }
                    else
                    {
                        deleted = false;
                    }
                }

                this.StatusLabel.Text = deleted ?
                    Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText4 :
                    Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText3;

                // キャッシュ破棄
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.curTab.Equals(tb))
                    {
                        ResetFocusedItem(tb.Text, prevFocused);
                    }

                    if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            if (tb.ImageIndex == 0)
                            {
                                // タブアイコン
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
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ResetFocusedItem(string tabName, int prevFocused)
        {
            do
            {
                this.curList.SelectedIndices.Clear();
            }
            while (this.curList.SelectedIndices.Count > 0);

            if (this.statuses.Tabs[tabName].AllCount > 0)
            {
                if (this.statuses.Tabs[tabName].AllCount - 1 > prevFocused && prevFocused > -1)
                {
                    this.curList.SelectedIndices.Add(prevFocused);
                }
                else
                {
                    this.curList.SelectedIndices.Add(this.statuses.Tabs[tabName].AllCount - 1);
                }

                if (this.curList.SelectedIndices.Count > 0)
                {
                    this.curList.EnsureVisible(this.curList.SelectedIndices[0]);
                    this.curList.FocusedItem = this.curList.Items[this.curList.SelectedIndices[0]];
                }
            }
        }

        private bool IsPostMine(PostClass p)
        {
            return p.IsMe || p.RetweetedBy.ToLower() == this.tw.Username.ToLower();
        }

        private void DoRefresh()
        {
            RefreshTab();
        }

        private void DoRefreshMore()
        {
            RefreshTab(more: true);
        }

        private void RefreshTab(bool more = false)
        {
            int startPage = more ? -1 : 1;
            if (this.curTab == null)
            {
                this.GetTimeline(WorkerType.Timeline, startPage);
                return;
            }

            TabClass tb = this.statuses.Tabs[this.curTab.Text];
            switch (tb.TabType)
            {
                case TabUsageType.Mentions:
                    this.GetTimeline(WorkerType.Reply, startPage);
                    break;
                case TabUsageType.DirectMessage:
                    this.GetTimeline(WorkerType.DirectMessegeRcv, startPage);
                    break;
                case TabUsageType.Favorites:
                    this.GetTimeline(WorkerType.Favorites, startPage);
                    break;
                case TabUsageType.UserTimeline:
                    this.GetTimeline(WorkerType.UserTimeline, startPage, 0, this.curTab.Text);
                    break;
                case TabUsageType.PublicSearch:
                    if (string.IsNullOrEmpty(tb.SearchWords))
                    {
                        return;
                    }
                    this.GetTimeline(WorkerType.PublicSearch, startPage, 0, this.curTab.Text);
                    break;
                case TabUsageType.Lists:
                    if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                    {
                        return;
                    }
                    this.GetTimeline(WorkerType.List, startPage, 0, this.curTab.Text);
                    break;
                case TabUsageType.Profile: 
                    /* TODO: profile tab ? */
                    break;
                default:
                    this.GetTimeline(WorkerType.Timeline, startPage);
                    break;
            }
        }

        private void ShowUserTimeline()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.AddNewTabForUserTimeline(this.curPost.ScreenName);
        }

        private void SetListProperty()
        {
            // 削除などで見つからない場合は処理せず
            if (this.curList == null)
            {
                return;
            }

            if (!this.isColumnChanged)
            {
                return;
            }

            int[] dispOrder = new int[this.curList.Columns.Count];
            for (int i = 0; i < this.curList.Columns.Count; i++)
            {
                for (int j = 0; j < this.curList.Columns.Count; j++)
                {
                    if (this.curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            // 列幅、列並びを他のタブに設定
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (!tb.Equals(this.curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i <= lst.Columns.Count - 1; i++)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = this.curList.Columns[i].Width;
                        }
                    }
                }
            }

            this.isColumnChanged = false;
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            // 文字数カウント
            int len = 140 - this.StatusText.Text.Length;
            if (this.NotifyIcon1 == null || !this.NotifyIcon1.Visible)
            {
                return len;
            }

            if ((isAuto && !this.IsKeyDown(Keys.Control) && this.settingDialog.PostShiftEnter)
                || (isAuto && !this.IsKeyDown(Keys.Shift) && !this.settingDialog.PostShiftEnter)
                || (!isAuto && isAddFooter))
            {
                if (this.settingDialog.UseRecommendStatus)
                {
                    len -= this.settingDialog.RecommendStatusText.Length;
                }
                else if (this.settingDialog.Status.Length > 0)
                {
                    len -= this.settingDialog.Status.Length + 1;
                }
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                len -= this.HashMgr.UseHash.Length + 1;
            }

            foreach (Match m in Regex.Matches(this.StatusText.Text, Twitter.UrlRegexPattern, RegexOptions.IgnoreCase))
            {
                len += m.Result("${url}").Length - this.settingDialog.TwitterConfiguration.ShortUrlLength;
            }

            if (this.ImageSelectionPanel.Visible && this.ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(this.ImageService))
            {
                len -= this.settingDialog.TwitterConfiguration.CharactersReservedPerMedia;
            }

            return len;
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            try
            {
                // キャッシュ要求（要求範囲±30を作成）
                startIndex -= 30;
                if (startIndex < 0)
                {
                    startIndex = 0;
                }

                endIndex += 30;
                if (endIndex >= this.statuses.Tabs[this.curTab.Text].AllCount)
                {
                    endIndex = this.statuses.Tabs[this.curTab.Text].AllCount - 1;
                }

                this.postCache = this.statuses.Item(this.curTab.Text, startIndex, endIndex); // 配列で取得
                this.itemCacheIndex = startIndex;
                this.itemCache = new ListViewItem[this.postCache.Length];
                for (int i = 0; i <= this.postCache.Length - 1; i++)
                {
                    this.itemCache[i] = this.CreateItem(this.curTab, this.postCache[i], startIndex + i);
                }
            }
            catch (Exception)
            {
                // キャッシュ要求が実データとずれるため（イベントの遅延？）
                this.postCache = null;
                this.itemCache = null;
            }
        }

        private ListViewItem CreateItem(TabPage tabPage, PostClass post, int index)
        {
            StringBuilder mk = new StringBuilder();
            if (post.FavoritedCount > 0)
            {
                mk.Append("+" + post.FavoritedCount.ToString());
            }

            string postedByDetail = post.ScreenName;
            if (post.IsRetweeted)
            {
                postedByDetail += string.Format("{0}(RT:{1})", Environment.NewLine, post.RetweetedBy);
            }
            string[] sitem = { string.Empty, post.Nickname, post.IsDeleted ? "(DELETED)" : post.TextFromApi, post.CreatedAt.ToString(this.settingDialog.DateTimeFormat), postedByDetail, string.Empty, mk.ToString(), post.Source };
            ImageListViewItem itm = new ImageListViewItem(sitem, this.iconDict, post.ImageUrl);
            itm.StateImageIndex = post.StateIndex;

            bool read = post.IsRead;
            if (!this.statuses.Tabs[tabPage.Text].UnreadManage || !this.settingDialog.UnreadManage)
            {
                // 未読管理していなかったら既読として扱う
                read = true;
            }

            this.ChangeItemStyleRead(read, itm, post, null);
            if (tabPage.Equals(this.curTab))
            {
                this.ColorizeList(itm, index);
            }

            return itm;
        }

        private void DrawListViewItemIcon(DrawListViewItemEventArgs e)
        {
            ImageListViewItem item = (ImageListViewItem)e.Item;
            Rectangle stateRect = default(Rectangle);

            // e.Bounds.Leftが常に0を指すから自前で計算
            Rectangle itemRect = item.Bounds;
            itemRect.Width = e.Item.ListView.Columns[0].Width;

            foreach (ColumnHeader clm in e.Item.ListView.Columns)
            {
                if (clm.DisplayIndex < e.Item.ListView.Columns[0].DisplayIndex)
                {
                    itemRect.X += clm.Width;
                }
            }

            Rectangle iconRect = default(Rectangle);
            if (item.Image != null)
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(this.iconSz, this.iconSz)), itemRect);
                iconRect.Offset(0, Convert.ToInt32(Math.Max(0, (itemRect.Height - this.iconSz) / 2)));
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this.iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }
            else
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(1, 1)), itemRect);
                //// iconRect.Offset(0, CType(Math.Max(0, (itemRect.Height - this._iconSz) / 2), Integer))
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this.iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }

            if (item.Image != null && iconRect.Width > 0)
            {
                e.Graphics.FillRectangle(Brushes.White, iconRect);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                try
                {
                    e.Graphics.DrawImage(item.Image, iconRect);
                }
                catch (ArgumentException)
                {
                    item.RegetImage();
                }
            }

            if (item.StateImageIndex > -1)
            {
                if (stateRect.Width > 0)
                {
                    e.Graphics.DrawImage(this.PostStateImageList.Images[item.StateImageIndex], stateRect);
                }
            }
        }

        private void DoTabSearch(string word, bool isCaseSensitive, bool isUseRegex, SEARCHTYPE searchType)
        {
            if (this.curList.VirtualListSize == 0)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            int cidx = 0;
            if (this.curList.SelectedIndices.Count > 0)
            {
                cidx = this.curList.SelectedIndices[0];
            }

            int toIdx = this.curList.VirtualListSize - 1;
            int stp = 1;
            switch (searchType)
            {
                case SEARCHTYPE.DialogSearch:
                    // ダイアログからの検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0];
                    }
                    else
                    {
                        cidx = 0;
                    }

                    break;
                case SEARCHTYPE.NextSearch:
                    // 次を検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0] + 1;
                        if (cidx > toIdx)
                        {
                            cidx = toIdx;
                        }
                    }
                    else
                    {
                        cidx = 0;
                    }

                    break;
                case SEARCHTYPE.PrevSearch:
                    // 前を検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0] - 1;
                        if (cidx < 0)
                        {
                            cidx = 0;
                        }
                    }
                    else
                    {
                        cidx = toIdx;
                    }

                    toIdx = 0;
                    stp = -1;
                    break;
            }

            bool fnd = false;
            RegexOptions regOpt = RegexOptions.None;
            StringComparison fndOpt = StringComparison.Ordinal;
            if (!isCaseSensitive)
            {
                regOpt = RegexOptions.IgnoreCase;
                fndOpt = StringComparison.OrdinalIgnoreCase;
            }

            try
            {
            RETRY:
                if (isUseRegex)
                {
                    // 正規表現検索
                    try
                    {
                        Regex searchRegex = new Regex(word, regOpt);
                        for (int idx = cidx; idx <= toIdx; idx += stp)
                        {
                            PostClass post = null;
                            try
                            {
                                post = this.statuses.Item(this.curTab.Text, idx);
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            if (searchRegex.IsMatch(post.Nickname) || searchRegex.IsMatch(post.TextFromApi) || searchRegex.IsMatch(post.ScreenName))
                            {
                                this.SelectListItem(this.curList, idx);
                                this.curList.EnsureVisible(idx);
                                return;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText1, "Hoehoe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 通常検索
                    for (int idx = cidx; idx <= toIdx; idx += stp)
                    {
                        PostClass post = null;
                        try
                        {
                            post = this.statuses.Item(this.curTab.Text, idx);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (post.Nickname.IndexOf(word, fndOpt) > -1 || post.TextFromApi.IndexOf(word, fndOpt) > -1 || post.ScreenName.IndexOf(word, fndOpt) > -1)
                        {
                            this.SelectListItem(this.curList, idx);
                            this.curList.EnsureVisible(idx);
                            return;
                        }
                    }
                }

                if (!fnd)
                {
                    switch (searchType)
                    {
                        case SEARCHTYPE.DialogSearch:
                        case SEARCHTYPE.NextSearch:
                            toIdx = cidx;
                            cidx = 0;
                            break;
                        case SEARCHTYPE.PrevSearch:
                            toIdx = cidx;
                            cidx = this.curList.Items.Count - 1;
                            break;
                    }

                    fnd = true;
                    goto RETRY;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // TODO: to hoehoe2
        private void RunTweenUp()
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    WorkingDirectory = MyCommon.SettingPath,
                    FileName = Path.Combine(MyCommon.SettingPath, "TweenUp3.exe"),
                    Arguments = "\"" + Application.StartupPath + "\""
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to execute TweenUp3.exe.");
            }
        }

        private void CheckNewVersion(bool startup = false)
        {
            string retMsg = string.Empty;
            string strVer = string.Empty;
            string strDetail = string.Empty;
            bool forceUpdate = this.IsKeyDown(Keys.Shift);

            try
            {
                retMsg = this.tw.GetVersionInfo();
            }
            catch (Exception)
            {
                this.StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText10, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                }

                return;
            }

            if (retMsg.Length > 0)
            {
                strVer = retMsg.Substring(0, 4);
                if (retMsg.Length > 4)
                {
                    strDetail = retMsg.Substring(5).Trim();
                }

                if (!string.IsNullOrEmpty(MyCommon.FileVersion) && strVer.CompareTo(MyCommon.FileVersion.Replace(".", string.Empty)) > 0)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.CheckNewVersionText3, strVer);
                    using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                    {
                        if (dialogAsShieldicon.ShowDialog(tmp, strDetail, Hoehoe.Properties.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            retMsg = this.tw.GetTweenBinary(strVer);
                            if (retMsg.Length == 0)
                            {
                                this.RunTweenUp();
                                MyCommon.IsEnding = true;
                                this.Close();
                                return;
                            }

                            if (!startup)
                            {
                                MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                }
                else
                {
                    if (forceUpdate)
                    {
                        string tmp = string.Format(Hoehoe.Properties.Resources.CheckNewVersionText6, strVer);
                        using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                        {
                            if (dialogAsShieldicon.ShowDialog(tmp, strDetail, Hoehoe.Properties.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                retMsg = this.tw.GetTweenBinary(strVer);
                                if (retMsg.Length == 0)
                                {
                                    this.RunTweenUp();
                                    MyCommon.IsEnding = true;
                                    this.Close();
                                    return;
                                }
                                if (!startup)
                                {
                                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                    }
                    else if (!startup)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText7 + MyCommon.FileVersion.Replace(".", string.Empty) + Hoehoe.Properties.Resources.CheckNewVersionText8 + strVer, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                this.StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText10, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Colorize()
        {
            this.colorize = false;
            this.DispSelectedPost();

            // 件数関連の場合、タイトル即時書き換え
            if (this.settingDialog.DispLatestPost != DispTitleEnum.None && this.settingDialog.DispLatestPost != DispTitleEnum.Post && this.settingDialog.DispLatestPost != DispTitleEnum.Ver && this.settingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                this.SetMainWindowTitle();
            }

            if (!this.StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }

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

        private void DispSelectedPost(bool forceupdate = false)
        {
            if (this.curList.SelectedIndices.Count == 0 || this.curPost == null)
            {
                return;
            }

            if (!forceupdate && this.curPost.Equals(this.displayPost))
            {
                return;
            }

            this.displayPost = this.curPost;
            if (this.displayItem != null)
            {
                this.displayItem.ImageDownloaded -= this.DisplayItemImage_Downloaded;
                this.displayItem = null;
            }

            this.displayItem = (ImageListViewItem)this.curList.Items[this.curList.SelectedIndices[0]];
            this.displayItem.ImageDownloaded += this.DisplayItemImage_Downloaded;

            string detailText = this.CreateDetailHtml(this.curPost.IsDeleted ? "(DELETED)" : this.curPost.Text);
            if (this.curPost.IsDm)
            {
                this.SourceLinkLabel.Tag = null;
                this.SourceLinkLabel.Text = string.Empty;
            }
            else
            {
                Match mc = Regex.Match(this.curPost.SourceHtml, "<a href=\"(?<sourceurl>.+?)\"");
                if (mc.Success)
                {
                    string src = mc.Groups["sourceurl"].Value;
                    this.SourceLinkLabel.Tag = mc.Groups["sourceurl"].Value;
                    mc = Regex.Match(src, "^https?://");
                    if (!mc.Success)
                    {
                        src = src.Insert(0, "http://twitter.com");
                    }

                    this.SourceLinkLabel.Tag = src;
                }
                else
                {
                    this.SourceLinkLabel.Tag = null;
                }

                if (string.IsNullOrEmpty(this.curPost.Source))
                {
                    this.SourceLinkLabel.Text = string.Empty;
                }
                else
                {
                    this.SourceLinkLabel.Text = this.curPost.Source;
                }
            }

            this.SourceLinkLabel.TabStop = false;

            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage && !this.curPost.IsOwl)
            {
                this.NameLabel.Text = "DM TO -> ";
            }
            else if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                this.NameLabel.Text = "DM FROM <- ";
            }
            else
            {
                this.NameLabel.Text = string.Empty;
            }

            this.NameLabel.Text += this.curPost.ScreenName + "/" + this.curPost.Nickname;
            this.NameLabel.Tag = this.curPost.ScreenName;
            if (!string.IsNullOrEmpty(this.curPost.RetweetedBy))
            {
                this.NameLabel.Text += string.Format(" (RT:{0})", this.curPost.RetweetedBy);
            }

            if (!string.IsNullOrEmpty(this.curPost.ImageUrl))
            {
                this.UserPicture.ReplaceImage(this.iconDict[this.curPost.ImageUrl]);
            }
            else
            {
                this.UserPicture.ClearImage();
            }

            this.NameLabel.ForeColor = SystemColors.ControlText;
            this.DateTimeLabel.Text = this.curPost.CreatedAt.ToString();
            if (this.curPost.IsOwl && (this.settingDialog.OneWayLove || this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage))
            {
                this.NameLabel.ForeColor = this.clrOWL;
            }

            if (this.curPost.IsRetweeted)
            {
                this.NameLabel.ForeColor = this.clrRetweet;
            }

            if (this.curPost.IsFav)
            {
                this.NameLabel.ForeColor = this.clrFav;
            }

            if (this.DumpPostClassToolStripMenuItem.Checked)
            {
                this.PostBrowser.Visible = false;
                this.PostBrowser.DocumentText = this.detailHtmlFormatHeader + this.curPost.GetDump() + this.detailHtmlFormatFooter;
                this.PostBrowser.Visible = true;
            }
            else
            {
                try
                {
                    if (this.PostBrowser.DocumentText != detailText)
                    {
                        this.PostBrowser.Visible = false;
                        this.PostBrowser.DocumentText = detailText;
                        List<string> lnks = new List<string>();
                        foreach (Match lnk in Regex.Matches(detailText, "<a target=\"_self\" href=\"(?<url>http[^\"]+)\"", RegexOptions.IgnoreCase))
                        {
                            lnks.Add(lnk.Result("${url}"));
                        }

                        this.thumbnail.GenThumbnail(this.curPost.StatusId, lnks, this.curPost.PostGeo, this.curPost.Media);
                    }
                }
                catch (COMException comex)
                {
                    // 原因不明
                    System.Diagnostics.Debug.Write(comex);
                }
                catch (UriFormatException)
                {
                    this.PostBrowser.DocumentText = detailText;
                }
                finally
                {
                    this.PostBrowser.Visible = true;
                }
            }
        }

        private ModifierState GetModifierState(bool isCtrl, bool isShift, bool isAlt)
        {
            ModifierState state = ModifierState.None;
            if (isCtrl)
            {
                state = state | ModifierState.Ctrl;
            }

            if (isShift)
            {
                state = state | ModifierState.Shift;
            }

            if (isAlt)
            {
                state = state | ModifierState.Alt;
            }

            return state;
        }

        private bool CommonKeyDown(Keys keyCode, FocusedControl focusedControl, ModifierState modifierState)
        {
            bool functionReturnValue = false;
            if (focusedControl == FocusedControl.ListTab)
            {
                // リストのカーソル移動関係（上下キー、PageUp/Downに該当）
                if (modifierState == (ModifierState.Ctrl | ModifierState.Shift) || modifierState == ModifierState.Ctrl || modifierState == ModifierState.None || modifierState == ModifierState.Shift)
                {
                    if (keyCode == Keys.J)
                    {
                        SendKeys.Send("{DOWN}");
                        return true;
                    }
                    else if (keyCode == Keys.K)
                    {
                        SendKeys.Send("{UP}");
                        return true;
                    }
                }

                if (modifierState == ModifierState.Shift || modifierState == ModifierState.None)
                {
                    if (keyCode == Keys.F)
                    {
                        SendKeys.Send("{PGDN}");
                        return true;
                    }
                    else if (keyCode == Keys.B)
                    {
                        SendKeys.Send("{PGUP}");
                        return true;
                    }
                }
            }

            // 修飾キーなし
            switch (modifierState)
            {
                case ModifierState.None:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F1:
                            this.OpenUriAsync(ApplicationHelpWebPageUrl);
                            return true;
                        case Keys.F3:
                            this.TrySearchWordInTabToBottom();
                            return true;
                        case Keys.F5:
                            this.DoRefresh();
                            return true;
                        case Keys.F6:
                            this.GetTimeline(WorkerType.Reply);
                            return true;
                        case Keys.F7:
                            this.GetTimeline(WorkerType.DirectMessegeRcv);
                            return true;
                    }

                    if (focusedControl != FocusedControl.StatusText)
                    {
                        // フォーカスStatusText以外
                        switch (keyCode)
                        {
                            case Keys.Space:
                            case Keys.ProcessKey:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    this.anchorFlag = false;
                                }

                                this.TrySearchAndFocusUnreadTweet();
                                return true;
                            case Keys.G:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    this.anchorFlag = false;
                                }

                                this.AddRelatedStatusesTab();
                                return true;
                        }
                    }

                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // フォーカスList
                        switch (keyCode)
                        {
                            case Keys.N:
                            case Keys.Right:
                                this.GoRelPost(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                this.GoRelPost(false);
                                return true;
                            case Keys.OemPeriod:
                                this.GoAnchor();
                                return true;
                            case Keys.I:
                                if (this.StatusText.Enabled)
                                {
                                    this.StatusText.Focus();
                                }

                                return true;
                            case Keys.Enter:
                                // case Keys.Return:
                                this.MakeReplyOrDirectStatus();
                                return true;
                            case Keys.R:
                                this.DoRefresh();
                                return true;
                        }

                        // 以下、アンカー初期化
                        this.anchorFlag = false;
                        switch (keyCode)
                        {
                            case Keys.L:
                                this.GoPost(true);
                                return true;
                            case Keys.H:
                                this.GoPost(false);
                                return true;
                            case Keys.Z:
                            case Keys.Oemcomma:
                                this.MoveTop();
                                return true;
                            case Keys.S:
                                this.GoNextTab(true);
                                return true;
                            case Keys.A:
                                this.GoNextTab(false);
                                return true;
                            case Keys.Oem4:
                                // ] in_reply_to参照元へ戻る
                                this.GoInReplyToPostTree();
                                return true;
                            case Keys.Oem6:
                                // [ in_reply_toへジャンプ
                                this.GoBackInReplyToPostTree();
                                return true;
                            case Keys.Escape:
                                if (this.ListTab.SelectedTab != null)
                                {
                                    TabUsageType tabtype = this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == TabUsageType.Related || tabtype == TabUsageType.UserTimeline || tabtype == TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = this.ListTab.SelectedTab;
                                        this.RemoveSpecifiedTab(relTp.Text, false);
                                        this.SaveConfigsTabs();
                                        return true;
                                    }
                                }

                                break;
                        }
                    }

                    break;
                case ModifierState.Ctrl:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.MakeReplyOrDirectStatus(false, true);
                            return true;
                        case Keys.D:
                            this.DeleteSelected();
                            return true;
                        case Keys.M:
                            this.MakeReplyOrDirectStatus(false, false);
                            return true;
                        case Keys.S:
                            this.ChangeSelectedFavStatus(true);
                            return true;
                        case Keys.I:
                            this.DoRepliedStatusOpen();
                            return true;
                        case Keys.Q:
                            this.DoQuote();
                            return true;
                        case Keys.B:
                            this.ChangeSelectetdTweetReadStateToRead();
                            return true;
                        case Keys.T:
                            this.ShowHashManageBox();
                            return true;
                        case Keys.L:
                            this.ConvertUrlByAutoSelectedService();
                            return true;
                        case Keys.Y:
                            if (focusedControl != FocusedControl.PostBrowser)
                            {
                                this.ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
                                return true;
                            }

                            break;
                        case Keys.F:
                            this.TrySearchWordInTab();
                            return true;
                        case Keys.U:
                            this.ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            this.TryOpenCurListSelectedUserHome();
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            this.TryOpenCurListSelectedUserFavorites();
                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            this.TryOpenSelectedTweetWebPage();
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            this.TryOpenUrlInCurrentTweet();
                            return true;
                    }

                    // フォーカスList
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.Home:
                            case Keys.End:
                                this.colorize = true; // スルーする
                                return false;
                            case Keys.N:
                                this.GoNextTab(true);
                                return true;
                            case Keys.P:
                                this.GoNextTab(false);
                                return true;
                            case Keys.C:
                                this.CopyStot();
                                return true;
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                // タブダイレクト選択(Ctrl+1～8,Ctrl+9)
                                int tabNo = keyCode - Keys.D1;
                                if (this.ListTab.TabPages.Count < tabNo)
                                {
                                    return functionReturnValue;
                                }

                                this.ListTab.SelectedIndex = tabNo;
                                this.ListTabSelect(this.ListTab.TabPages[tabNo]);
                                return true;
                            case Keys.D9:
                                this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                                this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                                return true;
                        }
                    }
                    else if (focusedControl == FocusedControl.StatusText)
                    {
                        // フォーカスStatusText
                        switch (keyCode)
                        {
                            case Keys.A:
                                this.StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!string.IsNullOrEmpty(this.StatusText.Text.Trim()))
                                {
                                    this.postHistory[this.postHistoryIndex] = new PostingStatus(this.StatusText.Text, this.replyToId, this.replyToName);
                                }

                                if (keyCode == Keys.Up)
                                {
                                    this.postHistoryIndex -= 1;
                                    if (this.postHistoryIndex < 0)
                                    {
                                        this.postHistoryIndex = 0;
                                    }
                                }
                                else
                                {
                                    this.postHistoryIndex += 1;
                                    if (this.postHistoryIndex > this.postHistory.Count - 1)
                                    {
                                        this.postHistoryIndex = this.postHistory.Count - 1;
                                    }
                                }

                                this.StatusText.Text = this.postHistory[this.postHistoryIndex].Status;
                                this.replyToId = this.postHistory[this.postHistoryIndex].InReplyToId;
                                this.replyToName = this.postHistory[this.postHistoryIndex].InReplyToName;
                                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                                return true;
                            case Keys.PageUp:
                            case Keys.P:
                                if (this.ListTab.SelectedIndex == 0)
                                {
                                    this.ListTab.SelectedIndex = this.ListTab.TabCount - 1;
                                }
                                else
                                {
                                    this.ListTab.SelectedIndex -= 1;
                                }

                                this.StatusText.Focus();
                                return true;
                            case Keys.PageDown:
                            case Keys.N:
                                if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                                {
                                    this.ListTab.SelectedIndex = 0;
                                }
                                else
                                {
                                    this.ListTab.SelectedIndex += 1;
                                }

                                this.StatusText.Focus();
                                return true;
                        }
                    }
                    else
                    {
                        // フォーカスPostBrowserもしくは関係なし
                        switch (keyCode)
                        {
                            case Keys.A:
                                this.PostBrowser.Document.ExecCommand("SelectAll", false, null);
                                return true;
                            case Keys.C:
                            case Keys.Insert:
                                string selText = WebBrowser_GetSelectionText(ref this.PostBrowser);
                                if (!string.IsNullOrEmpty(selText))
                                {
                                    CopyToClipboard(selText);
                                }

                                return true;
                            case Keys.Y:
                                MultiLineMenuItem.Checked = !MultiLineMenuItem.Checked;
                                ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
                                return true;
                        }
                    }

                    break;
                case ModifierState.Shift:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F3:
                            this.TrySearchWordInTabToTop();
                            return true;
                        case Keys.F5:
                            this.DoRefreshMore();
                            return true;
                        case Keys.F6:
                            this.GetTimeline(WorkerType.Reply, -1);
                            return true;
                        case Keys.F7:
                            this.GetTimeline(WorkerType.DirectMessegeRcv);
                            return true;
                    }

                    // フォーカスStatusText以外
                    if (focusedControl != FocusedControl.StatusText)
                    {
                        if (keyCode == Keys.R)
                        {
                            this.DoRefreshMore();
                            return true;
                        }
                    }

                    // フォーカスリスト
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.H:
                                this.GoTopEnd(true);
                                return true;
                            case Keys.L:
                                this.GoTopEnd(false);
                                return true;
                            case Keys.M:
                                this.GoMiddle();
                                return true;
                            case Keys.G:
                                this.GoLast();
                                return true;
                            case Keys.Z:
                                this.MoveMiddle();
                                return true;
                            case Keys.Oem4:
                                this.GoBackInReplyToPostTree(true, false);
                                return true;
                            case Keys.Oem6:
                                this.GoBackInReplyToPostTree(true, true);
                                return true;
                            case Keys.N:
                            case Keys.Right:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                this.GoFav(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                this.GoFav(false);
                                return true;
                            case Keys.Space:
                                this.GoBackSelectPostChain();
                                return true;
                        }
                    }

                    break;
                case ModifierState.Alt:
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.DoReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (this.curPost != null)
                            {
                                this.ShowUserStatus(this.curPost.ScreenName, false);
                                return true;
                            }

                            break;
                        case Keys.Up:
                            this.ScrollDownPostBrowser(false);
                            return true;
                        case Keys.Down:
                            this.ScrollDownPostBrowser(true);
                            return true;
                        case Keys.PageUp:
                            this.PageDownPostBrowser(false);
                            return true;
                        case Keys.PageDown:
                            this.PageDownPostBrowser(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // 別タブの同じ書き込みへ(ALT+←/→)
                        if (keyCode == Keys.Right)
                        {
                            this.GoSamePostToAnotherTab(false);
                            return true;
                        }

                        if (keyCode == Keys.Left)
                        {
                            this.GoSamePostToAnotherTab(true);
                            return true;
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Shift:
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.MakeReplyOrDirectStatus(false, true, true);
                            return true;
                        case Keys.C:
                            this.CopyIdUri();
                            return true;
                        case Keys.F:
                            if (this.ListTab.SelectedTab != null)
                            {
                                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                                {
                                    this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                                    return true;
                                }
                            }

                            break;
                        case Keys.S:
                            this.ChangeSelectedFavStatus(false);
                            return true;
                        case Keys.B:
                            this.ChangeSelectedTweetReadSateToUnread();
                            return true;
                        case Keys.T:
                            this.ChangeUseHashTagSetting();
                            return true;
                        case Keys.P:
                            this.ToggleImageSelectorView();
                            return true;
                        case Keys.H:
                            this.TryOpenSelectedRtUserHome();
                            return true;
                        case Keys.O:
                            this.OpenFavorarePageOfSelectedTweetUser();
                            return true;
                    }

                    if (focusedControl == FocusedControl.StatusText)
                    {
                        switch (keyCode)
                        {
                            case Keys.Up:
                                {
                                    int idx = 0;
                                    if (this.curList != null && this.curList.Items.Count != 0 && this.curList.SelectedIndices.Count > 0 && this.curList.SelectedIndices[0] > 0)
                                    {
                                        idx = this.curList.SelectedIndices[0] - 1;
                                        this.SelectListItem(this.curList, idx);
                                        this.curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Down:
                                {
                                    int idx = 0;
                                    if (this.curList != null && this.curList.Items.Count != 0 && this.curList.SelectedIndices.Count > 0 && this.curList.SelectedIndices[0] < this.curList.Items.Count - 1)
                                    {
                                        idx = this.curList.SelectedIndices[0] + 1;
                                        this.SelectListItem(this.curList, idx);
                                        this.curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Space:
                                if (this.StatusText.SelectionStart > 0)
                                {
                                    int endidx = this.StatusText.SelectionStart - 1;
                                    string startstr = string.Empty;
                                    bool pressed = false;
                                    for (int i = this.StatusText.SelectionStart - 1; i >= 0; i--)
                                    {
                                        char c = this.StatusText.Text[i];
                                        if (char.IsLetterOrDigit(c) || c == '_')
                                        {
                                            continue;
                                        }

                                        if (c == '@')
                                        {
                                            pressed = true;
                                            startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                            int cnt = this.AtIdSupl.ItemCount;
                                            this.ShowSuplDialog(this.StatusText, this.AtIdSupl, startstr.Length + 1, startstr);
                                            if (this.AtIdSupl.ItemCount != cnt)
                                            {
                                                this.modifySettingAtId = true;
                                            }
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                            this.ShowSuplDialog(this.StatusText, this.HashSupl, startstr.Length + 1, startstr);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    return pressed;
                                }

                                break;
                        }
                    }
                    else if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                {
                                    // ソートダイレクト選択(Ctrl+Shift+1～8,Ctrl+Shift+9)
                                    int colNo = keyCode - Keys.D1;
                                    DetailsListView lst = (DetailsListView)this.ListTab.SelectedTab.Tag;
                                    if (lst.Columns.Count < colNo)
                                    {
                                        return functionReturnValue;
                                    }

                                    var col = lst.Columns.Cast<ColumnHeader>().Where(x => x.DisplayIndex == colNo).FirstOrDefault();
                                    if (col == null)
                                    {
                                        return functionReturnValue;
                                    }

                                    MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                    return true;
                                }

                            case Keys.D9:
                                {
                                    DetailsListView lst = (DetailsListView)this.ListTab.SelectedTab.Tag;
                                    var col = lst.Columns.Cast<ColumnHeader>().OrderByDescending(x => x.DisplayIndex).First();
                                    MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                    return true;
                                }
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Alt:
                    if (keyCode == Keys.S)
                    {
                        this.FavoritesRetweetOriginal();
                        return true;
                    }
                    else if (keyCode == Keys.R)
                    {
                        this.FavoritesRetweetUnofficial();
                        return true;
                    }
                    else if (keyCode == Keys.H)
                    {
                        OpenUserAppointUrl();
                        return true;
                    }

                    break;
                case ModifierState.Alt | ModifierState.Shift:
                    if (focusedControl == FocusedControl.PostBrowser)
                    {
                        if (keyCode == Keys.R)
                        {
                            this.DoReTweetUnofficial();
                        }
                        else if (keyCode == Keys.C)
                        {
                            this.CopyUserId();
                        }

                        return true;
                    }

                    switch (keyCode)
                    {
                        case Keys.T:
                            if (!this.ExistCurrentPost)
                            {
                                return functionReturnValue;
                            }

                            this.DoTranslation(this.curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            this.DoReTweetUnofficial();
                            return true;
                        case Keys.C:
                            this.CopyUserId();
                            return true;
                        case Keys.Up:
                            this.thumbnail.ScrollThumbnail(false);
                            return true;
                        case Keys.Down:
                            this.thumbnail.ScrollThumbnail(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab && keyCode == Keys.Enter)
                    {
                        if (!this.SplitContainer3.Panel2Collapsed)
                        {
                            this.thumbnail.OpenPicture();
                        }

                        return true;
                    }

                    break;
            }

            return functionReturnValue;
        }

        private void ScrollDownPostBrowser(bool forward)
        {
            HtmlDocument doc = this.PostBrowser.Document;
            if (doc == null)
            {
                return;
            }

            if (doc.Body == null)
            {
                return;
            }

            if (forward)
            {
                doc.Body.ScrollTop += this.settingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= this.settingDialog.FontDetail.Height;
            }
        }

        private void PageDownPostBrowser(bool forward)
        {
            HtmlDocument doc = this.PostBrowser.Document;
            if (doc == null)
            {
                return;
            }

            if (doc.Body == null)
            {
                return;
            }

            if (forward)
            {
                doc.Body.ScrollTop += this.PostBrowser.ClientRectangle.Height - this.settingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= this.PostBrowser.ClientRectangle.Height - this.settingDialog.FontDetail.Height;
            }
        }

        private void GoNextTab(bool forward)
        {
            int idx = this.ListTab.SelectedIndex;
            if (forward)
            {
                idx += 1;
                if (idx > this.ListTab.TabPages.Count - 1)
                {
                    idx = 0;
                }
            }
            else
            {
                idx -= 1;
                if (idx < 0)
                {
                    idx = this.ListTab.TabPages.Count - 1;
                }
            }

            this.ListTab.SelectedIndex = idx;
            this.ListTabSelect(this.ListTab.TabPages[idx]);
        }

        private void CopyStot()
        {
            string clstr = string.Empty;
            StringBuilder sb = new StringBuilder();
            bool isProtected = false;
            bool isDm = false;
            if (this.curTab != null && this.statuses.GetTabByName(this.curTab.Text) != null)
            {
                isDm = this.statuses.GetTabByName(this.curTab.Text).TabType == TabUsageType.DirectMessage;
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                if (post.IsProtect)
                {
                    isProtected = true;
                    continue;
                }

                if (post.IsDeleted)
                {
                    continue;
                }

                if (!isDm)
                {
                    sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.OriginalStatusId, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}:{1} [{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine);
                }
            }

            if (isProtected)
            {
                // MessageBox.Show(My.Resources.CopyStotText1)
                MessageForm w = new MessageForm();
                w.ShowDialog(Hoehoe.Properties.Resources.CopyStotText1);
            }

            if (sb.Length > 0)
            {
                CopyToClipboard(sb.ToString());
            }
        }

        private void CopyIdUri()
        {
            string clstr = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (this.curTab == null)
            {
                return;
            }

            if (this.statuses.GetTabByName(this.curTab.Text) == null)
            {
                return;
            }

            if (this.statuses.GetTabByName(this.curTab.Text).TabType == TabUsageType.DirectMessage)
            {
                return;
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.OriginalStatusId, Environment.NewLine);
            }

            if (sb.Length > 0)
            {
                CopyToClipboard(sb.ToString());
            }
        }

        private void GoFav(bool forward)
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                if (this.curList.SelectedIndices.Count == 0)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = this.curList.SelectedIndices[0] + 1;
                    if (fIdx > this.curList.VirtualListSize - 1)
                    {
                        return;
                    }
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                if (this.curList.SelectedIndices.Count == 0)
                {
                    fIdx = this.curList.VirtualListSize - 1;
                }
                else
                {
                    fIdx = this.curList.SelectedIndices[0] - 1;
                    if (fIdx < 0)
                    {
                        return;
                    }
                }

                toIdx = 0;
                stp = -1;
            }

            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                if (this.statuses.Item(this.curTab.Text, idx).IsFav)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            long targetId = 0;

            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                // Directタブは対象外（見つかるはずがない）
                return;
            }

            if (this.curList.SelectedIndices.Count == 0)
            {
                // 未選択も処理しない
                return;
            }

            targetId = this.GetCurTabPost(this.curList.SelectedIndices[0]).StatusId;
            if (left)
            {
                // 左のタブへ
                if (this.ListTab.SelectedIndex == 0)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex - 1;
                }

                toIdx = 0;
                stp = -1;
            }
            else
            {
                // 右のタブへ
                if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex + 1;
                }

                toIdx = this.ListTab.TabCount - 1;
                stp = 1;
            }

            bool found = false;
            for (int tabidx = fIdx; tabidx <= toIdx; tabidx += stp)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage)
                {
                    // Directタブは対象外
                    continue;
                }

                for (int idx = 0; idx <= ((DetailsListView)this.ListTab.TabPages[tabidx].Tag).VirtualListSize - 1; idx++)
                {
                    if (this.statuses.Item(this.ListTab.TabPages[tabidx].Text, idx).StatusId == targetId)
                    {
                        this.ListTab.SelectedIndex = tabidx;
                        this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                        this.SelectListItem(this.curList, idx);
                        this.curList.EnsureVisible(idx);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }

        private void GoPost(bool forward)
        {
            if (this.curList.SelectedIndices.Count == 0 || this.curPost == null)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                fIdx = this.curList.SelectedIndices[0] + 1;
                if (fIdx > this.curList.VirtualListSize - 1)
                {
                    return;
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this.curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }

                toIdx = 0;
                stp = -1;
            }

            string name = this.curPost.IsRetweeted ? this.curPost.RetweetedBy : this.curPost.ScreenName;
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                var statusesItem = this.statuses.Item(this.curTab.Text, idx);
                var statusItemName = statusesItem.IsRetweeted ? statusesItem.RetweetedBy : statusesItem.ScreenName;
                if (statusItemName == name)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            if (forward)
            {
                fIdx = this.curList.SelectedIndices[0] + 1;
                if (fIdx > this.curList.VirtualListSize - 1)
                {
                    return;
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this.curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }

                toIdx = 0;
                stp = -1;
            }

            if (!this.anchorFlag)
            {
                if (this.curPost == null)
                {
                    return;
                }

                this.anchorPost = this.curPost;
                this.anchorFlag = true;
            }
            else
            {
                if (this.anchorPost == null)
                {
                    return;
                }
            }

            // TODO: VB's for-next to C#'s for
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                if (post.ScreenName == this.anchorPost.ScreenName
                    || post.RetweetedBy == this.anchorPost.ScreenName
                    || post.ScreenName == this.anchorPost.RetweetedBy
                    || (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == this.anchorPost.RetweetedBy)
                    || this.anchorPost.ReplyToList.Contains(post.ScreenName.ToLower())
                    || this.anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower())
                    || post.ReplyToList.Contains(this.anchorPost.ScreenName.ToLower())
                    || post.ReplyToList.Contains(this.anchorPost.RetweetedBy.ToLower()))
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            if (this.anchorPost == null)
            {
                return;
            }

            int idx = this.statuses.Tabs[this.curTab.Text].IndexOf(this.anchorPost.StatusId);
            if (idx == -1)
            {
                return;
            }

            this.SelectListItem(this.curList, idx);
            this.curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool top)
        {
            ListViewItem item = null;
            int idx = 0;

            if (top)
            {
                item = this.curList.GetItemAt(0, 25);
                idx = item != null ? item.Index : 0;
            }
            else
            {
                item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
                idx = item != null ? item.Index : this.curList.VirtualListSize - 1;
            }

            this.SelectListItem(this.curList, idx);
        }

        private void GoMiddle()
        {
            ListViewItem item = this.curList.GetItemAt(0, 0);
            int idx1 = item == null ? 0 : item.Index;

            item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
            int idx2 = item == null ? this.curList.VirtualListSize - 1 : item.Index;

            this.SelectListItem(this.curList, (idx1 + idx2) / 2);
        }

        private void GoLast()
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                this.SelectListItem(this.curList, this.curList.VirtualListSize - 1);
                this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            }
            else
            {
                this.SelectListItem(this.curList, 0);
                this.curList.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = this.curList.SelectedIndices[0];
            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            }
            else
            {
                this.curList.EnsureVisible(0);
            }

            this.curList.EnsureVisible(idx);
        }

        private void GoInReplyToPostTree()
        {
            if (this.curPost == null)
            {
                return;
            }

            TabClass curTabClass = this.statuses.Tabs[this.curTab.Text];

            if (curTabClass.TabType == TabUsageType.PublicSearch && this.curPost.InReplyToStatusId == 0 && this.curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = this.tw.GetStatusApi(false, this.curPost.StatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    this.curPost.InReplyToStatusId = post.InReplyToStatusId;
                    this.curPost.InReplyToUser = post.InReplyToUser;
                    this.curPost.IsReply = post.IsReply;
                    this.itemCache = null;
                    this.curList.RedrawItems(this.curItemIndex, this.curItemIndex, false);
                }
                else
                {
                    this.StatusLabel.Text = r;
                }
            }

            if (!(this.ExistCurrentPost && this.curPost.InReplyToUser != null && this.curPost.InReplyToStatusId > 0))
            {
                return;
            }

            if (this.replyChains == null || (this.replyChains.Count > 0 && this.replyChains.Peek().InReplyToId != this.curPost.StatusId))
            {
                this.replyChains = new Stack<ReplyChain>();
            }

            this.replyChains.Push(new ReplyChain(this.curPost.StatusId, this.curPost.InReplyToStatusId, this.curTab));

            int inReplyToIndex = 0;
            string inReplyToTabName = null;
            long inReplyToId = this.curPost.InReplyToStatusId;
            string inReplyToUser = this.curPost.InReplyToUser;
            Dictionary<long, PostClass> curTabPosts = null;

            if (this.statuses.Tabs[this.curTab.Text].IsInnerStorageTabType)
            {
                curTabPosts = curTabClass.Posts;
            }
            else
            {
                curTabPosts = this.statuses.Posts;
            }

            var inReplyToPosts = from tab in this.statuses.Tabs.Values
                                 orderby !object.ReferenceEquals(tab, curTabClass)
                                 from post in ((Dictionary<long, PostClass>)(tab.IsInnerStorageTabType ? tab.Posts : this.statuses.Posts)).Values
                                 where post.StatusId == inReplyToId
                                 let index = tab.IndexOf(post.StatusId)
                                 where index != -1
                                 select new { Tab = tab, Index = index };

            try
            {
                var inReplyPost = inReplyToPosts.First();
                inReplyToTabName = inReplyPost.Tab.TabName;
                inReplyToIndex = inReplyPost.Index;
            }
            catch (InvalidOperationException)
            {
                PostClass post = null;
                string r = this.tw.GetStatusApi(false, this.curPost.InReplyToStatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    post.IsRead = true;
                    this.statuses.AddPost(post);
                    this.statuses.DistributePosts();
                    this.RefreshTimeline(false);
                    try
                    {
                        var inReplyPost = inReplyToPosts.First();
                        inReplyToTabName = inReplyPost.Tab.TabName;
                        inReplyToIndex = inReplyPost.Index;
                    }
                    catch (InvalidOperationException)
                    {
                        this.OpenUriAsync(string.Format("http://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                        return;
                    }
                }
                else
                {
                    this.StatusLabel.Text = r;
                    this.OpenUriAsync(string.Format("http://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                    return;
                }
            }

            var tabPage = this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == inReplyToTabName);
            var listView = (DetailsListView)tabPage.Tag;
            if (!object.ReferenceEquals(this.curTab, tabPage))
            {
                this.ListTab.SelectTab(tabPage);
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (this.curPost == null)
            {
                return;
            }

            TabClass curTabClass = this.statuses.Tabs[this.curTab.Text];
            Dictionary<long, PostClass> curTabPosts = (Dictionary<long, PostClass>)(curTabClass.IsInnerStorageTabType ? curTabClass.Posts : this.statuses.Posts);

            if (parallel)
            {
                if (this.curPost.InReplyToStatusId != 0)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this.statuses.Posts)
                                where p.Value.StatusId != this.curPost.StatusId && p.Value.InReplyToStatusId == this.curPost.InReplyToStatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby (isForward ? indexOf : indexOf * -1)
                                orderby !object.ReferenceEquals(t.Value, curTabClass)
                                select new { Tab = t.Value, Post = p.Value, Index = indexOf };
                    try
                    {
                        var postList = posts.ToList();
                        for (int i = postList.Count - 1; i >= 0; i--)
                        {
                            int index = i;
                            if (postList.FindIndex(pst => pst.Post.StatusId == postList[index].Post.StatusId) != index)
                            {
                                postList.RemoveAt(index);
                            }
                        }

                        var post = postList.FirstOrDefault(pst => object.ReferenceEquals(pst.Tab, curTabClass) && (isForward ? pst.Index > this.curItemIndex : pst.Index < this.curItemIndex));
                        if (post == null)
                        {
                            post = postList.FirstOrDefault(pst => !object.ReferenceEquals(pst.Tab, curTabClass));
                        }

                        if (post == null)
                        {
                            post = postList.First();
                        }

                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                        var listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (this.replyChains == null || this.replyChains.Count < 1)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this.statuses.Posts)
                                where p.Value.InReplyToStatusId == this.curPost.StatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby indexOf
                                orderby !object.ReferenceEquals(t.Value, curTabClass)
                                select new { Tab = t.Value, Index = indexOf };
                    try
                    {
                        var post = posts.First();
                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                        var listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                else
                {
                    ReplyChain chainHead = this.replyChains.Pop();
                    if (chainHead.InReplyToId == this.curPost.StatusId)
                    {
                        int idx = this.statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                        if (idx == -1)
                        {
                            this.replyChains = null;
                        }
                        else
                        {
                            try
                            {
                                this.ListTab.SelectTab(chainHead.OriginalTab);
                            }
                            catch (Exception)
                            {
                                this.replyChains = null;
                            }

                            this.SelectListItem(this.curList, idx);
                            this.curList.EnsureVisible(idx);
                        }
                    }
                    else
                    {
                        this.replyChains = null;
                        this.GoBackInReplyToPostTree(parallel);
                    }
                }
            }
        }

        private void GoBackSelectPostChain()
        {
            try
            {
                this.selectPostChains.Pop();
                var tabPostPair = this.selectPostChains.Pop();
                if (!this.ListTab.TabPages.Contains(tabPostPair.Item1))
                {
                    return;
                }

                this.ListTab.SelectedTab = tabPostPair.Item1;
                if (tabPostPair.Item2 != null && this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId) > -1)
                {
                    this.SelectListItem(this.curList, this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                    this.curList.EnsureVisible(this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void PushSelectPostChain()
        {
            if (this.selectPostChains.Count == 0 || (this.selectPostChains.Peek().Item1.Text != this.curTab.Text || !object.ReferenceEquals(this.curPost, this.selectPostChains.Peek().Item2)))
            {
                this.selectPostChains.Push(Tuple.Create(this.curTab, this.curPost));
            }
        }

        private void TrimPostChain()
        {
            if (this.selectPostChains.Count < 2000)
            {
                return;
            }

            Stack<Tuple<TabPage, PostClass>> p = new Stack<Tuple<TabPage, PostClass>>();
            for (var i = 0; i < 2000; i++)
            {
                p.Push(this.selectPostChains.Pop());
            }

            this.selectPostChains.Clear();
            for (var i = 0; i < 2000; i++)
            {
                this.selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }

            for (int tabidx = 0; tabidx <= this.ListTab.TabCount - 1; tabidx++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType != TabUsageType.DirectMessage && this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    this.ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    return true;
                }
            }

            return false;
        }

        private bool GoDirectMessage(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }

            for (int tabidx = 0; tabidx < this.ListTab.TabCount; tabidx++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage && this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    this.ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    return true;
                }
            }

            return false;
        }

        private void SaveConfigsAll(bool ifModified)
        {
            if (!ifModified)
            {
                this.SaveConfigsCommon();
                this.SaveConfigsLocal();
                this.SaveConfigsTabs();
                this.SaveConfigsAtId();
            }
            else
            {
                if (this.modifySettingCommon)
                {
                    this.SaveConfigsCommon();
                }

                if (this.modifySettingLocal)
                {
                    this.SaveConfigsLocal();
                }

                if (this.modifySettingAtId)
                {
                    this.SaveConfigsAtId();
                }
            }
        }

        private void SaveConfigsAtId()
        {
            if (this.ignoreConfigSave || (!this.settingDialog.UseAtIdSupplement && this.AtIdSupl == null))
            {
                return;
            }

            this.modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(this.AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (this.ignoreConfigSave)
            {
                return;
            }

            this.modifySettingCommon = false;
            lock (this.syncObject)
            {
                this.cfgCommon.UserName = this.tw.Username;
                this.cfgCommon.UserId = this.tw.UserId;
                this.cfgCommon.Password = this.tw.Password;
                this.cfgCommon.Token = this.tw.AccessToken;
                this.cfgCommon.TokenSecret = this.tw.AccessTokenSecret;
                this.cfgCommon.UserAccounts = this.settingDialog.UserAccounts;
                this.cfgCommon.UserstreamStartup = this.settingDialog.UserstreamStartup;
                this.cfgCommon.UserstreamPeriod = this.settingDialog.UserstreamPeriodInt;
                this.cfgCommon.TimelinePeriod = this.settingDialog.TimelinePeriodInt;
                this.cfgCommon.ReplyPeriod = this.settingDialog.ReplyPeriodInt;
                this.cfgCommon.DMPeriod = this.settingDialog.DMPeriodInt;
                this.cfgCommon.PubSearchPeriod = this.settingDialog.PubSearchPeriodInt;
                this.cfgCommon.ListsPeriod = this.settingDialog.ListsPeriodInt;
                this.cfgCommon.UserTimelinePeriod = this.settingDialog.UserTimelinePeriodInt;
                this.cfgCommon.Read = this.settingDialog.Readed;
                this.cfgCommon.IconSize = this.settingDialog.IconSz;
                this.cfgCommon.UnreadManage = this.settingDialog.UnreadManage;
                this.cfgCommon.PlaySound = this.settingDialog.PlaySound;
                this.cfgCommon.OneWayLove = this.settingDialog.OneWayLove;
                this.cfgCommon.NameBalloon = this.settingDialog.NameBalloon;
                this.cfgCommon.PostCtrlEnter = this.settingDialog.PostCtrlEnter;
                this.cfgCommon.PostShiftEnter = this.settingDialog.PostShiftEnter;
                this.cfgCommon.CountApi = this.settingDialog.CountApi;
                this.cfgCommon.CountApiReply = this.settingDialog.CountApiReply;
                this.cfgCommon.PostAndGet = this.settingDialog.PostAndGet;
                this.cfgCommon.DispUsername = this.settingDialog.DispUsername;
                this.cfgCommon.MinimizeToTray = this.settingDialog.MinimizeToTray;
                this.cfgCommon.CloseToExit = this.settingDialog.CloseToExit;
                this.cfgCommon.DispLatestPost = this.settingDialog.DispLatestPost;
                this.cfgCommon.SortOrderLock = this.settingDialog.SortOrderLock;
                this.cfgCommon.TinyUrlResolve = this.settingDialog.TinyUrlResolve;
                this.cfgCommon.ShortUrlForceResolve = this.settingDialog.ShortUrlForceResolve;
                this.cfgCommon.PeriodAdjust = this.settingDialog.PeriodAdjust;
                this.cfgCommon.StartupVersion = this.settingDialog.StartupVersion;
                this.cfgCommon.StartupFollowers = this.settingDialog.StartupFollowers;
                this.cfgCommon.RestrictFavCheck = this.settingDialog.RestrictFavCheck;
                this.cfgCommon.AlwaysTop = this.settingDialog.AlwaysTop;
                this.cfgCommon.UrlConvertAuto = this.settingDialog.UrlConvertAuto;
                this.cfgCommon.Outputz = this.settingDialog.OutputzEnabled;
                this.cfgCommon.OutputzKey = this.settingDialog.OutputzKey;
                this.cfgCommon.OutputzUrlMode = this.settingDialog.OutputzUrlmode;
                this.cfgCommon.UseUnreadStyle = this.settingDialog.UseUnreadStyle;
                this.cfgCommon.DateTimeFormat = this.settingDialog.DateTimeFormat;
                this.cfgCommon.DefaultTimeOut = this.settingDialog.DefaultTimeOut;
                this.cfgCommon.RetweetNoConfirm = this.settingDialog.RetweetNoConfirm;
                this.cfgCommon.LimitBalloon = this.settingDialog.LimitBalloon;
                this.cfgCommon.EventNotifyEnabled = this.settingDialog.EventNotifyEnabled;
                this.cfgCommon.EventNotifyFlag = this.settingDialog.EventNotifyFlag;
                this.cfgCommon.IsMyEventNotifyFlag = this.settingDialog.IsMyEventNotifyFlag;
                this.cfgCommon.ForceEventNotify = this.settingDialog.ForceEventNotify;
                this.cfgCommon.FavEventUnread = this.settingDialog.FavEventUnread;
                this.cfgCommon.TranslateLanguage = this.settingDialog.TranslateLanguage;
                this.cfgCommon.EventSoundFile = this.settingDialog.EventSoundFile;
                this.cfgCommon.AutoShortUrlFirst = this.settingDialog.AutoShortUrlFirst;
                this.cfgCommon.TabIconDisp = this.settingDialog.TabIconDisp;
                this.cfgCommon.ReplyIconState = this.settingDialog.ReplyIconState;
                this.cfgCommon.ReadOwnPost = this.settingDialog.ReadOwnPost;
                this.cfgCommon.GetFav = this.settingDialog.GetFav;
                this.cfgCommon.IsMonospace = this.settingDialog.IsMonospace;
                if (this.IdeographicSpaceToSpaceToolStripMenuItem != null && this.IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed == false)
                {
                    this.cfgCommon.WideSpaceConvert = this.IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }

                this.cfgCommon.ReadOldPosts = this.settingDialog.ReadOldPosts;
                this.cfgCommon.UseSsl = this.settingDialog.UseSsl;
                this.cfgCommon.BilyUser = this.settingDialog.BitlyUser;
                this.cfgCommon.BitlyPwd = this.settingDialog.BitlyPwd;
                this.cfgCommon.ShowGrid = this.settingDialog.ShowGrid;
                this.cfgCommon.UseAtIdSupplement = this.settingDialog.UseAtIdSupplement;
                this.cfgCommon.UseHashSupplement = this.settingDialog.UseHashSupplement;
                this.cfgCommon.PreviewEnable = this.settingDialog.PreviewEnable;
                this.cfgCommon.Language = this.settingDialog.Language;
                this.cfgCommon.SortOrder = (int)this.statuses.SortOrder;
                this.cfgCommon.SortColumn = this.GetSortColumnIndex(this.statuses.SortMode);
                this.cfgCommon.Nicoms = this.settingDialog.Nicoms;
                this.cfgCommon.HashTags = this.HashMgr.HashHistories;
                if (this.HashMgr.IsPermanent)
                {
                    this.cfgCommon.HashSelected = this.HashMgr.UseHash;
                }
                else
                {
                    this.cfgCommon.HashSelected = string.Empty;
                }

                this.cfgCommon.HashIsHead = this.HashMgr.IsHead;
                this.cfgCommon.HashIsPermanent = this.HashMgr.IsPermanent;
                this.cfgCommon.HashIsNotAddToAtReply = this.HashMgr.IsNotAddToAtReply;
                this.cfgCommon.TwitterUrl = this.settingDialog.TwitterApiUrl;
                this.cfgCommon.TwitterSearchUrl = this.settingDialog.TwitterSearchApiUrl;
                this.cfgCommon.HotkeyEnabled = this.settingDialog.HotkeyEnabled;
                this.cfgCommon.HotkeyModifier = this.settingDialog.HotkeyMod;
                this.cfgCommon.HotkeyKey = this.settingDialog.HotkeyKey;
                this.cfgCommon.HotkeyValue = this.settingDialog.HotkeyValue;
                this.cfgCommon.BlinkNewMentions = this.settingDialog.BlinkNewMentions;
                if (this.ToolStripFocusLockMenuItem != null && !this.ToolStripFocusLockMenuItem.IsDisposed)
                {
                    this.cfgCommon.FocusLockToStatusText = this.ToolStripFocusLockMenuItem.Checked;
                }

                this.cfgCommon.UseAdditionalCount = this.settingDialog.UseAdditionalCount;
                this.cfgCommon.MoreCountApi = this.settingDialog.MoreCountApi;
                this.cfgCommon.FirstCountApi = this.settingDialog.FirstCountApi;
                this.cfgCommon.SearchCountApi = this.settingDialog.SearchCountApi;
                this.cfgCommon.FavoritesCountApi = this.settingDialog.FavoritesCountApi;
                this.cfgCommon.UserTimelineCountApi = this.settingDialog.UserTimelineCountApi;
                this.cfgCommon.TrackWord = this.tw.TrackWord;
                this.cfgCommon.AllAtReply = this.tw.AllAtReply;
                this.cfgCommon.OpenUserTimeline = this.settingDialog.OpenUserTimeline;
                this.cfgCommon.ListCountApi = this.settingDialog.ListCountApi;
                this.cfgCommon.UseImageService = this.ImageServiceCombo.SelectedIndex;
                this.cfgCommon.ListDoubleClickAction = this.settingDialog.ListDoubleClickAction;
                this.cfgCommon.UserAppointUrl = this.settingDialog.UserAppointUrl;
                this.cfgCommon.HideDuplicatedRetweets = this.settingDialog.HideDuplicatedRetweets;
                this.cfgCommon.IsPreviewFoursquare = this.settingDialog.IsPreviewFoursquare;
                this.cfgCommon.FoursquarePreviewHeight = this.settingDialog.FoursquarePreviewHeight;
                this.cfgCommon.FoursquarePreviewWidth = this.settingDialog.FoursquarePreviewWidth;
                this.cfgCommon.FoursquarePreviewZoom = this.settingDialog.FoursquarePreviewZoom;
                this.cfgCommon.IsListsIncludeRts = this.settingDialog.IsListStatusesIncludeRts;
                this.cfgCommon.TabMouseLock = this.settingDialog.TabMouseLock;
                this.cfgCommon.IsRemoveSameEvent = this.settingDialog.IsRemoveSameEvent;
                this.cfgCommon.IsUseNotifyGrowl = this.settingDialog.IsNotifyUseGrowl;

                this.cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (this.ignoreConfigSave)
            {
                return;
            }

            lock (this.syncObject)
            {
                this.modifySettingLocal = false;
                this.cfgLocal.FormSize = this.mySize;
                this.cfgLocal.FormLocation = this.myLoc;
                this.cfgLocal.SplitterDistance = this.mySpDis;
                this.cfgLocal.PreviewDistance = this.mySpDis3;
                this.cfgLocal.StatusMultiline = this.StatusText.Multiline;
                this.cfgLocal.StatusTextHeight = this.mySpDis2;
                this.cfgLocal.AdSplitterDistance = this.myAdSpDis;
                this.cfgLocal.StatusText = this.settingDialog.Status;
                this.cfgLocal.FontUnread = this.fntUnread;
                this.cfgLocal.ColorUnread = this.clrUnread;
                this.cfgLocal.FontRead = this.fntReaded;
                this.cfgLocal.ColorRead = this.clrRead;
                this.cfgLocal.FontDetail = this.fntDetail;
                this.cfgLocal.ColorDetail = this.clrDetail;
                this.cfgLocal.ColorDetailBackcolor = this.clrDetailBackcolor;
                this.cfgLocal.ColorDetailLink = this.clrDetailLink;
                this.cfgLocal.ColorFav = this.clrFav;
                this.cfgLocal.ColorOWL = this.clrOWL;
                this.cfgLocal.ColorRetweet = this.clrRetweet;
                this.cfgLocal.ColorSelf = this.clrSelf;
                this.cfgLocal.ColorAtSelf = this.clrAtSelf;
                this.cfgLocal.ColorTarget = this.clrTarget;
                this.cfgLocal.ColorAtTarget = this.clrAtTarget;
                this.cfgLocal.ColorAtFromTarget = this.clrAtFromTarget;
                this.cfgLocal.ColorAtTo = this.clrAtTo;
                this.cfgLocal.ColorListBackcolor = this.clrListBackcolor;
                this.cfgLocal.ColorInputBackcolor = this.InputBackColor;
                this.cfgLocal.ColorInputFont = this.clrInputForecolor;
                this.cfgLocal.FontInputFont = this.fntInputFont;
                this.cfgLocal.BrowserPath = this.settingDialog.BrowserPath;
                this.cfgLocal.UseRecommendStatus = this.settingDialog.UseRecommendStatus;
                this.cfgLocal.ProxyType = this.settingDialog.SelectedProxyType;
                this.cfgLocal.ProxyAddress = this.settingDialog.ProxyAddress;
                this.cfgLocal.ProxyPort = this.settingDialog.ProxyPort;
                this.cfgLocal.ProxyUser = this.settingDialog.ProxyUser;
                this.cfgLocal.ProxyPassword = this.settingDialog.ProxyPassword;
                if (this.ignoreConfigSave)
                {
                    return;
                }

                this.cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            SettingTabs tabSetting = new SettingTabs();
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[i].Text].TabType != TabUsageType.Related)
                {
                    tabSetting.Tabs.Add(this.statuses.Tabs[this.ListTab.TabPages[i].Text]);
                }
            }

            tabSetting.Save();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isAuto">True=先頭に挿入、False=カーソル位置に挿入</param>
        /// <param name="isReply">True=@,False=DM</param>
        /// <param name="isAll"></param>
        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            if (!this.StatusText.Enabled)
            {
                return;
            }

            if (this.curList == null)
            {
                return;
            }

            if (this.curTab == null)
            {
                return;
            }

            if (!this.ExistCurrentPost)
            {
                return;
            }

            // 複数あてリプライはReplyではなく通常ポスト
            if (this.curList.SelectedIndices.Count > 0)
            {
                // アイテムが1件以上選択されている
                if (this.curList.SelectedIndices.Count == 1 && !isAll && this.ExistCurrentPost)
                {
                    // 単独ユーザー宛リプライまたはDM
                    if ((this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                    {
                        // ダイレクトメッセージ
                        this.StatusText.Text = "D " + this.curPost.ScreenName + " " + this.StatusText.Text;
                        this.StatusText.SelectionStart = this.StatusText.Text.Length;
                        this.StatusText.Focus();
                        this.replyToId = 0;
                        this.replyToName = string.Empty;
                        return;
                    }

                    if (string.IsNullOrEmpty(this.StatusText.Text))
                    {
                        // 空の場合 : ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        this.StatusText.Text = "@" + this.curPost.ScreenName + " ";
                        this.replyToId = this.curPost.OriginalStatusId;
                        this.replyToName = this.curPost.ScreenName;
                    }
                    else
                    {
                        // 何か入力済の場合
                        if (isAuto)
                        {
                            // 1件選んでEnter or DoubleClick
                            if (this.StatusText.Text.Contains("@" + this.curPost.ScreenName + " "))
                            {
                                if (this.replyToId > 0 && this.replyToName == this.curPost.ScreenName)
                                {
                                    // 返信先書き換え
                                    this.replyToId = this.curPost.OriginalStatusId;
                                    this.replyToName = this.curPost.ScreenName;
                                }

                                return;
                            }

                            if (!this.StatusText.Text.StartsWith("@"))
                            {
                                // 文頭＠以外
                                if (this.StatusText.Text.StartsWith(". "))
                                {
                                    // 複数リプライ
                                    this.StatusText.Text = this.StatusText.Text.Insert(2, "@" + this.curPost.ScreenName + " ");
                                    this.replyToId = 0;
                                    this.replyToName = string.Empty;
                                }
                                else
                                {
                                    // 単独リプライ
                                    this.StatusText.Text = "@" + this.curPost.ScreenName + " " + this.StatusText.Text;
                                    this.replyToId = this.curPost.OriginalStatusId;
                                    this.replyToName = this.curPost.ScreenName;
                                }
                            }
                            else
                            {
                                // 文頭＠
                                // 複数リプライ
                                this.StatusText.Text = ". @" + this.curPost.ScreenName + " " + this.StatusText.Text;
                                this.replyToId = 0;
                                this.replyToName = string.Empty;
                            }
                        }
                        else
                        {
                            // 1件選んでCtrl-Rの場合（返信先操作せず）
                            int sidx = this.StatusText.SelectionStart;
                            string id = "@" + this.curPost.ScreenName + " ";
                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    id = " " + id;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, id);
                            sidx += id.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    // 複数リプライ
                    if (!isAuto && !isReply)
                    {
                        return;
                    }

                    // C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r
                    if (isAuto)
                    {
                        // Enter or DoubleClick
                        string statusTxt = this.StatusText.Text;
                        if (!statusTxt.StartsWith(". "))
                        {
                            statusTxt = ". " + statusTxt;
                            this.replyToId = 0;
                            this.replyToName = string.Empty;
                        }

                        for (int cnt = 0; cnt <= this.curList.SelectedIndices.Count - 1; cnt++)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[cnt]);
                            if (!statusTxt.Contains("@" + post.ScreenName + " "))
                            {
                                statusTxt = statusTxt.Insert(2, "@" + post.ScreenName + " ");
                            }
                        }

                        this.StatusText.Text = statusTxt;
                    }
                    else
                    {
                        // C-S-r or C-r
                        if (this.curList.SelectedIndices.Count > 1)
                        {
                            // 複数ポスト選択
                            string ids = string.Empty;
                            int sidx = this.StatusText.SelectionStart;
                            for (int cnt = 0; cnt <= this.curList.SelectedIndices.Count - 1; cnt++)
                            {
                                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[cnt]);
                                if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.ScreenName + " ";
                                }

                                if (isAll)
                                {
                                    foreach (string nm in post.ReplyToList)
                                    {
                                        if (!ids.Contains("@" + nm + " ") && !nm.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                            if (m.Success)
                                            {
                                                ids += "@" + m.Result("${id}") + " ";
                                            }
                                            else
                                            {
                                                ids += "@" + nm + " ";
                                            }
                                        }
                                    }
                                }
                            }

                            if (ids.Length == 0)
                            {
                                return;
                            }

                            if (!this.StatusText.Text.StartsWith(". "))
                            {
                                this.StatusText.Text = ". " + this.StatusText.Text;
                                sidx += 2;
                                this.replyToId = 0;
                                this.replyToName = string.Empty;
                            }

                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                        else
                        {
                            // 1件のみ選択のC-S-r（返信元付加する可能性あり）
                            string ids = string.Empty;
                            int sidx = this.StatusText.SelectionStart;
                            PostClass post = this.curPost;
                            if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ids += "@" + post.ScreenName + " ";
                            }

                            foreach (string nm in post.ReplyToList)
                            {
                                if (!ids.Contains("@" + nm + " ") && !nm.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                    if (m.Success)
                                    {
                                        ids += "@" + m.Result("${id}") + " ";
                                    }
                                    else
                                    {
                                        ids += "@" + nm + " ";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(post.RetweetedBy))
                            {
                                if (!ids.Contains("@" + post.RetweetedBy + " ") && !post.RetweetedBy.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.RetweetedBy + " ";
                                }
                            }

                            if (ids.Length == 0)
                            {
                                return;
                            }

                            if (string.IsNullOrEmpty(this.StatusText.Text))
                            {
                                // 未入力の場合のみ返信先付加
                                this.StatusText.Text = ids;
                                this.StatusText.SelectionStart = ids.Length;
                                this.StatusText.Focus();
                                this.replyToId = post.OriginalStatusId;
                                this.replyToName = post.ScreenName;
                                return;
                            }

                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                    }
                }

                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                this.StatusText.Focus();
            }
        }

        private void RefreshTasktrayIcon(bool forceRefresh)
        {
            if (this.colorize)
            {
                this.Colorize();
            }

            if (!this.TimerRefreshIcon.Enabled)
            {
                return;
            }

            if (forceRefresh)
            {
                this.isIdle = false;
            }

            this.iconCnt += 1;
            this.blinkCnt += 1;

            bool busy = false;
            foreach (BackgroundWorker bw in this.bworkers)
            {
                if (bw != null && bw.IsBusy)
                {
                    busy = true;
                    break;
                }
            }

            if (this.iconCnt > 3)
            {
                this.iconCnt = 0;
            }

            if (this.blinkCnt > 10)
            {
                this.blinkCnt = 0;
                this.SaveConfigsAll(true);
            }

            if (busy)
            {
                this.NotifyIcon1.Icon = this.iconRefresh[this.iconCnt];
                this.isIdle = false;
                this.myStatusError = false;
                return;
            }

            TabClass tb = this.statuses.GetTabByType(TabUsageType.Mentions);
            if (this.settingDialog.ReplyIconState != ReplyIconState.None && tb != null && tb.UnreadCount > 0)
            {
                if (this.blinkCnt > 0)
                {
                    return;
                }

                this.doBlink = !this.doBlink;
                if (this.doBlink || this.settingDialog.ReplyIconState == ReplyIconState.StaticIcon)
                {
                    this.NotifyIcon1.Icon = this.replyIcon;
                }
                else
                {
                    this.NotifyIcon1.Icon = this.replyIconBlink;
                }

                this.isIdle = false;
                return;
            }

            if (this.isIdle)
            {
                return;
            }

            this.isIdle = true;

            // 優先度：エラー→オフライン→アイドル．エラーは更新アイコンでクリアされる
            if (this.myStatusError)
            {
                this.NotifyIcon1.Icon = this.iconAtRed;
                return;
            }

            if (MyCommon.IsNetworkAvailable())
            {
                this.NotifyIcon1.Icon = this.iconAt;
            }
            else
            {
                this.NotifyIcon1.Icon = this.iconAtSmoke;
            }
        }

        private void TabMenuControl(string tabName)
        {
            if (this.statuses.Tabs[tabName].TabType != TabUsageType.Mentions && this.statuses.IsDefaultTab(tabName))
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else if (this.statuses.Tabs[tabName].TabType == TabUsageType.Mentions)
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = true;
                this.DeleteTbMenuItem.Enabled = true;
            }
        }

        private bool SelectTab(ref string tabName)
        {
            do
            {
                // 振り分け先タブ選択
                if (this.tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.settingDialog.AlwaysTop;
                    return false;
                }

                this.TopMost = this.settingDialog.AlwaysTop;
                tabName = this.tabDialog.SelectedTabName;
                this.ListTab.SelectedTab.Focus();

                // 新規タブを選択→タブ作成
                if (tabName == Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText1)
                {
                    using (InputTabName inputName = new InputTabName())
                    {
                        inputName.TabName = this.statuses.GetUniqueTabName();
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }

                        tabName = inputName.TabName;
                        inputName.Dispose();
                    }

                    this.TopMost = this.settingDialog.AlwaysTop;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        if (!this.statuses.AddTab(tabName, TabUsageType.UserDefined, null) || !this.AddNewTab(tabName, false, TabUsageType.UserDefined))
                        {
                            // もう一度タブ名入力
                            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // 既存タブを選択
                    return true;
                }
            }
            while (true);
        }

        private void MoveOrCopy(ref bool move, ref bool mark)
        {
            // 移動するか？
            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
            if (MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                move = false;
            }
            else
            {
                move = true;
            }

            if (!move)
            {
                // マークするか？
                tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText6, "\r\n");
                if (MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    mark = true;
                }
                else
                {
                    mark = false;
                }
            }
        }

        private void MoveMiddle()
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = this.curList.SelectedIndices[0];

            ListViewItem item = this.curList.GetItemAt(0, 25);
            int idx1 = item == null ? 0 : item.Index;

            item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
            int idx2 = item == null ? this.curList.VirtualListSize - 1 : item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0)
            {
                idx = 0;
            }

            this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            this.curList.EnsureVisible(idx);
        }

        private void ClearTab(string tabName, bool showWarning)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            if (showWarning)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine);
                if (MessageBox.Show(tmp, tabName + " " + Hoehoe.Properties.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            this.statuses.ClearTabIds(tabName);
            if (this.ListTab.SelectedTab.Text == tabName)
            {
                this.anchorPost = null;
                this.anchorFlag = false;
                this.itemCache = null;
                this.postCache = null;
                this.itemCacheIndex = -1;
                this.curItemIndex = -1;
                this.curPost = null;
            }

            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    tb.ImageIndex = -1;
                    ((DetailsListView)tb.Tag).VirtualListSize = 0;
                    break;
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
        }

        // メインウインドウタイトルの書き換え
        private void SetMainWindowTitle()
        {
            StringBuilder ttl = new StringBuilder(256);
            int ur = 0;
            int al = 0;
            if (this.settingDialog.DispLatestPost != DispTitleEnum.None
                && this.settingDialog.DispLatestPost != DispTitleEnum.Post
                && this.settingDialog.DispLatestPost != DispTitleEnum.Ver
                && this.settingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                foreach (string key in this.statuses.Tabs.Keys)
                {
                    ur += this.statuses.Tabs[key].UnreadCount;
                    al += this.statuses.Tabs[key].AllCount;
                }
            }

            if (this.settingDialog.DispUsername)
            {
                ttl.Append(this.tw.Username).Append(" - ");
            }

            ttl.Append("Hoehoe  ");
            switch (this.settingDialog.DispLatestPost)
            {
                case DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.FileVersion);
                    break;
                case DispTitleEnum.Post:
                    if (this.postHistory != null && this.postHistory.Count > 1)
                    {
                        ttl.Append(this.postHistory[this.postHistory.Count - 2].Status.Replace("\r\n", string.Empty));
                    }

                    break;
                case DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText1, this.statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this.statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText3, ur, this.statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this.statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case DispTitleEnum.OwnStatus:
                    if (this.prevFollowerCount == 0 && this.tw.FollowersCount > 0)
                    {
                        this.prevFollowerCount = this.tw.FollowersCount;
                    }

                    ttl.AppendFormat(Hoehoe.Properties.Resources.OwnStatusTitle, this.tw.StatusesCount, this.tw.FriendsCount, this.tw.FollowersCount, this.tw.FollowersCount - this.prevFollowerCount);
                    break;
            }

            try
            {
                this.Text = ttl.ToString();
            }
            catch (AccessViolationException ex)
            {
                // 原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private string GetStatusLabelText()
        {
            // ステータス欄にカウント表示
            // タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (this.statuses == null)
            {
                return string.Empty;
            }

            TabClass mentionTab = this.statuses.GetTabByType(TabUsageType.Mentions);
            TabClass dmessageTab = this.statuses.GetTabByType(TabUsageType.DirectMessage);
            if (mentionTab == null || dmessageTab == null)
            {
                return string.Empty;
            }

            int urat = mentionTab.UnreadCount + dmessageTab.UnreadCount;
            int ur = 0;
            int al = 0;
            int tur = 0;
            int tal = 0;
            StringBuilder slbl = new StringBuilder(256);
            try
            {
                foreach (string key in this.statuses.Tabs.Keys)
                {
                    ur += this.statuses.Tabs[key].UnreadCount;
                    al += this.statuses.Tabs[key].AllCount;
                    if (key.Equals(this.curTab.Text))
                    {
                        tur = this.statuses.Tabs[key].UnreadCount;
                        tal = this.statuses.Tabs[key].AllCount;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            this.unreadCounter = ur;
            this.unreadAtCounter = urat;

            slbl.AppendFormat(Hoehoe.Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, this.postTimestamps.Count, this.favTimestamps.Count, this.timeLineCount);
            if (this.settingDialog.TimelinePeriodInt == 0)
            {
                slbl.Append(Hoehoe.Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(this.settingDialog.TimelinePeriodInt.ToString() + Hoehoe.Properties.Resources.SetStatusLabelText3);
            }

            return slbl.ToString();
        }

        private void SetStatusLabelApi()
        {
            this.apiGauge.RemainCount = MyCommon.TwitterApiInfo.RemainCount;
            this.apiGauge.MaxCount = MyCommon.TwitterApiInfo.MaxCount;
            this.apiGauge.ResetTime = MyCommon.TwitterApiInfo.ResetTime;
        }

        private void SetStatusLabelUrl()
        {
            this.StatusLabelUrl.Text = this.GetStatusLabelText();
        }

        // タスクトレイアイコンのツールチップテキスト書き換え
        // Tween [未読/@]
        private void SetNotifyIconText()
        {
            StringBuilder ur = new StringBuilder(64);
            if (this.settingDialog.DispUsername)
            {
                ur.Append(this.tw.Username);
                ur.Append(" - ");
            }

            ur.Append("Hoehoe");
#if DEBUG
            ur.Append("(Debug Build)");
#endif
            if (this.unreadCounter != -1 && this.unreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(this.unreadCounter);
                ur.Append("/@");
                ur.Append(this.unreadAtCounter);
                ur.Append("]");
            }

            this.NotifyIcon1.Text = ur.ToString();
        }

        private void DoRepliedStatusOpen()
        {
            if (this.ExistCurrentPost && this.curPost.InReplyToUser != null && this.curPost.InReplyToStatusId > 0)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    this.OpenUriAsync(string.Format("httpstwitter.com/{0}/status/{1}", this.curPost.InReplyToUser, this.curPost.InReplyToStatusId));
                    return;
                }

                if (this.statuses.ContainsKey(this.curPost.InReplyToStatusId))
                {
                    PostClass repPost = this.statuses.Item(this.curPost.InReplyToStatusId);
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.Lists | TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(this.curPost.InReplyToStatusId))
                        {
                            break;
                        }

                        PostClass repPost = this.statuses.Item(this.curPost.InReplyToStatusId);
                        MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }

                    this.OpenUriAsync("http://twitter.com/" + this.curPost.InReplyToUser + "/status/" + this.curPost.InReplyToStatusId.ToString());
                }
            }
        }

        /// <summary>
        /// t.coで投稿時自動短縮する場合は、外部サービスでの短縮禁止
        /// Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
        /// 参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
        /// Appendix A.  Collected ABNF for URI
        /// http://www.ietf.org/rfc/rfc3986.txt
        /// </summary>
        /// <param name="urlCoonverterType"></param>
        /// <returns></returns>
        private bool ConvertUrl(UrlConverter urlCoonverterType)
        {
            string result = string.Empty;

            const string NicoUrlPattern = "^https?://[a-z]+\\.(nicovideo|niconicommons|nicolive)\\.jp/[a-z]+/[a-z0-9]+$";

            if (this.StatusText.SelectionLength > 0)
            {
                // 文字列が選択されている場合はその文字列について処理
                string tmp = this.StatusText.SelectedText;

                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http"))
                {
                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settingDialog.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            this.StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        this.StatusText.Select(this.StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                        this.StatusText.SelectedText = result;

                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = tmp, After = result });
                    }
                }
            }
            else
            {
                const string UrlPattern = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?://)" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";

                // 正規表現にマッチしたURL文字列をtinyurl化
                foreach (Match mt in Regex.Matches(this.StatusText.Text, UrlPattern, RegexOptions.IgnoreCase))
                {
                    if (this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) == -1)
                    {
                        continue;
                    }

                    string tmp = mt.Result("${url}");
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase))
                    {
                        tmp = "http://" + tmp;
                    }

                    // 選んだURLを選択（？）
                    this.StatusText.Select(this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settingDialog.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            this.StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        this.StatusText.Select(this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);
                        this.StatusText.SelectedText = result;

                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = mt.Result("${url}"), After = result });
                    }
                }
            }

            return true;
        }

        private void DoUrlUndo()
        {
            if (this.urlUndoBuffer != null)
            {
                string tmp = this.StatusText.Text;
                foreach (UrlUndoInfo data in this.urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }

                this.StatusText.Text = tmp;
                this.urlUndoBuffer = null;
                this.UrlUndoToolStripMenuItem.Enabled = false;
                this.StatusText.SelectionStart = 0;
                this.StatusText.SelectionLength = 0;
            }
        }

        private void DoSearchToolStrip(string url)
        {
            // 発言詳細で「選択文字列で検索」（選択文字列取得）
            string selText = this.WebBrowser_GetSelectionText(ref this.PostBrowser);

            if (selText != null)
            {
                if (url == Hoehoe.Properties.Resources.SearchItem4Url)
                {
                    // 公式検索
                    this.AddNewTabForSearch(selText);
                    return;
                }

                string tmp = string.Format(url, HttpUtility.UrlEncode(selText));
                this.OpenUriAsync(tmp);
            }
        }

        private void ListTabSelect(TabPage tab)
        {
            this.SetListProperty();

            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;

            this.curTab = tab;
            this.curList = (DetailsListView)tab.Tag;
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.curItemIndex = this.curList.SelectedIndices[0];
                this.curPost = this.GetCurTabPost(this.curItemIndex);
            }
            else
            {
                this.curItemIndex = -1;
                this.curPost = null;
            }

            this.anchorPost = null;
            this.anchorFlag = false;

            if (this.iconCol)
            {
                ((DetailsListView)tab.Tag).Columns[1].Text = this.columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= this.curList.Columns.Count - 1; i++)
                {
                    ((DetailsListView)tab.Tag).Columns[i].Text = this.columnTexts[i];
                }
            }
        }

        private void SelectListItem(DetailsListView listView, int index)
        {
            // 単一
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (listView.FocusedItem != null)
            {
                bnd = listView.FocusedItem.Bounds;
                flg = true;
            }

            do
            {
                listView.SelectedIndices.Clear();
            }
            while (listView.SelectedIndices.Count > 0);
            listView.Items[index].Selected = true;
            listView.Items[index].Focused = true;

            if (flg)
            {
                listView.Invalidate(bnd);
            }
        }

        private void SelectListItem(DetailsListView listView, int[] indecies, int focused)
        {
            // 複数
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (listView.FocusedItem != null)
            {
                bnd = listView.FocusedItem.Bounds;
                flg = true;
            }

            int fIdx = -1;
            if (indecies != null && !(indecies.Count() == 1 && indecies[0] == -1))
            {
                do
                {
                    listView.SelectedIndices.Clear();
                }
                while (listView.SelectedIndices.Count > 0);

                foreach (int idx in indecies)
                {
                    if (idx > -1 && listView.VirtualListSize > idx)
                    {
                        listView.SelectedIndices.Add(idx);
                        if (fIdx == -1)
                        {
                            fIdx = idx;
                        }
                    }
                }
            }

            if (focused > -1 && listView.VirtualListSize > focused)
            {
                listView.Items[focused].Focused = true;
            }
            else if (fIdx > -1)
            {
                listView.Items[fIdx].Focused = true;
            }

            if (flg)
            {
                listView.Invalidate(bnd);
            }
        }

        private void RunAsync(GetWorkerArg args)
        {
            BackgroundWorker bw = null;
            if (args.WorkerType != WorkerType.Follower)
            {
                for (int i = 0; i < this.bworkers.Length; i++)
                {
                    if (this.bworkers[i] != null && !this.bworkers[i].IsBusy)
                    {
                        bw = this.bworkers[i];
                        break;
                    }
                }

                if (bw == null)
                {
                    for (int i = 0; i < this.bworkers.Length; i++)
                    {
                        if (this.bworkers[i] == null)
                        {
                            this.bworkers[i] = new BackgroundWorker();
                            bw = this.bworkers[i];
                            bw.WorkerReportsProgress = true;
                            bw.WorkerSupportsCancellation = true;
                            bw.DoWork += this.GetTimelineWorker_DoWork;
                            bw.ProgressChanged += this.GetTimelineWorker_ProgressChanged;
                            bw.RunWorkerCompleted += this.GetTimelineWorker_RunWorkerCompleted;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (this.followerFetchWorker == null)
                {
                    this.followerFetchWorker = new BackgroundWorker();
                    bw = this.followerFetchWorker;
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;
                    bw.DoWork += this.GetTimelineWorker_DoWork;
                    bw.ProgressChanged += this.GetTimelineWorker_ProgressChanged;
                    bw.RunWorkerCompleted += this.GetTimelineWorker_RunWorkerCompleted;
                }
                else
                {
                    if (this.followerFetchWorker.IsBusy == false)
                    {
                        bw = this.followerFetchWorker;
                    }
                }
            }

            if (bw == null)
            {
                return;
            }

            bw.RunWorkerAsync(args);
        }

        private void StartUserStream()
        {
            this.tw.NewPostFromStream += this.Tw_NewPostFromStream;
            this.tw.UserStreamStarted += this.Tw_UserStreamStarted;
            this.tw.UserStreamStopped += this.Tw_UserStreamStopped;
            this.tw.PostDeleted += this.Tw_PostDeleted;
            this.tw.UserStreamEventReceived += this.Tw_UserStreamEventArrived;
            this.MenuItemUserStream.Text = "&UserStream ■";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = "&Start";
            this.StopToolStripMenuItem.Enabled = true;
            if (this.settingDialog.UserstreamStartup)
            {
                this.tw.StartUserStream();
            }
        }

        private bool IsInitialRead()
        {
            return this.waitTimeline || this.waitReply || this.waitDm || this.waitFav || this.waitPubSearch || this.waitUserTimeline || this.waitLists;
        }

        private void DoGetFollowersMenu()
        {
            this.GetTimeline(WorkerType.Follower);
            this.DispSelectedPost(true);
        }

        private void DoReTweetUnofficial()
        {
            // RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsDm || !this.StatusText.Enabled)
                {
                    return;
                }

                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                string rtdata = this.CreateRetweetUnofficial(this.curPost.Text);
                this.StatusText.Text = "RT @" + this.curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                this.StatusText.SelectionStart = 0;
                this.StatusText.Focus();
            }
        }

        private void DoReTweetOfficial(bool isConfirm)
        {
            // 公式RT
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    this.doFavRetweetFlags = false;
                    return;
                }

                if (this.curList.SelectedIndices.Count > 15)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.RetweetLimitText);
                    this.doFavRetweetFlags = false;
                    return;
                }
                else if (this.curList.SelectedIndices.Count > 1)
                {
                    string confirmMessage = Hoehoe.Properties.Resources.RetweetQuestion2;
                    if (this.doFavRetweetFlags)
                    {
                        confirmMessage = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText1;
                    }

                    switch (MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            this.doFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (this.curPost.IsDm || this.curPost.IsMe)
                    {
                        this.doFavRetweetFlags = false;
                        return;
                    }

                    if (!this.settingDialog.RetweetNoConfirm)
                    {
                        string confirmMessage = Hoehoe.Properties.Resources.RetweetQuestion1;
                        if (this.doFavRetweetFlags)
                        {
                            confirmMessage = Hoehoe.Properties.Resources.FavoritesRetweetQuestionText2;
                        }

                        if (isConfirm && MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            this.doFavRetweetFlags = false;
                            return;
                        }
                    }
                }

                GetWorkerArg args = new GetWorkerArg()
                {
                    Ids = new List<long>(),
                    SIds = new List<long>(),
                    TabName = this.curTab.Text,
                    WorkerType = WorkerType.Retweet
                };
                foreach (int idx in this.curList.SelectedIndices)
                {
                    PostClass post = this.GetCurTabPost(idx);
                    if (!post.IsMe && !post.IsProtect && !post.IsDm)
                    {
                        args.Ids.Add(post.StatusId);
                    }
                }

                this.RunAsync(args);
            }
        }

        private void FavoritesRetweetOriginal()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.doFavRetweetFlags = true;
            this.DoReTweetOfficial(true);
            if (this.doFavRetweetFlags)
            {
                this.doFavRetweetFlags = false;
                this.ChangeSelectedFavStatus(true, false);
            }
        }

        private void FavoritesRetweetUnofficial()
        {
            if (this.ExistCurrentPost && !this.curPost.IsDm)
            {
                this.doFavRetweetFlags = true;
                this.ChangeSelectedFavStatus(true);
                if (!this.curPost.IsProtect && this.doFavRetweetFlags)
                {
                    this.doFavRetweetFlags = false;
                    this.DoReTweetUnofficial();
                }
            }
        }

        /// <summary>
        /// Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string CreateRetweetUnofficial(string status)
        {
            MatchCollection ms = Regex.Matches(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>");
            foreach (Match m in ms)
            {
                if (m.Result("${link}").EndsWith("..."))
                {
                    break;
                }
            }

            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\" title=\"(?<title>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${title}");

            // その他のリンク(@IDなど)を置き換える
            status = Regex.Replace(status, "@<a target=\"_self\" href=\"https?://twitter.com/(#!/)?(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "@${url}");

            // ハッシュタグ
            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${link}");

            // <br>タグ除去
            if (this.StatusText.Multiline)
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
            return status.Replace("&nbsp;", " ");
        }

        private bool IsKeyDown(Keys key)
        {
            return (Control.ModifierKeys & key) == key;
        }

        private void FollowCommand(string id)
        {
            if (id == null)
            {
                return;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Follow");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                id = inputName.TabName.Trim();
            }

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (id == this.tw.Username)
            {
                return;
            }

            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = this.tw, Id = id };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.FollowCommandText1, this.FollowCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? Hoehoe.Properties.Resources.FRMessage2 + ret : Hoehoe.Properties.Resources.FRMessage3);
            }
        }

        private void RemoveCommand(string id, bool skipInput)
        {
            if (id == null)
            {
                return;
            }

            if (!skipInput)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Unfollow");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    id = inputName.TabName.Trim();
                }
            }

            if (string.IsNullOrEmpty(id) || id == this.tw.Username)
            {
                return;
            }

            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = this.tw, Id = id };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.RemoveCommandText1, this.RemoveCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? Hoehoe.Properties.Resources.FRMessage2 + ret : Hoehoe.Properties.Resources.FRMessage3);
            }
        }

        private void ShowFriendship(string id)
        {
            if (id == null)
            {
                return;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Show Friendships");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                id = inputName.TabName;
            }

            ShowFriendshipCore(id);
        }

        private void ShowFriendship(string[] ids)
        {
            foreach (string id in ids)
            {
                ShowFriendshipCore(id);
            }
        }

        private void ShowFriendshipCore(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                if (id.ToLower() == this.tw.Username.ToLower())
                {
                    return;
                }

                ShowFriendshipArgs args = new ShowFriendshipArgs() { Tw = this.tw };
                args.Ids.Add(new ShowFriendshipArgs.FriendshipInfo(id));
                string ret = string.Empty;
                using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                {
                    formInfo.ShowDialog();
                    ret = (string)formInfo.Result;
                }

                if (string.IsNullOrEmpty(ret))
                {
                    ShowFriendshipArgs.FriendshipInfo frsinfo = args.Ids[0];
                    string fing = frsinfo.IsFollowing ?
                        Hoehoe.Properties.Resources.GetFriendshipInfo1 :
                        Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    string fed = frsinfo.IsFollowed ?
                        Hoehoe.Properties.Resources.GetFriendshipInfo3 :
                        Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    string result = frsinfo.Id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine;
                    result += "  " + fing + System.Environment.NewLine;
                    result += "  " + fed;
                    if (frsinfo.IsFollowing)
                    {
                        if (MessageBox.Show(Hoehoe.Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result,
                            Hoehoe.Properties.Resources.GetFriendshipInfo8, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.RemoveCommand(frsinfo.Id, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show(result);
                    }
                }
                else
                {
                    MessageBox.Show(ret);
                }
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this.postBrowserStatusText, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
            {
                return m.Result("${ScreenName}");
            }
            else
            {
                return null;
            }
        }

        private void DoQuote()
        {
            // QT @id:内容
            // 返信先情報付加
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsDm || !this.StatusText.Enabled)
                {
                    return;
                }

                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                string rtdata = this.CreateRetweetUnofficial(this.curPost.Text);
                this.StatusText.Text = " QT @" + this.curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                this.replyToId = this.curPost.OriginalStatusId;
                this.replyToName = this.curPost.ScreenName;

                this.StatusText.SelectionStart = 0;
                this.StatusText.Focus();
            }
        }

        private void TryOpenSelectedRtUserHome()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                var post = this.GetCurTabPost(this.curList.SelectedIndices[0]);
                if (post.IsRetweeted)
                {
                    this.OpenUriAsync("https://twitter.com/" + post.RetweetedBy);
                }
            }
        }

        private void ShowUserStatus(string id, bool showInputDialog = true)
        {
            if (id == null)
            {
                return;
            }

            if (showInputDialog)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Show UserStatus");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    id = inputName.TabName.Trim();
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var user = new DataModels.Twitter.User();
            GetUserInfoArgs args = new GetUserInfoArgs() { Tw = this.tw, Id = id, User = user };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                if (string.IsNullOrEmpty(ret))
                {

                    using (ShowUserInfo userinfo = new ShowUserInfo())
                    {
                        userinfo.Owner = this;
                        userinfo.SetUser(user);
                        userinfo.ShowDialog(this);
                        this.Activate();
                        this.BringToFront();
                    }
                }
                else
                {
                    MessageBox.Show(ret);
                }
            }
        }

        private void LoadImageFromSelectedFile()
        {
            try
            {
                if (string.IsNullOrEmpty(this.ImagefilePathText.Text.Trim()) || string.IsNullOrEmpty(this.ImageService))
                {
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    return;
                }

                FileInfo fl = new FileInfo(this.ImagefilePathText.Text.Trim());
                if (!this.pictureServices[this.ImageService].CheckValidExtension(fl.Extension))
                {
                    // 画像以外の形式
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    return;
                }

                if (!this.pictureServices[this.ImageService].CheckValidFilesize(fl.Extension, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    MessageBox.Show("File is too large.");
                    return;
                }

                switch (this.pictureServices[this.ImageService].GetFileType(fl.Extension))
                {
                    case UploadFileType.Invalid:
                        this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        this.ImagefilePathText.Text = string.Empty;
                        break;
                    case UploadFileType.Picture:
                        Image img = null;
                        using (FileStream fs = new FileStream(this.ImagefilePathText.Text, FileMode.Open, FileAccess.Read))
                        {
                            img = Image.FromStream(fs);
                            fs.Close();
                        }

                        this.ImageSelectedPicture.Image = (new HttpVarious()).CheckValidImage(img, img.Width, img.Height);
                        this.ImageSelectedPicture.Tag = UploadFileType.Picture;
                        break;
                    case UploadFileType.MultiMedia:
                        this.ImageSelectedPicture.Image = Hoehoe.Properties.Resources.MultiMediaImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.MultiMedia;
                        break;
                    default:
                        this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        this.ImagefilePathText.Text = string.Empty;
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                this.ImagefilePathText.Text = string.Empty;
                MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                this.ImagefilePathText.Text = string.Empty;
                MessageBox.Show("The type of this file is not image.");
            }
        }

        private void SetImageServiceCombo()
        {
            string svc = string.Empty;
            if (this.ImageServiceCombo.SelectedIndex > -1)
            {
                svc = this.ImageServiceCombo.SelectedItem.ToString();
            }

            this.ImageServiceCombo.Items.Clear();
            this.ImageServiceCombo.Items.Add("TwitPic");
            this.ImageServiceCombo.Items.Add("img.ly");
            this.ImageServiceCombo.Items.Add("yfrog");
            this.ImageServiceCombo.Items.Add("lockerz");
            this.ImageServiceCombo.Items.Add("Twitter");

            if (string.IsNullOrEmpty(svc))
            {
                this.ImageServiceCombo.SelectedIndex = 0;
            }
            else
            {
                int idx = this.ImageServiceCombo.Items.IndexOf(svc);
                this.ImageServiceCombo.SelectedIndex = idx == -1 ? 0 : idx;
            }
        }

        private static void CopyToClipboard(string clstr)
        {
            try
            {
                Clipboard.SetDataObject(clstr, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyUserId()
        {
            if (this.curPost == null)
            {
                return;
            }

            CopyToClipboard(this.curPost.ScreenName);
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            // 新着通知
            if (this.IsBalloonRequired(ev))
            {
                this.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                StringBuilder title = new StringBuilder();
                if (this.settingDialog.DispUsername)
                {
                    title.Append(this.tw.Username);
                    title.Append(" - ");
                }

                title.Append(string.Format("Hoehoe [{0}] ", ev.Event.ToUpper()));
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(string.Format("by {0}", ev.Username));
                }

                string text = !string.IsNullOrEmpty(ev.Target) ? ev.Target : " ";

                if (this.settingDialog.IsNotifyUseGrowl)
                {
                    this.growlHelper.Notify(GrowlHelper.NotifyType.UserStreamEvent, ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    this.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                    this.NotifyIcon1.BalloonTipTitle = title.ToString();
                    this.NotifyIcon1.BalloonTipText = text;
                    this.NotifyIcon1.ShowBalloonTip(500);
                }
            }

            if (Convert.ToBoolean(ev.Eventtype & this.settingDialog.EventNotifyFlag) && this.IsMyEventNotityAsEventType(ev))
            {
                // サウンド再生
                if (!this.isInitializing && this.settingDialog.PlaySound)
                {
                    MyCommon.PlaySound(this.settingDialog.EventSoundFile);
                }
            }
        }

        private void DoTranslation(string str)
        {
            Bing bing = new Bing();
            string buf = string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            string srclng = string.Empty;
            string dstlng = this.settingDialog.TranslateLanguage;
            string msg = string.Empty;
            if (srclng != dstlng && bing.Translate(string.Empty, dstlng, str, ref buf))
            {
                this.PostBrowser.DocumentText = this.CreateDetailHtml(buf);
            }
            else
            {
                if (msg.StartsWith("Err:"))
                {
                    this.StatusLabel.Text = msg;
                }
            }
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle(caption);
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    id = inputName.TabName.Trim();
                }
                else
                {
                    id = string.Empty;
                }
            }

            return id;
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            if (isEnable)
            {
                this.tw.StartUserStream();
            }
            else
            {
                this.tw.StopUserStream();
            }

            this.timerTimeline.Enabled = isEnable;
        }

        private void OpenUserAppointUrl()
        {
            if (this.settingDialog.UserAppointUrl != null)
            {
                if (this.settingDialog.UserAppointUrl.Contains("{ID}") || this.settingDialog.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (this.curPost != null)
                    {
                        string url = this.settingDialog.UserAppointUrl
                            .Replace("{ID}", this.curPost.ScreenName)
                            .Replace("{STATUS}", this.curPost.OriginalStatusId.ToString());
                        this.OpenUriAsync(url);
                    }
                }
                else
                {
                    this.OpenUriAsync(this.settingDialog.UserAppointUrl);
                }
            }
        }


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
            this.ApplyNewFilters();
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
            string tabName = this.statuses.GetUniqueTabName();
            TabUsageType tabUsage = default(TabUsageType);
            if (!TryGetTabInfo(ref tabName, ref tabUsage, showusage: true))
            {
                return;
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
            finally
            {
                this.Cursor = Cursors.Default;
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

            this.ApplyNewFilters();
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

        private void SetFocusToMainMenu()
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            this.MenuStrip1.Tag = new object();
            this.MenuStrip1.Select();
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

        private void TryOpenCurListSelectedUserFavorites()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync(string.Format("https://twitter.com/{0}/favorites", this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName));
            }
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

        private void ChangeNewPostPopupSetting(bool popup)
        {
            this.cfgCommon.NewAllPop = this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked = popup;
            this.SetModifySettingCommon(true);
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

        private static bool IsTwitterSearchUrl(string url)
        {
            return url.StartsWith("http://twitter.com/search?q=") || url.StartsWith("https://twitter.com/search?q=");
        }

        private void TryOpenUrlInCurrentTweet()
        {
            if (this.PostBrowser.Document.Links.Count < 1)
            {
                return;
            }

            this.urlDialog.ClearUrl();
            string openUrlStr = string.Empty;
            foreach (HtmlElement linkElm in this.PostBrowser.Document.Links)
            {
                try
                {
                    string urlStr = linkElm.GetAttribute("title");
                    string href = MyCommon.IDNDecode(linkElm.GetAttribute("href"));
                    if (string.IsNullOrEmpty(urlStr))
                    {
                        urlStr = href;
                    }

                    string linkText = linkElm.InnerText;
                    if (!linkText.StartsWith("http") && !linkText.StartsWith("#") && !linkText.Contains("."))
                    {
                        linkText = "@" + linkText;
                    }

                    if (string.IsNullOrEmpty(urlStr))
                    {
                        continue;
                    }

                    openUrlStr = MyCommon.GetUrlEncodeMultibyteChar(urlStr);
                    this.urlDialog.AddUrl(new OpenUrlItem(linkText, openUrlStr, href));
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

            try
            {
                if (this.PostBrowser.Document.Links.Count != 1)
                {
                    if (this.urlDialog.ShowDialog() == DialogResult.OK)
                    {
                        openUrlStr = this.urlDialog.SelectedUrl;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (string.IsNullOrEmpty(openUrlStr))
            {
                return;
            }

            if (IsTwitterSearchUrl(openUrlStr))
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
                    return;
                }
            }

            openUrlStr = openUrlStr.Replace("://twitter.com/search?q=#", "://twitter.com/search?q=%23");
            this.OpenUriAsync(openUrlStr);
        }

        private void ChangePlaySoundSetting(bool play)
        {
            this.settingDialog.PlaySound = this.PlaySoundFileMenuItem.Checked = this.PlaySoundMenuItem.Checked = play;
            this.SetModifySettingCommon(true);
        }

        private void PostBrowser_NavigatedExtracted(Uri eUrl)
        {
            if (eUrl.AbsoluteUri != "about:blank")
            {
                this.DispSelectedPost();
                this.OpenUriAsync(eUrl.OriginalString);
            }
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

            if (IsTwitterSearchUrl(eUrlAbsoluteUri))
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
            if (!m.Success)
            {
                this.OpenUriAsync(eUrlOriginalString);
                return true;
            }

            string screenName = m.Result("${ScreenName}");
            if (!this.IsTwitterId(screenName))
            {
                this.OpenUriAsync(eUrlOriginalString);
                return true;
            }

            // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
            bool isCtrlKeyDown = this.IsKeyDown(Keys.Control);
            bool isOpenInTab = this.settingDialog.OpenUserTimeline;
            if ((isOpenInTab && !isCtrlKeyDown) || (!isOpenInTab && isCtrlKeyDown))
            {
                this.AddNewTabForUserTimeline(screenName);
            }
            else
            {
                this.OpenUriAsync(eUrlOriginalString);
            }
            return true;
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

        private void GetPostStatusHeaderFooter(bool isRemoveFooter, out string header, out string footer)
        {
            footer = string.Empty;
            header = string.Empty;
            if (this.StatusText.Text.StartsWith("D ") || this.StatusText.Text.StartsWith("d "))
            {
                // DM時は何もつけない
                footer = string.Empty;
                return;
            }

            // ハッシュタグ
            string hash = string.Empty;
            if (this.HashMgr.IsNotAddToAtReply)
            {
                if (this.replyToId == 0 && string.IsNullOrEmpty(this.replyToName))
                {
                    hash = this.HashMgr.UseHash;
                }
            }
            else
            {
                hash = this.HashMgr.UseHash;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                if (this.HashMgr.IsHead)
                {
                    header = hash + " ";
                }
                else
                {
                    footer = " " + hash;
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

        private bool GetPostImageInfo(out string imgService, out string imgPath)
        {
            imgService = imgPath = string.Empty;
            if (!this.ImageSelectionPanel.Visible)
            {
                return true;
            }

            // 画像投稿
            if (object.ReferenceEquals(this.ImageSelectedPicture.Image, this.ImageSelectedPicture.InitialImage)
                || this.ImageServiceCombo.SelectedIndex < 0
                || string.IsNullOrEmpty(this.ImagefilePathText.Text))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.PostPictureWarn1, Hoehoe.Properties.Resources.PostPictureWarn2);
                return false;
            }

            var rslt = MessageBox.Show(Hoehoe.Properties.Resources.PostPictureConfirm1, Hoehoe.Properties.Resources.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rslt == DialogResult.Cancel)
            {
                this.TimelinePanel.Visible = true;
                this.TimelinePanel.Enabled = true;
                this.ImageSelectionPanel.Visible = false;
                this.ImageSelectionPanel.Enabled = false;
                if (this.curList != null)
                {
                    this.curList.Focus();
                }
                return false;
            }

            imgService = this.ImageServiceCombo.Text;
            imgPath = this.ImagefilePathText.Text;

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

            return true;
        }

        private void TryPostTweet()
        {
            string statusTextTextTrim = this.StatusText.Text.Trim();
            if (statusTextTextTrim.Length == 0)
            {
                if (!this.ImageSelectionPanel.Enabled)
                {
                    this.DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && statusTextTextTrim == string.Format("RT @{0}: {1}", this.curPost.ScreenName, this.curPost.TextFromApi))
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

            this.postHistory[this.postHistory.Count - 1] = new PostingStatus(statusTextTextTrim, this.replyToId, this.replyToName);

            if (this.settingDialog.Nicoms)
            {
                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                this.ConvertUrl(UrlConverter.Nicoms);
            }

            this.StatusText.SelectionStart = this.StatusText.Text.Length;
            this.CheckReplyTo(this.StatusText.Text);

            // 整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = statusTextTextTrim;
            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase)
                    && !tmpStatus.EndsWith(" ."))
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

            string footer, header;
            GetPostStatusHeaderFooter(isRemoveFooter, out header, out footer);
            var postStatus = header + statusTextTextTrim + footer;
            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(postStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase)
                    && !postStatus.EndsWith(" ."))
                {
                    postStatus += " .";
                }
            }

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(postStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+");
                if (mc2.Success)
                {
                    postStatus = Regex.Replace(postStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+", "$& ");
                }
            }

            if (this.IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                postStatus = postStatus.Replace("　", " ");
            }

            if (isCutOff && postStatus.Length > 140)
            {
                postStatus = postStatus.Substring(0, 140);
                const string AtId = "(@|＠)[a-z0-9_/]+$";
                const string HashTag = "(^|[^0-9A-Z&\\/\\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$";
                const string Url = "https?:\\/\\/[a-z0-9!\\*'\\(\\);:&=\\+\\$\\/%#\\[\\]\\-_\\.,~?]+$";

                // 簡易判定
                string pattern = string.Format("({0})|({1})|({2})", AtId, HashTag, Url);
                Match mc = Regex.Match(postStatus, pattern, RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    // さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                    postStatus = postStatus.Substring(0, 140 - mc.Value.Length);
                }

                if (MessageBox.Show(postStatus, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            string imgService, imgPath;
            if (!GetPostImageInfo(out imgService, out imgPath))
            {
                return;
            }

            this.RunAsync(new GetWorkerArg()
            {
                WorkerType = WorkerType.PostMessage,
                PStatus = new PostingStatus()
                {
                    ImagePath = imgPath,
                    ImageService = imgService,
                    InReplyToId = this.replyToId,
                    InReplyToName = this.replyToName,
                    Status = postStatus
                }
            });

            // Google検索（試験実装）
            if (this.StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && statusTextTextTrim.Length > 7)
            {
                this.OpenUriAsync(string.Format(Hoehoe.Properties.Resources.SearchItem2Url, HttpUtility.UrlEncode(this.StatusText.Text.Substring(7))));
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

        private void ChangeSelectetdTweetReadState(bool read)
        {
            this.curList.BeginUpdate();
            if (this.settingDialog.UnreadManage)
            {
                foreach (int idx in this.curList.SelectedIndices)
                {
                    this.statuses.SetReadAllTab(read, this.curTab.Text, idx);
                }
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(read, idx, this.curTab);
            }

            this.ColorizeList();
            this.curList.EndUpdate();
        }

        private void ChangeTabsIconToRead()
        {
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (tb.ImageIndex == 0)
                    {
                        tb.ImageIndex = -1;
                    }
                }
            }
        }

        private void ChangeTabsIconToUnread()
        {
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (tb.ImageIndex == -1)
                    {
                        tb.ImageIndex = 0;
                    }
                }
            }
        }

        private void ChangeSelectetdTweetReadStateToRead()
        {
            ChangeSelectetdTweetReadState(read: true);
            if (this.settingDialog.TabIconDisp)
            {
                ChangeTabsIconToRead();
            }
            else
            {
                this.ListTab.Refresh();
            }
        }

        private void ChangeSelectedTweetReadSateToUnread()
        {
            ChangeSelectetdTweetReadState(read: false);
            if (this.settingDialog.TabIconDisp)
            {
                ChangeTabsIconToUnread();
            }
            else
            {
                this.ListTab.Refresh();
            }
        }

        private void TryUnfollowCurrentTweetUser()
        {
            this.RemoveCommand(this.curPost != null ? this.curPost.ScreenName : string.Empty, false);
        }

        private void TryUnfollowUserInCurrentTweet()
        {
            this.RemoveCommand(this.GetUserId(), false);
        }

        private void TryUnfollowCurrentIconUser()
        {
            if (this.NameLabel.Tag != null)
            {
                this.RemoveCommand((string)this.NameLabel.Tag, false);
            }
        }

        private void TrySaveCurrentTweetUserIcon()
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);
            if (this.SaveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                using (Image orgBmp = new Bitmap(this.iconDict[name]))
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
            catch (Exception ex)
            {
                // 処理中にキャッシュアウトする可能性あり
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void TrySaveLog()
        {
            DialogResult rslt = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine),
                Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

                var idxs = rslt == DialogResult.Yes ?
                    Enumerable.Range(0, this.curList.VirtualListSize) :
                    this.curList.SelectedIndices.Cast<int>();
                var lines = idxs
                    .Select(idx => this.statuses.Item(this.curTab.Text, idx))
                    .Select(post => string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                        post.Nickname,
                        post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""),
                        post.CreatedAt,
                        post.ScreenName,
                        post.StatusId,
                        post.ImageUrl,
                        post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""),
                        post.IsProtect ? "Protect" : string.Empty));
                using (StreamWriter sw = new StreamWriter(this.SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    foreach (var line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }

            this.TopMost = this.settingDialog.AlwaysTop;
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

        private void AddNewTabForAtUserSearch(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            this.AddNewTabForSearch("@" + id);
        }

        private void AddSearchTabForAtUserOfCurrentTweet()
        {
            if (this.NameLabel.Tag != null)
            {
                AddNewTabForAtUserSearch((string)this.NameLabel.Tag);
            }
        }

        private void AddSearchTabForAtUserInCurrentTweet()
        {
            this.AddNewTabForAtUserSearch(this.GetUserId());
        }

        private void ChangeSearchPanelControlsTabStop(Control pnl, bool newVariable)
        {
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = newVariable;
            }
        }

        private void AddTimelineTabForCurrentTweetUser()
        {
            if (this.NameLabel.Tag != null)
            {
                this.AddNewTabForUserTimeline((string)this.NameLabel.Tag);
            }
        }

        private void AddTimelineTabForUserInCurrentTweet()
        {
            this.AddNewTabForUserTimeline(this.GetUserId());
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

        private void TryCopySelectionInPostBrowser()
        {
            CopyToClipboard(this.WebBrowser_GetSelectionText(ref this.PostBrowser));
        }

        private void SetStatusLabelApiLuncher()
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

        private void AddRelatedStatusesTab()
        {
            if (!this.ExistCurrentPost || this.curPost.IsDm)
            {
                return;
            }

            // PublicSearchも除外した方がよい？
            if (this.statuses.GetTabByType(TabUsageType.Related) == null)
            {
                const string TabName = "Related Tweets";
                string newTabName = TabName;
                if (this.AddNewTab(newTabName, false, TabUsageType.Related))
                {
                    this.statuses.AddTab(newTabName, TabUsageType.Related, null);
                }
                else
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

        private void ChangeCurrentTabSoundFile(string soundfile)
        {
            if (this.soundfileListup || string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.statuses.Tabs[this.rclickTabName].SoundFile = soundfile;
            this.SaveConfigsTabs();
        }

        private void TryCopySourceName()
        {
            CopyToClipboard(this.SourceLinkLabel.Text);
        }

        private void TryOpenSourceLink()
        {
            string link = (string)this.SourceLinkLabel.Tag;
            if (!string.IsNullOrEmpty(link))
            {
                this.OpenUriAsync(link);
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

        private void TryCopySourceUrl()
        {
            CopyToClipboard(Convert.ToString(this.SourceLinkLabel.Tag));
        }

        private void TryOpenSelectedTweetWebPage()
        {
            if (this.curList.SelectedIndices.Count > 0 && this.statuses.Tabs[this.curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[0]);
                this.OpenUriAsync(string.Format("https://twitter.com/{0}/status/{1}", post.ScreenName, post.OriginalStatusId));
            }
        }

        private void StatusText_EnterExtracted()
        {
            /// フォーカスの戻り先を StatusText に設定
            this.Tag = this.StatusText;
            this.StatusText.BackColor = this.InputBackColor;
        }

        private void ShowSupplementBox(char eKeyChar)
        {
            if (eKeyChar == '@')
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
            }
            else if (eKeyChar == '#')
            {
                if (!this.settingDialog.UseHashSupplement)
                {
                    return;
                }

                this.ShowSuplDialog(this.StatusText, this.HashSupl);
            }
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

        private void ChangeStatusTextMultiline(bool isMultiLine)
        {
            this.StatusText.ScrollBars = isMultiLine ? ScrollBars.Vertical : ScrollBars.None;
            this.modifySettingLocal = true;
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

            this.ApplyNewFilters();
            this.SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null && ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                this.curPost = this.statuses.Item(this.ListTab.SelectedTab.Text, ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

        private void RenameCurrentTabName()
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.TabRename(ref this.rclickTabName);
        }

        private void RenameSelectedTabName()
        {
            string tn = this.ListTab.SelectedTab.Text;
            this.TabRename(ref tn);
        }

        private void DecrementTimer(ref int counter)
        {
            if (counter > 0)
            {
                Interlocked.Decrement(ref counter);
            }
        }

        private bool ResetWorkerTimer(ref int counter, int initailValue, WorkerType worker, bool reset)
        {
            if (reset || counter < 1 && initailValue > 0)
            {
                Interlocked.Exchange(ref counter, initailValue);
                if (!this.tw.IsUserstreamDataReceived && !reset)
                {
                    this.GetTimeline(worker);
                }
                return false;
            }
            return reset;
        }

        private void TimerTimeline_ElapsedExtracted()
        {
            DecrementTimer(ref this.timerHomeCounter);
            DecrementTimer(ref this.timerMentionCounter);
            DecrementTimer(ref this.timerDmCounter);
            DecrementTimer(ref this.timerPubSearchCounter);
            DecrementTimer(ref this.timerUserTimelineCounter);
            DecrementTimer(ref this.timerListsCounter);
            DecrementTimer(ref this.timerUsCounter);
            DecrementTimer(ref this.timerRefreshFollowers);

            // 'タイマー初期化
            this.resetTimers.Timeline = ResetWorkerTimer(ref this.timerHomeCounter, this.settingDialog.TimelinePeriodInt, WorkerType.Timeline, this.resetTimers.Timeline);
            this.resetTimers.Reply = ResetWorkerTimer(ref this.timerMentionCounter, this.settingDialog.ReplyPeriodInt, WorkerType.Reply, this.resetTimers.Reply);
            this.resetTimers.DirectMessage = ResetWorkerTimer(ref this.timerDmCounter, this.settingDialog.DMPeriodInt, WorkerType.DirectMessegeRcv, this.resetTimers.DirectMessage);
            this.resetTimers.PublicSearch = ResetWorkerTimer(ref this.timerPubSearchCounter, this.settingDialog.PubSearchPeriodInt, WorkerType.PublicSearch, this.resetTimers.PublicSearch);
            this.resetTimers.UserTimeline = ResetWorkerTimer(ref this.timerUserTimelineCounter, this.settingDialog.UserTimelinePeriodInt, WorkerType.UserTimeline, this.resetTimers.UserTimeline);
            this.resetTimers.Lists = ResetWorkerTimer(ref this.timerListsCounter, this.settingDialog.ListsPeriodInt, WorkerType.List, this.resetTimers.Lists);
            if (this.resetTimers.UserStream || (this.timerUsCounter <= 0 && this.settingDialog.UserstreamPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerUsCounter, this.settingDialog.UserstreamPeriodInt);
                if (this.isActiveUserstream)
                {
                    this.RefreshTimeline(true);
                }

                this.resetTimers.UserStream = false;
            }

            if (this.timerRefreshFollowers < 1)
            {
                Interlocked.Exchange(ref this.timerRefreshFollowers, 6 * 3600);
                this.DoGetFollowersMenu();
                this.GetTimeline(WorkerType.Configuration);
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }

            if (!this.isOsResumed)
            {
                return;
            }

            Interlocked.Increment(ref this.timerResumeWait);
            if (this.timerResumeWait > 30)
            {
                this.isOsResumed = false;
                Interlocked.Exchange(ref this.timerResumeWait, 0);
                this.GetTimeline(WorkerType.Timeline);
                this.GetTimeline(WorkerType.Reply);
                this.GetTimeline(WorkerType.DirectMessegeRcv);
                this.GetTimeline(WorkerType.PublicSearch);
                this.GetTimeline(WorkerType.UserTimeline);
                this.GetTimeline(WorkerType.List);
                this.DoGetFollowersMenu();
                this.GetTimeline(WorkerType.Configuration);
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }
        }

        private void ChangeAutoUrlConvertFlag(bool autoConvert)
        {
            this.settingDialog.UrlConvertAuto = autoConvert;
        }

        private bool TryGetTabInfo(ref string name, ref TabUsageType usageType, string title = "", string desc = "", bool showusage = false)
        {
            using (var form = new InputTabName() { TabName = name })
            {
                form.SetFormTitle(title);
                form.SetFormDescription(desc);
                form.SetIsShowUsage(showusage);
                var result = form.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return false;
                }
                name = form.TabName;
                if (showusage)
                {
                    usageType = form.Usage;
                }
            }
            return true;
        }

        private bool TryUserInputText(ref string val, string title = "", string desc = "")
        {
            TabUsageType tmp = TabUsageType.UserDefined;
            return TryGetTabInfo(ref val, ref tmp, title, desc, false);
        }

        private void ChangeTrackWordStatus()
        {
            if (!this.TrackToolStripMenuItem.Checked)
            {
                this.tw.TrackWord = string.Empty;
                this.tw.ReconnectUserStream();
            }
            else
            {
                string q = this.prevTrackWord;
                if (!TryUserInputText(ref q, "Input track word", "Track word"))
                {
                    this.TrackToolStripMenuItem.Checked = false;
                    return;
                }

                this.prevTrackWord = q;
                if (this.prevTrackWord != this.tw.TrackWord)
                {
                    this.tw.TrackWord = this.prevTrackWord;
                    this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.prevTrackWord);
                    this.tw.ReconnectUserStream();
                }
            }

            this.modifySettingCommon = true;
        }

        private void TranslateCurrentTweet()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.DoTranslation(this.curPost.TextFromApi);
        }

        private void ChangeUserStreamStatusDisplay(bool start)
        {
            this.MenuItemUserStream.Text = start ? "&UserStream ▶" : "&UserStream ■";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = start ? "&Stop" : "&Start";
            this.StopToolStripMenuItem.Enabled = true;
            this.StatusLabel.Text = start ? "UserStream Started." : "UserStream Stopped.";
        }

        private void ActivateMainFormControls()
        {
            /// 画面がアクティブになったら、発言欄の背景色戻す
            if (this.StatusText.Focused)
            {
                this.StatusText_EnterExtracted();
            }
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

        private void TweenMain_FormClosingExtracted(FormClosingEventArgs e)
        {
            if (!this.settingDialog.CloseToExit && e.CloseReason == CloseReason.UserClosing && !MyCommon.IsEnding)
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

        private void TweenMain_LocationChangedExtracted()
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myLoc = this.DesktopLocation;
                this.modifySettingLocal = true;
            }
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

            if (!MyCommon.IsNetworkAvailable())
            {
                this.isInitializing = false;
                this.timerTimeline.Enabled = true;
                return;
            }

            string tabNameAny = string.Empty;
            this.GetTimeline(WorkerType.BlockIds);
            if (this.settingDialog.StartupFollowers)
            {
                this.GetTimeline(WorkerType.Follower);
            }

            this.GetTimeline(WorkerType.Configuration);
            this.StartUserStream();
            this.waitTimeline = true;
            this.GetTimeline(WorkerType.Timeline, 1, 1);
            this.waitReply = true;
            this.GetTimeline(WorkerType.Reply, 1, 1);
            this.waitDm = true;
            this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 1);
            if (this.settingDialog.GetFav)
            {
                this.waitFav = true;
                this.GetTimeline(WorkerType.Favorites, 1, 1);
            }

            this.waitPubSearch = true;
            this.GetTimeline(WorkerType.PublicSearch);
            this.waitUserTimeline = true;
            this.GetTimeline(WorkerType.UserTimeline);
            this.waitLists = true;
            this.GetTimeline(WorkerType.List);
            int i = 0, j = 0;
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
                this.GetTimeline(WorkerType.Follower);
            }

            // 取得失敗の場合は再試行する
            if (this.settingDialog.TwitterConfiguration.PhotoSizeLimit == 0)
            {
                this.GetTimeline(WorkerType.Configuration);
            }

            // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
            if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ReAuthorizeText);
                this.TryShowSettingsBox();
            }

            this.isInitializing = false;
            this.timerTimeline.Enabled = true;
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

        private void TryCopyUrlInCurrentTweet()
        {
            MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + this.postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?://[^\"]+)\"", RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                if (m.Groups["url"].Value == this.postBrowserStatusText)
                {
                    CopyToClipboard(m.Groups["title"].Value);
                    break;
                }
            }

            if (mc.Count == 0)
            {
                CopyToClipboard(this.postBrowserStatusText);
            }
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

        private void TryOpenCurrentNameLabelUserHome()
        {
            if (this.NameLabel.Tag != null)
            {
                this.OpenUriAsync(string.Format("https://twitter.com/{0}", (string)this.NameLabel.Tag));
            }
        }

        private void ChangeUserPictureCursor(Cursor cursorsDefault)
        {
            this.UserPicture.Cursor = cursorsDefault;
        }

        #endregion private methods
    }
}