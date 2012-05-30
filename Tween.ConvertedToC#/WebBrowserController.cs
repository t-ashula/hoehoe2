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
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    public class InternetSecurityManager : WebBrowserAPI.IServiceProvider, WebBrowserAPI.IInternetSecurityManager
    {
        private object ocx = new object();
        private WebBrowserAPI.IServiceProvider ocxServiceProvider;
        private IntPtr profferServicePtr = new IntPtr();
        private WebBrowserAPI.IProfferService profferService;

        // DefaultですべてDisAllow
        private POLICY _Policy = 0;

        public InternetSecurityManager(System.Windows.Forms.WebBrowser _WebBrowser)
        {
            // ActiveXコントロール取得
            _WebBrowser.DocumentText = "about:blank";

            // ActiveXを初期化する
            do
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            while (!(_WebBrowser.ReadyState == WebBrowserReadyState.Complete));

            this.ocx = _WebBrowser.ActiveXInstance;

            // IServiceProvider.QueryService() を使って IProfferService を取得
            this.ocxServiceProvider = (WebBrowserAPI.IServiceProvider)this.ocx;

            int hresult = 0;
            try
            {
                hresult = this.ocxServiceProvider.QueryService(ref WebBrowserAPI.SID_SProfferService, ref WebBrowserAPI.IID_IProfferService, out this.profferServicePtr);
            }
            catch (SEHException)
            {
            }
            catch (ExternalException ex)
            {
                MyCommon.TraceOut(ex, "ocxServiceProvider.QueryService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }

            this.profferService = (WebBrowserAPI.IProfferService)Marshal.GetObjectForIUnknown(this.profferServicePtr);

            // IProfferService.ProfferService() を使って
            // 自分を IInternetSecurityManager として提供
            try
            {
                int cookie;
                hresult = this.profferService.ProfferService(ref WebBrowserAPI.IID_IInternetSecurityManager, this, out cookie);
            }
            catch (SEHException)
            {
            }
            catch (ExternalException ex)
            {
                MyCommon.TraceOut(ex, "IProfferSerive.ProfferService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }
        }

        [Flags]
        public enum POLICY : int
        {
            ALLOW_ACTIVEX = 0x1,
            ALLOW_SCRIPT = 0x2
        }

        public POLICY SecurityPolicy
        {
            get { return this._Policy; }
            set { this._Policy = value; }
        }

        public int QueryService(ref System.Guid guidService, ref System.Guid riid, out System.IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;
            if (guidService.CompareTo(WebBrowserAPI.IID_IInternetSecurityManager) == 0)
            {
                // 自分から IID_IInternetSecurityManager を QueryInterface して返す
                IntPtr punk = Marshal.GetIUnknownForObject(this);
                return Marshal.QueryInterface(punk, ref riid, out ppvObject);
            }

            return HRESULT.E_NOINTERFACE;
        }

        public int GetSecurityId(string pwszUrl, byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved)
        {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int GetSecuritySite(ref WebBrowserAPI.IInternetSecurityMgrSite pSite)
        {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int GetZoneMappings(int dwZone, ref System.Runtime.InteropServices.ComTypes.IEnumString ppenumString, int dwFlags)
        {
            ppenumString = null;
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int MapUrlToZone(string pwszUrl, out int pdwZone, int dwFlags)
        {
            pdwZone = 0;
            if (pwszUrl == "about:blank")
            {
                return WebBrowserAPI.INET_E_DEFAULT_ACTION;
            }

            try
            {
                string urlStr = MyCommon.IDNDecode(pwszUrl);
                if (string.IsNullOrEmpty(urlStr))
                {
                    return WebBrowserAPI.URLPOLICY_DISALLOW;
                }

                Uri url = new Uri(urlStr);
                if (url.Scheme == "data")
                {
                    return WebBrowserAPI.URLPOLICY_DISALLOW;
                }
            }
            catch (Exception)
            {
                return WebBrowserAPI.URLPOLICY_DISALLOW;
            }

            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int ProcessUrlAction(string pwszUrl, int dwAction, ref byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved)
        {
            // スクリプト実行状態かを検査しポリシー設定
            if (WebBrowserAPI.URLACTION_SCRIPT_MIN <= dwAction & dwAction <= WebBrowserAPI.URLACTION_SCRIPT_MAX)
            {
                // スクリプト実行状態
                if ((this._Policy & POLICY.ALLOW_SCRIPT) == POLICY.ALLOW_SCRIPT)
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                }
                else
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
                }

                if (Regex.IsMatch(pwszUrl, "^https?://((api\\.)?twitter\\.com/|([a-zA-Z0-9]+\\.)?twimg\\.com/|ssl\\.google-analytics\\.com/)"))
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                }

                return HRESULT.S_OK;
            }

            // ActiveX実行状態かを検査しポリシー設定
            if (WebBrowserAPI.URLACTION_ACTIVEX_MIN <= dwAction & dwAction <= WebBrowserAPI.URLACTION_ACTIVEX_MAX)
            {
                // ActiveX実行状態
                if ((this._Policy & POLICY.ALLOW_ACTIVEX) == POLICY.ALLOW_ACTIVEX)
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                }
                else
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
                }

                return HRESULT.S_OK;
            }

            // 他のものについてはデフォルト処理
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int QueryCustomPolicy(string pwszUrl, ref System.Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved)
        {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int SetSecuritySite(WebBrowserAPI.IInternetSecurityMgrSite pSite)
        {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        public int SetZoneMapping(int dwZone, string lpszPattern, int dwFlags)
        {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        #region "HRESULT"

        private class HRESULT
        {
            public static int S_OK = 0x0;
            public static int S_FALSE = 0x1;
            public static int E_NOTIMPL = unchecked((int)0x80004001);
            public static int E_NOINTERFACE = unchecked((int)0x80004002);
        }

        #endregion "HRESULT"
    }
}