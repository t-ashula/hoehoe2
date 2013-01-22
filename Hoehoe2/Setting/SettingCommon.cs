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
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using R = Hoehoe.Properties.Resources;

    [Serializable]
    public class SettingCommon : SettingBase<SettingCommon>
    {
        public SettingCommon()
        {
            UserName = string.Empty;
            Password = string.Empty;
            Token = string.Empty;
            TokenSecret = string.Empty;
            UserId = 0;
            TabList = new List<string>();
            TimelinePeriod = 90;
            ReplyPeriod = 180;
            DMPeriod = 600;
            PubSearchPeriod = 180;
            ListsPeriod = 180;
            Read = true;
            ListLock = false;
            IconSize = IconSizes.Icon16;
            NewAllPop = true;
            EventNotifyEnabled = true;
            EventNotifyFlag = EventType.All;
            IsMyEventNotifyFlag = EventType.All;
            ForceEventNotify = false;
            FavEventUnread = true;
            TranslateLanguage = R.TranslateDefaultLanguage;
            EventSoundFile = string.Empty;
            PlaySound = false;
            UnreadManage = true;
            OneWayLove = true;
            NameBalloon = NameBalloonEnum.NickName;
            PostCtrlEnter = false;
            PostShiftEnter = false;
            CountApi = 60;
            CountApiReply = 40;
            PostAndGet = true;
            DispUsername = false;
            MinimizeToTray = false;
            CloseToExit = false;
            DispLatestPost = DispTitleEnum.Post;
            SortOrderLock = false;
            TinyUrlResolve = true;
            ShortUrlForceResolve = false;
            PeriodAdjust = true;
            StartupVersion = true;
            StartupFollowers = true;
            RestrictFavCheck = false;
            AlwaysTop = false;
            CultureCode = string.Empty;
            UrlConvertAuto = false;
            Outputz = false;
            SortColumn = 3;
            SortOrder = 1;
            IsMonospace = false;
            ReadOldPosts = false;
            UseSsl = true;
            Language = "OS";
            Nicoms = false;
            HashTags = new List<string>();
            HashSelected = string.Empty;
            HashIsPermanent = false;
            HashIsHead = false;
            HashIsNotAddToAtReply = true;
            PreviewEnable = true;
            OutputzKey = string.Empty;
            OutputzUrlMode = OutputzUrlmode.twittercom;
            AutoShortUrlFirst = UrlConverter.Bitly;
            UseUnreadStyle = true;
            DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            DefaultTimeOut = 20;
            RetweetNoConfirm = false;
            LimitBalloon = false;
            TabIconDisp = true;
            ReplyIconState = ReplyIconState.StaticIcon;
            WideSpaceConvert = true;
            ReadOwnPost = false;
            GetFav = true;
            BilyUser = string.Empty;
            BitlyPwd = string.Empty;
            ShowGrid = false;
            UseAtIdSupplement = true;
            UseHashSupplement = true;
            TwitterUrl = "api.twitter.com";
            TwitterSearchUrl = "search.twitter.com";
            HotkeyEnabled = false;
            HotkeyModifier = Keys.None;
            HotkeyKey = Keys.None;
            HotkeyValue = 0;
            BlinkNewMentions = false;
            FocusLockToStatusText = false;
            UseAdditionalCount = false;
            MoreCountApi = 200;
            FirstCountApi = 100;
            SearchCountApi = 100;
            FavoritesCountApi = 40;
            TrackWord = string.Empty;
            AllAtReply = false;
            UserstreamStartup = true;
            UserstreamPeriod = 0;
            UserTimelineCountApi = 20;
            UserTimelinePeriod = 600;
            OpenUserTimeline = true;
            ListCountApi = 100;
            UseImageService = 0;
            ListDoubleClickAction = 0;
            UserAppointUrl = string.Empty;
            HideDuplicatedRetweets = false;
            IsPreviewFoursquare = false;
            FoursquarePreviewHeight = 300;
            FoursquarePreviewWidth = 300;
            FoursquarePreviewZoom = 15;
            IsListsIncludeRts = false;
            GAFirst = 0;
            GALast = 0;
            TabMouseLock = false;
            IsRemoveSameEvent = false;
            IsUseNotifyGrowl = false;
        }

        public string UserName { get; set; }

        [XmlIgnore]
        public string Password { get; set; }

        public string EncryptPassword
        {
            get { return CryptoUtils.TryEncrypt(Password); }
            set { Password = CryptoUtils.TryDecrypt(value); }
        }

        public string Token { get; set; }

        [XmlIgnore]
        public string TokenSecret { get; set; }

        public string EncryptTokenSecret
        {
            get { return CryptoUtils.TryEncrypt(TokenSecret); }
            set { TokenSecret = CryptoUtils.TryDecrypt(value); }
        }

        public long UserId { get; set; }

        public List<string> TabList { get; set; }

        public int TimelinePeriod { get; set; }

        public int ReplyPeriod { get; set; }

        public int DMPeriod { get; set; }

        public int PubSearchPeriod { get; set; }

        public int ListsPeriod { get; set; }

        public bool Read { get; set; }

        public bool ListLock { get; set; }

        public IconSizes IconSize { get; set; }

        public bool NewAllPop { get; set; }

        public bool EventNotifyEnabled { get; set; }

        public EventType EventNotifyFlag { get; set; }

        public EventType IsMyEventNotifyFlag { get; set; }

        public bool ForceEventNotify { get; set; }

        public bool FavEventUnread { get; set; }

        public string TranslateLanguage { get; set; }

        public string EventSoundFile { get; set; }

        public bool PlaySound { get; set; }

        public bool UnreadManage { get; set; }

        public bool OneWayLove { get; set; }

        public NameBalloonEnum NameBalloon { get; set; }

        public bool PostCtrlEnter { get; set; }

        public bool PostShiftEnter { get; set; }

        public int CountApi { get; set; }

        public int CountApiReply { get; set; }

        public bool PostAndGet { get; set; }

        public bool DispUsername { get; set; }

        public bool MinimizeToTray { get; set; }

        public bool CloseToExit { get; set; }

        public DispTitleEnum DispLatestPost { get; set; }

        public bool SortOrderLock { get; set; }

        public bool TinyUrlResolve { get; set; }

        public bool ShortUrlForceResolve { get; set; }

        public bool PeriodAdjust { get; set; }

        public bool StartupVersion { get; set; }

        public bool StartupFollowers { get; set; }

        public bool RestrictFavCheck { get; set; }

        public bool AlwaysTop { get; set; }

        public string CultureCode { get; set; }

        public bool UrlConvertAuto { get; set; }

        public bool Outputz { get; set; }

        public int SortColumn { get; set; }

        public int SortOrder { get; set; }

        public bool IsMonospace { get; set; }

        public bool ReadOldPosts { get; set; }

        public bool UseSsl { get; set; }

        public string Language { get; set; }

        public bool Nicoms { get; set; }

        public List<string> HashTags { get; set; }

        public string HashSelected { get; set; }

        public bool HashIsPermanent { get; set; }

        public bool HashIsHead { get; set; }

        public bool HashIsNotAddToAtReply { get; set; }

        public bool PreviewEnable { get; set; }

        [XmlIgnore]
        public string OutputzKey { get; set; }

        public string EncryptOutputzKey
        {
            get { return CryptoUtils.TryEncrypt(OutputzKey); }
            set { OutputzKey = CryptoUtils.TryDecrypt(value); }
        }

        public OutputzUrlmode OutputzUrlMode { get; set; }

        public UrlConverter AutoShortUrlFirst { get; set; }

        public bool UseUnreadStyle { get; set; }

        public string DateTimeFormat { get; set; }

        public int DefaultTimeOut { get; set; }

        public bool RetweetNoConfirm { get; set; }

        public bool LimitBalloon { get; set; }

        public bool TabIconDisp { get; set; }

        public ReplyIconState ReplyIconState { get; set; }

        public bool WideSpaceConvert { get; set; }

        public bool ReadOwnPost { get; set; }

        public bool GetFav { get; set; }

        public string BilyUser { get; set; }

        public string BitlyPwd { get; set; }

        public bool ShowGrid { get; set; }

        public bool UseAtIdSupplement { get; set; }

        public bool UseHashSupplement { get; set; }

        public string TwitterUrl { get; set; }

        public string TwitterSearchUrl { get; set; }

        public bool HotkeyEnabled { get; set; }

        public Keys HotkeyModifier { get; set; }

        public Keys HotkeyKey { get; set; }

        public int HotkeyValue { get; set; }

        public bool BlinkNewMentions { get; set; }

        public bool FocusLockToStatusText { get; set; }

        public bool UseAdditionalCount { get; set; }

        public int MoreCountApi { get; set; }

        public int FirstCountApi { get; set; }

        public int SearchCountApi { get; set; }

        public int FavoritesCountApi { get; set; }

        public string TrackWord { get; set; }

        public bool AllAtReply { get; set; }

        public bool UserstreamStartup { get; set; }

        public int UserstreamPeriod { get; set; }

        public int UserTimelineCountApi { get; set; }

        public int UserTimelinePeriod { get; set; }

        public bool OpenUserTimeline { get; set; }

        public int ListCountApi { get; set; }

        public int UseImageService { get; set; }

        public int ListDoubleClickAction { get; set; }

        public string UserAppointUrl { get; set; }

        public bool HideDuplicatedRetweets { get; set; }

        public bool IsPreviewFoursquare { get; set; }

        public int FoursquarePreviewHeight { get; set; }

        public int FoursquarePreviewWidth { get; set; }

        public int FoursquarePreviewZoom { get; set; }

        public bool IsListsIncludeRts { get; set; }

        public long GAFirst { get; set; }

        public long GALast { get; set; }

        public bool TabMouseLock { get; set; }

        public bool IsRemoveSameEvent { get; set; }

        public bool IsUseNotifyGrowl { get; set; }

        public List<UserAccount> UserAccounts { get; set; }

        #region "Settingクラス基本"

        public static SettingCommon Load()
        {
            return SettingCommon.LoadSettings();
        }

        public void Save()
        {
            SettingCommon.SaveSettings(this);
        }

        #endregion "Settingクラス基本"
    }
}