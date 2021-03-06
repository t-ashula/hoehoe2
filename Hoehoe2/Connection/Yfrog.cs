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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;

namespace Hoehoe
{
    public class Yfrog : HttpConnectionOAuthEcho, IMultimediaShareService
    {
        /// <summary>
        /// OAuthのコンシューマー鍵
        /// </summary>
        private const string ConsumerKey = "BIazYuf0scya8pyhLjkdg";

        /// <summary>
        /// OAuthの署名作成用秘密コンシューマーデータ
        /// </summary>
        private const string ConsumerSecretKey = "hVih4pcFCfcpHWXyICLQINmZ1LHXdMzHA4QXMWwBhMQ";

        /// <summary>
        /// Yfrog api key
        /// </summary>
        private const string ApiKey = "069IKPSW75e293f35ab00d91a1b862c5a654c46b";

        private const long MaxFileSize = 5 * 1024 * 1024;

        private readonly string[] _pictureExts = { ".jpg", ".jpeg", ".gif", ".png" };

        private readonly Twitter _tw;

        public Yfrog(Twitter twitter)
            : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"))
        {
            _tw = twitter;
            Initialize(ConsumerKey, ConsumerSecretKey, _tw.AccessToken, _tw.AccessTokenSecret, string.Empty, string.Empty);
        }

        public string Upload(ref string filePath, ref string message, long replyTo)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return "Err:File isn't exists.";
            }

            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }

            // FileInfo作成
            FileInfo mediaFile;
            try
            {
                mediaFile = new FileInfo(filePath);
            }
            catch (NotSupportedException ex)
            {
                return string.Format("Err:{0}", ex.Message);
            }

            if (!mediaFile.Exists)
            {
                return "Err:" + "File isn't exists.";
            }

            var content = string.Empty;
            HttpStatusCode ret;
            try
            {
                // yfrogへの投稿
                ret = UploadFile(mediaFile, message, ref content);
            }
            catch (Exception ex)
            {
                return string.Format("Err:{0}", ex.Message);
            }

            string url;
            if (ret != HttpStatusCode.OK)
            {
                return string.Format("Err:{0}", ret);
            }

            try
            {
                // URLの取得
                var responce = DataModels.D.CreateDataFromJson<ResponceObject>(content);
                url = responce.RSP.MediaUrl;
            }
            catch (XmlException ex)
            {
                return string.Format("Err:{0}", ex.Message);
            }
            catch (Exception ex)
            {
                return string.Format("Err:{0}", ex.Message);
            }

            if (string.IsNullOrEmpty(url))
            {
                url = string.Empty;
            }

            // アップロードまでは成功
            filePath = string.Empty;

            // Twitterへの投稿/投稿メッセージの再構成
            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }

            if (message.Length + Configs.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140)
            {
                message = message.Substring(0, 140 - Configs.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + url;
            }
            else
            {
                message += " " + url;
            }

            return _tw.PostStatus(message, 0);
        }

        public bool CheckValidExtension(string ext)
        {
            return Array.IndexOf(_pictureExts, ext.ToLower()) > -1;
        }

        public string GetFileOpenDialogFilter()
        {
            return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
        }

        public UploadFileType GetFileType(string ext)
        {
            return CheckValidExtension(ext) ? UploadFileType.Picture : UploadFileType.Invalid;
        }

        public bool IsSupportedFileType(UploadFileType type)
        {
            return type.Equals(UploadFileType.Picture);
        }

        public bool CheckValidFilesize(string ext, long fileSize)
        {
            return CheckValidExtension(ext) && fileSize <= MaxFileSize;
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

            // Check filetype and size(Max 5MB)
            if (!CheckValidExtension(mediaFile.Extension))
            {
                throw new ArgumentException("Service don't support this filetype.");
            }

            if (!CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
            {
                throw new ArgumentException("File is too large.");
            }

            var param = new Dictionary<string, string> { { "key", ApiKey }, { "message", message } };
            var binary = new List<KeyValuePair<string, FileInfo>> { new KeyValuePair<string, FileInfo>("media", mediaFile) };
            InstanceTimeout = 60000; // タイムアウト60秒
            return GetContent(PostMethod, new Uri("https://yfrog.com/api/xauth_upload"), param, binary, ref content, null, null);
        }

        [DataContract]
        private class ResponceObject
        {
            /*{"rsp":{"stat":"ok","mediaid":"h32nxmp","mediaurl":"http://yfrog.com/h32nxmp"}}*/

            [DataMember(Name = "rsp")]
            public Rsp RSP;

            [DataContract]
            public class Rsp
            {
                [DataMember(Name = "stat")]
                public string Stat;

                [DataMember(Name = "mediaid")]
                public string MediaId;

                [DataMember(Name = "mediaurl")]
                public string MediaUrl;
            }
        }
    }
}