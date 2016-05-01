// Hoehoe - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
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
using System.Linq;
using System.Text.RegularExpressions;

namespace Hoehoe
{
    public partial class Thumbnail
    {
        #region "MiilMe"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool MiilMe_GetUrl(GetUrlArgs args)
        {
            // http://miil.me/p/82lz1 を parseInt('82lz1', 36) して GET http://miil.me/api/photos/13558717.js? すると callback 付きで json が取れるので /url を取ればいい
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, @"^https?://miil\.me/p/(\w+)$", RegexOptions.IgnoreCase);
            if (!mc.Success)
            {
                return false;
            }

            args.AddThumbnailUrl(args.Url, mc.Value);
            return true;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">CreateImageArgs</param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private static bool MiilMe_CreateImage(CreateImageArgs args)
        {
            var photoId = new Uri(args.Url.Value).AbsolutePath.Split('/')[2];
            var apiUrl = $"http://miil.me/api/photos/{ToInt32(photoId, 36)}.js";

            var json = string.Empty;
            if (!new HttpVarious().GetData(apiUrl, null, ref json, 0, ref args.Errmsg, MyCommon.GetUserAgentString()))
            {
                return false;
            }

            // name="twitter:image" content="http://images.miil.me/i/ab9f8cd6-9500-11e2-97f8-123143016634.jpg"
            var thummc = Regex.Match(json, "\"url\"\\s*:\\s*\"([^\"]+)\"", RegexOptions.IgnoreCase);
            if (!thummc.Success)
            {
                return false;
            }

            var thumburl = thummc.Result("$1");
            var img = new HttpVarious().GetImage(thumburl, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }

        private static int ToInt32(string s, int rad)
        {
            return s.Select(n => ('0' <= n && n <= '9') ? (n - '0') : ('a' <= n && n <= 'z') ? (n - 'a' + 10) : ('A' <= n && n <= 'Z') ? (n - 'A' + 10) : 0).Aggregate(0, (current, d) => current * rad + d);
        }

        #endregion
    }
}