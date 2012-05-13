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
}