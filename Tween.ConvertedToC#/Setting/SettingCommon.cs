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

    [Serializable]
    public class SettingCommon : SettingBase<SettingCommon>
    {
        private List<UserAccount> userAccounts;
        public string UserName = string.Empty;

        [XmlIgnore]
        public string Password = string.Empty;

        public string EncryptPassword
        {
            get { return SettingCommon.Encrypt(this.Password); }
            set { this.Password = SettingCommon.Decrypt(value); }
        }

        public string Token = string.Empty;

        [XmlIgnore]
        public string TokenSecret = string.Empty;

        public string EncryptTokenSecret
        {
            get { return SettingCommon.Encrypt(this.TokenSecret); }
            set { this.TokenSecret = SettingCommon.Decrypt(value); }
        }

        public long UserId = 0;
        public List<string> TabList = new List<string>();
        public int TimelinePeriod = 90;
        public int ReplyPeriod = 180;
        public int DMPeriod = 600;
        public int PubSearchPeriod = 180;
        public int ListsPeriod = 180;
        public bool Read = true;
        public bool ListLock = false;
        public IconSizes IconSize = IconSizes.Icon16;
        public bool NewAllPop = true;
        public bool EventNotifyEnabled = true;
        public EventType EventNotifyFlag = EventType.All;
        public EventType IsMyEventNotifyFlag = EventType.All;
        public bool ForceEventNotify = false;
        public bool FavEventUnread = true;
        public string TranslateLanguage = Hoehoe.Properties.Resources.TranslateDefaultLanguage;
        public string EventSoundFile = string.Empty;
        public bool PlaySound = false;
        public bool UnreadManage = true;
        public bool OneWayLove = true;
        public NameBalloonEnum NameBalloon = NameBalloonEnum.NickName;
        public bool PostCtrlEnter = false;
        public bool PostShiftEnter = false;
        public int CountApi = 60;
        public int CountApiReply = 40;
        public bool PostAndGet = true;
        public bool DispUsername = false;
        public bool MinimizeToTray = false;
        public bool CloseToExit = false;
        public DispTitleEnum DispLatestPost = DispTitleEnum.Post;
        public bool SortOrderLock = false;
        public bool TinyUrlResolve = true;
        public bool ShortUrlForceResolve = false;
        public bool PeriodAdjust = true;
        public bool StartupVersion = true;
        public bool StartupFollowers = true;
        public bool RestrictFavCheck = false;
        public bool AlwaysTop = false;
        public string CultureCode = string.Empty;
        public bool UrlConvertAuto = false;
        public bool Outputz = false;
        public int SortColumn = 3;
        public int SortOrder = 1;
        public bool IsMonospace = false;
        public bool ReadOldPosts = false;
        public bool UseSsl = true;
        public string Language = "OS";
        public bool Nicoms = false;
        public List<string> HashTags = new List<string>();
        public string HashSelected = string.Empty;
        public bool HashIsPermanent = false;
        public bool HashIsHead = false;
        public bool HashIsNotAddToAtReply = true;

        public bool PreviewEnable = true;

        [XmlIgnore]
        public string OutputzKey = string.Empty;

        public string EncryptOutputzKey
        {
            get { return SettingCommon.Encrypt(this.OutputzKey); }
            set { this.OutputzKey = SettingCommon.Decrypt(value); }
        }

        public OutputzUrlmode OutputzUrlMode = OutputzUrlmode.twittercom;
        public UrlConverter AutoShortUrlFirst = UrlConverter.Bitly;
        public bool UseUnreadStyle = true;
        public string DateTimeFormat = "yyyy/MM/dd H:mm:ss";
        public int DefaultTimeOut = 20;
        public bool RetweetNoConfirm = false;
        public bool LimitBalloon = false;
        public bool TabIconDisp = true;
        public ReplyIconState ReplyIconState = ReplyIconState.StaticIcon;
        public bool WideSpaceConvert = true;
        public bool ReadOwnPost = false;
        public bool GetFav = true;
        public string BilyUser = string.Empty;
        public string BitlyPwd = string.Empty;
        public bool ShowGrid = false;
        public bool UseAtIdSupplement = true;
        public bool UseHashSupplement = true;
        public string TwitterUrl = "api.twitter.com";
        public string TwitterSearchUrl = "search.twitter.com";
        public bool HotkeyEnabled = false;
        public Keys HotkeyModifier = Keys.None;
        public Keys HotkeyKey = Keys.None;
        public int HotkeyValue = 0;
        public bool BlinkNewMentions = false;
        public bool FocusLockToStatusText = false;
        public bool UseAdditionalCount = false;
        public int MoreCountApi = 200;
        public int FirstCountApi = 100;
        public int SearchCountApi = 100;
        public int FavoritesCountApi = 40;
        public string TrackWord = string.Empty;
        public bool AllAtReply = false;
        public bool UserstreamStartup = true;
        public int UserstreamPeriod = 0;
        public int UserTimelineCountApi = 20;
        public int UserTimelinePeriod = 600;
        public bool OpenUserTimeline = true;
        public int ListCountApi = 100;
        public int UseImageService = 0;
        public int ListDoubleClickAction = 0;
        public string UserAppointUrl = string.Empty;
        public bool HideDuplicatedRetweets = false;
        public bool IsPreviewFoursquare = false;
        public int FoursquarePreviewHeight = 300;
        public int FoursquarePreviewWidth = 300;
        public int FoursquarePreviewZoom = 15;
        public bool IsListsIncludeRts = false;
        public long GAFirst = 0;
        public long GALast = 0;
        public bool TabMouseLock = false;
        public bool IsRemoveSameEvent = false;
        public bool IsUseNotifyGrowl = false;

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

        private static string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = string.Empty;
            }

            if (password.Length > 0)
            {
                try
                {
                    return CryptoUtils.EncryptString(password);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private static string Decrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = string.Empty;
            }

            if (password.Length > 0)
            {
                try
                {
                    password = CryptoUtils.DecryptString(password);
                }
                catch (Exception)
                {
                    password = string.Empty;
                }
            }

            return password;
        }

        public List<UserAccount> UserAccounts
        {
            get { return userAccounts; }
            set { userAccounts = value; }
        }
    }
}