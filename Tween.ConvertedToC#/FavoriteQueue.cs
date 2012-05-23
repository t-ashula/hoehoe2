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
    using System.Threading.Tasks;

    public class FavoriteQueue : IList<long>
    {
        private Twitter _tw;
        private List<long> _favoriteCache;

        public void AddRange(IEnumerable<long> stsIds)
        {
            this._favoriteCache.AddRange(stsIds);
        }

        public void FavoriteCacheStart()
        {
            if (this._favoriteCache.Count != 0)
            {
                List<long> cacheList = new List<long>(this._favoriteCache);
                this.Clear();
                Parallel.ForEach<long>(cacheList, new Action<long>((long stsId) => { this._tw.PostFavAdd(stsId); }));
            }
        }

        public void Add(long item)
        {
            if (!this.Contains(item))
            {
                this._favoriteCache.Add(item);
            }
        }

        public void Clear()
        {
            this._favoriteCache.Clear();
            this._favoriteCache.TrimExcess();
        }

        public bool Contains(long item)
        {
            return this._favoriteCache.Contains(item);
        }

        public void CopyTo(long[] array, int arrayIndex)
        {
            this._favoriteCache.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._favoriteCache.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(long item)
        {
            return this._favoriteCache.Remove(item);
        }

        public System.Collections.Generic.IEnumerator<long> GetEnumerator()
        {
            return this._favoriteCache.GetEnumerator();
        }

        public System.Collections.IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator1();
        }

        public int IndexOf(long item)
        {
            return this._favoriteCache.IndexOf(item);
        }

        public void Insert(int index, long item)
        {
            this._favoriteCache.Insert(index, item);
        }

        public long this[int index]
        {
            get { return this._favoriteCache[index]; }
            set { this._favoriteCache[index] = value; }
        }

        public void RemoveAt(int index)
        {
            this._favoriteCache.RemoveAt(index);
        }

        public FavoriteQueue(Twitter twitter)
        {
            this._favoriteCache = new List<long>();
            this._tw = twitter;
        }
    }
}