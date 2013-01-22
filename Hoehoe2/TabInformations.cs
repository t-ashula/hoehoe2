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
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// 個別タブの情報をDictionaryで保持
    /// </summary>
    public sealed class TabInformations
    {
        private static readonly TabInformations instance = new TabInformations();

        private readonly object _lockObj;
        private readonly object _lockUnread;

        private readonly IdComparerClass _sorter;
        private readonly Dictionary<long, PostClass> _statuses;
        private List<long> _addedIds;
        private readonly List<long> _deletedIds;
        private readonly Dictionary<long, PostClass> _retweets;
        private readonly Stack<TabClass> _removedTab;
        private int _addCount;
        private string _soundFile;
        private List<PostClass> _notifyPosts;
        private List<ListElement> _lists;

        private TabInformations()
        {
            BlockIds = new List<long>();
            _lockUnread = new object();
            _lockObj = new object();
            _sorter = new IdComparerClass();
            Tabs = new Dictionary<string, TabClass>();
            _statuses = new Dictionary<long, PostClass>();
            _deletedIds = new List<long>();
            _retweets = new Dictionary<long, PostClass>();
            _removedTab = new Stack<TabClass>();
            _lists = new List<ListElement>();
        }

        public static TabInformations Instance
        {
            get
            {
                return instance;
            }
        }

        public List<long> BlockIds { get; private set; }

        public List<ListElement> SubscribableLists
        {
            get
            {
                return _lists;
            }

            set
            {
                if (value != null && value.Count > 0)
                {
                    foreach (TabClass tb in GetTabsByType(TabUsageType.Lists))
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

        public Stack<TabClass> RemovedTab
        {
            get { return _removedTab; }
        }

        public Dictionary<string, TabClass> Tabs { get; set; }

        public Dictionary<string, TabClass>.KeyCollection KeysTab
        {
            get { return Tabs.Keys; }
        }

        public SortOrder SortOrder
        {
            get
            {
                return _sorter.Order;
            }

            set
            {
                _sorter.Order = value;
                foreach (string key in Tabs.Keys)
                {
                    Tabs[key].Sorter.Order = value;
                }
            }
        }

        public IdComparerClass.ComparerMode SortMode
        {
            get
            {
                return _sorter.Mode;
            }

            set
            {
                _sorter.Mode = value;
                foreach (string key in Tabs.Keys)
                {
                    Tabs[key].Sorter.Mode = value;
                }
            }
        }

        public Dictionary<long, PostClass> Posts
        {
            get { return _statuses; }
        }

        public bool AddTab(string tabName, TabUsageType tabType, ListElement list)
        {
            if (Tabs.ContainsKey(tabName))
            {
                return false;
            }

            Tabs.Add(tabName, new TabClass(tabName, tabType, list));
            Tabs[tabName].Sorter.Mode = _sorter.Mode;
            Tabs[tabName].Sorter.Order = _sorter.Order;
            return true;
        }

        public void RemoveTab(string tabName)
        {
            lock (_lockObj)
            {
                if (IsDefaultTab(tabName))
                {
                    return; // 念のため
                }

                var removeTab = Tabs[tabName];
                if (!removeTab.IsInnerStorageTabType)
                {
                    // 削除されるタブに有った tweet が他のタブになければ hometab に書き戻し
                    var homeTab = GetTabByType(TabUsageType.Home);
                    var dmessageTabName = GetTabByType(TabUsageType.DirectMessage).TabName;
                    for (var idx = 0; idx < removeTab.AllCount; ++idx)
                    {
                        var id = removeTab.GetId(idx);
                        if (id < 0)
                        {
                            continue;
                        }

                        if (!Tabs.Keys.Where(t => t != tabName && t != dmessageTabName).Any(t => Tabs[t].Contains(id)))
                        {
                            homeTab.Add(id, _statuses[id].IsRead, false);
                        }
                    }
                }

                _removedTab.Push(removeTab);
                Tabs.Remove(tabName);
            }
        }

        public bool ContainsTab(string tabText)
        {
            return Tabs.ContainsKey(tabText);
        }

        public bool ContainsTab(TabClass ts)
        {
            return Tabs.ContainsValue(ts);
        }

        public void SortPosts()
        {
            foreach (string key in Tabs.Keys)
            {
                Tabs[key].Sort();
            }
        }

        public SortOrder ToggleSortOrder(IdComparerClass.ComparerMode sortMode)
        {
            if (_sorter.Mode == sortMode)
            {
                _sorter.Order = _sorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
                foreach (string key in Tabs.Keys)
                {
                    Tabs[key].Sorter.Order = _sorter.Order;
                }
            }
            else
            {
                _sorter.Mode = sortMode;
                _sorter.Order = SortOrder.Ascending;
                foreach (string key in Tabs.Keys)
                {
                    Tabs[key].Sorter.Mode = sortMode;
                    Tabs[key].Sorter.Order = SortOrder.Ascending;
                }
            }

            SortPosts();
            return _sorter.Order;
        }

        public PostClass RetweetSource(long id)
        {
            return _retweets.ContainsKey(id) ? _retweets[id] : null;
        }

        /// <summary>
        /// 指定タブから該当ID削除
        /// </summary>
        /// <param name="id"></param>
        public void RemoveFavPost(long id)
        {
            lock (_lockObj)
            {
                if (!_statuses.ContainsKey(id))
                {
                    return;
                }

                PostClass post = _statuses[id];
                TabClass tab = GetTabByType(TabUsageType.Favorites);
                string tn = tab.TabName;
                TabUsageType tabUsage = tab.TabType;
                if (tab.Contains(id))
                {
                    // 未読管理
                    if (tab.UnreadManage && !post.IsRead)
                    {
                        lock (_lockUnread)
                        {
                            tab.UnreadCount--;
                            SetNextUnreadId(id, tab);
                        }
                    }

                    tab.Remove(id);
                }

                // FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                if (tabUsage == TabUsageType.Favorites && post.IsRetweeted)
                {
                    for (var i = 0; i < tab.AllCount; ++i)
                    {
                        PostClass toRemovePost = null;
                        try
                        {
                            toRemovePost = Item(tn, i);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            break;
                        }

                        if (toRemovePost.IsRetweeted && toRemovePost.RetweetedId == post.RetweetedId)
                        {
                            // 未読管理
                            if (tab.UnreadManage && !toRemovePost.IsRead)
                            {
                                lock (_lockUnread)
                                {
                                    tab.UnreadCount--;
                                    SetNextUnreadId(toRemovePost.StatusId, tab);
                                }
                            }

                            tab.Remove(toRemovePost.StatusId);
                        }
                    }
                }
            }
        }

        public void ScrubGeoReserve(long id, long uptoId)
        {
            lock (_lockObj)
            {
                ScrubGeo(id, uptoId);
            }
        }

        public void RemovePostReserve(long id)
        {
            lock (_lockObj)
            {
                _deletedIds.Add(id);
                DeletePost(id);                // UI選択行がずれるため、RemovePostは使用しない
            }
        }

        /// <summary>
        /// 各タブから該当ID削除
        /// </summary>
        /// <param name="id"></param>
        public void RemovePost(long id)
        {
            lock (_lockObj)
            {
                foreach (string key in Tabs.Keys)
                {
                    TabClass tab = Tabs[key];
                    if (!tab.Contains(id))
                    {
                        continue;
                    }

                    // 未読管理 未読数がずれる可能性があるためtab.Postsの未読も確認する
                    if (tab.UnreadManage)
                    {
                        bool changeUnread = tab.IsInnerStorageTabType ? !tab.Posts[id].IsRead : !_statuses[id].IsRead;
                        if (changeUnread)
                        {
                            lock (_lockUnread)
                            {
                                tab.UnreadCount--;
                                SetNextUnreadId(id, tab);
                            }
                        }
                    }

                    tab.Remove(id);
                }

                if (_statuses.ContainsKey(id))
                {
                    _statuses.Remove(id);
                }
            }
        }

        public int GetOldestUnreadIndex(string tabName)
        {
            TabClass tb = Tabs[tabName];
            var oldest = tb.OldestUnreadId;
            if (oldest > -1 && tb.Contains(oldest) && tb.UnreadCount > 0)
            {
                // 未読アイテムへ
                bool isRead = tb.IsInnerStorageTabType ? tb.Posts[oldest].IsRead : _statuses[oldest].IsRead;
                if (!isRead)
                {
                    return tb.IndexOf(oldest); // 最短経路
                }

                // 状態不整合（最古未読ＩＤが実は既読）
                lock (_lockUnread)
                {
                    SetNextUnreadId(-1, tb);   // 頭から探索
                }

                return oldest == -1 ? -1 : tb.IndexOf(oldest);
            }

            // 一見未読なさそうだが、未読カウントはあるので探索
            if (!(tb.UnreadManage && Configs.Instance.UnreadManage))
            {
                return -1;
            }

            lock (_lockUnread)
            {
                SetNextUnreadId(-1, tb);
            }

            return oldest == -1 ? -1 : tb.IndexOf(oldest);
        }

        /// <summary>
        /// 戻り値は追加件数
        /// </summary>
        /// <returns>追加件数</returns>
        public int DistributePosts()
        {
            lock (_lockObj)
            {
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
                    // タブに仮振分
                    Distribute();
                }
                catch (KeyNotFoundException)
                {
                    // タブ変更により振分が失敗した場合
                }

                int retCnt = _addedIds.Count;
                _addCount += retCnt;
                _addedIds.Clear();
                _addedIds = null;                // 後始末
                return retCnt;                // 件数
            }
        }

        public int SubmitUpdate(ref string soundFile, ref PostClass[] notifyPosts, ref bool isMentionIncluded, ref bool isDeletePost, bool isUserStream)
        {
            // 注：メインスレッドから呼ぶこと
            lock (_lockObj)
            {
                if (_notifyPosts == null)
                {
                    soundFile = string.Empty;
                    notifyPosts = null;
                    return 0;
                }

                foreach (TabClass tb in Tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                    {
                        _addCount += tb.GetTemporaryCount();
                    }

                    tb.AddSubmit(ref isMentionIncluded); // 振分確定（各タブに反映）
                }

                if (!isUserStream || SortMode != IdComparerClass.ComparerMode.Id)
                {
                    SortPosts();
                }

                if (isUserStream)
                {
                    isDeletePost = _deletedIds.Count > 0;
                    foreach (long id in _deletedIds)
                    {
                        RemovePost(id);
                    }

                    _deletedIds.Clear();
                }

                soundFile = _soundFile;
                _soundFile = string.Empty;
                notifyPosts = _notifyPosts.ToArray();
                _notifyPosts.Clear();
                _notifyPosts = null;
                int retCnt = _addCount;
                _addCount = 0;
                return retCnt;                // 件数（EndUpdateの戻り値と同じ）
            }
        }

        public void AddPost(PostClass item)
        {
            lock (_lockObj)
            {
                // 公式検索、リスト、関連発言の場合
                if (!string.IsNullOrEmpty(item.RelTabName))
                {
                    if (!Tabs.ContainsKey(item.RelTabName))
                    {
                        return;
                    }

                    TabClass tb = Tabs[item.RelTabName];
                    if (tb == null)
                    {
                        return;
                    }

                    if (tb.Contains(item.StatusId))
                    {
                        return;
                    }

                    tb.AddPostToInnerStorage(item);
                    return;
                }

                if (item.IsDm)
                {
                    // DM
                    TabClass tb = GetTabByType(TabUsageType.DirectMessage);
                    if (tb.Contains(item.StatusId))
                    {
                        return;
                    }

                    tb.AddPostToInnerStorage(item);
                    return;
                }

                if (_statuses.ContainsKey(item.StatusId))
                {
                    if (item.IsFav)
                    {
                        if (item.IsRetweeted)
                        {
                            item.IsFav = false;
                        }
                        else
                        {
                            _statuses[item.StatusId].IsFav = true;
                        }
                    }
                    else
                    {
                        return; // 追加済みなら何もしない
                    }
                }
                else
                {
                    if (item.IsFav && item.IsRetweeted)
                    {
                        item.IsFav = false;
                    }

                    // 既に持っている公式RTは捨てる
                    if (Configs.Instance.HideDuplicatedRetweets
                        && !item.IsMe
                        && _retweets.ContainsKey(item.RetweetedId)
                        && _retweets[item.RetweetedId].RetweetedCount > 0)
                    {
                        return;
                    }

                    if (BlockIds.Contains(item.UserId))
                    {
                        return;
                    }

                    _statuses.Add(item.StatusId, item);
                }

                if (item.IsRetweeted)
                {
                    AddRetweet(item);
                }

                if (item.IsFav && _retweets.ContainsKey(item.StatusId))
                {
                    // Fav済みのRetweet元発言は追加しない
                    return;
                }

                if (_addedIds == null)
                {
                    _addedIds = new List<long>();
                }

                // タブ追加用IDコレクション準備
                _addedIds.Add(item.StatusId);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="read">true=既読へ　false=未読へ</param>
        /// <param name="tabName"></param>
        /// <param name="index"></param>
        public void SetReadAllTab(bool read, string tabName, int index)
        {
            TabClass tb = Tabs[tabName];
            if (!tb.UnreadManage)
            {
                // 未読管理していなければ終了
                return;
            }

            long id = tb.GetId(index);
            if (id < 0)
            {
                return;
            }

            PostClass post = tb.IsInnerStorageTabType ? tb.Posts[id] : _statuses[id];
            if (post.IsRead == read)
            {
                // 状態変更なければ終了
                return;
            }

            post.IsRead = read;
            lock (_lockUnread)
            {
                if (read)
                {
                    tb.UnreadCount--;
                    SetNextUnreadId(id, tb); // 次の未読セット

                    // 他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        // 一般タブ
                        if (_statuses.ContainsKey(id) && !_statuses[id].IsRead)
                        {
                            foreach (string key in Tabs.Keys)
                            {
                                if (Tabs[key].UnreadManage && Tabs[key].Contains(id) && !Tabs[key].IsInnerStorageTabType)
                                {
                                    Tabs[key].UnreadCount--;
                                    if (Tabs[key].OldestUnreadId == id)
                                    {
                                        Tabs[key].OldestUnreadId = -1;
                                    }
                                }
                            }

                            _statuses[id].IsRead = true;
                        }
                    }
                    else
                    {
                        // 一般タブ
                        foreach (string key in Tabs.Keys)
                        {
                            if (key != tabName)
                            {
                                if (Tabs[key].UnreadManage && Tabs[key].Contains(id) && !Tabs[key].IsInnerStorageTabType)
                                {
                                    Tabs[key].UnreadCount--;
                                    if (Tabs[key].OldestUnreadId == id)
                                    {
                                        Tabs[key].OldestUnreadId = -1;
                                    }
                                }
                            }
                        }
                    }

                    // 内部保存タブ
                    foreach (string key in Tabs.Keys)
                    {
                        if (key != tabName)
                        {
                            if (Tabs[key].Contains(id) && Tabs[key].IsInnerStorageTabType && !Tabs[key].Posts[id].IsRead)
                            {
                                if (Tabs[key].UnreadManage)
                                {
                                    Tabs[key].UnreadCount--;
                                    if (Tabs[key].OldestUnreadId == id)
                                    {
                                        Tabs[key].OldestUnreadId = -1;
                                    }
                                }

                                Tabs[key].Posts[id].IsRead = true;
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
                        // 一般タブ
                        if (_statuses.ContainsKey(id) && _statuses[id].IsRead)
                        {
                            foreach (string key in Tabs.Keys)
                            {
                                if (Tabs[key].UnreadManage && Tabs[key].Contains(id) && !Tabs[key].IsInnerStorageTabType)
                                {
                                    Tabs[key].UnreadCount += 1;
                                    if (Tabs[key].OldestUnreadId > id)
                                    {
                                        Tabs[key].OldestUnreadId = id;
                                    }
                                }
                            }

                            _statuses[id].IsRead = false;
                        }
                    }
                    else
                    {
                        // 一般タブ
                        foreach (string key in Tabs.Keys)
                        {
                            if (key != tabName && Tabs[key].UnreadManage && Tabs[key].Contains(id) && !Tabs[key].IsInnerStorageTabType)
                            {
                                Tabs[key].UnreadCount += 1;
                                if (Tabs[key].OldestUnreadId > id)
                                {
                                    Tabs[key].OldestUnreadId = id;
                                }
                            }
                        }
                    }

                    // 内部保存タブ
                    foreach (string key in Tabs.Keys)
                    {
                        if (key != tabName && Tabs[key].Contains(id) && Tabs[key].IsInnerStorageTabType && Tabs[key].Posts[id].IsRead)
                        {
                            if (Tabs[key].UnreadManage)
                            {
                                Tabs[key].UnreadCount += 1;
                                if (Tabs[key].OldestUnreadId > id)
                                {
                                    Tabs[key].OldestUnreadId = id;
                                }
                            }

                            Tabs[key].Posts[id].IsRead = false;
                        }
                    }
                }
            }
        }

        // / TODO: パフォーマンスを勘案して、戻すか決める
        public void SetRead(bool read, string tabName, int index)
        {
            // Read_:True=既読へ　False=未読へ
            TabClass tb = Tabs[tabName];
            if (!tb.UnreadManage)
            {
                // 未読管理していなければ終了
                return;
            }

            long id = tb.GetId(index);
            if (id < 0)
            {
                return;
            }

            PostClass post = tb.IsInnerStorageTabType ? tb.Posts[id] : _statuses[id];
            if (post.IsRead == read)
            {
                // 状態変更なければ終了
                return;
            }

            post.IsRead = read;            // 指定の状態に変更
            lock (_lockUnread)
            {
                var noinnerUnreadManageTabKeys = Tabs.Keys.Where(k => k != tabName && Tabs[k].UnreadManage && !Tabs[k].IsInnerStorageTabType);
                if (read)
                {
                    tb.UnreadCount -= 1;
                    SetNextUnreadId(id, tb); // 次の未読セット

                    // 他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        return;
                    }

                    foreach (string key in noinnerUnreadManageTabKeys)
                    {
                        if (Tabs[key].Contains(id))
                        {
                            Tabs[key].UnreadCount--;
                            if (Tabs[key].OldestUnreadId == id)
                            {
                                Tabs[key].OldestUnreadId = -1;
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

                    foreach (string key in noinnerUnreadManageTabKeys)
                    {
                        if (Tabs[key].Contains(id))
                        {
                            Tabs[key].UnreadCount++;
                            if (Tabs[key].OldestUnreadId > id)
                            {
                                Tabs[key].OldestUnreadId = id;
                            }
                        }
                    }
                }
            }
        }

        public void SetRead()
        {
            TabClass tb = GetTabByType(TabUsageType.Home);
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
                        SetNextUnreadId(id, tb);

                        // 次の未読セット
                        foreach (string key in Tabs.Keys)
                        {
                            if (Tabs[key].UnreadManage && Tabs[key].Contains(id))
                            {
                                Tabs[key].UnreadCount -= 1;
                                if (Tabs[key].OldestUnreadId == id)
                                {
                                    Tabs[key].OldestUnreadId = -1;
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

            foreach (TabClass tb in GetTabsInnerStorageType())
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
            TabClass tab;
            if (!Tabs.TryGetValue(tabName, out tab))
            {
                throw new ArgumentException(string.Format("TabName={0} is not contained.", tabName));
            }

            long id = tab.GetId(index);
            if (id < 0)
            {
                throw new ArgumentException(string.Format("Index can't find. Index={0}/TabName={1}", index, tabName));
            }

            try
            {
                return tab.IsInnerStorageTabType ? tab.Posts[id] : _statuses[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Index={0}/TabName={1}", index, tabName), ex);
            }
        }

        public PostClass[] Item(string tabName, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex + 1;
            var posts = new PostClass[length];
            var tab = Tabs[tabName];
            var postss = tab.IsInnerStorageTabType ? tab.Posts : _statuses;
            for (var i = 0; i < length; ++i)
            {
                posts[i] = postss[tab.GetId(startIndex + i)];
            }

            return posts;
        }

        public bool ContainsKey(long id)
        {
            // DM,公式検索は非対応
            lock (_lockObj)
            {
                return _statuses.ContainsKey(id);
            }
        }

        public bool ContainsKey(long id, string tabName)
        {
            // DM,公式検索は対応版
            lock (_lockObj)
            {
                return Tabs.ContainsKey(tabName) && Tabs[tabName].Contains(id);
            }
        }

        public void SetUnreadManage(bool manage)
        {
            var tabs = Tabs.Where(t => t.Value.UnreadManage);
            if (manage)
            {
                foreach (var tab in tabs)
                {
                    var tb = tab.Value;
                    lock (_lockUnread)
                    {
                        int cnt = 0;
                        long oldest = long.MaxValue;
                        var posts = tb.IsInnerStorageTabType ? tb.Posts : _statuses;
                        foreach (long id in tb.BackupIds())
                        {
                            if (!posts[id].IsRead)
                            {
                                cnt++;
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
            else
            {
                foreach (var tab in tabs)
                {
                    var tb = tab.Value;
                    if (tb.UnreadCount > 0)
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
            TabClass tb = Tabs[original];
            Tabs.Remove(original);
            tb.TabName = newName;
            Tabs.Add(newName, tb);
        }

        public void FilterAll()
        {
            lock (_lockObj)
            {
                var homeTab = GetTabByType(TabUsageType.Home);
                var replyTab = GetTabByType(TabUsageType.Mentions);
                foreach (var tb in Tabs.Values.ToArray())
                {
                    if (!tb.FilterModified)
                    {
                        continue;
                    }

                    tb.FilterModified = false;
                    var orgIds = tb.BackupIds();
                    tb.ClearIDs();

                    // フィルター前のIDsを退避。どのタブにも含まれないidはrecentへ追加
                    // moveフィルターにヒットした際、recentに該当あればrecentから削除
                    foreach (long id in _statuses.Keys)
                    {
                        PostClass post = _statuses[id];
                        if (post.IsDm)
                        {
                            continue;
                        }

                        HITRESULT rslt = tb.AddFiltered(post);
                        switch (rslt)
                        {
                            case HITRESULT.CopyAndMark:
                                post.IsMark = true;             // マークあり
                                post.FilterHit = true;
                                break;
                            case HITRESULT.Move:
                                homeTab.Remove(post.StatusId, post.IsRead);
                                post.IsMark = false;
                                post.FilterHit = true;
                                break;
                            case HITRESULT.Copy:
                                post.IsMark = false;
                                post.FilterHit = true;
                                break;
                            case HITRESULT.Exclude:
                                if (tb.TabName == replyTab.TabName && post.IsReply)
                                {
                                    post.IsExcludeReply = true;
                                }

                                if (post.IsFav)
                                {
                                    GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                }

                                post.FilterHit = false;
                                break;
                            case HITRESULT.None:
                                if (tb.TabName == replyTab.TabName && post.IsReply)
                                {
                                    replyTab.Add(post.StatusId, post.IsRead, true);
                                }

                                if (post.IsFav)
                                {
                                    GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                }

                                post.FilterHit = false;
                                break;
                        }
                    }

                    tb.AddSubmit();

                    // 振分確定
                    foreach (long id in orgIds)
                    {
                        bool hit = false;
                        foreach (var tmp in Tabs.Values.ToArray())
                        {
                            if (tmp.Contains(id))
                            {
                                hit = true;
                                break;
                            }
                        }

                        if (!hit)
                        {
                            homeTab.Add(id, _statuses[id].IsRead, false);
                        }
                    }
                }

                SortPosts();
            }
        }

        public long[] GetId(string tabName, IEnumerable<int> indecies)
        {
            if (indecies.Count() == 0)
            {
                return null;
            }

            return indecies.Select(i => Tabs[tabName].GetId(i)).ToArray();
        }

        public long GetId(string tabName, int index)
        {
            return Tabs[tabName].GetId(index);
        }

        public int[] IndexOf(string tabName, long[] ids)
        {
            if (ids == null)
            {
                return null;
            }

            int[] idx = new int[ids.Length];
            TabClass tb = Tabs[tabName];
            for (int i = 0; i < ids.Length; i++)
            {
                idx[i] = tb.IndexOf(ids[i]);
            }

            return idx;
        }

        public int IndexOf(string tabName, long id)
        {
            return Tabs[tabName].IndexOf(id);
        }

        public void ClearTabIds(string tabName)
        {
            // 不要なPostを削除
            lock (_lockObj)
            {
                if (!Tabs[tabName].IsInnerStorageTabType)
                {
                    foreach (long id in Tabs[tabName].BackupIds())
                    {
                        bool hit = false;
                        foreach (TabClass tb in Tabs.Values)
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

                // 指定タブをクリア
                Tabs[tabName].ClearIDs();
            }
        }

        public void SetTabUnreadManage(string tabName, bool manage)
        {
            TabClass tb = Tabs[tabName];
            lock (_lockUnread)
            {
                if (manage)
                {
                    int cnt = 0;
                    long oldest = long.MaxValue;
                    var posts = tb.IsInnerStorageTabType ? tb.Posts : _statuses;
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
                        post.IsOwl = post.IsMe ? false : !follower.Contains(post.UserId);
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

        public TabClass GetTabByType(TabUsageType tabType)
        {
            // Home,Mentions,DM,Favは1つに制限する
            // その他のタイプを指定されたら、最初に合致したものを返す
            // 合致しなければNothingを返す
            lock (_lockObj)
            {
                foreach (TabClass tb in Tabs.Values)
                {
                    if (tb != null && tb.TabType == tabType)
                    {
                        return tb;
                    }
                }

                return null;
            }
        }

        public List<TabClass> GetTabsByType(TabUsageType tabType)
        {
            // 合致したタブをListで返す
            // 合致しなければ空のListを返す
            lock (_lockObj)
            {
                return Tabs.Values.Where(tb => (tabType & tb.TabType) == tb.TabType).ToList();
            }
        }

        public List<TabClass> GetTabsInnerStorageType()
        {
            lock (_lockObj)
            {
                return Tabs.Values.Where(tb => tb.IsInnerStorageTabType).ToList();
            }
        }

        public TabClass GetTabByName(string tabName)
        {
            lock (_lockObj)
            {
                if (Tabs.ContainsKey(tabName))
                {
                    return Tabs[tabName];
                }

                return null;
            }
        }

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            return tabName != null && Tabs.ContainsKey(tabName)
                && (Tabs[tabName].TabType == TabUsageType.Home
                || Tabs[tabName].TabType == TabUsageType.Mentions
                || Tabs[tabName].TabType == TabUsageType.DirectMessage
                || Tabs[tabName].TabType == TabUsageType.Favorites);
        }

        // 振り分け可能タブの判定処理
        public bool IsDistributableTab(string tabName)
        {
            return tabName != null && Tabs.ContainsKey(tabName)
                && (Tabs[tabName].TabType == TabUsageType.Mentions
                || Tabs[tabName].TabType == TabUsageType.UserDefined);
        }

        public string GetUniqueTabName()
        {
            string tabNameTemp = string.Format("MyTab{0}", (Tabs.Count + 1));
            for (int i = 2; i <= 100; i++)
            {
                if (Tabs.ContainsKey(tabNameTemp))
                {
                    tabNameTemp = string.Format("MyTab{0}", (Tabs.Count + i));
                }
                else
                {
                    break;
                }
            }

            return tabNameTemp;
        }

        public int GetAllUnreadCount()
        {
            return Tabs.Values.Select(tc => tc.UnreadCount).Sum();
        }

        public int GetAllCount()
        {
            return Tabs.Values.Select(tc => tc.AllCount).Sum();
        }

        private void ScrubGeo(long userId, long uptoStatusId)
        {
            lock (_lockObj)
            {
                var userPosts = from post in _statuses.Values
                                where post.UserId == userId && post.UserId <= uptoStatusId
                                select post;

                foreach (var p in userPosts)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }

                var userPosts2 = from tb in GetTabsInnerStorageType()
                                 from post in tb.Posts.Values
                                 where post.UserId == userId && post.UserId <= uptoStatusId
                                 select post;

                foreach (var p in userPosts2)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }
            }
        }

        private void DeletePost(long id)
        {
            lock (_lockObj)
            {
                if (_statuses.ContainsKey(id))
                {
                    _statuses[id].IsDeleted = true;
                }

                foreach (TabClass tb in GetTabsInnerStorageType())
                {
                    if (tb.Contains(id))
                    {
                        tb.Posts[id].IsDeleted = true;
                    }
                }
            }
        }

        private void SetNextUnreadId(long currentId, TabClass tab)
        {
            // CurrentID:今既読にしたID(OldestIDの可能性あり)
            // 最古未読が設定されていて、既読の場合（1発言以上存在）
            try
            {
                // 次の未読探索
                var posts = tab.IsInnerStorageTabType ? tab.Posts : _statuses;
                if (tab.OldestUnreadId > -1 && posts.ContainsKey(tab.OldestUnreadId)
                    && posts[tab.OldestUnreadId].IsRead && _sorter.Mode == IdComparerClass.ComparerMode.Id)
                {
                    if (tab.UnreadCount == 0)
                    {
                        // 未読数０→最古未読なし
                        tab.OldestUnreadId = -1;
                    }
                    else if (tab.OldestUnreadId == currentId && currentId > -1)
                    {
                        // 最古IDを既読にしたタイミング→次のIDから続けて探索
                        int idx = tab.IndexOf(currentId);
                        if (idx > -1)
                        {
                            // 続きから探索
                            FindUnreadId(idx, tab);
                        }
                        else
                        {
                            // 頭から探索
                            FindUnreadId(-1, tab);
                        }
                    }
                    else
                    {
                        // 頭から探索
                        FindUnreadId(-1, tab);
                    }
                }
                else
                {
                    // 頭から探索
                    FindUnreadId(-1, tab);
                }
            }
            catch (KeyNotFoundException)
            {
                // 頭から探索
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
            if (_sorter.Order == SortOrder.Ascending)
            {
                if (startIdx == -1)
                {
                    startIdx = 0;
                }
                else
                {
                    if (startIdx > tab.AllCount - 1)
                    {
                        startIdx = tab.AllCount - 1; // 念のため
                    }
                }

                toIdx = tab.AllCount - 1;
                if (toIdx < 0)
                {
                    toIdx = 0; // 念のため
                }

                stp = 1;
            }
            else
            {
                if (startIdx == -1)
                {
                    startIdx = tab.AllCount - 1;
                }

                if (startIdx < 0)
                {
                    startIdx = 0;
                }

                // 念のため
                toIdx = 0;
                stp = -1;
            }

            var posts = tab.IsInnerStorageTabType ? tab.Posts : _statuses;
            for (int i = startIdx; i <= toIdx; i += stp)
            {
                long id = tab.GetId(i);
                if (id > -1 && !posts[id].IsRead)
                {
                    tab.OldestUnreadId = id;
                    break;
                }
            }
        }

        private void Distribute()
        {
            // 各タブのフィルターと照合。合致したらタブにID追加
            // 通知メッセージ用に、表示必要な発言リストと再生サウンドを返す
            // notifyPosts = New List(Of PostClass)
            TabClass homeTab = GetTabByType(TabUsageType.Home);
            TabClass replyTab = GetTabByType(TabUsageType.Mentions);
            TabClass dmsgTab = GetTabByType(TabUsageType.DirectMessage);
            TabClass favTab = GetTabByType(TabUsageType.Favorites);
            foreach (long id in _addedIds)
            {
                PostClass post = _statuses[id];
                bool added = false;              // 通知リスト追加フラグ
                bool mv = false;                // 移動フラグ（Recent追加有無）
                HITRESULT rslt = HITRESULT.None;
                post.IsExcludeReply = false;
                foreach (string tn in Tabs.Keys)
                {
                    rslt = Tabs[tn].AddFiltered(post);
                    if (rslt != HITRESULT.None && rslt != HITRESULT.Exclude)
                    {
                        if (rslt == HITRESULT.CopyAndMark)
                        {
                            post.IsMark = true;
                        }

                        // マークあり
                        if (rslt == HITRESULT.Move)
                        {
                            mv = true; // 移動
                            post.IsMark = false;
                        }

                        if (Tabs[tn].Notify)
                        {
                            added = true;
                        }

                        // 通知あり
                        if (!string.IsNullOrEmpty(Tabs[tn].SoundFile) && string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = Tabs[tn].SoundFile;                            // wavファイル（未設定の場合のみ）
                        }

                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == HITRESULT.Exclude && Tabs[tn].TabType == TabUsageType.Mentions)
                        {
                            post.IsExcludeReply = true;
                        }

                        post.FilterHit = false;
                    }
                }

                // 移動されなかったらRecentに追加
                if (!mv)
                {
                    homeTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(homeTab.SoundFile) && string.IsNullOrEmpty(_soundFile))
                    {
                        _soundFile = homeTab.SoundFile;
                    }

                    if (homeTab.Notify)
                    {
                        added = true;
                    }
                }

                // 除外ルール適用のないReplyならReplyタブに追加
                if (post.IsReply && !post.IsExcludeReply)
                {
                    replyTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(replyTab.SoundFile))
                    {
                        _soundFile = replyTab.SoundFile;
                    }

                    if (replyTab.Notify)
                    {
                        added = true;
                    }
                }

                // Fav済み発言だったらFavoritesタブに追加
                if (post.IsFav)
                {
                    if (favTab.Contains(post.StatusId))
                    {
                        // 取得済みなら非通知
                        // _soundFile = ""
                        added = false;
                    }
                    else
                    {
                        favTab.Add(post.StatusId, post.IsRead, true);
                        if (!string.IsNullOrEmpty(favTab.SoundFile) && string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = favTab.SoundFile;
                        }

                        if (favTab.Notify)
                        {
                            added = true;
                        }
                    }
                }

                if (added)
                {
                    _notifyPosts.Add(post);
                }
            }

            foreach (TabClass tb in Tabs.Values)
            {
                if (tb.IsInnerStorageTabType && tb.Notify && tb.GetTemporaryCount() > 0)
                {
                    foreach (var post in tb.GetTemporaryPosts())
                    {
                        bool exist = false;
                        foreach (PostClass npost in _notifyPosts)
                        {
                            if (npost.StatusId == post.StatusId)
                            {
                                exist = true;
                                break;
                            }
                        }

                        if (!exist)
                        {
                            _notifyPosts.Add(post);
                        }
                    }

                    if (!string.IsNullOrEmpty(tb.SoundFile))
                    {
                        if (tb.TabType == TabUsageType.DirectMessage || string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = tb.SoundFile;
                        }
                    }
                }
            }
        }

        private void AddRetweet(PostClass item)
        {
            // True:追加、False:保持済み
            long retweetedId = item.RetweetedId;
            if (_retweets.ContainsKey(retweetedId))
            {
                _retweets[retweetedId].RetweetedCount += 1;
                if (_retweets[retweetedId].RetweetedCount > 10)
                {
                    _retweets[retweetedId].RetweetedCount = 0;
                }

                return;
            }

            var retweetPost = new PostClass
            {
                Nickname = item.Nickname,
                TextFromApi = item.TextFromApi,
                Text = item.Text,
                ImageUrl = item.ImageUrl,
                ScreenName = item.ScreenName,
                CreatedAt = item.CreatedAt,
                StatusId = retweetedId,
                IsFav = item.IsFav,
                IsRead = item.IsRead,
                IsReply = item.IsReply,
                IsExcludeReply = item.IsExcludeReply,
                IsProtect = item.IsProtect,
                IsOwl = item.IsOwl,
                IsMark = item.IsMark,
                InReplyToUser = item.InReplyToUser,
                InReplyToStatusId = item.InReplyToStatusId,
                Source = item.Source,
                SourceHtml = item.SourceHtml,
                ReplyToList = item.ReplyToList,
                IsMe = item.IsMe,
                IsDm = item.IsDm,
                UserId = item.UserId,
                FilterHit = item.FilterHit,
                RetweetedBy = string.Empty,
                RetweetedId = 0,
                PostGeo = item.PostGeo
            };
            _retweets.Add(retweetedId, retweetPost);
            _retweets[retweetedId].RetweetedCount += 1;
        }
    }
}