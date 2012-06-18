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
    using R = Hoehoe.Properties.Resources;

    public partial class FilterDialog
    {
        #region privates

        private EDITMODE editMode;
        private bool isDirectAdd;
        private TabInformations sts;
        private string cur;
        private List<string> idList = new List<string>();
        private TabsDialog tabDialog = new TabsDialog(true);

        #endregion privates

        #region constructor

        public FilterDialog()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region enums

        private enum EDITMODE
        {
            AddNew,
            Edit,
            None
        }

        #endregion enums

        #region properties

        private TweenMain TwMain
        {
            get { return (TweenMain)this.Owner; }
        }

        #endregion properties

        #region public methods

        public void SetCurrent(string tabName)
        {
            this.cur = tabName;
        }

        public void AddNewFilter(string id, string msg)
        {
            // 元フォームから直接呼ばれる
            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.closeButton.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;
            this.ListFilters.Enabled = false;

            this.RadioAND.Checked = true;
            this.RadioPLUS.Checked = false;
            this.UID.Text = id;
            this.UID.SelectAll();
            this.MSG1.Text = msg;
            this.MSG1.SelectAll();
            this.MSG2.Text = id + msg;
            this.MSG2.SelectAll();
            this.TextSource.Text = string.Empty;
            this.UID.Enabled = true;
            this.MSG1.Enabled = true;
            this.MSG2.Enabled = false;
            this.CheckRegex.Checked = false;
            this.CheckURL.Checked = false;
            this.CheckCaseSensitive.Checked = false;
            this.CheckRetweet.Checked = false;
            this.CheckLambda.Checked = false;

            this.RadioExAnd.Checked = true;
            this.RadioExPLUS.Checked = false;
            this.ExUID.Text = string.Empty;
            this.ExUID.SelectAll();
            this.ExMSG1.Text = string.Empty;
            this.ExMSG1.SelectAll();
            this.ExMSG2.Text = string.Empty;
            this.ExMSG2.SelectAll();
            this.TextExSource.Text = string.Empty;
            this.ExUID.Enabled = true;
            this.ExMSG1.Enabled = true;
            this.ExMSG2.Enabled = false;
            this.CheckExRegex.Checked = false;
            this.CheckExURL.Checked = false;
            this.CheckExCaseSensitive.Checked = false;
            this.CheckExRetweet.Checked = false;
            this.CheckExLambDa.Checked = false;

            this.OptCopy.Checked = true;
            this.CheckMark.Checked = true;
            this.UID.Focus();
            this.editMode = EDITMODE.AddNew;
            this.isDirectAdd = true;
        }

        #endregion public methods

        #region event handler

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.closeButton.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.closeButton.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;
            this.ListFilters.Enabled = false;

            this.RadioAND.Checked = true;
            this.RadioPLUS.Checked = false;
            this.UID.Text = string.Empty;
            this.MSG1.Text = string.Empty;
            this.MSG2.Text = string.Empty;
            this.TextSource.Text = string.Empty;
            this.UID.Enabled = true;
            this.MSG1.Enabled = true;
            this.MSG2.Enabled = false;
            this.CheckRegex.Checked = false;
            this.CheckURL.Checked = false;
            this.CheckCaseSensitive.Checked = false;
            this.CheckRetweet.Checked = false;
            this.CheckLambda.Checked = false;

            this.RadioExAnd.Checked = true;
            this.RadioExPLUS.Checked = false;
            this.ExUID.Text = string.Empty;
            this.ExMSG1.Text = string.Empty;
            this.ExMSG2.Text = string.Empty;
            this.TextExSource.Text = string.Empty;
            this.ExUID.Enabled = true;
            this.ExMSG1.Enabled = true;
            this.ExMSG2.Enabled = false;
            this.CheckExRegex.Checked = false;
            this.CheckExURL.Checked = false;
            this.CheckExCaseSensitive.Checked = false;
            this.CheckExRetweet.Checked = false;
            this.CheckExLambDa.Checked = false;

            this.OptCopy.Checked = true;
            this.CheckMark.Checked = true;
            this.UID.Focus();
            this.editMode = EDITMODE.AddNew;
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.ListFilters.SelectedIndex == -1)
            {
                return;
            }

            this.ShowDetail();

            int idx = this.ListFilters.SelectedIndex;
            this.ListFilters.SelectedIndex = -1;
            this.ListFilters.SelectedIndex = idx;
            this.ListFilters.Enabled = false;

            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.closeButton.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;

            this.editMode = EDITMODE.Edit;
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.ListFilters.SelectedIndex == -1)
            {
                return;
            }

            string tmp = string.Empty;
            DialogResult rslt = default(DialogResult);

            if (this.ListFilters.SelectedIndices.Count == 1)
            {
                tmp = string.Format(R.ButtonDelete_ClickText1, Environment.NewLine, this.ListFilters.SelectedItem.ToString());
                rslt = MessageBox.Show(tmp, R.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                tmp = string.Format(R.ButtonDelete_ClickText3, this.ListFilters.SelectedIndices.Count.ToString());
                rslt = MessageBox.Show(tmp, R.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            if (rslt == DialogResult.Cancel)
            {
                return;
            }

            for (int idx = this.ListFilters.Items.Count - 1; idx >= 0; idx--)
            {
                if (this.ListFilters.GetSelected(idx))
                {
                    this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].RemoveFilter((FiltersClass)this.ListFilters.Items[idx]);
                    this.ListFilters.Items.RemoveAt(idx);
                }
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.ListTabs.Enabled = true;
            this.GroupTab.Enabled = true;
            this.ListFilters.Enabled = true;
            this.ListFilters.Focus();
            if (this.ListFilters.SelectedIndex != -1)
            {
                this.ShowDetail();
            }

            this.EditFilterGroup.Enabled = false;
            this.ButtonNew.Enabled = true;
            if (this.ListFilters.SelectedIndex > -1)
            {
                this.ButtonEdit.Enabled = true;
                this.ButtonDelete.Enabled = true;
                this.ButtonRuleUp.Enabled = true;
                this.ButtonRuleDown.Enabled = true;
                this.ButtonRuleCopy.Enabled = true;
                this.ButtonRuleMove.Enabled = true;
            }
            else
            {
                this.ButtonEdit.Enabled = false;
                this.ButtonDelete.Enabled = false;
                this.ButtonRuleUp.Enabled = false;
                this.ButtonRuleDown.Enabled = false;
                this.ButtonRuleCopy.Enabled = false;
                this.ButtonRuleMove.Enabled = false;
            }

            this.closeButton.Enabled = true;
            if (this.isDirectAdd)
            {
                this.Close();
            }
        }

        private void RadioAND_CheckedChanged(object sender, EventArgs e)
        {
            bool flg = this.RadioAND.Checked;
            this.UID.Enabled = flg;
            this.MSG1.Enabled = flg;
            this.MSG2.Enabled = !flg;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            bool isBlankMatch = false;
            bool isBlankExclude = false;

            // 入力チェック
            if (!this.CheckMatchRule(ref isBlankMatch) || !this.CheckExcludeRule(ref isBlankExclude))
            {
                return;
            }

            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(R.ButtonOK_ClickText1, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int prevSelectedIndex = this.ListFilters.SelectedIndex;
            FiltersClass ft = new FiltersClass() { MoveFrom = this.OptMove.Checked, SetMark = this.CheckMark.Checked };

            string bdy = string.Empty;
            if (this.RadioAND.Checked)
            {
                ft.NameFilter = this.UID.Text;
                int cnt = this.TwMain.AtIdSupl.ItemCount;
                this.TwMain.AtIdSupl.AddItem("@" + ft.NameFilter);
                if (cnt != this.TwMain.AtIdSupl.ItemCount)
                {
                    this.TwMain.SetModifySettingAtId(true);
                }

                ft.SearchBoth = true;
                bdy = this.MSG1.Text;
            }
            else
            {
                ft.NameFilter = string.Empty;
                ft.SearchBoth = false;
                bdy = this.MSG2.Text;
            }

            ft.Source = this.TextSource.Text.Trim();

            if (this.CheckRegex.Checked || this.CheckLambda.Checked)
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

            ft.UseRegex = this.CheckRegex.Checked;
            ft.SearchUrl = this.CheckURL.Checked;
            ft.CaseSensitive = this.CheckCaseSensitive.Checked;
            ft.IsRt = this.CheckRetweet.Checked;
            ft.UseLambda = this.CheckLambda.Checked;

            bdy = string.Empty;
            if (this.RadioExAnd.Checked)
            {
                ft.ExNameFilter = this.ExUID.Text;
                ft.ExSearchBoth = true;
                bdy = this.ExMSG1.Text;
            }
            else
            {
                ft.ExNameFilter = string.Empty;
                ft.ExSearchBoth = false;
                bdy = this.ExMSG2.Text;
            }

            ft.ExSource = this.TextExSource.Text.Trim();

            if (this.CheckExRegex.Checked || this.CheckExLambDa.Checked)
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

            ft.ExUseRegex = this.CheckExRegex.Checked;
            ft.ExSearchUrl = this.CheckExURL.Checked;
            ft.ExCaseSensitive = this.CheckExCaseSensitive.Checked;
            ft.IsExRt = this.CheckExRetweet.Checked;
            ft.ExUseLambda = this.CheckExLambDa.Checked;

            if (this.editMode == EDITMODE.AddNew)
            {
                if (!this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].AddFilter(ft))
                {
                    MessageBox.Show(R.ButtonOK_ClickText4, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].EditFilter((FiltersClass)this.ListFilters.SelectedItem, ft);
            }

            this.SetFilters(this.ListTabs.SelectedItem.ToString());
            this.ListFilters.SelectedIndex = -1;
            if (this.editMode == EDITMODE.AddNew)
            {
                this.ListFilters.SelectedIndex = this.ListFilters.Items.Count - 1;
            }
            else
            {
                this.ListFilters.SelectedIndex = prevSelectedIndex;
            }

            this.editMode = EDITMODE.None;

            if (this.isDirectAdd)
            {
                this.Close();
            }
        }

        private void ListFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowDetail();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FilterDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.isDirectAdd = false;
        }

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.EditFilterGroup.Enabled)
                {
                    this.ButtonCancel_Click(null, null);
                }
                else
                {
                    this.ButtonClose_Click(null, null);
                }
            }
        }

        private void ListFilters_DoubleClick(object sender, EventArgs e)
        {
            if (this.ListFilters.SelectedItem == null)
            {
                return;
            }

            if (this.ListFilters.IndexFromPoint(this.ListFilters.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            if (this.ListFilters.Items[this.ListFilters.IndexFromPoint(this.ListFilters.PointToClient(Control.MousePosition))] == null)
            {
                return;
            }

            this.ButtonEdit_Click(sender, e);
        }

        private void FilterDialog_Shown(object sender, EventArgs e)
        {
            this.sts = TabInformations.GetInstance();
            this.ListTabs.Items.Clear();
            foreach (string key in this.sts.Tabs.Keys)
            {
                this.ListTabs.Items.Add(key);
            }

            this.SetTabnamesToDialog();

            this.ComboSound.Items.Clear();
            this.ComboSound.Items.Add(string.Empty);
            var names = MyCommon.GetSoundFileNames();
            if (names.Length > 0)
            {
                this.ComboSound.Items.AddRange(names);
            }

            this.idList.Clear();
            foreach (string tmp in this.TwMain.AtIdSupl.GetItemList())
            {
                this.idList.Add(tmp.Remove(0, 1)); // @文字削除
            }

            this.UID.AutoCompleteCustomSource.Clear();
            this.UID.AutoCompleteCustomSource.AddRange(this.idList.ToArray());

            this.ExUID.AutoCompleteCustomSource.Clear();
            this.ExUID.AutoCompleteCustomSource.AddRange(this.idList.ToArray());

            // 選択タブ変更
            if (this.ListTabs.Items.Count > 0)
            {
                if (this.cur.Length > 0)
                {
                    for (int i = 0; i < this.ListTabs.Items.Count; i++)
                    {
                        if (this.cur == this.ListTabs.Items[i].ToString())
                        {
                            this.ListTabs.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1)
            {
                this.SetFilters(this.ListTabs.SelectedItem.ToString());
            }
            else
            {
                this.ListFilters.Items.Clear();
            }
        }

        private void ButtonAddTab_Click(object sender, EventArgs e)
        {
            string tabName = null;
            TabUsageType tabType = default(TabUsageType);
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = this.sts.GetUniqueTabName();
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
                    string rslt = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
                    if (!string.IsNullOrEmpty(rslt))
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

                if (!this.sts.AddTab(tabName, tabType, list) || !((TweenMain)this.Owner).AddNewTab(tabName, false, tabType, list))
                {
                    string tmp = string.Format(R.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, R.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    // 成功
                    this.ListTabs.Items.Add(tabName);
                    this.SetTabnamesToDialog();
                }
            }
        }

        private void ButtonDeleteTab_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                string tb = this.ListTabs.SelectedItem.ToString();
                int idx = this.ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
                {
                    this.ListTabs.Items.RemoveAt(idx);
                    idx -= 1;
                    if (idx < 0)
                    {
                        idx = 0;
                    }

                    this.ListTabs.SelectedIndex = idx;
                    this.SetTabnamesToDialog();
                }
            }
        }

        private void ButtonRenameTab_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                string tb = this.ListTabs.SelectedItem.ToString();
                int idx = this.ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RenameTab(ref tb))
                {
                    this.ListTabs.Items.RemoveAt(idx);
                    this.ListTabs.Items.Insert(idx, tb);
                    this.ListTabs.SelectedIndex = idx;
                    this.SetTabnamesToDialog();
                }
            }
        }

        private void CheckManageRead_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                ((TweenMain)this.Owner).ChangeTabUnreadManage(this.ListTabs.SelectedItem.ToString(), this.CheckManageRead.Checked);
            }
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > 0 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                string selName = this.ListTabs.SelectedItem.ToString();
                string tgtName = this.ListTabs.Items[this.ListTabs.SelectedIndex - 1].ToString();
                ((TweenMain)this.Owner).ReorderTab(selName, tgtName, true);
                int idx = this.ListTabs.SelectedIndex;
                this.ListTabs.Items.RemoveAt(idx - 1);
                this.ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && this.ListTabs.SelectedIndex < this.ListTabs.Items.Count - 1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                string selName = this.ListTabs.SelectedItem.ToString();
                string tgtName = this.ListTabs.Items[this.ListTabs.SelectedIndex + 1].ToString();
                ((TweenMain)this.Owner).ReorderTab(selName, tgtName, false);
                int idx = this.ListTabs.SelectedIndex;
                this.ListTabs.Items.RemoveAt(idx + 1);
                this.ListTabs.Items.Insert(idx, tgtName);
            }
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].Notify = this.CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(this.ListTabs.SelectedItem.ToString()))
            {
                string filename = string.Empty;
                if (this.ComboSound.SelectedIndex > -1)
                {
                    filename = this.ComboSound.SelectedItem.ToString();
                }

                this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            bool flg = this.RadioExAnd.Checked;
            this.ExUID.Enabled = flg;
            this.ExMSG1.Enabled = flg;
            this.ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckMark.Enabled = !this.OptMove.Checked;
        }

        private void ButtonRuleUp_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && this.ListFilters.SelectedItem != null && this.ListFilters.SelectedIndex > 0)
            {
                string tabname = this.ListTabs.SelectedItem.ToString();
                FiltersClass selected = this.sts.Tabs[tabname].Filters[this.ListFilters.SelectedIndex];
                FiltersClass target = this.sts.Tabs[tabname].Filters[this.ListFilters.SelectedIndex - 1];
                int idx = this.ListFilters.SelectedIndex;
                this.ListFilters.Items.RemoveAt(idx - 1);
                this.ListFilters.Items.Insert(idx, target);
                this.sts.Tabs[tabname].Filters.RemoveAt(idx - 1);
                this.sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleDown_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && this.ListFilters.SelectedItem != null && this.ListFilters.SelectedIndex < this.ListFilters.Items.Count - 1)
            {
                string tabname = this.ListTabs.SelectedItem.ToString();
                FiltersClass selected = this.sts.Tabs[tabname].Filters[this.ListFilters.SelectedIndex];
                FiltersClass target = this.sts.Tabs[tabname].Filters[this.ListFilters.SelectedIndex + 1];
                int idx = this.ListFilters.SelectedIndex;
                this.ListFilters.Items.RemoveAt(idx + 1);
                this.ListFilters.Items.Insert(idx, target);
                this.sts.Tabs[tabname].Filters.RemoveAt(idx + 1);
                this.sts.Tabs[tabname].Filters.Insert(idx, target);
            }
        }

        private void ButtonRuleCopy_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && this.ListFilters.SelectedItem != null)
            {
                this.tabDialog.Text = R.ButtonRuleCopy_ClickText1;
                if (this.tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                string tabname = this.ListTabs.SelectedItem.ToString();
                StringCollection tabs = this.tabDialog.SelectedTabNames;
                List<FiltersClass> filters = new List<FiltersClass>();

                foreach (int idx in this.ListFilters.SelectedIndices)
                {
                    filters.Add(this.sts.Tabs[tabname].Filters[idx].CopyTo(new FiltersClass()));
                }

                foreach (string tb in tabs)
                {
                    if (tb != tabname)
                    {
                        foreach (FiltersClass flt in filters)
                        {
                            if (!this.sts.Tabs[tb].Filters.Contains(flt))
                            {
                                this.sts.Tabs[tb].AddFilter(flt.CopyTo(new FiltersClass()));
                            }
                        }
                    }
                }

                this.SetFilters(tabname);
            }
        }

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex > -1 && this.ListFilters.SelectedItem != null)
            {
                this.tabDialog.Text = R.ButtonRuleMove_ClickText1;
                if (this.tabDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                string tabname = this.ListTabs.SelectedItem.ToString();
                StringCollection tabs = this.tabDialog.SelectedTabNames;
                List<FiltersClass> filters = new List<FiltersClass>();

                foreach (int idx in this.ListFilters.SelectedIndices)
                {
                    filters.Add(this.sts.Tabs[tabname].Filters[idx].CopyTo(new FiltersClass()));
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
                            if (!this.sts.Tabs[tb].Filters.Contains(flt))
                            {
                                this.sts.Tabs[tb].AddFilter(flt.CopyTo(new FiltersClass()));
                            }
                        }
                    }
                }

                // TODO: VB's for/next loop to C# for{}
                for (int idx = this.ListFilters.Items.Count - 1; idx >= 0; idx += -1)
                {
                    if (this.ListFilters.GetSelected(idx))
                    {
                        this.sts.Tabs[this.ListTabs.SelectedItem.ToString()].RemoveFilter((FiltersClass)this.ListFilters.Items[idx]);
                        this.ListFilters.Items.RemoveAt(idx);
                    }
                }

                this.SetFilters(tabname);
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
                    string startstr = string.Empty;
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

        #endregion event handler

        #region private methods

        private void SetFilters(string tabName)
        {
            if (this.ListTabs.Items.Count == 0)
            {
                return;
            }

            this.ListFilters.Items.Clear();
            this.ListFilters.Items.AddRange(this.sts.Tabs[tabName].GetFilters());
            if (this.ListFilters.Items.Count > 0)
            {
                this.ListFilters.SelectedIndex = 0;
            }

            this.CheckManageRead.Checked = this.sts.Tabs[tabName].UnreadManage;
            this.CheckNotifyNew.Checked = this.sts.Tabs[tabName].Notify;

            int idx = this.ComboSound.Items.IndexOf(this.sts.Tabs[tabName].SoundFile);
            if (idx == -1)
            {
                idx = 0;
            }

            this.ComboSound.SelectedIndex = idx;

            if (this.isDirectAdd)
            {
                return;
            }

            this.ListTabs.Enabled = true;
            this.GroupTab.Enabled = true;
            this.ListFilters.Enabled = true;
            if (this.ListFilters.SelectedIndex != -1)
            {
                this.ShowDetail();
            }

            this.EditFilterGroup.Enabled = false;
            switch (TabInformations.GetInstance().Tabs[tabName].TabType)
            {
                case TabUsageType.Home:
                case TabUsageType.DirectMessage:
                case TabUsageType.Favorites:
                case TabUsageType.PublicSearch:
                case TabUsageType.Lists:
                case TabUsageType.Related:
                case TabUsageType.UserTimeline:
                    this.ButtonNew.Enabled = false;
                    this.ButtonEdit.Enabled = false;
                    this.ButtonDelete.Enabled = false;
                    this.ButtonRuleUp.Enabled = false;
                    this.ButtonRuleDown.Enabled = false;
                    this.ButtonRuleCopy.Enabled = false;
                    this.ButtonRuleMove.Enabled = false;
                    break;
                default:
                    this.ButtonNew.Enabled = true;
                    if (this.ListFilters.SelectedIndex > -1)
                    {
                        this.ButtonEdit.Enabled = true;
                        this.ButtonDelete.Enabled = true;
                        this.ButtonRuleUp.Enabled = true;
                        this.ButtonRuleDown.Enabled = true;
                        this.ButtonRuleCopy.Enabled = true;
                        this.ButtonRuleMove.Enabled = true;
                    }
                    else
                    {
                        this.ButtonEdit.Enabled = false;
                        this.ButtonDelete.Enabled = false;
                        this.ButtonRuleUp.Enabled = false;
                        this.ButtonRuleDown.Enabled = false;
                        this.ButtonRuleCopy.Enabled = false;
                        this.ButtonRuleMove.Enabled = false;
                    }

                    break;
            }

            switch (TabInformations.GetInstance().Tabs[tabName].TabType)
            {
                case TabUsageType.Home:
                    this.LabelTabType.Text = R.TabUsageTypeName_Home;
                    break;
                case TabUsageType.Mentions:
                    this.LabelTabType.Text = R.TabUsageTypeName_Mentions;
                    break;
                case TabUsageType.DirectMessage:
                    this.LabelTabType.Text = R.TabUsageTypeName_DirectMessage;
                    break;
                case TabUsageType.Favorites:
                    this.LabelTabType.Text = R.TabUsageTypeName_Favorites;
                    break;
                case TabUsageType.UserDefined:
                    this.LabelTabType.Text = R.TabUsageTypeName_UserDefined;
                    break;
                case TabUsageType.PublicSearch:
                    this.LabelTabType.Text = R.TabUsageTypeName_PublicSearch;
                    break;
                case TabUsageType.Lists:
                    this.LabelTabType.Text = R.TabUsageTypeName_Lists;
                    break;
                case TabUsageType.Related:
                    this.LabelTabType.Text = R.TabUsageTypeName_Related;
                    break;
                case TabUsageType.UserTimeline:
                    this.LabelTabType.Text = R.TabUsageTypeName_UserTimeline;
                    break;
                default:
                    this.LabelTabType.Text = "UNKNOWN";
                    break;
            }

            this.ButtonRenameTab.Enabled = true;
            if (TabInformations.GetInstance().IsDefaultTab(tabName))
            {
                this.ButtonDeleteTab.Enabled = false;
            }
            else
            {
                this.ButtonDeleteTab.Enabled = true;
            }

            this.closeButton.Enabled = true;
        }

        private void ShowDetail()
        {
            if (this.isDirectAdd)
            {
                return;
            }

            if (this.ListFilters.SelectedIndex > -1)
            {
                FiltersClass fc = (FiltersClass)this.ListFilters.SelectedItem;
                if (fc.SearchBoth)
                {
                    this.RadioAND.Checked = true;
                    this.RadioPLUS.Checked = false;
                    this.UID.Enabled = true;
                    this.MSG1.Enabled = true;
                    this.MSG2.Enabled = false;
                    this.UID.Text = fc.NameFilter;
                    this.UID.SelectAll();
                    this.MSG1.Text = string.Empty;
                    this.MSG2.Text = string.Empty;
                    foreach (string bf in fc.BodyFilter)
                    {
                        this.MSG1.Text += bf + " ";
                    }

                    this.MSG1.Text = this.MSG1.Text.Trim();
                    this.MSG1.SelectAll();
                }
                else
                {
                    this.RadioPLUS.Checked = true;
                    this.RadioAND.Checked = false;
                    this.UID.Enabled = false;
                    this.MSG1.Enabled = false;
                    this.MSG2.Enabled = true;
                    this.UID.Text = string.Empty;
                    this.MSG1.Text = string.Empty;
                    this.MSG2.Text = string.Empty;
                    foreach (string bf in fc.BodyFilter)
                    {
                        this.MSG2.Text += bf + " ";
                    }

                    this.MSG2.Text = this.MSG2.Text.Trim();
                    this.MSG2.SelectAll();
                }

                this.TextSource.Text = fc.Source;
                this.CheckRegex.Checked = fc.UseRegex;
                this.CheckURL.Checked = fc.SearchUrl;
                this.CheckCaseSensitive.Checked = fc.CaseSensitive;
                this.CheckRetweet.Checked = fc.IsRt;
                this.CheckLambda.Checked = fc.UseLambda;

                if (fc.ExSearchBoth)
                {
                    this.RadioExAnd.Checked = true;
                    this.RadioExPLUS.Checked = false;
                    this.ExUID.Enabled = true;
                    this.ExMSG1.Enabled = true;
                    this.ExMSG2.Enabled = false;
                    this.ExUID.Text = fc.ExNameFilter;
                    this.ExUID.SelectAll();
                    this.ExMSG1.Text = string.Empty;
                    this.ExMSG2.Text = string.Empty;
                    foreach (string bf in fc.ExBodyFilter)
                    {
                        this.ExMSG1.Text += bf + " ";
                    }

                    this.ExMSG1.Text = this.ExMSG1.Text.Trim();
                    this.ExMSG1.SelectAll();
                }
                else
                {
                    this.RadioExPLUS.Checked = true;
                    this.RadioExAnd.Checked = false;
                    this.ExUID.Enabled = false;
                    this.ExMSG1.Enabled = false;
                    this.ExMSG2.Enabled = true;
                    this.ExUID.Text = string.Empty;
                    this.ExMSG1.Text = string.Empty;
                    this.ExMSG2.Text = string.Empty;
                    foreach (string bf in fc.ExBodyFilter)
                    {
                        this.ExMSG2.Text += bf + " ";
                    }

                    this.ExMSG2.Text = this.ExMSG2.Text.Trim();
                    this.ExMSG2.SelectAll();
                }

                this.TextExSource.Text = fc.ExSource;
                this.CheckExRegex.Checked = fc.ExUseRegex;
                this.CheckExURL.Checked = fc.ExSearchUrl;
                this.CheckExCaseSensitive.Checked = fc.ExCaseSensitive;
                this.CheckExRetweet.Checked = fc.IsExRt;
                this.CheckExLambDa.Checked = fc.ExUseLambda;

                if (fc.MoveFrom)
                {
                    this.OptMove.Checked = true;
                }
                else
                {
                    this.OptCopy.Checked = true;
                }

                this.CheckMark.Checked = fc.SetMark;
                this.ButtonEdit.Enabled = true;
                this.ButtonDelete.Enabled = true;
                this.ButtonRuleUp.Enabled = true;
                this.ButtonRuleDown.Enabled = true;
                this.ButtonRuleCopy.Enabled = true;
                this.ButtonRuleMove.Enabled = true;
            }
            else
            {
                this.RadioAND.Checked = true;
                this.RadioPLUS.Checked = false;
                this.UID.Enabled = true;
                this.MSG1.Enabled = true;
                this.MSG2.Enabled = false;
                this.UID.Text = string.Empty;
                this.MSG1.Text = string.Empty;
                this.MSG2.Text = string.Empty;
                this.TextSource.Text = string.Empty;
                this.CheckRegex.Checked = false;
                this.CheckURL.Checked = false;
                this.CheckCaseSensitive.Checked = false;
                this.CheckRetweet.Checked = false;
                this.CheckLambda.Checked = false;
                this.RadioExAnd.Checked = true;
                this.RadioExPLUS.Checked = false;
                this.ExUID.Enabled = true;
                this.ExMSG1.Enabled = true;
                this.ExMSG2.Enabled = false;
                this.ExUID.Text = string.Empty;
                this.ExMSG1.Text = string.Empty;
                this.ExMSG2.Text = string.Empty;
                this.TextExSource.Text = string.Empty;
                this.CheckExRegex.Checked = false;
                this.CheckExURL.Checked = false;
                this.CheckExCaseSensitive.Checked = false;
                this.CheckExRetweet.Checked = false;
                this.CheckExLambDa.Checked = false;
                this.OptCopy.Checked = true;
                this.CheckMark.Checked = true;
                this.ButtonEdit.Enabled = false;
                this.ButtonDelete.Enabled = false;
                this.ButtonRuleUp.Enabled = false;
                this.ButtonRuleDown.Enabled = false;
                this.ButtonRuleCopy.Enabled = false;
                this.ButtonRuleMove.Enabled = false;
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
                Regex rgx = new System.Text.RegularExpressions.Regex(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(R.ButtonOK_ClickText3 + ex.Message, R.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool CheckMatchRule(ref bool isBlank)
        {
            isBlank = false;
            this.TextSource.Text = this.TextSource.Text.Trim();
            if (this.RadioAND.Checked)
            {
                this.MSG1.Text = this.MSG1.Text.Trim();
                this.UID.Text = this.UID.Text.Trim();
                if (!this.CheckRegex.Checked && !this.CheckLambda.Checked)
                {
                    this.MSG1.Text = this.MSG1.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(this.UID.Text) && string.IsNullOrEmpty(this.MSG1.Text) && string.IsNullOrEmpty(this.TextSource.Text) && !this.CheckRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (this.CheckLambda.Checked)
                {
                    if (!this.IsValidLambdaExp(this.UID.Text))
                    {
                        return false;
                    }

                    if (!this.IsValidLambdaExp(this.MSG1.Text))
                    {
                        return false;
                    }
                }
                else if (this.CheckRegex.Checked)
                {
                    if (!this.IsValidRegexp(this.UID.Text))
                    {
                        return false;
                    }

                    if (!this.IsValidRegexp(this.MSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                this.MSG2.Text = this.MSG2.Text.Trim();
                if (!this.CheckRegex.Checked && !this.CheckLambda.Checked)
                {
                    this.MSG2.Text = this.MSG2.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(this.MSG2.Text) && string.IsNullOrEmpty(this.TextSource.Text) && !this.CheckRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (this.CheckLambda.Checked && !this.IsValidLambdaExp(this.MSG2.Text))
                {
                    return false;
                }
                else if (this.CheckRegex.Checked && !this.IsValidRegexp(this.MSG2.Text))
                {
                    return false;
                }
            }

            if (this.CheckRegex.Checked && !this.IsValidRegexp(this.TextSource.Text))
            {
                return false;
            }

            return true;
        }

        private bool CheckExcludeRule(ref bool isBlank)
        {
            isBlank = false;
            this.TextExSource.Text = this.TextExSource.Text.Trim();
            if (this.RadioExAnd.Checked)
            {
                this.ExMSG1.Text = this.ExMSG1.Text.Trim();
                if (!this.CheckExRegex.Checked && !this.CheckExLambDa.Checked)
                {
                    this.ExMSG1.Text = this.ExMSG1.Text.Replace("　", " ");
                }

                this.ExUID.Text = this.ExUID.Text.Trim();
                if (string.IsNullOrEmpty(this.ExUID.Text) && string.IsNullOrEmpty(this.ExMSG1.Text) && string.IsNullOrEmpty(this.TextExSource.Text) && !this.CheckExRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (this.CheckExLambDa.Checked)
                {
                    if (!this.IsValidLambdaExp(this.ExUID.Text))
                    {
                        return false;
                    }

                    if (!this.IsValidLambdaExp(this.ExMSG1.Text))
                    {
                        return false;
                    }
                }
                else if (this.CheckExRegex.Checked)
                {
                    if (!this.IsValidRegexp(this.ExUID.Text))
                    {
                        return false;
                    }

                    if (!this.IsValidRegexp(this.ExMSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                this.ExMSG2.Text = this.ExMSG2.Text.Trim();
                if (!this.CheckExRegex.Checked && !this.CheckExLambDa.Checked)
                {
                    this.ExMSG2.Text = this.ExMSG2.Text.Replace("　", " ");
                }

                if (string.IsNullOrEmpty(this.ExMSG2.Text) && string.IsNullOrEmpty(this.TextExSource.Text) && !this.CheckExRetweet.Checked)
                {
                    isBlank = true;
                    return true;
                }

                if (this.CheckExLambDa.Checked && !this.IsValidLambdaExp(this.ExMSG2.Text))
                {
                    return false;
                }
                else if (this.CheckExRegex.Checked && !this.IsValidRegexp(this.ExMSG2.Text))
                {
                    return false;
                }
            }

            if (this.CheckExRegex.Checked && !this.IsValidRegexp(this.TextExSource.Text))
            {
                return false;
            }

            return true;
        }

        private void SetTabnamesToDialog()
        {
            this.tabDialog.ClearTab();
            foreach (string key in this.sts.Tabs.Keys)
            {
                if (TabInformations.GetInstance().IsDistributableTab(key))
                {
                    this.tabDialog.AddTab(key);
                }
            }
        }

        #endregion private methods
    }
}