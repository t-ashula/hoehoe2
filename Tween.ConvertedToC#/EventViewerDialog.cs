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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tween
{
    public partial class EventViewerDialog
    {
        public List<Twitter.FormattedEvent> EventSource { get; set; }

        private Twitter.FormattedEvent[] _filterdEventSource;
        private ListViewItem[] _itemCache;
        private int _itemCacheIndex;
        private TabPage _curTab;

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private ListViewItem CreateListViewItem(Twitter.FormattedEvent source)
        {
            return new ListViewItem(new[] { source.CreatedAt.ToString(), source.Event.ToUpper(), source.Username, source.Target });
        }

        private void EventViewerDialog_Shown(object sender, EventArgs e)
        {
            EventList.BeginUpdate();
            _curTab = TabEventType.SelectedTab;
            CreateFilterdEventSource();
            EventList.EndUpdate();
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void EventList_DoubleClick(object sender, EventArgs e)
        {
            if (!(EventList.SelectedIndices.Count == 0) && _filterdEventSource[EventList.SelectedIndices[0]] != null)
            {
                ((TweenMain)this.Owner).OpenUriAsync("http://twitter.com/" + _filterdEventSource[EventList.SelectedIndices[0]].Username);
            }
        }

        private MyCommon.EventType ParseEventTypeFromTag()
        {
            return (Tween.MyCommon.EventType)Enum.Parse(typeof(Tween.MyCommon.EventType), _curTab.Tag.ToString());
        }

        private bool IsFilterMatch(Twitter.FormattedEvent x)
        {
            if (!CheckBoxFilter.Checked || String.IsNullOrEmpty(TextBoxKeyword.Text))
            {
                return true;
            }
            if (CheckRegex.Checked)
            {
                try
                {
                    Regex rx = new Regex(TextBoxKeyword.Text);
                    return rx.Match(x.Username).Success || rx.Match(x.Target).Success;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Tween.My_Project.Resources.ButtonOK_ClickText3 + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                return x.Username.Contains(TextBoxKeyword.Text) || x.Target.Contains(TextBoxKeyword.Text);
            }
        }

        private void CreateFilterdEventSource()
        {
            if (EventSource != null && EventSource.Count > 0)
            {
                _filterdEventSource = EventSource.FindAll(x => CheckExcludeMyEvent.Checked ? !x.IsMe : true && Convert.ToBoolean(x.Eventtype & ParseEventTypeFromTag()) && IsFilterMatch(x)).ToArray();
                _itemCache = null;
                EventList.VirtualListSize = _filterdEventSource.Count();
                StatusLabelCount.Text = String.Format("{0} / {1}", _filterdEventSource.Count(), EventSource.Count);
            }
            else
            {
                StatusLabelCount.Text = "0 / 0";
            }
        }

        private void CheckExcludeMyEvent_CheckedChanged(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_Selecting(object sender, TabControlCancelEventArgs e)
        {
            _curTab = e.TabPage;
            if (!e.TabPage.Controls.Contains(EventList))
            {
                e.TabPage.Controls.Add(EventList);
            }
        }

        private void TextBoxKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CreateFilterdEventSource();
                e.Handled = true;
            }
        }

        private void EventList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_itemCache != null && e.ItemIndex >= _itemCacheIndex && e.ItemIndex < _itemCacheIndex + _itemCache.Length)
            {
                //キャッシュヒット
                e.Item = _itemCache[e.ItemIndex - _itemCacheIndex];
            }
            else
            {
                //キャッシュミス
                e.Item = CreateListViewItem(_filterdEventSource[e.ItemIndex]);
            }
        }

        private void EventList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            CreateCache(e.StartIndex, e.EndIndex);
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            //キャッシュ要求（要求範囲±30を作成）
            startIndex -= 30;
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            endIndex += 30;
            if (endIndex > _filterdEventSource.Count() - 1)
            {
                endIndex = _filterdEventSource.Count() - 1;
            }
            _itemCache = new ListViewItem[endIndex - startIndex + 1];
            _itemCacheIndex = startIndex;
            for (int i = 0; i <= endIndex - startIndex; i++)
            {
                _itemCache[i] = CreateListViewItem(_filterdEventSource[startIndex + i]);
            }
        }

        private void SaveLogButton_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Tween.My_Project.Resources.SaveLogMenuItem_ClickText5, Environment.NewLine), Tween.My_Project.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            string tabName = "";
            switch (rslt)
            {
                case DialogResult.Yes:
                    tabName = _curTab.Tag.ToString();
                    break;
                case DialogResult.No:
                    break;
                default:
                    return;
            }
            SaveFileDialog1.FileName = String.Format("TweenEvents{0}{1:yyMMdd-HHmmss}.tsv", tabName, DateTime.Now);
            SaveFileDialog1.InitialDirectory = Tween.My.MyProject.Application.Info.DirectoryPath;
            SaveFileDialog1.Filter = Tween.My_Project.Resources.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = Tween.My_Project.Resources.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames)
                {
                    return;
                }
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    switch (rslt)
                    {
                        case DialogResult.Yes:
                            SaveEventLog(_filterdEventSource.ToList(), sw);
                            break;
                        case DialogResult.No:
                            SaveEventLog(EventSource, sw);
                            break;
                        default:
                            break;
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void SaveEventLog(List<Twitter.FormattedEvent> source, StreamWriter sw)
        {
            foreach (Twitter.FormattedEvent ev in source)
            {
                sw.WriteLine(String.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}", ev.Eventtype, ev.CreatedAt, ev.Event, ev.Username, ev.Target, ev.Id));
            }
        }

        public EventViewerDialog()
        {
            InitializeComponent();
        }
    }
}