using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Xml;
namespace Tween
{

	public class TwitterPhoto : IMultimediaShareService
	{

		private string[] pictureExt = {
			".jpg",
			".jpeg",
			".gif",
			".png"

		};

		private const Int64 MaxfilesizeDefault = 3145728;
		// help/configurationにより取得されコンストラクタへ渡される

		private Int64 _MaxFileSize = 3145728;

		private Twitter tw;
		public bool CheckValidExtension(string ext)
		{
			if (Array.IndexOf(pictureExt, ext.ToLower()) > -1) {
				return true;
			}
			return false;
		}

		public bool CheckValidFilesize(string ext, long fileSize)
		{
			if (this.CheckValidExtension(ext)) {
				return fileSize <= _MaxFileSize;
			}
			return false;
		}

		public bool Configuration(string key, object value)
		{
			if (key == "MaxUploadFilesize") {
				Int64 val = default(Int64);
				try {
					val = Convert.ToInt64(value);
					if (val > 0) {
						_MaxFileSize = val;
					} else {
						_MaxFileSize = MaxfilesizeDefault;
					}
				} catch (Exception ex) {
					_MaxFileSize = MaxfilesizeDefault;
					return false;
					//error
				}
				return true;
				// 正常に設定終了
			}
			return true;
			// 設定項目がない場合はとりあえずエラー扱いにしない
		}

		public string GetFileOpenDialogFilter()
		{
			return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
		}

		public MyCommon.UploadFileType GetFileType(string ext)
		{
			if (this.CheckValidExtension(ext)) {
				return UploadFileType.Picture;
			}
			return UploadFileType.Invalid;
		}

		public bool IsSupportedFileType(MyCommon.UploadFileType type)
		{
			return type.Equals(UploadFileType.Picture);
		}

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
			if (!mediaFile.Exists)
				return "Err:File isn't exists.";
			if (MyCommon.IsAnimatedGif(filePath))
				return "Err:Don't support animatedGIF.";

			return tw.PostStatusWithMedia(message, reply_to, mediaFile);
		}

		public TwitterPhoto(Twitter twitter)
		{
			tw = twitter;
		}
	}
}
