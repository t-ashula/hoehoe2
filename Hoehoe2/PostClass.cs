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
    using System.Text;

    public sealed class PostClass : ICloneable
    {
        #region private

        private bool _isFav;
        private bool _isProtect;
        private bool _isMark;
        private long _inReplyToStatusId;
        private States _states = States.None;
        private bool _isDeleted;
        private StatusGeo _postGeo;

        #endregion private

        #region constructor

        public PostClass()
        {
            RelTabName = string.Empty;
            InReplyToUserId = 0;
            RetweetedId = 0;
            RetweetedBy = string.Empty;
            ReplyToList = new List<string>();
            _postGeo = new StatusGeo();
            Media = new Dictionary<string, string>();
        }

        #endregion constructor

        #region enumes

        [Flags]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8
        }

        #endregion enumes

        #region properties

        public int RetweetedCount { get; set; }

        public long RetweetedByUserId { get; set; }

        public Dictionary<string, string> Media { get; set; }

        public string Nickname { get; set; }

        public string TextFromApi { get; set; }

        public string ImageUrl { get; set; }

        public string NormalImageUrl
        {
            get
            {
                string name = ImageUrl;
                return name.Remove(name.LastIndexOf("_normal"), 7);
            }
        }

        public string ScreenName { get; set; }

        public DateTime CreatedAt { get; set; }

        public long StatusId { get; set; }

        public bool IsFav
        {
            get
            {
                if (!IsRetweeted)
                {
                    return _isFav;
                }

                var post = TabInformations.Instance.RetweetSource(RetweetedId);
                return post != null ? post.IsFav : _isFav;
            }

            set
            {
                _isFav = value;
                if (IsRetweeted)
                {
                    var post = TabInformations.Instance.RetweetSource(RetweetedId);
                    if (post != null)
                    {
                        post.IsFav = value;
                    }
                }
            }
        }

        public string Text { get; set; }

        public bool IsRead { get; set; }

        public bool IsReply { get; set; }

        public bool IsExcludeReply { get; set; }

        public bool IsProtect
        {
            get
            {
                return _isProtect;
            }

            set
            {
                if (value)
                {
                    _states = _states | States.Protect;
                }
                else
                {
                    _states = _states & ~States.Protect;
                }

                _isProtect = value;
            }
        }

        public bool IsOwl { get; set; }

        public bool IsMark
        {
            get
            {
                return _isMark;
            }

            set
            {
                if (value)
                {
                    _states = _states | States.Mark;
                }
                else
                {
                    _states = _states & ~States.Mark;
                }

                _isMark = value;
            }
        }

        public string InReplyToUser { get; set; }

        public long InReplyToStatusId
        {
            get
            {
                return _inReplyToStatusId;
            }

            set
            {
                if (value > 0)
                {
                    _states = _states | States.Reply;
                }
                else
                {
                    _states = _states & ~States.Reply;
                }

                _inReplyToStatusId = value;
            }
        }

        public long InReplyToUserId { get; set; }

        public string Source { get; set; }

        public string SourceHtml { get; set; }

        public List<string> ReplyToList { get; set; }

        public bool IsMe { get; set; }

        public bool IsDm { get; set; }

        public long UserId { get; set; }

        public bool FilterHit { get; set; }

        public string RetweetedBy { get; set; }

        public long RetweetedId { get; set; }

        public string RelTabName { get; set; }

        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }

            set
            {
                if (value)
                {
                    InReplyToStatusId = 0;
                    InReplyToUser = string.Empty;
                    InReplyToUserId = 0;
                    IsReply = false;
                    ReplyToList = new List<string>();
                    _states = States.None;
                }

                _isDeleted = value;
            }
        }

        public int FavoritedCount { get; set; }

        public StatusGeo PostGeo
        {
            get
            {
                return _postGeo;
            }

            set
            {
                if (value != null && (value.Lat != 0 || value.Lng != 0))
                {
                    _states = _states | States.Geo;
                }
                else
                {
                    _states = _states & ~States.Geo;
                }

                _postGeo = value;
            }
        }

        public int StateIndex
        {
            get { return Convert.ToInt32(_states) - 1; }
        }

        /// <summary>
        /// return RetweetedId > 0 ? RetweetedId : StatusId;
        /// </summary>
        /// <returns></returns>
        public long OriginalStatusId
        {
            get { return IsRetweeted ? RetweetedId : StatusId; }
        }

        public bool IsRetweeted
        {
            get { return RetweetedId != 0; }
        }

        #endregion properties

        #region public methods

        public PostClass Copy()
        {
            var post = (PostClass)Clone();
            post.ReplyToList = new List<string>(ReplyToList);
            return post;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!ReferenceEquals(GetType(), obj.GetType())))
            {
                return false;
            }

            return Equals((PostClass)obj);
        }

        public bool Equals(PostClass other)
        {
            if (other == null)
            {
                return false;
            }

            return Nickname == other.Nickname
                && TextFromApi == other.TextFromApi && ImageUrl == other.ImageUrl
                && ScreenName == other.ScreenName && CreatedAt == other.CreatedAt
                && StatusId == other.StatusId && IsFav == other.IsFav
                && Text == other.Text && IsRead == other.IsRead
                && IsReply == other.IsReply && IsExcludeReply == other.IsExcludeReply
                && IsProtect == other.IsProtect && IsOwl == other.IsOwl
                && IsMark == other.IsMark && InReplyToUser == other.InReplyToUser
                && InReplyToStatusId == other.InReplyToStatusId && Source == other.Source
                && SourceHtml == other.SourceHtml && ReplyToList.Equals(other.ReplyToList)
                && IsMe == other.IsMe && IsDm == other.IsDm
                && UserId == other.UserId && FilterHit == other.FilterHit
                && RetweetedBy == other.RetweetedBy && RetweetedId == other.RetweetedId
                && RelTabName == other.RelTabName && IsDeleted == other.IsDeleted && InReplyToUserId == other.InReplyToUserId;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string GetDump()
        {
            var sb = new StringBuilder(512);
            const string format1 = "{0,-20}:{1}<br/>";
            const string format2 = "{0,-20}:<xmp>{1}</xmp><br/>";
            sb.Append("-----Start PostClass Dump<br>");
            sb.AppendFormat(format1, "TextFromApi", TextFromApi);
            sb.AppendFormat(format2, "(PlainText)", TextFromApi);
            sb.AppendFormat(format1, "StatusId", StatusId);
            sb.AppendFormat(format1, "ImageUrl", ImageUrl);
            sb.AppendFormat(format1, "InReplyToStatusId", InReplyToStatusId);
            sb.AppendFormat(format1, "InReplyToUser", InReplyToUser);
            sb.AppendFormat(format1, "IsDM", IsDm);
            sb.AppendFormat(format1, "IsFav", IsFav);
            sb.AppendFormat(format1, "IsMark", IsMark);
            sb.AppendFormat(format1, "IsMe", IsMe);
            sb.AppendFormat(format1, "IsOwl", IsOwl);
            sb.AppendFormat(format1, "IsProtect", IsProtect);
            sb.AppendFormat(format1, "IsRead", IsRead);
            sb.AppendFormat(format1, "IsReply", IsReply);
            foreach (var nm in ReplyToList)
            {
                sb.AppendFormat(format1, "ReplyToList", nm);
            }

            sb.AppendFormat(format1, "ScreenName", ScreenName);
            sb.AppendFormat(format1, "NickName", Nickname);
            sb.AppendFormat(format1, "Text", Text);
            sb.AppendFormat(format2, "(PlainText)", Text);
            sb.AppendFormat(format1, "CreatedAt", CreatedAt);
            sb.AppendFormat(format1, "Source", Source);
            sb.AppendFormat(format1, "UserId", UserId);
            sb.AppendFormat(format1, "FilterHit", FilterHit);
            sb.AppendFormat(format1, "RetweetedBy", RetweetedBy);
            sb.AppendFormat(format1, "RetweetedId", RetweetedId);
            sb.AppendFormat(format1, "SearchTabName", RelTabName);
            sb.Append("-----End PostClass Dump<br>");
            return sb.ToString();
        }

        public bool IsMatch(string word, StringComparison opt)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if (Nickname.IndexOf(word, opt) > -1 || TextFromApi.IndexOf(word, opt) > -1 || ScreenName.IndexOf(word, opt) > -1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsMatch(System.Text.RegularExpressions.Regex regex)
        {
            if (regex != null)
            {
                if (regex.IsMatch(Nickname) || regex.IsMatch(TextFromApi) || regex.IsMatch(ScreenName))
                {
                    return true;
                }
            }

            return false;
        }

        public string MakeStatusUrl()
        {
            if (IsDm || IsDeleted)
            {
                return string.Empty;
            }

            return string.Format("https://twitter.com/{0}/status/{1}", ScreenName, OriginalStatusId);
        }

        public string MakeTsvLine()
        {
            return string.Format(
                "{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                Nickname,
                TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""),
                CreatedAt,
                ScreenName,
                StatusId,
                ImageUrl,
                Text.Replace("\n", string.Empty).Replace("\"", "\"\""),
                IsProtect ? "Protect" : string.Empty);
        }

        public string MakeReplyPostInfoLine()
        {
            return string.Format("{0} / {1}   ({2}){3}{4}", ScreenName, Nickname, CreatedAt, Environment.NewLine, TextFromApi);
        }

        #endregion public methods

        #region inner types

        public class StatusGeo
        {
            public double Lng { get; set; }

            public double Lat { get; set; }
        }

        #endregion inner types
    }
}