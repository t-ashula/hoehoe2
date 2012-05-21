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
using Hoehoe.DataModels;

namespace Hoehoe
{
    public class UserInfo
    {
        public UserInfo()
        {
        }

        public UserInfo(DataModels.Twitter.User user)
        {
            this.Id = user.Id;
            this.Name = user.Name.Trim();
            this.ScreenName = user.ScreenName;
            this.Location = user.Location;
            this.Description = user.Description;
            try
            {
                this.ImageUrl = new Uri(user.ProfileImageUrl);
            }
            catch (Exception ex)
            {
                this.ImageUrl = null;
            }
            this.Url = user.Url;
            this.Protect = user.Protected;
            this.FriendsCount = user.FriendsCount;
            this.FollowersCount = user.FollowersCount;
            this.CreatedAt = MyCommon.DateTimeParse(user.CreatedAt);
            this.StatusesCount = user.StatusesCount;
            this.Verified = user.Verified;
            //this.isFollowing = this.isFollowing;
            if (user.Status != null)
            {
                this.RecentPost = user.Status.Text;
                this.PostCreatedAt = MyCommon.DateTimeParse(user.Status.CreatedAt);
                this.PostSource = user.Status.Source;
            }
        }

        public Int64 Id = 0;
        public string Name = String.Empty;
        public string ScreenName = String.Empty;
        public string Location = String.Empty;
        public string Description = String.Empty;
        public Uri ImageUrl;
        public string Url = String.Empty;
        public bool Protect;
        public int FriendsCount;
        public int FollowersCount;
        public int FavoriteCount;
        public DateTime CreatedAt = new DateTime();
        public int StatusesCount;
        public bool Verified;
        public string RecentPost = String.Empty;
        public DateTime PostCreatedAt = new DateTime();
        // html形式　"<a href="http://sourceforge.jp/projects/tween/wiki/FrontPage" rel="nofollow">Tween</a>"
        public string PostSource = String.Empty;
        public bool IsFollowing;
        public bool IsFollowed;

        public override string ToString()
        {
            return this.ScreenName + " / " + this.Name;
        }
    }
}