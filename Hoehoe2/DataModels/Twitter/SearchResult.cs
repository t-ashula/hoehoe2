// Hoehoe - Client of Twitter
// Copyright (c)  2011- t.ashula (@t_ashula) <office@ashula.info>
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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hoehoe.DataModels.Twitter
{
    [DataContract]
    public class SearchResult
    {
        [DataMember(Name = "statuses")]
        public List<Status> Statuses;

        [DataMember(Name = "search_metadata")]
        public SearchMetadata SearchMetadata;
    }

    [DataContract]
    public class SearchMetadata
    {
        [DataMember(Name = "max_id")]
        public long MaxId;

        [DataMember(Name = "max_id_str")]
        public string MaxIDStr;

        [DataMember(Name = "since_id")]
        public long SinceId;

        [DataMember(Name = "since_id_str")]
        public string SinceIDStr;

        [DataMember(Name = "refresh_url")]
        public string RefreshUrl;

        [DataMember(Name = "next_results")]
        public string NextResults;

        [DataMember(Name = "count")]
        public int Count;

        [DataMember(Name = "completed_in")]
        public double CompletedIn;

        [DataMember(Name = "query")]
        public string Query;
    }
}