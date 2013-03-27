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
    using System.Collections.Specialized;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using R = Properties.Resources;

    public partial class FilterDialog
    {
        #region privates

        private EDITMODE _editMode;
        private bool _isDirectAdd;
        private TabInformations _sts;
        private string _cur;
        private readonly List<string> _idList = new List<string>();
        private readonly TabsDialog _tabDialog = new TabsDialog(true);

        #endregion

        #region constructor

        public FilterDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region enums

        private enum EDITMODE
        {
            AddNew,
            Edit,
            None
        }

        #endregion

        #region properties

        private TweenMain TwMain
        {
            get { return (TweenMain)Owner; }
        }

        #endregion

        #region public methods

        public void SetCurrent(string tabName)
        {
            _cur = tabName;
        }

        public void AddNewFilter(string id, string msg)
        {
            // 元フォームから直接呼ばれる
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            ButtonDelete.Enabled = false;
            closeButton.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;
            ListFilters.Enabled = false;

            RadioAND.Checked = true;
            RadioPLUS.Checked = false;
            UID.Text = id;
            UID.SelectAll();
            MSG1.Text = msg;
            MSG1.SelectAll();
            MSG2.Text = id + msg;
            MSG2.SelectAll();
            TextSource.Text = string.Empty;
            UID.Enabled = true;
            MSG1.Enabled = true;
            MSG2.Enabled = false;
            CheckRegex.Checked = false;
            CheckURL.Checked = false;
            CheckCaseSensitive.Checked = false;
            CheckRetweet.Checked = false;
            CheckLambda.Checked = false;

            RadioExAnd.Checked = true;
            RadioExPLUS.Checked = false;
            ExUID.Text = string.Empty;
            ExUID.SelectAll();
            ExMSG1.Text = string.Empty;
            ExMSG1.SelectAll();
            ExMSG2.Text = string.Empty;
            ExMSG2.SelectAll();
            TextExSource.Text = string.Empty;
            ExUID.Enabled = true;
            ExMSG1.Enabled = true;
            ExMSG2.Enabled = false;
            CheckExRegex.Checked = false;
            CheckExURL.Checked = false;
            CheckExCaseSensitive.Checked = false;
            CheckExRetweet.Checked = false;
            CheckExLambDa.Checked = false;

            OptCopy.Checked = true;
            CheckMark.Checked = true;
            UID.Focus();
            _editMode = EDITMODE.AddNew;
            _isDirectAdd = true;
        }

        #endregion

        #region event handler

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            ButtonNew_ClickExtracted();
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            ButtonEdit_ClickExtracted();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            ButtonDelete_ClickExtracted();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            ButtonCancel_ClickExtracted();
        }

        private void RadioAND_CheckedChanged(object sender, EventArgs e)
        {
            RadioAND_CheckedChangedExtracted(RadioAND.Checked);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            ButtonOK_ClickExtracted();
        }

        private void ListFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetail();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FilterDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isDirectAdd = false;
        }

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (EditFilterGroup.Enabled)
                {
                    ButtonCancel_ClickExtracted();
                }
                else
                {
                    Close();
                }
            }
        }

        private void ListFilters_DoubleClick(object sender, EventArgs e)
        {
            if (ListFilters.SelectedItem == null)
            {
                return;
            }

            int clickedIndex = ListFilters.IndexFromPoint(ListFilters.PointToClient(MousePosition));
            if (clickedIndex == ListBox.NoMatches)
            {
                return;
            }

            if (ListFilters.Items[clickedIndex] == null)
            {
                return;
            }

            ButtonEdit_ClickExtracted();
        }

        private void FilterDialog_Shown(object sender, EventArgs e)
        {
            _sts = TabInformations.Instance;
            ListTabs.Items.Clear();
            foreach (string key in _sts.Tabs.Keys)
            {
                ListTabs.Items.Add(key);
            }

            SetTabnamesToDialog();

            ComboSound.Items.Clear();
            ComboSound.Items.Add(string.Empty);
            var names = MyCommon.GetSoundFileNames();
            if (names.Length > 0)
            {
                ComboSound.Items.AddRange(names);
            }

            _idList.Clear();
            foreach (string tmp in TwMain.AtIdSupl.GetItemList())
            {
                _idList.Add(tmp.Remove(0, 1)); // @文字削除
            }

            UID.AutoCompleteCustomSource.Clear();
            UID.AutoCompleteCustomSource.AddRange(_idList.ToArray());

            ExUID.AutoCompleteCustomSource.Clear();
            ExUID.AutoCompleteCustomSource.AddRange(_idList.ToArray());

            // 選択タブ変更
            if (ListTabs.Items.Count > 0)
            {
                if (_cur.Length > 0)
                {
                    for (int i = 0; i < ListTabs.Items.Count; i++)
                    {
                        if (_cur == ListTabs.Items[i].ToString())
                        {
                            ListTabs.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1)
            {
                SetFilters((string)ListTabs.SelectedItem);
            }
            else
            {
                ListFilters.Items.Clear();
            }
        }

        private void ButtonAddTab_Click(object sender, EventArgs e)
        {
            ButtonAddTab_ClickExtracted();
        }

        private void ButtonDeleteTab_Click(object sender, EventArgs e)
        {
            ButtonDeleteTab_ClickExtracted();
        }

        private void ButtonRenameTab_Click(object sender, EventArgs e)
        {
            ButtonRenameTab_ClickExtracted();
        }

        private void CheckManageRead_CheckedChanged(object sender, EventArgs e)
        {
            CheckManageRead_CheckedChangedExtracted();
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            ButtonUp_ClickExtracted();
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            ButtonDown_ClickExtracted();
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            CheckNotifyNew_CheckedChangedExtracted();
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboSound_SelectedIndexChangedExtracted();
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            RadioExAnd_CheckedChangedExtracted(RadioExAnd.Checked);
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
        {
            CheckMark.Enabled = !OptMove.Checked;
        }

        private void ButtonRuleUp_Click(object sender, EventArgs e)
        {
            ButtonRuleUp_ClickExtracted();
        }

        private void ButtonRuleDown_Click(object sender, EventArgs e)
        {
            ButtonRuleDown_ClickExtracted();
        }

        private void ButtonRuleCopy_Click(object sender, EventArgs e)
        {
            ButtonRuleCopy_ClickExtracted();
        }

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            ButtonRuleMove_ClickExtracted();
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && e.Modifiers == (Keys.Shift | Keys.Control))
            {
                var main = (TweenMain)Owner;
                var tbox = (TextBox)sender;
                if (tbox.SelectionStart > 0)
                {
                    int endidx = tbox.SelectionStart - 1;
                    for (int i = tbox.SelectionStart - 1; i >= 0; i += -1)
                    {
                        char c = tbox.Text[i];
                        if (char.IsLetterOrDigit(c) || c == '_')
                        {
                            continue;
                        }

                        string startstr;
                        if (c == '@')
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.AtIdSupl, startstr.Length + 1, startstr);
                        }
                        else if (c == '#')
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.HashSupl, startstr.Length + 1, startstr);
                        }
                        else
                        {
                            break;
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void FilterTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var main = (TweenMain)Owner;
            var tbox = (TextBox)sender;
            if (e.KeyChar == '@')
            {
                // @マーク
                main.ShowSuplDialog(tbox, main.AtIdSupl);
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                main.ShowSuplDialog(tbox, main.HashSupl);
                e.Handled = true;
            }
        }

        #endregion

        #region private methods

        private void SetFilters(string tabName)
        {
            if (ListTabs.Items.Count == 0)
            {
                return;
            }

            var tab = _sts.Tabs[tabName];
            ListFilters.Items.Clear();
            ListFilters.Items.AddRange(tab.GetFilters());
            if (ListFilters.Items.Count > 0)
            {
                ListFilters.SelectedIndex = 0;
            }

            CheckManageRead.Checked = tab.UnreadManage;
            CheckNotifyNew.Checked = tab.Notify;
            int idx = ComboSound.Items.IndexOf(tab.SoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            ComboSound.SelectedIndex = idx;

            if (_isDirectAdd)
            {
                return;
            }

            ListTabs.Enabled = true;
            GroupTab.Enabled = true;
            ListFilters.Enabled = true;
            if (ListFilters.SelectedIndex != -1)
            {
                ShowDetail();
            }

            EditFilterGroup.Enabled = false;
            switch (tab.TabType)
            {
                case TabUsageType.Home:
                case TabUsageType.DirectMessage:
                case TabUsageType.Favorites:
                case TabUsageType.PublicSearch:
                case TabUsageType.Lists:
                case TabUsageType.Related:
                case TabUsageType.UserTimeline:
                    ButtonNew.Enabled = false;
                    ButtonEdit.Enabled = false;
                    ButtonDelete.Enabled = false;
                    ButtonRuleUp.Enabled = false;
                    ButtonRuleDown.Enabled = false;
                    ButtonRuleCopy.Enabled = false;
                    ButtonRuleMove.Enabled = false;
                    break;

                default:
                    ButtonNew.Enabled = true;
                    bool selected = ListFilters.SelectedIndex > -1;
                    ButtonEdit.Enabled = selected;
                    ButtonDelete.Enabled = selected;
                    ButtonRuleUp.Enabled = selected;
                    ButtonRuleDown.Enabled = selected;
                    ButtonRuleCopy.Enabled = selected;
                    ButtonRuleMove.Enabled = selected;
                    break;
            }

            switch (tab.TabType)
            {
                case TabUsageType.Home:
                    LabelTabType.Text = R.TabUsageTypeName_Home;
                    break;

                case TabUsageType.Mentions:
                    LabelTabType.Text = R.TabUsageTypeName_Mentions;
                    break;

                case TabUsageType.DirectMessage:
                    LabelTabType.Text = R.TabUsageTypeName_DirectMessage;
                    break;

                case TabUsageType.Favorites:
                    LabelTabType.Text = R.TabUsageTypeName_Favorites;
                    break;

                case TabUsageType.UserDefined:
                    LabelTabType.Text = R.TabUsageTypeName_UserDefined;
                    break;

                case TabUsageType.PublicSearch:
                    LabelTabType.Text = R.TabUsageTypeName_PublicSearch;
                    break;

                case TabUsageType.Lists:
                    LabelTabType.Text = R.TabUsageTypeName_Lists;
                    break;

                case TabUsageType.Related:
                    LabelTabType.Text = R.TabUsageTypeName_Related;
                    break;

                case TabUsageType.UserTimeline:
                    LabelTabType.Text = R.TabUsageTypeName_UserTimeline;
                    break;

                default:
                    LabelTabType.Text = "UNKNOWN";
                    break;
            }

            ButtonRenameTab.Enabled = true;
            ButtonDeleteTab.Enabled = !_sts.IsDefaultTab(tabName);
            closeButton.Enabled = true;
        }

        private void ShowDetail()
        {
            if (_isDirectAdd)
            {
                return;
            }

            if (ListFilters.SelectedIndex > -1)
            {
                var fc = (FiltersClass)ListFilters.SelectedItem;
                if (fc.SearchBoth)
                {
                    RadioAND.Checked = true;
                    RadioPLUS.Checked = false;
                    UID.Enabled = true;
                    MSG1.Enabled = true;
                    MSG2.Enabled = false;
                    UID.Text = fc.NameFilter;
                    UID.SelectAll();
                    MSG1.Text = string.Empty;
                    MSG2.Text = string.Empty;
                    foreach (string bf in fc.BodyFilter)
                    {
                        MSG1.Text += bf + " ";
                    }

                    MSG1.Text = MSG1.Text.Trim();
                    MSG1.SelectAll();
                }
                else
                {
                    RadioPLUS.Checked = true;
                    RadioAND.Checked = false;
                    UID.Enabled = false;
                    MSG1.Enabled = false;
                    MSG2.Enabled = true;
                    UID.Text = string.Empty;
                    MSG1.Text = string.Empty;
                    MSG2.Text = string.Empty;
                    foreach (string bf in fc.BodyFilter)
                    {
                        MSG2.Text += bf + " ";
                    }

                    MSG2.Text = MSG2.Text.Trim();
                    MSG2.SelectAll();
                }

                TextSource.Text = fc.Source;
                CheckRegex.Checked = fc.UseRegex;
                CheckURL.Checked = fc.SearchUrl;
                CheckCaseSensitive.Checked = fc.CaseSensitive;
                CheckRetweet.Checked = fc.IsRt;
                CheckLambda.Checked = fc.UseLambda;

                if (fc.ExSearchBoth)
                {
                    RadioExAnd.Checked = true;
                    RadioExPLUS.Checked = false;
                    ExUID.Enabled = true;
                    ExMSG1.Enabled = true;
                    ExMSG2.Enabled = false;
                    ExUID.Text = fc.ExNameFilter;
                    ExUID.SelectAll();
                    ExMSG1.Text = string.Empty;
                    ExMSG2.Text = string.Empty;
                    foreach (string bf in fc.ExBodyFilter)
                    {
                        ExMSG1.Text += bf + " ";
                    }

                    ExMSG1.Text = ExMSG1.Text.Trim();
                    ExMSG1.SelectAll();
                }
                else
                {
                    RadioExPLUS.Checked = true;
                    RadioExAnd.Checked = false;
                    ExUID.Enabled = false;
                    ExMSG1.Enabled = false;
                    ExMSG2.Enabled = true;
                    ExUID.Text = string.Empty;
                    ExMSG1.Text = string.Empty;
                    ExMSG2.Text = string.Empty;
                    foreach (string bf in fc.ExBodyFilter)
                    {
                        ExMSG2.Text += bf + " ";
                    }

                    ExMSG2.Text = ExMSG2.Text.Trim();
                    ExMSG2.SelectAll();
                }

                TextExSource.Text = fc.ExSource;
                CheckExRegex.Checked = fc.ExUseRegex;
                CheckExURL.Checked = fc.ExSearchUrl;
                CheckExCaseSensitive.Checked = fc.ExCaseSensitive;
                CheckExRetweet.Checked = fc.IsExRt;
                CheckExLambDa.Checked = fc.ExUseLambda;

                if (fc.MoveFrom)
                {
                    OptMove.Checked = true;
                }
                else
                {
                    OptCopy.Checked = true;
                }

                CheckMark.Checked = fc.SetMark;
                ButtonEdit.Enabled = true;
                ButtonDelete.Enabled = true;
                ButtonRuleUp.Enabled = true;
                ButtonRuleDown.Enabled = true;
                ButtonRuleCopy.Enabled = true;
                ButtonRuleMove.Enabled = true;
            }
            else
            {
                RadioAND.Checked = true;
                RadioPLUS.Checked = false;
                UID.Enabled = true;
                MSG1.Enabled = true;
                MSG2.Enabled = false;
                UID.Text = string.Empty;
                MSG1.Text = string.Empty;
                MSG2.Text = string.Empty;
                TextSource.Text = string.Empty;
                CheckRegex.Checked = false;
                CheckURL.Checked = false;
                CheckCaseSensitive.Checked = false;
                CheckRetweet.Checked = false;
                CheckLambda.Checked = false;
                RadioExAnd.Checked = true;
                RadioExPLUS.Checked = false;
                ExUID.Enabled = true;
                ExMSG1.Enabled = true;
                ExMSG2.Enabled = false;
                ExUID.Text = string.Empty;
                ExMSG1.Text = string.Empty;
                ExMSG2.Text = string.Empty;
                TextExSource.Text = string.Empty;
                CheckExRegex.Checked = false;
                CheckExURL.Checked = false;
                CheckExCaseSensitive.Checked = false;
                CheckExRetweet.Checked = false;
                CheckExLambDa.Checked = false;
                OptCopy.Checked = true;
                CheckMark.Checked = true;
                ButtonEdit.Enabled = false;
                ButtonDelete.Enabled = false;
                ButtonRuleUp.Enabled = false;
                ButtonRuleDown.Enabled = false;
                ButtonRuleCopy.Enabled = false;
                ButtonRuleMove.Enabled = false;
            }
        }

        private bool IsValidLambdaExp(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            try
            {
                LambdaExpression expr = DynamicExpression.ParseLambda<PostClass, bool>(text, new PostClass());
            }
            catch (Exception ex)
            {
                MessageBox.Show(R.IsValidLambdaExpText1 + ex.Message, R.IsValidLambdaExpText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool IsValidRegexp(string text)
        {
            try
            {
                var rgx = new Regex(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(R.ButtonOK_ClickText3 + ex.Message, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool CheckMatchRule(out bool isBlank)
        {
            isBlank = false;
            TextSource.Text = TextSource.Text.Trim();
            if (RadioAND.Checked)
            {
                MSG1.Text = MSG1.Text.Trim();
                UID.Text = UID.Text.Trim();
                if (!CheckRegex.Checked && !CheckLambda.Checked)
                {
                    MSG1.Text = MSG1.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(UID.Text) && string.IsNullOrEmpty(MSG1.Text) && string.IsNullOrEmpty(TextSource.Text) && !CheckRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (CheckLambda.Checked)
                {
                    if (!IsValidLambdaExp(UID.Text))
                    {
                        return false;
                    }

                    if (!IsValidLambdaExp(MSG1.Text))
                    {
                        return false;
                    }
                }
                else if (CheckRegex.Checked)
                {
                    if (!IsValidRegexp(UID.Text))
                    {
                        return false;
                    }

                    if (!IsValidRegexp(MSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                MSG2.Text = MSG2.Text.Trim();
                if (!CheckRegex.Checked && !CheckLambda.Checked)
                {
                    MSG2.Text = MSG2.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(MSG2.Text) && string.IsNullOrEmpty(TextSource.Text) && !CheckRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (CheckLambda.Checked && !IsValidLambdaExp(MSG2.Text))
                {
                    return false;
                }

                if (CheckRegex.Checked && !IsValidRegexp(MSG2.Text))
                {
                    return false;
                }
            }

            if (CheckRegex.Checked && !IsValidRegexp(TextSource.Text))
            {
                return false;
            }

            return true;
        }

        private bool CheckExcludeRule(out bool isBlank)
        {
            isBlank = false;
            TextExSource.Text = TextExSource.Text.Trim();
            if (RadioExAnd.Checked)
            {
                ExMSG1.Text = ExMSG1.Text.Trim();
                if (!CheckExRegex.Checked && !CheckExLambDa.Checked)
                {
                    ExMSG1.Text = ExMSG1.Text.Replace("　", " ");
                }

                ExUID.Text = ExUID.Text.Trim();
                if (string.IsNullOrEmpty(ExUID.Text) && string.IsNullOrEmpty(ExMSG1.Text) && string.IsNullOrEmpty(TextExSource.Text) && !CheckExRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (CheckExLambDa.Checked)
                {
                    if (!IsValidLambdaExp(ExUID.Text))
                    {
                        return false;
                    }

                    if (!IsValidLambdaExp(ExMSG1.Text))
                    {
                        return false;
                    }
                }
                else if (CheckExRegex.Checked)
                {
                    if (!IsValidRegexp(ExUID.Text))
                    {
                        return false;
                    }

                    if (!IsValidRegexp(ExMSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                ExMSG2.Text = ExMSG2.Text.Trim();
                if (!CheckExRegex.Checked && !CheckExLambDa.Checked)
                {
                    ExMSG2.Text = ExMSG2.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(ExMSG2.Text) && string.IsNullOrEmpty(TextExSource.Text) && !CheckExRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (CheckExLambDa.Checked && !IsValidLambdaExp(ExMSG2.Text))
                {
                    return false;
                }

                if (CheckExRegex.Checked && !IsValidRegexp(ExMSG2.Text))
                {
                    return false;
                }
            }

            if (CheckExRegex.Checked && !IsValidRegexp(TextExSource.Text))
            {
                return false;
            }

            return true;
        }

        private void SetTabnamesToDialog()
        {
            _tabDialog.ClearTab();
            foreach (string key in _sts.Tabs.Keys)
            {
                if (TabInformations.Instance.IsDistributableTab(key))
                {
                    _tabDialog.AddTab(key);
                }
            }
        }

        private void ButtonNew_ClickExtracted()
        {
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            closeButton.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            ButtonDelete.Enabled = false;
            closeButton.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;
            ListFilters.Enabled = false;

            RadioAND.Checked = true;
            RadioPLUS.Checked = false;
            UID.Text = string.Empty;
            MSG1.Text = string.Empty;
            MSG2.Text = string.Empty;
            TextSource.Text = string.Empty;
            UID.Enabled = true;
            MSG1.Enabled = true;
            MSG2.Enabled = false;
            CheckRegex.Checked = false;
            CheckURL.Checked = false;
            CheckCaseSensitive.Checked = false;
            CheckRetweet.Checked = false;
            CheckLambda.Checked = false;

            RadioExAnd.Checked = true;
            RadioExPLUS.Checked = false;
            ExUID.Text = string.Empty;
            ExMSG1.Text = string.Empty;
            ExMSG2.Text = string.Empty;
            TextExSource.Text = string.Empty;
            ExUID.Enabled = true;
            ExMSG1.Enabled = true;
            ExMSG2.Enabled = false;
            CheckExRegex.Checked = false;
            CheckExURL.Checked = false;
            CheckExCaseSensitive.Checked = false;
            CheckExRetweet.Checked = false;
            CheckExLambDa.Checked = false;

            OptCopy.Checked = true;
            CheckMark.Checked = true;
            UID.Focus();
            _editMode = EDITMODE.AddNew;
        }

        private void ButtonEdit_ClickExtracted()
        {
            if (ListFilters.SelectedIndex == -1)
            {
                return;
            }

            ShowDetail();

            int idx = ListFilters.SelectedIndex;
            ListFilters.SelectedIndex = -1;
            ListFilters.SelectedIndex = idx;
            ListFilters.Enabled = false;

            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonDelete.Enabled = false;
            closeButton.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;

            _editMode = EDITMODE.Edit;
        }

        private void ButtonDelete_ClickExtracted()
        {
            if (ListFilters.SelectedIndex == -1)
            {
                return;
            }

            string tmp;
            if (ListFilters.SelectedIndices.Count == 1)
            {
                var deleteUser = (string)ListFilters.SelectedItem;
                tmp = string.Format(R.ButtonDelete_ClickText1, Environment.NewLine, deleteUser);
            }
            else
            {
                tmp = string.Format(R.ButtonDelete_ClickText3, ListFilters.SelectedIndices.Count);
            }

            var rslt = MessageBox.Show(tmp, R.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            for (int idx = ListFilters.Items.Count - 1; idx >= 0; idx--)
            {
                if (ListFilters.GetSelected(idx))
                {
                    _sts.Tabs[ListTabs.SelectedItem.ToString()].RemoveFilter((FiltersClass)ListFilters.Items[idx]);
                    ListFilters.Items.RemoveAt(idx);
                }
            }
        }

        private void ButtonCancel_ClickExtracted()
        {
            ListTabs.Enabled = true;
            GroupTab.Enabled = true;
            ListFilters.Enabled = true;
            ListFilters.Focus();
            if (ListFilters.SelectedIndex != -1)
            {
                ShowDetail();
            }

            EditFilterGroup.Enabled = false;
            ButtonNew.Enabled = true;

            bool selected = ListFilters.SelectedIndex > -1;
            ButtonEdit.Enabled = selected;
            ButtonDelete.Enabled = selected;
            ButtonRuleUp.Enabled = selected;
            ButtonRuleDown.Enabled = selected;
            ButtonRuleCopy.Enabled = selected;
            ButtonRuleMove.Enabled = selected;

            closeButton.Enabled = true;
            if (_isDirectAdd)
            {
                Close();
            }
        }

        private void RadioAND_CheckedChangedExtracted(bool flg)
        {
            UID.Enabled = flg;
            MSG1.Enabled = flg;
            MSG2.Enabled = !flg;
        }

        private void ButtonOK_ClickExtracted()
        {
            bool isBlankMatch;
            bool isBlankExclude;

            // 入力チェック
            if (!CheckMatchRule(out isBlankMatch) || !CheckExcludeRule(out isBlankExclude))
            {
                return;
            }

            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(R.ButtonOK_ClickText1, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int prevSelectedIndex = ListFilters.SelectedIndex;
            var ft = new FiltersClass { MoveFrom = OptMove.Checked, SetMark = CheckMark.Checked };

            string bdy;
            if (RadioAND.Checked)
            {
                ft.NameFilter = UID.Text;
                TwMain.AddAtIdSuplItem("@" + ft.NameFilter);
                ft.SearchBoth = true;
                bdy = MSG1.Text;
            }
            else
            {
                ft.NameFilter = string.Empty;
                ft.SearchBoth = false;
                bdy = MSG2.Text;
            }

            ft.Source = TextSource.Text.Trim();

            if (CheckRegex.Checked || CheckLambda.Checked)
            {
                ft.BodyFilter.Add(bdy);
            }
            else
            {
                string[] bf = bdy.Trim().Split(' ');
                foreach (string bfs in bf)
                {
                    if (!string.IsNullOrEmpty(bfs))
                    {
                        ft.BodyFilter.Add(bfs.Trim());
                    }
                }
            }

            ft.UseRegex = CheckRegex.Checked;
            ft.SearchUrl = CheckURL.Checked;
            ft.CaseSensitive = CheckCaseSensitive.Checked;
            ft.IsRt = CheckRetweet.Checked;
            ft.UseLambda = CheckLambda.Checked;

            if (RadioExAnd.Checked)
            {
                ft.ExNameFilter = ExUID.Text;
                ft.ExSearchBoth = true;
                bdy = ExMSG1.Text;
            }
            else
            {
                ft.ExNameFilter = string.Empty;
                ft.ExSearchBoth = false;
                bdy = ExMSG2.Text;
            }

            ft.ExSource = TextExSource.Text.Trim();

            if (CheckExRegex.Checked || CheckExLambDa.Checked)
            {
                ft.ExBodyFilter.Add(bdy);
            }
            else
            {
                string[] bf = bdy.Trim().Split(' ');
                foreach (string bfs in bf)
                {
                    if (!string.IsNullOrEmpty(bfs))
                    {
                        ft.ExBodyFilter.Add(bfs.Trim());
                    }
                }
            }

            ft.ExUseRegex = CheckExRegex.Checked;
            ft.ExSearchUrl = CheckExURL.Checked;
            ft.ExCaseSensitive = CheckExCaseSensitive.Checked;
            ft.IsExRt = CheckExRetweet.Checked;
            ft.ExUseLambda = CheckExLambDa.Checked;

            if (_editMode == EDITMODE.AddNew)
            {
                if (!_sts.Tabs[ListTabs.SelectedItem.ToString()].AddFilter(ft))
                {
                    MessageBox.Show(R.ButtonOK_ClickText4, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].EditFilter((FiltersClass)ListFilters.SelectedItem, ft);
            }

            SetFilters(ListTabs.SelectedItem.ToString());
            ListFilters.SelectedIndex = -1;
            ListFilters.SelectedIndex = _editMode == EDITMODE.AddNew ? ListFilters.Items.Count - 1 : prevSelectedIndex;
            _editMode = EDITMODE.None;
            if (_isDirectAdd)
            {
                Close();
            }
        }

        private void ButtonAddTab_ClickExtracted()
        {
            string tabName;
            TabUsageType tabType;
            using (var inputName = new InputTabName())
            {
                inputName.TabName = _sts.GetUniqueTabName();
                inputName.SetIsShowUsage(true);
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel)
                {
                    return;
                }

                tabName = inputName.TabName;
                tabType = inputName.Usage;
            }

            if (!string.IsNullOrEmpty(tabName))
            {
                // List対応
                ListElement list = null;
                if (tabType == TabUsageType.Lists)
                {
                    string rslt = ((TweenMain)Owner).TwitterInstance.GetListsApi();
                    if (!string.IsNullOrEmpty(rslt))
                    {
                        MessageBox.Show("Failed to get lists. (" + rslt + ")");
                    }

                    using (var listAvail = new ListAvailable())
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

                if (!_sts.AddTab(tabName, tabType, list) || !((TweenMain)Owner).AddNewTab(tabName, false, tabType, list))
                {
                    string tmp = string.Format(R.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, R.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 成功
                ListTabs.Items.Add(tabName);
                SetTabnamesToDialog();
            }
        }

        private void ButtonDeleteTab_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty((string)ListTabs.SelectedItem))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)Owner).RemoveSpecifiedTab(tb, true))
                {
                    ListTabs.Items.RemoveAt(idx);
                    idx -= 1;
                    if (idx < 0)
                    {
                        idx = 0;
                    }

                    ListTabs.SelectedIndex = idx;
                    SetTabnamesToDialog();
                }
            }
        }

        private void ButtonRenameTab_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty((string)ListTabs.SelectedItem))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)Owner).RenameTab(ref tb))
                {
                    ListTabs.Items.RemoveAt(idx);
                    ListTabs.Items.Insert(idx, tb);
                    ListTabs.SelectedIndex = idx;
                    SetTabnamesToDialog();
                }
            }
        }

        private void CheckManageRead_CheckedChangedExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                ((TweenMain)Owner).ChangeTabUnreadManage(ListTabs.SelectedItem.ToString(), CheckManageRead.Checked);
            }
        }

        private void ButtonUp_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > 0 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex - 1].ToString();
                ((TweenMain)Owner).ReorderTab(selName, tgtName, true);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx - 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void ButtonDown_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && ListTabs.SelectedIndex < ListTabs.Items.Count - 1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex + 1].ToString();
                ((TweenMain)Owner).ReorderTab(selName, tgtName, false);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx + 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void CheckNotifyNew_CheckedChangedExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].Notify = CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChangedExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string filename = string.Empty;
                if (ComboSound.SelectedIndex > -1)
                {
                    filename = ComboSound.SelectedItem.ToString();
                }

                _sts.Tabs[ListTabs.SelectedItem.ToString()].SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChangedExtracted(bool flg)
        {
            ExUID.Enabled = flg;
            ExMSG1.Enabled = flg;
            ExMSG2.Enabled = !flg;
        }

        private void ButtonRuleUp_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null && ListFilters.SelectedIndex > 0)
            {
                string tabname = ListTabs.SelectedItem.ToString();

                // FiltersClass selected = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex];
                FiltersClass target = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex - 1];
                int idx = ListFilters.SelectedIndex;
                ListFilters.Items.RemoveAt(idx - 1);
                ListFilters.Items.Insert(idx, target);
                _sts.Tabs[tabname].Filters.RemoveAt(idx - 1);
                _sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleDown_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null && ListFilters.SelectedIndex < ListFilters.Items.Count - 1)
            {
                string tabname = ListTabs.SelectedItem.ToString();

                // FiltersClass selected = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex];
                FiltersClass target = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex + 1];
                int idx = ListFilters.SelectedIndex;
                ListFilters.Items.RemoveAt(idx + 1);
                ListFilters.Items.Insert(idx, target);
                _sts.Tabs[tabname].Filters.RemoveAt(idx + 1);
                _sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleCopy_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                _tabDialog.Text = R.ButtonRuleCopy_ClickText1;
                if (_tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                string tabname = ListTabs.SelectedItem.ToString();
                StringCollection tabs = _tabDialog.SelectedTabNames;
                var filters = new List<FiltersClass>();

                foreach (int idx in ListFilters.SelectedIndices)
                {
                    filters.Add(_sts.Tabs[tabname].Filters[idx].CopyTo(new FiltersClass()));
                }

                foreach (string tb in tabs)
                {
                    if (tb != tabname)
                    {
                        foreach (FiltersClass flt in filters)
                        {
                            if (!_sts.Tabs[tb].Filters.Contains(flt))
                            {
                                _sts.Tabs[tb].AddFilter(flt.CopyTo(new FiltersClass()));
                            }
                        }
                    }
                }

                SetFilters(tabname);
            }
        }

        private void ButtonRuleMove_ClickExtracted()
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                _tabDialog.Text = R.ButtonRuleMove_ClickText1;
                if (_tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                var tabname = ListTabs.SelectedItem.ToString();
                var tabs = _tabDialog.SelectedTabNames;
                var filters = new List<FiltersClass>();

                foreach (int idx in ListFilters.SelectedIndices)
                {
                    filters.Add(_sts.Tabs[tabname].Filters[idx].CopyTo(new FiltersClass()));
                }

                if (tabs.Count == 1 && tabs[0] == tabname)
                {
                    return;
                }

                foreach (string tb in tabs)
                {
                    if (tb != tabname)
                    {
                        foreach (FiltersClass flt in filters)
                        {
                            if (!_sts.Tabs[tb].Filters.Contains(flt))
                            {
                                _sts.Tabs[tb].AddFilter(flt.CopyTo(new FiltersClass()));
                            }
                        }
                    }
                }

                // TODO: VB's for/next loop to C# for{}
                for (int idx = ListFilters.Items.Count - 1; idx >= 0; idx += -1)
                {
                    if (ListFilters.GetSelected(idx))
                    {
                        _sts.Tabs[ListTabs.SelectedItem.ToString()].RemoveFilter((FiltersClass)ListFilters.Items[idx]);
                        ListFilters.Items.RemoveAt(idx);
                    }
                }

                SetFilters(tabname);
            }
        }

        #endregion
    }
}