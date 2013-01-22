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
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// OAuth認証を使用するHTTP通信。HMAC-SHA1固定
    /// </summary>
    /// <remarks>
    /// 使用前に認証情報を設定する。認証確認を伴う場合はAuthenticate系のメソッドを、認証不要な場合はInitializeを呼ぶこと。
    /// </remarks>
    public class HttpConnectionOAuth : HttpConnection, IHttpConnection
    {
        /// <summary>
        /// OAuth署名のoauth_timestamp算出用基準日付（1970/1/1 00:00:00）
        /// </summary>
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        /// <summary>
        /// OAuth署名のoauth_nonce算出用乱数クラス
        /// </summary>
        private static readonly Random NonceRandom = new Random();

        /// <summary>
        /// OAuthのアクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
        /// </summary>
        private string _token = string.Empty;

        /// <summary>
        /// OAuthの署名作成用秘密アクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
        /// </summary>
        private string _tokenSecret = string.Empty;

        /// <summary>
        /// OAuthのコンシューマー鍵
        /// </summary>
        private string _consumerKey;

        /// <summary>
        /// OAuthの署名作成用秘密コンシューマーデータ
        /// </summary>
        private string _consumerSecret;

        /// <summary>
        /// 認証成功時の応答でユーザー情報を取得する場合のキー。設定しない場合は、AuthUsernameもブランクのままとなる
        /// </summary>
        private string _userIdentKey = string.Empty;

        /// <summary>
        /// 認証成功時の応答でユーザーID情報を取得する場合のキー。設定しない場合は、AuthUserIdもブランクのままとなる
        /// </summary>
        private string _userIdIdentKey = string.Empty;

        /// <summary>
        /// 認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
        /// </summary>
        private string _authorizedUsername = string.Empty;

        /// <summary>
        /// 認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
        /// </summary>
        private long _authorizedUserId;

        /// <summary>
        /// Stream用のHttpWebRequest
        /// </summary>
        private HttpWebRequest _streamReq;

        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken
        {
            get { return _token; }
        }

        /// <summary>
        /// アクセストークン秘密鍵
        /// </summary>
        public string AccessTokenSecret
        {
            get { return _tokenSecret; }
        }

        /// <summary>
        /// 認証済みユーザー名
        /// </summary>
        public string AuthUsername
        {
            get { return _authorizedUsername; }
        }

        /// <summary>
        /// 認証済みユーザーId
        /// </summary>
        public long AuthUserId
        {
            get { return _authorizedUserId; }
            set { _authorizedUserId = value; }
        }

        /// <summary>
        /// OAuthの署名作成用秘密コンシューマーデータ(API Proxy 用)
        /// </summary>
        protected string ConsumerSecret
        {
            get { return _consumerSecret; }
        }

        /// <summary>
        /// 初期化。各種トークンの設定とユーザー識別情報設定
        /// </summary>
        /// <param name="consumerKey">コンシューマー鍵</param>
        /// <param name="consumerSecret">コンシューマー秘密鍵</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
        /// <param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
        public void Initialize(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string userIdentifier, string userIdIdentifier)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = accessToken;
            _tokenSecret = accessTokenSecret;
            _userIdentKey = userIdentifier;
            _userIdIdentKey = userIdIdentifier;
        }

        /// <summary>
        /// 初期化。各種トークンの設定とユーザー識別情報設定
        /// </summary>
        /// <param name="consumerKey">コンシューマー鍵</param>
        /// <param name="consumerSecret">コンシューマー秘密鍵</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
        /// <param name="username">認証済みユーザー名</param>
        /// <param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
        public void Initialize(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string username, long userId, string userIdentifier, string userIdIdentifier)
        {
            Initialize(consumerKey, consumerSecret, accessToken, accessTokenSecret, userIdentifier, userIdIdentifier);
            _authorizedUsername = username;
            _authorizedUserId = userId;
        }

        /// <summary>
        /// OAuth認証で指定のURLとHTTP通信を行い、結果を返す
        /// </summary>
        /// <param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        /// <param name="requestUri">通信先URI</param>
        /// <param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        /// <param name="content">[OUT]HTTP応答のボディデータ</param>
        /// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
        /// <param name="callback">処理終了直前に呼ばれるコールバック関数のデリゲート 不要な場合はNothingを渡すこと</param>
        /// <returns>HTTP応答のステータスコード</returns>
        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo, CallbackDelegate callback)
        {
            // 認証済かチェック
            if (string.IsNullOrEmpty(_token))
            {
                return HttpStatusCode.Unauthorized;
            }

            HttpWebRequest webReq = CreateRequest(method, requestUri, param, false);

            // OAuth認証ヘッダを付加
            AppendOAuthInfo(webReq, param, _token, _tokenSecret);

            HttpStatusCode code = content == null ?
                GetResponse(webReq, headerInfo, false) :
                GetResponse(webReq, ref content, headerInfo, false);

            if (callback != null)
            {
                StackFrame frame = new StackFrame(1);
                callback(frame.GetMethod().Name, ref code, ref content);
            }

            return code;
        }

        /// <summary>
        /// バイナリアップロード
        /// </summary>
        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, List<KeyValuePair<string, FileInfo>> binary, ref string content, Dictionary<string, string> headerInfo, CallbackDelegate callback)
        {
            // 認証済かチェック
            if (string.IsNullOrEmpty(_token))
            {
                return HttpStatusCode.Unauthorized;
            }

            HttpWebRequest webReq = CreateRequest(method, requestUri, param, binary, false);

            // OAuth認証ヘッダを付加
            AppendOAuthInfo(webReq, null, _token, _tokenSecret);

            HttpStatusCode code = content == null ?
                GetResponse(webReq, headerInfo, false) :
                GetResponse(webReq, ref content, headerInfo, false);

            if (callback != null)
            {
                StackFrame frame = new StackFrame(1);
                callback(frame.GetMethod().Name, ref code, ref content);
            }

            return code;
        }

        /// <summary>
        /// OAuth認証で指定のURLとHTTP通信を行い、ストリームを返す
        /// </summary>
        /// <param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        /// <param name="requestUri">通信先URI</param>
        /// <param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        /// <param name="content">[OUT]HTTP応答のボディストリーム</param>
        /// <returns>HTTP応答のステータスコード</returns>
        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref Stream content, string userAgent)
        {
            // 認証済かチェック
            if (string.IsNullOrEmpty(_token))
            {
                return HttpStatusCode.Unauthorized;
            }

            RequestAbort();
            _streamReq = CreateRequest(method, requestUri, param, false);

            // User-Agent指定がある場合は付加
            if (!string.IsNullOrEmpty(userAgent))
            {
                _streamReq.UserAgent = userAgent;
            }

            // OAuth認証ヘッダを付加
            AppendOAuthInfo(_streamReq, param, _token, _tokenSecret);
            try
            {
                HttpWebResponse webRes = (HttpWebResponse)_streamReq.GetResponse();
                content = webRes.GetResponseStream();
                return webRes.StatusCode;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    return res.StatusCode;
                }

                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RequestAbort()
        {
            try
            {
                if (_streamReq != null)
                {
                    _streamReq.Abort();
                    _streamReq = null;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// OAuth認証の開始要求（リクエストトークン取得）。PIN入力用の前段
        /// </summary>
        /// <remarks>
        /// 呼び出し元では戻されたurlをブラウザで開き、認証完了後PIN入力を受け付けて、リクエストトークンと共にAuthenticatePinFlowを呼び出す
        /// </remarks>
        /// <param name="requestTokenUrl">リクエストトークンの取得先URL</param>
        /// <param name="requestUri">ブラウザで開く認証用URLのベース</param>
        /// <param name="requestToken">[OUT]認証要求で戻されるリクエストトークン。使い捨て</param>
        /// <param name="authUri">[OUT]requestUriを元に生成された認証用URL。通常はリクエストトークンをクエリとして付加したUri</param>
        /// <returns>取得結果真偽値</returns>
        public bool AuthenticatePinFlowRequest(string requestTokenUrl, string authorizeUrl, ref string requestToken, ref Uri authUri)
        {
            // PIN-based flow
            authUri = GetAuthenticatePageUri(requestTokenUrl, authorizeUrl, ref requestToken);
            return authUri != null;
        }

        /// <summary>
        /// OAuth認証のアクセストークン取得。PIN入力用の後段
        /// </summary>
        /// <remarks>
        /// 事前にAuthenticatePinFlowRequestを呼んで、ブラウザで認証後に表示されるPINを入力してもらい、その値とともに呼び出すこと
        /// </remarks>
        /// <param name="accessTokenUrl">アクセストークンの取得先URL</param>
        /// <param name="requestUri">AuthenticatePinFlowRequestで取得したリクエストトークン</param>
        /// <param name="pinCode">Webで認証後に表示されるPINコード</param>
        /// <returns>取得結果真偽値</returns>
        public HttpStatusCode AuthenticatePinFlow(string accessTokenUrl, string requestToken, string pinCode)
        {
            // PIN-based flow
            if (string.IsNullOrEmpty(requestToken))
            {
                throw new Exception("Sequence error.(requestToken is blank)");
            }

            // アクセストークン取得
            string content = string.Empty;
            HttpStatusCode httpCode = GetOAuthToken(new Uri(accessTokenUrl), pinCode, requestToken, null, ref content);
            if (httpCode != HttpStatusCode.OK)
            {
                return httpCode;
            }

            NameValueCollection accessTokenData = ParseQueryString(content);
            if (accessTokenData == null)
            {
                throw new InvalidDataException("Return value is null.");
            }

            _token = accessTokenData["oauth_token"];
            _tokenSecret = accessTokenData["oauth_token_secret"];

            // サービスごとの独自拡張対応
            if (!string.IsNullOrEmpty(_userIdentKey))
            {
                _authorizedUsername = accessTokenData[_userIdentKey];
            }
            else
            {
                _authorizedUsername = string.Empty;
            }

            if (!string.IsNullOrEmpty(_userIdIdentKey))
            {
                try
                {
                    _authorizedUserId = Convert.ToInt64(accessTokenData[_userIdIdentKey]);
                }
                catch (Exception)
                {
                    _authorizedUserId = 0;
                }
            }
            else
            {
                _authorizedUserId = 0;
            }

            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidDataException("Token is null.");
            }

            return HttpStatusCode.OK;
        }

        /// <summary>
        /// OAuth認証のアクセストークン取得。xAuth方式
        /// </summary>
        /// <param name="accessTokenUrl">アクセストークンの取得先URL</param>
        /// <param name="username">認証用ユーザー名</param>
        /// <param name="password">認証用パスワード</param>
        /// <returns>取得結果真偽値</returns>
        public HttpStatusCode AuthenticateXAuth(Uri accessTokenUrl, string username, string password, ref string content)
        {
            // ユーザー・パスワードチェック
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Sequence error.(username or password is blank)");
            }

            // xAuthの拡張パラメータ設定
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("x_auth_mode", "client_auth");
            parameter.Add("x_auth_username", username);
            parameter.Add("x_auth_password", password);

            // アクセストークン取得
            HttpStatusCode httpCode = GetOAuthToken(accessTokenUrl, string.Empty, string.Empty, parameter, ref content);
            if (httpCode != HttpStatusCode.OK)
            {
                return httpCode;
            }

            NameValueCollection accessTokenData = ParseQueryString(content);
            if (accessTokenData == null)
            {
                throw new InvalidDataException("Return value is null.");
            }

            _token = accessTokenData["oauth_token"];
            _tokenSecret = accessTokenData["oauth_token_secret"];

            // サービスごとの独自拡張対応
            if (!string.IsNullOrEmpty(_userIdentKey))
            {
                _authorizedUsername = accessTokenData[_userIdentKey];
            }
            else
            {
                _authorizedUsername = string.Empty;
            }

            if (!string.IsNullOrEmpty(_userIdIdentKey))
            {
                try
                {
                    _authorizedUserId = Convert.ToInt64(accessTokenData[_userIdIdentKey]);
                }
                catch (Exception)
                {
                    _authorizedUserId = 0;
                }
            }
            else
            {
                _authorizedUserId = 0;
            }

            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidDataException("Token is null.");
            }

            return HttpStatusCode.OK;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="accessTokenUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        HttpStatusCode IHttpConnection.Authenticate(Uri accessTokenUrl, string username, string password, ref string content)
        {
            return AuthenticateXAuth(accessTokenUrl, username, password, ref content);
        }

        /// <summary>
        /// HTTPリクエストにOAuth関連ヘッダを追加
        /// </summary>
        /// <param name="webRequest">追加対象のHTTPリクエスト</param>
        /// <param name="query">OAuth追加情報＋クエリ or POSTデータ</param>
        /// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        /// <param name="tokenSecret">アクセストークンシークレット。認証処理では空文字列</param>
        protected virtual void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query, string token, string tokenSecret)
        {
            // OAuth共通情報取得
            Dictionary<string, string> parameter = GetOAuthParameter(token);

            // OAuth共通情報にquery情報を追加
            if (query != null)
            {
                foreach (KeyValuePair<string, string> item in query)
                {
                    parameter.Add(item.Key, item.Value);
                }
            }

            // 署名の作成・追加
            parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter));

            // HTTPリクエストのヘッダに追加
            StringBuilder sb = new StringBuilder("OAuth ");
            foreach (KeyValuePair<string, string> item in parameter)
            {
                // 各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
                if (item.Key.StartsWith("oauth_"))
                {
                    sb.AppendFormat("{0}=\"{1}\",", item.Key, UrlEncode(item.Value));
                }
            }

            webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString());
        }

        /// <summary>
        /// OAuthで使用する共通情報を取得する
        /// </summary>
        /// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        /// <returns>OAuth情報のディクショナリ</returns>
        protected Dictionary<string, string> GetOAuthParameter(string token)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("oauth_consumer_key", _consumerKey);
            parameter.Add("oauth_signature_method", "HMAC-SHA1");
            parameter.Add("oauth_timestamp", Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString()); // epoch秒
            parameter.Add("oauth_nonce", NonceRandom.Next(123400, 9999999).ToString());
            parameter.Add("oauth_version", "1.0");

            // トークンがあれば追加
            if (!string.IsNullOrEmpty(token))
            {
                parameter.Add("oauth_token", token);
            }

            return parameter;
        }

        /// <summary>
        /// OAuth認証ヘッダの署名作成
        /// </summary>
        /// <param name="tokenSecret">アクセストークン秘密鍵</param>
        /// <param name="method">HTTPメソッド文字列</param>
        /// <param name="uri">アクセス先Uri</param>
        /// <param name="parameter">クエリ、もしくはPOSTデータ</param>
        /// <returns>署名文字列</returns>
        protected virtual string CreateSignature(string tokenSecret, string method, Uri uri, Dictionary<string, string> parameter)
        {
            // パラメタをソート済みディクショナリに詰替（OAuthの仕様）
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(parameter);

            // URLエンコード済みのクエリ形式文字列に変換
            string paramString = CreateQueryString(sorted);

            // アクセス先URLの整形
            string url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);

            // 署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
            string signatureBase = string.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString));

            // 署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
            string key = UrlEncode(_consumerSecret) + "&";
            if (!string.IsNullOrEmpty(_tokenSecret))
            {
                key += UrlEncode(_tokenSecret);
            }

            // 鍵生成＆署名生成
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));
            }
        }

        /// <summary>
        /// OAuth認証のリクエストトークン取得。リクエストトークンと組み合わせた認証用のUriも生成する
        /// </summary>
        /// <param name="accessTokenUrl">リクエストトークンの取得先URL</param>
        /// <param name="authorizeUrl">ブラウザで開く認証用URLのベース</param>
        /// <param name="requestToken">[OUT]取得したリクエストトークン</param>
        /// <returns>取得結果真偽値</returns>
        private Uri GetAuthenticatePageUri(string requestTokenUrl, string authorizeUrl, ref string requestToken)
        {
            const string TokenKey = "oauth_token";

            // リクエストトークン取得
            string content = string.Empty;
            if (GetOAuthToken(new Uri(requestTokenUrl), string.Empty, string.Empty, null, ref content) != HttpStatusCode.OK)
            {
                return null;
            }

            NameValueCollection reqTokenData = ParseQueryString(content);
            if (reqTokenData == null)
            {
                return null;
            }

            requestToken = reqTokenData[TokenKey];

            // Uri生成
            return new UriBuilder(authorizeUrl) { Query = string.Format("{0}={1}", TokenKey, requestToken) }.Uri;
        }

        /// <summary>
        /// OAuth認証のトークン取得共通処理
        /// </summary>
        /// <param name="requestUri">各種トークンの取得先URL</param>
        /// <param name="pinCode">PINフロー時のアクセストークン取得時に設定。それ以外は空文字列</param>
        /// <param name="requestToken">PINフロー時のリクエストトークン取得時に設定。それ以外は空文字列</param>
        /// <param name="parameter">追加パラメータ。xAuthで使用</param>
        /// <returns>取得結果のデータ。正しく取得出来なかった場合はNothing</returns>
        private HttpStatusCode GetOAuthToken(Uri requestUri, string pinCode, string requestToken, Dictionary<string, string> parameter, ref string content)
        {
            HttpWebRequest webReq = null;

            // HTTPリクエスト生成。PINコードもパラメータも未指定の場合はGETメソッドで通信。それ以外はPOST
            if (string.IsNullOrEmpty(pinCode) && parameter == null)
            {
                webReq = CreateRequest(GetMethod, requestUri, null, false);
            }
            else
            {
                // ボディに追加パラメータ書き込み
                webReq = CreateRequest(PostMethod, requestUri, parameter, false);
            }

            // OAuth関連パラメータ準備。追加パラメータがあれば追加
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (parameter != null)
            {
                foreach (KeyValuePair<string, string> kvp in parameter)
                {
                    query.Add(kvp.Key, kvp.Value);
                }
            }

            // PINコードが指定されていればパラメータに追加
            if (!string.IsNullOrEmpty(pinCode))
            {
                query.Add("oauth_verifier", pinCode);
            }

            // OAuth関連情報をHTTPリクエストに追加
            AppendOAuthInfo(webReq, query, requestToken, string.Empty);

            // HTTP応答取得
            Dictionary<string, string> header = new Dictionary<string, string> { { "Date", string.Empty } };
            HttpStatusCode responseCode = GetResponse(webReq, ref content, header, false);
            if (responseCode == HttpStatusCode.OK)
            {
                return responseCode;
            }

            if (!string.IsNullOrEmpty(header["Date"]))
            {
                content += Environment.NewLine;
                content += "Check the Date & Time of this computer." + Environment.NewLine;
                content += string.Format("Server:{0}  PC:{1}", Convert.ToDateTime(header["Date"]), DateTime.Now);
            }

            return responseCode;
        }
    }
}