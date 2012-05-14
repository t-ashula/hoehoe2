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
using Microsoft.VisualBasic;

namespace Tween
{
    public partial class EventViewerDialog
    {
        public List<Twitter.FormattedEvent> EventSource { get; set; }

        private Twitter.FormattedEvent[] _filterdEventSource;
        private ListViewItem[] _ItemCache = null;

        private int _itemCacheIndex;

        private TabPage _curTab = null;

        private void OK_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private ListViewItem CreateListViewItem(Twitter.FormattedEvent source)
        {
            string[] s = {
				source.CreatedAt.ToString(),
				source.Event.ToUpper(),
				source.Username,
				source.Target
			};
            return new ListViewItem(s);
        }

        private void EventViewerDialog_Shown(object sender, System.EventArgs e)
        {
            EventList.BeginUpdate();
            _curTab = TabEventType.SelectedTab;
            CreateFilterdEventSource();
            EventList.EndUpdate();
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void EventList_DoubleClick(System.Object sender, System.EventArgs e)
        {
            if (!(EventList.SelectedIndices.Count == 0) && _filterdEventSource[EventList.SelectedIndices[0]] != null)
            {
                My.MyProject.Forms.TweenMain.OpenUriAsync("http://twitter.com/" + _filterdEventSource[EventList.SelectedIndices[0]].Username);
            }
        }

        private Tween.MyCommon.EventType ParseEventTypeFromTag()
        {
            return (Tween.MyCommon.EventType)Enum.Parse(typeof(Tween.MyCommon.EventType), _curTab.Tag.ToString());
        }

        private bool IsFilterMatch(Twitter.FormattedEvent x)
        {
            if (!CheckBoxFilter.Checked || string.IsNullOrEmpty(TextBoxKeyword.Text))
            {
                return true;
            }
            else
            {
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
        }

        private void CreateFilterdEventSource()
        {
            if (EventSource != null && EventSource.Count > 0)
            {
                _filterdEventSource = EventSource.FindAll(x => CheckExcludeMyEvent.Checked ? !x.IsMe : true && Convert.ToBoolean(x.Eventtype & ParseEventTypeFromTag()) && IsFilterMatch(x)).ToArray();
                _ItemCache = null;
                EventList.VirtualListSize = _filterdEventSource.Count();
                StatusLabelCount.Text = string.Format("{0} / {1}", _filterdEventSource.Count(), EventSource.Count);
            }
            else
            {
                StatusLabelCount.Text = "0 / 0";
            }
        }

        private void CheckExcludeMyEvent_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void ButtonRefresh_Click(System.Object sender, System.EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_Selecting(System.Object sender, System.Windows.Forms.TabControlCancelEventArgs e)
        {
            _curTab = e.TabPage;
            if (!e.TabPage.Controls.Contains(EventList))
            {
                e.TabPage.Controls.Add(EventList);
            }
        }

        private void TextBoxKeyword_KeyPress(System.Object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == Strings.ChrW((int)Keys.Enter))
            {
                CreateFilterdEventSource();
                e.Handled = true;
            }
        }

        private void EventList_RetrieveVirtualItem(System.Object sender, System.Windows.Forms.RetrieveVirtualItemEventArgs e)
        {
            if (_ItemCache != null && e.ItemIndex >= _itemCacheIndex && e.ItemIndex < _itemCacheIndex + _ItemCache.Length)
            {
                //キャッシュヒット
                e.Item = _ItemCache[e.ItemIndex - _itemCacheIndex];
            }
            else
            {
                //キャッシュミス
                e.Item = CreateListViewItem(_filterdEventSource[e.ItemIndex]);
            }
        }

        private void EventList_CacheVirtualItems(System.Object sender, System.Windows.Forms.CacheVirtualItemsEventArgs e)
        {
            CreateCache(e.StartIndex, e.EndIndex);
        }

        private void CreateCache(int StartIndex, int EndIndex)
        {
            //キャッシュ要求（要求範囲±30を作成）
            StartIndex -= 30;
            if (StartIndex < 0)
                StartIndex = 0;
            EndIndex += 30;
            if (EndIndex > _filterdEventSource.Count() - 1)
            {
                EndIndex = _filterdEventSource.Count() - 1;
            }
            _ItemCache = new ListViewItem[EndIndex - StartIndex + 1];
            _itemCacheIndex = StartIndex;
            for (int i = 0; i <= EndIndex - StartIndex; i++)
            {
                _ItemCache[i] = CreateListViewItem(_filterdEventSource[StartIndex + i]);
            }
        }

        private void SaveLogButton_Click(System.Object sender, System.EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Tween.My_Project.Resources.SaveLogMenuItem_ClickText5, Environment.NewLine), Tween.My_Project.Resources.SaveLogMenuItem_ClickText2, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (rslt)
            {
                case System.Windows.Forms.DialogResult.Yes:
                    SaveFileDialog1.FileName = "TweenEvents" + _curTab.Tag.ToString() + Strings.Format(DateAndTime.Now, "yyMMdd-HHmmss") + ".tsv";
                    break;
                case System.Windows.Forms.DialogResult.No:
                    SaveFileDialog1.FileName = "TweenEvents" + Strings.Format(DateAndTime.Now, "yyMMdd-HHmmss") + ".tsv";
                    break;
                default:
                    return;

                    break;
            }

            SaveFileDialog1.InitialDirectory = Tween.My.MyProject.Application.Info.DirectoryPath;
            SaveFileDialog1.Filter = Tween.My_Project.Resources.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = Tween.My_Project.Resources.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames)
                    return;
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    switch (rslt)
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            SaveEventLog(_filterdEventSource.ToList(), sw);
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            SaveEventLog(EventSource, sw);
                            break;
                        default:
                            break;
                        //
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void SaveEventLog(List<Twitter.FormattedEvent> source, StreamWriter sw)
        {
            foreach (Twitter.FormattedEvent _event in source)
            {
                sw.WriteLine(_event.Eventtype.ToString() + Constants.vbTab + "\"" + _event.CreatedAt.ToString() + "\"" + Constants.vbTab + _event.Event + Constants.vbTab + _event.Username + Constants.vbTab + _event.Target + Constants.vbTab + _event.Id.ToString());
            }
        }

        public EventViewerDialog()
        {
            Shown += EventViewerDialog_Shown;
            InitializeComponent();
        }
    }
}