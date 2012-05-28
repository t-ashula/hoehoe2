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
    using System.Drawing;
    using System.Xml.Serialization;

    [Serializable]
    public class SettingLocal : SettingBase<SettingLocal>
    {
        private FontConverter fontConverter = new FontConverter();
        private ColorConverter colorConverter = new ColorConverter();

        public SettingLocal()
        {
            this.FormLocation = new Point(0, 0);
            this.SplitterDistance = 200;
            this.AdSplitterDistance = 350;
            this.FormSize = new Size(600, 500);
            this.StatusText = string.Empty;
            this.PreviewDistance = -1;
            this.StatusTextHeight = 38;
            this.Width1 = 48;
            this.Width2 = 80;
            this.Width3 = 290;
            this.Width4 = 120;
            this.Width5 = 50;
            this.Width6 = 16;
            this.Width7 = 32;
            this.Width8 = 50;
            this.DisplayIndex1 = 2;
            this.DisplayIndex2 = 3;
            this.DisplayIndex3 = 4;
            this.DisplayIndex4 = 5;
            this.DisplayIndex5 = 6;
            this.DisplayIndex6 = 1;
            this.DisplayIndex7 = 0;
            this.DisplayIndex8 = 7;
            this.BrowserPath = string.Empty;
            this.ProxyType = HttpConnection.ProxyType.IE;
            this.ProxyAddress = "127.0.0.1";
            this.ProxyPort = 80;
            this.ProxyUser = string.Empty;
            this.FontUnread = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);
            this.ColorUnread = SystemColors.ControlText;
            this.FontRead = SystemFonts.DefaultFont;
            this.ColorRead = SystemColors.ControlText;
            this.ColorFav = Color.FromKnownColor(KnownColor.Red);
            this.ColorOWL = Color.FromKnownColor(KnownColor.Blue);
            this.ColorRetweet = Color.FromKnownColor(KnownColor.Green);
            this.FontDetail = SystemFonts.DefaultFont;
            this.ColorSelf = Color.FromKnownColor(KnownColor.AliceBlue);
            this.ColorAtSelf = Color.FromKnownColor(KnownColor.AntiqueWhite);
            this.ColorTarget = Color.FromKnownColor(KnownColor.LemonChiffon);
            this.ColorAtTarget = Color.FromKnownColor(KnownColor.LavenderBlush);
            this.ColorAtFromTarget = Color.FromKnownColor(KnownColor.Honeydew);
            this.ColorAtTo = Color.FromKnownColor(KnownColor.Pink);
            this.ColorInputBackcolor = Color.FromKnownColor(KnownColor.LemonChiffon);
            this.ColorInputFont = Color.FromKnownColor(KnownColor.ControlText);
            this.FontInputFont = SystemFonts.DefaultFont;
            this.ColorListBackcolor = Color.FromKnownColor(KnownColor.Window);
            this.ColorDetailBackcolor = Color.FromKnownColor(KnownColor.Window);
            this.ColorDetail = Color.FromKnownColor(KnownColor.ControlText);
            this.ColorDetailLink = Color.FromKnownColor(KnownColor.Blue);
            this.ProxyPassword = string.Empty;
        }

        public Point FormLocation { get; set; }

        public int SplitterDistance { get; set; }

        public int AdSplitterDistance { get; set; }

        public Size FormSize { get; set; }

        public string StatusText { get; set; }

        public bool UseRecommendStatus { get; set; }

        public int Width1 { get; set; }

        public int Width2 { get; set; }

        public int Width3 { get; set; }

        public int Width4 { get; set; }

        public int Width5 { get; set; }

        public int Width6 { get; set; }

        public int Width7 { get; set; }

        public int Width8 { get; set; }

        public int DisplayIndex1 { get; set; }

        public int DisplayIndex2 { get; set; }

        public int DisplayIndex3 { get; set; }

        public int DisplayIndex4 { get; set; }

        public int DisplayIndex5 { get; set; }

        public int DisplayIndex6 { get; set; }

        public int DisplayIndex7 { get; set; }

        public int DisplayIndex8 { get; set; }

        public string BrowserPath { get; set; }

        public HttpConnection.ProxyType ProxyType { get; set; }

        public string ProxyAddress { get; set; }

        public int ProxyPort { get; set; }

        public string ProxyUser { get; set; }

        public bool StatusMultiline { get; set; }

        public int StatusTextHeight { get; set; }

        public int PreviewDistance { get; set; }

        [XmlIgnore]
        public Font FontUnread { get; set; }

        public string FontUnreadStr
        {
            get { return this.fontConverter.ConvertToString(this.FontUnread); }
            set { this.FontUnread = (Font)this.fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorUnread { get; set; }

        public string ColorUnreadStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorUnread); }
            set { this.ColorUnread = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontRead { get; set; }

        public string FontReadStr
        {
            get { return this.fontConverter.ConvertToString(this.FontRead); }
            set { this.FontRead = (Font)this.fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorRead { get; set; }

        public string ColorReadStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorRead); }
            set { this.ColorRead = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorFav { get; set; }

        public string ColorFavStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorFav); }
            set { this.ColorFav = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorOWL { get; set; }

        public string ColorOWLStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorOWL); }
            set { this.ColorOWL = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorRetweet { get; set; }

        public string ColorRetweetStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorRetweet); }
            set { this.ColorRetweet = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontDetail { get; set; }

        public string FontDetailStr
        {
            get { return this.fontConverter.ConvertToString(this.FontDetail); }
            set { this.FontDetail = (Font)this.fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorSelf { get; set; }

        public string ColorSelfStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorSelf); }
            set { this.ColorSelf = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtSelf { get; set; }

        public string ColorAtSelfStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorAtSelf); }
            set { this.ColorAtSelf = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorTarget { get; set; }

        public string ColorTargetStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorTarget); }
            set { this.ColorTarget = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtTarget { get; set; }

        public string ColorAtTargetStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorAtTarget); }
            set { this.ColorAtTarget = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtFromTarget { get; set; }

        public string ColorAtFromTargetStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorAtFromTarget); }
            set { this.ColorAtFromTarget = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtTo { get; set; }

        public string ColorAtToStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorAtTo); }
            set { this.ColorAtTo = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorInputBackcolor { get; set; }

        public string ColorInputBackcolorStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorInputBackcolor); }
            set { this.ColorInputBackcolor = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorInputFont { get; set; }

        public string ColorInputFontStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorInputFont); }
            set { this.ColorInputFont = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontInputFont { get; set; }

        public string FontInputFontStr
        {
            get { return this.fontConverter.ConvertToString(this.FontInputFont); }
            set { this.FontInputFont = (Font)this.fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorListBackcolor { get; set; }

        public string ColorListBackcolorStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorListBackcolor); }
            set { this.ColorListBackcolor = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetailBackcolor { get; set; }

        public string ColorDetailBackcolorStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorDetailBackcolor); }
            set { this.ColorDetailBackcolor = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetail { get; set; }

        public string ColorDetailStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorDetail); }
            set { this.ColorDetail = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetailLink { get; set; }

        public string ColorDetailLinkStr
        {
            get { return this.colorConverter.ConvertToString(this.ColorDetailLink); }
            set { this.ColorDetailLink = (Color)this.colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public string ProxyPassword { get; set; }

        public string EncryptProxyPassword
        {
            get
            {
                string pwd = this.ProxyPassword;
                if (string.IsNullOrEmpty(pwd))
                {
                    return string.Empty;
                }

                try
                {
                    return CryptoUtils.EncryptString(pwd);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            set
            {
                string pwd = value;
                if (string.IsNullOrEmpty(pwd))
                {
                    pwd = string.Empty;
                }

                if (pwd.Length > 0)
                {
                    try
                    {
                        pwd = CryptoUtils.DecryptString(pwd);
                    }
                    catch (Exception)
                    {
                        pwd = string.Empty;
                    }
                }

                this.ProxyPassword = pwd;
            }
        }

        #region "Settingクラス基本"

        public static SettingLocal Load()
        {
            return SettingLocal.LoadSettings();
        }

        public void Save()
        {
            SettingLocal.SaveSettings(this);
        }

        #endregion "Settingクラス基本"
    }
}