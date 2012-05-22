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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using Hoehoe.DataModels;

    public partial class AppendSettingDialog
    {
        private static AppendSettingDialog _instance = new AppendSettingDialog();
        private Twitter _tw;
        private int _MytimelinePeriod;
        private int _MyDMPeriod;
        private int _MyPubSearchPeriod;
        private int _MyListsPeriod;
        private int _MyUserTimelinePeriod;
        private bool _MyReaded;
        private Hoehoe.IconSizes _MyIconSize;
        private string _MyStatusText;
        private string _MyRecommendStatusText;
        private bool _MyUnreadManage;
        private bool _MyPlaySound;
        private bool _MyOneWayLove;
        private Font _fntUnread;
        private Color _clUnread;
        private Font _fntReaded;
        private Color _clReaded;
        private Color _clFav;
        private Color _clOWL;
        private Color _clRetweet;
        private Font _fntDetail;
        private Color _clSelf;
        private Color _clAtSelf;
        private Color _clTarget;
        private Color _clAtTarget;
        private Color _clAtFromTarget;
        private Color _clAtTo;
        private Color _clInputBackcolor;
        private Color _clInputFont;
        private Font _fntInputFont;
        private Color _clListBackcolor;
        private Color _clDetailBackcolor;
        private Color _clDetail;
        private Color _clDetailLink;
        private NameBalloonEnum _myNameBalloon;
        private bool _myPostCtrlEnter;
        private bool _myPostShiftEnter;
        private int _countApi;
        private int _countApiReply;
        private string _browserpath;
        private bool _myUseRecommendStatus;
        private bool _myDispUsername;
        private DispTitleEnum _MyDispLatestPost;
        private bool _MySortOrderLock;
        private bool _MyMinimizeToTray;
        private bool _MyCloseToExit;
        private bool _MyTinyUrlResolve;
        private bool _MyShortUrlForceResolve;
        private HttpConnection.ProxyType _MyProxyType;
        private string _MyProxyAddress;
        private int _MyProxyPort;
        private string _MyProxyUser;
        private string _MyProxyPassword;
        private bool _MyPeriodAdjust;
        private bool _MyStartupVersion;
        private bool _MyStartupFollowers;
        private bool _MyRestrictFavCheck;
        private bool _MyAlwaysTop;
        private bool _MyUrlConvertAuto;
        private bool _MyShortenTco;
        private bool _MyOutputz;
        private string _MyOutputzKey;
        private OutputzUrlmode _MyOutputzUrlmode;
        private bool _MyNicoms;
        private bool _MyUnreadStyle;
        private string _MyDateTimeFormat;
        private int _MyDefaultTimeOut;
        private bool _MyLimitBalloon;
        private bool _MyPostAndGet;
        private int _MyReplyPeriod;
        private UrlConverter _MyAutoShortUrlFirst;
        private bool _MyTabIconDisp;
        private ReplyIconState _MyReplyIconState;
        private bool _MyReadOwnPost;
        private bool _MyGetFav;
        private bool _MyMonoSpace;
        private bool _MyReadOldPosts;
        private bool _MyUseSsl;
        private string _MyBitlyId;
        private string _MyBitlyPw;
        private bool _MyShowGrid;
        private bool _MyUseAtIdSupplement;
        private bool _MyUseHashSupplement;
        private string _MyLanguage;
        private string _MyTwitterApiUrl;
        private string _MyTwitterSearchApiUrl;
        private bool _MyPreviewEnable;
        private int _MoreCountApi;
        private int _FirstCountApi;
        private bool _MyUseAdditonalCount;
        private int _SearchCountApi;
        private int _FavoritesCountApi;
        private int _UserTimelineCountApi;
        private int _ListCountApi;
        private bool _MyRetweetNoConfirm;
        private bool _MyUserstreamStartup;
        private bool _MyOpenUserTimeline;
        private bool _ValidationError = false;
        private bool _MyEventNotifyEnabled;
        private EventType _MyEventNotifyFlag;
        private EventType _isMyEventNotifyFlag;
        private bool _MyForceEventNotify;
        private bool _MyFavEventUnread;
        private string _MyTranslateLanguage;
        private string _MyEventSoundFile;
        private int _MyUserstreamPeriod;
        private int _MyDoubleClickAction;
        private string _UserAppointUrl;
        private long _InitialUserId;
        private string _pin;
        private EventCheckboxTblElement[] eventCheckboxTableElements = null;

        public AppendSettingDialog()
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            this.Icon = Hoehoe.Properties.Resources.MIcon;
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

        public DataModels.Twitter.Configuration TwitterConfiguration { get; set; }

        public event IntervalChangedEventHandler IntervalChanged;

        public delegate void IntervalChangedEventHandler(object sender, IntervalChangedEventArgs e);

        public int UserstreamPeriodInt
        {
            get { return this._MyUserstreamPeriod; }
            set { this._MyUserstreamPeriod = value; }
        }

        public bool UserstreamStartup
        {
            get { return this._MyUserstreamStartup; }
            set { this._MyUserstreamStartup = value; }
        }

        public int TimelinePeriodInt
        {
            get { return this._MytimelinePeriod; }
            set { this._MytimelinePeriod = value; }
        }

        public int ReplyPeriodInt
        {
            get { return this._MyReplyPeriod; }
            set { this._MyReplyPeriod = value; }
        }

        public int DMPeriodInt
        {
            get { return this._MyDMPeriod; }
            set { this._MyDMPeriod = value; }
        }

        public int PubSearchPeriodInt
        {
            get { return this._MyPubSearchPeriod; }
            set { this._MyPubSearchPeriod = value; }
        }

        public int ListsPeriodInt
        {
            get { return this._MyListsPeriod; }
            set { this._MyListsPeriod = value; }
        }

        public int UserTimelinePeriodInt
        {
            get { return this._MyUserTimelinePeriod; }
            set { this._MyUserTimelinePeriod = value; }
        }

        public bool Readed
        {
            get { return this._MyReaded; }
            set { this._MyReaded = value; }
        }

        public IconSizes IconSz
        {
            get { return this._MyIconSize; }
            set { this._MyIconSize = value; }
        }

        public string Status
        {
            get { return this._MyStatusText; }
            set { this._MyStatusText = value; }
        }

        public bool UnreadManage
        {
            get { return this._MyUnreadManage; }
            set { this._MyUnreadManage = value; }
        }

        public bool PlaySound
        {
            get { return this._MyPlaySound; }
            set { this._MyPlaySound = value; }
        }

        public bool OneWayLove
        {
            get { return this._MyOneWayLove; }
            set { this._MyOneWayLove = value; }
        }

        // 未使用
        public Font FontUnread
        {
            get { return this._fntUnread; }
            set { this._fntUnread = value; }
        }

        public Color ColorUnread
        {
            get { return this._clUnread; }
            set { this._clUnread = value; }
        }

        // リストフォントとして使用
        public Font FontReaded
        {
            get { return this._fntReaded; }
            set { this._fntReaded = value; }
        }

        public Color ColorReaded
        {
            get { return this._clReaded; }
            set { this._clReaded = value; }
        }

        public Color ColorFav
        {
            get { return this._clFav; }
            set { this._clFav = value; }
        }

        public Color ColorOWL
        {
            get { return this._clOWL; }
            set { this._clOWL = value; }
        }

        public Color ColorRetweet
        {
            get { return this._clRetweet; }
            set { this._clRetweet = value; }
        }

        public Font FontDetail
        {
            get { return this._fntDetail; }
            set { this._fntDetail = value; }
        }

        public Color ColorDetail
        {
            get { return this._clDetail; }
            set { this._clDetail = value; }
        }

        public Color ColorDetailLink
        {
            get { return this._clDetailLink; }
            set { this._clDetailLink = value; }
        }

        public Color ColorSelf
        {
            get { return this._clSelf; }
            set { this._clSelf = value; }
        }

        public Color ColorAtSelf
        {
            get { return this._clAtSelf; }
            set { this._clAtSelf = value; }
        }

        public Color ColorTarget
        {
            get { return this._clTarget; }
            set { this._clTarget = value; }
        }

        public Color ColorAtTarget
        {
            get { return this._clAtTarget; }
            set { this._clAtTarget = value; }
        }

        public Color ColorAtFromTarget
        {
            get { return this._clAtFromTarget; }
            set { this._clAtFromTarget = value; }
        }

        public Color ColorAtTo
        {
            get { return this._clAtTo; }
            set { this._clAtTo = value; }
        }

        public Color ColorInputBackcolor
        {
            get { return this._clInputBackcolor; }
            set { this._clInputBackcolor = value; }
        }

        public Color ColorInputFont
        {
            get { return this._clInputFont; }
            set { this._clInputFont = value; }
        }

        public Font FontInputFont
        {
            get { return this._fntInputFont; }
            set { this._fntInputFont = value; }
        }

        public Color ColorListBackcolor
        {
            get { return this._clListBackcolor; }
            set { this._clListBackcolor = value; }
        }

        public Color ColorDetailBackcolor
        {
            get { return this._clDetailBackcolor; }
            set { this._clDetailBackcolor = value; }
        }

        public NameBalloonEnum NameBalloon
        {
            get { return this._myNameBalloon; }
            set { this._myNameBalloon = value; }
        }

        public bool PostCtrlEnter
        {
            get { return this._myPostCtrlEnter; }
            set { this._myPostCtrlEnter = value; }
        }

        public bool PostShiftEnter
        {
            get { return this._myPostShiftEnter; }
            set { this._myPostShiftEnter = value; }
        }

        public int CountApi
        {
            get { return this._countApi; }
            set { this._countApi = value; }
        }

        public int CountApiReply
        {
            get { return this._countApiReply; }
            set { this._countApiReply = value; }
        }

        public int MoreCountApi
        {
            get { return this._MoreCountApi; }
            set { this._MoreCountApi = value; }
        }

        public int FirstCountApi
        {
            get { return this._FirstCountApi; }
            set { this._FirstCountApi = value; }
        }

        public int SearchCountApi
        {
            get { return this._SearchCountApi; }
            set { this._SearchCountApi = value; }
        }

        public int FavoritesCountApi
        {
            get { return this._FavoritesCountApi; }
            set { this._FavoritesCountApi = value; }
        }

        public int UserTimelineCountApi
        {
            get { return this._UserTimelineCountApi; }
            set { this._UserTimelineCountApi = value; }
        }

        public int ListCountApi
        {
            get { return this._ListCountApi; }
            set { this._ListCountApi = value; }
        }

        public bool PostAndGet
        {
            get { return this._MyPostAndGet; }
            set { this._MyPostAndGet = value; }
        }

        public bool UseRecommendStatus
        {
            get { return this._myUseRecommendStatus; }
            set { this._myUseRecommendStatus = value; }
        }

        public string RecommendStatusText
        {
            get { return this._MyRecommendStatusText; }
            set { this._MyRecommendStatusText = value; }
        }

        public bool DispUsername
        {
            get { return this._myDispUsername; }
            set { this._myDispUsername = value; }
        }

        public bool CloseToExit
        {
            get { return this._MyCloseToExit; }
            set { this._MyCloseToExit = value; }
        }

        public bool MinimizeToTray
        {
            get { return this._MyMinimizeToTray; }
            set { this._MyMinimizeToTray = value; }
        }

        public DispTitleEnum DispLatestPost
        {
            get { return this._MyDispLatestPost; }
            set { this._MyDispLatestPost = value; }
        }

        public string BrowserPath
        {
            get { return this._browserpath; }
            set { this._browserpath = value; }
        }

        public bool TinyUrlResolve
        {
            get { return this._MyTinyUrlResolve; }
            set { this._MyTinyUrlResolve = value; }
        }

        public bool ShortUrlForceResolve
        {
            get { return this._MyShortUrlForceResolve; }
            set { this._MyShortUrlForceResolve = value; }
        }

        public bool SortOrderLock
        {
            get { return this._MySortOrderLock; }
            set { this._MySortOrderLock = value; }
        }

        public HttpConnection.ProxyType SelectedProxyType
        {
            get { return this._MyProxyType; }
            set { this._MyProxyType = value; }
        }

        public string ProxyAddress
        {
            get { return this._MyProxyAddress; }
            set { this._MyProxyAddress = value; }
        }

        public int ProxyPort
        {
            get { return this._MyProxyPort; }
            set { this._MyProxyPort = value; }
        }

        public string ProxyUser
        {
            get { return this._MyProxyUser; }
            set { this._MyProxyUser = value; }
        }

        public string ProxyPassword
        {
            get { return this._MyProxyPassword; }
            set { this._MyProxyPassword = value; }
        }

        public bool PeriodAdjust
        {
            get { return this._MyPeriodAdjust; }
            set { this._MyPeriodAdjust = value; }
        }

        public bool StartupVersion
        {
            get { return this._MyStartupVersion; }
            set { this._MyStartupVersion = value; }
        }

        public bool StartupFollowers
        {
            get { return this._MyStartupFollowers; }
            set { this._MyStartupFollowers = value; }
        }

        public bool RestrictFavCheck
        {
            get { return this._MyRestrictFavCheck; }
            set { this._MyRestrictFavCheck = value; }
        }

        public bool AlwaysTop
        {
            get { return this._MyAlwaysTop; }
            set { this._MyAlwaysTop = value; }
        }

        public bool UrlConvertAuto
        {
            get { return this._MyUrlConvertAuto; }
            set { this._MyUrlConvertAuto = value; }
        }

        public bool ShortenTco
        {
            get { return this._MyShortenTco; }
            set { this._MyShortenTco = value; }
        }

        public bool OutputzEnabled
        {
            get { return this._MyOutputz; }
            set { this._MyOutputz = value; }
        }

        public string OutputzKey
        {
            get { return this._MyOutputzKey; }
            set { this._MyOutputzKey = value; }
        }

        public OutputzUrlmode OutputzUrlmode
        {
            get { return this._MyOutputzUrlmode; }
            set { this._MyOutputzUrlmode = value; }
        }

        public bool Nicoms
        {
            get { return this._MyNicoms; }
            set { this._MyNicoms = value; }
        }

        public UrlConverter AutoShortUrlFirst
        {
            get { return this._MyAutoShortUrlFirst; }
            set { this._MyAutoShortUrlFirst = value; }
        }

        public bool UseUnreadStyle
        {
            get { return this._MyUnreadStyle; }
            set { this._MyUnreadStyle = value; }
        }

        public string DateTimeFormat
        {
            get { return this._MyDateTimeFormat; }
            set { this._MyDateTimeFormat = value; }
        }

        public int DefaultTimeOut
        {
            get { return this._MyDefaultTimeOut; }
            set { this._MyDefaultTimeOut = value; }
        }

        public bool RetweetNoConfirm
        {
            get { return this._MyRetweetNoConfirm; }
            set { this._MyRetweetNoConfirm = value; }
        }

        public bool TabIconDisp
        {
            get { return this._MyTabIconDisp; }
            set { this._MyTabIconDisp = value; }
        }

        public ReplyIconState ReplyIconState
        {
            get { return this._MyReplyIconState; }
            set { this._MyReplyIconState = value; }
        }

        public bool ReadOwnPost
        {
            get { return this._MyReadOwnPost; }
            set { this._MyReadOwnPost = value; }
        }

        public bool GetFav
        {
            get { return this._MyGetFav; }
            set { this._MyGetFav = value; }
        }

        public bool IsMonospace
        {
            get { return this._MyMonoSpace; }
            set { this._MyMonoSpace = value; }
        }

        public bool ReadOldPosts
        {
            get { return this._MyReadOldPosts; }
            set { this._MyReadOldPosts = value; }
        }

        public bool UseSsl
        {
            get { return this._MyUseSsl; }
            set { this._MyUseSsl = value; }
        }

        public string BitlyUser
        {
            get { return this._MyBitlyId; }
            set { this._MyBitlyId = value; }
        }

        public string BitlyPwd
        {
            get { return this._MyBitlyPw; }
            set { this._MyBitlyPw = value; }
        }

        public bool ShowGrid
        {
            get { return this._MyShowGrid; }
            set { this._MyShowGrid = value; }
        }

        public bool UseAtIdSupplement
        {
            get { return this._MyUseAtIdSupplement; }
            set { this._MyUseAtIdSupplement = value; }
        }

        public bool UseHashSupplement
        {
            get { return this._MyUseHashSupplement; }
            set { this._MyUseHashSupplement = value; }
        }

        public bool PreviewEnable
        {
            get { return this._MyPreviewEnable; }
            set { this._MyPreviewEnable = value; }
        }

        public bool UseAdditionalCount
        {
            get { return this._MyUseAdditonalCount; }
            set { this._MyUseAdditonalCount = value; }
        }

        public bool OpenUserTimeline
        {
            get { return this._MyOpenUserTimeline; }
            set { this._MyOpenUserTimeline = value; }
        }

        public string TwitterApiUrl
        {
            get { return this._MyTwitterApiUrl; }
            set { this._MyTwitterApiUrl = value; }
        }

        public string TwitterSearchApiUrl
        {
            get { return this._MyTwitterSearchApiUrl; }
            set { this._MyTwitterSearchApiUrl = value; }
        }

        public string Language
        {
            get { return this._MyLanguage; }
            set { this._MyLanguage = value; }
        }

        public bool LimitBalloon
        {
            get { return this._MyLimitBalloon; }
            set { this._MyLimitBalloon = value; }
        }

        public bool EventNotifyEnabled
        {
            get { return this._MyEventNotifyEnabled; }
            set { this._MyEventNotifyEnabled = value; }
        }

        public EventType EventNotifyFlag
        {
            get { return this._MyEventNotifyFlag; }
            set { this._MyEventNotifyFlag = value; }
        }

        public EventType IsMyEventNotifyFlag
        {
            get { return this._isMyEventNotifyFlag; }
            set { this._isMyEventNotifyFlag = value; }
        }

        public bool ForceEventNotify
        {
            get { return this._MyForceEventNotify; }
            set { this._MyForceEventNotify = value; }
        }

        public bool FavEventUnread
        {
            get { return this._MyFavEventUnread; }
            set { this._MyFavEventUnread = value; }
        }

        public string TranslateLanguage
        {
            get { return this._MyTranslateLanguage; }
            set
            {
                this._MyTranslateLanguage = value;
                ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
            }
        }

        public string EventSoundFile
        {
            get { return this._MyEventSoundFile; }
            set { this._MyEventSoundFile = value; }
        }

        public int ListDoubleClickAction
        {
            get { return this._MyDoubleClickAction; }
            set { this._MyDoubleClickAction = value; }
        }

        public string UserAppointUrl
        {
            get { return this._UserAppointUrl; }
            set { this._UserAppointUrl = value; }
        }

        public bool HotkeyEnabled { get; set; }

        public Keys HotkeyKey { get; set; }

        public int HotkeyValue { get; set; }

        public Keys HotkeyMod { get; set; }

        public bool BlinkNewMentions { get; set; }

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckUseRecommendStatus.Checked == true)
            {
                StatusText.Enabled = false;
            }
            else
            {
                StatusText.Enabled = true;
            }
        }

        private void TreeViewSetting_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.TreeViewSetting.SelectedNode == null)
            {
                return;
            }
            var pnl = (Panel)this.TreeViewSetting.SelectedNode.Tag;
            if (pnl == null)
            {
                return;
            }
            pnl.Enabled = false;
            pnl.Visible = false;
        }

        private void TreeViewSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            var pnl = (Panel)e.Node.Tag;
            if (pnl == null)
            {
                return;
            }
            pnl.Enabled = true;
            pnl.Visible = true;

            if (pnl.Name == "PreviewPanel")
            {
                IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNetworkAvailable()
                && (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Bitly
                || ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Jmp)
                && (!string.IsNullOrEmpty(TextBitlyId.Text) || !string.IsNullOrEmpty(TextBitlyPw.Text)))
            {
                if (!this.BitlyValidation(TextBitlyId.Text, TextBitlyPw.Text))
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.SettingSave_ClickText1);
                    this._ValidationError = true;
                    TreeViewSetting.SelectedNode.Name = "TweetActNode"; // 動作タブを選択
                    TextBitlyId.Focus();
                    return;
                }
                else
                {
                    this._ValidationError = false;
                }
            }
            else
            {
                this._ValidationError = false;
            }

            this.UserAccounts.Clear();
            foreach (var u in this.AuthUserCombo.Items)
            {
                this.UserAccounts.Add((UserAccount)u);
            }
            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.Username.ToLower() == ((UserAccount)this.AuthUserCombo.SelectedItem).Username.ToLower())
                    {
                        this._tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                        if (u.UserId == 0)
                        {
                            this._tw.VerifyCredentials();
                            u.UserId = this._tw.UserId;
                        }
                        break;
                    }
                }
            }
            else
            {
                this._tw.ClearAuthInfo();
                this._tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            }

#if UA // = True
			//フォロー
			if (this.FollowCheckBox.Checked) {
				//現在の設定内容で通信
				HttpConnection.ProxyType ptype = default(HttpConnection.ProxyType);
				if (RadioProxyNone.Checked) {
					ptype = HttpConnection.ProxyType.None;
				} else if (RadioProxyIE.Checked) {
					ptype = HttpConnection.ProxyType.IE;
				} else {
					ptype = HttpConnection.ProxyType.Specified;
				}
				string padr = TextProxyAddress.Text.Trim();
				int pport = int.Parse(TextProxyPort.Text.Trim());
				string pusr = TextProxyUser.Text.Trim();
				string ppw = TextProxyPassword.Text.Trim();
				HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);

				string ret = tw.PostFollowCommand("TweenApp");
			}
#endif
            IntervalChangedEventArgs arg = new IntervalChangedEventArgs();
            bool isIntervalChanged = false;

            try
            {
                this._MyUserstreamStartup = this.StartupUserstreamCheck.Checked;

                if (this._MyUserstreamPeriod != Convert.ToInt32(UserstreamPeriod.Text))
                {
                    this._MyUserstreamPeriod = Convert.ToInt32(UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }
                if (this._MytimelinePeriod != Convert.ToInt32(TimelinePeriod.Text))
                {
                    this._MytimelinePeriod = Convert.ToInt32(TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }
                if (this._MyDMPeriod != Convert.ToInt32(DMPeriod.Text))
                {
                    this._MyDMPeriod = Convert.ToInt32(DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }
                if (this._MyPubSearchPeriod != Convert.ToInt32(PubSearchPeriod.Text))
                {
                    this._MyPubSearchPeriod = Convert.ToInt32(PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (this._MyListsPeriod != Convert.ToInt32(ListsPeriod.Text))
                {
                    this._MyListsPeriod = Convert.ToInt32(ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }
                if (this._MyReplyPeriod != Convert.ToInt32(ReplyPeriod.Text))
                {
                    this._MyReplyPeriod = Convert.ToInt32(ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }
                if (this._MyUserTimelinePeriod != Convert.ToInt32(UserTimelinePeriod.Text))
                {
                    this._MyUserTimelinePeriod = Convert.ToInt32(UserTimelinePeriod.Text);
                    arg.UserTimeline = true;
                    isIntervalChanged = true;
                }

                if (isIntervalChanged)
                {
                    if (this.IntervalChanged != null)
                    {
                        this.IntervalChanged(this, arg);
                    }
                }

                this._MyReaded = StartupReaded.Checked;
                switch (IconSize.SelectedIndex)
                {
                    case 0:
                        this._MyIconSize = Hoehoe.IconSizes.IconNone;
                        break;
                    case 1:
                        this._MyIconSize = Hoehoe.IconSizes.Icon16;
                        break;
                    case 2:
                        this._MyIconSize = Hoehoe.IconSizes.Icon24;
                        break;
                    case 3:
                        this._MyIconSize = Hoehoe.IconSizes.Icon48;
                        break;
                    case 4:
                        this._MyIconSize = Hoehoe.IconSizes.Icon48_2;
                        break;
                }
                this._MyStatusText = StatusText.Text;
                this._MyPlaySound = PlaySnd.Checked;
                this._MyUnreadManage = UReadMng.Checked;
                this._MyOneWayLove = OneWayLv.Checked;
                this._fntUnread = lblUnread.Font;                // 未使用
                this._clUnread = lblUnread.ForeColor;
                this._fntReaded = lblListFont.Font;              // リストフォントとして使用
                this._clReaded = lblListFont.ForeColor;
                this._clFav = lblFav.ForeColor;
                this._clOWL = lblOWL.ForeColor;
                this._clRetweet = lblRetweet.ForeColor;
                this._fntDetail = lblDetail.Font;
                this._clSelf = lblSelf.BackColor;
                this._clAtSelf = lblAtSelf.BackColor;
                this._clTarget = lblTarget.BackColor;
                this._clAtTarget = lblAtTarget.BackColor;
                this._clAtFromTarget = lblAtFromTarget.BackColor;
                this._clAtTo = lblAtTo.BackColor;
                this._clInputBackcolor = lblInputBackcolor.BackColor;
                this._clInputFont = lblInputFont.ForeColor;
                this._clListBackcolor = lblListBackcolor.BackColor;
                this._clDetailBackcolor = lblDetailBackcolor.BackColor;
                this._clDetail = lblDetail.ForeColor;
                this._clDetailLink = lblDetailLink.ForeColor;
                this._fntInputFont = lblInputFont.Font;
                switch (cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        this._myNameBalloon = NameBalloonEnum.None;
                        break;
                    case 1:
                        this._myNameBalloon = NameBalloonEnum.UserID;
                        break;
                    case 2:
                        this._myNameBalloon = NameBalloonEnum.NickName;
                        break;
                }

                switch (ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        this._myPostShiftEnter = true;
                        this._myPostCtrlEnter = false;
                        break;
                    case 1:
                        this._myPostCtrlEnter = true;
                        this._myPostShiftEnter = false;
                        break;
                    case 0:
                        this._myPostCtrlEnter = false;
                        this._myPostShiftEnter = false;
                        break;
                }
                this._countApi = Convert.ToInt32(TextCountApi.Text);
                this._countApiReply = Convert.ToInt32(TextCountApiReply.Text);
                this._browserpath = BrowserPathText.Text.Trim();
                this._MyPostAndGet = CheckPostAndGet.Checked;
                this._myUseRecommendStatus = CheckUseRecommendStatus.Checked;
                this._myDispUsername = CheckDispUsername.Checked;
                this._MyCloseToExit = CheckCloseToExit.Checked;
                this._MyMinimizeToTray = CheckMinimizeToTray.Checked;
                switch (ComboDispTitle.SelectedIndex)
                {
                    case 0:
                        // None
                        this._MyDispLatestPost = DispTitleEnum.None;
                        break;
                    case 1:
                        // Ver
                        this._MyDispLatestPost = DispTitleEnum.Ver;
                        break;
                    case 2:
                        // Post
                        this._MyDispLatestPost = DispTitleEnum.Post;
                        break;
                    case 3:
                        // RepCount
                        this._MyDispLatestPost = DispTitleEnum.UnreadRepCount;
                        break;
                    case 4:
                        // AllCount
                        this._MyDispLatestPost = DispTitleEnum.UnreadAllCount;
                        break;
                    case 5:
                        // Rep+All
                        this._MyDispLatestPost = DispTitleEnum.UnreadAllRepCount;
                        break;
                    case 6:
                        // Unread/All
                        this._MyDispLatestPost = DispTitleEnum.UnreadCountAllCount;
                        break;
                    case 7:
                        // Count of Status/Follow/Follower
                        this._MyDispLatestPost = DispTitleEnum.OwnStatus;
                        break;
                }
                this._MySortOrderLock = CheckSortOrderLock.Checked;
                this._MyTinyUrlResolve = CheckTinyURL.Checked;
                this._MyShortUrlForceResolve = CheckForceResolve.Checked;
                ShortUrl.IsResolve = this._MyTinyUrlResolve;
                ShortUrl.IsForceResolve = this._MyShortUrlForceResolve;
                if (RadioProxyNone.Checked)
                {
                    this._MyProxyType = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    this._MyProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    this._MyProxyType = HttpConnection.ProxyType.Specified;
                }
                this._MyProxyAddress = TextProxyAddress.Text.Trim();
                this._MyProxyPort = int.Parse(TextProxyPort.Text.Trim());
                this._MyProxyUser = TextProxyUser.Text.Trim();
                this._MyProxyPassword = TextProxyPassword.Text.Trim();
                this._MyPeriodAdjust = CheckPeriodAdjust.Checked;
                this._MyStartupVersion = CheckStartupVersion.Checked;
                this._MyStartupFollowers = CheckStartupFollowers.Checked;
                this._MyRestrictFavCheck = CheckFavRestrict.Checked;
                this._MyAlwaysTop = CheckAlwaysTop.Checked;
                this._MyUrlConvertAuto = CheckAutoConvertUrl.Checked;
                this._MyShortenTco = ShortenTcoCheck.Checked;
                this._MyOutputz = CheckOutputz.Checked;
                this._MyOutputzKey = TextBoxOutputzKey.Text.Trim();

                switch (ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        this._MyOutputzUrlmode = OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        this._MyOutputzUrlmode = OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                this._MyNicoms = CheckNicoms.Checked;
                this._MyUnreadStyle = chkUnreadStyle.Checked;
                this._MyDateTimeFormat = CmbDateTimeFormat.Text;
                this._MyDefaultTimeOut = Convert.ToInt32(ConnectionTimeOut.Text);
                this._MyRetweetNoConfirm = CheckRetweetNoConfirm.Checked;
                this._MyLimitBalloon = CheckBalloonLimit.Checked;
                this._MyEventNotifyEnabled = CheckEventNotify.Checked;
                this.GetEventNotifyFlag(ref this._MyEventNotifyFlag, ref this._isMyEventNotifyFlag);
                this._MyForceEventNotify = CheckForceEventNotify.Checked;
                this._MyFavEventUnread = CheckFavEventUnread.Checked;
                this._MyTranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
                this._MyEventSoundFile = Convert.ToString(ComboBoxEventNotifySound.SelectedItem);
                this._MyAutoShortUrlFirst = (UrlConverter)ComboBoxAutoShortUrlFirst.SelectedIndex;
                this._MyTabIconDisp = chkTabIconDisp.Checked;
                this._MyReadOwnPost = chkReadOwnPost.Checked;
                this._MyGetFav = chkGetFav.Checked;
                this._MyMonoSpace = CheckMonospace.Checked;
                this._MyReadOldPosts = CheckReadOldPosts.Checked;
                this._MyUseSsl = CheckUseSsl.Checked;
                this._MyBitlyId = TextBitlyId.Text;
                this._MyBitlyPw = TextBitlyPw.Text;
                this._MyShowGrid = CheckShowGrid.Checked;
                this._MyUseAtIdSupplement = CheckAtIdSupple.Checked;
                this._MyUseHashSupplement = CheckHashSupple.Checked;
                this._MyPreviewEnable = CheckPreviewEnable.Checked;
                this._MyTwitterApiUrl = TwitterAPIText.Text.Trim();
                this._MyTwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim();
                switch (ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        this._MyReplyIconState = ReplyIconState.None;
                        break;
                    case 1:
                        this._MyReplyIconState = ReplyIconState.StaticIcon;
                        break;
                    case 2:
                        this._MyReplyIconState = ReplyIconState.BlinkIcon;
                        break;
                }
                switch (LanguageCombo.SelectedIndex)
                {
                    case 0:
                        this._MyLanguage = "OS";
                        break;
                    case 1:
                        this._MyLanguage = "ja";
                        break;
                    case 2:
                        this._MyLanguage = "en";
                        break;
                    case 3:
                        this._MyLanguage = "zh-CN";
                        break;
                    default:
                        this._MyLanguage = "en";
                        break;
                }
                this.HotkeyEnabled = this.HotkeyCheck.Checked;
                this.HotkeyMod = Keys.None;
                if (this.HotkeyAlt.Checked)
                {
                    this.HotkeyMod = this.HotkeyMod | Keys.Alt;
                }
                if (this.HotkeyShift.Checked)
                {
                    this.HotkeyMod = this.HotkeyMod | Keys.Shift;
                }
                if (this.HotkeyCtrl.Checked)
                {
                    this.HotkeyMod = this.HotkeyMod | Keys.Control;
                }
                if (this.HotkeyWin.Checked)
                {
                    this.HotkeyMod = this.HotkeyMod | Keys.LWin;
                }
                {
                    int tmp = this.HotkeyValue;
                    if (int.TryParse(HotkeyCode.Text, out tmp)) { this.HotkeyValue = tmp; }
                }
                this.HotkeyKey = (Keys)HotkeyText.Tag;
                this.BlinkNewMentions = ChkNewMentionsBlink.Checked;
                this._MyUseAdditonalCount = UseChangeGetCount.Checked;
                this._MoreCountApi = Convert.ToInt32(GetMoreTextCountApi.Text);
                this._FirstCountApi = Convert.ToInt32(FirstTextCountApi.Text);
                this._SearchCountApi = Convert.ToInt32(SearchTextCountApi.Text);
                this._FavoritesCountApi = Convert.ToInt32(FavoritesTextCountApi.Text);
                this._UserTimelineCountApi = Convert.ToInt32(UserTimelineTextCountApi.Text);
                this._ListCountApi = Convert.ToInt32(ListTextCountApi.Text);
                this._MyOpenUserTimeline = CheckOpenUserTimeline.Checked;
                this._MyDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex;
                this._UserAppointUrl = UserAppointUrlText.Text;
                this.HideDuplicatedRetweets = this.HideDuplicatedRetweetsCheck.Checked;
                this.IsPreviewFoursquare = this.IsPreviewFoursquareCheckBox.Checked;
                this.FoursquarePreviewHeight = Convert.ToInt32(this.FoursquarePreviewHeightTextBox.Text);
                this.FoursquarePreviewWidth = Convert.ToInt32(this.FoursquarePreviewWidthTextBox.Text);
                this.FoursquarePreviewZoom = Convert.ToInt32(this.FoursquarePreviewZoomTextBox.Text);
                this.IsListStatusesIncludeRts = this.IsListsIncludeRtsCheckBox.Checked;
                this.TabMouseLock = this.TabMouseLockCheck.Checked;
                this.IsRemoveSameEvent = this.IsRemoveSameFavEventCheckBox.Checked;
                this.IsNotifyUseGrowl = this.IsNotifyUseGrowlCheckBox.Checked;
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.Save_ClickText3);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            if (this.DialogResult == DialogResult.Cancel)
            {
                // キャンセル時は画面表示時のアカウントに戻す
                // キャンセル時でも認証済みアカウント情報は保存する
                this.UserAccounts.Clear();
                foreach (var u in this.AuthUserCombo.Items)
                {
                    this.UserAccounts.Add((UserAccount)u);
                }

                // アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (this._InitialUserId > 0)
                {
                    foreach (UserAccount u in this.UserAccounts)
                    {
                        if (u.UserId == this._InitialUserId)
                        {
                            this._tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                            userSet = true;
                            break;
                        }
                    }
                }

                // 認証済みアカウントが削除されていた場合、もしくは起動時アカウントがなかった場合は、アクティブユーザーなしとして初期化
                if (!userSet)
                {
                    this._tw.ClearAuthInfo();
                    this._tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
                }
            }

            if (this._tw != null && string.IsNullOrEmpty(this._tw.Username) && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(Hoehoe.Properties.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            if (this._ValidationError)
            {
                e.Cancel = true;
            }
            if (!e.Cancel && TreeViewSetting.SelectedNode != null)
            {
                Panel curPanel = (Panel)TreeViewSetting.SelectedNode.Tag;
                curPanel.Visible = false;
                curPanel.Enabled = false;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
#if UA //= "True"
			this.GroupBox2.Visible = true;
#else
            this.GroupBox2.Visible = false;
#endif
            this._tw = ((TweenMain)this.Owner).TwitterInstance;
            string uname = this._tw.Username;
            string pw = this._tw.Password;
            string tk = this._tw.AccessToken;
            string tks = this._tw.AccessTokenSecret;

            this.AuthClearButton.Enabled = true;

            this.AuthUserCombo.Items.Clear();
            if (this.UserAccounts.Count > 0)
            {
                this.AuthUserCombo.Items.AddRange(this.UserAccounts.ToArray());
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.UserId == this._tw.UserId)
                    {
                        this.AuthUserCombo.SelectedItem = u;
                        this._InitialUserId = u.UserId;
                        break;
                    }
                }
            }

            this.StartupUserstreamCheck.Checked = this._MyUserstreamStartup;
            UserstreamPeriod.Text = this._MyUserstreamPeriod.ToString();
            TimelinePeriod.Text = this._MytimelinePeriod.ToString();
            ReplyPeriod.Text = this._MyReplyPeriod.ToString();
            DMPeriod.Text = this._MyDMPeriod.ToString();
            PubSearchPeriod.Text = this._MyPubSearchPeriod.ToString();
            ListsPeriod.Text = this._MyListsPeriod.ToString();
            UserTimelinePeriod.Text = this._MyUserTimelinePeriod.ToString();

            StartupReaded.Checked = this._MyReaded;
            switch (this._MyIconSize)
            {
                case Hoehoe.IconSizes.IconNone:
                    IconSize.SelectedIndex = 0;
                    break;
                case Hoehoe.IconSizes.Icon16:
                    IconSize.SelectedIndex = 1;
                    break;
                case Hoehoe.IconSizes.Icon24:
                    IconSize.SelectedIndex = 2;
                    break;
                case Hoehoe.IconSizes.Icon48:
                    IconSize.SelectedIndex = 3;
                    break;
                case Hoehoe.IconSizes.Icon48_2:
                    IconSize.SelectedIndex = 4;
                    break;
            }
            StatusText.Text = this._MyStatusText;
            UReadMng.Checked = this._MyUnreadManage;
            StartupReaded.Enabled = this._MyUnreadManage != false;
            PlaySnd.Checked = this._MyPlaySound;
            OneWayLv.Checked = this._MyOneWayLove;

            lblListFont.Font = this._fntReaded;
            lblUnread.Font = this._fntUnread;
            lblUnread.ForeColor = this._clUnread;
            lblListFont.ForeColor = this._clReaded;
            lblFav.ForeColor = this._clFav;
            lblOWL.ForeColor = this._clOWL;
            lblRetweet.ForeColor = this._clRetweet;
            lblDetail.Font = this._fntDetail;
            lblSelf.BackColor = this._clSelf;
            lblAtSelf.BackColor = this._clAtSelf;
            lblTarget.BackColor = this._clTarget;
            lblAtTarget.BackColor = this._clAtTarget;
            lblAtFromTarget.BackColor = this._clAtFromTarget;
            lblAtTo.BackColor = this._clAtTo;
            lblInputBackcolor.BackColor = this._clInputBackcolor;
            lblInputFont.ForeColor = this._clInputFont;
            lblInputFont.Font = this._fntInputFont;
            lblListBackcolor.BackColor = this._clListBackcolor;
            lblDetailBackcolor.BackColor = this._clDetailBackcolor;
            lblDetail.ForeColor = this._clDetail;
            lblDetailLink.ForeColor = this._clDetailLink;

            switch (this._myNameBalloon)
            {
                case NameBalloonEnum.None:
                    cmbNameBalloon.SelectedIndex = 0;
                    break;
                case NameBalloonEnum.UserID:
                    cmbNameBalloon.SelectedIndex = 1;
                    break;
                case NameBalloonEnum.NickName:
                    cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            if (this._myPostCtrlEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (this._myPostShiftEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            TextCountApi.Text = this._countApi.ToString();
            TextCountApiReply.Text = this._countApiReply.ToString();
            BrowserPathText.Text = this._browserpath;
            CheckPostAndGet.Checked = this._MyPostAndGet;
            CheckUseRecommendStatus.Checked = this._myUseRecommendStatus;
            CheckDispUsername.Checked = this._myDispUsername;
            CheckCloseToExit.Checked = this._MyCloseToExit;
            CheckMinimizeToTray.Checked = this._MyMinimizeToTray;
            switch (this._MyDispLatestPost)
            {
                case DispTitleEnum.None:
                    ComboDispTitle.SelectedIndex = 0;
                    break;
                case DispTitleEnum.Ver:
                    ComboDispTitle.SelectedIndex = 1;
                    break;
                case DispTitleEnum.Post:
                    ComboDispTitle.SelectedIndex = 2;
                    break;
                case DispTitleEnum.UnreadRepCount:
                    ComboDispTitle.SelectedIndex = 3;
                    break;
                case DispTitleEnum.UnreadAllCount:
                    ComboDispTitle.SelectedIndex = 4;
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    ComboDispTitle.SelectedIndex = 5;
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    ComboDispTitle.SelectedIndex = 6;
                    break;
                case DispTitleEnum.OwnStatus:
                    ComboDispTitle.SelectedIndex = 7;
                    break;
            }
            CheckSortOrderLock.Checked = this._MySortOrderLock;
            CheckTinyURL.Checked = this._MyTinyUrlResolve;
            CheckForceResolve.Checked = this._MyShortUrlForceResolve;
            switch (this._MyProxyType)
            {
                case HttpConnection.ProxyType.None:
                    RadioProxyNone.Checked = true;
                    break;
                case HttpConnection.ProxyType.IE:
                    RadioProxyIE.Checked = true;
                    break;
                default:
                    RadioProxySpecified.Checked = true;
                    break;
            }
            bool chk = RadioProxySpecified.Checked;
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;

            TextProxyAddress.Text = this._MyProxyAddress;
            TextProxyPort.Text = this._MyProxyPort.ToString();
            TextProxyUser.Text = this._MyProxyUser;
            TextProxyPassword.Text = this._MyProxyPassword;

            CheckPeriodAdjust.Checked = this._MyPeriodAdjust;
            CheckStartupVersion.Checked = this._MyStartupVersion;
            CheckStartupFollowers.Checked = this._MyStartupFollowers;
            CheckFavRestrict.Checked = this._MyRestrictFavCheck;
            CheckAlwaysTop.Checked = this._MyAlwaysTop;
            CheckAutoConvertUrl.Checked = this._MyUrlConvertAuto;
            ShortenTcoCheck.Checked = this._MyShortenTco;
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
            CheckOutputz.Checked = this._MyOutputz;
            TextBoxOutputzKey.Text = this._MyOutputzKey;

            switch (this._MyOutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            CheckNicoms.Checked = this._MyNicoms;
            chkUnreadStyle.Checked = this._MyUnreadStyle;
            CmbDateTimeFormat.Text = this._MyDateTimeFormat;
            ConnectionTimeOut.Text = this._MyDefaultTimeOut.ToString();
            CheckRetweetNoConfirm.Checked = this._MyRetweetNoConfirm;
            CheckBalloonLimit.Checked = this._MyLimitBalloon;

            this.ApplyEventNotifyFlag(this._MyEventNotifyEnabled, this._MyEventNotifyFlag, this._isMyEventNotifyFlag);
            CheckForceEventNotify.Checked = this._MyForceEventNotify;
            CheckFavEventUnread.Checked = this._MyFavEventUnread;
            ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(this._MyTranslateLanguage);
            this.SoundFileListup();
            ComboBoxAutoShortUrlFirst.SelectedIndex = (int)this._MyAutoShortUrlFirst;
            chkTabIconDisp.Checked = this._MyTabIconDisp;
            chkReadOwnPost.Checked = this._MyReadOwnPost;
            chkGetFav.Checked = this._MyGetFav;
            CheckMonospace.Checked = this._MyMonoSpace;
            CheckReadOldPosts.Checked = this._MyReadOldPosts;
            CheckUseSsl.Checked = this._MyUseSsl;
            TextBitlyId.Text = this._MyBitlyId;
            TextBitlyPw.Text = this._MyBitlyPw;
            TextBitlyId.Modified = false;
            TextBitlyPw.Modified = false;
            CheckShowGrid.Checked = this._MyShowGrid;
            CheckAtIdSupple.Checked = this._MyUseAtIdSupplement;
            CheckHashSupple.Checked = this._MyUseHashSupplement;
            CheckPreviewEnable.Checked = this._MyPreviewEnable;
            TwitterAPIText.Text = this._MyTwitterApiUrl;
            TwitterSearchAPIText.Text = this._MyTwitterSearchApiUrl;
            switch (this._MyReplyIconState)
            {
                case ReplyIconState.None:
                    ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case ReplyIconState.StaticIcon:
                    ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case ReplyIconState.BlinkIcon:
                    ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }
            switch (this._MyLanguage)
            {
                case "OS":
                    LanguageCombo.SelectedIndex = 0;
                    break;
                case "ja":
                    LanguageCombo.SelectedIndex = 1;
                    break;
                case "en":
                    LanguageCombo.SelectedIndex = 2;
                    break;
                case "zh-CN":
                    LanguageCombo.SelectedIndex = 3;
                    break;
                default:
                    LanguageCombo.SelectedIndex = 0;
                    break;
            }
            HotkeyCheck.Checked = this.HotkeyEnabled;
            HotkeyAlt.Checked = (this.HotkeyMod & Keys.Alt) == Keys.Alt;
            HotkeyCtrl.Checked = (this.HotkeyMod & Keys.Control) == Keys.Control;
            HotkeyShift.Checked = (this.HotkeyMod & Keys.Shift) == Keys.Shift;
            HotkeyWin.Checked = (this.HotkeyMod & Keys.LWin) == Keys.LWin;
            HotkeyCode.Text = this.HotkeyValue.ToString();
            HotkeyText.Text = this.HotkeyKey.ToString();
            HotkeyText.Tag = this.HotkeyKey;
            HotkeyAlt.Enabled = this.HotkeyEnabled;
            HotkeyShift.Enabled = this.HotkeyEnabled;
            HotkeyCtrl.Enabled = this.HotkeyEnabled;
            HotkeyWin.Enabled = this.HotkeyEnabled;
            HotkeyText.Enabled = this.HotkeyEnabled;
            HotkeyCode.Enabled = this.HotkeyEnabled;
            ChkNewMentionsBlink.Checked = this.BlinkNewMentions;

            this.CheckOutputz_CheckedChanged(sender, e);

            GetMoreTextCountApi.Text = this._MoreCountApi.ToString();
            FirstTextCountApi.Text = this._FirstCountApi.ToString();
            SearchTextCountApi.Text = this._SearchCountApi.ToString();
            FavoritesTextCountApi.Text = this._FavoritesCountApi.ToString();
            UserTimelineTextCountApi.Text = this._UserTimelineCountApi.ToString();
            ListTextCountApi.Text = this._ListCountApi.ToString();
            UseChangeGetCount.Checked = this._MyUseAdditonalCount;
            Label28.Enabled = UseChangeGetCount.Checked;
            Label30.Enabled = UseChangeGetCount.Checked;
            Label53.Enabled = UseChangeGetCount.Checked;
            Label66.Enabled = UseChangeGetCount.Checked;
            Label17.Enabled = UseChangeGetCount.Checked;
            Label25.Enabled = UseChangeGetCount.Checked;
            GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked;
            FirstTextCountApi.Enabled = UseChangeGetCount.Checked;
            SearchTextCountApi.Enabled = UseChangeGetCount.Checked;
            FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked;
            UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked;
            ListTextCountApi.Enabled = UseChangeGetCount.Checked;
            CheckOpenUserTimeline.Checked = this._MyOpenUserTimeline;
            ListDoubleClickActionComboBox.SelectedIndex = this._MyDoubleClickAction;
            UserAppointUrlText.Text = this._UserAppointUrl;
            this.HideDuplicatedRetweetsCheck.Checked = this.HideDuplicatedRetweets;
            this.IsPreviewFoursquareCheckBox.Checked = this.IsPreviewFoursquare;
            this.FoursquarePreviewHeightTextBox.Text = this.FoursquarePreviewHeight.ToString();
            this.FoursquarePreviewWidthTextBox.Text = this.FoursquarePreviewWidth.ToString();
            this.FoursquarePreviewZoomTextBox.Text = this.FoursquarePreviewZoom.ToString();
            this.IsListsIncludeRtsCheckBox.Checked = this.IsListStatusesIncludeRts;
            this.TabMouseLockCheck.Checked = this.TabMouseLock;
            this.IsRemoveSameFavEventCheckBox.Checked = this.IsRemoveSameEvent;
            this.IsNotifyUseGrowlCheckBox.Checked = this.IsNotifyUseGrowl;

            IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;

            this.TreeViewSetting.Nodes["BasedNode"].Tag = BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = GetCountPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Tag = ActionPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Nodes["TweetActNode"].Tag = TweetActPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Tag = PreviewPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["TweetPrvNode"].Tag = TweetPrvPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["NotifyNode"].Tag = NotifyPanel;
            this.TreeViewSetting.Nodes["FontNode"].Tag = FontPanel;
            this.TreeViewSetting.Nodes["FontNode"].Nodes["FontNode2"].Tag = FontPanel2;
            this.TreeViewSetting.Nodes["ConnectionNode"].Tag = ConnectionPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ProxyNode"].Tag = ProxyPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["CooperateNode"].Tag = CooperatePanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"].Tag = ShortUrlPanel;

            this.TreeViewSetting.SelectedNode = this.TreeViewSetting.Nodes[0];
            this.TreeViewSetting.ExpandAll();
            ActiveControl = StartAuthButton;
        }

        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(UserstreamPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd < 0 || prd > 60)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(TimelinePeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(ReplyPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(DMPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(PubSearchPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.PubSearchPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 30 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.PubSearchPeriod_ValidatingText2);
                e.Cancel = true;
            }
        }

        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(ListsPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(UserTimelinePeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            this.CalcApiUsing();
        }

        private void UReadMng_CheckedChanged(object sender, EventArgs e)
        {
            StartupReaded.Enabled = UReadMng.Checked == true;
        }

        private void btnFontAndColor_Click(object sender, EventArgs e)
        {
            Button Btn = (Button)sender;
            DialogResult rtn = default(DialogResult);

            FontDialog1.AllowVerticalFonts = false;
            FontDialog1.AllowScriptChange = true;
            FontDialog1.AllowSimulations = true;
            FontDialog1.AllowVectorFonts = true;
            FontDialog1.FixedPitchOnly = false;
            FontDialog1.FontMustExist = true;
            FontDialog1.ScriptsOnly = false;
            FontDialog1.ShowApply = false;
            FontDialog1.ShowEffects = true;
            FontDialog1.ShowColor = true;

            switch (Btn.Name)
            {
                case "btnUnread":
                    FontDialog1.Color = lblUnread.ForeColor;
                    FontDialog1.Font = lblUnread.Font;
                    break;
                case "btnDetail":
                    FontDialog1.Color = lblDetail.ForeColor;
                    FontDialog1.Font = lblDetail.Font;
                    break;
                case "btnListFont":
                    FontDialog1.Color = lblListFont.ForeColor;
                    FontDialog1.Font = lblListFont.Font;
                    break;
                case "btnInputFont":
                    FontDialog1.Color = lblInputFont.ForeColor;
                    FontDialog1.Font = lblInputFont.Font;
                    break;
            }

            try
            {
                rtn = FontDialog1.ShowDialog();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (rtn == DialogResult.Cancel)
            {
                return;
            }

            switch (Btn.Name)
            {
                case "btnUnread":
                    lblUnread.ForeColor = FontDialog1.Color;
                    lblUnread.Font = FontDialog1.Font;
                    break;
                case "btnDetail":
                    lblDetail.ForeColor = FontDialog1.Color;
                    lblDetail.Font = FontDialog1.Font;
                    break;
                case "btnListFont":
                    lblListFont.ForeColor = FontDialog1.Color;
                    lblListFont.Font = FontDialog1.Font;
                    break;
                case "btnInputFont":
                    lblInputFont.ForeColor = FontDialog1.Color;
                    lblInputFont.Font = FontDialog1.Font;
                    break;
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Button Btn = (Button)sender;
            DialogResult rtn = default(DialogResult);

            ColorDialog1.AllowFullOpen = true;
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = false;
            ColorDialog1.SolidColorOnly = false;

            switch (Btn.Name)
            {
                case "btnSelf":
                    ColorDialog1.Color = lblSelf.BackColor;
                    break;
                case "btnAtSelf":
                    ColorDialog1.Color = lblAtSelf.BackColor;
                    break;
                case "btnTarget":
                    ColorDialog1.Color = lblTarget.BackColor;
                    break;
                case "btnAtTarget":
                    ColorDialog1.Color = lblAtTarget.BackColor;
                    break;
                case "btnAtFromTarget":
                    ColorDialog1.Color = lblAtFromTarget.BackColor;
                    break;
                case "btnFav":
                    ColorDialog1.Color = lblFav.ForeColor;
                    break;
                case "btnOWL":
                    ColorDialog1.Color = lblOWL.ForeColor;
                    break;
                case "btnRetweet":
                    ColorDialog1.Color = lblRetweet.ForeColor;
                    break;
                case "btnInputBackcolor":
                    ColorDialog1.Color = lblInputBackcolor.BackColor;
                    break;
                case "btnAtTo":
                    ColorDialog1.Color = lblAtTo.BackColor;
                    break;
                case "btnListBack":
                    ColorDialog1.Color = lblListBackcolor.BackColor;
                    break;
                case "btnDetailBack":
                    ColorDialog1.Color = lblDetailBackcolor.BackColor;
                    break;
                case "btnDetailLink":
                    ColorDialog1.Color = lblDetailLink.ForeColor;
                    break;
            }

            rtn = ColorDialog1.ShowDialog();

            if (rtn == DialogResult.Cancel)
                return;

            switch (Btn.Name)
            {
                case "btnSelf":
                    lblSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtSelf":
                    lblAtSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnTarget":
                    lblTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTarget":
                    lblAtTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtFromTarget":
                    lblAtFromTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnFav":
                    lblFav.ForeColor = ColorDialog1.Color;
                    break;
                case "btnOWL":
                    lblOWL.ForeColor = ColorDialog1.Color;
                    break;
                case "btnRetweet":
                    lblRetweet.ForeColor = ColorDialog1.Color;
                    break;
                case "btnInputBackcolor":
                    lblInputBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTo":
                    lblAtTo.BackColor = ColorDialog1.Color;
                    break;
                case "btnListBack":
                    lblListBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailBack":
                    lblDetailBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailLink":
                    lblDetailLink.ForeColor = ColorDialog1.Color;
                    break;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog filedlg = new OpenFileDialog())
            {
                filedlg.Filter = Hoehoe.Properties.Resources.Button3_ClickText1;
                filedlg.FilterIndex = 1;
                filedlg.Title = Hoehoe.Properties.Resources.Button3_ClickText2;
                filedlg.RestoreDirectory = true;

                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    BrowserPathText.Text = filedlg.FileName;
                }
            }
        }

        private void RadioProxySpecified_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = RadioProxySpecified.Checked;
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;
        }

        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            int port = 0;
            if (string.IsNullOrEmpty(TextProxyPort.Text.Trim()))
            {
                TextProxyPort.Text = "0";
            }
            if (!int.TryParse(TextProxyPort.Text.Trim(), out port))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextProxyPort_ValidatingText1);
                e.Cancel = true;
                return;
            }
            if (port < 0 | port > 65535)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextProxyPort_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void CheckOutputz_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckOutputz.Checked == true)
            {
                Label59.Enabled = true;
                Label60.Enabled = true;
                TextBoxOutputzKey.Enabled = true;
                ComboBoxOutputzUrlmode.Enabled = true;
            }
            else
            {
                Label59.Enabled = false;
                Label60.Enabled = false;
                TextBoxOutputzKey.Enabled = false;
                ComboBoxOutputzUrlmode.Enabled = false;
            }
        }

        private void TextBoxOutputzKey_Validating(object sender, CancelEventArgs e)
        {
            if (CheckOutputz.Checked)
            {
                TextBoxOutputzKey.Text = TextBoxOutputzKey.Text.Trim();
                if (TextBoxOutputzKey.Text.Length == 0)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.TextBoxOutputzKey_Validating);
                    e.Cancel = true;
                    return;
                }
            }
        }

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(CmbDateTimeFormat.Text);
            }
            catch (FormatException)
            {
                LabelDateTimeFormatApplied.Text = Hoehoe.Properties.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }
            return true;
        }

        private void CmbDateTimeFormat_TextUpdate(object sender, EventArgs e)
        {
            this.CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_Validating(object sender, CancelEventArgs e)
        {
            if (!this.CreateDateTimeFormatSample())
            {
                MessageBox.Show(Hoehoe.Properties.Resources.CmbDateTimeFormat_Validating);
                e.Cancel = true;
            }
        }

        private void ConnectionTimeOut_Validating(object sender, CancelEventArgs e)
        {
            int tm = 0;
            try
            {
                tm = Convert.ToInt32(ConnectionTimeOut.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (tm < (int)HttpTimeOut.MinValue || tm > (int)HttpTimeOut.MaxValue)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            this.CreateDateTimeFormatSample();
        }

        private void TextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(TextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void TextCountApiReply_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(TextCountApiReply.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Bitly || ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Jmp)
            {
                Label76.Enabled = true;
                Label77.Enabled = true;
                TextBitlyId.Enabled = true;
                TextBitlyPw.Enabled = true;
            }
            else
            {
                Label76.Enabled = false;
                Label77.Enabled = false;
                TextBitlyId.Enabled = false;
                TextBitlyPw.Enabled = false;
            }
        }

        private void ButtonBackToDefaultFontColor_Click(object sender, EventArgs e)
        {
            lblUnread.ForeColor = SystemColors.ControlText;
            lblUnread.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            lblListFont.ForeColor = SystemColors.ControlText;
            lblListFont.Font = SystemFonts.DefaultFont;

            lblDetail.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            lblDetail.Font = SystemFonts.DefaultFont;

            lblInputFont.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            lblInputFont.Font = SystemFonts.DefaultFont;

            lblSelf.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);

            lblAtSelf.BackColor = Color.FromKnownColor(KnownColor.AntiqueWhite);

            lblTarget.BackColor = Color.FromKnownColor(KnownColor.LemonChiffon);

            lblAtTarget.BackColor = Color.FromKnownColor(KnownColor.LavenderBlush);

            lblAtFromTarget.BackColor = Color.FromKnownColor(KnownColor.Honeydew);

            lblFav.ForeColor = Color.FromKnownColor(KnownColor.Red);

            lblOWL.ForeColor = Color.FromKnownColor(KnownColor.Blue);

            lblInputBackcolor.BackColor = Color.FromKnownColor(KnownColor.LemonChiffon);

            lblAtTo.BackColor = Color.FromKnownColor(KnownColor.Pink);

            lblListBackcolor.BackColor = Color.FromKnownColor(KnownColor.Window);

            lblDetailBackcolor.BackColor = Color.FromKnownColor(KnownColor.Window);

            lblDetailLink.ForeColor = Color.FromKnownColor(KnownColor.Blue);

            lblRetweet.ForeColor = Color.FromKnownColor(KnownColor.Green);
        }

        private bool StartAuth()
        {
            // 現在の設定内容で通信
            HttpConnection.ProxyType ptype = default(HttpConnection.ProxyType);
            if (RadioProxyNone.Checked)
            {
                ptype = HttpConnection.ProxyType.None;
            }
            else if (RadioProxyIE.Checked)
            {
                ptype = HttpConnection.ProxyType.IE;
            }
            else
            {
                ptype = HttpConnection.ProxyType.Specified;
            }
            string padr = TextProxyAddress.Text.Trim();
            int pport = int.Parse(TextProxyPort.Text.Trim());
            string pusr = TextProxyUser.Text.Trim();
            string ppw = TextProxyPassword.Text.Trim();

            // 通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
            HttpTwitter.SetTwitterUrl(TwitterAPIText.Text.Trim());
            HttpTwitter.SetTwitterSearchUrl(TwitterSearchAPIText.Text.Trim());
            this._tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            string pinPageUrl = string.Empty;
            string rslt = this._tw.StartAuthentication(ref pinPageUrl);
            if (string.IsNullOrEmpty(rslt))
            {
                using (var ab = new AuthBrowser())
                {
                    ab.IsAuthorized = true;
                    ab.UrlString = pinPageUrl;
                    if (ab.ShowDialog(this) == DialogResult.OK)
                    {
                        this._pin = ab.PinString;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }
        }

        private bool PinAuth()
        {
            string pin = this._pin;
            string rslt = this._tw.Authenticate(pin);
            if (string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK);
                int idx = -1;
                var user = new UserAccount
                {
                    Username = this._tw.Username,
                    UserId = this._tw.UserId,
                    Token = this._tw.AccessToken,
                    TokenSecret = this._tw.AccessTokenSecret
                };
                foreach (var u in this.AuthUserCombo.Items)
                {
                    if (((UserAccount)u).Username.ToLower() == this._tw.Username.ToLower())
                    {
                        idx = this.AuthUserCombo.Items.IndexOf(u);
                        break;
                    }
                }
                if (idx > -1)
                {
                    this.AuthUserCombo.Items.RemoveAt(idx);
                    this.AuthUserCombo.Items.Insert(idx, user);
                    this.AuthUserCombo.SelectedIndex = idx;
                }
                else
                {
                    this.AuthUserCombo.SelectedIndex = this.AuthUserCombo.Items.Add(user);
                }
                return true;
            }
            else
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }
        }

        private void StartAuthButton_Click(object sender, EventArgs e)
        {
            if (this.StartAuth())
            {
                if (this.PinAuth())
                {
                    this.CalcApiUsing();
                }
            }
        }

        private void AuthClearButton_Click(object sender, EventArgs e)
        {
            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                this.AuthUserCombo.Items.RemoveAt(this.AuthUserCombo.SelectedIndex);
                if (this.AuthUserCombo.Items.Count > 0)
                {
                    this.AuthUserCombo.SelectedIndex = 0;
                }
                else
                {
                    this.AuthUserCombo.SelectedIndex = -1;
                }
            }
            this.CalcApiUsing();
        }

        private void DisplayApiMaxCount()
        {
            if (MyCommon.TwitterApiInfo.MaxCount > -1)
            {
                LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, MyCommon.TwitterApiInfo.MaxCount);
            }
            else
            {
                LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, "???");
            }
        }

        private void CalcApiUsing()
        {
            int listsTabNum = 0;
            try
            {
                // 初回起動時などにNothingの場合あり
                listsTabNum = TabInformations.GetInstance().GetTabsByType(TabUsageType.Lists).Count;
            }
            catch (Exception)
            {
                return;
            }

            int userTimelineTabNum = 0;
            try
            {
                // 初回起動時などにNothingの場合あり
                userTimelineTabNum = TabInformations.GetInstance().GetTabsByType(TabUsageType.UserTimeline).Count;
            }
            catch (Exception)
            {
                return;
            }

            // Recent計算 0は手動更新
            int tmp = 0;
            int usingApi = 0;
            if (int.TryParse(TimelinePeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += 3600 / tmp;
                }
            }

            // Reply計算 0は手動更新
            if (int.TryParse(ReplyPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += 3600 / tmp;
                }
            }

            // DM計算 0は手動更新 送受信両方
            if (int.TryParse(DMPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += (3600 / tmp) * 2;
                }
            }

            // Listsタブ計算 0は手動更新
            int apiLists = 0;
            if (int.TryParse(ListsPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    apiLists = (3600 / tmp) * listsTabNum;
                    usingApi += apiLists;
                }
            }

            // UserTimelineタブ計算 0は手動更新
            int apiUserTimeline = 0;
            if (int.TryParse(UserTimelinePeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    apiUserTimeline = (3600 / tmp) * userTimelineTabNum;
                    usingApi += apiUserTimeline;
                }
            }

            if (this._tw != null)
            {
                if (MyCommon.TwitterApiInfo.MaxCount == -1)
                {
                    if (Twitter.AccountState == AccountState.Valid)
                    {
                        MyCommon.TwitterApiInfo.UsingCount = usingApi;
                        var proc = new Thread(new System.Threading.ThreadStart(() =>
                        {
                            this._tw.GetInfoApi(null); // 取得エラー時はinfoCountは初期状態（値：-1）
                            if (this.IsHandleCreated && this.IsDisposed)
                            {
                                Invoke(new MethodInvoker(DisplayApiMaxCount));
                            }
                        }));
                        proc.Start();
                    }
                    else
                    {
                        LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, usingApi, "???");
                    }
                }
                else
                {
                    LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, usingApi, MyCommon.TwitterApiInfo.MaxCount);
                }
            }

            LabelPostAndGet.Visible = CheckPostAndGet.Checked && !this._tw.UserStreamEnabled;
            LabelUserStreamActive.Visible = this._tw.UserStreamEnabled;

            LabelApiUsingUserStreamEnabled.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse2, apiLists + apiUserTimeline);
            LabelApiUsingUserStreamEnabled.Visible = this._tw.UserStreamEnabled;
        }

        private void CheckPostAndGet_CheckedChanged(object sender, EventArgs e)
        {
            this.CalcApiUsing();
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            do
            {
                Thread.Sleep(10);
                if (this.Disposing || this.IsDisposed)
                {
                    return;
                }
            }
            while (!this.IsHandleCreated);
            this.TopMost = this.AlwaysTop;
            this.CalcApiUsing();
        }

        private void ButtonApiCalc_Click(object sender, EventArgs e)
        {
            this.CalcApiUsing();
        }

        public static AppendSettingDialog Instance
        {
            get { return _instance; }
        }

        private bool BitlyValidation(string id, string apikey)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(apikey))
            {
                return false;
            }

            /// TODO: BitlyApi
            string req = "http://api.bit.ly/v3/validate";
            string content = string.Empty;
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("login", "tweenapi");
            param.Add("apiKey", "R_c5ee0e30bdfff88723c4457cc331886b");
            param.Add("x_login", id);
            param.Add("x_apiKey", apikey);
            param.Add("format", "txt");

            if (!(new HttpVarious()).PostData(req, param, ref content))
            {
                // 通信エラーの場合はとりあえずチェックを通ったことにする
                return true;
            }
            else if (content.Trim() == "1")
            {
                // 検証成功
                return true;
            }
            else if (content.Trim() == "0")
            {
                // 検証失敗 APIキーとIDの組み合わせが違う
                return false;
            }
            else
            {
                // 規定外応答：通信エラーの可能性があるためとりあえずチェックを通ったことにする
                return true;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this._ValidationError = false;
        }

        private void HotkeyText_KeyDown(object sender, KeyEventArgs e)
        {
            // KeyValueで判定する。表示文字とのテーブルを用意すること
            HotkeyText.Text = e.KeyCode.ToString();
            HotkeyCode.Text = e.KeyValue.ToString();
            HotkeyText.Tag = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void HotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            HotkeyCtrl.Enabled = HotkeyCheck.Checked;
            HotkeyAlt.Enabled = HotkeyCheck.Checked;
            HotkeyShift.Enabled = HotkeyCheck.Checked;
            HotkeyWin.Enabled = HotkeyCheck.Checked;
            HotkeyText.Enabled = HotkeyCheck.Checked;
            HotkeyCode.Enabled = HotkeyCheck.Checked;
        }

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(GetMoreTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UseChangeGetCount_CheckedChanged(object sender, EventArgs e)
        {
            GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked;
            FirstTextCountApi.Enabled = UseChangeGetCount.Checked;
            Label28.Enabled = UseChangeGetCount.Checked;
            Label30.Enabled = UseChangeGetCount.Checked;
            Label53.Enabled = UseChangeGetCount.Checked;
            Label66.Enabled = UseChangeGetCount.Checked;
            Label17.Enabled = UseChangeGetCount.Checked;
            Label25.Enabled = UseChangeGetCount.Checked;
            SearchTextCountApi.Enabled = UseChangeGetCount.Checked;
            FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked;
            UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked;
            ListTextCountApi.Enabled = UseChangeGetCount.Checked;
        }

        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(FirstTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void SearchTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(SearchTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 100))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void FavoritesTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(FavoritesTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UserTimelineTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(UserTimelineTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void ListTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(ListTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private EventCheckboxTblElement[] GetEventCheckboxTable()
        {
            if (this.eventCheckboxTableElements == null)
            {
                this.eventCheckboxTableElements = new EventCheckboxTblElement[] 
                {
                    new EventCheckboxTblElement { CheckBox = CheckFavoritesEvent, Type = EventType.Favorite },
                    new EventCheckboxTblElement { CheckBox = CheckUnfavoritesEvent, Type = EventType.Unfavorite },
                    new EventCheckboxTblElement { CheckBox = CheckFollowEvent, Type = EventType.Follow },
                    new EventCheckboxTblElement { CheckBox = CheckListMemberAddedEvent, Type = EventType.ListMemberAdded },
                    new EventCheckboxTblElement { CheckBox = CheckListMemberRemovedEvent, Type = EventType.ListMemberRemoved },
                    new EventCheckboxTblElement { CheckBox = CheckBlockEvent, Type = EventType.Block },
                    new EventCheckboxTblElement { CheckBox = CheckUserUpdateEvent, Type = EventType.UserUpdate },
                    new EventCheckboxTblElement { CheckBox = CheckListCreatedEvent, Type = EventType.ListCreated }
                };
            }
            return this.eventCheckboxTableElements;
        }

        private void GetEventNotifyFlag(ref EventType eventnotifyflag, ref EventType isMyeventnotifyflag)
        {
            EventType evt = EventType.None;
            EventType myevt = EventType.None;

            foreach (EventCheckboxTblElement tbl in this.GetEventCheckboxTable())
            {
                switch (tbl.CheckBox.CheckState)
                {
                    case CheckState.Checked:
                        evt = evt | tbl.Type;
                        myevt = myevt | tbl.Type;
                        break;
                    case CheckState.Indeterminate:
                        evt = evt | tbl.Type;
                        break;
                    case CheckState.Unchecked:
                        break;
                }
            }
            eventnotifyflag = evt;
            isMyeventnotifyflag = myevt;
        }

        private void ApplyEventNotifyFlag(bool rootEnabled, EventType eventnotifyflag, EventType isMyeventnotifyflag)
        {
            var evt = eventnotifyflag;
            var myevt = isMyeventnotifyflag;

            CheckEventNotify.Checked = rootEnabled;

            foreach (EventCheckboxTblElement tbl in this.GetEventCheckboxTable())
            {
                if (Convert.ToBoolean(evt & tbl.Type))
                {
                    if (Convert.ToBoolean(myevt & tbl.Type))
                    {
                        tbl.CheckBox.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        tbl.CheckBox.CheckState = CheckState.Indeterminate;
                    }
                }
                else
                {
                    tbl.CheckBox.CheckState = CheckState.Unchecked;
                }
                tbl.CheckBox.Enabled = rootEnabled;
            }
        }

        private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        {
            foreach (EventCheckboxTblElement tbl in this.GetEventCheckboxTable())
            {
                tbl.CheckBox.Enabled = CheckEventNotify.Checked;
            }
        }

        private void SoundFileListup()
        {
            if (string.IsNullOrEmpty(this._MyEventSoundFile))
            {
                this._MyEventSoundFile = string.Empty;
            }
            ComboBoxEventNotifySound.Items.Clear();
            ComboBoxEventNotifySound.Items.Add(string.Empty);
            DirectoryInfo oDir = new DirectoryInfo(MyCommon.AppDir + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.AppDir, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboBoxEventNotifySound.Items.Add(oFile.Name);
            }
            int idx = ComboBoxEventNotifySound.Items.IndexOf(this._MyEventSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }
            ComboBoxEventNotifySound.SelectedIndex = idx;
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !string.IsNullOrEmpty(UserAppointUrlText.Text))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }

        private void IsPreviewFoursquareCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FoursquareGroupBox.Enabled = IsPreviewFoursquareCheckBox.Checked;
        }

        private void OpenUrl(string url)
        {
            string myPath = url;
            string path = this.BrowserPathText.Text;
            try
            {
                if (!string.IsNullOrEmpty(this.BrowserPath))
                {
                    if (path.StartsWith("\"") && path.Length > 2 && path.IndexOf("\"", 2) > -1)
                    {
                        int sep = path.IndexOf("\"", 2);
                        string browserPath = path.Substring(1, sep - 1);
                        string arg = string.Empty;
                        if (sep < path.Length - 1)
                        {
                            arg = path.Substring(sep + 1);
                        }
                        myPath = arg + " " + myPath;
                        Process.Start(browserPath, myPath);
                    }
                    else
                    {
                        Process.Start(path, myPath);
                    }
                }
                else
                {
                    Process.Start(myPath);
                }
            }
            catch (Exception)
            {
            }
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }

        public class IntervalChangedEventArgs : EventArgs
        {
            public bool UserStream;
            public bool Timeline;
            public bool Reply;
            public bool DirectMessage;
            public bool PublicSearch;
            public bool Lists;
            public bool UserTimeline;
        }

        private class EventCheckboxTblElement
        {
            public CheckBox CheckBox;
            public EventType Type;
        }
    }
}