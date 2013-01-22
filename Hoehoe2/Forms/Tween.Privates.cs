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
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using TweenCustomControl;
    using R = Properties.Resources;

    public partial class TweenMain
    {
        #region private methods

        private static bool CheckAccountValid()
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                _accountCheckErrorCount += 1;
                if (_accountCheckErrorCount > 5)
                {
                    _accountCheckErrorCount = 0;
                    Twitter.AccountState = AccountState.Valid;
                    return true;
                }

                return false;
            }

            _accountCheckErrorCount = 0;
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

        private static bool IsTwitterSearchUrl(string url)
        {
            return url.StartsWith("http://twitter.com/search?q=") || url.StartsWith("https://twitter.com/search?q=");
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
                    HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }

            if (!string.IsNullOrEmpty(HashMgr.UseHash) && !hstr.Contains(HashMgr.UseHash + " "))
            {
                hstr += HashMgr.UseHash;
            }

            if (!string.IsNullOrEmpty(hstr))
            {
                HashMgr.AddHashToHistory(hstr.Trim(), false);
            }

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(statusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");
            if (_configs.UseAtIdSupplement)
            {
                if (AtIdSupl.AddRangeItem(m.Cast<Match>().Select(mid => mid.Result("${id}"))))
                {
                    SetModifySettingAtId(true);
                }
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (_replyToId == 0)
            {
                return;
            }

            // リプライ先ユーザー名がない場合も指定しない
            if (string.IsNullOrEmpty(_replyToName))
            {
                ClearReplyToInfo();
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
                    if (statusText.StartsWith("@" + _replyToName))
                    {
                        return;
                    }
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (statusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + _replyToName)
                        {
                            return;
                        }
                    }
                }
            }

            ClearReplyToInfo();
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
            _iconAt = LoadIcon(iconDir, "At.ico", R.At);                 // タスクトレイ通常時アイコン
            _iconAtRed = LoadIcon(iconDir, "AtRed.ico", R.AtRed);        // タスクトレイエラー時アイコン
            _iconAtSmoke = LoadIcon(iconDir, "AtSmoke.ico", R.AtSmoke);  // タスクトレイオフライン時アイコン
            _tabIcon = LoadIcon(iconDir, "Tab.ico", R.TabIcon);          // タブ見出し未読表示アイコン
            _mainIcon = LoadIcon(iconDir, "MIcon.ico", R.MIcon);         // 画面のアイコン
            _replyIcon = LoadIcon(iconDir, "Reply.ico", R.Reply);         // Replyのアイコン
            _replyIconBlink = LoadIcon(iconDir, "ReplyBlink.ico", R.ReplyBlink);            // Reply点滅のアイコン

            // タスクトレイ更新中アイコン アニメーション対応により4種類読み込み
            _iconRefresh[0] = LoadIcon(iconDir, "Refresh.ico", R.Refresh);
            _iconRefresh[1] = LoadIcon(iconDir, "Refresh2.ico", R.Refresh2);
            _iconRefresh[2] = LoadIcon(iconDir, "Refresh3.ico", R.Refresh3);
            _iconRefresh[3] = LoadIcon(iconDir, "Refresh4.ico", R.Refresh4);
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
            var columns = new[]
            {
                string.Empty,
                R.AddNewTabText2,
                R.AddNewTabText3,
                R.AddNewTabText4_2,
                R.AddNewTabText5,
                string.Empty,
                string.Empty,
                "Source"
            };
            for (var i = 0; i < columns.Length; ++i)
            {
                _columnTexts[i] = _columnOrgTexts[i] = columns[i];
            }

            // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE, U+25B4 BLACK UP-POINTING SMALL TRIANGLE
            string mark = _statuses.SortOrder == SortOrder.Descending ? "▾" : "▴";
            int c = _iconCol ? 2 : GetSortColumnIndex(_statuses.SortMode);
            _columnTexts[c] = _columnOrgTexts[c] + mark;
        }

        private void InitializeTraceFrag()
        {
#if DEBUG
            TraceOutToolStripMenuItem.Checked = true;
            MyCommon.TraceFlag = true;
#endif
            if (!MyCommon.FileVersion.EndsWith("0"))
            {
                TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void CreatePictureServices()
        {
            if (_pictureServices != null)
            {
                _pictureServices.Clear();
            }

            _pictureServices = new Dictionary<string, IMultimediaShareService>
            {
                { "TwitPic", new TwitPic(_tw) },
                { "img.ly", new Imgly(_tw) },
                { "yfrog", new Yfrog(_tw) },
                { "lockerz", new Plixi(_tw) },
                { "Twitter", new TwitterPhoto(_tw) }
            };
        }

        private void LoadConfig()
        {
            _cfgCommon = SettingCommon.Load();
            if (_cfgCommon.UserAccounts == null || _cfgCommon.UserAccounts.Count == 0)
            {
                _cfgCommon.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(_cfgCommon.UserName))
                {
                    _cfgCommon.UserAccounts.Add(new UserAccount
                    {
                        Username = _cfgCommon.UserName,
                        UserId = _cfgCommon.UserId,
                        Token = _cfgCommon.Token,
                        TokenSecret = _cfgCommon.TokenSecret
                    });
                }
            }

            _cfgLocal = SettingLocal.Load();
            var tabs = SettingTabs.Load().Tabs;
            foreach (var tb in tabs)
            {
                try
                {
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = _statuses.GetUniqueTabName();
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
            }

            if (_statuses.Tabs.Count == 0)
            {
                _statuses.AddTab(MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
            }
        }

        private void RefreshTimeline(bool isUserStream)
        {
            if (isUserStream)
            {
                RefreshTasktrayIcon(true);
            }

            // スクロール制御準備
            int smode = -1; // -1:制御しない,-2:最新へ,その他:topitem使用
            long topId = GetScrollPos(ref smode);
            int befCnt = _curList.VirtualListSize;

            // 現在の選択状態を退避
            var selId = new Dictionary<string, long[]>();
            var focusedId = new Dictionary<string, long>();
            SaveSelectedStatus(selId, focusedId);

            // mentionsの更新前件数を保持
            int dmessageCount = _statuses.GetTabByType(TabUsageType.DirectMessage).AllCount;

            // 更新確定
            PostClass[] notifyPosts = null;
            string soundFile = string.Empty;
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = _statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon.IsEnding)
            {
                return;
            }

            // リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = _statuses.Tabs[tab.Text];
                    lst.BeginUpdate();
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        if (lst.Equals(_curList))
                        {
                            _itemCache = null;
                            _postCache = null;
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

                        SelectListItem(lst, _statuses.IndexOf(tab.Text, selId[tab.Text]), _statuses.IndexOf(tab.Text, focusedId[tab.Text]));
                    }

                    lst.EndUpdate();
                    if (tabInfo.UnreadCount > 0)
                    {
                        if (_configs.TabIconDisp)
                        {
                            if (tab.ImageIndex == -1)
                            {
                                // タブアイコン
                                tab.ImageIndex = 0;
                            }
                        }
                    }
                }

                if (!_configs.TabIconDisp)
                {
                    ListTab.Refresh();
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
                    if (befCnt != _curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                // 最上行
                                if (_curList.VirtualListSize > 0)
                                {
                                    _curList.EnsureVisible(0);
                                }

                                break;
                            case -2:
                                // 最下行へ
                                if (_curList.VirtualListSize > 0)
                                {
                                    _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                }

                                break;
                            case -1:
                                // 制御しない
                                break;
                            default:
                                // 表示位置キープ
                                if (_curList.VirtualListSize > 0 && _statuses.IndexOf(_curTab.Text, topId) > -1)
                                {
                                    _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                    _curList.EnsureVisible(_statuses.IndexOf(_curTab.Text, topId));
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
            NotifyNewPosts(notifyPosts, soundFile, addCount, isMention || dmessageCount != _statuses.GetTabByType(TabUsageType.DirectMessage).AllCount);

            SetMainWindowTitle();
            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                SetStatusLabelUrl();
            }

            HashSupl.AddRangeItem(_tw.GetHashList());
        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (_curList == null || _curTab == null || _curList.VirtualListSize <= 0)
            {
                smode = -1;
                return topId;
            }

            if (_statuses.SortMode != IdComparerClass.ComparerMode.Id)
            {
                // 現在表示位置へ強制スクロール
                if (_curList.TopItem != null)
                {
                    topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                }

                smode = 0;
                return topId;
            }

            if (_statuses.SortOrder == SortOrder.Ascending)
            {
                // Id昇順
                if (ListLockMenuItem.Checked)
                {
                    // 制御しない(現在表示位置へ強制スクロール)
                    smode = -1;
                }
                else
                {
                    // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                    ListViewItem item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
                    if (item == null)
                    {
                        // 一番下
                        item = _curList.Items[_curList.Items.Count - 1];
                    }

                    if (item.Index == _curList.Items.Count - 1)
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
                if (ListLockMenuItem.Checked)
                {
                    // 現在表示位置へ強制スクロール
                    if (_curList.TopItem != null)
                    {
                        topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                    }

                    smode = 0;
                }
                else
                {
                    // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                    ListViewItem item = _curList.GetItemAt(0, 10);
                    if (item == null)
                    {
                        // 一番上
                        item = _curList.Items[0];
                    }

                    if (item.Index == 0)
                    {
                        // 最上行
                        smode = -3;
                    }
                    else
                    {
                        if (_curList.TopItem != null)
                        {
                            topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
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

            foreach (TabPage tab in ListTab.TabPages)
            {
                var lst = (DetailsListView)tab.Tag;
                if (lst.SelectedIndices.Count > 0 && lst.SelectedIndices.Count < 61)
                {
                    selId.Add(tab.Text, _statuses.GetId(tab.Text, lst.SelectedIndices.Cast<int>()));
                }
                else
                {
                    selId.Add(tab.Text, new long[] { -2 });
                }

                if (lst.FocusedItem != null)
                {
                    focusedId.Add(tab.Text, _statuses.GetId(tab.Text, lst.FocusedItem.Index));
                }
                else
                {
                    focusedId.Add(tab.Text, -2);
                }
            }
        }

        private bool IsBalloonRequired()
        {
            return IsBalloonRequired(new Twitter.FormattedEvent { Eventtype = EventType.None });
        }

        private bool IsBalloonRequired(Twitter.FormattedEvent ev)
        {
            return IsEventNotifyAsEventType(ev.Eventtype)
                && IsMyEventNotityAsEventType(ev)
                && (NewPostPopMenuItem.Checked || (_configs.ForceEventNotify && ev.Eventtype != EventType.None))
                && !_isInitializing
                && ((_configs.LimitBalloon && (WindowState == FormWindowState.Minimized || !Visible || ActiveForm == null)) || !_configs.LimitBalloon)
                && !Win32Api.IsScreenSaverRunning();
        }

        private bool IsEventNotifyAsEventType(EventType type)
        {
            return (_configs.EventNotifyEnabled && Convert.ToBoolean(type & _configs.EventNotifyFlag)) || type == EventType.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return Convert.ToBoolean(ev.Eventtype & _configs.IsMyEventNotifyFlag) ? true : !ev.IsMe;
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null
                && notifyPosts.Count() > 0
                && _configs.ReadOwnPost
                && notifyPosts.All(post => post.UserId == _tw.UserId || post.ScreenName == _tw.Username))
            {
                return;
            }

            // 新着通知
            if (IsBalloonRequired() && notifyPosts != null && notifyPosts.Length > 0)
            {
                // Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                if (_configs.IsNotifyUseGrowl)
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

                        switch (_configs.NameBalloon)
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
                            if (!ReferenceEquals(notifyPosts.Last(), post))
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
                        _growlHelper.Notify(nt, post.StatusId.ToString(), titleStr, notifyText, _iconDict[post.ImageUrl], post.ImageUrl);
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

                        switch (_configs.NameBalloon)
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

                    var titleStr = GetNotifyTitlteText(addCount, reply, dm);
                    var notifyIcon = dm ? ToolTipIcon.Warning :
                        reply ? ToolTipIcon.Warning :
                        ToolTipIcon.Info;
                    NotifyIcon1.BalloonTipTitle = titleStr;
                    NotifyIcon1.BalloonTipText = notifyText;
                    NotifyIcon1.BalloonTipIcon = notifyIcon;
                    NotifyIcon1.ShowBalloonTip(500);
                }
            }

            // サウンド再生
            if (!_isInitializing && _configs.PlaySound)
            {
                MyCommon.PlaySound(soundFile);
            }

            // mentions新着時に画面ブリンク
            if (!_isInitializing && _configs.BlinkNewMentions && newMentions && ActiveForm == null)
            {
                Win32Api.FlashMyWindow(Handle, Win32Api.FlashSpecification.FlashTray, 3);
            }
        }

        private string GetNotifyTitlteText(int addCount, bool reply, bool dm)
        {
            var title = new StringBuilder();
            if (_configs.DispUsername)
            {
                title.AppendFormat("{0} - ", _tw.Username);
            }

            string notifyType = string.Empty,
                msg1 = R.RefreshTimelineText1,
                msg2 = R.RefreshTimelineText2;
            if (dm)
            {
                notifyType = "[DM]";
                msg1 = R.RefreshDirectMessageText1;
                msg2 = R.RefreshDirectMessageText2;
            }
            else if (reply)
            {
                notifyType = "[Reply!]";
                msg1 = R.RefreshTimelineText1;
                msg2 = R.RefreshTimelineText2;
            }

            title.AppendFormat("Hoehoe {0} {1} {2} {3}", notifyType, msg1, addCount, msg2);
            return title.ToString();
        }

        private void ChangeCacheStyleRead(bool read, int index, TabPage tab)
        {
            // Read_:True=既読 False=未読
            // 未読管理していなかったら既読として扱う
            if (!_statuses.Tabs[_curTab.Text].UnreadManage || !_configs.UnreadManage)
            {
                read = true;
            }

            // 対象の特定
            ListViewItem itm = null;
            PostClass post = null;
            if (tab.Equals(_curTab) && _itemCache != null && index >= _itemCacheIndex && index < _itemCacheIndex + _itemCache.Length)
            {
                itm = _itemCache[index - _itemCacheIndex];
                post = _postCache[index - _itemCacheIndex];
            }
            else
            {
                itm = ((DetailsListView)tab.Tag).Items[index];
                post = _statuses.Item(tab.Text, index);
            }

            ChangeItemStyleRead(read, itm, post, (DetailsListView)tab.Tag);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem item, PostClass post, DetailsListView listView)
        {
            bool useUnreadStyle = _configs.UseUnreadStyle;

            // フォント
            Font fnt = read ? _fntReaded : _fntUnread;

            // 文字色
            Color cl = _clrUnread;
            if (post.IsFav)
            {
                cl = _clrFav;
            }
            else if (post.IsRetweeted)
            {
                cl = _clrRetweet;
            }
            else if (post.IsOwl && (post.IsDm || _configs.OneWayLove))
            {
                cl = _clrOwl;
            }
            else if (read || !useUnreadStyle)
            {
                cl = _clrRead;
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
            if (_itemCache == null)
            {
                return;
            }

            var post = _anchorFlag ? _anchorPost : _curPost;
            if (post == null)
            {
                return;
            }

            try
            {
                for (var cnt = 0; cnt < _itemCache.Length; ++cnt)
                {
                    _curList.ChangeItemBackColor(_itemCacheIndex + cnt, JudgeColor(post, _postCache[cnt]));
                }
            }
            catch (Exception)
            {
            }
        }

        private void ColorizeList(ListViewItem item, int index)
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            var post = _anchorFlag ? _anchorPost : _curPost;
            if (post == null)
            {
                return;
            }

            var target = GetCurTabPost(index);
            if (item.Index == -1)
            {
                item.BackColor = JudgeColor(post, target);
            }
            else
            {
                _curList.ChangeItemBackColor(item.Index, JudgeColor(post, target));
            }
        }

        private Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl = _clrListBackcolor;  // その他
            if (targetPost.StatusId == basePost.InReplyToStatusId)
            {
                cl = _clrAtTo;             // @先
            }
            else if (targetPost.IsMe)
            {
                cl = _clrSelf;            // 自分=発言者
            }
            else if (targetPost.IsReply)
            {
                cl = _clrAtSelf;          // 自分宛返信
            }
            else if (basePost.ReplyToList.Contains(targetPost.ScreenName.ToLower()))
            {
                cl = _clrAtFromTarget;    // 返信先
            }
            else if (targetPost.ReplyToList.Contains(basePost.ScreenName.ToLower()))
            {
                cl = _clrAtTarget;        // その人への返信
            }
            else if (targetPost.ScreenName.Equals(basePost.ScreenName, StringComparison.OrdinalIgnoreCase))
            {
                cl = _clrTarget;          // 発言者
            }

            return cl;
        }

        private string MakeStatusMessage(GetWorkerArg asyncArg, bool isFinish)
        {
            string smsg = string.Empty;
            switch (asyncArg.WorkerType)
            {
                case WorkerType.Timeline:
                    smsg = isFinish ?
                        R.GetTimelineWorker_RunWorkerCompletedText1 :
                        string.Format("{0}{1}{2}", R.GetTimelineWorker_RunWorkerCompletedText5, asyncArg.Page, R.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.Reply:
                    smsg = isFinish ?
                        R.GetTimelineWorker_RunWorkerCompletedText9 :
                        string.Format("{0}{1}{2}", R.GetTimelineWorker_RunWorkerCompletedText4, asyncArg.Page, R.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.DirectMessegeRcv:
                    smsg = isFinish ?
                        R.GetTimelineWorker_RunWorkerCompletedText11 :
                        string.Format("{0}{1}{2}", R.GetTimelineWorker_RunWorkerCompletedText8, asyncArg.Page, R.GetTimelineWorker_RunWorkerCompletedText6);
                    break;
                case WorkerType.FavAdd:
                    // 進捗メッセージ残す
                    smsg = isFinish ?
                        string.Empty :
                        string.Format("{0}{1}/{2}{3}{4}", R.GetTimelineWorker_RunWorkerCompletedText15, asyncArg.Page, asyncArg.Ids.Count, R.GetTimelineWorker_RunWorkerCompletedText16, asyncArg.Page - asyncArg.SIds.Count - 1);
                    break;
                case WorkerType.FavRemove:
                    // 進捗メッセージ残す
                    smsg = isFinish ?
                        string.Empty :
                        string.Format("{0}{1}/{2}{3}{4}", R.GetTimelineWorker_RunWorkerCompletedText17, asyncArg.Page, asyncArg.Ids.Count, R.GetTimelineWorker_RunWorkerCompletedText18, asyncArg.Page - asyncArg.SIds.Count - 1);
                    break;
                case WorkerType.Favorites:
                    smsg = isFinish ?
                        R.GetTimelineWorker_RunWorkerCompletedText20 :
                        R.GetTimelineWorker_RunWorkerCompletedText19;
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
                        R.UpdateFollowersMenuItem1_ClickText3 :
                        string.Empty;
                    break;
                case WorkerType.Configuration:
                    // 進捗メッセージ残す
                    break;
                case WorkerType.BlockIds:
                    smsg = isFinish ?
                        R.UpdateBlockUserText3 :
                        string.Empty;
                    break;
            }

            return smsg;
        }

        private void RemovePostFromFavTab(long[] ids)
        {
            string favTabName = _statuses.GetTabByType(TabUsageType.Favorites).TabName;
            bool isCurFavTab = _curTab.Text.Equals(favTabName);
            int fidx = 0;
            if (isCurFavTab)
            {
                if (_curList.FocusedItem != null)
                {
                    fidx = _curList.FocusedItem.Index;
                }
                else if (_curList.TopItem != null)
                {
                    fidx = _curList.TopItem.Index;
                }
            }

            foreach (long i in ids)
            {
                try
                {
                    _statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                }
            }

            if (_curTab != null && isCurFavTab)
            {
                // キャッシュ破棄
                _itemCache = null;
                _postCache = null;
                _curPost = null;
                //// _curItemIndex = -1
            }

            foreach (TabPage tp in ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = _statuses.Tabs[favTabName].AllCount;
                    break;
                }
            }

            if (isCurFavTab)
            {
                ResetFocusedItem(favTabName, fidx);
            }
        }

        private void GetTimeline(WorkerType workerType, int fromPage = 1, int toPage = 0, string tabName = "")
        {
            if (!MyCommon.IsNetworkAvailable())
            {
                return;
            }

            if (_lastTimeWork == null)
            {
                _lastTimeWork = new Dictionary<WorkerType, DateTime>();
            }

            // 非同期実行引数設定
            if (!_lastTimeWork.ContainsKey(workerType))
            {
                _lastTimeWork.Add(workerType, new DateTime());
            }

            double period = DateTime.Now.Subtract(_lastTimeWork[workerType]).TotalSeconds;
            if (period > 1 || period < -1)
            {
                _lastTimeWork[workerType] = DateTime.Now;
                RunAsync(new GetWorkerArg { Page = fromPage, EndPage = toPage, WorkerType = workerType, TabName = tabName });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isFavAdd">TrueでFavAdd,FalseでFavRemove</param>
        /// <param name="multiFavoriteChangeDialogEnable"></param>
        private void ChangeSelectedFavStatus(bool isFavAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            if (_statuses.Tabs[_curTab.Text].TabType == TabUsageType.DirectMessage || _curList.SelectedIndices.Count == 0 || !ExistCurrentPost)
            {
                return;
            }

            // 複数fav確認msg
            if (_curList.SelectedIndices.Count > 250 && isFavAdd)
            {
                MessageBox.Show(R.FavoriteLimitCountText);
                _doFavRetweetFlags = false;
                return;
            }

            if (multiFavoriteChangeDialogEnable && _curList.SelectedIndices.Count > 1)
            {
                if (isFavAdd)
                {
                    string confirmMessage = _doFavRetweetFlags ?
                        R.FavoriteRetweetQuestionText3 :
                        R.FavAddToolStripMenuItem_ClickText1;
                    var result = MessageBox.Show(confirmMessage, R.FavAddToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        _doFavRetweetFlags = false;
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show(R.FavRemoveToolStripMenuItem_ClickText1, R.FavRemoveToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            var selcteds = _curList.SelectedIndices.Cast<int>().Select(i => GetCurTabPost(i));
            var ids = isFavAdd ? selcteds.Where(p => !p.IsFav) : selcteds.Where(p => p.IsFav);
            if (ids.Count() == 0)
            {
                StatusLabel.Text = isFavAdd ?
                    R.FavAddToolStripMenuItem_ClickText4 :
                    R.FavRemoveToolStripMenuItem_ClickText4;
                return;
            }

            RunAsync(new GetWorkerArg
            {
                Ids = ids.Select(p => p.StatusId).ToList(),
                SIds = new List<long>(),
                TabName = _curTab.Text,
                WorkerType = isFavAdd ? WorkerType.FavAdd : WorkerType.FavRemove
            });
        }

        private PostClass GetCurTabPost(int index)
        {
            if (_postCache != null && index >= _itemCacheIndex && index < _itemCacheIndex + _postCache.Length)
            {
                return _postCache[index - _itemCacheIndex];
            }

            return _statuses.Item(_curTab.Text, index);
        }

        private void DeleteSelected()
        {
            if (_curTab == null || _curList == null)
            {
                return;
            }

            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            bool isDmTab = _statuses.Tabs[_curTab.Text].TabType == TabUsageType.DirectMessage;
            if (!isDmTab)
            {
                if (!_curList.SelectedIndices.Cast<int>().Select(i => GetCurTabPost(i)).Any(p => IsPostMine(p)))
                {
                    return;
                }
            }

            var tmp = string.Format(R.DeleteStripMenuItem_ClickText1, Environment.NewLine);
            var rslt = MessageBox.Show(tmp, R.DeleteStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            int prevFocused = 0;
            if (_curList.FocusedItem != null)
            {
                prevFocused = _curList.FocusedItem.Index;
            }
            else if (_curList.TopItem != null)
            {
                prevFocused = _curList.TopItem.Index;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                bool deleted = true;
                var statusIds = _curList.SelectedIndices.Cast<int>().Select(i => _statuses.GetId(_curTab.Text, i));
                foreach (var statusId in statusIds)
                {
                    string ret = string.Empty;
                    var post = _statuses.Item(statusId);
                    if (isDmTab)
                    {
                        ret = _tw.RemoveDirectMessage(statusId, post);
                    }
                    else
                    {
                        if (IsPostMine(post))
                        {
                            ret = _tw.RemoveStatus(statusId);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(ret))
                    {
                        _statuses.RemovePost(statusId);
                    }
                    else
                    {
                        deleted = false;
                    }
                }

                StatusLabel.Text = deleted ?
                    R.DeleteStripMenuItem_ClickText4 :
                    R.DeleteStripMenuItem_ClickText3;

                // キャッシュ破棄
                _itemCache = null;
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                    if (_curTab.Equals(tb))
                    {
                        ResetFocusedItem(tb.Text, prevFocused);
                    }
                }

                if (_configs.TabIconDisp)
                {
                    ChangeTabsIconToRead();
                }
                else
                {
                    ListTab.Refresh();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ResetFocusedItem(string tabName, int prevFocused)
        {
            do
            {
                _curList.SelectedIndices.Clear();
            }
            while (_curList.SelectedIndices.Count > 0);

            if (_statuses.Tabs[tabName].AllCount > 0)
            {
                if (_statuses.Tabs[tabName].AllCount - 1 > prevFocused && prevFocused > -1)
                {
                    _curList.SelectedIndices.Add(prevFocused);
                }
                else
                {
                    _curList.SelectedIndices.Add(_statuses.Tabs[tabName].AllCount - 1);
                }

                if (_curList.SelectedIndices.Count > 0)
                {
                    _curList.EnsureVisible(_curList.SelectedIndices[0]);
                    _curList.FocusedItem = _curList.Items[_curList.SelectedIndices[0]];
                }
            }
        }

        private bool IsPostMine(PostClass p)
        {
            return p.IsMe || p.RetweetedBy.ToLower() == _tw.Username.ToLower();
        }

        private void RefreshTab(bool more = false)
        {
            int startPage = more ? -1 : 1;
            if (_curTab == null)
            {
                GetTimeline(WorkerType.Timeline, startPage);
                return;
            }

            TabClass tb = _statuses.Tabs[_curTab.Text];
            switch (tb.TabType)
            {
                case TabUsageType.Mentions:
                    GetTimeline(WorkerType.Reply, startPage);
                    break;
                case TabUsageType.DirectMessage:
                    GetTimeline(WorkerType.DirectMessegeRcv, startPage);
                    break;
                case TabUsageType.Favorites:
                    GetTimeline(WorkerType.Favorites, startPage);
                    break;
                case TabUsageType.UserTimeline:
                    GetTimeline(WorkerType.UserTimeline, startPage, 0, _curTab.Text);
                    break;
                case TabUsageType.PublicSearch:
                    if (string.IsNullOrEmpty(tb.SearchWords))
                    {
                        return;
                    }

                    GetTimeline(WorkerType.PublicSearch, startPage, 0, _curTab.Text);
                    break;
                case TabUsageType.Lists:
                    if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                    {
                        return;
                    }

                    GetTimeline(WorkerType.List, startPage, 0, _curTab.Text);
                    break;
                case TabUsageType.Profile:
                    /* TODO: profile tab ? */
                    break;
                default:
                    GetTimeline(WorkerType.Timeline, startPage);
                    break;
            }
        }

        private void ShowUserTimeline()
        {
            if (!ExistCurrentPost)
            {
                return;
            }

            AddNewTabForUserTimeline(_curPost.ScreenName);
        }

        private void SetListProperty()
        {
            // 削除などで見つからない場合は処理せず
            if (_curList == null)
            {
                return;
            }

            if (!_isColumnChanged)
            {
                return;
            }

            int[] dispOrder = new int[_curList.Columns.Count];
            for (int i = 0; i < _curList.Columns.Count; i++)
            {
                for (int j = 0; j < _curList.Columns.Count; j++)
                {
                    if (_curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break;
                    }
                }
            }

            // 列幅、列並びを他のタブに設定
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (!tb.Equals(_curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i < lst.Columns.Count; ++i)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = _curList.Columns[i].Width;
                        }
                    }
                }
            }

            _isColumnChanged = false;
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            // 文字数カウント
            int len = 140 - StatusText.Text.Length;
            if (NotifyIcon1 == null || !NotifyIcon1.Visible)
            {
                return len;
            }

            if ((isAuto && !IsKeyDown(Keys.Control) && _configs.PostShiftEnter)
                || (isAuto && !IsKeyDown(Keys.Shift) && !_configs.PostShiftEnter)
                || (!isAuto && isAddFooter))
            {
                if (_configs.UseRecommendStatus)
                {
                    len -= _configs.RecommendStatusText.Length;
                }
                else if (_configs.Status.Length > 0)
                {
                    len -= _configs.Status.Length + 1;
                }
            }

            if (!string.IsNullOrEmpty(HashMgr.UseHash))
            {
                len -= HashMgr.UseHash.Length + 1;
            }

            foreach (Match m in Regex.Matches(StatusText.Text, Twitter.UrlRegexPattern, RegexOptions.IgnoreCase))
            {
                len += m.Result("${url}").Length - _configs.TwitterConfiguration.ShortUrlLength;
            }

            if (ImageSelectionPanel.Visible && ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(ImageService))
            {
                len -= _configs.TwitterConfiguration.CharactersReservedPerMedia;
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
                if (endIndex >= _statuses.Tabs[_curTab.Text].AllCount)
                {
                    endIndex = _statuses.Tabs[_curTab.Text].AllCount - 1;
                }

                _postCache = _statuses.Item(_curTab.Text, startIndex, endIndex); // 配列で取得
                _itemCacheIndex = startIndex;
                _itemCache = new ListViewItem[_postCache.Length];
                for (int i = 0; i <= _postCache.Length - 1; i++)
                {
                    _itemCache[i] = CreateItem(_curTab, _postCache[i], startIndex + i);
                }
            }
            catch (Exception)
            {
                // キャッシュ要求が実データとずれるため（イベントの遅延？）
                _postCache = null;
                _itemCache = null;
            }
        }

        private ListViewItem CreateItem(TabPage tabPage, PostClass post, int index)
        {
            var mk = new StringBuilder();
            if (post.FavoritedCount > 0)
            {
                mk.AppendFormat("+{0}", post.FavoritedCount);
            }

            string postedByDetail = post.ScreenName;
            if (post.IsRetweeted)
            {
                postedByDetail += string.Format("{0}(RT:{1})", Environment.NewLine, post.RetweetedBy);
            }

            bool read = post.IsRead;
            if (!_statuses.Tabs[tabPage.Text].UnreadManage || !_configs.UnreadManage)
            {
                // 未読管理していなかったら既読として扱う
                read = true;
            }

            var subitem = new[] { string.Empty, post.Nickname, (post.IsDeleted ? "(DELETED)" : string.Empty) + post.TextFromApi, post.CreatedAt.ToString(_configs.DateTimeFormat), postedByDetail, string.Empty, mk.ToString(), post.Source };
            var itm = new ImageListViewItem(subitem, _iconDict, post.ImageUrl)
            {
                StateImageIndex = post.StateIndex
            };

            ChangeItemStyleRead(read, itm, post, null);
            if (tabPage.Equals(_curTab))
            {
                ColorizeList(itm, index);
            }

            return itm;
        }

        private void DrawListViewItemIcon(DrawListViewItemEventArgs e)
        {
            ImageListViewItem item = (ImageListViewItem)e.Item;
            //// e.Bounds.Leftが常に0を指すから自前で計算
            Rectangle itemRect = item.Bounds;
            itemRect.Width = item.ListView.Columns[0].Width;
            foreach (ColumnHeader clm in item.ListView.Columns)
            {
                if (clm.DisplayIndex < item.ListView.Columns[0].DisplayIndex)
                {
                    itemRect.X += clm.Width;
                }
            }

            var iconSize = (item.Image != null) ? _iconSz : 1;
            var iconRect = Rectangle.Intersect(new Rectangle(item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(iconSize, iconSize)), itemRect);
            if (item.Image != null)
            {
                iconRect.Offset(0, (int)Math.Max(0.0, (itemRect.Height - _iconSz) / 2));
            }

            var stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + _iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);

            if (item.Image != null && iconRect.Width > 0)
            {
                e.Graphics.FillRectangle(Brushes.White, iconRect);
                e.Graphics.InterpolationMode = InterpolationMode.High;
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
                    e.Graphics.DrawImage(PostStateImageList.Images[item.StateImageIndex], stateRect);
                }
            }
        }

        private void SearchInTab(string word, bool isCaseSensitive, bool isUseRegex, InTabSearchType searchType)
        {
            if (_curList.VirtualListSize == 0)
            {
                MessageBox.Show(R.DoTabSearchText2, R.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            RegexOptions regOpt = RegexOptions.None;
            StringComparison fndOpt = StringComparison.Ordinal;
            if (!isCaseSensitive)
            {
                regOpt = RegexOptions.IgnoreCase;
                fndOpt = StringComparison.OrdinalIgnoreCase;
            }

            Regex searchRegex = null;
            if (isUseRegex)
            {
                try
                {
                    searchRegex = new Regex(word, regOpt);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(R.DoTabSearchText1, "Hoehoe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int cidx = _curList.SelectedIndices.Count > 0 ? _curList.SelectedIndices[0] : 0;
            if (searchType == InTabSearchType.NextSearch)
            {
                cidx++;
            }

            var listsize = _curList.VirtualListSize;
            var indecies = Enumerable.Range(0, listsize).Select(i => (i + cidx) % listsize);
            if (searchType == InTabSearchType.PrevSearch)
            {
                indecies = indecies.Reverse();
            }

            // 検索 : TODO: maybe slow.
            foreach (int idx in indecies)
            {
                try
                {
                    PostClass post = _statuses.Item(_curTab.Text, idx);
                    if ((isUseRegex && post.IsMatch(searchRegex)) || post.IsMatch(word, fndOpt))
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        return;
                    }
                }
                catch (Exception)
                {
                    // todo: do something?
                }
            }

            MessageBox.Show(R.DoTabSearchText2, R.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // TODO: to hoehoe2
        private void RunTweenUp()
        {
            try
            {
                Process.Start(new ProcessStartInfo
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
            bool forceUpdate = IsKeyDown(Keys.Shift);
            try
            {
                retMsg = _tw.GetVersionInfo();
            }
            catch (Exception)
            {
                StatusLabel.Text = R.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(R.CheckNewVersionText10, string.Format(R.CheckNewVersionText2, MyCommon.AppTitle), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
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
                    string tmp = string.Format(R.CheckNewVersionText3, strVer);
                    using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                    {
                        if (dialogAsShieldicon.ShowDialog(tmp, strDetail, string.Format(R.CheckNewVersionText1, MyCommon.AppTitle), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            retMsg = _tw.GetTweenBinary(strVer);
                            if (retMsg.Length == 0)
                            {
                                RunTweenUp();
                                MyCommon.IsEnding = true;
                                Close();
                                return;
                            }

                            if (!startup)
                            {
                                MessageBox.Show(R.CheckNewVersionText5 + Environment.NewLine + retMsg, string.Format(R.CheckNewVersionText2, MyCommon.AppTitle), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                }
                else
                {
                    if (forceUpdate)
                    {
                        string tmp = string.Format(R.CheckNewVersionText6, strVer);
                        using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                        {
                            if (dialogAsShieldicon.ShowDialog(tmp, strDetail, string.Format(R.CheckNewVersionText1, MyCommon.AppTitle), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                retMsg = _tw.GetTweenBinary(strVer);
                                if (retMsg.Length == 0)
                                {
                                    RunTweenUp();
                                    MyCommon.IsEnding = true;
                                    Close();
                                    return;
                                }

                                if (!startup)
                                {
                                    MessageBox.Show(R.CheckNewVersionText5 + Environment.NewLine + retMsg, string.Format(R.CheckNewVersionText2, MyCommon.AppTitle), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                    }
                    else if (!startup)
                    {
                        MessageBox.Show(R.CheckNewVersionText7 + MyCommon.FileVersion.Replace(".", string.Empty) + R.CheckNewVersionText8 + strVer, string.Format(R.CheckNewVersionText2, MyCommon.AppTitle), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                StatusLabel.Text = R.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(R.CheckNewVersionText10, string.Format(R.CheckNewVersionText2, MyCommon.AppTitle), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Colorize()
        {
            _colorize = false;
            DispSelectedPost();

            // 件数関連の場合、タイトル即時書き換え
            if (_configs.DispLatestPost != DispTitleEnum.None
                && _configs.DispLatestPost != DispTitleEnum.Post
                && _configs.DispLatestPost != DispTitleEnum.Ver
                && _configs.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                SetMainWindowTitle();
            }

            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                SetStatusLabelUrl();
            }

            if (_configs.TabIconDisp)
            {
                ChangeTabsIconToRead();
            }
            else
            {
                ListTab.Refresh();
            }
        }

        private void DispSelectedPost(bool forceupdate = false)
        {
            if (_curList.SelectedIndices.Count == 0 || _curPost == null)
            {
                return;
            }

            if (!forceupdate && _curPost.Equals(_displayPost))
            {
                return;
            }

            _displayPost = _curPost;
            if (_displayItem != null)
            {
                _displayItem.ImageDownloaded -= DisplayItemImage_Downloaded;
                _displayItem = null;
            }

            _displayItem = (ImageListViewItem)_curList.Items[_curList.SelectedIndices[0]];
            _displayItem.ImageDownloaded += DisplayItemImage_Downloaded;

            object tag = null;
            var txt = string.Empty;
            if (!_curPost.IsDm)
            {
                Match mc = Regex.Match(_curPost.SourceHtml, "<a href=\"(?<sourceurl>.+?)\"");
                if (mc.Success)
                {
                    string src = mc.Groups["sourceurl"].Value;
                    SourceLinkLabel.Tag = mc.Groups["sourceurl"].Value;
                    mc = Regex.Match(src, "^https?://");
                    if (!mc.Success)
                    {
                        src = src.Insert(0, "https://twitter.com");
                    }

                    tag = src;
                }

                txt = string.IsNullOrEmpty(_curPost.Source) ? string.Empty : _curPost.Source;
            }

            SourceLinkLabel.Tag = tag;
            SourceLinkLabel.Text = txt;
            SourceLinkLabel.TabStop = false;

            bool isCurTabDm = _statuses.Tabs[_curTab.Text].TabType == TabUsageType.DirectMessage;

            var name = !isCurTabDm ? string.Empty : _curPost.IsOwl ? "DM FROM <- " : "DM TO -> ";
            name += _curPost.ScreenName + "/" + _curPost.Nickname;
            if (_curPost.IsRetweeted)
            {
                name += string.Format(" (RT:{0})", _curPost.RetweetedBy);
            }

            NameLabel.Text = name;
            NameLabel.Tag = _curPost.ScreenName;

            if (!string.IsNullOrEmpty(_curPost.ImageUrl))
            {
                UserPicture.ReplaceImage(_iconDict[_curPost.ImageUrl]);
            }
            else
            {
                UserPicture.ClearImage();
            }

            DateTimeLabel.Text = _curPost.CreatedAt.ToString();

            var foreColor = SystemColors.ControlText;
            if (_curPost.IsOwl && (_configs.OneWayLove || isCurTabDm))
            {
                foreColor = _clrOwl;
            }

            if (_curPost.IsRetweeted)
            {
                foreColor = _clrRetweet;
            }

            if (_curPost.IsFav)
            {
                foreColor = _clrFav;
            }

            NameLabel.ForeColor = foreColor;

            if (DumpPostClassToolStripMenuItem.Checked)
            {
                PostBrowser.Visible = false;
                PostBrowser.DocumentText = _detailHtmlFormatHeader + _curPost.GetDump() + _detailHtmlFormatFooter;
                PostBrowser.Visible = true;
            }
            else
            {
                string detailText = CreateDetailHtml((_curPost.IsDeleted ? "(DELETED)" : string.Empty) + _curPost.Text);
                try
                {
                    if (PostBrowser.DocumentText != detailText)
                    {
                        PostBrowser.Visible = false;
                        PostBrowser.DocumentText = detailText;
                        var lnks = Regex.Matches(detailText, "<a target=\"_self\" href=\"(?<url>http[^\"]+)\"", RegexOptions.IgnoreCase).Cast<Match>()
                            .Select(m => m.Result("${url}")).ToList();
                        _thumbnail.GenThumbnail(_curPost.StatusId, lnks, _curPost.PostGeo, _curPost.Media);
                    }
                }
                catch (COMException comex)
                {
                    // 原因不明
                    Debug.Write(comex);
                }
                catch (UriFormatException)
                {
                    PostBrowser.DocumentText = detailText;
                }
                finally
                {
                    PostBrowser.Visible = true;
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
                if (modifierState == (ModifierState.Ctrl | ModifierState.Shift)
                    || modifierState == ModifierState.Ctrl
                    || modifierState == ModifierState.None
                    || modifierState == ModifierState.Shift)
                {
                    switch (keyCode)
                    {
                        case Keys.J:
                            SendKeys.Send("{DOWN}");
                            return true;
                        case Keys.K:
                            SendKeys.Send("{UP}");
                            return true;
                    }
                }

                if (modifierState == ModifierState.Shift || modifierState == ModifierState.None)
                {
                    switch (keyCode)
                    {
                        case Keys.F:
                            SendKeys.Send("{PGDN}");
                            return true;
                        case Keys.B:
                            SendKeys.Send("{PGUP}");
                            return true;
                    }
                }
            }

            switch (modifierState)
            {
                // 修飾キーなし
                case ModifierState.None:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F1:
                            OpenUriAsync(ApplicationHelpWebPageUrl);
                            return true;
                        case Keys.F3:
                            TrySearchWordInTabToBottom();
                            return true;
                        case Keys.F5:
                            RefreshTab();
                            return true;
                        case Keys.F6:
                            GetTimeline(WorkerType.Reply);
                            return true;
                        case Keys.F7:
                            GetTimeline(WorkerType.DirectMessegeRcv);
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
                                    _anchorFlag = false;
                                }

                                TrySearchAndFocusUnreadTweet();
                                return true;
                            case Keys.G:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    _anchorFlag = false;
                                }

                                AddRelatedStatusesTab();
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
                                GoRelPost(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                GoRelPost(false);
                                return true;
                            case Keys.OemPeriod:
                                GoAnchor();
                                return true;
                            case Keys.I:
                                if (StatusText.Enabled)
                                {
                                    StatusText.Focus();
                                }

                                return true;
                            case Keys.Enter:
                                // case Keys.Return:
                                MakeReplyOrDirectStatus();
                                return true;
                            case Keys.R:
                                RefreshTab();
                                return true;
                        }

                        // 以下、アンカー初期化
                        _anchorFlag = false;
                        switch (keyCode)
                        {
                            case Keys.L:
                                GoSameUsersPost(true);
                                return true;
                            case Keys.H:
                                GoSameUsersPost(false);
                                return true;
                            case Keys.Z:
                            case Keys.Oemcomma:
                                MoveTop();
                                return true;
                            case Keys.S:
                                GoNextTab(true);
                                return true;
                            case Keys.A:
                                GoNextTab(false);
                                return true;
                            case Keys.Oem4:
                                // ] in_reply_to参照元へ戻る
                                GoInReplyToPostTree();
                                return true;
                            case Keys.Oem6:
                                // [ in_reply_toへジャンプ
                                GoBackInReplyToPostTree();
                                return true;
                            case Keys.Escape:
                                if (ListTab.SelectedTab != null)
                                {
                                    TabUsageType tabtype = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == TabUsageType.Related || tabtype == TabUsageType.UserTimeline || tabtype == TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = ListTab.SelectedTab;
                                        RemoveSpecifiedTab(relTp.Text, false);
                                        SaveConfigsTabs();
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
                            MakeReplyOrDirectStatus(false);
                            return true;
                        case Keys.D:
                            DeleteSelected();
                            return true;
                        case Keys.M:
                            MakeReplyOrDirectStatus(false, false);
                            return true;
                        case Keys.S:
                            ChangeSelectedFavStatus(true);
                            return true;
                        case Keys.I:
                            OpenRepliedStatus();
                            return true;
                        case Keys.Q:
                            DoQuote();
                            return true;
                        case Keys.B:
                            ChangeSelectedTweetReadStateToRead();
                            return true;
                        case Keys.T:
                            ShowHashManageBox();
                            return true;
                        case Keys.L:
                            ConvertUrlByAutoSelectedService();
                            return true;
                        case Keys.Y:
                            if (focusedControl != FocusedControl.PostBrowser)
                            {
                                ChangeStatusTextMultilineState(MultiLineMenuItem.Checked);
                                return true;
                            }

                            break;
                        case Keys.F:
                            TrySearchWordInTab();
                            return true;
                        case Keys.U:
                            ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            TryOpenCurListSelectedUserHome();
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            TryOpenCurListSelectedUserFavorites();
                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            TryOpenSelectedTweetWebPage();
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            TryOpenUrlInCurrentTweet();
                            return true;
                    }

                    // フォーカスList
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.Home:
                            case Keys.End:
                                _colorize = true; // スルーする
                                return false;
                            case Keys.N:
                                GoNextTab(true);
                                return true;
                            case Keys.P:
                                GoNextTab(false);
                                return true;
                            case Keys.C:
                                CopyStot();
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
                                if (ListTab.TabPages.Count < tabNo)
                                {
                                    return functionReturnValue;
                                }

                                ListTabSelect(tabNo);
                                return true;
                            case Keys.D9:
                                ListTabSelect(ListTab.TabPages.Count - 1);
                                return true;
                        }
                    }
                    else if (focusedControl == FocusedControl.StatusText)
                    {
                        // フォーカスStatusText
                        switch (keyCode)
                        {
                            case Keys.A:
                                StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!string.IsNullOrEmpty(StatusText.Text.Trim()))
                                {
                                    _postHistory[_postHistoryIndex] = new PostingStatus(StatusText.Text, _replyToId, _replyToName);
                                }

                                if (keyCode == Keys.Up)
                                {
                                    _postHistoryIndex -= 1;
                                    if (_postHistoryIndex < 0)
                                    {
                                        _postHistoryIndex = 0;
                                    }
                                }
                                else
                                {
                                    _postHistoryIndex += 1;
                                    if (_postHistoryIndex > _postHistory.Count - 1)
                                    {
                                        _postHistoryIndex = _postHistory.Count - 1;
                                    }
                                }

                                StatusText.Text = _postHistory[_postHistoryIndex].Status;
                                _replyToId = _postHistory[_postHistoryIndex].InReplyToId;
                                _replyToName = _postHistory[_postHistoryIndex].InReplyToName;
                                StatusText.SelectionStart = StatusText.Text.Length;
                                return true;
                            case Keys.PageUp:
                            case Keys.P:
                                if (ListTab.SelectedIndex == 0)
                                {
                                    ListTab.SelectedIndex = ListTab.TabCount - 1;
                                }
                                else
                                {
                                    ListTab.SelectedIndex -= 1;
                                }

                                StatusText.Focus();
                                return true;
                            case Keys.PageDown:
                            case Keys.N:
                                if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                                {
                                    ListTab.SelectedIndex = 0;
                                }
                                else
                                {
                                    ListTab.SelectedIndex += 1;
                                }

                                StatusText.Focus();
                                return true;
                        }
                    }
                    else
                    {
                        // フォーカスPostBrowserもしくは関係なし
                        switch (keyCode)
                        {
                            case Keys.A:
                                PostBrowser.Document.ExecCommand("SelectAll", false, null);
                                return true;
                            case Keys.C:
                            case Keys.Insert:
                                string selText = WebBrowser_GetSelectionText(PostBrowser);
                                if (!string.IsNullOrEmpty(selText))
                                {
                                    CopyToClipboard(selText);
                                }

                                return true;
                            case Keys.Y:
                                MultiLineMenuItem.Checked = !MultiLineMenuItem.Checked;
                                ChangeStatusTextMultilineState(MultiLineMenuItem.Checked);
                                return true;
                        }
                    }

                    break;
                case ModifierState.Shift:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F3:
                            TrySearchWordInTabToTop();
                            return true;
                        case Keys.F5:
                            RefreshTab(true);
                            return true;
                        case Keys.F6:
                            GetTimeline(WorkerType.Reply, -1);
                            return true;
                        case Keys.F7:
                            GetTimeline(WorkerType.DirectMessegeRcv);
                            return true;
                    }

                    // フォーカスStatusText以外
                    if (focusedControl != FocusedControl.StatusText)
                    {
                        if (keyCode == Keys.R)
                        {
                            RefreshTab(true);
                            return true;
                        }
                    }

                    // フォーカスリスト
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.H:
                                GoTopEnd(true);
                                return true;
                            case Keys.L:
                                GoTopEnd(false);
                                return true;
                            case Keys.M:
                                GoMiddle();
                                return true;
                            case Keys.G:
                                GoLast();
                                return true;
                            case Keys.Z:
                                MoveMiddle();
                                return true;
                            case Keys.Oem4:
                                GoBackInReplyToPostTree(true, false);
                                return true;
                            case Keys.Oem6:
                                GoBackInReplyToPostTree(true);
                                return true;
                            case Keys.N:
                            case Keys.Right:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(false);
                                return true;
                            case Keys.Space:
                                GoBackSelectPostChain();
                                return true;
                        }
                    }

                    break;
                case ModifierState.Alt:
                    switch (keyCode)
                    {
                        case Keys.R:
                            DoReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (_curPost != null)
                            {
                                ShowUserStatus(_curPost.ScreenName, false);
                                return true;
                            }

                            break;
                        case Keys.Up:
                            ScrollDownPostBrowser(false);
                            return true;
                        case Keys.Down:
                            ScrollDownPostBrowser(true);
                            return true;
                        case Keys.PageUp:
                            PageDownPostBrowser(false);
                            return true;
                        case Keys.PageDown:
                            PageDownPostBrowser(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // 別タブの同じ書き込みへ(ALT+←/→)
                        if (keyCode == Keys.Right)
                        {
                            GoSamePostToAnotherTab(false);
                            return true;
                        }

                        if (keyCode == Keys.Left)
                        {
                            GoSamePostToAnotherTab(true);
                            return true;
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Shift:
                    switch (keyCode)
                    {
                        case Keys.R:
                            MakeReplyOrDirectStatus(false, true, true);
                            return true;
                        case Keys.C:
                            CopyIdUri();
                            return true;
                        case Keys.F:
                            if (ListTab.SelectedTab != null)
                            {
                                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                                {
                                    ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                                    return true;
                                }
                            }

                            break;
                        case Keys.S:
                            ChangeSelectedFavStatus(false);
                            return true;
                        case Keys.B:
                            ChangeSelectedTweetReadSateToUnread();
                            return true;
                        case Keys.T:
                            ChangeUseHashTagSetting();
                            return true;
                        case Keys.P:
                            ToggleImageSelectorView();
                            return true;
                        case Keys.H:
                            TryOpenSelectedRtUserHome();
                            return true;
                        case Keys.O:
                            OpenFavorarePageOfSelectedTweetUser();
                            return true;
                    }

                    if (focusedControl == FocusedControl.StatusText)
                    {
                        switch (keyCode)
                        {
                            case Keys.Up:
                                {
                                    int idx = 0;
                                    if (_curList != null && _curList.Items.Count != 0 && _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] > 0)
                                    {
                                        idx = _curList.SelectedIndices[0] - 1;
                                        SelectListItem(_curList, idx);
                                        _curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Down:
                                {
                                    int idx = 0;
                                    if (_curList != null && _curList.Items.Count != 0 && _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] < _curList.Items.Count - 1)
                                    {
                                        idx = _curList.SelectedIndices[0] + 1;
                                        SelectListItem(_curList, idx);
                                        _curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Space:
                                if (StatusText.SelectionStart > 0)
                                {
                                    int endidx = StatusText.SelectionStart - 1;
                                    string startstr = string.Empty;
                                    bool pressed = false;
                                    for (int i = StatusText.SelectionStart - 1; i >= 0; i--)
                                    {
                                        char c = StatusText.Text[i];
                                        if (char.IsLetterOrDigit(c) || c == '_')
                                        {
                                            continue;
                                        }

                                        if (c == '@')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            int cnt = AtIdSupl.ItemCount;
                                            ShowSuplDialog(StatusText, AtIdSupl, startstr.Length + 1, startstr);
                                            if (AtIdSupl.ItemCount != cnt)
                                            {
                                                _modifySettingAtId = true;
                                            }
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            ShowSuplDialog(StatusText, HashSupl, startstr.Length + 1, startstr);
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
                                    DetailsListView lst = (DetailsListView)ListTab.SelectedTab.Tag;
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
                                    DetailsListView lst = (DetailsListView)ListTab.SelectedTab.Tag;
                                    var col = lst.Columns.Cast<ColumnHeader>().OrderByDescending(x => x.DisplayIndex).First();
                                    MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                    return true;
                                }
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Alt:
                    switch (keyCode)
                    {
                        case Keys.S:
                            FavoritesRetweetOriginal();
                            return true;
                        case Keys.R:
                            FavoritesRetweetUnofficial();
                            return true;
                        case Keys.H:
                            OpenUserAppointUrl();
                            return true;
                    }

                    break;
                case ModifierState.Alt | ModifierState.Shift:
                    if (focusedControl == FocusedControl.PostBrowser)
                    {
                        if (keyCode == Keys.R)
                        {
                            DoReTweetUnofficial();
                        }
                        else if (keyCode == Keys.C)
                        {
                            CopyUserId();
                        }

                        return true;
                    }

                    switch (keyCode)
                    {
                        case Keys.T:
                            if (!ExistCurrentPost)
                            {
                                return functionReturnValue;
                            }

                            DoTranslation(_curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            DoReTweetUnofficial();
                            return true;
                        case Keys.C:
                            CopyUserId();
                            return true;
                        case Keys.Up:
                            _thumbnail.ScrollThumbnail(false);
                            return true;
                        case Keys.Down:
                            _thumbnail.ScrollThumbnail(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab && keyCode == Keys.Enter)
                    {
                        if (!SplitContainer3.Panel2Collapsed)
                        {
                            _thumbnail.OpenPicture();
                        }

                        return true;
                    }

                    break;
            }

            return functionReturnValue;
        }

        private void ScrollPostBrowser(int delta)
        {
            var doc = PostBrowser.Document;
            if (doc == null || doc.Body == null)
            {
                return;
            }

            doc.Body.ScrollTop += delta;
        }

        private void ScrollDownPostBrowser(bool forward)
        {
            int delta = _configs.FontDetail.Height;
            ScrollPostBrowser(forward ? delta : -delta);
        }

        private void PageDownPostBrowser(bool forward)
        {
            int delta = PostBrowser.ClientRectangle.Height - _configs.FontDetail.Height;
            ScrollPostBrowser(forward ? delta : -delta);
        }

        private void GoNextTab(bool forward)
        {
            int idx = ListTab.SelectedIndex;
            if (forward)
            {
                idx += 1;
                if (idx > ListTab.TabPages.Count - 1)
                {
                    idx = 0;
                }
            }
            else
            {
                idx -= 1;
                if (idx < 0)
                {
                    idx = ListTab.TabPages.Count - 1;
                }
            }

            ListTabSelect(idx);
        }

        private void CopyStot()
        {
            bool isDm = false;
            if (_curTab != null && _statuses.GetTabByName(_curTab.Text) != null)
            {
                isDm = _statuses.GetTabByName(_curTab.Text).TabType == TabUsageType.DirectMessage;
            }

            StringBuilder sb = new StringBuilder();
            bool isProtected = false;
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
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
                    sb.AppendFormat("{0}:{1} [https://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.OriginalStatusId, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}:{1} [{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine);
                }
            }

            if (isProtected)
            {
                new MessageForm().ShowDialog(R.CopyStotText1);
            }

            if (sb.Length > 0)
            {
                CopyToClipboard(sb.ToString());
            }
        }

        private void CopyIdUri()
        {
            if (_curTab == null
                || _statuses.GetTabByName(_curTab.Text) == null
                || _statuses.GetTabByName(_curTab.Text).TabType == TabUsageType.DirectMessage
                || _curList.SelectedIndices.Count < 1)
            {
                return;
            }

            var sb = string.Join(Environment.NewLine, _curList.SelectedIndices.Cast<int>().Select(i => _statuses.Item(_curTab.Text, i)).Select(p => p.MakeStatusUrl()));
            if (sb.Length > 0)
            {
                CopyToClipboard(sb);
            }
        }

        private void GoFav(bool forward)
        {
            if (_curList.VirtualListSize == 0)
            {
                return;
            }

            int toIndex = forward ? _curList.VirtualListSize - 1 : 0;
            int fromIndex = forward ? 0 : _curList.VirtualListSize - 1;
            if (_curList.SelectedIndices.Count != 0)
            {
                fromIndex = forward ?
                    _curList.SelectedIndices[0] + 1 :
                    _curList.SelectedIndices[0] - 1;
            }

            if (forward)
            {
                if (fromIndex > toIndex)
                {
                    return;
                }
            }
            else
            {
                if (fromIndex < toIndex)
                {
                    return;
                }
            }

            var idxs = forward ?
                Enumerable.Range(fromIndex, toIndex - fromIndex + 1) :
                Enumerable.Range(toIndex + 1, fromIndex - toIndex).Reverse();
            foreach (int idx in idxs)
            {
                if (_statuses.Item(_curTab.Text, idx).IsFav)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (_curList.VirtualListSize == 0
                || _statuses.Tabs[_curTab.Text].TabType == TabUsageType.DirectMessage
                || _curList.SelectedIndices.Count == 0)
            {
                // Directタブは対象外（見つかるはずがない）// 未選択も処理しない
                return;
            }

            long targetId = GetCurTabPost(_curList.SelectedIndices[0]).StatusId;
            var tabIdxs = left ?
                Enumerable.Range(0, ListTab.SelectedIndex).Reverse() :
                Enumerable.Range(ListTab.SelectedIndex + 1, ListTab.TabCount - ListTab.SelectedIndex);

            bool found = false;
            foreach (int tabidx in tabIdxs)
            {
                TabPage tab = ListTab.TabPages[tabidx];
                if (_statuses.Tabs[tab.Text].TabType == TabUsageType.DirectMessage)
                {
                    // Directタブは対象外
                    continue;
                }

                for (int idx = 0; idx < ((DetailsListView)tab.Tag).VirtualListSize; ++idx)
                {
                    if (_statuses.Item(tab.Text, idx).StatusId == targetId)
                    {
                        ListTabSelect(tabidx);
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
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

        private void GoSameUsersPost(bool forward)
        {
            if (_curList.SelectedIndices.Count == 0 || _curPost == null)
            {
                return;
            }

            int selected = _curList.SelectedIndices[0];
            int toIdx = forward ? _curList.VirtualListSize - 1 : 0;
            int fIdx = forward ? selected + 1 : selected - 1;
            if (forward && fIdx > toIdx)
            {
                return;
            }

            if (!forward && toIdx > fIdx)
            {
                return;
            }

            var idxs = forward ?
                Enumerable.Range(fIdx, toIdx - fIdx + 1) :
                Enumerable.Range(toIdx + 1, fIdx - toIdx).Reverse();
            string name = _curPost.IsRetweeted ? _curPost.RetweetedBy : _curPost.ScreenName;
            foreach (int idx in idxs)
            {
                var statusesItem = _statuses.Item(_curTab.Text, idx);
                var statusItemName = statusesItem.IsRetweeted ? statusesItem.RetweetedBy : statusesItem.ScreenName;
                if (statusItemName == name)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            if (!_anchorFlag)
            {
                if (_curPost == null)
                {
                    return;
                }

                _anchorPost = _curPost;
                _anchorFlag = true;
            }
            else
            {
                if (_anchorPost == null)
                {
                    return;
                }
            }

            int selected = _curList.SelectedIndices[0];
            int toIdx = forward ? _curList.VirtualListSize - 1 : 0;
            int fIdx = forward ? selected + 1 : selected - 1;
            if (forward && fIdx > toIdx)
            {
                return;
            }

            if (!forward && toIdx > fIdx)
            {
                return;
            }

            var idxs = forward ?
                Enumerable.Range(fIdx, toIdx - fIdx + 1) :
                Enumerable.Range(toIdx + 1, fIdx - toIdx).Reverse();

            foreach (int idx in idxs)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
                if (post.ScreenName == _anchorPost.ScreenName
                    || post.RetweetedBy == _anchorPost.ScreenName
                    || post.ScreenName == _anchorPost.RetweetedBy
                    || (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == _anchorPost.RetweetedBy)
                    || _anchorPost.ReplyToList.Contains(post.ScreenName.ToLower())
                    || _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower())
                    || post.ReplyToList.Contains(_anchorPost.ScreenName.ToLower())
                    || post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLower()))
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            if (_anchorPost == null)
            {
                return;
            }

            int idx = _statuses.Tabs[_curTab.Text].IndexOf(_anchorPost.StatusId);
            if (idx == -1)
            {
                return;
            }

            SelectListItem(_curList, idx);
            _curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool top)
        {
            ListViewItem item = null;
            int idx = 0;
            if (top)
            {
                item = _curList.GetItemAt(0, 25);
                idx = item != null ? item.Index : 0;
            }
            else
            {
                item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
                idx = item != null ? item.Index : _curList.VirtualListSize - 1;
            }

            SelectListItem(_curList, idx);
        }

        private void GoMiddle()
        {
            ListViewItem item = _curList.GetItemAt(0, 0);
            int idx1 = item != null ? item.Index : 0;
            item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            int idx2 = item != null ? item.Index : _curList.VirtualListSize - 1;
            SelectListItem(_curList, (idx1 + idx2) / 2);
        }

        private void GoLast()
        {
            if (_curList.VirtualListSize == 0)
            {
                return;
            }

            var idx = (_statuses.SortOrder == SortOrder.Ascending) ?
                _curList.VirtualListSize - 1 : 0;
            SelectListItem(_curList, idx);
            _curList.EnsureVisible(idx);
        }

        private void MoveTop()
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int selected = _curList.SelectedIndices[0];
            var idx = (_statuses.SortOrder == SortOrder.Ascending) ?
                _curList.VirtualListSize - 1 : 0;
            _curList.EnsureVisible(idx);
            _curList.EnsureVisible(selected);
        }

        private void GoInReplyToPostTree()
        {
            if (_curPost == null)
            {
                return;
            }

            TabClass curTabClass = _statuses.Tabs[_curTab.Text];
            if (curTabClass.TabType == TabUsageType.PublicSearch && _curPost.InReplyToStatusId == 0 && _curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = _tw.GetStatusApi(false, _curPost.StatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    _curPost.InReplyToStatusId = post.InReplyToStatusId;
                    _curPost.InReplyToUser = post.InReplyToUser;
                    _curPost.IsReply = post.IsReply;
                    _itemCache = null;
                    _curList.RedrawItems(_curItemIndex, _curItemIndex, false);
                }
                else
                {
                    StatusLabel.Text = r;
                }
            }

            if (!(ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId > 0))
            {
                return;
            }

            if (_replyChains == null || (_replyChains.Count > 0 && _replyChains.Peek().InReplyToId != _curPost.StatusId))
            {
                _replyChains = new Stack<ReplyChain>();
            }

            _replyChains.Push(new ReplyChain(_curPost.StatusId, _curPost.InReplyToStatusId, _curTab));

            Dictionary<long, PostClass> curTabPosts = _statuses.Tabs[_curTab.Text].IsInnerStorageTabType ? curTabClass.Posts : _statuses.Posts;
            long inReplyToId = _curPost.InReplyToStatusId;
            var inReplyToPosts = from tab in _statuses.Tabs.Values
                                 orderby !ReferenceEquals(tab, curTabClass)
                                 from post in (tab.IsInnerStorageTabType ? tab.Posts : _statuses.Posts).Values
                                 where post.StatusId == inReplyToId
                                 let index = tab.IndexOf(post.StatusId)
                                 where index != -1
                                 select new { Tab = tab, Index = index };

            int inReplyToIndex = 0;
            string inReplyToTabName = null;
            try
            {
                var inReplyPost = inReplyToPosts.First();
                inReplyToTabName = inReplyPost.Tab.TabName;
                inReplyToIndex = inReplyPost.Index;
            }
            catch (InvalidOperationException)
            {
                PostClass post = null;
                string r = _tw.GetStatusApi(false, _curPost.InReplyToStatusId, ref post);
                string inReplyToUser = _curPost.InReplyToUser;
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    post.IsRead = true;
                    _statuses.AddPost(post);
                    _statuses.DistributePosts();
                    RefreshTimeline(false);
                    try
                    {
                        var inReplyPost = inReplyToPosts.First();
                        inReplyToTabName = inReplyPost.Tab.TabName;
                        inReplyToIndex = inReplyPost.Index;
                    }
                    catch (InvalidOperationException)
                    {
                        OpenUriAsync(string.Format("https://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                        return;
                    }
                }
                else
                {
                    StatusLabel.Text = r;
                    OpenUriAsync(string.Format("https://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                    return;
                }
            }

            var tabPage = ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == inReplyToTabName);
            var listView = (DetailsListView)tabPage.Tag;
            if (!ReferenceEquals(_curTab, tabPage))
            {
                ListTab.SelectTab(tabPage);
            }

            SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (_curPost == null)
            {
                return;
            }

            var curTabClass = _statuses.Tabs[_curTab.Text];
            var curTabPosts = curTabClass.IsInnerStorageTabType ? curTabClass.Posts : _statuses.Posts;
            if (parallel)
            {
                if (_curPost.InReplyToStatusId == 0)
                {
                    return;
                }

                var posts = from t in _statuses.Tabs
                            from p in t.Value.IsInnerStorageTabType ? t.Value.Posts : _statuses.Posts
                            where p.Value.StatusId != _curPost.StatusId && p.Value.InReplyToStatusId == _curPost.InReplyToStatusId
                            let indexOf = t.Value.IndexOf(p.Value.StatusId)
                            where indexOf > -1
                            orderby (isForward ? indexOf : indexOf * -1)
                            orderby !ReferenceEquals(t.Value, curTabClass)
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

                    var post = postList.FirstOrDefault(pst => ReferenceEquals(pst.Tab, curTabClass) && (isForward ? pst.Index > _curItemIndex : pst.Index < _curItemIndex));
                    if (post == null)
                    {
                        post = postList.FirstOrDefault(pst => !ReferenceEquals(pst.Tab, curTabClass));
                    }

                    if (post == null)
                    {
                        post = postList.First();
                    }

                    ListTab.SelectTab(ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                    var listView = (DetailsListView)ListTab.SelectedTab.Tag;
                    SelectListItem(listView, post.Index);
                    listView.EnsureVisible(post.Index);
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            if (_replyChains == null || _replyChains.Count < 1)
            {
                var posts = from t in _statuses.Tabs
                            from p in t.Value.IsInnerStorageTabType ? t.Value.Posts : _statuses.Posts
                            where p.Value.InReplyToStatusId == _curPost.StatusId
                            let indexOf = t.Value.IndexOf(p.Value.StatusId)
                            where indexOf > -1
                            orderby indexOf
                            orderby !ReferenceEquals(t.Value, curTabClass)
                            select new { Tab = t.Value, Index = indexOf };
                try
                {
                    var post = posts.First();
                    ListTab.SelectTab(ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                    var listView = (DetailsListView)ListTab.SelectedTab.Tag;
                    SelectListItem(listView, post.Index);
                    listView.EnsureVisible(post.Index);
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            ReplyChain chainHead = _replyChains.Pop();
            if (chainHead.InReplyToId == _curPost.StatusId)
            {
                int idx = _statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                if (idx == -1)
                {
                    _replyChains = null;
                }
                else
                {
                    try
                    {
                        ListTab.SelectTab(chainHead.OriginalTab);
                    }
                    catch (Exception)
                    {
                        _replyChains = null;
                    }

                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                }
            }
            else
            {
                _replyChains = null;
                GoBackInReplyToPostTree(parallel);
            }
        }

        private void GoBackSelectPostChain()
        {
            try
            {
                _selectPostChains.Pop();
                var tabPostPair = _selectPostChains.Pop();
                if (!ListTab.TabPages.Contains(tabPostPair.Item1))
                {
                    return;
                }

                ListTab.SelectedTab = tabPostPair.Item1;
                if (tabPostPair.Item2 != null)
                {
                    var idx = _statuses.Tabs[_curTab.Text].IndexOf(tabPostPair.Item2.StatusId);
                    if (idx > -1)
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void PushSelectPostChain()
        {
            if (_selectPostChains.Count == 0
                || (_selectPostChains.Peek().Item1.Text != _curTab.Text
                || !ReferenceEquals(_curPost, _selectPostChains.Peek().Item2)))
            {
                _selectPostChains.Push(Tuple.Create(_curTab, _curPost));
            }
        }

        private void TrimPostChain()
        {
            int chainLimit = 2000;
            if (_selectPostChains.Count < chainLimit)
            {
                return;
            }

            var p = new Stack<Tuple<TabPage, PostClass>>();
            for (var i = 0; i < chainLimit; i++)
            {
                p.Push(_selectPostChains.Pop());
            }

            _selectPostChains.Clear();
            for (var i = 0; i < chainLimit; i++)
            {
                _selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }

            for (int tabidx = 0; tabidx < ListTab.TabCount; ++tabidx)
            {
                var tab = _statuses.Tabs[ListTab.TabPages[tabidx].Text];
                if (tab.TabType != TabUsageType.DirectMessage && tab.Contains(statusId))
                {
                    var idx = tab.IndexOf(statusId);
                    ListTabSelect(tabidx);
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
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

            for (int tabidx = 0; tabidx < ListTab.TabCount; ++tabidx)
            {
                var tab = _statuses.Tabs[ListTab.TabPages[tabidx].Text];
                if (tab.TabType == TabUsageType.DirectMessage && tab.Contains(statusId))
                {
                    var idx = tab.IndexOf(statusId);
                    ListTabSelect(tabidx);
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    return true;
                }
            }

            return false;
        }

        private void SaveConfigsAll(bool ifModified)
        {
            if (!ifModified)
            {
                SaveConfigsCommon();
                SaveConfigsLocal();
                SaveConfigsTabs();
                SaveConfigsAtId();
            }
            else
            {
                if (_modifySettingCommon)
                {
                    SaveConfigsCommon();
                }

                if (_modifySettingLocal)
                {
                    SaveConfigsLocal();
                }

                if (_modifySettingAtId)
                {
                    SaveConfigsAtId();
                }
            }
        }

        private void SaveConfigsAtId()
        {
            if (_ignoreConfigSave || (!_configs.UseAtIdSupplement && AtIdSupl == null))
            {
                return;
            }

            _modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (_ignoreConfigSave)
            {
                return;
            }

            _modifySettingCommon = false;
            lock (_syncObject)
            {
                _cfgCommon.UserName = _tw.Username;
                _cfgCommon.UserId = _tw.UserId;
                _cfgCommon.Password = _tw.Password;
                _cfgCommon.Token = _tw.AccessToken;
                _cfgCommon.TokenSecret = _tw.AccessTokenSecret;
                _cfgCommon.UserAccounts = _configs.UserAccounts;
                _cfgCommon.UserstreamStartup = _configs.UserstreamStartup;
                _cfgCommon.UserstreamPeriod = _configs.UserstreamPeriodInt;
                _cfgCommon.TimelinePeriod = _configs.TimelinePeriodInt;
                _cfgCommon.ReplyPeriod = _configs.ReplyPeriodInt;
                _cfgCommon.DMPeriod = _configs.DMPeriodInt;
                _cfgCommon.PubSearchPeriod = _configs.PubSearchPeriodInt;
                _cfgCommon.ListsPeriod = _configs.ListsPeriodInt;
                _cfgCommon.UserTimelinePeriod = _configs.UserTimelinePeriodInt;
                _cfgCommon.Read = _configs.Readed;
                _cfgCommon.IconSize = _configs.IconSz;
                _cfgCommon.UnreadManage = _configs.UnreadManage;
                _cfgCommon.PlaySound = _configs.PlaySound;
                _cfgCommon.OneWayLove = _configs.OneWayLove;
                _cfgCommon.NameBalloon = _configs.NameBalloon;
                _cfgCommon.PostCtrlEnter = _configs.PostCtrlEnter;
                _cfgCommon.PostShiftEnter = _configs.PostShiftEnter;
                _cfgCommon.CountApi = _configs.CountApi;
                _cfgCommon.CountApiReply = _configs.CountApiReply;
                _cfgCommon.PostAndGet = _configs.PostAndGet;
                _cfgCommon.DispUsername = _configs.DispUsername;
                _cfgCommon.MinimizeToTray = _configs.MinimizeToTray;
                _cfgCommon.CloseToExit = _configs.CloseToExit;
                _cfgCommon.DispLatestPost = _configs.DispLatestPost;
                _cfgCommon.SortOrderLock = _configs.SortOrderLock;
                _cfgCommon.TinyUrlResolve = _configs.TinyUrlResolve;
                _cfgCommon.ShortUrlForceResolve = _configs.ShortUrlForceResolve;
                _cfgCommon.PeriodAdjust = _configs.PeriodAdjust;
                _cfgCommon.StartupVersion = _configs.StartupVersion;
                _cfgCommon.StartupFollowers = _configs.StartupFollowers;
                _cfgCommon.RestrictFavCheck = _configs.RestrictFavCheck;
                _cfgCommon.AlwaysTop = _configs.AlwaysTop;
                _cfgCommon.UrlConvertAuto = _configs.UrlConvertAuto;
                _cfgCommon.Outputz = _configs.OutputzEnabled;
                _cfgCommon.OutputzKey = _configs.OutputzKey;
                _cfgCommon.OutputzUrlMode = _configs.OutputzUrlmode;
                _cfgCommon.UseUnreadStyle = _configs.UseUnreadStyle;
                _cfgCommon.DateTimeFormat = _configs.DateTimeFormat;
                _cfgCommon.DefaultTimeOut = _configs.DefaultTimeOut;
                _cfgCommon.RetweetNoConfirm = _configs.RetweetNoConfirm;
                _cfgCommon.LimitBalloon = _configs.LimitBalloon;
                _cfgCommon.EventNotifyEnabled = _configs.EventNotifyEnabled;
                _cfgCommon.EventNotifyFlag = _configs.EventNotifyFlag;
                _cfgCommon.IsMyEventNotifyFlag = _configs.IsMyEventNotifyFlag;
                _cfgCommon.ForceEventNotify = _configs.ForceEventNotify;
                _cfgCommon.FavEventUnread = _configs.FavEventUnread;
                _cfgCommon.TranslateLanguage = _configs.TranslateLanguage;
                _cfgCommon.EventSoundFile = _configs.EventSoundFile;
                _cfgCommon.AutoShortUrlFirst = _configs.AutoShortUrlFirst;
                _cfgCommon.TabIconDisp = _configs.TabIconDisp;
                _cfgCommon.ReplyIconState = _configs.ReplyIconState;
                _cfgCommon.ReadOwnPost = _configs.ReadOwnPost;
                _cfgCommon.GetFav = _configs.GetFav;
                _cfgCommon.IsMonospace = _configs.IsMonospace;
                if (IdeographicSpaceToSpaceToolStripMenuItem != null && !IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed)
                {
                    _cfgCommon.WideSpaceConvert = IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }

                _cfgCommon.ReadOldPosts = _configs.ReadOldPosts;
                _cfgCommon.UseSsl = _configs.UseSsl;
                _cfgCommon.BilyUser = _configs.BitlyUser;
                _cfgCommon.BitlyPwd = _configs.BitlyPwd;
                _cfgCommon.ShowGrid = _configs.ShowGrid;
                _cfgCommon.UseAtIdSupplement = _configs.UseAtIdSupplement;
                _cfgCommon.UseHashSupplement = _configs.UseHashSupplement;
                _cfgCommon.PreviewEnable = _configs.PreviewEnable;
                _cfgCommon.Language = _configs.Language;
                _cfgCommon.SortOrder = (int)_statuses.SortOrder;
                _cfgCommon.SortColumn = GetSortColumnIndex(_statuses.SortMode);
                _cfgCommon.Nicoms = _configs.Nicoms;
                _cfgCommon.HashTags = HashMgr.HashHistories;
                _cfgCommon.HashSelected = HashMgr.IsPermanent ? HashMgr.UseHash : string.Empty;
                _cfgCommon.HashIsHead = HashMgr.IsHead;
                _cfgCommon.HashIsPermanent = HashMgr.IsPermanent;
                _cfgCommon.HashIsNotAddToAtReply = HashMgr.IsNotAddToAtReply;
                _cfgCommon.TwitterUrl = _configs.TwitterApiUrl;
                _cfgCommon.TwitterSearchUrl = _configs.TwitterSearchApiUrl;
                _cfgCommon.HotkeyEnabled = _configs.HotkeyEnabled;
                _cfgCommon.HotkeyModifier = _configs.HotkeyMod;
                _cfgCommon.HotkeyKey = _configs.HotkeyKey;
                _cfgCommon.HotkeyValue = _configs.HotkeyValue;
                _cfgCommon.BlinkNewMentions = _configs.BlinkNewMentions;
                if (ToolStripFocusLockMenuItem != null && !ToolStripFocusLockMenuItem.IsDisposed)
                {
                    _cfgCommon.FocusLockToStatusText = ToolStripFocusLockMenuItem.Checked;
                }

                _cfgCommon.UseAdditionalCount = _configs.UseAdditionalCount;
                _cfgCommon.MoreCountApi = _configs.MoreCountApi;
                _cfgCommon.FirstCountApi = _configs.FirstCountApi;
                _cfgCommon.SearchCountApi = _configs.SearchCountApi;
                _cfgCommon.FavoritesCountApi = _configs.FavoritesCountApi;
                _cfgCommon.UserTimelineCountApi = _configs.UserTimelineCountApi;
                _cfgCommon.TrackWord = _tw.TrackWord;
                _cfgCommon.AllAtReply = _tw.AllAtReply;
                _cfgCommon.OpenUserTimeline = _configs.OpenUserTimeline;
                _cfgCommon.ListCountApi = _configs.ListCountApi;
                _cfgCommon.UseImageService = ImageServiceCombo.SelectedIndex;
                _cfgCommon.ListDoubleClickAction = _configs.ListDoubleClickAction;
                _cfgCommon.UserAppointUrl = _configs.UserAppointUrl;
                _cfgCommon.HideDuplicatedRetweets = _configs.HideDuplicatedRetweets;
                _cfgCommon.IsPreviewFoursquare = _configs.IsPreviewFoursquare;
                _cfgCommon.FoursquarePreviewHeight = _configs.FoursquarePreviewHeight;
                _cfgCommon.FoursquarePreviewWidth = _configs.FoursquarePreviewWidth;
                _cfgCommon.FoursquarePreviewZoom = _configs.FoursquarePreviewZoom;
                _cfgCommon.IsListsIncludeRts = _configs.IsListStatusesIncludeRts;
                _cfgCommon.TabMouseLock = _configs.TabMouseLock;
                _cfgCommon.IsRemoveSameEvent = _configs.IsRemoveSameEvent;
                _cfgCommon.IsUseNotifyGrowl = _configs.IsNotifyUseGrowl;

                _cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (_ignoreConfigSave)
            {
                return;
            }

            lock (_syncObject)
            {
                _modifySettingLocal = false;
                _cfgLocal.FormSize = _mySize;
                _cfgLocal.FormLocation = _myLoc;
                _cfgLocal.SplitterDistance = _mySpDis;
                _cfgLocal.PreviewDistance = _mySpDis3;
                _cfgLocal.StatusMultiline = StatusText.Multiline;
                _cfgLocal.StatusTextHeight = _mySpDis2;
                _cfgLocal.AdSplitterDistance = _myAdSpDis;
                _cfgLocal.StatusText = _configs.Status;
                _cfgLocal.FontUnread = _fntUnread;
                _cfgLocal.ColorUnread = _clrUnread;
                _cfgLocal.FontRead = _fntReaded;
                _cfgLocal.ColorRead = _clrRead;
                _cfgLocal.FontDetail = _fntDetail;
                _cfgLocal.ColorDetail = _clrDetail;
                _cfgLocal.ColorDetailBackcolor = _clrDetailBackcolor;
                _cfgLocal.ColorDetailLink = _clrDetailLink;
                _cfgLocal.ColorFav = _clrFav;
                _cfgLocal.ColorOWL = _clrOwl;
                _cfgLocal.ColorRetweet = _clrRetweet;
                _cfgLocal.ColorSelf = _clrSelf;
                _cfgLocal.ColorAtSelf = _clrAtSelf;
                _cfgLocal.ColorTarget = _clrTarget;
                _cfgLocal.ColorAtTarget = _clrAtTarget;
                _cfgLocal.ColorAtFromTarget = _clrAtFromTarget;
                _cfgLocal.ColorAtTo = _clrAtTo;
                _cfgLocal.ColorListBackcolor = _clrListBackcolor;
                _cfgLocal.ColorInputBackcolor = InputBackColor;
                _cfgLocal.ColorInputFont = _clrInputForecolor;
                _cfgLocal.FontInputFont = _fntInputFont;
                _cfgLocal.BrowserPath = _configs.BrowserPath;
                _cfgLocal.UseRecommendStatus = _configs.UseRecommendStatus;
                _cfgLocal.ProxyType = _configs.SelectedProxyType;
                _cfgLocal.ProxyAddress = _configs.ProxyAddress;
                _cfgLocal.ProxyPort = _configs.ProxyPort;
                _cfgLocal.ProxyUser = _configs.ProxyUser;
                _cfgLocal.ProxyPassword = _configs.ProxyPassword;
                if (_ignoreConfigSave)
                {
                    return;
                }

                _cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            var nonrel = ListTab.TabPages.Cast<TabPage>()
                .Select(tp => tp.Text)
                .Select(tp => _statuses.Tabs[tp])
                .Where(tab => tab.TabType != TabUsageType.Related);
            var settings = new SettingTabs();
            settings.Tabs.AddRange(nonrel);
            settings.Save();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isAuto">True=先頭に挿入、False=カーソル位置に挿入</param>
        /// <param name="isReply">True=@,False=DM</param>
        /// <param name="isAll"></param>
        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            if (!StatusText.Enabled || _curList == null || _curTab == null || !ExistCurrentPost)
            {
                return;
            }

            // アイテムが選択されてない
            if (_curList.SelectedIndices.Count < 1)
            {
                return;
            }

            // 単独ユーザー宛リプライまたはDM
            if (_curList.SelectedIndices.Count == 1 && !isAll && ExistCurrentPost)
            {
                if ((_statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                {
                    // ダイレクトメッセージ
                    StatusText.Text = "D " + _curPost.ScreenName + " " + StatusText.Text;
                    StatusText.SelectionStart = StatusText.Text.Length;
                    StatusText.Focus();
                    ClearReplyToInfo();
                    return;
                }

                if (string.IsNullOrEmpty(StatusText.Text))
                {
                    // 空の場合 : ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                    StatusText.Text = "@" + _curPost.ScreenName + " ";
                    _replyToId = _curPost.OriginalStatusId;
                    _replyToName = _curPost.ScreenName;
                }
                else
                {
                    // 何か入力済の場合
                    if (isAuto)
                    {
                        // 1件選んでEnter or DoubleClick
                        if (StatusText.Text.Contains("@" + _curPost.ScreenName + " "))
                        {
                            if (_replyToId > 0 && _replyToName == _curPost.ScreenName)
                            {
                                // 返信先書き換え
                                _replyToId = _curPost.OriginalStatusId;
                                _replyToName = _curPost.ScreenName;
                            }

                            return;
                        }

                        if (!StatusText.Text.StartsWith("@"))
                        {
                            // 文頭＠以外
                            if (StatusText.Text.StartsWith(". "))
                            {
                                // 複数リプライ
                                StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.ScreenName + " ");
                                ClearReplyToInfo();
                            }
                            else
                            {
                                // 単独リプライ
                                StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                _replyToId = _curPost.OriginalStatusId;
                                _replyToName = _curPost.ScreenName;
                            }
                        }
                        else
                        {
                            // 文頭＠
                            // 複数リプライ
                            StatusText.Text = ". @" + _curPost.ScreenName + " " + StatusText.Text;
                            ClearReplyToInfo();
                        }
                    }
                    else
                    {
                        // 1件選んでCtrl-Rの場合（返信先操作せず）
                        int selectionStart = StatusText.SelectionStart;
                        string id = "@" + _curPost.ScreenName + " ";
                        if (selectionStart > 0)
                        {
                            if (StatusText.Text.Substring(selectionStart - 1, 1) != " ")
                            {
                                id = " " + id;
                            }
                        }

                        StatusText.Text = StatusText.Text.Insert(selectionStart, id);
                        selectionStart += id.Length;
                        StatusText.SelectionStart = selectionStart;
                        StatusText.Focus();
                        return;
                    }
                }

                StatusText.SelectionStart = StatusText.Text.Length;
                StatusText.Focus();
                return;
            }

            // 複数リプライ
            if (!isAuto && !isReply)
            {
                return;
            }

            // C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r
            if (isAuto)
            {
                // Enter or DoubleClick
                string statusTxt = StatusText.Text;
                if (!statusTxt.StartsWith(". "))
                {
                    statusTxt = ". " + statusTxt;
                    ClearReplyToInfo();
                }

                for (int cnt = 0; cnt < _curList.SelectedIndices.Count; ++cnt)
                {
                    var name = _statuses.Item(_curTab.Text, _curList.SelectedIndices[cnt]).ScreenName;
                    if (!statusTxt.Contains("@" + name + " "))
                    {
                        statusTxt = statusTxt.Insert(2, "@" + name + " ");
                    }
                }

                StatusText.Text = statusTxt;
                StatusText.SelectionStart = StatusText.Text.Length;
                StatusText.Focus();
                return;
            }

            // C-S-r or C-r
            string ids = string.Empty;
            int sidx = StatusText.SelectionStart;
            PostClass post;
            if (_curList.SelectedIndices.Count > 1)
            {
                // 複数ポスト選択
                for (int cnt = 0; cnt <= _curList.SelectedIndices.Count - 1; cnt++)
                {
                    post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[cnt]);
                    if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(_tw.Username, StringComparison.CurrentCultureIgnoreCase))
                    {
                        ids += "@" + post.ScreenName + " ";
                    }

                    if (isAll)
                    {
                        foreach (string nm in post.ReplyToList)
                        {
                            if (!ids.Contains("@" + nm + " ") && !nm.Equals(_tw.Username, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                ids += string.Format("@{0} ", m.Success ? m.Result("${id}") : nm);
                            }
                        }
                    }
                }

                if (ids.Length == 0)
                {
                    return;
                }

                if (!StatusText.Text.StartsWith(". "))
                {
                    StatusText.Text = ". " + StatusText.Text;
                    sidx += 2;
                    ClearReplyToInfo();
                }

                if (sidx > 0)
                {
                    if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                    {
                        ids = " " + ids;
                    }
                }

                StatusText.Text = StatusText.Text.Insert(sidx, ids);
                sidx += ids.Length;
                StatusText.SelectionStart = sidx;
                StatusText.Focus();
                return;
            }

            // 1件のみ選択のC-S-r（返信元付加する可能性あり）
            ids = string.Empty;
            sidx = StatusText.SelectionStart;
            post = _curPost;
            if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(_tw.Username, StringComparison.CurrentCultureIgnoreCase))
            {
                ids += "@" + post.ScreenName + " ";
            }

            foreach (string nm in post.ReplyToList)
            {
                if (!ids.Contains("@" + nm + " ") && !nm.Equals(_tw.Username, StringComparison.CurrentCultureIgnoreCase))
                {
                    Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                    ids += string.Format("@{0} ", m.Success ? m.Result("${id}") : nm);
                }
            }

            if (!string.IsNullOrEmpty(post.RetweetedBy))
            {
                if (!ids.Contains("@" + post.RetweetedBy + " ") && !post.RetweetedBy.Equals(_tw.Username, StringComparison.CurrentCultureIgnoreCase))
                {
                    ids += "@" + post.RetweetedBy + " ";
                }
            }

            if (ids.Length == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(StatusText.Text))
            {
                // 未入力の場合のみ返信先付加
                StatusText.Text = ids;
                StatusText.SelectionStart = ids.Length;
                StatusText.Focus();
                _replyToId = post.OriginalStatusId;
                _replyToName = post.ScreenName;
                return;
            }

            if (sidx > 0)
            {
                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                {
                    ids = " " + ids;
                }
            }

            StatusText.Text = StatusText.Text.Insert(sidx, ids);
            sidx += ids.Length;
            StatusText.SelectionStart = sidx;
            StatusText.Focus();
        }

        private void RefreshTasktrayIcon(bool forceRefresh)
        {
            if (_colorize)
            {
                Colorize();
            }

            if (!TimerRefreshIcon.Enabled)
            {
                return;
            }

            if (forceRefresh)
            {
                _isIdle = false;
            }

            _iconCnt += 1;
            _blinkCnt += 1;
            bool busy = false;
            foreach (BackgroundWorker bw in _bworkers)
            {
                if (bw != null && bw.IsBusy)
                {
                    busy = true;
                    break;
                }
            }

            if (_iconCnt > 3)
            {
                _iconCnt = 0;
            }

            if (_blinkCnt > 10)
            {
                _blinkCnt = 0;
                SaveConfigsAll(true);
            }

            if (busy)
            {
                NotifyIcon1.Icon = _iconRefresh[_iconCnt];
                _isIdle = false;
                _myStatusError = false;
                return;
            }

            TabClass tb = _statuses.GetTabByType(TabUsageType.Mentions);
            if (_configs.ReplyIconState != ReplyIconState.None && tb != null && tb.UnreadCount > 0)
            {
                if (_blinkCnt > 0)
                {
                    return;
                }

                _doBlink = !_doBlink;
                NotifyIcon1.Icon = _doBlink || _configs.ReplyIconState == ReplyIconState.StaticIcon ?
                    _replyIcon : _replyIconBlink;
                _isIdle = false;
                return;
            }

            if (_isIdle)
            {
                return;
            }

            _isIdle = true;

            // 優先度：エラー→オフライン→アイドル．エラーは更新アイコンでクリアされる
            if (_myStatusError)
            {
                NotifyIcon1.Icon = _iconAtRed;
                return;
            }

            NotifyIcon1.Icon = MyCommon.IsNetworkAvailable() ? _iconAt : _iconAtSmoke;
        }

        private void ChangeTabMenuControl(string tabName)
        {
            FilterEditMenuItem.Enabled = EditRuleTbMenuItem.Enabled = true;
            var deletetab = _statuses.Tabs[tabName].TabType != TabUsageType.Mentions ? !_statuses.IsDefaultTab(tabName) : false;
            DeleteTabMenuItem.Enabled = DeleteTbMenuItem.Enabled = deletetab;
        }

        private bool SelectTab(ref string tabName)
        {
            do
            {
                // 振り分け先タブ選択
                if (_tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    TopMost = _configs.AlwaysTop;
                    return false;
                }

                TopMost = _configs.AlwaysTop;
                tabName = _tabDialog.SelectedTabName;
                ListTab.SelectedTab.Focus();
                if (tabName != R.IDRuleMenuItem_ClickText1)
                {
                    // 既存タブを選択
                    return true;
                }

                // 新規タブを選択→タブ作成
                var tn = _statuses.GetUniqueTabName();
                if (!TryUserInputText(ref tn))
                {
                    return false;
                }

                tabName = tn;
                TopMost = _configs.AlwaysTop;
                if (!string.IsNullOrEmpty(tabName))
                {
                    if (_statuses.AddTab(tabName, TabUsageType.UserDefined, null) && AddNewTab(tabName, false, TabUsageType.UserDefined))
                    {
                        return true;
                    }

                    // もう一度タブ名入力
                    string tmp = string.Format(R.IDRuleMenuItem_ClickText2, tabName);
                    MessageBox.Show(tmp, R.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            while (true);
        }

        private void GetMoveOrCopy(ref bool move, ref bool mark)
        {
            // 移動するか？
            string tmp = string.Format(R.IDRuleMenuItem_ClickText4, Environment.NewLine);
            var reslut = MessageBox.Show(tmp, R.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            move = reslut != DialogResult.Yes;
            if (!move)
            {
                // マークするか？
                tmp = string.Format(R.IDRuleMenuItem_ClickText6, Environment.NewLine);
                reslut = MessageBox.Show(tmp, R.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                mark = reslut == DialogResult.Yes;
            }
        }

        private void MoveMiddle()
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = _curList.SelectedIndices[0];
            var item1 = _curList.GetItemAt(0, 25);
            int idx1 = item1 == null ? 0 : item1.Index;
            var item2 = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            int idx2 = item2 == null ? _curList.VirtualListSize - 1 : item2.Index;
            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0)
            {
                idx = 0;
            }

            _curList.EnsureVisible(_curList.VirtualListSize - 1);
            _curList.EnsureVisible(idx);
        }

        private void ClearTab(string tabName, bool showWarning)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            if (showWarning)
            {
                string msg = string.Format(R.ClearTabMenuItem_ClickText1, Environment.NewLine);
                string caption = string.Format("{0} {1}", tabName, R.ClearTabMenuItem_ClickText2);
                var reslt = MessageBox.Show(msg, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (reslt == DialogResult.Cancel)
                {
                    return;
                }
            }

            _statuses.ClearTabIds(tabName);
            if (ListTab.SelectedTab.Text == tabName)
            {
                _anchorPost = null;
                _anchorFlag = false;
                _itemCache = null;
                _postCache = null;
                _itemCacheIndex = -1;
                _curItemIndex = -1;
                _curPost = null;
            }

            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    tb.ImageIndex = -1;
                    ((DetailsListView)tb.Tag).VirtualListSize = 0;
                    break;
                }
            }

            if (!_configs.TabIconDisp)
            {
                ListTab.Refresh();
            }

            SetMainWindowTitle();
            SetStatusLabelUrl();
        }

        // メインウインドウタイトルの書き換え
        private void SetMainWindowTitle()
        {
            int ur = 0;
            int al = 0;
            if (_configs.DispLatestPost != DispTitleEnum.None
                && _configs.DispLatestPost != DispTitleEnum.Post
                && _configs.DispLatestPost != DispTitleEnum.Ver
                && _configs.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                ur = _statuses.GetAllUnreadCount();
                al = _statuses.GetAllCount();
            }

            var ttl = new StringBuilder(256);
            if (_configs.DispUsername)
            {
                ttl.Append(_tw.Username).Append(" - ");
            }

            ttl.Append("Hoehoe  ");
            switch (_configs.DispLatestPost)
            {
                case DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.FileVersion);
                    break;
                case DispTitleEnum.Post:
                    if (_postHistory != null && _postHistory.Count > 1)
                    {
                        ttl.Append(_postHistory[_postHistory.Count - 2].Status.Replace("\r\n", string.Empty));
                    }

                    break;
                case DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(R.SetMainWindowTitleText1, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(R.SetMainWindowTitleText2, ur);
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(R.SetMainWindowTitleText3, ur, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(R.SetMainWindowTitleText4, ur, al);
                    break;
                case DispTitleEnum.OwnStatus:
                    if (_prevFollowerCount == 0 && _tw.FollowersCount > 0)
                    {
                        _prevFollowerCount = _tw.FollowersCount;
                    }

                    ttl.AppendFormat(R.OwnStatusTitle, _tw.StatusesCount, _tw.FriendsCount, _tw.FollowersCount, _tw.FollowersCount - _prevFollowerCount);
                    break;
            }

            try
            {
                Text = ttl.ToString();
            }
            catch (AccessViolationException ex)
            {
                // 原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
                Debug.Write(ex);
            }
        }

        private string GetStatusLabelText()
        {
            // ステータス欄にカウント表示
            // タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (_statuses == null)
            {
                return string.Empty;
            }

            TabClass mentionTab = _statuses.GetTabByType(TabUsageType.Mentions);
            TabClass dmessageTab = _statuses.GetTabByType(TabUsageType.DirectMessage);
            if (mentionTab == null || dmessageTab == null)
            {
                return string.Empty;
            }

            try
            {
                int ur = _statuses.GetAllUnreadCount();
                int al = _statuses.GetAllCount();
                int tur = _statuses.Tabs[_curTab.Text].UnreadCount;
                int tal = _statuses.Tabs[_curTab.Text].AllCount;
                _unreadCounter = ur;
                _unreadAtCounter = mentionTab.UnreadCount + dmessageTab.UnreadCount;
                StringBuilder slbl = new StringBuilder(256);
                slbl.AppendFormat(R.SetStatusLabelText1, tur, tal, ur, al, _unreadAtCounter, _postTimestamps.Count, _favTimestamps.Count, _timeLineCount);
                if (_configs.TimelinePeriodInt == 0)
                {
                    slbl.Append(R.SetStatusLabelText2);
                }
                else
                {
                    slbl.Append(string.Format("{0}{1}", _configs.TimelinePeriodInt, R.SetStatusLabelText3));
                }

                return slbl.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void SetStatusLabelApi()
        {
            _apiGauge.RemainCount = MyCommon.TwitterApiInfo.RemainCount;
            _apiGauge.MaxCount = MyCommon.TwitterApiInfo.MaxCount;
            _apiGauge.ResetTime = MyCommon.TwitterApiInfo.ResetTime;
        }

        private void SetStatusLabelUrl()
        {
            StatusLabelUrl.Text = GetStatusLabelText();
        }

        // タスクトレイアイコンのツールチップテキスト書き換え
        // Tween [未読/@]
        private void SetNotifyIconText()
        {
            StringBuilder ur = new StringBuilder(64);
            if (_configs.DispUsername)
            {
                ur.AppendFormat("{0} - ", _tw.Username);
            }

            ur.Append("Hoehoe");
#if DEBUG
            ur.Append("(Debug Build)");
#endif
            if (_unreadCounter != -1 && _unreadAtCounter != -1)
            {
                ur.AppendFormat(" [{0}/@{1}]", _unreadCounter, _unreadAtCounter);
            }

            NotifyIcon1.Text = ur.ToString();
        }

        private void OpenRepliedStatus()
        {
            if (ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId > 0)
            {
                if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenUriAsync(string.Format("https://twitter.com/{0}/status/{1}", _curPost.InReplyToUser, _curPost.InReplyToStatusId));
                    return;
                }

                if (_statuses.ContainsKey(_curPost.InReplyToStatusId))
                {
                    MessageBox.Show(_statuses.Item(_curPost.InReplyToStatusId).MakeReplyPostInfoLine());
                }
                else
                {
                    foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.Lists | TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(_curPost.InReplyToStatusId))
                        {
                            break;
                        }

                        MessageBox.Show(_statuses.Item(_curPost.InReplyToStatusId).MakeReplyPostInfoLine());
                        return;
                    }

                    OpenUriAsync(string.Format("https://twitter.com/{0}/status/{1}", _curPost.InReplyToUser, _curPost.InReplyToStatusId));
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

            if (StatusText.SelectionLength > 0)
            {
                // 文字列が選択されている場合はその文字列について処理
                string tmp = StatusText.SelectedText;

                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http"))
                {
                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (_configs.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    SetUrlUndoInfo(tmp, result);
                }
            }
            else
            {
                const string UrlPattern = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?://)" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";

                // 正規表現にマッチしたURL文字列をtinyurl化
                foreach (Match mt in Regex.Matches(StatusText.Text, UrlPattern, RegexOptions.IgnoreCase))
                {
                    string url = mt.Result("${url}");
                    if (StatusText.Text.IndexOf(url, StringComparison.Ordinal) == -1)
                    {
                        continue;
                    }

                    string tmp = url;
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase))
                    {
                        tmp = "http://" + tmp;
                    }

                    // 選んだURLを選択（？）
                    StatusText.Select(StatusText.Text.IndexOf(url, StringComparison.Ordinal), url.Length);

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (_configs.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    SetUrlUndoInfo(url, result);
                }
            }

            return true;
        }

        private void SetUrlUndoInfo(string before, string after)
        {
            if (!string.IsNullOrEmpty(after))
            {
                StatusText.Select(StatusText.Text.IndexOf(before, StringComparison.Ordinal), before.Length);
                StatusText.SelectedText = after;

                // undoバッファにセット
                if (_urlUndoBuffer == null)
                {
                    _urlUndoBuffer = new List<UrlUndoInfo>();
                    UrlUndoToolStripMenuItem.Enabled = true;
                }

                _urlUndoBuffer.Add(new UrlUndoInfo { Before = before, After = after });
            }
        }

        private void UndoUrlShortening()
        {
            if (_urlUndoBuffer != null)
            {
                string tmp = StatusText.Text;
                foreach (UrlUndoInfo data in _urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }

                StatusText.Text = tmp;
                _urlUndoBuffer = null;
                UrlUndoToolStripMenuItem.Enabled = false;
                StatusText.SelectionStart = 0;
                StatusText.SelectionLength = 0;
            }
        }

        private void SearchWebBySelectedWord(string url)
        {
            // 発言詳細で「選択文字列で検索」（選択文字列取得）
            string selText = WebBrowser_GetSelectionText(PostBrowser);
            if (selText != null)
            {
                if (url == R.SearchItem4Url)
                {
                    // 公式検索
                    AddNewTabForSearch(selText);
                    return;
                }

                OpenUriAsync(string.Format(url, HttpUtility.UrlEncode(selText)));
            }
        }

        private void ListTabSelect(int index)
        {
            ListTab.SelectedIndex = index;
            ListTabSelect(ListTab.TabPages[index]);
        }

        private void ListTabSelect(TabPage tab)
        {
            SetListProperty();
            _itemCache = null;
            _itemCacheIndex = -1;
            _postCache = null;
            _curTab = tab;
            _curList = (DetailsListView)tab.Tag;
            if (_curList.SelectedIndices.Count > 0)
            {
                _curItemIndex = _curList.SelectedIndices[0];
                _curPost = GetCurTabPost(_curItemIndex);
            }
            else
            {
                _curItemIndex = -1;
                _curPost = null;
            }

            _anchorPost = null;
            _anchorFlag = false;

            if (_iconCol)
            {
                _curList.Columns[1].Text = _columnTexts[2];
            }
            else
            {
                for (int i = 0; i < _curList.Columns.Count; i++)
                {
                    _curList.Columns[i].Text = _columnTexts[i];
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
            if (args.WorkerType == WorkerType.Follower)
            {
                if (_followerFetchWorker == null)
                {
                    bw = _followerFetchWorker = CreateTimelineWorker();
                }
                else
                {
                    if (!_followerFetchWorker.IsBusy)
                    {
                        bw = _followerFetchWorker;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _bworkers.Length; i++)
                {
                    if (_bworkers[i] != null && !_bworkers[i].IsBusy)
                    {
                        bw = _bworkers[i];
                        break;
                    }
                }

                if (bw == null)
                {
                    for (int i = 0; i < _bworkers.Length; i++)
                    {
                        if (_bworkers[i] == null)
                        {
                            _bworkers[i] = CreateTimelineWorker();
                            bw = _bworkers[i];
                            break;
                        }
                    }
                }
            }

            if (bw == null)
            {
                return;
            }

            bw.RunWorkerAsync(args);
        }

        private BackgroundWorker CreateTimelineWorker()
        {
            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += GetTimelineWorker_DoWork;
            bw.ProgressChanged += GetTimelineWorker_ProgressChanged;
            bw.RunWorkerCompleted += GetTimelineWorker_RunWorkerCompleted;
            return bw;
        }

        private void StartUserStream()
        {
            _tw.NewPostFromStream += Tw_NewPostFromStream;
            _tw.UserStreamStarted += Tw_UserStreamStarted;
            _tw.UserStreamStopped += Tw_UserStreamStopped;
            _tw.PostDeleted += Tw_PostDeleted;
            _tw.UserStreamEventReceived += Tw_UserStreamEventArrived;
            ChangeUserStreamStatusDisplay(true);
            if (_configs.UserstreamStartup)
            {
                _tw.StartUserStream();
            }
        }

        private bool IsInitialRead()
        {
            return _waitTimeline || _waitReply || _waitDm || _waitFav || _waitPubSearch || _waitUserTimeline || _waitLists;
        }

        private void GetFollowers()
        {
            GetTimeline(WorkerType.Follower);
            DispSelectedPost(true);
        }

        private void DoReTweetUnofficial()
        {
            if (ExistCurrentPost)
            {
                if (_curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                // RT @id:内容
                StatusText.Text = string.Format("RT @{0}: {1}", _curPost.ScreenName, HttpUtility.HtmlDecode(CreateRetweetUnofficial(_curPost.Text)));
                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void DoReTweetOfficial(bool isConfirm)
        {
            // 公式RT
            if (!ExistCurrentPost)
            {
                return;
            }

            if (_curPost.IsProtect)
            {
                MessageBox.Show("Protected.");
                _doFavRetweetFlags = false;
                return;
            }

            if (_curList.SelectedIndices.Count > 15)
            {
                MessageBox.Show(R.RetweetLimitText);
                _doFavRetweetFlags = false;
                return;
            }

            if (_curList.SelectedIndices.Count > 1)
            {
                string confirmMessage = R.RetweetQuestion2;
                if (_doFavRetweetFlags)
                {
                    confirmMessage = R.FavoriteRetweetQuestionText1;
                }

                var result = MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    _doFavRetweetFlags = false;
                    return;
                }
            }
            else
            {
                if (_curPost.IsDm || _curPost.IsMe)
                {
                    _doFavRetweetFlags = false;
                    return;
                }

                if (!_configs.RetweetNoConfirm)
                {
                    string confirmMessage = R.RetweetQuestion1;
                    if (_doFavRetweetFlags)
                    {
                        confirmMessage = R.FavoritesRetweetQuestionText2;
                    }

                    if (isConfirm && MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        _doFavRetweetFlags = false;
                        return;
                    }
                }
            }

            var ids = _curList.SelectedIndices.Cast<int>().Select(i => GetCurTabPost(i)).Where(p => !p.IsMe && !p.IsProtect && !p.IsDm);
            if (ids.Count() > 0)
            {
                RunAsync(new GetWorkerArg
                {
                    Ids = ids.Select(p => p.StatusId).ToList(),
                    SIds = new List<long>(),
                    TabName = _curTab.Text,
                    WorkerType = WorkerType.Retweet
                });
            }
        }

        private void FavoritesRetweetOriginal()
        {
            if (!ExistCurrentPost)
            {
                return;
            }

            _doFavRetweetFlags = true;
            DoReTweetOfficial(true);
            if (_doFavRetweetFlags)
            {
                _doFavRetweetFlags = false;
                ChangeSelectedFavStatus(true, false);
            }
        }

        private void FavoritesRetweetUnofficial()
        {
            if (ExistCurrentPost && !_curPost.IsDm)
            {
                _doFavRetweetFlags = true;
                ChangeSelectedFavStatus(true);
                if (!_curPost.IsProtect && _doFavRetweetFlags)
                {
                    _doFavRetweetFlags = false;
                    DoReTweetUnofficial();
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
            status = Regex.Replace(status, "@<a target=\"_self\" href=\"https?://twitter.com/(#!/)?(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "@${url}");
            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${link}");
            status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", StatusText.Multiline ? Environment.NewLine : string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            ClearReplyToInfo();
            return status.Replace("&nbsp;", " ");
        }

        private bool IsKeyDown(Keys key)
        {
            return (ModifierKeys & key) == key;
        }

        private void FollowCommand(string id)
        {
            if (id == null)
            {
                return;
            }

            var followid = id;
            if (!TryUserInputText(ref followid, "Follow", R.FRMessage1))
            {
                return;
            }

            id = followid;
            if (string.IsNullOrEmpty(id) || id == _tw.Username)
            {
                return;
            }

            using (var info = new FormInfo(this, R.FollowCommandText1, FollowCommand_DoWork, null, new FollowRemoveCommandArgs { Tw = _tw, Id = id }))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? R.FRMessage2 + ret : R.FRMessage3);
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
                var removeid = id;
                if (!TryUserInputText(ref removeid, "Unfollow", R.FRMessage1))
                {
                    return;
                }

                id = removeid;
            }

            if (string.IsNullOrEmpty(id) || id == _tw.Username)
            {
                return;
            }

            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs { Tw = _tw, Id = id };
            using (FormInfo info = new FormInfo(this, R.RemoveCommandText1, RemoveCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? R.FRMessage2 + ret : R.FRMessage3);
            }
        }

        private void ShowFriendship(string id)
        {
            if (id == null)
            {
                return;
            }

            var frid = id;
            if (!TryUserInputText(ref frid, "Show Friendships", R.FRMessage1))
            {
                return;
            }

            ShowFriendshipCore(frid);
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
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            id = id.Trim();
            if (id.ToLower() == _tw.Username.ToLower())
            {
                return;
            }

            ShowFriendshipArgs args = new ShowFriendshipArgs { Tw = _tw };
            args.Ids.Add(new ShowFriendshipArgs.FriendshipInfo(id));
            string ret = string.Empty;
            using (FormInfo formInfo = new FormInfo(this, R.ShowFriendshipText1, ShowFriendship_DoWork, null, args))
            {
                formInfo.ShowDialog();
                ret = (string)formInfo.Result;
            }

            if (!string.IsNullOrEmpty(ret))
            {
                MessageBox.Show(ret);
                return;
            }

            ShowFriendshipArgs.FriendshipInfo frsinfo = args.Ids[0];
            string fing = frsinfo.IsFollowing ?
                R.GetFriendshipInfo1 :
                R.GetFriendshipInfo2;
            string fed = frsinfo.IsFollowed ?
                R.GetFriendshipInfo3 :
                R.GetFriendshipInfo4;
            string result = frsinfo.Id + R.GetFriendshipInfo5 + Environment.NewLine;
            result += "  " + fing + Environment.NewLine;
            result += "  " + fed;
            if (frsinfo.IsFollowing)
            {
                var rslt = MessageBox.Show(R.GetFriendshipInfo7 + Environment.NewLine + result, R.GetFriendshipInfo8, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (rslt == DialogResult.Yes)
                {
                    RemoveCommand(frsinfo.Id, true);
                }
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(_postBrowserStatusText, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success)
            {
                string screenname = m.Result("${ScreenName}");
                if (IsTwitterId(screenname))
                {
                    return screenname;
                }
            }

            return null;
        }

        private void DoQuote()
        {
            // QT @id:内容 返信先情報付加
            if (ExistCurrentPost)
            {
                if (_curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                string rtdata = CreateRetweetUnofficial(_curPost.Text);
                StatusText.Text = " QT @" + _curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                _replyToId = _curPost.OriginalStatusId;
                _replyToName = _curPost.ScreenName;
                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void TryOpenSelectedRtUserHome()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                var post = GetCurTabPost(_curList.SelectedIndices[0]);
                if (post.IsRetweeted)
                {
                    OpenUriAsync("https://twitter.com/" + post.RetweetedBy);
                }
            }
        }

        private void ShowUserStatus(string id, bool showInputDialog = true)
        {
            if (id == null)
            {
                return;
            }

            var sid = id;
            if (showInputDialog)
            {
                if (!TryUserInputText(ref sid, "Show UserStatus", R.FRMessage1))
                {
                    return;
                }

                id = sid;
            }

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var user = new DataModels.Twitter.User();
            GetUserInfoArgs args = new GetUserInfoArgs { Tw = _tw, Id = id, User = user };
            using (FormInfo info = new FormInfo(this, R.doShowUserStatusText1, GetUserInfo_DoWork, null, args))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(ret);
                    return;
                }
            }

            using (ShowUserInfo userinfo = new ShowUserInfo())
            {
                userinfo.Owner = this;
                userinfo.SetUser(args.User);
                userinfo.ShowDialog(this);
                Activate();
                BringToFront();
            }
        }

        private void LoadImageFromSelectedFile()
        {
            try
            {
                string imagePath = ImagefilePathText.Text.Trim();
                if (string.IsNullOrEmpty(imagePath) || string.IsNullOrEmpty(ImageService))
                {
                    ClearImageSelectionForms();
                    return;
                }

                IMultimediaShareService service = _pictureServices[ImageService];
                FileInfo fl = new FileInfo(imagePath);
                if (!service.CheckValidExtension(fl.Extension))
                {
                    // 画像以外の形式
                    ClearImageSelectionForms();
                    return;
                }

                if (!service.CheckValidFilesize(fl.Extension, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    ClearImageSelectionForms();
                    MessageBox.Show("File is too large.");
                    return;
                }

                switch (service.GetFileType(fl.Extension))
                {
                    case UploadFileType.Picture:
                        Image img = null;
                        using (FileStream fs = new FileStream(ImagefilePathText.Text, FileMode.Open, FileAccess.Read))
                        {
                            img = Image.FromStream(fs);
                        }

                        ImageSelectedPicture.Image = MyCommon.CheckValidImage(img, img.Width, img.Height);
                        ImageSelectedPicture.Tag = UploadFileType.Picture;
                        break;
                    case UploadFileType.MultiMedia:
                        ImageSelectedPicture.Image = R.MultiMediaImage;
                        ImageSelectedPicture.Tag = UploadFileType.MultiMedia;
                        break;
                    default:
                        ClearImageSelectionForms();
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                ClearImageSelectionForms();
                MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                ClearImageSelectionForms();
                MessageBox.Show("The type of this file is not image.");
            }
        }

        private void ClearImageSelectionForms()
        {
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
            ImageSelectedPicture.Tag = UploadFileType.Invalid;
            ImagefilePathText.Text = string.Empty;
        }

        private void SetImageServiceCombo()
        {
            string svc = string.Empty;
            if (ImageServiceCombo.SelectedIndex > -1)
            {
                svc = ImageServiceCombo.SelectedItem.ToString();
            }

            if (_pictureServices == null)
            {
                CreatePictureServices();
            }

            ImageServiceCombo.Items.Clear();
            ImageServiceCombo.Items.AddRange(_pictureServices.Keys.ToArray());
            if (string.IsNullOrEmpty(svc))
            {
                ImageServiceCombo.SelectedIndex = 0;
            }
            else
            {
                int idx = ImageServiceCombo.Items.IndexOf(svc);
                ImageServiceCombo.SelectedIndex = idx == -1 ? 0 : idx;
            }
        }

        private void CopyUserId()
        {
            if (_curPost == null)
            {
                return;
            }

            CopyToClipboard(_curPost.ScreenName);
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            // 新着通知
            if (IsBalloonRequired(ev))
            {
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                StringBuilder title = new StringBuilder();
                if (_configs.DispUsername)
                {
                    title.AppendFormat("{0} - ", _tw.Username);
                }

                title.AppendFormat("Hoehoe [{0}] ", ev.Event.ToUpper());
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.AppendFormat("by {0}", ev.Username);
                }

                string text = !string.IsNullOrEmpty(ev.Target) ? ev.Target : " ";
                if (_configs.IsNotifyUseGrowl)
                {
                    _growlHelper.Notify(GrowlHelper.NotifyType.UserStreamEvent, ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                    NotifyIcon1.BalloonTipTitle = title.ToString();
                    NotifyIcon1.BalloonTipText = text;
                    NotifyIcon1.ShowBalloonTip(500);
                }
            }

            if (Convert.ToBoolean(ev.Eventtype & _configs.EventNotifyFlag) && IsMyEventNotityAsEventType(ev))
            {
                // サウンド再生
                if (!_isInitializing && _configs.PlaySound)
                {
                    MyCommon.PlaySound(_configs.EventSoundFile);
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
            string dstlng = _configs.TranslateLanguage;
            string msg = string.Empty;
            if (srclng != dstlng && bing.Translate(string.Empty, dstlng, str, ref buf))
            {
                PostBrowser.DocumentText = CreateDetailHtml(buf);
            }
            else
            {
                if (msg.StartsWith("Err:"))
                {
                    StatusLabel.Text = msg;
                }
            }
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            string id = string.Empty;
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }

            var uid = id;
            if (!TryUserInputText(ref uid, caption, R.FRMessage1))
            {
                return string.Empty;
            }

            return uid;
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            if (isEnable)
            {
                _tw.StartUserStream();
            }
            else
            {
                _tw.StopUserStream();
            }

            _timerTimeline.Enabled = isEnable;
        }

        private void OpenUserAppointUrl()
        {
            if (_configs.UserAppointUrl != null)
            {
                if (_configs.UserAppointUrl.Contains("{ID}") || _configs.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (_curPost != null)
                    {
                        string url = _configs.UserAppointUrl
                            .Replace("{ID}", _curPost.ScreenName)
                            .Replace("{STATUS}", _curPost.OriginalStatusId.ToString());
                        OpenUriAsync(url);
                    }
                }
                else
                {
                    OpenUriAsync(_configs.UserAppointUrl);
                }
            }
        }

        private void SetupOperateContextMenu()
        {
            if (ListTab.SelectedTab == null)
            {
                return;
            }

            if (_statuses == null || _statuses.Tabs == null || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
            {
                return;
            }

            bool existCurrentPost = ExistCurrentPost;
            ReplyStripMenuItem.Enabled = existCurrentPost;
            ReplyAllStripMenuItem.Enabled = existCurrentPost;
            DMStripMenuItem.Enabled = existCurrentPost;
            ShowProfileMenuItem.Enabled = existCurrentPost;
            ShowUserTimelineContextMenuItem.Enabled = existCurrentPost;
            ListManageUserContextToolStripMenuItem2.Enabled = existCurrentPost;
            MoveToFavToolStripMenuItem.Enabled = existCurrentPost;
            TabMenuItem.Enabled = existCurrentPost;
            IDRuleMenuItem.Enabled = existCurrentPost;
            UnreadStripMenuItem.Enabled = existCurrentPost;
            ReadedStripMenuItem.Enabled = existCurrentPost;

            TabUsageType selectedTabType = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
            bool dmsgOrNotExist = selectedTabType == TabUsageType.DirectMessage || !existCurrentPost || _curPost.IsDm;

            FavAddToolStripMenuItem.Enabled = !dmsgOrNotExist;
            FavRemoveToolStripMenuItem.Enabled = !dmsgOrNotExist;
            StatusOpenMenuItem.Enabled = !dmsgOrNotExist;
            FavorareMenuItem.Enabled = !dmsgOrNotExist;
            ShowRelatedStatusesMenuItem.Enabled = !dmsgOrNotExist;
            DeleteStripMenuItem.Text = !dmsgOrNotExist && _curPost.IsRetweeted ? R.DeleteMenuText2 : R.DeleteMenuText1;
            DeleteStripMenuItem.Enabled = !dmsgOrNotExist ? _curPost.IsMe : existCurrentPost && _curPost.IsDm;
            ReTweetOriginalStripMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            FavoriteRetweetContextMenu.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            ReTweetStripMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe;
            QuoteStripMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe;
            FavoriteRetweetUnofficialContextMenu.Enabled = !dmsgOrNotExist && !_curPost.IsMe;
            RepliedStatusOpenMenuItem.Enabled = existCurrentPost && selectedTabType != TabUsageType.PublicSearch && _curPost.InReplyToStatusId > 0;
            MoveToRTHomeMenuItem.Enabled = existCurrentPost && _curPost.IsRetweeted;
        }

        private void SetupPostBrowserContextMenu()
        {
            // URLコピーの項目の表示/非表示
            string postBrowserStatusText1 = PostBrowser.StatusText;
            bool isHttpUrl = postBrowserStatusText1.StartsWith("http");
            _postBrowserStatusText = isHttpUrl ? postBrowserStatusText1 : string.Empty;
            UrlCopyContextMenuItem.Enabled = isHttpUrl;

            bool enable = isHttpUrl && !string.IsNullOrEmpty(GetUserId());
            FollowContextMenuItem.Enabled = enable;
            RemoveContextMenuItem.Enabled = enable;
            FriendshipContextMenuItem.Enabled = enable;
            ShowUserStatusContextMenuItem.Enabled = enable;
            SearchPostsDetailToolStripMenuItem.Enabled = enable;
            IdFilterAddMenuItem.Enabled = enable;
            ListManageUserContextToolStripMenuItem.Enabled = enable;
            SearchAtPostsDetailToolStripMenuItem.Enabled = enable;
            UseHashtagMenuItem.Enabled = isHttpUrl && Regex.IsMatch(_postBrowserStatusText, "^https?://twitter.com/search\\?q=%23");

            // 文字列選択されてるときは選択文字列関係の項目を表示
            bool hasSelection = !string.IsNullOrEmpty(WebBrowser_GetSelectionText(PostBrowser));
            SelectionSearchContextMenuItem.Enabled = hasSelection;
            SelectionCopyContextMenuItem.Enabled = hasSelection;
            SelectionTranslationToolStripMenuItem.Enabled = hasSelection;

            // 発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            FriendshipAllMenuItem.Enabled = Regex.Matches(PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"").Cast<Match>()
                .Select(m => m.Result("${ScreenName}").ToLower())
                .Any(s => s != _tw.Username.ToLower());
            TranslationToolStripMenuItem.Enabled = _curPost != null;
        }

        private void SetupPostModeContextMenu()
        {
            ToolStripMenuItemUrlAutoShorten.Checked = _configs.UrlConvertAuto;
        }

        private void SetupSourceContextMenu()
        {
            bool dmsgOrNotExist = _curPost == null || !ExistCurrentPost || _curPost.IsDm;
            SourceCopyMenuItem.Enabled = SourceUrlCopyMenuItem.Enabled = !dmsgOrNotExist;
        }

        private void SetupTabPropertyContextMenu(bool fromMenuBar)
        {
            // 右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (string.IsNullOrEmpty(_rclickTabName) || fromMenuBar)
            {
                if (ListTab != null && ListTab.SelectedTab != null)
                {
                    _rclickTabName = ListTab.SelectedTab.Text;
                }
                else
                {
                    return;
                }
            }

            if (_statuses == null || _statuses.Tabs == null)
            {
                return;
            }

            TabClass tb = _statuses.Tabs[_rclickTabName];
            if (tb == null)
            {
                return;
            }

            NotifyDispMenuItem.Checked = tb.Notify;
            NotifyTbMenuItem.Checked = tb.Notify;

            _soundfileListup = true;
            MyCommon.ReloadSoundSelector(SoundFileComboBox.ComboBox, tb.SoundFile);
            MyCommon.ReloadSoundSelector(SoundFileTbComboBox.ComboBox, tb.SoundFile);
            _soundfileListup = false;

            UreadManageMenuItem.Checked = tb.UnreadManage;
            UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            ChangeTabMenuControl(_rclickTabName);
        }

        /// <summary>
        /// 発言詳細のアイコン右クリック時のメニュー制御
        /// </summary>
        private void SetupUserPictureContextMenu()
        {
            var saveiconmenu = false;
            var iconmenu = false;
            var iconmenutxt = R.ContextMenuStrip3_OpeningText1;
            if (_curList.SelectedIndices.Count <= 0 || _curPost == null)
            {
                iconmenutxt = R.ContextMenuStrip3_OpeningText2;
            }
            else
            {
                string name = _curPost.ImageUrl;
                if (!string.IsNullOrEmpty(name))
                {
                    saveiconmenu = _iconDict[_curPost.ImageUrl] != null;
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

            SaveIconPictureToolStripMenuItem.Enabled = saveiconmenu;
            IconNameToolStripMenuItem.Enabled = iconmenu;
            IconNameToolStripMenuItem.Text = iconmenutxt;

            object tag = NameLabel.Tag;
            bool hasName = tag != null;
            ShowUserStatusToolStripMenuItem.Enabled = hasName;
            SearchPostsDetailNameToolStripMenuItem.Enabled = hasName;
            ListManageUserContextToolStripMenuItem3.Enabled = hasName;

            bool enable = hasName && (string)tag != _tw.Username;
            FollowToolStripMenuItem.Enabled = enable;
            UnFollowToolStripMenuItem.Enabled = enable;
            ShowFriendShipToolStripMenuItem.Enabled = enable;
            SearchAtPostsDetailNameToolStripMenuItem.Enabled = enable;
        }

        private void SetupCommandMenu()
        {
            RtCountMenuItem.Enabled = ExistCurrentPost && !_curPost.IsDm;
        }

        private void SetupEditMenu()
        {
            UndoRemoveTabMenuItem.Enabled = _statuses.RemovedTab.Count != 0;
            PublicSearchQueryMenuItem.Enabled = ListTab.SelectedTab != null && _statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch;
            CopyUserIdStripMenuItem.Enabled = ExistCurrentPost;
            CopyURLMenuItem.Enabled = ExistCurrentPost && !_curPost.IsDm;
            CopySTOTMenuItem.Enabled = ExistCurrentPost && !_curPost.IsProtect;
        }

        private void SetupHelpMenu()
        {
            DebugModeToolStripMenuItem.Visible = MyCommon.DebugBuild || (IsKeyDown(Keys.Control) && IsKeyDown(Keys.Shift));
        }

        private void SetupOperateMenu()
        {
            if (ListTab.SelectedTab == null
                || _statuses == null
                || _statuses.Tabs == null
                || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
            {
                return;
            }

            bool existCurrentPost = ExistCurrentPost;
            ReplyOpMenuItem.Enabled = existCurrentPost;
            ReplyAllOpMenuItem.Enabled = existCurrentPost;
            DmOpMenuItem.Enabled = existCurrentPost;
            ShowProfMenuItem.Enabled = existCurrentPost;
            ShowUserTimelineToolStripMenuItem.Enabled = existCurrentPost;
            ListManageMenuItem.Enabled = existCurrentPost;
            OpenFavOpMenuItem.Enabled = existCurrentPost;
            CreateTabRuleOpMenuItem.Enabled = existCurrentPost;
            CreateIdRuleOpMenuItem.Enabled = existCurrentPost;
            ReadOpMenuItem.Enabled = existCurrentPost;
            UnreadOpMenuItem.Enabled = existCurrentPost;

            TabUsageType selectedTabType = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
            bool dmsgOrNotExist = selectedTabType == TabUsageType.DirectMessage || !existCurrentPost || _curPost.IsDm;
            FavOpMenuItem.Enabled = !dmsgOrNotExist;
            UnFavOpMenuItem.Enabled = !dmsgOrNotExist;
            OpenStatusOpMenuItem.Enabled = !dmsgOrNotExist;
            OpenFavotterOpMenuItem.Enabled = !dmsgOrNotExist;
            ShowRelatedStatusesMenuItem2.Enabled = !dmsgOrNotExist;

            DelOpMenuItem.Enabled = !dmsgOrNotExist ? _curPost.IsMe : existCurrentPost && _curPost.IsDm;
            RtOpMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            RtUnOpMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            QtOpMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            FavoriteRetweetMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;
            FavoriteRetweetUnofficialMenuItem.Enabled = !dmsgOrNotExist && !_curPost.IsMe && !_curPost.IsProtect;

            RefreshPrevOpMenuItem.Enabled = selectedTabType != TabUsageType.Favorites;
            OpenRepSourceOpMenuItem.Enabled = selectedTabType != TabUsageType.PublicSearch && existCurrentPost && _curPost.InReplyToStatusId > 0;
            OpenRterHomeMenuItem.Enabled = existCurrentPost && !string.IsNullOrEmpty(_curPost.RetweetedBy);
        }

        private void ShowAboutBox()
        {
            if (_aboutBox == null)
            {
                _aboutBox = new TweenAboutBox();
            }

            _aboutBox.ShowDialog();
            TopMost = _configs.AlwaysTop;
        }

        private void ShowApiInfoBox()
        {
            GetApiInfoArgs args = new GetApiInfoArgs { Tw = _tw, Info = new ApiInfo() };
            StringBuilder tmp = new StringBuilder();
            using (FormInfo dlg = new FormInfo(this, R.ApiInfo6, GetApiInfo_Dowork, null, args))
            {
                dlg.ShowDialog();
                if ((bool)dlg.Result)
                {
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo1, args.Info.MaxCount));
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo2, args.Info.RemainCount));
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo3, args.Info.ResetTime));
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo7, _tw.UserStreamEnabled ? R.Enable : R.Disable));
                    tmp.AppendLine();
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo8, args.Info.AccessLevel));
                    tmp.AppendLine();
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo9, args.Info.MediaMaxCount < 0 ? R.ApiInfo91 : args.Info.MediaMaxCount.ToString()));
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo10, args.Info.MediaRemainCount < 0 ? R.ApiInfo91 : args.Info.MediaRemainCount.ToString()));
                    tmp.AppendLine(string.Format("{0}{1}", R.ApiInfo11, args.Info.MediaResetTime == new DateTime() ? R.ApiInfo91 : args.Info.MediaResetTime.ToString()));
                    SetStatusLabelUrl();
                }
                else
                {
                    tmp.Append(R.ApiInfo5);
                }
            }

            MessageBox.Show(tmp.ToString(), R.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowCacheInfoBox()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine(string.Format("{0, -15} : {1}bytes ({2}MB)", "キャッシュメモリ容量", _iconDict.CacheMemoryLimit, _iconDict.CacheMemoryLimit / (1024 * 1024)));
            buf.AppendLine(string.Format("{0, -15} : {1}%", "物理メモリ使用割合", _iconDict.PhysicalMemoryLimit));
            buf.AppendLine(string.Format("{0, -15} : {1}", "キャッシュエントリ保持数", _iconDict.CacheCount));
            buf.AppendLine(string.Format("{0, -15} : {1}", "キャッシュエントリ破棄数", _iconDict.CacheRemoveCount));
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void ShowEventViewerBox()
        {
            if (_evtDialog == null || _evtDialog.IsDisposed)
            {
                _evtDialog = new EventViewerDialog();
                _evtDialog.Owner = this;

                // 親の中央に表示
                Point pos = _evtDialog.Location;
                pos.X = Convert.ToInt32(Location.X + ((Size.Width - _evtDialog.Size.Width) / 2));
                pos.Y = Convert.ToInt32(Location.Y + ((Size.Height - _evtDialog.Size.Height) / 2));
                _evtDialog.Location = pos;
            }

            _evtDialog.EventSource = _tw.StoredEvent;
            if (!_evtDialog.Visible)
            {
                _evtDialog.Show(this);
            }
            else
            {
                _evtDialog.Activate();
            }

            TopMost = _configs.AlwaysTop;
        }

        private void ShowPostImageFileSelectBox()
        {
            if (string.IsNullOrEmpty(ImageService))
            {
                return;
            }

            OpenFileDialog1.Filter = _pictureServices[ImageService].GetFileOpenDialogFilter();
            OpenFileDialog1.Title = R.PickPictureDialog1;
            OpenFileDialog1.FileName = string.Empty;

            try
            {
                AllowDrop = false;
                if (OpenFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
            }
            finally
            {
                AllowDrop = true;
            }

            ImagefilePathText.Text = OpenFileDialog1.FileName;
            LoadImageFromSelectedFile();
        }

        private void ShowFilterEditBox()
        {
            if (string.IsNullOrEmpty(_rclickTabName))
            {
                _rclickTabName = _statuses.GetTabByType(TabUsageType.Home).TabName;
            }

            _fltDialog.SetCurrent(_rclickTabName);
            _fltDialog.ShowDialog();
            TopMost = _configs.AlwaysTop;
            ApplyNewFilters();
            SaveConfigsTabs();
        }

        private void ShowFriendshipOfAllUserInCurrentTweet()
        {
            var ma = Regex.Matches(PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            if (ma.Count > 0)
            {
                ShowFriendship(ma.Cast<Match>().Select(m => m.Result("${ScreenName}")).ToArray());
            }
        }

        private void ShowFriendshipOfCurrentTweetUser()
        {
            ShowFriendship(_curPost == null ? string.Empty : _curPost.ScreenName);
        }

        private void ShowFriendshipOfCurrentLinkUser()
        {
            ShowFriendship(GetUserId());
        }

        private void ShowFriendshipOfCurrentIconUser()
        {
            if (NameLabel.Tag != null)
            {
                ShowFriendship((string)NameLabel.Tag);
            }
        }

        private void ShowListManageBox()
        {
            using (ListManage form = new ListManage(_tw))
            {
                form.ShowDialog(this);
            }
        }

        private void ShowCurrentTweetRtCountBox()
        {
            if (ExistCurrentPost)
            {
                using (FormInfo formInfo = new FormInfo(this, R.RtCountMenuItem_ClickText1, GetRetweet_DoWork))
                {
                    // ダイアログ表示
                    formInfo.ShowDialog();
                    int retweetCount = (int)formInfo.Result;
                    string msg = retweetCount < 0 ?
                        R.RtCountText2 :
                        string.Format("{0}{1}", retweetCount, R.RtCountText1);
                    MessageBox.Show(msg);
                }
            }
        }

        private void ShowtStatusOfCurrentLinkUser()
        {
            ShowUserStatus(GetUserId(), false);
        }

        private void ShowStatusOfCurrentIconUser()
        {
            if (NameLabel.Tag != null)
            {
                ShowUserStatus((string)NameLabel.Tag, false);
            }
        }

        private void ShowStatusOfCurrentTweetUser()
        {
            if (_curPost != null)
            {
                ShowUserStatus(_curPost.ScreenName, false);
            }
        }

        private void TryShowStatusOfCurrentTweetUser()
        {
            ShowUserStatus(_curPost == null ? string.Empty : _curPost.ScreenName);
        }

        private void ShowStatusOfUserSelf()
        {
            ShowUserStatus(_tw.Username, false);
        }

        private void ShowHashManageBox()
        {
            try
            {
                DialogResult rslt = HashMgr.ShowDialog();
                TopMost = _configs.AlwaysTop;
                if (rslt == DialogResult.Cancel)
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            ChangeUseHashTagSetting(false);
        }

        private void TryShowSettingsBox()
        {
            string uid = _tw.Username.ToLower();
            DialogResult result = default(DialogResult);
            try
            {
                result = _settingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return;
            }

            if (result != DialogResult.OK)
            {
                Twitter.AccountState = AccountState.Valid;
                TopMost = _configs.AlwaysTop;
                SaveConfigsAll(false);
                return;
            }

            lock (_syncObject)
            {
                _tw.SetTinyUrlResolve(_configs.TinyUrlResolve);
                _tw.SetRestrictFavCheck(_configs.RestrictFavCheck);
                _tw.ReadOwnPost = _configs.ReadOwnPost;
                _tw.SetUseSsl(_configs.UseSsl);
                ShortUrl.IsResolve = _configs.TinyUrlResolve;
                ShortUrl.IsForceResolve = _configs.ShortUrlForceResolve;
                ShortUrl.SetBitlyId(_configs.BitlyUser);
                ShortUrl.SetBitlyKey(_configs.BitlyPwd);
                HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
                HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);
                HttpConnection.InitializeConnection(_configs.DefaultTimeOut, _configs.SelectedProxyType, _configs.ProxyAddress, _configs.ProxyPort, _configs.ProxyUser, _configs.ProxyPassword);
                CreatePictureServices();
                try
                {
                    if (_configs.TabIconDisp)
                    {
                        ListTab.DrawItem -= ListTab_DrawItem;
                        ListTab.DrawMode = TabDrawMode.Normal;
                        ListTab.ImageList = TabImage;
                    }
                    else
                    {
                        ListTab.DrawItem -= ListTab_DrawItem;
                        ListTab.DrawItem += ListTab_DrawItem;
                        ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                        ListTab.ImageList = null;
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
                    if (!_configs.UnreadManage)
                    {
                        ReadedStripMenuItem.Enabled = false;
                        UnreadStripMenuItem.Enabled = false;
                        if (_configs.TabIconDisp)
                        {
                            foreach (TabPage myTab in ListTab.TabPages)
                            {
                                myTab.ImageIndex = -1;
                            }
                        }
                    }
                    else
                    {
                        ReadedStripMenuItem.Enabled = true;
                        UnreadStripMenuItem.Enabled = true;
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
                    foreach (TabPage mytab in ListTab.TabPages)
                    {
                        DetailsListView lst = (DetailsListView)mytab.Tag;
                        lst.GridLines = _configs.ShowGrid;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Instance"] = "ListTab(ShowGrid)";
                    ex.Data["IsTerminatePermission"] = false;
                    throw;
                }

                PlaySoundMenuItem.Checked = _configs.PlaySound;
                PlaySoundFileMenuItem.Checked = _configs.PlaySound;
                _fntUnread = _configs.FontUnread;
                _clrUnread = _configs.ColorUnread;
                _fntReaded = _configs.FontReaded;
                _clrRead = _configs.ColorReaded;
                _clrFav = _configs.ColorFav;
                _clrOwl = _configs.ColorOWL;
                _clrRetweet = _configs.ColorRetweet;
                _fntDetail = _configs.FontDetail;
                _clrDetail = _configs.ColorDetail;
                _clrDetailLink = _configs.ColorDetailLink;
                _clrDetailBackcolor = _configs.ColorDetailBackcolor;
                _clrSelf = _configs.ColorSelf;
                _clrAtSelf = _configs.ColorAtSelf;
                _clrTarget = _configs.ColorTarget;
                _clrAtTarget = _configs.ColorAtTarget;
                _clrAtFromTarget = _configs.ColorAtFromTarget;
                _clrAtTo = _configs.ColorAtTo;
                _clrListBackcolor = _configs.ColorListBackcolor;
                InputBackColor = _configs.ColorInputBackcolor;
                _clrInputForecolor = _configs.ColorInputFont;
                _fntInputFont = _configs.FontInputFont;
                try
                {
                    if (StatusText.Focused)
                    {
                        StatusText.BackColor = InputBackColor;
                    }

                    StatusText.Font = _fntInputFont;
                    StatusText.ForeColor = _clrInputForecolor;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                DisposeUserBrushes();
                InitUserBrushes();

                try
                {
                    _detailHtmlFormatFooter = GetDetailHtmlFormatFooter(_configs.IsMonospace);
                    _detailHtmlFormatHeader = GetDetailHtmlFormatHeader(_configs.IsMonospace);
                }
                catch (Exception ex)
                {
                    ex.Data["Instance"] = "Font";
                    ex.Data["IsTerminatePermission"] = false;
                    throw;
                }

                try
                {
                    _statuses.SetUnreadManage(_configs.UnreadManage);
                }
                catch (Exception ex)
                {
                    ex.Data["Instance"] = "_statuses";
                    ex.Data["IsTerminatePermission"] = false;
                    throw;
                }

                try
                {
                    foreach (TabPage tb in ListTab.TabPages)
                    {
                        if (_configs.TabIconDisp)
                        {
                            if (_statuses.Tabs[tb.Text].UnreadCount == 0)
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
                            ((DetailsListView)tb.Tag).Font = _fntReaded;
                            ((DetailsListView)tb.Tag).BackColor = _clrListBackcolor;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Instance"] = "ListTab(TabIconDisp no2)";
                    ex.Data["IsTerminatePermission"] = false;
                    throw;
                }

                SetMainWindowTitle();
                SetNotifyIconText();
                _itemCache = null;
                _postCache = null;
                if (_curList != null)
                {
                    _curList.Refresh();
                }

                ListTab.Refresh();
                Outputz.Key = _configs.OutputzKey;
                Outputz.Enabled = _configs.OutputzEnabled;
                switch (_configs.OutputzUrlmode)
                {
                    case OutputzUrlmode.twittercom:
                        Outputz.OutUrl = "http://twitter.com/";
                        break;
                    case OutputzUrlmode.twittercomWithUsername:
                        Outputz.OutUrl = "http://twitter.com/" + _tw.Username;
                        break;
                }

                _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                if (_configs.HotkeyEnabled)
                {
                    ///グローバルホットキーの登録。設定で変更可能にするかも
                    HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                    if ((_configs.HotkeyMod & Keys.Alt) == Keys.Alt)
                    {
                        modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                    }

                    if ((_configs.HotkeyMod & Keys.Control) == Keys.Control)
                    {
                        modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                    }

                    if ((_configs.HotkeyMod & Keys.Shift) == Keys.Shift)
                    {
                        modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                    }

                    if ((_configs.HotkeyMod & Keys.LWin) == Keys.LWin)
                    {
                        modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                    }

                    _hookGlobalHotkey.RegisterOriginalHotkey(_configs.HotkeyKey, _configs.HotkeyValue, modKey);
                }

                if (uid != _tw.Username)
                {
                    GetFollowers();
                }

                SetImageServiceCombo();
                if (_configs.IsNotifyUseGrowl)
                {
                    _growlHelper.RegisterGrowl();
                }

                try
                {
                    StatusText_TextChangedExtracted();
                }
                catch (Exception)
                {
                }
            }

            Twitter.AccountState = AccountState.Valid;
            TopMost = _configs.AlwaysTop;
            SaveConfigsAll(false);
        }

        private void SearchSelectedTextAtCurrentTab()
        {
            // 発言詳細の選択文字列で現在のタブを検索
            string txt = WebBrowser_GetSelectionText(PostBrowser);
            if (!string.IsNullOrEmpty(txt))
            {
                _searchDialog.SWord = txt;
                _searchDialog.CheckCaseSensitive = false;
                _searchDialog.CheckRegex = false;
                SearchInTab(_searchDialog.SWord, _searchDialog.CheckCaseSensitive, _searchDialog.CheckRegex, InTabSearchType.NextSearch);
            }
        }

        private void TrySearchWordInTabToBottom()
        {
            // 次を検索
            if (string.IsNullOrEmpty(_searchDialog.SWord))
            {
                TrySearchWordInTab();
            }
            else
            {
                SearchInTab(_searchDialog.SWord, _searchDialog.CheckCaseSensitive, _searchDialog.CheckRegex, InTabSearchType.NextSearch);
            }
        }

        private void TrySearchWordInTabToTop()
        {
            // 前を検索
            if (string.IsNullOrEmpty(_searchDialog.SWord))
            {
                if (!TryGetSearchCondition())
                {
                    return;
                }
            }

            SearchInTab(_searchDialog.SWord, _searchDialog.CheckCaseSensitive, _searchDialog.CheckRegex, InTabSearchType.PrevSearch);
        }

        private bool TryGetSearchCondition()
        {
            if (_searchDialog.ShowDialog() == DialogResult.Cancel)
            {
                TopMost = _configs.AlwaysTop;
                return false;
            }

            TopMost = _configs.AlwaysTop;
            if (string.IsNullOrEmpty(_searchDialog.SWord))
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

            SearchInTab(_searchDialog.SWord, _searchDialog.CheckCaseSensitive, _searchDialog.CheckRegex, InTabSearchType.DialogSearch);
        }

        private void TryFollowUserOfCurrentTweet()
        {
            FollowCommand(_curPost != null ? _curPost.ScreenName : string.Empty);
        }

        private void TryFollowUserOfCurrentLinkUser()
        {
            FollowCommand(GetUserId());
        }

        private void TryFollowUserOfCurrentIconUser()
        {
            if (NameLabel.Tag != null)
            {
                FollowCommand((string)NameLabel.Tag);
            }
        }

        private void DeleteSelectedTab(bool fromMenuBar)
        {
            if (string.IsNullOrEmpty(_rclickTabName) || fromMenuBar)
            {
                _rclickTabName = ListTab.SelectedTab.Text;
            }

            RemoveSpecifiedTab(_rclickTabName, true);
            SaveConfigsTabs();
        }

        private void AddNewTab()
        {
            string tabName = _statuses.GetUniqueTabName();
            TabUsageType tabUsage = default(TabUsageType);
            if (!TryGetTabInfo(ref tabName, ref tabUsage, showusage: true))
            {
                return;
            }

            TopMost = _configs.AlwaysTop;
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

            if (!_statuses.AddTab(tabName, tabUsage, list) || !AddNewTab(tabName, false, tabUsage, list))
            {
                string tmp = string.Format(R.AddTabMenuItem_ClickText1, tabName);
                MessageBox.Show(tmp, R.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // 成功
            SaveConfigsTabs();
            if (tabUsage == TabUsageType.PublicSearch)
            {
                ListTabSelect(ListTab.TabPages.Count - 1);
                ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }

            if (tabUsage == TabUsageType.Lists)
            {
                ListTabSelect(ListTab.TabPages.Count - 1);
                GetTimeline(WorkerType.List, 1, 0, tabName);
            }
        }

        private void ChangeAllrepliesSetting(bool useAllReply)
        {
            _tw.AllAtReply = useAllReply;
            SetModifySettingCommon(true);
            _tw.ReconnectUserStream();
        }

        private void OpenFavorarePageOfSelectedTweetUser()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                OpenFavorarePageOfUser(_statuses.Item(_curTab.Text, _curList.SelectedIndices[0]).ScreenName);
            }
        }

        private void OpenFavorarePageOfSelf()
        {
            OpenFavorarePageOfUser(_tw.Username);
        }

        private void TryOpenFavorarePageOfCurrentTweetUser()
        {
            string id = GetUserIdFromCurPostOrInput("Show Favstar");
            if (!string.IsNullOrEmpty(id))
            {
                OpenFavorarePageOfUser(id);
            }
        }

        private void OpenFavorarePageOfUser(string id)
        {
            OpenUriAsync(string.Format("{0}users/{1}/recent", R.FavstarUrl, id));
        }

        private void ExitApplication()
        {
            MyCommon.IsEnding = true;
            Close();
        }

        private void TryRestartApplication()
        {
            try
            {
                ExitApplication();
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

            var progressMessage = StatusLabel.Text;
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
                    progressMessage = R.PostWorker_RunWorkerCompletedText4;
                }
            }
            else
            {
                if (msg.Length > 0)
                {
                    progressMessage = msg;
                }
            }

            StatusLabel.Text = progressMessage;
        }

        private void ChangeUseHashTagSetting(bool toggle = true)
        {
            if (toggle)
            {
                HashMgr.ToggleHash();
            }

            if (!string.IsNullOrEmpty(HashMgr.UseHash))
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }

            _modifySettingCommon = true;
            StatusText_TextChangedExtracted();
        }

        private void ChangeWindowState()
        {
            if ((WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized)
                && Visible && ReferenceEquals(ActiveForm, this))
            {
                // アイコン化
                Visible = false;
            }
            else if (ActiveForm == null)
            {
                Visible = true;
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }

                Activate();
                BringToFront();
                StatusText.Focus();
            }
        }

        private void AddIdFilteringRuleFromSelectedTweets()
        {
            // 未選択なら処理終了
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            var names = _curList.SelectedIndices.Cast<int>()
                .Select(idx => _statuses.Item(_curTab.Text, idx))
                .Select(pc => pc.IsRetweeted ? pc.RetweetedBy : pc.ScreenName);
            TryAddIdsFilter(names);
        }

        private void ApplyNewFilters()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _itemCache = null;
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                _statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    if (_statuses.ContainsTab(tb.Text))
                    {
                        ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                        if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (_configs.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (_configs.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }

                if (!_configs.TabIconDisp)
                {
                    ListTab.Refresh();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void AddIdFilteringRuleFromCurrentTweet()
        {
            TryAddIdFilter(GetUserId());
        }

        private void TryAddIdFilter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            TryAddIdsFilter(new[] { name });
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
            if (!SelectTab(ref tabName))
            {
                return;
            }

            bool mv = false;
            bool mk = false;
            GetMoveOrCopy(ref mv, ref mk);

            _statuses.Tabs[tabName].AddFilters(names.Select(name => new FiltersClass
            {
                NameFilter = name,
                SearchBoth = true,
                MoveFrom = mv,
                SetMark = mk,
                UseRegex = false,
                SearchUrl = false
            }));
            SetModifySettingAtId(AtIdSupl.AddRangeItem(names.Select(name => "@" + name)));

            ApplyNewFilters();
            SaveConfigsTabs();
        }

        private void TryOpenCurrentTweetIconUrl()
        {
            if (_curPost == null)
            {
                return;
            }

            OpenUriAsync(_curPost.NormalImageUrl);
        }

        private void CancelPostImageSelecting()
        {
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
            ImageSelectedPicture.Tag = UploadFileType.Invalid;
            ImagefilePathText.CausesValidation = false;
            TimelinePanel.Visible = true;
            TimelinePanel.Enabled = true;
            ImageSelectionPanel.Visible = false;
            ImageSelectionPanel.Enabled = false;
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
            ImagefilePathText.CausesValidation = true;
        }

        private void ToggleImageSelectorView()
        {
            if (ImageSelectionPanel.Visible)
            {
                CancelPostImageSelecting();
            }
            else
            {
                ImageSelectionPanel.Visible = true;
                ImageSelectionPanel.Enabled = true;
                TimelinePanel.Visible = false;
                TimelinePanel.Enabled = false;
                ImagefilePathText.Focus();
            }
        }

        private void TryChangeImageUploadService()
        {
            if (ImageSelectedPicture.Tag == null || string.IsNullOrEmpty(ImageService))
            {
                return;
            }

            try
            {
                FileInfo fi = new FileInfo(ImagefilePathText.Text.Trim());
                if (!_pictureServices[ImageService].CheckValidFilesize(fi.Extension, fi.Length))
                {
                    ImagefilePathText.Text = string.Empty;
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = UploadFileType.Invalid;
                }
            }
            catch (Exception)
            {
            }

            _modifySettingCommon = true;
            SaveConfigsAll(false);
            if (ImageService == "Twitter")
            {
                StatusText_TextChangedExtracted();
            }
        }

        private void TrySearchAndFocusUnreadTweet()
        {
            if (ImageSelectionPanel.Enabled)
            {
                return;
            }

            // 現在タブから最終タブまで探索
            int idx = -1;
            DetailsListView lst = null;
            TabControl.TabPageCollection pages = ListTab.TabPages;
            foreach (var i in Enumerable.Range(0, pages.Count).Select(i => (i + pages.IndexOf(_curTab)) % pages.Count))
            {
                // 未読Index取得
                idx = _statuses.GetOldestUnreadIndex(pages[i].Text);
                if (idx > -1)
                {
                    ListTab.SelectedIndex = i;
                    lst = (DetailsListView)pages[i].Tag;
                    break;
                }
            }

            // 全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                ListTab.SelectedIndex = 0;
                lst = (DetailsListView)pages[0].Tag;
                if (_statuses.SortOrder == SortOrder.Ascending)
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
                SelectListItem(lst, idx);
                if (_statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if ((_statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - _iconSz - 10)
                        || (_statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < _iconSz + 10))
                    {
                        MoveTop();
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
            _cfgCommon.ListLock = LockListFileMenuItem.Checked = ListLockMenuItem.Checked = locked;
            SetModifySettingCommon(true);
        }

        private void ShowListSelectFormForCurrentTweetUser()
        {
            if (_curPost != null)
            {
                ShowListSelectForm(_curPost.ScreenName);
            }
        }

        private void ShowListSelectForm(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            if (_statuses.SubscribableLists.Count == 0)
            {
                string res = _tw.GetListsApi();
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Failed to get lists. (" + res + ")");
                    return;
                }
            }

            using (MyLists listSelectForm = new MyLists(user, _tw))
            {
                listSelectForm.ShowDialog(this);
            }
        }

        private void SetFocusToMainMenu()
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            MenuStrip1.Tag = new object();
            MenuStrip1.Select();
        }

        private void SetFocusFromMainMenu()
        {
            if (Tag != null)
            {
                // 設定された戻り先へ遷移
                if (ReferenceEquals(Tag, ListTab.SelectedTab))
                {
                    ((Control)ListTab.SelectedTab.Tag).Select();
                }
                else
                {
                    ((Control)Tag).Select();
                }
            }
            else
            {
                // 戻り先が指定されていない (初期状態) 場合はタブに遷移
                if (ListTab.SelectedIndex > -1 && ListTab.SelectedTab.HasChildren)
                {
                    Tag = ListTab.SelectedTab.Tag;
                    ((Control)Tag).Select();
                }
            }

            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            MenuStrip1.Tag = null;
        }

        private void TryOpenCurListSelectedUserFavorites()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                OpenUriAsync(string.Format("https://twitter.com/{0}/favorites", GetCurTabPost(_curList.SelectedIndices[0]).ScreenName));
            }
        }

        private void TryOpenCurListSelectedUserHome()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                OpenUriAsync("https://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName);
            }
            else if (_curList.SelectedIndices.Count == 0)
            {
                OpenUriAsync("https://twitter.com/");
            }
        }

        private void ChangeStatusTextMultilineState(bool multi)
        {
            // 発言欄複数行
            StatusText.Multiline = multi;
            _cfgLocal.StatusMultiline = multi;
            int baseHeight = SplitContainer2.Height - SplitContainer2.SplitterWidth;
            baseHeight -= multi ? _mySpDis2 : SplitContainer2.Panel2MinSize;

            SplitContainer2.SplitterDistance = baseHeight < 0 ? 0 : baseHeight;
            SetModifySettingLocal(true);
        }

        private void ChangeNewPostPopupSetting(bool popup)
        {
            _cfgCommon.NewAllPop = NewPostPopMenuItem.Checked = NotifyFileMenuItem.Checked = popup;
            SetModifySettingCommon(true);
        }

        private void ChangeNotifySetting(bool notify)
        {
            NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked = notify;
            if (string.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            _statuses.Tabs[_rclickTabName].Notify = notify;
            SaveConfigsTabs();
        }

        private void ActivateMainForm()
        {
            Visible = true;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Activate();
            BringToFront();
        }

        private void TryOpenUrlInCurrentTweet()
        {
            if (PostBrowser.Document.Links.Count < 1)
            {
                return;
            }

            _urlDialog.ClearUrl();
            string openUrlStr = string.Empty;
            foreach (HtmlElement linkElm in PostBrowser.Document.Links)
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
                    _urlDialog.AddUrl(new OpenUrlItem(linkText, openUrlStr, href));
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
                if (PostBrowser.Document.Links.Count != 1)
                {
                    openUrlStr = _urlDialog.ShowDialog() == DialogResult.OK ? _urlDialog.SelectedUrl : string.Empty;
                }
            }
            catch (Exception)
            {
                return;
            }

            TopMost = _configs.AlwaysTop;
            if (string.IsNullOrEmpty(openUrlStr))
            {
                return;
            }

            if (IsTwitterSearchUrl(openUrlStr))
            {
                // ハッシュタグの場合は、タブで開く
                string urlStr = HttpUtility.UrlDecode(openUrlStr);
                string hash = urlStr.Substring(urlStr.IndexOf("#"));
                HashSupl.AddItem(hash);
                HashMgr.AddHashToHistory(hash.Trim(), false);
                AddNewTabForSearch(hash);
                return;
            }

            if (_configs.OpenUserTimeline)
            {
                Match m = Regex.Match(openUrlStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                if (m.Success && IsTwitterId(m.Result("${ScreenName}")))
                {
                    AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    return;
                }
            }

            openUrlStr = openUrlStr.Replace("://twitter.com/search?q=#", "://twitter.com/search?q=%23");
            OpenUriAsync(openUrlStr);
        }

        private void ChangePlaySoundSetting(bool play)
        {
            _configs.PlaySound = PlaySoundFileMenuItem.Checked = PlaySoundMenuItem.Checked = play;
            SetModifySettingCommon(true);
        }

        private void PostBrowser_NavigatedExtracted(Uri url)
        {
            if (url.AbsoluteUri != "about:blank")
            {
                DispSelectedPost();
                OpenUriAsync(url.OriginalString);
            }
        }

        private bool NavigateNextUrl(Uri url)
        {
            if (url.Scheme == "data")
            {
                StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
                return true;
            }

            string absoluteUri = url.AbsoluteUri;
            if (absoluteUri == "about:blank")
            {
                return false;
            }

            if (IsTwitterSearchUrl(absoluteUri))
            {
                // ハッシュタグの場合は、タブで開く
                string urlStr = HttpUtility.UrlDecode(absoluteUri);
                string hash = urlStr.Substring(urlStr.IndexOf("#"));
                HashSupl.AddItem(hash);
                HashMgr.AddHashToHistory(hash.Trim(), false);
                AddNewTabForSearch(hash);
                return true;
            }

            Match m = Regex.Match(absoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
            string urlOriginalString = url.OriginalString;
            if (!m.Success)
            {
                OpenUriAsync(urlOriginalString);
                return true;
            }

            string screenName = m.Result("${ScreenName}");
            if (!IsTwitterId(screenName))
            {
                OpenUriAsync(urlOriginalString);
                return true;
            }

            // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
            bool isCtrlKeyDown = IsKeyDown(Keys.Control);
            bool isOpenInTab = _configs.OpenUserTimeline;
            if ((isOpenInTab && !isCtrlKeyDown) || (!isOpenInTab && isCtrlKeyDown))
            {
                AddNewTabForUserTimeline(screenName);
            }
            else
            {
                OpenUriAsync(urlOriginalString);
            }

            return true;
        }

        private void ChangeStatusLabelUrlTextByPostBrowserStatusText()
        {
            try
            {
                string postBrowserStatusText1 = PostBrowser.StatusText;
                if (postBrowserStatusText1.StartsWith("http")
                    || postBrowserStatusText1.StartsWith("ftp")
                    || postBrowserStatusText1.StartsWith("data"))
                {
                    StatusLabelUrl.Text = postBrowserStatusText1.Replace("&", "&&");
                }

                if (string.IsNullOrEmpty(postBrowserStatusText1))
                {
                    SetStatusLabelUrl();
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
            if (StatusText.Text.StartsWith("D ") || StatusText.Text.StartsWith("d "))
            {
                // DM時は何もつけない
                footer = string.Empty;
                return;
            }

            // ハッシュタグ
            string hash = string.Empty;
            if (HashMgr.IsNotAddToAtReply)
            {
                if (_replyToId == 0 && string.IsNullOrEmpty(_replyToName))
                {
                    hash = HashMgr.UseHash;
                }
            }
            else
            {
                hash = HashMgr.UseHash;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                if (HashMgr.IsHead)
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
                if (_configs.UseRecommendStatus)
                {
                    // 推奨ステータスを使用する
                    footer += _configs.RecommendStatusText;
                }
                else
                {
                    // テキストボックスに入力されている文字列を使用する
                    footer += " " + _configs.Status.Trim();
                }
            }
        }

        private bool GetPostImageInfo(out string imgService, out string imgPath)
        {
            imgService = imgPath = string.Empty;
            if (!ImageSelectionPanel.Visible)
            {
                return true;
            }

            // 画像投稿
            if (ReferenceEquals(ImageSelectedPicture.Image, ImageSelectedPicture.InitialImage)
                || ImageServiceCombo.SelectedIndex < 0
                || string.IsNullOrEmpty(ImagefilePathText.Text))
            {
                MessageBox.Show(R.PostPictureWarn1, R.PostPictureWarn2);
                return false;
            }

            var rslt = MessageBox.Show(R.PostPictureConfirm1, R.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rslt == DialogResult.Cancel)
            {
                TimelinePanel.Visible = true;
                TimelinePanel.Enabled = true;
                ImageSelectionPanel.Visible = false;
                ImageSelectionPanel.Enabled = false;
                if (_curList != null)
                {
                    _curList.Focus();
                }

                return false;
            }

            imgService = ImageServiceCombo.Text;
            imgPath = ImagefilePathText.Text;

            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
            ImagefilePathText.Text = string.Empty;
            TimelinePanel.Visible = true;
            TimelinePanel.Enabled = true;
            ImageSelectionPanel.Visible = false;
            ImageSelectionPanel.Enabled = false;
            if (_curList != null)
            {
                _curList.Focus();
            }

            return true;
        }

        private void TryPostTweet()
        {
            string statusTextTextTrim = StatusText.Text.Trim();
            if (statusTextTextTrim.Length == 0)
            {
                if (!ImageSelectionPanel.Enabled)
                {
                    RefreshTab();
                    return;
                }
            }

            if (ExistCurrentPost && statusTextTextTrim == string.Format("RT @{0}: {1}", _curPost.ScreenName, _curPost.TextFromApi))
            {
                DialogResult res = MessageBox.Show(string.Format(R.PostButton_Click1, Environment.NewLine), "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        DoReTweetOfficial(false);
                        StatusText.Text = string.Empty;
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            _postHistory[_postHistory.Count - 1] = new PostingStatus(statusTextTextTrim, _replyToId, _replyToName);

            if (_configs.Nicoms)
            {
                StatusText.SelectionStart = StatusText.Text.Length;
                ConvertUrl(UrlConverter.Nicoms);
            }

            StatusText.SelectionStart = StatusText.Text.Length;
            CheckReplyTo(StatusText.Text);

            // 整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = statusTextTextTrim;
            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase)
                    && !tmpStatus.EndsWith(" ."))
                {
                    adjustCount += 2;
                }
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                adjustCount += Regex.Matches(tmpStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+").Count;
            }

            bool isCutOff = false;
            bool isRemoveFooter = IsKeyDown(Keys.Shift);
            if (StatusText.Multiline && !_configs.PostCtrlEnter)
            {
                // 複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = IsKeyDown(Keys.Control);
            }

            if (_configs.PostShiftEnter)
            {
                isRemoveFooter = IsKeyDown(Keys.Control);
            }

            if (!isRemoveFooter && (StatusText.Text.Contains("RT @") || StatusText.Text.Contains("QT @")))
            {
                isRemoveFooter = true;
            }

            if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
            {
                if (MessageBox.Show(R.PostLengthOverMessage1, R.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    isCutOff = true;
                    if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
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
            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(postStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase)
                    && !postStatus.EndsWith(" ."))
                {
                    postStatus += " .";
                }
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(postStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+");
                if (mc2.Success)
                {
                    postStatus = Regex.Replace(postStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+", "$& ");
                }
            }

            if (IdeographicSpaceToSpaceToolStripMenuItem.Checked)
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

            RunAsync(new GetWorkerArg
            {
                WorkerType = WorkerType.PostMessage,
                PStatus = new PostingStatus
                {
                    ImagePath = imgPath,
                    ImageService = imgService,
                    InReplyToId = _replyToId,
                    InReplyToName = _replyToName,
                    Status = postStatus
                }
            });

            // Google検索（試験実装）
            if (StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && statusTextTextTrim.Length > 7)
            {
                OpenUriAsync(string.Format(R.SearchItem2Url, HttpUtility.UrlEncode(StatusText.Text.Substring(7))));
            }

            ClearReplyToInfo();
            StatusText.Text = string.Empty;
            _postHistory.Add(new PostingStatus());
            _postHistoryIndex = _postHistory.Count - 1;
            if (!ToolStripFocusLockMenuItem.Checked)
            {
                ((Control)ListTab.SelectedTab.Tag).Focus();
            }

            _urlUndoBuffer = null;
            UrlUndoToolStripMenuItem.Enabled = false; // Undoをできないように設定
        }

        private void FocusCurrentPublicSearchTabSearchInput()
        {
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != TabUsageType.PublicSearch)
                {
                    return;
                }

                ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void ChangeSelectetdTweetReadState(bool read)
        {
            _curList.BeginUpdate();
            if (_configs.UnreadManage)
            {
                foreach (int idx in _curList.SelectedIndices)
                {
                    _statuses.SetReadAllTab(read, _curTab.Text, idx);
                }
            }

            foreach (int idx in _curList.SelectedIndices)
            {
                ChangeCacheStyleRead(read, idx, _curTab);
            }

            ColorizeList();
            _curList.EndUpdate();
        }

        private void ChangeTabsIconToRead()
        {
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
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
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (tb.ImageIndex == -1)
                    {
                        tb.ImageIndex = 0;
                    }
                }
            }
        }

        private void ChangeSelectedTweetReadStateToRead()
        {
            ChangeSelectetdTweetReadState(true);
            if (_configs.TabIconDisp)
            {
                ChangeTabsIconToRead();
            }
            else
            {
                ListTab.Refresh();
            }
        }

        private void ChangeSelectedTweetReadSateToUnread()
        {
            ChangeSelectetdTweetReadState(false);
            if (_configs.TabIconDisp)
            {
                ChangeTabsIconToUnread();
            }
            else
            {
                ListTab.Refresh();
            }
        }

        private void TryUnfollowCurrentTweetUser()
        {
            RemoveCommand(_curPost != null ? _curPost.ScreenName : string.Empty, false);
        }

        private void TryUnfollowUserInCurrentTweet()
        {
            RemoveCommand(GetUserId(), false);
        }

        private void TryUnfollowCurrentIconUser()
        {
            if (NameLabel.Tag != null)
            {
                RemoveCommand((string)NameLabel.Tag, false);
            }
        }

        private void TrySaveCurrentTweetUserIcon()
        {
            if (_curPost == null)
            {
                return;
            }

            string name = _curPost.ImageUrl;
            SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);
            if (SaveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                using (Image orgBmp = new Bitmap(_iconDict[name]))
                using (Bitmap bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp2))
                    {
                        g.InterpolationMode = InterpolationMode.High;
                        g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                    }

                    bmp2.Save(SaveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                // 処理中にキャッシュアウトする可能性あり
                Debug.Write(ex);
            }
        }

        private void TrySaveLog()
        {
            DialogResult rslt = MessageBox.Show(string.Format(R.SaveLogMenuItem_ClickText1, Environment.NewLine), R.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            SaveFileDialog1.FileName = string.Format("HoehoePosts{0:yyMMdd-HHmmss}.tsv", DateTime.Now);
            SaveFileDialog1.InitialDirectory = MyCommon.AppDir;
            SaveFileDialog1.Filter = R.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = R.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames)
                {
                    return;
                }

                var idxs = rslt == DialogResult.Yes ?
                    Enumerable.Range(0, _curList.VirtualListSize) :
                    _curList.SelectedIndices.Cast<int>();
                var lines = idxs
                    .Select(idx => _statuses.Item(_curTab.Text, idx))
                    .Select(post => post.MakeTsvLine());
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    foreach (var line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }

            TopMost = _configs.AlwaysTop;
        }

        private void SaveCurrentTweetUserOriginalSizeIcon()
        {
            if (_curPost == null)
            {
                return;
            }

            string name = _curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));
            SaveFileDialog1.FileName = name.Substring(0, name.Length - 8);
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
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

            AddNewTabForSearch("@" + id);
        }

        private void AddSearchTabForAtUserOfCurrentTweet()
        {
            if (NameLabel.Tag != null)
            {
                AddNewTabForAtUserSearch((string)NameLabel.Tag);
            }
        }

        private void AddSearchTabForAtUserInCurrentTweet()
        {
            AddNewTabForAtUserSearch(GetUserId());
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
            if (NameLabel.Tag != null)
            {
                AddNewTabForUserTimeline((string)NameLabel.Tag);
            }
        }

        private void AddTimelineTabForUserInCurrentTweet()
        {
            AddNewTabForUserTimeline(GetUserId());
        }

        private void SelectAllItemInFocused()
        {
            if (StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                _curList.SelectAllItem();
            }
        }

        private void TryCopySelectionInPostBrowser()
        {
            CopyToClipboard(WebBrowser_GetSelectionText(PostBrowser));
        }

        private void SetStatusLabelApiLuncher()
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new SetStatusLabelApiDelegate(SetStatusLabelApi));
                }
                else
                {
                    SetStatusLabelApi();
                }
            }
            catch (ObjectDisposedException)
            {
                // todo: do something?
            }
            catch (InvalidOperationException)
            {
                // todo: do something?
            }
        }

        private void AddRelatedStatusesTab()
        {
            if (!ExistCurrentPost || _curPost.IsDm)
            {
                return;
            }

            // PublicSearchも除外した方がよい？
            if (_statuses.GetTabByType(TabUsageType.Related) == null)
            {
                const string TabName = "Related Tweets";
                string newTabName = TabName;
                if (AddNewTab(newTabName, false, TabUsageType.Related))
                {
                    _statuses.AddTab(newTabName, TabUsageType.Related, null);
                }
                else
                {
                    for (int i = 2; i <= 100; i++)
                    {
                        newTabName = TabName + i.ToString();
                        if (AddNewTab(newTabName, false, TabUsageType.Related))
                        {
                            _statuses.AddTab(newTabName, TabUsageType.Related, null);
                            break;
                        }
                    }
                }

                _statuses.GetTabByName(newTabName).UnreadManage = false;
                _statuses.GetTabByName(newTabName).Notify = false;
            }

            TabClass tb = _statuses.GetTabByType(TabUsageType.Related);
            tb.RelationTargetPost = _curPost;
            ClearTab(tb.TabName, false);
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                if (tb.TabName == ListTab.TabPages[i].Text)
                {
                    ListTabSelect(i);
                    break;
                }
            }

            GetTimeline(WorkerType.Related, 1, 1, tb.TabName);
        }

        private void ChangeCurrentTabSoundFile(string soundfile)
        {
            if (_soundfileListup || string.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            _statuses.Tabs[_rclickTabName].SoundFile = soundfile;
            SaveConfigsTabs();
        }

        private void TryCopySourceName()
        {
            CopyToClipboard(SourceLinkLabel.Text);
        }

        private void TryOpenSourceLink()
        {
            string link = (string)SourceLinkLabel.Tag;
            if (!string.IsNullOrEmpty(link))
            {
                OpenUriAsync(link);
            }
        }

        private void ChangeStatusLabelUrlText(string link, bool updateEmpty = false)
        {
            if (string.IsNullOrEmpty(link))
            {
                if (updateEmpty)
                {
                    StatusLabelUrl.Text = string.Empty;
                }
            }
            else
            {
                StatusLabelUrl.Text = link;
            }
        }

        private void TryCopySourceUrl()
        {
            CopyToClipboard(Convert.ToString(SourceLinkLabel.Tag));
        }

        private void TryOpenSelectedTweetWebPage()
        {
            if (_curList.SelectedIndices.Count > 0 && _statuses.Tabs[_curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                PostClass post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[0]);
                OpenUriAsync(string.Format("https://twitter.com/{0}/status/{1}", post.ScreenName, post.OriginalStatusId));
            }
        }

        private void StatusText_EnterExtracted()
        {
            /// フォーカスの戻り先を StatusText に設定
            Tag = StatusText;
            StatusText.BackColor = InputBackColor;
        }

        private void ShowSupplementBox(char keyChar)
        {
            if (keyChar == '@')
            {
                // @マーク
                if (!_configs.UseAtIdSupplement)
                {
                    return;
                }

                int cnt = AtIdSupl.ItemCount;
                ShowSuplDialog(StatusText, AtIdSupl);
                if (cnt != AtIdSupl.ItemCount)
                {
                    _modifySettingAtId = true;
                }
            }
            else if (keyChar == '#')
            {
                if (!_configs.UseHashSupplement)
                {
                    return;
                }

                ShowSuplDialog(StatusText, HashSupl);
            }
        }

        private void StatusText_LeaveExtracted()
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (ListTab.SelectedTab != null && MenuStrip1.Tag == null)
            {
                Tag = ListTab.SelectedTab.Tag;
            }

            StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void ChangeStatusTextMultiline(bool isMultiLine)
        {
            StatusText.ScrollBars = isMultiLine ? ScrollBars.Vertical : ScrollBars.None;
            _modifySettingLocal = true;
        }

        private void StatusText_TextChangedExtracted()
        {
            // 文字数カウント
            int len = GetRestStatusCount(true, false);
            lblLen.Text = len.ToString();
            StatusText.ForeColor = len < 0 ? Color.Red : _clrInputForecolor;
            if (string.IsNullOrEmpty(StatusText.Text))
            {
                ClearReplyToInfo();
            }
        }

        private void ChangeUserStreamStatus()
        {
            MenuItemUserStream.Enabled = false;
            if (StopRefreshAllMenuItem.Checked)
            {
                StopRefreshAllMenuItem.Checked = false;
                return;
            }

            if (_isActiveUserstream)
            {
                _tw.StopUserStream();
            }
            else
            {
                _tw.StartUserStream();
            }
        }

        private void AddFilteringRuleFromSelectedTweet()
        {
            // 選択発言を元にフィルタ追加
            foreach (int idx in _curList.SelectedIndices)
            {
                // タブ選択（or追加）
                string tabName = string.Empty;
                if (!SelectTab(ref tabName))
                {
                    return;
                }

                _fltDialog.SetCurrent(tabName);
                PostClass statusesItem = _statuses.Item(_curTab.Text, idx);
                string scname = statusesItem.IsRetweeted ? statusesItem.RetweetedBy : statusesItem.ScreenName;
                _fltDialog.AddNewFilter(scname, statusesItem.TextFromApi);
                _fltDialog.ShowDialog();
                TopMost = _configs.AlwaysTop;
            }

            ApplyNewFilters();
            SaveConfigsTabs();
            if (ListTab.SelectedTab != null && ((DetailsListView)ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                _curPost = _statuses.Item(ListTab.SelectedTab.Text, ((DetailsListView)ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

        private void RenameCurrentTabName()
        {
            if (string.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            RenameTab(ref _rclickTabName);
        }

        private void RenameSelectedTabName()
        {
            string tn = ListTab.SelectedTab.Text;
            RenameTab(ref tn);
        }

        private void DecrementTimer(ref int counter)
        {
            if (counter > 0)
            {
                Interlocked.Decrement(ref counter);
            }
        }

        private bool ResetWorkerTimer(ref int counter, int initailValue, WorkerType worker, bool reset, bool usflag = false)
        {
            if (reset || (counter < 1 && initailValue > 0))
            {
                Interlocked.Exchange(ref counter, initailValue);
                if (!usflag && !reset)
                {
                    GetTimeline(worker);
                }

                return false;
            }

            return reset;
        }

        private void TimerTimeline_ElapsedExtracted()
        {
            DecrementTimer(ref _timerHomeCounter);
            DecrementTimer(ref _timerMentionCounter);
            DecrementTimer(ref _timerDmCounter);
            DecrementTimer(ref _timerPubSearchCounter);
            DecrementTimer(ref _timerUserTimelineCounter);
            DecrementTimer(ref _timerListsCounter);
            DecrementTimer(ref _timerUsCounter);
            DecrementTimer(ref _timerRefreshFollowers);

            // 'タイマー初期化
            _resetTimers.Timeline = ResetWorkerTimer(ref _timerHomeCounter, _configs.TimelinePeriodInt, WorkerType.Timeline, _resetTimers.Timeline, _tw.IsUserstreamDataReceived);
            _resetTimers.Reply = ResetWorkerTimer(ref _timerMentionCounter, _configs.ReplyPeriodInt, WorkerType.Reply, _resetTimers.Reply, _tw.IsUserstreamDataReceived);
            _resetTimers.DirectMessage = ResetWorkerTimer(ref _timerDmCounter, _configs.DMPeriodInt, WorkerType.DirectMessegeRcv, _resetTimers.DirectMessage, _tw.IsUserstreamDataReceived);
            _resetTimers.PublicSearch = ResetWorkerTimer(ref _timerPubSearchCounter, _configs.PubSearchPeriodInt, WorkerType.PublicSearch, _resetTimers.PublicSearch);
            _resetTimers.UserTimeline = ResetWorkerTimer(ref _timerUserTimelineCounter, _configs.UserTimelinePeriodInt, WorkerType.UserTimeline, _resetTimers.UserTimeline);
            _resetTimers.Lists = ResetWorkerTimer(ref _timerListsCounter, _configs.ListsPeriodInt, WorkerType.List, _resetTimers.Lists);
            if (_resetTimers.UserStream || (_timerUsCounter <= 0 && _configs.UserstreamPeriodInt > 0))
            {
                Interlocked.Exchange(ref _timerUsCounter, _configs.UserstreamPeriodInt);
                if (_isActiveUserstream)
                {
                    RefreshTimeline(true);
                }

                _resetTimers.UserStream = false;
            }

            if (_timerRefreshFollowers < 1)
            {
                Interlocked.Exchange(ref _timerRefreshFollowers, 6 * 3600);
                GetFollowers();
                GetTimeline(WorkerType.Configuration);
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(TrimPostChain));
                }
            }

            if (!_isOsResumed)
            {
                return;
            }

            Interlocked.Increment(ref _timerResumeWait);
            if (_timerResumeWait > 30)
            {
                _isOsResumed = false;
                Interlocked.Exchange(ref _timerResumeWait, 0);
                GetTimeline(WorkerType.Timeline);
                GetTimeline(WorkerType.Reply);
                GetTimeline(WorkerType.DirectMessegeRcv);
                GetTimeline(WorkerType.PublicSearch);
                GetTimeline(WorkerType.UserTimeline);
                GetTimeline(WorkerType.List);
                GetFollowers();
                GetTimeline(WorkerType.Configuration);
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(TrimPostChain));
                }
            }
        }

        private void ChangeAutoUrlConvertFlag(bool autoConvert)
        {
            _configs.UrlConvertAuto = autoConvert;
        }

        private bool TryGetTabInfo(ref string name, ref TabUsageType usageType, string title = "", string desc = "", bool showusage = false)
        {
            using (var form = new InputTabName { TabName = name })
            {
                form.SetFormTitle(title);
                form.SetFormDescription(desc);
                form.SetIsShowUsage(showusage);
                var result = form.ShowDialog();
                if (result != DialogResult.OK)
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
            return TryGetTabInfo(ref val, ref tmp, title, desc);
        }

        private void ChangeTrackWordStatus()
        {
            if (!TrackToolStripMenuItem.Checked)
            {
                _tw.TrackWord = string.Empty;
                _tw.ReconnectUserStream();
            }
            else
            {
                string q = _prevTrackWord;
                if (!TryUserInputText(ref q, "Input track word", "Track word"))
                {
                    TrackToolStripMenuItem.Checked = false;
                    return;
                }

                _prevTrackWord = q;
                if (_prevTrackWord != _tw.TrackWord)
                {
                    _tw.TrackWord = _prevTrackWord;
                    TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(_prevTrackWord);
                    _tw.ReconnectUserStream();
                }
            }

            _modifySettingCommon = true;
        }

        private void TranslateCurrentTweet()
        {
            if (!ExistCurrentPost)
            {
                return;
            }

            DoTranslation(_curPost.TextFromApi);
        }

        private void ChangeUserStreamStatusDisplay(bool start)
        {
            MenuItemUserStream.Text = start ? "&UserStream ▶" : "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = start ? "&Stop" : "&Start";
            StopToolStripMenuItem.Enabled = true;
            StatusLabel.Text = start ? "UserStream Started." : "UserStream Stopped.";
        }

        private void ActivateMainFormControls()
        {
            // 画面がアクティブになったら、発言欄の背景色戻す
            if (StatusText.Focused)
            {
                StatusText_EnterExtracted();
            }
        }

        private void TweenMain_ClientSizeChangedExtracted()
        {
            if (!_initialLayout && Visible)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    _mySize = ClientSize;
                    _mySpDis = SplitContainer1.SplitterDistance;
                    _mySpDis3 = SplitContainer3.SplitterDistance;
                    if (StatusText.Multiline)
                    {
                        _mySpDis2 = StatusText.Height;
                    }

                    _myAdSpDis = SplitContainer4.SplitterDistance;
                    _modifySettingLocal = true;
                }
            }
        }

        private void DisposeAll()
        {
            // 後始末
            DisposeForms();
            DisposeIcons();
            DisposeInnerBrushes();
            DisposeUserBrushes();
            DisposeBworkers();
            _tabStringFormat.Dispose();
            if (_iconDict != null)
            {
                _iconDict.PauseGetImage = true;
                _iconDict.Dispose();
            }

            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

        private void DisposeForms()
        {
            _settingDialog.Dispose();
            _tabDialog.Dispose();
            _searchDialog.Dispose();
            _fltDialog.Dispose();
            _urlDialog.Dispose();
            _spaceKeyCanceler.Dispose();
            _apiGauge.Dispose();
            _shield.Dispose();
        }

        private void DisposeBworkers()
        {
            for (var i = 0; i < _bworkers.Length; ++i)
            {
                if (_bworkers[i] != null)
                {
                    _bworkers[i].Dispose();
                }
            }

            if (_followerFetchWorker != null)
            {
                _followerFetchWorker.Dispose();
            }
        }

        private void DisposeInnerBrushes()
        {
            _brsHighLight.Dispose();
            _brsHighLightText.Dispose();
            _brsDeactiveSelection.Dispose();
        }

        private void InitUserBrushes()
        {
            _brsForeColorUnread = new SolidBrush(_clrUnread);
            _brsForeColorReaded = new SolidBrush(_clrRead);
            _brsForeColorFav = new SolidBrush(_clrFav);
            _brsForeColorOwl = new SolidBrush(_clrOwl);
            _brsForeColorRetweet = new SolidBrush(_clrRetweet);
            _brsBackColorMine = new SolidBrush(_clrSelf);
            _brsBackColorAt = new SolidBrush(_clrAtSelf);
            _brsBackColorYou = new SolidBrush(_clrTarget);
            _brsBackColorAtYou = new SolidBrush(_clrAtTarget);
            _brsBackColorAtFromTarget = new SolidBrush(_clrAtFromTarget);
            _brsBackColorAtTo = new SolidBrush(_clrAtTo);
            _brsBackColorNone = new SolidBrush(_clrListBackcolor);
        }

        private void DisposeUserBrushes()
        {
            if (_brsForeColorUnread != null)
            {
                _brsForeColorUnread.Dispose();
            }

            if (_brsForeColorReaded != null)
            {
                _brsForeColorReaded.Dispose();
            }

            if (_brsForeColorFav != null)
            {
                _brsForeColorFav.Dispose();
            }

            if (_brsForeColorOwl != null)
            {
                _brsForeColorOwl.Dispose();
            }

            if (_brsForeColorRetweet != null)
            {
                _brsForeColorRetweet.Dispose();
            }

            if (_brsBackColorMine != null)
            {
                _brsBackColorMine.Dispose();
            }

            if (_brsBackColorAt != null)
            {
                _brsBackColorAt.Dispose();
            }

            if (_brsBackColorYou != null)
            {
                _brsBackColorYou.Dispose();
            }

            if (_brsBackColorAtYou != null)
            {
                _brsBackColorAtYou.Dispose();
            }

            if (_brsBackColorAtFromTarget != null)
            {
                _brsBackColorAtFromTarget.Dispose();
            }

            if (_brsBackColorAtTo != null)
            {
                _brsBackColorAtTo.Dispose();
            }

            if (_brsBackColorNone != null)
            {
                _brsBackColorNone.Dispose();
            }
        }

        private void DisposeIcons()
        {
            if (_iconAt != null)
            {
                _iconAt.Dispose();
            }

            if (_iconAtRed != null)
            {
                _iconAtRed.Dispose();
            }

            if (_iconAtSmoke != null)
            {
                _iconAtSmoke.Dispose();
            }

            if (_iconRefresh[0] != null)
            {
                _iconRefresh[0].Dispose();
            }

            if (_iconRefresh[1] != null)
            {
                _iconRefresh[1].Dispose();
            }

            if (_iconRefresh[2] != null)
            {
                _iconRefresh[2].Dispose();
            }

            if (_iconRefresh[3] != null)
            {
                _iconRefresh[3].Dispose();
            }

            if (_tabIcon != null)
            {
                _tabIcon.Dispose();
            }

            if (_mainIcon != null)
            {
                _mainIcon.Dispose();
            }

            if (_replyIcon != null)
            {
                _replyIcon.Dispose();
            }

            if (_replyIconBlink != null)
            {
                _replyIconBlink.Dispose();
            }
        }

        private void TweenMain_FormClosingExtracted(FormClosingEventArgs e)
        {
            if (!_configs.CloseToExit && e.CloseReason == CloseReason.UserClosing && !MyCommon.IsEnding)
            {
                e.Cancel = true;
                Visible = false;
            }
            else
            {
                _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                _ignoreConfigSave = true;
                MyCommon.IsEnding = true;
                _timerTimeline.Enabled = false;
                TimerRefreshIcon.Enabled = false;
            }
        }

        private void TweenMain_LocationChangedExtracted()
        {
            if (WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myLoc = DesktopLocation;
                _modifySettingLocal = true;
            }
        }

        private void ResizeMainForm()
        {
            if (!_initialLayout && _configs.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Visible = false;
            }

            if (_initialLayout && _cfgLocal != null && WindowState == FormWindowState.Normal && Visible)
            {
                ClientSize = _cfgLocal.FormSize;          // 'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                DesktopLocation = _cfgLocal.FormLocation; // '位置保持（最小化・最大化されたまま終了した場合の対応用）

                if (!SplitContainer4.Panel2Collapsed && _cfgLocal.AdSplitterDistance > SplitContainer4.Panel1MinSize)
                {
                    // Splitterの位置設定
                    SplitContainer4.SplitterDistance = _cfgLocal.AdSplitterDistance;
                }

                if (_cfgLocal.SplitterDistance > SplitContainer1.Panel1MinSize && _cfgLocal.SplitterDistance < SplitContainer1.Height - SplitContainer1.Panel2MinSize - SplitContainer1.SplitterWidth)
                {
                    // Splitterの位置設定
                    SplitContainer1.SplitterDistance = _cfgLocal.SplitterDistance;
                }

                // 発言欄複数行
                StatusText.Multiline = _cfgLocal.StatusMultiline;
                if (StatusText.Multiline)
                {
                    int dis = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    if (dis > SplitContainer2.Panel1MinSize && dis < SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    }

                    StatusText.Height = _cfgLocal.StatusTextHeight;
                }
                else
                {
                    if (SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth > 0)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
                    }
                }

                if (_cfgLocal.PreviewDistance > SplitContainer3.Panel1MinSize && _cfgLocal.PreviewDistance < SplitContainer3.Width - SplitContainer3.Panel2MinSize - SplitContainer3.SplitterWidth)
                {
                    SplitContainer3.SplitterDistance = _cfgLocal.PreviewDistance;
                }

                _initialLayout = false;
            }
        }

        private void TweenMain_ShownExtracted()
        {
            try
            {
                // 発言詳細部初期化
                PostBrowser.Url = new Uri("about:blank");
                PostBrowser.DocumentText = string.Empty;
            }
            catch (Exception)
            {
            }

            NotifyIcon1.Visible = true;
            _tw.UserIdChanged += Tw_UserIdChanged;

            if (!MyCommon.IsNetworkAvailable())
            {
                _isInitializing = false;
                _timerTimeline.Enabled = true;
                return;
            }

            string tabNameAny = string.Empty;
            GetTimeline(WorkerType.BlockIds);
            if (_configs.StartupFollowers)
            {
                GetTimeline(WorkerType.Follower);
            }

            GetTimeline(WorkerType.Configuration);
            StartUserStream();
            _waitTimeline = true;
            GetTimeline(WorkerType.Timeline, 1, 1);
            _waitReply = true;
            GetTimeline(WorkerType.Reply, 1, 1);
            _waitDm = true;
            GetTimeline(WorkerType.DirectMessegeRcv, 1, 1);
            if (_configs.GetFav)
            {
                _waitFav = true;
                GetTimeline(WorkerType.Favorites, 1, 1);
            }

            _waitPubSearch = true;
            GetTimeline(WorkerType.PublicSearch);
            _waitUserTimeline = true;
            GetTimeline(WorkerType.UserTimeline);
            _waitLists = true;
            GetTimeline(WorkerType.List);
            int i = 0, j = 0;
            int stth = 12 * 1000;
            int sl = 100;
            while (IsInitialRead() && !MyCommon.IsEnding)
            {
                Thread.Sleep(sl);
                Application.DoEvents();
                i += 1;
                j += 1;
                if (j > (stth / sl))
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
            if (_configs.StartupVersion)
            {
                CheckNewVersion(true);
            }

            // 取得失敗の場合は再試行する
            if (!_tw.GetFollowersSuccess && _configs.StartupFollowers)
            {
                GetTimeline(WorkerType.Follower);
            }

            // 取得失敗の場合は再試行する
            if (_configs.TwitterConfiguration.PhotoSizeLimit == 0)
            {
                GetTimeline(WorkerType.Configuration);
            }

            // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
            if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
            {
                MessageBox.Show(R.ReAuthorizeText);
                TryShowSettingsBox();
            }

            _isInitializing = false;
            _timerTimeline.Enabled = true;
        }

        private void UndoRemoveTab()
        {
            if (_statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TabClass tb = _statuses.RemovedTab.Pop();
            string renamed = tb.TabName;
            for (int i = 1; i <= int.MaxValue; i++)
            {
                if (!_statuses.ContainsTab(renamed))
                {
                    break;
                }

                renamed = string.Format("{0}({1})", tb.TabName, i);
            }

            tb.TabName = renamed;
            _statuses.Tabs.Add(renamed, tb);
            AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            SaveConfigsTabs();
        }

        private void ChangeCurrentTabUnreadManagement(bool isManage)
        {
            UreadManageMenuItem.Checked = UnreadMngTbMenuItem.Checked = isManage;
            if (string.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            ChangeTabUnreadManage(_rclickTabName, isManage);
            SaveConfigsTabs();
        }

        private void ConvertUrlByAutoSelectedService()
        {
            if (!ConvertUrl(_configs.AutoShortUrlFirst))
            {
                // 前回使用した短縮URLサービス以外を選択する
                UrlConverter svc = _configs.AutoShortUrlFirst;
                Random rnd = new Random();
                do
                {
                    svc = (UrlConverter)rnd.Next(Enum.GetNames(typeof(UrlConverter)).Length);
                }
                while (!(svc != _configs.AutoShortUrlFirst && svc != UrlConverter.Nicoms && svc != UrlConverter.Unu));
                ConvertUrl(svc);
            }
        }

        private void TryCopyUrlInCurrentTweet()
        {
            MatchCollection mc = Regex.Matches(PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + _postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?://[^\"]+)\"", RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                if (m.Groups["url"].Value == _postBrowserStatusText)
                {
                    CopyToClipboard(m.Groups["title"].Value);
                    break;
                }
            }

            if (mc.Count == 0)
            {
                CopyToClipboard(_postBrowserStatusText);
            }
        }

        private void TrySetHashtagFromCurrentTweet()
        {
            Match m = Regex.Match(_postBrowserStatusText, "^https?://twitter.com/search\\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                // 使用ハッシュタグとして設定
                HashMgr.SetPermanentHash("#" + m.Result("${hash}"));
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
                _modifySettingCommon = true;
            }
        }

        private void TryOpenCurrentNameLabelUserHome()
        {
            if (NameLabel.Tag != null)
            {
                var screenName = (string)NameLabel.Tag;
                OpenUriAsync(string.Format("https://twitter.com/{0}", screenName));
            }
        }

        private void ChangeUserPictureCursor(Cursor cursorsDefault)
        {
            UserPicture.Cursor = cursorsDefault;
        }

        private string GetDetailHtmlFormatHeader(bool useMonospace)
        {
            var ele = GetMonoEle(useMonospace);
            var ss = new Dictionary<string, Dictionary<string, string>>
            {
                { "a:link, a:visited, a:active, a:hover", new Dictionary<string, string>
                    {
                        { "color", _clrDetailLink.AsCssRgb() }
                    }
                },
                { "body", new Dictionary<string, string>
                    {
                        { "margin", "0px" },
                        { "background-color", _clrDetailBackcolor.AsCssRgb() }
                    }
                },
                { "body > p", new Dictionary<string, string>
                    {
                        { "vertical-align", "text-bottom" }
                    }
                },
                { ele, new Dictionary<string, string>
                    {
                        { "margin", "0" },
                        { "word-wrap", "break-word" },
                        { "font-family", string.Format("\"{0}\", sans-serif;", _fntDetail.Name) },
                        { "font-size", string.Format("{0}pt", _fntDetail.Size) },
                        { "color", _clrDetail.AsCssRgb() }
                    }
                }
            };

            return "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=10;IE=9;IE=8\"/>"
                + "<style type=\"text/css\">"
                + string.Join(string.Empty, ss.Select(sel => string.Format("{0}{{{1}}}", sel.Key, string.Join(string.Empty, sel.Value.Select(ps => string.Format("{0}: {1};", ps.Key, ps.Value))))))
                + "</style>"
                + "</head><body>" + "<" + ele + ">";
        }

        private string GetDetailHtmlFormatFooter(bool useMonospace)
        {
            return string.Format("</{0}></body></html>", GetMonoEle(useMonospace));
        }

        private string GetMonoEle(bool useMonospace)
        {
            return useMonospace ? "pre" : "p";
        }

        private void ClearReplyToInfo()
        {
            _replyToId = 0;
            _replyToName = string.Empty;
        }

        #endregion private methods
    }
}