using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class TweenMain : System.Windows.Forms.Form
	{

		//フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing && components != null) {
					components.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		//Windows フォーム デザイナで必要です。

		private System.ComponentModel.IContainer components;
		//メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
		//Windows フォーム デザイナを使用して変更できます。  
		//コード エディタを使って変更しないでください。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweenMain));
			this.ToolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLabelUrl = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabel = new Tween.TweenCustomControl.ToolStripLabelHistory();
			this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.HashStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
			this.ContextMenuPostMode = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ToolStripMenuItemUrlMultibyteSplit = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemApiCommandEvasion = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemUrlAutoShorten = new System.Windows.Forms.ToolStripMenuItem();
			this.IdeographicSpaceToSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MultiLineMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripFocusLockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator35 = new System.Windows.Forms.ToolStripSeparator();
			this.ImageSelectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.HashToggleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.HashManageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SplitContainer4 = new System.Windows.Forms.SplitContainer();
			this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
			this.TimelinePanel = new System.Windows.Forms.Panel();
			this.ListTab = new System.Windows.Forms.TabControl();
			this.ContextMenuTabProperty = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.AddTabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TabRenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			this.UreadManageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NotifyDispMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SoundFileComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.ToolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.FilterEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			this.ClearTabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.DeleteTabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TabImage = new System.Windows.Forms.ImageList(this.components);
			this.ImageSelectionPanel = new System.Windows.Forms.Panel();
			this.ImageSelectedPicture = new Tween.TweenCustomControl.PictureBoxEx();
			this.ImagePathPanel = new System.Windows.Forms.Panel();
			this.ImagefilePathText = new System.Windows.Forms.TextBox();
			this.FilePickButton = new System.Windows.Forms.Button();
			this.Label2 = new System.Windows.Forms.Label();
			this.ImageServiceCombo = new System.Windows.Forms.ComboBox();
			this.ImageCancelButton = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.ProfilePanel = new System.Windows.Forms.Panel();
			this.SplitContainer3 = new System.Windows.Forms.SplitContainer();
			this.SplitContainer2 = new System.Windows.Forms.SplitContainer();
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.UserPicture = new Tween.TweenCustomControl.PictureBoxEx();
			this.ContextMenuUserPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.FollowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UnFollowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ShowFriendShipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListManageUserContextToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator37 = new System.Windows.Forms.ToolStripSeparator();
			this.ShowUserStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchPostsDetailNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchAtPostsDetailNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.IconNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SaveIconPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NameLabel = new System.Windows.Forms.Label();
			this.PostBrowser = new System.Windows.Forms.WebBrowser();
			this.ContextMenuPostBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SelectionSearchContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchGoogleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchWikipediaContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchYatsContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchPublicSearchContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CurrentTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.SelectionCopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UrlCopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SelectionAllContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.FollowContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RemoveContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FriendshipContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FriendshipAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator36 = new System.Windows.Forms.ToolStripSeparator();
			this.ShowUserStatusContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchPostsDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SearchAtPostsDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
			this.IdFilterAddMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListManageUserContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
			this.UseHashtagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SelectionTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DateTimeLabel = new System.Windows.Forms.Label();
			this.SourceLinkLabel = new System.Windows.Forms.LinkLabel();
			this.ContextMenuSource = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SourceCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SourceUrlCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.StatusText = new System.Windows.Forms.TextBox();
			this.lblLen = new System.Windows.Forms.Label();
			this.PostButton = new System.Windows.Forms.Button();
			this.PreviewPicture = new Tween.TweenCustomControl.PictureBoxEx();
			this.PreviewScrollBar = new System.Windows.Forms.VScrollBar();
			this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
			this.MenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
			this.SaveFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
			this.NotifyFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PlaySoundFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LockListFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator43 = new System.Windows.Forms.ToolStripSeparator();
			this.StopRefreshAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
			this.TweenRestartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.EndFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.UndoRemoveTabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.CopySTOTMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CopyURLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CopyUserIdStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.MenuItemSubSearch = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemSearchNext = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemSearchPrev = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
			this.PublicSearchQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemOperate = new System.Windows.Forms.ToolStripMenuItem();
			this.ReplyOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReplyAllOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DmOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RtOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RtUnOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.QtOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
			this.FavOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FavoriteRetweetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FavoriteRetweetUnofficialMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UnFavOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator38 = new System.Windows.Forms.ToolStripSeparator();
			this.ShowProfMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ShowRelatedStatusesMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.ShowUserTimelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenHomeOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenFavOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenStatusOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenRepSourceOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenFavotterOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenUrlOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenRterHomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenUserSpecifiedUrlMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CreateRuleOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CreateTabRuleOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CreateIdRuleOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListManageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
			this.ChangeReadOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReadOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UnreadOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.JumpReadOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
			this.SelAllOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DelOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RefreshOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RefreshPrevOpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemTab = new System.Windows.Forms.ToolStripMenuItem();
			this.CreateTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RenameTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
			this.UnreadMngTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NotifyTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SoundFileTbComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.ToolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
			this.EditRuleTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
			this.ClearTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
			this.DeleteTbMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.TinyUrlConvertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UrlConvertAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UrlUndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TinyURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.IsgdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TwurlnlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BitlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.JmpStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UxnuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UpdateFollowersMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.FollowCommandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RemoveCommandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FriendshipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.OwnStatusMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenOwnHomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenOwnFavedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator41 = new System.Windows.Forms.ToolStripSeparator();
			this.UserStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UserTimelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UserFavorareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
			this.HashToggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.HashManageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RtCountMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListManageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemUserStream = new System.Windows.Forms.ToolStripMenuItem();
			this.StopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator40 = new System.Windows.Forms.ToolStripSeparator();
			this.TrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AllrepliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator42 = new System.Windows.Forms.ToolStripSeparator();
			this.EventViewerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.MatomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ShortcutKeyListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.VerUpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.ApiInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DebugModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DumpPostClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TraceOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CacheInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuOperate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ReplyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReplyAllStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DMStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReTweetOriginalStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ReTweetStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.QuoteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator39 = new System.Windows.Forms.ToolStripSeparator();
			this.FavAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FavoriteRetweetContextMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.FavoriteRetweetUnofficialContextMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.FavRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.ShowProfileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ShowRelatedStatusesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ShowUserTimelineContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.MoveToHomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MoveToFavToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.StatusOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RepliedStatusOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FavorareMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenURLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MoveToRTHomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenUserSpecifiedUrlMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.TabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.IDRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListManageUserContextToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.ToolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
			this.ReadedStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UnreadStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.JumpUnreadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.SelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DeleteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RefreshStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RefreshMoreStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuFile = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SettingStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.SaveLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.NewPostPopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PlaySoundMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ListLockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			this.EndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NotifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.TimerRefreshIcon = new System.Windows.Forms.Timer(this.components);
			this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.PostStateImageList = new System.Windows.Forms.ImageList(this.components);
			this.ToolStripContainer1.BottomToolStripPanel.SuspendLayout();
			this.ToolStripContainer1.ContentPanel.SuspendLayout();
			this.ToolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.ToolStripContainer1.SuspendLayout();
			this.StatusStrip1.SuspendLayout();
			this.ContextMenuPostMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.SplitContainer4).BeginInit();
			this.SplitContainer4.Panel1.SuspendLayout();
			this.SplitContainer4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.SplitContainer1).BeginInit();
			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			this.TimelinePanel.SuspendLayout();
			this.ContextMenuTabProperty.SuspendLayout();
			this.ImageSelectionPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.ImageSelectedPicture).BeginInit();
			this.ImagePathPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.SplitContainer3).BeginInit();
			this.SplitContainer3.Panel1.SuspendLayout();
			this.SplitContainer3.Panel2.SuspendLayout();
			this.SplitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.SplitContainer2).BeginInit();
			this.SplitContainer2.Panel1.SuspendLayout();
			this.SplitContainer2.Panel2.SuspendLayout();
			this.SplitContainer2.SuspendLayout();
			this.TableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.UserPicture).BeginInit();
			this.ContextMenuUserPicture.SuspendLayout();
			this.ContextMenuPostBrowser.SuspendLayout();
			this.ContextMenuSource.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.PreviewPicture).BeginInit();
			this.MenuStrip1.SuspendLayout();
			this.ContextMenuOperate.SuspendLayout();
			this.ContextMenuFile.SuspendLayout();
			this.SuspendLayout();
			//
			//ToolStripContainer1
			//
			//
			//ToolStripContainer1.BottomToolStripPanel
			//
			this.ToolStripContainer1.BottomToolStripPanel.Controls.Add(this.StatusStrip1);
			//
			//ToolStripContainer1.ContentPanel
			//
			this.ToolStripContainer1.ContentPanel.Controls.Add(this.SplitContainer4);
			resources.ApplyResources(this.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel");
			resources.ApplyResources(this.ToolStripContainer1, "ToolStripContainer1");
			this.ToolStripContainer1.LeftToolStripPanelVisible = false;
			this.ToolStripContainer1.Name = "ToolStripContainer1";
			this.ToolStripContainer1.RightToolStripPanelVisible = false;
			//
			//ToolStripContainer1.TopToolStripPanel
			//
			this.ToolStripContainer1.TopToolStripPanel.Controls.Add(this.MenuStrip1);
			//
			//StatusStrip1
			//
			resources.ApplyResources(this.StatusStrip1, "StatusStrip1");
			this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.StatusLabelUrl,
				this.StatusLabel,
				this.ToolStripStatusLabel1,
				this.HashStripSplitButton
			});
			this.StatusStrip1.Name = "StatusStrip1";
			this.StatusStrip1.ShowItemToolTips = true;
			this.StatusStrip1.SizingGrip = false;
			//
			//StatusLabelUrl
			//
			resources.ApplyResources(this.StatusLabelUrl, "StatusLabelUrl");
			this.StatusLabelUrl.AutoToolTip = true;
			this.StatusLabelUrl.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.StatusLabelUrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.StatusLabelUrl.Name = "StatusLabelUrl";
			this.StatusLabelUrl.Spring = true;
			//
			//StatusLabel
			//
			this.StatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.StatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.StatusLabel.DoubleClickEnabled = true;
			this.StatusLabel.Name = "StatusLabel";
			resources.ApplyResources(this.StatusLabel, "StatusLabel");
			//
			//ToolStripStatusLabel1
			//
			resources.ApplyResources(this.ToolStripStatusLabel1, "ToolStripStatusLabel1");
			this.ToolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
			//
			//HashStripSplitButton
			//
			this.HashStripSplitButton.AutoToolTip = false;
			this.HashStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.HashStripSplitButton.DropDown = this.ContextMenuPostMode;
			this.HashStripSplitButton.DropDownButtonWidth = 13;
			this.HashStripSplitButton.Name = "HashStripSplitButton";
			resources.ApplyResources(this.HashStripSplitButton, "HashStripSplitButton");
			//
			//ContextMenuPostMode
			//
			this.ContextMenuPostMode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.ToolStripMenuItemUrlMultibyteSplit,
				this.ToolStripMenuItemApiCommandEvasion,
				this.ToolStripMenuItemUrlAutoShorten,
				this.IdeographicSpaceToSpaceToolStripMenuItem,
				this.MultiLineMenuItem,
				this.ToolStripFocusLockMenuItem,
				this.ToolStripSeparator35,
				this.ImageSelectMenuItem,
				this.ToolStripSeparator8,
				this.HashToggleMenuItem,
				this.HashManageMenuItem
			});
			this.ContextMenuPostMode.Name = "ContextMenuStripPostMode";
			this.ContextMenuPostMode.OwnerItem = this.HashStripSplitButton;
			resources.ApplyResources(this.ContextMenuPostMode, "ContextMenuPostMode");
			//
			//ToolStripMenuItemUrlMultibyteSplit
			//
			this.ToolStripMenuItemUrlMultibyteSplit.CheckOnClick = true;
			this.ToolStripMenuItemUrlMultibyteSplit.Name = "ToolStripMenuItemUrlMultibyteSplit";
			resources.ApplyResources(this.ToolStripMenuItemUrlMultibyteSplit, "ToolStripMenuItemUrlMultibyteSplit");
			//
			//ToolStripMenuItemApiCommandEvasion
			//
			this.ToolStripMenuItemApiCommandEvasion.Checked = true;
			this.ToolStripMenuItemApiCommandEvasion.CheckOnClick = true;
			this.ToolStripMenuItemApiCommandEvasion.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ToolStripMenuItemApiCommandEvasion.Name = "ToolStripMenuItemApiCommandEvasion";
			resources.ApplyResources(this.ToolStripMenuItemApiCommandEvasion, "ToolStripMenuItemApiCommandEvasion");
			//
			//ToolStripMenuItemUrlAutoShorten
			//
			this.ToolStripMenuItemUrlAutoShorten.CheckOnClick = true;
			this.ToolStripMenuItemUrlAutoShorten.Name = "ToolStripMenuItemUrlAutoShorten";
			resources.ApplyResources(this.ToolStripMenuItemUrlAutoShorten, "ToolStripMenuItemUrlAutoShorten");
			//
			//IdeographicSpaceToSpaceToolStripMenuItem
			//
			this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = true;
			this.IdeographicSpaceToSpaceToolStripMenuItem.CheckOnClick = true;
			this.IdeographicSpaceToSpaceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.IdeographicSpaceToSpaceToolStripMenuItem.Name = "IdeographicSpaceToSpaceToolStripMenuItem";
			resources.ApplyResources(this.IdeographicSpaceToSpaceToolStripMenuItem, "IdeographicSpaceToSpaceToolStripMenuItem");
			//
			//MultiLineMenuItem
			//
			this.MultiLineMenuItem.CheckOnClick = true;
			this.MultiLineMenuItem.Name = "MultiLineMenuItem";
			resources.ApplyResources(this.MultiLineMenuItem, "MultiLineMenuItem");
			//
			//ToolStripFocusLockMenuItem
			//
			this.ToolStripFocusLockMenuItem.CheckOnClick = true;
			this.ToolStripFocusLockMenuItem.Name = "ToolStripFocusLockMenuItem";
			resources.ApplyResources(this.ToolStripFocusLockMenuItem, "ToolStripFocusLockMenuItem");
			//
			//ToolStripSeparator35
			//
			this.ToolStripSeparator35.Name = "ToolStripSeparator35";
			resources.ApplyResources(this.ToolStripSeparator35, "ToolStripSeparator35");
			//
			//ImageSelectMenuItem
			//
			this.ImageSelectMenuItem.Name = "ImageSelectMenuItem";
			resources.ApplyResources(this.ImageSelectMenuItem, "ImageSelectMenuItem");
			//
			//ToolStripSeparator8
			//
			this.ToolStripSeparator8.Name = "ToolStripSeparator8";
			resources.ApplyResources(this.ToolStripSeparator8, "ToolStripSeparator8");
			//
			//HashToggleMenuItem
			//
			this.HashToggleMenuItem.CheckOnClick = true;
			this.HashToggleMenuItem.Name = "HashToggleMenuItem";
			resources.ApplyResources(this.HashToggleMenuItem, "HashToggleMenuItem");
			//
			//HashManageMenuItem
			//
			this.HashManageMenuItem.Name = "HashManageMenuItem";
			resources.ApplyResources(this.HashManageMenuItem, "HashManageMenuItem");
			//
			//SplitContainer4
			//
			this.SplitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.SplitContainer4, "SplitContainer4");
			this.SplitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.SplitContainer4.Name = "SplitContainer4";
			//
			//SplitContainer4.Panel1
			//
			this.SplitContainer4.Panel1.Controls.Add(this.SplitContainer1);
			this.SplitContainer4.TabStop = false;
			//
			//SplitContainer1
			//
			this.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.SplitContainer1, "SplitContainer1");
			this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.SplitContainer1.Name = "SplitContainer1";
			//
			//SplitContainer1.Panel1
			//
			this.SplitContainer1.Panel1.Controls.Add(this.TimelinePanel);
			this.SplitContainer1.Panel1.Controls.Add(this.ImageSelectionPanel);
			this.SplitContainer1.Panel1.Controls.Add(this.ProfilePanel);
			//
			//SplitContainer1.Panel2
			//
			this.SplitContainer1.Panel2.Controls.Add(this.SplitContainer3);
			this.SplitContainer1.TabStop = false;
			//
			//TimelinePanel
			//
			this.TimelinePanel.Controls.Add(this.ListTab);
			resources.ApplyResources(this.TimelinePanel, "TimelinePanel");
			this.TimelinePanel.Name = "TimelinePanel";
			//
			//ListTab
			//
			resources.ApplyResources(this.ListTab, "ListTab");
			this.ListTab.AllowDrop = true;
			this.ListTab.ContextMenuStrip = this.ContextMenuTabProperty;
			this.ListTab.ImageList = this.TabImage;
			this.ListTab.Multiline = true;
			this.ListTab.Name = "ListTab";
			this.ListTab.SelectedIndex = 0;
			this.ListTab.TabStop = false;
			//
			//ContextMenuTabProperty
			//
			this.ContextMenuTabProperty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.AddTabMenuItem,
				this.TabRenameMenuItem,
				this.ToolStripSeparator20,
				this.UreadManageMenuItem,
				this.NotifyDispMenuItem,
				this.SoundFileComboBox,
				this.ToolStripSeparator18,
				this.FilterEditMenuItem,
				this.ToolStripSeparator19,
				this.ClearTabMenuItem,
				this.ToolStripSeparator11,
				this.DeleteTabMenuItem
			});
			this.ContextMenuTabProperty.Name = "ContextMenuStrip3";
			this.ContextMenuTabProperty.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			resources.ApplyResources(this.ContextMenuTabProperty, "ContextMenuTabProperty");
			//
			//AddTabMenuItem
			//
			this.AddTabMenuItem.Name = "AddTabMenuItem";
			resources.ApplyResources(this.AddTabMenuItem, "AddTabMenuItem");
			//
			//TabRenameMenuItem
			//
			this.TabRenameMenuItem.Name = "TabRenameMenuItem";
			resources.ApplyResources(this.TabRenameMenuItem, "TabRenameMenuItem");
			//
			//ToolStripSeparator20
			//
			this.ToolStripSeparator20.Name = "ToolStripSeparator20";
			resources.ApplyResources(this.ToolStripSeparator20, "ToolStripSeparator20");
			//
			//UreadManageMenuItem
			//
			this.UreadManageMenuItem.CheckOnClick = true;
			this.UreadManageMenuItem.Name = "UreadManageMenuItem";
			resources.ApplyResources(this.UreadManageMenuItem, "UreadManageMenuItem");
			//
			//NotifyDispMenuItem
			//
			this.NotifyDispMenuItem.CheckOnClick = true;
			this.NotifyDispMenuItem.Name = "NotifyDispMenuItem";
			resources.ApplyResources(this.NotifyDispMenuItem, "NotifyDispMenuItem");
			//
			//SoundFileComboBox
			//
			this.SoundFileComboBox.AutoToolTip = true;
			this.SoundFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SoundFileComboBox.Name = "SoundFileComboBox";
			resources.ApplyResources(this.SoundFileComboBox, "SoundFileComboBox");
			//
			//ToolStripSeparator18
			//
			this.ToolStripSeparator18.Name = "ToolStripSeparator18";
			resources.ApplyResources(this.ToolStripSeparator18, "ToolStripSeparator18");
			//
			//FilterEditMenuItem
			//
			this.FilterEditMenuItem.Name = "FilterEditMenuItem";
			resources.ApplyResources(this.FilterEditMenuItem, "FilterEditMenuItem");
			//
			//ToolStripSeparator19
			//
			this.ToolStripSeparator19.Name = "ToolStripSeparator19";
			resources.ApplyResources(this.ToolStripSeparator19, "ToolStripSeparator19");
			//
			//ClearTabMenuItem
			//
			this.ClearTabMenuItem.Name = "ClearTabMenuItem";
			resources.ApplyResources(this.ClearTabMenuItem, "ClearTabMenuItem");
			//
			//ToolStripSeparator11
			//
			this.ToolStripSeparator11.Name = "ToolStripSeparator11";
			resources.ApplyResources(this.ToolStripSeparator11, "ToolStripSeparator11");
			//
			//DeleteTabMenuItem
			//
			this.DeleteTabMenuItem.Name = "DeleteTabMenuItem";
			resources.ApplyResources(this.DeleteTabMenuItem, "DeleteTabMenuItem");
			//
			//TabImage
			//
			this.TabImage.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.TabImage, "TabImage");
			this.TabImage.TransparentColor = System.Drawing.Color.Transparent;
			//
			//ImageSelectionPanel
			//
			resources.ApplyResources(this.ImageSelectionPanel, "ImageSelectionPanel");
			this.ImageSelectionPanel.Controls.Add(this.ImageSelectedPicture);
			this.ImageSelectionPanel.Controls.Add(this.ImagePathPanel);
			this.ImageSelectionPanel.Name = "ImageSelectionPanel";
			//
			//ImageSelectedPicture
			//
			resources.ApplyResources(this.ImageSelectedPicture, "ImageSelectedPicture");
			this.ImageSelectedPicture.Name = "ImageSelectedPicture";
			this.ImageSelectedPicture.TabStop = false;
			//
			//ImagePathPanel
			//
			this.ImagePathPanel.Controls.Add(this.ImagefilePathText);
			this.ImagePathPanel.Controls.Add(this.FilePickButton);
			this.ImagePathPanel.Controls.Add(this.Label2);
			this.ImagePathPanel.Controls.Add(this.ImageServiceCombo);
			this.ImagePathPanel.Controls.Add(this.ImageCancelButton);
			this.ImagePathPanel.Controls.Add(this.Label1);
			resources.ApplyResources(this.ImagePathPanel, "ImagePathPanel");
			this.ImagePathPanel.Name = "ImagePathPanel";
			//
			//ImagefilePathText
			//
			resources.ApplyResources(this.ImagefilePathText, "ImagefilePathText");
			this.ImagefilePathText.Name = "ImagefilePathText";
			//
			//FilePickButton
			//
			resources.ApplyResources(this.FilePickButton, "FilePickButton");
			this.FilePickButton.Name = "FilePickButton";
			this.FilePickButton.UseVisualStyleBackColor = true;
			//
			//Label2
			//
			resources.ApplyResources(this.Label2, "Label2");
			this.Label2.Name = "Label2";
			//
			//ImageServiceCombo
			//
			resources.ApplyResources(this.ImageServiceCombo, "ImageServiceCombo");
			this.ImageServiceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ImageServiceCombo.FormattingEnabled = true;
			this.ImageServiceCombo.Items.AddRange(new object[] {
				resources.GetString("ImageServiceCombo.Items"),
				resources.GetString("ImageServiceCombo.Items1")
			});
			this.ImageServiceCombo.Name = "ImageServiceCombo";
			//
			//ImageCancelButton
			//
			resources.ApplyResources(this.ImageCancelButton, "ImageCancelButton");
			this.ImageCancelButton.Name = "ImageCancelButton";
			this.ImageCancelButton.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Name = "Label1";
			//
			//ProfilePanel
			//
			resources.ApplyResources(this.ProfilePanel, "ProfilePanel");
			this.ProfilePanel.Name = "ProfilePanel";
			//
			//SplitContainer3
			//
			resources.ApplyResources(this.SplitContainer3, "SplitContainer3");
			this.SplitContainer3.Name = "SplitContainer3";
			//
			//SplitContainer3.Panel1
			//
			this.SplitContainer3.Panel1.Controls.Add(this.SplitContainer2);
			//
			//SplitContainer3.Panel2
			//
			this.SplitContainer3.Panel2.Controls.Add(this.PreviewPicture);
			this.SplitContainer3.Panel2.Controls.Add(this.PreviewScrollBar);
			this.SplitContainer3.Panel2Collapsed = true;
			this.SplitContainer3.TabStop = false;
			//
			//SplitContainer2
			//
			resources.ApplyResources(this.SplitContainer2, "SplitContainer2");
			this.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.SplitContainer2.MinimumSize = new System.Drawing.Size(0, 22);
			this.SplitContainer2.Name = "SplitContainer2";
			//
			//SplitContainer2.Panel1
			//
			this.SplitContainer2.Panel1.Controls.Add(this.TableLayoutPanel1);
			//
			//SplitContainer2.Panel2
			//
			this.SplitContainer2.Panel2.Controls.Add(this.StatusText);
			this.SplitContainer2.Panel2.Controls.Add(this.lblLen);
			this.SplitContainer2.Panel2.Controls.Add(this.PostButton);
			this.SplitContainer2.TabStop = false;
			//
			//TableLayoutPanel1
			//
			resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
			this.TableLayoutPanel1.Controls.Add(this.UserPicture, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.NameLabel, 1, 0);
			this.TableLayoutPanel1.Controls.Add(this.PostBrowser, 1, 1);
			this.TableLayoutPanel1.Controls.Add(this.DateTimeLabel, 2, 0);
			this.TableLayoutPanel1.Controls.Add(this.SourceLinkLabel, 3, 0);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			//
			//UserPicture
			//
			this.UserPicture.BackColor = System.Drawing.Color.White;
			this.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.UserPicture.ContextMenuStrip = this.ContextMenuUserPicture;
			resources.ApplyResources(this.UserPicture, "UserPicture");
			this.UserPicture.Name = "UserPicture";
			this.TableLayoutPanel1.SetRowSpan(this.UserPicture, 2);
			this.UserPicture.TabStop = false;
			//
			//ContextMenuUserPicture
			//
			this.ContextMenuUserPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.FollowToolStripMenuItem,
				this.UnFollowToolStripMenuItem,
				this.ShowFriendShipToolStripMenuItem,
				this.ListManageUserContextToolStripMenuItem3,
				this.ToolStripSeparator37,
				this.ShowUserStatusToolStripMenuItem,
				this.SearchPostsDetailNameToolStripMenuItem,
				this.SearchAtPostsDetailNameToolStripMenuItem,
				this.ToolStripMenuItem1,
				this.IconNameToolStripMenuItem,
				this.SaveIconPictureToolStripMenuItem
			});
			this.ContextMenuUserPicture.Name = "ContextMenuStrip3";
			this.ContextMenuUserPicture.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			resources.ApplyResources(this.ContextMenuUserPicture, "ContextMenuUserPicture");
			//
			//FollowToolStripMenuItem
			//
			this.FollowToolStripMenuItem.Name = "FollowToolStripMenuItem";
			resources.ApplyResources(this.FollowToolStripMenuItem, "FollowToolStripMenuItem");
			//
			//UnFollowToolStripMenuItem
			//
			this.UnFollowToolStripMenuItem.Name = "UnFollowToolStripMenuItem";
			resources.ApplyResources(this.UnFollowToolStripMenuItem, "UnFollowToolStripMenuItem");
			//
			//ShowFriendShipToolStripMenuItem
			//
			this.ShowFriendShipToolStripMenuItem.Name = "ShowFriendShipToolStripMenuItem";
			resources.ApplyResources(this.ShowFriendShipToolStripMenuItem, "ShowFriendShipToolStripMenuItem");
			//
			//ListManageUserContextToolStripMenuItem3
			//
			this.ListManageUserContextToolStripMenuItem3.Name = "ListManageUserContextToolStripMenuItem3";
			resources.ApplyResources(this.ListManageUserContextToolStripMenuItem3, "ListManageUserContextToolStripMenuItem3");
			//
			//ToolStripSeparator37
			//
			this.ToolStripSeparator37.Name = "ToolStripSeparator37";
			resources.ApplyResources(this.ToolStripSeparator37, "ToolStripSeparator37");
			//
			//ShowUserStatusToolStripMenuItem
			//
			this.ShowUserStatusToolStripMenuItem.Name = "ShowUserStatusToolStripMenuItem";
			resources.ApplyResources(this.ShowUserStatusToolStripMenuItem, "ShowUserStatusToolStripMenuItem");
			//
			//SearchPostsDetailNameToolStripMenuItem
			//
			this.SearchPostsDetailNameToolStripMenuItem.Name = "SearchPostsDetailNameToolStripMenuItem";
			resources.ApplyResources(this.SearchPostsDetailNameToolStripMenuItem, "SearchPostsDetailNameToolStripMenuItem");
			//
			//SearchAtPostsDetailNameToolStripMenuItem
			//
			this.SearchAtPostsDetailNameToolStripMenuItem.Name = "SearchAtPostsDetailNameToolStripMenuItem";
			resources.ApplyResources(this.SearchAtPostsDetailNameToolStripMenuItem, "SearchAtPostsDetailNameToolStripMenuItem");
			//
			//ToolStripMenuItem1
			//
			this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
			resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
			//
			//IconNameToolStripMenuItem
			//
			this.IconNameToolStripMenuItem.Name = "IconNameToolStripMenuItem";
			resources.ApplyResources(this.IconNameToolStripMenuItem, "IconNameToolStripMenuItem");
			//
			//SaveIconPictureToolStripMenuItem
			//
			this.SaveIconPictureToolStripMenuItem.Name = "SaveIconPictureToolStripMenuItem";
			resources.ApplyResources(this.SaveIconPictureToolStripMenuItem, "SaveIconPictureToolStripMenuItem");
			//
			//NameLabel
			//
			this.NameLabel.AutoEllipsis = true;
			resources.ApplyResources(this.NameLabel, "NameLabel");
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.UseMnemonic = false;
			//
			//PostBrowser
			//
			this.PostBrowser.AllowWebBrowserDrop = false;
			this.TableLayoutPanel1.SetColumnSpan(this.PostBrowser, 3);
			this.PostBrowser.ContextMenuStrip = this.ContextMenuPostBrowser;
			resources.ApplyResources(this.PostBrowser, "PostBrowser");
			this.PostBrowser.IsWebBrowserContextMenuEnabled = false;
			this.PostBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.PostBrowser.Name = "PostBrowser";
			this.PostBrowser.TabStop = false;
			this.PostBrowser.WebBrowserShortcutsEnabled = false;
			//
			//ContextMenuPostBrowser
			//
			this.ContextMenuPostBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SelectionSearchContextMenuItem,
				this.ToolStripSeparator13,
				this.SelectionCopyContextMenuItem,
				this.UrlCopyContextMenuItem,
				this.SelectionAllContextMenuItem,
				this.ToolStripSeparator5,
				this.FollowContextMenuItem,
				this.RemoveContextMenuItem,
				this.FriendshipContextMenuItem,
				this.FriendshipAllMenuItem,
				this.ToolStripSeparator36,
				this.ShowUserStatusContextMenuItem,
				this.SearchPostsDetailToolStripMenuItem,
				this.SearchAtPostsDetailToolStripMenuItem,
				this.ToolStripSeparator32,
				this.IdFilterAddMenuItem,
				this.ListManageUserContextToolStripMenuItem,
				this.ToolStripSeparator33,
				this.UseHashtagMenuItem,
				this.SelectionTranslationToolStripMenuItem,
				this.TranslationToolStripMenuItem
			});
			this.ContextMenuPostBrowser.Name = "ContextMenuStrip4";
			resources.ApplyResources(this.ContextMenuPostBrowser, "ContextMenuPostBrowser");
			//
			//SelectionSearchContextMenuItem
			//
			this.SelectionSearchContextMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SearchGoogleContextMenuItem,
				this.SearchWikipediaContextMenuItem,
				this.SearchYatsContextMenuItem,
				this.SearchPublicSearchContextMenuItem,
				this.CurrentTabToolStripMenuItem
			});
			this.SelectionSearchContextMenuItem.Name = "SelectionSearchContextMenuItem";
			resources.ApplyResources(this.SelectionSearchContextMenuItem, "SelectionSearchContextMenuItem");
			//
			//SearchGoogleContextMenuItem
			//
			this.SearchGoogleContextMenuItem.Name = "SearchGoogleContextMenuItem";
			resources.ApplyResources(this.SearchGoogleContextMenuItem, "SearchGoogleContextMenuItem");
			//
			//SearchWikipediaContextMenuItem
			//
			this.SearchWikipediaContextMenuItem.Name = "SearchWikipediaContextMenuItem";
			resources.ApplyResources(this.SearchWikipediaContextMenuItem, "SearchWikipediaContextMenuItem");
			//
			//SearchYatsContextMenuItem
			//
			this.SearchYatsContextMenuItem.Name = "SearchYatsContextMenuItem";
			resources.ApplyResources(this.SearchYatsContextMenuItem, "SearchYatsContextMenuItem");
			//
			//SearchPublicSearchContextMenuItem
			//
			this.SearchPublicSearchContextMenuItem.Name = "SearchPublicSearchContextMenuItem";
			resources.ApplyResources(this.SearchPublicSearchContextMenuItem, "SearchPublicSearchContextMenuItem");
			//
			//CurrentTabToolStripMenuItem
			//
			this.CurrentTabToolStripMenuItem.Name = "CurrentTabToolStripMenuItem";
			resources.ApplyResources(this.CurrentTabToolStripMenuItem, "CurrentTabToolStripMenuItem");
			//
			//ToolStripSeparator13
			//
			this.ToolStripSeparator13.Name = "ToolStripSeparator13";
			resources.ApplyResources(this.ToolStripSeparator13, "ToolStripSeparator13");
			//
			//SelectionCopyContextMenuItem
			//
			this.SelectionCopyContextMenuItem.Name = "SelectionCopyContextMenuItem";
			resources.ApplyResources(this.SelectionCopyContextMenuItem, "SelectionCopyContextMenuItem");
			//
			//UrlCopyContextMenuItem
			//
			resources.ApplyResources(this.UrlCopyContextMenuItem, "UrlCopyContextMenuItem");
			this.UrlCopyContextMenuItem.Name = "UrlCopyContextMenuItem";
			//
			//SelectionAllContextMenuItem
			//
			this.SelectionAllContextMenuItem.Name = "SelectionAllContextMenuItem";
			resources.ApplyResources(this.SelectionAllContextMenuItem, "SelectionAllContextMenuItem");
			//
			//ToolStripSeparator5
			//
			this.ToolStripSeparator5.Name = "ToolStripSeparator5";
			resources.ApplyResources(this.ToolStripSeparator5, "ToolStripSeparator5");
			//
			//FollowContextMenuItem
			//
			this.FollowContextMenuItem.Name = "FollowContextMenuItem";
			resources.ApplyResources(this.FollowContextMenuItem, "FollowContextMenuItem");
			//
			//RemoveContextMenuItem
			//
			this.RemoveContextMenuItem.Name = "RemoveContextMenuItem";
			resources.ApplyResources(this.RemoveContextMenuItem, "RemoveContextMenuItem");
			//
			//FriendshipContextMenuItem
			//
			this.FriendshipContextMenuItem.Name = "FriendshipContextMenuItem";
			resources.ApplyResources(this.FriendshipContextMenuItem, "FriendshipContextMenuItem");
			//
			//FriendshipAllMenuItem
			//
			this.FriendshipAllMenuItem.Name = "FriendshipAllMenuItem";
			resources.ApplyResources(this.FriendshipAllMenuItem, "FriendshipAllMenuItem");
			//
			//ToolStripSeparator36
			//
			this.ToolStripSeparator36.Name = "ToolStripSeparator36";
			resources.ApplyResources(this.ToolStripSeparator36, "ToolStripSeparator36");
			//
			//ShowUserStatusContextMenuItem
			//
			this.ShowUserStatusContextMenuItem.Name = "ShowUserStatusContextMenuItem";
			resources.ApplyResources(this.ShowUserStatusContextMenuItem, "ShowUserStatusContextMenuItem");
			//
			//SearchPostsDetailToolStripMenuItem
			//
			this.SearchPostsDetailToolStripMenuItem.Name = "SearchPostsDetailToolStripMenuItem";
			resources.ApplyResources(this.SearchPostsDetailToolStripMenuItem, "SearchPostsDetailToolStripMenuItem");
			//
			//SearchAtPostsDetailToolStripMenuItem
			//
			this.SearchAtPostsDetailToolStripMenuItem.Name = "SearchAtPostsDetailToolStripMenuItem";
			resources.ApplyResources(this.SearchAtPostsDetailToolStripMenuItem, "SearchAtPostsDetailToolStripMenuItem");
			//
			//ToolStripSeparator32
			//
			this.ToolStripSeparator32.Name = "ToolStripSeparator32";
			resources.ApplyResources(this.ToolStripSeparator32, "ToolStripSeparator32");
			//
			//IdFilterAddMenuItem
			//
			this.IdFilterAddMenuItem.Name = "IdFilterAddMenuItem";
			resources.ApplyResources(this.IdFilterAddMenuItem, "IdFilterAddMenuItem");
			//
			//ListManageUserContextToolStripMenuItem
			//
			this.ListManageUserContextToolStripMenuItem.Name = "ListManageUserContextToolStripMenuItem";
			resources.ApplyResources(this.ListManageUserContextToolStripMenuItem, "ListManageUserContextToolStripMenuItem");
			//
			//ToolStripSeparator33
			//
			this.ToolStripSeparator33.Name = "ToolStripSeparator33";
			resources.ApplyResources(this.ToolStripSeparator33, "ToolStripSeparator33");
			//
			//UseHashtagMenuItem
			//
			this.UseHashtagMenuItem.Name = "UseHashtagMenuItem";
			resources.ApplyResources(this.UseHashtagMenuItem, "UseHashtagMenuItem");
			//
			//SelectionTranslationToolStripMenuItem
			//
			this.SelectionTranslationToolStripMenuItem.Name = "SelectionTranslationToolStripMenuItem";
			resources.ApplyResources(this.SelectionTranslationToolStripMenuItem, "SelectionTranslationToolStripMenuItem");
			//
			//TranslationToolStripMenuItem
			//
			this.TranslationToolStripMenuItem.Name = "TranslationToolStripMenuItem";
			resources.ApplyResources(this.TranslationToolStripMenuItem, "TranslationToolStripMenuItem");
			//
			//DateTimeLabel
			//
			resources.ApplyResources(this.DateTimeLabel, "DateTimeLabel");
			this.DateTimeLabel.AutoEllipsis = true;
			this.DateTimeLabel.Name = "DateTimeLabel";
			//
			//SourceLinkLabel
			//
			resources.ApplyResources(this.SourceLinkLabel, "SourceLinkLabel");
			this.SourceLinkLabel.AutoEllipsis = true;
			this.SourceLinkLabel.ContextMenuStrip = this.ContextMenuSource;
			this.SourceLinkLabel.MaximumSize = new System.Drawing.Size(130, 0);
			this.SourceLinkLabel.Name = "SourceLinkLabel";
			this.SourceLinkLabel.TabStop = true;
			//
			//ContextMenuSource
			//
			this.ContextMenuSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SourceCopyMenuItem,
				this.SourceUrlCopyMenuItem
			});
			this.ContextMenuSource.Name = "ContextMenuSource";
			resources.ApplyResources(this.ContextMenuSource, "ContextMenuSource");
			//
			//SourceCopyMenuItem
			//
			this.SourceCopyMenuItem.Name = "SourceCopyMenuItem";
			resources.ApplyResources(this.SourceCopyMenuItem, "SourceCopyMenuItem");
			//
			//SourceUrlCopyMenuItem
			//
			this.SourceUrlCopyMenuItem.Name = "SourceUrlCopyMenuItem";
			resources.ApplyResources(this.SourceUrlCopyMenuItem, "SourceUrlCopyMenuItem");
			//
			//StatusText
			//
			resources.ApplyResources(this.StatusText, "StatusText");
			this.StatusText.Name = "StatusText";
			//
			//lblLen
			//
			resources.ApplyResources(this.lblLen, "lblLen");
			this.lblLen.Name = "lblLen";
			//
			//PostButton
			//
			resources.ApplyResources(this.PostButton, "PostButton");
			this.PostButton.Name = "PostButton";
			this.PostButton.TabStop = false;
			this.PostButton.UseVisualStyleBackColor = true;
			//
			//PreviewPicture
			//
			resources.ApplyResources(this.PreviewPicture, "PreviewPicture");
			this.PreviewPicture.Name = "PreviewPicture";
			this.PreviewPicture.TabStop = false;
			//
			//PreviewScrollBar
			//
			resources.ApplyResources(this.PreviewScrollBar, "PreviewScrollBar");
			this.PreviewScrollBar.LargeChange = 1;
			this.PreviewScrollBar.Maximum = 0;
			this.PreviewScrollBar.Name = "PreviewScrollBar";
			//
			//MenuStrip1
			//
			resources.ApplyResources(this.MenuStrip1, "MenuStrip1");
			this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.MenuItemFile,
				this.MenuItemEdit,
				this.MenuItemOperate,
				this.MenuItemTab,
				this.MenuItemCommand,
				this.MenuItemUserStream,
				this.MenuItemHelp
			});
			this.MenuStrip1.Name = "MenuStrip1";
			this.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			//
			//MenuItemFile
			//
			this.MenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SettingFileMenuItem,
				this.ToolStripSeparator21,
				this.SaveFileMenuItem,
				this.ToolStripSeparator23,
				this.NotifyFileMenuItem,
				this.PlaySoundFileMenuItem,
				this.LockListFileMenuItem,
				this.ToolStripSeparator43,
				this.StopRefreshAllMenuItem,
				this.ToolStripSeparator24,
				this.TweenRestartMenuItem,
				this.EndFileMenuItem
			});
			this.MenuItemFile.Name = "MenuItemFile";
			resources.ApplyResources(this.MenuItemFile, "MenuItemFile");
			//
			//SettingFileMenuItem
			//
			this.SettingFileMenuItem.Name = "SettingFileMenuItem";
			resources.ApplyResources(this.SettingFileMenuItem, "SettingFileMenuItem");
			//
			//ToolStripSeparator21
			//
			this.ToolStripSeparator21.Name = "ToolStripSeparator21";
			resources.ApplyResources(this.ToolStripSeparator21, "ToolStripSeparator21");
			//
			//SaveFileMenuItem
			//
			this.SaveFileMenuItem.Name = "SaveFileMenuItem";
			resources.ApplyResources(this.SaveFileMenuItem, "SaveFileMenuItem");
			//
			//ToolStripSeparator23
			//
			this.ToolStripSeparator23.Name = "ToolStripSeparator23";
			resources.ApplyResources(this.ToolStripSeparator23, "ToolStripSeparator23");
			//
			//NotifyFileMenuItem
			//
			this.NotifyFileMenuItem.CheckOnClick = true;
			this.NotifyFileMenuItem.Name = "NotifyFileMenuItem";
			resources.ApplyResources(this.NotifyFileMenuItem, "NotifyFileMenuItem");
			//
			//PlaySoundFileMenuItem
			//
			this.PlaySoundFileMenuItem.CheckOnClick = true;
			this.PlaySoundFileMenuItem.Name = "PlaySoundFileMenuItem";
			resources.ApplyResources(this.PlaySoundFileMenuItem, "PlaySoundFileMenuItem");
			//
			//LockListFileMenuItem
			//
			this.LockListFileMenuItem.CheckOnClick = true;
			this.LockListFileMenuItem.Name = "LockListFileMenuItem";
			resources.ApplyResources(this.LockListFileMenuItem, "LockListFileMenuItem");
			//
			//ToolStripSeparator43
			//
			this.ToolStripSeparator43.Name = "ToolStripSeparator43";
			resources.ApplyResources(this.ToolStripSeparator43, "ToolStripSeparator43");
			//
			//StopRefreshAllMenuItem
			//
			this.StopRefreshAllMenuItem.CheckOnClick = true;
			this.StopRefreshAllMenuItem.Name = "StopRefreshAllMenuItem";
			resources.ApplyResources(this.StopRefreshAllMenuItem, "StopRefreshAllMenuItem");
			//
			//ToolStripSeparator24
			//
			this.ToolStripSeparator24.Name = "ToolStripSeparator24";
			resources.ApplyResources(this.ToolStripSeparator24, "ToolStripSeparator24");
			//
			//TweenRestartMenuItem
			//
			this.TweenRestartMenuItem.Name = "TweenRestartMenuItem";
			resources.ApplyResources(this.TweenRestartMenuItem, "TweenRestartMenuItem");
			//
			//EndFileMenuItem
			//
			this.EndFileMenuItem.Name = "EndFileMenuItem";
			resources.ApplyResources(this.EndFileMenuItem, "EndFileMenuItem");
			//
			//MenuItemEdit
			//
			this.MenuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.UndoRemoveTabMenuItem,
				this.ToolStripSeparator12,
				this.CopySTOTMenuItem,
				this.CopyURLMenuItem,
				this.CopyUserIdStripMenuItem,
				this.ToolStripSeparator6,
				this.MenuItemSubSearch,
				this.MenuItemSearchNext,
				this.MenuItemSearchPrev,
				this.ToolStripSeparator22,
				this.PublicSearchQueryMenuItem
			});
			this.MenuItemEdit.Name = "MenuItemEdit";
			resources.ApplyResources(this.MenuItemEdit, "MenuItemEdit");
			//
			//UndoRemoveTabMenuItem
			//
			this.UndoRemoveTabMenuItem.Name = "UndoRemoveTabMenuItem";
			resources.ApplyResources(this.UndoRemoveTabMenuItem, "UndoRemoveTabMenuItem");
			//
			//ToolStripSeparator12
			//
			this.ToolStripSeparator12.Name = "ToolStripSeparator12";
			resources.ApplyResources(this.ToolStripSeparator12, "ToolStripSeparator12");
			//
			//CopySTOTMenuItem
			//
			this.CopySTOTMenuItem.Name = "CopySTOTMenuItem";
			resources.ApplyResources(this.CopySTOTMenuItem, "CopySTOTMenuItem");
			//
			//CopyURLMenuItem
			//
			this.CopyURLMenuItem.Name = "CopyURLMenuItem";
			resources.ApplyResources(this.CopyURLMenuItem, "CopyURLMenuItem");
			//
			//CopyUserIdStripMenuItem
			//
			this.CopyUserIdStripMenuItem.Name = "CopyUserIdStripMenuItem";
			resources.ApplyResources(this.CopyUserIdStripMenuItem, "CopyUserIdStripMenuItem");
			//
			//ToolStripSeparator6
			//
			this.ToolStripSeparator6.Name = "ToolStripSeparator6";
			resources.ApplyResources(this.ToolStripSeparator6, "ToolStripSeparator6");
			//
			//MenuItemSubSearch
			//
			this.MenuItemSubSearch.Name = "MenuItemSubSearch";
			resources.ApplyResources(this.MenuItemSubSearch, "MenuItemSubSearch");
			//
			//MenuItemSearchNext
			//
			this.MenuItemSearchNext.Name = "MenuItemSearchNext";
			resources.ApplyResources(this.MenuItemSearchNext, "MenuItemSearchNext");
			//
			//MenuItemSearchPrev
			//
			this.MenuItemSearchPrev.Name = "MenuItemSearchPrev";
			resources.ApplyResources(this.MenuItemSearchPrev, "MenuItemSearchPrev");
			//
			//ToolStripSeparator22
			//
			this.ToolStripSeparator22.Name = "ToolStripSeparator22";
			resources.ApplyResources(this.ToolStripSeparator22, "ToolStripSeparator22");
			//
			//PublicSearchQueryMenuItem
			//
			this.PublicSearchQueryMenuItem.Name = "PublicSearchQueryMenuItem";
			resources.ApplyResources(this.PublicSearchQueryMenuItem, "PublicSearchQueryMenuItem");
			//
			//MenuItemOperate
			//
			this.MenuItemOperate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.ReplyOpMenuItem,
				this.ReplyAllOpMenuItem,
				this.DmOpMenuItem,
				this.RtOpMenuItem,
				this.RtUnOpMenuItem,
				this.QtOpMenuItem,
				this.ToolStripSeparator25,
				this.FavOpMenuItem,
				this.FavoriteRetweetMenuItem,
				this.FavoriteRetweetUnofficialMenuItem,
				this.UnFavOpMenuItem,
				this.ToolStripSeparator38,
				this.ShowProfMenuItem,
				this.ShowRelatedStatusesMenuItem2,
				this.ShowUserTimelineToolStripMenuItem,
				this.OpenOpMenuItem,
				this.CreateRuleOpMenuItem,
				this.ListManageMenuItem,
				this.ToolStripSeparator26,
				this.ChangeReadOpMenuItem,
				this.JumpReadOpMenuItem,
				this.ToolStripSeparator27,
				this.SelAllOpMenuItem,
				this.DelOpMenuItem,
				this.RefreshOpMenuItem,
				this.RefreshPrevOpMenuItem
			});
			this.MenuItemOperate.Name = "MenuItemOperate";
			resources.ApplyResources(this.MenuItemOperate, "MenuItemOperate");
			//
			//ReplyOpMenuItem
			//
			this.ReplyOpMenuItem.Name = "ReplyOpMenuItem";
			resources.ApplyResources(this.ReplyOpMenuItem, "ReplyOpMenuItem");
			//
			//ReplyAllOpMenuItem
			//
			this.ReplyAllOpMenuItem.Name = "ReplyAllOpMenuItem";
			resources.ApplyResources(this.ReplyAllOpMenuItem, "ReplyAllOpMenuItem");
			//
			//DmOpMenuItem
			//
			this.DmOpMenuItem.Name = "DmOpMenuItem";
			resources.ApplyResources(this.DmOpMenuItem, "DmOpMenuItem");
			//
			//RtOpMenuItem
			//
			this.RtOpMenuItem.Name = "RtOpMenuItem";
			resources.ApplyResources(this.RtOpMenuItem, "RtOpMenuItem");
			//
			//RtUnOpMenuItem
			//
			this.RtUnOpMenuItem.Name = "RtUnOpMenuItem";
			resources.ApplyResources(this.RtUnOpMenuItem, "RtUnOpMenuItem");
			//
			//QtOpMenuItem
			//
			this.QtOpMenuItem.Name = "QtOpMenuItem";
			resources.ApplyResources(this.QtOpMenuItem, "QtOpMenuItem");
			//
			//ToolStripSeparator25
			//
			this.ToolStripSeparator25.Name = "ToolStripSeparator25";
			resources.ApplyResources(this.ToolStripSeparator25, "ToolStripSeparator25");
			//
			//FavOpMenuItem
			//
			this.FavOpMenuItem.Name = "FavOpMenuItem";
			resources.ApplyResources(this.FavOpMenuItem, "FavOpMenuItem");
			//
			//FavoriteRetweetMenuItem
			//
			this.FavoriteRetweetMenuItem.Name = "FavoriteRetweetMenuItem";
			resources.ApplyResources(this.FavoriteRetweetMenuItem, "FavoriteRetweetMenuItem");
			//
			//FavoriteRetweetUnofficialMenuItem
			//
			this.FavoriteRetweetUnofficialMenuItem.Name = "FavoriteRetweetUnofficialMenuItem";
			resources.ApplyResources(this.FavoriteRetweetUnofficialMenuItem, "FavoriteRetweetUnofficialMenuItem");
			//
			//UnFavOpMenuItem
			//
			this.UnFavOpMenuItem.Name = "UnFavOpMenuItem";
			resources.ApplyResources(this.UnFavOpMenuItem, "UnFavOpMenuItem");
			//
			//ToolStripSeparator38
			//
			this.ToolStripSeparator38.Name = "ToolStripSeparator38";
			resources.ApplyResources(this.ToolStripSeparator38, "ToolStripSeparator38");
			//
			//ShowProfMenuItem
			//
			this.ShowProfMenuItem.Name = "ShowProfMenuItem";
			resources.ApplyResources(this.ShowProfMenuItem, "ShowProfMenuItem");
			//
			//ShowRelatedStatusesMenuItem2
			//
			this.ShowRelatedStatusesMenuItem2.Name = "ShowRelatedStatusesMenuItem2";
			resources.ApplyResources(this.ShowRelatedStatusesMenuItem2, "ShowRelatedStatusesMenuItem2");
			//
			//ShowUserTimelineToolStripMenuItem
			//
			this.ShowUserTimelineToolStripMenuItem.Name = "ShowUserTimelineToolStripMenuItem";
			resources.ApplyResources(this.ShowUserTimelineToolStripMenuItem, "ShowUserTimelineToolStripMenuItem");
			//
			//OpenOpMenuItem
			//
			this.OpenOpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.OpenHomeOpMenuItem,
				this.OpenFavOpMenuItem,
				this.OpenStatusOpMenuItem,
				this.OpenRepSourceOpMenuItem,
				this.OpenFavotterOpMenuItem,
				this.OpenUrlOpMenuItem,
				this.OpenRterHomeMenuItem,
				this.OpenUserSpecifiedUrlMenuItem
			});
			this.OpenOpMenuItem.Name = "OpenOpMenuItem";
			resources.ApplyResources(this.OpenOpMenuItem, "OpenOpMenuItem");
			//
			//OpenHomeOpMenuItem
			//
			this.OpenHomeOpMenuItem.Name = "OpenHomeOpMenuItem";
			resources.ApplyResources(this.OpenHomeOpMenuItem, "OpenHomeOpMenuItem");
			//
			//OpenFavOpMenuItem
			//
			this.OpenFavOpMenuItem.Name = "OpenFavOpMenuItem";
			resources.ApplyResources(this.OpenFavOpMenuItem, "OpenFavOpMenuItem");
			//
			//OpenStatusOpMenuItem
			//
			this.OpenStatusOpMenuItem.Name = "OpenStatusOpMenuItem";
			resources.ApplyResources(this.OpenStatusOpMenuItem, "OpenStatusOpMenuItem");
			//
			//OpenRepSourceOpMenuItem
			//
			this.OpenRepSourceOpMenuItem.Name = "OpenRepSourceOpMenuItem";
			resources.ApplyResources(this.OpenRepSourceOpMenuItem, "OpenRepSourceOpMenuItem");
			//
			//OpenFavotterOpMenuItem
			//
			this.OpenFavotterOpMenuItem.Name = "OpenFavotterOpMenuItem";
			resources.ApplyResources(this.OpenFavotterOpMenuItem, "OpenFavotterOpMenuItem");
			//
			//OpenUrlOpMenuItem
			//
			this.OpenUrlOpMenuItem.Name = "OpenUrlOpMenuItem";
			resources.ApplyResources(this.OpenUrlOpMenuItem, "OpenUrlOpMenuItem");
			//
			//OpenRterHomeMenuItem
			//
			this.OpenRterHomeMenuItem.Name = "OpenRterHomeMenuItem";
			resources.ApplyResources(this.OpenRterHomeMenuItem, "OpenRterHomeMenuItem");
			//
			//OpenUserSpecifiedUrlMenuItem
			//
			this.OpenUserSpecifiedUrlMenuItem.Name = "OpenUserSpecifiedUrlMenuItem";
			resources.ApplyResources(this.OpenUserSpecifiedUrlMenuItem, "OpenUserSpecifiedUrlMenuItem");
			//
			//CreateRuleOpMenuItem
			//
			this.CreateRuleOpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.CreateTabRuleOpMenuItem,
				this.CreateIdRuleOpMenuItem
			});
			this.CreateRuleOpMenuItem.Name = "CreateRuleOpMenuItem";
			resources.ApplyResources(this.CreateRuleOpMenuItem, "CreateRuleOpMenuItem");
			//
			//CreateTabRuleOpMenuItem
			//
			this.CreateTabRuleOpMenuItem.Name = "CreateTabRuleOpMenuItem";
			resources.ApplyResources(this.CreateTabRuleOpMenuItem, "CreateTabRuleOpMenuItem");
			//
			//CreateIdRuleOpMenuItem
			//
			this.CreateIdRuleOpMenuItem.Name = "CreateIdRuleOpMenuItem";
			resources.ApplyResources(this.CreateIdRuleOpMenuItem, "CreateIdRuleOpMenuItem");
			//
			//ListManageMenuItem
			//
			this.ListManageMenuItem.Name = "ListManageMenuItem";
			resources.ApplyResources(this.ListManageMenuItem, "ListManageMenuItem");
			//
			//ToolStripSeparator26
			//
			this.ToolStripSeparator26.Name = "ToolStripSeparator26";
			resources.ApplyResources(this.ToolStripSeparator26, "ToolStripSeparator26");
			//
			//ChangeReadOpMenuItem
			//
			this.ChangeReadOpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.ReadOpMenuItem,
				this.UnreadOpMenuItem
			});
			this.ChangeReadOpMenuItem.Name = "ChangeReadOpMenuItem";
			resources.ApplyResources(this.ChangeReadOpMenuItem, "ChangeReadOpMenuItem");
			//
			//ReadOpMenuItem
			//
			this.ReadOpMenuItem.Name = "ReadOpMenuItem";
			resources.ApplyResources(this.ReadOpMenuItem, "ReadOpMenuItem");
			//
			//UnreadOpMenuItem
			//
			this.UnreadOpMenuItem.Name = "UnreadOpMenuItem";
			resources.ApplyResources(this.UnreadOpMenuItem, "UnreadOpMenuItem");
			//
			//JumpReadOpMenuItem
			//
			this.JumpReadOpMenuItem.Name = "JumpReadOpMenuItem";
			resources.ApplyResources(this.JumpReadOpMenuItem, "JumpReadOpMenuItem");
			//
			//ToolStripSeparator27
			//
			this.ToolStripSeparator27.Name = "ToolStripSeparator27";
			resources.ApplyResources(this.ToolStripSeparator27, "ToolStripSeparator27");
			//
			//SelAllOpMenuItem
			//
			this.SelAllOpMenuItem.Name = "SelAllOpMenuItem";
			resources.ApplyResources(this.SelAllOpMenuItem, "SelAllOpMenuItem");
			//
			//DelOpMenuItem
			//
			this.DelOpMenuItem.Name = "DelOpMenuItem";
			resources.ApplyResources(this.DelOpMenuItem, "DelOpMenuItem");
			//
			//RefreshOpMenuItem
			//
			this.RefreshOpMenuItem.Name = "RefreshOpMenuItem";
			resources.ApplyResources(this.RefreshOpMenuItem, "RefreshOpMenuItem");
			//
			//RefreshPrevOpMenuItem
			//
			this.RefreshPrevOpMenuItem.Name = "RefreshPrevOpMenuItem";
			resources.ApplyResources(this.RefreshPrevOpMenuItem, "RefreshPrevOpMenuItem");
			//
			//MenuItemTab
			//
			this.MenuItemTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.CreateTbMenuItem,
				this.RenameTbMenuItem,
				this.ToolStripSeparator28,
				this.UnreadMngTbMenuItem,
				this.NotifyTbMenuItem,
				this.SoundFileTbComboBox,
				this.ToolStripSeparator29,
				this.EditRuleTbMenuItem,
				this.ToolStripSeparator30,
				this.ClearTbMenuItem,
				this.ToolStripSeparator31,
				this.DeleteTbMenuItem
			});
			this.MenuItemTab.Name = "MenuItemTab";
			resources.ApplyResources(this.MenuItemTab, "MenuItemTab");
			//
			//CreateTbMenuItem
			//
			this.CreateTbMenuItem.Name = "CreateTbMenuItem";
			resources.ApplyResources(this.CreateTbMenuItem, "CreateTbMenuItem");
			//
			//RenameTbMenuItem
			//
			this.RenameTbMenuItem.Name = "RenameTbMenuItem";
			resources.ApplyResources(this.RenameTbMenuItem, "RenameTbMenuItem");
			//
			//ToolStripSeparator28
			//
			this.ToolStripSeparator28.Name = "ToolStripSeparator28";
			resources.ApplyResources(this.ToolStripSeparator28, "ToolStripSeparator28");
			//
			//UnreadMngTbMenuItem
			//
			this.UnreadMngTbMenuItem.CheckOnClick = true;
			this.UnreadMngTbMenuItem.Name = "UnreadMngTbMenuItem";
			resources.ApplyResources(this.UnreadMngTbMenuItem, "UnreadMngTbMenuItem");
			//
			//NotifyTbMenuItem
			//
			this.NotifyTbMenuItem.CheckOnClick = true;
			this.NotifyTbMenuItem.Name = "NotifyTbMenuItem";
			resources.ApplyResources(this.NotifyTbMenuItem, "NotifyTbMenuItem");
			//
			//SoundFileTbComboBox
			//
			this.SoundFileTbComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SoundFileTbComboBox.Name = "SoundFileTbComboBox";
			resources.ApplyResources(this.SoundFileTbComboBox, "SoundFileTbComboBox");
			//
			//ToolStripSeparator29
			//
			this.ToolStripSeparator29.Name = "ToolStripSeparator29";
			resources.ApplyResources(this.ToolStripSeparator29, "ToolStripSeparator29");
			//
			//EditRuleTbMenuItem
			//
			this.EditRuleTbMenuItem.Name = "EditRuleTbMenuItem";
			resources.ApplyResources(this.EditRuleTbMenuItem, "EditRuleTbMenuItem");
			//
			//ToolStripSeparator30
			//
			this.ToolStripSeparator30.Name = "ToolStripSeparator30";
			resources.ApplyResources(this.ToolStripSeparator30, "ToolStripSeparator30");
			//
			//ClearTbMenuItem
			//
			this.ClearTbMenuItem.Name = "ClearTbMenuItem";
			resources.ApplyResources(this.ClearTbMenuItem, "ClearTbMenuItem");
			//
			//ToolStripSeparator31
			//
			this.ToolStripSeparator31.Name = "ToolStripSeparator31";
			resources.ApplyResources(this.ToolStripSeparator31, "ToolStripSeparator31");
			//
			//DeleteTbMenuItem
			//
			this.DeleteTbMenuItem.Name = "DeleteTbMenuItem";
			resources.ApplyResources(this.DeleteTbMenuItem, "DeleteTbMenuItem");
			//
			//MenuItemCommand
			//
			this.MenuItemCommand.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.TinyUrlConvertToolStripMenuItem,
				this.UpdateFollowersMenuItem1,
				this.ToolStripSeparator1,
				this.FollowCommandMenuItem,
				this.RemoveCommandMenuItem,
				this.FriendshipMenuItem,
				this.ToolStripSeparator3,
				this.OwnStatusMenuItem,
				this.OpenOwnHomeMenuItem,
				this.OpenOwnFavedMenuItem,
				this.ToolStripSeparator41,
				this.UserStatusToolStripMenuItem,
				this.UserTimelineToolStripMenuItem,
				this.UserFavorareToolStripMenuItem,
				this.ToolStripSeparator34,
				this.HashToggleToolStripMenuItem,
				this.HashManageToolStripMenuItem,
				this.RtCountMenuItem,
				this.ListManageToolStripMenuItem
			});
			this.MenuItemCommand.Name = "MenuItemCommand";
			resources.ApplyResources(this.MenuItemCommand, "MenuItemCommand");
			//
			//TinyUrlConvertToolStripMenuItem
			//
			this.TinyUrlConvertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.UrlConvertAutoToolStripMenuItem,
				this.UrlUndoToolStripMenuItem,
				this.TinyURLToolStripMenuItem,
				this.IsgdToolStripMenuItem,
				this.TwurlnlToolStripMenuItem,
				this.BitlyToolStripMenuItem,
				this.JmpStripMenuItem,
				this.UxnuMenuItem
			});
			this.TinyUrlConvertToolStripMenuItem.Name = "TinyUrlConvertToolStripMenuItem";
			resources.ApplyResources(this.TinyUrlConvertToolStripMenuItem, "TinyUrlConvertToolStripMenuItem");
			//
			//UrlConvertAutoToolStripMenuItem
			//
			this.UrlConvertAutoToolStripMenuItem.Name = "UrlConvertAutoToolStripMenuItem";
			resources.ApplyResources(this.UrlConvertAutoToolStripMenuItem, "UrlConvertAutoToolStripMenuItem");
			//
			//UrlUndoToolStripMenuItem
			//
			resources.ApplyResources(this.UrlUndoToolStripMenuItem, "UrlUndoToolStripMenuItem");
			this.UrlUndoToolStripMenuItem.Name = "UrlUndoToolStripMenuItem";
			//
			//TinyURLToolStripMenuItem
			//
			this.TinyURLToolStripMenuItem.Name = "TinyURLToolStripMenuItem";
			resources.ApplyResources(this.TinyURLToolStripMenuItem, "TinyURLToolStripMenuItem");
			//
			//IsgdToolStripMenuItem
			//
			this.IsgdToolStripMenuItem.Name = "IsgdToolStripMenuItem";
			resources.ApplyResources(this.IsgdToolStripMenuItem, "IsgdToolStripMenuItem");
			//
			//TwurlnlToolStripMenuItem
			//
			this.TwurlnlToolStripMenuItem.Name = "TwurlnlToolStripMenuItem";
			resources.ApplyResources(this.TwurlnlToolStripMenuItem, "TwurlnlToolStripMenuItem");
			//
			//BitlyToolStripMenuItem
			//
			this.BitlyToolStripMenuItem.Name = "BitlyToolStripMenuItem";
			resources.ApplyResources(this.BitlyToolStripMenuItem, "BitlyToolStripMenuItem");
			//
			//JmpStripMenuItem
			//
			this.JmpStripMenuItem.Name = "JmpStripMenuItem";
			resources.ApplyResources(this.JmpStripMenuItem, "JmpStripMenuItem");
			//
			//UxnuMenuItem
			//
			this.UxnuMenuItem.Name = "UxnuMenuItem";
			resources.ApplyResources(this.UxnuMenuItem, "UxnuMenuItem");
			//
			//UpdateFollowersMenuItem1
			//
			this.UpdateFollowersMenuItem1.Name = "UpdateFollowersMenuItem1";
			resources.ApplyResources(this.UpdateFollowersMenuItem1, "UpdateFollowersMenuItem1");
			//
			//ToolStripSeparator1
			//
			this.ToolStripSeparator1.Name = "ToolStripSeparator1";
			resources.ApplyResources(this.ToolStripSeparator1, "ToolStripSeparator1");
			//
			//FollowCommandMenuItem
			//
			this.FollowCommandMenuItem.Name = "FollowCommandMenuItem";
			resources.ApplyResources(this.FollowCommandMenuItem, "FollowCommandMenuItem");
			//
			//RemoveCommandMenuItem
			//
			this.RemoveCommandMenuItem.Name = "RemoveCommandMenuItem";
			resources.ApplyResources(this.RemoveCommandMenuItem, "RemoveCommandMenuItem");
			//
			//FriendshipMenuItem
			//
			this.FriendshipMenuItem.Name = "FriendshipMenuItem";
			resources.ApplyResources(this.FriendshipMenuItem, "FriendshipMenuItem");
			//
			//ToolStripSeparator3
			//
			this.ToolStripSeparator3.Name = "ToolStripSeparator3";
			resources.ApplyResources(this.ToolStripSeparator3, "ToolStripSeparator3");
			//
			//OwnStatusMenuItem
			//
			this.OwnStatusMenuItem.Name = "OwnStatusMenuItem";
			resources.ApplyResources(this.OwnStatusMenuItem, "OwnStatusMenuItem");
			//
			//OpenOwnHomeMenuItem
			//
			this.OpenOwnHomeMenuItem.Name = "OpenOwnHomeMenuItem";
			resources.ApplyResources(this.OpenOwnHomeMenuItem, "OpenOwnHomeMenuItem");
			//
			//OpenOwnFavedMenuItem
			//
			this.OpenOwnFavedMenuItem.Name = "OpenOwnFavedMenuItem";
			resources.ApplyResources(this.OpenOwnFavedMenuItem, "OpenOwnFavedMenuItem");
			//
			//ToolStripSeparator41
			//
			this.ToolStripSeparator41.Name = "ToolStripSeparator41";
			resources.ApplyResources(this.ToolStripSeparator41, "ToolStripSeparator41");
			//
			//UserStatusToolStripMenuItem
			//
			this.UserStatusToolStripMenuItem.Name = "UserStatusToolStripMenuItem";
			resources.ApplyResources(this.UserStatusToolStripMenuItem, "UserStatusToolStripMenuItem");
			//
			//UserTimelineToolStripMenuItem
			//
			this.UserTimelineToolStripMenuItem.Name = "UserTimelineToolStripMenuItem";
			resources.ApplyResources(this.UserTimelineToolStripMenuItem, "UserTimelineToolStripMenuItem");
			//
			//UserFavorareToolStripMenuItem
			//
			this.UserFavorareToolStripMenuItem.Name = "UserFavorareToolStripMenuItem";
			resources.ApplyResources(this.UserFavorareToolStripMenuItem, "UserFavorareToolStripMenuItem");
			//
			//ToolStripSeparator34
			//
			this.ToolStripSeparator34.Name = "ToolStripSeparator34";
			resources.ApplyResources(this.ToolStripSeparator34, "ToolStripSeparator34");
			//
			//HashToggleToolStripMenuItem
			//
			this.HashToggleToolStripMenuItem.CheckOnClick = true;
			this.HashToggleToolStripMenuItem.Name = "HashToggleToolStripMenuItem";
			resources.ApplyResources(this.HashToggleToolStripMenuItem, "HashToggleToolStripMenuItem");
			//
			//HashManageToolStripMenuItem
			//
			this.HashManageToolStripMenuItem.Name = "HashManageToolStripMenuItem";
			resources.ApplyResources(this.HashManageToolStripMenuItem, "HashManageToolStripMenuItem");
			//
			//RtCountMenuItem
			//
			this.RtCountMenuItem.Name = "RtCountMenuItem";
			resources.ApplyResources(this.RtCountMenuItem, "RtCountMenuItem");
			//
			//ListManageToolStripMenuItem
			//
			this.ListManageToolStripMenuItem.Name = "ListManageToolStripMenuItem";
			resources.ApplyResources(this.ListManageToolStripMenuItem, "ListManageToolStripMenuItem");
			//
			//MenuItemUserStream
			//
			this.MenuItemUserStream.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.StopToolStripMenuItem,
				this.ToolStripSeparator40,
				this.TrackToolStripMenuItem,
				this.AllrepliesToolStripMenuItem,
				this.ToolStripSeparator42,
				this.EventViewerMenuItem
			});
			resources.ApplyResources(this.MenuItemUserStream, "MenuItemUserStream");
			this.MenuItemUserStream.Name = "MenuItemUserStream";
			//
			//StopToolStripMenuItem
			//
			this.StopToolStripMenuItem.Name = "StopToolStripMenuItem";
			resources.ApplyResources(this.StopToolStripMenuItem, "StopToolStripMenuItem");
			//
			//ToolStripSeparator40
			//
			this.ToolStripSeparator40.Name = "ToolStripSeparator40";
			resources.ApplyResources(this.ToolStripSeparator40, "ToolStripSeparator40");
			//
			//TrackToolStripMenuItem
			//
			this.TrackToolStripMenuItem.CheckOnClick = true;
			this.TrackToolStripMenuItem.Name = "TrackToolStripMenuItem";
			resources.ApplyResources(this.TrackToolStripMenuItem, "TrackToolStripMenuItem");
			//
			//AllrepliesToolStripMenuItem
			//
			this.AllrepliesToolStripMenuItem.CheckOnClick = true;
			this.AllrepliesToolStripMenuItem.Name = "AllrepliesToolStripMenuItem";
			resources.ApplyResources(this.AllrepliesToolStripMenuItem, "AllrepliesToolStripMenuItem");
			//
			//ToolStripSeparator42
			//
			this.ToolStripSeparator42.Name = "ToolStripSeparator42";
			resources.ApplyResources(this.ToolStripSeparator42, "ToolStripSeparator42");
			//
			//EventViewerMenuItem
			//
			this.EventViewerMenuItem.Name = "EventViewerMenuItem";
			resources.ApplyResources(this.EventViewerMenuItem, "EventViewerMenuItem");
			//
			//MenuItemHelp
			//
			this.MenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.MatomeMenuItem,
				this.ShortcutKeyListMenuItem,
				this.ToolStripSeparator16,
				this.VerUpMenuItem,
				this.ToolStripSeparator14,
				this.ApiInfoMenuItem,
				this.ToolStripSeparator7,
				this.AboutMenuItem,
				this.DebugModeToolStripMenuItem
			});
			this.MenuItemHelp.Name = "MenuItemHelp";
			resources.ApplyResources(this.MenuItemHelp, "MenuItemHelp");
			//
			//MatomeMenuItem
			//
			this.MatomeMenuItem.Name = "MatomeMenuItem";
			resources.ApplyResources(this.MatomeMenuItem, "MatomeMenuItem");
			//
			//ShortcutKeyListMenuItem
			//
			this.ShortcutKeyListMenuItem.Name = "ShortcutKeyListMenuItem";
			resources.ApplyResources(this.ShortcutKeyListMenuItem, "ShortcutKeyListMenuItem");
			//
			//ToolStripSeparator16
			//
			this.ToolStripSeparator16.Name = "ToolStripSeparator16";
			resources.ApplyResources(this.ToolStripSeparator16, "ToolStripSeparator16");
			//
			//VerUpMenuItem
			//
			this.VerUpMenuItem.Name = "VerUpMenuItem";
			resources.ApplyResources(this.VerUpMenuItem, "VerUpMenuItem");
			//
			//ToolStripSeparator14
			//
			this.ToolStripSeparator14.Name = "ToolStripSeparator14";
			resources.ApplyResources(this.ToolStripSeparator14, "ToolStripSeparator14");
			//
			//ApiInfoMenuItem
			//
			this.ApiInfoMenuItem.Name = "ApiInfoMenuItem";
			resources.ApplyResources(this.ApiInfoMenuItem, "ApiInfoMenuItem");
			//
			//ToolStripSeparator7
			//
			this.ToolStripSeparator7.Name = "ToolStripSeparator7";
			resources.ApplyResources(this.ToolStripSeparator7, "ToolStripSeparator7");
			//
			//AboutMenuItem
			//
			this.AboutMenuItem.Name = "AboutMenuItem";
			resources.ApplyResources(this.AboutMenuItem, "AboutMenuItem");
			//
			//DebugModeToolStripMenuItem
			//
			this.DebugModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.DumpPostClassToolStripMenuItem,
				this.TraceOutToolStripMenuItem,
				this.CacheInfoMenuItem
			});
			this.DebugModeToolStripMenuItem.Name = "DebugModeToolStripMenuItem";
			resources.ApplyResources(this.DebugModeToolStripMenuItem, "DebugModeToolStripMenuItem");
			//
			//DumpPostClassToolStripMenuItem
			//
			this.DumpPostClassToolStripMenuItem.CheckOnClick = true;
			this.DumpPostClassToolStripMenuItem.Name = "DumpPostClassToolStripMenuItem";
			resources.ApplyResources(this.DumpPostClassToolStripMenuItem, "DumpPostClassToolStripMenuItem");
			//
			//TraceOutToolStripMenuItem
			//
			this.TraceOutToolStripMenuItem.CheckOnClick = true;
			this.TraceOutToolStripMenuItem.Name = "TraceOutToolStripMenuItem";
			resources.ApplyResources(this.TraceOutToolStripMenuItem, "TraceOutToolStripMenuItem");
			//
			//CacheInfoMenuItem
			//
			this.CacheInfoMenuItem.Name = "CacheInfoMenuItem";
			resources.ApplyResources(this.CacheInfoMenuItem, "CacheInfoMenuItem");
			//
			//ContextMenuOperate
			//
			this.ContextMenuOperate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.ReplyStripMenuItem,
				this.ReplyAllStripMenuItem,
				this.DMStripMenuItem,
				this.ReTweetOriginalStripMenuItem,
				this.ReTweetStripMenuItem,
				this.QuoteStripMenuItem,
				this.ToolStripSeparator39,
				this.FavAddToolStripMenuItem,
				this.FavoriteRetweetContextMenu,
				this.FavoriteRetweetUnofficialContextMenu,
				this.FavRemoveToolStripMenuItem,
				this.ToolStripSeparator2,
				this.ShowProfileMenuItem,
				this.ShowRelatedStatusesMenuItem,
				this.ShowUserTimelineContextMenuItem,
				this.ToolStripMenuItem6,
				this.ToolStripMenuItem7,
				this.ListManageUserContextToolStripMenuItem2,
				this.ToolStripSeparator4,
				this.ToolStripMenuItem11,
				this.JumpUnreadMenuItem,
				this.ToolStripSeparator10,
				this.SelectAllMenuItem,
				this.DeleteStripMenuItem,
				this.RefreshStripMenuItem,
				this.RefreshMoreStripMenuItem
			});
			this.ContextMenuOperate.Name = "ContextMenuStrip2";
			this.ContextMenuOperate.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			resources.ApplyResources(this.ContextMenuOperate, "ContextMenuOperate");
			//
			//ReplyStripMenuItem
			//
			this.ReplyStripMenuItem.Name = "ReplyStripMenuItem";
			resources.ApplyResources(this.ReplyStripMenuItem, "ReplyStripMenuItem");
			//
			//ReplyAllStripMenuItem
			//
			this.ReplyAllStripMenuItem.Name = "ReplyAllStripMenuItem";
			resources.ApplyResources(this.ReplyAllStripMenuItem, "ReplyAllStripMenuItem");
			//
			//DMStripMenuItem
			//
			this.DMStripMenuItem.Name = "DMStripMenuItem";
			resources.ApplyResources(this.DMStripMenuItem, "DMStripMenuItem");
			//
			//ReTweetOriginalStripMenuItem
			//
			this.ReTweetOriginalStripMenuItem.Name = "ReTweetOriginalStripMenuItem";
			resources.ApplyResources(this.ReTweetOriginalStripMenuItem, "ReTweetOriginalStripMenuItem");
			//
			//ReTweetStripMenuItem
			//
			this.ReTweetStripMenuItem.Name = "ReTweetStripMenuItem";
			resources.ApplyResources(this.ReTweetStripMenuItem, "ReTweetStripMenuItem");
			//
			//QuoteStripMenuItem
			//
			this.QuoteStripMenuItem.Name = "QuoteStripMenuItem";
			resources.ApplyResources(this.QuoteStripMenuItem, "QuoteStripMenuItem");
			//
			//ToolStripSeparator39
			//
			this.ToolStripSeparator39.Name = "ToolStripSeparator39";
			resources.ApplyResources(this.ToolStripSeparator39, "ToolStripSeparator39");
			//
			//FavAddToolStripMenuItem
			//
			this.FavAddToolStripMenuItem.Name = "FavAddToolStripMenuItem";
			resources.ApplyResources(this.FavAddToolStripMenuItem, "FavAddToolStripMenuItem");
			//
			//FavoriteRetweetContextMenu
			//
			this.FavoriteRetweetContextMenu.Name = "FavoriteRetweetContextMenu";
			resources.ApplyResources(this.FavoriteRetweetContextMenu, "FavoriteRetweetContextMenu");
			//
			//FavoriteRetweetUnofficialContextMenu
			//
			this.FavoriteRetweetUnofficialContextMenu.Name = "FavoriteRetweetUnofficialContextMenu";
			resources.ApplyResources(this.FavoriteRetweetUnofficialContextMenu, "FavoriteRetweetUnofficialContextMenu");
			//
			//FavRemoveToolStripMenuItem
			//
			this.FavRemoveToolStripMenuItem.Name = "FavRemoveToolStripMenuItem";
			resources.ApplyResources(this.FavRemoveToolStripMenuItem, "FavRemoveToolStripMenuItem");
			//
			//ToolStripSeparator2
			//
			this.ToolStripSeparator2.Name = "ToolStripSeparator2";
			resources.ApplyResources(this.ToolStripSeparator2, "ToolStripSeparator2");
			//
			//ShowProfileMenuItem
			//
			this.ShowProfileMenuItem.Name = "ShowProfileMenuItem";
			resources.ApplyResources(this.ShowProfileMenuItem, "ShowProfileMenuItem");
			//
			//ShowRelatedStatusesMenuItem
			//
			this.ShowRelatedStatusesMenuItem.Name = "ShowRelatedStatusesMenuItem";
			resources.ApplyResources(this.ShowRelatedStatusesMenuItem, "ShowRelatedStatusesMenuItem");
			//
			//ShowUserTimelineContextMenuItem
			//
			this.ShowUserTimelineContextMenuItem.Name = "ShowUserTimelineContextMenuItem";
			resources.ApplyResources(this.ShowUserTimelineContextMenuItem, "ShowUserTimelineContextMenuItem");
			//
			//ToolStripMenuItem6
			//
			this.ToolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.MoveToHomeToolStripMenuItem,
				this.MoveToFavToolStripMenuItem,
				this.StatusOpenMenuItem,
				this.RepliedStatusOpenMenuItem,
				this.FavorareMenuItem,
				this.OpenURLMenuItem,
				this.MoveToRTHomeMenuItem,
				this.OpenUserSpecifiedUrlMenuItem2
			});
			this.ToolStripMenuItem6.Name = "ToolStripMenuItem6";
			resources.ApplyResources(this.ToolStripMenuItem6, "ToolStripMenuItem6");
			//
			//MoveToHomeToolStripMenuItem
			//
			this.MoveToHomeToolStripMenuItem.Name = "MoveToHomeToolStripMenuItem";
			resources.ApplyResources(this.MoveToHomeToolStripMenuItem, "MoveToHomeToolStripMenuItem");
			//
			//MoveToFavToolStripMenuItem
			//
			this.MoveToFavToolStripMenuItem.Name = "MoveToFavToolStripMenuItem";
			resources.ApplyResources(this.MoveToFavToolStripMenuItem, "MoveToFavToolStripMenuItem");
			//
			//StatusOpenMenuItem
			//
			this.StatusOpenMenuItem.Name = "StatusOpenMenuItem";
			resources.ApplyResources(this.StatusOpenMenuItem, "StatusOpenMenuItem");
			//
			//RepliedStatusOpenMenuItem
			//
			this.RepliedStatusOpenMenuItem.Name = "RepliedStatusOpenMenuItem";
			resources.ApplyResources(this.RepliedStatusOpenMenuItem, "RepliedStatusOpenMenuItem");
			//
			//FavorareMenuItem
			//
			this.FavorareMenuItem.Name = "FavorareMenuItem";
			resources.ApplyResources(this.FavorareMenuItem, "FavorareMenuItem");
			//
			//OpenURLMenuItem
			//
			this.OpenURLMenuItem.Name = "OpenURLMenuItem";
			resources.ApplyResources(this.OpenURLMenuItem, "OpenURLMenuItem");
			//
			//MoveToRTHomeMenuItem
			//
			this.MoveToRTHomeMenuItem.Name = "MoveToRTHomeMenuItem";
			resources.ApplyResources(this.MoveToRTHomeMenuItem, "MoveToRTHomeMenuItem");
			//
			//OpenUserSpecifiedUrlMenuItem2
			//
			this.OpenUserSpecifiedUrlMenuItem2.Name = "OpenUserSpecifiedUrlMenuItem2";
			resources.ApplyResources(this.OpenUserSpecifiedUrlMenuItem2, "OpenUserSpecifiedUrlMenuItem2");
			//
			//ToolStripMenuItem7
			//
			this.ToolStripMenuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.TabMenuItem,
				this.IDRuleMenuItem
			});
			this.ToolStripMenuItem7.Name = "ToolStripMenuItem7";
			resources.ApplyResources(this.ToolStripMenuItem7, "ToolStripMenuItem7");
			//
			//TabMenuItem
			//
			this.TabMenuItem.Name = "TabMenuItem";
			resources.ApplyResources(this.TabMenuItem, "TabMenuItem");
			//
			//IDRuleMenuItem
			//
			this.IDRuleMenuItem.Name = "IDRuleMenuItem";
			resources.ApplyResources(this.IDRuleMenuItem, "IDRuleMenuItem");
			//
			//ListManageUserContextToolStripMenuItem2
			//
			this.ListManageUserContextToolStripMenuItem2.Name = "ListManageUserContextToolStripMenuItem2";
			resources.ApplyResources(this.ListManageUserContextToolStripMenuItem2, "ListManageUserContextToolStripMenuItem2");
			//
			//ToolStripSeparator4
			//
			this.ToolStripSeparator4.Name = "ToolStripSeparator4";
			resources.ApplyResources(this.ToolStripSeparator4, "ToolStripSeparator4");
			//
			//ToolStripMenuItem11
			//
			this.ToolStripMenuItem11.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.ReadedStripMenuItem,
				this.UnreadStripMenuItem
			});
			this.ToolStripMenuItem11.Name = "ToolStripMenuItem11";
			resources.ApplyResources(this.ToolStripMenuItem11, "ToolStripMenuItem11");
			//
			//ReadedStripMenuItem
			//
			this.ReadedStripMenuItem.Name = "ReadedStripMenuItem";
			resources.ApplyResources(this.ReadedStripMenuItem, "ReadedStripMenuItem");
			//
			//UnreadStripMenuItem
			//
			this.UnreadStripMenuItem.Name = "UnreadStripMenuItem";
			resources.ApplyResources(this.UnreadStripMenuItem, "UnreadStripMenuItem");
			//
			//JumpUnreadMenuItem
			//
			this.JumpUnreadMenuItem.Name = "JumpUnreadMenuItem";
			resources.ApplyResources(this.JumpUnreadMenuItem, "JumpUnreadMenuItem");
			//
			//ToolStripSeparator10
			//
			this.ToolStripSeparator10.Name = "ToolStripSeparator10";
			resources.ApplyResources(this.ToolStripSeparator10, "ToolStripSeparator10");
			//
			//SelectAllMenuItem
			//
			this.SelectAllMenuItem.Name = "SelectAllMenuItem";
			resources.ApplyResources(this.SelectAllMenuItem, "SelectAllMenuItem");
			//
			//DeleteStripMenuItem
			//
			this.DeleteStripMenuItem.Name = "DeleteStripMenuItem";
			resources.ApplyResources(this.DeleteStripMenuItem, "DeleteStripMenuItem");
			//
			//RefreshStripMenuItem
			//
			this.RefreshStripMenuItem.Name = "RefreshStripMenuItem";
			resources.ApplyResources(this.RefreshStripMenuItem, "RefreshStripMenuItem");
			//
			//RefreshMoreStripMenuItem
			//
			this.RefreshMoreStripMenuItem.Name = "RefreshMoreStripMenuItem";
			resources.ApplyResources(this.RefreshMoreStripMenuItem, "RefreshMoreStripMenuItem");
			//
			//ContextMenuFile
			//
			this.ContextMenuFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SettingStripMenuItem,
				this.ToolStripSeparator9,
				this.SaveLogMenuItem,
				this.ToolStripSeparator17,
				this.NewPostPopMenuItem,
				this.PlaySoundMenuItem,
				this.ListLockMenuItem,
				this.ToolStripSeparator15,
				this.EndToolStripMenuItem
			});
			this.ContextMenuFile.Name = "ContextMenuStrip1";
			this.ContextMenuFile.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.ContextMenuFile.ShowCheckMargin = true;
			this.ContextMenuFile.ShowImageMargin = false;
			resources.ApplyResources(this.ContextMenuFile, "ContextMenuFile");
			//
			//SettingStripMenuItem
			//
			this.SettingStripMenuItem.Name = "SettingStripMenuItem";
			resources.ApplyResources(this.SettingStripMenuItem, "SettingStripMenuItem");
			//
			//ToolStripSeparator9
			//
			this.ToolStripSeparator9.Name = "ToolStripSeparator9";
			resources.ApplyResources(this.ToolStripSeparator9, "ToolStripSeparator9");
			//
			//SaveLogMenuItem
			//
			this.SaveLogMenuItem.Name = "SaveLogMenuItem";
			resources.ApplyResources(this.SaveLogMenuItem, "SaveLogMenuItem");
			//
			//ToolStripSeparator17
			//
			this.ToolStripSeparator17.Name = "ToolStripSeparator17";
			resources.ApplyResources(this.ToolStripSeparator17, "ToolStripSeparator17");
			//
			//NewPostPopMenuItem
			//
			this.NewPostPopMenuItem.CheckOnClick = true;
			this.NewPostPopMenuItem.Name = "NewPostPopMenuItem";
			resources.ApplyResources(this.NewPostPopMenuItem, "NewPostPopMenuItem");
			//
			//PlaySoundMenuItem
			//
			this.PlaySoundMenuItem.CheckOnClick = true;
			this.PlaySoundMenuItem.Name = "PlaySoundMenuItem";
			resources.ApplyResources(this.PlaySoundMenuItem, "PlaySoundMenuItem");
			//
			//ListLockMenuItem
			//
			this.ListLockMenuItem.CheckOnClick = true;
			this.ListLockMenuItem.Name = "ListLockMenuItem";
			resources.ApplyResources(this.ListLockMenuItem, "ListLockMenuItem");
			//
			//ToolStripSeparator15
			//
			this.ToolStripSeparator15.Name = "ToolStripSeparator15";
			resources.ApplyResources(this.ToolStripSeparator15, "ToolStripSeparator15");
			//
			//EndToolStripMenuItem
			//
			this.EndToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.EndToolStripMenuItem.Name = "EndToolStripMenuItem";
			resources.ApplyResources(this.EndToolStripMenuItem, "EndToolStripMenuItem");
			//
			//NotifyIcon1
			//
			this.NotifyIcon1.ContextMenuStrip = this.ContextMenuFile;
			resources.ApplyResources(this.NotifyIcon1, "NotifyIcon1");
			//
			//TimerRefreshIcon
			//
			this.TimerRefreshIcon.Interval = 50;
			//
			//OpenFileDialog1
			//
			this.OpenFileDialog1.FileName = "OpenFileDialog1";
			//
			//PostStateImageList
			//
			this.PostStateImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("PostStateImageList.ImageStream");
			this.PostStateImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.PostStateImageList.Images.SetKeyName(0, "S0.ico");
			this.PostStateImageList.Images.SetKeyName(1, "S1.ico");
			this.PostStateImageList.Images.SetKeyName(2, "S2.ico");
			this.PostStateImageList.Images.SetKeyName(3, "S3.ico");
			this.PostStateImageList.Images.SetKeyName(4, "S4.ico");
			this.PostStateImageList.Images.SetKeyName(5, "S5.ico");
			this.PostStateImageList.Images.SetKeyName(6, "S6.ico");
			this.PostStateImageList.Images.SetKeyName(7, "S7.ico");
			this.PostStateImageList.Images.SetKeyName(8, "S8.ico");
			this.PostStateImageList.Images.SetKeyName(9, "S9.ico");
			this.PostStateImageList.Images.SetKeyName(10, "S10.ico");
			this.PostStateImageList.Images.SetKeyName(11, "S11.ico");
			this.PostStateImageList.Images.SetKeyName(12, "S12.ico");
			this.PostStateImageList.Images.SetKeyName(13, "S13.ico");
			this.PostStateImageList.Images.SetKeyName(14, "S14.ico");
			//
			//TweenMain
			//
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.ToolStripContainer1);
			this.Name = "TweenMain";
			this.ToolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
			this.ToolStripContainer1.BottomToolStripPanel.PerformLayout();
			this.ToolStripContainer1.ContentPanel.ResumeLayout(false);
			this.ToolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.ToolStripContainer1.TopToolStripPanel.PerformLayout();
			this.ToolStripContainer1.ResumeLayout(false);
			this.ToolStripContainer1.PerformLayout();
			this.StatusStrip1.ResumeLayout(false);
			this.StatusStrip1.PerformLayout();
			this.ContextMenuPostMode.ResumeLayout(false);
			this.SplitContainer4.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.SplitContainer4).EndInit();
			this.SplitContainer4.ResumeLayout(false);
			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.SplitContainer1).EndInit();
			this.SplitContainer1.ResumeLayout(false);
			this.TimelinePanel.ResumeLayout(false);
			this.ContextMenuTabProperty.ResumeLayout(false);
			this.ImageSelectionPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.ImageSelectedPicture).EndInit();
			this.ImagePathPanel.ResumeLayout(false);
			this.ImagePathPanel.PerformLayout();
			this.SplitContainer3.Panel1.ResumeLayout(false);
			this.SplitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.SplitContainer3).EndInit();
			this.SplitContainer3.ResumeLayout(false);
			this.SplitContainer2.Panel1.ResumeLayout(false);
			this.SplitContainer2.Panel2.ResumeLayout(false);
			this.SplitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.SplitContainer2).EndInit();
			this.SplitContainer2.ResumeLayout(false);
			this.TableLayoutPanel1.ResumeLayout(false);
			this.TableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.UserPicture).EndInit();
			this.ContextMenuUserPicture.ResumeLayout(false);
			this.ContextMenuPostBrowser.ResumeLayout(false);
			this.ContextMenuSource.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.PreviewPicture).EndInit();
			this.MenuStrip1.ResumeLayout(false);
			this.MenuStrip1.PerformLayout();
			this.ContextMenuOperate.ResumeLayout(false);
			this.ContextMenuFile.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.NotifyIcon withEventsField_NotifyIcon1;
		internal System.Windows.Forms.NotifyIcon NotifyIcon1 {
			get { return withEventsField_NotifyIcon1; }
			set {
				if (withEventsField_NotifyIcon1 != null) {
					withEventsField_NotifyIcon1.BalloonTipClicked -= NotifyIcon1_BalloonTipClicked;
					withEventsField_NotifyIcon1.MouseClick -= NotifyIcon1_MouseClick;
					withEventsField_NotifyIcon1.MouseMove -= NotifyIcon1_MouseMove;
				}
				withEventsField_NotifyIcon1 = value;
				if (withEventsField_NotifyIcon1 != null) {
					withEventsField_NotifyIcon1.BalloonTipClicked += NotifyIcon1_BalloonTipClicked;
					withEventsField_NotifyIcon1.MouseClick += NotifyIcon1_MouseClick;
					withEventsField_NotifyIcon1.MouseMove += NotifyIcon1_MouseMove;
				}
			}
		}
		internal System.Windows.Forms.ContextMenuStrip ContextMenuFile;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_EndToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem EndToolStripMenuItem {
			get { return withEventsField_EndToolStripMenuItem; }
			set {
				if (withEventsField_EndToolStripMenuItem != null) {
					withEventsField_EndToolStripMenuItem.Click -= EndToolStripMenuItem_Click;
				}
				withEventsField_EndToolStripMenuItem = value;
				if (withEventsField_EndToolStripMenuItem != null) {
					withEventsField_EndToolStripMenuItem.Click += EndToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuOperate;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuOperate {
			get { return withEventsField_ContextMenuOperate; }
			set {
				if (withEventsField_ContextMenuOperate != null) {
					withEventsField_ContextMenuOperate.Opening -= ContextMenuOperate_Opening;
				}
				withEventsField_ContextMenuOperate = value;
				if (withEventsField_ContextMenuOperate != null) {
					withEventsField_ContextMenuOperate.Opening += ContextMenuOperate_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DMStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DMStripMenuItem {
			get { return withEventsField_DMStripMenuItem; }
			set {
				if (withEventsField_DMStripMenuItem != null) {
					withEventsField_DMStripMenuItem.Click -= DMStripMenuItem_Click;
				}
				withEventsField_DMStripMenuItem = value;
				if (withEventsField_DMStripMenuItem != null) {
					withEventsField_DMStripMenuItem.Click += DMStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DeleteStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DeleteStripMenuItem {
			get { return withEventsField_DeleteStripMenuItem; }
			set {
				if (withEventsField_DeleteStripMenuItem != null) {
					withEventsField_DeleteStripMenuItem.Click -= DeleteStripMenuItem_Click;
				}
				withEventsField_DeleteStripMenuItem = value;
				if (withEventsField_DeleteStripMenuItem != null) {
					withEventsField_DeleteStripMenuItem.Click += DeleteStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RefreshStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RefreshStripMenuItem {
			get { return withEventsField_RefreshStripMenuItem; }
			set {
				if (withEventsField_RefreshStripMenuItem != null) {
					withEventsField_RefreshStripMenuItem.Click -= RefreshStripMenuItem_Click;
				}
				withEventsField_RefreshStripMenuItem = value;
				if (withEventsField_RefreshStripMenuItem != null) {
					withEventsField_RefreshStripMenuItem.Click += RefreshStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SettingStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SettingStripMenuItem {
			get { return withEventsField_SettingStripMenuItem; }
			set {
				if (withEventsField_SettingStripMenuItem != null) {
					withEventsField_SettingStripMenuItem.Click -= SettingStripMenuItem_Click;
				}
				withEventsField_SettingStripMenuItem = value;
				if (withEventsField_SettingStripMenuItem != null) {
					withEventsField_SettingStripMenuItem.Click += SettingStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
		internal System.Windows.Forms.ImageList TabImage;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_NewPostPopMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem NewPostPopMenuItem {
			get { return withEventsField_NewPostPopMenuItem; }
			set {
				if (withEventsField_NewPostPopMenuItem != null) {
					withEventsField_NewPostPopMenuItem.CheckStateChanged -= NewPostPopMenuItem_CheckStateChanged;
				}
				withEventsField_NewPostPopMenuItem = value;
				if (withEventsField_NewPostPopMenuItem != null) {
					withEventsField_NewPostPopMenuItem.CheckStateChanged += NewPostPopMenuItem_CheckStateChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListLockMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ListLockMenuItem {
			get { return withEventsField_ListLockMenuItem; }
			set {
				if (withEventsField_ListLockMenuItem != null) {
					withEventsField_ListLockMenuItem.CheckStateChanged -= ListLockMenuItem_CheckStateChanged;
				}
				withEventsField_ListLockMenuItem = value;
				if (withEventsField_ListLockMenuItem != null) {
					withEventsField_ListLockMenuItem.CheckStateChanged += ListLockMenuItem_CheckStateChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_JumpUnreadMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem JumpUnreadMenuItem {
			get { return withEventsField_JumpUnreadMenuItem; }
			set {
				if (withEventsField_JumpUnreadMenuItem != null) {
					withEventsField_JumpUnreadMenuItem.Click -= JumpUnreadMenuItem_Click;
				}
				withEventsField_JumpUnreadMenuItem = value;
				if (withEventsField_JumpUnreadMenuItem != null) {
					withEventsField_JumpUnreadMenuItem.Click += JumpUnreadMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator15;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SaveLogMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SaveLogMenuItem {
			get { return withEventsField_SaveLogMenuItem; }
			set {
				if (withEventsField_SaveLogMenuItem != null) {
					withEventsField_SaveLogMenuItem.Click -= SaveLogMenuItem_Click;
				}
				withEventsField_SaveLogMenuItem = value;
				if (withEventsField_SaveLogMenuItem != null) {
					withEventsField_SaveLogMenuItem.Click += SaveLogMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator17;
		internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
		private System.Windows.Forms.Timer withEventsField_TimerRefreshIcon;
		internal System.Windows.Forms.Timer TimerRefreshIcon {
			get { return withEventsField_TimerRefreshIcon; }
			set {
				if (withEventsField_TimerRefreshIcon != null) {
					withEventsField_TimerRefreshIcon.Tick -= TimerRefreshIcon_Tick;
				}
				withEventsField_TimerRefreshIcon = value;
				if (withEventsField_TimerRefreshIcon != null) {
					withEventsField_TimerRefreshIcon.Tick += TimerRefreshIcon_Tick;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuTabProperty;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuTabProperty {
			get { return withEventsField_ContextMenuTabProperty; }
			set {
				if (withEventsField_ContextMenuTabProperty != null) {
					withEventsField_ContextMenuTabProperty.Opening -= ContextMenuTabProperty_Opening;
				}
				withEventsField_ContextMenuTabProperty = value;
				if (withEventsField_ContextMenuTabProperty != null) {
					withEventsField_ContextMenuTabProperty.Opening += ContextMenuTabProperty_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UreadManageMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UreadManageMenuItem {
			get { return withEventsField_UreadManageMenuItem; }
			set {
				if (withEventsField_UreadManageMenuItem != null) {
					withEventsField_UreadManageMenuItem.Click -= UreadManageMenuItem_Click;
				}
				withEventsField_UreadManageMenuItem = value;
				if (withEventsField_UreadManageMenuItem != null) {
					withEventsField_UreadManageMenuItem.Click += UreadManageMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_NotifyDispMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem NotifyDispMenuItem {
			get { return withEventsField_NotifyDispMenuItem; }
			set {
				if (withEventsField_NotifyDispMenuItem != null) {
					withEventsField_NotifyDispMenuItem.Click -= NotifyDispMenuItem_Click;
				}
				withEventsField_NotifyDispMenuItem = value;
				if (withEventsField_NotifyDispMenuItem != null) {
					withEventsField_NotifyDispMenuItem.Click += NotifyDispMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripComboBox withEventsField_SoundFileComboBox;
		internal System.Windows.Forms.ToolStripComboBox SoundFileComboBox {
			get { return withEventsField_SoundFileComboBox; }
			set {
				if (withEventsField_SoundFileComboBox != null) {
					withEventsField_SoundFileComboBox.SelectedIndexChanged -= SoundFileComboBox_SelectedIndexChanged;
				}
				withEventsField_SoundFileComboBox = value;
				if (withEventsField_SoundFileComboBox != null) {
					withEventsField_SoundFileComboBox.SelectedIndexChanged += SoundFileComboBox_SelectedIndexChanged;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator18;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DeleteTabMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DeleteTabMenuItem {
			get { return withEventsField_DeleteTabMenuItem; }
			set {
				if (withEventsField_DeleteTabMenuItem != null) {
					withEventsField_DeleteTabMenuItem.Click -= DeleteTabMenuItem_Click;
				}
				withEventsField_DeleteTabMenuItem = value;
				if (withEventsField_DeleteTabMenuItem != null) {
					withEventsField_DeleteTabMenuItem.Click += DeleteTabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FilterEditMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FilterEditMenuItem {
			get { return withEventsField_FilterEditMenuItem; }
			set {
				if (withEventsField_FilterEditMenuItem != null) {
					withEventsField_FilterEditMenuItem.Click -= FilterEditMenuItem_Click;
				}
				withEventsField_FilterEditMenuItem = value;
				if (withEventsField_FilterEditMenuItem != null) {
					withEventsField_FilterEditMenuItem.Click += FilterEditMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_AddTabMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem AddTabMenuItem {
			get { return withEventsField_AddTabMenuItem; }
			set {
				if (withEventsField_AddTabMenuItem != null) {
					withEventsField_AddTabMenuItem.Click -= AddTabMenuItem_Click;
				}
				withEventsField_AddTabMenuItem = value;
				if (withEventsField_AddTabMenuItem != null) {
					withEventsField_AddTabMenuItem.Click += AddTabMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator20;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator10;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectAllMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectAllMenuItem {
			get { return withEventsField_SelectAllMenuItem; }
			set {
				if (withEventsField_SelectAllMenuItem != null) {
					withEventsField_SelectAllMenuItem.Click -= SelectAllMenuItem_Click;
				}
				withEventsField_SelectAllMenuItem = value;
				if (withEventsField_SelectAllMenuItem != null) {
					withEventsField_SelectAllMenuItem.Click += SelectAllMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ClearTabMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ClearTabMenuItem {
			get { return withEventsField_ClearTabMenuItem; }
			set {
				if (withEventsField_ClearTabMenuItem != null) {
					withEventsField_ClearTabMenuItem.Click -= ClearTabMenuItem_Click;
				}
				withEventsField_ClearTabMenuItem = value;
				if (withEventsField_ClearTabMenuItem != null) {
					withEventsField_ClearTabMenuItem.Click += ClearTabMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_PlaySoundMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem PlaySoundMenuItem {
			get { return withEventsField_PlaySoundMenuItem; }
			set {
				if (withEventsField_PlaySoundMenuItem != null) {
					withEventsField_PlaySoundMenuItem.CheckedChanged -= PlaySoundMenuItem_CheckedChanged;
				}
				withEventsField_PlaySoundMenuItem = value;
				if (withEventsField_PlaySoundMenuItem != null) {
					withEventsField_PlaySoundMenuItem.CheckedChanged += PlaySoundMenuItem_CheckedChanged;
				}
			}
		}
		internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuUserPicture;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuUserPicture {
			get { return withEventsField_ContextMenuUserPicture; }
			set {
				if (withEventsField_ContextMenuUserPicture != null) {
					withEventsField_ContextMenuUserPicture.Opening -= ContextMenuUserPicture_Opening;
				}
				withEventsField_ContextMenuUserPicture = value;
				if (withEventsField_ContextMenuUserPicture != null) {
					withEventsField_ContextMenuUserPicture.Opening += ContextMenuUserPicture_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_IconNameToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem IconNameToolStripMenuItem {
			get { return withEventsField_IconNameToolStripMenuItem; }
			set {
				if (withEventsField_IconNameToolStripMenuItem != null) {
					withEventsField_IconNameToolStripMenuItem.Click -= IconNameToolStripMenuItem_Click;
				}
				withEventsField_IconNameToolStripMenuItem = value;
				if (withEventsField_IconNameToolStripMenuItem != null) {
					withEventsField_IconNameToolStripMenuItem.Click += IconNameToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SaveIconPictureToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SaveIconPictureToolStripMenuItem {
			get { return withEventsField_SaveIconPictureToolStripMenuItem; }
			set {
				if (withEventsField_SaveIconPictureToolStripMenuItem != null) {
					withEventsField_SaveIconPictureToolStripMenuItem.Click -= SaveIconPictureToolStripMenuItem_Click;
				}
				withEventsField_SaveIconPictureToolStripMenuItem = value;
				if (withEventsField_SaveIconPictureToolStripMenuItem != null) {
					withEventsField_SaveIconPictureToolStripMenuItem.Click += SaveIconPictureToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripContainer ToolStripContainer1;
		private System.Windows.Forms.SplitContainer withEventsField_SplitContainer1;
		internal System.Windows.Forms.SplitContainer SplitContainer1 {
			get { return withEventsField_SplitContainer1; }
			set {
				if (withEventsField_SplitContainer1 != null) {
					withEventsField_SplitContainer1.SplitterMoved -= SplitContainer1_SplitterMoved;
				}
				withEventsField_SplitContainer1 = value;
				if (withEventsField_SplitContainer1 != null) {
					withEventsField_SplitContainer1.SplitterMoved += SplitContainer1_SplitterMoved;
				}
			}
		}
		private System.Windows.Forms.TabControl withEventsField_ListTab;
		internal System.Windows.Forms.TabControl ListTab {
			get { return withEventsField_ListTab; }
			set {
				if (withEventsField_ListTab != null) {
					withEventsField_ListTab.Deselected -= ListTab_Deselected;
					withEventsField_ListTab.MouseMove -= ListTab_MouseMove;
					withEventsField_ListTab.SelectedIndexChanged -= ListTab_SelectedIndexChanged;
					withEventsField_ListTab.KeyDown -= ListTab_KeyDown;
					withEventsField_ListTab.MouseClick -= ListTab_MouseClick;
					withEventsField_ListTab.MouseDoubleClick -= Tabs_DoubleClick;
					withEventsField_ListTab.MouseDown -= Tabs_MouseDown;
					withEventsField_ListTab.DragEnter -= Tabs_DragEnter;
					withEventsField_ListTab.DragDrop -= Tabs_DragDrop;
					withEventsField_ListTab.MouseUp -= ListTab_MouseUp;
					withEventsField_ListTab.Selecting -= ListTab_Selecting;
				}
				withEventsField_ListTab = value;
				if (withEventsField_ListTab != null) {
					withEventsField_ListTab.Deselected += ListTab_Deselected;
					withEventsField_ListTab.MouseMove += ListTab_MouseMove;
					withEventsField_ListTab.SelectedIndexChanged += ListTab_SelectedIndexChanged;
					withEventsField_ListTab.KeyDown += ListTab_KeyDown;
					withEventsField_ListTab.MouseClick += ListTab_MouseClick;
					withEventsField_ListTab.MouseDoubleClick += Tabs_DoubleClick;
					withEventsField_ListTab.MouseDown += Tabs_MouseDown;
					withEventsField_ListTab.DragEnter += Tabs_DragEnter;
					withEventsField_ListTab.DragDrop += Tabs_DragDrop;
					withEventsField_ListTab.MouseUp += ListTab_MouseUp;
					withEventsField_ListTab.Selecting += ListTab_Selecting;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemTab;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemTab {
			get { return withEventsField_MenuItemTab; }
			set {
				if (withEventsField_MenuItemTab != null) {
					withEventsField_MenuItemTab.DropDownOpening -= MenuItemTab_DropDownOpening;
				}
				withEventsField_MenuItemTab = value;
				if (withEventsField_MenuItemTab != null) {
					withEventsField_MenuItemTab.DropDownOpening += MenuItemTab_DropDownOpening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemOperate;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemOperate {
			get { return withEventsField_MenuItemOperate; }
			set {
				if (withEventsField_MenuItemOperate != null) {
					withEventsField_MenuItemOperate.DropDownOpening -= MenuItemOperate_DropDownOpening;
				}
				withEventsField_MenuItemOperate = value;
				if (withEventsField_MenuItemOperate != null) {
					withEventsField_MenuItemOperate.DropDownOpening += MenuItemOperate_DropDownOpening;
				}
			}
		}
		internal System.Windows.Forms.StatusStrip StatusStrip1;
		internal System.Windows.Forms.ToolStripStatusLabel StatusLabelUrl;
		private System.Windows.Forms.MenuStrip withEventsField_MenuStrip1;
		internal System.Windows.Forms.MenuStrip MenuStrip1 {
			get { return withEventsField_MenuStrip1; }
			set {
				if (withEventsField_MenuStrip1 != null) {
					withEventsField_MenuStrip1.MenuActivate -= MenuStrip1_MenuActivate;
					withEventsField_MenuStrip1.MenuDeactivate -= MenuStrip1_MenuDeactivate;
				}
				withEventsField_MenuStrip1 = value;
				if (withEventsField_MenuStrip1 != null) {
					withEventsField_MenuStrip1.MenuActivate += MenuStrip1_MenuActivate;
					withEventsField_MenuStrip1.MenuDeactivate += MenuStrip1_MenuDeactivate;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem MenuItemFile;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemEdit;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemEdit {
			get { return withEventsField_MenuItemEdit; }
			set {
				if (withEventsField_MenuItemEdit != null) {
					withEventsField_MenuItemEdit.DropDownOpening -= MenuItemEdit_DropDownOpening;
				}
				withEventsField_MenuItemEdit = value;
				if (withEventsField_MenuItemEdit != null) {
					withEventsField_MenuItemEdit.DropDownOpening += MenuItemEdit_DropDownOpening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CopySTOTMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CopySTOTMenuItem {
			get { return withEventsField_CopySTOTMenuItem; }
			set {
				if (withEventsField_CopySTOTMenuItem != null) {
					withEventsField_CopySTOTMenuItem.Click -= CopySTOTMenuItem_Click;
				}
				withEventsField_CopySTOTMenuItem = value;
				if (withEventsField_CopySTOTMenuItem != null) {
					withEventsField_CopySTOTMenuItem.Click += CopySTOTMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CopyURLMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CopyURLMenuItem {
			get { return withEventsField_CopyURLMenuItem; }
			set {
				if (withEventsField_CopyURLMenuItem != null) {
					withEventsField_CopyURLMenuItem.Click -= CopyURLMenuItem_Click;
				}
				withEventsField_CopyURLMenuItem = value;
				if (withEventsField_CopyURLMenuItem != null) {
					withEventsField_CopyURLMenuItem.Click += CopyURLMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemSubSearch;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemSubSearch {
			get { return withEventsField_MenuItemSubSearch; }
			set {
				if (withEventsField_MenuItemSubSearch != null) {
					withEventsField_MenuItemSubSearch.Click -= MenuItemSubSearch_Click;
				}
				withEventsField_MenuItemSubSearch = value;
				if (withEventsField_MenuItemSubSearch != null) {
					withEventsField_MenuItemSubSearch.Click += MenuItemSubSearch_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemSearchNext;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemSearchNext {
			get { return withEventsField_MenuItemSearchNext; }
			set {
				if (withEventsField_MenuItemSearchNext != null) {
					withEventsField_MenuItemSearchNext.Click -= MenuItemSearchNext_Click;
				}
				withEventsField_MenuItemSearchNext = value;
				if (withEventsField_MenuItemSearchNext != null) {
					withEventsField_MenuItemSearchNext.Click += MenuItemSearchNext_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemSearchPrev;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemSearchPrev {
			get { return withEventsField_MenuItemSearchPrev; }
			set {
				if (withEventsField_MenuItemSearchPrev != null) {
					withEventsField_MenuItemSearchPrev.Click -= MenuItemSearchPrev_Click;
				}
				withEventsField_MenuItemSearchPrev = value;
				if (withEventsField_MenuItemSearchPrev != null) {
					withEventsField_MenuItemSearchPrev.Click += MenuItemSearchPrev_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemCommand;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemCommand {
			get { return withEventsField_MenuItemCommand; }
			set {
				if (withEventsField_MenuItemCommand != null) {
					withEventsField_MenuItemCommand.DropDownOpening -= MenuItemCommand_DropDownOpening;
				}
				withEventsField_MenuItemCommand = value;
				if (withEventsField_MenuItemCommand != null) {
					withEventsField_MenuItemCommand.DropDownOpening += MenuItemCommand_DropDownOpening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MenuItemHelp;
		internal System.Windows.Forms.ToolStripMenuItem MenuItemHelp {
			get { return withEventsField_MenuItemHelp; }
			set {
				if (withEventsField_MenuItemHelp != null) {
					withEventsField_MenuItemHelp.DropDownOpening -= MenuItemHelp_DropDownOpening;
				}
				withEventsField_MenuItemHelp = value;
				if (withEventsField_MenuItemHelp != null) {
					withEventsField_MenuItemHelp.DropDownOpening += MenuItemHelp_DropDownOpening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MatomeMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MatomeMenuItem {
			get { return withEventsField_MatomeMenuItem; }
			set {
				if (withEventsField_MatomeMenuItem != null) {
					withEventsField_MatomeMenuItem.Click -= MatomeMenuItem_Click;
				}
				withEventsField_MatomeMenuItem = value;
				if (withEventsField_MatomeMenuItem != null) {
					withEventsField_MatomeMenuItem.Click += MatomeMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator16;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_VerUpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem VerUpMenuItem {
			get { return withEventsField_VerUpMenuItem; }
			set {
				if (withEventsField_VerUpMenuItem != null) {
					withEventsField_VerUpMenuItem.Click -= VerUpMenuItem_Click;
				}
				withEventsField_VerUpMenuItem = value;
				if (withEventsField_VerUpMenuItem != null) {
					withEventsField_VerUpMenuItem.Click += VerUpMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator14;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_AboutMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem AboutMenuItem {
			get { return withEventsField_AboutMenuItem; }
			set {
				if (withEventsField_AboutMenuItem != null) {
					withEventsField_AboutMenuItem.Click -= AboutMenuItem_Click;
				}
				withEventsField_AboutMenuItem = value;
				if (withEventsField_AboutMenuItem != null) {
					withEventsField_AboutMenuItem.Click += AboutMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.SplitContainer withEventsField_SplitContainer2;
		internal System.Windows.Forms.SplitContainer SplitContainer2 {
			get { return withEventsField_SplitContainer2; }
			set {
				if (withEventsField_SplitContainer2 != null) {
					withEventsField_SplitContainer2.SplitterMoved -= SplitContainer2_SplitterMoved;
					withEventsField_SplitContainer2.MouseDoubleClick -= SplitContainer2_MouseDoubleClick;
				}
				withEventsField_SplitContainer2 = value;
				if (withEventsField_SplitContainer2 != null) {
					withEventsField_SplitContainer2.SplitterMoved += SplitContainer2_SplitterMoved;
					withEventsField_SplitContainer2.MouseDoubleClick += SplitContainer2_MouseDoubleClick;
				}
			}
		}
		internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		private TweenCustomControl.PictureBoxEx withEventsField_UserPicture;
		internal TweenCustomControl.PictureBoxEx UserPicture {
			get { return withEventsField_UserPicture; }
			set {
				if (withEventsField_UserPicture != null) {
					withEventsField_UserPicture.MouseEnter -= UserPicture_MouseEnter;
					withEventsField_UserPicture.MouseLeave -= UserPicture_MouseLeave;
					withEventsField_UserPicture.DoubleClick -= UserPicture_DoubleClick;
				}
				withEventsField_UserPicture = value;
				if (withEventsField_UserPicture != null) {
					withEventsField_UserPicture.MouseEnter += UserPicture_MouseEnter;
					withEventsField_UserPicture.MouseLeave += UserPicture_MouseLeave;
					withEventsField_UserPicture.DoubleClick += UserPicture_DoubleClick;
				}
			}
		}
		internal System.Windows.Forms.Label NameLabel;
		internal System.Windows.Forms.Label DateTimeLabel;
		private System.Windows.Forms.TextBox withEventsField_StatusText;
		internal System.Windows.Forms.TextBox StatusText {
			get { return withEventsField_StatusText; }
			set {
				if (withEventsField_StatusText != null) {
					withEventsField_StatusText.KeyPress -= StatusText_KeyPress;
					withEventsField_StatusText.KeyUp -= StatusText_KeyUp;
					withEventsField_StatusText.TextChanged -= StatusText_TextChanged;
					withEventsField_StatusText.Enter -= StatusText_Enter;
					withEventsField_StatusText.Leave -= StatusText_Leave;
					withEventsField_StatusText.KeyDown -= StatusText_KeyDown;
					withEventsField_StatusText.MultilineChanged -= StatusText_MultilineChanged;
				}
				withEventsField_StatusText = value;
				if (withEventsField_StatusText != null) {
					withEventsField_StatusText.KeyPress += StatusText_KeyPress;
					withEventsField_StatusText.KeyUp += StatusText_KeyUp;
					withEventsField_StatusText.TextChanged += StatusText_TextChanged;
					withEventsField_StatusText.Enter += StatusText_Enter;
					withEventsField_StatusText.Leave += StatusText_Leave;
					withEventsField_StatusText.KeyDown += StatusText_KeyDown;
					withEventsField_StatusText.MultilineChanged += StatusText_MultilineChanged;
				}
			}
		}
		internal System.Windows.Forms.Label lblLen;
		private System.Windows.Forms.Button withEventsField_PostButton;
		internal System.Windows.Forms.Button PostButton {
			get { return withEventsField_PostButton; }
			set {
				if (withEventsField_PostButton != null) {
					withEventsField_PostButton.Click -= PostButton_Click;
				}
				withEventsField_PostButton = value;
				if (withEventsField_PostButton != null) {
					withEventsField_PostButton.Click += PostButton_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem TinyUrlConvertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UpdateFollowersMenuItem1;
		internal System.Windows.Forms.ToolStripMenuItem UpdateFollowersMenuItem1 {
			get { return withEventsField_UpdateFollowersMenuItem1; }
			set {
				if (withEventsField_UpdateFollowersMenuItem1 != null) {
					withEventsField_UpdateFollowersMenuItem1.Click -= GetFollowersAllToolStripMenuItem_Click;
				}
				withEventsField_UpdateFollowersMenuItem1 = value;
				if (withEventsField_UpdateFollowersMenuItem1 != null) {
					withEventsField_UpdateFollowersMenuItem1.Click += GetFollowersAllToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TinyURLToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TinyURLToolStripMenuItem {
			get { return withEventsField_TinyURLToolStripMenuItem; }
			set {
				if (withEventsField_TinyURLToolStripMenuItem != null) {
					withEventsField_TinyURLToolStripMenuItem.Click -= TinyURLToolStripMenuItem_Click;
				}
				withEventsField_TinyURLToolStripMenuItem = value;
				if (withEventsField_TinyURLToolStripMenuItem != null) {
					withEventsField_TinyURLToolStripMenuItem.Click += TinyURLToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_IsgdToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem IsgdToolStripMenuItem {
			get { return withEventsField_IsgdToolStripMenuItem; }
			set {
				if (withEventsField_IsgdToolStripMenuItem != null) {
					withEventsField_IsgdToolStripMenuItem.Click -= IsgdToolStripMenuItem_Click;
				}
				withEventsField_IsgdToolStripMenuItem = value;
				if (withEventsField_IsgdToolStripMenuItem != null) {
					withEventsField_IsgdToolStripMenuItem.Click += IsgdToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UrlConvertAutoToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UrlConvertAutoToolStripMenuItem {
			get { return withEventsField_UrlConvertAutoToolStripMenuItem; }
			set {
				if (withEventsField_UrlConvertAutoToolStripMenuItem != null) {
					withEventsField_UrlConvertAutoToolStripMenuItem.Click -= UrlConvertAutoToolStripMenuItem_Click;
				}
				withEventsField_UrlConvertAutoToolStripMenuItem = value;
				if (withEventsField_UrlConvertAutoToolStripMenuItem != null) {
					withEventsField_UrlConvertAutoToolStripMenuItem.Click += UrlConvertAutoToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UrlUndoToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UrlUndoToolStripMenuItem {
			get { return withEventsField_UrlUndoToolStripMenuItem; }
			set {
				if (withEventsField_UrlUndoToolStripMenuItem != null) {
					withEventsField_UrlUndoToolStripMenuItem.Click -= UrlUndoToolStripMenuItem_Click;
				}
				withEventsField_UrlUndoToolStripMenuItem = value;
				if (withEventsField_UrlUndoToolStripMenuItem != null) {
					withEventsField_UrlUndoToolStripMenuItem.Click += UrlUndoToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuPostBrowser;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuPostBrowser {
			get { return withEventsField_ContextMenuPostBrowser; }
			set {
				if (withEventsField_ContextMenuPostBrowser != null) {
					withEventsField_ContextMenuPostBrowser.Opening -= ContextMenuPostBrowser_Opening;
				}
				withEventsField_ContextMenuPostBrowser = value;
				if (withEventsField_ContextMenuPostBrowser != null) {
					withEventsField_ContextMenuPostBrowser.Opening += ContextMenuPostBrowser_Opening;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem SelectionSearchContextMenuItem;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectionCopyContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectionCopyContextMenuItem {
			get { return withEventsField_SelectionCopyContextMenuItem; }
			set {
				if (withEventsField_SelectionCopyContextMenuItem != null) {
					withEventsField_SelectionCopyContextMenuItem.Click -= SelectionCopyContextMenuItem_Click;
				}
				withEventsField_SelectionCopyContextMenuItem = value;
				if (withEventsField_SelectionCopyContextMenuItem != null) {
					withEventsField_SelectionCopyContextMenuItem.Click += SelectionCopyContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectionAllContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectionAllContextMenuItem {
			get { return withEventsField_SelectionAllContextMenuItem; }
			set {
				if (withEventsField_SelectionAllContextMenuItem != null) {
					withEventsField_SelectionAllContextMenuItem.Click -= SelectionAllContextMenuItem_Click;
				}
				withEventsField_SelectionAllContextMenuItem = value;
				if (withEventsField_SelectionAllContextMenuItem != null) {
					withEventsField_SelectionAllContextMenuItem.Click += SelectionAllContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchWikipediaContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchWikipediaContextMenuItem {
			get { return withEventsField_SearchWikipediaContextMenuItem; }
			set {
				if (withEventsField_SearchWikipediaContextMenuItem != null) {
					withEventsField_SearchWikipediaContextMenuItem.Click -= SearchWikipediaContextMenuItem_Click;
				}
				withEventsField_SearchWikipediaContextMenuItem = value;
				if (withEventsField_SearchWikipediaContextMenuItem != null) {
					withEventsField_SearchWikipediaContextMenuItem.Click += SearchWikipediaContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchGoogleContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchGoogleContextMenuItem {
			get { return withEventsField_SearchGoogleContextMenuItem; }
			set {
				if (withEventsField_SearchGoogleContextMenuItem != null) {
					withEventsField_SearchGoogleContextMenuItem.Click -= SearchGoogleContextMenuItem_Click;
				}
				withEventsField_SearchGoogleContextMenuItem = value;
				if (withEventsField_SearchGoogleContextMenuItem != null) {
					withEventsField_SearchGoogleContextMenuItem.Click += SearchGoogleContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchYatsContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchYatsContextMenuItem {
			get { return withEventsField_SearchYatsContextMenuItem; }
			set {
				if (withEventsField_SearchYatsContextMenuItem != null) {
					withEventsField_SearchYatsContextMenuItem.Click -= SearchYatsContextMenuItem_Click;
				}
				withEventsField_SearchYatsContextMenuItem = value;
				if (withEventsField_SearchYatsContextMenuItem != null) {
					withEventsField_SearchYatsContextMenuItem.Click += SearchYatsContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchPublicSearchContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchPublicSearchContextMenuItem {
			get { return withEventsField_SearchPublicSearchContextMenuItem; }
			set {
				if (withEventsField_SearchPublicSearchContextMenuItem != null) {
					withEventsField_SearchPublicSearchContextMenuItem.Click -= SearchPublicSearchContextMenuItem_Click;
				}
				withEventsField_SearchPublicSearchContextMenuItem = value;
				if (withEventsField_SearchPublicSearchContextMenuItem != null) {
					withEventsField_SearchPublicSearchContextMenuItem.Click += SearchPublicSearchContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CurrentTabToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CurrentTabToolStripMenuItem {
			get { return withEventsField_CurrentTabToolStripMenuItem; }
			set {
				if (withEventsField_CurrentTabToolStripMenuItem != null) {
					withEventsField_CurrentTabToolStripMenuItem.Click -= CurrentTabToolStripMenuItem_Click;
				}
				withEventsField_CurrentTabToolStripMenuItem = value;
				if (withEventsField_CurrentTabToolStripMenuItem != null) {
					withEventsField_CurrentTabToolStripMenuItem.Click += CurrentTabToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UrlCopyContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UrlCopyContextMenuItem {
			get { return withEventsField_UrlCopyContextMenuItem; }
			set {
				if (withEventsField_UrlCopyContextMenuItem != null) {
					withEventsField_UrlCopyContextMenuItem.Click -= UrlCopyContextMenuItem_Click;
				}
				withEventsField_UrlCopyContextMenuItem = value;
				if (withEventsField_UrlCopyContextMenuItem != null) {
					withEventsField_UrlCopyContextMenuItem.Click += UrlCopyContextMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MoveToHomeToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MoveToHomeToolStripMenuItem {
			get { return withEventsField_MoveToHomeToolStripMenuItem; }
			set {
				if (withEventsField_MoveToHomeToolStripMenuItem != null) {
					withEventsField_MoveToHomeToolStripMenuItem.Click -= MoveToHomeToolStripMenuItem_Click;
				}
				withEventsField_MoveToHomeToolStripMenuItem = value;
				if (withEventsField_MoveToHomeToolStripMenuItem != null) {
					withEventsField_MoveToHomeToolStripMenuItem.Click += MoveToHomeToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MoveToFavToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MoveToFavToolStripMenuItem {
			get { return withEventsField_MoveToFavToolStripMenuItem; }
			set {
				if (withEventsField_MoveToFavToolStripMenuItem != null) {
					withEventsField_MoveToFavToolStripMenuItem.Click -= MoveToFavToolStripMenuItem_Click;
				}
				withEventsField_MoveToFavToolStripMenuItem = value;
				if (withEventsField_MoveToFavToolStripMenuItem != null) {
					withEventsField_MoveToFavToolStripMenuItem.Click += MoveToFavToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_StatusOpenMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem StatusOpenMenuItem {
			get { return withEventsField_StatusOpenMenuItem; }
			set {
				if (withEventsField_StatusOpenMenuItem != null) {
					withEventsField_StatusOpenMenuItem.Click -= StatusOpenMenuItem_Click;
				}
				withEventsField_StatusOpenMenuItem = value;
				if (withEventsField_StatusOpenMenuItem != null) {
					withEventsField_StatusOpenMenuItem.Click += StatusOpenMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RepliedStatusOpenMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RepliedStatusOpenMenuItem {
			get { return withEventsField_RepliedStatusOpenMenuItem; }
			set {
				if (withEventsField_RepliedStatusOpenMenuItem != null) {
					withEventsField_RepliedStatusOpenMenuItem.Click -= RepliedStatusOpenMenuItem_Click;
				}
				withEventsField_RepliedStatusOpenMenuItem = value;
				if (withEventsField_RepliedStatusOpenMenuItem != null) {
					withEventsField_RepliedStatusOpenMenuItem.Click += RepliedStatusOpenMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavorareMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavorareMenuItem {
			get { return withEventsField_FavorareMenuItem; }
			set {
				if (withEventsField_FavorareMenuItem != null) {
					withEventsField_FavorareMenuItem.Click -= FavorareMenuItem_Click;
				}
				withEventsField_FavorareMenuItem = value;
				if (withEventsField_FavorareMenuItem != null) {
					withEventsField_FavorareMenuItem.Click += FavorareMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenURLMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenURLMenuItem {
			get { return withEventsField_OpenURLMenuItem; }
			set {
				if (withEventsField_OpenURLMenuItem != null) {
					withEventsField_OpenURLMenuItem.Click -= OpenURLMenuItem_Click;
				}
				withEventsField_OpenURLMenuItem = value;
				if (withEventsField_OpenURLMenuItem != null) {
					withEventsField_OpenURLMenuItem.Click += OpenURLMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TabMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TabMenuItem {
			get { return withEventsField_TabMenuItem; }
			set {
				if (withEventsField_TabMenuItem != null) {
					withEventsField_TabMenuItem.Click -= TabMenuItem_Click;
				}
				withEventsField_TabMenuItem = value;
				if (withEventsField_TabMenuItem != null) {
					withEventsField_TabMenuItem.Click += TabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_IDRuleMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem IDRuleMenuItem {
			get { return withEventsField_IDRuleMenuItem; }
			set {
				if (withEventsField_IDRuleMenuItem != null) {
					withEventsField_IDRuleMenuItem.Click -= IDRuleMenuItem_Click;
				}
				withEventsField_IDRuleMenuItem = value;
				if (withEventsField_IDRuleMenuItem != null) {
					withEventsField_IDRuleMenuItem.Click += IDRuleMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReadedStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReadedStripMenuItem {
			get { return withEventsField_ReadedStripMenuItem; }
			set {
				if (withEventsField_ReadedStripMenuItem != null) {
					withEventsField_ReadedStripMenuItem.Click -= ReadedStripMenuItem_Click;
				}
				withEventsField_ReadedStripMenuItem = value;
				if (withEventsField_ReadedStripMenuItem != null) {
					withEventsField_ReadedStripMenuItem.Click += ReadedStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UnreadStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UnreadStripMenuItem {
			get { return withEventsField_UnreadStripMenuItem; }
			set {
				if (withEventsField_UnreadStripMenuItem != null) {
					withEventsField_UnreadStripMenuItem.Click -= UnreadStripMenuItem_Click;
				}
				withEventsField_UnreadStripMenuItem = value;
				if (withEventsField_UnreadStripMenuItem != null) {
					withEventsField_UnreadStripMenuItem.Click += UnreadStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReplyStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReplyStripMenuItem {
			get { return withEventsField_ReplyStripMenuItem; }
			set {
				if (withEventsField_ReplyStripMenuItem != null) {
					withEventsField_ReplyStripMenuItem.Click -= ReplyStripMenuItem_Click;
				}
				withEventsField_ReplyStripMenuItem = value;
				if (withEventsField_ReplyStripMenuItem != null) {
					withEventsField_ReplyStripMenuItem.Click += ReplyStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReplyAllStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReplyAllStripMenuItem {
			get { return withEventsField_ReplyAllStripMenuItem; }
			set {
				if (withEventsField_ReplyAllStripMenuItem != null) {
					withEventsField_ReplyAllStripMenuItem.Click -= ReplyAllStripMenuItem_Click;
				}
				withEventsField_ReplyAllStripMenuItem = value;
				if (withEventsField_ReplyAllStripMenuItem != null) {
					withEventsField_ReplyAllStripMenuItem.Click += ReplyAllStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavAddToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavAddToolStripMenuItem {
			get { return withEventsField_FavAddToolStripMenuItem; }
			set {
				if (withEventsField_FavAddToolStripMenuItem != null) {
					withEventsField_FavAddToolStripMenuItem.Click -= FavAddToolStripMenuItem_Click;
				}
				withEventsField_FavAddToolStripMenuItem = value;
				if (withEventsField_FavAddToolStripMenuItem != null) {
					withEventsField_FavAddToolStripMenuItem.Click += FavAddToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavRemoveToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavRemoveToolStripMenuItem {
			get { return withEventsField_FavRemoveToolStripMenuItem; }
			set {
				if (withEventsField_FavRemoveToolStripMenuItem != null) {
					withEventsField_FavRemoveToolStripMenuItem.Click -= FavRemoveToolStripMenuItem_Click;
				}
				withEventsField_FavRemoveToolStripMenuItem = value;
				if (withEventsField_FavRemoveToolStripMenuItem != null) {
					withEventsField_FavRemoveToolStripMenuItem.Click += FavRemoveToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReTweetStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReTweetStripMenuItem {
			get { return withEventsField_ReTweetStripMenuItem; }
			set {
				if (withEventsField_ReTweetStripMenuItem != null) {
					withEventsField_ReTweetStripMenuItem.Click -= ReTweetStripMenuItem_Click;
				}
				withEventsField_ReTweetStripMenuItem = value;
				if (withEventsField_ReTweetStripMenuItem != null) {
					withEventsField_ReTweetStripMenuItem.Click += ReTweetStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuPostMode;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuPostMode {
			get { return withEventsField_ContextMenuPostMode; }
			set {
				if (withEventsField_ContextMenuPostMode != null) {
					withEventsField_ContextMenuPostMode.Opening -= ContextMenuPostMode_Opening;
				}
				withEventsField_ContextMenuPostMode = value;
				if (withEventsField_ContextMenuPostMode != null) {
					withEventsField_ContextMenuPostMode.Opening += ContextMenuPostMode_Opening;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemUrlMultibyteSplit;
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemApiCommandEvasion;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ToolStripMenuItemUrlAutoShorten;
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemUrlAutoShorten {
			get { return withEventsField_ToolStripMenuItemUrlAutoShorten; }
			set {
				if (withEventsField_ToolStripMenuItemUrlAutoShorten != null) {
					withEventsField_ToolStripMenuItemUrlAutoShorten.CheckedChanged -= ToolStripMenuItemUrlAutoShorten_CheckedChanged;
				}
				withEventsField_ToolStripMenuItemUrlAutoShorten = value;
				if (withEventsField_ToolStripMenuItemUrlAutoShorten != null) {
					withEventsField_ToolStripMenuItemUrlAutoShorten.CheckedChanged += ToolStripMenuItemUrlAutoShorten_CheckedChanged;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem DebugModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DumpPostClassToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DumpPostClassToolStripMenuItem {
			get { return withEventsField_DumpPostClassToolStripMenuItem; }
			set {
				if (withEventsField_DumpPostClassToolStripMenuItem != null) {
					withEventsField_DumpPostClassToolStripMenuItem.Click -= DumpPostClassToolStripMenuItem_Click;
				}
				withEventsField_DumpPostClassToolStripMenuItem = value;
				if (withEventsField_DumpPostClassToolStripMenuItem != null) {
					withEventsField_DumpPostClassToolStripMenuItem.Click += DumpPostClassToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TraceOutToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TraceOutToolStripMenuItem {
			get { return withEventsField_TraceOutToolStripMenuItem; }
			set {
				if (withEventsField_TraceOutToolStripMenuItem != null) {
					withEventsField_TraceOutToolStripMenuItem.Click -= TraceOutToolStripMenuItem_Click;
				}
				withEventsField_TraceOutToolStripMenuItem = value;
				if (withEventsField_TraceOutToolStripMenuItem != null) {
					withEventsField_TraceOutToolStripMenuItem.Click += TraceOutToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TwurlnlToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TwurlnlToolStripMenuItem {
			get { return withEventsField_TwurlnlToolStripMenuItem; }
			set {
				if (withEventsField_TwurlnlToolStripMenuItem != null) {
					withEventsField_TwurlnlToolStripMenuItem.Click -= TwurlnlToolStripMenuItem_Click;
				}
				withEventsField_TwurlnlToolStripMenuItem = value;
				if (withEventsField_TwurlnlToolStripMenuItem != null) {
					withEventsField_TwurlnlToolStripMenuItem.Click += TwurlnlToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TabRenameMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TabRenameMenuItem {
			get { return withEventsField_TabRenameMenuItem; }
			set {
				if (withEventsField_TabRenameMenuItem != null) {
					withEventsField_TabRenameMenuItem.Click -= TabRenameMenuItem_Click;
				}
				withEventsField_TabRenameMenuItem = value;
				if (withEventsField_TabRenameMenuItem != null) {
					withEventsField_TabRenameMenuItem.Click += TabRenameMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_BitlyToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem BitlyToolStripMenuItem {
			get { return withEventsField_BitlyToolStripMenuItem; }
			set {
				if (withEventsField_BitlyToolStripMenuItem != null) {
					withEventsField_BitlyToolStripMenuItem.Click -= BitlyToolStripMenuItem_Click;
				}
				withEventsField_BitlyToolStripMenuItem = value;
				if (withEventsField_BitlyToolStripMenuItem != null) {
					withEventsField_BitlyToolStripMenuItem.Click += BitlyToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ApiInfoMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ApiInfoMenuItem {
			get { return withEventsField_ApiInfoMenuItem; }
			set {
				if (withEventsField_ApiInfoMenuItem != null) {
					withEventsField_ApiInfoMenuItem.Click -= ApiInfoMenuItem_Click;
				}
				withEventsField_ApiInfoMenuItem = value;
				if (withEventsField_ApiInfoMenuItem != null) {
					withEventsField_ApiInfoMenuItem.Click += ApiInfoMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_IdeographicSpaceToSpaceToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem IdeographicSpaceToSpaceToolStripMenuItem {
			get { return withEventsField_IdeographicSpaceToSpaceToolStripMenuItem; }
			set {
				if (withEventsField_IdeographicSpaceToSpaceToolStripMenuItem != null) {
					withEventsField_IdeographicSpaceToSpaceToolStripMenuItem.Click -= IdeographicSpaceToSpaceToolStripMenuItem_Click;
				}
				withEventsField_IdeographicSpaceToSpaceToolStripMenuItem = value;
				if (withEventsField_IdeographicSpaceToSpaceToolStripMenuItem != null) {
					withEventsField_IdeographicSpaceToSpaceToolStripMenuItem.Click += IdeographicSpaceToSpaceToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FollowCommandMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FollowCommandMenuItem {
			get { return withEventsField_FollowCommandMenuItem; }
			set {
				if (withEventsField_FollowCommandMenuItem != null) {
					withEventsField_FollowCommandMenuItem.Click -= FollowCommandMenuItem_Click;
				}
				withEventsField_FollowCommandMenuItem = value;
				if (withEventsField_FollowCommandMenuItem != null) {
					withEventsField_FollowCommandMenuItem.Click += FollowCommandMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RemoveCommandMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RemoveCommandMenuItem {
			get { return withEventsField_RemoveCommandMenuItem; }
			set {
				if (withEventsField_RemoveCommandMenuItem != null) {
					withEventsField_RemoveCommandMenuItem.Click -= RemoveCommandMenuItem_Click;
				}
				withEventsField_RemoveCommandMenuItem = value;
				if (withEventsField_RemoveCommandMenuItem != null) {
					withEventsField_RemoveCommandMenuItem.Click += RemoveCommandMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FriendshipMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FriendshipMenuItem {
			get { return withEventsField_FriendshipMenuItem; }
			set {
				if (withEventsField_FriendshipMenuItem != null) {
					withEventsField_FriendshipMenuItem.Click -= FriendshipMenuItem_Click;
				}
				withEventsField_FriendshipMenuItem = value;
				if (withEventsField_FriendshipMenuItem != null) {
					withEventsField_FriendshipMenuItem.Click += FriendshipMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OwnStatusMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OwnStatusMenuItem {
			get { return withEventsField_OwnStatusMenuItem; }
			set {
				if (withEventsField_OwnStatusMenuItem != null) {
					withEventsField_OwnStatusMenuItem.Click -= OwnStatusMenuItem_Click;
				}
				withEventsField_OwnStatusMenuItem = value;
				if (withEventsField_OwnStatusMenuItem != null) {
					withEventsField_OwnStatusMenuItem.Click += OwnStatusMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReTweetOriginalStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReTweetOriginalStripMenuItem {
			get { return withEventsField_ReTweetOriginalStripMenuItem; }
			set {
				if (withEventsField_ReTweetOriginalStripMenuItem != null) {
					withEventsField_ReTweetOriginalStripMenuItem.Click -= ReTweetOriginalStripMenuItem_Click;
				}
				withEventsField_ReTweetOriginalStripMenuItem = value;
				if (withEventsField_ReTweetOriginalStripMenuItem != null) {
					withEventsField_ReTweetOriginalStripMenuItem.Click += ReTweetOriginalStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FollowContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FollowContextMenuItem {
			get { return withEventsField_FollowContextMenuItem; }
			set {
				if (withEventsField_FollowContextMenuItem != null) {
					withEventsField_FollowContextMenuItem.Click -= FollowContextMenuItem_Click;
				}
				withEventsField_FollowContextMenuItem = value;
				if (withEventsField_FollowContextMenuItem != null) {
					withEventsField_FollowContextMenuItem.Click += FollowContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RemoveContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RemoveContextMenuItem {
			get { return withEventsField_RemoveContextMenuItem; }
			set {
				if (withEventsField_RemoveContextMenuItem != null) {
					withEventsField_RemoveContextMenuItem.Click -= RemoveContextMenuItem_Click;
				}
				withEventsField_RemoveContextMenuItem = value;
				if (withEventsField_RemoveContextMenuItem != null) {
					withEventsField_RemoveContextMenuItem.Click += RemoveContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FriendshipContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FriendshipContextMenuItem {
			get { return withEventsField_FriendshipContextMenuItem; }
			set {
				if (withEventsField_FriendshipContextMenuItem != null) {
					withEventsField_FriendshipContextMenuItem.Click -= FriendshipContextMenuItem_Click;
				}
				withEventsField_FriendshipContextMenuItem = value;
				if (withEventsField_FriendshipContextMenuItem != null) {
					withEventsField_FriendshipContextMenuItem.Click += FriendshipContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_JmpStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem JmpStripMenuItem {
			get { return withEventsField_JmpStripMenuItem; }
			set {
				if (withEventsField_JmpStripMenuItem != null) {
					withEventsField_JmpStripMenuItem.Click -= JmpToolStripMenuItem_Click;
				}
				withEventsField_JmpStripMenuItem = value;
				if (withEventsField_JmpStripMenuItem != null) {
					withEventsField_JmpStripMenuItem.Click += JmpToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_QuoteStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem QuoteStripMenuItem {
			get { return withEventsField_QuoteStripMenuItem; }
			set {
				if (withEventsField_QuoteStripMenuItem != null) {
					withEventsField_QuoteStripMenuItem.Click -= QuoteStripMenuItem_Click;
				}
				withEventsField_QuoteStripMenuItem = value;
				if (withEventsField_QuoteStripMenuItem != null) {
					withEventsField_QuoteStripMenuItem.Click += QuoteStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RefreshMoreStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RefreshMoreStripMenuItem {
			get { return withEventsField_RefreshMoreStripMenuItem; }
			set {
				if (withEventsField_RefreshMoreStripMenuItem != null) {
					withEventsField_RefreshMoreStripMenuItem.Click -= RefreshMoreStripMenuItem_Click;
				}
				withEventsField_RefreshMoreStripMenuItem = value;
				if (withEventsField_RefreshMoreStripMenuItem != null) {
					withEventsField_RefreshMoreStripMenuItem.Click += RefreshMoreStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UndoRemoveTabMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UndoRemoveTabMenuItem {
			get { return withEventsField_UndoRemoveTabMenuItem; }
			set {
				if (withEventsField_UndoRemoveTabMenuItem != null) {
					withEventsField_UndoRemoveTabMenuItem.Click -= UndoRemoveTabMenuItem_Click;
				}
				withEventsField_UndoRemoveTabMenuItem = value;
				if (withEventsField_UndoRemoveTabMenuItem != null) {
					withEventsField_UndoRemoveTabMenuItem.Click += UndoRemoveTabMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MoveToRTHomeMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MoveToRTHomeMenuItem {
			get { return withEventsField_MoveToRTHomeMenuItem; }
			set {
				if (withEventsField_MoveToRTHomeMenuItem != null) {
					withEventsField_MoveToRTHomeMenuItem.Click -= MoveToRTHomeMenuItem_Click;
				}
				withEventsField_MoveToRTHomeMenuItem = value;
				if (withEventsField_MoveToRTHomeMenuItem != null) {
					withEventsField_MoveToRTHomeMenuItem.Click += MoveToRTHomeMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_IdFilterAddMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem IdFilterAddMenuItem {
			get { return withEventsField_IdFilterAddMenuItem; }
			set {
				if (withEventsField_IdFilterAddMenuItem != null) {
					withEventsField_IdFilterAddMenuItem.Click -= IdFilterAddMenuItem_Click;
				}
				withEventsField_IdFilterAddMenuItem = value;
				if (withEventsField_IdFilterAddMenuItem != null) {
					withEventsField_IdFilterAddMenuItem.Click += IdFilterAddMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator22;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_PublicSearchQueryMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem PublicSearchQueryMenuItem {
			get { return withEventsField_PublicSearchQueryMenuItem; }
			set {
				if (withEventsField_PublicSearchQueryMenuItem != null) {
					withEventsField_PublicSearchQueryMenuItem.Click -= PublicSearchQueryMenuItem_Click;
				}
				withEventsField_PublicSearchQueryMenuItem = value;
				if (withEventsField_PublicSearchQueryMenuItem != null) {
					withEventsField_PublicSearchQueryMenuItem.Click += PublicSearchQueryMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UseHashtagMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UseHashtagMenuItem {
			get { return withEventsField_UseHashtagMenuItem; }
			set {
				if (withEventsField_UseHashtagMenuItem != null) {
					withEventsField_UseHashtagMenuItem.Click -= UseHashtagMenuItem_Click;
				}
				withEventsField_UseHashtagMenuItem = value;
				if (withEventsField_UseHashtagMenuItem != null) {
					withEventsField_UseHashtagMenuItem.Click += UseHashtagMenuItem_Click;
				}
			}
		}
		private Tween.TweenCustomControl.ToolStripLabelHistory withEventsField_StatusLabel;
		internal Tween.TweenCustomControl.ToolStripLabelHistory StatusLabel {
			get { return withEventsField_StatusLabel; }
			set {
				if (withEventsField_StatusLabel != null) {
					withEventsField_StatusLabel.DoubleClick -= StatusLabel_DoubleClick;
				}
				withEventsField_StatusLabel = value;
				if (withEventsField_StatusLabel != null) {
					withEventsField_StatusLabel.DoubleClick += StatusLabel_DoubleClick;
				}
			}
		}
		private System.Windows.Forms.ToolStripSplitButton withEventsField_HashStripSplitButton;
		internal System.Windows.Forms.ToolStripSplitButton HashStripSplitButton {
			get { return withEventsField_HashStripSplitButton; }
			set {
				if (withEventsField_HashStripSplitButton != null) {
					withEventsField_HashStripSplitButton.ButtonClick -= HashStripSplitButton_ButtonClick;
				}
				withEventsField_HashStripSplitButton = value;
				if (withEventsField_HashStripSplitButton != null) {
					withEventsField_HashStripSplitButton.ButtonClick += HashStripSplitButton_ButtonClick;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_HashToggleMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem HashToggleMenuItem {
			get { return withEventsField_HashToggleMenuItem; }
			set {
				if (withEventsField_HashToggleMenuItem != null) {
					withEventsField_HashToggleMenuItem.Click -= HashToggleMenuItem_Click;
				}
				withEventsField_HashToggleMenuItem = value;
				if (withEventsField_HashToggleMenuItem != null) {
					withEventsField_HashToggleMenuItem.Click += HashToggleMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_HashManageMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem HashManageMenuItem {
			get { return withEventsField_HashManageMenuItem; }
			set {
				if (withEventsField_HashManageMenuItem != null) {
					withEventsField_HashManageMenuItem.Click -= HashManageMenuItem_Click;
				}
				withEventsField_HashManageMenuItem = value;
				if (withEventsField_HashManageMenuItem != null) {
					withEventsField_HashManageMenuItem.Click += HashManageMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_MultiLineMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MultiLineMenuItem {
			get { return withEventsField_MultiLineMenuItem; }
			set {
				if (withEventsField_MultiLineMenuItem != null) {
					withEventsField_MultiLineMenuItem.Click -= MultiLineMenuItem_Click;
				}
				withEventsField_MultiLineMenuItem = value;
				if (withEventsField_MultiLineMenuItem != null) {
					withEventsField_MultiLineMenuItem.Click += MultiLineMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SettingFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SettingFileMenuItem {
			get { return withEventsField_SettingFileMenuItem; }
			set {
				if (withEventsField_SettingFileMenuItem != null) {
					withEventsField_SettingFileMenuItem.Click -= SettingStripMenuItem_Click;
				}
				withEventsField_SettingFileMenuItem = value;
				if (withEventsField_SettingFileMenuItem != null) {
					withEventsField_SettingFileMenuItem.Click += SettingStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator21;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SaveFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SaveFileMenuItem {
			get { return withEventsField_SaveFileMenuItem; }
			set {
				if (withEventsField_SaveFileMenuItem != null) {
					withEventsField_SaveFileMenuItem.Click -= SaveLogMenuItem_Click;
				}
				withEventsField_SaveFileMenuItem = value;
				if (withEventsField_SaveFileMenuItem != null) {
					withEventsField_SaveFileMenuItem.Click += SaveLogMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator23;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_NotifyFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem NotifyFileMenuItem {
			get { return withEventsField_NotifyFileMenuItem; }
			set {
				if (withEventsField_NotifyFileMenuItem != null) {
					withEventsField_NotifyFileMenuItem.CheckStateChanged -= NewPostPopMenuItem_CheckStateChanged;
				}
				withEventsField_NotifyFileMenuItem = value;
				if (withEventsField_NotifyFileMenuItem != null) {
					withEventsField_NotifyFileMenuItem.CheckStateChanged += NewPostPopMenuItem_CheckStateChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_PlaySoundFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem PlaySoundFileMenuItem {
			get { return withEventsField_PlaySoundFileMenuItem; }
			set {
				if (withEventsField_PlaySoundFileMenuItem != null) {
					withEventsField_PlaySoundFileMenuItem.CheckStateChanged -= PlaySoundMenuItem_CheckedChanged;
				}
				withEventsField_PlaySoundFileMenuItem = value;
				if (withEventsField_PlaySoundFileMenuItem != null) {
					withEventsField_PlaySoundFileMenuItem.CheckStateChanged += PlaySoundMenuItem_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_LockListFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem LockListFileMenuItem {
			get { return withEventsField_LockListFileMenuItem; }
			set {
				if (withEventsField_LockListFileMenuItem != null) {
					withEventsField_LockListFileMenuItem.CheckStateChanged -= ListLockMenuItem_CheckStateChanged;
				}
				withEventsField_LockListFileMenuItem = value;
				if (withEventsField_LockListFileMenuItem != null) {
					withEventsField_LockListFileMenuItem.CheckStateChanged += ListLockMenuItem_CheckStateChanged;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator24;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_EndFileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem EndFileMenuItem {
			get { return withEventsField_EndFileMenuItem; }
			set {
				if (withEventsField_EndFileMenuItem != null) {
					withEventsField_EndFileMenuItem.Click -= EndToolStripMenuItem_Click;
				}
				withEventsField_EndFileMenuItem = value;
				if (withEventsField_EndFileMenuItem != null) {
					withEventsField_EndFileMenuItem.Click += EndToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReplyOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReplyOpMenuItem {
			get { return withEventsField_ReplyOpMenuItem; }
			set {
				if (withEventsField_ReplyOpMenuItem != null) {
					withEventsField_ReplyOpMenuItem.Click -= ReplyStripMenuItem_Click;
				}
				withEventsField_ReplyOpMenuItem = value;
				if (withEventsField_ReplyOpMenuItem != null) {
					withEventsField_ReplyOpMenuItem.Click += ReplyStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReplyAllOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReplyAllOpMenuItem {
			get { return withEventsField_ReplyAllOpMenuItem; }
			set {
				if (withEventsField_ReplyAllOpMenuItem != null) {
					withEventsField_ReplyAllOpMenuItem.Click -= ReplyAllStripMenuItem_Click;
				}
				withEventsField_ReplyAllOpMenuItem = value;
				if (withEventsField_ReplyAllOpMenuItem != null) {
					withEventsField_ReplyAllOpMenuItem.Click += ReplyAllStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DmOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DmOpMenuItem {
			get { return withEventsField_DmOpMenuItem; }
			set {
				if (withEventsField_DmOpMenuItem != null) {
					withEventsField_DmOpMenuItem.Click -= DMStripMenuItem_Click;
				}
				withEventsField_DmOpMenuItem = value;
				if (withEventsField_DmOpMenuItem != null) {
					withEventsField_DmOpMenuItem.Click += DMStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RtOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RtOpMenuItem {
			get { return withEventsField_RtOpMenuItem; }
			set {
				if (withEventsField_RtOpMenuItem != null) {
					withEventsField_RtOpMenuItem.Click -= ReTweetOriginalStripMenuItem_Click;
				}
				withEventsField_RtOpMenuItem = value;
				if (withEventsField_RtOpMenuItem != null) {
					withEventsField_RtOpMenuItem.Click += ReTweetOriginalStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RtUnOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RtUnOpMenuItem {
			get { return withEventsField_RtUnOpMenuItem; }
			set {
				if (withEventsField_RtUnOpMenuItem != null) {
					withEventsField_RtUnOpMenuItem.Click -= ReTweetStripMenuItem_Click;
				}
				withEventsField_RtUnOpMenuItem = value;
				if (withEventsField_RtUnOpMenuItem != null) {
					withEventsField_RtUnOpMenuItem.Click += ReTweetStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_QtOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem QtOpMenuItem {
			get { return withEventsField_QtOpMenuItem; }
			set {
				if (withEventsField_QtOpMenuItem != null) {
					withEventsField_QtOpMenuItem.Click -= QuoteStripMenuItem_Click;
				}
				withEventsField_QtOpMenuItem = value;
				if (withEventsField_QtOpMenuItem != null) {
					withEventsField_QtOpMenuItem.Click += QuoteStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator25;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavOpMenuItem {
			get { return withEventsField_FavOpMenuItem; }
			set {
				if (withEventsField_FavOpMenuItem != null) {
					withEventsField_FavOpMenuItem.Click -= FavAddToolStripMenuItem_Click;
				}
				withEventsField_FavOpMenuItem = value;
				if (withEventsField_FavOpMenuItem != null) {
					withEventsField_FavOpMenuItem.Click += FavAddToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UnFavOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UnFavOpMenuItem {
			get { return withEventsField_UnFavOpMenuItem; }
			set {
				if (withEventsField_UnFavOpMenuItem != null) {
					withEventsField_UnFavOpMenuItem.Click -= FavRemoveToolStripMenuItem_Click;
				}
				withEventsField_UnFavOpMenuItem = value;
				if (withEventsField_UnFavOpMenuItem != null) {
					withEventsField_UnFavOpMenuItem.Click += FavRemoveToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem OpenOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CreateRuleOpMenuItem;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator26;
		internal System.Windows.Forms.ToolStripMenuItem ChangeReadOpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_JumpReadOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem JumpReadOpMenuItem {
			get { return withEventsField_JumpReadOpMenuItem; }
			set {
				if (withEventsField_JumpReadOpMenuItem != null) {
					withEventsField_JumpReadOpMenuItem.Click -= JumpUnreadMenuItem_Click;
				}
				withEventsField_JumpReadOpMenuItem = value;
				if (withEventsField_JumpReadOpMenuItem != null) {
					withEventsField_JumpReadOpMenuItem.Click += JumpUnreadMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator27;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelAllOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelAllOpMenuItem {
			get { return withEventsField_SelAllOpMenuItem; }
			set {
				if (withEventsField_SelAllOpMenuItem != null) {
					withEventsField_SelAllOpMenuItem.Click -= SelectAllMenuItem_Click;
				}
				withEventsField_SelAllOpMenuItem = value;
				if (withEventsField_SelAllOpMenuItem != null) {
					withEventsField_SelAllOpMenuItem.Click += SelectAllMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DelOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DelOpMenuItem {
			get { return withEventsField_DelOpMenuItem; }
			set {
				if (withEventsField_DelOpMenuItem != null) {
					withEventsField_DelOpMenuItem.Click -= DeleteStripMenuItem_Click;
				}
				withEventsField_DelOpMenuItem = value;
				if (withEventsField_DelOpMenuItem != null) {
					withEventsField_DelOpMenuItem.Click += DeleteStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RefreshOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RefreshOpMenuItem {
			get { return withEventsField_RefreshOpMenuItem; }
			set {
				if (withEventsField_RefreshOpMenuItem != null) {
					withEventsField_RefreshOpMenuItem.Click -= RefreshStripMenuItem_Click;
				}
				withEventsField_RefreshOpMenuItem = value;
				if (withEventsField_RefreshOpMenuItem != null) {
					withEventsField_RefreshOpMenuItem.Click += RefreshStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RefreshPrevOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RefreshPrevOpMenuItem {
			get { return withEventsField_RefreshPrevOpMenuItem; }
			set {
				if (withEventsField_RefreshPrevOpMenuItem != null) {
					withEventsField_RefreshPrevOpMenuItem.Click -= RefreshMoreStripMenuItem_Click;
				}
				withEventsField_RefreshPrevOpMenuItem = value;
				if (withEventsField_RefreshPrevOpMenuItem != null) {
					withEventsField_RefreshPrevOpMenuItem.Click += RefreshMoreStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenHomeOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenHomeOpMenuItem {
			get { return withEventsField_OpenHomeOpMenuItem; }
			set {
				if (withEventsField_OpenHomeOpMenuItem != null) {
					withEventsField_OpenHomeOpMenuItem.Click -= MoveToHomeToolStripMenuItem_Click;
				}
				withEventsField_OpenHomeOpMenuItem = value;
				if (withEventsField_OpenHomeOpMenuItem != null) {
					withEventsField_OpenHomeOpMenuItem.Click += MoveToHomeToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenFavOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenFavOpMenuItem {
			get { return withEventsField_OpenFavOpMenuItem; }
			set {
				if (withEventsField_OpenFavOpMenuItem != null) {
					withEventsField_OpenFavOpMenuItem.Click -= MoveToFavToolStripMenuItem_Click;
				}
				withEventsField_OpenFavOpMenuItem = value;
				if (withEventsField_OpenFavOpMenuItem != null) {
					withEventsField_OpenFavOpMenuItem.Click += MoveToFavToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenStatusOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenStatusOpMenuItem {
			get { return withEventsField_OpenStatusOpMenuItem; }
			set {
				if (withEventsField_OpenStatusOpMenuItem != null) {
					withEventsField_OpenStatusOpMenuItem.Click -= StatusOpenMenuItem_Click;
				}
				withEventsField_OpenStatusOpMenuItem = value;
				if (withEventsField_OpenStatusOpMenuItem != null) {
					withEventsField_OpenStatusOpMenuItem.Click += StatusOpenMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenRepSourceOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenRepSourceOpMenuItem {
			get { return withEventsField_OpenRepSourceOpMenuItem; }
			set {
				if (withEventsField_OpenRepSourceOpMenuItem != null) {
					withEventsField_OpenRepSourceOpMenuItem.Click -= RepliedStatusOpenMenuItem_Click;
				}
				withEventsField_OpenRepSourceOpMenuItem = value;
				if (withEventsField_OpenRepSourceOpMenuItem != null) {
					withEventsField_OpenRepSourceOpMenuItem.Click += RepliedStatusOpenMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenFavotterOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenFavotterOpMenuItem {
			get { return withEventsField_OpenFavotterOpMenuItem; }
			set {
				if (withEventsField_OpenFavotterOpMenuItem != null) {
					withEventsField_OpenFavotterOpMenuItem.Click -= FavorareMenuItem_Click;
				}
				withEventsField_OpenFavotterOpMenuItem = value;
				if (withEventsField_OpenFavotterOpMenuItem != null) {
					withEventsField_OpenFavotterOpMenuItem.Click += FavorareMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenUrlOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenUrlOpMenuItem {
			get { return withEventsField_OpenUrlOpMenuItem; }
			set {
				if (withEventsField_OpenUrlOpMenuItem != null) {
					withEventsField_OpenUrlOpMenuItem.Click -= OpenURLMenuItem_Click;
				}
				withEventsField_OpenUrlOpMenuItem = value;
				if (withEventsField_OpenUrlOpMenuItem != null) {
					withEventsField_OpenUrlOpMenuItem.Click += OpenURLMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenRterHomeMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenRterHomeMenuItem {
			get { return withEventsField_OpenRterHomeMenuItem; }
			set {
				if (withEventsField_OpenRterHomeMenuItem != null) {
					withEventsField_OpenRterHomeMenuItem.Click -= MoveToRTHomeMenuItem_Click;
				}
				withEventsField_OpenRterHomeMenuItem = value;
				if (withEventsField_OpenRterHomeMenuItem != null) {
					withEventsField_OpenRterHomeMenuItem.Click += MoveToRTHomeMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CreateTabRuleOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CreateTabRuleOpMenuItem {
			get { return withEventsField_CreateTabRuleOpMenuItem; }
			set {
				if (withEventsField_CreateTabRuleOpMenuItem != null) {
					withEventsField_CreateTabRuleOpMenuItem.Click -= TabMenuItem_Click;
				}
				withEventsField_CreateTabRuleOpMenuItem = value;
				if (withEventsField_CreateTabRuleOpMenuItem != null) {
					withEventsField_CreateTabRuleOpMenuItem.Click += TabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CreateIdRuleOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CreateIdRuleOpMenuItem {
			get { return withEventsField_CreateIdRuleOpMenuItem; }
			set {
				if (withEventsField_CreateIdRuleOpMenuItem != null) {
					withEventsField_CreateIdRuleOpMenuItem.Click -= IDRuleMenuItem_Click;
				}
				withEventsField_CreateIdRuleOpMenuItem = value;
				if (withEventsField_CreateIdRuleOpMenuItem != null) {
					withEventsField_CreateIdRuleOpMenuItem.Click += IDRuleMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ReadOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ReadOpMenuItem {
			get { return withEventsField_ReadOpMenuItem; }
			set {
				if (withEventsField_ReadOpMenuItem != null) {
					withEventsField_ReadOpMenuItem.Click -= ReadedStripMenuItem_Click;
				}
				withEventsField_ReadOpMenuItem = value;
				if (withEventsField_ReadOpMenuItem != null) {
					withEventsField_ReadOpMenuItem.Click += ReadedStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UnreadOpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UnreadOpMenuItem {
			get { return withEventsField_UnreadOpMenuItem; }
			set {
				if (withEventsField_UnreadOpMenuItem != null) {
					withEventsField_UnreadOpMenuItem.Click -= UnreadStripMenuItem_Click;
				}
				withEventsField_UnreadOpMenuItem = value;
				if (withEventsField_UnreadOpMenuItem != null) {
					withEventsField_UnreadOpMenuItem.Click += UnreadStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CreateTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CreateTbMenuItem {
			get { return withEventsField_CreateTbMenuItem; }
			set {
				if (withEventsField_CreateTbMenuItem != null) {
					withEventsField_CreateTbMenuItem.Click -= AddTabMenuItem_Click;
				}
				withEventsField_CreateTbMenuItem = value;
				if (withEventsField_CreateTbMenuItem != null) {
					withEventsField_CreateTbMenuItem.Click += AddTabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RenameTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RenameTbMenuItem {
			get { return withEventsField_RenameTbMenuItem; }
			set {
				if (withEventsField_RenameTbMenuItem != null) {
					withEventsField_RenameTbMenuItem.Click -= TabRenameMenuItem_Click;
				}
				withEventsField_RenameTbMenuItem = value;
				if (withEventsField_RenameTbMenuItem != null) {
					withEventsField_RenameTbMenuItem.Click += TabRenameMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UnreadMngTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UnreadMngTbMenuItem {
			get { return withEventsField_UnreadMngTbMenuItem; }
			set {
				if (withEventsField_UnreadMngTbMenuItem != null) {
					withEventsField_UnreadMngTbMenuItem.Click -= UreadManageMenuItem_Click;
				}
				withEventsField_UnreadMngTbMenuItem = value;
				if (withEventsField_UnreadMngTbMenuItem != null) {
					withEventsField_UnreadMngTbMenuItem.Click += UreadManageMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_NotifyTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem NotifyTbMenuItem {
			get { return withEventsField_NotifyTbMenuItem; }
			set {
				if (withEventsField_NotifyTbMenuItem != null) {
					withEventsField_NotifyTbMenuItem.Click -= NotifyDispMenuItem_Click;
				}
				withEventsField_NotifyTbMenuItem = value;
				if (withEventsField_NotifyTbMenuItem != null) {
					withEventsField_NotifyTbMenuItem.Click += NotifyDispMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_EditRuleTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem EditRuleTbMenuItem {
			get { return withEventsField_EditRuleTbMenuItem; }
			set {
				if (withEventsField_EditRuleTbMenuItem != null) {
					withEventsField_EditRuleTbMenuItem.Click -= FilterEditMenuItem_Click;
				}
				withEventsField_EditRuleTbMenuItem = value;
				if (withEventsField_EditRuleTbMenuItem != null) {
					withEventsField_EditRuleTbMenuItem.Click += FilterEditMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ClearTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ClearTbMenuItem {
			get { return withEventsField_ClearTbMenuItem; }
			set {
				if (withEventsField_ClearTbMenuItem != null) {
					withEventsField_ClearTbMenuItem.Click -= ClearTabMenuItem_Click;
				}
				withEventsField_ClearTbMenuItem = value;
				if (withEventsField_ClearTbMenuItem != null) {
					withEventsField_ClearTbMenuItem.Click += ClearTabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_DeleteTbMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DeleteTbMenuItem {
			get { return withEventsField_DeleteTbMenuItem; }
			set {
				if (withEventsField_DeleteTbMenuItem != null) {
					withEventsField_DeleteTbMenuItem.Click -= DeleteTabMenuItem_Click;
				}
				withEventsField_DeleteTbMenuItem = value;
				if (withEventsField_DeleteTbMenuItem != null) {
					withEventsField_DeleteTbMenuItem.Click += DeleteTabMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripComboBox withEventsField_SoundFileTbComboBox;
		internal System.Windows.Forms.ToolStripComboBox SoundFileTbComboBox {
			get { return withEventsField_SoundFileTbComboBox; }
			set {
				if (withEventsField_SoundFileTbComboBox != null) {
					withEventsField_SoundFileTbComboBox.SelectedIndexChanged -= SoundFileComboBox_SelectedIndexChanged;
				}
				withEventsField_SoundFileTbComboBox = value;
				if (withEventsField_SoundFileTbComboBox != null) {
					withEventsField_SoundFileTbComboBox.SelectedIndexChanged += SoundFileComboBox_SelectedIndexChanged;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator28;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator29;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator30;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator31;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator32;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator33;
		private System.Windows.Forms.WebBrowser withEventsField_PostBrowser;
		internal System.Windows.Forms.WebBrowser PostBrowser {
			get { return withEventsField_PostBrowser; }
			set {
				if (withEventsField_PostBrowser != null) {
					withEventsField_PostBrowser.Navigated -= PostBrowser_Navigated;
					withEventsField_PostBrowser.Navigating -= PostBrowser_Navigating;
					withEventsField_PostBrowser.StatusTextChanged -= PostBrowser_StatusTextChanged;
					withEventsField_PostBrowser.PreviewKeyDown -= PostBrowser_PreviewKeyDown;
				}
				withEventsField_PostBrowser = value;
				if (withEventsField_PostBrowser != null) {
					withEventsField_PostBrowser.Navigated += PostBrowser_Navigated;
					withEventsField_PostBrowser.Navigating += PostBrowser_Navigating;
					withEventsField_PostBrowser.StatusTextChanged += PostBrowser_StatusTextChanged;
					withEventsField_PostBrowser.PreviewKeyDown += PostBrowser_PreviewKeyDown;
				}
			}
		}
		private System.Windows.Forms.SplitContainer withEventsField_SplitContainer3;
		internal System.Windows.Forms.SplitContainer SplitContainer3 {
			get { return withEventsField_SplitContainer3; }
			set {
				if (withEventsField_SplitContainer3 != null) {
					withEventsField_SplitContainer3.SplitterMoved -= SplitContainer3_SplitterMoved;
				}
				withEventsField_SplitContainer3 = value;
				if (withEventsField_SplitContainer3 != null) {
					withEventsField_SplitContainer3.SplitterMoved += SplitContainer3_SplitterMoved;
				}
			}
		}
		internal TweenCustomControl.PictureBoxEx PreviewPicture;
		internal System.Windows.Forms.VScrollBar PreviewScrollBar;
		internal System.Windows.Forms.ToolTip ToolTip1;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FollowToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FollowToolStripMenuItem {
			get { return withEventsField_FollowToolStripMenuItem; }
			set {
				if (withEventsField_FollowToolStripMenuItem != null) {
					withEventsField_FollowToolStripMenuItem.Click -= FollowToolStripMenuItem_Click;
				}
				withEventsField_FollowToolStripMenuItem = value;
				if (withEventsField_FollowToolStripMenuItem != null) {
					withEventsField_FollowToolStripMenuItem.Click += FollowToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UnFollowToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UnFollowToolStripMenuItem {
			get { return withEventsField_UnFollowToolStripMenuItem; }
			set {
				if (withEventsField_UnFollowToolStripMenuItem != null) {
					withEventsField_UnFollowToolStripMenuItem.Click -= UnFollowToolStripMenuItem_Click;
				}
				withEventsField_UnFollowToolStripMenuItem = value;
				if (withEventsField_UnFollowToolStripMenuItem != null) {
					withEventsField_UnFollowToolStripMenuItem.Click += UnFollowToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowFriendShipToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowFriendShipToolStripMenuItem {
			get { return withEventsField_ShowFriendShipToolStripMenuItem; }
			set {
				if (withEventsField_ShowFriendShipToolStripMenuItem != null) {
					withEventsField_ShowFriendShipToolStripMenuItem.Click -= ShowFriendShipToolStripMenuItem_Click;
				}
				withEventsField_ShowFriendShipToolStripMenuItem = value;
				if (withEventsField_ShowFriendShipToolStripMenuItem != null) {
					withEventsField_ShowFriendShipToolStripMenuItem.Click += ShowFriendShipToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowUserStatusToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowUserStatusToolStripMenuItem {
			get { return withEventsField_ShowUserStatusToolStripMenuItem; }
			set {
				if (withEventsField_ShowUserStatusToolStripMenuItem != null) {
					withEventsField_ShowUserStatusToolStripMenuItem.Click -= ShowUserStatusToolStripMenuItem_Click;
				}
				withEventsField_ShowUserStatusToolStripMenuItem = value;
				if (withEventsField_ShowUserStatusToolStripMenuItem != null) {
					withEventsField_ShowUserStatusToolStripMenuItem.Click += ShowUserStatusToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowUserStatusContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowUserStatusContextMenuItem {
			get { return withEventsField_ShowUserStatusContextMenuItem; }
			set {
				if (withEventsField_ShowUserStatusContextMenuItem != null) {
					withEventsField_ShowUserStatusContextMenuItem.Click -= ShowUserStatusContextMenuItem_Click;
				}
				withEventsField_ShowUserStatusContextMenuItem = value;
				if (withEventsField_ShowUserStatusContextMenuItem != null) {
					withEventsField_ShowUserStatusContextMenuItem.Click += ShowUserStatusContextMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowProfileMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowProfileMenuItem {
			get { return withEventsField_ShowProfileMenuItem; }
			set {
				if (withEventsField_ShowProfileMenuItem != null) {
					withEventsField_ShowProfileMenuItem.Click -= ShowProfileMenuItem_Click;
				}
				withEventsField_ShowProfileMenuItem = value;
				if (withEventsField_ShowProfileMenuItem != null) {
					withEventsField_ShowProfileMenuItem.Click += ShowProfileMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowProfMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowProfMenuItem {
			get { return withEventsField_ShowProfMenuItem; }
			set {
				if (withEventsField_ShowProfMenuItem != null) {
					withEventsField_ShowProfMenuItem.Click -= ShowProfileMenuItem_Click;
				}
				withEventsField_ShowProfMenuItem = value;
				if (withEventsField_ShowProfMenuItem != null) {
					withEventsField_ShowProfMenuItem.Click += ShowProfileMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator34;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_HashToggleToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem HashToggleToolStripMenuItem {
			get { return withEventsField_HashToggleToolStripMenuItem; }
			set {
				if (withEventsField_HashToggleToolStripMenuItem != null) {
					withEventsField_HashToggleToolStripMenuItem.Click -= HashToggleMenuItem_Click;
				}
				withEventsField_HashToggleToolStripMenuItem = value;
				if (withEventsField_HashToggleToolStripMenuItem != null) {
					withEventsField_HashToggleToolStripMenuItem.Click += HashToggleMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_HashManageToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem HashManageToolStripMenuItem {
			get { return withEventsField_HashManageToolStripMenuItem; }
			set {
				if (withEventsField_HashManageToolStripMenuItem != null) {
					withEventsField_HashManageToolStripMenuItem.Click -= HashManageMenuItem_Click;
				}
				withEventsField_HashManageToolStripMenuItem = value;
				if (withEventsField_HashManageToolStripMenuItem != null) {
					withEventsField_HashManageToolStripMenuItem.Click += HashManageMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_RtCountMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RtCountMenuItem {
			get { return withEventsField_RtCountMenuItem; }
			set {
				if (withEventsField_RtCountMenuItem != null) {
					withEventsField_RtCountMenuItem.Click -= RtCountMenuItem_Click;
				}
				withEventsField_RtCountMenuItem = value;
				if (withEventsField_RtCountMenuItem != null) {
					withEventsField_RtCountMenuItem.Click += RtCountMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchPostsDetailToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchPostsDetailToolStripMenuItem {
			get { return withEventsField_SearchPostsDetailToolStripMenuItem; }
			set {
				if (withEventsField_SearchPostsDetailToolStripMenuItem != null) {
					withEventsField_SearchPostsDetailToolStripMenuItem.Click -= SearchPostsDetailToolStripMenuItem_Click;
				}
				withEventsField_SearchPostsDetailToolStripMenuItem = value;
				if (withEventsField_SearchPostsDetailToolStripMenuItem != null) {
					withEventsField_SearchPostsDetailToolStripMenuItem.Click += SearchPostsDetailToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchAtPostsDetailNameToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchAtPostsDetailNameToolStripMenuItem {
			get { return withEventsField_SearchAtPostsDetailNameToolStripMenuItem; }
			set {
				if (withEventsField_SearchAtPostsDetailNameToolStripMenuItem != null) {
					withEventsField_SearchAtPostsDetailNameToolStripMenuItem.Click -= SearchAtPostsDetailNameToolStripMenuItem_Click;
				}
				withEventsField_SearchAtPostsDetailNameToolStripMenuItem = value;
				if (withEventsField_SearchAtPostsDetailNameToolStripMenuItem != null) {
					withEventsField_SearchAtPostsDetailNameToolStripMenuItem.Click += SearchAtPostsDetailNameToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.Panel TimelinePanel;
		internal System.Windows.Forms.Panel ProfilePanel;
		internal System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Button withEventsField_FilePickButton;
		internal System.Windows.Forms.Button FilePickButton {
			get { return withEventsField_FilePickButton; }
			set {
				if (withEventsField_FilePickButton != null) {
					withEventsField_FilePickButton.Click -= FilePickButton_Click;
					withEventsField_FilePickButton.KeyDown -= ImageSelection_KeyDown;
					withEventsField_FilePickButton.KeyPress -= ImageSelection_KeyPress;
					withEventsField_FilePickButton.PreviewKeyDown -= ImageSelection_PreviewKeyDown;
				}
				withEventsField_FilePickButton = value;
				if (withEventsField_FilePickButton != null) {
					withEventsField_FilePickButton.Click += FilePickButton_Click;
					withEventsField_FilePickButton.KeyDown += ImageSelection_KeyDown;
					withEventsField_FilePickButton.KeyPress += ImageSelection_KeyPress;
					withEventsField_FilePickButton.PreviewKeyDown += ImageSelection_PreviewKeyDown;
				}
			}
		}
		private System.Windows.Forms.TextBox withEventsField_ImagefilePathText;
		internal System.Windows.Forms.TextBox ImagefilePathText {
			get { return withEventsField_ImagefilePathText; }
			set {
				if (withEventsField_ImagefilePathText != null) {
					withEventsField_ImagefilePathText.Validating -= ImagefilePathText_Validating;
					withEventsField_ImagefilePathText.KeyDown -= ImageSelection_KeyDown;
					withEventsField_ImagefilePathText.KeyPress -= ImageSelection_KeyPress;
					withEventsField_ImagefilePathText.PreviewKeyDown -= ImageSelection_PreviewKeyDown;
				}
				withEventsField_ImagefilePathText = value;
				if (withEventsField_ImagefilePathText != null) {
					withEventsField_ImagefilePathText.Validating += ImagefilePathText_Validating;
					withEventsField_ImagefilePathText.KeyDown += ImageSelection_KeyDown;
					withEventsField_ImagefilePathText.KeyPress += ImageSelection_KeyPress;
					withEventsField_ImagefilePathText.PreviewKeyDown += ImageSelection_PreviewKeyDown;
				}
			}
		}
		internal System.Windows.Forms.Label Label1;
		internal TweenCustomControl.PictureBoxEx ImageSelectedPicture;
		private System.Windows.Forms.Panel withEventsField_ImageSelectionPanel;
		internal System.Windows.Forms.Panel ImageSelectionPanel {
			get { return withEventsField_ImageSelectionPanel; }
			set {
				if (withEventsField_ImageSelectionPanel != null) {
					withEventsField_ImageSelectionPanel.VisibleChanged -= ImageSelectionPanel_VisibleChanged;
				}
				withEventsField_ImageSelectionPanel = value;
				if (withEventsField_ImageSelectionPanel != null) {
					withEventsField_ImageSelectionPanel.VisibleChanged += ImageSelectionPanel_VisibleChanged;
				}
			}
		}
		internal System.Windows.Forms.Panel ImagePathPanel;
		private System.Windows.Forms.ComboBox withEventsField_ImageServiceCombo;
		internal System.Windows.Forms.ComboBox ImageServiceCombo {
			get { return withEventsField_ImageServiceCombo; }
			set {
				if (withEventsField_ImageServiceCombo != null) {
					withEventsField_ImageServiceCombo.KeyDown -= ImageSelection_KeyDown;
					withEventsField_ImageServiceCombo.KeyPress -= ImageSelection_KeyPress;
					withEventsField_ImageServiceCombo.PreviewKeyDown -= ImageSelection_PreviewKeyDown;
					withEventsField_ImageServiceCombo.SelectedIndexChanged -= ImageServiceCombo_SelectedIndexChanged;
				}
				withEventsField_ImageServiceCombo = value;
				if (withEventsField_ImageServiceCombo != null) {
					withEventsField_ImageServiceCombo.KeyDown += ImageSelection_KeyDown;
					withEventsField_ImageServiceCombo.KeyPress += ImageSelection_KeyPress;
					withEventsField_ImageServiceCombo.PreviewKeyDown += ImageSelection_PreviewKeyDown;
					withEventsField_ImageServiceCombo.SelectedIndexChanged += ImageServiceCombo_SelectedIndexChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ImageSelectMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ImageSelectMenuItem {
			get { return withEventsField_ImageSelectMenuItem; }
			set {
				if (withEventsField_ImageSelectMenuItem != null) {
					withEventsField_ImageSelectMenuItem.Click -= ImageSelectMenuItem_Click;
				}
				withEventsField_ImageSelectMenuItem = value;
				if (withEventsField_ImageSelectMenuItem != null) {
					withEventsField_ImageSelectMenuItem.Click += ImageSelectMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator35;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FriendshipAllMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FriendshipAllMenuItem {
			get { return withEventsField_FriendshipAllMenuItem; }
			set {
				if (withEventsField_FriendshipAllMenuItem != null) {
					withEventsField_FriendshipAllMenuItem.Click -= FriendshipAllMenuItem_Click;
				}
				withEventsField_FriendshipAllMenuItem = value;
				if (withEventsField_FriendshipAllMenuItem != null) {
					withEventsField_FriendshipAllMenuItem.Click += FriendshipAllMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator36;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UserStatusToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UserStatusToolStripMenuItem {
			get { return withEventsField_UserStatusToolStripMenuItem; }
			set {
				if (withEventsField_UserStatusToolStripMenuItem != null) {
					withEventsField_UserStatusToolStripMenuItem.Click -= UserStatusToolStripMenuItem_Click;
				}
				withEventsField_UserStatusToolStripMenuItem = value;
				if (withEventsField_UserStatusToolStripMenuItem != null) {
					withEventsField_UserStatusToolStripMenuItem.Click += UserStatusToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ImageCancelButton;
		internal System.Windows.Forms.Button ImageCancelButton {
			get { return withEventsField_ImageCancelButton; }
			set {
				if (withEventsField_ImageCancelButton != null) {
					withEventsField_ImageCancelButton.Click -= ImageCancelButton_Click;
				}
				withEventsField_ImageCancelButton = value;
				if (withEventsField_ImageCancelButton != null) {
					withEventsField_ImageCancelButton.Click += ImageCancelButton_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ToolStripFocusLockMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ToolStripFocusLockMenuItem {
			get { return withEventsField_ToolStripFocusLockMenuItem; }
			set {
				if (withEventsField_ToolStripFocusLockMenuItem != null) {
					withEventsField_ToolStripFocusLockMenuItem.Click -= ToolStripFocusLockMenuItem_CheckedChanged;
				}
				withEventsField_ToolStripFocusLockMenuItem = value;
				if (withEventsField_ToolStripFocusLockMenuItem != null) {
					withEventsField_ToolStripFocusLockMenuItem.Click += ToolStripFocusLockMenuItem_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListManageUserContextToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ListManageUserContextToolStripMenuItem {
			get { return withEventsField_ListManageUserContextToolStripMenuItem; }
			set {
				if (withEventsField_ListManageUserContextToolStripMenuItem != null) {
					withEventsField_ListManageUserContextToolStripMenuItem.Click -= ListManageUserContextToolStripMenuItem_Click;
				}
				withEventsField_ListManageUserContextToolStripMenuItem = value;
				if (withEventsField_ListManageUserContextToolStripMenuItem != null) {
					withEventsField_ListManageUserContextToolStripMenuItem.Click += ListManageUserContextToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator37;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShortcutKeyListMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShortcutKeyListMenuItem {
			get { return withEventsField_ShortcutKeyListMenuItem; }
			set {
				if (withEventsField_ShortcutKeyListMenuItem != null) {
					withEventsField_ShortcutKeyListMenuItem.Click -= ShortcutKeyListMenuItem_Click;
				}
				withEventsField_ShortcutKeyListMenuItem = value;
				if (withEventsField_ShortcutKeyListMenuItem != null) {
					withEventsField_ShortcutKeyListMenuItem.Click += ShortcutKeyListMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListManageMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ListManageMenuItem {
			get { return withEventsField_ListManageMenuItem; }
			set {
				if (withEventsField_ListManageMenuItem != null) {
					withEventsField_ListManageMenuItem.Click -= ListManageUserContextToolStripMenuItem_Click;
				}
				withEventsField_ListManageMenuItem = value;
				if (withEventsField_ListManageMenuItem != null) {
					withEventsField_ListManageMenuItem.Click += ListManageUserContextToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListManageUserContextToolStripMenuItem2;
		internal System.Windows.Forms.ToolStripMenuItem ListManageUserContextToolStripMenuItem2 {
			get { return withEventsField_ListManageUserContextToolStripMenuItem2; }
			set {
				if (withEventsField_ListManageUserContextToolStripMenuItem2 != null) {
					withEventsField_ListManageUserContextToolStripMenuItem2.Click -= ListManageUserContextToolStripMenuItem_Click;
				}
				withEventsField_ListManageUserContextToolStripMenuItem2 = value;
				if (withEventsField_ListManageUserContextToolStripMenuItem2 != null) {
					withEventsField_ListManageUserContextToolStripMenuItem2.Click += ListManageUserContextToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListManageUserContextToolStripMenuItem3;
		internal System.Windows.Forms.ToolStripMenuItem ListManageUserContextToolStripMenuItem3 {
			get { return withEventsField_ListManageUserContextToolStripMenuItem3; }
			set {
				if (withEventsField_ListManageUserContextToolStripMenuItem3 != null) {
					withEventsField_ListManageUserContextToolStripMenuItem3.Click -= ListManageUserContextToolStripMenuItem_Click;
				}
				withEventsField_ListManageUserContextToolStripMenuItem3 = value;
				if (withEventsField_ListManageUserContextToolStripMenuItem3 != null) {
					withEventsField_ListManageUserContextToolStripMenuItem3.Click += ListManageUserContextToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ListManageToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ListManageToolStripMenuItem {
			get { return withEventsField_ListManageToolStripMenuItem; }
			set {
				if (withEventsField_ListManageToolStripMenuItem != null) {
					withEventsField_ListManageToolStripMenuItem.Click -= ListManageToolStripMenuItem_Click;
				}
				withEventsField_ListManageToolStripMenuItem = value;
				if (withEventsField_ListManageToolStripMenuItem != null) {
					withEventsField_ListManageToolStripMenuItem.Click += ListManageToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CacheInfoMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CacheInfoMenuItem {
			get { return withEventsField_CacheInfoMenuItem; }
			set {
				if (withEventsField_CacheInfoMenuItem != null) {
					withEventsField_CacheInfoMenuItem.Click -= CacheInfoMenuItem_Click;
				}
				withEventsField_CacheInfoMenuItem = value;
				if (withEventsField_CacheInfoMenuItem != null) {
					withEventsField_CacheInfoMenuItem.Click += CacheInfoMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.LinkLabel withEventsField_SourceLinkLabel;
		internal System.Windows.Forms.LinkLabel SourceLinkLabel {
			get { return withEventsField_SourceLinkLabel; }
			set {
				if (withEventsField_SourceLinkLabel != null) {
					withEventsField_SourceLinkLabel.LinkClicked -= SourceLinkLabel_LinkClicked;
					withEventsField_SourceLinkLabel.MouseEnter -= SourceLinkLabel_MouseEnter;
					withEventsField_SourceLinkLabel.MouseLeave -= SourceLinkLabel_MouseLeave;
				}
				withEventsField_SourceLinkLabel = value;
				if (withEventsField_SourceLinkLabel != null) {
					withEventsField_SourceLinkLabel.LinkClicked += SourceLinkLabel_LinkClicked;
					withEventsField_SourceLinkLabel.MouseEnter += SourceLinkLabel_MouseEnter;
					withEventsField_SourceLinkLabel.MouseLeave += SourceLinkLabel_MouseLeave;
				}
			}
		}
		internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_CopyUserIdStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CopyUserIdStripMenuItem {
			get { return withEventsField_CopyUserIdStripMenuItem; }
			set {
				if (withEventsField_CopyUserIdStripMenuItem != null) {
					withEventsField_CopyUserIdStripMenuItem.Click -= CopyUserIdStripMenuItem_Click;
				}
				withEventsField_CopyUserIdStripMenuItem = value;
				if (withEventsField_CopyUserIdStripMenuItem != null) {
					withEventsField_CopyUserIdStripMenuItem.Click += CopyUserIdStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavoriteRetweetMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavoriteRetweetMenuItem {
			get { return withEventsField_FavoriteRetweetMenuItem; }
			set {
				if (withEventsField_FavoriteRetweetMenuItem != null) {
					withEventsField_FavoriteRetweetMenuItem.Click -= FavoriteRetweetMenuItem_Click;
				}
				withEventsField_FavoriteRetweetMenuItem = value;
				if (withEventsField_FavoriteRetweetMenuItem != null) {
					withEventsField_FavoriteRetweetMenuItem.Click += FavoriteRetweetMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavoriteRetweetUnofficialMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FavoriteRetweetUnofficialMenuItem {
			get { return withEventsField_FavoriteRetweetUnofficialMenuItem; }
			set {
				if (withEventsField_FavoriteRetweetUnofficialMenuItem != null) {
					withEventsField_FavoriteRetweetUnofficialMenuItem.Click -= FavoriteRetweetUnofficialMenuItem_Click;
				}
				withEventsField_FavoriteRetweetUnofficialMenuItem = value;
				if (withEventsField_FavoriteRetweetUnofficialMenuItem != null) {
					withEventsField_FavoriteRetweetUnofficialMenuItem.Click += FavoriteRetweetUnofficialMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator38;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator39;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavoriteRetweetContextMenu;
		internal System.Windows.Forms.ToolStripMenuItem FavoriteRetweetContextMenu {
			get { return withEventsField_FavoriteRetweetContextMenu; }
			set {
				if (withEventsField_FavoriteRetweetContextMenu != null) {
					withEventsField_FavoriteRetweetContextMenu.Click -= FavoriteRetweetMenuItem_Click;
				}
				withEventsField_FavoriteRetweetContextMenu = value;
				if (withEventsField_FavoriteRetweetContextMenu != null) {
					withEventsField_FavoriteRetweetContextMenu.Click += FavoriteRetweetMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_FavoriteRetweetUnofficialContextMenu;
		internal System.Windows.Forms.ToolStripMenuItem FavoriteRetweetUnofficialContextMenu {
			get { return withEventsField_FavoriteRetweetUnofficialContextMenu; }
			set {
				if (withEventsField_FavoriteRetweetUnofficialContextMenu != null) {
					withEventsField_FavoriteRetweetUnofficialContextMenu.Click -= FavoriteRetweetUnofficialMenuItem_Click;
				}
				withEventsField_FavoriteRetweetUnofficialContextMenu = value;
				if (withEventsField_FavoriteRetweetUnofficialContextMenu != null) {
					withEventsField_FavoriteRetweetUnofficialContextMenu.Click += FavoriteRetweetUnofficialMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowRelatedStatusesMenuItem2;
		internal System.Windows.Forms.ToolStripMenuItem ShowRelatedStatusesMenuItem2 {
			get { return withEventsField_ShowRelatedStatusesMenuItem2; }
			set {
				if (withEventsField_ShowRelatedStatusesMenuItem2 != null) {
					withEventsField_ShowRelatedStatusesMenuItem2.Click -= ShowRelatedStatusesMenuItem_Click;
				}
				withEventsField_ShowRelatedStatusesMenuItem2 = value;
				if (withEventsField_ShowRelatedStatusesMenuItem2 != null) {
					withEventsField_ShowRelatedStatusesMenuItem2.Click += ShowRelatedStatusesMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowRelatedStatusesMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowRelatedStatusesMenuItem {
			get { return withEventsField_ShowRelatedStatusesMenuItem; }
			set {
				if (withEventsField_ShowRelatedStatusesMenuItem != null) {
					withEventsField_ShowRelatedStatusesMenuItem.Click -= ShowRelatedStatusesMenuItem_Click;
				}
				withEventsField_ShowRelatedStatusesMenuItem = value;
				if (withEventsField_ShowRelatedStatusesMenuItem != null) {
					withEventsField_ShowRelatedStatusesMenuItem.Click += ShowRelatedStatusesMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripMenuItem MenuItemUserStream;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_StopToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem StopToolStripMenuItem {
			get { return withEventsField_StopToolStripMenuItem; }
			set {
				if (withEventsField_StopToolStripMenuItem != null) {
					withEventsField_StopToolStripMenuItem.Click -= StopToolStripMenuItem_Click;
				}
				withEventsField_StopToolStripMenuItem = value;
				if (withEventsField_StopToolStripMenuItem != null) {
					withEventsField_StopToolStripMenuItem.Click += StopToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator40;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TrackToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TrackToolStripMenuItem {
			get { return withEventsField_TrackToolStripMenuItem; }
			set {
				if (withEventsField_TrackToolStripMenuItem != null) {
					withEventsField_TrackToolStripMenuItem.Click -= TrackToolStripMenuItem_Click;
				}
				withEventsField_TrackToolStripMenuItem = value;
				if (withEventsField_TrackToolStripMenuItem != null) {
					withEventsField_TrackToolStripMenuItem.Click += TrackToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_AllrepliesToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem AllrepliesToolStripMenuItem {
			get { return withEventsField_AllrepliesToolStripMenuItem; }
			set {
				if (withEventsField_AllrepliesToolStripMenuItem != null) {
					withEventsField_AllrepliesToolStripMenuItem.Click -= AllrepliesToolStripMenuItem_Click;
				}
				withEventsField_AllrepliesToolStripMenuItem = value;
				if (withEventsField_AllrepliesToolStripMenuItem != null) {
					withEventsField_AllrepliesToolStripMenuItem.Click += AllrepliesToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TweenRestartMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TweenRestartMenuItem {
			get { return withEventsField_TweenRestartMenuItem; }
			set {
				if (withEventsField_TweenRestartMenuItem != null) {
					withEventsField_TweenRestartMenuItem.Click -= TweenRestartMenuItem_Click;
				}
				withEventsField_TweenRestartMenuItem = value;
				if (withEventsField_TweenRestartMenuItem != null) {
					withEventsField_TweenRestartMenuItem.Click += TweenRestartMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchPostsDetailNameToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchPostsDetailNameToolStripMenuItem {
			get { return withEventsField_SearchPostsDetailNameToolStripMenuItem; }
			set {
				if (withEventsField_SearchPostsDetailNameToolStripMenuItem != null) {
					withEventsField_SearchPostsDetailNameToolStripMenuItem.Click -= SearchPostsDetailNameToolStripMenuItem_Click;
				}
				withEventsField_SearchPostsDetailNameToolStripMenuItem = value;
				if (withEventsField_SearchPostsDetailNameToolStripMenuItem != null) {
					withEventsField_SearchPostsDetailNameToolStripMenuItem.Click += SearchPostsDetailNameToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SearchAtPostsDetailToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SearchAtPostsDetailToolStripMenuItem {
			get { return withEventsField_SearchAtPostsDetailToolStripMenuItem; }
			set {
				if (withEventsField_SearchAtPostsDetailToolStripMenuItem != null) {
					withEventsField_SearchAtPostsDetailToolStripMenuItem.Click -= SearchAtPostsDetailToolStripMenuItem_Click;
				}
				withEventsField_SearchAtPostsDetailToolStripMenuItem = value;
				if (withEventsField_SearchAtPostsDetailToolStripMenuItem != null) {
					withEventsField_SearchAtPostsDetailToolStripMenuItem.Click += SearchAtPostsDetailToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator41;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenOwnHomeMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenOwnHomeMenuItem {
			get { return withEventsField_OpenOwnHomeMenuItem; }
			set {
				if (withEventsField_OpenOwnHomeMenuItem != null) {
					withEventsField_OpenOwnHomeMenuItem.Click -= OpenOwnHomeMenuItem_Click;
				}
				withEventsField_OpenOwnHomeMenuItem = value;
				if (withEventsField_OpenOwnHomeMenuItem != null) {
					withEventsField_OpenOwnHomeMenuItem.Click += OpenOwnHomeMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenOwnFavedMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenOwnFavedMenuItem {
			get { return withEventsField_OpenOwnFavedMenuItem; }
			set {
				if (withEventsField_OpenOwnFavedMenuItem != null) {
					withEventsField_OpenOwnFavedMenuItem.Click -= OpenOwnFavedMenuItem_Click;
				}
				withEventsField_OpenOwnFavedMenuItem = value;
				if (withEventsField_OpenOwnFavedMenuItem != null) {
					withEventsField_OpenOwnFavedMenuItem.Click += OpenOwnFavedMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator42;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_EventViewerMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem EventViewerMenuItem {
			get { return withEventsField_EventViewerMenuItem; }
			set {
				if (withEventsField_EventViewerMenuItem != null) {
					withEventsField_EventViewerMenuItem.Click -= EventViewerMenuItem_Click;
				}
				withEventsField_EventViewerMenuItem = value;
				if (withEventsField_EventViewerMenuItem != null) {
					withEventsField_EventViewerMenuItem.Click += EventViewerMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_TranslationToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TranslationToolStripMenuItem {
			get { return withEventsField_TranslationToolStripMenuItem; }
			set {
				if (withEventsField_TranslationToolStripMenuItem != null) {
					withEventsField_TranslationToolStripMenuItem.Click -= TranslationToolStripMenuItem_Click;
				}
				withEventsField_TranslationToolStripMenuItem = value;
				if (withEventsField_TranslationToolStripMenuItem != null) {
					withEventsField_TranslationToolStripMenuItem.Click += TranslationToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UxnuMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UxnuMenuItem {
			get { return withEventsField_UxnuMenuItem; }
			set {
				if (withEventsField_UxnuMenuItem != null) {
					withEventsField_UxnuMenuItem.Click -= UxnuMenuItem_Click;
				}
				withEventsField_UxnuMenuItem = value;
				if (withEventsField_UxnuMenuItem != null) {
					withEventsField_UxnuMenuItem.Click += UxnuMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowUserTimelineToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowUserTimelineToolStripMenuItem {
			get { return withEventsField_ShowUserTimelineToolStripMenuItem; }
			set {
				if (withEventsField_ShowUserTimelineToolStripMenuItem != null) {
					withEventsField_ShowUserTimelineToolStripMenuItem.Click -= ShowUserTimelineToolStripMenuItem_Click;
				}
				withEventsField_ShowUserTimelineToolStripMenuItem = value;
				if (withEventsField_ShowUserTimelineToolStripMenuItem != null) {
					withEventsField_ShowUserTimelineToolStripMenuItem.Click += ShowUserTimelineToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ShowUserTimelineContextMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ShowUserTimelineContextMenuItem {
			get { return withEventsField_ShowUserTimelineContextMenuItem; }
			set {
				if (withEventsField_ShowUserTimelineContextMenuItem != null) {
					withEventsField_ShowUserTimelineContextMenuItem.Click -= ShowUserTimelineToolStripMenuItem_Click;
				}
				withEventsField_ShowUserTimelineContextMenuItem = value;
				if (withEventsField_ShowUserTimelineContextMenuItem != null) {
					withEventsField_ShowUserTimelineContextMenuItem.Click += ShowUserTimelineToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UserTimelineToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UserTimelineToolStripMenuItem {
			get { return withEventsField_UserTimelineToolStripMenuItem; }
			set {
				if (withEventsField_UserTimelineToolStripMenuItem != null) {
					withEventsField_UserTimelineToolStripMenuItem.Click -= UserTimelineToolStripMenuItem_Click;
				}
				withEventsField_UserTimelineToolStripMenuItem = value;
				if (withEventsField_UserTimelineToolStripMenuItem != null) {
					withEventsField_UserTimelineToolStripMenuItem.Click += UserTimelineToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_UserFavorareToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UserFavorareToolStripMenuItem {
			get { return withEventsField_UserFavorareToolStripMenuItem; }
			set {
				if (withEventsField_UserFavorareToolStripMenuItem != null) {
					withEventsField_UserFavorareToolStripMenuItem.Click -= UserFavorareToolStripMenuItem_Click;
				}
				withEventsField_UserFavorareToolStripMenuItem = value;
				if (withEventsField_UserFavorareToolStripMenuItem != null) {
					withEventsField_UserFavorareToolStripMenuItem.Click += UserFavorareToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectionTranslationToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectionTranslationToolStripMenuItem {
			get { return withEventsField_SelectionTranslationToolStripMenuItem; }
			set {
				if (withEventsField_SelectionTranslationToolStripMenuItem != null) {
					withEventsField_SelectionTranslationToolStripMenuItem.Click -= SelectionTranslationToolStripMenuItem_Click;
				}
				withEventsField_SelectionTranslationToolStripMenuItem = value;
				if (withEventsField_SelectionTranslationToolStripMenuItem != null) {
					withEventsField_SelectionTranslationToolStripMenuItem.Click += SelectionTranslationToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator43;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_StopRefreshAllMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem StopRefreshAllMenuItem {
			get { return withEventsField_StopRefreshAllMenuItem; }
			set {
				if (withEventsField_StopRefreshAllMenuItem != null) {
					withEventsField_StopRefreshAllMenuItem.CheckedChanged -= StopRefreshAllMenuItem_CheckedChanged;
				}
				withEventsField_StopRefreshAllMenuItem = value;
				if (withEventsField_StopRefreshAllMenuItem != null) {
					withEventsField_StopRefreshAllMenuItem.CheckedChanged += StopRefreshAllMenuItem_CheckedChanged;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenUserSpecifiedUrlMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenUserSpecifiedUrlMenuItem {
			get { return withEventsField_OpenUserSpecifiedUrlMenuItem; }
			set {
				if (withEventsField_OpenUserSpecifiedUrlMenuItem != null) {
					withEventsField_OpenUserSpecifiedUrlMenuItem.Click -= OpenUserSpecifiedUrlMenuItem_Click;
				}
				withEventsField_OpenUserSpecifiedUrlMenuItem = value;
				if (withEventsField_OpenUserSpecifiedUrlMenuItem != null) {
					withEventsField_OpenUserSpecifiedUrlMenuItem.Click += OpenUserSpecifiedUrlMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_OpenUserSpecifiedUrlMenuItem2;
		internal System.Windows.Forms.ToolStripMenuItem OpenUserSpecifiedUrlMenuItem2 {
			get { return withEventsField_OpenUserSpecifiedUrlMenuItem2; }
			set {
				if (withEventsField_OpenUserSpecifiedUrlMenuItem2 != null) {
					withEventsField_OpenUserSpecifiedUrlMenuItem2.Click -= OpenUserSpecifiedUrlMenuItem_Click;
				}
				withEventsField_OpenUserSpecifiedUrlMenuItem2 = value;
				if (withEventsField_OpenUserSpecifiedUrlMenuItem2 != null) {
					withEventsField_OpenUserSpecifiedUrlMenuItem2.Click += OpenUserSpecifiedUrlMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.ImageList PostStateImageList;
		private System.Windows.Forms.SplitContainer withEventsField_SplitContainer4;
		internal System.Windows.Forms.SplitContainer SplitContainer4 {
			get { return withEventsField_SplitContainer4; }
			set {
				if (withEventsField_SplitContainer4 != null) {
					withEventsField_SplitContainer4.SplitterMoved -= SplitContainer4_SplitterMoved;
					withEventsField_SplitContainer4.Resize -= SplitContainer4_Resize;
				}
				withEventsField_SplitContainer4 = value;
				if (withEventsField_SplitContainer4 != null) {
					withEventsField_SplitContainer4.SplitterMoved += SplitContainer4_SplitterMoved;
					withEventsField_SplitContainer4.Resize += SplitContainer4_Resize;
				}
			}
		}
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuSource;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuSource {
			get { return withEventsField_ContextMenuSource; }
			set {
				if (withEventsField_ContextMenuSource != null) {
					withEventsField_ContextMenuSource.Opening -= ContextMenuSource_Opening;
				}
				withEventsField_ContextMenuSource = value;
				if (withEventsField_ContextMenuSource != null) {
					withEventsField_ContextMenuSource.Opening += ContextMenuSource_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SourceCopyMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SourceCopyMenuItem {
			get { return withEventsField_SourceCopyMenuItem; }
			set {
				if (withEventsField_SourceCopyMenuItem != null) {
					withEventsField_SourceCopyMenuItem.Click -= SourceCopyMenuItem_Click;
				}
				withEventsField_SourceCopyMenuItem = value;
				if (withEventsField_SourceCopyMenuItem != null) {
					withEventsField_SourceCopyMenuItem.Click += SourceCopyMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SourceUrlCopyMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SourceUrlCopyMenuItem {
			get { return withEventsField_SourceUrlCopyMenuItem; }
			set {
				if (withEventsField_SourceUrlCopyMenuItem != null) {
					withEventsField_SourceUrlCopyMenuItem.Click -= SourceUrlCopyMenuItem_Click;
				}
				withEventsField_SourceUrlCopyMenuItem = value;
				if (withEventsField_SourceUrlCopyMenuItem != null) {
					withEventsField_SourceUrlCopyMenuItem.Click += SourceUrlCopyMenuItem_Click;
				}
			}

		}
	}
}
