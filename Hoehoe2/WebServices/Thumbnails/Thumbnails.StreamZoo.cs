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

namespace Hoehoe
{
    using System.Text.RegularExpressions;

    public partial class Thumbnail
    {
        #region "StreamZoo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool StreamZoo_GetUrl(GetUrlArgs args)
        {
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, @"^http://streamzoo\.com/i/(\d+)$", RegexOptions.IgnoreCase);
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
        private static bool StreamZoo_CreateImage(CreateImageArgs args)
        {
            var src = string.Empty;
            if (!new HttpVarious().GetData(args.Url.Value, null, ref src, 0, ref args.Errmsg, MyCommon.GetUserAgentString()))
            {
                return false;
            }

            // 	<meta property="og:image"  content="http://cdn.streamzoo.com/si_5790766_p194egkoms_lr.jpg" /> 
            var thummc = Regex.Match(src, "property=\"og:image\"(?: *)content=\"([^\"]+)\"", RegexOptions.IgnoreCase);
            if (!thummc.Success)
            {
                return false;
            }

            var thumburl = thummc.Result("$1");
            var img = new HttpVarious().GetImage(thumburl, args.Url.Value, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }
        #endregion "StreamZoo"
    }
}