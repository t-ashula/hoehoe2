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
        }

        public PostClass(string nickname, string textFromApi, string text, string imageUrl, string screenName, DateTime createdAt, long statusId, bool isFav, bool isRead, bool isReply, bool isExcludeReply, bool isProtect, bool isOwl, bool isMark, string inReplyToUser, long inReplyToStatusId, string source, string sourceHtml, List<string> replyToList, bool isMe, bool isDm, long userId, bool filterHit, string retweetedBy, long retweetedId, StatusGeo geo)
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

        public string ScreenName { get; set; }

        public System.DateTime CreatedAt { get; set; }

        public long StatusId { get; set; }

        public bool IsFav
        {
            get
            {
                if (this.RetweetedId > 0 && TabInformations.GetInstance().RetweetSource(this.RetweetedId) != null)
                {
                    return TabInformations.GetInstance().RetweetSource(this.RetweetedId).IsFav;
                }
                else
                {
                    return this.isFav;
                }
            }

            set
            {
                this.isFav = value;
                if (this.RetweetedId > 0 && TabInformations.GetInstance().RetweetSource(this.RetweetedId) != null)
                {
                    TabInformations.GetInstance().RetweetSource(this.RetweetedId).IsFav = value;
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