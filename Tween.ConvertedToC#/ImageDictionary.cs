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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.Caching;
using System.Threading;

namespace Hoehoe
{
    public class ImageDictionary : IDictionary<string, Image>, IDisposable
    {
        private readonly object _lockObject = new object();
        private MemoryCache _innerDictionary;
        private Stack<KeyValuePair<string, Action<Image>>> _waitStack;
        private CacheItemPolicy _cachePolicy = new CacheItemPolicy();
        private long _removedCount = 0;
        private Semaphore _netSemaphore;

        public ImageDictionary(int cacheMemoryLimit)
        {
            lock (this._lockObject)
            {
                //5Mb,80%
                //キャッシュチェック間隔はデフォルト値（2分毎）
                this._innerDictionary = new MemoryCache("imageCache", new NameValueCollection() {
                    { "CacheMemoryLimitMegabytes", cacheMemoryLimit.ToString() },
                    { "PhysicalMemoryLimitPercentage", "80" }
                });
                this._waitStack = new Stack<KeyValuePair<string, Action<Image>>>();
                this._cachePolicy.RemovedCallback = CacheRemoved;
                this._cachePolicy.SlidingExpiration = TimeSpan.FromMinutes(30);
                //30分参照されなかったら削除
                this._netSemaphore = new Semaphore(5, 5);
            }
        }

        public long CacheCount
        {
            get { return _innerDictionary.GetCount(); }
        }

        public long CacheRemoveCount
        {
            get { return _removedCount; }
        }

        public long CacheMemoryLimit
        {
            get { return _innerDictionary.CacheMemoryLimit; }
        }

        public long PhysicalMemoryLimit
        {
            get { return _innerDictionary.PhysicalMemoryLimit; }
        }

        public TimeSpan PollingInterval
        {
            get { return _innerDictionary.PollingInterval; }
        }

        private void CacheRemoved(CacheEntryRemovedArguments item)
        {
            ((Image)item.CacheItem.Value).Dispose();
            _removedCount += 1;
        }

        public void Add(KeyValuePair<string, Image> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(string key, Image value)
        {
            lock (this._lockObject)
            {
                if (this._innerDictionary.Contains(key))
                {
                    return;
                }
                this._innerDictionary.Add(key, value, this._cachePolicy);
            }
        }

        public bool Remove(KeyValuePair<string, Image> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            lock (this._lockObject)
            {
                this._innerDictionary.Remove(key);
            }
            return true;
        }

        public Image this[string key, bool force, Action<Image> callBack]
        {
            get
            {
                lock (this._lockObject)
                {
                    if (force)
                    {
                        this._innerDictionary.Remove(key);
                    }
                    else
                    {
                        if (this._innerDictionary.Contains(key))
                        {
                            return (Image)this._innerDictionary[key];
                        }
                    }
                    //スタックに積む
                    this._waitStack.Push(new KeyValuePair<string, Action<Image>>(key, callBack));
                }
                return null;
            }
        }

        public Image this[string key]
        {
            get
            {
                lock (this._lockObject)
                {
                    if (this._innerDictionary[key] != null)
                    {
                        try
                        {
                            return (Image)this._innerDictionary[key];
                        }
                        catch (Exception ex)
                        {
                            this._innerDictionary.Remove(key);
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                lock (this._lockObject)
                {
                    this._innerDictionary.Remove(key);
                    this._innerDictionary.Add(key, value, this._cachePolicy);
                }
            }
        }

        public void Clear()
        {
            lock (this._lockObject)
            {
                this._innerDictionary.Trim(100);
            }
        }

        public bool Contains(KeyValuePair<string, Image> item)
        {
            lock (this._lockObject)
            {
                return this._innerDictionary.Contains(item.Key) && object.ReferenceEquals(this._innerDictionary[item.Key], item.Value);
            }
        }

        public void CopyTo(KeyValuePair<string, Image>[] array, int arrayIndex)
        {
            lock (this._lockObject)
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                lock (this._lockObject)
                {
                    return Convert.ToInt32(this._innerDictionary.GetCount());
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(string key)
        {
            return this._innerDictionary.Contains(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                lock (this._lockObject)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public bool TryGetValue(string key, out Image value)
        {
            lock (this._lockObject)
            {
                if (this._innerDictionary.Contains(key))
                {
                    value = (Image)this._innerDictionary[key];
                    return true;
                }
                else
                {
                    value = default(Image);
                    return false;
                }
            }
        }

        public ICollection<Image> Values
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<KeyValuePair<string, Image>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator1()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        public void Dispose()
        {
            lock (this._lockObject)
            {
                this._netSemaphore.Dispose();
                this._innerDictionary.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        //取得一時停止
        private bool _pauseGetImage = false;

        private bool _pauseGetImage_popping = false;

        public bool PauseGetImage
        {
            get { return this._pauseGetImage; }
            set
            {
                if (!this._pauseGetImage && !_pauseGetImage_popping)
                {
                    _pauseGetImage_popping = true;
                    //最新から処理し
                    ThreadStart imgDlProc = null;
                    imgDlProc = () =>
                    {
                        while (!this._pauseGetImage)
                        {
                            if (this._waitStack.Count > 0)
                            {
                                KeyValuePair<string, Action<Image>> req = default(KeyValuePair<string, Action<Image>>);
                                lock (this._lockObject)
                                {
                                    req = this._waitStack.Pop();
                                }
                                if (AppendSettingDialog.Instance.IconSz == MyCommon.IconSizes.IconNone)
                                {
                                    continue;
                                }
                                GetImageDelegate proc = new GetImageDelegate(GetImage);
                                try
                                {
                                    this._netSemaphore.WaitOne();
                                }
                                catch (Exception)
                                {
                                    //Disposed,Release漏れ
                                }
                                proc.BeginInvoke(req, null, null);
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                        _pauseGetImage_popping = false;
                    };
                    imgDlProc.BeginInvoke(null, null);
                }
            }
        }

        public delegate void GetImageDelegate(KeyValuePair<string, Action<Image>> arg1);

        private void GetImage(KeyValuePair<string, Action<Image>> downloadAsyncInfo)
        {
            Image callbackImage = null;
            lock (_lockObject)
            {
                if (this._innerDictionary[downloadAsyncInfo.Key] != null)
                {
                    callbackImage = (Image)this._innerDictionary[downloadAsyncInfo.Key];
                }
            }
            if (callbackImage != null)
            {
                if (downloadAsyncInfo.Value != null)
                {
                    downloadAsyncInfo.Value.Invoke(callbackImage);
                }
                this._netSemaphore.Release();
                return;
            }
            HttpVarious hv = new HttpVarious();
            Image dlImage = hv.GetImage(downloadAsyncInfo.Key, 10000);
            lock (_lockObject)
            {
                if (this._innerDictionary[downloadAsyncInfo.Key] == null)
                {
                    if (dlImage != null)
                    {
                        this._innerDictionary.Add(downloadAsyncInfo.Key, dlImage, this._cachePolicy);
                        callbackImage = dlImage;
                    }
                }
                else
                {
                    callbackImage = (Image)this._innerDictionary[downloadAsyncInfo.Key];
                }
            }
            if (downloadAsyncInfo.Value != null)
            {
                downloadAsyncInfo.Value.Invoke(callbackImage);
            }
            this._netSemaphore.Release();
        }
    }
}