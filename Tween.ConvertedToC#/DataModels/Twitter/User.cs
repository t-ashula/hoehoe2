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
    public class User
    {
        [DataMember(Name = "statuses_count")]
        public int StatusesCount;

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor;

        [DataMember(Name = "show_all_inline_media")]
        public bool ShowAllInlineMedia;

        [DataMember(Name = "profile_use_background_image")]
        public bool ProfileUseBackgroundImage;

        [DataMember(Name = "contributors_enabled")]
        public bool ContributorsEnabled;

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor;

        [DataMember(Name = "location")]
        public string Location;

        [DataMember(Name = "geo_enabled")]
        public bool GeoEnabled;

        [DataMember(Name = "description")]
        public string Description;

        [DataMember(Name = "friends_count")]
        public int FriendsCount;

        [DataMember(Name = "verified")]
        public bool Verified;

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount;

        [DataMember(Name = "created_at")]
        public string CreatedAt;

        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor;

        [DataMember(Name = "follow_request_sent")]
        public string FollowRequestSent;

        [DataMember(Name = "time_zone")]
        public string TimeZone;

        [DataMember(Name = "followers_count")]
        public int FollowersCount;

        [DataMember(Name = "url")]
        public string Url;

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl;

        [DataMember(Name = "notifications")]
        public string Notifications;

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor;

        [DataMember(Name = "protected")]
        public bool Protected;

        [DataMember(Name = "id_str")]
        public string IdStr;

        [DataMember(Name = "lang")]
        public string Lang;

        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageUrl;

        [DataMember(Name = "screen_name")]
        public string ScreenName;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "following")]
        public string Following;

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor;

        [DataMember(Name = "id")]
        public long Id;

        [DataMember(Name = "listed_count")]
        public int ListedCount;

        [DataMember(Name = "profile_background_tile")]
        public bool ProfileBackgroundTile;

        [DataMember(Name = "utc_offset")]
        public string UtcOffset;

        [DataMember(Name = "place", IsRequired = false)]
        public Place Place;

        [DataMember(Name = "status", IsRequired = false)]
        public Status Status;
    }
}