// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
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
using System.Xml;

namespace Hoehoe
{
    public class Plixi : HttpConnectionOAuthEcho, IMultimediaShareService
    {
        //OAuth関連
        ///<summary>
        ///OAuthのコンシューマー鍵 : TODO
        ///</summary>
        private const string ConsumerKey = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM";

        ///<summary>
        ///OAuthの署名作成用秘密コンシューマーデータ
        ///</summary>
        private const string ConsumerSecretKey = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM";
        private const string ApiKey = "c0263958-f32c-4704-9dac-cdc806b1249c";

        private string[] _pictureExts = { ".jpg", ".jpeg", ".gif", ".png" };

        private const long MaxFileSize = 5 * 1024 * 1024;

        private Twitter _tw;

        public string Upload(ref string filePath, ref string message, long replyTo)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return "Err:File isn't specified.";
            }
            if (String.IsNullOrEmpty(message))
            {
                message = "";
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

            string content = "";
            HttpStatusCode ret = default(HttpStatusCode);
            //Plixiへの投稿
            try
            {
                ret = UploadFile(mediaFile, message, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            string url = "";
            if (ret == HttpStatusCode.Created)
            {
                XmlDocument xd = new XmlDocument();
                try
                {
                    xd.LoadXml(content);
                    //MediaUrlの取得
                    url = xd.ChildNodes[0].ChildNodes[2].InnerText;
                }
                catch (XmlException ex)
                {
                    return "Err:" + ex.Message;
                }
                catch (Exception Ex)
                {
                    return "Err:" + Ex.Message;
                }
            }
            else
            {
                return "Err:" + ret.ToString();
            }
            //アップロードまでは成功
            filePath = "";
            if (String.IsNullOrEmpty(url))
            {
                url = "";
            }
            if (String.IsNullOrEmpty(message))
            {
                message = "";
            }
            //Twitterへの投稿
            //投稿メッセージの再構成
            if (message.Length + AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140)
            {
                message = message.Substring(0, 140 - AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + url;
            }
            else
            {
                message += " " + url;
            }
            return _tw.PostStatus(message, replyTo);
        }

        private HttpStatusCode UploadFile(FileInfo mediaFile, string message, ref string content)
        {
            //Message必須
            if (String.IsNullOrEmpty(message))
            {
                message = "";
            }
            //Check filetype and size(Max 5MB)
            if (!CheckValidExtension(mediaFile.Extension))
            {
                throw new ArgumentException("Service don't support this filetype.");
            }
            if (!CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
            {
                throw new ArgumentException("File is too large.");
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("api_key", ApiKey);
            param.Add("message", message);
            param.Add("isoauth", "true");
            List<KeyValuePair<string, FileInfo>> binary = new List<KeyValuePair<string, FileInfo>>();
            binary.Add(new KeyValuePair<string, FileInfo>("media", mediaFile));
            this.InstanceTimeout = 60000;
            //タイムアウト60秒

            return GetContent(PostMethod, new Uri("http://api.plixi.com/api/upload.aspx"), param, binary, ref content, null, null);
        }

        public bool CheckValidExtension(string ext)
        {
            return Array.IndexOf(_pictureExts, ext.ToLower()) > -1;
        }

        public string GetFileOpenDialogFilter()
        {
            return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
        }

        public Hoehoe.MyCommon.UploadFileType GetFileType(string ext)
        {
            if (CheckValidExtension(ext))
            {
                return Hoehoe.MyCommon.UploadFileType.Picture;
            }
            return Hoehoe.MyCommon.UploadFileType.Invalid;
        }

        public bool IsSupportedFileType(Hoehoe.MyCommon.UploadFileType type)
        {
            return type.Equals(Hoehoe.MyCommon.UploadFileType.Picture);
        }

        public bool CheckValidFilesize(string ext, long fileSize)
        {
            if (CheckValidExtension(ext))
            {
                return fileSize <= MaxFileSize;
            }
            return false;
        }

        public bool Configuration(string key, object value)
        {
            return true;
        }

        public Plixi(Twitter twitter)
            : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1/account/verify_credentials.json"))
        {
            _tw = twitter;
            Initialize(MyCommon.DecryptString(ConsumerKey), MyCommon.DecryptString(ConsumerSecretKey), _tw.AccessToken, _tw.AccessTokenSecret, "", "");
        }
    }
}