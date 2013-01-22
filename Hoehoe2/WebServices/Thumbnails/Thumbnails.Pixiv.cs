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
    using System.Text.RegularExpressions;
    using System.Web;

    public partial class Thumbnail
    {
        #region "Pixiv"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Pixiv_GetUrl(GetUrlArgs args)
        {
            // 参考: http://tail.s68.xrea.com/blog/2009/02/pixivflash.html Pixivの画像をFlashとかで取得する方法など:しっぽのブログ
            // ユーザー向けの画像ページ http://www.pixiv.net/member_illust.php?mode=medium&illust_id=[ID番号]
            // 非ログインユーザー向けの画像ページ http://www.pixiv.net/index.php?mode=medium&illust_id=[ID番号]
            // サムネイルURL http://img[サーバー番号].pixiv.net/img/[ユーザー名]/[サムネイルID]_s.[拡張子]
            // サムネイルURLは画像ページから抽出する
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended,
                                 @"^http://www\.pixiv\.net/(member_illust|index)\.php\?(.*)illust_id=([0-9]+)(.*)$", RegexOptions.IgnoreCase);
            if (!mc.Success)
            {
                return false;
            }

            args.AddThumbnailUrl(args.Url.Replace("amp;", string.Empty), mc.Value);
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
        private static bool Pixiv_CreateImage(CreateImageArgs args)
        {
            var url = new Uri(args.Url.Value);
            var queries = HttpUtility.ParseQueryString(url.Query);
            if (!string.IsNullOrEmpty(queries["tag"]) && queries["tag"].StartsWith("R-18"))
            {
                args.Errmsg = "NotSupported";
                return false;
            }
            //var mc = Regex.Match(args.Url.Value, "^http://www\\.pixiv\\.net/(member_illust|index)\\.php\\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase);

            HttpVarious http = new HttpVarious();
            string src = string.Empty;
            if (!http.GetData(Regex.Replace(args.Url.Value, "amp;", string.Empty), null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                return false;
            }

            // illustIDをキャプチャ
            var illustId = queries["illust_id"];
            var mc2 = Regex.Match(src, string.Format(@"http://i(mg)?([0-9]+)\.pixiv\.net/.+/img/.+/{0}_[ms]\.([a-zA-Z]+)", illustId));
            if (!mc2.Success)
            {
                args.Errmsg = Regex.Match(src, "<span class='error'>ログインしてください</span>").Success ? "NotSupported" : "Pattern NotFound";
                return false;
            }

            var img = http.GetImage(mc2.Value, args.Url.Value, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }

        #endregion "Pixiv"
    }
}