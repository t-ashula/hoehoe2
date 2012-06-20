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
        private static TabInformations instance = new TabInformations();

        private readonly object lockObj;
        private readonly object lockUnread;

        private IdComparerClass sorter;
        private Dictionary<long, PostClass> statuses;
        private List<long> addedIds;
        private List<long> deletedIds;
        private Dictionary<long, PostClass> retweets;
        private Stack<TabClass> removedTab;
        private int addCount;
        private string soundFile;
        private List<PostClass> notifyPosts;
        private List<ListElement> lists;

        private TabInformations()
        {
            this.BlockIds = new List<long>();
            this.lockUnread = new object();
            this.lockObj = new object();
            this.sorter = new IdComparerClass();
            this.Tabs = new Dictionary<string, TabClass>();
            this.statuses = new Dictionary<long, PostClass>();
            this.deletedIds = new List<long>();
            this.retweets = new Dictionary<long, PostClass>();
            this.removedTab = new Stack<TabClass>();
            this.lists = new List<ListElement>();
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
                return this.lists;
            }

            set
            {
                if (value != null && value.Count > 0)
                {
                    foreach (TabClass tb in this.GetTabsByType(TabUsageType.Lists))
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

                this.lists = value;
            }
        }

        public Stack<TabClass> RemovedTab
        {
            get { return this.removedTab; }
        }

        public Dictionary<string, TabClass> Tabs { get; set; }

        public Dictionary<string, TabClass>.KeyCollection KeysTab
        {
            get { return this.Tabs.Keys; }
        }

        public SortOrder SortOrder
        {
            get
            {
                return this.sorter.Order;
            }

            set
            {
                this.sorter.Order = value;
                foreach (string key in this.Tabs.Keys)
                {
                    this.Tabs[key].Sorter.Order = value;
                }
            }
        }

        public IdComparerClass.ComparerMode SortMode
        {
            get
            {
                return this.sorter.Mode;
            }

            set
            {
                this.sorter.Mode = value;
                foreach (string key in this.Tabs.Keys)
                {
                    this.Tabs[key].Sorter.Mode = value;
                }
            }
        }

        public Dictionary<long, PostClass> Posts
        {
            get { return this.statuses; }
        }
        
        public bool AddTab(string tabName, TabUsageType tabType, ListElement list)
        {
            if (this.Tabs.ContainsKey(tabName))
            {
                return false;
            }

            this.Tabs.Add(tabName, new TabClass(tabName, tabType, list));
            this.Tabs[tabName].Sorter.Mode = this.sorter.Mode;
            this.Tabs[tabName].Sorter.Order = this.sorter.Order;
            return true;
        }

        public void RemoveTab(string tabName)
        {
            lock (this.lockObj)
            {
                if (this.IsDefaultTab(tabName))
                {
                    return; // 念のため
                }

                var removeTab = this.Tabs[tabName];
                if (!removeTab.IsInnerStorageTabType)
                {
                    // 削除されるタブに有った tweet が他のタブになければ hometab に書き戻し
                    var homeTab = this.GetTabByType(TabUsageType.Home);
                    var dmessageTabName = this.GetTabByType(TabUsageType.DirectMessage).TabName;
                    for (var idx = 0; idx < removeTab.AllCount; ++idx)
                    {
                        var id = removeTab.GetId(idx);
                        if (id < 0)
                        {
                            continue;
                        }

                        if (!this.Tabs.Keys.Where(t => t != tabName && t != dmessageTabName).Any(t => this.Tabs[t].Contains(id)))
                        {
                            homeTab.Add(id, this.statuses[id].IsRead, false);
                        }
                    }
                }

                this.removedTab.Push(removeTab);
                this.Tabs.Remove(tabName);
            }
        }

        public bool ContainsTab(string tabText)
        {
            return this.Tabs.ContainsKey(tabText);
        }

        public bool ContainsTab(TabClass ts)
        {
            return this.Tabs.ContainsValue(ts);
        }

        public void SortPosts()
        {
            foreach (string key in this.Tabs.Keys)
            {
                this.Tabs[key].Sort();
            }
        }

        public SortOrder ToggleSortOrder(IdComparerClass.ComparerMode sortMode)
        {
            if (this.sorter.Mode == sortMode)
            {
                this.sorter.Order = this.sorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
                foreach (string key in this.Tabs.Keys)
                {
                    this.Tabs[key].Sorter.Order = this.sorter.Order;
                }
            }
            else
            {
                this.sorter.Mode = sortMode;
                this.sorter.Order = SortOrder.Ascending;
                foreach (string key in this.Tabs.Keys)
                {
                    this.Tabs[key].Sorter.Mode = sortMode;
                    this.Tabs[key].Sorter.Order = SortOrder.Ascending;
                }
            }

            this.SortPosts();
            return this.sorter.Order;
        }

        public PostClass RetweetSource(long id)
        {
            return this.retweets.ContainsKey(id) ? this.retweets[id] : null;
        }

        /// <summary>
        /// 指定タブから該当ID削除
        /// </summary>
        /// <param name="id"></param>
        public void RemoveFavPost(long id)
        {
            lock (this.lockObj)
            {
                if (!this.statuses.ContainsKey(id))
                {
                    return;
                }

                PostClass post = this.statuses[id];
                TabClass tab = this.GetTabByType(TabUsageType.Favorites);
                string tn = tab.TabName;
                TabUsageType tabUsage = tab.TabType;
                if (tab.Contains(id))
                {
                    // 未読管理
                    if (tab.UnreadManage && !post.IsRead)
                    {
                        lock (this.lockUnread)
                        {
                            tab.UnreadCount--;
                            this.SetNextUnreadId(id, tab);
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
                            toRemovePost = this.Item(tn, i);
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
                                lock (this.lockUnread)
                                {
                                    tab.UnreadCount--;
                                    this.SetNextUnreadId(toRemovePost.StatusId, tab);
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
            lock (this.lockObj)
            {
                this.ScrubGeo(id, uptoId);
            }
        }

        public void RemovePostReserve(long id)
        {
            lock (this.lockObj)
            {
                this.deletedIds.Add(id);
                this.DeletePost(id);                // UI選択行がずれるため、RemovePostは使用しない
            }
        }

        /// <summary>
        /// 各タブから該当ID削除
        /// </summary>
        /// <param name="id"></param>
        public void RemovePost(long id)
        {
            lock (this.lockObj)
            {
                foreach (string key in this.Tabs.Keys)
                {
                    TabClass tab = this.Tabs[key];
                    if (!tab.Contains(id))
                    {
                        continue;
                    }

                    // 未読管理 未読数がずれる可能性があるためtab.Postsの未読も確認する
                    if (tab.UnreadManage)
                    {                        
                        bool changeUnread = tab.IsInnerStorageTabType ? !tab.Posts[id].IsRead : !this.statuses[id].IsRead;
                        if (changeUnread)
                        {
                            lock (this.lockUnread)
                            {
                                tab.UnreadCount--;
                                this.SetNextUnreadId(id, tab);
                            }
                        }
                    }

                    tab.Remove(id);
                }

                if (this.statuses.ContainsKey(id))
                {
                    this.statuses.Remove(id);
                }
            }
        }

        public int GetOldestUnreadIndex(string tabName)
        {
            TabClass tb = this.Tabs[tabName];
            var oldest = tb.OldestUnreadId;
            if (oldest > -1 && tb.Contains(oldest) && tb.UnreadCount > 0)
            {
                // 未読アイテムへ
                bool isRead = tb.IsInnerStorageTabType ? tb.Posts[oldest].IsRead : this.statuses[oldest].IsRead;
                if (!isRead)
                {
                    return tb.IndexOf(oldest); // 最短経路
                }

                // 状態不整合（最古未読ＩＤが実は既読）
                lock (this.lockUnread)
                {
                    this.SetNextUnreadId(-1, tb);   // 頭から探索
                }

                return oldest == -1 ? -1 : tb.IndexOf(oldest);
            }

            // 一見未読なさそうだが、未読カウントはあるので探索
            if (!(tb.UnreadManage && Configs.Instance.UnreadManage))
            {
                return -1;
            }

            lock (this.lockUnread)
            {
                this.SetNextUnreadId(-1, tb);
            }

            return oldest == -1 ? -1 : tb.IndexOf(oldest);
        }

        /// <summary>
        /// 戻り値は追加件数
        /// </summary>
        /// <returns>追加件数</returns>
        public int DistributePosts()
        {
            lock (this.lockObj)
            {
                if (this.addedIds == null)
                {
                    this.addedIds = new List<long>();
                }

                if (this.notifyPosts == null)
                {
                    this.notifyPosts = new List<PostClass>();
                }

                try
                {
                    // タブに仮振分
                    this.Distribute();
                }
                catch (KeyNotFoundException)
                {
                    // タブ変更により振分が失敗した場合
                }

                int retCnt = this.addedIds.Count;
                this.addCount += retCnt;
                this.addedIds.Clear();
                this.addedIds = null;                // 後始末
                return retCnt;                // 件数
            }
        }

        public int SubmitUpdate(ref string soundFile, ref PostClass[] notifyPosts, ref bool isMentionIncluded, ref bool isDeletePost, bool isUserStream)
        {
            // 注：メインスレッドから呼ぶこと
            lock (this.lockObj)
            {
                if (this.notifyPosts == null)
                {
                    soundFile = string.Empty;
                    notifyPosts = null;
                    return 0;
                }

                foreach (TabClass tb in this.Tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                    {
                        this.addCount += tb.GetTemporaryCount();
                    }

                    tb.AddSubmit(ref isMentionIncluded); // 振分確定（各タブに反映）
                }

                if (!isUserStream || this.SortMode != IdComparerClass.ComparerMode.Id)
                {
                    this.SortPosts();
                }

                if (isUserStream)
                {
                    isDeletePost = this.deletedIds.Count > 0;
                    foreach (long id in this.deletedIds)
                    {
                        this.RemovePost(id);
                    }

                    this.deletedIds.Clear();
                }

                soundFile = this.soundFile;
                this.soundFile = string.Empty;
                notifyPosts = this.notifyPosts.ToArray();
                this.notifyPosts.Clear();
                this.notifyPosts = null;
                int retCnt = this.addCount;
                this.addCount = 0;
                return retCnt;                // 件数（EndUpdateの戻り値と同じ）
            }
        }

        public void AddPost(PostClass item)
        {
            lock (this.lockObj)
            {
                // 公式検索、リスト、関連発言の場合
                if (!string.IsNullOrEmpty(item.RelTabName))
                {
                    if (!this.Tabs.ContainsKey(item.RelTabName))
                    {
                        return;
                    }

                    TabClass tb = this.Tabs[item.RelTabName];
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
                    TabClass tb = this.GetTabByType(TabUsageType.DirectMessage);
                    if (tb.Contains(item.StatusId))
                    {
                        return;
                    }

                    tb.AddPostToInnerStorage(item);
                    return;
                }
                
                if (this.statuses.ContainsKey(item.StatusId))
                {
                    if (item.IsFav)
                    {
                        if (item.IsRetweeted)
                        {
                            item.IsFav = false;
                        }
                        else
                        {
                            this.statuses[item.StatusId].IsFav = true;
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
                        && this.retweets.ContainsKey(item.RetweetedId)
                        && this.retweets[item.RetweetedId].RetweetedCount > 0)
                    {
                        return;
                    }
                    
                    if (this.BlockIds.Contains(item.UserId))
                    {
                        return;
                    }
                    
                    this.statuses.Add(item.StatusId, item);
                }

                if (item.IsRetweeted)
                {
                    this.AddRetweet(item);
                }
                
                if (item.IsFav && this.retweets.ContainsKey(item.StatusId))
                {
                    // Fav済みのRetweet元発言は追加しない
                    return;
                }
                
                if (this.addedIds == null)
                {
                    this.addedIds = new List<long>();
                }
                
                // タブ追加用IDコレクション準備
                this.addedIds.Add(item.StatusId);
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
            TabClass tb = this.Tabs[tabName];

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

            PostClass post = tb.IsInnerStorageTabType ? tb.Posts[id] : this.statuses[id];

            if (post.IsRead == read)
            {
                // 状態変更なければ終了
                return;
            }

            post.IsRead = read;
            lock (this.lockUnread)
            {
                if (read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(id, tb); // 次の未読セット

                    // 他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        // 一般タブ
                        if (this.statuses.ContainsKey(id) && !this.statuses[id].IsRead)
                        {
                            foreach (string key in this.Tabs.Keys)
                            {
                                if (this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                                {
                                    this.Tabs[key].UnreadCount -= 1;
                                    if (this.Tabs[key].OldestUnreadId == id)
                                    {
                                        this.Tabs[key].OldestUnreadId = -1;
                                    }
                                }
                            }

                            this.statuses[id].IsRead = true;
                        }
                    }
                    else
                    {
                        // 一般タブ
                        foreach (string key in this.Tabs.Keys)
                        {
                            if (key != tabName && this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                            {
                                this.Tabs[key].UnreadCount -= 1;
                                if (this.Tabs[key].OldestUnreadId == id)
                                {
                                    this.Tabs[key].OldestUnreadId = -1;
                                }
                            }
                        }
                    }

                    // 内部保存タブ
                    foreach (string key in this.Tabs.Keys)
                    {
                        if (key != tabName && this.Tabs[key].Contains(id) && this.Tabs[key].IsInnerStorageTabType && !this.Tabs[key].Posts[id].IsRead)
                        {
                            if (this.Tabs[key].UnreadManage)
                            {
                                this.Tabs[key].UnreadCount -= 1;
                                if (this.Tabs[key].OldestUnreadId == id)
                                {
                                    this.Tabs[key].OldestUnreadId = -1;
                                }
                            }

                            this.Tabs[key].Posts[id].IsRead = true;
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
                        if (this.statuses.ContainsKey(id) && this.statuses[id].IsRead)
                        {
                            foreach (string key in this.Tabs.Keys)
                            {
                                if (this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                                {
                                    this.Tabs[key].UnreadCount += 1;
                                    if (this.Tabs[key].OldestUnreadId > id)
                                    {
                                        this.Tabs[key].OldestUnreadId = id;
                                    }
                                }
                            }

                            this.statuses[id].IsRead = false;
                        }
                    }
                    else
                    {
                        // 一般タブ
                        foreach (string key in this.Tabs.Keys)
                        {
                            if (key != tabName && this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                            {
                                this.Tabs[key].UnreadCount += 1;
                                if (this.Tabs[key].OldestUnreadId > id)
                                {
                                    this.Tabs[key].OldestUnreadId = id;
                                }
                            }
                        }
                    }

                    // 内部保存タブ
                    foreach (string key in this.Tabs.Keys)
                    {
                        if (key != tabName && this.Tabs[key].Contains(id) && this.Tabs[key].IsInnerStorageTabType && this.Tabs[key].Posts[id].IsRead)
                        {
                            if (this.Tabs[key].UnreadManage)
                            {
                                this.Tabs[key].UnreadCount += 1;
                                if (this.Tabs[key].OldestUnreadId > id)
                                {
                                    this.Tabs[key].OldestUnreadId = id;
                                }
                            }

                            this.Tabs[key].Posts[id].IsRead = false;
                        }
                    }
                }
            }
        }

        // / TODO: パフォーマンスを勘案して、戻すか決める
        public void SetRead(bool read, string tabName, int index)
        {
            // Read_:True=既読へ　False=未読へ
            TabClass tb = this.Tabs[tabName];

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

            PostClass post = tb.IsInnerStorageTabType ? tb.Posts[id] : this.statuses[id];

            if (post.IsRead == read)
            {
                // 状態変更なければ終了
                return;
            }

            post.IsRead = read;            // 指定の状態に変更
            lock (this.lockUnread)
            {
                if (read)
                {
                    tb.UnreadCount -= 1;
                    this.SetNextUnreadId(id, tb); // 次の未読セット

                    // 他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        return;
                    }

                    foreach (string key in this.Tabs.Keys)
                    {
                        if (key != tabName && this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                        {
                            this.Tabs[key].UnreadCount -= 1;
                            if (this.Tabs[key].OldestUnreadId == id)
                            {
                                this.Tabs[key].OldestUnreadId = -1;
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

                    foreach (string key in this.Tabs.Keys)
                    {
                        if (!(key == tabName) && this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id) && !this.Tabs[key].IsInnerStorageTabType)
                        {
                            this.Tabs[key].UnreadCount += 1;
                            if (this.Tabs[key].OldestUnreadId > id)
                            {
                                this.Tabs[key].OldestUnreadId = id;
                            }
                        }
                    }
                }
            }
        }

        public void SetRead()
        {
            TabClass tb = this.GetTabByType(TabUsageType.Home);
            if (!tb.UnreadManage)
            {
                return;
            }

            lock (this.lockObj)
            {
                for (int i = 0; i < tb.AllCount; i++)
                {
                    long id = tb.GetId(i);
                    if (id < 0)
                    {
                        return;
                    }

                    if (!this.statuses[id].IsReply && !this.statuses[id].IsRead && !this.statuses[id].FilterHit)
                    {
                        this.statuses[id].IsRead = true;
                        this.SetNextUnreadId(id, tb);

                        // 次の未読セット
                        foreach (string key in this.Tabs.Keys)
                        {
                            if (this.Tabs[key].UnreadManage && this.Tabs[key].Contains(id))
                            {
                                this.Tabs[key].UnreadCount -= 1;
                                if (this.Tabs[key].OldestUnreadId == id)
                                {
                                    this.Tabs[key].OldestUnreadId = -1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public PostClass Item(long id)
        {
            if (this.statuses.ContainsKey(id))
            {
                return this.statuses[id];
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
            if (!this.Tabs.ContainsKey(tabName))
            {
                throw new ArgumentException("TabName=" + tabName + " is not contained.");
            }

            long id = this.Tabs[tabName].GetId(index);
            if (id < 0)
            {
                throw new ArgumentException(string.Format("Index can't find. Index={0}/TabName={1}", index, tabName));
            }

            try
            {
                if (this.Tabs[tabName].IsInnerStorageTabType)
                {
                    return this.Tabs[tabName].Posts[this.Tabs[tabName].GetId(index)];
                }
                else
                {
                    return this.statuses[this.Tabs[tabName].GetId(index)];
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Index={0}/TabName={1}", index, tabName), ex);
            }
        }

        public PostClass[] Item(string tabName, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex + 1;
            PostClass[] posts = new PostClass[length];
            if (this.Tabs[tabName].IsInnerStorageTabType)
            {
                for (int i = 0; i < length; i++)
                {
                    posts[i] = this.Tabs[tabName].Posts[this.Tabs[tabName].GetId(startIndex + i)];
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    posts[i] = this.statuses[this.Tabs[tabName].GetId(startIndex + i)];
                }
            }

            return posts;
        }

        public bool ContainsKey(long id)
        {
            // DM,公式検索は非対応
            lock (this.lockObj)
            {
                return this.statuses.ContainsKey(id);
            }
        }

        public bool ContainsKey(long id, string tabName)
        {
            // DM,公式検索は対応版
            lock (this.lockObj)
            {
                return this.Tabs.ContainsKey(tabName) ? this.Tabs[tabName].Contains(id) : false;
            }
        }

        public void SetUnreadManage(bool manage)
        {
            if (manage)
            {
                foreach (string key in this.Tabs.Keys)
                {
                    TabClass tb = this.Tabs[key];
                    if (tb.UnreadManage)
                    {
                        lock (this.lockUnread)
                        {
                            int cnt = 0;
                            long oldest = long.MaxValue;
                            Dictionary<long, PostClass> posts = tb.IsInnerStorageTabType ? tb.Posts : this.statuses;

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
                foreach (string key in this.Tabs.Keys)
                {
                    TabClass tb = this.Tabs[key];
                    if (tb.UnreadManage && tb.UnreadCount > 0)
                    {
                        lock (this.lockUnread)
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
            TabClass tb = this.Tabs[original];
            this.Tabs.Remove(original);
            tb.TabName = newName;
            this.Tabs.Add(newName, tb);
        }

        public void FilterAll()
        {
            lock (this.lockObj)
            {
                TabClass tbr = this.GetTabByType(TabUsageType.Home);
                TabClass replyTab = this.GetTabByType(TabUsageType.Mentions);
                foreach (TabClass tb in this.Tabs.Values.ToArray())
                {
                    if (tb.FilterModified)
                    {
                        tb.FilterModified = false;
                        long[] orgIds = tb.BackupIds();
                        tb.ClearIDs();

                        // フィルター前のIDsを退避。どのタブにも含まれないidはrecentへ追加
                        // moveフィルターにヒットした際、recentに該当あればrecentから削除
                        foreach (long id in this.statuses.Keys)
                        {
                            PostClass post = this.statuses[id];
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
                                    tbr.Remove(post.StatusId, post.IsRead);
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
                                        this.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
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
                                        this.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
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
                            foreach (var tmp in this.Tabs.Values.ToArray())
                            {
                                if (tmp.Contains(id))
                                {
                                    hit = true;
                                    break;
                                }
                            }

                            if (!hit)
                            {
                                tbr.Add(id, this.statuses[id].IsRead, false);
                            }
                        }
                    }
                }

                this.SortPosts();
            }
        }

        public long[] GetId(string tabName, IEnumerable<int> indecies)
        {
            if (indecies.Count() == 0)
            {
                return null;
            }

            return indecies.Select(i => this.Tabs[tabName].GetId(i)).ToArray();
        }

        public long GetId(string tabName, int index)
        {
            return this.Tabs[tabName].GetId(index);
        }

        public int[] IndexOf(string tabName, long[] ids)
        {
            if (ids == null)
            {
                return null;
            }

            int[] idx = new int[ids.Length];
            TabClass tb = this.Tabs[tabName];
            for (int i = 0; i < ids.Length; i++)
            {
                idx[i] = tb.IndexOf(ids[i]);
            }

            return idx;
        }

        public int IndexOf(string tabName, long id)
        {
            return this.Tabs[tabName].IndexOf(id);
        }

        public void ClearTabIds(string tabName)
        {
            // 不要なPostを削除
            lock (this.lockObj)
            {
                if (!this.Tabs[tabName].IsInnerStorageTabType)
                {
                    foreach (long id in this.Tabs[tabName].BackupIds())
                    {
                        bool hit = false;
                        foreach (TabClass tb in this.Tabs.Values)
                        {
                            if (tb.Contains(id))
                            {
                                hit = true;
                                break;
                            }
                        }

                        if (!hit)
                        {
                            this.statuses.Remove(id);
                        }
                    }
                }

                // 指定タブをクリア
                this.Tabs[tabName].ClearIDs();
            }
        }

        public void SetTabUnreadManage(string tabName, bool manage)
        {
            TabClass tb = this.Tabs[tabName];
            lock (this.lockUnread)
            {
                if (manage)
                {
                    int cnt = 0;
                    long oldest = long.MaxValue;
                    Dictionary<long, PostClass> posts = tb.IsInnerStorageTabType ? tb.Posts : this.statuses;

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
            lock (this.lockObj)
            {
                if (follower.Count > 0)
                {
                    foreach (PostClass post in this.statuses.Values)
                    {
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
                    foreach (long id in this.statuses.Keys)
                    {
                        this.statuses[id].IsOwl = false;
                    }
                }
            }
        }

        public TabClass GetTabByType(TabUsageType tabType)
        {
            // Home,Mentions,DM,Favは1つに制限する
            // その他のタイプを指定されたら、最初に合致したものを返す
            // 合致しなければNothingを返す
            lock (this.lockObj)
            {
                foreach (TabClass tb in this.Tabs.Values)
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
            lock (this.lockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in this.Tabs.Values)
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
            // 合致したタブをListで返す
            // 合致しなければ空のListを返す
            lock (this.lockObj)
            {
                List<TabClass> tbs = new List<TabClass>();
                foreach (TabClass tb in this.Tabs.Values)
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
            lock (this.lockObj)
            {
                if (this.Tabs.ContainsKey(tabName))
                {
                    return this.Tabs[tabName];
                }

                return null;
            }
        }

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            if (tabName != null && this.Tabs.ContainsKey(tabName) && (this.Tabs[tabName].TabType == TabUsageType.Home || this.Tabs[tabName].TabType == TabUsageType.Mentions || this.Tabs[tabName].TabType == TabUsageType.DirectMessage || this.Tabs[tabName].TabType == TabUsageType.Favorites))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 振り分け可能タブの判定処理
        public bool IsDistributableTab(string tabName)
        {
            return tabName != null && this.Tabs.ContainsKey(tabName) && (this.Tabs[tabName].TabType == TabUsageType.Mentions || this.Tabs[tabName].TabType == TabUsageType.UserDefined);
        }

        public string GetUniqueTabName()
        {
            string tabNameTemp = "MyTab" + (this.Tabs.Count + 1).ToString();
            for (int i = 2; i <= 100; i++)
            {
                if (this.Tabs.ContainsKey(tabNameTemp))
                {
                    tabNameTemp = "MyTab" + (this.Tabs.Count + i).ToString();
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
            return this.Tabs.Values.Select(tc => tc.UnreadCount).Sum();
        }

        public int GetAllCount()
        {
            return this.Tabs.Values.Select(tc => tc.AllCount).Sum();
        }

        private void ScrubGeo(long userId, long uptoStatusId)
        {
            lock (this.lockObj)
            {
                var userPosts = from post in this.statuses.Values
                                where post.UserId == userId && post.UserId <= uptoStatusId
                                select post;

                foreach (var p in userPosts)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }

                var userPosts2 = from tb in this.GetTabsInnerStorageType()
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
            lock (this.lockObj)
            {
                PostClass post = null;
                if (this.statuses.ContainsKey(id))
                {
                    post = this.statuses[id];
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

        private void SetNextUnreadId(long currentId, TabClass tab)
        {
            // CurrentID:今既読にしたID(OldestIDの可能性あり)
            // 最古未読が設定されていて、既読の場合（1発言以上存在）
            try
            {
                Dictionary<long, PostClass> posts = tab.IsInnerStorageTabType ? tab.Posts : this.statuses;

                // 次の未読探索
                if (tab.OldestUnreadId > -1 && posts.ContainsKey(tab.OldestUnreadId) && posts[tab.OldestUnreadId].IsRead && this.sorter.Mode == IdComparerClass.ComparerMode.Id)
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
                            this.FindUnreadId(idx, tab);
                        }
                        else
                        {
                            // 頭から探索
                            this.FindUnreadId(-1, tab);
                        }
                    }
                    else
                    {
                        // 頭から探索
                        this.FindUnreadId(-1, tab);
                    }
                }
                else
                {
                    // 頭から探索
                    this.FindUnreadId(-1, tab);
                }
            }
            catch (KeyNotFoundException)
            {
                // 頭から探索
                this.FindUnreadId(-1, tab);
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
            if (this.sorter.Order == System.Windows.Forms.SortOrder.Ascending)
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

            Dictionary<long, PostClass> posts = tab.IsInnerStorageTabType ? tab.Posts : this.statuses;

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
            TabClass homeTab = this.GetTabByType(TabUsageType.Home);
            TabClass replyTab = this.GetTabByType(TabUsageType.Mentions);
            TabClass dmsgTab = this.GetTabByType(TabUsageType.DirectMessage);
            TabClass favTab = this.GetTabByType(TabUsageType.Favorites);
            foreach (long id in this.addedIds)
            {
                PostClass post = this.statuses[id];
                bool @add = false;              // 通知リスト追加フラグ
                bool mv = false;                // 移動フラグ（Recent追加有無）
                HITRESULT rslt = HITRESULT.None;
                post.IsExcludeReply = false;
                foreach (string tn in this.Tabs.Keys)
                {
                    rslt = this.Tabs[tn].AddFiltered(post);
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

                        if (this.Tabs[tn].Notify)
                        {
                            @add = true;
                        }

                        // 通知あり
                        if (!string.IsNullOrEmpty(this.Tabs[tn].SoundFile) && string.IsNullOrEmpty(this.soundFile))
                        {
                            this.soundFile = this.Tabs[tn].SoundFile;                            // wavファイル（未設定の場合のみ）
                        }

                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == HITRESULT.Exclude && this.Tabs[tn].TabType == TabUsageType.Mentions)
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
                    if (!string.IsNullOrEmpty(homeTab.SoundFile) && string.IsNullOrEmpty(this.soundFile))
                    {
                        this.soundFile = homeTab.SoundFile;
                    }

                    if (homeTab.Notify)
                    {
                        @add = true;
                    }
                }

                // 除外ルール適用のないReplyならReplyタブに追加
                if (post.IsReply && !post.IsExcludeReply)
                {
                    replyTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(replyTab.SoundFile))
                    {
                        this.soundFile = replyTab.SoundFile;
                    }

                    if (replyTab.Notify)
                    {
                        @add = true;
                    }
                }

                // Fav済み発言だったらFavoritesタブに追加
                if (post.IsFav)
                {
                    if (favTab.Contains(post.StatusId))
                    {
                        // 取得済みなら非通知
                        // _soundFile = ""
                        @add = false;
                    }
                    else
                    {
                        favTab.Add(post.StatusId, post.IsRead, true);
                        if (!string.IsNullOrEmpty(favTab.SoundFile) && string.IsNullOrEmpty(this.soundFile))
                        {
                            this.soundFile = favTab.SoundFile;
                        }

                        if (favTab.Notify)
                        {
                            @add = true;
                        }
                    }
                }

                if (@add)
                {
                    this.notifyPosts.Add(post);
                }
            }

            foreach (TabClass tb in this.Tabs.Values)
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
                                foreach (PostClass npost in this.notifyPosts)
                                {
                                    if (npost.StatusId == post.StatusId)
                                    {
                                        exist = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }

                                if (!exist)
                                {
                                    this.notifyPosts.Add(post);
                                }
                            }

                            if (!string.IsNullOrEmpty(tb.SoundFile))
                            {
                                if (tb.TabType == TabUsageType.DirectMessage || string.IsNullOrEmpty(this.soundFile))
                                {
                                    this.soundFile = tb.SoundFile;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddRetweet(PostClass item)
        {
            // True:追加、False:保持済み
            if (this.retweets.ContainsKey(item.RetweetedId))
            {
                this.retweets[item.RetweetedId].RetweetedCount += 1;
                if (this.retweets[item.RetweetedId].RetweetedCount > 10)
                {
                    this.retweets[item.RetweetedId].RetweetedCount = 0;
                }

                return;
            }

            this.retweets.Add(item.RetweetedId, new PostClass(item.Nickname, item.TextFromApi, item.Text, item.ImageUrl, item.ScreenName, item.CreatedAt, item.RetweetedId, item.IsFav, item.IsRead, item.IsReply, item.IsExcludeReply, item.IsProtect, item.IsOwl, item.IsMark, item.InReplyToUser, item.InReplyToStatusId, item.Source, item.SourceHtml, item.ReplyToList, item.IsMe, item.IsDm, item.UserId, item.FilterHit, string.Empty, 0, item.PostGeo));
            this.retweets[item.RetweetedId].RetweetedCount += 1;
        }
    }
}