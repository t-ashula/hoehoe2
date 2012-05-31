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
        private List<string> body;
        private List<string> exbody;
        private bool useLambda;
        private bool exuseLambda;
        private LambdaExpression lambdaExp;
        private Delegate lambdaExpDelegate;
        private LambdaExpression exlambdaExp;
        private Delegate exlambdaExpDelegate;

        public FiltersClass()
        {
            this.NameFilter = string.Empty;
            this.ExNameFilter = string.Empty;
            this.body = new List<string>();
            this.exbody = new List<string>();
            this.SearchBoth = true;
            this.ExSearchBoth = true;
            this.SetMark = true;
            this.Source = string.Empty;
            this.ExSource = string.Empty;
        }

        public string NameFilter { get; set; }

        public string ExNameFilter { get; set; }

        [XmlIgnore]
        public List<string> BodyFilter
        {
            get
            {
                return this.body;
            }

            set
            {
                this.ClearLambdaExp();
                this.body = value;
            }
        }

        public string[] BodyFilterArray
        {
            get { return this.body.ToArray(); }
            set { this.body = new List<string>(value); }
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
                this.ClearExLambdaExp();
                this.exbody = value;
            }
        }

        public string[] ExBodyFilterArray
        {
            get { return this.exbody.ToArray(); }
            set { this.exbody = new List<string>(value); }
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
                return this.useLambda;
            }

            set
            {
                this.ClearLambdaExp();
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
                this.ClearExLambdaExp();
                this.exuseLambda = value;
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
            if (this.SearchUrl)
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
            if (this.CaseSensitive)
            {
                compOpt = StringComparison.Ordinal;
                regexOption = RegexOptions.None;
            }
            else
            {
                compOpt = StringComparison.OrdinalIgnoreCase;
                regexOption = RegexOptions.IgnoreCase;
            }

            if (this.SearchBoth)
            {
                if (string.IsNullOrEmpty(this.NameFilter) || (!this.UseRegex && (post.ScreenName.Equals(this.NameFilter, compOpt) || post.RetweetedBy.Equals(this.NameFilter, compOpt))) || (this.UseRegex && (Regex.IsMatch(post.ScreenName, this.NameFilter, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, this.NameFilter, regexOption)))))
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
                            if (this.UseRegex)
                            {
                                if (!Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isHit = false;
                                }
                            }
                            else
                            {
                                if (this.CaseSensitive)
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
                        if (this.UseRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption)))
                            {
                                isHit = false;
                            }
                        }
                        else
                        {
                            if (this.CaseSensitive)
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

            if (this.IsRt)
            {
                if (!post.IsRetweeted)
                {
                    isHit = false;
                }
            }

            if (!string.IsNullOrEmpty(this.Source))
            {
                if (this.UseRegex)
                {
                    if (!Regex.IsMatch(sourceText, this.Source, regexOption))
                    {
                        isHit = false;
                    }
                }
                else
                {
                    if (!sourceText.Equals(this.Source, compOpt))
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
            if (this.ExSearchUrl)
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
            if (!string.IsNullOrEmpty(this.ExNameFilter) || this.exbody.Count > 0)
            {
                if (this.ExCaseSensitive)
                {
                    compOpt = StringComparison.Ordinal;
                    regexOption = RegexOptions.None;
                }
                else
                {
                    compOpt = StringComparison.OrdinalIgnoreCase;
                    regexOption = RegexOptions.IgnoreCase;
                }

                if (this.ExSearchBoth)
                {
                    if (string.IsNullOrEmpty(this.ExNameFilter) 
                        || (!this.ExUseRegex && (post.ScreenName.Equals(this.ExNameFilter, compOpt) 
                        || post.RetweetedBy.Equals(this.ExNameFilter, compOpt)))
                        || (this.ExUseRegex && (Regex.IsMatch(post.ScreenName, this.ExNameFilter, regexOption) 
                        || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, this.ExNameFilter, regexOption)))))
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
                                    if (this.ExUseRegex)
                                    {
                                        if (Regex.IsMatch(bodyText, fs, regexOption))
                                        {
                                            isExclude = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.ExCaseSensitive)
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
                            if (this.ExUseRegex)
                            {
                                if (Regex.IsMatch(post.ScreenName, fs, regexOption) || (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, regexOption)) || Regex.IsMatch(bodyText, fs, regexOption))
                                {
                                    isExclude = true;
                                }
                            }
                            else
                            {
                                if (this.ExCaseSensitive)
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

            if (this.IsExRt)
            {
                if (post.IsRetweeted)
                {
                    isExclude = true;
                }
            }

            if (!string.IsNullOrEmpty(this.ExSource))
            {
                if (this.ExUseRegex)
                {
                    if (Regex.IsMatch(sourceText, this.ExSource, regexOption))
                    {
                        isExclude = true;
                    }
                }
                else
                {
                    if (sourceText.Equals(this.ExSource, compOpt))
                    {
                        isExclude = true;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.NameFilter) && this.body.Count == 0 && !this.IsRt && string.IsNullOrEmpty(this.Source))
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
                    if (this.MoveFrom)
                    {
                        return HITRESULT.Move;
                    }
                    else
                    {
                        return this.SetMark ? HITRESULT.CopyAndMark : HITRESULT.Copy;
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

        /// <summary>
        /// フィルタ一覧に表示する文言生成
        /// </summary>
        /// <returns></returns>
        private string MakeSummary()
        {
            StringBuilder fs = new StringBuilder();
            if (!string.IsNullOrEmpty(this.NameFilter) || this.body.Count > 0 || this.IsRt || !string.IsNullOrEmpty(this.Source))
            {
                if (this.SearchBoth)
                {
                    if (!string.IsNullOrEmpty(this.NameFilter))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, this.NameFilter);
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
                if (this.SearchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }

                if (this.UseRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }

                if (this.SearchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }

                if (this.CaseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }

                if (this.IsRt)
                {
                    fs.Append("RT/");
                }

                if (this.useLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(this.Source))
                {
                    fs.AppendFormat("Src…{0}/", this.Source);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            if (!string.IsNullOrEmpty(this.ExNameFilter) || this.exbody.Count > 0 || this.IsExRt || !string.IsNullOrEmpty(this.ExSource))
            {
                // 除外
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText12);
                if (this.ExSearchBoth)
                {
                    if (!string.IsNullOrEmpty(this.ExNameFilter))
                    {
                        fs.AppendFormat(Hoehoe.Properties.Resources.SetFiltersText1, this.ExNameFilter);
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
                if (this.ExSearchBoth)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText6);
                }

                if (this.ExUseRegex)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText7);
                }

                if (this.ExSearchUrl)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText8);
                }

                if (this.ExCaseSensitive)
                {
                    fs.Append(Hoehoe.Properties.Resources.SetFiltersText13);
                }

                if (this.IsExRt)
                {
                    fs.Append("RT/");
                }

                if (this.exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }

                if (!string.IsNullOrEmpty(this.ExSource))
                {
                    fs.AppendFormat("Src…{0}/", this.ExSource);
                }

                fs.Length -= 1;
                fs.Append(")");
            }

            fs.Append("(");
            if (this.MoveFrom)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText11);
            }

            if (!this.MoveFrom && this.SetMark)
            {
                fs.Append(Hoehoe.Properties.Resources.SetFiltersText10);
            }
            else if (!this.MoveFrom)
            {
                fs.Length -= 1;
            }

            fs.Append(")");
            return fs.ToString();
        }

        private void ClearLambdaExp()
        {
            this.lambdaExp = null;
            this.lambdaExpDelegate = null;
        }
        
        private void ClearExLambdaExp()
        {
            this.exlambdaExp = null;
            this.exlambdaExpDelegate = null;
        }
    }
}