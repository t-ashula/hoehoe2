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
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using R = Properties.Resources;

    public partial class AppendSettingDialog
    {
        #region privates

        private Twitter _tw;
        private bool _validationError;
        private long _initialUserId;
        private string _pin;
        private EventCheckboxTblElement[] _eventCheckboxTableElements;
        private readonly Configs _configurations = Configs.Instance;
        private PeriodValidator _apiCountValidator;
        private PeriodValidator _searchApiCountValidator;
        private PeriodValidator _userStreamPeriodValidator;
        private PeriodValidator _timelilePeriodValidator;
        private PeriodValidator _dmessagesPeriodValidator;
        private PeriodValidator _publicSearchPeriodValidator;
        private PeriodValidator _proxyPortNumberValidator;
        private PeriodValidator _timeoutLimitsValidator;

        #endregion privates

        #region constructor

        public AppendSettingDialog()
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            Icon = R.MIcon;
            InitEventCheckboxTable();
            InitPeriodValidators();
        }

        #endregion constructor

        #region delegates

        public delegate void IntervalChangedEventHandler(object sender, IntervalChangedEventArgs e);

        #endregion delegates

        #region events

        public event IntervalChangedEventHandler IntervalChanged;

        #endregion events

        #region event handler

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            StatusText.Enabled = !CheckUseRecommendStatus.Checked;
        }

        private void TreeViewSetting_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (TreeViewSetting.SelectedNode == null)
            {
                return;
            }

            var pnl = (Panel)TreeViewSetting.SelectedNode.Tag;
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
                if (!BitlyValidation(TextBitlyId.Text, TextBitlyPw.Text))
                {
                    MessageBox.Show(R.SettingSave_ClickText1);
                    _validationError = true;
                    TreeViewSetting.SelectedNode.Name = "TweetActNode"; // 動作タブを選択
                    TextBitlyId.Focus();
                    return;
                }
            }

            _validationError = false;
            _configurations.UserAccounts.Clear();
            _configurations.UserAccounts.AddRange(AuthUserCombo.Items.Cast<UserAccount>());
            if (AuthUserCombo.SelectedIndex < 0)
            {
                _tw.ClearAuthInfo();
                _tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            }
            else
            {
                string selectedUser = ((UserAccount)AuthUserCombo.SelectedItem).Username.ToLower();
                var newuser = _configurations.UserAccounts.First(u => u.Username.ToLower() == selectedUser);
                _tw.Initialize(newuser.Token, newuser.TokenSecret, newuser.Username, newuser.UserId);
                if (newuser.UserId == 0)
                {
                    _tw.VerifyCredentials();
                    newuser.UserId = _tw.UserId;
                }
            }

            try
            {
                _configurations.UserstreamStartup = StartupUserstreamCheck.Checked;

                var arg = new IntervalChangedEventArgs();
                bool isIntervalChanged = SaveIntarvals(arg);
                if (isIntervalChanged)
                {
                    if (IntervalChanged != null)
                    {
                        IntervalChanged(this, arg);
                    }
                }

                _configurations.Readed = StartupReaded.Checked;
                _configurations.IconSz = IconSize.SelectedIndex < 0 ? IconSizes.IconNone : (IconSizes)IconSize.SelectedIndex;
                _configurations.Status = StatusText.Text;
                _configurations.PlaySound = PlaySnd.Checked;
                _configurations.UnreadManage = UReadMng.Checked;
                _configurations.OneWayLove = OneWayLv.Checked;
                _configurations.FontUnread = lblUnread.Font;                // 未使用
                _configurations.ColorUnread = lblUnread.ForeColor;
                _configurations.FontReaded = lblListFont.Font;              // リストフォントとして使用
                _configurations.ColorReaded = lblListFont.ForeColor;
                _configurations.ColorFav = lblFav.ForeColor;
                _configurations.ColorOWL = lblOWL.ForeColor;
                _configurations.ColorRetweet = lblRetweet.ForeColor;
                _configurations.FontDetail = lblDetail.Font;
                _configurations.ColorSelf = lblSelf.BackColor;
                _configurations.ColorAtSelf = lblAtSelf.BackColor;
                _configurations.ColorTarget = lblTarget.BackColor;
                _configurations.ColorAtTarget = lblAtTarget.BackColor;
                _configurations.ColorAtFromTarget = lblAtFromTarget.BackColor;
                _configurations.ColorAtTo = lblAtTo.BackColor;
                _configurations.ColorInputBackcolor = lblInputBackcolor.BackColor;
                _configurations.ColorInputFont = lblInputFont.ForeColor;
                _configurations.ColorListBackcolor = lblListBackcolor.BackColor;
                _configurations.ColorDetailBackcolor = lblDetailBackcolor.BackColor;
                _configurations.ColorDetail = lblDetail.ForeColor;
                _configurations.ColorDetailLink = lblDetailLink.ForeColor;
                _configurations.FontInputFont = lblInputFont.Font;
                switch (cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        _configurations.NameBalloon = NameBalloonEnum.None;
                        break;

                    case 1:
                        _configurations.NameBalloon = NameBalloonEnum.UserID;
                        break;

                    case 2:
                        _configurations.NameBalloon = NameBalloonEnum.NickName;
                        break;
                }

                switch (ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        _configurations.PostShiftEnter = true;
                        _configurations.PostCtrlEnter = false;
                        break;

                    case 1:
                        _configurations.PostCtrlEnter = true;
                        _configurations.PostShiftEnter = false;
                        break;

                    case 0:
                        _configurations.PostCtrlEnter = false;
                        _configurations.PostShiftEnter = false;
                        break;
                }

                _configurations.CountApi = Convert.ToInt32(TextCountApi.Text);
                _configurations.CountApiReply = Convert.ToInt32(TextCountApiReply.Text);
                _configurations.BrowserPath = BrowserPathText.Text.Trim();
                _configurations.PostAndGet = CheckPostAndGet.Checked;
                _configurations.UseRecommendStatus = CheckUseRecommendStatus.Checked;
                _configurations.DispUsername = CheckDispUsername.Checked;
                _configurations.CloseToExit = CheckCloseToExit.Checked;
                _configurations.MinimizeToTray = CheckMinimizeToTray.Checked;
                switch (ComboDispTitle.SelectedIndex)
                {
                    case 0:

                        // None
                        _configurations.DispLatestPost = DispTitleEnum.None;
                        break;

                    case 1:

                        // Ver
                        _configurations.DispLatestPost = DispTitleEnum.Ver;
                        break;

                    case 2:

                        // Post
                        _configurations.DispLatestPost = DispTitleEnum.Post;
                        break;

                    case 3:

                        // RepCount
                        _configurations.DispLatestPost = DispTitleEnum.UnreadRepCount;
                        break;

                    case 4:

                        // AllCount
                        _configurations.DispLatestPost = DispTitleEnum.UnreadAllCount;
                        break;

                    case 5:

                        // Rep+All
                        _configurations.DispLatestPost = DispTitleEnum.UnreadAllRepCount;
                        break;

                    case 6:

                        // Unread/All
                        _configurations.DispLatestPost = DispTitleEnum.UnreadCountAllCount;
                        break;

                    case 7:

                        // Count of Status/Follow/Follower
                        _configurations.DispLatestPost = DispTitleEnum.OwnStatus;
                        break;
                }

                _configurations.SortOrderLock = CheckSortOrderLock.Checked;
                _configurations.TinyUrlResolve = CheckTinyURL.Checked;
                _configurations.ShortUrlForceResolve = CheckForceResolve.Checked;
                ShortUrl.IsResolve = _configurations.TinyUrlResolve;
                ShortUrl.IsForceResolve = _configurations.ShortUrlForceResolve;
                if (RadioProxyNone.Checked)
                {
                    _configurations.SelectedProxyType = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    _configurations.SelectedProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    _configurations.SelectedProxyType = HttpConnection.ProxyType.Specified;
                }

                _configurations.ProxyAddress = TextProxyAddress.Text.Trim();
                _configurations.ProxyPort = int.Parse(TextProxyPort.Text.Trim());
                _configurations.ProxyUser = TextProxyUser.Text.Trim();
                _configurations.ProxyPassword = TextProxyPassword.Text.Trim();
                _configurations.PeriodAdjust = CheckPeriodAdjust.Checked;
                _configurations.StartupVersion = CheckStartupVersion.Checked;
                _configurations.StartupFollowers = CheckStartupFollowers.Checked;
                _configurations.RestrictFavCheck = CheckFavRestrict.Checked;
                _configurations.AlwaysTop = CheckAlwaysTop.Checked;
                _configurations.UrlConvertAuto = CheckAutoConvertUrl.Checked;
                _configurations.ShortenTco = ShortenTcoCheck.Checked;
                _configurations.OutputzEnabled = CheckOutputz.Checked;
                _configurations.OutputzKey = TextBoxOutputzKey.Text.Trim();

                switch (ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        _configurations.OutputzUrlmode = OutputzUrlmode.twittercom;
                        break;

                    case 1:
                        _configurations.OutputzUrlmode = OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                _configurations.Nicoms = CheckNicoms.Checked;
                _configurations.UseUnreadStyle = chkUnreadStyle.Checked;
                _configurations.DateTimeFormat = CmbDateTimeFormat.Text;
                _configurations.DefaultTimeOut = Convert.ToInt32(ConnectionTimeOut.Text);
                _configurations.RetweetNoConfirm = CheckRetweetNoConfirm.Checked;
                _configurations.LimitBalloon = CheckBalloonLimit.Checked;
                _configurations.EventNotifyEnabled = CheckEventNotify.Checked;
                {
                    var m = _configurations.EventNotifyFlag;
                    var i = _configurations.IsMyEventNotifyFlag;
                    GetEventNotifyFlag(ref m, ref i);
                    _configurations.EventNotifyFlag = m;
                    _configurations.IsMyEventNotifyFlag = i;
                }

                _configurations.ForceEventNotify = CheckForceEventNotify.Checked;
                _configurations.FavEventUnread = CheckFavEventUnread.Checked;
                _configurations.TranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
                _configurations.EventSoundFile = Convert.ToString(ComboBoxEventNotifySound.SelectedItem);
                _configurations.AutoShortUrlFirst = (UrlConverter)ComboBoxAutoShortUrlFirst.SelectedIndex;
                _configurations.TabIconDisp = CheckTabIconDisp.Checked;
                _configurations.ReadOwnPost = chkReadOwnPost.Checked;
                _configurations.GetFav = CheckGetFav.Checked;
                _configurations.IsMonospace = CheckMonospace.Checked;
                _configurations.ReadOldPosts = CheckReadOldPosts.Checked;
                _configurations.UseSsl = CheckUseSsl.Checked;
                _configurations.BitlyUser = TextBitlyId.Text;
                _configurations.BitlyPwd = TextBitlyPw.Text;
                _configurations.ShowGrid = CheckShowGrid.Checked;
                _configurations.UseAtIdSupplement = CheckAtIdSupple.Checked;
                _configurations.UseHashSupplement = CheckHashSupple.Checked;
                _configurations.PreviewEnable = CheckPreviewEnable.Checked;
                _configurations.TwitterApiUrl = TwitterAPIText.Text.Trim();
                _configurations.TwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim();
                switch (ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        _configurations.ReplyIconState = ReplyIconState.None;
                        break;

                    case 1:
                        _configurations.ReplyIconState = ReplyIconState.StaticIcon;
                        break;

                    case 2:
                        _configurations.ReplyIconState = ReplyIconState.BlinkIcon;
                        break;
                }

                var lngs = new[] { "OS", "ja", "en", "zh-CN" };
                _configurations.Language = LanguageCombo.SelectedIndex < 0
                    || LanguageCombo.SelectedIndex > lngs.Length ? "en" : lngs[LanguageCombo.SelectedIndex];

                _configurations.HotkeyEnabled = HotkeyCheck.Checked;
                _configurations.HotkeyMod = Keys.None;
                if (HotkeyAlt.Checked)
                {
                    _configurations.HotkeyMod = _configurations.HotkeyMod | Keys.Alt;
                }

                if (HotkeyShift.Checked)
                {
                    _configurations.HotkeyMod = _configurations.HotkeyMod | Keys.Shift;
                }

                if (HotkeyCtrl.Checked)
                {
                    _configurations.HotkeyMod = _configurations.HotkeyMod | Keys.Control;
                }

                if (HotkeyWin.Checked)
                {
                    _configurations.HotkeyMod = _configurations.HotkeyMod | Keys.LWin;
                }

                {
                    int tmp;
                    if (int.TryParse(HotkeyCode.Text, out tmp))
                    {
                        _configurations.HotkeyValue = tmp;
                    }
                }

                _configurations.HotkeyKey = (Keys)HotkeyText.Tag;
                _configurations.BlinkNewMentions = CheckNewMentionsBlink.Checked;
                _configurations.UseAdditionalCount = UseChangeGetCount.Checked;
                _configurations.MoreCountApi = Convert.ToInt32(GetMoreTextCountApi.Text);
                _configurations.FirstCountApi = Convert.ToInt32(FirstTextCountApi.Text);
                _configurations.SearchCountApi = Convert.ToInt32(SearchTextCountApi.Text);
                _configurations.FavoritesCountApi = Convert.ToInt32(FavoritesTextCountApi.Text);
                _configurations.UserTimelineCountApi = Convert.ToInt32(UserTimelineTextCountApi.Text);
                _configurations.ListCountApi = Convert.ToInt32(ListTextCountApi.Text);
                _configurations.OpenUserTimeline = CheckOpenUserTimeline.Checked;
                _configurations.ListDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex;
                _configurations.UserAppointUrl = UserAppointUrlText.Text;
                _configurations.HideDuplicatedRetweets = HideDuplicatedRetweetsCheck.Checked;
                _configurations.IsPreviewFoursquare = IsPreviewFoursquareCheckBox.Checked;
                _configurations.FoursquarePreviewHeight = Convert.ToInt32(FoursquarePreviewHeightTextBox.Text);
                _configurations.FoursquarePreviewWidth = Convert.ToInt32(FoursquarePreviewWidthTextBox.Text);
                _configurations.FoursquarePreviewZoom = Convert.ToInt32(FoursquarePreviewZoomTextBox.Text);
                _configurations.IsListStatusesIncludeRts = IsListsIncludeRtsCheckBox.Checked;
                _configurations.TabMouseLock = TabMouseLockCheck.Checked;
                _configurations.IsRemoveSameEvent = IsRemoveSameFavEventCheckBox.Checked;
                _configurations.IsNotifyUseGrowl = IsNotifyUseGrowlCheckBox.Checked;
            }
            catch (Exception)
            {
                MessageBox.Show(R.Save_ClickText3);
                DialogResult = DialogResult.Cancel;
            }
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            if (DialogResult == DialogResult.Cancel)
            {
                // キャンセル時は画面表示時のアカウントに戻す
                // キャンセル時でも認証済みアカウント情報は保存する
                _configurations.UserAccounts.Clear();
                foreach (var u in AuthUserCombo.Items)
                {
                    _configurations.UserAccounts.Add((UserAccount)u);
                }

                // アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (_initialUserId > 0)
                {
                    foreach (var u in _configurations.UserAccounts)
                    {
                        if (u.UserId == _initialUserId)
                        {
                            _tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                            userSet = true;
                            break;
                        }
                    }
                }

                // 認証済みアカウントが削除されていた場合、もしくは起動時アカウントがなかった場合は、アクティブユーザーなしとして初期化
                if (!userSet)
                {
                    _tw.ClearAuthInfo();
                    _tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
                }
            }

            if (_tw != null && string.IsNullOrEmpty(_tw.Username) && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(R.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            if (_validationError)
            {
                e.Cancel = true;
            }

            if (!e.Cancel && TreeViewSetting.SelectedNode != null)
            {
                var curPanel = (Panel)TreeViewSetting.SelectedNode.Tag;
                curPanel.Visible = false;
                curPanel.Enabled = false;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            GroupBox2.Visible = false;
            _tw = ((TweenMain)Owner).TwitterInstance;
            AuthClearButton.Enabled = true;

            AuthUserCombo.Items.Clear();
            if (_configurations.UserAccounts.Count > 0)
            {
                AuthUserCombo.Items.AddRange(_configurations.UserAccounts.ToArray());
                foreach (UserAccount u in _configurations.UserAccounts)
                {
                    if (u.UserId == _tw.UserId)
                    {
                        AuthUserCombo.SelectedItem = u;
                        _initialUserId = u.UserId;
                        break;
                    }
                }
            }

            StartupUserstreamCheck.Checked = _configurations.UserstreamStartup;
            UserstreamPeriod.Text = _configurations.UserstreamPeriodInt.ToString();
            TimelinePeriod.Text = _configurations.TimelinePeriodInt.ToString();
            ReplyPeriod.Text = _configurations.ReplyPeriodInt.ToString();
            DMPeriod.Text = _configurations.DMPeriodInt.ToString();
            PubSearchPeriod.Text = _configurations.PubSearchPeriodInt.ToString();
            ListsPeriod.Text = _configurations.ListsPeriodInt.ToString();
            UserTimelinePeriod.Text = _configurations.UserTimelinePeriodInt.ToString();
            StartupReaded.Checked = _configurations.Readed;

            switch (_configurations.IconSz)
            {
                case IconSizes.IconNone:
                    IconSize.SelectedIndex = 0;
                    break;

                case IconSizes.Icon16:
                    IconSize.SelectedIndex = 1;
                    break;

                case IconSizes.Icon24:
                    IconSize.SelectedIndex = 2;
                    break;

                case IconSizes.Icon48:
                    IconSize.SelectedIndex = 3;
                    break;

                case IconSizes.Icon48_2:
                    IconSize.SelectedIndex = 4;
                    break;
            }

            StatusText.Text = _configurations.Status;
            UReadMng.Checked = _configurations.UnreadManage;
            StartupReaded.Enabled = _configurations.UnreadManage;
            PlaySnd.Checked = _configurations.PlaySound;
            OneWayLv.Checked = _configurations.OneWayLove;
            lblListFont.Font = _configurations.FontReaded;
            lblUnread.Font = _configurations.FontUnread;
            lblUnread.ForeColor = _configurations.ColorUnread;
            lblListFont.ForeColor = _configurations.ColorReaded;
            lblFav.ForeColor = _configurations.ColorFav;
            lblOWL.ForeColor = _configurations.ColorOWL;
            lblRetweet.ForeColor = _configurations.ColorRetweet;
            lblDetail.Font = _configurations.FontDetail;
            lblSelf.BackColor = _configurations.ColorSelf;
            lblAtSelf.BackColor = _configurations.ColorAtSelf;
            lblTarget.BackColor = _configurations.ColorTarget;
            lblAtTarget.BackColor = _configurations.ColorAtTarget;
            lblAtFromTarget.BackColor = _configurations.ColorAtFromTarget;
            lblAtTo.BackColor = _configurations.ColorAtTo;
            lblInputBackcolor.BackColor = _configurations.ColorInputBackcolor;
            lblInputFont.ForeColor = _configurations.ColorInputFont;
            lblInputFont.Font = _configurations.FontInputFont;
            lblListBackcolor.BackColor = _configurations.ColorListBackcolor;
            lblDetailBackcolor.BackColor = _configurations.ColorDetailBackcolor;
            lblDetail.ForeColor = _configurations.ColorDetail;
            lblDetailLink.ForeColor = _configurations.ColorDetailLink;

            switch (_configurations.NameBalloon)
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

            if (_configurations.PostCtrlEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (_configurations.PostShiftEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            TextCountApi.Text = _configurations.CountApi.ToString();
            TextCountApiReply.Text = _configurations.CountApiReply.ToString();
            BrowserPathText.Text = _configurations.BrowserPath;
            CheckPostAndGet.Checked = _configurations.PostAndGet;
            CheckUseRecommendStatus.Checked = _configurations.UseRecommendStatus;
            CheckDispUsername.Checked = _configurations.DispUsername;
            CheckCloseToExit.Checked = _configurations.CloseToExit;
            CheckMinimizeToTray.Checked = _configurations.MinimizeToTray;
            switch (_configurations.DispLatestPost)
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

            CheckSortOrderLock.Checked = _configurations.SortOrderLock;
            CheckTinyURL.Checked = _configurations.TinyUrlResolve;
            CheckForceResolve.Checked = _configurations.ShortUrlForceResolve;
            switch (_configurations.SelectedProxyType)
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

            ChangeProxySettingControlsStatus(RadioProxySpecified.Checked);

            TextProxyAddress.Text = _configurations.ProxyAddress;
            TextProxyPort.Text = _configurations.ProxyPort.ToString();
            TextProxyUser.Text = _configurations.ProxyUser;
            TextProxyPassword.Text = _configurations.ProxyPassword;

            CheckPeriodAdjust.Checked = _configurations.PeriodAdjust;
            CheckStartupVersion.Checked = _configurations.StartupVersion;
            CheckStartupFollowers.Checked = _configurations.StartupFollowers;
            CheckFavRestrict.Checked = _configurations.RestrictFavCheck;
            CheckAlwaysTop.Checked = _configurations.AlwaysTop;
            CheckAutoConvertUrl.Checked = _configurations.UrlConvertAuto;
            ShortenTcoCheck.Checked = _configurations.ShortenTco;
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
            CheckOutputz.Checked = _configurations.OutputzEnabled;
            ChangeOutputzControlsStatus(CheckOutputz.Checked);
            TextBoxOutputzKey.Text = _configurations.OutputzKey;
            switch (_configurations.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;

                case OutputzUrlmode.twittercomWithUsername:
                    ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            CheckNicoms.Checked = _configurations.Nicoms;
            chkUnreadStyle.Checked = _configurations.UseUnreadStyle;
            CmbDateTimeFormat.Text = _configurations.DateTimeFormat;
            ConnectionTimeOut.Text = _configurations.DefaultTimeOut.ToString();
            CheckRetweetNoConfirm.Checked = _configurations.RetweetNoConfirm;
            CheckBalloonLimit.Checked = _configurations.LimitBalloon;
            ApplyEventNotifyFlag(_configurations.EventNotifyEnabled, _configurations.EventNotifyFlag, _configurations.IsMyEventNotifyFlag);
            CheckForceEventNotify.Checked = _configurations.ForceEventNotify;
            CheckFavEventUnread.Checked = _configurations.FavEventUnread;
            ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(_configurations.TranslateLanguage);
            SoundFileListup();
            ComboBoxAutoShortUrlFirst.SelectedIndex = (int)_configurations.AutoShortUrlFirst;
            CheckTabIconDisp.Checked = _configurations.TabIconDisp;
            chkReadOwnPost.Checked = _configurations.ReadOwnPost;
            CheckGetFav.Checked = _configurations.GetFav;
            CheckMonospace.Checked = _configurations.IsMonospace;
            CheckReadOldPosts.Checked = _configurations.ReadOldPosts;
            CheckUseSsl.Checked = _configurations.UseSsl;
            TextBitlyId.Text = _configurations.BitlyUser;
            TextBitlyPw.Text = _configurations.BitlyPwd;
            TextBitlyId.Modified = false;
            TextBitlyPw.Modified = false;
            CheckShowGrid.Checked = _configurations.ShowGrid;
            CheckAtIdSupple.Checked = _configurations.UseAtIdSupplement;
            CheckHashSupple.Checked = _configurations.UseHashSupplement;
            CheckPreviewEnable.Checked = _configurations.PreviewEnable;
            TwitterAPIText.Text = _configurations.TwitterApiUrl;
            TwitterSearchAPIText.Text = _configurations.TwitterSearchApiUrl;
            switch (_configurations.ReplyIconState)
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

            switch (_configurations.Language)
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

            HotkeyCheck.Checked = _configurations.HotkeyEnabled;
            HotkeyAlt.Checked = (_configurations.HotkeyMod & Keys.Alt) == Keys.Alt;
            HotkeyCtrl.Checked = (_configurations.HotkeyMod & Keys.Control) == Keys.Control;
            HotkeyShift.Checked = (_configurations.HotkeyMod & Keys.Shift) == Keys.Shift;
            HotkeyWin.Checked = (_configurations.HotkeyMod & Keys.LWin) == Keys.LWin;
            HotkeyCode.Text = _configurations.HotkeyValue.ToString();
            HotkeyText.Text = _configurations.HotkeyKey.ToString();
            HotkeyText.Tag = _configurations.HotkeyKey;
            HotkeyAlt.Enabled = _configurations.HotkeyEnabled;
            HotkeyShift.Enabled = _configurations.HotkeyEnabled;
            HotkeyCtrl.Enabled = _configurations.HotkeyEnabled;
            HotkeyWin.Enabled = _configurations.HotkeyEnabled;
            HotkeyText.Enabled = _configurations.HotkeyEnabled;
            HotkeyCode.Enabled = _configurations.HotkeyEnabled;
            CheckNewMentionsBlink.Checked = _configurations.BlinkNewMentions;
            GetMoreTextCountApi.Text = _configurations.MoreCountApi.ToString();
            FirstTextCountApi.Text = _configurations.FirstCountApi.ToString();
            SearchTextCountApi.Text = _configurations.SearchCountApi.ToString();
            FavoritesTextCountApi.Text = _configurations.FavoritesCountApi.ToString();
            UserTimelineTextCountApi.Text = _configurations.UserTimelineCountApi.ToString();
            ListTextCountApi.Text = _configurations.ListCountApi.ToString();
            UseChangeGetCount.Checked = _configurations.UseAdditionalCount;
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
            CheckOpenUserTimeline.Checked = _configurations.OpenUserTimeline;
            ListDoubleClickActionComboBox.SelectedIndex = _configurations.ListDoubleClickAction;
            UserAppointUrlText.Text = _configurations.UserAppointUrl;
            HideDuplicatedRetweetsCheck.Checked = _configurations.HideDuplicatedRetweets;
            IsPreviewFoursquareCheckBox.Checked = _configurations.IsPreviewFoursquare;
            FoursquarePreviewHeightTextBox.Text = _configurations.FoursquarePreviewHeight.ToString();
            FoursquarePreviewWidthTextBox.Text = _configurations.FoursquarePreviewWidth.ToString();
            FoursquarePreviewZoomTextBox.Text = _configurations.FoursquarePreviewZoom.ToString();
            IsListsIncludeRtsCheckBox.Checked = _configurations.IsListStatusesIncludeRts;
            TabMouseLockCheck.Checked = _configurations.TabMouseLock;
            IsRemoveSameFavEventCheckBox.Checked = _configurations.IsRemoveSameEvent;
            IsNotifyUseGrowlCheckBox.Checked = _configurations.IsNotifyUseGrowl;
            IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;

            TreeViewSetting.Nodes["BasedNode"].Tag = BasedPanel;
            TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = GetPeriodPanel;
            TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = StartupPanel;
            TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = GetCountPanel;
            TreeViewSetting.Nodes["ActionNode"].Tag = ActionPanel;
            TreeViewSetting.Nodes["ActionNode"].Nodes["TweetActNode"].Tag = TweetActPanel;
            TreeViewSetting.Nodes["PreviewNode"].Tag = PreviewPanel;
            TreeViewSetting.Nodes["PreviewNode"].Nodes["TweetPrvNode"].Tag = TweetPrvPanel;
            TreeViewSetting.Nodes["PreviewNode"].Nodes["NotifyNode"].Tag = NotifyPanel;
            TreeViewSetting.Nodes["FontNode"].Tag = FontPanel;
            TreeViewSetting.Nodes["FontNode"].Nodes["FontNode2"].Tag = FontPanel2;
            TreeViewSetting.Nodes["ConnectionNode"].Tag = ConnectionPanel;
            TreeViewSetting.Nodes["ConnectionNode"].Nodes["ProxyNode"].Tag = ProxyPanel;
            TreeViewSetting.Nodes["ConnectionNode"].Nodes["CooperateNode"].Tag = CooperatePanel;
            TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"].Tag = ShortUrlPanel;

            TreeViewSetting.SelectedNode = TreeViewSetting.Nodes[0];
            TreeViewSetting.ExpandAll();
            ActiveControl = StartAuthButton;
        }

        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_userStreamPeriodValidator.IsValidPeriod(UserstreamPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_timelilePeriodValidator.IsValidPeriod(TimelinePeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_timelilePeriodValidator.IsValidPeriod(ReplyPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_dmessagesPeriodValidator.IsValidPeriod(DMPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_publicSearchPeriodValidator.IsValidPeriod(PubSearchPeriod.Text))
            {
                e.Cancel = true;
            }
        }

        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!_dmessagesPeriodValidator.IsValidPeriod(ListsPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            if (!_dmessagesPeriodValidator.IsValidPeriod(UserTimelinePeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            CalcApiUsing();
        }

        private void UReadMng_CheckedChanged(object sender, EventArgs e)
        {
            StartupReaded.Enabled = UReadMng.Checked;
        }

        private void ButtonFontAndColor_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnUnread":
                    ChangeLabelFontAndColor(lblUnread);
                    break;

                case "btnDetail":
                    ChangeLabelFontAndColor(lblDetail);
                    break;

                case "btnListFont":
                    ChangeLabelFontAndColor(lblListFont);
                    break;

                case "btnInputFont":
                    ChangeLabelFontAndColor(lblInputFont);
                    break;
            }
        }

        private void ButtonColor_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnSelf":
                    ChangeLabelColor(lblSelf);
                    break;

                case "btnAtSelf":
                    ChangeLabelColor(lblAtSelf);
                    break;

                case "btnTarget":
                    ChangeLabelColor(lblTarget);
                    break;

                case "btnAtTarget":
                    ChangeLabelColor(lblAtTarget);
                    break;

                case "btnAtFromTarget":
                    ChangeLabelColor(lblAtFromTarget);
                    break;

                case "btnFav":
                    ChangeLabelColor(lblFav, false);
                    break;

                case "btnOWL":
                    ChangeLabelColor(lblOWL, false);
                    break;

                case "btnRetweet":
                    ChangeLabelColor(lblRetweet, false);
                    break;

                case "btnInputBackcolor":
                    ChangeLabelColor(lblInputBackcolor);
                    break;

                case "btnAtTo":
                    ChangeLabelColor(lblAtTo);
                    break;

                case "btnListBack":
                    ChangeLabelColor(lblListBackcolor);
                    break;

                case "btnDetailBack":
                    ChangeLabelColor(lblDetailBackcolor);
                    break;

                case "btnDetailLink":
                    ChangeLabelColor(lblDetailLink, false);
                    break;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (var filedlg = new OpenFileDialog())
            {
                filedlg.Filter = R.Button3_ClickText1;
                filedlg.FilterIndex = 1;
                filedlg.Title = R.Button3_ClickText2;
                filedlg.RestoreDirectory = true;

                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    BrowserPathText.Text = filedlg.FileName;
                }
            }
        }

        private void RadioProxySpecified_CheckedChanged(object sender, EventArgs e)
        {
            ChangeProxySettingControlsStatus(RadioProxySpecified.Checked);
        }

        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_proxyPortNumberValidator.IsValidPeriod(TextProxyPort.Text);
        }

        private void CheckOutputz_CheckedChanged(object sender, EventArgs e)
        {
            ChangeOutputzControlsStatus(CheckOutputz.Checked);
        }

        private void TextBoxOutputzKey_Validating(object sender, CancelEventArgs e)
        {
            if (CheckOutputz.Checked)
            {
                TextBoxOutputzKey.Text = TextBoxOutputzKey.Text.Trim();
                if (TextBoxOutputzKey.Text.Length == 0)
                {
                    MessageBox.Show(R.TextBoxOutputzKey_Validating);
                    e.Cancel = true;
                }
            }
        }

        private void CmbDateTimeFormat_TextUpdate(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_Validating(object sender, CancelEventArgs e)
        {
            if (!CreateDateTimeFormatSample())
            {
                MessageBox.Show(R.CmbDateTimeFormat_Validating);
                e.Cancel = true;
            }
        }

        private void ConnectionTimeOut_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_timeoutLimitsValidator.IsValidPeriod(ConnectionTimeOut.Text);
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void TextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(TextCountApi.Text);
        }

        private void TextCountApiReply_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(TextCountApiReply.Text);
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = ComboBoxAutoShortUrlFirst.SelectedIndex;
            var newVariable = selected == (int)UrlConverter.Bitly || selected == (int)UrlConverter.Jmp;
            Label76.Enabled = newVariable;
            Label77.Enabled = newVariable;
            TextBitlyId.Enabled = newVariable;
            TextBitlyPw.Enabled = newVariable;
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
            if (StartAuth())
            {
                if (PinAuth())
                {
                    CalcApiUsing();
                }
            }
        }

        private void AuthClearButton_Click(object sender, EventArgs e)
        {
            if (AuthUserCombo.SelectedIndex > -1)
            {
                AuthUserCombo.Items.RemoveAt(AuthUserCombo.SelectedIndex);
                AuthUserCombo.SelectedIndex = AuthUserCombo.Items.Count > 0 ? 0 : -1;
            }

            CalcApiUsing();
        }

        private void CheckPostAndGet_CheckedChanged(object sender, EventArgs e)
        {
            CalcApiUsing();
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            do
            {
                Thread.Sleep(10);
                if (Disposing || IsDisposed)
                {
                    return;
                }
            }
            while (!IsHandleCreated);
            TopMost = _configurations.AlwaysTop;
            CalcApiUsing();
        }

        private void ButtonApiCalc_Click(object sender, EventArgs e)
        {
            CalcApiUsing();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _validationError = false;
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
            ChangeHotkeyControlsStatus(HotkeyCheck.Checked);
        }

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(GetMoreTextCountApi.Text);
        }

        private void UseChangeGetCount_CheckedChanged(object sender, EventArgs e)
        {
            ChangeUseChangeGetCountControlStatus(UseChangeGetCount.Checked);
        }

        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(FirstTextCountApi.Text);
        }

        private void SearchTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_searchApiCountValidator.IsValidPeriod(SearchTextCountApi.Text);
        }

        private void FavoritesTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(FavoritesTextCountApi.Text);
        }

        private void UserTimelineTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(UserTimelineTextCountApi.Text);
        }

        private void ListTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_apiCountValidator.IsValidPeriod(ListTextCountApi.Text);
        }

        private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var tbl in _eventCheckboxTableElements)
            {
                tbl.CheckBox.Enabled = CheckEventNotify.Checked;
            }
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserAppointUrlText.Text)
                && !UserAppointUrlText.Text.StartsWith("http"))
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
            OpenUrl("https://twitter.com/signup");
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }

        #endregion event handler

        #region private methods

        private void InitPeriodValidators()
        {
            _apiCountValidator = new PeriodValidator(
                cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                R.TextCountApi_Validating1,
                R.TextCountApi_Validating1);
            _searchApiCountValidator = new PeriodValidator(
                cnt => !(cnt != 0 && (cnt < 20 || cnt > 100)),
                R.TextSearchCountApi_Validating1,
                R.TextSearchCountApi_Validating1);
            _proxyPortNumberValidator = new PeriodValidator(
                p => !(p < 0 && p > 65535),
                R.TextProxyPort_ValidatingText1,
                R.TextProxyPort_ValidatingText2);
            _timeoutLimitsValidator = new PeriodValidator(
                tm => !(tm < (int)HttpTimeOut.MinValue || tm > (int)HttpTimeOut.MaxValue),
                R.ConnectionTimeOut_ValidatingText1,
                R.ConnectionTimeOut_ValidatingText1);
            _userStreamPeriodValidator = new PeriodValidator(
                i => !(i < 0 || i > 60),
                R.UserstreamPeriod_ValidatingText1,
                R.UserstreamPeriod_ValidatingText1);
            _timelilePeriodValidator = new PeriodValidator(
                i => !(i != 0 && (i < 15 || i > 6000)),
                R.TimelinePeriod_ValidatingText1,
                R.TimelinePeriod_ValidatingText2);
            _dmessagesPeriodValidator = new PeriodValidator(
                i => !(i != 0 && (i < 15 || i > 6000)),
                R.DMPeriod_ValidatingText1,
                R.DMPeriod_ValidatingText2);
            _publicSearchPeriodValidator = new PeriodValidator(
                i => (i == 0 || (i >= 30 && i <= 6000)),
                R.PubSearchPeriod_ValidatingText1,
                R.PubSearchPeriod_ValidatingText2);
        }

        private bool SaveIntarvals(IntervalChangedEventArgs arg)
        {
            if (_configurations.UserstreamPeriodInt != Convert.ToInt32(UserstreamPeriod.Text))
            {
                _configurations.UserstreamPeriodInt = Convert.ToInt32(UserstreamPeriod.Text);
                arg.UserStream = true;
            }

            if (_configurations.TimelinePeriodInt != Convert.ToInt32(TimelinePeriod.Text))
            {
                _configurations.TimelinePeriodInt = Convert.ToInt32(TimelinePeriod.Text);
                arg.Timeline = true;
            }

            if (_configurations.DMPeriodInt != Convert.ToInt32(DMPeriod.Text))
            {
                _configurations.DMPeriodInt = Convert.ToInt32(DMPeriod.Text);
                arg.DirectMessage = true;
            }

            if (_configurations.PubSearchPeriodInt != Convert.ToInt32(PubSearchPeriod.Text))
            {
                _configurations.PubSearchPeriodInt = Convert.ToInt32(PubSearchPeriod.Text);
                arg.PublicSearch = true;
            }

            if (_configurations.ListsPeriodInt != Convert.ToInt32(ListsPeriod.Text))
            {
                _configurations.ListsPeriodInt = Convert.ToInt32(ListsPeriod.Text);
                arg.Lists = true;
            }

            if (_configurations.ReplyPeriodInt != Convert.ToInt32(ReplyPeriod.Text))
            {
                _configurations.ReplyPeriodInt = Convert.ToInt32(ReplyPeriod.Text);
                arg.Reply = true;
            }

            if (_configurations.UserTimelinePeriodInt != Convert.ToInt32(UserTimelinePeriod.Text))
            {
                _configurations.UserTimelinePeriodInt = Convert.ToInt32(UserTimelinePeriod.Text);
                arg.UserTimeline = true;
            }

            return arg.UserStream || arg.Timeline || arg.DirectMessage || arg.PublicSearch || arg.Lists || arg.Reply || arg.UserTimeline;
        }

        private bool TrySelectFontAndColor(ref Font f, ref Color c)
        {
            FontDialog1.Color = c;
            FontDialog1.Font = f;

            try
            {
                if (FontDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return false;
                }

                c = FontDialog1.Color;
                f = FontDialog1.Font;
                return true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void ChangeLabelFontAndColor(Label lb)
        {
            var c = lb.ForeColor;
            var f = lb.Font;
            if (TrySelectFontAndColor(ref f, ref c))
            {
                lb.ForeColor = c;
                lb.Font = f;
            }
        }

        private bool TrySelectColor(ref Color c)
        {
            ColorDialog1.AllowFullOpen = true;
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = false;
            ColorDialog1.SolidColorOnly = false;
            ColorDialog1.Color = c;
            var rtn = ColorDialog1.ShowDialog();
            if (rtn == DialogResult.Cancel)
            {
                return false;
            }

            c = ColorDialog1.Color;
            return true;
        }

        private void ChangeLabelColor(Label lb, bool back = true)
        {
            var c = back ? lb.BackColor : lb.ForeColor;
            if (TrySelectColor(ref c))
            {
                if (back)
                {
                    lb.BackColor = c;
                }
                else
                {
                    lb.ForeColor = c;
                }
            }
        }

        private void ChangeProxySettingControlsStatus(bool chk)
        {
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;
        }

        private void ChangeOutputzControlsStatus(bool cheked)
        {
            Label59.Enabled = cheked;
            Label60.Enabled = cheked;
            TextBoxOutputzKey.Enabled = cheked;
            ComboBoxOutputzUrlmode.Enabled = cheked;
        }

        private void ChangeHotkeyControlsStatus(bool chk)
        {
            HotkeyCtrl.Enabled = chk;
            HotkeyAlt.Enabled = chk;
            HotkeyShift.Enabled = chk;
            HotkeyWin.Enabled = chk;
            HotkeyText.Enabled = chk;
            HotkeyCode.Enabled = chk;
        }

        private void ChangeUseChangeGetCountControlStatus(bool check)
        {
            GetMoreTextCountApi.Enabled = check;
            FirstTextCountApi.Enabled = check;
            Label28.Enabled = check;
            Label30.Enabled = check;
            Label53.Enabled = check;
            Label66.Enabled = check;
            Label17.Enabled = check;
            Label25.Enabled = check;
            SearchTextCountApi.Enabled = check;
            FavoritesTextCountApi.Enabled = check;
            UserTimelineTextCountApi.Enabled = check;
            ListTextCountApi.Enabled = check;
        }

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(CmbDateTimeFormat.Text);
            }
            catch (FormatException)
            {
                LabelDateTimeFormatApplied.Text = R.CreateDateTimeFormatSampleText1;
                return false;
            }

            return true;
        }

        private bool StartAuth()
        {
            // 現在の設定内容で通信
            HttpConnection.ProxyType ptype =
                RadioProxyNone.Checked ? HttpConnection.ProxyType.None :
                RadioProxyIE.Checked ? HttpConnection.ProxyType.IE :
                HttpConnection.ProxyType.Specified;

            string padr = TextProxyAddress.Text.Trim();
            int pport = int.Parse(TextProxyPort.Text.Trim());
            string pusr = TextProxyUser.Text.Trim();
            string ppw = TextProxyPassword.Text.Trim();

            // 通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
            HttpTwitter.SetTwitterUrl(TwitterAPIText.Text.Trim());
            _tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            string pinPageUrl = string.Empty;
            string rslt = _tw.StartAuthentication(ref pinPageUrl);
            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(R.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }

            using (var ab = new AuthBrowser())
            {
                ab.IsAuthorized = true;
                ab.UrlString = pinPageUrl;
                if (ab.ShowDialog(this) == DialogResult.OK)
                {
                    _pin = ab.PinString;
                    return true;
                }

                return false;
            }
        }

        private bool PinAuth()
        {
            string pin = _pin;
            string rslt = _tw.Authenticate(pin);
            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(R.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }

            MessageBox.Show(R.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK);
            int idx = -1;
            var user = new UserAccount
            {
                Username = _tw.Username,
                UserId = _tw.UserId,
                Token = _tw.AccessToken,
                TokenSecret = _tw.AccessTokenSecret
            };

            foreach (var u in AuthUserCombo.Items)
            {
                if (((UserAccount)u).Username.ToLower() == _tw.Username.ToLower())
                {
                    idx = AuthUserCombo.Items.IndexOf(u);
                    break;
                }
            }

            if (idx > -1)
            {
                AuthUserCombo.Items.RemoveAt(idx);
                AuthUserCombo.Items.Insert(idx, user);
                AuthUserCombo.SelectedIndex = idx;
            }
            else
            {
                AuthUserCombo.SelectedIndex = AuthUserCombo.Items.Add(user);
            }

            return true;
        }

        private void DisplayApiMaxCount()
        {
            var v = (MyCommon.TwitterApiInfo.MaxCount > -1) ? MyCommon.TwitterApiInfo.MaxCount.ToString() : "???";
            LabelApiUsing.Text = string.Format(R.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, v);
        }

        private void CalcApiUsing()
        {
            int listsTabNum;
            try
            {
                // 初回起動時などにNothingの場合あり
                listsTabNum = TabInformations.Instance.GetTabsByType(TabUsageType.Lists).Count;
            }
            catch (Exception)
            {
                return;
            }

            int userTimelineTabNum;
            try
            {
                // 初回起動時などにNothingの場合あり
                userTimelineTabNum = TabInformations.Instance.GetTabsByType(TabUsageType.UserTimeline).Count;
            }
            catch (Exception)
            {
                return;
            }

            // Recent計算 0は手動更新
            int tmp;
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

            if (_tw != null)
            {
                if (MyCommon.TwitterApiInfo.MaxCount == -1)
                {
                    if (Twitter.AccountState == AccountState.Valid)
                    {
                        MyCommon.TwitterApiInfo.UsingCount = usingApi;
                        var proc = new Thread(() =>
                            {
                                _tw.GetInfoApi(null); // 取得エラー時はinfoCountは初期状態（値：-1）
                                if (IsHandleCreated && IsDisposed)
                                {
                                    Invoke(new MethodInvoker(DisplayApiMaxCount));
                                }
                            });
                        proc.Start();
                    }
                    else
                    {
                        LabelApiUsing.Text = string.Format(R.SettingAPIUse1, usingApi, "???");
                    }
                }
                else
                {
                    LabelApiUsing.Text = string.Format(R.SettingAPIUse1, usingApi, MyCommon.TwitterApiInfo.MaxCount);
                }
            }

            LabelPostAndGet.Visible = CheckPostAndGet.Checked && !_tw.UserStreamEnabled;
            LabelUserStreamActive.Visible = _tw.UserStreamEnabled;

            LabelApiUsingUserStreamEnabled.Text = string.Format(R.SettingAPIUse2, apiLists + apiUserTimeline);
            LabelApiUsingUserStreamEnabled.Visible = _tw.UserStreamEnabled;
        }

        private bool BitlyValidation(string id, string apikey)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(apikey))
            {
                return false;
            }

            // TODO: BitlyApi
            string req = "http://api.bit.ly/v3/validate";
            string content = string.Empty;
            var param = new Dictionary<string, string>
                {
                { "login", "tweenapi" },
                { "apiKey", "R_c5ee0e30bdfff88723c4457cc331886b" },
                { "x_login", id },
                { "x_apiKey", apikey },
                { "format", "txt" }
            };

            if (!(new HttpVarious()).PostData(req, param, ref content))
            {
                // 通信エラーの場合はとりあえずチェックを通ったことにする
                return true;
            }

            if (content.Trim() == "1")
            {
                // 検証成功
                return true;
            }

            if (content.Trim() == "0")
            {
                // 検証失敗 APIキーとIDの組み合わせが違う
                return false;
            }

            // 規定外応答：通信エラーの可能性があるためとりあえずチェックを通ったことにする
            return true;
        }

        private void InitEventCheckboxTable()
        {
            if (_eventCheckboxTableElements == null)
            {
                _eventCheckboxTableElements = new[]
                {
                    new EventCheckboxTblElement(CheckFavoritesEvent, EventType.Favorite),
                    new EventCheckboxTblElement(CheckUnfavoritesEvent, EventType.Unfavorite),
                    new EventCheckboxTblElement(CheckFollowEvent, EventType.Follow),
                    new EventCheckboxTblElement(CheckListMemberAddedEvent, EventType.ListMemberAdded),
                    new EventCheckboxTblElement(CheckListMemberRemovedEvent, EventType.ListMemberRemoved),
                    new EventCheckboxTblElement(CheckBlockEvent, EventType.Block),
                    new EventCheckboxTblElement(CheckUserUpdateEvent, EventType.UserUpdate),
                    new EventCheckboxTblElement(CheckListCreatedEvent, EventType.ListCreated)
                };
            }
        }

        private void GetEventNotifyFlag(ref EventType eventnotifyflag, ref EventType isMyeventnotifyflag)
        {
            var evt = EventType.None;
            var myevt = EventType.None;

            foreach (EventCheckboxTblElement tbl in _eventCheckboxTableElements)
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

            CheckEventNotify.Checked = rootEnabled;

            foreach (var tbl in _eventCheckboxTableElements)
            {
                tbl.CheckBox.CheckState = Convert.ToBoolean(evt & tbl.EventType) ?
                                              (Convert.ToBoolean(myevt & tbl.EventType) ?
                                                   CheckState.Checked :
                                                   CheckState.Indeterminate) :
                                              CheckState.Unchecked;
                tbl.CheckBox.Enabled = rootEnabled;
            }
        }

        private void SoundFileListup()
        {
            if (string.IsNullOrEmpty(_configurations.EventSoundFile))
            {
                _configurations.EventSoundFile = string.Empty;
            }

            MyCommon.ReloadSoundSelector(ComboBoxEventNotifySound, _configurations.EventSoundFile);
        }

        private void OpenUrl(string url)
        {
            string browserPath = !string.IsNullOrEmpty(_configurations.BrowserPath) ?
                _configurations.BrowserPath :
                BrowserPathText.Text;
            MyCommon.TryOpenUrl(url, browserPath);
        }

        #endregion private methods

        #region inner class

        private class EventCheckboxTblElement
        {
            public EventCheckboxTblElement(CheckBox cb, EventType et)
            {
                CheckBox = cb;
                EventType = et;
            }

            public CheckBox CheckBox { get; private set; }

            public EventType EventType { get; private set; }
        }

        private class PeriodValidator
        {
            private readonly Func<int, bool> _validator;
            private readonly string _msg1;
            private readonly string _msg2;

            public PeriodValidator(Func<int, bool> f, string err1, string err2)
            {
                _validator = f;
                _msg1 = err1;
                _msg2 = err2;
            }

            public bool IsValidPeriod(string input)
            {
                int t;
                if (!int.TryParse(input, out t))
                {
                    MessageBox.Show(_msg1);
                    return false;
                }

                if (!_validator(t))
                {
                    MessageBox.Show(_msg2);
                    return false;
                }

                return true;
            }
        }

        #endregion inner class
    }
}