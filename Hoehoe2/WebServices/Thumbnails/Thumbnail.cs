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
    using System.Windows.Forms;

    public partial class Thumbnail
    {
        private readonly object _lckPrev = new object();
        private PreviewData _preview;
        private readonly TweenMain _tweenMain;

        private readonly ThumbnailService[] _thumbnailServices =
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
            new ThumbnailService("Twimg", Twimg_GetUrl, Twimg_CreateImage),
            new ThumbnailService("TwitrPix", TwitrPix_GetUrl, TwitrPix_CreateImage),
            new ThumbnailService("Pckles", Pckles_GetUrl, Pckles_CreateImage),
            new ThumbnailService("via.me", ViaMe_GetUrl, ViaMe_CreateImage),
            new ThumbnailService("tiqav", Tiqav_GetUrl, Tiqav_CreateImage),
            new ThumbnailService("miilme", MiilMe_GetUrl, MiilMe_CreateImage),
            new ThumbnailService("StreamZoo", StreamZoo_GetUrl, StreamZoo_CreateImage),
            new ThumbnailService("My365", My365_GetUrl, My365_CreateImage),
            new ThumbnailService("Path", Path_GetUrl, Path_CreateImage)
        };

        public Thumbnail(TweenMain owner)
        {
            _tweenMain = owner;

            owner.PreviewScrollBar.Scroll += PreviewScrollBar_Scroll;
            owner.PreviewPicture.MouseLeave += PreviewPicture_MouseLeave;
            owner.PreviewPicture.DoubleClick += PreviewPicture_DoubleClick;
        }

        private delegate bool UrlCreatorDelegate(GetUrlArgs args);

        private delegate bool ImageCreatorDelegate(CreateImageArgs args);

        private PostClass _curPost
        {
            get { return _tweenMain.CurPost; }
        }

        public void GenThumbnail(long id, List<string> links, PostClass.StatusGeo geo, Dictionary<string, string> media)
        {
            if (!_tweenMain.IsPreviewEnable)
            {
                _tweenMain.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            if (_tweenMain.PreviewPicture.Image != null)
            {
                _tweenMain.PreviewPicture.Image.Dispose();
                _tweenMain.PreviewPicture.Image = null;
                _tweenMain.SplitContainer3.Panel2Collapsed = true;
            }

            if (links.Count == 0 && geo == null && (media == null || media.Count == 0))
            {
                _tweenMain.PreviewScrollBar.Maximum = 0;
                _tweenMain.PreviewScrollBar.Enabled = false;
                _tweenMain.SplitContainer3.Panel2Collapsed = true;
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
                foreach (var svc in _thumbnailServices)
                {
                    if (svc.UrlCreator(new GetUrlArgs { Url = url, ImgList = imglist }))
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
                    foreach (var svc in _thumbnailServices)
                    {
                        if (svc.UrlCreator(new GetUrlArgs { Url = m.Key, Extended = m.Value, ImgList = imglist }))
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
                var args = new GetUrlArgs { Url = string.Empty, ImgList = imglist, GeoInfo = new Google.GlobalLocation { Latitude = geo.Lat, Longitude = geo.Lng } };
                if (TwitterGeo_GetUrl(args))
                {
                    // URLに対応したサムネイル作成処理デリゲートをリストに登録
                    dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(args.Url, TwitterGeo_CreateImage));
                }
            }

            if (imglist.Count == 0)
            {
                _tweenMain.PreviewScrollBar.Maximum = 0;
                _tweenMain.PreviewScrollBar.Enabled = false;
                _tweenMain.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            ThumbnailProgressChanged(0);
            var bgw = new BackgroundWorker();
            bgw.DoWork += Bgw_DoWork;
            bgw.RunWorkerCompleted += Bgw_Completed;
            bgw.RunWorkerAsync(new PreviewData(id, imglist, dlg));
        }

        public void ScrollThumbnail(bool forward)
        {
            if (forward)
            {
                _tweenMain.PreviewScrollBar.Value = Math.Min(_tweenMain.PreviewScrollBar.Value + 1, _tweenMain.PreviewScrollBar.Maximum);
                PreviewScrollBar_Scroll(_tweenMain.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, _tweenMain.PreviewScrollBar.Value));
            }
            else
            {
                _tweenMain.PreviewScrollBar.Value = Math.Max(_tweenMain.PreviewScrollBar.Value - 1, _tweenMain.PreviewScrollBar.Minimum);
                PreviewScrollBar_Scroll(_tweenMain.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, _tweenMain.PreviewScrollBar.Value));
            }
        }

        public void OpenPicture()
        {
            if (_preview != null)
            {
                if (_tweenMain.PreviewScrollBar.Value < _preview.Pics.Count)
                {
                    _tweenMain.OpenUriAsync(_preview.Pics[_tweenMain.PreviewScrollBar.Value].Key);
                }
            }
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

        private static bool Default_CreateImage(CreateImageArgs args)
        {
            var img = (new HttpVarious()).GetImage(args.Url.Value, args.Url.Key, 10000, ref args.Errmsg);
            if (img == null)
            {
                return false;
            }

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.AddTooltipInfo(args.Url.Key, string.Empty, img);
            return true;
        }

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
                    _tweenMain.SetStatusLabel("can't get Thumbnail.");
                }
                else
                {
                    _tweenMain.SetStatusLabel("can't get Thumbnail.(" + addMsg + ")");
                }
            }
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var arg = (PreviewData)e.Argument;
            arg.AdditionalErrorMessage = string.Empty;

            foreach (var url in arg.Urls)
            {
                var args = new CreateImageArgs { Url = url, Pics = arg.Pics, TooltipText = arg.TooltipText, Errmsg = string.Empty };
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
            var prv = e.Result as PreviewData;
            if (prv == null || prv.IsError)
            {
                _tweenMain.PreviewScrollBar.Maximum = 0;
                _tweenMain.PreviewScrollBar.Enabled = false;
                _tweenMain.SplitContainer3.Panel2Collapsed = true;
                if (prv != null && !string.IsNullOrEmpty(prv.AdditionalErrorMessage))
                {
                    ThumbnailProgressChanged(-1, prv.AdditionalErrorMessage);
                }
                else
                {
                    ThumbnailProgressChanged(-1);
                }

                return;
            }

            lock (_lckPrev)
            {
                if (_curPost != null && prv.StatusId == _curPost.StatusId)
                {
                    _preview = prv;
                    _tweenMain.SplitContainer3.Panel2Collapsed = false;
                    _tweenMain.PreviewScrollBar.Maximum = _preview.Pics.Count - 1;
                    _tweenMain.PreviewScrollBar.Enabled = _tweenMain.PreviewScrollBar.Maximum > 0;
                    _tweenMain.PreviewScrollBar.Value = 0;
                    _tweenMain.PreviewPicture.Image = _preview.Pics[0].Value;
                    string prevtooltipTextValue = _preview.TooltipText[0].Value;
                    _tweenMain.ToolTip1.SetToolTip(_tweenMain.PreviewPicture, string.IsNullOrEmpty(prevtooltipTextValue) ? string.Empty : prevtooltipTextValue);
                }
                else if (_curPost == null || (_preview != null && _curPost.StatusId != _preview.StatusId))
                {
                    _tweenMain.PreviewScrollBar.Maximum = 0;
                    _tweenMain.PreviewScrollBar.Enabled = false;
                    _tweenMain.SplitContainer3.Panel2Collapsed = true;
                }
            }

            ThumbnailProgressChanged(100);
        }

        private void PreviewScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            lock (_lckPrev)
            {
                if (_preview != null && _curPost != null && _preview.StatusId == _curPost.StatusId)
                {
                    if (_preview.Pics.Count > e.NewValue)
                    {
                        _tweenMain.PreviewPicture.Image = _preview.Pics[e.NewValue].Value;
                        if (!string.IsNullOrEmpty(_preview.TooltipText[e.NewValue].Value))
                        {
                            _tweenMain.ToolTip1.Hide(_tweenMain.PreviewPicture);
                            _tweenMain.ToolTip1.SetToolTip(_tweenMain.PreviewPicture, _preview.TooltipText[e.NewValue].Value);
                        }
                        else
                        {
                            _tweenMain.ToolTip1.SetToolTip(_tweenMain.PreviewPicture, string.Empty);
                            _tweenMain.ToolTip1.Hide(_tweenMain.PreviewPicture);
                        }
                    }
                }
            }
        }

        private void PreviewPicture_MouseLeave(object sender, EventArgs e)
        {
            _tweenMain.ToolTip1.Hide(_tweenMain.PreviewPicture);
        }

        private void PreviewPicture_DoubleClick(object sender, EventArgs e)
        {
            OpenPicture();
        }

        private class PreviewData : IDisposable
        {
            // 重複する呼び出しを検出するには
            private bool disposedValue;

            public PreviewData(long id, List<KeyValuePair<string, string>> urlList, List<KeyValuePair<string, ImageCreatorDelegate>> imageCreatorList)
            {
                StatusId = id;
                Urls = urlList;
                ImageCreators = imageCreatorList;
                Pics = new List<KeyValuePair<string, Image>>();
                TooltipText = new List<KeyValuePair<string, string>>();
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
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 明示的に呼び出されたときにマネージ リソースを解放します
                    }

                    // TODO: 共有のアンマネージ リソースを解放します
                    foreach (KeyValuePair<string, Image> pic in Pics)
                    {
                        if (pic.Value != null)
                        {
                            pic.Value.Dispose();
                        }
                    }
                }

                disposedValue = true;
            }

            #endregion " IDisposable Support "
        }

        private class GetUrlArgs
        {
            public string Url { get; set; }

            public string Extended { get; set; }

            public List<KeyValuePair<string, string>> ImgList { get; set; }

            public Google.GlobalLocation GeoInfo { get; set; }

            public void AddThumbnailUrl(string baseUrl, string thumbnailUrl)
            {
                ImgList.Add(new KeyValuePair<string, string>(baseUrl, thumbnailUrl));
            }
        }

        private class CreateImageArgs
        {
            public KeyValuePair<string, string> Url { get; set; }

            public List<KeyValuePair<string, Image>> Pics { get; set; }

            public List<KeyValuePair<string, string>> TooltipText { get; set; }

            public string Errmsg;

            public void AddTooltipInfo(string url, string tooltip, Image img)
            {
                TooltipText.Add(new KeyValuePair<string, string>(url, tooltip));
                Pics.Add(new KeyValuePair<string, Image>(url, img));
            }
        }

        private class ThumbnailService
        {
            public string Name { get; private set; }

            public UrlCreatorDelegate UrlCreator { get; private set; }

            public ImageCreatorDelegate ImageCreator { get; private set; }

            public ThumbnailService(string name, UrlCreatorDelegate urlcreator, ImageCreatorDelegate imagecreator)
            {
                Name = name;
                UrlCreator = urlcreator;
                ImageCreator = imagecreator;
            }
        }
    }
}