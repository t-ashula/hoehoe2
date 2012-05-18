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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Hoehoe
{
    public class Twitter : IDisposable
    {
        //Hashtag用正規表現
        private const string LATIN_ACCENTS = "\\xc0-\\xd6\\xd8-\\xf6\\xf8-\\xff";
        private const string NON_LATIN_HASHTAG_CHARS = "\\u0400-\\u04ff\\u0500-\\u0527\\u1100-\\u11ff\\u3130-\\u3185\\uA960-\\uA97F\\uAC00-\\uD7AF\\uD7B0-\\uD7FF";
        //Private Const CJ_HASHTAG_CHARACTERS As String = "\u30A1-\u30FA\uFF66-\uFF9F\uFF10-\uFF19\uFF21-\uFF3A\uFF41-\uFF5A\u3041-\u3096\u3400-\u4DBF\u4E00-\u9FFF\u20000-\u2A6DF\u2A700-\u2B73F\u2B740-\u2B81F\u2F800-\u2FA1F"
        private const string CJ_HASHTAG_CHARACTERS = "\\u30A1-\\u30FA\\u30FC\\u3005\\uFF66-\\uFF9F\\uFF10-\\uFF19\\uFF21-\\uFF3A\\uFF41-\\uFF5A\\u3041-\\u309A\\u3400-\\u4DBF\\p{IsCJKUnifiedIdeographs}";
        private const string HASHTAG_BOUNDARY = "^|$|\\s|「|」|。|\\.|!";
        private const string HASHTAG_ALPHA = "[a-z_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        private const string HASHTAG_ALPHANUMERIC = "[a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        private const string HASHTAG_TERMINATOR = "[^a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        public const string HASHTAG = "(" + HASHTAG_BOUNDARY + ")(#|＃)(" + HASHTAG_ALPHANUMERIC + "*" + HASHTAG_ALPHA + HASHTAG_ALPHANUMERIC + "*)(?=" + HASHTAG_TERMINATOR + "|" + HASHTAG_BOUNDARY + ")";
        //URL正規表現
        private const string URL_VALID_DOMAIN = "(?<domain>(?:[^\\p{P}\\s][\\.\\-_](?=[^\\p{P}\\s])|[^\\p{P}\\s]){1,}\\.[a-z]{2,}(?::[0-9]+)?)";
        private const string URL_VALID_GENERAL_PATH_CHARS = "[a-z0-9!*';:=+$/%#\\[\\]\\-_&,~]";
        private const string URL_BALANCE_PARENS = "(?:\\(" + URL_VALID_GENERAL_PATH_CHARS + "+\\))";
        private const string URL_VALID_URL_PATH_ENDING_CHARS = "(?:[a-z0-9=_#/\\-\\+]+|" + URL_BALANCE_PARENS + ")";
        private const string PTH = "(?:" + URL_BALANCE_PARENS + "|@" + URL_VALID_GENERAL_PATH_CHARS + "+/" + "|[.,]?" + URL_VALID_GENERAL_PATH_CHARS + "+" + ")";
        private const string PTH2 = "(/(?:" + PTH + "+" + URL_VALID_URL_PATH_ENDING_CHARS + "|" + PTH + "+" + URL_VALID_URL_PATH_ENDING_CHARS + "?|" + URL_VALID_URL_PATH_ENDING_CHARS + ")?)?";
        private const string QRY = "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\[\\]\\-_.,~]*[a-z0-9_&=#])?";

        public const string RgUrl = "(?<before>(?:[^\\\"':!=#]|^|\\:/))" + "(?<url>(?<protocol>https?://)" + URL_VALID_DOMAIN + PTH2 + QRY + ")";
        public delegate void GetIconImageDelegate(PostClass post);

        private readonly object _lockObj = new object();
        private List<long> _followerIds = new List<long>();
        private bool _getFollowerResult;
        private List<long> noRTIds = new List<long>();
        private bool _getNoRetweetResult;
        private int _followersCount;
        private int _friendsCount;
        private int _statusesCount;
        private string _location = "";
        private string _bio = "";
        private string _protocol = "https://";

        //プロパティからアクセスされる共通情報
        private string _uname;

        private int _iconSz;
        private bool _getIcon;
        private IDictionary<string, Image> _dIcon;
        private bool _tinyUrlResolve;
        private bool _restrictFavCheck;
        //private string _hubServer;
        private bool _readOwnPost;
        private List<string> _hashList = new List<string>();

        //共通で使用する状態
        //private int _remainCountApi = -1;

        private Outputz _outputz = new Outputz();

        //max_idで古い発言を取得するために保持（lists分は個別タブで管理）
        private long _minHomeTimeline = long.MaxValue;

        private long _minMentions = long.MaxValue;
        private long _minDirectmessage = long.MaxValue;
        private long _minDirectmessageSent = long.MaxValue;
        private HttpTwitter _twCon = new HttpTwitter();

        public event UserIdChangedEventHandler UserIdChanged;

        public delegate void UserIdChangedEventHandler();

        public string Authenticate(string username, string password)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = _twCon.AuthUserAndPass(username, password, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    _uname = username.ToLower();
                    if (AppendSettingDialog.Instance.UserstreamStartup)
                    {
                        this.ReconnectUserStream();
                    }
                    return "";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        string errMsg = GetErrorMessageJson(content);
                        if (String.IsNullOrEmpty(errMsg))
                        {
                            return Hoehoe.Properties.Resources.Unauthorized + Environment.NewLine + content;
                        }
                        else
                        {
                            return "Auth error:" + errMsg;
                        }
                    }
                case HttpStatusCode.Forbidden:
                    {
                        string errMsg = GetErrorMessageJson(content);
                        if (String.IsNullOrEmpty(errMsg))
                        {
                            return "Err:Forbidden";
                        }
                        else
                        {
                            return "Err:" + errMsg;
                        }
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string StartAuthentication(ref string pinPageUrl)
        {
            //OAuth PIN Flow
            bool res = false;
            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = _twCon.AuthGetRequestToken(ref pinPageUrl);
            }
            catch (Exception)
            {
                return "Err:" + "Failed to access auth server.";
            }

            return "";
        }

        public string Authenticate(string pinCode)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";

            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = _twCon.AuthGetAccessToken(pinCode);
            }
            catch (Exception)
            {
                return "Err:" + "Failed to access auth acc server.";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    _uname = Username.ToLower();
                    if (AppendSettingDialog.Instance.UserstreamStartup)
                    {
                        this.ReconnectUserStream();
                    }
                    return "";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        string errMsg = GetErrorMessageJson(content);
                        if (String.IsNullOrEmpty(errMsg))
                        {
                            return "Check the PIN or retry." + Environment.NewLine + content;
                        }
                        else
                        {
                            return "Auth error:" + errMsg;
                        }
                    }
                case HttpStatusCode.Forbidden:
                    {
                        string errMsg = GetErrorMessageJson(content);
                        if (String.IsNullOrEmpty(errMsg))
                        {
                            return "Err:Forbidden";
                        }
                        else
                        {
                            return "Err:" + errMsg;
                        }
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public void ClearAuthInfo()
        {
            Twitter.AccountState = MyCommon.AccountState.Invalid;
            MyCommon.TwitterApiInfo.Initialize();
            _twCon.ClearAuthInfo();
        }

        public void VerifyCredentials()
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.VerifyCredentials(ref content);
            }
            catch (Exception)
            {
                return;
            }

            if (res == HttpStatusCode.OK)
            {
                Twitter.AccountState = MyCommon.AccountState.Valid;
                TwitterDataModel.User user = null;
                try
                {
                    user = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                }
                catch (SerializationException)
                {
                    return;
                }
                _twCon.AuthenticatedUserId = user.Id;
            }
        }

        private string GetErrorMessageJson(string content)
        {
            try
            {
                if (!String.IsNullOrEmpty(content))
                {
                    using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(content), XmlDictionaryReaderQuotas.Max))
                    {
                        XElement xElm = XElement.Load(jsonReader);
                        return xElm.Element("error") != null ? xElm.Element("error").Value : "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void Initialize(string token, string tokenSecret, string username, long userId)
        {
            //OAuth認証
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(tokenSecret) || String.IsNullOrEmpty(username))
            {
                Twitter.AccountState = MyCommon.AccountState.Invalid;
            }
            MyCommon.TwitterApiInfo.Initialize();
            _twCon.Initialize(token, tokenSecret, username, userId);
            _uname = username.ToLower();
            if (AppendSettingDialog.Instance.UserstreamStartup)
            {
                this.ReconnectUserStream();
            }
        }

        public string PreProcessUrl(string orgData)
        {
            int posl1 = 0;
            int posl2 = 0;
            string href = "<a href=\"";

            while (true)
            {
                if (orgData.IndexOf(href, posl2, StringComparison.Ordinal) > -1)
                {
                    string urlStr = "";
                    // IDN展開
                    posl1 = orgData.IndexOf(href, posl2, StringComparison.Ordinal);
                    posl1 += href.Length;
                    posl2 = orgData.IndexOf("\"", posl1, StringComparison.Ordinal);
                    urlStr = orgData.Substring(posl1, posl2 - posl1);

                    if (!urlStr.StartsWith("http://") && !urlStr.StartsWith("https://") && !urlStr.StartsWith("ftp://"))
                    {
                        continue;
                    }

                    string replacedUrl = MyCommon.IDNDecode(urlStr);
                    if (replacedUrl == null)
                    {
                        continue;
                    }
                    if (replacedUrl == urlStr)
                    {
                        continue;
                    }
                    orgData = orgData.Replace("<a href=\"" + urlStr, "<a href=\"" + replacedUrl);
                    posl2 = 0;
                }
                else
                {
                    break;
                }
            }
            return orgData;
        }

        private string GetPlainText(string orgData)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(orgData, "(?<tagStart><a [^>]+>)(?<text>[^<]+)(?<tagEnd></a>)", "${text}"));
        }

        // htmlの簡易サニタイズ(詳細表示に不要なタグの除去)
        private string SanitizeHtml(string orgdata)
        {
            string retdata = orgdata;
            retdata = Regex.Replace(retdata, "<(script|object|applet|image|frameset|fieldset|legend|style).*" + "</(script|object|applet|image|frameset|fieldset|legend|style)>", "", RegexOptions.IgnoreCase);
            retdata = Regex.Replace(retdata, "<(frame|link|iframe|img)>", "", RegexOptions.IgnoreCase);
            return retdata;
        }

        private string AdjustHtml(string orgData)
        {
            string retStr = orgData;
            retStr = Regex.Replace(retStr, "<a [^>]*href=\"/", "<a href=\"" + _protocol + "twitter.com/");
            retStr = retStr.Replace("<a href=", "<a target=\"_self\" href=");
            retStr = retStr.Replace("\r\n", "<br>");

            //半角スペースを置換(Thanks @anis774)
            bool ret = false;
            do
            {
                ret = EscapeSpace(ref retStr);
            } while (!ret);

            return SanitizeHtml(retStr);
        }

        private bool EscapeSpace(ref string html)
        {
            //半角スペースを置換(Thanks @anis774)
            bool isTag = false;
            for (int i = 0; i < html.Length; i++)
            {
                if (html[i] == '<')
                {
                    isTag = true;
                }
                if (html[i] == '>')
                {
                    isTag = false;
                }

                if ((!isTag) && (html[i] == ' '))
                {
                    html = html.Remove(i, 1);
                    html = html.Insert(i, "&nbsp;");
                    return false;
                }
            }
            return true;
        }

        private class PostInfo
        {
            public string CreatedAt;
            public string Id;
            public string Text;
            public string UserId;

            public PostInfo(string created, string idStr, string txt, string uid)
            {
                this.CreatedAt = created;
                this.Id = idStr;
                this.Text = txt;
                this.UserId = uid;
            }

            public bool Equals(PostInfo dst)
            {
                return this.CreatedAt == dst.CreatedAt && this.Id == dst.Id && this.Text == dst.Text && this.UserId == dst.UserId;
            }
        }

        PostInfo _prevPostInfo = new PostInfo("", "", "", "");

        private bool IsPostRestricted(TwitterDataModel.Status status)
        {
            PostInfo currentPost = new PostInfo("", "", "", "");
            currentPost.CreatedAt = status.CreatedAt;
            currentPost.Id = status.IdStr;
            if (status.Text == null)
            {
                currentPost.Text = "";
            }
            else
            {
                currentPost.Text = status.Text;
            }
            currentPost.UserId = status.User.IdStr;

            if (currentPost.Equals(_prevPostInfo))
            {
                return true;
            }
            _prevPostInfo.CreatedAt = currentPost.CreatedAt;
            _prevPostInfo.Id = currentPost.Id;
            _prevPostInfo.Text = currentPost.Text;
            _prevPostInfo.UserId = currentPost.UserId;

            return false;
        }

        public string PostStatus(string postStr, long replyToId)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            postStr = postStr.Trim();

            if (Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
            {
                return SendDirectMessage(postStr);
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.UpdateStatus(postStr, replyToId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    TwitterDataModel.Status status = null;
                    try
                    {
                        status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    _followersCount = status.User.FollowersCount;
                    _friendsCount = status.User.FriendsCount;
                    _statusesCount = status.User.StatusesCount;
                    _location = status.User.Location;
                    _bio = status.User.Description;

                    if (IsPostRestricted(status))
                    {
                        return "OK:Delaying?";
                    }
                    return _outputz.Post(postStr.Length) ? "" : "Outputz:Failed";
                case HttpStatusCode.NotFound:
                    return "";
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
                    }
                case HttpStatusCode.Conflict:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.RequestUriTooLong:
                    //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostStatusWithMedia(string postStr, long replyToId, FileInfo mediaFile)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            postStr = postStr.Trim();

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.UpdateStatusWithMedia(postStr, replyToId, mediaFile, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    TwitterDataModel.Status status = null;
                    try
                    {
                        status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    _followersCount = status.User.FollowersCount;
                    _friendsCount = status.User.FriendsCount;
                    _statusesCount = status.User.StatusesCount;
                    _location = status.User.Location;
                    _bio = status.User.Description;

                    if (IsPostRestricted(status))
                    {
                        return "OK:Delaying?";
                    }
                    return _outputz.Post(postStr.Length) ? "" : "Outputz:Failed";
                case HttpStatusCode.NotFound:
                    return "";
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
                    }
                case HttpStatusCode.Conflict:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.RequestUriTooLong:
                    //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string SendDirectMessage(string postStr)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }
            if (MyCommon.TwitterApiInfo.AccessLevel != ApiAccessLevel.None)
            {
                if (!MyCommon.TwitterApiInfo.IsDirectMessagePermission)
                {
                    return "Auth Err:try to re-authorization.";
                }
            }

            postStr = postStr.Trim();
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            Match mc = Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            try
            {
                res = _twCon.SendDirectMessage(mc.Groups["body"].Value, mc.Groups["id"].Value, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    TwitterDataModel.Directmessage status = null;
                    try
                    {
                        status = MyCommon.CreateDataFromJson<TwitterDataModel.Directmessage>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    _followersCount = status.Sender.FollowersCount;
                    _friendsCount = status.Sender.FriendsCount;
                    _statusesCount = status.Sender.StatusesCount;
                    _location = status.Sender.Location;
                    _bio = status.Sender.Description;
                    return _outputz.Post(postStr.Length) ? "" : "Outputz:Failed";
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
                    }
                case HttpStatusCode.Conflict:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.RequestUriTooLong:
                    //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString();
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        string errMsg = GetErrorMessageJson(content);
                        return String.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string RemoveStatus(long id)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = _twCon.DestroyStatus(id);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.NotFound:
                    return "";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRetweet(long id, bool read)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            //データ部分の生成
            long target = id;
            PostClass post = TabInformations.GetInstance().Item(id);
            if (post == null)
            {
                return "Err:Target isn't found.";
            }
            if (TabInformations.GetInstance().Item(id).RetweetedId > 0)
            {
                //再RTの場合は元発言をRT
                target = TabInformations.GetInstance().Item(id).RetweetedId;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.RetweetStatus(target, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            if (res == HttpStatusCode.Unauthorized)
            {
                return Hoehoe.Properties.Resources.Unauthorized + " or blocked user.";
            }
            else if (res != HttpStatusCode.OK)
            {
                return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            Twitter.AccountState = MyCommon.AccountState.Valid;
            TwitterDataModel.Status status = null;
            try
            {
                status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }

            //ReTweetしたものをTLに追加
            post = CreatePostsFromStatusData(status);
            if (post == null)
            {
                return "Invalid Json!";
            }

            //二重取得回避
            lock (_lockObj)
            {
                if (TabInformations.GetInstance().ContainsKey(post.StatusId))
                {
                    return "";
                }
            }
            //Retweet判定
            if (post.RetweetedId == 0)
            {
                return "Invalid Json!";
            }

            //ユーザー情報
            post.IsMe = true;
            post.IsRead = read;
            post.IsOwl = false;
            if (_readOwnPost)
            {
                post.IsRead = true;
            }
            post.IsDm = false;

            TabInformations.GetInstance().AddPost(post);

            return "";
        }

        public string RemoveDirectMessage(long id, PostClass post)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.TwitterApiInfo.AccessLevel != ApiAccessLevel.None)
            {
                if (!MyCommon.TwitterApiInfo.IsDirectMessagePermission)
                {
                    return "Auth Err:try to re-authorization.";
                }
            }

            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = _twCon.DestroyDirectMessage(id);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.NotFound:
                    return "";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostFollowCommand(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.CreateFriendships(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRemoveCommand(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.DestroyFriendships(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostCreateBlock(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.CreateBlock(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostDestroyBlock(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.DestroyBlock(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostReportSpam(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.ReportSpam(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetFriendshipInfo(string screenName, ref bool isFollowing, ref bool isFollowed)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.ShowFriendships(_uname, screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    try
                    {
                        var relation = MyCommon.CreateDataFromJson<TwitterDataModel.Relationship>(content);
                        isFollowing = relation.Relationship_.Source.Following;
                        isFollowed = relation.Relationship_.Source.FollowedBy;
                        return "";
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetUserInfo(string screenName, ref TwitterDataModel.User user)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            user = null;
            try
            {
                res = _twCon.ShowUserInfo(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    try
                    {
                        user = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    return "";
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetStatus_Retweeted_Count(long statusId, ref int retweetedCount)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            retweetedCount = 0;
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    res = _twCon.Statusid_retweeted_by_ids(statusId, 100, i, ref content);
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message;
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        try
                        {
                            Int64[] ids = MyCommon.CreateDataFromJson<long[]>(content);
                            retweetedCount += ids.Length;
                            if (ids.Length < 100)
                            {
                                break;
                            }
                        }
                        catch (SerializationException ex)
                        {
                            retweetedCount = -1;
                            MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                            return "Err:Json Parse Error(DataContractJsonSerializer)";
                        }
                        catch (Exception ex)
                        {
                            retweetedCount = -1;
                            MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                            return "Err:Invalid Json!";
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        retweetedCount = -1;
                        return "Err:API Limits?";
                    case HttpStatusCode.Unauthorized:
                        retweetedCount = -1;
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    default:
                        retweetedCount = -1;
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }
            }
            return "";
        }

        public string PostFavAdd(long id)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.CreateFavorites(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    if (!_restrictFavCheck)
                    {
                        return "";
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            //http://twitter.com/statuses/show/id.xml APIを発行して本文を取得
            content = "";
            try
            {
                res = _twCon.ShowStatuses(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    TwitterDataModel.Status status = null;
                    try
                    {
                        status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    return status.Favorited ? "" : "NG(Restricted?)";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostFavRemove(long id)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.DestroyFavorites(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfile(string name, string url, string location, string description)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.UpdateProfile(name, url, location, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfileImage(string filename)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.UpdateProfileImage(new FileInfo(filename), ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = GetErrorMessageJson(content);
                    return String.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string Username
        {
            get { return _twCon.AuthenticatedUsername; }
        }

        public long UserId
        {
            get { return _twCon.AuthenticatedUserId; }
        }

        public string Password
        {
            get { return _twCon.Password; }
        }

        private static MyCommon.AccountState _accountState = MyCommon.AccountState.Valid;

        public static MyCommon.AccountState AccountState
        {
            get { return _accountState; }
            set { _accountState = value; }
        }

        public void SetGetIcon(bool value)
        {
            _getIcon = value;
        }

        public void SetTinyUrlResolve(bool value)
        {
            _tinyUrlResolve = value;
        }

        public void SetRestrictFavCheck(bool value)
        {
            _restrictFavCheck = value;
        }

        public void SetIconSize(int value)
        {
            _iconSz = value;
        }

        #region "TODO:バージョンアップ"

        public string GetVersionInfo()
        {
            string content = "";
            if (!(new HttpVarious()).GetData("http://tween.sourceforge.jp/version.txt?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), null, ref content, MyCommon.GetUserAgentString()))
            {
                throw new Exception("GetVersionInfo Failed");
            }
            return content;
        }

        public string GetTweenBinary(string strVer)
        {
            try
            {
                //本体
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenNew.exe")))
                {
                    return "Err:Download failed";
                }
                //英語リソース
                if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, "en")))
                {
                    Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, "en"));
                }
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenResEn" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, "en"), "Tween.resourcesNew.dll")))
                {
                    return "Err:Download failed";
                }
                //その他言語圏のリソース。取得失敗しても継続
                //UIの言語圏のリソース
                string curCul = "";
                if (!Thread.CurrentThread.CurrentUICulture.IsNeutralCulture)
                {
                    int idx = Thread.CurrentThread.CurrentUICulture.Name.LastIndexOf('-');
                    if (idx > -1)
                    {
                        curCul = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, idx);
                    }
                    else
                    {
                        curCul = Thread.CurrentThread.CurrentUICulture.Name;
                    }
                }
                else
                {
                    curCul = Thread.CurrentThread.CurrentUICulture.Name;
                }
                if (!String.IsNullOrEmpty(curCul) && curCul != "en" && curCul != "ja")
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, curCul)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, curCul));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, curCul), "Tween.resourcesNew.dll")))
                    {
                        //Return "Err:Download failed"
                    }
                }
                //スレッドの言語圏のリソース
                string curCul2 = null;
                if (!Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
                {
                    int idx = Thread.CurrentThread.CurrentCulture.Name.LastIndexOf('-');
                    if (idx > -1)
                    {
                        curCul2 = Thread.CurrentThread.CurrentCulture.Name.Substring(0, idx);
                    }
                    else
                    {
                        curCul2 = Thread.CurrentThread.CurrentCulture.Name;
                    }
                }
                else
                {
                    curCul2 = Thread.CurrentThread.CurrentCulture.Name;
                }
                if (!String.IsNullOrEmpty(curCul2) && curCul2 != "en" && curCul2 != curCul)
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, curCul2)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, curCul2));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul2 + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, curCul2), "Tween.resourcesNew.dll")))
                    {
                        //Return "Err:Download failed"
                    }
                }

                //アップデータ
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenUp3.gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenUp3.exe")))
                {
                    return "Err:Download failed";
                }
                //シリアライザDLL
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenNew.XmlSerializers.dll")))
                {
                    return "Err:Download failed";
                }
                return "";
            }
            catch (Exception)
            {
                return "Err:Download failed";
            }
        }

        #endregion "バージョンアップ"

        public IDictionary<string, Image> DetailIcon
        {
            get { return _dIcon; }
            set { _dIcon = value; }
        }

        public bool ReadOwnPost
        {
            get { return _readOwnPost; }
            set { _readOwnPost = value; }
        }

        public int FollowersCount
        {
            get { return _followersCount; }
        }

        public int FriendsCount
        {
            get { return _friendsCount; }
        }

        public int StatusesCount
        {
            get { return _statusesCount; }
        }

        public string Location
        {
            get { return _location; }
        }

        public string Bio
        {
            get { return _bio; }
        }

        public void SetUseSsl(bool value)
        {
            HttpTwitter.SetUseSsl(value);
            _protocol = value ? "https://" : "http://";
        }

        public string GetTimelineApi(bool read, MyCommon.WorkerType gType, bool more, bool startup)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            int count = AppendSettingDialog.Instance.CountApi;
            if (gType == MyCommon.WorkerType.Reply)
            {
                count = AppendSettingDialog.Instance.CountApiReply;
            }
            if (AppendSettingDialog.Instance.UseAdditionalCount)
            {
                if (more && AppendSettingDialog.Instance.MoreCountApi != 0)
                {
                    count = AppendSettingDialog.Instance.MoreCountApi;
                }
                else if (startup && AppendSettingDialog.Instance.FirstCountApi != 0 && gType == MyCommon.WorkerType.Timeline)
                {
                    count = AppendSettingDialog.Instance.FirstCountApi;
                }
            }
            try
            {
                if (gType == MyCommon.WorkerType.Timeline)
                {
                    if (more)
                    {
                        res = _twCon.HomeTimeline(count, this._minHomeTimeline, 0, ref content);
                    }
                    else
                    {
                        res = _twCon.HomeTimeline(count, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = _twCon.Mentions(count, this._minMentions, 0, ref content);
                    }
                    else
                    {
                        res = _twCon.Mentions(count, 0, 0, ref content);
                    }
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return gType == MyCommon.WorkerType.Timeline ? 
                CreatePostsFromJson(content, gType, null, read, count, ref this._minHomeTimeline) : 
                CreatePostsFromJson(content, gType, null, read, count, ref this._minMentions);            
        }

        public string GetUserTimelineApi(bool read, int count, string userName, TabClass tab, bool more)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            if (count == 0)
            {
                count = 20;
            }
            try
            {
                if (String.IsNullOrEmpty(userName))
                {
                    string target = tab.User;
                    if (String.IsNullOrEmpty(target))
                    {
                        return "";
                    }
                    userName = target;
                    res = _twCon.UserTimeline(0, target, count, 0, 0, ref content);
                }
                else
                {
                    if (more)
                    {
                        res = _twCon.UserTimeline(0, userName, count, tab.OldestId, 0, ref content);
                    }
                    else
                    {
                        res = _twCon.UserTimeline(0, userName, count, 0, 0, ref content);
                    }
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    return "Err:@" + userName + "'s Tweets are protected.";
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            List<TwitterDataModel.Status> items = null;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (TwitterDataModel.Status status in items)
            {
                PostClass item = CreatePostsFromStatusData(status);
                if (item == null)
                {
                    continue;
                }
                if (item.StatusId < tab.OldestId)
                {
                    tab.OldestId = item.StatusId;
                }
                item.IsRead = read;
                if (item.IsMe && !read && _readOwnPost)
                {
                    item.IsRead = true;
                }
                if (tab != null)
                {
                    item.RelTabName = tab.TabName;
                }
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(item);
            }

            return "";
        }

        public string GetStatusApi(bool read, Int64 id, ref PostClass post)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.ShowStatuses(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Forbidden:
                    return "Err:Protected user's tweet";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            TwitterDataModel.Status status = null;
            try
            {
                status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            PostClass item = CreatePostsFromStatusData(status);
            if (item == null)
            {
                return "Err:Can't create post";
            }
            item.IsRead = read;
            if (item.IsMe && !read && _readOwnPost)
            {
                item.IsRead = true;
            }
            post = item;
            return "";
        }

        public string GetStatusApi(bool read, Int64 id, TabClass tab)
        {
            PostClass post = null;
            string r = this.GetStatusApi(read, id, ref post);
            if (String.IsNullOrEmpty(r))
            {
                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }
            return r;
        }

        private PostClass CreatePostsFromStatusData(TwitterDataModel.Status status)
        {
            PostClass post = new PostClass();
            TwitterDataModel.Entities entities = null;
            post.StatusId = status.Id;
            if (status.RetweetedStatus != null)
            {
                TwitterDataModel.RetweetedStatus retweeted = status.RetweetedStatus;
                post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);
                //Id
                post.RetweetedId = retweeted.Id;
                //本文
                post.TextFromApi = retweeted.Text;
                entities = retweeted.Entities;
                //Source取得（htmlの場合は、中身を取り出し）
                post.Source = retweeted.Source;
                //Reply先
                { long t; long.TryParse(retweeted.InReplyToStatusId, out t); post.InReplyToStatusId = t; }
                post.InReplyToUser = retweeted.InReplyToScreenName;
                { long t; long.TryParse(status.InReplyToUserId, out t); post.InReplyToUserId = t; }

                //幻覚fav対策
                TabClass tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.RetweetedId);

                if (retweeted.Geo != null)
                {
                    post.PostGeo = new PostClass.StatusGeo
                    {
                        Lat = retweeted.Geo.Coordinates[0],
                        Lng = retweeted.Geo.Coordinates[1]
                    };
                }

                //以下、ユーザー情報
                TwitterDataModel.User user = retweeted.User;
                if (user.ScreenName == null || status.User.ScreenName == null)
                {
                    return null;
                }
                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrl;
                post.IsProtect = user.Protected;
                //Retweetした人
                post.RetweetedBy = status.User.ScreenName;
                post.RetweetedByUserId = status.User.Id;
                post.IsMe = post.RetweetedBy.ToLower().Equals(_uname);
            }
            else
            {
                post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                //本文
                post.TextFromApi = status.Text;
                entities = status.Entities;
                //Source取得（htmlの場合は、中身を取り出し）
                post.Source = status.Source;
                { long t; long.TryParse(status.InReplyToStatusId, out t); post.InReplyToStatusId = t; }
                post.InReplyToUser = status.InReplyToScreenName;
                { long t; long.TryParse(status.InReplyToUserId, out t); post.InReplyToUserId = t; }

                if (status.Geo != null)
                {
                    post.PostGeo = new PostClass.StatusGeo
                    {
                        Lat = status.Geo.Coordinates[0],
                        Lng = status.Geo.Coordinates[1]
                    };
                }

                //以下、ユーザー情報
                TwitterDataModel.User user = status.User;
                if (user.ScreenName == null)
                {
                    return null;
                }
                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrl;
                post.IsProtect = user.Protected;
                post.IsMe = post.ScreenName.ToLower().Equals(_uname);

                //幻覚fav対策
                TabClass tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.StatusId) && TabInformations.GetInstance().Item(post.StatusId).IsFav;
            }
            //HTMLに整形
            { var t = post.TextFromApi; post.Text = CreateHtmlAnchor(ref t, post.ReplyToList, entities, post.Media); post.TextFromApi = t; }
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
            post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
            //Source整形
            CreateSource(ref post);
            post.IsReply = post.ReplyToList.Contains(_uname);
            post.IsExcludeReply = false;
            if (post.IsMe)
            {
                post.IsOwl = false;
            }
            else
            {
                if (_followerIds.Count > 0)
                {
                    post.IsOwl = !_followerIds.Contains(post.UserId);
                }
            }
            post.IsDm = false;
            return post;
        }

        private string CreatePostsFromJson(string content, MyCommon.WorkerType gType, TabClass tab, bool read, int count, ref long minimumId)
        {
            List<TwitterDataModel.Status> items = null;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (TwitterDataModel.Status status in items)
            {
                PostClass post = null;
                post = CreatePostsFromStatusData(status);
                if (post == null)
                {
                    continue;
                }

                if (minimumId > post.StatusId)
                {
                    minimumId = post.StatusId;
                }
                //二重取得回避
                lock (_lockObj)
                {
                    if (tab == null)
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName))
                        {
                            continue;
                        }
                    }
                }

                //RT禁止ユーザーによるもの
                if (post.RetweetedId > 0 && this.noRTIds.Contains(post.RetweetedByUserId))
                {
                    continue;
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost)
                {
                    post.IsRead = true;
                }

                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }
            return "";
        }

        private string CreatePostsFromPhoenixSearch(string content, MyCommon.WorkerType gType, TabClass tab, bool read, int count, ref long minimumId, ref string nextPageQuery)
        {
            TwitterDataModel.SearchResult items = null;
            try
            {
                items = MyCommon.CreateDataFromJson<TwitterDataModel.SearchResult>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            nextPageQuery = items.NextPage;

            foreach (TwitterDataModel.Status status in items.Statuses)
            {
                PostClass post = CreatePostsFromStatusData(status);
                if (post == null)
                {
                    continue;
                }

                if (minimumId > post.StatusId)
                {
                    minimumId = post.StatusId;
                }
                //二重取得回避
                lock (_lockObj)
                {
                    if (tab == null)
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName))
                        {
                            continue;
                        }
                    }
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost)
                {
                    post.IsRead = true;
                }

                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return String.IsNullOrEmpty(items.ErrMsg) ? "" : "Err:" + items.ErrMsg;
        }

        public string GetListStatus(bool read, TabClass tab, bool more, bool startup)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            int count = 0;
            if (AppendSettingDialog.Instance.UseAdditionalCount)
            {
                count = AppendSettingDialog.Instance.ListCountApi;
                if (count == 0)
                {
                    if (more && AppendSettingDialog.Instance.MoreCountApi != 0)
                    {
                        count = AppendSettingDialog.Instance.MoreCountApi;
                    }
                    else if (startup && AppendSettingDialog.Instance.FirstCountApi != 0)
                    {
                        count = AppendSettingDialog.Instance.FirstCountApi;
                    }
                    else
                    {
                        count = AppendSettingDialog.Instance.CountApi;
                    }
                }
            }
            else
            {
                count = AppendSettingDialog.Instance.CountApi;
            }
            try
            {
                if (more)
                {
                    res = _twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, tab.OldestId, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
                else
                {
                    res = _twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, 0, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            var t = tab.OldestId;
            var ret = CreatePostsFromJson(content, MyCommon.WorkerType.List, tab, read, count, ref t);
            tab.OldestId = t;
            return ret;
        }

        private PostClass CheckReplyToPost(List<PostClass> relPosts)
        {
            PostClass tmpPost = relPosts[0];
            PostClass lastPost = null;
            while (tmpPost != null)
            {
                if (tmpPost.InReplyToStatusId == 0)
                {
                    return null;
                }
                lastPost = tmpPost;
                var replyToPost = from p in relPosts
                                  where p.StatusId == tmpPost.InReplyToStatusId
                                  select p;
                tmpPost = replyToPost.FirstOrDefault();
            }
            return lastPost;
        }

        public string GetRelatedResult(bool read, TabClass tab)
        {
            string rslt = "";
            List<PostClass> relPosts = new List<PostClass>();
            if (tab.RelationTargetPost.TextFromApi.Contains("@") && tab.RelationTargetPost.InReplyToStatusId == 0)
            {
                //検索結果対応
                PostClass p = TabInformations.GetInstance().Item(tab.RelationTargetPost.StatusId);
                if (p != null && p.InReplyToStatusId > 0)
                {
                    tab.RelationTargetPost = p;
                }
                else
                {
                    rslt = this.GetStatusApi(read, tab.RelationTargetPost.StatusId, ref p);
                    if (!String.IsNullOrEmpty(rslt))
                    {
                        return rslt;
                    }
                    tab.RelationTargetPost = p;
                }
            }
            relPosts.Add(tab.RelationTargetPost.Copy());
            PostClass tmpPost = relPosts[0];
            do
            {
                rslt = this.GetRelatedResultsApi(read, tmpPost, tab, relPosts);
                if (!String.IsNullOrEmpty(rslt))
                {
                    break;
                }
                tmpPost = CheckReplyToPost(relPosts);
            } while (tmpPost != null);
            relPosts.ForEach(p => TabInformations.GetInstance().AddPost(p));
            return rslt;
        }

        private string GetRelatedResultsApi(bool read, PostClass post, TabClass tab, List<PostClass> relatedPosts)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                if (post.RetweetedId > 0)
                {
                    res = _twCon.GetRelatedResults(post.RetweetedId, ref content);
                }
                else
                {
                    res = _twCon.GetRelatedResults(post.StatusId, ref content);
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            List<TwitterDataModel.RelatedResult> items = null;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.RelatedResult>>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            PostClass targetItem = post;
            if (targetItem == null)
            {
                return "";
            }
            else
            {
                targetItem = targetItem.Copy();
            }
            targetItem.RelTabName = tab.TabName;
            TabInformations.GetInstance().AddPost(targetItem);
            PostClass replyToItem = null;
            string replyToUserName = targetItem.InReplyToUser;
            if (targetItem.InReplyToStatusId > 0 && TabInformations.GetInstance().Item(targetItem.InReplyToStatusId) != null)
            {
                replyToItem = TabInformations.GetInstance().Item(targetItem.InReplyToStatusId).Copy();
                replyToItem.IsRead = read;
                if (replyToItem.IsMe && !read && _readOwnPost)
                {
                    replyToItem.IsRead = true;
                }
                replyToItem.RelTabName = tab.TabName;
            }

            bool replyAdded = false;
            foreach (TwitterDataModel.RelatedResult relatedData in items)
            {
                foreach (TwitterDataModel.RelatedTweet result in relatedData.Results)
                {
                    PostClass item = CreatePostsFromStatusData(result.Status);
                    if (item == null)
                    {
                        continue;
                    }
                    if (targetItem.InReplyToStatusId == item.StatusId)
                    {
                        replyToItem = null;
                        replyAdded = true;
                    }
                    item.IsRead = read;
                    if (item.IsMe && !read && _readOwnPost)
                    {
                        item.IsRead = true;
                    }
                    if (tab != null)
                    {
                        item.RelTabName = tab.TabName;
                    }
                    //非同期アイコン取得＆StatusDictionaryに追加
                    relatedPosts.Add(item);
                }
            }
            if (replyToItem != null)
            {
                relatedPosts.Add(replyToItem);
            }
            else if (targetItem.InReplyToStatusId > 0 && !replyAdded)
            {
                PostClass p = null;
                string rslt = GetStatusApi(read, targetItem.InReplyToStatusId, ref p);
                if (String.IsNullOrEmpty(rslt))
                {
                    p.IsRead = read;
                    p.RelTabName = tab.TabName;
                    relatedPosts.Add(p);
                }
                return rslt;
            }

            //MRTとかに対応のためツイート内にあるツイートを指すURLを取り込む
            MatchCollection ma = Regex.Matches(tab.RelationTargetPost.Text, "title=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/(?<StatusId>[0-9]+))\"");
            foreach (Match m in ma)
            {
                Int64 statusId;
                if (Int64.TryParse(m.Groups["StatusId"].Value, out statusId))
                {
                    PostClass p = null;
                    PostClass p2 = TabInformations.GetInstance().Item(statusId);
                    if (p2 == null)
                    {
                        GetStatusApi(read, statusId, ref p);
                    }
                    else
                    {
                        p = p2.Copy();
                    }
                    if (p != null)
                    {
                        p.IsRead = read;
                        p.RelTabName = tab.TabName;
                        relatedPosts.Add(p);
                    }
                }
            }
            return "";
        }

        public string GetSearch(bool read, TabClass tab, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            int page = 0;
            long sinceId = 0;
            int count = 100;
            if (AppendSettingDialog.Instance.UseAdditionalCount && AppendSettingDialog.Instance.SearchCountApi != 0)
            {
                count = AppendSettingDialog.Instance.SearchCountApi;
            }
            else
            {
                count = AppendSettingDialog.Instance.CountApi;
            }

            if (more)
            {
                page = tab.GetSearchPage(count);
            }
            else
            {
                sinceId = tab.SinceId;
            }

            try
            {
                res = _twCon.Search(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.BadRequest:
                    return "Invalid query";
                case HttpStatusCode.NotFound:
                    return "Invalid query";
                case HttpStatusCode.PaymentRequired:
                    //API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab))
            {
                return "";
            }
            content = Regex.Replace(content, "[\\x00-\\x1f-[\\x0a\\x0d]]+", " ");
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.LoadXml(content);
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid ATOM!";
            }
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdoc.NameTable);
            nsmgr.AddNamespace("search", "http://www.w3.org/2005/Atom");
            nsmgr.AddNamespace("twitter", "http://api.twitter.com/");
            nsmgr.AddNamespace("georss", "http://www.georss.org/georss");
            foreach (XmlNode xentryNode in xdoc.DocumentElement.SelectNodes("/search:feed/search:entry", nsmgr))
            {
                XmlElement xentry = (XmlElement)xentryNode;
                PostClass post = new PostClass();
                try
                {
                    post.StatusId = long.Parse(xentry["id"].InnerText.Split(':')[2]);
                    if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName))
                    {
                        continue;
                    }
                    post.CreatedAt = DateTime.Parse(xentry["published"].InnerText);
                    //本文
                    post.TextFromApi = xentry["title"].InnerText;
                    //Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xentry["twitter:source"].InnerText;
                    post.InReplyToStatusId = 0;
                    post.InReplyToUser = "";
                    post.InReplyToUserId = 0;
                    post.IsFav = false;

                    // Geoが勝手に付加されるバグがいっこうに修正されないので暫定的にGeo情報を無視する
                    if (xentry["twitter:geo"].HasChildNodes)
                    {
                        string[] pnt = ((XmlElement)xentry.SelectSingleNode("twitter:geo/georss:point", nsmgr)).InnerText.Split(' ');
                        post.PostGeo = new PostClass.StatusGeo
                        {
                            Lat = double.Parse(pnt[0]),
                            Lng = double.Parse(pnt[1])
                        };
                    }

                    //以下、ユーザー情報
                    XmlElement xUentry = (XmlElement)xentry.SelectSingleNode("./search:author", nsmgr);
                    post.UserId = 0;
                    post.ScreenName = xUentry["name"].InnerText.Split(' ')[0].Trim();
                    post.Nickname = xUentry["name"].InnerText.Substring(post.ScreenName.Length).Trim();
                    if (post.Nickname.Length > 2)
                    {
                        post.Nickname = post.Nickname.Substring(1, post.Nickname.Length - 2);
                    }
                    else
                    {
                        post.Nickname = post.ScreenName;
                    }
                    post.ImageUrl = ((XmlElement)xentry.SelectSingleNode("./search:link[@type='image/png']", nsmgr)).GetAttribute("href");
                    post.IsProtect = false;
                    post.IsMe = post.ScreenName.ToLower().Equals(_uname);
                    //HTMLに整形
                    post.Text = CreateHtmlAnchor(HttpUtility.HtmlEncode(post.TextFromApi), post.ReplyToList, post.Media);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    //Source整形
                    CreateSource(ref post);
                    post.IsRead = read;
                    post.IsReply = post.ReplyToList.Contains(_uname);
                    post.IsExcludeReply = false;
                    post.IsOwl = false;
                    if (post.IsMe && !read && _readOwnPost)
                    {
                        post.IsRead = true;
                    }
                    post.IsDm = false;
                    post.RelTabName = tab.TabName;
                    if (!more && post.StatusId > tab.SinceId)
                    {
                        tab.SinceId = post.StatusId;
                    }
                }
                catch (Exception ex)
                {
                    MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                    continue;
                }
                TabInformations.GetInstance().AddPost(post);
            }
            return "";
        }

        public string GetPhoenixSearch(bool read, TabClass tab, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            int page = 0;
            long sinceId = 0;
            int count = 100;
            string querystr = "";
            if (AppendSettingDialog.Instance.UseAdditionalCount && AppendSettingDialog.Instance.SearchCountApi != 0)
            {
                count = AppendSettingDialog.Instance.SearchCountApi;
            }
            if (more)
            {
                page = tab.GetSearchPage(count);
                if (!String.IsNullOrEmpty(tab.NextPageQuery))
                {
                    querystr = tab.NextPageQuery;
                }
            }
            else
            {
                sinceId = tab.SinceId;
            }

            try
            {
                if (String.IsNullOrEmpty(querystr))
                {
                    res = _twCon.PhoenixSearch(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
                }
                else
                {
                    res = _twCon.PhoenixSearch(querystr, ref content);
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.BadRequest:
                    return "Invalid query";
                case HttpStatusCode.NotFound:
                    return "Invalid query";
                case HttpStatusCode.PaymentRequired:
                    //API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab))
            {
                return "";
            }

            var oid = tab.OldestId;
            var npq = tab.NextPageQuery;
            var ret = CreatePostsFromPhoenixSearch(content, MyCommon.WorkerType.PublicSearch, tab, read, count, ref oid, ref npq);
            tab.OldestId = oid;
            tab.NextPageQuery = npq;
            return ret;
        }

        private string CreateDirectMessagesFromJson(string content, MyCommon.WorkerType gType, bool read)
        {
            List<TwitterDataModel.Directmessage> item = null;
            try
            {
                if (gType == MyCommon.WorkerType.UserStream)
                {
                    List<TwitterDataModel.DirectmessageEvent> itm = MyCommon.CreateDataFromJson<List<TwitterDataModel.DirectmessageEvent>>(content);
                    item = new List<TwitterDataModel.Directmessage>();
                    foreach (TwitterDataModel.DirectmessageEvent dat in itm)
                    {
                        item.Add(dat.Directmessage);
                    }
                }
                else
                {
                    item = MyCommon.CreateDataFromJson<List<TwitterDataModel.Directmessage>>(content);
                }
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (TwitterDataModel.Directmessage message in item)
            {
                PostClass post = new PostClass();
                try
                {
                    post.StatusId = message.Id;
                    if (gType != MyCommon.WorkerType.UserStream)
                    {
                        if (gType == MyCommon.WorkerType.DirectMessegeRcv)
                        {
                            if (_minDirectmessage > post.StatusId)
                            {
                                _minDirectmessage = post.StatusId;
                            }
                        }
                        else
                        {
                            if (_minDirectmessageSent > post.StatusId)
                            {
                                _minDirectmessageSent = post.StatusId;
                            }
                        }
                    }

                    //二重取得回避
                    lock (_lockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.DirectMessage).Contains(post.StatusId))
                        {
                            continue;
                        }
                    }
                    //sender_id
                    //recipient_id
                    post.CreatedAt = MyCommon.DateTimeParse(message.CreatedAt);
                    //本文
                    post.TextFromApi = message.Text;
                    //HTMLに整形
                    post.Text = CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, post.Media);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
                    post.IsFav = false;

                    //以下、ユーザー情報
                    TwitterDataModel.User user = null;
                    if (gType == MyCommon.WorkerType.UserStream)
                    {
                        if (_twCon.AuthenticatedUsername.Equals(message.Recipient.ScreenName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            user = message.Sender;
                            post.IsMe = false;
                            post.IsOwl = true;
                        }
                        else
                        {
                            user = message.Recipient;
                            post.IsMe = true;
                            post.IsOwl = false;
                        }
                    }
                    else
                    {
                        if (gType == MyCommon.WorkerType.DirectMessegeRcv)
                        {
                            user = message.Sender;
                            post.IsMe = false;
                            post.IsOwl = true;
                        }
                        else
                        {
                            user = message.Recipient;
                            post.IsMe = true;
                            post.IsOwl = false;
                        }
                    }

                    post.UserId = user.Id;
                    post.ScreenName = user.ScreenName;
                    post.Nickname = user.Name.Trim();
                    post.ImageUrl = user.ProfileImageUrl;
                    post.IsProtect = user.Protected;
                }
                catch (Exception ex)
                {
                    MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                    MessageBox.Show("Parse Error(CreateDirectMessagesFromJson)");
                    continue;
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost)
                {
                    post.IsRead = true;
                }
                post.IsReply = false;
                post.IsExcludeReply = false;
                post.IsDm = true;

                TabInformations.GetInstance().AddPost(post);
            }

            return "";
        }

        public string GetDirectMessageApi(bool read, Hoehoe.MyCommon.WorkerType gType, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.TwitterApiInfo.AccessLevel != ApiAccessLevel.None)
            {
                if (!MyCommon.TwitterApiInfo.IsDirectMessagePermission)
                {
                    return "Auth Err:try to re-authorization.";
                }
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                if (gType == MyCommon.WorkerType.DirectMessegeRcv)
                {
                    if (more)
                    {
                        res = _twCon.DirectMessages(20, _minDirectmessage, 0, ref content);
                    }
                    else
                    {
                        res = _twCon.DirectMessages(20, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = _twCon.DirectMessagesSent(20, _minDirectmessageSent, 0, ref content);
                    }
                    else
                    {
                        res = _twCon.DirectMessagesSent(20, 0, 0, ref content);
                    }
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return CreateDirectMessagesFromJson(content, gType, read);
        }

        int _prevFavPage = 1;

        public string GetFavoritesApi(bool read, Hoehoe.MyCommon.WorkerType gType, bool more)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            if (MyCommon.IsEnding)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            int count = AppendSettingDialog.Instance.CountApi;
            if (AppendSettingDialog.Instance.UseAdditionalCount && AppendSettingDialog.Instance.FavoritesCountApi != 0)
            {
                count = AppendSettingDialog.Instance.FavoritesCountApi;
            }

            // 前ページ取得の場合はページカウンタをインクリメント、それ以外の場合はページカウンタリセット
            if (more)
            {
                _prevFavPage += 1;
            }
            else
            {
                _prevFavPage = 1;
            }

            try
            {
                res = _twCon.Favorites(count, _prevFavPage, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<TwitterDataModel.Status>));
            List<TwitterDataModel.Status> item = null;
            try
            {
                item = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (TwitterDataModel.Status status in item)
            {
                PostClass post = new PostClass();
                TwitterDataModel.Entities entities = null;

                try
                {
                    post.StatusId = status.Id;
                    //二重取得回避
                    lock (_lockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Contains(post.StatusId))
                        {
                            continue;
                        }
                    }
                    //Retweet判定
                    if (status.RetweetedStatus != null)
                    {
                        TwitterDataModel.RetweetedStatus retweeted = status.RetweetedStatus;
                        post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);
                        //Id
                        post.RetweetedId = post.StatusId;
                        //本文
                        post.TextFromApi = retweeted.Text;
                        entities = retweeted.Entities;
                        //Source取得（htmlの場合は、中身を取り出し）
                        post.Source = retweeted.Source;
                        //Reply先
                        { long t; long.TryParse(retweeted.InReplyToStatusId, out t); post.InReplyToStatusId = t; }
                        post.InReplyToUser = retweeted.InReplyToScreenName;
                        { long t; long.TryParse(retweeted.InReplyToUserId, out t); post.InReplyToUserId = t; }
                        post.IsFav = true;
                        //以下、ユーザー情報
                        TwitterDataModel.User user = retweeted.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrl;
                        post.IsProtect = user.Protected;
                        //Retweetした人
                        post.RetweetedBy = status.User.ScreenName;
                        post.IsMe = post.RetweetedBy.ToLower().Equals(_uname);
                    }
                    else
                    {
                        post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                        //本文
                        post.TextFromApi = status.Text;
                        entities = status.Entities;
                        //Source取得（htmlの場合は、中身を取り出し）
                        post.Source = status.Source;
                        { long t; long.TryParse(status.InReplyToStatusId, out t); post.InReplyToStatusId = t; }
                        post.InReplyToUser = status.InReplyToScreenName;
                        { long t; long.TryParse(status.InReplyToUserId, out t); post.InReplyToUserId = t; }

                        post.IsFav = true;
                        //以下、ユーザー情報
                        TwitterDataModel.User user = status.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrl;
                        post.IsProtect = user.Protected;
                        post.IsMe = post.ScreenName.ToLower().Equals(_uname);
                    }
                    //HTMLに整形
                    { var t = post.TextFromApi; post.Text = CreateHtmlAnchor(ref t, post.ReplyToList, entities, post.Media); post.TextFromApi = t; }
                    post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
                    //Source整形
                    CreateSource(ref post);
                    post.IsRead = read;
                    post.IsReply = post.ReplyToList.Contains(_uname);
                    post.IsExcludeReply = false;

                    if (post.IsMe)
                    {
                        post.IsOwl = false;
                    }
                    else
                    {
                        if (_followerIds.Count > 0)
                        {
                            post.IsOwl = !_followerIds.Contains(post.UserId);
                        }
                    }

                    post.IsDm = false;
                }
                catch (Exception ex)
                {
                    MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                    continue;
                }

                TabInformations.GetInstance().AddPost(post);
            }

            return "";
        }

        private string ReplaceTextFromApi(string text, TwitterDataModel.Entities entities)
        {
            if (entities != null)
            {
                if (entities.Urls != null)
                {
                    foreach (var m in entities.Urls)
                    {
                        if (!String.IsNullOrEmpty(m.DisplayUrl))
                        {
                            text = text.Replace(m.Url, m.DisplayUrl);
                        }
                    }
                }
                if (entities.Media != null)
                {
                    foreach (var m in entities.Media)
                    {
                        if (!String.IsNullOrEmpty(m.DisplayUrl))
                        {
                            text = text.Replace(m.Url, m.DisplayUrl);
                        }
                    }
                }
            }
            return text;
        }

        public string GetFollowersApi()
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            long cursor = -1;
            List<long> tmpFollower = new List<long>(_followerIds);
            _followerIds.Clear();
            do
            {
                string ret = FollowerApi(ref cursor);
                if (!String.IsNullOrEmpty(ret))
                {
                    _followerIds.Clear();
                    _followerIds.AddRange(tmpFollower);
                    _getFollowerResult = false;
                    return ret;
                }
            } while (cursor > 0);

            TabInformations.GetInstance().RefreshOwl(_followerIds);

            _getFollowerResult = true;
            return "";
        }

        public bool GetFollowersSuccess
        {
            get { return _getFollowerResult; }
        }

        private string FollowerApi(ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.FollowerIds(cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var followers = MyCommon.CreateDataFromJson<TwitterDataModel.Ids>(content);
                _followerIds.AddRange(followers.Id);
                cursor = followers.NextCursor;
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string GetNoRetweetIdsApi()
        {
            if (MyCommon.IsEnding)
            {
                return "";
            }

            long cursor = -1;
            List<long> tmpIds = new List<long>(noRTIds);
            noRTIds.Clear();
            do
            {
                string ret = NoRetweetApi(ref cursor);
                if (!String.IsNullOrEmpty(ret))
                {
                    noRTIds.Clear();
                    noRTIds.AddRange(tmpIds);
                    _getNoRetweetResult = false;
                    return ret;
                }
            } while (cursor > 0);

            _getNoRetweetResult = true;
            return "";
        }

        private string NoRetweetApi(ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.NoRetweetIds(cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var ids = MyCommon.CreateDataFromJson<long[]>(content);
                noRTIds.AddRange(ids);
                cursor = 0;
                //0より小さければ何でも良い。
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public bool GetNoRetweetSuccess
        {
            get { return _getNoRetweetResult; }
        }

        public string ConfigurationApi()
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.GetConfiguration(ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                AppendSettingDialog.Instance.TwitterConfiguration = MyCommon.CreateDataFromJson<TwitterDataModel.Configuration>(content);
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string GetListsApi()
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            long cursor = -1;
            List<ListElement> lists = new List<ListElement>();
            do
            {
                try
                {
                    res = _twCon.GetLists(this.Username, cursor, ref content);
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = MyCommon.AccountState.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = MyCommon.CreateDataFromJson<TwitterDataModel.Lists>(content);
                    lists.AddRange(from le in lst.Lists_ select new ListElement(le, this));
                    cursor = lst.NextCursor;
                }
                catch (SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch (Exception ex)
                {
                    MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
            } while (cursor != 0);

            cursor = -1;
            content = "";
            do
            {
                try
                {
                    res = _twCon.GetListsSubscriptions(this.Username, cursor, ref content);
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = MyCommon.AccountState.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = MyCommon.AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = MyCommon.CreateDataFromJson<TwitterDataModel.Lists>(content);
                    lists.AddRange(from le in lst.Lists_ select new ListElement(le, this));
                    cursor = lst.NextCursor;
                }
                catch (SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch (Exception ex)
                {
                    MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
            } while (cursor != 0);

            TabInformations.GetInstance().SubscribableLists = lists;
            return "";
        }

        public string DeleteList(string listId)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";

            try
            {
                res = _twCon.DeleteListID(this.Username, listId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return "";
        }

        public string EditList(string listId, string newName, bool isPrivate, string description, ref ListElement list)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = "";

            try
            {
                res = _twCon.UpdateListID(this.Username, listId, newName, isPrivate, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = MyCommon.CreateDataFromJson<TwitterDataModel.ListElementData>(content);
                ListElement newList = new ListElement(le, this);
                list.Description = newList.Description;
                list.Id = newList.Id;
                list.IsPublic = newList.IsPublic;
                list.MemberCount = newList.MemberCount;
                list.Name = newList.Name;
                list.SubscriberCount = newList.SubscriberCount;
                list.Slug = newList.Slug;
                list.Nickname = newList.Nickname;
                list.Username = newList.Username;
                list.UserId = newList.UserId;
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string GetListMembers(string listId, List<UserInfo> lists, ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.GetListMembers(this.Username, listId, cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var users = MyCommon.CreateDataFromJson<TwitterDataModel.Users>(content);
                Array.ForEach<TwitterDataModel.User>(users.users, new Action<TwitterDataModel.User>(u => { lists.Add(new UserInfo(u)); }));
                cursor = users.NextCursor;
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string CreateListApi(string listName, bool isPrivate, string description)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.CreateLists(listName, isPrivate, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = MyCommon.CreateDataFromJson<TwitterDataModel.ListElementData>(content);
                TabInformations.GetInstance().SubscribableLists.Add(new ListElement(le, this));
                return "";
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string ContainsUserAtList(string listId, string user, ref bool value)
        {
            value = false;

            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = this._twCon.ShowListMember(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.NotFound:
                    value = false;
                    return "";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var u = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                value = true;
                return "";
            }
            catch (Exception)
            {
                value = false;
                return "";
            }
        }

        public string AddUserToList(string listId, string user)
        {
            string content = "";
            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = _twCon.CreateListMembers(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return "";
        }

        public string RemoveUserToList(string listId, string user)
        {
            string content = "";
            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = _twCon.DeleteListMembers(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
            return "";
        }

        private class range
        {
            public int fromIndex { get; set; }

            public int toIndex { get; set; }

            public range(int fromIndex, int toIndex)
            {
                this.fromIndex = fromIndex;
                this.toIndex = toIndex;
            }
        }

        public string CreateHtmlAnchor(string text, List<string> atList, Dictionary<string, string> media)
        {
            if (text == null)
            {
                return null;
            }
            string retStr = text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>");
            //絶対パス表現のUriをリンクに置換
            retStr = Regex.Replace(retStr, RgUrl, new MatchEvaluator((Match mu) =>
            {
                StringBuilder sb = new StringBuilder(mu.Result("${before}<a href=\""));
                string url = mu.Result("${url}");
                string title = ShortUrl.ResolveMedia(url, true);
                if (url != title)
                {
                    title = ShortUrl.ResolveMedia(title, false);
                }
                sb.Append(url + "\" title=\"" + title + "\">").Append(url).Append("</a>");
                if (media != null && !media.ContainsKey(url))
                {
                    media.Add(url, title);
                }
                return sb.ToString();
            }), RegexOptions.IgnoreCase);

            //@先をリンクに置換（リスト）
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\\p{IsLatin-1Supplement}\\-]{0,79})", "$1$2<a href=\"/$3\">$3</a>");
            Match m = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})");
            while (m.Success)
            {
                if (!atList.Contains(m.Result("$2").ToLower()))
                {
                    atList.Add(m.Result("$2").ToLower());
                }
                m = m.NextMatch();
            }
            //@先をリンクに置換
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})", "$1$2<a href=\"/$3\">$3</a>");

            //ハッシュタグを抽出し、リンクに置換
            List<range> anchorRange = new List<range>();
            for (int i = 0; i < retStr.Length; i++)
            {
                int index = retStr.IndexOf("<a ", i);
                if (index > -1 && index < retStr.Length)
                {
                    i = index;
                    int toIndex = retStr.IndexOf("</a>", index);
                    if (toIndex > -1)
                    {
                        anchorRange.Add(new range(index, toIndex + 3));
                        i = toIndex;
                    }
                }
            }
            retStr = Regex.Replace(retStr, HASHTAG, new MatchEvaluator((Match mh) =>
            {
                foreach (range rng in anchorRange)
                {
                    if (mh.Index >= rng.fromIndex && mh.Index <= rng.toIndex)
                    {
                        return mh.Result("$0");
                    }
                }
                lock (_lockObj)
                {
                    _hashList.Add("#" + mh.Result("$3"));
                }
                return mh.Result("$1") + "<a href=\"" + _protocol + "twitter.com/search?q=%23" + mh.Result("$3") + "\">" + mh.Result("$2$3") + "</a>";
            }), RegexOptions.IgnoreCase);
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;");
            //retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr), True)) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            retStr = AdjustHtml(PreProcessUrl(retStr));
            //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return retStr;
        }

        private class EntityInfo
        {
            public int StartIndex { get; set; }

            public int EndIndex { get; set; }

            public string Text { get; set; }

            public string Html { get; set; }

            public string Display { get; set; }
        }

        public string CreateHtmlAnchor(ref string text, List<string> atList, TwitterDataModel.Entities entities, Dictionary<string, string> media)
        {
            string ret = text;

            if (entities != null)
            {
                SortedList<int, EntityInfo> etInfo = new SortedList<int, EntityInfo>();
                //URL
                if (entities.Urls != null)
                {
                    foreach (var ent in entities.Urls)
                    {
                        if (String.IsNullOrEmpty(ent.DisplayUrl))
                        {
                            etInfo.Add(ent.Indices[0], new EntityInfo
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = "<a href=\"" + ent.Url + "\">" + ent.Url + "</a>"
                            });
                        }
                        else
                        {
                            string expanded = ShortUrl.ResolveMedia(ent.ExpandedUrl, false);
                            etInfo.Add(ent.Indices[0], new EntityInfo
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = "<a href=\"" + ent.Url + "\" title=\"" + expanded + "\">" + ent.DisplayUrl + "</a>",
                                Display = ent.DisplayUrl
                            });
                            if (media != null && !media.ContainsKey(ent.Url))
                            {
                                media.Add(ent.Url, expanded);
                            }
                        }
                    }
                }

                if (entities.Hashtags != null)
                {
                    foreach (var ent in entities.Hashtags)
                    {
                        string hash = text.Substring(ent.Indices[0], ent.Indices[1] - ent.Indices[0]);
                        etInfo.Add(ent.Indices[0], new EntityInfo
                        {
                            StartIndex = ent.Indices[0],
                            EndIndex = ent.Indices[1],
                            Text = hash,
                            Html = "<a href=\"" + _protocol + "twitter.com/search?q=%23" + ent.Text + "\">" + hash + "</a>"
                        });
                        lock (_lockObj)
                        {
                            _hashList.Add("#" + ent.Text);
                        }
                    }
                }

                if (entities.UserMentions != null)
                {
                    foreach (var ent in entities.UserMentions)
                    {
                        string screenName = text.Substring(ent.Indices[0] + 1, ent.Indices[1] - ent.Indices[0] - 1);
                        etInfo.Add(ent.Indices[0] + 1, new EntityInfo
                        {
                            StartIndex = ent.Indices[0] + 1,
                            EndIndex = ent.Indices[1],
                            Text = ent.ScreenName,
                            Html = "<a href=\"/" + ent.ScreenName + "\">" + screenName + "</a>"
                        });
                        if (!atList.Contains(ent.ScreenName.ToLower()))
                        {
                            atList.Add(ent.ScreenName.ToLower());
                        }
                    }
                }

                if (entities.Media != null)
                {
                    foreach (var ent  in entities.Media)
                    {
                        if (ent.Type == "photo")
                        {
                            etInfo.Add(ent.Indices[0], new EntityInfo
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = "<a href=\"" + ent.Url + "\" title=\"" + ent.ExpandedUrl + "\">" + ent.DisplayUrl + "</a>",
                                Display = ent.DisplayUrl
                            });
                            if (media != null && !media.ContainsKey(ent.Url))
                                media.Add(ent.Url, ent.MediaUrl);
                        }
                    }
                }

                if (etInfo.Count > 0)
                {
                    try
                    {
                        int idx = 0;
                        ret = "";
                        foreach (var et  in etInfo)
                        {   ret += text.Substring(idx, et.Key - idx) + et.Value.Html;
                            idx = et.Value.EndIndex;
                        }
                        ret += text.Substring(idx);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Twitterのバグで不正なエンティティ（Index指定範囲が重なっている）が返ってくる場合の対応
                        ret = text;
                        entities = null;
                        if (media != null)
                        {
                            media.Clear();
                        }
                    }
                }
            }

            ret = Regex.Replace(ret, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            ret = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(ret), false));//IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return ret;
        }

        //Source整形
        private void CreateSource(ref PostClass post)
        {
            if (post.Source.StartsWith("<"))
            {
                if (!post.Source.Contains("</a>"))
                {
                    post.Source += "</a>";
                }
                Match mS = Regex.Match(post.Source, ">(?<source>.+)<");
                if (mS.Success)
                {
                    post.SourceHtml = string.Copy(ShortUrl.Resolve(PreProcessUrl(post.Source), false));
                    post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"));
                }
                else
                {
                    post.Source = "";
                    post.SourceHtml = "";
                }
            }
            else
            {
                if (post.Source == "web")
                {
                    post.SourceHtml = Hoehoe.Properties.Resources.WebSourceString;
                }
                else if (post.Source == "Keitai Mail")
                {
                    post.SourceHtml = Hoehoe.Properties.Resources.KeitaiMailSourceString;
                }
                else
                {
                    post.SourceHtml = string.Copy(post.Source);
                }
            }
        }

        public bool GetInfoApi(ApiInfo info)
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return true;
            }

            if (MyCommon.IsEnding)
            {
                return true;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.RateLimitStatus(ref content);
            }
            catch (Exception)
            {
                MyCommon.TwitterApiInfo.Initialize();
                return false;
            }

            if (res != HttpStatusCode.OK)
            {
                return false;
            }

            try
            {
                var limit = MyCommon.CreateDataFromJson<TwitterDataModel.RateLimitStatus>(content);
                ApiInformationChangedEventArgs arg = new ApiInformationChangedEventArgs();
                arg.ApiInfo.MaxCount = limit.HourlyLimit;
                arg.ApiInfo.RemainCount = limit.RemainingHits;
                arg.ApiInfo.ResetTime = MyCommon.DateTimeParse(limit.RestTime);
                arg.ApiInfo.ResetTimeInSeconds = limit.RestTimeInSeconds;
                if (info != null)
                {
                    arg.ApiInfo.UsingCount = info.UsingCount;
                    info.MaxCount = arg.ApiInfo.MaxCount;
                    info.RemainCount = arg.ApiInfo.RemainCount;
                    info.ResetTime = arg.ApiInfo.ResetTime;
                    info.ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
                }

                if (ApiInformationChanged != null)
                {
                    ApiInformationChanged(this, arg);
                }
                MyCommon.TwitterApiInfo.WriteBackEventArgs(arg);
                return true;
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                MyCommon.TwitterApiInfo.Initialize();
                return false;
            }
        }

        public string GetBlockUserIds()
        {
            if (Twitter.AccountState != MyCommon.AccountState.Valid)
            {
                return "";
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = "";
            try
            {
                res = _twCon.GetBlockUserIds(ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var ids = MyCommon.CreateDataFromJson<List<long>>(content);
                if (ids.Contains(this.UserId))
                {
                    ids.Remove(this.UserId);
                }
                TabInformations.GetInstance().BlockIds.AddRange(ids);
                return ("");
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string[] GetHashList()
        {
            string[] hashArray = null;
            lock (_lockObj)
            {
                hashArray = _hashList.ToArray();
                _hashList.Clear();
            }
            return hashArray;
        }

        public string AccessToken
        {
            get { return _twCon.AccessToken; }
        }

        public string AccessTokenSecret
        {
            get { return _twCon.AccessTokenSecret; }
        }

        public event ApiInformationChangedEventHandler ApiInformationChanged;

        public delegate void ApiInformationChangedEventHandler(object sender, ApiInformationChangedEventArgs e);

        private void Twitter_ApiInformationChanged(object sender, ApiInformationChangedEventArgs e)
        {
        }

        #region "UserStream"

        public string TrackWord { get; set; }

        public bool AllAtReply { get; set; }

        public event NewPostFromStreamEventHandler NewPostFromStream;

        public delegate void NewPostFromStreamEventHandler();

        public event UserStreamStartedEventHandler UserStreamStarted;

        public delegate void UserStreamStartedEventHandler();

        public event UserStreamStoppedEventHandler UserStreamStopped;

        public delegate void UserStreamStoppedEventHandler();

        //public event UserStreamGetFriendsListEventHandler UserStreamGetFriendsList;

        public delegate void UserStreamGetFriendsListEventHandler();

        public event PostDeletedEventHandler PostDeleted;

        public delegate void PostDeletedEventHandler(long id);

        public event UserStreamEventReceivedEventHandler UserStreamEventReceived;

        public delegate void UserStreamEventReceivedEventHandler(FormattedEvent eventType);
        private DateTime _lastUserstreamDataReceived;
        private TwitterUserstream withEventsField_userStream;

        private TwitterUserstream userStream
        {
            get { return withEventsField_userStream; }
            set
            {
                if (withEventsField_userStream != null)
                {
                    withEventsField_userStream.StatusArrived -= userStream_StatusArrived;
                    withEventsField_userStream.Started -= userStream_Started;
                    withEventsField_userStream.Stopped -= userStream_Stopped;
                }
                withEventsField_userStream = value;
                if (withEventsField_userStream != null)
                {
                    withEventsField_userStream.StatusArrived += userStream_StatusArrived;
                    withEventsField_userStream.Started += userStream_Started;
                    withEventsField_userStream.Stopped += userStream_Stopped;
                }
            }
        }

        public class FormattedEvent
        {
            public Hoehoe.MyCommon.EventType Eventtype { get; set; }

            public DateTime CreatedAt { get; set; }

            public string Event { get; set; }

            public string Username { get; set; }

            public string Target { get; set; }

            public Int64 Id { get; set; }

            public bool IsMe { get; set; }
        }

        public List<FormattedEvent> StoredEvent { get; set; }

        private class EventTypeTableElement
        {
            public string Name;

            public Hoehoe.MyCommon.EventType Type;

            public EventTypeTableElement(string name, Hoehoe.MyCommon.EventType type)
            {
                this.Name = name;
                this.Type = type;
            }
        }

        private EventTypeTableElement[] EventTable = {
			new EventTypeTableElement("favorite", MyCommon.EventType.Favorite),
			new EventTypeTableElement("unfavorite", MyCommon.EventType.Unfavorite),
			new EventTypeTableElement("follow", MyCommon.EventType.Follow),
			new EventTypeTableElement("list_member_added", MyCommon.EventType.ListMemberAdded),
			new EventTypeTableElement("list_member_removed", MyCommon.EventType.ListMemberRemoved),
			new EventTypeTableElement("block",MyCommon. EventType.Block),
			new EventTypeTableElement("unblock",MyCommon. EventType.Unblock),
			new EventTypeTableElement("user_update",MyCommon. EventType.UserUpdate),
			new EventTypeTableElement("deleted", MyCommon.EventType.Deleted),
			new EventTypeTableElement("list_created", MyCommon.EventType.ListCreated),
			new EventTypeTableElement("list_updated", MyCommon.EventType.ListUpdated)
		};

        public MyCommon.EventType EventNameToEventType(string eventName)
        {
            return (from tbl in EventTable where tbl.Name.Equals(eventName) select tbl.Type).FirstOrDefault();
        }

        public bool IsUserstreamDataReceived
        {
            get { return DateTime.Now.Subtract(this._lastUserstreamDataReceived).TotalSeconds < 31; }
        }

        private void userStream_StatusArrived(string line)
        {
            this._lastUserstreamDataReceived = DateTime.Now;
            if (String.IsNullOrEmpty(line))
            {
                return;
            }

            bool isDm = false;
            try
            {
                using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(line), XmlDictionaryReaderQuotas.Max))
                {
                    XElement xElm = XElement.Load(jsonReader);
                    if (xElm.Element("friends") != null)
                    {
                        Debug.Print("friends");
                        return;
                    }
                    if (xElm.Element("delete") != null)
                    {
                        Debug.Print("delete");
                        Int64 id = default(Int64);
                        if (xElm.Element("delete").Element("direct_message") != null && xElm.Element("delete").Element("direct_message").Element("id") != null)
                        {
                            id = Convert.ToInt64(xElm.Element("delete").Element("direct_message").Element("id").Value);
                            if (PostDeleted != null)
                            {
                                PostDeleted(id);
                            }
                        }
                        else if (xElm.Element("delete").Element("status") != null && xElm.Element("delete").Element("status").Element("id") != null)
                        {
                            id = Convert.ToInt64(xElm.Element("delete").Element("status").Element("id").Value);
                            if (PostDeleted != null)
                            {
                                PostDeleted(id);
                            }
                        }
                        else
                        {
                            MyCommon.TraceOut("delete:" + line);
                            return;
                        }
                        for (int i = this.StoredEvent.Count - 1; i >= 0; i--)
                        {
                            FormattedEvent sEvt = this.StoredEvent[i];
                            if (sEvt.Id == id && (sEvt.Event == "favorite" || sEvt.Event == "unfavorite"))
                            {
                                this.StoredEvent.RemoveAt(i);
                            }
                        }
                        return;
                    }
                    if (xElm.Element("limit") != null)
                    {
                        Debug.Print(line);
                        return;
                    }
                    if (xElm.Element("event") != null)
                    {
                        Debug.Print("event: " + xElm.Element("event").Value);
                        CreateEventFromJson(line);
                        return;
                    }
                    if (xElm.Element("direct_message") != null)
                    {
                        Debug.Print("direct_message");
                        isDm = true;
                    }
                    else if (xElm.Element("scrub_geo") != null)
                    {
                        try
                        {
                            TabInformations.GetInstance().ScrubGeoReserve(long.Parse(xElm.Element("scrub_geo").Element("user_id").Value), long.Parse(xElm.Element("scrub_geo").Element("up_to_status_id").Value));
                        }
                        catch (Exception)
                        {
                            MyCommon.TraceOut("scrub_geo:" + line);
                        }
                        return;
                    }
                }

                StringBuilder res = new StringBuilder();
                res.Length = 0;
                res.Append("[");
                res.Append(line);
                res.Append("]");

                if (isDm)
                {
                    CreateDirectMessagesFromJson(res.ToString(), MyCommon.WorkerType.UserStream, false);
                }
                else
                {
                    long t = -1;
                    CreatePostsFromJson(res.ToString(), MyCommon.WorkerType.Timeline, null, false, 0, ref t);
                }
            }
            catch (NullReferenceException)
            {
                MyCommon.TraceOut("NullRef StatusArrived: " + line);
            }

            if (NewPostFromStream != null)
            {
                NewPostFromStream();
            }
        }

        private void CreateEventFromJson(string content)
        {
            TwitterDataModel.EventData eventData = null;
            try
            {
                eventData = MyCommon.CreateDataFromJson<TwitterDataModel.EventData>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex, "Event Serialize Exception!" + Environment.NewLine + content);
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, "Event Exception!" + Environment.NewLine + content);
            }

            FormattedEvent evt = new FormattedEvent();
            evt.CreatedAt = MyCommon.DateTimeParse(eventData.CreatedAt);
            evt.Event = eventData.Event;
            evt.Username = eventData.Source.ScreenName;
            evt.IsMe = evt.Username.ToLower().Equals(this.Username.ToLower());
            evt.Eventtype = EventNameToEventType(evt.Event);
            switch (eventData.Event)
            {
                case "access_revoked":
                    return;
                case "follow":
                    if (eventData.Target.ScreenName.ToLower().Equals(_uname))
                    {
                        if (!this._followerIds.Contains(eventData.Source.Id))
                            this._followerIds.Add(eventData.Source.Id);
                    }
                    else
                    {
                        return;
                        //Block後のUndoをすると、SourceとTargetが逆転したfollowイベントが帰ってくるため。
                    }
                    evt.Target = "";
                    break;
                case "favorite":
                case "unfavorite":
                    evt.Target = "@" + eventData.TargetObject.User.ScreenName + ":" + HttpUtility.HtmlDecode(eventData.TargetObject.Text);
                    evt.Id = eventData.TargetObject.Id;
                    if (AppendSettingDialog.Instance.IsRemoveSameEvent)
                    {
                        if (StoredEvent.Any((FormattedEvent ev) => { return ev.Username == evt.Username && ev.Eventtype == evt.Eventtype && ev.Target == evt.Target; }))
                        {
                            return;
                        }
                    }
                    if (TabInformations.GetInstance().ContainsKey(eventData.TargetObject.Id))
                    {
                        PostClass post = TabInformations.GetInstance().Item(eventData.TargetObject.Id);
                        if (eventData.Event == "favorite")
                        {
                            if (evt.Username.ToLower().Equals(_uname))
                            {
                                post.IsFav = true;
                                TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                            }
                            else
                            {
                                post.FavoritedCount += 1;
                                if (!TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Contains(post.StatusId))
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread && post.IsRead)
                                    {
                                        post.IsRead = false;
                                    }
                                    TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                }
                                else
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread)
                                    {
                                        TabInformations.GetInstance().SetRead(false, TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).TabName, TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).IndexOf(post.StatusId));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (evt.Username.ToLower().Equals(_uname))
                            {
                                post.IsFav = false;
                            }
                            else
                            {
                                post.FavoritedCount -= 1;
                                if (post.FavoritedCount < 0)
                                {
                                    post.FavoritedCount = 0;
                                }
                            }
                        }
                    }
                    break;
                case "list_member_added":
                case "list_member_removed":
                case "list_updated":
                    evt.Target = eventData.TargetObject.FullName;
                    break;
                case "block":
                    if (!TabInformations.GetInstance().BlockIds.Contains(eventData.Target.Id))
                    {
                        TabInformations.GetInstance().BlockIds.Add(eventData.Target.Id);
                    }
                    evt.Target = "";
                    break;
                case "unblock":
                    if (TabInformations.GetInstance().BlockIds.Contains(eventData.Target.Id))
                    {
                        TabInformations.GetInstance().BlockIds.Remove(eventData.Target.Id);
                    }
                    evt.Target = "";
                    break;
                case "user_update":
                    evt.Target = "";
                    break;
                case "list_created":
                    evt.Target = "";
                    break;
                default:
                    MyCommon.TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content);
                    break;
            }
            this.StoredEvent.Insert(0, evt);
            if (UserStreamEventReceived != null)
            {
                UserStreamEventReceived(evt);
            }
        }

        private void userStream_Started()
        {
            if (UserStreamStarted != null)
            {
                UserStreamStarted();
            }
        }

        private void userStream_Stopped()
        {
            if (UserStreamStopped != null)
            {
                UserStreamStopped();
            }
        }

        public bool UserStreamEnabled
        {
            get { return userStream == null ? false : userStream.Enabled; }
        }

        public void StartUserStream()
        {
            if (userStream != null)
            {
                StopUserStream();
            }
            userStream = new TwitterUserstream(_twCon);
            userStream.Start(this.AllAtReply, this.TrackWord);
        }

        public void StopUserStream()
        {
            if (userStream != null)
            {
                userStream.Dispose();
            }
            userStream = null;
            if (!MyCommon.IsEnding)
            {
                if (UserStreamStopped != null)
                {
                    UserStreamStopped();
                }
            }
        }

        public void ReconnectUserStream()
        {
            if (userStream != null)
            {
                this.StartUserStream();
            }
        }

        private class TwitterUserstream : IDisposable
        {
            public event StatusArrivedEventHandler StatusArrived;

            public delegate void StatusArrivedEventHandler(string status);

            public event StoppedEventHandler Stopped;

            public delegate void StoppedEventHandler();

            public event StartedEventHandler Started;

            public delegate void StartedEventHandler();

            private HttpTwitter _twCon;
            private Thread _streamThread;
            private bool _streamActive;
            private bool _allAtreplies;
            private string _trackwords = "";

            public TwitterUserstream(HttpTwitter twitterConnection)
            {
                _twCon = (HttpTwitter)twitterConnection.Clone();
            }

            public void Start(bool allAtReplies, string trackwords)
            {
                this.AllAtReplies = allAtReplies;
                this.TrackWords = trackwords;
                _streamActive = true;
                if (_streamThread != null && _streamThread.IsAlive)
                {
                    return;
                }
                _streamThread = new Thread(UserStreamLoop);
                _streamThread.Name = "UserStreamReceiver";
                _streamThread.IsBackground = true;
                _streamThread.Start();
            }

            public bool Enabled
            {
                get { return _streamActive; }
            }

            public bool AllAtReplies
            {
                get { return _allAtreplies; }
                set { _allAtreplies = value; }
            }

            public string TrackWords
            {
                get { return _trackwords; }
                set { _trackwords = value; }
            }

            private void UserStreamLoop()
            {
                Stream st = null;
                StreamReader sr = null;
                int sleepSec = 0;
                do
                {
                    try
                    {
                        if (!MyCommon.IsNetworkAvailable())
                        {
                            sleepSec = 30;
                            continue;
                        }

                        if (Started != null)
                        {
                            Started();
                        }
                        HttpStatusCode res = _twCon.UserStream(ref st, _allAtreplies, _trackwords, MyCommon.GetUserAgentString());
                        switch (res)
                        {
                            case HttpStatusCode.OK:
                                Twitter.AccountState = MyCommon.AccountState.Valid;
                                break;
                            case HttpStatusCode.Unauthorized:
                                Twitter.AccountState = MyCommon.AccountState.Invalid;
                                sleepSec = 120;
                                continue;
                        }

                        if (st == null)
                        {
                            sleepSec = 30;
                            continue;
                        }

                        sr = new StreamReader(st);

                        while (_streamActive && !sr.EndOfStream && Twitter.AccountState == MyCommon.AccountState.Valid)
                        {
                            if (StatusArrived != null)
                            {
                                StatusArrived(sr.ReadLine());
                            }
                        }

                        if (sr.EndOfStream || Twitter.AccountState == MyCommon.AccountState.Invalid)
                        {
                            sleepSec = 30;
                            continue;
                        }
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.Timeout)
                        {
                            sleepSec = 30;
                        }
                        else if (ex.Response != null && (int)((HttpWebResponse)ex.Response).StatusCode == 420)
                        {
                            break;
                        }
                        else
                        {
                            sleepSec = 30;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (IOException)
                    {
                        sleepSec = 30;
                    }
                    catch (ArgumentException ex)
                    {
                        //System.ArgumentException: ストリームを読み取れませんでした。
                        //サーバー側もしくは通信経路上で切断された場合？タイムアウト頻発後発生
                        sleepSec = 30;
                        MyCommon.TraceOut(ex, "Stop:ArgumentException");
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut("Stop:Exception." + Environment.NewLine + ex.Message);
                        MyCommon.ExceptionOut(ex);
                        sleepSec = 30;
                    }
                    finally
                    {
                        if (_streamActive)
                        {
                            if (Stopped != null)
                            {
                                Stopped();
                            }
                        }
                        _twCon.RequestAbort();
                        if (sr != null)
                        {
                            sr.Close();
                        }
                        if (st != null)
                        {
                            st.Close();
                        }
                        if (sleepSec > 0)
                        {
                            int ms = 0;
                            while (_streamActive && ms < sleepSec * 1000)
                            {
                                Thread.Sleep(500);
                                ms += 500;
                            }
                        }
                        sleepSec = 0;
                    }
                } while (this._streamActive);

                if (_streamActive)
                {
                    if (Stopped != null)
                    {
                        Stopped();
                    }
                }
                MyCommon.TraceOut("Stop:EndLoop");
            }

            #region "IDisposable Support"

            // 重複する呼び出しを検出するには
            private bool disposedValue;

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                        _streamActive = false;
                        if (_streamThread != null && _streamThread.IsAlive)
                        {
                            _streamThread.Abort();
                        }
                    }

                    // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                    // TODO: 大きなフィールドを null に設定します。
                }
                this.disposedValue = true;
            }

            // TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
            //Protected Overrides Sub Finalize()
            //    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            //    Dispose(False)
            //    MyBase.Finalize()
            //End Sub

            // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion "IDisposable Support"
        }

        #endregion "UserStream"

        #region "IDisposable Support"

        // 重複する呼び出しを検出するには
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.StopUserStream();
                }
            }
            this.disposedValue = true;
        }

        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion "IDisposable Support"

        public Twitter()
        {
            ApiInformationChanged += Twitter_ApiInformationChanged;
        }
   }
}