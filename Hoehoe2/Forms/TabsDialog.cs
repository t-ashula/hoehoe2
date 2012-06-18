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
    using System.Collections.Specialized;
    using System.Windows.Forms;
    using R = Hoehoe.Properties.Resources;

    public partial class TabsDialog
    {
        #region private

        private bool multiSelect = false;
        private string newtabItem = R.AddNewTabText1;

        #endregion private

        #region constructor

        public TabsDialog()
        {
            this.InitializeComponent();
        }

        public TabsDialog(bool multiselect)
        {
            this.InitializeComponent();
            this.MultiSelect = true;
        }

        #endregion constructor

        #region properties

        public string SelectedTabName
        {
            get
            {
                return this.TabList.SelectedIndex == -1 ? string.Empty : Convert.ToString(this.TabList.SelectedItem);
            }
        }

        public StringCollection SelectedTabNames
        {
            get
            {
                if (this.TabList.SelectedIndex == -1)
                {
                    return null;
                }

                StringCollection ret = new StringCollection();
                foreach (object selitem in this.TabList.SelectedItems)
                {
                    ret.Add(Convert.ToString(selitem));
                }

                return ret;
            }
        }

        public bool MultiSelect
        {
            get
            {
                return this.multiSelect;
            }

            set
            {
                this.multiSelect = value;
                if (value)
                {
                    this.TabList.SelectionMode = SelectionMode.MultiExtended;
                    if (this.TabList.Items[0].ToString() == R.AddNewTabText1)
                    {
                        this.TabList.Items.RemoveAt(0);
                    }
                }
                else
                {
                    this.TabList.SelectionMode = SelectionMode.One;
                    if (this.TabList.Items[0].ToString() != R.AddNewTabText1)
                    {
                        this.TabList.Items.Insert(0, R.AddNewTabText1);
                    }
                }
            }
        }

        #endregion properties

        #region public methods

        public void AddTab(string tabName)
        {
            foreach (string obj in this.TabList.Items)
            {
                if (obj == tabName)
                {
                    return;
                }
            }

            this.TabList.Items.Add(tabName);
        }

        public void RemoveTab(string tabName)
        {
            for (int i = 0; i < this.TabList.Items.Count; i++)
            {
                if (Convert.ToString(this.TabList.Items[i]) == tabName)
                {
                    this.TabList.Items.RemoveAt(i);
                    return;
                }
            }
        }

        public void ClearTab()
        {
            int startidx = 1;
            if (this.multiSelect)
            {
                startidx = 0;
            }

            for (int i = startidx; i < this.TabList.Items.Count; i++)
            {
                this.TabList.Items.RemoveAt(0);
            }
        }

        #endregion public methods

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

        private void TabsDialog_Load(object sender, EventArgs e)
        {
            if (this.multiSelect)
            {
                this.TabList.SelectedIndex = -1;
            }
            else
            {
                if (this.TabList.SelectedIndex == -1)
                {
                    this.TabList.SelectedIndex = 0;
                }
            }
        }

        private void TabList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void TabList_DoubleClick(object sender, EventArgs e)
        {
            if (this.TabList.SelectedItem == null)
            {
                return;
            }

            if (this.TabList.IndexFromPoint(this.TabList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TabsDialog_Shown(object sender, EventArgs e)
        {
            this.TabList.Focus();
        }

        #endregion event handler
    }
}