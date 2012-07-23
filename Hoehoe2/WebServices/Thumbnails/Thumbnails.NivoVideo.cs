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
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using R = Hoehoe.Properties.Resources;

    public partial class Thumbnail
    {
        #region "ニコニコ動画"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Nicovideo_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(?:(www|ext)\\.nicovideo\\.jp/watch|nico\\.ms)/(?:sm|nm)?([0-9]+)(\\?.+)?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.AddThumbnailUrl(args.Url, mc.Value);
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
        private static bool Nicovideo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            Match mc = Regex.Match(args.Url.Value, "^http://(?:(www|ext)\\.nicovideo\\.jp/watch|nico\\.ms)/(?<id>(?:sm|nm)?([0-9]+))(\\?.+)?$", RegexOptions.IgnoreCase);
            string apiurl = "http://www.nicovideo.jp/api/getthumbinfo/" + mc.Groups["id"].Value;
            string src = string.Empty;
            string imgurl = string.Empty;
            if ((new HttpVarious()).GetData(apiurl, null, ref src, 0, ref args.Errmsg, MyCommon.GetUserAgentString()))
            {
                StringBuilder sb = new StringBuilder();
                XmlDocument xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);
                    string status = xdoc.SelectSingleNode("/nicovideo_thumb_response").Attributes["status"].Value;
                    if (status == "ok")
                    {
                        imgurl = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/thumbnail_url").InnerText;
                        string tmp = null;
                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/title").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(R.NiconicoInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/length").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(R.NiconicoInfoText2);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            DateTime tm = new DateTime();
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/first_retrieve").InnerText;
                            if (DateTime.TryParse(tmp, out tm))
                            {
                                sb.Append(R.NiconicoInfoText3);
                                sb.Append(tm.ToString());
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/view_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(R.NiconicoInfoText4);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/comment_num").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(R.NiconicoInfoText5);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/mylist_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(R.NiconicoInfoText6);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (status == "fail")
                    {
                        args.Errmsg = xdoc.SelectSingleNode("/nicovideo_thumb_response/error/code").InnerText;
                        imgurl = string.Empty;
                    }
                    else
                    {
                        args.Errmsg = "UnknownResponse";
                        imgurl = string.Empty;
                    }
                }
                catch (Exception)
                {
                    imgurl = string.Empty;
                    args.Errmsg = "Invalid XML";
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    Image img = new HttpVarious().GetImage(imgurl, args.Url.Key, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, sb.ToString().Trim()));
                    return true;
                }
            }

            return false;
        }

        #endregion "ニコニコ動画"
    }
}