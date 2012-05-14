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

namespace Tween
{
    public partial class InputTabName
    {
        private void OkButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            TextTabName.Text = "";
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public string TabName
        {
            get { return this.TextTabName.Text.Trim(); }
            set { TextTabName.Text = value.Trim(); }
        }

        public void SetFormTitle(string value)
        {
            this.Text = value;
        }

        public void SetFormDescription(string value)
        {
            this.LabelDescription.Text = value;
        }

        private bool _isShowUsage;

        public void SetIsShowUsage(bool value)
        {
            _isShowUsage = value;
        }

        public MyCommon.TabUsageType Usage { get; private set; }

        private void InputTabName_Load(object sender, EventArgs e)
        {
            this.LabelUsage.Visible = false;
            this.ComboUsage.Visible = false;
            this.ComboUsage.Items.Add(Tween.My_Project.Resources.InputTabName_Load1);
            this.ComboUsage.Items.Add("Lists");
            this.ComboUsage.Items.Add("PublicSearch");
            this.ComboUsage.SelectedIndex = 0;
        }

        private void InputTabName_Shown(object sender, EventArgs e)
        {
            ActiveControl = TextTabName;
            if (_isShowUsage)
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
                    Usage = Tween.MyCommon.TabUsageType.UserDefined;
                    break;
                case 1:
                    Usage = Tween.MyCommon.TabUsageType.Lists;
                    break;
                case 2:
                    Usage = Tween.MyCommon.TabUsageType.PublicSearch;
                    break;
                default:
                    Usage = Tween.MyCommon.TabUsageType.Undefined;
                    break;
            }
        }

        public InputTabName()
        {
            InitializeComponent();
        }
    }
}