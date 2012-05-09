using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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

using System.IO;
using System.Net;
using System.Xml;
namespace Tween
{

	public class TwitPic : HttpConnectionOAuthEcho, IMultimediaShareService
	{


		//OAuth関連
		///<summary>
		///OAuthのコンシューマー鍵
		///</summary>

		private const string ConsumerKey = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM";
		///<summary>
		///OAuthの署名作成用秘密コンシューマーデータ
		///</summary>

		private const string ConsumerSecretKey = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM";
		private const string ApiKey = "287b60562aea3cab9f58fa54015848e8";
		private string[] pictureExt = {
			".jpg",
			".jpeg",
			".gif",
			".png"
		};
		private string[] multimediaExt = {
			".avi",
			".wmv",
			".flv",
			".m4v",
			".mov",
			".mp4",
			".rm",
			".mpeg",
			".mpg",
			".3gp",
			".3g2"

		};
			//Image only
		private const long MaxFileSize = 10 * 1024 * 1024;
		//Multimedia filesize limit unknown. But length limit is 1:30.


		private Twitter tw;
		public string Upload(ref string filePath, ref string message, long reply_to)
		{
			if (string.IsNullOrEmpty(filePath))
				return "Err:File isn't specified.";
			if (string.IsNullOrEmpty(message))
				message = "";
			FileInfo mediaFile = null;
			try {
				mediaFile = new FileInfo(filePath);
			} catch (NotSupportedException ex) {
				return "Err:" + ex.Message;
			}
			if (mediaFile == null || !mediaFile.Exists)
				return "Err:File isn't exists.";

			string content = "";
			HttpStatusCode ret = default(HttpStatusCode);
			//TwitPicへの投稿
			try {
				ret = UploadFile(mediaFile, message, ref content);
			} catch (Exception ex) {
				return "Err:" + ex.Message;
			}
			string url = "";
			if (ret == HttpStatusCode.OK) {
				XmlDocument xd = new XmlDocument();
				try {
					xd.LoadXml(content);
					//URLの取得
					url = xd.SelectSingleNode("/image/url").InnerText;
				} catch (XmlException ex) {
					return "Err:" + ex.Message;
				} catch (Exception Ex) {
					return "Err:" + Ex.Message;
				}
			} else {
				return "Err:" + ret.ToString();
			}
			//アップロードまでは成功
			filePath = "";
			if (string.IsNullOrEmpty(message))
				message = "";
			if (string.IsNullOrEmpty(url))
				url = "";
			//Twitterへの投稿
			//投稿メッセージの再構成
			if (message.Length + AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140) {
				message = message.Substring(0, 140 - AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + url;
			} else {
				message += " " + url;
			}
			return tw.PostStatus(message, 0);
		}

		private HttpStatusCode UploadFile(FileInfo mediaFile, string message, ref string content)
		{

			//Message必須
			if (string.IsNullOrEmpty(message))
				message = "";
			//Check filetype and size(Max 5MB)
			if (!this.CheckValidExtension(mediaFile.Extension))
				throw new ArgumentException("Service don't support this filetype.");
			if (!this.CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
				throw new ArgumentException("File is too large.");

			Dictionary<string, string> param = new Dictionary<string, string>();
			param.Add("key", ApiKey);
			param.Add("message", message);
			List<KeyValuePair<string, FileInfo>> binary = new List<KeyValuePair<string, FileInfo>>();
			binary.Add(new KeyValuePair<string, FileInfo>("media", mediaFile));
			if (this.GetFileType(mediaFile.Extension) == UploadFileType.Picture) {
				this.InstanceTimeout = 60000;
				//タイムアウト60秒
			} else {
				this.InstanceTimeout = 120000;
			}

			return GetContent(PostMethod, new Uri("http://api.twitpic.com/2/upload.xml"), param, binary, ref content, null, null);
		}

		public bool CheckValidExtension(string ext)
		{
			if (Array.IndexOf(pictureExt, ext.ToLower()) > -1)
				return true;
			if (Array.IndexOf(multimediaExt, ext.ToLower()) > -1)
				return true;
			return false;
		}

		public string GetFileOpenDialogFilter()
		{
			return "Image Files(*" + string.Join(";*", pictureExt) + ")|*" + string.Join(";*", pictureExt) + "|Videos(*" + string.Join(";*", multimediaExt) + ")|*" + string.Join(";*", multimediaExt);
		}

		public UploadFileType GetFileType(string ext)
		{
			if (Array.IndexOf(pictureExt, ext.ToLower()) > -1)
				return UploadFileType.Picture;
			if (Array.IndexOf(multimediaExt, ext.ToLower()) > -1)
				return UploadFileType.MultiMedia;
			return UploadFileType.Invalid;
		}

		public bool IsSupportedFileType(UploadFileType type)
		{
			return !type.Equals(UploadFileType.Invalid);
		}

		public bool CheckValidFilesize(string ext, long fileSize)
		{
			if (Array.IndexOf(pictureExt, ext.ToLower()) > -1)
				return fileSize <= MaxFileSize;
			if (Array.IndexOf(multimediaExt, ext.ToLower()) > -1)
				return true;
			//Multimedia : no check
			return false;
		}

		public bool Configuration(string key, object value)
		{
			return true;
		}

		public TwitPic(Twitter twitter) : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1/account/verify_credentials.json"))
		{
			tw = twitter;
			Initialize(MyCommon.DecryptString(ConsumerKey), MyCommon.DecryptString(ConsumerSecretKey), tw.AccessToken, tw.AccessTokenSecret, "", "");
		}
	}
}
