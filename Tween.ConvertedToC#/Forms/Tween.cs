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
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using Hoehoe.DataModels;
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
        private IDictionary<string, Image> iconDict;

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
        private string prevTrackWord;

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
                        this.PostButton_Click(null, null);
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
            else if (post.RetweetedId > 0)
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

            ImageListViewItem itm = null;
            if (post.RetweetedId == 0)
            {
                string[] sitem = { string.Empty, post.Nickname, post.IsDeleted ? "(DELETED)" : post.TextFromApi, post.CreatedAt.ToString(this.settingDialog.DateTimeFormat), post.ScreenName, string.Empty, mk.ToString(), post.Source };
                itm = new ImageListViewItem(sitem, (ImageDictionary)this.iconDict, post.ImageUrl);
            }
            else
            {
                string[] sitem = { string.Empty, post.Nickname, post.IsDeleted ? "(DELETED)" : post.TextFromApi, post.CreatedAt.ToString(this.settingDialog.DateTimeFormat), post.ScreenName + Environment.NewLine + "(RT:" + post.RetweetedBy + ")", string.Empty, mk.ToString(), post.Source };
                itm = new ImageListViewItem(sitem, (ImageDictionary)this.iconDict, post.ImageUrl);
            }

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
                                dialogAsShieldicon.Dispose();
                                this.Close();
                                return;
                            }
                            else
                            {
                                if (!startup)
                                {
                                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }

                        dialogAsShieldicon.Dispose();
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
                                    dialogAsShieldicon.Dispose();
                                    this.Close();
                                    return;
                                }
                                else
                                {
                                    if (!startup)
                                    {
                                        MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText5 + Environment.NewLine + retMsg, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    }
                                }
                            }

                            dialogAsShieldicon.Dispose();
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

        private void DispSelectedPost()
        {
            this.DispSelectedPost(false);
        }

        private void DispSelectedPost(bool forceupdate)
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
                this.NameLabel.Text += " (RT:" + this.curPost.RetweetedBy + ")";
            }

            if (this.UserPicture.Image != null)
            {
                this.UserPicture.Image.Dispose();
            }

            if (!string.IsNullOrEmpty(this.curPost.ImageUrl) && this.iconDict[this.curPost.ImageUrl] != null)
            {
                try
                {
                    this.UserPicture.Image = new Bitmap(this.iconDict[this.curPost.ImageUrl]);
                }
                catch (Exception)
                {
                    this.UserPicture.Image = null;
                }
            }
            else
            {
                this.UserPicture.Image = null;
            }

            this.NameLabel.ForeColor = SystemColors.ControlText;
            this.DateTimeLabel.Text = this.curPost.CreatedAt.ToString();
            if (this.curPost.IsOwl && (this.settingDialog.OneWayLove || this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.DirectMessage))
            {
                this.NameLabel.ForeColor = this.clrOWL;
            }

            if (this.curPost.RetweetedId > 0)
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
                            this.MenuItemSearchNext_Click(null, null);
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

                                this.JumpUnreadMenuItem_Click(null, null);
                                return true;
                            case Keys.G:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    this.anchorFlag = false;
                                }

                                this.ShowRelatedStatusesMenuItem_Click(null, null);
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
                            this.ReadedStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            this.HashManageMenuItem_Click(null, null);
                            return true;
                        case Keys.L:
                            this.UrlConvertAutoToolStripMenuItem_Click(null, null);
                            return true;
                        case Keys.Y:
                            if (!(focusedControl == FocusedControl.PostBrowser))
                            {
                                this.MultiLineMenuItem_Click(null, null);
                                return true;
                            }

                            break;
                        case Keys.F:
                            this.MenuItemSubSearch_Click(null, null);
                            return true;
                        case Keys.U:
                            this.ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            if (this.curList.SelectedIndices.Count > 0)
                            {
                                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName);
                            }
                            else if (this.curList.SelectedIndices.Count == 0)
                            {
                                this.OpenUriAsync("http://twitter.com/");
                            }

                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            if (this.curList.SelectedIndices.Count > 0)
                            {
                                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName + "/favorites");
                            }

                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            this.StatusOpenMenuItem_Click(null, null);
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            this.OpenURLMenuItem_Click(null, null);
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
                                MultiLineMenuItem_Click(null, null);
                                return true;
                        }
                    }

                    break;
                case ModifierState.Shift:
                    // フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F3:
                            this.MenuItemSearchPrev_Click(null, null);
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
                                this.DoShowUserStatus(this.curPost.ScreenName, false);
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
                            this.UnreadStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            this.HashToggleMenuItem_Click(null, null);
                            return true;
                        case Keys.P:
                            this.ImageSelectMenuItem_Click(null, null);
                            return true;
                        case Keys.H:
                            this.DoMoveToRTHome();
                            return true;
                        case Keys.O:
                            this.FavorareMenuItem_Click(null, null);
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
                                            break; // TODO: might not be correct. Was : Exit For
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
                    if (post.RetweetedId > 0)
                    {
                        sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.RetweetedId, Environment.NewLine);
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine);
                    }
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
                if (post.RetweetedId > 0)
                {
                    sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.RetweetedId, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.StatusId, Environment.NewLine);
                }
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

            string name = string.Empty;
            if (this.curPost.RetweetedId == 0)
            {
                name = this.curPost.ScreenName;
            }
            else
            {
                name = this.curPost.RetweetedBy;
            }

            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                if (this.statuses.Item(this.curTab.Text, idx).RetweetedId == 0)
                {
                    if (this.statuses.Item(this.curTab.Text, idx).ScreenName == name)
                    {
                        this.SelectListItem(this.curList, idx);
                        this.curList.EnsureVisible(idx);
                        break;
                    }
                }
                else
                {
                    if (this.statuses.Item(this.curTab.Text, idx).RetweetedBy == name)
                    {
                        this.SelectListItem(this.curList, idx);
                        this.curList.EnsureVisible(idx);
                        break;
                    }
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
                if (post.ScreenName == this.anchorPost.ScreenName || post.RetweetedBy == this.anchorPost.ScreenName || post.ScreenName == this.anchorPost.RetweetedBy || (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == this.anchorPost.RetweetedBy) || this.anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) || this.anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) || post.ReplyToList.Contains(this.anchorPost.ScreenName.ToLower()) || post.ReplyToList.Contains(this.anchorPost.RetweetedBy.ToLower()))
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
                        this.OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
                        return;
                    }
                }
                else
                {
                    this.StatusLabel.Text = r;
                    this.OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
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

                        var post = postList.FirstOrDefault(pst => object.ReferenceEquals(pst.Tab, curTabClass) && (bool)(isForward ? pst.Index > this.curItemIndex : pst.Index < this.curItemIndex));
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
                        // 空の場合
                        // ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        this.StatusText.Text = "@" + this.curPost.ScreenName + " ";
                        if (this.curPost.RetweetedId > 0)
                        {
                            this.replyToId = this.curPost.RetweetedId;
                        }
                        else
                        {
                            this.replyToId = this.curPost.StatusId;
                        }

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
                                    if (this.curPost.RetweetedId > 0)
                                    {
                                        this.replyToId = this.curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        this.replyToId = this.curPost.StatusId;
                                    }

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
                                    if (this.curPost.RetweetedId > 0)
                                    {
                                        this.replyToId = this.curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        this.replyToId = this.curPost.StatusId;
                                    }

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
                                if (post.RetweetedId > 0)
                                {
                                    this.replyToId = post.RetweetedId;
                                }
                                else
                                {
                                    this.replyToId = post.StatusId;
                                }

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
        private bool UrlConvert(UrlConverter urlCoonverterType)
        {
            string result = string.Empty;

            const string NicoUrlPattern = "^https?:// [a-z]+\\.(nicovideo|niconicommons|nicolive)\\.jp/[a-z]+/[a-z0-9]+$";

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
                const string UrlPattern = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?:// )" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";

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
            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Follow");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs();
                    arg.Tw = this.tw;
                    arg.Id = inputName.TabName.Trim();
                    using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.FollowCommandText1, this.FollowCommand_DoWork, null, arg))
                    {
                        info.ShowDialog();
                        string ret = (string)info.Result;
                        if (!string.IsNullOrEmpty(ret))
                        {
                            MessageBox.Show(Hoehoe.Properties.Resources.FRMessage2 + ret);
                        }
                        else
                        {
                            MessageBox.Show(Hoehoe.Properties.Resources.FRMessage3);
                        }
                    }
                }
            }
        }
        
        private void RemoveCommand(string id, bool skipInput)
        {
            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = this.tw, Id = id };
            if (!skipInput)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Unfollow");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                    {
                        arg.Tw = this.tw;
                        arg.Id = inputName.TabName.Trim();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.RemoveCommandText1, this.RemoveCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.FRMessage2 + ret);
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.FRMessage3);
                }
            }
        }
        
        private void ShowFriendship(string id)
        {
            ShowFriendshipArgs args = new ShowFriendshipArgs();
            args.Tw = this.tw;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Show Friendships");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    string ret = string.Empty;
                    args.Ids.Add(new ShowFriendshipArgs.FriendshipInfo(inputName.TabName.Trim()));
                    using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                    {
                        formInfo.ShowDialog();
                        ret = (string)formInfo.Result;
                    }

                    string result = string.Empty;
                    if (string.IsNullOrEmpty(ret))
                    {
                        if (args.Ids[0].IsFollowing)
                        {
                            result = Hoehoe.Properties.Resources.GetFriendshipInfo1 + System.Environment.NewLine;
                        }
                        else
                        {
                            result = Hoehoe.Properties.Resources.GetFriendshipInfo2 + System.Environment.NewLine;
                        }

                        if (args.Ids[0].IsFollowed)
                        {
                            result += Hoehoe.Properties.Resources.GetFriendshipInfo3;
                        }
                        else
                        {
                            result += Hoehoe.Properties.Resources.GetFriendshipInfo4;
                        }

                        result = args.Ids[0].Id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + result;
                    }
                    else
                    {
                        result = ret;
                    }

                    MessageBox.Show(result);
                }
            }
        }
        
        private void ShowFriendship(string[] ids)
        {
            foreach (string id in ids)
            {
                string ret = string.Empty;
                ShowFriendshipArgs args = new ShowFriendshipArgs();
                args.Tw = this.tw;
                args.Ids.Add(new ShowFriendshipArgs.FriendshipInfo(id.Trim()));
                using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                {
                    formInfo.ShowDialog();
                    ret = (string)formInfo.Result;
                }

                string result = string.Empty;
                ShowFriendshipArgs.FriendshipInfo fInfo = args.Ids[0];
                string ff = string.Empty;
                if (string.IsNullOrEmpty(ret))
                {
                    ff = "  ";
                    if (fInfo.IsFollowing)
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo1;
                    }
                    else
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    }

                    ff += System.Environment.NewLine + "  ";
                    if (fInfo.IsFollowed)
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo3;
                    }
                    else
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    }

                    result += fInfo.Id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + ff;
                    if (fInfo.IsFollowing)
                    {
                        if (MessageBox.Show(Hoehoe.Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result, Hoehoe.Properties.Resources.GetFriendshipInfo8, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.RemoveCommand(fInfo.Id, true);
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
            Match m = Regex.Match(this.postBrowserStatusText, "^https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
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
                if (this.curPost.RetweetedId == 0)
                {
                    this.replyToId = this.curPost.StatusId;
                }
                else
                {
                    this.replyToId = this.curPost.RetweetedId;
                }

                this.replyToName = this.curPost.ScreenName;

                this.StatusText.SelectionStart = 0;
                this.StatusText.Focus();
            }
        }
        
        private void DoMoveToRTHome()
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                PostClass post = this.GetCurTabPost(this.curList.SelectedIndices[0]);
                if (post.RetweetedId > 0)
                {
                    this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).RetweetedBy);
                }
            }
        }
        
        private void DoShowUserStatus(string id, bool showInputDialog)
        {
            DataModels.Twitter.User user = null;
            if (showInputDialog)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Show UserStatus");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                    {
                        id = inputName.TabName.Trim();
                        GetUserInfoArgs args = new GetUserInfoArgs() { Tw = this.tw, Id = id, User = user };
                        using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
                        {
                            info.ShowDialog();
                            string ret = (string)info.Result;
                            if (string.IsNullOrEmpty(ret))
                            {
                                this.DoShowUserStatus(args.User);
                            }
                            else
                            {
                                MessageBox.Show(ret);
                            }
                        }
                    }
                }
            }
            else
            {
                GetUserInfoArgs args = new GetUserInfoArgs() { Tw = this.tw, Id = id, User = user };
                using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
                {
                    info.ShowDialog();
                    string ret = (string)info.Result;
                    if (string.IsNullOrEmpty(ret))
                    {
                        this.DoShowUserStatus(args.User);
                    }
                    else
                    {
                        MessageBox.Show(ret);
                    }
                }
            }
        }
        
        private void DoShowUserStatus(DataModels.Twitter.User user)
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
        
        private void ShowUserStatus(string id, bool showInputDialog)
        {
            this.DoShowUserStatus(id, showInputDialog);
        }
        
        private void ShowUserStatus(string id)
        {
            this.DoShowUserStatus(id, true);
        }
        
        private void ImageFromSelectedFile()
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
                if (idx == -1)
                {
                    this.ImageServiceCombo.SelectedIndex = 0;
                }
                else
                {
                    this.ImageServiceCombo.SelectedIndex = idx;
                }
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

                title.Append("Hoehoe [");
                title.Append(ev.Event.ToUpper());
                title.Append("] by ");
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(ev.Username.ToString());
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
                        string url = this.settingDialog.UserAppointUrl;
                        url = url.Replace("{ID}", this.curPost.ScreenName);
                        if (this.curPost.RetweetedId != 0)
                        {
                            url = url.Replace("{STATUS}", this.curPost.RetweetedId.ToString());
                        }
                        else
                        {
                            url = url.Replace("{STATUS}", this.curPost.StatusId.ToString());
                        }

                        this.OpenUriAsync(url);
                    }
                }
                else
                {
                    this.OpenUriAsync(this.settingDialog.UserAppointUrl);
                }
            }
        }

        #region event handler

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            // 画面がアクティブになったら、発言欄の背景色戻す
            if (this.StatusText.Focused)
            {
                this.StatusText_Enter(this.StatusText, EventArgs.Empty);
            }
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            // 後始末
            this.settingDialog.Dispose();
            this.tabDialog.Dispose();
            this.searchDialog.Dispose();
            this.fltDialog.Dispose();
            this.urlDialog.Dispose();
            this.spaceKeyCanceler.Dispose();
            if (this.iconAt != null)
            {
                this.iconAt.Dispose();
            }

            if (this.iconAtRed != null)
            {
                this.iconAtRed.Dispose();
            }

            if (this.iconAtSmoke != null)
            {
                this.iconAtSmoke.Dispose();
            }

            if (this.iconRefresh[0] != null)
            {
                this.iconRefresh[0].Dispose();
            }

            if (this.iconRefresh[1] != null)
            {
                this.iconRefresh[1].Dispose();
            }

            if (this.iconRefresh[2] != null)
            {
                this.iconRefresh[2].Dispose();
            }

            if (this.iconRefresh[3] != null)
            {
                this.iconRefresh[3].Dispose();
            }

            if (this.tabIcon != null)
            {
                this.tabIcon.Dispose();
            }

            if (this.mainIcon != null)
            {
                this.mainIcon.Dispose();
            }

            if (this.replyIcon != null)
            {
                this.replyIcon.Dispose();
            }

            if (this.replyIconBlink != null)
            {
                this.replyIconBlink.Dispose();
            }

            this.brsHighLight.Dispose();
            this.brsHighLightText.Dispose();
            if (this.brsForeColorUnread != null)
            {
                this.brsForeColorUnread.Dispose();
            }

            if (this.brsForeColorReaded != null)
            {
                this.brsForeColorReaded.Dispose();
            }

            if (this.brsForeColorFav != null)
            {
                this.brsForeColorFav.Dispose();
            }

            if (this.brsForeColorOWL != null)
            {
                this.brsForeColorOWL.Dispose();
            }

            if (this.brsForeColorRetweet != null)
            {
                this.brsForeColorRetweet.Dispose();
            }

            if (this.brsBackColorMine != null)
            {
                this.brsBackColorMine.Dispose();
            }

            if (this.brsBackColorAt != null)
            {
                this.brsBackColorAt.Dispose();
            }

            if (this.brsBackColorYou != null)
            {
                this.brsBackColorYou.Dispose();
            }

            if (this.brsBackColorAtYou != null)
            {
                this.brsBackColorAtYou.Dispose();
            }

            if (this.brsBackColorAtFromTarget != null)
            {
                this.brsBackColorAtFromTarget.Dispose();
            }

            if (this.brsBackColorAtTo != null)
            {
                this.brsBackColorAtTo.Dispose();
            }

            if (this.brsBackColorNone != null)
            {
                this.brsBackColorNone.Dispose();
            }

            if (this.brsDeactiveSelection != null)
            {
                this.brsDeactiveSelection.Dispose();
            }

            this.shield.Dispose();
            this.tabStringFormat.Dispose();
            foreach (BackgroundWorker bw in this.bworkers)
            {
                if (bw != null)
                {
                    bw.Dispose();
                }
            }

            if (this.followerFetchWorker != null)
            {
                this.followerFetchWorker.Dispose();
            }

            this.apiGauge.Dispose();
            if (this.iconDict != null)
            {
                ((ImageDictionary)this.iconDict).PauseGetImage = true;
                ((IDisposable)this.iconDict).Dispose();
            }

            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            this.ignoreConfigSave = true;
            this.Visible = false;

            this.securityManager = new InternetSecurityManager(this.PostBrowser);
            this.thumbnail = new Thumbnail(this);

            MyCommon.TwitterApiInfo.Changed += this.SetStatusLabelApiHandler;
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;

            this.VerUpMenuItem.Image = this.shield.Icon;
            var cmdArgs = System.Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (cmdArgs.Length != 0 && cmdArgs.Contains("/d"))
            {
                MyCommon.TraceFlag = true;
            }

            this.spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this.spaceKeyCanceler.SpaceCancel += this.SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            this.InitializeTraceFrag();

            // アイコン読み込み
            this.LoadIcons();

            // 発言保持クラス
            this.statuses = TabInformations.GetInstance();

            // アイコン設定
            this.Icon = this.mainIcon;          // メインフォーム（TweenMain）
            this.NotifyIcon1.Icon = this.iconAt;     // タスクトレイ
            this.TabImage.Images.Add(this.tabIcon);  // タブ見出し

            this.settingDialog.Owner = this;
            this.searchDialog.Owner = this;
            this.fltDialog.Owner = this;
            this.tabDialog.Owner = this;
            this.urlDialog.Owner = this;

            this.postHistory.Add(new PostingStatus());
            this.postHistoryIndex = 0;
            this.replyToId = 0;
            this.replyToName = string.Empty;

            // <<<<<<<<<設定関連>>>>>>>>>
            // '設定読み出し
            this.LoadConfig();

            // 新着バルーン通知のチェック状態設定
            this.NewPostPopMenuItem.Checked = this.cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = this.NewPostPopMenuItem.Checked;

            // フォント＆文字色＆背景色保持
            this.fntUnread = this.cfgLocal.FontUnread;
            this.clrUnread = this.cfgLocal.ColorUnread;
            this.fntReaded = this.cfgLocal.FontRead;
            this.clrRead = this.cfgLocal.ColorRead;
            this.clrFav = this.cfgLocal.ColorFav;
            this.clrOWL = this.cfgLocal.ColorOWL;
            this.clrRetweet = this.cfgLocal.ColorRetweet;
            this.fntDetail = this.cfgLocal.FontDetail;
            this.clrDetail = this.cfgLocal.ColorDetail;
            this.clrDetailLink = this.cfgLocal.ColorDetailLink;
            this.clrDetailBackcolor = this.cfgLocal.ColorDetailBackcolor;
            this.clrSelf = this.cfgLocal.ColorSelf;
            this.clrAtSelf = this.cfgLocal.ColorAtSelf;
            this.clrTarget = this.cfgLocal.ColorTarget;
            this.clrAtTarget = this.cfgLocal.ColorAtTarget;
            this.clrAtFromTarget = this.cfgLocal.ColorAtFromTarget;
            this.clrAtTo = this.cfgLocal.ColorAtTo;
            this.clrListBackcolor = this.cfgLocal.ColorListBackcolor;
            this.InputBackColor = this.cfgLocal.ColorInputBackcolor;
            this.clrInputForecolor = this.cfgLocal.ColorInputFont;
            this.fntInputFont = this.cfgLocal.FontInputFont;

            this.brsForeColorUnread = new SolidBrush(this.clrUnread);
            this.brsForeColorReaded = new SolidBrush(this.clrRead);
            this.brsForeColorFav = new SolidBrush(this.clrFav);
            this.brsForeColorOWL = new SolidBrush(this.clrOWL);
            this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
            this.brsBackColorMine = new SolidBrush(this.clrSelf);
            this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
            this.brsBackColorYou = new SolidBrush(this.clrTarget);
            this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
            this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
            this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
            this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);

            // StringFormatオブジェクトへの事前設定
            this.tabStringFormat.Alignment = StringAlignment.Center;
            this.tabStringFormat.LineAlignment = StringAlignment.Center;

            // 設定画面への反映
            HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);
            this.settingDialog.TwitterApiUrl = this.cfgCommon.TwitterUrl;
            this.settingDialog.TwitterSearchApiUrl = this.cfgCommon.TwitterSearchUrl;

            // 認証関連
            if (string.IsNullOrEmpty(this.cfgCommon.Token))
            {
                this.cfgCommon.UserName = string.Empty;
            }

            this.tw.Initialize(this.cfgCommon.Token, this.cfgCommon.TokenSecret, this.cfgCommon.UserName, this.cfgCommon.UserId);

            this.settingDialog.UserAccounts = this.cfgCommon.UserAccounts;
            this.settingDialog.TimelinePeriodInt = this.cfgCommon.TimelinePeriod;
            this.settingDialog.ReplyPeriodInt = this.cfgCommon.ReplyPeriod;
            this.settingDialog.DMPeriodInt = this.cfgCommon.DMPeriod;
            this.settingDialog.PubSearchPeriodInt = this.cfgCommon.PubSearchPeriod;
            this.settingDialog.UserTimelinePeriodInt = this.cfgCommon.UserTimelinePeriod;
            this.settingDialog.ListsPeriodInt = this.cfgCommon.ListsPeriod;

            // 不正値チェック
            if (!cmdArgs.Contains("nolimit"))
            {
                if (this.settingDialog.TimelinePeriodInt < 15 && this.settingDialog.TimelinePeriodInt > 0)
                {
                    this.settingDialog.TimelinePeriodInt = 15;
                }

                if (this.settingDialog.ReplyPeriodInt < 15 && this.settingDialog.ReplyPeriodInt > 0)
                {
                    this.settingDialog.ReplyPeriodInt = 15;
                }

                if (this.settingDialog.DMPeriodInt < 15 && this.settingDialog.DMPeriodInt > 0)
                {
                    this.settingDialog.DMPeriodInt = 15;
                }

                if (this.settingDialog.PubSearchPeriodInt < 30 && this.settingDialog.PubSearchPeriodInt > 0)
                {
                    this.settingDialog.PubSearchPeriodInt = 30;
                }

                if (this.settingDialog.UserTimelinePeriodInt < 15 && this.settingDialog.UserTimelinePeriodInt > 0)
                {
                    this.settingDialog.UserTimelinePeriodInt = 15;
                }

                if (this.settingDialog.ListsPeriodInt < 15 && this.settingDialog.ListsPeriodInt > 0)
                {
                    this.settingDialog.ListsPeriodInt = 15;
                }
            }

            // 起動時読み込み分を既読にするか。Trueなら既読として処理
            this.settingDialog.Readed = this.cfgCommon.Read;

            // 新着取得時のリストスクロールをするか。Trueならスクロールしない
            this.ListLockMenuItem.Checked = this.cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = this.cfgCommon.ListLock;
            this.settingDialog.IconSz = this.cfgCommon.IconSize;

            // 文末ステータス
            this.settingDialog.Status = this.cfgLocal.StatusText;

            // 未読管理。Trueなら未読管理する
            this.settingDialog.UnreadManage = this.cfgCommon.UnreadManage;

            // サウンド再生（タブ別設定より優先）
            this.settingDialog.PlaySound = this.cfgCommon.PlaySound;
            this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;

            // 片思い表示。Trueなら片思い表示する
            this.settingDialog.OneWayLove = this.cfgCommon.OneWayLove;

            // フォント＆文字色＆背景色
            this.settingDialog.FontUnread = this.fntUnread;
            this.settingDialog.ColorUnread = this.clrUnread;
            this.settingDialog.FontReaded = this.fntReaded;
            this.settingDialog.ColorReaded = this.clrRead;
            this.settingDialog.ColorFav = this.clrFav;
            this.settingDialog.ColorOWL = this.clrOWL;
            this.settingDialog.ColorRetweet = this.clrRetweet;
            this.settingDialog.FontDetail = this.fntDetail;
            this.settingDialog.ColorDetail = this.clrDetail;
            this.settingDialog.ColorDetailLink = this.clrDetailLink;
            this.settingDialog.ColorDetailBackcolor = this.clrDetailBackcolor;
            this.settingDialog.ColorSelf = this.clrSelf;
            this.settingDialog.ColorAtSelf = this.clrAtSelf;
            this.settingDialog.ColorTarget = this.clrTarget;
            this.settingDialog.ColorAtTarget = this.clrAtTarget;
            this.settingDialog.ColorAtFromTarget = this.clrAtFromTarget;
            this.settingDialog.ColorAtTo = this.clrAtTo;
            this.settingDialog.ColorListBackcolor = this.clrListBackcolor;
            this.settingDialog.ColorInputBackcolor = this.InputBackColor;
            this.settingDialog.ColorInputFont = this.clrInputForecolor;
            this.settingDialog.FontInputFont = this.fntInputFont;
            this.settingDialog.NameBalloon = this.cfgCommon.NameBalloon;
            this.settingDialog.PostCtrlEnter = this.cfgCommon.PostCtrlEnter;
            this.settingDialog.PostShiftEnter = this.cfgCommon.PostShiftEnter;
            this.settingDialog.CountApi = this.cfgCommon.CountApi;
            this.settingDialog.CountApiReply = this.cfgCommon.CountApiReply;
            if (this.settingDialog.CountApi < 20 || this.settingDialog.CountApi > 200)
            {
                this.settingDialog.CountApi = 60;
            }

            if (this.settingDialog.CountApiReply < 20 || this.settingDialog.CountApiReply > 200)
            {
                this.settingDialog.CountApiReply = 40;
            }

            this.settingDialog.BrowserPath = this.cfgLocal.BrowserPath;
            this.settingDialog.PostAndGet = this.cfgCommon.PostAndGet;
            this.settingDialog.UseRecommendStatus = this.cfgLocal.UseRecommendStatus;
            this.settingDialog.DispUsername = this.cfgCommon.DispUsername;
            this.settingDialog.CloseToExit = this.cfgCommon.CloseToExit;
            this.settingDialog.MinimizeToTray = this.cfgCommon.MinimizeToTray;
            this.settingDialog.DispLatestPost = this.cfgCommon.DispLatestPost;
            this.settingDialog.SortOrderLock = this.cfgCommon.SortOrderLock;
            this.settingDialog.TinyUrlResolve = this.cfgCommon.TinyUrlResolve;
            this.settingDialog.ShortUrlForceResolve = this.cfgCommon.ShortUrlForceResolve;
            this.settingDialog.SelectedProxyType = this.cfgLocal.ProxyType;
            this.settingDialog.ProxyAddress = this.cfgLocal.ProxyAddress;
            this.settingDialog.ProxyPort = this.cfgLocal.ProxyPort;
            this.settingDialog.ProxyUser = this.cfgLocal.ProxyUser;
            this.settingDialog.ProxyPassword = this.cfgLocal.ProxyPassword;
            this.settingDialog.PeriodAdjust = this.cfgCommon.PeriodAdjust;
            this.settingDialog.StartupVersion = this.cfgCommon.StartupVersion;
            this.settingDialog.StartupFollowers = this.cfgCommon.StartupFollowers;
            this.settingDialog.RestrictFavCheck = this.cfgCommon.RestrictFavCheck;
            this.settingDialog.AlwaysTop = this.cfgCommon.AlwaysTop;
            this.settingDialog.UrlConvertAuto = false;
            this.settingDialog.OutputzEnabled = this.cfgCommon.Outputz;
            this.settingDialog.OutputzKey = this.cfgCommon.OutputzKey;
            this.settingDialog.OutputzUrlmode = this.cfgCommon.OutputzUrlMode;
            this.settingDialog.UseUnreadStyle = this.cfgCommon.UseUnreadStyle;
            this.settingDialog.DefaultTimeOut = this.cfgCommon.DefaultTimeOut;
            this.settingDialog.RetweetNoConfirm = this.cfgCommon.RetweetNoConfirm;
            this.settingDialog.PlaySound = this.cfgCommon.PlaySound;
            this.settingDialog.DateTimeFormat = this.cfgCommon.DateTimeFormat;
            this.settingDialog.LimitBalloon = this.cfgCommon.LimitBalloon;
            this.settingDialog.EventNotifyEnabled = this.cfgCommon.EventNotifyEnabled;
            this.settingDialog.EventNotifyFlag = this.cfgCommon.EventNotifyFlag;
            this.settingDialog.IsMyEventNotifyFlag = this.cfgCommon.IsMyEventNotifyFlag;
            this.settingDialog.ForceEventNotify = this.cfgCommon.ForceEventNotify;
            this.settingDialog.FavEventUnread = this.cfgCommon.FavEventUnread;
            this.settingDialog.TranslateLanguage = this.cfgCommon.TranslateLanguage;
            this.settingDialog.EventSoundFile = this.cfgCommon.EventSoundFile;

            // 廃止サービスが選択されていた場合bit.lyへ読み替え
            if (this.cfgCommon.AutoShortUrlFirst < 0)
            {
                this.cfgCommon.AutoShortUrlFirst = UrlConverter.Bitly;
            }

            this.settingDialog.AutoShortUrlFirst = this.cfgCommon.AutoShortUrlFirst;
            this.settingDialog.TabIconDisp = this.cfgCommon.TabIconDisp;
            this.settingDialog.ReplyIconState = this.cfgCommon.ReplyIconState;
            this.settingDialog.ReadOwnPost = this.cfgCommon.ReadOwnPost;
            this.settingDialog.GetFav = this.cfgCommon.GetFav;
            this.settingDialog.ReadOldPosts = this.cfgCommon.ReadOldPosts;
            this.settingDialog.UseSsl = this.cfgCommon.UseSsl;
            this.settingDialog.BitlyUser = this.cfgCommon.BilyUser;
            this.settingDialog.BitlyPwd = this.cfgCommon.BitlyPwd;
            this.settingDialog.ShowGrid = this.cfgCommon.ShowGrid;
            this.settingDialog.Language = this.cfgCommon.Language;
            this.settingDialog.UseAtIdSupplement = this.cfgCommon.UseAtIdSupplement;
            this.settingDialog.UseHashSupplement = this.cfgCommon.UseHashSupplement;
            this.settingDialog.PreviewEnable = this.cfgCommon.PreviewEnable;
            this.AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            this.settingDialog.IsMonospace = this.cfgCommon.IsMonospace;
            if (this.settingDialog.IsMonospace)
            {
                this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
            }
            else
            {
                this.detailHtmlFormatHeader = DetailHtmlFormat1;
                this.detailHtmlFormatFooter = DetailHtmlFormat7;
            }

            this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
            if (this.settingDialog.IsMonospace)
            {
                this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
            }
            else
            {
                this.detailHtmlFormatHeader += DetailHtmlFormat6;
            }

            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = this.cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = this.cfgCommon.FocusLockToStatusText;

            this.settingDialog.RecommendStatusText = " [TWNv" + Regex.Replace(MyCommon.FileVersion.Replace(".", string.Empty), "^0*", string.Empty) + "]";

            // 書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(this.settingDialog.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    this.settingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                this.settingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            this.settingDialog.Nicoms = this.cfgCommon.Nicoms;
            this.settingDialog.HotkeyEnabled = this.cfgCommon.HotkeyEnabled;
            this.settingDialog.HotkeyMod = this.cfgCommon.HotkeyModifier;
            this.settingDialog.HotkeyKey = this.cfgCommon.HotkeyKey;
            this.settingDialog.HotkeyValue = this.cfgCommon.HotkeyValue;
            this.settingDialog.BlinkNewMentions = this.cfgCommon.BlinkNewMentions;
            this.settingDialog.UseAdditionalCount = this.cfgCommon.UseAdditionalCount;
            this.settingDialog.MoreCountApi = this.cfgCommon.MoreCountApi;
            this.settingDialog.FirstCountApi = this.cfgCommon.FirstCountApi;
            this.settingDialog.SearchCountApi = this.cfgCommon.SearchCountApi;
            this.settingDialog.FavoritesCountApi = this.cfgCommon.FavoritesCountApi;
            this.settingDialog.UserTimelineCountApi = this.cfgCommon.UserTimelineCountApi;
            this.settingDialog.ListCountApi = this.cfgCommon.ListCountApi;
            this.settingDialog.UserstreamStartup = this.cfgCommon.UserstreamStartup;
            this.settingDialog.UserstreamPeriodInt = this.cfgCommon.UserstreamPeriod;
            this.settingDialog.OpenUserTimeline = this.cfgCommon.OpenUserTimeline;
            this.settingDialog.ListDoubleClickAction = this.cfgCommon.ListDoubleClickAction;
            this.settingDialog.UserAppointUrl = this.cfgCommon.UserAppointUrl;
            this.settingDialog.HideDuplicatedRetweets = this.cfgCommon.HideDuplicatedRetweets;
            this.settingDialog.IsPreviewFoursquare = this.cfgCommon.IsPreviewFoursquare;
            this.settingDialog.FoursquarePreviewHeight = this.cfgCommon.FoursquarePreviewHeight;
            this.settingDialog.FoursquarePreviewWidth = this.cfgCommon.FoursquarePreviewWidth;
            this.settingDialog.FoursquarePreviewZoom = this.cfgCommon.FoursquarePreviewZoom;
            this.settingDialog.IsListStatusesIncludeRts = this.cfgCommon.IsListsIncludeRts;
            this.settingDialog.TabMouseLock = this.cfgCommon.TabMouseLock;
            this.settingDialog.IsRemoveSameEvent = this.cfgCommon.IsRemoveSameEvent;
            this.settingDialog.IsNotifyUseGrowl = this.cfgCommon.IsUseNotifyGrowl;

            // ハッシュタグ関連
            this.HashSupl = new AtIdSupplement(this.cfgCommon.HashTags, "#");
            this.HashMgr = new HashtagManage(this.HashSupl, this.cfgCommon.HashTags.ToArray(), this.cfgCommon.HashSelected, this.cfgCommon.HashIsPermanent, this.cfgCommon.HashIsHead, this.cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.HashMgr.IsPermanent)
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
            }

            this.isInitializing = true;

            // アイコンリスト作成
            try
            {
                this.iconDict = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }

            ((ImageDictionary)this.iconDict).PauseGetImage = false;

            bool saveRequired = false;

            // ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(this.tw.Username))
            {
                saveRequired = true;

                // 設定せずにキャンセルされた場合はプログラム終了
                if (this.settingDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // 設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
                if (string.IsNullOrEmpty(this.tw.Username))
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // フォント＆文字色＆背景色保持
                this.fntUnread = this.settingDialog.FontUnread;
                this.clrUnread = this.settingDialog.ColorUnread;
                this.fntReaded = this.settingDialog.FontReaded;
                this.clrRead = this.settingDialog.ColorReaded;
                this.clrFav = this.settingDialog.ColorFav;
                this.clrOWL = this.settingDialog.ColorOWL;
                this.clrRetweet = this.settingDialog.ColorRetweet;
                this.fntDetail = this.settingDialog.FontDetail;
                this.clrDetail = this.settingDialog.ColorDetail;
                this.clrDetailLink = this.settingDialog.ColorDetailLink;
                this.clrDetailBackcolor = this.settingDialog.ColorDetailBackcolor;
                this.clrSelf = this.settingDialog.ColorSelf;
                this.clrAtSelf = this.settingDialog.ColorAtSelf;
                this.clrTarget = this.settingDialog.ColorTarget;
                this.clrAtTarget = this.settingDialog.ColorAtTarget;
                this.clrAtFromTarget = this.settingDialog.ColorAtFromTarget;
                this.clrAtTo = this.settingDialog.ColorAtTo;
                this.clrListBackcolor = this.settingDialog.ColorListBackcolor;
                this.InputBackColor = this.settingDialog.ColorInputBackcolor;
                this.clrInputForecolor = this.settingDialog.ColorInputFont;
                this.fntInputFont = this.settingDialog.FontInputFont;
                this.brsForeColorUnread.Dispose();
                this.brsForeColorReaded.Dispose();
                this.brsForeColorFav.Dispose();
                this.brsForeColorOWL.Dispose();
                this.brsForeColorRetweet.Dispose();
                this.brsForeColorUnread = new SolidBrush(this.clrUnread);
                this.brsForeColorReaded = new SolidBrush(this.clrRead);
                this.brsForeColorFav = new SolidBrush(this.clrFav);
                this.brsForeColorOWL = new SolidBrush(this.clrOWL);
                this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                this.brsBackColorMine.Dispose();
                this.brsBackColorAt.Dispose();
                this.brsBackColorYou.Dispose();
                this.brsBackColorAtYou.Dispose();
                this.brsBackColorAtFromTarget.Dispose();
                this.brsBackColorAtTo.Dispose();
                this.brsBackColorNone.Dispose();
                this.brsBackColorMine = new SolidBrush(this.clrSelf);
                this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
                this.brsBackColorYou = new SolidBrush(this.clrTarget);
                this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);

                if (this.settingDialog.IsMonospace)
                {
                    this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                    this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
                }
                else
                {
                    this.detailHtmlFormatHeader = DetailHtmlFormat1;
                    this.detailHtmlFormatFooter = DetailHtmlFormat7;
                }

                this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                if (this.settingDialog.IsMonospace)
                {
                    this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
                }
                else
                {
                    this.detailHtmlFormatHeader += DetailHtmlFormat6;
                }
            }

            if (this.settingDialog.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((this.settingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }

                if ((this.settingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }

                if ((this.settingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }

                if ((this.settingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                this.hookGlobalHotkey.RegisterOriginalHotkey(this.settingDialog.HotkeyKey, this.settingDialog.HotkeyValue, modKey);
            }

            // Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(this.settingDialog.DefaultTimeOut, this.settingDialog.SelectedProxyType, this.settingDialog.ProxyAddress, this.settingDialog.ProxyPort, this.settingDialog.ProxyUser, this.settingDialog.ProxyPassword);

            this.tw.SetRestrictFavCheck(this.settingDialog.RestrictFavCheck);
            this.tw.ReadOwnPost = this.settingDialog.ReadOwnPost;
            this.tw.SetUseSsl(this.settingDialog.UseSsl);
            ShortUrl.IsResolve = this.settingDialog.TinyUrlResolve;
            ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(this.settingDialog.BitlyUser);
            ShortUrl.SetBitlyKey(this.settingDialog.BitlyPwd);
            HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);
            this.tw.TrackWord = this.cfgCommon.TrackWord;
            this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.tw.TrackWord);
            this.tw.AllAtReply = this.cfgCommon.AllAtReply;
            this.AllrepliesToolStripMenuItem.Checked = this.tw.AllAtReply;

            Outputz.Key = this.settingDialog.OutputzKey;
            Outputz.Enabled = this.settingDialog.OutputzEnabled;
            switch (this.settingDialog.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    Outputz.OutUrl = "http://twitter.com/";
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    Outputz.OutUrl = "http://twitter.com/" + this.tw.Username;
                    break;
            }

            // 画像投稿サービス
            this.CreatePictureServices();
            this.SetImageServiceCombo();
            this.ImageSelectionPanel.Enabled = false;
            this.ImageServiceCombo.SelectedIndex = this.cfgCommon.UseImageService;

            // ウィンドウ設定
            this.ClientSize = this.cfgLocal.FormSize;
            this.mySize = this.cfgLocal.FormSize;          // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            this.myLoc = this.cfgLocal.FormLocation;       // タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = this.cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(this.mySize.Width, SystemInformation.CaptionHeight));
                bool outOfScreen = true;
                if (Screen.AllScreens.Length == 1)
                {
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (!Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty)
                        {
                            outOfScreen = false;
                            break;
                        }
                    }

                    if (outOfScreen)
                    {
                        this.DesktopLocation = new Point(0, 0);
                        this.myLoc = this.DesktopLocation;
                    }
                }
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            this.mySpDis = this.cfgLocal.SplitterDistance;
            this.mySpDis2 = this.cfgLocal.StatusTextHeight;
            this.mySpDis3 = this.cfgLocal.PreviewDistance;
            if (this.mySpDis3 == -1)
            {
                this.mySpDis3 = this.mySize.Width - 150;
                if (this.mySpDis3 < 1)
                {
                    this.mySpDis3 = 50;
                }

                this.cfgLocal.PreviewDistance = this.mySpDis3;
            }

            this.myAdSpDis = this.cfgLocal.AdSplitterDistance;
            this.MultiLineMenuItem.Checked = this.cfgLocal.StatusMultiline;
            this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;

            // 入力欄
            this.StatusText.Font = this.fntInputFont;
            this.StatusText.ForeColor = this.clrInputForecolor;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this.settingDialog.UnreadManage == false)
            {
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
            }

            if (this.settingDialog.IsNotifyUseGrowl)
            {
                this.growlHelper.RegisterGrowl();
            }

            // タイマー設定
            this.timerTimeline.AutoReset = true;
            this.timerTimeline.SynchronizingObject = this;

            // Recent取得間隔
            this.timerTimeline.Interval = 1000;
            this.timerTimeline.Enabled = true;

            // 更新中アイコンアニメーション間隔
            this.TimerRefreshIcon.Interval = 200;
            this.TimerRefreshIcon.Enabled = true;

            // 状態表示部の初期化（画面右下）
            this.StatusLabel.Text = string.Empty;
            this.StatusLabel.AutoToolTip = false;
            this.StatusLabel.ToolTipText = string.Empty;

            // 文字カウンタ初期化
            this.lblLen.Text = this.GetRestStatusCount(true, false).ToString();

            this.statuses.SortOrder = (SortOrder)this.cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (this.cfgCommon.SortColumn)
            {
                case 0:
                case 5:
                case 6:
                    // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                    // ソートしない Idソートに読み替え
                    mode = IdComparerClass.ComparerMode.Id;
                    break;
                case 1:
                    // ニックネーム
                    mode = IdComparerClass.ComparerMode.Nickname;
                    break;
                case 2:
                    // 本文
                    mode = IdComparerClass.ComparerMode.Data;
                    break;
                case 3:
                    // 時刻=発言Id
                    mode = IdComparerClass.ComparerMode.Id;
                    break;
                case 4:
                    // 名前
                    mode = IdComparerClass.ComparerMode.Name;
                    break;
                case 7:
                    // Source
                    mode = IdComparerClass.ComparerMode.Source;
                    break;
            }

            this.statuses.SortMode = mode;

            switch (this.settingDialog.IconSz)
            {
                case IconSizes.IconNone:
                    this.iconSz = 0;
                    break;
                case IconSizes.Icon16:
                    this.iconSz = 16;
                    break;
                case IconSizes.Icon24:
                    this.iconSz = 26;
                    break;
                case IconSizes.Icon48:
                    this.iconSz = 48;
                    break;
                case IconSizes.Icon48_2:
                    this.iconSz = 48;
                    this.iconCol = true;
                    break;
            }

            if (this.iconSz == 0)
            {
                this.tw.SetGetIcon(false);
            }
            else
            {
                this.tw.SetGetIcon(true);
                this.tw.SetIconSize(this.iconSz);
            }

            this.tw.SetTinyUrlResolve(this.settingDialog.TinyUrlResolve);
            ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;

            // 発言詳細部アイコンをリストアイコンにサイズ変更
            int sz = this.iconSz;
            if (this.iconSz == 0)
            {
                sz = 16;
            }

            this.tw.DetailIcon = this.iconDict;

            this.StatusLabel.Text = Hoehoe.Properties.Resources.Form1_LoadText1;  // 画面右下の状態表示を変更
            this.StatusLabelUrl.Text = string.Empty;  // 画面左下のリンク先URL表示部を初期化
            this.NameLabel.Text = string.Empty;       // 発言詳細部名前ラベル初期化
            this.DateTimeLabel.Text = string.Empty;   // 発言詳細部日時ラベル初期化
            this.SourceLinkLabel.Text = string.Empty; // Source部分初期化

            // <<<<<<<<タブ関連>>>>>>>
            // デフォルトタブの存在チェック、ない場合には追加
            if (this.statuses.GetTabByType(TabUsageType.Home) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.RECENT))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.RECENT].TabType = TabUsageType.Home;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.Mentions) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.REPLY))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.REPLY].TabType = TabUsageType.Mentions;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.DirectMessage) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.DM))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.DM].TabType = TabUsageType.DirectMessage;
                }
            }

            if (this.statuses.GetTabByType(TabUsageType.Favorites) == null)
            {
                if (!this.statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.FAV))
                {
                    this.statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
                }
                else
                {
                    this.statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.FAV].TabType = TabUsageType.Favorites;
                }
            }

            foreach (string tn in this.statuses.Tabs.Keys)
            {
                if (this.statuses.Tabs[tn].TabType == TabUsageType.Undefined)
                {
                    this.statuses.Tabs[tn].TabType = TabUsageType.UserDefined;
                }

                if (!this.AddNewTab(tn, true, this.statuses.Tabs[tn].TabType, this.statuses.Tabs[tn].ListInfo))
                {
                    throw new Exception(Hoehoe.Properties.Resources.TweenMain_LoadText1);
                }
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            this.CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            this.CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (this.settingDialog.MinimizeToTray == false || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }

            this.curTab = this.ListTab.SelectedTab;
            this.curItemIndex = -1;
            this.curList = (DetailsListView)this.curTab.Tag;
            this.SetMainWindowTitle();
            this.SetNotifyIconText();

            if (this.settingDialog.TabIconDisp)
            {
                this.ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                this.ListTab.DrawItem += this.ListTab_DrawItem;
                this.ListTab.ImageList = null;
            }

#if UA // = "True"
			ab = new AdsBrowser();
			this.SplitContainer4.Panel2.Controls.Add(ab);
#else
            this.SplitContainer4.Panel2Collapsed = true;
#endif

            this.ignoreConfigSave = false;
            this.TweenMain_Resize(null, null);
            if (saveRequired)
            {
                this.SaveConfigsAll(false);
            }

            if (this.tw.UserId == 0)
            {
                this.tw.VerifyCredentials();
                foreach (var ua in this.cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == this.tw.Username.ToLower())
                    {
                        ua.UserId = this.tw.UserId;
                        break;
                    }
                }
            }

            foreach (var ua in this.settingDialog.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == this.tw.Username.ToLower())
                {
                    ua.UserId = this.tw.UserId;
                    break;
                }
            }
        }

        private void SpaceKeyCanceler_SpaceCancel(object sender, EventArgs e)
        {
            this.JumpUnreadMenuItem_Click(null, null);
        }

        private void ListTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            string txt = null;
            try
            {
                txt = this.ListTab.TabPages[e.Index].Text;
            }
            catch (Exception)
            {
                return;
            }

            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            if (e.State == DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }

            Brush fore = null;
            try
            {
                if (this.statuses.Tabs[txt].UnreadCount > 0)
                {
                    fore = Brushes.Red;
                }
                else
                {
                    fore = SystemBrushes.ControlText;
                }
            }
            catch (Exception)
            {
                fore = SystemBrushes.ControlText;
            }

            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, this.tabStringFormat);
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e)
        {
            if (!this.timerTimeline.Enabled)
            {
                return;
            }

            this.resetTimers = e;
        }

        private void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            if (this.timerHomeCounter > 0)
            {
                Interlocked.Decrement(ref this.timerHomeCounter);
            }

            if (this.timerMentionCounter > 0)
            {
                Interlocked.Decrement(ref this.timerMentionCounter);
            }

            if (this.timerDmCounter > 0)
            {
                Interlocked.Decrement(ref this.timerDmCounter);
            }

            if (this.timerPubSearchCounter > 0)
            {
                Interlocked.Decrement(ref this.timerPubSearchCounter);
            }

            if (this.timerUserTimelineCounter > 0)
            {
                Interlocked.Decrement(ref this.timerUserTimelineCounter);
            }

            if (this.timerListsCounter > 0)
            {
                Interlocked.Decrement(ref this.timerListsCounter);
            }

            if (this.timerUsCounter > 0)
            {
                Interlocked.Decrement(ref this.timerUsCounter);
            }

            Interlocked.Increment(ref this.timerRefreshFollowers);

            // 'タイマー初期化
            if (this.resetTimers.Timeline || (this.timerHomeCounter <= 0 && this.settingDialog.TimelinePeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerHomeCounter, this.settingDialog.TimelinePeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.Timeline)
                {
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                }

                this.resetTimers.Timeline = false;
            }

            if (this.resetTimers.Reply || (this.timerMentionCounter <= 0 && this.settingDialog.ReplyPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerMentionCounter, this.settingDialog.ReplyPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.Reply)
                {
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                }

                this.resetTimers.Reply = false;
            }

            if (this.resetTimers.DirectMessage || (this.timerDmCounter <= 0 && this.settingDialog.DMPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerDmCounter, this.settingDialog.DMPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.resetTimers.DirectMessage)
                {
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                }

                this.resetTimers.DirectMessage = false;
            }

            if (this.resetTimers.PublicSearch || (this.timerPubSearchCounter <= 0 && this.settingDialog.PubSearchPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerPubSearchCounter, this.settingDialog.PubSearchPeriodInt);
                if (!this.resetTimers.PublicSearch)
                {
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                }

                this.resetTimers.PublicSearch = false;
            }

            if (this.resetTimers.UserTimeline || (this.timerUserTimelineCounter <= 0 && this.settingDialog.UserTimelinePeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerUserTimelineCounter, this.settingDialog.UserTimelinePeriodInt);
                if (!this.resetTimers.UserTimeline)
                {
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                }

                this.resetTimers.UserTimeline = false;
            }

            if (this.resetTimers.Lists || (this.timerListsCounter <= 0 && this.settingDialog.ListsPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerListsCounter, this.settingDialog.ListsPeriodInt);
                if (!this.resetTimers.Lists)
                {
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                }

                this.resetTimers.Lists = false;
            }

            if (this.resetTimers.UserStream || (this.timerUsCounter <= 0 && this.settingDialog.UserstreamPeriodInt > 0))
            {
                Interlocked.Exchange(ref this.timerUsCounter, this.settingDialog.UserstreamPeriodInt);
                if (this.isActiveUserstream)
                {
                    this.RefreshTimeline(true);
                }

                this.resetTimers.UserStream = false;
            }

            if (this.timerRefreshFollowers > 6 * 3600)
            {
                Interlocked.Exchange(ref this.timerRefreshFollowers, 0);
                this.DoGetFollowersMenu();
                this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }

            if (this.isOsResumed)
            {
                Interlocked.Increment(ref this.timerResumeWait);
                if (this.timerResumeWait > 30)
                {
                    this.isOsResumed = false;
                    Interlocked.Exchange(ref this.timerResumeWait, 0);
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                    this.DoGetFollowersMenu();
                    this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                    if (InvokeRequired && !IsDisposed)
                    {
                        this.Invoke(new MethodInvoker(this.TrimPostChain));
                    }
                }
            }
        }

        private void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.curList == null || this.curList.SelectedIndices.Count != 1)
            {
                return;
            }

            this.curItemIndex = this.curList.SelectedIndices[0];
            if (this.curItemIndex > this.curList.VirtualListSize - 1)
            {
                return;
            }

            try
            {
                this.curPost = this.GetCurTabPost(this.curItemIndex);
            }
            catch (ArgumentException)
            {
                return;
            }

            this.PushSelectPostChain();

            if (this.settingDialog.UnreadManage)
            {
                this.statuses.SetReadAllTab(true, this.curTab.Text, this.curItemIndex);
            }

            // キャッシュの書き換え
            this.ChangeCacheStyleRead(true, this.curItemIndex, this.curTab);

            // 既読へ（フォント、文字色）
            this.ColorizeList();
            this.colorize = true;
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            if (this.StatusText.Text.Trim().Length == 0)
            {
                if (!this.ImageSelectionPanel.Enabled)
                {
                    this.DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && this.StatusText.Text.Trim() == string.Format("RT @{0}: {1}", this.curPost.ScreenName, this.curPost.TextFromApi))
            {
                DialogResult res = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.PostButton_Click1, Environment.NewLine), "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        this.DoReTweetOfficial(false);
                        this.StatusText.Text = string.Empty;
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            this.postHistory[this.postHistory.Count - 1] = new PostingStatus(this.StatusText.Text.Trim(), this.replyToId, this.replyToName);

            if (this.settingDialog.Nicoms)
            {
                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                this.UrlConvert(UrlConverter.Nicoms);
            }

            this.StatusText.SelectionStart = this.StatusText.Text.Length;
            GetWorkerArg args = new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = WorkerType.PostMessage };
            this.CheckReplyTo(this.StatusText.Text);

            // 整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = this.StatusText.Text.Trim();
            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !tmpStatus.EndsWith(" ."))
                {
                    adjustCount += 2;
                }
            }

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                adjustCount += Regex.Matches(tmpStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+").Count;
            }

            bool isCutOff = false;
            bool isRemoveFooter = this.IsKeyDown(Keys.Shift);
            if (this.StatusText.Multiline && !this.settingDialog.PostCtrlEnter)
            {
                // 複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = this.IsKeyDown(Keys.Control);
            }

            if (this.settingDialog.PostShiftEnter)
            {
                isRemoveFooter = this.IsKeyDown(Keys.Control);
            }

            if (!isRemoveFooter && (this.StatusText.Text.Contains("RT @") || this.StatusText.Text.Contains("QT @")))
            {
                isRemoveFooter = true;
            }

            if (this.GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
            {
                if (MessageBox.Show(Hoehoe.Properties.Resources.PostLengthOverMessage1, Hoehoe.Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    isCutOff = true;
                    if (this.GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
                    {
                        isRemoveFooter = true;
                    }
                }
                else
                {
                    return;
                }
            }

            string footer = string.Empty;
            string header = string.Empty;
            if (this.StatusText.Text.StartsWith("D ") || this.StatusText.Text.StartsWith("d "))
            {
                // DM時は何もつけない
                footer = string.Empty;
            }
            else
            {
                // ハッシュタグ
                if (this.HashMgr.IsNotAddToAtReply)
                {
                    if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.replyToId == 0 && string.IsNullOrEmpty(this.replyToName))
                    {
                        if (this.HashMgr.IsHead)
                        {
                            header = this.HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + this.HashMgr.UseHash;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
                    {
                        if (this.HashMgr.IsHead)
                        {
                            header = this.HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + this.HashMgr.UseHash;
                        }
                    }
                }

                if (!isRemoveFooter)
                {
                    if (this.settingDialog.UseRecommendStatus)
                    {
                        // 推奨ステータスを使用する
                        footer += this.settingDialog.RecommendStatusText;
                    }
                    else
                    {
                        // テキストボックスに入力されている文字列を使用する
                        footer += " " + this.settingDialog.Status.Trim();
                    }
                }
            }

            args.PStatus.Status = header + this.StatusText.Text.Trim() + footer;

            if (this.ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(args.PStatus.Status, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !args.PStatus.Status.EndsWith(" ."))
                {
                    args.PStatus.Status += " .";
                }
            }

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+");
                if (mc2.Success)
                {
                    args.PStatus.Status = Regex.Replace(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+", "$& ");
                }
            }

            if (this.IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                args.PStatus.Status = args.PStatus.Status.Replace("　", " ");
            }

            if (isCutOff && args.PStatus.Status.Length > 140)
            {
                args.PStatus.Status = args.PStatus.Status.Substring(0, 140);
                const string AtId = "(@|＠)[a-z0-9_/]+$";
                const string HashTag = "(^|[^0-9A-Z&\\/\\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$";
                const string Url = "https?:\\/\\/[a-z0-9!\\*'\\(\\);:&=\\+\\$\\/%#\\[\\]\\-_\\.,~?]+$";

                // 簡易判定
                string pattern = string.Format("({0})|({1})|({2})", AtId, HashTag, Url);
                Match mc = Regex.Match(args.PStatus.Status, pattern, RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    // さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                    args.PStatus.Status = args.PStatus.Status.Substring(0, 140 - mc.Value.Length);
                }

                if (MessageBox.Show(args.PStatus.Status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            args.PStatus.InReplyToId = this.replyToId;
            args.PStatus.InReplyToName = this.replyToName;
            if (this.ImageSelectionPanel.Visible)
            {
                // 画像投稿
                if (!object.ReferenceEquals(this.ImageSelectedPicture.Image, this.ImageSelectedPicture.InitialImage) && this.ImageServiceCombo.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ImagefilePathText.Text))
                {
                    if (MessageBox.Show(Hoehoe.Properties.Resources.PostPictureConfirm1, Hoehoe.Properties.Resources.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        this.TimelinePanel.Visible = true;
                        this.TimelinePanel.Enabled = true;
                        this.ImageSelectionPanel.Visible = false;
                        this.ImageSelectionPanel.Enabled = false;
                        if (this.curList != null)
                        {
                            this.curList.Focus();
                        }

                        return;
                    }

                    args.PStatus.ImageService = this.ImageServiceCombo.Text;
                    args.PStatus.ImagePath = this.ImagefilePathText.Text;
                    this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                    this.ImagefilePathText.Text = string.Empty;
                    this.TimelinePanel.Visible = true;
                    this.TimelinePanel.Enabled = true;
                    this.ImageSelectionPanel.Visible = false;
                    this.ImageSelectionPanel.Enabled = false;
                    if (this.curList != null)
                    {
                        this.curList.Focus();
                    }
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.PostPictureWarn1, Hoehoe.Properties.Resources.PostPictureWarn2);
                    return;
                }
            }

            this.RunAsync(args);

            // Google検索（試験実装）
            if (this.StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && this.StatusText.Text.Trim().Length > 7)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.SearchItem2Url, HttpUtility.UrlEncode(this.StatusText.Text.Substring(7)));
                this.OpenUriAsync(tmp);
            }

            this.replyToId = 0;
            this.replyToName = string.Empty;
            this.StatusText.Text = string.Empty;
            this.postHistory.Add(new PostingStatus());
            this.postHistoryIndex = this.postHistory.Count - 1;
            if (!this.ToolStripFocusLockMenuItem.Checked)
            {
                ((Control)this.ListTab.SelectedTab.Tag).Focus();
            }

            this.urlUndoBuffer = null;
            this.UrlUndoToolStripMenuItem.Enabled = false; // Undoをできないように設定
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.IsEnding = true;
            this.Close();
        }

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.settingDialog.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon.IsEnding == false)
            {
                // _endingFlag=False:フォームの×ボタン
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                // Google.GASender.GetInstance().TrackEventWithCategory("post", "end", this.Tw.UserId_)
                this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                this.ignoreConfigSave = true;
                MyCommon.IsEnding = true;
                this.timerTimeline.Enabled = false;
                this.TimerRefreshIcon.Enabled = false;
            }
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
            this.BringToFront();
        }

        private void GetTimelineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            if (bw.CancellationPending || MyCommon.IsEnding)
            {
                e.Cancel = true;
                return;
            }

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            //// Tween.My.MyProject.Application.InitCulture(); // TODO: Need this here?
            string ret = string.Empty;
            GetWorkerResult rslt = new GetWorkerResult();
            bool read = !this.settingDialog.UnreadManage;
            if (this.isInitializing && this.settingDialog.UnreadManage)
            {
                read = this.settingDialog.Readed;
            }

            GetWorkerArg args = (GetWorkerArg)e.Argument;
            if (!CheckAccountValid())
            {
                // エラー表示のみ行ない、後処理キャンセル
                rslt.RetMsg = "Auth error. Check your account";
                rslt.WorkerType = WorkerType.ErrorState;
                rslt.TabName = args.TabName;
                e.Result = rslt;
                return;
            }

            if (args.WorkerType != WorkerType.OpenUri)
            {
                bw.ReportProgress(0, string.Empty);
            }

            // Notifyアイコンアニメーション開始
            switch (args.WorkerType)
            {
                case WorkerType.Timeline:
                case WorkerType.Reply:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, this.isInitializing);
                    if (string.IsNullOrEmpty(ret) && args.WorkerType == WorkerType.Timeline && this.settingDialog.ReadOldPosts)
                    {
                        // 新着時未読クリア
                        this.statuses.SetRead();
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.DirectMessegeRcv:
                    // 送信分もまとめて取得
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeSnt, args.Page == -1);
                    }

                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.FavAdd:
                    // スレッド処理はしない
                    if (this.statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this.statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = null;
                            if (tbc.IsInnerStorageTabType)
                            {
                                post = tbc.Posts[args.Ids[i]];
                            }
                            else
                            {
                                post = this.statuses.Item(args.Ids[i]);
                            }

                            args.Page = i + 1;
                            bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                            if (!post.IsFav)
                            {
                                if (post.RetweetedId == 0)
                                {
                                    ret = this.tw.PostFavAdd(post.StatusId);
                                }
                                else
                                {
                                    ret = this.tw.PostFavAdd(post.RetweetedId);
                                }

                                if (ret.Length == 0)
                                {
                                    // リスト再描画必要
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
                                    this.favTimestamps.Add(DateTime.Now);
                                    if (string.IsNullOrEmpty(post.RelTabName))
                                    {
                                        // 検索,リストUserTimeline.Relatedタブからのfavは、favタブへ追加せず。それ以外は追加
                                        this.statuses.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                    }
                                    else
                                    {
                                        // 検索,リスト,UserTimeline.Relatedタブからのfavで、TLでも取得済みならfav反映
                                        if (this.statuses.ContainsKey(post.StatusId))
                                        {
                                            PostClass postTl = this.statuses.Item(post.StatusId);
                                            postTl.IsFav = true;
                                            this.statuses.GetTabByType(TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, false);
                                        }
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
                                    {
                                        if (tb.Contains(post.StatusId))
                                        {
                                            tb.Posts[post.StatusId].IsFav = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    rslt.SIds = args.SIds;
                    break;
                case WorkerType.FavRemove:
                    // スレッド処理はしない
                    if (this.statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this.statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = tbc.IsInnerStorageTabType ? tbc.Posts[args.Ids[i]] : this.statuses.Item(args.Ids[i]);
                            args.Page = i + 1;
                            bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                            if (post.IsFav)
                            {
                                ret = post.RetweetedId == 0 ? this.tw.PostFavRemove(post.StatusId) : this.tw.PostFavRemove(post.RetweetedId);
                                if (ret.Length == 0)
                                {
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = false;

                                    // リスト再描画必要
                                    if (this.statuses.ContainsKey(post.StatusId))
                                    {
                                        this.statuses.Item(post.StatusId).IsFav = false;
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
                                    {
                                        if (tb.Contains(post.StatusId))
                                        {
                                            tb.Posts[post.StatusId].IsFav = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    rslt.SIds = args.SIds;
                    break;
                case WorkerType.PostMessage:
                    bw.ReportProgress(200);
                    if (string.IsNullOrEmpty(args.PStatus.ImagePath))
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            ret = this.tw.PostStatus(args.PStatus.Status, args.PStatus.InReplyToId);
                            if (string.IsNullOrEmpty(ret) || ret.StartsWith("OK:") || ret.StartsWith("Outputz:") || ret.StartsWith("Warn:") || ret == "Err:Status is a duplicate." || args.PStatus.Status.StartsWith("D", StringComparison.OrdinalIgnoreCase) || args.PStatus.Status.StartsWith("DM", StringComparison.OrdinalIgnoreCase) || Twitter.AccountState != AccountState.Valid)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        ret = this.pictureServices[args.PStatus.ImageService].Upload(ref args.PStatus.ImagePath, ref args.PStatus.Status, args.PStatus.InReplyToId);
                    }

                    bw.ReportProgress(300);
                    rslt.PStatus = args.PStatus;
                    break;
                case WorkerType.Retweet:
                    bw.ReportProgress(200);
                    for (int i = 0; i < args.Ids.Count; i++)
                    {
                        ret = this.tw.PostRetweet(args.Ids[i], read);
                    }

                    bw.ReportProgress(300);
                    break;
                case WorkerType.Follower:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText1);
                    ret = this.tw.GetFollowersApi();
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this.tw.GetNoRetweetIdsApi();
                    }

                    break;
                case WorkerType.Configuration:
                    ret = this.tw.ConfigurationApi();
                    break;
                case WorkerType.OpenUri:
                    string myPath = Convert.ToString(args.Url);
                    try
                    {
                        if (!string.IsNullOrEmpty(this.settingDialog.BrowserPath))
                        {
                            if (this.settingDialog.BrowserPath.StartsWith("\"") && this.settingDialog.BrowserPath.Length > 2 && this.settingDialog.BrowserPath.IndexOf("\"", 2) > -1)
                            {
                                int sep = this.settingDialog.BrowserPath.IndexOf("\"", 2);
                                string browserPath = this.settingDialog.BrowserPath.Substring(1, sep - 1);
                                string arg = string.Empty;
                                if (sep < this.settingDialog.BrowserPath.Length - 1)
                                {
                                    arg = this.settingDialog.BrowserPath.Substring(sep + 1);
                                }

                                myPath = arg + " " + myPath;
                                Process.Start(browserPath, myPath);
                            }
                            else
                            {
                                Process.Start(this.settingDialog.BrowserPath, myPath);
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

                    break;
                case WorkerType.Favorites:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetFavoritesApi(read, args.WorkerType, args.Page == -1);
                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.PublicSearch:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.PublicSearch))
                        {
                            if (!string.IsNullOrEmpty(tb.SearchWords))
                            {
                                ret = this.tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetSearch(read, tb, false);
                            if (string.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = this.tw.GetSearch(read, tb, true);
                            }
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.UserTimeline:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    int count = 20;
                    if (this.settingDialog.UseAdditionalCount)
                    {
                        count = this.settingDialog.UserTimelineCountApi;
                    }

                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.UserTimeline))
                        {
                            if (!string.IsNullOrEmpty(tb.User))
                            {
                                ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts();                     // 振り分け
                    break;
                case WorkerType.List:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        // 定期更新
                        foreach (TabClass tb in this.statuses.GetTabsByType(TabUsageType.Lists))
                        {
                            if (tb.ListInfo != null && tb.ListInfo.Id != 0)
                            {
                                ret = this.tw.GetListStatus(read, tb, false, this.isInitializing);
                            }
                        }
                    }
                    else
                    {
                        // 手動更新（特定タブのみ更新）
                        TabClass tb = this.statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetListStatus(read, tb, args.Page == -1, this.isInitializing);
                        }
                    }

                    rslt.AddCount = this.statuses.DistributePosts(); // 振り分け
                    break;
                case WorkerType.Related:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetRelatedResult(read, this.statuses.GetTabByName(args.TabName));
                    rslt.AddCount = this.statuses.DistributePosts();
                    break;
                case WorkerType.BlockIds:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateBlockUserText1);
                    ret = this.tw.GetBlockUserIds();
                    if (TabInformations.GetInstance().BlockIds.Count == 0)
                    {
                        this.tw.GetBlockUserIds();
                    }

                    break;
            }

            // キャンセル要求
            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // 時速表示用
            if (args.WorkerType == WorkerType.FavAdd)
            {
                System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                for (int i = this.favTimestamps.Count - 1; i >= 0; i += -1)
                {
                    if (this.favTimestamps[i].CompareTo(oneHour) < 0)
                    {
                        this.favTimestamps.RemoveAt(i);
                    }
                }
            }

            if (args.WorkerType == WorkerType.Timeline && !this.isInitializing)
            {
                lock (this.syncObject)
                {
                    DateTime tm = DateTime.Now;
                    if (this.timeLineTimestamps.ContainsKey(tm))
                    {
                        this.timeLineTimestamps[tm] += rslt.AddCount;
                    }
                    else
                    {
                        this.timeLineTimestamps.Add(tm, rslt.AddCount);
                    }

                    DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                    List<DateTime> keys = new List<DateTime>();
                    this.timeLineCount = 0;
                    foreach (DateTime key in this.timeLineTimestamps.Keys)
                    {
                        if (key.CompareTo(oneHour) < 0)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            this.timeLineCount += this.timeLineTimestamps[key];
                        }
                    }

                    foreach (DateTime key in keys)
                    {
                        this.timeLineTimestamps.Remove(key);
                    }

                    keys.Clear();
                }
            }

            // 終了ステータス
            if (args.WorkerType != WorkerType.OpenUri)
            {
                bw.ReportProgress(100, this.MakeStatusMessage(args, true));
            }

            // ステータス書き換え、Notifyアイコンアニメーション開始
            rslt.RetMsg = ret;
            rslt.WorkerType = args.WorkerType;
            rslt.TabName = args.TabName;
            if (args.WorkerType == WorkerType.DirectMessegeRcv || args.WorkerType == WorkerType.DirectMessegeSnt || args.WorkerType == WorkerType.Reply || args.WorkerType == WorkerType.Timeline || args.WorkerType == WorkerType.Favorites)
            {
                // 値が正しいか後でチェック。10ページ毎の継続確認
                rslt.Page = args.Page - 1;
            }

            e.Result = rslt;
        }

        private void GetTimelineWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }

            if (e.ProgressPercentage > 100)
            {
                // 発言投稿
                if (e.ProgressPercentage == 200)
                {
                    // 開始
                    this.StatusLabel.Text = "Posting...";
                }

                if (e.ProgressPercentage == 300)
                {
                    // 終了
                    this.StatusLabel.Text = Hoehoe.Properties.Resources.PostWorker_RunWorkerCompletedText4;
                }
            }
            else
            {
                string smsg = (string)e.UserState;
                if (smsg.Length > 0)
                {
                    this.StatusLabel.Text = smsg;
                }
            }
        }

        private void GetTimelineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (MyCommon.IsEnding || e.Cancelled)
            {
                // キャンセル
                return;
            }

            if (e.Error != null)
            {
                this.myStatusError = true;
                this.waitTimeline = false;
                this.waitReply = false;
                this.waitDm = false;
                this.waitFav = false;
                this.waitPubSearch = false;
                this.waitUserTimeline = false;
                this.waitLists = false;
                throw new Exception("BackgroundWorker Exception", e.Error);
            }

            GetWorkerResult rslt = (GetWorkerResult)e.Result;

            if (rslt.WorkerType == WorkerType.OpenUri)
            {
                return;
            }

            // エラー
            if (rslt.RetMsg.Length > 0)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = rslt.RetMsg;
            }

            if (rslt.WorkerType == WorkerType.ErrorState)
            {
                return;
            }

            if (rslt.WorkerType == WorkerType.FavRemove)
            {
                this.RemovePostFromFavTab(rslt.SIds.ToArray());
            }

            if (rslt.WorkerType == WorkerType.Timeline || rslt.WorkerType == WorkerType.Reply || rslt.WorkerType == WorkerType.List || rslt.WorkerType == WorkerType.PublicSearch || rslt.WorkerType == WorkerType.DirectMessegeRcv || rslt.WorkerType == WorkerType.DirectMessegeSnt || rslt.WorkerType == WorkerType.Favorites || rslt.WorkerType == WorkerType.Follower || rslt.WorkerType == WorkerType.FavAdd || rslt.WorkerType == WorkerType.FavRemove || rslt.WorkerType == WorkerType.Related || rslt.WorkerType == WorkerType.UserTimeline || rslt.WorkerType == WorkerType.BlockIds || rslt.WorkerType == WorkerType.Configuration)
            {
                // リスト反映
                this.RefreshTimeline(false);
            }

            switch (rslt.WorkerType)
            {
                case WorkerType.Timeline:
                    this.waitTimeline = false;
                    if (!this.isInitializing)
                    {
                        // 'API使用時の取得調整は別途考える（カウント調整？）
                    }

                    break;
                case WorkerType.Reply:
                    this.waitReply = false;
                    if (rslt.NewDM && !this.isInitializing)
                    {
                        this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    }

                    break;
                case WorkerType.Favorites:
                    this.waitFav = false;
                    break;
                case WorkerType.DirectMessegeRcv:
                    this.waitDm = false;
                    break;
                case WorkerType.FavAdd:
                case WorkerType.FavRemove:
                    if (this.curList != null && this.curTab != null)
                    {
                        this.curList.BeginUpdate();
                        if (rslt.WorkerType == WorkerType.FavRemove && this.statuses.Tabs[this.curTab.Text].TabType == TabUsageType.Favorites)
                        {
                            // 色変えは不要
                        }
                        else
                        {
                            for (int i = 0; i < rslt.SIds.Count; i++)
                            {
                                if (this.curTab.Text.Equals(rslt.TabName))
                                {
                                    int idx = this.statuses.Tabs[rslt.TabName].IndexOf(rslt.SIds[i]);
                                    if (idx > -1)
                                    {
                                        TabClass tb = this.statuses.Tabs[rslt.TabName];
                                        if (tb != null)
                                        {
                                            PostClass post = null;
                                            if (tb.TabType == TabUsageType.Lists || tb.TabType == TabUsageType.PublicSearch)
                                            {
                                                post = tb.Posts[rslt.SIds[i]];
                                            }
                                            else
                                            {
                                                post = this.statuses.Item(rslt.SIds[i]);
                                            }

                                            this.ChangeCacheStyleRead(post.IsRead, idx, this.curTab);
                                        }

                                        if (idx == this.curItemIndex)
                                        {
                                            // 選択アイテム再表示
                                            this.DispSelectedPost(true);
                                        }
                                    }
                                }
                            }
                        }

                        this.curList.EndUpdate();
                    }

                    break;
                case WorkerType.PostMessage:
                    if (string.IsNullOrEmpty(rslt.RetMsg) || rslt.RetMsg.StartsWith("Outputz") || rslt.RetMsg.StartsWith("OK:") || rslt.RetMsg == "Warn:Status is a duplicate.")
                    {
                        this.postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this.postTimestamps.Count - 1; i >= 0; i += -1)
                        {
                            if (this.postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this.postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!this.HashMgr.IsPermanent && !string.IsNullOrEmpty(this.HashMgr.UseHash))
                        {
                            this.HashMgr.ClearHashtag();
                            this.HashStripSplitButton.Text = "#[-]";
                            this.HashToggleMenuItem.Checked = false;
                            this.HashToggleToolStripMenuItem.Checked = false;
                        }

                        this.SetMainWindowTitle();
                        rslt.RetMsg = string.Empty;
                    }
                    else
                    {
                        DialogResult retry = default(DialogResult);
                        try
                        {
                            retry = MessageBox.Show(string.Format("{0}   --->   [ " + rslt.RetMsg + " ]" + Environment.NewLine + "\"" + rslt.PStatus.Status + "\"" + Environment.NewLine + "{1}", Hoehoe.Properties.Resources.StatusUpdateFailed1, Hoehoe.Properties.Resources.StatusUpdateFailed2), "Failed to update status", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
                        }
                        catch (Exception)
                        {
                            retry = DialogResult.Abort;
                        }

                        if (retry == DialogResult.Retry)
                        {
                            RunAsync(new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = WorkerType.PostMessage, PStatus = rslt.PStatus });
                        }
                        else
                        {
                            if (ToolStripFocusLockMenuItem.Checked)
                            {
                                // 連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                                this.StatusText_Enter(this.StatusText, new EventArgs());
                            }
                        }
                    }

                    if (rslt.RetMsg.Length == 0 && this.settingDialog.PostAndGet)
                    {
                        if (this.isActiveUserstream)
                        {
                            this.RefreshTimeline(true);
                        }
                        else
                        {
                            this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        }
                    }

                    break;
                case WorkerType.Retweet:
                    if (rslt.RetMsg.Length == 0)
                    {
                        this.postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this.postTimestamps.Count - 1; i >= 0; i--)
                        {
                            if (this.postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this.postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!this.isActiveUserstream && this.settingDialog.PostAndGet)
                        {
                            this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        }
                    }

                    break;
                case WorkerType.Follower:
                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    break;
                case WorkerType.Configuration:
                    // this._waitFollower = False
                    if (this.settingDialog.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        this.pictureServices["Twitter"].Configuration("MaxUploadFilesize", this.settingDialog.TwitterConfiguration.PhotoSizeLimit);
                    }

                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    break;
                case WorkerType.PublicSearch:
                    this.waitPubSearch = false;
                    break;
                case WorkerType.UserTimeline:
                    this.waitUserTimeline = false;
                    break;
                case WorkerType.List:
                    this.waitLists = false;
                    break;
                case WorkerType.Related:
                    {
                        TabClass tb = this.statuses.GetTabByType(TabUsageType.Related);
                        if (tb != null && tb.RelationTargetPost != null && tb.Contains(tb.RelationTargetPost.StatusId))
                        {
                            foreach (TabPage tp in this.ListTab.TabPages)
                            {
                                if (tp.Text == tb.TabName)
                                {
                                    ((DetailsListView)tp.Tag).SelectedIndices.Add(tb.IndexOf(tb.RelationTargetPost.StatusId));
                                    ((DetailsListView)tp.Tag).Items[tb.IndexOf(tb.RelationTargetPost.StatusId)].Focused = true;
                                    break;
                                }
                            }
                        }
                    }

                    break;
            }
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }

                this.Activate();
                this.BringToFront();
            }
        }

        private void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (this.settingDialog.ListDoubleClickAction)
            {
                case 0:
                    this.MakeReplyOrDirectStatus();
                    break;
                case 1:
                    this.FavoriteChange(true);
                    break;
                case 2:
                    if (this.curPost != null)
                    {
                        this.ShowUserStatus(this.curPost.ScreenName, false);
                    }

                    break;
                case 3:
                    this.ShowUserTimeline();
                    break;
                case 4:
                    this.ShowRelatedStatusesMenuItem_Click(null, null);
                    break;
                case 5:
                    this.MoveToHomeToolStripMenuItem_Click(null, null);
                    break;
                case 6:
                    this.StatusOpenMenuItem_Click(null, null);
                    break;
                case 7:
                    // 動作なし
                    break;
            }
        }

        private void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoriteChange(true);
        }

        private void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoriteChange(false);
        }

        private void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetOriginal();
        }

        private void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetUnofficial();
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName);
            }
            else if (this.curList.SelectedIndices.Count == 0)
            {
                this.OpenUriAsync("http://twitter.com/");
            }
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this.curList.SelectedIndices[0]).ScreenName + "/favorites");
            }
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if ((!this.initialLayout) && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.mySize = this.ClientSize;
                    this.mySpDis = this.SplitContainer1.SplitterDistance;
                    this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (this.StatusText.Multiline)
                    {
                        this.mySpDis2 = this.StatusText.Height;
                    }

                    this.myAdSpDis = this.SplitContainer4.SplitterDistance;
                    this.modifySettingLocal = true;
                }
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.settingDialog.SortOrderLock)
            {
                return;
            }

            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            if (this.iconCol)
            {
                mode = IdComparerClass.ComparerMode.Id;
            }
            else
            {
                switch (e.Column)
                {
                    case 0:
                    case 5:
                    case 6:
                        // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                        // ソートしない
                        return;
                    case 1:
                        // ニックネーム
                        mode = IdComparerClass.ComparerMode.Nickname;
                        break;
                    case 2:
                        // 本文
                        mode = IdComparerClass.ComparerMode.Data;
                        break;
                    case 3:
                        // 時刻=発言Id
                        mode = IdComparerClass.ComparerMode.Id;
                        break;
                    case 4:
                        // 名前
                        mode = IdComparerClass.ComparerMode.Name;
                        break;
                    case 7:
                        // Source
                        mode = IdComparerClass.ComparerMode.Source;
                        break;
                }
            }

            this.statuses.ToggleSortOrder(mode);
            this.InitColumnText();

            if (this.iconCol)
            {
                ((DetailsListView)sender).Columns[0].Text = this.columnOrgTexts[0];
                ((DetailsListView)sender).Columns[1].Text = this.columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    ((DetailsListView)sender).Columns[i].Text = this.columnOrgTexts[i];
                }

                ((DetailsListView)sender).Columns[e.Column].Text = this.columnTexts[e.Column];
            }

            this.itemCache = null;
            this.postCache = null;

            if (this.statuses.Tabs[this.curTab.Text].AllCount > 0 && this.curPost != null)
            {
                int idx = this.statuses.Tabs[this.curTab.Text].IndexOf(this.curPost.StatusId);
                if (idx > -1)
                {
                    this.SelectListItem(this.curList, idx);
                    this.curList.EnsureVisible(idx);
                }
            }

            this.curList.Refresh();
            this.modifySettingCommon = true;
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myLoc = this.DesktopLocation;
                this.modifySettingLocal = true;
            }
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            if (this.ListTab.SelectedTab == null)
            {
                return;
            }

            if (this.statuses == null || this.statuses.Tabs == null || !this.statuses.Tabs.ContainsKey(this.ListTab.SelectedTab.Text))
            {
                return;
            }

            if (!this.ExistCurrentPost)
            {
                this.ReplyStripMenuItem.Enabled = false;
                this.ReplyAllStripMenuItem.Enabled = false;
                this.DMStripMenuItem.Enabled = false;
                this.ShowProfileMenuItem.Enabled = false;
                this.ShowUserTimelineContextMenuItem.Enabled = false;
                this.ListManageUserContextToolStripMenuItem2.Enabled = false;
                this.MoveToFavToolStripMenuItem.Enabled = false;
                this.TabMenuItem.Enabled = false;
                this.IDRuleMenuItem.Enabled = false;
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
            }
            else
            {
                this.ShowProfileMenuItem.Enabled = true;
                this.ListManageUserContextToolStripMenuItem2.Enabled = true;
                this.ReplyStripMenuItem.Enabled = true;
                this.ReplyAllStripMenuItem.Enabled = true;
                this.DMStripMenuItem.Enabled = true;
                this.ShowUserTimelineContextMenuItem.Enabled = true;
                this.MoveToFavToolStripMenuItem.Enabled = true;
                this.TabMenuItem.Enabled = true;
                this.IDRuleMenuItem.Enabled = true;
                this.ReadedStripMenuItem.Enabled = true;
                this.UnreadStripMenuItem.Enabled = true;
            }

            this.DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
            if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage || !this.ExistCurrentPost || this.curPost.IsDm)
            {
                this.FavAddToolStripMenuItem.Enabled = false;
                this.FavRemoveToolStripMenuItem.Enabled = false;
                this.StatusOpenMenuItem.Enabled = false;
                this.FavorareMenuItem.Enabled = false;
                this.ShowRelatedStatusesMenuItem.Enabled = false;
                this.ReTweetStripMenuItem.Enabled = false;
                this.ReTweetOriginalStripMenuItem.Enabled = false;
                this.QuoteStripMenuItem.Enabled = false;
                this.FavoriteRetweetContextMenu.Enabled = false;
                this.FavoriteRetweetUnofficialContextMenu.Enabled = false;
                if (this.ExistCurrentPost && this.curPost.IsDm)
                {
                    this.DeleteStripMenuItem.Enabled = true;
                }
                else
                {
                    this.DeleteStripMenuItem.Enabled = false;
                }
            }
            else
            {
                this.FavAddToolStripMenuItem.Enabled = true;
                this.FavRemoveToolStripMenuItem.Enabled = true;
                this.StatusOpenMenuItem.Enabled = true;
                this.FavorareMenuItem.Enabled = true;
                this.ShowRelatedStatusesMenuItem.Enabled = true;
                if (this.curPost.IsMe)
                {
                    this.ReTweetOriginalStripMenuItem.Enabled = false;
                    this.FavoriteRetweetContextMenu.Enabled = false;
                    if (string.IsNullOrEmpty(this.curPost.RetweetedBy))
                    {
                        this.DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
                    }
                    else
                    {
                        this.DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText2;
                    }

                    this.DeleteStripMenuItem.Enabled = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(this.curPost.RetweetedBy))
                    {
                        this.DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
                    }
                    else
                    {
                        this.DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText2;
                    }

                    this.DeleteStripMenuItem.Enabled = false;
                    if (this.curPost.IsProtect)
                    {
                        this.ReTweetOriginalStripMenuItem.Enabled = false;
                        this.ReTweetStripMenuItem.Enabled = false;
                        this.QuoteStripMenuItem.Enabled = false;
                        this.FavoriteRetweetContextMenu.Enabled = false;
                        this.FavoriteRetweetUnofficialContextMenu.Enabled = false;
                    }
                    else
                    {
                        this.ReTweetOriginalStripMenuItem.Enabled = true;
                        this.ReTweetStripMenuItem.Enabled = true;
                        this.QuoteStripMenuItem.Enabled = true;
                        this.FavoriteRetweetContextMenu.Enabled = true;
                        this.FavoriteRetweetUnofficialContextMenu.Enabled = true;
                    }
                }
            }

            if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch || !this.ExistCurrentPost || !(this.curPost.InReplyToStatusId > 0))
            {
                this.RepliedStatusOpenMenuItem.Enabled = false;
            }
            else
            {
                this.RepliedStatusOpenMenuItem.Enabled = true;
            }

            if (!this.ExistCurrentPost || string.IsNullOrEmpty(this.curPost.RetweetedBy))
            {
                this.MoveToRTHomeMenuItem.Enabled = false;
            }
            else
            {
                this.MoveToRTHomeMenuItem.Enabled = true;
            }
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true);
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, false);
        }

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoStatusDelete();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            this.curList.BeginUpdate();
            if (this.settingDialog.UnreadManage)
            {
                foreach (int idx in this.curList.SelectedIndices)
                {
                    this.statuses.SetReadAllTab(true, this.curTab.Text, idx);
                }
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(true, idx, this.curTab);
            }

            this.ColorizeList();
            this.curList.EndUpdate();
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

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            this.curList.BeginUpdate();
            if (this.settingDialog.UnreadManage)
            {
                foreach (int idx in this.curList.SelectedIndices)
                {
                    this.statuses.SetReadAllTab(false, this.curTab.Text, idx);
                }
            }

            foreach (int idx in this.curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(false, idx, this.curTab);
            }

            this.ColorizeList();
            this.curList.EndUpdate();
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (this.settingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == -1)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                }
            }

            if (!this.settingDialog.TabIconDisp)
            {
                this.ListTab.Refresh();
            }
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRefresh();
        }

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = default(DialogResult);
            string uid = this.tw.Username.ToLower();
            foreach (UserAccount u in this.settingDialog.UserAccounts)
            {
                if (u.UserId == this.tw.UserId)
                {
                    break;
                }
            }

            try
            {
                result = this.settingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return;
            }

            if (result == DialogResult.OK)
            {
                lock (this.syncObject)
                {
                    this.tw.SetTinyUrlResolve(this.settingDialog.TinyUrlResolve);
                    this.tw.SetRestrictFavCheck(this.settingDialog.RestrictFavCheck);
                    this.tw.ReadOwnPost = this.settingDialog.ReadOwnPost;
                    this.tw.SetUseSsl(this.settingDialog.UseSsl);
                    ShortUrl.IsResolve = this.settingDialog.TinyUrlResolve;
                    ShortUrl.IsForceResolve = this.settingDialog.ShortUrlForceResolve;
                    ShortUrl.SetBitlyId(this.settingDialog.BitlyUser);
                    ShortUrl.SetBitlyKey(this.settingDialog.BitlyPwd);
                    HttpTwitter.SetTwitterUrl(this.cfgCommon.TwitterUrl);
                    HttpTwitter.SetTwitterSearchUrl(this.cfgCommon.TwitterSearchUrl);

                    HttpConnection.InitializeConnection(this.settingDialog.DefaultTimeOut, this.settingDialog.SelectedProxyType, this.settingDialog.ProxyAddress, this.settingDialog.ProxyPort, this.settingDialog.ProxyUser, this.settingDialog.ProxyPassword);
                    this.CreatePictureServices();
#if UA // = "True"
					this.SplitContainer4.Panel2.Controls.RemoveAt(0);
					this.ab = new AdsBrowser();
					this.SplitContainer4.Panel2.Controls.Add(ab);
#endif
                    try
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.Normal;
                            this.ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawItem += this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                            this.ListTab.ImageList = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        if (!this.settingDialog.UnreadManage)
                        {
                            this.ReadedStripMenuItem.Enabled = false;
                            this.UnreadStripMenuItem.Enabled = false;
                            if (this.settingDialog.TabIconDisp)
                            {
                                foreach (TabPage myTab in this.ListTab.TabPages)
                                {
                                    myTab.ImageIndex = -1;
                                }
                            }
                        }
                        else
                        {
                            this.ReadedStripMenuItem.Enabled = true;
                            this.UnreadStripMenuItem.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(UnreadManage)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage mytab in this.ListTab.TabPages)
                        {
                            DetailsListView lst = (DetailsListView)mytab.Tag;
                            lst.GridLines = this.settingDialog.ShowGrid;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(ShowGrid)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    this.PlaySoundMenuItem.Checked = this.settingDialog.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = this.settingDialog.PlaySound;
                    this.fntUnread = this.settingDialog.FontUnread;
                    this.clrUnread = this.settingDialog.ColorUnread;
                    this.fntReaded = this.settingDialog.FontReaded;
                    this.clrRead = this.settingDialog.ColorReaded;
                    this.clrFav = this.settingDialog.ColorFav;
                    this.clrOWL = this.settingDialog.ColorOWL;
                    this.clrRetweet = this.settingDialog.ColorRetweet;
                    this.fntDetail = this.settingDialog.FontDetail;
                    this.clrDetail = this.settingDialog.ColorDetail;
                    this.clrDetailLink = this.settingDialog.ColorDetailLink;
                    this.clrDetailBackcolor = this.settingDialog.ColorDetailBackcolor;
                    this.clrSelf = this.settingDialog.ColorSelf;
                    this.clrAtSelf = this.settingDialog.ColorAtSelf;
                    this.clrTarget = this.settingDialog.ColorTarget;
                    this.clrAtTarget = this.settingDialog.ColorAtTarget;
                    this.clrAtFromTarget = this.settingDialog.ColorAtFromTarget;
                    this.clrAtTo = this.settingDialog.ColorAtTo;
                    this.clrListBackcolor = this.settingDialog.ColorListBackcolor;
                    this.InputBackColor = this.settingDialog.ColorInputBackcolor;
                    this.clrInputForecolor = this.settingDialog.ColorInputFont;
                    this.fntInputFont = this.settingDialog.FontInputFont;
                    try
                    {
                        if (this.StatusText.Focused)
                        {
                            this.StatusText.BackColor = this.InputBackColor;
                        }

                        this.StatusText.Font = this.fntInputFont;
                        this.StatusText.ForeColor = this.clrInputForecolor;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    this.brsForeColorUnread.Dispose();
                    this.brsForeColorReaded.Dispose();
                    this.brsForeColorFav.Dispose();
                    this.brsForeColorOWL.Dispose();
                    this.brsForeColorRetweet.Dispose();
                    this.brsForeColorUnread = new SolidBrush(this.clrUnread);
                    this.brsForeColorReaded = new SolidBrush(this.clrRead);
                    this.brsForeColorFav = new SolidBrush(this.clrFav);
                    this.brsForeColorOWL = new SolidBrush(this.clrOWL);
                    this.brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                    this.brsBackColorMine.Dispose();
                    this.brsBackColorAt.Dispose();
                    this.brsBackColorYou.Dispose();
                    this.brsBackColorAtYou.Dispose();
                    this.brsBackColorAtFromTarget.Dispose();
                    this.brsBackColorAtTo.Dispose();
                    this.brsBackColorNone.Dispose();
                    this.brsBackColorMine = new SolidBrush(this.clrSelf);
                    this.brsBackColorAt = new SolidBrush(this.clrAtSelf);
                    this.brsBackColorYou = new SolidBrush(this.clrTarget);
                    this.brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                    this.brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                    this.brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                    this.brsBackColorNone = new SolidBrush(this.clrListBackcolor);
                    try
                    {
                        if (this.settingDialog.IsMonospace)
                        {
                            this.detailHtmlFormatHeader = DetailHtmlFormatMono1;
                            this.detailHtmlFormatFooter = DetailHtmlFormatMono7;
                        }
                        else
                        {
                            this.detailHtmlFormatHeader = DetailHtmlFormat1;
                            this.detailHtmlFormatFooter = DetailHtmlFormat7;
                        }

                        this.detailHtmlFormatHeader += this.fntDetail.Name + DetailHtmlFormat2 + this.fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                        if (this.settingDialog.IsMonospace)
                        {
                            this.detailHtmlFormatHeader += DetailHtmlFormatMono6;
                        }
                        else
                        {
                            this.detailHtmlFormatHeader += DetailHtmlFormat6;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "Font";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        this.statuses.SetUnreadManage(this.settingDialog.UnreadManage);
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "_statuses";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage tb in this.ListTab.TabPages)
                        {
                            if (this.settingDialog.TabIconDisp)
                            {
                                if (this.statuses.Tabs[tb.Text].UnreadCount == 0)
                                {
                                    tb.ImageIndex = -1;
                                }
                                else
                                {
                                    tb.ImageIndex = 0;
                                }
                            }

                            if (tb.Tag != null && tb.Controls.Count > 0)
                            {
                                ((DetailsListView)tb.Tag).Font = this.fntReaded;
                                ((DetailsListView)tb.Tag).BackColor = this.clrListBackcolor;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp no2)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    this.SetMainWindowTitle();
                    this.SetNotifyIconText();

                    this.itemCache = null;
                    this.postCache = null;
                    if (this.curList != null)
                    {
                        this.curList.Refresh();
                    }

                    this.ListTab.Refresh();

                    Outputz.Key = this.settingDialog.OutputzKey;
                    Outputz.Enabled = this.settingDialog.OutputzEnabled;
                    switch (this.settingDialog.OutputzUrlmode)
                    {
                        case OutputzUrlmode.twittercom:
                            Outputz.OutUrl = "http:// twitter.com/";
                            break;
                        case OutputzUrlmode.twittercomWithUsername:
                            Outputz.OutUrl = "http:// twitter.com/" + this.tw.Username;
                            break;
                    }

                    this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (this.settingDialog.HotkeyEnabled)
                    {
                        ///グローバルホットキーの登録。設定で変更可能にするかも
                        HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                        if ((this.settingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                        }

                        if ((this.settingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                        }

                        this.hookGlobalHotkey.RegisterOriginalHotkey(this.settingDialog.HotkeyKey, this.settingDialog.HotkeyValue, modKey);
                    }

                    if (uid != this.tw.Username)
                    {
                        this.DoGetFollowersMenu();
                    }

                    this.SetImageServiceCombo();
                    if (this.settingDialog.IsNotifyUseGrowl)
                    {
                        this.growlHelper.RegisterGrowl();
                    }

                    try
                    {
                        this.StatusText_TextChanged(null, null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Twitter.AccountState = AccountState.Valid;

            this.TopMost = this.settingDialog.AlwaysTop;
            this.SaveConfigsAll(false);
        }

        private void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                this.DispSelectedPost();
                this.OpenUriAsync(e.Url.OriginalString);
            }
        }

        private void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "data")
            {
                this.StatusLabelUrl.Text = this.PostBrowser.StatusText.Replace("&", "&&");
            }
            else if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;

                if (e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") || e.Url.AbsoluteUri.StartsWith("https:// twitter.com/search?q=%23"))
                {
                    // ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(e.Url.AbsoluteUri);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    this.HashSupl.AddItem(hash);
                    this.HashMgr.AddHashToHistory(hash.Trim(), false);
                    this.AddNewTabForSearch(hash);
                    return;
                }
                else
                {
                    Match m = Regex.Match(e.Url.AbsoluteUri, "^https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
                    {
                        // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
                        if (this.settingDialog.OpenUserTimeline)
                        {
                            if (this.IsKeyDown(Keys.Control))
                            {
                                this.OpenUriAsync(e.Url.OriginalString);
                            }
                            else
                            {
                                this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                            }
                        }
                        else
                        {
                            if (this.IsKeyDown(Keys.Control))
                            {
                                this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                            }
                            else
                            {
                                this.OpenUriAsync(e.Url.OriginalString);
                            }
                        }
                    }
                    else
                    {
                        this.OpenUriAsync(e.Url.OriginalString);
                    }
                }
            }
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                TabPage relTp = this.ListTab.SelectedTab;
                this.RemoveSpecifiedTab(relTp.Text, false);
                this.SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;
            this.prevSelectedTab = e.TabPage;
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D
            if (!this.settingDialog.TabMouseLock && e.Button == MouseButtons.Left && this.tabDraging)
            {
                string tn = string.Empty;
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(this.tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(this.tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    // タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = this.ListTab.SelectedTab.Text;
                }

                if (string.IsNullOrEmpty(tn))
                {
                    return;
                }

                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    if (tb.Text == tn)
                    {
                        this.ListTab.DoDragDrop(tb, DragDropEffects.All);
                        break;
                    }
                }
            }
            else
            {
                this.tabDraging = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Left <= cpos.X & cpos.X <= rect.Right & rect.Top <= cpos.Y & cpos.Y <= rect.Bottom)
                {
                    this.rclickTabName = this.ListTab.TabPages[i].Text;
                    break;
                }
            }
        }

        private void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this._curList.Refresh()
            this.DispSelectedPost();
            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (this.ListTab.Focused || ((Control)this.ListTab.SelectedTab.Tag).Focused)
            {
                this.Tag = this.ListTab.Tag;
            }

            this.TabMenuControl(this.ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.PostBrowser.StatusText.StartsWith("http") || this.PostBrowser.StatusText.StartsWith("ftp") || this.PostBrowser.StatusText.StartsWith("data"))
                {
                    this.StatusLabelUrl.Text = this.PostBrowser.StatusText.Replace("&", "&&");
                }

                if (string.IsNullOrEmpty(this.PostBrowser.StatusText))
                {
                    this.SetStatusLabelUrl();
                }
            }
            catch (Exception)
            {
            }
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                // @マーク
                if (!this.settingDialog.UseAtIdSupplement)
                {
                    return;
                }

                int cnt = this.AtIdSupl.ItemCount;
                this.ShowSuplDialog(this.StatusText, this.AtIdSupl);
                if (cnt != this.AtIdSupl.ItemCount)
                {
                    this.modifySettingAtId = true;
                }

                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!this.settingDialog.UseHashSupplement)
                {
                    return;
                }

                this.ShowSuplDialog(this.StatusText, this.HashSupl);
                e.Handled = true;
            }
        }

        private void StatusText_KeyUp(object sender, KeyEventArgs e)
        {
            // スペースキーで未読ジャンプ
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.ProcessKey)
                {
                    bool isSpace = false;
                    foreach (char c in this.StatusText.Text.ToCharArray())
                    {
                        if (c == ' ' || c == '　')
                        {
                            isSpace = true;
                        }
                        else
                        {
                            isSpace = false;
                            break;
                        }
                    }

                    if (isSpace)
                    {
                        e.Handled = true;
                        this.StatusText.Text = string.Empty;
                        this.JumpUnreadMenuItem_Click(null, null);
                    }
                }
            }

            this.StatusText_TextChanged(null, null);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            // 文字数カウント
            int len = this.GetRestStatusCount(true, false);
            this.lblLen.Text = len.ToString();
            if (len < 0)
            {
                this.StatusText.ForeColor = Color.Red;
            }
            else
            {
                this.StatusText.ForeColor = this.clrInputForecolor;
            }

            if (string.IsNullOrEmpty(this.StatusText.Text))
            {
                this.replyToId = 0;
                this.replyToName = string.Empty;
            }
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (this.itemCache != null && e.StartIndex >= this.itemCacheIndex && e.EndIndex < this.itemCacheIndex + this.itemCache.Length && this.curList.Equals(sender))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            if (this.curList.Equals(sender))
            {
                this.CreateCache(e.StartIndex, e.EndIndex);
            }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this.itemCache != null && e.ItemIndex >= this.itemCacheIndex && e.ItemIndex < this.itemCacheIndex + this.itemCache.Length && this.curList.Equals(sender))
            {
                // A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = this.itemCache[e.ItemIndex - this.itemCacheIndex];
            }
            else
            {
                // A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((Hoehoe.TweenCustomControl.DetailsListView)sender).Parent;
                try
                {
                    e.Item = this.CreateItem(tb, this.statuses.Item(tb.Text, e.ItemIndex), e.ItemIndex);
                }
                catch (Exception)
                {
                    // 不正な要求に対する間に合わせの応答
                    e.Item = new ImageListViewItem(new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }, string.Empty);
                }
            }
        }

        private void MyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void MyList_HScrolled(object sender, EventArgs e)
        {
            DetailsListView listView = (DetailsListView)sender;
            listView.Refresh();
        }

        private void MyList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.State == 0)
            {
                return;
            }

            e.DrawDefault = false;
            if (!e.Item.Selected)
            {
                SolidBrush brs2 = null;
                var cl = e.Item.BackColor;
                if (cl == this.clrSelf)
                {
                    brs2 = this.brsBackColorMine;
                }
                else if (cl == this.clrAtSelf)
                {
                    brs2 = this.brsBackColorAt;
                }
                else if (cl == this.clrTarget)
                {
                    brs2 = this.brsBackColorYou;
                }
                else if (cl == this.clrAtTarget)
                {
                    brs2 = this.brsBackColorAtYou;
                }
                else if (cl == this.clrAtFromTarget)
                {
                    brs2 = this.brsBackColorAtFromTarget;
                }
                else if (cl == this.clrAtTo)
                {
                    brs2 = this.brsBackColorAtTo;
                }
                else
                {
                    brs2 = this.brsBackColorNone;
                }

                e.Graphics.FillRectangle(brs2, e.Bounds);
            }
            else
            {
                // 選択中の行
                if (((Control)sender).Focused)
                {
                    e.Graphics.FillRectangle(this.brsHighLight, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(this.brsDeactiveSelection, e.Bounds);
                }
            }

            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                e.DrawFocusRectangle();
            }

            this.DrawListViewItemIcon(e);
        }

        private void MyList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ItemState == 0)
            {
                return;
            }

            if (e.ColumnIndex > 0)
            {
                // アイコン以外の列
                RectangleF rct = e.Bounds;
                RectangleF rctB = e.Bounds;
                rct.Width = e.Header.Width;
                rctB.Width = e.Header.Width;
                if (this.iconCol)
                {
                    rct.Y += e.Item.Font.Height;
                    rct.Height -= e.Item.Font.Height;
                    rctB.Height = e.Item.Font.Height;
                }

                int heightDiff = 0;
                int drawLineCount = Math.Max(1, Math.DivRem(Convert.ToInt32(rct.Height), e.Item.Font.Height, out heightDiff));

                // フォントの高さの半分を足してるのは保険。無くてもいいかも。
                if (!this.iconCol && drawLineCount <= 1)
                {
                }
                else if (heightDiff < e.Item.Font.Height * 0.7)
                {
                    rct.Height = Convert.ToSingle(e.Item.Font.Height * drawLineCount) - 1;
                }
                else
                {
                    drawLineCount += 1;
                }

                if (!e.Item.Selected)
                {
                    // 選択されていない行
                    // 文字色
                    SolidBrush brs = null;
                    bool flg = false;
                    var cl = e.Item.ForeColor;
                    if (cl == this.clrUnread)
                    {
                        brs = this.brsForeColorUnread;
                    }
                    else if (cl == this.clrRead)
                    {
                        brs = this.brsForeColorReaded;
                    }
                    else if (cl == this.clrFav)
                    {
                        brs = this.brsForeColorFav;
                    }
                    else if (cl == this.clrOWL)
                    {
                        brs = this.brsForeColorOWL;
                    }
                    else if (cl == this.clrRetweet)
                    {
                        brs = this.brsForeColorRetweet;
                    }
                    else
                    {
                        brs = new SolidBrush(e.Item.ForeColor);
                        flg = true;
                    }

                    if (rct.Width > 0)
                    {
                        if (this.iconCol)
                        {
                            using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                            {
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), brs.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                            }
                        }
                        else if (drawLineCount == 1)
                        {
                            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                        }
                        else
                        {
                            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), brs.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                        }
                    }

                    if (flg)
                    {
                        brs.Dispose();
                    }
                }
                else
                {
                    if (rct.Width > 0)
                    {
                        // 選択中の行
                        using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                        {
                            if (((Control)sender).Focused)
                            {
                                if (this.iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this.brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                            }
                            else
                            {
                                if (this.iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this.brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this.brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            // 検索メニュー
            this.searchDialog.Owner = this;
            if (this.searchDialog.ShowDialog() == DialogResult.Cancel)
            {
                this.TopMost = this.settingDialog.AlwaysTop;
                return;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (!string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            // 次を検索
            if (string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                if (this.searchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.settingDialog.AlwaysTop;
                    return;
                }

                this.TopMost = this.settingDialog.AlwaysTop;
                if (string.IsNullOrEmpty(this.searchDialog.SWord))
                {
                    return;
                }

                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
            else
            {
                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            // 前を検索
            if (string.IsNullOrEmpty(this.searchDialog.SWord))
            {
                if (this.searchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.settingDialog.AlwaysTop;
                    return;
                }

                this.TopMost = this.settingDialog.AlwaysTop;
                if (string.IsNullOrEmpty(this.searchDialog.SWord))
                {
                    return;
                }
            }

            this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.PrevSearch);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            if (this.aboutBox == null)
            {
                this.aboutBox = new TweenAboutBox();
            }

            this.aboutBox.ShowDialog();
            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ImageSelectionPanel.Enabled)
            {
                return;
            }

            // 現在タブから最終タブまで探索
            int bgnIdx = this.ListTab.TabPages.IndexOf(this.curTab);
            int idx = -1;
            DetailsListView lst = null;
            for (int i = bgnIdx; i < this.ListTab.TabPages.Count; i++)
            {
                // 未読Index取得
                idx = this.statuses.GetOldestUnreadIndex(this.ListTab.TabPages[i].Text);
                if (idx > -1)
                {
                    this.ListTab.SelectedIndex = i;
                    lst = (DetailsListView)this.ListTab.TabPages[i].Tag;
                    break;
                }
            }

            // 未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
            if (idx == -1 && bgnIdx > 0)
            {
                for (int i = 0; i < bgnIdx; i++)
                {
                    idx = this.statuses.GetOldestUnreadIndex(this.ListTab.TabPages[i].Text);
                    if (idx > -1)
                    {
                        this.ListTab.SelectedIndex = i;
                        lst = (DetailsListView)this.ListTab.TabPages[i].Tag;
                        break;
                    }
                }
            }

            // 全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                this.ListTab.SelectedIndex = 0;
                lst = (DetailsListView)this.ListTab.TabPages[0].Tag;
                if (this.statuses.SortOrder == SortOrder.Ascending)
                {
                    idx = lst.VirtualListSize - 1;
                }
                else
                {
                    idx = 0;
                }
            }

            if (lst.VirtualListSize > 0 && idx > -1 && lst.VirtualListSize > idx)
            {
                this.SelectListItem(lst, idx);
                if (this.statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if ((this.statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - this.iconSz - 10) || (this.statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < this.iconSz + 10))
                    {
                        this.MoveTop();
                    }
                    else
                    {
                        lst.EnsureVisible(idx);
                    }
                }
                else
                {
                    lst.EnsureVisible(idx);
                }
            }

            lst.Focus();
        }

        private void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curList.SelectedIndices.Count > 0 && this.statuses.Tabs[this.curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[0]);
                var sid = post.RetweetedId == 0 ? post.StatusId : post.RetweetedId;
                this.OpenUriAsync("http://twitter.com/" + post.ScreenName + "/status/" + sid.ToString());
            }
        }

        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curList.SelectedIndices.Count > 0)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, this.curList.SelectedIndices[0]);
                this.OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + post.ScreenName + "/recent");
            }
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            this.CheckNewVersion();
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(this.displayItem))
            {
                if (this.UserPicture.Image != null)
                {
                    this.UserPicture.Image.Dispose();
                }

                if (this.displayItem.Image != null)
                {
                    try
                    {
                        this.UserPicture.Image = new Bitmap(this.displayItem.Image);
                    }
                    catch (Exception)
                    {
                        this.UserPicture.Image = null;
                    }
                }
                else
                {
                    this.UserPicture.Image = null;
                }
            }
        }

        private void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationHelpWebPageUrl);
        }

        private void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationShortcutKeyHelpWebPageUrl);
        }

        private void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ListTab.SelectedTab != null)
            {
                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    Control pnl = this.ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused || pnl.Controls["comboLang"].Focused || pnl.Controls["buttonSearch"].Focused)
                    {
                        return;
                    }
                }

                ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
                if (modState == ModifierState.NotFlags)
                {
                    return;
                }

                if (modState != ModifierState.None)
                {
                    this.anchorFlag = false;
                }

                if (this.CommonKeyDown(e.KeyCode, FocusedControl.ListTab, modState))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            this.anchorFlag = false;
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            // フォーカスの戻り先を StatusText に設定
            this.Tag = this.StatusText;
            this.StatusText.BackColor = this.InputBackColor;
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (this.ListTab.SelectedTab != null && this.MenuStrip1.Tag == null)
            {
                this.Tag = this.ListTab.SelectedTab.Tag;
            }

            this.StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            if (this.CommonKeyDown(e.KeyCode, FocusedControl.StatusText, modState))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            this.StatusText_TextChanged(null, null);
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine), Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            this.SaveFileDialog1.FileName = string.Format("HoehoePosts{0:yyMMdd-HHmmss}.tsv", DateTime.Now);
            this.SaveFileDialog1.InitialDirectory = MyCommon.AppDir;
            this.SaveFileDialog1.Filter = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText3;
            this.SaveFileDialog1.FilterIndex = 0;
            this.SaveFileDialog1.Title = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText4;
            this.SaveFileDialog1.RestoreDirectory = true;

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!this.SaveFileDialog1.ValidateNames)
                {
                    return;
                }

                using (StreamWriter sw = new StreamWriter(this.SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    if (rslt == DialogResult.Yes)
                    {
                        // All
                        for (int idx = 0; idx <= this.curList.VirtualListSize - 1; idx++)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }

                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}", post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""), post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl, post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }
                    else
                    {
                        foreach (int idx in this.curList.SelectedIndices)
                        {
                            PostClass post = this.statuses.Item(this.curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }

                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}", post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""), post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl, post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }

                    sw.Close();
                }
            }

            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState modState = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            bool res = this.CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, modState);
            if (res)
            {
                e.IsInputKey = true;
            }
        }

        private void ListTab_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this.RemoveSpecifiedTab(this.ListTab.TabPages[i].Text, true);
                        this.SaveConfigsTabs();
                        break;
                    }
                }
            }
        }

        private void Tabs_DoubleClick(object sender, MouseEventArgs e)
        {
            string tn = this.ListTab.SelectedTab.Text;
            this.TabRename(ref tn);
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.settingDialog.TabMouseLock)
            {
                return;
            }

            Point cpos = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this.tabDraging = true;
                        this.tabMouseDownPoint = e.Location;
                        break;
                    }
                }
            }
            else
            {
                this.tabDraging = false;
            }
        }

        private void Tabs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TabPage)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Tabs_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TabPage)))
            {
                return;
            }

            this.tabDraging = false;
            string tn = string.Empty;
            bool bef = false;
            Point cpos = new Point(e.X, e.Y);
            Point spos = this.ListTab.PointToClient(cpos);
            int i = 0;
            for (i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Left <= spos.X && spos.X <= rect.Right && rect.Top <= spos.Y && spos.Y <= rect.Bottom)
                {
                    tn = this.ListTab.TabPages[i].Text;
                    bef = spos.X <= (rect.Left + rect.Right) / 2;
                    break;
                }
            }

            // タブのないところにドロップ->最後尾へ移動
            if (string.IsNullOrEmpty(tn))
            {
                tn = this.ListTab.TabPages[this.ListTab.TabPages.Count - 1].Text;
                bef = false;
                i = this.ListTab.TabPages.Count - 1;
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn)
            {
                return;
            }

            this.ReOrderTab(tp.Text, tn, bef);
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            this.tabDraging = false;
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            // 200ms
            this.RefreshTasktrayIcon(false);
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            // 右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (string.IsNullOrEmpty(this.rclickTabName) || !object.ReferenceEquals(sender, this.ContextMenuTabProperty))
            {
                if (this.ListTab != null && this.ListTab.SelectedTab != null)
                {
                    this.rclickTabName = this.ListTab.SelectedTab.Text;
                }
                else
                {
                    return;
                }
            }

            if (this.statuses == null)
            {
                return;
            }

            if (this.statuses.Tabs == null)
            {
                return;
            }

            TabClass tb = this.statuses.Tabs[this.rclickTabName];
            if (tb == null)
            {
                return;
            }

            this.NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;
            this.soundfileListup = true;
            this.SoundFileComboBox.Items.Clear();
            this.SoundFileTbComboBox.Items.Clear();
            this.SoundFileComboBox.Items.Add(string.Empty);
            this.SoundFileTbComboBox.Items.Add(string.Empty);
            DirectoryInfo soundDir = new DirectoryInfo(MyCommon.AppDir + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.AppDir, "Sounds")))
            {
                soundDir = soundDir.GetDirectories("Sounds")[0];
            }

            foreach (FileInfo soundFile in soundDir.GetFiles("*.wav"))
            {
                this.SoundFileComboBox.Items.Add(soundFile.Name);
                this.SoundFileTbComboBox.Items.Add(soundFile.Name);
            }

            int idx = this.SoundFileComboBox.Items.IndexOf(tb.SoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            this.SoundFileComboBox.SelectedIndex = idx;
            this.SoundFileTbComboBox.SelectedIndex = idx;
            this.soundfileListup = false;
            this.UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            this.TabMenuControl(this.rclickTabName);
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            this.UreadManageMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.UnreadMngTbMenuItem.Checked = this.UreadManageMenuItem.Checked;

            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.ChangeTabUnreadManage(this.rclickTabName, this.UreadManageMenuItem.Checked);
            this.SaveConfigsTabs();
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            this.NotifyDispMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NotifyTbMenuItem.Checked = this.NotifyDispMenuItem.Checked;

            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.statuses.Tabs[this.rclickTabName].Notify = this.NotifyDispMenuItem.Checked;
            this.SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.soundfileListup || string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.statuses.Tabs[this.rclickTabName].SoundFile = (string)((ToolStripComboBox)sender).SelectedItem;
            this.SaveConfigsTabs();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.rclickTabName) || object.ReferenceEquals(sender, this.DeleteTbMenuItem))
            {
                this.rclickTabName = this.ListTab.SelectedTab.Text;
            }

            this.RemoveSpecifiedTab(this.rclickTabName, true);
            this.SaveConfigsTabs();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                this.rclickTabName = this.statuses.GetTabByType(TabUsageType.Home).TabName;
            }

            this.fltDialog.SetCurrent(this.rclickTabName);
            this.fltDialog.ShowDialog();
            this.TopMost = this.settingDialog.AlwaysTop;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                this.statuses.FilterAll();
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
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

            this.SaveConfigsTabs();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = null;
            TabUsageType tabUsage = default(TabUsageType);
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = this.statuses.GetUniqueTabName();
                inputName.SetIsShowUsage(true);
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return;
                }

                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (!string.IsNullOrEmpty(tabName))
            {
                // List対応
                ListElement list = null;
                if (tabUsage == TabUsageType.Lists)
                {
                    using (ListAvailable listAvail = new ListAvailable())
                    {
                        if (listAvail.ShowDialog(this) == DialogResult.Cancel)
                        {
                            return;
                        }

                        if (listAvail.SelectedList == null)
                        {
                            return;
                        }

                        list = listAvail.SelectedList;
                    }
                }

                if (!this.statuses.AddTab(tabName, tabUsage, list) || !this.AddNewTab(tabName, false, tabUsage, list))
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    // 成功
                    this.SaveConfigsTabs();
                    if (tabUsage == TabUsageType.PublicSearch)
                    {
                        this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                        this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                        this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                    }

                    if (tabUsage == TabUsageType.Lists)
                    {
                        this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                        this.ListTabSelect(this.ListTab.TabPages[this.ListTab.TabPages.Count - 1]);
                        this.GetTimeline(WorkerType.List, 1, 0, tabName);
                    }
                }
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            // 選択発言を元にフィルタ追加
            foreach (int idx in this.curList.SelectedIndices)
            {
                // タブ選択（or追加）
                string tabName = string.Empty;
                if (!this.SelectTab(ref tabName))
                {
                    return;
                }

                this.fltDialog.SetCurrent(tabName);
                PostClass statusesItem = this.statuses.Item(this.curTab.Text, idx);
                if (statusesItem.RetweetedId == 0)
                {
                    this.fltDialog.AddNewFilter(statusesItem.ScreenName, statusesItem.TextFromApi);
                }
                else
                {
                    this.fltDialog.AddNewFilter(statusesItem.RetweetedBy, statusesItem.TextFromApi);
                }

                this.fltDialog.ShowDialog();
                this.TopMost = this.settingDialog.AlwaysTop;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                this.statuses.FilterAll();
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.settingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
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

            this.SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null && ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                this.curPost = this.statuses.Item(this.ListTab.SelectedTab.Text, ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true, true);
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = string.Empty;

            // 未選択なら処理終了
            if (this.curList.SelectedIndices.Count == 0)
            {
                return;
            }

            // タブ選択（or追加）
            if (!this.SelectTab(ref tabName))
            {
                return;
            }

            bool mv = false;
            bool mk = false;
            this.MoveOrCopy(ref mv, ref mk);

            List<string> ids = new List<string>();
            foreach (int idx in this.curList.SelectedIndices)
            {
                PostClass post = this.statuses.Item(this.curTab.Text, idx);
                if (!ids.Contains(post.ScreenName))
                {
                    FiltersClass fc = new FiltersClass();
                    ids.Add(post.ScreenName);
                    if (post.RetweetedId == 0)
                    {
                        fc.NameFilter = post.ScreenName;
                    }
                    else
                    {
                        fc.NameFilter = post.RetweetedBy;
                    }

                    fc.SearchBoth = true;
                    fc.MoveFrom = mv;
                    fc.SetMark = mk;
                    fc.UseRegex = false;
                    fc.SearchUrl = false;
                    this.statuses.Tabs[tabName].AddFilter(fc);
                }
            }

            if (ids.Count != 0)
            {
                List<string> atids = new List<string>();
                foreach (string id in ids)
                {
                    atids.Add("@" + id);
                }

                int cnt = this.AtIdSupl.ItemCount;
                this.AtIdSupl.AddRangeItem(atids.ToArray());
                if (this.AtIdSupl.ItemCount != cnt)
                {
                    this.modifySettingAtId = true;
                }
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.itemCache = null;
                this.postCache = null;
                this.curPost = null;
                this.curItemIndex = -1;
                this.statuses.FilterAll();
                foreach (TabPage tb in this.ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                    if (this.statuses.ContainsTab(tb.Text))
                    {
                        if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (this.settingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (this.settingDialog.TabIconDisp)
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
            finally
            {
                this.Cursor = Cursors.Default;
            }

            this.SaveConfigsTabs();
        }

        private void CopySTOTMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyStot();
        }

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyIdUri();
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            if (this.StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                this.StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                for (int i = 0; i <= this.curList.VirtualListSize - 1; i++)
                {
                    this.curList.SelectedIndices.Add(i);
                }
            }
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            if (this.PostBrowser.Document.Links.Count > 0)
            {
                this.urlDialog.ClearUrl();
                string openUrlStr = string.Empty;
                if (this.PostBrowser.Document.Links.Count == 1)
                {
                    string urlStr = string.Empty;
                    try
                    {
                        urlStr = MyCommon.IDNDecode(this.PostBrowser.Document.Links[0].GetAttribute("href"));
                    }
                    catch (ArgumentException)
                    {
                        // 変なHTML？
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(urlStr))
                    {
                        return;
                    }

                    openUrlStr = MyCommon.GetUrlEncodeMultibyteChar(urlStr);
                }
                else
                {
                    foreach (HtmlElement linkElm in this.PostBrowser.Document.Links)
                    {
                        string urlStr = string.Empty;
                        string linkText = string.Empty;
                        string href = string.Empty;
                        try
                        {
                            urlStr = linkElm.GetAttribute("title");
                            href = MyCommon.IDNDecode(linkElm.GetAttribute("href"));
                            if (string.IsNullOrEmpty(urlStr))
                            {
                                urlStr = href;
                            }

                            linkText = linkElm.InnerText;
                            if (!linkText.StartsWith("http") && !linkText.StartsWith("#") && !linkText.Contains("."))
                            {
                                linkText = "@" + linkText;
                            }
                        }
                        catch (ArgumentException)
                        {
                            // 変なHTML？
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        if (string.IsNullOrEmpty(urlStr))
                        {
                            continue;
                        }

                        this.urlDialog.AddUrl(new OpenUrlItem(linkText, MyCommon.GetUrlEncodeMultibyteChar(urlStr), href));
                    }

                    try
                    {
                        if (this.urlDialog.ShowDialog() == DialogResult.OK)
                        {
                            openUrlStr = this.urlDialog.SelectedUrl;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    this.TopMost = this.settingDialog.AlwaysTop;
                }

                if (string.IsNullOrEmpty(openUrlStr))
                {
                    return;
                }

                if (openUrlStr.StartsWith("http://twitter.com/search?q=") || openUrlStr.StartsWith("https:// twitter.com/search?q="))
                {
                    // ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(openUrlStr);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    this.HashSupl.AddItem(hash);
                    this.HashMgr.AddHashToHistory(hash.Trim(), false);
                    this.AddNewTabForSearch(hash);
                    return;
                }

                Match m = Regex.Match(openUrlStr, "^https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                if (this.settingDialog.OpenUserTimeline && m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
                {
                    this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                }
                else
                {
                    this.OpenUriAsync(openUrlStr);
                }

                return;
            }
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.ClearTab(this.rclickTabName, true);
        }

        private void SetStatusLabelApiHandler(object sender, ApiInformationChangedEventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new SetStatusLabelApiDelegate(this.SetStatusLabelApi));
                }
                else
                {
                    this.SetStatusLabelApi();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            if (!this.initialLayout && this.settingDialog.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }

            if (this.initialLayout && this.cfgLocal != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                this.ClientSize = this.cfgLocal.FormSize;          // 'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                this.DesktopLocation = this.cfgLocal.FormLocation; // '位置保持（最小化・最大化されたまま終了した場合の対応用）

                if (!this.SplitContainer4.Panel2Collapsed && this.cfgLocal.AdSplitterDistance > this.SplitContainer4.Panel1MinSize)
                {
                    // Splitterの位置設定
                    this.SplitContainer4.SplitterDistance = this.cfgLocal.AdSplitterDistance;
                }

                if (this.cfgLocal.SplitterDistance > this.SplitContainer1.Panel1MinSize && this.cfgLocal.SplitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    // Splitterの位置設定
                    this.SplitContainer1.SplitterDistance = this.cfgLocal.SplitterDistance;
                }

                // 発言欄複数行
                this.StatusText.Multiline = this.cfgLocal.StatusMultiline;
                if (this.StatusText.Multiline)
                {
                    int dis = this.SplitContainer2.Height - this.cfgLocal.StatusTextHeight - this.SplitContainer2.SplitterWidth;
                    if (dis > this.SplitContainer2.Panel1MinSize && dis < this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.cfgLocal.StatusTextHeight - this.SplitContainer2.SplitterWidth;
                    }

                    this.StatusText.Height = this.cfgLocal.StatusTextHeight;
                }
                else
                {
                    if (this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth > 0)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth;
                    }
                }

                if (this.cfgLocal.PreviewDistance > this.SplitContainer3.Panel1MinSize && this.cfgLocal.PreviewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = this.cfgLocal.PreviewDistance;
                }

                this.initialLayout = false;
            }
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.PlaySoundMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.PlaySoundFileMenuItem.Checked = this.PlaySoundMenuItem.Checked;
            this.settingDialog.PlaySound = this.PlaySoundMenuItem.Checked;
            this.modifySettingCommon = true;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.mySpDis = this.SplitContainer1.SplitterDistance;
                if (this.StatusText.Multiline)
                {
                    this.mySpDis2 = this.StatusText.Height;
                }

                this.modifySettingLocal = true;
            }
        }

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myAdSpDis = this.SplitContainer4.SplitterDistance;
                this.modifySettingLocal = true;
            }
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRepliedStatusOpen();
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            // 発言詳細のアイコン右クリック時のメニュー制御
            if (this.curList.SelectedIndices.Count > 0 && this.curPost != null)
            {
                string name = this.curPost.ImageUrl;
                if (name != null && name.Length > 0)
                {
                    int idx = name.LastIndexOf('/');
                    if (idx != -1)
                    {
                        name = Path.GetFileName(name.Substring(idx));
                        if (name.Contains("_normal.") || name.EndsWith("_normal"))
                        {
                            name = name.Replace("_normal", string.Empty);
                            this.IconNameToolStripMenuItem.Text = name;
                            this.IconNameToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            this.IconNameToolStripMenuItem.Enabled = false;
                            this.IconNameToolStripMenuItem.Text = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText1;
                        }
                    }
                    else
                    {
                        this.IconNameToolStripMenuItem.Enabled = false;
                        this.IconNameToolStripMenuItem.Text = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText1;
                    }

                    if (this.iconDict[this.curPost.ImageUrl] != null)
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    this.IconNameToolStripMenuItem.Enabled = false;
                    this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    this.IconNameToolStripMenuItem.Text = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText1;
                }
            }
            else
            {
                this.IconNameToolStripMenuItem.Enabled = false;
                this.SaveIconPictureToolStripMenuItem.Enabled = false;
                this.IconNameToolStripMenuItem.Text = Hoehoe.Properties.Resources.ContextMenuStrip3_OpeningText2;
            }

            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                if (id == this.tw.Username)
                {
                    this.FollowToolStripMenuItem.Enabled = false;
                    this.UnFollowToolStripMenuItem.Enabled = false;
                    this.ShowFriendShipToolStripMenuItem.Enabled = false;
                    this.ShowUserStatusToolStripMenuItem.Enabled = true;
                    this.SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                    this.ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
                else
                {
                    this.FollowToolStripMenuItem.Enabled = true;
                    this.UnFollowToolStripMenuItem.Enabled = true;
                    this.ShowFriendShipToolStripMenuItem.Enabled = true;
                    this.ShowUserStatusToolStripMenuItem.Enabled = true;
                    this.SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
            }
            else
            {
                this.FollowToolStripMenuItem.Enabled = false;
                this.UnFollowToolStripMenuItem.Enabled = false;
                this.ShowFriendShipToolStripMenuItem.Enabled = false;
                this.ShowUserStatusToolStripMenuItem.Enabled = false;
                this.SearchPostsDetailNameToolStripMenuItem.Enabled = false;
                this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                this.ListManageUserContextToolStripMenuItem3.Enabled = false;
            }
        }

        private void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            this.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));
            this.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8);
            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // STUB
            }
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curPost == null)
            {
                return;
            }

            string name = this.curPost.ImageUrl;
            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);
            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(this.iconDict[name]))
                    {
                        using (Bitmap bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bmp2))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                            }

                            bmp2.Save(this.SaveFileDialog1.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 処理中にキャッシュアウトする可能性あり
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            this.StatusText.Multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            this.MultiLineMenuItem.Checked = this.StatusText.Multiline;
            this.modifySettingLocal = true;
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            if (this.StatusText.Multiline)
            {
                this.StatusText.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                this.StatusText.ScrollBars = ScrollBars.None;
            }

            this.modifySettingLocal = true;
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            // 発言欄複数行
            this.StatusText.Multiline = this.MultiLineMenuItem.Checked;
            this.cfgLocal.StatusMultiline = this.MultiLineMenuItem.Checked;
            if (this.MultiLineMenuItem.Checked)
            {
                if (this.SplitContainer2.Height - this.mySpDis2 - this.SplitContainer2.SplitterWidth < 0)
                {
                    this.SplitContainer2.SplitterDistance = 0;
                }
                else
                {
                    this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.mySpDis2 - this.SplitContainer2.SplitterWidth;
                }
            }
            else
            {
                this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth;
            }

            this.modifySettingLocal = true;
        }

        private void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.TinyUrl);
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Isgd);
        }

        private void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Twurl);
        }

        private void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Uxnu);
        }

        private void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.UrlConvert(this.settingDialog.AutoShortUrlFirst))
            {
                // 前回使用した短縮URLサービス以外を選択する
                UrlConverter svc = this.settingDialog.AutoShortUrlFirst;
                Random rnd = new Random();
                do
                {
                    svc = (UrlConverter)rnd.Next(System.Enum.GetNames(typeof(UrlConverter)).Length);
                }
                while (!(svc != this.settingDialog.AutoShortUrlFirst && svc != UrlConverter.Nicoms && svc != UrlConverter.Unu));
                this.UrlConvert(svc);
            }
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoUrlUndo();
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.NotifyFileMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked;
            this.cfgCommon.NewAllPop = this.NewPostPopMenuItem.Checked;
            this.modifySettingCommon = true;
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = this.ListLockMenuItem.Checked;
            this.cfgCommon.ListLock = this.ListLockMenuItem.Checked;
            this.modifySettingCommon = true;
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            this.MenuStrip1.Tag = new object();
            this.MenuStrip1.Select();
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            if (this.Tag != null)
            {
                // 設定された戻り先へ遷移
                if (object.ReferenceEquals(this.Tag, this.ListTab.SelectedTab))
                {
                    ((Control)this.ListTab.SelectedTab.Tag).Select();
                }
                else
                {
                    ((Control)this.Tag).Select();
                }
            }
            else
            {
                // 戻り先が指定されていない (初期状態) 場合はタブに遷移
                if (this.ListTab.SelectedIndex > -1 && this.ListTab.SelectedTab.HasChildren)
                {
                    this.Tag = this.ListTab.SelectedTab.Tag;
                    ((Control)this.Tag).Select();
                }
            }

            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            this.MenuStrip1.Tag = null;
        }

        private void MyList_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (this.cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (this.iconCol)
            {
                this.cfgLocal.Width1 = lst.Columns[0].Width;
                this.cfgLocal.Width3 = lst.Columns[1].Width;
            }
            else
            {
                int[] darr = new int[lst.Columns.Count];
                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    darr[lst.Columns[i].DisplayIndex] = i;
                }

                MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    switch (darr[i])
                    {
                        case 0:
                            this.cfgLocal.DisplayIndex1 = i;
                            break;
                        case 1:
                            this.cfgLocal.DisplayIndex2 = i;
                            break;
                        case 2:
                            this.cfgLocal.DisplayIndex3 = i;
                            break;
                        case 3:
                            this.cfgLocal.DisplayIndex4 = i;
                            break;
                        case 4:
                            this.cfgLocal.DisplayIndex5 = i;
                            break;
                        case 5:
                            this.cfgLocal.DisplayIndex6 = i;
                            break;
                        case 6:
                            this.cfgLocal.DisplayIndex7 = i;
                            break;
                        case 7:
                            this.cfgLocal.DisplayIndex8 = i;
                            break;
                    }
                }

                this.cfgLocal.Width1 = lst.Columns[0].Width;
                this.cfgLocal.Width2 = lst.Columns[1].Width;
                this.cfgLocal.Width3 = lst.Columns[2].Width;
                this.cfgLocal.Width4 = lst.Columns[3].Width;
                this.cfgLocal.Width5 = lst.Columns[4].Width;
                this.cfgLocal.Width6 = lst.Columns[5].Width;
                this.cfgLocal.Width7 = lst.Columns[6].Width;
                this.cfgLocal.Width8 = lst.Columns[7].Width;
            }

            this.modifySettingLocal = true;
            this.isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (this.iconCol)
            {
                if (this.cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this.cfgLocal.Width1 = lst.Columns[0].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    this.cfgLocal.Width3 = lst.Columns[1].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }
            }
            else
            {
                if (this.cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this.cfgLocal.Width1 = lst.Columns[0].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    this.cfgLocal.Width2 = lst.Columns[1].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    this.cfgLocal.Width3 = lst.Columns[2].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    this.cfgLocal.Width4 = lst.Columns[3].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    this.cfgLocal.Width5 = lst.Columns[4].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    this.cfgLocal.Width6 = lst.Columns[5].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    this.cfgLocal.Width7 = lst.Columns[6].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }

                if (this.cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    this.cfgLocal.Width8 = lst.Columns[7].Width;
                    this.modifySettingLocal = true;
                    this.isColumnChanged = true;
                }
            }
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            try
            {
                Clipboard.SetDataObject(this.WebBrowser_GetSelectionText(ref this.PostBrowser), false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細ですべて選択
            this.PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }
        
        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem1Url);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem2Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem3Url);
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            this.DoSearchToolStrip(Hoehoe.Properties.Resources.SearchItem4Url);
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + this.postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?:// [^\"]+)\"", RegexOptions.IgnoreCase);
                foreach (Match m in mc)
                {
                    if (m.Groups["url"].Value == this.postBrowserStatusText)
                    {
                        Clipboard.SetDataObject(m.Groups["title"].Value, false, 5, 100);
                        break;
                    }
                }

                if (mc.Count == 0)
                {
                    Clipboard.SetDataObject(this.postBrowserStatusText, false, 5, 100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ContextMenuPostBrowser_Opening(object sender, CancelEventArgs e)
        {
            // URLコピーの項目の表示/非表示
            if (this.PostBrowser.StatusText.StartsWith("http"))
            {
                this.postBrowserStatusText = this.PostBrowser.StatusText;
                string name = this.GetUserId();
                this.UrlCopyContextMenuItem.Enabled = true;
                if (name != null)
                {
                    this.FollowContextMenuItem.Enabled = true;
                    this.RemoveContextMenuItem.Enabled = true;
                    this.FriendshipContextMenuItem.Enabled = true;
                    this.ShowUserStatusContextMenuItem.Enabled = true;
                    this.SearchPostsDetailToolStripMenuItem.Enabled = true;
                    this.IdFilterAddMenuItem.Enabled = true;
                    this.ListManageUserContextToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.FollowContextMenuItem.Enabled = false;
                    this.RemoveContextMenuItem.Enabled = false;
                    this.FriendshipContextMenuItem.Enabled = false;
                    this.ShowUserStatusContextMenuItem.Enabled = false;
                    this.SearchPostsDetailToolStripMenuItem.Enabled = false;
                    this.IdFilterAddMenuItem.Enabled = false;
                    this.ListManageUserContextToolStripMenuItem.Enabled = false;
                    this.SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                }

                this.UseHashtagMenuItem.Enabled = Regex.IsMatch(this.postBrowserStatusText, "^https?:// twitter.com/search\\?q=%23");
            }
            else
            {
                this.postBrowserStatusText = string.Empty;
                this.UrlCopyContextMenuItem.Enabled = false;
                this.FollowContextMenuItem.Enabled = false;
                this.RemoveContextMenuItem.Enabled = false;
                this.FriendshipContextMenuItem.Enabled = false;
                this.ShowUserStatusContextMenuItem.Enabled = false;
                this.SearchPostsDetailToolStripMenuItem.Enabled = false;
                this.SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                this.UseHashtagMenuItem.Enabled = false;
                this.IdFilterAddMenuItem.Enabled = false;
                this.ListManageUserContextToolStripMenuItem.Enabled = false;
            }

            // 文字列選択されていないときは選択文字列関係の項目を非表示に
            string selectText = this.WebBrowser_GetSelectionText(ref this.PostBrowser);
            if (selectText == null)
            {
                this.SelectionSearchContextMenuItem.Enabled = false;
                this.SelectionCopyContextMenuItem.Enabled = false;
                this.SelectionTranslationToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.SelectionSearchContextMenuItem.Enabled = true;
                this.SelectionCopyContextMenuItem.Enabled = true;
                this.SelectionTranslationToolStripMenuItem.Enabled = true;
            }

            // 発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            bool fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != this.tw.Username.ToLower())
                {
                    fAllFlag = true;
                    break;
                }
            }

            this.FriendshipAllMenuItem.Enabled = fAllFlag;
            this.TranslationToolStripMenuItem.Enabled = this.curPost != null;
            e.Cancel = false;
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細の選択文字列で現在のタブを検索
            string txt = this.WebBrowser_GetSelectionText(ref this.PostBrowser);

            if (txt != null)
            {
                this.searchDialog.SWord = txt;
                this.searchDialog.CheckCaseSensitive = false;
                this.searchDialog.CheckRegex = false;

                this.DoTabSearch(this.searchDialog.SWord, this.searchDialog.CheckCaseSensitive, this.searchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.StatusText.Multiline)
            {
                this.mySpDis2 = this.StatusText.Height;
            }

            this.modifySettingLocal = true;
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                this.ImageSelectionPanel.Visible = true;
                this.ImageSelectionPanel.Enabled = true;
                this.TimelinePanel.Visible = false;
                this.TimelinePanel.Enabled = false;
                this.ImagefilePathText.Text = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                this.ImageFromSelectedFile();
                this.Activate();
                this.BringToFront();
                this.StatusText.Focus();
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string data = e.Data.GetData(DataFormats.StringFormat, true) as string;
                if (data != null)
                {
                    this.StatusText.Text += data;
                }
            }
        }

        private void TweenMain_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                FileInfo fl = new FileInfo(filename);
                string ext = fl.Extension;

                if (!string.IsNullOrEmpty(this.ImageService) && this.pictureServices[this.ImageService].CheckValidFilesize(ext, fl.Length))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }

                foreach (string svc in this.ImageServiceCombo.Items)
                {
                    if (string.IsNullOrEmpty(svc))
                    {
                        continue;
                    }

                    if (this.pictureServices[svc].CheckValidFilesize(ext, fl.Length))
                    {
                        this.ImageServiceCombo.SelectedItem = svc;
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }

                e.Effect = DragDropEffects.None;
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        
        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.ListTabSelect(e.TabPage);
        }
        
        private void TweenMain_Shown(object sender, EventArgs e)
        {
            try
            {
                // 発言詳細部初期化
                this.PostBrowser.Url = new Uri("about:blank");
                this.PostBrowser.DocumentText = string.Empty;
            }
            catch (Exception)
            {
            }

            this.NotifyIcon1.Visible = true;
            this.tw.UserIdChanged += this.Tw_UserIdChanged;

            if (MyCommon.IsNetworkAvailable())
            {
                string tabNameAny = string.Empty;
                this.GetTimeline(WorkerType.BlockIds, 0, 0, tabNameAny);
                if (this.settingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, tabNameAny);
                }

                this.GetTimeline(WorkerType.Configuration, 0, 0, tabNameAny);
                this.StartUserStream();
                this.waitTimeline = true;
                this.GetTimeline(WorkerType.Timeline, 1, 1, tabNameAny);
                this.waitReply = true;
                this.GetTimeline(WorkerType.Reply, 1, 1, tabNameAny);
                this.waitDm = true;
                this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 1, tabNameAny);
                if (this.settingDialog.GetFav)
                {
                    this.waitFav = true;
                    this.GetTimeline(WorkerType.Favorites, 1, 1, tabNameAny);
                }

                this.waitPubSearch = true;
                this.GetTimeline(WorkerType.PublicSearch, 1, 0, tabNameAny);
                this.waitUserTimeline = true;
                this.GetTimeline(WorkerType.UserTimeline, 1, 0, tabNameAny);
                this.waitLists = true;
                this.GetTimeline(WorkerType.List, 1, 0, tabNameAny);
                int i = 0;
                int j = 0;
                while (this.IsInitialRead() && !MyCommon.IsEnding)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    i += 1;
                    j += 1;
                    if (j > 1200)
                    {
                        // 120秒間初期処理が終了しなかったら強制的に打ち切る
                        break;
                    }

                    if (i > 50)
                    {
                        if (MyCommon.IsEnding)
                        {
                            return;
                        }

                        i = 0;
                    }
                }

                if (MyCommon.IsEnding)
                {
                    return;
                }

                // バージョンチェック（引数：起動時チェックの場合はTrue･･･チェック結果のメッセージを表示しない）
                if (this.settingDialog.StartupVersion)
                {
                    this.CheckNewVersion(true);
                }

                // 取得失敗の場合は再試行する
                if (!this.tw.GetFollowersSuccess && this.settingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, tabNameAny);
                }

                // 取得失敗の場合は再試行する
                if (this.settingDialog.TwitterConfiguration.PhotoSizeLimit == 0)
                {
                    this.GetTimeline(WorkerType.Configuration, 0, 0, tabNameAny);
                }

                // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
                if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ReAuthorizeText);
                    this.SettingStripMenuItem_Click(null, null);
                }
            }

            this.isInitializing = false;
            this.timerTimeline.Enabled = true;
        }
        
        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoGetFollowersMenu();
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetUnofficial();
        }
        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetOfficial(true);
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curPost != null)
            {
                this.DispSelectedPost(true);
            }
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            if (MyCommon.DebugBuild || (this.IsKeyDown(Keys.CapsLock) && this.IsKeyDown(Keys.Control) && this.IsKeyDown(Keys.Shift)))
            {
                this.DebugModeToolStripMenuItem.Visible = true;
            }
            else
            {
                this.DebugModeToolStripMenuItem.Visible = false;
            }
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            this.settingDialog.UrlConvertAuto = this.ToolStripMenuItemUrlAutoShorten.Checked;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            this.ToolStripMenuItemUrlAutoShorten.Checked = this.settingDialog.UrlConvertAuto;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.TraceFlag = this.TraceOutToolStripMenuItem.Checked;
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_Leave(this.StatusText, EventArgs.Empty);
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.rclickTabName))
            {
                return;
            }

            this.TabRename(ref this.rclickTabName);
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Bitly);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Jmp);
        }

        private void ApiInfoMenuItem_Click(object sender, EventArgs e)
        {
            GetApiInfoArgs args = new GetApiInfoArgs
            {
                Tw = this.tw,
                Info = new ApiInfo()
            };

            StringBuilder tmp = new StringBuilder();
            using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.ApiInfo6, this.GetApiInfo_Dowork, null, args))
            {
                dlg.ShowDialog();
                if (Convert.ToBoolean(dlg.Result))
                {
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo1 + args.Info.MaxCount.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo2 + args.Info.RemainCount.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo3 + args.Info.ResetTime.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo7 + (this.tw.UserStreamEnabled ? Hoehoe.Properties.Resources.Enable : Hoehoe.Properties.Resources.Disable).ToString());

                    tmp.AppendLine();
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo8 + args.Info.AccessLevel.ToString());
                    this.SetStatusLabelUrl();

                    tmp.AppendLine();
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo9 + (args.Info.MediaMaxCount < 0 ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaMaxCount.ToString()));
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo10 + (args.Info.MediaRemainCount < 0 ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaRemainCount.ToString()));
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo11 + (args.Info.MediaResetTime == new DateTime() ? Hoehoe.Properties.Resources.ApiInfo91 : args.Info.MediaResetTime.ToString()));
                }
                else
                {
                    tmp.Append(Hoehoe.Properties.Resources.ApiInfo5);
                }
            }

            MessageBox.Show(tmp.ToString(), Hoehoe.Properties.Resources.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            this.FollowCommand(id);
        }

        private void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            this.RemoveCommand(id, false);
        }

        private void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            this.ShowFriendship(id);
        }

        private void OwnStatusMenuItem_Click(object sender, EventArgs e)
        {
            this.DoShowUserStatus(this.tw.Username, false);
        }

        private void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.FollowCommand(name);
            }
        }

        private void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.RemoveCommand(name, false);
            }
        }

        private void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.ShowFriendship(name);
            }
        }

        private void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            List<string> ids = new List<string>();
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != this.tw.Username.ToLower())
                {
                    ids.Add(mu.Result("${ScreenName}"));
                }
            }

            this.ShowFriendship(ids.ToArray());
        }

        private void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.ShowUserStatus(name);
            }
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.AddNewTabForUserTimeline(name);
            }
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                this.AddNewTabForSearch("@" + name);
            }
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.modifySettingCommon = true;
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.modifySettingCommon = true;
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoQuote();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }

            string tabName = pnl.Parent.Text;
            TabClass tb = this.statuses.Tabs[tabName];
            ComboBox cmb = (ComboBox)pnl.Controls["comboSearch"];
            ComboBox cmbLang = (ComboBox)pnl.Controls["comboLang"];
            ComboBox cmbusline = (ComboBox)pnl.Controls["comboUserline"];
            cmb.Text = cmb.Text.Trim();

            // TODO: confirm this-> 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
            bool inQuote = false;
            StringBuilder buf = new StringBuilder();
            char[] c = cmb.Text.ToCharArray();
            for (int cnt = 0; cnt < cmb.Text.Length; cnt++)
            {
                if (cnt > cmb.Text.Length - 4)
                {
                    buf.Append(cmb.Text.Substring(cnt));
                    break;
                }

                if (c[cnt] == Convert.ToChar("\""))
                {
                    inQuote = !inQuote;
                }
                else
                {
                    if (!inQuote && cmb.Text.Substring(cnt, 4).Equals(" or ", StringComparison.OrdinalIgnoreCase))
                    {
                        buf.Append(" OR ");
                        cnt += 3;
                        continue;
                    }
                }

                buf.Append(c[cnt]);
            }

            cmb.Text = buf.ToString();

            tb.SearchWords = cmb.Text;
            tb.SearchLang = cmbLang.Text;
            if (string.IsNullOrEmpty(cmb.Text))
            {
                ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
                this.SaveConfigsTabs();
                return;
            }

            if (tb.IsQueryChanged())
            {
                int idx = ((ComboBox)pnl.Controls["comboSearch"]).Items.IndexOf(tb.SearchWords);
                if (idx > -1)
                {
                    ((ComboBox)pnl.Controls["comboSearch"]).Items.RemoveAt(idx);
                }

                ((ComboBox)pnl.Controls["comboSearch"]).Items.Insert(0, tb.SearchWords);
                cmb.Text = tb.SearchWords;
                cmb.SelectAll();
                DetailsListView lst = (DetailsListView)pnl.Parent.Tag;
                lst.VirtualListSize = 0;
                lst.Items.Clear();
                this.statuses.ClearTabIds(tabName);
                this.SaveConfigsTabs(); // 検索条件の保存
            }

            this.GetTimeline(WorkerType.PublicSearch, 1, 0, tabName);
            ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            // もっと前を取得
            this.DoRefreshMore();
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            if (this.statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TabClass tb = this.statuses.RemovedTab.Pop();
                string renamed = tb.TabName;
                for (int i = 1; i <= int.MaxValue; i++)
                {
                    if (!this.statuses.ContainsTab(renamed))
                    {
                        break;
                    }

                    renamed = tb.TabName + "(" + i.ToString() + ")";
                }

                tb.TabName = renamed;
                this.statuses.Tabs.Add(renamed, tb);
                this.AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
                this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1;
                this.SaveConfigsTabs();
            }
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.DoMoveToRTHome();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                string tabName = string.Empty;

                // 未選択なら処理終了
                if (this.curList.SelectedIndices.Count == 0)
                {
                    return;
                }

                // タブ選択（or追加）
                if (!this.SelectTab(ref tabName))
                {
                    return;
                }

                bool mv = false;
                bool mk = false;
                this.MoveOrCopy(ref mv, ref mk);

                FiltersClass fc = new FiltersClass()
                {
                    NameFilter = name,
                    SearchBoth = true,
                    MoveFrom = mv,
                    SetMark = mk,
                    UseRegex = false,
                    SearchUrl = false
                };
                this.statuses.Tabs[tabName].AddFilter(fc);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    this.itemCache = null;
                    this.postCache = null;
                    this.curPost = null;
                    this.curItemIndex = -1;
                    this.statuses.FilterAll();
                    foreach (TabPage tb in this.ListTab.TabPages)
                    {
                        ((DetailsListView)tb.Tag).VirtualListSize = this.statuses.Tabs[tb.Text].AllCount;
                        if (this.statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (this.settingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (this.settingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
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

                this.SaveConfigsTabs();
            }
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string user = null;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            if (object.ReferenceEquals(menuItem.Owner, this.ContextMenuPostBrowser))
            {
                user = this.GetUserId();
                if (user == null)
                {
                    return;
                }
            }
            else if (this.curPost != null)
            {
                user = this.curPost.ScreenName;
            }
            else
            {
                return;
            }

            if (TabInformations.GetInstance().SubscribableLists.Count == 0)
            {
                string res = this.tw.GetListsApi();

                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Failed to get lists. (" + res + ")");
                    return;
                }
            }

            using (MyLists listSelectForm = new MyLists(user, this.tw))
            {
                listSelectForm.ShowDialog(this);
            }
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            Control pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = true;
            }
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            Control pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = false;
            }
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ListTab.SelectedTab != null)
            {
                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType != TabUsageType.PublicSearch)
                {
                    return;
                }

                this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(this.postBrowserStatusText, "^https?:// twitter.com/search\\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                // 使用ハッシュタグとして設定
                this.HashMgr.SetPermanentHash("#" + m.Result("${hash}"));
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashToggleToolStripMenuItem.Checked = true;
                this.modifySettingCommon = true;
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(this.StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt = default(DialogResult);
            try
            {
                rslt = this.HashMgr.ShowDialog();
            }
            catch (Exception)
            {
                return;
            }

            this.TopMost = this.settingDialog.AlwaysTop;
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                this.HashStripSplitButton.Text = "#[-]";
                this.HashToggleMenuItem.Checked = false;
                this.HashToggleToolStripMenuItem.Checked = false;
            }

            this.modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            this.HashToggleMenuItem_Click(null, null);
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.HashMgr.ToggleHash();
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                this.HashStripSplitButton.Text = "#[-]";
                this.HashToggleMenuItem.Checked = false;
                this.HashToggleToolStripMenuItem.Checked = false;
            }

            this.modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            if (this.ListTab.SelectedTab == null)
            {
                return;
            }

            if (this.statuses == null || this.statuses.Tabs == null || !this.statuses.Tabs.ContainsKey(this.ListTab.SelectedTab.Text))
            {
                return;
            }

            if (!this.ExistCurrentPost)
            {
                this.ReplyOpMenuItem.Enabled = false;
                this.ReplyAllOpMenuItem.Enabled = false;
                this.DmOpMenuItem.Enabled = false;
                this.ShowProfMenuItem.Enabled = false;
                this.ShowUserTimelineToolStripMenuItem.Enabled = false;
                this.ListManageMenuItem.Enabled = false;
                this.OpenFavOpMenuItem.Enabled = false;
                this.CreateTabRuleOpMenuItem.Enabled = false;
                this.CreateIdRuleOpMenuItem.Enabled = false;
                this.ReadOpMenuItem.Enabled = false;
                this.UnreadOpMenuItem.Enabled = false;
            }
            else
            {
                this.ReplyOpMenuItem.Enabled = true;
                this.ReplyAllOpMenuItem.Enabled = true;
                this.DmOpMenuItem.Enabled = true;
                this.ShowProfMenuItem.Enabled = true;
                this.ShowUserTimelineToolStripMenuItem.Enabled = true;
                this.ListManageMenuItem.Enabled = true;
                this.OpenFavOpMenuItem.Enabled = true;
                this.CreateTabRuleOpMenuItem.Enabled = true;
                this.CreateIdRuleOpMenuItem.Enabled = true;
                this.ReadOpMenuItem.Enabled = true;
                this.UnreadOpMenuItem.Enabled = true;
            }

            if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage || !this.ExistCurrentPost || this.curPost.IsDm)
            {
                this.FavOpMenuItem.Enabled = false;
                this.UnFavOpMenuItem.Enabled = false;
                this.OpenStatusOpMenuItem.Enabled = false;
                this.OpenFavotterOpMenuItem.Enabled = false;
                this.ShowRelatedStatusesMenuItem2.Enabled = false;
                this.RtOpMenuItem.Enabled = false;
                this.RtUnOpMenuItem.Enabled = false;
                this.QtOpMenuItem.Enabled = false;
                this.FavoriteRetweetMenuItem.Enabled = false;
                this.FavoriteRetweetUnofficialMenuItem.Enabled = false;
                if (this.ExistCurrentPost && this.curPost.IsDm)
                {
                    this.DelOpMenuItem.Enabled = true;
                }
            }
            else
            {
                this.FavOpMenuItem.Enabled = true;
                this.UnFavOpMenuItem.Enabled = true;
                this.OpenStatusOpMenuItem.Enabled = true;
                this.OpenFavotterOpMenuItem.Enabled = true;
                this.ShowRelatedStatusesMenuItem2.Enabled = true;
                if (this.curPost.IsMe)
                {
                    this.RtOpMenuItem.Enabled = false;
                    this.FavoriteRetweetMenuItem.Enabled = false;
                    this.DelOpMenuItem.Enabled = true;
                }
                else
                {
                    this.DelOpMenuItem.Enabled = false;
                    if (this.curPost.IsProtect)
                    {
                        this.RtOpMenuItem.Enabled = false;
                        this.RtUnOpMenuItem.Enabled = false;
                        this.QtOpMenuItem.Enabled = false;
                        this.FavoriteRetweetMenuItem.Enabled = false;
                        this.FavoriteRetweetUnofficialMenuItem.Enabled = false;
                    }
                    else
                    {
                        this.RtOpMenuItem.Enabled = true;
                        this.RtUnOpMenuItem.Enabled = true;
                        this.QtOpMenuItem.Enabled = true;
                        this.FavoriteRetweetMenuItem.Enabled = true;
                        this.FavoriteRetweetUnofficialMenuItem.Enabled = true;
                    }
                }
            }

            if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType != TabUsageType.Favorites)
            {
                this.RefreshPrevOpMenuItem.Enabled = true;
            }
            else
            {
                this.RefreshPrevOpMenuItem.Enabled = false;
            }

            if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch || !this.ExistCurrentPost || !(this.curPost.InReplyToStatusId > 0))
            {
                this.OpenRepSourceOpMenuItem.Enabled = false;
            }
            else
            {
                this.OpenRepSourceOpMenuItem.Enabled = true;
            }

            if (!this.ExistCurrentPost || string.IsNullOrEmpty(this.curPost.RetweetedBy))
            {
                this.OpenRterHomeMenuItem.Enabled = false;
            }
            else
            {
                this.OpenRterHomeMenuItem.Enabled = true;
            }
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            this.ContextMenuTabProperty_Opening(sender, null);
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                this.modifySettingLocal = true;
            }
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            if (this.statuses.RemovedTab.Count == 0)
            {
                this.UndoRemoveTabMenuItem.Enabled = false;
            }
            else
            {
                this.UndoRemoveTabMenuItem.Enabled = true;
            }

            if (this.ListTab.SelectedTab != null)
            {
                if (this.statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    this.PublicSearchQueryMenuItem.Enabled = true;
                }
                else
                {
                    this.PublicSearchQueryMenuItem.Enabled = false;
                }
            }
            else
            {
                this.PublicSearchQueryMenuItem.Enabled = false;
            }

            if (!this.ExistCurrentPost)
            {
                this.CopySTOTMenuItem.Enabled = false;
                this.CopyURLMenuItem.Enabled = false;
                this.CopyUserIdStripMenuItem.Enabled = false;
            }
            else
            {
                this.CopySTOTMenuItem.Enabled = true;
                this.CopyURLMenuItem.Enabled = true;
                this.CopyUserIdStripMenuItem.Enabled = true;
                if (this.curPost.IsDm)
                {
                    this.CopyURLMenuItem.Enabled = false;
                }

                if (this.curPost.IsProtect)
                {
                    this.CopySTOTMenuItem.Enabled = false;
                }
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            this.SetNotifyIconText();
        }

        private void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this.curPost != null)
            {
                id = this.curPost.ScreenName;
            }

            this.ShowUserStatus(id);
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.FollowCommand(id);
                }
            }
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.RemoveCommand(id, false);
                }
            }
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.ShowFriendship(id);
                }
            }
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                this.ShowUserStatus(id, false);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                this.AddNewTabForUserTimeline(id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                string id = (string)this.NameLabel.Tag;
                this.AddNewTabForSearch("@" + id);
            }
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            if (this.curPost != null)
            {
                this.ShowUserStatus(this.curPost.ScreenName, false);
            }
        }

        #region callback

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = this.tw.GetInfoApi(args.Info);
        }

        private void FollowCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostFollowCommand(arg.Id);
        }

        private void RemoveCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostRemoveCommand(arg.Id);
        }

        private void ShowFriendship_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowFriendshipArgs arg = (ShowFriendshipArgs)e.Argument;
            string result = string.Empty;
            foreach (ShowFriendshipArgs.FriendshipInfo fInfo in arg.Ids)
            {
                string rt = arg.Tw.GetFriendshipInfo(fInfo.Id, ref fInfo.IsFollowing, ref fInfo.IsFollowed);
                if (!string.IsNullOrEmpty(rt))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = rt;
                    }

                    fInfo.IsError = true;
                }
            }

            e.Result = result;
        }

        private void GetUserInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            GetUserInfoArgs args = (GetUserInfoArgs)e.Argument;
            e.Result = args.Tw.GetUserInfo(args.Id, ref args.User);
        }

        private void GetRetweet_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;

            long statusid = 0;
            if (this.curPost.RetweetedId > 0)
            {
                statusid = this.curPost.RetweetedId;
            }
            else
            {
                statusid = this.curPost.StatusId;
            }

            this.tw.GetStatus_Retweeted_Count(statusid, ref counter);
            e.Result = counter;
        }
        #endregion

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost)
            {
                using (FormInfo formInfo = new FormInfo(this, Hoehoe.Properties.Resources.RtCountMenuItem_ClickText1, this.GetRetweet_DoWork))
                {
                    int retweet_count = 0;

                    // ダイアログ表示
                    formInfo.ShowDialog();
                    retweet_count = Convert.ToInt32(formInfo.Result);
                    if (retweet_count < 0)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.RtCountText2);
                    }
                    else
                    {
                        MessageBox.Show(retweet_count.ToString() + Hoehoe.Properties.Resources.RtCountText1);
                    }
                }
            }
        }

        private void HookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            if ((this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized) && this.Visible && object.ReferenceEquals(Form.ActiveForm, this))
            {
                // アイコン化
                this.Visible = false;
            }
            else if (Form.ActiveForm == null)
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }

                this.Activate();
                this.BringToFront();
                this.StatusText.Focus();
            }
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Default;
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (this.NameLabel.Tag != null)
            {
                this.OpenUriAsync("http://twitter.com/" + this.NameLabel.Tag.ToString());
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MultiLineMenuItem.PerformClick();
        }

        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ImageSelectionPanel.Visible == true)
            {
                this.ImagefilePathText.CausesValidation = false;
                this.TimelinePanel.Visible = true;
                this.TimelinePanel.Enabled = true;
                this.ImageSelectionPanel.Visible = false;
                this.ImageSelectionPanel.Enabled = false;
                ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
                this.ImagefilePathText.CausesValidation = true;
            }
            else
            {
                this.ImageSelectionPanel.Visible = true;
                this.ImageSelectionPanel.Enabled = true;
                this.TimelinePanel.Visible = false;
                this.TimelinePanel.Enabled = false;
                this.ImagefilePathText.Focus();
            }
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ImageService))
            {
                return;
            }

            this.OpenFileDialog1.Filter = this.pictureServices[this.ImageService].GetFileOpenDialogFilter();
            this.OpenFileDialog1.Title = Hoehoe.Properties.Resources.PickPictureDialog1;
            this.OpenFileDialog1.FileName = string.Empty;

            try
            {
                this.AllowDrop = false;
                if (this.OpenFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
            }
            finally
            {
                this.AllowDrop = true;
            }

            this.ImagefilePathText.Text = this.OpenFileDialog1.FileName;
            this.ImageFromSelectedFile();
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (this.ImageCancelButton.Focused)
            {
                this.ImagefilePathText.CausesValidation = false;
                return;
            }

            this.ImagefilePathText.Text = this.ImagefilePathText.Text.Trim();
            if (string.IsNullOrEmpty(this.ImagefilePathText.Text))
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
            }
            else
            {
                this.ImageFromSelectedFile();
            }
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                this.TimelinePanel.Visible = true;
                this.TimelinePanel.Enabled = true;
                this.ImageSelectionPanel.Visible = false;
                this.ImageSelectionPanel.Enabled = false;
                ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
                this.ImagefilePathText.CausesValidation = true;
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 0x1b)
            {
                this.ImagefilePathText.CausesValidation = false;
                e.Handled = true;
            }
        }

        private void ImageSelection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ImagefilePathText.CausesValidation = false;
            }
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
        {
            this.ImagefilePathText.CausesValidation = false;
            this.TimelinePanel.Visible = true;
            this.TimelinePanel.Enabled = true;
            this.ImageSelectionPanel.Visible = false;
            this.ImageSelectionPanel.Enabled = false;
            ((DetailsListView)this.ListTab.SelectedTab.Tag).Focus();
            this.ImagefilePathText.CausesValidation = true;
        }

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(this.ImageService))
            {
                try
                {
                    FileInfo fi = new FileInfo(this.ImagefilePathText.Text.Trim());
                    if (!this.pictureServices[this.ImageService].CheckValidFilesize(fi.Extension, fi.Length))
                    {
                        this.ImagefilePathText.Text = string.Empty;
                        this.ImageSelectedPicture.Image = this.ImageSelectedPicture.InitialImage;
                        this.ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    }
                }
                catch (Exception)
                {
                }

                this.modifySettingCommon = true;
                this.SaveConfigsAll(false);
                if (this.ImageService == "Twitter")
                {
                    this.StatusText_TextChanged(null, null);
                }
            }
        }

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ListManage form = new ListManage(this.tw))
            {
                form.ShowDialog(this);
            }
        }

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = Convert.ToString(this.SourceLinkLabel.Tag);
            if (!string.IsNullOrEmpty(link) && e.Button == MouseButtons.Left)
            {
                this.OpenUriAsync(link);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            string link = Convert.ToString(this.SourceLinkLabel.Tag);
            if (!string.IsNullOrEmpty(link))
            {
                this.StatusLabelUrl.Text = link;
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            this.SetStatusLabelUrl();
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost && !this.curPost.IsDm)
            {
                this.RtCountMenuItem.Enabled = true;
            }
            else
            {
                this.RtCountMenuItem.Enabled = false;
            }
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyUserId();
        }

        private void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            TabClass backToTab = this.curTab == null ? this.statuses.Tabs[this.ListTab.SelectedTab.Text] : this.statuses.Tabs[this.curTab.Text];
            if (this.ExistCurrentPost && !this.curPost.IsDm)
            {
                // PublicSearchも除外した方がよい？
                if (this.statuses.GetTabByType(TabUsageType.Related) == null)
                {
                    const string TabName = "Related Tweets";
                    string newTabName = TabName;
                    if (!this.AddNewTab(newTabName, false, TabUsageType.Related))
                    {
                        for (int i = 2; i <= 100; i++)
                        {
                            newTabName = TabName + i.ToString();
                            if (this.AddNewTab(newTabName, false, TabUsageType.Related))
                            {
                                this.statuses.AddTab(newTabName, TabUsageType.Related, null);
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.statuses.AddTab(newTabName, TabUsageType.Related, null);
                    }

                    this.statuses.GetTabByName(newTabName).UnreadManage = false;
                    this.statuses.GetTabByName(newTabName).Notify = false;
                }

                TabClass tb = this.statuses.GetTabByType(TabUsageType.Related);
                tb.RelationTargetPost = this.curPost;
                this.ClearTab(tb.TabName, false);
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (tb.TabName == this.ListTab.TabPages[i].Text)
                    {
                        this.ListTab.SelectedIndex = i;
                        this.ListTabSelect(this.ListTab.TabPages[i]);
                        break;
                    }
                }

                this.GetTimeline(WorkerType.Related, 1, 1, tb.TabName);
            }
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("キャッシュメモリ容量         : {0}bytes({1}MB)" + "\r\n", ((ImageDictionary)this.iconDict).CacheMemoryLimit, ((ImageDictionary)this.iconDict).CacheMemoryLimit / 1048576);
            buf.AppendFormat("物理メモリ使用割合           : {0}%" + "\r\n", ((ImageDictionary)this.iconDict).PhysicalMemoryLimit);
            buf.AppendFormat("キャッシュエントリ保持数     : {0}" + "\r\n", ((ImageDictionary)this.iconDict).CacheCount);
            buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + "\r\n", ((ImageDictionary)this.iconDict).CacheRemoveCount);
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void Tw_UserIdChanged()
        {
            this.modifySettingCommon = true;
        }

        private void Tw_PostDeleted(long id)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action(() =>
                    {
                        this.statuses.RemovePostReserve(id);
                        if (this.curTab != null && this.statuses.Tabs[this.curTab.Text].Contains(id))
                        {
                            this.itemCache = null;
                            this.itemCacheIndex = -1;
                            this.postCache = null;
                            ((DetailsListView)this.curTab.Tag).Update();
                            if (this.curPost != null & this.curPost.StatusId == id)
                            {
                                DispSelectedPost(true);
                            }
                        }
                    }));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void Tw_NewPostFromStream()
        {
            if (this.settingDialog.ReadOldPosts)
            {
                // 新着時未読クリア
                this.statuses.SetRead();
            }

            int rsltAddCount = this.statuses.DistributePosts();
            lock (this.syncObject)
            {
                DateTime tm = DateTime.Now;
                if (this.timeLineTimestamps.ContainsKey(tm))
                {
                    this.timeLineTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    this.timeLineTimestamps.Add(tm, rsltAddCount);
                }

                DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                this.timeLineCount = 0;
                foreach (System.DateTime key in this.timeLineTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                    {
                        keys.Add(key);
                    }
                    else
                    {
                        this.timeLineCount += this.timeLineTimestamps[key];
                    }
                }

                foreach (DateTime key in keys)
                {
                    this.timeLineTimestamps.Remove(key);
                }

                keys.Clear();
            }

            if (this.settingDialog.UserstreamPeriodInt > 0)
            {
                return;
            }

            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<bool>(this.RefreshTimeline), true);
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void Tw_UserStreamStarted()
        {
            this.isActiveUserstream = true;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.Tw_UserStreamStarted));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            this.MenuItemUserStream.Text = "&UserStream ▶";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = "&Stop";
            this.StopToolStripMenuItem.Enabled = true;
            this.StatusLabel.Text = "UserStream Started.";
        }

        private void Tw_UserStreamStopped()
        {
            this.isActiveUserstream = false;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.Tw_UserStreamStopped));
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            this.MenuItemUserStream.Text = "&UserStream ■";
            this.MenuItemUserStream.Enabled = true;
            this.StopToolStripMenuItem.Text = "&Start";
            this.StopToolStripMenuItem.Enabled = true;

            this.StatusLabel.Text = "UserStream Stopped.";
        }

        private void Tw_UserStreamEventArrived(Twitter.FormattedEvent ev)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<Twitter.FormattedEvent>(this.Tw_UserStreamEventArrived), ev);
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            this.StatusLabel.Text = "Event: " + ev.Event;
            this.NotifyEvent(ev);
            if (ev.Event == "favorite" || ev.Event == "unfavorite")
            {
                if (this.curTab != null && this.statuses.Tabs[this.curTab.Text].Contains(ev.Id))
                {
                    this.itemCache = null;
                    this.itemCacheIndex = -1;
                    this.postCache = null;
                    ((DetailsListView)this.curTab.Tag).Update();
                }

                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(this.tw.Username.ToLower()))
                {
                    this.RemovePostFromFavTab(new long[] { ev.Id });
                }
            }
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MenuItemUserStream.Enabled = false;
            if (this.StopRefreshAllMenuItem.Checked)
            {
                this.StopRefreshAllMenuItem.Checked = false;
                return;
            }

            if (this.isActiveUserstream)
            {
                this.tw.StopUserStream();
            }
            else
            {
                this.tw.StartUserStream();
            }
        }

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TrackToolStripMenuItem.Checked)
            {
                using (InputTabName inputForm = new InputTabName())
                {
                    inputForm.TabName = this.prevTrackWord;
                    inputForm.SetFormTitle("Input track word");
                    inputForm.SetFormDescription("Track word");
                    if (inputForm.ShowDialog() != DialogResult.OK)
                    {
                        this.TrackToolStripMenuItem.Checked = false;
                        return;
                    }

                    this.prevTrackWord = inputForm.TabName.Trim();
                }

                if (this.prevTrackWord != this.tw.TrackWord)
                {
                    this.tw.TrackWord = this.prevTrackWord;
                    this.modifySettingCommon = true;
                    this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.prevTrackWord);
                    this.tw.ReconnectUserStream();
                }
            }
            else
            {
                this.tw.TrackWord = string.Empty;
                this.tw.ReconnectUserStream();
            }

            this.modifySettingCommon = true;
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tw.AllAtReply = this.AllrepliesToolStripMenuItem.Checked;
            this.modifySettingCommon = true;
            this.tw.ReconnectUserStream();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            if (this.evtDialog == null || this.evtDialog.IsDisposed)
            {
                this.evtDialog = new EventViewerDialog();
                this.evtDialog.Owner = this;

                // 親の中央に表示
                Point pos = this.evtDialog.Location;
                pos.X = Convert.ToInt32(this.Location.X + ((this.Size.Width - this.evtDialog.Size.Width) / 2));
                pos.Y = Convert.ToInt32(this.Location.Y + ((this.Size.Height - this.evtDialog.Size.Height) / 2));
                this.evtDialog.Location = pos;
            }

            this.evtDialog.EventSource = this.tw.StoredEvent;
            if (!this.evtDialog.Visible)
            {
                this.evtDialog.Show(this);
            }
            else
            {
                this.evtDialog.Activate();
            }

            this.TopMost = this.settingDialog.AlwaysTop;
        }

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.IsEnding = true;
            try
            {
                this.Close();
                Application.Restart();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to restart. Please run Tween manually.");
            }
        }

        private void OpenOwnFavedMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.tw.Username))
            {
                this.OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + this.tw.Username + "/recent");
            }
        }

        private void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync("http://twitter.com/" + this.tw.Username);
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }

            this.DoTranslation(this.curPost.TextFromApi);
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoTranslation(this.WebBrowser_GetSelectionText(ref this.PostBrowser));
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowUserTimeline();
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = this.GetUserIdFromCurPostOrInput("Show UserTimeline");
            if (!string.IsNullOrEmpty(id))
            {
                this.AddNewTabForUserTimeline(id);
            }
        }

        private void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = this.GetUserIdFromCurPostOrInput("Show Favstar");
            if (!string.IsNullOrEmpty(id))
            {
                this.OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + id + "/recent");
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                this.isOsResumed = true;
            }
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.TimelineRefreshEnableChange(!this.StopRefreshAllMenuItem.Checked);
        }

        private void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUserAppointUrl();
        }

        private void ImageSelectionPanel_VisibleChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChanged(null, null);
        }

        private void SplitContainer4_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            if (this.SplitContainer4.Panel2Collapsed)
            {
                return;
            }

            if (this.SplitContainer4.Height < this.SplitContainer4.SplitterWidth + this.SplitContainer4.Panel2MinSize + this.SplitContainer4.SplitterDistance && this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - this.SplitContainer4.Panel2MinSize > 0)
            {
                this.SplitContainer4.SplitterDistance = this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - this.SplitContainer4.Panel2MinSize;
            }

            if (this.SplitContainer4.Panel2.Height > 90 && this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - 90 > 0)
            {
                this.SplitContainer4.SplitterDistance = this.SplitContainer4.Height - this.SplitContainer4.SplitterWidth - 90;
            }
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            string selText = this.SourceLinkLabel.Text;
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            string selText = Convert.ToString(this.SourceLinkLabel.Tag);
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            if (this.curPost == null || !this.ExistCurrentPost || this.curPost.IsDm)
            {
                this.SourceCopyMenuItem.Enabled = false;
                this.SourceUrlCopyMenuItem.Enabled = false;
            }
            else
            {
                this.SourceCopyMenuItem.Enabled = true;
                this.SourceUrlCopyMenuItem.Enabled = true;
            }
        }

        private void GrowlHelper_Callback(object sender, GrowlHelper.NotifyCallbackEventArgs e)
        {
            if (Form.ActiveForm == null)
            {
                this.BeginInvoke(
                    new Action(() =>
                    {
                        this.Visible = true;
                        if (WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }

                        this.Activate();
                        this.BringToFront();
                        if (e.NotifyType == GrowlHelper.NotifyType.DirectMessage)
                        {
                            if (!this.GoDirectMessage(e.StatusId))
                            {
                                this.StatusText.Focus();
                            }
                        }
                        else
                        {
                            if (!this.GoStatus(e.StatusId))
                            {
                                this.StatusText.Focus();
                            }
                        }
                    }));
            }
        }
        #endregion

        #endregion private methods
    }
}