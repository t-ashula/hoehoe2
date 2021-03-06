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

namespace Hoehoe
{
    public class UserInfo
    {
        public long Id;
        public string Name;
        public string ScreenName;
        public string Location;
        public string Description;
        public Uri ImageUrl;
        public string Url;
        public bool Protect;
        public int FriendsCount;
        public int FollowersCount;
        public int FavoriteCount;
        public DateTime CreatedAt;
        public int StatusesCount;
        public bool Verified;
        public string RecentPost;
        public DateTime PostCreatedAt;
        public string PostSource; // html形式　"<a href="http://sourceforge.jp/projects/tween/wiki/FrontPage" rel="nofollow">Tween</a>"
        public bool IsFollowing;
        public bool IsFollowed;

        public UserInfo()
        {
        }

        public UserInfo(DataModels.Twitter.User user)
        {
            Id = user.Id;
            Name = user.Name.Trim();
            ScreenName = user.ScreenName;
            Location = user.Location;
            Description = user.Description;
            try
            {
                ImageUrl = new Uri(user.ProfileImageUrl);
            }
            catch (Exception)
            {
                ImageUrl = null;
            }

            Url = user.Url;
            Protect = user.Protected;
            FriendsCount = user.FriendsCount;
            FollowersCount = user.FollowersCount;
            CreatedAt = MyCommon.DateTimeParse(user.CreatedAt);
            StatusesCount = user.StatusesCount;
            Verified = user.Verified;
            //// isFollowing = isFollowing;
            if (user.Status == null) return;

            RecentPost = user.Status.Text;
            PostCreatedAt = MyCommon.DateTimeParse(user.Status.CreatedAt);
            PostSource = user.Status.Source;
        }

        public override string ToString()
        {
            return ScreenName + " / " + Name;
        }
    }
}