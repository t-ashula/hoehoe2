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
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using DataModels.Twitter;

    public class ListElement
    {
        private List<UserInfo> _members;
        private long _cursor = -1;

        public ListElement()
        {
            Nickname = string.Empty;
            Username = string.Empty;
            UserId = 0;
            MemberCount = 0;
            SubscriberCount = 0;
            IsPublic = true;
            Slug = string.Empty;
            Description = string.Empty;
            Name = string.Empty;
            Id = 0;
        }

        public ListElement(ListElementData listElementData, Twitter tw)
            : this()
        {
            Description = listElementData.Description;
            Id = listElementData.Id;
            IsPublic = listElementData.Mode == "public";
            MemberCount = listElementData.MemberCount;
            Name = listElementData.Name;
            SubscriberCount = listElementData.SubscriberCount;
            Slug = listElementData.Slug;
            Nickname = listElementData.User.Name.Trim();
            Username = listElementData.User.ScreenName;
            UserId = listElementData.User.Id;
            Tw = tw;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public bool IsPublic { get; set; }

        // 購読者数
        public int SubscriberCount { get; set; }

        // リストメンバ数
        public int MemberCount { get; set; }

        public long UserId { get; set; }

        public string Username { get; set; }

        public string Nickname { get; set; }

        [XmlIgnore]
        public List<UserInfo> Members
        {
            get
            {
                if (_members == null)
                {
                    _members = new List<UserInfo>();
                }

                return _members;
            }
        }

        [XmlIgnore]
        public long Cursor
        {
            get { return _cursor; }
        }

        protected Twitter Tw { get; set; }

        public virtual string Refresh()
        {
            var t = this;
            return Tw.EditList(Id.ToString(), Name, !IsPublic, Description, ref t);
        }

        public string RefreshMembers()
        {
            List<UserInfo> users = new List<UserInfo>();
            _cursor = -1;
            string result = Tw.GetListMembers(Id.ToString(), users, ref _cursor);
            _members = users;
            return string.IsNullOrEmpty(result) ? ToString() : result;
        }

        public string GetMoreMembers()
        {
            string result = Tw.GetListMembers(Id.ToString(), _members, ref _cursor);
            return string.IsNullOrEmpty(result) ? ToString() : result;
        }

        public override string ToString()
        {
            return string.Format("@{0}/{1} [{2}]", Username, Name, IsPublic ? "Public" : "Protected");
        }
    }
}