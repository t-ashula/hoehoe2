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

namespace Hoehoe
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using Hoehoe.TweenCustomControl;

    public partial class TweenMain
    {
        #region event handler

        #region cleanuped

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowAboutBox();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            this.AddNewTab();
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeAllrepliesSetting(this.AllrepliesToolStripMenuItem.Checked);
        }

        private void ApiInfoMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowApiInfoBox();
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Bitly);
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowCacheInfoBox();
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearTab(this.rclickTabName, true);
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            this.SetupOperateContextMenu();
        }

        private void ContextMenuPostBrowser_Opening(object sender, CancelEventArgs e)
        {
            this.SetupPostBrowserContextMenu();
            e.Cancel = false;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            this.SetupPostModeContextMenu();
        }

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            this.SetupSourceContextMenu();
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            this.SetupTabPropertyContextMenu(fromMenuBar: false);
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            this.SetupUserPictureContextMenu();
        }

        private void CopySTOTMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyStot();
        }

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyIdUri();
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyUserId();
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchSelectedTextAtCurrentTab();
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, false);
        }

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelected();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedTab(fromMenuBar: false);
        }

        private void DeleteTbMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedTab(fromMenuBar: true);
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(this.displayItem))
            {
                this.UserPicture.ReplaceImage(this.displayItem.Image);
            }
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DispSelectedPost(true);
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExitApplication();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowEventViewerBox();
        }

        private void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeSelectedFavStatus(true);
        }

        private void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeSelectedFavStatus(false);
        }

        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenFavorarePageOfSelectedTweetUser();
        }

        private void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetOriginal();
        }

        private void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            this.FavoritesRetweetUnofficial();
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            this.ShowPostImageFileSelectBox();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFilterEditBox();
        }

        private void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            this.TryFollowUserOfCurrentTweet();
        }

        private void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            this.TryFollowUserOfCurrentLinkUser();
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryFollowUserOfCurrentIconUser();
        }

        private void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfAllUserInCurrentTweet();
        }

        private void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfCurrentLinkUser();
        }

        private void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfCurrentTweetUser();
        }

        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.GetFollowers();
        }

        private void GetTimelineWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.DisplayTimelineWorkerProgressChanged(e.ProgressPercentage, (string)e.UserState);
        }

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowHashManageBox();
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            this.ChangeUseHashTagSetting();
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeUseHashTagSetting();
        }

        private void HookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            this.ChangeWindowState();
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            this.AddIdFilteringRuleFromSelectedTweets();
        }

        private void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenCurrentTweetIconUrl();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            this.AddIdFilteringRuleFromCurrentTweet();
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetModifySettingCommon(true);
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
        {
            this.CancelPostImageSelecting();
        }

        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            this.ToggleImageSelectorView();
        }

        private void ImageSelectionPanel_VisibleChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChangedExtracted();
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.CancelPostImageSelecting();
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 0x1b)
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

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.TryChangeImageUploadService();
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
                this.LoadImageFromSelectedFile();
            }
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Isgd);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Jmp);
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            this.TrySearchAndFocusUnreadTweet();
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.ChangeListLockSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowListManageBox();
        }

        private void ListManageUserContextToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowListSelectForm(this.GetUserId());
        }

        private void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationHelpWebPageUrl);
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupCommandMenu();
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupEditMenu();
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupHelpMenu();
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupOperateMenu();
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            this.TrySearchWordInTabToBottom();
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            this.TrySearchWordInTabToTop();
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            this.TrySearchWordInTab();
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            this.SetupTabPropertyContextMenu(true);
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            this.SetFocusToMainMenu();
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            this.SetFocusFromMainMenu();
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenCurListSelectedUserFavorites();
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenCurListSelectedUserHome();
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenSelectedRtUserHome();
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked);
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.ChangeNewPostPopupSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeNotifySetting(((ToolStripMenuItem)sender).Checked);
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.ActivateMainForm();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ActivateMainForm();
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            this.SetNotifyIconText();
        }

        private void OpenOwnFavedMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenFavorarePageOfSelf();
        }

        private void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync("https://twitter.com/" + this.tw.Username);
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenUrlInCurrentTweet();
        }

        private void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUserAppointUrl();
        }

        private void OwnStatusMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowStatusOfUserSelf();
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangePlaySoundSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.PostBrowser_NavigatedExtracted(e.Url);
        }

        private void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = this.NavigateNextUrl(e.Url);
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

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            this.ChangeStatusLabelUrlTextByPostBrowserStatusText();
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            this.TryPostTweet();
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            this.FocusCurrentPublicSearchTabSearchInput();
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoQuote();
        }

        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetOfficial(true);
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoReTweetUnofficial();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeSelectedTweetReadStateToRead();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            // もっと前を取得
            this.RefreshTab(more: true);
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshTab();
        }

        private void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            this.TryUnfollowCurrentTweetUser();
        }

        private void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            this.TryUnfollowUserInCurrentTweet();
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryUnfollowCurrentIconUser();
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenRepliedStatus();
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true, true);
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MakeReplyOrDirectStatus(false, true);
        }

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowCurrentTweetRtCountBox();
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TrySaveCurrentTweetUserIcon();
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            this.TrySaveLog();
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveCurrentTweetUserOriginalSizeIcon();
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddSearchTabForAtUserOfCurrentTweet();
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddSearchTabForAtUserInCurrentTweet();
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            this.ChangeSearchPanelControlsTabStop((Control)sender, true);
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            this.ChangeSearchPanelControlsTabStop((Control)sender, false);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(Hoehoe.Properties.Resources.SearchItem2Url);
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddTimelineTabForCurrentTweetUser();
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddTimelineTabForUserInCurrentTweet();
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(Hoehoe.Properties.Resources.SearchItem4Url);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(Hoehoe.Properties.Resources.SearchItem1Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(Hoehoe.Properties.Resources.SearchItem3Url);
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAllItemInFocused();
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細ですべて選択
            this.PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            this.TryCopySelectionInPostBrowser();
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoTranslation(this.WebBrowser_GetSelectionText(this.PostBrowser));
        }

        private void TwitterApiInfo_Changed(object sender, ApiInformationChangedEventArgs e)
        {
            this.SetStatusLabelApiLuncher();
        }

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryShowSettingsBox();
        }

        private void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenUriAsync(ApplicationShortcutKeyHelpWebPageUrl);
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowFriendshipOfCurrentIconUser();
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowStatusOfCurrentTweetUser();
        }

        private void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            this.AddRelatedStatusesTab();
        }

        private void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowtStatusOfCurrentLinkUser();
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowStatusOfCurrentIconUser();
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowUserTimeline();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ChangeCurrentTabSoundFile((string)((ToolStripComboBox)sender).SelectedItem);
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            this.TryCopySourceName();
        }

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.TryOpenSourceLink();
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            this.ChangeStatusLabelUrlText((string)this.SourceLinkLabel.Tag);
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            this.SetStatusLabelUrl();
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            this.TryCopySourceUrl();
        }

        private void SpaceKeyCanceler_SpaceCancel(object sender, EventArgs e)
        {
            this.TrySearchAndFocusUnreadTweet();
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

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ChangeStatusTextMultilineState(this.MultiLineMenuItem.Checked); // this.MultiLineMenuItem.PerformClick();
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            this.StatusText.Multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            this.MultiLineMenuItem.Checked = this.StatusText.Multiline;
            this.modifySettingLocal = true;
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.StatusText.Multiline)
            {
                this.mySpDis2 = this.StatusText.Height;
            }

            this.modifySettingLocal = true;
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                this.modifySettingLocal = true;
            }
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

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myAdSpDis = this.SplitContainer4.SplitterDistance;
                this.modifySettingLocal = true;
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(this.StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenSelectedTweetWebPage();
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            this.StatusText_EnterExtracted();
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

            this.StatusText_TextChangedExtracted();
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;
            if (keyChar == '@' || keyChar == '#')
            {
                e.Handled = true;
                this.ShowSupplementBox(keyChar);
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
                        this.TrySearchAndFocusUnreadTweet();
                    }
                }
            }

            this.StatusText_TextChangedExtracted();
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            this.StatusText_LeaveExtracted();
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            this.ChangeStatusTextMultiline(this.StatusText.Multiline);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChangedExtracted();
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.TimelineRefreshEnableChange(!this.StopRefreshAllMenuItem.Checked);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeUserStreamStatus();
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                this.isOsResumed = true;
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            this.AddFilteringRuleFromSelectedTweet();
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            this.RenameCurrentTabName();
        }

        private void Tabs_DoubleClick(object sender, MouseEventArgs e)
        {
            this.RenameSelectedTabName();
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
            Point spos = this.ListTab.PointToClient(new Point(e.X, e.Y));
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Contains(spos))
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
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn)
            {
                return;
            }

            this.ReorderTab(tp.Text, tn, bef);
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

        private void Tabs_MouseDownExtracted(MouseEventArgs e)
        {
            if (this.configs.TabMouseLock)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                this.tabDraging = false;
            }
            else
            {
                Point cpos = e.Location;
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(cpos))
                    {
                        this.tabDraging = true;
                        this.tabMouseDownPoint = cpos;
                        break;
                    }
                }
            }
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            this.Tabs_MouseDownExtracted(e);
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e)
        {
            if (!this.timerTimeline.Enabled)
            {
                return;
            }

            this.resetTimers = e;
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            // 200ms
            this.RefreshTasktrayIcon(false);
        }

        private void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            this.TimerTimeline_ElapsedExtracted();
        }

        private void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.TinyUrl);
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.SetModifySettingCommon(true);
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangeAutoUrlConvertFlag(this.ToolStripMenuItemUrlAutoShorten.Checked);
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTraceFlag(this.TraceOutToolStripMenuItem.Checked);
        }

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeTrackWordStatus();
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TranslateCurrentTweet();
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            this.ActivateMainFormControls();
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            this.TweenMain_ClientSizeChangedExtracted();
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_LeaveExtracted();
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            this.DisposeAll();
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
                this.LoadImageFromSelectedFile();
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

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.TweenMain_FormClosingExtracted(e);
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            this.TweenMain_LoadExtracted();
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            this.TweenMain_LocationChangedExtracted();
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            this.ResizeMainForm();
        }

        private void TweenMain_Shown(object sender, EventArgs e)
        {
            this.TweenMain_ShownExtracted();
        }

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            this.TryRestartApplication();
        }

        private void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Twurl);
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            this.UndoRemoveTab();
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeSelectedTweetReadSateToUnread();
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeCurrentTabUnreadManagement(((ToolStripMenuItem)sender).Checked);
        }

        private void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrlByAutoSelectedService();
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            this.TryCopyUrlInCurrentTweet();
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UndoUrlShortening();
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            this.TrySetHashtagFromCurrentTweet();
        }

        private void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryOpenFavorarePageOfCurrentTweetUser();
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            this.TryOpenCurrentNameLabelUserHome();
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.ChangeUserPictureCursor(Cursors.Hand);
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            this.ChangeUserPictureCursor(Cursors.Default);
        }

        private void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TryShowStatusOfCurrentTweetUser();
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddNewTabForUserTimeline(this.GetUserIdFromCurPostOrInput("Show UserTimeline"));
        }

        private void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            this.ConvertUrl(UrlConverter.Uxnu);
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            this.CheckNewVersion();
        }

        #endregion cleanuped

        private void SearchButton_ClickExtracted(Control pnl)
        {
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

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }

            this.SearchButton_ClickExtracted(pnl);
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

        private void TweenMain_LoadExtracted()
        {
            this.ignoreConfigSave = true;
            this.Visible = false;
            this.VerUpMenuItem.Image = this.shield.Icon;
            this.spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this.spaceKeyCanceler.SpaceCancel += this.SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            this.InitializeTraceFrag();

            this.statuses = TabInformations.GetInstance(); // 発言保持クラス

            // アイコン設定
            this.LoadIcons();                        // アイコン読み込み
            this.Icon = this.mainIcon;               // メインフォーム（TweenMain）
            this.NotifyIcon1.Icon = this.iconAt;     // タスクトレイ
            this.TabImage.Images.Add(this.tabIcon);  // タブ見出し

            this.settingDialog.Owner = this;
            this.searchDialog.Owner = this;
            this.fltDialog.Owner = this;
            this.tabDialog.Owner = this;
            this.urlDialog.Owner = this;

            this.postHistory.Add(new PostingStatus());
            this.postHistoryIndex = 0;

            this.ClearReplyToInfo();

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

            this.InitUserBrushes();

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

            this.configs.UserAccounts = this.cfgCommon.UserAccounts;
            this.configs.TimelinePeriodInt = this.cfgCommon.TimelinePeriod;
            this.configs.ReplyPeriodInt = this.cfgCommon.ReplyPeriod;
            this.configs.DMPeriodInt = this.cfgCommon.DMPeriod;
            this.configs.PubSearchPeriodInt = this.cfgCommon.PubSearchPeriod;
            this.configs.UserTimelinePeriodInt = this.cfgCommon.UserTimelinePeriod;
            this.configs.ListsPeriodInt = this.cfgCommon.ListsPeriod;

            // 不正値チェック
            if (!MyCommon.NoLimit)
            {
                if (this.configs.TimelinePeriodInt < 15 && this.configs.TimelinePeriodInt > 0)
                {
                    this.configs.TimelinePeriodInt = 15;
                }

                if (this.configs.ReplyPeriodInt < 15 && this.configs.ReplyPeriodInt > 0)
                {
                    this.configs.ReplyPeriodInt = 15;
                }

                if (this.configs.DMPeriodInt < 15 && this.configs.DMPeriodInt > 0)
                {
                    this.configs.DMPeriodInt = 15;
                }

                if (this.configs.PubSearchPeriodInt < 30 && this.configs.PubSearchPeriodInt > 0)
                {
                    this.configs.PubSearchPeriodInt = 30;
                }

                if (this.configs.UserTimelinePeriodInt < 15 && this.configs.UserTimelinePeriodInt > 0)
                {
                    this.configs.UserTimelinePeriodInt = 15;
                }

                if (this.configs.ListsPeriodInt < 15 && this.configs.ListsPeriodInt > 0)
                {
                    this.configs.ListsPeriodInt = 15;
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
            this.detailHtmlFormatFooter = this.GetDetailHtmlFormatFooter(this.settingDialog.IsMonospace);
            this.detailHtmlFormatHeader = this.GetDetailHtmlFormatHeader(this.settingDialog.IsMonospace);

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
            this.configs.UserstreamStartup = this.cfgCommon.UserstreamStartup;
            this.configs.UserstreamPeriodInt = this.cfgCommon.UserstreamPeriod;
            this.settingDialog.OpenUserTimeline = this.cfgCommon.OpenUserTimeline;
            this.settingDialog.ListDoubleClickAction = this.cfgCommon.ListDoubleClickAction;
            this.settingDialog.UserAppointUrl = this.cfgCommon.UserAppointUrl;
            this.configs.HideDuplicatedRetweets = this.cfgCommon.HideDuplicatedRetweets;
            this.configs.IsPreviewFoursquare = this.cfgCommon.IsPreviewFoursquare;
            this.configs.FoursquarePreviewHeight = this.cfgCommon.FoursquarePreviewHeight;
            this.configs.FoursquarePreviewWidth = this.cfgCommon.FoursquarePreviewWidth;
            this.configs.FoursquarePreviewZoom = this.cfgCommon.FoursquarePreviewZoom;
            this.configs.IsListStatusesIncludeRts = this.cfgCommon.IsListsIncludeRts;
            this.configs.TabMouseLock = this.cfgCommon.TabMouseLock;
            this.configs.IsRemoveSameEvent = this.cfgCommon.IsRemoveSameEvent;
            this.configs.IsNotifyUseGrowl = this.cfgCommon.IsUseNotifyGrowl;

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

            this.iconDict.PauseGetImage = false;

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
                this.DisposeUserBrushes();
                this.InitUserBrushes();
                this.detailHtmlFormatFooter = this.GetDetailHtmlFormatFooter(this.settingDialog.IsMonospace);
                this.detailHtmlFormatHeader = this.GetDetailHtmlFormatHeader(this.settingDialog.IsMonospace);
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

            if (this.configs.IsNotifyUseGrowl)
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

            this.SplitContainer4.Panel2Collapsed = true;
            this.ignoreConfigSave = false;
            this.ResizeMainForm();
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

            foreach (var ua in this.configs.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == this.tw.Username.ToLower())
                {
                    ua.UserId = this.tw.UserId;
                    break;
                }
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
            this.tw.GetStatusRetweetedCount(this.CurPost.OriginalStatusId, ref counter);
            e.Result = counter;
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
                                ret = this.tw.PostFavAdd(post.OriginalStatusId);
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
                                ret = this.tw.PostFavRemove(post.OriginalStatusId);
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
            if (args.WorkerType == WorkerType.DirectMessegeRcv
                || args.WorkerType == WorkerType.DirectMessegeSnt
                || args.WorkerType == WorkerType.Reply
                || args.WorkerType == WorkerType.Timeline
                || args.WorkerType == WorkerType.Favorites)
            {
                // 値が正しいか後でチェック。10ページ毎の継続確認
                rslt.Page = args.Page - 1;
            }

            e.Result = rslt;
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

            if (rslt.WorkerType == WorkerType.Timeline
                || rslt.WorkerType == WorkerType.Reply
                || rslt.WorkerType == WorkerType.List
                || rslt.WorkerType == WorkerType.PublicSearch
                || rslt.WorkerType == WorkerType.DirectMessegeRcv
                || rslt.WorkerType == WorkerType.DirectMessegeSnt
                || rslt.WorkerType == WorkerType.Favorites
                || rslt.WorkerType == WorkerType.Follower
                || rslt.WorkerType == WorkerType.FavAdd
                || rslt.WorkerType == WorkerType.FavRemove
                || rslt.WorkerType == WorkerType.Related
                || rslt.WorkerType == WorkerType.UserTimeline
                || rslt.WorkerType == WorkerType.BlockIds
                || rslt.WorkerType == WorkerType.Configuration)
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
                        this.GetTimeline(WorkerType.DirectMessegeRcv);
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
                            retry = MessageBox.Show(
                                string.Format("{0}   --->   [ {1} ]{2}\"{3}\"{2}{4}", Hoehoe.Properties.Resources.StatusUpdateFailed1, rslt.RetMsg, Environment.NewLine, rslt.PStatus.Status, Hoehoe.Properties.Resources.StatusUpdateFailed2),
                                "Failed to update status", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
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
                                this.StatusText_EnterExtracted();
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
                            this.GetTimeline(WorkerType.Timeline);
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
                            this.GetTimeline(WorkerType.Timeline);
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
                    if (this.configs.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        this.pictureServices["Twitter"].Configuration("MaxUploadFilesize", this.configs.TwitterConfiguration.PhotoSizeLimit);
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

        #endregion callback

        #region ListTab events

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this.itemCache = null;
            this.itemCacheIndex = -1;
            this.postCache = null;
            this.prevSelectedTab = e.TabPage;
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

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D
            if (!this.configs.TabMouseLock && e.Button == MouseButtons.Left && this.tabDraging)
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

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            this.tabDraging = false;
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

            this.ChangeTabMenuControl(this.ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.ListTabSelect(e.TabPage);
        }

        #endregion ListTab events

        #region MyList events

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

        private void MyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void MyList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.State == 0)
            {
                return;
            }

            e.DrawDefault = false;
            if (e.Item.Selected)
            {
                // 選択中の行
                SolidBrush brs1 = ((Control)sender).Focused ? this.brsHighLight : this.brsDeactiveSelection;
                e.Graphics.FillRectangle(brs1, e.Bounds);
            }
            else
            {
                var cl = e.Item.BackColor;
                SolidBrush brs2 = (cl == this.clrSelf) ? this.brsBackColorMine :
                    (cl == this.clrAtSelf) ? this.brsBackColorAt :
                    (cl == this.clrTarget) ? this.brsBackColorYou :
                    (cl == this.clrAtTarget) ? this.brsBackColorAtYou :
                    (cl == this.clrAtFromTarget) ? this.brsBackColorAtFromTarget :
                    (cl == this.clrAtTo) ? this.brsBackColorAtTo : this.brsBackColorNone;
                e.Graphics.FillRectangle(brs2, e.Bounds);
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

            if (e.ColumnIndex < 1)
            {
                return;
            }

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
            else
            {
                if (heightDiff < e.Item.Font.Height * 0.7)
                {
                    rct.Height = (float)(e.Item.Font.Height * drawLineCount) - 1;
                }
                else
                {
                    drawLineCount += 1;
                }
            }

            if (!(rct.Width > 0))
            {
                return;
            }

            Color foreColor;
            if (e.Item.Selected)
            {
                foreColor = (((Control)sender).Focused) ? this.brsHighLightText.Color : this.brsForeColorUnread.Color;
            }
            else
            {
                // 選択されていない行 // 文字色                    
                var cl = e.Item.ForeColor;
                foreColor =
                    cl == this.clrUnread ? this.brsForeColorUnread.Color :
                    cl == this.clrRead ? this.brsForeColorReaded.Color :
                    cl == this.clrFav ? this.brsForeColorFav.Color :
                    cl == this.clrOWL ? this.brsForeColorOWL.Color :
                    cl == this.clrRetweet ? this.brsForeColorRetweet.Color : cl;
            }

            var multiLineFmt = TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            var singleLineFmt = TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            if (this.iconCol)
            {
                var subitems = e.Item.SubItems;
                string iconcol2txt = string.Format("{0} / {1} ({2}) {3}{4} [{5}]", subitems[4].Text, subitems[1].Text, subitems[3].Text, subitems[5].Text, subitems[6].Text, subitems[7].Text);
                using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                {
                    TextRenderer.DrawText(e.Graphics, subitems[2].Text, e.Item.Font, Rectangle.Round(rct), foreColor, multiLineFmt);
                    TextRenderer.DrawText(e.Graphics, iconcol2txt, fnt, Rectangle.Round(rctB), foreColor, singleLineFmt);
                }
            }
            else
            {
                var frmflg = (drawLineCount == 1) ? singleLineFmt | TextFormatFlags.VerticalCenter : multiLineFmt;
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, Rectangle.Round(rct), foreColor, frmflg);
            }
        }

        private void MyList_HScrolled(object sender, EventArgs e)
        {
            ((DetailsListView)sender).Refresh();
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            this.anchorFlag = false;
        }

        private void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (this.settingDialog.ListDoubleClickAction)
            {
                case 0:
                    this.MakeReplyOrDirectStatus();
                    break;
                case 1:
                    this.ChangeSelectedFavStatus(true);
                    break;
                case 2:
                    this.ShowStatusOfCurrentTweetUser();
                    break;
                case 3:
                    this.ShowUserTimeline();
                    break;
                case 4:
                    this.AddRelatedStatusesTab();
                    break;
                case 5:
                    this.TryOpenCurListSelectedUserHome();
                    break;
                case 6:
                    this.TryOpenSelectedTweetWebPage();
                    break;
                case 7:
                    // 動作なし
                    break;
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

        #endregion MyList events

        #region userstream

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

                DateTime oneHour = tm.Subtract(new TimeSpan(1, 0, 0));
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

            if (this.configs.UserstreamPeriodInt > 0)
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

        private void Tw_UserIdChanged()
        {
            this.modifySettingCommon = true;
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

            this.ChangeUserStreamStatusDisplay(start: true);
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

            this.ChangeUserStreamStatusDisplay(start: false);
        }

        #endregion userstream

        #endregion event handler
    }
}