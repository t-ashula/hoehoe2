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
        #region privates

        private static AppendSettingDialog instance = new AppendSettingDialog();
        private Twitter tw;
        private bool validationError;
        private EventType myEventNotifyFlag;
        private EventType isMyEventNotifyFlag;
        private string myTranslateLanguage;
        private long initialUserId;
        private string pin;
        private EventCheckboxTblElement[] eventCheckboxTableElements = null;

        #endregion privates

        #region constructor

        public AppendSettingDialog()
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            this.Icon = Hoehoe.Properties.Resources.MIcon;
        }

        #endregion constructor

        #region delegates

        public delegate void IntervalChangedEventHandler(object sender, IntervalChangedEventArgs e);

        #endregion delegates

        #region events

        public event IntervalChangedEventHandler IntervalChanged;

        #endregion events

        #region properties

        public static AppendSettingDialog Instance
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

        public DataModels.Twitter.Configuration TwitterConfiguration { get; set; }

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

        // 未使用
        public Font FontUnread { get; set; }

        public Color ColorUnread { get; set; }

        // リストフォントとして使用
        public Font FontReaded { get; set; }

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

        public EventType EventNotifyFlag
        {
            get { return this.myEventNotifyFlag; }
            set { this.myEventNotifyFlag = value; }
        }

        public EventType IsMyEventNotifyFlag
        {
            get { return this.isMyEventNotifyFlag; }
            set { this.isMyEventNotifyFlag = value; }
        }

        public bool ForceEventNotify { get; set; }

        public bool FavEventUnread { get; set; }

        public string TranslateLanguage
        {
            get
            {
                return this.myTranslateLanguage;
            }

            set
            {
                this.myTranslateLanguage = value;
                ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
            }
        }

        public string EventSoundFile { get; set; }

        public int ListDoubleClickAction { get; set; }

        public string UserAppointUrl { get; set; }

        public bool HotkeyEnabled { get; set; }

        public Keys HotkeyKey { get; set; }

        public int HotkeyValue { get; set; }

        public Keys HotkeyMod { get; set; }

        public bool BlinkNewMentions { get; set; }

        #endregion properties

        #region event handler

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            StatusText.Enabled = !CheckUseRecommendStatus.Checked;
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
                    this.validationError = true;
                    TreeViewSetting.SelectedNode.Name = "TweetActNode"; // 動作タブを選択
                    TextBitlyId.Focus();
                    return;
                }
                else
                {
                    this.validationError = false;
                }
            }
            else
            {
                this.validationError = false;
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
                        this.tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                        if (u.UserId == 0)
                        {
                            this.tw.VerifyCredentials();
                            u.UserId = this.tw.UserId;
                        }

                        break;
                    }
                }
            }
            else
            {
                this.tw.ClearAuthInfo();
                this.tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            }

            IntervalChangedEventArgs arg = new IntervalChangedEventArgs();
            bool isIntervalChanged = false;

            try
            {
                this.UserstreamStartup = this.StartupUserstreamCheck.Checked;

                if (this.UserstreamPeriodInt != Convert.ToInt32(UserstreamPeriod.Text))
                {
                    this.UserstreamPeriodInt = Convert.ToInt32(UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }

                if (this.TimelinePeriodInt != Convert.ToInt32(TimelinePeriod.Text))
                {
                    this.TimelinePeriodInt = Convert.ToInt32(TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }

                if (this.DMPeriodInt != Convert.ToInt32(DMPeriod.Text))
                {
                    this.DMPeriodInt = Convert.ToInt32(DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }

                if (this.PubSearchPeriodInt != Convert.ToInt32(PubSearchPeriod.Text))
                {
                    this.PubSearchPeriodInt = Convert.ToInt32(PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (this.ListsPeriodInt != Convert.ToInt32(ListsPeriod.Text))
                {
                    this.ListsPeriodInt = Convert.ToInt32(ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }

                if (this.ReplyPeriodInt != Convert.ToInt32(ReplyPeriod.Text))
                {
                    this.ReplyPeriodInt = Convert.ToInt32(ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }

                if (this.UserTimelinePeriodInt != Convert.ToInt32(UserTimelinePeriod.Text))
                {
                    this.UserTimelinePeriodInt = Convert.ToInt32(UserTimelinePeriod.Text);
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

                this.Readed = StartupReaded.Checked;
                switch (IconSize.SelectedIndex)
                {
                    case 0:
                        this.IconSz = Hoehoe.IconSizes.IconNone;
                        break;
                    case 1:
                        this.IconSz = Hoehoe.IconSizes.Icon16;
                        break;
                    case 2:
                        this.IconSz = Hoehoe.IconSizes.Icon24;
                        break;
                    case 3:
                        this.IconSz = Hoehoe.IconSizes.Icon48;
                        break;
                    case 4:
                        this.IconSz = Hoehoe.IconSizes.Icon48_2;
                        break;
                }

                this.Status = StatusText.Text;
                this.PlaySound = PlaySnd.Checked;
                this.UnreadManage = UReadMng.Checked;
                this.OneWayLove = OneWayLv.Checked;
                this.FontUnread = lblUnread.Font;                // 未使用
                this.ColorUnread = lblUnread.ForeColor;
                this.FontReaded = lblListFont.Font;              // リストフォントとして使用
                this.ColorReaded = lblListFont.ForeColor;
                this.ColorFav = lblFav.ForeColor;
                this.ColorOWL = lblOWL.ForeColor;
                this.ColorRetweet = lblRetweet.ForeColor;
                this.FontDetail = lblDetail.Font;
                this.ColorSelf = lblSelf.BackColor;
                this.ColorAtSelf = lblAtSelf.BackColor;
                this.ColorTarget = lblTarget.BackColor;
                this.ColorAtTarget = lblAtTarget.BackColor;
                this.ColorAtFromTarget = lblAtFromTarget.BackColor;
                this.ColorAtTo = lblAtTo.BackColor;
                this.ColorInputBackcolor = lblInputBackcolor.BackColor;
                this.ColorInputFont = lblInputFont.ForeColor;
                this.ColorListBackcolor = lblListBackcolor.BackColor;
                this.ColorDetailBackcolor = lblDetailBackcolor.BackColor;
                this.ColorDetail = lblDetail.ForeColor;
                this.ColorDetailLink = lblDetailLink.ForeColor;
                this.FontInputFont = lblInputFont.Font;
                switch (cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        this.NameBalloon = NameBalloonEnum.None;
                        break;
                    case 1:
                        this.NameBalloon = NameBalloonEnum.UserID;
                        break;
                    case 2:
                        this.NameBalloon = NameBalloonEnum.NickName;
                        break;
                }

                switch (ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        this.PostShiftEnter = true;
                        this.PostCtrlEnter = false;
                        break;
                    case 1:
                        this.PostCtrlEnter = true;
                        this.PostShiftEnter = false;
                        break;
                    case 0:
                        this.PostCtrlEnter = false;
                        this.PostShiftEnter = false;
                        break;
                }

                this.CountApi = Convert.ToInt32(TextCountApi.Text);
                this.CountApiReply = Convert.ToInt32(TextCountApiReply.Text);
                this.BrowserPath = this.BrowserPathText.Text.Trim();
                this.PostAndGet = CheckPostAndGet.Checked;
                this.UseRecommendStatus = CheckUseRecommendStatus.Checked;
                this.DispUsername = CheckDispUsername.Checked;
                this.CloseToExit = CheckCloseToExit.Checked;
                this.MinimizeToTray = CheckMinimizeToTray.Checked;
                switch (ComboDispTitle.SelectedIndex)
                {
                    case 0:
                        // None
                        this.DispLatestPost = DispTitleEnum.None;
                        break;
                    case 1:
                        // Ver
                        this.DispLatestPost = DispTitleEnum.Ver;
                        break;
                    case 2:
                        // Post
                        this.DispLatestPost = DispTitleEnum.Post;
                        break;
                    case 3:
                        // RepCount
                        this.DispLatestPost = DispTitleEnum.UnreadRepCount;
                        break;
                    case 4:
                        // AllCount
                        this.DispLatestPost = DispTitleEnum.UnreadAllCount;
                        break;
                    case 5:
                        // Rep+All
                        this.DispLatestPost = DispTitleEnum.UnreadAllRepCount;
                        break;
                    case 6:
                        // Unread/All
                        this.DispLatestPost = DispTitleEnum.UnreadCountAllCount;
                        break;
                    case 7:
                        // Count of Status/Follow/Follower
                        this.DispLatestPost = DispTitleEnum.OwnStatus;
                        break;
                }

                this.SortOrderLock = CheckSortOrderLock.Checked;
                this.TinyUrlResolve = CheckTinyURL.Checked;
                this.ShortUrlForceResolve = CheckForceResolve.Checked;
                ShortUrl.IsResolve = this.TinyUrlResolve;
                ShortUrl.IsForceResolve = this.ShortUrlForceResolve;
                if (RadioProxyNone.Checked)
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.Specified;
                }

                this.ProxyAddress = TextProxyAddress.Text.Trim();
                this.ProxyPort = int.Parse(TextProxyPort.Text.Trim());
                this.ProxyUser = TextProxyUser.Text.Trim();
                this.ProxyPassword = TextProxyPassword.Text.Trim();
                this.PeriodAdjust = CheckPeriodAdjust.Checked;
                this.StartupVersion = CheckStartupVersion.Checked;
                this.StartupFollowers = CheckStartupFollowers.Checked;
                this.RestrictFavCheck = CheckFavRestrict.Checked;
                this.AlwaysTop = CheckAlwaysTop.Checked;
                this.UrlConvertAuto = CheckAutoConvertUrl.Checked;
                this.ShortenTco = ShortenTcoCheck.Checked;
                this.OutputzEnabled = CheckOutputz.Checked;
                this.OutputzKey = TextBoxOutputzKey.Text.Trim();

                switch (ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        this.OutputzUrlmode = OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        this.OutputzUrlmode = OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                this.Nicoms = CheckNicoms.Checked;
                this.UseUnreadStyle = chkUnreadStyle.Checked;
                this.DateTimeFormat = CmbDateTimeFormat.Text;
                this.DefaultTimeOut = Convert.ToInt32(ConnectionTimeOut.Text);
                this.RetweetNoConfirm = CheckRetweetNoConfirm.Checked;
                this.LimitBalloon = CheckBalloonLimit.Checked;
                this.EventNotifyEnabled = CheckEventNotify.Checked;
                this.GetEventNotifyFlag(ref this.myEventNotifyFlag, ref this.isMyEventNotifyFlag);
                this.ForceEventNotify = CheckForceEventNotify.Checked;
                this.FavEventUnread = CheckFavEventUnread.Checked;
                this.myTranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
                this.EventSoundFile = Convert.ToString(ComboBoxEventNotifySound.SelectedItem);
                this.AutoShortUrlFirst = (UrlConverter)ComboBoxAutoShortUrlFirst.SelectedIndex;
                this.TabIconDisp = chkTabIconDisp.Checked;
                this.ReadOwnPost = chkReadOwnPost.Checked;
                this.GetFav = chkGetFav.Checked;
                this.IsMonospace = CheckMonospace.Checked;
                this.ReadOldPosts = CheckReadOldPosts.Checked;
                this.UseSsl = CheckUseSsl.Checked;
                this.BitlyUser = TextBitlyId.Text;
                this.BitlyPwd = TextBitlyPw.Text;
                this.ShowGrid = CheckShowGrid.Checked;
                this.UseAtIdSupplement = CheckAtIdSupple.Checked;
                this.UseHashSupplement = CheckHashSupple.Checked;
                this.PreviewEnable = CheckPreviewEnable.Checked;
                this.TwitterApiUrl = TwitterAPIText.Text.Trim();
                this.TwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim();
                switch (ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        this.ReplyIconState = ReplyIconState.None;
                        break;
                    case 1:
                        this.ReplyIconState = ReplyIconState.StaticIcon;
                        break;
                    case 2:
                        this.ReplyIconState = ReplyIconState.BlinkIcon;
                        break;
                }

                switch (LanguageCombo.SelectedIndex)
                {
                    case 0:
                        this.Language = "OS";
                        break;
                    case 1:
                        this.Language = "ja";
                        break;
                    case 2:
                        this.Language = "en";
                        break;
                    case 3:
                        this.Language = "zh-CN";
                        break;
                    default:
                        this.Language = "en";
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
                    if (int.TryParse(HotkeyCode.Text, out tmp))
                    {
                        this.HotkeyValue = tmp;
                    }
                }

                this.HotkeyKey = (Keys)HotkeyText.Tag;
                this.BlinkNewMentions = ChkNewMentionsBlink.Checked;
                this.UseAdditionalCount = UseChangeGetCount.Checked;
                this.MoreCountApi = Convert.ToInt32(GetMoreTextCountApi.Text);
                this.FirstCountApi = Convert.ToInt32(FirstTextCountApi.Text);
                this.SearchCountApi = Convert.ToInt32(SearchTextCountApi.Text);
                this.FavoritesCountApi = Convert.ToInt32(FavoritesTextCountApi.Text);
                this.UserTimelineCountApi = Convert.ToInt32(UserTimelineTextCountApi.Text);
                this.ListCountApi = Convert.ToInt32(ListTextCountApi.Text);
                this.OpenUserTimeline = CheckOpenUserTimeline.Checked;
                this.ListDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex;
                this.UserAppointUrl = UserAppointUrlText.Text;
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
                if (this.initialUserId > 0)
                {
                    foreach (UserAccount u in this.UserAccounts)
                    {
                        if (u.UserId == this.initialUserId)
                        {
                            this.tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                            userSet = true;
                            break;
                        }
                    }
                }

                // 認証済みアカウントが削除されていた場合、もしくは起動時アカウントがなかった場合は、アクティブユーザーなしとして初期化
                if (!userSet)
                {
                    this.tw.ClearAuthInfo();
                    this.tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
                }
            }

            if (this.tw != null && string.IsNullOrEmpty(this.tw.Username) && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(Hoehoe.Properties.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            if (this.validationError)
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
            this.tw = ((TweenMain)this.Owner).TwitterInstance;
            string uname = this.tw.Username;
            string pw = this.tw.Password;
            string tk = this.tw.AccessToken;
            string tks = this.tw.AccessTokenSecret;

            this.AuthClearButton.Enabled = true;

            this.AuthUserCombo.Items.Clear();
            if (this.UserAccounts.Count > 0)
            {
                this.AuthUserCombo.Items.AddRange(this.UserAccounts.ToArray());
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.UserId == this.tw.UserId)
                    {
                        this.AuthUserCombo.SelectedItem = u;
                        this.initialUserId = u.UserId;
                        break;
                    }
                }
            }

            this.StartupUserstreamCheck.Checked = this.UserstreamStartup;
            UserstreamPeriod.Text = this.UserstreamPeriodInt.ToString();
            TimelinePeriod.Text = this.TimelinePeriodInt.ToString();
            ReplyPeriod.Text = this.ReplyPeriodInt.ToString();
            DMPeriod.Text = this.DMPeriodInt.ToString();
            PubSearchPeriod.Text = this.PubSearchPeriodInt.ToString();
            ListsPeriod.Text = this.ListsPeriodInt.ToString();
            UserTimelinePeriod.Text = this.UserTimelinePeriodInt.ToString();

            StartupReaded.Checked = this.Readed;
            switch (this.IconSz)
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

            StatusText.Text = this.Status;
            UReadMng.Checked = this.UnreadManage;
            StartupReaded.Enabled = this.UnreadManage != false;
            PlaySnd.Checked = this.PlaySound;
            OneWayLv.Checked = this.OneWayLove;

            lblListFont.Font = this.FontReaded;
            lblUnread.Font = this.FontUnread;
            lblUnread.ForeColor = this.ColorUnread;
            lblListFont.ForeColor = this.ColorReaded;
            lblFav.ForeColor = this.ColorFav;
            lblOWL.ForeColor = this.ColorOWL;
            lblRetweet.ForeColor = this.ColorRetweet;
            lblDetail.Font = this.FontDetail;
            lblSelf.BackColor = this.ColorSelf;
            lblAtSelf.BackColor = this.ColorAtSelf;
            lblTarget.BackColor = this.ColorTarget;
            lblAtTarget.BackColor = this.ColorAtTarget;
            lblAtFromTarget.BackColor = this.ColorAtFromTarget;
            lblAtTo.BackColor = this.ColorAtTo;
            lblInputBackcolor.BackColor = this.ColorInputBackcolor;
            lblInputFont.ForeColor = this.ColorInputFont;
            lblInputFont.Font = this.FontInputFont;
            lblListBackcolor.BackColor = this.ColorListBackcolor;
            lblDetailBackcolor.BackColor = this.ColorDetailBackcolor;
            lblDetail.ForeColor = this.ColorDetail;
            lblDetailLink.ForeColor = this.ColorDetailLink;

            switch (this.NameBalloon)
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

            if (this.PostCtrlEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (this.PostShiftEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            TextCountApi.Text = this.CountApi.ToString();
            TextCountApiReply.Text = this.CountApiReply.ToString();
            BrowserPathText.Text = this.BrowserPath;
            CheckPostAndGet.Checked = this.PostAndGet;
            CheckUseRecommendStatus.Checked = this.UseRecommendStatus;
            CheckDispUsername.Checked = this.DispUsername;
            CheckCloseToExit.Checked = this.CloseToExit;
            CheckMinimizeToTray.Checked = this.MinimizeToTray;
            switch (this.DispLatestPost)
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

            CheckSortOrderLock.Checked = this.SortOrderLock;
            CheckTinyURL.Checked = this.TinyUrlResolve;
            CheckForceResolve.Checked = this.ShortUrlForceResolve;
            switch (this.SelectedProxyType)
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

            TextProxyAddress.Text = this.ProxyAddress;
            TextProxyPort.Text = this.ProxyPort.ToString();
            TextProxyUser.Text = this.ProxyUser;
            TextProxyPassword.Text = this.ProxyPassword;

            CheckPeriodAdjust.Checked = this.PeriodAdjust;
            CheckStartupVersion.Checked = this.StartupVersion;
            CheckStartupFollowers.Checked = this.StartupFollowers;
            CheckFavRestrict.Checked = this.RestrictFavCheck;
            CheckAlwaysTop.Checked = this.AlwaysTop;
            CheckAutoConvertUrl.Checked = this.UrlConvertAuto;
            ShortenTcoCheck.Checked = this.ShortenTco;
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
            CheckOutputz.Checked = this.OutputzEnabled;
            TextBoxOutputzKey.Text = this.OutputzKey;

            switch (this.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            CheckNicoms.Checked = this.Nicoms;
            chkUnreadStyle.Checked = this.UseUnreadStyle;
            CmbDateTimeFormat.Text = this.DateTimeFormat;
            ConnectionTimeOut.Text = this.DefaultTimeOut.ToString();
            CheckRetweetNoConfirm.Checked = this.RetweetNoConfirm;
            CheckBalloonLimit.Checked = this.LimitBalloon;

            this.ApplyEventNotifyFlag(this.EventNotifyEnabled, this.myEventNotifyFlag, this.isMyEventNotifyFlag);
            CheckForceEventNotify.Checked = this.ForceEventNotify;
            CheckFavEventUnread.Checked = this.FavEventUnread;
            ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(this.myTranslateLanguage);
            this.SoundFileListup();
            ComboBoxAutoShortUrlFirst.SelectedIndex = (int)this.AutoShortUrlFirst;
            chkTabIconDisp.Checked = this.TabIconDisp;
            chkReadOwnPost.Checked = this.ReadOwnPost;
            chkGetFav.Checked = this.GetFav;
            CheckMonospace.Checked = this.IsMonospace;
            CheckReadOldPosts.Checked = this.ReadOldPosts;
            CheckUseSsl.Checked = this.UseSsl;
            TextBitlyId.Text = this.BitlyUser;
            TextBitlyPw.Text = this.BitlyPwd;
            TextBitlyId.Modified = false;
            TextBitlyPw.Modified = false;
            CheckShowGrid.Checked = this.ShowGrid;
            CheckAtIdSupple.Checked = this.UseAtIdSupplement;
            CheckHashSupple.Checked = this.UseHashSupplement;
            CheckPreviewEnable.Checked = this.PreviewEnable;
            TwitterAPIText.Text = this.TwitterApiUrl;
            TwitterSearchAPIText.Text = this.TwitterSearchApiUrl;
            switch (this.ReplyIconState)
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

            switch (this.Language)
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

            GetMoreTextCountApi.Text = this.MoreCountApi.ToString();
            FirstTextCountApi.Text = this.FirstCountApi.ToString();
            SearchTextCountApi.Text = this.SearchCountApi.ToString();
            FavoritesTextCountApi.Text = this.FavoritesCountApi.ToString();
            UserTimelineTextCountApi.Text = this.UserTimelineCountApi.ToString();
            ListTextCountApi.Text = this.ListCountApi.ToString();
            UseChangeGetCount.Checked = this.UseAdditionalCount;
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
            CheckOpenUserTimeline.Checked = this.OpenUserTimeline;
            ListDoubleClickActionComboBox.SelectedIndex = this.ListDoubleClickAction;
            UserAppointUrlText.Text = this.UserAppointUrl;
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

            this.TreeViewSetting.Nodes["BasedNode"].Tag = this.BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = GetCountPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Tag = this.ActionPanel;
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

        private void ButtonFontAndColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
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

            switch (btn.Name)
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

            switch (btn.Name)
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

        private void ButtonColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            DialogResult rtn = default(DialogResult);

            ColorDialog1.AllowFullOpen = true;
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = false;
            ColorDialog1.SolidColorOnly = false;

            switch (btn.Name)
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
            {
                return;
            }

            switch (btn.Name)
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

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.validationError = false;
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

        private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        {
            foreach (EventCheckboxTblElement tbl in this.GetEventCheckboxTable())
            {
                tbl.CheckBox.Enabled = CheckEventNotify.Checked;
            }
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

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }

        #endregion event handler

        #region private methods

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
            this.tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            string pinPageUrl = string.Empty;
            string rslt = this.tw.StartAuthentication(ref pinPageUrl);
            if (string.IsNullOrEmpty(rslt))
            {
                using (var ab = new AuthBrowser())
                {
                    ab.IsAuthorized = true;
                    ab.UrlString = pinPageUrl;
                    if (ab.ShowDialog(this) == DialogResult.OK)
                    {
                        this.pin = ab.PinString;
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
            string pin = this.pin;
            string rslt = this.tw.Authenticate(pin);
            if (string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK);
                int idx = -1;
                var user = new UserAccount
                {
                    Username = this.tw.Username,
                    UserId = this.tw.UserId,
                    Token = this.tw.AccessToken,
                    TokenSecret = this.tw.AccessTokenSecret
                };

                foreach (var u in this.AuthUserCombo.Items)
                {
                    if (((UserAccount)u).Username.ToLower() == this.tw.Username.ToLower())
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

            if (this.tw != null)
            {
                if (MyCommon.TwitterApiInfo.MaxCount == -1)
                {
                    if (Twitter.AccountState == AccountState.Valid)
                    {
                        MyCommon.TwitterApiInfo.UsingCount = usingApi;
                        var proc = new Thread(new System.Threading.ThreadStart(() =>
                        {
                            this.tw.GetInfoApi(null); // 取得エラー時はinfoCountは初期状態（値：-1）
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

            LabelPostAndGet.Visible = CheckPostAndGet.Checked && !this.tw.UserStreamEnabled;
            LabelUserStreamActive.Visible = this.tw.UserStreamEnabled;

            LabelApiUsingUserStreamEnabled.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse2, apiLists + apiUserTimeline);
            LabelApiUsingUserStreamEnabled.Visible = this.tw.UserStreamEnabled;
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

        private void SoundFileListup()
        {
            if (string.IsNullOrEmpty(this.EventSoundFile))
            {
                this.EventSoundFile = string.Empty;
            }

            ComboBoxEventNotifySound.Items.Clear();
            ComboBoxEventNotifySound.Items.Add(string.Empty);
            DirectoryInfo dir = new DirectoryInfo(MyCommon.AppDir + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.AppDir, "Sounds")))
            {
                dir = dir.GetDirectories("Sounds")[0];
            }

            foreach (FileInfo file in dir.GetFiles("*.wav"))
            {
                ComboBoxEventNotifySound.Items.Add(file.Name);
            }

            int idx = ComboBoxEventNotifySound.Items.IndexOf(this.EventSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            ComboBoxEventNotifySound.SelectedIndex = idx;
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

        #endregion private methods

        #region inner class

        private class EventCheckboxTblElement
        {
            public CheckBox CheckBox;
            public EventType Type;
        }

        #endregion inner class
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
}