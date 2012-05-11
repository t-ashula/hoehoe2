// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tween
{
    public class FavoriteQueue : IList<long>
    {
        //Private Shared _instance As New FavoriteQueue
        //Public Shared ReadOnly Property GetInstance As FavoriteQueue
        //    Get
        //        Return _instance
        //    End Get
        //End Property

        private Twitter tw;

        private List<long> FavoriteCache = new List<long>();

        public void AddRange(IEnumerable<long> stsIds)
        {
            FavoriteCache.AddRange(stsIds);
        }

        //Public Sub FavoriteCacheAdd(ByVal statusId As Long, ByVal res As HttpStatusCode, Optional ByRef isMsg As Boolean = True)
        //    'If Not SettingInfo.Instance.IsUseFavoriteQueue Then Exit Sub
        //    Select Case res
        //        Case HttpStatusCode.BadGateway, HttpStatusCode.BadRequest, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.RequestTimeout
        //            isMsg = False
        //            FavoriteCache.Add(statusId)
        //    End Select
        //End Sub

        public void FavoriteCacheStart()
        {
            if (!(FavoriteCache.Count == 0))
            {
                List<long> _cacheList = new List<long>(FavoriteCache);
                this.Clear();
                Parallel.ForEach<long>(_cacheList, new Action<long>((long stsId) => { tw.PostFavAdd(stsId); }));
            }
        }

        public void Add(long item)
        {
            if (!this.Contains(item))
            {
                FavoriteCache.Add(item);
            }
        }

        public void Clear()
        {
            FavoriteCache.Clear();
            FavoriteCache.TrimExcess();
        }

        public bool Contains(long item)
        {
            return FavoriteCache.Contains(item);
        }

        public void CopyTo(long[] array, int arrayIndex)
        {
            FavoriteCache.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return FavoriteCache.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(long item)
        {
            return FavoriteCache.Remove(item);
        }

        public System.Collections.Generic.IEnumerator<long> GetEnumerator()
        {
            return FavoriteCache.GetEnumerator();
        }

        public System.Collections.IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        public int IndexOf(long item)
        {
            return FavoriteCache.IndexOf(item);
        }

        public void Insert(int index, long item)
        {
            FavoriteCache.Insert(index, item);
        }

        public long this[int index]
        {
            get { return FavoriteCache[index]; }
            set { FavoriteCache[index] = value; }
        }

        public void RemoveAt(int index)
        {
            FavoriteCache.RemoveAt(index);
        }

        public FavoriteQueue(Twitter twitter)
        {
            this.tw = twitter;
        }
    }
}