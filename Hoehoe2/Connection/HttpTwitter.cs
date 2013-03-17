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
using System.IO;
using System.Linq;
using System.Net;

namespace Hoehoe
{
    public class HttpTwitter : ICloneable
    {
        /// <summary>
        /// OAuthのコンシューマー鍵 :
        /// </summary>
        // private const string ConsumerKey = "IQKbtAYlXLripLGPWd0HUA"; //
        private const string ConsumerKey = "BIazYuf0scya8pyhLjkdg";

        /// <summary>
        /// OAuthの署名作成用秘密コンシューマーデータ
        /// </summary>
        //private const string ConsumerSecret = "GgDYlkSvaPxGxC4X8liwpUoqKwwr3lCADbz8A7ADU";
        private const string ConsumerSecret = "hVih4pcFCfcpHWXyICLQINmZ1LHXdMzHA4QXMWwBhMQ";

        /// <summary>
        /// OAuthのアクセストークン取得先URI
        /// </summary>
        private const string AccessTokenUrlXAuth = "https://api.twitter.com/oauth/access_token";

        private const string RequestTokenUrl = "https://api.twitter.com/oauth/request_token";
        private const string AuthorizeUrl = "https://api.twitter.com/oauth/authorize";
        private const string AccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        private const string PostMethod = "POST";
        private const string GetMethod = "GET";

        private static string protocol = "http://";
        private static string twitterUrl = "api.twitter.com";
        private static string twitterSearchUrl = "search.twitter.com";
        private const string TwitterUserStreamUrl = "userstream.twitter.com";
        private const string TwitterStreamUrl = "stream.twitter.com";

        /// <summary>
        /// HttpConnectionApi or HttpConnectionOAuth
        /// </summary>
        private IHttpConnection _httpCon;

        private readonly HttpVarious _httpConVar = new HttpVarious();
        private AuthMethod _connectionType = AuthMethod.Basic;

        // for OAuth
        private string _requestToken;

        private string _tk = string.Empty;
        private string _tks = string.Empty;
        private string _un = string.Empty;

        private enum AuthMethod
        {
            OAuth,
            Basic
        }

        public string AccessToken
        {
            get { return _httpCon != null ? ((HttpConnectionOAuth)_httpCon).AccessToken : string.Empty; }
        }

        public string AccessTokenSecret
        {
            get { return _httpCon != null ? ((HttpConnectionOAuth)_httpCon).AccessTokenSecret : string.Empty; }
        }

        public string AuthenticatedUsername
        {
            get { return _httpCon != null ? _httpCon.AuthUsername : string.Empty; }
        }

        public long AuthenticatedUserId
        {
            get
            {
                return _httpCon != null ? _httpCon.AuthUserId : 0;
            }
            set
            {
                if (_httpCon != null)
                {
                    _httpCon.AuthUserId = value;
                }
            }
        }

        public string Password
        {
            get { return string.Empty; }
        }

        public static void SetUseSsl(bool useSsl)
        {
            protocol = useSsl ? "https://" : "http://";
        }

        public static void SetTwitterUrl(string value)
        {
            twitterUrl = value;
            HttpOAuthApiProxy.SetProxyHost(value);
        }

        public static void SetTwitterSearchUrl(string value)
        {
            twitterSearchUrl = value;
        }

        public void Initialize(string accessToken, string accessTokenSecret, string username, long userId)
        {
            var con = new HttpOAuthApiProxy();
            if (_tk != accessToken || _tks != accessTokenSecret || _un != username || _connectionType != AuthMethod.OAuth)
            {
                // 以前の認証状態よりひとつでも変化があったらhttpヘッダより読み取ったカウントは初期化
                _tk = accessToken;
                _tks = accessTokenSecret;
                _un = username;
            }

            con.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username, userId, "screen_name", "user_id");
            _httpCon = con;
            _connectionType = AuthMethod.OAuth;
            _requestToken = string.Empty;
        }

        public bool AuthGetRequestToken(out string content)
        {
            Uri authUri;
            bool result = ((HttpOAuthApiProxy)_httpCon).AuthenticatePinFlowRequest(RequestTokenUrl, AuthorizeUrl, ref _requestToken, out authUri);
            content = authUri == null ? string.Empty : authUri.ToString();
            return result;
        }

        public HttpStatusCode AuthGetAccessToken(string pin)
        {
            return ((HttpOAuthApiProxy)_httpCon).AuthenticatePinFlow(AccessTokenUrl, _requestToken, pin);
        }

        public HttpStatusCode AuthUserAndPass(string username, string password, ref string content)
        {
            return _httpCon.Authenticate(new Uri(AccessTokenUrlXAuth), username, password, ref content);
        }

        public void ClearAuthInfo()
        {
            Initialize(string.Empty, string.Empty, string.Empty, 0);
        }

        public HttpStatusCode UpdateStatus(string status, long replyToId, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "status", status },
                    { "include_entities", "true" }
                };
            if (replyToId > 0)
            {
                param.Add("in_reply_to_status_id", string.Format("{0}", replyToId));
            }

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("statuses", "update"), param, ref content, null, null);
        }

        public HttpStatusCode UpdateStatusWithMedia(string status, long replyToId, FileInfo mediaFile, ref string content)
        {
            // 画像投稿用エンドポイント
            var param = new Dictionary<string, string>
                {
                    { "status", status },
                    { "include_entities", "true" }
                };
            if (replyToId > 0)
            {
                param.Add("in_reply_to_status_id", "" + replyToId);
            }

            var binary = new List<KeyValuePair<string, FileInfo>> { new KeyValuePair<string, FileInfo>("media[]", mediaFile) };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("statuses", "update_with_media"), param, binary, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DestroyStatus(long id)
        {
            var t = string.Empty;
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("statuses", "destroy", "" + id), null, ref t, null, null);
        }

        public HttpStatusCode SendDirectMessage(string status, string sendto, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "text", status },
                    { "screen_name", sendto }
                };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("direct_messages", "new"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyDirectMessage(long id)
        {
            string t = string.Empty;
            var param = new Dictionary<string, string> { { "id", "" + id } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("direct_messages", "destroy"), param, ref t, null, null);
        }

        public HttpStatusCode RetweetStatus(long id, ref string content)
        {
            var param = new Dictionary<string, string> { { "trim_user", "true" } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("statuses", "retweet", "" + id), param, ref content, null, null);
        }

        public HttpStatusCode ShowUserInfo(string screenName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", screenName }, { "include_entities", "true" } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("users", "show"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateFriendships(string screenName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", screenName } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("friendships", "create"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyFriendships(string screenName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", screenName } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("friendships", "destroy"), param, ref content, null, null);
        }

        public HttpStatusCode CreateBlock(string screenName, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "screen_name", screenName },
                    { "include_entities", "" + false },
                    { "skip_status", "" + true }
                };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("blocks", "create"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyBlock(string screenName, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "screen_name", screenName },
                    { "include_entities", "" + false },
                    { "skip_status", "" + true }
                };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("blocks", "destroy"), param, ref content, null, null);
        }

        public HttpStatusCode ReportSpam(string screenName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", screenName } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("users", "report_spam"), param, ref content, null, null);
        }

        public HttpStatusCode ShowFriendships(string souceScreenName, string targetScreenName, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "source_screen_name", souceScreenName },
                    { "target_screen_name", targetScreenName }
                };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("friendships", "show"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode ShowStatuses(long id, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "include_entities", "" + true },
                    { "id", "" + id }
                };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("statuses", "show"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateFavorites(long id, ref string content)
        {
            var param = new Dictionary<string, string> { { "id", "" + id } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("favorites", "create"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyFavorites(long id, ref string content)
        {
            var param = new Dictionary<string, string> { { "id", "" + id } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("favorites", "destroy"), param, ref content, null, null);
        }

        public HttpStatusCode HomeTimeline(int count, long maxID, long sinceID, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "include_entities", "" + true },
                    { "contributor_details", "" + true }
                };
            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (maxID > 0)
            {
                param.Add("max_id", "" + maxID);
            }

            if (sinceID > 0)
            {
                param.Add("since_id", "" + sinceID);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("statuses", "home_timeline"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UserTimeline(long userID, string screenName, int count, long maxID, long sinceID, ref string content)
        {
            if ((userID == 0 && string.IsNullOrEmpty(screenName)) || (userID != 0 && !string.IsNullOrEmpty(screenName)))
            {
                return HttpStatusCode.BadRequest;
            }

            var param = new Dictionary<string, string>
                {
                    { "include_rts", "" + true },
                    { "include_entities", "" + true },
                    { "contributor_details", "" + true }
                };

            if (userID > 0)
            {
                param.Add("user_id", "" + userID);
            }

            if (!string.IsNullOrEmpty(screenName))
            {
                param.Add("screen_name", screenName);
            }

            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (maxID > 0)
            {
                param.Add("max_id", "" + maxID);
            }

            if (sinceID > 0)
            {
                param.Add("since_id", "" + sinceID);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("statuses", "user_timeline"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode Mentions(int count, long maxId, long sinceId, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "include_entities", "" + true },
                    { "contributor_details", "" + true }
                };

            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (maxId > 0)
            {
                param.Add("max_id", "" + maxId);
            }

            if (sinceId > 0)
            {
                param.Add("since_id", "" + sinceId);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("statuses", "mentions_timeline"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DirectMessages(int count, long maxId, long sinceId, ref string content)
        {
            var param = new Dictionary<string, string> { { "include_entities", "true" } };
            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (maxId > 0)
            {
                param.Add("max_id", "" + maxId);
            }

            if (sinceId > 0)
            {
                param.Add("since_id", "" + sinceId);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("direct_messages", string.Empty), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DirectMessagesSent(int count, long maxId, long sinceId, ref string content)
        {
            var param = new Dictionary<string, string> { { "include_entities", "" + true } };
            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (maxId > 0)
            {
                param.Add("max_id", "" + maxId);
            }

            if (sinceId > 0)
            {
                param.Add("since_id", "" + sinceId);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("direct_messages", "sent"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode Favorites(int count, int page, ref string content)
        {
            var param = new Dictionary<string, string> { { "include_entities", "true" } };
            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (page > 0)
            {
                param.Add("page", "" + page);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("favorites", "list"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode Search(string words, string lang, int count, long sinceId, ref string content)
        {
            var param = new Dictionary<string, string> { { "q", words } };
            if (!string.IsNullOrEmpty(lang))
            {
                param.Add("lang", lang);
            }

            if (count > 0)
            {
                param.Add("count", "" + count);
            }

            if (sinceId > 0)
            {
                param.Add("since_id", "" + sinceId);
            }

            return param.Count == 0 ?
                HttpStatusCode.BadRequest :
                _httpCon.GetContent(GetMethod, CreateTwitterUri("search", "tweets"), param, ref content, null, GetApiCallback);
        }

        public HttpStatusCode SavedSearches(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("saved_searches", "list"), null, ref content, null, GetApiCallback);
        }

        public HttpStatusCode FollowerIds(long cursor, ref string content)
        {
            // TODO: add screen_name or user_id
            var param = new Dictionary<string, string> { { "cursor", "" + cursor } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("followers", "ids"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode NoRetweetIds(long cursor, ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("friendships", "no_retweets", "ids"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode RateLimitStatus(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("application", "rate_limit_status"), null, ref content, null, GetApiCallback);
        }

        #region "Lists"

        public HttpStatusCode GetLists(string user, long cursor, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", user } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("lists", "list"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UpdateListID(string user, string listId, string name, bool isPrivate, string description, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "screen_name", user },
                    { "list_id", listId },
                    { "mode", isPrivate ? "private" : "public" }
                };
            if (name != null)
            {
                param.Add("name", name);
            }

            if (description != null)
            {
                param.Add("description", description);
            }

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("lists", "update"), param, ref content, null, null);
        }

        public HttpStatusCode DeleteListID(string user, string listId, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", user }, { "list_id", listId } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("lists", "destroy"), param, ref content, null, null);
        }

        public HttpStatusCode GetListsSubscriptions(string user, long cursor, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", user }, { "cursor", "" + cursor } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("lists", "subscriptions"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetListsStatuses(long userId, long listId, int perPage, long maxId, long sinceId, bool isRTinclude, ref string content)
        {
            // 認証なくても取得できるが、protectedユーザー分が抜ける
            var param = new Dictionary<string, string>
                {
                    { "user_id", "" + userId },
                    { "list_id", "" + listId },
                    { "include_entities", "" + true }
                };
            if (isRTinclude)
            {
                param.Add("include_rts", "" + true);
            }

            if (perPage > 0)
            {
                param.Add("per_page", "" + perPage);
            }

            if (maxId > 0)
            {
                param.Add("max_id", "" + maxId);
            }

            if (sinceId > 0)
            {
                param.Add("since_id", "" + sinceId);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("lists", "statuses"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateLists(string listname, bool isPrivate, string description, ref string content)
        {
            var param = new Dictionary<string, string> { { "name", listname }, { "mode", isPrivate ? "private" : "public" } };
            if (!string.IsNullOrEmpty(description))
            {
                param.Add("description", description);
            }

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("lists", "create"), param, ref content, null, null);
        }

        public HttpStatusCode GetListMembers(string user, string listId, long cursor, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", user }, { "list_id", listId }, { "cursor", "" + cursor } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("lists", "members"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateListMembers(string listId, string memberName, ref string content)
        {
            var param = new Dictionary<string, string> { { "list_id", listId }, { "screen_name", memberName } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("lists", "members", "create"), param, ref content, null, null);
        }

        public HttpStatusCode DeleteListMembers(string listId, string memberName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", memberName }, { "list_id", listId } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("lists", "members", "destroy"), param, ref content, null, null);
        }

        public HttpStatusCode ShowListMember(string listId, string memberName, ref string content)
        {
            var param = new Dictionary<string, string> { { "screen_name", memberName }, { "list_id", listId } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("lists", "members", "show"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        #endregion

        public HttpStatusCode UpdateProfile(string name, string url, string location, string description, ref string content)
        {
            var param = new Dictionary<string, string>
                {
                    { "name", name },
                    { "url", url },
                    { "location", location },
                    { "description", description },
                    { "include_entities", "" + true }
                };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("account", "update_profile"), param, ref content, null, null);
        }

        public HttpStatusCode UpdateProfileImage(FileInfo imageFile, ref string content)
        {
            var binary = new List<KeyValuePair<string, FileInfo>> { new KeyValuePair<string, FileInfo>("image", imageFile) };
            var param = new Dictionary<string, string> { { "include_entities", "" + false }, { "skip_status", "" + true } };
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("account", "update_profile_image"), param, binary, ref content, null, null);
        }

        [Obsolete("no public api offered yet.")]
        public HttpStatusCode GetRelatedResults(long id, ref string content)
        {
            // official client only api '/1.1/conversation/show.json?id=:id'
            // var apiuri = CreateTwitterUri("conversation", "show", "" + id);
            var apiuri = CreateTwitterUri("related_results", "show", "" + id, true, twitterUrl, "1");
            var param = new Dictionary<string, string> { { "include_entities", "true" } };
            return _httpCon.GetContent(GetMethod, apiuri, param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetBlockUserIds(long cursor, ref string content)
        {
            var param = new Dictionary<string, string> { { "cursor", "" + cursor } };
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("blocks", "ids"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetConfiguration(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("help", "configuration"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode VerifyCredentials(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("account", "verify_credentials"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UserStream(ref Stream content, bool allAtReplies, string trackwords, string userAgent)
        {
            var param = new Dictionary<string, string>();

            if (allAtReplies)
            {
                param.Add("replies", "all");
            }

            if (!string.IsNullOrEmpty(trackwords))
            {
                param.Add("track", trackwords);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUserStreamUri("user"), param, ref content, userAgent);
        }

        public HttpStatusCode FilterStream(ref Stream content, string trackwords, string userAgent)
        {
            // 文中の日本語キーワードに反応せず、使えない（スペースで分かち書きしてないと反応しない）
            var param = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(trackwords))
            {
                param.Add("track", string.Join(",", trackwords.Split(" ".ToCharArray())));
            }

            return _httpCon.GetContent(PostMethod, CreateTwitterStreamUri("statuses", "filter"), param, ref content, userAgent);
        }

        public void RequestAbort()
        {
            _httpCon.RequestAbort();
        }

        public object Clone()
        {
            var myCopy = new HttpTwitter();
            myCopy.Initialize(AccessToken, AccessTokenSecret, AuthenticatedUsername, AuthenticatedUserId);
            return myCopy;
        }

        #region "Proxy API"

        private static Uri CreateTwitterUri(string subject, string verb, string arg = null, bool ssl = true, string host = "api.twitter.com", string version = "1.1")
        {
            var schema = ssl ? "https:" : "http:";
            host = string.IsNullOrEmpty(host) ? "api.twitter.com" : host;
            version = string.IsNullOrEmpty(version) ? "1.1" : version;
            var path = string.Empty;
            if (!string.IsNullOrEmpty(arg))
            {
                path = "/" + arg;
            }
            if (!string.IsNullOrEmpty(verb))
            {
                path = "/" + verb + path;
            }
            path = subject + path + ".json";
            return new Uri(schema + "//" + host + "/" + version + "/" + path);
        }

        private Uri CreateTwitterUri(string path)
        {
            return new Uri(string.Format("{0}{1}{2}", protocol, twitterUrl, path));
        }

        private Uri CreateTwitterUserStreamUri(string path)
        {
            return CreateTwitterUri(path, string.Empty, string.Empty, true, TwitterUserStreamUrl);
        }

        private Uri CreateTwitterStreamUri(string subject, string verb)
        {
            return CreateTwitterUri(subject, verb, null, true, TwitterStreamUrl);
        }

        #endregion

        private void GetApiCallback(object sender, ref HttpStatusCode code, ref string content)
        {
            if (code < HttpStatusCode.InternalServerError)
            {
                MyCommon.TwitterApiInfo.ParseHttpHeaders(MyCommon.TwitterApiInfo.HttpHeaders);
            }
        }
    }
}