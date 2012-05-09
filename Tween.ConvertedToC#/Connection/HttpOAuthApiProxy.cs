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

using System.Security;
using System.Text;
namespace Tween
{

	public class HttpOAuthApiProxy : HttpConnectionOAuth
	{

		private const string _apiHost = "api.twitter.com";

		private static string _proxyHost = "";
		static internal string ProxyHost {
			set {
				if (string.IsNullOrEmpty(value) || value == _apiHost) {
					_proxyHost = "";
				} else {
					_proxyHost = value;
				}
			}
		}

		protected override string CreateSignature(string tokenSecret, string method, Uri uri, Dictionary<string, string> parameter)
		{
			//パラメタをソート済みディクショナリに詰替（OAuthの仕様）
			SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(parameter);
			//URLエンコード済みのクエリ形式文字列に変換
			string paramString = CreateQueryString(sorted);
			//アクセス先URLの整形
			string url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);
			//本来のアクセス先URLに再設定（api.twitter.com固定）
			if (!string.IsNullOrEmpty(_proxyHost) && url.StartsWith(uri.Scheme + "://" + _proxyHost)) {
				url = url.Replace(uri.Scheme + "://" + _proxyHost, uri.Scheme + "://" + _apiHost);
			}
			//署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
			string signatureBase = string.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString));
			//署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
			string key = UrlEncode(consumerSecret) + "&";
			if (!string.IsNullOrEmpty(tokenSecret))
				key += UrlEncode(tokenSecret);
			//鍵生成＆署名生成
			using (System.Security.Cryptography.HMACSHA1 hmac = new System.Security.Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))) {
				byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
				return Convert.ToBase64String(hash);
			}
		}

	}
}
