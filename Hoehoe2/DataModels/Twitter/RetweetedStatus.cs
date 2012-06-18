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

namespace Hoehoe.DataModels.Twitter
{
    using System.Runtime.Serialization;

    [DataContract]
    public class RetweetedStatus
    {
        [DataMember(Name = "coordinates", IsRequired = false)]
        public Coordinates Coordinates;

        [DataMember(Name = "geo", IsRequired = false)]
        public Geo Geo;

        [DataMember(Name = "in_reply_to_user_id")]
        public string InReplyToUserId;

        [DataMember(Name = "source")]
        public string Source;

        [DataMember(Name = "user")]
        public User User;

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName;

        [DataMember(Name = "created_at")]
        public string CreatedAt;

        [DataMember(Name = "contributors")]
        public int[] Contributors;

        [DataMember(Name = "favorited")]
        public bool Favorited;

        [DataMember(Name = "truncated")]
        public bool Truncated;

        [DataMember(Name = "id")]
        public long Id;

        [DataMember(Name = "annotations", IsRequired = false)]
        public Annotations Annotations;

        [DataMember(Name = "place", IsRequired = false)]
        public Place Place;

        [DataMember(Name = "in_reply_to_status_id")]
        public string InReplyToStatusId;

        [DataMember(Name = "text")]
        public string Text;

        [DataMember(Name = "entities", IsRequired = false)]
        public Entities Entities;
    }
}