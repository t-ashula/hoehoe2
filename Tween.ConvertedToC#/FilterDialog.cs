using System;
using System.Collections.Generic;

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

using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Tween
{
    public partial class FilterDialog
    {
        private EDITMODE _mode;
        private bool _directAdd;
        private TabInformations _sts;
        private string _cur;

        private List<string> idlist = new List<string>();

        private TabsDialog tabdialog = new TabsDialog(true);

        private enum EDITMODE
        {
            AddNew,
            Edit,
            None
        }

        private void SetFilters(string tabName)
        {
            if (ListTabs.Items.Count == 0)
                return;

            ListFilters.Items.Clear();
            ListFilters.Items.AddRange(_sts.Tabs[tabName].GetFilters());
            if (ListFilters.Items.Count > 0)
                ListFilters.SelectedIndex = 0;

            CheckManageRead.Checked = _sts.Tabs[tabName].UnreadManage;
            CheckNotifyNew.Checked = _sts.Tabs[tabName].Notify;

            int idx = ComboSound.Items.IndexOf(_sts.Tabs[tabName].SoundFile);
            if (idx == -1)
                idx = 0;
            ComboSound.SelectedIndex = idx;

            if (_directAdd)
                return;

            ListTabs.Enabled = true;
            GroupTab.Enabled = true;
            ListFilters.Enabled = true;
            if (ListFilters.SelectedIndex != -1)
            {
                ShowDetail();
            }
            EditFilterGroup.Enabled = false;
            switch (TabInformations.GetInstance().Tabs[tabName].TabType)
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
                    if (ListFilters.SelectedIndex > -1)
                    {
                        ButtonEdit.Enabled = true;
                        ButtonDelete.Enabled = true;
                        ButtonRuleUp.Enabled = true;
                        ButtonRuleDown.Enabled = true;
                        ButtonRuleCopy.Enabled = true;
                        ButtonRuleMove.Enabled = true;
                    }
                    else
                    {
                        ButtonEdit.Enabled = false;
                        ButtonDelete.Enabled = false;
                        ButtonRuleUp.Enabled = false;
                        ButtonRuleDown.Enabled = false;
                        ButtonRuleCopy.Enabled = false;
                        ButtonRuleMove.Enabled = false;
                    }
                    break;
            }
            switch (TabInformations.GetInstance().Tabs[tabName].TabType)
            {
                case TabUsageType.Home:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_Home;
                    break;
                case TabUsageType.Mentions:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_Mentions;
                    break;
                case TabUsageType.DirectMessage:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_DirectMessage;
                    break;
                case TabUsageType.Favorites:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_Favorites;
                    break;
                case TabUsageType.UserDefined:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_UserDefined;
                    break;
                case TabUsageType.PublicSearch:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_PublicSearch;
                    break;
                case TabUsageType.Lists:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_Lists;
                    break;
                case TabUsageType.Related:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_Related;
                    break;
                case TabUsageType.UserTimeline:
                    LabelTabType.Text = Tween.My.Resources.TabUsageTypeName_UserTimeline;
                    break;
                default:
                    LabelTabType.Text = "UNKNOWN";
                    break;
            }
            ButtonRenameTab.Enabled = true;
            if (TabInformations.GetInstance().IsDefaultTab(tabName))
            {
                ButtonDeleteTab.Enabled = false;
            }
            else
            {
                ButtonDeleteTab.Enabled = true;
            }
            ButtonClose.Enabled = true;
        }

        public void SetCurrent(string TabName)
        {
            _cur = TabName;
        }

        public void AddNewFilter(string id, string msg)
        {
            //元フォームから直接呼ばれる
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
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
            TextSource.Text = "";
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
            ExUID.Text = "";
            ExUID.SelectAll();
            ExMSG1.Text = "";
            ExMSG1.SelectAll();
            ExMSG2.Text = "";
            ExMSG2.SelectAll();
            TextExSource.Text = "";
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
            _mode = EDITMODE.AddNew;
            _directAdd = true;
        }

        private void ButtonNew_Click(System.Object sender, System.EventArgs e)
        {
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonClose.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;
            ListFilters.Enabled = false;

            RadioAND.Checked = true;
            RadioPLUS.Checked = false;
            UID.Text = "";
            MSG1.Text = "";
            MSG2.Text = "";
            TextSource.Text = "";
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
            ExUID.Text = "";
            ExMSG1.Text = "";
            ExMSG2.Text = "";
            TextExSource.Text = "";
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
            _mode = EDITMODE.AddNew;
        }

        private void ButtonEdit_Click(System.Object sender, System.EventArgs e)
        {
            if (ListFilters.SelectedIndex == -1)
                return;

            ShowDetail();

            int idx = ListFilters.SelectedIndex;
            ListFilters.SelectedIndex = -1;
            ListFilters.SelectedIndex = idx;
            ListFilters.Enabled = false;

            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;

            _mode = EDITMODE.Edit;
        }

        private void ButtonDelete_Click(System.Object sender, System.EventArgs e)
        {
            if (ListFilters.SelectedIndex == -1)
                return;
            string tmp = "";
            System.Windows.Forms.DialogResult rslt = default(System.Windows.Forms.DialogResult);

            if (ListFilters.SelectedIndices.Count == 1)
            {
                tmp = string.Format(Tween.My.Resources.ButtonDelete_ClickText1, Constants.vbCrLf, ListFilters.SelectedItem.ToString());
                rslt = MessageBox.Show(tmp, Tween.My.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                tmp = string.Format(Tween.My.Resources.ButtonDelete_ClickText3, ListFilters.SelectedIndices.Count.ToString());
                rslt = MessageBox.Show(tmp, Tween.My.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            if (rslt == System.Windows.Forms.DialogResult.Cancel)
                return;

            for (int idx = ListFilters.Items.Count - 1; idx >= 0; idx += -1)
            {
                if (ListFilters.GetSelected(idx))
                {
                    _sts.Tabs[ListTabs.SelectedItem.ToString()].RemoveFilter((FiltersClass)ListFilters.Items[idx]);
                    ListFilters.Items.RemoveAt(idx);
                }
            }
        }

        private void ButtonCancel_Click(System.Object sender, System.EventArgs e)
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
            if (ListFilters.SelectedIndex > -1)
            {
                ButtonEdit.Enabled = true;
                ButtonDelete.Enabled = true;
                ButtonRuleUp.Enabled = true;
                ButtonRuleDown.Enabled = true;
                ButtonRuleCopy.Enabled = true;
                ButtonRuleMove.Enabled = true;
            }
            else
            {
                ButtonEdit.Enabled = false;
                ButtonDelete.Enabled = false;
                ButtonRuleUp.Enabled = false;
                ButtonRuleDown.Enabled = false;
                ButtonRuleCopy.Enabled = false;
                ButtonRuleMove.Enabled = false;
            }
            ButtonClose.Enabled = true;
            if (_directAdd)
            {
                this.Close();
            }
        }

        private void ShowDetail()
        {
            if (_directAdd)
                return;

            if (ListFilters.SelectedIndex > -1)
            {
                FiltersClass fc = (FiltersClass)ListFilters.SelectedItem;
                if (fc.SearchBoth)
                {
                    RadioAND.Checked = true;
                    RadioPLUS.Checked = false;
                    UID.Enabled = true;
                    MSG1.Enabled = true;
                    MSG2.Enabled = false;
                    UID.Text = fc.NameFilter;
                    UID.SelectAll();
                    MSG1.Text = "";
                    MSG2.Text = "";
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
                    UID.Text = "";
                    MSG1.Text = "";
                    MSG2.Text = "";
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
                    ExMSG1.Text = "";
                    ExMSG2.Text = "";
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
                    ExUID.Text = "";
                    ExMSG1.Text = "";
                    ExMSG2.Text = "";
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
                UID.Text = "";
                MSG1.Text = "";
                MSG2.Text = "";
                TextSource.Text = "";
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
                ExUID.Text = "";
                ExMSG1.Text = "";
                ExMSG2.Text = "";
                TextExSource.Text = "";
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

        private void RadioAND_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            bool flg = RadioAND.Checked;
            UID.Enabled = flg;
            MSG1.Enabled = flg;
            MSG2.Enabled = !flg;
        }

        private void ButtonOK_Click(System.Object sender, System.EventArgs e)
        {
            bool isBlankMatch = false;
            bool isBlankExclude = false;

            //入力チェック
            if (!CheckMatchRule(ref isBlankMatch) || !CheckExcludeRule(ref isBlankExclude))
            {
                return;
            }
            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(Tween.My.Resources.ButtonOK_ClickText1, Tween.My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int i = ListFilters.SelectedIndex;
            FiltersClass ft = null;

            ft = new FiltersClass();

            ft.MoveFrom = OptMove.Checked;
            ft.SetMark = CheckMark.Checked;

            string bdy = "";
            if (RadioAND.Checked)
            {
                ft.NameFilter = UID.Text;
                int cnt = My.MyProject.Forms.TweenMain.AtIdSupl.ItemCount;
                My.MyProject.Forms.TweenMain.AtIdSupl.AddItem("@" + ft.NameFilter);
                if (cnt != My.MyProject.Forms.TweenMain.AtIdSupl.ItemCount)
                {
                    My.MyProject.Forms.TweenMain.ModifySettingAtId = true;
                }
                ft.SearchBoth = true;
                bdy = MSG1.Text;
            }
            else
            {
                ft.NameFilter = "";
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
                string[] bf = bdy.Trim().Split(Strings.Chr(32));
                foreach (string bfs in bf)
                {
                    if (!string.IsNullOrEmpty(bfs))
                        ft.BodyFilter.Add(bfs.Trim());
                }
            }

            ft.UseRegex = CheckRegex.Checked;
            ft.SearchUrl = CheckURL.Checked;
            ft.CaseSensitive = CheckCaseSensitive.Checked;
            ft.IsRt = CheckRetweet.Checked;
            ft.UseLambda = CheckLambda.Checked;

            bdy = "";
            if (RadioExAnd.Checked)
            {
                ft.ExNameFilter = ExUID.Text;
                ft.ExSearchBoth = true;
                bdy = ExMSG1.Text;
            }
            else
            {
                ft.ExNameFilter = "";
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
                string[] bf = bdy.Trim().Split(Strings.Chr(32));
                foreach (string bfs in bf)
                {
                    if (!string.IsNullOrEmpty(bfs))
                        ft.ExBodyFilter.Add(bfs.Trim());
                }
            }

            ft.ExUseRegex = CheckExRegex.Checked;
            ft.ExSearchUrl = CheckExURL.Checked;
            ft.ExCaseSensitive = CheckExCaseSensitive.Checked;
            ft.IsExRt = CheckExRetweet.Checked;
            ft.ExUseLambda = CheckExLambDa.Checked;

            if (_mode == EDITMODE.AddNew)
            {
                if (!_sts.Tabs[ListTabs.SelectedItem.ToString()].AddFilter(ft))
                {
                    MessageBox.Show(Tween.My.Resources.ButtonOK_ClickText4, Tween.My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].EditFilter((FiltersClass)ListFilters.SelectedItem, ft);
            }

            SetFilters(ListTabs.SelectedItem.ToString());
            ListFilters.SelectedIndex = -1;
            if (_mode == EDITMODE.AddNew)
            {
                ListFilters.SelectedIndex = ListFilters.Items.Count - 1;
            }
            else
            {
                ListFilters.SelectedIndex = i;
            }
            _mode = EDITMODE.None;

            if (_directAdd)
            {
                this.Close();
            }
        }

        private bool IsValidLambdaExp(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            try
            {
                LambdaExpression expr = null;
                expr = DynamicExpression.ParseLambda(text, new PostClass());
            }
            catch (ParseException ex)
            {
                MessageBox.Show(Tween.My.Resources.IsValidLambdaExpText1 + ex.Message, Tween.My.Resources.IsValidLambdaExpText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private bool IsValidRegexp(string text)
        {
            try
            {
                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Tween.My.Resources.ButtonOK_ClickText3 + ex.Message, Tween.My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private bool CheckMatchRule(ref bool isBlank)
        {
            isBlank = false;
            TextSource.Text = TextSource.Text.Trim();
            if (RadioAND.Checked)
            {
                MSG1.Text = MSG1.Text.Trim();
                UID.Text = UID.Text.Trim();
                if (!CheckRegex.Checked && !CheckLambda.Checked)
                    MSG1.Text = MSG1.Text.Replace("　", " ");

                if (string.IsNullOrEmpty(UID.Text) && string.IsNullOrEmpty(MSG1.Text) && string.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
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
                    MSG2.Text = MSG2.Text.Replace("　", " ");
                if (string.IsNullOrEmpty(MSG2.Text) && string.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckLambda.Checked && !IsValidLambdaExp(MSG2.Text))
                {
                    return false;
                }
                else if (CheckRegex.Checked && !IsValidRegexp(MSG2.Text))
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

        private bool CheckExcludeRule(ref bool isBlank)
        {
            isBlank = false;
            TextExSource.Text = TextExSource.Text.Trim();
            if (RadioExAnd.Checked)
            {
                ExMSG1.Text = ExMSG1.Text.Trim();
                if (!CheckExRegex.Checked && !CheckExLambDa.Checked)
                    ExMSG1.Text = ExMSG1.Text.Replace("　", " ");
                ExUID.Text = ExUID.Text.Trim();
                if (string.IsNullOrEmpty(ExUID.Text) && string.IsNullOrEmpty(ExMSG1.Text) && string.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
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
                    ExMSG2.Text = ExMSG2.Text.Replace("　", " ");
                if (string.IsNullOrEmpty(ExMSG2.Text) && string.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckExLambDa.Checked && !IsValidLambdaExp(ExMSG2.Text))
                {
                    return false;
                }
                else if (CheckExRegex.Checked && !IsValidRegexp(ExMSG2.Text))
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

        private void ListFilters_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            ShowDetail();
        }

        private void ButtonClose_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FilterDialog_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _directAdd = false;
        }

        private void FilterDialog_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (EditFilterGroup.Enabled)
                {
                    ButtonCancel_Click(null, null);
                }
                else
                {
                    ButtonClose_Click(null, null);
                }
            }
        }

        private void ListFilters_DoubleClick(System.Object sender, System.EventArgs e)
        {
            if (ListFilters.SelectedItem == null)
            {
                return;
            }

            if (ListFilters.IndexFromPoint(ListFilters.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            if (ListFilters.Items[ListFilters.IndexFromPoint(ListFilters.PointToClient(Control.MousePosition))] == null)
            {
                return;
            }
            ButtonEdit_Click(sender, e);
        }

        private void FilterDialog_Shown(object sender, System.EventArgs e)
        {
            _sts = TabInformations.GetInstance();
            ListTabs.Items.Clear();
            foreach (string key in _sts.Tabs.Keys)
            {
                ListTabs.Items.Add(key);
            }
            SetTabnamesToDialog();

            ComboSound.Items.Clear();
            ComboSound.Items.Add("");
            System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo(Tween.My.MyProject.Application.Info.DirectoryPath + System.IO.Path.DirectorySeparatorChar);
            if (System.IO.Directory.Exists(System.IO.Path.Combine(Tween.My.MyProject.Application.Info.DirectoryPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (System.IO.FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboSound.Items.Add(oFile.Name);
            }

            idlist.Clear();
            foreach (string tmp in My.MyProject.Forms.TweenMain.AtIdSupl.GetItemList())
            {
                idlist.Add(tmp.Remove(0, 1));
                // @文字削除
            }
            UID.AutoCompleteCustomSource.Clear();
            UID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            ExUID.AutoCompleteCustomSource.Clear();
            ExUID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            //選択タブ変更
            if (ListTabs.Items.Count > 0)
            {
                if (_cur.Length > 0)
                {
                    for (int i = 0; i <= ListTabs.Items.Count - 1; i++)
                    {
                        if (_cur == ListTabs.Items[i].ToString())
                        {
                            ListTabs.SelectedIndex = i;
                            //tabdialog.TabList.Items.Remove(_cur)
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
            }
        }

        private void SetTabnamesToDialog()
        {
            tabdialog.ClearTab();
            foreach (string key in _sts.Tabs.Keys)
            {
                if (TabInformations.GetInstance().IsDistributableTab(key))
                    tabdialog.AddTab(key);
            }
        }

        private void ListTabs_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1)
            {
                SetFilters(ListTabs.SelectedItem.ToString());
            }
            else
            {
                ListFilters.Items.Clear();
            }
        }

        private void ButtonAddTab_Click(System.Object sender, System.EventArgs e)
        {
            string tabName = null;
            TabUsageType tabType = default(TabUsageType);
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = _sts.GetUniqueTabName();
                inputName.IsShowUsage = true;
                inputName.ShowDialog();
                if (inputName.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                    return;
                tabName = inputName.TabName;
                tabType = inputName.Usage;
            }
            if (!string.IsNullOrEmpty(tabName))
            {
                //List対応
                ListElement list = null;
                if (tabType == TabUsageType.Lists)
                {
                    string rslt = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
                    if (!string.IsNullOrEmpty(rslt))
                    {
                        MessageBox.Show("Failed to get lists. (" + rslt + ")");
                    }
                    using (ListAvailable listAvail = new ListAvailable())
                    {
                        if (listAvail.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                            return;
                        if (listAvail.SelectedList == null)
                            return;
                        list = listAvail.SelectedList;
                    }
                }
                if (!_sts.AddTab(tabName, tabType, list) || !((TweenMain)this.Owner).AddNewTab(tabName, false, tabType, list))
                {
                    string tmp = string.Format(Tween.My.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Tween.My.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    //成功
                    ListTabs.Items.Add(tabName);
                    SetTabnamesToDialog();
                }
            }
        }

        private void ButtonDeleteTab_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
                {
                    ListTabs.Items.RemoveAt(idx);
                    idx -= 1;
                    if (idx < 0)
                        idx = 0;
                    ListTabs.SelectedIndex = idx;
                    SetTabnamesToDialog();
                }
            }
        }

        private void ButtonRenameTab_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).TabRename(ref tb))
                {
                    ListTabs.Items.RemoveAt(idx);
                    ListTabs.Items.Insert(idx, tb);
                    ListTabs.SelectedIndex = idx;
                    SetTabnamesToDialog();
                }
            }
        }

        private void CheckManageRead_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                ((TweenMain)this.Owner).ChangeTabUnreadManage(ListTabs.SelectedItem.ToString(), CheckManageRead.Checked);
            }
        }

        private void ButtonUp_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > 0 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex - 1].ToString();
                ((TweenMain)this.Owner).ReOrderTab(selName, tgtName, true);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx - 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void ButtonDown_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListTabs.SelectedIndex < ListTabs.Items.Count - 1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex + 1].ToString();
                ((TweenMain)this.Owner).ReOrderTab(selName, tgtName, false);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx + 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void CheckNotifyNew_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].Notify = CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string filename = "";
                if (ComboSound.SelectedIndex > -1)
                    filename = ComboSound.SelectedItem.ToString();
                _sts.Tabs[ListTabs.SelectedItem.ToString()].SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            bool flg = RadioExAnd.Checked;
            ExUID.Enabled = flg;
            ExMSG1.Enabled = flg;
            ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            CheckMark.Enabled = !OptMove.Checked;
        }

        private void ButtonRuleUp_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null && ListFilters.SelectedIndex > 0)
            {
                string tabname = ListTabs.SelectedItem.ToString();
                FiltersClass selected = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex];
                FiltersClass target = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex - 1];
                int idx = ListFilters.SelectedIndex;
                ListFilters.Items.RemoveAt(idx - 1);
                ListFilters.Items.Insert(idx, target);
                _sts.Tabs[tabname].Filters.RemoveAt(idx - 1);
                _sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleDown_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null && ListFilters.SelectedIndex < ListFilters.Items.Count - 1)
            {
                string tabname = ListTabs.SelectedItem.ToString();
                FiltersClass selected = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex];
                FiltersClass target = _sts.Tabs[tabname].Filters[ListFilters.SelectedIndex + 1];
                int idx = ListFilters.SelectedIndex;
                ListFilters.Items.RemoveAt(idx + 1);
                ListFilters.Items.Insert(idx, target);
                _sts.Tabs[tabname].Filters.RemoveAt(idx + 1);
                _sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleCopy_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                tabdialog.Text = Tween.My.Resources.ButtonRuleCopy_ClickText1;
                if (tabdialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                string tabname = ListTabs.SelectedItem.ToString();
                StringCollection tabs = tabdialog.SelectedTabNames;
                System.Collections.Generic.List<FiltersClass> filters = new System.Collections.Generic.List<FiltersClass>();

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

        private void ButtonRuleMove_Click(System.Object sender, System.EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                tabdialog.Text = Tween.My.Resources.ButtonRuleMove_ClickText1;
                if (tabdialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                string tabname = ListTabs.SelectedItem.ToString();
                StringCollection tabs = tabdialog.SelectedTabNames;
                System.Collections.Generic.List<FiltersClass> filters = new System.Collections.Generic.List<FiltersClass>();

                foreach (int idx in ListFilters.SelectedIndices)
                {
                    filters.Add(_sts.Tabs[tabname].Filters[idx].CopyTo(new FiltersClass()));
                }
                if (tabs.Count == 1 && tabs[0] == tabname)
                    return;
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

        private void FilterTextBox_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && e.Modifiers == (Keys.Shift | Keys.Control))
            {
                TweenMain main = (TweenMain)this.Owner;
                TextBox tbox = (TextBox)sender;
                if (tbox.SelectionStart > 0)
                {
                    int endidx = tbox.SelectionStart - 1;
                    string startstr = "";
                    for (int i = tbox.SelectionStart - 1; i >= 0; i += -1)
                    {
                        char c = tbox.Text[i];
                        if (char.IsLetterOrDigit(c) || c == "_")
                        {
                            continue;
                        }
                        if (c == "@")
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.AtIdSupl, startstr.Length + 1, startstr);
                        }
                        else if (c == "#")
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.HashSupl, startstr.Length + 1, startstr);
                        }
                        else
                        {
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        private void FilterTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            TweenMain main = (TweenMain)this.Owner;
            TextBox tbox = (TextBox)sender;
            if (e.KeyChar == "@")
            {
                //If Not SettingDialog.UseAtIdSupplement Then Exit Sub
                //@マーク
                main.ShowSuplDialog(tbox, main.AtIdSupl);
                e.Handled = true;
            }
            else if (e.KeyChar == "#")
            {
                //If Not SettingDialog.UseHashSupplement Then Exit Sub
                main.ShowSuplDialog(tbox, main.HashSupl);
                e.Handled = true;
            }
        }

        public FilterDialog()
        {
            Shown += FilterDialog_Shown;
            KeyDown += FilterDialog_KeyDown;
            FormClosed += FilterDialog_FormClosed;
            InitializeComponent();
        }
    }
}