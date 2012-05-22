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

using System;
using System.Collections.Generic;

namespace Hoehoe
{
    public sealed class PostClass : ICloneable
    {
        public class StatusGeo
        {
            public double Lng { get; set; }

            public double Lat { get; set; }
        }

        private bool _isFav;
        private bool _isProtect;
        private bool _isMark;
        private long _inReplyToStatusId;
        private States _states = States.None;
        private bool _IsDeleted;
        private StatusGeo _postGeo = new StatusGeo();

        public int RetweetedCount { get; set; }

        public long RetweetedByUserId { get; set; }

        public Dictionary<string, string> Media { get; set; }

        [Flags]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8
        }

        public PostClass(string nickname, string textFromApi, string text, string imageUrl, string screenName, DateTime createdAt, long statusId,
            bool isFav, bool isRead, bool isReply, bool isExcludeReply, bool isProtect, bool isOwl, bool isMark,
            string inReplyToUser, long inReplyToStatusId, string source, string sourceHtml,
            List<string> replyToList, bool isMe, bool isDm, long userId, bool filterHit, string retweetedBy, long retweetedId, StatusGeo geo)
        {
            Nickname = nickname;
            TextFromApi = textFromApi;
            ImageUrl = imageUrl;
            ScreenName = screenName;
            CreatedAt = createdAt;
            StatusId = statusId;
            _isFav = isFav;
            Text = text;
            IsRead = isRead;
            IsReply = isReply;
            IsExcludeReply = isExcludeReply;
            _isProtect = isProtect;
            IsOwl = isOwl;
            _isMark = isMark;
            InReplyToUser = inReplyToUser;
            _inReplyToStatusId = inReplyToStatusId;
            Source = source;
            SourceHtml = sourceHtml;
            ReplyToList = replyToList;
            IsMe = isMe;
            IsDm = isDm;
            UserId = userId;
            FilterHit = filterHit;
            RetweetedBy = retweetedBy;
            RetweetedId = retweetedId;
            _postGeo = geo;
            RelTabName = "";
            InReplyToUserId = 0;
        }

        public PostClass()
        {
            RelTabName = "";
            InReplyToUserId = 0;
            RetweetedId = 0;
            RetweetedBy = "";
            ReplyToList = new List<string>();
        }

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
                if (RetweetedId > 0 && TabInformations.GetInstance().RetweetSource(RetweetedId) != null)
                {
                    return TabInformations.GetInstance().RetweetSource(RetweetedId).IsFav;
                }
                else
                {
                    return _isFav;
                }
            }
            set
            {
                _isFav = value;
                if (RetweetedId > 0 && TabInformations.GetInstance().RetweetSource(RetweetedId) != null)
                {
                    TabInformations.GetInstance().RetweetSource(RetweetedId).IsFav = value;
                }
            }
        }

        public string Text { get; set; }

        public bool IsRead { get; set; }

        public bool IsReply { get; set; }

        public bool IsExcludeReply { get; set; }

        public bool IsProtect
        {
            get { return _isProtect; }
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
            get { return _isMark; }
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
            get { return _inReplyToStatusId; }
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
            get { return _IsDeleted; }
            set
            {
                if (value)
                {
                    this.InReplyToStatusId = 0;
                    this.InReplyToUser = "";
                    this.InReplyToUserId = 0;
                    this.IsReply = false;
                    this.ReplyToList = new List<string>();
                    this._states = States.None;
                }
                _IsDeleted = value;
            }
        }

        public int FavoritedCount { get; set; }

        public StatusGeo PostGeo
        {
            get { return _postGeo; }
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
            return (this.Nickname == other.Nickname) && (this.TextFromApi == other.TextFromApi) && (this.ImageUrl == other.ImageUrl) && (this.ScreenName == other.ScreenName) && (this.CreatedAt == other.CreatedAt) && (this.StatusId == other.StatusId) && (this.IsFav == other.IsFav) && (this.Text == other.Text) && (this.IsRead == other.IsRead) && (this.IsReply == other.IsReply) && (this.IsExcludeReply == other.IsExcludeReply) && (this.IsProtect == other.IsProtect) && (this.IsOwl == other.IsOwl) && (this.IsMark == other.IsMark) && (this.InReplyToUser == other.InReplyToUser) && (this.InReplyToStatusId == other.InReplyToStatusId) && (this.Source == other.Source) && (this.SourceHtml == other.SourceHtml) && (this.ReplyToList.Equals(other.ReplyToList)) && (this.IsMe == other.IsMe) && (this.IsDm == other.IsDm) && (this.UserId == other.UserId) && (this.FilterHit == other.FilterHit) && (this.RetweetedBy == other.RetweetedBy) && (this.RetweetedId == other.RetweetedId) && (this.RelTabName == other.RelTabName) && (this.IsDeleted == other.IsDeleted) && (this.InReplyToUserId == other.InReplyToUserId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region "IClonable.Clone"

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion "IClonable.Clone"
    }
}