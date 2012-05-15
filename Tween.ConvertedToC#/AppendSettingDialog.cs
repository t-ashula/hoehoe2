// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Tween
{
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
        private Tween.MyCommon.IconSizes _MyIconSize;
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
        private Tween.MyCommon.NameBalloonEnum _myNameBalloon;
        private bool _myPostCtrlEnter;
        private bool _myPostShiftEnter;
        private bool _usePostMethod;
        private int _countApi;
        private int _countApiReply;
        private string _browserpath;
        private bool _myUseRecommendStatus;
        private bool _myDispUsername;
        private Tween.MyCommon.DispTitleEnum _MyDispLatestPost;
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
        private Tween.MyCommon.OutputzUrlmode _MyOutputzUrlmode;
        private bool _MyNicoms;
        private bool _MyUnreadStyle;
        private string _MyDateTimeFormat;
        private int _MyDefaultTimeOut;
        private bool _MyLimitBalloon;
        private bool _MyPostAndGet;
        private int _MyReplyPeriod;
        private Tween.MyCommon.UrlConverter _MyAutoShortUrlFirst;
        private bool _MyTabIconDisp;
        private Tween.MyCommon.ReplyIconState _MyReplyIconState;
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
        private Tween.MyCommon.EventType _MyEventNotifyFlag;
        private Tween.MyCommon.EventType _isMyEventNotifyFlag;
        private bool _MyForceEventNotify;
        private bool _MyFavEventUnread;
        private string _MyTranslateLanguage;
        private string _MyEventSoundFile;
        private int _MyUserstreamPeriod;
        private int _MyDoubleClickAction;
        private string _UserAppointUrl;

        public bool HideDuplicatedRetweets { get; set; }

        public bool IsPreviewFoursquare { get; set; }

        public int FoursquarePreviewHeight { get; set; }

        public int FoursquarePreviewWidth { get; set; }

        public int FoursquarePreviewZoom { get; set; }

        public bool IsListStatusesIncludeRts { get; set; }

        public List<UserAccount> UserAccounts { get; set; }

        private long _InitialUserId;

        public bool TabMouseLock { get; set; }

        public bool IsRemoveSameEvent { get; set; }

        public bool IsNotifyUseGrowl { get; set; }

        public TwitterDataModel.Configuration TwitterConfiguration { get; set; }

        private string _pin;

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

        public event IntervalChangedEventHandler IntervalChanged;

        public delegate void IntervalChangedEventHandler(object sender, IntervalChangedEventArgs e);

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
            if (My.MyProject.Forms.TweenMain.IsNetworkAvailable()
                && (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)Tween.MyCommon.UrlConverter.Bitly
                || ComboBoxAutoShortUrlFirst.SelectedIndex == (int)Tween.MyCommon.UrlConverter.Jmp)
                && (!String.IsNullOrEmpty(TextBitlyId.Text) || !String.IsNullOrEmpty(TextBitlyPw.Text)))
            {
                if (!BitlyValidation(TextBitlyId.Text, TextBitlyPw.Text))
                {
                    MessageBox.Show(Tween.My_Project.Resources.SettingSave_ClickText1);
                    _ValidationError = true;
                    TreeViewSetting.SelectedNode.Name = "TweetActNode";
                    // 動作タブを選択
                    TextBitlyId.Focus();
                    return;
                }
                else
                {
                    _ValidationError = false;
                }
            }
            else
            {
                _ValidationError = false;
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
                        _tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                        if (u.UserId == 0)
                        {
                            _tw.VerifyCredentials();
                            u.UserId = _tw.UserId;
                        }
                        break;
                    }
                }
            }
            else
            {
                _tw.ClearAuthInfo();
                _tw.Initialize("", "", "", 0);
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
                _MyUserstreamStartup = this.StartupUserstreamCheck.Checked;

                if (_MyUserstreamPeriod != Convert.ToInt32(UserstreamPeriod.Text))
                {
                    _MyUserstreamPeriod = Convert.ToInt32(UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }
                if (_MytimelinePeriod != Convert.ToInt32(TimelinePeriod.Text))
                {
                    _MytimelinePeriod = Convert.ToInt32(TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }
                if (_MyDMPeriod != Convert.ToInt32(DMPeriod.Text))
                {
                    _MyDMPeriod = Convert.ToInt32(DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }
                if (_MyPubSearchPeriod != Convert.ToInt32(PubSearchPeriod.Text))
                {
                    _MyPubSearchPeriod = Convert.ToInt32(PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (_MyListsPeriod != Convert.ToInt32(ListsPeriod.Text))
                {
                    _MyListsPeriod = Convert.ToInt32(ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }
                if (_MyReplyPeriod != Convert.ToInt32(ReplyPeriod.Text))
                {
                    _MyReplyPeriod = Convert.ToInt32(ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }
                if (_MyUserTimelinePeriod != Convert.ToInt32(UserTimelinePeriod.Text))
                {
                    _MyUserTimelinePeriod = Convert.ToInt32(UserTimelinePeriod.Text);
                    arg.UserTimeline = true;
                    isIntervalChanged = true;
                }

                if (isIntervalChanged)
                {
                    if (IntervalChanged != null)
                    {
                        IntervalChanged(this, arg);
                    }
                }

                _MyReaded = StartupReaded.Checked;
                switch (IconSize.SelectedIndex)
                {
                    case 0:
                        _MyIconSize = Tween.MyCommon.IconSizes.IconNone;
                        break;
                    case 1:
                        _MyIconSize = Tween.MyCommon.IconSizes.Icon16;
                        break;
                    case 2:
                        _MyIconSize = Tween.MyCommon.IconSizes.Icon24;
                        break;
                    case 3:
                        _MyIconSize = Tween.MyCommon.IconSizes.Icon48;
                        break;
                    case 4:
                        _MyIconSize = Tween.MyCommon.IconSizes.Icon48_2;
                        break;
                }
                _MyStatusText = StatusText.Text;
                _MyPlaySound = PlaySnd.Checked;
                _MyUnreadManage = UReadMng.Checked;
                _MyOneWayLove = OneWayLv.Checked;

                _fntUnread = lblUnread.Font;
                //未使用
                _clUnread = lblUnread.ForeColor;
                _fntReaded = lblListFont.Font;
                //リストフォントとして使用
                _clReaded = lblListFont.ForeColor;
                _clFav = lblFav.ForeColor;
                _clOWL = lblOWL.ForeColor;
                _clRetweet = lblRetweet.ForeColor;
                _fntDetail = lblDetail.Font;
                _clSelf = lblSelf.BackColor;
                _clAtSelf = lblAtSelf.BackColor;
                _clTarget = lblTarget.BackColor;
                _clAtTarget = lblAtTarget.BackColor;
                _clAtFromTarget = lblAtFromTarget.BackColor;
                _clAtTo = lblAtTo.BackColor;
                _clInputBackcolor = lblInputBackcolor.BackColor;
                _clInputFont = lblInputFont.ForeColor;
                _clListBackcolor = lblListBackcolor.BackColor;
                _clDetailBackcolor = lblDetailBackcolor.BackColor;
                _clDetail = lblDetail.ForeColor;
                _clDetailLink = lblDetailLink.ForeColor;
                _fntInputFont = lblInputFont.Font;
                switch (cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        _myNameBalloon = Tween.MyCommon.NameBalloonEnum.None;
                        break;
                    case 1:
                        _myNameBalloon = Tween.MyCommon.NameBalloonEnum.UserID;
                        break;
                    case 2:
                        _myNameBalloon = Tween.MyCommon.NameBalloonEnum.NickName;
                        break;
                }

                switch (ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        _myPostShiftEnter = true;
                        _myPostCtrlEnter = false;
                        break;
                    case 1:
                        _myPostCtrlEnter = true;
                        _myPostShiftEnter = false;
                        break;
                    case 0:
                        _myPostCtrlEnter = false;
                        _myPostShiftEnter = false;
                        break;
                }
                _usePostMethod = false;
                _countApi = Convert.ToInt32(TextCountApi.Text);
                _countApiReply = Convert.ToInt32(TextCountApiReply.Text);
                _browserpath = BrowserPathText.Text.Trim();
                _MyPostAndGet = CheckPostAndGet.Checked;
                _myUseRecommendStatus = CheckUseRecommendStatus.Checked;
                _myDispUsername = CheckDispUsername.Checked;
                _MyCloseToExit = CheckCloseToExit.Checked;
                _MyMinimizeToTray = CheckMinimizeToTray.Checked;
                switch (ComboDispTitle.SelectedIndex)
                {
                    case 0:
                        //None
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.None;
                        break;
                    case 1:
                        //Ver
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.Ver;
                        break;
                    case 2:
                        //Post
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.Post;
                        break;
                    case 3:
                        //RepCount
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.UnreadRepCount;
                        break;
                    case 4:
                        //AllCount
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.UnreadAllCount;
                        break;
                    case 5:
                        //Rep+All
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.UnreadAllRepCount;
                        break;
                    case 6:
                        //Unread/All
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.UnreadCountAllCount;
                        break;
                    case 7:
                        //Count of Status/Follow/Follower
                        _MyDispLatestPost = Tween.MyCommon.DispTitleEnum.OwnStatus;
                        break;
                }
                _MySortOrderLock = CheckSortOrderLock.Checked;
                _MyTinyUrlResolve = CheckTinyURL.Checked;
                _MyShortUrlForceResolve = CheckForceResolve.Checked;
                ShortUrl.IsResolve = _MyTinyUrlResolve;
                ShortUrl.IsForceResolve = _MyShortUrlForceResolve;
                if (RadioProxyNone.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    _MyProxyType = HttpConnection.ProxyType.Specified;
                }
                _MyProxyAddress = TextProxyAddress.Text.Trim();
                _MyProxyPort = int.Parse(TextProxyPort.Text.Trim());
                _MyProxyUser = TextProxyUser.Text.Trim();
                _MyProxyPassword = TextProxyPassword.Text.Trim();
                _MyPeriodAdjust = CheckPeriodAdjust.Checked;
                _MyStartupVersion = CheckStartupVersion.Checked;
                _MyStartupFollowers = CheckStartupFollowers.Checked;
                _MyRestrictFavCheck = CheckFavRestrict.Checked;
                _MyAlwaysTop = CheckAlwaysTop.Checked;
                _MyUrlConvertAuto = CheckAutoConvertUrl.Checked;
                _MyShortenTco = ShortenTcoCheck.Checked;
                _MyOutputz = CheckOutputz.Checked;
                _MyOutputzKey = TextBoxOutputzKey.Text.Trim();

                switch (ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        _MyOutputzUrlmode = Tween.MyCommon.OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        _MyOutputzUrlmode = Tween.MyCommon.OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                _MyNicoms = CheckNicoms.Checked;
                _MyUnreadStyle = chkUnreadStyle.Checked;
                _MyDateTimeFormat = CmbDateTimeFormat.Text;
                _MyDefaultTimeOut = Convert.ToInt32(ConnectionTimeOut.Text);
                _MyRetweetNoConfirm = CheckRetweetNoConfirm.Checked;
                _MyLimitBalloon = CheckBalloonLimit.Checked;
                _MyEventNotifyEnabled = CheckEventNotify.Checked;
                GetEventNotifyFlag(ref _MyEventNotifyFlag, ref _isMyEventNotifyFlag);
                _MyForceEventNotify = CheckForceEventNotify.Checked;
                _MyFavEventUnread = CheckFavEventUnread.Checked;
                _MyTranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
                _MyEventSoundFile = Convert.ToString(ComboBoxEventNotifySound.SelectedItem);
                _MyAutoShortUrlFirst = (Tween.MyCommon.UrlConverter)ComboBoxAutoShortUrlFirst.SelectedIndex;
                _MyTabIconDisp = chkTabIconDisp.Checked;
                _MyReadOwnPost = chkReadOwnPost.Checked;
                _MyGetFav = chkGetFav.Checked;
                _MyMonoSpace = CheckMonospace.Checked;
                _MyReadOldPosts = CheckReadOldPosts.Checked;
                _MyUseSsl = CheckUseSsl.Checked;
                _MyBitlyId = TextBitlyId.Text;
                _MyBitlyPw = TextBitlyPw.Text;
                _MyShowGrid = CheckShowGrid.Checked;
                _MyUseAtIdSupplement = CheckAtIdSupple.Checked;
                _MyUseHashSupplement = CheckHashSupple.Checked;
                _MyPreviewEnable = CheckPreviewEnable.Checked;
                _MyTwitterApiUrl = TwitterAPIText.Text.Trim();
                _MyTwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim();
                switch (ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        _MyReplyIconState = Tween.MyCommon.ReplyIconState.None;
                        break;
                    case 1:
                        _MyReplyIconState = Tween.MyCommon.ReplyIconState.StaticIcon;
                        break;
                    case 2:
                        _MyReplyIconState = Tween.MyCommon.ReplyIconState.BlinkIcon;
                        break;
                }
                switch (LanguageCombo.SelectedIndex)
                {
                    case 0:
                        _MyLanguage = "OS";
                        break;
                    case 1:
                        _MyLanguage = "ja";
                        break;
                    case 2:
                        _MyLanguage = "en";
                        break;
                    case 3:
                        _MyLanguage = "zh-CN";
                        break;
                    default:
                        _MyLanguage = "en";
                        break;
                }
                HotkeyEnabled = this.HotkeyCheck.Checked;
                HotkeyMod = Keys.None;
                if (this.HotkeyAlt.Checked)
                {
                    HotkeyMod = HotkeyMod | Keys.Alt;
                }
                if (this.HotkeyShift.Checked)
                {
                    HotkeyMod = HotkeyMod | Keys.Shift;
                }
                if (this.HotkeyCtrl.Checked)
                {
                    HotkeyMod = HotkeyMod | Keys.Control;
                }
                if (this.HotkeyWin.Checked)
                {
                    HotkeyMod = HotkeyMod | Keys.LWin;
                }
                if (Microsoft.VisualBasic.Information.IsNumeric(HotkeyCode.Text))
                {
                    HotkeyValue = Convert.ToInt32(HotkeyCode.Text);
                }
                HotkeyKey = (Keys)HotkeyText.Tag;
                BlinkNewMentions = ChkNewMentionsBlink.Checked;
                _MyUseAdditonalCount = UseChangeGetCount.Checked;
                _MoreCountApi = Convert.ToInt32(GetMoreTextCountApi.Text);
                _FirstCountApi = Convert.ToInt32(FirstTextCountApi.Text);
                _SearchCountApi = Convert.ToInt32(SearchTextCountApi.Text);
                _FavoritesCountApi = Convert.ToInt32(FavoritesTextCountApi.Text);
                _UserTimelineCountApi = Convert.ToInt32(UserTimelineTextCountApi.Text);
                _ListCountApi = Convert.ToInt32(ListTextCountApi.Text);
                _MyOpenUserTimeline = CheckOpenUserTimeline.Checked;
                _MyDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex;
                _UserAppointUrl = UserAppointUrlText.Text;
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
                MessageBox.Show(Tween.My_Project.Resources.Save_ClickText3);
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
                //キャンセル時は画面表示時のアカウントに戻す
                //キャンセル時でも認証済みアカウント情報は保存する
                this.UserAccounts.Clear();
                foreach (var u in this.AuthUserCombo.Items)
                {
                    this.UserAccounts.Add((UserAccount)u);
                }
                //アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (this._InitialUserId > 0)
                {
                    foreach (UserAccount u in this.UserAccounts)
                    {
                        if (u.UserId == this._InitialUserId)
                        {
                            _tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                            userSet = true;
                            break;
                        }
                    }
                }
                //認証済みアカウントが削除されていた場合、もしくは起動時アカウントがなかった場合は、
                //アクティブユーザーなしとして初期化
                if (!userSet)
                {
                    _tw.ClearAuthInfo();
                    _tw.Initialize("", "", "", 0);
                }
            }

            if (_tw != null && String.IsNullOrEmpty(_tw.Username) && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(Tween.My_Project.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            if (_ValidationError)
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
            _tw = ((TweenMain)this.Owner).TwitterInstance;
            string uname = _tw.Username;
            string pw = _tw.Password;
            string tk = _tw.AccessToken;
            string tks = _tw.AccessTokenSecret;

            this.AuthClearButton.Enabled = true;

            this.AuthUserCombo.Items.Clear();
            if (this.UserAccounts.Count > 0)
            {
                this.AuthUserCombo.Items.AddRange(this.UserAccounts.ToArray());
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.UserId == _tw.UserId)
                    {
                        this.AuthUserCombo.SelectedItem = u;
                        this._InitialUserId = u.UserId;
                        break;
                    }
                }
            }

            this.StartupUserstreamCheck.Checked = _MyUserstreamStartup;
            UserstreamPeriod.Text = _MyUserstreamPeriod.ToString();
            TimelinePeriod.Text = _MytimelinePeriod.ToString();
            ReplyPeriod.Text = _MyReplyPeriod.ToString();
            DMPeriod.Text = _MyDMPeriod.ToString();
            PubSearchPeriod.Text = _MyPubSearchPeriod.ToString();
            ListsPeriod.Text = _MyListsPeriod.ToString();
            UserTimelinePeriod.Text = _MyUserTimelinePeriod.ToString();

            StartupReaded.Checked = _MyReaded;
            switch (_MyIconSize)
            {
                case Tween.MyCommon.IconSizes.IconNone:
                    IconSize.SelectedIndex = 0;
                    break;
                case Tween.MyCommon.IconSizes.Icon16:
                    IconSize.SelectedIndex = 1;
                    break;
                case Tween.MyCommon.IconSizes.Icon24:
                    IconSize.SelectedIndex = 2;
                    break;
                case Tween.MyCommon.IconSizes.Icon48:
                    IconSize.SelectedIndex = 3;
                    break;
                case Tween.MyCommon.IconSizes.Icon48_2:
                    IconSize.SelectedIndex = 4;
                    break;
            }
            StatusText.Text = _MyStatusText;
            UReadMng.Checked = _MyUnreadManage;
            StartupReaded.Enabled = _MyUnreadManage != false;
            PlaySnd.Checked = _MyPlaySound;
            OneWayLv.Checked = _MyOneWayLove;

            lblListFont.Font = _fntReaded;
            lblUnread.Font = _fntUnread;
            lblUnread.ForeColor = _clUnread;
            lblListFont.ForeColor = _clReaded;
            lblFav.ForeColor = _clFav;
            lblOWL.ForeColor = _clOWL;
            lblRetweet.ForeColor = _clRetweet;
            lblDetail.Font = _fntDetail;
            lblSelf.BackColor = _clSelf;
            lblAtSelf.BackColor = _clAtSelf;
            lblTarget.BackColor = _clTarget;
            lblAtTarget.BackColor = _clAtTarget;
            lblAtFromTarget.BackColor = _clAtFromTarget;
            lblAtTo.BackColor = _clAtTo;
            lblInputBackcolor.BackColor = _clInputBackcolor;
            lblInputFont.ForeColor = _clInputFont;
            lblInputFont.Font = _fntInputFont;
            lblListBackcolor.BackColor = _clListBackcolor;
            lblDetailBackcolor.BackColor = _clDetailBackcolor;
            lblDetail.ForeColor = _clDetail;
            lblDetailLink.ForeColor = _clDetailLink;

            switch (_myNameBalloon)
            {
                case Tween.MyCommon.NameBalloonEnum.None:
                    cmbNameBalloon.SelectedIndex = 0;
                    break;
                case Tween.MyCommon.NameBalloonEnum.UserID:
                    cmbNameBalloon.SelectedIndex = 1;
                    break;
                case Tween.MyCommon.NameBalloonEnum.NickName:
                    cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            if (_myPostCtrlEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (_myPostShiftEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            TextCountApi.Text = _countApi.ToString();
            TextCountApiReply.Text = _countApiReply.ToString();
            BrowserPathText.Text = _browserpath;
            CheckPostAndGet.Checked = _MyPostAndGet;
            CheckUseRecommendStatus.Checked = _myUseRecommendStatus;
            CheckDispUsername.Checked = _myDispUsername;
            CheckCloseToExit.Checked = _MyCloseToExit;
            CheckMinimizeToTray.Checked = _MyMinimizeToTray;
            switch (_MyDispLatestPost)
            {
                case Tween.MyCommon.DispTitleEnum.None:
                    ComboDispTitle.SelectedIndex = 0;
                    break;
                case Tween.MyCommon.DispTitleEnum.Ver:
                    ComboDispTitle.SelectedIndex = 1;
                    break;
                case Tween.MyCommon.DispTitleEnum.Post:
                    ComboDispTitle.SelectedIndex = 2;
                    break;
                case Tween.MyCommon.DispTitleEnum.UnreadRepCount:
                    ComboDispTitle.SelectedIndex = 3;
                    break;
                case Tween.MyCommon.DispTitleEnum.UnreadAllCount:
                    ComboDispTitle.SelectedIndex = 4;
                    break;
                case Tween.MyCommon.DispTitleEnum.UnreadAllRepCount:
                    ComboDispTitle.SelectedIndex = 5;
                    break;
                case Tween.MyCommon.DispTitleEnum.UnreadCountAllCount:
                    ComboDispTitle.SelectedIndex = 6;
                    break;
                case Tween.MyCommon.DispTitleEnum.OwnStatus:
                    ComboDispTitle.SelectedIndex = 7;
                    break;
            }
            CheckSortOrderLock.Checked = _MySortOrderLock;
            CheckTinyURL.Checked = _MyTinyUrlResolve;
            CheckForceResolve.Checked = _MyShortUrlForceResolve;
            switch (_MyProxyType)
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

            TextProxyAddress.Text = _MyProxyAddress;
            TextProxyPort.Text = _MyProxyPort.ToString();
            TextProxyUser.Text = _MyProxyUser;
            TextProxyPassword.Text = _MyProxyPassword;

            CheckPeriodAdjust.Checked = _MyPeriodAdjust;
            CheckStartupVersion.Checked = _MyStartupVersion;
            CheckStartupFollowers.Checked = _MyStartupFollowers;
            CheckFavRestrict.Checked = _MyRestrictFavCheck;
            CheckAlwaysTop.Checked = _MyAlwaysTop;
            CheckAutoConvertUrl.Checked = _MyUrlConvertAuto;
            ShortenTcoCheck.Checked = _MyShortenTco;
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
            CheckOutputz.Checked = _MyOutputz;
            TextBoxOutputzKey.Text = _MyOutputzKey;

            switch (_MyOutputzUrlmode)
            {
                case Tween.MyCommon.OutputzUrlmode.twittercom:
                    ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case Tween.MyCommon.OutputzUrlmode.twittercomWithUsername:
                    ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            CheckNicoms.Checked = _MyNicoms;
            chkUnreadStyle.Checked = _MyUnreadStyle;
            CmbDateTimeFormat.Text = _MyDateTimeFormat;
            ConnectionTimeOut.Text = _MyDefaultTimeOut.ToString();
            CheckRetweetNoConfirm.Checked = _MyRetweetNoConfirm;
            CheckBalloonLimit.Checked = _MyLimitBalloon;

            ApplyEventNotifyFlag(_MyEventNotifyEnabled, _MyEventNotifyFlag, _isMyEventNotifyFlag);
            CheckForceEventNotify.Checked = _MyForceEventNotify;
            CheckFavEventUnread.Checked = _MyFavEventUnread;
            ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(_MyTranslateLanguage);
            SoundFileListup();
            ComboBoxAutoShortUrlFirst.SelectedIndex = (int)_MyAutoShortUrlFirst;
            chkTabIconDisp.Checked = _MyTabIconDisp;
            chkReadOwnPost.Checked = _MyReadOwnPost;
            chkGetFav.Checked = _MyGetFav;
            CheckMonospace.Checked = _MyMonoSpace;
            CheckReadOldPosts.Checked = _MyReadOldPosts;
            CheckUseSsl.Checked = _MyUseSsl;
            TextBitlyId.Text = _MyBitlyId;
            TextBitlyPw.Text = _MyBitlyPw;
            TextBitlyId.Modified = false;
            TextBitlyPw.Modified = false;
            CheckShowGrid.Checked = _MyShowGrid;
            CheckAtIdSupple.Checked = _MyUseAtIdSupplement;
            CheckHashSupple.Checked = _MyUseHashSupplement;
            CheckPreviewEnable.Checked = _MyPreviewEnable;
            TwitterAPIText.Text = _MyTwitterApiUrl;
            TwitterSearchAPIText.Text = _MyTwitterSearchApiUrl;
            switch (_MyReplyIconState)
            {
                case Tween.MyCommon.ReplyIconState.None:
                    ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case Tween.MyCommon.ReplyIconState.StaticIcon:
                    ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case Tween.MyCommon.ReplyIconState.BlinkIcon:
                    ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }
            switch (_MyLanguage)
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
            HotkeyCheck.Checked = HotkeyEnabled;
            HotkeyAlt.Checked = ((HotkeyMod & Keys.Alt) == Keys.Alt);
            HotkeyCtrl.Checked = ((HotkeyMod & Keys.Control) == Keys.Control);
            HotkeyShift.Checked = ((HotkeyMod & Keys.Shift) == Keys.Shift);
            HotkeyWin.Checked = ((HotkeyMod & Keys.LWin) == Keys.LWin);
            HotkeyCode.Text = HotkeyValue.ToString();
            HotkeyText.Text = HotkeyKey.ToString();
            HotkeyText.Tag = HotkeyKey;
            HotkeyAlt.Enabled = HotkeyEnabled;
            HotkeyShift.Enabled = HotkeyEnabled;
            HotkeyCtrl.Enabled = HotkeyEnabled;
            HotkeyWin.Enabled = HotkeyEnabled;
            HotkeyText.Enabled = HotkeyEnabled;
            HotkeyCode.Enabled = HotkeyEnabled;
            ChkNewMentionsBlink.Checked = BlinkNewMentions;

            CheckOutputz_CheckedChanged(sender, e);

            GetMoreTextCountApi.Text = _MoreCountApi.ToString();
            FirstTextCountApi.Text = _FirstCountApi.ToString();
            SearchTextCountApi.Text = _SearchCountApi.ToString();
            FavoritesTextCountApi.Text = _FavoritesCountApi.ToString();
            UserTimelineTextCountApi.Text = _UserTimelineCountApi.ToString();
            ListTextCountApi.Text = _ListCountApi.ToString();
            UseChangeGetCount.Checked = _MyUseAdditonalCount;
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
            CheckOpenUserTimeline.Checked = _MyOpenUserTimeline;
            ListDoubleClickActionComboBox.SelectedIndex = _MyDoubleClickAction;
            UserAppointUrlText.Text = _UserAppointUrl;
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
                MessageBox.Show(Tween.My_Project.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd < 0 || prd > 60)
            {
                MessageBox.Show(Tween.My_Project.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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
                MessageBox.Show(Tween.My_Project.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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
                MessageBox.Show(Tween.My_Project.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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
                MessageBox.Show(Tween.My_Project.Resources.PubSearchPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 30 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.PubSearchPeriod_ValidatingText2);
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
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Tween.My_Project.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
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

        public int UserstreamPeriodInt
        {
            get { return _MyUserstreamPeriod; }
            set { _MyUserstreamPeriod = value; }
        }

        public bool UserstreamStartup
        {
            get { return this._MyUserstreamStartup; }
            set { this._MyUserstreamStartup = value; }
        }

        public int TimelinePeriodInt
        {
            get { return _MytimelinePeriod; }
            set { _MytimelinePeriod = value; }
        }

        public int ReplyPeriodInt
        {
            get { return _MyReplyPeriod; }
            set { _MyReplyPeriod = value; }
        }

        public int DMPeriodInt
        {
            get { return _MyDMPeriod; }
            set { _MyDMPeriod = value; }
        }

        public int PubSearchPeriodInt
        {
            get { return _MyPubSearchPeriod; }
            set { _MyPubSearchPeriod = value; }
        }

        public int ListsPeriodInt
        {
            get { return _MyListsPeriod; }
            set { _MyListsPeriod = value; }
        }

        public int UserTimelinePeriodInt
        {
            get { return _MyUserTimelinePeriod; }
            set { _MyUserTimelinePeriod = value; }
        }

        public bool Readed
        {
            get { return _MyReaded; }
            set { _MyReaded = value; }
        }

        public Tween.MyCommon.IconSizes IconSz
        {
            get { return _MyIconSize; }
            set { _MyIconSize = value; }
        }

        public string Status
        {
            get { return _MyStatusText; }
            set { _MyStatusText = value; }
        }

        public bool UnreadManage
        {
            get { return _MyUnreadManage; }
            set { _MyUnreadManage = value; }
        }

        public bool PlaySound
        {
            get { return _MyPlaySound; }
            set { _MyPlaySound = value; }
        }

        public bool OneWayLove
        {
            get { return _MyOneWayLove; }
            set { _MyOneWayLove = value; }
        }

        ///''未使用
        public Font FontUnread
        {
            get { return _fntUnread; }
            //無視
            set { _fntUnread = value; }
        }

        public Color ColorUnread
        {
            get { return _clUnread; }
            set { _clUnread = value; }
        }

        ///''リストフォントとして使用
        public Font FontReaded
        {
            get { return _fntReaded; }
            set { _fntReaded = value; }
        }

        public Color ColorReaded
        {
            get { return _clReaded; }
            set { _clReaded = value; }
        }

        public Color ColorFav
        {
            get { return _clFav; }
            set { _clFav = value; }
        }

        public Color ColorOWL
        {
            get { return _clOWL; }
            set { _clOWL = value; }
        }

        public Color ColorRetweet
        {
            get { return _clRetweet; }
            set { _clRetweet = value; }
        }

        public Font FontDetail
        {
            get { return _fntDetail; }
            set { _fntDetail = value; }
        }

        public Color ColorDetail
        {
            get { return _clDetail; }
            set { _clDetail = value; }
        }

        public Color ColorDetailLink
        {
            get { return _clDetailLink; }
            set { _clDetailLink = value; }
        }

        public Color ColorSelf
        {
            get { return _clSelf; }
            set { _clSelf = value; }
        }

        public Color ColorAtSelf
        {
            get { return _clAtSelf; }
            set { _clAtSelf = value; }
        }

        public Color ColorTarget
        {
            get { return _clTarget; }
            set { _clTarget = value; }
        }

        public Color ColorAtTarget
        {
            get { return _clAtTarget; }
            set { _clAtTarget = value; }
        }

        public Color ColorAtFromTarget
        {
            get { return _clAtFromTarget; }
            set { _clAtFromTarget = value; }
        }

        public Color ColorAtTo
        {
            get { return _clAtTo; }
            set { _clAtTo = value; }
        }

        public Color ColorInputBackcolor
        {
            get { return _clInputBackcolor; }
            set { _clInputBackcolor = value; }
        }

        public Color ColorInputFont
        {
            get { return _clInputFont; }
            set { _clInputFont = value; }
        }

        public Font FontInputFont
        {
            get { return _fntInputFont; }
            set { _fntInputFont = value; }
        }

        public Color ColorListBackcolor
        {
            get { return _clListBackcolor; }
            set { _clListBackcolor = value; }
        }

        public Color ColorDetailBackcolor
        {
            get { return _clDetailBackcolor; }
            set { _clDetailBackcolor = value; }
        }

        public Tween.MyCommon.NameBalloonEnum NameBalloon
        {
            get { return _myNameBalloon; }
            set { _myNameBalloon = value; }
        }

        public bool PostCtrlEnter
        {
            get { return _myPostCtrlEnter; }
            set { _myPostCtrlEnter = value; }
        }

        public bool PostShiftEnter
        {
            get { return _myPostShiftEnter; }
            set { _myPostShiftEnter = value; }
        }

        public int CountApi
        {
            get { return _countApi; }
            set { _countApi = value; }
        }

        public int CountApiReply
        {
            get { return _countApiReply; }
            set { _countApiReply = value; }
        }

        public int MoreCountApi
        {
            get { return _MoreCountApi; }
            set { _MoreCountApi = value; }
        }

        public int FirstCountApi
        {
            get { return _FirstCountApi; }
            set { _FirstCountApi = value; }
        }

        public int SearchCountApi
        {
            get { return _SearchCountApi; }
            set { _SearchCountApi = value; }
        }

        public int FavoritesCountApi
        {
            get { return _FavoritesCountApi; }
            set { _FavoritesCountApi = value; }
        }

        public int UserTimelineCountApi
        {
            get { return _UserTimelineCountApi; }
            set { _UserTimelineCountApi = value; }
        }

        public int ListCountApi
        {
            get { return _ListCountApi; }
            set { _ListCountApi = value; }
        }

        public bool PostAndGet
        {
            get { return _MyPostAndGet; }
            set { _MyPostAndGet = value; }
        }

        public bool UseRecommendStatus
        {
            get { return _myUseRecommendStatus; }
            set { _myUseRecommendStatus = value; }
        }

        public string RecommendStatusText
        {
            get { return _MyRecommendStatusText; }
            set { _MyRecommendStatusText = value; }
        }

        public bool DispUsername
        {
            get { return _myDispUsername; }
            set { _myDispUsername = value; }
        }

        public bool CloseToExit
        {
            get { return _MyCloseToExit; }
            set { _MyCloseToExit = value; }
        }

        public bool MinimizeToTray
        {
            get { return _MyMinimizeToTray; }
            set { _MyMinimizeToTray = value; }
        }

        public Tween.MyCommon.DispTitleEnum DispLatestPost
        {
            get { return _MyDispLatestPost; }
            set { _MyDispLatestPost = value; }
        }

        public string BrowserPath
        {
            get { return _browserpath; }
            set { _browserpath = value; }
        }

        public bool TinyUrlResolve
        {
            get { return _MyTinyUrlResolve; }
            set { _MyTinyUrlResolve = value; }
        }

        public bool ShortUrlForceResolve
        {
            get { return _MyShortUrlForceResolve; }
            set { _MyShortUrlForceResolve = value; }
        }

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

        public bool SortOrderLock
        {
            get { return _MySortOrderLock; }
            set { _MySortOrderLock = value; }
        }

        public HttpConnection.ProxyType SelectedProxyType
        {
            get { return _MyProxyType; }
            set { _MyProxyType = value; }
        }

        public string ProxyAddress
        {
            get { return _MyProxyAddress; }
            set { _MyProxyAddress = value; }
        }

        public int ProxyPort
        {
            get { return _MyProxyPort; }
            set { _MyProxyPort = value; }
        }

        public string ProxyUser
        {
            get { return _MyProxyUser; }
            set { _MyProxyUser = value; }
        }

        public string ProxyPassword
        {
            get { return _MyProxyPassword; }
            set { _MyProxyPassword = value; }
        }

        public bool PeriodAdjust
        {
            get { return _MyPeriodAdjust; }
            set { _MyPeriodAdjust = value; }
        }

        public bool StartupVersion
        {
            get { return _MyStartupVersion; }
            set { _MyStartupVersion = value; }
        }

        public bool StartupFollowers
        {
            get { return _MyStartupFollowers; }
            set { _MyStartupFollowers = value; }
        }

        public bool RestrictFavCheck
        {
            get { return _MyRestrictFavCheck; }
            set { _MyRestrictFavCheck = value; }
        }

        public bool AlwaysTop
        {
            get { return _MyAlwaysTop; }
            set { _MyAlwaysTop = value; }
        }

        public bool UrlConvertAuto
        {
            get { return _MyUrlConvertAuto; }
            set { _MyUrlConvertAuto = value; }
        }

        public bool ShortenTco
        {
            get { return _MyShortenTco; }
            set { _MyShortenTco = value; }
        }

        public bool OutputzEnabled
        {
            get { return _MyOutputz; }
            set { _MyOutputz = value; }
        }

        public string OutputzKey
        {
            get { return _MyOutputzKey; }
            set { _MyOutputzKey = value; }
        }

        public Tween.MyCommon.OutputzUrlmode OutputzUrlmode
        {
            get { return _MyOutputzUrlmode; }
            set { _MyOutputzUrlmode = value; }
        }

        public bool Nicoms
        {
            get { return _MyNicoms; }
            set { _MyNicoms = value; }
        }

        public Tween.MyCommon.UrlConverter AutoShortUrlFirst
        {
            get { return _MyAutoShortUrlFirst; }
            set { _MyAutoShortUrlFirst = value; }
        }

        public bool UseUnreadStyle
        {
            get { return _MyUnreadStyle; }
            set { _MyUnreadStyle = value; }
        }

        public string DateTimeFormat
        {
            get { return _MyDateTimeFormat; }
            set { _MyDateTimeFormat = value; }
        }

        public int DefaultTimeOut
        {
            get { return _MyDefaultTimeOut; }
            set { _MyDefaultTimeOut = value; }
        }

        public bool RetweetNoConfirm
        {
            get { return _MyRetweetNoConfirm; }
            set { _MyRetweetNoConfirm = value; }
        }

        public bool TabIconDisp
        {
            get { return _MyTabIconDisp; }
            set { _MyTabIconDisp = value; }
        }

        public Tween.MyCommon.ReplyIconState ReplyIconState
        {
            get { return _MyReplyIconState; }
            set { _MyReplyIconState = value; }
        }

        public bool ReadOwnPost
        {
            get { return _MyReadOwnPost; }
            set { _MyReadOwnPost = value; }
        }

        public bool GetFav
        {
            get { return _MyGetFav; }
            set { _MyGetFav = value; }
        }

        public bool IsMonospace
        {
            get { return _MyMonoSpace; }
            set { _MyMonoSpace = value; }
        }

        public bool ReadOldPosts
        {
            get { return _MyReadOldPosts; }
            set { _MyReadOldPosts = value; }
        }

        public bool UseSsl
        {
            get { return _MyUseSsl; }
            set { _MyUseSsl = value; }
        }

        public string BitlyUser
        {
            get { return _MyBitlyId; }
            set { _MyBitlyId = value; }
        }

        public string BitlyPwd
        {
            get { return _MyBitlyPw; }
            set { _MyBitlyPw = value; }
        }

        public bool ShowGrid
        {
            get { return _MyShowGrid; }
            set { _MyShowGrid = value; }
        }

        public bool UseAtIdSupplement
        {
            get { return _MyUseAtIdSupplement; }
            set { _MyUseAtIdSupplement = value; }
        }

        public bool UseHashSupplement
        {
            get { return _MyUseHashSupplement; }
            set { _MyUseHashSupplement = value; }
        }

        public bool PreviewEnable
        {
            get { return _MyPreviewEnable; }
            set { _MyPreviewEnable = value; }
        }

        public bool UseAdditionalCount
        {
            get { return _MyUseAdditonalCount; }
            set { _MyUseAdditonalCount = value; }
        }

        public bool OpenUserTimeline
        {
            get { return _MyOpenUserTimeline; }
            set { _MyOpenUserTimeline = value; }
        }

        public string TwitterApiUrl
        {
            get { return _MyTwitterApiUrl; }
            set { _MyTwitterApiUrl = value; }
        }

        public string TwitterSearchApiUrl
        {
            get { return _MyTwitterSearchApiUrl; }
            set { _MyTwitterSearchApiUrl = value; }
        }

        public string Language
        {
            get { return _MyLanguage; }
            set { _MyLanguage = value; }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog filedlg = new OpenFileDialog())
            {
                filedlg.Filter = Tween.My_Project.Resources.Button3_ClickText1;
                filedlg.FilterIndex = 1;
                filedlg.Title = Tween.My_Project.Resources.Button3_ClickText2;
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
            if (String.IsNullOrEmpty(TextProxyPort.Text.Trim()))
            {
                TextProxyPort.Text = "0";
            }
            if (!int.TryParse(TextProxyPort.Text.Trim(), out port))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextProxyPort_ValidatingText1);
                e.Cancel = true;
                return;
            }
            if (port < 0 | port > 65535)
            {
                MessageBox.Show(Tween.My_Project.Resources.TextProxyPort_ValidatingText2);
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
                    MessageBox.Show(Tween.My_Project.Resources.TextBoxOutputzKey_Validating);
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
                LabelDateTimeFormatApplied.Text = Tween.My_Project.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }
            return true;
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
                MessageBox.Show(Tween.My_Project.Resources.CmbDateTimeFormat_Validating);
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
                MessageBox.Show(Tween.My_Project.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (tm < (int)Tween.MyCommon.HttpTimeOut.MinValue || tm > (int)Tween.MyCommon.HttpTimeOut.MaxValue)
            {
                MessageBox.Show(Tween.My_Project.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        public bool LimitBalloon
        {
            get { return _MyLimitBalloon; }
            set { _MyLimitBalloon = value; }
        }

        public bool EventNotifyEnabled
        {
            get { return _MyEventNotifyEnabled; }
            set { _MyEventNotifyEnabled = value; }
        }

        public Tween.MyCommon.EventType EventNotifyFlag
        {
            get { return _MyEventNotifyFlag; }
            set { _MyEventNotifyFlag = value; }
        }

        public Tween.MyCommon.EventType IsMyEventNotifyFlag
        {
            get { return _isMyEventNotifyFlag; }
            set { _isMyEventNotifyFlag = value; }
        }

        public bool ForceEventNotify
        {
            get { return _MyForceEventNotify; }
            set { _MyForceEventNotify = value; }
        }

        public bool FavEventUnread
        {
            get { return _MyFavEventUnread; }
            set { _MyFavEventUnread = value; }
        }

        public string TranslateLanguage
        {
            get { return _MyTranslateLanguage; }
            set
            {
                _MyTranslateLanguage = value;
                ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
            }
        }

        public string EventSoundFile
        {
            get { return _MyEventSoundFile; }
            set { _MyEventSoundFile = value; }
        }

        public int ListDoubleClickAction
        {
            get { return _MyDoubleClickAction; }
            set { _MyDoubleClickAction = value; }
        }

        public string UserAppointUrl
        {
            get { return _UserAppointUrl; }
            set { _UserAppointUrl = value; }
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)Tween.MyCommon.UrlConverter.Bitly || ComboBoxAutoShortUrlFirst.SelectedIndex == (int)Tween.MyCommon.UrlConverter.Jmp)
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
            lblUnread.ForeColor = System.Drawing.SystemColors.ControlText;
            lblUnread.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            lblListFont.ForeColor = System.Drawing.SystemColors.ControlText;
            lblListFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblDetail.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblDetail.Font = System.Drawing.SystemFonts.DefaultFont;

            lblInputFont.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblInputFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue);

            lblAtSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite);

            lblTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush);

            lblAtFromTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew);

            lblFav.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Red);

            lblOWL.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblInputBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTo.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Pink);

            lblListBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

            lblDetailBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

            lblDetailLink.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblRetweet.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Green);
        }

        private bool StartAuth()
        {
            //現在の設定内容で通信
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

            //通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
            HttpTwitter.TwitterUrl = TwitterAPIText.Text.Trim();
            HttpTwitter.TwitterSearchUrl = TwitterSearchAPIText.Text.Trim();
            _tw.Initialize("", "", "", 0);
            string pinPageUrl = "";
            string rslt = _tw.StartAuthentication(ref pinPageUrl);
            if (String.IsNullOrEmpty(rslt))
            {
                using (var ab = new AuthBrowser())
                {
                    ab.Auth = true;
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
                MessageBox.Show(Tween.My_Project.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }
        }

        private bool PinAuth()
        {
            string pin = this._pin;
            string rslt = _tw.Authenticate(pin);
            if (String.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Tween.My_Project.Resources.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK);
                int idx = -1;
                var user = new UserAccount
                {
                    Username = _tw.Username,
                    UserId = _tw.UserId,
                    Token = _tw.AccessToken,
                    TokenSecret = _tw.AccessTokenSecret
                };
                foreach (var u in this.AuthUserCombo.Items)
                {
                    if (((UserAccount)u).Username.ToLower() == _tw.Username.ToLower())
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
                MessageBox.Show(Tween.My_Project.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }
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
            CalcApiUsing();
        }

        private void DisplayApiMaxCount()
        {
            if (MyCommon.TwitterApiInfo.MaxCount > -1)
            {
                LabelApiUsing.Text = string.Format(Tween.My_Project.Resources.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, MyCommon.TwitterApiInfo.MaxCount);
            }
            else
            {
                LabelApiUsing.Text = string.Format(Tween.My_Project.Resources.SettingAPIUse1, MyCommon.TwitterApiInfo.UsingCount, "???");
            }
        }

        private void CalcApiUsing()
        {
            int listsTabNum = 0;
            try
            {
                // 初回起動時などにNothingの場合あり
                listsTabNum = TabInformations.GetInstance().GetTabsByType(Tween.MyCommon.TabUsageType.Lists).Count;
            }
            catch (Exception)
            {
                return;
            }

            int userTimelineTabNum = 0;
            try
            {
                // 初回起動時などにNothingの場合あり
                userTimelineTabNum = TabInformations.GetInstance().GetTabsByType(Tween.MyCommon.TabUsageType.UserTimeline).Count;
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

            if (_tw != null)
            {
                if (MyCommon.TwitterApiInfo.MaxCount == -1)
                {
                    if (Twitter.AccountState == Tween.MyCommon.AccountState.Valid)
                    {
                        MyCommon.TwitterApiInfo.UsingCount = usingApi;
                        var proc = new Thread(new System.Threading.ThreadStart(() =>
                        {
                            _tw.GetInfoApi(null); //'取得エラー時はinfoCountは初期状態（値：-1）
                            if (this.IsHandleCreated && this.IsDisposed)
                            {
                                Invoke(new MethodInvoker(DisplayApiMaxCount));
                            }
                        }));
                        proc.Start();
                    }
                    else
                    {
                        LabelApiUsing.Text = string.Format(Tween.My_Project.Resources.SettingAPIUse1, usingApi, "???");
                    }
                }
                else
                {
                    LabelApiUsing.Text = string.Format(Tween.My_Project.Resources.SettingAPIUse1, usingApi, MyCommon.TwitterApiInfo.MaxCount);
                }
            }

            LabelPostAndGet.Visible = CheckPostAndGet.Checked && !_tw.UserStreamEnabled;
            LabelUserStreamActive.Visible = _tw.UserStreamEnabled;

            LabelApiUsingUserStreamEnabled.Text = String.Format(Tween.My_Project.Resources.SettingAPIUse2, (apiLists + apiUserTimeline).ToString());
            LabelApiUsingUserStreamEnabled.Visible = _tw.UserStreamEnabled;
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
                if (this.Disposing || this.IsDisposed)
                {
                    return;
                }
            } while (!(this.IsHandleCreated));
            this.TopMost = this.AlwaysTop;
            CalcApiUsing();
        }

        private void ButtonApiCalc_Click(object sender, EventArgs e)
        {
            CalcApiUsing();
        }

        public static AppendSettingDialog Instance
        {
            get { return _instance; }
        }

        private bool BitlyValidation(string id, string apikey)
        {
            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(apikey))
            {
                return false;
            }

            string req = "http://api.bit.ly/v3/validate";
            string content = "";
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("login", "tweenapi");
            param.Add("apiKey", "R_c5ee0e30bdfff88723c4457cc331886b");
            param.Add("x_login", id);
            param.Add("x_apiKey", apikey);
            param.Add("format", "txt");

            if (!(new HttpVarious()).PostData(req, param, ref content))
            {
                return true;
                // 通信エラーの場合はとりあえずチェックを通ったことにする
            }
            else if (content.Trim() == "1")
            {
                return true;
                // 検証成功
            }
            else if (content.Trim() == "0")
            {
                return false;
                // 検証失敗 APIキーとIDの組み合わせが違う
            }
            else
            {
                return true;
                // 規定外応答：通信エラーの可能性があるためとりあえずチェックを通ったことにする
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _ValidationError = false;
        }

        public bool HotkeyEnabled { get; set; }

        public Keys HotkeyKey { get; set; }

        public int HotkeyValue { get; set; }

        public Keys HotkeyMod { get; set; }

        private void HotkeyText_KeyDown(object sender, KeyEventArgs e)
        {
            //KeyValueで判定する。
            //表示文字とのテーブルを用意すること
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

        public bool BlinkNewMentions { get; set; }

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = int.Parse(GetMoreTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 100))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextSearchCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
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
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (!(cnt == 0) && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Tween.My_Project.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private class EventCheckboxTblElement
        {
            public CheckBox CheckBox;
            public Tween.MyCommon.EventType Type;
        }
        private EventCheckboxTblElement[] eventCheckboxTableElements = null;

        private EventCheckboxTblElement[] GetEventCheckboxTable()
        {
            if (eventCheckboxTableElements == null)
            {
                eventCheckboxTableElements = new EventCheckboxTblElement[] { 
                    new EventCheckboxTblElement { CheckBox = CheckFavoritesEvent, Type = Tween.MyCommon.EventType.Favorite }, 
                    new EventCheckboxTblElement { CheckBox = CheckUnfavoritesEvent, Type = Tween.MyCommon.EventType.Unfavorite }, 
                    new EventCheckboxTblElement { CheckBox = CheckFollowEvent, Type = Tween.MyCommon.EventType.Follow }, 
                    new EventCheckboxTblElement { CheckBox = CheckListMemberAddedEvent, Type = Tween.MyCommon.EventType.ListMemberAdded },
                    new EventCheckboxTblElement { CheckBox = CheckListMemberRemovedEvent, Type = Tween.MyCommon.EventType.ListMemberRemoved },
                    new EventCheckboxTblElement { CheckBox = CheckBlockEvent, Type = Tween.MyCommon.EventType.Block }, 
                    new EventCheckboxTblElement { CheckBox = CheckUserUpdateEvent, Type = Tween.MyCommon.EventType.UserUpdate }, 
                    new EventCheckboxTblElement { CheckBox = CheckListCreatedEvent, Type = Tween.MyCommon.EventType.ListCreated } 
                };
            }
            return eventCheckboxTableElements;
        }

        private void GetEventNotifyFlag(ref Tween.MyCommon.EventType eventnotifyflag, ref Tween.MyCommon.EventType isMyeventnotifyflag)
        {
            MyCommon.EventType evt = MyCommon.EventType.None;
            MyCommon.EventType myevt = MyCommon.EventType.None;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
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
                    //
                }
            }
            eventnotifyflag = evt;
            isMyeventnotifyflag = myevt;
        }

        private void ApplyEventNotifyFlag(bool rootEnabled, Tween.MyCommon.EventType eventnotifyflag, Tween.MyCommon.EventType isMyeventnotifyflag)
        {
            var evt = eventnotifyflag;
            var myevt = isMyeventnotifyflag;

            CheckEventNotify.Checked = rootEnabled;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
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
            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                tbl.CheckBox.Enabled = CheckEventNotify.Checked;
            }
        }

        private void SoundFileListup()
        {
            if (_MyEventSoundFile == null)
            {
                _MyEventSoundFile = "";
            }
            ComboBoxEventNotifySound.Items.Clear();
            ComboBoxEventNotifySound.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Tween.My.MyProject.Application.Info.DirectoryPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Tween.My.MyProject.Application.Info.DirectoryPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboBoxEventNotifySound.Items.Add(oFile.Name);
            }
            int idx = ComboBoxEventNotifySound.Items.IndexOf(_MyEventSoundFile);
            if (idx == -1)
            {
                idx = 0;
            }
            ComboBoxEventNotifySound.SelectedIndex = idx;
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !String.IsNullOrEmpty(UserAppointUrlText.Text))
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
                if (!String.IsNullOrEmpty(BrowserPath))
                {
                    if (path.StartsWith("\"") && path.Length > 2 && path.IndexOf("\"", 2) > -1)
                    {
                        int sep = path.IndexOf("\"", 2);
                        string browserPath = path.Substring(1, sep - 1);
                        string arg = "";
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
                //                MessageBox.Show("ブラウザの起動に失敗、またはタイムアウトしました。" + ex.ToString())
            }
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        public AppendSettingDialog()
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            this.Icon = Tween.My_Project.Resources.MIcon;
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }
    }
}