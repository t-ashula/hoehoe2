// Hoehoe - Client of Twitter
// Copyright (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Hoehoe.DataModels.Twitter;

    public class Configs
    {
        private static Configs instance = new Configs();

        private Configs()
        {
        }

        public static Configs Instance
        {
            get { return instance; }
        }

        public bool HideDuplicatedRetweets { get; set; }
        public bool IsPreviewFoursquare { get; set; }
        public int FoursquarePreviewHeight { get; set; }
        public int FoursquarePreviewWidth { get; set; }
        public int FoursquarePreviewZoom { get; set; }
        public bool IsListStatusesIncludeRts { get; set; }
        public List<UserAccount> UserAccounts { get; set; }
        public bool TabMouseLock { get; set; }
        public bool IsRemoveSameEvent { get; set; }
        public bool IsNotifyUseGrowl { get; set; }
        public Configuration TwitterConfiguration { get; set; }
        public int UserstreamPeriodInt { get; set; }
        public bool UserstreamStartup { get; set; }
        public int TimelinePeriodInt { get; set; }
        public int ReplyPeriodInt { get; set; }
        public int DMPeriodInt { get; set; }
        public int PubSearchPeriodInt { get; set; }
        public int ListsPeriodInt { get; set; }
        public int UserTimelinePeriodInt { get; set; }
        public bool Readed { get; set; }
        public IconSizes IconSz { get; set; }
        public string Status { get; set; }
        public bool UnreadManage { get; set; }
        public bool PlaySound { get; set; }
        public bool OneWayLove { get; set; }

        public Font FontUnread { get; set; }
        public Color ColorUnread { get; set; }
        public Font FontReaded { get; set; } // リストフォントとして使用
        public Color ColorReaded { get; set; }
        public Color ColorFav { get; set; }
        public Color ColorOWL { get; set; }
        public Color ColorRetweet { get; set; }
        public Font FontDetail { get; set; }
        public Color ColorDetail { get; set; }
        public Color ColorDetailLink { get; set; }
        public Color ColorSelf { get; set; }
        public Color ColorAtSelf { get; set; }
        public Color ColorTarget { get; set; }
        public Color ColorAtTarget { get; set; }
        public Color ColorAtFromTarget { get; set; }
        public Color ColorAtTo { get; set; }
        public Color ColorInputBackcolor { get; set; }
        public Color ColorInputFont { get; set; }
        public Font FontInputFont { get; set; }
        public Color ColorListBackcolor { get; set; }
        public Color ColorDetailBackcolor { get; set; }

        public NameBalloonEnum NameBalloon { get; set; }

        public bool PostCtrlEnter { get; set; }
        public bool PostShiftEnter { get; set; }

        public int CountApi { get; set; }
        public int CountApiReply { get; set; }
        public int MoreCountApi { get; set; }
        public int FirstCountApi { get; set; }
        public int SearchCountApi { get; set; }
        public int FavoritesCountApi { get; set; }
        public int UserTimelineCountApi { get; set; }
        public int ListCountApi { get; set; }

        public bool PostAndGet { get; set; }

        public bool UseRecommendStatus { get; set; }
        public string RecommendStatusText { get; set; }

        public bool DispUsername { get; set; }  
        public bool CloseToExit { get; set; }
        public bool MinimizeToTray { get; set; }
        public DispTitleEnum DispLatestPost { get; set; }

        public string BrowserPath { get; set; }

        public bool TinyUrlResolve { get; set; }
        public bool ShortUrlForceResolve { get; set; }

        public bool SortOrderLock { get; set; }

        public HttpConnection.ProxyType SelectedProxyType { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }

        public bool PeriodAdjust { get; set; }

        public bool StartupVersion { get; set; }
        public bool StartupFollowers { get; set; }

        public bool RestrictFavCheck { get; set; }

        public bool AlwaysTop { get; set; }

        public bool UrlConvertAuto { get; set; }

        public bool ShortenTco { get; set; }

        public bool OutputzEnabled { get; set; }

        public string OutputzKey { get; set; }

        public OutputzUrlmode OutputzUrlmode { get; set; }

        public bool Nicoms { get; set; }

        public UrlConverter AutoShortUrlFirst { get; set; }

        public bool UseUnreadStyle { get; set; }

        public string DateTimeFormat { get; set; }

        public int DefaultTimeOut { get; set; }

        public bool RetweetNoConfirm { get; set; }

        public bool TabIconDisp { get; set; }

        public ReplyIconState ReplyIconState { get; set; }

        public bool ReadOwnPost { get; set; }

        public bool GetFav { get; set; }

        public bool IsMonospace { get; set; }

        public bool ReadOldPosts { get; set; }

        public bool UseSsl { get; set; }

        public string BitlyUser { get; set; }

        public string BitlyPwd { get; set; }

        public bool ShowGrid { get; set; }

        public bool UseAtIdSupplement { get; set; }

        public bool UseHashSupplement { get; set; }

        public bool PreviewEnable { get; set; }

        public bool UseAdditionalCount { get; set; }

        public bool OpenUserTimeline { get; set; }

        public string TwitterApiUrl { get; set; }

        public string TwitterSearchApiUrl { get; set; }

        public string Language { get; set; }

        public bool LimitBalloon { get; set; }

        public bool EventNotifyEnabled { get; set; }

        // public EventType EventNotifyFlag
        // public EventType IsMyEventNotifyFlag

        public bool ForceEventNotify { get; set; }

        public bool FavEventUnread { get; set; }

        // public string TranslateLanguage

        public string EventSoundFile { get; set; }

        public int ListDoubleClickAction { get; set; }

        public string UserAppointUrl { get; set; }

        public bool HotkeyEnabled { get; set; }

        public Keys HotkeyKey { get; set; }

        public int HotkeyValue { get; set; }

        public Keys HotkeyMod { get; set; }

        public bool BlinkNewMentions { get; set; }
    }
}
