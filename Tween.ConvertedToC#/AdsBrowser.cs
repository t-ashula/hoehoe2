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

using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tween.My_Project;

namespace Tween.TweenCustomControl
{
    public class AdsBrowser : WebBrowser
    {
        private string adsPath;
        private System.Timers.Timer withEventsField_refreshTimer;

        private System.Timers.Timer refreshTimer
        {
            get { return withEventsField_refreshTimer; }
            set
            {
                if (withEventsField_refreshTimer != null)
                {
                    withEventsField_refreshTimer.Elapsed -= refreshTimer_Elapsed;
                }
                withEventsField_refreshTimer = value;
                if (withEventsField_refreshTimer != null)
                {
                    withEventsField_refreshTimer.Elapsed += refreshTimer_Elapsed;
                }
            }
        }

        public AdsBrowser()
            : base()
        {
            Disposed += AdsBrowser_Disposed;

            adsPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllText(adsPath, Resources.ads);

            this.Size = new Size(728 + 5, 90);
            this.ScrollBarsEnabled = false;
            this.AllowWebBrowserDrop = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScriptErrorsSuppressed = true;
            this.TabStop = false;
            this.WebBrowserShortcutsEnabled = false;
            this.Dock = DockStyle.Fill;
            this.Visible = false;
            this.Navigate(adsPath);
            this.Visible = true;

            this.refreshTimer = new System.Timers.Timer(45 * 1000);
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.SynchronizingObject = this;
            this.refreshTimer.Enabled = true;
        }

        private void AdsBrowser_Disposed(object sender, System.EventArgs e)
        {
            this.refreshTimer.Dispose();
            File.Delete(adsPath);
        }

        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Visible = false;
            this.Refresh();
            this.Visible = true;
        }
    }
}