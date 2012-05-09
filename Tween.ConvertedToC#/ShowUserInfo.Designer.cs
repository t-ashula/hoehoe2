using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class ShowUserInfo : System.Windows.Forms.Form
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

		//Windows フォーム デザイナーで必要です。

		private System.ComponentModel.IContainer components;
		//メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
		//Windows フォーム デザイナーを使用して変更できます。  
		//コード エディターを使って変更しないでください。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowUserInfo));
			this.ButtonClose = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.LinkLabelWeb = new System.Windows.Forms.LinkLabel();
			this.LabelLocation = new System.Windows.Forms.Label();
			this.LabelName = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.LinkLabelFollowing = new System.Windows.Forms.LinkLabel();
			this.LinkLabelFollowers = new System.Windows.Forms.LinkLabel();
			this.Label7 = new System.Windows.Forms.Label();
			this.LabelCreatedAt = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.LinkLabelTweet = new System.Windows.Forms.LinkLabel();
			this.Label9 = new System.Windows.Forms.Label();
			this.LinkLabelFav = new System.Windows.Forms.LinkLabel();
			this.ButtonFollow = new System.Windows.Forms.Button();
			this.ButtonUnFollow = new System.Windows.Forms.Button();
			this.LabelIsProtected = new System.Windows.Forms.Label();
			this.LabelIsFollowing = new System.Windows.Forms.Label();
			this.LabelIsFollowed = new System.Windows.Forms.Label();
			this.UserPicture = new System.Windows.Forms.PictureBox();
			this.ContextMenuUserPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ChangeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BackgroundWorkerImageLoader = new System.ComponentModel.BackgroundWorker();
			this.LabelScreenName = new System.Windows.Forms.Label();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
			this.ContextMenuRecentPostBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SelectionCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LabelRecentPost = new System.Windows.Forms.Label();
			this.LabelIsVerified = new System.Windows.Forms.Label();
			this.ButtonSearchPosts = new System.Windows.Forms.Button();
			this.LabelId = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.ButtonEdit = new System.Windows.Forms.Button();
			this.RecentPostBrowser = new System.Windows.Forms.WebBrowser();
			this.DescriptionBrowser = new System.Windows.Forms.WebBrowser();
			this.TextBoxName = new System.Windows.Forms.TextBox();
			this.TextBoxLocation = new System.Windows.Forms.TextBox();
			this.TextBoxWeb = new System.Windows.Forms.TextBox();
			this.TextBoxDescription = new System.Windows.Forms.TextBox();
			this.ButtonBlock = new System.Windows.Forms.Button();
			this.ButtonReportSpam = new System.Windows.Forms.Button();
			this.ButtonBlockDestroy = new System.Windows.Forms.Button();
			this.LinkLabel2 = new System.Windows.Forms.LinkLabel();
			this.OpenFileDialogIcon = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)this.UserPicture).BeginInit();
			this.ContextMenuUserPicture.SuspendLayout();
			this.ContextMenuRecentPostBrowser.SuspendLayout();
			this.SuspendLayout();
			//
			//ButtonClose
			//
			this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.ButtonClose, "ButtonClose");
			this.ButtonClose.Name = "ButtonClose";
			this.ButtonClose.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			resources.ApplyResources(this.Label1, "Label1");
			this.Label1.Name = "Label1";
			this.Label1.UseMnemonic = false;
			//
			//Label2
			//
			resources.ApplyResources(this.Label2, "Label2");
			this.Label2.Name = "Label2";
			//
			//Label3
			//
			resources.ApplyResources(this.Label3, "Label3");
			this.Label3.Name = "Label3";
			//
			//Label4
			//
			resources.ApplyResources(this.Label4, "Label4");
			this.Label4.Name = "Label4";
			//
			//LinkLabelWeb
			//
			this.LinkLabelWeb.AutoEllipsis = true;
			this.LinkLabelWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.LinkLabelWeb, "LinkLabelWeb");
			this.LinkLabelWeb.Name = "LinkLabelWeb";
			this.LinkLabelWeb.TabStop = true;
			this.LinkLabelWeb.UseMnemonic = false;
			//
			//LabelLocation
			//
			this.LabelLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.LabelLocation, "LabelLocation");
			this.LabelLocation.Name = "LabelLocation";
			this.LabelLocation.UseMnemonic = false;
			//
			//LabelName
			//
			this.LabelName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.LabelName, "LabelName");
			this.LabelName.Name = "LabelName";
			this.LabelName.UseMnemonic = false;
			//
			//Label5
			//
			resources.ApplyResources(this.Label5, "Label5");
			this.Label5.Name = "Label5";
			//
			//Label6
			//
			resources.ApplyResources(this.Label6, "Label6");
			this.Label6.Name = "Label6";
			//
			//LinkLabelFollowing
			//
			resources.ApplyResources(this.LinkLabelFollowing, "LinkLabelFollowing");
			this.LinkLabelFollowing.Name = "LinkLabelFollowing";
			this.LinkLabelFollowing.TabStop = true;
			//
			//LinkLabelFollowers
			//
			resources.ApplyResources(this.LinkLabelFollowers, "LinkLabelFollowers");
			this.LinkLabelFollowers.Name = "LinkLabelFollowers";
			this.LinkLabelFollowers.TabStop = true;
			//
			//Label7
			//
			resources.ApplyResources(this.Label7, "Label7");
			this.Label7.Name = "Label7";
			//
			//LabelCreatedAt
			//
			resources.ApplyResources(this.LabelCreatedAt, "LabelCreatedAt");
			this.LabelCreatedAt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LabelCreatedAt.Name = "LabelCreatedAt";
			//
			//Label8
			//
			resources.ApplyResources(this.Label8, "Label8");
			this.Label8.Name = "Label8";
			//
			//LinkLabelTweet
			//
			resources.ApplyResources(this.LinkLabelTweet, "LinkLabelTweet");
			this.LinkLabelTweet.Name = "LinkLabelTweet";
			this.LinkLabelTweet.TabStop = true;
			//
			//Label9
			//
			resources.ApplyResources(this.Label9, "Label9");
			this.Label9.Name = "Label9";
			//
			//LinkLabelFav
			//
			resources.ApplyResources(this.LinkLabelFav, "LinkLabelFav");
			this.LinkLabelFav.Name = "LinkLabelFav";
			this.LinkLabelFav.TabStop = true;
			//
			//ButtonFollow
			//
			resources.ApplyResources(this.ButtonFollow, "ButtonFollow");
			this.ButtonFollow.Name = "ButtonFollow";
			this.ButtonFollow.UseVisualStyleBackColor = true;
			//
			//ButtonUnFollow
			//
			resources.ApplyResources(this.ButtonUnFollow, "ButtonUnFollow");
			this.ButtonUnFollow.Name = "ButtonUnFollow";
			this.ButtonUnFollow.UseVisualStyleBackColor = true;
			//
			//LabelIsProtected
			//
			resources.ApplyResources(this.LabelIsProtected, "LabelIsProtected");
			this.LabelIsProtected.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LabelIsProtected.Name = "LabelIsProtected";
			//
			//LabelIsFollowing
			//
			resources.ApplyResources(this.LabelIsFollowing, "LabelIsFollowing");
			this.LabelIsFollowing.Name = "LabelIsFollowing";
			//
			//LabelIsFollowed
			//
			resources.ApplyResources(this.LabelIsFollowed, "LabelIsFollowed");
			this.LabelIsFollowed.Name = "LabelIsFollowed";
			//
			//UserPicture
			//
			this.UserPicture.BackColor = System.Drawing.Color.White;
			this.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.UserPicture.ContextMenuStrip = this.ContextMenuUserPicture;
			resources.ApplyResources(this.UserPicture, "UserPicture");
			this.UserPicture.Name = "UserPicture";
			this.UserPicture.TabStop = false;
			//
			//ContextMenuUserPicture
			//
			this.ContextMenuUserPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.ChangeIconToolStripMenuItem });
			this.ContextMenuUserPicture.Name = "ContextMenuStrip2";
			resources.ApplyResources(this.ContextMenuUserPicture, "ContextMenuUserPicture");
			//
			//ChangeIconToolStripMenuItem
			//
			this.ChangeIconToolStripMenuItem.Name = "ChangeIconToolStripMenuItem";
			resources.ApplyResources(this.ChangeIconToolStripMenuItem, "ChangeIconToolStripMenuItem");
			//
			//BackgroundWorkerImageLoader
			//
			//
			//LabelScreenName
			//
			this.LabelScreenName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.LabelScreenName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.LabelScreenName, "LabelScreenName");
			this.LabelScreenName.Name = "LabelScreenName";
			//
			//ToolTip1
			//
			this.ToolTip1.ShowAlways = true;
			//
			//LinkLabel1
			//
			resources.ApplyResources(this.LinkLabel1, "LinkLabel1");
			this.LinkLabel1.Name = "LinkLabel1";
			this.LinkLabel1.TabStop = true;
			//
			//ContextMenuRecentPostBrowser
			//
			this.ContextMenuRecentPostBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SelectionCopyToolStripMenuItem,
				this.SelectAllToolStripMenuItem
			});
			this.ContextMenuRecentPostBrowser.Name = "ContextMenuStrip1";
			resources.ApplyResources(this.ContextMenuRecentPostBrowser, "ContextMenuRecentPostBrowser");
			//
			//SelectionCopyToolStripMenuItem
			//
			this.SelectionCopyToolStripMenuItem.Name = "SelectionCopyToolStripMenuItem";
			resources.ApplyResources(this.SelectionCopyToolStripMenuItem, "SelectionCopyToolStripMenuItem");
			//
			//SelectAllToolStripMenuItem
			//
			this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
			resources.ApplyResources(this.SelectAllToolStripMenuItem, "SelectAllToolStripMenuItem");
			//
			//LabelRecentPost
			//
			resources.ApplyResources(this.LabelRecentPost, "LabelRecentPost");
			this.LabelRecentPost.Name = "LabelRecentPost";
			//
			//LabelIsVerified
			//
			resources.ApplyResources(this.LabelIsVerified, "LabelIsVerified");
			this.LabelIsVerified.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LabelIsVerified.Name = "LabelIsVerified";
			//
			//ButtonSearchPosts
			//
			resources.ApplyResources(this.ButtonSearchPosts, "ButtonSearchPosts");
			this.ButtonSearchPosts.Name = "ButtonSearchPosts";
			this.ButtonSearchPosts.UseVisualStyleBackColor = true;
			//
			//LabelId
			//
			resources.ApplyResources(this.LabelId, "LabelId");
			this.LabelId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LabelId.Name = "LabelId";
			//
			//Label12
			//
			resources.ApplyResources(this.Label12, "Label12");
			this.Label12.Name = "Label12";
			//
			//ButtonEdit
			//
			resources.ApplyResources(this.ButtonEdit, "ButtonEdit");
			this.ButtonEdit.Name = "ButtonEdit";
			this.ButtonEdit.UseVisualStyleBackColor = true;
			//
			//RecentPostBrowser
			//
			this.RecentPostBrowser.AllowWebBrowserDrop = false;
			this.RecentPostBrowser.ContextMenuStrip = this.ContextMenuRecentPostBrowser;
			this.RecentPostBrowser.IsWebBrowserContextMenuEnabled = false;
			resources.ApplyResources(this.RecentPostBrowser, "RecentPostBrowser");
			this.RecentPostBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.RecentPostBrowser.Name = "RecentPostBrowser";
			this.RecentPostBrowser.TabStop = false;
			this.RecentPostBrowser.Url = new System.Uri("about:blank", System.UriKind.Absolute);
			this.RecentPostBrowser.WebBrowserShortcutsEnabled = false;
			//
			//DescriptionBrowser
			//
			this.DescriptionBrowser.AllowWebBrowserDrop = false;
			this.DescriptionBrowser.ContextMenuStrip = this.ContextMenuRecentPostBrowser;
			this.DescriptionBrowser.IsWebBrowserContextMenuEnabled = false;
			resources.ApplyResources(this.DescriptionBrowser, "DescriptionBrowser");
			this.DescriptionBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.DescriptionBrowser.Name = "DescriptionBrowser";
			this.DescriptionBrowser.TabStop = false;
			this.DescriptionBrowser.Url = new System.Uri("about:blank", System.UriKind.Absolute);
			this.DescriptionBrowser.WebBrowserShortcutsEnabled = false;
			//
			//TextBoxName
			//
			resources.ApplyResources(this.TextBoxName, "TextBoxName");
			this.TextBoxName.Name = "TextBoxName";
			this.TextBoxName.TabStop = false;
			//
			//TextBoxLocation
			//
			resources.ApplyResources(this.TextBoxLocation, "TextBoxLocation");
			this.TextBoxLocation.Name = "TextBoxLocation";
			this.TextBoxLocation.TabStop = false;
			//
			//TextBoxWeb
			//
			resources.ApplyResources(this.TextBoxWeb, "TextBoxWeb");
			this.TextBoxWeb.Name = "TextBoxWeb";
			this.TextBoxWeb.TabStop = false;
			//
			//TextBoxDescription
			//
			resources.ApplyResources(this.TextBoxDescription, "TextBoxDescription");
			this.TextBoxDescription.Name = "TextBoxDescription";
			this.TextBoxDescription.TabStop = false;
			//
			//ButtonBlock
			//
			resources.ApplyResources(this.ButtonBlock, "ButtonBlock");
			this.ButtonBlock.Name = "ButtonBlock";
			this.ButtonBlock.UseVisualStyleBackColor = true;
			//
			//ButtonReportSpam
			//
			resources.ApplyResources(this.ButtonReportSpam, "ButtonReportSpam");
			this.ButtonReportSpam.Name = "ButtonReportSpam";
			this.ButtonReportSpam.UseVisualStyleBackColor = true;
			//
			//ButtonBlockDestroy
			//
			resources.ApplyResources(this.ButtonBlockDestroy, "ButtonBlockDestroy");
			this.ButtonBlockDestroy.Name = "ButtonBlockDestroy";
			this.ButtonBlockDestroy.UseVisualStyleBackColor = true;
			//
			//LinkLabel2
			//
			resources.ApplyResources(this.LinkLabel2, "LinkLabel2");
			this.LinkLabel2.Name = "LinkLabel2";
			this.LinkLabel2.TabStop = true;
			//
			//OpenFileDialogIcon
			//
			this.OpenFileDialogIcon.FileName = "OpenFileDialog1";
			//
			//ShowUserInfo
			//
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.ButtonClose;
			this.Controls.Add(this.LinkLabel2);
			this.Controls.Add(this.ButtonBlockDestroy);
			this.Controls.Add(this.ButtonReportSpam);
			this.Controls.Add(this.ButtonBlock);
			this.Controls.Add(this.TextBoxDescription);
			this.Controls.Add(this.TextBoxWeb);
			this.Controls.Add(this.ButtonEdit);
			this.Controls.Add(this.LabelId);
			this.Controls.Add(this.TextBoxLocation);
			this.Controls.Add(this.TextBoxName);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.ButtonSearchPosts);
			this.Controls.Add(this.LinkLabel1);
			this.Controls.Add(this.RecentPostBrowser);
			this.Controls.Add(this.UserPicture);
			this.Controls.Add(this.LabelIsVerified);
			this.Controls.Add(this.DescriptionBrowser);
			this.Controls.Add(this.LabelScreenName);
			this.Controls.Add(this.LabelRecentPost);
			this.Controls.Add(this.LinkLabelFav);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.LabelIsProtected);
			this.Controls.Add(this.LabelCreatedAt);
			this.Controls.Add(this.LinkLabelTweet);
			this.Controls.Add(this.LabelIsFollowed);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.LabelIsFollowing);
			this.Controls.Add(this.LinkLabelFollowers);
			this.Controls.Add(this.ButtonUnFollow);
			this.Controls.Add(this.LinkLabelFollowing);
			this.Controls.Add(this.Label6);
			this.Controls.Add(this.LabelName);
			this.Controls.Add(this.ButtonFollow);
			this.Controls.Add(this.Label5);
			this.Controls.Add(this.Label7);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.LabelLocation);
			this.Controls.Add(this.LinkLabelWeb);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.ButtonClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShowUserInfo";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)this.UserPicture).EndInit();
			this.ContextMenuUserPicture.ResumeLayout(false);
			this.ContextMenuRecentPostBrowser.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Button withEventsField_ButtonClose;
		internal System.Windows.Forms.Button ButtonClose {
			get { return withEventsField_ButtonClose; }
			set {
				if (withEventsField_ButtonClose != null) {
					withEventsField_ButtonClose.Click -= ButtonClose_Click;
				}
				withEventsField_ButtonClose = value;
				if (withEventsField_ButtonClose != null) {
					withEventsField_ButtonClose.Click += ButtonClose_Click;
				}
			}
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label4;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabelWeb;
		internal System.Windows.Forms.LinkLabel LinkLabelWeb {
			get { return withEventsField_LinkLabelWeb; }
			set {
				if (withEventsField_LinkLabelWeb != null) {
					withEventsField_LinkLabelWeb.LinkClicked -= LinkLabelWeb_LinkClicked;
				}
				withEventsField_LinkLabelWeb = value;
				if (withEventsField_LinkLabelWeb != null) {
					withEventsField_LinkLabelWeb.LinkClicked += LinkLabelWeb_LinkClicked;
				}
			}
		}
		internal System.Windows.Forms.Label LabelLocation;
		internal System.Windows.Forms.Label LabelName;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Label Label6;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabelFollowing;
		internal System.Windows.Forms.LinkLabel LinkLabelFollowing {
			get { return withEventsField_LinkLabelFollowing; }
			set {
				if (withEventsField_LinkLabelFollowing != null) {
					withEventsField_LinkLabelFollowing.LinkClicked -= LinkLabelFollowing_LinkClicked;
				}
				withEventsField_LinkLabelFollowing = value;
				if (withEventsField_LinkLabelFollowing != null) {
					withEventsField_LinkLabelFollowing.LinkClicked += LinkLabelFollowing_LinkClicked;
				}
			}
		}
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabelFollowers;
		internal System.Windows.Forms.LinkLabel LinkLabelFollowers {
			get { return withEventsField_LinkLabelFollowers; }
			set {
				if (withEventsField_LinkLabelFollowers != null) {
					withEventsField_LinkLabelFollowers.LinkClicked -= LinkLabelFollowers_LinkClicked;
				}
				withEventsField_LinkLabelFollowers = value;
				if (withEventsField_LinkLabelFollowers != null) {
					withEventsField_LinkLabelFollowers.LinkClicked += LinkLabelFollowers_LinkClicked;
				}
			}
		}
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.Label LabelCreatedAt;
		internal System.Windows.Forms.Label Label8;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabelTweet;
		internal System.Windows.Forms.LinkLabel LinkLabelTweet {
			get { return withEventsField_LinkLabelTweet; }
			set {
				if (withEventsField_LinkLabelTweet != null) {
					withEventsField_LinkLabelTweet.LinkClicked -= LinkLabelTweet_LinkClicked;
				}
				withEventsField_LinkLabelTweet = value;
				if (withEventsField_LinkLabelTweet != null) {
					withEventsField_LinkLabelTweet.LinkClicked += LinkLabelTweet_LinkClicked;
				}
			}
		}
		internal System.Windows.Forms.Label Label9;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabelFav;
		internal System.Windows.Forms.LinkLabel LinkLabelFav {
			get { return withEventsField_LinkLabelFav; }
			set {
				if (withEventsField_LinkLabelFav != null) {
					withEventsField_LinkLabelFav.LinkClicked -= LinkLabelFav_LinkClicked;
				}
				withEventsField_LinkLabelFav = value;
				if (withEventsField_LinkLabelFav != null) {
					withEventsField_LinkLabelFav.LinkClicked += LinkLabelFav_LinkClicked;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonFollow;
		internal System.Windows.Forms.Button ButtonFollow {
			get { return withEventsField_ButtonFollow; }
			set {
				if (withEventsField_ButtonFollow != null) {
					withEventsField_ButtonFollow.Click -= ButtonFollow_Click;
				}
				withEventsField_ButtonFollow = value;
				if (withEventsField_ButtonFollow != null) {
					withEventsField_ButtonFollow.Click += ButtonFollow_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonUnFollow;
		internal System.Windows.Forms.Button ButtonUnFollow {
			get { return withEventsField_ButtonUnFollow; }
			set {
				if (withEventsField_ButtonUnFollow != null) {
					withEventsField_ButtonUnFollow.Click -= ButtonUnFollow_Click;
				}
				withEventsField_ButtonUnFollow = value;
				if (withEventsField_ButtonUnFollow != null) {
					withEventsField_ButtonUnFollow.Click += ButtonUnFollow_Click;
				}
			}
		}
		internal System.Windows.Forms.Label LabelIsProtected;
		internal System.Windows.Forms.Label LabelIsFollowing;
		internal System.Windows.Forms.Label LabelIsFollowed;
		private System.Windows.Forms.PictureBox withEventsField_UserPicture;
		internal System.Windows.Forms.PictureBox UserPicture {
			get { return withEventsField_UserPicture; }
			set {
				if (withEventsField_UserPicture != null) {
					withEventsField_UserPicture.DoubleClick -= UserPicture_DoubleClick;
					withEventsField_UserPicture.MouseEnter -= UserPicture_MouseEnter;
					withEventsField_UserPicture.MouseLeave -= UserPicture_MouseLeave;
				}
				withEventsField_UserPicture = value;
				if (withEventsField_UserPicture != null) {
					withEventsField_UserPicture.DoubleClick += UserPicture_DoubleClick;
					withEventsField_UserPicture.MouseEnter += UserPicture_MouseEnter;
					withEventsField_UserPicture.MouseLeave += UserPicture_MouseLeave;
				}
			}
		}
		private System.ComponentModel.BackgroundWorker withEventsField_BackgroundWorkerImageLoader;
		internal System.ComponentModel.BackgroundWorker BackgroundWorkerImageLoader {
			get { return withEventsField_BackgroundWorkerImageLoader; }
			set {
				if (withEventsField_BackgroundWorkerImageLoader != null) {
					withEventsField_BackgroundWorkerImageLoader.DoWork -= BackgroundWorkerImageLoader_DoWork;
					withEventsField_BackgroundWorkerImageLoader.RunWorkerCompleted -= BackgroundWorkerImageLoader_RunWorkerCompleted;
				}
				withEventsField_BackgroundWorkerImageLoader = value;
				if (withEventsField_BackgroundWorkerImageLoader != null) {
					withEventsField_BackgroundWorkerImageLoader.DoWork += BackgroundWorkerImageLoader_DoWork;
					withEventsField_BackgroundWorkerImageLoader.RunWorkerCompleted += BackgroundWorkerImageLoader_RunWorkerCompleted;
				}
			}
		}
		internal System.Windows.Forms.Label LabelScreenName;
		private System.Windows.Forms.WebBrowser withEventsField_DescriptionBrowser;
		internal System.Windows.Forms.WebBrowser DescriptionBrowser {
			get { return withEventsField_DescriptionBrowser; }
			set {
				if (withEventsField_DescriptionBrowser != null) {
					withEventsField_DescriptionBrowser.Navigating -= WebBrowser_Navigating;
					withEventsField_DescriptionBrowser.StatusTextChanged -= WebBrowser_StatusTextChanged;
				}
				withEventsField_DescriptionBrowser = value;
				if (withEventsField_DescriptionBrowser != null) {
					withEventsField_DescriptionBrowser.Navigating += WebBrowser_Navigating;
					withEventsField_DescriptionBrowser.StatusTextChanged += WebBrowser_StatusTextChanged;
				}
			}
		}
		internal System.Windows.Forms.ToolTip ToolTip1;
		private System.Windows.Forms.ContextMenuStrip withEventsField_ContextMenuRecentPostBrowser;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuRecentPostBrowser {
			get { return withEventsField_ContextMenuRecentPostBrowser; }
			set {
				if (withEventsField_ContextMenuRecentPostBrowser != null) {
					withEventsField_ContextMenuRecentPostBrowser.Opening -= ContextMenuStrip1_Opening;
				}
				withEventsField_ContextMenuRecentPostBrowser = value;
				if (withEventsField_ContextMenuRecentPostBrowser != null) {
					withEventsField_ContextMenuRecentPostBrowser.Opening += ContextMenuStrip1_Opening;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectionCopyToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectionCopyToolStripMenuItem {
			get { return withEventsField_SelectionCopyToolStripMenuItem; }
			set {
				if (withEventsField_SelectionCopyToolStripMenuItem != null) {
					withEventsField_SelectionCopyToolStripMenuItem.Click -= SelectionCopyToolStripMenuItem_Click;
				}
				withEventsField_SelectionCopyToolStripMenuItem = value;
				if (withEventsField_SelectionCopyToolStripMenuItem != null) {
					withEventsField_SelectionCopyToolStripMenuItem.Click += SelectionCopyToolStripMenuItem_Click;
				}
			}
		}
		private System.Windows.Forms.ToolStripMenuItem withEventsField_SelectAllToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem {
			get { return withEventsField_SelectAllToolStripMenuItem; }
			set {
				if (withEventsField_SelectAllToolStripMenuItem != null) {
					withEventsField_SelectAllToolStripMenuItem.Click -= SelectAllToolStripMenuItem_Click;
				}
				withEventsField_SelectAllToolStripMenuItem = value;
				if (withEventsField_SelectAllToolStripMenuItem != null) {
					withEventsField_SelectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.Label LabelRecentPost;
		private System.Windows.Forms.WebBrowser withEventsField_RecentPostBrowser;
		internal System.Windows.Forms.WebBrowser RecentPostBrowser {
			get { return withEventsField_RecentPostBrowser; }
			set {
				if (withEventsField_RecentPostBrowser != null) {
					withEventsField_RecentPostBrowser.Navigating -= WebBrowser_Navigating;
					withEventsField_RecentPostBrowser.StatusTextChanged -= WebBrowser_StatusTextChanged;
				}
				withEventsField_RecentPostBrowser = value;
				if (withEventsField_RecentPostBrowser != null) {
					withEventsField_RecentPostBrowser.Navigating += WebBrowser_Navigating;
					withEventsField_RecentPostBrowser.StatusTextChanged += WebBrowser_StatusTextChanged;
				}
			}
		}
		internal System.Windows.Forms.Label LabelIsVerified;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabel1;
		internal System.Windows.Forms.LinkLabel LinkLabel1 {
			get { return withEventsField_LinkLabel1; }
			set {
				if (withEventsField_LinkLabel1 != null) {
					withEventsField_LinkLabel1.LinkClicked -= LinkLabel1_LinkClicked;
				}
				withEventsField_LinkLabel1 = value;
				if (withEventsField_LinkLabel1 != null) {
					withEventsField_LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonSearchPosts;
		internal System.Windows.Forms.Button ButtonSearchPosts {
			get { return withEventsField_ButtonSearchPosts; }
			set {
				if (withEventsField_ButtonSearchPosts != null) {
					withEventsField_ButtonSearchPosts.Click -= ButtonSearchPosts_Click;
				}
				withEventsField_ButtonSearchPosts = value;
				if (withEventsField_ButtonSearchPosts != null) {
					withEventsField_ButtonSearchPosts.Click += ButtonSearchPosts_Click;
				}
			}
		}
		internal System.Windows.Forms.Label LabelId;
		internal System.Windows.Forms.Label Label12;
		private System.Windows.Forms.Button withEventsField_ButtonEdit;
		internal System.Windows.Forms.Button ButtonEdit {
			get { return withEventsField_ButtonEdit; }
			set {
				if (withEventsField_ButtonEdit != null) {
					withEventsField_ButtonEdit.Click -= ButtonEdit_Click;
				}
				withEventsField_ButtonEdit = value;
				if (withEventsField_ButtonEdit != null) {
					withEventsField_ButtonEdit.Click += ButtonEdit_Click;
				}
			}
		}
		internal System.Windows.Forms.ContextMenuStrip ContextMenuUserPicture;
		private System.Windows.Forms.ToolStripMenuItem withEventsField_ChangeIconToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ChangeIconToolStripMenuItem {
			get { return withEventsField_ChangeIconToolStripMenuItem; }
			set {
				if (withEventsField_ChangeIconToolStripMenuItem != null) {
					withEventsField_ChangeIconToolStripMenuItem.Click -= ChangeIconToolStripMenuItem_Click;
				}
				withEventsField_ChangeIconToolStripMenuItem = value;
				if (withEventsField_ChangeIconToolStripMenuItem != null) {
					withEventsField_ChangeIconToolStripMenuItem.Click += ChangeIconToolStripMenuItem_Click;
				}
			}
		}
		internal System.Windows.Forms.OpenFileDialog OpenFileDialogIcon;
		internal System.Windows.Forms.TextBox TextBoxName;
		internal System.Windows.Forms.TextBox TextBoxLocation;
		internal System.Windows.Forms.TextBox TextBoxWeb;
		internal System.Windows.Forms.TextBox TextBoxDescription;
		private System.Windows.Forms.Button withEventsField_ButtonBlock;
		internal System.Windows.Forms.Button ButtonBlock {
			get { return withEventsField_ButtonBlock; }
			set {
				if (withEventsField_ButtonBlock != null) {
					withEventsField_ButtonBlock.Click -= ButtonBlock_Click;
				}
				withEventsField_ButtonBlock = value;
				if (withEventsField_ButtonBlock != null) {
					withEventsField_ButtonBlock.Click += ButtonBlock_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonReportSpam;
		internal System.Windows.Forms.Button ButtonReportSpam {
			get { return withEventsField_ButtonReportSpam; }
			set {
				if (withEventsField_ButtonReportSpam != null) {
					withEventsField_ButtonReportSpam.Click -= ButtonReportSpam_Click;
				}
				withEventsField_ButtonReportSpam = value;
				if (withEventsField_ButtonReportSpam != null) {
					withEventsField_ButtonReportSpam.Click += ButtonReportSpam_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_ButtonBlockDestroy;
		internal System.Windows.Forms.Button ButtonBlockDestroy {
			get { return withEventsField_ButtonBlockDestroy; }
			set {
				if (withEventsField_ButtonBlockDestroy != null) {
					withEventsField_ButtonBlockDestroy.Click -= ButtonBlockDestroy_Click;
				}
				withEventsField_ButtonBlockDestroy = value;
				if (withEventsField_ButtonBlockDestroy != null) {
					withEventsField_ButtonBlockDestroy.Click += ButtonBlockDestroy_Click;
				}
			}
		}
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabel2;
		internal System.Windows.Forms.LinkLabel LinkLabel2 {
			get { return withEventsField_LinkLabel2; }
			set {
				if (withEventsField_LinkLabel2 != null) {
					withEventsField_LinkLabel2.LinkClicked -= LinkLabel2_LinkClicked;
				}
				withEventsField_LinkLabel2 = value;
				if (withEventsField_LinkLabel2 != null) {
					withEventsField_LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
				}
			}
		}
	}
}
