<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppendSettingDialog
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("更新間隔")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("起動時の動作")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("取得件数")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("UserStream")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("基本", New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3, TreeNode4})
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ツイートの動作")
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("動作", New System.Windows.Forms.TreeNode() {TreeNode6})
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("リストの表示")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("表示", New System.Windows.Forms.TreeNode() {TreeNode8})
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("フォント2")
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("フォント", New System.Windows.Forms.TreeNode() {TreeNode10})
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("プロキシ")
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("通信", New System.Windows.Forms.TreeNode() {TreeNode12})
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.UserStreamPanel = New System.Windows.Forms.Panel()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.UserstreamPeriod = New System.Windows.Forms.TextBox()
        Me.Label83 = New System.Windows.Forms.Label()
        Me.Label70 = New System.Windows.Forms.Label()
        Me.StartupUserstreamCheck = New System.Windows.Forms.CheckBox()
        Me.ActionPanel = New System.Windows.Forms.Panel()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.HotkeyCheck = New System.Windows.Forms.CheckBox()
        Me.HotkeyCode = New System.Windows.Forms.Label()
        Me.HotkeyText = New System.Windows.Forms.TextBox()
        Me.HotkeyWin = New System.Windows.Forms.CheckBox()
        Me.HotkeyAlt = New System.Windows.Forms.CheckBox()
        Me.HotkeyShift = New System.Windows.Forms.CheckBox()
        Me.HotkeyCtrl = New System.Windows.Forms.CheckBox()
        Me.Label82 = New System.Windows.Forms.Label()
        Me.CheckHashSupple = New System.Windows.Forms.CheckBox()
        Me.Label79 = New System.Windows.Forms.Label()
        Me.CheckAtIdSupple = New System.Windows.Forms.CheckBox()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.CheckFavRestrict = New System.Windows.Forms.CheckBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.PlaySnd = New System.Windows.Forms.CheckBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.BrowserPathText = New System.Windows.Forms.TextBox()
        Me.UReadMng = New System.Windows.Forms.CheckBox()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.CheckCloseToExit = New System.Windows.Forms.CheckBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.CheckMinimizeToTray = New System.Windows.Forms.CheckBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.CheckReadOldPosts = New System.Windows.Forms.CheckBox()
        Me.FontPanel2 = New System.Windows.Forms.Panel()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.btnInputFont = New System.Windows.Forms.Button()
        Me.btnInputBackcolor = New System.Windows.Forms.Button()
        Me.btnAtTo = New System.Windows.Forms.Button()
        Me.btnListBack = New System.Windows.Forms.Button()
        Me.btnAtFromTarget = New System.Windows.Forms.Button()
        Me.btnAtTarget = New System.Windows.Forms.Button()
        Me.btnTarget = New System.Windows.Forms.Button()
        Me.btnAtSelf = New System.Windows.Forms.Button()
        Me.btnSelf = New System.Windows.Forms.Button()
        Me.lblInputFont = New System.Windows.Forms.Label()
        Me.lblInputBackcolor = New System.Windows.Forms.Label()
        Me.lblAtTo = New System.Windows.Forms.Label()
        Me.lblListBackcolor = New System.Windows.Forms.Label()
        Me.lblAtFromTarget = New System.Windows.Forms.Label()
        Me.lblAtTarget = New System.Windows.Forms.Label()
        Me.lblTarget = New System.Windows.Forms.Label()
        Me.lblAtSelf = New System.Windows.Forms.Label()
        Me.lblSelf = New System.Windows.Forms.Label()
        Me.ButtonBackToDefaultFontColor2 = New System.Windows.Forms.Button()
        Me.Label89 = New System.Windows.Forms.Label()
        Me.Label91 = New System.Windows.Forms.Label()
        Me.Label95 = New System.Windows.Forms.Label()
        Me.Label99 = New System.Windows.Forms.Label()
        Me.Label101 = New System.Windows.Forms.Label()
        Me.Label103 = New System.Windows.Forms.Label()
        Me.Label105 = New System.Windows.Forms.Label()
        Me.Label107 = New System.Windows.Forms.Label()
        Me.Label109 = New System.Windows.Forms.Label()
        Me.GetPeriodPanel = New System.Windows.Forms.Panel()
        Me.TimelinePeriod = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonApiCalc = New System.Windows.Forms.Button()
        Me.LabelPostAndGet = New System.Windows.Forms.Label()
        Me.LabelApiUsing = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.ListsPeriod = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PubSearchPeriod = New System.Windows.Forms.TextBox()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.ReplyPeriod = New System.Windows.Forms.TextBox()
        Me.CheckPostAndGet = New System.Windows.Forms.CheckBox()
        Me.CheckPeriodAdjust = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.DMPeriod = New System.Windows.Forms.TextBox()
        Me.TweetPrvPanel = New System.Windows.Forms.Panel()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.LabelDateTimeFormatApplied = New System.Windows.Forms.Label()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.CmbDateTimeFormat = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.IconSize = New System.Windows.Forms.ComboBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.CheckSortOrderLock = New System.Windows.Forms.CheckBox()
        Me.Label78 = New System.Windows.Forms.Label()
        Me.CheckShowGrid = New System.Windows.Forms.CheckBox()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.chkReadOwnPost = New System.Windows.Forms.CheckBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.chkUnreadStyle = New System.Windows.Forms.CheckBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.OneWayLv = New System.Windows.Forms.CheckBox()
        Me.PreviewPanel = New System.Windows.Forms.Panel()
        Me.ReplyIconStateCombo = New System.Windows.Forms.ComboBox()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.ChkNewMentionsBlink = New System.Windows.Forms.CheckBox()
        Me.chkTabIconDisp = New System.Windows.Forms.CheckBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.CheckPreviewEnable = New System.Windows.Forms.CheckBox()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.LanguageCombo = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CheckAlwaysTop = New System.Windows.Forms.CheckBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.CheckMonospace = New System.Windows.Forms.CheckBox()
        Me.Label68 = New System.Windows.Forms.Label()
        Me.CheckBalloonLimit = New System.Windows.Forms.CheckBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ComboDispTitle = New System.Windows.Forms.ComboBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.cmbNameBalloon = New System.Windows.Forms.ComboBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.CheckDispUsername = New System.Windows.Forms.CheckBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.CheckBox3 = New System.Windows.Forms.CheckBox()
        Me.ConnectionPanel = New System.Windows.Forms.Panel()
        Me.CheckNicoms = New System.Windows.Forms.CheckBox()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputzUrlmode = New System.Windows.Forms.ComboBox()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.TextBoxOutputzKey = New System.Windows.Forms.TextBox()
        Me.CheckOutputz = New System.Windows.Forms.CheckBox()
        Me.CheckEnableBasicAuth = New System.Windows.Forms.CheckBox()
        Me.TwitterSearchAPIText = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TwitterAPIText = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CheckUseSsl = New System.Windows.Forms.CheckBox()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.ConnectionTimeOut = New System.Windows.Forms.TextBox()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.GetCountPanel = New System.Windows.Forms.Panel()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.FavoritesTextCountApi = New System.Windows.Forms.TextBox()
        Me.SearchTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.FirstTextCountApi = New System.Windows.Forms.TextBox()
        Me.GetMoreTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.UseChangeGetCount = New System.Windows.Forms.CheckBox()
        Me.TextCountApiReply = New System.Windows.Forms.TextBox()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.TextCountApi = New System.Windows.Forms.TextBox()
        Me.BasedPanel = New System.Windows.Forms.Panel()
        Me.AuthBasicRadio = New System.Windows.Forms.RadioButton()
        Me.AuthOAuthRadio = New System.Windows.Forms.RadioButton()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.AuthClearButton = New System.Windows.Forms.Button()
        Me.AuthUserLabel = New System.Windows.Forms.Label()
        Me.AuthStateLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.AuthorizeButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Username = New System.Windows.Forms.TextBox()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.ProxyPanel = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.TextProxyPassword = New System.Windows.Forms.TextBox()
        Me.LabelProxyPassword = New System.Windows.Forms.Label()
        Me.TextProxyUser = New System.Windows.Forms.TextBox()
        Me.LabelProxyUser = New System.Windows.Forms.Label()
        Me.TextProxyPort = New System.Windows.Forms.TextBox()
        Me.LabelProxyPort = New System.Windows.Forms.Label()
        Me.TextProxyAddress = New System.Windows.Forms.TextBox()
        Me.LabelProxyAddress = New System.Windows.Forms.Label()
        Me.RadioProxySpecified = New System.Windows.Forms.RadioButton()
        Me.RadioProxyIE = New System.Windows.Forms.RadioButton()
        Me.RadioProxyNone = New System.Windows.Forms.RadioButton()
        Me.TweetActPanel = New System.Windows.Forms.Panel()
        Me.TextBitlyPw = New System.Windows.Forms.TextBox()
        Me.ComboBoxPostKeySelect = New System.Windows.Forms.ComboBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.CheckRetweetNoConfirm = New System.Windows.Forms.CheckBox()
        Me.TextBitlyId = New System.Windows.Forms.TextBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.CheckUseRecommendStatus = New System.Windows.Forms.CheckBox()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.StatusText = New System.Windows.Forms.TextBox()
        Me.ComboBoxAutoShortUrlFirst = New System.Windows.Forms.ComboBox()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.CheckTinyURL = New System.Windows.Forms.CheckBox()
        Me.CheckAutoConvertUrl = New System.Windows.Forms.CheckBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.FontPanel = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnRetweet = New System.Windows.Forms.Button()
        Me.lblRetweet = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.ButtonBackToDefaultFontColor = New System.Windows.Forms.Button()
        Me.btnDetailLink = New System.Windows.Forms.Button()
        Me.lblDetailLink = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.btnUnread = New System.Windows.Forms.Button()
        Me.lblUnread = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.btnDetailBack = New System.Windows.Forms.Button()
        Me.lblDetailBackcolor = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.btnDetail = New System.Windows.Forms.Button()
        Me.lblDetail = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.btnOWL = New System.Windows.Forms.Button()
        Me.lblOWL = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.btnFav = New System.Windows.Forms.Button()
        Me.lblFav = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnListFont = New System.Windows.Forms.Button()
        Me.lblListFont = New System.Windows.Forms.Label()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.StartupPanel = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.StartupReaded = New System.Windows.Forms.CheckBox()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.CheckStartupFollowers = New System.Windows.Forms.CheckBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.CheckStartupVersion = New System.Windows.Forms.CheckBox()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.chkGetFav = New System.Windows.Forms.CheckBox()
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.Cancel = New System.Windows.Forms.Button()
        Me.Save = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.UserStreamPanel.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.ActionPanel.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.FontPanel2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GetPeriodPanel.SuspendLayout()
        Me.TweetPrvPanel.SuspendLayout()
        Me.PreviewPanel.SuspendLayout()
        Me.ConnectionPanel.SuspendLayout()
        Me.GetCountPanel.SuspendLayout()
        Me.BasedPanel.SuspendLayout()
        Me.ProxyPanel.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TweetActPanel.SuspendLayout()
        Me.FontPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.StartupPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Top
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TreeView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Window
        Me.SplitContainer1.Panel2.Controls.Add(Me.BasedPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetPeriodPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetCountPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.StartupPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.UserStreamPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ActionPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetActPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.PreviewPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetPrvPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ConnectionPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ProxyPanel)
        Me.SplitContainer1.Size = New System.Drawing.Size(701, 368)
        Me.SplitContainer1.SplitterDistance = 172
        Me.SplitContainer1.TabIndex = 0
        '
        'TreeView1
        '
        Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView1.Location = New System.Drawing.Point(0, 0)
        Me.TreeView1.Name = "TreeView1"
        TreeNode1.Name = "PeriodNode"
        TreeNode1.Text = "更新間隔"
        TreeNode2.Name = "StartUpNode"
        TreeNode2.Text = "起動時の動作"
        TreeNode3.Name = "GetCountNode"
        TreeNode3.Text = "取得件数"
        TreeNode4.Name = "UserStreamNode"
        TreeNode4.Text = "UserStream"
        TreeNode5.Name = "BasedNode"
        TreeNode5.Text = "基本"
        TreeNode6.Name = "TweetActNode"
        TreeNode6.Text = "ツイートの動作"
        TreeNode7.Name = "ActionNode"
        TreeNode7.Text = "動作"
        TreeNode8.Name = "TweetPrvNode"
        TreeNode8.Text = "リストの表示"
        TreeNode9.Name = "PreviewNode"
        TreeNode9.Text = "表示"
        TreeNode10.Name = "FontNode2"
        TreeNode10.Text = "フォント2"
        TreeNode11.Name = "FontNode"
        TreeNode11.Text = "フォント"
        TreeNode12.Name = "ProxyNode"
        TreeNode12.Text = "プロキシ"
        TreeNode13.Name = "ConnectionNode"
        TreeNode13.Text = "通信"
        Me.TreeView1.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode5, TreeNode7, TreeNode9, TreeNode11, TreeNode13})
        Me.TreeView1.Size = New System.Drawing.Size(172, 368)
        Me.TreeView1.TabIndex = 0
        '
        'UserStreamPanel
        '
        Me.UserStreamPanel.Controls.Add(Me.GroupBox4)
        Me.UserStreamPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UserStreamPanel.Enabled = False
        Me.UserStreamPanel.Location = New System.Drawing.Point(0, 0)
        Me.UserStreamPanel.Name = "UserStreamPanel"
        Me.UserStreamPanel.Size = New System.Drawing.Size(525, 368)
        Me.UserStreamPanel.TabIndex = 44
        Me.UserStreamPanel.Visible = False
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.UserstreamPeriod)
        Me.GroupBox4.Controls.Add(Me.Label83)
        Me.GroupBox4.Controls.Add(Me.Label70)
        Me.GroupBox4.Controls.Add(Me.StartupUserstreamCheck)
        Me.GroupBox4.Location = New System.Drawing.Point(21, 20)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(387, 60)
        Me.GroupBox4.TabIndex = 43
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Userstream"
        '
        'UserstreamPeriod
        '
        Me.UserstreamPeriod.Location = New System.Drawing.Point(241, 36)
        Me.UserstreamPeriod.Name = "UserstreamPeriod"
        Me.UserstreamPeriod.Size = New System.Drawing.Size(65, 19)
        Me.UserstreamPeriod.TabIndex = 3
        '
        'Label83
        '
        Me.Label83.AutoSize = True
        Me.Label83.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label83.Location = New System.Drawing.Point(8, 39)
        Me.Label83.Name = "Label83"
        Me.Label83.Size = New System.Drawing.Size(145, 12)
        Me.Label83.TabIndex = 2
        Me.Label83.Text = "発言一覧への反映間隔（秒）"
        '
        'Label70
        '
        Me.Label70.AutoSize = True
        Me.Label70.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label70.Location = New System.Drawing.Point(8, 15)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(89, 12)
        Me.Label70.TabIndex = 0
        Me.Label70.Text = "起動時自動接続"
        '
        'StartupUserstreamCheck
        '
        Me.StartupUserstreamCheck.AutoSize = True
        Me.StartupUserstreamCheck.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.StartupUserstreamCheck.Location = New System.Drawing.Point(241, 14)
        Me.StartupUserstreamCheck.Name = "StartupUserstreamCheck"
        Me.StartupUserstreamCheck.Size = New System.Drawing.Size(67, 16)
        Me.StartupUserstreamCheck.TabIndex = 1
        Me.StartupUserstreamCheck.Text = "接続する"
        Me.StartupUserstreamCheck.UseVisualStyleBackColor = True
        '
        'ActionPanel
        '
        Me.ActionPanel.BackColor = System.Drawing.SystemColors.Window
        Me.ActionPanel.Controls.Add(Me.GroupBox3)
        Me.ActionPanel.Controls.Add(Me.Label82)
        Me.ActionPanel.Controls.Add(Me.CheckHashSupple)
        Me.ActionPanel.Controls.Add(Me.Label79)
        Me.ActionPanel.Controls.Add(Me.CheckAtIdSupple)
        Me.ActionPanel.Controls.Add(Me.Label57)
        Me.ActionPanel.Controls.Add(Me.Label56)
        Me.ActionPanel.Controls.Add(Me.CheckFavRestrict)
        Me.ActionPanel.Controls.Add(Me.Button3)
        Me.ActionPanel.Controls.Add(Me.PlaySnd)
        Me.ActionPanel.Controls.Add(Me.Label14)
        Me.ActionPanel.Controls.Add(Me.Label15)
        Me.ActionPanel.Controls.Add(Me.Label38)
        Me.ActionPanel.Controls.Add(Me.BrowserPathText)
        Me.ActionPanel.Controls.Add(Me.UReadMng)
        Me.ActionPanel.Controls.Add(Me.Label44)
        Me.ActionPanel.Controls.Add(Me.CheckCloseToExit)
        Me.ActionPanel.Controls.Add(Me.Label40)
        Me.ActionPanel.Controls.Add(Me.CheckMinimizeToTray)
        Me.ActionPanel.Controls.Add(Me.Label41)
        Me.ActionPanel.Controls.Add(Me.Label39)
        Me.ActionPanel.Controls.Add(Me.CheckReadOldPosts)
        Me.ActionPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ActionPanel.Enabled = False
        Me.ActionPanel.Location = New System.Drawing.Point(0, 0)
        Me.ActionPanel.Name = "ActionPanel"
        Me.ActionPanel.Size = New System.Drawing.Size(525, 368)
        Me.ActionPanel.TabIndex = 52
        Me.ActionPanel.Visible = False
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.HotkeyCheck)
        Me.GroupBox3.Controls.Add(Me.HotkeyCode)
        Me.GroupBox3.Controls.Add(Me.HotkeyText)
        Me.GroupBox3.Controls.Add(Me.HotkeyWin)
        Me.GroupBox3.Controls.Add(Me.HotkeyAlt)
        Me.GroupBox3.Controls.Add(Me.HotkeyShift)
        Me.GroupBox3.Controls.Add(Me.HotkeyCtrl)
        Me.GroupBox3.Location = New System.Drawing.Point(21, 269)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(474, 41)
        Me.GroupBox3.TabIndex = 76
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "ホットキー"
        '
        'HotkeyCheck
        '
        Me.HotkeyCheck.AutoSize = True
        Me.HotkeyCheck.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyCheck.Location = New System.Drawing.Point(4, 15)
        Me.HotkeyCheck.Name = "HotkeyCheck"
        Me.HotkeyCheck.Size = New System.Drawing.Size(48, 16)
        Me.HotkeyCheck.TabIndex = 0
        Me.HotkeyCheck.Text = "有効"
        Me.HotkeyCheck.UseVisualStyleBackColor = True
        '
        'HotkeyCode
        '
        Me.HotkeyCode.AutoSize = True
        Me.HotkeyCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.HotkeyCode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyCode.Location = New System.Drawing.Point(340, 16)
        Me.HotkeyCode.Name = "HotkeyCode"
        Me.HotkeyCode.Size = New System.Drawing.Size(13, 14)
        Me.HotkeyCode.TabIndex = 6
        Me.HotkeyCode.Text = "0"
        '
        'HotkeyText
        '
        Me.HotkeyText.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.HotkeyText.Location = New System.Drawing.Point(257, 13)
        Me.HotkeyText.Name = "HotkeyText"
        Me.HotkeyText.ReadOnly = True
        Me.HotkeyText.Size = New System.Drawing.Size(77, 19)
        Me.HotkeyText.TabIndex = 5
        '
        'HotkeyWin
        '
        Me.HotkeyWin.AutoSize = True
        Me.HotkeyWin.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyWin.Location = New System.Drawing.Point(211, 15)
        Me.HotkeyWin.Name = "HotkeyWin"
        Me.HotkeyWin.Size = New System.Drawing.Size(42, 16)
        Me.HotkeyWin.TabIndex = 4
        Me.HotkeyWin.Text = "Win"
        Me.HotkeyWin.UseVisualStyleBackColor = True
        '
        'HotkeyAlt
        '
        Me.HotkeyAlt.AutoSize = True
        Me.HotkeyAlt.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyAlt.Location = New System.Drawing.Point(168, 15)
        Me.HotkeyAlt.Name = "HotkeyAlt"
        Me.HotkeyAlt.Size = New System.Drawing.Size(39, 16)
        Me.HotkeyAlt.TabIndex = 3
        Me.HotkeyAlt.Text = "Alt"
        Me.HotkeyAlt.UseVisualStyleBackColor = True
        '
        'HotkeyShift
        '
        Me.HotkeyShift.AutoSize = True
        Me.HotkeyShift.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyShift.Location = New System.Drawing.Point(116, 15)
        Me.HotkeyShift.Name = "HotkeyShift"
        Me.HotkeyShift.Size = New System.Drawing.Size(48, 16)
        Me.HotkeyShift.TabIndex = 2
        Me.HotkeyShift.Text = "Shift"
        Me.HotkeyShift.UseVisualStyleBackColor = True
        '
        'HotkeyCtrl
        '
        Me.HotkeyCtrl.AutoSize = True
        Me.HotkeyCtrl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.HotkeyCtrl.Location = New System.Drawing.Point(69, 15)
        Me.HotkeyCtrl.Name = "HotkeyCtrl"
        Me.HotkeyCtrl.Size = New System.Drawing.Size(43, 16)
        Me.HotkeyCtrl.TabIndex = 1
        Me.HotkeyCtrl.Text = "Ctrl"
        Me.HotkeyCtrl.UseVisualStyleBackColor = True
        '
        'Label82
        '
        Me.Label82.AutoSize = True
        Me.Label82.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label82.Location = New System.Drawing.Point(26, 248)
        Me.Label82.Name = "Label82"
        Me.Label82.Size = New System.Drawing.Size(76, 12)
        Me.Label82.TabIndex = 74
        Me.Label82.Text = "#タグ入力補助"
        '
        'CheckHashSupple
        '
        Me.CheckHashSupple.AutoSize = True
        Me.CheckHashSupple.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckHashSupple.Location = New System.Drawing.Point(188, 247)
        Me.CheckHashSupple.Name = "CheckHashSupple"
        Me.CheckHashSupple.Size = New System.Drawing.Size(67, 16)
        Me.CheckHashSupple.TabIndex = 75
        Me.CheckHashSupple.Text = "使用する"
        Me.CheckHashSupple.UseVisualStyleBackColor = True
        '
        'Label79
        '
        Me.Label79.AutoSize = True
        Me.Label79.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label79.Location = New System.Drawing.Point(26, 226)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(72, 12)
        Me.Label79.TabIndex = 72
        Me.Label79.Text = "@ID入力補助"
        '
        'CheckAtIdSupple
        '
        Me.CheckAtIdSupple.AutoSize = True
        Me.CheckAtIdSupple.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckAtIdSupple.Location = New System.Drawing.Point(188, 225)
        Me.CheckAtIdSupple.Name = "CheckAtIdSupple"
        Me.CheckAtIdSupple.Size = New System.Drawing.Size(67, 16)
        Me.CheckAtIdSupple.TabIndex = 73
        Me.CheckAtIdSupple.Text = "使用する"
        Me.CheckAtIdSupple.UseVisualStyleBackColor = True
        '
        'Label57
        '
        Me.Label57.AutoSize = True
        Me.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label57.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label57.Location = New System.Drawing.Point(26, 210)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(340, 12)
        Me.Label57.TabIndex = 71
        Me.Label57.Text = "発言を再取得してFav結果を検証します。通信量が増えるのでOff推奨"
        '
        'Label56
        '
        Me.Label56.AutoSize = True
        Me.Label56.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label56.Location = New System.Drawing.Point(26, 192)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(103, 12)
        Me.Label56.TabIndex = 69
        Me.Label56.Text = "Fav結果厳密チェック"
        '
        'CheckFavRestrict
        '
        Me.CheckFavRestrict.AutoSize = True
        Me.CheckFavRestrict.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckFavRestrict.Location = New System.Drawing.Point(188, 191)
        Me.CheckFavRestrict.Name = "CheckFavRestrict"
        Me.CheckFavRestrict.Size = New System.Drawing.Size(74, 16)
        Me.CheckFavRestrict.TabIndex = 70
        Me.CheckFavRestrict.Text = "チェックする"
        Me.CheckFavRestrict.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Button3.Location = New System.Drawing.Point(418, 157)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 21)
        Me.Button3.TabIndex = 65
        Me.Button3.Text = "参照"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'PlaySnd
        '
        Me.PlaySnd.AutoSize = True
        Me.PlaySnd.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.PlaySnd.Location = New System.Drawing.Point(184, 17)
        Me.PlaySnd.Name = "PlaySnd"
        Me.PlaySnd.Size = New System.Drawing.Size(67, 16)
        Me.PlaySnd.TabIndex = 40
        Me.PlaySnd.Text = "再生する"
        Me.PlaySnd.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label14.Location = New System.Drawing.Point(22, 18)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(66, 12)
        Me.Label14.TabIndex = 39
        Me.Label14.Text = "サウンド再生"
        '
        'Label15
        '
        Me.Label15.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label15.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label15.Location = New System.Drawing.Point(22, 36)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(408, 22)
        Me.Label15.TabIndex = 41
        Me.Label15.Text = "タブのサウンドを設定した上で、「再生する」を選ぶとサウンドが再生されます。"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label38.Location = New System.Drawing.Point(24, 70)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(53, 12)
        Me.Label38.TabIndex = 42
        Me.Label38.Text = "未読管理"
        '
        'BrowserPathText
        '
        Me.BrowserPathText.Location = New System.Drawing.Point(184, 160)
        Me.BrowserPathText.Name = "BrowserPathText"
        Me.BrowserPathText.Size = New System.Drawing.Size(228, 19)
        Me.BrowserPathText.TabIndex = 64
        '
        'UReadMng
        '
        Me.UReadMng.AutoSize = True
        Me.UReadMng.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.UReadMng.Location = New System.Drawing.Point(184, 69)
        Me.UReadMng.Name = "UReadMng"
        Me.UReadMng.Size = New System.Drawing.Size(43, 16)
        Me.UReadMng.TabIndex = 43
        Me.UReadMng.Text = "する"
        Me.UReadMng.UseVisualStyleBackColor = True
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label44.Location = New System.Drawing.Point(21, 164)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(60, 12)
        Me.Label44.TabIndex = 63
        Me.Label44.Text = "ブラウザパス"
        '
        'CheckCloseToExit
        '
        Me.CheckCloseToExit.AutoSize = True
        Me.CheckCloseToExit.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckCloseToExit.Location = New System.Drawing.Point(184, 113)
        Me.CheckCloseToExit.Name = "CheckCloseToExit"
        Me.CheckCloseToExit.Size = New System.Drawing.Size(67, 16)
        Me.CheckCloseToExit.TabIndex = 47
        Me.CheckCloseToExit.Text = "終了する"
        Me.CheckCloseToExit.UseVisualStyleBackColor = True
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label40.Location = New System.Drawing.Point(22, 114)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(100, 12)
        Me.Label40.TabIndex = 46
        Me.Label40.Text = "×ボタンを押したとき"
        '
        'CheckMinimizeToTray
        '
        Me.CheckMinimizeToTray.AutoSize = True
        Me.CheckMinimizeToTray.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckMinimizeToTray.Location = New System.Drawing.Point(184, 135)
        Me.CheckMinimizeToTray.Name = "CheckMinimizeToTray"
        Me.CheckMinimizeToTray.Size = New System.Drawing.Size(90, 16)
        Me.CheckMinimizeToTray.TabIndex = 49
        Me.CheckMinimizeToTray.Text = "アイコン化する"
        Me.CheckMinimizeToTray.UseVisualStyleBackColor = True
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label41.Location = New System.Drawing.Point(22, 136)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(76, 12)
        Me.Label41.TabIndex = 48
        Me.Label41.Text = "最小化したとき"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label39.Location = New System.Drawing.Point(22, 92)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(89, 12)
        Me.Label39.TabIndex = 44
        Me.Label39.Text = "新着時未読クリア"
        '
        'CheckReadOldPosts
        '
        Me.CheckReadOldPosts.AutoSize = True
        Me.CheckReadOldPosts.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckReadOldPosts.Location = New System.Drawing.Point(184, 91)
        Me.CheckReadOldPosts.Name = "CheckReadOldPosts"
        Me.CheckReadOldPosts.Size = New System.Drawing.Size(43, 16)
        Me.CheckReadOldPosts.TabIndex = 45
        Me.CheckReadOldPosts.Text = "する"
        Me.CheckReadOldPosts.UseVisualStyleBackColor = True
        '
        'FontPanel2
        '
        Me.FontPanel2.Controls.Add(Me.GroupBox5)
        Me.FontPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FontPanel2.Enabled = False
        Me.FontPanel2.Location = New System.Drawing.Point(0, 0)
        Me.FontPanel2.Name = "FontPanel2"
        Me.FontPanel2.Size = New System.Drawing.Size(525, 368)
        Me.FontPanel2.TabIndex = 25
        Me.FontPanel2.Visible = False
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.btnInputFont)
        Me.GroupBox5.Controls.Add(Me.btnInputBackcolor)
        Me.GroupBox5.Controls.Add(Me.btnAtTo)
        Me.GroupBox5.Controls.Add(Me.btnListBack)
        Me.GroupBox5.Controls.Add(Me.btnAtFromTarget)
        Me.GroupBox5.Controls.Add(Me.btnAtTarget)
        Me.GroupBox5.Controls.Add(Me.btnTarget)
        Me.GroupBox5.Controls.Add(Me.btnAtSelf)
        Me.GroupBox5.Controls.Add(Me.btnSelf)
        Me.GroupBox5.Controls.Add(Me.lblInputFont)
        Me.GroupBox5.Controls.Add(Me.lblInputBackcolor)
        Me.GroupBox5.Controls.Add(Me.lblAtTo)
        Me.GroupBox5.Controls.Add(Me.lblListBackcolor)
        Me.GroupBox5.Controls.Add(Me.lblAtFromTarget)
        Me.GroupBox5.Controls.Add(Me.lblAtTarget)
        Me.GroupBox5.Controls.Add(Me.lblTarget)
        Me.GroupBox5.Controls.Add(Me.lblAtSelf)
        Me.GroupBox5.Controls.Add(Me.lblSelf)
        Me.GroupBox5.Controls.Add(Me.ButtonBackToDefaultFontColor2)
        Me.GroupBox5.Controls.Add(Me.Label89)
        Me.GroupBox5.Controls.Add(Me.Label91)
        Me.GroupBox5.Controls.Add(Me.Label95)
        Me.GroupBox5.Controls.Add(Me.Label99)
        Me.GroupBox5.Controls.Add(Me.Label101)
        Me.GroupBox5.Controls.Add(Me.Label103)
        Me.GroupBox5.Controls.Add(Me.Label105)
        Me.GroupBox5.Controls.Add(Me.Label107)
        Me.GroupBox5.Controls.Add(Me.Label109)
        Me.GroupBox5.Location = New System.Drawing.Point(22, 18)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(489, 290)
        Me.GroupBox5.TabIndex = 1
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "フォント＆色設定"
        '
        'btnInputFont
        '
        Me.btnInputFont.AutoSize = True
        Me.btnInputFont.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnInputFont.Location = New System.Drawing.Point(398, 216)
        Me.btnInputFont.Name = "btnInputFont"
        Me.btnInputFont.Size = New System.Drawing.Size(75, 22)
        Me.btnInputFont.TabIndex = 69
        Me.btnInputFont.Text = "フォント&&色"
        Me.btnInputFont.UseVisualStyleBackColor = True
        '
        'btnInputBackcolor
        '
        Me.btnInputBackcolor.AutoSize = True
        Me.btnInputBackcolor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnInputBackcolor.Location = New System.Drawing.Point(398, 191)
        Me.btnInputBackcolor.Name = "btnInputBackcolor"
        Me.btnInputBackcolor.Size = New System.Drawing.Size(75, 22)
        Me.btnInputBackcolor.TabIndex = 68
        Me.btnInputBackcolor.Text = "背景色"
        Me.btnInputBackcolor.UseVisualStyleBackColor = True
        '
        'btnAtTo
        '
        Me.btnAtTo.AutoSize = True
        Me.btnAtTo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnAtTo.Location = New System.Drawing.Point(398, 141)
        Me.btnAtTo.Name = "btnAtTo"
        Me.btnAtTo.Size = New System.Drawing.Size(75, 22)
        Me.btnAtTo.TabIndex = 66
        Me.btnAtTo.Text = "背景色"
        Me.btnAtTo.UseVisualStyleBackColor = True
        '
        'btnListBack
        '
        Me.btnListBack.AutoSize = True
        Me.btnListBack.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnListBack.Location = New System.Drawing.Point(398, 166)
        Me.btnListBack.Name = "btnListBack"
        Me.btnListBack.Size = New System.Drawing.Size(75, 22)
        Me.btnListBack.TabIndex = 67
        Me.btnListBack.Text = "背景色"
        Me.btnListBack.UseVisualStyleBackColor = True
        '
        'btnAtFromTarget
        '
        Me.btnAtFromTarget.AutoSize = True
        Me.btnAtFromTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnAtFromTarget.Location = New System.Drawing.Point(398, 116)
        Me.btnAtFromTarget.Name = "btnAtFromTarget"
        Me.btnAtFromTarget.Size = New System.Drawing.Size(75, 22)
        Me.btnAtFromTarget.TabIndex = 65
        Me.btnAtFromTarget.Text = "背景色"
        Me.btnAtFromTarget.UseVisualStyleBackColor = True
        '
        'btnAtTarget
        '
        Me.btnAtTarget.AutoSize = True
        Me.btnAtTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnAtTarget.Location = New System.Drawing.Point(398, 91)
        Me.btnAtTarget.Name = "btnAtTarget"
        Me.btnAtTarget.Size = New System.Drawing.Size(75, 22)
        Me.btnAtTarget.TabIndex = 64
        Me.btnAtTarget.Text = "背景色"
        Me.btnAtTarget.UseVisualStyleBackColor = True
        '
        'btnTarget
        '
        Me.btnTarget.AutoSize = True
        Me.btnTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnTarget.Location = New System.Drawing.Point(398, 66)
        Me.btnTarget.Name = "btnTarget"
        Me.btnTarget.Size = New System.Drawing.Size(75, 22)
        Me.btnTarget.TabIndex = 63
        Me.btnTarget.Text = "背景色"
        Me.btnTarget.UseVisualStyleBackColor = True
        '
        'btnAtSelf
        '
        Me.btnAtSelf.AutoSize = True
        Me.btnAtSelf.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnAtSelf.Location = New System.Drawing.Point(398, 41)
        Me.btnAtSelf.Name = "btnAtSelf"
        Me.btnAtSelf.Size = New System.Drawing.Size(75, 22)
        Me.btnAtSelf.TabIndex = 62
        Me.btnAtSelf.Text = "背景色"
        Me.btnAtSelf.UseVisualStyleBackColor = True
        '
        'btnSelf
        '
        Me.btnSelf.AutoSize = True
        Me.btnSelf.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSelf.Location = New System.Drawing.Point(398, 16)
        Me.btnSelf.Name = "btnSelf"
        Me.btnSelf.Size = New System.Drawing.Size(75, 22)
        Me.btnSelf.TabIndex = 61
        Me.btnSelf.Text = "背景色"
        Me.btnSelf.UseVisualStyleBackColor = True
        '
        'lblInputFont
        '
        Me.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputFont.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblInputFont.Location = New System.Drawing.Point(214, 220)
        Me.lblInputFont.Name = "lblInputFont"
        Me.lblInputFont.Size = New System.Drawing.Size(102, 19)
        Me.lblInputFont.TabIndex = 60
        Me.lblInputFont.Text = "This is sample."
        Me.lblInputFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblInputBackcolor
        '
        Me.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputBackcolor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblInputBackcolor.Location = New System.Drawing.Point(214, 195)
        Me.lblInputBackcolor.Name = "lblInputBackcolor"
        Me.lblInputBackcolor.Size = New System.Drawing.Size(102, 19)
        Me.lblInputBackcolor.TabIndex = 59
        Me.lblInputBackcolor.Text = "This is sample."
        Me.lblInputBackcolor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAtTo
        '
        Me.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAtTo.Location = New System.Drawing.Point(214, 145)
        Me.lblAtTo.Name = "lblAtTo"
        Me.lblAtTo.Size = New System.Drawing.Size(102, 19)
        Me.lblAtTo.TabIndex = 57
        Me.lblAtTo.Text = "This is sample."
        Me.lblAtTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblListBackcolor
        '
        Me.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListBackcolor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblListBackcolor.Location = New System.Drawing.Point(214, 170)
        Me.lblListBackcolor.Name = "lblListBackcolor"
        Me.lblListBackcolor.Size = New System.Drawing.Size(102, 19)
        Me.lblListBackcolor.TabIndex = 58
        Me.lblListBackcolor.Text = "This is sample."
        Me.lblListBackcolor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAtFromTarget
        '
        Me.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtFromTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAtFromTarget.Location = New System.Drawing.Point(214, 120)
        Me.lblAtFromTarget.Name = "lblAtFromTarget"
        Me.lblAtFromTarget.Size = New System.Drawing.Size(102, 19)
        Me.lblAtFromTarget.TabIndex = 56
        Me.lblAtFromTarget.Text = "This is sample."
        Me.lblAtFromTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAtTarget
        '
        Me.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAtTarget.Location = New System.Drawing.Point(214, 95)
        Me.lblAtTarget.Name = "lblAtTarget"
        Me.lblAtTarget.Size = New System.Drawing.Size(102, 19)
        Me.lblAtTarget.TabIndex = 55
        Me.lblAtTarget.Text = "This is sample."
        Me.lblAtTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTarget
        '
        Me.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTarget.Location = New System.Drawing.Point(214, 70)
        Me.lblTarget.Name = "lblTarget"
        Me.lblTarget.Size = New System.Drawing.Size(102, 19)
        Me.lblTarget.TabIndex = 54
        Me.lblTarget.Text = "This is sample."
        Me.lblTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAtSelf
        '
        Me.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtSelf.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAtSelf.Location = New System.Drawing.Point(214, 45)
        Me.lblAtSelf.Name = "lblAtSelf"
        Me.lblAtSelf.Size = New System.Drawing.Size(102, 19)
        Me.lblAtSelf.TabIndex = 53
        Me.lblAtSelf.Text = "This is sample."
        Me.lblAtSelf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSelf
        '
        Me.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSelf.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSelf.Location = New System.Drawing.Point(214, 19)
        Me.lblSelf.Name = "lblSelf"
        Me.lblSelf.Size = New System.Drawing.Size(102, 19)
        Me.lblSelf.TabIndex = 52
        Me.lblSelf.Text = "This is sample."
        Me.lblSelf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ButtonBackToDefaultFontColor2
        '
        Me.ButtonBackToDefaultFontColor2.AutoSize = True
        Me.ButtonBackToDefaultFontColor2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ButtonBackToDefaultFontColor2.Location = New System.Drawing.Point(191, 252)
        Me.ButtonBackToDefaultFontColor2.Name = "ButtonBackToDefaultFontColor2"
        Me.ButtonBackToDefaultFontColor2.Size = New System.Drawing.Size(90, 22)
        Me.ButtonBackToDefaultFontColor2.TabIndex = 51
        Me.ButtonBackToDefaultFontColor2.Text = "デフォルトに戻す"
        Me.ButtonBackToDefaultFontColor2.UseVisualStyleBackColor = True
        '
        'Label89
        '
        Me.Label89.AutoSize = True
        Me.Label89.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label89.Location = New System.Drawing.Point(9, 220)
        Me.Label89.Name = "Label89"
        Me.Label89.Size = New System.Drawing.Size(74, 12)
        Me.Label89.TabIndex = 48
        Me.Label89.Text = "入力欄フォント"
        '
        'Label91
        '
        Me.Label91.AutoSize = True
        Me.Label91.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label91.Location = New System.Drawing.Point(9, 195)
        Me.Label91.Name = "Label91"
        Me.Label91.Size = New System.Drawing.Size(131, 12)
        Me.Label91.TabIndex = 45
        Me.Label91.Text = "入力欄アクティブ時背景色"
        '
        'Label95
        '
        Me.Label95.AutoSize = True
        Me.Label95.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label95.Location = New System.Drawing.Point(9, 145)
        Me.Label95.Name = "Label95"
        Me.Label95.Size = New System.Drawing.Size(102, 12)
        Me.Label95.TabIndex = 39
        Me.Label95.Text = "その発言の@先発言"
        '
        'Label99
        '
        Me.Label99.AutoSize = True
        Me.Label99.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label99.Location = New System.Drawing.Point(9, 170)
        Me.Label99.Name = "Label99"
        Me.Label99.Size = New System.Drawing.Size(53, 12)
        Me.Label99.TabIndex = 42
        Me.Label99.Text = "一般発言"
        '
        'Label101
        '
        Me.Label101.AutoSize = True
        Me.Label101.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label101.Location = New System.Drawing.Point(9, 120)
        Me.Label101.Name = "Label101"
        Me.Label101.Size = New System.Drawing.Size(134, 12)
        Me.Label101.TabIndex = 36
        Me.Label101.Text = "その発言の@先の人の発言"
        '
        'Label103
        '
        Me.Label103.AutoSize = True
        Me.Label103.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label103.Location = New System.Drawing.Point(9, 95)
        Me.Label103.Name = "Label103"
        Me.Label103.Size = New System.Drawing.Size(88, 12)
        Me.Label103.TabIndex = 33
        Me.Label103.Text = "その人への@返信"
        '
        'Label105
        '
        Me.Label105.AutoSize = True
        Me.Label105.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label105.Location = New System.Drawing.Point(9, 70)
        Me.Label105.Name = "Label105"
        Me.Label105.Size = New System.Drawing.Size(70, 12)
        Me.Label105.TabIndex = 30
        Me.Label105.Text = "その人の発言"
        '
        'Label107
        '
        Me.Label107.AutoSize = True
        Me.Label107.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label107.Location = New System.Drawing.Point(9, 45)
        Me.Label107.Name = "Label107"
        Me.Label107.Size = New System.Drawing.Size(81, 12)
        Me.Label107.TabIndex = 27
        Me.Label107.Text = "自分への@返信"
        '
        'Label109
        '
        Me.Label109.AutoSize = True
        Me.Label109.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label109.Location = New System.Drawing.Point(9, 20)
        Me.Label109.Name = "Label109"
        Me.Label109.Size = New System.Drawing.Size(63, 12)
        Me.Label109.TabIndex = 24
        Me.Label109.Text = "自分の発言"
        '
        'GetPeriodPanel
        '
        Me.GetPeriodPanel.BackColor = System.Drawing.SystemColors.Window
        Me.GetPeriodPanel.Controls.Add(Me.TimelinePeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label3)
        Me.GetPeriodPanel.Controls.Add(Me.ButtonApiCalc)
        Me.GetPeriodPanel.Controls.Add(Me.LabelPostAndGet)
        Me.GetPeriodPanel.Controls.Add(Me.LabelApiUsing)
        Me.GetPeriodPanel.Controls.Add(Me.Label33)
        Me.GetPeriodPanel.Controls.Add(Me.ListsPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label7)
        Me.GetPeriodPanel.Controls.Add(Me.PubSearchPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label69)
        Me.GetPeriodPanel.Controls.Add(Me.ReplyPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.CheckPostAndGet)
        Me.GetPeriodPanel.Controls.Add(Me.CheckPeriodAdjust)
        Me.GetPeriodPanel.Controls.Add(Me.Label5)
        Me.GetPeriodPanel.Controls.Add(Me.DMPeriod)
        Me.GetPeriodPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GetPeriodPanel.Enabled = False
        Me.GetPeriodPanel.Location = New System.Drawing.Point(0, 0)
        Me.GetPeriodPanel.Name = "GetPeriodPanel"
        Me.GetPeriodPanel.Size = New System.Drawing.Size(525, 368)
        Me.GetPeriodPanel.TabIndex = 1
        Me.GetPeriodPanel.Visible = False
        '
        'TimelinePeriod
        '
        Me.TimelinePeriod.Location = New System.Drawing.Point(258, 21)
        Me.TimelinePeriod.Name = "TimelinePeriod"
        Me.TimelinePeriod.Size = New System.Drawing.Size(65, 19)
        Me.TimelinePeriod.TabIndex = 29
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(25, 24)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(130, 12)
        Me.Label3.TabIndex = 28
        Me.Label3.Text = "タイムライン更新間隔（秒）"
        '
        'ButtonApiCalc
        '
        Me.ButtonApiCalc.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ButtonApiCalc.Location = New System.Drawing.Point(258, 167)
        Me.ButtonApiCalc.Name = "ButtonApiCalc"
        Me.ButtonApiCalc.Size = New System.Drawing.Size(75, 23)
        Me.ButtonApiCalc.TabIndex = 41
        Me.ButtonApiCalc.Text = "再計算"
        Me.ButtonApiCalc.UseVisualStyleBackColor = True
        '
        'LabelPostAndGet
        '
        Me.LabelPostAndGet.AutoSize = True
        Me.LabelPostAndGet.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelPostAndGet.Location = New System.Drawing.Point(31, 207)
        Me.LabelPostAndGet.Name = "LabelPostAndGet"
        Me.LabelPostAndGet.Size = New System.Drawing.Size(285, 12)
        Me.LabelPostAndGet.TabIndex = 42
        Me.LabelPostAndGet.Text = "投稿時取得が有効のため、投稿のたびにAPIを消費します。"
        '
        'LabelApiUsing
        '
        Me.LabelApiUsing.AutoSize = True
        Me.LabelApiUsing.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelApiUsing.Location = New System.Drawing.Point(31, 175)
        Me.LabelApiUsing.Name = "LabelApiUsing"
        Me.LabelApiUsing.Size = New System.Drawing.Size(23, 12)
        Me.LabelApiUsing.TabIndex = 40
        Me.LabelApiUsing.Text = "999"
        Me.LabelApiUsing.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label33.Location = New System.Drawing.Point(25, 138)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(102, 12)
        Me.Label33.TabIndex = 38
        Me.Label33.Text = "Lists更新間隔（秒）"
        '
        'ListsPeriod
        '
        Me.ListsPeriod.Location = New System.Drawing.Point(258, 135)
        Me.ListsPeriod.Name = "ListsPeriod"
        Me.ListsPeriod.Size = New System.Drawing.Size(65, 19)
        Me.ListsPeriod.TabIndex = 39
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(25, 114)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(137, 12)
        Me.Label7.TabIndex = 36
        Me.Label7.Text = "Twitter検索更新間隔（秒）"
        '
        'PubSearchPeriod
        '
        Me.PubSearchPeriod.Location = New System.Drawing.Point(258, 111)
        Me.PubSearchPeriod.Name = "PubSearchPeriod"
        Me.PubSearchPeriod.Size = New System.Drawing.Size(65, 19)
        Me.PubSearchPeriod.TabIndex = 37
        '
        'Label69
        '
        Me.Label69.AutoSize = True
        Me.Label69.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label69.Location = New System.Drawing.Point(25, 66)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(123, 12)
        Me.Label69.TabIndex = 32
        Me.Label69.Text = "Mentions更新間隔（秒）"
        '
        'ReplyPeriod
        '
        Me.ReplyPeriod.Location = New System.Drawing.Point(258, 63)
        Me.ReplyPeriod.Name = "ReplyPeriod"
        Me.ReplyPeriod.Size = New System.Drawing.Size(65, 19)
        Me.ReplyPeriod.TabIndex = 33
        '
        'CheckPostAndGet
        '
        Me.CheckPostAndGet.AutoSize = True
        Me.CheckPostAndGet.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckPostAndGet.Location = New System.Drawing.Point(35, 43)
        Me.CheckPostAndGet.Name = "CheckPostAndGet"
        Me.CheckPostAndGet.Size = New System.Drawing.Size(84, 16)
        Me.CheckPostAndGet.TabIndex = 30
        Me.CheckPostAndGet.Text = "投稿時取得"
        Me.CheckPostAndGet.UseVisualStyleBackColor = True
        '
        'CheckPeriodAdjust
        '
        Me.CheckPeriodAdjust.AutoSize = True
        Me.CheckPeriodAdjust.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckPeriodAdjust.Location = New System.Drawing.Point(252, 43)
        Me.CheckPeriodAdjust.Name = "CheckPeriodAdjust"
        Me.CheckPeriodAdjust.Size = New System.Drawing.Size(91, 16)
        Me.CheckPeriodAdjust.TabIndex = 31
        Me.CheckPeriodAdjust.Text = "自動調整する"
        Me.CheckPeriodAdjust.UseVisualStyleBackColor = True
        Me.CheckPeriodAdjust.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(25, 90)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(94, 12)
        Me.Label5.TabIndex = 34
        Me.Label5.Text = "DM更新間隔（秒）"
        '
        'DMPeriod
        '
        Me.DMPeriod.Location = New System.Drawing.Point(258, 87)
        Me.DMPeriod.Name = "DMPeriod"
        Me.DMPeriod.Size = New System.Drawing.Size(65, 19)
        Me.DMPeriod.TabIndex = 35
        '
        'TweetPrvPanel
        '
        Me.TweetPrvPanel.Controls.Add(Me.Label47)
        Me.TweetPrvPanel.Controls.Add(Me.LabelDateTimeFormatApplied)
        Me.TweetPrvPanel.Controls.Add(Me.Label62)
        Me.TweetPrvPanel.Controls.Add(Me.CmbDateTimeFormat)
        Me.TweetPrvPanel.Controls.Add(Me.Label23)
        Me.TweetPrvPanel.Controls.Add(Me.Label11)
        Me.TweetPrvPanel.Controls.Add(Me.IconSize)
        Me.TweetPrvPanel.Controls.Add(Me.TextBox3)
        Me.TweetPrvPanel.Controls.Add(Me.Label21)
        Me.TweetPrvPanel.Controls.Add(Me.CheckSortOrderLock)
        Me.TweetPrvPanel.Controls.Add(Me.Label78)
        Me.TweetPrvPanel.Controls.Add(Me.CheckShowGrid)
        Me.TweetPrvPanel.Controls.Add(Me.Label73)
        Me.TweetPrvPanel.Controls.Add(Me.chkReadOwnPost)
        Me.TweetPrvPanel.Controls.Add(Me.Label17)
        Me.TweetPrvPanel.Controls.Add(Me.chkUnreadStyle)
        Me.TweetPrvPanel.Controls.Add(Me.Label16)
        Me.TweetPrvPanel.Controls.Add(Me.OneWayLv)
        Me.TweetPrvPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TweetPrvPanel.Enabled = False
        Me.TweetPrvPanel.Location = New System.Drawing.Point(0, 0)
        Me.TweetPrvPanel.Name = "TweetPrvPanel"
        Me.TweetPrvPanel.Size = New System.Drawing.Size(525, 368)
        Me.TweetPrvPanel.TabIndex = 86
        Me.TweetPrvPanel.Visible = False
        '
        'Label47
        '
        Me.Label47.AutoSize = True
        Me.Label47.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label47.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label47.Location = New System.Drawing.Point(214, 134)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(131, 12)
        Me.Label47.TabIndex = 104
        Me.Label47.Text = "再起動後有効になります。"
        '
        'LabelDateTimeFormatApplied
        '
        Me.LabelDateTimeFormatApplied.AutoSize = True
        Me.LabelDateTimeFormatApplied.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelDateTimeFormatApplied.Location = New System.Drawing.Point(264, 90)
        Me.LabelDateTimeFormatApplied.Name = "LabelDateTimeFormatApplied"
        Me.LabelDateTimeFormatApplied.Size = New System.Drawing.Size(44, 12)
        Me.LabelDateTimeFormatApplied.TabIndex = 100
        Me.LabelDateTimeFormatApplied.Text = "Label63"
        '
        'Label62
        '
        Me.Label62.AutoSize = True
        Me.Label62.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label62.Location = New System.Drawing.Point(214, 90)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(44, 12)
        Me.Label62.TabIndex = 99
        Me.Label62.Text = "Sample:"
        '
        'CmbDateTimeFormat
        '
        Me.CmbDateTimeFormat.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CmbDateTimeFormat.Items.AddRange(New Object() {"yyyy/MM/dd H:mm:ss", "yy/M/d H:mm:ss", "H:mm:ss yy/M/d", "M/d H:mm:ss", "M/d H:mm", "H:mm:ss M/d", "H:mm:ss", "H:mm", "tt h:mm", "M/d tt h:mm:ss", "M/d tt h:mm"})
        Me.CmbDateTimeFormat.Location = New System.Drawing.Point(216, 67)
        Me.CmbDateTimeFormat.Name = "CmbDateTimeFormat"
        Me.CmbDateTimeFormat.Size = New System.Drawing.Size(199, 20)
        Me.CmbDateTimeFormat.TabIndex = 98
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label23.Location = New System.Drawing.Point(25, 70)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(113, 12)
        Me.Label23.TabIndex = 97
        Me.Label23.Text = "リストの日時フォーマット"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(25, 108)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(163, 12)
        Me.Label11.TabIndex = 101
        Me.Label11.Text = "リストのアイコンサイズ（初期値16）"
        '
        'IconSize
        '
        Me.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.IconSize.FormattingEnabled = True
        Me.IconSize.Items.AddRange(New Object() {"none", "16*16", "24*24", "48*48", "48*48(2Column)"})
        Me.IconSize.Location = New System.Drawing.Point(252, 105)
        Me.IconSize.Name = "IconSize"
        Me.IconSize.Size = New System.Drawing.Size(161, 20)
        Me.IconSize.TabIndex = 103
        '
        'TextBox3
        '
        Me.TextBox3.Enabled = False
        Me.TextBox3.Location = New System.Drawing.Point(216, 106)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(34, 19)
        Me.TextBox3.TabIndex = 102
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label21.Location = New System.Drawing.Point(27, 202)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(44, 12)
        Me.Label21.TabIndex = 95
        Me.Label21.Text = "ソート順"
        '
        'CheckSortOrderLock
        '
        Me.CheckSortOrderLock.AutoSize = True
        Me.CheckSortOrderLock.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckSortOrderLock.Location = New System.Drawing.Point(218, 202)
        Me.CheckSortOrderLock.Name = "CheckSortOrderLock"
        Me.CheckSortOrderLock.Size = New System.Drawing.Size(67, 16)
        Me.CheckSortOrderLock.TabIndex = 96
        Me.CheckSortOrderLock.Text = "ロックする"
        Me.CheckSortOrderLock.UseVisualStyleBackColor = True
        '
        'Label78
        '
        Me.Label78.AutoSize = True
        Me.Label78.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label78.Location = New System.Drawing.Point(27, 180)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(73, 12)
        Me.Label78.TabIndex = 93
        Me.Label78.Text = "リスト区切り線"
        '
        'CheckShowGrid
        '
        Me.CheckShowGrid.AutoSize = True
        Me.CheckShowGrid.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckShowGrid.Location = New System.Drawing.Point(218, 180)
        Me.CheckShowGrid.Name = "CheckShowGrid"
        Me.CheckShowGrid.Size = New System.Drawing.Size(67, 16)
        Me.CheckShowGrid.TabIndex = 94
        Me.CheckShowGrid.Text = "表示する"
        Me.CheckShowGrid.UseVisualStyleBackColor = True
        '
        'Label73
        '
        Me.Label73.AutoSize = True
        Me.Label73.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label73.Location = New System.Drawing.Point(27, 159)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(87, 12)
        Me.Label73.TabIndex = 91
        Me.Label73.Text = "自発言の既読化"
        '
        'chkReadOwnPost
        '
        Me.chkReadOwnPost.AutoSize = True
        Me.chkReadOwnPost.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkReadOwnPost.Location = New System.Drawing.Point(218, 158)
        Me.chkReadOwnPost.Name = "chkReadOwnPost"
        Me.chkReadOwnPost.Size = New System.Drawing.Size(76, 16)
        Me.chkReadOwnPost.TabIndex = 92
        Me.chkReadOwnPost.Text = "既読にする"
        Me.chkReadOwnPost.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label17.Location = New System.Drawing.Point(26, 43)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(158, 12)
        Me.Label17.TabIndex = 83
        Me.Label17.Text = "未読スタイル（フォント＆色）適用"
        '
        'chkUnreadStyle
        '
        Me.chkUnreadStyle.AutoSize = True
        Me.chkUnreadStyle.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkUnreadStyle.Location = New System.Drawing.Point(217, 42)
        Me.chkUnreadStyle.Name = "chkUnreadStyle"
        Me.chkUnreadStyle.Size = New System.Drawing.Size(67, 16)
        Me.chkUnreadStyle.TabIndex = 84
        Me.chkUnreadStyle.Text = "適用する"
        Me.chkUnreadStyle.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label16.Location = New System.Drawing.Point(26, 21)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(97, 12)
        Me.Label16.TabIndex = 81
        Me.Label16.Text = "片思い色分け表示"
        '
        'OneWayLv
        '
        Me.OneWayLv.AutoSize = True
        Me.OneWayLv.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.OneWayLv.Location = New System.Drawing.Point(217, 20)
        Me.OneWayLv.Name = "OneWayLv"
        Me.OneWayLv.Size = New System.Drawing.Size(43, 16)
        Me.OneWayLv.TabIndex = 82
        Me.OneWayLv.Text = "する"
        Me.OneWayLv.UseVisualStyleBackColor = True
        '
        'PreviewPanel
        '
        Me.PreviewPanel.Controls.Add(Me.ReplyIconStateCombo)
        Me.PreviewPanel.Controls.Add(Me.Label72)
        Me.PreviewPanel.Controls.Add(Me.Label43)
        Me.PreviewPanel.Controls.Add(Me.Label48)
        Me.PreviewPanel.Controls.Add(Me.ChkNewMentionsBlink)
        Me.PreviewPanel.Controls.Add(Me.chkTabIconDisp)
        Me.PreviewPanel.Controls.Add(Me.Label35)
        Me.PreviewPanel.Controls.Add(Me.CheckPreviewEnable)
        Me.PreviewPanel.Controls.Add(Me.Label81)
        Me.PreviewPanel.Controls.Add(Me.LanguageCombo)
        Me.PreviewPanel.Controls.Add(Me.Label13)
        Me.PreviewPanel.Controls.Add(Me.CheckAlwaysTop)
        Me.PreviewPanel.Controls.Add(Me.Label58)
        Me.PreviewPanel.Controls.Add(Me.Label75)
        Me.PreviewPanel.Controls.Add(Me.CheckMonospace)
        Me.PreviewPanel.Controls.Add(Me.Label68)
        Me.PreviewPanel.Controls.Add(Me.CheckBalloonLimit)
        Me.PreviewPanel.Controls.Add(Me.Label10)
        Me.PreviewPanel.Controls.Add(Me.ComboDispTitle)
        Me.PreviewPanel.Controls.Add(Me.Label45)
        Me.PreviewPanel.Controls.Add(Me.cmbNameBalloon)
        Me.PreviewPanel.Controls.Add(Me.Label46)
        Me.PreviewPanel.Controls.Add(Me.CheckDispUsername)
        Me.PreviewPanel.Controls.Add(Me.Label25)
        Me.PreviewPanel.Controls.Add(Me.CheckBox3)
        Me.PreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PreviewPanel.Enabled = False
        Me.PreviewPanel.Location = New System.Drawing.Point(0, 0)
        Me.PreviewPanel.Name = "PreviewPanel"
        Me.PreviewPanel.Size = New System.Drawing.Size(525, 368)
        Me.PreviewPanel.TabIndex = 50
        Me.PreviewPanel.Visible = False
        '
        'ReplyIconStateCombo
        '
        Me.ReplyIconStateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ReplyIconStateCombo.FormattingEnabled = True
        Me.ReplyIconStateCombo.Items.AddRange(New Object() {"通知なし", "アイコン変更", "アイコン変更＆点滅"})
        Me.ReplyIconStateCombo.Location = New System.Drawing.Point(217, 142)
        Me.ReplyIconStateCombo.Name = "ReplyIconStateCombo"
        Me.ReplyIconStateCombo.Size = New System.Drawing.Size(121, 20)
        Me.ReplyIconStateCombo.TabIndex = 94
        '
        'Label72
        '
        Me.Label72.AutoSize = True
        Me.Label72.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label72.Location = New System.Drawing.Point(26, 145)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(134, 12)
        Me.Label72.TabIndex = 93
        Me.Label72.Text = "未読Mentions通知アイコン"
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label43.Location = New System.Drawing.Point(26, 169)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(135, 12)
        Me.Label43.TabIndex = 95
        Me.Label43.Text = "新着Mentions時画面点滅"
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label48.Location = New System.Drawing.Point(26, 121)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(115, 12)
        Me.Label48.TabIndex = 91
        Me.Label48.Text = "タブの未読アイコン表示"
        '
        'ChkNewMentionsBlink
        '
        Me.ChkNewMentionsBlink.AutoSize = True
        Me.ChkNewMentionsBlink.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ChkNewMentionsBlink.Location = New System.Drawing.Point(217, 168)
        Me.ChkNewMentionsBlink.Name = "ChkNewMentionsBlink"
        Me.ChkNewMentionsBlink.Size = New System.Drawing.Size(67, 16)
        Me.ChkNewMentionsBlink.TabIndex = 96
        Me.ChkNewMentionsBlink.Text = "点滅する"
        Me.ChkNewMentionsBlink.UseVisualStyleBackColor = True
        '
        'chkTabIconDisp
        '
        Me.chkTabIconDisp.AutoSize = True
        Me.chkTabIconDisp.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkTabIconDisp.Location = New System.Drawing.Point(217, 120)
        Me.chkTabIconDisp.Name = "chkTabIconDisp"
        Me.chkTabIconDisp.Size = New System.Drawing.Size(67, 16)
        Me.chkTabIconDisp.TabIndex = 92
        Me.chkTabIconDisp.Text = "表示する"
        Me.chkTabIconDisp.UseVisualStyleBackColor = True
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label35.Location = New System.Drawing.Point(26, 193)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(126, 12)
        Me.Label35.TabIndex = 59
        Me.Label35.Text = "画像リンクサムネイル表示"
        '
        'CheckPreviewEnable
        '
        Me.CheckPreviewEnable.AutoSize = True
        Me.CheckPreviewEnable.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckPreviewEnable.Location = New System.Drawing.Point(217, 192)
        Me.CheckPreviewEnable.Name = "CheckPreviewEnable"
        Me.CheckPreviewEnable.Size = New System.Drawing.Size(67, 16)
        Me.CheckPreviewEnable.TabIndex = 60
        Me.CheckPreviewEnable.Text = "使用する"
        Me.CheckPreviewEnable.UseVisualStyleBackColor = True
        '
        'Label81
        '
        Me.Label81.AutoSize = True
        Me.Label81.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label81.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label81.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label81.Location = New System.Drawing.Point(87, 290)
        Me.Label81.Name = "Label81"
        Me.Label81.Size = New System.Drawing.Size(115, 12)
        Me.Label81.TabIndex = 84
        Me.Label81.Text = "Apply after restarting"
        '
        'LanguageCombo
        '
        Me.LanguageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LanguageCombo.FormattingEnabled = True
        Me.LanguageCombo.Items.AddRange(New Object() {"OS Default", "Japanese", "English", "Simplified Chinese"})
        Me.LanguageCombo.Location = New System.Drawing.Point(213, 290)
        Me.LanguageCombo.Name = "LanguageCombo"
        Me.LanguageCombo.Size = New System.Drawing.Size(121, 20)
        Me.LanguageCombo.TabIndex = 85
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label13.Location = New System.Drawing.Point(26, 290)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(53, 12)
        Me.Label13.TabIndex = 83
        Me.Label13.Text = "Language"
        '
        'CheckAlwaysTop
        '
        Me.CheckAlwaysTop.AutoSize = True
        Me.CheckAlwaysTop.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckAlwaysTop.Location = New System.Drawing.Point(217, 265)
        Me.CheckAlwaysTop.Name = "CheckAlwaysTop"
        Me.CheckAlwaysTop.Size = New System.Drawing.Size(112, 16)
        Me.CheckAlwaysTop.TabIndex = 82
        Me.CheckAlwaysTop.Text = "最前面に表示する"
        Me.CheckAlwaysTop.UseVisualStyleBackColor = True
        '
        'Label58
        '
        Me.Label58.AutoSize = True
        Me.Label58.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label58.Location = New System.Drawing.Point(26, 266)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(86, 12)
        Me.Label58.TabIndex = 81
        Me.Label58.Text = "常に最前面表示"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label75.Location = New System.Drawing.Point(26, 237)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(162, 12)
        Me.Label75.TabIndex = 63
        Me.Label75.Text = "発言詳細表示フォント（AA対応）"
        '
        'CheckMonospace
        '
        Me.CheckMonospace.AutoSize = True
        Me.CheckMonospace.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckMonospace.Location = New System.Drawing.Point(217, 236)
        Me.CheckMonospace.Name = "CheckMonospace"
        Me.CheckMonospace.Size = New System.Drawing.Size(171, 16)
        Me.CheckMonospace.TabIndex = 64
        Me.CheckMonospace.Text = "等幅（フォント適用不具合あり）"
        Me.CheckMonospace.UseVisualStyleBackColor = True
        '
        'Label68
        '
        Me.Label68.AutoSize = True
        Me.Label68.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label68.Location = New System.Drawing.Point(24, 67)
        Me.Label68.Name = "Label68"
        Me.Label68.Size = New System.Drawing.Size(92, 12)
        Me.Label68.TabIndex = 47
        Me.Label68.Text = "バルーン表示制限"
        '
        'CheckBalloonLimit
        '
        Me.CheckBalloonLimit.AutoSize = True
        Me.CheckBalloonLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckBalloonLimit.Location = New System.Drawing.Point(215, 66)
        Me.CheckBalloonLimit.Name = "CheckBalloonLimit"
        Me.CheckBalloonLimit.Size = New System.Drawing.Size(182, 16)
        Me.CheckBalloonLimit.TabIndex = 48
        Me.CheckBalloonLimit.Text = "画面最小化・アイコン時のみ表示"
        Me.CheckBalloonLimit.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(24, 21)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(130, 12)
        Me.Label10.TabIndex = 43
        Me.Label10.Text = "新着バルーンのユーザー名"
        '
        'ComboDispTitle
        '
        Me.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDispTitle.FormattingEnabled = True
        Me.ComboDispTitle.Items.AddRange(New Object() {"（なし）", "バージョン", "最終発言", "＠未読数", "未読数", "未読数(@未読数)", "全未読/全発言数", "発言数/フォロー数/フォロワー数"})
        Me.ComboDispTitle.Location = New System.Drawing.Point(215, 88)
        Me.ComboDispTitle.Name = "ComboDispTitle"
        Me.ComboDispTitle.Size = New System.Drawing.Size(197, 20)
        Me.ComboDispTitle.TabIndex = 50
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label45.Location = New System.Drawing.Point(24, 91)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(60, 12)
        Me.Label45.TabIndex = 49
        Me.Label45.Text = "タイトルバー"
        '
        'cmbNameBalloon
        '
        Me.cmbNameBalloon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNameBalloon.FormattingEnabled = True
        Me.cmbNameBalloon.Items.AddRange(New Object() {"なし", "ユーザーID", "ニックネーム"})
        Me.cmbNameBalloon.Location = New System.Drawing.Point(215, 18)
        Me.cmbNameBalloon.Name = "cmbNameBalloon"
        Me.cmbNameBalloon.Size = New System.Drawing.Size(100, 20)
        Me.cmbNameBalloon.TabIndex = 44
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label46.Location = New System.Drawing.Point(24, 45)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(122, 12)
        Me.Label46.TabIndex = 45
        Me.Label46.Text = "タイトルバーとツールチップ"
        '
        'CheckDispUsername
        '
        Me.CheckDispUsername.AutoSize = True
        Me.CheckDispUsername.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckDispUsername.Location = New System.Drawing.Point(215, 44)
        Me.CheckDispUsername.Name = "CheckDispUsername"
        Me.CheckDispUsername.Size = New System.Drawing.Size(109, 16)
        Me.CheckDispUsername.TabIndex = 46
        Me.CheckDispUsername.Text = "ユーザー名を表示"
        Me.CheckDispUsername.UseVisualStyleBackColor = True
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Enabled = False
        Me.Label25.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label25.Location = New System.Drawing.Point(26, 215)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(134, 12)
        Me.Label25.TabIndex = 61
        Me.Label25.Text = "発言詳細部のアイコン表示"
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Enabled = False
        Me.CheckBox3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckBox3.Location = New System.Drawing.Point(217, 214)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(67, 16)
        Me.CheckBox3.TabIndex = 62
        Me.CheckBox3.Text = "表示する"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'ConnectionPanel
        '
        Me.ConnectionPanel.Controls.Add(Me.CheckNicoms)
        Me.ConnectionPanel.Controls.Add(Me.Label60)
        Me.ConnectionPanel.Controls.Add(Me.ComboBoxOutputzUrlmode)
        Me.ConnectionPanel.Controls.Add(Me.Label59)
        Me.ConnectionPanel.Controls.Add(Me.TextBoxOutputzKey)
        Me.ConnectionPanel.Controls.Add(Me.CheckOutputz)
        Me.ConnectionPanel.Controls.Add(Me.CheckEnableBasicAuth)
        Me.ConnectionPanel.Controls.Add(Me.TwitterSearchAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label31)
        Me.ConnectionPanel.Controls.Add(Me.TwitterAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label8)
        Me.ConnectionPanel.Controls.Add(Me.CheckUseSsl)
        Me.ConnectionPanel.Controls.Add(Me.Label64)
        Me.ConnectionPanel.Controls.Add(Me.ConnectionTimeOut)
        Me.ConnectionPanel.Controls.Add(Me.Label63)
        Me.ConnectionPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConnectionPanel.Enabled = False
        Me.ConnectionPanel.Location = New System.Drawing.Point(0, 0)
        Me.ConnectionPanel.Name = "ConnectionPanel"
        Me.ConnectionPanel.Size = New System.Drawing.Size(525, 368)
        Me.ConnectionPanel.TabIndex = 53
        Me.ConnectionPanel.Visible = False
        '
        'CheckNicoms
        '
        Me.CheckNicoms.AutoSize = True
        Me.CheckNicoms.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckNicoms.Location = New System.Drawing.Point(23, 295)
        Me.CheckNicoms.Name = "CheckNicoms"
        Me.CheckNicoms.Size = New System.Drawing.Size(237, 16)
        Me.CheckNicoms.TabIndex = 24
        Me.CheckNicoms.Text = "ニコニコ動画のURLをnico.msで短縮して送信"
        Me.CheckNicoms.UseVisualStyleBackColor = True
        '
        'Label60
        '
        Me.Label60.AutoSize = True
        Me.Label60.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label60.Location = New System.Drawing.Point(36, 245)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(99, 12)
        Me.Label60.TabIndex = 22
        Me.Label60.Text = "アウトプット先のURL"
        '
        'ComboBoxOutputzUrlmode
        '
        Me.ComboBoxOutputzUrlmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputzUrlmode.FormattingEnabled = True
        Me.ComboBoxOutputzUrlmode.Items.AddRange(New Object() {"twitter.com", "twitter.com/username"})
        Me.ComboBoxOutputzUrlmode.Location = New System.Drawing.Point(205, 242)
        Me.ComboBoxOutputzUrlmode.Name = "ComboBoxOutputzUrlmode"
        Me.ComboBoxOutputzUrlmode.Size = New System.Drawing.Size(182, 20)
        Me.ComboBoxOutputzUrlmode.TabIndex = 23
        '
        'Label59
        '
        Me.Label59.AutoSize = True
        Me.Label59.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label59.Location = New System.Drawing.Point(36, 199)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(63, 12)
        Me.Label59.TabIndex = 20
        Me.Label59.Text = "復活の呪文"
        '
        'TextBoxOutputzKey
        '
        Me.TextBoxOutputzKey.Location = New System.Drawing.Point(205, 196)
        Me.TextBoxOutputzKey.Name = "TextBoxOutputzKey"
        Me.TextBoxOutputzKey.Size = New System.Drawing.Size(182, 19)
        Me.TextBoxOutputzKey.TabIndex = 21
        '
        'CheckOutputz
        '
        Me.CheckOutputz.AutoSize = True
        Me.CheckOutputz.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckOutputz.Location = New System.Drawing.Point(23, 162)
        Me.CheckOutputz.Name = "CheckOutputz"
        Me.CheckOutputz.Size = New System.Drawing.Size(115, 16)
        Me.CheckOutputz.TabIndex = 19
        Me.CheckOutputz.Text = "Outputzに対応する"
        Me.CheckOutputz.UseVisualStyleBackColor = True
        '
        'CheckEnableBasicAuth
        '
        Me.CheckEnableBasicAuth.AutoSize = True
        Me.CheckEnableBasicAuth.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckEnableBasicAuth.Location = New System.Drawing.Point(22, 329)
        Me.CheckEnableBasicAuth.Name = "CheckEnableBasicAuth"
        Me.CheckEnableBasicAuth.Size = New System.Drawing.Size(178, 16)
        Me.CheckEnableBasicAuth.TabIndex = 18
        Me.CheckEnableBasicAuth.Text = "BASIC認証への変更を許可する"
        Me.CheckEnableBasicAuth.UseVisualStyleBackColor = True
        '
        'TwitterSearchAPIText
        '
        Me.TwitterSearchAPIText.Location = New System.Drawing.Point(262, 125)
        Me.TwitterSearchAPIText.Name = "TwitterSearchAPIText"
        Me.TwitterSearchAPIText.Size = New System.Drawing.Size(125, 19)
        Me.TwitterSearchAPIText.TabIndex = 17
        Me.TwitterSearchAPIText.Text = "search.twitter.com"
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label31.Location = New System.Drawing.Point(21, 128)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(228, 12)
        Me.Label31.TabIndex = 16
        Me.Label31.Text = "Twitter SearchAPI URL (search.twitter.com)"
        '
        'TwitterAPIText
        '
        Me.TwitterAPIText.Location = New System.Drawing.Point(262, 100)
        Me.TwitterAPIText.Name = "TwitterAPIText"
        Me.TwitterAPIText.Size = New System.Drawing.Size(125, 19)
        Me.TwitterAPIText.TabIndex = 15
        Me.TwitterAPIText.Text = "api.twitter.com"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(21, 103)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(174, 12)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Twitter API URL (api.twitter.com)"
        '
        'CheckUseSsl
        '
        Me.CheckUseSsl.AutoSize = True
        Me.CheckUseSsl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckUseSsl.Location = New System.Drawing.Point(23, 78)
        Me.CheckUseSsl.Name = "CheckUseSsl"
        Me.CheckUseSsl.Size = New System.Drawing.Size(145, 16)
        Me.CheckUseSsl.TabIndex = 13
        Me.CheckUseSsl.Text = "通信にHTTPSを使用する"
        Me.CheckUseSsl.UseVisualStyleBackColor = True
        '
        'Label64
        '
        Me.Label64.AutoSize = True
        Me.Label64.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label64.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label64.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label64.Location = New System.Drawing.Point(21, 51)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(349, 12)
        Me.Label64.TabIndex = 12
        Me.Label64.Text = "※タイムアウトが頻発する場合に調整してください。初期設定は20秒です。"
        '
        'ConnectionTimeOut
        '
        Me.ConnectionTimeOut.Location = New System.Drawing.Point(262, 18)
        Me.ConnectionTimeOut.Name = "ConnectionTimeOut"
        Me.ConnectionTimeOut.Size = New System.Drawing.Size(123, 19)
        Me.ConnectionTimeOut.TabIndex = 11
        '
        'Label63
        '
        Me.Label63.AutoSize = True
        Me.Label63.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label63.Location = New System.Drawing.Point(21, 21)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(131, 12)
        Me.Label63.TabIndex = 10
        Me.Label63.Text = "タイムアウトまでの時間(秒)"
        '
        'GetCountPanel
        '
        Me.GetCountPanel.BackColor = System.Drawing.SystemColors.Window
        Me.GetCountPanel.Controls.Add(Me.Label30)
        Me.GetCountPanel.Controls.Add(Me.Label28)
        Me.GetCountPanel.Controls.Add(Me.Label19)
        Me.GetCountPanel.Controls.Add(Me.FavoritesTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.SearchTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label66)
        Me.GetCountPanel.Controls.Add(Me.FirstTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.GetMoreTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label53)
        Me.GetCountPanel.Controls.Add(Me.UseChangeGetCount)
        Me.GetCountPanel.Controls.Add(Me.TextCountApiReply)
        Me.GetCountPanel.Controls.Add(Me.Label67)
        Me.GetCountPanel.Controls.Add(Me.TextCountApi)
        Me.GetCountPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GetCountPanel.Enabled = False
        Me.GetCountPanel.Location = New System.Drawing.Point(0, 0)
        Me.GetCountPanel.Name = "GetCountPanel"
        Me.GetCountPanel.Size = New System.Drawing.Size(525, 368)
        Me.GetCountPanel.TabIndex = 44
        Me.GetCountPanel.Visible = False
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Location = New System.Drawing.Point(28, 192)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(117, 12)
        Me.Label30.TabIndex = 52
        Me.Label30.Text = "PublicSearchの取得数"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(28, 144)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(63, 12)
        Me.Label28.TabIndex = 51
        Me.Label28.Text = "初回の更新"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(31, 58)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(87, 12)
        Me.Label19.TabIndex = 50
        Me.Label19.Text = "Mentions取得数"
        '
        'FavoritesTextCountApi
        '
        Me.FavoritesTextCountApi.Location = New System.Drawing.Point(259, 159)
        Me.FavoritesTextCountApi.Name = "FavoritesTextCountApi"
        Me.FavoritesTextCountApi.Size = New System.Drawing.Size(58, 19)
        Me.FavoritesTextCountApi.TabIndex = 48
        '
        'SearchTextCountApi
        '
        Me.SearchTextCountApi.Location = New System.Drawing.Point(259, 185)
        Me.SearchTextCountApi.Name = "SearchTextCountApi"
        Me.SearchTextCountApi.Size = New System.Drawing.Size(58, 19)
        Me.SearchTextCountApi.TabIndex = 49
        '
        'Label66
        '
        Me.Label66.AutoSize = True
        Me.Label66.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label66.Location = New System.Drawing.Point(28, 166)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(99, 12)
        Me.Label66.TabIndex = 47
        Me.Label66.Text = "Favoritesの取得数"
        '
        'FirstTextCountApi
        '
        Me.FirstTextCountApi.Location = New System.Drawing.Point(259, 135)
        Me.FirstTextCountApi.Name = "FirstTextCountApi"
        Me.FirstTextCountApi.Size = New System.Drawing.Size(58, 19)
        Me.FirstTextCountApi.TabIndex = 46
        '
        'GetMoreTextCountApi
        '
        Me.GetMoreTextCountApi.Location = New System.Drawing.Point(259, 112)
        Me.GetMoreTextCountApi.Name = "GetMoreTextCountApi"
        Me.GetMoreTextCountApi.Size = New System.Drawing.Size(59, 19)
        Me.GetMoreTextCountApi.TabIndex = 45
        '
        'Label53
        '
        Me.Label53.AutoSize = True
        Me.Label53.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label53.Location = New System.Drawing.Point(28, 118)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(79, 12)
        Me.Label53.TabIndex = 44
        Me.Label53.Text = "前データの更新"
        '
        'UseChangeGetCount
        '
        Me.UseChangeGetCount.AutoSize = True
        Me.UseChangeGetCount.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.UseChangeGetCount.Location = New System.Drawing.Point(30, 89)
        Me.UseChangeGetCount.Name = "UseChangeGetCount"
        Me.UseChangeGetCount.Size = New System.Drawing.Size(247, 16)
        Me.UseChangeGetCount.TabIndex = 43
        Me.UseChangeGetCount.Text = "次の項目の更新時の取得数を個別に設定する"
        Me.UseChangeGetCount.UseVisualStyleBackColor = True
        '
        'TextCountApiReply
        '
        Me.TextCountApiReply.Location = New System.Drawing.Point(258, 51)
        Me.TextCountApiReply.Name = "TextCountApiReply"
        Me.TextCountApiReply.Size = New System.Drawing.Size(58, 19)
        Me.TextCountApiReply.TabIndex = 41
        '
        'Label67
        '
        Me.Label67.AutoSize = True
        Me.Label67.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label67.Location = New System.Drawing.Point(30, 24)
        Me.Label67.Name = "Label67"
        Me.Label67.Size = New System.Drawing.Size(77, 12)
        Me.Label67.TabIndex = 39
        Me.Label67.Text = "標準取得件数"
        '
        'TextCountApi
        '
        Me.TextCountApi.Location = New System.Drawing.Point(259, 21)
        Me.TextCountApi.Name = "TextCountApi"
        Me.TextCountApi.Size = New System.Drawing.Size(57, 19)
        Me.TextCountApi.TabIndex = 40
        '
        'BasedPanel
        '
        Me.BasedPanel.BackColor = System.Drawing.SystemColors.Window
        Me.BasedPanel.Controls.Add(Me.AuthBasicRadio)
        Me.BasedPanel.Controls.Add(Me.AuthOAuthRadio)
        Me.BasedPanel.Controls.Add(Me.Label6)
        Me.BasedPanel.Controls.Add(Me.AuthClearButton)
        Me.BasedPanel.Controls.Add(Me.AuthUserLabel)
        Me.BasedPanel.Controls.Add(Me.AuthStateLabel)
        Me.BasedPanel.Controls.Add(Me.Label4)
        Me.BasedPanel.Controls.Add(Me.AuthorizeButton)
        Me.BasedPanel.Controls.Add(Me.Label1)
        Me.BasedPanel.Controls.Add(Me.Label2)
        Me.BasedPanel.Controls.Add(Me.Username)
        Me.BasedPanel.Controls.Add(Me.Password)
        Me.BasedPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BasedPanel.Enabled = False
        Me.BasedPanel.Location = New System.Drawing.Point(0, 0)
        Me.BasedPanel.Name = "BasedPanel"
        Me.BasedPanel.Size = New System.Drawing.Size(525, 368)
        Me.BasedPanel.TabIndex = 0
        Me.BasedPanel.Visible = False
        '
        'AuthBasicRadio
        '
        Me.AuthBasicRadio.AutoSize = True
        Me.AuthBasicRadio.Enabled = False
        Me.AuthBasicRadio.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthBasicRadio.Location = New System.Drawing.Point(227, 20)
        Me.AuthBasicRadio.Name = "AuthBasicRadio"
        Me.AuthBasicRadio.Size = New System.Drawing.Size(57, 16)
        Me.AuthBasicRadio.TabIndex = 14
        Me.AuthBasicRadio.Text = "BASIC"
        Me.AuthBasicRadio.UseVisualStyleBackColor = True
        '
        'AuthOAuthRadio
        '
        Me.AuthOAuthRadio.AutoSize = True
        Me.AuthOAuthRadio.Checked = True
        Me.AuthOAuthRadio.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthOAuthRadio.Location = New System.Drawing.Point(113, 20)
        Me.AuthOAuthRadio.Name = "AuthOAuthRadio"
        Me.AuthOAuthRadio.Size = New System.Drawing.Size(93, 16)
        Me.AuthOAuthRadio.TabIndex = 13
        Me.AuthOAuthRadio.TabStop = True
        Me.AuthOAuthRadio.Text = "OAuth(xAuth)"
        Me.AuthOAuthRadio.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(22, 22)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(53, 12)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "認証方法"
        '
        'AuthClearButton
        '
        Me.AuthClearButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthClearButton.Location = New System.Drawing.Point(305, 64)
        Me.AuthClearButton.Name = "AuthClearButton"
        Me.AuthClearButton.Size = New System.Drawing.Size(75, 23)
        Me.AuthClearButton.TabIndex = 18
        Me.AuthClearButton.Text = "クリア"
        Me.AuthClearButton.UseVisualStyleBackColor = True
        '
        'AuthUserLabel
        '
        Me.AuthUserLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthUserLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthUserLabel.Location = New System.Drawing.Point(113, 68)
        Me.AuthUserLabel.Name = "AuthUserLabel"
        Me.AuthUserLabel.Size = New System.Drawing.Size(149, 14)
        Me.AuthUserLabel.TabIndex = 17
        Me.AuthUserLabel.Text = "認証済み"
        '
        'AuthStateLabel
        '
        Me.AuthStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthStateLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthStateLabel.Location = New System.Drawing.Point(113, 54)
        Me.AuthStateLabel.Name = "AuthStateLabel"
        Me.AuthStateLabel.Size = New System.Drawing.Size(112, 14)
        Me.AuthStateLabel.TabIndex = 16
        Me.AuthStateLabel.Text = "Not Authenticated"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(23, 54)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 12)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "認証状態"
        '
        'AuthorizeButton
        '
        Me.AuthorizeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.AuthorizeButton.Location = New System.Drawing.Point(305, 126)
        Me.AuthorizeButton.Name = "AuthorizeButton"
        Me.AuthorizeButton.Size = New System.Drawing.Size(75, 23)
        Me.AuthorizeButton.TabIndex = 23
        Me.AuthorizeButton.Text = "認証する"
        Me.AuthorizeButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(22, 112)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 12)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "ユーザー名"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(22, 133)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 12)
        Me.Label2.TabIndex = 21
        Me.Label2.Text = "パスワード"
        '
        'Username
        '
        Me.Username.Location = New System.Drawing.Point(113, 109)
        Me.Username.Name = "Username"
        Me.Username.Size = New System.Drawing.Size(186, 19)
        Me.Username.TabIndex = 20
        '
        'Password
        '
        Me.Password.Location = New System.Drawing.Point(113, 130)
        Me.Password.Name = "Password"
        Me.Password.Size = New System.Drawing.Size(186, 19)
        Me.Password.TabIndex = 22
        Me.Password.UseSystemPasswordChar = True
        '
        'ProxyPanel
        '
        Me.ProxyPanel.Controls.Add(Me.GroupBox2)
        Me.ProxyPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProxyPanel.Enabled = False
        Me.ProxyPanel.Location = New System.Drawing.Point(0, 0)
        Me.ProxyPanel.Name = "ProxyPanel"
        Me.ProxyPanel.Size = New System.Drawing.Size(525, 368)
        Me.ProxyPanel.TabIndex = 54
        Me.ProxyPanel.Visible = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label55)
        Me.GroupBox2.Controls.Add(Me.TextProxyPassword)
        Me.GroupBox2.Controls.Add(Me.LabelProxyPassword)
        Me.GroupBox2.Controls.Add(Me.TextProxyUser)
        Me.GroupBox2.Controls.Add(Me.LabelProxyUser)
        Me.GroupBox2.Controls.Add(Me.TextProxyPort)
        Me.GroupBox2.Controls.Add(Me.LabelProxyPort)
        Me.GroupBox2.Controls.Add(Me.TextProxyAddress)
        Me.GroupBox2.Controls.Add(Me.LabelProxyAddress)
        Me.GroupBox2.Controls.Add(Me.RadioProxySpecified)
        Me.GroupBox2.Controls.Add(Me.RadioProxyIE)
        Me.GroupBox2.Controls.Add(Me.RadioProxyNone)
        Me.GroupBox2.Location = New System.Drawing.Point(21, 18)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(449, 161)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "プロキシの設定"
        '
        'Label55
        '
        Me.Label55.AutoSize = True
        Me.Label55.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label55.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label55.Location = New System.Drawing.Point(28, 134)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(314, 12)
        Me.Label55.TabIndex = 11
        Me.Label55.Text = "※認証が不要な場合は、ユーザ名とパスワードは空にしてください。"
        '
        'TextProxyPassword
        '
        Me.TextProxyPassword.Location = New System.Drawing.Point(274, 103)
        Me.TextProxyPassword.Name = "TextProxyPassword"
        Me.TextProxyPassword.Size = New System.Drawing.Size(96, 19)
        Me.TextProxyPassword.TabIndex = 10
        Me.TextProxyPassword.UseSystemPasswordChar = True
        '
        'LabelProxyPassword
        '
        Me.LabelProxyPassword.AutoSize = True
        Me.LabelProxyPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelProxyPassword.Location = New System.Drawing.Point(205, 106)
        Me.LabelProxyPassword.Name = "LabelProxyPassword"
        Me.LabelProxyPassword.Size = New System.Drawing.Size(69, 12)
        Me.LabelProxyPassword.TabIndex = 9
        Me.LabelProxyPassword.Text = "パスワード(&W)"
        '
        'TextProxyUser
        '
        Me.TextProxyUser.Location = New System.Drawing.Point(131, 103)
        Me.TextProxyUser.Name = "TextProxyUser"
        Me.TextProxyUser.Size = New System.Drawing.Size(68, 19)
        Me.TextProxyUser.TabIndex = 8
        '
        'LabelProxyUser
        '
        Me.LabelProxyUser.AutoSize = True
        Me.LabelProxyUser.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelProxyUser.Location = New System.Drawing.Point(62, 106)
        Me.LabelProxyUser.Name = "LabelProxyUser"
        Me.LabelProxyUser.Size = New System.Drawing.Size(63, 12)
        Me.LabelProxyUser.TabIndex = 7
        Me.LabelProxyUser.Text = "ユーザ名(&U)"
        '
        'TextProxyPort
        '
        Me.TextProxyPort.Location = New System.Drawing.Point(297, 78)
        Me.TextProxyPort.Name = "TextProxyPort"
        Me.TextProxyPort.Size = New System.Drawing.Size(73, 19)
        Me.TextProxyPort.TabIndex = 6
        '
        'LabelProxyPort
        '
        Me.LabelProxyPort.AutoSize = True
        Me.LabelProxyPort.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelProxyPort.Location = New System.Drawing.Point(243, 81)
        Me.LabelProxyPort.Name = "LabelProxyPort"
        Me.LabelProxyPort.Size = New System.Drawing.Size(48, 12)
        Me.LabelProxyPort.TabIndex = 5
        Me.LabelProxyPort.Text = "ポート(&P)"
        '
        'TextProxyAddress
        '
        Me.TextProxyAddress.Location = New System.Drawing.Point(102, 78)
        Me.TextProxyAddress.Name = "TextProxyAddress"
        Me.TextProxyAddress.Size = New System.Drawing.Size(135, 19)
        Me.TextProxyAddress.TabIndex = 4
        '
        'LabelProxyAddress
        '
        Me.LabelProxyAddress.AutoSize = True
        Me.LabelProxyAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelProxyAddress.Location = New System.Drawing.Point(38, 81)
        Me.LabelProxyAddress.Name = "LabelProxyAddress"
        Me.LabelProxyAddress.Size = New System.Drawing.Size(58, 12)
        Me.LabelProxyAddress.TabIndex = 3
        Me.LabelProxyAddress.Text = "プロキシ(&X)"
        '
        'RadioProxySpecified
        '
        Me.RadioProxySpecified.AutoSize = True
        Me.RadioProxySpecified.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioProxySpecified.Location = New System.Drawing.Point(6, 62)
        Me.RadioProxySpecified.Name = "RadioProxySpecified"
        Me.RadioProxySpecified.Size = New System.Drawing.Size(66, 16)
        Me.RadioProxySpecified.TabIndex = 2
        Me.RadioProxySpecified.Text = "指定する"
        Me.RadioProxySpecified.UseVisualStyleBackColor = True
        '
        'RadioProxyIE
        '
        Me.RadioProxyIE.AutoSize = True
        Me.RadioProxyIE.Checked = True
        Me.RadioProxyIE.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioProxyIE.Location = New System.Drawing.Point(6, 40)
        Me.RadioProxyIE.Name = "RadioProxyIE"
        Me.RadioProxyIE.Size = New System.Drawing.Size(190, 16)
        Me.RadioProxyIE.TabIndex = 1
        Me.RadioProxyIE.TabStop = True
        Me.RadioProxyIE.Text = "InternetExplorerの設定を使用する"
        Me.RadioProxyIE.UseVisualStyleBackColor = True
        '
        'RadioProxyNone
        '
        Me.RadioProxyNone.AutoSize = True
        Me.RadioProxyNone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioProxyNone.Location = New System.Drawing.Point(6, 18)
        Me.RadioProxyNone.Name = "RadioProxyNone"
        Me.RadioProxyNone.Size = New System.Drawing.Size(76, 16)
        Me.RadioProxyNone.TabIndex = 0
        Me.RadioProxyNone.Text = "使用しない"
        Me.RadioProxyNone.UseVisualStyleBackColor = True
        '
        'TweetActPanel
        '
        Me.TweetActPanel.BackColor = System.Drawing.SystemColors.Window
        Me.TweetActPanel.Controls.Add(Me.TextBitlyPw)
        Me.TweetActPanel.Controls.Add(Me.ComboBoxPostKeySelect)
        Me.TweetActPanel.Controls.Add(Me.Label27)
        Me.TweetActPanel.Controls.Add(Me.CheckRetweetNoConfirm)
        Me.TweetActPanel.Controls.Add(Me.TextBitlyId)
        Me.TweetActPanel.Controls.Add(Me.Label42)
        Me.TweetActPanel.Controls.Add(Me.Label12)
        Me.TweetActPanel.Controls.Add(Me.Label77)
        Me.TweetActPanel.Controls.Add(Me.CheckUseRecommendStatus)
        Me.TweetActPanel.Controls.Add(Me.Label76)
        Me.TweetActPanel.Controls.Add(Me.StatusText)
        Me.TweetActPanel.Controls.Add(Me.ComboBoxAutoShortUrlFirst)
        Me.TweetActPanel.Controls.Add(Me.Label50)
        Me.TweetActPanel.Controls.Add(Me.Label71)
        Me.TweetActPanel.Controls.Add(Me.CheckTinyURL)
        Me.TweetActPanel.Controls.Add(Me.CheckAutoConvertUrl)
        Me.TweetActPanel.Controls.Add(Me.Label29)
        Me.TweetActPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TweetActPanel.Enabled = False
        Me.TweetActPanel.Location = New System.Drawing.Point(0, 0)
        Me.TweetActPanel.Name = "TweetActPanel"
        Me.TweetActPanel.Size = New System.Drawing.Size(525, 368)
        Me.TweetActPanel.TabIndex = 77
        Me.TweetActPanel.Visible = False
        '
        'TextBitlyPw
        '
        Me.TextBitlyPw.Location = New System.Drawing.Point(343, 94)
        Me.TextBitlyPw.Name = "TextBitlyPw"
        Me.TextBitlyPw.Size = New System.Drawing.Size(70, 19)
        Me.TextBitlyPw.TabIndex = 69
        '
        'ComboBoxPostKeySelect
        '
        Me.ComboBoxPostKeySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPostKeySelect.FormattingEnabled = True
        Me.ComboBoxPostKeySelect.Items.AddRange(New Object() {"Enter", "Ctrl+Enter", "Shift+Enter"})
        Me.ComboBoxPostKeySelect.Location = New System.Drawing.Point(181, 136)
        Me.ComboBoxPostKeySelect.Name = "ComboBoxPostKeySelect"
        Me.ComboBoxPostKeySelect.Size = New System.Drawing.Size(246, 20)
        Me.ComboBoxPostKeySelect.TabIndex = 60
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label27.Location = New System.Drawing.Point(19, 139)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(137, 12)
        Me.Label27.TabIndex = 59
        Me.Label27.Text = "POSTキー（デフォルトEnter）"
        '
        'CheckRetweetNoConfirm
        '
        Me.CheckRetweetNoConfirm.AutoSize = True
        Me.CheckRetweetNoConfirm.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckRetweetNoConfirm.Location = New System.Drawing.Point(181, 159)
        Me.CheckRetweetNoConfirm.Name = "CheckRetweetNoConfirm"
        Me.CheckRetweetNoConfirm.Size = New System.Drawing.Size(92, 16)
        Me.CheckRetweetNoConfirm.TabIndex = 62
        Me.CheckRetweetNoConfirm.Text = "RT確認しない"
        Me.CheckRetweetNoConfirm.UseVisualStyleBackColor = True
        '
        'TextBitlyId
        '
        Me.TextBitlyId.Location = New System.Drawing.Point(201, 95)
        Me.TextBitlyId.Name = "TextBitlyId"
        Me.TextBitlyId.Size = New System.Drawing.Size(71, 19)
        Me.TextBitlyId.TabIndex = 57
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label42.Location = New System.Drawing.Point(19, 160)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(44, 12)
        Me.Label42.TabIndex = 61
        Me.Label42.Text = "公式RT"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(20, 211)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(107, 12)
        Me.Label12.TabIndex = 66
        Me.Label12.Text = "フッター（文末に付加）"
        '
        'Label77
        '
        Me.Label77.AutoSize = True
        Me.Label77.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label77.Location = New System.Drawing.Point(278, 97)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(42, 12)
        Me.Label77.TabIndex = 58
        Me.Label77.Text = "APIKey"
        '
        'CheckUseRecommendStatus
        '
        Me.CheckUseRecommendStatus.AutoSize = True
        Me.CheckUseRecommendStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckUseRecommendStatus.Location = New System.Drawing.Point(182, 211)
        Me.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus"
        Me.CheckUseRecommendStatus.Size = New System.Drawing.Size(195, 16)
        Me.CheckUseRecommendStatus.TabIndex = 67
        Me.CheckUseRecommendStatus.Text = "推奨フッターを使用する[TWNv○○]"
        Me.CheckUseRecommendStatus.UseVisualStyleBackColor = True
        '
        'Label76
        '
        Me.Label76.AutoSize = True
        Me.Label76.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label76.Location = New System.Drawing.Point(179, 97)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(16, 12)
        Me.Label76.TabIndex = 56
        Me.Label76.Text = "ID"
        '
        'StatusText
        '
        Me.StatusText.Location = New System.Drawing.Point(182, 233)
        Me.StatusText.Name = "StatusText"
        Me.StatusText.Size = New System.Drawing.Size(232, 19)
        Me.StatusText.TabIndex = 68
        '
        'ComboBoxAutoShortUrlFirst
        '
        Me.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxAutoShortUrlFirst.FormattingEnabled = True
        Me.ComboBoxAutoShortUrlFirst.Items.AddRange(New Object() {"tinyurl", "is.gd", "twurl.nl", "bit.ly", "j.mp"})
        Me.ComboBoxAutoShortUrlFirst.Location = New System.Drawing.Point(181, 71)
        Me.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst"
        Me.ComboBoxAutoShortUrlFirst.Size = New System.Drawing.Size(246, 20)
        Me.ComboBoxAutoShortUrlFirst.TabIndex = 55
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label50.Location = New System.Drawing.Point(20, 22)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(84, 12)
        Me.Label50.TabIndex = 50
        Me.Label50.Text = "短縮URLを解決"
        '
        'Label71
        '
        Me.Label71.AutoSize = True
        Me.Label71.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label71.Location = New System.Drawing.Point(19, 74)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(154, 12)
        Me.Label71.TabIndex = 54
        Me.Label71.Text = "URL自動短縮で優先的に使用"
        '
        'CheckTinyURL
        '
        Me.CheckTinyURL.AutoSize = True
        Me.CheckTinyURL.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckTinyURL.Location = New System.Drawing.Point(182, 21)
        Me.CheckTinyURL.Name = "CheckTinyURL"
        Me.CheckTinyURL.Size = New System.Drawing.Size(67, 16)
        Me.CheckTinyURL.TabIndex = 51
        Me.CheckTinyURL.Text = "解決する"
        Me.CheckTinyURL.UseVisualStyleBackColor = True
        '
        'CheckAutoConvertUrl
        '
        Me.CheckAutoConvertUrl.AutoSize = True
        Me.CheckAutoConvertUrl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckAutoConvertUrl.Location = New System.Drawing.Point(182, 43)
        Me.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl"
        Me.CheckAutoConvertUrl.Size = New System.Drawing.Size(91, 16)
        Me.CheckAutoConvertUrl.TabIndex = 53
        Me.CheckAutoConvertUrl.Text = "自動短縮する"
        Me.CheckAutoConvertUrl.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label29.Location = New System.Drawing.Point(20, 44)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(121, 12)
        Me.Label29.TabIndex = 52
        Me.Label29.Text = "入力欄URLの自動短縮"
        '
        'FontPanel
        '
        Me.FontPanel.Controls.Add(Me.GroupBox1)
        Me.FontPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FontPanel.Enabled = False
        Me.FontPanel.Location = New System.Drawing.Point(0, 0)
        Me.FontPanel.Name = "FontPanel"
        Me.FontPanel.Size = New System.Drawing.Size(525, 368)
        Me.FontPanel.TabIndex = 51
        Me.FontPanel.Visible = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnRetweet)
        Me.GroupBox1.Controls.Add(Me.lblRetweet)
        Me.GroupBox1.Controls.Add(Me.Label80)
        Me.GroupBox1.Controls.Add(Me.ButtonBackToDefaultFontColor)
        Me.GroupBox1.Controls.Add(Me.btnDetailLink)
        Me.GroupBox1.Controls.Add(Me.lblDetailLink)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.btnUnread)
        Me.GroupBox1.Controls.Add(Me.lblUnread)
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.btnDetailBack)
        Me.GroupBox1.Controls.Add(Me.lblDetailBackcolor)
        Me.GroupBox1.Controls.Add(Me.Label37)
        Me.GroupBox1.Controls.Add(Me.btnDetail)
        Me.GroupBox1.Controls.Add(Me.lblDetail)
        Me.GroupBox1.Controls.Add(Me.Label26)
        Me.GroupBox1.Controls.Add(Me.btnOWL)
        Me.GroupBox1.Controls.Add(Me.lblOWL)
        Me.GroupBox1.Controls.Add(Me.Label24)
        Me.GroupBox1.Controls.Add(Me.btnFav)
        Me.GroupBox1.Controls.Add(Me.lblFav)
        Me.GroupBox1.Controls.Add(Me.Label22)
        Me.GroupBox1.Controls.Add(Me.btnListFont)
        Me.GroupBox1.Controls.Add(Me.lblListFont)
        Me.GroupBox1.Controls.Add(Me.Label61)
        Me.GroupBox1.Location = New System.Drawing.Point(22, 18)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(484, 267)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "フォント＆色設定"
        '
        'btnRetweet
        '
        Me.btnRetweet.AutoSize = True
        Me.btnRetweet.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRetweet.Location = New System.Drawing.Point(396, 119)
        Me.btnRetweet.Name = "btnRetweet"
        Me.btnRetweet.Size = New System.Drawing.Size(75, 22)
        Me.btnRetweet.TabIndex = 14
        Me.btnRetweet.Text = "文字色"
        Me.btnRetweet.UseVisualStyleBackColor = True
        '
        'lblRetweet
        '
        Me.lblRetweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblRetweet.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRetweet.Location = New System.Drawing.Point(214, 120)
        Me.lblRetweet.Name = "lblRetweet"
        Me.lblRetweet.Size = New System.Drawing.Size(104, 19)
        Me.lblRetweet.TabIndex = 13
        Me.lblRetweet.Text = "This is sample."
        Me.lblRetweet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label80
        '
        Me.Label80.AutoSize = True
        Me.Label80.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label80.Location = New System.Drawing.Point(9, 123)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(50, 12)
        Me.Label80.TabIndex = 12
        Me.Label80.Text = "ReTweet"
        '
        'ButtonBackToDefaultFontColor
        '
        Me.ButtonBackToDefaultFontColor.AutoSize = True
        Me.ButtonBackToDefaultFontColor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ButtonBackToDefaultFontColor.Location = New System.Drawing.Point(194, 236)
        Me.ButtonBackToDefaultFontColor.Name = "ButtonBackToDefaultFontColor"
        Me.ButtonBackToDefaultFontColor.Size = New System.Drawing.Size(90, 22)
        Me.ButtonBackToDefaultFontColor.TabIndex = 51
        Me.ButtonBackToDefaultFontColor.Text = "デフォルトに戻す"
        Me.ButtonBackToDefaultFontColor.UseVisualStyleBackColor = True
        '
        'btnDetailLink
        '
        Me.btnDetailLink.AutoSize = True
        Me.btnDetailLink.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDetailLink.Location = New System.Drawing.Point(396, 169)
        Me.btnDetailLink.Name = "btnDetailLink"
        Me.btnDetailLink.Size = New System.Drawing.Size(75, 22)
        Me.btnDetailLink.TabIndex = 20
        Me.btnDetailLink.Text = "文字色"
        Me.btnDetailLink.UseVisualStyleBackColor = True
        '
        'lblDetailLink
        '
        Me.lblDetailLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailLink.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDetailLink.Location = New System.Drawing.Point(214, 170)
        Me.lblDetailLink.Name = "lblDetailLink"
        Me.lblDetailLink.Size = New System.Drawing.Size(104, 19)
        Me.lblDetailLink.TabIndex = 19
        Me.lblDetailLink.Text = "This is sample."
        Me.lblDetailLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label18.Location = New System.Drawing.Point(8, 173)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(77, 12)
        Me.Label18.TabIndex = 18
        Me.Label18.Text = "発言詳細リンク"
        '
        'btnUnread
        '
        Me.btnUnread.AutoSize = True
        Me.btnUnread.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnUnread.Location = New System.Drawing.Point(396, 44)
        Me.btnUnread.Name = "btnUnread"
        Me.btnUnread.Size = New System.Drawing.Size(75, 22)
        Me.btnUnread.TabIndex = 5
        Me.btnUnread.Text = "フォント&&色"
        Me.btnUnread.UseVisualStyleBackColor = True
        '
        'lblUnread
        '
        Me.lblUnread.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUnread.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblUnread.Location = New System.Drawing.Point(214, 45)
        Me.lblUnread.Name = "lblUnread"
        Me.lblUnread.Size = New System.Drawing.Size(104, 19)
        Me.lblUnread.TabIndex = 4
        Me.lblUnread.Text = "This is sample."
        Me.lblUnread.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label20.Location = New System.Drawing.Point(9, 48)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(62, 12)
        Me.Label20.TabIndex = 3
        Me.Label20.Text = "未読フォント"
        '
        'btnDetailBack
        '
        Me.btnDetailBack.AutoSize = True
        Me.btnDetailBack.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDetailBack.Location = New System.Drawing.Point(396, 194)
        Me.btnDetailBack.Name = "btnDetailBack"
        Me.btnDetailBack.Size = New System.Drawing.Size(75, 22)
        Me.btnDetailBack.TabIndex = 23
        Me.btnDetailBack.Text = "背景色"
        Me.btnDetailBack.UseVisualStyleBackColor = True
        '
        'lblDetailBackcolor
        '
        Me.lblDetailBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailBackcolor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDetailBackcolor.Location = New System.Drawing.Point(214, 195)
        Me.lblDetailBackcolor.Name = "lblDetailBackcolor"
        Me.lblDetailBackcolor.Size = New System.Drawing.Size(104, 19)
        Me.lblDetailBackcolor.TabIndex = 22
        Me.lblDetailBackcolor.Text = "This is sample."
        Me.lblDetailBackcolor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label37.Location = New System.Drawing.Point(9, 198)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(89, 12)
        Me.Label37.TabIndex = 21
        Me.Label37.Text = "発言詳細背景色"
        '
        'btnDetail
        '
        Me.btnDetail.AutoSize = True
        Me.btnDetail.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDetail.Location = New System.Drawing.Point(396, 144)
        Me.btnDetail.Name = "btnDetail"
        Me.btnDetail.Size = New System.Drawing.Size(75, 22)
        Me.btnDetail.TabIndex = 17
        Me.btnDetail.Text = "フォント&&色"
        Me.btnDetail.UseVisualStyleBackColor = True
        '
        'lblDetail
        '
        Me.lblDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetail.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDetail.Location = New System.Drawing.Point(214, 145)
        Me.lblDetail.Name = "lblDetail"
        Me.lblDetail.Size = New System.Drawing.Size(104, 19)
        Me.lblDetail.TabIndex = 16
        Me.lblDetail.Text = "This is sample."
        Me.lblDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label26.Location = New System.Drawing.Point(9, 148)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(77, 12)
        Me.Label26.TabIndex = 15
        Me.Label26.Text = "発言詳細文字"
        '
        'btnOWL
        '
        Me.btnOWL.AutoSize = True
        Me.btnOWL.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnOWL.Location = New System.Drawing.Point(396, 94)
        Me.btnOWL.Name = "btnOWL"
        Me.btnOWL.Size = New System.Drawing.Size(75, 22)
        Me.btnOWL.TabIndex = 11
        Me.btnOWL.Text = "文字色"
        Me.btnOWL.UseVisualStyleBackColor = True
        '
        'lblOWL
        '
        Me.lblOWL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOWL.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblOWL.Location = New System.Drawing.Point(214, 95)
        Me.lblOWL.Name = "lblOWL"
        Me.lblOWL.Size = New System.Drawing.Size(104, 19)
        Me.lblOWL.TabIndex = 10
        Me.lblOWL.Text = "This is sample."
        Me.lblOWL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label24.Location = New System.Drawing.Point(9, 98)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(63, 12)
        Me.Label24.TabIndex = 9
        Me.Label24.Text = "片思い発言"
        '
        'btnFav
        '
        Me.btnFav.AutoSize = True
        Me.btnFav.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnFav.Location = New System.Drawing.Point(396, 69)
        Me.btnFav.Name = "btnFav"
        Me.btnFav.Size = New System.Drawing.Size(75, 22)
        Me.btnFav.TabIndex = 8
        Me.btnFav.Text = "文字色"
        Me.btnFav.UseVisualStyleBackColor = True
        '
        'lblFav
        '
        Me.lblFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFav.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFav.Location = New System.Drawing.Point(214, 70)
        Me.lblFav.Name = "lblFav"
        Me.lblFav.Size = New System.Drawing.Size(104, 19)
        Me.lblFav.TabIndex = 7
        Me.lblFav.Text = "This is sample."
        Me.lblFav.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label22.Location = New System.Drawing.Point(9, 73)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(48, 12)
        Me.Label22.TabIndex = 6
        Me.Label22.Text = "Fav発言"
        '
        'btnListFont
        '
        Me.btnListFont.AutoSize = True
        Me.btnListFont.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnListFont.Location = New System.Drawing.Point(396, 19)
        Me.btnListFont.Name = "btnListFont"
        Me.btnListFont.Size = New System.Drawing.Size(75, 22)
        Me.btnListFont.TabIndex = 2
        Me.btnListFont.Text = "フォント&&色"
        Me.btnListFont.UseVisualStyleBackColor = True
        '
        'lblListFont
        '
        Me.lblListFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListFont.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblListFont.Location = New System.Drawing.Point(214, 20)
        Me.lblListFont.Name = "lblListFont"
        Me.lblListFont.Size = New System.Drawing.Size(104, 19)
        Me.lblListFont.TabIndex = 1
        Me.lblListFont.Text = "This is sample."
        Me.lblListFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label61
        '
        Me.Label61.AutoSize = True
        Me.Label61.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label61.Location = New System.Drawing.Point(9, 23)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(62, 12)
        Me.Label61.TabIndex = 0
        Me.Label61.Text = "リストフォント"
        '
        'StartupPanel
        '
        Me.StartupPanel.BackColor = System.Drawing.SystemColors.Window
        Me.StartupPanel.Controls.Add(Me.Label9)
        Me.StartupPanel.Controls.Add(Me.StartupReaded)
        Me.StartupPanel.Controls.Add(Me.Label54)
        Me.StartupPanel.Controls.Add(Me.CheckStartupFollowers)
        Me.StartupPanel.Controls.Add(Me.Label51)
        Me.StartupPanel.Controls.Add(Me.CheckStartupVersion)
        Me.StartupPanel.Controls.Add(Me.Label74)
        Me.StartupPanel.Controls.Add(Me.chkGetFav)
        Me.StartupPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StartupPanel.Enabled = False
        Me.StartupPanel.Location = New System.Drawing.Point(0, 0)
        Me.StartupPanel.Name = "StartupPanel"
        Me.StartupPanel.Size = New System.Drawing.Size(525, 368)
        Me.StartupPanel.TabIndex = 44
        Me.StartupPanel.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(22, 24)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(114, 12)
        Me.Label9.TabIndex = 36
        Me.Label9.Text = "起動時読み込みポスト"
        '
        'StartupReaded
        '
        Me.StartupReaded.AutoSize = True
        Me.StartupReaded.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.StartupReaded.Location = New System.Drawing.Point(257, 21)
        Me.StartupReaded.Name = "StartupReaded"
        Me.StartupReaded.Size = New System.Drawing.Size(76, 16)
        Me.StartupReaded.TabIndex = 37
        Me.StartupReaded.Text = "既読にする"
        Me.StartupReaded.UseVisualStyleBackColor = True
        '
        'Label54
        '
        Me.Label54.AutoSize = True
        Me.Label54.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label54.Location = New System.Drawing.Point(22, 93)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(163, 12)
        Me.Label54.TabIndex = 40
        Me.Label54.Text = "起動時片思いユーザーリスト取得"
        '
        'CheckStartupFollowers
        '
        Me.CheckStartupFollowers.AutoSize = True
        Me.CheckStartupFollowers.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckStartupFollowers.Location = New System.Drawing.Point(255, 92)
        Me.CheckStartupFollowers.Name = "CheckStartupFollowers"
        Me.CheckStartupFollowers.Size = New System.Drawing.Size(67, 16)
        Me.CheckStartupFollowers.TabIndex = 41
        Me.CheckStartupFollowers.Text = "取得する"
        Me.CheckStartupFollowers.UseVisualStyleBackColor = True
        '
        'Label51
        '
        Me.Label51.AutoSize = True
        Me.Label51.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label51.Location = New System.Drawing.Point(22, 56)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(117, 12)
        Me.Label51.TabIndex = 38
        Me.Label51.Text = "起動時バージョンチェック"
        '
        'CheckStartupVersion
        '
        Me.CheckStartupVersion.AutoSize = True
        Me.CheckStartupVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckStartupVersion.Location = New System.Drawing.Point(255, 55)
        Me.CheckStartupVersion.Name = "CheckStartupVersion"
        Me.CheckStartupVersion.Size = New System.Drawing.Size(74, 16)
        Me.CheckStartupVersion.TabIndex = 39
        Me.CheckStartupVersion.Text = "チェックする"
        Me.CheckStartupVersion.UseVisualStyleBackColor = True
        '
        'Label74
        '
        Me.Label74.AutoSize = True
        Me.Label74.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label74.Location = New System.Drawing.Point(22, 131)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(84, 12)
        Me.Label74.TabIndex = 42
        Me.Label74.Text = "起動時Fav取得"
        '
        'chkGetFav
        '
        Me.chkGetFav.AutoSize = True
        Me.chkGetFav.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkGetFav.Location = New System.Drawing.Point(255, 130)
        Me.chkGetFav.Name = "chkGetFav"
        Me.chkGetFav.Size = New System.Drawing.Size(67, 16)
        Me.chkGetFav.TabIndex = 43
        Me.chkGetFav.Text = "取得する"
        Me.chkGetFav.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.CausesValidation = False
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Cancel.Location = New System.Drawing.Point(613, 374)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Cancel.TabIndex = 4
        Me.Cancel.Text = "キャンセル"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Save
        '
        Me.Save.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Save.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Save.Location = New System.Drawing.Point(532, 374)
        Me.Save.Name = "Save"
        Me.Save.Size = New System.Drawing.Size(75, 23)
        Me.Save.TabIndex = 3
        Me.Save.Text = "OK"
        Me.Save.UseVisualStyleBackColor = True
        '
        'AppendSettingDialog
        '
        Me.AcceptButton = Me.Save
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel
        Me.ClientSize = New System.Drawing.Size(701, 403)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.Controls.Add(Me.SplitContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AppendSettingDialog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "設定"
        Me.TopMost = True
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.UserStreamPanel.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.ActionPanel.ResumeLayout(False)
        Me.ActionPanel.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.FontPanel2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GetPeriodPanel.ResumeLayout(False)
        Me.GetPeriodPanel.PerformLayout()
        Me.TweetPrvPanel.ResumeLayout(False)
        Me.TweetPrvPanel.PerformLayout()
        Me.PreviewPanel.ResumeLayout(False)
        Me.PreviewPanel.PerformLayout()
        Me.ConnectionPanel.ResumeLayout(False)
        Me.ConnectionPanel.PerformLayout()
        Me.GetCountPanel.ResumeLayout(False)
        Me.GetCountPanel.PerformLayout()
        Me.BasedPanel.ResumeLayout(False)
        Me.BasedPanel.PerformLayout()
        Me.ProxyPanel.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TweetActPanel.ResumeLayout(False)
        Me.TweetActPanel.PerformLayout()
        Me.FontPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.StartupPanel.ResumeLayout(False)
        Me.StartupPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents BasedPanel As System.Windows.Forms.Panel
    Friend WithEvents AuthBasicRadio As System.Windows.Forms.RadioButton
    Friend WithEvents AuthOAuthRadio As System.Windows.Forms.RadioButton
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents AuthClearButton As System.Windows.Forms.Button
    Friend WithEvents AuthUserLabel As System.Windows.Forms.Label
    Friend WithEvents AuthStateLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents AuthorizeButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Username As System.Windows.Forms.TextBox
    Friend WithEvents Password As System.Windows.Forms.TextBox
    Friend WithEvents GetPeriodPanel As System.Windows.Forms.Panel
    Friend WithEvents TimelinePeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ButtonApiCalc As System.Windows.Forms.Button
    Friend WithEvents LabelPostAndGet As System.Windows.Forms.Label
    Friend WithEvents LabelApiUsing As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents ListsPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents PubSearchPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents ReplyPeriod As System.Windows.Forms.TextBox
    Friend WithEvents CheckPostAndGet As System.Windows.Forms.CheckBox
    Friend WithEvents CheckPeriodAdjust As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents DMPeriod As System.Windows.Forms.TextBox
    Friend WithEvents StartupPanel As System.Windows.Forms.Panel
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents UserstreamPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents StartupUserstreamCheck As System.Windows.Forms.CheckBox
    Friend WithEvents GetCountPanel As System.Windows.Forms.Panel
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents StartupReaded As System.Windows.Forms.CheckBox
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents CheckStartupFollowers As System.Windows.Forms.CheckBox
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents CheckStartupVersion As System.Windows.Forms.CheckBox
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents chkGetFav As System.Windows.Forms.CheckBox
    Friend WithEvents TextCountApiReply As System.Windows.Forms.TextBox
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents TextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents FavoritesTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents SearchTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents FirstTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents GetMoreTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents UseChangeGetCount As System.Windows.Forms.CheckBox
    Friend WithEvents ActionPanel As System.Windows.Forms.Panel
    Friend WithEvents PreviewPanel As System.Windows.Forms.Panel
    Friend WithEvents FontPanel As System.Windows.Forms.Panel
    Friend WithEvents ConnectionPanel As System.Windows.Forms.Panel
    Friend WithEvents ProxyPanel As System.Windows.Forms.Panel
    Friend WithEvents ComboBoxPostKeySelect As System.Windows.Forms.ComboBox
    Friend WithEvents CheckRetweetNoConfirm As System.Windows.Forms.CheckBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents HotkeyCheck As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCode As System.Windows.Forms.Label
    Friend WithEvents HotkeyText As System.Windows.Forms.TextBox
    Friend WithEvents HotkeyWin As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyAlt As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyShift As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCtrl As System.Windows.Forms.CheckBox
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents CheckHashSupple As System.Windows.Forms.CheckBox
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents CheckAtIdSupple As System.Windows.Forms.CheckBox
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents TextBitlyId As System.Windows.Forms.TextBox
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxAutoShortUrlFirst As System.Windows.Forms.ComboBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents CheckAutoConvertUrl As System.Windows.Forms.CheckBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents CheckFavRestrict As System.Windows.Forms.CheckBox
    Friend WithEvents CheckTinyURL As System.Windows.Forms.CheckBox
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents PlaySnd As System.Windows.Forms.CheckBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents BrowserPathText As System.Windows.Forms.TextBox
    Friend WithEvents UReadMng As System.Windows.Forms.CheckBox
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents CheckCloseToExit As System.Windows.Forms.CheckBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents CheckMinimizeToTray As System.Windows.Forms.CheckBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents CheckReadOldPosts As System.Windows.Forms.CheckBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents StatusText As System.Windows.Forms.TextBox
    Friend WithEvents CheckUseRecommendStatus As System.Windows.Forms.CheckBox
    Friend WithEvents TweetActPanel As System.Windows.Forms.Panel
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents CheckPreviewEnable As System.Windows.Forms.CheckBox
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents LanguageCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents CheckAlwaysTop As System.Windows.Forms.CheckBox
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents CheckMonospace As System.Windows.Forms.CheckBox
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents CheckBalloonLimit As System.Windows.Forms.CheckBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents ComboDispTitle As System.Windows.Forms.ComboBox
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents cmbNameBalloon As System.Windows.Forms.ComboBox
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents CheckDispUsername As System.Windows.Forms.CheckBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents TweetPrvPanel As System.Windows.Forms.Panel
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents CheckSortOrderLock As System.Windows.Forms.CheckBox
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents CheckShowGrid As System.Windows.Forms.CheckBox
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents chkReadOwnPost As System.Windows.Forms.CheckBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents chkUnreadStyle As System.Windows.Forms.CheckBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents OneWayLv As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnRetweet As System.Windows.Forms.Button
    Friend WithEvents lblRetweet As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents ButtonBackToDefaultFontColor As System.Windows.Forms.Button
    Friend WithEvents btnDetailLink As System.Windows.Forms.Button
    Friend WithEvents lblDetailLink As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents btnUnread As System.Windows.Forms.Button
    Friend WithEvents lblUnread As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents btnDetailBack As System.Windows.Forms.Button
    Friend WithEvents lblDetailBackcolor As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents btnDetail As System.Windows.Forms.Button
    Friend WithEvents lblDetail As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents btnOWL As System.Windows.Forms.Button
    Friend WithEvents lblOWL As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents btnFav As System.Windows.Forms.Button
    Friend WithEvents lblFav As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnListFont As System.Windows.Forms.Button
    Friend WithEvents lblListFont As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents CheckEnableBasicAuth As System.Windows.Forms.CheckBox
    Friend WithEvents TwitterSearchAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents TwitterAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CheckUseSsl As System.Windows.Forms.CheckBox
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents ConnectionTimeOut As System.Windows.Forms.TextBox
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents TextProxyPassword As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPassword As System.Windows.Forms.Label
    Friend WithEvents TextProxyUser As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyUser As System.Windows.Forms.Label
    Friend WithEvents TextProxyPort As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPort As System.Windows.Forms.Label
    Friend WithEvents TextProxyAddress As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyAddress As System.Windows.Forms.Label
    Friend WithEvents RadioProxySpecified As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyIE As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyNone As System.Windows.Forms.RadioButton
    Friend WithEvents FontPanel2 As System.Windows.Forms.Panel
    Friend WithEvents CheckNicoms As System.Windows.Forms.CheckBox
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxOutputzUrlmode As System.Windows.Forms.ComboBox
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents TextBoxOutputzKey As System.Windows.Forms.TextBox
    Friend WithEvents CheckOutputz As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents ButtonBackToDefaultFontColor2 As System.Windows.Forms.Button
    Friend WithEvents Label89 As System.Windows.Forms.Label
    Friend WithEvents Label91 As System.Windows.Forms.Label
    Friend WithEvents Label95 As System.Windows.Forms.Label
    Friend WithEvents Label99 As System.Windows.Forms.Label
    Friend WithEvents Label101 As System.Windows.Forms.Label
    Friend WithEvents Label103 As System.Windows.Forms.Label
    Friend WithEvents Label105 As System.Windows.Forms.Label
    Friend WithEvents Label107 As System.Windows.Forms.Label
    Friend WithEvents Label109 As System.Windows.Forms.Label
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents TextBitlyPw As System.Windows.Forms.TextBox
    Friend WithEvents lblInputFont As System.Windows.Forms.Label
    Friend WithEvents lblInputBackcolor As System.Windows.Forms.Label
    Friend WithEvents lblAtTo As System.Windows.Forms.Label
    Friend WithEvents lblListBackcolor As System.Windows.Forms.Label
    Friend WithEvents lblAtFromTarget As System.Windows.Forms.Label
    Friend WithEvents lblAtTarget As System.Windows.Forms.Label
    Friend WithEvents lblTarget As System.Windows.Forms.Label
    Friend WithEvents lblAtSelf As System.Windows.Forms.Label
    Friend WithEvents lblSelf As System.Windows.Forms.Label
    Friend WithEvents btnInputFont As System.Windows.Forms.Button
    Friend WithEvents btnInputBackcolor As System.Windows.Forms.Button
    Friend WithEvents btnAtTo As System.Windows.Forms.Button
    Friend WithEvents btnListBack As System.Windows.Forms.Button
    Friend WithEvents btnAtFromTarget As System.Windows.Forms.Button
    Friend WithEvents btnAtTarget As System.Windows.Forms.Button
    Friend WithEvents btnTarget As System.Windows.Forms.Button
    Friend WithEvents btnAtSelf As System.Windows.Forms.Button
    Friend WithEvents btnSelf As System.Windows.Forms.Button
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents LabelDateTimeFormatApplied As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents CmbDateTimeFormat As System.Windows.Forms.ComboBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents IconSize As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents ReplyIconStateCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents ChkNewMentionsBlink As System.Windows.Forms.CheckBox
    Friend WithEvents chkTabIconDisp As System.Windows.Forms.CheckBox
    Friend WithEvents UserStreamPanel As System.Windows.Forms.Panel
End Class
