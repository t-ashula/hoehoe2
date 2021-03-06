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
    using R = Properties.Resources;

    public partial class TweenAboutBox
    {
        #region constructor

        public TweenAboutBox()
        {
            InitializeComponent();
        }

        #endregion

        #region event handler

        private void TweenAboutBox_Load(object sender, EventArgs e)
        {
            SetAboutInfoStrings();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TweenAboutBox_Shown(object sender, EventArgs e)
        {
            OKButton.Focus();
        }

        #endregion

        #region privates

        private void SetAboutInfoStrings()
        {
            // フォームのタイトルを設定します。
            var applicationTitle = !string.IsNullOrEmpty(MyCommon.AppTitle) ?
                MyCommon.AppTitle :
                Path.GetFileNameWithoutExtension(MyCommon.AppAssemblyName);
            Text = string.Format(R.TweenAboutBox_LoadText1, applicationTitle);
            LabelProductName.Text = MyCommon.AppAssemblyProductName;
            LabelVersion.Text = string.Format(R.TweenAboutBox_LoadText2, MyCommon.FileVersion + "(" + MyCommon.AppVersion + ")");
            LabelCopyright.Text = MyCommon.AppAssemblyCopyright;
            LabelCompanyName.Text = MyCommon.AppAssemblyCompanyName;
            TextBoxDescription.Text = MyCommon.AppAssemblyDescription;
            ChangeLog.Text = R.ChangeLog;
            TextBoxDescription.Text = R.Description;
        }

        #endregion
    }
}