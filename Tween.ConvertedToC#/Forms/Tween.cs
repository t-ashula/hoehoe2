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
        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/FrontPage";

        /// <summary>
        /// TODO: hoehoe webpage
        /// </summary>
        private const string ApplicationShortcutKeyHelpWebPageUrl = "http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        // 各種設定
        // 画面サイズ
        private Size _mySize;

        // 画面位置
        private Point _myLoc;

        // 区切り位置
        private int _mySpDis;

        // 発言欄区切り位置
        private int _mySpDis2;

        // プレビュー区切り位置
        private int _mySpDis3;

        // Ad区切り位置
        private int _myAdSpDis;

        // アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private int _iconSz;

        // 1列表示の時True（48サイズのとき）
        private bool _iconCol;

        // 雑多なフラグ類
        // True:起動時処理中
        private bool _initial;

        private bool _initialLayout = true;

        // True:起動時処理中
        private bool _ignoreConfigSave;

        // タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private bool _tabDrag;

        // タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private TabPage _beforeSelectedTab;

        private Point _tabMouseDownPoint;

        // 右クリックしたタブの名前（Tabコントロール機能不足対応）
        private string _rclickTabName;

        // ロック用
        private readonly object _syncObject = new object();

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
        private string _detailHtmlFormatHeader;
        private string _detailHtmlFormatFooter;
        private bool _myStatusError;
        private bool _myStatusOnline;
        private bool _soundfileListup;

        private SpaceKeyCanceler _spaceKeyCanceler;

        // 設定ファイル関連
        private SettingLocal _cfgLocal;

        private SettingCommon _cfgCommon;
        private bool _modifySettingLocal;
        private bool _modifySettingCommon;
        private bool _modifySettingAtId;

        // twitter解析部
        private Twitter tw = new Twitter();

        // Growl呼び出し部
        private GrowlHelper withEventsField_gh = new GrowlHelper("Hoehoe");

        private GrowlHelper GrowlHelper
        {
            get
            {
                return this.withEventsField_gh;
            }

            set
            {
                if (this.withEventsField_gh != null)
                {
                    this.withEventsField_gh.NotifyClicked -= this.GrowlHelper_Callback;
                }
                this.withEventsField_gh = value;
                if (this.withEventsField_gh != null)
                {
                    this.withEventsField_gh.NotifyClicked += this.GrowlHelper_Callback;
                }
            }
        }

        // サブ画面インスタンス

        private AppendSettingDialog withEventsField_SettingDialog = AppendSettingDialog.Instance; // 設定画面インスタンス

        private AppendSettingDialog SettingDialog
        {
            get
            {
                return this.withEventsField_SettingDialog;
            }

            set
            {
                if (this.withEventsField_SettingDialog != null)
                {
                    this.withEventsField_SettingDialog.IntervalChanged -= this.TimerInterval_Changed;
                }
                this.withEventsField_SettingDialog = value;
                if (this.withEventsField_SettingDialog != null)
                {
                    this.withEventsField_SettingDialog.IntervalChanged += this.TimerInterval_Changed;
                }
            }
        }

        private TabsDialog TabDialog = new TabsDialog();      // タブ選択ダイアログインスタンス
        private SearchWord SearchDialog = new SearchWord();   // 検索画面インスタンス
        private FilterDialog fltDialog = new FilterDialog();  // フィルター編集画面
        private OpenURL UrlDialog = new OpenURL();
        private DialogAsShieldIcon dialogAsShieldicon;         // シールドアイコン付きダイアログ
        public AtIdSupplement AtIdSupl; // @id補助
        public AtIdSupplement HashSupl;        // Hashtag補助
        public HashtagManage HashMgr;
        private TweenAboutBox aboutBox;
        private EventViewerDialog evtDialog;

        // 表示フォント、色、アイコン
        // 未読用フォント
        private Font _fntUnread;

        // 未読用文字色
        private Color clrUnread;

        // 既読用フォント
        private Font _fntReaded;

        // 既読用文字色
        private Color clrRead;

        // Fav用文字色
        private Color clrFav;

        // 片思い用文字色
        private Color clrOWL;

        // Retweet用文字色
        private Color clrRetweet;

        // 発言詳細部用フォント
        private Font _fntDetail;

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
        private Color clrInputBackcolor;

        // 入力欄文字色
        private Color clrInputForecolor;

        // 入力欄フォント
        private Font _fntInputFont;

        // アイコン画像リスト
        private IDictionary<string, Image> _TIconDic;

        // At.ico             タスクトレイアイコン：通常時
        private Icon _NIconAt;

        // AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon _NIconAtRed;

        // AtSmoke.ico        タスクトレイアイコン：オフライン時
        private Icon _NIconAtSmoke;

        // Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon[] _NIconRefresh = new Icon[4];

        // Tab.ico            未読のあるタブ用アイコン
        private Icon _TabIcon;

        // Main.ico           画面左上のアイコン
        private Icon _MainIcon;

        // 5g
        private Icon _ReplyIcon;

        // 6g
        private Icon _ReplyIconBlink;

        private PostClass anchorPost;

        // True:関連発言移動中（関連移動以外のオペレーションをするとFalseへ。Trueだとリスト背景色をアンカー発言選択中として描画）
        private bool anchorFlag;

        // 発言履歴
        private List<PostingStatus> _history = new List<PostingStatus>();

        // 発言履歴カレントインデックス
        private int _hisIdx;

        // 発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
        // リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
        private long _replyToId;

        // リプライ先ステータスの書き込み者の名前
        private string _replyToName;

        // 時速表示用
        private List<DateTime> _postTimestamps = new List<DateTime>();

        private List<DateTime> _favTimestamps = new List<DateTime>();
        private Dictionary<DateTime, int> timeLineTimestamps = new Dictionary<DateTime, int>();
        private int timeLineCount;

        // 以下DrawItem関連

        private SolidBrush _brsHighLight = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private SolidBrush _brsHighLightText = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
        private SolidBrush _brsForeColorUnread;
        private SolidBrush _brsForeColorReaded;
        private SolidBrush _brsForeColorFav;
        private SolidBrush _brsForeColorOWL;
        private SolidBrush _brsForeColorRetweet;
        private SolidBrush _brsBackColorMine;
        private SolidBrush _brsBackColorAt;
        private SolidBrush _brsBackColorYou;
        private SolidBrush _brsBackColorAtYou;
        private SolidBrush _brsBackColorAtFromTarget;
        private SolidBrush _brsBackColorAtTo;
        private SolidBrush _brsBackColorNone;

        // Listにフォーカスないときの選択行の背景色
        private SolidBrush _brsDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace));

        private StringFormat tabStringFormat = new StringFormat();

        private ToolStripAPIGauge _apiGauge = new ToolStripAPIGauge();

        private TabInformations _statuses;
        private ListViewItem[] _itemCache;
        private int _itemCacheIndex;
        private PostClass[] _postCache;
        private TabPage _curTab;
        private int _curItemIndex;
        private DetailsListView _curList;
        private PostClass _curPost;
        private bool _isColumnChanged = false;
        private bool _waitTimeline = false;
        private bool _waitReply = false;
        private bool _waitDm = false;
        private bool _waitFav = false;
        private bool _waitPubSearch = false;
        private bool _waitUserTimeline = false;
        private bool _waitLists = false;
        private BackgroundWorker[] _bw = new BackgroundWorker[19];
        private BackgroundWorker followerFetchWorker;
        private ShieldIcon _shield = new ShieldIcon();
        private InternetSecurityManager _securityManager;
        private Thumbnail _thumbnail;
        private int _unreadCounter = -1;
        private int _unreadAtCounter = -1;
        private string[] _columnOrgTexts = new string[9];
        private string[] _columnTexts = new string[9];
        private bool _DoFavRetweetFlags = false;
        private bool isOsResumed = false;
        private Dictionary<string, IMultimediaShareService> _pictureServices;
        private string _postBrowserStatusText = string.Empty;
        private bool _colorize = false;
        private System.Timers.Timer withEventsField_TimerTimeline = new System.Timers.Timer();

        private System.Timers.Timer TimerTimeline
        {
            get
            {
                return this.withEventsField_TimerTimeline;
            }

            set
            {
                if (this.withEventsField_TimerTimeline != null)
                {
                    this.withEventsField_TimerTimeline.Elapsed -= this.TimerTimeline_Elapsed;
                }
                this.withEventsField_TimerTimeline = value;
                if (this.withEventsField_TimerTimeline != null)
                {
                    this.withEventsField_TimerTimeline.Elapsed += this.TimerTimeline_Elapsed;
                }
            }
        }

        private ImageListViewItem displayItem;

        // URL短縮のUndo用
        private struct UrlUndoInfo
        {
            public string Before;
            public string After;
        }

        private List<UrlUndoInfo> urlUndoBuffer = null;

        private class ReplyChain
        {
            public long OriginalId;
            public long InReplyToId;
            public TabPage OriginalTab;

            public ReplyChain(long originalId, long inReplyToId, TabPage originalTab)
            {
                this.OriginalId = originalId;
                this.InReplyToId = inReplyToId;
                this.OriginalTab = originalTab;
            }
        }

        // [, ]でのリプライ移動の履歴
        private Stack<ReplyChain> _replyChains;

        // ポスト選択履歴
        private Stack<Tuple<TabPage, PostClass>> _selectPostChains = new Stack<Tuple<TabPage, PostClass>>();

        // Backgroundworkerの処理結果通知用引数構造体
        private class GetWorkerResult
        {
            // 処理結果詳細メッセージ。エラー時に値がセットされる
            public string RetMsg = string.Empty;

            // 取得対象ページ番号
            public int Page;

            // 取得終了ページ番号（継続可能ならインクリメントされて返る。pageと比較して継続判定）
            public int EndPage;

            // 処理種別
            public WorkerType WorkerType;

            // 新規取得したアイコンイメージ
            public Dictionary<string, Image> Imgs;

            // Fav追加・削除時のタブ名
            public string TabName = string.Empty;

            // Fav追加・削除時のID
            public List<long> Ids;

            // Fav追加・削除成功分のID
            public List<long> SIds;

            public bool NewDM;
            public int AddCount;
            public PostingStatus PStatus;
        }

        // Backgroundworkerへ処理内容を通知するための引数用構造体
        private class GetWorkerArg
        {
            // 処理対象ページ番号
            public int Page;

            // 処理終了ページ番号（起動時の読み込みページ数。通常時はpageと同じ値をセット）
            public int EndPage;

            // 処理種別
            public WorkerType WorkerType;

            // URLをブラウザで開くときのアドレス
            public string Url = string.Empty;

            // 発言POST時の発言内容
            public PostingStatus PStatus = new PostingStatus();

            // Fav追加・削除時のItemIndex
            public List<long> Ids;

            // Fav追加・削除成功分のItemIndex
            public List<long> SIds;

            // Fav追加・削除時のタブ名
            public string TabName = string.Empty;
        }

        // 検索処理タイプ
        private enum SEARCHTYPE
        {
            DialogSearch,
            NextSearch,
            PrevSearch
        }

        private class PostingStatus
        {
            public string Status = string.Empty;
            public long InReplyToId = 0;
            public string InReplyToName = string.Empty;

            // 画像投稿サービス名
            public string ImageService = string.Empty;

            public string ImagePath = string.Empty;

            public PostingStatus()
            {
            }

            public PostingStatus(string status, long replyToId, string replyToName)
            {
                this.Status = status;
                this.InReplyToId = replyToId;
                this.InReplyToName = replyToName;
            }
        }

        private class SpaceKeyCanceler : NativeWindow, IDisposable
        {
            const int WM_KEYDOWN = 0x100;
            const int VK_SPACE = 0x20;

            public SpaceKeyCanceler(Control control)
            {
                this.AssignHandle(control.Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if ((m.Msg == WM_KEYDOWN) && (Convert.ToInt32(m.WParam) == VK_SPACE))
                {
                    if (this.SpaceCancel != null)
                    {
                        this.SpaceCancel(this, EventArgs.Empty);
                    }
                    return;
                }

                base.WndProc(ref m);
            }

            public event EventHandler SpaceCancel;

            public void Dispose()
            {
                this.ReleaseHandle();
            }
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            // 画面がアクティブになったら、発言欄の背景色戻す
            if (StatusText.Focused)
            {
                this.StatusText_Enter(this.StatusText, EventArgs.Empty);
            }
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            // 後始末
            this.SettingDialog.Dispose();
            this.TabDialog.Dispose();
            this.SearchDialog.Dispose();
            this.fltDialog.Dispose();
            this.UrlDialog.Dispose();
            this._spaceKeyCanceler.Dispose();
            if (this._NIconAt != null)
            {
                this._NIconAt.Dispose();
            }
            if (this._NIconAtRed != null)
            {
                this._NIconAtRed.Dispose();
            }
            if (this._NIconAtSmoke != null)
            {
                this._NIconAtSmoke.Dispose();
            }
            if (this._NIconRefresh[0] != null)
            {
                this._NIconRefresh[0].Dispose();
            }
            if (this._NIconRefresh[1] != null)
            {
                this._NIconRefresh[1].Dispose();
            }
            if (this._NIconRefresh[2] != null)
            {
                this._NIconRefresh[2].Dispose();
            }
            if (this._NIconRefresh[3] != null)
            {
                this._NIconRefresh[3].Dispose();
            }
            if (this._TabIcon != null)
            {
                this._TabIcon.Dispose();
            }
            if (this._MainIcon != null)
            {
                this._MainIcon.Dispose();
            }
            if (this._ReplyIcon != null)
            {
                this._ReplyIcon.Dispose();
            }
            if (this._ReplyIconBlink != null)
            {
                this._ReplyIconBlink.Dispose();
            }
            this._brsHighLight.Dispose();
            this._brsHighLightText.Dispose();
            if (this._brsForeColorUnread != null)
            {
                this._brsForeColorUnread.Dispose();
            }
            if (this._brsForeColorReaded != null)
            {
                this._brsForeColorReaded.Dispose();
            }
            if (this._brsForeColorFav != null)
            {
                this._brsForeColorFav.Dispose();
            }
            if (this._brsForeColorOWL != null)
            {
                this._brsForeColorOWL.Dispose();
            }
            if (this._brsForeColorRetweet != null)
            {
                this._brsForeColorRetweet.Dispose();
            }
            if (this._brsBackColorMine != null)
            {
                this._brsBackColorMine.Dispose();
            }
            if (this._brsBackColorAt != null)
            {
                this._brsBackColorAt.Dispose();
            }
            if (this._brsBackColorYou != null)
            {
                this._brsBackColorYou.Dispose();
            }
            if (this._brsBackColorAtYou != null)
            {
                this._brsBackColorAtYou.Dispose();
            }
            if (this._brsBackColorAtFromTarget != null)
            {
                this._brsBackColorAtFromTarget.Dispose();
            }
            if (this._brsBackColorAtTo != null)
            {
                this._brsBackColorAtTo.Dispose();
            }
            if (this._brsBackColorNone != null)
            {
                this._brsBackColorNone.Dispose();
            }
            if (this._brsDeactiveSelection != null)
            {
                this._brsDeactiveSelection.Dispose();
            }
            this._shield.Dispose();
            this.tabStringFormat.Dispose();
            foreach (BackgroundWorker bw in this._bw)
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
            this._apiGauge.Dispose();
            if (this._TIconDic != null)
            {
                ((ImageDictionary)this._TIconDic).PauseGetImage = true;
                ((IDisposable)this._TIconDic).Dispose();
            }
            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
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

            this._NIconAt = Hoehoe.Properties.Resources.At;
            this._NIconAtRed = Hoehoe.Properties.Resources.AtRed;
            this._NIconAtSmoke = Hoehoe.Properties.Resources.AtSmoke;
            this._NIconRefresh[0] = Hoehoe.Properties.Resources.Refresh;
            this._NIconRefresh[1] = Hoehoe.Properties.Resources.Refresh2;
            this._NIconRefresh[2] = Hoehoe.Properties.Resources.Refresh3;
            this._NIconRefresh[3] = Hoehoe.Properties.Resources.Refresh4;
            this._TabIcon = Hoehoe.Properties.Resources.TabIcon;
            this._MainIcon = Hoehoe.Properties.Resources.MIcon;
            this._ReplyIcon = Hoehoe.Properties.Resources.Reply;
            this._ReplyIconBlink = Hoehoe.Properties.Resources.ReplyBlink;

            if (!Directory.Exists(Path.Combine(dir, "Icons")))
            {
                return;
            }

            this.LoadIcon(ref this._NIconAt, "Icons\\At.ico");

            // タスクトレイエラー時アイコン
            this.LoadIcon(ref this._NIconAtRed, "Icons\\AtRed.ico");

            // タスクトレイオフライン時アイコン
            this.LoadIcon(ref this._NIconAtSmoke, "Icons\\AtSmoke.ico");

            // タスクトレイ更新中アイコン
            // アニメーション対応により4種類読み込み
            this.LoadIcon(ref this._NIconRefresh[0], "Icons\\Refresh.ico");
            this.LoadIcon(ref this._NIconRefresh[1], "Icons\\Refresh2.ico");
            this.LoadIcon(ref this._NIconRefresh[2], "Icons\\Refresh3.ico");
            this.LoadIcon(ref this._NIconRefresh[3], "Icons\\Refresh4.ico");

            // タブ見出し未読表示アイコン
            this.LoadIcon(ref this._TabIcon, "Icons\\Tab.ico");

            // 画面のアイコン
            this.LoadIcon(ref this._MainIcon, "Icons\\MIcon.ico");

            // Replyのアイコン
            this.LoadIcon(ref this._ReplyIcon, "Icons\\Reply.ico");

            // Reply点滅のアイコン
            this.LoadIcon(ref this._ReplyIconBlink, "Icons\\ReplyBlink.ico");
        }

        private void InitColumnText()
        {
            this._columnTexts[0] = string.Empty;
            this._columnTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            this._columnTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            this._columnTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            this._columnTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            this._columnTexts[5] = string.Empty;
            this._columnTexts[6] = string.Empty;
            this._columnTexts[7] = "Source";

            this._columnOrgTexts[0] = string.Empty;
            this._columnOrgTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            this._columnOrgTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            this._columnOrgTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            this._columnOrgTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            this._columnOrgTexts[5] = string.Empty;
            this._columnOrgTexts[6] = string.Empty;
            this._columnOrgTexts[7] = "Source";

            int c = 0;
            switch (this._statuses.SortMode)
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

            if (this._iconCol)
            {
                if (this._statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this._columnTexts[2] = this._columnOrgTexts[2] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this._columnTexts[2] = this._columnOrgTexts[2] + "▴";
                }
            }
            else
            {
                if (this._statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this._columnTexts[c] = this._columnOrgTexts[c] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this._columnTexts[c] = this._columnOrgTexts[c] + "▴";
                }
            }
        }

        private void InitializeTraceFrag()
        {
#if DEBUG
			TraceOutToolStripMenuItem.Checked = true;
			MyCommon.TraceFlag = true;
#endif
            if (!MyCommon.fileVersion.EndsWith("0"))
            {
                TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this._ignoreConfigSave = true;
            this.Visible = false;

            this._securityManager = new InternetSecurityManager(PostBrowser);
            this._thumbnail = new Thumbnail(this);

            MyCommon.TwitterApiInfo.Changed += this.SetStatusLabelApiHandler;
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;

            VerUpMenuItem.Image = this._shield.Icon;
            var cmdArgs = System.Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (!(cmdArgs.Length == 0) && cmdArgs.Contains("/d"))
            {
                MyCommon.TraceFlag = true;
            }

            this._spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this._spaceKeyCanceler.SpaceCancel += this.SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            // MyCommon.fileVersion = ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version;
            this.InitializeTraceFrag();
            this.LoadIcons();
            // アイコン読み込み

            // 発言保持クラス
            this._statuses = TabInformations.GetInstance();

            // アイコン設定
            this.Icon = this._MainIcon;
            // メインフォーム（TweenMain）
            NotifyIcon1.Icon = this._NIconAt;
            // タスクトレイ
            TabImage.Images.Add(this._TabIcon);
            // タブ見出し

            this.SettingDialog.Owner = this;
            this.SearchDialog.Owner = this;
            this.fltDialog.Owner = this;
            this.TabDialog.Owner = this;
            this.UrlDialog.Owner = this;

            this._history.Add(new PostingStatus());
            this._hisIdx = 0;
            this._replyToId = 0;
            this._replyToName = string.Empty;

            // <<<<<<<<<設定関連>>>>>>>>>
            // '設定読み出し
            this.LoadConfig();

            // 新着バルーン通知のチェック状態設定
            NewPostPopMenuItem.Checked = this._cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = NewPostPopMenuItem.Checked;

            // フォント＆文字色＆背景色保持
            this._fntUnread = this._cfgLocal.FontUnread;
            this.clrUnread = this._cfgLocal.ColorUnread;
            this._fntReaded = this._cfgLocal.FontRead;
            this.clrRead = this._cfgLocal.ColorRead;
            this.clrFav = this._cfgLocal.ColorFav;
            this.clrOWL = this._cfgLocal.ColorOWL;
            this.clrRetweet = this._cfgLocal.ColorRetweet;
            this._fntDetail = this._cfgLocal.FontDetail;
            this.clrDetail = this._cfgLocal.ColorDetail;
            this.clrDetailLink = this._cfgLocal.ColorDetailLink;
            this.clrDetailBackcolor = this._cfgLocal.ColorDetailBackcolor;
            this.clrSelf = this._cfgLocal.ColorSelf;
            this.clrAtSelf = this._cfgLocal.ColorAtSelf;
            this.clrTarget = this._cfgLocal.ColorTarget;
            this.clrAtTarget = this._cfgLocal.ColorAtTarget;
            this.clrAtFromTarget = this._cfgLocal.ColorAtFromTarget;
            this.clrAtTo = this._cfgLocal.ColorAtTo;
            this.clrListBackcolor = this._cfgLocal.ColorListBackcolor;
            this.clrInputBackcolor = this._cfgLocal.ColorInputBackcolor;
            this.clrInputForecolor = this._cfgLocal.ColorInputFont;
            this._fntInputFont = this._cfgLocal.FontInputFont;

            this._brsForeColorUnread = new SolidBrush(this.clrUnread);
            this._brsForeColorReaded = new SolidBrush(this.clrRead);
            this._brsForeColorFav = new SolidBrush(this.clrFav);
            this._brsForeColorOWL = new SolidBrush(this.clrOWL);
            this._brsForeColorRetweet = new SolidBrush(this.clrRetweet);
            this._brsBackColorMine = new SolidBrush(this.clrSelf);
            this._brsBackColorAt = new SolidBrush(this.clrAtSelf);
            this._brsBackColorYou = new SolidBrush(this.clrTarget);
            this._brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
            this._brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
            this._brsBackColorAtTo = new SolidBrush(this.clrAtTo);
            this._brsBackColorNone = new SolidBrush(this.clrListBackcolor);

            // StringFormatオブジェクトへの事前設定
            this.tabStringFormat.Alignment = StringAlignment.Center;
            this.tabStringFormat.LineAlignment = StringAlignment.Center;

            // 設定画面への反映
            HttpTwitter.SetTwitterUrl(this._cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this._cfgCommon.TwitterSearchUrl);
            this.SettingDialog.TwitterApiUrl = this._cfgCommon.TwitterUrl;
            this.SettingDialog.TwitterSearchApiUrl = this._cfgCommon.TwitterSearchUrl;

            // 認証関連
            if (string.IsNullOrEmpty(this._cfgCommon.Token))
            {
                this._cfgCommon.UserName = string.Empty;
            }
            this.tw.Initialize(this._cfgCommon.Token, this._cfgCommon.TokenSecret, this._cfgCommon.UserName, this._cfgCommon.UserId);

            this.SettingDialog.UserAccounts = this._cfgCommon.UserAccounts;
            this.SettingDialog.TimelinePeriodInt = this._cfgCommon.TimelinePeriod;
            this.SettingDialog.ReplyPeriodInt = this._cfgCommon.ReplyPeriod;
            this.SettingDialog.DMPeriodInt = this._cfgCommon.DMPeriod;
            this.SettingDialog.PubSearchPeriodInt = this._cfgCommon.PubSearchPeriod;
            this.SettingDialog.UserTimelinePeriodInt = this._cfgCommon.UserTimelinePeriod;
            this.SettingDialog.ListsPeriodInt = this._cfgCommon.ListsPeriod;
            // 不正値チェック
            if (!cmdArgs.Contains("nolimit"))
            {
                if (this.SettingDialog.TimelinePeriodInt < 15 && this.SettingDialog.TimelinePeriodInt > 0)
                {
                    this.SettingDialog.TimelinePeriodInt = 15;
                }
                if (this.SettingDialog.ReplyPeriodInt < 15 && this.SettingDialog.ReplyPeriodInt > 0)
                {
                    this.SettingDialog.ReplyPeriodInt = 15;
                }
                if (this.SettingDialog.DMPeriodInt < 15 && this.SettingDialog.DMPeriodInt > 0)
                {
                    this.SettingDialog.DMPeriodInt = 15;
                }
                if (this.SettingDialog.PubSearchPeriodInt < 30 && this.SettingDialog.PubSearchPeriodInt > 0)
                {
                    this.SettingDialog.PubSearchPeriodInt = 30;
                }
                if (this.SettingDialog.UserTimelinePeriodInt < 15 && this.SettingDialog.UserTimelinePeriodInt > 0)
                {
                    this.SettingDialog.UserTimelinePeriodInt = 15;
                }
                if (this.SettingDialog.ListsPeriodInt < 15 && this.SettingDialog.ListsPeriodInt > 0)
                {
                    this.SettingDialog.ListsPeriodInt = 15;
                }
            }

            // 起動時読み込み分を既読にするか。Trueなら既読として処理
            this.SettingDialog.Readed = this._cfgCommon.Read;
            // 新着取得時のリストスクロールをするか。Trueならスクロールしない
            ListLockMenuItem.Checked = this._cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = this._cfgCommon.ListLock;
            this.SettingDialog.IconSz = this._cfgCommon.IconSize;
            // 文末ステータス
            this.SettingDialog.Status = this._cfgLocal.StatusText;
            // 未読管理。Trueなら未読管理する
            this.SettingDialog.UnreadManage = this._cfgCommon.UnreadManage;
            // サウンド再生（タブ別設定より優先）
            this.SettingDialog.PlaySound = this._cfgCommon.PlaySound;
            PlaySoundMenuItem.Checked = this.SettingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.SettingDialog.PlaySound;
            // 片思い表示。Trueなら片思い表示する
            this.SettingDialog.OneWayLove = this._cfgCommon.OneWayLove;
            // フォント＆文字色＆背景色
            this.SettingDialog.FontUnread = this._fntUnread;
            this.SettingDialog.ColorUnread = this.clrUnread;
            this.SettingDialog.FontReaded = this._fntReaded;
            this.SettingDialog.ColorReaded = this.clrRead;
            this.SettingDialog.ColorFav = this.clrFav;
            this.SettingDialog.ColorOWL = this.clrOWL;
            this.SettingDialog.ColorRetweet = this.clrRetweet;
            this.SettingDialog.FontDetail = this._fntDetail;
            this.SettingDialog.ColorDetail = this.clrDetail;
            this.SettingDialog.ColorDetailLink = this.clrDetailLink;
            this.SettingDialog.ColorDetailBackcolor = this.clrDetailBackcolor;
            this.SettingDialog.ColorSelf = this.clrSelf;
            this.SettingDialog.ColorAtSelf = this.clrAtSelf;
            this.SettingDialog.ColorTarget = this.clrTarget;
            this.SettingDialog.ColorAtTarget = this.clrAtTarget;
            this.SettingDialog.ColorAtFromTarget = this.clrAtFromTarget;
            this.SettingDialog.ColorAtTo = this.clrAtTo;
            this.SettingDialog.ColorListBackcolor = this.clrListBackcolor;
            this.SettingDialog.ColorInputBackcolor = this.clrInputBackcolor;
            this.SettingDialog.ColorInputFont = this.clrInputForecolor;
            this.SettingDialog.FontInputFont = this._fntInputFont;
            this.SettingDialog.NameBalloon = this._cfgCommon.NameBalloon;
            this.SettingDialog.PostCtrlEnter = this._cfgCommon.PostCtrlEnter;
            this.SettingDialog.PostShiftEnter = this._cfgCommon.PostShiftEnter;
            this.SettingDialog.CountApi = this._cfgCommon.CountApi;
            this.SettingDialog.CountApiReply = this._cfgCommon.CountApiReply;
            if (this.SettingDialog.CountApi < 20 || this.SettingDialog.CountApi > 200)
            {
                this.SettingDialog.CountApi = 60;
            }
            if (this.SettingDialog.CountApiReply < 20 || this.SettingDialog.CountApiReply > 200)
            {
                this.SettingDialog.CountApiReply = 40;
            }

            this.SettingDialog.BrowserPath = this._cfgLocal.BrowserPath;
            this.SettingDialog.PostAndGet = this._cfgCommon.PostAndGet;
            this.SettingDialog.UseRecommendStatus = this._cfgLocal.UseRecommendStatus;
            this.SettingDialog.DispUsername = this._cfgCommon.DispUsername;
            this.SettingDialog.CloseToExit = this._cfgCommon.CloseToExit;
            this.SettingDialog.MinimizeToTray = this._cfgCommon.MinimizeToTray;
            this.SettingDialog.DispLatestPost = this._cfgCommon.DispLatestPost;
            this.SettingDialog.SortOrderLock = this._cfgCommon.SortOrderLock;
            this.SettingDialog.TinyUrlResolve = this._cfgCommon.TinyUrlResolve;
            this.SettingDialog.ShortUrlForceResolve = this._cfgCommon.ShortUrlForceResolve;
            this.SettingDialog.SelectedProxyType = this._cfgLocal.ProxyType;
            this.SettingDialog.ProxyAddress = this._cfgLocal.ProxyAddress;
            this.SettingDialog.ProxyPort = this._cfgLocal.ProxyPort;
            this.SettingDialog.ProxyUser = this._cfgLocal.ProxyUser;
            this.SettingDialog.ProxyPassword = this._cfgLocal.ProxyPassword;
            this.SettingDialog.PeriodAdjust = this._cfgCommon.PeriodAdjust;
            this.SettingDialog.StartupVersion = this._cfgCommon.StartupVersion;
            this.SettingDialog.StartupFollowers = this._cfgCommon.StartupFollowers;
            this.SettingDialog.RestrictFavCheck = this._cfgCommon.RestrictFavCheck;
            this.SettingDialog.AlwaysTop = this._cfgCommon.AlwaysTop;
            this.SettingDialog.UrlConvertAuto = false;
            this.SettingDialog.OutputzEnabled = this._cfgCommon.Outputz;
            this.SettingDialog.OutputzKey = this._cfgCommon.OutputzKey;
            this.SettingDialog.OutputzUrlmode = this._cfgCommon.OutputzUrlMode;
            this.SettingDialog.UseUnreadStyle = this._cfgCommon.UseUnreadStyle;
            this.SettingDialog.DefaultTimeOut = this._cfgCommon.DefaultTimeOut;
            this.SettingDialog.RetweetNoConfirm = this._cfgCommon.RetweetNoConfirm;
            this.SettingDialog.PlaySound = this._cfgCommon.PlaySound;
            this.SettingDialog.DateTimeFormat = this._cfgCommon.DateTimeFormat;
            this.SettingDialog.LimitBalloon = this._cfgCommon.LimitBalloon;
            this.SettingDialog.EventNotifyEnabled = this._cfgCommon.EventNotifyEnabled;
            this.SettingDialog.EventNotifyFlag = this._cfgCommon.EventNotifyFlag;
            this.SettingDialog.IsMyEventNotifyFlag = this._cfgCommon.IsMyEventNotifyFlag;
            this.SettingDialog.ForceEventNotify = this._cfgCommon.ForceEventNotify;
            this.SettingDialog.FavEventUnread = this._cfgCommon.FavEventUnread;
            this.SettingDialog.TranslateLanguage = this._cfgCommon.TranslateLanguage;
            this.SettingDialog.EventSoundFile = this._cfgCommon.EventSoundFile;

            // 廃止サービスが選択されていた場合bit.lyへ読み替え
            if (this._cfgCommon.AutoShortUrlFirst < 0)
            {
                this._cfgCommon.AutoShortUrlFirst = UrlConverter.Bitly;
            }

            this.SettingDialog.AutoShortUrlFirst = this._cfgCommon.AutoShortUrlFirst;
            this.SettingDialog.TabIconDisp = this._cfgCommon.TabIconDisp;
            this.SettingDialog.ReplyIconState = this._cfgCommon.ReplyIconState;
            this.SettingDialog.ReadOwnPost = this._cfgCommon.ReadOwnPost;
            this.SettingDialog.GetFav = this._cfgCommon.GetFav;
            this.SettingDialog.ReadOldPosts = this._cfgCommon.ReadOldPosts;
            this.SettingDialog.UseSsl = this._cfgCommon.UseSsl;
            this.SettingDialog.BitlyUser = this._cfgCommon.BilyUser;
            this.SettingDialog.BitlyPwd = this._cfgCommon.BitlyPwd;
            this.SettingDialog.ShowGrid = this._cfgCommon.ShowGrid;
            this.SettingDialog.Language = this._cfgCommon.Language;
            this.SettingDialog.UseAtIdSupplement = this._cfgCommon.UseAtIdSupplement;
            this.SettingDialog.UseHashSupplement = this._cfgCommon.UseHashSupplement;
            this.SettingDialog.PreviewEnable = this._cfgCommon.PreviewEnable;
            this.AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            this.SettingDialog.IsMonospace = this._cfgCommon.IsMonospace;
            if (this.SettingDialog.IsMonospace)
            {
                this._detailHtmlFormatHeader = DetailHtmlFormatMono1;
                this._detailHtmlFormatFooter = DetailHtmlFormatMono7;
            }
            else
            {
                this._detailHtmlFormatHeader = DetailHtmlFormat1;
                this._detailHtmlFormatFooter = DetailHtmlFormat7;
            }
            this._detailHtmlFormatHeader += this._fntDetail.Name + DetailHtmlFormat2 + this._fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
            if (this.SettingDialog.IsMonospace)
            {
                this._detailHtmlFormatHeader += DetailHtmlFormatMono6;
            }
            else
            {
                this._detailHtmlFormatHeader += DetailHtmlFormat6;
            }
            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = this._cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = this._cfgCommon.FocusLockToStatusText;

            this.SettingDialog.RecommendStatusText = " [TWNv" + Regex.Replace(MyCommon.fileVersion.Replace(".", string.Empty), "^0*", string.Empty) + "]";

            // 書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(this.SettingDialog.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    this.SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                this.SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            this.SettingDialog.Nicoms = this._cfgCommon.Nicoms;
            this.SettingDialog.HotkeyEnabled = this._cfgCommon.HotkeyEnabled;
            this.SettingDialog.HotkeyMod = this._cfgCommon.HotkeyModifier;
            this.SettingDialog.HotkeyKey = this._cfgCommon.HotkeyKey;
            this.SettingDialog.HotkeyValue = this._cfgCommon.HotkeyValue;
            this.SettingDialog.BlinkNewMentions = this._cfgCommon.BlinkNewMentions;
            this.SettingDialog.UseAdditionalCount = this._cfgCommon.UseAdditionalCount;
            this.SettingDialog.MoreCountApi = this._cfgCommon.MoreCountApi;
            this.SettingDialog.FirstCountApi = this._cfgCommon.FirstCountApi;
            this.SettingDialog.SearchCountApi = this._cfgCommon.SearchCountApi;
            this.SettingDialog.FavoritesCountApi = this._cfgCommon.FavoritesCountApi;
            this.SettingDialog.UserTimelineCountApi = this._cfgCommon.UserTimelineCountApi;
            this.SettingDialog.ListCountApi = this._cfgCommon.ListCountApi;
            this.SettingDialog.UserstreamStartup = this._cfgCommon.UserstreamStartup;
            this.SettingDialog.UserstreamPeriodInt = this._cfgCommon.UserstreamPeriod;
            this.SettingDialog.OpenUserTimeline = this._cfgCommon.OpenUserTimeline;
            this.SettingDialog.ListDoubleClickAction = this._cfgCommon.ListDoubleClickAction;
            this.SettingDialog.UserAppointUrl = this._cfgCommon.UserAppointUrl;
            this.SettingDialog.HideDuplicatedRetweets = this._cfgCommon.HideDuplicatedRetweets;
            this.SettingDialog.IsPreviewFoursquare = this._cfgCommon.IsPreviewFoursquare;
            this.SettingDialog.FoursquarePreviewHeight = this._cfgCommon.FoursquarePreviewHeight;
            this.SettingDialog.FoursquarePreviewWidth = this._cfgCommon.FoursquarePreviewWidth;
            this.SettingDialog.FoursquarePreviewZoom = this._cfgCommon.FoursquarePreviewZoom;
            this.SettingDialog.IsListStatusesIncludeRts = this._cfgCommon.IsListsIncludeRts;
            this.SettingDialog.TabMouseLock = this._cfgCommon.TabMouseLock;
            this.SettingDialog.IsRemoveSameEvent = this._cfgCommon.IsRemoveSameEvent;
            this.SettingDialog.IsNotifyUseGrowl = this._cfgCommon.IsUseNotifyGrowl;

            // ハッシュタグ関連
            this.HashSupl = new AtIdSupplement(this._cfgCommon.HashTags, "#");
            this.HashMgr = new HashtagManage(this.HashSupl, this._cfgCommon.HashTags.ToArray(), this._cfgCommon.HashSelected, this._cfgCommon.HashIsPermanent, this._cfgCommon.HashIsHead, this._cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.HashMgr.IsPermanent)
            {
                HashStripSplitButton.Text = this.HashMgr.UseHash;
            }

            this._initial = true;

            // アイコンリスト作成
            try
            {
                this._TIconDic = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }
            ((ImageDictionary)this._TIconDic).PauseGetImage = false;

            bool saveRequired = false;
            // ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(this.tw.Username))
            {
                saveRequired = true;
                // 設定せずにキャンセルされた場合はプログラム終了
                if (this.SettingDialog.ShowDialog(this) == DialogResult.Cancel)
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
                // 新しい設定を反映
                // フォント＆文字色＆背景色保持
                this._fntUnread = this.SettingDialog.FontUnread;
                this.clrUnread = this.SettingDialog.ColorUnread;
                this._fntReaded = this.SettingDialog.FontReaded;
                this.clrRead = this.SettingDialog.ColorReaded;
                this.clrFav = this.SettingDialog.ColorFav;
                this.clrOWL = this.SettingDialog.ColorOWL;
                this.clrRetweet = this.SettingDialog.ColorRetweet;
                this._fntDetail = this.SettingDialog.FontDetail;
                this.clrDetail = this.SettingDialog.ColorDetail;
                this.clrDetailLink = this.SettingDialog.ColorDetailLink;
                this.clrDetailBackcolor = this.SettingDialog.ColorDetailBackcolor;
                this.clrSelf = this.SettingDialog.ColorSelf;
                this.clrAtSelf = this.SettingDialog.ColorAtSelf;
                this.clrTarget = this.SettingDialog.ColorTarget;
                this.clrAtTarget = this.SettingDialog.ColorAtTarget;
                this.clrAtFromTarget = this.SettingDialog.ColorAtFromTarget;
                this.clrAtTo = this.SettingDialog.ColorAtTo;
                this.clrListBackcolor = this.SettingDialog.ColorListBackcolor;
                this.clrInputBackcolor = this.SettingDialog.ColorInputBackcolor;
                this.clrInputForecolor = this.SettingDialog.ColorInputFont;
                this._fntInputFont = this.SettingDialog.FontInputFont;
                this._brsForeColorUnread.Dispose();
                this._brsForeColorReaded.Dispose();
                this._brsForeColorFav.Dispose();
                this._brsForeColorOWL.Dispose();
                this._brsForeColorRetweet.Dispose();
                this._brsForeColorUnread = new SolidBrush(this.clrUnread);
                this._brsForeColorReaded = new SolidBrush(this.clrRead);
                this._brsForeColorFav = new SolidBrush(this.clrFav);
                this._brsForeColorOWL = new SolidBrush(this.clrOWL);
                this._brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                this._brsBackColorMine.Dispose();
                this._brsBackColorAt.Dispose();
                this._brsBackColorYou.Dispose();
                this._brsBackColorAtYou.Dispose();
                this._brsBackColorAtFromTarget.Dispose();
                this._brsBackColorAtTo.Dispose();
                this._brsBackColorNone.Dispose();
                this._brsBackColorMine = new SolidBrush(this.clrSelf);
                this._brsBackColorAt = new SolidBrush(this.clrAtSelf);
                this._brsBackColorYou = new SolidBrush(this.clrTarget);
                this._brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                this._brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                this._brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                this._brsBackColorNone = new SolidBrush(this.clrListBackcolor);

                if (this.SettingDialog.IsMonospace)
                {
                    this._detailHtmlFormatHeader = DetailHtmlFormatMono1;
                    this._detailHtmlFormatFooter = DetailHtmlFormatMono7;
                }
                else
                {
                    this._detailHtmlFormatHeader = DetailHtmlFormat1;
                    this._detailHtmlFormatFooter = DetailHtmlFormat7;
                }
                this._detailHtmlFormatHeader += this._fntDetail.Name + DetailHtmlFormat2 + this._fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                if (this.SettingDialog.IsMonospace)
                {
                    this._detailHtmlFormatHeader += DetailHtmlFormatMono6;
                }
                else
                {
                    this._detailHtmlFormatHeader += DetailHtmlFormat6;
                }
                // 他の設定項目は、随時設定画面で保持している値を読み出して使用
            }

            if (this.SettingDialog.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((this.SettingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }
                if ((this.SettingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }
                if ((this.SettingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }
                if ((this.SettingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                this._hookGlobalHotkey.RegisterOriginalHotkey(this.SettingDialog.HotkeyKey, this.SettingDialog.HotkeyValue, modKey);
            }

            // Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(this.SettingDialog.DefaultTimeOut, this.SettingDialog.SelectedProxyType, this.SettingDialog.ProxyAddress, this.SettingDialog.ProxyPort, this.SettingDialog.ProxyUser, this.SettingDialog.ProxyPassword);

            this.tw.SetRestrictFavCheck(this.SettingDialog.RestrictFavCheck);
            this.tw.ReadOwnPost = this.SettingDialog.ReadOwnPost;
            this.tw.SetUseSsl(this.SettingDialog.UseSsl);
            ShortUrl.IsResolve = this.SettingDialog.TinyUrlResolve;
            ShortUrl.IsForceResolve = this.SettingDialog.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(this.SettingDialog.BitlyUser);
            ShortUrl.SetBitlyKey(this.SettingDialog.BitlyPwd);
            HttpTwitter.SetTwitterUrl(this._cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this._cfgCommon.TwitterSearchUrl);
            this.tw.TrackWord = this._cfgCommon.TrackWord;
            TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this.tw.TrackWord);
            this.tw.AllAtReply = this._cfgCommon.AllAtReply;
            AllrepliesToolStripMenuItem.Checked = this.tw.AllAtReply;

            Outputz.Key = this.SettingDialog.OutputzKey;
            Outputz.Enabled = this.SettingDialog.OutputzEnabled;
            switch (this.SettingDialog.OutputzUrlmode)
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
            ImageSelectionPanel.Enabled = false;

            ImageServiceCombo.SelectedIndex = this._cfgCommon.UseImageService;

            // ウィンドウ設定
            this.ClientSize = this._cfgLocal.FormSize;
            this._mySize = this._cfgLocal.FormSize;
            // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            this._myLoc = this._cfgLocal.FormLocation;
            // タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = this._cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(this._mySize.Width, SystemInformation.CaptionHeight));
                bool outOfScreen = true;
                // ハングするとの報告
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
                        this._myLoc = this.DesktopLocation;
                    }
                }
            }
            this.TopMost = this.SettingDialog.AlwaysTop;
            this._mySpDis = this._cfgLocal.SplitterDistance;
            this._mySpDis2 = this._cfgLocal.StatusTextHeight;
            this._mySpDis3 = this._cfgLocal.PreviewDistance;
            if (this._mySpDis3 == -1)
            {
                this._mySpDis3 = this._mySize.Width - 150;
                if (this._mySpDis3 < 1)
                {
                    this._mySpDis3 = 50;
                }
                this._cfgLocal.PreviewDistance = this._mySpDis3;
            }
            this._myAdSpDis = this._cfgLocal.AdSplitterDistance;
            MultiLineMenuItem.Checked = this._cfgLocal.StatusMultiline;
            // Me.Tween_ClientSizeChanged(Me, Nothing)
            PlaySoundMenuItem.Checked = this.SettingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.SettingDialog.PlaySound;
            // 入力欄
            StatusText.Font = this._fntInputFont;
            StatusText.ForeColor  = this.clrInputForecolor;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this.SettingDialog.UnreadManage == false)
            {
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }

            if (this.SettingDialog.IsNotifyUseGrowl)
            {
                GrowlHelper.RegisterGrowl();
            }

            // タイマー設定
            this.TimerTimeline.AutoReset = true;
            this.TimerTimeline.SynchronizingObject = this;
            // Recent取得間隔
            this.TimerTimeline.Interval = 1000;
            this.TimerTimeline.Enabled = true;

            // 更新中アイコンアニメーション間隔
            this.TimerRefreshIcon.Interval = 200;
            this.TimerRefreshIcon.Enabled = true;

            // 状態表示部の初期化（画面右下）
            StatusLabel.Text = string.Empty;
            StatusLabel.AutoToolTip = false;
            StatusLabel.ToolTipText = string.Empty;
            // 文字カウンタ初期化
            lblLen.Text = this.GetRestStatusCount(true, false).ToString();

            ///'''''''''''''''''''''''''''''''''''''
            this._statuses.SortOrder = (SortOrder)this._cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (this._cfgCommon.SortColumn)
            {
                case 0:
                case 5:
                case 6:
                    // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                    // ソートしない
                    mode = IdComparerClass.ComparerMode.Id;
                    // Idソートに読み替え
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
            this._statuses.SortMode = mode;
            ///'''''''''''''''''''''''''''''''''''''

            switch (this.SettingDialog.IconSz)
            {
                case IconSizes.IconNone:
                    this._iconSz = 0;
                    break;
                case IconSizes.Icon16:
                    this._iconSz = 16;
                    break;
                case IconSizes.Icon24:
                    this._iconSz = 26;
                    break;
                case IconSizes.Icon48:
                    this._iconSz = 48;
                    break;
                case IconSizes.Icon48_2:
                    this._iconSz = 48;
                    this._iconCol = true;
                    break;
            }
            if (this._iconSz == 0)
            {
                this.tw.SetGetIcon(false);
            }
            else
            {
                this.tw.SetGetIcon(true);
                this.tw.SetIconSize(this._iconSz);
            }
            this.tw.SetTinyUrlResolve(this.SettingDialog.TinyUrlResolve);
            ShortUrl.IsForceResolve = this.SettingDialog.ShortUrlForceResolve;

            // 発言詳細部アイコンをリストアイコンにサイズ変更
            int sz = this._iconSz;
            if (this._iconSz == 0)
            {
                sz = 16;
            }

            this.tw.DetailIcon = this._TIconDic;

            StatusLabel.Text = Hoehoe.Properties.Resources.Form1_LoadText1;
            // 画面右下の状態表示を変更
            StatusLabelUrl.Text = string.Empty;
            // 画面左下のリンク先URL表示部を初期化
            NameLabel.Text = string.Empty;
            // 発言詳細部名前ラベル初期化
            DateTimeLabel.Text = string.Empty;
            // 発言詳細部日時ラベル初期化
            SourceLinkLabel.Text = string.Empty;
            // Source部分初期化

            // <<<<<<<<タブ関連>>>>>>>
            // デフォルトタブの存在チェック、ない場合には追加
            if (this._statuses.GetTabByType(TabUsageType.Home) == null)
            {
                if (!this._statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.RECENT))
                {
                    this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                }
                else
                {
                    this._statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.RECENT].TabType = TabUsageType.Home;
                }
            }
            if (this._statuses.GetTabByType(TabUsageType.Mentions) == null)
            {
                if (!this._statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.REPLY))
                {
                    this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                }
                else
                {
                    this._statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.REPLY].TabType = TabUsageType.Mentions;
                }
            }
            if (this._statuses.GetTabByType(TabUsageType.DirectMessage) == null)
            {
                if (!this._statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.DM))
                {
                    this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                }
                else
                {
                    this._statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.DM].TabType = TabUsageType.DirectMessage;
                }
            }
            if (this._statuses.GetTabByType(TabUsageType.Favorites) == null)
            {
                if (!this._statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.FAV))
                {
                    this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
                }
                else
                {
                    this._statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.FAV].TabType = TabUsageType.Favorites;
                }
            }
            foreach (string tn in this._statuses.Tabs.Keys)
            {
                if (this._statuses.Tabs[tn].TabType == TabUsageType.Undefined)
                {
                    this._statuses.Tabs[tn].TabType = TabUsageType.UserDefined;
                }
                if (!this.AddNewTab(tn, true, this._statuses.Tabs[tn].TabType, this._statuses.Tabs[tn].ListInfo))
                {
                    throw new Exception(Hoehoe.Properties.Resources.TweenMain_LoadText1);
                }
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (this.SettingDialog.MinimizeToTray == false || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }
            this._curTab = ListTab.SelectedTab;
            this._curItemIndex = -1;
            this._curList = (DetailsListView)this._curTab.Tag;
            this.SetMainWindowTitle();
            this.SetNotifyIconText();

            if (this.SettingDialog.TabIconDisp)
            {
                ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                ListTab.DrawItem += this.ListTab_DrawItem;
                ListTab.ImageList = null;
            }

#if UA // = "True"
			ab = new AdsBrowser();
			this.SplitContainer4.Panel2.Controls.Add(ab);
#else
            SplitContainer4.Panel2Collapsed = true;
#endif

            this._ignoreConfigSave = false;
            this.TweenMain_Resize(null, null);
            if (saveRequired)
            {
                this.SaveConfigsAll(false);
            }

            if (this.tw.UserId == 0)
            {
                this.tw.VerifyCredentials();
                foreach (var ua in this._cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == this.tw.Username.ToLower())
                    {
                        ua.UserId = this.tw.UserId;
                        break;
                    }
                }
            }
            foreach (var ua in this.SettingDialog.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == this.tw.Username.ToLower())
                {
                    ua.UserId = this.tw.UserId;
                    break;
                }
            }
        }

        private void CreatePictureServices()
        {
            if (this._pictureServices != null)
            {
                this._pictureServices.Clear();
            }
            this._pictureServices = null;
            this._pictureServices = new Dictionary<string, IMultimediaShareService> 
            {
                { "TwitPic", new TwitPic(this.tw) },
                { "img.ly", new imgly(this.tw) },
                { "yfrog", new yfrog(this.tw) },
                { "lockerz", new Plixi(this.tw) },
                { "Twitter", new TwitterPhoto(this.tw) }
            };
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
                txt = ListTab.TabPages[e.Index].Text;
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
                if (this._statuses.Tabs[txt].UnreadCount > 0)
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

        private void LoadConfig()
        {
            this._cfgCommon = SettingCommon.Load();
            if (this._cfgCommon.UserAccounts == null || this._cfgCommon.UserAccounts.Count == 0)
            {
                this._cfgCommon.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(this._cfgCommon.UserName))
                {
                    this._cfgCommon.UserAccounts.Add(new UserAccount
                    {
                        Username = this._cfgCommon.UserName,
                        UserId = this._cfgCommon.UserId,
                        Token = this._cfgCommon.Token,
                        TokenSecret = this._cfgCommon.TokenSecret
                    });
                }
            }
            this._cfgLocal = SettingLocal.Load();
            List<TabClass> tabs = SettingTabs.Load().Tabs;
            foreach (TabClass tb in tabs)
            {
                try
                {
                    this._statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = this._statuses.GetUniqueTabName();
                    this._statuses.Tabs.Add(tb.TabName, tb);
                }
            }
            if (this._statuses.Tabs.Count == 0)
            {
                this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                this._statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
            }
        }

        private void TimerInterval_Changed(object sender, AppendSettingDialog.IntervalChangedEventArgs e)
        {
            if (!this.TimerTimeline.Enabled)
            {
                return;
            }
            this.ResetTimers = e;
        }

        private AppendSettingDialog.IntervalChangedEventArgs ResetTimers = new AppendSettingDialog.IntervalChangedEventArgs();
        int _timerHomeCounter;
        int _timerMentionCounter;
        int _timerDmCounter;
        int _timerPubSearchCounter;
        int _timerUserTimelineCounter;
        int _timerListsCounter;
        int _timerUsCounter;
        int _timerResumeWait;
        int _timerRefreshFollowers;

        private void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            if (this._timerHomeCounter > 0)
            {
                Interlocked.Decrement(ref this._timerHomeCounter);
            }
            if (this._timerMentionCounter > 0)
            {
                Interlocked.Decrement(ref this._timerMentionCounter);
            }
            if (this._timerDmCounter > 0)
            {
                Interlocked.Decrement(ref this._timerDmCounter);
            }
            if (this._timerPubSearchCounter > 0)
            {
                Interlocked.Decrement(ref this._timerPubSearchCounter);
            }
            if (this._timerUserTimelineCounter > 0)
            {
                Interlocked.Decrement(ref this._timerUserTimelineCounter);
            }
            if (this._timerListsCounter > 0)
            {
                Interlocked.Decrement(ref this._timerListsCounter);
            }
            if (this._timerUsCounter > 0)
            {
                Interlocked.Decrement(ref this._timerUsCounter);
            }
            Interlocked.Increment(ref this._timerRefreshFollowers);

            // 'タイマー初期化
            if (this.ResetTimers.Timeline || this._timerHomeCounter <= 0 && this.SettingDialog.TimelinePeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerHomeCounter, this.SettingDialog.TimelinePeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.ResetTimers.Timeline)
                {
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                }
                this.ResetTimers.Timeline = false;
            }
            if (this.ResetTimers.Reply || this._timerMentionCounter <= 0 && this.SettingDialog.ReplyPeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerMentionCounter, this.SettingDialog.ReplyPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.ResetTimers.Reply)
                {
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                }
                this.ResetTimers.Reply = false;
            }
            if (this.ResetTimers.DirectMessage || this._timerDmCounter <= 0 && this.SettingDialog.DMPeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerDmCounter, this.SettingDialog.DMPeriodInt);
                if (!this.tw.IsUserstreamDataReceived && !this.ResetTimers.DirectMessage)
                {
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                }
                this.ResetTimers.DirectMessage = false;
            }
            if (this.ResetTimers.PublicSearch || this._timerPubSearchCounter <= 0 && this.SettingDialog.PubSearchPeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerPubSearchCounter, this.SettingDialog.PubSearchPeriodInt);
                if (!this.ResetTimers.PublicSearch)
                {
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                }
                this.ResetTimers.PublicSearch = false;
            }
            if (this.ResetTimers.UserTimeline || this._timerUserTimelineCounter <= 0 && this.SettingDialog.UserTimelinePeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerUserTimelineCounter, this.SettingDialog.UserTimelinePeriodInt);
                if (!this.ResetTimers.UserTimeline)
                {
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                }
                this.ResetTimers.UserTimeline = false;
            }
            if (this.ResetTimers.Lists || this._timerListsCounter <= 0 && this.SettingDialog.ListsPeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerListsCounter, this.SettingDialog.ListsPeriodInt);
                if (!this.ResetTimers.Lists)
                {
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                }
                this.ResetTimers.Lists = false;
            }
            if (this.ResetTimers.UserStream || this._timerUsCounter <= 0 && this.SettingDialog.UserstreamPeriodInt > 0)
            {
                Interlocked.Exchange(ref this._timerUsCounter, this.SettingDialog.UserstreamPeriodInt);
                if (this._isActiveUserstream)
                {
                    this.RefreshTimeline(true);
                }
                this.ResetTimers.UserStream = false;
            }
            if (this._timerRefreshFollowers > 6 * 3600)
            {
                Interlocked.Exchange(ref this._timerRefreshFollowers, 0);
                this.doGetFollowersMenu();
                this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }
            if (this.isOsResumed)
            {
                Interlocked.Increment(ref this._timerResumeWait);
                if (this._timerResumeWait > 30)
                {
                    this.isOsResumed = false;
                    Interlocked.Exchange(ref this._timerResumeWait, 0);
                    this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.Reply, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                    this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                    this.doGetFollowersMenu();
                    this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                    if (InvokeRequired && !IsDisposed)
                    {
                        this.Invoke(new MethodInvoker(this.TrimPostChain));
                    }
                }
            }
        }

        private void RefreshTimeline(bool isUserStream)
        {
            if (isUserStream)
            {
                this.RefreshTasktrayIcon(true);
            }
            // スクロール制御準備
            int smode = -1;
            // -1:制御しない,-2:最新へ,その他:topitem使用
            long topId = this.GetScrollPos(ref smode);
            int befCnt = this._curList.VirtualListSize;

            // 現在の選択状態を退避
            Dictionary<string, long[]> selId = new Dictionary<string, long[]>();
            Dictionary<string, long> focusedId = new Dictionary<string, long>();
            this.SaveSelectedStatus(selId, focusedId);

            // mentionsの更新前件数を保持
            int dmessageCount = this._statuses.GetTabByType(TabUsageType.DirectMessage).AllCount;

            // 更新確定
            PostClass[] notifyPosts = null;
            string soundFile = string.Empty;
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = this._statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon.IsEnding)
            {
                return;
            }

            // リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = this._statuses.Tabs[tab.Text];
                    lst.BeginUpdate();
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        if (lst.Equals(this._curList))
                        {
                            this._itemCache = null;
                            this._postCache = null;
                        }
                        try
                        {
                            lst.VirtualListSize = tabInfo.AllCount;
                            // リスト件数更新
                        }
                        catch (Exception)
                        {
                            // アイコン描画不具合あり？
                        }
                        this.SelectListItem(lst, this._statuses.IndexOf(tab.Text, selId[tab.Text]), this._statuses.IndexOf(tab.Text, focusedId[tab.Text]));
                    }
                    lst.EndUpdate();
                    if (tabInfo.UnreadCount > 0)
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            if (tab.ImageIndex == -1)
                            {
                                // タブアイコン
                                tab.ImageIndex = 0;
                            }
                        }
                    }
                }
                if (!this.SettingDialog.TabIconDisp)
                {
                    ListTab.Refresh();
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
                    if (befCnt != this._curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                // 最上行
                                if (this._curList.VirtualListSize > 0)
                                {
                                    this._curList.EnsureVisible(0);
                                }
                                break;
                            case -2:
                                // 最下行へ
                                if (this._curList.VirtualListSize > 0)
                                {
                                    this._curList.EnsureVisible(this._curList.VirtualListSize - 1);
                                }
                                break;
                            case -1:
                                break;
                            // 制御しない
                            default:
                                // 表示位置キープ
                                if (this._curList.VirtualListSize > 0 && this._statuses.IndexOf(this._curTab.Text, topId) > -1)
                                {
                                    this._curList.EnsureVisible(this._curList.VirtualListSize - 1);
                                    this._curList.EnsureVisible(this._statuses.IndexOf(this._curTab.Text, topId));
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
            this.NotifyNewPosts(notifyPosts, soundFile, addCount, isMention || dmessageCount != this._statuses.GetTabByType(TabUsageType.DirectMessage).AllCount);

            this.SetMainWindowTitle();
            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }

            this.HashSupl.AddRangeItem(this.tw.GetHashList());
        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (this._curList != null && this._curTab != null && this._curList.VirtualListSize > 0)
            {
                if (this._statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if (this._statuses.SortOrder == SortOrder.Ascending)
                    {
                        // Id昇順
                        if (ListLockMenuItem.Checked)
                        {
                            // 制御しない
                            smode = -1;
                            // '現在表示位置へ強制スクロール
                        }
                        else
                        {
                            // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                            ListViewItem item = this._curList.GetItemAt(0, this._curList.ClientSize.Height - 1);
                            // 一番下
                            if (item == null)
                            {
                                item = this._curList.Items[this._curList.Items.Count - 1];
                            }
                            if (item.Index == this._curList.Items.Count - 1)
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
                        if (ListLockMenuItem.Checked)
                        {
                            // 現在表示位置へ強制スクロール
                            if (this._curList.TopItem != null)
                            {
                                topId = this._statuses.GetId(this._curTab.Text, this._curList.TopItem.Index);
                            }
                            smode = 0;
                        }
                        else
                        {
                            // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                            ListViewItem item = this._curList.GetItemAt(0, 10);
                            // 一番上
                            if (item == null)
                            {
                                item = this._curList.Items[0];
                            }
                            if (item.Index == 0)
                            {
                                smode = -3;
                                // 最上行
                            }
                            else
                            {
                                if (this._curList.TopItem != null)
                                {
                                    topId = this._statuses.GetId(this._curTab.Text, this._curList.TopItem.Index);
                                }
                                smode = 0;
                            }
                        }
                    }
                }
                else
                {
                    // 現在表示位置へ強制スクロール
                    if (this._curList.TopItem != null)
                    {
                        topId = this._statuses.GetId(this._curTab.Text, this._curList.TopItem.Index);
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
            foreach (TabPage tab in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tab.Tag;
                if (lst.SelectedIndices.Count > 0 && lst.SelectedIndices.Count < 61)
                {
                    selId.Add(tab.Text, this._statuses.GetId(tab.Text, lst.SelectedIndices));
                }
                else
                {
                    selId.Add(tab.Text, new long[1] { -2 });
                }
                if (lst.FocusedItem != null)
                {
                    focusedId.Add(tab.Text, this._statuses.GetId(tab.Text, lst.FocusedItem.Index));
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
            return this.SettingDialog.EventNotifyEnabled && Convert.ToBoolean(type & this.SettingDialog.EventNotifyFlag) || type == EventType.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return Convert.ToBoolean(ev.Eventtype & this.SettingDialog.IsMyEventNotifyFlag) ? true : !ev.IsMe;
        }

        private bool BalloonRequired(Twitter.FormattedEvent ev)
        {
            return this.IsEventNotifyAsEventType(ev.Eventtype) && this.IsMyEventNotityAsEventType(ev) && (NewPostPopMenuItem.Checked || (this.SettingDialog.ForceEventNotify && ev.Eventtype != EventType.None)) && !this._initial && ((this.SettingDialog.LimitBalloon && (this.WindowState == FormWindowState.Minimized || !this.Visible || Form.ActiveForm == null)) || !this.SettingDialog.LimitBalloon) && !Win32Api.IsScreenSaverRunning();
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null && notifyPosts.Count() > 0 && this.SettingDialog.ReadOwnPost && notifyPosts.All(post => post.UserId == this.tw.UserId || post.ScreenName == this.tw.Username))
            {
                return;
            }

            // 新着通知
            if (this.BalloonRequired())
            {
                if (notifyPosts != null && notifyPosts.Length > 0)
                {
                    // Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                    if (this.SettingDialog.IsNotifyUseGrowl)
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
                            switch (this.SettingDialog.NameBalloon)
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
                            if (this.SettingDialog.DispUsername)
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
                            GrowlHelper.Notify(nt, post.StatusId.ToString(), title.ToString(), notifyText, this._TIconDic[post.ImageUrl], post.ImageUrl);
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
                            switch (this.SettingDialog.NameBalloon)
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
                        GrowlHelper.NotifyType nt = default(GrowlHelper.NotifyType);
                        if (this.SettingDialog.DispUsername)
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
                            nt = GrowlHelper.NotifyType.DirectMessage;
                        }
                        else if (reply)
                        {
                            notifyIcon = ToolTipIcon.Warning;
                            title.Append("Hoehoe [Reply!] ");
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                            nt = GrowlHelper.NotifyType.Reply;
                        }
                        else
                        {
                            notifyIcon = ToolTipIcon.Info;
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
                        NotifyIcon1.BalloonTipTitle = title.ToString();
                        NotifyIcon1.BalloonTipText = notifyText;
                        NotifyIcon1.BalloonTipIcon = notifyIcon;
                        NotifyIcon1.ShowBalloonTip(500);
                    }
                }
            }

            // サウンド再生
            if (!this._initial && this.SettingDialog.PlaySound && !string.IsNullOrEmpty(soundFile))
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
            if (!this._initial && this.SettingDialog.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                Win32Api.FlashMyWindow(this.Handle, Hoehoe.Win32Api.FlashSpecification.FlashTray, 3);
            }
        }

        private void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._curList == null || this._curList.SelectedIndices.Count != 1)
            {
                return;
            }

            this._curItemIndex = this._curList.SelectedIndices[0];
            if (this._curItemIndex > this._curList.VirtualListSize - 1)
            {
                return;
            }

            try
            {
                this._curPost = this.GetCurTabPost(this._curItemIndex);
            }
            catch (ArgumentException)
            {
                return;
            }

            this.PushSelectPostChain();

            if (this.SettingDialog.UnreadManage)
            {
                this._statuses.SetReadAllTab(true, this._curTab.Text, this._curItemIndex);
            }
            // キャッシュの書き換え
            this.ChangeCacheStyleRead(true, this._curItemIndex, this._curTab);
            // 既読へ（フォント、文字色）

            this.ColorizeList();
            this._colorize = true;
        }

        private void ChangeCacheStyleRead(bool read, int index, TabPage tab)
        {
            // Read:True=既読 False=未読
            // 未読管理していなかったら既読として扱う
            if (!this._statuses.Tabs[this._curTab.Text].UnreadManage || !this.SettingDialog.UnreadManage)
            {
                read = true;
            }

            // 対象の特定
            ListViewItem itm = null;
            PostClass post = null;
            if (tab.Equals(this._curTab) && this._itemCache != null && index >= this._itemCacheIndex && index < this._itemCacheIndex + this._itemCache.Length)
            {
                itm = this._itemCache[index - this._itemCacheIndex];
                post = this._postCache[index - this._itemCacheIndex];
            }
            else
            {
                itm = ((DetailsListView)tab.Tag).Items[index];
                post = this._statuses.Item(tab.Text, index);
            }

            this.ChangeItemStyleRead(read, itm, post, (DetailsListView)tab.Tag);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem item, PostClass post, DetailsListView listView)
        {
            // フォント
            Font fnt = read ? this._fntReaded : this._fntUnread;
            item.SubItems[5].Text = read ? string.Empty : "★";
            // 文字色
            Color cl = default(Color);
            if (post.IsFav)
            {
                cl  = this.clrFav;
            }
            else if (post.RetweetedId > 0)
            {
                cl  = this.clrRetweet;
            }
            else if (post.IsOwl && (post.IsDm || this.SettingDialog.OneWayLove))
            {
                cl  = this.clrOWL;
            }
            else if (read || !this.SettingDialog.UseUnreadStyle)
            {
                cl  = this.clrRead;
            }
            else
            {
                cl  = this.clrUnread;
            }
            if (listView == null || item.Index == -1)
            {
                item.ForeColor = cl;
                if (this.SettingDialog.UseUnreadStyle)
                {
                    item.Font = fnt;
                }
            }
            else
            {
                listView.Update();
                if (this.SettingDialog.UseUnreadStyle)
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
            PostClass post = this.anchorFlag ? this.anchorPost : this._curPost;

            if (this._itemCache == null)
            {
                return;
            }

            if (post == null)
            {
                return;
            }

            try
            {
                for (int cnt = 0; cnt < this._itemCache.Length; cnt++)
                {
                    this._curList.ChangeItemBackColor(this._itemCacheIndex + cnt, this.JudgeColor(post, this._postCache[cnt]));
                }
            }
            catch (Exception)
            {
            }
        }

        private void ColorizeList(ListViewItem item, int index)
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass post = this.anchorFlag ? this.anchorPost : this._curPost;
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
                this._curList.ChangeItemBackColor(item.Index, this.JudgeColor(post, target));
            }
        }

        private Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl = default(Color);
            if (targetPost.StatusId == basePost.InReplyToStatusId)
            {
                // @先
                cl  = this.clrAtTo;
            }
            else if (targetPost.IsMe)
            {
                // 自分=発言者
                cl  = this.clrSelf;
            }
            else if (targetPost.IsReply)
            {
                // 自分宛返信
                cl  = this.clrAtSelf;
            }
            else if (basePost.ReplyToList.Contains(targetPost.ScreenName.ToLower()))
            {
                // 返信先
                cl  = this.clrAtFromTarget;
            }
            else if (targetPost.ReplyToList.Contains(basePost.ScreenName.ToLower()))
            {
                // その人への返信
                cl  = this.clrAtTarget;
            }
            else if (targetPost.ScreenName.Equals(basePost.ScreenName, StringComparison.OrdinalIgnoreCase))
            {
                // 発言者
                cl  = this.clrTarget;
            }
            else
            {
                // その他
                cl  = this.clrListBackcolor;
            }
            return cl;
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            if (StatusText.Text.Trim().Length == 0)
            {
                if (!ImageSelectionPanel.Enabled)
                {
                    this.DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && StatusText.Text.Trim() == string.Format("RT @{0}: {1}", this._curPost.ScreenName, this._curPost.TextFromApi))
            {
                DialogResult res = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.PostButton_Click1, Environment.NewLine), "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        this.doReTweetOfficial(false);
                        StatusText.Text = string.Empty;
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            this._history[this._history.Count - 1] = new PostingStatus(StatusText.Text.Trim(), this._replyToId, this._replyToName);

            if (this.SettingDialog.Nicoms)
            {
                StatusText.SelectionStart = StatusText.Text.Length;
                this.UrlConvert(UrlConverter.Nicoms);
            }

            StatusText.SelectionStart = StatusText.Text.Length;
            GetWorkerArg args = new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = WorkerType.PostMessage };
            this.CheckReplyTo(StatusText.Text);

            // 整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = StatusText.Text.Trim();
            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !tmpStatus.EndsWith(" ."))
                {
                    adjustCount += 2;
                }
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                adjustCount += Regex.Matches(tmpStatus, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+").Count;
            }

            bool isCutOff = false;
            bool isRemoveFooter = this.isKeyDown(Keys.Shift);
            if (StatusText.Multiline && !this.SettingDialog.PostCtrlEnter)
            {
                // 複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = this.isKeyDown(Keys.Control);
            }
            if (this.SettingDialog.PostShiftEnter)
            {
                isRemoveFooter = this.isKeyDown(Keys.Control);
            }
            if (!isRemoveFooter && (StatusText.Text.Contains("RT @") || StatusText.Text.Contains("QT @")))
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
            if (StatusText.Text.StartsWith("D ") || StatusText.Text.StartsWith("d "))
            {
                // DM時は何もつけない
                footer = string.Empty;
            }
            else
            {
                // ハッシュタグ
                if (this.HashMgr.IsNotAddToAtReply)
                {
                    if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this._replyToId == 0 && string.IsNullOrEmpty(this._replyToName))
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
                    if (this.SettingDialog.UseRecommendStatus)
                    {
                        // 推奨ステータスを使用する
                        footer += this.SettingDialog.RecommendStatusText;
                    }
                    else
                    {
                        // テキストボックスに入力されている文字列を使用する
                        footer += " " + this.SettingDialog.Status.Trim();
                    }
                }
            }
            args.PStatus.Status = header + StatusText.Text.Trim() + footer;

            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(args.PStatus.Status, "^[+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\\*)([+\\-\\[\\]\\s\\\\.,*/(){}^~|='&%$#\"<>?]+|$)", RegexOptions.IgnoreCase) && !args.PStatus.Status.EndsWith(" ."))
                {
                    args.PStatus.Status += " .";
                }
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+");
                if (mc2.Success)
                {
                    args.PStatus.Status = Regex.Replace(args.PStatus.Status, "https?:\\/\\/[-_.!~*'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#^]+", "$& ");
                }
            }

            if (IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                args.PStatus.Status = args.PStatus.Status.Replace("　", " ");
            }

            if (isCutOff && args.PStatus.Status.Length > 140)
            {
                args.PStatus.Status = args.PStatus.Status.Substring(0, 140);
                string AtId = "(@|＠)[a-z0-9_/]+$";
                string HashTag = "(^|[^0-9A-Z&\\/\\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$";
                string Url = "https?:\\/\\/[a-z0-9!\\*'\\(\\);:&=\\+\\$\\/%#\\[\\]\\-_\\.,~?]+$";
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

            args.PStatus.InReplyToId = this._replyToId;
            args.PStatus.InReplyToName = this._replyToName;
            if (ImageSelectionPanel.Visible)
            {
                // 画像投稿
                if (!object.ReferenceEquals(ImageSelectedPicture.Image, ImageSelectedPicture.InitialImage) && ImageServiceCombo.SelectedIndex > -1 && !string.IsNullOrEmpty(ImagefilePathText.Text))
                {
                    if (MessageBox.Show(Hoehoe.Properties.Resources.PostPictureConfirm1, Hoehoe.Properties.Resources.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        TimelinePanel.Visible = true;
                        TimelinePanel.Enabled = true;
                        ImageSelectionPanel.Visible = false;
                        ImageSelectionPanel.Enabled = false;
                        if (this._curList != null)
                        {
                            this._curList.Focus();
                        }
                        return;
                    }
                    args.PStatus.ImageService = ImageServiceCombo.Text;
                    args.PStatus.ImagePath = ImagefilePathText.Text;
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImagefilePathText.Text = string.Empty;
                    TimelinePanel.Visible = true;
                    TimelinePanel.Enabled = true;
                    ImageSelectionPanel.Visible = false;
                    ImageSelectionPanel.Enabled = false;
                    if (this._curList != null)
                    {
                        this._curList.Focus();
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
            if (StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && StatusText.Text.Trim().Length > 7)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.SearchItem2Url, HttpUtility.UrlEncode(StatusText.Text.Substring(7)));
                this.OpenUriAsync(tmp);
            }

            this._replyToId = 0;
            this._replyToName = string.Empty;
            StatusText.Text = string.Empty;
            this._history.Add(new PostingStatus());
            this._hisIdx = this._history.Count - 1;
            if (!ToolStripFocusLockMenuItem.Checked)
            {
                ((Control)ListTab.SelectedTab.Tag).Focus();
            }
            this.urlUndoBuffer = null;
            UrlUndoToolStripMenuItem.Enabled = false;
            // Undoをできないように設定
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.IsEnding = true;
            this.Close();
        }

        private void Tween_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.SettingDialog.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon.IsEnding == false)
            {
                // _endingFlag=False:フォームの×ボタン
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                // Google.GASender.GetInstance().TrackEventWithCategory("post", "end", this.tw.UserId)
                this._hookGlobalHotkey.UnregisterAllOriginalHotkey();
                this._ignoreConfigSave = true;
                MyCommon.IsEnding = true;
                this.TimerTimeline.Enabled = false;
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

        static int _accountCheckErrorCount = 0;

        private static bool CheckAccountValid()
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                _accountCheckErrorCount += 1;
                if (_accountCheckErrorCount > 5)
                {
                    _accountCheckErrorCount = 0;
                    Twitter.AccountState = AccountState.Valid;
                    return true;
                }
                return false;
            }
            _accountCheckErrorCount = 0;
            return true;
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

            // Tween.My.MyProject.Application.InitCulture(); // TODO: Need this here?

            string ret = string.Empty;
            GetWorkerResult rslt = new GetWorkerResult();

            bool read = !this.SettingDialog.UnreadManage;
            if (this._initial && this.SettingDialog.UnreadManage)
            {
                read = this.SettingDialog.Readed;
            }

            GetWorkerArg args = (GetWorkerArg)e.Argument;

            if (!CheckAccountValid())
            {
                rslt.RetMsg = "Auth error. Check your account";
                rslt.WorkerType = WorkerType.ErrorState;
                // エラー表示のみ行ない、後処理キャンセル
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
                    ret = this.tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, this._initial);
                    // 新着時未読クリア
                    if (string.IsNullOrEmpty(ret) && args.WorkerType == WorkerType.Timeline && this.SettingDialog.ReadOldPosts)
                    {
                        this._statuses.SetRead();
                    }
                    // 振り分け
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.DirectMessegeRcv:
                    // 送信分もまとめて取得
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this.tw.GetDirectMessageApi(read, WorkerType.DirectMessegeSnt, args.Page == -1);
                    }
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.FavAdd:
                    // スレッド処理はしない
                    if (this._statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this._statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = null;
                            if (tbc.IsInnerStorageTabType)
                            {
                                post = tbc.Posts[args.Ids[i]];
                            }
                            else
                            {
                                post = this._statuses.Item(args.Ids[i]);
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
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
                                    // リスト再描画必要
                                    this._favTimestamps.Add(DateTime.Now);
                                    if (string.IsNullOrEmpty(post.RelTabName))
                                    {
                                        // 検索,リストUserTimeline.Relatedタブからのfavは、favタブへ追加せず。それ以外は追加
                                        this._statuses.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                    }
                                    else
                                    {
                                        // 検索,リスト,UserTimeline.Relatedタブからのfavで、TLでも取得済みならfav反映
                                        if (this._statuses.ContainsKey(post.StatusId))
                                        {
                                            PostClass postTl = this._statuses.Item(post.StatusId);
                                            postTl.IsFav = true;
                                            this._statuses.GetTabByType(TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, false);
                                        }
                                    }
                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
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
                    if (this._statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = this._statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = tbc.IsInnerStorageTabType ? tbc.Posts[args.Ids[i]] : this._statuses.Item(args.Ids[i]);
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
                                    if (this._statuses.ContainsKey(post.StatusId))
                                    {
                                        this._statuses.Item(post.StatusId).IsFav = false;
                                    }
                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
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
                        ret = this._pictureServices[args.PStatus.ImageService].Upload(ref args.PStatus.ImagePath, ref args.PStatus.Status, args.PStatus.InReplyToId);
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
                        if (!string.IsNullOrEmpty(this.SettingDialog.BrowserPath))
                        {
                            if (this.SettingDialog.BrowserPath.StartsWith("\"") && this.SettingDialog.BrowserPath.Length > 2 && this.SettingDialog.BrowserPath.IndexOf("\"", 2) > -1)
                            {
                                int sep = this.SettingDialog.BrowserPath.IndexOf("\"", 2);
                                string browserPath = this.SettingDialog.BrowserPath.Substring(1, sep - 1);
                                string arg = string.Empty;
                                if (sep < this.SettingDialog.BrowserPath.Length - 1)
                                {
                                    arg = this.SettingDialog.BrowserPath.Substring(sep + 1);
                                }
                                myPath = arg + " " + myPath;
                                Process.Start(browserPath, myPath);
                            }
                            else
                            {
                                Process.Start(this.SettingDialog.BrowserPath, myPath);
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
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.PublicSearch:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.PublicSearch))
                        {
                            if (!string.IsNullOrEmpty(tb.SearchWords))
                            {
                                ret = this.tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetSearch(read, tb, false);
                            if (string.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = this.tw.GetSearch(read, tb, true);
                            }
                        }
                    }
                    // 振り分け
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.UserTimeline:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    int count = 20;
                    if (this.SettingDialog.UseAdditionalCount)
                    {
                        count = this.SettingDialog.UserTimelineCountApi;
                    }
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.UserTimeline))
                        {
                            if (!string.IsNullOrEmpty(tb.User))
                            {
                                ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }
                    // 振り分け
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.List:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        // 定期更新
                        foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.Lists))
                        {
                            if (tb.ListInfo != null && tb.ListInfo.Id != 0)
                            {
                                ret = this.tw.GetListStatus(read, tb, false, this._initial);
                            }
                        }
                    }
                    else
                    {
                        // 手動更新（特定タブのみ更新）
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this.tw.GetListStatus(read, tb, args.Page == -1, this._initial);
                        }
                    }
                    // 振り分け
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.Related:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this.tw.GetRelatedResult(read, this._statuses.GetTabByName(args.TabName));
                    rslt.AddCount = this._statuses.DistributePosts();
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
                for (int i = this._favTimestamps.Count - 1; i >= 0; i += -1)
                {
                    if (this._favTimestamps[i].CompareTo(oneHour) < 0)
                    {
                        this._favTimestamps.RemoveAt(i);
                    }
                }
            }
            if (args.WorkerType == WorkerType.Timeline && !this._initial)
            {
                lock (this._syncObject)
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
                rslt.Page = args.Page - 1;
                // 値が正しいか後でチェック。10ページ毎の継続確認
            }

            e.Result = rslt;
        }

        private string MakeStatusMessage(GetWorkerArg AsyncArg, bool Finish)
        {
            string smsg = string.Empty;
            if (!Finish)
            {
                // 継続中メッセージ
                switch (AsyncArg.WorkerType)
                {
                    case WorkerType.Timeline:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.Reply:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText4 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.DirectMessegeRcv:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText8 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case WorkerType.FavAdd:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15 + AsyncArg.Page.ToString() + "/" + AsyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText16 + (AsyncArg.Page - AsyncArg.SIds.Count - 1).ToString();
                        break;
                    case WorkerType.FavRemove:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17 + AsyncArg.Page.ToString() + "/" + AsyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText18 + (AsyncArg.Page - AsyncArg.SIds.Count - 1).ToString();
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
                switch (AsyncArg.WorkerType)
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
                        break;
                    // 進捗メッセージ残す
                    case WorkerType.FavRemove:
                        break;
                    // 進捗メッセージ残す
                    case WorkerType.Favorites:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText20;
                        break;
                    case WorkerType.Follower:
                        smsg = Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText3;
                        break;
                    case WorkerType.Configuration:
                        break;
                    // 進捗メッセージ残す
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

        private void GetTimelineWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (MyCommon.IsEnding)
            {
                return;
            }
            if (e.ProgressPercentage > 100)
            {
                // 発言投稿
                // 開始
                if (e.ProgressPercentage == 200)
                {
                    StatusLabel.Text = "Posting...";
                }
                // 終了
                if (e.ProgressPercentage == 300)
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.PostWorker_RunWorkerCompletedText4;
                }
            }
            else
            {
                string smsg = (string)e.UserState;
                if (smsg.Length > 0)
                {
                    StatusLabel.Text = smsg;
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
                this._myStatusError = true;
                this._waitTimeline = false;
                this._waitReply = false;
                this._waitDm = false;
                this._waitFav = false;
                this._waitPubSearch = false;
                this._waitUserTimeline = false;
                this._waitLists = false;
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
                this._myStatusError = true;
                StatusLabel.Text = rslt.RetMsg;
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
                this.RefreshTimeline(false);
                // リスト反映
            }

            switch (rslt.WorkerType)
            {
                case WorkerType.Timeline:
                    this._waitTimeline = false;
                    if (!this._initial)
                    {
                        // 'API使用時の取得調整は別途考える（カウント調整？）
                    }
                    break;
                case WorkerType.Reply:
                    this._waitReply = false;
                    if (rslt.NewDM && !this._initial)
                    {
                        this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 0, string.Empty);
                    }
                    break;
                case WorkerType.Favorites:
                    this._waitFav = false;
                    break;
                case WorkerType.DirectMessegeRcv:
                    this._waitDm = false;
                    break;
                case WorkerType.FavAdd:
                case WorkerType.FavRemove:
                    if (this._curList != null && this._curTab != null)
                    {
                        this._curList.BeginUpdate();
                        if (rslt.WorkerType == WorkerType.FavRemove && this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.Favorites)
                        {
                            // 色変えは不要
                        }
                        else
                        {
                            for (int i = 0; i < rslt.SIds.Count; i++)
                            {
                                if (this._curTab.Text.Equals(rslt.TabName))
                                {
                                    int idx = this._statuses.Tabs[rslt.TabName].IndexOf(rslt.SIds[i]);
                                    if (idx > -1)
                                    {
                                        TabClass tb = this._statuses.Tabs[rslt.TabName];
                                        if (tb != null)
                                        {
                                            PostClass post = null;
                                            if (tb.TabType == TabUsageType.Lists || tb.TabType == TabUsageType.PublicSearch)
                                            {
                                                post = tb.Posts[rslt.SIds[i]];
                                            }
                                            else
                                            {
                                                post = this._statuses.Item(rslt.SIds[i]);
                                            }
                                            this.ChangeCacheStyleRead(post.IsRead, idx, this._curTab);
                                        }
                                        if (idx == this._curItemIndex)
                                        {
                                            // 選択アイテム再表示
                                            this.DispSelectedPost(true);
                                        }
                                    }
                                }
                            }
                        }
                        this._curList.EndUpdate();
                    }
                    break;
                case WorkerType.PostMessage:
                    if (string.IsNullOrEmpty(rslt.RetMsg) || rslt.RetMsg.StartsWith("Outputz") || rslt.RetMsg.StartsWith("OK:") || rslt.RetMsg == "Warn:Status is a duplicate.")
                    {
                        this._postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this._postTimestamps.Count - 1; i >= 0; i += -1)
                        {
                            if (this._postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this._postTimestamps.RemoveAt(i);
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
                                StatusText_Enter(StatusText, new EventArgs());
                            }
                        }
                    }
                    if (rslt.RetMsg.Length == 0 && this.SettingDialog.PostAndGet)
                    {
                        if (this._isActiveUserstream)
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
                        this._postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this._postTimestamps.Count - 1; i >= 0; i--)
                        {
                            if (this._postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this._postTimestamps.RemoveAt(i);
                            }
                        }
                        if (!this._isActiveUserstream && this.SettingDialog.PostAndGet)
                        {
                            this.GetTimeline(WorkerType.Timeline, 1, 0, string.Empty);
                        }
                    }
                    break;
                case WorkerType.Follower:
                    this._itemCache = null;
                    this._postCache = null;
                    if (this._curList != null)
                    {
                        this._curList.Refresh();
                    }
                    break;
                case WorkerType.Configuration:
                    // this._waitFollower = False
                    if (this.SettingDialog.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        this._pictureServices["Twitter"].Configuration("MaxUploadFilesize", this.SettingDialog.TwitterConfiguration.PhotoSizeLimit);
                    }
                    this._itemCache = null;
                    this._postCache = null;
                    if (this._curList != null)
                    {
                        this._curList.Refresh();
                    }
                    break;
                case WorkerType.PublicSearch:
                    this._waitPubSearch = false;
                    break;
                case WorkerType.UserTimeline:
                    this._waitUserTimeline = false;
                    break;
                case WorkerType.List:
                    this._waitLists = false;
                    break;
                case WorkerType.Related:
                    {
                        TabClass tb = this._statuses.GetTabByType(TabUsageType.Related);
                        if (tb != null && tb.RelationTargetPost != null && tb.Contains(tb.RelationTargetPost.StatusId))
                        {
                            foreach (TabPage tp in ListTab.TabPages)
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

        private void RemovePostFromFavTab(long[] ids)
        {
            string favTabName = this._statuses.GetTabByType(TabUsageType.Favorites).TabName;
            int fidx = 0;
            if (this._curTab.Text.Equals(favTabName))
            {
                if (this._curList.FocusedItem != null)
                {
                    fidx = this._curList.FocusedItem.Index;
                }
                else if (this._curList.TopItem != null)
                {
                    fidx = this._curList.TopItem.Index;
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
                    this._statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (this._curTab != null && this._curTab.Text.Equals(favTabName))
            {
                this._itemCache = null;
                // キャッシュ破棄
                this._postCache = null;
                this._curPost = null;
                // this._curItemIndex = -1
            }
            foreach (TabPage tp in ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = this._statuses.Tabs[favTabName].AllCount;
                    break;
                }
            }
            if (this._curTab.Text.Equals(favTabName))
            {
                do
                {
                    this._curList.SelectedIndices.Clear();
                } 
                while (this._curList.SelectedIndices.Count > 0);
                
                if (this._statuses.Tabs[favTabName].AllCount > 0)
                {
                    if (this._statuses.Tabs[favTabName].AllCount - 1 > fidx && fidx > -1)
                    {
                        this._curList.SelectedIndices.Add(fidx);
                    }
                    else
                    {
                        this._curList.SelectedIndices.Add(this._statuses.Tabs[favTabName].AllCount - 1);
                    }

                    if (this._curList.SelectedIndices.Count > 0)
                    {
                        this._curList.EnsureVisible(this._curList.SelectedIndices[0]);
                        this._curList.FocusedItem = this._curList.Items[this._curList.SelectedIndices[0]];
                    }
                }
            }
        }

        Dictionary<WorkerType, DateTime> _lastTime;

        private void GetTimeline(WorkerType workerType, int fromPage, int toPage, string tabName)
        {
            if (!MyCommon.IsNetworkAvailable())
            {
                return;
            }

            if (this._lastTime == null)
            {
                this._lastTime = new Dictionary<WorkerType, DateTime>();
            }

            // 非同期実行引数設定
            if (!this._lastTime.ContainsKey(workerType))
            {
                this._lastTime.Add(workerType, new DateTime());
            }

            double period = DateTime.Now.Subtract(this._lastTime[workerType]).TotalSeconds;
            if (period > 1 || period < -1)
            {
                this._lastTime[workerType] = DateTime.Now;
                this.RunAsync(new GetWorkerArg() { Page = fromPage, EndPage = toPage, WorkerType = workerType, TabName = tabName });
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
            switch (this.SettingDialog.ListDoubleClickAction)
            {
                case 0:
                    this.MakeReplyOrDirectStatus();
                    break;
                case 1:
                    this.FavoriteChange(true);
                    break;
                case 2:
                    if (this._curPost != null)
                    {
                        this.ShowUserStatus(this._curPost.ScreenName, false);
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
                    break;
                // 動作なし
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

        private void FavoriteChange(bool isFavAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            // TrueでFavAdd,FalseでFavRemove
            if (this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage || this._curList.SelectedIndices.Count == 0 || !this.ExistCurrentPost)
            {
                return;
            }

            // 複数fav確認msg
            if (this._curList.SelectedIndices.Count > 250 && isFavAdd)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.FavoriteLimitCountText);
                this._DoFavRetweetFlags = false;
                return;
            }
            if (multiFavoriteChangeDialogEnable && this._curList.SelectedIndices.Count > 1)
            {
                if (isFavAdd)
                {
                    string QuestionText = Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText1;
                    if (this._DoFavRetweetFlags)
                    {
                        QuestionText = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText3;
                    }
                    if (MessageBox.Show(QuestionText, Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        this._DoFavRetweetFlags = false;
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
                TabName = this._curTab.Text,
                WorkerType = isFavAdd ? WorkerType.FavAdd : WorkerType.FavRemove
            };
            foreach (int idx in this._curList.SelectedIndices)
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
                    StatusLabel.Text = Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText4;
                }
                else
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;
                }
                return;
            }

            this.RunAsync(args);
        }

        private PostClass GetCurTabPost(int index)
        {
            if (this._postCache != null && index >= this._itemCacheIndex && index < this._itemCacheIndex + this._postCache.Length)
            {
                return this._postCache[index - this._itemCacheIndex];
            }
            else
            {
                return this._statuses.Item(this._curTab.Text, index);
            }
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this._curList.SelectedIndices[0]).ScreenName);
            }
            else if (this._curList.SelectedIndices.Count == 0)
            {
                this.OpenUriAsync("http://twitter.com/");
            }
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curList.SelectedIndices.Count > 0)
            {
                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this._curList.SelectedIndices[0]).ScreenName + "/favorites");
            }
        }

        private void Tween_ClientSizeChanged(object sender, EventArgs e)
        {
            if ((!this._initialLayout) && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this._mySize = this.ClientSize;
                    this._mySpDis = this.SplitContainer1.SplitterDistance;
                    this._mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (StatusText.Multiline)
                    {
                        this._mySpDis2 = this.StatusText.Height;
                    }
                    this._myAdSpDis = this.SplitContainer4.SplitterDistance;
                    this._modifySettingLocal = true;
                }
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.SettingDialog.SortOrderLock)
            {
                return;
            }
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            if (this._iconCol)
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
            this._statuses.ToggleSortOrder(mode);
            this.InitColumnText();

            if (this._iconCol)
            {
                ((DetailsListView)sender).Columns[0].Text = this._columnOrgTexts[0];
                ((DetailsListView)sender).Columns[1].Text = this._columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    ((DetailsListView)sender).Columns[i].Text = this._columnOrgTexts[i];
                }
                ((DetailsListView)sender).Columns[e.Column].Text = this._columnTexts[e.Column];
            }

            this._itemCache = null;
            this._postCache = null;

            if (this._statuses.Tabs[this._curTab.Text].AllCount > 0 && this._curPost != null)
            {
                int idx = this._statuses.Tabs[this._curTab.Text].IndexOf(this._curPost.StatusId);
                if (idx > -1)
                {
                    this.SelectListItem(this._curList, idx);
                    this._curList.EnsureVisible(idx);
                }
            }
            this._curList.Refresh();
            this._modifySettingCommon = true;
        }

        private void Tween_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._myLoc = this.DesktopLocation;
                this._modifySettingLocal = true;
            }
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            if (ListTab.SelectedTab == null)
            {
                return;
            }
            if (this._statuses == null || this._statuses.Tabs == null || !this._statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
            {
                return;
            }
            if (!this.ExistCurrentPost)
            {
                ReplyStripMenuItem.Enabled = false;
                ReplyAllStripMenuItem.Enabled = false;
                DMStripMenuItem.Enabled = false;
                ShowProfileMenuItem.Enabled = false;
                ShowUserTimelineContextMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem2.Enabled = false;
                MoveToFavToolStripMenuItem.Enabled = false;
                TabMenuItem.Enabled = false;
                IDRuleMenuItem.Enabled = false;
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }
            else
            {
                ShowProfileMenuItem.Enabled = true;
                ListManageUserContextToolStripMenuItem2.Enabled = true;
                ReplyStripMenuItem.Enabled = true;
                ReplyAllStripMenuItem.Enabled = true;
                DMStripMenuItem.Enabled = true;
                ShowUserTimelineContextMenuItem.Enabled = true;
                MoveToFavToolStripMenuItem.Enabled = true;
                TabMenuItem.Enabled = true;
                IDRuleMenuItem.Enabled = true;
                ReadedStripMenuItem.Enabled = true;
                UnreadStripMenuItem.Enabled = true;
            }
            DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
            if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage || !this.ExistCurrentPost || this._curPost.IsDm)
            {
                FavAddToolStripMenuItem.Enabled = false;
                FavRemoveToolStripMenuItem.Enabled = false;
                StatusOpenMenuItem.Enabled = false;
                FavorareMenuItem.Enabled = false;
                ShowRelatedStatusesMenuItem.Enabled = false;

                ReTweetStripMenuItem.Enabled = false;
                ReTweetOriginalStripMenuItem.Enabled = false;
                QuoteStripMenuItem.Enabled = false;
                FavoriteRetweetContextMenu.Enabled = false;
                FavoriteRetweetUnofficialContextMenu.Enabled = false;
                if (this.ExistCurrentPost && this._curPost.IsDm)
                {
                    DeleteStripMenuItem.Enabled = true;
                }
                else
                {
                    DeleteStripMenuItem.Enabled = false;
                }
            }
            else
            {
                FavAddToolStripMenuItem.Enabled = true;
                FavRemoveToolStripMenuItem.Enabled = true;
                StatusOpenMenuItem.Enabled = true;
                FavorareMenuItem.Enabled = true;
                ShowRelatedStatusesMenuItem.Enabled = true;
                // PublicSearchの時問題出るかも

                if (this._curPost.IsMe)
                {
                    ReTweetOriginalStripMenuItem.Enabled = false;
                    FavoriteRetweetContextMenu.Enabled = false;
                    if (string.IsNullOrEmpty(this._curPost.RetweetedBy))
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
                    }
                    else
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText2;
                    }
                    DeleteStripMenuItem.Enabled = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(this._curPost.RetweetedBy))
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
                    }
                    else
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText2;
                    }
                    DeleteStripMenuItem.Enabled = false;
                    if (this._curPost.IsProtect)
                    {
                        ReTweetOriginalStripMenuItem.Enabled = false;
                        ReTweetStripMenuItem.Enabled = false;
                        QuoteStripMenuItem.Enabled = false;
                        FavoriteRetweetContextMenu.Enabled = false;
                        FavoriteRetweetUnofficialContextMenu.Enabled = false;
                    }
                    else
                    {
                        ReTweetOriginalStripMenuItem.Enabled = true;
                        ReTweetStripMenuItem.Enabled = true;
                        QuoteStripMenuItem.Enabled = true;
                        FavoriteRetweetContextMenu.Enabled = true;
                        FavoriteRetweetUnofficialContextMenu.Enabled = true;
                    }
                }
            }
            if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch || !this.ExistCurrentPost || !(this._curPost.InReplyToStatusId > 0))
            {
                RepliedStatusOpenMenuItem.Enabled = false;
            }
            else
            {
                RepliedStatusOpenMenuItem.Enabled = true;
            }
            if (!this.ExistCurrentPost || string.IsNullOrEmpty(this._curPost.RetweetedBy))
            {
                MoveToRTHomeMenuItem.Enabled = false;
            }
            else
            {
                MoveToRTHomeMenuItem.Enabled = true;
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

        private void DoStatusDelete()
        {
            if (this._curTab == null || this._curList == null)
            {
                return;
            }
            if (this._statuses.Tabs[this._curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                bool myPost = false;
                foreach (int idx in this._curList.SelectedIndices)
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
                if (this._curList.SelectedIndices.Count == 0)
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
            if (this._curList.FocusedItem != null)
            {
                fidx = this._curList.FocusedItem.Index;
            }
            else if (this._curList.TopItem != null)
            {
                fidx = this._curList.TopItem.Index;
            }
            else
            {
                fidx = 0;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                bool rslt = true;
                foreach (long id in this._statuses.GetId(this._curTab.Text, this._curList.SelectedIndices))
                {
                    string rtn = string.Empty;
                    if (this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage)
                    {
                        rtn = this.tw.RemoveDirectMessage(id, this._statuses.Item(id));
                    }
                    else
                    {
                        if (this._statuses.Item(id).IsMe || this._statuses.Item(id).RetweetedBy.ToLower() == this.tw.Username.ToLower())
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
                        this._statuses.RemovePost(id);
                    }
                }

                if (rslt)
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText4;
                    // 成功
                }
                else
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText3;
                    // 失敗
                }

                this._itemCache = null;
                // キャッシュ破棄
                this._postCache = null;
                this._curPost = null;
                this._curItemIndex = -1;
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this._statuses.Tabs[tb.Text].AllCount;
                    if (this._curTab.Equals(tb))
                    {
                        do
                        {
                            this._curList.SelectedIndices.Clear();
                        } while (this._curList.SelectedIndices.Count > 0);
                        if (this._statuses.Tabs[tb.Text].AllCount > 0)
                        {
                            if (this._statuses.Tabs[tb.Text].AllCount - 1 > fidx && fidx > -1)
                            {
                                this._curList.SelectedIndices.Add(fidx);
                            }
                            else
                            {
                                this._curList.SelectedIndices.Add(this._statuses.Tabs[tb.Text].AllCount - 1);
                            }
                            if (this._curList.SelectedIndices.Count > 0)
                            {
                                this._curList.EnsureVisible(this._curList.SelectedIndices[0]);
                                this._curList.FocusedItem = this._curList.Items[this._curList.SelectedIndices[0]];
                            }
                        }
                    }
                    if (this._statuses.Tabs[tb.Text].UnreadCount == 0)
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            if (tb.ImageIndex == 0)
                            {
                                // タブアイコン
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }
                if (!this.SettingDialog.TabIconDisp)
                {
                    ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoStatusDelete();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            this._curList.BeginUpdate();
            if (this.SettingDialog.UnreadManage)
            {
                foreach (int idx in this._curList.SelectedIndices)
                {
                    this._statuses.SetReadAllTab(true, this._curTab.Text, idx);
                }
            }
            foreach (int idx in this._curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(true, idx, this._curTab);
            }
            this.ColorizeList();
            this._curList.EndUpdate();
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (this._statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this.SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                            // タブアイコン
                        }
                    }
                }
            }
            if (!this.SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            this._curList.BeginUpdate();
            if (this.SettingDialog.UnreadManage)
            {
                foreach (int idx in this._curList.SelectedIndices)
                {
                    this._statuses.SetReadAllTab(false, this._curTab.Text, idx);
                }
            }
            foreach (int idx in this._curList.SelectedIndices)
            {
                this.ChangeCacheStyleRead(false, idx, this._curTab);
            }
            this.ColorizeList();
            this._curList.EndUpdate();
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (this._statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (this.SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == -1)
                        {
                            tb.ImageIndex = 0;
                            // タブアイコン
                        }
                    }
                }
            }
            if (!this.SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRefresh();
        }

        private void DoRefresh()
        {
            if (this._curTab != null)
            {
                switch (this._statuses.Tabs[this._curTab.Text].TabType)
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
                    // Case TabUsageType.Profile
                    // ' TODO
                    case TabUsageType.PublicSearch:
                        {
                            // ' TODO
                            TabClass tb = this._statuses.Tabs[this._curTab.Text];
                            if (string.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }
                            this.GetTimeline(WorkerType.PublicSearch, 1, 0, this._curTab.Text);
                        }
                        break;
                    case TabUsageType.UserTimeline:
                        this.GetTimeline(WorkerType.UserTimeline, 1, 0, this._curTab.Text);
                        break;
                    case TabUsageType.Lists:
                        {
                            // ' TODO
                            TabClass tb = this._statuses.Tabs[this._curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }

                            this.GetTimeline(WorkerType.List, 1, 0, this._curTab.Text);
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
            if (this._curTab != null)
            {
                switch (this._statuses.Tabs[this._curTab.Text].TabType)
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
                    // ' TODO
                    case TabUsageType.PublicSearch:
                        {
                            // TODO
                            TabClass tb = this._statuses.Tabs[this._curTab.Text];
                            if (string.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }
                            this.GetTimeline(WorkerType.PublicSearch, -1, 0, this._curTab.Text);
                        }
                        break;
                    case TabUsageType.UserTimeline:
                        this.GetTimeline(WorkerType.UserTimeline, -1, 0, this._curTab.Text);
                        break;
                    case TabUsageType.Lists:
                        {
                            // ' TODO
                            TabClass tb = this._statuses.Tabs[this._curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }
                            this.GetTimeline(WorkerType.List, -1, 0, this._curTab.Text);
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

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = default(DialogResult);
            string uid = this.tw.Username.ToLower();
            foreach (UserAccount u in this.SettingDialog.UserAccounts)
            {
                if (u.UserId == this.tw.UserId)
                {
                    break;
                }
            }

            try
            {
                result = this.SettingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return;
            }

            if (result == DialogResult.OK)
            {
                lock (this._syncObject)
                {
                    this.tw.SetTinyUrlResolve(this.SettingDialog.TinyUrlResolve);
                    this.tw.SetRestrictFavCheck(this.SettingDialog.RestrictFavCheck);
                    this.tw.ReadOwnPost = this.SettingDialog.ReadOwnPost;
                    this.tw.SetUseSsl(this.SettingDialog.UseSsl);
                    ShortUrl.IsResolve = this.SettingDialog.TinyUrlResolve;
                    ShortUrl.IsForceResolve = this.SettingDialog.ShortUrlForceResolve;
                    ShortUrl.SetBitlyId(this.SettingDialog.BitlyUser);
                    ShortUrl.SetBitlyKey(this.SettingDialog.BitlyPwd);
                    HttpTwitter.SetTwitterUrl(this._cfgCommon.TwitterUrl);
                    HttpTwitter.SetTwitterSearchUrl(this._cfgCommon.TwitterSearchUrl);

                    HttpConnection.InitializeConnection(this.SettingDialog.DefaultTimeOut, this.SettingDialog.SelectedProxyType, this.SettingDialog.ProxyAddress, this.SettingDialog.ProxyPort, this.SettingDialog.ProxyUser, this.SettingDialog.ProxyPassword);
                    this.CreatePictureServices();
#if UA // = "True"
					this.SplitContainer4.Panel2.Controls.RemoveAt(0);
					this.ab = new AdsBrowser();
					this.SplitContainer4.Panel2.Controls.Add(ab);
#endif
                    try
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            ListTab.DrawItem -= this.ListTab_DrawItem;
                            ListTab.DrawMode = TabDrawMode.Normal;
                            ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            ListTab.DrawItem -= this.ListTab_DrawItem;
                            ListTab.DrawItem += this.ListTab_DrawItem;
                            ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                            ListTab.ImageList = null;
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
                        if (!this.SettingDialog.UnreadManage)
                        {
                            ReadedStripMenuItem.Enabled = false;
                            UnreadStripMenuItem.Enabled = false;
                            if (this.SettingDialog.TabIconDisp)
                            {
                                foreach (TabPage myTab in ListTab.TabPages)
                                {
                                    myTab.ImageIndex = -1;
                                }
                            }
                        }
                        else
                        {
                            ReadedStripMenuItem.Enabled = true;
                            UnreadStripMenuItem.Enabled = true;
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
                        foreach (TabPage mytab in ListTab.TabPages)
                        {
                            DetailsListView lst = (DetailsListView)mytab.Tag;
                            lst.GridLines = this.SettingDialog.ShowGrid;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(ShowGrid)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    PlaySoundMenuItem.Checked = this.SettingDialog.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = this.SettingDialog.PlaySound;
                    this._fntUnread = this.SettingDialog.FontUnread;
                    this.clrUnread = this.SettingDialog.ColorUnread;
                    this._fntReaded = this.SettingDialog.FontReaded;
                    this.clrRead = this.SettingDialog.ColorReaded;
                    this.clrFav = this.SettingDialog.ColorFav;
                    this.clrOWL = this.SettingDialog.ColorOWL;
                    this.clrRetweet = this.SettingDialog.ColorRetweet;
                    this._fntDetail = this.SettingDialog.FontDetail;
                    this.clrDetail = this.SettingDialog.ColorDetail;
                    this.clrDetailLink = this.SettingDialog.ColorDetailLink;
                    this.clrDetailBackcolor = this.SettingDialog.ColorDetailBackcolor;
                    this.clrSelf = this.SettingDialog.ColorSelf;
                    this.clrAtSelf = this.SettingDialog.ColorAtSelf;
                    this.clrTarget = this.SettingDialog.ColorTarget;
                    this.clrAtTarget = this.SettingDialog.ColorAtTarget;
                    this.clrAtFromTarget = this.SettingDialog.ColorAtFromTarget;
                    this.clrAtTo = this.SettingDialog.ColorAtTo;
                    this.clrListBackcolor = this.SettingDialog.ColorListBackcolor;
                    this.clrInputBackcolor = this.SettingDialog.ColorInputBackcolor;
                    this.clrInputForecolor = this.SettingDialog.ColorInputFont;
                    this._fntInputFont = this.SettingDialog.FontInputFont;
                    try
                    {
                        if (StatusText.Focused)
                        {
                            StatusText.BackColor  = this.clrInputBackcolor;
                        }
                        StatusText.Font = this._fntInputFont;
                        StatusText.ForeColor  = this.clrInputForecolor;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    this._brsForeColorUnread.Dispose();
                    this._brsForeColorReaded.Dispose();
                    this._brsForeColorFav.Dispose();
                    this._brsForeColorOWL.Dispose();
                    this._brsForeColorRetweet.Dispose();
                    this._brsForeColorUnread = new SolidBrush(this.clrUnread);
                    this._brsForeColorReaded = new SolidBrush(this.clrRead);
                    this._brsForeColorFav = new SolidBrush(this.clrFav);
                    this._brsForeColorOWL = new SolidBrush(this.clrOWL);
                    this._brsForeColorRetweet = new SolidBrush(this.clrRetweet);
                    this._brsBackColorMine.Dispose();
                    this._brsBackColorAt.Dispose();
                    this._brsBackColorYou.Dispose();
                    this._brsBackColorAtYou.Dispose();
                    this._brsBackColorAtFromTarget.Dispose();
                    this._brsBackColorAtTo.Dispose();
                    this._brsBackColorNone.Dispose();
                    this._brsBackColorMine = new SolidBrush(this.clrSelf);
                    this._brsBackColorAt = new SolidBrush(this.clrAtSelf);
                    this._brsBackColorYou = new SolidBrush(this.clrTarget);
                    this._brsBackColorAtYou = new SolidBrush(this.clrAtTarget);
                    this._brsBackColorAtFromTarget = new SolidBrush(this.clrAtFromTarget);
                    this._brsBackColorAtTo = new SolidBrush(this.clrAtTo);
                    this._brsBackColorNone = new SolidBrush(this.clrListBackcolor);
                    try
                    {
                        if (this.SettingDialog.IsMonospace)
                        {
                            this._detailHtmlFormatHeader = DetailHtmlFormatMono1;
                            this._detailHtmlFormatFooter = DetailHtmlFormatMono7;
                        }
                        else
                        {
                            this._detailHtmlFormatHeader = DetailHtmlFormat1;
                            this._detailHtmlFormatFooter = DetailHtmlFormat7;
                        }
                        this._detailHtmlFormatHeader += this._fntDetail.Name + DetailHtmlFormat2 + this._fntDetail.Size.ToString() + DetailHtmlFormat3 + this.clrDetail.R.ToString() + "," + this.clrDetail.G.ToString() + "," + this.clrDetail.B.ToString() + DetailHtmlFormat4 + this.clrDetailLink.R.ToString() + "," + this.clrDetailLink.G.ToString() + "," + this.clrDetailLink.B.ToString() + DetailHtmlFormat5 + this.clrDetailBackcolor.R.ToString() + "," + this.clrDetailBackcolor.G.ToString() + "," + this.clrDetailBackcolor.B.ToString();
                        if (this.SettingDialog.IsMonospace)
                        {
                            this._detailHtmlFormatHeader += DetailHtmlFormatMono6;
                        }
                        else
                        {
                            this._detailHtmlFormatHeader += DetailHtmlFormat6;
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
                        this._statuses.SetUnreadManage(this.SettingDialog.UnreadManage);
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "_statuses";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage tb in ListTab.TabPages)
                        {
                            if (this.SettingDialog.TabIconDisp)
                            {
                                if (this._statuses.Tabs[tb.Text].UnreadCount == 0)
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
                                ((DetailsListView)tb.Tag).Font = this._fntReaded;
                                ((DetailsListView)tb.Tag).BackColor  = this.clrListBackcolor;
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

                    this._itemCache = null;
                    this._postCache = null;
                    if (this._curList != null)
                    {
                        this._curList.Refresh();
                    }
                    ListTab.Refresh();

                    Outputz.Key = this.SettingDialog.OutputzKey;
                    Outputz.Enabled = this.SettingDialog.OutputzEnabled;
                    switch (this.SettingDialog.OutputzUrlmode)
                    {
                        case OutputzUrlmode.twittercom:
                            Outputz.OutUrl = "http:// twitter.com/";
                            break;
                        case OutputzUrlmode.twittercomWithUsername:
                            Outputz.OutUrl = "http:// twitter.com/" + this.tw.Username;
                            break;
                    }

                    this._hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (this.SettingDialog.HotkeyEnabled)
                    {
                        ///グローバルホットキーの登録。設定で変更可能にするかも
                        HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                        if ((this.SettingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                        }
                        if ((this.SettingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                        }
                        if ((this.SettingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                        }
                        if ((this.SettingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                        }

                        this._hookGlobalHotkey.RegisterOriginalHotkey(this.SettingDialog.HotkeyKey, this.SettingDialog.HotkeyValue, modKey);
                    }

                    if (uid != this.tw.Username)
                    {
                        this.doGetFollowersMenu();
                    }

                    this.SetImageServiceCombo();
                    if (this.SettingDialog.IsNotifyUseGrowl)
                    {
                        GrowlHelper.RegisterGrowl();
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

            this.TopMost = this.SettingDialog.AlwaysTop;
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
                StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
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
                        if (this.SettingDialog.OpenUserTimeline)
                        {
                            if (this.isKeyDown(Keys.Control))
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
                            if (this.isKeyDown(Keys.Control))
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

        public void AddNewTabForSearch(string searchWord)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.PublicSearch))
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
                if (this._statuses.ContainsTab(tabName))
                {
                    tabName += "_";
                }
                else
                {
                    break;
                }
            }
            // タブ追加
            this._statuses.AddTab(tabName, TabUsageType.PublicSearch, null);
            this.AddNewTab(tabName, false, TabUsageType.PublicSearch);
            // 追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            // 検索条件の設定
            ComboBox cmb = (ComboBox)ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            this.SaveConfigsTabs();
            // 検索実行
            this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
        }

        private void ShowUserTimeline()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }
            this.AddNewTabForUserTimeline(this._curPost.ScreenName);
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                TabPage relTp = ListTab.SelectedTab;
                this.RemoveSpecifiedTab(relTp.Text, false);
                this.SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        public void AddNewTabForUserTimeline(string user)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.UserTimeline))
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
            while (this._statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }
            // タブ追加
            this._statuses.AddTab(tabName, TabUsageType.UserTimeline, null);
            this._statuses.Tabs[tabName].User = user;
            this.AddNewTab(tabName, false, TabUsageType.UserTimeline);
            // 追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            this.SaveConfigsTabs();
            // 検索実行
            this.GetTimeline(WorkerType.UserTimeline, 1, 0, tabName);
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
            if (tabName == Hoehoe.Properties.Resources.AddNewTabText1)
            {
                return false;
            }

            // タブタイプ重複チェック
            if (!startup)
            {
                if (tabType == TabUsageType.DirectMessage || tabType == TabUsageType.Favorites || tabType == TabUsageType.Home || tabType == TabUsageType.Mentions || tabType == TabUsageType.Related)
                {
                    if (this._statuses.GetTabByType(tabType) != null)
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

            int cnt = ListTab.TabPages.Count;

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
                label.Text = tabType == TabUsageType.Lists ? listInfo.ToString() : this._statuses.Tabs[tabName].User + "'s Timeline";
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

                if (this._statuses.ContainsTab(tabName))
                {
                    cmb.Items.Add(this._statuses.Tabs[tabName].SearchWords);
                    cmb.Text = this._statuses.Tabs[tabName].SearchWords;
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
                if (this._statuses.ContainsTab(tabName))
                {
                    cmbLang.Text = this._statuses.Tabs[tabName].SearchLang;
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
            if (!this._iconCol)
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
            listCustom.Font = this._fntReaded;
            listCustom.BackColor  = this.clrListBackcolor;
            listCustom.GridLines = this.SettingDialog.ShowGrid;
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
            colHd1.Text = this._columnTexts[0];
            colHd1.Width = 48;
            colHd2.Text = this._columnTexts[1];
            colHd2.Width = 80;
            colHd3.Text = this._columnTexts[2];
            colHd3.Width = 300;
            colHd4.Text = this._columnTexts[3];
            colHd4.Width = 50;
            colHd5.Text = this._columnTexts[4];
            colHd5.Width = 50;
            colHd6.Text = this._columnTexts[5];
            colHd6.Width = 16;
            colHd7.Text = this._columnTexts[6];
            colHd7.Width = 16;
            colHd8.Text = this._columnTexts[7];
            colHd8.Width = 50;

            if (this._statuses.IsDistributableTab(tabName))
            {
                this.TabDialog.AddTab(tabName);
            }

            listCustom.SmallImageList = new ImageList();
            if (this._iconSz > 0)
            {
                listCustom.SmallImageList.ImageSize = new Size(this._iconSz, this._iconSz);
            }
            else
            {
                listCustom.SmallImageList.ImageSize = new Size(1, 1);
            }

            int[] dispOrder = new int[8];
            if (!startup)
            {
                for (int i = 0; i < this._curList.Columns.Count; i++)
                {
                    for (int j = 0; j < this._curList.Columns.Count; j++)
                    {
                        if (this._curList.Columns[j].DisplayIndex == i)
                        {
                            dispOrder[i] = j;
                            break;
                        }
                    }
                }
                for (int i = 0; i <= this._curList.Columns.Count - 1; i++)
                {
                    listCustom.Columns[i].Width = this._curList.Columns[i].Width;
                    listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                }
            }
            else
            {
                if (this._iconCol)
                {
                    listCustom.Columns[0].Width = this._cfgLocal.Width1;
                    listCustom.Columns[1].Width = this._cfgLocal.Width3;
                    listCustom.Columns[0].DisplayIndex = 0;
                    listCustom.Columns[1].DisplayIndex = 1;
                }
                else
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (this._cfgLocal.DisplayIndex1 == i)
                        {
                            dispOrder[i] = 0;
                        }
                        else if (this._cfgLocal.DisplayIndex2 == i)
                        {
                            dispOrder[i] = 1;
                        }
                        else if (this._cfgLocal.DisplayIndex3 == i)
                        {
                            dispOrder[i] = 2;
                        }
                        else if (this._cfgLocal.DisplayIndex4 == i)
                        {
                            dispOrder[i] = 3;
                        }
                        else if (this._cfgLocal.DisplayIndex5 == i)
                        {
                            dispOrder[i] = 4;
                        }
                        else if (this._cfgLocal.DisplayIndex6 == i)
                        {
                            dispOrder[i] = 5;
                        }
                        else if (this._cfgLocal.DisplayIndex7 == i)
                        {
                            dispOrder[i] = 6;
                        }
                        else if (this._cfgLocal.DisplayIndex8 == i)
                        {
                            dispOrder[i] = 7;
                        }
                    }
                    listCustom.Columns[0].Width = this._cfgLocal.Width1;
                    listCustom.Columns[1].Width = this._cfgLocal.Width2;
                    listCustom.Columns[2].Width = this._cfgLocal.Width3;
                    listCustom.Columns[3].Width = this._cfgLocal.Width4;
                    listCustom.Columns[4].Width = this._cfgLocal.Width5;
                    listCustom.Columns[5].Width = this._cfgLocal.Width6;
                    listCustom.Columns[6].Width = this._cfgLocal.Width7;
                    listCustom.Columns[7].Width = this._cfgLocal.Width8;
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

        public bool RemoveSpecifiedTab(string TabName, bool confirm)
        {
            int idx = 0;
            for (idx = 0; idx < ListTab.TabPages.Count; idx++)
            {
                if (ListTab.TabPages[idx].Text == TabName)
                {
                    break;
                }
            }

            if (this._statuses.IsDefaultTab(TabName))
            {
                return false;
            }

            if (confirm)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.RemoveSpecifiedTabText1, Environment.NewLine);
                if (MessageBox.Show(tmp, TabName + " " + Hoehoe.Properties.Resources.RemoveSpecifiedTabText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                {
                    return false;
                }
            }

            this.SetListProperty();
            // 他のタブに列幅等を反映

            TabUsageType tabType = this._statuses.Tabs[TabName].TabType;

            // オブジェクトインスタンスの削除
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.ListTab.SuspendLayout();
            this.SuspendLayout();

            TabPage tabPage = ListTab.TabPages[idx];
            DetailsListView listCustom = (DetailsListView)tabPage.Tag;
            tabPage.Tag = null;

            tabPage.SuspendLayout();

            if (object.ReferenceEquals(this.ListTab.SelectedTab, this.ListTab.TabPages[idx]))
            {
                this.ListTab.SelectTab(this._beforeSelectedTab != null && this.ListTab.TabPages.Contains(this._beforeSelectedTab) ? this._beforeSelectedTab : this.ListTab.TabPages[0]);
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

            this.TabDialog.RemoveTab(TabName);

            listCustom.SmallImageList = null;
            listCustom.ListViewItemSorter = null;

            // キャッシュのクリア
            if (this._curTab.Equals(tabPage))
            {
                this._curTab = null;
                this._curItemIndex = -1;
                this._curList = null;
                this._curPost = null;
            }
            this._itemCache = null;
            this._itemCacheIndex = -1;
            this._postCache = null;

            tabPage.ResumeLayout(false);

            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.ResumeLayout(false);
            this.ListTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            tabPage.Dispose();
            listCustom.Dispose();
            this._statuses.RemoveTab(TabName);

            foreach (TabPage tp in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tp.Tag;
                if (lst.VirtualListSize != this._statuses.Tabs[tp.Text].AllCount)
                {
                    lst.VirtualListSize = this._statuses.Tabs[tp.Text].AllCount;
                }
            }

            return true;
        }

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this._itemCache = null;
            this._itemCacheIndex = -1;
            this._postCache = null;
            this._beforeSelectedTab = e.TabPage;
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D

            if (!this.SettingDialog.TabMouseLock && e.Button == MouseButtons.Left && this._tabDrag)
            {
                string tn = string.Empty;
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(this._tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(this._tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    // タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = ListTab.SelectedTab.Text;
                }

                if (string.IsNullOrEmpty(tn))
                {
                    return;
                }

                foreach (TabPage tb in ListTab.TabPages)
                {
                    if (tb.Text == tn)
                    {
                        ListTab.DoDragDrop(tb, DragDropEffects.All);
                        break;
                    }
                }
            }
            else
            {
                this._tabDrag = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Left <= cpos.X & cpos.X <= rect.Right & rect.Top <= cpos.Y & cpos.Y <= rect.Bottom)
                {
                    this._rclickTabName = ListTab.TabPages[i].Text;
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
            if (ListTab.Focused || ((Control)ListTab.SelectedTab.Tag).Focused)
            {
                this.Tag = ListTab.Tag;
            }
            this.TabMenuControl(ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
        }

        private void SetListProperty()
        {
            // 削除などで見つからない場合は処理せず
            if (this._curList == null)
            {
                return;
            }
            if (!this._isColumnChanged)
            {
                return;
            }

            int[] dispOrder = new int[this._curList.Columns.Count];
            for (int i = 0; i < this._curList.Columns.Count; i++)
            {
                for (int j = 0; j < this._curList.Columns.Count; j++)
                {
                    if (this._curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            // 列幅、列並びを他のタブに設定
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (!tb.Equals(this._curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i <= lst.Columns.Count - 1; i++)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = this._curList.Columns[i].Width;
                        }
                    }
                }
            }

            this._isColumnChanged = false;
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (PostBrowser.StatusText.StartsWith("http") || PostBrowser.StatusText.StartsWith("ftp") || PostBrowser.StatusText.StartsWith("data"))
                {
                    StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
                }
                if (string.IsNullOrEmpty(PostBrowser.StatusText))
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
                if (!this.SettingDialog.UseAtIdSupplement)
                {
                    return;
                }
                // @マーク
                int cnt = this.AtIdSupl.ItemCount;
                this.ShowSuplDialog(StatusText, this.AtIdSupl);
                if (cnt != this.AtIdSupl.ItemCount)
                {
                    this._modifySettingAtId = true;
                }
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!this.SettingDialog.UseHashSupplement)
                {
                    return;
                }
                this.ShowSuplDialog(StatusText, this.HashSupl);
                e.Handled = true;
            }
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
            this.TopMost = this.SettingDialog.AlwaysTop;
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

        private void StatusText_KeyUp(object sender, KeyEventArgs e)
        {
            // スペースキーで未読ジャンプ
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.ProcessKey)
                {
                    bool isSpace = false;
                    foreach (char c in StatusText.Text.ToCharArray())
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
                        StatusText.Text = string.Empty;
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
            lblLen.Text = len.ToString();
            if (len < 0)
            {
                StatusText.ForeColor = Color.Red;
            }
            else
            {
                StatusText.ForeColor  = this.clrInputForecolor;
            }

            if (string.IsNullOrEmpty(StatusText.Text))
            {
                this._replyToId = 0;
                this._replyToName = string.Empty;
            }
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            // 文字数カウント
            int len = 140 - StatusText.Text.Length;
            if (this.NotifyIcon1 == null || !this.NotifyIcon1.Visible)
            {
                return len;
            }
            if ((isAuto && !this.isKeyDown(Keys.Control) && this.SettingDialog.PostShiftEnter)
                || (isAuto && !this.isKeyDown(Keys.Shift) && !this.SettingDialog.PostShiftEnter)
                || (!isAuto && isAddFooter))
            {
                if (this.SettingDialog.UseRecommendStatus)
                {
                    len -= this.SettingDialog.RecommendStatusText.Length;
                }
                else if (this.SettingDialog.Status.Length > 0)
                {
                    len -= this.SettingDialog.Status.Length + 1;
                }
            }
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                len -= this.HashMgr.UseHash.Length + 1;
            }

            foreach (Match m in Regex.Matches(StatusText.Text, Twitter.RgUrl, RegexOptions.IgnoreCase))
            {
                len += m.Result("${url}").Length - this.SettingDialog.TwitterConfiguration.ShortUrlLength;
            }
            if (ImageSelectionPanel.Visible && ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(this.ImageService))
            {
                len -= this.SettingDialog.TwitterConfiguration.CharactersReservedPerMedia;
            }
            return len;
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (this._itemCache != null && e.StartIndex >= this._itemCacheIndex && e.EndIndex < this._itemCacheIndex + this._itemCache.Length && this._curList.Equals(sender))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            if (this._curList.Equals(sender))
            {
                this.CreateCache(e.StartIndex, e.EndIndex);
            }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this._itemCache != null && e.ItemIndex >= this._itemCacheIndex && e.ItemIndex < this._itemCacheIndex + this._itemCache.Length && this._curList.Equals(sender))
            {
                // A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = this._itemCache[e.ItemIndex - this._itemCacheIndex];
            }
            else
            {
                // A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((Hoehoe.TweenCustomControl.DetailsListView)sender).Parent;
                try
                {
                    e.Item = this.CreateItem(tb, this._statuses.Item(tb.Text, e.ItemIndex), e.ItemIndex);
                }
                catch (Exception)
                {
                    // 不正な要求に対する間に合わせの応答
                    e.Item = new ImageListViewItem(new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }, string.Empty);
                }
            }
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
                if (endIndex >= this._statuses.Tabs[this._curTab.Text].AllCount)
                {
                    endIndex = this._statuses.Tabs[this._curTab.Text].AllCount - 1;
                }
                this._postCache = this._statuses.Item(this._curTab.Text, startIndex, endIndex);
                // 配列で取得
                this._itemCacheIndex = startIndex;

                this._itemCache = new ListViewItem[this._postCache.Length];
                for (int i = 0; i <= this._postCache.Length - 1; i++)
                {
                    this._itemCache[i] = this.CreateItem(this._curTab, this._postCache[i], startIndex + i);
                }
            }
            catch (Exception)
            {
                // キャッシュ要求が実データとずれるため（イベントの遅延？）
                this._postCache = null;
                this._itemCache = null;
            }
        }

        private ListViewItem CreateItem(TabPage Tab, PostClass Post, int Index)
        {
            StringBuilder mk = new StringBuilder();
            if (Post.FavoritedCount > 0)
            {
                mk.Append("+" + Post.FavoritedCount.ToString());
            }
            ImageListViewItem itm = null;
            if (Post.RetweetedId == 0)
            {
                string[] sitem = { string.Empty, Post.Nickname, Post.IsDeleted ? "(DELETED)" : Post.TextFromApi, Post.CreatedAt.ToString(this.SettingDialog.DateTimeFormat), Post.ScreenName, string.Empty, mk.ToString(), Post.Source };
                itm = new ImageListViewItem(sitem, (ImageDictionary)this._TIconDic, Post.ImageUrl);
            }
            else
            {
                string[] sitem = { string.Empty, Post.Nickname, Post.IsDeleted ? "(DELETED)" : Post.TextFromApi, Post.CreatedAt.ToString(this.SettingDialog.DateTimeFormat), Post.ScreenName + Environment.NewLine + "(RT:" + Post.RetweetedBy + ")", string.Empty, mk.ToString(), Post.Source };
                itm = new ImageListViewItem(sitem, (ImageDictionary)this._TIconDic, Post.ImageUrl);
            }
            itm.StateImageIndex = Post.StateIndex;

            bool read = Post.IsRead;
            // 未読管理していなかったら既読として扱う
            if (!this._statuses.Tabs[Tab.Text].UnreadManage || !this.SettingDialog.UnreadManage)
            {
                read = true;
            }
            this.ChangeItemStyleRead(read, itm, Post, null);
            if (Tab.Equals(this._curTab))
            {
                this.ColorizeList(itm, Index);
            }
            return itm;
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
            // e.ItemStateでうまく判定できない？？？
            if (!e.Item.Selected)
            {
                SolidBrush brs2 = null;
                var cl = e.Item.BackColor;
                if (cl == this.clrSelf)
                {
                    brs2 = this._brsBackColorMine;
                }
                else if (cl == this.clrAtSelf)
                {
                    brs2 = this._brsBackColorAt;
                }
                else if (cl == this.clrTarget)
                {
                    brs2 = this._brsBackColorYou;
                }
                else if (cl == this.clrAtTarget)
                {
                    brs2 = this._brsBackColorAtYou;
                }
                else if (cl == this.clrAtFromTarget)
                {
                    brs2 = this._brsBackColorAtFromTarget;
                }
                else if (cl == this.clrAtTo)
                {
                    brs2 = this._brsBackColorAtTo;
                }
                else
                {
                    brs2 = this._brsBackColorNone;
                }
                e.Graphics.FillRectangle(brs2, e.Bounds);
            }
            else
            {
                // 選択中の行
                if (((Control)sender).Focused)
                {
                    e.Graphics.FillRectangle(this._brsHighLight, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(this._brsDeactiveSelection, e.Bounds);
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
                if (this._iconCol)
                {
                    rct.Y += e.Item.Font.Height;
                    rct.Height -= e.Item.Font.Height;
                    rctB.Height = e.Item.Font.Height;
                }

                int heightDiff = 0;
                int drawLineCount = Math.Max(1, Math.DivRem(Convert.ToInt32(rct.Height), e.Item.Font.Height, out heightDiff));

                // フォントの高さの半分を足してるのは保険。無くてもいいかも。
                if (!this._iconCol && drawLineCount <= 1)
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
                        brs = this._brsForeColorUnread;
                    }
                    else if (cl == this.clrRead)
                    {
                        brs = this._brsForeColorReaded;
                    }
                    else if (cl == this.clrFav)
                    {
                        brs = this._brsForeColorFav;
                    }
                    else if (cl == this.clrOWL)
                    {
                        brs = this._brsForeColorOWL;
                    }
                    else if (cl == this.clrRetweet)
                    {
                        brs = this._brsForeColorRetweet;
                    }
                    else
                    {
                        brs = new SolidBrush(e.Item.ForeColor);
                        flg = true;
                    }

                    if (rct.Width > 0)
                    {
                        if (this._iconCol)
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
                                if (this._iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this._brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this._brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this._brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this._brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                            }
                            else
                            {
                                if (this._iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), this._brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), this._brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this._brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), this._brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                            }
                        }
                    }
                }
            }
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
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(this._iconSz, this._iconSz)), itemRect);
                iconRect.Offset(0, Convert.ToInt32(Math.Max(0, (itemRect.Height - this._iconSz) / 2)));
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this._iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }
            else
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(1, 1)), itemRect);
                // iconRect.Offset(0, CType(Math.Max(0, (itemRect.Height - this._iconSz) / 2), Integer))
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + this._iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
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
            if (this._curList.VirtualListSize == 0)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            int cidx = 0;
            if (this._curList.SelectedIndices.Count > 0)
            {
                cidx = this._curList.SelectedIndices[0];
            }
            int toIdx = this._curList.VirtualListSize - 1;

            int stp = 1;
            switch (searchType)
            {
                case SEARCHTYPE.DialogSearch:
                    // ダイアログからの検索
                    if (this._curList.SelectedIndices.Count > 0)
                    {
                        cidx = this._curList.SelectedIndices[0];
                    }
                    else
                    {
                        cidx = 0;
                    }
                    break;
                case SEARCHTYPE.NextSearch:
                    // 次を検索
                    if (this._curList.SelectedIndices.Count > 0)
                    {
                        cidx = this._curList.SelectedIndices[0] + 1;
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
                    if (this._curList.SelectedIndices.Count > 0)
                    {
                        cidx = this._curList.SelectedIndices[0] - 1;
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
                                post = this._statuses.Item(this._curTab.Text, idx);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (searchRegex.IsMatch(post.Nickname) || searchRegex.IsMatch(post.TextFromApi) || searchRegex.IsMatch(post.ScreenName))
                            {
                                this.SelectListItem(this._curList, idx);
                                this._curList.EnsureVisible(idx);
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
                            post = this._statuses.Item(this._curTab.Text, idx);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        if (post.Nickname.IndexOf(word, fndOpt) > -1 || post.TextFromApi.IndexOf(word, fndOpt) > -1 || post.ScreenName.IndexOf(word, fndOpt) > -1)
                        {
                            this.SelectListItem(this._curList, idx);
                            this._curList.EnsureVisible(idx);
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
                            cidx = this._curList.Items.Count - 1;
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

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            // 検索メニュー
            this.SearchDialog.Owner = this;
            if (this.SearchDialog.ShowDialog() == DialogResult.Cancel)
            {
                this.TopMost = this.SettingDialog.AlwaysTop;
                return;
            }
            this.TopMost = this.SettingDialog.AlwaysTop;

            if (!string.IsNullOrEmpty(this.SearchDialog.SWord))
            {
                this.DoTabSearch(this.SearchDialog.SWord, this.SearchDialog.CheckCaseSensitive, this.SearchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            // 次を検索
            if (string.IsNullOrEmpty(this.SearchDialog.SWord))
            {
                if (this.SearchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.SettingDialog.AlwaysTop;
                    return;
                }
                this.TopMost = this.SettingDialog.AlwaysTop;
                if (string.IsNullOrEmpty(this.SearchDialog.SWord))
                {
                    return;
                }
                this.DoTabSearch(this.SearchDialog.SWord, this.SearchDialog.CheckCaseSensitive, this.SearchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
            else
            {
                this.DoTabSearch(this.SearchDialog.SWord, this.SearchDialog.CheckCaseSensitive, this.SearchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            // 前を検索
            if (string.IsNullOrEmpty(this.SearchDialog.SWord))
            {
                if (this.SearchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.SettingDialog.AlwaysTop;
                    return;
                }
                this.TopMost = this.SettingDialog.AlwaysTop;
                if (string.IsNullOrEmpty(this.SearchDialog.SWord))
                {
                    return;
                }
            }

            this.DoTabSearch(this.SearchDialog.SWord, this.SearchDialog.CheckCaseSensitive, this.SearchDialog.CheckRegex, SEARCHTYPE.PrevSearch);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            if (this.aboutBox == null)
            {
                this.aboutBox = new TweenAboutBox();
            }
            this.aboutBox.ShowDialog();
            this.TopMost = this.SettingDialog.AlwaysTop;
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageSelectionPanel.Enabled)
            {
                return;
            }

            // 現在タブから最終タブまで探索
            int bgnIdx = ListTab.TabPages.IndexOf(this._curTab);
            int idx = -1;
            DetailsListView lst = null;
            for (int i = bgnIdx; i < ListTab.TabPages.Count; i++)
            {
                // 未読Index取得
                idx = this._statuses.GetOldestUnreadIndex(ListTab.TabPages[i].Text);
                if (idx > -1)
                {
                    ListTab.SelectedIndex = i;
                    lst = (DetailsListView)ListTab.TabPages[i].Tag;
                    break;
                }
            }

            // 未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
            if (idx == -1 && bgnIdx > 0)
            {
                for (int i = 0; i < bgnIdx; i++)
                {
                    idx = this._statuses.GetOldestUnreadIndex(ListTab.TabPages[i].Text);
                    if (idx > -1)
                    {
                        ListTab.SelectedIndex = i;
                        lst = (DetailsListView)ListTab.TabPages[i].Tag;
                        break;
                    }
                }
            }

            // 全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                ListTab.SelectedIndex = 0;
                lst = (DetailsListView)ListTab.TabPages[0].Tag;
                // this._curTab = ListTab.TabPages(0)
                if (this._statuses.SortOrder == SortOrder.Ascending)
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
                if (this._statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if (this._statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - this._iconSz - 10 || this._statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < this._iconSz + 10)
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
            if (this._curList.SelectedIndices.Count > 0 && this._statuses.Tabs[this._curTab.Text].TabType != TabUsageType.DirectMessage)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, this._curList.SelectedIndices[0]);
                var sid = post.RetweetedId == 0 ? post.StatusId : post.RetweetedId;
                this.OpenUriAsync("http://twitter.com/" + post.ScreenName + "/status/" + sid.ToString());
            }
        }

        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curList.SelectedIndices.Count > 0)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, this._curList.SelectedIndices[0]);
                this.OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + post.ScreenName + "/recent");
            }
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            this.CheckNewVersion();
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
            bool forceUpdate = this.isKeyDown(Keys.Shift);

            try
            {
                retMsg = this.tw.GetVersionInfo();
            }
            catch (Exception)
            {
                StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
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
                if (!string.IsNullOrEmpty(MyCommon.fileVersion) && strVer.CompareTo(MyCommon.fileVersion.Replace(".", string.Empty)) > 0)
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
                        MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText7 + MyCommon.fileVersion.Replace(".", string.Empty) + Hoehoe.Properties.Resources.CheckNewVersionText8 + strVer, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                StatusLabel.Text = Hoehoe.Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText10, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Colorize()
        {
            this._colorize = false;
            this.DispSelectedPost();
            // 件数関連の場合、タイトル即時書き換え
            if (this.SettingDialog.DispLatestPost != DispTitleEnum.None && this.SettingDialog.DispLatestPost != DispTitleEnum.Post && this.SettingDialog.DispLatestPost != DispTitleEnum.Ver && this.SettingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                this.SetMainWindowTitle();
            }
            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                this.SetStatusLabelUrl();
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (this._statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this.SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
            }
            if (!this.SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        public string CreateDetailHtml(string orgdata)
        {
            return this._detailHtmlFormatHeader + orgdata + this._detailHtmlFormatFooter;
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(this.displayItem))
            {
                if (UserPicture.Image != null)
                {
                    UserPicture.Image.Dispose();
                }
                if (this.displayItem.Image != null)
                {
                    try
                    {
                        UserPicture.Image = new Bitmap(this.displayItem.Image);
                    }
                    catch (Exception)
                    {
                        UserPicture.Image = null;
                    }
                }
                else
                {
                    UserPicture.Image = null;
                }
            }
        }

        private void DispSelectedPost()
        {
            this.DispSelectedPost(false);
        }

        PostClass _displayPost;

        private void DispSelectedPost(bool forceupdate)
        {
            if (this._curList.SelectedIndices.Count == 0 || this._curPost == null)
            {
                return;
            }

            if (!forceupdate && this._curPost.Equals(this._displayPost))
            {
                return;
            }

            this._displayPost = this._curPost;
            if (this.displayItem != null)
            {
                this.displayItem.ImageDownloaded -= this.DisplayItemImage_Downloaded;
                this.displayItem = null;
            }
            this.displayItem = (ImageListViewItem)this._curList.Items[this._curList.SelectedIndices[0]];
            this.displayItem.ImageDownloaded += this.DisplayItemImage_Downloaded;

            string detailText = this.CreateDetailHtml(this._curPost.IsDeleted ? "(DELETED)" : this._curPost.Text);
            if (this._curPost.IsDm)
            {
                SourceLinkLabel.Tag = null;
                SourceLinkLabel.Text = string.Empty;
            }
            else
            {
                Match mc = Regex.Match(this._curPost.SourceHtml, "<a href=\"(?<sourceurl>.+?)\"");
                if (mc.Success)
                {
                    string src = mc.Groups["sourceurl"].Value;
                    SourceLinkLabel.Tag = mc.Groups["sourceurl"].Value;
                    mc = Regex.Match(src, "^https?:// ");
                    if (!mc.Success)
                    {
                        src = src.Insert(0, "http://twitter.com");
                    }
                    SourceLinkLabel.Tag = src;
                }
                else
                {
                    SourceLinkLabel.Tag = null;
                }
                if (string.IsNullOrEmpty(this._curPost.Source))
                {
                    SourceLinkLabel.Text = string.Empty;
                }
                else
                {
                    SourceLinkLabel.Text = this._curPost.Source;
                }
            }
            SourceLinkLabel.TabStop = false;

            if (this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage && !this._curPost.IsOwl)
            {
                NameLabel.Text = "DM TO -> ";
            }
            else if (this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                NameLabel.Text = "DM FROM <- ";
            }
            else
            {
                NameLabel.Text = string.Empty;
            }
            NameLabel.Text += this._curPost.ScreenName + "/" + this._curPost.Nickname;
            NameLabel.Tag = this._curPost.ScreenName;
            if (!string.IsNullOrEmpty(this._curPost.RetweetedBy))
            {
                NameLabel.Text += " (RT:" + this._curPost.RetweetedBy + ")";
            }
            if (UserPicture.Image != null)
            {
                UserPicture.Image.Dispose();
            }
            if (!string.IsNullOrEmpty(this._curPost.ImageUrl) && this._TIconDic[this._curPost.ImageUrl] != null)
            {
                try
                {
                    UserPicture.Image = new Bitmap(this._TIconDic[this._curPost.ImageUrl]);
                }
                catch (Exception)
                {
                    UserPicture.Image = null;
                }
            }
            else
            {
                UserPicture.Image = null;
            }

            NameLabel.ForeColor = SystemColors.ControlText;
            DateTimeLabel.Text = this._curPost.CreatedAt.ToString();
            if (this._curPost.IsOwl && (this.SettingDialog.OneWayLove || this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage))
            {
                NameLabel.ForeColor  = this.clrOWL;
            }
            if (this._curPost.RetweetedId > 0)
            {
                NameLabel.ForeColor  = this.clrRetweet;
            }
            if (this._curPost.IsFav)
            {
                NameLabel.ForeColor  = this.clrFav;
            }

            if (DumpPostClassToolStripMenuItem.Checked)
            {
                StringBuilder sb = new StringBuilder(512);

                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", this._curPost.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", this._curPost.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", this._curPost.StatusId.ToString());
                // sb.AppendFormat("ImageIndex     : {0}<br>", this._curPost.ImageIndex.ToString)
                sb.AppendFormat("ImageUrl       : {0}<br>", this._curPost.ImageUrl);
                sb.AppendFormat("InReplyToStatusId    : {0}<br>", this._curPost.InReplyToStatusId.ToString());
                sb.AppendFormat("InReplyToUser  : {0}<br>", this._curPost.InReplyToUser);
                sb.AppendFormat("IsDM           : {0}<br>", this._curPost.IsDm.ToString());
                sb.AppendFormat("IsFav          : {0}<br>", this._curPost.IsFav.ToString());
                sb.AppendFormat("IsMark         : {0}<br>", this._curPost.IsMark.ToString());
                sb.AppendFormat("IsMe           : {0}<br>", this._curPost.IsMe.ToString());
                sb.AppendFormat("IsOwl          : {0}<br>", this._curPost.IsOwl.ToString());
                sb.AppendFormat("IsProtect      : {0}<br>", this._curPost.IsProtect.ToString());
                sb.AppendFormat("IsRead         : {0}<br>", this._curPost.IsRead.ToString());
                sb.AppendFormat("IsReply        : {0}<br>", this._curPost.IsReply.ToString());

                foreach (string nm in this._curPost.ReplyToList)
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName           : {0}<br>", this._curPost.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", this._curPost.Nickname);
                sb.AppendFormat("Text   : {0}<br>", this._curPost.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", this._curPost.Text);
                sb.AppendFormat("CreatedAt          : {0}<br>", this._curPost.CreatedAt.ToString());
                sb.AppendFormat("Source         : {0}<br>", this._curPost.Source);
                sb.AppendFormat("UserId            : {0}<br>", this._curPost.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", this._curPost.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", this._curPost.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", this._curPost.RetweetedId);
                sb.AppendFormat("SearchTabName  : {0}<br>", this._curPost.RelTabName);
                sb.Append("-----End PostClass Dump<br>");

                PostBrowser.Visible = false;
                PostBrowser.DocumentText = this._detailHtmlFormatHeader + sb.ToString() + this._detailHtmlFormatFooter;
                PostBrowser.Visible = true;
            }
            else
            {
                try
                {
                    if (PostBrowser.DocumentText != detailText)
                    {
                        PostBrowser.Visible = false;
                        PostBrowser.DocumentText = detailText;
                        List<string> lnks = new List<string>();
                        foreach (Match lnk in Regex.Matches(detailText, "<a target=\"_self\" href=\"(?<url>http[^\"]+)\"", RegexOptions.IgnoreCase))
                        {
                            lnks.Add(lnk.Result("${url}"));
                        }
                        this._thumbnail.GenThumbnail(this._curPost.StatusId, lnks, this._curPost.PostGeo, this._curPost.Media);
                    }
                }
                catch (COMException comex)
                {
                    // 原因不明
                    System.Diagnostics.Debug.Write(comex);
                }
                catch (UriFormatException)
                {
                    PostBrowser.DocumentText = detailText;
                }
                finally
                {
                    PostBrowser.Visible = true;
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
            if (ListTab.SelectedTab != null)
            {
                if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    Control pnl = ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused || pnl.Controls["comboLang"].Focused || pnl.Controls["buttonSearch"].Focused)
                    {
                        return;
                    }
                }
                ModifierState State = this.GetModifierState(e.Control, e.Shift, e.Alt);
                if (State == ModifierState.NotFlags)
                {
                    return;
                }
                if (State != ModifierState.None)
                {
                    this.anchorFlag = false;
                }
                if (this.CommonKeyDown(e.KeyCode, FocusedControl.ListTab, State))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
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

        [Flags]
        private enum ModifierState : int
        {
            None = 0,
            Alt = 1,
            Shift = 2,
            Ctrl = 4,
            // CShift = 11
            // CAlt = 12
            // AShift = 13
            NotFlags = 8

            // ListTab = 101
            // PostBrowser = 102
            // StatusText = 103
        }

        private enum FocusedControl : int
        {
            None,
            ListTab,
            StatusText,
            PostBrowser
        }

        private bool CommonKeyDown(Keys keyCode, FocusedControl focusedControl, ModifierState modifierState)
        {
            bool functionReturnValue = false;
            // リストのカーソル移動関係（上下キー、PageUp/Downに該当）
            if (focusedControl == FocusedControl.ListTab)
            {
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
                                if (ListTab.SelectedTab != null)
                                {
                                    TabUsageType tabtype = this._statuses.Tabs[ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == TabUsageType.Related || tabtype == TabUsageType.UserTimeline || tabtype == TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = ListTab.SelectedTab;
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
                            this.doRepliedStatusOpen();
                            return true;
                        case Keys.Q:
                            this.doQuote();
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
                            if (this._curList.SelectedIndices.Count > 0)
                            {
                                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this._curList.SelectedIndices[0]).ScreenName);
                            }
                            else if (this._curList.SelectedIndices.Count == 0)
                            {
                                this.OpenUriAsync("http://twitter.com/");
                            }
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            if (this._curList.SelectedIndices.Count > 0)
                            {
                                this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this._curList.SelectedIndices[0]).ScreenName + "/favorites");
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
                                this._colorize = true;
                                // スルーする
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
                                if (ListTab.TabPages.Count < tabNo)
                                {
                                    return functionReturnValue;
                                }
                                ListTab.SelectedIndex = tabNo;
                                this.ListTabSelect(ListTab.TabPages[tabNo]);
                                return true;
                            case Keys.D9:
                                ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                                this.ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                                return true;
                        }
                    }
                    else if (focusedControl == FocusedControl.StatusText)
                    {
                        // フォーカスStatusText
                        switch (keyCode)
                        {
                            case Keys.A:
                                StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!string.IsNullOrEmpty(StatusText.Text.Trim()))
                                {
                                    this._history[_hisIdx] = new PostingStatus(StatusText.Text, this._replyToId, this._replyToName);
                                }
                                if (keyCode == Keys.Up)
                                {
                                    this._hisIdx -= 1;
                                    if (_hisIdx < 0)
                                    {
                                        this._hisIdx = 0;
                                    }
                                }
                                else
                                {
                                    this._hisIdx += 1;
                                    if (_hisIdx > this._history.Count - 1)
                                    {
                                        this._hisIdx = this._history.Count - 1;
                                    }
                                }
                                StatusText.Text = this._history[_hisIdx].Status;
                                this._replyToId = this._history[_hisIdx].InReplyToId;
                                this._replyToName = this._history[_hisIdx].InReplyToName;
                                StatusText.SelectionStart = StatusText.Text.Length;
                                return true;
                            case Keys.PageUp:
                            case Keys.P:
                                if (ListTab.SelectedIndex == 0)
                                {
                                    ListTab.SelectedIndex = ListTab.TabCount - 1;
                                }
                                else
                                {
                                    ListTab.SelectedIndex -= 1;
                                }
                                StatusText.Focus();
                                return true;
                            case Keys.PageDown:
                            case Keys.N:
                                if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                                {
                                    ListTab.SelectedIndex = 0;
                                }
                                else
                                {
                                    ListTab.SelectedIndex += 1;
                                }
                                StatusText.Focus();
                                return true;
                        }
                    }
                    else
                    {
                        // フォーカスPostBrowserもしくは関係なし
                        switch (keyCode)
                        {
                            case Keys.A:
                                PostBrowser.Document.ExecCommand("SelectAll", false, null);
                                return true;
                            case Keys.C:
                            case Keys.Insert:
                                string selText = WebBrowser_GetSelectionText(ref PostBrowser);
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
                            this.doReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (this._curPost != null)
                            {
                                this.doShowUserStatus(this._curPost.ScreenName, false);
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
                            if (ListTab.SelectedTab != null)
                            {
                                if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                                {
                                    ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
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
                            this.doMoveToRTHome();
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
                                    if (this._curList != null && this._curList.Items.Count != 0 && this._curList.SelectedIndices.Count > 0 && this._curList.SelectedIndices[0] > 0)
                                    {
                                        idx = this._curList.SelectedIndices[0] - 1;
                                        this.SelectListItem(this._curList, idx);
                                        this._curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }
                                break;
                            case Keys.Down:
                                {
                                    int idx = 0;
                                    if (this._curList != null && this._curList.Items.Count != 0 && this._curList.SelectedIndices.Count > 0 && this._curList.SelectedIndices[0] < this._curList.Items.Count - 1)
                                    {
                                        idx = this._curList.SelectedIndices[0] + 1;
                                        this.SelectListItem(this._curList, idx);
                                        this._curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }
                                break;
                            case Keys.Space:
                                if (StatusText.SelectionStart > 0)
                                {
                                    int endidx = StatusText.SelectionStart - 1;
                                    string startstr = string.Empty;
                                    bool pressed = false;
                                    for (int i = StatusText.SelectionStart - 1; i >= 0; i--)
                                    {
                                        char c = StatusText.Text[i];
                                        if (char.IsLetterOrDigit(c) || c == '_')
                                        {
                                            continue;
                                        }
                                        if (c == '@')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            int cnt = this.AtIdSupl.ItemCount;
                                            this.ShowSuplDialog(StatusText, this.AtIdSupl, startstr.Length + 1, startstr);
                                            if (this.AtIdSupl.ItemCount != cnt)
                                            {
                                                this._modifySettingAtId = true;
                                            }
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            this.ShowSuplDialog(StatusText, this.HashSupl, startstr.Length + 1, startstr);
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
                                // ソートダイレクト選択(Ctrl+Shift+1～8,Ctrl+Shift+9)
                                {
                                    int colNo = keyCode - Keys.D1;
                                    DetailsListView lst = (DetailsListView)ListTab.SelectedTab.Tag;
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
                                    DetailsListView lst = (DetailsListView)ListTab.SelectedTab.Tag;
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
                            this.doReTweetUnofficial();
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
                            this.doTranslation(this._curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            this.doReTweetUnofficial();
                            return true;
                        case Keys.C:
                            this.CopyUserId();
                            return true;
                        case Keys.Up:
                            this._thumbnail.ScrollThumbnail(false);
                            return true;
                        case Keys.Down:
                            this._thumbnail.ScrollThumbnail(true);
                            return true;
                    }
                    if (focusedControl == FocusedControl.ListTab && keyCode == Keys.Enter)
                    {
                        if (!this.SplitContainer3.Panel2Collapsed)
                        {
                            this._thumbnail.OpenPicture();
                        }
                        return true;
                    }
                    break;
            }
            return functionReturnValue;
        }

        private void ScrollDownPostBrowser(bool forward)
        {
            HtmlDocument doc = PostBrowser.Document;
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
                doc.Body.ScrollTop += this.SettingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= this.SettingDialog.FontDetail.Height;
            }
        }

        private void PageDownPostBrowser(bool forward)
        {
            HtmlDocument doc = PostBrowser.Document;
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
                doc.Body.ScrollTop += PostBrowser.ClientRectangle.Height - this.SettingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= PostBrowser.ClientRectangle.Height - this.SettingDialog.FontDetail.Height;
            }
        }

        private void GoNextTab(bool forward)
        {
            int idx = ListTab.SelectedIndex;
            if (forward)
            {
                idx += 1;
                if (idx > ListTab.TabPages.Count - 1)
                {
                    idx = 0;
                }
            }
            else
            {
                idx -= 1;
                if (idx < 0)
                {
                    idx = ListTab.TabPages.Count - 1;
                }
            }
            ListTab.SelectedIndex = idx;
            this.ListTabSelect(ListTab.TabPages[idx]);
        }

        private void CopyStot()
        {
            string clstr = string.Empty;
            StringBuilder sb = new StringBuilder();
            bool IsProtected = false;
            bool isDm = false;
            if (this._curTab != null && this._statuses.GetTabByName(this._curTab.Text) != null)
            {
                isDm = this._statuses.GetTabByName(this._curTab.Text).TabType == TabUsageType.DirectMessage;
            }
            foreach (int idx in this._curList.SelectedIndices)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, idx);
                if (post.IsProtect)
                {
                    IsProtected = true;
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
            if (IsProtected)
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
            if (this._curTab == null)
            {
                return;
            }
            if (this._statuses.GetTabByName(this._curTab.Text) == null)
            {
                return;
            }
            if (this._statuses.GetTabByName(this._curTab.Text).TabType == TabUsageType.DirectMessage)
            {
                return;
            }
            foreach (int idx in this._curList.SelectedIndices)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, idx);
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
            if (this._curList.VirtualListSize == 0)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                if (this._curList.SelectedIndices.Count == 0)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = this._curList.SelectedIndices[0] + 1;
                    if (fIdx > this._curList.VirtualListSize - 1)
                    {
                        return;
                    }
                }
                toIdx = this._curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                if (this._curList.SelectedIndices.Count == 0)
                {
                    fIdx = this._curList.VirtualListSize - 1;
                }
                else
                {
                    fIdx = this._curList.SelectedIndices[0] - 1;
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
                if (this._statuses.Item(this._curTab.Text, idx).IsFav)
                {
                    this.SelectListItem(this._curList, idx);
                    this._curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (this._curList.VirtualListSize == 0)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            long targetId = 0;

            if (this._statuses.Tabs[this._curTab.Text].TabType == TabUsageType.DirectMessage)
            {
                return;
            }
            // Directタブは対象外（見つかるはずがない）
            if (this._curList.SelectedIndices.Count == 0)
            {
                return;
            }
            // 未選択も処理しない

            targetId = this.GetCurTabPost(this._curList.SelectedIndices[0]).StatusId;

            if (left)
            {
                // 左のタブへ
                if (ListTab.SelectedIndex == 0)
                {
                    return;
                }
                else
                {
                    fIdx = ListTab.SelectedIndex - 1;
                }
                toIdx = 0;
                stp = -1;
            }
            else
            {
                // 右のタブへ
                if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                {
                    return;
                }
                else
                {
                    fIdx = ListTab.SelectedIndex + 1;
                }
                toIdx = ListTab.TabCount - 1;
                stp = 1;
            }

            bool found = false;
            for (int tabidx = fIdx; tabidx <= toIdx; tabidx += stp)
            {
                if (this._statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage)
                    continue;
                // Directタブは対象外
                for (int idx = 0; idx <= ((DetailsListView)ListTab.TabPages[tabidx].Tag).VirtualListSize - 1; idx++)
                {
                    if (this._statuses.Item(ListTab.TabPages[tabidx].Text, idx).StatusId == targetId)
                    {
                        ListTab.SelectedIndex = tabidx;
                        this.ListTabSelect(ListTab.TabPages[tabidx]);
                        this.SelectListItem(this._curList, idx);
                        this._curList.EnsureVisible(idx);
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
            if (this._curList.SelectedIndices.Count == 0 || this._curPost == null)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                fIdx = this._curList.SelectedIndices[0] + 1;
                if (fIdx > this._curList.VirtualListSize - 1)
                {
                    return;
                }
                toIdx = this._curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this._curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }
                toIdx = 0;
                stp = -1;
            }

            string name = string.Empty;
            if (this._curPost.RetweetedId == 0)
            {
                name = this._curPost.ScreenName;
            }
            else
            {
                name = this._curPost.RetweetedBy;
            }
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                if (this._statuses.Item(this._curTab.Text, idx).RetweetedId == 0)
                {
                    if (this._statuses.Item(this._curTab.Text, idx).ScreenName == name)
                    {
                        this.SelectListItem(this._curList, idx);
                        this._curList.EnsureVisible(idx);
                        break;
                    }
                }
                else
                {
                    if (this._statuses.Item(this._curTab.Text, idx).RetweetedBy == name)
                    {
                        this.SelectListItem(this._curList, idx);
                        this._curList.EnsureVisible(idx);
                        break;
                    }
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (this._curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            if (forward)
            {
                fIdx = this._curList.SelectedIndices[0] + 1;
                if (fIdx > this._curList.VirtualListSize - 1)
                {
                    return;
                }
                toIdx = this._curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = this._curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }
                toIdx = 0;
                stp = -1;
            }

            if (!this.anchorFlag)
            {
                if (this._curPost == null)
                {
                    return;
                }
                this.anchorPost = this._curPost;
                this.anchorFlag = true;
            }
            else
            {
                if (this.anchorPost == null)
                {
                    return;
                }
            }

            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, idx);
                if (post.ScreenName == this.anchorPost.ScreenName || post.RetweetedBy == this.anchorPost.ScreenName || post.ScreenName == this.anchorPost.RetweetedBy || (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == this.anchorPost.RetweetedBy) || this.anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) || this.anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) || post.ReplyToList.Contains(this.anchorPost.ScreenName.ToLower()) || post.ReplyToList.Contains(this.anchorPost.RetweetedBy.ToLower()))
                {
                    this.SelectListItem(this._curList, idx);
                    this._curList.EnsureVisible(idx);
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
            int idx = this._statuses.Tabs[this._curTab.Text].IndexOf(this.anchorPost.StatusId);
            if (idx == -1)
            {
                return;
            }
            this.SelectListItem(this._curList, idx);
            this._curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool goTop)
        {
            ListViewItem item = null;
            int idx = 0;

            if (goTop)
            {
                item = this._curList.GetItemAt(0, 25);
                idx = item != null ? item.Index : 0;
            }
            else
            {
                item = this._curList.GetItemAt(0, this._curList.ClientSize.Height - 1);
                idx = item != null ? item.Index : this._curList.VirtualListSize - 1;
            }
            this.SelectListItem(this._curList, idx);
        }

        private void GoMiddle()
        {
            ListViewItem item = this._curList.GetItemAt(0, 0);
            int idx1 = item == null ? 0 : item.Index;

            item = this._curList.GetItemAt(0, this._curList.ClientSize.Height - 1);
            int idx2 = item == null ? this._curList.VirtualListSize - 1 : item.Index;

            int idx3 = (idx1 + idx2) / 2;

            this.SelectListItem(this._curList, idx3);
        }

        private void GoLast()
        {
            if (this._curList.VirtualListSize == 0)
            {
                return;
            }

            if (this._statuses.SortOrder == SortOrder.Ascending)
            {
                this.SelectListItem(this._curList, this._curList.VirtualListSize - 1);
                this._curList.EnsureVisible(this._curList.VirtualListSize - 1);
            }
            else
            {
                this.SelectListItem(this._curList, 0);
                this._curList.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            if (this._curList.SelectedIndices.Count == 0)
            {
                return;
            }
            int idx = this._curList.SelectedIndices[0];
            if (this._statuses.SortOrder == SortOrder.Ascending)
            {
                this._curList.EnsureVisible(this._curList.VirtualListSize - 1);
            }
            else
            {
                this._curList.EnsureVisible(0);
            }
            this._curList.EnsureVisible(idx);
        }

        private void GoInReplyToPostTree()
        {
            if (this._curPost == null)
            {
                return;
            }

            TabClass curTabClass = this._statuses.Tabs[this._curTab.Text];

            if (curTabClass.TabType == TabUsageType.PublicSearch && this._curPost.InReplyToStatusId == 0 && this._curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = this.tw.GetStatusApi(false, this._curPost.StatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    this._curPost.InReplyToStatusId = post.InReplyToStatusId;
                    this._curPost.InReplyToUser = post.InReplyToUser;
                    this._curPost.IsReply = post.IsReply;
                    this._itemCache = null;
                    this._curList.RedrawItems(this._curItemIndex, this._curItemIndex, false);
                }
                else
                {
                    this.StatusLabel.Text = r;
                }
            }

            if (!(this.ExistCurrentPost && this._curPost.InReplyToUser != null && this._curPost.InReplyToStatusId > 0))
            {
                return;
            }

            if (this._replyChains == null || (this._replyChains.Count > 0 && this._replyChains.Peek().InReplyToId != this._curPost.StatusId))
            {
                this._replyChains = new Stack<ReplyChain>();
            }
            this._replyChains.Push(new ReplyChain(this._curPost.StatusId, this._curPost.InReplyToStatusId, this._curTab));

            int inReplyToIndex = 0;
            string inReplyToTabName = null;
            long inReplyToId = this._curPost.InReplyToStatusId;
            string inReplyToUser = this._curPost.InReplyToUser;
            Dictionary<long, PostClass> curTabPosts = null;

            if (this._statuses.Tabs[this._curTab.Text].IsInnerStorageTabType)
            {
                curTabPosts = curTabClass.Posts;
            }
            else
            {
                curTabPosts = this._statuses.Posts;
            }

            var inReplyToPosts = from tab in this._statuses.Tabs.Values
                                 orderby !object.ReferenceEquals(tab, curTabClass)
                                 from post in ((Dictionary<long, PostClass>)(tab.IsInnerStorageTabType ? tab.Posts : this._statuses.Posts)).Values
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
                string r = this.tw.GetStatusApi(false, this._curPost.InReplyToStatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    post.IsRead = true;
                    this._statuses.AddPost(post);
                    this._statuses.DistributePosts();
                    // this._statuses.SubmitUpdate(Nothing, Nothing, Nothing, False)
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

            if (!object.ReferenceEquals(this._curTab, tabPage))
            {
                this.ListTab.SelectTab(tabPage);
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (this._curPost == null)
            {
                return;
            }

            TabClass curTabClass = this._statuses.Tabs[this._curTab.Text];
            Dictionary<long, PostClass> curTabPosts = (Dictionary<long, PostClass>)(curTabClass.IsInnerStorageTabType ? curTabClass.Posts : this._statuses.Posts);

            if (parallel)
            {
                if (this._curPost.InReplyToStatusId != 0)
                {
                    var posts = from t in this._statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this._statuses.Posts)
                                where p.Value.StatusId != this._curPost.StatusId && p.Value.InReplyToStatusId == this._curPost.InReplyToStatusId
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
                        var post = postList.FirstOrDefault(pst => object.ReferenceEquals(pst.Tab, curTabClass) && (bool)(isForward ? pst.Index > this._curItemIndex : pst.Index < this._curItemIndex));
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
                if (this._replyChains == null || this._replyChains.Count < 1)
                {
                    var posts = from t in this._statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : this._statuses.Posts)
                                where p.Value.InReplyToStatusId == this._curPost.StatusId
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
                    ReplyChain chainHead = this._replyChains.Pop();
                    if (chainHead.InReplyToId == this._curPost.StatusId)
                    {
                        int idx = this._statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                        if (idx == -1)
                        {
                            this._replyChains = null;
                        }
                        else
                        {
                            try
                            {
                                ListTab.SelectTab(chainHead.OriginalTab);
                            }
                            catch (Exception)
                            {
                                this._replyChains = null;
                            }
                            this.SelectListItem(this._curList, idx);
                            this._curList.EnsureVisible(idx);
                        }
                    }
                    else
                    {
                        this._replyChains = null;
                        this.GoBackInReplyToPostTree(parallel);
                    }
                }
            }
        }

        private void GoBackSelectPostChain()
        {
            try
            {
                this._selectPostChains.Pop();
                var tabPostPair = this._selectPostChains.Pop();
                if (!this.ListTab.TabPages.Contains(tabPostPair.Item1))
                {
                    return;
                }
                this.ListTab.SelectedTab = tabPostPair.Item1;
                if (tabPostPair.Item2 != null && this._statuses.Tabs[this._curTab.Text].IndexOf(tabPostPair.Item2.StatusId) > -1)
                {
                    this.SelectListItem(this._curList, this._statuses.Tabs[this._curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                    this._curList.EnsureVisible(this._statuses.Tabs[this._curTab.Text].IndexOf(tabPostPair.Item2.StatusId));
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void PushSelectPostChain()
        {
            if (this._selectPostChains.Count == 0 || (this._selectPostChains.Peek().Item1.Text != this._curTab.Text || !object.ReferenceEquals(this._curPost, this._selectPostChains.Peek().Item2)))
            {
                this._selectPostChains.Push(Tuple.Create(this._curTab, this._curPost));
            }
        }

        private void TrimPostChain()
        {
            if (this._selectPostChains.Count < 2000)
            {
                return;
            }
            Stack<Tuple<TabPage, PostClass>> p = new Stack<Tuple<TabPage, PostClass>>();
            for (var i = 0; i < 2000; i++)
            {
                p.Push(this._selectPostChains.Pop());
            }
            this._selectPostChains.Clear();
            for (var i = 0; i < 2000; i++)
            {
                this._selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(long statusId)
        {
            if (statusId == 0)
            {
                return false;
            }
            for (int tabidx = 0; tabidx <= ListTab.TabCount - 1; tabidx++)
            {
                if (this._statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType != TabUsageType.DirectMessage && this._statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this._statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(ListTab.TabPages[tabidx]);
                    this.SelectListItem(this._curList, idx);
                    this._curList.EnsureVisible(idx);
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
            for (int tabidx = 0; tabidx < ListTab.TabCount; tabidx++)
            {
                if (this._statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == TabUsageType.DirectMessage && this._statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = this._statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    this.ListTabSelect(ListTab.TabPages[tabidx]);
                    this.SelectListItem(this._curList, idx);
                    this._curList.EnsureVisible(idx);
                    return true;
                }
            }
            return false;
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            this.anchorFlag = false;
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            // フォーカスの戻り先を StatusText に設定
            this.Tag = StatusText;
            StatusText.BackColor  = this.clrInputBackcolor;
        }

        public Color InputBackColor
        {
            get { return this.clrInputBackcolor; }
            set { this.clrInputBackcolor = value; }
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (ListTab.SelectedTab != null && MenuStrip1.Tag == null)
            {
                this.Tag = ListTab.SelectedTab.Tag;
            }
            StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            ModifierState State = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags)
            {
                return;
            }
            if (this.CommonKeyDown(e.KeyCode, FocusedControl.StatusText, State))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            this.StatusText_TextChanged(null, null);
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
                if (this._modifySettingCommon)
                {
                    this.SaveConfigsCommon();
                }
                if (this._modifySettingLocal)
                {
                    this.SaveConfigsLocal();
                }
                if (this._modifySettingAtId)
                {
                    this.SaveConfigsAtId();
                }
            }
        }

        private void SaveConfigsAtId()
        {
            if (this._ignoreConfigSave || !this.SettingDialog.UseAtIdSupplement && this.AtIdSupl == null)
            {
                return;
            }

            this._modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(this.AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (this._ignoreConfigSave)
            {
                return;
            }

            this._modifySettingCommon = false;
            lock (this._syncObject)
            {
                this._cfgCommon.UserName = this.tw.Username;
                this._cfgCommon.UserId = this.tw.UserId;
                this._cfgCommon.Password = this.tw.Password;
                this._cfgCommon.Token = this.tw.AccessToken;
                this._cfgCommon.TokenSecret = this.tw.AccessTokenSecret;
                this._cfgCommon.UserAccounts = this.SettingDialog.UserAccounts;
                this._cfgCommon.UserstreamStartup = this.SettingDialog.UserstreamStartup;
                this._cfgCommon.UserstreamPeriod = this.SettingDialog.UserstreamPeriodInt;
                this._cfgCommon.TimelinePeriod = this.SettingDialog.TimelinePeriodInt;
                this._cfgCommon.ReplyPeriod = this.SettingDialog.ReplyPeriodInt;
                this._cfgCommon.DMPeriod = this.SettingDialog.DMPeriodInt;
                this._cfgCommon.PubSearchPeriod = this.SettingDialog.PubSearchPeriodInt;
                this._cfgCommon.ListsPeriod = this.SettingDialog.ListsPeriodInt;
                this._cfgCommon.UserTimelinePeriod = this.SettingDialog.UserTimelinePeriodInt;
                this._cfgCommon.Read = this.SettingDialog.Readed;
                this._cfgCommon.IconSize = this.SettingDialog.IconSz;
                this._cfgCommon.UnreadManage = this.SettingDialog.UnreadManage;
                this._cfgCommon.PlaySound = this.SettingDialog.PlaySound;
                this._cfgCommon.OneWayLove = this.SettingDialog.OneWayLove;
                this._cfgCommon.NameBalloon = this.SettingDialog.NameBalloon;
                this._cfgCommon.PostCtrlEnter = this.SettingDialog.PostCtrlEnter;
                this._cfgCommon.PostShiftEnter = this.SettingDialog.PostShiftEnter;
                this._cfgCommon.CountApi = this.SettingDialog.CountApi;
                this._cfgCommon.CountApiReply = this.SettingDialog.CountApiReply;
                this._cfgCommon.PostAndGet = this.SettingDialog.PostAndGet;
                this._cfgCommon.DispUsername = this.SettingDialog.DispUsername;
                this._cfgCommon.MinimizeToTray = this.SettingDialog.MinimizeToTray;
                this._cfgCommon.CloseToExit = this.SettingDialog.CloseToExit;
                this._cfgCommon.DispLatestPost = this.SettingDialog.DispLatestPost;
                this._cfgCommon.SortOrderLock = this.SettingDialog.SortOrderLock;
                this._cfgCommon.TinyUrlResolve = this.SettingDialog.TinyUrlResolve;
                this._cfgCommon.ShortUrlForceResolve = this.SettingDialog.ShortUrlForceResolve;
                this._cfgCommon.PeriodAdjust = this.SettingDialog.PeriodAdjust;
                this._cfgCommon.StartupVersion = this.SettingDialog.StartupVersion;
                this._cfgCommon.StartupFollowers = this.SettingDialog.StartupFollowers;
                this._cfgCommon.RestrictFavCheck = this.SettingDialog.RestrictFavCheck;
                this._cfgCommon.AlwaysTop = this.SettingDialog.AlwaysTop;
                this._cfgCommon.UrlConvertAuto = this.SettingDialog.UrlConvertAuto;
                this._cfgCommon.Outputz = this.SettingDialog.OutputzEnabled;
                this._cfgCommon.OutputzKey = this.SettingDialog.OutputzKey;
                this._cfgCommon.OutputzUrlMode = this.SettingDialog.OutputzUrlmode;
                this._cfgCommon.UseUnreadStyle = this.SettingDialog.UseUnreadStyle;
                this._cfgCommon.DateTimeFormat = this.SettingDialog.DateTimeFormat;
                this._cfgCommon.DefaultTimeOut = this.SettingDialog.DefaultTimeOut;
                this._cfgCommon.RetweetNoConfirm = this.SettingDialog.RetweetNoConfirm;
                this._cfgCommon.LimitBalloon = this.SettingDialog.LimitBalloon;
                this._cfgCommon.EventNotifyEnabled = this.SettingDialog.EventNotifyEnabled;
                this._cfgCommon.EventNotifyFlag = this.SettingDialog.EventNotifyFlag;
                this._cfgCommon.IsMyEventNotifyFlag = this.SettingDialog.IsMyEventNotifyFlag;
                this._cfgCommon.ForceEventNotify = this.SettingDialog.ForceEventNotify;
                this._cfgCommon.FavEventUnread = this.SettingDialog.FavEventUnread;
                this._cfgCommon.TranslateLanguage = this.SettingDialog.TranslateLanguage;
                this._cfgCommon.EventSoundFile = this.SettingDialog.EventSoundFile;
                this._cfgCommon.AutoShortUrlFirst = this.SettingDialog.AutoShortUrlFirst;
                this._cfgCommon.TabIconDisp = this.SettingDialog.TabIconDisp;
                this._cfgCommon.ReplyIconState = this.SettingDialog.ReplyIconState;
                this._cfgCommon.ReadOwnPost = this.SettingDialog.ReadOwnPost;
                this._cfgCommon.GetFav = this.SettingDialog.GetFav;
                this._cfgCommon.IsMonospace = this.SettingDialog.IsMonospace;
                if (IdeographicSpaceToSpaceToolStripMenuItem != null && IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed == false)
                {
                    this._cfgCommon.WideSpaceConvert = this.IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }
                this._cfgCommon.ReadOldPosts = this.SettingDialog.ReadOldPosts;
                this._cfgCommon.UseSsl = this.SettingDialog.UseSsl;
                this._cfgCommon.BilyUser = this.SettingDialog.BitlyUser;
                this._cfgCommon.BitlyPwd = this.SettingDialog.BitlyPwd;
                this._cfgCommon.ShowGrid = this.SettingDialog.ShowGrid;
                this._cfgCommon.UseAtIdSupplement = this.SettingDialog.UseAtIdSupplement;
                this._cfgCommon.UseHashSupplement = this.SettingDialog.UseHashSupplement;
                this._cfgCommon.PreviewEnable = this.SettingDialog.PreviewEnable;
                this._cfgCommon.Language = this.SettingDialog.Language;

                this._cfgCommon.SortOrder = (int)this._statuses.SortOrder;
                switch (this._statuses.SortMode)
                {
                    case IdComparerClass.ComparerMode.Nickname:
                        // ニックネーム
                        this._cfgCommon.SortColumn = 1;
                        break;
                    case IdComparerClass.ComparerMode.Data:
                        // 本文
                        this._cfgCommon.SortColumn = 2;
                        break;
                    case IdComparerClass.ComparerMode.Id:
                        // 時刻=発言Id
                        this._cfgCommon.SortColumn = 3;
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        // 名前
                        this._cfgCommon.SortColumn = 4;
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        // Source
                        this._cfgCommon.SortColumn = 7;
                        break;
                }

                this._cfgCommon.Nicoms = this.SettingDialog.Nicoms;
                this._cfgCommon.HashTags = this.HashMgr.HashHistories;
                if (this.HashMgr.IsPermanent)
                {
                    this._cfgCommon.HashSelected = this.HashMgr.UseHash;
                }
                else
                {
                    this._cfgCommon.HashSelected = string.Empty;
                }
                this._cfgCommon.HashIsHead = this.HashMgr.IsHead;
                this._cfgCommon.HashIsPermanent = this.HashMgr.IsPermanent;
                this._cfgCommon.HashIsNotAddToAtReply = this.HashMgr.IsNotAddToAtReply;
                this._cfgCommon.TwitterUrl = this.SettingDialog.TwitterApiUrl;
                this._cfgCommon.TwitterSearchUrl = this.SettingDialog.TwitterSearchApiUrl;
                this._cfgCommon.HotkeyEnabled = this.SettingDialog.HotkeyEnabled;
                this._cfgCommon.HotkeyModifier = this.SettingDialog.HotkeyMod;
                this._cfgCommon.HotkeyKey = this.SettingDialog.HotkeyKey;
                this._cfgCommon.HotkeyValue = this.SettingDialog.HotkeyValue;
                this._cfgCommon.BlinkNewMentions = this.SettingDialog.BlinkNewMentions;
                if (ToolStripFocusLockMenuItem != null && ToolStripFocusLockMenuItem.IsDisposed == false)
                {
                    this._cfgCommon.FocusLockToStatusText = this.ToolStripFocusLockMenuItem.Checked;
                }
                this._cfgCommon.UseAdditionalCount = this.SettingDialog.UseAdditionalCount;
                this._cfgCommon.MoreCountApi = this.SettingDialog.MoreCountApi;
                this._cfgCommon.FirstCountApi = this.SettingDialog.FirstCountApi;
                this._cfgCommon.SearchCountApi = this.SettingDialog.SearchCountApi;
                this._cfgCommon.FavoritesCountApi = this.SettingDialog.FavoritesCountApi;
                this._cfgCommon.UserTimelineCountApi = this.SettingDialog.UserTimelineCountApi;
                this._cfgCommon.TrackWord = this.tw.TrackWord;
                this._cfgCommon.AllAtReply = this.tw.AllAtReply;
                this._cfgCommon.OpenUserTimeline = this.SettingDialog.OpenUserTimeline;
                this._cfgCommon.ListCountApi = this.SettingDialog.ListCountApi;
                this._cfgCommon.UseImageService = ImageServiceCombo.SelectedIndex;
                this._cfgCommon.ListDoubleClickAction = this.SettingDialog.ListDoubleClickAction;
                this._cfgCommon.UserAppointUrl = this.SettingDialog.UserAppointUrl;
                this._cfgCommon.HideDuplicatedRetweets = this.SettingDialog.HideDuplicatedRetweets;
                this._cfgCommon.IsPreviewFoursquare = this.SettingDialog.IsPreviewFoursquare;
                this._cfgCommon.FoursquarePreviewHeight = this.SettingDialog.FoursquarePreviewHeight;
                this._cfgCommon.FoursquarePreviewWidth = this.SettingDialog.FoursquarePreviewWidth;
                this._cfgCommon.FoursquarePreviewZoom = this.SettingDialog.FoursquarePreviewZoom;
                this._cfgCommon.IsListsIncludeRts = this.SettingDialog.IsListStatusesIncludeRts;
                this._cfgCommon.TabMouseLock = this.SettingDialog.TabMouseLock;
                this._cfgCommon.IsRemoveSameEvent = this.SettingDialog.IsRemoveSameEvent;
                this._cfgCommon.IsUseNotifyGrowl = this.SettingDialog.IsNotifyUseGrowl;

                this._cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (this._ignoreConfigSave)
            {
                return;
            }
            lock (this._syncObject)
            {
                this._modifySettingLocal = false;
                this._cfgLocal.FormSize = this._mySize;
                this._cfgLocal.FormLocation = this._myLoc;
                this._cfgLocal.SplitterDistance = this._mySpDis;
                this._cfgLocal.PreviewDistance = this._mySpDis3;
                this._cfgLocal.StatusMultiline = StatusText.Multiline;
                this._cfgLocal.StatusTextHeight = this._mySpDis2;
                this._cfgLocal.AdSplitterDistance = this._myAdSpDis;
                this._cfgLocal.StatusText = this.SettingDialog.Status;

                this._cfgLocal.FontUnread = this._fntUnread;
                this._cfgLocal.ColorUnread  = this.clrUnread;
                this._cfgLocal.FontRead = this._fntReaded;
                this._cfgLocal.ColorRead  = this.clrRead;
                this._cfgLocal.FontDetail = this._fntDetail;
                this._cfgLocal.ColorDetail  = this.clrDetail;
                this._cfgLocal.ColorDetailBackcolor  = this.clrDetailBackcolor;
                this._cfgLocal.ColorDetailLink  = this.clrDetailLink;
                this._cfgLocal.ColorFav  = this.clrFav;
                this._cfgLocal.ColorOWL  = this.clrOWL;
                this._cfgLocal.ColorRetweet  = this.clrRetweet;
                this._cfgLocal.ColorSelf  = this.clrSelf;
                this._cfgLocal.ColorAtSelf  = this.clrAtSelf;
                this._cfgLocal.ColorTarget  = this.clrTarget;
                this._cfgLocal.ColorAtTarget  = this.clrAtTarget;
                this._cfgLocal.ColorAtFromTarget  = this.clrAtFromTarget;
                this._cfgLocal.ColorAtTo  = this.clrAtTo;
                this._cfgLocal.ColorListBackcolor  = this.clrListBackcolor;
                this._cfgLocal.ColorInputBackcolor  = this.clrInputBackcolor;
                this._cfgLocal.ColorInputFont  = this.clrInputForecolor;
                this._cfgLocal.FontInputFont = this._fntInputFont;

                this._cfgLocal.BrowserPath = this.SettingDialog.BrowserPath;
                this._cfgLocal.UseRecommendStatus = this.SettingDialog.UseRecommendStatus;
                this._cfgLocal.ProxyType = this.SettingDialog.SelectedProxyType;
                this._cfgLocal.ProxyAddress = this.SettingDialog.ProxyAddress;
                this._cfgLocal.ProxyPort = this.SettingDialog.ProxyPort;
                this._cfgLocal.ProxyUser = this.SettingDialog.ProxyUser;
                this._cfgLocal.ProxyPassword = this.SettingDialog.ProxyPassword;
                if (this._ignoreConfigSave)
                {
                    return;
                }
                this._cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            SettingTabs tabSetting = new SettingTabs();
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                if (this._statuses.Tabs[ListTab.TabPages[i].Text].TabType != TabUsageType.Related)
                {
                    tabSetting.Tabs.Add(this._statuses.Tabs[ListTab.TabPages[i].Text]);
                }
            }
            tabSetting.Save();
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine), Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            SaveFileDialog1.FileName = string.Format("HoehoePosts{0:yyMMdd-HHmmss}.tsv", DateTime.Now);
            SaveFileDialog1.InitialDirectory = MyCommon.AppDir;
            SaveFileDialog1.Filter = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = Hoehoe.Properties.Resources.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames)
                {
                    return;
                }
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    if (rslt == DialogResult.Yes)
                    {
                        // All
                        for (int idx = 0; idx <= this._curList.VirtualListSize - 1; idx++)
                        {
                            PostClass post = this._statuses.Item(this._curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }
                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                                post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""),
                                post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl,
                                post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }
                    else
                    {
                        foreach (int idx in this._curList.SelectedIndices)
                        {
                            PostClass post = this._statuses.Item(this._curTab.Text, idx);
                            string protect = string.Empty;
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }
                            sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                                post.Nickname, post.TextFromApi.Replace("\n", string.Empty).Replace("\"", "\"\""),
                                post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl,
                                post.Text.Replace("\n", string.Empty).Replace("\"", "\"\""), protect));
                        }
                    }
                    sw.Close();
                }
            }
            this.TopMost = this.SettingDialog.AlwaysTop;
        }

        private void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState State = this.GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags)
            {
                return;
            }
            bool KeyRes = this.CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, State);
            if (KeyRes)
            {
                e.IsInputKey = true;
            }
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
            this.TopMost = this.SettingDialog.AlwaysTop;
            if (string.IsNullOrEmpty(newTabText))
            {
                return false;
            }
            // 新タブ名存在チェック
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (ListTab.TabPages[i].Text == newTabText)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.Tabs_DoubleClickText1, newTabText);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            // タブ名のリスト作り直し（デフォルトタブ以外は再作成）
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (this._statuses.IsDistributableTab(ListTab.TabPages[i].Text))
                {
                    this.TabDialog.RemoveTab(ListTab.TabPages[i].Text);
                }
                if (ListTab.TabPages[i].Text == tabName)
                {
                    ListTab.TabPages[i].Text = newTabText;
                }
            }
            this._statuses.RenameTab(tabName, newTabText);
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (this._statuses.IsDistributableTab(ListTab.TabPages[i].Text))
                {
                    if (ListTab.TabPages[i].Text == tabName)
                    {
                        ListTab.TabPages[i].Text = newTabText;
                    }
                    this.TabDialog.AddTab(ListTab.TabPages[i].Text);
                }
            }
            this.SaveConfigsCommon();
            this.SaveConfigsTabs();
            this._rclickTabName = newTabText;
            tabName = newTabText;
            return true;
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
            string tn = ListTab.SelectedTab.Text;
            this.TabRename(ref tn);
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.SettingDialog.TabMouseLock)
            {
                return;
            }
            Point cpos = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this._tabDrag = true;
                        this._tabMouseDownPoint = e.Location;
                        break;
                    }
                }
            }
            else
            {
                this._tabDrag = false;
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

            this._tabDrag = false;
            string tn = string.Empty;
            bool bef = false;
            Point cpos = new Point(e.X, e.Y);
            Point spos = ListTab.PointToClient(cpos);
            int i = 0;
            for (i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Left <= spos.X && spos.X <= rect.Right && rect.Top <= spos.Y && spos.Y <= rect.Bottom)
                {
                    tn = ListTab.TabPages[i].Text;
                    bef = spos.X <= (rect.Left + rect.Right) / 2;
                    break;
                }
            }

            // タブのないところにドロップ->最後尾へ移動
            if (string.IsNullOrEmpty(tn))
            {
                tn = ListTab.TabPages[ListTab.TabPages.Count - 1].Text;
                bef = false;
                i = ListTab.TabPages.Count - 1;
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn)
            {
                return;
            }

            this.ReOrderTab(tp.Text, tn, bef);
        }

        public void ReOrderTab(string targetTabText, string baseTabText, bool isBeforeBaseTab)
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

            TabPage mTp = null;
            for (int j = 0; j < ListTab.TabPages.Count; j++)
            {
                if (ListTab.TabPages[j].Text == targetTabText)
                {
                    mTp = ListTab.TabPages[j];
                    ListTab.TabPages.Remove(mTp);
                    if (j < baseIndex)
                    {
                        baseIndex -= 1;
                    }
                    break;
                }
            }
            if (isBeforeBaseTab)
            {
                ListTab.TabPages.Insert(baseIndex, mTp);
            }
            else
            {
                ListTab.TabPages.Insert(baseIndex + 1, mTp);
            }

            ListTab.ResumeLayout();
            this.SaveConfigsTabs();
        }

        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            // isAuto:True=先頭に挿入、False=カーソル位置に挿入
            // isReply:True=@,False=DM
            if (!StatusText.Enabled)
            {
                return;
            }
            if (this._curList == null)
            {
                return;
            }
            if (this._curTab == null)
            {
                return;
            }
            if (!this.ExistCurrentPost)
            {
                return;
            }

            // 複数あてリプライはReplyではなく通常ポスト
            // ↑仕様変更で全部リプライ扱いでＯＫ（先頭ドット付加しない）
            // 090403暫定でドットを付加しないようにだけ修正。単独と複数の処理は統合できると思われる。
            // 090513 all @ replies 廃止の仕様変更によりドット付加に戻し(syo68k)

            if (this._curList.SelectedIndices.Count > 0)
            {
                // アイテムが1件以上選択されている
                if (this._curList.SelectedIndices.Count == 1 && !isAll && this.ExistCurrentPost)
                {
                    // 単独ユーザー宛リプライまたはDM
                    if ((this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                    {
                        // ダイレクトメッセージ
                        StatusText.Text = "D " + this._curPost.ScreenName + " " + StatusText.Text;
                        StatusText.SelectionStart = StatusText.Text.Length;
                        StatusText.Focus();
                        this._replyToId = 0;
                        this._replyToName = string.Empty;
                        return;
                    }
                    if (string.IsNullOrEmpty(StatusText.Text))
                    {
                        // 空の場合
                        // ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        StatusText.Text = "@" + this._curPost.ScreenName + " ";
                        if (this._curPost.RetweetedId > 0)
                        {
                            this._replyToId = this._curPost.RetweetedId;
                        }
                        else
                        {
                            this._replyToId = this._curPost.StatusId;
                        }
                        this._replyToName = this._curPost.ScreenName;
                    }
                    else
                    {
                        // 何か入力済の場合
                        if (isAuto)
                        {
                            // 1件選んでEnter or DoubleClick
                            if (StatusText.Text.Contains("@" + this._curPost.ScreenName + " "))
                            {
                                if (this._replyToId > 0 && this._replyToName == this._curPost.ScreenName)
                                {
                                    // 返信先書き換え
                                    if (this._curPost.RetweetedId > 0)
                                    {
                                        this._replyToId = this._curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        this._replyToId = this._curPost.StatusId;
                                    }
                                    this._replyToName = this._curPost.ScreenName;
                                }
                                return;
                            }
                            if (!StatusText.Text.StartsWith("@"))
                            {
                                // 文頭＠以外
                                if (StatusText.Text.StartsWith(". "))
                                {
                                    // 複数リプライ
                                    StatusText.Text = StatusText.Text.Insert(2, "@" + this._curPost.ScreenName + " ");
                                    this._replyToId = 0;
                                    this._replyToName = string.Empty;
                                }
                                else
                                {
                                    // 単独リプライ
                                    StatusText.Text = "@" + this._curPost.ScreenName + " " + StatusText.Text;
                                    if (this._curPost.RetweetedId > 0)
                                    {
                                        this._replyToId = this._curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        this._replyToId = this._curPost.StatusId;
                                    }
                                    this._replyToName = this._curPost.ScreenName;
                                }
                            }
                            else
                            {
                                // 文頭＠
                                // 複数リプライ
                                StatusText.Text = ". @" + this._curPost.ScreenName + " " + StatusText.Text;
                                this._replyToId = 0;
                                this._replyToName = string.Empty;
                            }
                        }
                        else
                        {
                            // 1件選んでCtrl-Rの場合（返信先操作せず）
                            int sidx = StatusText.SelectionStart;
                            string id = "@" + this._curPost.ScreenName + " ";
                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    id = " " + id;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, id);
                            sidx += id.Length;
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            // this._reply_to_id = 0
                            // this._reply_to_name = Nothing
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
                        string sTxt = StatusText.Text;
                        if (!sTxt.StartsWith(". "))
                        {
                            sTxt = ". " + sTxt;
                            this._replyToId = 0;
                            this._replyToName = string.Empty;
                        }
                        for (int cnt = 0; cnt <= this._curList.SelectedIndices.Count - 1; cnt++)
                        {
                            PostClass post = this._statuses.Item(this._curTab.Text, this._curList.SelectedIndices[cnt]);
                            if (!sTxt.Contains("@" + post.ScreenName + " "))
                            {
                                sTxt = sTxt.Insert(2, "@" + post.ScreenName + " ");
                            }
                        }
                        StatusText.Text = sTxt;
                    }
                    else
                    {
                        // C-S-r or C-r
                        if (this._curList.SelectedIndices.Count > 1)
                        {
                            // 複数ポスト選択

                            string ids = string.Empty;
                            int sidx = StatusText.SelectionStart;
                            for (int cnt = 0; cnt <= this._curList.SelectedIndices.Count - 1; cnt++)
                            {
                                PostClass post = this._statuses.Item(this._curTab.Text, this._curList.SelectedIndices[cnt]);
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
                            if (!StatusText.Text.StartsWith(". "))
                            {
                                StatusText.Text = ". " + StatusText.Text;
                                sidx += 2;
                                this._replyToId = 0;
                                this._replyToName = string.Empty;
                            }
                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            return;
                        }
                        else
                        {
                            // 1件のみ選択のC-S-r（返信元付加する可能性あり）

                            string ids = string.Empty;
                            int sidx = StatusText.SelectionStart;
                            PostClass post = this._curPost;
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
                            if (string.IsNullOrEmpty(StatusText.Text))
                            {
                                // 未入力の場合のみ返信先付加
                                StatusText.Text = ids;
                                StatusText.SelectionStart = ids.Length;
                                StatusText.Focus();
                                if (post.RetweetedId > 0)
                                {
                                    this._replyToId = post.RetweetedId;
                                }
                                else
                                {
                                    this._replyToId = post.StatusId;
                                }
                                this._replyToName = post.ScreenName;
                                return;
                            }

                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            return;
                        }
                    }
                }
                StatusText.SelectionStart = StatusText.Text.Length;
                StatusText.Focus();
            }
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            this._tabDrag = false;
        }

        int _iconCnt = 0;
        int _blinkCnt = 0;
        bool _beBlink;
        bool _isIdle;

        private void RefreshTasktrayIcon(bool forceRefresh)
        {
            if (this._colorize)
            {
                this.Colorize();
            }
            if (!TimerRefreshIcon.Enabled)
            {
                return;
            }

            if (forceRefresh)
            {
                this._isIdle = false;
            }

            this._iconCnt += 1;
            this._blinkCnt += 1;

            bool busy = false;
            foreach (BackgroundWorker bw in this._bw)
            {
                if (bw != null && bw.IsBusy)
                {
                    busy = true;
                    break;
                }
            }

            if (this._iconCnt > 3)
            {
                this._iconCnt = 0;
            }
            if (this._blinkCnt > 10)
            {
                this._blinkCnt = 0;
                // 未保存の変更を保存
                this.SaveConfigsAll(true);
            }

            if (busy)
            {
                NotifyIcon1.Icon = this._NIconRefresh[this._iconCnt];
                this._isIdle = false;
                this._myStatusError = false;
                return;
            }

            TabClass tb = this._statuses.GetTabByType(TabUsageType.Mentions);
            if (this.SettingDialog.ReplyIconState != ReplyIconState.None && tb != null && tb.UnreadCount > 0)
            {
                if (this._blinkCnt > 0)
                {
                    return;
                }
                this._beBlink = !this._beBlink;
                if (this._beBlink || this.SettingDialog.ReplyIconState == ReplyIconState.StaticIcon)
                {
                    NotifyIcon1.Icon = this._ReplyIcon;
                }
                else
                {
                    NotifyIcon1.Icon = this._ReplyIconBlink;
                }
                this._isIdle = false;
                return;
            }

            if (this._isIdle)
            {
                return;
            }
            this._isIdle = true;
            // 優先度：エラー→オフライン→アイドル
            // エラーは更新アイコンでクリアされる
            if (this._myStatusError)
            {
                NotifyIcon1.Icon = this._NIconAtRed;
                return;
            }
            if (MyCommon.IsNetworkAvailable())
            {
                NotifyIcon1.Icon = this._NIconAt;
            }
            else
            {
                NotifyIcon1.Icon = this._NIconAtSmoke;
            }
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            // 200ms
            this.RefreshTasktrayIcon(false);
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            // 右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (string.IsNullOrEmpty(this._rclickTabName) || !object.ReferenceEquals(sender, ContextMenuTabProperty))
            {
                if (ListTab != null && ListTab.SelectedTab != null)
                {
                    this._rclickTabName = ListTab.SelectedTab.Text;
                }
                else
                {
                    return;
                }
            }

            if (this._statuses == null)
            {
                return;
            }
            if (this._statuses.Tabs == null)
            {
                return;
            }

            TabClass tb = this._statuses.Tabs[this._rclickTabName];
            if (tb == null)
            {
                return;
            }

            NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;
            this._soundfileListup = true;
            SoundFileComboBox.Items.Clear();
            this.SoundFileTbComboBox.Items.Clear();
            SoundFileComboBox.Items.Add(string.Empty);
            this.SoundFileTbComboBox.Items.Add(string.Empty);
            DirectoryInfo oDir = new DirectoryInfo(MyCommon.AppDir + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.AppDir, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                SoundFileComboBox.Items.Add(oFile.Name);
                this.SoundFileTbComboBox.Items.Add(oFile.Name);
            }
            int idx = SoundFileComboBox.Items.IndexOf(tb.SoundFile);
            if (idx == -1)
            {
                idx = 0;
            }
            SoundFileComboBox.SelectedIndex = idx;
            this.SoundFileTbComboBox.SelectedIndex = idx;
            this._soundfileListup = false;
            UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            this.TabMenuControl(this._rclickTabName);
        }

        private void TabMenuControl(string tabName)
        {
            if (this._statuses.Tabs[tabName].TabType != TabUsageType.Mentions && this._statuses.IsDefaultTab(tabName))
            {
                FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else if (this._statuses.Tabs[tabName].TabType == TabUsageType.Mentions)
            {
                FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else
            {
                FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                DeleteTabMenuItem.Enabled = true;
                this.DeleteTbMenuItem.Enabled = true;
            }
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            UreadManageMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.UnreadMngTbMenuItem.Checked = UreadManageMenuItem.Checked;

            if (string.IsNullOrEmpty(this._rclickTabName))
            {
                return;
            }
            this.ChangeTabUnreadManage(this._rclickTabName, UreadManageMenuItem.Checked);

            this.SaveConfigsTabs();
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

            this._statuses.SetTabUnreadManage(tabName, isManage);
            if (this.SettingDialog.TabIconDisp)
            {
                if (this._statuses.Tabs[tabName].UnreadCount > 0)
                {
                    ListTab.TabPages[idx].ImageIndex = 0;
                }
                else
                {
                    ListTab.TabPages[idx].ImageIndex = -1;
                }
            }

            if (this._curTab.Text == tabName)
            {
                this._itemCache = null;
                this._postCache = null;
                this._curList.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (!this.SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            NotifyDispMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked;

            if (string.IsNullOrEmpty(this._rclickTabName))
            {
                return;
            }

            this._statuses.Tabs[this._rclickTabName].Notify = NotifyDispMenuItem.Checked;

            this.SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._soundfileListup || string.IsNullOrEmpty(this._rclickTabName))
            {
                return;
            }

            this._statuses.Tabs[this._rclickTabName].SoundFile = (string)((ToolStripComboBox)sender).SelectedItem;

            this.SaveConfigsTabs();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._rclickTabName) || object.ReferenceEquals(sender, this.DeleteTbMenuItem))
            {
                this._rclickTabName = ListTab.SelectedTab.Text;
            }

            this.RemoveSpecifiedTab(this._rclickTabName, true);
            this.SaveConfigsTabs();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._rclickTabName))
            {
                this._rclickTabName = this._statuses.GetTabByType(TabUsageType.Home).TabName;
            }
            this.fltDialog.SetCurrent(this._rclickTabName);
            this.fltDialog.ShowDialog();
            this.TopMost = this.SettingDialog.AlwaysTop;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this._itemCache = null;
                this._postCache = null;
                this._curPost = null;
                this._curItemIndex = -1;
                this._statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this._statuses.Tabs[tb.Text].AllCount;
                    if (this._statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
                if (!this.SettingDialog.TabIconDisp)
                    ListTab.Refresh();
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
                inputName.TabName = this._statuses.GetUniqueTabName();
                inputName.SetIsShowUsage(true);
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }
            this.TopMost = this.SettingDialog.AlwaysTop;
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
                if (!this._statuses.AddTab(tabName, tabUsage, list) || !this.AddNewTab(tabName, false, tabUsage, list))
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
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        this.ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                        ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                    }
                    if (tabUsage == TabUsageType.Lists)
                    {
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        this.ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                        this.GetTimeline(WorkerType.List, 1, 0, tabName);
                    }
                }
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            // 選択発言を元にフィルタ追加
            foreach (int idx in this._curList.SelectedIndices)
            {
                string tabName = string.Empty;
                // タブ選択（or追加）
                if (!this.SelectTab(ref tabName))
                {
                    return;
                }

                this.fltDialog.SetCurrent(tabName);
                PostClass statusesItem = this._statuses.Item(this._curTab.Text, idx);
                if (statusesItem.RetweetedId == 0)
                {
                    this.fltDialog.AddNewFilter(statusesItem.ScreenName, statusesItem.TextFromApi);
                }
                else
                {
                    this.fltDialog.AddNewFilter(statusesItem.RetweetedBy, statusesItem.TextFromApi);
                }
                this.fltDialog.ShowDialog();
                this.TopMost = this.SettingDialog.AlwaysTop;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this._itemCache = null;
                this._postCache = null;
                this._curPost = null;
                this._curItemIndex = -1;
                this._statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this._statuses.Tabs[tb.Text].AllCount;
                    if (this._statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
                if (!this.SettingDialog.TabIconDisp)
                    ListTab.Refresh();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            this.SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null && ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                this._curPost = this._statuses.Item(this.ListTab.SelectedTab.Text, ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

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
                    if (this.SettingDialog.PostCtrlEnter)
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
                    else if (this.SettingDialog.PostShiftEnter)
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
                            StatusText.Text = StatusText.Text.Remove(pos1, StatusText.SelectionLength);
                            // 選択状態文字列削除
                        }
                        StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine);
                        // 改行挿入
                        StatusText.SelectionStart = pos1 + Environment.NewLine.Length;
                        // カーソルを改行の次の文字へ移動
                        return true;
                    }
                    else if (doPost)
                    {
                        this.PostButton_Click(null, null);
                        return true;
                    }
                }
                else if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch && (ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focused || ListTab.SelectedTab.Controls["panelSearch"].Controls["comboLang"].Focused))
                {
                    this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true, true);
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = string.Empty;

            // 未選択なら処理終了
            if (this._curList.SelectedIndices.Count == 0)
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
            foreach (int idx in this._curList.SelectedIndices)
            {
                PostClass post = this._statuses.Item(this._curTab.Text, idx);
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
                    this._statuses.Tabs[tabName].AddFilter(fc);
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
                    this._modifySettingAtId = true;
                }
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this._itemCache = null;
                this._postCache = null;
                this._curPost = null;
                this._curItemIndex = -1;
                this._statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = this._statuses.Tabs[tb.Text].AllCount;
                    if (this._statuses.ContainsTab(tb.Text))
                    {
                        if (this._statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (this.SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (this.SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }
                if (!this.SettingDialog.TabIconDisp)
                {
                    ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            this.SaveConfigsTabs();
        }

        private bool SelectTab(ref string tabName)
        {
            do
            {
                // 振り分け先タブ選択
                if (this.TabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = this.SettingDialog.AlwaysTop;
                    return false;
                }
                this.TopMost = this.SettingDialog.AlwaysTop;
                tabName = this.TabDialog.SelectedTabName;

                ListTab.SelectedTab.Focus();
                // 新規タブを選択→タブ作成
                if (tabName == Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText1)
                {
                    using (InputTabName inputName = new InputTabName())
                    {
                        inputName.TabName = this._statuses.GetUniqueTabName();
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                        tabName = inputName.TabName;
                        inputName.Dispose();
                    }
                    this.TopMost = this.SettingDialog.AlwaysTop;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        if (!this._statuses.AddTab(tabName, TabUsageType.UserDefined, null) || !this.AddNewTab(tabName, false, TabUsageType.UserDefined))
                        {
                            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            // もう一度タブ名入力
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
            } while (true);
        }

        private void MoveOrCopy(ref bool move, ref bool mark)
        {
            // 移動するか？
            string _tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
            if (MessageBox.Show(_tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                _tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText6, "\r\n");
                if (MessageBox.Show(_tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    mark = true;
                }
                else
                {
                    mark = false;
                }
            }
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
            if (StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                for (int i = 0; i <= this._curList.VirtualListSize - 1; i++)
                {
                    this._curList.SelectedIndices.Add(i);
                }
            }
        }

        private void MoveMiddle()
        {
            if (this._curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = this._curList.SelectedIndices[0];

            ListViewItem item = this._curList.GetItemAt(0, 25);
            int idx1 = item == null ? 0 : item.Index;

            item = this._curList.GetItemAt(0, this._curList.ClientSize.Height - 1);
            int idx2 = item == null ? this._curList.VirtualListSize - 1 : item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0)
            {
                idx = 0;
            }
            this._curList.EnsureVisible(this._curList.VirtualListSize - 1);
            this._curList.EnsureVisible(idx);
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            if (PostBrowser.Document.Links.Count > 0)
            {
                this.UrlDialog.ClearUrl();

                string openUrlStr = string.Empty;

                if (PostBrowser.Document.Links.Count == 1)
                {
                    string urlStr = string.Empty;
                    try
                    {
                        urlStr = MyCommon.IDNDecode(PostBrowser.Document.Links[0].GetAttribute("href"));
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
                    openUrlStr = MyCommon.urlEncodeMultibyteChar(urlStr);
                }
                else
                {
                    foreach (HtmlElement linkElm in PostBrowser.Document.Links)
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
                        this.UrlDialog.AddUrl(new OpenUrlItem(linkText, MyCommon.urlEncodeMultibyteChar(urlStr), href));
                    }
                    try
                    {
                        if (this.UrlDialog.ShowDialog() == DialogResult.OK)
                        {
                            openUrlStr = this.UrlDialog.SelectedUrl;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    this.TopMost = this.SettingDialog.AlwaysTop;
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
                if (this.SettingDialog.OpenUserTimeline && m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
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
            if (string.IsNullOrEmpty(this._rclickTabName))
            {
                return;
            }
            this.ClearTab(this._rclickTabName, true);
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

            this._statuses.ClearTabIds(tabName);
            if (ListTab.SelectedTab.Text == tabName)
            {
                this.anchorPost = null;
                this.anchorFlag = false;
                this._itemCache = null;
                this._postCache = null;
                this._itemCacheIndex = -1;
                this._curItemIndex = -1;
                this._curPost = null;
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    tb.ImageIndex = -1;
                    ((DetailsListView)tb.Tag).VirtualListSize = 0;
                    break;
                }
            }
            if (!this.SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
        }

        long _prevFollowerCount = 0;

        // メインウインドウタイトルの書き換え
        private void SetMainWindowTitle()
        {
            StringBuilder ttl = new StringBuilder(256);
            int ur = 0;
            int al = 0;
            if (this.SettingDialog.DispLatestPost != DispTitleEnum.None
                && this.SettingDialog.DispLatestPost != DispTitleEnum.Post
                && this.SettingDialog.DispLatestPost != DispTitleEnum.Ver
                && this.SettingDialog.DispLatestPost != DispTitleEnum.OwnStatus)
            {
                foreach (string key in this._statuses.Tabs.Keys)
                {
                    ur += this._statuses.Tabs[key].UnreadCount;
                    al += this._statuses.Tabs[key].AllCount;
                }
            }

            if (this.SettingDialog.DispUsername)
            {
                ttl.Append(this.tw.Username).Append(" - ");
            }
            ttl.Append("Hoehoe  ");
            switch (this.SettingDialog.DispLatestPost)
            {
                case DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.fileVersion);
                    break;
                case DispTitleEnum.Post:
                    if (this._history != null && this._history.Count > 1)
                    {
                        ttl.Append(this._history[this._history.Count - 2].Status.Replace("\r\n", string.Empty));
                    }
                    break;
                case DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText1, this._statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this._statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText3, ur, this._statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + this._statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount);
                    break;
                case DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case DispTitleEnum.OwnStatus:
                    if (this._prevFollowerCount == 0 && this.tw.FollowersCount > 0)
                    {
                        this._prevFollowerCount = this.tw.FollowersCount;
                    }
                    ttl.AppendFormat(Hoehoe.Properties.Resources.OwnStatusTitle, this.tw.StatusesCount, this.tw.FriendsCount, this.tw.FollowersCount, this.tw.FollowersCount - this._prevFollowerCount);
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
            if (this._statuses == null)
            {
                return string.Empty;
            }
            TabClass tbRep = this._statuses.GetTabByType(TabUsageType.Mentions);
            TabClass tbDm = this._statuses.GetTabByType(TabUsageType.DirectMessage);
            if (tbRep == null || tbDm == null)
            {
                return string.Empty;
            }
            int urat = tbRep.UnreadCount + tbDm.UnreadCount;
            int ur = 0;
            int al = 0;
            int tur = 0;
            int tal = 0;
            StringBuilder slbl = new StringBuilder(256);
            try
            {
                foreach (string key in this._statuses.Tabs.Keys)
                {
                    ur += this._statuses.Tabs[key].UnreadCount;
                    al += this._statuses.Tabs[key].AllCount;
                    if (key.Equals(this._curTab.Text))
                    {
                        tur = this._statuses.Tabs[key].UnreadCount;
                        tal = this._statuses.Tabs[key].AllCount;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            this._unreadCounter = ur;
            this._unreadAtCounter = urat;

            slbl.AppendFormat(Hoehoe.Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, this._postTimestamps.Count, this._favTimestamps.Count, this.timeLineCount);
            if (this.SettingDialog.TimelinePeriodInt == 0)
            {
                slbl.Append(Hoehoe.Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(this.SettingDialog.TimelinePeriodInt.ToString() + Hoehoe.Properties.Resources.SetStatusLabelText3);
            }
            return slbl.ToString();
        }

        public delegate void SetStatusLabelApiDelegate();

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

        private void SetStatusLabelApi()
        {
            this._apiGauge.RemainCount = MyCommon.TwitterApiInfo.RemainCount;
            this._apiGauge.MaxCount = MyCommon.TwitterApiInfo.MaxCount;
            this._apiGauge.ResetTime = MyCommon.TwitterApiInfo.ResetTime;
        }

        private void SetStatusLabelUrl()
        {
            StatusLabelUrl.Text = this.GetStatusLabelText();
        }

        public void SetStatusLabel(string text)
        {
            StatusLabel.Text = text;
        }

        // タスクトレイアイコンのツールチップテキスト書き換え
        // Tween [未読/@]
        private void SetNotifyIconText()
        {
            StringBuilder ur = new StringBuilder(64);
            if (this.SettingDialog.DispUsername)
            {
                ur.Append(this.tw.Username);
                ur.Append(" - ");
            }
            ur.Append("Hoehoe");
#if DEBUG
			static_SetNotifyIconText_ur.Append("(Debug Build)");
#endif
            if (this._unreadCounter != -1 && this._unreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(this._unreadCounter);
                ur.Append("/@");
                ur.Append(this._unreadAtCounter);
                ur.Append("]");
            }
            NotifyIcon1.Text = ur.ToString();
        }

        internal void CheckReplyTo(string StatusText)
        {
            // ハッシュタグの保存
            MatchCollection m = Regex.Matches(StatusText, Twitter.HASHTAG, RegexOptions.IgnoreCase);
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
            m = Regex.Matches(StatusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");

            if (this.SettingDialog.UseAtIdSupplement)
            {
                int bCnt = this.AtIdSupl.ItemCount;
                foreach (Match mid in m)
                {
                    this.AtIdSupl.AddItem(mid.Result("${id}"));
                }
                if (bCnt != this.AtIdSupl.ItemCount)
                {
                    this._modifySettingAtId = true;
                }
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (this._replyToId == 0)
            {
                return;
            }

            // リプライ先ユーザー名がない場合も指定しない
            if (string.IsNullOrEmpty(this._replyToName))
            {
                this._replyToId = 0;
                return;
            }

            // 通常Reply
            // 次の条件を満たす場合に in_reply_to_status_id 指定
            // 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
            // 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
            // 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する

            if (m != null)
            {
                if (StatusText.StartsWith("@"))
                {
                    if (StatusText.StartsWith("@" + this._replyToName))
                    {
                        return;
                    }
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (StatusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + this._replyToName)
                        {
                            return;
                        }
                    }
                }
            }

            this._replyToId = 0;
            this._replyToName = string.Empty;
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            if (!this._initialLayout && this.SettingDialog.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            if (this._initialLayout && this._cfgLocal != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                this.ClientSize = this._cfgLocal.FormSize;          // 'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                this.DesktopLocation = this._cfgLocal.FormLocation; // '位置保持（最小化・最大化されたまま終了した場合の対応用）

                if (!SplitContainer4.Panel2Collapsed && this._cfgLocal.AdSplitterDistance > this.SplitContainer4.Panel1MinSize)
                {
                    // Splitterの位置設定
                    this.SplitContainer4.SplitterDistance = this._cfgLocal.AdSplitterDistance;
                }
                if (this._cfgLocal.SplitterDistance > this.SplitContainer1.Panel1MinSize && this._cfgLocal.SplitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    // Splitterの位置設定
                    this.SplitContainer1.SplitterDistance = this._cfgLocal.SplitterDistance;
                }
                // 発言欄複数行
                StatusText.Multiline = this._cfgLocal.StatusMultiline;
                if (StatusText.Multiline)
                {
                    int dis = SplitContainer2.Height - this._cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    if (dis > SplitContainer2.Panel1MinSize && dis < SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - this._cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    }
                    StatusText.Height = this._cfgLocal.StatusTextHeight;
                }
                else
                {
                    if (SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth > 0)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
                    }
                }
                if (this._cfgLocal.PreviewDistance > this.SplitContainer3.Panel1MinSize && this._cfgLocal.PreviewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = this._cfgLocal.PreviewDistance;
                }
                this._initialLayout = false;
            }
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            PlaySoundMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.PlaySoundFileMenuItem.Checked = PlaySoundMenuItem.Checked;
            this.SettingDialog.PlaySound = PlaySoundMenuItem.Checked;
            this._modifySettingCommon = true;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._mySpDis = SplitContainer1.SplitterDistance;
                if (StatusText.Multiline)
                {
                    this._mySpDis2 = StatusText.Height;
                }
                this._modifySettingLocal = true;
            }
        }

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._myAdSpDis = SplitContainer4.SplitterDistance;
                this._modifySettingLocal = true;
            }
        }

        private void doRepliedStatusOpen()
        {
            if (this.ExistCurrentPost && this._curPost.InReplyToUser != null && this._curPost.InReplyToStatusId > 0)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    this.OpenUriAsync(string.Format("https:// twitter.com/{0}/status/{1}", this._curPost.InReplyToUser, this._curPost.InReplyToStatusId));
                    return;
                }
                if (this._statuses.ContainsKey(this._curPost.InReplyToStatusId))
                {
                    PostClass repPost = this._statuses.Item(this._curPost.InReplyToStatusId);
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.Lists | TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(this._curPost.InReplyToStatusId))
                        {
                            break;
                        }
                        PostClass repPost = this._statuses.Item(this._curPost.InReplyToStatusId);
                        MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }
                    this.OpenUriAsync("http://twitter.com/" + this._curPost.InReplyToUser + "/status/" + this._curPost.InReplyToStatusId.ToString());
                }
            }
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.doRepliedStatusOpen();
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            // 発言詳細のアイコン右クリック時のメニュー制御
            if (this._curList.SelectedIndices.Count > 0 && this._curPost != null)
            {
                string name = this._curPost.ImageUrl;
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
                    if (this._TIconDic[this._curPost.ImageUrl] != null)
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
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id == this.tw.Username)
                {
                    FollowToolStripMenuItem.Enabled = false;
                    UnFollowToolStripMenuItem.Enabled = false;
                    ShowFriendShipToolStripMenuItem.Enabled = false;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
                else
                {
                    FollowToolStripMenuItem.Enabled = true;
                    UnFollowToolStripMenuItem.Enabled = true;
                    ShowFriendShipToolStripMenuItem.Enabled = true;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
            }
            else
            {
                FollowToolStripMenuItem.Enabled = false;
                UnFollowToolStripMenuItem.Enabled = false;
                ShowFriendShipToolStripMenuItem.Enabled = false;
                ShowUserStatusToolStripMenuItem.Enabled = false;
                SearchPostsDetailNameToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem3.Enabled = false;
            }
        }

        private void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost == null)
            {
                return;
            }
            string name = this._curPost.ImageUrl;
            this.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost == null)
            {
                return;
            }
            string name = this._curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));

            this.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8);

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // STUB
            }
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost == null)
            {
                return;
            }
            string name = this._curPost.ImageUrl;

            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(this._TIconDic[name]))
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
            MultiLineMenuItem.Checked = this.StatusText.Multiline;
            this._modifySettingLocal = true;
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
            this._modifySettingLocal = true;
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            // 発言欄複数行
            StatusText.Multiline = MultiLineMenuItem.Checked;
            this._cfgLocal.StatusMultiline = MultiLineMenuItem.Checked;
            if (MultiLineMenuItem.Checked)
            {
                if (SplitContainer2.Height - this._mySpDis2 - SplitContainer2.SplitterWidth < 0)
                {
                    SplitContainer2.SplitterDistance = 0;
                }
                else
                {
                    SplitContainer2.SplitterDistance = SplitContainer2.Height - this._mySpDis2 - SplitContainer2.SplitterWidth;
                }
            }
            else
            {
                SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
            }
            this._modifySettingLocal = true;
        }

        private bool UrlConvert(UrlConverter urlCoonverterType)
        {
            // t.coで投稿時自動短縮する場合は、外部サービスでの短縮禁止
            // If this.SettingDialog.UrlConvertAuto AndAlso this.SettingDialog.ShortenTco Then Exit Function

            // Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
            // 参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
            // Appendix A.  Collected ABNF for URI
            // http://www.ietf.org/rfc/rfc3986.txt

            string result = string.Empty;

            const string nico = "^https?:// [a-z]+\\.(nicovideo|niconicommons|nicolive)\\.jp/[a-z]+/[a-z0-9]+$";

            if (StatusText.SelectionLength > 0)
            {
                string tmp = StatusText.SelectedText;
                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http"))
                {
                    // 文字列が選択されている場合はその文字列について処理

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.SettingDialog.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        StatusText.Select(StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                        StatusText.SelectedText = result;

                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = tmp, After = result });
                    }
                }
            }
            else
            {
                const string url = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?:// )" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";
                // 正規表現にマッチしたURL文字列をtinyurl化
                foreach (Match mt in Regex.Matches(StatusText.Text, url, RegexOptions.IgnoreCase))
                {
                    if (StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) == -1)
                    {
                        continue;
                    }
                    string tmp = mt.Result("${url}");
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase))
                    {
                        tmp = "http://" + tmp;
                    }
                    // 選んだURLを選択（？）
                    StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.SettingDialog.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != UrlConverter.Nicoms)
                    {
                        // 短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        result = ShortUrl.Make(urlCoonverterType, tmp);
                        if (result.Equals("Can't convert"))
                        {
                            StatusLabel.Text = result.Insert(0, urlCoonverterType.ToString() + ":");
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);
                        StatusText.SelectedText = result;
                        // undoバッファにセット
                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndoInfo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(new UrlUndoInfo() { Before = mt.Result("${url}"), After = result });
                    }
                }
            }

            return true;
        }

        private void doUrlUndo()
        {
            if (this.urlUndoBuffer != null)
            {
                string tmp = StatusText.Text;
                foreach (UrlUndoInfo data in this.urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }
                StatusText.Text = tmp;
                this.urlUndoBuffer = null;
                UrlUndoToolStripMenuItem.Enabled = false;
                StatusText.SelectionStart = 0;
                StatusText.SelectionLength = 0;
            }
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
            if (!this.UrlConvert(this.SettingDialog.AutoShortUrlFirst))
            {
                UrlConverter svc = this.SettingDialog.AutoShortUrlFirst;
                Random rnd = new Random();
                // 前回使用した短縮URLサービス以外を選択する
                do
                {
                    svc = (UrlConverter)rnd.Next(System.Enum.GetNames(typeof(UrlConverter)).Length);
                } while (!(svc != this.SettingDialog.AutoShortUrlFirst && svc != UrlConverter.Nicoms && svc != UrlConverter.Unu));
                this.UrlConvert(svc);
            }
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doUrlUndo();
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.NotifyFileMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked;
            this._cfgCommon.NewAllPop = NewPostPopMenuItem.Checked;
            this._modifySettingCommon = true;
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = ListLockMenuItem.Checked;
            this._cfgCommon.ListLock = ListLockMenuItem.Checked;
            this._modifySettingCommon = true;
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            MenuStrip1.Tag = new object();
            MenuStrip1.Select();
            // StatusText がフォーカスを持っている場合 Leave が発生
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            // 設定された戻り先へ遷移
            if (this.Tag != null)
            {
                if (object.ReferenceEquals(this.Tag, this.ListTab.SelectedTab))
                {
                    ((Control)this.ListTab.SelectedTab.Tag).Select();
                }
                else
                {
                    ((Control)this.Tag).Select();
                }
                // 戻り先が指定されていない (初期状態) 場合はタブに遷移
            }
            else
            {
                if (ListTab.SelectedIndex > -1 && ListTab.SelectedTab.HasChildren)
                {
                    this.Tag = ListTab.SelectedTab.Tag;
                    ((Control)this.Tag).Select();
                }
            }
            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            MenuStrip1.Tag = null;
        }

        private void MyList_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (this._cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (this._iconCol)
            {
                this._cfgLocal.Width1 = lst.Columns[0].Width;
                this._cfgLocal.Width3 = lst.Columns[1].Width;
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
                            this._cfgLocal.DisplayIndex1 = i;
                            break;
                        case 1:
                            this._cfgLocal.DisplayIndex2 = i;
                            break;
                        case 2:
                            this._cfgLocal.DisplayIndex3 = i;
                            break;
                        case 3:
                            this._cfgLocal.DisplayIndex4 = i;
                            break;
                        case 4:
                            this._cfgLocal.DisplayIndex5 = i;
                            break;
                        case 5:
                            this._cfgLocal.DisplayIndex6 = i;
                            break;
                        case 6:
                            this._cfgLocal.DisplayIndex7 = i;
                            break;
                        case 7:
                            this._cfgLocal.DisplayIndex8 = i;
                            break;
                    }
                }
                this._cfgLocal.Width1 = lst.Columns[0].Width;
                this._cfgLocal.Width2 = lst.Columns[1].Width;
                this._cfgLocal.Width3 = lst.Columns[2].Width;
                this._cfgLocal.Width4 = lst.Columns[3].Width;
                this._cfgLocal.Width5 = lst.Columns[4].Width;
                this._cfgLocal.Width6 = lst.Columns[5].Width;
                this._cfgLocal.Width7 = lst.Columns[6].Width;
                this._cfgLocal.Width8 = lst.Columns[7].Width;
            }
            this._modifySettingLocal = true;
            this._isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this._cfgLocal == null)
            {
                return;
            }
            DetailsListView lst = (DetailsListView)sender;
            if (this._iconCol)
            {
                if (this._cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this._cfgLocal.Width1 = lst.Columns[0].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    this._cfgLocal.Width3 = lst.Columns[1].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
            }
            else
            {
                if (this._cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    this._cfgLocal.Width1 = lst.Columns[0].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    this._cfgLocal.Width2 = lst.Columns[1].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    this._cfgLocal.Width3 = lst.Columns[2].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    this._cfgLocal.Width4 = lst.Columns[3].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    this._cfgLocal.Width5 = lst.Columns[4].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    this._cfgLocal.Width6 = lst.Columns[5].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    this._cfgLocal.Width7 = lst.Columns[6].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
                if (this._cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    this._cfgLocal.Width8 = lst.Columns[7].Width;
                    this._modifySettingLocal = true;
                    this._isColumnChanged = true;
                }
            }
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

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            try
            {
                Clipboard.SetDataObject(this.WebBrowser_GetSelectionText(ref PostBrowser), false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void doSearchToolStrip(string url)
        {
            // 発言詳細で「選択文字列で検索」（選択文字列取得）
            string selText = this.WebBrowser_GetSelectionText(ref PostBrowser);

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

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細ですべて選択
            PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            this.doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem1Url);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            this.doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem2Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            this.doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem3Url);
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            this.doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem4Url);
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + this._postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?:// [^\"]+)\"", RegexOptions.IgnoreCase);
                foreach (Match m in mc)
                {
                    if (m.Groups["url"].Value == this._postBrowserStatusText)
                    {
                        Clipboard.SetDataObject(m.Groups["title"].Value, false, 5, 100);
                        break;
                    }
                }
                if (mc.Count == 0)
                {
                    Clipboard.SetDataObject(this._postBrowserStatusText, false, 5, 100);
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
            if (PostBrowser.StatusText.StartsWith("http"))
            {
                this._postBrowserStatusText = PostBrowser.StatusText;
                string name = this.GetUserId();
                UrlCopyContextMenuItem.Enabled = true;
                if (name != null)
                {
                    FollowContextMenuItem.Enabled = true;
                    RemoveContextMenuItem.Enabled = true;
                    FriendshipContextMenuItem.Enabled = true;
                    ShowUserStatusContextMenuItem.Enabled = true;
                    SearchPostsDetailToolStripMenuItem.Enabled = true;
                    IdFilterAddMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = true;
                }
                else
                {
                    FollowContextMenuItem.Enabled = false;
                    RemoveContextMenuItem.Enabled = false;
                    FriendshipContextMenuItem.Enabled = false;
                    ShowUserStatusContextMenuItem.Enabled = false;
                    SearchPostsDetailToolStripMenuItem.Enabled = false;
                    IdFilterAddMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem.Enabled = false;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                }

                UseHashtagMenuItem.Enabled = Regex.IsMatch(this._postBrowserStatusText, "^https?:// twitter.com/search\\?q=%23");
            }
            else
            {
                this._postBrowserStatusText = string.Empty;
                UrlCopyContextMenuItem.Enabled = false;
                FollowContextMenuItem.Enabled = false;
                RemoveContextMenuItem.Enabled = false;
                FriendshipContextMenuItem.Enabled = false;
                ShowUserStatusContextMenuItem.Enabled = false;
                SearchPostsDetailToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                UseHashtagMenuItem.Enabled = false;
                IdFilterAddMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem.Enabled = false;
            }
            // 文字列選択されていないときは選択文字列関係の項目を非表示に
            string _selText = this.WebBrowser_GetSelectionText(ref PostBrowser);
            if (_selText == null)
            {
                SelectionSearchContextMenuItem.Enabled = false;
                SelectionCopyContextMenuItem.Enabled = false;
                SelectionTranslationToolStripMenuItem.Enabled = false;
            }
            else
            {
                SelectionSearchContextMenuItem.Enabled = true;
                SelectionCopyContextMenuItem.Enabled = true;
                SelectionTranslationToolStripMenuItem.Enabled = true;
            }
            // 発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            bool fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != this.tw.Username.ToLower())
                {
                    fAllFlag = true;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            this.FriendshipAllMenuItem.Enabled = fAllFlag;

            TranslationToolStripMenuItem.Enabled = this._curPost != null;

            e.Cancel = false;
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細の選択文字列で現在のタブを検索
            string _selText = this.WebBrowser_GetSelectionText(ref PostBrowser);

            if (_selText != null)
            {
                this.SearchDialog.SWord = _selText;
                this.SearchDialog.CheckCaseSensitive = false;
                this.SearchDialog.CheckRegex = false;

                this.DoTabSearch(this.SearchDialog.SWord, this.SearchDialog.CheckCaseSensitive, this.SearchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (StatusText.Multiline)
            {
                this._mySpDis2 = StatusText.Height;
            }
            this._modifySettingLocal = true;
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ImageSelectionPanel.Visible = true;
                ImageSelectionPanel.Enabled = true;
                TimelinePanel.Visible = false;
                TimelinePanel.Enabled = false;
                ImagefilePathText.Text = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                this.ImageFromSelectedFile();
                this.Activate();
                this.BringToFront();
                StatusText.Focus();
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string data = e.Data.GetData(DataFormats.StringFormat, true) as string;
                if (data != null)
                {
                    StatusText.Text += data;
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

                if (!string.IsNullOrEmpty(this.ImageService) && this._pictureServices[this.ImageService].CheckValidFilesize(ext, fl.Length))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
                foreach (string svc in ImageServiceCombo.Items)
                {
                    if (string.IsNullOrEmpty(svc))
                    {
                        continue;
                    }
                    if (this._pictureServices[svc].CheckValidFilesize(ext, fl.Length))
                    {
                        ImageServiceCombo.SelectedItem = svc;
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

        public void OpenUriAsync(string uri)
        {
            this.RunAsync(new GetWorkerArg() { WorkerType = WorkerType.OpenUri, Url = uri });
        }

        private void ListTabSelect(TabPage tab)
        {
            this.SetListProperty();

            this._itemCache = null;
            this._itemCacheIndex = -1;
            this._postCache = null;

            this._curTab = tab;
            this._curList = (DetailsListView)tab.Tag;
            if (this._curList.SelectedIndices.Count > 0)
            {
                this._curItemIndex = this._curList.SelectedIndices[0];
                this._curPost = this.GetCurTabPost(this._curItemIndex);
            }
            else
            {
                this._curItemIndex = -1;
                this._curPost = null;
            }

            this.anchorPost = null;
            this.anchorFlag = false;

            if (this._iconCol)
            {
                ((DetailsListView)tab.Tag).Columns[1].Text = this._columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= this._curList.Columns.Count - 1; i++)
                {
                    ((DetailsListView)tab.Tag).Columns[i].Text = this._columnTexts[i];
                }
            }
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.ListTabSelect(e.TabPage);
        }

        private void SelectListItem(DetailsListView dlView, int index)
        {
            // 単一
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (dlView.FocusedItem != null)
            {
                bnd = dlView.FocusedItem.Bounds;
                flg = true;
            }

            do
            {
                dlView.SelectedIndices.Clear();
            } while (dlView.SelectedIndices.Count > 0);
            dlView.Items[index].Selected = true;
            // LView.SelectedIndices.Add(Index)
            dlView.Items[index].Focused = true;

            if (flg)
            {
                dlView.Invalidate(bnd);
            }
        }

        private void SelectListItem(DetailsListView dlView, int[] indecies, int focused)
        {
            // 複数
            Rectangle bnd = default(Rectangle);
            bool flg = false;
            if (dlView.FocusedItem != null)
            {
                bnd = dlView.FocusedItem.Bounds;
                flg = true;
            }

            int fIdx = -1;
            if (indecies != null && !(indecies.Count() == 1 && indecies[0] == -1))
            {
                do
                {
                    dlView.SelectedIndices.Clear();
                } while (dlView.SelectedIndices.Count > 0);
                foreach (int idx in indecies)
                {
                    if (idx > -1 && dlView.VirtualListSize > idx)
                    {
                        dlView.SelectedIndices.Add(idx);
                        if (fIdx == -1)
                            fIdx = idx;
                    }
                }
            }
            if (focused > -1 && dlView.VirtualListSize > focused)
            {
                dlView.Items[focused].Focused = true;
            }
            else if (fIdx > -1)
            {
                dlView.Items[fIdx].Focused = true;
            }
            if (flg)
            {
                dlView.Invalidate(bnd);
            }
        }

        private void RunAsync(GetWorkerArg args)
        {
            BackgroundWorker bw = null;
            if (args.WorkerType != WorkerType.Follower)
            {
                for (int i = 0; i < this._bw.Length; i++)
                {
                    if (this._bw[i] != null && !this._bw[i].IsBusy)
                    {
                        bw = this._bw[i];
                        break;
                    }
                }
                if (bw == null)
                {
                    for (int i = 0; i < this._bw.Length; i++)
                    {
                        if (this._bw[i] == null)
                        {
                            this._bw[i] = new BackgroundWorker();
                            bw = this._bw[i];
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
            this.tw.NewPostFromStream += this.tw_NewPostFromStream;
            this.tw.UserStreamStarted += this.tw_UserStreamStarted;
            this.tw.UserStreamStopped += this.tw_UserStreamStopped;
            this.tw.PostDeleted += this.tw_PostDeleted;
            this.tw.UserStreamEventReceived += this.tw_UserStreamEventArrived;

            MenuItemUserStream.Text = "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Start";
            StopToolStripMenuItem.Enabled = true;
            if (this.SettingDialog.UserstreamStartup)
            {
                this.tw.StartUserStream();
            }
        }

        private void TweenMain_Shown(object sender, EventArgs e)
        {
            try
            {
                PostBrowser.Url = new Uri("about:blank");
                PostBrowser.DocumentText = string.Empty;
                // 発言詳細部初期化
            }
            catch (Exception)
            {
            }

            NotifyIcon1.Visible = true;
            this.tw.UserIdChanged += this.tw_UserIdChanged;

            if (MyCommon.IsNetworkAvailable())
            {
                this.GetTimeline(WorkerType.BlockIds, 0, 0, string.Empty);
                if (this.SettingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, string.Empty);
                }
                this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                this.StartUserStream();
                this._waitTimeline = true;
                this.GetTimeline(WorkerType.Timeline, 1, 1, string.Empty);
                this._waitReply = true;
                this.GetTimeline(WorkerType.Reply, 1, 1, string.Empty);
                this._waitDm = true;
                this.GetTimeline(WorkerType.DirectMessegeRcv, 1, 1, string.Empty);
                if (this.SettingDialog.GetFav)
                {
                    this._waitFav = true;
                    this.GetTimeline(WorkerType.Favorites, 1, 1, string.Empty);
                }
                this._waitPubSearch = true;
                this.GetTimeline(WorkerType.PublicSearch, 1, 0, string.Empty);
                // tabname="":全タブ
                this._waitUserTimeline = true;
                this.GetTimeline(WorkerType.UserTimeline, 1, 0, string.Empty);
                // tabname="":全タブ
                this._waitLists = true;
                this.GetTimeline(WorkerType.List, 1, 0, string.Empty);
                // tabname="":全タブ
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
                        break;
                    }
                    // 120秒間初期処理が終了しなかったら強制的に打ち切る
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
                if (this.SettingDialog.StartupVersion)
                {
                    this.CheckNewVersion(true);
                }

                // 取得失敗の場合は再試行する
                if (!this.tw.GetFollowersSuccess && this.SettingDialog.StartupFollowers)
                {
                    this.GetTimeline(WorkerType.Follower, 0, 0, string.Empty);
                }

                // 取得失敗の場合は再試行する
                if (this.SettingDialog.TwitterConfiguration.PhotoSizeLimit == 0)
                {
                    this.GetTimeline(WorkerType.Configuration, 0, 0, string.Empty);
                }

                // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
                if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ReAuthorizeText);
                    this.SettingStripMenuItem_Click(null, null);
                }

                //
            }
            this._initial = false;

            this.TimerTimeline.Enabled = true;
        }

        private bool IsInitialRead()
        {
            return this._waitTimeline || this._waitReply || this._waitDm || this._waitFav || this._waitPubSearch || this._waitUserTimeline || this._waitLists;
        }

        private void doGetFollowersMenu()
        {
            this.GetTimeline(WorkerType.Follower, 1, 0, string.Empty);
            this.DispSelectedPost(true);
        }

        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doGetFollowersMenu();
        }

        private void doReTweetUnofficial()
        {
            // RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (this._curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (this._curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = this.CreateRetweetUnofficial(this._curPost.Text);
                StatusText.Text = "RT @" + this._curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doReTweetUnofficial();
        }

        private void doReTweetOfficial(bool isConfirm)
        {
            // 公式RT
            if (this.ExistCurrentPost)
            {
                if (this._curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    this._DoFavRetweetFlags = false;
                    return;
                }
                if (this._curList.SelectedIndices.Count > 15)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.RetweetLimitText);
                    this._DoFavRetweetFlags = false;
                    return;
                }
                else if (this._curList.SelectedIndices.Count > 1)
                {
                    string QuestionText = Hoehoe.Properties.Resources.RetweetQuestion2;
                    if (this._DoFavRetweetFlags)
                    {
                        QuestionText = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText1;
                    }
                    switch (MessageBox.Show(QuestionText, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            this._DoFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (this._curPost.IsDm || this._curPost.IsMe)
                    {
                        this._DoFavRetweetFlags = false;
                        return;
                    }
                    if (!this.SettingDialog.RetweetNoConfirm)
                    {
                        string Questiontext = Hoehoe.Properties.Resources.RetweetQuestion1;
                        if (this._DoFavRetweetFlags)
                        {
                            Questiontext = Hoehoe.Properties.Resources.FavoritesRetweetQuestionText2;
                        }
                        if (isConfirm && MessageBox.Show(Questiontext, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            this._DoFavRetweetFlags = false;
                            return;
                        }
                    }
                }
                GetWorkerArg args = new GetWorkerArg()
                {
                    Ids = new List<long>(),
                    SIds = new List<long>(),
                    TabName = this._curTab.Text,
                    WorkerType = WorkerType.Retweet
                };
                foreach (int idx in this._curList.SelectedIndices)
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

        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doReTweetOfficial(true);
        }

        private void FavoritesRetweetOriginal()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }
            this._DoFavRetweetFlags = true;
            this.doReTweetOfficial(true);
            if (this._DoFavRetweetFlags)
            {
                this._DoFavRetweetFlags = false;
                this.FavoriteChange(true, false);
            }
        }

        private void FavoritesRetweetUnofficial()
        {
            if (this.ExistCurrentPost && !this._curPost.IsDm)
            {
                this._DoFavRetweetFlags = true;
                this.FavoriteChange(true);
                if (!this._curPost.IsProtect && this._DoFavRetweetFlags)
                {
                    this._DoFavRetweetFlags = false;
                    this.doReTweetUnofficial();
                }
            }
        }

        private string CreateRetweetUnofficial(string status)
        {
            // Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
            // 展開しないように変更
            // 展開するか判定
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
            if (StatusText.Multiline)
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            this._replyToId = 0;
            this._replyToName = string.Empty;
            status = status.Replace("&nbsp;", " ");

            return status;
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost != null)
            {
                this.DispSelectedPost(true);
            }
        }

        private bool isKeyDown(Keys key)
        {
            return (Control.ModifierKeys & key) == key;
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            if (MyCommon.DebugBuild || this.isKeyDown(Keys.CapsLock) && this.isKeyDown(Keys.Control) && this.isKeyDown(Keys.Shift))
            {
                DebugModeToolStripMenuItem.Visible = true;
            }
            else
            {
                DebugModeToolStripMenuItem.Visible = false;
            }
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            this.SettingDialog.UrlConvertAuto = ToolStripMenuItemUrlAutoShorten.Checked;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            ToolStripMenuItemUrlAutoShorten.Checked = this.SettingDialog.UrlConvertAuto;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.TraceFlag = TraceOutToolStripMenuItem.Checked;
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_Leave(StatusText, EventArgs.Empty);
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._rclickTabName))
            {
                return;
            }
            this.TabRename(ref this._rclickTabName);
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Bitly);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UrlConvert(UrlConverter.Jmp);
        }

        private class GetApiInfoArgs
        {
            public Twitter Tw;
            public ApiInfo Info;
        }

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = this.tw.GetInfoApi(args.Info);
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
            if (this._curPost != null)
            {
                id = this._curPost.ScreenName;
            }
            this.FollowCommand(id);
        }

        private void FollowCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostFollowCommand(arg.Id);
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

        private void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this._curPost != null)
            {
                id = this._curPost.ScreenName;
            }
            this.RemoveCommand(id, false);
        }

        private class FollowRemoveCommandArgs
        {
            public Twitter Tw;
            public string Id;
        }

        private void RemoveCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            FollowRemoveCommandArgs arg = (FollowRemoveCommandArgs)e.Argument;
            e.Result = arg.Tw.PostRemoveCommand(arg.Id);
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

        private void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            string id = string.Empty;
            if (this._curPost != null)
            {
                id = this._curPost.ScreenName;
            }
            this.ShowFriendship(id);
        }

        private class ShowFriendshipArgs
        {
            public Twitter Tw;

            public class FriendshipInfo
            {
                public string id = string.Empty;
                public bool isFollowing = false;
                public bool isFollowed = false;
                public bool isError = false;

                public FriendshipInfo(string id)
                {
                    this.id = id;
                }
            }

            public List<FriendshipInfo> ids = new List<FriendshipInfo>();
        }

        private void ShowFriendship_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowFriendshipArgs arg = (ShowFriendshipArgs)e.Argument;
            string result = string.Empty;
            foreach (ShowFriendshipArgs.FriendshipInfo fInfo in arg.ids)
            {
                string rt = arg.Tw.GetFriendshipInfo(fInfo.id, ref fInfo.isFollowing, ref fInfo.isFollowed);
                if (!string.IsNullOrEmpty(rt))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = rt;
                    }
                    fInfo.isError = true;
                }
            }
            e.Result = result;
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
                    args.ids.Add(new ShowFriendshipArgs.FriendshipInfo(inputName.TabName.Trim()));
                    using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                    {
                        _info.ShowDialog();
                        ret = (string)_info.Result;
                    }
                    string result = string.Empty;
                    if (string.IsNullOrEmpty(ret))
                    {
                        if (args.ids[0].isFollowing)
                        {
                            result = Hoehoe.Properties.Resources.GetFriendshipInfo1 + System.Environment.NewLine;
                        }
                        else
                        {
                            result = Hoehoe.Properties.Resources.GetFriendshipInfo2 + System.Environment.NewLine;
                        }

                        if (args.ids[0].isFollowed)
                        {
                            result += Hoehoe.Properties.Resources.GetFriendshipInfo3;
                        }
                        else
                        {
                            result += Hoehoe.Properties.Resources.GetFriendshipInfo4;
                        }
                        result = args.ids[0].id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + result;
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
                args.ids.Add(new ShowFriendshipArgs.FriendshipInfo(id.Trim()));
                using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, this.ShowFriendship_DoWork, null, args))
                {
                    _info.ShowDialog();
                    ret = (string)_info.Result;
                }
                string result = string.Empty;
                ShowFriendshipArgs.FriendshipInfo fInfo = args.ids[0];
                string ff = string.Empty;
                if (string.IsNullOrEmpty(ret))
                {
                    ff = "  ";
                    if (fInfo.isFollowing)
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo1;
                    }
                    else
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo2;
                    }
                    ff += System.Environment.NewLine + "  ";
                    if (fInfo.isFollowed)
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo3;
                    }
                    else
                    {
                        ff += Hoehoe.Properties.Resources.GetFriendshipInfo4;
                    }
                    result += fInfo.id + Hoehoe.Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + ff;
                    if (fInfo.isFollowing)
                    {
                        if (MessageBox.Show(Hoehoe.Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result, Hoehoe.Properties.Resources.GetFriendshipInfo8, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.RemoveCommand(fInfo.id, true);
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

        private void OwnStatusMenuItem_Click(object sender, EventArgs e)
        {
            this.doShowUserStatus(this.tw.Username, false);
        }

        // TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        // URLから切り出した文字列を渡す

        public bool IsTwitterId(string name)
        {
            if (this.SettingDialog.TwitterConfiguration.NonUsernamePaths == null || this.SettingDialog.TwitterConfiguration.NonUsernamePaths.Length == 0)
            {
                return !Regex.Match(name, "^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            }
            else
            {
                return !this.SettingDialog.TwitterConfiguration.NonUsernamePaths.Contains(name.ToLower());
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this._postBrowserStatusText, "^https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && this.IsTwitterId(m.Result("${ScreenName}")))
            {
                return m.Result("${ScreenName}");
            }
            else
            {
                return null;
            }
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
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?:// twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
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
            this._modifySettingCommon = true;
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this._modifySettingCommon = true;
        }

        private void doQuote()
        {
            // QT @id:内容
            // 返信先情報付加
            if (this.ExistCurrentPost)
            {
                if (this._curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (this._curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = this.CreateRetweetUnofficial(this._curPost.Text);

                StatusText.Text = " QT @" + this._curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                if (this._curPost.RetweetedId == 0)
                {
                    this._replyToId = this._curPost.StatusId;
                }
                else
                {
                    this._replyToId = this._curPost.RetweetedId;
                }
                this._replyToName = this._curPost.ScreenName;

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doQuote();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }
            string tbName = pnl.Parent.Text;
            TabClass tb = this._statuses.Tabs[tbName];
            ComboBox cmb = (ComboBox)pnl.Controls["comboSearch"];
            ComboBox cmbLang = (ComboBox)pnl.Controls["comboLang"];
            ComboBox cmbusline = (ComboBox)pnl.Controls["comboUserline"];
            cmb.Text = cmb.Text.Trim();
            // 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
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
                ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
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
                this._statuses.ClearTabIds(tbName);
                this.SaveConfigsTabs();
                // 検索条件の保存
            }

            this.GetTimeline(WorkerType.PublicSearch, 1, 0, tbName);
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            // もっと前を取得
            this.DoRefreshMore();
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            if (this._statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TabClass tb = this._statuses.RemovedTab.Pop();
                string renamed = tb.TabName;
                for (int i = 1; i <= int.MaxValue; i++)
                {
                    if (!this._statuses.ContainsTab(renamed))
                    {
                        break;
                    }
                    renamed = tb.TabName + "(" + i.ToString() + ")";
                }
                tb.TabName = renamed;
                this._statuses.Tabs.Add(renamed, tb);
                this.AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
                ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                this.SaveConfigsTabs();
            }
        }

        private void doMoveToRTHome()
        {
            if (this._curList.SelectedIndices.Count > 0)
            {
                PostClass post = this.GetCurTabPost(this._curList.SelectedIndices[0]);
                if (post.RetweetedId > 0)
                {
                    this.OpenUriAsync("http://twitter.com/" + this.GetCurTabPost(this._curList.SelectedIndices[0]).RetweetedBy);
                }
            }
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.doMoveToRTHome();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            string name = this.GetUserId();
            if (name != null)
            {
                string tabName = string.Empty;

                // 未選択なら処理終了
                if (this._curList.SelectedIndices.Count == 0)
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
                this._statuses.Tabs[tabName].AddFilter(fc);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    this._itemCache = null;
                    this._postCache = null;
                    this._curPost = null;
                    this._curItemIndex = -1;
                    this._statuses.FilterAll();
                    foreach (TabPage tb in ListTab.TabPages)
                    {
                        ((DetailsListView)tb.Tag).VirtualListSize = this._statuses.Tabs[tb.Text].AllCount;
                        if (this._statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (this.SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (this.SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                    if (!this.SettingDialog.TabIconDisp)
                    {
                        ListTab.Refresh();
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
            else if (this._curPost != null)
            {
                user = this._curPost.ScreenName;
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
            if (ListTab.SelectedTab != null)
            {
                if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType != TabUsageType.PublicSearch)
                {
                    return;
                }
                ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(this._postBrowserStatusText, "^https?:// twitter.com/search\\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                this.HashMgr.SetPermanentHash("#" + m.Result("${hash}"));
                HashStripSplitButton.Text = this.HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
                // 使用ハッシュタグとして設定
                this._modifySettingCommon = true;
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
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
            this.TopMost = this.SettingDialog.AlwaysTop;
            if (rslt == DialogResult.Cancel)
                return;
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                HashStripSplitButton.Text = this.HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            this._modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.HashMgr.ToggleHash();
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                HashStripSplitButton.Text = this.HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            this._modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            this.HashToggleMenuItem_Click(null, null);
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            if (ListTab.SelectedTab == null)
            {
                return;
            }
            if (this._statuses == null || this._statuses.Tabs == null || !this._statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
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

            if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.DirectMessage || !this.ExistCurrentPost || this._curPost.IsDm)
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
                if (this.ExistCurrentPost && this._curPost.IsDm)
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
                // PublicSearchの時問題出るかも

                if (this._curPost.IsMe)
                {
                    this.RtOpMenuItem.Enabled = false;
                    this.FavoriteRetweetMenuItem.Enabled = false;
                    this.DelOpMenuItem.Enabled = true;
                }
                else
                {
                    this.DelOpMenuItem.Enabled = false;
                    if (this._curPost.IsProtect)
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

            if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType != TabUsageType.Favorites)
            {
                this.RefreshPrevOpMenuItem.Enabled = true;
            }
            else
            {
                this.RefreshPrevOpMenuItem.Enabled = false;
            }

            if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch || !this.ExistCurrentPost || !(this._curPost.InReplyToStatusId > 0))
            {
                OpenRepSourceOpMenuItem.Enabled = false;
            }
            else
            {
                OpenRepSourceOpMenuItem.Enabled = true;
            }

            if (!this.ExistCurrentPost || string.IsNullOrEmpty(this._curPost.RetweetedBy))
            {
                OpenRterHomeMenuItem.Enabled = false;
            }
            else
            {
                OpenRterHomeMenuItem.Enabled = true;
            }
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            this.ContextMenuTabProperty_Opening(sender, null);
        }

        public Twitter TwitterInstance
        {
            get { return this.tw; }
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._mySpDis3 = SplitContainer3.SplitterDistance;
                this._modifySettingLocal = true;
            }
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            if (this._statuses.RemovedTab.Count == 0)
            {
                UndoRemoveTabMenuItem.Enabled = false;
            }
            else
            {
                UndoRemoveTabMenuItem.Enabled = true;
            }
            if (ListTab.SelectedTab != null)
            {
                if (this._statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    PublicSearchQueryMenuItem.Enabled = true;
                }
                else
                {
                    PublicSearchQueryMenuItem.Enabled = false;
                }
            }
            else
            {
                PublicSearchQueryMenuItem.Enabled = false;
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
                if (this._curPost.IsDm)
                {
                    this.CopyURLMenuItem.Enabled = false;
                }
                if (this._curPost.IsProtect)
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
            if (this._curPost != null)
            {
                id = this._curPost.ScreenName;
            }
            this.ShowUserStatus(id);
        }

        private class GetUserInfoArgs
        {
            public Hoehoe.Twitter tw;
            public string id;
            public DataModels.Twitter.User user;
        }

        private void GetUserInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            GetUserInfoArgs args = (GetUserInfoArgs)e.Argument;
            e.Result = args.tw.GetUserInfo(args.id, ref args.user);
        }

        private void doShowUserStatus(string id, bool ShowInputDialog)
        {
            DataModels.Twitter.User user = null;
            if (ShowInputDialog)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Show UserStatus");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(inputName.TabName.Trim()))
                    {
                        id = inputName.TabName.Trim();
                        GetUserInfoArgs args = new GetUserInfoArgs() { tw = this.tw, id = id, user = user };
                        using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
                        {
                            info.ShowDialog();
                            string ret = (string)info.Result;
                            if (string.IsNullOrEmpty(ret))
                            {
                                this.doShowUserStatus(args.user);
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
                GetUserInfoArgs args = new GetUserInfoArgs() { tw = this.tw, id = id, user = user };
                using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, this.GetUserInfo_DoWork, null, args))
                {
                    info.ShowDialog();
                    string ret = (string)info.Result;
                    if (string.IsNullOrEmpty(ret))
                    {
                        this.doShowUserStatus(args.user);
                    }
                    else
                    {
                        MessageBox.Show(ret);
                    }
                }
            }
        }

        private void doShowUserStatus(DataModels.Twitter.User user)
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

        private void ShowUserStatus(string id, bool ShowInputDialog)
        {
            this.doShowUserStatus(id, ShowInputDialog);
        }

        private void ShowUserStatus(string id)
        {
            this.doShowUserStatus(id, true);
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.FollowCommand(id);
                }
            }
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.RemoveCommand(id, false);
                }
            }
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.tw.Username)
                {
                    this.ShowFriendship(id);
                }
            }
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                this.ShowUserStatus(id, false);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                this.AddNewTabForUserTimeline(id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                this.AddNewTabForSearch("@" + id);
            }
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost != null)
            {
                this.ShowUserStatus(this._curPost.ScreenName, false);
            }
        }

        private void GetRetweet_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;

            long statusid = 0;
            if (this._curPost.RetweetedId > 0)
            {
                statusid = this._curPost.RetweetedId;
            }
            else
            {
                statusid = this._curPost.StatusId;
            }
            this.tw.GetStatus_Retweeted_Count(statusid, ref counter);

            e.Result = counter;
        }

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost)
            {
                using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.RtCountMenuItem_ClickText1, this.GetRetweet_DoWork))
                {
                    int retweet_count = 0;

                    // ダイアログ表示
                    _info.ShowDialog();
                    retweet_count = Convert.ToInt32(_info.Result);
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

        private HookGlobalHotkey withEventsField__hookGlobalHotkey;

        private HookGlobalHotkey _hookGlobalHotkey
        {
            get { return this.withEventsField__hookGlobalHotkey; }
            set
            {
                if (this.withEventsField__hookGlobalHotkey != null)
                {
                    this.withEventsField__hookGlobalHotkey.HotkeyPressed -= this._hookGlobalHotkey_HotkeyPressed;
                }
                this.withEventsField__hookGlobalHotkey = value;
                if (this.withEventsField__hookGlobalHotkey != null)
                {
                    this.withEventsField__hookGlobalHotkey.HotkeyPressed += this._hookGlobalHotkey_HotkeyPressed;
                }
            }
        }

        public TweenMain()
        {
            this._hookGlobalHotkey = new HookGlobalHotkey(this);
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            this._apiGauge.Control.Size = new Size(70, 22);
            this._apiGauge.Control.Margin = new Padding(0, 3, 0, 2);
            this._apiGauge.GaugeHeight = 8;
            this._apiGauge.Control.DoubleClick += this.ApiInfoMenuItem_Click;
            this.StatusStrip1.Items.Insert(2, this._apiGauge);
        }

        private void _hookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
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
            if (NameLabel.Tag != null)
            {
                this.OpenUriAsync("http://twitter.com/" + NameLabel.Tag.ToString());
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MultiLineMenuItem.PerformClick();
        }

        public PostClass CurPost
        {
            get { return this._curPost; }
        }

        public bool IsPreviewEnable
        {
            get { return this.SettingDialog.PreviewEnable; }
        }

        #region "画像投稿"

        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageSelectionPanel.Visible == true)
            {
                ImagefilePathText.CausesValidation = false;
                TimelinePanel.Visible = true;
                TimelinePanel.Enabled = true;
                ImageSelectionPanel.Visible = false;
                ImageSelectionPanel.Enabled = false;
                ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
                ImagefilePathText.CausesValidation = true;
            }
            else
            {
                ImageSelectionPanel.Visible = true;
                ImageSelectionPanel.Enabled = true;
                TimelinePanel.Visible = false;
                TimelinePanel.Enabled = false;
                ImagefilePathText.Focus();
            }
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ImageService))
            {
                return;
            }
            OpenFileDialog1.Filter = this._pictureServices[this.ImageService].GetFileOpenDialogFilter();
            OpenFileDialog1.Title = Hoehoe.Properties.Resources.PickPictureDialog1;
            OpenFileDialog1.FileName = string.Empty;

            try
            {
                this.AllowDrop = false;
                if (OpenFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
            }
            finally
            {
                this.AllowDrop = true;
            }

            ImagefilePathText.Text = OpenFileDialog1.FileName;
            this.ImageFromSelectedFile();
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (ImageCancelButton.Focused)
            {
                ImagefilePathText.CausesValidation = false;
                return;
            }
            ImagefilePathText.Text = ImagefilePathText.Text.Trim();
            if (string.IsNullOrEmpty(ImagefilePathText.Text))
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = UploadFileType.Invalid;
            }
            else
            {
                this.ImageFromSelectedFile();
            }
        }

        private void ImageFromSelectedFile()
        {
            try
            {
                if (string.IsNullOrEmpty(ImagefilePathText.Text.Trim()) || string.IsNullOrEmpty(this.ImageService))
                {
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    ImagefilePathText.Text = string.Empty;
                    return;
                }

                FileInfo fl = new FileInfo(ImagefilePathText.Text.Trim());
                if (!this._pictureServices[this.ImageService].CheckValidExtension(fl.Extension))
                {
                    // 画像以外の形式
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    ImagefilePathText.Text = string.Empty;
                    return;
                }

                if (!this._pictureServices[this.ImageService].CheckValidFilesize(fl.Extension, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    ImagefilePathText.Text = string.Empty;
                    MessageBox.Show("File is too large.");
                    return;
                }

                switch (this._pictureServices[this.ImageService].GetFileType(fl.Extension))
                {
                    case UploadFileType.Invalid:
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        ImagefilePathText.Text = string.Empty;
                        break;
                    case UploadFileType.Picture:
                        Image img = null;
                        using (FileStream fs = new FileStream(ImagefilePathText.Text, FileMode.Open, FileAccess.Read))
                        {
                            img = Image.FromStream(fs);
                            fs.Close();
                        }

                        ImageSelectedPicture.Image = (new HttpVarious()).CheckValidImage(img, img.Width, img.Height);
                        ImageSelectedPicture.Tag = UploadFileType.Picture;
                        break;
                    case UploadFileType.MultiMedia:
                        ImageSelectedPicture.Image = Hoehoe.Properties.Resources.MultiMediaImage;
                        ImageSelectedPicture.Tag = UploadFileType.MultiMedia;
                        break;
                    default:
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = UploadFileType.Invalid;
                        ImagefilePathText.Text = string.Empty;
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = UploadFileType.Invalid;
                ImagefilePathText.Text = string.Empty;
                MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = UploadFileType.Invalid;
                ImagefilePathText.Text = string.Empty;
                MessageBox.Show("The type of this file is not image.");
            }
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = UploadFileType.Invalid;
                TimelinePanel.Visible = true;
                TimelinePanel.Enabled = true;
                ImageSelectionPanel.Visible = false;
                ImageSelectionPanel.Enabled = false;
                ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
                ImagefilePathText.CausesValidation = true;
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 0x1b)
            {
                ImagefilePathText.CausesValidation = false;
                e.Handled = true;
            }
        }

        private void ImageSelection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ImagefilePathText.CausesValidation = false;
            }
        }

        private void SetImageServiceCombo()
        {
            string svc = string.Empty;
            if (ImageServiceCombo.SelectedIndex > -1)
                svc = ImageServiceCombo.SelectedItem.ToString();
            ImageServiceCombo.Items.Clear();
            ImageServiceCombo.Items.Add("TwitPic");
            ImageServiceCombo.Items.Add("img.ly");
            ImageServiceCombo.Items.Add("yfrog");
            ImageServiceCombo.Items.Add("lockerz");
            ImageServiceCombo.Items.Add("Twitter");

            if (string.IsNullOrEmpty(svc))
            {
                ImageServiceCombo.SelectedIndex = 0;
            }
            else
            {
                int idx = ImageServiceCombo.Items.IndexOf(svc);
                if (idx == -1)
                {
                    ImageServiceCombo.SelectedIndex = 0;
                }
                else
                {
                    ImageServiceCombo.SelectedIndex = idx;
                }
            }
        }

        private string ImageService
        {
            get { return Convert.ToString(ImageServiceCombo.SelectedItem); }
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
        {
            ImagefilePathText.CausesValidation = false;
            TimelinePanel.Visible = true;
            TimelinePanel.Enabled = true;
            ImageSelectionPanel.Visible = false;
            ImageSelectionPanel.Enabled = false;
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
            ImagefilePathText.CausesValidation = true;
        }

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ImageSelectedPicture.Tag != null && !string.IsNullOrEmpty(this.ImageService))
            {
                try
                {
                    FileInfo fi = new FileInfo(ImagefilePathText.Text.Trim());
                    if (!this._pictureServices[this.ImageService].CheckValidFilesize(fi.Extension, fi.Length))
                    {
                        ImagefilePathText.Text = string.Empty;
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = UploadFileType.Invalid;
                    }
                }
                catch (Exception)
                {
                }
                this._modifySettingCommon = true;
                this.SaveConfigsAll(false);
                if (this.ImageService == "Twitter")
                {
                    this.StatusText_TextChanged(null, null);
                }
            }
        }

        #endregion "画像投稿"

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ListManage form = new ListManage(this.tw))
            {
                form.ShowDialog(this);
            }
        }

        public void SetModifySettingCommon(bool value)
        {
            this._modifySettingCommon = value;
        }

        public void SetModifySettingLocal(bool value)
        {
            this._modifySettingLocal = value;
        }

        public void SetModifySettingAtId(bool value)
        {
            this._modifySettingAtId = value;
        }

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = Convert.ToString(SourceLinkLabel.Tag);
            if (!string.IsNullOrEmpty(link) && e.Button == MouseButtons.Left)
            {
                this.OpenUriAsync(link);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            string link = Convert.ToString(SourceLinkLabel.Tag);
            if (!string.IsNullOrEmpty(link))
            {
                StatusLabelUrl.Text = link;
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            this.SetStatusLabelUrl();
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost && !this._curPost.IsDm)
            {
                RtCountMenuItem.Enabled = true;
            }
            else
            {
                RtCountMenuItem.Enabled = false;
            }
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyUserId();
        }

        private void CopyUserId()
        {
            if (this._curPost == null)
            {
                return;
            }
            string clstr = this._curPost.ScreenName;
            try
            {
                Clipboard.SetDataObject(clstr, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            TabClass backToTab = this._curTab == null ? this._statuses.Tabs[ListTab.SelectedTab.Text] : this._statuses.Tabs[this._curTab.Text];
            if (this.ExistCurrentPost && !this._curPost.IsDm)
            {
                // PublicSearchも除外した方がよい？
                if (this._statuses.GetTabByType(TabUsageType.Related) == null)
                {
                    const string TabName = "Related Tweets";
                    string tName = TabName;
                    if (!this.AddNewTab(tName, false, TabUsageType.Related))
                    {
                        for (int i = 2; i <= 100; i++)
                        {
                            tName = TabName + i.ToString();
                            if (this.AddNewTab(tName, false, TabUsageType.Related))
                            {
                                this._statuses.AddTab(tName, TabUsageType.Related, null);
                                break;
                            }
                        }
                    }
                    else
                    {
                        this._statuses.AddTab(tName, TabUsageType.Related, null);
                    }
                    this._statuses.GetTabByName(tName).UnreadManage = false;
                    this._statuses.GetTabByName(tName).Notify = false;
                }

                TabClass tb = this._statuses.GetTabByType(TabUsageType.Related);
                tb.RelationTargetPost = this._curPost;
                this.ClearTab(tb.TabName, false);
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (tb.TabName == ListTab.TabPages[i].Text)
                    {
                        ListTab.SelectedIndex = i;
                        this.ListTabSelect(ListTab.TabPages[i]);
                        break;
                    }
                }

                this.GetTimeline(WorkerType.Related, 1, 1, tb.TabName);
            }
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("キャッシュメモリ容量         : {0}bytes({1}MB)" + "\r\n", ((ImageDictionary)this._TIconDic).CacheMemoryLimit, ((ImageDictionary)this._TIconDic).CacheMemoryLimit / 1048576);
            buf.AppendFormat("物理メモリ使用割合           : {0}%" + "\r\n", ((ImageDictionary)this._TIconDic).PhysicalMemoryLimit);
            buf.AppendFormat("キャッシュエントリ保持数     : {0}" + "\r\n", ((ImageDictionary)this._TIconDic).CacheCount);
            buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + "\r\n", ((ImageDictionary)this._TIconDic).CacheRemoveCount);
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void tw_UserIdChanged()
        {
            this._modifySettingCommon = true;
        }

        #region "Userstream"

        private bool _isActiveUserstream = false;

        private void tw_PostDeleted(long id)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action(() =>
                    {
                        this._statuses.RemovePostReserve(id);
                        if (this._curTab != null && this._statuses.Tabs[this._curTab.Text].Contains(id))
                        {
                            this._itemCache = null;
                            this._itemCacheIndex = -1;
                            this._postCache = null;
                            ((DetailsListView)_curTab.Tag).Update();
                            if (this._curPost != null & this._curPost.StatusId == id)
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

        private void tw_NewPostFromStream()
        {
            if (this.SettingDialog.ReadOldPosts)
            {
                this._statuses.SetRead();
                // 新着時未読クリア
            }

            int rsltAddCount = this._statuses.DistributePosts();
            lock (this._syncObject)
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

            if (this.SettingDialog.UserstreamPeriodInt > 0)
                return;

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

        private void tw_UserStreamStarted()
        {
            this._isActiveUserstream = true;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.tw_UserStreamStarted));
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

            MenuItemUserStream.Text = "&UserStream ▶";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Stop";
            StopToolStripMenuItem.Enabled = true;

            StatusLabel.Text = "UserStream Started.";
        }

        private void tw_UserStreamStopped()
        {
            this._isActiveUserstream = false;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(this.tw_UserStreamStopped));
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

            MenuItemUserStream.Text = "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Start";
            StopToolStripMenuItem.Enabled = true;

            StatusLabel.Text = "UserStream Stopped.";
        }

        private void tw_UserStreamEventArrived(Twitter.FormattedEvent ev)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<Twitter.FormattedEvent>(this.tw_UserStreamEventArrived), ev);
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
            StatusLabel.Text = "Event: " + ev.Event;
            this.NotifyEvent(ev);
            if (ev.Event == "favorite" || ev.Event == "unfavorite")
            {
                if (this._curTab != null && this._statuses.Tabs[this._curTab.Text].Contains(ev.Id))
                {
                    this._itemCache = null;
                    this._itemCacheIndex = -1;
                    this._postCache = null;
                    ((DetailsListView)this._curTab.Tag).Update();
                }
                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(this.tw.Username.ToLower()))
                {
                    this.RemovePostFromFavTab(new long[] { ev.Id });
                }
            }
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            // 新着通知
            if (this.BalloonRequired(ev))
            {
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                StringBuilder title = new StringBuilder();
                if (this.SettingDialog.DispUsername)
                {
                    title.Append(this.tw.Username);
                    title.Append(" - ");
                }
                else
                {
                    // title.Clear()
                }
                title.Append("Hoehoe [");
                title.Append(ev.Event.ToUpper());
                title.Append("] by ");
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(ev.Username.ToString());
                }
                else
                {
                    // title.Append("")
                }
                string text = null;
                if (!string.IsNullOrEmpty(ev.Target))
                {
                    // NotifyIcon1.BalloonTipText = ev.Target
                    text = ev.Target;
                }
                else
                {
                    // NotifyIcon1.BalloonTipText = " "
                    text = " ";
                }
                // NotifyIcon1.ShowBalloonTip(500)
                if (this.SettingDialog.IsNotifyUseGrowl)
                {
                    GrowlHelper.Notify(GrowlHelper.NotifyType.UserStreamEvent, ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                    NotifyIcon1.BalloonTipTitle = title.ToString();
                    NotifyIcon1.BalloonTipText = text;
                    NotifyIcon1.ShowBalloonTip(500);
                }
            }

            // サウンド再生
            string snd = this.SettingDialog.EventSoundFile;
            if (!this._initial && this.SettingDialog.PlaySound && !string.IsNullOrEmpty(snd))
            {
                if (Convert.ToBoolean(ev.Eventtype & this.SettingDialog.EventNotifyFlag) && this.IsMyEventNotityAsEventType(ev))
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

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuItemUserStream.Enabled = false;
            if (StopRefreshAllMenuItem.Checked)
            {
                StopRefreshAllMenuItem.Checked = false;
                return;
            }
            if (this._isActiveUserstream)
            {
                this.tw.StopUserStream();
            }
            else
            {
                this.tw.StartUserStream();
            }
        }

        string _prevTrackWord;

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackToolStripMenuItem.Checked)
            {
                using (InputTabName inputForm = new InputTabName())
                {
                    inputForm.TabName = this._prevTrackWord;
                    inputForm.SetFormTitle("Input track word");
                    inputForm.SetFormDescription("Track word");
                    if (inputForm.ShowDialog() != DialogResult.OK)
                    {
                        TrackToolStripMenuItem.Checked = false;
                        return;
                    }
                    this._prevTrackWord = inputForm.TabName.Trim();
                }
                if (this._prevTrackWord != this.tw.TrackWord)
                {
                    this.tw.TrackWord = this._prevTrackWord;
                    this._modifySettingCommon = true;
                    TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this._prevTrackWord);
                    this.tw.ReconnectUserStream();
                }
            }
            else
            {
                this.tw.TrackWord = string.Empty;
                this.tw.ReconnectUserStream();
            }
            this._modifySettingCommon = true;
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tw.AllAtReply = AllrepliesToolStripMenuItem.Checked;
            this._modifySettingCommon = true;
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
                pos.X = Convert.ToInt32(this.Location.X + this.Size.Width / 2 - this.evtDialog.Size.Width / 2);
                pos.Y = Convert.ToInt32(this.Location.Y + this.Size.Height / 2 - this.evtDialog.Size.Height / 2);
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
            this.TopMost = this.SettingDialog.AlwaysTop;
        }

        #endregion "Userstream"

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

        private void doTranslation(string str)
        {
            Bing bing = new Bing();
            string buf = string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            string srclng = string.Empty;
            string dstlng = this.SettingDialog.TranslateLanguage;
            string msg = string.Empty;
            if (srclng != dstlng && bing.Translate(string.Empty, dstlng, str, ref buf))
            {
                PostBrowser.DocumentText = this.CreateDetailHtml(buf);
            }
            else
            {
                if (msg.StartsWith("Err:"))
                {
                    StatusLabel.Text = msg;
                }
            }
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }
            this.doTranslation(this._curPost.TextFromApi);
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.doTranslation(this.WebBrowser_GetSelectionText(ref PostBrowser));
        }

        private bool ExistCurrentPost
        {
            get { return this._curPost != null && !this._curPost.IsDeleted; }
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowUserTimeline();
        }

        public bool FavEventChangeUnread
        {
            get { return this.SettingDialog.FavEventUnread; }
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            string id = string.Empty;
            if (this._curPost != null)
            {
                id = this._curPost.ScreenName;
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
            this.TimerTimeline.Enabled = isEnable;
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.TimelineRefreshEnableChange(!StopRefreshAllMenuItem.Checked);
        }

        private void OpenUserAppointUrl()
        {
            if (this.SettingDialog.UserAppointUrl != null)
            {
                if (this.SettingDialog.UserAppointUrl.Contains("{ID}") || this.SettingDialog.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (this._curPost != null)
                    {
                        string xUrl = this.SettingDialog.UserAppointUrl;
                        xUrl = xUrl.Replace("{ID}", this._curPost.ScreenName);
                        if (this._curPost.RetweetedId != 0)
                        {
                            xUrl = xUrl.Replace("{STATUS}", this._curPost.RetweetedId.ToString());
                        }
                        else
                        {
                            xUrl = xUrl.Replace("{STATUS}", this._curPost.StatusId.ToString());
                        }
                        this.OpenUriAsync(xUrl);
                    }
                }
                else
                {
                    this.OpenUriAsync(this.SettingDialog.UserAppointUrl);
                }
            }
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
            if (SplitContainer4.Panel2Collapsed)
            {
                return;
            }
            if (SplitContainer4.Height < SplitContainer4.SplitterWidth + SplitContainer4.Panel2MinSize + SplitContainer4.SplitterDistance && SplitContainer4.Height - SplitContainer4.SplitterWidth - SplitContainer4.Panel2MinSize > 0)
            {
                SplitContainer4.SplitterDistance = SplitContainer4.Height - SplitContainer4.SplitterWidth - SplitContainer4.Panel2MinSize;
            }
            if (SplitContainer4.Panel2.Height > 90 && SplitContainer4.Height - SplitContainer4.SplitterWidth - 90 > 0)
            {
                SplitContainer4.SplitterDistance = SplitContainer4.Height - SplitContainer4.SplitterWidth - 90;
            }
        }

        private void Ga_Sent()
        {
            this._modifySettingCommon = true;
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            string selText = SourceLinkLabel.Text;
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
            string selText = Convert.ToString(SourceLinkLabel.Tag);
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
            if (this._curPost == null || !this.ExistCurrentPost || this._curPost.IsDm)
            {
                SourceCopyMenuItem.Enabled = false;
                SourceUrlCopyMenuItem.Enabled = false;
            }
            else
            {
                SourceCopyMenuItem.Enabled = true;
                SourceUrlCopyMenuItem.Enabled = true;
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
    }
}