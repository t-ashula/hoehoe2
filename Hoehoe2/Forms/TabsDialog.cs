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

using System.Linq;

namespace Hoehoe
{
    using System;
    using System.Collections.Specialized;
    using System.Windows.Forms;
    using R = Properties.Resources;

    public partial class TabsDialog
    {
        #region private

        private bool _multiSelect;

        // private string _newtabItem = R.AddNewTabText1;

        #endregion

        #region constructor

        public TabsDialog(bool multiselect = false)
        {
            InitializeComponent();
            MultiSelect = multiselect;
        }

        #endregion

        #region properties

        public string SelectedTabName
        {
            get
            {
                return TabList.SelectedIndex == -1 ? string.Empty : Convert.ToString(TabList.SelectedItem);
            }
        }

        public StringCollection SelectedTabNames
        {
            get
            {
                if (TabList.SelectedIndex == -1)
                {
                    return null;
                }

                var ret = new StringCollection();
                foreach (var selitem in TabList.SelectedItems)
                {
                    ret.Add((string)selitem);
                }

                return ret;
            }
        }

        public bool MultiSelect
        {
            get
            {
                return _multiSelect;
            }

            set
            {
                _multiSelect = value;
                if (value)
                {
                    TabList.SelectionMode = SelectionMode.MultiExtended;
                    if (TabList.Items[0].ToString() == R.AddNewTabText1)
                    {
                        TabList.Items.RemoveAt(0);
                    }
                }
                else
                {
                    TabList.SelectionMode = SelectionMode.One;
                    if (TabList.Items[0].ToString() != R.AddNewTabText1)
                    {
                        TabList.Items.Insert(0, R.AddNewTabText1);
                    }
                }
            }
        }

        #endregion

        #region public methods

        public void AddTab(string tabName)
        {
            if (TabList.Items.Cast<string>().Any(obj => obj == tabName))
            {
                return;
            }

            TabList.Items.Add(tabName);
        }

        public void RemoveTab(string tabName)
        {
            for (int i = 0; i < TabList.Items.Count; i++)
            {
                if (Convert.ToString(TabList.Items[i]) == tabName)
                {
                    TabList.Items.RemoveAt(i);
                    return;
                }
            }
        }

        public void ClearTab()
        {
            int startidx = 1;
            if (_multiSelect)
            {
                startidx = 0;
            }

            for (int i = startidx; i < TabList.Items.Count; i++)
            {
                TabList.Items.RemoveAt(0);
            }
        }

        #endregion

        #region event handler

        private void OK_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TabsDialog_Load(object sender, EventArgs e)
        {
            if (_multiSelect)
            {
                TabList.SelectedIndex = -1;
            }
            else
            {
                if (TabList.SelectedIndex == -1)
                {
                    TabList.SelectedIndex = 0;
                }
            }
        }

        private void TabList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void TabList_DoubleClick(object sender, EventArgs e)
        {
            if (TabList.SelectedItem == null)
            {
                return;
            }

            if (TabList.IndexFromPoint(TabList.PointToClient(MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void TabsDialog_Shown(object sender, EventArgs e)
        {
            TabList.Focus();
        }

        #endregion
    }
}