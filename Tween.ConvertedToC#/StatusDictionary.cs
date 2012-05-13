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
}