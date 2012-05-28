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

    [Serializable]
    public sealed class FiltersClass : IEquatable<FiltersClass>
    {
        private string name = string.Empty;
        private List<string> body = new List<string>();
        private bool searchBoth = true;
        private bool searchUrl;
        private bool caseSensitive;
        private bool useRegex;
        private bool isRt;
        private string source = string.Empty;
        private string exname = string.Empty;
        private List<string> exbody = new List<string>();
        private bool exsearchBoth = true;
        private bool exsearchUrl;
        private bool exuseRegex;
        private bool excaseSensitive;
        private bool isExRt;
        private string exsource = string.Empty;
        private bool moveFrom;
        private bool setMark = true;
        private bool useLambda;
        private bool exuseLambda;

        private LambdaExpression lambdaExp;
        private Delegate lambdaExpDelegate;
        private LambdaExpression exlambdaExp;
        private Delegate exlambdaExpDelegate;

        public FiltersClass()
        {
        }

        public string NameFilter
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string ExNameFilter
        {
            get { return this.exname; }
            set { this.exname = value; }
        }

        [XmlIgnore]
        public List<string> BodyFilter
        {
            get
            {
                return this.body;
            }

            set
            {
                this.lambdaExp = null;
                this.lambdaExpDelegate = null;
                this.body = value;
            }
        }

        public string[] BodyFilterArray
        {
            get
            {
                return this.body.ToArray();
            }

            set
            {
                this.body = new List<string>();
                foreach (string filter in value)
                {
                    this.body.Add(filter);
                }
            }
        }

        [XmlIgnore]
        public List<string> ExBodyFilter
        {
            get
            {
                return this.exbody;
            }

            set
            {
                this.exlambdaExp = null;
                this.exlambdaExpDelegate = null;
                this.exbody = value;
            }
        }

        public string[] ExBodyFilterArray
        {
            get
            {
                return this.exbody.ToArray();
            }

            set
            {
                this.exbody = new List<string>();
                foreach (string filter in value)
                {
                    this.exbody.Add(filter);
                }
            }
        }

        public bool SearchBoth
        {
            get { return this.searchBoth; }
            set { this.searchBoth = value; }
        }

        public bool ExSearchBoth
        {
            get { return this.exsearchBoth; }
            set { this.exsearchBoth = value; }
        }

        public bool MoveFrom
        {
            get { return this.moveFrom; }
            set { this.moveFrom = value; }
        }

        public bool SetMark
        {
            get { return this.setMark; }
            set { this.setMark = value; }
        }

        public bool SearchUrl
        {
            get { return this.searchUrl; }
            set { this.searchUrl = value; }
        }

        public bool ExSearchUrl
        {
            get { return this.exsearchUrl; }
            set { this.exsearchUrl = value; }
        }

        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set { this.caseSensitive = value; }
        }

        public bool ExCaseSensitive
        {
            get { return this.excaseSensitive; }
            set { this.excaseSensitive = value; }
        }

        public bool UseLambda
        {
            get
            {
                return this.useLambda;
            }

            set
            {
                this.lambdaExp = null;
                this.lambdaExpDelegate = null;
                this.useLambda = value;
            }
        }

        public bool ExUseLambda
        {
            get
            {
                return this.exuseLambda;
            }

            set
            {
                this.exlambdaExp = null;
                this.exlambdaExpDelegate = null;
                this.exuseLambda = value;
            }
        }

        public bool UseRegex
        {
            get { return this.useRegex; }
            set { this.useRegex = value; }
        }

        public bool ExUseRegex
        {
            get { return this.exuseRegex; }
            set { this.exuseRegex = value; }
        }

        public bool IsRt
        {
            get { return this.isRt; }
            set { this.isRt = value; }
        }

        public bool IsExRt
        {
            get { return this.isExRt; }
            set { this.isExRt = value; }
        }

        public string Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public string ExSource
        {
            get { return this.exsource; }
            set { this.exsource = value; }
        }

        public override string ToString()
        {
            return this.MakeSummary();
        }

        public bool ExecuteLambdaExpression(string expr, PostClass post)
        {
            if (this.lambdaExp == null || this.lambdaExpDelegate == null)
            {
                this.lambdaExp = DynamicExpression.ParseLambda<PostClass, bool>(expr, post);
                this.lambdaExpDelegate = this.lambdaExp.Compile();
            }

            return (bool)this.lambdaExpDelegate.DynamicInvoke(post);
        }

        public bool ExecuteExLambdaExpression(string expr, PostClass post)
        {
            if (this.exlambdaExp == null || this.exlambdaExpDelegate == null)
            {
                this.exlambdaExp = DynamicExpression.ParseLambda<PostClass, bool>(expr, post);
                this.exlambdaExpDelegate = this.exlambdaExp.Compile();
            }

            return (bool)this.exlambdaExpDelegate.DynamicInvoke(post);
        }

        public HITRESULT IsHit(PostClass post)
        {
            bool isHit = true;
            string bodyText = null;
            string sourceText = null;
            if (this.searchUrl)
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
            if (this.caseSensitive)
            {
                compOpt = StringComparison.Ordinal;
                regexOption = RegexOptions.None;
            }
            else
            {
                compOpt = StringComparison.OrdinalIgnoreCase;
                regexOption = RegexOptions.IgnoreCase;
            }

            if (this.searchBoth)
            {
                if (string.IsNullOrEmpty(this.name) || (!this.useRegex && (post.ScreenName.Equals(this.name, compOpt) || post.RetweetedBy.Equals(this.name, compOpt))) || (this.useRegex && (Regex.IsMatch(post.ScreenName, this.name, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, this.name, regexOption)))))
                {
                    if (this.useLambda)
                    {
                        if (!this.ExecuteLambdaExpression(this.body[0], post))
                        {
                            isHit = false;
                        }
                    }
                    else
                    {
                        foreach (string fs in this.body)
                        {
                            if (this.useRegex)
                            {
                                if (!Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isHit = false;
                                }
                            }
                            else
                            {
                                if (this.caseSensitive)
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
                if (this.useLambda)
                {
                    if (!this.ExecuteLambdaExpression(this.body[0], post))
                    {
                        isHit = false;
                    }
                }
                else
                {
                    foreach (string fs in this.body)
                    {
                        if (this.useRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption)))
                            {
                                isHit = false;
                            }
                        }
                        else
                        {
                            if (this.caseSensitive)
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

            if (this.isRt)
            {
                if (post.RetweetedId == 0)
                {
                    isHit = false;
                }
            }

            if (!string.IsNullOrEmpty(this.source))
            {
                if (this.useRegex)
                {
                    if (!Regex.IsMatch(sourceText, this.source, regexOption))
                    {
                        isHit = false;
                    }
                }
                else
                {
                    if (!sourceText.Equals(this.source, compOpt))
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
            if (this.exsearchUrl)
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
            if (!string.IsNullOrEmpty(this.exname) || this.exbody.Count > 0)
            {
                if (this.excaseSensitive)
                {
                    compOpt = StringComparison.Ordinal;
                    regexOption = RegexOptions.None;
                }
                else
                {
                    compOpt = StringComparison.OrdinalIgnoreCase;
                    regexOption = RegexOptions.IgnoreCase;
                }

                if (this.exsearchBoth)
                {
                    if (string.IsNullOrEmpty(this.exname) || (!this.exuseRegex && (post.ScreenName.Equals(this.exname, compOpt) || post.RetweetedBy.Equals(this.exname, compOpt))) || (this.exuseRegex && (Regex.IsMatch(post.ScreenName, this.exname, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, this.exname, regexOption)))))
                    {
                        if (this.exbody.Count > 0)
                        {
                            if (this.exuseLambda)
                            {
                                if (this.ExecuteExLambdaExpression(this.exbody[0], post))
                                {
                                    isExclude = true;
                                }
                            }
                            else
                            {
                                foreach (string fs in this.exbody)
                                {
                                    if (this.exuseRegex)
                                    {
                                        if (Regex.IsMatch(bodyText, fs, regexOption))
                                        {
                                            isExclude = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.excaseSensitive)
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
                    if (this.exuseLambda)
                    {
                        if (this.ExecuteExLambdaExpression(this.exbody[0], post))
                        {
                            isExclude = true;
                        }
                    }
                    else
                    {
                        foreach (string fs in this.exbody)
                        {
                            if (this.exuseRegex)
                            {
                                if (Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isExclude = true;
                                }
                            }
                            else
                            {
                                if (this.excaseSensitive)
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

            if (this.isExRt)
            {
                if (post.RetweetedId > 0)
                {
                    isExclude = true;
                }
            }

            if (!string.IsNullOrEmpty(this.exsource))
            {
                if (this.exuseRegex)
                {
                    if (Regex.IsMatch(sourceText, this.exsource, regexOption))
                    {
                        isExclude = true;
                    }
                }
                else
                {
                    if (sourceText.Equals(this.exsource, compOpt))
                    {
                        isExclude = true;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.name) && this.body.Count == 0 && !this.isRt && string.IsNullOrEmpty(this.source))
            {
                isHit = false;
            }

            if (isHit)
            {
                if (isExclude)
                {
                    return HITRESULT.Exclude;
                }
                else
                {
                    if (this.moveFrom)
                    {
                        return HITRESULT.Move;
                    }
                    else
                    {
                        return this.setMark ? HITRESULT.CopyAndMark : HITRESULT.Copy;
                    }
                }
            }
            else
            {
                return isExclude ? HITRESULT.Exclude : HITRESULT.None;
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

        // フィルタ一覧に表示する文言生成
        private string MakeSummary()
        {
            StringBuilder fs = new StringBuilder();
            if (!string.IsNullOrEmpty(this.name) || this.body.Count > 0 || this.isRt || !string.IsNullOrEmpty(this.source))
            {
                if (this.searchBoth)
                {
                    if (!string.IsNullOrEmpty(this.name))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, this.name);
                    }
                    else
                    {
                        fs.Append(Hoehoe.Properties.Resources.SetFiltersText2);
                    }
                }

                if (this.body.Count > 0)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText3);
                    foreach (string bf in this.body)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }

                    fs.Length -= 1;
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText4);
                }

                fs.Append("(");
                if (this.searchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }

                if (this.useRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }

                if (this.searchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }

                if (this.caseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }

                if (this.isRt)
                {
                    fs.Append("RT/");
                }

                if (this.useLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(this.source))
                {
                    fs.AppendFormat("Src…{0}/", this.source);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            if (!string.IsNullOrEmpty(this.exname) || this.exbody.Count > 0 || this.isExRt || !string.IsNullOrEmpty(this.exsource))
            {
                // 除外
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText12);
                if (this.exsearchBoth)
                {
                    if (!string.IsNullOrEmpty(this.exname))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, this.exname);
                    }
                    else
                    {
                        fs.Append(Hoehoe.Properties.Resources.SetFiltersText2);
                    }
                }

                if (this.exbody.Count > 0)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText3);
                    foreach (string bf in this.exbody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }

                    fs.Length -= 1;
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText4);
                }

                fs.Append("(");
                if (this.exsearchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }

                if (this.exuseRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }

                if (this.exsearchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }

                if (this.excaseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }

                if (this.isExRt)
                {
                    fs.Append("RT/");
                }

                if (this.exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(this.exsource))
                {
                    fs.AppendFormat("Src…{0}/", this.exsource);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            fs.Append("(");
            if (this.moveFrom)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText11);
            }

            if (!this.moveFrom && this.setMark)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText10);
            }
            else if (!this.moveFrom)
            {
                fs.Length -= 1;
            }

            fs.Append(")");
            return fs.ToString();
        }
    }
}