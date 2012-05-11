using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Security.Principal;

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

using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Tween
{
    public static class MyCommon
    {
        private static readonly object LockObj = new object();

        //終了フラグ
        public static bool _endingFlag;

        public static string cultureStr = null;

        public static string settingPath;

        public enum IconSizes
        {
            IconNone = 0,
            Icon16 = 1,
            Icon24 = 2,
            Icon48 = 3,
            Icon48_2 = 4
        }

        public enum NameBalloonEnum
        {
            None,
            UserID,
            NickName
        }

        public enum DispTitleEnum
        {
            None,
            Ver,
            Post,
            UnreadRepCount,
            UnreadAllCount,
            UnreadAllRepCount,
            UnreadCountAllCount,
            OwnStatus
        }

        public enum LogUnitEnum
        {
            Minute,
            Hour,
            Day
        }

        public enum UploadFileType
        {
            Invalid,
            Picture,
            MultiMedia
        }

        public enum UrlConverter
        {
            TinyUrl,
            Isgd,
            Twurl,
            Bitly,
            Jmp,
            Uxnu,
            //特殊
            Nicoms,
            //廃止
            Unu = -1
        }

        public enum OutputzUrlmode
        {
            twittercom,
            twittercomWithUsername
        }

        public enum HITRESULT
        {
            None,
            Copy,
            CopyAndMark,
            Move,
            Exclude
        }

        public enum HttpTimeOut
        {
            MinValue = 10,
            MaxValue = 120,
            DefaultValue = 20
        }

        //Backgroundworkerへ処理種別を通知するための引数用Enum
        public enum WORKERTYPE
        {
            Timeline,
            //タイムライン取得
            Reply,
            //返信取得
            DirectMessegeRcv,
            //受信DM取得
            DirectMessegeSnt,
            //送信DM取得
            PostMessage,
            //発言POST
            FavAdd,
            //Fav追加
            FavRemove,
            //Fav削除
            Follower,
            //Followerリスト取得
            OpenUri,
            //Uri開く
            Favorites,
            //Fav取得
            Retweet,
            //Retweetする
            PublicSearch,
            //公式検索
            List,
            //Lists
            Related,
            //関連発言
            UserStream,
            //UserStream
            UserTimeline,
            //UserTimeline
            BlockIds,
            //Blocking/ids
            Configuration,
            //Twitter Configuration読み込み
            ///
            ErrorState
            //エラー表示のみで後処理終了(認証エラー時など)
        }

        public struct DEFAULTTAB
        {
            public const string RECENT = "Recent";
            public const string REPLY = "Reply";
            public const string DM = "Direct";

            public const string FAV = "Favorites";

            private string dummy;

            private new object ReferenceEquals()
            {
                return new object();
            }

            private new object Equals()
            {
                return new object();
            }
        }

        public const object Block = null;

        public static bool TraceFlag = false;
#if DEBUG
		public static bool DebugBuild = true;
#else
        public static bool DebugBuild = false;
#endif

        public enum ACCOUNT_STATE
        {
            Valid,
            Invalid
        }

        public enum REPLY_ICONSTATE
        {
            None,
            StaticIcon,
            BlinkIcon
        }

        [FlagsAttribute()]
        public enum EVENTTYPE
        {
            None = 0,
            Favorite = 1,
            Unfavorite = 2,
            Follow = 4,
            ListMemberAdded = 8,
            ListMemberRemoved = 16,
            Block = 32,
            Unblock = 64,
            UserUpdate = 128,
            Deleted = 256,
            ListCreated = 512,
            ListUpdated = 1024,

            All = (None | Favorite | Unfavorite | Follow | ListMemberAdded | ListMemberRemoved | Block | Unblock | UserUpdate | Deleted | ListCreated | ListUpdated)
        }

        public static void TraceOut(Exception ex, string Message)
        {
            bool a = true;
            string buf = ExceptionOutMessage(ex, ref a);
            TraceOut(TraceFlag, Message + Environment.NewLine + buf);
        }

        public static void TraceOut(string Message)
        {
            TraceOut(TraceFlag, Message);
        }

        public static void TraceOut(bool OutputFlag, string Message)
        {
            lock (LockObj)
            {
                if (!OutputFlag)
                    return;
                DateTime now = DateTime.Now;
                string fileName = string.Format("TweenTrace-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
                {
                    writer.WriteLine("**** TraceOut: {0} ****", DateTime.Now.ToString());
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText1);
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText2);
                    writer.WriteLine();
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText3);
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText4, Environment.OSVersion.VersionString);
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText5, Environment.Version.ToString());
                    writer.WriteLine(Tween.My_Project.Resources.TraceOutText6, fileVersion);
                    writer.WriteLine(Message);
                    writer.WriteLine();
                }
            }
        }

        // エラー内容をバッファに書き出し
        // 注意：最終的にファイル出力されるエラーログに記録されるため次の情報は書き出さない
        // 文頭メッセージ、権限、動作環境
        // Dataプロパティにある終了許可フラグのパースもここで行う

        public static string ExceptionOutMessage(Exception ex, ref bool IsTerminatePermission)
        {
            if (ex == null)
                return "";

            StringBuilder buf = new StringBuilder();

            buf.AppendFormat(Tween.My_Project.Resources.UnhandledExceptionText8, ex.GetType().FullName, ex.Message);
            buf.AppendLine();
            if (ex.Data != null)
            {
                bool needHeader = true;
                foreach (DictionaryEntry dt in ex.Data)
                {
                    if (needHeader)
                    {
                        buf.AppendLine();
                        buf.AppendLine("-------Extra Information-------");
                        needHeader = false;
                    }
                    buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value);
                    buf.AppendLine();
                    if (dt.Key.Equals("IsTerminatePermission"))
                    {
                        IsTerminatePermission = Convert.ToBoolean(dt.Value);
                    }
                }
                if (!needHeader)
                {
                    buf.AppendLine("-----End Extra Information-----");
                }
            }
            buf.AppendLine(ex.StackTrace);
            buf.AppendLine();

            //InnerExceptionが存在する場合書き出す
            Exception _ex = ex.InnerException;
            int nesting = 0;
            while (_ex != null)
            {
                buf.AppendFormat("-----InnerException[{0}]-----" + Constants.vbCrLf, nesting);
                buf.AppendLine();
                buf.AppendFormat(Tween.My_Project.Resources.UnhandledExceptionText8, _ex.GetType().FullName, _ex.Message);
                buf.AppendLine();
                if (_ex.Data != null)
                {
                    bool needHeader = true;

                    foreach (DictionaryEntry dt in _ex.Data)
                    {
                        if (needHeader)
                        {
                            buf.AppendLine();
                            buf.AppendLine("-------Extra Information-------");
                            needHeader = false;
                        }
                        buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value);
                        if (dt.Key.Equals("IsTerminatePermission"))
                        {
                            IsTerminatePermission = Convert.ToBoolean(dt.Value);
                        }
                    }
                    if (!needHeader)
                    {
                        buf.AppendLine("-----End Extra Information-----");
                    }
                }
                buf.AppendLine(_ex.StackTrace);
                buf.AppendLine();
                nesting += 1;
                _ex = _ex.InnerException;
            }
            return buf.ToString();
        }

        public static bool ExceptionOut(Exception ex)
        {
            lock (LockObj)
            {
                bool IsTerminatePermission = true;
                DateTime now = DateTime.Now;
                string fileName = string.Format("Tween-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
                {
                    WindowsIdentity ident = WindowsIdentity.GetCurrent();
                    WindowsPrincipal princ = new WindowsPrincipal(ident);

                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText1, DateTime.Now.ToString());
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText2);
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText3);
                    // 権限書き出し
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText11 + princ.IsInRole(WindowsBuiltInRole.Administrator).ToString());
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText12 + princ.IsInRole(WindowsBuiltInRole.User).ToString());
                    writer.WriteLine();
                    // OSVersion,AppVersion書き出し
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText4);
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText5, Environment.OSVersion.VersionString);
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText6, Environment.Version.ToString());
                    writer.WriteLine(Tween.My_Project.Resources.UnhandledExceptionText7, fileVersion);

                    writer.Write(ExceptionOutMessage(ex, ref IsTerminatePermission));
                    writer.Flush();
                }

                switch (MessageBox.Show(string.Format(Tween.My_Project.Resources.UnhandledExceptionText9, fileName, Environment.NewLine), Tween.My_Project.Resources.UnhandledExceptionText10, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error))
                {
                    case DialogResult.Yes:
                        System.Diagnostics.Process.Start(fileName);
                        return false;
                    case DialogResult.No:
                        return false;
                    case DialogResult.Cancel:
                        return IsTerminatePermission;
                }
                return IsTerminatePermission;
            }
        }

        /// <summary>
        /// URLに含まれているマルチバイト文字列を%xx形式でエンコードします。
        /// <newpara>
        /// マルチバイト文字のコードはUTF-8またはUnicodeで自動的に判断します。
        /// </newpara>
        /// </summary>
        /// <param name = input>エンコード対象のURL</param>
        /// <returns>マルチバイト文字の部分をUTF-8/%xx形式でエンコードした文字列を返します。</returns>

        public static string urlEncodeMultibyteChar(string _input)
        {
            Uri uri = null;
            StringBuilder sb = new StringBuilder(256);
            string result = "";
            char c = 'd';
            foreach (char c_loopVariable in _input)
            {
                c = c_loopVariable;
                if (Convert.ToInt32(c) > 127)
                    break; // TODO: might not be correct. Was : Exit For
            }
            if (Convert.ToInt32(c) <= 127)
                return _input;

            string input = HttpUtility.UrlDecode(_input);
        retry:
            foreach (char c_loopVariable in input)
            {
                c = c_loopVariable;
                if (Convert.ToInt32(c) > 255)
                {
                    // Unicodeの場合(1charが複数のバイトで構成されている）
                    // UriクラスをNewして再構成し、入力をPathAndQueryのみとしてやり直す
                    foreach (byte b_loopVariable in Encoding.UTF8.GetBytes(new string(new[] { c })))
                    {
                        var b = b_loopVariable;
                        sb.AppendFormat("%{0:X2}", b);
                    }
                }
                else if (Convert.ToInt32(c) > 127 || c == '%')
                {
                    // UTF-8の場合
                    // UriクラスをNewして再構成し、入力をinputからAuthority部分を除去してやり直す
                    if (uri == null)
                    {
                        uri = new Uri(input);
                        input = input.Remove(0, uri.GetLeftPart(UriPartial.Authority).Length);
                        sb.Length = 0;
                        goto retry;
                    }
                    else
                    {
                        sb.Append("%" + Convert.ToInt16(c).ToString("X2").ToUpper());
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (uri == null)
            {
                result = sb.ToString();
            }
            else
            {
                result = uri.GetLeftPart(UriPartial.Authority) + sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// URLのドメイン名をPunycode展開します。
        /// <para>
        /// ドメイン名がIDNでない場合はそのまま返します。
        /// ドメインラベルの区切り文字はFULLSTOP(.、U002E)に置き換えられます。
        /// </para>
        /// </summary>
        /// <param name="input">展開対象のURL</param>
        /// <returns>IDNが含まれていた場合はPunycodeに展開したURLをを返します。Punycode展開時にエラーが発生した場合はNothingを返します。</returns>

        public static string IDNDecode(string input)
        {
            string result = "";
            IdnMapping IDNConverter = new IdnMapping();

            if (!input.Contains("://"))
                return null;

            // ドメイン名をPunycode展開
            string Domain = null;
            string AsciiDomain = null;

            try
            {
                Domain = input.Split('/')[2];
                AsciiDomain = IDNConverter.GetAscii(Domain);
            }
            catch (Exception ex)
            {
                return null;
            }

            return input.Replace("://" + Domain, "://" + AsciiDomain);
        }

        public static void MoveArrayItem(int[] values, int idx_fr, int idx_to)
        {
            int moved_value = values[idx_fr];
            int num_moved = Math.Abs(idx_fr - idx_to);

            if (idx_to < idx_fr)
            {
                Array.Copy(values, idx_to, values, idx_to + 1, num_moved);
            }
            else
            {
                Array.Copy(values, idx_fr + 1, values, idx_fr, num_moved);
            }

            values[idx_to] = moved_value;
        }

        public static string EncryptString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            //文字列をバイト型配列にする
            byte[] bytesIn = System.Text.Encoding.UTF8.GetBytes(str);

            //DESCryptoServiceProviderオブジェクトの作成
            using (System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                //共有キーと初期化ベクタを決定
                //パスワードをバイト配列にする
                byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes("_tween_encrypt_key_");
                //共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                //暗号化されたデータを書き出すためのMemoryStream
                using (System.IO.MemoryStream msOut = new System.IO.MemoryStream())
                {
                    //DES暗号化オブジェクトの作成
                    using (System.Security.Cryptography.ICryptoTransform desdecrypt = des.CreateEncryptor())
                    {
                        //書き込むためのCryptoStreamの作成
                        using (System.Security.Cryptography.CryptoStream cryptStream = new System.Security.Cryptography.CryptoStream(msOut, desdecrypt, System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            //書き込む
                            cryptStream.Write(bytesIn, 0, bytesIn.Length);
                            cryptStream.FlushFinalBlock();
                            //暗号化されたデータを取得
                            byte[] bytesOut = msOut.ToArray();

                            //閉じる
                            cryptStream.Close();
                            msOut.Close();

                            //Base64で文字列に変更して結果を返す
                            return System.Convert.ToBase64String(bytesOut);
                        }
                    }
                }
            }
        }

        public static string DecryptString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            //DESCryptoServiceProviderオブジェクトの作成
            using (System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                //共有キーと初期化ベクタを決定
                //パスワードをバイト配列にする
                byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes("_tween_encrypt_key_");
                //共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                //Base64で文字列をバイト配列に戻す
                byte[] bytesIn = System.Convert.FromBase64String(str);
                //暗号化されたデータを読み込むためのMemoryStream
                using (System.IO.MemoryStream msIn = new System.IO.MemoryStream(bytesIn))
                {
                    //DES復号化オブジェクトの作成
                    using (System.Security.Cryptography.ICryptoTransform desdecrypt = des.CreateDecryptor())
                    {
                        //読み込むためのCryptoStreamの作成
                        using (System.Security.Cryptography.CryptoStream cryptStreem = new System.Security.Cryptography.CryptoStream(msIn, desdecrypt, System.Security.Cryptography.CryptoStreamMode.Read))
                        {
                            //復号化されたデータを取得するためのStreamReader
                            using (System.IO.StreamReader srOut = new System.IO.StreamReader(cryptStreem, System.Text.Encoding.UTF8))
                            {
                                //復号化されたデータを取得する
                                string result = srOut.ReadToEnd();

                                //閉じる
                                srOut.Close();
                                cryptStreem.Close();
                                msIn.Close();

                                return result;
                            }
                        }
                    }
                }
            }
        }

        public static byte[] ResizeBytesArray(byte[] bytes, int newSize)
        {
            byte[] newBytes = new byte[newSize];
            if (bytes.Length <= newSize)
            {
                int i = 0;
                for (i = 0; i <= bytes.Length - 1; i++)
                {
                    newBytes[i] = bytes[i];
                }
            }
            else
            {
                int pos = 0;
                int i = 0;
                for (i = 0; i <= bytes.Length - 1; i++)
                {
                    newBytes[pos] = (byte)(newBytes[pos] ^ bytes[i]);
                    pos += 1;
                    if (pos >= newBytes.Length)
                    {
                        pos = 0;
                    }
                }
            }
            return newBytes;
        }

        public static bool IsNT6()
        {
            //NT6 kernelかどうか検査
            return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6;
        }

        [FlagsAttribute()]
        public enum TabUsageType
        {
            Undefined = 0,
            Home = 1,
            //Unique
            Mentions = 2,
            //Unique
            DirectMessage = 4,
            //Unique
            Favorites = 8,
            //Unique
            UserDefined = 16,
            LocalQuery = 32,
            //Pin(no save/no save query/distribute/no update(normal update))
            Profile = 64,
            //Pin(save/no distribute/manual update)
            PublicSearch = 128,
            //Pin(save/no distribute/auto update)
            Lists = 256,
            Related = 512,
            UserTimeline = 1024
            //RTMyTweet
            //RTByOthers
            //RTByMe
        }

        public static string fileVersion = "";

        public static string GetUserAgentString()
        {
            if (string.IsNullOrEmpty(fileVersion))
            {
                throw new Exception("fileversion is not Initialized.");
            }
            return "Tween/" + fileVersion;
        }

        public static ApiInformation TwitterApiInfo = new ApiInformation();

        public static bool IsAnimatedGif(string filename)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(filename);
                if (img == null)
                    return false;
                if (img.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
                {
                    System.Drawing.Imaging.FrameDimension fd = new System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList[0]);
                    int fd_count = img.GetFrameCount(fd);
                    if (fd_count > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }

        public static System.DateTime DateTimeParse(string input)
        {
            System.DateTime rslt = default(System.DateTime);
            string[] format = { "ddd MMM dd HH:mm:ss zzzz yyyy" };
            foreach (string fmt in format)
            {
                if (DateTime.TryParseExact(input, fmt, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out rslt))
                {
                    return rslt;
                }
                else
                {
                    continue;
                }
            }
            TraceOut("Parse Error(DateTimeFormat) : " + input);
            return new System.DateTime();
        }

        public static T CreateDataFromJson<T>(string content)
        {
            T data = default(T);
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buf = Encoding.Unicode.GetBytes(content);
                stream.Write(Encoding.Unicode.GetBytes(content), offset: 0, count: buf.Length);
                stream.Seek(offset: 0, loc: SeekOrigin.Begin);
                data = (T)(new DataContractJsonSerializer(typeof(T))).ReadObject(stream);
            }
            return data;
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, "^(?(\")(\".+?\"@)|(([0-9a-zA-Z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-zA-Z])@))" + "(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,6}))$");
        }
    }
}