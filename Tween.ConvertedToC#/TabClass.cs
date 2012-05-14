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
    [Serializable]
    public sealed class TabClass
    {
        private bool _unreadManage = false;
        private List<FiltersClass> _filters;
        private int _unreadCount = 0;
        private List<long> _ids;
        private List<TemporaryId> _tmpIds = new List<TemporaryId>();
        private MyCommon.TabUsageType _tabType = MyCommon.TabUsageType.Undefined;
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
            {
                qry.Add("lang", _searchLang);
            }
            _beforeQuery = qry;
        }

        public bool IsQueryChanged()
        {
            Dictionary<string, string> qry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_searchWords))
            {
                qry.Add("q", _searchWords);
                if (!string.IsNullOrEmpty(_searchLang))
                {
                    qry.Add("lang", _searchLang);
                }
            }
            if (qry.Count != _beforeQuery.Count)
            {
                return true;
            }

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
            if (_tmpIds.Count == 0)
            {
                return tempPosts.ToArray();
            }
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
            _tabType = MyCommon.TabUsageType.Undefined;
            _listInfo = null;
        }

        public TabClass(string tabName, MyCommon.TabUsageType tabType, ListElement list)
        {
            Posts = new Dictionary<long, PostClass>();
            TabName = tabName;
            _filters = new List<FiltersClass>();
            Notify = true;
            SoundFile = "";
            _unreadManage = true;
            _ids = new List<long>();
            OldestUnreadId = -1;
            _tabType = tabType;
            ListInfo = list;
            if (IsInnerStorageTabType)
            {
                _sorter.Posts = Posts;
            }
            else
            {
                _sorter.Posts = TabInformations.GetInstance().Posts;
            }
        }

        public void Sort()
        {
            if (_sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                _ids.Sort(_sorter.GetCompareMethod());
                return;
            }
            long[] ar = null;
            if (_sorter.Order == SortOrder.Ascending)
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderBy(n => _sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderBy(n => _sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderBy(n => _sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderBy(n => _sorter.Posts[n].Source).ToArray();
                        break;
                }
            }
            else
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderByDescending(n => _sorter.Posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderByDescending(n => _sorter.Posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderByDescending(n => _sorter.Posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderByDescending(n => _sorter.Posts[n].Source).ToArray();
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
        private void Add(long id, bool read)
        {
            if (this._ids.Contains(id))
            {
                return;
            }

            if (Sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                if (Sorter.Order == SortOrder.Ascending)
                {
                    _ids.Add(id);
                }
                else
                {
                    _ids.Insert(0, id);
                }
            }
            else
            {
                _ids.Add(id);
            }

            if (!read && _unreadManage)
            {
                _unreadCount += 1;
                if (id < OldestUnreadId)
                {
                    OldestUnreadId = id;
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
                _tmpIds.Add(new TemporaryId(id, read));
            }
        }

        //フィルタに合致したら追加
        public MyCommon.HITRESULT AddFiltered(PostClass post)
        {
            if (IsInnerStorageTabType)
            {
                return MyCommon.HITRESULT.None;
            }
            MyCommon.HITRESULT rslt = MyCommon.HITRESULT.None;
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
                            case MyCommon.HITRESULT.None:
                                break;
                            case MyCommon.HITRESULT.Copy:
                                if (rslt != MyCommon.HITRESULT.CopyAndMark)
                                {
                                    rslt = MyCommon.HITRESULT.Copy;
                                }
                                break;
                            case MyCommon.HITRESULT.CopyAndMark:
                                rslt = MyCommon.HITRESULT.CopyAndMark;
                                break;
                            case MyCommon.HITRESULT.Move:
                                rslt = MyCommon.HITRESULT.Move;
                                break;
                            case MyCommon.HITRESULT.Exclude:
                                rslt = MyCommon.HITRESULT.Exclude;
                                break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        //IsHitでNullRef出る場合あり。暫定対応
                        MyCommon.TraceOut("IsHitでNullRef: " + ft.ToString());
                        rslt = MyCommon.HITRESULT.None;
                    }
                }
            }

            if (rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
            {
                _tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
            }

            return rslt;
            //マーク付けは呼び出し元で行うこと
        }

        //検索結果の追加
        public void AddPostToInnerStorage(PostClass post)
        {
            if (Posts.ContainsKey(post.StatusId))
            {
                return;
            }
            Posts.Add(post.StatusId, post);
            _tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
        }

        public void AddSubmit(ref bool isMentionIncluded)
        {
            if (_tmpIds.Count == 0)
            {
                return;
            }
            _tmpIds.Sort((TemporaryId x, TemporaryId y) => x.Id.CompareTo(y.Id));
            foreach (TemporaryId tId in _tmpIds)
            {
                if (this.TabType == MyCommon.TabUsageType.Mentions && TabInformations.GetInstance().Item(tId.Id).IsReply)
                {
                    isMentionIncluded = true;
                }
                this.Add(tId.Id, tId.Read);
            }
            _tmpIds.Clear();
        }

        public void AddSubmit()
        {
            bool mention = false;
            AddSubmit(ref mention);
        }

        public void Remove(long id)
        {
            if (!_ids.Contains(id))
            {
                return;
            }
            _ids.Remove(id);
            if (IsInnerStorageTabType)
            {
                Posts.Remove(id);
            }
        }

        public void Remove(long id, bool read)
        {
            if (!_ids.Contains(id))
            {
                return;
            }

            if (!read && _unreadManage)
            {
                _unreadCount -= 1;
                OldestUnreadId = -1;
            }

            _ids.Remove(id);
            if (IsInnerStorageTabType)
            {
                Posts.Remove(id);
            }
        }

        public bool UnreadManage
        {
            get { return _unreadManage; }
            set
            {
                _unreadManage = value;
                if (!value)
                {
                    OldestUnreadId = -1;
                    _unreadCount = 0;
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
            get { return this.UnreadManage && AppendSettingDialog.Instance.UnreadManage ? _unreadCount : 0; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _unreadCount = value;
            }
        }

        public int AllCount
        {
            get { return _ids.Count; }
        }

        public FiltersClass[] GetFilters()
        {
            lock (_lockObj)
            {
                return _filters.ToArray();
            }
        }

        public void RemoveFilter(FiltersClass filter)
        {
            lock (_lockObj)
            {
                _filters.Remove(filter);
                FilterModified = true;
            }
        }

        public bool AddFilter(FiltersClass filter)
        {
            lock (_lockObj)
            {
                if (_filters.Contains(filter))
                {
                    return false;
                }
                _filters.Add(filter);
                FilterModified = true;
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
            FilterModified = true;
        }

        [System.Xml.Serialization.XmlIgnore]
        public List<FiltersClass> Filters
        {
            get { lock (_lockObj) { return _filters; } }
            set { lock (_lockObj) { _filters = value; } }
        }

        public FiltersClass[] FilterArray
        {
            get { lock (_lockObj) { return _filters.ToArray(); } }
            set
            {
                lock (_lockObj)
                {
                    foreach (FiltersClass filters in value)
                    {
                        _filters.Add(filters);
                    }
                }
            }
        }

        public bool Contains(long id)
        {
            return _ids.Contains(id);
        }

        public void ClearIDs()
        {
            _ids.Clear();
            _tmpIds.Clear();
            _unreadCount = 0;
            OldestUnreadId = -1;
            if (Posts != null)
            {
                Posts.Clear();
            }
        }

        public long GetId(int index)
        {
            return index < _ids.Count ? _ids[index] : -1;
        }

        public int IndexOf(long id)
        {
            return _ids.IndexOf(id);
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool FilterModified { get; set; }

        public long[] BackupIds()
        {
            return _ids.ToArray();
        }

        public string TabName { get; set; }

        public MyCommon.TabUsageType TabType
        {
            get { return _tabType; }
            set
            {
                _tabType = value;
                if (IsInnerStorageTabType)
                {
                    _sorter.Posts = Posts;
                }
                else
                {
                    _sorter.Posts = TabInformations.GetInstance().Posts;
                }
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                return _tabType == MyCommon.TabUsageType.PublicSearch || _tabType == MyCommon.TabUsageType.DirectMessage || _tabType == MyCommon.TabUsageType.Lists || _tabType == MyCommon.TabUsageType.UserTimeline || _tabType == MyCommon.TabUsageType.Related;
            }
        }
    }
}