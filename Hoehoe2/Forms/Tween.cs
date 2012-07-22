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
    using Hoehoe.TweenCustomControl;
    using R = Hoehoe.Properties.Resources;

    public partial class TweenMain
    {
        #region private fields

        private static int accountCheckErrorCount;

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/FrontPage";

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationShortcutKeyHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        private readonly string[] TwitterSearchLangs = { "ja", "en", "ar", "da", "nl", "fa", "fi", "fr", "de", "hu", "is", "it", "no", "pl", "pt", "ru", "es", "sv", "th" };

        // ロック用
        private readonly object syncObject = new object();

        // 各種設定
        private Size mySize; // 画面サイズ

        private Point myLoc; // 画面位置
        private int mySpDis; // 区切り位置
        private int mySpDis2; // 発言欄区切り位置
        private int mySpDis3; // プレビュー区切り位置
        private int myAdSpDis; // Ad区切り位置
        private int iconSz; // アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private bool iconCol; // 1列表示の時True（48サイズのとき）

        // 雑多なフラグ類
        private bool isInitializing; // True:起動時処理中

        private bool initialLayout = true; // True:起動時処理中
        private bool ignoreConfigSave;

        private bool tabDraging; // タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private TabPage prevSelectedTab; // タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private Point tabMouseDownPoint;
        private string rclickTabName; // 右クリックしたタブの名前（Tabコントロール機能不足対応）

        private string detailHtmlFormatHeader;
        private string detailHtmlFormatFooter;
        private bool myStatusError;
        private bool soundfileListup;

        private SpaceKeyCanceler spaceKeyCanceler;

        /// 設定関連
        private Configs configs = Configs.Instance;

        private SettingLocal cfgLocal;
        private SettingCommon cfgCommon;
        private bool modifySettingLocal;
        private bool modifySettingCommon;
        private bool modifySettingAtId;

        // twitter解析部
        private Twitter tw = new Twitter();

        // Growl呼び出し部
        private GrowlHelper growlHelper;

        // サブ画面インスタンス
        private AppendSettingDialog settingDialog = new AppendSettingDialog(); // 設定画面インスタンス

        private TabsDialog tabDialog = new TabsDialog();      // タブ選択ダイアログインスタンス
        private SearchWord searchDialog = new SearchWord();   // 検索画面インスタンス
        private FilterDialog fltDialog = new FilterDialog();  // フィルター編集画面
        private OpenURL urlDialog = new OpenURL();
        private TweenAboutBox aboutBox;
        private EventViewerDialog evtDialog;

        // 表示フォント、色、アイコン
        private Font fntUnread; // 未読用フォント

        private Color clrUnread; // 未読用文字色
        private Font fntReaded; // 既読用フォント
        private Color clrRead; // 既読用文字色
        private Color clrFav; // Fav用文字色
        private Color clrOWL; // 片思い用文字色
        private Color clrRetweet; // Retweet用文字色
        private Font fntDetail; // 発言詳細部用フォント
        private Color clrDetail; // 発言詳細部用色
        private Color clrDetailLink; // 発言詳細部用リンク文字色
        private Color clrDetailBackcolor; // 発言詳細部用背景色
        private Color clrSelf; // 自分の発言用背景色
        private Color clrAtSelf; // 自分宛返信用背景色
        private Color clrTarget; // 選択発言者の他の発言用背景色
        private Color clrAtTarget; // 選択発言中の返信先用背景色
        private Color clrAtFromTarget; // 選択発言者への返信発言用背景色
        private Color clrAtTo; // 選択発言の唯一＠先
        private Color clrListBackcolor; // リスト部通常発言背景色
        private Color clrInputBackcolor; // 入力欄背景色
        private Color clrInputForecolor; // 入力欄文字色
        private Font fntInputFont; // 入力欄フォント

        // アイコン画像リスト
        private ImageDictionary iconDict;

        private Icon iconAt;         // At.ico             タスクトレイアイコン：通常時
        private Icon iconAtRed;      // AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon iconAtSmoke;    // AtSmoke.ico        タスクトレイアイコン：オフライン時
        private Icon[] iconRefresh = new Icon[4]; // Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon tabIcon;        // Tab.ico            未読のあるタブ用アイコン
        private Icon mainIcon;       // Main.ico           画面左上のアイコン
        private Icon replyIcon;      // 5g
        private Icon replyIconBlink; // 6g

        private PostClass anchorPost;

        // True:関連発言移動中（関連移動以外のオペレーションをするとFalseへ。Trueだとリスト背景色をアンカー発言選択中として描画）
        private bool anchorFlag;

        // 発言履歴
        private List<PostingStatus> postHistory = new List<PostingStatus>();

        // 発言履歴カレントインデックス
        private int postHistoryIndex;

        // 発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
        // リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
        private long replyToId;

        // リプライ先ステータスの書き込み者の名前
        private string replyToName;

        // 時速表示用
        private List<DateTime> postTimestamps = new List<DateTime>();

        private List<DateTime> favTimestamps = new List<DateTime>();
        private Dictionary<DateTime, int> timeLineTimestamps = new Dictionary<DateTime, int>();
        private int timeLineCount;

        // 以下DrawItem関連
        private SolidBrush brsHighLight = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));

        private SolidBrush brsHighLightText = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
        private SolidBrush brsForeColorUnread;
        private SolidBrush brsForeColorReaded;
        private SolidBrush brsForeColorFav;
        private SolidBrush brsForeColorOWL;
        private SolidBrush brsForeColorRetweet;
        private SolidBrush brsBackColorMine;
        private SolidBrush brsBackColorAt;
        private SolidBrush brsBackColorYou;
        private SolidBrush brsBackColorAtYou;
        private SolidBrush brsBackColorAtFromTarget;
        private SolidBrush brsBackColorAtTo;
        private SolidBrush brsBackColorNone;

        // Listにフォーカスないときの選択行の背景色
        private SolidBrush brsDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace));

        private StringFormat tabStringFormat = new StringFormat();

        private ToolStripAPIGauge apiGauge = new ToolStripAPIGauge();

        private TabInformations statuses;
        private ListViewItem[] itemCache;
        private int itemCacheIndex;
        private PostClass[] postCache;
        private TabPage curTab;
        private int curItemIndex;
        private DetailsListView curList;
        private PostClass curPost;
        private bool isColumnChanged;
        private bool waitTimeline;
        private bool waitReply;
        private bool waitDm;
        private bool waitFav;
        private bool waitPubSearch;
        private bool waitUserTimeline;
        private bool waitLists;
        private BackgroundWorker[] bworkers = new BackgroundWorker[19];
        private BackgroundWorker followerFetchWorker;
        private ShieldIcon shield = new ShieldIcon();
        private InternetSecurityManager securityManager;
        private Thumbnail thumbnail;
        private int unreadCounter = -1;
        private int unreadAtCounter = -1;
        private string[] columnOrgTexts = new string[9];
        private string[] columnTexts = new string[9];
        private bool doFavRetweetFlags;
        private bool isOsResumed;
        private Dictionary<string, IMultimediaShareService> pictureServices;
        private string postBrowserStatusText = string.Empty;
        private bool colorize;
        private System.Timers.Timer timerTimeline;
        private ImageListViewItem displayItem;
        private List<UrlUndoInfo> urlUndoBuffer;

        // [, ]でのリプライ移動の履歴
        private Stack<ReplyChain> replyChains;

        // ポスト選択履歴
        private Stack<Tuple<TabPage, PostClass>> selectPostChains = new Stack<Tuple<TabPage, PostClass>>();

        // タイマー系
        private IntervalChangedEventArgs resetTimers = new IntervalChangedEventArgs();

        private int timerHomeCounter;
        private int timerMentionCounter;
        private int timerDmCounter;
        private int timerPubSearchCounter;
        private int timerUserTimelineCounter;
        private int timerListsCounter;
        private int timerUsCounter;
        private int timerResumeWait;
        private int timerRefreshFollowers;
        private Dictionary<WorkerType, DateTime> lastTimeWork;

        private PostClass displayPost;
        private int iconCnt;
        private int blinkCnt;
        private bool doBlink;
        private bool isIdle;
        private long prevFollowerCount = 0;
        private HookGlobalHotkey hookGlobalHotkey;
        private bool isActiveUserstream = false;
        private string prevTrackWord = string.Empty;

        #endregion private fields

        #region constructor

        public TweenMain()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            this.InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            this.hookGlobalHotkey = new HookGlobalHotkey(this);
            this.hookGlobalHotkey.HotkeyPressed += this.HookGlobalHotkey_HotkeyPressed;
            this.settingDialog.IntervalChanged += this.TimerInterval_Changed;
            this.apiGauge.Control.Size = new Size(70, 22);
            this.apiGauge.Control.Margin = new Padding(0, 3, 0, 2);
            this.apiGauge.GaugeHeight = 8;
            this.apiGauge.Control.DoubleClick += this.ApiInfoMenuItem_Click;
            this.StatusStrip1.Items.Insert(2, this.apiGauge);
            this.growlHelper = new GrowlHelper("Hoehoe");
            this.growlHelper.NotifyClicked += this.GrowlHelper_Callback;
            this.timerTimeline = new System.Timers.Timer();
            this.timerTimeline.Elapsed += this.TimerTimeline_Elapsed;

            this.securityManager = new InternetSecurityManager(this.PostBrowser);
            this.thumbnail = new Thumbnail(this);
            MyCommon.TwitterApiInfo.Changed += this.TwitterApiInfo_Changed;
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;
        }

        #endregion constructor

        #region delegates

        public delegate void SetStatusLabelApiDelegate();

        #endregion delegates

        #region properties

        public Color InputBackColor { get; set; }

        public Twitter TwitterInstance
        {
            get { return this.tw; }
        }

        public PostClass CurPost
        {
            get { return this.curPost; }
        }

        public bool IsPreviewEnable
        {
            get { return this.configs.PreviewEnable; }
        }

        public bool FavEventChangeUnread
        {
            get { return this.configs.FavEventUnread; }
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
            get { return Convert.ToString(this.ImageServiceCombo.SelectedItem); }
        }

        private bool ExistCurrentPost
        {
            get { return this.curPost != null && !this.curPost.IsDeleted; }
        }

        #endregion properties

        #region public methods

        public void AddNewTabForSearch(string searchWord)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch))
            {
                if (tb.SearchWords == searchWord && string.IsNullOrEmpty(tb.SearchLang))
                {
                    foreach (TabPage tp in this.ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            this.ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }

            // ユニークなタブ名生成
            string tabName = searchWord;
            for (int i = 0; i <= 100; i++)
            {
                if (this.statuses.ContainsTab(tabName))
                {
                    tabName += "_";
                }
                else
                {
                    break;
                }
            }

            // タブ追加
            this.statuses.AddTab(tabName, TabUsageType.PublicSearch, null);
            this.AddNewTab(tabName, false, TabUsageType.PublicSearch);

            // 追加したタブをアクティブに
            this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;

            // 検索条件の設定
            ComboBox cmb = (ComboBox)this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            this.SaveConfigsTabs();

            // 検索実行
            this.SearchButton_Click(this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
        }

        public void AddNewTabForUserTimeline(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in this.statuses.GetTabsByType(TabUsageType.UserTimeline))
            {
                if (tb.User == user)
                {
                    foreach (TabPage tp in this.ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            this.ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }

            // ユニークなタブ名生成
            string tabName = "user:" + user;
            while (this.statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }

            // タブ追加
            this.statuses.AddTab(tabName, TabUsageType.UserTimeline, null);
            this.statuses.Tabs[tabName].User = user;
            this.AddNewTab(tabName, false, TabUsageType.UserTimeline);

            // 追加したタブをアクティブに
            this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
            this.SaveConfigsTabs();

            // 検索実行
            this.GetTimeline(WorkerType.UserTimeline, 1, 0, tabName);
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
            pnl.Enter += this.SearchControls_Enter;
            pnl.Leave += this.SearchControls_Leave;

            cmb.Text = string.Empty;
            cmb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmb.Dock = DockStyle.Fill;
            cmb.Name = "comboSearch";
            cmb.DropDownStyle = ComboBoxStyle.DropDown;
            cmb.ImeMode = ImeMode.NoControl;
            cmb.TabStop = false;
            cmb.AutoCompleteMode = AutoCompleteMode.None;
            cmb.KeyDown += this.SearchComboBox_KeyDown;

            if (this.statuses.ContainsTab(tabName))
            {
                cmb.Items.Add(this.statuses.Tabs[tabName].SearchWords);
                cmb.Text = this.statuses.Tabs[tabName].SearchWords;
            }

            cmbLang.Text = string.Empty;
            cmbLang.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbLang.Dock = DockStyle.Right;
            cmbLang.Width = 50;
            cmbLang.Name = "comboLang";
            cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLang.TabStop = false;
            cmbLang.Items.Add(string.Empty);
            cmbLang.Items.AddRange(this.TwitterSearchLangs);
            if (this.statuses.ContainsTab(tabName))
            {
                cmbLang.Text = this.statuses.Tabs[tabName].SearchLang;
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
            btn.Click += this.SearchButton_Click;
            return pnl;
        }

        private DetailsListView CreateDetailListView(string tabName, bool startup)
        {
            this.InitColumnText();
            var colhds = new ColumnHeader[8]; // アイコン, ニックネーム, 本文, 日付, ユーザID, 未読, マーク＆プロテクト, ソース
            var widths = new int[] { 48, 80, 300, 50, 50, 16, 16, 50 };
            for (var i = 0; i < colhds.Length; ++i)
            {
                colhds[i] = new ColumnHeader()
                {
                    Width = widths[i],
                    Text = this.columnTexts[i]
                };
            }

            DetailsListView listCustom = new DetailsListView();
            listCustom.ContextMenuStrip = this.ContextMenuOperate;
            listCustom.Font = this.fntReaded;
            listCustom.BackColor = this.clrListBackcolor;
            listCustom.GridLines = this.configs.ShowGrid;

            var sz = this.iconSz > 0 ? this.iconSz : 1;
            listCustom.SmallImageList = new ImageList() { ImageSize = new Size(sz, sz) };
            listCustom.Columns.AddRange(this.iconCol ? 
                new ColumnHeader[] { colhds[0], colhds[2] } :
                new ColumnHeader[] { colhds[0], colhds[1], colhds[2], colhds[3], colhds[4], colhds[5], colhds[6], colhds[7] });
            
            int[] dispOrder = new int[8];
            if (!startup)
            {
                for (int i = 0; i < this.curList.Columns.Count; i++)
                {
                    for (int j = 0; j < this.curList.Columns.Count; j++)
                    {
                        if (this.curList.Columns[j].DisplayIndex == i)
                        {
                            dispOrder[i] = j;
                            break;
                        }
                    }
                }

                for (int i = 0; i < this.curList.Columns.Count; i++)
                {
                    listCustom.Columns[i].Width = this.curList.Columns[i].Width;
                    listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                }
            }
            else
            {
                if (this.iconCol)
                {
                    listCustom.Columns[0].Width = this.cfgLocal.Width1;
                    listCustom.Columns[1].Width = this.cfgLocal.Width3;
                    listCustom.Columns[0].DisplayIndex = 0;
                    listCustom.Columns[1].DisplayIndex = 1;
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (this.cfgLocal.DisplayIndex1 == i)
                        {
                            dispOrder[i] = 0;
                        }
                        else if (this.cfgLocal.DisplayIndex2 == i)
                        {
                            dispOrder[i] = 1;
                        }
                        else if (this.cfgLocal.DisplayIndex3 == i)
                        {
                            dispOrder[i] = 2;
                        }
                        else if (this.cfgLocal.DisplayIndex4 == i)
                        {
                            dispOrder[i] = 3;
                        }
                        else if (this.cfgLocal.DisplayIndex5 == i)
                        {
                            dispOrder[i] = 4;
                        }
                        else if (this.cfgLocal.DisplayIndex6 == i)
                        {
                            dispOrder[i] = 5;
                        }
                        else if (this.cfgLocal.DisplayIndex7 == i)
                        {
                            dispOrder[i] = 6;
                        }
                        else if (this.cfgLocal.DisplayIndex8 == i)
                        {
                            dispOrder[i] = 7;
                        }
                    }

                    listCustom.Columns[0].Width = this.cfgLocal.Width1;
                    listCustom.Columns[1].Width = this.cfgLocal.Width2;
                    listCustom.Columns[2].Width = this.cfgLocal.Width3;
                    listCustom.Columns[3].Width = this.cfgLocal.Width4;
                    listCustom.Columns[4].Width = this.cfgLocal.Width5;
                    listCustom.Columns[5].Width = this.cfgLocal.Width6;
                    listCustom.Columns[6].Width = this.cfgLocal.Width7;
                    listCustom.Columns[7].Width = this.cfgLocal.Width8;
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
            foreach (TabPage tb in this.ListTab.TabPages)
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
                    if (this.statuses.GetTabByType(tabType) != null)
                    {
                        return false;
                    }
                }
            }

            var tabPage = new TabPage();

            int cnt = this.ListTab.TabPages.Count;

            //// ToDo:Create and set controls follow tabtypes

            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.ListTab.SuspendLayout();
            this.SuspendLayout();

            tabPage.SuspendLayout();

            /// UserTimeline関連
            Label label = null;
            if (tabType == TabUsageType.UserTimeline || tabType == TabUsageType.Lists)
            {
                label = new Label();
                label.Dock = DockStyle.Top;
                label.Name = "labelUser";
                label.Text = tabType == TabUsageType.Lists ? listInfo.ToString() : this.statuses.Tabs[tabName].User + "'s Timeline";
                label.TextAlign = ContentAlignment.MiddleLeft;
                using (var tmpComboBox = new ComboBox())
                {
                    label.Height = tmpComboBox.Height;
                }
            }

            this.ListTab.Controls.Add(tabPage);
            var listCustom = this.CreateDetailListView(tabName, startup);
            listCustom.SelectedIndexChanged += this.MyList_SelectedIndexChanged;
            listCustom.MouseDoubleClick += this.MyList_MouseDoubleClick;
            listCustom.ColumnClick += this.MyList_ColumnClick;
            listCustom.DrawColumnHeader += this.MyList_DrawColumnHeader;
            listCustom.DragDrop += this.TweenMain_DragDrop;
            listCustom.DragOver += this.TweenMain_DragOver;
            listCustom.DrawItem += this.MyList_DrawItem;
            listCustom.MouseClick += this.MyList_MouseClick;
            listCustom.ColumnReordered += this.MyList_ColumnReordered;
            listCustom.ColumnWidthChanged += this.MyList_ColumnWidthChanged;
            listCustom.CacheVirtualItems += this.MyList_CacheVirtualItems;
            listCustom.RetrieveVirtualItem += this.MyList_RetrieveVirtualItem;
            listCustom.DrawSubItem += this.MyList_DrawSubItem;
            listCustom.HScrolled += this.MyList_HScrolled;
            tabPage.Controls.Add(listCustom);

            if (this.statuses.IsDistributableTab(tabName))
            {
                this.tabDialog.AddTab(tabName);
            }

            /// 検索関連の準備
            Panel pnl = null;
            if (tabType == TabUsageType.PublicSearch)
            {
                pnl = this.CreateSearchPanel(tabName);
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

            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.ResumeLayout(false);
            this.ListTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            tabPage.Tag = listCustom;
            return true;
        }
        
        public bool RemoveSpecifiedTab(string tabName, bool confirm)
        {
            int idx = 0;
            for (idx = 0; idx < this.ListTab.TabPages.Count; idx++)
            {
                if (this.ListTab.TabPages[idx].Text == tabName)
                {
                    break;
                }
            }

            if (this.statuses.IsDefaultTab(tabName))
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

            this.SetListProperty(); // 他のタブに列幅等を反映

            TabUsageType tabType = this.statuses.Tabs[tabName].TabType;

            // オブジェクトインスタンスの削除
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.ListTab.SuspendLayout();
            this.SuspendLayout();

            TabPage tabPage = this.ListTab.TabPages[idx];
            DetailsListView listCustom = (DetailsListView)tabPage.Tag;
            tabPage.Tag = null;
            tabPage.SuspendLayout();

            if (object.ReferenceEquals(this.ListTab.SelectedTab, this.ListTab.TabPages[idx]))
            {
                this.ListTab.SelectTab(this.prevSelectedTab != null && this.ListTab.TabPages.Contains(this.prevSelectedTab) ? this.prevSelectedTab : this.ListTab.TabPages[0]);
            }

            this.ListTab.Controls.Remove(tabPage);

            Control pnl = null;
            if (tabType == TabUsageType.PublicSearch)
            {
                pnl = tabPage.Controls["panelSearch"];
                foreach (Control ctrl in pnl.Controls)
                {
                    if (ctrl.Name == "buttonSearch")
                    {
                        ctrl.Click -= this.SearchButton_Click;
                    }

                    ctrl.Enter -= this.SearchControls_Enter;
                    ctrl.Leave -= this.SearchControls_Leave;
                    pnl.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }

                tabPage.Controls.Remove(pnl);
            }

            tabPage.Controls.Remove(listCustom);
            listCustom.Columns.Clear();
            listCustom.ContextMenuStrip = null;

            listCustom.SelectedIndexChanged -= this.MyList_SelectedIndexChanged;
            listCustom.MouseDoubleClick -= this.MyList_MouseDoubleClick;
            listCustom.ColumnClick -= this.MyList_ColumnClick;
            listCustom.DrawColumnHeader -= this.MyList_DrawColumnHeader;
            listCustom.DragDrop -= this.TweenMain_DragDrop;
            listCustom.DragOver -= this.TweenMain_DragOver;
            listCustom.DrawItem -= this.MyList_DrawItem;
            listCustom.MouseClick -= this.MyList_MouseClick;
            listCustom.ColumnReordered -= this.MyList_ColumnReordered;
            listCustom.ColumnWidthChanged -= this.MyList_ColumnWidthChanged;
            listCustom.CacheVirtualItems -= this.MyList_CacheVirtualItems;
            listCustom.RetrieveVirtualItem -= this.MyList_RetrieveVirtualItem;
            listCustom.DrawSubItem -= this.MyList_DrawSubItem;
            listCustom.HScrolled -= this.MyList_HScrolled;

            this.tabDialog.RemoveTab(tabName);

            listCustom.SmallImageList = null;
            listCustom.ListViewItemSorter = null;

            // キャッシュのクリア
            if (this.curTab.Equals(tabPage))
            {
                this.curTab = null;
                this.curItemIndex = -1;
                this.curList = null;
                this.curPost = null;
            }

            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;

            tabPage.ResumeLayout(false);

            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.ResumeLayout(false);
            this.ListTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            tabPage.Dispose();
            listCustom.Dispose();
            this.statuses.RemoveTab(tabName);

            foreach (TabPage tp in this.ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tp.Tag;
                if (lst.VirtualListSize != this.statuses.Tabs[tp.Text].AllCount)
                {
                    lst.VirtualListSize = this.statuses.Tabs[tp.Text].AllCount;
                }
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

            this.TopMost = this.configs.AlwaysTop;
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
            return this.detailHtmlFormatHeader + orgdata + this.detailHtmlFormatFooter;
        }

        public bool RenameTab(ref string tabName)
        {
            // タブ名変更
            string newTabText = tabName;
            if (!this.TryUserInputText(ref newTabText))
            {
                return false;
            }

            this.TopMost = this.configs.AlwaysTop;
            if (string.IsNullOrEmpty(newTabText))
            {
                return false;
            }

            // 新タブ名存在チェック
            for (int i = 0; i < this.ListTab.TabCount; i++)
            {
                if (this.ListTab.TabPages[i].Text == newTabText)
                {
                    string tmp = string.Format(R.Tabs_DoubleClickText1, newTabText);
                    MessageBox.Show(tmp, R.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            // タブ名のリスト作り直し（デフォルトタブ以外は再作成）
            for (int i = 0; i < this.ListTab.TabCount; i++)
            {
                if (this.statuses.IsDistributableTab(this.ListTab.TabPages[i].Text))
                {
                    this.tabDialog.RemoveTab(this.ListTab.TabPages[i].Text);
                }

                if (this.ListTab.TabPages[i].Text == tabName)
                {
                    this.ListTab.TabPages[i].Text = newTabText;
                }
            }

            this.statuses.RenameTab(tabName, newTabText);
            for (int i = 0; i < this.ListTab.TabCount; i++)
            {
                if (this.statuses.IsDistributableTab(this.ListTab.TabPages[i].Text))
                {
                    if (this.ListTab.TabPages[i].Text == tabName)
                    {
                        this.ListTab.TabPages[i].Text = newTabText;
                    }

                    this.tabDialog.AddTab(this.ListTab.TabPages[i].Text);
                }
            }

            this.SaveConfigsCommon();
            this.SaveConfigsTabs();
            this.rclickTabName = newTabText;
            tabName = newTabText;
            return true;
        }

        public void ReorderTab(string targetTabText, string baseTabText, bool isBeforeBaseTab)
        {
            int baseIndex = 0;
            for (baseIndex = 0; baseIndex < this.ListTab.TabPages.Count; baseIndex++)
            {
                if (this.ListTab.TabPages[baseIndex].Text == baseTabText)
                {
                    break;
                }
            }

            this.ListTab.SuspendLayout();
            TabPage tp = null;
            for (int j = 0; j < this.ListTab.TabPages.Count; j++)
            {
                if (this.ListTab.TabPages[j].Text == targetTabText)
                {
                    tp = this.ListTab.TabPages[j];
                    this.ListTab.TabPages.Remove(tp);
                    if (j < baseIndex)
                    {
                        baseIndex -= 1;
                    }

                    break;
                }
            }

            if (isBeforeBaseTab)
            {
                this.ListTab.TabPages.Insert(baseIndex, tp);
            }
            else
            {
                this.ListTab.TabPages.Insert(baseIndex + 1, tp);
            }

            this.ListTab.ResumeLayout();
            this.SaveConfigsTabs();
        }

        public void ChangeTabUnreadManage(string tabName, bool isManage)
        {
            int idx = 0;
            for (idx = 0; idx < this.ListTab.TabCount; idx++)
            {
                if (this.ListTab.TabPages[idx].Text == tabName)
                {
                    break;
                }
            }

            this.statuses.SetTabUnreadManage(tabName, isManage);
            if (this.configs.TabIconDisp)
            {
                this.ListTab.TabPages[idx].ImageIndex = this.statuses.Tabs[tabName].UnreadCount > 0 ? 0 : -1;
            }

            if (this.curTab.Text == tabName)
            {
                this.itemCache = null;
                this.postCache = null;
                this.curList.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (!this.configs.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        public void SetStatusLabel(string text)
        {
            this.StatusLabel.Text = text;
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
                this.RunAsync(new GetWorkerArg() { WorkerType = WorkerType.OpenUri, Url = uri });
            }
        }

        /// <summary>
        /// TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        /// </summary>
        /// <param name="name">URLから切り出した文字列</param>
        /// <returns></returns>
        public bool IsTwitterId(string name)
        {
            string[] nonUsernames = this.configs.TwitterConfiguration.NonUsernamePaths;
            if (nonUsernames == null || nonUsernames.Length == 0)
            {
                return !Regex.Match(name, "^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            }

            return !nonUsernames.Contains(name.ToLower());
        }

        public void SetModifySettingCommon(bool value)
        {
            this.modifySettingCommon = value;
        }

        public void SetModifySettingLocal(bool value)
        {
            this.modifySettingLocal = value;
        }

        public void SetModifySettingAtId(bool value)
        {
            this.modifySettingAtId = value;
        }

        public bool AddAtIdSuplItem(string item)
        {
            if (this.AtIdSupl.AddItem(item))
            {
                this.SetModifySettingAtId(true);
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
                if (this.StatusText.Focused)
                {
                    bool doNewLine = false;
                    bool doPost = false;

                    // Ctrl+Enter投稿時
                    if (this.configs.PostCtrlEnter)
                    {
                        if (this.StatusText.Multiline)
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
                    else if (this.configs.PostShiftEnter)
                    {
                        if (this.StatusText.Multiline)
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
                        if (this.StatusText.Multiline)
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
                        int pos1 = this.StatusText.SelectionStart;
                        if (this.StatusText.SelectionLength > 0)
                        {
                            // 選択状態文字列削除
                            this.StatusText.Text = this.StatusText.Text.Remove(pos1, this.StatusText.SelectionLength);
                        }

                        this.StatusText.Text = this.StatusText.Text.Insert(pos1, Environment.NewLine); // 改行挿入
                        this.StatusText.SelectionStart = pos1 + Environment.NewLine.Length;       // カーソルを改行の次の文字へ移動
                        return true;
                    }
                    else if (doPost)
                    {
                        this.TryPostTweet();
                        return true;
                    }
                }
                else if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch && (this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focused || this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboLang"].Focused))
                {
                    this.SearchButton_Click(this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        #endregion protected methods
    }
}