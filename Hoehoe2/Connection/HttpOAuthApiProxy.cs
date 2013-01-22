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
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class HttpOAuthApiProxy : HttpConnectionOAuth
    {
        private const string ApiHost = "api.twitter.com";

        private static string _proxyHost = string.Empty;

        internal static void SetProxyHost(string value)
        {
            _proxyHost = (string.IsNullOrEmpty(value) || value == ApiHost) ? string.Empty : value;
        }

        protected override string CreateSignature(string tokenSecret, string method, Uri uri, Dictionary<string, string> parameter)
        {
            // パラメタをソート済みディクショナリに詰替（OAuthの仕様）
            var sorted = new SortedDictionary<string, string>(parameter);

            // URLエンコード済みのクエリ形式文字列に変換
            var paramString = CreateQueryString(sorted);

            // アクセス先URLの整形
            var url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);

            // 本来のアクセス先URLに再設定（api.twitter.com固定）
            if (!string.IsNullOrEmpty(_proxyHost) && url.StartsWith(uri.Scheme + "://" + _proxyHost))
            {
                url = url.Replace(uri.Scheme + "://" + _proxyHost, uri.Scheme + "://" + ApiHost);
            }

            // 署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
            var signatureBase = string.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString));

            // 署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
            var key = UrlEncode(ConsumerSecret) + "&";
            if (!string.IsNullOrEmpty(tokenSecret))
            {
                key += UrlEncode(tokenSecret);
            }

            // 鍵生成＆署名生成
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));
            }
        }
    }
}