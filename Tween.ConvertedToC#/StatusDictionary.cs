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