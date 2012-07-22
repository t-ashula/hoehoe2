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
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Text.RegularExpressions;
    using Hoehoe.DataModels;

    public partial class Thumbnail
    {
        #region "PicPlz"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool PicPlz_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://picplz\\.com/user/\\w+/pic/(?<longurl_ids>\\w+)/?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
                return true;
            }

            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://picplz\\.com/(?<shorturl_ids>\\w+)?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool PicPlz_CreateImage(CreateImageArgs args)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = "http://api.picplz.com/api/v2/pic.json?";
            Match mc = Regex.Match(args.Url.Value, "^http://picplz\\.com/user/\\w+/pic/(?<longurl_ids>\\w+)/?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                apiurl += "longurl_ids=" + mc.Groups["longurl_ids"].Value;
            }
            else
            {
                mc = Regex.Match(args.Url.Value, "^http://picplz\\.com/(?<shorturl_ids>\\w+)?$", RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    apiurl += "shorturl_ids=" + mc.Groups["shorturl_ids"].Value;
                }
                else
                {
                    return false;
                }
            }

            string src = string.Empty;
            string imgurl = string.Empty;
            if ((new HttpVarious()).GetData(apiurl, null, ref src, 0, ref args.Errmsg, MyCommon.GetUserAgentString()))
            {
                StringBuilder sb = new StringBuilder();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PicPlzDataModel.ResultData));
                PicPlzDataModel.ResultData res = default(PicPlzDataModel.ResultData);

                try
                {
                    res = D.CreateDataFromJson<PicPlzDataModel.ResultData>(src);
                }
                catch (Exception)
                {
                    return false;
                }

                if (res.Result == "ok")
                {
                    try
                    {
                        imgurl = res.Value.Pics[0].PicFiles.Pic320rh.ImgUrl;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        sb.Append(res.Value.Pics[0].Caption);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    Image img = http.GetImage(imgurl, args.Url.Key, 0, ref args.Errmsg);
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

        #endregion "PicPlz"

        private class PicPlzDataModel
        {
            [DataContract]
            public class Icon
            {
                [DataMember(Name = "url")]
                public string Url;

                [DataMember(Name = "width")]
                public int Width;

                [DataMember(Name = "height")]
                public int Height;
            }

            [DataContract]
            public class Creator
            {
                [DataMember(Name = "username")]
                public string Username;

                [DataMember(Name = "display_name")]
                public string DisplayName;

                [DataMember(Name = "following_count")]
                public int FollowingCount;

                [DataMember(Name = "follower_count")]
                public int FollowerCount;

                [DataMember(Name = "id")]
                public int Id;

                [DataMember(Name = "icon")]
                public PicPlzDataModel.Icon Icon;
            }

            [DataContract]
            public class PicFileInfo
            {
                [DataMember(Name = "width")]
                public int Width;

                [DataMember(Name = "img_url")]
                public string ImgUrl;

                [DataMember(Name = "height")]
                public int Height;
            }

            [DataContract]
            public class PicFiles
            {
                [DataMember(Name = "640r")]
                public PicFileInfo Pic640r;

                [DataMember(Name = "100sh")]
                public PicFileInfo Pic100sh;

                [DataMember(Name = "320rh")]
                public PicFileInfo Pic320rh;
            }

            [DataContract]
            public class Pics
            {
                [DataMember(Name = "view_count")]
                public int ViewCount;

                [DataMember(Name = "creator")]
                public Creator Creator;

                [DataMember(Name = "url")]
                public string Url;

                [DataMember(Name = "pic_files")]
                public PicFiles PicFiles;

                [DataMember(Name = "caption")]
                public string Caption;

                [DataMember(Name = "comment_count")]
                public int CommentCount;

                [DataMember(Name = "like_count")]
                public int LikeCount;

                [DataMember(Name = "date")]
                public long Date;

                [DataMember(Name = "id")]
                public int Id;
            }

            [DataContract]
            public class Value
            {
                [DataMember(Name = "pics")]
                public Pics[] Pics;
            }

            [DataContract]
            public class ResultData
            {
                [DataMember(Name = "result")]
                public string Result;

                [DataMember(Name = "value")]
                public Value Value;
            }
        }
    }
}