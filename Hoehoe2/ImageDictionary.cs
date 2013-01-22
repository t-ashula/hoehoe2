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
        private readonly object _lockObject = new object();
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy();
        private readonly MemoryCache _innerDictionary;
        private readonly Semaphore _netSemaphore;
        private bool _pauseGetImage; // 取得一時停止
        private bool _popping;
        private long _removedCount;
        private readonly Stack<KeyValuePair<string, Action<Image>>> waitStack;

        public ImageDictionary(int cacheMemoryLimit)
        {
            lock (_lockObject)
            {
                // 5Mb,80%
                // キャッシュチェック間隔はデフォルト値（2分毎）
                _innerDictionary = new MemoryCache(
                    "imageCache",
                    new NameValueCollection
                    {
                        { "CacheMemoryLimitMegabytes", cacheMemoryLimit.ToString() },
                        { "PhysicalMemoryLimitPercentage", "80" }
                    });
                waitStack = new Stack<KeyValuePair<string, Action<Image>>>();
                _cachePolicy.RemovedCallback = CacheRemoved;
                _cachePolicy.SlidingExpiration = TimeSpan.FromMinutes(30); // 30分参照されなかったら削除
                _netSemaphore = new Semaphore(5, 5);
            }
        }

        public delegate void GetImageDelegate(KeyValuePair<string, Action<Image>> arg1);

        public long CacheCount
        {
            get { return _innerDictionary.GetCount(); }
        }

        public long CacheMemoryLimit
        {
            get { return _innerDictionary.CacheMemoryLimit; }
        }

        public long CacheRemoveCount
        {
            get { return _removedCount; }
        }

        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return Convert.ToInt32(_innerDictionary.GetCount());
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
                return _pauseGetImage;
            }

            set
            {
                if (!_pauseGetImage && !_popping)
                {
                    // 最新から処理し
                    _popping = true;
                    ThreadStart imgDlProc = () =>
                    {
                        while (!_pauseGetImage)
                        {
                            if (waitStack.Count > 0)
                            {
                                KeyValuePair<string, Action<Image>> req = default(KeyValuePair<string, Action<Image>>);
                                lock (_lockObject)
                                {
                                    req = waitStack.Pop();
                                }

                                if (Configs.Instance.IconSz == IconSizes.IconNone)
                                {
                                    continue;
                                }

                                var proc = new GetImageDelegate(GetImage);
                                try
                                {
                                    _netSemaphore.WaitOne();
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

                        _popping = false;
                    };

                    imgDlProc.BeginInvoke(null, null);
                }
            }
        }

        public long PhysicalMemoryLimit
        {
            get { return _innerDictionary.PhysicalMemoryLimit; }
        }

        public TimeSpan PollingInterval
        {
            get { return _innerDictionary.PollingInterval; }
        }

        public ICollection<Image> Values
        {
            get { throw new NotImplementedException(); }
        }

        public Image this[string key, bool force, Action<Image> callBack]
        {
            get
            {
                lock (_lockObject)
                {
                    if (force)
                    {
                        _innerDictionary.Remove(key);
                    }
                    else
                    {
                        if (_innerDictionary.Contains(key))
                        {
                            return (Image)_innerDictionary[key];
                        }
                    }

                    // スタックに積む
                    waitStack.Push(new KeyValuePair<string, Action<Image>>(key, callBack));
                }

                return null;
            }
        }

        public Image this[string key]
        {
            get
            {
                lock (_lockObject)
                {
                    if (_innerDictionary[key] == null)
                    {
                        return null;
                    }

                    try
                    {
                        return (Image)_innerDictionary[key];
                    }
                    catch (Exception)
                    {
                        _innerDictionary.Remove(key);
                        return null;
                    }
                }
            }

            set
            {
                lock (_lockObject)
                {
                    _innerDictionary.Remove(key);
                    _innerDictionary.Add(key, value, _cachePolicy);
                }
            }
        }

        public void Add(KeyValuePair<string, Image> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(string key, Image value)
        {
            lock (_lockObject)
            {
                if (_innerDictionary.Contains(key))
                {
                    return;
                }

                _innerDictionary.Add(key, value, _cachePolicy);
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _innerDictionary.Trim(100);
            }
        }

        public bool Contains(KeyValuePair<string, Image> item)
        {
            lock (_lockObject)
            {
                return _innerDictionary.Contains(item.Key) && ReferenceEquals(_innerDictionary[item.Key], item.Value);
            }
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.Contains(key);
        }

        public void CopyTo(KeyValuePair<string, Image>[] array, int arrayIndex)
        {
            lock (_lockObject)
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                _netSemaphore.Dispose();
                _innerDictionary.Dispose();
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
            return Remove(item.Key);
        }

        public bool Remove(string key)
        {
            lock (_lockObject)
            {
                _innerDictionary.Remove(key);
            }

            return true;
        }

        public bool TryGetValue(string key, out Image value)
        {
            lock (_lockObject)
            {
                if (_innerDictionary.Contains(key))
                {
                    value = (Image)_innerDictionary[key];
                    return true;
                }

                value = default(Image);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        private void CacheRemoved(CacheEntryRemovedArguments item)
        {
            ((Image)item.CacheItem.Value).Dispose();
            _removedCount += 1;
        }

        private void GetImage(KeyValuePair<string, Action<Image>> downloadAsyncInfo)
        {
            Image callbackImage = null;
            lock (_lockObject)
            {
                if (_innerDictionary[downloadAsyncInfo.Key] != null)
                {
                    callbackImage = (Image)_innerDictionary[downloadAsyncInfo.Key];
                }
            }

            if (callbackImage != null)
            {
                if (downloadAsyncInfo.Value != null)
                {
                    downloadAsyncInfo.Value.Invoke(callbackImage);
                }

                _netSemaphore.Release();
                return;
            }

            var hv = new HttpVarious();
            Image image = hv.GetImage(downloadAsyncInfo.Key, 10000);
            lock (_lockObject)
            {
                if (_innerDictionary[downloadAsyncInfo.Key] == null)
                {
                    if (image != null)
                    {
                        _innerDictionary.Add(downloadAsyncInfo.Key, image, _cachePolicy);
                        callbackImage = image;
                    }
                }
                else
                {
                    callbackImage = (Image)_innerDictionary[downloadAsyncInfo.Key];
                }
            }

            if (downloadAsyncInfo.Value != null)
            {
                downloadAsyncInfo.Value.Invoke(callbackImage);
            }

            _netSemaphore.Release();
        }
    }
}