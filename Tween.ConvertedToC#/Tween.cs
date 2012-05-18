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

//コンパイル後コマンド
//"c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"
//"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Hoehoe.TweenCustomControl;
using System.Media;

namespace Hoehoe
{
    public partial class TweenMain
    {
        //各種設定
        //画面サイズ
        private Size _mySize;

        //画面位置
        private Point _myLoc;

        //区切り位置
        private int _mySpDis;

        //発言欄区切り位置
        private int _mySpDis2;

        //プレビュー区切り位置
        private int _mySpDis3;

        //Ad区切り位置
        private int _myAdSpDis;

        //アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private int _iconSz;

        //1列表示の時True（48サイズのとき）
        private bool _iconCol;

        //雑多なフラグ類
        //True:起動時処理中
        private bool _initial;

        private bool _initialLayout = true;

        //True:起動時処理中
        private bool _ignoreConfigSave;

        //タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private bool _tabDrag;

        //タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private TabPage _beforeSelectedTab;

        private Point _tabMouseDownPoint;

        //右クリックしたタブの名前（Tabコントロール機能不足対応）
        private string _rclickTabName;

        //ロック用
        private readonly object _syncObject = new object();

        private const string detailHtmlFormatMono1 = "<html><head><style type=\"text/css\"><!-- pre {font-family: \"";
        private const string detailHtmlFormat2 = "\", sans-serif; font-size: ";
        private const string detailHtmlFormat3 = "pt; word-wrap: break-word; color:rgb(";
        private const string detailHtmlFormat4 = ");} a:link, a:visited, a:active, a:hover {color:rgb(";
        private const string detailHtmlFormat5 = "); } --></style></head><body style=\"margin:0px; background-color:rgb(";
        private const string detailHtmlFormatMono6 = ");\"><pre>";
        private const string detailHtmlFormatMono7 = "</pre></body></html>";
        private const string detailHtmlFormat1 = "<html><head><style type=\"text/css\"><!-- p {font-family: \"";
        private const string detailHtmlFormat6 = ");\"><p>";
        private const string detailHtmlFormat7 = "</p></body></html>";
        private string _detailHtmlFormatHeader;
        private string _detailHtmlFormatFooter;
        private bool _myStatusError = false;
        private bool _myStatusOnline = false;
        private bool _soundfileListup = false;

        private SpaceKeyCanceler _spaceKeyCanceler;

        //設定ファイル関連

        private SettingLocal _cfgLocal;
        private SettingCommon _cfgCommon;
        private bool _modifySettingLocal = false;
        private bool _modifySettingCommon = false;

        private bool _modifySettingAtId = false;

        //twitter解析部
        private Twitter tw = new Twitter();

        //Growl呼び出し部
        private GrowlHelper withEventsField_gh = new GrowlHelper("Tween");

        private GrowlHelper gh
        {
            get { return withEventsField_gh; }
            set
            {
                if (withEventsField_gh != null)
                {
                    withEventsField_gh.NotifyClicked -= GrowlHelper_Callback;
                }
                withEventsField_gh = value;
                if (withEventsField_gh != null)
                {
                    withEventsField_gh.NotifyClicked += GrowlHelper_Callback;
                }
            }
        }

        //サブ画面インスタンス
        private AppendSettingDialog withEventsField_SettingDialog = AppendSettingDialog.Instance;

        private AppendSettingDialog SettingDialog
        {
            get { return withEventsField_SettingDialog; }
            set
            {
                if (withEventsField_SettingDialog != null)
                {
                    withEventsField_SettingDialog.IntervalChanged -= TimerInterval_Changed;
                }
                withEventsField_SettingDialog = value;
                if (withEventsField_SettingDialog != null)
                {
                    withEventsField_SettingDialog.IntervalChanged += TimerInterval_Changed;
                }
            }
            //設定画面インスタンス
        }

        //タブ選択ダイアログインスタンス
        private TabsDialog TabDialog = new TabsDialog();

        //検索画面インスタンス
        private SearchWord SearchDialog = new SearchWord();

        //フィルター編集画面
        private FilterDialog fltDialog = new FilterDialog();

        private OpenURL UrlDialog = new OpenURL();

        // シールドアイコン付きダイアログ
        private DialogAsShieldIcon dialogAsShieldicon;

        //@id補助
        public AtIdSupplement AtIdSupl;

        //Hashtag補助
        public AtIdSupplement HashSupl;

        public HashtagManage HashMgr;

        private TweenAboutBox _AboutBox;
        private EventViewerDialog evtDialog;

        //表示フォント、色、アイコン
        //未読用フォント
        private Font _fntUnread;

        //未読用文字色
        private Color _clUnread;

        //既読用フォント
        private Font _fntReaded;

        //既読用文字色
        private Color _clReaded;

        //Fav用文字色
        private Color _clFav;

        //片思い用文字色
        private Color _clOWL;

        //Retweet用文字色
        private Color _clRetweet;

        //発言詳細部用フォント
        private Font _fntDetail;

        //発言詳細部用色
        private Color _clDetail;

        //発言詳細部用リンク文字色
        private Color _clDetailLink;

        //発言詳細部用背景色
        private Color _clDetailBackcolor;

        //自分の発言用背景色
        private Color _clSelf;

        //自分宛返信用背景色
        private Color _clAtSelf;

        //選択発言者の他の発言用背景色
        private Color _clTarget;

        //選択発言中の返信先用背景色
        private Color _clAtTarget;

        //選択発言者への返信発言用背景色
        private Color _clAtFromTarget;

        //選択発言の唯一＠先
        private Color _clAtTo;

        //リスト部通常発言背景色
        private Color _clListBackcolor;

        //入力欄背景色
        private Color _clInputBackcolor;

        //入力欄文字色
        private Color _clInputFont;

        //入力欄フォント
        private Font _fntInputFont;

        //アイコン画像リスト
        private IDictionary<string, Image> _TIconDic;

        //At.ico             タスクトレイアイコン：通常時
        private Icon _NIconAt;

        //AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon _NIconAtRed;

        //AtSmoke.ico        タスクトレイアイコン：オフライン時
        private Icon _NIconAtSmoke;

        //Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon[] _NIconRefresh = new Icon[4];

        //Tab.ico            未読のあるタブ用アイコン
        private Icon _TabIcon;

        //Main.ico           画面左上のアイコン
        private Icon _MainIcon;

        //5g
        private Icon _ReplyIcon;

        //6g
        private Icon _ReplyIconBlink;

        private PostClass _anchorPost;

        //True:関連発言移動中（関連移動以外のオペレーションをするとFalseへ。Trueだとリスト背景色をアンカー発言選択中として描画）
        private bool _anchorFlag;

        //発言履歴
        private List<PostingStatus> _history = new List<PostingStatus>();

        //発言履歴カレントインデックス
        private int _hisIdx;

        //発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
        // リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
        private long _replyToId;

        // リプライ先ステータスの書き込み者の名前
        private string _replyToName;

        //時速表示用
        private List<DateTime> _postTimestamps = new List<DateTime>();

        private List<DateTime> _favTimestamps = new List<DateTime>();
        private Dictionary<DateTime, int> _tlTimestamps = new Dictionary<DateTime, int>();

        private int _tlCount;

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

        //Listにフォーカスないときの選択行の背景色
        private SolidBrush _brsDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace));

        private StringFormat sfTab = new StringFormat();

        ///''''''''''''''''''''''''''''''''''''''''''''''''''
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
        private BackgroundWorker _bwFollower;
        private int _cMode;
        private ShieldIcon _shield = new ShieldIcon();
        private InternetSecurityManager _securityManager;
        private Thumbnail _thumbnail;
        private int _unreadCounter = -1;
        private int _unreadAtCounter = -1;
        private string[] _columnOrgTexts = new string[9];
        private string[] _columnTexts = new string[9];
        private bool _DoFavRetweetFlags = false;
        private bool _osResumed = false;
        private Dictionary<string, IMultimediaShareService> _pictureServices;
        private string _postBrowserStatusText = "";
        private bool _colorize = false;
        private System.Timers.Timer withEventsField_TimerTimeline = new System.Timers.Timer();

        private System.Timers.Timer TimerTimeline
        {
            get { return withEventsField_TimerTimeline; }
            set
            {
                if (withEventsField_TimerTimeline != null)
                {
                    withEventsField_TimerTimeline.Elapsed -= TimerTimeline_Elapsed;
                }
                withEventsField_TimerTimeline = value;
                if (withEventsField_TimerTimeline != null)
                {
                    withEventsField_TimerTimeline.Elapsed += TimerTimeline_Elapsed;
                }
            }
        }

        private ImageListViewItem displayItem;

        //URL短縮のUndo用
        private struct urlUndo
        {
            public string Before;
            public string After;
        }

        private List<urlUndo> urlUndoBuffer = null;

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

        //[, ]でのリプライ移動の履歴
        private Stack<ReplyChain> _replyChains;

        //ポスト選択履歴
        private Stack<Tuple<TabPage, PostClass>> _selectPostChains = new Stack<Tuple<TabPage, PostClass>>();

        //Backgroundworkerの処理結果通知用引数構造体
        private class GetWorkerResult
        {
            //処理結果詳細メッセージ。エラー時に値がセットされる
            public string RetMsg = "";

            //取得対象ページ番号
            public int Page;

            //取得終了ページ番号（継続可能ならインクリメントされて返る。pageと比較して継続判定）
            public int EndPage;

            //処理種別
            public Hoehoe.MyCommon.WorkerType WorkerType;

            //新規取得したアイコンイメージ
            public Dictionary<string, Image> Imgs;

            //Fav追加・削除時のタブ名
            public string TabName = "";

            //Fav追加・削除時のID
            public List<long> Ids;

            //Fav追加・削除成功分のID
            public List<long> SIds;

            public bool NewDM;
            public int AddCount;
            public PostingStatus PStatus;
        }

        //Backgroundworkerへ処理内容を通知するための引数用構造体
        private class GetWorkerArg
        {
            //処理対象ページ番号
            public int Page;

            //処理終了ページ番号（起動時の読み込みページ数。通常時はpageと同じ値をセット）
            public int EndPage;

            //処理種別
            public Hoehoe.MyCommon.WorkerType WorkerType;

            //URLをブラウザで開くときのアドレス
            public string Url = "";

            //発言POST時の発言内容
            public PostingStatus PStatus = new PostingStatus();

            //Fav追加・削除時のItemIndex
            public List<long> Ids;

            //Fav追加・削除成功分のItemIndex
            public List<long> SIds;

            //Fav追加・削除時のタブ名
            public string TabName = "";
        }

        //検索処理タイプ
        private enum SEARCHTYPE
        {
            DialogSearch,
            NextSearch,
            PrevSearch
        }

        private class PostingStatus
        {
            public string Status = "";
            public long InReplyToId = 0;
            public string InReplyToName = "";

            //画像投稿サービス名
            public string ImageService = "";

            public string ImagePath = "";

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
            int WM_KEYDOWN = 0x100;
            int VK_SPACE = 0x20;

            public SpaceKeyCanceler(Control control)
            {
                this.AssignHandle(control.Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if ((m.Msg == WM_KEYDOWN) && (Convert.ToInt32(m.WParam) == VK_SPACE))
                {
                    if (SpaceCancel != null)
                    {
                        SpaceCancel(this, EventArgs.Empty);
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
            //画面がアクティブになったら、発言欄の背景色戻す
            if (StatusText.Focused)
            {
                this.StatusText_Enter(this.StatusText, EventArgs.Empty);
            }
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            //後始末
            SettingDialog.Dispose();
            TabDialog.Dispose();
            SearchDialog.Dispose();
            fltDialog.Dispose();
            UrlDialog.Dispose();
            _spaceKeyCanceler.Dispose();
            if (_NIconAt != null)
            {
                _NIconAt.Dispose();
            }
            if (_NIconAtRed != null)
            {
                _NIconAtRed.Dispose();
            }
            if (_NIconAtSmoke != null)
            {
                _NIconAtSmoke.Dispose();
            }
            if (_NIconRefresh[0] != null)
            {
                _NIconRefresh[0].Dispose();
            }
            if (_NIconRefresh[1] != null)
            {
                _NIconRefresh[1].Dispose();
            }
            if (_NIconRefresh[2] != null)
            {
                _NIconRefresh[2].Dispose();
            }
            if (_NIconRefresh[3] != null)
            {
                _NIconRefresh[3].Dispose();
            }
            if (_TabIcon != null)
            {
                _TabIcon.Dispose();
            }
            if (_MainIcon != null)
            {
                _MainIcon.Dispose();
            }
            if (_ReplyIcon != null)
            {
                _ReplyIcon.Dispose();
            }
            if (_ReplyIconBlink != null)
            {
                _ReplyIconBlink.Dispose();
            }
            _brsHighLight.Dispose();
            _brsHighLightText.Dispose();
            if (_brsForeColorUnread != null)
            {
                _brsForeColorUnread.Dispose();
            }
            if (_brsForeColorReaded != null)
            {
                _brsForeColorReaded.Dispose();
            }
            if (_brsForeColorFav != null)
            {
                _brsForeColorFav.Dispose();
            }
            if (_brsForeColorOWL != null)
            {
                _brsForeColorOWL.Dispose();
            }
            if (_brsForeColorRetweet != null)
            {
                _brsForeColorRetweet.Dispose();
            }
            if (_brsBackColorMine != null)
            {
                _brsBackColorMine.Dispose();
            }
            if (_brsBackColorAt != null)
            {
                _brsBackColorAt.Dispose();
            }
            if (_brsBackColorYou != null)
            {
                _brsBackColorYou.Dispose();
            }
            if (_brsBackColorAtYou != null)
            {
                _brsBackColorAtYou.Dispose();
            }
            if (_brsBackColorAtFromTarget != null)
            {
                _brsBackColorAtFromTarget.Dispose();
            }
            if (_brsBackColorAtTo != null)
            {
                _brsBackColorAtTo.Dispose();
            }
            if (_brsBackColorNone != null)
            {
                _brsBackColorNone.Dispose();
            }
            if (_brsDeactiveSelection != null)
            {
                _brsDeactiveSelection.Dispose();
            }
            _shield.Dispose();
            sfTab.Dispose();
            foreach (BackgroundWorker bw in _bw)
            {
                if (bw != null)
                {
                    bw.Dispose();
                }
            }
            if (_bwFollower != null)
            {
                _bwFollower.Dispose();
            }
            this._apiGauge.Dispose();
            if (_TIconDic != null)
            {
                ((ImageDictionary)this._TIconDic).PauseGetImage = true;
                ((IDisposable)_TIconDic).Dispose();
            }
            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
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
            //着せ替えアイコン対応
            //タスクトレイ通常時アイコン
            string dir = Application.StartupPath;

            _NIconAt = Hoehoe.Properties.Resources.At;
            _NIconAtRed = Hoehoe.Properties.Resources.AtRed;
            _NIconAtSmoke = Hoehoe.Properties.Resources.AtSmoke;
            _NIconRefresh[0] = Hoehoe.Properties.Resources.Refresh;
            _NIconRefresh[1] = Hoehoe.Properties.Resources.Refresh2;
            _NIconRefresh[2] = Hoehoe.Properties.Resources.Refresh3;
            _NIconRefresh[3] = Hoehoe.Properties.Resources.Refresh4;
            _TabIcon = Hoehoe.Properties.Resources.TabIcon;
            _MainIcon = Hoehoe.Properties.Resources.MIcon;
            _ReplyIcon = Hoehoe.Properties.Resources.Reply;
            _ReplyIconBlink = Hoehoe.Properties.Resources.ReplyBlink;

            if (!Directory.Exists(Path.Combine(dir, "Icons")))
            {
                return;
            }

            LoadIcon(ref _NIconAt, "Icons\\At.ico");

            //タスクトレイエラー時アイコン
            LoadIcon(ref _NIconAtRed, "Icons\\AtRed.ico");

            //タスクトレイオフライン時アイコン
            LoadIcon(ref _NIconAtSmoke, "Icons\\AtSmoke.ico");

            //タスクトレイ更新中アイコン
            //アニメーション対応により4種類読み込み
            LoadIcon(ref _NIconRefresh[0], "Icons\\Refresh.ico");
            LoadIcon(ref _NIconRefresh[1], "Icons\\Refresh2.ico");
            LoadIcon(ref _NIconRefresh[2], "Icons\\Refresh3.ico");
            LoadIcon(ref _NIconRefresh[3], "Icons\\Refresh4.ico");

            //タブ見出し未読表示アイコン
            LoadIcon(ref _TabIcon, "Icons\\Tab.ico");

            //画面のアイコン
            LoadIcon(ref _MainIcon, "Icons\\MIcon.ico");

            //Replyのアイコン
            LoadIcon(ref _ReplyIcon, "Icons\\Reply.ico");

            //Reply点滅のアイコン
            LoadIcon(ref _ReplyIconBlink, "Icons\\ReplyBlink.ico");
        }

        private void InitColumnText()
        {
            _columnTexts[0] = "";
            _columnTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            _columnTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            _columnTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            _columnTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            _columnTexts[5] = "";
            _columnTexts[6] = "";
            _columnTexts[7] = "Source";

            _columnOrgTexts[0] = "";
            _columnOrgTexts[1] = Hoehoe.Properties.Resources.AddNewTabText2;
            _columnOrgTexts[2] = Hoehoe.Properties.Resources.AddNewTabText3;
            _columnOrgTexts[3] = Hoehoe.Properties.Resources.AddNewTabText4_2;
            _columnOrgTexts[4] = Hoehoe.Properties.Resources.AddNewTabText5;
            _columnOrgTexts[5] = "";
            _columnOrgTexts[6] = "";
            _columnOrgTexts[7] = "Source";

            int c = 0;
            switch (_statuses.SortMode)
            {
                case IdComparerClass.ComparerMode.Nickname:
                    //ニックネーム
                    c = 1;
                    break;
                case IdComparerClass.ComparerMode.Data:
                    //本文
                    c = 2;
                    break;
                case IdComparerClass.ComparerMode.Id:
                    //時刻=発言Id
                    c = 3;
                    break;
                case IdComparerClass.ComparerMode.Name:
                    //名前
                    c = 4;
                    break;
                case IdComparerClass.ComparerMode.Source:
                    //Source
                    c = 7;
                    break;
            }

            if (_iconCol)
            {
                if (_statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    _columnTexts[2] = _columnOrgTexts[2] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    _columnTexts[2] = _columnOrgTexts[2] + "▴";
                }
            }
            else
            {
                if (_statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    _columnTexts[c] = _columnOrgTexts[c] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    _columnTexts[c] = _columnOrgTexts[c] + "▴";
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
            _ignoreConfigSave = true;
            this.Visible = false;

            _securityManager = new InternetSecurityManager(PostBrowser);
            _thumbnail = new Thumbnail(this);

            MyCommon.TwitterApiInfo.Changed += SetStatusLabelApiHandler;
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            VerUpMenuItem.Image = _shield.Icon;
            var cmdArgs = System.Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (!(cmdArgs.Length == 0) && cmdArgs.Contains("/d"))
            {
                MyCommon.TraceFlag = true;
            }

            this._spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this._spaceKeyCanceler.SpaceCancel += spaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            MyCommon.fileVersion = ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version;
            InitializeTraceFrag();
            LoadIcons();
            // アイコン読み込み

            //発言保持クラス
            _statuses = TabInformations.GetInstance();

            //アイコン設定
            this.Icon = _MainIcon;
            //メインフォーム（TweenMain）
            NotifyIcon1.Icon = _NIconAt;
            //タスクトレイ
            TabImage.Images.Add(_TabIcon);
            //タブ見出し

            SettingDialog.Owner = this;
            SearchDialog.Owner = this;
            fltDialog.Owner = this;
            TabDialog.Owner = this;
            UrlDialog.Owner = this;

            _history.Add(new PostingStatus());
            _hisIdx = 0;
            _replyToId = 0;
            _replyToName = "";

            //<<<<<<<<<設定関連>>>>>>>>>
            //'設定読み出し
            LoadConfig();

            //新着バルーン通知のチェック状態設定
            NewPostPopMenuItem.Checked = _cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = NewPostPopMenuItem.Checked;

            //フォント＆文字色＆背景色保持
            _fntUnread = _cfgLocal.FontUnread;
            _clUnread = _cfgLocal.ColorUnread;
            _fntReaded = _cfgLocal.FontRead;
            _clReaded = _cfgLocal.ColorRead;
            _clFav = _cfgLocal.ColorFav;
            _clOWL = _cfgLocal.ColorOWL;
            _clRetweet = _cfgLocal.ColorRetweet;
            _fntDetail = _cfgLocal.FontDetail;
            _clDetail = _cfgLocal.ColorDetail;
            _clDetailLink = _cfgLocal.ColorDetailLink;
            _clDetailBackcolor = _cfgLocal.ColorDetailBackcolor;
            _clSelf = _cfgLocal.ColorSelf;
            _clAtSelf = _cfgLocal.ColorAtSelf;
            _clTarget = _cfgLocal.ColorTarget;
            _clAtTarget = _cfgLocal.ColorAtTarget;
            _clAtFromTarget = _cfgLocal.ColorAtFromTarget;
            _clAtTo = _cfgLocal.ColorAtTo;
            _clListBackcolor = _cfgLocal.ColorListBackcolor;
            _clInputBackcolor = _cfgLocal.ColorInputBackcolor;
            _clInputFont = _cfgLocal.ColorInputFont;
            _fntInputFont = _cfgLocal.FontInputFont;

            _brsForeColorUnread = new SolidBrush(_clUnread);
            _brsForeColorReaded = new SolidBrush(_clReaded);
            _brsForeColorFav = new SolidBrush(_clFav);
            _brsForeColorOWL = new SolidBrush(_clOWL);
            _brsForeColorRetweet = new SolidBrush(_clRetweet);
            _brsBackColorMine = new SolidBrush(_clSelf);
            _brsBackColorAt = new SolidBrush(_clAtSelf);
            _brsBackColorYou = new SolidBrush(_clTarget);
            _brsBackColorAtYou = new SolidBrush(_clAtTarget);
            _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
            _brsBackColorAtTo = new SolidBrush(_clAtTo);
            //_brsBackColorNone = New SolidBrush(Color.FromKnownColor(KnownColor.Window))
            _brsBackColorNone = new SolidBrush(_clListBackcolor);

            // StringFormatオブジェクトへの事前設定
            sfTab.Alignment = StringAlignment.Center;
            sfTab.LineAlignment = StringAlignment.Center;

            //設定画面への反映
            HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);
            SettingDialog.TwitterApiUrl = _cfgCommon.TwitterUrl;
            SettingDialog.TwitterSearchApiUrl = _cfgCommon.TwitterSearchUrl;

            //認証関連
            if (String.IsNullOrEmpty(_cfgCommon.Token))
            {
                _cfgCommon.UserName = "";
            }
            tw.Initialize(_cfgCommon.Token, _cfgCommon.TokenSecret, _cfgCommon.UserName, _cfgCommon.UserId);

            SettingDialog.UserAccounts = _cfgCommon.UserAccounts;

            SettingDialog.TimelinePeriodInt = _cfgCommon.TimelinePeriod;
            SettingDialog.ReplyPeriodInt = _cfgCommon.ReplyPeriod;
            SettingDialog.DMPeriodInt = _cfgCommon.DMPeriod;
            SettingDialog.PubSearchPeriodInt = _cfgCommon.PubSearchPeriod;
            SettingDialog.UserTimelinePeriodInt = _cfgCommon.UserTimelinePeriod;
            SettingDialog.ListsPeriodInt = _cfgCommon.ListsPeriod;
            //不正値チェック
            if (!cmdArgs.Contains("nolimit"))
            {
                if (SettingDialog.TimelinePeriodInt < 15 && SettingDialog.TimelinePeriodInt > 0)
                {
                    SettingDialog.TimelinePeriodInt = 15;
                }
                if (SettingDialog.ReplyPeriodInt < 15 && SettingDialog.ReplyPeriodInt > 0)
                {
                    SettingDialog.ReplyPeriodInt = 15;
                }
                if (SettingDialog.DMPeriodInt < 15 && SettingDialog.DMPeriodInt > 0)
                {
                    SettingDialog.DMPeriodInt = 15;
                }
                if (SettingDialog.PubSearchPeriodInt < 30 && SettingDialog.PubSearchPeriodInt > 0)
                {
                    SettingDialog.PubSearchPeriodInt = 30;
                }
                if (SettingDialog.UserTimelinePeriodInt < 15 && SettingDialog.UserTimelinePeriodInt > 0)
                {
                    SettingDialog.UserTimelinePeriodInt = 15;
                }
                if (SettingDialog.ListsPeriodInt < 15 && SettingDialog.ListsPeriodInt > 0)
                {
                    SettingDialog.ListsPeriodInt = 15;
                }
            }

            //起動時読み込み分を既読にするか。Trueなら既読として処理
            SettingDialog.Readed = _cfgCommon.Read;
            //新着取得時のリストスクロールをするか。Trueならスクロールしない
            ListLockMenuItem.Checked = _cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = _cfgCommon.ListLock;
            SettingDialog.IconSz = _cfgCommon.IconSize;
            //文末ステータス
            SettingDialog.Status = _cfgLocal.StatusText;
            //未読管理。Trueなら未読管理する
            SettingDialog.UnreadManage = _cfgCommon.UnreadManage;
            //サウンド再生（タブ別設定より優先）
            SettingDialog.PlaySound = _cfgCommon.PlaySound;
            PlaySoundMenuItem.Checked = SettingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound;
            //片思い表示。Trueなら片思い表示する
            SettingDialog.OneWayLove = _cfgCommon.OneWayLove;
            //フォント＆文字色＆背景色
            SettingDialog.FontUnread = _fntUnread;
            SettingDialog.ColorUnread = _clUnread;
            SettingDialog.FontReaded = _fntReaded;
            SettingDialog.ColorReaded = _clReaded;
            SettingDialog.ColorFav = _clFav;
            SettingDialog.ColorOWL = _clOWL;
            SettingDialog.ColorRetweet = _clRetweet;
            SettingDialog.FontDetail = _fntDetail;
            SettingDialog.ColorDetail = _clDetail;
            SettingDialog.ColorDetailLink = _clDetailLink;
            SettingDialog.ColorDetailBackcolor = _clDetailBackcolor;
            SettingDialog.ColorSelf = _clSelf;
            SettingDialog.ColorAtSelf = _clAtSelf;
            SettingDialog.ColorTarget = _clTarget;
            SettingDialog.ColorAtTarget = _clAtTarget;
            SettingDialog.ColorAtFromTarget = _clAtFromTarget;
            SettingDialog.ColorAtTo = _clAtTo;
            SettingDialog.ColorListBackcolor = _clListBackcolor;
            SettingDialog.ColorInputBackcolor = _clInputBackcolor;
            SettingDialog.ColorInputFont = _clInputFont;
            SettingDialog.FontInputFont = _fntInputFont;
            SettingDialog.NameBalloon = _cfgCommon.NameBalloon;
            SettingDialog.PostCtrlEnter = _cfgCommon.PostCtrlEnter;
            SettingDialog.PostShiftEnter = _cfgCommon.PostShiftEnter;
            SettingDialog.CountApi = _cfgCommon.CountApi;
            SettingDialog.CountApiReply = _cfgCommon.CountApiReply;
            if (SettingDialog.CountApi < 20 || SettingDialog.CountApi > 200)
            {
                SettingDialog.CountApi = 60;
            }
            if (SettingDialog.CountApiReply < 20 || SettingDialog.CountApiReply > 200)
            {
                SettingDialog.CountApiReply = 40;
            }

            SettingDialog.BrowserPath = _cfgLocal.BrowserPath;
            SettingDialog.PostAndGet = _cfgCommon.PostAndGet;
            SettingDialog.UseRecommendStatus = _cfgLocal.UseRecommendStatus;
            SettingDialog.DispUsername = _cfgCommon.DispUsername;
            SettingDialog.CloseToExit = _cfgCommon.CloseToExit;
            SettingDialog.MinimizeToTray = _cfgCommon.MinimizeToTray;
            SettingDialog.DispLatestPost = _cfgCommon.DispLatestPost;
            SettingDialog.SortOrderLock = _cfgCommon.SortOrderLock;
            SettingDialog.TinyUrlResolve = _cfgCommon.TinyUrlResolve;
            SettingDialog.ShortUrlForceResolve = _cfgCommon.ShortUrlForceResolve;
            SettingDialog.SelectedProxyType = _cfgLocal.ProxyType;
            SettingDialog.ProxyAddress = _cfgLocal.ProxyAddress;
            SettingDialog.ProxyPort = _cfgLocal.ProxyPort;
            SettingDialog.ProxyUser = _cfgLocal.ProxyUser;
            SettingDialog.ProxyPassword = _cfgLocal.ProxyPassword;
            SettingDialog.PeriodAdjust = _cfgCommon.PeriodAdjust;
            SettingDialog.StartupVersion = _cfgCommon.StartupVersion;
            SettingDialog.StartupFollowers = _cfgCommon.StartupFollowers;
            SettingDialog.RestrictFavCheck = _cfgCommon.RestrictFavCheck;
            SettingDialog.AlwaysTop = _cfgCommon.AlwaysTop;
            SettingDialog.UrlConvertAuto = false;
            SettingDialog.OutputzEnabled = _cfgCommon.Outputz;
            SettingDialog.OutputzKey = _cfgCommon.OutputzKey;
            SettingDialog.OutputzUrlmode = _cfgCommon.OutputzUrlMode;
            SettingDialog.UseUnreadStyle = _cfgCommon.UseUnreadStyle;
            SettingDialog.DefaultTimeOut = _cfgCommon.DefaultTimeOut;
            SettingDialog.RetweetNoConfirm = _cfgCommon.RetweetNoConfirm;
            SettingDialog.PlaySound = _cfgCommon.PlaySound;
            SettingDialog.DateTimeFormat = _cfgCommon.DateTimeFormat;
            SettingDialog.LimitBalloon = _cfgCommon.LimitBalloon;
            SettingDialog.EventNotifyEnabled = _cfgCommon.EventNotifyEnabled;
            SettingDialog.EventNotifyFlag = _cfgCommon.EventNotifyFlag;
            SettingDialog.IsMyEventNotifyFlag = _cfgCommon.IsMyEventNotifyFlag;
            SettingDialog.ForceEventNotify = _cfgCommon.ForceEventNotify;
            SettingDialog.FavEventUnread = _cfgCommon.FavEventUnread;
            SettingDialog.TranslateLanguage = _cfgCommon.TranslateLanguage;
            SettingDialog.EventSoundFile = _cfgCommon.EventSoundFile;

            //廃止サービスが選択されていた場合bit.lyへ読み替え
            if (_cfgCommon.AutoShortUrlFirst < 0)
            {
                _cfgCommon.AutoShortUrlFirst = Hoehoe.MyCommon.UrlConverter.Bitly;
            }

            SettingDialog.AutoShortUrlFirst = _cfgCommon.AutoShortUrlFirst;
            SettingDialog.TabIconDisp = _cfgCommon.TabIconDisp;
            SettingDialog.ReplyIconState = _cfgCommon.ReplyIconState;
            SettingDialog.ReadOwnPost = _cfgCommon.ReadOwnPost;
            SettingDialog.GetFav = _cfgCommon.GetFav;
            SettingDialog.ReadOldPosts = _cfgCommon.ReadOldPosts;
            SettingDialog.UseSsl = _cfgCommon.UseSsl;
            SettingDialog.BitlyUser = _cfgCommon.BilyUser;
            SettingDialog.BitlyPwd = _cfgCommon.BitlyPwd;
            SettingDialog.ShowGrid = _cfgCommon.ShowGrid;
            SettingDialog.Language = _cfgCommon.Language;
            SettingDialog.UseAtIdSupplement = _cfgCommon.UseAtIdSupplement;
            SettingDialog.UseHashSupplement = _cfgCommon.UseHashSupplement;
            SettingDialog.PreviewEnable = _cfgCommon.PreviewEnable;
            AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            SettingDialog.IsMonospace = _cfgCommon.IsMonospace;
            if (SettingDialog.IsMonospace)
            {
                _detailHtmlFormatHeader = detailHtmlFormatMono1;
                _detailHtmlFormatFooter = detailHtmlFormatMono7;
            }
            else
            {
                _detailHtmlFormatHeader = detailHtmlFormat1;
                _detailHtmlFormatFooter = detailHtmlFormat7;
            }
            _detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString() + "," + _clDetail.G.ToString() + "," + _clDetail.B.ToString() + detailHtmlFormat4 + _clDetailLink.R.ToString() + "," + _clDetailLink.G.ToString() + "," + _clDetailLink.B.ToString() + detailHtmlFormat5 + _clDetailBackcolor.R.ToString() + "," + _clDetailBackcolor.G.ToString() + "," + _clDetailBackcolor.B.ToString();
            if (SettingDialog.IsMonospace)
            {
                _detailHtmlFormatHeader += detailHtmlFormatMono6;
            }
            else
            {
                _detailHtmlFormatHeader += detailHtmlFormat6;
            }
            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = _cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = _cfgCommon.FocusLockToStatusText;

            SettingDialog.RecommendStatusText = " [TWNv" + Regex.Replace(MyCommon.fileVersion.Replace(".", ""), "^0*", "") + "]";

            //書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(SettingDialog.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            SettingDialog.Nicoms = _cfgCommon.Nicoms;
            SettingDialog.HotkeyEnabled = _cfgCommon.HotkeyEnabled;
            SettingDialog.HotkeyMod = _cfgCommon.HotkeyModifier;
            SettingDialog.HotkeyKey = _cfgCommon.HotkeyKey;
            SettingDialog.HotkeyValue = _cfgCommon.HotkeyValue;
            SettingDialog.BlinkNewMentions = _cfgCommon.BlinkNewMentions;
            SettingDialog.UseAdditionalCount = _cfgCommon.UseAdditionalCount;
            SettingDialog.MoreCountApi = _cfgCommon.MoreCountApi;
            SettingDialog.FirstCountApi = _cfgCommon.FirstCountApi;
            SettingDialog.SearchCountApi = _cfgCommon.SearchCountApi;
            SettingDialog.FavoritesCountApi = _cfgCommon.FavoritesCountApi;
            SettingDialog.UserTimelineCountApi = _cfgCommon.UserTimelineCountApi;
            SettingDialog.ListCountApi = _cfgCommon.ListCountApi;
            SettingDialog.UserstreamStartup = _cfgCommon.UserstreamStartup;
            SettingDialog.UserstreamPeriodInt = _cfgCommon.UserstreamPeriod;
            SettingDialog.OpenUserTimeline = _cfgCommon.OpenUserTimeline;
            SettingDialog.ListDoubleClickAction = _cfgCommon.ListDoubleClickAction;
            SettingDialog.UserAppointUrl = _cfgCommon.UserAppointUrl;
            SettingDialog.HideDuplicatedRetweets = _cfgCommon.HideDuplicatedRetweets;
            SettingDialog.IsPreviewFoursquare = _cfgCommon.IsPreviewFoursquare;
            SettingDialog.FoursquarePreviewHeight = _cfgCommon.FoursquarePreviewHeight;
            SettingDialog.FoursquarePreviewWidth = _cfgCommon.FoursquarePreviewWidth;
            SettingDialog.FoursquarePreviewZoom = _cfgCommon.FoursquarePreviewZoom;
            SettingDialog.IsListStatusesIncludeRts = _cfgCommon.IsListsIncludeRts;
            SettingDialog.TabMouseLock = _cfgCommon.TabMouseLock;
            SettingDialog.IsRemoveSameEvent = _cfgCommon.IsRemoveSameEvent;
            SettingDialog.IsNotifyUseGrowl = _cfgCommon.IsUseNotifyGrowl;

            //ハッシュタグ関連
            HashSupl = new AtIdSupplement(_cfgCommon.HashTags, "#");
            HashMgr = new HashtagManage(HashSupl, _cfgCommon.HashTags.ToArray(), _cfgCommon.HashSelected, _cfgCommon.HashIsPermanent, _cfgCommon.HashIsHead, _cfgCommon.HashIsNotAddToAtReply);
            if (!String.IsNullOrEmpty(HashMgr.UseHash) && HashMgr.IsPermanent)
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
            }

            _initial = true;

            //アイコンリスト作成
            try
            {
                _TIconDic = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }
            ((ImageDictionary)this._TIconDic).PauseGetImage = false;

            bool saveRequired = false;
            //ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (String.IsNullOrEmpty(tw.Username))
            {
                saveRequired = true;
                //設定せずにキャンセルされた場合はプログラム終了
                if (SettingDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    Application.Exit();
                    //強制終了
                    return;
                }
                //設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
                if (String.IsNullOrEmpty(tw.Username))
                {
                    Application.Exit();
                    //強制終了
                    return;
                }
                //新しい設定を反映
                //フォント＆文字色＆背景色保持
                _fntUnread = SettingDialog.FontUnread;
                _clUnread = SettingDialog.ColorUnread;
                _fntReaded = SettingDialog.FontReaded;
                _clReaded = SettingDialog.ColorReaded;
                _clFav = SettingDialog.ColorFav;
                _clOWL = SettingDialog.ColorOWL;
                _clRetweet = SettingDialog.ColorRetweet;
                _fntDetail = SettingDialog.FontDetail;
                _clDetail = SettingDialog.ColorDetail;
                _clDetailLink = SettingDialog.ColorDetailLink;
                _clDetailBackcolor = SettingDialog.ColorDetailBackcolor;
                _clSelf = SettingDialog.ColorSelf;
                _clAtSelf = SettingDialog.ColorAtSelf;
                _clTarget = SettingDialog.ColorTarget;
                _clAtTarget = SettingDialog.ColorAtTarget;
                _clAtFromTarget = SettingDialog.ColorAtFromTarget;
                _clAtTo = SettingDialog.ColorAtTo;
                _clListBackcolor = SettingDialog.ColorListBackcolor;
                _clInputBackcolor = SettingDialog.ColorInputBackcolor;
                _clInputFont = SettingDialog.ColorInputFont;
                _fntInputFont = SettingDialog.FontInputFont;
                _brsForeColorUnread.Dispose();
                _brsForeColorReaded.Dispose();
                _brsForeColorFav.Dispose();
                _brsForeColorOWL.Dispose();
                _brsForeColorRetweet.Dispose();
                _brsForeColorUnread = new SolidBrush(_clUnread);
                _brsForeColorReaded = new SolidBrush(_clReaded);
                _brsForeColorFav = new SolidBrush(_clFav);
                _brsForeColorOWL = new SolidBrush(_clOWL);
                _brsForeColorRetweet = new SolidBrush(_clRetweet);
                _brsBackColorMine.Dispose();
                _brsBackColorAt.Dispose();
                _brsBackColorYou.Dispose();
                _brsBackColorAtYou.Dispose();
                _brsBackColorAtFromTarget.Dispose();
                _brsBackColorAtTo.Dispose();
                _brsBackColorNone.Dispose();
                _brsBackColorMine = new SolidBrush(_clSelf);
                _brsBackColorAt = new SolidBrush(_clAtSelf);
                _brsBackColorYou = new SolidBrush(_clTarget);
                _brsBackColorAtYou = new SolidBrush(_clAtTarget);
                _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
                _brsBackColorAtTo = new SolidBrush(_clAtTo);
                _brsBackColorNone = new SolidBrush(_clListBackcolor);

                if (SettingDialog.IsMonospace)
                {
                    _detailHtmlFormatHeader = detailHtmlFormatMono1;
                    _detailHtmlFormatFooter = detailHtmlFormatMono7;
                }
                else
                {
                    _detailHtmlFormatHeader = detailHtmlFormat1;
                    _detailHtmlFormatFooter = detailHtmlFormat7;
                }
                _detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString() + "," + _clDetail.G.ToString() + "," + _clDetail.B.ToString() + detailHtmlFormat4 + _clDetailLink.R.ToString() + "," + _clDetailLink.G.ToString() + "," + _clDetailLink.B.ToString() + detailHtmlFormat5 + _clDetailBackcolor.R.ToString() + "," + _clDetailBackcolor.G.ToString() + "," + _clDetailBackcolor.B.ToString();
                if (SettingDialog.IsMonospace)
                {
                    _detailHtmlFormatHeader += detailHtmlFormatMono6;
                }
                else
                {
                    _detailHtmlFormatHeader += detailHtmlFormat6;
                }
                //他の設定項目は、随時設定画面で保持している値を読み出して使用
            }

            if (SettingDialog.HotkeyEnabled)
            {
                ///グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((SettingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }
                if ((SettingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }
                if ((SettingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }
                if ((SettingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                _hookGlobalHotkey.RegisterOriginalHotkey(SettingDialog.HotkeyKey, SettingDialog.HotkeyValue, modKey);
            }

            //Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(SettingDialog.DefaultTimeOut, SettingDialog.SelectedProxyType, SettingDialog.ProxyAddress, SettingDialog.ProxyPort, SettingDialog.ProxyUser, SettingDialog.ProxyPassword);

            tw.SetRestrictFavCheck(SettingDialog.RestrictFavCheck);
            tw.ReadOwnPost = SettingDialog.ReadOwnPost;
            tw.SetUseSsl(SettingDialog.UseSsl);
            ShortUrl.IsResolve = SettingDialog.TinyUrlResolve;
            ShortUrl.IsForceResolve = SettingDialog.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(SettingDialog.BitlyUser);
            ShortUrl.SetBitlyKey(SettingDialog.BitlyPwd);
            HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);
            tw.TrackWord = _cfgCommon.TrackWord;
            TrackToolStripMenuItem.Checked = !String.IsNullOrEmpty(tw.TrackWord);
            tw.AllAtReply = _cfgCommon.AllAtReply;
            AllrepliesToolStripMenuItem.Checked = tw.AllAtReply;

            Outputz.Key = SettingDialog.OutputzKey;
            Outputz.Enabled = SettingDialog.OutputzEnabled;
            switch (SettingDialog.OutputzUrlmode)
            {
                case Hoehoe.MyCommon.OutputzUrlmode.twittercom:
                    Outputz.OutUrl = "http://twitter.com/";
                    break;
                case Hoehoe.MyCommon.OutputzUrlmode.twittercomWithUsername:
                    Outputz.OutUrl = "http://twitter.com/" + tw.Username;
                    break;
            }

            //画像投稿サービス
            this.CreatePictureServices();
            SetImageServiceCombo();
            ImageSelectionPanel.Enabled = false;

            ImageServiceCombo.SelectedIndex = _cfgCommon.UseImageService;

            //ウィンドウ設定
            this.ClientSize = _cfgLocal.FormSize;
            _mySize = _cfgLocal.FormSize;
            //サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            _myLoc = _cfgLocal.FormLocation;
            //タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = _cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(_mySize.Width, SystemInformation.CaptionHeight));
                bool outOfScreen = true;
                //ハングするとの報告
                if (Screen.AllScreens.Length == 1)
                {
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (!Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty)
                        {
                            outOfScreen = false;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                    if (outOfScreen)
                    {
                        this.DesktopLocation = new Point(0, 0);
                        _myLoc = this.DesktopLocation;
                    }
                }
            }
            this.TopMost = SettingDialog.AlwaysTop;
            _mySpDis = _cfgLocal.SplitterDistance;
            _mySpDis2 = _cfgLocal.StatusTextHeight;
            _mySpDis3 = _cfgLocal.PreviewDistance;
            if (_mySpDis3 == -1)
            {
                _mySpDis3 = _mySize.Width - 150;
                if (_mySpDis3 < 1)
                {
                    _mySpDis3 = 50;
                }
                _cfgLocal.PreviewDistance = _mySpDis3;
            }
            _myAdSpDis = _cfgLocal.AdSplitterDistance;
            MultiLineMenuItem.Checked = _cfgLocal.StatusMultiline;
            //Me.Tween_ClientSizeChanged(Me, Nothing)
            PlaySoundMenuItem.Checked = SettingDialog.PlaySound;
            this.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound;
            //入力欄
            StatusText.Font = _fntInputFont;
            StatusText.ForeColor = _clInputFont;

            //全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (SettingDialog.UnreadManage == false)
            {
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }

            if (SettingDialog.IsNotifyUseGrowl)
            {
                gh.RegisterGrowl();
            }

            //タイマー設定
            TimerTimeline.AutoReset = true;
            TimerTimeline.SynchronizingObject = this;
            //Recent取得間隔
            TimerTimeline.Interval = 1000;
            TimerTimeline.Enabled = true;

            //更新中アイコンアニメーション間隔
            TimerRefreshIcon.Interval = 200;
            TimerRefreshIcon.Enabled = true;

            //状態表示部の初期化（画面右下）
            StatusLabel.Text = "";
            StatusLabel.AutoToolTip = false;
            StatusLabel.ToolTipText = "";
            //文字カウンタ初期化
            lblLen.Text = GetRestStatusCount(true, false).ToString();

            ///'''''''''''''''''''''''''''''''''''''
            _statuses.SortOrder = (SortOrder)_cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (_cfgCommon.SortColumn)
            {
                case 0:
                case 5:
                case 6:
                    //0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                    //ソートしない
                    mode = IdComparerClass.ComparerMode.Id;
                    //Idソートに読み替え
                    break;
                case 1:
                    //ニックネーム
                    mode = IdComparerClass.ComparerMode.Nickname;
                    break;
                case 2:
                    //本文
                    mode = IdComparerClass.ComparerMode.Data;
                    break;
                case 3:
                    //時刻=発言Id
                    mode = IdComparerClass.ComparerMode.Id;
                    break;
                case 4:
                    //名前
                    mode = IdComparerClass.ComparerMode.Name;
                    break;
                case 7:
                    //Source
                    mode = IdComparerClass.ComparerMode.Source;
                    break;
            }
            _statuses.SortMode = mode;
            ///'''''''''''''''''''''''''''''''''''''

            switch (SettingDialog.IconSz)
            {
                case Hoehoe.MyCommon.IconSizes.IconNone:
                    _iconSz = 0;
                    break;
                case Hoehoe.MyCommon.IconSizes.Icon16:
                    _iconSz = 16;
                    break;
                case Hoehoe.MyCommon.IconSizes.Icon24:
                    _iconSz = 26;
                    break;
                case Hoehoe.MyCommon.IconSizes.Icon48:
                    _iconSz = 48;
                    break;
                case Hoehoe.MyCommon.IconSizes.Icon48_2:
                    _iconSz = 48;
                    _iconCol = true;
                    break;
            }
            if (_iconSz == 0)
            {
                tw.SetGetIcon(false);
            }
            else
            {
                tw.SetGetIcon(true);
                tw.SetIconSize(_iconSz);
            }
            tw.SetTinyUrlResolve(SettingDialog.TinyUrlResolve);
            ShortUrl.IsForceResolve = SettingDialog.ShortUrlForceResolve;

            //発言詳細部アイコンをリストアイコンにサイズ変更
            int sz = _iconSz;
            if (_iconSz == 0)
            {
                sz = 16;
            }

            tw.DetailIcon = _TIconDic;

            StatusLabel.Text = Hoehoe.Properties.Resources.Form1_LoadText1;
            //画面右下の状態表示を変更
            StatusLabelUrl.Text = "";
            //画面左下のリンク先URL表示部を初期化
            NameLabel.Text = "";
            //発言詳細部名前ラベル初期化
            DateTimeLabel.Text = "";
            //発言詳細部日時ラベル初期化
            SourceLinkLabel.Text = "";
            //Source部分初期化

            //<<<<<<<<タブ関連>>>>>>>
            //デフォルトタブの存在チェック、ない場合には追加
            if (_statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.Home) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.RECENT))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, Hoehoe.MyCommon.TabUsageType.Home, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.RECENT].TabType = Hoehoe.MyCommon.TabUsageType.Home;
                }
            }
            if (_statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.Mentions) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.REPLY))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, Hoehoe.MyCommon.TabUsageType.Mentions, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.REPLY].TabType = Hoehoe.MyCommon.TabUsageType.Mentions;
                }
            }
            if (_statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.DirectMessage) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.DM))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, Hoehoe.MyCommon.TabUsageType.DirectMessage, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.DM].TabType = Hoehoe.MyCommon.TabUsageType.DirectMessage;
                }
            }
            if (_statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.Favorites) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.FAV))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, Hoehoe.MyCommon.TabUsageType.Favorites, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.FAV].TabType = Hoehoe.MyCommon.TabUsageType.Favorites;
                }
            }
            foreach (string tn in _statuses.Tabs.Keys)
            {
                if (_statuses.Tabs[tn].TabType == Hoehoe.MyCommon.TabUsageType.Undefined)
                {
                    _statuses.Tabs[tn].TabType = Hoehoe.MyCommon.TabUsageType.UserDefined;
                }
                if (!AddNewTab(tn, true, _statuses.Tabs[tn].TabType, _statuses.Tabs[tn].ListInfo))
                {
                    throw new Exception(Hoehoe.Properties.Resources.TweenMain_LoadText1);
                }
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (SettingDialog.MinimizeToTray == false || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }
            _curTab = ListTab.SelectedTab;
            _curItemIndex = -1;
            _curList = (DetailsListView)_curTab.Tag;
            SetMainWindowTitle();
            SetNotifyIconText();

            if (SettingDialog.TabIconDisp)
            {
                ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                ListTab.DrawItem += ListTab_DrawItem;
                ListTab.ImageList = null;
            }

#if UA //= "True"
			ab = new AdsBrowser();
			this.SplitContainer4.Panel2.Controls.Add(ab);
#else
            SplitContainer4.Panel2Collapsed = true;
#endif

            _ignoreConfigSave = false;
            this.TweenMain_Resize(null, null);
            if (saveRequired)
            {
                SaveConfigsAll(false);
            }

            if (tw.UserId == 0)
            {
                tw.VerifyCredentials();
                foreach (var ua in _cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == tw.Username.ToLower())
                    {
                        ua.UserId = tw.UserId;
                        break;
                    }
                }
            }
            foreach (var ua in SettingDialog.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == tw.Username.ToLower())
                {
                    ua.UserId = tw.UserId;
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
            this._pictureServices = new Dictionary<string, IMultimediaShareService> {
                { "TwitPic", new TwitPic(tw) },
                { "img.ly", new imgly(tw) },
                { "yfrog", new yfrog(tw) },
                { "lockerz", new Plixi(tw) },
                { "Twitter", new TwitterPhoto(tw) }
            };
        }

        private void spaceKeyCanceler_SpaceCancel(object sender, EventArgs e)
        {
            JumpUnreadMenuItem_Click(null, null);
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
                if (_statuses.Tabs[txt].UnreadCount > 0)
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
            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, sfTab);
        }

        private void LoadConfig()
        {
            _cfgCommon = SettingCommon.Load();
            if (_cfgCommon.UserAccounts == null || _cfgCommon.UserAccounts.Count == 0)
            {
                _cfgCommon.UserAccounts = new List<UserAccount>();
                if (!String.IsNullOrEmpty(_cfgCommon.UserName))
                {
                    _cfgCommon.UserAccounts.Add(new UserAccount
                    {
                        Username = _cfgCommon.UserName,
                        UserId = _cfgCommon.UserId,
                        Token = _cfgCommon.Token,
                        TokenSecret = _cfgCommon.TokenSecret
                    });
                }
            }
            _cfgLocal = SettingLocal.Load();
            List<TabClass> tabs = SettingTabs.Load().Tabs;
            foreach (TabClass tb in tabs)
            {
                try
                {
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = _statuses.GetUniqueTabName();
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
            }
            if (_statuses.Tabs.Count == 0)
            {
                _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, Hoehoe.MyCommon.TabUsageType.Home, null);
                _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, Hoehoe.MyCommon.TabUsageType.Mentions, null);
                _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, Hoehoe.MyCommon.TabUsageType.DirectMessage, null);
                _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, Hoehoe.MyCommon.TabUsageType.Favorites, null);
            }
        }

        private void TimerInterval_Changed(object sender, AppendSettingDialog.IntervalChangedEventArgs e)
        {
            if (!TimerTimeline.Enabled)
            {
                return;
            }
            ResetTimers = e;
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
            if (_timerHomeCounter > 0)
            {
                Interlocked.Decrement(ref _timerHomeCounter);
            }
            if (_timerMentionCounter > 0)
            {
                Interlocked.Decrement(ref _timerMentionCounter);
            }
            if (_timerDmCounter > 0)
            {
                Interlocked.Decrement(ref _timerDmCounter);
            }
            if (_timerPubSearchCounter > 0)
            {
                Interlocked.Decrement(ref _timerPubSearchCounter);
            }
            if (_timerUserTimelineCounter > 0)
            {
                Interlocked.Decrement(ref _timerUserTimelineCounter);
            }
            if (_timerListsCounter > 0)
            {
                Interlocked.Decrement(ref _timerListsCounter);
            }
            if (_timerUsCounter > 0)
            {
                Interlocked.Decrement(ref _timerUsCounter);
            }
            Interlocked.Increment(ref _timerRefreshFollowers);

            //'タイマー初期化
            if (ResetTimers.Timeline || _timerHomeCounter <= 0 && SettingDialog.TimelinePeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerHomeCounter, SettingDialog.TimelinePeriodInt);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.Timeline)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.Timeline, 1, 0, "");
                }
                ResetTimers.Timeline = false;
            }
            if (ResetTimers.Reply || _timerMentionCounter <= 0 && SettingDialog.ReplyPeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerMentionCounter, SettingDialog.ReplyPeriodInt);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.Reply)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.Reply, 1, 0, "");
                }
                ResetTimers.Reply = false;
            }
            if (ResetTimers.DirectMessage || _timerDmCounter <= 0 && SettingDialog.DMPeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerDmCounter, SettingDialog.DMPeriodInt);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.DirectMessage)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.DirectMessegeRcv, 1, 0, "");
                }
                ResetTimers.DirectMessage = false;
            }
            if (ResetTimers.PublicSearch || _timerPubSearchCounter <= 0 && SettingDialog.PubSearchPeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerPubSearchCounter, SettingDialog.PubSearchPeriodInt);
                if (!ResetTimers.PublicSearch)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.PublicSearch, 1, 0, "");
                }
                ResetTimers.PublicSearch = false;
            }
            if (ResetTimers.UserTimeline || _timerUserTimelineCounter <= 0 && SettingDialog.UserTimelinePeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerUserTimelineCounter, SettingDialog.UserTimelinePeriodInt);
                if (!ResetTimers.UserTimeline)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.UserTimeline, 1, 0, "");
                }
                ResetTimers.UserTimeline = false;
            }
            if (ResetTimers.Lists || _timerListsCounter <= 0 && SettingDialog.ListsPeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerListsCounter, SettingDialog.ListsPeriodInt);
                if (!ResetTimers.Lists)
                {
                    GetTimeline(Hoehoe.MyCommon.WorkerType.List, 1, 0, "");
                }
                ResetTimers.Lists = false;
            }
            if (ResetTimers.UserStream || _timerUsCounter <= 0 && SettingDialog.UserstreamPeriodInt > 0)
            {
                Interlocked.Exchange(ref _timerUsCounter, SettingDialog.UserstreamPeriodInt);
                if (this._isActiveUserstream)
                {
                    RefreshTimeline(true);
                }
                ResetTimers.UserStream = false;
            }
            if (_timerRefreshFollowers > 6 * 3600)
            {
                Interlocked.Exchange(ref _timerRefreshFollowers, 0);
                doGetFollowersMenu();
                GetTimeline(Hoehoe.MyCommon.WorkerType.Configuration, 0, 0, "");
                if (InvokeRequired && !IsDisposed)
                {
                    this.Invoke(new MethodInvoker(this.TrimPostChain));
                }
            }
            if (_osResumed)
            {
                Interlocked.Increment(ref _timerResumeWait);
                if (_timerResumeWait > 30)
                {
                    _osResumed = false;
                    Interlocked.Exchange(ref _timerResumeWait, 0);
                    GetTimeline(Hoehoe.MyCommon.WorkerType.Timeline, 1, 0, "");
                    GetTimeline(Hoehoe.MyCommon.WorkerType.Reply, 1, 0, "");
                    GetTimeline(Hoehoe.MyCommon.WorkerType.DirectMessegeRcv, 1, 0, "");
                    GetTimeline(Hoehoe.MyCommon.WorkerType.PublicSearch, 1, 0, "");
                    GetTimeline(Hoehoe.MyCommon.WorkerType.UserTimeline, 1, 0, "");
                    GetTimeline(Hoehoe.MyCommon.WorkerType.List, 1, 0, "");
                    doGetFollowersMenu();
                    GetTimeline(Hoehoe.MyCommon.WorkerType.Configuration, 0, 0, "");
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
            //スクロール制御準備
            int smode = -1;
            //-1:制御しない,-2:最新へ,その他:topitem使用
            long topId = GetScrollPos(ref smode);
            int befCnt = _curList.VirtualListSize;

            //現在の選択状態を退避
            Dictionary<string, long[]> selId = new Dictionary<string, long[]>();
            Dictionary<string, long> focusedId = new Dictionary<string, long>();
            SaveSelectedStatus(selId, focusedId);

            //mentionsの更新前件数を保持
            int dmCount = _statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.DirectMessage).AllCount;

            //更新確定
            PostClass[] notifyPosts = null;
            string soundFile = "";
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = _statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon.IsEnding)
            {
                return;
            }

            //リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = _statuses.Tabs[tab.Text];
                    lst.BeginUpdate();
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        if (lst.Equals(_curList))
                        {
                            _itemCache = null;
                            _postCache = null;
                        }
                        try
                        {
                            lst.VirtualListSize = tabInfo.AllCount;
                            //リスト件数更新
                        }
                        catch (Exception)
                        {
                            //アイコン描画不具合あり？
                        }
                        this.SelectListItem(lst, _statuses.IndexOf(tab.Text, selId[tab.Text]), _statuses.IndexOf(tab.Text, focusedId[tab.Text]));
                    }
                    lst.EndUpdate();
                    if (tabInfo.UnreadCount > 0)
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            if (tab.ImageIndex == -1)
                            {//タブアイコン
                                tab.ImageIndex = 0;
                            }
                        }
                    }
                }
                if (!SettingDialog.TabIconDisp)
                {
                    ListTab.Refresh();
                }
            }
            catch (Exception)
            {
            }

            //スクロール制御後処理
            if (smode != -1)
            {
                try
                {
                    if (befCnt != _curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                //最上行
                                if (_curList.VirtualListSize > 0)
                                {
                                    _curList.EnsureVisible(0);
                                }
                                break;
                            case -2:
                                //最下行へ
                                if (_curList.VirtualListSize > 0)
                                {
                                    _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                }
                                break;
                            case -1:
                                break;
                            //制御しない
                            default:
                                //表示位置キープ
                                if (_curList.VirtualListSize > 0 && _statuses.IndexOf(_curTab.Text, topId) > -1)
                                {
                                    _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                    _curList.EnsureVisible(_statuses.IndexOf(_curTab.Text, topId));
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

            //新着通知
            NotifyNewPosts(notifyPosts, soundFile, addCount, isMention || dmCount != _statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.DirectMessage).AllCount);

            SetMainWindowTitle();
            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                SetStatusLabelUrl();
            }

            HashSupl.AddRangeItem(tw.GetHashList());
        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (_curList != null && _curTab != null && _curList.VirtualListSize > 0)
            {
                if (_statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if (_statuses.SortOrder == SortOrder.Ascending)
                    {
                        //Id昇順
                        if (ListLockMenuItem.Checked)
                        {
                            //制御しない
                            smode = -1;
                            //'現在表示位置へ強制スクロール
                        }
                        else
                        {
                            //最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                            ListViewItem item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
                            //一番下
                            if (item == null)
                            {
                                item = _curList.Items[_curList.Items.Count - 1];
                            }
                            if (item.Index == _curList.Items.Count - 1)
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
                        //Id降順
                        if (ListLockMenuItem.Checked)
                        {
                            //現在表示位置へ強制スクロール
                            if (_curList.TopItem != null)
                            {
                                topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                            }
                            smode = 0;
                        }
                        else
                        {
                            //最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                            ListViewItem item = _curList.GetItemAt(0, 10);
                            //一番上
                            if (item == null)
                            {
                                item = _curList.Items[0];
                            }
                            if (item.Index == 0)
                            {
                                smode = -3;
                                //最上行
                            }
                            else
                            {
                                if (_curList.TopItem != null)
                                {
                                    topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                                }
                                smode = 0;
                            }
                        }
                    }
                }
                else
                {
                    //現在表示位置へ強制スクロール
                    if (_curList.TopItem != null)
                    {
                        topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
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
                    selId.Add(tab.Text, _statuses.GetId(tab.Text, lst.SelectedIndices));
                }
                else
                {
                    selId.Add(tab.Text, new long[1] { -2 });
                }
                if (lst.FocusedItem != null)
                {
                    focusedId.Add(tab.Text, _statuses.GetId(tab.Text, lst.FocusedItem.Index));
                }
                else
                {
                    focusedId.Add(tab.Text, -2);
                }
            }
        }

        private bool BalloonRequired()
        {
            return BalloonRequired(new Twitter.FormattedEvent { Eventtype = Hoehoe.MyCommon.EventType.None });
        }

        private bool IsEventNotifyAsEventType(Hoehoe.MyCommon.EventType type)
        {
            return SettingDialog.EventNotifyEnabled && Convert.ToBoolean(type & SettingDialog.EventNotifyFlag) || type == Hoehoe.MyCommon.EventType.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return Convert.ToBoolean(ev.Eventtype & SettingDialog.IsMyEventNotifyFlag) ? true : !ev.IsMe;
        }

        private bool BalloonRequired(Twitter.FormattedEvent ev)
        {
            return IsEventNotifyAsEventType(ev.Eventtype) && IsMyEventNotityAsEventType(ev) && (NewPostPopMenuItem.Checked || (SettingDialog.ForceEventNotify && ev.Eventtype != Hoehoe.MyCommon.EventType.None)) && !_initial && ((SettingDialog.LimitBalloon && (this.WindowState == FormWindowState.Minimized || !this.Visible || Form.ActiveForm == null)) || !SettingDialog.LimitBalloon) && !Win32Api.IsScreenSaverRunning();
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null && notifyPosts.Count() > 0 && this.SettingDialog.ReadOwnPost && notifyPosts.All(post => post.UserId == tw.UserId || post.ScreenName == tw.Username))
            {
                return;
            }

            //新着通知
            if (BalloonRequired())
            {
                if (notifyPosts != null && notifyPosts.Length > 0)
                {
                    //Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                    if (SettingDialog.IsNotifyUseGrowl)
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
                            switch (SettingDialog.NameBalloon)
                            {
                                case Hoehoe.MyCommon.NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case Hoehoe.MyCommon.NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }
                            sb.Append(post.TextFromApi);
                            if (notifyPosts.Count() > 3)
                            {
                                if (!Object.ReferenceEquals(notifyPosts.Last(), post))
                                {
                                    continue;
                                }
                            }

                            StringBuilder title = new StringBuilder();
                            GrowlHelper.NotifyType nt = default(GrowlHelper.NotifyType);
                            if (SettingDialog.DispUsername)
                            {
                                title.Append(tw.Username);
                                title.Append(" - ");
                            }
                            else
                            {
                                //title.Clear()
                            }
                            if (dm)
                            {
                                title.Append("Tween [DM] ");
                                title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText2);
                                nt = GrowlHelper.NotifyType.DirectMessage;
                            }
                            else if (reply)
                            {
                                title.Append("Tween [Reply!] ");
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                                nt = GrowlHelper.NotifyType.Reply;
                            }
                            else
                            {
                                title.Append("Tween ");
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                                nt = GrowlHelper.NotifyType.Notify;
                            }
                            string bText = sb.ToString();
                            if (String.IsNullOrEmpty(bText))
                            {
                                return;
                            }
                            gh.Notify(nt, post.StatusId.ToString(), title.ToString(), bText, this._TIconDic[post.ImageUrl], post.ImageUrl);
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
                            switch (SettingDialog.NameBalloon)
                            {
                                case Hoehoe.MyCommon.NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case Hoehoe.MyCommon.NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }
                            sb.Append(post.TextFromApi);
                        }

                        StringBuilder title = new StringBuilder();
                        ToolTipIcon ntIcon = default(ToolTipIcon);
                        GrowlHelper.NotifyType nt = default(GrowlHelper.NotifyType);
                        if (SettingDialog.DispUsername)
                        {
                            title.Append(tw.Username);
                            title.Append(" - ");
                        }
                        else
                        {
                        }

                        if (dm)
                        {
                            ntIcon = ToolTipIcon.Warning;
                            title.Append("Tween [DM] ");
                            title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshDirectMessageText2);
                            nt = GrowlHelper.NotifyType.DirectMessage;
                        }
                        else if (reply)
                        {
                            ntIcon = ToolTipIcon.Warning;
                            title.Append("Tween [Reply!] ");
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                            nt = GrowlHelper.NotifyType.Reply;
                        }
                        else
                        {
                            ntIcon = ToolTipIcon.Info;
                            title.Append("Tween ");
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Hoehoe.Properties.Resources.RefreshTimelineText2);
                            nt = GrowlHelper.NotifyType.Notify;
                        }
                        string bText = sb.ToString();
                        if (String.IsNullOrEmpty(bText))
                        {
                            return;
                        }
                        NotifyIcon1.BalloonTipTitle = title.ToString();
                        NotifyIcon1.BalloonTipText = bText;
                        NotifyIcon1.BalloonTipIcon = ntIcon;
                        NotifyIcon1.ShowBalloonTip(500);
                    }
                }
            }

            //サウンド再生
            if (!_initial && SettingDialog.PlaySound && !String.IsNullOrEmpty(soundFile))
            {
                try
                {
                    string dir = MyCommon.GetAppDir();
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

            //mentions新着時に画面ブリンク
            if (!_initial && SettingDialog.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                Win32Api.FlashMyWindow(this.Handle, Hoehoe.Win32Api.FlashSpecification.FlashTray, 3);
            }
        }

        private void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_curList == null || _curList.SelectedIndices.Count != 1)
            {
                return;
            }

            _curItemIndex = _curList.SelectedIndices[0];
            if (_curItemIndex > _curList.VirtualListSize - 1)
            {
                return;
            }

            try
            {
                _curPost = GetCurTabPost(_curItemIndex);
            }
            catch (ArgumentException ex)
            {
                return;
            }

            this.PushSelectPostChain();

            if (SettingDialog.UnreadManage)
            {
                _statuses.SetReadAllTab(true, _curTab.Text, _curItemIndex);
            }
            //キャッシュの書き換え
            ChangeCacheStyleRead(true, _curItemIndex, _curTab);
            //既読へ（フォント、文字色）

            ColorizeList();
            _colorize = true;
        }

        private void ChangeCacheStyleRead(bool read, int index, TabPage tab)
        {
            //Read:True=既読 False=未読
            //未読管理していなかったら既読として扱う
            if (!_statuses.Tabs[_curTab.Text].UnreadManage || !SettingDialog.UnreadManage)
            {
                read = true;
            }

            //対象の特定
            ListViewItem itm = null;
            PostClass post = null;
            if (tab.Equals(_curTab) && _itemCache != null && index >= _itemCacheIndex && index < _itemCacheIndex + _itemCache.Length)
            {
                itm = _itemCache[index - _itemCacheIndex];
                post = _postCache[index - _itemCacheIndex];
            }
            else
            {
                itm = ((DetailsListView)tab.Tag).Items[index];
                post = _statuses.Item(tab.Text, index);
            }

            ChangeItemStyleRead(read, itm, post, (DetailsListView)tab.Tag);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem lvItem, PostClass post, DetailsListView dlView)
        {
            //フォント
            Font fnt = read ? _fntReaded : _fntUnread;
            lvItem.SubItems[5].Text = read ? "" : "★";
            //文字色
            Color cl = default(Color);
            if (post.IsFav)
            {
                cl = _clFav;
            }
            else if (post.RetweetedId > 0)
            {
                cl = _clRetweet;
            }
            else if (post.IsOwl && (post.IsDm || SettingDialog.OneWayLove))
            {
                cl = _clOWL;
            }
            else if (read || !SettingDialog.UseUnreadStyle)
            {
                cl = _clReaded;
            }
            else
            {
                cl = _clUnread;
            }
            if (dlView == null || lvItem.Index == -1)
            {
                lvItem.ForeColor = cl;
                if (SettingDialog.UseUnreadStyle)
                {
                    lvItem.Font = fnt;
                }
            }
            else
            {
                dlView.Update();
                if (SettingDialog.UseUnreadStyle)
                {
                    dlView.ChangeItemFontAndColor(lvItem.Index, cl, fnt);
                }
                else
                {
                    dlView.ChangeItemForeColor(lvItem.Index, cl);
                }
            }
        }

        private void ColorizeList()
        {
            //Index:更新対象のListviewItem.Index。Colorを返す。
            //-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass post = _anchorFlag ? _anchorPost : _curPost;

            if (_itemCache == null)
            {
                return;
            }

            if (post == null)
            {
                return;
            }

            try
            {
                for (int cnt = 0; cnt < _itemCache.Length; cnt++)
                {
                    _curList.ChangeItemBackColor(_itemCacheIndex + cnt, JudgeColor(post, _postCache[cnt]));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ColorizeList(ListViewItem lvItem, int index)
        {
            //Index:更新対象のListviewItem.Index。Colorを返す。
            //-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass post = _anchorFlag ? _anchorPost : _curPost;
            PostClass tPost = GetCurTabPost(index);

            if (post == null)
            {
                return;
            }

            if (lvItem.Index == -1)
            {
                lvItem.BackColor = JudgeColor(post, tPost);
            }
            else
            {
                _curList.ChangeItemBackColor(lvItem.Index, JudgeColor(post, tPost));
            }
        }

        private Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl = default(Color);
            if (targetPost.StatusId == basePost.InReplyToStatusId)
            {
                //@先
                cl = _clAtTo;
            }
            else if (targetPost.IsMe)
            {
                //自分=発言者
                cl = _clSelf;
            }
            else if (targetPost.IsReply)
            {
                //自分宛返信
                cl = _clAtSelf;
            }
            else if (basePost.ReplyToList.Contains(targetPost.ScreenName.ToLower()))
            {
                //返信先
                cl = _clAtFromTarget;
            }
            else if (targetPost.ReplyToList.Contains(basePost.ScreenName.ToLower()))
            {
                //その人への返信
                cl = _clAtTarget;
            }
            else if (targetPost.ScreenName.Equals(basePost.ScreenName, StringComparison.OrdinalIgnoreCase))
            {
                //発言者
                cl = _clTarget;
            }
            else
            {
                //その他
                cl = _clListBackcolor;
            }
            return cl;
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            if (StatusText.Text.Trim().Length == 0)
            {
                if (!ImageSelectionPanel.Enabled)
                {
                    DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && StatusText.Text.Trim() == string.Format("RT @{0}: {1}", _curPost.ScreenName, _curPost.TextFromApi))
            {
                DialogResult rtResult = MessageBox.Show(string.Format(Hoehoe.Properties.Resources.PostButton_Click1, Environment.NewLine), "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (rtResult)
                {
                    case DialogResult.Yes:
                        doReTweetOfficial(false);
                        StatusText.Text = "";
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            _history[_history.Count - 1] = new PostingStatus(StatusText.Text.Trim(), _replyToId, _replyToName);

            if (SettingDialog.Nicoms)
            {
                StatusText.SelectionStart = StatusText.Text.Length;
                UrlConvert(Hoehoe.MyCommon.UrlConverter.Nicoms);
            }

            StatusText.SelectionStart = StatusText.Text.Length;
            GetWorkerArg args = new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = Hoehoe.MyCommon.WorkerType.PostMessage };
            CheckReplyTo(StatusText.Text);

            //整形によって増加する文字数を取得
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
            bool isRemoveFooter = isKeyDown(Keys.Shift);
            if (StatusText.Multiline && !SettingDialog.PostCtrlEnter)
            {
                //複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = isKeyDown(Keys.Control);
            }
            if (SettingDialog.PostShiftEnter)
            {
                isRemoveFooter = isKeyDown(Keys.Control);
            }
            if (!isRemoveFooter && (StatusText.Text.Contains("RT @") || StatusText.Text.Contains("QT @")))
            {
                isRemoveFooter = true;
            }
            if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
            {
                if (MessageBox.Show(Hoehoe.Properties.Resources.PostLengthOverMessage1, Hoehoe.Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    isCutOff = true;
                    if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
                    {
                        isRemoveFooter = true;
                    }
                }
                else
                {
                    return;
                }
            }

            string footer = "";
            string header = "";
            if (StatusText.Text.StartsWith("D ") || StatusText.Text.StartsWith("d "))
            {
                //DM時は何もつけない
                footer = "";
            }
            else
            {
                //ハッシュタグ
                if (HashMgr.IsNotAddToAtReply)
                {
                    if (!String.IsNullOrEmpty(HashMgr.UseHash) && _replyToId == 0 && String.IsNullOrEmpty(_replyToName))
                    {
                        if (HashMgr.IsHead)
                        {
                            header = HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + HashMgr.UseHash;
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(HashMgr.UseHash))
                    {
                        if (HashMgr.IsHead)
                        {
                            header = HashMgr.UseHash + " ";
                        }
                        else
                        {
                            footer = " " + HashMgr.UseHash;
                        }
                    }
                }
                if (!isRemoveFooter)
                {
                    if (SettingDialog.UseRecommendStatus)
                    {
                        // 推奨ステータスを使用する
                        footer += SettingDialog.RecommendStatusText;
                    }
                    else
                    {
                        // テキストボックスに入力されている文字列を使用する
                        footer += " " + SettingDialog.Status.Trim();
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
                //簡易判定
                string pattern = string.Format("({0})|({1})|({2})", AtId, HashTag, Url);
                Match mc = Regex.Match(args.PStatus.Status, pattern, RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    //さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                    args.PStatus.Status = args.PStatus.Status.Substring(0, 140 - mc.Value.Length);
                }
                if (MessageBox.Show(args.PStatus.Status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            args.PStatus.InReplyToId = _replyToId;
            args.PStatus.InReplyToName = _replyToName;
            if (ImageSelectionPanel.Visible)
            {
                //画像投稿
                if (!object.ReferenceEquals(ImageSelectedPicture.Image, ImageSelectedPicture.InitialImage) && ImageServiceCombo.SelectedIndex > -1 && !String.IsNullOrEmpty(ImagefilePathText.Text))
                {
                    if (MessageBox.Show(Hoehoe.Properties.Resources.PostPictureConfirm1, Hoehoe.Properties.Resources.PostPictureConfirm2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        TimelinePanel.Visible = true;
                        TimelinePanel.Enabled = true;
                        ImageSelectionPanel.Visible = false;
                        ImageSelectionPanel.Enabled = false;
                        if (_curList != null)
                        {
                            _curList.Focus();
                        }
                        return;
                    }
                    args.PStatus.ImageService = ImageServiceCombo.Text;
                    args.PStatus.ImagePath = ImagefilePathText.Text;
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImagefilePathText.Text = "";
                    TimelinePanel.Visible = true;
                    TimelinePanel.Enabled = true;
                    ImageSelectionPanel.Visible = false;
                    ImageSelectionPanel.Enabled = false;
                    if (_curList != null)
                    {
                        _curList.Focus();
                    }
                }
                else
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.PostPictureWarn1, Hoehoe.Properties.Resources.PostPictureWarn2);
                    return;
                }
            }

            RunAsync(args);

            //Google検索（試験実装）
            if (StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && StatusText.Text.Trim().Length > 7)
            {
                string tmp = string.Format(Hoehoe.Properties.Resources.SearchItem2Url, HttpUtility.UrlEncode(StatusText.Text.Substring(7)));
                OpenUriAsync(tmp);
            }

            _replyToId = 0;
            _replyToName = "";
            StatusText.Text = "";
            _history.Add(new PostingStatus());
            _hisIdx = _history.Count - 1;
            if (!ToolStripFocusLockMenuItem.Checked)
            {
                ((Control)ListTab.SelectedTab.Tag).Focus();
            }
            urlUndoBuffer = null;
            UrlUndoToolStripMenuItem.Enabled = false;
            //Undoをできないように設定
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.IsEnding = true;
            this.Close();
        }

        private void Tween_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SettingDialog.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon.IsEnding == false)
            {
                //_endingFlag=False:フォームの×ボタン
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                //Google.GASender.GetInstance().TrackEventWithCategory("post", "end", tw.UserId)
                _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                _ignoreConfigSave = true;
                MyCommon.IsEnding = true;
                TimerTimeline.Enabled = false;
                TimerRefreshIcon.Enabled = false;
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
            if (Twitter.AccountState != Hoehoe.MyCommon.AccountState.Valid)
            {
                _accountCheckErrorCount += 1;
                if (_accountCheckErrorCount > 5)
                {
                    _accountCheckErrorCount = 0;
                    Twitter.AccountState = Hoehoe.MyCommon.AccountState.Valid;
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

            //Tween.My.MyProject.Application.InitCulture(); // TODO: Need this here?

            string ret = "";
            GetWorkerResult rslt = new GetWorkerResult();

            bool read = !SettingDialog.UnreadManage;
            if (_initial && SettingDialog.UnreadManage)
            {
                read = SettingDialog.Readed;
            }

            GetWorkerArg args = (GetWorkerArg)e.Argument;

            if (!CheckAccountValid())
            {
                rslt.RetMsg = "Auth error. Check your account";
                rslt.WorkerType = Hoehoe.MyCommon.WorkerType.ErrorState;
                //エラー表示のみ行ない、後処理キャンセル
                rslt.TabName = args.TabName;
                e.Result = rslt;
                return;
            }

            if (args.WorkerType != Hoehoe.MyCommon.WorkerType.OpenUri)
            {
                bw.ReportProgress(0, "");
            }
            //Notifyアイコンアニメーション開始
            switch (args.WorkerType)
            {
                case Hoehoe.MyCommon.WorkerType.Timeline:
                case Hoehoe.MyCommon.WorkerType.Reply:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, _initial);
                    //新着時未読クリア
                    if (String.IsNullOrEmpty(ret) && args.WorkerType == Hoehoe.MyCommon.WorkerType.Timeline && SettingDialog.ReadOldPosts)
                    {
                        _statuses.SetRead();
                    }
                    //振り分け
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.DirectMessegeRcv:
                    //送信分もまとめて取得
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = tw.GetDirectMessageApi(read, Hoehoe.MyCommon.WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (String.IsNullOrEmpty(ret))
                    {
                        ret = tw.GetDirectMessageApi(read, Hoehoe.MyCommon.WorkerType.DirectMessegeSnt, args.Page == -1);
                    }
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.FavAdd:
                    //スレッド処理はしない
                    if (_statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = _statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = null;
                            if (tbc.IsInnerStorageTabType)
                            {
                                post = tbc.Posts[args.Ids[i]];
                            }
                            else
                            {
                                post = _statuses.Item(args.Ids[i]);
                            }
                            args.Page = i + 1;
                            bw.ReportProgress(50, MakeStatusMessage(args, false));
                            if (!post.IsFav)
                            {
                                if (post.RetweetedId == 0)
                                {
                                    ret = tw.PostFavAdd(post.StatusId);
                                }
                                else
                                {
                                    ret = tw.PostFavAdd(post.RetweetedId);
                                }
                                if (ret.Length == 0)
                                {
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
                                    //リスト再描画必要
                                    _favTimestamps.Add(DateTime.Now);
                                    if (String.IsNullOrEmpty(post.RelTabName))
                                    {
                                        //検索,リストUserTimeline.Relatedタブからのfavは、favタブへ追加せず。それ以外は追加
                                        _statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                    }
                                    else
                                    {
                                        //検索,リスト,UserTimeline.Relatedタブからのfavで、TLでも取得済みならfav反映
                                        if (_statuses.ContainsKey(post.StatusId))
                                        {
                                            PostClass postTl = _statuses.Item(post.StatusId);
                                            postTl.IsFav = true;
                                            _statuses.GetTabByType(Hoehoe.MyCommon.TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, false);
                                        }
                                    }
                                    //検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in _statuses.GetTabsByType(Hoehoe.MyCommon.TabUsageType.PublicSearch | Hoehoe.MyCommon.TabUsageType.Lists | Hoehoe.MyCommon.TabUsageType.UserTimeline | Hoehoe.MyCommon.TabUsageType.Related))
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
                case Hoehoe.MyCommon.WorkerType.FavRemove:
                    //スレッド処理はしない
                    if (_statuses.Tabs.ContainsKey(args.TabName))
                    {
                        TabClass tbc = _statuses.Tabs[args.TabName];
                        for (int i = 0; i < args.Ids.Count; i++)
                        {
                            PostClass post = tbc.IsInnerStorageTabType ? tbc.Posts[args.Ids[i]] : _statuses.Item(args.Ids[i]);
                            args.Page = i + 1;
                            bw.ReportProgress(50, MakeStatusMessage(args, false));
                            if (post.IsFav)
                            {
                                ret = post.RetweetedId == 0 ? tw.PostFavRemove(post.StatusId) : tw.PostFavRemove(post.RetweetedId);
                                if (ret.Length == 0)
                                {
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = false;
                                    //リスト再描画必要
                                    if (_statuses.ContainsKey(post.StatusId))
                                    {
                                        _statuses.Item(post.StatusId).IsFav = false;
                                    }
                                    //検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in _statuses.GetTabsByType(Hoehoe.MyCommon.TabUsageType.PublicSearch | Hoehoe.MyCommon.TabUsageType.Lists | Hoehoe.MyCommon.TabUsageType.UserTimeline | Hoehoe.MyCommon.TabUsageType.Related))
                                    {
                                        if (tb.Contains(post.StatusId))
                                            tb.Posts[post.StatusId].IsFav = false;
                                    }
                                }
                            }
                        }
                    }
                    rslt.SIds = args.SIds;
                    break;
                case Hoehoe.MyCommon.WorkerType.PostMessage:
                    bw.ReportProgress(200);
                    if (String.IsNullOrEmpty(args.PStatus.ImagePath))
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            ret = tw.PostStatus(args.PStatus.Status, args.PStatus.InReplyToId);
                            if (String.IsNullOrEmpty(ret) || ret.StartsWith("OK:") || ret.StartsWith("Outputz:") || ret.StartsWith("Warn:") || ret == "Err:Status is a duplicate." || args.PStatus.Status.StartsWith("D", StringComparison.OrdinalIgnoreCase) || args.PStatus.Status.StartsWith("DM", StringComparison.OrdinalIgnoreCase) || Twitter.AccountState != Hoehoe.MyCommon.AccountState.Valid)
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
                case Hoehoe.MyCommon.WorkerType.Retweet:
                    bw.ReportProgress(200);
                    for (int i = 0; i < args.Ids.Count; i++)
                    {
                        ret = tw.PostRetweet(args.Ids[i], read);
                    }
                    bw.ReportProgress(300);
                    break;
                case Hoehoe.MyCommon.WorkerType.Follower:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText1);
                    ret = tw.GetFollowersApi();
                    if (String.IsNullOrEmpty(ret))
                    {
                        ret = tw.GetNoRetweetIdsApi();
                    }
                    break;
                case Hoehoe.MyCommon.WorkerType.Configuration:
                    ret = tw.ConfigurationApi();
                    break;
                case Hoehoe.MyCommon.WorkerType.OpenUri:
                    string myPath = Convert.ToString(args.Url);

                    try
                    {
                        if (!String.IsNullOrEmpty(SettingDialog.BrowserPath))
                        {
                            if (SettingDialog.BrowserPath.StartsWith("\"") && SettingDialog.BrowserPath.Length > 2 && SettingDialog.BrowserPath.IndexOf("\"", 2) > -1)
                            {
                                int sep = SettingDialog.BrowserPath.IndexOf("\"", 2);
                                string browserPath = SettingDialog.BrowserPath.Substring(1, sep - 1);
                                string arg = "";
                                if (sep < SettingDialog.BrowserPath.Length - 1)
                                {
                                    arg = SettingDialog.BrowserPath.Substring(sep + 1);
                                }
                                myPath = arg + " " + myPath;
                                Process.Start(browserPath, myPath);
                            }
                            else
                            {
                                Process.Start(SettingDialog.BrowserPath, myPath);
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
                case Hoehoe.MyCommon.WorkerType.Favorites:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = tw.GetFavoritesApi(read, args.WorkerType, args.Page == -1);
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.PublicSearch:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    if (String.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in _statuses.GetTabsByType(Hoehoe.MyCommon.TabUsageType.PublicSearch))
                        {
                            if (!String.IsNullOrEmpty(tb.SearchWords))
                            {
                                ret = tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = tw.GetSearch(read, tb, false);
                            if (String.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = tw.GetSearch(read, tb, true);
                            }
                        }
                    }
                    //振り分け
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.UserTimeline:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    int count = 20;
                    if (SettingDialog.UseAdditionalCount)
                        count = SettingDialog.UserTimelineCountApi;
                    if (String.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in _statuses.GetTabsByType(Hoehoe.MyCommon.TabUsageType.UserTimeline))
                        {
                            if (!String.IsNullOrEmpty(tb.User))
                            {
                                ret = tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }
                    //振り分け
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.List:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    if (String.IsNullOrEmpty(args.TabName))
                    {
                        //定期更新
                        foreach (TabClass tb in _statuses.GetTabsByType(Hoehoe.MyCommon.TabUsageType.Lists))
                        {
                            if (tb.ListInfo != null && tb.ListInfo.Id != 0)
                            {
                                ret = tw.GetListStatus(read, tb, false, _initial);
                            }
                        }
                    }
                    else
                    {
                        //手動更新（特定タブのみ更新）
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = tw.GetListStatus(read, tb, args.Page == -1, _initial);
                        }
                    }
                    //振り分け
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.Related:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = tw.GetRelatedResult(read, _statuses.GetTabByName(args.TabName));
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case Hoehoe.MyCommon.WorkerType.BlockIds:
                    bw.ReportProgress(50, Hoehoe.Properties.Resources.UpdateBlockUserText1);
                    ret = tw.GetBlockUserIds();
                    if (TabInformations.GetInstance().BlockIds.Count == 0)
                    {
                        tw.GetBlockUserIds();
                    }
                    break;
            }
            //キャンセル要求
            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //時速表示用
            if (args.WorkerType == Hoehoe.MyCommon.WorkerType.FavAdd)
            {
                System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                for (int i = _favTimestamps.Count - 1; i >= 0; i += -1)
                {
                    if (_favTimestamps[i].CompareTo(oneHour) < 0)
                    {
                        _favTimestamps.RemoveAt(i);
                    }
                }
            }
            if (args.WorkerType == Hoehoe.MyCommon.WorkerType.Timeline && !_initial)
            {
                lock (_syncObject)
                {
                    DateTime tm = DateTime.Now;
                    if (_tlTimestamps.ContainsKey(tm))
                    {
                        _tlTimestamps[tm] += rslt.AddCount;
                    }
                    else
                    {
                        _tlTimestamps.Add(tm, rslt.AddCount);
                    }
                    DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                    List<DateTime> keys = new List<DateTime>();
                    _tlCount = 0;
                    foreach (DateTime key in _tlTimestamps.Keys)
                    {
                        if (key.CompareTo(oneHour) < 0)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            _tlCount += _tlTimestamps[key];
                        }
                    }
                    foreach (DateTime key in keys)
                    {
                        _tlTimestamps.Remove(key);
                    }
                    keys.Clear();
                }
            }

            //終了ステータス
            if (args.WorkerType != Hoehoe.MyCommon.WorkerType.OpenUri)
            {
                bw.ReportProgress(100, MakeStatusMessage(args, true));
            }

            //ステータス書き換え、Notifyアイコンアニメーション開始
            rslt.RetMsg = ret;
            rslt.WorkerType = args.WorkerType;
            rslt.TabName = args.TabName;
            if (args.WorkerType == Hoehoe.MyCommon.WorkerType.DirectMessegeRcv || args.WorkerType == Hoehoe.MyCommon.WorkerType.DirectMessegeSnt || args.WorkerType == Hoehoe.MyCommon.WorkerType.Reply || args.WorkerType == Hoehoe.MyCommon.WorkerType.Timeline || args.WorkerType == Hoehoe.MyCommon.WorkerType.Favorites)
            {
                rslt.Page = args.Page - 1;
                //値が正しいか後でチェック。10ページ毎の継続確認
            }

            e.Result = rslt;
        }

        private string MakeStatusMessage(GetWorkerArg AsyncArg, bool Finish)
        {
            string smsg = "";
            if (!Finish)
            {
                //継続中メッセージ
                switch (AsyncArg.WorkerType)
                {
                    case Hoehoe.MyCommon.WorkerType.Timeline:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case Hoehoe.MyCommon.WorkerType.Reply:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText4 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case Hoehoe.MyCommon.WorkerType.DirectMessegeRcv:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText8 + AsyncArg.Page.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6;
                        break;
                    case Hoehoe.MyCommon.WorkerType.FavAdd:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15 + AsyncArg.Page.ToString() + "/" + AsyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText16 + (AsyncArg.Page - AsyncArg.SIds.Count - 1).ToString();
                        break;
                    case Hoehoe.MyCommon.WorkerType.FavRemove:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17 + AsyncArg.Page.ToString() + "/" + AsyncArg.Ids.Count.ToString() + Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText18 + (AsyncArg.Page - AsyncArg.SIds.Count - 1).ToString();
                        break;
                    case Hoehoe.MyCommon.WorkerType.Favorites:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText19;
                        break;
                    case Hoehoe.MyCommon.WorkerType.PublicSearch:
                        smsg = "Search refreshing...";
                        break;
                    case Hoehoe.MyCommon.WorkerType.List:
                        smsg = "List refreshing...";
                        break;
                    case Hoehoe.MyCommon.WorkerType.Related:
                        smsg = "Related refreshing...";
                        break;
                    case Hoehoe.MyCommon.WorkerType.UserTimeline:
                        smsg = "UserTimeline refreshing...";
                        break;
                }
            }
            else
            {
                //完了メッセージ
                switch (AsyncArg.WorkerType)
                {
                    case Hoehoe.MyCommon.WorkerType.Timeline:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText1;
                        break;
                    case Hoehoe.MyCommon.WorkerType.Reply:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText9;
                        break;
                    case Hoehoe.MyCommon.WorkerType.DirectMessegeRcv:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText11;
                        break;
                    case Hoehoe.MyCommon.WorkerType.DirectMessegeSnt:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText13;
                        break;
                    case Hoehoe.MyCommon.WorkerType.FavAdd:
                        break;
                    //進捗メッセージ残す
                    case Hoehoe.MyCommon.WorkerType.FavRemove:
                        break;
                    //進捗メッセージ残す
                    case Hoehoe.MyCommon.WorkerType.Favorites:
                        smsg = Hoehoe.Properties.Resources.GetTimelineWorker_RunWorkerCompletedText20;
                        break;
                    case Hoehoe.MyCommon.WorkerType.Follower:
                        smsg = Hoehoe.Properties.Resources.UpdateFollowersMenuItem1_ClickText3;
                        break;
                    case Hoehoe.MyCommon.WorkerType.Configuration:
                        break;
                    //進捗メッセージ残す
                    case Hoehoe.MyCommon.WorkerType.PublicSearch:
                        smsg = "Search refreshed";
                        break;
                    case Hoehoe.MyCommon.WorkerType.List:
                        smsg = "List refreshed";
                        break;
                    case Hoehoe.MyCommon.WorkerType.Related:
                        smsg = "Related refreshed";
                        break;
                    case Hoehoe.MyCommon.WorkerType.UserTimeline:
                        smsg = "UserTimeline refreshed";
                        break;
                    case Hoehoe.MyCommon.WorkerType.BlockIds:
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
                //発言投稿
                //開始
                if (e.ProgressPercentage == 200)
                {
                    StatusLabel.Text = "Posting...";
                }
                //終了
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
                //キャンセル
                return;
            }

            if (e.Error != null)
            {
                _myStatusError = true;
                _waitTimeline = false;
                _waitReply = false;
                _waitDm = false;
                _waitFav = false;
                _waitPubSearch = false;
                _waitUserTimeline = false;
                _waitLists = false;
                throw new Exception("BackgroundWorker Exception", e.Error);
            }

            GetWorkerResult rslt = (GetWorkerResult)e.Result;

            if (rslt.WorkerType == Hoehoe.MyCommon.WorkerType.OpenUri)
            {
                return;
            }

            //エラー
            if (rslt.RetMsg.Length > 0)
            {
                _myStatusError = true;
                StatusLabel.Text = rslt.RetMsg;
            }

            if (rslt.WorkerType == Hoehoe.MyCommon.WorkerType.ErrorState)
            {
                return;
            }

            if (rslt.WorkerType == Hoehoe.MyCommon.WorkerType.FavRemove)
            {
                this.RemovePostFromFavTab(rslt.SIds.ToArray());
            }

            if (rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Timeline || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Reply || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.List || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.PublicSearch || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.DirectMessegeRcv || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.DirectMessegeSnt || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Favorites || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Follower || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.FavAdd || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.FavRemove || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Related || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.UserTimeline || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.BlockIds || rslt.WorkerType == Hoehoe.MyCommon.WorkerType.Configuration)
            {
                RefreshTimeline(false);
                //リスト反映
            }

            switch (rslt.WorkerType)
            {
                case Hoehoe.MyCommon.WorkerType.Timeline:
                    _waitTimeline = false;
                    if (!_initial)
                    {
                        //    'API使用時の取得調整は別途考える（カウント調整？）
                    }
                    break;
                case Hoehoe.MyCommon.WorkerType.Reply:
                    _waitReply = false;
                    if (rslt.NewDM && !_initial)
                    {
                        GetTimeline(Hoehoe.MyCommon.WorkerType.DirectMessegeRcv, 1, 0, "");
                    }
                    break;
                case Hoehoe.MyCommon.WorkerType.Favorites:
                    _waitFav = false;
                    break;
                case Hoehoe.MyCommon.WorkerType.DirectMessegeRcv:
                    _waitDm = false;
                    break;
                case Hoehoe.MyCommon.WorkerType.FavAdd:
                case Hoehoe.MyCommon.WorkerType.FavRemove:
                    if (_curList != null && _curTab != null)
                    {
                        _curList.BeginUpdate();
                        if (rslt.WorkerType == Hoehoe.MyCommon.WorkerType.FavRemove && _statuses.Tabs[_curTab.Text].TabType == Hoehoe.MyCommon.TabUsageType.Favorites)
                        {
                            //色変えは不要
                        }
                        else
                        {
                            for (int i = 0; i < rslt.SIds.Count; i++)
                            {
                                if (_curTab.Text.Equals(rslt.TabName))
                                {
                                    int idx = _statuses.Tabs[rslt.TabName].IndexOf(rslt.SIds[i]);
                                    if (idx > -1)
                                    {
                                        TabClass tb = _statuses.Tabs[rslt.TabName];
                                        if (tb != null)
                                        {
                                            PostClass post = null;
                                            if (tb.TabType == MyCommon.TabUsageType.Lists || tb.TabType == MyCommon.TabUsageType.PublicSearch)
                                            {
                                                post = tb.Posts[rslt.SIds[i]];
                                            }
                                            else
                                            {
                                                post = _statuses.Item(rslt.SIds[i]);
                                            }
                                            ChangeCacheStyleRead(post.IsRead, idx, _curTab);
                                        }
                                        if (idx == _curItemIndex)
                                        {
                                            //選択アイテム再表示
                                            DispSelectedPost(true);
                                        }
                                    }
                                }
                            }
                        }
                        _curList.EndUpdate();
                    }
                    break;
                case MyCommon.WorkerType.PostMessage:
                    if (String.IsNullOrEmpty(rslt.RetMsg) || rslt.RetMsg.StartsWith("Outputz") || rslt.RetMsg.StartsWith("OK:") || rslt.RetMsg == "Warn:Status is a duplicate.")
                    {
                        _postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = _postTimestamps.Count - 1; i >= 0; i += -1)
                        {
                            if (_postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                _postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!HashMgr.IsPermanent && !String.IsNullOrEmpty(HashMgr.UseHash))
                        {
                            HashMgr.ClearHashtag();
                            this.HashStripSplitButton.Text = "#[-]";
                            this.HashToggleMenuItem.Checked = false;
                            this.HashToggleToolStripMenuItem.Checked = false;
                        }
                        SetMainWindowTitle();
                        rslt.RetMsg = "";
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
                            RunAsync(new GetWorkerArg() { Page = 0, EndPage = 0, WorkerType = MyCommon.WorkerType.PostMessage, PStatus = rslt.PStatus });
                        }
                        else
                        {
                            if (ToolStripFocusLockMenuItem.Checked)
                            {
                                //連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                                StatusText_Enter(StatusText, new EventArgs());
                            }
                        }
                    }
                    if (rslt.RetMsg.Length == 0 && SettingDialog.PostAndGet)
                    {
                        if (_isActiveUserstream)
                        {
                            RefreshTimeline(true);
                        }
                        else
                        {
                            GetTimeline(MyCommon.WorkerType.Timeline, 1, 0, "");
                        }
                    }
                    break;
                case MyCommon.WorkerType.Retweet:
                    if (rslt.RetMsg.Length == 0)
                    {
                        _postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = _postTimestamps.Count - 1; i >= 0; i--)
                        {
                            if (_postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                _postTimestamps.RemoveAt(i);
                            }
                        }
                        if (!_isActiveUserstream && SettingDialog.PostAndGet)
                        {
                            GetTimeline(MyCommon.WorkerType.Timeline, 1, 0, "");
                        }
                    }
                    break;
                case MyCommon.WorkerType.Follower:
                    _itemCache = null;
                    _postCache = null;
                    if (_curList != null)
                    {
                        _curList.Refresh();
                    }
                    break;
                case MyCommon.WorkerType.Configuration:
                    //_waitFollower = False
                    if (SettingDialog.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        _pictureServices["Twitter"].Configuration("MaxUploadFilesize", SettingDialog.TwitterConfiguration.PhotoSizeLimit);
                    }
                    _itemCache = null;
                    _postCache = null;
                    if (_curList != null)
                    {
                        _curList.Refresh();
                    }
                    break;
                case MyCommon.WorkerType.PublicSearch:
                    _waitPubSearch = false;
                    break;
                case MyCommon.WorkerType.UserTimeline:
                    _waitUserTimeline = false;
                    break;
                case MyCommon.WorkerType.List:
                    _waitLists = false;
                    break;
                case MyCommon.WorkerType.Related:
                    {
                        TabClass tb = _statuses.GetTabByType(MyCommon.TabUsageType.Related);
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

        private void RemovePostFromFavTab(Int64[] ids)
        {
            string favTabName = _statuses.GetTabByType(MyCommon.TabUsageType.Favorites).TabName;
            int fidx = 0;
            if (_curTab.Text.Equals(favTabName))
            {
                if (_curList.FocusedItem != null)
                {
                    fidx = _curList.FocusedItem.Index;
                }
                else if (_curList.TopItem != null)
                {
                    fidx = _curList.TopItem.Index;
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
                    _statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (_curTab != null && _curTab.Text.Equals(favTabName))
            {
                _itemCache = null;
                //キャッシュ破棄
                _postCache = null;
                _curPost = null;
                //_curItemIndex = -1
            }
            foreach (TabPage tp in ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = _statuses.Tabs[favTabName].AllCount;
                    break;
                }
            }
            if (_curTab.Text.Equals(favTabName))
            {
                do
                {
                    _curList.SelectedIndices.Clear();
                } while (_curList.SelectedIndices.Count > 0);
                if (_statuses.Tabs[favTabName].AllCount > 0)
                {
                    if (_statuses.Tabs[favTabName].AllCount - 1 > fidx && fidx > -1)
                    {
                        _curList.SelectedIndices.Add(fidx);
                    }
                    else
                    {
                        _curList.SelectedIndices.Add(_statuses.Tabs[favTabName].AllCount - 1);
                    }
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        _curList.EnsureVisible(_curList.SelectedIndices[0]);
                        _curList.FocusedItem = _curList.Items[_curList.SelectedIndices[0]];
                    }
                }
            }
        }

        Dictionary<Hoehoe.MyCommon.WorkerType, DateTime> _lastTime;

        private void GetTimeline(MyCommon.WorkerType wkType, int fromPage, int toPage, string tabName)
        {
            if (!MyCommon.IsNetworkAvailable())
            {
                return;
            }

            if (_lastTime == null)
            {
                _lastTime = new Dictionary<MyCommon.WorkerType, DateTime>();
            }

            //非同期実行引数設定
            if (!_lastTime.ContainsKey(wkType))
            {
                _lastTime.Add(wkType, new DateTime());
            }
            double period = DateTime.Now.Subtract(_lastTime[wkType]).TotalSeconds;
            if (period > 1 || period < -1)
            {
                _lastTime[wkType] = DateTime.Now;
                RunAsync(new GetWorkerArg() { Page = fromPage, EndPage = toPage, WorkerType = wkType, TabName = tabName });
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
            switch (SettingDialog.ListDoubleClickAction)
            {
                case 0:
                    MakeReplyOrDirectStatus();
                    break;
                case 1:
                    FavoriteChange(true);
                    break;
                case 2:
                    if (_curPost != null)
                    {
                        ShowUserStatus(_curPost.ScreenName, false);
                    }
                    break;
                case 3:
                    ShowUserTimeline();
                    break;
                case 4:
                    ShowRelatedStatusesMenuItem_Click(null, null);
                    break;
                case 5:
                    MoveToHomeToolStripMenuItem_Click(null, null);
                    break;
                case 6:
                    StatusOpenMenuItem_Click(null, null);
                    break;
                case 7:
                    break;
                //動作なし
            }
        }

        private void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteChange(true);
        }

        private void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FavoriteChange(false);
        }

        private void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            FavoritesRetweetOriginal();
        }

        private void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            FavoritesRetweetUnofficial();
        }

        private void FavoriteChange(bool isFavAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            //TrueでFavAdd,FalseでFavRemove
            if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage || _curList.SelectedIndices.Count == 0 || !this.ExistCurrentPost)
            {
                return;
            }

            //複数fav確認msg
            if (_curList.SelectedIndices.Count > 250 && isFavAdd)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.FavoriteLimitCountText);
                _DoFavRetweetFlags = false;
                return;
            }
            if (multiFavoriteChangeDialogEnable && _curList.SelectedIndices.Count > 1)
            {
                if (isFavAdd)
                {
                    string QuestionText = Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText1;
                    if (_DoFavRetweetFlags)
                    {
                        QuestionText = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText3;
                    }
                    if (MessageBox.Show(QuestionText, Hoehoe.Properties.Resources.FavAddToolStripMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        _DoFavRetweetFlags = false;
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
                TabName = _curTab.Text,
                WorkerType = isFavAdd ? MyCommon.WorkerType.FavAdd : MyCommon.WorkerType.FavRemove
            };
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = GetCurTabPost(idx);
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

            RunAsync(args);
        }

        private PostClass GetCurTabPost(int index)
        {
            if (_postCache != null && index >= _itemCacheIndex && index < _itemCacheIndex + _postCache.Length)
            {
                return _postCache[index - _itemCacheIndex];
            }
            else
            {
                return _statuses.Item(_curTab.Text, index);
            }
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName);
            }
            else if (_curList.SelectedIndices.Count == 0)
            {
                OpenUriAsync("http://twitter.com/");
            }
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName + "/favorites");
            }
        }

        private void Tween_ClientSizeChanged(object sender, EventArgs e)
        {
            if ((!_initialLayout) && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    _mySize = this.ClientSize;
                    _mySpDis = this.SplitContainer1.SplitterDistance;
                    _mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (StatusText.Multiline)
                    {
                        _mySpDis2 = this.StatusText.Height;
                    }
                    _myAdSpDis = this.SplitContainer4.SplitterDistance;
                    _modifySettingLocal = true;
                }
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (SettingDialog.SortOrderLock)
            {
                return;
            }
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            if (_iconCol)
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
                        //0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                        //ソートしない
                        return;
                    case 1:
                        //ニックネーム
                        mode = IdComparerClass.ComparerMode.Nickname;
                        break;
                    case 2:
                        //本文
                        mode = IdComparerClass.ComparerMode.Data;
                        break;
                    case 3:
                        //時刻=発言Id
                        mode = IdComparerClass.ComparerMode.Id;
                        break;
                    case 4:
                        //名前
                        mode = IdComparerClass.ComparerMode.Name;
                        break;
                    case 7:
                        //Source
                        mode = IdComparerClass.ComparerMode.Source;
                        break;
                }
            }
            _statuses.ToggleSortOrder(mode);
            InitColumnText();

            if (_iconCol)
            {
                ((DetailsListView)sender).Columns[0].Text = _columnOrgTexts[0];
                ((DetailsListView)sender).Columns[1].Text = _columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    ((DetailsListView)sender).Columns[i].Text = _columnOrgTexts[i];
                }
                ((DetailsListView)sender).Columns[e.Column].Text = _columnTexts[e.Column];
            }

            _itemCache = null;
            _postCache = null;

            if (_statuses.Tabs[_curTab.Text].AllCount > 0 && _curPost != null)
            {
                int idx = _statuses.Tabs[_curTab.Text].IndexOf(_curPost.StatusId);
                if (idx > -1)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                }
            }
            _curList.Refresh();
            _modifySettingCommon = true;
        }

        private void Tween_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myLoc = this.DesktopLocation;
                _modifySettingLocal = true;
            }
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            if (ListTab.SelectedTab == null)
            {
                return;
            }
            if (_statuses == null || _statuses.Tabs == null || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
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
            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || _curPost.IsDm)
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
                if (this.ExistCurrentPost && _curPost.IsDm)
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
                //PublicSearchの時問題出るかも

                if (_curPost.IsMe)
                {
                    ReTweetOriginalStripMenuItem.Enabled = false;
                    FavoriteRetweetContextMenu.Enabled = false;
                    if (String.IsNullOrEmpty(_curPost.RetweetedBy))
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
                    if (String.IsNullOrEmpty(_curPost.RetweetedBy))
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText1;
                    }
                    else
                    {
                        DeleteStripMenuItem.Text = Hoehoe.Properties.Resources.DeleteMenuText2;
                    }
                    DeleteStripMenuItem.Enabled = false;
                    if (_curPost.IsProtect)
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
            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch || !this.ExistCurrentPost || !(_curPost.InReplyToStatusId > 0))
            {
                RepliedStatusOpenMenuItem.Enabled = false;
            }
            else
            {
                RepliedStatusOpenMenuItem.Enabled = true;
            }
            if (!this.ExistCurrentPost || String.IsNullOrEmpty(_curPost.RetweetedBy))
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
            MakeReplyOrDirectStatus(false, true);
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, false);
        }

        private void doStatusDelete()
        {
            if (_curTab == null || _curList == null)
            {
                return;
            }
            if (_statuses.Tabs[_curTab.Text].TabType != MyCommon.TabUsageType.DirectMessage)
            {
                bool myPost = false;
                foreach (int idx in _curList.SelectedIndices)
                {
                    if (GetCurTabPost(idx).IsMe || GetCurTabPost(idx).RetweetedBy.ToLower() == tw.Username.ToLower())
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
                if (_curList.SelectedIndices.Count == 0)
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
            if (_curList.FocusedItem != null)
            {
                fidx = _curList.FocusedItem.Index;
            }
            else if (_curList.TopItem != null)
            {
                fidx = _curList.TopItem.Index;
            }
            else
            {
                fidx = 0;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                bool rslt = true;
                foreach (long id in _statuses.GetId(_curTab.Text, _curList.SelectedIndices))
                {
                    string rtn = "";
                    if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage)
                    {
                        rtn = tw.RemoveDirectMessage(id, _statuses.Item(id));
                    }
                    else
                    {
                        if (_statuses.Item(id).IsMe || _statuses.Item(id).RetweetedBy.ToLower() == tw.Username.ToLower())
                        {
                            rtn = tw.RemoveStatus(id);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (rtn.Length > 0)
                    {
                        //エラー
                        rslt = false;
                    }
                    else
                    {
                        _statuses.RemovePost(id);
                    }
                }

                if (rslt)
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText4;
                    //成功
                }
                else
                {
                    StatusLabel.Text = Hoehoe.Properties.Resources.DeleteStripMenuItem_ClickText3;
                    //失敗
                }

                _itemCache = null;
                //キャッシュ破棄
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                    if (_curTab.Equals(tb))
                    {
                        do
                        {
                            _curList.SelectedIndices.Clear();
                        } while (_curList.SelectedIndices.Count > 0);
                        if (_statuses.Tabs[tb.Text].AllCount > 0)
                        {
                            if (_statuses.Tabs[tb.Text].AllCount - 1 > fidx && fidx > -1)
                            {
                                _curList.SelectedIndices.Add(fidx);
                            }
                            else
                            {
                                _curList.SelectedIndices.Add(_statuses.Tabs[tb.Text].AllCount - 1);
                            }
                            if (_curList.SelectedIndices.Count > 0)
                            {
                                _curList.EnsureVisible(_curList.SelectedIndices[0]);
                                _curList.FocusedItem = _curList.Items[_curList.SelectedIndices[0]];
                            }
                        }
                    }
                    if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            if (tb.ImageIndex == 0)
                            {
                                //タブアイコン
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }
                if (!SettingDialog.TabIconDisp)
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
            doStatusDelete();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            _curList.BeginUpdate();
            if (SettingDialog.UnreadManage)
            {
                foreach (int idx in _curList.SelectedIndices)
                {
                    _statuses.SetReadAllTab(true, _curTab.Text, idx);
                }
            }
            foreach (int idx in _curList.SelectedIndices)
            {
                ChangeCacheStyleRead(true, idx, _curTab);
            }
            ColorizeList();
            _curList.EndUpdate();
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                            //タブアイコン
                        }
                    }
                }
            }
            if (!SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            _curList.BeginUpdate();
            if (SettingDialog.UnreadManage)
            {
                foreach (int idx in _curList.SelectedIndices)
                {
                    _statuses.SetReadAllTab(false, _curTab.Text, idx);
                }
            }
            foreach (int idx in _curList.SelectedIndices)
            {
                ChangeCacheStyleRead(false, idx, _curTab);
            }
            ColorizeList();
            _curList.EndUpdate();
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == -1)
                        {
                            tb.ImageIndex = 0;
                            //タブアイコン
                        }
                    }
                }
            }
            if (!SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            DoRefresh();
        }

        private void DoRefresh()
        {
            if (_curTab != null)
            {
                switch (_statuses.Tabs[_curTab.Text].TabType)
                {
                    case MyCommon.TabUsageType.Mentions:
                        GetTimeline(MyCommon.WorkerType.Reply, 1, 0, "");
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        GetTimeline(MyCommon.WorkerType.DirectMessegeRcv, 1, 0, "");
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        GetTimeline(MyCommon.WorkerType.Favorites, 1, 0, "");
                        break;
                    //Case TabUsageType.Profile
                    //' TODO
                    case MyCommon.TabUsageType.PublicSearch:
                        {
                            //' TODO
                            TabClass tb = _statuses.Tabs[_curTab.Text];
                            if (String.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }
                            GetTimeline(MyCommon.WorkerType.PublicSearch, 1, 0, _curTab.Text);
                        }
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        GetTimeline(MyCommon.WorkerType.UserTimeline, 1, 0, _curTab.Text);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        {
                            //' TODO
                            TabClass tb = _statuses.Tabs[_curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }

                            GetTimeline(MyCommon.WorkerType.List, 1, 0, _curTab.Text);
                        }
                        break;
                    default:
                        GetTimeline(MyCommon.WorkerType.Timeline, 1, 0, "");
                        break;
                }
            }
            else
            {
                GetTimeline(MyCommon.WorkerType.Timeline, 1, 0, "");
            }
        }

        private void DoRefreshMore()
        {
            //ページ指定をマイナス1に
            if (_curTab != null)
            {
                switch (_statuses.Tabs[_curTab.Text].TabType)
                {
                    case MyCommon.TabUsageType.Mentions:
                        GetTimeline(MyCommon.WorkerType.Reply, -1, 0, "");
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        GetTimeline(MyCommon.WorkerType.DirectMessegeRcv, -1, 0, "");
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        GetTimeline(MyCommon.WorkerType.Favorites, -1, 0, "");
                        break;
                    case MyCommon.TabUsageType.Profile:
                        break;
                    //' TODO
                    case MyCommon.TabUsageType.PublicSearch:
                        {
                            // TODO
                            TabClass tb = _statuses.Tabs[_curTab.Text];
                            if (String.IsNullOrEmpty(tb.SearchWords))
                            {
                                return;
                            }
                            GetTimeline(MyCommon.WorkerType.PublicSearch, -1, 0, _curTab.Text);
                        }
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        GetTimeline(MyCommon.WorkerType.UserTimeline, -1, 0, _curTab.Text);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        {
                            //' TODO
                            TabClass tb = _statuses.Tabs[_curTab.Text];
                            if (tb.ListInfo == null || tb.ListInfo.Id == 0)
                            {
                                return;
                            }
                            GetTimeline(MyCommon.WorkerType.List, -1, 0, _curTab.Text);
                        }
                        break;
                    default:
                        GetTimeline(MyCommon.WorkerType.Timeline, -1, 0, "");
                        break;
                }
            }
            else
            {
                GetTimeline(MyCommon.WorkerType.Timeline, -1, 0, "");
            }
        }

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = default(DialogResult);
            string uid = tw.Username.ToLower();
            foreach (UserAccount u in SettingDialog.UserAccounts)
            {
                if (u.UserId == tw.UserId)
                {
                    break;
                }
            }

            try
            {
                result = SettingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return;
            }

            if (result == DialogResult.OK)
            {
                lock (_syncObject)
                {
                    tw.SetTinyUrlResolve(SettingDialog.TinyUrlResolve);
                    tw.SetRestrictFavCheck(SettingDialog.RestrictFavCheck);
                    tw.ReadOwnPost = SettingDialog.ReadOwnPost;
                    tw.SetUseSsl(SettingDialog.UseSsl);
                    ShortUrl.IsResolve = SettingDialog.TinyUrlResolve;
                    ShortUrl.IsForceResolve = SettingDialog.ShortUrlForceResolve;
                    ShortUrl.SetBitlyId(SettingDialog.BitlyUser);
                    ShortUrl.SetBitlyKey(SettingDialog.BitlyPwd);
                    HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
                    HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);

                    HttpConnection.InitializeConnection(SettingDialog.DefaultTimeOut, SettingDialog.SelectedProxyType, SettingDialog.ProxyAddress, SettingDialog.ProxyPort, SettingDialog.ProxyUser, SettingDialog.ProxyPassword);
                    this.CreatePictureServices();
#if UA //= "True"
					this.SplitContainer4.Panel2.Controls.RemoveAt(0);
					this.ab = new AdsBrowser();
					this.SplitContainer4.Panel2.Controls.Add(ab);
#endif
                    try
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            ListTab.DrawItem -= ListTab_DrawItem;
                            ListTab.DrawMode = TabDrawMode.Normal;
                            ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            ListTab.DrawItem -= ListTab_DrawItem;
                            ListTab.DrawItem += ListTab_DrawItem;
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
                        if (!SettingDialog.UnreadManage)
                        {
                            ReadedStripMenuItem.Enabled = false;
                            UnreadStripMenuItem.Enabled = false;
                            if (SettingDialog.TabIconDisp)
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
                            lst.GridLines = SettingDialog.ShowGrid;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(ShowGrid)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    PlaySoundMenuItem.Checked = SettingDialog.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound;
                    _fntUnread = SettingDialog.FontUnread;
                    _clUnread = SettingDialog.ColorUnread;
                    _fntReaded = SettingDialog.FontReaded;
                    _clReaded = SettingDialog.ColorReaded;
                    _clFav = SettingDialog.ColorFav;
                    _clOWL = SettingDialog.ColorOWL;
                    _clRetweet = SettingDialog.ColorRetweet;
                    _fntDetail = SettingDialog.FontDetail;
                    _clDetail = SettingDialog.ColorDetail;
                    _clDetailLink = SettingDialog.ColorDetailLink;
                    _clDetailBackcolor = SettingDialog.ColorDetailBackcolor;
                    _clSelf = SettingDialog.ColorSelf;
                    _clAtSelf = SettingDialog.ColorAtSelf;
                    _clTarget = SettingDialog.ColorTarget;
                    _clAtTarget = SettingDialog.ColorAtTarget;
                    _clAtFromTarget = SettingDialog.ColorAtFromTarget;
                    _clAtTo = SettingDialog.ColorAtTo;
                    _clListBackcolor = SettingDialog.ColorListBackcolor;
                    _clInputBackcolor = SettingDialog.ColorInputBackcolor;
                    _clInputFont = SettingDialog.ColorInputFont;
                    _fntInputFont = SettingDialog.FontInputFont;
                    try
                    {
                        if (StatusText.Focused)
                        {
                            StatusText.BackColor = _clInputBackcolor;
                        }
                        StatusText.Font = _fntInputFont;
                        StatusText.ForeColor = _clInputFont;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    _brsForeColorUnread.Dispose();
                    _brsForeColorReaded.Dispose();
                    _brsForeColorFav.Dispose();
                    _brsForeColorOWL.Dispose();
                    _brsForeColorRetweet.Dispose();
                    _brsForeColorUnread = new SolidBrush(_clUnread);
                    _brsForeColorReaded = new SolidBrush(_clReaded);
                    _brsForeColorFav = new SolidBrush(_clFav);
                    _brsForeColorOWL = new SolidBrush(_clOWL);
                    _brsForeColorRetweet = new SolidBrush(_clRetweet);
                    _brsBackColorMine.Dispose();
                    _brsBackColorAt.Dispose();
                    _brsBackColorYou.Dispose();
                    _brsBackColorAtYou.Dispose();
                    _brsBackColorAtFromTarget.Dispose();
                    _brsBackColorAtTo.Dispose();
                    _brsBackColorNone.Dispose();
                    _brsBackColorMine = new SolidBrush(_clSelf);
                    _brsBackColorAt = new SolidBrush(_clAtSelf);
                    _brsBackColorYou = new SolidBrush(_clTarget);
                    _brsBackColorAtYou = new SolidBrush(_clAtTarget);
                    _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
                    _brsBackColorAtTo = new SolidBrush(_clAtTo);
                    _brsBackColorNone = new SolidBrush(_clListBackcolor);
                    try
                    {
                        if (SettingDialog.IsMonospace)
                        {
                            _detailHtmlFormatHeader = detailHtmlFormatMono1;
                            _detailHtmlFormatFooter = detailHtmlFormatMono7;
                        }
                        else
                        {
                            _detailHtmlFormatHeader = detailHtmlFormat1;
                            _detailHtmlFormatFooter = detailHtmlFormat7;
                        }
                        _detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString() + "," + _clDetail.G.ToString() + "," + _clDetail.B.ToString() + detailHtmlFormat4 + _clDetailLink.R.ToString() + "," + _clDetailLink.G.ToString() + "," + _clDetailLink.B.ToString() + detailHtmlFormat5 + _clDetailBackcolor.R.ToString() + "," + _clDetailBackcolor.G.ToString() + "," + _clDetailBackcolor.B.ToString();
                        if (SettingDialog.IsMonospace)
                        {
                            _detailHtmlFormatHeader += detailHtmlFormatMono6;
                        }
                        else
                        {
                            _detailHtmlFormatHeader += detailHtmlFormat6;
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
                        _statuses.SetUnreadManage(SettingDialog.UnreadManage);
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
                            if (SettingDialog.TabIconDisp)
                            {
                                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
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
                                ((DetailsListView)tb.Tag).Font = _fntReaded;
                                ((DetailsListView)tb.Tag).BackColor = _clListBackcolor;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp no2)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }
                    SetMainWindowTitle();
                    SetNotifyIconText();

                    _itemCache = null;
                    _postCache = null;
                    if (_curList != null)
                    {
                        _curList.Refresh();
                    }
                    ListTab.Refresh();

                    Outputz.Key = SettingDialog.OutputzKey;
                    Outputz.Enabled = SettingDialog.OutputzEnabled;
                    switch (SettingDialog.OutputzUrlmode)
                    {
                        case MyCommon.OutputzUrlmode.twittercom:
                            Outputz.OutUrl = "http://twitter.com/";
                            break;
                        case MyCommon.OutputzUrlmode.twittercomWithUsername:
                            Outputz.OutUrl = "http://twitter.com/" + tw.Username;
                            break;
                    }

                    _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (SettingDialog.HotkeyEnabled)
                    {
                        ///グローバルホットキーの登録。設定で変更可能にするかも
                        HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                        if ((SettingDialog.HotkeyMod & Keys.Alt) == Keys.Alt)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                        }
                        if ((SettingDialog.HotkeyMod & Keys.Control) == Keys.Control)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                        }
                        if ((SettingDialog.HotkeyMod & Keys.Shift) == Keys.Shift)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                        }
                        if ((SettingDialog.HotkeyMod & Keys.LWin) == Keys.LWin)
                        {
                            modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                        }

                        _hookGlobalHotkey.RegisterOriginalHotkey(SettingDialog.HotkeyKey, SettingDialog.HotkeyValue, modKey);
                    }

                    if (uid != tw.Username)
                    {
                        this.doGetFollowersMenu();
                    }

                    SetImageServiceCombo();
                    if (SettingDialog.IsNotifyUseGrowl)
                    {
                        gh.RegisterGrowl();
                    }
                    try
                    {
                        StatusText_TextChanged(null, null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Twitter.AccountState = MyCommon.AccountState.Valid;

            this.TopMost = SettingDialog.AlwaysTop;
            SaveConfigsAll(false);
        }

        private void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                DispSelectedPost();
                OpenUriAsync(e.Url.OriginalString);
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

                if (e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") || e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23"))
                {
                    //ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(e.Url.AbsoluteUri);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    HashSupl.AddItem(hash);
                    HashMgr.AddHashToHistory(hash.Trim(), false);
                    AddNewTabForSearch(hash);
                    return;
                }
                else
                {
                    Match m = Regex.Match(e.Url.AbsoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (m.Success && IsTwitterId(m.Result("${ScreenName}")))
                    {
                        // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
                        if (SettingDialog.OpenUserTimeline)
                        {
                            if (isKeyDown(Keys.Control))
                            {
                                OpenUriAsync(e.Url.OriginalString);
                            }
                            else
                            {
                                this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                            }
                        }
                        else
                        {
                            if (isKeyDown(Keys.Control))
                            {
                                this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                            }
                            else
                            {
                                OpenUriAsync(e.Url.OriginalString);
                            }
                        }
                    }
                    else
                    {
                        OpenUriAsync(e.Url.OriginalString);
                    }
                }
            }
        }

        public void AddNewTabForSearch(string searchWord)
        {
            //同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.PublicSearch))
            {
                if (tb.SearchWords == searchWord && String.IsNullOrEmpty(tb.SearchLang))
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
            //ユニークなタブ名生成
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
            //タブ追加
            _statuses.AddTab(tabName, MyCommon.TabUsageType.PublicSearch, null);
            AddNewTab(tabName, false, MyCommon.TabUsageType.PublicSearch);
            //追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            //検索条件の設定
            ComboBox cmb = (ComboBox)ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            SaveConfigsTabs();
            //検索実行
            this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
        }

        private void ShowUserTimeline()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }
            AddNewTabForUserTimeline(_curPost.ScreenName);
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                TabPage relTp = ListTab.SelectedTab;
                RemoveSpecifiedTab(relTp.Text, false);
                SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        public void AddNewTabForUserTimeline(string user)
        {
            //同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.UserTimeline))
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
            //ユニークなタブ名生成
            string tabName = "user:" + user;
            while (_statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }
            //タブ追加
            _statuses.AddTab(tabName, MyCommon.TabUsageType.UserTimeline, null);
            _statuses.Tabs[tabName].User = user;
            AddNewTab(tabName, false, MyCommon.TabUsageType.UserTimeline);
            //追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            SaveConfigsTabs();
            //検索実行
            GetTimeline(MyCommon.WorkerType.UserTimeline, 1, 0, tabName);
        }

        public bool AddNewTab(string tabName, bool startup, Hoehoe.MyCommon.TabUsageType tabType, ListElement listInfo = null)
        {
            //重複チェック
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    return false;
                }
            }

            //新規タブ名チェック
            if (tabName == Hoehoe.Properties.Resources.AddNewTabText1)
            {
                return false;
            }

            //タブタイプ重複チェック
            if (!startup)
            {
                if (tabType == MyCommon.TabUsageType.DirectMessage || tabType == MyCommon.TabUsageType.Favorites || tabType == MyCommon.TabUsageType.Home || tabType == MyCommon.TabUsageType.Mentions || tabType == MyCommon.TabUsageType.Related)
                {
                    if (_statuses.GetTabByType(tabType) != null)
                    {
                        return false;
                    }
                }
            }

            TabPage tabPage = new TabPage();
            DetailsListView listCustom = new DetailsListView();
            ColumnHeader colHd1 = new ColumnHeader();            //アイコン
            ColumnHeader colHd2 = new ColumnHeader();            //ニックネーム
            ColumnHeader colHd3 = new ColumnHeader();            //本文
            ColumnHeader colHd4 = new ColumnHeader();            //日付
            ColumnHeader colHd5 = new ColumnHeader();            //ユーザID
            ColumnHeader colHd6 = new ColumnHeader();            //未読
            ColumnHeader colHd7 = new ColumnHeader();            //マーク＆プロテクト
            ColumnHeader colHd8 = new ColumnHeader();            //ソース

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
            if (tabType == MyCommon.TabUsageType.UserTimeline || tabType == MyCommon.TabUsageType.Lists)
            {
                label = new Label();
                label.Dock = DockStyle.Top;
                label.Name = "labelUser";
                label.Text = tabType == MyCommon.TabUsageType.Lists ? listInfo.ToString() : _statuses.Tabs[tabName].User + "'s Timeline";
                label.TextAlign = ContentAlignment.MiddleLeft;
                using (ComboBox tmpComboBox = new ComboBox())
                {
                    label.Height = tmpComboBox.Height;
                }
                tabPage.Controls.Add(label);
            }

            /// 検索関連の準備
            Panel pnl = null;
            if (tabType == MyCommon.TabUsageType.PublicSearch)
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
                pnl.Enter += SearchControls_Enter;
                pnl.Leave += SearchControls_Leave;

                cmb.Text = "";
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

                cmbLang.Text = "";
                cmbLang.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                cmbLang.Dock = DockStyle.Right;
                cmbLang.Width = 50;
                cmbLang.Name = "comboLang";
                cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbLang.TabStop = false;
                cmbLang.Items.Add("");
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
            }

            this.ListTab.Controls.Add(tabPage);
            tabPage.Controls.Add(listCustom);

            if (tabType == MyCommon.TabUsageType.PublicSearch)
            {
                tabPage.Controls.Add(pnl);
            }
            if (tabType == MyCommon.TabUsageType.UserTimeline || tabType == MyCommon.TabUsageType.Lists)
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
            if (!_iconCol)
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
            listCustom.Font = _fntReaded;
            listCustom.BackColor = _clListBackcolor;
            listCustom.GridLines = SettingDialog.ShowGrid;
            listCustom.AllowDrop = true;
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

            InitColumnText();
            colHd1.Text = _columnTexts[0];
            colHd1.Width = 48;
            colHd2.Text = _columnTexts[1];
            colHd2.Width = 80;
            colHd3.Text = _columnTexts[2];
            colHd3.Width = 300;
            colHd4.Text = _columnTexts[3];
            colHd4.Width = 50;
            colHd5.Text = _columnTexts[4];
            colHd5.Width = 50;
            colHd6.Text = _columnTexts[5];
            colHd6.Width = 16;
            colHd7.Text = _columnTexts[6];
            colHd7.Width = 16;
            colHd8.Text = _columnTexts[7];
            colHd8.Width = 50;

            if (_statuses.IsDistributableTab(tabName))
            {
                TabDialog.AddTab(tabName);
            }

            listCustom.SmallImageList = new ImageList();
            if (_iconSz > 0)
            {
                listCustom.SmallImageList.ImageSize = new Size(_iconSz, _iconSz);
            }
            else
            {
                listCustom.SmallImageList.ImageSize = new Size(1, 1);
            }

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
                for (int i = 0; i <= _curList.Columns.Count - 1; i++)
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
                    for (int i = 0; i <= 7; i++)
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
                    for (int i = 0; i <= 7; i++)
                    {
                        listCustom.Columns[dispOrder[i]].DisplayIndex = i;
                    }
                }
            }

            if (tabType == MyCommon.TabUsageType.PublicSearch)
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

            if (_statuses.IsDefaultTab(TabName))
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

            SetListProperty();
            //他のタブに列幅等を反映

            MyCommon.TabUsageType tabType = _statuses.Tabs[TabName].TabType;

            //オブジェクトインスタンスの削除
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
            if (tabType == MyCommon.TabUsageType.PublicSearch)
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

            TabDialog.RemoveTab(TabName);

            listCustom.SmallImageList = null;
            listCustom.ListViewItemSorter = null;

            //キャッシュのクリア
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

            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.ResumeLayout(false);
            this.ListTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            tabPage.Dispose();
            listCustom.Dispose();
            _statuses.RemoveTab(TabName);

            foreach (TabPage tp in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tp.Tag;
                if (lst.VirtualListSize != _statuses.Tabs[tp.Text].AllCount)
                {
                    lst.VirtualListSize = _statuses.Tabs[tp.Text].AllCount;
                }
            }

            return true;
        }

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            _itemCache = null;
            _itemCacheIndex = -1;
            _postCache = null;
            _beforeSelectedTab = e.TabPage;
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            //タブのD&D

            if (!SettingDialog.TabMouseLock && e.Button == MouseButtons.Left && _tabDrag)
            {
                string tn = "";
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(_tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(_tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    //タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = ListTab.SelectedTab.Text;
                }

                if (String.IsNullOrEmpty(tn))
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
                _tabDrag = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Left <= cpos.X & cpos.X <= rect.Right & rect.Top <= cpos.Y & cpos.Y <= rect.Bottom)
                {
                    _rclickTabName = ListTab.TabPages[i].Text;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        private void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_curList.Refresh()
            DispSelectedPost();
            SetMainWindowTitle();
            SetStatusLabelUrl();
            if (ListTab.Focused || ((Control)ListTab.SelectedTab.Tag).Focused)
            {
                this.Tag = ListTab.Tag;
            }
            TabMenuControl(ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
        }

        private void SetListProperty()
        {
            //削除などで見つからない場合は処理せず
            if (_curList == null)
            {
                return;
            }
            if (!_isColumnChanged)
            {
                return;
            }

            int[] dispOrder = new int[_curList.Columns.Count];
            for (int i = 0; i < _curList.Columns.Count; i++)
            {
                for (int j = 0; j < _curList.Columns.Count; j++)
                {
                    if (_curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            //列幅、列並びを他のタブに設定
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (!tb.Equals(_curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i <= lst.Columns.Count - 1; i++)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = _curList.Columns[i].Width;
                        }
                    }
                }
            }

            _isColumnChanged = false;
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (PostBrowser.StatusText.StartsWith("http") || PostBrowser.StatusText.StartsWith("ftp") || PostBrowser.StatusText.StartsWith("data"))
                {
                    StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
                }
                if (String.IsNullOrEmpty(PostBrowser.StatusText))
                {
                    SetStatusLabelUrl();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                if (!SettingDialog.UseAtIdSupplement)
                {
                    return;
                }
                //@マーク
                int cnt = AtIdSupl.ItemCount;
                ShowSuplDialog(StatusText, AtIdSupl);
                if (cnt != AtIdSupl.ItemCount)
                {
                    _modifySettingAtId = true;
                }
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!SettingDialog.UseHashSupplement)
                {
                    return;
                }
                ShowSuplDialog(StatusText, HashSupl);
                e.Handled = true;
            }
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog)
        {
            ShowSuplDialog(owner, dialog, 0, "");
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset)
        {
            ShowSuplDialog(owner, dialog, offset, "");
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
            this.TopMost = SettingDialog.AlwaysTop;
            int selStart = owner.SelectionStart;
            string fHalf = "";
            string eHalf = "";
            if (dialog.DialogResult == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(dialog.InputText))
                {
                    if (selStart > 0)
                    {
                        fHalf = owner.Text.Substring(0, selStart - offset);
                    }
                    if (selStart < owner.Text.Length)
                    {
                        eHalf = owner.Text.Substring(selStart);
                    }
                    owner.Text = fHalf + dialog.InputText + eHalf;
                    owner.SelectionStart = selStart + dialog.InputText.Length;
                }
            }
            else
            {
                if (selStart > 0)
                {
                    fHalf = owner.Text.Substring(0, selStart);
                }
                if (selStart < owner.Text.Length)
                {
                    eHalf = owner.Text.Substring(selStart);
                }
                owner.Text = fHalf + eHalf;
                if (selStart > 0)
                {
                    owner.SelectionStart = selStart;
                }
            }
            owner.Focus();
        }

        private void StatusText_KeyUp(object sender, KeyEventArgs e)
        {
            //スペースキーで未読ジャンプ
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
                        StatusText.Text = "";
                        JumpUnreadMenuItem_Click(null, null);
                    }
                }
            }
            this.StatusText_TextChanged(null, null);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            //文字数カウント
            int pLen = GetRestStatusCount(true, false);
            lblLen.Text = pLen.ToString();
            if (pLen < 0)
            {
                StatusText.ForeColor = Color.Red;
            }
            else
            {
                StatusText.ForeColor = _clInputFont;
            }
            if (String.IsNullOrEmpty(StatusText.Text))
            {
                _replyToId = 0;
                _replyToName = "";
            }
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            //文字数カウント
            int pLen = 140 - StatusText.Text.Length;
            if (this.NotifyIcon1 == null || !this.NotifyIcon1.Visible)
            {
                return pLen;
            }
            if ((isAuto && !isKeyDown(Keys.Control) && SettingDialog.PostShiftEnter)
                || (isAuto && !isKeyDown(Keys.Shift) && !SettingDialog.PostShiftEnter)
                || (!isAuto && isAddFooter))
            {
                if (SettingDialog.UseRecommendStatus)
                {
                    pLen -= SettingDialog.RecommendStatusText.Length;
                }
                else if (SettingDialog.Status.Length > 0)
                {
                    pLen -= SettingDialog.Status.Length + 1;
                }
            }
            if (!String.IsNullOrEmpty(HashMgr.UseHash))
            {
                pLen -= HashMgr.UseHash.Length + 1;
            }

            foreach (Match m in Regex.Matches(StatusText.Text, Twitter.RgUrl, RegexOptions.IgnoreCase))
            {
                pLen += m.Result("${url}").Length - SettingDialog.TwitterConfiguration.ShortUrlLength;
            }
            if (ImageSelectionPanel.Visible && ImageSelectedPicture.Tag != null && !String.IsNullOrEmpty(this.ImageService))
            {
                pLen -= SettingDialog.TwitterConfiguration.CharactersReservedPerMedia;
            }
            return pLen;
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (_itemCache != null && e.StartIndex >= _itemCacheIndex && e.EndIndex < _itemCacheIndex + _itemCache.Length && _curList.Equals(sender))
            {
                //If the newly requested cache is a subset of the old cache,
                //no need to rebuild everything, so do nothing.
                return;
            }

            //Now we need to rebuild the cache.
            if (_curList.Equals(sender))
            {
                CreateCache(e.StartIndex, e.EndIndex);
            }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_itemCache != null && e.ItemIndex >= _itemCacheIndex && e.ItemIndex < _itemCacheIndex + _itemCache.Length && _curList.Equals(sender))
            {
                //A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = _itemCache[e.ItemIndex - _itemCacheIndex];
            }
            else
            {
                //A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((Hoehoe.TweenCustomControl.DetailsListView)sender).Parent;
                try
                {
                    e.Item = CreateItem(tb, _statuses.Item(tb.Text, e.ItemIndex), e.ItemIndex);
                }
                catch (Exception)
                {
                    //不正な要求に対する間に合わせの応答
                    e.Item = new ImageListViewItem(new string[] { "", "", "", "", "", "", "", "" }, "");
                }
            }
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            try
            {
                //キャッシュ要求（要求範囲±30を作成）
                startIndex -= 30;
                if (startIndex < 0)
                {
                    startIndex = 0;
                }
                endIndex += 30;
                if (endIndex >= _statuses.Tabs[_curTab.Text].AllCount)
                {
                    endIndex = _statuses.Tabs[_curTab.Text].AllCount - 1;
                }
                _postCache = _statuses.Item(_curTab.Text, startIndex, endIndex);
                //配列で取得
                _itemCacheIndex = startIndex;

                _itemCache = new ListViewItem[_postCache.Length];
                for (int i = 0; i <= _postCache.Length - 1; i++)
                {
                    _itemCache[i] = CreateItem(_curTab, _postCache[i], startIndex + i);
                }
            }
            catch (Exception)
            {
                //キャッシュ要求が実データとずれるため（イベントの遅延？）
                _postCache = null;
                _itemCache = null;
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
                string[] sitem = {
					"",
					Post.Nickname,
					Post.IsDeleted ? "(DELETED)" : Post.TextFromApi,
					Post.CreatedAt.ToString(SettingDialog.DateTimeFormat),
					Post.ScreenName,
					"",
					mk.ToString(),
					Post.Source
				};
                itm = new ImageListViewItem(sitem, (ImageDictionary)this._TIconDic, Post.ImageUrl);
            }
            else
            {
                string[] sitem = {
					"",
					Post.Nickname,
					Post.IsDeleted ? "(DELETED)" : Post.TextFromApi,
					Post.CreatedAt.ToString(SettingDialog.DateTimeFormat),
					Post.ScreenName + Environment.NewLine + "(RT:" + Post.RetweetedBy + ")",
					"",
					mk.ToString(),
					Post.Source
				};
                itm = new ImageListViewItem(sitem, (ImageDictionary)this._TIconDic, Post.ImageUrl);
            }
            itm.StateImageIndex = Post.StateIndex;

            bool read = Post.IsRead;
            //未読管理していなかったら既読として扱う
            if (!_statuses.Tabs[Tab.Text].UnreadManage || !SettingDialog.UnreadManage)
            {
                read = true;
            }
            ChangeItemStyleRead(read, itm, Post, null);
            if (Tab.Equals(_curTab))
            {
                ColorizeList(itm, Index);
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
            //e.ItemStateでうまく判定できない？？？
            if (!e.Item.Selected)
            {
                SolidBrush brs2 = null;
                var cl = e.Item.BackColor;
                if (cl == _clSelf)
                {
                    brs2 = _brsBackColorMine;
                }
                else if (cl == _clAtSelf)
                {
                    brs2 = _brsBackColorAt;
                }
                else if (cl == _clTarget)
                {
                    brs2 = _brsBackColorYou;
                }
                else if (cl == _clAtTarget)
                {
                    brs2 = _brsBackColorAtYou;
                }
                else if (cl == _clAtFromTarget)
                {
                    brs2 = _brsBackColorAtFromTarget;
                }
                else if (cl == _clAtTo)
                {
                    brs2 = _brsBackColorAtTo;
                }
                else
                {
                    brs2 = _brsBackColorNone;
                }
                e.Graphics.FillRectangle(brs2, e.Bounds);
            }
            else
            {
                //選択中の行
                if (((Control)sender).Focused)
                {
                    e.Graphics.FillRectangle(_brsHighLight, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(_brsDeactiveSelection, e.Bounds);
                }
            }
            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
                e.DrawFocusRectangle();
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
                //アイコン以外の列
                RectangleF rct = e.Bounds;
                RectangleF rctB = e.Bounds;
                rct.Width = e.Header.Width;
                rctB.Width = e.Header.Width;
                if (_iconCol)
                {
                    rct.Y += e.Item.Font.Height;
                    rct.Height -= e.Item.Font.Height;
                    rctB.Height = e.Item.Font.Height;
                }

                int heightDiff = 0;
                int drawLineCount = Math.Max(1, Math.DivRem(Convert.ToInt32(rct.Height), e.Item.Font.Height, out heightDiff));

                //フォントの高さの半分を足してるのは保険。無くてもいいかも。
                if (!_iconCol && drawLineCount <= 1)
                {
                }
                else if (heightDiff < e.Item.Font.Height * 0.7)
                {
                    rct.Height = Convert.ToSingle((e.Item.Font.Height * drawLineCount)) - 1;
                }
                else
                {
                    drawLineCount += 1;
                }

                if (!e.Item.Selected)
                {
                    //選択されていない行
                    //文字色
                    SolidBrush brs = null;
                    bool flg = false;
                    var cl = e.Item.ForeColor;
                    if (cl == _clUnread)
                    {
                        brs = _brsForeColorUnread;
                    }
                    else if (cl == _clReaded)
                    {
                        brs = _brsForeColorReaded;
                    }
                    else if (cl == _clFav)
                    {
                        brs = _brsForeColorFav;
                    }
                    else if (cl == _clOWL)
                    {
                        brs = _brsForeColorOWL;
                    }
                    else if (cl == _clRetweet)
                    {
                        brs = _brsForeColorRetweet;
                    }
                    else
                    {
                        brs = new SolidBrush(e.Item.ForeColor);
                        flg = true;
                    }

                    if (rct.Width > 0)
                    {
                        if (_iconCol)
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
                        //選択中の行
                        using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                        {
                            if (((Control)sender).Focused)
                            {
                                if (_iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), _brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), _brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), _brsHighLightText.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), _brsHighLightText.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                            }
                            else
                            {
                                if (_iconCol)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[2].Text, e.Item.Font, Rectangle.Round(rct), _brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                    TextRenderer.DrawText(e.Graphics, e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]", fnt, Rectangle.Round(rctB), _brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
                                }
                                else if (drawLineCount == 1)
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), _brsForeColorUnread.Color, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                                }
                                else
                                {
                                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), _brsForeColorUnread.Color, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix);
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

            //e.Bounds.Leftが常に0を指すから自前で計算
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
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(_iconSz, _iconSz)), itemRect);
                iconRect.Offset(0, Convert.ToInt32(Math.Max(0, (itemRect.Height - _iconSz) / 2)));
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + _iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }
            else
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(1, 1)), itemRect);
                //iconRect.Offset(0, CType(Math.Max(0, (itemRect.Height - _iconSz) / 2), Integer))
                stateRect = Rectangle.Intersect(new Rectangle(iconRect.Location.X + _iconSz + 2, iconRect.Location.Y, 18, 16), itemRect);
            }

            if (item.Image != null && iconRect.Width > 0)
            {
                e.Graphics.FillRectangle(Brushes.White, iconRect);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                try
                {
                    e.Graphics.DrawImage(item.Image, iconRect);
                }
                catch (ArgumentException ex)
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
            if (_curList.VirtualListSize == 0)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText2, Hoehoe.Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            int cidx = 0;
            if (_curList.SelectedIndices.Count > 0)
            {
                cidx = _curList.SelectedIndices[0];
            }
            int toIdx = _curList.VirtualListSize - 1;

            int stp = 1;
            switch (searchType)
            {
                case SEARCHTYPE.DialogSearch:
                    //ダイアログからの検索
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        cidx = _curList.SelectedIndices[0];
                    }
                    else
                    {
                        cidx = 0;
                    }
                    break;
                case SEARCHTYPE.NextSearch:
                    //次を検索
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        cidx = _curList.SelectedIndices[0] + 1;
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
                    //前を検索
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        cidx = _curList.SelectedIndices[0] - 1;
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
                                post = _statuses.Item(_curTab.Text, idx);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (searchRegex.IsMatch(post.Nickname) || searchRegex.IsMatch(post.TextFromApi) || searchRegex.IsMatch(post.ScreenName))
                            {
                                SelectListItem(_curList, idx);
                                _curList.EnsureVisible(idx);
                                return;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show(Hoehoe.Properties.Resources.DoTabSearchText1, "Tween", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            post = _statuses.Item(_curTab.Text, idx);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        if (post.Nickname.IndexOf(word, fndOpt) > -1 || post.TextFromApi.IndexOf(word, fndOpt) > -1 || post.ScreenName.IndexOf(word, fndOpt) > -1)
                        {
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
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
                            cidx = _curList.Items.Count - 1;
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
            //検索メニュー
            SearchDialog.Owner = this;
            if (SearchDialog.ShowDialog() == DialogResult.Cancel)
            {
                this.TopMost = SettingDialog.AlwaysTop;
                return;
            }
            this.TopMost = SettingDialog.AlwaysTop;

            if (!String.IsNullOrEmpty(SearchDialog.SWord))
            {
                DoTabSearch(SearchDialog.SWord, SearchDialog.CheckCaseSensitive, SearchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            //次を検索
            if (String.IsNullOrEmpty(SearchDialog.SWord))
            {
                if (SearchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = SettingDialog.AlwaysTop;
                    return;
                }
                this.TopMost = SettingDialog.AlwaysTop;
                if (String.IsNullOrEmpty(SearchDialog.SWord))
                {
                    return;
                }
                DoTabSearch(SearchDialog.SWord, SearchDialog.CheckCaseSensitive, SearchDialog.CheckRegex, SEARCHTYPE.DialogSearch);
            }
            else
            {
                DoTabSearch(SearchDialog.SWord, SearchDialog.CheckCaseSensitive, SearchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            //前を検索
            if (String.IsNullOrEmpty(SearchDialog.SWord))
            {
                if (SearchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = SettingDialog.AlwaysTop;
                    return;
                }
                this.TopMost = SettingDialog.AlwaysTop;
                if (String.IsNullOrEmpty(SearchDialog.SWord))
                {
                    return;
                }
            }

            DoTabSearch(SearchDialog.SWord, SearchDialog.CheckCaseSensitive, SearchDialog.CheckRegex, SEARCHTYPE.PrevSearch);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            if (_AboutBox == null)
            {
                _AboutBox = new TweenAboutBox();
            }
            _AboutBox.ShowDialog();
            this.TopMost = SettingDialog.AlwaysTop;
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageSelectionPanel.Enabled)
            {
                return;
            }

            //現在タブから最終タブまで探索
            int bgnIdx = ListTab.TabPages.IndexOf(_curTab);
            int idx = -1;
            DetailsListView lst = null;
            for (int i = bgnIdx; i < ListTab.TabPages.Count; i++)
            {
                //未読Index取得
                idx = _statuses.GetOldestUnreadIndex(ListTab.TabPages[i].Text);
                if (idx > -1)
                {
                    ListTab.SelectedIndex = i;
                    lst = (DetailsListView)ListTab.TabPages[i].Tag;
                    break;
                }
            }

            //未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
            if (idx == -1 && bgnIdx > 0)
            {
                for (int i = 0; i < bgnIdx; i++)
                {
                    idx = _statuses.GetOldestUnreadIndex(ListTab.TabPages[i].Text);
                    if (idx > -1)
                    {
                        ListTab.SelectedIndex = i;
                        lst = (DetailsListView)ListTab.TabPages[i].Tag;
                        break;
                    }
                }
            }

            //全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                ListTab.SelectedIndex = 0;
                lst = (DetailsListView)ListTab.TabPages[0].Tag;
                //_curTab = ListTab.TabPages(0)
                if (_statuses.SortOrder == SortOrder.Ascending)
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
                SelectListItem(lst, idx);
                if (_statuses.SortMode == IdComparerClass.ComparerMode.Id)
                {
                    if (_statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - _iconSz - 10 || _statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < _iconSz + 10)
                    {
                        MoveTop();
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
            if (_curList.SelectedIndices.Count > 0 && _statuses.Tabs[_curTab.Text].TabType != MyCommon.TabUsageType.DirectMessage)
            {
                PostClass post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[0]);
                var sid = post.RetweetedId == 0 ? post.StatusId : post.RetweetedId;
                OpenUriAsync("http://twitter.com/" + post.ScreenName + "/status/" + sid.ToString());
            }
        }

        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                PostClass post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[0]);
                OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + post.ScreenName + "/recent");
            }
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            CheckNewVersion();
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
            string retMsg = "";
            string strVer = "";
            string strDetail = "";
            bool forceUpdate = isKeyDown(Keys.Shift);

            try
            {
                retMsg = tw.GetVersionInfo();
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
                if (!String.IsNullOrEmpty(MyCommon.fileVersion) && strVer.CompareTo(MyCommon.fileVersion.Replace(".", "")) > 0)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.CheckNewVersionText3, strVer);
                    using (DialogAsShieldIcon dialogAsShieldicon = new DialogAsShieldIcon())
                    {
                        if (dialogAsShieldicon.ShowDialog(tmp, strDetail, Hoehoe.Properties.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            retMsg = tw.GetTweenBinary(strVer);
                            if (retMsg.Length == 0)
                            {
                                RunTweenUp();
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
                                retMsg = tw.GetTweenBinary(strVer);
                                if (retMsg.Length == 0)
                                {
                                    RunTweenUp();
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
                        MessageBox.Show(Hoehoe.Properties.Resources.CheckNewVersionText7 + MyCommon.fileVersion.Replace(".", "") + Hoehoe.Properties.Resources.CheckNewVersionText8 + strVer, Hoehoe.Properties.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            _colorize = false;
            DispSelectedPost();
            //件数関連の場合、タイトル即時書き換え
            if (SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.None && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.Post && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.Ver && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                SetMainWindowTitle();
            }
            if (!StatusLabelUrl.Text.StartsWith("http"))
            {
                SetStatusLabelUrl();
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (SettingDialog.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
            }
            if (!SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        public string createDetailHtml(string orgdata)
        {
            return _detailHtmlFormatHeader + orgdata + _detailHtmlFormatFooter;
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(displayItem))
            {
                if (UserPicture.Image != null)
                {
                    UserPicture.Image.Dispose();
                }
                if (displayItem.Image != null)
                {
                    try
                    {
                        UserPicture.Image = new Bitmap(displayItem.Image);
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
            DispSelectedPost(false);
        }

        PostClass _displayPost;

        private void DispSelectedPost(bool forceupdate)
        {
            if (_curList.SelectedIndices.Count == 0 || _curPost == null)
            {
                return;
            }

            if (!forceupdate && _curPost.Equals(_displayPost))
            {
                return;
            }

            _displayPost = _curPost;
            if (displayItem != null)
            {
                displayItem.ImageDownloaded -= this.DisplayItemImage_Downloaded;
                displayItem = null;
            }
            displayItem = (ImageListViewItem)_curList.Items[_curList.SelectedIndices[0]];
            displayItem.ImageDownloaded += this.DisplayItemImage_Downloaded;

            string dTxt = createDetailHtml(_curPost.IsDeleted ? "(DELETED)" : _curPost.Text);
            if (_curPost.IsDm)
            {
                SourceLinkLabel.Tag = null;
                SourceLinkLabel.Text = "";
            }
            else
            {
                Match mc = Regex.Match(_curPost.SourceHtml, "<a href=\"(?<sourceurl>.+?)\"");
                if (mc.Success)
                {
                    string src = mc.Groups["sourceurl"].Value;
                    SourceLinkLabel.Tag = mc.Groups["sourceurl"].Value;
                    mc = Regex.Match(src, "^https?://");
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
                if (String.IsNullOrEmpty(_curPost.Source))
                {
                    SourceLinkLabel.Text = "";
                }
                else
                {
                    SourceLinkLabel.Text = _curPost.Source;
                }
            }
            SourceLinkLabel.TabStop = false;

            if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage && !_curPost.IsOwl)
            {
                NameLabel.Text = "DM TO -> ";
            }
            else if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage)
            {
                NameLabel.Text = "DM FROM <- ";
            }
            else
            {
                NameLabel.Text = "";
            }
            NameLabel.Text += _curPost.ScreenName + "/" + _curPost.Nickname;
            NameLabel.Tag = _curPost.ScreenName;
            if (!String.IsNullOrEmpty(_curPost.RetweetedBy))
            {
                NameLabel.Text += " (RT:" + _curPost.RetweetedBy + ")";
            }
            if (UserPicture.Image != null)
            {
                UserPicture.Image.Dispose();
            }
            if (!String.IsNullOrEmpty(_curPost.ImageUrl) && _TIconDic[_curPost.ImageUrl] != null)
            {
                try
                {
                    UserPicture.Image = new Bitmap(_TIconDic[_curPost.ImageUrl]);
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
            DateTimeLabel.Text = _curPost.CreatedAt.ToString();
            if (_curPost.IsOwl && (SettingDialog.OneWayLove || _statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage))
            {
                NameLabel.ForeColor = _clOWL;
            }
            if (_curPost.RetweetedId > 0)
            {
                NameLabel.ForeColor = _clRetweet;
            }
            if (_curPost.IsFav)
            {
                NameLabel.ForeColor = _clFav;
            }

            if (DumpPostClassToolStripMenuItem.Checked)
            {
                StringBuilder sb = new StringBuilder(512);

                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", _curPost.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", _curPost.StatusId.ToString());
                //sb.AppendFormat("ImageIndex     : {0}<br>", _curPost.ImageIndex.ToString)
                sb.AppendFormat("ImageUrl       : {0}<br>", _curPost.ImageUrl);
                sb.AppendFormat("InReplyToStatusId    : {0}<br>", _curPost.InReplyToStatusId.ToString());
                sb.AppendFormat("InReplyToUser  : {0}<br>", _curPost.InReplyToUser);
                sb.AppendFormat("IsDM           : {0}<br>", _curPost.IsDm.ToString());
                sb.AppendFormat("IsFav          : {0}<br>", _curPost.IsFav.ToString());
                sb.AppendFormat("IsMark         : {0}<br>", _curPost.IsMark.ToString());
                sb.AppendFormat("IsMe           : {0}<br>", _curPost.IsMe.ToString());
                sb.AppendFormat("IsOwl          : {0}<br>", _curPost.IsOwl.ToString());
                sb.AppendFormat("IsProtect      : {0}<br>", _curPost.IsProtect.ToString());
                sb.AppendFormat("IsRead         : {0}<br>", _curPost.IsRead.ToString());
                sb.AppendFormat("IsReply        : {0}<br>", _curPost.IsReply.ToString());

                foreach (string nm in _curPost.ReplyToList)
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName           : {0}<br>", _curPost.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", _curPost.Nickname);
                sb.AppendFormat("Text   : {0}<br>", _curPost.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.Text);
                sb.AppendFormat("CreatedAt          : {0}<br>", _curPost.CreatedAt.ToString());
                sb.AppendFormat("Source         : {0}<br>", _curPost.Source);
                sb.AppendFormat("UserId            : {0}<br>", _curPost.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", _curPost.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", _curPost.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", _curPost.RetweetedId);
                sb.AppendFormat("SearchTabName  : {0}<br>", _curPost.RelTabName);
                sb.Append("-----End PostClass Dump<br>");

                PostBrowser.Visible = false;
                PostBrowser.DocumentText = _detailHtmlFormatHeader + sb.ToString() + _detailHtmlFormatFooter;
                PostBrowser.Visible = true;
            }
            else
            {
                try
                {
                    if (PostBrowser.DocumentText != dTxt)
                    {
                        PostBrowser.Visible = false;
                        PostBrowser.DocumentText = dTxt;
                        List<string> lnks = new List<string>();
                        foreach (Match lnk in Regex.Matches(dTxt, "<a target=\"_self\" href=\"(?<url>http[^\"]+)\"", RegexOptions.IgnoreCase))
                        {
                            lnks.Add(lnk.Result("${url}"));
                        }
                        _thumbnail.GenThumbnail(_curPost.StatusId, lnks, _curPost.PostGeo, _curPost.Media);
                    }
                }
                catch (COMException comex)
                {
                    //原因不明
                    System.Diagnostics.Debug.Write(comex);
                }
                catch (UriFormatException)
                {
                    PostBrowser.DocumentText = dTxt;
                }
                finally
                {
                    PostBrowser.Visible = true;
                }
            }
        }

        // TODO: to hoehoe
        private void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/FrontPage");
        }

        // TODO: to hoehoe
        private void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC");
        }

        private void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                {
                    Control pnl = ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused || pnl.Controls["comboLang"].Focused || pnl.Controls["buttonSearch"].Focused)
                    {
                        return;
                    }
                }
                ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
                if (State == ModifierState.NotFlags)
                {
                    return;
                }
                if (State != ModifierState.None)
                {
                    _anchorFlag = false;
                }
                if (CommonKeyDown(e.KeyCode, FocusedControl.ListTab, State))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private ModifierState GetModifierState(bool sControl, bool sShift, bool sAlt)
        {
            ModifierState state = ModifierState.None;
            if (sControl)
            {
                state = state | ModifierState.Ctrl;
            }
            if (sShift)
            {
                state = state | ModifierState.Shift;
            }
            if (sAlt)
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
            //CShift = 11
            //CAlt = 12
            //AShift = 13
            NotFlags = 8

            //ListTab = 101
            //PostBrowser = 102
            //StatusText = 103
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
            //リストのカーソル移動関係（上下キー、PageUp/Downに該当）
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

            //修飾キーなし
            switch (modifierState)
            {
                case ModifierState.None:
                    //フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F1:
                            // todo hoehoe2
                            OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/FrontPage");
                            return true;
                        case Keys.F3:
                            MenuItemSearchNext_Click(null, null);
                            return true;
                        case Keys.F5:
                            DoRefresh();
                            return true;
                        case Keys.F6:
                            GetTimeline(MyCommon.WorkerType.Reply, 1, 0, "");
                            return true;
                        case Keys.F7:
                            GetTimeline(MyCommon.WorkerType.DirectMessegeRcv, 1, 0, "");
                            return true;
                    }
                    if (focusedControl != FocusedControl.StatusText)
                    {
                        //フォーカスStatusText以外
                        switch (keyCode)
                        {
                            case Keys.Space:
                            case Keys.ProcessKey:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    _anchorFlag = false;
                                }
                                JumpUnreadMenuItem_Click(null, null);
                                return true;
                            case Keys.G:
                                if (focusedControl == FocusedControl.ListTab)
                                {
                                    _anchorFlag = false;
                                }
                                ShowRelatedStatusesMenuItem_Click(null, null);
                                return true;
                        }
                    }
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        //フォーカスList
                        switch (keyCode)
                        {
                            case Keys.N:
                            case Keys.Right:
                                GoRelPost(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                GoRelPost(false);
                                return true;
                            case Keys.OemPeriod:
                                GoAnchor();
                                return true;
                            case Keys.I:
                                if (this.StatusText.Enabled)
                                {
                                    this.StatusText.Focus();
                                }
                                return true;
                            case Keys.Enter:
                                //case Keys.Return:
                                MakeReplyOrDirectStatus();
                                return true;
                            case Keys.R:
                                DoRefresh();
                                return true;
                        }
                        //以下、アンカー初期化
                        _anchorFlag = false;
                        switch (keyCode)
                        {
                            case Keys.L:
                                GoPost(true);
                                return true;
                            case Keys.H:
                                GoPost(false);
                                return true;
                            case Keys.Z:
                            case Keys.Oemcomma:
                                MoveTop();
                                return true;
                            case Keys.S:
                                GoNextTab(true);
                                return true;
                            case Keys.A:
                                GoNextTab(false);
                                return true;
                            case Keys.Oem4:
                                // ] in_reply_to参照元へ戻る
                                GoInReplyToPostTree();
                                return true;
                            case Keys.Oem6:
                                // [ in_reply_toへジャンプ
                                GoBackInReplyToPostTree();
                                return true;
                            case Keys.Escape:
                                if (ListTab.SelectedTab != null)
                                {
                                    MyCommon.TabUsageType tabtype = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == MyCommon.TabUsageType.Related || tabtype == MyCommon.TabUsageType.UserTimeline || tabtype == MyCommon.TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = ListTab.SelectedTab;
                                        RemoveSpecifiedTab(relTp.Text, false);
                                        SaveConfigsTabs();
                                        return true;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ModifierState.Ctrl:
                    //フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.R:
                            MakeReplyOrDirectStatus(false, true);
                            return true;
                        case Keys.D:
                            doStatusDelete();
                            return true;
                        case Keys.M:
                            MakeReplyOrDirectStatus(false, false);
                            return true;
                        case Keys.S:
                            FavoriteChange(true);
                            return true;
                        case Keys.I:
                            doRepliedStatusOpen();
                            return true;
                        case Keys.Q:
                            doQuote();
                            return true;
                        case Keys.B:
                            ReadedStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            HashManageMenuItem_Click(null, null);
                            return true;
                        case Keys.L:
                            UrlConvertAutoToolStripMenuItem_Click(null, null);
                            return true;
                        case Keys.Y:
                            if (!(focusedControl == FocusedControl.PostBrowser))
                            {
                                MultiLineMenuItem_Click(null, null);
                                return true;
                            }
                            break;
                        case Keys.F:
                            MenuItemSubSearch_Click(null, null);
                            return true;
                        case Keys.U:
                            ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            if (_curList.SelectedIndices.Count > 0)
                            {
                                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName);
                            }
                            else if (_curList.SelectedIndices.Count == 0)
                            {
                                OpenUriAsync("http://twitter.com/");
                            }
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            if (_curList.SelectedIndices.Count > 0)
                            {
                                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName + "/favorites");
                            }
                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            StatusOpenMenuItem_Click(null, null);
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            OpenURLMenuItem_Click(null, null);
                            return true;
                    }
                    //フォーカスList
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.Home:
                            case Keys.End:
                                _colorize = true;
                                //スルーする
                                return false;
                            case Keys.N:
                                GoNextTab(true);
                                return true;
                            case Keys.P:
                                GoNextTab(false);
                                return true;
                            case Keys.C:
                                CopyStot();
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
                                ListTabSelect(ListTab.TabPages[tabNo]);
                                return true;
                            case Keys.D9:
                                ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                                ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                                return true;
                        }
                    }
                    else if (focusedControl == FocusedControl.StatusText)
                    {
                        //フォーカスStatusText
                        switch (keyCode)
                        {
                            case Keys.A:
                                StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!String.IsNullOrEmpty(StatusText.Text.Trim()))
                                {
                                    _history[_hisIdx] = new PostingStatus(StatusText.Text, _replyToId, _replyToName);
                                }
                                if (keyCode == Keys.Up)
                                {
                                    _hisIdx -= 1;
                                    if (_hisIdx < 0)
                                    {
                                        _hisIdx = 0;
                                    }
                                }
                                else
                                {
                                    _hisIdx += 1;
                                    if (_hisIdx > _history.Count - 1)
                                    {
                                        _hisIdx = _history.Count - 1;
                                    }
                                }
                                StatusText.Text = _history[_hisIdx].Status;
                                _replyToId = _history[_hisIdx].InReplyToId;
                                _replyToName = _history[_hisIdx].InReplyToName;
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
                        //フォーカスPostBrowserもしくは関係なし
                        switch (keyCode)
                        {
                            case Keys.A:
                                PostBrowser.Document.ExecCommand("SelectAll", false, null);
                                return true;
                            case Keys.C:
                            case Keys.Insert:
                                string selText = WebBrowser_GetSelectionText(ref PostBrowser);
                                if (!String.IsNullOrEmpty(selText))
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
                    //フォーカス関係なし
                    switch (keyCode)
                    {
                        case Keys.F3:
                            MenuItemSearchPrev_Click(null, null);
                            return true;
                        case Keys.F5:
                            DoRefreshMore();
                            return true;
                        case Keys.F6:
                            GetTimeline(MyCommon.WorkerType.Reply, -1, 0, "");
                            return true;
                        case Keys.F7:
                            GetTimeline(MyCommon.WorkerType.DirectMessegeRcv, -1, 0, "");
                            return true;
                    }
                    //フォーカスStatusText以外
                    if (focusedControl != FocusedControl.StatusText)
                    {
                        if (keyCode == Keys.R)
                        {
                            DoRefreshMore();
                            return true;
                        }
                    }
                    //フォーカスリスト
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        switch (keyCode)
                        {
                            case Keys.H:
                                GoTopEnd(true);
                                return true;
                            case Keys.L:
                                GoTopEnd(false);
                                return true;
                            case Keys.M:
                                GoMiddle();
                                return true;
                            case Keys.G:
                                GoLast();
                                return true;
                            case Keys.Z:
                                MoveMiddle();
                                return true;
                            case Keys.Oem4:
                                GoBackInReplyToPostTree(true, false);
                                return true;
                            case Keys.Oem6:
                                GoBackInReplyToPostTree(true, true);
                                return true;
                            case Keys.N:
                            case Keys.Right:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(false);
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
                            doReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (_curPost != null)
                            {
                                doShowUserStatus(_curPost.ScreenName, false);
                                return true;
                            }
                            break;
                        case Keys.Up:
                            ScrollDownPostBrowser(false);
                            return true;
                        case Keys.Down:
                            ScrollDownPostBrowser(true);
                            return true;
                        case Keys.PageUp:
                            PageDownPostBrowser(false);
                            return true;
                        case Keys.PageDown:
                            PageDownPostBrowser(true);
                            return true;
                    }
                    if (focusedControl == FocusedControl.ListTab)
                    {
                        // 別タブの同じ書き込みへ(ALT+←/→)
                        if (keyCode == Keys.Right)
                        {
                            GoSamePostToAnotherTab(false);
                            return true;
                        }
                        if (keyCode == Keys.Left)
                        {
                            GoSamePostToAnotherTab(true);
                            return true;
                        }
                    }
                    break;
                case ModifierState.Ctrl | ModifierState.Shift:
                    switch (keyCode)
                    {
                        case Keys.R:
                            MakeReplyOrDirectStatus(false, true, true);
                            return true;
                        case Keys.C:
                            CopyIdUri();
                            return true;
                        case Keys.F:
                            if (ListTab.SelectedTab != null)
                            {
                                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                                {
                                    ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                                    return true;
                                }
                            }
                            break;
                        case Keys.S:
                            FavoriteChange(false);
                            return true;
                        case Keys.B:
                            UnreadStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            HashToggleMenuItem_Click(null, null);
                            return true;
                        case Keys.P:
                            ImageSelectMenuItem_Click(null, null);
                            return true;
                        case Keys.H:
                            doMoveToRTHome();
                            return true;
                        case Keys.O:
                            FavorareMenuItem_Click(null, null);
                            return true;
                    }
                    if (focusedControl == FocusedControl.StatusText)
                    {
                        switch (keyCode)
                        {
                            case Keys.Up:
                                {
                                    int idx = 0;
                                    if (_curList != null && _curList.Items.Count != 0 && _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] > 0)
                                    {
                                        idx = _curList.SelectedIndices[0] - 1;
                                        SelectListItem(_curList, idx);
                                        _curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }
                                break;
                            case Keys.Down:
                                {
                                    int idx = 0;
                                    if (_curList != null && _curList.Items.Count != 0 && _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] < _curList.Items.Count - 1)
                                    {
                                        idx = _curList.SelectedIndices[0] + 1;
                                        SelectListItem(_curList, idx);
                                        _curList.EnsureVisible(idx);
                                        return true;
                                    }
                                }
                                break;
                            case Keys.Space:
                                if (StatusText.SelectionStart > 0)
                                {
                                    int endidx = StatusText.SelectionStart - 1;
                                    string startstr = "";
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
                                            int cnt = AtIdSupl.ItemCount;
                                            ShowSuplDialog(StatusText, AtIdSupl, startstr.Length + 1, startstr);
                                            if (AtIdSupl.ItemCount != cnt)
                                            {
                                                _modifySettingAtId = true;
                                            }
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            ShowSuplDialog(StatusText, HashSupl, startstr.Length + 1, startstr);
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
                        FavoritesRetweetOriginal();
                        return true;
                    }
                    else if (keyCode == Keys.R)
                    {
                        FavoritesRetweetUnofficial();
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
                            doReTweetUnofficial();
                        }
                        else if (keyCode == Keys.C)
                        {
                            CopyUserId();
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
                            doTranslation(_curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            doReTweetUnofficial();
                            return true;
                        case Keys.C:
                            CopyUserId();
                            return true;
                        case Keys.Up:
                            _thumbnail.ScrollThumbnail(false);
                            return true;
                        case Keys.Down:
                            _thumbnail.ScrollThumbnail(true);
                            return true;
                    }
                    if (focusedControl == FocusedControl.ListTab && keyCode == Keys.Enter)
                    {
                        if (!this.SplitContainer3.Panel2Collapsed)
                        {
                            _thumbnail.OpenPicture();
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
                doc.Body.ScrollTop += SettingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= SettingDialog.FontDetail.Height;
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
                doc.Body.ScrollTop += PostBrowser.ClientRectangle.Height - SettingDialog.FontDetail.Height;
            }
            else
            {
                doc.Body.ScrollTop -= PostBrowser.ClientRectangle.Height - SettingDialog.FontDetail.Height;
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
            ListTabSelect(ListTab.TabPages[idx]);
        }

        private void CopyStot()
        {
            string clstr = "";
            StringBuilder sb = new StringBuilder();
            bool IsProtected = false;
            bool isDm = false;
            if (this._curTab != null && this._statuses.GetTabByName(this._curTab.Text) != null)
            {
                isDm = this._statuses.GetTabByName(this._curTab.Text).TabType == MyCommon.TabUsageType.DirectMessage;
            }
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
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
                //MessageBox.Show(My.Resources.CopyStotText1)
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
            string clstr = "";
            StringBuilder sb = new StringBuilder();
            if (this._curTab == null)
            {
                return;
            }
            if (this._statuses.GetTabByName(this._curTab.Text) == null)
            {
                return;
            }
            if (this._statuses.GetTabByName(this._curTab.Text).TabType == MyCommon.TabUsageType.DirectMessage)
            {
                return;
            }
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
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
            if (_curList.VirtualListSize == 0)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                if (_curList.SelectedIndices.Count == 0)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = _curList.SelectedIndices[0] + 1;
                    if (fIdx > _curList.VirtualListSize - 1)
                    {
                        return;
                    }
                }
                toIdx = _curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                if (_curList.SelectedIndices.Count == 0)
                {
                    fIdx = _curList.VirtualListSize - 1;
                }
                else
                {
                    fIdx = _curList.SelectedIndices[0] - 1;
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
                if (_statuses.Item(_curTab.Text, idx).IsFav)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (_curList.VirtualListSize == 0)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            long targetId = 0;

            if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage)
            {
                return;
            }
            // Directタブは対象外（見つかるはずがない）
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }
            //未選択も処理しない

            targetId = GetCurTabPost(_curList.SelectedIndices[0]).StatusId;

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
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == MyCommon.TabUsageType.DirectMessage)
                    continue;
                // Directタブは対象外
                for (int idx = 0; idx <= ((DetailsListView)ListTab.TabPages[tabidx].Tag).VirtualListSize - 1; idx++)
                {
                    if (_statuses.Item(ListTab.TabPages[tabidx].Text, idx).StatusId == targetId)
                    {
                        ListTab.SelectedIndex = tabidx;
                        ListTabSelect(ListTab.TabPages[tabidx]);
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
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
            if (_curList.SelectedIndices.Count == 0 || _curPost == null)
            {
                return;
            }
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                fIdx = _curList.SelectedIndices[0] + 1;
                if (fIdx > _curList.VirtualListSize - 1)
                {
                    return;
                }
                toIdx = _curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = _curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }
                toIdx = 0;
                stp = -1;
            }

            string name = "";
            if (_curPost.RetweetedId == 0)
            {
                name = _curPost.ScreenName;
            }
            else
            {
                name = _curPost.RetweetedBy;
            }
            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                if (_statuses.Item(_curTab.Text, idx).RetweetedId == 0)
                {
                    if (_statuses.Item(_curTab.Text, idx).ScreenName == name)
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        break;
                    }
                }
                else
                {
                    if (_statuses.Item(_curTab.Text, idx).RetweetedBy == name)
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        break;
                    }
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            if (forward)
            {
                fIdx = _curList.SelectedIndices[0] + 1;
                if (fIdx > _curList.VirtualListSize - 1)
                {
                    return;
                }
                toIdx = _curList.VirtualListSize - 1;
                stp = 1;
            }
            else
            {
                fIdx = _curList.SelectedIndices[0] - 1;
                if (fIdx < 0)
                {
                    return;
                }
                toIdx = 0;
                stp = -1;
            }

            if (!_anchorFlag)
            {
                if (_curPost == null)
                {
                    return;
                }
                _anchorPost = _curPost;
                _anchorFlag = true;
            }
            else
            {
                if (_anchorPost == null)
                {
                    return;
                }
            }

            for (int idx = fIdx; idx <= toIdx; idx += stp)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
                if (post.ScreenName == _anchorPost.ScreenName || post.RetweetedBy == _anchorPost.ScreenName || post.ScreenName == _anchorPost.RetweetedBy || (!String.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == _anchorPost.RetweetedBy) || _anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) || _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) || post.ReplyToList.Contains(_anchorPost.ScreenName.ToLower()) || post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLower()))
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            if (_anchorPost == null)
            {
                return;
            }
            int idx = _statuses.Tabs[_curTab.Text].IndexOf(_anchorPost.StatusId);
            if (idx == -1)
            {
                return;
            }
            SelectListItem(_curList, idx);
            _curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool goTop)
        {
            ListViewItem item = null;
            int idx = 0;

            if (goTop)
            {
                item = _curList.GetItemAt(0, 25);
                idx = item != null ? item.Index : 0;
            }
            else
            {
                item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
                idx = item != null ? item.Index : _curList.VirtualListSize - 1;
            }
            SelectListItem(_curList, idx);
        }

        private void GoMiddle()
        {
            ListViewItem item = _curList.GetItemAt(0, 0);
            int idx1 = item == null ? 0 : item.Index;

            item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            int idx2 = item == null ? _curList.VirtualListSize - 1 : item.Index;

            int idx3 = (idx1 + idx2) / 2;

            SelectListItem(_curList, idx3);
        }

        private void GoLast()
        {
            if (_curList.VirtualListSize == 0)
            {
                return;
            }

            if (_statuses.SortOrder == SortOrder.Ascending)
            {
                SelectListItem(_curList, _curList.VirtualListSize - 1);
                _curList.EnsureVisible(_curList.VirtualListSize - 1);
            }
            else
            {
                SelectListItem(_curList, 0);
                _curList.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }
            int idx = _curList.SelectedIndices[0];
            if (_statuses.SortOrder == SortOrder.Ascending)
            {
                _curList.EnsureVisible(_curList.VirtualListSize - 1);
            }
            else
            {
                _curList.EnsureVisible(0);
            }
            _curList.EnsureVisible(idx);
        }

        private void GoInReplyToPostTree()
        {
            if (_curPost == null)
            {
                return;
            }

            TabClass curTabClass = _statuses.Tabs[_curTab.Text];

            if (curTabClass.TabType == MyCommon.TabUsageType.PublicSearch && _curPost.InReplyToStatusId == 0 && _curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = tw.GetStatusApi(false, _curPost.StatusId, ref post);
                if (String.IsNullOrEmpty(r) && post != null)
                {
                    _curPost.InReplyToStatusId = post.InReplyToStatusId;
                    _curPost.InReplyToUser = post.InReplyToUser;
                    _curPost.IsReply = post.IsReply;
                    _itemCache = null;
                    _curList.RedrawItems(_curItemIndex, _curItemIndex, false);
                }
                else
                {
                    this.StatusLabel.Text = r;
                }
            }

            if (!(this.ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId > 0))
            {
                return;
            }

            if (_replyChains == null || (_replyChains.Count > 0 && _replyChains.Peek().InReplyToId != _curPost.StatusId))
            {
                _replyChains = new Stack<ReplyChain>();
            }
            _replyChains.Push(new ReplyChain(_curPost.StatusId, _curPost.InReplyToStatusId, _curTab));

            int inReplyToIndex = 0;
            string inReplyToTabName = null;
            long inReplyToId = _curPost.InReplyToStatusId;
            string inReplyToUser = _curPost.InReplyToUser;
            Dictionary<long, PostClass> curTabPosts = null;

            if (_statuses.Tabs[_curTab.Text].IsInnerStorageTabType)
            {
                curTabPosts = curTabClass.Posts;
            }
            else
            {
                curTabPosts = _statuses.Posts;
            }

            var inReplyToPosts = from tab in _statuses.Tabs.Values
                                 orderby !object.ReferenceEquals(tab, curTabClass)
                                 from post in ((Dictionary<long, PostClass>)(tab.IsInnerStorageTabType ? tab.Posts : _statuses.Posts)).Values
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
                string r = tw.GetStatusApi(false, _curPost.InReplyToStatusId, ref post);
                if (String.IsNullOrEmpty(r) && post != null)
                {
                    post.IsRead = true;
                    _statuses.AddPost(post);
                    _statuses.DistributePosts();
                    //_statuses.SubmitUpdate(Nothing, Nothing, Nothing, False)
                    this.RefreshTimeline(false);
                    try
                    {
                        var inReplyPost = inReplyToPosts.First();
                        inReplyToTabName = inReplyPost.Tab.TabName;
                        inReplyToIndex = inReplyPost.Index;
                    }
                    catch (InvalidOperationException)
                    {
                        OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
                        return;
                    }
                }
                else
                {
                    this.StatusLabel.Text = r;
                    OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
                    return;
                }
            }

            var tabPage = this.ListTab.TabPages.Cast<TabPage>().First(tp => tp.Text == inReplyToTabName);
            var listView = (DetailsListView)tabPage.Tag;

            if (!object.ReferenceEquals(_curTab, tabPage))
            {
                this.ListTab.SelectTab(tabPage);
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (_curPost == null)
            {
                return;
            }

            TabClass curTabClass = _statuses.Tabs[_curTab.Text];
            Dictionary<long, PostClass> curTabPosts = (Dictionary<long, PostClass>)(curTabClass.IsInnerStorageTabType ? curTabClass.Posts : _statuses.Posts);

            if (parallel)
            {
                if (_curPost.InReplyToStatusId != 0)
                {
                    var posts = from t in _statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : _statuses.Posts)
                                where p.Value.StatusId != _curPost.StatusId && p.Value.InReplyToStatusId == _curPost.InReplyToStatusId
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
                        var post = postList.FirstOrDefault(pst => object.ReferenceEquals(pst.Tab, curTabClass) && (bool)(isForward ? pst.Index > _curItemIndex : pst.Index < _curItemIndex));
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
                        SelectListItem(listView, post.Index);
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
                if (_replyChains == null || _replyChains.Count < 1)
                {
                    var posts = from t in _statuses.Tabs
                                from p in (Dictionary<long, PostClass>)(t.Value.IsInnerStorageTabType ? t.Value.Posts : _statuses.Posts)
                                where p.Value.InReplyToStatusId == _curPost.StatusId
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
                        SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                else
                {
                    ReplyChain chainHead = _replyChains.Pop();
                    if (chainHead.InReplyToId == _curPost.StatusId)
                    {
                        int idx = _statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                        if (idx == -1)
                        {
                            _replyChains = null;
                        }
                        else
                        {
                            try
                            {
                                ListTab.SelectTab(chainHead.OriginalTab);
                            }
                            catch (Exception ex)
                            {
                                _replyChains = null;
                            }
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
                        }
                    }
                    else
                    {
                        _replyChains = null;
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
                this._selectPostChains.Push(Tuple.Create(this._curTab, _curPost));
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
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType != MyCommon.TabUsageType.DirectMessage && _statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = _statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    ListTabSelect(ListTab.TabPages[tabidx]);
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
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
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == MyCommon.TabUsageType.DirectMessage && _statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    var idx = _statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    ListTabSelect(ListTab.TabPages[tabidx]);
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    return true;
                }
            }
            return false;
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            _anchorFlag = false;
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            // フォーカスの戻り先を StatusText に設定
            this.Tag = StatusText;
            StatusText.BackColor = _clInputBackcolor;
        }

        public Color InputBackColor
        {
            get { return _clInputBackcolor; }
            set { _clInputBackcolor = value; }
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
            ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags)
            {
                return;
            }
            if (CommonKeyDown(e.KeyCode, FocusedControl.StatusText, State))
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
                SaveConfigsCommon();
                SaveConfigsLocal();
                SaveConfigsTabs();
                SaveConfigsAtId();
            }
            else
            {
                if (_modifySettingCommon)
                {
                    SaveConfigsCommon();
                }
                if (_modifySettingLocal)
                {
                    SaveConfigsLocal();
                }
                if (_modifySettingAtId)
                {
                    SaveConfigsAtId();
                }
            }
        }

        private void SaveConfigsAtId()
        {
            if (_ignoreConfigSave || !SettingDialog.UseAtIdSupplement && AtIdSupl == null)
            {
                return;
            }

            _modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (_ignoreConfigSave)
            {
                return;
            }

            _modifySettingCommon = false;
            lock (_syncObject)
            {
                _cfgCommon.UserName = tw.Username;
                _cfgCommon.UserId = tw.UserId;
                _cfgCommon.Password = tw.Password;
                _cfgCommon.Token = tw.AccessToken;
                _cfgCommon.TokenSecret = tw.AccessTokenSecret;
                _cfgCommon.UserAccounts = SettingDialog.UserAccounts;
                _cfgCommon.UserstreamStartup = SettingDialog.UserstreamStartup;
                _cfgCommon.UserstreamPeriod = SettingDialog.UserstreamPeriodInt;
                _cfgCommon.TimelinePeriod = SettingDialog.TimelinePeriodInt;
                _cfgCommon.ReplyPeriod = SettingDialog.ReplyPeriodInt;
                _cfgCommon.DMPeriod = SettingDialog.DMPeriodInt;
                _cfgCommon.PubSearchPeriod = SettingDialog.PubSearchPeriodInt;
                _cfgCommon.ListsPeriod = SettingDialog.ListsPeriodInt;
                _cfgCommon.UserTimelinePeriod = SettingDialog.UserTimelinePeriodInt;
                _cfgCommon.Read = SettingDialog.Readed;
                _cfgCommon.IconSize = SettingDialog.IconSz;
                _cfgCommon.UnreadManage = SettingDialog.UnreadManage;
                _cfgCommon.PlaySound = SettingDialog.PlaySound;
                _cfgCommon.OneWayLove = SettingDialog.OneWayLove;
                _cfgCommon.NameBalloon = SettingDialog.NameBalloon;
                _cfgCommon.PostCtrlEnter = SettingDialog.PostCtrlEnter;
                _cfgCommon.PostShiftEnter = SettingDialog.PostShiftEnter;
                _cfgCommon.CountApi = SettingDialog.CountApi;
                _cfgCommon.CountApiReply = SettingDialog.CountApiReply;
                _cfgCommon.PostAndGet = SettingDialog.PostAndGet;
                _cfgCommon.DispUsername = SettingDialog.DispUsername;
                _cfgCommon.MinimizeToTray = SettingDialog.MinimizeToTray;
                _cfgCommon.CloseToExit = SettingDialog.CloseToExit;
                _cfgCommon.DispLatestPost = SettingDialog.DispLatestPost;
                _cfgCommon.SortOrderLock = SettingDialog.SortOrderLock;
                _cfgCommon.TinyUrlResolve = SettingDialog.TinyUrlResolve;
                _cfgCommon.ShortUrlForceResolve = SettingDialog.ShortUrlForceResolve;
                _cfgCommon.PeriodAdjust = SettingDialog.PeriodAdjust;
                _cfgCommon.StartupVersion = SettingDialog.StartupVersion;
                _cfgCommon.StartupFollowers = SettingDialog.StartupFollowers;
                _cfgCommon.RestrictFavCheck = SettingDialog.RestrictFavCheck;
                _cfgCommon.AlwaysTop = SettingDialog.AlwaysTop;
                _cfgCommon.UrlConvertAuto = SettingDialog.UrlConvertAuto;
                _cfgCommon.Outputz = SettingDialog.OutputzEnabled;
                _cfgCommon.OutputzKey = SettingDialog.OutputzKey;
                _cfgCommon.OutputzUrlMode = SettingDialog.OutputzUrlmode;
                _cfgCommon.UseUnreadStyle = SettingDialog.UseUnreadStyle;
                _cfgCommon.DateTimeFormat = SettingDialog.DateTimeFormat;
                _cfgCommon.DefaultTimeOut = SettingDialog.DefaultTimeOut;
                _cfgCommon.RetweetNoConfirm = SettingDialog.RetweetNoConfirm;
                _cfgCommon.LimitBalloon = SettingDialog.LimitBalloon;
                _cfgCommon.EventNotifyEnabled = SettingDialog.EventNotifyEnabled;
                _cfgCommon.EventNotifyFlag = SettingDialog.EventNotifyFlag;
                _cfgCommon.IsMyEventNotifyFlag = SettingDialog.IsMyEventNotifyFlag;
                _cfgCommon.ForceEventNotify = SettingDialog.ForceEventNotify;
                _cfgCommon.FavEventUnread = SettingDialog.FavEventUnread;
                _cfgCommon.TranslateLanguage = SettingDialog.TranslateLanguage;
                _cfgCommon.EventSoundFile = SettingDialog.EventSoundFile;
                _cfgCommon.AutoShortUrlFirst = SettingDialog.AutoShortUrlFirst;
                _cfgCommon.TabIconDisp = SettingDialog.TabIconDisp;
                _cfgCommon.ReplyIconState = SettingDialog.ReplyIconState;
                _cfgCommon.ReadOwnPost = SettingDialog.ReadOwnPost;
                _cfgCommon.GetFav = SettingDialog.GetFav;
                _cfgCommon.IsMonospace = SettingDialog.IsMonospace;
                if (IdeographicSpaceToSpaceToolStripMenuItem != null && IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed == false)
                {
                    _cfgCommon.WideSpaceConvert = this.IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }
                _cfgCommon.ReadOldPosts = SettingDialog.ReadOldPosts;
                _cfgCommon.UseSsl = SettingDialog.UseSsl;
                _cfgCommon.BilyUser = SettingDialog.BitlyUser;
                _cfgCommon.BitlyPwd = SettingDialog.BitlyPwd;
                _cfgCommon.ShowGrid = SettingDialog.ShowGrid;
                _cfgCommon.UseAtIdSupplement = SettingDialog.UseAtIdSupplement;
                _cfgCommon.UseHashSupplement = SettingDialog.UseHashSupplement;
                _cfgCommon.PreviewEnable = SettingDialog.PreviewEnable;
                _cfgCommon.Language = SettingDialog.Language;

                _cfgCommon.SortOrder = (int)_statuses.SortOrder;
                switch (_statuses.SortMode)
                {
                    case IdComparerClass.ComparerMode.Nickname:
                        //ニックネーム
                        _cfgCommon.SortColumn = 1;
                        break;
                    case IdComparerClass.ComparerMode.Data:
                        //本文
                        _cfgCommon.SortColumn = 2;
                        break;
                    case IdComparerClass.ComparerMode.Id:
                        //時刻=発言Id
                        _cfgCommon.SortColumn = 3;
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        //名前
                        _cfgCommon.SortColumn = 4;
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        //Source
                        _cfgCommon.SortColumn = 7;
                        break;
                }

                _cfgCommon.Nicoms = SettingDialog.Nicoms;
                _cfgCommon.HashTags = HashMgr.HashHistories;
                if (HashMgr.IsPermanent)
                {
                    _cfgCommon.HashSelected = HashMgr.UseHash;
                }
                else
                {
                    _cfgCommon.HashSelected = "";
                }
                _cfgCommon.HashIsHead = HashMgr.IsHead;
                _cfgCommon.HashIsPermanent = HashMgr.IsPermanent;
                _cfgCommon.HashIsNotAddToAtReply = HashMgr.IsNotAddToAtReply;
                _cfgCommon.TwitterUrl = SettingDialog.TwitterApiUrl;
                _cfgCommon.TwitterSearchUrl = SettingDialog.TwitterSearchApiUrl;
                _cfgCommon.HotkeyEnabled = SettingDialog.HotkeyEnabled;
                _cfgCommon.HotkeyModifier = SettingDialog.HotkeyMod;
                _cfgCommon.HotkeyKey = SettingDialog.HotkeyKey;
                _cfgCommon.HotkeyValue = SettingDialog.HotkeyValue;
                _cfgCommon.BlinkNewMentions = SettingDialog.BlinkNewMentions;
                if (ToolStripFocusLockMenuItem != null && ToolStripFocusLockMenuItem.IsDisposed == false)
                {
                    _cfgCommon.FocusLockToStatusText = this.ToolStripFocusLockMenuItem.Checked;
                }
                _cfgCommon.UseAdditionalCount = SettingDialog.UseAdditionalCount;
                _cfgCommon.MoreCountApi = SettingDialog.MoreCountApi;
                _cfgCommon.FirstCountApi = SettingDialog.FirstCountApi;
                _cfgCommon.SearchCountApi = SettingDialog.SearchCountApi;
                _cfgCommon.FavoritesCountApi = SettingDialog.FavoritesCountApi;
                _cfgCommon.UserTimelineCountApi = SettingDialog.UserTimelineCountApi;
                _cfgCommon.TrackWord = tw.TrackWord;
                _cfgCommon.AllAtReply = tw.AllAtReply;
                _cfgCommon.OpenUserTimeline = SettingDialog.OpenUserTimeline;
                _cfgCommon.ListCountApi = SettingDialog.ListCountApi;
                _cfgCommon.UseImageService = ImageServiceCombo.SelectedIndex;
                _cfgCommon.ListDoubleClickAction = SettingDialog.ListDoubleClickAction;
                _cfgCommon.UserAppointUrl = SettingDialog.UserAppointUrl;
                _cfgCommon.HideDuplicatedRetweets = SettingDialog.HideDuplicatedRetweets;
                _cfgCommon.IsPreviewFoursquare = SettingDialog.IsPreviewFoursquare;
                _cfgCommon.FoursquarePreviewHeight = SettingDialog.FoursquarePreviewHeight;
                _cfgCommon.FoursquarePreviewWidth = SettingDialog.FoursquarePreviewWidth;
                _cfgCommon.FoursquarePreviewZoom = SettingDialog.FoursquarePreviewZoom;
                _cfgCommon.IsListsIncludeRts = SettingDialog.IsListStatusesIncludeRts;
                _cfgCommon.TabMouseLock = SettingDialog.TabMouseLock;
                _cfgCommon.IsRemoveSameEvent = SettingDialog.IsRemoveSameEvent;
                _cfgCommon.IsUseNotifyGrowl = SettingDialog.IsNotifyUseGrowl;

                _cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (_ignoreConfigSave)
            {
                return;
            }
            lock (_syncObject)
            {
                _modifySettingLocal = false;
                _cfgLocal.FormSize = _mySize;
                _cfgLocal.FormLocation = _myLoc;
                _cfgLocal.SplitterDistance = _mySpDis;
                _cfgLocal.PreviewDistance = _mySpDis3;
                _cfgLocal.StatusMultiline = StatusText.Multiline;
                _cfgLocal.StatusTextHeight = _mySpDis2;
                _cfgLocal.AdSplitterDistance = _myAdSpDis;
                _cfgLocal.StatusText = SettingDialog.Status;

                _cfgLocal.FontUnread = _fntUnread;
                _cfgLocal.ColorUnread = _clUnread;
                _cfgLocal.FontRead = _fntReaded;
                _cfgLocal.ColorRead = _clReaded;
                _cfgLocal.FontDetail = _fntDetail;
                _cfgLocal.ColorDetail = _clDetail;
                _cfgLocal.ColorDetailBackcolor = _clDetailBackcolor;
                _cfgLocal.ColorDetailLink = _clDetailLink;
                _cfgLocal.ColorFav = _clFav;
                _cfgLocal.ColorOWL = _clOWL;
                _cfgLocal.ColorRetweet = _clRetweet;
                _cfgLocal.ColorSelf = _clSelf;
                _cfgLocal.ColorAtSelf = _clAtSelf;
                _cfgLocal.ColorTarget = _clTarget;
                _cfgLocal.ColorAtTarget = _clAtTarget;
                _cfgLocal.ColorAtFromTarget = _clAtFromTarget;
                _cfgLocal.ColorAtTo = _clAtTo;
                _cfgLocal.ColorListBackcolor = _clListBackcolor;
                _cfgLocal.ColorInputBackcolor = _clInputBackcolor;
                _cfgLocal.ColorInputFont = _clInputFont;
                _cfgLocal.FontInputFont = _fntInputFont;

                _cfgLocal.BrowserPath = SettingDialog.BrowserPath;
                _cfgLocal.UseRecommendStatus = SettingDialog.UseRecommendStatus;
                _cfgLocal.ProxyType = SettingDialog.SelectedProxyType;
                _cfgLocal.ProxyAddress = SettingDialog.ProxyAddress;
                _cfgLocal.ProxyPort = SettingDialog.ProxyPort;
                _cfgLocal.ProxyUser = SettingDialog.ProxyUser;
                _cfgLocal.ProxyPassword = SettingDialog.ProxyPassword;
                if (_ignoreConfigSave)
                {
                    return;
                }
                _cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            SettingTabs tabSetting = new SettingTabs();
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                if (_statuses.Tabs[ListTab.TabPages[i].Text].TabType != MyCommon.TabUsageType.Related)
                {
                    tabSetting.Tabs.Add(_statuses.Tabs[ListTab.TabPages[i].Text]);
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

            SaveFileDialog1.FileName = String.Format("TweenPosts{0:yyMMdd-HHmmss}.tsv", DateTime.Now);
            SaveFileDialog1.InitialDirectory = MyCommon.GetAppDir();
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
                        //All
                        for (int idx = 0; idx <= _curList.VirtualListSize - 1; idx++)
                        {
                            PostClass post = _statuses.Item(_curTab.Text, idx);
                            string protect = "";
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }
                            sw.WriteLine(String.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                                post.Nickname, post.TextFromApi.Replace("\n", "").Replace("\"", "\"\""),
                                post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl,
                                post.Text.Replace("\n", "").Replace("\"", "\"\""), protect));
                        }
                    }
                    else
                    {
                        foreach (int idx in _curList.SelectedIndices)
                        {
                            PostClass post = _statuses.Item(_curTab.Text, idx);
                            string protect = "";
                            if (post.IsProtect)
                            {
                                protect = "Protect";
                            }
                            sw.WriteLine(String.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}\t\"{6}\"\t{7}",
                                post.Nickname, post.TextFromApi.Replace("\n", "").Replace("\"", "\"\""),
                                post.CreatedAt, post.ScreenName, post.StatusId, post.ImageUrl,
                                post.Text.Replace("\n", "").Replace("\"", "\"\""), protect));
                        }
                    }
                    sw.Close();
                }
            }
            this.TopMost = SettingDialog.AlwaysTop;
        }

        private void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags)
            {
                return;
            }
            bool KeyRes = CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, State);
            if (KeyRes)
            {
                e.IsInputKey = true;
            }
        }

        public bool TabRename(ref string tabName)
        {
            //タブ名変更
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
            this.TopMost = SettingDialog.AlwaysTop;
            if (String.IsNullOrEmpty(newTabText))
            {
                return false;
            }
            //新タブ名存在チェック
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (ListTab.TabPages[i].Text == newTabText)
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.Tabs_DoubleClickText1, newTabText);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            //タブ名のリスト作り直し（デフォルトタブ以外は再作成）
            for (int i = 0; i < ListTab.TabCount; i++)
            {
                if (_statuses.IsDistributableTab(ListTab.TabPages[i].Text))
                {
                    TabDialog.RemoveTab(ListTab.TabPages[i].Text);
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
                    TabDialog.AddTab(ListTab.TabPages[i].Text);
                }
            }
            SaveConfigsCommon();
            SaveConfigsTabs();
            _rclickTabName = newTabText;
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
            TabRename(ref tn);
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (SettingDialog.TabMouseLock)
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
                        _tabDrag = true;
                        _tabMouseDownPoint = e.Location;
                        break;
                    }
                }
            }
            else
            {
                _tabDrag = false;
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

            _tabDrag = false;
            string tn = "";
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

            //タブのないところにドロップ->最後尾へ移動
            if (String.IsNullOrEmpty(tn))
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

            ReOrderTab(tp.Text, tn, bef);
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
            SaveConfigsTabs();
        }

        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            //isAuto:True=先頭に挿入、False=カーソル位置に挿入
            //isReply:True=@,False=DM
            if (!StatusText.Enabled)
            {
                return;
            }
            if (_curList == null)
            {
                return;
            }
            if (_curTab == null)
            {
                return;
            }
            if (!this.ExistCurrentPost)
            {
                return;
            }

            // 複数あてリプライはReplyではなく通常ポスト
            //↑仕様変更で全部リプライ扱いでＯＫ（先頭ドット付加しない）
            //090403暫定でドットを付加しないようにだけ修正。単独と複数の処理は統合できると思われる。
            //090513 all @ replies 廃止の仕様変更によりドット付加に戻し(syo68k)

            if (_curList.SelectedIndices.Count > 0)
            {
                // アイテムが1件以上選択されている
                if (_curList.SelectedIndices.Count == 1 && !isAll && this.ExistCurrentPost)
                {
                    // 単独ユーザー宛リプライまたはDM
                    if ((_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                    {
                        // ダイレクトメッセージ
                        StatusText.Text = "D " + _curPost.ScreenName + " " + StatusText.Text;
                        StatusText.SelectionStart = StatusText.Text.Length;
                        StatusText.Focus();
                        _replyToId = 0;
                        _replyToName = "";
                        return;
                    }
                    if (String.IsNullOrEmpty(StatusText.Text))
                    {
                        //空の場合
                        // ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        StatusText.Text = "@" + _curPost.ScreenName + " ";
                        if (_curPost.RetweetedId > 0)
                        {
                            _replyToId = _curPost.RetweetedId;
                        }
                        else
                        {
                            _replyToId = _curPost.StatusId;
                        }
                        _replyToName = _curPost.ScreenName;
                    }
                    else
                    {
                        //何か入力済の場合
                        if (isAuto)
                        {
                            //1件選んでEnter or DoubleClick
                            if (StatusText.Text.Contains("@" + _curPost.ScreenName + " "))
                            {
                                if (_replyToId > 0 && _replyToName == _curPost.ScreenName)
                                {
                                    //返信先書き換え
                                    if (_curPost.RetweetedId > 0)
                                    {
                                        _replyToId = _curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        _replyToId = _curPost.StatusId;
                                    }
                                    _replyToName = _curPost.ScreenName;
                                }
                                return;
                            }
                            if (!StatusText.Text.StartsWith("@"))
                            {
                                //文頭＠以外
                                if (StatusText.Text.StartsWith(". "))
                                {
                                    // 複数リプライ
                                    StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.ScreenName + " ");
                                    _replyToId = 0;
                                    _replyToName = "";
                                }
                                else
                                {
                                    // 単独リプライ
                                    StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                    if (_curPost.RetweetedId > 0)
                                    {
                                        _replyToId = _curPost.RetweetedId;
                                    }
                                    else
                                    {
                                        _replyToId = _curPost.StatusId;
                                    }
                                    _replyToName = _curPost.ScreenName;
                                }
                            }
                            else
                            {
                                //文頭＠
                                // 複数リプライ
                                StatusText.Text = ". @" + _curPost.ScreenName + " " + StatusText.Text;
                                _replyToId = 0;
                                _replyToName = "";
                            }
                        }
                        else
                        {
                            //1件選んでCtrl-Rの場合（返信先操作せず）
                            int sidx = StatusText.SelectionStart;
                            string id = "@" + _curPost.ScreenName + " ";
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
                            //_reply_to_id = 0
                            //_reply_to_name = Nothing
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

                    //C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r
                    if (isAuto)
                    {
                        //Enter or DoubleClick
                        string sTxt = StatusText.Text;
                        if (!sTxt.StartsWith(". "))
                        {
                            sTxt = ". " + sTxt;
                            _replyToId = 0;
                            _replyToName = "";
                        }
                        for (int cnt = 0; cnt <= _curList.SelectedIndices.Count - 1; cnt++)
                        {
                            PostClass post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[cnt]);
                            if (!sTxt.Contains("@" + post.ScreenName + " "))
                            {
                                sTxt = sTxt.Insert(2, "@" + post.ScreenName + " ");
                            }
                        }
                        StatusText.Text = sTxt;
                    }
                    else
                    {
                        //C-S-r or C-r
                        if (_curList.SelectedIndices.Count > 1)
                        {
                            //複数ポスト選択

                            string ids = "";
                            int sidx = StatusText.SelectionStart;
                            for (int cnt = 0; cnt <= _curList.SelectedIndices.Count - 1; cnt++)
                            {
                                PostClass post = _statuses.Item(_curTab.Text, _curList.SelectedIndices[cnt]);
                                if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.ScreenName + " ";
                                }
                                if (isAll)
                                {
                                    foreach (string nm in post.ReplyToList)
                                    {
                                        if (!ids.Contains("@" + nm + " ") && !nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
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
                                _replyToId = 0;
                                _replyToName = "";
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
                            //1件のみ選択のC-S-r（返信元付加する可能性あり）

                            string ids = "";
                            int sidx = StatusText.SelectionStart;
                            PostClass post = _curPost;
                            if (!ids.Contains("@" + post.ScreenName + " ") && !post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ids += "@" + post.ScreenName + " ";
                            }
                            foreach (string nm in post.ReplyToList)
                            {
                                if (!ids.Contains("@" + nm + " ") && !nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
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
                            if (!String.IsNullOrEmpty(post.RetweetedBy))
                            {
                                if (!ids.Contains("@" + post.RetweetedBy + " ") && !post.RetweetedBy.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.RetweetedBy + " ";
                                }
                            }
                            if (ids.Length == 0)
                            {
                                return;
                            }
                            if (String.IsNullOrEmpty(StatusText.Text))
                            {
                                //未入力の場合のみ返信先付加
                                StatusText.Text = ids;
                                StatusText.SelectionStart = ids.Length;
                                StatusText.Focus();
                                if (post.RetweetedId > 0)
                                {
                                    _replyToId = post.RetweetedId;
                                }
                                else
                                {
                                    _replyToId = post.StatusId;
                                }
                                _replyToName = post.ScreenName;
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
            _tabDrag = false;
        }

        int _iconCnt = 0;
        int _blinkCnt = 0;
        bool _beBlink;
        bool _isIdle;

        private void RefreshTasktrayIcon(bool forceRefresh)
        {
            if (_colorize)
            {
                Colorize();
            }
            if (!TimerRefreshIcon.Enabled)
            {
                return;
            }

            if (forceRefresh)
            {
                _isIdle = false;
            }

            _iconCnt += 1;
            _blinkCnt += 1;

            bool busy = false;
            foreach (BackgroundWorker bw in this._bw)
            {
                if (bw != null && bw.IsBusy)
                {
                    busy = true;
                    break;
                }
            }

            if (_iconCnt > 3)
            {
                _iconCnt = 0;
            }
            if (_blinkCnt > 10)
            {
                _blinkCnt = 0;
                //未保存の変更を保存
                SaveConfigsAll(true);
            }

            if (busy)
            {
                NotifyIcon1.Icon = _NIconRefresh[_iconCnt];
                _isIdle = false;
                _myStatusError = false;
                return;
            }

            TabClass tb = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
            if (SettingDialog.ReplyIconState != MyCommon.ReplyIconState.None && tb != null && tb.UnreadCount > 0)
            {
                if (_blinkCnt > 0)
                {
                    return;
                }
                _beBlink = !_beBlink;
                if (_beBlink || SettingDialog.ReplyIconState == MyCommon.ReplyIconState.StaticIcon)
                {
                    NotifyIcon1.Icon = _ReplyIcon;
                }
                else
                {
                    NotifyIcon1.Icon = _ReplyIconBlink;
                }
                _isIdle = false;
                return;
            }

            if (_isIdle)
            {
                return;
            }
            _isIdle = true;
            //優先度：エラー→オフライン→アイドル
            //エラーは更新アイコンでクリアされる
            if (_myStatusError)
            {
                NotifyIcon1.Icon = _NIconAtRed;
                return;
            }
            if (MyCommon.IsNetworkAvailable())
            {
                NotifyIcon1.Icon = _NIconAt;
            }
            else
            {
                NotifyIcon1.Icon = _NIconAtSmoke;
            }
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            //200ms
            this.RefreshTasktrayIcon(false);
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            //右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (String.IsNullOrEmpty(_rclickTabName) || !object.ReferenceEquals(sender, ContextMenuTabProperty))
            {
                if (ListTab != null && ListTab.SelectedTab != null)
                {
                    _rclickTabName = ListTab.SelectedTab.Text;
                }
                else
                {
                    return;
                }
            }

            if (_statuses == null)
            {
                return;
            }
            if (_statuses.Tabs == null)
            {
                return;
            }

            TabClass tb = _statuses.Tabs[_rclickTabName];
            if (tb == null)
            {
                return;
            }

            NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;
            _soundfileListup = true;
            SoundFileComboBox.Items.Clear();
            this.SoundFileTbComboBox.Items.Clear();
            SoundFileComboBox.Items.Add("");
            this.SoundFileTbComboBox.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(MyCommon.GetAppDir() + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.GetAppDir(), "Sounds")))
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
            _soundfileListup = false;
            UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            TabMenuControl(_rclickTabName);
        }

        private void TabMenuControl(string tabName)
        {
            if (_statuses.Tabs[tabName].TabType != MyCommon.TabUsageType.Mentions && _statuses.IsDefaultTab(tabName))
            {
                FilterEditMenuItem.Enabled = true;
                this.EditRuleTbMenuItem.Enabled = true;
                DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else if (_statuses.Tabs[tabName].TabType == MyCommon.TabUsageType.Mentions)
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

            if (String.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }
            ChangeTabUnreadManage(_rclickTabName, UreadManageMenuItem.Checked);

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
            if (SettingDialog.TabIconDisp)
            {
                if (_statuses.Tabs[tabName].UnreadCount > 0)
                {
                    ListTab.TabPages[idx].ImageIndex = 0;
                }
                else
                {
                    ListTab.TabPages[idx].ImageIndex = -1;
                }
            }

            if (_curTab.Text == tabName)
            {
                _itemCache = null;
                _postCache = null;
                _curList.Refresh();
            }

            SetMainWindowTitle();
            SetStatusLabelUrl();
            if (!SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            NotifyDispMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked;

            if (String.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            _statuses.Tabs[_rclickTabName].Notify = NotifyDispMenuItem.Checked;

            SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_soundfileListup || String.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }

            _statuses.Tabs[_rclickTabName].SoundFile = (string)((ToolStripComboBox)sender).SelectedItem;

            SaveConfigsTabs();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_rclickTabName) || object.ReferenceEquals(sender, this.DeleteTbMenuItem))
            {
                _rclickTabName = ListTab.SelectedTab.Text;
            }

            RemoveSpecifiedTab(_rclickTabName, true);
            SaveConfigsTabs();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_rclickTabName))
            {
                _rclickTabName = _statuses.GetTabByType(MyCommon.TabUsageType.Home).TabName;
            }
            fltDialog.SetCurrent(_rclickTabName);
            fltDialog.ShowDialog();
            this.TopMost = SettingDialog.AlwaysTop;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _itemCache = null;
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                _statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                    if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
                if (!SettingDialog.TabIconDisp)
                    ListTab.Refresh();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            SaveConfigsTabs();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = null;
            MyCommon.TabUsageType tabUsage = default(MyCommon.TabUsageType);
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = _statuses.GetUniqueTabName();
                inputName.SetIsShowUsage(true);
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }
            this.TopMost = SettingDialog.AlwaysTop;
            if (!String.IsNullOrEmpty(tabName))
            {
                //List対応
                ListElement list = null;
                if (tabUsage == MyCommon.TabUsageType.Lists)
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
                if (!_statuses.AddTab(tabName, tabUsage, list) || !AddNewTab(tabName, false, tabUsage, list))
                {
                    string tmp = string.Format(Hoehoe.Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    //成功
                    SaveConfigsTabs();
                    if (tabUsage == MyCommon.TabUsageType.PublicSearch)
                    {
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                        ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                    }
                    if (tabUsage == MyCommon.TabUsageType.Lists)
                    {
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        ListTabSelect(ListTab.TabPages[ListTab.TabPages.Count - 1]);
                        GetTimeline(MyCommon.WorkerType.List, 1, 0, tabName);
                    }
                }
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            //選択発言を元にフィルタ追加
            foreach (int idx in _curList.SelectedIndices)
            {
                string tabName = "";
                //タブ選択（or追加）
                if (!SelectTab(ref tabName))
                {
                    return;
                }

                fltDialog.SetCurrent(tabName);
                PostClass statusesItem = _statuses.Item(_curTab.Text, idx);
                if (statusesItem.RetweetedId == 0)
                {
                    fltDialog.AddNewFilter(statusesItem.ScreenName, statusesItem.TextFromApi);
                }
                else
                {
                    fltDialog.AddNewFilter(statusesItem.RetweetedBy, statusesItem.TextFromApi);
                }
                fltDialog.ShowDialog();
                this.TopMost = SettingDialog.AlwaysTop;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _itemCache = null;
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                _statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                    if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = 0;
                        }
                    }
                    else
                    {
                        if (SettingDialog.TabIconDisp)
                        {
                            tb.ImageIndex = -1;
                        }
                    }
                }
                if (!SettingDialog.TabIconDisp)
                    ListTab.Refresh();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null && ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                _curPost = _statuses.Item(this.ListTab.SelectedTab.Text, ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            //TextBox1でEnterを押してもビープ音が鳴らないようにする
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                if (StatusText.Focused)
                {
                    bool doNewLine = false;
                    bool doPost = false;

                    //Ctrl+Enter投稿時
                    if (SettingDialog.PostCtrlEnter)
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
                            if (((keyData & Keys.Control) == Keys.Control))
                            {
                                doPost = true;
                            }
                        }

                        //SHift+Enter投稿時
                    }
                    else if (SettingDialog.PostShiftEnter)
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
                            if (((keyData & Keys.Shift) == Keys.Shift))
                            {
                                doPost = true;
                            }
                        }

                        //Enter投稿時
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
                            //選択状態文字列削除
                        }
                        StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine);
                        //改行挿入
                        StatusText.SelectionStart = pos1 + Environment.NewLine.Length;
                        //カーソルを改行の次の文字へ移動
                        return true;
                    }
                    else if (doPost)
                    {
                        PostButton_Click(null, null);
                        return true;
                    }
                }
                else if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch && (ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focused || ListTab.SelectedTab.Controls["panelSearch"].Controls["comboLang"].Focused))
                {
                    this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, true, true);
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = "";

            //未選択なら処理終了
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            //タブ選択（or追加）
            if (!SelectTab(ref tabName))
            {
                return;
            }

            bool mv = false;
            bool mk = false;
            MoveOrCopy(ref mv, ref mk);

            List<string> ids = new List<string>();
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Item(_curTab.Text, idx);
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
                    _statuses.Tabs[tabName].AddFilter(fc);
                }
            }
            if (ids.Count != 0)
            {
                List<string> atids = new List<string>();
                foreach (string id in ids)
                {
                    atids.Add("@" + id);
                }
                int cnt = AtIdSupl.ItemCount;
                AtIdSupl.AddRangeItem(atids.ToArray());
                if (AtIdSupl.ItemCount != cnt)
                {
                    _modifySettingAtId = true;
                }
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _itemCache = null;
                _postCache = null;
                _curPost = null;
                _curItemIndex = -1;
                _statuses.FilterAll();
                foreach (TabPage tb in ListTab.TabPages)
                {
                    ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                    if (_statuses.ContainsTab(tb.Text))
                    {
                        if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                }
                if (!SettingDialog.TabIconDisp)
                {
                    ListTab.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            SaveConfigsTabs();
        }

        private bool SelectTab(ref string tabName)
        {
            do
            {
                //振り分け先タブ選択
                if (TabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.TopMost = SettingDialog.AlwaysTop;
                    return false;
                }
                this.TopMost = SettingDialog.AlwaysTop;
                tabName = TabDialog.SelectedTabName;

                ListTab.SelectedTab.Focus();
                //新規タブを選択→タブ作成
                if (tabName == Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText1)
                {
                    using (InputTabName inputName = new InputTabName())
                    {
                        inputName.TabName = _statuses.GetUniqueTabName();
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                        tabName = inputName.TabName;
                        inputName.Dispose();
                    }
                    this.TopMost = SettingDialog.AlwaysTop;
                    if (!String.IsNullOrEmpty(tabName))
                    {
                        if (!_statuses.AddTab(tabName, MyCommon.TabUsageType.UserDefined, null) || !AddNewTab(tabName, false, MyCommon.TabUsageType.UserDefined))
                        {
                            string tmp = string.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //もう一度タブ名入力
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    //既存タブを選択
                    return true;
                }
            } while (true);
        }

        private void MoveOrCopy(ref bool move, ref bool mark)
        {
            //移動するか？
            string _tmp = String.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
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
                //マークするか？
                _tmp = String.Format(Hoehoe.Properties.Resources.IDRuleMenuItem_ClickText6, "\r\n");
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
                for (int i = 0; i <= _curList.VirtualListSize - 1; i++)
                {
                    _curList.SelectedIndices.Add(i);
                }
            }
        }

        private void MoveMiddle()
        {
            if (_curList.SelectedIndices.Count == 0)
            {
                return;
            }

            int idx = _curList.SelectedIndices[0];

            ListViewItem item = _curList.GetItemAt(0, 25);
            int idx1 = item == null ? 0 : item.Index;

            item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            int idx2 = item == null ? _curList.VirtualListSize - 1 : item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0)
            {
                idx = 0;
            }
            _curList.EnsureVisible(_curList.VirtualListSize - 1);
            _curList.EnsureVisible(idx);
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            if (PostBrowser.Document.Links.Count > 0)
            {
                UrlDialog.ClearUrl();

                string openUrlStr = "";

                if (PostBrowser.Document.Links.Count == 1)
                {
                    string urlStr = "";
                    try
                    {
                        urlStr = MyCommon.IDNDecode(PostBrowser.Document.Links[0].GetAttribute("href"));
                    }
                    catch (ArgumentException)
                    {
                        //変なHTML？
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (String.IsNullOrEmpty(urlStr))
                    {
                        return;
                    }
                    openUrlStr = MyCommon.urlEncodeMultibyteChar(urlStr);
                }
                else
                {
                    foreach (HtmlElement linkElm in PostBrowser.Document.Links)
                    {
                        string urlStr = "";
                        string linkText = "";
                        string href = "";
                        try
                        {
                            urlStr = linkElm.GetAttribute("title");
                            href = MyCommon.IDNDecode(linkElm.GetAttribute("href"));
                            if (String.IsNullOrEmpty(urlStr))
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
                            //変なHTML？
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        if (String.IsNullOrEmpty(urlStr))
                        {
                            continue;
                        }
                        UrlDialog.AddUrl(new OpenUrlItem(linkText, MyCommon.urlEncodeMultibyteChar(urlStr), href));
                    }
                    try
                    {
                        if (UrlDialog.ShowDialog() == DialogResult.OK)
                        {
                            openUrlStr = UrlDialog.SelectedUrl;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    this.TopMost = SettingDialog.AlwaysTop;
                }
                if (String.IsNullOrEmpty(openUrlStr))
                {
                    return;
                }

                if (openUrlStr.StartsWith("http://twitter.com/search?q=") || openUrlStr.StartsWith("https://twitter.com/search?q="))
                {
                    //ハッシュタグの場合は、タブで開く
                    string urlStr = HttpUtility.UrlDecode(openUrlStr);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    HashSupl.AddItem(hash);
                    HashMgr.AddHashToHistory(hash.Trim(), false);
                    AddNewTabForSearch(hash);
                    return;
                }
                Match m = Regex.Match(openUrlStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                if (SettingDialog.OpenUserTimeline && m.Success && IsTwitterId(m.Result("${ScreenName}")))
                {
                    this.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                }
                else
                {
                    OpenUriAsync(openUrlStr);
                }
                return;
            }
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }
            ClearTab(_rclickTabName, true);
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

            _statuses.ClearTabIds(tabName);
            if (ListTab.SelectedTab.Text == tabName)
            {
                _anchorPost = null;
                _anchorFlag = false;
                _itemCache = null;
                _postCache = null;
                _itemCacheIndex = -1;
                _curItemIndex = -1;
                _curPost = null;
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
            if (!SettingDialog.TabIconDisp)
            {
                ListTab.Refresh();
            }

            SetMainWindowTitle();
            SetStatusLabelUrl();
        }

        long _prevFollowerCount = 0;

        //メインウインドウタイトルの書き換え
        private void SetMainWindowTitle()
        {
            StringBuilder ttl = new StringBuilder(256);
            int ur = 0;
            int al = 0;
            if (SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.None
                && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.Post
                && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.Ver
                && SettingDialog.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                foreach (string key in _statuses.Tabs.Keys)
                {
                    ur += _statuses.Tabs[key].UnreadCount;
                    al += _statuses.Tabs[key].AllCount;
                }
            }

            if (SettingDialog.DispUsername)
            {
                ttl.Append(tw.Username).Append(" - ");
            }
            ttl.Append("Tween  ");
            switch (SettingDialog.DispLatestPost)
            {
                case MyCommon.DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.fileVersion);
                    break;
                case MyCommon.DispTitleEnum.Post:
                    if (_history != null && _history.Count > 1)
                    {
                        ttl.Append(_history[_history.Count - 2].Status.Replace("\r\n", ""));
                    }
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText1, _statuses.GetTabByType(MyCommon.TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText3, ur, _statuses.GetTabByType(MyCommon.TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Hoehoe.Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    if (_prevFollowerCount == 0 && tw.FollowersCount > 0)
                    {
                        _prevFollowerCount = tw.FollowersCount;
                    }
                    ttl.AppendFormat(Hoehoe.Properties.Resources.OwnStatusTitle, tw.StatusesCount, tw.FriendsCount, tw.FollowersCount, tw.FollowersCount - _prevFollowerCount);
                    break;
            }

            try
            {
                this.Text = ttl.ToString();
            }
            catch (AccessViolationException ex)
            {
                //原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private string GetStatusLabelText()
        {
            //ステータス欄にカウント表示
            //タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (_statuses == null)
            {
                return "";
            }
            TabClass tbRep = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
            TabClass tbDm = _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage);
            if (tbRep == null || tbDm == null)
            {
                return "";
            }
            int urat = tbRep.UnreadCount + tbDm.UnreadCount;
            int ur = 0;
            int al = 0;
            int tur = 0;
            int tal = 0;
            StringBuilder slbl = new StringBuilder(256);
            try
            {
                foreach (string key in _statuses.Tabs.Keys)
                {
                    ur += _statuses.Tabs[key].UnreadCount;
                    al += _statuses.Tabs[key].AllCount;
                    if (key.Equals(_curTab.Text))
                    {
                        tur = _statuses.Tabs[key].UnreadCount;
                        tal = _statuses.Tabs[key].AllCount;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            _unreadCounter = ur;
            _unreadAtCounter = urat;

            slbl.AppendFormat(Hoehoe.Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, _postTimestamps.Count, _favTimestamps.Count, _tlCount);
            if (SettingDialog.TimelinePeriodInt == 0)
            {
                slbl.Append(Hoehoe.Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(SettingDialog.TimelinePeriodInt.ToString() + Hoehoe.Properties.Resources.SetStatusLabelText3);
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
                    Invoke(new SetStatusLabelApiDelegate(SetStatusLabelApi));
                }
                else
                {
                    SetStatusLabelApi();
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
            StatusLabelUrl.Text = GetStatusLabelText();
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
            if (SettingDialog.DispUsername)
            {
                ur.Append(tw.Username);
                ur.Append(" - ");
            }
            ur.Append("Tween");
#if DEBUG
			static_SetNotifyIconText_ur.Append("(Debug Build)");
#endif
            if (_unreadCounter != -1 && _unreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(_unreadCounter);
                ur.Append("/@");
                ur.Append(_unreadAtCounter);
                ur.Append("]");
            }
            NotifyIcon1.Text = ur.ToString();
        }

        internal void CheckReplyTo(string StatusText)
        {
            //ハッシュタグの保存
            MatchCollection m = Regex.Matches(StatusText, Twitter.HASHTAG, RegexOptions.IgnoreCase);
            string hstr = "";
            foreach (Match hm in m)
            {
                if (!hstr.Contains("#" + hm.Result("$3") + " "))
                {
                    hstr += "#" + hm.Result("$3") + " ";
                    HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }
            if (!String.IsNullOrEmpty(HashMgr.UseHash) && !hstr.Contains(HashMgr.UseHash + " "))
            {
                hstr += HashMgr.UseHash;
            }
            if (!String.IsNullOrEmpty(hstr))
            {
                HashMgr.AddHashToHistory(hstr.Trim(), false);
            }

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(StatusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");

            if (SettingDialog.UseAtIdSupplement)
            {
                int bCnt = AtIdSupl.ItemCount;
                foreach (Match mid in m)
                {
                    AtIdSupl.AddItem(mid.Result("${id}"));
                }
                if (bCnt != AtIdSupl.ItemCount)
                {
                    _modifySettingAtId = true;
                }
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (_replyToId == 0)
            {
                return;
            }

            // リプライ先ユーザー名がない場合も指定しない
            if (String.IsNullOrEmpty(_replyToName))
            {
                _replyToId = 0;
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
                    if (StatusText.StartsWith("@" + _replyToName))
                    {
                        return;
                    }
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (StatusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + _replyToName)
                        {
                            return;
                        }
                    }
                }
            }

            _replyToId = 0;
            _replyToName = "";
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            if (!_initialLayout && SettingDialog.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            if (_initialLayout && _cfgLocal != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                this.ClientSize = _cfgLocal.FormSize;         //'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                this.DesktopLocation = _cfgLocal.FormLocation;//'位置保持（最小化・最大化されたまま終了した場合の対応用）

                if (!SplitContainer4.Panel2Collapsed && _cfgLocal.AdSplitterDistance > this.SplitContainer4.Panel1MinSize)
                {
                    this.SplitContainer4.SplitterDistance = _cfgLocal.AdSplitterDistance;
                    //Splitterの位置設定
                }
                if (_cfgLocal.SplitterDistance > this.SplitContainer1.Panel1MinSize && _cfgLocal.SplitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    this.SplitContainer1.SplitterDistance = _cfgLocal.SplitterDistance;
                    //Splitterの位置設定
                }
                //発言欄複数行
                StatusText.Multiline = _cfgLocal.StatusMultiline;
                if (StatusText.Multiline)
                {
                    int dis = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    if (dis > SplitContainer2.Panel1MinSize && dis < SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth;
                    }
                    StatusText.Height = _cfgLocal.StatusTextHeight;
                }
                else
                {
                    if (SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth > 0)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
                    }
                }
                if (_cfgLocal.PreviewDistance > this.SplitContainer3.Panel1MinSize && _cfgLocal.PreviewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = _cfgLocal.PreviewDistance;
                }
                _initialLayout = false;
            }
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            PlaySoundMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.PlaySoundFileMenuItem.Checked = PlaySoundMenuItem.Checked;
            SettingDialog.PlaySound = PlaySoundMenuItem.Checked;
            _modifySettingCommon = true;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis = SplitContainer1.SplitterDistance;
                if (StatusText.Multiline)
                {
                    _mySpDis2 = StatusText.Height;
                }
                _modifySettingLocal = true;
            }
        }

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myAdSpDis = SplitContainer4.SplitterDistance;
                _modifySettingLocal = true;
            }
        }

        private void doRepliedStatusOpen()
        {
            if (this.ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId > 0)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenUriAsync(String.Format("https://twitter.com/{0}/status/{1}", _curPost.InReplyToUser, _curPost.InReplyToStatusId));
                    return;
                }
                if (_statuses.ContainsKey(_curPost.InReplyToStatusId))
                {
                    PostClass repPost = _statuses.Item(_curPost.InReplyToStatusId);
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.Lists | MyCommon.TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(_curPost.InReplyToStatusId))
                        {
                            break;
                        }
                        PostClass repPost = _statuses.Item(_curPost.InReplyToStatusId);
                        MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }
                    OpenUriAsync("http://twitter.com/" + _curPost.InReplyToUser + "/status/" + _curPost.InReplyToStatusId.ToString());
                }
            }
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            doRepliedStatusOpen();
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            //発言詳細のアイコン右クリック時のメニュー制御
            if (_curList.SelectedIndices.Count > 0 && _curPost != null)
            {
                string name = _curPost.ImageUrl;
                if (name != null && name.Length > 0)
                {
                    int idx = name.LastIndexOf('/');
                    if (idx != -1)
                    {
                        name = Path.GetFileName(name.Substring(idx));
                        if (name.Contains("_normal.") || name.EndsWith("_normal"))
                        {
                            name = name.Replace("_normal", "");
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
                    if (this._TIconDic[_curPost.ImageUrl] != null)
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
                if (id == tw.Username)
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
            if (_curPost == null)
            {
                return;
            }
            string name = _curPost.ImageUrl;
            OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost == null)
            {
                return;
            }
            string name = _curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));

            this.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8);

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // STUB
            }
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost == null)
            {
                return;
            }
            string name = _curPost.ImageUrl;

            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(_TIconDic[name]))
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
                    //処理中にキャッシュアウトする可能性あり
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            this.StatusText.Multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            MultiLineMenuItem.Checked = this.StatusText.Multiline;
            _modifySettingLocal = true;
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
            _modifySettingLocal = true;
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            //発言欄複数行
            StatusText.Multiline = MultiLineMenuItem.Checked;
            _cfgLocal.StatusMultiline = MultiLineMenuItem.Checked;
            if (MultiLineMenuItem.Checked)
            {
                if (SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth < 0)
                {
                    SplitContainer2.SplitterDistance = 0;
                }
                else
                {
                    SplitContainer2.SplitterDistance = SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth;
                }
            }
            else
            {
                SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
            }
            _modifySettingLocal = true;
        }

        private bool UrlConvert(Hoehoe.MyCommon.UrlConverter urlCoonverterType)
        {
            //t.coで投稿時自動短縮する場合は、外部サービスでの短縮禁止
            //If SettingDialog.UrlConvertAuto AndAlso SettingDialog.ShortenTco Then Exit Function

            //Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
            //参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
            //Appendix A.  Collected ABNF for URI
            //http://www.ietf.org/rfc/rfc3986.txt

            string result = "";

            const string nico = "^https?://[a-z]+\\.(nicovideo|niconicommons|nicolive)\\.jp/[a-z]+/[a-z0-9]+$";

            if (StatusText.SelectionLength > 0)
            {
                string tmp = StatusText.SelectedText;
                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http"))
                {
                    // 文字列が選択されている場合はその文字列について処理

                    //nico.ms使用、nicovideoにマッチしたら変換
                    if (SettingDialog.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != MyCommon.UrlConverter.Nicoms)
                    {
                        //短縮URL変換 日本語を含むかもしれないのでURLエンコードする
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

                    if (!String.IsNullOrEmpty(result))
                    {
                        StatusText.Select(StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                        StatusText.SelectedText = result;

                        //undoバッファにセット
                        if (urlUndoBuffer == null)
                        {
                            urlUndoBuffer = new List<urlUndo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        urlUndoBuffer.Add(new urlUndo() { Before = tmp, After = result });
                    }
                }
            }
            else
            {
                const string url = "(?<before>(?:[^\\\"':!=]|^|\\:))" + "(?<url>(?<protocol>https?://)" + "(?<domain>(?:[\\.-]|[^\\p{P}\\s])+\\.[a-z]{2,}(?::[0-9]+)?)" + "(?<path>/[a-z0-9!*'();:&=+$/%#\\-_.,~@]*[a-z0-9)=#/]?)?" + "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\-_.,~@?]*[a-z0-9_&=#/])?)";
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
                    //選んだURLを選択（？）
                    StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    //nico.ms使用、nicovideoにマッチしたら変換
                    if (SettingDialog.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (urlCoonverterType != MyCommon.UrlConverter.Nicoms)
                    {
                        //短縮URL変換 日本語を含むかもしれないのでURLエンコードする
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

                    if (!String.IsNullOrEmpty(result))
                    {
                        StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);
                        StatusText.SelectedText = result;
                        //undoバッファにセット
                        if (urlUndoBuffer == null)
                        {
                            urlUndoBuffer = new List<urlUndo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        urlUndoBuffer.Add(new urlUndo() { Before = mt.Result("${url}"), After = result });
                    }
                }
            }

            return true;
        }

        private void doUrlUndo()
        {
            if (urlUndoBuffer != null)
            {
                string tmp = StatusText.Text;
                foreach (urlUndo data in urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }
                StatusText.Text = tmp;
                urlUndoBuffer = null;
                UrlUndoToolStripMenuItem.Enabled = false;
                StatusText.SelectionStart = 0;
                StatusText.SelectionLength = 0;
            }
        }

        private void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.TinyUrl);
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.Isgd);
        }

        private void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.Twurl);
        }

        private void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.Uxnu);
        }

        private void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UrlConvert(SettingDialog.AutoShortUrlFirst))
            {
                MyCommon.UrlConverter svc = SettingDialog.AutoShortUrlFirst;
                Random rnd = new Random();
                // 前回使用した短縮URLサービス以外を選択する
                do
                {
                    svc = (MyCommon.UrlConverter)rnd.Next(System.Enum.GetNames(typeof(MyCommon.UrlConverter)).Length);
                } while (!(svc != SettingDialog.AutoShortUrlFirst && svc != MyCommon.UrlConverter.Nicoms && svc != MyCommon.UrlConverter.Unu));
                UrlConvert(svc);
            }
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doUrlUndo();
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.NotifyFileMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked;
            _cfgCommon.NewAllPop = NewPostPopMenuItem.Checked;
            _modifySettingCommon = true;
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = ListLockMenuItem.Checked;
            _cfgCommon.ListLock = ListLockMenuItem.Checked;
            _modifySettingCommon = true;
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
            if (_cfgLocal == null)
            {
                return;
            }

            DetailsListView lst = (DetailsListView)sender;
            if (_iconCol)
            {
                _cfgLocal.Width1 = lst.Columns[0].Width;
                _cfgLocal.Width3 = lst.Columns[1].Width;
            }
            else
            {
                int[] darr = new int[lst.Columns.Count];
                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    darr[lst.Columns[i].DisplayIndex] = i;
                }
                MyCommon.MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    switch (darr[i])
                    {
                        case 0:
                            _cfgLocal.DisplayIndex1 = i;
                            break;
                        case 1:
                            _cfgLocal.DisplayIndex2 = i;
                            break;
                        case 2:
                            _cfgLocal.DisplayIndex3 = i;
                            break;
                        case 3:
                            _cfgLocal.DisplayIndex4 = i;
                            break;
                        case 4:
                            _cfgLocal.DisplayIndex5 = i;
                            break;
                        case 5:
                            _cfgLocal.DisplayIndex6 = i;
                            break;
                        case 6:
                            _cfgLocal.DisplayIndex7 = i;
                            break;
                        case 7:
                            _cfgLocal.DisplayIndex8 = i;
                            break;
                    }
                }
                _cfgLocal.Width1 = lst.Columns[0].Width;
                _cfgLocal.Width2 = lst.Columns[1].Width;
                _cfgLocal.Width3 = lst.Columns[2].Width;
                _cfgLocal.Width4 = lst.Columns[3].Width;
                _cfgLocal.Width5 = lst.Columns[4].Width;
                _cfgLocal.Width6 = lst.Columns[5].Width;
                _cfgLocal.Width7 = lst.Columns[6].Width;
                _cfgLocal.Width8 = lst.Columns[7].Width;
            }
            _modifySettingLocal = true;
            _isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (_cfgLocal == null)
            {
                return;
            }
            DetailsListView lst = (DetailsListView)sender;
            if (_iconCol)
            {
                if (_cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    _cfgLocal.Width1 = lst.Columns[0].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[1].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
            else
            {
                if (_cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    _cfgLocal.Width1 = lst.Columns[0].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width2 = lst.Columns[1].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[2].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    _cfgLocal.Width4 = lst.Columns[3].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    _cfgLocal.Width5 = lst.Columns[4].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    _cfgLocal.Width6 = lst.Columns[5].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    _cfgLocal.Width7 = lst.Columns[6].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    _cfgLocal.Width8 = lst.Columns[7].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
        }

        public string WebBrowser_GetSelectionText(ref WebBrowser componentInstance)
        {
            //発言詳細で「選択文字列をコピー」を行う
            //WebBrowserコンポーネントのインスタンスを渡す
            Type typ = componentInstance.ActiveXInstance.GetType();
            object selObj = typ.InvokeMember("selection", BindingFlags.GetProperty, null, componentInstance.Document.DomDocument, null);
            object objRange = selObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, null, selObj, null);
            return (string)objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, null, objRange, null);
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細で「選択文字列をコピー」
            try
            {
                Clipboard.SetDataObject(WebBrowser_GetSelectionText(ref PostBrowser), false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void doSearchToolStrip(string url)
        {
            //発言詳細で「選択文字列で検索」（選択文字列取得）
            string selText = WebBrowser_GetSelectionText(ref PostBrowser);

            if (selText != null)
            {
                if (url == Hoehoe.Properties.Resources.SearchItem4Url)
                {
                    //公式検索
                    AddNewTabForSearch(selText);
                    return;
                }

                string tmp = string.Format(url, HttpUtility.UrlEncode(selText));
                OpenUriAsync(tmp);
            }
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細ですべて選択
            PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem1Url);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem2Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem3Url);
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            doSearchToolStrip(Hoehoe.Properties.Resources.SearchItem4Url);
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, "<a[^>]*href=\"(?<url>" + this._postBrowserStatusText.Replace(".", "\\.") + ")\"[^>]*title=\"(?<title>https?://[^\"]+)\"", RegexOptions.IgnoreCase);
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
                string name = GetUserId();
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

                UseHashtagMenuItem.Enabled = Regex.IsMatch(this._postBrowserStatusText, "^https?://twitter.com/search\\?q=%23");
            }
            else
            {
                this._postBrowserStatusText = "";
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
            string _selText = WebBrowser_GetSelectionText(ref PostBrowser);
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
            //発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            bool fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != tw.Username.ToLower())
                {
                    fAllFlag = true;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            this.FriendshipAllMenuItem.Enabled = fAllFlag;

            TranslationToolStripMenuItem.Enabled = _curPost != null;

            e.Cancel = false;
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細の選択文字列で現在のタブを検索
            string _selText = WebBrowser_GetSelectionText(ref PostBrowser);

            if (_selText != null)
            {
                SearchDialog.SWord = _selText;
                SearchDialog.CheckCaseSensitive = false;
                SearchDialog.CheckRegex = false;

                DoTabSearch(SearchDialog.SWord, SearchDialog.CheckCaseSensitive, SearchDialog.CheckRegex, SEARCHTYPE.NextSearch);
            }
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (StatusText.Multiline)
            {
                _mySpDis2 = StatusText.Height;
            }
            _modifySettingLocal = true;
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ImageSelectionPanel.Visible = true;
                ImageSelectionPanel.Enabled = true;
                TimelinePanel.Visible = false;
                TimelinePanel.Enabled = false;
                ImagefilePathText.Text = ((string[])(e.Data.GetData(DataFormats.FileDrop, false)))[0];
                ImageFromSelectedFile();
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
                string filename = ((string[])(e.Data.GetData(DataFormats.FileDrop, false)))[0];
                FileInfo fl = new FileInfo(filename);
                string ext = fl.Extension;

                if (!String.IsNullOrEmpty(this.ImageService) && this._pictureServices[this.ImageService].CheckValidFilesize(ext, fl.Length))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
                foreach (string svc in ImageServiceCombo.Items)
                {
                    if (String.IsNullOrEmpty(svc))
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
            RunAsync(new GetWorkerArg() { WorkerType = MyCommon.WorkerType.OpenUri, Url = uri });
        }

        private void ListTabSelect(TabPage tab)
        {
            SetListProperty();

            _itemCache = null;
            _itemCacheIndex = -1;
            _postCache = null;

            _curTab = tab;
            _curList = (DetailsListView)tab.Tag;
            if (_curList.SelectedIndices.Count > 0)
            {
                _curItemIndex = _curList.SelectedIndices[0];
                _curPost = GetCurTabPost(_curItemIndex);
            }
            else
            {
                _curItemIndex = -1;
                _curPost = null;
            }

            _anchorPost = null;
            _anchorFlag = false;

            if (_iconCol)
            {
                ((DetailsListView)tab.Tag).Columns[1].Text = _columnTexts[2];
            }
            else
            {
                for (int i = 0; i <= _curList.Columns.Count - 1; i++)
                {
                    ((DetailsListView)tab.Tag).Columns[i].Text = _columnTexts[i];
                }
            }
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            ListTabSelect(e.TabPage);
        }

        private void SelectListItem(DetailsListView dlView, int index)
        {
            //単一
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
            //LView.SelectedIndices.Add(Index)
            dlView.Items[index].Focused = true;

            if (flg)
            {
                dlView.Invalidate(bnd);
            }
        }

        private void SelectListItem(DetailsListView dlView, int[] indecies, int focused)
        {
            //複数
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
            if (args.WorkerType != MyCommon.WorkerType.Follower)
            {
                for (int i = 0; i < _bw.Length; i++)
                {
                    if (_bw[i] != null && !_bw[i].IsBusy)
                    {
                        bw = _bw[i];
                        break;
                    }
                }
                if (bw == null)
                {
                    for (int i = 0; i < _bw.Length; i++)
                    {
                        if (_bw[i] == null)
                        {
                            _bw[i] = new BackgroundWorker();
                            bw = _bw[i];
                            bw.WorkerReportsProgress = true;
                            bw.WorkerSupportsCancellation = true;
                            bw.DoWork += GetTimelineWorker_DoWork;
                            bw.ProgressChanged += GetTimelineWorker_ProgressChanged;
                            bw.RunWorkerCompleted += GetTimelineWorker_RunWorkerCompleted;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (_bwFollower == null)
                {
                    _bwFollower = new BackgroundWorker();
                    bw = _bwFollower;
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;
                    bw.DoWork += GetTimelineWorker_DoWork;
                    bw.ProgressChanged += GetTimelineWorker_ProgressChanged;
                    bw.RunWorkerCompleted += GetTimelineWorker_RunWorkerCompleted;
                }
                else
                {
                    if (_bwFollower.IsBusy == false)
                    {
                        bw = _bwFollower;
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
            tw.NewPostFromStream += tw_NewPostFromStream;
            tw.UserStreamStarted += tw_UserStreamStarted;
            tw.UserStreamStopped += tw_UserStreamStopped;
            tw.PostDeleted += tw_PostDeleted;
            tw.UserStreamEventReceived += tw_UserStreamEventArrived;

            MenuItemUserStream.Text = "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Start";
            StopToolStripMenuItem.Enabled = true;
            if (SettingDialog.UserstreamStartup)
            {
                tw.StartUserStream();
            }
        }

        private void TweenMain_Shown(object sender, EventArgs e)
        {
            try
            {
                PostBrowser.Url = new Uri("about:blank");
                PostBrowser.DocumentText = "";
                //発言詳細部初期化
            }
            catch (Exception)
            {
            }

            NotifyIcon1.Visible = true;
            tw.UserIdChanged += tw_UserIdChanged;

            if (MyCommon.IsNetworkAvailable())
            {
                GetTimeline(MyCommon.WorkerType.BlockIds, 0, 0, "");
                if (SettingDialog.StartupFollowers)
                {
                    GetTimeline(MyCommon.WorkerType.Follower, 0, 0, "");
                }
                GetTimeline(MyCommon.WorkerType.Configuration, 0, 0, "");
                StartUserStream();
                _waitTimeline = true;
                GetTimeline(MyCommon.WorkerType.Timeline, 1, 1, "");
                _waitReply = true;
                GetTimeline(MyCommon.WorkerType.Reply, 1, 1, "");
                _waitDm = true;
                GetTimeline(MyCommon.WorkerType.DirectMessegeRcv, 1, 1, "");
                if (SettingDialog.GetFav)
                {
                    _waitFav = true;
                    GetTimeline(MyCommon.WorkerType.Favorites, 1, 1, "");
                }
                _waitPubSearch = true;
                GetTimeline(MyCommon.WorkerType.PublicSearch, 1, 0, "");
                //tabname="":全タブ
                _waitUserTimeline = true;
                GetTimeline(MyCommon.WorkerType.UserTimeline, 1, 0, "");
                //tabname="":全タブ
                _waitLists = true;
                GetTimeline(MyCommon.WorkerType.List, 1, 0, "");
                //tabname="":全タブ
                int i = 0;
                int j = 0;
                while ((IsInitialRead()) && !MyCommon.IsEnding)
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

                //バージョンチェック（引数：起動時チェックの場合はTrue･･･チェック結果のメッセージを表示しない）
                if (SettingDialog.StartupVersion)
                {
                    CheckNewVersion(true);
                }

                // 取得失敗の場合は再試行する
                if (!tw.GetFollowersSuccess && SettingDialog.StartupFollowers)
                {
                    GetTimeline(MyCommon.WorkerType.Follower, 0, 0, "");
                }

                // 取得失敗の場合は再試行する
                if (SettingDialog.TwitterConfiguration.PhotoSizeLimit == 0)
                {
                    GetTimeline(MyCommon.WorkerType.Configuration, 0, 0, "");
                }

                // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
                if (MyCommon.TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ReAuthorizeText);
                    SettingStripMenuItem_Click(null, null);
                }

                //
            }
            _initial = false;

            TimerTimeline.Enabled = true;
        }

        private bool IsInitialRead()
        {
            return _waitTimeline || _waitReply || _waitDm || _waitFav || _waitPubSearch || _waitUserTimeline || _waitLists;
        }

        private void doGetFollowersMenu()
        {
            GetTimeline(MyCommon.WorkerType.Follower, 1, 0, "");
            DispSelectedPost(true);
        }

        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doGetFollowersMenu();
        }

        private void doReTweetUnofficial()
        {
            //RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = CreateRetweetUnofficial(_curPost.Text);
                StatusText.Text = "RT @" + _curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            doReTweetUnofficial();
        }

        private void doReTweetOfficial(bool isConfirm)
        {
            //公式RT
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    _DoFavRetweetFlags = false;
                    return;
                }
                if (_curList.SelectedIndices.Count > 15)
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.RetweetLimitText);
                    _DoFavRetweetFlags = false;
                    return;
                }
                else if (_curList.SelectedIndices.Count > 1)
                {
                    string QuestionText = Hoehoe.Properties.Resources.RetweetQuestion2;
                    if (_DoFavRetweetFlags)
                    {
                        QuestionText = Hoehoe.Properties.Resources.FavoriteRetweetQuestionText1;
                    }
                    switch (MessageBox.Show(QuestionText, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            _DoFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (_curPost.IsDm || _curPost.IsMe)
                    {
                        _DoFavRetweetFlags = false;
                        return;
                    }
                    if (!SettingDialog.RetweetNoConfirm)
                    {
                        string Questiontext = Hoehoe.Properties.Resources.RetweetQuestion1;
                        if (_DoFavRetweetFlags)
                        {
                            Questiontext = Hoehoe.Properties.Resources.FavoritesRetweetQuestionText2;
                        }
                        if (isConfirm && MessageBox.Show(Questiontext, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            _DoFavRetweetFlags = false;
                            return;
                        }
                    }
                }
                GetWorkerArg args = new GetWorkerArg()
                {
                    Ids = new List<long>(),
                    SIds = new List<long>(),
                    TabName = _curTab.Text,
                    WorkerType = MyCommon.WorkerType.Retweet
                };
                foreach (int idx in _curList.SelectedIndices)
                {
                    PostClass post = GetCurTabPost(idx);
                    if (!post.IsMe && !post.IsProtect && !post.IsDm)
                    {
                        args.Ids.Add(post.StatusId);
                    }
                }
                RunAsync(args);
            }
        }

        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            doReTweetOfficial(true);
        }

        private void FavoritesRetweetOriginal()
        {
            if (!this.ExistCurrentPost)
            {
                return;
            }
            _DoFavRetweetFlags = true;
            doReTweetOfficial(true);
            if (_DoFavRetweetFlags)
            {
                _DoFavRetweetFlags = false;
                FavoriteChange(true, false);
            }
        }

        private void FavoritesRetweetUnofficial()
        {
            if (this.ExistCurrentPost && !_curPost.IsDm)
            {
                _DoFavRetweetFlags = true;
                FavoriteChange(true);
                if (!_curPost.IsProtect && _DoFavRetweetFlags)
                {
                    _DoFavRetweetFlags = false;
                    doReTweetUnofficial();
                }
            }
        }

        private string CreateRetweetUnofficial(string status)
        {
            // Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
            //展開しないように変更
            //展開するか判定
            MatchCollection ms = Regex.Matches(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>");
            foreach (Match m in ms)
            {
                if (m.Result("${link}").EndsWith("..."))
                {
                    break;
                }
            }
            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\" title=\"(?<title>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${title}");
            //その他のリンク(@IDなど)を置き換える
            status = Regex.Replace(status, "@<a target=\"_self\" href=\"https?://twitter.com/(#!/)?(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "@${url}");
            //ハッシュタグ
            status = Regex.Replace(status, "<a target=\"_self\" href=\"(?<url>[^\"]+)\"[^>]*>(?<link>[^<]+)</a>", "${link}");
            //<br>タグ除去
            if (StatusText.Multiline)
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                status = Regex.Replace(status, "(\\r\\n|\\n|\\r)?<br>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            _replyToId = 0;
            _replyToName = "";
            status = status.Replace("&nbsp;", " ");

            return status;
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost != null)
            {
                DispSelectedPost(true);
            }
        }

        private bool isKeyDown(Keys key)
        {
            return (Control.ModifierKeys & key) == key;
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            if (MyCommon.DebugBuild || isKeyDown(Keys.CapsLock) && isKeyDown(Keys.Control) && isKeyDown(Keys.Shift))
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
            SettingDialog.UrlConvertAuto = ToolStripMenuItemUrlAutoShorten.Checked;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            ToolStripMenuItemUrlAutoShorten.Checked = SettingDialog.UrlConvertAuto;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.TraceFlag = TraceOutToolStripMenuItem.Checked;
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            //画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_Leave(StatusText, EventArgs.Empty);
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_rclickTabName))
            {
                return;
            }
            TabRename(ref _rclickTabName);
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.Bitly);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlConvert(MyCommon.UrlConverter.Jmp);
        }

        private class GetApiInfoArgs
        {
            public Twitter Tw;
            public ApiInfo Info;
        }

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = tw.GetInfoApi(args.Info);
        }

        private void ApiInfoMenuItem_Click(object sender, EventArgs e)
        {
            GetApiInfoArgs args = new GetApiInfoArgs
            {
                Tw = tw,
                Info = new ApiInfo()
            };

            StringBuilder tmp = new StringBuilder();
            using (FormInfo dlg = new FormInfo(this, Hoehoe.Properties.Resources.ApiInfo6, GetApiInfo_Dowork, null, args))
            {
                dlg.ShowDialog();
                if (Convert.ToBoolean(dlg.Result))
                {
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo1 + args.Info.MaxCount.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo2 + args.Info.RemainCount.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo3 + args.Info.ResetTime.ToString());
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo7 + (tw.UserStreamEnabled ? Hoehoe.Properties.Resources.Enable : Hoehoe.Properties.Resources.Disable).ToString());

                    tmp.AppendLine();
                    tmp.AppendLine(Hoehoe.Properties.Resources.ApiInfo8 + args.Info.AccessLevel.ToString());
                    SetStatusLabelUrl();

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
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
            FollowCommand(id);
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
                if (inputName.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs();
                    arg.Tw = tw;
                    arg.Id = inputName.TabName.Trim();
                    using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.FollowCommandText1, FollowCommand_DoWork, null, arg))
                    {
                        info.ShowDialog();
                        string ret = (string)info.Result;
                        if (!String.IsNullOrEmpty(ret))
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
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
            RemoveCommand(id, false);
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
            FollowRemoveCommandArgs arg = new FollowRemoveCommandArgs() { Tw = tw, Id = id };
            if (!skipInput)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Unfollow");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(inputName.TabName.Trim()))
                    {
                        arg.Tw = tw;
                        arg.Id = inputName.TabName.Trim();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.RemoveCommandText1, RemoveCommand_DoWork, null, arg))
            {
                info.ShowDialog();
                string ret = (string)info.Result;
                if (!String.IsNullOrEmpty(ret))
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
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
            ShowFriendship(id);
        }

        private class ShowFriendshipArgs
        {
            public Twitter Tw;

            public class FriendshipInfo
            {
                public string id = "";
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
            string result = "";
            foreach (ShowFriendshipArgs.FriendshipInfo fInfo in arg.ids)
            {
                string rt = arg.Tw.GetFriendshipInfo(fInfo.id, ref fInfo.isFollowing, ref fInfo.isFollowed);
                if (!String.IsNullOrEmpty(rt))
                {
                    if (String.IsNullOrEmpty(result))
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
            args.Tw = tw;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle("Show Friendships");
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    string ret = "";
                    args.ids.Add(new ShowFriendshipArgs.FriendshipInfo(inputName.TabName.Trim()));
                    using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, ShowFriendship_DoWork, null, args))
                    {
                        _info.ShowDialog();
                        ret = (string)_info.Result;
                    }
                    string result = "";
                    if (String.IsNullOrEmpty(ret))
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
                string ret = "";
                ShowFriendshipArgs args = new ShowFriendshipArgs();
                args.Tw = tw;
                args.ids.Add(new ShowFriendshipArgs.FriendshipInfo(id.Trim()));
                using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.ShowFriendshipText1, ShowFriendship_DoWork, null, args))
                {
                    _info.ShowDialog();
                    ret = (string)_info.Result;
                }
                string result = "";
                ShowFriendshipArgs.FriendshipInfo fInfo = args.ids[0];
                string ff = "";
                if (String.IsNullOrEmpty(ret))
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
                            RemoveCommand(fInfo.id, true);
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
            doShowUserStatus(tw.Username, false);
        }

        // TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        // URLから切り出した文字列を渡す

        public bool IsTwitterId(string name)
        {
            if (SettingDialog.TwitterConfiguration.NonUsernamePaths == null || SettingDialog.TwitterConfiguration.NonUsernamePaths.Length == 0)
            {
                return !Regex.Match(name, "^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            }
            else
            {
                return !SettingDialog.TwitterConfiguration.NonUsernamePaths.Contains(name.ToLower());
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this._postBrowserStatusText, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && IsTwitterId(m.Result("${ScreenName}")))
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
            string name = GetUserId();
            if (name != null)
            {
                FollowCommand(name);
            }
        }

        private void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                RemoveCommand(name, false);
            }
        }

        private void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                ShowFriendship(name);
            }
        }

        private void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, "href=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?\"");
            List<string> ids = new List<string>();
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != tw.Username.ToLower())
                {
                    ids.Add(mu.Result("${ScreenName}"));
                }
            }
            ShowFriendship(ids.ToArray());
        }

        private void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                ShowUserStatus(name);
            }
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                AddNewTabForUserTimeline(name);
            }
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                AddNewTabForSearch("@" + name);
            }
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _modifySettingCommon = true;
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            _modifySettingCommon = true;
        }

        private void doQuote()
        {
            //QT @id:内容
            //返信先情報付加
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm || !StatusText.Enabled)
                {
                    return;
                }

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = CreateRetweetUnofficial(_curPost.Text);

                StatusText.Text = " QT @" + _curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata);
                if (_curPost.RetweetedId == 0)
                {
                    _replyToId = _curPost.StatusId;
                }
                else
                {
                    _replyToId = _curPost.RetweetedId;
                }
                _replyToName = _curPost.ScreenName;

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            doQuote();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            //公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }
            string tbName = pnl.Parent.Text;
            TabClass tb = _statuses.Tabs[tbName];
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
            if (String.IsNullOrEmpty(cmb.Text))
            {
                ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
                SaveConfigsTabs();
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
                _statuses.ClearTabIds(tbName);
                SaveConfigsTabs();
                //検索条件の保存
            }

            GetTimeline(MyCommon.WorkerType.PublicSearch, 1, 0, tbName);
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            //もっと前を取得
            DoRefreshMore();
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            if (_statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TabClass tb = _statuses.RemovedTab.Pop();
                string renamed = tb.TabName;
                for (int i = 1; i <= int.MaxValue; i++)
                {
                    if (!_statuses.ContainsTab(renamed))
                    {
                        break;
                    }
                    renamed = tb.TabName + "(" + i.ToString() + ")";
                }
                tb.TabName = renamed;
                _statuses.Tabs.Add(renamed, tb);
                AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
                ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                SaveConfigsTabs();
            }
        }

        private void doMoveToRTHome()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                PostClass post = GetCurTabPost(_curList.SelectedIndices[0]);
                if (post.RetweetedId > 0)
                {
                    OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).RetweetedBy);
                }
            }
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            doMoveToRTHome();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                string tabName = "";

                //未選択なら処理終了
                if (_curList.SelectedIndices.Count == 0)
                {
                    return;
                }

                //タブ選択（or追加）
                if (!SelectTab(ref tabName))
                {
                    return;
                }

                bool mv = false;
                bool mk = false;
                MoveOrCopy(ref mv, ref mk);

                FiltersClass fc = new FiltersClass()
                {
                    NameFilter = name,
                    SearchBoth = true,
                    MoveFrom = mv,
                    SetMark = mk,
                    UseRegex = false,
                    SearchUrl = false
                };
                _statuses.Tabs[tabName].AddFilter(fc);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    _itemCache = null;
                    _postCache = null;
                    _curPost = null;
                    _curItemIndex = -1;
                    _statuses.FilterAll();
                    foreach (TabPage tb in ListTab.TabPages)
                    {
                        ((DetailsListView)tb.Tag).VirtualListSize = _statuses.Tabs[tb.Text].AllCount;
                        if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                        {
                            if (SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            if (SettingDialog.TabIconDisp)
                            {
                                tb.ImageIndex = -1;
                            }
                        }
                    }
                    if (!SettingDialog.TabIconDisp)
                    {
                        ListTab.Refresh();
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
                SaveConfigsTabs();
            }
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string user = null;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            if (object.ReferenceEquals(menuItem.Owner, this.ContextMenuPostBrowser))
            {
                user = GetUserId();
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

                if (!String.IsNullOrEmpty(res))
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
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != MyCommon.TabUsageType.PublicSearch)
                {
                    return;
                }
                ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(this._postBrowserStatusText, "^https?://twitter.com/search\\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                HashMgr.SetPermanentHash("#" + m.Result("${hash}"));
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
                //使用ハッシュタグとして設定
                _modifySettingCommon = true;
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
                rslt = HashMgr.ShowDialog();
            }
            catch (Exception)
            {
                return;
            }
            this.TopMost = SettingDialog.AlwaysTop;
            if (rslt == DialogResult.Cancel)
                return;
            if (!String.IsNullOrEmpty(HashMgr.UseHash))
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            _modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            HashMgr.ToggleHash();
            if (!String.IsNullOrEmpty(HashMgr.UseHash))
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            _modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            HashToggleMenuItem_Click(null, null);
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            if (ListTab.SelectedTab == null)
            {
                return;
            }
            if (_statuses == null || _statuses.Tabs == null || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text))
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

            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || _curPost.IsDm)
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
                if (this.ExistCurrentPost && _curPost.IsDm)
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
                //PublicSearchの時問題出るかも

                if (_curPost.IsMe)
                {
                    this.RtOpMenuItem.Enabled = false;
                    this.FavoriteRetweetMenuItem.Enabled = false;
                    this.DelOpMenuItem.Enabled = true;
                }
                else
                {
                    this.DelOpMenuItem.Enabled = false;
                    if (_curPost.IsProtect)
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

            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != MyCommon.TabUsageType.Favorites)
            {
                this.RefreshPrevOpMenuItem.Enabled = true;
            }
            else
            {
                this.RefreshPrevOpMenuItem.Enabled = false;
            }

            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch || !this.ExistCurrentPost || !(_curPost.InReplyToStatusId > 0))
            {
                OpenRepSourceOpMenuItem.Enabled = false;
            }
            else
            {
                OpenRepSourceOpMenuItem.Enabled = true;
            }

            if (!this.ExistCurrentPost || String.IsNullOrEmpty(_curPost.RetweetedBy))
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
            ContextMenuTabProperty_Opening(sender, null);
        }

        public Twitter TwitterInstance
        {
            get { return tw; }
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis3 = SplitContainer3.SplitterDistance;
                _modifySettingLocal = true;
            }
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            if (_statuses.RemovedTab.Count == 0)
            {
                UndoRemoveTabMenuItem.Enabled = false;
            }
            else
            {
                UndoRemoveTabMenuItem.Enabled = true;
            }
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
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
                if (_curPost.IsDm)
                {
                    this.CopyURLMenuItem.Enabled = false;
                }
                if (_curPost.IsProtect)
                {
                    this.CopySTOTMenuItem.Enabled = false;
                }
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            SetNotifyIconText();
        }

        private void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
            ShowUserStatus(id);
        }

        private class GetUserInfoArgs
        {
            public Hoehoe.Twitter tw;
            public string id;
            public TwitterDataModel.User user;
        }

        private void GetUserInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            GetUserInfoArgs args = (GetUserInfoArgs)e.Argument;
            e.Result = args.tw.GetUserInfo(args.id, ref args.user);
        }

        private void doShowUserStatus(string id, bool ShowInputDialog)
        {
            TwitterDataModel.User user = null;
            if (ShowInputDialog)
            {
                using (InputTabName inputName = new InputTabName())
                {
                    inputName.SetFormTitle("Show UserStatus");
                    inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                    inputName.TabName = id;
                    if (inputName.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(inputName.TabName.Trim()))
                    {
                        id = inputName.TabName.Trim();
                        GetUserInfoArgs args = new GetUserInfoArgs() { tw = tw, id = id, user = user };
                        using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, GetUserInfo_DoWork, null, args))
                        {
                            info.ShowDialog();
                            string ret = (string)info.Result;
                            if (String.IsNullOrEmpty(ret))
                            {
                                doShowUserStatus(args.user);
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
                GetUserInfoArgs args = new GetUserInfoArgs() { tw = tw, id = id, user = user };
                using (FormInfo info = new FormInfo(this, Hoehoe.Properties.Resources.doShowUserStatusText1, GetUserInfo_DoWork, null, args))
                {
                    info.ShowDialog();
                    string ret = (string)info.Result;
                    if (String.IsNullOrEmpty(ret))
                    {
                        doShowUserStatus(args.user);
                    }
                    else
                    {
                        MessageBox.Show(ret);
                    }
                }
            }
        }

        private void doShowUserStatus(TwitterDataModel.User user)
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
            doShowUserStatus(id, ShowInputDialog);
        }

        private void ShowUserStatus(string id)
        {
            doShowUserStatus(id, true);
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    FollowCommand(id);
                }
            }
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    RemoveCommand(id, false);
                }
            }
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    ShowFriendship(id);
                }
            }
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                ShowUserStatus(id, false);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                AddNewTabForUserTimeline(id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                AddNewTabForSearch("@" + id);
            }
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost != null)
            {
                ShowUserStatus(_curPost.ScreenName, false);
            }
        }

        private void GetRetweet_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;

            long statusid = 0;
            if (_curPost.RetweetedId > 0)
            {
                statusid = _curPost.RetweetedId;
            }
            else
            {
                statusid = _curPost.StatusId;
            }
            tw.GetStatus_Retweeted_Count(statusid, ref counter);

            e.Result = counter;
        }

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost)
            {
                using (FormInfo _info = new FormInfo(this, Hoehoe.Properties.Resources.RtCountMenuItem_ClickText1, GetRetweet_DoWork))
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
            get { return withEventsField__hookGlobalHotkey; }
            set
            {
                if (withEventsField__hookGlobalHotkey != null)
                {
                    withEventsField__hookGlobalHotkey.HotkeyPressed -= _hookGlobalHotkey_HotkeyPressed;
                }
                withEventsField__hookGlobalHotkey = value;
                if (withEventsField__hookGlobalHotkey != null)
                {
                    withEventsField__hookGlobalHotkey.HotkeyPressed += _hookGlobalHotkey_HotkeyPressed;
                }
            }
        }

        public TweenMain()
        {
            _hookGlobalHotkey = new HookGlobalHotkey(this);
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
                //アイコン化
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
                OpenUriAsync("http://twitter.com/" + NameLabel.Tag.ToString());
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MultiLineMenuItem.PerformClick();
        }

        public PostClass CurPost
        {
            get { return _curPost; }
        }

        public bool IsPreviewEnable
        {
            get { return SettingDialog.PreviewEnable; }
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
            if (String.IsNullOrEmpty(this.ImageService))
            {
                return;
            }
            OpenFileDialog1.Filter = this._pictureServices[this.ImageService].GetFileOpenDialogFilter();
            OpenFileDialog1.Title = Hoehoe.Properties.Resources.PickPictureDialog1;
            OpenFileDialog1.FileName = "";

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
            ImageFromSelectedFile();
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (ImageCancelButton.Focused)
            {
                ImagefilePathText.CausesValidation = false;
                return;
            }
            ImagefilePathText.Text = (ImagefilePathText.Text).Trim();
            if (String.IsNullOrEmpty(ImagefilePathText.Text))
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
            }
            else
            {
                ImageFromSelectedFile();
            }
        }

        private void ImageFromSelectedFile()
        {
            try
            {
                if (String.IsNullOrEmpty(ImagefilePathText.Text.Trim()) || String.IsNullOrEmpty(this.ImageService))
                {
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                    ImagefilePathText.Text = "";
                    return;
                }

                FileInfo fl = new FileInfo(ImagefilePathText.Text.Trim());
                if (!this._pictureServices[this.ImageService].CheckValidExtension(fl.Extension))
                {
                    //画像以外の形式
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                    ImagefilePathText.Text = "";
                    return;
                }

                if (!this._pictureServices[this.ImageService].CheckValidFilesize(fl.Extension, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                    ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                    ImagefilePathText.Text = "";
                    MessageBox.Show("File is too large.");
                    return;
                }

                switch (this._pictureServices[this.ImageService].GetFileType(fl.Extension))
                {
                    case MyCommon.UploadFileType.Invalid:
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                        ImagefilePathText.Text = "";
                        break;
                    case MyCommon.UploadFileType.Picture:
                        Image img = null;
                        using (FileStream fs = new FileStream(ImagefilePathText.Text, FileMode.Open, FileAccess.Read))
                        {
                            img = Image.FromStream(fs);
                            fs.Close();
                        }

                        ImageSelectedPicture.Image = (new HttpVarious()).CheckValidImage(img, img.Width, img.Height);
                        ImageSelectedPicture.Tag = MyCommon.UploadFileType.Picture;
                        break;
                    case MyCommon.UploadFileType.MultiMedia:
                        ImageSelectedPicture.Image = Hoehoe.Properties.Resources.MultiMediaImage;
                        ImageSelectedPicture.Tag = MyCommon.UploadFileType.MultiMedia;
                        break;
                    default:
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                        ImagefilePathText.Text = "";
                        break;
                }
            }
            catch (FileNotFoundException ex)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                ImagefilePathText.Text = "";
                MessageBox.Show("File not found.");
            }
            catch (Exception ex)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                ImagefilePathText.Text = "";
                MessageBox.Show("The type of this file is not image.");
            }
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
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
            string svc = "";
            if (ImageServiceCombo.SelectedIndex > -1)
                svc = ImageServiceCombo.SelectedItem.ToString();
            ImageServiceCombo.Items.Clear();
            ImageServiceCombo.Items.Add("TwitPic");
            ImageServiceCombo.Items.Add("img.ly");
            ImageServiceCombo.Items.Add("yfrog");
            ImageServiceCombo.Items.Add("lockerz");
            ImageServiceCombo.Items.Add("Twitter");

            if (String.IsNullOrEmpty(svc))
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
            if (ImageSelectedPicture.Tag != null && !String.IsNullOrEmpty(this.ImageService))
            {
                try
                {
                    FileInfo fi = new FileInfo(ImagefilePathText.Text.Trim());
                    if (!this._pictureServices[this.ImageService].CheckValidFilesize(fi.Extension, fi.Length))
                    {
                        ImagefilePathText.Text = "";
                        ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage;
                        ImageSelectedPicture.Tag = MyCommon.UploadFileType.Invalid;
                    }
                }
                catch (Exception ex)
                {
                }
                _modifySettingCommon = true;
                SaveConfigsAll(false);
                if (this.ImageService == "Twitter")
                {
                    this.StatusText_TextChanged(null, null);
                }
            }
        }

        #endregion "画像投稿"

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ListManage form = new ListManage(tw))
            {
                form.ShowDialog(this);
            }
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

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = Convert.ToString(SourceLinkLabel.Tag);
            if (!String.IsNullOrEmpty(link) && e.Button == MouseButtons.Left)
            {
                OpenUriAsync(link);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            string link = Convert.ToString(SourceLinkLabel.Tag);
            if (!String.IsNullOrEmpty(link))
            {
                StatusLabelUrl.Text = link;
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            SetStatusLabelUrl();
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost && !_curPost.IsDm)
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
            CopyUserId();
        }

        private void CopyUserId()
        {
            if (_curPost == null)
            {
                return;
            }
            string clstr = _curPost.ScreenName;
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
            TabClass backToTab = _curTab == null ? _statuses.Tabs[ListTab.SelectedTab.Text] : _statuses.Tabs[_curTab.Text];
            if (this.ExistCurrentPost && !_curPost.IsDm)
            {
                //PublicSearchも除外した方がよい？
                if (_statuses.GetTabByType(MyCommon.TabUsageType.Related) == null)
                {
                    const string TabName = "Related Tweets";
                    string tName = TabName;
                    if (!this.AddNewTab(tName, false, MyCommon.TabUsageType.Related))
                    {
                        for (int i = 2; i <= 100; i++)
                        {
                            tName = TabName + i.ToString();
                            if (this.AddNewTab(tName, false, MyCommon.TabUsageType.Related))
                            {
                                _statuses.AddTab(tName, MyCommon.TabUsageType.Related, null);
                                break;
                            }
                        }
                    }
                    else
                    {
                        _statuses.AddTab(tName, MyCommon.TabUsageType.Related, null);
                    }
                    _statuses.GetTabByName(tName).UnreadManage = false;
                    _statuses.GetTabByName(tName).Notify = false;
                }

                TabClass tb = _statuses.GetTabByType(MyCommon.TabUsageType.Related);
                tb.RelationTargetPost = _curPost;
                this.ClearTab(tb.TabName, false);
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (tb.TabName == ListTab.TabPages[i].Text)
                    {
                        ListTab.SelectedIndex = i;
                        ListTabSelect(ListTab.TabPages[i]);
                        break;
                    }
                }

                GetTimeline(MyCommon.WorkerType.Related, 1, 1, tb.TabName);
            }
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("キャッシュメモリ容量         : {0}bytes({1}MB)" + "\r\n", ((ImageDictionary)_TIconDic).CacheMemoryLimit, ((ImageDictionary)_TIconDic).CacheMemoryLimit / 1048576);
            buf.AppendFormat("物理メモリ使用割合           : {0}%" + "\r\n", ((ImageDictionary)_TIconDic).PhysicalMemoryLimit);
            buf.AppendFormat("キャッシュエントリ保持数     : {0}" + "\r\n", ((ImageDictionary)_TIconDic).CacheCount);
            buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + "\r\n", ((ImageDictionary)_TIconDic).CacheRemoveCount);
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
                        _statuses.RemovePostReserve(id);
                        if (_curTab != null && _statuses.Tabs[_curTab.Text].Contains(id))
                        {
                            _itemCache = null;
                            _itemCacheIndex = -1;
                            _postCache = null;
                            ((DetailsListView)_curTab.Tag).Update();
                            if (_curPost != null & _curPost.StatusId == id)
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
            if (SettingDialog.ReadOldPosts)
            {
                _statuses.SetRead();
                //新着時未読クリア
            }

            int rsltAddCount = _statuses.DistributePosts();
            lock (_syncObject)
            {
                DateTime tm = DateTime.Now;
                if (_tlTimestamps.ContainsKey(tm))
                {
                    _tlTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    _tlTimestamps.Add(tm, rsltAddCount);
                }
                DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                _tlCount = 0;
                foreach (System.DateTime key in _tlTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                    {
                        keys.Add(key);
                    }
                    else
                    {
                        _tlCount += _tlTimestamps[key];
                    }
                }
                foreach (DateTime key in keys)
                {
                    _tlTimestamps.Remove(key);
                }
                keys.Clear();
            }

            if (SettingDialog.UserstreamPeriodInt > 0)
                return;

            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<bool>(RefreshTimeline), true);
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
                    Invoke(new MethodInvoker(tw_UserStreamStarted));
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
                    Invoke(new MethodInvoker(tw_UserStreamStopped));
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
                    Invoke(new Action<Twitter.FormattedEvent>(tw_UserStreamEventArrived), ev);
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
            NotifyEvent(ev);
            if (ev.Event == "favorite" || ev.Event == "unfavorite")
            {
                if (_curTab != null && _statuses.Tabs[_curTab.Text].Contains(ev.Id))
                {
                    _itemCache = null;
                    _itemCacheIndex = -1;
                    _postCache = null;
                    ((DetailsListView)_curTab.Tag).Update();
                }
                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(tw.Username.ToLower()))
                {
                    RemovePostFromFavTab(new Int64[] { ev.Id });
                }
            }
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            //新着通知
            if (BalloonRequired(ev))
            {
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                StringBuilder title = new StringBuilder();
                if (SettingDialog.DispUsername)
                {
                    title.Append(tw.Username);
                    title.Append(" - ");
                }
                else
                {
                    //title.Clear()
                }
                title.Append("Tween [");
                title.Append(ev.Event.ToUpper());
                title.Append("] by ");
                if (!String.IsNullOrEmpty(ev.Username))
                {
                    title.Append(ev.Username.ToString());
                }
                else
                {
                    //title.Append("")
                }
                string text = null;
                if (!String.IsNullOrEmpty(ev.Target))
                {
                    //NotifyIcon1.BalloonTipText = ev.Target
                    text = ev.Target;
                }
                else
                {
                    //NotifyIcon1.BalloonTipText = " "
                    text = " ";
                }
                //NotifyIcon1.ShowBalloonTip(500)
                if (SettingDialog.IsNotifyUseGrowl)
                {
                    gh.Notify(GrowlHelper.NotifyType.UserStreamEvent, ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                    NotifyIcon1.BalloonTipTitle = title.ToString();
                    NotifyIcon1.BalloonTipText = text;
                    NotifyIcon1.ShowBalloonTip(500);
                }
            }

            //サウンド再生
            string snd = SettingDialog.EventSoundFile;
            if (!_initial && SettingDialog.PlaySound && !String.IsNullOrEmpty(snd))
            {
                if (Convert.ToBoolean(ev.Eventtype & SettingDialog.EventNotifyFlag) && IsMyEventNotityAsEventType(ev))
                {
                    try
                    {
                        string dir = MyCommon.GetAppDir();
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
                tw.StopUserStream();
            }
            else
            {
                tw.StartUserStream();
            }
        }

        string _prevTrackWord;

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackToolStripMenuItem.Checked)
            {
                using (InputTabName inputForm = new InputTabName())
                {
                    inputForm.TabName = _prevTrackWord;
                    inputForm.SetFormTitle("Input track word");
                    inputForm.SetFormDescription("Track word");
                    if (inputForm.ShowDialog() != DialogResult.OK)
                    {
                        TrackToolStripMenuItem.Checked = false;
                        return;
                    }
                    _prevTrackWord = inputForm.TabName.Trim();
                }
                if (_prevTrackWord != tw.TrackWord)
                {
                    tw.TrackWord = _prevTrackWord;
                    this._modifySettingCommon = true;
                    TrackToolStripMenuItem.Checked = !String.IsNullOrEmpty(_prevTrackWord);
                    tw.ReconnectUserStream();
                }
            }
            else
            {
                tw.TrackWord = "";
                tw.ReconnectUserStream();
            }
            this._modifySettingCommon = true;
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tw.AllAtReply = AllrepliesToolStripMenuItem.Checked;
            this._modifySettingCommon = true;
            tw.ReconnectUserStream();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            if (evtDialog == null || evtDialog.IsDisposed)
            {
                evtDialog = new EventViewerDialog();
                evtDialog.Owner = this;
                //親の中央に表示
                Point pos = evtDialog.Location;
                pos.X = Convert.ToInt32(this.Location.X + this.Size.Width / 2 - evtDialog.Size.Width / 2);
                pos.Y = Convert.ToInt32(this.Location.Y + this.Size.Height / 2 - evtDialog.Size.Height / 2);
                evtDialog.Location = pos;
            }
            evtDialog.EventSource = tw.StoredEvent;
            if (!evtDialog.Visible)
            {
                evtDialog.Show(this);
            }
            else
            {
                evtDialog.Activate();
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
            if (!String.IsNullOrEmpty(tw.Username))
            {
                OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + tw.Username + "/recent");
            }
        }

        private void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync("http://twitter.com/" + tw.Username);
        }

        private void doTranslation(string str)
        {
            Bing bing = new Bing();
            string buf = "";
            if (String.IsNullOrEmpty(str))
            {
                return;
            }
            string srclng = "";
            string dstlng = SettingDialog.TranslateLanguage;
            string msg = "";
            if (srclng != dstlng && bing.Translate("", dstlng, str, ref buf))
            {
                PostBrowser.DocumentText = createDetailHtml(buf);
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
            doTranslation(_curPost.TextFromApi);
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doTranslation(WebBrowser_GetSelectionText(ref PostBrowser));
        }

        private bool ExistCurrentPost
        {
            get { return _curPost != null && !_curPost.IsDeleted; }
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUserTimeline();
        }

        public bool FavEventChangeUnread
        {
            get { return SettingDialog.FavEventUnread; }
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
            using (InputTabName inputName = new InputTabName())
            {
                inputName.SetFormTitle(caption);
                inputName.SetFormDescription(Hoehoe.Properties.Resources.FRMessage1);
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    id = inputName.TabName.Trim();
                }
                else
                {
                    id = "";
                }
            }
            return id;
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = GetUserIdFromCurPostOrInput("Show UserTimeline");
            if (!String.IsNullOrEmpty(id))
            {
                AddNewTabForUserTimeline(id);
            }
        }

        private void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = GetUserIdFromCurPostOrInput("Show Favstar");
            if (!String.IsNullOrEmpty(id))
            {
                OpenUriAsync(Hoehoe.Properties.Resources.FavstarUrl + "users/" + id + "/recent");
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                _osResumed = true;
            }
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            if (isEnable)
            {
                tw.StartUserStream();
            }
            else
            {
                tw.StopUserStream();
            }
            TimerTimeline.Enabled = isEnable;
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            TimelineRefreshEnableChange(!StopRefreshAllMenuItem.Checked);
        }

        private void OpenUserAppointUrl()
        {
            if (SettingDialog.UserAppointUrl != null)
            {
                if (SettingDialog.UserAppointUrl.Contains("{ID}") || SettingDialog.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (_curPost != null)
                    {
                        string xUrl = SettingDialog.UserAppointUrl;
                        xUrl = xUrl.Replace("{ID}", _curPost.ScreenName);
                        if (_curPost.RetweetedId != 0)
                        {
                            xUrl = xUrl.Replace("{STATUS}", _curPost.RetweetedId.ToString());
                        }
                        else
                        {
                            xUrl = xUrl.Replace("{STATUS}", _curPost.StatusId.ToString());
                        }
                        OpenUriAsync(xUrl);
                    }
                }
                else
                {
                    OpenUriAsync(SettingDialog.UserAppointUrl);
                }
            }
        }

        private void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            OpenUserAppointUrl();
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
            if (_curPost == null || !ExistCurrentPost || _curPost.IsDm)
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
    }
}