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
    using System.Drawing;
    using System.Text.RegularExpressions;

    public partial class Thumbnail
    {
        #region "Piapro"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Piapro_GetUrl(GetUrlArgs args)
        {
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://piapro\\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\\-]+)$");
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
        /// <param name="args">Class CreateImageArgs
        ///                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
        ///                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
        ///                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
        ///                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private static bool Piapro_CreateImage(CreateImageArgs args)
        {
            var mc = Regex.Match(args.Url.Value, "^http://piapro\\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\\-]+)$");
            if (!mc.Success)
            {
                return false;
            }

            string src = string.Empty;
            var http = new HttpVarious();
            if (!http.GetData(args.Url.Key, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                return false;
            }

            Match mc2 = Regex.Match(src, "<meta property=\"og:image\" content=\"(?<big_img>http://c1\\.piapro\\.jp/timg/[0-9a-z]+_\\d{14}_0500_0500\\.(?:jpg|png|gif)?)\" />");
            if (!mc2.Success)
            {
                args.Errmsg = "Pattern NotFound";
                return false;
            }

            // 各画像には120x120のサムネイルがある（多分）ので、URLを置き換える。元々ページに埋め込まれている画像は500x500
            var r = new Regex("_\\d{4}_\\d{4}");
            string minImgUrl = r.Replace(mc2.Groups["big_img"].Value, "_0120_0120");
            Image img = http.GetImage(minImgUrl, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }

        #endregion "Piapro"
    }
}