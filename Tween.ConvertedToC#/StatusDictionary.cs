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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tween
{
    public sealed class TabInformations
    {
        //個別タブの情報をDictionaryで保持
        private IdComparerClass _sorter;

        private Dictionary<string, TabClass> _tabs = new Dictionary<string, TabClass>();
        private Dictionary<long, PostClass> _statuses = new Dictionary<long, PostClass>();
        private List<long> _addedIds;
        private List<long> _deletedIds = new List<long>();
        private Dictionary<long, PostClass> _retweets = new Dictionary<long, PostClass>();
        private Stack<TabClass> _removedTab = new Stack<TabClass>();

        private List<ScrubGeoInfo> _scrubGeo = new List<ScrubGeoInfo>();

        private class ScrubGeoInfo
        {
            public long UserId;
            public long UpToStatusId;
        }

        public List<long> BlockIds = new List<long>();
        //発言の追加
        //AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        //トランザクション用
        private int _addCount;

        private string _soundFile;
        private List<PostClass> _notifyPosts;
        private readonly object LockObj = new object();

        private readonly object LockUnread = new object();

        private static TabInformations _instance = new TabInformations();
        //List

        private List<ListElement> _lists = new List<ListElement>();

        private TabInformations()
        {
            _sorter = new IdComparerClass();
        }

        public static TabInformations GetInstance()
        {
            return _instance;
            //singleton
        }

        public List<ListElement> SubscribableLists
        {
            get { return _lists; }
            set
            {
                if (value != null && value.Count > 0)
                {
                    foreach (TabClass tb in this.GetTabsByType(Tween.MyCommon.TabUsageType.Lists))
                    {
                        foreach (ListElement list in value)
                        {
                            if (tb.ListInfo.Id == list.Id)
                            {
                                tb.ListInfo = list;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
                _lists = value;
            }
        }

        public bool AddTab(string TabName, Tween.MyCommon.TabUsageType TabType, ListElement List)
        {
            if (_tabs.ContainsKey(TabName))
                return false;
            _tabs.Add(TabName, new TabClass(TabName, TabType, List));
            _tabs[TabName].Sorter.Mode = _sorter.Mode;
            _tabs[TabName].Sorter.Order = _sorter.Order;
            return true;
        }

        //Public Sub AddTab(ByVal TabName As String, ByVal Tab As TabClass)
        //    _tabs.Add(TabName, Tab)
        //End Sub

        public void RemoveTab(string TabName)
        {
            lock (LockObj)
            {
                if (IsDefaultTab(TabName))
                    return;
                //念のため
                if (!_tabs[TabName].IsInnerStorageTabType)
                {
                    TabClass homeTab = GetTabByType(Tween.MyCommon.TabUsageType.Home);
                    string dmName = GetTabByType(Tween.MyCommon.TabUsageType.DirectMessage).TabName;

                    for (int idx = 0; idx <= _tabs[TabName].AllCount - 1; idx++)
                    {
                        bool exist = false;
                        long Id = _tabs[TabName].GetId(idx);
                        if (Id < 0)
                            continue;
                        foreach (string key in _tabs.Keys)
                        {
                            if (!(key == TabName) && key != dmName)
                            {
                                if (_tabs[key].Contains(Id))
                                {
                                    exist = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }
                        }
                        if (!exist)
                            homeTab.Add(Id, _statuses[Id].IsRead, false);
                    }
                }
                _removedTab.Push(_tabs[TabName]);
                _tabs.Remove(TabName);
            }
        }

        public Stack<TabClass> RemovedTab;//= _removedTab;

        public bool ContainsTab(string TabText)
        {
            return _tabs.ContainsKey(TabText);
        }

        public bool ContainsTab(TabClass ts)
        {
            return _tabs.ContainsValue(ts);
        }

        public Dictionary<string, TabClass> Tabs
        {
            get { return _tabs; }
            set { _tabs = value; }
        }

        public System.Collections.Generic.Dictionary<string, TabClass>.KeyCollection KeysTab
        {
            get { return _tabs.Keys; }
        }

        public void SortPosts()
        {
            foreach (string key in _tabs.Keys)
            {
                _tabs[key].Sort();
            }
        }

        public SortOrder SortOrder
        {
            get { return _sorter.Order; }
            set
            {
                _sorter.Order = value;
                foreach (string key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Order = value;
                }
            }
        }

        public IdComparerClass.ComparerMode SortMode
        {
            get { return _sorter.Mode; }
            set
            {
                _sorter.Mode = value;
                foreach (string key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Mode = value;
                }
            }
        }

        public System.Windows.Forms.SortOrder ToggleSortOrder(IdComparerClass.ComparerMode SortMode)
        {
            if (_sorter.Mode == SortMode)
            {
                if (_sorter.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    _sorter.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    _sorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
                foreach (string key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Order = _sorter.Order;
                }
            }
            else
            {
                _sorter.Mode = SortMode;
                _sorter.Order = System.Windows.Forms.SortOrder.Ascending;
                foreach (string key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Mode = SortMode;
                    _tabs[key].Sorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            this.SortPosts();
            return _sorter.Order;
        }

        public PostClass RetweetSource(long Id)
        {
            //get
            {
                if (_retweets.ContainsKey(Id))
                {
                    return _retweets[Id];
                }
                else
                {
                    return null;
                }
            }
        }

        public void RemoveFavPost(long Id)
        {
            lock (LockObj)
            {
                PostClass post = null;
                TabClass tab = this.GetTabByType(Tween.MyCommon.TabUsageType.Favorites);
                string tn = tab.TabName;
                if (_statuses.ContainsKey(Id))
                {
                    post = _statuses[Id];
                    //指定タブから該当ID削除
                    Tween.MyCommon.TabUsageType tType = tab.TabType;
                    if (tab.Contains(Id))
                    {
                        //未読管理
                        if (tab.UnreadManage && !post.IsRead)
                        {
                            lock (LockUnread)
                            {
                                tab.UnreadCount -= 1;
                                this.SetNextUnreadId(Id, tab);
                            }
                        }
                        tab.Remove(Id);
                    }
                    //FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                    if (tType == Tween.MyCommon.TabUsageType.Favorites && post.RetweetedId > 0)
                    {
                        for (int i = 0; i <= tab.AllCount - 1; i++)
                        {
                            PostClass rPost = null;
                            try
                            {
                                rPost = this.Item(tn, i);
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                break; // TODO: might not be correct. Was : Exit For
                            }
                            if (rPost.RetweetedId > 0 && rPost.RetweetedId == post.RetweetedId)
                            {
                                //未読管理
                                if (tab.UnreadManage && !rPost.IsRead)
                                {
                                    lock (LockUnread)
                                    {
                                        tab.UnreadCount -= 1;
                                        this.SetNextUnreadId(rPost.StatusId, tab);
                                    }
                                }
                                tab.Remove(rPost.StatusId);
                            }
                        }
                    }
                }
                //'TabType=PublicSearchの場合（Postの保存先がTabClass内）
                //If tab.Contains(StatusId) AndAlso _
                //   (tab.TabType = TabUsageType.PublicSearch OrElse tab.TabType = TabUsageType.DirectMessage) Then
                //    post = tab.Posts(StatusId)
                //    If tab.UnreadManage AndAlso Not post.IsRead Then    '未読管理
                //        SyncLock LockUnread
                //            tab.UnreadCount -= 1
                //            Me.SetNextUnreadId(StatusId, tab)
                //        End SyncLock
                //    End If
                //    tab.Remove(StatusId)
                //End If
            }
        }

        public void ScrubGeoReserve(long id, long upToStatusId)
        {
            lock (LockObj)
            {
                //Me._scrubGeo.Add(New ScrubGeoInfo With {.UserId = id, .UpToStatusId = upToStatusId})
                this.ScrubGeo(id, upToStatusId);
            }
        }

        private void ScrubGeo(long userId, long upToStatusId)
        {
            lock (LockObj)
            {
                var userPosts = from post in this._statuses.Values where post.UserId == userId && post.UserId <= upToStatusId select post;

                foreach (var p_loopVariable in userPosts)
                {
                    var p = p_loopVariable;
                    p.PostGeo = new PostClass.StatusGeo();
                }

                var userPosts2 = from tb in this.GetTabsInnerStorageType() from post in tb.Posts.Values where post.UserId == userId && post.UserId <= upToStatusId select post;

                foreach (var p_loopVariable in userPosts2)
                {
                    var p = p_loopVariable;
                    p.PostGeo = new PostClass.StatusGeo();
                }
            }
        }

        public void RemovePostReserve(long id)
        {
            lock (LockObj)
            {
                this._deletedIds.Add(id);
                this.DeletePost(id);
                //UI選択行がずれるため、RemovePostは使用しない
            }
        }

        public void RemovePost(long Id)
        {
            lock (LockObj)
            {
                PostClass post = null;
                //If _statuses.ContainsKey(Id) Then
                //各タブから該当ID削除
                foreach (string key in _tabs.Keys)
                {
                    TabClass tab = _tabs[key];
                    if (tab.Contains(Id))
                    {
                        if (!tab.IsInnerStorageTabType)
                        {
                            post = _statuses[Id];
                            //未読管理
                            if (tab.UnreadManage && !post.IsRead)
                            {
                                lock (LockUnread)
                                {
                                    tab.UnreadCount -= 1;
                                    this.SetNextUnreadId(Id, tab);
                                }
                            }
                            //未読数がずれる可能性があるためtab.Postsの未読も確認する
                        }
                        else
                        {
                            //未読管理
                            if (tab.UnreadManage && !tab.Posts[Id].IsRead)
                            {
                                lock (LockUnread)
                                {
                                    tab.UnreadCount -= 1;
                                    this.SetNextUnreadId(Id, tab);
                                }
                            }
                        }
                        tab.Remove(Id);
                    }
                }
                if (_statuses.ContainsKey(Id))
                    _statuses.Remove(Id);
                //End If
            }
        }

        private void DeletePost(long Id)
        {
            lock (LockObj)
            {
                PostClass post = null;
                if (_statuses.ContainsKey(Id))
                {
                    post = _statuses[Id];
                    post.IsDeleted = true;
                }
                foreach (TabClass tb in this.GetTabsInnerStorageType())
                {
                    if (tb.Contains(Id))
                    {
                        post = tb.Posts[Id];
                        post.IsDeleted = true;
                    }
                }
            }
        }

        public int GetOldestUnreadIndex(string TabName)
        {
            TabClass tb = _tabs[TabName];
            if (tb.OldestUnreadId > -1 && tb.Contains(tb.OldestUnreadId) && tb.UnreadCount > 0)
            {
                //未読アイテムへ
                bool isRead = false;
                if (!tb.IsInnerStorageTabType)
                {
                    isRead = _statuses[tb.OldestUnreadId].IsRead;
                }
                else
                {
                    isRead = tb.Posts[tb.OldestUnreadId].IsRead;
                }
                if (isRead)
                {
                    //状態不整合（最古未読ＩＤが実は既読）
                    lock (LockUnread)
                    {
                        this.SetNextUnreadId(-1, tb);
                        //頭から探索
                    }
                    if (tb.OldestUnreadId == -1)
                    {
                        return -1;
                    }
                    else
                    {
                        return tb.IndexOf(tb.OldestUnreadId);
                    }
                }
                else
                {
                    return tb.IndexOf(tb.OldestUnreadId);
                    //最短経路
                }
            }
            else
            {
                //一見未読なさそうだが、未読カウントはあるので探索
                //If tb.UnreadCount > 0 Then
                if (!(tb.UnreadManage && AppendSettingDialog.Instance.UnreadManage))
                    return -1;
                lock (LockUnread)
                {
                    this.SetNextUnreadId(-1, tb);
                }
                if (tb.OldestUnreadId == -1)
                {
                    return -1;
                }
                else
                {
                    return tb.IndexOf(tb.OldestUnreadId);
                }
                //    Else
                //    Return -1
                //End If
            }
        }

        private void SetNextUnreadId(long CurrentId, TabClass Tab)
        {
            //CurrentID:今既読にしたID(OldestIDの可能性あり)
            //最古未読が設定されていて、既読の場合（1発言以上存在）
            try
            {
                Dictionary<long, PostClass> posts = null;
                if (!Tab.IsInnerStorageTabType)
                {
                    posts = _statuses;
                }
                else
                {
                    posts = Tab.Posts;
                }
                //次の未読探索
                if (Tab.OldestUnreadId > -1 && posts.ContainsKey(Tab.OldestUnreadId) && posts[Tab.OldestUnreadId].IsRead && _sorter.Mode == IdComparerClass.ComparerMode.Id)
                {
                    if (Tab.UnreadCount == 0)
                    {
                        //未読数０→最古未読なし
                        Tab.OldestUnreadId = -1;
                    }
                    else if (Tab.OldestUnreadId == CurrentId && CurrentId > -1)
                    {
                        //最古IDを既読にしたタイミング→次のIDから続けて探索
                        int idx = Tab.IndexOf(CurrentId);
                        if (idx > -1)
                        {
                            //続きから探索
                            FindUnreadId(idx, Tab);
                        }
                        else
                        {
                            //頭から探索
                            FindUnreadId(-1, Tab);
                        }
                    }
                    else
                    {
                        //頭から探索
                        FindUnreadId(-1, Tab);
                    }
                }
                else
                {
                    //頭から探索
                    FindUnreadId(-1, Tab);
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                //頭から探索
                FindUnreadId(-1, Tab);
            }
        }

        private void FindUnreadId(int StartIdx, TabClass Tab)
        {
            if (Tab.AllCount == 0)
            {
                Tab.OldestUnreadId = -1;
                Tab.UnreadCount = 0;
                return;
            }
            int toIdx = 0;
            int stp = 1;
            Tab.OldestUnreadId = -1;
            if (_sorter.Order == System.Windows.Forms.SortOrder.Ascending)
            {
                if (StartIdx == -1)
                {
                    StartIdx = 0;
                }
                else
                {
                    //StartIdx += 1
                    if (StartIdx > Tab.AllCount - 1)
                        StartIdx = Tab.AllCount - 1;
                    //念のため
                }
                toIdx = Tab.AllCount - 1;
                if (toIdx < 0)
                    toIdx = 0;
                //念のため
                stp = 1;
            }
            else
            {
                if (StartIdx == -1)
                {
                    StartIdx = Tab.AllCount - 1;
                }
                else
                {
                    //StartIdx -= 1
                }
                if (StartIdx < 0)
                    StartIdx = 0;
                //念のため
                toIdx = 0;
                stp = -1;
            }

            Dictionary<long, PostClass> posts = null;
            if (!Tab.IsInnerStorageTabType)
            {
                posts = _statuses;
            }
            else
            {
                posts = Tab.Posts;
            }

            for (int i = StartIdx; i <= toIdx; i += stp)
            {
                long id = Tab.GetId(i);
                if (id > -1 && !posts[id].IsRead)
                {
                    Tab.OldestUnreadId = id;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        public int DistributePosts()
        {
            lock (LockObj)
            {
                //戻り値は追加件数
                //If _addedIds Is Nothing Then Return 0
                //If _addedIds.Count = 0 Then Return 0

                if (_addedIds == null)
                    _addedIds = new List<long>();
                if (_notifyPosts == null)
                    _notifyPosts = new List<PostClass>();
                try
                {
                    this.Distribute();
                    //タブに仮振分
                }
                catch (KeyNotFoundException ex)
                {
                    //タブ変更により振分が失敗した場合
                }
                int retCnt = _addedIds.Count;
                _addCount += retCnt;
                _addedIds.Clear();
                _addedIds = null;
                //後始末
                return retCnt;
                //件数
            }
        }

        public int SubmitUpdate(ref string soundFile, ref PostClass[] notifyPosts, ref bool isMentionIncluded, ref bool isDeletePost, bool isUserStream)
        {
            //注：メインスレッドから呼ぶこと
            lock (LockObj)
            {
                if (_notifyPosts == null)
                {
                    soundFile = "";
                    notifyPosts = null;
                    return 0;
                }

                foreach (TabClass tb in _tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                    {
                        _addCount += tb.GetTemporaryCount();
                    }
                    tb.AddSubmit(ref isMentionIncluded);
                    //振分確定（各タブに反映）
                }
                //'UserStreamで反映間隔10秒以下だったら、30秒ごとにソートする
                //'10秒以上だったら毎回ソート
                //Static lastSort As DateTime = Now
                //If AppendSettingDialog.Instance.UserstreamPeriodInt < 10 AndAlso isUserStream Then
                //    If Now.Subtract(lastSort) > TimeSpan.FromSeconds(30) Then
                //        lastSort = Now
                //        isUserStream = False
                //    End If
                //Else
                //    isUserStream = False
                //End If
                if (!isUserStream || this.SortMode != IdComparerClass.ComparerMode.Id)
                {
                    this.SortPosts();
                }
                if (isUserStream)
                {
                    isDeletePost = this._deletedIds.Count > 0;
                    foreach (long id in this._deletedIds)
                    {
                        //Me.DeletePost(StatusId)
                        this.RemovePost(id);
                    }
                    this._deletedIds.Clear();
                }

                soundFile = _soundFile;
                _soundFile = "";
                notifyPosts = _notifyPosts.ToArray();
                _notifyPosts.Clear();
                _notifyPosts = null;
                int retCnt = _addCount;
                _addCount = 0;
                return retCnt;
                //件数（EndUpdateの戻り値と同じ）
            }
        }

        private void Distribute()
        {
            //各タブのフィルターと照合。合致したらタブにID追加
            //通知メッセージ用に、表示必要な発言リストと再生サウンドを返す
            //notifyPosts = New List(Of PostClass)
            TabClass homeTab = GetTabByType(Tween.MyCommon.TabUsageType.Home);
            TabClass replyTab = GetTabByType(Tween.MyCommon.TabUsageType.Mentions);
            TabClass dmTab = GetTabByType(Tween.MyCommon.TabUsageType.DirectMessage);
            TabClass favTab = GetTabByType(Tween.MyCommon.TabUsageType.Favorites);
            foreach (long id in _addedIds)
            {
                PostClass post = _statuses[id];
                bool @add = false;
                //通知リスト追加フラグ
                bool mv = false;
                //移動フラグ（Recent追加有無）
                Tween.MyCommon.HITRESULT rslt = Tween.MyCommon.HITRESULT.None;
                post.IsExcludeReply = false;
                foreach (string tn in _tabs.Keys)
                {
                    rslt = _tabs[tn].AddFiltered(post);
                    if (rslt != Tween.MyCommon.HITRESULT.None && rslt != Tween.MyCommon.HITRESULT.Exclude)
                    {
                        if (rslt == Tween.MyCommon.HITRESULT.CopyAndMark)
                            post.IsMark = true;
                        //マークあり
                        if (rslt == Tween.MyCommon.HITRESULT.Move)
                        {
                            mv = true;
                            //移動
                            post.IsMark = false;
                        }
                        if (_tabs[tn].Notify)
                            @add = true;
                        //通知あり
                        if (!string.IsNullOrEmpty(_tabs[tn].SoundFile) && string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = _tabs[tn].SoundFile;
                            //wavファイル（未設定の場合のみ）
                        }
                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == Tween.MyCommon.HITRESULT.Exclude && _tabs[tn].TabType == Tween.MyCommon.TabUsageType.Mentions)
                        {
                            post.IsExcludeReply = true;
                        }
                        post.FilterHit = false;
                    }
                }
                //移動されなかったらRecentに追加
                if (!mv)
                {
                    homeTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(homeTab.SoundFile) && string.IsNullOrEmpty(_soundFile))
                        _soundFile = homeTab.SoundFile;
                    if (homeTab.Notify)
                        @add = true;
                }
                //除外ルール適用のないReplyならReplyタブに追加
                if (post.IsReply && !post.IsExcludeReply)
                {
                    replyTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(replyTab.SoundFile))
                        _soundFile = replyTab.SoundFile;
                    if (replyTab.Notify)
                        @add = true;
                }
                //Fav済み発言だったらFavoritesタブに追加
                if (post.IsFav)
                {
                    if (favTab.Contains(post.StatusId))
                    {
                        //取得済みなら非通知
                        //_soundFile = ""
                        @add = false;
                    }
                    else
                    {
                        favTab.Add(post.StatusId, post.IsRead, true);
                        if (!string.IsNullOrEmpty(favTab.SoundFile) && string.IsNullOrEmpty(_soundFile))
                            _soundFile = favTab.SoundFile;
                        if (favTab.Notify)
                            @add = true;
                    }
                }
                if (@add)
                    _notifyPosts.Add(post);
            }
            foreach (TabClass tb in _tabs.Values)
            {
                if (tb.IsInnerStorageTabType)
                {
                    if (tb.Notify)
                    {
                        if (tb.GetTemporaryCount() > 0)
                        {
                            foreach (PostClass post in tb.GetTemporaryPosts())
                            {
                                bool exist = false;
                                foreach (PostClass npost in _notifyPosts)
                                {
                                    if (npost.StatusId == post.StatusId)
                                    {
                                        exist = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                                if (!exist)
                                    _notifyPosts.Add(post);
                            }
                            if (!string.IsNullOrEmpty(tb.SoundFile))
                            {
                                if (tb.TabType == Tween.MyCommon.TabUsageType.DirectMessage || string.IsNullOrEmpty(_soundFile))
                                {
                                    _soundFile = tb.SoundFile;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddPost(PostClass Item)
        {
            lock (LockObj)
            {
                if (string.IsNullOrEmpty(Item.RelTabName))
                {
                    if (!Item.IsDm)
                    {
                        if (_statuses.ContainsKey(Item.StatusId))
                        {
                            if (Item.IsFav)
                            {
                                if (Item.RetweetedId == 0)
                                {
                                    _statuses[Item.StatusId].IsFav = true;
                                }
                                else
                                {
                                    Item.IsFav = false;
                                }
                            }
                            else
                            {
                                return;
                                //追加済みなら何もしない
                            }
                        }
                        else
                        {
                            if (Item.IsFav && Item.RetweetedId > 0)
                                Item.IsFav = false;
                            //既に持っている公式RTは捨てる
                            if (AppendSettingDialog.Instance.HideDuplicatedRetweets && !Item.IsMe && this._retweets.ContainsKey(Item.RetweetedId) && this._retweets[Item.RetweetedId].RetweetedCount > 0)
                                return;
                            if (BlockIds.Contains(Item.UserId))
                                return;
                            _statuses.Add(Item.StatusId, Item);
                        }
                        if (Item.RetweetedId > 0)
                        {
                            this.AddRetweet(Item);
                        }
                        if (Item.IsFav && _retweets.ContainsKey(Item.StatusId))
                        {
                            return;
                            //Fav済みのRetweet元発言は追加しない
                        }
                        if (_addedIds == null)
                            _addedIds = new List<long>();
                        //タブ追加用IDコレクション準備
                        _addedIds.Add(Item.StatusId);
                    }
                    else
                    {
                        //DM
                        TabClass tb = this.GetTabByType(Tween.MyCommon.TabUsageType.DirectMessage);
                        if (tb.Contains(Item.StatusId))
                            return;
                        tb.AddPostToInnerStorage(Item);
                    }
                }
                else
                {
                    //公式検索、リスト、関連発言の場合
                    TabClass tb = null;
                    if (this.Tabs.ContainsKey(Item.RelTabName))
                    {
                        tb = this.Tabs[Item.RelTabName];
                    }
                    else
                    {
                        return;
                    }
                    if (tb == null)
                        return;
                    if (tb.Contains(Item.StatusId))
                        return;
                    //tb.Add(Item.StatusId, Item.IsRead, True)
                    tb.AddPostToInnerStorage(Item);
                }
            }
        }

        private void AddRetweet(PostClass item)
        {
            //True:追加、False:保持済み
            if (_retweets.ContainsKey(item.RetweetedId))
            {
                _retweets[item.RetweetedId].RetweetedCount += 1;
                if (_retweets[item.RetweetedId].RetweetedCount > 10)
                {
                    _retweets[item.RetweetedId].RetweetedCount = 0;
                }
                return;
            }

            _retweets.Add(item.RetweetedId, new PostClass(item.Nickname, item.TextFromApi, item.Text, item.ImageUrl, item.ScreenName, item.CreatedAt, item.RetweetedId, item.IsFav, item.IsRead, item.IsReply,
            item.IsExcludeReply, item.IsProtect, item.IsOwl, item.IsMark, item.InReplyToUser, item.InReplyToStatusId, item.Source, item.SourceHtml, item.ReplyToList, item.IsMe,
            item.IsDm, item.UserId, item.FilterHit, "", 0, item.PostGeo));
            _retweets[item.RetweetedId].RetweetedCount += 1;
        }

        public void SetReadAllTab(bool Read, string TabName, int Index)
        {
            //Read:True=既読へ　False=未読へ
            TabClass tb = _tabs[TabName];

            if (tb.UnreadManage == false)
                return;
            //未読管理していなければ終了

            long Id = tb.GetId(Index);
            if (Id < 0)
                return;
            PostClass post = null;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[Id];
            }
            else
            {
                post = tb.Posts[Id];
            }

            if (post.IsRead == Read)
                return;
            //状態変更なければ終了

            post.IsRead = Read;

            lock (LockUnread)
            {
                if (Read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(Id, tb);
                    //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        if (_statuses.ContainsKey(Id) && !_statuses[Id].IsRead)
                        {
                            foreach (string key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                                {
                                    _tabs[key].UnreadCount -= 1;
                                    if (_tabs[key].OldestUnreadId == Id)
                                        _tabs[key].OldestUnreadId = -1;
                                }
                            }
                            _statuses[Id].IsRead = true;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (string key in _tabs.Keys)
                        {
                            if (key != TabName && _tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount -= 1;
                                if (_tabs[key].OldestUnreadId == Id)
                                    _tabs[key].OldestUnreadId = -1;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != TabName && _tabs[key].Contains(Id) && _tabs[key].IsInnerStorageTabType && !_tabs[key].Posts[Id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount -= 1;
                                if (_tabs[key].OldestUnreadId == Id)
                                    _tabs[key].OldestUnreadId = -1;
                            }
                            _tabs[key].Posts[Id].IsRead = true;
                        }
                    }
                }
                else
                {
                    tb.UnreadCount += 1;
                    //If tb.OldestUnreadId > Id OrElse tb.OldestUnreadId = -1 Then tb.OldestUnreadId = Id
                    if (tb.OldestUnreadId > Id)
                        tb.OldestUnreadId = Id;
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        if (_statuses.ContainsKey(Id) && _statuses[Id].IsRead)
                        {
                            foreach (string key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                                {
                                    _tabs[key].UnreadCount += 1;
                                    if (_tabs[key].OldestUnreadId > Id)
                                        _tabs[key].OldestUnreadId = Id;
                                }
                            }
                            _statuses[Id].IsRead = false;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (string key in _tabs.Keys)
                        {
                            if (key != TabName && _tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount += 1;
                                if (_tabs[key].OldestUnreadId > Id)
                                    _tabs[key].OldestUnreadId = Id;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != TabName && _tabs[key].Contains(Id) && _tabs[key].IsInnerStorageTabType && _tabs[key].Posts[Id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount += 1;
                                if (_tabs[key].OldestUnreadId > Id)
                                    _tabs[key].OldestUnreadId = Id;
                            }
                            _tabs[key].Posts[Id].IsRead = false;
                        }
                    }
                }
            }
        }

        /// TODO: パフォーマンスを勘案して、戻すか決める
        public void SetRead(bool Read, string TabName, int Index)
        {
            //Read:True=既読へ　False=未読へ
            TabClass tb = _tabs[TabName];

            if (tb.UnreadManage == false)
                return;
            //未読管理していなければ終了

            long Id = tb.GetId(Index);
            if (Id < 0)
                return;
            PostClass post = null;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[Id];
            }
            else
            {
                post = tb.Posts[Id];
            }

            if (post.IsRead == Read)
                return;
            //状態変更なければ終了

            post.IsRead = Read;
            //指定の状態に変更

            lock (LockUnread)
            {
                if (Read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(Id, tb);
                    //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                        return;
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != TabName && _tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount -= 1;
                            if (_tabs[key].OldestUnreadId == Id)
                                _tabs[key].OldestUnreadId = -1;
                        }
                    }
                }
                else
                {
                    tb.UnreadCount += 1;
                    //If tb.OldestUnreadId > Id OrElse tb.OldestUnreadId = -1 Then tb.OldestUnreadId = Id
                    if (tb.OldestUnreadId > Id)
                        tb.OldestUnreadId = Id;
                    if (tb.IsInnerStorageTabType)
                        return;
                    foreach (string key in _tabs.Keys)
                    {
                        if (!(key == TabName) && _tabs[key].UnreadManage && _tabs[key].Contains(Id) && !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount += 1;
                            if (_tabs[key].OldestUnreadId > Id)
                                _tabs[key].OldestUnreadId = Id;
                        }
                    }
                }
            }
        }

        public void SetRead()
        {
            TabClass tb = GetTabByType(Tween.MyCommon.TabUsageType.Home);
            if (tb.UnreadManage == false)
                return;

            lock (LockObj)
            {
                for (int i = 0; i <= tb.AllCount - 1; i++)
                {
                    long id = tb.GetId(i);
                    if (id < 0)
                        return;
                    if (!_statuses[id].IsReply && !_statuses[id].IsRead && !_statuses[id].FilterHit)
                    {
                        _statuses[id].IsRead = true;
                        this.SetNextUnreadId(id, tb);
                        //次の未読セット
                        foreach (string key in _tabs.Keys)
                        {
                            if (_tabs[key].UnreadManage && _tabs[key].Contains(id))
                            {
                                _tabs[key].UnreadCount -= 1;
                                if (_tabs[key].OldestUnreadId == id)
                                    _tabs[key].OldestUnreadId = -1;
                            }
                        }
                    }
                }
            }
        }

        public PostClass Item(long ID)
        {
            //get
            {
                if (_statuses.ContainsKey(ID))
                    return _statuses[ID];
                foreach (TabClass tb in this.GetTabsInnerStorageType())
                {
                    if (tb.Contains(ID))
                    {
                        return tb.Posts[ID];
                    }
                }
                return null;
            }
        }

        public PostClass Item(string TabName, int Index)
        {
            //get
            {
                if (!_tabs.ContainsKey(TabName))
                    throw new ArgumentException("TabName=" + TabName + " is not contained.");
                long id = _tabs[TabName].GetId(Index);
                if (id < 0)
                    throw new ArgumentException("Index can't find. Index=" + Index.ToString() + "/TabName=" + TabName);
                try
                {
                    if (_tabs[TabName].IsInnerStorageTabType)
                    {
                        return _tabs[TabName].Posts[_tabs[TabName].GetId(Index)];
                    }
                    else
                    {
                        return _statuses[_tabs[TabName].GetId(Index)];
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Index=" + Index.ToString() + "/TabName=" + TabName, ex);
                }
            }
        }

        public PostClass[] Item(string TabName, int StartIndex, int EndIndex)
        {
            //get
            {
                int length = EndIndex - StartIndex + 1;
                PostClass[] posts = new PostClass[length];
                if (_tabs[TabName].IsInnerStorageTabType)
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        posts[i] = _tabs[TabName].Posts[_tabs[TabName].GetId(StartIndex + i)];
                    }
                }
                else
                {
                    for (int i = 0; i <= length - 1; i++)
                    {
                        posts[i] = _statuses[_tabs[TabName].GetId(StartIndex + i)];
                    }
                }
                return posts;
            }
        }

        //Public ReadOnly Property ItemCount() As Integer
        //    Get
        //        SyncLock LockObj
        //            Return _statuses.Count   'DM,公式検索は除く
        //        End SyncLock
        //    End Get
        //End Property

        public bool ContainsKey(long Id)
        {
            //DM,公式検索は非対応
            lock (LockObj)
            {
                return _statuses.ContainsKey(Id);
            }
        }

        public bool ContainsKey(long Id, string TabName)
        {
            //DM,公式検索は対応版
            lock (LockObj)
            {
                if (_tabs.ContainsKey(TabName))
                {
                    return _tabs[TabName].Contains(Id);
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetUnreadManage(bool Manage)
        {
            if (Manage)
            {
                foreach (string key in _tabs.Keys)
                {
                    TabClass tb = _tabs[key];
                    if (tb.UnreadManage)
                    {
                        lock (LockUnread)
                        {
                            int cnt = 0;
                            long oldest = long.MaxValue;
                            Dictionary<long, PostClass> posts = null;
                            if (!tb.IsInnerStorageTabType)
                            {
                                posts = _statuses;
                            }
                            else
                            {
                                posts = tb.Posts;
                            }
                            foreach (long id in tb.BackupIds())
                            {
                                if (!posts[id].IsRead)
                                {
                                    cnt += 1;
                                    if (oldest > id)
                                        oldest = id;
                                }
                            }
                            if (oldest == long.MaxValue)
                                oldest = -1;
                            tb.OldestUnreadId = oldest;
                            tb.UnreadCount = cnt;
                        }
                    }
                }
            }
            else
            {
                foreach (string key in _tabs.Keys)
                {
                    TabClass tb = _tabs[key];
                    if (tb.UnreadManage && tb.UnreadCount > 0)
                    {
                        lock (LockUnread)
                        {
                            tb.UnreadCount = 0;
                            tb.OldestUnreadId = -1;
                        }
                    }
                }
            }
        }

        public void RenameTab(string Original, string NewName)
        {
            TabClass tb = _tabs[Original];
            _tabs.Remove(Original);
            tb.TabName = NewName;
            _tabs.Add(NewName, tb);
        }

        public void FilterAll()
        {
            lock (LockObj)
            {
                TabClass tbr = GetTabByType(Tween.MyCommon.TabUsageType.Home);
                TabClass replyTab = GetTabByType(Tween.MyCommon.TabUsageType.Mentions);
                foreach (TabClass tb in _tabs.Values.ToArray())
                {
                    if (tb.FilterModified)
                    {
                        tb.FilterModified = false;
                        long[] orgIds = tb.BackupIds();
                        tb.ClearIDs();
                        ///'''''''''''フィルター前のIDsを退避。どのタブにも含まれないidはrecentへ追加
                        ///'''''''''''moveフィルターにヒットした際、recentに該当あればrecentから削除
                        foreach (long id in _statuses.Keys)
                        {
                            PostClass post = _statuses[id];
                            if (post.IsDm)
                                continue;
                            Tween.MyCommon.HITRESULT rslt = Tween.MyCommon.HITRESULT.None;
                            rslt = tb.AddFiltered(post);
                            switch (rslt)
                            {
                                case Tween.MyCommon.HITRESULT.CopyAndMark:
                                    post.IsMark = true;
                                    //マークあり
                                    post.FilterHit = true;
                                    break;
                                case Tween.MyCommon.HITRESULT.Move:
                                    tbr.Remove(post.StatusId, post.IsRead);
                                    post.IsMark = false;
                                    post.FilterHit = true;
                                    break;
                                case Tween.MyCommon.HITRESULT.Copy:
                                    post.IsMark = false;
                                    post.FilterHit = true;
                                    break;
                                case Tween.MyCommon.HITRESULT.Exclude:
                                    if (tb.TabName == replyTab.TabName && post.IsReply)
                                        post.IsExcludeReply = true;
                                    if (post.IsFav)
                                        GetTabByType(Tween.MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                    post.FilterHit = false;
                                    break;
                                case Tween.MyCommon.HITRESULT.None:
                                    if (tb.TabName == replyTab.TabName && post.IsReply)
                                        replyTab.Add(post.StatusId, post.IsRead, true);
                                    if (post.IsFav)
                                        GetTabByType(Tween.MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                    post.FilterHit = false;
                                    break;
                            }
                        }
                        tb.AddSubmit();
                        //振分確定
                        foreach (long id in orgIds)
                        {
                            bool hit = false;
                            foreach (TabClass tbTemp in _tabs.Values.ToArray())
                            {
                                if (tbTemp.Contains(id))
                                {
                                    hit = true;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }
                            if (!hit)
                                tbr.Add(id, _statuses[id].IsRead, false);
                        }
                    }
                }
                this.SortPosts();
            }
        }

        public long[] GetId(string TabName, System.Windows.Forms.ListView.SelectedIndexCollection IndexCollection)
        {
            if (IndexCollection.Count == 0)
                return null;

            TabClass tb = _tabs[TabName];
            long[] Ids = new long[IndexCollection.Count];
            for (int i = 0; i <= Ids.Length - 1; i++)
            {
                Ids[i] = tb.GetId(IndexCollection[i]);
            }
            return Ids;
        }

        public long GetId(string TabName, int Index)
        {
            return _tabs[TabName].GetId(Index);
        }

        public int[] IndexOf(string TabName, long[] Ids)
        {
            if (Ids == null)
                return null;
            int[] idx = new int[Ids.Length];
            TabClass tb = _tabs[TabName];
            for (int i = 0; i <= Ids.Length - 1; i++)
            {
                idx[i] = tb.IndexOf(Ids[i]);
            }
            return idx;
        }

        public int IndexOf(string TabName, long Id)
        {
            return _tabs[TabName].IndexOf(Id);
        }

        public void ClearTabIds(string TabName)
        {
            //不要なPostを削除
            lock (LockObj)
            {
                if (!_tabs[TabName].IsInnerStorageTabType)
                {
                    foreach (long Id in _tabs[TabName].BackupIds())
                    {
                        bool Hit = false;
                        foreach (TabClass tb in _tabs.Values)
                        {
                            if (tb.Contains(Id))
                            {
                                Hit = true;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                        if (!Hit)
                            _statuses.Remove(Id);
                    }
                }

                //指定タブをクリア
                _tabs[TabName].ClearIDs();
            }
        }

        public void SetTabUnreadManage(string TabName, bool Manage)
        {
            TabClass tb = _tabs[TabName];
            lock (LockUnread)
            {
                if (Manage)
                {
                    int cnt = 0;
                    long oldest = long.MaxValue;
                    Dictionary<long, PostClass> posts = null;
                    if (!tb.IsInnerStorageTabType)
                    {
                        posts = _statuses;
                    }
                    else
                    {
                        posts = tb.Posts;
                    }
                    foreach (long id in tb.BackupIds())
                    {
                        if (!posts[id].IsRead)
                        {
                            cnt += 1;
                            if (oldest > id)
                                oldest = id;
                        }
                    }
                    if (oldest == long.MaxValue)
                        oldest = -1;
                    tb.OldestUnreadId = oldest;
                    tb.UnreadCount = cnt;
                }
                else
                {
                    tb.OldestUnreadId = -1;
                    tb.UnreadCount = 0;
                }
            }
            tb.UnreadManage = Manage;
        }

        public void RefreshOwl(List<long> follower)
        {
            lock (LockObj)
            {
                if (follower.Count > 0)
                {
                    foreach (PostClass post in _statuses.Values)
                    {
                        //If post.UserId = 0 OrElse post.IsDm Then Continue For
                        if (post.IsMe)
                        {
                            post.IsOwl = false;
                        }
                        else
                        {
                            post.IsOwl = !follower.Contains(post.UserId);
                        }
                    }
                }
                else
                {
                    foreach (long id in _statuses.Keys)
                    {
                        _statuses[id].IsOwl = false;
                    }
                }
            }
        }

        public TabClass GetTabByType(Tween.MyCommon.TabUsageType tabType)
        {
            //Home,Mentions,DM,Favは1つに制限する
            //その他のタイプを指定されたら、最初に合致したものを返す
            //合致しなければNothingを返す
            lock (LockObj)
            {
                foreach (TabClass tb in _tabs.Values)
                {
                    if (tb != null && tb.TabType == tabType)
                        return tb;
                }
                return null;
            }
        }

        public List<TabClass> GetTabsByType(Tween.MyCommon.TabUsageType tabType)
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (LockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in _tabs.Values)
                {
                    if ((tabType & tb.TabType) == tb.TabType)
                        tbs.Add(tb);
                }
                return tbs;
            }
        }

        public List<TabClass> GetTabsInnerStorageType()
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (LockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in _tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                        tbs.Add(tb);
                }
                return tbs;
            }
        }

        public TabClass GetTabByName(string tabName)
        {
            lock (LockObj)
            {
                if (_tabs.ContainsKey(tabName))
                    return _tabs[tabName];
                return null;
            }
        }

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            if (tabName != null && _tabs.ContainsKey(tabName) && (_tabs[tabName].TabType == Tween.MyCommon.TabUsageType.Home || _tabs[tabName].TabType == Tween.MyCommon.TabUsageType.Mentions || _tabs[tabName].TabType == Tween.MyCommon.TabUsageType.DirectMessage || _tabs[tabName].TabType == Tween.MyCommon.TabUsageType.Favorites))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //振り分け可能タブの判定処理
        public bool IsDistributableTab(string tabName)
        {
            return tabName != null && this._tabs.ContainsKey(tabName) && (_tabs[tabName].TabType == Tween.MyCommon.TabUsageType.Mentions || _tabs[tabName].TabType == Tween.MyCommon.TabUsageType.UserDefined);
        }

        public string GetUniqueTabName()
        {
            string tabNameTemp = "MyTab" + (_tabs.Count + 1).ToString();
            for (int i = 2; i <= 100; i++)
            {
                if (_tabs.ContainsKey(tabNameTemp))
                {
                    tabNameTemp = "MyTab" + (_tabs.Count + i).ToString();
                }
                else
                {
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            return tabNameTemp;
        }

        public Dictionary<long, PostClass> Posts
        {
            get { return _statuses; }
        }
    }

    [Serializable]
    public sealed class TabClass
    {
        private bool _unreadManage = false;
        private List<FiltersClass> _filters;
        private int _unreadCount = 0;
        private List<long> _ids;
        private List<TemporaryId> _tmpIds = new List<TemporaryId>();
        private Tween.MyCommon.TabUsageType _tabType = Tween.MyCommon.TabUsageType.Undefined;

        private IdComparerClass _sorter = new IdComparerClass();

        private readonly object _lockObj = new object();

        public string User { get; set; }

        #region "検索"

        //Search query
        private string _searchLang = "";

        private string _searchWords = "";

        private string _nextPageQuery = "";

        public string SearchLang
        {
            get { return _searchLang; }
            set
            {
                SinceId = 0;
                _searchLang = value;
            }
        }

        public string SearchWords
        {
            get { return _searchWords; }
            set
            {
                SinceId = 0;
                _searchWords = value.Trim();
            }
        }

        public string NextPageQuery
        {
            get { return _nextPageQuery; }
            set { _nextPageQuery = value; }
        }

        public int GetSearchPage(int count)
        {
            return ((_ids.Count / count) + 1);
        }

        private Dictionary<string, string> _beforeQuery = new Dictionary<string, string>();

        public void SaveQuery(bool more)
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_searchWords))
            {
                _beforeQuery = qry;
                return;
            }
            qry.Add("q", _searchWords);
            if (!string.IsNullOrEmpty(_searchLang))
                qry.Add("lang", _searchLang);
            _beforeQuery = qry;
        }

        public bool IsQueryChanged()
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_searchWords))
            {
                qry.Add("q", _searchWords);
                if (!string.IsNullOrEmpty(_searchLang))
                    qry.Add("lang", _searchLang);
            }
            if (qry.Count != _beforeQuery.Count)
                return true;

            foreach (KeyValuePair<string, string> kvp in qry)
            {
                if (!_beforeQuery.ContainsKey(kvp.Key) || _beforeQuery[kvp.Key] != kvp.Value)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion "検索"

        #region "リスト"

        private ListElement _listInfo;

        public ListElement ListInfo
        {
            get { return _listInfo; }
            set { _listInfo = value; }
        }

        #endregion "リスト"

        [System.Xml.Serialization.XmlIgnore()]
        public PostClass RelationTargetPost { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public long OldestId { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public long SinceId { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public Dictionary<long, PostClass> Posts { get; set; }

        public PostClass[] GetTemporaryPosts()
        {
            List<PostClass> tempPosts = new List<PostClass>();
            if (_tmpIds.Count == 0)
                return tempPosts.ToArray();
            foreach (TemporaryId tempId in _tmpIds)
            {
                tempPosts.Add(Posts[tempId.Id]);
            }
            return tempPosts.ToArray();
        }

        public int GetTemporaryCount()
        {
            return _tmpIds.Count;
        }

        private struct TemporaryId
        {
            public long Id;

            public bool Read;

            public TemporaryId(long argId, bool argRead)
            {
                Id = argId;
                Read = argRead;
            }
        }

        public TabClass()
        {
            Posts = new Dictionary<long, PostClass>();
            _filters = new List<FiltersClass>();
            Notify = true;
            SoundFile = "";
            _unreadManage = true;
            _ids = new List<long>();
            this.OldestUnreadId = -1;
            _tabType = Tween.MyCommon.TabUsageType.Undefined;
            _listInfo = null;
        }

        public TabClass(string TabName, Tween.MyCommon.TabUsageType TabType, ListElement list)
        {
            Posts = new Dictionary<long, PostClass>();
            this.TabName = TabName;
            _filters = new List<FiltersClass>();
            Notify = true;
            SoundFile = "";
            _unreadManage = true;
            _ids = new List<long>();
            this.OldestUnreadId = -1;
            _tabType = TabType;
            this.ListInfo = list;
            if (this.IsInnerStorageTabType)
            {
                _sorter.posts = Posts;
            }
            else
            {
                _sorter.posts = TabInformations.GetInstance().Posts;
            }
        }

        public void Sort()
        {
            if (_sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                _ids.Sort(_sorter.CmpMethod());
                return;
            }
            long[] ar = null;
            if (_sorter.Order == SortOrder.Ascending)
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderBy(n => _sorter.posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderBy(n => _sorter.posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderBy(n => _sorter.posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderBy(n => _sorter.posts[n].Source).ToArray();
                        break;
                }
            }
            else
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].Source).ToArray();
                        break;
                }
            }
            _ids = new List<long>(ar);
        }

        public IdComparerClass Sorter
        {
            get { return _sorter; }
        }

        //無条件に追加
        private void Add(long ID, bool Read)
        {
            if (this._ids.Contains(ID))
                return;

            if (this.Sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                if (this.Sorter.Order == SortOrder.Ascending)
                {
                    this._ids.Add(ID);
                }
                else
                {
                    this._ids.Insert(0, ID);
                }
            }
            else
            {
                this._ids.Add(ID);
            }

            if (!Read && this._unreadManage)
            {
                this._unreadCount += 1;
                if (ID < this.OldestUnreadId)
                    this.OldestUnreadId = ID;
                //If Me.OldestUnreadId = -1 Then
                //    Me.OldestUnreadId = ID
                //Else
                //    If ID < Me.OldestUnreadId Then Me.OldestUnreadId = ID
                //End If
            }
        }

        public void Add(long ID, bool Read, bool Temporary)
        {
            if (!Temporary)
            {
                this.Add(ID, Read);
            }
            else
            {
                _tmpIds.Add(new TemporaryId(ID, Read));
            }
        }

        //フィルタに合致したら追加
        public Tween.MyCommon.HITRESULT AddFiltered(PostClass post)
        {
            if (this.IsInnerStorageTabType)
                return Tween.MyCommon.HITRESULT.None;

            Tween.MyCommon.HITRESULT rslt = Tween.MyCommon.HITRESULT.None;
            //全フィルタ評価（優先順位あり）
            lock (this._lockObj)
            {
                foreach (FiltersClass ft in _filters)
                {
                    try
                    {
                        switch (ft.IsHit(post))
                        {
                            //フィルタクラスでヒット判定
                            case Tween.MyCommon.HITRESULT.None:
                                break;
                            case Tween.MyCommon.HITRESULT.Copy:
                                if (rslt != Tween.MyCommon.HITRESULT.CopyAndMark)
                                    rslt = Tween.MyCommon.HITRESULT.Copy;
                                break;
                            case Tween.MyCommon.HITRESULT.CopyAndMark:
                                rslt = Tween.MyCommon.HITRESULT.CopyAndMark;
                                break;
                            case Tween.MyCommon.HITRESULT.Move:
                                rslt = Tween.MyCommon.HITRESULT.Move;
                                break;
                            case Tween.MyCommon.HITRESULT.Exclude:
                                rslt = Tween.MyCommon.HITRESULT.Exclude;
                                break; // TODO: might not be correct. Was : Exit For

                                break;
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        //IsHitでNullRef出る場合あり。暫定対応
                        MyCommon.TraceOut("IsHitでNullRef: " + ft.ToString());
                        rslt = Tween.MyCommon.HITRESULT.None;
                    }
                }
            }

            if (rslt != Tween.MyCommon.HITRESULT.None && rslt != Tween.MyCommon.HITRESULT.Exclude)
            {
                _tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
            }

            return rslt;
            //マーク付けは呼び出し元で行うこと
        }

        //検索結果の追加
        public void AddPostToInnerStorage(PostClass Post)
        {
            if (Posts.ContainsKey(Post.StatusId))
                return;
            Posts.Add(Post.StatusId, Post);
            _tmpIds.Add(new TemporaryId(Post.StatusId, Post.IsRead));
        }

        public void AddSubmit(ref bool isMentionIncluded)
        {
            if (_tmpIds.Count == 0)
                return;
            _tmpIds.Sort((TemporaryId x, TemporaryId y) => x.Id.CompareTo(y.Id));
            foreach (TemporaryId tId in _tmpIds)
            {
                if (this.TabType == Tween.MyCommon.TabUsageType.Mentions && TabInformations.GetInstance().Item(tId.Id).IsReply)
                    isMentionIncluded = true;
                this.Add(tId.Id, tId.Read);
            }
            _tmpIds.Clear();
        }

        public void AddSubmit()
        {
            bool mention = false;
            AddSubmit(ref mention);
        }

        public void Remove(long Id)
        {
            if (!this._ids.Contains(Id))
                return;
            this._ids.Remove(Id);
            if (this.IsInnerStorageTabType)
                Posts.Remove(Id);
        }

        public void Remove(long Id, bool Read)
        {
            if (!this._ids.Contains(Id))
                return;

            if (!Read && this._unreadManage)
            {
                this._unreadCount -= 1;
                this.OldestUnreadId = -1;
            }

            this._ids.Remove(Id);
            if (this.IsInnerStorageTabType)
                Posts.Remove(Id);
        }

        public bool UnreadManage
        {
            get { return _unreadManage; }
            set
            {
                this._unreadManage = value;
                if (!value)
                {
                    this.OldestUnreadId = -1;
                    this._unreadCount = 0;
                }
            }
        }

        public bool Notify { get; set; }

        public string SoundFile { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public long OldestUnreadId { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public int UnreadCount
        {
            get { return this.UnreadManage && AppendSettingDialog.Instance.UnreadManage ? _unreadCount : 0; }
            set
            {
                if (value < 0)
                    value = 0;
                _unreadCount = value;
            }
        }

        public int AllCount
        {
            get { return this._ids.Count; }
        }

        public FiltersClass[] GetFilters()
        {
            lock (this._lockObj)
            {
                return _filters.ToArray();
            }
        }

        public void RemoveFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                _filters.Remove(filter);
                this.FilterModified = true;
            }
        }

        public bool AddFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                if (_filters.Contains(filter))
                    return false;
                _filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        public void EditFilter(FiltersClass original, FiltersClass modified)
        {
            original.BodyFilter = modified.BodyFilter;
            original.NameFilter = modified.NameFilter;
            original.SearchBoth = modified.SearchBoth;
            original.SearchUrl = modified.SearchUrl;
            original.UseRegex = modified.UseRegex;
            original.CaseSensitive = modified.CaseSensitive;
            original.IsRt = modified.IsRt;
            original.UseLambda = modified.UseLambda;
            original.Source = modified.Source;
            original.ExBodyFilter = modified.ExBodyFilter;
            original.ExNameFilter = modified.ExNameFilter;
            original.ExSearchBoth = modified.ExSearchBoth;
            original.ExSearchUrl = modified.ExSearchUrl;
            original.ExUseRegex = modified.ExUseRegex;
            original.ExCaseSensitive = modified.ExCaseSensitive;
            original.IsExRt = modified.IsExRt;
            original.ExUseLambda = modified.ExUseLambda;
            original.ExSource = modified.ExSource;
            original.MoveFrom = modified.MoveFrom;
            original.SetMark = modified.SetMark;
            this.FilterModified = true;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public List<FiltersClass> Filters
        {
            get
            {
                lock (this._lockObj)
                {
                    return _filters;
                }
            }
            set
            {
                lock (this._lockObj)
                {
                    _filters = value;
                }
            }
        }

        public FiltersClass[] FilterArray
        {
            get
            {
                lock (this._lockObj)
                {
                    return _filters.ToArray();
                }
            }
            set
            {
                lock (this._lockObj)
                {
                    foreach (FiltersClass filters in value)
                    {
                        _filters.Add(filters);
                    }
                }
            }
        }

        public bool Contains(long ID)
        {
            return _ids.Contains(ID);
        }

        public void ClearIDs()
        {
            _ids.Clear();
            _tmpIds.Clear();
            _unreadCount = 0;
            this.OldestUnreadId = -1;
            if (Posts != null)
            {
                Posts.Clear();
            }
        }

        public long GetId(int Index)
        {
            return Index < _ids.Count ? _ids[Index] : -1;
        }

        public int IndexOf(long ID)
        {
            return _ids.IndexOf(ID);
        }

        [System.Xml.Serialization.XmlIgnore()]
        public bool FilterModified { get; set; }

        public long[] BackupIds()
        {
            return _ids.ToArray();
        }

        public string TabName { get; set; }

        public Tween.MyCommon.TabUsageType TabType
        {
            get { return _tabType; }
            set
            {
                _tabType = value;
                if (this.IsInnerStorageTabType)
                {
                    _sorter.posts = Posts;
                }
                else
                {
                    _sorter.posts = TabInformations.GetInstance().Posts;
                }
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                if (_tabType == Tween.MyCommon.TabUsageType.PublicSearch || _tabType == Tween.MyCommon.TabUsageType.DirectMessage || _tabType == Tween.MyCommon.TabUsageType.Lists || _tabType == Tween.MyCommon.TabUsageType.UserTimeline || _tabType == Tween.MyCommon.TabUsageType.Related)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    [Serializable]
    public sealed class FiltersClass : System.IEquatable<FiltersClass>
    {
        private string _name = "";
        private List<string> _body = new List<string>();
        private bool _searchBoth = true;
        private bool _searchUrl = false;
        private bool _caseSensitive = false;
        private bool _useRegex = false;
        private bool _isRt = false;
        private string _source = "";
        private string _exname = "";
        private List<string> _exbody = new List<string>();
        private bool _exsearchBoth = true;
        private bool _exsearchUrl = false;
        private bool _exuseRegex = false;
        private bool _excaseSensitive = false;
        private bool _isExRt = false;
        private string _exSource = "";
        private bool _moveFrom = false;
        private bool _setMark = true;
        private bool _useLambda = false;

        private bool _exuseLambda = false;

        // ラムダ式コンパイルキャッシュ
        private LambdaExpression _lambdaExp = null;

        private Delegate _lambdaExpDelegate = null;
        private LambdaExpression _exlambdaExp = null;

        private Delegate _exlambdaExpDelegate = null;

        public FiltersClass()
        {
        }

        //フィルタ一覧に表示する文言生成
        private string MakeSummary()
        {
            StringBuilder fs = new StringBuilder();
            if (!string.IsNullOrEmpty(_name) || _body.Count > 0 || _isRt || !string.IsNullOrEmpty(_source))
            {
                if (_searchBoth)
                {
                    if (!string.IsNullOrEmpty(_name))
                    {
                        fs.AppendFormat(Tween.My_Project.Resources.SetFiltersText1, _name);
                    }
                    else
                    {
                        fs.Append(Tween.My_Project.Resources.SetFiltersText2);
                    }
                }
                if (_body.Count > 0)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText3);
                    foreach (string bf in _body)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length -= 1;
                    fs.Append(Tween.My_Project.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_searchBoth)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText6);
                }
                if (_useRegex)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText7);
                }
                if (_searchUrl)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText8);
                }
                if (_caseSensitive)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText13);
                }
                if (_isRt)
                {
                    fs.Append("RT/");
                }
                if (_useLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!string.IsNullOrEmpty(_source))
                {
                    fs.AppendFormat("Src…{0}/", _source);
                }
                fs.Length -= 1;
                fs.Append(")");
            }
            if (!string.IsNullOrEmpty(_exname) || _exbody.Count > 0 || _isExRt || !string.IsNullOrEmpty(_exSource))
            {
                //除外
                fs.Append(Tween.My_Project.Resources.SetFiltersText12);
                if (_exsearchBoth)
                {
                    if (!string.IsNullOrEmpty(_exname))
                    {
                        fs.AppendFormat(Tween.My_Project.Resources.SetFiltersText1, _exname);
                    }
                    else
                    {
                        fs.Append(Tween.My_Project.Resources.SetFiltersText2);
                    }
                }
                if (_exbody.Count > 0)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText3);
                    foreach (string bf in _exbody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length -= 1;
                    fs.Append(Tween.My_Project.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_exsearchBoth)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText6);
                }
                if (_exuseRegex)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText7);
                }
                if (_exsearchUrl)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText8);
                }
                if (_excaseSensitive)
                {
                    fs.Append(Tween.My_Project.Resources.SetFiltersText13);
                }
                if (_isExRt)
                {
                    fs.Append("RT/");
                }
                if (_exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!string.IsNullOrEmpty(_exSource))
                {
                    fs.AppendFormat("Src…{0}/", _exSource);
                }
                fs.Length -= 1;
                fs.Append(")");
            }

            fs.Append("(");
            if (_moveFrom)
            {
                fs.Append(Tween.My_Project.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Tween.My_Project.Resources.SetFiltersText11);
            }
            if (!_moveFrom && _setMark)
            {
                fs.Append(Tween.My_Project.Resources.SetFiltersText10);
            }
            else if (!_moveFrom)
            {
                fs.Length -= 1;
            }

            fs.Append(")");

            return fs.ToString();
        }

        public string NameFilter
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ExNameFilter
        {
            get { return _exname; }
            set { _exname = value; }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public List<string> BodyFilter
        {
            get { return _body; }
            set
            {
                _lambdaExp = null;
                _lambdaExpDelegate = null;
                _body = value;
            }
        }

        public string[] BodyFilterArray
        {
            get { return _body.ToArray(); }
            set
            {
                _body = new List<string>();
                foreach (string filter in value)
                {
                    _body.Add(filter);
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public List<string> ExBodyFilter
        {
            get { return _exbody; }
            set
            {
                _exlambdaExp = null;
                _exlambdaExpDelegate = null;
                _exbody = value;
            }
        }

        public string[] ExBodyFilterArray
        {
            get { return _exbody.ToArray(); }
            set
            {
                _exbody = new List<string>();
                foreach (string filter in value)
                {
                    _exbody.Add(filter);
                }
            }
        }

        public bool SearchBoth
        {
            get { return _searchBoth; }
            set { _searchBoth = value; }
        }

        public bool ExSearchBoth
        {
            get { return _exsearchBoth; }
            set { _exsearchBoth = value; }
        }

        public bool MoveFrom
        {
            get { return _moveFrom; }
            set { _moveFrom = value; }
        }

        public bool SetMark
        {
            get { return _setMark; }
            set { _setMark = value; }
        }

        public bool SearchUrl
        {
            get { return _searchUrl; }
            set { _searchUrl = value; }
        }

        public bool ExSearchUrl
        {
            get { return _exsearchUrl; }
            set { _exsearchUrl = value; }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set { _caseSensitive = value; }
        }

        public bool ExCaseSensitive
        {
            get { return _excaseSensitive; }
            set { _excaseSensitive = value; }
        }

        public bool UseLambda
        {
            get { return _useLambda; }
            set
            {
                _lambdaExp = null;
                _lambdaExpDelegate = null;
                _useLambda = value;
            }
        }

        public bool ExUseLambda
        {
            get { return _exuseLambda; }
            set
            {
                _exlambdaExp = null;
                _exlambdaExpDelegate = null;
                _exuseLambda = value;
            }
        }

        public bool UseRegex
        {
            get { return _useRegex; }
            set { _useRegex = value; }
        }

        public bool ExUseRegex
        {
            get { return _exuseRegex; }
            set { _exuseRegex = value; }
        }

        public bool IsRt
        {
            get { return _isRt; }
            set { _isRt = value; }
        }

        public bool IsExRt
        {
            get { return _isExRt; }
            set { _isExRt = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public string ExSource
        {
            get { return _exSource; }
            set { _exSource = value; }
        }

        public override string ToString()
        {
            return MakeSummary();
        }

        public bool ExecuteLambdaExpression(string expr, PostClass post)
        {
            if (_lambdaExp == null || _lambdaExpDelegate == null)
            {
                _lambdaExp = DynamicExpression.ParseLambda<PostClass, bool>(expr, post);
                _lambdaExpDelegate = _lambdaExp.Compile();
            }
            return ((bool)_lambdaExpDelegate.DynamicInvoke(post));
        }

        public bool ExecuteExLambdaExpression(string expr, PostClass post)
        {
            if (_exlambdaExp == null || _exlambdaExpDelegate == null)
            {
                _exlambdaExp = DynamicExpression.ParseLambda<PostClass, bool>(expr, post);
                _exlambdaExpDelegate = _exlambdaExp.Compile();
            }
            return ((bool)_exlambdaExpDelegate.DynamicInvoke(post));
        }

        public Tween.MyCommon.HITRESULT IsHit(PostClass post)
        {
            bool bHit = true;
            string tBody = null;
            string tSource = null;
            if (_searchUrl)
            {
                tBody = post.Text;
                tSource = post.SourceHtml;
            }
            else
            {
                tBody = post.TextFromApi;
                tSource = post.Source;
            }
            //検索オプション
            System.StringComparison compOpt = default(System.StringComparison);
            System.Text.RegularExpressions.RegexOptions rgOpt = default(System.Text.RegularExpressions.RegexOptions);
            if (_caseSensitive)
            {
                compOpt = StringComparison.Ordinal;
                rgOpt = RegexOptions.None;
            }
            else
            {
                compOpt = StringComparison.OrdinalIgnoreCase;
                rgOpt = RegexOptions.IgnoreCase;
            }
            if (_searchBoth)
            {
                if (string.IsNullOrEmpty(_name) || (!_useRegex && (post.ScreenName.Equals(_name, compOpt) || post.RetweetedBy.Equals(_name, compOpt))) || (_useRegex && (Regex.IsMatch(post.ScreenName, _name, rgOpt) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _name, rgOpt)))))
                {
                    if (_useLambda)
                    {
                        if (!ExecuteLambdaExpression(_body[0], post))
                            bHit = false;
                    }
                    else
                    {
                        foreach (string fs in _body)
                        {
                            if (_useRegex)
                            {
                                if (!Regex.IsMatch(tBody, fs, rgOpt))
                                    bHit = false;
                            }
                            else
                            {
                                if (_caseSensitive)
                                {
                                    if (!tBody.Contains(fs))
                                        bHit = false;
                                }
                                else
                                {
                                    if (!tBody.ToLower().Contains(fs.ToLower()))
                                        bHit = false;
                                }
                            }
                            if (!bHit)
                                break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
                else
                {
                    bHit = false;
                }
            }
            else
            {
                if (_useLambda)
                {
                    if (!ExecuteLambdaExpression(_body[0], post))
                        bHit = false;
                }
                else
                {
                    foreach (string fs in _body)
                    {
                        if (_useRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, rgOpt) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) || Regex.IsMatch(tBody, fs, rgOpt)))
                                bHit = false;
                        }
                        else
                        {
                            if (_caseSensitive)
                            {
                                if (!(post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || tBody.Contains(fs)))
                                    bHit = false;
                            }
                            else
                            {
                                if (!(post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || tBody.ToLower().Contains(fs.ToLower())))
                                    bHit = false;
                            }
                        }
                        if (!bHit)
                            break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            if (_isRt)
            {
                if (post.RetweetedId == 0)
                    bHit = false;
            }
            if (!string.IsNullOrEmpty(_source))
            {
                if (_useRegex)
                {
                    if (!Regex.IsMatch(tSource, _source, rgOpt))
                        bHit = false;
                }
                else
                {
                    if (!tSource.Equals(_source, compOpt))
                        bHit = false;
                }
            }
            if (bHit)
            {
                //除外判定
                if (_exsearchUrl)
                {
                    tBody = post.Text;
                    tSource = post.SourceHtml;
                }
                else
                {
                    tBody = post.TextFromApi;
                    tSource = post.Source;
                }

                bool exFlag = false;
                if (!string.IsNullOrEmpty(_exname) || _exbody.Count > 0)
                {
                    if (_excaseSensitive)
                    {
                        compOpt = StringComparison.Ordinal;
                        rgOpt = RegexOptions.None;
                    }
                    else
                    {
                        compOpt = StringComparison.OrdinalIgnoreCase;
                        rgOpt = RegexOptions.IgnoreCase;
                    }
                    if (_exsearchBoth)
                    {
                        if (string.IsNullOrEmpty(_exname) || (!_exuseRegex && (post.ScreenName.Equals(_exname, compOpt) || post.RetweetedBy.Equals(_exname, compOpt))) || (_exuseRegex && (Regex.IsMatch(post.ScreenName, _exname, rgOpt) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _exname, rgOpt)))))
                        {
                            if (_exbody.Count > 0)
                            {
                                if (_exuseLambda)
                                {
                                    if (ExecuteExLambdaExpression(_exbody[0], post))
                                        exFlag = true;
                                }
                                else
                                {
                                    foreach (string fs in _exbody)
                                    {
                                        if (_exuseRegex)
                                        {
                                            if (Regex.IsMatch(tBody, fs, rgOpt))
                                                exFlag = true;
                                        }
                                        else
                                        {
                                            if (_excaseSensitive)
                                            {
                                                if (tBody.Contains(fs))
                                                    exFlag = true;
                                            }
                                            else
                                            {
                                                if (tBody.ToLower().Contains(fs.ToLower()))
                                                    exFlag = true;
                                            }
                                        }
                                        if (exFlag)
                                            break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                            else
                            {
                                exFlag = true;
                            }
                        }
                    }
                    else
                    {
                        if (_exuseLambda)
                        {
                            if (ExecuteExLambdaExpression(_exbody[0], post))
                                exFlag = true;
                        }
                        else
                        {
                            foreach (string fs in _exbody)
                            {
                                if (_exuseRegex)
                                {
                                    if (Regex.IsMatch(post.ScreenName, fs, rgOpt) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) || Regex.IsMatch(tBody, fs, rgOpt))
                                        exFlag = true;
                                }
                                else
                                {
                                    if (_excaseSensitive)
                                    {
                                        if (post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || tBody.Contains(fs))
                                            exFlag = true;
                                    }
                                    else
                                    {
                                        if (post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || tBody.ToLower().Contains(fs.ToLower()))
                                            exFlag = true;
                                    }
                                }
                                if (exFlag)
                                    break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
                if (_isExRt)
                {
                    if (post.RetweetedId > 0)
                        exFlag = true;
                }
                if (!string.IsNullOrEmpty(_exSource))
                {
                    if (_exuseRegex)
                    {
                        if (Regex.IsMatch(tSource, _exSource, rgOpt))
                            exFlag = true;
                    }
                    else
                    {
                        if (tSource.Equals(_exSource, compOpt))
                            exFlag = true;
                    }
                }

                if (string.IsNullOrEmpty(_name) && _body.Count == 0 && !_isRt && string.IsNullOrEmpty(_source))
                {
                    bHit = false;
                }
                if (bHit)
                {
                    if (!exFlag)
                    {
                        if (_moveFrom)
                        {
                            return Tween.MyCommon.HITRESULT.Move;
                        }
                        else
                        {
                            if (_setMark)
                            {
                                return Tween.MyCommon.HITRESULT.CopyAndMark;
                            }
                            return Tween.MyCommon.HITRESULT.Copy;
                        }
                    }
                    else
                    {
                        return Tween.MyCommon.HITRESULT.Exclude;
                    }
                }
                else
                {
                    if (exFlag)
                    {
                        return Tween.MyCommon.HITRESULT.Exclude;
                    }
                    else
                    {
                        return Tween.MyCommon.HITRESULT.None;
                    }
                }
            }
            else
            {
                return Tween.MyCommon.HITRESULT.None;
            }
        }

        public bool Equals(FiltersClass other)
        {
            if (this.BodyFilter.Count != other.BodyFilter.Count)
                return false;
            if (this.ExBodyFilter.Count != other.ExBodyFilter.Count)
                return false;
            for (int i = 0; i <= this.BodyFilter.Count - 1; i++)
            {
                if (this.BodyFilter[i] != other.BodyFilter[i])
                    return false;
            }
            for (int i = 0; i <= this.ExBodyFilter.Count - 1; i++)
            {
                if (this.ExBodyFilter[i] != other.ExBodyFilter[i])
                    return false;
            }

            return (this.MoveFrom == other.MoveFrom) & (this.SetMark == other.SetMark) & (this.NameFilter == other.NameFilter) & (this.SearchBoth == other.SearchBoth) & (this.SearchUrl == other.SearchUrl) & (this.UseRegex == other.UseRegex) & (this.ExNameFilter == other.ExNameFilter) & (this.ExSearchBoth == other.ExSearchBoth) & (this.ExSearchUrl == other.ExSearchUrl) & (this.ExUseRegex == other.ExUseRegex) & (this.IsRt == other.IsRt) & (this.Source == other.Source) & (this.IsExRt == other.IsExRt) & (this.ExSource == other.ExSource) & (this.UseLambda == other.UseLambda) & (this.ExUseLambda == other.ExUseLambda);
        }

        public FiltersClass CopyTo(FiltersClass destination)
        {
            if (this.BodyFilter.Count > 0)
            {
                foreach (string flt in this.BodyFilter)
                {
                    destination.BodyFilter.Add(string.Copy(flt));
                }
            }

            if (this.ExBodyFilter.Count > 0)
            {
                foreach (string flt in this.ExBodyFilter)
                {
                    destination.ExBodyFilter.Add(string.Copy(flt));
                }
            }

            destination.MoveFrom = this.MoveFrom;
            destination.SetMark = this.SetMark;
            destination.NameFilter = this.NameFilter;
            destination.SearchBoth = this.SearchBoth;
            destination.SearchUrl = this.SearchUrl;
            destination.UseRegex = this.UseRegex;
            destination.ExNameFilter = this.ExNameFilter;
            destination.ExSearchBoth = this.ExSearchBoth;
            destination.ExSearchUrl = this.ExSearchUrl;
            destination.ExUseRegex = this.ExUseRegex;
            destination.IsRt = this.IsRt;
            destination.Source = this.Source;
            destination.IsExRt = this.IsExRt;
            destination.ExSource = this.ExSource;
            destination.UseLambda = this.UseLambda;
            destination.ExUseLambda = this.ExUseLambda;
            return destination;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
                return false;
            return this.Equals((FiltersClass)obj);
        }

        public override int GetHashCode()
        {
            return this.MoveFrom.GetHashCode() ^ this.SetMark.GetHashCode() ^ this.BodyFilter.GetHashCode() ^ this.NameFilter.GetHashCode() ^ this.SearchBoth.GetHashCode() ^ this.SearchUrl.GetHashCode() ^ this.UseRegex.GetHashCode() ^ this.ExBodyFilter.GetHashCode() ^ this.ExNameFilter.GetHashCode() ^ this.ExSearchBoth.GetHashCode() ^ this.ExSearchUrl.GetHashCode() ^ this.ExUseRegex.GetHashCode() ^ this.IsRt.GetHashCode() ^ this.Source.GetHashCode() ^ this.IsExRt.GetHashCode() ^ this.ExSource.GetHashCode() ^ this.UseLambda.GetHashCode() ^ this.ExUseLambda.GetHashCode();
        }
    }

    //ソート比較クラス：ID比較のみ
    public sealed class IdComparerClass : IComparer<long>
    {
        /// <summary>
        /// 比較する方法
        /// </summary>
        public enum ComparerMode
        {
            Id,
            Data,
            Name,
            Nickname,
            Source
        }

        private SortOrder _order;
        private ComparerMode _mode;
        private Dictionary<long, PostClass> _statuses;

        private Comparison<long> _CmpMethod;

        /// <summary>
        /// 昇順か降順か Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public SortOrder Order
        {
            get { return _order; }
            set
            {
                _order = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// 並び替えの方法 Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public ComparerMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// ListViewItemComparerクラスのコンストラクタ（引数付は未使用）
        /// </summary>
        /// <param name="col">並び替える列番号</param>
        /// <param name="ord">昇順か降順か</param>
        /// <param name="cmod">並び替えの方法</param>

        public IdComparerClass()
        {
            _order = SortOrder.Ascending;
            _mode = ComparerMode.Id;
            SetCmpMethod(_mode, _order);
        }

        public Dictionary<long, PostClass> posts
        {
            get { return _statuses; }
            set { _statuses = value; }
        }

        // 指定したソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        public Comparison<long> CmpMethod(ComparerMode _sortmode, SortOrder _sortorder)
        {
            //get
            {
                Comparison<long> _method = null;
                if (_sortorder == SortOrder.Ascending)
                {
                    // 昇順
                    switch (_sortmode)
                    {
                        case ComparerMode.Data:
                            _method = Compare_ModeData_Ascending;
                            break;
                        case ComparerMode.Id:
                            _method = Compare_ModeId_Ascending;
                            break;
                        case ComparerMode.Name:
                            _method = Compare_ModeName_Ascending;
                            break;
                        case ComparerMode.Nickname:
                            _method = Compare_ModeNickName_Ascending;
                            break;
                        case ComparerMode.Source:
                            _method = Compare_ModeSource_Ascending;
                            break;
                    }
                }
                else
                {
                    // 降順
                    switch (_sortmode)
                    {
                        case ComparerMode.Data:
                            _method = Compare_ModeData_Descending;
                            break;
                        case ComparerMode.Id:
                            _method = Compare_ModeId_Descending;
                            break;
                        case ComparerMode.Name:
                            _method = Compare_ModeName_Descending;
                            break;
                        case ComparerMode.Nickname:
                            _method = Compare_ModeNickName_Descending;
                            break;
                        case ComparerMode.Source:
                            _method = Compare_ModeSource_Descending;
                            break;
                    }
                }
                return _method;
            }
        }

        // ソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        // (overload 現在の使用中の比較関数のアドレスを返す)
        public Comparison<long> CmpMethod()
        {
            { return _CmpMethod; }
        }

        // ソートモードとソートオーダーに従い比較関数のアドレスを切り替え
        private void SetCmpMethod(ComparerMode mode, SortOrder order)
        {
            _CmpMethod = this.CmpMethod(mode, order);
        }

        //xがyより小さいときはマイナスの数、大きいときはプラスの数、
        //同じときは0を返す (こちらは未使用 一応比較関数群呼び出しの形のまま残しておく)
        public int Compare(long x, long y)
        {
            return _CmpMethod(x, y);
        }

        // 比較用関数群 いずれもステータスIDの順序を考慮する
        // 本文比較　昇順
        public int Compare_ModeData_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].TextFromApi, _statuses[y].TextFromApi);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // 本文比較　降順
        public int Compare_ModeData_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].TextFromApi, _statuses[x].TextFromApi);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // ステータスID比較　昇順
        public int Compare_ModeId_Ascending(long x, long y)
        {
            return x.CompareTo(y);
        }

        // ステータスID比較　降順
        public int Compare_ModeId_Descending(long x, long y)
        {
            return y.CompareTo(x);
        }

        // 表示名比較　昇順
        public int Compare_ModeName_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].ScreenName, _statuses[y].ScreenName);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // 表示名比較　降順
        public int Compare_ModeName_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].ScreenName, _statuses[x].ScreenName);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // ユーザー名比較　昇順
        public int Compare_ModeNickName_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].Nickname, _statuses[y].Nickname);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // ユーザー名比較　降順
        public int Compare_ModeNickName_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].Nickname, _statuses[x].Nickname);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // Source比較　昇順
        public int Compare_ModeSource_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].Source, _statuses[y].Source);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // Source比較　降順
        public int Compare_ModeSource_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].Source, _statuses[x].Source);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }
    }
}