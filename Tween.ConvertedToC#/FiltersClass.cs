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
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Hoehoe
{
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
            if (!String.IsNullOrEmpty(_name) || _body.Count > 0 || _isRt || !String.IsNullOrEmpty(_source))
            {
                if (_searchBoth)
                {
                    if (!String.IsNullOrEmpty(_name))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, _name);
                    }
                    else
                    {
                        fs.Append(Hoehoe.Properties.Resources.SetFiltersText2);
                    }
                }
                if (_body.Count > 0)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText3);
                    foreach (string bf in _body)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length -= 1;
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_searchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }
                if (_useRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }
                if (_searchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }
                if (_caseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }
                if (_isRt)
                {
                    fs.Append("RT/");
                }
                if (_useLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!String.IsNullOrEmpty(_source))
                {
                    fs.AppendFormat("Src…{0}/", _source);
                }
                fs.Length -= 1;
                fs.Append(")");
            }
            if (!String.IsNullOrEmpty(_exname) || _exbody.Count > 0 || _isExRt || !String.IsNullOrEmpty(_exSource))
            {
                //除外
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText12);
                if (_exsearchBoth)
                {
                    if (!String.IsNullOrEmpty(_exname))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, _exname);
                    }
                    else
                    {
                        fs.Append(Hoehoe.Properties.Resources.SetFiltersText2);
                    }
                }
                if (_exbody.Count > 0)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText3);
                    foreach (string bf in _exbody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length -= 1;
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_exsearchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }
                if (_exuseRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }
                if (_exsearchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }
                if (_excaseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }
                if (_isExRt)
                {
                    fs.Append("RT/");
                }
                if (_exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!String.IsNullOrEmpty(_exSource))
                {
                    fs.AppendFormat("Src…{0}/", _exSource);
                }
                fs.Length -= 1;
                fs.Append(")");
            }

            fs.Append("(");
            if (_moveFrom)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText11);
            }
            if (!_moveFrom && _setMark)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText10);
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

        [XmlIgnore]
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

        [XmlIgnore]
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

        public Hoehoe.MyCommon.HITRESULT IsHit(PostClass post)
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
            StringComparison compOpt = default(StringComparison);
            RegexOptions rgOpt = default(RegexOptions);
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
                if (String.IsNullOrEmpty(_name) || (!_useRegex && (post.ScreenName.Equals(_name, compOpt) || post.RetweetedBy.Equals(_name, compOpt))) || (_useRegex && (Regex.IsMatch(post.ScreenName, _name, rgOpt) || (!String.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _name, rgOpt)))))
                {
                    if (_useLambda)
                    {
                        if (!ExecuteLambdaExpression(_body[0], post))
                        {
                            bHit = false;
                        }
                    }
                    else
                    {
                        foreach (string fs in _body)
                        {
                            if (_useRegex)
                            {
                                if (!Regex.IsMatch(tBody, fs, rgOpt))
                                {
                                    bHit = false;
                                }
                            }
                            else
                            {
                                if (_caseSensitive)
                                {
                                    if (!tBody.Contains(fs))
                                    {
                                        bHit = false;
                                    }
                                }
                                else
                                {
                                    if (!tBody.ToLower().Contains(fs.ToLower()))
                                    {
                                        bHit = false;
                                    }
                                }
                            }
                            if (!bHit)
                            {
                                break;
                            }
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
                    {
                        bHit = false;
                    }
                }
                else
                {
                    foreach (string fs in _body)
                    {
                        if (_useRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, rgOpt) || (!String.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) || Regex.IsMatch(tBody, fs, rgOpt)))
                            {
                                bHit = false;
                            }
                        }
                        else
                        {
                            if (_caseSensitive)
                            {
                                if (!(post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || tBody.Contains(fs)))
                                {
                                    bHit = false;
                                }
                            }
                            else
                            {
                                if (!(post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || tBody.ToLower().Contains(fs.ToLower())))
                                {
                                    bHit = false;
                                }
                            }
                        }
                        if (!bHit)
                        {
                            break;
                        }
                    }
                }
            }
            if (_isRt)
            {
                if (post.RetweetedId == 0)
                {
                    bHit = false;
                }
            }
            if (!String.IsNullOrEmpty(_source))
            {
                if (_useRegex)
                {
                    if (!Regex.IsMatch(tSource, _source, rgOpt))
                    {
                        bHit = false;
                    }
                }
                else
                {
                    if (!tSource.Equals(_source, compOpt))
                    {
                        bHit = false;
                    }
                }
            }
            if (!bHit)
            {
                return Hoehoe.MyCommon.HITRESULT.None;
            }
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
            if (!String.IsNullOrEmpty(_exname) || _exbody.Count > 0)
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
                    if (String.IsNullOrEmpty(_exname) || (!_exuseRegex && (post.ScreenName.Equals(_exname, compOpt) || post.RetweetedBy.Equals(_exname, compOpt))) || (_exuseRegex && (Regex.IsMatch(post.ScreenName, _exname, rgOpt) || (!String.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _exname, rgOpt)))))
                    {
                        if (_exbody.Count > 0)
                        {
                            if (_exuseLambda)
                            {
                                if (ExecuteExLambdaExpression(_exbody[0], post))
                                {
                                    exFlag = true;
                                }
                            }
                            else
                            {
                                foreach (string fs in _exbody)
                                {
                                    if (_exuseRegex)
                                    {
                                        if (Regex.IsMatch(tBody, fs, rgOpt))
                                        {
                                            exFlag = true;
                                        }
                                    }
                                    else
                                    {
                                        if (_excaseSensitive)
                                        {
                                            if (tBody.Contains(fs))
                                            {
                                                exFlag = true;
                                            }
                                        }
                                        else
                                        {
                                            if (tBody.ToLower().Contains(fs.ToLower()))
                                            {
                                                exFlag = true;
                                            }
                                        }
                                    }
                                    if (exFlag)
                                    {
                                        break;
                                    }
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
                                if (Regex.IsMatch(post.ScreenName, fs, rgOpt) || (!String.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) || Regex.IsMatch(tBody, fs, rgOpt))
                                {
                                    exFlag = true;
                                }
                            }
                            else
                            {
                                if (_excaseSensitive)
                                {
                                    if (post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || tBody.Contains(fs))
                                    {
                                        exFlag = true;
                                    }
                                }
                                else
                                {
                                    if (post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || tBody.ToLower().Contains(fs.ToLower()))
                                    {
                                        exFlag = true;
                                    }
                                }
                            }
                            if (exFlag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (_isExRt)
            {
                if (post.RetweetedId > 0)
                {
                    exFlag = true;
                }
            }
            if (!String.IsNullOrEmpty(_exSource))
            {
                if (_exuseRegex)
                {
                    if (Regex.IsMatch(tSource, _exSource, rgOpt))
                    {
                        exFlag = true;
                    }
                }
                else
                {
                    if (tSource.Equals(_exSource, compOpt))
                    {
                        exFlag = true;
                    }
                }
            }
            if (String.IsNullOrEmpty(_name) && _body.Count == 0 && !_isRt && String.IsNullOrEmpty(_source))
            {
                bHit = false;
            }
            if (bHit)
            {
                if (exFlag)
                {
                    return Hoehoe.MyCommon.HITRESULT.Exclude;
                }
                else
                {
                    if (_moveFrom)
                    {
                        return Hoehoe.MyCommon.HITRESULT.Move;
                    }
                    else
                    {
                        return _setMark ? Hoehoe.MyCommon.HITRESULT.CopyAndMark : Hoehoe.MyCommon.HITRESULT.Copy;
                    }
                }
            }
            else
            {
                return exFlag ? Hoehoe.MyCommon.HITRESULT.Exclude : Hoehoe.MyCommon.HITRESULT.None;
            }
        }

        public bool Equals(FiltersClass other)
        {
            if (this.BodyFilter.Count != other.BodyFilter.Count)
            {
                return false;
            }
            if (this.ExBodyFilter.Count != other.ExBodyFilter.Count)
            {
                return false;
            }
            for (int i = 0; i < this.BodyFilter.Count; i++)
            {
                if (this.BodyFilter[i] != other.BodyFilter[i])
                {
                    return false;
                }
            }
            for (int i = 0; i < this.ExBodyFilter.Count; i++)
            {
                if (this.ExBodyFilter[i] != other.ExBodyFilter[i])
                {
                    return false;
                }
            }

            return (this.MoveFrom == other.MoveFrom)
                && (this.SetMark == other.SetMark)
                && (this.NameFilter == other.NameFilter)
                && (this.SearchBoth == other.SearchBoth)
                && (this.SearchUrl == other.SearchUrl)
                && (this.UseRegex == other.UseRegex)
                && (this.ExNameFilter == other.ExNameFilter)
                && (this.ExSearchBoth == other.ExSearchBoth)
                && (this.ExSearchUrl == other.ExSearchUrl)
                && (this.ExUseRegex == other.ExUseRegex)
                && (this.IsRt == other.IsRt)
                && (this.Source == other.Source)
                && (this.IsExRt == other.IsExRt)
                && (this.ExSource == other.ExSource)
                && (this.UseLambda == other.UseLambda)
                && (this.ExUseLambda == other.ExUseLambda);
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
            {
                return false;
            }
            return this.Equals((FiltersClass)obj);
        }

        public override int GetHashCode()
        {
            return this.MoveFrom.GetHashCode() ^ this.SetMark.GetHashCode()
                ^ this.BodyFilter.GetHashCode() ^ this.NameFilter.GetHashCode()
                ^ this.SearchBoth.GetHashCode() ^ this.SearchUrl.GetHashCode()
                ^ this.UseRegex.GetHashCode() ^ this.ExBodyFilter.GetHashCode()
                ^ this.ExNameFilter.GetHashCode() ^ this.ExSearchBoth.GetHashCode()
                ^ this.ExSearchUrl.GetHashCode() ^ this.ExUseRegex.GetHashCode()
                ^ this.IsRt.GetHashCode() ^ this.Source.GetHashCode()
                ^ this.IsExRt.GetHashCode() ^ this.ExSource.GetHashCode()
                ^ this.UseLambda.GetHashCode() ^ this.ExUseLambda.GetHashCode();
        }
    }
}