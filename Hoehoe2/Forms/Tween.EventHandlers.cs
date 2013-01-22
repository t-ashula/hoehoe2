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
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using Hoehoe.TweenCustomControl;
    using R = Hoehoe.Properties.Resources;

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
            this.ClearTab(this._rclickTabName, true);
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
            if (sender.Equals(this._displayItem))
            {
                this.UserPicture.ReplaceImage(this._displayItem.Image);
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
            this.OpenUriAsync("https://twitter.com/" + this._tw.Username);
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
            this.SearchWebBySelectedWord(R.SearchItem2Url);
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
            this.SearchWebBySelectedWord(R.SearchItem4Url);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(R.SearchItem1Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            this.SearchWebBySelectedWord(R.SearchItem3Url);
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
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._mySpDis = this.SplitContainer1.SplitterDistance;
                if (this.StatusText.Multiline)
                {
                    this._mySpDis2 = this.StatusText.Height;
                }

                this._modifySettingLocal = true;
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
            this._modifySettingLocal = true;
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.StatusText.Multiline)
            {
                this._mySpDis2 = this.StatusText.Height;
            }

            this._modifySettingLocal = true;
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._mySpDis3 = this.SplitContainer3.SplitterDistance;
                this._modifySettingLocal = true;
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
            if (this.WindowState == FormWindowState.Normal && !this._initialLayout)
            {
                this._myAdSpDis = this.SplitContainer4.SplitterDistance;
                this._modifySettingLocal = true;
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
                this._isOsResumed = true;
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

            this._tabDraging = false;
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
            if (this._configs.TabMouseLock)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                this._tabDraging = false;
            }
            else
            {
                Point cpos = e.Location;
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(cpos))
                    {
                        this._tabDraging = true;
                        this._tabMouseDownPoint = cpos;
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
            if (!this._timerTimeline.Enabled)
            {
                return;
            }

            this._resetTimers = e;
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

                if (!string.IsNullOrEmpty(this.ImageService) && this._pictureServices[this.ImageService].CheckValidFilesize(ext, fl.Length))
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

                    if (this._pictureServices[svc].CheckValidFilesize(ext, fl.Length))
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
            TabClass tb = this._statuses.Tabs[tabName];
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
                this._statuses.ClearTabIds(tabName);
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
            this._ignoreConfigSave = true;
            this.Visible = false;
            this.VerUpMenuItem.Image = this._shield.Icon;
            this._spaceKeyCanceler = new SpaceKeyCanceler(this.PostButton);
            this._spaceKeyCanceler.SpaceCancel += this.SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            this.InitializeTraceFrag();

            this._statuses = TabInformations.Instance; // 発言保持クラス

            // アイコン設定
            this.LoadIcons();                        // アイコン読み込み
            this.Icon = this._mainIcon;               // メインフォーム（TweenMain）
            this.NotifyIcon1.Icon = this._iconAt;     // タスクトレイ
            this.TabImage.Images.Add(this._tabIcon);  // タブ見出し

            this._settingDialog.Owner = this;
            this._searchDialog.Owner = this;
            this._fltDialog.Owner = this;
            this._tabDialog.Owner = this;
            this._urlDialog.Owner = this;

            this._postHistory.Add(new PostingStatus());
            this._postHistoryIndex = 0;

            this.ClearReplyToInfo();

            // <<<<<<<<<設定関連>>>>>>>>>
            // '設定読み出し
            this.LoadConfig();

            // 新着バルーン通知のチェック状態設定
            this.NewPostPopMenuItem.Checked = this._cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = this._cfgCommon.NewAllPop;

            // フォント＆文字色＆背景色保持
            this._fntUnread = this._cfgLocal.FontUnread;
            this._clrUnread = this._cfgLocal.ColorUnread;
            this._fntReaded = this._cfgLocal.FontRead;
            this._clrRead = this._cfgLocal.ColorRead;
            this._clrFav = this._cfgLocal.ColorFav;
            this._clrOwl = this._cfgLocal.ColorOWL;
            this._clrRetweet = this._cfgLocal.ColorRetweet;
            this._fntDetail = this._cfgLocal.FontDetail;
            this._clrDetail = this._cfgLocal.ColorDetail;
            this._clrDetailLink = this._cfgLocal.ColorDetailLink;
            this._clrDetailBackcolor = this._cfgLocal.ColorDetailBackcolor;
            this._clrSelf = this._cfgLocal.ColorSelf;
            this._clrAtSelf = this._cfgLocal.ColorAtSelf;
            this._clrTarget = this._cfgLocal.ColorTarget;
            this._clrAtTarget = this._cfgLocal.ColorAtTarget;
            this._clrAtFromTarget = this._cfgLocal.ColorAtFromTarget;
            this._clrAtTo = this._cfgLocal.ColorAtTo;
            this._clrListBackcolor = this._cfgLocal.ColorListBackcolor;
            this.InputBackColor = this._cfgLocal.ColorInputBackcolor;
            this._clrInputForecolor = this._cfgLocal.ColorInputFont;
            this._fntInputFont = this._cfgLocal.FontInputFont;

            this.InitUserBrushes();

            // StringFormatオブジェクトへの事前設定
            this._tabStringFormat.Alignment = StringAlignment.Center;
            this._tabStringFormat.LineAlignment = StringAlignment.Center;

            // 設定画面への反映
            HttpTwitter.SetTwitterUrl(this._cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this._cfgCommon.TwitterSearchUrl);
            this._configs.TwitterApiUrl = this._cfgCommon.TwitterUrl;
            this._configs.TwitterSearchApiUrl = this._cfgCommon.TwitterSearchUrl;

            // 認証関連
            if (string.IsNullOrEmpty(this._cfgCommon.Token))
            {
                this._cfgCommon.UserName = string.Empty;
            }

            this._tw.Initialize(this._cfgCommon.Token, this._cfgCommon.TokenSecret, this._cfgCommon.UserName, this._cfgCommon.UserId);

            this._configs.UserAccounts = this._cfgCommon.UserAccounts;
            this._configs.TimelinePeriodInt = this._cfgCommon.TimelinePeriod;
            this._configs.ReplyPeriodInt = this._cfgCommon.ReplyPeriod;
            this._configs.DMPeriodInt = this._cfgCommon.DMPeriod;
            this._configs.PubSearchPeriodInt = this._cfgCommon.PubSearchPeriod;
            this._configs.UserTimelinePeriodInt = this._cfgCommon.UserTimelinePeriod;
            this._configs.ListsPeriodInt = this._cfgCommon.ListsPeriod;

            // 不正値チェック
            if (!MyCommon.NoLimit)
            {
                if (this._configs.TimelinePeriodInt < 15 && this._configs.TimelinePeriodInt > 0)
                {
                    this._configs.TimelinePeriodInt = 15;
                }

                if (this._configs.ReplyPeriodInt < 15 && this._configs.ReplyPeriodInt > 0)
                {
                    this._configs.ReplyPeriodInt = 15;
                }

                if (this._configs.DMPeriodInt < 15 && this._configs.DMPeriodInt > 0)
                {
                    this._configs.DMPeriodInt = 15;
                }

                if (this._configs.PubSearchPeriodInt < 30 && this._configs.PubSearchPeriodInt > 0)
                {
                    this._configs.PubSearchPeriodInt = 30;
                }

                if (this._configs.UserTimelinePeriodInt < 15 && this._configs.UserTimelinePeriodInt > 0)
                {
                    this._configs.UserTimelinePeriodInt = 15;
                }

                if (this._configs.ListsPeriodInt < 15 && this._configs.ListsPeriodInt > 0)
                {
                    this._configs.ListsPeriodInt = 15;
                }
            }

            // 起動時読み込み分を既読にするか。Trueなら既読として処理
            this._configs.Readed = this._cfgCommon.Read;

            // 新着取得時のリストスクロールをするか。Trueならスクロールしない
            this.ListLockMenuItem.Checked = this._cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = this._cfgCommon.ListLock;
            this._configs.IconSz = this._cfgCommon.IconSize;

            // 文末ステータス
            this._configs.Status = this._cfgLocal.StatusText;

            // 未読管理。Trueなら未読管理する
            this._configs.UnreadManage = this._cfgCommon.UnreadManage;

            // サウンド再生（タブ別設定より優先）
            this._configs.PlaySound = this._cfgCommon.PlaySound;
            this.PlaySoundMenuItem.Checked = this._configs.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this._configs.PlaySound;

            // 片思い表示。Trueなら片思い表示する
            this._configs.OneWayLove = this._cfgCommon.OneWayLove;

            // フォント＆文字色＆背景色
            this._configs.FontUnread = this._fntUnread;
            this._configs.ColorUnread = this._clrUnread;
            this._configs.FontReaded = this._fntReaded;
            this._configs.ColorReaded = this._clrRead;
            this._configs.ColorFav = this._clrFav;
            this._configs.ColorOWL = this._clrOwl;
            this._configs.ColorRetweet = this._clrRetweet;
            this._configs.FontDetail = this._fntDetail;
            this._configs.ColorDetail = this._clrDetail;
            this._configs.ColorDetailLink = this._clrDetailLink;
            this._configs.ColorDetailBackcolor = this._clrDetailBackcolor;
            this._configs.ColorSelf = this._clrSelf;
            this._configs.ColorAtSelf = this._clrAtSelf;
            this._configs.ColorTarget = this._clrTarget;
            this._configs.ColorAtTarget = this._clrAtTarget;
            this._configs.ColorAtFromTarget = this._clrAtFromTarget;
            this._configs.ColorAtTo = this._clrAtTo;
            this._configs.ColorListBackcolor = this._clrListBackcolor;
            this._configs.ColorInputBackcolor = this.InputBackColor;
            this._configs.ColorInputFont = this._clrInputForecolor;
            this._configs.FontInputFont = this._fntInputFont;
            this._configs.NameBalloon = this._cfgCommon.NameBalloon;
            this._configs.PostCtrlEnter = this._cfgCommon.PostCtrlEnter;
            this._configs.PostShiftEnter = this._cfgCommon.PostShiftEnter;
            this._configs.CountApi = this._cfgCommon.CountApi;
            this._configs.CountApiReply = this._cfgCommon.CountApiReply;
            if (this._configs.CountApi < 20 || this._configs.CountApi > 200)
            {
                this._configs.CountApi = 60;
            }

            if (this._configs.CountApiReply < 20 || this._configs.CountApiReply > 200)
            {
                this._configs.CountApiReply = 40;
            }

            this._configs.BrowserPath = this._cfgLocal.BrowserPath;
            this._configs.PostAndGet = this._cfgCommon.PostAndGet;
            this._configs.UseRecommendStatus = this._cfgLocal.UseRecommendStatus;
            this._configs.DispUsername = this._cfgCommon.DispUsername;
            this._configs.CloseToExit = this._cfgCommon.CloseToExit;
            this._configs.MinimizeToTray = this._cfgCommon.MinimizeToTray;
            this._configs.DispLatestPost = this._cfgCommon.DispLatestPost;
            this._configs.SortOrderLock = this._cfgCommon.SortOrderLock;
            this._configs.TinyUrlResolve = this._cfgCommon.TinyUrlResolve;
            this._configs.ShortUrlForceResolve = this._cfgCommon.ShortUrlForceResolve;
            this._configs.SelectedProxyType = this._cfgLocal.ProxyType;
            this._configs.ProxyAddress = this._cfgLocal.ProxyAddress;
            this._configs.ProxyPort = this._cfgLocal.ProxyPort;
            this._configs.ProxyUser = this._cfgLocal.ProxyUser;
            this._configs.ProxyPassword = this._cfgLocal.ProxyPassword;
            this._configs.PeriodAdjust = this._cfgCommon.PeriodAdjust;
            this._configs.StartupVersion = this._cfgCommon.StartupVersion;
            this._configs.StartupFollowers = this._cfgCommon.StartupFollowers;
            this._configs.RestrictFavCheck = this._cfgCommon.RestrictFavCheck;
            this._configs.AlwaysTop = this._cfgCommon.AlwaysTop;
            this._configs.UrlConvertAuto = false;
            this._configs.OutputzEnabled = this._cfgCommon.Outputz;
            this._configs.OutputzKey = this._cfgCommon.OutputzKey;
            this._configs.OutputzUrlmode = this._cfgCommon.OutputzUrlMode;
            this._configs.UseUnreadStyle = this._cfgCommon.UseUnreadStyle;
            this._configs.DefaultTimeOut = this._cfgCommon.DefaultTimeOut;
            this._configs.RetweetNoConfirm = this._cfgCommon.RetweetNoConfirm;
            this._configs.PlaySound = this._cfgCommon.PlaySound;
            this._configs.DateTimeFormat = this._cfgCommon.DateTimeFormat;
            this._configs.LimitBalloon = this._cfgCommon.LimitBalloon;
            this._configs.EventNotifyEnabled = this._cfgCommon.EventNotifyEnabled;
            this._configs.EventNotifyFlag = this._cfgCommon.EventNotifyFlag;
            this._configs.IsMyEventNotifyFlag = this._cfgCommon.IsMyEventNotifyFlag;
            this._configs.ForceEventNotify = this._cfgCommon.ForceEventNotify;
            this._configs.FavEventUnread = this._cfgCommon.FavEventUnread;
            this._configs.TranslateLanguage = this._cfgCommon.TranslateLanguage;
            this._configs.EventSoundFile = this._cfgCommon.EventSoundFile;

            // 廃止サービスが選択されていた場合bit.lyへ読み替え
            if (this._cfgCommon.AutoShortUrlFirst < 0)
            {
                this._cfgCommon.AutoShortUrlFirst = UrlConverter.Bitly;
            }

            this._configs.AutoShortUrlFirst = this._cfgCommon.AutoShortUrlFirst;
            this._configs.TabIconDisp = this._cfgCommon.TabIconDisp;
            this._configs.ReplyIconState = this._cfgCommon.ReplyIconState;
            this._configs.ReadOwnPost = this._cfgCommon.ReadOwnPost;
            this._configs.GetFav = this._cfgCommon.GetFav;
            this._configs.ReadOldPosts = this._cfgCommon.ReadOldPosts;
            this._configs.UseSsl = this._cfgCommon.UseSsl;
            this._configs.BitlyUser = this._cfgCommon.BilyUser;
            this._configs.BitlyPwd = this._cfgCommon.BitlyPwd;
            this._configs.ShowGrid = this._cfgCommon.ShowGrid;
            this._configs.Language = this._cfgCommon.Language;
            this._configs.UseAtIdSupplement = this._cfgCommon.UseAtIdSupplement;
            this._configs.UseHashSupplement = this._cfgCommon.UseHashSupplement;
            this._configs.PreviewEnable = this._cfgCommon.PreviewEnable;
            this.AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            this._configs.IsMonospace = this._cfgCommon.IsMonospace;
            this._detailHtmlFormatFooter = this.GetDetailHtmlFormatFooter(this._configs.IsMonospace);
            this._detailHtmlFormatHeader = this.GetDetailHtmlFormatHeader(this._configs.IsMonospace);

            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = this._cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = this._cfgCommon.FocusLockToStatusText;

            this._configs.RecommendStatusText = " [HH2v" + Regex.Replace(MyCommon.FileVersion.Replace(".", string.Empty), "^0*", string.Empty) + "]";

            // 書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(this._configs.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    this._configs.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                this._configs.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            this._configs.Nicoms = this._cfgCommon.Nicoms;
            this._configs.HotkeyEnabled = this._cfgCommon.HotkeyEnabled;
            this._configs.HotkeyMod = this._cfgCommon.HotkeyModifier;
            this._configs.HotkeyKey = this._cfgCommon.HotkeyKey;
            this._configs.HotkeyValue = this._cfgCommon.HotkeyValue;
            this._configs.BlinkNewMentions = this._cfgCommon.BlinkNewMentions;
            this._configs.UseAdditionalCount = this._cfgCommon.UseAdditionalCount;
            this._configs.MoreCountApi = this._cfgCommon.MoreCountApi;
            this._configs.FirstCountApi = this._cfgCommon.FirstCountApi;
            this._configs.SearchCountApi = this._cfgCommon.SearchCountApi;
            this._configs.FavoritesCountApi = this._cfgCommon.FavoritesCountApi;
            this._configs.UserTimelineCountApi = this._cfgCommon.UserTimelineCountApi;
            this._configs.ListCountApi = this._cfgCommon.ListCountApi;
            this._configs.UserstreamStartup = this._cfgCommon.UserstreamStartup;
            this._configs.UserstreamPeriodInt = this._cfgCommon.UserstreamPeriod;
            this._configs.OpenUserTimeline = this._cfgCommon.OpenUserTimeline;
            this._configs.ListDoubleClickAction = this._cfgCommon.ListDoubleClickAction;
            this._configs.UserAppointUrl = this._cfgCommon.UserAppointUrl;
            this._configs.HideDuplicatedRetweets = this._cfgCommon.HideDuplicatedRetweets;
            this._configs.IsPreviewFoursquare = this._cfgCommon.IsPreviewFoursquare;
            this._configs.FoursquarePreviewHeight = this._cfgCommon.FoursquarePreviewHeight;
            this._configs.FoursquarePreviewWidth = this._cfgCommon.FoursquarePreviewWidth;
            this._configs.FoursquarePreviewZoom = this._cfgCommon.FoursquarePreviewZoom;
            this._configs.IsListStatusesIncludeRts = this._cfgCommon.IsListsIncludeRts;
            this._configs.TabMouseLock = this._cfgCommon.TabMouseLock;
            this._configs.IsRemoveSameEvent = this._cfgCommon.IsRemoveSameEvent;
            this._configs.IsNotifyUseGrowl = this._cfgCommon.IsUseNotifyGrowl;

            // ハッシュタグ関連
            this.HashSupl = new AtIdSupplement(this._cfgCommon.HashTags, "#");
            this.HashMgr = new HashtagManage(this.HashSupl, this._cfgCommon.HashTags.ToArray(), this._cfgCommon.HashSelected, this._cfgCommon.HashIsPermanent, this._cfgCommon.HashIsHead, this._cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(this.HashMgr.UseHash) && this.HashMgr.IsPermanent)
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
            }

            this._isInitializing = true;

            // アイコンリスト作成
            try
            {
                this._iconDict = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }

            this._iconDict.PauseGetImage = false;

            bool saveRequired = false;

            // ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(this._tw.Username))
            {
                saveRequired = true;

                // 設定せずにキャンセルされた場合はプログラム終了
                if (this._settingDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // 設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
                if (string.IsNullOrEmpty(this._tw.Username))
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // フォント＆文字色＆背景色保持
                this._fntUnread = this._configs.FontUnread;
                this._clrUnread = this._configs.ColorUnread;
                this._fntReaded = this._configs.FontReaded;
                this._clrRead = this._configs.ColorReaded;
                this._clrFav = this._configs.ColorFav;
                this._clrOwl = this._configs.ColorOWL;
                this._clrRetweet = this._configs.ColorRetweet;
                this._fntDetail = this._configs.FontDetail;
                this._clrDetail = this._configs.ColorDetail;
                this._clrDetailLink = this._configs.ColorDetailLink;
                this._clrDetailBackcolor = this._configs.ColorDetailBackcolor;
                this._clrSelf = this._configs.ColorSelf;
                this._clrAtSelf = this._configs.ColorAtSelf;
                this._clrTarget = this._configs.ColorTarget;
                this._clrAtTarget = this._configs.ColorAtTarget;
                this._clrAtFromTarget = this._configs.ColorAtFromTarget;
                this._clrAtTo = this._configs.ColorAtTo;
                this._clrListBackcolor = this._configs.ColorListBackcolor;
                this.InputBackColor = this._configs.ColorInputBackcolor;
                this._clrInputForecolor = this._configs.ColorInputFont;
                this._fntInputFont = this._configs.FontInputFont;
                this.DisposeUserBrushes();
                this.InitUserBrushes();
                this._detailHtmlFormatFooter = this.GetDetailHtmlFormatFooter(this._configs.IsMonospace);
                this._detailHtmlFormatHeader = this.GetDetailHtmlFormatHeader(this._configs.IsMonospace);
            }

            if (this._configs.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((this._configs.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }

                if ((this._configs.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }

                if ((this._configs.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }

                if ((this._configs.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                this._hookGlobalHotkey.RegisterOriginalHotkey(this._configs.HotkeyKey, this._configs.HotkeyValue, modKey);
            }

            // Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(this._configs.DefaultTimeOut, this._configs.SelectedProxyType, this._configs.ProxyAddress, this._configs.ProxyPort, this._configs.ProxyUser, this._configs.ProxyPassword);

            this._tw.SetRestrictFavCheck(this._configs.RestrictFavCheck);
            this._tw.ReadOwnPost = this._configs.ReadOwnPost;
            this._tw.SetUseSsl(this._configs.UseSsl);
            ShortUrl.IsResolve = this._configs.TinyUrlResolve;
            ShortUrl.IsForceResolve = this._configs.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(this._configs.BitlyUser);
            ShortUrl.SetBitlyKey(this._configs.BitlyPwd);
            HttpTwitter.SetTwitterUrl(this._cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(this._cfgCommon.TwitterSearchUrl);
            this._tw.TrackWord = this._cfgCommon.TrackWord;
            this.TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(this._tw.TrackWord);
            this._tw.AllAtReply = this._cfgCommon.AllAtReply;
            this.AllrepliesToolStripMenuItem.Checked = this._tw.AllAtReply;

            Outputz.Key = this._configs.OutputzKey;
            Outputz.Enabled = this._configs.OutputzEnabled;
            switch (this._configs.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    Outputz.OutUrl = "http://twitter.com/";
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    Outputz.OutUrl = "http://twitter.com/" + this._tw.Username;
                    break;
            }

            // 画像投稿サービス
            this.CreatePictureServices();
            this.SetImageServiceCombo();
            this.ImageSelectionPanel.Enabled = false;
            this.ImageServiceCombo.SelectedIndex = this._cfgCommon.UseImageService;

            // ウィンドウ設定
            this.ClientSize = this._cfgLocal.FormSize;
            this._mySize = this._cfgLocal.FormSize;          // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            this._myLoc = this._cfgLocal.FormLocation;       // タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = this._cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(this._mySize.Width, SystemInformation.CaptionHeight));
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
                        this._myLoc = this.DesktopLocation;
                    }
                }
            }

            this.TopMost = this._configs.AlwaysTop;
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
            this.MultiLineMenuItem.Checked = this._cfgLocal.StatusMultiline;
            this.PlaySoundMenuItem.Checked = this._configs.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this._configs.PlaySound;

            // 入力欄
            this.StatusText.Font = this._fntInputFont;
            this.StatusText.ForeColor = this._clrInputForecolor;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (!this._configs.UnreadManage)
            {
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
            }

            if (this._configs.IsNotifyUseGrowl)
            {
                this._growlHelper.RegisterGrowl();
            }

            // タイマー設定
            this._timerTimeline.AutoReset = true;
            this._timerTimeline.SynchronizingObject = this;

            // Recent取得間隔
            this._timerTimeline.Interval = 1000;
            this._timerTimeline.Enabled = true;

            // 更新中アイコンアニメーション間隔
            this.TimerRefreshIcon.Interval = 200;
            this.TimerRefreshIcon.Enabled = true;

            // 状態表示部の初期化（画面右下）
            this.StatusLabel.Text = string.Empty;
            this.StatusLabel.AutoToolTip = false;
            this.StatusLabel.ToolTipText = string.Empty;

            // 文字カウンタ初期化
            this.lblLen.Text = this.GetRestStatusCount(true, false).ToString();

            this._statuses.SortOrder = (SortOrder)this._cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (this._cfgCommon.SortColumn)
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

            this._statuses.SortMode = mode;

            switch (this._configs.IconSz)
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
                this._tw.SetGetIcon(false);
            }
            else
            {
                this._tw.SetGetIcon(true);
                this._tw.SetIconSize(this._iconSz);
            }

            this._tw.SetTinyUrlResolve(this._configs.TinyUrlResolve);
            ShortUrl.IsForceResolve = this._configs.ShortUrlForceResolve;

            this._tw.DetailIcon = this._iconDict;
            this.StatusLabel.Text = R.Form1_LoadText1;  // 画面右下の状態表示を変更
            this.StatusLabelUrl.Text = string.Empty;  // 画面左下のリンク先URL表示部を初期化
            this.NameLabel.Text = string.Empty;       // 発言詳細部名前ラベル初期化
            this.DateTimeLabel.Text = string.Empty;   // 発言詳細部日時ラベル初期化
            this.SourceLinkLabel.Text = string.Empty; // Source部分初期化

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
                    throw new Exception(R.TweenMain_LoadText1);
                }
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            this.CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            this.CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (!this._configs.MinimizeToTray || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }

            this._curTab = this.ListTab.SelectedTab;
            this._curItemIndex = -1;
            this._curList = (DetailsListView)this._curTab.Tag;
            this.SetMainWindowTitle();
            this.SetNotifyIconText();

            if (this._configs.TabIconDisp)
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
            this._ignoreConfigSave = false;
            this.ResizeMainForm();
            if (saveRequired)
            {
                this.SaveConfigsAll(false);
            }

            if (this._tw.UserId == 0)
            {
                this._tw.VerifyCredentials();
                foreach (var ua in this._cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == this._tw.Username.ToLower())
                    {
                        ua.UserId = this._tw.UserId;
                        break;
                    }
                }
            }

            foreach (var ua in this._configs.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == this._tw.Username.ToLower())
                {
                    ua.UserId = this._tw.UserId;
                    break;
                }
            }
        }

        #region callback

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = this._tw.GetInfoApi(args.Info);
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
            this._tw.GetStatusRetweetedCount(this.CurPost.OriginalStatusId, ref counter);
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
            bool read = !this._configs.UnreadManage;
            if (this._isInitializing && this._configs.UnreadManage)
            {
                read = this._configs.Readed;
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
                    ret = this._tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, this._isInitializing);
                    if (string.IsNullOrEmpty(ret) && args.WorkerType == WorkerType.Timeline && this._configs.ReadOldPosts)
                    {
                        // 新着時未読クリア
                        this._statuses.SetRead();
                    }

                    rslt.AddCount = this._statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.DirectMessegeRcv:
                    // 送信分もまとめて取得
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this._tw.GetDirectMessageApi(read, WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this._tw.GetDirectMessageApi(read, WorkerType.DirectMessegeSnt, args.Page == -1);
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
                                ret = this._tw.PostFavAdd(post.OriginalStatusId);
                                if (ret.Length == 0)
                                {
                                    // リスト再描画必要
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
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
                                ret = this._tw.PostFavRemove(post.OriginalStatusId);
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
                            ret = this._tw.PostStatus(args.PStatus.Status, args.PStatus.InReplyToId);
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
                        ret = this._tw.PostRetweet(args.Ids[i], read);
                    }

                    bw.ReportProgress(300);
                    break;
                case WorkerType.Follower:
                    bw.ReportProgress(50, R.UpdateFollowersMenuItem1_ClickText1);
                    ret = this._tw.GetFollowersApi();
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = this._tw.GetNoRetweetIdsApi();
                    }

                    break;
                case WorkerType.Configuration:
                    ret = this._tw.ConfigurationApi();
                    break;
                case WorkerType.OpenUri:
                    string myPath = args.Url;
                    string browserPath = this._configs.BrowserPath;
                    MyCommon.TryOpenUrl(myPath, browserPath);
                    break;
                case WorkerType.Favorites:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this._tw.GetFavoritesApi(read, args.WorkerType, args.Page == -1);
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
                                ret = this._tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this._tw.GetSearch(read, tb, false);
                            if (string.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = this._tw.GetSearch(read, tb, true);
                            }
                        }
                    }

                    rslt.AddCount = this._statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.UserTimeline:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    int count = 20;
                    if (this._configs.UseAdditionalCount)
                    {
                        count = this._configs.UserTimelineCountApi;
                    }

                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in this._statuses.GetTabsByType(TabUsageType.UserTimeline))
                        {
                            if (!string.IsNullOrEmpty(tb.User))
                            {
                                ret = this._tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this._tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }

                    rslt.AddCount = this._statuses.DistributePosts();                     // 振り分け
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
                                ret = this._tw.GetListStatus(read, tb, false, this._isInitializing);
                            }
                        }
                    }
                    else
                    {
                        // 手動更新（特定タブのみ更新）
                        TabClass tb = this._statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = this._tw.GetListStatus(read, tb, args.Page == -1, this._isInitializing);
                        }
                    }

                    rslt.AddCount = this._statuses.DistributePosts(); // 振り分け
                    break;
                case WorkerType.Related:
                    bw.ReportProgress(50, this.MakeStatusMessage(args, false));
                    ret = this._tw.GetRelatedResult(read, this._statuses.GetTabByName(args.TabName));
                    rslt.AddCount = this._statuses.DistributePosts();
                    break;
                case WorkerType.BlockIds:
                    bw.ReportProgress(50, R.UpdateBlockUserText1);
                    ret = this._tw.GetBlockUserIds();
                    if (TabInformations.Instance.BlockIds.Count == 0)
                    {
                        this._tw.GetBlockUserIds();
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

            if (args.WorkerType == WorkerType.Timeline && !this._isInitializing)
            {
                lock (this._syncObject)
                {
                    DateTime tm = DateTime.Now;
                    if (this._timeLineTimestamps.ContainsKey(tm))
                    {
                        this._timeLineTimestamps[tm] += rslt.AddCount;
                    }
                    else
                    {
                        this._timeLineTimestamps.Add(tm, rslt.AddCount);
                    }

                    DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                    List<DateTime> keys = new List<DateTime>();
                    this._timeLineCount = 0;
                    foreach (DateTime key in this._timeLineTimestamps.Keys)
                    {
                        if (key.CompareTo(oneHour) < 0)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            this._timeLineCount += this._timeLineTimestamps[key];
                        }
                    }

                    foreach (DateTime key in keys)
                    {
                        this._timeLineTimestamps.Remove(key);
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
                    this._waitTimeline = false;
                    if (!this._isInitializing)
                    {
                        // 'API使用時の取得調整は別途考える（カウント調整？）
                    }

                    break;
                case WorkerType.Reply:
                    this._waitReply = false;
                    if (rslt.NewDM && !this._isInitializing)
                    {
                        this.GetTimeline(WorkerType.DirectMessegeRcv);
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
                            retry = MessageBox.Show(
                                string.Format("{0}   --->   [ {1} ]{2}\"{3}\"{2}{4}", R.StatusUpdateFailed1, rslt.RetMsg, Environment.NewLine, rslt.PStatus.Status, R.StatusUpdateFailed2),
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

                    if (rslt.RetMsg.Length == 0 && this._configs.PostAndGet)
                    {
                        if (this._isActiveUserstream)
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
                        this._postTimestamps.Add(DateTime.Now);
                        System.DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                        for (int i = this._postTimestamps.Count - 1; i >= 0; i--)
                        {
                            if (this._postTimestamps[i].CompareTo(oneHour) < 0)
                            {
                                this._postTimestamps.RemoveAt(i);
                            }
                        }

                        if (!this._isActiveUserstream && this._configs.PostAndGet)
                        {
                            this.GetTimeline(WorkerType.Timeline);
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
                    if (this._configs.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        this._pictureServices["Twitter"].Configuration("MaxUploadFilesize", this._configs.TwitterConfiguration.PhotoSizeLimit);
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
            this._itemCache = null;
            this._itemCacheIndex = -1;
            this._postCache = null;
            this._prevSelectedTab = e.TabPage;
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

            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, this._tabStringFormat);
        }

        private void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ListTab.SelectedTab != null)
            {
                if (this._statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
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
                    this._anchorFlag = false;
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
            if (!this._configs.TabMouseLock && e.Button == MouseButtons.Left && this._tabDraging)
            {
                string tn = string.Empty;
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(this._tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(this._tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
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
                this._tabDraging = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                Rectangle rect = this.ListTab.GetTabRect(i);
                if (rect.Contains(cpos))
                {
                    this._rclickTabName = this.ListTab.TabPages[i].Text;
                    break;
                }
            }
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            this._tabDraging = false;
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
            if (this._itemCache != null
                && e.StartIndex >= this._itemCacheIndex
                && e.EndIndex < this._itemCacheIndex + this._itemCache.Length
                && this._curList.Equals(sender))
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

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this._configs.SortOrderLock)
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
                SolidBrush brs1 = ((Control)sender).Focused ? this._brsHighLight : this._brsDeactiveSelection;
                e.Graphics.FillRectangle(brs1, e.Bounds);
            }
            else
            {
                var cl = e.Item.BackColor;
                SolidBrush brs2 = (cl == this._clrSelf) ? this._brsBackColorMine :
                    (cl == this._clrAtSelf) ? this._brsBackColorAt :
                    (cl == this._clrTarget) ? this._brsBackColorYou :
                    (cl == this._clrAtTarget) ? this._brsBackColorAtYou :
                    (cl == this._clrAtFromTarget) ? this._brsBackColorAtFromTarget :
                    (cl == this._clrAtTo) ? this._brsBackColorAtTo : this._brsBackColorNone;
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
                foreColor = ((Control)sender).Focused ? this._brsHighLightText.Color : this._brsForeColorUnread.Color;
            }
            else
            {
                // 選択されていない行 // 文字色
                var cl = e.Item.ForeColor;
                foreColor =
                    cl == this._clrUnread ? this._brsForeColorUnread.Color :
                    cl == this._clrRead ? this._brsForeColorReaded.Color :
                    cl == this._clrFav ? this._brsForeColorFav.Color :
                    cl == this._clrOwl ? this._brsForeColorOwl.Color :
                    cl == this._clrRetweet ? this._brsForeColorRetweet.Color : cl;
            }

            var multiLineFmt = TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            var singleLineFmt = TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            if (this._iconCol)
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
            this._anchorFlag = false;
        }

        private void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (this._configs.ListDoubleClickAction)
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

            if (this._configs.UnreadManage)
            {
                this._statuses.SetReadAllTab(true, this._curTab.Text, this._curItemIndex);
            }

            // キャッシュの書き換え
            this.ChangeCacheStyleRead(true, this._curItemIndex, this._curTab);

            // 既読へ（フォント、文字色）
            this.ColorizeList();
            this._colorize = true;
        }

        #endregion MyList events

        #region userstream

        private void Tw_NewPostFromStream()
        {
            if (this._configs.ReadOldPosts)
            {
                // 新着時未読クリア
                this._statuses.SetRead();
            }

            int rsltAddCount = this._statuses.DistributePosts();
            lock (this._syncObject)
            {
                DateTime tm = DateTime.Now;
                if (this._timeLineTimestamps.ContainsKey(tm))
                {
                    this._timeLineTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    this._timeLineTimestamps.Add(tm, rsltAddCount);
                }

                DateTime oneHour = tm.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                this._timeLineCount = 0;
                foreach (System.DateTime key in this._timeLineTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                    {
                        keys.Add(key);
                    }
                    else
                    {
                        this._timeLineCount += this._timeLineTimestamps[key];
                    }
                }

                foreach (DateTime key in keys)
                {
                    this._timeLineTimestamps.Remove(key);
                }

                keys.Clear();
            }

            if (this._configs.UserstreamPeriodInt > 0)
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
                        this._statuses.RemovePostReserve(id);
                        if (this._curTab != null && this._statuses.Tabs[this._curTab.Text].Contains(id))
                        {
                            this._itemCache = null;
                            this._itemCacheIndex = -1;
                            this._postCache = null;
                            ((DetailsListView)this._curTab.Tag).Update();
                            if (this._curPost != null && this._curPost.StatusId == id)
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
            this._modifySettingCommon = true;
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
                if (this._curTab != null && this._statuses.Tabs[this._curTab.Text].Contains(ev.Id))
                {
                    this._itemCache = null;
                    this._itemCacheIndex = -1;
                    this._postCache = null;
                    ((DetailsListView)this._curTab.Tag).Update();
                }

                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(this._tw.Username.ToLower()))
                {
                    this.RemovePostFromFavTab(new long[] { ev.Id });
                }
            }
        }

        private void Tw_UserStreamStarted()
        {
            this._isActiveUserstream = true;
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
            this._isActiveUserstream = false;
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