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

// コンパイル後コマンド
// "c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /h2 /a:"$(TargetPath)"
// "C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /h2 /a:"$(TargetPath)"

namespace Hoehoe
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using TweenCustomControl;
    using R = Properties.Resources;

    public partial class TweenMain
    {
        #region private fields

        private static int _accountCheckErrorCount;

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/FrontPage";

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationShortcutKeyHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        private readonly string[] _twitterSearchLangs = { "ja", "en", "ar", "da", "nl", "fa", "fi", "fr", "de", "hu", "is", "it", "no", "pl", "pt", "ru", "es", "sv", "th" };

        // ロック用
        private readonly object _syncObject = new object();

        /* 各種設定 */
        private Size _mySize; // 画面サイズ
        private Point _myLoc; // 画面位置
        private int _mySpDis; // 区切り位置
        private int _mySpDis2; // 発言欄区切り位置
        private int _mySpDis3; // プレビュー区切り位置
        private int _myAdSpDis; // Ad区切り位置
        private int _iconSz; // アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private bool _iconCol; // 1列表示の時True（48サイズのとき）

        /* 雑多なフラグ類 */
        private bool _isInitializing; // True:起動時処理中
        private bool _initialLayout = true; // True:起動時処理中
        private bool _ignoreConfigSave;
        private bool _tabDraging; // タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private TabPage _prevSelectedTab; // タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private Point _tabMouseDownPoint;
        private string _rclickTabName; // 右クリックしたタブの名前（Tabコントロール機能不足対応）

        private string _detailHtmlFormatHeader;
        private string _detailHtmlFormatFooter;
        private bool _myStatusError;
        private bool _soundfileListup;

        private SpaceKeyCanceler _spaceKeyCanceler;

        /* 設定関連 */
        private readonly Configs _configs = Configs.Instance;
        private SettingLocal _cfgLocal;
        private SettingCommon _cfgCommon;
        private bool _modifySettingLocal;
        private bool _modifySettingCommon;
        private bool _modifySettingAtId;

        // twitter解析部
        private readonly Twitter _tw = new Twitter();

        // Growl呼び出し部
        private readonly GrowlHelper _growlHelper;

        /* サブ画面インスタンス */
        private readonly AppendSettingDialog _settingDialog = new AppendSettingDialog(); // 設定画面インスタンス
        private readonly TabsDialog _tabDialog = new TabsDialog();      // タブ選択ダイアログインスタンス
        private readonly SearchWord _searchDialog = new SearchWord();   // 検索画面インスタンス
        private readonly FilterDialog _fltDialog = new FilterDialog();  // フィルター編集画面
        private readonly OpenURL _urlDialog = new OpenURL();
        private TweenAboutBox _aboutBox;
        private EventViewerDialog _evtDialog;

        /* 表示フォント、色、アイコン */
        private Font _fntUnread; // 未読用フォント
        private Color _clrUnread; // 未読用文字色
        private Font _fntReaded; // 既読用フォント
        private Color _clrRead; // 既読用文字色
        private Color _clrFav; // Fav用文字色
        private Color _clrOwl; // 片思い用文字色
        private Color _clrRetweet; // Retweet用文字色
        private Font _fntDetail; // 発言詳細部用フォント
        private Color _clrDetail; // 発言詳細部用色
        private Color _clrDetailLink; // 発言詳細部用リンク文字色
        private Color _clrDetailBackcolor; // 発言詳細部用背景色
        private Color _clrSelf; // 自分の発言用背景色
        private Color _clrAtSelf; // 自分宛返信用背景色
        private Color _clrTarget; // 選択発言者の他の発言用背景色
        private Color _clrAtTarget; // 選択発言中の返信先用背景色
        private Color _clrAtFromTarget; // 選択発言者への返信発言用背景色
        private Color _clrAtTo; // 選択発言の唯一＠先
        private Color _clrListBackcolor; // リスト部通常発言背景色
        private Color _clrInputBackcolor; // 入力欄背景色
        private Color _clrInputForecolor; // 入力欄文字色
        private Font _fntInputFont; // 入力欄フォント

        /* アイコン画像リスト */
        private ImageDictionary _iconDict;
        private Icon _iconAt;         // At.ico             タスクトレイアイコン：通常時
        private Icon _iconAtRed;      // AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon _iconAtSmoke;    // AtSmoke.ico        タスクトレイアイコン：オフライン時
        private readonly Icon[] _iconRefresh = new Icon[4]; // Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon _tabIcon;        // Tab.ico            未読のあるタブ用アイコン
        private Icon _mainIcon;       // Main.ico           画面左上のアイコン
        private Icon _replyIcon;      // 5g
        private Icon _replyIconBlink; // 6g

        private PostClass _anchorPost;

        // True:関連発言移動中（関連移動以外のオペレーションをするとFalseへ。Trueだとリスト背景色をアンカー発言選択中として描画）
        private bool _anchorFlag;

        // 発言履歴
        private readonly List<PostingStatus> _postHistory = new List<PostingStatus>();

        // 発言履歴カレントインデックス
        private int _postHistoryIndex;

        // 発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
        // リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
        private long _replyToId;

        // リプライ先ステータスの書き込み者の名前
        private string _replyToName;

        /* 時速表示用 */
        private readonly List<DateTime> _postTimestamps = new List<DateTime>();
        private readonly List<DateTime> _favTimestamps = new List<DateTime>();
        private readonly Dictionary<DateTime, int> _timeLineTimestamps = new Dictionary<DateTime, int>();
        private int _timeLineCount;

        /* 以下DrawItem関連 */
        private readonly SolidBrush _brsHighLight = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private readonly SolidBrush _brsHighLightText = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
        private SolidBrush _brsForeColorUnread;
        private SolidBrush _brsForeColorReaded;
        private SolidBrush _brsForeColorFav;
        private SolidBrush _brsForeColorOwl;
        private SolidBrush _brsForeColorRetweet;
        private SolidBrush _brsBackColorMine;
        private SolidBrush _brsBackColorAt;
        private SolidBrush _brsBackColorYou;
        private SolidBrush _brsBackColorAtYou;
        private SolidBrush _brsBackColorAtFromTarget;
        private SolidBrush _brsBackColorAtTo;
        private SolidBrush _brsBackColorNone;

        // Listにフォーカスないときの選択行の背景色
        private readonly SolidBrush _brsDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace));

        private readonly StringFormat _tabStringFormat = new StringFormat();

        private readonly ToolStripAPIGauge _apiGauge = new ToolStripAPIGauge();

        private TabInformations _statuses;
        private ListViewItem[] _itemCache;
        private int _itemCacheIndex;
        private PostClass[] _postCache;
        private TabPage _curTab;
        private int _curItemIndex;
        private DetailsListView _curList;
        private PostClass _curPost;
        private bool _isColumnChanged;
        private bool _waitTimeline;
        private bool _waitReply;
        private bool _waitDm;
        private bool _waitFav;
        private bool _waitPubSearch;
        private bool _waitUserTimeline;
        private bool _waitLists;
        private readonly BackgroundWorker[] _bworkers = new BackgroundWorker[19];
        private BackgroundWorker _followerFetchWorker;
        private readonly ShieldIcon _shield = new ShieldIcon();
        private InternetSecurityManager _securityManager;
        private readonly Thumbnail _thumbnail;
        private int _unreadCounter = -1;
        private int _unreadAtCounter = -1;
        private readonly string[] _columnOrgTexts = new string[9];
        private readonly string[] _columnTexts = new string[9];
        private bool _doFavRetweetFlags;
        private bool _isOsResumed;
        private Dictionary<string, IMultimediaShareService> _pictureServices;
        private string _postBrowserStatusText = string.Empty;
        private bool _colorize;
        private readonly System.Timers.Timer _timerTimeline;
        private ImageListViewItem _displayItem;
        private List<UrlUndoInfo> _urlUndoBuffer;

        // [, ]でのリプライ移動の履歴
        private Stack<ReplyChain> _replyChains;

        // ポスト選択履歴
        private readonly Stack<Tuple<TabPage, PostClass>> _selectPostChains = new Stack<Tuple<TabPage, PostClass>>();

        /* タイマー系 */
        private IntervalChangedEventArgs _resetTimers = new IntervalChangedEventArgs();
        private int _timerHomeCounter;
        private int _timerMentionCounter;
        private int _timerDmCounter;
        private int _timerPubSearchCounter;
        private int _timerUserTimelineCounter;
        private int _timerListsCounter;
        private int _timerUsCounter;
        private int _timerResumeWait;
        private int _timerRefreshFollowers;
        private Dictionary<WorkerType, DateTime> _lastTimeWork;

        private PostClass _displayPost;
        private int _iconCnt;
        private int _blinkCnt;
        private bool _doBlink;
        private bool _isIdle;
        private long _prevFollowerCount;
        private readonly HookGlobalHotkey _hookGlobalHotkey;
        private bool _isActiveUserstream;
        private string _prevTrackWord = string.Empty;

        #endregion private fields

        #region constructor

        public TweenMain()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            _hookGlobalHotkey = new HookGlobalHotkey(this);
            _hookGlobalHotkey.HotkeyPressed += HookGlobalHotkey_HotkeyPressed;
            _settingDialog.IntervalChanged += TimerInterval_Changed;
            _apiGauge.Control.Size = new Size(70, 22);
            _apiGauge.Control.Margin = new Padding(0, 3, 0, 2);
            _apiGauge.GaugeHeight = 8;
            _apiGauge.Control.DoubleClick += ApiInfoMenuItem_Click;
            StatusStrip1.Items.Insert(2, _apiGauge);
            _growlHelper = new GrowlHelper("Hoehoe");
            _growlHelper.NotifyClicked += GrowlHelper_Callback;
            _timerTimeline = new System.Timers.Timer();
            _timerTimeline.Elapsed += TimerTimeline_Elapsed;

            _securityManager = new InternetSecurityManager(PostBrowser);
            _thumbnail = new Thumbnail(this);
            MyCommon.TwitterApiInfo.Changed += TwitterApiInfo_Changed;
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        #endregion constructor

        #region delegates

        public delegate void SetStatusLabelApiDelegate();

        #endregion delegates

        #region properties

        public Color InputBackColor { get; set; }

        public Twitter TwitterInstance
        {
            get { return _tw; }
        }

        public PostClass CurPost
        {
            get { return _curPost; }
        }

        public bool IsPreviewEnable
        {
            get { return _configs.PreviewEnable; }
        }

        public bool FavEventChangeUnread
        {
            get { return _configs.FavEventUnread; }
        }

        /// <summary>
        /// @id補助
        /// </summary>
        public AtIdSupplement AtIdSupl { get; set; }

        /// <summary>
        /// Hashtag補助
        /// </summary>
        public AtIdSupplement HashSupl { get; set; }

        public HashtagManage HashMgr { get; set; }

        private string ImageService
        {
            get { return Convert.ToString(ImageServiceCombo.SelectedItem); }
        }

        private bool ExistCurrentPost
        {
            get { return _curPost != null && !_curPost.IsDeleted; }
        }

        #endregion properties

        #region public methods

        public void AddNewTabForSearch(string searchWord)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in _statuses.GetTabsByType(TabUsageType.PublicSearch))
            {
                if (tb.SearchWords == searchWord && string.IsNullOrEmpty(tb.SearchLang))
                {
                    foreach (TabPage tp in ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }

            // ユニークなタブ名生成
            string tabName = searchWord;
            for (int i = 0; i <= 100; i++)
            {
                if (_statuses.ContainsTab(tabName))
                {
                    tabName += "_";
                }
                else
                {
                    break;
                }
            }

            // タブ追加
            _statuses.AddTab(tabName, TabUsageType.PublicSearch, null);
            AddNewTab(tabName, false, TabUsageType.PublicSearch);

            // 追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;

            // 検索条件の設定
            ComboBox cmb = (ComboBox)ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            SaveConfigsTabs();

            // 検索実行
            SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
        }

        public void AddNewTabForUserTimeline(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in _statuses.GetTabsByType(TabUsageType.UserTimeline))
            {
                if (tb.User == user)
                {
                    foreach (TabPage tp in ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }

            // ユニークなタブ名生成
            string tabName = "user:" + user;
            while (_statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }

            // タブ追加
            _statuses.AddTab(tabName, TabUsageType.UserTimeline, null);
            _statuses.Tabs[tabName].User = user;
            AddNewTab(tabName, false, TabUsageType.UserTimeline);

            // 追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            SaveConfigsTabs();

            // 検索実行
            GetTimeline(WorkerType.UserTimeline, 1, 0, tabName);
        }

        private Panel CreateSearchPanel(string tabName)
        {
            Panel pnl = new Panel();
            Label lbl = new Label();
            ComboBox cmb = new ComboBox();
            Button btn = new Button();
            ComboBox cmbLang = new ComboBox();

            pnl.SuspendLayout();
            pnl.Controls.Add(cmb);
            pnl.Controls.Add(cmbLang);
            pnl.Controls.Add(btn);
            pnl.Controls.Add(lbl);
            pnl.Name = "panelSearch";
            pnl.Dock = DockStyle.Top;
            pnl.Height = cmb.Height;
            pnl.Enter += SearchControls_Enter;
            pnl.Leave += SearchControls_Leave;

            cmb.Text = string.Empty;
            cmb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmb.Dock = DockStyle.Fill;
            cmb.Name = "comboSearch";
            cmb.DropDownStyle = ComboBoxStyle.DropDown;
            cmb.ImeMode = ImeMode.NoControl;
            cmb.TabStop = false;
            cmb.AutoCompleteMode = AutoCompleteMode.None;
            cmb.KeyDown += SearchComboBox_KeyDown;

            if (_statuses.ContainsTab(tabName))
            {
                cmb.Items.Add(_statuses.Tabs[tabName].SearchWords);
                cmb.Text = _statuses.Tabs[tabName].SearchWords;
            }

            cmbLang.Text = string.Empty;
            cmbLang.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbLang.Dock = DockStyle.Right;
            cmbLang.Width = 50;
            cmbLang.Name = "comboLang";
            cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLang.TabStop = false;
            cmbLang.Items.Add(string.Empty);
            cmbLang.Items.AddRange(_twitterSearchLangs);
            if (_statuses.ContainsTab(tabName))
            {
                cmbLang.Text = _statuses.Tabs[tabName].SearchLang;
            }

            lbl.Text = "Search(C-S-f)";
            lbl.Name = "label1";
            lbl.Dock = DockStyle.Left;
            lbl.Width = 90;
            lbl.Height = cmb.Height;
            lbl.TextAlign = ContentAlignment.MiddleLeft;

            btn.Text = "Search";
            btn.Name = "buttonSearch";
            btn.UseVisualStyleBackColor = true;
            btn.Dock = DockStyle.Right;
            btn.TabStop = false;
            btn.Click += SearchButton_Click;
            return pnl;
        }

        private DetailsListView CreateDetailListView(string tabName, bool startup)
        {
            InitColumnText();
            var colhds = new ColumnHeader[8]; // アイコン, ニックネーム, 本文, 日付, ユーザID, 未読, マーク＆プロテクト, ソース
            var widths = new[] { 48, 80, 300, 50, 50, 16, 16, 50 };
            for (var i = 0; i < colhds.Length; ++i)
            {
                colhds[i] = new ColumnHeader
                {
                    Width = widths[i],
                    Text = _columnTexts[i]
                };
            }

            DetailsListView listCustom = new DetailsListView
            {
                ContextMenuStrip = ContextMenuOperate,
                Font = _fntReaded,
                BackColor = _clrListBackcolor,
                GridLines = _configs.ShowGrid
            };

            var sz = _iconSz > 0 ? _iconSz : 1;
            listCustom.SmallImageList = new ImageList { ImageSize = new Size(sz, sz) };
            listCustom.Columns.AddRange(_iconCol ?
                new[] { colhds[0], colhds[2] } :
                new[] { colhds[0], colhds[1], colhds[2], colhds[3], colhds[4], colhds[5], colhds[6], colhds[7] });

            int[] dispOrder = new int[8];
            if (!startup)
            {
                for (int i = 0; i < _curList.Columns.Count; i++)
                {
                    for (int j = 0; j < _curList.Columns.Count; j++)
                    {
                        if (_curList.Columns[j].DisplayIndex == i)
                        {
                            dispOrder[i] = j;
                            break;
                        }
                    }
                }

                for (int i = 0; i < _curList.Columns.Count; i++)
                {
                    listCustom.Columns[i].Width = _curList.Columns[i].Width;
                    listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                }
            }
            else
            {
                if (_iconCol)
                {
                    listCustom.Columns[0].Width = _cfgLocal.Width1;
                    listCustom.Columns[1].Width = _cfgLocal.Width3;
                    listCustom.Columns[0].DisplayIndex = 0;
                    listCustom.Columns[1].DisplayIndex = 1;
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (_cfgLocal.DisplayIndex1 == i)
                        {
                            dispOrder[i] = 0;
                        }
                        else if (_cfgLocal.DisplayIndex2 == i)
                        {
                            dispOrder[i] = 1;
                        }
                        else if (_cfgLocal.DisplayIndex3 == i)
                        {
                            dispOrder[i] = 2;
                        }
                        else if (_cfgLocal.DisplayIndex4 == i)
                        {
                            dispOrder[i] = 3;
                        }
                        else if (_cfgLocal.DisplayIndex5 == i)
                        {
                            dispOrder[i] = 4;
                        }
                        else if (_cfgLocal.DisplayIndex6 == i)
                        {
                            dispOrder[i] = 5;
                        }
                        else if (_cfgLocal.DisplayIndex7 == i)
                        {
                            dispOrder[i] = 6;
                        }
                        else if (_cfgLocal.DisplayIndex8 == i)
                        {
                            dispOrder[i] = 7;
                        }
                    }

                    listCustom.Columns[0].Width = _cfgLocal.Width1;
                    listCustom.Columns[1].Width = _cfgLocal.Width2;
                    listCustom.Columns[2].Width = _cfgLocal.Width3;
                    listCustom.Columns[3].Width = _cfgLocal.Width4;
                    listCustom.Columns[4].Width = _cfgLocal.Width5;
                    listCustom.Columns[5].Width = _cfgLocal.Width6;
                    listCustom.Columns[6].Width = _cfgLocal.Width7;
                    listCustom.Columns[7].Width = _cfgLocal.Width8;
                    for (int i = 0; i < 8; i++)
                    {
                        listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                    }
                }
            }

            return listCustom;
        }

        public bool AddNewTab(string tabName, bool startup, TabUsageType tabType, ListElement listInfo = null)
        {
            // 重複チェック
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    return false;
                }
            }

            // 新規タブ名チェック
            if (tabName == R.AddNewTabText1)
            {
                return false;
            }

            // タブタイプ重複チェック
            if (!startup)
            {
                if (tabType == TabUsageType.DirectMessage
                    || tabType == TabUsageType.Favorites
                    || tabType == TabUsageType.Home
                    || tabType == TabUsageType.Mentions
                    || tabType == TabUsageType.Related)
                {
                    if (_statuses.GetTabByType(tabType) != null)
                    {
                        return false;
                    }
                }
            }

            var tabPage = new TabPage();

            int cnt = ListTab.TabPages.Count;

            //// ToDo:Create and set controls follow tabtypes

            SplitContainer1.Panel1.SuspendLayout();
            SplitContainer1.Panel2.SuspendLayout();
            SplitContainer1.SuspendLayout();
            ListTab.SuspendLayout();
            SuspendLayout();

            tabPage.SuspendLayout();

            /// UserTimeline関連
            Label label = null;
            if (tabType == TabUsageType.UserTimeline || tabType == TabUsageType.Lists)
            {
                label = new Label();
                label.Dock = DockStyle.Top;
                label.Name = "labelUser";
                label.Text = tabType == TabUsageType.Lists ? listInfo.ToString() : _statuses.Tabs[tabName].User + "'s Timeline";
                label.TextAlign = ContentAlignment.MiddleLeft;
                using (var tmpComboBox = new ComboBox())
                {
                    label.Height = tmpComboBox.Height;
                }
            }

            ListTab.Controls.Add(tabPage);
            var listCustom = CreateDetailListView(tabName, startup);
            listCustom.SelectedIndexChanged += MyList_SelectedIndexChanged;
            listCustom.MouseDoubleClick += MyList_MouseDoubleClick;
            listCustom.ColumnClick += MyList_ColumnClick;
            listCustom.DrawColumnHeader += MyList_DrawColumnHeader;
            listCustom.DragDrop += TweenMain_DragDrop;
            listCustom.DragOver += TweenMain_DragOver;
            listCustom.DrawItem += MyList_DrawItem;
            listCustom.MouseClick += MyList_MouseClick;
            listCustom.ColumnReordered += MyList_ColumnReordered;
            listCustom.ColumnWidthChanged += MyList_ColumnWidthChanged;
            listCustom.CacheVirtualItems += MyList_CacheVirtualItems;
            listCustom.RetrieveVirtualItem += MyList_RetrieveVirtualItem;
            listCustom.DrawSubItem += MyList_DrawSubItem;
            listCustom.HScrolled += MyList_HScrolled;
            tabPage.Controls.Add(listCustom);

            if (_statuses.IsDistributableTab(tabName))
            {
                _tabDialog.AddTab(tabName);
            }

            /// 検索関連の準備
            Panel pnl = null;
            if (tabType == TabUsageType.PublicSearch)
            {
                pnl = CreateSearchPanel(tabName);
                tabPage.Controls.Add(pnl);
            }

            if (tabType == TabUsageType.UserTimeline || tabType == TabUsageType.Lists)
            {
                tabPage.Controls.Add(label);
            }

            tabPage.Location = new Point(4, 4);
            tabPage.Name = "CTab" + cnt.ToString();
            tabPage.Size = new Size(380, 260);
            tabPage.TabIndex = 2 + cnt;
            tabPage.Text = tabName;
            tabPage.UseVisualStyleBackColor = true;

            if (tabType == TabUsageType.PublicSearch)
            {
                pnl.ResumeLayout(false);
            }

            tabPage.ResumeLayout(false);

            SplitContainer1.Panel1.ResumeLayout(false);
            SplitContainer1.Panel2.ResumeLayout(false);
            SplitContainer1.ResumeLayout(false);
            ListTab.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
            tabPage.Tag = listCustom;
            return true;
        }

        public bool RemoveSpecifiedTab(string tabName, bool confirm)
        {
            int idx = 0;
            for (idx = 0; idx < ListTab.TabPages.Count; idx++)
            {
                if (ListTab.TabPages[idx].Text == tabName)
                {
                    break;
                }
            }

            if (_statuses.IsDefaultTab(tabName))
            {
                return false;
            }

            if (confirm)
            {
                string tmp = string.Format(R.RemoveSpecifiedTabText1, Environment.NewLine);
                var result = MessageBox.Show(tmp, tabName + " " + R.RemoveSpecifiedTabText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            SetListProperty(); // 他のタブに列幅等を反映

            TabUsageType tabType = _statuses.Tabs[tabName].TabType;

            // オブジェクトインスタンスの削除
            SplitContainer1.Panel1.SuspendLayout();
            SplitContainer1.Panel2.SuspendLayout();
            SplitContainer1.SuspendLayout();
            ListTab.SuspendLayout();
            SuspendLayout();

            TabPage tabPage = ListTab.TabPages[idx];
            DetailsListView listCustom = (DetailsListView)tabPage.Tag;
            tabPage.Tag = null;
            tabPage.SuspendLayout();

            if (ReferenceEquals(ListTab.SelectedTab, ListTab.TabPages[idx]))
            {
                ListTab.SelectTab(_prevSelectedTab != null && ListTab.TabPages.Contains(_prevSelectedTab) ? _prevSelectedTab : ListTab.TabPages[0]);
            }

            ListTab.Controls.Remove(tabPage);

            Control pnl = null;
            if (tabType == TabUsageType.PublicSearch)
            {
                pnl = tabPage.Controls["panelSearch"];
                foreach (Control ctrl in pnl.Controls)
                {
                    if (ctrl.Name == "buttonSearch")
                    {
                        ctrl.Click -= SearchButton_Click;
                    }

                    ctrl.Enter -= SearchControls_Enter;
                    ctrl.Leave -= SearchControls_Leave;
                    pnl.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }

                tabPage.Controls.Remove(pnl);
            }

            tabPage.Controls.Remove(listCustom);
            listCustom.Columns.Clear();
            listCustom.ContextMenuStrip = null;

            listCustom.SelectedIndexChanged -= MyList_SelectedIndexChanged;
            listCustom.MouseDoubleClick -= MyList_MouseDoubleClick;
            listCustom.ColumnClick -= MyList_ColumnClick;
            listCustom.DrawColumnHeader -= MyList_DrawColumnHeader;
            listCustom.DragDrop -= TweenMain_DragDrop;
            listCustom.DragOver -= TweenMain_DragOver;
            listCustom.DrawItem -= MyList_DrawItem;
            listCustom.MouseClick -= MyList_MouseClick;
            listCustom.ColumnReordered -= MyList_ColumnReordered;
            listCustom.ColumnWidthChanged -= MyList_ColumnWidthChanged;
            listCustom.CacheVirtualItems -= MyList_CacheVirtualItems;
            listCustom.RetrieveVirtualItem -= MyList_RetrieveVirtualItem;
            listCustom.DrawSubItem -= MyList_DrawSubItem;
            listCustom.HScrolled -= MyList_HScrolled;

            _tabDialog.RemoveTab(tabName);

            listCustom.SmallImageList = null;
            listCustom.ListViewItemSorter = null;

            // キャッシュのクリア
            if (_curTab.Equals(tabPage))
            {
                _curTab = null;
                _curItemIndex = -1;
                _curList = null;
                _curPost = null;
            }

            _itemCache = null;
            _itemCacheIndex = -1;
            _postCache = null;

            tabPage.ResumeLayout(false);

            SplitContainer1.Panel1.ResumeLayout(false);
            SplitContainer1.Panel2.ResumeLayout(false);
            SplitContainer1.ResumeLayout(false);
            ListTab.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

            tabPage.Dispose();
            listCustom.Dispose();
            _statuses.RemoveTab(tabName);

            foreach (TabPage tp in ListTab.TabPages)
            {
                var lst = (DetailsListView)tp.Tag;
                lst.VirtualListSize = _statuses.Tabs[tp.Text].AllCount;
            }

            return true;
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset = 0, string startswith = "")
        {
            dialog.StartsWith = startswith;
            if (dialog.Visible)
            {
                dialog.Focus();
            }
            else
            {
                dialog.ShowDialog();
            }

            TopMost = _configs.AlwaysTop;
            int selStart = owner.SelectionStart;
            string frontHalf = string.Empty;
            string lastHalf = string.Empty;
            if (dialog.DialogResult == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.InputText))
                {
                    if (selStart > 0)
                    {
                        frontHalf = owner.Text.Substring(0, selStart - offset);
                    }

                    if (selStart < owner.Text.Length)
                    {
                        lastHalf = owner.Text.Substring(selStart);
                    }

                    owner.Text = frontHalf + dialog.InputText + lastHalf;
                    owner.SelectionStart = selStart + dialog.InputText.Length;
                }
            }
            else
            {
                if (selStart > 0)
                {
                    frontHalf = owner.Text.Substring(0, selStart);
                }

                if (selStart < owner.Text.Length)
                {
                    lastHalf = owner.Text.Substring(selStart);
                }

                owner.Text = frontHalf + lastHalf;
                if (selStart > 0)
                {
                    owner.SelectionStart = selStart;
                }
            }

            owner.Focus();
        }

        public string CreateDetailHtml(string orgdata)
        {
            return _detailHtmlFormatHeader + orgdata + _detailHtmlFormatFooter;
        }

        public bool RenameTab(ref string tabName)
        {
            // タブ名変更
            string newTabText = tabName;
            if (!TryUserInputText(ref newTabText))
            {
                return false;
            }

            TopMost = _configs.AlwaysTop;
            if (string.IsNullOrEmpty(newTabText))
            {
                return false;
            }

            // 新タブ名存在チェック
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (ListTab.TabPages[i].Text == newTabText)
                {
                    string tmp = string.Format(R.Tabs_DoubleClickText1, newTabText);
                    MessageBox.Show(tmp, R.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            // タブ名のリスト作り直し（デフォルトタブ以外は再作成）
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (_statuses.IsDistributableTab(ListTab.TabPages[i].Text))
                {
                    _tabDialog.RemoveTab(ListTab.TabPages[i].Text);
                }

                if (ListTab.TabPages[i].Text == tabName)
                {
                    ListTab.TabPages[i].Text = newTabText;
                }
            }

            _statuses.RenameTab(tabName, newTabText);
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (_statuses.IsDistributableTab(ListTab.TabPages[i].Text))
                {
                    if (ListTab.TabPages[i].Text == tabName)
                    {
                        ListTab.TabPages[i].Text = newTabText;
                    }

                    _tabDialog.AddTab(ListTab.TabPages[i].Text);
                }
            }

            SaveConfigsCommon();
            SaveConfigsTabs();
            _rclickTabName = newTabText;
            tabName = newTabText;
            return true;
        }

        public void ReorderTab(string targetTabText, string baseTabText, bool isBeforeBaseTab)
        {
            int baseIndex = 0;
            for (baseIndex = 0; baseIndex < ListTab.TabPages.Count; baseIndex++)
            {
                if (ListTab.TabPages[baseIndex].Text == baseTabText)
                {
                    break;
                }
            }

            ListTab.SuspendLayout();
            TabPage tp = null;
            for (int j = 0; j < ListTab.TabPages.Count; j++)
            {
                if (ListTab.TabPages[j].Text == targetTabText)
                {
                    tp = ListTab.TabPages[j];
                    ListTab.TabPages.Remove(tp);
                    if (j < baseIndex)
                    {
                        baseIndex -= 1;
                    }

                    break;
                }
            }

            if (isBeforeBaseTab)
            {
                ListTab.TabPages.Insert(baseIndex, tp);
            }
            else
            {
                ListTab.TabPages.Insert(baseIndex + 1, tp);
            }

            ListTab.ResumeLayout();
            SaveConfigsTabs();
        }

        public void ChangeTabUnreadManage(string tabName, bool isManage)
        {
            int idx = 0;
            for (idx = 0; idx < ListTab.TabCount; idx++)
            {
                if (ListTab.TabPages[idx].Text == tabName)
                {
                    break;
                }
            }

            _statuses.SetTabUnreadManage(tabName, isManage);
            if (_configs.TabIconDisp)
            {
                ListTab.TabPages[idx].ImageIndex = _statuses.Tabs[tabName].UnreadCount > 0 ? 0 : -1;
            }

            if (_curTab.Text == tabName)
            {
                _itemCache = null;
                _postCache = null;
                _curList.Refresh();
            }

            SetMainWindowTitle();
            SetStatusLabelUrl();
            if (!_configs.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        public void SetStatusLabel(string text)
        {
            StatusLabel.Text = text;
        }

        public string WebBrowser_GetSelectionText(WebBrowser componentInstance)
        {
            // 発言詳細で「選択文字列をコピー」を行う
            // WebBrowserコンポーネントのインスタンスを渡す
            Type typ = componentInstance.ActiveXInstance.GetType();
            object selObj = typ.InvokeMember("selection", BindingFlags.GetProperty, null, componentInstance.Document.DomDocument, null);
            object objRange = selObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, null, selObj, null);
            return (string)objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, null, objRange, null);
        }

        public void OpenUriAsync(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                RunAsync(new GetWorkerArg { WorkerType = WorkerType.OpenUri, Url = uri });
            }
        }

        /// <summary>
        /// TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        /// </summary>
        /// <param name="name">URLから切り出した文字列</param>
        /// <returns></returns>
        public bool IsTwitterId(string name)
        {
            string[] nonUsernames = _configs.TwitterConfiguration.NonUsernamePaths;
            if (nonUsernames == null || nonUsernames.Length == 0)
            {
                return !Regex.Match(name, "^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            }

            return !nonUsernames.Contains(name.ToLower());
        }

        public void SetModifySettingCommon(bool value)
        {
            _modifySettingCommon = value;
        }

        public void SetModifySettingLocal(bool value)
        {
            _modifySettingLocal = value;
        }

        public void SetModifySettingAtId(bool value)
        {
            _modifySettingAtId = value;
        }

        public bool AddAtIdSuplItem(string item)
        {
            if (AtIdSupl.AddItem(item))
            {
                SetModifySettingAtId(true);
                return true;
            }

            return false;
        }

        #endregion public methods

        #region protected methods

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // TextBox1でEnterを押してもビープ音が鳴らないようにする
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                if (StatusText.Focused)
                {
                    bool doNewLine = false;
                    bool doPost = false;

                    // Ctrl+Enter投稿時
                    if (_configs.PostCtrlEnter)
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control)
                            {
                                doNewLine = true;
                            }

                            if ((keyData & Keys.Control) == Keys.Control)
                            {
                                doPost = true;
                            }
                        }
                        else
                        {
                            if ((keyData & Keys.Control) == Keys.Control)
                            {
                                doPost = true;
                            }
                        }

                        // SHift+Enter投稿時
                    }
                    else if (_configs.PostShiftEnter)
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) != Keys.Shift)
                            {
                                doNewLine = true;
                            }

                            if ((keyData & Keys.Shift) == Keys.Shift)
                            {
                                doPost = true;
                            }
                        }
                        else
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift)
                            {
                                doPost = true;
                            }
                        }

                        // Enter投稿時
                    }
                    else
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control)
                            {
                                doNewLine = true;
                            }

                            if (((keyData & Keys.Control) != Keys.Control && (keyData & Keys.Shift) != Keys.Shift) || ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) == Keys.Shift))
                            {
                                doPost = true;
                            }
                        }
                        else
                        {
                            if (((keyData & Keys.Shift) == Keys.Shift) || (((keyData & Keys.Control) != Keys.Control) && ((keyData & Keys.Shift) != Keys.Shift)))
                            {
                                doPost = true;
                            }
                        }
                    }

                    if (doNewLine)
                    {
                        int pos1 = StatusText.SelectionStart;
                        if (StatusText.SelectionLength > 0)
                        {
                            // 選択状態文字列削除
                            StatusText.Text = StatusText.Text.Remove(pos1, StatusText.SelectionLength);
                        }

                        StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine); // 改行挿入
                        StatusText.SelectionStart = pos1 + Environment.NewLine.Length;       // カーソルを改行の次の文字へ移動
                        return true;
                    }

                    if (doPost)
                    {
                        TryPostTweet();
                        return true;
                    }
                }
                else if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch && (ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focused || ListTab.SelectedTab.Controls["panelSearch"].Controls["comboLang"].Focused))
                {
                    SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        #endregion protected methods
    }
}