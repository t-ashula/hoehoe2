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
    using System.IO;
    using System.Net;
    using System.Xml;

    public class imgly : HttpConnectionOAuthEcho, IMultimediaShareService
    {
        /// <summary>
        /// OAuthのコンシューマー鍵
        /// </summary>
        private const string ConsumerKey = "BIazYuf0scya8pyhLjkdg";

        /// <summary>
        /// OAuthの署名作成用秘密コンシューマーデータ
        /// </summary>
        private const string ConsumerSecretKey = "hVih4pcFCfcpHWXyICLQINmZ1LHXdMzHA4QXMWwBhMQ";

        private const long MaxFileSize = 4 * 1024 * 1024;

        private string[] pictureExts = { ".jpg", ".jpeg", ".gif", ".png" };

        private Twitter tw;

        public imgly(Twitter twitter)
            : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1/account/verify_credentials.json"))
        {
            this.tw = twitter;
            this.Initialize(ConsumerKey, ConsumerSecretKey, this.tw.AccessToken, this.tw.AccessTokenSecret, string.Empty, string.Empty);
        }

        public string Upload(ref string filePath, ref string message, long replyTo)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return "Err:File isn't specified.";
            }

            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }
            
            FileInfo mediaFile = null;
            try
            {
                mediaFile = new FileInfo(filePath);
            }
            catch (NotSupportedException ex)
            {
                return "Err:" + ex.Message;
            }
            
            if (mediaFile == null || !mediaFile.Exists)
            {
                return "Err:File isn't exists.";
            }

            string content = string.Empty;
            HttpStatusCode ret = default(HttpStatusCode);
            try
            {
                // img.lyへの投稿
                ret = this.UploadFile(mediaFile, message, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            
            string url = string.Empty;
            if (ret == HttpStatusCode.OK)
            {
                XmlDocument xd = new XmlDocument();
                try
                {
                    // URLの取得
                    xd.LoadXml(content);
                    url = xd.SelectSingleNode("/image/url").InnerText;
                }
                catch (XmlException ex)
                {
                    return "Err:" + ex.Message;
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message;
                }
            }
            else
            {
                return "Err:" + ret.ToString();
            }

            // アップロードまでは成功
            filePath = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                url = string.Empty;
            }

            // Twitterへの投稿/投稿メッセージの再構成
            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }

            if (message.Length + AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140)
            {
                message = message.Substring(0, 140 - AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + url;
            }
            else
            {
                message += " " + url;
            }

            return this.tw.PostStatus(message, replyTo);
        }

        public bool CheckValidExtension(string ext)
        {
            return Array.IndexOf(this.pictureExts, ext.ToLower()) > -1;
        }

        public string GetFileOpenDialogFilter()
        {
            return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
        }

        public Hoehoe.UploadFileType GetFileType(string ext)
        {
            return this.CheckValidExtension(ext) ? Hoehoe.UploadFileType.Picture : Hoehoe.UploadFileType.Invalid;            
        }

        public bool IsSupportedFileType(Hoehoe.UploadFileType type)
        {
            return type.Equals(Hoehoe.UploadFileType.Picture);
        }

        public bool CheckValidFilesize(string ext, long fileSize)
        {
            return this.CheckValidExtension(ext) && fileSize <= MaxFileSize;
        }

        public bool Configuration(string key, object value)
        {
            return true;
        }

        private HttpStatusCode UploadFile(FileInfo mediaFile, string message, ref string content)
        {
            // Message必須
            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }
            
            // Check filetype and size(Max 4MB)
            if (!this.CheckValidExtension(mediaFile.Extension))
            {
                throw new ArgumentException("Service don't support this filetype.");
            }

            if (!this.CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
            {
                throw new ArgumentException("File is too large.");
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("message", message);
            List<KeyValuePair<string, FileInfo>> binary = new List<KeyValuePair<string, FileInfo>>();
            binary.Add(new KeyValuePair<string, FileInfo>("media", mediaFile));
            this.InstanceTimeout = 60000; // タイムアウト60秒
            return this.GetContent(HttpConnection.PostMethod, new Uri("http://img.ly/api/2/upload.xml"), param, binary, ref content, null, null);
        }
    }
}