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
    [Serializable]
    public sealed class TabClass
    {
        private bool _unreadManage = false;
        private List<FiltersClass> _filters;
        private int _unreadCount = 0;
        private List<long> _ids;
        private List<TemporaryId> _tmpIds = new List<TemporaryId>();
        private TabUsageType _tabType = TabUsageType.Undefined;
        private IdComparerClass _sorter = new IdComparerClass();
        private readonly object _lockObj = new object();

        public string User { get; set; }

        #region "検索"

        // Search query
        private string _searchLang = "";

        private string _searchWords = "";
        private string _nextPageQuery = "";

        public string SearchLang
        {
            get { return this._searchLang; }
            set
            {
                this.SinceId = 0;
                this._searchLang = value;
            }
        }

        public string SearchWords
        {
            get { return this._searchWords; }
            set
            {
                this.SinceId = 0;
                this._searchWords = value.Trim();
            }
        }

        public string NextPageQuery
        {
            get { return this._nextPageQuery; }
            set { this._nextPageQuery = value; }
        }

        public int GetSearchPage(int count)
        {
            return (this._ids.Count / count) + 1;
        }

        private Dictionary<string, string> _beforeQuery = new Dictionary<string, string>();

        public void SaveQuery(bool more)
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this._searchWords))
            {
                this._beforeQuery = qry;
                return;
            }
            qry.Add("q", this._searchWords);
            if (!string.IsNullOrEmpty(this._searchLang))
            {
                qry.Add("lang", this._searchLang);
            }
            this._beforeQuery = qry;
        }

        public bool IsQueryChanged()
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this._searchWords))
            {
                qry.Add("q", this._searchWords);
                if (!string.IsNullOrEmpty(this._searchLang))
                {
                    qry.Add("lang", this._searchLang);
                }
            }
            if (qry.Count != this._beforeQuery.Count)
            {
                return true;
            }

            foreach (KeyValuePair<string, string> kvp in qry)
            {
                if (!this._beforeQuery.ContainsKey(kvp.Key) || this._beforeQuery[kvp.Key] != kvp.Value)
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
            get { return this._listInfo; }
            set { this._listInfo = value; }
        }

        #endregion "リスト"

        [System.Xml.Serialization.XmlIgnore]
        public PostClass RelationTargetPost { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public long OldestId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public long SinceId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Dictionary<long, PostClass> Posts { get; set; }

        public PostClass[] GetTemporaryPosts()
        {
            List<PostClass> tempPosts = new List<PostClass>();
            if (this._tmpIds.Count == 0)
            {
                return tempPosts.ToArray();
            }
            foreach (TemporaryId tempId in this._tmpIds)
            {
                tempPosts.Add(this.Posts[tempId.Id]);
            }
            return tempPosts.ToArray();
        }

        public int GetTemporaryCount()
        {
            return this._tmpIds.Count;
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

        public TabClass()
        {
            this.Posts = new Dictionary<long, PostClass>();
            this._filters = new List<FiltersClass>();
            this.Notify = true;
            this.SoundFile = "";
            this._unreadManage = true;
            this._ids = new List<long>();
            this.OldestUnreadId = -1;
            this._tabType = TabUsageType.Undefined;
            this._listInfo = null;
        }

        public TabClass(string tabName, TabUsageType tabType, ListElement list)
        {
            this.Posts = new Dictionary<long, PostClass>();
            this.TabName = tabName;
            this._filters = new List<FiltersClass>();
            this.Notify = true;
            this.SoundFile = "";
            this._unreadManage = true;
            this._ids = new List<long>();
            this.OldestUnreadId = -1;
            this._tabType = tabType;
            this.ListInfo = list;
            if (this.IsInnerStorageTabType)
            {
                this._sorter.Posts = this.Posts;
            }
            else
            {
                this._sorter.Posts = TabInformations.GetInstance().Posts;
            }
        }

        public void Sort()
        {
            if (this._sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                this._ids.Sort(this._sorter.GetCompareMethod());
                return;
            }
            long[] ar = null;
            if (this._sorter.Order == SortOrder.Ascending)
            {
                switch (this._sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = this._ids.OrderBy(n => this._sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = this._ids.OrderBy(n => this._sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = this._ids.OrderBy(n => this._sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = this._ids.OrderBy(n => this._sorter.Posts[n].Source).ToArray();
                        break;
                }
            }
            else
            {
                switch (this._sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = this._ids.OrderByDescending(n => this._sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = this._ids.OrderByDescending(n => this._sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = this._ids.OrderByDescending(n => this._sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = this._ids.OrderByDescending(n => this._sorter.Posts[n].Source).ToArray();
                        break;
                }
            }
            this._ids = new List<long>(ar);
        }

        public IdComparerClass Sorter
        {
            get { return this._sorter; }
        }

        // 無条件に追加
        private void Add(long id, bool read)
        {
            if (this._ids.Contains(id))
            {
                return;
            }

            if (this.Sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                if (this.Sorter.Order == SortOrder.Ascending)
                {
                    this._ids.Add(id);
                }
                else
                {
                    this._ids.Insert(0, id);
                }
            }
            else
            {
                this._ids.Add(id);
            }

            if (!read && this._unreadManage)
            {
                this._unreadCount += 1;
                if (id < this.OldestUnreadId)
                {
                    this.OldestUnreadId = id;
                }
            }
        }

        public void Add(long id, bool read, bool temporary)
        {
            if (!temporary)
            {
                this.Add(id, read);
            }
            else
            {
                this._tmpIds.Add(new TemporaryId(id, read));
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
            lock (this._lockObj)
            {
                foreach (FiltersClass ft in this._filters)
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
                this._tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
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
            this._tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
        }

        public void AddSubmit(ref bool isMentionIncluded)
        {
            if (this._tmpIds.Count == 0)
            {
                return;
            }
            this._tmpIds.Sort((TemporaryId x, TemporaryId y) => x.Id.CompareTo(y.Id));
            foreach (TemporaryId tId in this._tmpIds)
            {
                if (this.TabType == TabUsageType.Mentions && TabInformations.GetInstance().Item(tId.Id).IsReply)
                {
                    isMentionIncluded = true;
                }
                this.Add(tId.Id, tId.Read);
            }
            this._tmpIds.Clear();
        }

        public void AddSubmit()
        {
            bool mention = false;
            this.AddSubmit(ref mention);
        }

        public void Remove(long id)
        {
            if (!this._ids.Contains(id))
            {
                return;
            }
            this._ids.Remove(id);
            if (this.IsInnerStorageTabType)
            {
                this.Posts.Remove(id);
            }
        }

        public void Remove(long id, bool read)
        {
            if (!this._ids.Contains(id))
            {
                return;
            }

            if (!read && this._unreadManage)
            {
                this._unreadCount -= 1;
                this.OldestUnreadId = -1;
            }

            this._ids.Remove(id);
            if (this.IsInnerStorageTabType)
            {
                this.Posts.Remove(id);
            }
        }

        public bool UnreadManage
        {
            get { return this._unreadManage; }
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

        [System.Xml.Serialization.XmlIgnore]
        public long OldestUnreadId { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public int UnreadCount
        {
            get { return this.UnreadManage && AppendSettingDialog.Instance.UnreadManage ? this._unreadCount : 0; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this._unreadCount = value;
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
                return this._filters.ToArray();
            }
        }

        public void RemoveFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                this._filters.Remove(filter);
                this.FilterModified = true;
            }
        }

        public bool AddFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                if (this._filters.Contains(filter))
                {
                    return false;
                }
                this._filters.Add(filter);
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

        [System.Xml.Serialization.XmlIgnore]
        public List<FiltersClass> Filters
        {
            get { lock (this._lockObj) { return this._filters; } }
            set { lock (this._lockObj) { this._filters = value; } }
        }

        public FiltersClass[] FilterArray
        {
            get { lock (this._lockObj) { return this._filters.ToArray(); } }
            set
            {
                lock (this._lockObj)
                {
                    foreach (FiltersClass filters in value)
                    {
                        this._filters.Add(filters);
                    }
                }
            }
        }

        public bool Contains(long id)
        {
            return this._ids.Contains(id);
        }

        public void ClearIDs()
        {
            this._ids.Clear();
            this._tmpIds.Clear();
            this._unreadCount = 0;
            this.OldestUnreadId = -1;
            if (this.Posts != null)
            {
                this.Posts.Clear();
            }
        }

        public long GetId(int index)
        {
            return index < this._ids.Count ? this._ids[index] : -1;
        }

        public int IndexOf(long id)
        {
            return this._ids.IndexOf(id);
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool FilterModified { get; set; }

        public long[] BackupIds()
        {
            return this._ids.ToArray();
        }

        public string TabName { get; set; }

        public TabUsageType TabType
        {
            get { return this._tabType; }
            set
            {
                this._tabType = value;
                if (this.IsInnerStorageTabType)
                {
                    this._sorter.Posts = this.Posts;
                }
                else
                {
                    this._sorter.Posts = TabInformations.GetInstance().Posts;
                }
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                return this._tabType == TabUsageType.PublicSearch
                    || this._tabType == TabUsageType.DirectMessage
                    || this._tabType == TabUsageType.Lists
                    || this._tabType == TabUsageType.UserTimeline 
                    || this._tabType == TabUsageType.Related;
            }
        }
    }
}