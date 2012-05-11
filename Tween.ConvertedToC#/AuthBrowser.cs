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
using System.Text.RegularExpressions;

namespace Tween
{
    public partial class AuthBrowser
    {
        public string UrlString { get; set; }

        public string PinString { get; set; }

        public bool Auth { get; set; }

        private InternetSecurityManager SecurityManager;

        private void AuthWebBrowser_DocumentCompleted(System.Object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.AuthWebBrowser.Url.OriginalString == "https://api.twitter.com/oauth/authorize")
            {
                Regex rg = new Regex("<code>(\\d+)</code>");
                Match m = rg.Match(this.AuthWebBrowser.DocumentText);
                if (m.Success)
                {
                    PinString = m.Result("${1}");
                    PinText.Text = m.Result("${1}");
                    PinText.Focus();
                }
            }
        }

        private void AuthBrowser_Load(object sender, System.EventArgs e)
        {
            this.SecurityManager = new InternetSecurityManager(this.AuthWebBrowser);

            this.AuthWebBrowser.Navigate(new Uri(UrlString));
            if (!Auth)
            {
                this.Label1.Visible = false;
                this.PinText.Visible = false;
            }
        }

        private void AuthWebBrowser_Navigating(System.Object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            this.AddressLabel.Text = e.Url.OriginalString;
        }

        private void NextButton_Click(System.Object sender, System.EventArgs e)
        {
            PinString = PinText.Text.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void Cancel_Click(System.Object sender, System.EventArgs e)
        {
            PinString = "";
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public AuthBrowser()
        {
            Load += AuthBrowser_Load;
            InitializeComponent();
        }
    }
}