namespace Tween
{
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

    public sealed partial class TweenAboutBox
    {
        private void TweenAboutBox_Load(System.Object sender, System.EventArgs e)
        {
            // フォームのタイトルを設定します。
            string ApplicationTitle = null;
            if (!string.IsNullOrEmpty(Tween.My.MyProject.Application.Info.Title))
            {
                ApplicationTitle = Tween.My.MyProject.Application.Info.Title;
            }
            else
            {
                ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(Tween.My.MyProject.Application.Info.AssemblyName);
            }
            this.Text = string.Format(Tween.My_Project.Resources.TweenAboutBox_LoadText1, ApplicationTitle);
            // バージョン情報ボックスに表示されたテキストをすべて初期化します。
            // TODO: [プロジェクト] メニューの下にある [プロジェクト プロパティ] ダイアログの [アプリケーション] ペインで、アプリケーションのアセンブリ情報を
            //    カスタマイズします。
            this.LabelProductName.Text = Tween.My.MyProject.Application.Info.ProductName;
            this.LabelVersion.Text = string.Format(Tween.My_Project.Resources.TweenAboutBox_LoadText2, MyCommon.fileVersion + "(" + Tween.My.MyProject.Application.Info.Version.ToString() + ")");
            this.LabelCopyright.Text = Tween.My.MyProject.Application.Info.Copyright;
            this.LabelCompanyName.Text = Tween.My.MyProject.Application.Info.CompanyName;
            this.TextBoxDescription.Text = Tween.My.MyProject.Application.Info.Description;
            this.ChangeLog.Text = Tween.My_Project.Resources.ChangeLog;
            this.TextBoxDescription.Text = Tween.My_Project.Resources.Description;
        }

        private void OKButton_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void TweenAboutBox_Shown(object sender, System.EventArgs e)
        {
            OKButton.Focus();
        }

        public TweenAboutBox()
        {
            Shown += TweenAboutBox_Shown;
            Load += TweenAboutBox_Load;
            InitializeComponent();
        }
    }
}