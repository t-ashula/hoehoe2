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
    using System.IO;

    public partial class TweenAboutBox
    {
        #region constructor

        public TweenAboutBox()
        {
            this.InitializeComponent();
        }

        #endregion constructor

        #region event handler

        private void TweenAboutBox_Load(object sender, EventArgs e)
        {
            this.SetAboutInfoStrings();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TweenAboutBox_Shown(object sender, EventArgs e)
        {
            this.OKButton.Focus();
        }

        #endregion event handler

        #region privates

        private void SetAboutInfoStrings()
        {
            // フォームのタイトルを設定します。
            string applicationTitle = !string.IsNullOrEmpty(MyCommon.AppTitle) ? MyCommon.AppTitle : Path.GetFileNameWithoutExtension(MyCommon.AppAssemblyName);
            this.Text = string.Format(Hoehoe.Properties.Resources.TweenAboutBox_LoadText1, applicationTitle);
            this.LabelProductName.Text = MyCommon.AppAssemblyProductName;
            this.LabelVersion.Text = string.Format(Hoehoe.Properties.Resources.TweenAboutBox_LoadText2, MyCommon.FileVersion + "(" + MyCommon.AppVersion.ToString() + ")");
            this.LabelCopyright.Text = MyCommon.AppAssemblyCopyright;
            this.LabelCompanyName.Text = MyCommon.AppAssemblyCompanyName;
            this.TextBoxDescription.Text = MyCommon.AppAssemblyDescription;
            this.ChangeLog.Text = Hoehoe.Properties.Resources.ChangeLog;
            this.TextBoxDescription.Text = Hoehoe.Properties.Resources.Description;
        }

        #endregion privates
    }
}