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
            ShowAboutBox();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTab();
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeAllrepliesSetting(AllrepliesToolStripMenuItem.Checked);
        }

        private void ApiInfoMenuItem_Click(object sender, EventArgs e)
        {
            ShowApiInfoBox();
        }

        private void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.Bitly);
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            ShowCacheInfoBox();
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            ClearTab(_rclickTabName, true);
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            SetupOperateContextMenu();
        }

        private void ContextMenuPostBrowser_Opening(object sender, CancelEventArgs e)
        {
            SetupPostBrowserContextMenu();
            e.Cancel = false;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            SetupPostModeContextMenu();
        }

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            SetupSourceContextMenu();
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            SetupTabPropertyContextMenu(fromMenuBar: false);
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            SetupUserPictureContextMenu();
        }

        private void CopySTOTMenuItem_Click(object sender, EventArgs e)
        {
            CopyStot();
        }

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
        {
            CopyIdUri();
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyUserId();
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchSelectedTextAtCurrentTab();
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, false);
        }

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedTab(fromMenuBar: false);
        }

        private void DeleteTbMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedTab(fromMenuBar: true);
        }

        private void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(_displayItem))
            {
                UserPicture.ReplaceImage(_displayItem.Image);
            }
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DispSelectedPost(true);
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            ShowEventViewerBox();
        }

        private void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSelectedFavStatus(true);
        }

        private void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSelectedFavStatus(false);
        }

        private void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            OpenFavorarePageOfSelectedTweetUser();
        }

        private void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            FavoritesRetweetOriginal();
        }

        private void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            FavoritesRetweetUnofficial();
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            ShowPostImageFileSelectBox();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            ShowFilterEditBox();
        }

        private void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            TryFollowUserOfCurrentTweet();
        }

        private void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            TryFollowUserOfCurrentLinkUser();
        }

        private void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryFollowUserOfCurrentIconUser();
        }

        private void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfAllUserInCurrentTweet();
        }

        private void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfCurrentLinkUser();
        }

        private void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfCurrentTweetUser();
        }

        private void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetFollowers();
        }

        private void GetTimelineWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DisplayTimelineWorkerProgressChanged(e.ProgressPercentage, (string)e.UserState);
        }

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            ShowHashManageBox();
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            ChangeUseHashTagSetting();
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            ChangeUseHashTagSetting();
        }

        private void HookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            ChangeWindowState();
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            AddIdFilteringRuleFromSelectedTweets();
        }

        private void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenCurrentTweetIconUrl();
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            AddIdFilteringRuleFromCurrentTweet();
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetModifySettingCommon(true);
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
        {
            CancelPostImageSelecting();
        }

        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            ToggleImageSelectorView();
        }

        private void ImageSelectionPanel_VisibleChanged(object sender, EventArgs e)
        {
            StatusText_TextChangedExtracted();
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                CancelPostImageSelecting();
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 0x1b)
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

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryChangeImageUploadService();
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
                LoadImageFromSelectedFile();
            }
        }

        private void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.Isgd);
        }

        private void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.Jmp);
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            TrySearchAndFocusUnreadTweet();
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ChangeListLockSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowListManageBox();
        }

        private void ListManageUserContextToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageMenuItem_Click(object sender, EventArgs e)
        {
            ShowListSelectFormForCurrentTweetUser();
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowListSelectForm(GetUserId());
        }

        private void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync(ApplicationHelpWebPageUrl);
        }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            SetupCommandMenu();
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            SetupEditMenu();
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            SetupHelpMenu();
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            SetupOperateMenu();
        }

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            TrySearchWordInTabToBottom();
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            TrySearchWordInTabToTop();
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            TrySearchWordInTab();
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            SetupTabPropertyContextMenu(true);
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            SetFocusToMainMenu();
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            SetFocusFromMainMenu();
        }

        private void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenCurListSelectedUserFavorites();
        }

        private void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenCurListSelectedUserHome();
        }

        private void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenSelectedRtUserHome();
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatusTextMultilineState(MultiLineMenuItem.Checked);
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ChangeNewPostPopupSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            ChangeNotifySetting(((ToolStripMenuItem)sender).Checked);
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            ActivateMainForm();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ActivateMainForm();
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            SetNotifyIconText();
        }

        private void OpenOwnFavedMenuItem_Click(object sender, EventArgs e)
        {
            OpenFavorarePageOfSelf();
        }

        private void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync("https://twitter.com/" + _tw.Username);
        }

        private void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenUrlInCurrentTweet();
        }

        private void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            OpenUserAppointUrl();
        }

        private void OwnStatusMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfUserSelf();
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ChangePlaySoundSetting(((ToolStripMenuItem)sender).Checked);
        }

        private void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            PostBrowser_NavigatedExtracted(e.Url);
        }

        private void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = NavigateNextUrl(e.Url);
        }

        private void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState modState = GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            bool res = CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, modState);
            if (res)
            {
                e.IsInputKey = true;
            }
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            ChangeStatusLabelUrlTextByPostBrowserStatusText();
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            TryPostTweet();
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            FocusCurrentPublicSearchTabSearchInput();
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
        {
            DoQuote();
        }

        private void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            DoReTweetOfficial(true);
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            DoReTweetUnofficial();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSelectedTweetReadStateToRead();
        }

        private void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            // もっと前を取得
            RefreshTab(more: true);
        }

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTab();
        }

        private void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            TryUnfollowCurrentTweetUser();
        }

        private void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            TryUnfollowUserInCurrentTweet();
        }

        private void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryUnfollowCurrentIconUser();
        }

        private void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenRepliedStatus();
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, true, true);
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, true);
        }

        private void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            ShowCurrentTweetRtCountBox();
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrySaveCurrentTweetUserIcon();
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            TrySaveLog();
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentTweetUserOriginalSizeIcon();
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSearchTabForAtUserOfCurrentTweet();
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSearchTabForAtUserInCurrentTweet();
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            ChangeSearchPanelControlsTabStop((Control)sender, true);
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            ChangeSearchPanelControlsTabStop((Control)sender, false);
        }

        private void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            SearchWebBySelectedWord(R.SearchItem2Url);
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTimelineTabForCurrentTweetUser();
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTimelineTabForUserInCurrentTweet();
        }

        private void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            SearchWebBySelectedWord(R.SearchItem4Url);
        }

        private void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            SearchWebBySelectedWord(R.SearchItem1Url);
        }

        private void SearchYatsContextMenuItem_Click(object sender, EventArgs e)
        {
            SearchWebBySelectedWord(R.SearchItem3Url);
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllItemInFocused();
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細ですべて選択
            PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            TryCopySelectionInPostBrowser();
        }

        private void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoTranslation(WebBrowser_GetSelectionText(PostBrowser));
        }

        private void TwitterApiInfo_Changed(object sender, ApiInformationChangedEventArgs e)
        {
            SetStatusLabelApiLuncher();
        }

        private void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            TryShowSettingsBox();
        }

        private void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            OpenUriAsync(ApplicationShortcutKeyHelpWebPageUrl);
        }

        private void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFriendshipOfCurrentIconUser();
        }

        private void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfCurrentTweetUser();
        }

        private void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            AddRelatedStatusesTab();
        }

        private void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            ShowtStatusOfCurrentLinkUser();
        }

        private void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatusOfCurrentIconUser();
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUserTimeline();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeCurrentTabSoundFile((string)((ToolStripComboBox)sender).SelectedItem);
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            TryCopySourceName();
        }

        private void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TryOpenSourceLink();
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            ChangeStatusLabelUrlText((string)SourceLinkLabel.Tag);
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            SetStatusLabelUrl();
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            TryCopySourceUrl();
        }

        private void SpaceKeyCanceler_SpaceCancel(object sender, EventArgs e)
        {
            TrySearchAndFocusUnreadTweet();
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis = SplitContainer1.SplitterDistance;
                if (StatusText.Multiline)
                {
                    _mySpDis2 = StatusText.Height;
                }

                _modifySettingLocal = true;
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ChangeStatusTextMultilineState(MultiLineMenuItem.Checked); // MultiLineMenuItem.PerformClick();
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            StatusText.Multiline = SplitContainer2.Panel2.Height > SplitContainer2.Panel2MinSize + 2;
            MultiLineMenuItem.Checked = StatusText.Multiline;
            _modifySettingLocal = true;
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (StatusText.Multiline)
            {
                _mySpDis2 = StatusText.Height;
            }

            _modifySettingLocal = true;
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis3 = SplitContainer3.SplitterDistance;
                _modifySettingLocal = true;
            }
        }

        private void SplitContainer4_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
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

        private void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myAdSpDis = SplitContainer4.SplitterDistance;
                _modifySettingLocal = true;
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenSelectedTweetWebPage();
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            StatusText_EnterExtracted();
        }

        private void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            ModifierState modState = GetModifierState(e.Control, e.Shift, e.Alt);
            if (modState == ModifierState.NotFlags)
            {
                return;
            }

            if (CommonKeyDown(e.KeyCode, FocusedControl.StatusText, modState))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            StatusText_TextChangedExtracted();
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;
            if (keyChar == '@' || keyChar == '#')
            {
                e.Handled = true;
                ShowSupplementBox(keyChar);
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
                        TrySearchAndFocusUnreadTweet();
                    }
                }
            }

            StatusText_TextChangedExtracted();
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            StatusText_LeaveExtracted();
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            ChangeStatusTextMultiline(StatusText.Multiline);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            StatusText_TextChangedExtracted();
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            TimelineRefreshEnableChange(!StopRefreshAllMenuItem.Checked);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeUserStreamStatus();
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                _isOsResumed = true;
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            AddFilteringRuleFromSelectedTweet();
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            RenameCurrentTabName();
        }

        private void Tabs_DoubleClick(object sender, MouseEventArgs e)
        {
            RenameSelectedTabName();
        }

        private void Tabs_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TabPage)))
            {
                return;
            }

            _tabDraging = false;
            string tn = string.Empty;
            bool bef = false;
            Point spos = ListTab.PointToClient(new Point(e.X, e.Y));
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Contains(spos))
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
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn)
            {
                return;
            }

            ReorderTab(tp.Text, tn, bef);
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
            if (_configs.TabMouseLock)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                _tabDraging = false;
            }
            else
            {
                Point cpos = e.Location;
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (ListTab.GetTabRect(i).Contains(cpos))
                    {
                        _tabDraging = true;
                        _tabMouseDownPoint = cpos;
                        break;
                    }
                }
            }
        }

        private void Tabs_MouseDown(object sender, MouseEventArgs e)
        {
            Tabs_MouseDownExtracted(e);
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e)
        {
            if (!_timerTimeline.Enabled)
            {
                return;
            }

            _resetTimers = e;
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            // 200ms
            RefreshTasktrayIcon(false);
        }

        private void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            TimerTimeline_ElapsedExtracted();
        }

        private void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.TinyUrl);
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SetModifySettingCommon(true);
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            ChangeAutoUrlConvertFlag(ToolStripMenuItemUrlAutoShorten.Checked);
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTraceFlag(TraceOutToolStripMenuItem.Checked);
        }

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTrackWordStatus();
        }

        private void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslateCurrentTweet();
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            ActivateMainFormControls();
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            TweenMain_ClientSizeChangedExtracted();
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            StatusText_LeaveExtracted();
        }

        private void TweenMain_Disposed(object sender, EventArgs e)
        {
            DisposeAll();
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
                LoadImageFromSelectedFile();
                Activate();
                BringToFront();
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

                if (!string.IsNullOrEmpty(ImageService) && _pictureServices[ImageService].CheckValidFilesize(ext, fl.Length))
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

                    if (_pictureServices[svc].CheckValidFilesize(ext, fl.Length))
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

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            TweenMain_FormClosingExtracted(e);
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            TweenMain_LoadExtracted();
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            TweenMain_LocationChangedExtracted();
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            ResizeMainForm();
        }

        private void TweenMain_Shown(object sender, EventArgs e)
        {
            TweenMain_ShownExtracted();
        }

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            TryRestartApplication();
        }

        private void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.Twurl);
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            UndoRemoveTab();
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSelectedTweetReadSateToUnread();
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCurrentTabUnreadManagement(((ToolStripMenuItem)sender).Checked);
        }

        private void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrlByAutoSelectedService();
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            TryCopyUrlInCurrentTweet();
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoUrlShortening();
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            TrySetHashtagFromCurrentTweet();
        }

        private void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryOpenFavorarePageOfCurrentTweetUser();
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            TryOpenCurrentNameLabelUserHome();
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            ChangeUserPictureCursor(Cursors.Hand);
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            ChangeUserPictureCursor(Cursors.Default);
        }

        private void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryShowStatusOfCurrentTweetUser();
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTabForUserTimeline(GetUserIdFromCurPostOrInput("Show UserTimeline"));
        }

        private void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            ConvertUrl(UrlConverter.Uxnu);
        }

        private void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            CheckNewVersion();
        }

        #endregion cleanuped

        private void SearchButton_ClickExtracted(Control pnl)
        {
            string tabName = pnl.Parent.Text;
            TabClass tb = _statuses.Tabs[tabName];
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
                _statuses.ClearTabIds(tabName);
                SaveConfigsTabs(); // 検索条件の保存
            }

            GetTimeline(WorkerType.PublicSearch, 1, 0, tabName);
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null)
            {
                return;
            }

            SearchButton_ClickExtracted(pnl);
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

        private void TweenMain_LoadExtracted()
        {
            _ignoreConfigSave = true;
            Visible = false;
            VerUpMenuItem.Image = _shield.Icon;
            _spaceKeyCanceler = new SpaceKeyCanceler(PostButton);
            _spaceKeyCanceler.SpaceCancel += SpaceKeyCanceler_SpaceCancel;

            Regex.CacheSize = 100;

            InitializeTraceFrag();

            _statuses = TabInformations.Instance; // 発言保持クラス

            // アイコン設定
            LoadIcons();                        // アイコン読み込み
            Icon = _mainIcon;               // メインフォーム（TweenMain）
            NotifyIcon1.Icon = _iconAt;     // タスクトレイ
            TabImage.Images.Add(_tabIcon);  // タブ見出し

            _settingDialog.Owner = this;
            _searchDialog.Owner = this;
            _fltDialog.Owner = this;
            _tabDialog.Owner = this;
            _urlDialog.Owner = this;

            _postHistory.Add(new PostingStatus());
            _postHistoryIndex = 0;

            ClearReplyToInfo();

            // <<<<<<<<<設定関連>>>>>>>>>
            // '設定読み出し
            LoadConfig();

            // 新着バルーン通知のチェック状態設定
            NewPostPopMenuItem.Checked = _cfgCommon.NewAllPop;
            NotifyFileMenuItem.Checked = _cfgCommon.NewAllPop;

            // フォント＆文字色＆背景色保持
            _fntUnread = _cfgLocal.FontUnread;
            _clrUnread = _cfgLocal.ColorUnread;
            _fntReaded = _cfgLocal.FontRead;
            _clrRead = _cfgLocal.ColorRead;
            _clrFav = _cfgLocal.ColorFav;
            _clrOwl = _cfgLocal.ColorOWL;
            _clrRetweet = _cfgLocal.ColorRetweet;
            _fntDetail = _cfgLocal.FontDetail;
            _clrDetail = _cfgLocal.ColorDetail;
            _clrDetailLink = _cfgLocal.ColorDetailLink;
            _clrDetailBackcolor = _cfgLocal.ColorDetailBackcolor;
            _clrSelf = _cfgLocal.ColorSelf;
            _clrAtSelf = _cfgLocal.ColorAtSelf;
            _clrTarget = _cfgLocal.ColorTarget;
            _clrAtTarget = _cfgLocal.ColorAtTarget;
            _clrAtFromTarget = _cfgLocal.ColorAtFromTarget;
            _clrAtTo = _cfgLocal.ColorAtTo;
            _clrListBackcolor = _cfgLocal.ColorListBackcolor;
            InputBackColor = _cfgLocal.ColorInputBackcolor;
            _clrInputForecolor = _cfgLocal.ColorInputFont;
            _fntInputFont = _cfgLocal.FontInputFont;

            InitUserBrushes();

            // StringFormatオブジェクトへの事前設定
            _tabStringFormat.Alignment = StringAlignment.Center;
            _tabStringFormat.LineAlignment = StringAlignment.Center;

            // 設定画面への反映
            HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);
            _configs.TwitterApiUrl = _cfgCommon.TwitterUrl;
            _configs.TwitterSearchApiUrl = _cfgCommon.TwitterSearchUrl;

            // 認証関連
            if (string.IsNullOrEmpty(_cfgCommon.Token))
            {
                _cfgCommon.UserName = string.Empty;
            }

            _tw.Initialize(_cfgCommon.Token, _cfgCommon.TokenSecret, _cfgCommon.UserName, _cfgCommon.UserId);

            _configs.UserAccounts = _cfgCommon.UserAccounts;
            _configs.TimelinePeriodInt = _cfgCommon.TimelinePeriod;
            _configs.ReplyPeriodInt = _cfgCommon.ReplyPeriod;
            _configs.DMPeriodInt = _cfgCommon.DMPeriod;
            _configs.PubSearchPeriodInt = _cfgCommon.PubSearchPeriod;
            _configs.UserTimelinePeriodInt = _cfgCommon.UserTimelinePeriod;
            _configs.ListsPeriodInt = _cfgCommon.ListsPeriod;

            // 不正値チェック
            if (!MyCommon.NoLimit)
            {
                if (_configs.TimelinePeriodInt < 15 && _configs.TimelinePeriodInt > 0)
                {
                    _configs.TimelinePeriodInt = 15;
                }

                if (_configs.ReplyPeriodInt < 15 && _configs.ReplyPeriodInt > 0)
                {
                    _configs.ReplyPeriodInt = 15;
                }

                if (_configs.DMPeriodInt < 15 && _configs.DMPeriodInt > 0)
                {
                    _configs.DMPeriodInt = 15;
                }

                if (_configs.PubSearchPeriodInt < 30 && _configs.PubSearchPeriodInt > 0)
                {
                    _configs.PubSearchPeriodInt = 30;
                }

                if (_configs.UserTimelinePeriodInt < 15 && _configs.UserTimelinePeriodInt > 0)
                {
                    _configs.UserTimelinePeriodInt = 15;
                }

                if (_configs.ListsPeriodInt < 15 && _configs.ListsPeriodInt > 0)
                {
                    _configs.ListsPeriodInt = 15;
                }
            }

            // 起動時読み込み分を既読にするか。Trueなら既読として処理
            _configs.Readed = _cfgCommon.Read;

            // 新着取得時のリストスクロールをするか。Trueならスクロールしない
            ListLockMenuItem.Checked = _cfgCommon.ListLock;
            LockListFileMenuItem.Checked = _cfgCommon.ListLock;
            _configs.IconSz = _cfgCommon.IconSize;

            // 文末ステータス
            _configs.Status = _cfgLocal.StatusText;

            // 未読管理。Trueなら未読管理する
            _configs.UnreadManage = _cfgCommon.UnreadManage;

            // サウンド再生（タブ別設定より優先）
            _configs.PlaySound = _cfgCommon.PlaySound;
            PlaySoundMenuItem.Checked = _configs.PlaySound;
            PlaySoundFileMenuItem.Checked = _configs.PlaySound;

            // 片思い表示。Trueなら片思い表示する
            _configs.OneWayLove = _cfgCommon.OneWayLove;

            // フォント＆文字色＆背景色
            _configs.FontUnread = _fntUnread;
            _configs.ColorUnread = _clrUnread;
            _configs.FontReaded = _fntReaded;
            _configs.ColorReaded = _clrRead;
            _configs.ColorFav = _clrFav;
            _configs.ColorOWL = _clrOwl;
            _configs.ColorRetweet = _clrRetweet;
            _configs.FontDetail = _fntDetail;
            _configs.ColorDetail = _clrDetail;
            _configs.ColorDetailLink = _clrDetailLink;
            _configs.ColorDetailBackcolor = _clrDetailBackcolor;
            _configs.ColorSelf = _clrSelf;
            _configs.ColorAtSelf = _clrAtSelf;
            _configs.ColorTarget = _clrTarget;
            _configs.ColorAtTarget = _clrAtTarget;
            _configs.ColorAtFromTarget = _clrAtFromTarget;
            _configs.ColorAtTo = _clrAtTo;
            _configs.ColorListBackcolor = _clrListBackcolor;
            _configs.ColorInputBackcolor = InputBackColor;
            _configs.ColorInputFont = _clrInputForecolor;
            _configs.FontInputFont = _fntInputFont;
            _configs.NameBalloon = _cfgCommon.NameBalloon;
            _configs.PostCtrlEnter = _cfgCommon.PostCtrlEnter;
            _configs.PostShiftEnter = _cfgCommon.PostShiftEnter;
            _configs.CountApi = _cfgCommon.CountApi;
            _configs.CountApiReply = _cfgCommon.CountApiReply;
            if (_configs.CountApi < 20 || _configs.CountApi > 200)
            {
                _configs.CountApi = 60;
            }

            if (_configs.CountApiReply < 20 || _configs.CountApiReply > 200)
            {
                _configs.CountApiReply = 40;
            }

            _configs.BrowserPath = _cfgLocal.BrowserPath;
            _configs.PostAndGet = _cfgCommon.PostAndGet;
            _configs.UseRecommendStatus = _cfgLocal.UseRecommendStatus;
            _configs.DispUsername = _cfgCommon.DispUsername;
            _configs.CloseToExit = _cfgCommon.CloseToExit;
            _configs.MinimizeToTray = _cfgCommon.MinimizeToTray;
            _configs.DispLatestPost = _cfgCommon.DispLatestPost;
            _configs.SortOrderLock = _cfgCommon.SortOrderLock;
            _configs.TinyUrlResolve = _cfgCommon.TinyUrlResolve;
            _configs.ShortUrlForceResolve = _cfgCommon.ShortUrlForceResolve;
            _configs.SelectedProxyType = _cfgLocal.ProxyType;
            _configs.ProxyAddress = _cfgLocal.ProxyAddress;
            _configs.ProxyPort = _cfgLocal.ProxyPort;
            _configs.ProxyUser = _cfgLocal.ProxyUser;
            _configs.ProxyPassword = _cfgLocal.ProxyPassword;
            _configs.PeriodAdjust = _cfgCommon.PeriodAdjust;
            _configs.StartupVersion = _cfgCommon.StartupVersion;
            _configs.StartupFollowers = _cfgCommon.StartupFollowers;
            _configs.RestrictFavCheck = _cfgCommon.RestrictFavCheck;
            _configs.AlwaysTop = _cfgCommon.AlwaysTop;
            _configs.UrlConvertAuto = false;
            _configs.OutputzEnabled = _cfgCommon.Outputz;
            _configs.OutputzKey = _cfgCommon.OutputzKey;
            _configs.OutputzUrlmode = _cfgCommon.OutputzUrlMode;
            _configs.UseUnreadStyle = _cfgCommon.UseUnreadStyle;
            _configs.DefaultTimeOut = _cfgCommon.DefaultTimeOut;
            _configs.RetweetNoConfirm = _cfgCommon.RetweetNoConfirm;
            _configs.PlaySound = _cfgCommon.PlaySound;
            _configs.DateTimeFormat = _cfgCommon.DateTimeFormat;
            _configs.LimitBalloon = _cfgCommon.LimitBalloon;
            _configs.EventNotifyEnabled = _cfgCommon.EventNotifyEnabled;
            _configs.EventNotifyFlag = _cfgCommon.EventNotifyFlag;
            _configs.IsMyEventNotifyFlag = _cfgCommon.IsMyEventNotifyFlag;
            _configs.ForceEventNotify = _cfgCommon.ForceEventNotify;
            _configs.FavEventUnread = _cfgCommon.FavEventUnread;
            _configs.TranslateLanguage = _cfgCommon.TranslateLanguage;
            _configs.EventSoundFile = _cfgCommon.EventSoundFile;

            // 廃止サービスが選択されていた場合bit.lyへ読み替え
            if (_cfgCommon.AutoShortUrlFirst < 0)
            {
                _cfgCommon.AutoShortUrlFirst = UrlConverter.Bitly;
            }

            _configs.AutoShortUrlFirst = _cfgCommon.AutoShortUrlFirst;
            _configs.TabIconDisp = _cfgCommon.TabIconDisp;
            _configs.ReplyIconState = _cfgCommon.ReplyIconState;
            _configs.ReadOwnPost = _cfgCommon.ReadOwnPost;
            _configs.GetFav = _cfgCommon.GetFav;
            _configs.ReadOldPosts = _cfgCommon.ReadOldPosts;
            _configs.UseSsl = _cfgCommon.UseSsl;
            _configs.BitlyUser = _cfgCommon.BilyUser;
            _configs.BitlyPwd = _cfgCommon.BitlyPwd;
            _configs.ShowGrid = _cfgCommon.ShowGrid;
            _configs.Language = _cfgCommon.Language;
            _configs.UseAtIdSupplement = _cfgCommon.UseAtIdSupplement;
            _configs.UseHashSupplement = _cfgCommon.UseHashSupplement;
            _configs.PreviewEnable = _cfgCommon.PreviewEnable;
            AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            _configs.IsMonospace = _cfgCommon.IsMonospace;
            _detailHtmlFormatFooter = GetDetailHtmlFormatFooter(_configs.IsMonospace);
            _detailHtmlFormatHeader = GetDetailHtmlFormatHeader(_configs.IsMonospace);

            IdeographicSpaceToSpaceToolStripMenuItem.Checked = _cfgCommon.WideSpaceConvert;
            ToolStripFocusLockMenuItem.Checked = _cfgCommon.FocusLockToStatusText;

            _configs.RecommendStatusText = " [HH2v" + Regex.Replace(MyCommon.FileVersion.Replace(".", string.Empty), "^0*", string.Empty) + "]";

            // 書式指定文字列エラーチェック
            try
            {
                if (DateTime.Now.ToString(_configs.DateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    _configs.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                _configs.DateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }

            _configs.Nicoms = _cfgCommon.Nicoms;
            _configs.HotkeyEnabled = _cfgCommon.HotkeyEnabled;
            _configs.HotkeyMod = _cfgCommon.HotkeyModifier;
            _configs.HotkeyKey = _cfgCommon.HotkeyKey;
            _configs.HotkeyValue = _cfgCommon.HotkeyValue;
            _configs.BlinkNewMentions = _cfgCommon.BlinkNewMentions;
            _configs.UseAdditionalCount = _cfgCommon.UseAdditionalCount;
            _configs.MoreCountApi = _cfgCommon.MoreCountApi;
            _configs.FirstCountApi = _cfgCommon.FirstCountApi;
            _configs.SearchCountApi = _cfgCommon.SearchCountApi;
            _configs.FavoritesCountApi = _cfgCommon.FavoritesCountApi;
            _configs.UserTimelineCountApi = _cfgCommon.UserTimelineCountApi;
            _configs.ListCountApi = _cfgCommon.ListCountApi;
            _configs.UserstreamStartup = _cfgCommon.UserstreamStartup;
            _configs.UserstreamPeriodInt = _cfgCommon.UserstreamPeriod;
            _configs.OpenUserTimeline = _cfgCommon.OpenUserTimeline;
            _configs.ListDoubleClickAction = _cfgCommon.ListDoubleClickAction;
            _configs.UserAppointUrl = _cfgCommon.UserAppointUrl;
            _configs.HideDuplicatedRetweets = _cfgCommon.HideDuplicatedRetweets;
            _configs.IsPreviewFoursquare = _cfgCommon.IsPreviewFoursquare;
            _configs.FoursquarePreviewHeight = _cfgCommon.FoursquarePreviewHeight;
            _configs.FoursquarePreviewWidth = _cfgCommon.FoursquarePreviewWidth;
            _configs.FoursquarePreviewZoom = _cfgCommon.FoursquarePreviewZoom;
            _configs.IsListStatusesIncludeRts = _cfgCommon.IsListsIncludeRts;
            _configs.TabMouseLock = _cfgCommon.TabMouseLock;
            _configs.IsRemoveSameEvent = _cfgCommon.IsRemoveSameEvent;
            _configs.IsNotifyUseGrowl = _cfgCommon.IsUseNotifyGrowl;

            // ハッシュタグ関連
            HashSupl = new AtIdSupplement(_cfgCommon.HashTags, "#");
            HashMgr = new HashtagManage(HashSupl, _cfgCommon.HashTags.ToArray(), _cfgCommon.HashSelected, _cfgCommon.HashIsPermanent, _cfgCommon.HashIsHead, _cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(HashMgr.UseHash) && HashMgr.IsPermanent)
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
            }

            _isInitializing = true;

            // アイコンリスト作成
            try
            {
                _iconDict = new ImageDictionary(5);
            }
            catch (Exception)
            {
                MessageBox.Show("Please install [.NET Framework 4 (Full)].");
                Application.Exit();
                return;
            }

            _iconDict.PauseGetImage = false;

            bool saveRequired = false;

            // ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(_tw.Username))
            {
                saveRequired = true;

                // 設定せずにキャンセルされた場合はプログラム終了
                if (_settingDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // 設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
                if (string.IsNullOrEmpty(_tw.Username))
                {
                    // 強制終了
                    Application.Exit();
                    return;
                }

                // フォント＆文字色＆背景色保持
                _fntUnread = _configs.FontUnread;
                _clrUnread = _configs.ColorUnread;
                _fntReaded = _configs.FontReaded;
                _clrRead = _configs.ColorReaded;
                _clrFav = _configs.ColorFav;
                _clrOwl = _configs.ColorOWL;
                _clrRetweet = _configs.ColorRetweet;
                _fntDetail = _configs.FontDetail;
                _clrDetail = _configs.ColorDetail;
                _clrDetailLink = _configs.ColorDetailLink;
                _clrDetailBackcolor = _configs.ColorDetailBackcolor;
                _clrSelf = _configs.ColorSelf;
                _clrAtSelf = _configs.ColorAtSelf;
                _clrTarget = _configs.ColorTarget;
                _clrAtTarget = _configs.ColorAtTarget;
                _clrAtFromTarget = _configs.ColorAtFromTarget;
                _clrAtTo = _configs.ColorAtTo;
                _clrListBackcolor = _configs.ColorListBackcolor;
                InputBackColor = _configs.ColorInputBackcolor;
                _clrInputForecolor = _configs.ColorInputFont;
                _fntInputFont = _configs.FontInputFont;
                DisposeUserBrushes();
                InitUserBrushes();
                _detailHtmlFormatFooter = GetDetailHtmlFormatFooter(_configs.IsMonospace);
                _detailHtmlFormatHeader = GetDetailHtmlFormatHeader(_configs.IsMonospace);
            }

            if (_configs.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((_configs.HotkeyMod & Keys.Alt) == Keys.Alt)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Alt;
                }

                if ((_configs.HotkeyMod & Keys.Control) == Keys.Control)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Ctrl;
                }

                if ((_configs.HotkeyMod & Keys.Shift) == Keys.Shift)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Shift;
                }

                if ((_configs.HotkeyMod & Keys.LWin) == Keys.LWin)
                {
                    modKey = modKey | HookGlobalHotkey.ModKeys.Win;
                }

                _hookGlobalHotkey.RegisterOriginalHotkey(_configs.HotkeyKey, _configs.HotkeyValue, modKey);
            }

            // Twitter用通信クラス初期化
            HttpConnection.InitializeConnection(_configs.DefaultTimeOut, _configs.SelectedProxyType, _configs.ProxyAddress, _configs.ProxyPort, _configs.ProxyUser, _configs.ProxyPassword);

            _tw.SetRestrictFavCheck(_configs.RestrictFavCheck);
            _tw.ReadOwnPost = _configs.ReadOwnPost;
            _tw.SetUseSsl(_configs.UseSsl);
            ShortUrl.IsResolve = _configs.TinyUrlResolve;
            ShortUrl.IsForceResolve = _configs.ShortUrlForceResolve;
            ShortUrl.SetBitlyId(_configs.BitlyUser);
            ShortUrl.SetBitlyKey(_configs.BitlyPwd);
            HttpTwitter.SetTwitterUrl(_cfgCommon.TwitterUrl);
            HttpTwitter.SetTwitterSearchUrl(_cfgCommon.TwitterSearchUrl);
            _tw.TrackWord = _cfgCommon.TrackWord;
            TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(_tw.TrackWord);
            _tw.AllAtReply = _cfgCommon.AllAtReply;
            AllrepliesToolStripMenuItem.Checked = _tw.AllAtReply;

            Outputz.Key = _configs.OutputzKey;
            Outputz.Enabled = _configs.OutputzEnabled;
            switch (_configs.OutputzUrlmode)
            {
                case OutputzUrlmode.twittercom:
                    Outputz.OutUrl = "http://twitter.com/";
                    break;
                case OutputzUrlmode.twittercomWithUsername:
                    Outputz.OutUrl = "http://twitter.com/" + _tw.Username;
                    break;
            }

            // 画像投稿サービス
            CreatePictureServices();
            SetImageServiceCombo();
            ImageSelectionPanel.Enabled = false;
            ImageServiceCombo.SelectedIndex = _cfgCommon.UseImageService;

            // ウィンドウ設定
            ClientSize = _cfgLocal.FormSize;
            _mySize = _cfgLocal.FormSize;          // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            _myLoc = _cfgLocal.FormLocation;       // タイトルバー領域
            if (WindowState != FormWindowState.Minimized)
            {
                DesktopLocation = _cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(Location, new Size(_mySize.Width, SystemInformation.CaptionHeight));
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
                        DesktopLocation = new Point(0, 0);
                        _myLoc = DesktopLocation;
                    }
                }
            }

            TopMost = _configs.AlwaysTop;
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
            PlaySoundMenuItem.Checked = _configs.PlaySound;
            PlaySoundFileMenuItem.Checked = _configs.PlaySound;

            // 入力欄
            StatusText.Font = _fntInputFont;
            StatusText.ForeColor = _clrInputForecolor;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (!_configs.UnreadManage)
            {
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }

            if (_configs.IsNotifyUseGrowl)
            {
                _growlHelper.RegisterGrowl();
            }

            // タイマー設定
            _timerTimeline.AutoReset = true;
            _timerTimeline.SynchronizingObject = this;

            // Recent取得間隔
            _timerTimeline.Interval = 1000;
            _timerTimeline.Enabled = true;

            // 更新中アイコンアニメーション間隔
            TimerRefreshIcon.Interval = 200;
            TimerRefreshIcon.Enabled = true;

            // 状態表示部の初期化（画面右下）
            StatusLabel.Text = string.Empty;
            StatusLabel.AutoToolTip = false;
            StatusLabel.ToolTipText = string.Empty;

            // 文字カウンタ初期化
            lblLen.Text = GetRestStatusCount(true, false).ToString();

            _statuses.SortOrder = (SortOrder)_cfgCommon.SortOrder;
            IdComparerClass.ComparerMode mode = default(IdComparerClass.ComparerMode);
            switch (_cfgCommon.SortColumn)
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

            _statuses.SortMode = mode;

            switch (_configs.IconSz)
            {
                case IconSizes.IconNone:
                    _iconSz = 0;
                    break;
                case IconSizes.Icon16:
                    _iconSz = 16;
                    break;
                case IconSizes.Icon24:
                    _iconSz = 26;
                    break;
                case IconSizes.Icon48:
                    _iconSz = 48;
                    break;
                case IconSizes.Icon48_2:
                    _iconSz = 48;
                    _iconCol = true;
                    break;
            }

            if (_iconSz == 0)
            {
                _tw.SetGetIcon(false);
            }
            else
            {
                _tw.SetGetIcon(true);
                _tw.SetIconSize(_iconSz);
            }

            _tw.SetTinyUrlResolve(_configs.TinyUrlResolve);
            ShortUrl.IsForceResolve = _configs.ShortUrlForceResolve;

            _tw.DetailIcon = _iconDict;
            StatusLabel.Text = R.Form1_LoadText1;  // 画面右下の状態表示を変更
            StatusLabelUrl.Text = string.Empty;  // 画面左下のリンク先URL表示部を初期化
            NameLabel.Text = string.Empty;       // 発言詳細部名前ラベル初期化
            DateTimeLabel.Text = string.Empty;   // 発言詳細部日時ラベル初期化
            SourceLinkLabel.Text = string.Empty; // Source部分初期化

            // <<<<<<<<タブ関連>>>>>>>
            // デフォルトタブの存在チェック、ない場合には追加
            if (_statuses.GetTabByType(TabUsageType.Home) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.RECENT))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.RECENT, TabUsageType.Home, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.RECENT].TabType = TabUsageType.Home;
                }
            }

            if (_statuses.GetTabByType(TabUsageType.Mentions) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.REPLY))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.REPLY, TabUsageType.Mentions, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.REPLY].TabType = TabUsageType.Mentions;
                }
            }

            if (_statuses.GetTabByType(TabUsageType.DirectMessage) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.DM))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.DM, TabUsageType.DirectMessage, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.DM].TabType = TabUsageType.DirectMessage;
                }
            }

            if (_statuses.GetTabByType(TabUsageType.Favorites) == null)
            {
                if (!_statuses.Tabs.ContainsKey(Hoehoe.MyCommon.DEFAULTTAB.FAV))
                {
                    _statuses.AddTab(Hoehoe.MyCommon.DEFAULTTAB.FAV, TabUsageType.Favorites, null);
                }
                else
                {
                    _statuses.Tabs[Hoehoe.MyCommon.DEFAULTTAB.FAV].TabType = TabUsageType.Favorites;
                }
            }

            foreach (string tn in _statuses.Tabs.Keys)
            {
                if (_statuses.Tabs[tn].TabType == TabUsageType.Undefined)
                {
                    _statuses.Tabs[tn].TabType = TabUsageType.UserDefined;
                }

                if (!AddNewTab(tn, true, _statuses.Tabs[tn].TabType, _statuses.Tabs[tn].ListInfo))
                {
                    throw new Exception(R.TweenMain_LoadText1);
                }
            }

            JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (!_configs.MinimizeToTray || WindowState != FormWindowState.Minimized)
            {
                Visible = true;
            }

            _curTab = ListTab.SelectedTab;
            _curItemIndex = -1;
            _curList = (DetailsListView)_curTab.Tag;
            SetMainWindowTitle();
            SetNotifyIconText();

            if (_configs.TabIconDisp)
            {
                ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                ListTab.DrawItem += ListTab_DrawItem;
                ListTab.ImageList = null;
            }

            SplitContainer4.Panel2Collapsed = true;
            _ignoreConfigSave = false;
            ResizeMainForm();
            if (saveRequired)
            {
                SaveConfigsAll(false);
            }

            if (_tw.UserId == 0)
            {
                _tw.VerifyCredentials();
                foreach (var ua in _cfgCommon.UserAccounts)
                {
                    if (ua.Username.ToLower() == _tw.Username.ToLower())
                    {
                        ua.UserId = _tw.UserId;
                        break;
                    }
                }
            }

            foreach (var ua in _configs.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == _tw.Username.ToLower())
                {
                    ua.UserId = _tw.UserId;
                    break;
                }
            }
        }

        #region callback

        private void GetApiInfo_Dowork(object sender, DoWorkEventArgs e)
        {
            GetApiInfoArgs args = (GetApiInfoArgs)e.Argument;
            e.Result = _tw.GetInfoApi(args.Info);
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
            _tw.GetStatusRetweetedCount(CurPost.OriginalStatusId, ref counter);
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
            bool read = !_configs.UnreadManage;
            if (_isInitializing && _configs.UnreadManage)
            {
                read = _configs.Readed;
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
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = _tw.GetTimelineApi(read, args.WorkerType, args.Page == -1, _isInitializing);
                    if (string.IsNullOrEmpty(ret) && args.WorkerType == WorkerType.Timeline && _configs.ReadOldPosts)
                    {
                        // 新着時未読クリア
                        _statuses.SetRead();
                    }

                    rslt.AddCount = _statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.DirectMessegeRcv:
                    // 送信分もまとめて取得
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = _tw.GetDirectMessageApi(read, WorkerType.DirectMessegeRcv, args.Page == -1);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = _tw.GetDirectMessageApi(read, WorkerType.DirectMessegeSnt, args.Page == -1);
                    }

                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case WorkerType.FavAdd:
                    // スレッド処理はしない
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
                                ret = _tw.PostFavAdd(post.OriginalStatusId);
                                if (ret.Length == 0)
                                {
                                    // リスト再描画必要
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = true;
                                    _favTimestamps.Add(DateTime.Now);
                                    if (string.IsNullOrEmpty(post.RelTabName))
                                    {
                                        // 検索,リストUserTimeline.Relatedタブからのfavは、favタブへ追加せず。それ以外は追加
                                        _statuses.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                    }
                                    else
                                    {
                                        // 検索,リスト,UserTimeline.Relatedタブからのfavで、TLでも取得済みならfav反映
                                        if (_statuses.ContainsKey(post.StatusId))
                                        {
                                            PostClass postTl = _statuses.Item(post.StatusId);
                                            postTl.IsFav = true;
                                            _statuses.GetTabByType(TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, false);
                                        }
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
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
                                ret = _tw.PostFavRemove(post.OriginalStatusId);
                                if (ret.Length == 0)
                                {
                                    args.SIds.Add(post.StatusId);
                                    post.IsFav = false;

                                    // リスト再描画必要
                                    if (_statuses.ContainsKey(post.StatusId))
                                    {
                                        _statuses.Item(post.StatusId).IsFav = false;
                                    }

                                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                                    foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.PublicSearch | TabUsageType.Lists | TabUsageType.UserTimeline | TabUsageType.Related))
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
                            ret = _tw.PostStatus(args.PStatus.Status, args.PStatus.InReplyToId);
                            if (string.IsNullOrEmpty(ret) || ret.StartsWith("OK:") || ret.StartsWith("Outputz:") || ret.StartsWith("Warn:") || ret == "Err:Status is a duplicate." || args.PStatus.Status.StartsWith("D", StringComparison.OrdinalIgnoreCase) || args.PStatus.Status.StartsWith("DM", StringComparison.OrdinalIgnoreCase) || Twitter.AccountState != AccountState.Valid)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        ret = _pictureServices[args.PStatus.ImageService].Upload(ref args.PStatus.ImagePath, ref args.PStatus.Status, args.PStatus.InReplyToId);
                    }

                    bw.ReportProgress(300);
                    rslt.PStatus = args.PStatus;
                    break;
                case WorkerType.Retweet:
                    bw.ReportProgress(200);
                    for (int i = 0; i < args.Ids.Count; i++)
                    {
                        ret = _tw.PostRetweet(args.Ids[i], read);
                    }

                    bw.ReportProgress(300);
                    break;
                case WorkerType.Follower:
                    bw.ReportProgress(50, R.UpdateFollowersMenuItem1_ClickText1);
                    ret = _tw.GetFollowersApi();
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = _tw.GetNoRetweetIdsApi();
                    }

                    break;
                case WorkerType.Configuration:
                    ret = _tw.ConfigurationApi();
                    break;
                case WorkerType.OpenUri:
                    string myPath = args.Url;
                    string browserPath = _configs.BrowserPath;
                    MyCommon.TryOpenUrl(myPath, browserPath);
                    break;
                case WorkerType.Favorites:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = _tw.GetFavoritesApi(read, args.WorkerType, args.Page == -1);
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case WorkerType.PublicSearch:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.PublicSearch))
                        {
                            if (!string.IsNullOrEmpty(tb.SearchWords))
                            {
                                ret = _tw.GetSearch(read, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = _tw.GetSearch(read, tb, false);
                            if (string.IsNullOrEmpty(ret) && args.Page == -1)
                            {
                                ret = _tw.GetSearch(read, tb, true);
                            }
                        }
                    }

                    rslt.AddCount = _statuses.DistributePosts();                    // 振り分け
                    break;
                case WorkerType.UserTimeline:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    int count = 20;
                    if (_configs.UseAdditionalCount)
                    {
                        count = _configs.UserTimelineCountApi;
                    }

                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.UserTimeline))
                        {
                            if (!string.IsNullOrEmpty(tb.User))
                            {
                                ret = _tw.GetUserTimelineApi(read, count, tb.User, tb, false);
                            }
                        }
                    }
                    else
                    {
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = _tw.GetUserTimelineApi(read, count, tb.User, tb, args.Page == -1);
                        }
                    }

                    rslt.AddCount = _statuses.DistributePosts();                     // 振り分け
                    break;
                case WorkerType.List:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    if (string.IsNullOrEmpty(args.TabName))
                    {
                        // 定期更新
                        foreach (TabClass tb in _statuses.GetTabsByType(TabUsageType.Lists))
                        {
                            if (tb.ListInfo != null && tb.ListInfo.Id != 0)
                            {
                                ret = _tw.GetListStatus(read, tb, false, _isInitializing);
                            }
                        }
                    }
                    else
                    {
                        // 手動更新（特定タブのみ更新）
                        TabClass tb = _statuses.GetTabByName(args.TabName);
                        if (tb != null)
                        {
                            ret = _tw.GetListStatus(read, tb, args.Page == -1, _isInitializing);
                        }
                    }

                    rslt.AddCount = _statuses.DistributePosts(); // 振り分け
                    break;
                case WorkerType.Related:
                    bw.ReportProgress(50, MakeStatusMessage(args, false));
                    ret = _tw.GetRelatedResult(read, _statuses.GetTabByName(args.TabName));
                    rslt.AddCount = _statuses.DistributePosts();
                    break;
                case WorkerType.BlockIds:
                    bw.ReportProgress(50, R.UpdateBlockUserText1);
                    ret = _tw.GetBlockUserIds();
                    if (TabInformations.Instance.BlockIds.Count == 0)
                    {
                        _tw.GetBlockUserIds();
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
                for (int i = _favTimestamps.Count - 1; i >= 0; i += -1)
                {
                    if (_favTimestamps[i].CompareTo(oneHour) < 0)
                    {
                        _favTimestamps.RemoveAt(i);
                    }
                }
            }

            if (args.WorkerType == WorkerType.Timeline && !_isInitializing)
            {
                lock (_syncObject)
                {
                    DateTime tm = DateTime.Now;
                    if (_timeLineTimestamps.ContainsKey(tm))
                    {
                        _timeLineTimestamps[tm] += rslt.AddCount;
                    }
                    else
                    {
                        _timeLineTimestamps.Add(tm, rslt.AddCount);
                    }

                    DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                    List<DateTime> keys = new List<DateTime>();
                    _timeLineCount = 0;
                    foreach (DateTime key in _timeLineTimestamps.Keys)
                    {
                        if (key.CompareTo(oneHour) < 0)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            _timeLineCount += _timeLineTimestamps[key];
                        }
                    }

                    foreach (DateTime key in keys)
                    {
                        _timeLineTimestamps.Remove(key);
                    }

                    keys.Clear();
                }
            }

            // 終了ステータス
            if (args.WorkerType != WorkerType.OpenUri)
            {
                bw.ReportProgress(100, MakeStatusMessage(args, true));
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
            if (rslt.WorkerType == WorkerType.OpenUri)
            {
                return;
            }

            // エラー
            if (rslt.RetMsg.Length > 0)
            {
                _myStatusError = true;
                StatusLabel.Text = rslt.RetMsg;
            }

            if (rslt.WorkerType == WorkerType.ErrorState)
            {
                return;
            }

            if (rslt.WorkerType == WorkerType.FavRemove)
            {
                RemovePostFromFavTab(rslt.SIds.ToArray());
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
                RefreshTimeline(false);
            }

            switch (rslt.WorkerType)
            {
                case WorkerType.Timeline:
                    _waitTimeline = false;
                    if (!_isInitializing)
                    {
                        // 'API使用時の取得調整は別途考える（カウント調整？）
                    }

                    break;
                case WorkerType.Reply:
                    _waitReply = false;
                    if (rslt.NewDM && !_isInitializing)
                    {
                        GetTimeline(WorkerType.DirectMessegeRcv);
                    }

                    break;
                case WorkerType.Favorites:
                    _waitFav = false;
                    break;
                case WorkerType.DirectMessegeRcv:
                    _waitDm = false;
                    break;
                case WorkerType.FavAdd:
                case WorkerType.FavRemove:
                    if (_curList != null && _curTab != null)
                    {
                        _curList.BeginUpdate();
                        if (rslt.WorkerType == WorkerType.FavRemove && _statuses.Tabs[_curTab.Text].TabType == TabUsageType.Favorites)
                        {
                            // 色変えは不要
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
                                            if (tb.TabType == TabUsageType.Lists || tb.TabType == TabUsageType.PublicSearch)
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
                                            // 選択アイテム再表示
                                            DispSelectedPost(true);
                                        }
                                    }
                                }
                            }
                        }

                        _curList.EndUpdate();
                    }

                    break;
                case WorkerType.PostMessage:
                    if (string.IsNullOrEmpty(rslt.RetMsg) || rslt.RetMsg.StartsWith("Outputz") || rslt.RetMsg.StartsWith("OK:") || rslt.RetMsg == "Warn:Status is a duplicate.")
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

                        if (!HashMgr.IsPermanent && !string.IsNullOrEmpty(HashMgr.UseHash))
                        {
                            HashMgr.ClearHashtag();
                            HashStripSplitButton.Text = "#[-]";
                            HashToggleMenuItem.Checked = false;
                            HashToggleToolStripMenuItem.Checked = false;
                        }

                        SetMainWindowTitle();
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
                                StatusText_EnterExtracted();
                            }
                        }
                    }

                    if (rslt.RetMsg.Length == 0 && _configs.PostAndGet)
                    {
                        if (_isActiveUserstream)
                        {
                            RefreshTimeline(true);
                        }
                        else
                        {
                            GetTimeline(WorkerType.Timeline);
                        }
                    }

                    break;
                case WorkerType.Retweet:
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

                        if (!_isActiveUserstream && _configs.PostAndGet)
                        {
                            GetTimeline(WorkerType.Timeline);
                        }
                    }

                    break;
                case WorkerType.Follower:
                    _itemCache = null;
                    _postCache = null;
                    if (_curList != null)
                    {
                        _curList.Refresh();
                    }

                    break;
                case WorkerType.Configuration:
                    // _waitFollower = False
                    if (_configs.TwitterConfiguration.PhotoSizeLimit != 0)
                    {
                        _pictureServices["Twitter"].Configuration("MaxUploadFilesize", _configs.TwitterConfiguration.PhotoSizeLimit);
                    }

                    _itemCache = null;
                    _postCache = null;
                    if (_curList != null)
                    {
                        _curList.Refresh();
                    }

                    break;
                case WorkerType.PublicSearch:
                    _waitPubSearch = false;
                    break;
                case WorkerType.UserTimeline:
                    _waitUserTimeline = false;
                    break;
                case WorkerType.List:
                    _waitLists = false;
                    break;
                case WorkerType.Related:
                    {
                        TabClass tb = _statuses.GetTabByType(TabUsageType.Related);
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

        private void GrowlHelper_Callback(object sender, GrowlHelper.NotifyCallbackEventArgs e)
        {
            if (Form.ActiveForm == null)
            {
                BeginInvoke(
                    new Action(() =>
                    {
                        Visible = true;
                        if (WindowState == FormWindowState.Minimized)
                        {
                            WindowState = FormWindowState.Normal;
                        }

                        Activate();
                        BringToFront();
                        if (e.NotifyType == GrowlHelper.NotifyType.DirectMessage)
                        {
                            if (!GoDirectMessage(e.StatusId))
                            {
                                StatusText.Focus();
                            }
                        }
                        else
                        {
                            if (!GoStatus(e.StatusId))
                            {
                                StatusText.Focus();
                            }
                        }
                    }));
            }
        }

        #endregion callback

        #region ListTab events

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            _itemCache = null;
            _itemCacheIndex = -1;
            _postCache = null;
            _prevSelectedTab = e.TabPage;
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

            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, _tabStringFormat);
        }

        private void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == TabUsageType.PublicSearch)
                {
                    Control pnl = ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused || pnl.Controls["comboLang"].Focused || pnl.Controls["buttonSearch"].Focused)
                    {
                        return;
                    }
                }

                ModifierState modState = GetModifierState(e.Control, e.Shift, e.Alt);
                if (modState == ModifierState.NotFlags)
                {
                    return;
                }

                if (modState != ModifierState.None)
                {
                    _anchorFlag = false;
                }

                if (CommonKeyDown(e.KeyCode, FocusedControl.ListTab, modState))
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
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        RemoveSpecifiedTab(ListTab.TabPages[i].Text, true);
                        SaveConfigsTabs();
                        break;
                    }
                }
            }
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D
            if (!_configs.TabMouseLock && e.Button == MouseButtons.Left && _tabDraging)
            {
                string tn = string.Empty;
                Rectangle dragEnableRectangle = new Rectangle(Convert.ToInt32(_tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), Convert.ToInt32(_tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
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
                _tabDraging = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Contains(cpos))
                {
                    _rclickTabName = ListTab.TabPages[i].Text;
                    break;
                }
            }
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            _tabDraging = false;
        }

        private void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // _curList.Refresh()
            DispSelectedPost();
            SetMainWindowTitle();
            SetStatusLabelUrl();
            if (ListTab.Focused || ((Control)ListTab.SelectedTab.Tag).Focused)
            {
                Tag = ListTab.Tag;
            }

            ChangeTabMenuControl(ListTab.SelectedTab.Text);
            PushSelectPostChain();
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            ListTabSelect(e.TabPage);
        }

        #endregion ListTab events

        #region MyList events

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (_itemCache != null
                && e.StartIndex >= _itemCacheIndex
                && e.EndIndex < _itemCacheIndex + _itemCache.Length
                && _curList.Equals(sender))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            if (_curList.Equals(sender))
            {
                CreateCache(e.StartIndex, e.EndIndex);
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_configs.SortOrderLock)
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

                MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

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
                SolidBrush brs1 = ((Control)sender).Focused ? _brsHighLight : _brsDeactiveSelection;
                e.Graphics.FillRectangle(brs1, e.Bounds);
            }
            else
            {
                var cl = e.Item.BackColor;
                SolidBrush brs2 = (cl == _clrSelf) ? _brsBackColorMine :
                    (cl == _clrAtSelf) ? _brsBackColorAt :
                    (cl == _clrTarget) ? _brsBackColorYou :
                    (cl == _clrAtTarget) ? _brsBackColorAtYou :
                    (cl == _clrAtFromTarget) ? _brsBackColorAtFromTarget :
                    (cl == _clrAtTo) ? _brsBackColorAtTo : _brsBackColorNone;
                e.Graphics.FillRectangle(brs2, e.Bounds);
            }

            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                e.DrawFocusRectangle();
            }

            DrawListViewItemIcon(e);
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
            if (_iconCol)
            {
                rct.Y += e.Item.Font.Height;
                rct.Height -= e.Item.Font.Height;
                rctB.Height = e.Item.Font.Height;
            }

            int heightDiff = 0;
            int drawLineCount = Math.Max(1, Math.DivRem(Convert.ToInt32(rct.Height), e.Item.Font.Height, out heightDiff));

            // フォントの高さの半分を足してるのは保険。無くてもいいかも。
            if (!_iconCol && drawLineCount <= 1)
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
                foreColor = ((Control)sender).Focused ? _brsHighLightText.Color : _brsForeColorUnread.Color;
            }
            else
            {
                // 選択されていない行 // 文字色
                var cl = e.Item.ForeColor;
                foreColor =
                    cl == _clrUnread ? _brsForeColorUnread.Color :
                    cl == _clrRead ? _brsForeColorReaded.Color :
                    cl == _clrFav ? _brsForeColorFav.Color :
                    cl == _clrOwl ? _brsForeColorOwl.Color :
                    cl == _clrRetweet ? _brsForeColorRetweet.Color : cl;
            }

            var multiLineFmt = TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            var singleLineFmt = TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.NoPrefix;
            if (_iconCol)
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
            _anchorFlag = false;
        }

        private void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (_configs.ListDoubleClickAction)
            {
                case 0:
                    MakeReplyOrDirectStatus();
                    break;
                case 1:
                    ChangeSelectedFavStatus(true);
                    break;
                case 2:
                    ShowStatusOfCurrentTweetUser();
                    break;
                case 3:
                    ShowUserTimeline();
                    break;
                case 4:
                    AddRelatedStatusesTab();
                    break;
                case 5:
                    TryOpenCurListSelectedUserHome();
                    break;
                case 6:
                    TryOpenSelectedTweetWebPage();
                    break;
                case 7:
                    // 動作なし
                    break;
            }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_itemCache != null && e.ItemIndex >= _itemCacheIndex && e.ItemIndex < _itemCacheIndex + _itemCache.Length && _curList.Equals(sender))
            {
                // A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = _itemCache[e.ItemIndex - _itemCacheIndex];
            }
            else
            {
                // A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((Hoehoe.TweenCustomControl.DetailsListView)sender).Parent;
                try
                {
                    e.Item = CreateItem(tb, _statuses.Item(tb.Text, e.ItemIndex), e.ItemIndex);
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
            catch (ArgumentException)
            {
                return;
            }

            PushSelectPostChain();

            if (_configs.UnreadManage)
            {
                _statuses.SetReadAllTab(true, _curTab.Text, _curItemIndex);
            }

            // キャッシュの書き換え
            ChangeCacheStyleRead(true, _curItemIndex, _curTab);

            // 既読へ（フォント、文字色）
            ColorizeList();
            _colorize = true;
        }

        #endregion MyList events

        #region userstream

        private void Tw_NewPostFromStream()
        {
            if (_configs.ReadOldPosts)
            {
                // 新着時未読クリア
                _statuses.SetRead();
            }

            int rsltAddCount = _statuses.DistributePosts();
            lock (_syncObject)
            {
                DateTime tm = DateTime.Now;
                if (_timeLineTimestamps.ContainsKey(tm))
                {
                    _timeLineTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    _timeLineTimestamps.Add(tm, rsltAddCount);
                }

                DateTime oneHour = tm.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                _timeLineCount = 0;
                foreach (System.DateTime key in _timeLineTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                    {
                        keys.Add(key);
                    }
                    else
                    {
                        _timeLineCount += _timeLineTimestamps[key];
                    }
                }

                foreach (DateTime key in keys)
                {
                    _timeLineTimestamps.Remove(key);
                }

                keys.Clear();
            }

            if (_configs.UserstreamPeriodInt > 0)
            {
                return;
            }

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

        private void Tw_PostDeleted(long id)
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
                            if (_curPost != null && _curPost.StatusId == id)
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
            _modifySettingCommon = true;
        }

        private void Tw_UserStreamEventArrived(Twitter.FormattedEvent ev)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new Action<Twitter.FormattedEvent>(Tw_UserStreamEventArrived), ev);
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

                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(_tw.Username.ToLower()))
                {
                    RemovePostFromFavTab(new long[] { ev.Id });
                }
            }
        }

        private void Tw_UserStreamStarted()
        {
            _isActiveUserstream = true;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(Tw_UserStreamStarted));
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

            ChangeUserStreamStatusDisplay(start: true);
        }

        private void Tw_UserStreamStopped()
        {
            _isActiveUserstream = false;
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke(new MethodInvoker(Tw_UserStreamStopped));
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

            ChangeUserStreamStatusDisplay(start: false);
        }

        #endregion userstream

        #endregion event handler
    }
}