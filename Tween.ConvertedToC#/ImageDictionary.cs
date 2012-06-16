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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Runtime.Caching;
    using System.Threading;

    public class ImageDictionary : IDictionary<string, Image>, IDisposable
    {
        private readonly object lockObject = new object();
        private CacheItemPolicy cachePolicy = new CacheItemPolicy();
        private MemoryCache innerDictionary;
        private Semaphore netSemaphore;
        private bool pauseGetImage; // 取得一時停止
        private bool popping;
        private long removedCount = 0;
        private Stack<KeyValuePair<string, Action<Image>>> waitStack;

        public ImageDictionary(int cacheMemoryLimit)
        {
            lock (this.lockObject)
            {
                // 5Mb,80%
                // キャッシュチェック間隔はデフォルト値（2分毎）
                this.innerDictionary = new MemoryCache(
                    "imageCache",
                    new NameValueCollection
                    {
                        { "CacheMemoryLimitMegabytes", cacheMemoryLimit.ToString() },
                        { "PhysicalMemoryLimitPercentage", "80" }
                    });
                this.waitStack = new Stack<KeyValuePair<string, Action<Image>>>();
                this.cachePolicy.RemovedCallback = this.CacheRemoved;
                this.cachePolicy.SlidingExpiration = TimeSpan.FromMinutes(30); // 30分参照されなかったら削除
                this.netSemaphore = new Semaphore(5, 5);
            }
        }

        public delegate void GetImageDelegate(KeyValuePair<string, Action<Image>> arg1);

        public long CacheCount
        {
            get { return this.innerDictionary.GetCount(); }
        }

        public long CacheMemoryLimit
        {
            get { return this.innerDictionary.CacheMemoryLimit; }
        }

        public long CacheRemoveCount
        {
            get { return this.removedCount; }
        }

        public int Count
        {
            get
            {
                lock (this.lockObject)
                {
                    return Convert.ToInt32(this.innerDictionary.GetCount());
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<string> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool PauseGetImage
        {
            get
            {
                return this.pauseGetImage;
            }

            set
            {
                if (!this.pauseGetImage && !this.popping)
                {
                    // 最新から処理し
                    this.popping = true;
                    ThreadStart imgDlProc = () =>
                    {
                        while (!this.pauseGetImage)
                        {
                            if (this.waitStack.Count > 0)
                            {
                                KeyValuePair<string, Action<Image>> req = default(KeyValuePair<string, Action<Image>>);
                                lock (this.lockObject)
                                {
                                    req = this.waitStack.Pop();
                                }

                                if (Configs.Instance.IconSz == IconSizes.IconNone)
                                {
                                    continue;
                                }

                                GetImageDelegate proc = new GetImageDelegate(GetImage);
                                try
                                {
                                    this.netSemaphore.WaitOne();
                                }
                                catch (Exception)
                                {
                                    // Disposed,Release漏れ
                                }

                                proc.BeginInvoke(req, null, null);
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }

                        popping = false;
                    };

                    imgDlProc.BeginInvoke(null, null);
                }
            }
        }

        public long PhysicalMemoryLimit
        {
            get { return this.innerDictionary.PhysicalMemoryLimit; }
        }

        public TimeSpan PollingInterval
        {
            get { return this.innerDictionary.PollingInterval; }
        }

        public ICollection<Image> Values
        {
            get { throw new NotImplementedException(); }
        }

        public Image this[string key, bool force, Action<Image> callBack]
        {
            get
            {
                lock (this.lockObject)
                {
                    if (force)
                    {
                        this.innerDictionary.Remove(key);
                    }
                    else
                    {
                        if (this.innerDictionary.Contains(key))
                        {
                            return (Image)this.innerDictionary[key];
                        }
                    }

                    // スタックに積む
                    this.waitStack.Push(new KeyValuePair<string, Action<Image>>(key, callBack));
                }

                return null;
            }
        }

        public Image this[string key]
        {
            get
            {
                lock (this.lockObject)
                {
                    if (this.innerDictionary[key] == null)
                    {
                        return null;
                    }

                    try
                    {
                        return (Image)this.innerDictionary[key];
                    }
                    catch (Exception)
                    {
                        this.innerDictionary.Remove(key);
                        return null;
                    }
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.innerDictionary.Remove(key);
                    this.innerDictionary.Add(key, value, this.cachePolicy);
                }
            }
        }

        public void Add(KeyValuePair<string, Image> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(string key, Image value)
        {
            lock (this.lockObject)
            {
                if (this.innerDictionary.Contains(key))
                {
                    return;
                }

                this.innerDictionary.Add(key, value, this.cachePolicy);
            }
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                this.innerDictionary.Trim(100);
            }
        }

        public bool Contains(KeyValuePair<string, Image> item)
        {
            lock (this.lockObject)
            {
                return this.innerDictionary.Contains(item.Key) && object.ReferenceEquals(this.innerDictionary[item.Key], item.Value);
            }
        }

        public bool ContainsKey(string key)
        {
            return this.innerDictionary.Contains(key);
        }

        public void CopyTo(KeyValuePair<string, Image>[] array, int arrayIndex)
        {
            lock (this.lockObject)
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            lock (this.lockObject)
            {
                this.netSemaphore.Dispose();
                this.innerDictionary.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public IEnumerator<KeyValuePair<string, Image>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator1()
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, Image> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            lock (this.lockObject)
            {
                this.innerDictionary.Remove(key);
            }

            return true;
        }

        public bool TryGetValue(string key, out Image value)
        {
            lock (this.lockObject)
            {
                if (this.innerDictionary.Contains(key))
                {
                    value = (Image)this.innerDictionary[key];
                    return true;
                }

                value = default(Image);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator1();
        }

        private void CacheRemoved(CacheEntryRemovedArguments item)
        {
            ((Image)item.CacheItem.Value).Dispose();
            this.removedCount += 1;
        }

        private void GetImage(KeyValuePair<string, Action<Image>> downloadAsyncInfo)
        {
            Image callbackImage = null;
            lock (this.lockObject)
            {
                if (this.innerDictionary[downloadAsyncInfo.Key] != null)
                {
                    callbackImage = (Image)this.innerDictionary[downloadAsyncInfo.Key];
                }
            }

            if (callbackImage != null)
            {
                if (downloadAsyncInfo.Value != null)
                {
                    downloadAsyncInfo.Value.Invoke(callbackImage);
                }

                this.netSemaphore.Release();
                return;
            }

            HttpVarious hv = new HttpVarious();
            Image image = hv.GetImage(downloadAsyncInfo.Key, 10000);
            lock (this.lockObject)
            {
                if (this.innerDictionary[downloadAsyncInfo.Key] == null)
                {
                    if (image != null)
                    {
                        this.innerDictionary.Add(downloadAsyncInfo.Key, image, this.cachePolicy);
                        callbackImage = image;
                    }
                }
                else
                {
                    callbackImage = (Image)this.innerDictionary[downloadAsyncInfo.Key];
                }
            }

            if (downloadAsyncInfo.Value != null)
            {
                downloadAsyncInfo.Value.Invoke(callbackImage);
            }

            this.netSemaphore.Release();
        }
    }
}