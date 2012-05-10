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