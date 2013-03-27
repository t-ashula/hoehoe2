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

using System.Drawing;
using System.Text.RegularExpressions;

namespace Hoehoe
{
    public partial class Thumbnail
    {
        #region "Plixi(TweetPhoto)"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Plixi_GetUrl(GetUrlArgs args)
        {
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^(http://tweetphoto\\.com/[0-9]+|http://pic\\.gd/[a-z0-9]+|http://(lockerz|plixi)\\.com/[ps]/[0-9]+)$", RegexOptions.IgnoreCase);
            if (!mc.Success)
            {
                return false;
            }

            const string Api = "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url=";
            args.AddThumbnailUrl(args.Url, Api + (string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended));
            return true;
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
        private static bool Plixi_CreateImage(CreateImageArgs args)
        {
            string referer;
            if (!args.Url.Key.Contains("t.co"))
            {
                referer = args.Url.Key;
            }
            else
            {
                if (args.Url.Value.Contains("tweetphoto.com"))
                {
                    referer = "http://tweetphoto.com";
                }
                else if (args.Url.Value.Contains("http://lockerz.com"))
                {
                    referer = "http://lockerz.com";
                }
                else
                {
                    referer = "http://plixi.com";
                }
            }

            Image img = (new HttpVarious()).GetImage(args.Url.Value, referer, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }

        #endregion
    }
}