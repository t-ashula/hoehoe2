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

using System.Runtime.Serialization;

namespace Hoehoe.DataModels.Twitter
{
    [DataContract]
    public class BlocksIds
    {
        /* "previous_cursor": 0,
         * "ids": [   "123"  ],
         * "previous_cursor_str": "0",
         * "next_cursor": 0,
         * "next_cursor_str": "0"
         */

        [DataMember(Name = "previous_cursor")]
        public long PreviousCursor;

        [DataMember(Name = "next_cursor")]
        public long NextCursor;

        [DataMember(Name = "ids")]
        public long[] Ids;
    }
}