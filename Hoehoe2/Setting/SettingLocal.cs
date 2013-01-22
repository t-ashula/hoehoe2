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
        private readonly FontConverter _fontConverter = new FontConverter();
        private readonly ColorConverter _colorConverter = new ColorConverter();

        public SettingLocal()
        {
            FormLocation = new Point(0, 0);
            SplitterDistance = 200;
            AdSplitterDistance = 350;
            FormSize = new Size(600, 500);
            StatusText = string.Empty;
            PreviewDistance = -1;
            StatusTextHeight = 38;
            Width1 = 48;
            Width2 = 80;
            Width3 = 290;
            Width4 = 120;
            Width5 = 50;
            Width6 = 16;
            Width7 = 32;
            Width8 = 50;
            DisplayIndex1 = 2;
            DisplayIndex2 = 3;
            DisplayIndex3 = 4;
            DisplayIndex4 = 5;
            DisplayIndex5 = 6;
            DisplayIndex6 = 1;
            DisplayIndex7 = 0;
            DisplayIndex8 = 7;
            BrowserPath = string.Empty;
            ProxyType = HttpConnection.ProxyType.IE;
            ProxyAddress = "127.0.0.1";
            ProxyPort = 80;
            ProxyUser = string.Empty;
            FontUnread = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);
            ColorUnread = SystemColors.ControlText;
            FontRead = SystemFonts.DefaultFont;
            ColorRead = SystemColors.ControlText;
            ColorFav = Color.FromKnownColor(KnownColor.Red);
            ColorOWL = Color.FromKnownColor(KnownColor.Blue);
            ColorRetweet = Color.FromKnownColor(KnownColor.Green);
            FontDetail = SystemFonts.DefaultFont;
            ColorSelf = Color.FromKnownColor(KnownColor.AliceBlue);
            ColorAtSelf = Color.FromKnownColor(KnownColor.AntiqueWhite);
            ColorTarget = Color.FromKnownColor(KnownColor.LemonChiffon);
            ColorAtTarget = Color.FromKnownColor(KnownColor.LavenderBlush);
            ColorAtFromTarget = Color.FromKnownColor(KnownColor.Honeydew);
            ColorAtTo = Color.FromKnownColor(KnownColor.Pink);
            ColorInputBackcolor = Color.FromKnownColor(KnownColor.LemonChiffon);
            ColorInputFont = Color.FromKnownColor(KnownColor.ControlText);
            FontInputFont = SystemFonts.DefaultFont;
            ColorListBackcolor = Color.FromKnownColor(KnownColor.Window);
            ColorDetailBackcolor = Color.FromKnownColor(KnownColor.Window);
            ColorDetail = Color.FromKnownColor(KnownColor.ControlText);
            ColorDetailLink = Color.FromKnownColor(KnownColor.Blue);
            ProxyPassword = string.Empty;
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
            get { return _fontConverter.ConvertToString(FontUnread); }
            set { FontUnread = (Font)_fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorUnread { get; set; }

        public string ColorUnreadStr
        {
            get { return _colorConverter.ConvertToString(ColorUnread); }
            set { ColorUnread = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontRead { get; set; }

        public string FontReadStr
        {
            get { return _fontConverter.ConvertToString(FontRead); }
            set { FontRead = (Font)_fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorRead { get; set; }

        public string ColorReadStr
        {
            get { return _colorConverter.ConvertToString(ColorRead); }
            set { ColorRead = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorFav { get; set; }

        public string ColorFavStr
        {
            get { return _colorConverter.ConvertToString(ColorFav); }
            set { ColorFav = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorOWL { get; set; }

        public string ColorOWLStr
        {
            get { return _colorConverter.ConvertToString(ColorOWL); }
            set { ColorOWL = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorRetweet { get; set; }

        public string ColorRetweetStr
        {
            get { return _colorConverter.ConvertToString(ColorRetweet); }
            set { ColorRetweet = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontDetail { get; set; }

        public string FontDetailStr
        {
            get { return _fontConverter.ConvertToString(FontDetail); }
            set { FontDetail = (Font)_fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorSelf { get; set; }

        public string ColorSelfStr
        {
            get { return _colorConverter.ConvertToString(ColorSelf); }
            set { ColorSelf = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtSelf { get; set; }

        public string ColorAtSelfStr
        {
            get { return _colorConverter.ConvertToString(ColorAtSelf); }
            set { ColorAtSelf = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorTarget { get; set; }

        public string ColorTargetStr
        {
            get { return _colorConverter.ConvertToString(ColorTarget); }
            set { ColorTarget = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtTarget { get; set; }

        public string ColorAtTargetStr
        {
            get { return _colorConverter.ConvertToString(ColorAtTarget); }
            set { ColorAtTarget = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtFromTarget { get; set; }

        public string ColorAtFromTargetStr
        {
            get { return _colorConverter.ConvertToString(ColorAtFromTarget); }
            set { ColorAtFromTarget = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorAtTo { get; set; }

        public string ColorAtToStr
        {
            get { return _colorConverter.ConvertToString(ColorAtTo); }
            set { ColorAtTo = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorInputBackcolor { get; set; }

        public string ColorInputBackcolorStr
        {
            get { return _colorConverter.ConvertToString(ColorInputBackcolor); }
            set { ColorInputBackcolor = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorInputFont { get; set; }

        public string ColorInputFontStr
        {
            get { return _colorConverter.ConvertToString(ColorInputFont); }
            set { ColorInputFont = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Font FontInputFont { get; set; }

        public string FontInputFontStr
        {
            get { return _fontConverter.ConvertToString(FontInputFont); }
            set { FontInputFont = (Font)_fontConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorListBackcolor { get; set; }

        public string ColorListBackcolorStr
        {
            get { return _colorConverter.ConvertToString(ColorListBackcolor); }
            set { ColorListBackcolor = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetailBackcolor { get; set; }

        public string ColorDetailBackcolorStr
        {
            get { return _colorConverter.ConvertToString(ColorDetailBackcolor); }
            set { ColorDetailBackcolor = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetail { get; set; }

        public string ColorDetailStr
        {
            get { return _colorConverter.ConvertToString(ColorDetail); }
            set { ColorDetail = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public Color ColorDetailLink { get; set; }

        public string ColorDetailLinkStr
        {
            get { return _colorConverter.ConvertToString(ColorDetailLink); }
            set { ColorDetailLink = (Color)_colorConverter.ConvertFromString(value); }
        }

        [XmlIgnore]
        public string ProxyPassword { get; set; }

        public string EncryptProxyPassword
        {
            get { return CryptoUtils.TryEncrypt(ProxyPassword); }
            set { ProxyPassword = CryptoUtils.TryDecrypt(value); }
        }

        #region "Settingクラス基本"

        public static SettingLocal Load()
        {
            return LoadSettings();
        }

        public void Save()
        {
            SaveSettings(this);
        }

        #endregion "Settingクラス基本"
    }
}