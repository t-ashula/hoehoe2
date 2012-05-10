using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
            File.WriteAllText(adsPath, Tween.My.Resources.ads);

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