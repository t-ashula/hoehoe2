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

using System.Net;
using System.Text;
namespace Tween
{

	public class HttpConnectionOAuthEcho : HttpConnectionOAuth
	{

		private Uri _realm;
		private Uri _serviceProvider;
		private string _token;

		private string _tokenSecret;
		public Uri Realm {
			set { _realm = value; }
		}

		public Uri ServiceProvider {
			set { _serviceProvider = value; }
		}

		protected override void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query, string token, string tokenSecret)
		{
			//OAuth共通情報取得
			Dictionary<string, string> parameter = GetOAuthParameter(token);
			//OAuth共通情報にquery情報を追加
			if (query != null) {
				foreach (KeyValuePair<string, string> item in query) {
					parameter.Add(item.Key, item.Value);
				}
			}
			//署名の作成・追加(GETメソッド固定。ServiceProvider呼び出し用の署名作成)
			parameter.Add("oauth_signature", CreateSignature(tokenSecret, GetMethod, _serviceProvider, parameter));
			//HTTPリクエストのヘッダに追加
			StringBuilder sb = new StringBuilder("OAuth ");
			sb.AppendFormat("realm=\"{0}://{1}{2}\",", _realm.Scheme, _realm.Host, _realm.AbsolutePath);
			foreach (KeyValuePair<string, string> item in parameter) {
				//各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
				if (item.Key.StartsWith("oauth_")) {
					sb.AppendFormat("{0}=\"{1}\",", item.Key, UrlEncode(item.Value));
				}
			}
			webRequest.Headers.Add("X-Verify-Credentials-Authorization", sb.ToString());
			webRequest.Headers.Add("X-Auth-Service-Provider", string.Format("{0}://{1}{2}", _serviceProvider.Scheme, _serviceProvider.Host, _serviceProvider.AbsolutePath));
		}


		public HttpConnectionOAuthEcho(Uri realm, Uri serviceProvider)
		{
			_realm = realm;
			_serviceProvider = serviceProvider;
		}
	}
}
