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

    [Serializable]
    public sealed class TabClass
    {
        private readonly object lockObj = new object();

        private bool unreadManage;
        private List<FiltersClass> filters;
        private int unreadCount = 0;
        private List<long> ids;
        private List<TemporaryId> tmpIds = new List<TemporaryId>();
        private TabUsageType tabType = TabUsageType.Undefined;
        private IdComparerClass sorter = new IdComparerClass();

        private string searchLang = string.Empty;
        private string searchWords = string.Empty;
        private string nextPageQuery = string.Empty;
        private Dictionary<string, string> beforeQuery = new Dictionary<string, string>();

        private ListElement listInfo;

        public TabClass()
        {
            this.Posts = new Dictionary<long, PostClass>();
            this.filters = new List<FiltersClass>();
            this.Notify = true;
            this.SoundFile = string.Empty;
            this.unreadManage = true;
            this.ids = new List<long>();
            this.OldestUnreadId = -1;
            this.tabType = TabUsageType.Undefined;
            this.listInfo = null;
        }

        public TabClass(string tabName, TabUsageType tabType, ListElement list)
        {
            this.Posts = new Dictionary<long, PostClass>();
            this.TabName = tabName;
            this.filters = new List<FiltersClass>();
            this.Notify = true;
            this.SoundFile = string.Empty;
            this.unreadManage = true;
            this.ids = new List<long>();
            this.OldestUnreadId = -1;
            this.tabType = tabType;
            this.ListInfo = list;
            if (this.IsInnerStorageTabType)
            {
                this.sorter.Posts = this.Posts;
            }
            else
            {
                this.sorter.Posts = TabInformations.GetInstance().Posts;
            }
        }

        public string User { get; set; }

        public string SearchLang
        {
            get
            {
                return this.searchLang;
            }

            set
            {
                this.SinceId = 0;
                this.searchLang = value;
            }
        }

        public string SearchWords
        {
            get
            {
                return this.searchWords;
            }

            set
            {
                this.SinceId = 0;
                this.searchWords = value.Trim();
            }
        }

        public string NextPageQuery
        {
            get { return this.nextPageQuery; }
            set { this.nextPageQuery = value; }
        }

        public ListElement ListInfo
        {
            get { return this.listInfo; }
            set { this.listInfo = value; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public PostClass RelationTargetPost { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public long OldestId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public long SinceId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Dictionary<long, PostClass> Posts { get; set; }

        public IdComparerClass Sorter
        {
            get { return this.sorter; }
        }

        public bool UnreadManage
        {
            get
            {
                return this.unreadManage;
            }

            set
            {
                this.unreadManage = value;
                if (!value)
                {
                    this.OldestUnreadId = -1;
                    this.unreadCount = 0;
                }
            }
        }

        public bool Notify { get; set; }

        public string SoundFile { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public long OldestUnreadId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public int UnreadCount
        {
            get { return this.UnreadManage && AppendSettingDialog.Instance.UnreadManage ? this.unreadCount : 0; }
            set { this.unreadCount = value < 0 ? 0 : value; }
        }

        public int AllCount
        {
            get { return this.ids.Count; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public List<FiltersClass> Filters
        {
            get
            {
                lock (this.lockObj)
                {
                    return this.filters;
                }
            }

            set
            {
                lock (this.lockObj)
                {
                    this.filters = value;
                }
            }
        }

        public FiltersClass[] FilterArray
        {
            get
            {
                lock (this.lockObj)
                {
                    return this.filters.ToArray();
                }
            }

            set
            {
                lock (this.lockObj)
                {
                    foreach (FiltersClass filters in value)
                    {
                        this.filters.Add(filters);
                    }
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool FilterModified { get; set; }

        public string TabName { get; set; }

        public TabUsageType TabType
        {
            get
            {
                return this.tabType;
            }

            set
            {
                this.tabType = value;
                if (this.IsInnerStorageTabType)
                {
                    this.sorter.Posts = this.Posts;
                }
                else
                {
                    this.sorter.Posts = TabInformations.GetInstance().Posts;
                }
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                return this.tabType == TabUsageType.PublicSearch
                    || this.tabType == TabUsageType.DirectMessage
                    || this.tabType == TabUsageType.Lists
                    || this.tabType == TabUsageType.UserTimeline
                    || this.tabType == TabUsageType.Related;
            }
        }

        public int GetSearchPage(int count)
        {
            return (this.ids.Count / count) + 1;
        }

        public void SaveQuery(bool more)
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this.searchWords))
            {
                this.beforeQuery = qry;
                return;
            }

            qry.Add("q", this.searchWords);
            if (!string.IsNullOrEmpty(this.searchLang))
            {
                qry.Add("lang", this.searchLang);
            }

            this.beforeQuery = qry;
        }

        public bool IsQueryChanged()
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.searchWords))
            {
                qry.Add("q", this.searchWords);
                if (!string.IsNullOrEmpty(this.searchLang))
                {
                    qry.Add("lang", this.searchLang);
                }
            }

            if (qry.Count != this.beforeQuery.Count)
            {
                return true;
            }

            foreach (KeyValuePair<string, string> kvp in qry)
            {
                if (!this.beforeQuery.ContainsKey(kvp.Key) || this.beforeQuery[kvp.Key] != kvp.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public PostClass[] GetTemporaryPosts()
        {
            List<PostClass> tempPosts = new List<PostClass>();
            if (this.tmpIds.Count == 0)
            {
                return tempPosts.ToArray();
            }

            foreach (TemporaryId tempId in this.tmpIds)
            {
                tempPosts.Add(this.Posts[tempId.Id]);
            }

            return tempPosts.ToArray();
        }

        public int GetTemporaryCount()
        {
            return this.tmpIds.Count;
        }

        public void Sort()
        {
            if (this.sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                this.ids.Sort(this.sorter.GetCompareMethod());
                return;
            }

            long[] ar = null;
            if (this.sorter.Order == SortOrder.Ascending)
            {
                switch (this.sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = this.ids.OrderBy(n => this.sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = this.ids.OrderBy(n => this.sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = this.ids.OrderBy(n => this.sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = this.ids.OrderBy(n => this.sorter.Posts[n].Source).ToArray();
                        break;
                }
            }
            else
            {
                switch (this.sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = this.ids.OrderByDescending(n => this.sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = this.ids.OrderByDescending(n => this.sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = this.ids.OrderByDescending(n => this.sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = this.ids.OrderByDescending(n => this.sorter.Posts[n].Source).ToArray();
                        break;
                }
            }

            this.ids = new List<long>(ar);
        }

        public void Add(long id, bool read, bool temporary)
        {
            if (!temporary)
            {
                this.Add(id, read);
            }
            else
            {
                this.tmpIds.Add(new TemporaryId(id, read));
            }
        }

        // フィルタに合致したら追加
        public HITRESULT AddFiltered(PostClass post)
        {
            if (this.IsInnerStorageTabType)
            {
                return HITRESULT.None;
            }

            HITRESULT rslt = HITRESULT.None;

            // 全フィルタ評価（優先順位あり）
            lock (this.lockObj)
            {
                foreach (FiltersClass ft in this.filters)
                {
                    try
                    {
                        switch (ft.IsHit(post))
                        {
                            // フィルタクラスでヒット判定
                            case HITRESULT.None:
                                break;
                            case HITRESULT.Copy:
                                if (rslt != HITRESULT.CopyAndMark)
                                {
                                    rslt = HITRESULT.Copy;
                                }

                                break;
                            case HITRESULT.CopyAndMark:
                                rslt = HITRESULT.CopyAndMark;
                                break;
                            case HITRESULT.Move:
                                rslt = HITRESULT.Move;
                                break;
                            case HITRESULT.Exclude:
                                rslt = HITRESULT.Exclude;
                                break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        // IsHitでNullRef出る場合あり。暫定対応
                        MyCommon.TraceOut("IsHitでNullRef: " + ft.ToString());
                        rslt = HITRESULT.None;
                    }
                }
            }

            if (rslt != HITRESULT.None && rslt != HITRESULT.Exclude)
            {
                this.tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
            }

            // マーク付けは呼び出し元で行うこと
            return rslt;
        }

        // 検索結果の追加
        public void AddPostToInnerStorage(PostClass post)
        {
            if (this.Posts.ContainsKey(post.StatusId))
            {
                return;
            }

            this.Posts.Add(post.StatusId, post);
            this.tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
        }

        public void AddSubmit(ref bool isMentionIncluded)
        {
            if (this.tmpIds.Count == 0)
            {
                return;
            }

            this.tmpIds.Sort((TemporaryId x, TemporaryId y) => x.Id.CompareTo(y.Id));
            foreach (TemporaryId tId in this.tmpIds)
            {
                if (this.TabType == TabUsageType.Mentions && TabInformations.GetInstance().Item(tId.Id).IsReply)
                {
                    isMentionIncluded = true;
                }

                this.Add(tId.Id, tId.Read);
            }

            this.tmpIds.Clear();
        }

        public void AddSubmit()
        {
            bool mention = false;
            this.AddSubmit(ref mention);
        }

        public void Remove(long id)
        {
            if (!this.ids.Contains(id))
            {
                return;
            }

            this.ids.Remove(id);
            if (this.IsInnerStorageTabType)
            {
                this.Posts.Remove(id);
            }
        }

        public void Remove(long id, bool read)
        {
            if (!this.ids.Contains(id))
            {
                return;
            }

            if (!read && this.unreadManage)
            {
                this.unreadCount -= 1;
                this.OldestUnreadId = -1;
            }

            this.ids.Remove(id);
            if (this.IsInnerStorageTabType)
            {
                this.Posts.Remove(id);
            }
        }

        public FiltersClass[] GetFilters()
        {
            lock (this.lockObj)
            {
                return this.filters.ToArray();
            }
        }

        public void RemoveFilter(FiltersClass filter)
        {
            lock (this.lockObj)
            {
                this.filters.Remove(filter);
                this.FilterModified = true;
            }
        }

        public bool AddFilter(FiltersClass filter)
        {
            lock (this.lockObj)
            {
                if (this.filters.Contains(filter))
                {
                    return false;
                }

                this.filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        public void AddFilters(IEnumerable<FiltersClass> fs)
        {
            foreach (var f in fs)
            {
                this.AddFilter(f);
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

        public bool Contains(long id)
        {
            return this.ids.Contains(id);
        }

        public void ClearIDs()
        {
            this.ids.Clear();
            this.tmpIds.Clear();
            this.unreadCount = 0;
            this.OldestUnreadId = -1;
            if (this.Posts != null)
            {
                this.Posts.Clear();
            }
        }

        public long GetId(int index)
        {
            return index < this.ids.Count ? this.ids[index] : -1;
        }

        public int IndexOf(long id)
        {
            return this.ids.IndexOf(id);
        }

        public long[] BackupIds()
        {
            return this.ids.ToArray();
        }

        // 無条件に追加
        private void Add(long id, bool read)
        {
            if (this.ids.Contains(id))
            {
                return;
            }

            if (this.Sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                if (this.Sorter.Order == SortOrder.Ascending)
                {
                    this.ids.Add(id);
                }
                else
                {
                    this.ids.Insert(0, id);
                }
            }
            else
            {
                this.ids.Add(id);
            }

            if (!read && this.unreadManage)
            {
                this.unreadCount += 1;
                if (id < this.OldestUnreadId)
                {
                    this.OldestUnreadId = id;
                }
            }
        }

        private struct TemporaryId
        {
            public long Id;
            public bool Read;

            public TemporaryId(long argId, bool argRead)
            {
                this.Id = argId;
                this.Read = argRead;
            }
        }
    }
}