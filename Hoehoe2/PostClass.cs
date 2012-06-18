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

        private bool isFav;
        private bool isProtect;
        private bool isMark;
        private long inReplyToStatusId;
        private States states = States.None;
        private bool isDeleted;
        private StatusGeo postGeo;

        #endregion private

        #region constructor

        public PostClass()
        {
            this.RelTabName = string.Empty;
            this.InReplyToUserId = 0;
            this.RetweetedId = 0;
            this.RetweetedBy = string.Empty;
            this.ReplyToList = new List<string>();
            this.postGeo = new StatusGeo();
            this.Media = new Dictionary<string, string>();
        }

        public PostClass(string nickname, string textFromApi, string text, string imageUrl, string screenName, DateTime createdAt, long statusId, bool isFav, bool isRead, bool isReply, bool isExcludeReply, bool isProtect, bool isOwl, bool isMark, string inReplyToUser, long inReplyToStatusId, string source, string sourceHtml, List<string> replyToList, bool isMe, bool isDm, long userId, bool filterHit, string retweetedBy, long retweetedId, StatusGeo geo)
            : this()
        {
            this.Nickname = nickname;
            this.TextFromApi = textFromApi;
            this.ImageUrl = imageUrl;
            this.ScreenName = screenName;
            this.CreatedAt = createdAt;
            this.StatusId = statusId;
            this.isFav = isFav;
            this.Text = text;
            this.IsRead = isRead;
            this.IsReply = isReply;
            this.IsExcludeReply = isExcludeReply;
            this.isProtect = isProtect;
            this.IsOwl = isOwl;
            this.isMark = isMark;
            this.InReplyToUser = inReplyToUser;
            this.inReplyToStatusId = inReplyToStatusId;
            this.Source = source;
            this.SourceHtml = sourceHtml;
            this.ReplyToList = replyToList;
            this.IsMe = isMe;
            this.IsDm = isDm;
            this.UserId = userId;
            this.FilterHit = filterHit;
            this.RetweetedBy = retweetedBy;
            this.RetweetedId = retweetedId;
            this.postGeo = geo;
            this.RelTabName = string.Empty;
            this.InReplyToUserId = 0;
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
                string name = this.ImageUrl;
                return name.Remove(name.LastIndexOf("_normal"), 7);
            }
        }

        public string ScreenName { get; set; }

        public System.DateTime CreatedAt { get; set; }

        public long StatusId { get; set; }

        public bool IsFav
        {
            get
            {
                if (!this.IsRetweeted)
                {
                    return this.isFav;
                }

                var post = TabInformations.GetInstance().RetweetSource(this.RetweetedId);
                return post != null ? post.IsFav : this.isFav;
            }

            set
            {
                this.isFav = value;
                if (this.IsRetweeted)
                {
                    var post = TabInformations.GetInstance().RetweetSource(this.RetweetedId);
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
                return this.isProtect;
            }

            set
            {
                if (value)
                {
                    this.states = this.states | States.Protect;
                }
                else
                {
                    this.states = this.states & ~States.Protect;
                }

                this.isProtect = value;
            }
        }

        public bool IsOwl { get; set; }

        public bool IsMark
        {
            get
            {
                return this.isMark;
            }

            set
            {
                if (value)
                {
                    this.states = this.states | States.Mark;
                }
                else
                {
                    this.states = this.states & ~States.Mark;
                }

                this.isMark = value;
            }
        }

        public string InReplyToUser { get; set; }

        public long InReplyToStatusId
        {
            get
            {
                return this.inReplyToStatusId;
            }

            set
            {
                if (value > 0)
                {
                    this.states = this.states | States.Reply;
                }
                else
                {
                    this.states = this.states & ~States.Reply;
                }

                this.inReplyToStatusId = value;
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
                return this.isDeleted;
            }

            set
            {
                if (value)
                {
                    this.InReplyToStatusId = 0;
                    this.InReplyToUser = string.Empty;
                    this.InReplyToUserId = 0;
                    this.IsReply = false;
                    this.ReplyToList = new List<string>();
                    this.states = States.None;
                }

                this.isDeleted = value;
            }
        }

        public int FavoritedCount { get; set; }

        public StatusGeo PostGeo
        {
            get
            {
                return this.postGeo;
            }

            set
            {
                if (value != null && (value.Lat != 0 || value.Lng != 0))
                {
                    this.states = this.states | States.Geo;
                }
                else
                {
                    this.states = this.states & ~States.Geo;
                }

                this.postGeo = value;
            }
        }

        public int StateIndex
        {
            get { return Convert.ToInt32(this.states) - 1; }
        }

        /// <summary>
        /// return this.RetweetedId > 0 ? this.RetweetedId : this.StatusId;
        /// </summary>
        /// <returns></returns>
        public long OriginalStatusId
        {
            get { return this.IsRetweeted ? this.RetweetedId : this.StatusId; }
        }

        public bool IsRetweeted
        {
            get { return this.RetweetedId != 0; }
        }

        #endregion properties

        #region public methods

        public PostClass Copy()
        {
            PostClass post = (PostClass)this.Clone();
            post.ReplyToList = new List<string>(this.ReplyToList);
            return post;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }

            return this.Equals((PostClass)obj);
        }

        public bool Equals(PostClass other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Nickname == other.Nickname
                && this.TextFromApi == other.TextFromApi && this.ImageUrl == other.ImageUrl
                && this.ScreenName == other.ScreenName && this.CreatedAt == other.CreatedAt
                && this.StatusId == other.StatusId && this.IsFav == other.IsFav
                && this.Text == other.Text && this.IsRead == other.IsRead
                && this.IsReply == other.IsReply && this.IsExcludeReply == other.IsExcludeReply
                && this.IsProtect == other.IsProtect && this.IsOwl == other.IsOwl
                && this.IsMark == other.IsMark && this.InReplyToUser == other.InReplyToUser
                && this.InReplyToStatusId == other.InReplyToStatusId && this.Source == other.Source
                && this.SourceHtml == other.SourceHtml && this.ReplyToList.Equals(other.ReplyToList)
                && this.IsMe == other.IsMe && this.IsDm == other.IsDm
                && this.UserId == other.UserId && this.FilterHit == other.FilterHit
                && this.RetweetedBy == other.RetweetedBy && this.RetweetedId == other.RetweetedId
                && this.RelTabName == other.RelTabName && this.IsDeleted == other.IsDeleted && this.InReplyToUserId == other.InReplyToUserId;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetDump()
        {
            StringBuilder sb = new StringBuilder(512);
            var format1 = "{0,-20}:{1}<br/>";
            var format2 = "{0,-20}:<xmp>{1}</xmp><br/>";
            sb.Append("-----Start PostClass Dump<br>");
            sb.AppendFormat(format1, "TextFromApi", this.TextFromApi);
            sb.AppendFormat(format2, "(PlainText)", this.TextFromApi);
            sb.AppendFormat(format1, "StatusId", this.StatusId);
            sb.AppendFormat(format1, "ImageUrl", this.ImageUrl);
            sb.AppendFormat(format1, "InReplyToStatusId", this.InReplyToStatusId);
            sb.AppendFormat(format1, "InReplyToUser", this.InReplyToUser);
            sb.AppendFormat(format1, "IsDM", this.IsDm);
            sb.AppendFormat(format1, "IsFav", this.IsFav);
            sb.AppendFormat(format1, "IsMark", this.IsMark);
            sb.AppendFormat(format1, "IsMe", this.IsMe);
            sb.AppendFormat(format1, "IsOwl", this.IsOwl);
            sb.AppendFormat(format1, "IsProtect", this.IsProtect);
            sb.AppendFormat(format1, "IsRead", this.IsRead);
            sb.AppendFormat(format1, "IsReply", this.IsReply);
            foreach (var nm in this.ReplyToList)
            {
                sb.AppendFormat(format1, "ReplyToList", nm);
            }

            sb.AppendFormat(format1, "ScreenName", this.ScreenName);
            sb.AppendFormat(format1, "NickName", this.Nickname);
            sb.AppendFormat(format1, "Text", this.Text);
            sb.AppendFormat(format2, "(PlainText)", this.Text);
            sb.AppendFormat(format1, "CreatedAt", this.CreatedAt);
            sb.AppendFormat(format1, "Source", this.Source);
            sb.AppendFormat(format1, "UserId", this.UserId);
            sb.AppendFormat(format1, "FilterHit", this.FilterHit);
            sb.AppendFormat(format1, "RetweetedBy", this.RetweetedBy);
            sb.AppendFormat(format1, "RetweetedId", this.RetweetedId);
            sb.AppendFormat(format1, "SearchTabName", this.RelTabName);
            sb.Append("-----End PostClass Dump<br>");
            return sb.ToString();
        }

        public bool IsMatch(string word, StringComparison opt)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if (this.Nickname.IndexOf(word, opt) > -1 || this.TextFromApi.IndexOf(word, opt) > -1 || this.ScreenName.IndexOf(word, opt) > -1)
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
                if (regex.IsMatch(this.Nickname) || regex.IsMatch(this.TextFromApi) || regex.IsMatch(this.ScreenName))
                {
                    return true;
                }
            }

            return false;
        }

        public string MakeStatusUrl()
        {
            if (this.IsDm || this.IsDeleted)
            {
                return string.Empty;
            }

            return string.Format("https://twitter.com/{0}/status/{1}", this.ScreenName, this.OriginalStatusId);
        }

        public string MakeTsvLine()
        {
            return string.Format(
                "{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                this.Nickname,
                this.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""),
                this.CreatedAt,
                this.ScreenName,
                this.StatusId,
                this.ImageUrl,
                this.Text.Replace("\n", string.Empty).Replace("\"", "\"\""),
                this.IsProtect ? "Protect" : string.Empty);
        }

        public string MakeReplyPostInfoLine()
        {
            return string.Format("{0} / {1}   ({2}){3}{4}", this.ScreenName, this.Nickname, this.CreatedAt, Environment.NewLine, this.TextFromApi);
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