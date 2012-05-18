using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Hoehoe
{
    partial class ShowUserInfo : System.Windows.Forms.Form
    {

        //フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Windows フォーム デザイナーで必要です。

        private System.ComponentModel.IContainer components;
        //メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
        //Windows フォーム デザイナーを使用して変更できます。  
        //コード エディターを使って変更しないでください。
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
            this.ButtonClose.Click += new EventHandler(this.ButtonClose_Click);
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
            this.LinkLabelWeb.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabelWeb_LinkClicked);
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
            this.LinkLabelFollowing.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabelFollowing_LinkClicked);
            //
            //LinkLabelFollowers
            //
            resources.ApplyResources(this.LinkLabelFollowers, "LinkLabelFollowers");
            this.LinkLabelFollowers.Name = "LinkLabelFollowers";
            this.LinkLabelFollowers.TabStop = true;
            this.LinkLabelFollowers.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkLabelFollowers_LinkClicked);
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
            this.LinkLabelTweet.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabelTweet_LinkClicked);
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
            this.LinkLabelFav.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabelFav_LinkClicked);
            //
            //ButtonFollow
            //
            resources.ApplyResources(this.ButtonFollow, "ButtonFollow");
            this.ButtonFollow.Name = "ButtonFollow";
            this.ButtonFollow.UseVisualStyleBackColor = true;
            this.ButtonFollow.Click += new EventHandler(this.ButtonFollow_Click);
            //
            //ButtonUnFollow
            //
            resources.ApplyResources(this.ButtonUnFollow, "ButtonUnFollow");
            this.ButtonUnFollow.Name = "ButtonUnFollow";
            this.ButtonUnFollow.UseVisualStyleBackColor = true;
            ButtonUnFollow.Click += new EventHandler(this.ButtonUnFollow_Click);
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
            this.UserPicture.DoubleClick += new EventHandler(this.UserPicture_DoubleClick);
            this.UserPicture.MouseEnter += new EventHandler(this.UserPicture_MouseEnter);
            this.UserPicture.MouseLeave += new EventHandler(this.UserPicture_MouseLeave);
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
            this.ChangeIconToolStripMenuItem.Click += new EventHandler(this.ChangeIconToolStripMenuItem_Click);
            //
            //BackgroundWorkerImageLoader
            //
            this.BackgroundWorkerImageLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerImageLoader_DoWork);
            this.BackgroundWorkerImageLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerImageLoader_RunWorkerCompleted);
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
            this.LinkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
            //
            //ContextMenuRecentPostBrowser
            //
            this.ContextMenuRecentPostBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.SelectionCopyToolStripMenuItem,
				this.SelectAllToolStripMenuItem
			});
            this.ContextMenuRecentPostBrowser.Name = "ContextMenuStrip1";
            resources.ApplyResources(this.ContextMenuRecentPostBrowser, "ContextMenuRecentPostBrowser");
            this.ContextMenuRecentPostBrowser.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            //
            //SelectionCopyToolStripMenuItem
            //
            this.SelectionCopyToolStripMenuItem.Name = "SelectionCopyToolStripMenuItem";
            resources.ApplyResources(this.SelectionCopyToolStripMenuItem, "SelectionCopyToolStripMenuItem");
            this.SelectionCopyToolStripMenuItem.Click += new EventHandler(this.SelectionCopyToolStripMenuItem_Click);
            //
            //SelectAllToolStripMenuItem
            //
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            resources.ApplyResources(this.SelectAllToolStripMenuItem, "SelectAllToolStripMenuItem");
            this.SelectAllToolStripMenuItem.Click += new EventHandler(this.SelectAllToolStripMenuItem_Click);
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
            this.ButtonSearchPosts.Click += new EventHandler(this.ButtonSearchPosts_Click);
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
            this.ButtonEdit.Click += new EventHandler(this.ButtonEdit_Click);
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
            this.RecentPostBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
            this.RecentPostBrowser.StatusTextChanged += new EventHandler(this.WebBrowser_StatusTextChanged);
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
            this.DescriptionBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
            this.DescriptionBrowser.StatusTextChanged += new EventHandler(this.WebBrowser_StatusTextChanged);
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
            this.ButtonBlock.Click += new EventHandler(this.ButtonBlock_Click);
            //
            //ButtonReportSpam
            //
            resources.ApplyResources(this.ButtonReportSpam, "ButtonReportSpam");
            this.ButtonReportSpam.Name = "ButtonReportSpam";
            this.ButtonReportSpam.UseVisualStyleBackColor = true;
            this.ButtonReportSpam.Click += new EventHandler(this.ButtonReportSpam_Click);
            //
            //ButtonBlockDestroy
            //
            resources.ApplyResources(this.ButtonBlockDestroy, "ButtonBlockDestroy");
            this.ButtonBlockDestroy.Name = "ButtonBlockDestroy";
            this.ButtonBlockDestroy.UseVisualStyleBackColor = true;
            this.ButtonBlockDestroy.Click += new EventHandler(this.ButtonBlockDestroy_Click);
            //
            //LinkLabel2
            //
            resources.ApplyResources(this.LinkLabel2, "LinkLabel2");
            this.LinkLabel2.Name = "LinkLabel2";
            this.LinkLabel2.TabStop = true;
            this.LinkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
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
            this.DragDrop += new DragEventHandler(this.ShowUserInfo_DragDrop);
            this.DragOver += new DragEventHandler(this.ShowUserInfo_DragOver);
            this.MouseEnter += new EventHandler(this.ShowUserInfo_MouseEnter);
            this.Shown += new EventHandler(this.ShowUserInfo_Shown);
            this.FormClosing += new FormClosingEventHandler(this.ShowUserInfo_FormClosing);
            this.Activated += new EventHandler(this.ShowUserInfo_Activated);
            this.Load += new EventHandler(this.ShowUserInfo_Load);
            this.FormClosed += new FormClosedEventHandler(this.ShowUserInfo_FormClosed);
            ((System.ComponentModel.ISupportInitialize)this.UserPicture).EndInit();
            this.ContextMenuUserPicture.ResumeLayout(false);
            this.ContextMenuRecentPostBrowser.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal System.Windows.Forms.Button ButtonClose;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.LinkLabel LinkLabelWeb;
        internal System.Windows.Forms.Label LabelLocation;
        internal System.Windows.Forms.Label LabelName;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.LinkLabel LinkLabelFollowing;
        internal System.Windows.Forms.LinkLabel LinkLabelFollowers;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label LabelCreatedAt;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.LinkLabel LinkLabelTweet;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.LinkLabel LinkLabelFav;
        internal System.Windows.Forms.Button ButtonFollow;
        internal System.Windows.Forms.Button ButtonUnFollow;
        internal System.Windows.Forms.Label LabelIsProtected;
        internal System.Windows.Forms.Label LabelIsFollowing;
        internal System.Windows.Forms.Label LabelIsFollowed;
        internal System.Windows.Forms.PictureBox UserPicture;
        internal System.ComponentModel.BackgroundWorker BackgroundWorkerImageLoader;
        internal System.Windows.Forms.Label LabelScreenName;
        internal System.Windows.Forms.WebBrowser DescriptionBrowser;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuRecentPostBrowser;
        internal System.Windows.Forms.ToolStripMenuItem SelectionCopyToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
        internal System.Windows.Forms.Label LabelRecentPost;
        internal System.Windows.Forms.WebBrowser RecentPostBrowser;
        internal System.Windows.Forms.Label LabelIsVerified;
        internal System.Windows.Forms.LinkLabel LinkLabel1;
        internal System.Windows.Forms.Button ButtonSearchPosts;
        internal System.Windows.Forms.Label LabelId;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Button ButtonEdit;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuUserPicture;
        internal System.Windows.Forms.ToolStripMenuItem ChangeIconToolStripMenuItem;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialogIcon;
        internal System.Windows.Forms.TextBox TextBoxName;
        internal System.Windows.Forms.TextBox TextBoxLocation;
        internal System.Windows.Forms.TextBox TextBoxWeb;
        internal System.Windows.Forms.TextBox TextBoxDescription;
        internal System.Windows.Forms.Button ButtonBlock;
        internal System.Windows.Forms.Button ButtonReportSpam;
        internal System.Windows.Forms.Button ButtonBlockDestroy;
        internal System.Windows.Forms.LinkLabel LinkLabel2;
    }
}