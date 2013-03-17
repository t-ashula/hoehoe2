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
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Windows.Forms;
    using R = Properties.Resources;

    public sealed class MyCommon
    {
        /// <summary>
        ///
        /// </summary>
        public static ApiInformation TwitterApiInfo = new ApiInformation();

        private static readonly object lockObj = new object();

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public static bool IsEnding { get; set; }

        /// <summary>
        ///
        /// </summary>
        public static string CultureStr { get; set; }

        /// <summary>
        ///
        /// </summary>
        public static string SettingPath { get; set; }

        public static bool NoLimit { get; set; }

        /// <summary>
        ///
        /// </summary>
        public static bool TraceFlag { get; set; }

#if DEBUG
        public static bool DebugBuild = true;
#else

        public static bool DebugBuild { get; set; }

#endif

        public static string FileVersion
        {
            get { return AppFileVersion; }
        }

        public static string AppDir
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public static Assembly AppAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public static string AppTitle
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyTitleAttribute>().Title; }
        }

        public static string AppAssemblyDescription
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyDescriptionAttribute>().Description; }
        }

        public static string AppAssemblyCompanyName
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyCompanyAttribute>().Company; }
        }

        public static string AppAssemblyCopyright
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyCopyrightAttribute>().Copyright; }
        }

        public static string AppAssemblyProductName
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyProductAttribute>().Product; }
        }

        public static string AppAssemblyName
        {
            get { return AppAssembly.GetName().Name; }
        }

        public static Version AppVersion
        {
            get { return AppAssembly.GetName().Version; }
        }

        public static string AppFileVersion
        {
            get { return GetAppAssemblyCustomeAttr<AssemblyFileVersionAttribute>().Version; }
        }

        public static void TraceOut(Exception ex, string message)
        {
            bool a = true;
            string buf = ExceptionOutMessage(ex, ref a);
            TraceOut(TraceFlag, message + Environment.NewLine + buf);
        }

        public static void TraceOut(string message)
        {
            TraceOut(TraceFlag, message);
        }

        public static void TraceOut(bool outputFlag, string message)
        {
            lock (lockObj)
            {
                if (!outputFlag)
                {
                    return;
                }

                DateTime now = DateTime.Now;
                string fileName = string.Format("HoehoeTrace-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (var writer = new StreamWriter(fileName))
                {
                    writer.WriteLine("**** TraceOut: {0} ****", now);
                    writer.WriteLine(R.TraceOutText1);
                    writer.WriteLine(R.TraceOutText2);
                    writer.WriteLine();
                    writer.WriteLine(R.TraceOutText3);
                    writer.WriteLine(R.TraceOutText4, Environment.OSVersion.VersionString);
                    writer.WriteLine(R.TraceOutText5, Environment.Version);
                    writer.WriteLine(R.TraceOutText6, FileVersion);
                    writer.WriteLine(message);
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// エラー内容をバッファに書き出し
        /// 注意：最終的にファイル出力されるエラーログに記録されるため次の情報は書き出さない
        /// 文頭メッセージ、権限、動作環境
        /// Dataプロパティにある終了許可フラグのパースもここで行う
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isTerminatePermission"></param>
        /// <returns></returns>
        public static string ExceptionOutMessage(Exception ex, ref bool isTerminatePermission)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var buf = new StringBuilder();
            buf.AppendFormat(R.UnhandledExceptionText8, ex.GetType().FullName, ex.Message);
            buf.AppendLine();

            // if (ex.Data != null)
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
                        isTerminatePermission = Convert.ToBoolean(dt.Value);
                    }
                }

                if (!needHeader)
                {
                    buf.AppendLine("-----End Extra Information-----");
                }
            }

            buf.AppendLine(ex.StackTrace);
            buf.AppendLine();

            // InnerExceptionが存在する場合書き出す
            Exception innerEx = ex.InnerException;
            int nesting = 0;
            while (innerEx != null)
            {
                buf.AppendFormat("-----InnerException[{0}]-----" + Environment.NewLine, nesting);
                buf.AppendLine();
                buf.AppendFormat(R.UnhandledExceptionText8, innerEx.GetType().FullName, innerEx.Message);
                buf.AppendLine();

                // if (innerEx.Data != null)
                {
                    bool needHeader = true;

                    foreach (DictionaryEntry dt in innerEx.Data)
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
                            isTerminatePermission = Convert.ToBoolean(dt.Value);
                        }
                    }

                    if (!needHeader)
                    {
                        buf.AppendLine("-----End Extra Information-----");
                    }
                }

                buf.AppendLine(innerEx.StackTrace);
                buf.AppendLine();
                nesting += 1;
                innerEx = innerEx.InnerException;
            }

            return buf.ToString();
        }

        public static bool ExceptionOut(Exception ex)
        {
            lock (lockObj)
            {
                bool isTerminatePermission = true;
                DateTime now = DateTime.Now;
                string fileName = string.Format("Hoehoe-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (var writer = new StreamWriter(fileName))
                {
                    WindowsIdentity ident = WindowsIdentity.GetCurrent();
                    var princ = new WindowsPrincipal(ident);

                    writer.WriteLine(R.UnhandledExceptionText1, DateTime.Now);
                    writer.WriteLine(R.UnhandledExceptionText2);
                    writer.WriteLine(R.UnhandledExceptionText3);

                    // 権限書き出し
                    writer.WriteLine(R.UnhandledExceptionText11 + princ.IsInRole(WindowsBuiltInRole.Administrator));
                    writer.WriteLine(R.UnhandledExceptionText12 + princ.IsInRole(WindowsBuiltInRole.User));
                    writer.WriteLine();

                    // OSVersion,AppVersion書き出し
                    writer.WriteLine(R.UnhandledExceptionText4);
                    writer.WriteLine(R.UnhandledExceptionText5, Environment.OSVersion.VersionString);
                    writer.WriteLine(R.UnhandledExceptionText6, Environment.Version);
                    writer.WriteLine(R.UnhandledExceptionText7, FileVersion);

                    writer.Write(ExceptionOutMessage(ex, ref isTerminatePermission));
                    writer.Flush();
                }

                switch (MessageBox.Show(string.Format(R.UnhandledExceptionText9, fileName, Environment.NewLine), R.UnhandledExceptionText10, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error))
                {
                    case DialogResult.Yes:
                        Process.Start(fileName);
                        return false;
                    case DialogResult.No:
                        return false;
                    case DialogResult.Cancel:
                        return isTerminatePermission;
                }

                return isTerminatePermission;
            }
        }

        /// <summary>
        /// URLに含まれているマルチバイト文字列を%xx形式でエンコードします。
        /// <newpara>
        /// マルチバイト文字のコードはUTF-8またはUnicodeで自動的に判断します。
        /// </newpara>
        /// </summary>
        /// <param name="input">エンコード対象のURL</param>
        /// <returns>マルチバイト文字の部分をUTF-8/%xx形式でエンコードした文字列を返します。</returns>
        public static string GetUrlEncodeMultibyteChar(string input)
        {
            Uri uri = null;
            var sb = new StringBuilder(256);
            char c = 'd';
            foreach (char cc in input)
            {
                c = cc;
                if (Convert.ToInt32(c) > 127)
                {
                    break;
                }
            }

            if (Convert.ToInt32(c) <= 127)
            {
                return input;
            }

            string decodedInput = HttpUtility.UrlDecode(input);
        retry:
            foreach (char cc in decodedInput)
            {
                c = cc;
                if (Convert.ToInt32(c) > 255)
                {
                    // Unicodeの場合(1charが複数のバイトで構成されている）
                    // UriクラスをNewして再構成し、入力をPathAndQueryのみとしてやり直す
                    foreach (var b in Encoding.UTF8.GetBytes(new string(new[] { c })))
                    {
                        sb.AppendFormat("%{0:X2}", b);
                    }
                }
                else if (Convert.ToInt32(c) > 127 || c == '%')
                {
                    // UTF-8の場合
                    // UriクラスをNewして再構成し、入力をinputからAuthority部分を除去してやり直す
                    if (uri == null)
                    {
                        uri = new Uri(decodedInput);
                        decodedInput = decodedInput.Remove(0, uri.GetLeftPart(UriPartial.Authority).Length);
                        sb.Length = 0;
                        goto retry;
                    }

                    sb.Append("%" + Convert.ToInt16(c).ToString("X2").ToUpper());
                }
                else
                {
                    sb.Append(c);
                }
            }

            return uri == null ? sb.ToString() : uri.GetLeftPart(UriPartial.Authority) + sb;
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
            var idnConverter = new IdnMapping();

            if (!input.Contains("://"))
            {
                return null;
            }

            // ドメイン名をPunycode展開
            string domain;
            string asciiDomain;
            try
            {
                domain = input.Split('/')[2];
                asciiDomain = idnConverter.GetAscii(domain);
            }
            catch (Exception)
            {
                return null;
            }

            return input.Replace("://" + domain, "://" + asciiDomain);
        }

        public static string GetUserAgentString()
        {
            if (string.IsNullOrEmpty(FileVersion))
            {
                throw new Exception("fileversion is not Initialized.");
            }

            return "Hoehoe/" + FileVersion;
        }

        public static bool IsAnimatedGif(string filename)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(filename);
                if (img.RawFormat.Guid == ImageFormat.Gif.Guid)
                {
                    var fd = new FrameDimension(img.FrameDimensionsList[0]);
                    return img.GetFrameCount(fd) > 1;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (img != null)
                {
                    img.Dispose();
                }
            }
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static DateTime DateTimeParse(string input)
        {
            string[] format = { "ddd MMM dd HH:mm:ss zzzz yyyy" };
            foreach (string fmt in format)
            {
                DateTime rslt;
                if (DateTime.TryParseExact(input, fmt, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out rslt))
                {
                    return rslt;
                }
            }

            TraceOut("Parse Error(DateTimeFormat) : " + input);
            return new DateTime();
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string[] GetSoundFileNames()
        {
            return new DirectoryInfo(GetSoundDir()).GetFiles("*.wav").Select(f => f.Name).ToArray();
        }

        public static void PlaySound(string snd)
        {
            if (string.IsNullOrEmpty(snd))
            {
                return;
            }

            try
            {
                using (var sndPlayer = new SoundPlayer(Path.Combine(GetSoundDir(), snd)))
                {
                    sndPlayer.Play();
                }
            }
            catch (Exception)
            {
            }
        }

        public static string GetSoundDir()
        {
            var basedir = AppDir;
            var dir = Path.Combine(basedir, "Sounds");
            return Directory.Exists(dir) ? dir : basedir;
        }

        public static void ReloadSoundSelector(ComboBox soundFileComboBox, string currentSoundFile)
        {
            soundFileComboBox.Items.Clear();
            soundFileComboBox.Items.Add(string.Empty);
            var names = GetSoundFileNames();
            if (names.Length > 0)
            {
                soundFileComboBox.Items.AddRange(names);
            }

            int idx = soundFileComboBox.Items.IndexOf(currentSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            soundFileComboBox.SelectedIndex = idx;
        }

        public static Image CheckValidImage(Image img, int width, int height)
        {
            if (img == null)
            {
                return null;
            }

            var bmp = new Bitmap(width, height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(img, 0, 0, width, height);
                }

                bmp.Tag = img.Tag;
                return bmp;
            }
            catch (Exception)
            {
                bmp.Dispose();
                bmp = new Bitmap(width, height) { Tag = img.Tag };
                return bmp;
            }
            finally
            {
                img.Dispose();
            }
        }

        public static void TryOpenUrl(string myPath, string browserPath)
        {
            try
            {
                if (string.IsNullOrEmpty(browserPath))
                {
                    Process.Start(myPath);
                }
                else
                {
                    if (browserPath.StartsWith("\"") && browserPath.Length > 2 && browserPath.IndexOf("\"", 2) > -1)
                    {
                        int sep = browserPath.IndexOf("\"", 2);
                        browserPath = browserPath.Substring(1, sep - 1);
                        string arg = string.Empty;
                        if (sep < browserPath.Length - 1)
                        {
                            arg = browserPath.Substring(sep + 1);
                        }
                        myPath = arg + " " + myPath;
                        Process.Start(browserPath, myPath);
                    }
                    else
                    {
                        Process.Start(browserPath, myPath);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static T GetAppAssemblyCustomeAttr<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(AppAssembly, typeof(T));
        }

        public struct DEFAULTTAB
        {
            public const string RECENT = "Recent";
            public const string REPLY = "Reply";
            public const string DM = "Direct";
            public const string FAV = "Favorites";
        }
    }
}