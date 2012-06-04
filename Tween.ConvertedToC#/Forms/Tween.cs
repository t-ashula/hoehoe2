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
// "c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"
// "C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"

namespace Hoehoe
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Windows.Forms;
    using Hoehoe.TweenCustomControl;

    public partial class TweenMain
    {
        #region private fields

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/FrontPage";

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationShortcutKeyHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        private const string DetailHtmlFormatMono1 = "<html><head><style type=\"text/css\"><!-- pre {font-family: \"";
        private const string DetailHtmlFormat2 = "\", sans-serif; font-size: ";
        private const string DetailHtmlFormat3 = "pt; word-wrap: break-word; color:rgb(";
        private const string DetailHtmlFormat4 = ");} a:link, a:visited, a:active, a:hover {color:rgb(";
        private const string DetailHtmlFormat5 = "); } --></style></head><body style=\"margin:0px; background-color:rgb(";
        private const string DetailHtmlFormatMono6 = ");\"><pre>";
        private const string DetailHtmlFormatMono7 = "</pre></body></html>";
        private const string DetailHtmlFormat1 = "<html><head><style type=\"text/css\"><!-- p {font-family: \"";
        private const string DetailHtmlFormat6 = ");\"><p>";
        private const string DetailHtmlFormat7 = "</p></body></html>";

        private static int accountCheckErrorCount;

        // ロック用
        private readonly object syncObject = new object();

        // 各種設定
        // 画面サイズ
        private Size mySize;

        // 画面位置
        private Point myLoc;

        // 区切り位置
        private int mySpDis;

        // 発言欄区切り位置
        private int mySpDis2;

        // プレビュー区切り位置
        private int mySpDis3;

        // Ad区切り位置
        private int myAdSpDis;

        // アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private int iconSz;

        // 1列表示の時True（48サイズのとき）
        private bool iconCol;

        // 雑多なフラグ類
        // True:起動時処理中
        private bool isInitializing;

        private bool initialLayout = true;

        // True:起動時処理中
        private bool ignoreConfigSave;

        // タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private bool tabDraging;

        // タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private TabPage prevSelectedTab;

        private Point tabMouseDownPoint;

        // 右クリックしたタブの名前（Tabコントロール機能不足対応）
        private string rclickTabName;

        private string detailHtmlFormatHeader;
        private string detailHtmlFormatFooter;
        private bool myStatusError;
        private bool soundfileListup;

        private SpaceKeyCanceler spaceKeyCanceler;

        // 設定ファイル関連
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
        private AppendSettingDialog settingDialog = AppendSettingDialog.Instance; // 設定画面インスタンス

        private TabsDialog tabDialog = new TabsDialog();      // タブ選択ダイアログインスタンス
        private SearchWord searchDialog = new SearchWord();   // 検索画面インスタンス
        private FilterDialog fltDialog = new FilterDialog();  // フィルター編集画面
        private OpenURL urlDialog = new OpenURL();

        private TweenAboutBox aboutBox;
        private EventViewerDialog evtDialog;

        // 表示フォント、色、アイコン
        // 未読用フォント
        private Font fntUnread;

        // 未読用文字色
        private Color clrUnread;

        // 既読用フォント
        private Font fntReaded;

        // 既読用文字色
        private Color clrRead;

        // Fav用文字色
        private Color clrFav;

        // 片思い用文字色
        private Color clrOWL;

        // Retweet用文字色
        private Color clrRetweet;

        // 発言詳細部用フォント
        private Font fntDetail;

        // 発言詳細部用色
        private Color clrDetail;

        // 発言詳細部用リンク文字色
        private Color clrDetailLink;

        // 発言詳細部用背景色
        private Color clrDetailBackcolor;

        // 自分の発言用背景色
        private Color clrSelf;

        // 自分宛返信用背景色
        private Color clrAtSelf;

        // 選択発言者の他の発言用背景色
        private Color clrTarget;

        // 選択発言中の返信先用背景色
        private Color clrAtTarget;

        // 選択発言者への返信発言用背景色
        private Color clrAtFromTarget;

        // 選択発言の唯一＠先
        private Color clrAtTo;

        // リスト部通常発言背景色
        private Color clrListBackcolor;

        // 入力欄背景色

        // 入力欄文字色
        private Color clrInputForecolor;

        // 入力欄フォント
        private Font fntInputFont;

        // アイコン画像リスト
        private ImageDictionary iconDict;

        // At.ico             タスクトレイアイコン：通常時
        private Icon iconAt;

        // AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon iconAtRed;

        // AtSmoke.ico        タスクトレイアイコン：オフライン時
        private Icon iconAtSmoke;

        // Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon[] iconRefresh = new Icon[4];

        // Tab.ico            未読のあるタブ用アイコン
        private Icon tabIcon;

        // Main.ico           画面左上のアイコン
        private Icon mainIcon;

        // 5g
        private Icon replyIcon;

        // 6g
        private Icon replyIconBlink;

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
        private string prevTrackWord = "";

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
        }

        #endregion constructor

        #region delegates

        public delegate void SetStatusLabelApiDelegate();

        #endregion delegates

        #region enums

        // 検索処理タイプ
        private enum SEARCHTYPE
        {
            DialogSearch,
            NextSearch,
            PrevSearch
        }

        [Flags]
        private enum ModifierState : int
        {
            None = 0,
            Alt = 1,
            Shift = 2,
            Ctrl = 4,
            NotFlags = 8
        }

        private enum FocusedControl : int
        {
            None,
            ListTab,
            StatusText,
            PostBrowser
        }

        #endregion enums

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
            get { return this.settingDialog.PreviewEnable; }
        }

        public bool FavEventChangeUnread
        {
            get { return this.settingDialog.FavEventUnread; }
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
            foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch))
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
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.UserTimeline))
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
            if (tabName == Hoehoe.Properties.Resources.AddNewTabText1)
            {
                return false;
            }

            // タブタイプ重複チェック
            if (!startup)
            {
                if (tabType == TabUsageType.DirectMessage || tabType == TabUsageType.Favorites || tabType == TabUsageType.Home || tabType == TabUsageType.Mentions || tabType == TabUsageType.Related)
                {
                    if (this.statuses.GetTabByType(tabType) != null)
                    {
                        return false;
                    }
                }
            }

            TabPage tabPage = new TabPage();
            DetailsListView listCustom = new DetailsListView();
            ColumnHeader colHd1 = new ColumnHeader();            // アイコン
            ColumnHeader colHd2 = new ColumnHeader();            // ニックネーム
            ColumnHeader colHd3 = new ColumnHeader();            // 本文
            ColumnHeader colHd4 = new ColumnHeader();            // 日付
            ColumnHeader colHd5 = new ColumnHeader();            // ユーザID
            ColumnHeader colHd6 = new ColumnHeader();            // 未読
            ColumnHeader colHd7 = new ColumnHeader();            // マーク＆プロテクト
            ColumnHeader colHd8 = new ColumnHeader();            // ソース

            int cnt = this.ListTab.TabPages.Count;

            ///ToDo:Create and set controls follow tabtypes

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
                using (ComboBox tmpComboBox = new ComboBox())
                {
                    label.Height = tmpComboBox.Height;
                }

                tabPage.Controls.Add(label);
            }

            /// 検索関連の準備
            Panel pnl = null;
            if (tabType == TabUsageType.PublicSearch)
            {
                pnl = new Panel();

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
                cmbLang.Items.Add("ja");
                cmbLang.Items.Add("en");
                cmbLang.Items.Add("ar");
                cmbLang.Items.Add("da");
                cmbLang.Items.Add("nl");
                cmbLang.Items.Add("fa");
                cmbLang.Items.Add("fi");
                cmbLang.Items.Add("fr");
                cmbLang.Items.Add("de");
                cmbLang.Items.Add("hu");
                cmbLang.Items.Add("is");
                cmbLang.Items.Add("it");
                cmbLang.Items.Add("no");
                cmbLang.Items.Add("pl");
                cmbLang.Items.Add("pt");
                cmbLang.Items.Add("ru");
                cmbLang.Items.Add("es");
                cmbLang.Items.Add("sv");
                cmbLang.Items.Add("th");
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
            }

            this.ListTab.Controls.Add(tabPage);
            tabPage.Controls.Add(listCustom);

            if (tabType == TabUsageType.PublicSearch)
            {
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

            listCustom.AllowColumnReorder = true;
            if (!this.iconCol)
            {
                listCustom.Columns.AddRange(new ColumnHeader[] { colHd1, colHd2, colHd3, colHd4, colHd5, colHd6, colHd7, colHd8 });
            }
            else
            {
                listCustom.Columns.AddRange(new ColumnHeader[] { colHd1, colHd3 });
            }

            listCustom.ContextMenuStrip = this.ContextMenuOperate;
            listCustom.Dock = DockStyle.Fill;
            listCustom.FullRowSelect = true;
            listCustom.HideSelection = false;
            listCustom.Location = new Point(0, 0);
            listCustom.Margin = new Padding(0);
            listCustom.Name = "CList" + Environment.TickCount.ToString();
            listCustom.ShowItemToolTips = true;
            listCustom.Size = new Size(380, 260);
            listCustom.UseCompatibleStateImageBehavior = false;
            listCustom.View = View.Details;
            listCustom.OwnerDraw = true;
            listCustom.VirtualMode = true;
            listCustom.Font = this.fntReaded;
            listCustom.BackColor = this.clrListBackcolor;
            listCustom.GridLines = this.settingDialog.ShowGrid;
            listCustom.AllowDrop = true;
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

            this.InitColumnText();
            colHd1.Text = this.columnTexts[0];
            colHd1.Width = 48;
            colHd2.Text = this.columnTexts[1];
            colHd2.Width = 80;
            colHd3.Text = this.columnTexts[2];
            colHd3.Width = 300;
            colHd4.Text = this.columnTexts[3];
            colHd4.Width = 50;
            colHd5.Text = this.columnTexts[4];
            colHd5.Width = 50;
            colHd6.Text = this.columnTexts[5];
            colHd6.Width = 16;
            colHd7.Text = this.columnTexts[6];
            colHd7.Width = 16;
            colHd8.Text = this.columnTexts[7];
            colHd8.Width = 50;

            if (this.statuses.IsDistributableTab(tabName))
            {
                this.tabDialog.AddTab(tabName);
            }

            listCustom.SmallImageList = new ImageList();
            if (this.iconSz > 0)
            {
                listCustom.SmallImageList.ImageSize = new Size(this.iconSz, this.iconSz);
            }
            else
            {
                listCustom.SmallImageList.ImageSize = new Size(1, 1);
            }

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

                for (int i = 0; i <= this.curList.Columns.Count - 1; i++)
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
                    for (int i = 0; i <= 7; i++)
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
                    for (int i = 0; i <= 7; i++)
                    {
                        listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                    }
                }
            }

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
                string tmp = string.Format(Hoehoe.Properties.Resources.RemoveSpecifiedTabText1, Environment.NewLine);
                if (MessageBox.Show(tmp, tabName + " " + Hoehoe.Properties.Resources.RemoveSpecifiedTabText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
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

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog)
        {
            this.ShowSuplDialog(owner, dialog, 0, string.Empty);
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset)
        {
            this.ShowSuplDialog(owner, dialog, offset, string.Empty);
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset, string startswith)
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

            this.TopMost = this.settingDialog.AlwaysTop;
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

        public bool TabRename(ref string tabName)
        {
            // タブ名変更
            string newTabText = null;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = tabName;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return false;
                }

                newTabText = inputName.TabName;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (string.IsNullOrEmpty(newTabText))
            {
                return false;
            }

            // 新タブ名存在チェック
            for (int i = 0; i < this.ListTab.TabCount; i++)
            {
                if (this.ListTab.TabPages[i].Text == newTabText)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.Tabs_DoubleClickText1, newTabText);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        public void ReOrderTab(string targetTabText, string baseTabText, bool isBeforeBaseTab)
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

            TabPage mTp = null;
            for (int j = 0; j < this.ListTab.TabPages.Count; j++)
            {
                if (this.ListTab.TabPages[j].Text == targetTabText)
                {
                    mTp = this.ListTab.TabPages[j];
                    this.ListTab.TabPages.Remove(mTp);
                    if (j < baseIndex)
                    {
                        baseIndex -= 1;
                    }

                    break;
                }
            }

            if (isBeforeBaseTab)
            {
                this.ListTab.TabPages.Insert(baseIndex, mTp);
            }
            else
            {
                this.ListTab.TabPages.Insert(baseIndex + 1, mTp);
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
            if (this.settingDialog.TabIconDisp)
            {
                if (this.statuses.Tabs[tabName].UnreadCount > 0)
                {
                    this.ListTab.TabPages[idx].ImageIndex = 0;
                }
                else
                {
                    this.ListTab.TabPages[idx].ImageIndex = -1;
                }
            }

            if (this.curTab.Text == tabName)
            {
                this.itemCache = null;
                this.postCache = null;
                this.curList.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        public void SetStatusLabel(string text)
        {
            this.StatusLabel.Text = text;
        }

        public string WebBrowser_GetSelectionText(ref WebBrowser componentInstance)
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
            this.RunAsync(new GetWorkerArg() { WorkerType = WorkerType.OpenUri, Url = uri });
        }

        /// <summary>
        /// TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        /// </summary>
        /// <param name="name">URLから切り出した文字列</param>
        /// <returns></returns>
        public bool IsTwitterId(string name)
        {
            if (this.settingDialog.TwitterConfiguration.NonUsernamePaths == null || this.settingDialog.TwitterConfiguration.NonUsernamePaths.Length == 0)
            {
                return !Regex.Match(name, "^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            }
            else
            {
                return !this.settingDialog.TwitterConfiguration.NonUsernamePaths.Contains(name.ToLower());
            }
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
                    if (this.settingDialog.PostCtrlEnter)
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
                    else if (this.settingDialog.PostShiftEnter)
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

        #region private methods

        private static bool CheckAccountValid()
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                accountCheckErrorCount += 1;
                if (accountCheckErrorCount > 5)
                {
                    accountCheckErrorCount = 0;
                    Twitter.AccountState = AccountState.Valid;
                    return true;
                }

                return false;
            }

            accountCheckErrorCount = 0;
            return true;
        }

        private static void MoveArrayItem(int[] values, int fromIndex, int toIndex)
        {
            int movedValue = values[fromIndex];
            int numMoved = Math.Abs(fromIndex - toIndex);

            if (toIndex < fromIndex)
            {
                Array.Copy(values, toIndex, values, toIndex + 1, numMoved);
            }
            else
            {
                Array.Copy(values, fromIndex + 1, values, fromIndex, numMoved);
            }

            values[toIndex] = movedValue;
        }

        private void CheckReplyTo(string statusText)
        {
            // ハッシュタグの保存
            MatchCollection m = Regex.Matches(statusText, Twitter.HashtagRegexPattern, RegexOptions.IgnoreCase);
            string hstr = string.Empty;
            foreach (Match hm in m)
            {
                if (!hstr.Contains("#" + hm.Result("$3") + " "))
                {
                    hstr += "#" + hm.Result("$3") + " ";
                    this.HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && !hstr.Contains(this.HashMgr.UseHash + " "))
            {
                hstr += this.HashMgr.UseHash;
            }

            if (!string.IsNullOrEmpty(hstr))
            {
                this.HashMgr.AddHashToHistory(hstr.Trim(), false);
            }

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(statusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");
            if (this.settingDialog.UseAtIdSupplement)
            {
                int cnt = this.AtIdSupl.ItemCount;
                foreach (Match mid in m)
                {
                    this.AtIdSupl.AddItem(mid.Result("${id}"));
                }

                if (cnt != this.AtIdSupl.ItemCount)
                {
                    this.modifySettingAtId = true;
                }
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (this.replyToId == 0)
            {
                return;
            }

            // リプライ先ユーザー名がない場合も指定しない
            if (string.IsNullOrEmpty(this.replyToName))
            {
                this.replyToId = 0;
                return;
            }

            // 通常Reply
            // 次の条件を満たす場合に in_reply_to_status_id 指定
            // 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
            // 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
            // 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する
            if (m != null)
            {
                if (statusText.StartsWith("@"))
                {
                    if (statusText.StartsWith("@" + this.replyToName))
                    {
                        return;
                    }
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (statusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + this.replyToName)
                        {
                            return;
                        }
                    }
                }
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
        }

        private void LoadIcon(ref Icon iconInstance, string fileName)
        {
            string dir = Application.StartupPath;
            if (File.Exists(Path.Combine(dir, fileName)))
            {
                try
                {
                    iconInstance = new Icon(Path.Combine(dir, fileName));
                }
                catch (Exception)
                {
                }
            }
        }

        private void LoadIcons()
        {
            // 着せ替えアイコン対応
            // タスクトレイ通常時アイコン
            string dir = Application.StartupPath;

            this.iconAt = Hoehoe.Properties.Resources.At;
            this.iconAtRed = Hoehoe.Properties.Resources.AtRed;
            this.iconAtSmoke = Hoehoe.Properties.Resources.AtSmoke;
            this.iconRefresh[0] = Hoehoe.Properties.Resources.Refresh;
            this.iconRefresh[1] = Hoehoe.Properties.Resources.Refresh2;
            this.iconRefresh[2] = Hoehoe.Properties.Resources.Refresh3;
            this.iconRefresh[3] = Hoehoe.Properties.Resources.Refresh4;
            this.tabIcon = Hoehoe.Properties.Resources.TabIcon;
            this.mainIcon = Hoehoe.Properties.Resources.MIcon;
            this.replyIcon = Hoehoe.Properties.Resources.Reply;
            this.replyIconBlink = Hoehoe.Properties.Resources.ReplyBlink;

            if (!Directory.Exists(Path.Combine(dir, "Icons")))
            {
                return;
            }

            this.LoadIcon(ref this.iconAt, "Icons\\At.ico");

            // タスクトレイエラー時アイコン
            this.LoadIcon(ref this.iconAtRed, "Icons\\AtRed.ico");

            // タスクトレイオフライン時アイコン
            this.LoadIcon(ref this.iconAtSmoke, "Icons\\AtSmoke.ico");

            // タスクトレイ更新中アイコン
            // アニメーション対応により4種類読み込み
            this.LoadIcon(ref this.iconRefresh[0], "Icons\\Refresh.ico");
            this.LoadIcon(ref this.iconRefresh[1], "Icons\\Refresh2.ico");
            this.LoadIcon(ref this.iconRefresh[2], "Icons\\Refresh3.ico");
            this.LoadIcon(ref this.iconRefresh[3], "Icons\\Refresh4.ico");

            // タブ見出し未読表示アイコン
            this.LoadIcon(ref this.tabIcon, "Icons\\Tab.ico");

            // 画面のアイコン
            this.LoadIcon(ref this.mainIcon, "Icons\\MIcon.ico");

            // Replyのアイコン
            this.LoadIcon(ref this.replyIcon, "Icons\\Reply.ico");

            // Reply点滅のアイコン
            this.LoadIcon(ref this.replyIconBlink, "Icons\\ReplyBlink.ico");
        }

        private void InitColumnText()
        {
            this.columnTexts[0] = string.Empty;
            this.columnTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            this.columnTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            this.columnTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            this.columnTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            this.columnTexts[5] = string.Empty;
            this.columnTexts[6] = string.Empty;
            this.columnTexts[7] = "Source";

            this.columnOrgTexts[0] = string.Empty;
            this.columnOrgTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            this.columnOrgTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            this.columnOrgTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            this.columnOrgTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            this.columnOrgTexts[5] = string.Empty;
            this.columnOrgTexts[6] = string.Empty;
            this.columnOrgTexts[7] = "Source";

            int c = 0;
            switch (this.statuses.SortMode)
            {
                case IdComparerClass.ComparerMode.Nickname:
                    // ニックネーム
                    c = 1;
                    break;
                case IdComparerClass.ComparerMode.Data:
                    // 本文
                    c = 2;
                    break;
                case IdComparerClass.ComparerMode.Id:
                    // 時刻=発言Id
                    c = 3;
                    break;
                case IdComparerClass.ComparerMode.Name:
                    // 名前
                    c = 4;
                    break;
                case IdComparerClass.ComparerMode.Source:
                    // Source
                    c = 7;
                    break;
            }

            if (this.iconCol)
            {
                if (this.statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this.columnTexts[2] = this.columnOrgTexts[2] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this.columnTexts[2] = this.columnOrgTexts[2] + "▴";
                }
            }
            else
            {
                if (this.statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this.columnTexts[c] = this.columnOrgTexts[c] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this.columnTexts[c] = this.columnOrgTexts[c] + "▴";
                }
            }
        }

        private void InitializeTraceFrag()
        {
#if DEBUG
			TraceOutToolStripMenuItem.Checked = true;
			MyCommon.TraceFlag = true;
#endif
            if (!MyCommon.FileVersion.EndsWith("0"))
            {
                this.TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void CreatePictureServices()
        {
            if (this.pictureServices != null)
            {
                this.pictureServices.Clear();
            }

            this.pictureServices = null;
            this.pictureServices = new Dictionary<string, IMultimediaShareService>
            {
                { "TwitPic", new TwitPic(this.tw) },
                { "img.ly", new Imgly(this.tw) },
                { "yfrog", new Yfrog(this.tw) },
                { "lockerz", new Plixi(this.tw) },
                { "Twitter", new TwitterPhoto(this.tw) }
            };
        }

        private void LoadConfig()
        {
            this.cfgCommon = SettingCommon.Load();
            if (this.cfgCommon.UserAccounts == null || this.cfgCommon.UserAccounts.Count == 0)
            {
                this.cfgCommon.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(this.cfgCommon.UserName))
                {
                    this.cfgCommon.UserAccounts.Add(new UserAccount
                    {
                        Username = this.cfgCommon.UserName,
                        UserId = this.cfgCommon.UserId,
                        Token = this.cfgCommon.Token,
                        TokenSecret = this.cfgCommon.TokenSecret
                    });
                }
            }

            this.cfgLocal = SettingLocal.Load();
            List<TabClass> tabs = SettingTabs.Load().Tabs;
            foreach (TabClass tb in tabs)
            {
                try
                {
                    this.statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = this.statuses.GetUniqueTabName();
                    this.statuses.Tabs.Add(tb.TabName, tb);
                }
            }

            if (this.statuses.Tabs.Count == 0)
            {
                this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
            }
        }

        private void RefreshTimeline(bool isUserStream)
        {
            if (isUserStream)
            {
                this.RefreshTasktrayIcon(true);
            }

            // スクロール制御準備
            int smode = -1; // -1:制御しない,-2:最新へ,その他:topitem使用
            long topId = this.GetScrollPos(ref smode);
            int befCnt = this.curList.VirtualListSize;

            // 現在の選択状態を退避
            Dictionary<string, long[]> selId = new Dictionary<string, long[]>();
            Dictionary<string, long> focusedId = new Dictionary<string, long>();
            this.SaveSelectedStatus(selId, focusedId);

            // mentionsの更新前件数を保持
            int dmessageCount = this.statuses.GetTabByType(TabUsageType.DirectMessage).AllCount;

            // 更新確定
            PostClass[] notifyPosts = null;
            string soundFile = string.Empty;
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = this.statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon.IsEnding)
            {
                return;
            }

            // リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in this.ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = this.statuses.Tabs[tab.Text];
                    lst.BeginUpdate();
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        if (lst.Equals(this.curList))
                        {
                            this.itemCache = null;
                            this.postCache = null;
                        }

                        try
                        {
                            // リスト件数更新
                            lst.VirtualListSize = tabInfo.AllCount;
                        }
                        catch (Exception)
                        {
                            // アイコン描画不具合あり？
                        }

                        this.SelectListItem(lst, this.statuses.IndexOf(tab.Text, selId[tab.Text]), this.statuses.IndexOf(tab.Text, focusedId[tab.Text]));
                    }

                    lst.EndUpdate();
                    if (tabInfo.UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            if (tab.ImageIndex == -1)
                            {
                                // タブアイコン
                                tab.ImageIndex = 0;
                            }
                        }
                    }
                }

                if (!this.settingDialog.TabIconDisp)
                {
                    this.ListTab.Refresh();
                }
            }
            catch (Exception)
            {
            }

            // スクロール制御後処理
            if (smode != -1)
            {
                try
                {
                    if (befCnt != this.curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                // 最上行
                                if (this.curList.VirtualListSize > 0)
                                {
                                    this.curList.EnsureVisible(0);
                                }

                                break;
                            case -2:
                                // 最下行へ
                                if (this.curList.VirtualListSize > 0)
                                {
                                    this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
                                }

                                break;
                            case -1:
                                // 制御しない
                                break;
                            default:
                                // 表示位置キープ
                                if (this.curList.VirtualListSize > 0 && this.statuses.IndexOf(this.curTab.Text, topId) > -1)
                                {
                                    this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
                                    this.curList.EnsureVisible(this.statuses.IndexOf(this.curTab.Text, topId));
                                }

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Msg"] = "Ref2";
                    throw;
                }
            }

            // 新着通知
            this.NotifyNewPosts(notifyPosts, soundFile, addCount, isMention || dmessageCount != this.statuses.GetTabByType(TabUsageType.DirectMessage).AllCount);

            this.SetMainWindowTitle();
            if (!this.StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }

            this.HashSupl.AddRangeItem(this.tw.GetHashList());
        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (this.curList != null && this.curTab != null && this.curList.VirtualListSize > 0)
            {
                if (this.statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if (this.statuses.SortOrder == SortOrder.Ascending)
                    {
                        // Id昇順
                        if (this.ListLockMenuItem.Checked)
                        {
                            // 制御しない(現在表示位置へ強制スクロール)
                            smode = -1;
                        }
                        else
                        {
                            // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                            ListViewItem item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
                            if (item == null)
                            {
                                // 一番下
                                item = this.curList.Items[this.curList.Items.Count - 1];
                            }

                            if (item.Index == this.curList.Items.Count - 1)
                            {
                                smode = -2;
                            }
                            else
                            {
                                smode = -1;
                            }
                        }
                    }
                    else
                    {
                        // Id降順
                        if (this.ListLockMenuItem.Checked)
                        {
                            // 現在表示位置へ強制スクロール
                            if (this.curList.TopItem != null)
                            {
                                topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                            }

                            smode = 0;
                        }
                        else
                        {
                            // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                            ListViewItem item = this.curList.GetItemAt(0, 10);
                            if (item == null)
                            {
                                // 一番上
                                item = this.curList.Items[0];
                            }

                            if (item.Index == 0)
                            {
                                // 最上行
                                smode = -3;
                            }
                            else
                            {
                                if (this.curList.TopItem != null)
                                {
                                    topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                                }

                                smode = 0;
                            }
                        }
                    }
                }
                else
                {
                    // 現在表示位置へ強制スクロール
                    if (this.curList.TopItem != null)
                    {
                        topId = this.statuses.GetId(this.curTab.Text, this.curList.TopItem.Index);
                    }

                    smode = 0;
                }
            }
            else
            {
                smode = -1;
            }

            return topId;
        }

        private void SaveSelectedStatus(Dictionary<string, long[]> selId, Dictionary<string, long> focusedId)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            foreach (TabPage tab in this.ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tab.Tag;
                if (lst.SelectedIndices.Count > 0 && lst.SelectedIndices.Count < 61)
                {
                    selId.Add(tab.Text, this.statuses.GetId(tab.Text, lst.SelectedIndices));
                }
                else
                {
                    selId.Add(tab.Text, new long[1] { -2 });
                }

                if (lst.FocusedItem != null)
                {
                    focusedId.Add(tab.Text, this.statuses.GetId(tab.Text, lst.FocusedItem.Index));
                }
                else
                {
                    focusedId.Add(tab.Text, -2);
                }
            }
        }

        private bool BalloonRequired()
        {
            return this.BalloonRequired(new Twitter.FormattedEvent { Eventtype = EventType.None });
        }

        private bool IsEventNotifyAsEventType(EventType type)
        {
            return (this.settingDialog.EventNotifyEnabled && Convert.ToBoolean(type & this.settingDialog.EventNotifyFlag)) || type == EventType.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return Convert.ToBoolean(ev.Eventtype & this.settingDialog.IsMyEventNotifyFlag) ? true : !ev.IsMe;
        }

        private bool BalloonRequired(Twitter.FormattedEvent ev)
        {
            return this.IsEventNotifyAsEventType(ev.Eventtype) && this.IsMyEventNotityAsEventType(ev) && (this.NewPostPopMenuItem.Checked || (this.settingDialog.ForceEventNotify && ev.Eventtype != EventType.None)) && !this.isInitializing && ((this.settingDialog.LimitBalloon && (this.WindowState == FormWindowState.Minimized || !this.Visible || Form.ActiveForm == null)) || !this.settingDialog.LimitBalloon) && !Win32Api.IsScreenSaverRunning();
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null && notifyPosts.Count() > 0 && this.settingDialog.ReadOwnPost && notifyPosts.All(post => post.UserId == this.tw.UserId || post.ScreenName == this.tw.Username))
            {
                return;
            }

            // 新着通知
            if (this.BalloonRequired())
            {
                if (notifyPosts != null && notifyPosts.Length > 0)
                {
                    // Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                    if (this.settingDialog.IsNotifyUseGrowl)
                    {
                        StringBuilder sb = new StringBuilder();
                        bool reply = false;
                        bool dm = false;
                        foreach (var post in notifyPosts)
                        {
                            if (!(notifyPosts.Count() > 3))
                            {
                                sb.Clear();
                                reply = false;
                                dm = false;
                            }

                            if (post.IsReply && !post.IsExcludeReply)
                            {
                                reply = true;
                            }

                            if (post.IsDm)
                            {
                                dm = true;
                            }

                            if (sb.Length > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }

                            switch (this.settingDialog.NameBalloon)
                            {
                                case NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }

                            sb.Append(post.TextFromApi);
                            if (notifyPosts.Count() > 3)
                            {
                                if (!object.ReferenceEquals(notifyPosts.Last(), post))
                                {
                                    continue;
                                }
                            }

                            StringBuilder title = new StringBuilder();
                            GrowlHelper.NotifyType nt = default(GrowlHelper.NotifyType);
                            if (this.settingDialog.DispUsername)
                            {
                                title.Append(this.tw.Username);
                                title.Append(" - ");
                            }
                            else
                            {
                                // title.Clear()
                            }

                            if (dm)
                            {
                                title.Append("Hoehoe [DM] ");
                                title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText2);
                                nt = GrowlHelper.NotifyType.DirectMessage;
                            }
                            else if (reply)
                            {
                                title.Append("Hoehoe [Reply!] ");
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                                nt = GrowlHelper.NotifyType.Reply;
                            }
                            else
                            {
                                title.Append("Hoehoe ");
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                                nt = GrowlHelper.NotifyType.Notify;
                            }

                            string notifyText = sb.ToString();
                            if (string.IsNullOrEmpty(notifyText))
                            {
                                return;
                            }

                            this.growlHelper.Notify(nt, post.StatusId.ToString(), title.ToString(), notifyText, this.iconDict[post.ImageUrl], post.ImageUrl);
                        }
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        bool reply = false;
                        bool dm = false;
                        foreach (PostClass post in notifyPosts)
                        {
                            if (post.IsReply && !post.IsExcludeReply)
                            {
                                reply = true;
                            }

                            if (post.IsDm)
                            {
                                dm = true;
                            }

                            if (sb.Length > 0)
                            {
                                sb.Append(System.Environment.NewLine);
                            }

                            switch (this.settingDialog.NameBalloon)
                            {
                                case NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }

                            sb.Append(post.TextFromApi);
                        }

                        StringBuilder title = new StringBuilder();
                        ToolTipIcon notifyIcon = default(ToolTipIcon);
                        if (this.settingDialog.DispUsername)
                        {
                            title.Append(this.tw.Username);
                            title.Append(" - ");
                        }
                        else
                        {
                        }

                        if (dm)
                        {
                            notifyIcon = ToolTipIcon.Warning;
                            title.Append("Hoehoe [DM] ");
                            title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText2);
                        }
                        else if (reply)
                        {
                            notifyIcon = ToolTipIcon.Warning;
                            title.Append("Hoehoe [Reply!] ");
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                        }
                        else
                        {
                            notifyIcon = ToolTipIcon.Info;
                            title.Append("Hoehoe ");
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                        }

                        string notifyText = sb.ToString();
                        if (string.IsNullOrEmpty(notifyText))
                        {
                            return;
                        }

                        this.NotifyIcon1.BalloonTipTitle = title.ToString();
                        this.NotifyIcon1.BalloonTipText = notifyText;
                        this.NotifyIcon1.BalloonTipIcon = notifyIcon;
                        this.NotifyIcon1.ShowBalloonTip(500);
                    }
                }
            }

            // サウンド再生
            if (!this.isInitializing && this.settingDialog.PlaySound && !string.IsNullOrEmpty(soundFile))
            {
                try
                {
                    string dir = MyCommon.AppDir;
                    if (Directory.Exists(Path.Combine(dir, "Sounds")))
                    {
                        dir = Path.Combine(dir, "Sounds");
                    }

                    new SoundPlayer(Path.Combine(dir, soundFile)).Play();
                }
                catch (Exception)
                {
                }
            }

            // mentions新着時に画面ブリンク
            if (!this.isInitializing && this.settingDialog.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                Win32Api.FlashMyWindow(this.Handle, Hoehoe.Win32Api.FlashSpecification.FlashTray, 3);
            }
        }

        private void ChangeCacheStyleRead(bool read, int index, TabPage tab)
        {
            // Read_:True=既読 False=未読
            // 未読管理していなかったら既読として扱う
            if (!this.statuses.Tabs[this.curTab.Text].UnreadManage || !this.settingDialog.UnreadManage)
            {
                read = true;
            }

            // 対象の特定
            ListViewItem itm = null;
            PostClass post = null;
            if (tab.Equals(this.curTab) && this.itemCache != null && index >= this.itemCacheIndex && index < this.itemCacheIndex + this.itemCache.Length)
            {
                itm = this.itemCache[index - this.itemCacheIndex];
                post = this.postCache[index - this.itemCacheIndex];
            }
            else
            {
                itm = ((DetailsListView)tab.Tag).Items[index];
                post = this.statuses.Item(tab.Text, index);
            }

            this.ChangeItemStyleRead(read, itm, post, (DetailsListView)tab.Tag);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem item, PostClass post, DetailsListView listView)
        {
            // フォント
            Font fnt = read ? this.fntReaded : this.fntUnread;
            item.SubItems[5].Text = read ? string.Empty : "★";

            // 文字色
            Color cl = default(Color);
            if (post.IsFav)
            {
                cl = this.clrFav;
            }
            else if (post.IsRetweeted)
            {
                cl = this.clrRetweet;
            }
            else if (post.IsOwl && (post.IsDm || this.settingDialog.OneWayLove))
            {
                cl = this.clrOWL;
            }
            else if (read || !this.settingDialog.UseUnreadStyle)
            {
                cl = this.clrRead;
            }
            else
            {
                cl = this.clrUnread;
            }

            if (listView == null || item.Index == -1)
            {
                item.ForeColor = cl;
                if (this.settingDialog.UseUnreadStyle)
                {
                    item.Font = fnt;
                }
            }
            else
            {
                listView.Update();
                if (this.settingDialog.UseUnreadStyle)
                {
                    listView.ChangeItemFontAndColor(item.Index, cl, fnt);
                }
                else
                {
                    listView.ChangeItemForeColor(item.Index, cl);
                }
            }
        }

        private void ColorizeList()
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass post = this.anchorFlag ? this.anchorPost : this.curPost;

            if (this.itemCache == null)
            {
                return;
            }

            if (post == null)
            {
                return;
            }

            try
            {
                for (int cnt = 0; cnt < this.itemCache.Length; cnt++)
                {
                    this.curList.ChangeItemBackColor(this.itemCacheIndex + cnt, this.JudgeColor(post, this.postCache[cnt]));
                }
            }
            catch (Exception)
            {
            }
        }

        private void ColorizeList(ListViewItem item, int index)
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass post = this.anchorFlag ? this.anchorPost : this.curPost;
            PostClass target = this.GetCurTabPost(index);
            if (post == null)
            {
                return;
            }

            if (item.Index == -1)
            {
                item.BackColor = this.JudgeColor(post, target);
            }
            else
            {
                this.curList.ChangeItemBackColor(item.Index, this.JudgeColor(post, target));
            }
        }

        private Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl = default(Color);
            if (targetPost.StatusId == basePost.InReplyToStatusId)
            {
                // @先
                cl = this.clrAtTo;
            }
            else if (targetPost.IsMe)
            {
                // 自分=発言者
                cl = this.clrSelf;
            }
            else if (targetPost.IsReply)
            {
                // 自分宛返信
                cl = this.clrAtSelf;
            }
            else if (basePost.ReplyToList.Contains(targetPost.ScreenName.ToLower()))
            {
                // 返信先
                cl = this.clrAtFromTarget;
            }
            else if (targetPost.ReplyToList.Contains(basePost.ScreenName.ToLower()))
            {
                // その人への返信
                cl = this.clrAtTarget;
            }
            else if (targetPost.ScreenName.Equals(basePost.ScreenName, StringComparison.OrdinalIgnoreCase))
            {
                // 発言者
                cl = this.clrTarget;
            }
            else
            {
                // その他
                cl = this.clrListBackcolor;
            }

            return cl;
        }

        private string MakeStatusMessage(GetWorkerArg asyncArg, bool isFinish)
        {
            string smsg = string.Empty;
            if (!isFinish)
            {
                // 継続中メッセージ
                switch (asyncArg.WorkerType)
                {
                    case WorkerType.Timeline:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5 + asyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.Reply:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText4 + asyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.DirectMessegeRcv:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText8 + asyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.FavAdd:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15 + asyncArg.Page.ToString() + "/" + asyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText16 + (asyncArg.Page - asyncArg.SIds.Count - 1).ToString();
                        break;
                    case WorkerType.FavRemove:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17 + asyncArg.Page.ToString() + "/" + asyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText18 + (asyncArg.Page - asyncArg.SIds.Count - 1).ToString();
                        break;
                    case WorkerType.Favorites:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText19;
                        break;
                    case WorkerType.PublicSearch:
                        smsg = "Search refreshing...";
                        break;
                    case WorkerType.List:
                        smsg = "List refreshing...";
                        break;
                    case WorkerType.Related:
                        smsg = "Related refreshing...";
                        break;
                    case WorkerType.UserTimeline:
                        smsg = "UserTimeline refreshing...";
                        break;
                }
            }
            else
            {
                // 完了メッセージ
                switch (asyncArg.WorkerType)
                {
                    case WorkerType.Timeline:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText1;
                        break;
                    case WorkerType.Reply:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText9;
                        break;
                    case WorkerType.DirectMessegeRcv:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText11;
                        break;
                    case WorkerType.DirectMessegeSnt:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText13;
                        break;
                    case WorkerType.FavAdd:
                        // 進捗メッセージ残す
                        break;
                    case WorkerType.FavRemove:
                        // 進捗メッセージ残す
                        break;
                    case WorkerType.Favorites:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText20;
                        break;
                    case WorkerType.Follower:
                        smsg = Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText3;
                        break;
                    case WorkerType.Configuration:
                        // 進捗メッセージ残す
                        break;
                    case WorkerType.PublicSearch:
                        smsg = "Search refreshed";
                        break;
                    case WorkerType.List:
                        smsg = "List refreshed";
                        break;
                    case WorkerType.Related:
                        smsg = "Related refreshed";
                        break;
                    case WorkerType.UserTimeline:
                        smsg = "UserTimeline refreshed";
                        break;
                    case WorkerType.BlockIds:
                        smsg = Hoehoe.Properties.Resources.UpdateBlockUserText3;
                        break;
                }
            }

            return smsg;
        }

        private void RemovePostFromFavTab(long[] ids)
        {
            string favTabName = this.statuses.GetTabByType(TabUsageType.Favorites).TabName;
            int fidx = 0;
            if (this.curTab.Text.Equals(favTabName))
            {
                if (this.curList.FocusedItem != null)
                {
                    fidx = this.curList.FocusedItem.Index;
                }
                else if (this.curList.TopItem != null)
                {
                    fidx = this.curList.TopItem.Index;
                }
                else
                {
                    fidx = 0;
                }
            }

            foreach (long i in ids)
            {
                try
                {
                    this.statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (this.curTab != null && this.curTab.Text.Equals(favTabName))
            {
                // キャッシュ破棄
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                //// this._curItemIndex = -1
            }

            foreach (TabPage tp in this.ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = this.statuses.Tabs[favTabName].AllCount;
                    break;
                }
            }

            if (this.curTab.Text.Equals(favTabName))
            {
                do
                {
                    this.curList.SelectedIndices.Clear();
                }
                while (this.curList.SelectedIndices.Count > 0);

                if (this.statuses.Tabs[favTabName].AllCount > 0)
                {
                    if (this.statuses.Tabs[favTabName].AllCount - 1 > fidx && fidx > -1)
                    {
                        this.curList.SelectedIndices.Add(fidx);
                    }
                    else
                    {
                        this.curList.SelectedIndices.Add(this.statuses.Tabs[favTabName].AllCount - 1);
                    }

                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        this.curList.EnsureVisible(this.curList.SelectedIndices[0]);
                        this.curList.FocusedItem = this.curList.Items[this.curList.SelectedIndices[0]];
                    }
                }
            }
        }

        private void GetTimeline(WorkerType workerType, int fromPage, int toPage, string tabName)
        {
            if (!MyCommon.IsNetworkAvailable())
            {
                return;
            }

            if (this.lastTimeWork == null)
            {
                this.lastTimeWork = new Dictionary<WorkerType, DateTime>();
            }

            // 非同期実行引数設定
            if (!this.lastTimeWork.ContainsKey(workerType))
            {
                this.lastTimeWork.Add(workerType, new DateTime());
            }

            double period = DateTime.Now.Subtract(this.lastTimeWork[workerType]).TotalSeconds;
            if (period > 1 || period < -1)
            {
                this.lastTimeWork[workerType] = DateTime.Now;
                this.RunAsync(new GetWorkerArg() { Page = fromPage, EndPage = toPage, WorkerType = workerType, TabName = tabName });
            }
        }

        private void FavoriteChange(bool isFavAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            // TrueでFavAdd,FalseでFavRemove
            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage || this.curList.SelectedIndices.Count == 0 || !this.ExistCurrentPost)
            {
                return;
            }

            // 複数fav確認msg
            if (this.curList.SelectedIndices.Count > 250 && isFavAdd)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.FavoriteLimitCountText);
                this.doFavRetweetFlags = false;
                return;
            }

            if (multiFavoriteChangeDialogEnable && this.curList.SelectedIndices.Count > 1)
            {
                if (isFavAdd)
                {
                    string confirmMessage = Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText1;
                    if (this.doFavRetweetFlags)
                    {
                        confirmMessage = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText3;
                    }

                    if (MessageBox.Show(confirmMessage, Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        this.doFavRetweetFlags = false;
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show(Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText1, Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            GetWorkerArg args = new GetWorkerArg()
            {
                Ids = new List<long>(),
                SIds = new List<long>(),
                TabName = this.curTab.Text,
                WorkerType = isFavAdd ? WorkerType.FavAdd : WorkerType.FavRemove
            };

            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.GetCurTabPost(idx);
                if (isFavAdd)
                {
                    if (!post.IsFav)
                    {
                        args.Ids.Add(post.StatusId);
                    }
                }
                else
                {
                    if (post.IsFav)
                    {
                        args.Ids.Add(post.StatusId);
                    }
                }
            }

            if (args.Ids.Count == 0)
            {
                if (isFavAdd)
                {
                    this.StatusLabel.Text = Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText4;
                }
                else
                {
                    this.StatusLabel.Text = Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;
                }

                return;
            }

            this.RunAsync(args);
        }

        private PostClass GetCurTabPost(int index)
        {
            if (this.postCache != null && index >= this.itemCacheIndex && index < this.itemCacheIndex + this.postCache.Length)
            {
                return this.postCache[index - this.itemCacheIndex];
            }
            else
            {
                return this.statuses.Item(this.curTab.Text, index);
            }
        }

        private void DoStatusDelete()
        {
            if (this.curTab == null || this.curList == null)
            {
                return;
            }

            if (this.statuses.Tabs[this.curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                bool myPost = false;
                foreach (int idx in this.curList.SelectedIndices)
                {
                    if (this.GetCurTabPost(idx).IsMe || this.GetCurTabPost(idx).RetweetedBy.ToLower() == this.tw.Username.ToLower())
                    {
                        myPost = true;
                        break;
                    }
                }

                if (!myPost)
                {
                    return;
                }
            }
            else
            {
                if (this.curList.SelectedIndices.Count == 0)
                {
                    return;
                }
            }

            string tmp = string.Format(Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText1, Environment.NewLine);

            if (MessageBox.Show(tmp, Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }

            int fidx = 0;
            if (this.curList.FocusedItem != null)
            {
                fidx = this.curList.FocusedItem.Index;
            }
            else if (this.curList.TopItem != null)
            {
                fidx = this.curList.TopItem.Index;
            }
            else
            {
                fidx = 0;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                bool rslt = true;
                foreach (long id in this.statuses.GetId(this.curTab.Text, this.curList.SelectedIndices))
                {
                    string rtn = string.Empty;
                    if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage)
                    {
                        rtn = this.tw.RemoveDirectMessage(id, this.statuses.Item(id));
                    }
                    else
                    {
                        if (this.statuses.Item(id).IsMe || this.statuses.Item(id).RetweetedBy.ToLower() == this.tw.Username.ToLower())
                        {
                            rtn = this.tw.RemoveStatus(id);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (rtn.Length > 0)
                    {
                        // エラー
                        rslt = false;
                    }
                    else
                    {
                        this.statuses.RemovePost(id);
                    }
                }

                if (rslt)
                {
                    // 成功
                    this.StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText4;
                }
                else
                {
                    // 失敗
                    this.StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText3;
                }

                // キャッシュ破棄
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.curTab.Equals(tb))
                    {
                        do
                        {
                            this.curList.SelectedIndices.Clear();
                        }
                        while (this.curList.SelectedIndices.Count > 0);

                        if (this.statuses.Tabs[tb.Text].AllCount > 0)
                        {
                            if (this.statuses.Tabs[tb.Text].AllCount - 1 > fidx && fidx > -1)
                            {
                                this.curList.SelectedIndices.Add(fidx);
                            }
                            else
                            {
                                this.curList.SelectedIndices.Add(this.statuses.Tabs[tb.Text].AllCount - 1);
                            }

                            if (this.curList.SelectedIndices.Count > 0)
                            {
                                this.curList.EnsureVisible(this.curList.SelectedIndices[0]);
                                this.curList.FocusedItem = this.curList.Items[this.curList.SelectedIndices[0]];
                            }
                        }
                    }

                    if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            if (tb.ImageIndex == 0)
                            {
                                // タブアイコン
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }

                if (!this.settingDialog.TabIconDisp)
                {
                    this.ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void DoRefresh()
        {
            if (this.curTab != null)
            {
                switch (this.statuses.Tabs[this.curTab.Text].TabType)
                {
                    case TabUsageType.Mentions:
                        this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                        break;
                    case TabUsageType.DirectMessage:
                        this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                        break;
                    case TabUsageType.Favorites:
                        this.GetTimeline(WorkerType.Favorites, 1, 0, string.Empty);
                        break;
                    case TabUsageType.PublicSearch:
                        {
                            // ' TODO
                            TabClass tb = this.statuses.Tabs[this.curTab.Text];
                            if (string.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }

                            this.GetTimeline(WorkerType.PublicSearch, 1, 0, this.curTab.Text);
                        }

                        break;
                    case TabUsageType.UserTimeline:
                        this.GetTimeline(WorkerType.UserTimeline, 1, 0, this.curTab.Text);
                        break;
                    case TabUsageType.Lists:
                        {
                            // ' TODO
                            TabClass tb = this.statuses.Tabs[this.curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }

                            this.GetTimeline(WorkerType.List, 1, 0, this.curTab.Text);
                        }

                        break;
                    default:
                        this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        break;
                }
            }
            else
            {
                this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
            }
        }

        private void DoRefreshMore()
        {
            // ページ指定をマイナス1に
            if (this.curTab != null)
            {
                switch (this.statuses.Tabs[this.curTab.Text].TabType)
                {
                    case TabUsageType.Mentions:
                        this.GetTimeline(WorkerType.Reply, -1, 0, string.Empty);
                        break;
                    case TabUsageType.DirectMessage:
                        this.GetTimeline(WorkerType.DirectMessegeRcv, -1, 0, string.Empty);
                        break;
                    case TabUsageType.Favorites:
                        this.GetTimeline(WorkerType.Favorites, -1, 0, string.Empty);
                        break;
                    case TabUsageType.Profile:
                        break;
                    case TabUsageType.PublicSearch:
                        {
                            // TODO
                            TabClass tb = this.statuses.Tabs[this.curTab.Text];
                            if (string.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }

                            this.GetTimeline(WorkerType.PublicSearch, -1, 0, this.curTab.Text);
                        }

                        break;
                    case TabUsageType.UserTimeline:
                        this.GetTimeline(WorkerType.UserTimeline, -1, 0, this.curTab.Text);
                        break;
                    case TabUsageType.Lists:
                        {
                            // ' TODO
                            TabClass tb = this.statuses.Tabs[this.curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }

                            this.GetTimeline(WorkerType.List, -1, 0, this.curTab.Text);
                        }

                        break;
                    default:
                        this.GetTimeline(WorkerType.Timeline, -1, 0, string.Empty);
                        break;
                }
            }
            else
            {
                this.GetTimeline(WorkerType.Timeline, -1, 0, string.Empty);
            }
        }

        private void ShowUserTimeline()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.AddNewTabForUserTimeline(this.curPost.ScreenName);
        }

        private void SetListProperty()
        {
            // 削除などで見つからない場合は処理せず
            if (this.curList == null)
            {
                return;
            }

            if (!this.isColumnChanged)
            {
                return;
            }

            int[] dispOrder = new int[this.curList.Columns.Count];
            for (int i = 0; i < this.curList.Columns.Count; i++)
            {
                for (int j = 0; j < this.curList.Columns.Count; j++)
                {
                    if (this.curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            // 列幅、列並びを他のタブに設定
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (!tb.Equals(this.curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i <= lst.Columns.Count - 1; i++)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = this.curList.Columns[i].Width;
                        }
                    }
                }
            }

            this.isColumnChanged = false;
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            // 文字数カウント
            int len = 140 - this.StatusText.Text.Length;
            if (this.NotifyIcon1 == null || !this.NotifyIcon1.Visible)
            {
                return len;
            }

            if ((isAuto && !this.IsKeyDown(Keys.Control) && this.settingDialog.PostShiftEnter)
                || (isAuto && !this.IsKeyDown(Keys.Shift) && !this.settingDialog.PostShiftEnter)
                || (!isAuto && isAddFooter))
            {
                if (this.settingDialog.UseRecommendStatus)
                {
                    len -= this.settingDialog.RecommendStatusText.Length;
                }
                else if (this.settingDialog.Status.Length > 0)
                {
                    len -= this.settingDialog.Status.Length + 1;
                }
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                len -= this.HashMgr.UseHash.Length + 1;
            }

            foreach (Match m in Regex.Matches(this.StatusText.Text, Twitter.UrlRegexPattern, RegexOptions.IgnoreCase))
            {
                len += m.Result("${url}").Length - this.settingDialog.TwitterConfiguration.ShortUrlLength;
            }

            if (this.ImageSelectionPanel.Visible && this.ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(this.ImageService))
            {
                len -= this.settingDialog.TwitterConfiguration.CharactersReservedPerMedia;
            }

            return len;
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            try
            {
                // キャッシュ要求（要求範囲±30を作成）
                startIndex -= 30;
                if (startIndex < 0)
                {
                    startIndex = 0;
                }

                endIndex += 30;
                if (endIndex >= this.statuses.Tabs[this.curTab.Text].AllCount)
                {
                    endIndex = this.statuses.Tabs[this.curTab.Text].AllCount - 1;
                }

                this.postCache = this.statuses.Item(this.curTab.Text, startIndex, endIndex); // 配列で取得
                this.itemCacheIndex = startIndex;
                this.itemCache = new ListViewItem[this.postCache.Length];
                for (int i = 0; i <= this.postCache.Length - 1; i++)
                {
                    this.itemCache[i] = this.CreateItem(this.curTab, this.postCache[i], startIndex + i);
                }
            }
            catch (Exception)
            {
                // キャッシュ要求が実データとずれるため（イベントの遅延？）
                this.postCache = null;
                this.itemCache = null;
            }
        }

        private ListViewItem CreateItem(TabPage tabPage, PostClass post, int index)
        {
            StringBuilder mk = new StringBuilder();
            if (post.FavoritedCount > 0)
            {
                mk.Append("+" + post.FavoritedCount.ToString());
            }

            string postedByDetail = post.ScreenName;
            if (post.IsRetweeted)
            {
                postedByDetail += string.Format("{0}(RT:{1})", Environment.NewLine, post.RetweetedBy);
            }
            string[] sitem = { string.Empty, post.Nickname, post.IsDeleted ? "(DELETED)" : post.TextFromApi, post.CreatedAt.ToString(this.settingDialog.DateTimeFormat), postedByDetail, string.Empty, mk.ToString(), post.Source };
            ImageListViewItem itm = new ImageListViewItem(sitem, this.iconDict, post.ImageUrl);
            itm.StateImageIndex = post.StateIndex;

            bool read = post.IsRead;
            if (!this.statuses.Tabs[tabPage.Text].UnreadManage || !this.settingDialog.UnreadManage)
            {
                // 未読管理していなかったら既読として扱う
                read = true;
            }

            this.ChangeItemStyleRead(read, itm, post, null);
            if (tabPage.Equals(this.curTab))
            {
                this.ColorizeList(itm, index);
            }

            return itm;
        }

        private void DrawListViewItemIcon(DrawListViewItemEventArgs e)
        {
            ImageListViewItem item = (ImageListViewItem)e.Item;
            Rectangle stateRect = default(Rectangle);

            // e.Bounds.Leftが常に0を指すから自前で計算
            Rectangle itemRect = item.Bounds;
            itemRect.Width = e.Item.ListView.Columns[0].Width;

            foreach (ColumnHeader clm in e.Item.ListView.Columns)
            {
                if (clm.DisplayIndex < e.Item.ListView.Columns[0].DisplayIndex)
                {
                    itemRect.X += clm.Width;
                }
            }

            Rectangle iconRect = default(Rectangle);
            if (item.Image != null)
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(this.iconSz, this.iconSz)), itemRect);
                iconRect.Offset(0, Convert.ToInt32(Math.Max(0, (itemRect.Height - this.iconSz) / 2)));
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this.iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }
            else
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(1, 1)), itemRect);
                //// iconRect.Offset(0, CType(Math.Max(0, (itemRect.Height - this._iconSz) / 2), Integer))
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this.iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }

            if (item.Image != null && iconRect.Width > 0)
            {
                e.Graphics.FillRectangle(Brushes.White, iconRect);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                try
                {
                    e.Graphics.DrawImage(item.Image, iconRect);
                }
                catch (ArgumentException)
                {
                    item.RegetImage();
                }
            }

            if (item.StateImageIndex > -1)
            {
                if (stateRect.Width > 0)
                {
                    e.Graphics.DrawImage(this.PostStateImageList.Images[item.StateImageIndex], stateRect);
                }
            }
        }

        private void DoTabSearch(string word, bool isCaseSensitive, bool isUseRegex, SEARCHTYPE searchType)
        {
            if (this.curList.VirtualListSize == 0)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            int cidx = 0;
            if (this.curList.SelectedIndices.Count > 0)
            {
                cidx = this.curList.SelectedIndices[0];
            }

            int toIdx = this.curList.VirtualListSize - 1;
            int stp = 1;
            switch (searchType)
            {
                case SEARCHTYPE.DialogSearch:
                    // ダイアログからの検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0];
                    }
                    else
                    {
                        cidx = 0;
                    }

                    break;
                case SEARCHTYPE.NextSearch:
                    // 次を検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0] + 1;
                        if (cidx > toIdx)
                        {
                            cidx = toIdx;
                        }
                    }
                    else
                    {
                        cidx = 0;
                    }

                    break;
                case SEARCHTYPE.PrevSearch:
                    // 前を検索
                    if (this.curList.SelectedIndices.Count > 0)
                    {
                        cidx = this.curList.SelectedIndices[0] - 1;
                        if (cidx < 0)
                        {
                            cidx = 0;
                        }
                    }
                    else
                    {
                        cidx = toIdx;
                    }

                    toIdx = 0;
                    stp = -1;
                    break;
            }

            bool fnd = false;
            RegexOptions regOpt = RegexOptions.None;
            StringComparison fndOpt = StringComparison.Ordinal;
            if (!isCaseSensitive)
            {
                regOpt = RegexOptions.IgnoreCase;
                fndOpt = StringComparison.OrdinalIgnoreCase;
            }

            try
            {
            RETRY:
                if (isUseRegex)
                {
                    // 正規表現検索
                    try
                    {
                        Regex searchRegex = new Regex(word, regOpt);
                        for (int idx = cidx; idx <= toIdx; idx += stp)
                        {
                            PostClass post = null;
                            try
                            {
                                post = this.statuses.Item(this.curTab.Text, idx);
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            if (searchRegex.IsMatch(post.Nickname) || searchRegex.IsMatch(post.TextFromApi) || searchRegex.IsMatch(post.ScreenName))
                            {
                                this.SelectListItem(this.curList, idx);
                                this.curList.EnsureVisible(idx);
                                return;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText1, "Hoehoe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 通常検索
                    for (int idx = cidx; idx <= toIdx; idx += stp)
                    {
                        PostClass post = null;
                        try
                        {
                            post = this.statuses.Item(this.curTab.Text, idx);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (post.Nickname.IndexOf(word, fndOpt) > -1 || post.TextFromApi.IndexOf(word, fndOpt) > -1 || post.ScreenName.IndexOf(word, fndOpt) > -1)
                        {
                            this.SelectListItem(this.curList, idx);
                            this.curList.EnsureVisible(idx);
                            return;
                        }
                    }
                }

                if (!fnd)
                {
                    switch (searchType)
                    {
                        case SEARCHTYPE.DialogSearch:
                        case SEARCHTYPE.NextSearch:
                            toIdx = cidx;
                            cidx = 0;
                            break;
                        case SEARCHTYPE.PrevSearch:
                            toIdx = cidx;
                            cidx = this.curList.Items.Count - 1;
                            break;
                    }

                    fnd = true;
                    goto RETRY;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // TODO: to hoehoe2
        private void RunTweenUp()
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    WorkingDirectory = MyCommon.SettingPath,
                    FileName = Path.Combine(MyCommon.SettingPath, "TweenUp3.exe"),
                    Arguments = "\"" + Application.StartupPath + "\""
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to execute TweenUp3.exe.");
            }
        }

        private void CheckNewVersion(bool startup = false)
        {
            string retMsg = string.Empty;
            string strVer = string.Empty;
            string strDetail = string.Empty;
            bool forceUpdate = this.IsKeyDown(Keys.Shift);

            try
            {
                retMsg = this.tw.GetVersionInfo();
            }
            catch (Exception)
            {
                this.StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText10, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                }

                return;
            }

            if (retMsg.Length > 0)
            {
                strVer = retMsg.Substring(0, 4);
                if (retMsg.Length > 4)
                {
                    strDetail = retMsg.Substring(5).Trim();
                }

                if (!string.IsNullOrEmpty(MyCommon.FileVersion) && strVer.CompareTo(MyCommon.FileVersion.Replace(".", string.Empty)) > 0)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.CheckNewVersionText3, strVer);
                    using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                    {
                        if (dialogAsShieldicon.ShowDialog(tmp, strDetail, Hoehoe.Properties.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            retMsg = this.tw.GetTweenBinary(strVer);
                            if (retMsg.Length == 0)
                            {
                                this.RunTweenUp();
                                MyCommon.IsEnding = true;
                                this.Close();
                                return;
                            }

                            if (!startup)
                            {
                                MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                }
                else
                {
                    if (forceUpdate)
                    {
                        string tmp = string.Format(Hoehoe.Properties.Resources.CheckNewVersionText6, strVer);
                        using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                        {
                            if (dialogAsShieldicon.ShowDialog(tmp, strDetail, Hoehoe.Properties.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                retMsg = this.tw.GetTweenBinary(strVer);
                                if (retMsg.Length == 0)
                                {
                                    this.RunTweenUp();
                                    MyCommon.IsEnding = true;
                                    this.Close();
                                    return;
                                }
                                if (!startup)
                                {
                                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                    }
                    else if (!startup)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText7 + MyCommon.FileVersion.Replace(".", string.Empty) + Hoehoe.Properties.Resources.CheckNewVersionText8 + strVer, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                this.StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText10, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Colorize()
        {
            this.colorize = false;
            this.DispSelectedPost();

            // 件数関連の場合、タイトル即時書き換え
            if (this.settingDialog.DispLatestPost != DispTitleEnum.None && this.settingDialog.DispLatestPost != DispTitleEnum.Post && this.settingDialog.DispLatestPost != DispTitleEnum.Ver && this.settingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                this.SetMainWindowTitle();
            }

            if (!this.StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }

            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this.settingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        private void DispSelectedPost(bool forceupdate = false)
        {
            if (this.curList.SelectedIndices.Count == 0 || this.curPost == null)
            {
                return;
            }

            if (!forceupdate && this.curPost.Equals(this.displayPost))
            {
                return;
            }

            this.displayPost = this.curPost;
            if (this.displayItem != null)
            {
                this.displayItem.ImageDownloaded -= this.DisplayItemImage_Downloaded;
                this.displayItem = null;
            }

            this.displayItem = (ImageListViewItem)this.curList.Items[this.curList.SelectedIndices[0]];
            this.displayItem.ImageDownloaded += this.DisplayItemImage_Downloaded;

            string detailText = this.CreateDetailHtml(this.curPost.IsDeleted ? "(DELETED)" : this.curPost.Text);
            if (this.curPost.IsDm)
            {
                this.SourceLinkLabel.Tag = null;
                this.SourceLinkLabel.Text = string.Empty;
            }
            else
            {
                Match mc = Regex.Match(this.curPost.SourceHtml, "<a href=\"(?<sourceurl>.+?)\"");
                if (mc.Success)
                {
                    string src = mc.Groups["sourceurl"].Value;
                    this.SourceLinkLabel.Tag = mc.Groups["sourceurl"].Value;
                    mc = Regex.Match(src, "^https?:// ");
                    if (!mc.Success)
                    {
                        src = src.Insert(0, "http://twitter.com");
                    }

                    this.SourceLinkLabel.Tag = src;
                }
                else
                {
                    this.SourceLinkLabel.Tag = null;
                }

                if (string.IsNullOrEmpty(this.curPost.Source))
                {
                    this.SourceLinkLabel.Text = string.Empty;
                }
                else
                {
                    this.SourceLinkLabel.Text = this.curPost.Source;
                }
            }

            this.SourceLinkLabel.TabStop = false;

            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage && !this.curPost.IsOwl)
            {
                this.NameLabel.Text = "DM TO -> ";
            }
            else if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                this.NameLabel.Text = "DM FROM <- ";
            }
            else
            {
                this.NameLabel.Text = string.Empty;
            }

            this.NameLabel.Text += this.curPost.ScreenName + "/" + this.curPost.Nickname;
            this.NameLabel.Tag = this.curPost.ScreenName;
            if (!string.IsNullOrEmpty(this.curPost.RetweetedBy))
            {
                this.NameLabel.Text += string.Format(" (RT:{0})", this.curPost.RetweetedBy);
            }

            if (!string.IsNullOrEmpty(this.curPost.ImageUrl))
            {
                this.UserPicture.ReplaceImage(this.iconDict[this.curPost.ImageUrl]);
            }
            else
            {
                this.UserPicture.ClearImage();
            }
            
            this.NameLabel.ForeColor = SystemColors.ControlText;
            this.DateTimeLabel.Text = this.curPost.CreatedAt.ToString();
            if (this.curPost.IsOwl && (this.settingDialog.OneWayLove || this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage))
            {
                this.NameLabel.ForeColor = this.clrOWL;
            }

            if (this.curPost.IsRetweeted)
            {
                this.NameLabel.ForeColor = this.clrRetweet;
            }

            if (this.curPost.IsFav)
            {
                this.NameLabel.ForeColor = this.clrFav;
            }

            if (this.DumpPostClassToolStripMenuItem.Checked)
            {
                StringBuilder sb = new StringBuilder(512);
                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", this.curPost.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", this.curPost.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", this.curPost.StatusId.ToString());
                //// sb.AppendFormat("ImageIndex     : {0}<br>", this._curPost.ImageIndex.ToString)
                sb.AppendFormat("ImageUrl       : {0}<br>", this.curPost.ImageUrl);
                sb.AppendFormat("InReplyToStatusId    : {0}<br>", this.curPost.InReplyToStatusId.ToString());
                sb.AppendFormat("InReplyToUser  : {0}<br>", this.curPost.InReplyToUser);
                sb.AppendFormat("IsDM           : {0}<br>", this.curPost.IsDm);
                sb.AppendFormat("IsFav          : {0}<br>", this.curPost.IsFav);
                sb.AppendFormat("IsMark         : {0}<br>", this.curPost.IsMark);
                sb.AppendFormat("IsMe           : {0}<br>", this.curPost.IsMe);
                sb.AppendFormat("IsOwl          : {0}<br>", this.curPost.IsOwl);
                sb.AppendFormat("IsProtect      : {0}<br>", this.curPost.IsProtect);
                sb.AppendFormat("IsRead         : {0}<br>", this.curPost.IsRead);
                sb.AppendFormat("IsReply        : {0}<br>", this.curPost.IsReply);

                foreach (string nm in this.curPost.ReplyToList)
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName     : {0}<br>", this.curPost.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", this.curPost.Nickname);
                sb.AppendFormat("Text           : {0}<br>", this.curPost.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", this.curPost.Text);
                sb.AppendFormat("CreatedAt      : {0}<br>", this.curPost.CreatedAt);
                sb.AppendFormat("Source         : {0}<br>", this.curPost.Source);
                sb.AppendFormat("UserId         : {0}<br>", this.curPost.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", this.curPost.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", this.curPost.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", this.curPost.RetweetedId);
                sb.AppendFormat("SearchTabName  : {0}<br>", this.curPost.RelTabName);
                sb.Append("-----End PostClass Dump<br>");

                this.PostBrowser.Visible = false;
                this.PostBrowser.DocumentText = this.detailHtmlFormatHeader + sb.ToString() + this.detailHtmlFormatFooter;
                this.PostBrowser.Visible = true;
            }
            else
            {
                try
                {
                    if (this.PostBrowser.DocumentText != detailText)
                    {
                        this.PostBrowser.Visible = false;
                        this.PostBrowser.DocumentText = detailText;
                        List<string> lnks = new List<string>();
                        foreach (Match lnk in Regex.Matches(detailText, "<a target=\"_self\" href=\"(?<url>http[^\"]+)\"", RegexOptions.IgnoreCase))
                        {
                            lnks.Add(lnk.Result("${url}"));
                        }

                        this.thumbnail.GenThumbnail(this.curPost.StatusId, lnks, this.curPost.PostGeo, this.curPost.Media);
                    }
                }
                catch (COMException comex)
                {
                    // 原因不明
                    System.Diagnostics.Debug.Write(comex);
                }
                catch (UriFormatException)
                {
                    this.PostBrowser.DocumentText = detailText;
                }
                finally
                {
                    this.PostBrowser.Visible = true;
                }
            }
        }

        private ModifierState GetModifierState(bool isCtrl, bool isShift, bool isAlt)
        {
            ModifierState state = ModifierState.None;
            if (isCtrl)
            {
                state = state | ModifierState.Ctrl;
            }

            if (isShift)
            {
                state = state | ModifierState.Shift;
            }

            if (isAlt)
            {
                state = state | ModifierState.Alt;
            }

            return state;
        }

        private bool CommonKeyDown(Keys keyCode, FocusedControl focusedControl, ModifierState modifierState)
        {
            bool functionReturnValue = false;
            if (focusedControl == FocusedControl.ListTab)
            {
                // リストのカーソル移動関係（上下キー、PageUp/Downに該当）
                if (modifierState == (ModifierState.Ctrl | ModifierState.Shift) || modifierState == ModifierState.Ctrl || modifierState == ModifierState.None || modifierState == ModifierState.Shift)
                {
                    if (keyCode == Keys.J)
                    {
                        SendKeys.Send("{DOWN}");
                        return true;
                    }
                    else if (keyCode == Keys.K)
                    {
                        SendKeys.Send("{UP}");
                        return true;
                    }
                }

                if (modifierState == ModifierState.Shift || modifierState == ModifierState.None)
                {
                    if (keyCode == Keys.F)
                    {
                        SendKeys.Send("{PGDN}");
                        return true;
                    }
                    else if (keyCode == Keys.B)
                    {
                        SendKeys.Send("{PGUP}");
                        return true;
                    }
                }
            }

            // 修飾キーなし
            switch (modifierState)
            {
                case ModifierState.None:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F1:
                            this.OpenUriAsync(ApplicationHelpWebPageUrl);
                            return true;
                        case Keys.F3:
                            this.TrySearchWordInTabToBottom();
                            return true;
                        case Keys.F5:
                            this.DoRefresh();
                            return true;
                        case Keys.F6:
                            this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                            return true;
                        case Keys.F7:
                            this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                            return true;
                    }

                    if (focusedControl != FocusedControl.StatusText)
                    {
                        // フォーカスStatusText以外
                        switch (keyCode)
                        {
                            case Keys.Space:
                            case Keys.ProcessKey:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    this.anchorFlag = false;
                                }

                                this.TrySearchAndFocusUnreadTweet();
                                return true;
                            case Keys.G:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    this.anchorFlag = false;
                                }

                                this.AddRelatedStatusesTab();
                                return true;
                        }
                    }

                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // フォーカスList
                        switch (keyCode)
                        {
                            case Keys.N:
                            case Keys.Right:
                                this.GoRelPost(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                this.GoRelPost(false);
                                return true;
                            case Keys.OemPeriod:
                                this.GoAnchor();
                                return true;
                            case Keys.I:
                                if (this.StatusText.Enabled)
                                {
                                    this.StatusText.Focus();
                                }

                                return true;
                            case Keys.Enter:
                                // case Keys.Return:
                                this.MakeReplyOrDirectStatus();
                                return true;
                            case Keys.R:
                                this.DoRefresh();
                                return true;
                        }

                        // 以下、アンカー初期化
                        this.anchorFlag = false;
                        switch (keyCode)
                        {
                            case Keys.L:
                                this.GoPost(true);
                                return true;
                            case Keys.H:
                                this.GoPost(false);
                                return true;
                            case Keys.Z:
                            case Keys.Oemcomma:
                                this.MoveTop();
                                return true;
                            case Keys.S:
                                this.GoNextTab(true);
                                return true;
                            case Keys.A:
                                this.GoNextTab(false);
                                return true;
                            case Keys.Oem4:
                                // ] in_reply_to参照元へ戻る
                                this.GoInReplyToPostTree();
                                return true;
                            case Keys.Oem6:
                                // [ in_reply_toへジャンプ
                                this.GoBackInReplyToPostTree();
                                return true;
                            case Keys.Escape:
                                if (this.ListTab.SelectedTab != null)
                                {
                                    TabUsageType tabtype = this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == TabUsageType.Related || tabtype == TabUsageType.UserTimeline || tabtype == TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = this.ListTab.SelectedTab;
                                        this.RemoveSpecifiedTab(relTp.Text, false);
                                        this.SaveConfigsTabs();
                                        return true;
                                    }
                                }

                                break;
                        }
                    }

                    break;
                case ModifierState.Ctrl:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.MakeReplyOrDirectStatus(false, true);
                            return true;
                        case Keys.D:
                            this.DoStatusDelete();
                            return true;
                        case Keys.M:
                            this.MakeReplyOrDirectStatus(false, false);
                            return true;
                        case Keys.S:
                            this.FavoriteChange(true);
                            return true;
                        case Keys.I:
                            this.DoRepliedStatusOpen();
                            return true;
                        case Keys.Q:
                            this.DoQuote();
                            return true;
                        case Keys.B:
                            this.ChangeSelectetdTweetReadStateToRead();
                            return true;
                        case Keys.T:
                            this.ShowHashManageBox();
                            return true;
                        case Keys.L:
                            this.ConvertUrlByAutoSelectedService();
                            return true;
                        case Keys.Y:
                            if (focusedControl != FocusedControl.PostBrowser)
                            {
                                this.ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
                                return true;
                            }

                            break;
                        case Keys.F:
                            this.TrySearchWordInTab();
                            return true;
                        case Keys.U:
                            this.ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            this.TryOpenCurListSelectedUserHome();
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            this.TryOpenCurListSelectedUserFavorites();
                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            this.TryOpenSelectedTweetWebPage();
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            this.TryOpenUrlInCurrentTweet();
                            return true;
                    }

                    // フォーカスList
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.Home:
                            case Keys.End:
                                this.colorize = true; // スルーする
                                return false;
                            case Keys.N:
                                this.GoNextTab(true);
                                return true;
                            case Keys.P:
                                this.GoNextTab(false);
                                return true;
                            case Keys.C:
                                this.CopyStot();
                                return true;
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                // タブダイレクト選択(Ctrl+1～8,Ctrl+9)
                                int tabNo = keyCode - Keys.D1;
                                if (this.ListTab.TabPages.Count < tabNo)
                                {
                                    return functionReturnValue;
                                }

                                this.ListTab.SelectedIndex = tabNo;
                                this.ListTabSelect(this.ListTab.TabPages[tabNo]);
                                return true;
                            case Keys.D9:
                                this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                                this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                                return true;
                        }
                    }
                    else if (focusedControl == FocusedControl.StatusText)
                    {
                        // フォーカスStatusText
                        switch (keyCode)
                        {
                            case Keys.A:
                                this.StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!string.IsNullOrEmpty(this.StatusText.Text.Trim()))
                                {
                                    this.postHistory[this.postHistoryIndex] = new PostingStatus(this.StatusText.Text, this.replyToId, this.replyToName);
                                }

                                if (keyCode == Keys.Up)
                                {
                                    this.postHistoryIndex -= 1;
                                    if (this.postHistoryIndex < 0)
                                    {
                                        this.postHistoryIndex = 0;
                                    }
                                }
                                else
                                {
                                    this.postHistoryIndex += 1;
                                    if (this.postHistoryIndex > this.postHistory.Count - 1)
                                    {
                                        this.postHistoryIndex = this.postHistory.Count - 1;
                                    }
                                }

                                this.StatusText.Text = this.postHistory[this.postHistoryIndex].Status;
                                this.replyToId = this.postHistory[this.postHistoryIndex].InReplyToId;
                                this.replyToName = this.postHistory[this.postHistoryIndex].InReplyToName;
                                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                                return true;
                            case Keys.PageUp:
                            case Keys.P:
                                if (this.ListTab.SelectedIndex == 0)
                                {
                                    this.ListTab.SelectedIndex = this.ListTab.TabCount - 1;
                                }
                                else
                                {
                                    this.ListTab.SelectedIndex -= 1;
                                }

                                this.StatusText.Focus();
                                return true;
                            case Keys.PageDown:
                            case Keys.N:
                                if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                                {
                                    this.ListTab.SelectedIndex = 0;
                                }
                                else
                                {
                                    this.ListTab.SelectedIndex += 1;
                                }

                                this.StatusText.Focus();
                                return true;
                        }
                    }
                    else
                    {
                        // フォーカスPostBrowserもしくは関係なし
                        switch (keyCode)
                        {
                            case Keys.A:
                                this.PostBrowser.Document.ExecCommand("SelectAll", false, null);
                                return true;
                            case Keys.C:
                            case Keys.Insert:
                                string selText = WebBrowser_GetSelectionText(ref this.PostBrowser);
                                if (!string.IsNullOrEmpty(selText))
                                {
                                    try
                                    {
                                        Clipboard.SetDataObject(selText, false, 5, 100);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }

                                return true;
                            case Keys.Y:
                                MultiLineMenuItem.Checked = !MultiLineMenuItem.Checked;
                                ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
                                return true;
                        }
                    }

                    break;
                case ModifierState.Shift:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F3:
                            this.TrySearchWordInTabToTop();
                            return true;
                        case Keys.F5:
                            this.DoRefreshMore();
                            return true;
                        case Keys.F6:
                            this.GetTimeline(WorkerType.Reply, -1, 0, string.Empty);
                            return true;
                        case Keys.F7:
                            this.GetTimeline(WorkerType.DirectMessegeRcv, -1, 0, string.Empty);
                            return true;
                    }

                    // フォーカスStatusText以外
                    if (focusedControl != FocusedControl.StatusText)
                    {
                        if (keyCode == Keys.R)
                        {
                            this.DoRefreshMore();
                            return true;
                        }
                    }

                    // フォーカスリスト
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.H:
                                this.GoTopEnd(true);
                                return true;
                            case Keys.L:
                                this.GoTopEnd(false);
                                return true;
                            case Keys.M:
                                this.GoMiddle();
                                return true;
                            case Keys.G:
                                this.GoLast();
                                return true;
                            case Keys.Z:
                                this.MoveMiddle();
                                return true;
                            case Keys.Oem4:
                                this.GoBackInReplyToPostTree(true, false);
                                return true;
                            case Keys.Oem6:
                                this.GoBackInReplyToPostTree(true, true);
                                return true;
                            case Keys.N:
                            case Keys.Right:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                this.GoFav(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                this.GoFav(false);
                                return true;
                            case Keys.Space:
                                this.GoBackSelectPostChain();
                                return true;
                        }
                    }

                    break;
                case ModifierState.Alt:
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.DoReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (this.curPost != null)
                            {
                                this.ShowUserStatus(this.curPost.ScreenName, false);
                                return true;
                            }

                            break;
                        case Keys.Up:
                            this.ScrollDownPostBrowser(false);
                            return true;
                        case Keys.Down:
                            this.ScrollDownPostBrowser(true);
                            return true;
                        case Keys.PageUp:
                            this.PageDownPostBrowser(false);
                            return true;
                        case Keys.PageDown:
                            this.PageDownPostBrowser(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // 別タブの同じ書き込みへ(ALT+←/→)
                        if (keyCode == Keys.Right)
                        {
                            this.GoSamePostToAnotherTab(false);
                            return true;
                        }

                        if (keyCode == Keys.Left)
                        {
                            this.GoSamePostToAnotherTab(true);
                            return true;
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Shift:
                    switch (keyCode)
                    {
                        case Keys.R:
                            this.MakeReplyOrDirectStatus(false, true, true);
                            return true;
                        case Keys.C:
                            this.CopyIdUri();
                            return true;
                        case Keys.F:
                            if (this.ListTab.SelectedTab != null)
                            {
                                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                                {
                                    this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                                    return true;
                                }
                            }

                            break;
                        case Keys.S:
                            this.FavoriteChange(false);
                            return true;
                        case Keys.B:
                            this.ChangeSelectedTweetReadSateToUnread();
                            return true;
                        case Keys.T:
                            this.ChangeUseHashTagSetting();
                            return true;
                        case Keys.P:
                            this.ToggleImageSelectorView();
                            return true;
                        case Keys.H:
                            this.TryOpenSelectedRtUserHome();
                            return true;
                        case Keys.O:
                            this.OpenFavorarePageOfSelectedTweetUser();
                            return true;
                    }

                    if (focusedControl == FocusedControl.StatusText)
                    {
                        switch (keyCode)
                        {
                            case Keys.Up:
                                {
                                    int idx = 0;
                                    if (this.curList != null && this.curList.Items.Count != 0 && this.curList.SelectedIndices.Count > 0 && this.curList.SelectedIndices[0] > 0)
                                    {
                                        idx = this.curList.SelectedIndices[0] - 1;
                                        this.SelectListItem(this.curList, idx);
                                        this.curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Down:
                                {
                                    int idx = 0;
                                    if (this.curList != null && this.curList.Items.Count != 0 && this.curList.SelectedIndices.Count > 0 && this.curList.SelectedIndices[0] < this.curList.Items.Count - 1)
                                    {
                                        idx = this.curList.SelectedIndices[0] + 1;
                                        this.SelectListItem(this.curList, idx);
                                        this.curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }

                                break;
                            case Keys.Space:
                                if (this.StatusText.SelectionStart > 0)
                                {
                                    int endidx = this.StatusText.SelectionStart - 1;
                                    string startstr = string.Empty;
                                    bool pressed = false;
                                    for (int i = this.StatusText.SelectionStart - 1; i >= 0; i--)
                                    {
                                        char c = this.StatusText.Text[i];
                                        if (char.IsLetterOrDigit(c) || c == '_')
                                        {
                                            continue;
                                        }

                                        if (c == '@')
                                        {
                                            pressed = true;
                                            startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                            int cnt = this.AtIdSupl.ItemCount;
                                            this.ShowSuplDialog(this.StatusText, this.AtIdSupl, startstr.Length + 1, startstr);
                                            if (this.AtIdSupl.ItemCount != cnt)
                                            {
                                                this.modifySettingAtId = true;
                                            }
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                            this.ShowSuplDialog(this.StatusText, this.HashSupl, startstr.Length + 1, startstr);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    return pressed;
                                }

                                break;
                        }
                    }
                    else if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                {
                                    // ソートダイレクト選択(Ctrl+Shift+1～8,Ctrl+Shift+9)
                                    int colNo = keyCode - Keys.D1;
                                    DetailsListView lst = (DetailsListView)this.ListTab.SelectedTab.Tag;
                                    if (lst.Columns.Count < colNo)
                                    {
                                        return functionReturnValue;
                                    }

                                    var col = lst.Columns.Cast<ColumnHeader>().Where(x => x.DisplayIndex == colNo).FirstOrDefault();
                                    if (col == null)
                                    {
                                        return functionReturnValue;
                                    }

                                    MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                    return true;
                                }

                            case Keys.D9:
                                {
                                    DetailsListView lst = (DetailsListView)this.ListTab.SelectedTab.Tag;
                                    var col = lst.Columns.Cast<ColumnHeader>().OrderByDescending(x => x.DisplayIndex).First();
                                    MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                    return true;
                                }
                        }
                    }

                    break;
                case ModifierState.Ctrl | ModifierState.Alt:
                    if (keyCode == Keys.S)
                    {
                        this.FavoritesRetweetOriginal();
                        return true;
                    }
                    else if (keyCode == Keys.R)
                    {
                        this.FavoritesRetweetUnofficial();
                        return true;
                    }
                    else if (keyCode == Keys.H)
                    {
                        OpenUserAppointUrl();
                        return true;
                    }

                    break;
                case ModifierState.Alt | ModifierState.Shift:
                    if (focusedControl == FocusedControl.PostBrowser)
                    {
                        if (keyCode == Keys.R)
                        {
                            this.DoReTweetUnofficial();
                        }
                        else if (keyCode == Keys.C)
                        {
                            this.CopyUserId();
                        }

                        return true;
                    }

                    switch (keyCode)
                    {
                        case Keys.T:
                            if (!this.ExistCurrentPost)
                            {
                                return functionReturnValue;
                            }

                            this.DoTranslation(this.curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            this.DoReTweetUnofficial();
                            return true;
                        case Keys.C:
                            this.CopyUserId();
                            return true;
                        case Keys.Up:
                            this.thumbnail.ScrollThumbnail(false);
                            return true;
                        case Keys.Down:
                            this.thumbnail.ScrollThumbnail(true);
                            return true;
                    }

                    if (focusedControl == FocusedControl.ListTab && keyCode == Keys.Enter)
                    {
                        if (!this.SplitContainer3.Panel2Collapsed)
                        {
                            this.thumbnail.OpenPicture();
                        }

                        return true;
                    }

                    break;
            }

            return functionReturnValue;
        }

        private void ScrollDownPostBrowser(bool forward)
        {
            HtmlDocument doc = this.PostBrowser.Document;
            if (doc == null)
            {
                return;
            }

            if (doc.Body == null)
            {
                return;
            }

            if (forward)
            {
                doc.Body.ScrollTop += this.settingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= this.settingDialog.FontDetail.Height;
            }
        }

        private void PageDownPostBrowser(bool forward)
        {
            HtmlDocument doc = this.PostBrowser.Document;
            if (doc == null)
            {
                return;
            }

            if (doc.Body == null)
            {
                return;
            }

            if (forward)
            {
                doc.Body.ScrollTop += this.PostBrowser.ClientRectangle.Height - this.settingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= this.PostBrowser.ClientRectangle.Height - this.settingDialog.FontDetail.Height;
            }
        }

        private void GoNextTab(bool forward)
        {
            int idx = this.ListTab.SelectedIndex;
            if (forward)
            {
                idx += 1;
                if (idx > this.ListTab.TabPages.Count - 1)
                {
                    idx = 0;
                }
            }
            else
            {
                idx -= 1;
                if (idx < 0)
                {
                    idx = this.ListTab.TabPages.Count - 1;
                }
            }

            this.ListTab.SelectedIndex = idx;
            this.ListTabSelect(this.ListTab.TabPages[idx]);
        }

        private void CopyStot()
        {
            string clstr = string.Empty;
            StringBuilder sb = new StringBuilder();
            bool isProtected = false;
            bool isDm = false;
            if (this.curTab != null && this.statuses.GetTabByName(this.curTab.Text) != null)
            {
                isDm = this.statuses.GetTabByName(this.curTab.Text).TabType == TabUsageType.DirectMessage;
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                if (post.IsProtect)
                {
                    isProtected = true;
                    continue;
                }

                if (post.IsDeleted)
                {
                    continue;
                }

                if (!isDm)
                {
                    sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.OriginalStatusId, Environment.NewLine);                 
                }
                else
                {
                    sb.AppendFormat("{0}:{1} [{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine);
                }
            }

            if (isProtected)
            {
                // MessageBox.Show(My.Resources.CopyStotText1)
                MessageForm w = new MessageForm();
                w.ShowDialog(Hoehoe.Properties.Resources.CopyStotText1);
            }

            if (sb.Length > 0)
            {
                clstr = sb.ToString();
                try
                {
                    Clipboard.SetDataObject(clstr, false, 5, 100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void CopyIdUri()
        {
            string clstr = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (this.curTab == null)
            {
                return;
            }

            if (this.statuses.GetTabByName(this.curTab.Text) == null)
            {
                return;
            }

            if (this.statuses.GetTabByName(this.curTab.Text).TabType == TabUsageType.DirectMessage)
            {
                return;
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.OriginalStatusId, Environment.NewLine);
            }

            if (sb.Length > 0)
            {
                clstr = sb.ToString();
                try
                {
                    Clipboard.SetDataObject(clstr, false, 5, 100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void GoFav(bool forward)
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                if (this.curList.SelectedIndices.Count == 0)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = this.curList.SelectedIndices[0] + 1;
                    if (fIdx > this.curList.VirtualListSize - 1)
                    {
                        return;
                    }
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                if (this.curList.SelectedIndices.Count == 0)
                {
                    fIdx = this.curList.VirtualListSize - 1;
                }
                else
                {
                    fIdx = this.curList.SelectedIndices[0] - 1;
                    if (fIdx < 0)
                    {
                        return;
                    }
                }

                toIdx = 0;
                stp = -1;
            }

            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                if (this.statuses.Item(this.curTab.Text, idx).IsFav)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            long targetId = 0;

            if (this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                // Directタブは対象外（見つかるはずがない）
                return;
            }

            if (this.curList.SelectedIndices.Count == 0)
            {
                // 未選択も処理しない
                return;
            }

            targetId = this.GetCurTabPost(this.curList.SelectedIndices[0]).StatusId;
            if (left)
            {
                // 左のタブへ
                if (this.ListTab.SelectedIndex == 0)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex - 1;
                }

                toIdx = 0;
                stp = -1;
            }
            else
            {
                // 右のタブへ
                if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex + 1;
                }

                toIdx = this.ListTab.TabCount - 1;
                stp = 1;
            }

            bool found = false;
            for (int tabidx = fIdx; tabidx <= toIdx; tabidx += stp)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage)
                {
                    // Directタブは対象外
                    continue;
                }

                for (int idx = 0; idx <= ((DetailsListView)this.ListTab.TabPages[tabidx].Tag).VirtualListSize - 1; idx++)
                {
                    if (this.statuses.Item(this.ListTab.TabPages[tabidx].Text, idx).StatusId == targetId)
                    {
                        this.ListTab.SelectedIndex = tabidx;
                        this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                        this.SelectListItem(this.curList, idx);
                        this.curList.EnsureVisible(idx);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }

        private void GoPost(bool forward)
        {
            if (this.curList.SelectedIndices.Count == 0 || this.curPost == null)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                fIdx = this.curList.SelectedIndices[0] + 1;
                if (fIdx > this.curList.VirtualListSize - 1)
                {
                    return;
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this.curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }

                toIdx = 0;
                stp = -1;
            }

            string name = this.curPost.IsRetweeted ? this.curPost.RetweetedBy : this.curPost.ScreenName;
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                var statusesItem = this.statuses.Item(this.curTab.Text, idx);
                var statusItemName = statusesItem.IsRetweeted ? statusesItem.RetweetedBy : statusesItem.ScreenName;
                if (statusItemName == name)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            if (forward)
            {
                fIdx = this.curList.SelectedIndices[0] + 1;
                if (fIdx > this.curList.VirtualListSize - 1)
                {
                    return;
                }

                toIdx = this.curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this.curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }

                toIdx = 0;
                stp = -1;
            }

            if (!this.anchorFlag)
            {
                if (this.curPost == null)
                {
                    return;
                }

                this.anchorPost = this.curPost;
                this.anchorFlag = true;
            }
            else
            {
                if (this.anchorPost == null)
                {
                    return;
                }
            }

            // TODO: VB's for-next to C#'s for
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                if (post.ScreenName == this.anchorPost.ScreenName 
                    || post.RetweetedBy == this.anchorPost.ScreenName 
                    || post.ScreenName == this.anchorPost.RetweetedBy 
                    || (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == this.anchorPost.RetweetedBy) 
                    || this.anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) 
                    || this.anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) 
                    || post.ReplyToList.Contains(this.anchorPost.ScreenName.ToLower()) 
                    || post.ReplyToList.Contains(this.anchorPost.RetweetedBy.ToLower()))
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            if (this.anchorPost == null)
            {
                return;
            }

            int idx = this.statuses.Tabs[this.curTab.Text].IndexOf(this.anchorPost.StatusId);
            if (idx == -1)
            {
                return;
            }

            this.SelectListItem(this.curList, idx);
            this.curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool top)
        {
            ListViewItem item = null;
            int idx = 0;

            if (top)
            {
                item = this.curList.GetItemAt(0, 25);
                idx = item != null ? item.Index : 0;
            }
            else
            {
                item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
                idx = item != null ? item.Index : this.curList.VirtualListSize - 1;
            }

            this.SelectListItem(this.curList, idx);
        }

        private void GoMiddle()
        {
            ListViewItem item = this.curList.GetItemAt(0, 0);
            int idx1 = item == null ? 0 : item.Index;

            item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
            int idx2 = item == null ? this.curList.VirtualListSize - 1 : item.Index;

            this.SelectListItem(this.curList, (idx1 + idx2) / 2);
        }

        private void GoLast()
        {
            if (this.curList.VirtualListSize == 0)
            {
                return;
            }

            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                this.SelectListItem(this.curList, this.curList.VirtualListSize - 1);
                this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            }
            else
            {
                this.SelectListItem(this.curList, 0);
                this.curList.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = this.curList.SelectedIndices[0];
            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            }
            else
            {
                this.curList.EnsureVisible(0);
            }

            this.curList.EnsureVisible(idx);
        }

        private void GoInReplyToPostTree()
        {
            if (this.curPost == null)
            {
                return;
            }

            TabClass curTabClass = this.statuses.Tabs[this.curTab.Text];

            if (curTabClass.TabType == TabUsageType.PublicSearch && this.curPost.InReplyToStatusId == 0 && this.curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = this.tw.GetStatusApi(false, this.curPost.StatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    this.curPost.InReplyToStatusId = post.InReplyToStatusId;
                    this.curPost.InReplyToUser = post.InReplyToUser;
                    this.curPost.IsReply = post.IsReply;
                    this.itemCache = null;
                    this.curList.RedrawItems(this.curItemIndex, this.curItemIndex, false);
                }
                else
                {
                    this.StatusLabel.Text = r;
                }
            }

            if (!(this.ExistCurrentPost && this.curPost.InReplyToUser != null && this.curPost.InReplyToStatusId > 0))
            {
                return;
            }

            if (this.replyChains == null || (this.replyChains.Count > 0 && this.replyChains.Peek().InReplyToId != this.curPost.StatusId))
            {
                this.replyChains = new Stack<ReplyChain>();
            }

            this.replyChains.Push(new ReplyChain(this.curPost.StatusId, this.curPost.InReplyToStatusId, this.curTab));

            int inReplyToIndex = 0;
            string inReplyToTabName = null;
            long inReplyToId = this.curPost.InReplyToStatusId;
            string inReplyToUser = this.curPost.InReplyToUser;
            Dictionary<long, PostClass> curTabPosts = null;

            if (this.statuses.Tabs[this.curTab.Text].IsInnerStorageTabType)
            {
                curTabPosts = curTabClass.Posts;
            }
            else
            {
                curTabPosts = this.statuses.Posts;
            }

            var inReplyToPosts = from tab in this.statuses.Tabs.Values
                                 orderby !object.ReferenceEquals(tab, curTabClass)
                                 from post in ((Dictionary<long, PostClass>)(tab.IsInnerStorageTabType ? tab.Posts : this.statuses.Posts)).Values
                                 where post.StatusId == inReplyToId
                                 let index = tab.IndexOf(post.StatusId)
                                 where index != -1
                                 select new { Tab = tab, Index = index };

            try
            {
                var inReplyPost = inReplyToPosts.First();
                inReplyToTabName = inReplyPost.Tab.TabName;
                inReplyToIndex = inReplyPost.Index;
            }
            catch (InvalidOperationException)
            {
                PostClass post = null;
                string r = this.tw.GetStatusApi(false, this.curPost.InReplyToStatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    post.IsRead = true;
                    this.statuses.AddPost(post);
                    this.statuses.DistributePosts();
                    this.RefreshTimeline(false);
                    try
                    {
                        var inReplyPost = inReplyToPosts.First();
                        inReplyToTabName = inReplyPost.Tab.TabName;
                        inReplyToIndex = inReplyPost.Index;
                    }
                    catch (InvalidOperationException)
                    {
                        this.OpenUriAsync(string.Format("http://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                        return;
                    }
                }
                else
                {
                    this.StatusLabel.Text = r;
                    this.OpenUriAsync(string.Format("http://twitter.com/{0}/statuses/{1}", inReplyToUser, inReplyToId));
                    return;
                }
            }

            var tabPage = this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == inReplyToTabName);
            var listView = (DetailsListView)tabPage.Tag;
            if (!object.ReferenceEquals(this.curTab, tabPage))
            {
                this.ListTab.SelectTab(tabPage);
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (this.curPost == null)
            {
                return;
            }

            TabClass curTabClass = this.statuses.Tabs[this.curTab.Text];
            Dictionary<long, PostClass> curTabPosts = (Dictionary<long, PostClass>)(curTabClass.IsInnerStorageTabType ? curTabClass.Posts : this.statuses.Posts);

            if (parallel)
            {
                if (this.curPost.InReplyToStatusId != 0)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this.statuses.Posts)
                                where p.Value.StatusId != this.curPost.StatusId && p.Value.InReplyToStatusId == this.curPost.InReplyToStatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby (isForward ? indexOf : indexOf * -1)
                                orderby !object.ReferenceEquals(t.Value, curTabClass)
                                select new { Tab = t.Value, Post = p.Value, Index = indexOf };
                    try
                    {
                        var postList = posts.ToList();
                        for (int i = postList.Count - 1; i >= 0; i--)
                        {
                            int index = i;
                            if (postList.FindIndex(pst => pst.Post.StatusId == postList[index].Post.StatusId) != index)
                            {
                                postList.RemoveAt(index);
                            }
                        }

                        var post = postList.FirstOrDefault(pst => object.ReferenceEquals(pst.Tab, curTabClass) && (isForward ? pst.Index > this.curItemIndex : pst.Index < this.curItemIndex));
                        if (post == null)
                        {
                            post = postList.FirstOrDefault(pst => !object.ReferenceEquals(pst.Tab, curTabClass));
                        }

                        if (post == null)
                        {
                            post = postList.First();
                        }

                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                        var listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (this.replyChains == null || this.replyChains.Count < 1)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this.statuses.Posts)
                                where p.Value.InReplyToStatusId == this.curPost.StatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby indexOf
                                orderby !object.ReferenceEquals(t.Value, curTabClass)
                                select new { Tab = t.Value, Index = indexOf };
                    try
                    {
                        var post = posts.First();
                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == post.Tab.TabName));
                        var listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                else
                {
                    ReplyChain chainHead = this.replyChains.Pop();
                    if (chainHead.InReplyToId == this.curPost.StatusId)
                    {
                        int idx = this.statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                        if (idx == -1)
                        {
                            this.replyChains = null;
                        }
                        else
                        {
                            try
                            {
                                this.ListTab.SelectTab(chainHead.OriginalTab);
                            }
                            catch (Exception)
                            {
                                this.replyChains = null;
                            }

                            this.SelectListItem(this.curList, idx);
                            this.curList.EnsureVisible(idx);
                        }
                    }
                    else
                    {
                        this.replyChains = null;
                        this.GoBackInReplyToPostTree(parallel);
                    }
                }
            }
        }

        private void GoBackSelectPostChain()
        {
            try
            {
                this.selectPostChains.Pop();
                var tabPostPair = this.selectPostChains.Pop();
                if (!this.ListTab.TabPages.Contains(tabPostPair.Item1))
                {
                    return;
                }

                this.ListTab.SelectedTab = tabPostPair.Item1;
                if (tabPostPair.Item2 != null && this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId) > -1)
                {
                    this.SelectListItem(this.curList, this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                    this.curList.EnsureVisible(this.statuses.Tabs[this.curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void PushSelectPostChain()
        {
            if (this.selectPostChains.Count == 0 || (this.selectPostChains.Peek().Item1.Text != this.curTab.Text || !object.ReferenceEquals(this.curPost, this.selectPostChains.Peek().Item2)))
            {
                this.selectPostChains.Push(Tuple.Create(this.curTab, this.curPost));
            }
        }

        private void TrimPostChain()
        {
            if (this.selectPostChains.Count < 2000)
            {
                return;
            }

            Stack<Tuple<TabPage, PostClass>> p = new Stack<Tuple<TabPage, PostClass>>();
            for (var i = 0; i < 2000; i++)
            {
                p.Push(this.selectPostChains.Pop());
            }

            this.selectPostChains.Clear();
            for (var i = 0; i < 2000; i++)
            {
                this.selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }

            for (int tabidx = 0; tabidx <= this.ListTab.TabCount - 1; tabidx++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType != TabUsageType.DirectMessage && this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    this.ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    return true;
                }
            }

            return false;
        }

        private bool GoDirectMessage(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }

            for (int tabidx = 0; tabidx < this.ListTab.TabCount; tabidx++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage && this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this.statuses.Tabs[this.ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    this.ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(this.ListTab.TabPages[tabidx]);
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                    return true;
                }
            }

            return false;
        }

        private void SaveConfigsAll(bool ifModified)
        {
            if (!ifModified)
            {
                this.SaveConfigsCommon();
                this.SaveConfigsLocal();
                this.SaveConfigsTabs();
                this.SaveConfigsAtId();
            }
            else
            {
                if (this.modifySettingCommon)
                {
                    this.SaveConfigsCommon();
                }

                if (this.modifySettingLocal)
                {
                    this.SaveConfigsLocal();
                }

                if (this.modifySettingAtId)
                {
                    this.SaveConfigsAtId();
                }
            }
        }

        private void SaveConfigsAtId()
        {
            if (this.ignoreConfigSave || (!this.settingDialog.UseAtIdSupplement && this.AtIdSupl == null))
            {
                return;
            }

            this.modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(this.AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (this.ignoreConfigSave)
            {
                return;
            }

            this.modifySettingCommon = false;
            lock (this.syncObject)
            {
                this.cfgCommon.UserName = this.tw.Username;
                this.cfgCommon.UserId = this.tw.UserId;
                this.cfgCommon.Password = this.tw.Password;
                this.cfgCommon.Token = this.tw.AccessToken;
                this.cfgCommon.TokenSecret = this.tw.AccessTokenSecret;
                this.cfgCommon.UserAccounts = this.settingDialog.UserAccounts;
                this.cfgCommon.UserstreamStartup = this.settingDialog.UserstreamStartup;
                this.cfgCommon.UserstreamPeriod = this.settingDialog.UserstreamPeriodInt;
                this.cfgCommon.TimelinePeriod = this.settingDialog.TimelinePeriodInt;
                this.cfgCommon.ReplyPeriod = this.settingDialog.ReplyPeriodInt;
                this.cfgCommon.DMPeriod = this.settingDialog.DMPeriodInt;
                this.cfgCommon.PubSearchPeriod = this.settingDialog.PubSearchPeriodInt;
                this.cfgCommon.ListsPeriod = this.settingDialog.ListsPeriodInt;
                this.cfgCommon.UserTimelinePeriod = this.settingDialog.UserTimelinePeriodInt;
                this.cfgCommon.Read = this.settingDialog.Readed;
                this.cfgCommon.IconSize = this.settingDialog.IconSz;
                this.cfgCommon.UnreadManage = this.settingDialog.UnreadManage;
                this.cfgCommon.PlaySound = this.settingDialog.PlaySound;
                this.cfgCommon.OneWayLove = this.settingDialog.OneWayLove;
                this.cfgCommon.NameBalloon = this.settingDialog.NameBalloon;
                this.cfgCommon.PostCtrlEnter = this.settingDialog.PostCtrlEnter;
                this.cfgCommon.PostShiftEnter = this.settingDialog.PostShiftEnter;
                this.cfgCommon.CountApi = this.settingDialog.CountApi;
                this.cfgCommon.CountApiReply = this.settingDialog.CountApiReply;
                this.cfgCommon.PostAndGet = this.settingDialog.PostAndGet;
                this.cfgCommon.DispUsername = this.settingDialog.DispUsername;
                this.cfgCommon.MinimizeToTray = this.settingDialog.MinimizeToTray;
                this.cfgCommon.CloseToExit = this.settingDialog.CloseToExit;
                this.cfgCommon.DispLatestPost = this.settingDialog.DispLatestPost;
                this.cfgCommon.SortOrderLock = this.settingDialog.SortOrderLock;
                this.cfgCommon.TinyUrlResolve = this.settingDialog.TinyUrlResolve;
                this.cfgCommon.ShortUrlForceResolve = this.settingDialog.ShortUrlForceResolve;
                this.cfgCommon.PeriodAdjust = this.settingDialog.PeriodAdjust;
                this.cfgCommon.StartupVersion = this.settingDialog.StartupVersion;
                this.cfgCommon.StartupFollowers = this.settingDialog.StartupFollowers;
                this.cfgCommon.RestrictFavCheck = this.settingDialog.RestrictFavCheck;
                this.cfgCommon.AlwaysTop = this.settingDialog.AlwaysTop;
                this.cfgCommon.UrlConvertAuto = this.settingDialog.UrlConvertAuto;
                this.cfgCommon.Outputz = this.settingDialog.OutputzEnabled;
                this.cfgCommon.OutputzKey = this.settingDialog.OutputzKey;
                this.cfgCommon.OutputzUrlMode = this.settingDialog.OutputzUrlmode;
                this.cfgCommon.UseUnreadStyle = this.settingDialog.UseUnreadStyle;
                this.cfgCommon.DateTimeFormat = this.settingDialog.DateTimeFormat;
                this.cfgCommon.DefaultTimeOut = this.settingDialog.DefaultTimeOut;
                this.cfgCommon.RetweetNoConfirm = this.settingDialog.RetweetNoConfirm;
                this.cfgCommon.LimitBalloon = this.settingDialog.LimitBalloon;
                this.cfgCommon.EventNotifyEnabled = this.settingDialog.EventNotifyEnabled;
                this.cfgCommon.EventNotifyFlag = this.settingDialog.EventNotifyFlag;
                this.cfgCommon.IsMyEventNotifyFlag = this.settingDialog.IsMyEventNotifyFlag;
                this.cfgCommon.ForceEventNotify = this.settingDialog.ForceEventNotify;
                this.cfgCommon.FavEventUnread = this.settingDialog.FavEventUnread;
                this.cfgCommon.TranslateLanguage = this.settingDialog.TranslateLanguage;
                this.cfgCommon.EventSoundFile = this.settingDialog.EventSoundFile;
                this.cfgCommon.AutoShortUrlFirst = this.settingDialog.AutoShortUrlFirst;
                this.cfgCommon.TabIconDisp = this.settingDialog.TabIconDisp;
                this.cfgCommon.ReplyIconState = this.settingDialog.ReplyIconState;
                this.cfgCommon.ReadOwnPost = this.settingDialog.ReadOwnPost;
                this.cfgCommon.GetFav = this.settingDialog.GetFav;
                this.cfgCommon.IsMonospace = this.settingDialog.IsMonospace;
                if (this.IdeographicSpaceToSpaceToolStripMenuItem != null && this.IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed == false)
                {
                    this.cfgCommon.WideSpaceConvert = this.IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }

                this.cfgCommon.ReadOldPosts = this.settingDialog.ReadOldPosts;
                this.cfgCommon.UseSsl = this.settingDialog.UseSsl;
                this.cfgCommon.BilyUser = this.settingDialog.BitlyUser;
                this.cfgCommon.BitlyPwd = this.settingDialog.BitlyPwd;
                this.cfgCommon.ShowGrid = this.settingDialog.ShowGrid;
                this.cfgCommon.UseAtIdSupplement = this.settingDialog.UseAtIdSupplement;
                this.cfgCommon.UseHashSupplement = this.settingDialog.UseHashSupplement;
                this.cfgCommon.PreviewEnable = this.settingDialog.PreviewEnable;
                this.cfgCommon.Language = this.settingDialog.Language;
                this.cfgCommon.SortOrder = (int)this.statuses.SortOrder;
                switch (this.statuses.SortMode)
                {
                    case IdComparerClass.ComparerMode.Nickname:
                        // ニックネーム
                        this.cfgCommon.SortColumn = 1;
                        break;
                    case IdComparerClass.ComparerMode.Data:
                        // 本文
                        this.cfgCommon.SortColumn = 2;
                        break;
                    case IdComparerClass.ComparerMode.Id:
                        // 時刻=発言Id
                        this.cfgCommon.SortColumn = 3;
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        // 名前
                        this.cfgCommon.SortColumn = 4;
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        // Source
                        this.cfgCommon.SortColumn = 7;
                        break;
                }

                this.cfgCommon.Nicoms = this.settingDialog.Nicoms;
                this.cfgCommon.HashTags = this.HashMgr.HashHistories;
                if (this.HashMgr.IsPermanent)
                {
                    this.cfgCommon.HashSelected = this.HashMgr.UseHash;
                }
                else
                {
                    this.cfgCommon.HashSelected = string.Empty;
                }

                this.cfgCommon.HashIsHead = this.HashMgr.IsHead;
                this.cfgCommon.HashIsPermanent = this.HashMgr.IsPermanent;
                this.cfgCommon.HashIsNotAddToAtReply = this.HashMgr.IsNotAddToAtReply;
                this.cfgCommon.TwitterUrl = this.settingDialog.TwitterApiUrl;
                this.cfgCommon.TwitterSearchUrl = this.settingDialog.TwitterSearchApiUrl;
                this.cfgCommon.HotkeyEnabled = this.settingDialog.HotkeyEnabled;
                this.cfgCommon.HotkeyModifier = this.settingDialog.HotkeyMod;
                this.cfgCommon.HotkeyKey = this.settingDialog.HotkeyKey;
                this.cfgCommon.HotkeyValue = this.settingDialog.HotkeyValue;
                this.cfgCommon.BlinkNewMentions = this.settingDialog.BlinkNewMentions;
                if (this.ToolStripFocusLockMenuItem != null && !this.ToolStripFocusLockMenuItem.IsDisposed)
                {
                    this.cfgCommon.FocusLockToStatusText = this.ToolStripFocusLockMenuItem.Checked;
                }

                this.cfgCommon.UseAdditionalCount = this.settingDialog.UseAdditionalCount;
                this.cfgCommon.MoreCountApi = this.settingDialog.MoreCountApi;
                this.cfgCommon.FirstCountApi = this.settingDialog.FirstCountApi;
                this.cfgCommon.SearchCountApi = this.settingDialog.SearchCountApi;
                this.cfgCommon.FavoritesCountApi = this.settingDialog.FavoritesCountApi;
                this.cfgCommon.UserTimelineCountApi = this.settingDialog.UserTimelineCountApi;
                this.cfgCommon.TrackWord = this.tw.TrackWord;
                this.cfgCommon.AllAtReply = this.tw.AllAtReply;
                this.cfgCommon.OpenUserTimeline = this.settingDialog.OpenUserTimeline;
                this.cfgCommon.ListCountApi = this.settingDialog.ListCountApi;
                this.cfgCommon.UseImageService = this.ImageServiceCombo.SelectedIndex;
                this.cfgCommon.ListDoubleClickAction = this.settingDialog.ListDoubleClickAction;
                this.cfgCommon.UserAppointUrl = this.settingDialog.UserAppointUrl;
                this.cfgCommon.HideDuplicatedRetweets = this.settingDialog.HideDuplicatedRetweets;
                this.cfgCommon.IsPreviewFoursquare = this.settingDialog.IsPreviewFoursquare;
                this.cfgCommon.FoursquarePreviewHeight = this.settingDialog.FoursquarePreviewHeight;
                this.cfgCommon.FoursquarePreviewWidth = this.settingDialog.FoursquarePreviewWidth;
                this.cfgCommon.FoursquarePreviewZoom = this.settingDialog.FoursquarePreviewZoom;
                this.cfgCommon.IsListsIncludeRts = this.settingDialog.IsListStatusesIncludeRts;
                this.cfgCommon.TabMouseLock = this.settingDialog.TabMouseLock;
                this.cfgCommon.IsRemoveSameEvent = this.settingDialog.IsRemoveSameEvent;
                this.cfgCommon.IsUseNotifyGrowl = this.settingDialog.IsNotifyUseGrowl;

                this.cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (this.ignoreConfigSave)
            {
                return;
            }

            lock (this.syncObject)
            {
                this.modifySettingLocal = false;
                this.cfgLocal.FormSize = this.mySize;
                this.cfgLocal.FormLocation = this.myLoc;
                this.cfgLocal.SplitterDistance = this.mySpDis;
                this.cfgLocal.PreviewDistance = this.mySpDis3;
                this.cfgLocal.StatusMultiline = this.StatusText.Multiline;
                this.cfgLocal.StatusTextHeight = this.mySpDis2;
                this.cfgLocal.AdSplitterDistance = this.myAdSpDis;
                this.cfgLocal.StatusText = this.settingDialog.Status;
                this.cfgLocal.FontUnread = this.fntUnread;
                this.cfgLocal.ColorUnread = this.clrUnread;
                this.cfgLocal.FontRead = this.fntReaded;
                this.cfgLocal.ColorRead = this.clrRead;
                this.cfgLocal.FontDetail = this.fntDetail;
                this.cfgLocal.ColorDetail = this.clrDetail;
                this.cfgLocal.ColorDetailBackcolor = this.clrDetailBackcolor;
                this.cfgLocal.ColorDetailLink = this.clrDetailLink;
                this.cfgLocal.ColorFav = this.clrFav;
                this.cfgLocal.ColorOWL = this.clrOWL;
                this.cfgLocal.ColorRetweet = this.clrRetweet;
                this.cfgLocal.ColorSelf = this.clrSelf;
                this.cfgLocal.ColorAtSelf = this.clrAtSelf;
                this.cfgLocal.ColorTarget = this.clrTarget;
                this.cfgLocal.ColorAtTarget = this.clrAtTarget;
                this.cfgLocal.ColorAtFromTarget = this.clrAtFromTarget;
                this.cfgLocal.ColorAtTo = this.clrAtTo;
                this.cfgLocal.ColorListBackcolor = this.clrListBackcolor;
                this.cfgLocal.ColorInputBackcolor = this.InputBackColor;
                this.cfgLocal.ColorInputFont = this.clrInputForecolor;
                this.cfgLocal.FontInputFont = this.fntInputFont;
                this.cfgLocal.BrowserPath = this.settingDialog.BrowserPath;
                this.cfgLocal.UseRecommendStatus = this.settingDialog.UseRecommendStatus;
                this.cfgLocal.ProxyType = this.settingDialog.SelectedProxyType;
                this.cfgLocal.ProxyAddress = this.settingDialog.ProxyAddress;
                this.cfgLocal.ProxyPort = this.settingDialog.ProxyPort;
                this.cfgLocal.ProxyUser = this.settingDialog.ProxyUser;
                this.cfgLocal.ProxyPassword = this.settingDialog.ProxyPassword;
                if (this.ignoreConfigSave)
                {
                    return;
                }

                this.cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            SettingTabs tabSetting = new SettingTabs();
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                if (this.statuses.Tabs[this.ListTab.TabPages[i].Text].TabType != TabUsageType.Related)
                {
                    tabSetting.Tabs.Add(this.statuses.Tabs[this.ListTab.TabPages[i].Text]);
                }
            }

            tabSetting.Save();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isAuto">True=先頭に挿入、False=カーソル位置に挿入</param>
        /// <param name="isReply">True=@,False=DM</param>
        /// <param name="isAll"></param>
        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            if (!this.StatusText.Enabled)
            {
                return;
            }

            if (this.curList == null)
            {
                return;
            }

            if (this.curTab == null)
            {
                return;
            }

            if (!this.ExistCurrentPost)
            {
                return;
            }

            // 複数あてリプライはReplyではなく通常ポスト
            if (this.curList.SelectedIndices.Count > 0)
            {
                // アイテムが1件以上選択されている
                if (this.curList.SelectedIndices.Count == 1 && !isAll && this.ExistCurrentPost)
                {
                    // 単独ユーザー宛リプライまたはDM
                    if ((this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                    {
                        // ダイレクトメッセージ
                        this.StatusText.Text = "D " + this.curPost.ScreenName + " " + this.StatusText.Text;
                        this.StatusText.SelectionStart = this.StatusText.Text.Length;
                        this.StatusText.Focus();
                        this.replyToId = 0;
                        this.replyToName = string.Empty;
                        return;
                    }

                    if (string.IsNullOrEmpty(this.StatusText.Text))
                    {
                        // 空の場合 : ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        this.StatusText.Text = "@" + this.curPost.ScreenName + " ";
                        this.replyToId = this.curPost.OriginalStatusId;
                        this.replyToName = this.curPost.ScreenName;
                    }
                    else
                    {
                        // 何か入力済の場合
                        if (isAuto)
                        {
                            // 1件選んでEnter or DoubleClick
                            if (this.StatusText.Text.Contains("@" + this.curPost.ScreenName + " "))
                            {
                                if (this.replyToId > 0 && this.replyToName == this.curPost.ScreenName)
                                {
                                    // 返信先書き換え
                                    this.replyToId = this.curPost.OriginalStatusId;
                                    this.replyToName = this.curPost.ScreenName;
                                }

                                return;
                            }

                            if (!this.StatusText.Text.StartsWith("@"))
                            {
                                // 文頭＠以外
                                if (this.StatusText.Text.StartsWith(". "))
                                {
                                    // 複数リプライ
                                    this.StatusText.Text = this.StatusText.Text.Insert(2, "@" + this.curPost.ScreenName + " ");
                                    this.replyToId = 0;
                                    this.replyToName = string.Empty;
                                }
                                else
                                {
                                    // 単独リプライ
                                    this.StatusText.Text = "@" + this.curPost.ScreenName + " " + this.StatusText.Text;
                                    this.replyToId = this.curPost.OriginalStatusId;
                                    this.replyToName = this.curPost.ScreenName;
                                }
                            }
                            else
                            {
                                // 文頭＠
                                // 複数リプライ
                                this.StatusText.Text = ". @" + this.curPost.ScreenName + " " + this.StatusText.Text;
                                this.replyToId = 0;
                                this.replyToName = string.Empty;
                            }
                        }
                        else
                        {
                            // 1件選んでCtrl-Rの場合（返信先操作せず）
                            int sidx = this.StatusText.SelectionStart;
                            string id = "@" + this.curPost.ScreenName + " ";
                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    id = " " + id;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, id);
                            sidx += id.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    // 複数リプライ
                    if (!isAuto && !isReply)
                    {
                        return;
                    }

                    // C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r
                    if (isAuto)
                    {
                        // Enter or DoubleClick
                        string statusTxt = this.StatusText.Text;
                        if (!statusTxt.StartsWith(". "))
                        {
                            statusTxt = ". " + statusTxt;
                            this.replyToId = 0;
                            this.replyToName = string.Empty;
                        }

                        for (int cnt = 0; cnt <= this.curList.SelectedIndices.Count - 1; cnt++)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[cnt]);
                            if (!statusTxt.Contains("@" + post.ScreenName + " "))
                            {
                                statusTxt = statusTxt.Insert(2, "@" + post.ScreenName + " ");
                            }
                        }

                        this.StatusText.Text = statusTxt;
                    }
                    else
                    {
                        // C-S-r or C-r
                        if (this.curList.SelectedIndices.Count > 1)
                        {
                            // 複数ポスト選択
                            string ids = string.Empty;
                            int sidx = this.StatusText.SelectionStart;
                            for (int cnt = 0; cnt <= this.curList.SelectedIndices.Count - 1; cnt++)
                            {
                                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[cnt]);
                                if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.ScreenName + " ";
                                }

                                if (isAll)
                                {
                                    foreach (string nm in post.ReplyToList)
                                    {
                                        if (!ids.Contains("@" + nm + " ") && !nm.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                            if (m.Success)
                                            {
                                                ids += "@" + m.Result("${id}") + " ";
                                            }
                                            else
                                            {
                                                ids += "@" + nm + " ";
                                            }
                                        }
                                    }
                                }
                            }

                            if (ids.Length == 0)
                            {
                                return;
                            }

                            if (!this.StatusText.Text.StartsWith(". "))
                            {
                                this.StatusText.Text = ". " + this.StatusText.Text;
                                sidx += 2;
                                this.replyToId = 0;
                                this.replyToName = string.Empty;
                            }

                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                        else
                        {
                            // 1件のみ選択のC-S-r（返信元付加する可能性あり）
                            string ids = string.Empty;
                            int sidx = this.StatusText.SelectionStart;
                            PostClass post = this.curPost;
                            if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ids += "@" + post.ScreenName + " ";
                            }

                            foreach (string nm in post.ReplyToList)
                            {
                                if (!ids.Contains("@" + nm + " ") && !nm.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                    if (m.Success)
                                    {
                                        ids += "@" + m.Result("${id}") + " ";
                                    }
                                    else
                                    {
                                        ids += "@" + nm + " ";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(post.RetweetedBy))
                            {
                                if (!ids.Contains("@" + post.RetweetedBy + " ") && !post.RetweetedBy.Equals(this.tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.RetweetedBy + " ";
                                }
                            }

                            if (ids.Length == 0)
                            {
                                return;
                            }

                            if (string.IsNullOrEmpty(this.StatusText.Text))
                            {
                                // 未入力の場合のみ返信先付加
                                this.StatusText.Text = ids;
                                this.StatusText.SelectionStart = ids.Length;
                                this.StatusText.Focus();
                                this.replyToId = post.OriginalStatusId;
                                this.replyToName = post.ScreenName;
                                return;
                            }

                            if (sidx > 0)
                            {
                                if (this.StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }

                            this.StatusText.Text = this.StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            this.StatusText.SelectionStart = sidx;
                            this.StatusText.Focus();
                            return;
                        }
                    }
                }

                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                this.StatusText.Focus();
            }
        }

        private void RefreshTasktrayIcon(bool forceRefresh)
        {
            if (this.colorize)
            {
                this.Colorize();
            }

            if (!this.TimerRefreshIcon.Enabled)
            {
                return;
            }

            if (forceRefresh)
            {
                this.isIdle = false;
            }

            this.iconCnt += 1;
            this.blinkCnt += 1;

            bool busy = false;
            foreach (BackgroundWorker bw in this.bworkers)
            {
                if (bw != null && bw.IsBusy)
                {
                    busy = true;
                    break;
                }
            }

            if (this.iconCnt > 3)
            {
                this.iconCnt = 0;
            }

            if (this.blinkCnt > 10)
            {
                this.blinkCnt = 0;
                this.SaveConfigsAll(true);
            }

            if (busy)
            {
                this.NotifyIcon1.Icon = this.iconRefresh[this.iconCnt];
                this.isIdle = false;
                this.myStatusError = false;
                return;
            }

            TabClass tb = this.statuses.GetTabByType(TabUsageType.Mentions);
            if (this.settingDialog.ReplyIconState != ReplyIconState.None && tb != null && tb.UnreadCount > 0)
            {
                if (this.blinkCnt > 0)
                {
                    return;
                }

                this.doBlink = !this.doBlink;
                if (this.doBlink || this.settingDialog.ReplyIconState == ReplyIconState.StaticIcon)
                {
                    this.NotifyIcon1.Icon = this.replyIcon;
                }
                else
                {
                    this.NotifyIcon1.Icon = this.replyIconBlink;
                }

                this.isIdle = false;
                return;
            }

            if (this.isIdle)
            {
                return;
            }

            this.isIdle = true;

            // 優先度：エラー→オフライン→アイドル．エラーは更新アイコンでクリアされる
            if (this.myStatusError)
            {
                this.NotifyIcon1.Icon = this.iconAtRed;
                return;
            }

            if (MyCommon.IsNetworkAvailable())
            {
                this.NotifyIcon1.Icon = this.iconAt;
            }
            else
            {
                this.NotifyIcon1.Icon = this.iconAtSmoke;
            }
        }

        private void TabMenuControl(string tabName)
        {
            if (this.statuses.Tabs[tabName].TabType != TabUsageType.Mentions && this.statuses.IsDefaultTab(tabName))
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else if (this.statuses.Tabs[tabName].TabType == TabUsageType.Mentions)
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else
            {
                this.FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                this.DeleteTabMenuItem.Enabled = true;
                this.DeleteTbMenuItem.Enabled = true;
            }
        }

        private bool SelectTab(ref string tabName)
        {
            do
            {
                // 振り分け先タブ選択
                if (this.tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.settingDialog.AlwaysTop;
                    return false;
                }

                this.TopMost = this.settingDialog.AlwaysTop;
                tabName = this.tabDialog.SelectedTabName;
                this.ListTab.SelectedTab.Focus();

                // 新規タブを選択→タブ作成
                if (tabName == Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText1)
                {
                    using (InputTabName inputName = new InputTabName())
                    {
                        inputName.TabName = this.statuses.GetUniqueTabName();
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }

                        tabName = inputName.TabName;
                        inputName.Dispose();
                    }

                    this.TopMost = this.settingDialog.AlwaysTop;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        if (!this.statuses.AddTab(tabName, TabUsageType.UserDefined, null) || !this.AddNewTab(tabName, false, TabUsageType.UserDefined))
                        {
                            // もう一度タブ名入力
                            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // 既存タブを選択
                    return true;
                }
            }
            while (true);
        }

        private void MoveOrCopy(ref bool move, ref bool mark)
        {
            // 移動するか？
            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
            if (MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                move = false;
            }
            else
            {
                move = true;
            }

            if (!move)
            {
                // マークするか？
                tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText6, "\r\n");
                if (MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    mark = true;
                }
                else
                {
                    mark = false;
                }
            }
        }

        private void MoveMiddle()
        {
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = this.curList.SelectedIndices[0];

            ListViewItem item = this.curList.GetItemAt(0, 25);
            int idx1 = item == null ? 0 : item.Index;

            item = this.curList.GetItemAt(0, this.curList.ClientSize.Height - 1);
            int idx2 = item == null ? this.curList.VirtualListSize - 1 : item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0)
            {
                idx = 0;
            }

            this.curList.EnsureVisible(this.curList.VirtualListSize - 1);
            this.curList.EnsureVisible(idx);
        }

        private void ClearTab(string tabName, bool showWarning)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            if (showWarning)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine);
                if (MessageBox.Show(tmp, tabName + " " + Hoehoe.Properties.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            this.statuses.ClearTabIds(tabName);
            if (this.ListTab.SelectedTab.Text == tabName)
            {
                this.anchorPost = null;
                this.anchorFlag = false;
                this.itemCache = null;
                this.postCache = null;
                this.itemCacheIndex = -1;
                this.curItemIndex = -1;
                this.curPost = null;
            }

            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    tb.ImageIndex = -1;
                    ((DetailsListView)tb.Tag).VirtualListSize = 0;
                    break;
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
        }

        // メインウインドウタイトルの書き換え
        private void SetMainWindowTitle()
        {
            StringBuilder ttl = new StringBuilder(256);
            int ur = 0;
            int al = 0;
            if (this.settingDialog.DispLatestPost != DispTitleEnum.None
                && this.settingDialog.DispLatestPost != DispTitleEnum.Post
                && this.settingDialog.DispLatestPost != DispTitleEnum.Ver
                && this.settingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                foreach (string key in this.statuses.Tabs.Keys)
                {
                    ur += this.statuses.Tabs[key].UnreadCount;
                    al += this.statuses.Tabs[key].AllCount;
                }
            }

            if (this.settingDialog.DispUsername)
            {
                ttl.Append(this.tw.Username).Append(" - ");
            }

            ttl.Append("Hoehoe  ");
            switch (this.settingDialog.DispLatestPost)
            {
                case DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.FileVersion);
                    break;
                case DispTitleEnum.Post:
                    if (this.postHistory != null && this.postHistory.Count > 1)
                    {
                        ttl.Append(this.postHistory[this.postHistory.Count - 2].Status.Replace("\r\n", string.Empty));
                    }

                    break;
                case DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText1, this.statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this.statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText3, ur, this.statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this.statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case DispTitleEnum.OwnStatus:
                    if (this.prevFollowerCount == 0 && this.tw.FollowersCount > 0)
                    {
                        this.prevFollowerCount = this.tw.FollowersCount;
                    }

                    ttl.AppendFormat(Hoehoe.Properties.Resources.OwnStatusTitle, this.tw.StatusesCount, this.tw.FriendsCount, this.tw.FollowersCount, this.tw.FollowersCount - this.prevFollowerCount);
                    break;
            }

            try
            {
                this.Text = ttl.ToString();
            }
            catch (AccessViolationException ex)
            {
                // 原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private string GetStatusLabelText()
        {
            // ステータス欄にカウント表示
            // タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (this.statuses == null)
            {
                return string.Empty;
            }

            TabClass mentionTab = this.statuses.GetTabByType(TabUsageType.Mentions);
            TabClass dmessageTab = this.statuses.GetTabByType(TabUsageType.DirectMessage);
            if (mentionTab == null || dmessageTab == null)
            {
                return string.Empty;
            }

            int urat = mentionTab.UnreadCount + dmessageTab.UnreadCount;
            int ur = 0;
            int al = 0;
            int tur = 0;
            int tal = 0;
            StringBuilder slbl = new StringBuilder(256);
            try
            {
                foreach (string key in this.statuses.Tabs.Keys)
                {
                    ur += this.statuses.Tabs[key].UnreadCount;
                    al += this.statuses.Tabs[key].AllCount;
                    if (key.Equals(this.curTab.Text))
                    {
                        tur = this.statuses.Tabs[key].UnreadCount;
                        tal = this.statuses.Tabs[key].AllCount;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            this.unreadCounter = ur;
            this.unreadAtCounter = urat;

            slbl.AppendFormat(Hoehoe.Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, this.postTimestamps.Count, this.favTimestamps.Count, this.timeLineCount);
            if (this.settingDialog.TimelinePeriodInt == 0)
            {
                slbl.Append(Hoehoe.Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(this.settingDialog.TimelinePeriodInt.ToString() + Hoehoe.Properties.Resources.SetStatusLabelText3);
            }

            return slbl.ToString();
        }

        private void SetStatusLabelApi()
        {
            this.apiGauge.RemainCount = MyCommon.TwitterApiInfo.RemainCount;
            this.apiGauge.MaxCount = MyCommon.TwitterApiInfo.MaxCount;
            this.apiGauge.ResetTime = MyCommon.TwitterApiInfo.ResetTime;
        }

        private void SetStatusLabelUrl()
        {
            this.StatusLabelUrl.Text = this.GetStatusLabelText();
        }

        // タスクトレイアイコンのツールチップテキスト書き換え
        // Tween [未読/@]
        private void SetNotifyIconText()
        {
            StringBuilder ur = new StringBuilder(64);
            if (this.settingDialog.DispUsername)
            {
                ur.Append(this.tw.Username);
                ur.Append(" - ");
            }

            ur.Append("Hoehoe");
#if DEBUG
			ur.Append("(Debug Build)");
#endif
            if (this.unreadCounter != -1 && this.unreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(this.unreadCounter);
                ur.Append("/@");
                ur.Append(this.unreadAtCounter);
                ur.Append("]");
            }

            this.NotifyIcon1.Text = ur.ToString();
        }

        private void DoRepliedStatusOpen()
        {
            if (this.ExistCurrentPost && this.curPost.InReplyToUser != null && this.curPost.InReplyToStatusId > 0)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    this.OpenUriAsync(string.Format("https:// twitter.com/{0}/status/{1}", this.curPost.InReplyToUser, this.curPost.InReplyToStatusId));
                    return;
                }

                if (this.statuses.ContainsKey(this.curPost.InReplyToStatusId))
                {
                    PostClass repPost = this.statuses.Item(this.curPost.InReplyToStatusId);
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.Lists | TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(this.curPost.InReplyToStatusId))
                        {
                            break;
                        }

                        PostClass repPost = this.statuses.Item(this.curPost.InReplyToStatusId);
                        MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }

                    this.OpenUriAsync("http://twitter.com/" + this.curPost.InReplyToUser + "/status/" + this.curPost.InReplyToStatusId.ToString());
                }
            }
        }

        /// <summary>
        /// t.coで投稿時自動短縮する場合は、外部サービスでの短縮禁止
        /// Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
        /// 参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
        /// Appendix A.  Collected ABNF for URI
        /// http://www.ietf.org/rfc/rfc3986.txt
        /// </summary>
        /// <param name="urlCoonverterType"></param>
        /// <returns></returns>
        private bool ConvertUrl(UrlConverter urlCoonverterType)
        {
            string result = string.Empty;

            const string NicoUrlPattern = "^https?://[a-z]+\\.(nicovideo|niconicommons|nicolive)\\.jp/[a-z]+/[a-z0-9]+$";

            if (this.StatusText.SelectionLength > 0)
            {
                // 文字列が選択されている場合はその文字列について処理
                string tmp = this.StatusText.SelectedText;

                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http"))
                {
                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settingDialog.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            this.StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        this.StatusText.Select(this.StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                        this.StatusText.SelectedText = result;

                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = tmp, After = result });
                    }
                }
            }
            else
            {
                const string UrlPattern = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?://)" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";

                // 正規表現にマッチしたURL文字列をtinyurl化
                foreach (Match mt in Regex.Matches(this.StatusText.Text, UrlPattern, RegexOptions.IgnoreCase))
                {
                    if (this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) == -1)
                    {
                        continue;
                    }

                    string tmp = mt.Result("${url}");
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase))
                    {
                        tmp = "http://" + tmp;
                    }

                    // 選んだURLを選択（？）
                    this.StatusText.Select(this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settingDialog.Nicoms && Regex.IsMatch(tmp, NicoUrlPattern))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            this.StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        this.StatusText.Select(this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);
                        this.StatusText.SelectedText = result;

                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = mt.Result("${url}"), After = result });
                    }
                }
            }

            return true;
        }

        private void DoUrlUndo()
        {
            if (this.urlUndoBuffer != null)
            {
                string tmp = this.StatusText.Text;
                foreach (UrlUndoInfo data in this.urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }

                this.StatusText.Text = tmp;
                this.urlUndoBuffer = null;
                this.UrlUndoToolStripMenuItem.Enabled = false;
                this.StatusText.SelectionStart = 0;
                this.StatusText.SelectionLength = 0;
            }
        }

        private void DoSearchToolStrip(string url)
        {
            // 発言詳細で「選択文字列で検索」（選択文字列取得）
            string selText = this.WebBrowser_GetSelectionText(ref this.PostBrowser);

            if (selText != null)
            {
                if (url == Hoehoe.Properties.Resources.SearchItem4Url)
                {
                    // 公式検索
                    this.AddNewTabForSearch(selText);
                    return;
                }

                string tmp = string.Format(url, HttpUtility.UrlEncode(selText));
                this.OpenUriAsync(tmp);
            }
        }

        private void ListTabSelect(TabPage tab)
        {
            this.SetListProperty();

            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;

            this.curTab = tab;
            this.curList = (DetailsListView)tab.Tag;
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.curItemIndex = this.curList.SelectedIndices[0];
                this.curPost = this.GetCurTabPost(this.curItemIndex);
            }
            else
            {
                this.curItemIndex = -1;
                this.curPost = null;
            }

            this.anchorPost = null;
            this.anchorFlag = false;

            if (this.iconCol)
            {
                ((DetailsListView)tab.Tag).Columns[1].Text = this.columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= this.curList.Columns.Count - 1; i++)
                {
                    ((DetailsListView)tab.Tag).Columns[i].Text = this.columnTexts[i];
                }
            }
        }

        private void SelectListItem(DetailsListView listView, int index)
        {
            // 単一
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (listView.FocusedItem != null)
            {
                bnd = listView.FocusedItem.Bounds;
                flg = true;
            }

            do
            {
                listView.SelectedIndices.Clear();
            }
            while (listView.SelectedIndices.Count > 0);
            listView.Items[index].Selected = true;
            listView.Items[index].Focused = true;

            if (flg)
            {
                listView.Invalidate(bnd);
            }
        }

        private void SelectListItem(DetailsListView listView, int[] indecies, int focused)
        {
            // 複数
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (listView.FocusedItem != null)
            {
                bnd = listView.FocusedItem.Bounds;
                flg = true;
            }

            int fIdx = -1;
            if (indecies != null && !(indecies.Count() == 1 && indecies[0] == -1))
            {
                do
                {
                    listView.SelectedIndices.Clear();
                }
                while (listView.SelectedIndices.Count > 0);

                foreach (int idx in indecies)
                {
                    if (idx > -1 && listView.VirtualListSize > idx)
                    {
                        listView.SelectedIndices.Add(idx);
                        if (fIdx == -1)
                        {
                            fIdx = idx;
                        }
                    }
                }
            }

            if (focused > -1 && listView.VirtualListSize > focused)
            {
                listView.Items[focused].Focused = true;
            }
            else if (fIdx > -1)
            {
                listView.Items[fIdx].Focused = true;
            }

            if (flg)
            {
                listView.Invalidate(bnd);
            }
        }

        private void RunAsync(GetWorkerArg args)
        {
            BackgroundWorker bw = null;
            if (args.WorkerType != WorkerType.Follower)
            {
                for (int i = 0; i < this.bworkers.Length; i++)
                {
                    if (this.bworkers[i] != null && !this.bworkers[i].IsBusy)
                    {
                        bw = this.bworkers[i];
                        break;
                    }
                }

                if (bw == null)
                {
                    for (int i = 0; i < this.bworkers.Length; i++)
                    {
                        if (this.bworkers[i] == null)
                        {
                            this.bworkers[i] = new BackgroundWorker();
                            bw = this.bworkers[i];
                            bw.WorkerReportsProgress = true;
                            bw.WorkerSupportsCancellation = true;
                            bw.DoWork += this.GetTimelineWorker_DoWork;
                            bw.ProgressChanged += this.GetTimelineWorker_ProgressChanged;
                            bw.RunWorkerCompleted += this.GetTimelineWorker_RunWorkerCompleted;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (this.followerFetchWorker == null)
                {
                    this.followerFetchWorker = new BackgroundWorker();
                    bw = this.followerFetchWorker;
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;
                    bw.DoWork += this.GetTimelineWorker_DoWork;
                    bw.ProgressChanged += this.GetTimelineWorker_ProgressChanged;
                    bw.RunWorkerCompleted += this.GetTimelineWorker_RunWorkerCompleted;
                }
                else
                {
                    if (this.followerFetchWorker.IsBusy == false)
                    {
                        bw = this.followerFetchWorker;
                    }
                }
            }

            if (bw == null)
            {
                return;
            }

            bw.RunWorkerAsync(args);
        }

        private void StartUserStream()
        {
            this.tw.NewPostFromStream += this.Tw_NewPostFromStream;
            this.tw.UserStreamStarted += this.Tw_UserStreamStarted;
            this.tw.UserStreamStopped += this.Tw_UserStreamStopped;
            this.tw.PostDeleted += this.Tw_PostDeleted;
            this.tw.UserStreamEventReceived += this.Tw_UserStreamEventArrived;
            this.MenuItemUserStream.Text = "&UserStream ■";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = "&Start";
            this.StopToolStripMenuItem.Enabled = true;
            if (this.settingDialog.UserstreamStartup)
            {
                this.tw.StartUserStream();
            }
        }

        private bool IsInitialRead()
        {
            return this.waitTimeline || this.waitReply || this.waitDm || this.waitFav || this.waitPubSearch || this.waitUserTimeline || this.waitLists;
        }

        private void DoGetFollowersMenu()
        {
            this.GetTimeline(WorkerType.Follower, 1, 0, string.Empty);
            this.DispSelectedPost(true);
        }

        private void DoReTweetUnofficial()
        {
            // RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsDm || !this.StatusText.Enabled)
                {
                    return;
                }

                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                string rtdata = this.CreateRetweetUnofficial(this.curPost.Text);
                this.StatusText.Text = "RT @" + this.curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                this.StatusText.SelectionStart = 0;
                this.StatusText.Focus();
            }
        }

        private void DoReTweetOfficial(bool isConfirm)
        {
            // 公式RT
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    this.doFavRetweetFlags = false;
                    return;
                }

                if (this.curList.SelectedIndices.Count > 15)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.RetweetLimitText);
                    this.doFavRetweetFlags = false;
                    return;
                }
                else if (this.curList.SelectedIndices.Count > 1)
                {
                    string confirmMessage = Hoehoe.Properties.Resources.RetweetQuestion2;
                    if (this.doFavRetweetFlags)
                    {
                        confirmMessage = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText1;
                    }

                    switch (MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            this.doFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (this.curPost.IsDm || this.curPost.IsMe)
                    {
                        this.doFavRetweetFlags = false;
                        return;
                    }

                    if (!this.settingDialog.RetweetNoConfirm)
                    {
                        string confirmMessage = Hoehoe.Properties.Resources.RetweetQuestion1;
                        if (this.doFavRetweetFlags)
                        {
                            confirmMessage = Hoehoe.Properties.Resources.FavoritesRetweetQuestionText2;
                        }

                        if (isConfirm && MessageBox.Show(confirmMessage, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            this.doFavRetweetFlags = false;
                            return;
                        }
                    }
                }

                GetWorkerArg args = new GetWorkerArg()
                {
                    Ids = new List<long>(),
                    SIds = new List<long>(),
                    TabName = this.curTab.Text,
                    WorkerType = WorkerType.Retweet
                };
                foreach (int idx in this.curList.SelectedIndices)
                {
                    PostClass post = this.GetCurTabPost(idx);
                    if (!post.IsMe && !post.IsProtect && !post.IsDm)
                    {
                        args.Ids.Add(post.StatusId);
                    }
                }

                this.RunAsync(args);
            }
        }

        private void FavoritesRetweetOriginal()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.doFavRetweetFlags = true;
            this.DoReTweetOfficial(true);
            if (this.doFavRetweetFlags)
            {
                this.doFavRetweetFlags = false;
                this.FavoriteChange(true, false);
            }
        }

        private void FavoritesRetweetUnofficial()
        {
            if (this.ExistCurrentPost && !this.curPost.IsDm)
            {
                this.doFavRetweetFlags = true;
                this.FavoriteChange(true);
                if (!this.curPost.IsProtect && this.doFavRetweetFlags)
                {
                    this.doFavRetweetFlags = false;
                    this.DoReTweetUnofficial();
                }
            }
        }

        /// <summary>
        /// Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string CreateRetweetUnofficial(string status)
        {
            MatchCollection ms = Regex.Matches(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>(https?|shttp|ftps?):// [^<]+)</a>");
            foreach (Match m in ms)
            {
                if (m.Result("${link}").EndsWith("..."))
                {
                    break;
                }
            }

            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\" title=\"(?<title>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${title}");

            // その他のリンク(@IDなど)を置き換える
            status = Regex.Replace(status, "@<a target=\"_self\" href=\"https?:// twitter.com/(#!/)?(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "@${url}");

            // ハッシュタグ
            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${link}");

            // <br>タグ除去
            if (this.StatusText.Multiline)
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
            return status.Replace("&nbsp;", " ");
        }

        private bool IsKeyDown(Keys key)
        {
            return (Control.ModifierKeys & key) == key;
        }

        private void FollowCommand(string id)
        {
            if (id == null)
            {
                return;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Follow");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                id = inputName.TabName.Trim();
            }

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (id == this.tw.Username)
            {
                return;
            }

            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = this.tw, Id = id };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.FollowCommandText1, this.FollowCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? Hoehoe.Properties.Resources.FRMessage2 + ret : Hoehoe.Properties.Resources.FRMessage3);
            }
        }

        private void RemoveCommand(string id, bool skipInput)
        {
            if (id == null)
            {
                return;
            }

            if (!skipInput)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Unfollow");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    id = inputName.TabName.Trim();
                }
            }

            if (string.IsNullOrEmpty(id) || id == this.tw.Username)
            {
                return;
            }

            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = this.tw, Id = id };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.RemoveCommandText1, this.RemoveCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                MessageBox.Show(!string.IsNullOrEmpty(ret) ? Hoehoe.Properties.Resources.FRMessage2 + ret : Hoehoe.Properties.Resources.FRMessage3);
            }
        }

        private void ShowFriendship(string id)
        {
            if (id == null)
            {
                return;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Show Friendships");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                id = inputName.TabName;
            }

            ShowFriendshipCore(id);
        }

        private void ShowFriendship(string[] ids)
        {
            foreach (string id in ids)
            {
                ShowFriendshipCore(id);
            }
        }

        private void ShowFriendshipCore(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                if (id.ToLower() == this.tw.Username.ToLower())
                {
                    return;
                }

                ShowFriendshipArgs args = new ShowFriendshipArgs() { Tw = this.tw };
                args.Ids.Add(new ShowFriendshipArgs.FriendshipInfo(id));
                string ret = string.Empty;
                using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                {
                    formInfo.ShowDialog();
                    ret = (string)formInfo.Result;
                }

                if (string.IsNullOrEmpty(ret))
                {
                    ShowFriendshipArgs.FriendshipInfo frsinfo = args.Ids[0];
                    string fing = frsinfo.IsFollowing ?
                        Hoehoe.Properties.Resources.GetFriendshipInfo1 :
                        Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    string fed = frsinfo.IsFollowed ?
                        Hoehoe.Properties.Resources.GetFriendshipInfo3 :
                        Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    string result = frsinfo.Id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine;
                    result += "  " + fing + System.Environment.NewLine;
                    result += "  " + fed;
                    if (frsinfo.IsFollowing)
                    {
                        if (MessageBox.Show(Hoehoe.Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result,
                            Hoehoe.Properties.Resources.GetFriendshipInfo8, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.RemoveCommand(frsinfo.Id, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show(result);
                    }
                }
                else
                {
                    MessageBox.Show(ret);
                }
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this.postBrowserStatusText, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
            {
                return m.Result("${ScreenName}");
            }
            else
            {
                return null;
            }
        }

        private void DoQuote()
        {
            // QT @id:内容
            // 返信先情報付加
            if (this.ExistCurrentPost)
            {
                if (this.curPost.IsDm || !this.StatusText.Enabled)
                {
                    return;
                }

                if (this.curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                string rtdata = this.CreateRetweetUnofficial(this.curPost.Text);
                this.StatusText.Text = " QT @" + this.curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                this.replyToId = this.curPost.OriginalStatusId;
                this.replyToName = this.curPost.ScreenName;

                this.StatusText.SelectionStart = 0;
                this.StatusText.Focus();
            }
        }

        private void TryOpenSelectedRtUserHome()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                var post = this.GetCurTabPost(this.curList.SelectedIndices[0]);
                if (post.IsRetweeted)
                {
                    this.OpenUriAsync("https://twitter.com/" + post.RetweetedBy);
                }
            }
        }
        
        private void ShowUserStatus(string id, bool showInputDialog = true)
        {
            if (id == null)
            {
                return;
            }

            if (showInputDialog)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Show UserStatus");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    id = inputName.TabName.Trim();
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var user = new DataModels.Twitter.User();
            GetUserInfoArgs args = new GetUserInfoArgs() { Tw = this.tw, Id = id, User = user };
            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                if (string.IsNullOrEmpty(ret))
                {

                    using (ShowUserInfo userinfo = new ShowUserInfo())
                    {
                        userinfo.Owner = this;
                        userinfo.SetUser(user);
                        userinfo.ShowDialog(this);
                        this.Activate();
                        this.BringToFront();
                    }
                }
                else
                {
                    MessageBox.Show(ret);
                }
            }
        }

        private void LoadImageFromSelectedFile()
        {
            try
            {
                if (string.IsNullOrEmpty(this.ImagefilePathText.Text.Trim()) || string.IsNullOrEmpty(this.ImageService))
                {
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    return;
                }

                FileInfo fl = new FileInfo(this.ImagefilePathText.Text.Trim());
                if (!this.pictureServices[this.ImageService].CheckValidExtension(fl.Extension))
                {
                    // 画像以外の形式
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    return;
                }

                if (!this.pictureServices[this.ImageService].CheckValidFilesize(fl.Extension, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    this.ImagefilePathText.Text = string.Empty;
                    MessageBox.Show("File is too large.");
                    return;
                }

                switch (this.pictureServices[this.ImageService].GetFileType(fl.Extension))
                {
                    case UploadFileType.Invalid:
                        this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        this.ImagefilePathText.Text = string.Empty;
                        break;
                    case UploadFileType.Picture:
                        Image img = null;
                        using (FileStream fs = new FileStream(this.ImagefilePathText.Text, FileMode.Open, FileAccess.Read))
                        {
                            img = Image.FromStream(fs);
                            fs.Close();
                        }

                        this.ImageSelectedPicture.Image = (new HttpVarious()).CheckValidImage(img, img.Width, img.Height);
                        this.ImageSelectedPicture.Tag = UploadFileType.Picture;
                        break;
                    case UploadFileType.MultiMedia:
                        this.ImageSelectedPicture.Image = Hoehoe.Properties.Resources.MultiMediaImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.MultiMedia;
                        break;
                    default:
                        this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        this.ImagefilePathText.Text = string.Empty;
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                this.ImagefilePathText.Text = string.Empty;
                MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                this.ImagefilePathText.Text = string.Empty;
                MessageBox.Show("The type of this file is not image.");
            }
        }

        private void SetImageServiceCombo()
        {
            string svc = string.Empty;
            if (this.ImageServiceCombo.SelectedIndex > -1)
            {
                svc = this.ImageServiceCombo.SelectedItem.ToString();
            }

            this.ImageServiceCombo.Items.Clear();
            this.ImageServiceCombo.Items.Add("TwitPic");
            this.ImageServiceCombo.Items.Add("img.ly");
            this.ImageServiceCombo.Items.Add("yfrog");
            this.ImageServiceCombo.Items.Add("lockerz");
            this.ImageServiceCombo.Items.Add("Twitter");

            if (string.IsNullOrEmpty(svc))
            {
                this.ImageServiceCombo.SelectedIndex = 0;
            }
            else
            {
                int idx = this.ImageServiceCombo.Items.IndexOf(svc);
                this.ImageServiceCombo.SelectedIndex = idx == -1 ? 0 : idx;
            }
        }

        private void CopyUserId()
        {
            if (this.curPost == null)
            {
                return;
            }

            string clstr = this.curPost.ScreenName;
            try
            {
                Clipboard.SetDataObject(clstr, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            // 新着通知
            if (this.BalloonRequired(ev))
            {
                this.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                StringBuilder title = new StringBuilder();
                if (this.settingDialog.DispUsername)
                {
                    title.Append(this.tw.Username);
                    title.Append(" - ");
                }

                title.Append(string.Format("Hoehoe [{0}] ", ev.Event.ToUpper()));
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(string.Format("by {0}", ev.Username));
                }

                string text = !string.IsNullOrEmpty(ev.Target) ? ev.Target : " ";

                if (this.settingDialog.IsNotifyUseGrowl)
                {
                    this.growlHelper.Notify(GrowlHelper.NotifyType.UserStreamEvent, ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    this.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                    this.NotifyIcon1.BalloonTipTitle = title.ToString();
                    this.NotifyIcon1.BalloonTipText = text;
                    this.NotifyIcon1.ShowBalloonTip(500);
                }
            }

            // サウンド再生
            string snd = this.settingDialog.EventSoundFile;
            if (!this.isInitializing && this.settingDialog.PlaySound && !string.IsNullOrEmpty(snd))
            {
                if (Convert.ToBoolean(ev.Eventtype & this.settingDialog.EventNotifyFlag) && this.IsMyEventNotityAsEventType(ev))
                {
                    try
                    {
                        string dir = MyCommon.AppDir;
                        if (Directory.Exists(Path.Combine(dir, "Sounds")))
                        {
                            dir = Path.Combine(dir, "Sounds");
                        }

                        new SoundPlayer(Path.Combine(dir, snd)).Play();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void DoTranslation(string str)
        {
            Bing bing = new Bing();
            string buf = string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            string srclng = string.Empty;
            string dstlng = this.settingDialog.TranslateLanguage;
            string msg = string.Empty;
            if (srclng != dstlng && bing.Translate(string.Empty, dstlng, str, ref buf))
            {
                this.PostBrowser.DocumentText = this.CreateDetailHtml(buf);
            }
            else
            {
                if (msg.StartsWith("Err:"))
                {
                    this.StatusLabel.Text = msg;
                }
            }
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle(caption);
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    id = inputName.TabName.Trim();
                }
                else
                {
                    id = string.Empty;
                }
            }

            return id;
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            if (isEnable)
            {
                this.tw.StartUserStream();
            }
            else
            {
                this.tw.StopUserStream();
            }

            this.timerTimeline.Enabled = isEnable;
        }

        private void OpenUserAppointUrl()
        {
            if (this.settingDialog.UserAppointUrl != null)
            {
                if (this.settingDialog.UserAppointUrl.Contains("{ID}") || this.settingDialog.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (this.curPost != null)
                    {
                        string url = this.settingDialog.UserAppointUrl
                            .Replace("{ID}", this.curPost.ScreenName)
                            .Replace("{STATUS}", this.curPost.OriginalStatusId.ToString());
                        this.OpenUriAsync(url);
                    }
                }
                else
                {
                    this.OpenUriAsync(this.settingDialog.UserAppointUrl);
                }
            }
        }

        #endregion private methods
    }
}