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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;

    public partial class Thumbnail
    {
        #region "画像直リンク"

        private static bool IsDirectLink(string url)
        {
            return Regex.Match(url, "^http://.*(\\.jpg|\\.jpeg|\\.gif|\\.png|\\.bmp)$", RegexOptions.IgnoreCase).Success;
        }

        private static bool DirectLink_GetUrl(GetUrlArgs args)
        {
            // 画像拡張子で終わるURL（直リンク）
            if (IsDirectLink(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended))
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended));
                return true;
            }
            else
            {
                return false;
            }
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
        private static bool DirectLink_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "画像直リンク"
    }
}