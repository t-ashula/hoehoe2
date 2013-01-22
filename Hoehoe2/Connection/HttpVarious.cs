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
    using System.Drawing;
    using System.IO;
    using System.Net;

    public class HttpVarious : HttpConnection
    {
        private delegate Image CheckValidImageDelegate(Image img, int width, int height);

        public string GetRedirectTo(string url)
        {
            try
            {
                HttpWebRequest req = CreateRequest(HeadMethod, new Uri(url), null, false);
                req.Timeout = 5000;
                req.AllowAutoRedirect = false;
                string data = string.Empty;
                var head = new Dictionary<string, string>();
                GetResponse(req, ref data, head, false);
                return head.ContainsKey("Location") ? head["Location"] : url;
            }
            catch (Exception)
            {
                return url;
            }
        }

        public Image GetImage(Uri url)
        {
            string t = string.Empty;
            return GetImage(url.ToString(), string.Empty, 10000, ref t);
        }

        public Image GetImage(string url)
        {
            string t = string.Empty;
            return GetImage(url, string.Empty, 10000, ref t);
        }

        public Image GetImage(string url, int timeout)
        {
            string t = string.Empty;
            return GetImage(url, string.Empty, timeout, ref t);
        }

        public Image GetImage(string url, string referer)
        {
            string t = string.Empty;
            return GetImage(url, referer, 10000, ref t);
        }

        public Image GetImage(string url, string referer, int timeout, ref string errmsg)
        {
            return GetImageInternal(MyCommon.CheckValidImage, url, referer, timeout, ref errmsg);
        }

        public Image GetIconImage(string url, int timeout)
        {
            string t = string.Empty;
            return GetImageInternal(CheckValidIconImage, url, string.Empty, timeout, ref t);
        }

        public bool PostData(string url, Dictionary<string, string> param)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(url), param, false);
                HttpStatusCode res = GetResponse(req, null, false);
                return res == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PostData(string url, Dictionary<string, string> param, ref string content)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(url), param, false);
                HttpStatusCode res = GetResponse(req, ref content, null, false);
                return res == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool GetData(string url, Dictionary<string, string> param, ref string content, string userAgent)
        {
            string t = string.Empty;
            return GetData(url, param, ref content, 100000, ref t, userAgent);
        }

        public bool GetData(string url, Dictionary<string, string> param, ref string content)
        {
            string t = string.Empty;
            return GetData(url, param, ref content, 100000, ref t, string.Empty);
        }

        public bool GetData(string url, Dictionary<string, string> param, ref string content, int timeout)
        {
            string t = string.Empty;
            return GetData(url, param, ref content, timeout, ref t, string.Empty);
        }

        public bool GetData(string url, Dictionary<string, string> param, ref string content, int timeout, ref string errmsg, string userAgent)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), param, false);
                if (timeout < 3000 || timeout > 100000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }

                if (!string.IsNullOrEmpty(userAgent))
                {
                    req.UserAgent = userAgent;
                }

                HttpStatusCode res = GetResponse(req, ref content, null, false);
                if (res == HttpStatusCode.OK)
                {
                    return true;
                }

                if (errmsg != null)
                {
                    errmsg = res.ToString();
                }

                return false;
            }
            catch (Exception ex)
            {
                if (errmsg != null)
                {
                    errmsg = ex.Message;
                }

                return false;
            }
        }

        public HttpStatusCode GetContent(string method, Uri url, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo, string userAgent)
        {
            // Searchで使用。呼び出し元で例外キャッチしている。
            HttpWebRequest req = CreateRequest(method, url, param, false);
            req.UserAgent = userAgent;
            return GetResponse(req, ref content, headerInfo, false);
        }

        public bool GetDataToFile(string url, string savePath)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
                req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                req.UserAgent = MyCommon.GetUserAgentString();
                using (var strm = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        HttpStatusCode res = GetResponse(req, strm, null, false);
                        strm.Close();
                        return res == HttpStatusCode.OK;
                    }
                    catch (Exception)
                    {
                        strm.Close();
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Image GetImageInternal(CheckValidImageDelegate checkImage, string url, string referer, int timeout, ref string errmsg)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
                if (!string.IsNullOrEmpty(referer))
                {
                    req.Referer = referer;
                }

                if (timeout < 3000 || timeout > 30000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }

                Bitmap img = null;
                HttpStatusCode ret = GetResponse(req, ref img, null, false);
                if (errmsg != null)
                {
                    errmsg = ret == HttpStatusCode.OK ? string.Empty : ret.ToString();
                }

                if (img != null)
                {
                    img.Tag = url;
                }

                if (ret == HttpStatusCode.OK)
                {
                    return checkImage(img, img.Width, img.Height);
                }

                return null;
            }
            catch (WebException ex)
            {
                if (errmsg != null)
                {
                    errmsg = ex.Message;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Image CheckValidIconImage(Image img, int width, int height)
        {
            return MyCommon.CheckValidImage(img, 48, 48);
        }
    }
}