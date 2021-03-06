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

using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using R = Hoehoe.Properties.Resources;

namespace Hoehoe
{
    public partial class Thumbnail
    {
        #region "youtube"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Youtube_GetUrl(GetUrlArgs args)
        {
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www\\.youtube\\.com/watch\\?v=([\\w\\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.AddThumbnailUrl(args.Url, mc.Result("${0}"));
                return true;
            }

            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://youtu\\.be/([\\w\\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.AddThumbnailUrl(args.Url, mc.Result("${0}"));
                return true;
            }

            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">Class CreateImageArgs
        ///                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
        ///                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
        ///                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
        ///                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private static bool Youtube_CreateImage(CreateImageArgs args)
        {
            // 参考
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
            // デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries
            // デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
            Match mcimg = Regex.Match(args.Url.Value, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            if (!mcimg.Success)
            {
                return false;
            }

            string imgurl = mcimg.Result("http://i.ytimg.com/vi/${videoid}/default.jpg");
            if (string.IsNullOrEmpty(imgurl))
            {
                return false;
            }

            string videourl = (new HttpVarious()).GetRedirectTo(args.Url.Value);
            var mc = Regex.Match(videourl, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            if (videourl.StartsWith("http://www.youtube.com/index?ytsession="))
            {
                videourl = args.Url.Value;
                mc = Regex.Match(videourl, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            }

            if (!mc.Success)
            {
                return false;
            }

            string apiurl = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups["videoid"].Value;
            string src = string.Empty;
            if (!(new HttpVarious()).GetData(apiurl, null, ref src, 5000))
            {
                return false;
            }

            var sb = new StringBuilder();
            try
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(src);
                var nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                nsmgr.AddNamespace("root", "http://www.w3.org/2005/Atom");
                nsmgr.AddNamespace("app", "http://purl.org/atom/app#");
                nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");

                var xentryNode = xdoc.DocumentElement.SelectSingleNode("/root:entry/media:group", nsmgr);
                var xentry = (XmlElement)xentryNode;
                string tmp;
                try
                {
                    tmp = xentry["media:title"].InnerText;
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        sb.AppendLine(R.YouTubeInfoText1 + tmp);
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    int sec;
                    if (int.TryParse(xentry["yt:duration"].Attributes["seconds"].Value, out sec))
                    {
                        sb.Append(R.YouTubeInfoText2);
                        sb.Append(string.Format("{0:d}:{1:d2}", sec / 60, sec % 60));
                        sb.AppendLine();
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    DateTime tmpdate;
                    xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                    if (DateTime.TryParse(xentry["published"].InnerText, out tmpdate))
                    {
                        sb.Append(R.YouTubeInfoText3);
                        sb.Append(tmpdate);
                        sb.AppendLine();
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    int count;
                    xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                    tmp = xentry["yt:statistics"].Attributes["viewCount"].Value;
                    if (int.TryParse(tmp, out count))
                    {
                        sb.Append(R.YouTubeInfoText4);
                        sb.Append(tmp);
                        sb.AppendLine();
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry/app:control", nsmgr);
                    if (xentry != null)
                    {
                        sb.AppendLine(string.Format("{0}:{1}", xentry["yt:state"].Attributes["name"].Value, xentry["yt:state"].InnerText));
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }

            var http = new HttpVarious();
            Image img = http.GetImage(imgurl, videourl, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.AddTooltipInfo(args.Url.Key, sb.ToString().Trim(), img);
            return true;
        }

        #endregion
    }
}