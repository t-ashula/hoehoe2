using System;
using System.Collections.Generic;
using System.Drawing;

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

using System.Net;

namespace Tween
{
    public class HttpVarious : HttpConnection
    {
        public string GetRedirectTo(string url)
        {
            try
            {
                HttpWebRequest req = CreateRequest(HeadMethod, new Uri(url), null, false);
                req.Timeout = 5000;
                req.AllowAutoRedirect = false;
                string data = "";
                Dictionary<string, string> head = new Dictionary<string, string>();
                HttpStatusCode ret = GetResponse(req, data, head, false);
                if (head.ContainsKey("Location"))
                {
                    return head["Location"];
                }
                else
                {
                    return url;
                }
            }
            catch (Exception ex)
            {
                return url;
            }
        }

        public Image GetImage(Uri url)
        {
            return GetImage(url.ToString(), "", 10000, ref null);
        }

        public Image GetImage(string url)
        {
            return GetImage(url, "", 10000, ref null);
        }

        public Image GetImage(string url, int timeout)
        {
            return GetImage(url, "", timeout, ref null);
        }

        public Image GetImage(string url, string referer)
        {
            return GetImage(url, referer, 10000, ref null);
        }

        public Image GetImage(string url, string referer, int timeout, ref string errmsg)
        {
            return GetImageInternal(CheckValidImage, url, referer, timeout, ref errmsg);
        }

        public Image GetIconImage(string url, int timeout)
        {
            return GetImageInternal(CheckValidIconImage, url, "", timeout, ref null);
        }

        private delegate Image CheckValidImageDelegate(Image img, int width, int height);

        private Image GetImageInternal(CheckValidImageDelegate CheckImage, string url, string referer, int timeout, ref string errmsg)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
                if (!string.IsNullOrEmpty(referer))
                    req.Referer = referer;
                if (timeout < 3000 || timeout > 30000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }
                Bitmap img = null;
                HttpStatusCode ret = GetResponse(req, img, null, false);
                if (errmsg != null)
                {
                    if (ret == HttpStatusCode.OK)
                    {
                        errmsg = "";
                    }
                    else
                    {
                        errmsg = ret.ToString();
                    }
                }
                if (img != null)
                    img.Tag = url;
                if (ret == HttpStatusCode.OK)
                    return CheckImage(img, img.Width, img.Height);
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
                HttpStatusCode res = this.GetResponse(req, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param, ref string content)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
                HttpStatusCode res = this.GetResponse(req, content, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content, string userAgent)
        {
            return GetData(Url, param, ref content, 100000, ref null, userAgent);
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content)
        {
            return GetData(Url, param, ref content, 100000, ref null, "");
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content, int timeout)
        {
            return GetData(Url, param, ref content, timeout, ref null, "");
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content, int timeout, ref string errmsg, string userAgent)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), param, false);
                if (timeout < 3000 || timeout > 100000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }
                if (!string.IsNullOrEmpty(userAgent))
                    req.UserAgent = userAgent;
                HttpStatusCode res = this.GetResponse(req, content, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
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

        public HttpStatusCode GetContent(string method, Uri Url, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo, string userAgent)
        {
            //Searchで使用。呼び出し元で例外キャッチしている。
            HttpWebRequest req = CreateRequest(method, Url, param, false);
            req.UserAgent = userAgent;
            return this.GetResponse(req, content, headerInfo, false);
        }

        public bool GetDataToFile(string Url, string savePath)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), null, false);
                req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                req.UserAgent = MyCommon.GetUserAgentString();
                using (System.IO.FileStream strm = new System.IO.FileStream(savePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    try
                    {
                        HttpStatusCode res = this.GetResponse(req, strm, null, false);
                        strm.Close();
                        if (res == HttpStatusCode.OK)
                            return true;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        strm.Close();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Image CheckValidIconImage(Image img, int width, int height)
        {
            return CheckValidImage(img, 48, 48);
        }

        public Image CheckValidImage(Image img, int width, int height)
        {
            if (img == null)
                return null;
            Bitmap bmp = new Bitmap(width, height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(img, 0, 0, width, height);
                }
                bmp.Tag = img.Tag;
                return bmp;
            }
            catch (Exception ex)
            {
                bmp.Dispose();
                bmp = new Bitmap(width, height);
                bmp.Tag = img.Tag;
                return bmp;
            }
            finally
            {
                img.Dispose();
            }
        }
    }
}