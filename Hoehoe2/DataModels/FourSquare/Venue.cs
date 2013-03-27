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

using System.Runtime.Serialization;

namespace Hoehoe.DataModels.FourSquare
{
    [DataContract]
    public class Venue
    {
        [DataMember(Name = "id")]
        public string Id;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "location")]
        public Location Location;

        [DataMember(Name = "verified")]
        public bool Verified;

        [DataMember(Name = "stats")]
        public Stats Stats;

        [DataMember(Name = "mayor")]
        public Mayor Mayor;

        [DataMember(Name = "shortUrl")]
        public string ShortUrl;

        [DataMember(Name = "timeZone")]
        public string TimeZone;
    }
}