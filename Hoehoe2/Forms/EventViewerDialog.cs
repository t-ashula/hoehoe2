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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using R = Hoehoe.Properties.Resources;

    public partial class EventViewerDialog
    {
        #region privates

        private Twitter.FormattedEvent[] filterdEventSource;
        private ListViewItem[] itemCache;
        private int itemCacheIndex;
        private TabPage curTab;

        #endregion privates

        #region constructor

        public EventViewerDialog()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region properties

        public List<Hoehoe.Twitter.FormattedEvent> EventSource { get; set; }

        #endregion properties

        #region event handler

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

        private void EventViewerDialog_Shown(object sender, EventArgs e)
        {
            this.EventList.BeginUpdate();
            this.curTab = this.TabEventType.SelectedTab;
            this.CreateFilterdEventSource();
            this.EventList.EndUpdate();
            this.TopMost = Configs.Instance.AlwaysTop;
        }

        private void EventList_DoubleClick(object sender, EventArgs e)
        {
            if (this.EventList.SelectedIndices.Count != 0)
            {
                var selectedEvent = this.filterdEventSource[this.EventList.SelectedIndices[0]];
                if (selectedEvent != null)
                {
                    ((TweenMain)this.Owner).OpenUriAsync("http://twitter.com/" + selectedEvent.Username);
                }
            }
        }

        private void CheckExcludeMyEvent_CheckedChanged(object sender, EventArgs e)
        {
            this.CreateFilterdEventSource();
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            this.CreateFilterdEventSource();
        }

        private void TabEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CreateFilterdEventSource();
        }

        private void TabEventType_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.curTab = e.TabPage;
            if (!e.TabPage.Controls.Contains(this.EventList))
            {
                e.TabPage.Controls.Add(this.EventList);
            }
        }

        private void TextBoxKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.CreateFilterdEventSource();
                e.Handled = true;
            }
        }

        private void EventList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this.itemCache != null && e.ItemIndex >= this.itemCacheIndex && e.ItemIndex < this.itemCacheIndex + this.itemCache.Length)
            {
                // キャッシュヒット
                e.Item = this.itemCache[e.ItemIndex - this.itemCacheIndex];
            }
            else
            {
                // キャッシュミス
                e.Item = this.CreateListViewItem(this.filterdEventSource[e.ItemIndex]);
            }
        }

        private void EventList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            this.CreateCache(e.StartIndex, e.EndIndex);
        }

        private void SaveLogButton_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(R.SaveLogMenuItem_ClickText5, Environment.NewLine), R.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            string tabName = string.Empty;
            switch (rslt)
            {
                case DialogResult.Yes:
                    tabName = (string)this.curTab.Tag;
                    break;
                case DialogResult.No:
                    break;
                default:
                    return;
            }

            this.SaveFileDialog1.FileName = string.Format("HoehoeEvents{0}{1:yyMMdd-HHmmss}.tsv", tabName, DateTime.Now);
            this.SaveFileDialog1.InitialDirectory = MyCommon.AppDir;
            this.SaveFileDialog1.Filter = R.SaveLogMenuItem_ClickText3;
            this.SaveFileDialog1.FilterIndex = 0;
            this.SaveFileDialog1.Title = R.SaveLogMenuItem_ClickText4;
            this.SaveFileDialog1.RestoreDirectory = true;

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!this.SaveFileDialog1.ValidateNames)
                {
                    return;
                }

                using (StreamWriter sw = new StreamWriter(this.SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    switch (rslt)
                    {
                        case DialogResult.Yes:
                            this.SaveEventLog(this.filterdEventSource.ToList(), sw);
                            break;
                        case DialogResult.No:
                            this.SaveEventLog(this.EventSource, sw);
                            break;
                        default:
                            break;
                    }
                }
            }

            this.TopMost = Configs.Instance.AlwaysTop;
        }

        #endregion event handler

        #region private methods

        private ListViewItem CreateListViewItem(Twitter.FormattedEvent source)
        {
            return new ListViewItem(new[] { source.CreatedAt.ToString(), source.Event.ToUpper(), source.Username, source.Target });
        }

        private EventType ParseEventTypeFromTag()
        {
            return (EventType)Enum.Parse(typeof(EventType), (string)this.curTab.Tag);
        }

        private bool IsFilterMatch(Twitter.FormattedEvent x)
        {
            if (!this.CheckBoxFilter.Checked || string.IsNullOrEmpty(this.TextBoxKeyword.Text))
            {
                return true;
            }

            if (!this.CheckRegex.Checked)
            {
                return x.Username.Contains(this.TextBoxKeyword.Text) || x.Target.Contains(this.TextBoxKeyword.Text);
            }

            try
            {
                Regex rx = new Regex(this.TextBoxKeyword.Text);
                return rx.Match(x.Username).Success || rx.Match(x.Target).Success;
            }
            catch (Exception ex)
            {
                MessageBox.Show(R.ButtonOK_ClickText3 + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        private void CreateFilterdEventSource()
        {
            if (this.EventSource != null && this.EventSource.Count > 0)
            {
                this.filterdEventSource = this.EventSource.FindAll(x => this.CheckExcludeMyEvent.Checked ? !x.IsMe : true
                    && Convert.ToBoolean(x.Eventtype & this.ParseEventTypeFromTag())
                    && this.IsFilterMatch(x)).ToArray();
                this.itemCache = null;
                this.EventList.VirtualListSize = this.filterdEventSource.Count();
                this.StatusLabelCount.Text = string.Format("{0} / {1}", this.filterdEventSource.Count(), this.EventSource.Count);
            }
            else
            {
                this.StatusLabelCount.Text = "0 / 0";
            }
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            // キャッシュ要求（要求範囲±30を作成）
            startIndex -= 30;
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            endIndex += 30;
            if (endIndex > this.filterdEventSource.Count() - 1)
            {
                endIndex = this.filterdEventSource.Count() - 1;
            }

            this.itemCache = new ListViewItem[endIndex - startIndex + 1];
            this.itemCacheIndex = startIndex;
            for (int i = 0; i <= endIndex - startIndex; i++)
            {
                this.itemCache[i] = this.CreateListViewItem(this.filterdEventSource[startIndex + i]);
            }
        }

        private void SaveEventLog(List<Twitter.FormattedEvent> source, StreamWriter sw)
        {
            foreach (var ev in source)
            {
                sw.WriteLine(string.Format("{0}\t\"{1}\"\t{2}\t{3}\t{4}\t{5}", ev.Eventtype, ev.CreatedAt, ev.Event, ev.Username, ev.Target, ev.Id));
            }
        }

        #endregion private methods
    }
}