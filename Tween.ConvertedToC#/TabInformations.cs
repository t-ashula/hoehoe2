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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Hoehoe
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
        private int _addCount;
        private string _soundFile;
        private List<PostClass> _notifyPosts;
        private readonly object _lockObj = new object();
        private readonly object _lockUnread = new object();
        private static TabInformations _instance = new TabInformations();
        private List<ListElement> _lists = new List<ListElement>();

        private class ScrubGeoInfo
        {
            public long UserId;
            public long UpToStatusId;
        }

        public List<long> BlockIds = new List<long>();

        private TabInformations()
        {
            _sorter = new IdComparerClass();
        }

        public static TabInformations GetInstance()
        {
            return _instance;
        }

        public List<ListElement> SubscribableLists
        {
            get { return _lists; }
            set
            {
                if (value != null && value.Count > 0)
                {
                    foreach (TabClass tb in this.GetTabsByType(MyCommon.TabUsageType.Lists))
                    {
                        foreach (ListElement list in value)
                        {
                            if (tb.ListInfo.Id == list.Id)
                            {
                                tb.ListInfo = list;
                                break;
                            }
                        }
                    }
                }
                _lists = value;
            }
        }

        public bool AddTab(string tabName, MyCommon.TabUsageType tabType, ListElement list)
        {
            if (_tabs.ContainsKey(tabName))
            {
                return false;
            }
            _tabs.Add(tabName, new TabClass(tabName, tabType, list));
            _tabs[tabName].Sorter.Mode = _sorter.Mode;
            _tabs[tabName].Sorter.Order = _sorter.Order;
            return true;
        }

        public void RemoveTab(string tabName)
        {
            lock (_lockObj)
            {
                if (IsDefaultTab(tabName))
                {
                    return;
                }
                //念のため
                if (!_tabs[tabName].IsInnerStorageTabType)
                {
                    TabClass homeTab = GetTabByType(MyCommon.TabUsageType.Home);
                    string dmName = GetTabByType(MyCommon.TabUsageType.DirectMessage).TabName;

                    for (int idx = 0; idx < _tabs[tabName].AllCount; idx++)
                    {
                        bool exist = false;
                        long id = _tabs[tabName].GetId(idx);
                        if (id < 0)
                        {
                            continue;
                        }
                        foreach (string key in _tabs.Keys)
                        {
                            if (!(key == tabName) && key != dmName)
                            {
                                if (_tabs[key].Contains(id))
                                {
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        if (!exist)
                        {
                            homeTab.Add(id, _statuses[id].IsRead, false);
                        }
                    }
                }
                _removedTab.Push(_tabs[tabName]);
                _tabs.Remove(tabName);
            }
        }

        public Stack<TabClass> RemovedTab { get { return _removedTab; } }

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

        public PostClass RetweetSource(long id)
        {
            if (_retweets.ContainsKey(id))
            {
                return _retweets[id];
            }
            else
            {
                return null;
            }
        }

        public void RemoveFavPost(long id)
        {
            lock (_lockObj)
            {
                PostClass post = null;
                TabClass tab = this.GetTabByType(MyCommon.TabUsageType.Favorites);
                string tn = tab.TabName;
                if (_statuses.ContainsKey(id))
                {
                    post = _statuses[id];
                    //指定タブから該当ID削除
                    MyCommon.TabUsageType tType = tab.TabType;
                    if (tab.Contains(id))
                    {
                        //未読管理
                        if (tab.UnreadManage && !post.IsRead)
                        {
                            lock (_lockUnread)
                            {
                                tab.UnreadCount -= 1;
                                this.SetNextUnreadId(id, tab);
                            }
                        }
                        tab.Remove(id);
                    }
                    //FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                    if (tType == MyCommon.TabUsageType.Favorites && post.RetweetedId > 0)
                    {
                        for (int i = 0; i < tab.AllCount; i++)
                        {
                            PostClass rPost = null;
                            try
                            {
                                rPost = this.Item(tn, i);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                break;
                            }
                            if (rPost.RetweetedId > 0 && rPost.RetweetedId == post.RetweetedId)
                            {
                                //未読管理
                                if (tab.UnreadManage && !rPost.IsRead)
                                {
                                    lock (_lockUnread)
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
            }
        }

        public void ScrubGeoReserve(long id, long upToStatusId)
        {
            lock (_lockObj)
            {
                this.ScrubGeo(id, upToStatusId);
            }
        }

        private void ScrubGeo(long userId, long upToStatusId)
        {
            lock (_lockObj)
            {
                var userPosts = from post in this._statuses.Values
                                where post.UserId == userId && post.UserId <= upToStatusId
                                select post;

                foreach (var p in userPosts)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }

                var userPosts2 = from tb in this.GetTabsInnerStorageType()
                                 from post in tb.Posts.Values
                                 where post.UserId == userId && post.UserId <= upToStatusId
                                 select post;

                foreach (var p in userPosts2)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }
            }
        }

        public void RemovePostReserve(long id)
        {
            lock (_lockObj)
            {
                this._deletedIds.Add(id);
                this.DeletePost(id);
                //UI選択行がずれるため、RemovePostは使用しない
            }
        }

        public void RemovePost(long id)
        {
            lock (_lockObj)
            {
                PostClass post = null;
                //If _statuses.ContainsKey(Id) Then
                //各タブから該当ID削除
                foreach (string key in _tabs.Keys)
                {
                    TabClass tab = _tabs[key];
                    if (tab.Contains(id))
                    {
                        if (!tab.IsInnerStorageTabType)
                        {
                            post = _statuses[id];
                            //未読管理
                            if (tab.UnreadManage && !post.IsRead)
                            {
                                lock (_lockUnread)
                                {
                                    tab.UnreadCount -= 1;
                                    this.SetNextUnreadId(id, tab);
                                }
                            }
                            //未読数がずれる可能性があるためtab.Postsの未読も確認する
                        }
                        else
                        {
                            //未読管理
                            if (tab.UnreadManage && !tab.Posts[id].IsRead)
                            {
                                lock (_lockUnread)
                                {
                                    tab.UnreadCount -= 1;
                                    this.SetNextUnreadId(id, tab);
                                }
                            }
                        }
                        tab.Remove(id);
                    }
                }
                if (_statuses.ContainsKey(id))
                {
                    _statuses.Remove(id);
                }
                //End If
            }
        }

        private void DeletePost(long id)
        {
            lock (_lockObj)
            {
                PostClass post = null;
                if (_statuses.ContainsKey(id))
                {
                    post = _statuses[id];
                    post.IsDeleted = true;
                }
                foreach (TabClass tb in this.GetTabsInnerStorageType())
                {
                    if (tb.Contains(id))
                    {
                        post = tb.Posts[id];
                        post.IsDeleted = true;
                    }
                }
            }
        }

        public int GetOldestUnreadIndex(string tabName)
        {
            TabClass tb = _tabs[tabName];
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
                    lock (_lockUnread)
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
                if (!(tb.UnreadManage && AppendSettingDialog.Instance.UnreadManage))
                {
                    return -1;
                }
                lock (_lockUnread)
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
            }
        }

        private void SetNextUnreadId(long currentId, TabClass tab)
        {
            //CurrentID:今既読にしたID(OldestIDの可能性あり)
            //最古未読が設定されていて、既読の場合（1発言以上存在）
            try
            {
                Dictionary<long, PostClass> posts = null;
                if (!tab.IsInnerStorageTabType)
                {
                    posts = _statuses;
                }
                else
                {
                    posts = tab.Posts;
                }
                //次の未読探索
                if (tab.OldestUnreadId > -1 && posts.ContainsKey(tab.OldestUnreadId) && posts[tab.OldestUnreadId].IsRead && _sorter.Mode == IdComparerClass.ComparerMode.Id)
                {
                    if (tab.UnreadCount == 0)
                    {
                        //未読数０→最古未読なし
                        tab.OldestUnreadId = -1;
                    }
                    else if (tab.OldestUnreadId == currentId && currentId > -1)
                    {
                        //最古IDを既読にしたタイミング→次のIDから続けて探索
                        int idx = tab.IndexOf(currentId);
                        if (idx > -1)
                        {
                            //続きから探索
                            FindUnreadId(idx, tab);
                        }
                        else
                        {
                            //頭から探索
                            FindUnreadId(-1, tab);
                        }
                    }
                    else
                    {
                        //頭から探索
                        FindUnreadId(-1, tab);
                    }
                }
                else
                {
                    //頭から探索
                    FindUnreadId(-1, tab);
                }
            }
            catch (KeyNotFoundException)
            {
                //頭から探索
                FindUnreadId(-1, tab);
            }
        }

        private void FindUnreadId(int startIdx, TabClass tab)
        {
            if (tab.AllCount == 0)
            {
                tab.OldestUnreadId = -1;
                tab.UnreadCount = 0;
                return;
            }
            int toIdx = 0;
            int stp = 1;
            tab.OldestUnreadId = -1;
            if (_sorter.Order == System.Windows.Forms.SortOrder.Ascending)
            {
                if (startIdx == -1)
                {
                    startIdx = 0;
                }
                else
                {
                    //StartIdx += 1
                    if (startIdx > tab.AllCount - 1)
                    {
                        startIdx = tab.AllCount - 1;
                    }
                    //念のため
                }
                toIdx = tab.AllCount - 1;
                if (toIdx < 0)
                {
                    toIdx = 0;
                }
                //念のため
                stp = 1;
            }
            else
            {
                if (startIdx == -1)
                {
                    startIdx = tab.AllCount - 1;
                }
                else
                {
                }
                if (startIdx < 0)
                {
                    startIdx = 0;
                }
                //念のため
                toIdx = 0;
                stp = -1;
            }

            Dictionary<long, PostClass> posts = null;
            if (!tab.IsInnerStorageTabType)
            {
                posts = _statuses;
            }
            else
            {
                posts = tab.Posts;
            }

            for (int i = startIdx; i <= toIdx; i += stp)
            {
                long id = tab.GetId(i);
                if (id > -1 && !posts[id].IsRead)
                {
                    tab.OldestUnreadId = id;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        public int DistributePosts()
        {
            lock (_lockObj)
            {
                //戻り値は追加件数
                //If _addedIds Is Nothing Then Return 0
                //If _addedIds.Count = 0 Then Return 0

                if (_addedIds == null)
                {
                    _addedIds = new List<long>();
                }
                if (_notifyPosts == null)
                {
                    _notifyPosts = new List<PostClass>();
                }
                try
                {
                    this.Distribute();
                    //タブに仮振分
                }
                catch (KeyNotFoundException)
                {
                    //タブ変更により振分が失敗した場合
                }
                int retCnt = _addedIds.Count;
                _addCount += retCnt;
                _addedIds.Clear();
                _addedIds = null;
                //後始末
                return retCnt;                //件数
            }
        }

        public int SubmitUpdate(ref string soundFile, ref PostClass[] notifyPosts, ref bool isMentionIncluded, ref bool isDeletePost, bool isUserStream)
        {
            //注：メインスレッドから呼ぶこと
            lock (_lockObj)
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
            TabClass homeTab = GetTabByType(MyCommon.TabUsageType.Home);
            TabClass replyTab = GetTabByType(MyCommon.TabUsageType.Mentions);
            TabClass dmTab = GetTabByType(MyCommon.TabUsageType.DirectMessage);
            TabClass favTab = GetTabByType(MyCommon.TabUsageType.Favorites);
            foreach (long id in _addedIds)
            {
                PostClass post = _statuses[id];
                bool @add = false;
                //通知リスト追加フラグ
                bool mv = false;
                //移動フラグ（Recent追加有無）
                MyCommon.HITRESULT rslt = MyCommon.HITRESULT.None;
                post.IsExcludeReply = false;
                foreach (string tn in _tabs.Keys)
                {
                    rslt = _tabs[tn].AddFiltered(post);
                    if (rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
                    {
                        if (rslt == MyCommon.HITRESULT.CopyAndMark)
                        {
                            post.IsMark = true;
                        }
                        //マークあり
                        if (rslt == MyCommon.HITRESULT.Move)
                        {
                            mv = true; //移動
                            post.IsMark = false;
                        }
                        if (_tabs[tn].Notify)
                        {
                            @add = true;
                        }
                        //通知あり
                        if (!String.IsNullOrEmpty(_tabs[tn].SoundFile) && String.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = _tabs[tn].SoundFile;
                            //wavファイル（未設定の場合のみ）
                        }
                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == MyCommon.HITRESULT.Exclude && _tabs[tn].TabType == MyCommon.TabUsageType.Mentions)
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
                    if (!String.IsNullOrEmpty(homeTab.SoundFile) && String.IsNullOrEmpty(_soundFile))
                    {
                        _soundFile = homeTab.SoundFile;
                    }

                    if (homeTab.Notify)
                    {
                        @add = true;
                    }
                }
                //除外ルール適用のないReplyならReplyタブに追加
                if (post.IsReply && !post.IsExcludeReply)
                {
                    replyTab.Add(post.StatusId, post.IsRead, true);
                    if (!String.IsNullOrEmpty(replyTab.SoundFile))
                    {
                        _soundFile = replyTab.SoundFile;
                    }
                    if (replyTab.Notify)
                    {
                        @add = true;
                    }
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
                        if (!String.IsNullOrEmpty(favTab.SoundFile) && String.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = favTab.SoundFile;
                        }
                        if (favTab.Notify)
                        {
                            @add = true;
                        }
                    }
                }
                if (@add)
                {
                    _notifyPosts.Add(post);
                }
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
                                {
                                    _notifyPosts.Add(post);
                                }
                            }
                            if (!String.IsNullOrEmpty(tb.SoundFile))
                            {
                                if (tb.TabType == MyCommon.TabUsageType.DirectMessage || String.IsNullOrEmpty(_soundFile))
                                {
                                    _soundFile = tb.SoundFile;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddPost(PostClass item)
        {
            lock (_lockObj)
            {
                if (String.IsNullOrEmpty(item.RelTabName))
                {
                    if (!item.IsDm)
                    {
                        if (_statuses.ContainsKey(item.StatusId))
                        {
                            if (item.IsFav)
                            {
                                if (item.RetweetedId == 0)
                                {
                                    _statuses[item.StatusId].IsFav = true;
                                }
                                else
                                {
                                    item.IsFav = false;
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
                            if (item.IsFav && item.RetweetedId > 0)
                            {
                                item.IsFav = false;
                            }
                            //既に持っている公式RTは捨てる
                            if (AppendSettingDialog.Instance.HideDuplicatedRetweets && !item.IsMe && this._retweets.ContainsKey(item.RetweetedId) && this._retweets[item.RetweetedId].RetweetedCount > 0)
                            {
                                return;
                            }
                            if (BlockIds.Contains(item.UserId))
                            {
                                return;
                            }
                            _statuses.Add(item.StatusId, item);
                        }
                        if (item.RetweetedId > 0)
                        {
                            this.AddRetweet(item);
                        }
                        if (item.IsFav && _retweets.ContainsKey(item.StatusId))
                        {
                            return;
                            //Fav済みのRetweet元発言は追加しない
                        }
                        if (_addedIds == null)
                        {
                            _addedIds = new List<long>();
                        }
                        //タブ追加用IDコレクション準備
                        _addedIds.Add(item.StatusId);
                    }
                    else
                    {
                        //DM
                        TabClass tb = this.GetTabByType(MyCommon.TabUsageType.DirectMessage);
                        if (tb.Contains(item.StatusId))
                        {
                            return;
                        }
                        tb.AddPostToInnerStorage(item);
                    }
                }
                else
                {
                    //公式検索、リスト、関連発言の場合
                    TabClass tb = null;
                    if (this.Tabs.ContainsKey(item.RelTabName))
                    {
                        tb = this.Tabs[item.RelTabName];
                    }
                    else
                    {
                        return;
                    }
                    if (tb == null)
                    {
                        return;
                    }
                    if (tb.Contains(item.StatusId))
                    {
                        return;
                    }
                    tb.AddPostToInnerStorage(item);
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

        public void SetReadAllTab(bool read, string tabName, int index)
        {
            //Read:True=既読へ　False=未読へ
            TabClass tb = _tabs[tabName];

            if (!tb.UnreadManage)
            {
                return;
            }
            //未読管理していなければ終了

            long id = tb.GetId(index);
            if (id < 0)
            {
                return;
            }
            PostClass post = null;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[id];
            }
            else
            {
                post = tb.Posts[id];
            }

            if (post.IsRead == read)
            {
                return;
            }
            //状態変更なければ終了

            post.IsRead = read;

            lock (_lockUnread)
            {
                if (read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(id, tb);
                    //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        if (_statuses.ContainsKey(id) && !_statuses[id].IsRead)
                        {
                            foreach (string key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                                {
                                    _tabs[key].UnreadCount -= 1;
                                    if (_tabs[key].OldestUnreadId == id)
                                    {
                                        _tabs[key].OldestUnreadId = -1;
                                    }
                                }
                            }
                            _statuses[id].IsRead = true;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (string key in _tabs.Keys)
                        {
                            if (key != tabName && _tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount -= 1;
                                if (_tabs[key].OldestUnreadId == id)
                                {
                                    _tabs[key].OldestUnreadId = -1;
                                }
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != tabName && _tabs[key].Contains(id) && _tabs[key].IsInnerStorageTabType && !_tabs[key].Posts[id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount -= 1;
                                if (_tabs[key].OldestUnreadId == id)
                                {
                                    _tabs[key].OldestUnreadId = -1;
                                }
                            }
                            _tabs[key].Posts[id].IsRead = true;
                        }
                    }
                }
                else
                {
                    tb.UnreadCount += 1;
                    //If tb.OldestUnreadId > Id OrElse tb.OldestUnreadId = -1 Then tb.OldestUnreadId = Id
                    if (tb.OldestUnreadId > id)
                    {
                        tb.OldestUnreadId = id;
                    }
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        if (_statuses.ContainsKey(id) && _statuses[id].IsRead)
                        {
                            foreach (string key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                                {
                                    _tabs[key].UnreadCount += 1;
                                    if (_tabs[key].OldestUnreadId > id)
                                    {
                                        _tabs[key].OldestUnreadId = id;
                                    }
                                }
                            }
                            _statuses[id].IsRead = false;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (string key in _tabs.Keys)
                        {
                            if (key != tabName && _tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount += 1;
                                if (_tabs[key].OldestUnreadId > id)
                                {
                                    _tabs[key].OldestUnreadId = id;
                                }
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != tabName && _tabs[key].Contains(id) && _tabs[key].IsInnerStorageTabType && _tabs[key].Posts[id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount += 1;
                                if (_tabs[key].OldestUnreadId > id)
                                {
                                    _tabs[key].OldestUnreadId = id;
                                }
                            }
                            _tabs[key].Posts[id].IsRead = false;
                        }
                    }
                }
            }
        }

        /// TODO: パフォーマンスを勘案して、戻すか決める
        public void SetRead(bool read, string tabName, int index)
        {
            //Read:True=既読へ　False=未読へ
            TabClass tb = _tabs[tabName];

            if (!tb.UnreadManage)
            {
                return;
            }
            //未読管理していなければ終了

            long id = tb.GetId(index);
            if (id < 0)
            {
                return;
            }
            PostClass post = null;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[id];
            }
            else
            {
                post = tb.Posts[id];
            }

            if (post.IsRead == read)
            {
                return;
            }
            //状態変更なければ終了

            post.IsRead = read;
            //指定の状態に変更

            lock (_lockUnread)
            {
                if (read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(id, tb);
                    //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        return;
                    }
                    foreach (string key in _tabs.Keys)
                    {
                        if (key != tabName && _tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount -= 1;
                            if (_tabs[key].OldestUnreadId == id)
                            {
                                _tabs[key].OldestUnreadId = -1;
                            }
                        }
                    }
                }
                else
                {
                    tb.UnreadCount += 1;
                    if (tb.OldestUnreadId > id)
                    {
                        tb.OldestUnreadId = id;
                    }
                    if (tb.IsInnerStorageTabType)
                    {
                        return;
                    }
                    foreach (string key in _tabs.Keys)
                    {
                        if (!(key == tabName) && _tabs[key].UnreadManage && _tabs[key].Contains(id) && !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount += 1;
                            if (_tabs[key].OldestUnreadId > id)
                            {
                                _tabs[key].OldestUnreadId = id;
                            }
                        }
                    }
                }
            }
        }

        public void SetRead()
        {
            TabClass tb = GetTabByType(MyCommon.TabUsageType.Home);
            if (!tb.UnreadManage)
            {
                return;
            }

            lock (_lockObj)
            {
                for (int i = 0; i < tb.AllCount; i++)
                {
                    long id = tb.GetId(i);
                    if (id < 0)
                    {
                        return;
                    }
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
                                {
                                    _tabs[key].OldestUnreadId = -1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public PostClass Item(long id)
        {
            if (_statuses.ContainsKey(id))
            {
                return _statuses[id];
            }
            foreach (TabClass tb in this.GetTabsInnerStorageType())
            {
                if (tb.Contains(id))
                {
                    return tb.Posts[id];
                }
            }
            return null;
        }

        public PostClass Item(string tabName, int index)
        {
            if (!_tabs.ContainsKey(tabName))
            {
                throw new ArgumentException("TabName=" + tabName + " is not contained.");
            }
            long id = _tabs[tabName].GetId(index);
            if (id < 0)
            {
                throw new ArgumentException("Index can't find. Index=" + index.ToString() + "/TabName=" + tabName);
            }
            try
            {
                if (_tabs[tabName].IsInnerStorageTabType)
                {
                    return _tabs[tabName].Posts[_tabs[tabName].GetId(index)];
                }
                else
                {
                    return _statuses[_tabs[tabName].GetId(index)];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Index=" + index.ToString() + "/TabName=" + tabName, ex);
            }
        }

        public PostClass[] Item(string tabName, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex + 1;
            PostClass[] posts = new PostClass[length];
            if (_tabs[tabName].IsInnerStorageTabType)
            {
                for (int i = 0; i < length; i++)
                {
                    posts[i] = _tabs[tabName].Posts[_tabs[tabName].GetId(startIndex + i)];
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    posts[i] = _statuses[_tabs[tabName].GetId(startIndex + i)];
                }
            }
            return posts;
        }

        public bool ContainsKey(long id)
        {
            //DM,公式検索は非対応
            lock (_lockObj)
            {
                return _statuses.ContainsKey(id);
            }
        }

        public bool ContainsKey(long id, string tabName)
        {
            //DM,公式検索は対応版
            lock (_lockObj)
            {
                if (_tabs.ContainsKey(tabName))
                {
                    return _tabs[tabName].Contains(id);
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetUnreadManage(bool manage)
        {
            if (manage)
            {
                foreach (string key in _tabs.Keys)
                {
                    TabClass tb = _tabs[key];
                    if (tb.UnreadManage)
                    {
                        lock (_lockUnread)
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
                                    {
                                        oldest = id;
                                    }
                                }
                            }
                            if (oldest == long.MaxValue)
                            {
                                oldest = -1;
                            }
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
                        lock (_lockUnread)
                        {
                            tb.UnreadCount = 0;
                            tb.OldestUnreadId = -1;
                        }
                    }
                }
            }
        }

        public void RenameTab(string original, string newName)
        {
            TabClass tb = _tabs[original];
            _tabs.Remove(original);
            tb.TabName = newName;
            _tabs.Add(newName, tb);
        }

        public void FilterAll()
        {
            lock (_lockObj)
            {
                TabClass tbr = GetTabByType(MyCommon.TabUsageType.Home);
                TabClass replyTab = GetTabByType(MyCommon.TabUsageType.Mentions);
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
                            {
                                continue;
                            }
                            MyCommon.HITRESULT rslt = MyCommon.HITRESULT.None;
                            rslt = tb.AddFiltered(post);
                            switch (rslt)
                            {
                                case MyCommon.HITRESULT.CopyAndMark:
                                    post.IsMark = true;             //マークあり
                                    post.FilterHit = true;
                                    break;
                                case MyCommon.HITRESULT.Move:
                                    tbr.Remove(post.StatusId, post.IsRead);
                                    post.IsMark = false;
                                    post.FilterHit = true;
                                    break;
                                case MyCommon.HITRESULT.Copy:
                                    post.IsMark = false;
                                    post.FilterHit = true;
                                    break;
                                case MyCommon.HITRESULT.Exclude:
                                    if (tb.TabName == replyTab.TabName && post.IsReply)
                                    {
                                        post.IsExcludeReply = true;
                                    }
                                    if (post.IsFav)
                                    {
                                        GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                    }
                                    post.FilterHit = false;
                                    break;
                                case MyCommon.HITRESULT.None:
                                    if (tb.TabName == replyTab.TabName && post.IsReply)
                                    {
                                        replyTab.Add(post.StatusId, post.IsRead, true);
                                    }
                                    if (post.IsFav)
                                    {
                                        GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                    }
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
                                    break;
                                }
                            }
                            if (!hit)
                            {
                                tbr.Add(id, _statuses[id].IsRead, false);
                            }
                        }
                    }
                }
                this.SortPosts();
            }
        }

        public long[] GetId(string tabName, System.Windows.Forms.ListView.SelectedIndexCollection indexCollection)
        {
            if (indexCollection.Count == 0)
            {
                return null;
            }

            TabClass tb = _tabs[tabName];
            long[] ids = new long[indexCollection.Count];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = tb.GetId(indexCollection[i]);
            }
            return ids;
        }

        public long GetId(string tabName, int index)
        {
            return _tabs[tabName].GetId(index);
        }

        public int[] IndexOf(string tabName, long[] ids)
        {
            if (ids == null)
            {
                return null;
            }
            int[] idx = new int[ids.Length];
            TabClass tb = _tabs[tabName];
            for (int i = 0; i < ids.Length; i++)
            {
                idx[i] = tb.IndexOf(ids[i]);
            }
            return idx;
        }

        public int IndexOf(string tabName, long id)
        {
            return _tabs[tabName].IndexOf(id);
        }

        public void ClearTabIds(string tabName)
        {
            //不要なPostを削除
            lock (_lockObj)
            {
                if (!_tabs[tabName].IsInnerStorageTabType)
                {
                    foreach (long id in _tabs[tabName].BackupIds())
                    {
                        bool hit = false;
                        foreach (TabClass tb in _tabs.Values)
                        {
                            if (tb.Contains(id))
                            {
                                hit = true;
                                break;
                            }
                        }
                        if (!hit)
                        {
                            _statuses.Remove(id);
                        }
                    }
                }
                //指定タブをクリア
                _tabs[tabName].ClearIDs();
            }
        }

        public void SetTabUnreadManage(string tabName, bool manage)
        {
            TabClass tb = _tabs[tabName];
            lock (_lockUnread)
            {
                if (manage)
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
                            {
                                oldest = id;
                            }
                        }
                    }
                    if (oldest == long.MaxValue)
                    {
                        oldest = -1;
                    }
                    tb.OldestUnreadId = oldest;
                    tb.UnreadCount = cnt;
                }
                else
                {
                    tb.OldestUnreadId = -1;
                    tb.UnreadCount = 0;
                }
            }
            tb.UnreadManage = manage;
        }

        public void RefreshOwl(List<long> follower)
        {
            lock (_lockObj)
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

        public TabClass GetTabByType(MyCommon.TabUsageType tabType)
        {
            //Home,Mentions,DM,Favは1つに制限する
            //その他のタイプを指定されたら、最初に合致したものを返す
            //合致しなければNothingを返す
            lock (_lockObj)
            {
                foreach (TabClass tb in _tabs.Values)
                {
                    if (tb != null && tb.TabType == tabType)
                    {
                        return tb;
                    }
                }
                return null;
            }
        }

        public List<TabClass> GetTabsByType(MyCommon.TabUsageType tabType)
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (_lockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in _tabs.Values)
                {
                    if ((tabType & tb.TabType) == tb.TabType)
                    {
                        tbs.Add(tb);
                    }
                }
                return tbs;
            }
        }

        public List<TabClass> GetTabsInnerStorageType()
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (_lockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in _tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                    {
                        tbs.Add(tb);
                    }
                }
                return tbs;
            }
        }

        public TabClass GetTabByName(string tabName)
        {
            lock (_lockObj)
            {
                if (_tabs.ContainsKey(tabName))
                {
                    return _tabs[tabName];
                }
                return null;
            }
        }

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            if (tabName != null && _tabs.ContainsKey(tabName) && (_tabs[tabName].TabType == MyCommon.TabUsageType.Home || _tabs[tabName].TabType == MyCommon.TabUsageType.Mentions || _tabs[tabName].TabType == MyCommon.TabUsageType.DirectMessage || _tabs[tabName].TabType == MyCommon.TabUsageType.Favorites))
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
            return tabName != null && this._tabs.ContainsKey(tabName) && (_tabs[tabName].TabType == MyCommon.TabUsageType.Mentions || _tabs[tabName].TabType == MyCommon.TabUsageType.UserDefined);
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
                    break;
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