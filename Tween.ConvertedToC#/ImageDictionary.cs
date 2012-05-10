using System;
using System.Collections.Generic;

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

using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.Caching;
using System.Threading;

namespace Tween
{
    public class ImageDictionary : IDictionary<string, Image>, IDisposable
    {
        private readonly object lockObject = new object();
        private MemoryCache innerDictionary;
        private Stack<KeyValuePair<string, Action<Image>>> waitStack;
        private CacheItemPolicy cachePolicy = new CacheItemPolicy();
        private long removedCount = 0;

        private Semaphore netSemaphore;

        public ImageDictionary(int cacheMemoryLimit)
		{
			lock (this.lockObject) {
				//5Mb,80%
				//キャッシュチェック間隔はデフォルト値（2分毎）
				var nvc = new NameValueCollection();
				nvc.Add({
					"CacheMemoryLimitMegabytes",
					cacheMemoryLimit.ToString()
				});
				nvc.Add({
					"PhysicalMemoryLimitPercentage",
					"80"
				});
				this.innerDictionary = new MemoryCache("imageCache", nvc);
				this.waitStack = new Stack<KeyValuePair<string, Action<Image>>>();
				this.cachePolicy.RemovedCallback = CacheRemoved;
				this.cachePolicy.SlidingExpiration = TimeSpan.FromMinutes(30);
				//30分参照されなかったら削除
				this.netSemaphore = new Semaphore(5, 5);
			}
		}

        public long CacheCount
        {
            get { return innerDictionary.GetCount(); }
        }

        public long CacheRemoveCount
        {
            get { return removedCount; }
        }

        public long CacheMemoryLimit
        {
            get { return innerDictionary.CacheMemoryLimit; }
        }

        public long PhysicalMemoryLimit
        {
            get { return innerDictionary.PhysicalMemoryLimit; }
        }

        public TimeSpan PollingInterval
        {
            get { return innerDictionary.PollingInterval; }
        }

        private void CacheRemoved(CacheEntryRemovedArguments item)
        {
            ((Image)item.CacheItem.Value).Dispose();
            removedCount += 1;
            //System.Diagnostics.Debug.Print("cache delete")
        }

        public void Add(System.Collections.Generic.KeyValuePair<string, Image> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(string key, Image value)
        {
            lock (this.lockObject)
            {
                if (this.innerDictionary.Contains(key))
                    return;
                this.innerDictionary.Add(key, value, this.cachePolicy);
            }
        }

        public bool Remove(System.Collections.Generic.KeyValuePair<string, Image> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            lock (this.lockObject)
            {
                this.innerDictionary.Remove(key);
            }
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
                            return (Image)this.innerDictionary[key];
                    }
                    //スタックに積む
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
                    if (this.innerDictionary[key] != null)
                    {
                        try
                        {
                            return (Image)this.innerDictionary[key];
                        }
                        catch (Exception ex)
                        {
                            this.innerDictionary.Remove(key);
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
                lock (this.lockObject)
                {
                    this.innerDictionary.Remove(key);
                    this.innerDictionary.Add(key, value, this.cachePolicy);
                }
            }
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                this.innerDictionary.Trim(100);
            }
        }

        public bool Contains(System.Collections.Generic.KeyValuePair<string, Image> item)
        {
            lock (this.lockObject)
            {
                return this.innerDictionary.Contains(item.Key) && object.ReferenceEquals(this.innerDictionary[item.Key], item.Value);
            }
        }

        public void CopyTo(System.Collections.Generic.KeyValuePair<string, Image>[] array, int arrayIndex)
        {
            lock (this.lockObject)
            {
                throw new NotImplementedException();
            }
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

        public bool ContainsKey(string key)
        {
            return this.innerDictionary.Contains(key);
        }

        public System.Collections.Generic.ICollection<string> Keys
        {
            get
            {
                lock (this.lockObject)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public bool TryGetValue(string key, ref Image value)
        {
            lock (this.lockObject)
            {
                if (this.innerDictionary.Contains(key))
                {
                    value = (Image)this.innerDictionary[key];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public System.Collections.Generic.ICollection<Image> Values
        {
            get
            {
                lock (this.lockObject)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, Image>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public System.Collections.IEnumerator GetEnumerator1()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
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

        //取得一時停止
        private bool _pauseGetImage = false;

        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_PauseGetImage_popping_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();

        bool static_PauseGetImage_popping;

        public bool PauseGetImage
        {
            get { return this._pauseGetImage; }
            set
            {
                this._pauseGetImage = value;
                lock (static_PauseGetImage_popping_Init)
                {
                    try
                    {
                        if (InitStaticVariableHelper(static_PauseGetImage_popping_Init))
                        {
                            static_PauseGetImage_popping = false;
                        }
                    }
                    finally
                    {
                        static_PauseGetImage_popping_Init.State = 1;
                    }
                }

                if (!this._pauseGetImage && !static_PauseGetImage_popping)
                {
                    static_PauseGetImage_popping = true;
                    //最新から処理し
                    ThreadStart imgDlProc = null;
                    imgDlProc = () =>
                    {
                        while (!this._pauseGetImage)
                        {
                            if (this.waitStack.Count > 0)
                            {
                                KeyValuePair<string, Action<Image>> req = default(KeyValuePair<string, Action<Image>>);
                                lock (this.lockObject)
                                {
                                    req = this.waitStack.Pop();
                                }
                                if (AppendSettingDialog.Instance.IconSz == IconSizes.IconNone)
                                    continue;
                                GetImageDelegate proc = new GetImageDelegate(GetImage);
                                try
                                {
                                    this.netSemaphore.WaitOne();
                                }
                                catch (Exception ex)
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
                        static_PauseGetImage_popping = false;
                    };
                    imgDlProc.BeginInvoke(null, null);
                }
            }
        }

        public delegate void GetImageDelegate(KeyValuePair<string, Action<Image>> arg1);

        private void GetImage(KeyValuePair<string, Action<Image>> downloadAsyncInfo)
        {
            Image callbackImage = null;
            lock (lockObject)
            {
                if (this.innerDictionary[downloadAsyncInfo.Key] != null)
                {
                    callbackImage = (Image)this.innerDictionary[downloadAsyncInfo.Key];
                }
            }
            if (callbackImage != null)
            {
                if (downloadAsyncInfo.Value != null)
                    downloadAsyncInfo.Value.Invoke(callbackImage);
                this.netSemaphore.Release();
                return;
            }
            HttpVarious hv = new HttpVarious();
            Image dlImage = hv.GetImage(downloadAsyncInfo.Key, 10000);
            lock (lockObject)
            {
                if (this.innerDictionary[downloadAsyncInfo.Key] == null)
                {
                    if (dlImage != null)
                    {
                        this.innerDictionary.Add(downloadAsyncInfo.Key, dlImage, this.cachePolicy);
                        callbackImage = dlImage;
                    }
                }
                else
                {
                    callbackImage = (Image)this.innerDictionary[downloadAsyncInfo.Key];
                }
            }
            if (downloadAsyncInfo.Value != null)
                downloadAsyncInfo.Value.Invoke(callbackImage);
            this.netSemaphore.Release();
        }

        private static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
        {
            if (flag.State == 0)
            {
                flag.State = 2;
                return true;
            }
            else if (flag.State == 2)
            {
                throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
            }
            else
            {
                return false;
            }
        }
    }
}