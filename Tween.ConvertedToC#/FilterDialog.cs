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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Hoehoe
{
    public partial class FilterDialog
    {
        private EDITMODE _mode;
        private bool _directAdd;
        private TabInformations _sts;
        private string _cur;
        private List<string> _idList = new List<string>();
        private TabsDialog _tabDialog = new TabsDialog(true);

        private enum EDITMODE
        {
            AddNew,
            Edit,
            None
        }

        private void SetFilters(string tabName)
        {
            if (ListTabs.Items.Count == 0)
            {
                return;
            }

            ListFilters.Items.Clear();
            ListFilters.Items.AddRange(_sts.Tabs[tabName].GetFilters());
            if (ListFilters.Items.Count > 0)
            {
                ListFilters.SelectedIndex = 0;
            }
            CheckManageRead.Checked = _sts.Tabs[tabName].UnreadManage;
            CheckNotifyNew.Checked = _sts.Tabs[tabName].Notify;

            int idx = ComboSound.Items.IndexOf(_sts.Tabs[tabName].SoundFile);
            if (idx == -1)
            {
                idx = 0;
            }
            ComboSound.SelectedIndex = idx;

            if (_directAdd)
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
            switch (TabInformations.GetInstance().Tabs[tabName].TabType)
            {
                case MyCommon.TabUsageType.Home:
                case MyCommon.TabUsageType.DirectMessage:
                case MyCommon.TabUsageType.Favorites:
                case MyCommon.TabUsageType.PublicSearch:
                case MyCommon.TabUsageType.Lists:
                case MyCommon.TabUsageType.Related:
                case MyCommon.TabUsageType.UserTimeline:
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
                case MyCommon.TabUsageType.Home:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_Home;
                    break;
                case MyCommon.TabUsageType.Mentions:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_Mentions;
                    break;
                case MyCommon.TabUsageType.DirectMessage:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_DirectMessage;
                    break;
                case MyCommon.TabUsageType.Favorites:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_Favorites;
                    break;
                case MyCommon.TabUsageType.UserDefined:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_UserDefined;
                    break;
                case MyCommon.TabUsageType.PublicSearch:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_PublicSearch;
                    break;
                case MyCommon.TabUsageType.Lists:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_Lists;
                    break;
                case MyCommon.TabUsageType.Related:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_Related;
                    break;
                case MyCommon.TabUsageType.UserTimeline:
                    LabelTabType.Text = Hoehoe.Properties.Resources.TabUsageTypeName_UserTimeline;
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
            closeButton.Enabled = true;
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

        private void ButtonNew_Click(object sender, EventArgs e)
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

        private void ButtonEdit_Click(object sender, EventArgs e)
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

            _mode = EDITMODE.Edit;
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (ListFilters.SelectedIndex == -1)
            {
                return;
            }
            string tmp = "";
            DialogResult rslt = default(DialogResult);

            if (ListFilters.SelectedIndices.Count == 1)
            {
                tmp = String.Format(Hoehoe.Properties.Resources.ButtonDelete_ClickText1, Environment.NewLine, ListFilters.SelectedItem.ToString());
                rslt = MessageBox.Show(tmp, Hoehoe.Properties.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                tmp = String.Format(Hoehoe.Properties.Resources.ButtonDelete_ClickText3, ListFilters.SelectedIndices.Count.ToString());
                rslt = MessageBox.Show(tmp, Hoehoe.Properties.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
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

        private void ButtonCancel_Click(object sender, EventArgs e)
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
            closeButton.Enabled = true;
            if (_directAdd)
            {
                this.Close();
            }
        }

        private void ShowDetail()
        {
            if (_directAdd)
            {
                return;
            }

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

        private void RadioAND_CheckedChanged(object sender, EventArgs e)
        {
            bool flg = RadioAND.Checked;
            UID.Enabled = flg;
            MSG1.Enabled = flg;
            MSG2.Enabled = !flg;
        }

        private TweenMain TwMain { get { return (TweenMain)this.Owner; } }

        private void ButtonOK_Click(object sender, EventArgs e)
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
                MessageBox.Show(Hoehoe.Properties.Resources.ButtonOK_ClickText1, Hoehoe.Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int prevSelectedIndex = ListFilters.SelectedIndex;
            FiltersClass ft = new FiltersClass() { MoveFrom = OptMove.Checked, SetMark = CheckMark.Checked };

            string bdy = "";
            if (RadioAND.Checked)
            {
                ft.NameFilter = UID.Text;
                int cnt = TwMain.AtIdSupl.ItemCount;
                TwMain.AtIdSupl.AddItem("@" + ft.NameFilter);
                if (cnt != TwMain.AtIdSupl.ItemCount)
                {
                    TwMain.SetModifySettingAtId(true);
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
                string[] bf = bdy.Trim().Split(' ');
                foreach (string bfs in bf)
                {
                    if (!String.IsNullOrEmpty(bfs))
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
                string[] bf = bdy.Trim().Split(' ');
                foreach (string bfs in bf)
                {
                    if (!String.IsNullOrEmpty(bfs))
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

            if (_mode == EDITMODE.AddNew)
            {
                if (!_sts.Tabs[ListTabs.SelectedItem.ToString()].AddFilter(ft))
                {
                    MessageBox.Show(Hoehoe.Properties.Resources.ButtonOK_ClickText4, Hoehoe.Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ListFilters.SelectedIndex = prevSelectedIndex;
            }
            _mode = EDITMODE.None;

            if (_directAdd)
            {
                this.Close();
            }
        }

        private bool IsValidLambdaExp(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return true;
            }
            try
            {
                LambdaExpression expr = DynamicExpression.ParseLambda<PostClass, bool>(text, new PostClass());
            }
            catch (ParseException ex)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.IsValidLambdaExpText1 + ex.Message, Hoehoe.Properties.Resources.IsValidLambdaExpText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private bool IsValidRegexp(string text)
        {
            try
            {
                Regex rgx = new System.Text.RegularExpressions.Regex(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Hoehoe.Properties.Resources.ButtonOK_ClickText3 + ex.Message, Hoehoe.Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                {
                    MSG1.Text = MSG1.Text.Replace("　", " ");
                }

                if (String.IsNullOrEmpty(UID.Text) && String.IsNullOrEmpty(MSG1.Text) && String.IsNullOrEmpty(TextSource.Text) && !CheckRetweet.Checked)
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
                if (String.IsNullOrEmpty(MSG2.Text) && String.IsNullOrEmpty(TextSource.Text) && !CheckRetweet.Checked)
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
                {
                    ExMSG1.Text = ExMSG1.Text.Replace("　", " ");
                }
                ExUID.Text = ExUID.Text.Trim();
                if (String.IsNullOrEmpty(ExUID.Text) && String.IsNullOrEmpty(ExMSG1.Text) && String.IsNullOrEmpty(TextExSource.Text) && !CheckExRetweet.Checked)
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
                if (String.IsNullOrEmpty(ExMSG2.Text) && String.IsNullOrEmpty(TextExSource.Text) && !CheckExRetweet.Checked)
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

        private void ListFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetail();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FilterDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _directAdd = false;
        }

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
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

        private void ListFilters_DoubleClick(object sender, EventArgs e)
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

        private void FilterDialog_Shown(object sender, EventArgs e)
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
            DirectoryInfo oDir = new DirectoryInfo(MyCommon.GetAppDir() + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(MyCommon.GetAppDir(), "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboSound.Items.Add(oFile.Name);
            }

            _idList.Clear();
            foreach (string tmp in TwMain.AtIdSupl.GetItemList())
            {
                _idList.Add(tmp.Remove(0, 1));
                // @文字削除
            }
            UID.AutoCompleteCustomSource.Clear();
            UID.AutoCompleteCustomSource.AddRange(_idList.ToArray());

            ExUID.AutoCompleteCustomSource.Clear();
            ExUID.AutoCompleteCustomSource.AddRange(_idList.ToArray());

            //選択タブ変更
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

        private void SetTabnamesToDialog()
        {
            _tabDialog.ClearTab();
            foreach (string key in _sts.Tabs.Keys)
            {
                if (TabInformations.GetInstance().IsDistributableTab(key))
                {
                    _tabDialog.AddTab(key);
                }
            }
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
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

        private void ButtonAddTab_Click(object sender, EventArgs e)
        {
            string tabName = null;
            MyCommon.TabUsageType tabType = default(MyCommon.TabUsageType);
            using (InputTabName inputName = new InputTabName())
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
            if (!String.IsNullOrEmpty(tabName))
            {
                //List対応
                ListElement list = null;
                if (tabType == MyCommon.TabUsageType.Lists)
                {
                    string rslt = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
                    if (!String.IsNullOrEmpty(rslt))
                    {
                        MessageBox.Show("Failed to get lists. (" + rslt + ")");
                    }
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
                if (!_sts.AddTab(tabName, tabType, list) || !((TweenMain)this.Owner).AddNewTab(tabName, false, tabType, list))
                {
                    string tmp = String.Format(Hoehoe.Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Hoehoe.Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void ButtonDeleteTab_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
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

        private void ButtonRenameTab_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
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

        private void CheckManageRead_CheckedChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                ((TweenMain)this.Owner).ChangeTabUnreadManage(ListTabs.SelectedItem.ToString(), CheckManageRead.Checked);
            }
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > 0 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex - 1].ToString();
                ((TweenMain)this.Owner).ReOrderTab(selName, tgtName, true);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx - 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListTabs.SelectedIndex < ListTabs.Items.Count - 1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string selName = ListTabs.SelectedItem.ToString();
                string tgtName = ListTabs.Items[ListTabs.SelectedIndex + 1].ToString();
                ((TweenMain)this.Owner).ReOrderTab(selName, tgtName, false);
                int idx = ListTabs.SelectedIndex;
                ListTabs.Items.RemoveAt(idx + 1);
                ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].Notify = CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !String.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string filename = "";
                if (ComboSound.SelectedIndex > -1)
                {
                    filename = ComboSound.SelectedItem.ToString();
                }
                _sts.Tabs[ListTabs.SelectedItem.ToString()].SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            bool flg = RadioExAnd.Checked;
            ExUID.Enabled = flg;
            ExMSG1.Enabled = flg;
            ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
        {
            CheckMark.Enabled = !OptMove.Checked;
        }

        private void ButtonRuleUp_Click(object sender, EventArgs e)
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

        private void ButtonRuleDown_Click(object sender, EventArgs e)
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

        private void ButtonRuleCopy_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                _tabDialog.Text = Hoehoe.Properties.Resources.ButtonRuleCopy_ClickText1;
                if (_tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string tabname = ListTabs.SelectedItem.ToString();
                StringCollection tabs = _tabDialog.SelectedTabNames;
                List<FiltersClass> filters = new List<FiltersClass>();

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

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                _tabDialog.Text = Hoehoe.Properties.Resources.ButtonRuleMove_ClickText1;
                if (_tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string tabname = ListTabs.SelectedItem.ToString();
                StringCollection tabs = _tabDialog.SelectedTabNames;
                List<FiltersClass> filters = new List<FiltersClass>();

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

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
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
                        if (char.IsLetterOrDigit(c) || c == '_')
                        {
                            continue;
                        }
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
            TweenMain main = (TweenMain)this.Owner;
            TextBox tbox = (TextBox)sender;
            if (e.KeyChar == '@')
            {
                //@マーク
                main.ShowSuplDialog(tbox, main.AtIdSupl);
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                main.ShowSuplDialog(tbox, main.HashSupl);
                e.Handled = true;
            }
        }

        public FilterDialog()
        {
            InitializeComponent();
        }
    }
}