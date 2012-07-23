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
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Xml;

    public partial class Thumbnail
    {
        #region "TINAMI"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Tinami_GetUrl(GetUrlArgs args)
        {
            //// http://www.tinami.com/view/250818
            //// http://tinami.jp/5dj6 (短縮URL)
            var mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www\\.tinami\\.com/view/\\d+$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.AddThumbnailUrl(args.Url, mc.Value);
                return true;
            }

            // 短縮URL
            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://tinami\\.jp/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                try
                {
                    args.AddThumbnailUrl(args.Url, "http://www.tinami.com/view/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString());
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
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
        private static bool Tinami_CreateImage(CreateImageArgs args)
        {
            var mc = Regex.Match(args.Url.Value, "^http://www\\.tinami\\.com/view/(?<ContentId>\\d+)$", RegexOptions.IgnoreCase);
            if (!mc.Success)
            {
                return false;
            }

            string src = string.Empty;
            const string ApiKey = "4e353d9113dce";             // TODO: TINAMI API Key
            string contentInfo = mc.Result("http://api.tinami.com/content/info?api_key=" + ApiKey + "&cont_id=${ContentId}");
            HttpVarious http = new HttpVarious();
            if (!http.GetData(contentInfo, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                return false;
            }

            XmlDocument xdoc = new XmlDocument();
            string thumbnailUrl = string.Empty;
            try
            {
                xdoc.LoadXml(src);
                var stat = xdoc.SelectSingleNode("/rsp").Attributes.GetNamedItem("stat").InnerText;
                if (stat != "ok")
                {
                    args.Errmsg = xdoc.SelectSingleNode("/rsp/err") != null ?
                        xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText :
                        "DeletedOrSuspended";
                    return false;
                }

                if (xdoc.SelectSingleNode("/rsp/content/thumbnails/thumbnail_150x150") != null)
                {
                    var nd = xdoc.SelectSingleNode("/rsp/content/thumbnails/thumbnail_150x150");
                    thumbnailUrl = nd.Attributes.GetNamedItem("url").InnerText;
                    if (string.IsNullOrEmpty(thumbnailUrl))
                    {
                        return false;
                    }

                    Image img = http.GetImage(thumbnailUrl, args.Url.Key);
                    if (img == null)
                    {
                        return false;
                    }

                    args.AddTooltipInfo(args.Url.Key, string.Empty, img); 
                    return true;
                }

                // エラー処理 エラーメッセージが返ってきた場合はここで処理
                if (xdoc.SelectSingleNode("/rsp/err") != null)
                {
                    args.Errmsg = xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText;
                }

                return false;
            }
            catch (Exception ex)
            {
                args.Errmsg = ex.Message;
                return false;
            }
        }

        #endregion "TINAMI"
    }
}