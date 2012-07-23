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
    using System.Windows.Forms;

    public partial class Thumbnail
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
    }
}