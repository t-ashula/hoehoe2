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
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using R = Properties.Resources;

    [Serializable]
    public sealed class FiltersClass : IEquatable<FiltersClass>
    {
        private List<string> _body;
        private List<string> _exbody;
        private bool _useLambda;
        private bool _exuseLambda;
        private LambdaExpression _lambdaExp;
        private Delegate _lambdaExpDelegate;
        private LambdaExpression _exlambdaExp;
        private Delegate _exlambdaExpDelegate;

        public FiltersClass()
        {
            NameFilter = string.Empty;
            ExNameFilter = string.Empty;
            _body = new List<string>();
            _exbody = new List<string>();
            SearchBoth = true;
            ExSearchBoth = true;
            SetMark = true;
            Source = string.Empty;
            ExSource = string.Empty;
        }

        public string NameFilter { get; set; }

        public string ExNameFilter { get; set; }

        [XmlIgnore]
        public List<string> BodyFilter
        {
            get
            {
                return _body;
            }

            set
            {
                ClearLambdaExp();
                _body = value;
            }
        }

        public string[] BodyFilterArray
        {
            get { return _body.ToArray(); }
            set { _body = new List<string>(value); }
        }

        [XmlIgnore]
        public List<string> ExBodyFilter
        {
            get
            {
                return _exbody;
            }

            set
            {
                ClearExLambdaExp();
                _exbody = value;
            }
        }

        public string[] ExBodyFilterArray
        {
            get { return _exbody.ToArray(); }
            set { _exbody = new List<string>(value); }
        }

        public bool SearchBoth { get; set; }

        public bool ExSearchBoth { get; set; }

        public bool MoveFrom { get; set; }

        public bool SetMark { get; set; }

        public bool SearchUrl { get; set; }

        public bool ExSearchUrl { get; set; }

        public bool CaseSensitive { get; set; }

        public bool ExCaseSensitive { get; set; }

        public bool UseLambda
        {
            get
            {
                return _useLambda;
            }

            set
            {
                ClearLambdaExp();
                _useLambda = value;
            }
        }

        public bool ExUseLambda
        {
            get
            {
                return _exuseLambda;
            }

            set
            {
                ClearExLambdaExp();
                _exuseLambda = value;
            }
        }

        public bool UseRegex { get; set; }

        public bool ExUseRegex { get; set; }

        public bool IsRt { get; set; }

        public bool IsExRt { get; set; }

        public string Source { get; set; }

        public string ExSource { get; set; }

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

            return (bool)_lambdaExpDelegate.DynamicInvoke(post);
        }

        public bool ExecuteExLambdaExpression(string expr, PostClass post)
        {
            if (_exlambdaExp == null || _exlambdaExpDelegate == null)
            {
                _exlambdaExp = DynamicExpression.ParseLambda<PostClass, bool>(expr, post);
                _exlambdaExpDelegate = _exlambdaExp.Compile();
            }

            return (bool)_exlambdaExpDelegate.DynamicInvoke(post);
        }

        public HITRESULT IsHit(PostClass post)
        {
            bool isHit = true;
            string bodyText = null;
            string sourceText = null;
            if (SearchUrl)
            {
                bodyText = post.Text;
                sourceText = post.SourceHtml;
            }
            else
            {
                bodyText = post.TextFromApi;
                sourceText = post.Source;
            }

            // 検索オプション
            StringComparison compOpt = default(StringComparison);
            RegexOptions regexOption = default(RegexOptions);
            if (CaseSensitive)
            {
                compOpt = StringComparison.Ordinal;
                regexOption = RegexOptions.None;
            }
            else
            {
                compOpt = StringComparison.OrdinalIgnoreCase;
                regexOption = RegexOptions.IgnoreCase;
            }

            if (SearchBoth)
            {
                if (string.IsNullOrEmpty(NameFilter) || (!UseRegex && (post.ScreenName.Equals(NameFilter, compOpt) || post.RetweetedBy.Equals(NameFilter, compOpt))) || (UseRegex && (Regex.IsMatch(post.ScreenName, NameFilter, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, NameFilter, regexOption)))))
                {
                    if (_useLambda)
                    {
                        if (!ExecuteLambdaExpression(_body[0], post))
                        {
                            isHit = false;
                        }
                    }
                    else
                    {
                        foreach (string fs in _body)
                        {
                            if (UseRegex)
                            {
                                if (!Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isHit = false;
                                }
                            }
                            else
                            {
                                if (CaseSensitive)
                                {
                                    if (!bodyText.Contains(fs))
                                    {
                                        isHit = false;
                                    }
                                }
                                else
                                {
                                    if (!bodyText.ToLower().Contains(fs.ToLower()))
                                    {
                                        isHit = false;
                                    }
                                }
                            }

                            if (!isHit)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    isHit = false;
                }
            }
            else
            {
                if (_useLambda)
                {
                    if (!ExecuteLambdaExpression(_body[0], post))
                    {
                        isHit = false;
                    }
                }
                else
                {
                    foreach (string fs in _body)
                    {
                        if (UseRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption)))
                            {
                                isHit = false;
                            }
                        }
                        else
                        {
                            if (CaseSensitive)
                            {
                                if (!(post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || bodyText.Contains(fs)))
                                {
                                    isHit = false;
                                }
                            }
                            else
                            {
                                if (!(post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || bodyText.ToLower().Contains(fs.ToLower())))
                                {
                                    isHit = false;
                                }
                            }
                        }

                        if (!isHit)
                        {
                            break;
                        }
                    }
                }
            }

            if (IsRt)
            {
                if (!post.IsRetweeted)
                {
                    isHit = false;
                }
            }

            if (!string.IsNullOrEmpty(Source))
            {
                if (UseRegex)
                {
                    if (!Regex.IsMatch(sourceText, Source, regexOption))
                    {
                        isHit = false;
                    }
                }
                else
                {
                    if (!sourceText.Equals(Source, compOpt))
                    {
                        isHit = false;
                    }
                }
            }

            if (!isHit)
            {
                return HITRESULT.None;
            }

            // 除外判定
            if (ExSearchUrl)
            {
                bodyText = post.Text;
                sourceText = post.SourceHtml;
            }
            else
            {
                bodyText = post.TextFromApi;
                sourceText = post.Source;
            }

            bool isExclude = false;
            if (!string.IsNullOrEmpty(ExNameFilter) || _exbody.Count > 0)
            {
                if (ExCaseSensitive)
                {
                    compOpt = StringComparison.Ordinal;
                    regexOption = RegexOptions.None;
                }
                else
                {
                    compOpt = StringComparison.OrdinalIgnoreCase;
                    regexOption = RegexOptions.IgnoreCase;
                }

                if (ExSearchBoth)
                {
                    if (string.IsNullOrEmpty(ExNameFilter)
                        || (!ExUseRegex && (post.ScreenName.Equals(ExNameFilter, compOpt)
                        || post.RetweetedBy.Equals(ExNameFilter, compOpt)))
                        || (ExUseRegex && (Regex.IsMatch(post.ScreenName, ExNameFilter, regexOption)
                        || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, ExNameFilter, regexOption)))))
                    {
                        if (_exbody.Count > 0)
                        {
                            if (_exuseLambda)
                            {
                                if (ExecuteExLambdaExpression(_exbody[0], post))
                                {
                                    isExclude = true;
                                }
                            }
                            else
                            {
                                foreach (string fs in _exbody)
                                {
                                    if (ExUseRegex)
                                    {
                                        if (Regex.IsMatch(bodyText, fs, regexOption))
                                        {
                                            isExclude = true;
                                        }
                                    }
                                    else
                                    {
                                        if (ExCaseSensitive)
                                        {
                                            if (bodyText.Contains(fs))
                                            {
                                                isExclude = true;
                                            }
                                        }
                                        else
                                        {
                                            if (bodyText.ToLower().Contains(fs.ToLower()))
                                            {
                                                isExclude = true;
                                            }
                                        }
                                    }

                                    if (isExclude)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            isExclude = true;
                        }
                    }
                }
                else
                {
                    if (_exuseLambda)
                    {
                        if (ExecuteExLambdaExpression(_exbody[0], post))
                        {
                            isExclude = true;
                        }
                    }
                    else
                    {
                        foreach (string fs in _exbody)
                        {
                            if (ExUseRegex)
                            {
                                if (Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isExclude = true;
                                }
                            }
                            else
                            {
                                if (ExCaseSensitive)
                                {
                                    if (post.ScreenName.Contains(fs) || post.RetweetedBy.Contains(fs) || bodyText.Contains(fs))
                                    {
                                        isExclude = true;
                                    }
                                }
                                else
                                {
                                    if (post.ScreenName.ToLower().Contains(fs.ToLower()) || post.RetweetedBy.ToLower().Contains(fs.ToLower()) || bodyText.ToLower().Contains(fs.ToLower()))
                                    {
                                        isExclude = true;
                                    }
                                }
                            }

                            if (isExclude)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (IsExRt)
            {
                if (post.IsRetweeted)
                {
                    isExclude = true;
                }
            }

            if (!string.IsNullOrEmpty(ExSource))
            {
                if (ExUseRegex)
                {
                    if (Regex.IsMatch(sourceText, ExSource, regexOption))
                    {
                        isExclude = true;
                    }
                }
                else
                {
                    if (sourceText.Equals(ExSource, compOpt))
                    {
                        isExclude = true;
                    }
                }
            }

            if (string.IsNullOrEmpty(NameFilter) && _body.Count == 0 && !IsRt && string.IsNullOrEmpty(Source))
            {
                isHit = false;
            }

            if (isHit)
            {
                if (isExclude)
                {
                    return HITRESULT.Exclude;
                }

                if (MoveFrom)
                {
                    return HITRESULT.Move;
                }

                return SetMark ? HITRESULT.CopyAndMark : HITRESULT.Copy;
            }

            return isExclude ? HITRESULT.Exclude : HITRESULT.None;
        }

        public bool Equals(FiltersClass other)
        {
            if (BodyFilter.Count != other.BodyFilter.Count)
            {
                return false;
            }

            if (ExBodyFilter.Count != other.ExBodyFilter.Count)
            {
                return false;
            }

            for (int i = 0; i < BodyFilter.Count; i++)
            {
                if (BodyFilter[i] != other.BodyFilter[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < ExBodyFilter.Count; i++)
            {
                if (ExBodyFilter[i] != other.ExBodyFilter[i])
                {
                    return false;
                }
            }

            return (MoveFrom == other.MoveFrom)
                && (SetMark == other.SetMark)
                && (NameFilter == other.NameFilter)
                && (SearchBoth == other.SearchBoth)
                && (SearchUrl == other.SearchUrl)
                && (UseRegex == other.UseRegex)
                && (ExNameFilter == other.ExNameFilter)
                && (ExSearchBoth == other.ExSearchBoth)
                && (ExSearchUrl == other.ExSearchUrl)
                && (ExUseRegex == other.ExUseRegex)
                && (IsRt == other.IsRt)
                && (Source == other.Source)
                && (IsExRt == other.IsExRt)
                && (ExSource == other.ExSource)
                && (UseLambda == other.UseLambda)
                && (ExUseLambda == other.ExUseLambda);
        }

        public FiltersClass CopyTo(FiltersClass destination)
        {
            if (BodyFilter.Count > 0)
            {
                foreach (string flt in BodyFilter)
                {
                    destination.BodyFilter.Add(string.Copy(flt));
                }
            }

            if (ExBodyFilter.Count > 0)
            {
                foreach (string flt in ExBodyFilter)
                {
                    destination.ExBodyFilter.Add(string.Copy(flt));
                }
            }

            destination.MoveFrom = MoveFrom;
            destination.SetMark = SetMark;
            destination.NameFilter = NameFilter;
            destination.SearchBoth = SearchBoth;
            destination.SearchUrl = SearchUrl;
            destination.UseRegex = UseRegex;
            destination.ExNameFilter = ExNameFilter;
            destination.ExSearchBoth = ExSearchBoth;
            destination.ExSearchUrl = ExSearchUrl;
            destination.ExUseRegex = ExUseRegex;
            destination.IsRt = IsRt;
            destination.Source = Source;
            destination.IsExRt = IsExRt;
            destination.ExSource = ExSource;
            destination.UseLambda = UseLambda;
            destination.ExUseLambda = ExUseLambda;
            return destination;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!ReferenceEquals(GetType(), obj.GetType())))
            {
                return false;
            }

            return Equals((FiltersClass)obj);
        }

        public override int GetHashCode()
        {
            return MoveFrom.GetHashCode() ^ SetMark.GetHashCode()
                ^ BodyFilter.GetHashCode() ^ NameFilter.GetHashCode()
                ^ SearchBoth.GetHashCode() ^ SearchUrl.GetHashCode()
                ^ UseRegex.GetHashCode() ^ ExBodyFilter.GetHashCode()
                ^ ExNameFilter.GetHashCode() ^ ExSearchBoth.GetHashCode()
                ^ ExSearchUrl.GetHashCode() ^ ExUseRegex.GetHashCode()
                ^ IsRt.GetHashCode() ^ Source.GetHashCode()
                ^ IsExRt.GetHashCode() ^ ExSource.GetHashCode()
                ^ UseLambda.GetHashCode() ^ ExUseLambda.GetHashCode();
        }

        /// <summary>
        /// フィルタ一覧に表示する文言生成
        /// </summary>
        /// <returns></returns>
        private string MakeSummary()
        {
            StringBuilder fs = new StringBuilder();
            if (!string.IsNullOrEmpty(NameFilter) || _body.Count > 0 || IsRt || !string.IsNullOrEmpty(Source))
            {
                if (SearchBoth)
                {
                    if (!string.IsNullOrEmpty(NameFilter))
                    {
                        fs.AppendFormat(R.SetFiltersText1, NameFilter);
                    }
                    else
                    {
                        fs.Append(R.SetFiltersText2);
                    }
                }

                if (_body.Count > 0)
                {
                    fs.Append(R.SetFiltersText3);
                    foreach (string bf in _body)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }

                    fs.Length -= 1;
                    fs.Append(R.SetFiltersText4);
                }

                fs.Append("(");
                if (SearchBoth)
                {
                    fs.Append(R.SetFiltersText5);
                }
                else
                {
                    fs.Append(R.SetFiltersText6);
                }

                if (UseRegex)
                {
                    fs.Append(R.SetFiltersText7);
                }

                if (SearchUrl)
                {
                    fs.Append(R.SetFiltersText8);
                }

                if (CaseSensitive)
                {
                    fs.Append(R.SetFiltersText13);
                }

                if (IsRt)
                {
                    fs.Append("RT/");
                }

                if (_useLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(Source))
                {
                    fs.AppendFormat("Src…{0}/", Source);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            if (!string.IsNullOrEmpty(ExNameFilter) || _exbody.Count > 0 || IsExRt || !string.IsNullOrEmpty(ExSource))
            {
                // 除外
                fs.Append(R.SetFiltersText12);
                if (ExSearchBoth)
                {
                    if (!string.IsNullOrEmpty(ExNameFilter))
                    {
                        fs.AppendFormat(R.SetFiltersText1, ExNameFilter);
                    }
                    else
                    {
                        fs.Append(R.SetFiltersText2);
                    }
                }

                if (_exbody.Count > 0)
                {
                    fs.Append(R.SetFiltersText3);
                    foreach (string bf in _exbody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }

                    fs.Length -= 1;
                    fs.Append(R.SetFiltersText4);
                }

                fs.Append("(");
                if (ExSearchBoth)
                {
                    fs.Append(R.SetFiltersText5);
                }
                else
                {
                    fs.Append(R.SetFiltersText6);
                }

                if (ExUseRegex)
                {
                    fs.Append(R.SetFiltersText7);
                }

                if (ExSearchUrl)
                {
                    fs.Append(R.SetFiltersText8);
                }

                if (ExCaseSensitive)
                {
                    fs.Append(R.SetFiltersText13);
                }

                if (IsExRt)
                {
                    fs.Append("RT/");
                }

                if (_exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(ExSource))
                {
                    fs.AppendFormat("Src…{0}/", ExSource);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            fs.Append("(");
            if (MoveFrom)
            {
                fs.Append(R.SetFiltersText9);
            }
            else
            {
                fs.Append(R.SetFiltersText11);
            }

            if (!MoveFrom && SetMark)
            {
                fs.Append(R.SetFiltersText10);
            }
            else if (!MoveFrom)
            {
                fs.Length -= 1;
            }

            fs.Append(")");
            return fs.ToString();
        }

        private void ClearLambdaExp()
        {
            _lambdaExp = null;
            _lambdaExpDelegate = null;
        }

        private void ClearExLambdaExp()
        {
            _exlambdaExp = null;
            _exlambdaExpDelegate = null;
        }
    }
}