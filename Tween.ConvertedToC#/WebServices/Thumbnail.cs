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
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;
    using Hoehoe.DataModels;

    public class Thumbnail
    {
        private object lckPrev = new object();
        private PreviewData preview;
        private TweenMain tweenMain;

        private ThumbnailService[] thumbnailServices =
        {
            new ThumbnailService("ImgUr", ImgUr_GetUrl, ImgUr_CreateImage),
            new ThumbnailService("DirectLink", DirectLink_GetUrl, DirectLink_CreateImage),
            new ThumbnailService("TwitPic", TwitPic_GetUrl, TwitPic_CreateImage),
            new ThumbnailService("yfrog", Yfrog_GetUrl, Yfrog_CreateImage),
            new ThumbnailService("Plixi(TweetPhoto)", Plixi_GetUrl, Plixi_CreateImage),
            new ThumbnailService("MobyPicture", MobyPicture_GetUrl, MobyPicture_CreateImage),
            new ThumbnailService("携帯百景", MovaPic_GetUrl, MovaPic_CreateImage),
            new ThumbnailService("はてなフォトライフ", Hatena_GetUrl, Hatena_CreateImage),
            new ThumbnailService("PhotoShare/bctiny", PhotoShare_GetUrl, PhotoShare_CreateImage),
            new ThumbnailService("img.ly", Imgly_GetUrl, Imgly_CreateImage),
            new ThumbnailService("brightkite", Brightkite_GetUrl, Brightkite_CreateImage),
            new ThumbnailService("Twitgoo", Twitgoo_GetUrl, Twitgoo_CreateImage),
            new ThumbnailService("youtube", Youtube_GetUrl, Youtube_CreateImage),
            new ThumbnailService("ニコニコ動画", Nicovideo_GetUrl, Nicovideo_CreateImage),
            new ThumbnailService("ニコニコ静画", Nicoseiga_GetUrl, Nicoseiga_CreateImage),
            new ThumbnailService("Pixiv", Pixiv_GetUrl, Pixiv_CreateImage),
            new ThumbnailService("flickr", Flickr_GetUrl, Flickr_CreateImage),
            new ThumbnailService("フォト蔵", Photozou_GetUrl, Photozou_CreateImage),
            new ThumbnailService("TwitVideo", TwitVideo_GetUrl, TwitVideo_CreateImage),
            new ThumbnailService("Piapro", Piapro_GetUrl, Piapro_CreateImage),
            new ThumbnailService("Tumblr", Tumblr_GetUrl, Tumblr_CreateImage),
            new ThumbnailService("ついっぷるフォト", TwipplePhoto_GetUrl, TwipplePhoto_CreateImage),
            new ThumbnailService("mypix/shamoji", Mypix_GetUrl, Mypix_CreateImage),
            new ThumbnailService("ow.ly", Owly_GetUrl, Owly_CreateImage),
            new ThumbnailService("vimeo", Vimeo_GetUrl, Vimeo_CreateImage),
            new ThumbnailService("cloudfiles", CloudFiles_GetUrl, CloudFiles_CreateImage),
            new ThumbnailService("instagram", Instagram_GetUrl, Instagram_CreateImage),
            new ThumbnailService("pikubo", Pikubo_GetUrl, Pikubo_CreateImage),
            new ThumbnailService("PicPlz", PicPlz_GetUrl, PicPlz_CreateImage),
            new ThumbnailService("FourSquare", Foursquare_GetUrl, Foursquare_CreateImage),
            new ThumbnailService("TINAMI", Tinami_GetUrl, Tinami_CreateImage),
            new ThumbnailService("Twimg", Twimg_GetUrl, Twimg_CreateImage)
        };

        public Thumbnail(TweenMain owner)
        {
            this.tweenMain = owner;

            owner.PreviewScrollBar.Scroll += this.PreviewScrollBar_Scroll;
            owner.PreviewPicture.MouseLeave += this.PreviewPicture_MouseLeave;
            owner.PreviewPicture.DoubleClick += this.PreviewPicture_DoubleClick;
        }

        private delegate bool UrlCreatorDelegate(GetUrlArgs args);

        private delegate bool ImageCreatorDelegate(CreateImageArgs args);

        private PostClass _curPost
        {
            get { return this.tweenMain.CurPost; }
        }

        public void GenThumbnail(long id, List<string> links, PostClass.StatusGeo geo, Dictionary<string, string> media)
        {
            if (!this.tweenMain.IsPreviewEnable)
            {
                this.tweenMain.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            if (this.tweenMain.PreviewPicture.Image != null)
            {
                this.tweenMain.PreviewPicture.Image.Dispose();
                this.tweenMain.PreviewPicture.Image = null;
                this.tweenMain.SplitContainer3.Panel2Collapsed = true;
            }

            if (links.Count == 0 && geo == null && (media == null || media.Count == 0))
            {
                this.tweenMain.PreviewScrollBar.Maximum = 0;
                this.tweenMain.PreviewScrollBar.Enabled = false;
                this.tweenMain.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            if (media != null && media.Count > 0)
            {
                foreach (var link in links.ToArray())
                {
                    if (media.ContainsKey(link))
                    {
                        links.Remove(link);
                    }
                }
            }

            var imglist = new List<KeyValuePair<string, string>>();
            var dlg = new List<KeyValuePair<string, ImageCreatorDelegate>>();

            foreach (string url in links)
            {
                foreach (ThumbnailService svc in this.thumbnailServices)
                {
                    GetUrlArgs args = new GetUrlArgs() { Url = url, ImgList = imglist };
                    if (svc.UrlCreator(args))
                    {
                        // URLに対応したサムネイル作成処理デリゲートをリストに登録
                        dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(url, svc.ImageCreator));
                        break;
                    }
                }
            }

            if (media != null)
            {
                foreach (var m in media)
                {
                    foreach (ThumbnailService svc in this.thumbnailServices)
                    {
                        if (svc.UrlCreator(new GetUrlArgs() { Url = m.Key, Extended = m.Value, ImgList = imglist }))
                        {
                            // URLに対応したサムネイル作成処理デリゲートをリストに登録
                            dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(m.Key, svc.ImageCreator));
                            break;
                        }
                    }
                }
            }

            if (geo != null)
            {
                GetUrlArgs args = new GetUrlArgs() { Url = string.Empty, ImgList = imglist, GeoInfo = new Google.GlobalLocation { Latitude = geo.Lat, Longitude = geo.Lng } };
                if (TwitterGeo_GetUrl(args))
                {
                    // URLに対応したサムネイル作成処理デリゲートをリストに登録
                    dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(args.Url, new ImageCreatorDelegate(TwitterGeo_CreateImage)));
                }
            }

            if (imglist.Count == 0)
            {
                this.tweenMain.PreviewScrollBar.Maximum = 0;
                this.tweenMain.PreviewScrollBar.Enabled = false;
                this.tweenMain.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            this.ThumbnailProgressChanged(0);
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += this.Bgw_DoWork;
            bgw.RunWorkerCompleted += this.Bgw_Completed;
            bgw.RunWorkerAsync(new PreviewData(id, imglist, dlg));
        }

        public void ScrollThumbnail(bool forward)
        {
            if (forward)
            {
                this.tweenMain.PreviewScrollBar.Value = Math.Min(this.tweenMain.PreviewScrollBar.Value + 1, this.tweenMain.PreviewScrollBar.Maximum);
                this.PreviewScrollBar_Scroll(this.tweenMain.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, this.tweenMain.PreviewScrollBar.Value));
            }
            else
            {
                this.tweenMain.PreviewScrollBar.Value = Math.Max(this.tweenMain.PreviewScrollBar.Value - 1, this.tweenMain.PreviewScrollBar.Minimum);
                this.PreviewScrollBar_Scroll(this.tweenMain.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, this.tweenMain.PreviewScrollBar.Value));
            }
        }

        public void OpenPicture()
        {
            if (this.preview != null)
            {
                if (this.tweenMain.PreviewScrollBar.Value < this.preview.Pics.Count)
                {
                    this.tweenMain.OpenUriAsync(this.preview.Pics[this.tweenMain.PreviewScrollBar.Value].Key);
                }
            }
        }

        private static bool IsDirectLink(string url)
        {
            return Regex.Match(url, "^http://.*(\\.jpg|\\.jpeg|\\.gif|\\.png|\\.bmp)$", RegexOptions.IgnoreCase).Success;
        }

        #region "テンプレ"

#if template
		/// <summary>
		/// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
		/// </summary>
		/// <param name="args">Class GetUrlArgs
		///                                 args.url        URL文字列
		///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
		/// </param>
		/// <returns>成功した場合True,失敗の場合False</returns>
		/// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

		private bool ServiceName_GetUrl(GetUrlArgs args)
		{
			// TODO URL判定処理を記述
			Match mc = Regex.Match(args.url, "^http://imgur\\.com/(\\w+)\\.jpg$", RegexOptions.IgnoreCase);
			if (mc.Success) {
				// TODO 成功時はサムネイルURLを作成しimglist.Addする
				args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://i.imgur.com/${1}l.jpg")));
				return true;
			} else {
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
		private bool ServiceName_CreateImage(CreateImageArgs args)
		{
			// TODO: サムネイル画像読み込み処理を記述します
			Image img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, ref args.errmsg);
			if (img == null) {
				return false;
			}
			// 成功した場合はURLに対応する画像、ツールチップテキストを登録
			args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
			args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, string.Empty));
			return true;
		}
#endif

        #endregion "テンプレ"

        #region "ImgUr"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool ImgUr_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://imgur\\.com/(\\w+)\\.jpg$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://i.imgur.com/${1}l.jpg")));
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
        private static bool ImgUr_CreateImage(CreateImageArgs args)
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

        #endregion "ImgUr"

        #region "画像直リンク"

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

        #region "TwitPic"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool TwitPic_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(www\\.)?twitpic\\.com/(?<photoId>\\w+)(/full/?)?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://twitpic.com/show/thumb/${photoId}")));
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
        private static bool TwitPic_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "TwitPic"

        #region "yfrog"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Yfrog_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://yfrog\\.com/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended + ".th.jpg"));
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
        private static bool Yfrog_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "yfrog"

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
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^(http://tweetphoto\\.com/[0-9]+|http://pic\\.gd/[a-z0-9]+|http://(lockerz|plixi)\\.com/[ps]/[0-9]+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                const string Api = "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url=";
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, Api + (string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended)));
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
        private static bool Plixi_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            string referer = string.Empty;
            if (args.Url.Key.Contains("t.co"))
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
            else
            {
                referer = args.Url.Key;
            }

            Image img = (new HttpVarious()).GetImage(args.Url.Value, referer, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "Plixi(TweetPhoto)"

        #region "MobyPicture"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool MobyPicture_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://moby\\.to/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://mobypicture.com/?${1}:small")));
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
        private static bool MobyPicture_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "MobyPicture"

        #region "携帯百景"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool MovaPic_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://movapic\\.com/pic/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://image.movapic.com/pic/s_${1}.jpeg")));
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
        private static bool MovaPic_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "携帯百景"

        #region "はてなフォトライフ"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Hatena_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://f\\.hatena\\.ne\\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\\d{8})\\d+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg")));
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
        private static bool Hatena_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "はてなフォトライフ"

        #region "PhotoShare"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool PhotoShare_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(?:www\\.)?bcphotoshare\\.com/photos/\\d+/(\\d+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://images.bcphotoshare.com/storages/${1}/thumb180.jpg")));
                return true;
            }

            // 短縮URL
            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://bctiny\\.com/p(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                try
                {
                    args.ImgList.Add(new KeyValuePair<string, string>(args.Url, "http://images.bcphotoshare.com/storages/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString() + "/thumb180.jpg"));
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
        private static bool PhotoShare_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "PhotoShare"

        #region "img.ly"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Imgly_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://img\\.ly/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://img.ly/show/thumb/${1}")));
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
        private static bool Imgly_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "img.ly"

        #region "brightkite"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Brightkite_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://brightkite\\.com/objects/((\\w{2})(\\w{2})\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://cdn.brightkite.com/${2}/${3}/${1}-feed.jpg")));
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
        private static bool Brightkite_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "brightkite"

        #region "Twitgoo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Twitgoo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://twitgoo\\.com/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://twitgoo.com/${1}/mini")));
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
        private static bool Twitgoo_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "Twitgoo"

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
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www\\.youtube\\.com/watch\\?v=([\\w\\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("${0}")));
                return true;
            }

            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://youtu\\.be/([\\w\\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("${0}")));
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
            // TODO: サムネイル画像読み込み処理を記述します
            // 参考
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
            // デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries
            // デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
            string imgurl = string.Empty;
            Match mcimg = Regex.Match(args.Url.Value, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            if (mcimg.Success)
            {
                imgurl = mcimg.Result("http://i.ytimg.com/vi/${videoid}/default.jpg");
            }
            else
            {
                return false;
            }

            string videourl = (new HttpVarious()).GetRedirectTo(args.Url.Value);
            Match mc = Regex.Match(videourl, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            if (videourl.StartsWith("http://www.youtube.com/index?ytsession="))
            {
                videourl = args.Url.Value;
                mc = Regex.Match(videourl, "^http://(?:(www\\.youtube\\.com)|(youtu\\.be))/(watch\\?v=)?(?<videoid>([\\w\\-]+))", RegexOptions.IgnoreCase);
            }

            if (mc.Success)
            {
                string apiurl = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups["videoid"].Value;
                string src = string.Empty;
                if ((new HttpVarious()).GetData(apiurl, null, ref src, 5000))
                {
                    StringBuilder sb = new StringBuilder();
                    XmlDocument xdoc = new XmlDocument();
                    try
                    {
                        xdoc.LoadXml(src);
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                        nsmgr.AddNamespace("root", "http://www.w3.org/2005/Atom");
                        nsmgr.AddNamespace("app", "http://purl.org/atom/app#");
                        nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");

                        XmlNode xentryNode = xdoc.DocumentElement.SelectSingleNode("/root:entry/media:group", nsmgr);
                        XmlElement xentry = (XmlElement)xentryNode;
                        string tmp = string.Empty;
                        try
                        {
                            tmp = xentry["media:title"].InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Hoehoe.Properties.Resources.YouTubeInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            int sec = 0;
                            if (int.TryParse(xentry["yt:duration"].Attributes["seconds"].Value, out sec))
                            {
                                sb.Append(Hoehoe.Properties.Resources.YouTubeInfoText2);
                                sb.AppendFormat("{0:d}:{1:d2}", sec / 60, sec % 60);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            DateTime tmpdate = new DateTime();
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            if (DateTime.TryParse(xentry["published"].InnerText, out tmpdate))
                            {
                                sb.Append(Hoehoe.Properties.Resources.YouTubeInfoText3);
                                sb.Append(tmpdate);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            int count = 0;
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            tmp = xentry["yt:statistics"].Attributes["viewCount"].Value;
                            if (int.TryParse(tmp, out count))
                            {
                                sb.Append(Hoehoe.Properties.Resources.YouTubeInfoText4);
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
                                sb.Append(xentry["yt:state"].Attributes["name"].Value);
                                sb.Append(":");
                                sb.Append(xentry["yt:state"].InnerText);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (!string.IsNullOrEmpty(imgurl))
                    {
                        HttpVarious http = new HttpVarious();
                        Image img = http.GetImage(imgurl, videourl, 10000, ref args.Errmsg);
                        if (img == null)
                        {
                            return false;
                        }

                        args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                        args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, sb.ToString().Trim()));
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion "youtube"

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
        private static bool Nicovideo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText1);
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText2);
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText3);
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText4);
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText5);
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
                                sb.Append(Hoehoe.Properties.Resources.NiconicoInfoText6);
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
                        string errcode = xdoc.SelectSingleNode("/nicovideo_thumb_response/error/code").InnerText;
                        args.Errmsg = errcode;
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

        #endregion "ニコニコ動画"

        #region "ニコニコ静画"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Nicoseiga_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(?:seiga\\.nicovideo\\.jp/seiga/|nico\\.ms/)im\\d+");
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool Nicoseiga_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^http://(?:seiga\\.nicovideo\\.jp/seiga/|nico\\.ms/)im(?<id>\\d+)");
            if (mc.Success)
            {
                Image img = http.GetImage("http://lohas.nicoseiga.jp/thumb/" + mc.Groups["id"].Value + "q?", args.Url.Key, 0, ref args.Errmsg);
                if (img == null)
                {
                    return false;
                }

                args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                return true;
            }

            return false;
        }

        #endregion "ニコニコ静画"

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
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www\\.pixiv\\.net/(member_illust|index)\\.php\\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url.Replace("amp;", string.Empty), mc.Value));
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
        private static bool Pixiv_CreateImage(CreateImageArgs args)
        {
            // illustIDをキャプチャ
            Match mc = Regex.Match(args.Url.Value, "^http://www\\.pixiv\\.net/(member_illust|index)\\.php\\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase);
            if (mc.Groups["tag"].Value == "R-18" || mc.Groups["tag"].Value == "R-18G")
            {
                args.Errmsg = "NotSupported";
                return false;
            }

            HttpVarious http = new HttpVarious();
            string src = string.Empty;
            if (http.GetData(Regex.Replace(mc.Groups[0].Value, "amp;", string.Empty), null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                Match mc2 = Regex.Match(src, mc.Result("http://img([0-9]+)\\.pixiv\\.net/img/.+/${illustId}_[ms]\\.([a-zA-Z]+)"));
                if (mc2.Success)
                {
                    Image img = http.GetImage(mc2.Value, args.Url.Value, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }

                if (Regex.Match(src, "<span class='error'>ログインしてください</span>").Success)
                {
                    args.Errmsg = "NotSupported";
                }
                else
                {
                    args.Errmsg = "Pattern NotFound";
                }
            }

            return false;
        }

        #endregion "Pixiv"

        #region "flickr"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Flickr_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www.flickr.com/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended));
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
        private static bool Flickr_CreateImage(CreateImageArgs args)
        {
            /*
            // 参考: http://tanarky.blogspot.com/2010/03/flickr-urlunavailable.html アグレッシブエンジニア: flickr の画像URL仕様についてまとめ(Unavailable画像)
            // 画像URL仕様　http://farm{farm}.static.flickr.com/{server}/{id}_{secret}_{size}.{extension}
            // photostreamなど複数の画像がある場合先頭の一つのみ認識と言うことにする
            // (二つ目のキャプチャ 一つ目の画像はユーザーアイコン）
            */

            string src = string.Empty;
            Match mc = Regex.Match(args.Url.Value, "^http://www.flickr.com/", RegexOptions.IgnoreCase);
            HttpVarious http = new HttpVarious();
            if (http.GetData(args.Url.Value, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                MatchCollection mc2 = Regex.Matches(src, mc.Result("http://farm[0-9]+\\.static\\.flickr\\.com/[0-9]+/.+?\\.([a-zA-Z]+)"));

                // 二つ以上キャプチャした場合先頭の一つだけ 一つだけの場合はユーザーアイコンしか取れなかった
                if (mc2.Count > 1)
                {
                    Image img = http.GetImage(mc2[1].Value, args.Url.Value, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }

                args.Errmsg = "Pattern NotFound";
            }

            return false;
        }

        #endregion "flickr"

        #region "フォト蔵"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Photozou_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://photozou\\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase);
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
        private static bool Photozou_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^http://photozou\\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                string src = string.Empty;
                string show_info = mc.Result("http://api.photozou.jp/rest/photo_info?photo_id=${photoId}");
                if (http.GetData(show_info, null, ref src, 0, ref args.Errmsg, string.Empty))
                {
                    XmlDocument xdoc = new XmlDocument();
                    string thumbnailUrl = string.Empty;
                    try
                    {
                        xdoc.LoadXml(src);
                        thumbnailUrl = xdoc.SelectSingleNode("/rsp/info/photo/thumbnail_image_url").InnerText;
                    }
                    catch (Exception ex)
                    {
                        args.Errmsg = ex.Message;
                        thumbnailUrl = string.Empty;
                    }

                    if (string.IsNullOrEmpty(thumbnailUrl))
                    {
                        return false;
                    }

                    Image img = http.GetImage(thumbnailUrl, args.Url.Key);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }
            }

            return false;
        }

        #endregion "フォト蔵"

        #region "TwitVideo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool TwitVideo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://twitvideo\\.jp/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://twitvideo.jp/img/thumb/${1}")));
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
        private static bool TwitVideo_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "TwitVideo"

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
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://piapro\\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\\-]+)$");
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool Piapro_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^http://piapro\\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\\-]+)$");
            if (mc.Success)
            {
                string src = string.Empty;
                if (http.GetData(args.Url.Key, null, ref src, 0, ref args.Errmsg, string.Empty))
                {
                    Match mc2 = Regex.Match(src, "<meta property=\"og:image\" content=\"(?<big_img>http://c1\\.piapro\\.jp/timg/[0-9a-z]+_\\d{14}_0500_0500\\.(?:jpg|png|gif)?)\" />");
                    if (mc2.Success)
                    {
                        // 各画像には120x120のサムネイルがある（多分）ので、URLを置き換える。元々ページに埋め込まれている画像は500x500
                        Regex r = new Regex("_\\d{4}_\\d{4}");
                        string minImgUrl = r.Replace(mc2.Groups["big_img"].Value, "_0120_0120");
                        Image img = http.GetImage(minImgUrl, args.Url.Key, 0, ref args.Errmsg);
                        if (img == null)
                        {
                            return false;
                        }

                        args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                        args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                        return true;
                    }

                    args.Errmsg = "Pattern NotFound";
                }
            }

            return false;
        }

        #endregion "Piapro"

        #region "Tumblr"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Tumblr_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(.+\\.)?tumblr\\.com/.+/?", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool Tumblr_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            string targetUrl = args.Url.Value;
            string tmp = http.GetRedirectTo(targetUrl);
            while (!targetUrl.Equals(tmp))
            {
                targetUrl = tmp;
                tmp = http.GetRedirectTo(targetUrl);
            }

            Match mc = Regex.Match(targetUrl, "(?<base>http://.+?\\.tumblr\\.com/)post/(?<postID>[0-9]+)(/(?<subject>.+?)/)?", RegexOptions.IgnoreCase);
            string apiurl = mc.Groups["base"].Value + "api/read?id=" + mc.Groups["postID"].Value;
            string src = string.Empty;
            string imgurl = null;
            if (http.GetData(apiurl, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                XmlDocument xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);
                    string type = xdoc.SelectSingleNode("/tumblr/posts/post").Attributes["type"].Value;
                    if (type == "photo")
                    {
                        imgurl = xdoc.SelectSingleNode("/tumblr/posts/post/photo-url").InnerText;
                    }
                    else
                    {
                        args.Errmsg = "PostType:" + type;
                        imgurl = string.Empty;
                    }
                }
                catch (Exception)
                {
                    imgurl = string.Empty;
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    Image img = http.GetImage(imgurl, args.Url.Key, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }
            }

            return false;
        }

        #endregion "Tumblr"

        #region "ついっぷるフォト"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool TwipplePhoto_GetUrl(GetUrlArgs args)
        {
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://p\\.twipple\\.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase);
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
        private static bool TwipplePhoto_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^http://p.twipple.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                string src = string.Empty;
                if (http.GetData(args.Url.Key, null, ref src, 0, ref args.Errmsg, string.Empty))
                {
                    string thumbnailUrl = string.Empty;
                    string contentId = mc.Groups["contentId"].Value;
                    StringBuilder dataDir = new StringBuilder();

                    // DataDir作成
                    dataDir.Append("data");
                    for (int i = 0; i < contentId.Length; i++)
                    {
                        dataDir.Append("/");
                        dataDir.Append(contentId[i]);
                    }

                    // サムネイルURL抽出
                    thumbnailUrl = Regex.Match(src, "http://p\\.twipple\\.jp/" + dataDir.ToString() + "_s\\.([a-zA-Z]+)").Value;

                    if (string.IsNullOrEmpty(thumbnailUrl))
                    {
                        return false;
                    }

                    Image img = http.GetImage(thumbnailUrl, args.Url.Key, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }
            }

            return false;
        }

        #endregion "ついっぷるフォト"

        #region "mypix/shamoji"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Mypix_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://(www\\.mypix\\.jp|www\\.shamoji\\.info)/app\\.php/picture/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value + "/thumb.jpg"));
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
        private static bool Mypix_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "mypix/shamoji"

        #region "ow.ly"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Owly_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://ow\\.ly/i/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://static.ow.ly/photos/thumb/${1}.jpg")));
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
        private static bool Owly_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "ow.ly"

        #region "vimeo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Vimeo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://vimeo\\.com/[0-9]+", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool Vimeo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "http://vimeo\\.com/(?<postID>[0-9]+)", RegexOptions.IgnoreCase);
            string apiurl = "http://vimeo.com/api/v2/video/" + mc.Groups["postID"].Value + ".xml";
            string src = string.Empty;
            string imgurl = null;
            if (http.GetData(apiurl, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                XmlDocument xdoc = new XmlDocument();
                StringBuilder sb = new StringBuilder();
                try
                {
                    xdoc.LoadXml(src);
                    try
                    {
                        string tmp = xdoc.SelectSingleNode("videos/video/title").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText1);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        DateTime tmpdate = new DateTime();
                        if (DateTime.TryParse(xdoc.SelectSingleNode("videos/video/upload_date").InnerText, out tmpdate))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText2);
                            sb.Append(tmpdate);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        string tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_likes").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText3);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        string tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_plays").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText4);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        string tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_comments").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText5);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        int sec = 0;
                        if (int.TryParse(xdoc.SelectSingleNode("videos/video/duration").InnerText, out sec))
                        {
                            sb.Append(Hoehoe.Properties.Resources.VimeoInfoText6);
                            sb.AppendFormat("{0:d}:{1:d2}", sec / 60, sec % 60);
                            sb.AppendLine();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        string tmp = xdoc.SelectSingleNode("videos/video/thumbnail_medium").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            imgurl = tmp;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception)
                {
                    imgurl = string.Empty;
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

        #endregion "vimeo"

        #region "cloudfiles"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool CloudFiles_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://c[0-9]+\\.cdn[0-9]+\\.cloudfiles\\.rackspacecloud\\.com/[a-z_0-9]+", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool CloudFiles_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "cloudfiles"

        #region "Instagram"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Instagram_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://instagr.am/p/.+/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
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
        private static bool Instagram_CreateImage(CreateImageArgs args)
        {
            string src = string.Empty;
            HttpVarious http = new HttpVarious();
            if (http.GetData(args.Url.Value, null, ref src, 0, ref args.Errmsg, string.Empty))
            {
                Match mc = Regex.Match(src, "<meta property=\"og:image\" content=\"(?<url>.+)\" ?/>");
                if (mc.Success)
                {
                    Image img = http.GetImage(mc.Groups["url"].Value, args.Url.Key, 0, ref args.Errmsg);
                    if (img == null)
                    {
                        return false;
                    }

                    args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                    args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                    return true;
                }

                args.Errmsg = "Pattern NotFound";
            }

            return false;
        }

        #endregion "Instagram"

        #region "pikubo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Pikubo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://pikubo\\.me/([a-z0-9-_]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Result("http://pikubo.me/q/${1}")));
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
        private static bool Pikubo_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 0, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            return true;
        }

        #endregion "pikubo"

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

        #region "Foursquare"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Foursquare_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^https?://(4sq|foursquare).com/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                if (!AppendSettingDialog.Instance.IsPreviewFoursquare)
                {
                    return false;
                }

                args.ImgList.Add(new KeyValuePair<string, string>(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, string.Empty));
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
        private static bool Foursquare_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            string tipsText = string.Empty;
            string mapsUrl = Foursquare.GetInstance.GetMapsUri(args.Url.Key, ref tipsText);
            if (mapsUrl == null)
            {
                return false;
            }

            Image img = (new HttpVarious()).GetImage(mapsUrl, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, tipsText));
            return true;
        }

        #endregion "Foursquare"

        #region "Twitter Geo"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool TwitterGeo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            if (args.GeoInfo != null && (args.GeoInfo.Latitude != 0 || args.GeoInfo.Longitude != 0))
            {
                string url = (new Google()).CreateGoogleStaticMapsUri(args.GeoInfo);
                args.ImgList.Add(new KeyValuePair<string, string>(url, url));
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
        private static bool TwitterGeo_CreateImage(CreateImageArgs args)
        {
            Image img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            string url = args.Url.Value;
            try
            {
                // URLをStaticMapAPIから通常のURLへ変換
                // 仕様：ズーム率、サムネイルサイズの設定は無視する
                // 参考：http://imakoko.didit.jp/imakoko_html/memo/parameters_google.html
                // サンプル
                // static版 http://maps.google.com/maps/api/staticmap?center=35.16959869,136.93813205&size=300x300&zoom=15&markers=35.16959869,136.93813205&sensor=false
                // 通常URL  http://maps.google.com/maps?ll=35.16959869,136.93813205&size=300x300&zoom=15&markers=35.16959869,136.93813205&sensor=false
                url = url.Replace("/maps/api/staticmap?center=", "?ll=");
                url = url.Replace("&markers=", "&q=");
                url = Regex.Replace(url, "&size=\\d+x\\d+&zoom=\\d+", string.Empty);
                url = url.Replace("&sensor=false", string.Empty);
            }
            catch (Exception)
            {
                url = args.Url.Value;
            }

            args.Pics.Add(new KeyValuePair<string, Image>(url, img));
            args.TooltipText.Add(new KeyValuePair<string, string>(url, string.Empty));
            return true;
        }

        #endregion "Twitter Geo"

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
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://www\\.tinami\\.com/view/\\d+$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.ImgList.Add(new KeyValuePair<string, string>(args.Url, mc.Value));
                return true;
            }

            // 短縮URL
            mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^http://tinami\\.jp/(\\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                try
                {
                    args.ImgList.Add(new KeyValuePair<string, string>(args.Url, "http://www.tinami.com/view/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString()));
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
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^http://www\\.tinami\\.com/view/(?<ContentId>\\d+)$", RegexOptions.IgnoreCase);
            const string ApiKey = "4e353d9113dce";
            if (mc.Success)
            {
                string src = string.Empty;
                string contentInfo = mc.Result("http://api.tinami.com/content/info?api_key=" + ApiKey + "&cont_id=${ContentId}");
                if (http.GetData(contentInfo, null, ref src, 0, ref args.Errmsg, string.Empty))
                {
                    XmlDocument xdoc = new XmlDocument();
                    string thumbnailUrl = string.Empty;
                    try
                    {
                        xdoc.LoadXml(src);
                        var stat = xdoc.SelectSingleNode("/rsp").Attributes.GetNamedItem("stat").InnerText;
                        if (stat == "ok")
                        {
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

                                args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                                args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
                                return true;
                            }
                            else
                            {
                                // エラー処理 エラーメッセージが返ってきた場合はここで処理
                                if (xdoc.SelectSingleNode("/rsp/err") != null)
                                {
                                    args.Errmsg = xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText;
                                }

                                return false;
                            }
                        }
                        else
                        {
                            if (xdoc.SelectSingleNode("/rsp/err") != null)
                            {
                                args.Errmsg = xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText;
                            }
                            else
                            {
                                args.Errmsg = "DeletedOrSuspended";
                            }

                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        args.Errmsg = ex.Message;
                        return false;
                    }
                }
            }

            return false;
        }

        #endregion "TINAMI"

        #region "Twitter公式"

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">Class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>
        private static bool Twimg_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            Match mc = Regex.Match(string.IsNullOrEmpty(args.Extended) ? args.Url : args.Extended, "^https?://p\\.twimg\\.com/.*$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
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
        private static bool Twimg_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            HttpVarious http = new HttpVarious();
            Match mc = Regex.Match(args.Url.Value, "^https?://p\\.twimg\\.com/.*$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                string src = string.Empty;
                string contentInfo = args.Url.Value + ":thumb";
                var img = http.GetImage(contentInfo, src, 0, ref args.Errmsg);
                if (img == null)
                {
                    return false;
                }

                args.Pics.Add(new KeyValuePair<string, Image>(args.Url.Key, img));
                args.TooltipText.Add(new KeyValuePair<string, string>(args.Url.Key, string.Empty));
            }

            return false;
        }

        #endregion "Twitter公式"

        private void ThumbnailProgressChanged(int pp, string addMsg = "")
        {
            // 開始
            if (pp == 0)
            {
                // Owner.SetStatusLabel("Thumbnail generating...")
                // 正常終了
            }
            else if (pp == 100)
            {
                // Owner.SetStatusLabel("Thumbnail generated.")
                //  エラー
            }
            else
            {
                if (string.IsNullOrEmpty(addMsg))
                {
                    this.tweenMain.SetStatusLabel("can't get Thumbnail.");
                }
                else
                {
                    this.tweenMain.SetStatusLabel("can't get Thumbnail.(" + addMsg + ")");
                }
            }
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            PreviewData arg = (PreviewData)e.Argument;
            arg.AdditionalErrorMessage = string.Empty;

            foreach (KeyValuePair<string, string> url in arg.Urls)
            {
                CreateImageArgs args = new CreateImageArgs() { Url = url, Pics = arg.Pics, TooltipText = arg.TooltipText, Errmsg = string.Empty };
                if (!arg.ImageCreators[arg.Urls.IndexOf(url)].Value(args))
                {
                    arg.AdditionalErrorMessage = args.Errmsg;
                    arg.IsError = true;
                }
            }

            arg.IsError = arg.Pics.Count == 0;
            e.Result = arg;
        }

        private void Bgw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            PreviewData prv = e.Result as PreviewData;
            if (prv == null || prv.IsError)
            {
                this.tweenMain.PreviewScrollBar.Maximum = 0;
                this.tweenMain.PreviewScrollBar.Enabled = false;
                this.tweenMain.SplitContainer3.Panel2Collapsed = true;
                if (prv != null && !string.IsNullOrEmpty(prv.AdditionalErrorMessage))
                {
                    this.ThumbnailProgressChanged(-1, prv.AdditionalErrorMessage);
                }
                else
                {
                    this.ThumbnailProgressChanged(-1);
                }

                return;
            }

            lock (this.lckPrev)
            {
                if (prv != null && this._curPost != null && prv.StatusId == this._curPost.StatusId)
                {
                    this.preview = prv;
                    this.tweenMain.SplitContainer3.Panel2Collapsed = false;
                    this.tweenMain.PreviewScrollBar.Maximum = this.preview.Pics.Count - 1;
                    this.tweenMain.PreviewScrollBar.Enabled = this.tweenMain.PreviewScrollBar.Maximum > 0;
                    this.tweenMain.PreviewScrollBar.Value = 0;
                    this.tweenMain.PreviewPicture.Image = this.preview.Pics[0].Value;
                    string prevtooltipTextValue = this.preview.TooltipText[0].Value;
                    this.tweenMain.ToolTip1.SetToolTip(this.tweenMain.PreviewPicture, string.IsNullOrEmpty(prevtooltipTextValue) ? string.Empty : prevtooltipTextValue);
                }
                else if (this._curPost == null || (this.preview != null && this._curPost.StatusId != this.preview.StatusId))
                {
                    this.tweenMain.PreviewScrollBar.Maximum = 0;
                    this.tweenMain.PreviewScrollBar.Enabled = false;
                    this.tweenMain.SplitContainer3.Panel2Collapsed = true;
                }
            }

            this.ThumbnailProgressChanged(100);
        }

        private void PreviewScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            lock (this.lckPrev)
            {
                if (this.preview != null && this._curPost != null && this.preview.StatusId == this._curPost.StatusId)
                {
                    if (this.preview.Pics.Count > e.NewValue)
                    {
                        this.tweenMain.PreviewPicture.Image = this.preview.Pics[e.NewValue].Value;
                        if (!string.IsNullOrEmpty(this.preview.TooltipText[e.NewValue].Value))
                        {
                            this.tweenMain.ToolTip1.Hide(this.tweenMain.PreviewPicture);
                            this.tweenMain.ToolTip1.SetToolTip(this.tweenMain.PreviewPicture, this.preview.TooltipText[e.NewValue].Value);
                        }
                        else
                        {
                            this.tweenMain.ToolTip1.SetToolTip(this.tweenMain.PreviewPicture, string.Empty);
                            this.tweenMain.ToolTip1.Hide(this.tweenMain.PreviewPicture);
                        }
                    }
                }
            }
        }

        private void PreviewPicture_MouseLeave(object sender, EventArgs e)
        {
            this.tweenMain.ToolTip1.Hide(this.tweenMain.PreviewPicture);
        }

        private void PreviewPicture_DoubleClick(object sender, EventArgs e)
        {
            this.OpenPicture();
        }

        private class PreviewData : IDisposable
        {
            // 重複する呼び出しを検出するには
            private bool disposedValue;

            public PreviewData(long id, List<KeyValuePair<string, string>> urlList, List<KeyValuePair<string, ImageCreatorDelegate>> imageCreatorList)
            {
                this.StatusId = id;
                this.Urls = urlList;
                this.ImageCreators = imageCreatorList;
                this.Pics = new List<KeyValuePair<string, Image>>();
                this.TooltipText = new List<KeyValuePair<string, string>>();
            }

            public long StatusId { get; private set; }

            public List<KeyValuePair<string, string>> Urls { get; private set; }

            public List<KeyValuePair<string, Image>> Pics { get; private set; }

            public List<KeyValuePair<string, string>> TooltipText { get; private set; }

            public List<KeyValuePair<string, ImageCreatorDelegate>> ImageCreators { get; private set; }

            public bool IsError { get; set; }

            public string AdditionalErrorMessage { get; set; }

            #region " IDisposable Support "

            // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 明示的に呼び出されたときにマネージ リソースを解放します
                    }

                    // TODO: 共有のアンマネージ リソースを解放します
                    foreach (KeyValuePair<string, Image> pic in this.Pics)
                    {
                        if (pic.Value != null)
                        {
                            pic.Value.Dispose();
                        }
                    }
                }

                this.disposedValue = true;
            }

            #endregion " IDisposable Support "
        }

        private class GetUrlArgs
        {
            public string Url { get; set; }

            public string Extended { get; set; }

            public List<KeyValuePair<string, string>> ImgList { get; set; }

            public Google.GlobalLocation GeoInfo { get; set; }
        }

        private class CreateImageArgs
        {
            public KeyValuePair<string, string> Url { get; set; }

            public List<KeyValuePair<string, Image>> Pics { get; set; }

            public List<KeyValuePair<string, string>> TooltipText { get; set; }

            public string Errmsg;
        }

        private class ThumbnailService
        {
            public string Name { get; private set; }

            public UrlCreatorDelegate UrlCreator { get; private set; }

            public ImageCreatorDelegate ImageCreator { get; private set; }

            public ThumbnailService(string name, UrlCreatorDelegate urlcreator, ImageCreatorDelegate imagecreator)
            {
                this.Name = name;
                this.UrlCreator = urlcreator;
                this.ImageCreator = imagecreator;
            }
        }

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