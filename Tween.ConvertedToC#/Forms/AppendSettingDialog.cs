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
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    public partial class AppendSettingDialog
    {
        #region privates
        private Twitter tw;
        private bool validationError;
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
            }

            this.validationError = false;            
            this.configurations.UserAccounts.Clear();
            this.configurations.UserAccounts.AddRange(this.AuthUserCombo.Items.Cast<UserAccount>());
            if (this.AuthUserCombo.SelectedIndex < 0)
            {
                this.tw.ClearAuthInfo();
                this.tw.Initialize(string.Empty, string.Empty, string.Empty, 0);
            }
            else
            {
                string selectedUser = ((UserAccount)this.AuthUserCombo.SelectedItem).Username.ToLower();
                var newuser = this.configurations.UserAccounts.Where(u => u.Username.ToLower() == selectedUser).First();
                this.tw.Initialize(newuser.Token, newuser.TokenSecret, newuser.Username, newuser.UserId);
                if (newuser.UserId == 0)
                {
                    this.tw.VerifyCredentials();
                    newuser.UserId = this.tw.UserId;
                }
            }

            try
            {
                this.configurations.UserstreamStartup = this.StartupUserstreamCheck.Checked;

                var arg = new IntervalChangedEventArgs();
                bool isIntervalChanged = SaveIntarvals(arg);
                if (isIntervalChanged)
                {
                    if (this.IntervalChanged != null)
                    {
                        this.IntervalChanged(this, arg);
                    }
                }

                this.configurations.Readed = this.StartupReaded.Checked;
                this.configurations.IconSz = this.IconSize.SelectedIndex < 0 ? IconSizes.IconNone : (IconSizes)this.IconSize.SelectedIndex;
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

                this.configurations.SortOrderLock = this.CheckSortOrderLock.Checked;
                this.configurations.TinyUrlResolve = this.CheckTinyURL.Checked;
                this.configurations.ShortUrlForceResolve = this.CheckForceResolve.Checked;
                ShortUrl.IsResolve = this.configurations.TinyUrlResolve;
                ShortUrl.IsForceResolve = this.configurations.ShortUrlForceResolve;
                if (this.RadioProxyNone.Checked)
                {
                    this.configurations.SelectedProxyType = HttpConnection.ProxyType.None;
                }
                else if (this.RadioProxyIE.Checked)
                {
                    this.configurations.SelectedProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    this.configurations.SelectedProxyType = HttpConnection.ProxyType.Specified;
                }

                this.configurations.ProxyAddress = this.TextProxyAddress.Text.Trim();
                this.configurations.ProxyPort = int.Parse(this.TextProxyPort.Text.Trim());
                this.configurations.ProxyUser = this.TextProxyUser.Text.Trim();
                this.configurations.ProxyPassword = this.TextProxyPassword.Text.Trim();
                this.configurations.PeriodAdjust = this.CheckPeriodAdjust.Checked;
                this.configurations.StartupVersion = this.CheckStartupVersion.Checked;
                this.configurations.StartupFollowers = this.CheckStartupFollowers.Checked;
                this.configurations.RestrictFavCheck = this.CheckFavRestrict.Checked;
                this.configurations.AlwaysTop = this.CheckAlwaysTop.Checked;
                this.configurations.UrlConvertAuto = this.CheckAutoConvertUrl.Checked;
                this.configurations.ShortenTco = this.ShortenTcoCheck.Checked;
                this.configurations.OutputzEnabled = this.CheckOutputz.Checked;
                this.configurations.OutputzKey = this.TextBoxOutputzKey.Text.Trim();

                switch (this.ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        this.configurations.OutputzUrlmode = OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        this.configurations.OutputzUrlmode = OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                this.configurations.Nicoms = this.CheckNicoms.Checked;
                this.configurations.UseUnreadStyle = this.chkUnreadStyle.Checked;
                this.configurations.DateTimeFormat = this.CmbDateTimeFormat.Text;
                this.configurations.DefaultTimeOut = Convert.ToInt32(this.ConnectionTimeOut.Text);
                this.configurations.RetweetNoConfirm = this.CheckRetweetNoConfirm.Checked;
                this.configurations.LimitBalloon = this.CheckBalloonLimit.Checked;
                this.configurations.EventNotifyEnabled = this.CheckEventNotify.Checked;
                {
                    var m = this.configurations.EventNotifyFlag;
                    var i = this.configurations.IsMyEventNotifyFlag;
                    this.GetEventNotifyFlag(ref m, ref i);
                    this.configurations.EventNotifyFlag = m;
                    this.configurations.IsMyEventNotifyFlag = i;
                }
                this.configurations.ForceEventNotify = this.CheckForceEventNotify.Checked;
                this.configurations.FavEventUnread = this.CheckFavEventUnread.Checked;
                this.configurations.TranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(this.ComboBoxTranslateLanguage.SelectedIndex);
                this.configurations.EventSoundFile = Convert.ToString(this.ComboBoxEventNotifySound.SelectedItem);
                this.configurations.AutoShortUrlFirst = (UrlConverter)this.ComboBoxAutoShortUrlFirst.SelectedIndex;
                this.configurations.TabIconDisp = this.CheckTabIconDisp.Checked;
                this.configurations.ReadOwnPost = this.chkReadOwnPost.Checked;
                this.configurations.GetFav = this.CheckGetFav.Checked;
                this.configurations.IsMonospace = this.CheckMonospace.Checked;
                this.configurations.ReadOldPosts = this.CheckReadOldPosts.Checked;
                this.configurations.UseSsl = this.CheckUseSsl.Checked;
                this.configurations.BitlyUser = this.TextBitlyId.Text;
                this.configurations.BitlyPwd = this.TextBitlyPw.Text;
                this.configurations.ShowGrid = this.CheckShowGrid.Checked;
                this.configurations.UseAtIdSupplement = this.CheckAtIdSupple.Checked;
                this.configurations.UseHashSupplement = this.CheckHashSupple.Checked;
                this.configurations.PreviewEnable = this.CheckPreviewEnable.Checked;
                this.configurations.TwitterApiUrl = this.TwitterAPIText.Text.Trim();
                this.configurations.TwitterSearchApiUrl = this.TwitterSearchAPIText.Text.Trim();
                switch (this.ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        this.configurations.ReplyIconState = ReplyIconState.None;
                        break;
                    case 1:
                        this.configurations.ReplyIconState = ReplyIconState.StaticIcon;
                        break;
                    case 2:
                        this.configurations.ReplyIconState = ReplyIconState.BlinkIcon;
                        break;
                }

                var lngs = new[] { "OS", "ja", "en", "zh-CN" };
                this.configurations.Language = this.LanguageCombo.SelectedIndex < 0
                    || this.LanguageCombo.SelectedIndex > lngs.Length ? "en" : lngs[this.LanguageCombo.SelectedIndex];

                this.configurations.HotkeyEnabled = this.HotkeyCheck.Checked;
                this.configurations.HotkeyMod = Keys.None;
                if (this.HotkeyAlt.Checked)
                {
                    this.configurations.HotkeyMod = this.configurations.HotkeyMod | Keys.Alt;
                }

                if (this.HotkeyShift.Checked)
                {
                    this.configurations.HotkeyMod = this.configurations.HotkeyMod | Keys.Shift;
                }

                if (this.HotkeyCtrl.Checked)
                {
                    this.configurations.HotkeyMod = this.configurations.HotkeyMod | Keys.Control;
                }

                if (this.HotkeyWin.Checked)
                {
                    this.configurations.HotkeyMod = this.configurations.HotkeyMod | Keys.LWin;
                }

                {
                    int tmp = this.configurations.HotkeyValue;
                    if (int.TryParse(this.HotkeyCode.Text, out tmp))
                    {
                        this.configurations.HotkeyValue = tmp;
                    }
                }

                this.configurations.HotkeyKey = (Keys)this.HotkeyText.Tag;
                this.configurations.BlinkNewMentions = this.CheckNewMentionsBlink.Checked;
                this.configurations.UseAdditionalCount = this.UseChangeGetCount.Checked;
                this.configurations.MoreCountApi = Convert.ToInt32(this.GetMoreTextCountApi.Text);
                this.configurations.FirstCountApi = Convert.ToInt32(this.FirstTextCountApi.Text);
                this.configurations.SearchCountApi = Convert.ToInt32(this.SearchTextCountApi.Text);
                this.configurations.FavoritesCountApi = Convert.ToInt32(this.FavoritesTextCountApi.Text);
                this.configurations.UserTimelineCountApi = Convert.ToInt32(this.UserTimelineTextCountApi.Text);
                this.configurations.ListCountApi = Convert.ToInt32(this.ListTextCountApi.Text);
                this.configurations.OpenUserTimeline = this.CheckOpenUserTimeline.Checked;
                this.configurations.ListDoubleClickAction = this.ListDoubleClickActionComboBox.SelectedIndex;
                this.configurations.UserAppointUrl = this.UserAppointUrlText.Text;
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

        private bool SaveIntarvals(IntervalChangedEventArgs arg)
        {
            if (this.configurations.UserstreamPeriodInt != Convert.ToInt32(this.UserstreamPeriod.Text))
            {
                this.configurations.UserstreamPeriodInt = Convert.ToInt32(this.UserstreamPeriod.Text);
                arg.UserStream = true;
            }

            if (this.configurations.TimelinePeriodInt != Convert.ToInt32(this.TimelinePeriod.Text))
            {
                this.configurations.TimelinePeriodInt = Convert.ToInt32(this.TimelinePeriod.Text);
                arg.Timeline = true;
            }

            if (this.configurations.DMPeriodInt != Convert.ToInt32(this.DMPeriod.Text))
            {
                this.configurations.DMPeriodInt = Convert.ToInt32(this.DMPeriod.Text);
                arg.DirectMessage = true;
            }

            if (this.configurations.PubSearchPeriodInt != Convert.ToInt32(this.PubSearchPeriod.Text))
            {
                this.configurations.PubSearchPeriodInt = Convert.ToInt32(this.PubSearchPeriod.Text);
                arg.PublicSearch = true;
            }

            if (this.configurations.ListsPeriodInt != Convert.ToInt32(this.ListsPeriod.Text))
            {
                this.configurations.ListsPeriodInt = Convert.ToInt32(this.ListsPeriod.Text);
                arg.Lists = true;
            }

            if (this.configurations.ReplyPeriodInt != Convert.ToInt32(this.ReplyPeriod.Text))
            {
                this.configurations.ReplyPeriodInt = Convert.ToInt32(this.ReplyPeriod.Text);
                arg.Reply = true;
            }

            if (this.configurations.UserTimelinePeriodInt != Convert.ToInt32(this.UserTimelinePeriod.Text))
            {
                this.configurations.UserTimelinePeriodInt = Convert.ToInt32(this.UserTimelinePeriod.Text);
                arg.UserTimeline = true;
            }

            return arg.UserStream || arg.Timeline || arg.DirectMessage || arg.PublicSearch || arg.Lists || arg.Reply || arg.UserTimeline;
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
                    foreach (var u in this.configurations.UserAccounts)
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
            this.GroupBox2.Visible = false;
            this.tw = ((TweenMain)this.Owner).TwitterInstance;
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

            this.CheckSortOrderLock.Checked = this.configurations.SortOrderLock;
            this.CheckTinyURL.Checked = this.configurations.TinyUrlResolve;
            this.CheckForceResolve.Checked = this.configurations.ShortUrlForceResolve;
            switch (this.configurations.SelectedProxyType)
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

            this.ChangeProxySettingControlsStatus(this.RadioProxySpecified.Checked);

            this.TextProxyAddress.Text = this.configurations.ProxyAddress;
            this.TextProxyPort.Text = this.configurations.ProxyPort.ToString();
            this.TextProxyUser.Text = this.configurations.ProxyUser;
            this.TextProxyPassword.Text = this.configurations.ProxyPassword;

            this.CheckPeriodAdjust.Checked = this.configurations.PeriodAdjust;
            this.CheckStartupVersion.Checked = this.configurations.StartupVersion;
            this.CheckStartupFollowers.Checked = this.configurations.StartupFollowers;
            this.CheckFavRestrict.Checked = this.configurations.RestrictFavCheck;
            this.CheckAlwaysTop.Checked = this.configurations.AlwaysTop;
            this.CheckAutoConvertUrl.Checked = this.configurations.UrlConvertAuto;
            this.ShortenTcoCheck.Checked = this.configurations.ShortenTco;
            this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;
            this.CheckOutputz.Checked = this.configurations.OutputzEnabled;
            this.ChangeOutputzControlsStatus(this.CheckOutputz.Checked); 
            this.TextBoxOutputzKey.Text = this.configurations.OutputzKey;
            switch (this.configurations.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    this.ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    this.ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            this.CheckNicoms.Checked = this.configurations.Nicoms;
            this.chkUnreadStyle.Checked = this.configurations.UseUnreadStyle;
            this.CmbDateTimeFormat.Text = this.configurations.DateTimeFormat;
            this.ConnectionTimeOut.Text = this.configurations.DefaultTimeOut.ToString();
            this.CheckRetweetNoConfirm.Checked = this.configurations.RetweetNoConfirm;
            this.CheckBalloonLimit.Checked = this.configurations.LimitBalloon;
            this.ApplyEventNotifyFlag(this.configurations.EventNotifyEnabled, this.configurations.EventNotifyFlag, this.configurations.IsMyEventNotifyFlag);
            this.CheckForceEventNotify.Checked = this.configurations.ForceEventNotify;
            this.CheckFavEventUnread.Checked = this.configurations.FavEventUnread;
            this.ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(this.configurations.TranslateLanguage);
            this.SoundFileListup();
            this.ComboBoxAutoShortUrlFirst.SelectedIndex = (int)this.configurations.AutoShortUrlFirst;
            this.CheckTabIconDisp.Checked = this.configurations.TabIconDisp;
            this.chkReadOwnPost.Checked = this.configurations.ReadOwnPost;
            this.CheckGetFav.Checked = this.configurations.GetFav;
            this.CheckMonospace.Checked = this.configurations.IsMonospace;
            this.CheckReadOldPosts.Checked = this.configurations.ReadOldPosts;
            this.CheckUseSsl.Checked = this.configurations.UseSsl;
            this.TextBitlyId.Text = this.configurations.BitlyUser;
            this.TextBitlyPw.Text = this.configurations.BitlyPwd;
            this.TextBitlyId.Modified = false;
            this.TextBitlyPw.Modified = false;
            this.CheckShowGrid.Checked = this.configurations.ShowGrid;
            this.CheckAtIdSupple.Checked = this.configurations.UseAtIdSupplement;
            this.CheckHashSupple.Checked = this.configurations.UseHashSupplement;
            this.CheckPreviewEnable.Checked = this.configurations.PreviewEnable;
            this.TwitterAPIText.Text = this.configurations.TwitterApiUrl;
            this.TwitterSearchAPIText.Text = this.configurations.TwitterSearchApiUrl;
            switch (this.configurations.ReplyIconState)
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

            switch (this.configurations.Language)
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

            this.HotkeyCheck.Checked = this.configurations.HotkeyEnabled;
            this.HotkeyAlt.Checked = (this.configurations.HotkeyMod & Keys.Alt) == Keys.Alt;
            this.HotkeyCtrl.Checked = (this.configurations.HotkeyMod & Keys.Control) == Keys.Control;
            this.HotkeyShift.Checked = (this.configurations.HotkeyMod & Keys.Shift) == Keys.Shift;
            this.HotkeyWin.Checked = (this.configurations.HotkeyMod & Keys.LWin) == Keys.LWin;
            this.HotkeyCode.Text = this.configurations.HotkeyValue.ToString();
            this.HotkeyText.Text = this.configurations.HotkeyKey.ToString();
            this.HotkeyText.Tag = this.configurations.HotkeyKey;
            this.HotkeyAlt.Enabled = this.configurations.HotkeyEnabled;
            this.HotkeyShift.Enabled = this.configurations.HotkeyEnabled;
            this.HotkeyCtrl.Enabled = this.configurations.HotkeyEnabled;
            this.HotkeyWin.Enabled = this.configurations.HotkeyEnabled;
            this.HotkeyText.Enabled = this.configurations.HotkeyEnabled;
            this.HotkeyCode.Enabled = this.configurations.HotkeyEnabled;
            this.CheckNewMentionsBlink.Checked = this.configurations.BlinkNewMentions;
            this.GetMoreTextCountApi.Text = this.configurations.MoreCountApi.ToString();
            this.FirstTextCountApi.Text = this.configurations.FirstCountApi.ToString();
            this.SearchTextCountApi.Text = this.configurations.SearchCountApi.ToString();
            this.FavoritesTextCountApi.Text = this.configurations.FavoritesCountApi.ToString();
            this.UserTimelineTextCountApi.Text = this.configurations.UserTimelineCountApi.ToString();
            this.ListTextCountApi.Text = this.configurations.ListCountApi.ToString();
            this.UseChangeGetCount.Checked = this.configurations.UseAdditionalCount;
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
            this.CheckOpenUserTimeline.Checked = this.configurations.OpenUserTimeline;
            this.ListDoubleClickActionComboBox.SelectedIndex = this.configurations.ListDoubleClickAction;
            this.UserAppointUrlText.Text = this.configurations.UserAppointUrl;
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

        private class PeriodValidator
        {
            private Func<int, bool> validator;
            private string msg1;
            private string msg2;

            public PeriodValidator(Func<int, bool> f, string err1, string err2)
            {
                this.validator = f;
                this.msg1 = err1;
                this.msg2 = err2;
            }

            public bool IsValidPeriod(string input)
            {
                int t = 0;
                if (Int32.TryParse(input, out t))
                {
                    MessageBox.Show(this.msg1);
                    return false;
                }

                if (!validator(t))
                {
                    MessageBox.Show(this.msg2);
                    return false;
                }

                return true;
            }
        };

        private PeriodValidator usPV;
        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (this.usPV == null)
            {
                usPV = new PeriodValidator(
                    i => i >= 0 && i <= 60,
                    Hoehoe.Properties.Resources.UserstreamPeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.UserstreamPeriod_ValidatingText1);
            }

            if (!usPV.IsValidPeriod(this.UserstreamPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private PeriodValidator tlPV;
        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            if (tlPV == null)
            {
                tlPV = new PeriodValidator(
                    i => !(i != 0 && (i < 15 || i > 6000)),
                    Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText2);
            }

            if (!tlPV.IsValidPeriod(this.TimelinePeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private PeriodValidator rpPV;
        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (rpPV == null)
            {
                rpPV = new PeriodValidator(
                    i => !(i != 0 && (i < 15 || i > 6000)),
                    Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.TimelinePeriod_ValidatingText2);
            }

            if (!rpPV.IsValidPeriod(this.ReplyPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private PeriodValidator dmPV;
        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (dmPV == null)
            {
                dmPV = new PeriodValidator(
                    i => !(i != 0 && (i < 15 || i > 6000)),
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
            }

            if(!dmPV.IsValidPeriod(this.DMPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private PeriodValidator psPV;
        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (psPV == null)
            {
                psPV = new PeriodValidator(
                    i => !(i != 0 && (i < 30 || i > 6000)),
                    Hoehoe.Properties.Resources.PubSearchPeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.PubSearchPeriod_ValidatingText2);
            }
            
            if (!psPV.IsValidPeriod(this.PubSearchPeriod.Text))
            {
                e.Cancel = true;
                return;
            }
        }

        private PeriodValidator lsPV;
        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (lsPV == null)
            {
                lsPV = new PeriodValidator(prd => !(prd != 0 && (prd < 15 || prd > 6000)),
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
            }

            if (!lsPV.IsValidPeriod(this.ListsPeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private PeriodValidator utPV;
        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            if (utPV == null)
            {
                utPV = new PeriodValidator(prd => !(prd != 0 && (prd < 15 || prd > 6000)),
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText1,
                    Hoehoe.Properties.Resources.DMPeriod_ValidatingText2);
            }

            if (!utPV.IsValidPeriod(this.UserTimelinePeriod.Text))
            {
                e.Cancel = true;
                return;
            }

            this.CalcApiUsing();
        }

        private void UReadMng_CheckedChanged(object sender, EventArgs e)
        {
            this.StartupReaded.Enabled = this.UReadMng.Checked;
        }

        private bool TrySelectFontAndColor(ref Color c, ref Font f)
        {
            this.FontDialog1.Color = c;
            this.FontDialog1.Font = f;

            try
            {
                if (this.FontDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return false;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            c = this.FontDialog1.Color;
            f = this.FontDialog1.Font;
            return true;
        }

        private void ButtonFontAndColor_Click_Extract(Label lb)
        {
            var c = lb.ForeColor;
            var f = lb.Font;
            if (this.TrySelectFontAndColor(ref c, ref f))
            {
                lb.ForeColor = c;
                lb.Font = f;
            }
        }

        private void ButtonFontAndColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;            
            switch (btn.Name)
            {
                case "btnUnread":
                    ButtonFontAndColor_Click_Extract(this.lblUnread);
                    break;
                case "btnDetail":
                    ButtonFontAndColor_Click_Extract(this.lblDetail);
                    break;
                case "btnListFont":
                    ButtonFontAndColor_Click_Extract(this.lblListFont);
                    break;
                case "btnInputFont":
                    ButtonFontAndColor_Click_Extract(this.lblInputFont);
                    break;
            }
        }

        private bool TrySelectColor(ref Color c)
        {
            this.ColorDialog1.AllowFullOpen = true;
            this.ColorDialog1.AnyColor = true;
            this.ColorDialog1.FullOpen = false;
            this.ColorDialog1.SolidColorOnly = false;
            this.ColorDialog1.Color = c;
            var rtn = this.ColorDialog1.ShowDialog();
            if (rtn == DialogResult.Cancel)
            {
                return false;
            }

            c = this.ColorDialog1.Color;
            return true;
        }

        private void ButtonColor_ClickExtractd(Label lb, bool back = true)
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

        private void ButtonColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnSelf": this.ButtonColor_ClickExtractd(this.lblSelf); break;
                case "btnAtSelf": this.ButtonColor_ClickExtractd(this.lblAtSelf); break;
                case "btnTarget": this.ButtonColor_ClickExtractd(this.lblTarget); break;
                case "btnAtTarget": this.ButtonColor_ClickExtractd(this.lblAtTarget); break;
                case "btnAtFromTarget": this.ButtonColor_ClickExtractd(this.lblAtFromTarget); break;
                case "btnFav": this.ButtonColor_ClickExtractd(this.lblFav, false); break;
                case "btnOWL": this.ButtonColor_ClickExtractd(this.lblOWL, false); break;
                case "btnRetweet": this.ButtonColor_ClickExtractd(this.lblRetweet, false); break;
                case "btnInputBackcolor": this.ButtonColor_ClickExtractd(this.lblInputBackcolor); break;
                case "btnAtTo": this.ButtonColor_ClickExtractd(this.lblAtTo); break;
                case "btnListBack": this.ButtonColor_ClickExtractd(this.lblListBackcolor); break;
                case "btnDetailBack": this.ButtonColor_ClickExtractd(this.lblDetailBackcolor); break;
                case "btnDetailLink": this.ButtonColor_ClickExtractd(this.lblDetailLink, false); break;
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
            ChangeProxySettingControlsStatus(this.RadioProxySpecified.Checked);
        }

        private void ChangeProxySettingControlsStatus(bool chk)
        {
            this.LabelProxyAddress.Enabled = chk;
            this.TextProxyAddress.Enabled = chk;
            this.LabelProxyPort.Enabled = chk;
            this.TextProxyPort.Enabled = chk;
            this.LabelProxyUser.Enabled = chk;
            this.TextProxyUser.Enabled = chk;
            this.LabelProxyPassword.Enabled = chk;
            this.TextProxyPassword.Enabled = chk;
        }

        private PeriodValidator ppV;
        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            if (this.ppV == null)
            {
                ppV = new PeriodValidator(p => !(p < 0 && p > 65535),
                    Hoehoe.Properties.Resources.TextProxyPort_ValidatingText1,
                    Hoehoe.Properties.Resources.TextProxyPort_ValidatingText2);
            }
            e.Cancel = !ppV.IsValidPeriod(this.TextProxyPort.Text);
        }

        private void CheckOutputz_CheckedChanged(object sender, EventArgs e)
        {
            ChangeOutputzControlsStatus(this.CheckOutputz.Checked);
        }

        private void ChangeOutputzControlsStatus(bool cheked)
        {
            this.Label59.Enabled = cheked;
            this.Label60.Enabled = cheked;
            this.TextBoxOutputzKey.Enabled = cheked;
            this.ComboBoxOutputzUrlmode.Enabled = cheked;
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

        private PeriodValidator tmPV;
        private void ConnectionTimeOut_Validating(object sender, CancelEventArgs e)
        {
            if (tmPV == null)
            {
                tmPV = new PeriodValidator(tm => !(tm < (int)HttpTimeOut.MinValue || tm > (int)HttpTimeOut.MaxValue),
                    Hoehoe.Properties.Resources.ConnectionTimeOut_ValidatingText1,
                    Hoehoe.Properties.Resources.ConnectionTimeOut_ValidatingText1);
            }

            e.Cancel = !tmPV.IsValidPeriod(this.ConnectionTimeOut.Text);
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            this.CreateDateTimeFormatSample();
        }

        private PeriodValidator tcPV;
        private void TextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (tcPV == null ) {
                tcPV = new PeriodValidator(cnt => !(cnt <20 || cnt >200),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !tcPV.IsValidPeriod(this.TextCountApi.Text);

        }

        private PeriodValidator trPV;
        private void TextCountApiReply_Validating(object sender, CancelEventArgs e)
        {
            if (trPV == null)
            {
                trPV = new PeriodValidator(cnt => !(cnt < 20 || cnt > 200),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !trPV.IsValidPeriod(this.TextCountApiReply.Text);
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool newVariable = (this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Bitly 
                || this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)UrlConverter.Jmp);
            this.Label76.Enabled = newVariable;
            this.Label77.Enabled = newVariable;
            this.TextBitlyId.Enabled = newVariable;
            this.TextBitlyPw.Enabled = newVariable;
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
                this.AuthUserCombo.SelectedIndex = this.AuthUserCombo.Items.Count > 0 ? 0 : -1;
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
            this.TopMost = this.configurations.AlwaysTop;
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
            ChangeHotkeyControlsStatus(this.HotkeyCheck.Checked);
        }

        private void ChangeHotkeyControlsStatus(bool chk)
        {
            this.HotkeyCtrl.Enabled = chk;
            this.HotkeyAlt.Enabled = chk;
            this.HotkeyShift.Enabled = chk;
            this.HotkeyWin.Enabled = chk;
            this.HotkeyText.Enabled = chk;
            this.HotkeyCode.Enabled = chk;
        }

        private PeriodValidator gmPV;
        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (gmPV == null)
            {
                gmPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !gmPV.IsValidPeriod(this.GetMoreTextCountApi.Text);
        }

        private void UseChangeGetCount_CheckedChanged(object sender, EventArgs e)
        {
            ChangeUseChangeGetCountControlStatus(this.UseChangeGetCount.Checked);
        }

        private void ChangeUseChangeGetCountControlStatus(bool check)
        {
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

        private PeriodValidator ftPV;
        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (ftPV == null)
            {
                ftPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !ftPV.IsValidPeriod(this.FirstTextCountApi.Text);
        }

        private PeriodValidator stPV;
        private void SearchTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (stPV == null)
            {
                stPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 100)),
                    Hoehoe.Properties.Resources.TextSearchCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextSearchCountApi_Validating1);
            }
            e.Cancel = !stPV.IsValidPeriod(this.SearchTextCountApi.Text);
        }

        private PeriodValidator fvPV;
        private void FavoritesTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (fvPV == null)
            {
                fvPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !fvPV.IsValidPeriod(this.FavoritesTextCountApi.Text);
        }

        private PeriodValidator utlPV;
        private void UserTimelineTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (utlPV == null)
            {
                utlPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !utlPV.IsValidPeriod(this.UserTimelineTextCountApi.Text);
        }

        private PeriodValidator lstPV;
        private void ListTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            if (lstPV == null)
            {
                lstPV = new PeriodValidator(cnt => !(cnt != 0 && (cnt < 20 || cnt > 200)),
                    Hoehoe.Properties.Resources.TextCountApi_Validating1,
                    Hoehoe.Properties.Resources.TextCountApi_Validating1);
            }
            e.Cancel = !lstPV.IsValidPeriod(this.ListTextCountApi.Text);
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
            HttpConnection.ProxyType ptype =
                (this.RadioProxyNone.Checked) ? HttpConnection.ProxyType.None :
                (this.RadioProxyIE.Checked) ? HttpConnection.ProxyType.IE :
                HttpConnection.ProxyType.Specified;

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
            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }

            using (var ab = new AuthBrowser())
            {
                ab.IsAuthorized = true;
                ab.UrlString = pinPageUrl;
                if (ab.ShowDialog(this) == DialogResult.OK)
                {
                    this.pin = ab.PinString;
                    return true;
                }
                return false;
            }
        }

        private bool PinAuth()
        {
            string pin = this.pin;
            string rslt = this.tw.Authenticate(pin);
            if (!string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Hoehoe.Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }

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
            var param = new Dictionary<string, string>()
            {
                {"login", "tweenapi"},
                {"apiKey", "R_c5ee0e30bdfff88723c4457cc331886b"},
                {"x_login", id},
                {"x_apiKey", apikey},
                {"format", "txt"},
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
            if (string.IsNullOrEmpty(this.configurations.EventSoundFile))
            {
                this.configurations.EventSoundFile = string.Empty;
            }

            MyCommon.ReloadSoundSelector(this.ComboBoxEventNotifySound, this.configurations.EventSoundFile);
        }

        private void OpenUrl(string url)
        {
            string myPath = url;
            string browserPath = !string.IsNullOrEmpty(this.configurations.BrowserPath) ? 
                this.configurations.BrowserPath :
                this.BrowserPathText.Text;
            MyCommon.TryOpenUrl(myPath, browserPath);
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