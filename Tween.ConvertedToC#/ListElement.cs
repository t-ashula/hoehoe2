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
    using System.Xml.Serialization;
    using Hoehoe.DataModels;
    using Hoehoe.DataModels.Twitter;

    public class ListElement
    {
        private List<UserInfo> members;
        private long cursor = -1;

        public ListElement()
        {
            this.Nickname = string.Empty;
            this.Username = string.Empty;
            this.UserId = 0;
            this.MemberCount = 0;
            this.SubscriberCount = 0;
            this.IsPublic = true;
            this.Slug = string.Empty;
            this.Description = string.Empty;
            this.Name = string.Empty;
            this.Id = 0;
        }

        public ListElement(ListElementData listElementData, Twitter tw)
            : this()
        {
            this.Description = listElementData.Description;
            this.Id = listElementData.Id;
            this.IsPublic = listElementData.Mode == "public";
            this.MemberCount = listElementData.MemberCount;
            this.Name = listElementData.Name;
            this.SubscriberCount = listElementData.SubscriberCount;
            this.Slug = listElementData.Slug;
            this.Nickname = listElementData.User.Name.Trim();
            this.Username = listElementData.User.ScreenName;
            this.UserId = listElementData.User.Id;
            this.Tw = tw;
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
                if (this.members == null)
                {
                    this.members = new List<UserInfo>();
                }

                return this.members;
            }
        }

        [XmlIgnore]
        public long Cursor
        {
            get { return this.cursor; }
        }

        protected Twitter Tw { get; set; }

        public virtual string Refresh()
        {
            var t = this;
            return this.Tw.EditList(this.Id.ToString(), this.Name, !this.IsPublic, this.Description, ref t);
        }

        public string RefreshMembers()
        {
            List<UserInfo> users = new List<UserInfo>();
            this.cursor = -1;
            string result = this.Tw.GetListMembers(this.Id.ToString(), users, ref this.cursor);
            this.members = users;
            return string.IsNullOrEmpty(result) ? this.ToString() : result;
        }

        public string GetMoreMembers()
        {
            string result = this.Tw.GetListMembers(this.Id.ToString(), this.members, ref this.cursor);
            return string.IsNullOrEmpty(result) ? this.ToString() : result;
        }

        public override string ToString()
        {
            return string.Format("@{0}/{1} [{2}]", this.Username, this.Name, this.IsPublic ? "Public" : "Protected");
        }
    }
}