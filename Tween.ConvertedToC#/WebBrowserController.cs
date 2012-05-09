using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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

using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;
namespace Tween
{

	public class InternetSecurityManager : WebBrowserAPI.IServiceProvider, WebBrowserAPI.IInternetSecurityManager
	{

		#region "HRESULT"
		private class HRESULT
		{
			public static int S_OK = 0x0;
			public static int S_FALSE = 0x1;
			public static int E_NOTIMPL = 0x80004001;
			public static int E_NOINTERFACE = 0x80004002;
		}
		#endregion

		#region "WebBrowserAPI"
		private class WebBrowserAPI
		{

			public static int INET_E_DEFAULT_ACTION = 0x800c0011;
			public enum URLZONE
			{
				URLZONE_LOCAL_MACHINE = 0,
				URLZONE_INTRANET = URLZONE_LOCAL_MACHINE + 1,
				URLZONE_TRUSTED = URLZONE_INTRANET + 1,
				URLZONE_INTERNET = URLZONE_TRUSTED + 1,
				URLZONE_UNTRUSTED = URLZONE_INTERNET + 1
			}


			public static int URLACTION_MIN = 0x1000;
			public static int URLACTION_DOWNLOAD_MIN = 0x1000;
			public static int URLACTION_DOWNLOAD_SIGNED_ACTIVEX = 0x1001;
			public static int URLACTION_DOWNLOAD_UNSIGNED_ACTIVEX = 0x1004;
			public static int URLACTION_DOWNLOAD_CURR_MAX = 0x1004;

			public static int URLACTION_DOWNLOAD_MAX = 0x11ff;
			public static int URLACTION_ACTIVEX_MIN = 0x1200;
			public static int URLACTION_ACTIVEX_RUN = 0x1200;
			public static int URLPOLICY_ACTIVEX_CHECK_LIST = 0x10000;
			public static int URLACTION_ACTIVEX_OVERRIDE_OBJECT_SAFETY = 0x1201;
			public static int URLACTION_ACTIVEX_OVERRIDE_DATA_SAFETY = 0x1202;
			public static int URLACTION_ACTIVEX_OVERRIDE_SCRIPT_SAFETY = 0x1203;
			public static int URLACTION_SCRIPT_OVERRIDE_SAFETY = 0x1401;
			public static int URLACTION_ACTIVEX_CONFIRM_NOOBJECTSAFETY = 0x1204;
			public static int URLACTION_ACTIVEX_TREATASUNTRUSTED = 0x1205;
			public static int URLACTION_ACTIVEX_NO_WEBOC_SCRIPT = 0x1206;
			public static int URLACTION_ACTIVEX_CURR_MAX = 0x1206;

			public static int URLACTION_ACTIVEX_MAX = 0x13ff;
			public static int URLACTION_SCRIPT_MIN = 0x1400;
			public static int URLACTION_SCRIPT_RUN = 0x1400;
			public static int URLACTION_SCRIPT_JAVA_USE = 0x1402;
			public static int URLACTION_SCRIPT_SAFE_ACTIVEX = 0x1405;
			public static int URLACTION_CROSS_DOMAIN_DATA = 0x1406;
			public static int URLACTION_SCRIPT_PASTE = 0x1407;
			public static int URLACTION_SCRIPT_CURR_MAX = 0x1407;

			public static int URLACTION_SCRIPT_MAX = 0x15ff;
			public static int URLACTION_HTML_MIN = 0x1600;
				// aggregate next two
			public static int URLACTION_HTML_SUBMIT_FORMS = 0x1601;
				//
			public static int URLACTION_HTML_SUBMIT_FORMS_FROM = 0x1602;
				//
			public static int URLACTION_HTML_SUBMIT_FORMS_TO = 0x1603;
			public static int URLACTION_HTML_FONT_DOWNLOAD = 0x1604;
				// derive from Java custom policy
			public static int URLACTION_HTML_JAVA_RUN = 0x1605;
			public static int URLACTION_HTML_USERDATA_SAVE = 0x1606;
			public static int URLACTION_HTML_SUBFRAME_NAVIGATE = 0x1607;
			public static int URLACTION_HTML_META_REFRESH = 0x1608;
			public static int URLACTION_HTML_MIXED_CONTENT = 0x1609;

			public static int URLACTION_HTML_MAX = 0x17ff;
			public static int URLACTION_SHELL_MIN = 0x1800;
			public static int URLACTION_SHELL_INSTALL_DTITEMS = 0x1800;
			public static int URLACTION_SHELL_MOVE_OR_COPY = 0x1802;
			public static int URLACTION_SHELL_FILE_DOWNLOAD = 0x1803;
			public static int URLACTION_SHELL_VERB = 0x1804;
			public static int URLACTION_SHELL_WEBVIEW_VERB = 0x1805;
			public static int URLACTION_SHELL_SHELLEXECUTE = 0x1806;
			public static int URLACTION_SHELL_CURR_MAX = 0x1806;

			public static int URLACTION_SHELL_MAX = 0x19ff;

			public static int URLACTION_NETWORK_MIN = 0x1a00;
			public static int URLACTION_CREDENTIALS_USE = 0x1a00;
			public static int URLPOLICY_CREDENTIALS_SILENT_LOGON_OK = 0x0;
			public static int URLPOLICY_CREDENTIALS_MUST_PROMPT_USER = 0x10000;
			public static int URLPOLICY_CREDENTIALS_CONDITIONAL_PROMPT = 0x20000;

			public static int URLPOLICY_CREDENTIALS_ANONYMOUS_ONLY = 0x30000;
			public static int URLACTION_AUTHENTICATE_CLIENT = 0x1a01;
			public static int URLPOLICY_AUTHENTICATE_CLEARTEXT_OK = 0x0;
			public static int URLPOLICY_AUTHENTICATE_CHALLENGE_RESPONSE = 0x10000;

			public static int URLPOLICY_AUTHENTICATE_MUTUAL_ONLY = 0x30000;

			public static int URLACTION_COOKIES = 0x1a02;

			public static int URLACTION_COOKIES_SESSION = 0x1a03;

			public static int URLACTION_CLIENT_CERT_PROMPT = 0x1a04;
			public static int URLACTION_COOKIES_THIRD_PARTY = 0x1a05;

			public static int URLACTION_COOKIES_SESSION_THIRD_PARTY = 0x1a06;

			public static int URLACTION_COOKIES_ENABLED = 0x1a10;
			public static int URLACTION_NETWORK_CURR_MAX = 0x1a10;

			public static int URLACTION_NETWORK_MAX = 0x1bff;

			public static int URLACTION_JAVA_MIN = 0x1c00;
			public static int URLACTION_JAVA_PERMISSIONS = 0x1c00;
			public static int URLPOLICY_JAVA_PROHIBIT = 0x0;
			public static int URLPOLICY_JAVA_HIGH = 0x10000;
			public static int URLPOLICY_JAVA_MEDIUM = 0x20000;
			public static int URLPOLICY_JAVA_LOW = 0x30000;
			public static int URLPOLICY_JAVA_CUSTOM = 0x800000;
			public static int URLACTION_JAVA_CURR_MAX = 0x1c00;

			public static int URLACTION_JAVA_MAX = 0x1cff;

			// The following Infodelivery actions should have no default policies
			// in the registry.  They assume that no default policy means fall
			// back to the global restriction.  If an admin sets a policy per
			// zone, then it overrides the global restriction.

			public static int URLACTION_INFODELIVERY_MIN = 0x1d00;
			public static int URLACTION_INFODELIVERY_NO_ADDING_CHANNELS = 0x1d00;
			public static int URLACTION_INFODELIVERY_NO_EDITING_CHANNELS = 0x1d01;
			public static int URLACTION_INFODELIVERY_NO_REMOVING_CHANNELS = 0x1d02;
			public static int URLACTION_INFODELIVERY_NO_ADDING_SUBSCRIPTIONS = 0x1d03;
			public static int URLACTION_INFODELIVERY_NO_EDITING_SUBSCRIPTIONS = 0x1d04;
			public static int URLACTION_INFODELIVERY_NO_REMOVING_SUBSCRIPTIONS = 0x1d05;
			public static int URLACTION_INFODELIVERY_NO_CHANNEL_LOGGING = 0x1d06;
			public static int URLACTION_INFODELIVERY_CURR_MAX = 0x1d06;
			public static int URLACTION_INFODELIVERY_MAX = 0x1dff;
			public static int URLACTION_CHANNEL_SOFTDIST_MIN = 0x1e00;
			public static int URLACTION_CHANNEL_SOFTDIST_PERMISSIONS = 0x1e05;
			public static int URLPOLICY_CHANNEL_SOFTDIST_PROHIBIT = 0x10000;
			public static int URLPOLICY_CHANNEL_SOFTDIST_PRECACHE = 0x20000;
			public static int URLPOLICY_CHANNEL_SOFTDIST_AUTOINSTALL = 0x30000;

			public static int URLACTION_CHANNEL_SOFTDIST_MAX = 0x1eff;
			// For each action specified above the system maintains
			// a set of policies for the action.
			// The only policies supported currently are permissions (i.e. is something allowed)
			// and logging status.
			// IMPORTANT: If you are defining your own policies don't overload the meaning of the
			// loword of the policy. You can use the hiword to store any policy bits which are only
			// meaningful to your action.
			// For an example of how to do this look at the URLPOLICY_JAVA above

			// Permissions
			public static byte URLPOLICY_ALLOW = 0x0;
			public static byte URLPOLICY_QUERY = 0x1;

			public static byte URLPOLICY_DISALLOW = 0x3;
			// Notifications are not done when user already queried.
			public static int URLPOLICY_NOTIFY_ON_ALLOW = 0x10;

			public static int URLPOLICY_NOTIFY_ON_DISALLOW = 0x20;
			// Logging is done regardless of whether user was queried.
			public static int URLPOLICY_LOG_ON_ALLOW = 0x40;

			public static int URLPOLICY_LOG_ON_DISALLOW = 0x80;

			public static int URLPOLICY_MASK_PERMISSIONS = 0xf;


			public static int URLPOLICY_DONTCHECKDLGBOX = 0x100;

			// ----------------------------------------------------------------------
			// ここ以下は COM Interface の宣言です。
			public static Guid IID_IProfferService = new Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0");
			public static Guid SID_SProfferService = new Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0");

			public static Guid IID_IInternetSecurityManager = new Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b");
			[ComImport(), Guid("6d5140c1-7436-11ce-8034-00aa006009fa"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IServiceProvider
			{
				[PreserveSig()]
				int QueryService(				[In()]
ref Guid guidService, 				[In()]
ref Guid riid, 				[Out()]
ref IntPtr ppvObject);
			}

			[ComImport(), Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IProfferService
			{
				[PreserveSig()]
				int ProfferService(				[In()]
ref Guid guidService, 				[In()]
IServiceProvider psp, 				[Out()]
ref int cookie);

				[PreserveSig()]
				int RevokeService(				[In()]
int cookie);
			}

			[ComImport(), Guid("79eac9ed-baf9-11ce-8c82-00aa004ba90b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IInternetSecurityMgrSite
			{
				[PreserveSig()]
				int GetWindow(				[Out()]
ref IntPtr hwnd);

				[PreserveSig()]
				int EnableModeless(				[In(), MarshalAs(UnmanagedType.Bool)]
bool fEnable);
			}

			[ComImport(), Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IInternetSecurityManager
			{
				[PreserveSig()]
				int SetSecuritySite(				[In()]
IInternetSecurityMgrSite pSite);

				[PreserveSig()]
				int GetSecuritySite(				[Out()]
ref IInternetSecurityMgrSite pSite);

				[PreserveSig()]
				int MapUrlToZone(				[In(), MarshalAs(UnmanagedType.LPWStr)]
string pwszUrl, 				[Out()]
ref int pdwZone, int dwFlags);

				[PreserveSig()]
				int GetSecurityId(				[MarshalAs(UnmanagedType.LPWStr)]
string pwszUrl, 				[MarshalAs(UnmanagedType.LPArray)]
byte[] pbSecurityId, ref UInt32 pcbSecurityId, UInt32 dwReserved);

				[PreserveSig()]
				int ProcessUrlAction(				[In(), MarshalAs(UnmanagedType.LPWStr)]
string pwszUrl, int dwAction, 				[Out()]
ref byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved);

				[PreserveSig()]
				int QueryCustomPolicy(				[In(), MarshalAs(UnmanagedType.LPWStr)]
string pwszUrl, ref Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved);

				[PreserveSig()]
				int SetZoneMapping(int dwZone, 				[In(), MarshalAs(UnmanagedType.LPWStr)]
string lpszPattern, int dwFlags);

				[PreserveSig()]
				int GetZoneMappings(int dwZone, ref System.Runtime.InteropServices.ComTypes.IEnumString ppenumString, int dwFlags);
			}
		}
		#endregion

		[Flags()]
		public enum POLICY : int
		{
			ALLOW_ACTIVEX = 0x1,
			ALLOW_SCRIPT = 0x2
		}

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
			//ActiveXを初期化する
			int hresult = 0;

			do {
				Thread.Sleep(100);
				Application.DoEvents();
			} while (!(_WebBrowser.ReadyState == WebBrowserReadyState.Complete));

			ocx = _WebBrowser.ActiveXInstance;

			// IServiceProvider.QueryService() を使って IProfferService を取得
			ocxServiceProvider = (WebBrowserAPI.IServiceProvider)ocx;

			try {
				hresult = ocxServiceProvider.QueryService(WebBrowserAPI.SID_SProfferService, WebBrowserAPI.IID_IProfferService, profferServicePtr);
			} catch (SEHException ex) {
			} catch (ExternalException ex) {
				MyCommon.TraceOut(ex, "ocxServiceProvider.QueryService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
				return;
			}


			profferService = (WebBrowserAPI.IProfferService)Marshal.GetObjectForIUnknown(profferServicePtr);

			// IProfferService.ProfferService() を使って
			// 自分を IInternetSecurityManager として提供
			try {
				hresult = profferService.ProfferService(WebBrowserAPI.IID_IInternetSecurityManager, this, cookie: 0);
			} catch (SEHException ex) {
			} catch (ExternalException ex) {
				MyCommon.TraceOut(ex, "IProfferSerive.ProfferService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
				return;
			}

		}

		private int QueryService(ref System.Guid guidService, ref System.Guid riid, ref System.IntPtr ppvObject)
		{

			ppvObject = IntPtr.Zero;
			if (guidService.CompareTo(WebBrowserAPI.IID_IInternetSecurityManager) == 0) {
				// 自分から IID_IInternetSecurityManager を
				// QueryInterface して返す
				IntPtr punk = Marshal.GetIUnknownForObject(this);
				return Marshal.QueryInterface(punk, ref riid, out ppvObject);
			}
			return HRESULT.E_NOINTERFACE;
		}

		private int GetSecurityId(string pwszUrl, byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved)
		{
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int GetSecuritySite(ref WebBrowserAPI.IInternetSecurityMgrSite pSite)
		{
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int GetZoneMappings(int dwZone, ref System.Runtime.InteropServices.ComTypes.IEnumString ppenumString, int dwFlags)
		{
			ppenumString = null;
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int MapUrlToZone(string pwszUrl, ref int pdwZone, int dwFlags)
		{
			pdwZone = 0;
			if (pwszUrl == "about:blank")
				return WebBrowserAPI.INET_E_DEFAULT_ACTION;
			try {
				string urlStr = MyCommon.IDNDecode(pwszUrl);
				if (urlStr == null)
					return WebBrowserAPI.URLPOLICY_DISALLOW;
				Uri url = new Uri(urlStr);
				if (url.Scheme == "data") {
					return WebBrowserAPI.URLPOLICY_DISALLOW;
				}
			} catch (Exception ex) {
				return WebBrowserAPI.URLPOLICY_DISALLOW;
			}
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int ProcessUrlAction(string pwszUrl, int dwAction, ref byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved)
		{
			//スクリプト実行状態かを検査しポリシー設定
			if (WebBrowserAPI.URLACTION_SCRIPT_MIN <= dwAction & dwAction <= WebBrowserAPI.URLACTION_SCRIPT_MAX) {
				// スクリプト実行状態
				if ((_Policy & POLICY.ALLOW_SCRIPT) == POLICY.ALLOW_SCRIPT) {
					pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
				} else {
					pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
				}
				if (Regex.IsMatch(pwszUrl, "^https?://((api\\.)?twitter\\.com/|([a-zA-Z0-9]+\\.)?twimg\\.com/|ssl\\.google-analytics\\.com/)"))
					pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
				return HRESULT.S_OK;
			}
			// ActiveX実行状態かを検査しポリシー設定
			if (WebBrowserAPI.URLACTION_ACTIVEX_MIN <= dwAction & dwAction <= WebBrowserAPI.URLACTION_ACTIVEX_MAX) {
				// ActiveX実行状態
				if ((_Policy & POLICY.ALLOW_ACTIVEX) == POLICY.ALLOW_ACTIVEX) {
					pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
				} else {
					pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
				}
				return HRESULT.S_OK;
			}
			//他のものについてはデフォルト処理
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int QueryCustomPolicy(string pwszUrl, ref System.Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved)
		{
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int SetSecuritySite(WebBrowserAPI.IInternetSecurityMgrSite pSite)
		{
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}

		private int SetZoneMapping(int dwZone, string lpszPattern, int dwFlags)
		{
			return WebBrowserAPI.INET_E_DEFAULT_ACTION;
		}


		public POLICY SecurityPolicy {
			get { return _Policy; }
			set { _Policy = value; }
		}

	}
}
