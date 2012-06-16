// Hoehoe - Client of Twitter
// Copyright (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
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
    using System.Linq;
    using System.Text;
    using Hoehoe.DataModels.Twitter;

    public class Configs
    {
        private static Configs instance = new Configs();

        private Configs()
        {
        }

        public static Configs Instance
        {
            get { return instance; }
        }

        public bool HideDuplicatedRetweets { get; set; }
        public bool IsPreviewFoursquare { get; set; }
        public int FoursquarePreviewHeight { get; set; }
        public int FoursquarePreviewWidth { get; set; }
        public int FoursquarePreviewZoom { get; set; }
        public bool IsListStatusesIncludeRts { get; set; }
        public List<UserAccount> UserAccounts { get; set; }
        public bool TabMouseLock { get; set; }
        public bool IsRemoveSameEvent { get; set; }
        public bool IsNotifyUseGrowl { get; set; }
        public Configuration TwitterConfiguration { get; set; }
        public int UserstreamPeriodInt { get; set; }
        public bool UserstreamStartup { get; set; }
        public int TimelinePeriodInt { get; set; }
        public int ReplyPeriodInt { get; set; }
        public int DMPeriodInt { get; set; }
        public int PubSearchPeriodInt { get; set; }
        public int ListsPeriodInt { get; set; }
        public int UserTimelinePeriodInt { get; set; }
        public bool Readed { get; set; }
        public IconSizes IconSz { get; set; }
        public string Status { get; set; }
        public bool UnreadManage { get; set; }
        public bool PlaySound { get; set; }
        public bool OneWayLove { get; set; }
    }
}
