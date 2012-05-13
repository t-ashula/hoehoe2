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

namespace Tween
{
    public sealed class PostClass : ICloneable
    {
        public class StatusGeo
        {
            public double Lng { get; set; }

            public double Lat { get; set; }
        }

        private string _Nick;
        private string _textFromApi;
        private string _ImageUrl;
        private string _screenName;
        private System.DateTime _createdAt;
        private long _statusId;
        private bool _IsFav;
        private string _text;
        private bool _IsRead;
        private bool _IsReply;
        private bool _IsExcludeReply;
        private bool _IsProtect;
        private bool _IsOWL;
        private bool _IsMark;
        private string _InReplyToUser;
        private long _InReplyToStatusId;
        private string _Source;
        private string _SourceHtml;
        private List<string> _ReplyToList = new List<string>();
        private bool _IsMe;
        private bool _IsDm;
        private States _states = States.None;
        private long _UserId;
        private bool _FilterHit;
        private string _RetweetedBy = "";
        private long _RetweetedId = 0;
        private string _SearchTabName = "";
        private bool _IsDeleted = false;
        private long _InReplyToUserId = 0;
        private StatusGeo _postGeo = new StatusGeo();

        public int RetweetedCount { get; set; }

        public long RetweetedByUserId { get; set; }

        public Dictionary<string, string> Media { get; set; }

        [FlagsAttribute()]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8
        }

        public PostClass(string Nickname, string textFromApi, string text, string ImageUrl, string screenName, System.DateTime createdAt, long statusId, bool IsFav, bool IsRead, bool IsReply,
        bool IsExcludeReply, bool IsProtect, bool IsOwl, bool IsMark, string InReplyToUser, long InReplyToStatusId, string Source, string SourceHtml, List<string> ReplyToList, bool IsMe,
        bool IsDm, long userId, bool FilterHit, string RetweetedBy, long RetweetedId, StatusGeo Geo)
        {
            _Nick = Nickname;
            _textFromApi = textFromApi;
            _ImageUrl = ImageUrl;
            _screenName = screenName;
            _createdAt = createdAt;
            _statusId = statusId;
            _IsFav = IsFav;
            _text = text;
            _IsRead = IsRead;
            _IsReply = IsReply;
            _IsExcludeReply = IsExcludeReply;
            _IsProtect = IsProtect;
            _IsOWL = IsOwl;
            _IsMark = IsMark;
            _InReplyToUser = InReplyToUser;
            _InReplyToStatusId = InReplyToStatusId;
            _Source = Source;
            _SourceHtml = SourceHtml;
            _ReplyToList = ReplyToList;
            _IsMe = IsMe;
            _IsDm = IsDm;
            _UserId = userId;
            _FilterHit = FilterHit;
            _RetweetedBy = RetweetedBy;
            _RetweetedId = RetweetedId;
            _postGeo = Geo;
        }

        public PostClass()
        {
        }

        public string Nickname
        {
            get { return _Nick; }
            set { _Nick = value; }
        }

        public string TextFromApi
        {
            get { return _textFromApi; }
            set { _textFromApi = value; }
        }

        public string ImageUrl
        {
            get { return _ImageUrl; }
            set { _ImageUrl = value; }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set { _screenName = value; }
        }

        public System.DateTime CreatedAt
        {
            get { return _createdAt; }
            set { _createdAt = value; }
        }

        public long StatusId
        {
            get { return _statusId; }
            set { _statusId = value; }
        }

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
                    return _IsFav;
                }
            }
            set
            {
                _IsFav = value;
                if (this.RetweetedId > 0 && TabInformations.GetInstance().RetweetSource(this.RetweetedId) != null)
                {
                    TabInformations.GetInstance().RetweetSource(this.RetweetedId).IsFav = value;
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool IsRead
        {
            get { return _IsRead; }
            set { _IsRead = value; }
        }

        public bool IsReply
        {
            get { return _IsReply; }
            set { _IsReply = value; }
        }

        public bool IsExcludeReply
        {
            get { return _IsExcludeReply; }
            set { _IsExcludeReply = value; }
        }

        public bool IsProtect
        {
            get { return _IsProtect; }
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
                _IsProtect = value;
            }
        }

        public bool IsOwl
        {
            get { return _IsOWL; }
            set { _IsOWL = value; }
        }

        public bool IsMark
        {
            get { return _IsMark; }
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
                _IsMark = value;
            }
        }

        public string InReplyToUser
        {
            get { return _InReplyToUser; }
            set { _InReplyToUser = value; }
        }

        public long InReplyToStatusId
        {
            get { return _InReplyToStatusId; }
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
                _InReplyToStatusId = value;
            }
        }

        public long InReplyToUserId
        {
            get { return _InReplyToUserId; }
            set { _InReplyToUserId = value; }
        }

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public string SourceHtml
        {
            get { return _SourceHtml; }
            set { _SourceHtml = value; }
        }

        public List<string> ReplyToList
        {
            get { return _ReplyToList; }
            set { _ReplyToList = value; }
        }

        public bool IsMe
        {
            get { return _IsMe; }
            set { _IsMe = value; }
        }

        public bool IsDm
        {
            get { return _IsDm; }
            set { _IsDm = value; }
        }

        //Public ReadOnly Property StatusIndex() As Integer
        //    Get
        //        Return _statuses
        //    End Get
        //End Property
        public long UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public bool FilterHit
        {
            get { return _FilterHit; }
            set { _FilterHit = value; }
        }

        public string RetweetedBy
        {
            get { return _RetweetedBy; }
            set { _RetweetedBy = value; }
        }

        public long RetweetedId
        {
            get { return _RetweetedId; }
            set { _RetweetedId = value; }
        }

        public string RelTabName
        {
            get { return _SearchTabName; }
            set { _SearchTabName = value; }
        }

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
                return false;
            return this.Equals((PostClass)obj);
        }

        public bool Equals(PostClass other)
        {
            if (other == null)
                return false;
            return (this.Nickname == other.Nickname) && (this.TextFromApi == other.TextFromApi) && (this.ImageUrl == other.ImageUrl) && (this.ScreenName == other.ScreenName) && (this.CreatedAt == other.CreatedAt) && (this.StatusId == other.StatusId) && (this.IsFav == other.IsFav) && (this.Text == other.Text) && (this.IsRead == other.IsRead) && (this.IsReply == other.IsReply) && (this.IsExcludeReply == other.IsExcludeReply) && (this.IsProtect == other.IsProtect) && (this.IsOwl == other.IsOwl) && (this.IsMark == other.IsMark) && (this.InReplyToUser == other.InReplyToUser) && (this.InReplyToStatusId == other.InReplyToStatusId) && (this.Source == other.Source) && (this.SourceHtml == other.SourceHtml) && (this.ReplyToList.Equals(other.ReplyToList)) && (this.IsMe == other.IsMe) && (this.IsDm == other.IsDm) && (this.UserId == other.UserId) && (this.FilterHit == other.FilterHit) && (this.RetweetedBy == other.RetweetedBy) && (this.RetweetedId == other.RetweetedId) && (this.RelTabName == other.RelTabName) && (this.IsDeleted == other.IsDeleted) && (this.InReplyToUserId == other.InReplyToUserId);
        }

        #region "IClonable.Clone"

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion "IClonable.Clone"
    }
}