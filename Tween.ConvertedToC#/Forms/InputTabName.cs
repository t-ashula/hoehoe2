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
    using System.Windows.Forms;

    public partial class InputTabName
    {
        #region privates

        private bool isShowUsage;

        #endregion privates

        #region constructor

        public InputTabName()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region properties

        public string TabName
        {
            get { return this.TextTabName.Text.Trim(); }
            set { TextTabName.Text = value.Trim(); }
        }

        public TabUsageType Usage { get; private set; }

        #endregion properties

        #region public methods

        public void SetFormTitle(string value)
        {
            this.Text = value;
        }

        public void SetFormDescription(string value)
        {
            this.LabelDescription.Text = value;
        }

        public void SetIsShowUsage(bool value)
        {
            this.isShowUsage = value;
        }

        #endregion public methods

        #region event handler

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            TextTabName.Text = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InputTabName_Load(object sender, EventArgs e)
        {
            this.LabelUsage.Visible = false;
            this.ComboUsage.Visible = false;
            this.ComboUsage.Items.Add(Hoehoe.Properties.Resources.InputTabName_Load1);
            this.ComboUsage.Items.Add("Lists");
            this.ComboUsage.Items.Add("PublicSearch");
            this.ComboUsage.SelectedIndex = 0;
        }

        private void InputTabName_Shown(object sender, EventArgs e)
        {
            ActiveControl = TextTabName;
            if (this.isShowUsage)
            {
                this.LabelUsage.Visible = true;
                this.ComboUsage.Visible = true;
            }
        }

        private void ComboUsage_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ComboUsage.SelectedIndex)
            {
                case 0:
                    this.Usage = TabUsageType.UserDefined;
                    break;
                case 1:
                    this.Usage = TabUsageType.Lists;
                    break;
                case 2:
                    this.Usage = TabUsageType.PublicSearch;
                    break;
                default:
                    this.Usage = TabUsageType.Undefined;
                    break;
            }
        }

        #endregion event handler
    }
}