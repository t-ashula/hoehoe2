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
using System.IO;

namespace Hoehoe
{
    public class TwitterPhoto : IMultimediaShareService
    {
        private string[] _pictureExts = { ".jpg", ".jpeg", ".gif", ".png" };

        private const Int64 MaxfilesizeDefault = 3145728;        // help/configurationにより取得されコンストラクタへ渡される
        
        private Int64 _maxFileSize = 3145728;
        private Twitter _tw;

        public bool CheckValidExtension(string ext)
        {
            return Array.IndexOf(_pictureExts, ext.ToLower()) > -1;
        }

        public bool CheckValidFilesize(string ext, long fileSize)
        {
            if (CheckValidExtension(ext))
            {
                return fileSize <= _maxFileSize;
            }
            return false;
        }

        public bool Configuration(string key, object value)
        {
            if (key == "MaxUploadFilesize")
            {
                try
                {
                    Int64 val = Convert.ToInt64(value);
                    _maxFileSize = val > 0 ? val : MaxfilesizeDefault;
                }
                catch (Exception)
                {
                    _maxFileSize = MaxfilesizeDefault;
                    return false;                    //error
                }
                return true; // 正常に設定終了
            }
            return true;     // 設定項目がない場合はとりあえずエラー扱いにしない
        }

        public string GetFileOpenDialogFilter()
        {
            return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
        }

        public MyCommon.UploadFileType GetFileType(string ext)
        {
            if (CheckValidExtension(ext))
            {
                return MyCommon.UploadFileType.Picture;
            }
            return MyCommon.UploadFileType.Invalid;
        }

        public bool IsSupportedFileType(MyCommon.UploadFileType type)
        {
            return type.Equals(MyCommon.UploadFileType.Picture);
        }

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
            if (!mediaFile.Exists)
            {
                return "Err:File isn't exists.";
            }
            if (MyCommon.IsAnimatedGif(filePath))
            {
                return "Err:Don't support animatedGIF.";
            }

            return _tw.PostStatusWithMedia(message, replyTo, mediaFile);
        }

        public TwitterPhoto(Twitter twitter)
        {
            _tw = twitter;
        }
    }
}