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
    using System.Threading;
    using System.Windows.Forms;

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

        private Configs configurations = Configs.Instance;

        #endregion privates

        #region constructor

        public AppendSettingDialog()
        {
            // この呼び出しはデザイナーで必要です。
            this.InitializeComponent();

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
                this.ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
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
            this.StatusText.Enabled = !this.CheckUseRecommendStatus.Checked;
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
                this.IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNetworkAvailable()
                && (this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Bitly
                || this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Jmp)
                && (!string.IsNullOrEmpty(this.TextBitlyId.Text) || !string.IsNullOrEmpty(this.TextBitlyPw.Text)))
            {
                if (!this.BitlyValidation(this.TextBitlyId.Text, this.TextBitlyPw.Text))
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.SettingSave_ClickText1);
                    this.validationError = true;
                    this.TreeViewSetting.SelectedNode.Name = "TweetActNode"; // 動作タブを選択
                    this.TextBitlyId.Focus();
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

            this.configurations.UserAccounts.Clear();
            foreach (var u in this.AuthUserCombo.Items)
            {
                this.configurations.UserAccounts.Add((UserAccount)u);
            }

            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                foreach (UserAccount u in this.configurations.UserAccounts)
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
                this.configurations.UserstreamStartup = this.StartupUserstreamCheck.Checked;

                if (this.configurations.UserstreamPeriodInt != Convert.ToInt32(this.UserstreamPeriod.Text))
                {
                    this.configurations.UserstreamPeriodInt = Convert.ToInt32(this.UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.TimelinePeriodInt != Convert.ToInt32(this.TimelinePeriod.Text))
                {
                    this.configurations.TimelinePeriodInt = Convert.ToInt32(this.TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.DMPeriodInt != Convert.ToInt32(this.DMPeriod.Text))
                {
                    this.configurations.DMPeriodInt = Convert.ToInt32(this.DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.PubSearchPeriodInt != Convert.ToInt32(this.PubSearchPeriod.Text))
                {
                    this.configurations.PubSearchPeriodInt = Convert.ToInt32(this.PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.ListsPeriodInt != Convert.ToInt32(this.ListsPeriod.Text))
                {
                    this.configurations.ListsPeriodInt = Convert.ToInt32(this.ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.ReplyPeriodInt != Convert.ToInt32(this.ReplyPeriod.Text))
                {
                    this.configurations.ReplyPeriodInt = Convert.ToInt32(this.ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }

                if (this.configurations.UserTimelinePeriodInt != Convert.ToInt32(this.UserTimelinePeriod.Text))
                {
                    this.configurations.UserTimelinePeriodInt = Convert.ToInt32(this.UserTimelinePeriod.Text);
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

                this.configurations.Readed = this.StartupReaded.Checked;
                switch (this.IconSize.SelectedIndex)
                {
                    case 0:
                        this.configurations.IconSz = IconSizes.IconNone;
                        break;
                    case 1:
                        this.configurations.IconSz = IconSizes.Icon16;
                        break;
                    case 2:
                        this.configurations.IconSz = IconSizes.Icon24;
                        break;
                    case 3:
                        this.configurations.IconSz = IconSizes.Icon48;
                        break;
                    case 4:
                        this.configurations.IconSz = IconSizes.Icon48_2;
                        break;
                }

                this.configurations.Status = this.StatusText.Text;
                this.configurations.PlaySound = this.PlaySnd.Checked;
                this.configurations.UnreadManage = this.UReadMng.Checked;
                this.configurations.OneWayLove = this.OneWayLv.Checked;
                this.configurations.FontUnread = this.lblUnread.Font;                // 未使用
                this.configurations.ColorUnread = this.lblUnread.ForeColor;
                this.configurations.FontReaded = this.lblListFont.Font;              // リストフォントとして使用
                this.configurations.ColorReaded = this.lblListFont.ForeColor;
                this.configurations.ColorFav = this.lblFav.ForeColor;
                this.configurations.ColorOWL = this.lblOWL.ForeColor;
                this.configurations.ColorRetweet = this.lblRetweet.ForeColor;
                this.configurations.FontDetail = this.lblDetail.Font;
                this.configurations.ColorSelf = this.lblSelf.BackColor;
                this.configurations.ColorAtSelf = this.lblAtSelf.BackColor;
                this.configurations.ColorTarget = this.lblTarget.BackColor;
                this.configurations.ColorAtTarget = this.lblAtTarget.BackColor;
                this.configurations.ColorAtFromTarget = this.lblAtFromTarget.BackColor;
                this.configurations.ColorAtTo = this.lblAtTo.BackColor;
                this.configurations.ColorInputBackcolor = this.lblInputBackcolor.BackColor;
                this.configurations.ColorInputFont = this.lblInputFont.ForeColor;
                this.configurations.ColorListBackcolor = this.lblListBackcolor.BackColor;
                this.configurations.ColorDetailBackcolor = this.lblDetailBackcolor.BackColor;
                this.configurations.ColorDetail = this.lblDetail.ForeColor;
                this.configurations.ColorDetailLink = this.lblDetailLink.ForeColor;
                this.configurations.FontInputFont = this.lblInputFont.Font;
                switch (this.cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        this.configurations.NameBalloon = NameBalloonEnum.None;
                        break;
                    case 1:
                        this.configurations.NameBalloon = NameBalloonEnum.UserID;
                        break;
                    case 2:
                        this.configurations.NameBalloon = NameBalloonEnum.NickName;
                        break;
                }

                switch (this.ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        this.configurations.PostShiftEnter = true;
                        this.configurations.PostCtrlEnter = false;
                        break;
                    case 1:
                        this.configurations.PostCtrlEnter = true;
                        this.configurations.PostShiftEnter = false;
                        break;
                    case 0:
                        this.configurations.PostCtrlEnter = false;
                        this.configurations.PostShiftEnter = false;
                        break;
                }

                this.configurations.CountApi = Convert.ToInt32(this.TextCountApi.Text);
                this.configurations.CountApiReply = Convert.ToInt32(this.TextCountApiReply.Text);
                this.configurations.BrowserPath = this.BrowserPathText.Text.Trim();
                this.configurations.PostAndGet = this.CheckPostAndGet.Checked;
                this.configurations.UseRecommendStatus = this.CheckUseRecommendStatus.Checked;
                this.configurations.DispUsername = this.CheckDispUsername.Checked;
                this.configurations.CloseToExit = this.CheckCloseToExit.Checked;
                this.configurations.MinimizeToTray = this.CheckMinimizeToTray.Checked;
                switch (this.ComboDispTitle.SelectedIndex)
                {
                    case 0:
                        // None
                        this.configurations.DispLatestPost = DispTitleEnum.None;
                        break;
                    case 1:
                        // Ver
                        this.configurations.DispLatestPost = DispTitleEnum.Ver;
                        break;
                    case 2:
                        // Post
                        this.configurations.DispLatestPost = DispTitleEnum.Post;
                        break;
                    case 3:
                        // RepCount
                        this.configurations.DispLatestPost = DispTitleEnum.UnreadRepCount;
                        break;
                    case 4:
                        // AllCount
                        this.configurations.DispLatestPost = DispTitleEnum.UnreadAllCount;
                        break;
                    case 5:
                        // Rep+All
                        this.configurations.DispLatestPost = DispTitleEnum.UnreadAllRepCount;
                        break;
                    case 6:
                        // Unread/All
                        this.configurations.DispLatestPost = DispTitleEnum.UnreadCountAllCount;
                        break;
                    case 7:
                        // Count of Status/Follow/Follower
                        this.configurations.DispLatestPost = DispTitleEnum.OwnStatus;
                        break;
                }

                this.SortOrderLock = this.CheckSortOrderLock.Checked;
                this.configurations.TinyUrlResolve = this.CheckTinyURL.Checked;
                this.configurations.ShortUrlForceResolve = this.CheckForceResolve.Checked;
                ShortUrl.IsResolve = this.configurations.TinyUrlResolve;
                ShortUrl.IsForceResolve = this.configurations.ShortUrlForceResolve;
                if (this.RadioProxyNone.Checked)
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.None;
                }
                else if (this.RadioProxyIE.Checked)
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    this.SelectedProxyType = HttpConnection.ProxyType.Specified;
                }

                this.ProxyAddress = this.TextProxyAddress.Text.Trim();
                this.ProxyPort = int.Parse(this.TextProxyPort.Text.Trim());
                this.ProxyUser = this.TextProxyUser.Text.Trim();
                this.ProxyPassword = this.TextProxyPassword.Text.Trim();
                this.PeriodAdjust = this.CheckPeriodAdjust.Checked;
                this.StartupVersion = this.CheckStartupVersion.Checked;
                this.StartupFollowers = this.CheckStartupFollowers.Checked;
                this.RestrictFavCheck = this.CheckFavRestrict.Checked;
                this.AlwaysTop = this.CheckAlwaysTop.Checked;
                this.UrlConvertAuto = this.CheckAutoConvertUrl.Checked;
                this.ShortenTco = this.ShortenTcoCheck.Checked;
                this.OutputzEnabled = this.CheckOutputz.Checked;
                this.OutputzKey = this.TextBoxOutputzKey.Text.Trim();

                switch (this.ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        this.OutputzUrlmode = OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        this.OutputzUrlmode = OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                this.Nicoms = this.CheckNicoms.Checked;
                this.UseUnreadStyle = this.chkUnreadStyle.Checked;
                this.DateTimeFormat = this.CmbDateTimeFormat.Text;
                this.DefaultTimeOut = Convert.ToInt32(this.ConnectionTimeOut.Text);
                this.RetweetNoConfirm = this.CheckRetweetNoConfirm.Checked;
                this.LimitBalloon = this.CheckBalloonLimit.Checked;
                this.EventNotifyEnabled = this.CheckEventNotify.Checked;
                this.GetEventNotifyFlag(ref this.myEventNotifyFlag, ref this.isMyEventNotifyFlag);
                this.ForceEventNotify = this.CheckForceEventNotify.Checked;
                this.FavEventUnread = this.CheckFavEventUnread.Checked;
                this.myTranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(this.ComboBoxTranslateLanguage.SelectedIndex);
                this.EventSoundFile = Convert.ToString(this.ComboBoxEventNotifySound.SelectedItem);
                this.AutoShortUrlFirst = (UrlConverter)this.ComboBoxAutoShortUrlFirst.SelectedIndex;
                this.TabIconDisp = this.CheckTabIconDisp.Checked;
                this.ReadOwnPost = this.chkReadOwnPost.Checked;
                this.GetFav = this.CheckGetFav.Checked;
                this.IsMonospace = this.CheckMonospace.Checked;
                this.ReadOldPosts = this.CheckReadOldPosts.Checked;
                this.UseSsl = this.CheckUseSsl.Checked;
                this.BitlyUser = this.TextBitlyId.Text;
                this.BitlyPwd = this.TextBitlyPw.Text;
                this.ShowGrid = this.CheckShowGrid.Checked;
                this.UseAtIdSupplement = this.CheckAtIdSupple.Checked;
                this.UseHashSupplement = this.CheckHashSupple.Checked;
                this.PreviewEnable = this.CheckPreviewEnable.Checked;
                this.TwitterApiUrl = this.TwitterAPIText.Text.Trim();
                this.TwitterSearchApiUrl = this.TwitterSearchAPIText.Text.Trim();
                switch (this.ReplyIconStateCombo.SelectedIndex)
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

                switch (this.LanguageCombo.SelectedIndex)
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
                    if (int.TryParse(this.HotkeyCode.Text, out tmp))
                    {
                        this.HotkeyValue = tmp;
                    }
                }

                this.HotkeyKey = (Keys)this.HotkeyText.Tag;
                this.BlinkNewMentions = this.CheckNewMentionsBlink.Checked;
                this.UseAdditionalCount = this.UseChangeGetCount.Checked;
                this.configurations.MoreCountApi = Convert.ToInt32(this.GetMoreTextCountApi.Text);
                this.configurations.FirstCountApi = Convert.ToInt32(this.FirstTextCountApi.Text);
                this.configurations.SearchCountApi = Convert.ToInt32(this.SearchTextCountApi.Text);
                this.configurations.FavoritesCountApi = Convert.ToInt32(this.FavoritesTextCountApi.Text);
                this.configurations.UserTimelineCountApi = Convert.ToInt32(this.UserTimelineTextCountApi.Text);
                this.configurations.ListCountApi = Convert.ToInt32(this.ListTextCountApi.Text);
                this.OpenUserTimeline = this.CheckOpenUserTimeline.Checked;
                this.ListDoubleClickAction = this.ListDoubleClickActionComboBox.SelectedIndex;
                this.UserAppointUrl = this.UserAppointUrlText.Text;
                this.configurations.HideDuplicatedRetweets = this.HideDuplicatedRetweetsCheck.Checked;
                this.configurations.IsPreviewFoursquare = this.IsPreviewFoursquareCheckBox.Checked;
                this.configurations.FoursquarePreviewHeight = Convert.ToInt32(this.FoursquarePreviewHeightTextBox.Text);
                this.configurations.FoursquarePreviewWidth = Convert.ToInt32(this.FoursquarePreviewWidthTextBox.Text);
                this.configurations.FoursquarePreviewZoom = Convert.ToInt32(this.FoursquarePreviewZoomTextBox.Text);
                this.configurations.IsListStatusesIncludeRts = this.IsListsIncludeRtsCheckBox.Checked;
                this.configurations.TabMouseLock = this.TabMouseLockCheck.Checked;
                this.configurations.IsRemoveSameEvent = this.IsRemoveSameFavEventCheckBox.Checked;
                this.configurations.IsNotifyUseGrowl = this.IsNotifyUseGrowlCheckBox.Checked;
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
                this.configurations.UserAccounts.Clear();
                foreach (var u in this.AuthUserCombo.Items)
                {
                    this.configurations.UserAccounts.Add((UserAccount)u);
                }

                // アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (this.initialUserId > 0)
                {
                    foreach (UserAccount u in this.configurations.UserAccounts)
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

            if (!e.Cancel && this.TreeViewSetting.SelectedNode != null)
            {
                Panel curPanel = (Panel)this.TreeViewSetting.SelectedNode.Tag;
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
            if (this.configurations.UserAccounts.Count > 0)
            {
                this.AuthUserCombo.Items.AddRange(this.configurations.UserAccounts.ToArray());
                foreach (UserAccount u in this.configurations.UserAccounts)
                {
                    if (u.UserId == this.tw.UserId)
                    {
                        this.AuthUserCombo.SelectedItem = u;
                        this.initialUserId = u.UserId;
                        break;
                    }
                }
            }

            this.StartupUserstreamCheck.Checked = this.configurations.UserstreamStartup;
            this.UserstreamPeriod.Text = this.configurations.UserstreamPeriodInt.ToString();
            this.TimelinePeriod.Text = this.configurations.TimelinePeriodInt.ToString();
            this.ReplyPeriod.Text = this.configurations.ReplyPeriodInt.ToString();
            this.DMPeriod.Text = this.configurations.DMPeriodInt.ToString();
            this.PubSearchPeriod.Text = this.configurations.PubSearchPeriodInt.ToString();
            this.ListsPeriod.Text = this.configurations.ListsPeriodInt.ToString();
            this.UserTimelinePeriod.Text = this.configurations.UserTimelinePeriodInt.ToString();
            this.StartupReaded.Checked = this.configurations.Readed;

            switch (this.configurations.IconSz)
            {
                case IconSizes.IconNone:
                    this.IconSize.SelectedIndex = 0;
                    break;
                case IconSizes.Icon16:
                    this.IconSize.SelectedIndex = 1;
                    break;
                case IconSizes.Icon24:
                    this.IconSize.SelectedIndex = 2;
                    break;
                case IconSizes.Icon48:
                    this.IconSize.SelectedIndex = 3;
                    break;
                case IconSizes.Icon48_2:
                    this.IconSize.SelectedIndex = 4;
                    break;
            }

            this.StatusText.Text = this.configurations.Status;
            this.UReadMng.Checked = this.configurations.UnreadManage;
            this.StartupReaded.Enabled = this.configurations.UnreadManage != false;
            this.PlaySnd.Checked = this.configurations.PlaySound;
            this.OneWayLv.Checked = this.configurations.OneWayLove;
            this.lblListFont.Font = this.configurations.FontReaded;
            this.lblUnread.Font = this.configurations.FontUnread;
            this.lblUnread.ForeColor = this.configurations.ColorUnread;
            this.lblListFont.ForeColor = this.configurations.ColorReaded;
            this.lblFav.ForeColor = this.configurations.ColorFav;
            this.lblOWL.ForeColor = this.configurations.ColorOWL;
            this.lblRetweet.ForeColor = this.configurations.ColorRetweet;
            this.lblDetail.Font = this.configurations.FontDetail;
            this.lblSelf.BackColor = this.configurations.ColorSelf;
            this.lblAtSelf.BackColor = this.configurations.ColorAtSelf;
            this.lblTarget.BackColor = this.configurations.ColorTarget;
            this.lblAtTarget.BackColor = this.configurations.ColorAtTarget;
            this.lblAtFromTarget.BackColor = this.configurations.ColorAtFromTarget;
            this.lblAtTo.BackColor = this.configurations.ColorAtTo;
            this.lblInputBackcolor.BackColor = this.configurations.ColorInputBackcolor;
            this.lblInputFont.ForeColor = this.configurations.ColorInputFont;
            this.lblInputFont.Font = this.configurations.FontInputFont;
            this.lblListBackcolor.BackColor = this.configurations.ColorListBackcolor;
            this.lblDetailBackcolor.BackColor = this.configurations.ColorDetailBackcolor;
            this.lblDetail.ForeColor = this.configurations.ColorDetail;
            this.lblDetailLink.ForeColor = this.configurations.ColorDetailLink;

            switch (this.configurations.NameBalloon)
            {
                case NameBalloonEnum.None:
                    this.cmbNameBalloon.SelectedIndex = 0;
                    break;
                case NameBalloonEnum.UserID:
                    this.cmbNameBalloon.SelectedIndex = 1;
                    break;
                case NameBalloonEnum.NickName:
                    this.cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            if (this.configurations.PostCtrlEnter)
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (this.configurations.PostShiftEnter)
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            this.TextCountApi.Text = this.configurations.CountApi.ToString();
            this.TextCountApiReply.Text = this.configurations.CountApiReply.ToString();
            this.BrowserPathText.Text = this.configurations.BrowserPath;
            this.CheckPostAndGet.Checked = this.configurations.PostAndGet;
            this.CheckUseRecommendStatus.Checked = this.configurations.UseRecommendStatus;
            this.CheckDispUsername.Checked = this.configurations.DispUsername;
            this.CheckCloseToExit.Checked = this.configurations.CloseToExit;
            this.CheckMinimizeToTray.Checked = this.configurations.MinimizeToTray;
            switch (this.configurations.DispLatestPost)
            {
                case DispTitleEnum.None:
                    this.ComboDispTitle.SelectedIndex = 0;
                    break;
                case DispTitleEnum.Ver:
                    this.ComboDispTitle.SelectedIndex = 1;
                    break;
                case DispTitleEnum.Post:
                    this.ComboDispTitle.SelectedIndex = 2;
                    break;
                case DispTitleEnum.UnreadRepCount:
                    this.ComboDispTitle.SelectedIndex = 3;
                    break;
                case DispTitleEnum.UnreadAllCount:
                    this.ComboDispTitle.SelectedIndex = 4;
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    this.ComboDispTitle.SelectedIndex = 5;
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    this.ComboDispTitle.SelectedIndex = 6;
                    break;
                case DispTitleEnum.OwnStatus:
                    this.ComboDispTitle.SelectedIndex = 7;
                    break;
            }

            this.CheckSortOrderLock.Checked = this.SortOrderLock;
            this.CheckTinyURL.Checked = this.configurations.TinyUrlResolve;
            this.CheckForceResolve.Checked = this.configurations.ShortUrlForceResolve;
            switch (this.SelectedProxyType)
            {
                case HttpConnection.ProxyType.None:
                    this.RadioProxyNone.Checked = true;
                    break;
                case HttpConnection.ProxyType.IE:
                    this.RadioProxyIE.Checked = true;
                    break;
                default:
                    this.RadioProxySpecified.Checked = true;
                    break;
            }

            bool chk = this.RadioProxySpecified.Checked;
            this.LabelProxyAddress.Enabled = chk;
            this.TextProxyAddress.Enabled = chk;
            this.LabelProxyPort.Enabled = chk;
            this.TextProxyPort.Enabled = chk;
            this.LabelProxyUser.Enabled = chk;
            this.TextProxyUser.Enabled = chk;
            this.LabelProxyPassword.Enabled = chk;
            this.TextProxyPassword.Enabled = chk;

            this.TextProxyAddress.Text = this.ProxyAddress;
            this.TextProxyPort.Text = this.ProxyPort.ToString();
            this.TextProxyUser.Text = this.ProxyUser;
            this.TextProxyPassword.Text = this.ProxyPassword;

            this.CheckPeriodAdjust.Checked = this.PeriodAdjust;
            this.CheckStartupVersion.Checked = this.StartupVersion;
            this.CheckStartupFollowers.Checked = this.StartupFollowers;
            this.CheckFavRestrict.Checked = this.RestrictFavCheck;
            this.CheckAlwaysTop.Checked = this.AlwaysTop;
            this.CheckAutoConvertUrl.Checked = this.UrlConvertAuto;
            this.ShortenTcoCheck.Checked = this.ShortenTco;
            this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;
            this.CheckOutputz.Checked = this.OutputzEnabled;
            this.TextBoxOutputzKey.Text = this.OutputzKey;

            switch (this.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    this.ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    this.ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            this.CheckNicoms.Checked = this.Nicoms;
            this.chkUnreadStyle.Checked = this.UseUnreadStyle;
            this.CmbDateTimeFormat.Text = this.DateTimeFormat;
            this.ConnectionTimeOut.Text = this.DefaultTimeOut.ToString();
            this.CheckRetweetNoConfirm.Checked = this.RetweetNoConfirm;
            this.CheckBalloonLimit.Checked = this.LimitBalloon;
            this.ApplyEventNotifyFlag(this.EventNotifyEnabled, this.myEventNotifyFlag, this.isMyEventNotifyFlag);
            this.CheckForceEventNotify.Checked = this.ForceEventNotify;
            this.CheckFavEventUnread.Checked = this.FavEventUnread;
            this.ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(this.myTranslateLanguage);
            this.SoundFileListup();
            this.ComboBoxAutoShortUrlFirst.SelectedIndex = (int)this.AutoShortUrlFirst;
            this.CheckTabIconDisp.Checked = this.TabIconDisp;
            this.chkReadOwnPost.Checked = this.ReadOwnPost;
            this.CheckGetFav.Checked = this.GetFav;
            this.CheckMonospace.Checked = this.IsMonospace;
            this.CheckReadOldPosts.Checked = this.ReadOldPosts;
            this.CheckUseSsl.Checked = this.UseSsl;
            this.TextBitlyId.Text = this.BitlyUser;
            this.TextBitlyPw.Text = this.BitlyPwd;
            this.TextBitlyId.Modified = false;
            this.TextBitlyPw.Modified = false;
            this.CheckShowGrid.Checked = this.ShowGrid;
            this.CheckAtIdSupple.Checked = this.UseAtIdSupplement;
            this.CheckHashSupple.Checked = this.UseHashSupplement;
            this.CheckPreviewEnable.Checked = this.PreviewEnable;
            this.TwitterAPIText.Text = this.TwitterApiUrl;
            this.TwitterSearchAPIText.Text = this.TwitterSearchApiUrl;
            switch (this.ReplyIconState)
            {
                case ReplyIconState.None:
                    this.ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case ReplyIconState.StaticIcon:
                    this.ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case ReplyIconState.BlinkIcon:
                    this.ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }

            switch (this.Language)
            {
                case "OS":
                    this.LanguageCombo.SelectedIndex = 0;
                    break;
                case "ja":
                    this.LanguageCombo.SelectedIndex = 1;
                    break;
                case "en":
                    this.LanguageCombo.SelectedIndex = 2;
                    break;
                case "zh-CN":
                    this.LanguageCombo.SelectedIndex = 3;
                    break;
                default:
                    this.LanguageCombo.SelectedIndex = 0;
                    break;
            }

            this.HotkeyCheck.Checked = this.HotkeyEnabled;
            this.HotkeyAlt.Checked = (this.HotkeyMod & Keys.Alt) == Keys.Alt;
            this.HotkeyCtrl.Checked = (this.HotkeyMod & Keys.Control) == Keys.Control;
            this.HotkeyShift.Checked = (this.HotkeyMod & Keys.Shift) == Keys.Shift;
            this.HotkeyWin.Checked = (this.HotkeyMod & Keys.LWin) == Keys.LWin;
            this.HotkeyCode.Text = this.HotkeyValue.ToString();
            this.HotkeyText.Text = this.HotkeyKey.ToString();
            this.HotkeyText.Tag = this.HotkeyKey;
            this.HotkeyAlt.Enabled = this.HotkeyEnabled;
            this.HotkeyShift.Enabled = this.HotkeyEnabled;
            this.HotkeyCtrl.Enabled = this.HotkeyEnabled;
            this.HotkeyWin.Enabled = this.HotkeyEnabled;
            this.HotkeyText.Enabled = this.HotkeyEnabled;
            this.HotkeyCode.Enabled = this.HotkeyEnabled;
            this.CheckNewMentionsBlink.Checked = this.BlinkNewMentions;

            this.CheckOutputz_CheckedChanged(sender, e);

            this.GetMoreTextCountApi.Text = this.configurations.MoreCountApi.ToString();
            this.FirstTextCountApi.Text = this.configurations.FirstCountApi.ToString();
            this.SearchTextCountApi.Text = this.configurations.SearchCountApi.ToString();
            this.FavoritesTextCountApi.Text = this.configurations.FavoritesCountApi.ToString();
            this.UserTimelineTextCountApi.Text = this.configurations.UserTimelineCountApi.ToString();
            this.ListTextCountApi.Text = this.configurations.ListCountApi.ToString();
            this.UseChangeGetCount.Checked = this.UseAdditionalCount;
            this.Label28.Enabled = this.UseChangeGetCount.Checked;
            this.Label30.Enabled = this.UseChangeGetCount.Checked;
            this.Label53.Enabled = this.UseChangeGetCount.Checked;
            this.Label66.Enabled = this.UseChangeGetCount.Checked;
            this.Label17.Enabled = this.UseChangeGetCount.Checked;
            this.Label25.Enabled = this.UseChangeGetCount.Checked;
            this.GetMoreTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.FirstTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.SearchTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.FavoritesTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.UserTimelineTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.ListTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.CheckOpenUserTimeline.Checked = this.OpenUserTimeline;
            this.ListDoubleClickActionComboBox.SelectedIndex = this.ListDoubleClickAction;
            this.UserAppointUrlText.Text = this.UserAppointUrl;
            this.HideDuplicatedRetweetsCheck.Checked = this.configurations.HideDuplicatedRetweets;
            this.IsPreviewFoursquareCheckBox.Checked = this.configurations.IsPreviewFoursquare;
            this.FoursquarePreviewHeightTextBox.Text = this.configurations.FoursquarePreviewHeight.ToString();
            this.FoursquarePreviewWidthTextBox.Text = this.configurations.FoursquarePreviewWidth.ToString();
            this.FoursquarePreviewZoomTextBox.Text = this.configurations.FoursquarePreviewZoom.ToString();
            this.IsListsIncludeRtsCheckBox.Checked = this.configurations.IsListStatusesIncludeRts;
            this.TabMouseLockCheck.Checked = this.configurations.TabMouseLock;
            this.IsRemoveSameFavEventCheckBox.Checked = this.configurations.IsRemoveSameEvent;
            this.IsNotifyUseGrowlCheckBox.Checked = this.configurations.IsNotifyUseGrowl;

            this.IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;

            this.TreeViewSetting.Nodes["BasedNode"].Tag = this.BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = this.GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = this.StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = this.GetCountPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Tag = this.ActionPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Nodes["TweetActNode"].Tag = this.TweetActPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Tag = this.PreviewPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["TweetPrvNode"].Tag = this.TweetPrvPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["NotifyNode"].Tag = this.NotifyPanel;
            this.TreeViewSetting.Nodes["FontNode"].Tag = this.FontPanel;
            this.TreeViewSetting.Nodes["FontNode"].Nodes["FontNode2"].Tag = this.FontPanel2;
            this.TreeViewSetting.Nodes["ConnectionNode"].Tag = this.ConnectionPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ProxyNode"].Tag = this.ProxyPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["CooperateNode"].Tag = this.CooperatePanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"].Tag = this.ShortUrlPanel;

            this.TreeViewSetting.SelectedNode = this.TreeViewSetting.Nodes[0];
            this.TreeViewSetting.ExpandAll();
            ActiveControl = this.StartAuthButton;
        }

        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd = 0;
            try
            {
                prd = Convert.ToInt32(this.UserstreamPeriod.Text);
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
                prd = Convert.ToInt32(this.TimelinePeriod.Text);
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
                prd = Convert.ToInt32(this.ReplyPeriod.Text);
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
                prd = Convert.ToInt32(this.DMPeriod.Text);
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
                prd = Convert.ToInt32(this.PubSearchPeriod.Text);
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
                prd = Convert.ToInt32(this.ListsPeriod.Text);
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
                prd = Convert.ToInt32(this.UserTimelinePeriod.Text);
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
            this.StartupReaded.Enabled = this.UReadMng.Checked == true;
        }

        private void ButtonFontAndColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            DialogResult rtn = default(DialogResult);

            this.FontDialog1.AllowVerticalFonts = false;
            this.FontDialog1.AllowScriptChange = true;
            this.FontDialog1.AllowSimulations = true;
            this.FontDialog1.AllowVectorFonts = true;
            this.FontDialog1.FixedPitchOnly = false;
            this.FontDialog1.FontMustExist = true;
            this.FontDialog1.ScriptsOnly = false;
            this.FontDialog1.ShowApply = false;
            this.FontDialog1.ShowEffects = true;
            this.FontDialog1.ShowColor = true;

            switch (btn.Name)
            {
                case "btnUnread":
                    this.FontDialog1.Color = this.lblUnread.ForeColor;
                    this.FontDialog1.Font = this.lblUnread.Font;
                    break;
                case "btnDetail":
                    this.FontDialog1.Color = this.lblDetail.ForeColor;
                    this.FontDialog1.Font = this.lblDetail.Font;
                    break;
                case "btnListFont":
                    this.FontDialog1.Color = this.lblListFont.ForeColor;
                    this.FontDialog1.Font = this.lblListFont.Font;
                    break;
                case "btnInputFont":
                    this.FontDialog1.Color = this.lblInputFont.ForeColor;
                    this.FontDialog1.Font = this.lblInputFont.Font;
                    break;
            }

            try
            {
                rtn = this.FontDialog1.ShowDialog();
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
                    this.lblUnread.ForeColor = this.FontDialog1.Color;
                    this.lblUnread.Font = this.FontDialog1.Font;
                    break;
                case "btnDetail":
                    this.lblDetail.ForeColor = this.FontDialog1.Color;
                    this.lblDetail.Font = this.FontDialog1.Font;
                    break;
                case "btnListFont":
                    this.lblListFont.ForeColor = this.FontDialog1.Color;
                    this.lblListFont.Font = this.FontDialog1.Font;
                    break;
                case "btnInputFont":
                    this.lblInputFont.ForeColor = this.FontDialog1.Color;
                    this.lblInputFont.Font = this.FontDialog1.Font;
                    break;
            }
        }

        private void ButtonColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            DialogResult rtn = default(DialogResult);

            this.ColorDialog1.AllowFullOpen = true;
            this.ColorDialog1.AnyColor = true;
            this.ColorDialog1.FullOpen = false;
            this.ColorDialog1.SolidColorOnly = false;

            switch (btn.Name)
            {
                case "btnSelf":
                    this.ColorDialog1.Color = this.lblSelf.BackColor;
                    break;
                case "btnAtSelf":
                    this.ColorDialog1.Color = this.lblAtSelf.BackColor;
                    break;
                case "btnTarget":
                    this.ColorDialog1.Color = this.lblTarget.BackColor;
                    break;
                case "btnAtTarget":
                    this.ColorDialog1.Color = this.lblAtTarget.BackColor;
                    break;
                case "btnAtFromTarget":
                    this.ColorDialog1.Color = this.lblAtFromTarget.BackColor;
                    break;
                case "btnFav":
                    this.ColorDialog1.Color = this.lblFav.ForeColor;
                    break;
                case "btnOWL":
                    this.ColorDialog1.Color = this.lblOWL.ForeColor;
                    break;
                case "btnRetweet":
                    this.ColorDialog1.Color = this.lblRetweet.ForeColor;
                    break;
                case "btnInputBackcolor":
                    this.ColorDialog1.Color = this.lblInputBackcolor.BackColor;
                    break;
                case "btnAtTo":
                    this.ColorDialog1.Color = this.lblAtTo.BackColor;
                    break;
                case "btnListBack":
                    this.ColorDialog1.Color = this.lblListBackcolor.BackColor;
                    break;
                case "btnDetailBack":
                    this.ColorDialog1.Color = this.lblDetailBackcolor.BackColor;
                    break;
                case "btnDetailLink":
                    this.ColorDialog1.Color = this.lblDetailLink.ForeColor;
                    break;
            }

            rtn = this.ColorDialog1.ShowDialog();
            if (rtn == DialogResult.Cancel)
            {
                return;
            }

            switch (btn.Name)
            {
                case "btnSelf":
                    this.lblSelf.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnAtSelf":
                    this.lblAtSelf.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnTarget":
                    this.lblTarget.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnAtTarget":
                    this.lblAtTarget.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnAtFromTarget":
                    this.lblAtFromTarget.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnFav":
                    this.lblFav.ForeColor = this.ColorDialog1.Color;
                    break;
                case "btnOWL":
                    this.lblOWL.ForeColor = this.ColorDialog1.Color;
                    break;
                case "btnRetweet":
                    this.lblRetweet.ForeColor = this.ColorDialog1.Color;
                    break;
                case "btnInputBackcolor":
                    this.lblInputBackcolor.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnAtTo":
                    this.lblAtTo.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnListBack":
                    this.lblListBackcolor.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnDetailBack":
                    this.lblDetailBackcolor.BackColor = this.ColorDialog1.Color;
                    break;
                case "btnDetailLink":
                    this.lblDetailLink.ForeColor = this.ColorDialog1.Color;
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
                    this.BrowserPathText.Text = filedlg.FileName;
                }
            }
        }

        private void RadioProxySpecified_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = this.RadioProxySpecified.Checked;
            this.LabelProxyAddress.Enabled = chk;
            this.TextProxyAddress.Enabled = chk;
            this.LabelProxyPort.Enabled = chk;
            this.TextProxyPort.Enabled = chk;
            this.LabelProxyUser.Enabled = chk;
            this.TextProxyUser.Enabled = chk;
            this.LabelProxyPassword.Enabled = chk;
            this.TextProxyPassword.Enabled = chk;
        }

        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            int port = 0;
            if (string.IsNullOrEmpty(this.TextProxyPort.Text.Trim()))
            {
                this.TextProxyPort.Text = "0";
            }

            if (!int.TryParse(this.TextProxyPort.Text.Trim(), out port))
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
            if (this.CheckOutputz.Checked == true)
            {
                this.Label59.Enabled = true;
                this.Label60.Enabled = true;
                this.TextBoxOutputzKey.Enabled = true;
                this.ComboBoxOutputzUrlmode.Enabled = true;
            }
            else
            {
                this.Label59.Enabled = false;
                this.Label60.Enabled = false;
                this.TextBoxOutputzKey.Enabled = false;
                this.ComboBoxOutputzUrlmode.Enabled = false;
            }
        }

        private void TextBoxOutputzKey_Validating(object sender, CancelEventArgs e)
        {
            if (this.CheckOutputz.Checked)
            {
                this.TextBoxOutputzKey.Text = this.TextBoxOutputzKey.Text.Trim();
                if (this.TextBoxOutputzKey.Text.Length == 0)
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
                tm = Convert.ToInt32(this.ConnectionTimeOut.Text);
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
                cnt = int.Parse(this.TextCountApi.Text);
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
                cnt = int.Parse(this.TextCountApiReply.Text);
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
            if (this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Bitly
                || this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Jmp)
            {
                this.Label76.Enabled = true;
                this.Label77.Enabled = true;
                this.TextBitlyId.Enabled = true;
                this.TextBitlyPw.Enabled = true;
            }
            else
            {
                this.Label76.Enabled = false;
                this.Label77.Enabled = false;
                this.TextBitlyId.Enabled = false;
                this.TextBitlyPw.Enabled = false;
            }
        }

        private void ButtonBackToDefaultFontColor_Click(object sender, EventArgs e)
        {
            this.lblUnread.ForeColor = SystemColors.ControlText;
            this.lblUnread.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            this.lblListFont.ForeColor = SystemColors.ControlText;
            this.lblListFont.Font = SystemFonts.DefaultFont;

            this.lblDetail.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            this.lblDetail.Font = SystemFonts.DefaultFont;

            this.lblInputFont.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            this.lblInputFont.Font = SystemFonts.DefaultFont;

            this.lblSelf.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);

            this.lblAtSelf.BackColor = Color.FromKnownColor(KnownColor.AntiqueWhite);

            this.lblTarget.BackColor = Color.FromKnownColor(KnownColor.LemonChiffon);

            this.lblAtTarget.BackColor = Color.FromKnownColor(KnownColor.LavenderBlush);

            this.lblAtFromTarget.BackColor = Color.FromKnownColor(KnownColor.Honeydew);

            this.lblFav.ForeColor = Color.FromKnownColor(KnownColor.Red);

            this.lblOWL.ForeColor = Color.FromKnownColor(KnownColor.Blue);

            this.lblInputBackcolor.BackColor = Color.FromKnownColor(KnownColor.LemonChiffon);

            this.lblAtTo.BackColor = Color.FromKnownColor(KnownColor.Pink);

            this.lblListBackcolor.BackColor = Color.FromKnownColor(KnownColor.Window);

            this.lblDetailBackcolor.BackColor = Color.FromKnownColor(KnownColor.Window);

            this.lblDetailLink.ForeColor = Color.FromKnownColor(KnownColor.Blue);

            this.lblRetweet.ForeColor = Color.FromKnownColor(KnownColor.Green);
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
            this.HotkeyText.Text = e.KeyCode.ToString();
            this.HotkeyCode.Text = e.KeyValue.ToString();
            this.HotkeyText.Tag = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void HotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = this.HotkeyCheck.Checked;
            this.HotkeyCtrl.Enabled = chk;
            this.HotkeyAlt.Enabled = chk;
            this.HotkeyShift.Enabled = chk;
            this.HotkeyWin.Enabled = chk;
            this.HotkeyText.Enabled = chk;
            this.HotkeyCode.Enabled = chk;
        }

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(this.GetMoreTextCountApi.Text);
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
            bool check = this.UseChangeGetCount.Checked;
            this.GetMoreTextCountApi.Enabled = check;
            this.FirstTextCountApi.Enabled = check;
            this.Label28.Enabled = check;
            this.Label30.Enabled = check;
            this.Label53.Enabled = check;
            this.Label66.Enabled = check;
            this.Label17.Enabled = check;
            this.Label25.Enabled = check;
            this.SearchTextCountApi.Enabled = check;
            this.FavoritesTextCountApi.Enabled = check;
            this.UserTimelineTextCountApi.Enabled = check;
            this.ListTextCountApi.Enabled = check;
        }

        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(this.FirstTextCountApi.Text);
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
                cnt = int.Parse(this.SearchTextCountApi.Text);
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
                cnt = int.Parse(this.FavoritesTextCountApi.Text);
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
                cnt = int.Parse(this.UserTimelineTextCountApi.Text);
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
                cnt = int.Parse(this.ListTextCountApi.Text);
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
                tbl.CheckBox.Enabled = this.CheckEventNotify.Checked;
            }
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UserAppointUrlText.Text)
                && !this.UserAppointUrlText.Text.StartsWith("http"))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }

        private void IsPreviewFoursquareCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.FoursquareGroupBox.Enabled = this.IsPreviewFoursquareCheckBox.Checked;
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;
        }

        #endregion event handler

        #region private methods

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                this.LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(this.CmbDateTimeFormat.Text);
            }
            catch (FormatException)
            {
                this.LabelDateTimeFormatApplied.Text = Hoehoe.Properties.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }

            return true;
        }

        private bool StartAuth()
        {
            // 現在の設定内容で通信
            HttpConnection.ProxyType ptype = default(HttpConnection.ProxyType);
            if (this.RadioProxyNone.Checked)
            {
                ptype = HttpConnection.ProxyType.None;
            }
            else if (this.RadioProxyIE.Checked)
            {
                ptype = HttpConnection.ProxyType.IE;
            }
            else
            {
                ptype = HttpConnection.ProxyType.Specified;
            }

            string padr = this.TextProxyAddress.Text.Trim();
            int pport = int.Parse(this.TextProxyPort.Text.Trim());
            string pusr = this.TextProxyUser.Text.Trim();
            string ppw = this.TextProxyPassword.Text.Trim();

            // 通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
            HttpTwitter.SetTwitterUrl(this.TwitterAPIText.Text.Trim());
            HttpTwitter.SetTwitterSearchUrl(this.TwitterSearchAPIText.Text.Trim());
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
            string v = (MyCommon.TwitterApiInfo.MaxCount > -1) ? MyCommon.TwitterApiInfo.MaxCount.ToString() : "???";
            this.LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, v);
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
            if (int.TryParse(this.TimelinePeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += 3600 / tmp;
                }
            }

            // Reply計算 0は手動更新
            if (int.TryParse(this.ReplyPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += 3600 / tmp;
                }
            }

            // DM計算 0は手動更新 送受信両方
            if (int.TryParse(this.DMPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    usingApi += (3600 / tmp) * 2;
                }
            }

            // Listsタブ計算 0は手動更新
            int apiLists = 0;
            if (int.TryParse(this.ListsPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    apiLists = (3600 / tmp) * listsTabNum;
                    usingApi += apiLists;
                }
            }

            // UserTimelineタブ計算 0は手動更新
            int apiUserTimeline = 0;
            if (int.TryParse(this.UserTimelinePeriod.Text, out tmp))
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
                        var proc = new Thread(new ThreadStart(() =>
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
                        this.LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, usingApi, "???");
                    }
                }
                else
                {
                    this.LabelApiUsing.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse1, usingApi, MyCommon.TwitterApiInfo.MaxCount);
                }
            }

            this.LabelPostAndGet.Visible = this.CheckPostAndGet.Checked && !this.tw.UserStreamEnabled;
            this.LabelUserStreamActive.Visible = this.tw.UserStreamEnabled;

            this.LabelApiUsingUserStreamEnabled.Text = string.Format(Hoehoe.Properties.Resources.SettingAPIUse2, apiLists + apiUserTimeline);
            this.LabelApiUsingUserStreamEnabled.Visible = this.tw.UserStreamEnabled;
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
                    new EventCheckboxTblElement(this.CheckFavoritesEvent, EventType.Favorite),
                    new EventCheckboxTblElement(this.CheckUnfavoritesEvent, EventType.Unfavorite),
                    new EventCheckboxTblElement(this.CheckFollowEvent, EventType.Follow),
                    new EventCheckboxTblElement(this.CheckListMemberAddedEvent, EventType.ListMemberAdded),
                    new EventCheckboxTblElement(this.CheckListMemberRemovedEvent, EventType.ListMemberRemoved),
                    new EventCheckboxTblElement(this.CheckBlockEvent, EventType.Block),
                    new EventCheckboxTblElement(this.CheckUserUpdateEvent, EventType.UserUpdate),
                    new EventCheckboxTblElement(this.CheckListCreatedEvent, EventType.ListCreated)
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
                        evt = evt | tbl.EventType;
                        myevt = myevt | tbl.EventType;
                        break;
                    case CheckState.Indeterminate:
                        evt = evt | tbl.EventType;
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

            this.CheckEventNotify.Checked = rootEnabled;

            foreach (EventCheckboxTblElement tbl in this.GetEventCheckboxTable())
            {
                if (Convert.ToBoolean(evt & tbl.EventType))
                {
                    if (Convert.ToBoolean(myevt & tbl.EventType))
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

            this.ComboBoxEventNotifySound.Items.Clear();
            this.ComboBoxEventNotifySound.Items.Add(string.Empty);
            var names = MyCommon.GetSoundFileNames();
            if (names.Length > 0)
            {
                this.ComboBoxEventNotifySound.Items.AddRange(names);
            }

            int idx = this.ComboBoxEventNotifySound.Items.IndexOf(this.EventSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            this.ComboBoxEventNotifySound.SelectedIndex = idx;
        }

        private void OpenUrl(string url)
        {
            string myPath = url;
            string path = this.BrowserPathText.Text;
            try
            {
                if (string.IsNullOrEmpty(this.configurations.BrowserPath))
                {
                    Process.Start(myPath);
                }
                else
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
            }
            catch (Exception)
            {
            }
        }

        #endregion private methods

        #region inner class

        private class EventCheckboxTblElement
        {
            public EventCheckboxTblElement(CheckBox cb, EventType et)
            {
                this.CheckBox = cb;
                this.EventType = et;
            }

            public CheckBox CheckBox { get; private set; }

            public EventType EventType { get; private set; }
        }

        #endregion inner class
    }
}