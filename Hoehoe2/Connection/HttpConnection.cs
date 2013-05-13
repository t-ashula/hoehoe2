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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Hoehoe
{
    /// <summary>
    /// HttpWebRequest,HttpWebResponseを使用した基本的な通信機能を提供する
    /// </summary>
    /// <remarks>
    /// プロキシ情報などを設定するため、使用前に静的メソッドInitializeConnectionを呼び出すこと。
    /// 通信方式によって必要になるHTTPヘッダの付加などは、派生クラスで行う。
    /// </remarks>
    public class HttpConnection
    {
        internal const string PostMethod = "POST";
        internal const string GetMethod = "GET";
        internal const string HeadMethod = "HEAD";

        /// <summary>
        /// プロキシ
        /// </summary>
        private static WebProxy _proxy;

        /// <summary>
        /// ユーザーが選択したプロキシの方式
        /// </summary>
        private static ProxyType _proxyKind = ProxyType.IE;

        /// <summary>
        /// クッキー保存用コンテナ
        /// </summary>
        private static readonly CookieContainer CookieContainer = new CookieContainer();

        /// <summary>
        /// 初期化済みフラグ
        /// </summary>
        private static bool _isInitialize;

        /// <summary>
        /// 通信タイムアウト時間（ms）
        /// </summary>
        private static int _defTimeout = 20 * 1000;

        /// <summary>
        /// 通信タイムアウト時間（ms）
        /// </summary>
        private int _curTimeout;

        /// <summary>
        /// The proxy type.
        /// </summary>
        public enum ProxyType
        {
            None,
            IE,
            Specified
        }

        /// <summary>
        /// 通信タイムアウト時間（ms）。10～120秒の範囲で指定。範囲外は20秒とする
        /// </summary>
        protected static int DefaultTimeout
        {
            get { return _defTimeout; }

            set
            {
                const int TimeoutMinValue = 10 * 1000;
                const int TimeoutMaxValue = 120 * 1000;
                const int TimeoutDefaultValue = 20 * 1000;
                if (value < TimeoutMinValue || value > TimeoutMaxValue)
                {
                    // 範囲外ならデフォルト値設定
                    _defTimeout = TimeoutDefaultValue;
                }
                else
                {
                    _defTimeout = value;
                }
            }
        }

        /// <summary>
        /// 通信タイムアウト時間（ms）。10～120秒の範囲で指定。範囲外は20秒とする
        /// </summary>
        protected int InstanceTimeout
        {
            get { return _curTimeout; }

            set
            {
                const int TimeoutMinValue = 10 * 1000;
                const int TimeoutMaxValue = 120 * 1000;
                if (value < TimeoutMinValue || value > TimeoutMaxValue)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Set {0}-{1}: Value={2}", TimeoutMinValue, TimeoutMaxValue, value));
                }

                _curTimeout = value;
            }
        }

        /// <summary>
        /// 通信クラスの初期化処理。タイムアウト値とプロキシを設定する
        /// </summary>
        /// <remarks>
        /// 通信開始前に最低一度呼び出すこと
        /// </remarks>
        /// <param name="timeout">タイムアウト値（秒）</param>
        /// <param name="proxyType">なし・指定・IEデフォルト</param>
        /// <param name="proxyAddress">プロキシのホスト名orIPアドレス</param>
        /// <param name="proxyPort">プロキシのポート番号</param>
        /// <param name="proxyUser">プロキシ認証が必要な場合のユーザ名。不要なら空文字</param>
        /// <param name="proxyPassword">プロキシ認証が必要な場合のパスワード。不要なら空文字</param>
        public static void InitializeConnection(int timeout, ProxyType proxyType, string proxyAddress, int proxyPort, string proxyUser, string proxyPassword)
        {
            _isInitialize = true;
            ServicePointManager.Expect100Continue = false;
            DefaultTimeout = timeout * 1000; // s -> ms
            switch (proxyType)
            {
                case ProxyType.None:
                    _proxy = null;
                    break;

                case ProxyType.Specified:
                    _proxy = new WebProxy(string.Format("http://{0}:{1}", proxyAddress, proxyPort));
                    if (!string.IsNullOrEmpty(proxyUser) || !string.IsNullOrEmpty(proxyPassword))
                    {
                        _proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword);
                    }

                    break;

                case ProxyType.IE:

                    // IE設定（システム設定）はデフォルト値なので処理しない
                    break;
            }

            _proxyKind = proxyType;
            Win32Api.SetProxy(proxyType, proxyAddress, proxyPort, proxyUser, proxyPassword);
        }

        /// <summary>
        /// HttpWebRequestオブジェクトを取得する。パラメータはGET/HEAD/DELETEではクエリに、POST/PUTではエンティティボディに変換される。
        /// </summary>
        /// <remarks>
        /// 追加で必要となるHTTPヘッダや通信オプションは呼び出し元で付加すること
        /// （Timeout,AutomaticDecompression,AllowAutoRedirect,UserAgent,ContentType,Accept,HttpRequestHeader.Authorization,カスタムヘッダ）
        /// POST/PUTでクエリが必要な場合は、requestUriに含めること。
        /// </remarks>
        /// <param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        /// <param name="requestUri">通信先URI</param>
        /// <param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        /// <param name="withCookie">通信にcookieを使用するか</param>
        /// <returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
        protected HttpWebRequest CreateRequest(string method, Uri requestUri, Dictionary<string, string> param, bool withCookie)
        {
            if (!_isInitialize)
            {
                throw new Exception("Sequence error.(not initialized)");
            }

            // GETメソッドの場合はクエリとurlを結合
            var ub = new UriBuilder(requestUri.AbsoluteUri);
            if (param != null && (method == "GET" || method == "DELETE" || method == "HEAD"))
            {
                ub.Query = CreateQueryString(param);
            }

            var webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);
            webReq.ReadWriteTimeout = 90 * 1000;            // Streamの読み込みは90秒でタイムアウト（デフォルト5分）

            // プロキシ設定
            if (_proxyKind != ProxyType.IE)
            {
                webReq.Proxy = _proxy;
            }

            webReq.Method = method;
            if (method == "POST" || method == "PUT")
            {
                // POST/PUTメソッドの場合は、ボディデータとしてクエリ構成して書き込み
                webReq.ContentType = "application/x-www-form-urlencoded";
                using (var writer = new StreamWriter(webReq.GetRequestStream()))
                {
                    writer.Write(CreateQueryString(param));
                }
            }

            // cookie設定
            if (withCookie)
            {
                webReq.CookieContainer = CookieContainer;
            }

            // タイムアウト設定
            webReq.Timeout = InstanceTimeout > 0 ? InstanceTimeout : DefaultTimeout;

            return webReq;
        }

        /// <summary>
        /// HttpWebRequestオブジェクトを取得する。multipartでのバイナリアップロード用。
        /// </summary>
        /// <remarks>
        /// methodにはPOST/PUTのみ指定可能
        /// </remarks>
        /// <param name="method">HTTP通信メソッド（POST/PUT）</param>
        /// <param name="requestUri">通信先URI</param>
        /// <param name="param">form-dataで指定する名前と文字列のディクショナリ</param>
        /// <param name="binaryFileInfo">form-dataで指定する名前とバイナリファイル情報のリスト</param>
        /// <param name="withCookie">通信にcookieを使用するか</param>
        /// <returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
        protected HttpWebRequest CreateRequest(string method, Uri requestUri, Dictionary<string, string> param, List<KeyValuePair<string, FileInfo>> binaryFileInfo, bool withCookie)
        {
            if (!_isInitialize)
            {
                throw new Exception("Sequence error.(not initialized)");
            }

            // methodはPOST,PUTのみ許可
            if (!(method == "POST" || method == "PUT"))
            {
                throw new ArgumentException("Method must be POST or PUT");
            }

            if ((param == null || param.Count == 0) && (binaryFileInfo == null || binaryFileInfo.Count == 0))
            {
                throw new ArgumentException("Data is empty");
            }

            var ub = new UriBuilder(requestUri.AbsoluteUri);
            var webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);

            // プロキシ設定
            if (_proxyKind != ProxyType.IE)
            {
                webReq.Proxy = _proxy;
            }

            webReq.Method = method;
            var boundary = string.Format("{0}", Environment.TickCount);
            webReq.ContentType = "multipart/form-data; boundary=" + boundary;
            using (var reqStream = webReq.GetRequestStream())
            {
                // POST送信する文字データを作成
                if (param != null)
                {
                    var postData = string.Join(
                        string.Empty,
                        param.Select(kvp => string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, kvp.Key, kvp.Value)));
                    var postBytes = Encoding.UTF8.GetBytes(postData);
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }

                // POST送信するバイナリデータを作成
                if (binaryFileInfo != null)
                {
                    var crlfByte = Encoding.UTF8.GetBytes("\r\n");
                    foreach (var kvp in binaryFileInfo)
                    {
                        // コンテンツタイプの指定
                        var mime = GetMimeType(kvp.Value.Extension.ToLower());

                        var postData = "--" + boundary + "\r\n"
                                       + "Content-Disposition: form-data; name=\"" + kvp.Key + "\"; filename=\"" + kvp.Value.Name + "\"" + "\r\n"
                                       + "Content-Type: " + mime + "\r\n";
                        if (mime == "application/octet-stream")
                        {
                            postData += "Content-Transfer-Encoding: binary" + "\r\n";
                        }

                        postData += "\r\n";
                        var postBytes = Encoding.UTF8.GetBytes(postData);
                        reqStream.Write(postBytes, 0, postBytes.Length);

                        // ファイルを読み出してHTTPのストリームに書き込み
                        using (var fs = new FileStream(kvp.Value.FullName, FileMode.Open, FileAccess.Read))
                        {
                            var readBytes = new byte[4097];
                            while (true)
                            {
                                int readSize = fs.Read(readBytes, 0, readBytes.Length);
                                if (readSize == 0)
                                {
                                    break;
                                }

                                reqStream.Write(readBytes, 0, readSize);
                            }
                        }

                        reqStream.Write(crlfByte, 0, crlfByte.Length);
                    }
                }

                // 終端
                var endBytes = Encoding.UTF8.GetBytes("--" + boundary + "--" + "\r\n");
                reqStream.Write(endBytes, 0, endBytes.Length);
                reqStream.Close();
            }

            // cookie設定
            if (withCookie)
            {
                webReq.CookieContainer = CookieContainer;
            }

            // タイムアウト設定
            webReq.Timeout = InstanceTimeout > 0 ? InstanceTimeout : DefaultTimeout;

            return webReq;
        }

        private static string GetMimeType(string extension)
        {
            string mime;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".jpe":
                    mime = "image/jpeg";
                    break;

                case ".gif":
                    mime = "image/gif";
                    break;

                case ".png":
                    mime = "image/png";
                    break;

                case ".tiff":
                case ".tif":
                    mime = "image/tiff";
                    break;

                case ".bmp":
                    mime = "image/x-bmp";
                    break;

                case ".avi":
                    mime = "video/avi";
                    break;

                case ".wmv":
                    mime = "video/x-ms-wmv";
                    break;

                case ".flv":
                    mime = "video/x-flv";
                    break;

                case ".m4v":
                    mime = "video/x-m4v";
                    break;

                case ".mov":
                    mime = "video/quicktime";
                    break;

                case ".mp4":
                    mime = "video/3gpp";
                    break;

                case ".rm":
                    mime = "application/vnd.rn-realmedia";
                    break;

                case ".mpeg":
                case ".mpg":
                    mime = "video/mpeg";
                    break;

                case ".3gp":
                    mime = "movie/3gp";
                    break;

                case ".3g2":
                    mime = "video/3gpp2";
                    break;

                default:
                    mime = "application/octet-stream";
                    break;
            }

            return mime;
        }

        /// <summary>
        /// HTTPの応答を処理し、引数で指定されたストリームに書き込み
        /// </summary>
        /// <remarks>
        /// リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        /// WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        /// gzipファイルのダウンロードを想定しているため、他形式の場合は伸張時に問題が発生する可能性があります。
        /// </remarks>
        /// <param name="webRequest">HTTP通信リクエストオブジェクト</param>
        /// <param name="contentStream">[OUT]HTTP応答のボディストリームのコピー先</param>
        /// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        /// <param name="withCookie">通信にcookieを使用する</param>
        /// <returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, Stream contentStream, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (var webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    var statusCode = webRes.StatusCode;

                    // cookie保持
                    if (withCookie)
                    {
                        SaveCookie(webRes.Cookies);
                    }

                    // リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);

                    // 応答のストリームをコピーして戻す
                    if (webRes.ContentLength > 0)
                    {
                        // gzipなら応答ストリームの内容は伸張済み。それ以外なら伸張する。
                        if (webRes.ContentEncoding == "gzip" || webRes.ContentEncoding == "deflate")
                        {
                            using (var stream = webRes.GetResponseStream())
                            {
                                if (stream != null)
                                {
                                    CopyStream(stream, contentStream);
                                }
                            }
                        }
                        else
                        {
                            // TODO: need using ?
                            using (var stream = new GZipStream(webRes.GetResponseStream(), CompressionMode.Decompress))
                            {
                                // if (stream != null)
                                {
                                    CopyStream(stream, contentStream);
                                }
                            }
                        }
                    }

                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    return res.StatusCode;
                }

                throw;
            }
        }

        /// <summary>
        /// HTTPの応答を処理し、応答ボディデータをテキストとして返却する
        /// </summary>
        /// <remarks>
        /// リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        /// WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        /// テキストの文字コードはUTF-8を前提として、エンコードはしていません
        /// </remarks>
        /// <param name="webRequest">HTTP通信リクエストオブジェクト</param>
        /// <param name="contentText">[OUT]HTTP応答のボディデータ</param>
        /// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        /// <param name="withCookie">通信にcookieを使用する</param>
        /// <returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, ref string contentText, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (var webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    var statusCode = webRes.StatusCode;

                    // cookie保持
                    if (withCookie)
                    {
                        SaveCookie(webRes.Cookies);
                    }

                    // リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);

                    // 応答のストリームをテキストに書き出し
                    if (contentText == null)
                    {
                        throw new ArgumentNullException("contentText");
                    }

                    using (var sr = new StreamReader(webRes.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }

                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }

                    return res.StatusCode;
                }

                throw;
            }
        }

        /// <summary>
        /// HTTPの応答を処理します。応答ボディデータが不要な用途向け。
        /// </summary>
        /// <remarks>
        /// リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        /// WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        /// </remarks>
        /// <param name="webRequest">HTTP通信リクエストオブジェクト</param>
        /// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        /// <param name="withCookie">通信にcookieを使用する</param>
        /// <returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (var webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;

                    // cookie保持
                    if (withCookie)
                    {
                        SaveCookie(webRes.Cookies);
                    }

                    // リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    return res.StatusCode;
                }

                throw;
            }
        }

        /// <summary>
        /// HTTPの応答を処理し、応答ボディデータをBitmapとして返却します
        /// </summary>
        /// <remarks>
        /// リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        /// WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        /// </remarks>
        /// <param name="webRequest">HTTP通信リクエストオブジェクト</param>
        /// <param name="contentBitmap">[OUT]HTTP応答のボディデータを書き込むBitmap</param>
        /// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        /// <param name="withCookie">通信にcookieを使用する</param>
        /// <returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, ref Bitmap contentBitmap, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (var webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;

                    // Cookie保持
                    if (withCookie)
                    {
                        SaveCookie(webRes.Cookies);
                    }

                    // リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);

                    // 応答のストリームをBitmapにして戻す
                    contentBitmap = new Bitmap(webRes.GetResponseStream());
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    return res.StatusCode;
                }

                throw;
            }
        }

        /// <summary>
        /// クエリコレクションをkey=value形式の文字列に構成して戻す
        /// </summary>
        /// <param name="param">クエリ、またはポストデータとなるkey-valueコレクション</param>
        protected string CreateQueryString(IDictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
            {
                return string.Empty;
            }

            var query = new StringBuilder();
            foreach (string key in param.Keys)
            {
                query.AppendFormat("{0}={1}&", UrlEncode(key), UrlEncode(param[key]));
            }

            return query.ToString(0, query.Length - 1);
        }

        /// <summary>
        /// クエリ形式（key1=value1&amp;key2=value2&amp;...）の文字列をkey-valueコレクションに詰め直し
        /// </summary>
        /// <param name="queryString">クエリ文字列</param>
        /// <returns>key-valueのコレクション</returns>
        protected NameValueCollection ParseQueryString(string queryString)
        {
            var query = new NameValueCollection();
            string[] parts = queryString.Split('&');
            foreach (string part in parts)
            {
                int index = part.IndexOf('=');
                if (index == -1)
                {
                    query.Add(Uri.UnescapeDataString(part), string.Empty);
                }
                else
                {
                    query.Add(Uri.UnescapeDataString(part.Substring(0, index)), Uri.UnescapeDataString(part.Substring(index + 1)));
                }
            }

            return query;
        }

        /// <summary>
        /// 2バイト文字も考慮したUrlエンコード
        /// </summary>
        /// <param name="stringToEncode">エンコードする文字列</param>
        /// <returns>エンコード結果文字列</returns>
        protected string UrlEncode(string stringToEncode)
        {
            const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var sb = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(stringToEncode);
            foreach (byte b in bytes)
            {
                if (UnreservedChars.IndexOf((char)b) != -1)
                {
                    sb.Append((char)b);
                }
                else
                {
                    sb.AppendFormat("%{0:X2}", b);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// クッキーを保存。ホスト名なしのドメインの場合、ドメイン名から先頭のドットを除去して追加しないと再利用されないため
        /// </summary>
        private void SaveCookie(CookieCollection cookieCollection)
        {
            foreach (Cookie ck in cookieCollection)
            {
                if (ck.Domain.StartsWith("."))
                {
                    ck.Domain = ck.Domain.Substring(1, ck.Domain.Length - 1);
                    CookieContainer.Add(ck);
                }
            }
        }

        /// <summary>
        /// in/outのストリームインスタンスを受け取り、コピーして返却
        /// </summary>
        /// <param name="inStream">コピー元ストリームインスタンス。読み取り可であること</param>
        /// <param name="outStream">コピー先ストリームインスタンス。書き込み可であること</param>
        private void CopyStream(Stream inStream, Stream outStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }

            if (!inStream.CanRead)
            {
                throw new ArgumentException("Input stream can not read.");
            }

            if (!outStream.CanWrite)
            {
                throw new ArgumentException("Output stream can not write.");
            }

            if (inStream.CanSeek && inStream.Length == 0)
            {
                throw new ArgumentException("Input stream do not have data.");
            }

            do
            {
                var buffer = new byte[1025];
                int i = buffer.Length;
                i = inStream.Read(buffer, 0, i);
                if (i == 0)
                {
                    break;
                }

                outStream.Write(buffer, 0, i);
            }
            while (true);
        }

        /// <summary>
        /// headerInfoのキー情報で指定されたHTTPヘッダ情報を取得・格納する。redirect応答時はLocationヘッダの内容を追記する
        /// </summary>
        /// <param name="webResponse">HTTP応答</param>
        /// <param name="headerInfo">[IN/OUT]キーにヘッダ名を指定したデータ空のコレクション。取得した値をデータにセットして戻す</param>
        private void GetHeaderInfo(HttpWebResponse webResponse, Dictionary<string, string> headerInfo)
        {
            if (headerInfo == null)
            {
                return;
            }

            if (headerInfo.Count > 0)
            {
                var keys = new string[headerInfo.Count];
                headerInfo.Keys.CopyTo(keys, 0);
                foreach (var key in keys)
                {
                    if (Array.IndexOf(webResponse.Headers.AllKeys, key) > -1)
                    {
                        headerInfo[key] = webResponse.Headers[key];
                    }
                    else
                    {
                        headerInfo[key] = string.Empty;
                    }
                }
            }

            var statusCode = webResponse.StatusCode;
            if (statusCode == HttpStatusCode.MovedPermanently
                || statusCode == HttpStatusCode.Found
                || statusCode == HttpStatusCode.SeeOther
                || statusCode == HttpStatusCode.TemporaryRedirect)
            {
                if (headerInfo.ContainsKey("Location"))
                {
                    headerInfo["Location"] = webResponse.Headers["Location"];
                }
                else
                {
                    headerInfo.Add("Location", webResponse.Headers["Location"]);
                }
            }
        }
    }
}