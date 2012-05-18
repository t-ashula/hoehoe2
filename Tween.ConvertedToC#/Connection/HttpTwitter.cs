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
using System.Net;

namespace Hoehoe
{
    public class HttpTwitter : ICloneable
    {
        //OAuth関連
        ///<summary>
        ///OAuthのコンシューマー鍵 : TODO
        ///</summary>
        private const string ConsumerKey = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM";
        ///<summary>
        ///OAuthの署名作成用秘密コンシューマーデータ
        ///</summary>
        private const string ConsumerSecret = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM";
        ///<summary>
        ///OAuthのアクセストークン取得先URI
        ///</summary>
        private const string AccessTokenUrlXAuth = "https://api.twitter.com/oauth/access_token";
        private const string RequestTokenUrl = "https://api.twitter.com/oauth/request_token";
        private const string AuthorizeUrl = "https://api.twitter.com/oauth/authorize";
        private const string AccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        private static string _protocol = "http://";
        private const string PostMethod = "POST";
        private const string GetMethod = "GET";

        /// <summary>
        /// HttpConnectionApi or HttpConnectionOAuth
        /// </summary>        
        private IHttpConnection _httpCon;

        private HttpVarious _httpConVar = new HttpVarious();

        private enum AuthMethod
        {
            OAuth,
            Basic
        }

        private AuthMethod _connectionType = AuthMethod.Basic;

        private string _requestToken;

        //for OAuth
        private string _tk = "";
        private string _tks = "";
        private string _un = "";
        public void Initialize(string accessToken, string accessTokenSecret, string username, long userId)
        {
            HttpOAuthApiProxy con = new HttpOAuthApiProxy();
            if (_tk != accessToken || _tks != accessTokenSecret || _un != username || _connectionType != AuthMethod.OAuth)
            {
                // 以前の認証状態よりひとつでも変化があったらhttpヘッダより読み取ったカウントは初期化
                _tk = accessToken;
                _tks = accessTokenSecret;
                _un = username;
            }
            con.Initialize(MyCommon.DecryptString(ConsumerKey), MyCommon.DecryptString(ConsumerSecret), accessToken, accessTokenSecret, username, userId, "screen_name", "user_id");
            _httpCon = con;
            _connectionType = AuthMethod.OAuth;
            _requestToken = "";
        }

        public string AccessToken
        {
            get { return _httpCon != null ? ((HttpConnectionOAuth)_httpCon).AccessToken : ""; }
        }

        public string AccessTokenSecret
        {
            get { return _httpCon != null ? ((HttpConnectionOAuth)_httpCon).AccessTokenSecret : ""; }
        }

        public string AuthenticatedUsername
        {
            get { return _httpCon != null ? _httpCon.AuthUsername : ""; }
        }

        public long AuthenticatedUserId
        {
            get { return _httpCon != null ? _httpCon.AuthUserId : 0; }
            set { if (_httpCon != null) { _httpCon.AuthUserId = value; } }
        }

        public string Password
        {
            get { return ""; }
        }

        public bool AuthGetRequestToken(ref string content)
        {
            Uri authUri = null;
            bool result = ((HttpOAuthApiProxy)_httpCon).AuthenticatePinFlowRequest(RequestTokenUrl, AuthorizeUrl, ref _requestToken, ref authUri);
            content = authUri.ToString();
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
            this.Initialize("", "", "", 0);
        }

        public static void SetUseSsl(bool useSSL)
        {
            _protocol = useSSL ? "https://" : "http://";
        }

        public HttpStatusCode UpdateStatus(string status, long replyToId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("status", status);
            if (replyToId > 0)
            {
                param.Add("in_reply_to_status_id", replyToId.ToString());
            }
            param.Add("include_entities", "true");
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/update.json"), param, ref content, null, null);
        }

        public HttpStatusCode UpdateStatusWithMedia(string status, long replyToId, FileInfo mediaFile, ref string content)
        {
            //画像投稿用エンドポイント
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("status", status);
            if (replyToId > 0)
            {
                param.Add("in_reply_to_status_id", replyToId.ToString());
            }
            param.Add("include_entities", "true");
            List<KeyValuePair<string, FileInfo>> binary = new List<KeyValuePair<string, FileInfo>>();
            binary.Add(new KeyValuePair<string, FileInfo>("media[]", mediaFile));
            return _httpCon.GetContent(PostMethod, new Uri("https://upload.twitter.com/1/statuses/update_with_media.json"), param, binary, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DestroyStatus(long id)
        {
            string t = "";
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/destroy/" + id.ToString() + ".json"), null, ref t, null, null);
        }

        public HttpStatusCode SendDirectMessage(string status, string sendto, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("text", status);
            param.Add("screen_name", sendto);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/direct_messages/new.json"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyDirectMessage(long id)
        {
            string t = "";
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/direct_messages/destroy/" + id.ToString() + ".json"), null, ref t, null, null);
        }

        public HttpStatusCode RetweetStatus(long id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("include_entities", "true");

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/retweet/" + id.ToString() + ".json"), param, ref content, null, null);
        }

        public HttpStatusCode ShowUserInfo(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            param.Add("include_entities", "true");
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/users/show.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateFriendships(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/friendships/create.json"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyFriendships(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/friendships/destroy.json"), param, ref content, null, null);
        }

        public HttpStatusCode CreateBlock(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/blocks/create.json"), param, ref content, null, null);
        }

        public HttpStatusCode DestroyBlock(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/blocks/destroy.json"), param, ref content, null, null);
        }

        public HttpStatusCode ReportSpam(string screenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/report_spam.json"), param, ref content, null, null);
        }

        public HttpStatusCode ShowFriendships(string souceScreenName, string targetScreenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("source_screen_name", souceScreenName);
            param.Add("target_screen_name", targetScreenName);
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/friendships/show.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode ShowStatuses(long id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("include_entities", "true");
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/show/" + id.ToString() + ".json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateFavorites(long id, ref string content)
        {
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/favorites/create/" + id.ToString() + ".json"), null, ref content, null, null);
        }

        public HttpStatusCode DestroyFavorites(long id, ref string content)
        {
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/favorites/destroy/" + id.ToString() + ".json"), null, ref content, null, null);
        }

        public HttpStatusCode HomeTimeline(int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/home_timeline.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UserTimeline(long user_id, string screen_name, int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            if ((user_id == 0 && String.IsNullOrEmpty(screen_name)) || (user_id != 0 && !String.IsNullOrEmpty(screen_name)))
            {
                return HttpStatusCode.BadRequest;
            }

            if (user_id > 0)
            {
                param.Add("user_id", user_id.ToString());
            }
            if (!string.IsNullOrEmpty(screen_name))
            {
                param.Add("screen_name", screen_name);
            }
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            param.Add("include_rts", "true");
            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/user_timeline.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode PublicTimeline(int count, long maxId, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (maxId > 0)
            {
                param.Add("max_id", maxId.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }

            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/public_timeline.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode Mentions(int count, long maxId, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (maxId > 0)
            {
                param.Add("max_id", maxId.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }

            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/mentions.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DirectMessages(int count, long maxId, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (maxId > 0)
            {
                param.Add("max_id", maxId.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }
            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/direct_messages.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode DirectMessagesSent(int count, long maxId, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (maxId > 0)
            {
                param.Add("max_id", maxId.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }
            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/direct_messages/sent.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode Favorites(int count, int page, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count != 20)
                param.Add("count", count.ToString());

            if (page > 0)
            {
                param.Add("page", page.ToString());
            }

            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/favorites.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode PhoenixSearch(string querystr, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string[] tmp = null;
            string[] paramstr = null;
            if (string.IsNullOrEmpty(querystr))
                return HttpStatusCode.BadRequest;

            tmp = querystr.Split(new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tmp2 in tmp)
            {
                paramstr = tmp2.Split(new char[] { '=' });
                param.Add(paramstr[0], paramstr[1]);
            }

            return _httpConVar.GetContent(GetMethod, CreateTwitterUri("/phoenix_search.phoenix"), param, ref content, null, "Hoehoe");
        }

        public HttpStatusCode PhoenixSearch(string words, string lang, int rpp, int page, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(words))
            {
                param.Add("q", words);
            }
            param.Add("include_entities", "1");
            param.Add("contributor_details", "true");
            if (!String.IsNullOrEmpty(lang))
            {
                param.Add("lang", lang);
            }
            if (rpp > 0)
            {
                param.Add("rpp", rpp.ToString());
            }
            if (page > 0)
            {
                param.Add("page", page.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }

            return _httpConVar.GetContent(GetMethod, CreateTwitterUri("/phoenix_search.phoenix"), param, ref content, null, "Hoehoe");
        }

        public HttpStatusCode Search(string words, string lang, int rpp, int page, long sinceId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(words))
            {
                param.Add("q", words);
            }
            if (!string.IsNullOrEmpty(lang))
            {
                param.Add("lang", lang);
            }
            if (rpp > 0)
            {
                param.Add("rpp", rpp.ToString());
            }
            if (page > 0)
            {
                param.Add("page", page.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }

            if (param.Count == 0)
            {
                return HttpStatusCode.BadRequest;
            }

            return _httpConVar.GetContent(GetMethod, CreateTwitterSearchUri("/search.atom"), param, ref content, null, "Hoehoe");
        }

        public HttpStatusCode SavedSearches(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/saved_searches.json"), null, ref content, null, GetApiCallback);
        }

        public HttpStatusCode FollowerIds(long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cursor", cursor.ToString());
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/followers/ids.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode NoRetweetIds(long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cursor", cursor.ToString());
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/friendships/no_retweet_ids.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode RateLimitStatus(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/account/rate_limit_status.json"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        #region "Lists"

        public HttpStatusCode GetLists(string user, long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", user);
            param.Add("cursor", cursor.ToString());
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/lists.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UpdateListID(string user, string listId, string name, bool isPrivate, string description, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", user);
            param.Add("list_id", listId);
            param.Add("mode", isPrivate ? "private" : "public");
            if (name != null)
            {
                param.Add("name", name);
            }
            if (description != null)
            {
                param.Add("description", description);
            }
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/lists/update.json"), param, ref content, null, null);
        }

        public HttpStatusCode DeleteListID(string user, string listId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", user);
            param.Add("list_id", listId);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/lists/destroy.json"), param, ref content, null, null);
        }

        public HttpStatusCode GetListsSubscriptions(string user, long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", user);
            param.Add("cursor", cursor.ToString());
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/lists/subscriptions.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetListsStatuses(long userId, long listId, int perPage, long maxId, long sinceId, bool isRTinclude, ref string content)
        {
            //認証なくても取得できるが、protectedユーザー分が抜ける
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("user_id", userId.ToString());
            param.Add("list_id", listId.ToString());
            if (isRTinclude)
            {
                param.Add("include_rts", "true");
            }
            if (perPage > 0)
            {
                param.Add("per_page", perPage.ToString());
            }
            if (maxId > 0)
            {
                param.Add("max_id", maxId.ToString());
            }
            if (sinceId > 0)
            {
                param.Add("since_id", sinceId.ToString());
            }
            param.Add("include_entities", "true");

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/lists/statuses.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateLists(string listname, bool isPrivate, string description, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("name", listname);
            param.Add("mode", isPrivate ? "private" : "public");
            if (!String.IsNullOrEmpty(description))
            {
                param.Add("description", description);
            }
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/lists/create.json"), param, ref content, null, null);
        }

        public HttpStatusCode GetListMembers(string user, string listId, long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", user);
            param.Add("list_id", listId);
            param.Add("cursor", cursor.ToString());
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/lists/members.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode CreateListMembers(string listId, string memberName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("list_id", listId);
            param.Add("screen_name", memberName);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/lists/members/create.json"), param, ref content, null, null);
        }

        public HttpStatusCode DeleteListMembers(string listId, string memberName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", memberName);
            param.Add("list_id", listId);
            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/lists/members/destroy.json"), param, ref content, null, null);
        }

        public HttpStatusCode ShowListMember(string listId, string memberName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", memberName);
            param.Add("list_id", listId);
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/lists/members/show.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        #endregion "Lists"

        public HttpStatusCode Statusid_retweeted_by_ids(long statusid, int count, int page, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (page > 0)
            {
                param.Add("page", page.ToString());
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/" + statusid.ToString() + "/retweeted_by/ids.json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode UpdateProfile(string name, string url, string location, string description, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("name", name);
            param.Add("url", url);
            param.Add("location", location);
            param.Add("description", description);
            param.Add("include_entities", "true");

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/account/update_profile.json"), param, ref content, null, null);
        }

        public HttpStatusCode UpdateProfileImage(FileInfo imageFile, ref string content)
        {
            List<KeyValuePair<string, FileInfo>> binary = new List<KeyValuePair<string, FileInfo>>();
            binary.Add(new KeyValuePair<string, FileInfo>("image", imageFile));

            return _httpCon.GetContent(PostMethod, CreateTwitterUri("/1/account/update_profile_image.json"), null, binary, ref content, null, null);
        }

        public HttpStatusCode GetRelatedResults(long id, ref string content)
        {
            //認証なくても取得できるが、protectedユーザー分が抜ける
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("include_entities", "true");
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/related_results/show/" + id.ToString() + ".json"), param, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetBlockUserIds(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/blocks/blocking/ids.json"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode GetConfiguration(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/help/configuration.json"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        public HttpStatusCode VerifyCredentials(ref string content)
        {
            return _httpCon.GetContent(GetMethod, CreateTwitterUri("/1/account/verify_credentials.json"), null, ref content, MyCommon.TwitterApiInfo.HttpHeaders, GetApiCallback);
        }

        #region "Proxy API"

        private static string _twitterUrl = "api.twitter.com";
        private static string _TwitterSearchUrl = "search.twitter.com";
        private static string _twitterUserStreamUrl = "userstream.twitter.com";
        private static string _twitterStreamUrl = "stream.twitter.com";

        private Uri CreateTwitterUri(string path)
        {
            return new Uri(String.Format("{0}{1}{2}", _protocol, _twitterUrl, path));
        }

        private Uri CreateTwitterSearchUri(string path)
        {
            return new Uri(String.Format("{0}{1}{2}", _protocol, _TwitterSearchUrl, path));
        }

        private Uri CreateTwitterUserStreamUri(string path)
        {
            return new Uri(String.Format("{0}{1}{2}", "https://", _twitterUserStreamUrl, path));
        }

        private Uri CreateTwitterStreamUri(string path)
        {
            return new Uri(String.Format("{0}{1}{2}", "http://", _twitterStreamUrl, path));
        }

        public static void SetTwitterUrl(string value)
        {
            _twitterUrl = value;
            HttpOAuthApiProxy.SetProxyHost(value);
        }

        public static void SetTwitterSearchUrl(string value)
        {
            _TwitterSearchUrl = value;
        }

        #endregion "Proxy API"

        private void GetApiCallback(object sender, ref HttpStatusCode code, ref string content)
        {
            if (code < HttpStatusCode.InternalServerError)
            {
                MyCommon.TwitterApiInfo.ParseHttpHeaders(MyCommon.TwitterApiInfo.HttpHeaders);
            }
        }

        public HttpStatusCode UserStream(ref Stream content, bool allAtReplies, string trackwords, string userAgent)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            if (allAtReplies)
            {
                param.Add("replies", "all");
            }

            if (!string.IsNullOrEmpty(trackwords))
            {
                param.Add("track", trackwords);
            }

            return _httpCon.GetContent(GetMethod, CreateTwitterUserStreamUri("/2/user.json"), param, ref content, userAgent);
        }

        public HttpStatusCode FilterStream(ref Stream content, string trackwords, string userAgent)
        {
            //文中の日本語キーワードに反応せず、使えない（スペースで分かち書きしてないと反応しない）
            Dictionary<string, string> param = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(trackwords))
            {
                param.Add("track", String.Join(",", trackwords.Split(" ".ToCharArray())));
            }

            return _httpCon.GetContent(PostMethod, CreateTwitterStreamUri("/1/statuses/filter.json"), param, ref content, userAgent);
        }

        public void RequestAbort()
        {
            _httpCon.RequestAbort();
        }

        public object Clone()
        {
            HttpTwitter myCopy = new HttpTwitter();
            myCopy.Initialize(this.AccessToken, this.AccessTokenSecret, this.AuthenticatedUsername, this.AuthenticatedUserId);
            return myCopy;
        }
    }
}