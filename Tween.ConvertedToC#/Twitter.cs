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
    using Hoehoe.DataModels;
    using Hoehoe.DataModels.Twitter;

    public class Twitter : IDisposable
    {
        public const string HashtagRegexPattern = "(" + HashTagBoundary + ")(#|＃)(" + HashtagAlphaNumeric + "*" + HashtagAlpha + HashtagAlphaNumeric + "*)(?=" + HashtagTerminator + "|" + HashTagBoundary + ")";
        public const string UrlRegexPattern = "(?<before>(?:[^\\\"':!=#]|^|\\:/))" + "(?<url>(?<protocol>https?://)" + UrlValidDomain + PTH2 + QRY + ")";

        // Hashtag用正規表現
        private const string LatinAccents = "\\xc0-\\xd6\\xd8-\\xf6\\xf8-\\xff";
        private const string NonLatinHashtagChars = "\\u0400-\\u04ff\\u0500-\\u0527\\u1100-\\u11ff\\u3130-\\u3185\\uA960-\\uA97F\\uAC00-\\uD7AF\\uD7B0-\\uD7FF";
        private const string CjHashtagChars = "\\u30A1-\\u30FA\\u30FC\\u3005\\uFF66-\\uFF9F\\uFF10-\\uFF19\\uFF21-\\uFF3A\\uFF41-\\uFF5A\\u3041-\\u309A\\u3400-\\u4DBF\\p{IsCJKUnifiedIdeographs}";
        private const string HashTagBoundary = "^|$|\\s|「|」|。|\\.|!";
        private const string HashtagAlpha = "[a-z_" + LatinAccents + NonLatinHashtagChars + CjHashtagChars + "]";
        private const string HashtagAlphaNumeric = "[a-z0-9_" + LatinAccents + NonLatinHashtagChars + CjHashtagChars + "]";
        private const string HashtagTerminator = "[^a-z0-9_" + LatinAccents + NonLatinHashtagChars + CjHashtagChars + "]";

        // URL正規表現
        private const string UrlValidDomain = "(?<domain>(?:[^\\p{P}\\s][\\.\\-_](?=[^\\p{P}\\s])|[^\\p{P}\\s]){1,}\\.[a-z]{2,}(?::[0-9]+)?)";
        private const string UrlValidGeneralPathChars = "[a-z0-9!*';:=+$/%#\\[\\]\\-_&,~]";
        private const string UrlBalanceParens = "(?:\\(" + UrlValidGeneralPathChars + "+\\))";
        private const string UrlValidPathEndingChars = "(?:[a-z0-9=_#/\\-\\+]+|" + UrlBalanceParens + ")";
        private const string PTH = "(?:" + UrlBalanceParens + "|@" + UrlValidGeneralPathChars + "+/" + "|[.,]?" + UrlValidGeneralPathChars + "+" + ")";
        private const string PTH2 = "(/(?:" + PTH + "+" + UrlValidPathEndingChars + "|" + PTH + "+" + UrlValidPathEndingChars + "?|" + UrlValidPathEndingChars + ")?)?";
        private const string QRY = "(?<query>\\?[a-z0-9!*'();:&=+$/%#\\[\\]\\-_.,~]*[a-z0-9_&=#])?";

        private static AccountState accountState = AccountState.Valid;

        private readonly object lockObj = new object();
        private List<long> followerIds = new List<long>();
        private bool getFollowerResult;
        private List<long> noRetweetIds = new List<long>();
        private bool getNoRetweetResult;
        private int followersCount;
        private int friendsCount;
        private int statusesCount;
        private string location = string.Empty;
        private string bio = string.Empty;
        private string protocol = "https://";

        // プロパティからアクセスされる共通情報
        private string uname;

        private int iconSz;
        private bool getIcon;
        private IDictionary<string, Image> userIcons;
        private bool tinyUrlResolve;
        private bool restrictFavCheck;
        private bool readOwnPost;

        private List<string> hashList = new List<string>();

        private Outputz outputz = new Outputz();

        private long minHomeTimeline = long.MaxValue; // max_idで古い発言を取得するために保持（lists分は個別タブで管理）
        private long minMentions = long.MaxValue;
        private long minDirectmessage = long.MaxValue;
        private long minDirectmessageSent = long.MaxValue;

        private HttpTwitter twitterConnection = new HttpTwitter();
        
        private PostInfo prevPostInfo = new PostInfo(string.Empty, string.Empty, string.Empty, string.Empty);

        private int prevFavPage = 1;
        private DateTime lastUserstreamDataReceived;
        private TwitterUserstream withEventsFielduserStream;

        private EventTypeTableElement[] eventsTable = {
            new EventTypeTableElement("favorite", EventType.Favorite),
            new EventTypeTableElement("unfavorite", EventType.Unfavorite),
            new EventTypeTableElement("follow", EventType.Follow),
            new EventTypeTableElement("list_member_added", EventType.ListMemberAdded),
            new EventTypeTableElement("list_member_removed", EventType.ListMemberRemoved),
            new EventTypeTableElement("block", EventType.Block),
            new EventTypeTableElement("unblock", EventType.Unblock),
            new EventTypeTableElement("user_update", EventType.UserUpdate),
            new EventTypeTableElement("deleted", EventType.Deleted),
            new EventTypeTableElement("list_created", EventType.ListCreated),
            new EventTypeTableElement("list_updated", EventType.ListUpdated)
        };

        // 重複する呼び出しを検出するには
        private bool disposedValue;

        public Twitter()
        {
            this.ApiInformationChanged += this.Twitter_ApiInformationChanged;
        }

        public delegate void GetIconImageDelegate(PostClass post);
        public delegate void UserIdChangedEventHandler();
        public delegate void ApiInformationChangedEventHandler(object sender, ApiInformationChangedEventArgs e);
        public delegate void NewPostFromStreamEventHandler();

        public delegate void UserStreamStartedEventHandler();

        public delegate void UserStreamStoppedEventHandler();
        public delegate void UserStreamGetFriendsListEventHandler();
        public delegate void PostDeletedEventHandler(long id);

        public delegate void UserStreamEventReceivedEventHandler(FormattedEvent eventType);

        public event UserIdChangedEventHandler UserIdChanged;
        public event ApiInformationChangedEventHandler ApiInformationChanged;

        public event NewPostFromStreamEventHandler NewPostFromStream;

        public event UserStreamStartedEventHandler UserStreamStarted;

        public event UserStreamStoppedEventHandler UserStreamStopped;


        public event UserStreamGetFriendsListEventHandler UserStreamGetFriendsList;

        public event PostDeletedEventHandler PostDeleted;

        public event UserStreamEventReceivedEventHandler UserStreamEventReceived;

        public static AccountState AccountState
        {
            get { return accountState; }
            set { accountState = value; }
        }

        public string Username
        {
            get { return this.twitterConnection.AuthenticatedUsername; }
        }

        public long UserId
        {
            get { return this.twitterConnection.AuthenticatedUserId; }
        }

        public string Password
        {
            get { return this.twitterConnection.Password; }
        }

        public IDictionary<string, Image> DetailIcon
        {
            get { return this.userIcons; }
            set { this.userIcons = value; }
        }

        public bool ReadOwnPost
        {
            get { return this.readOwnPost; }
            set { this.readOwnPost = value; }
        }

        public int FollowersCount
        {
            get { return this.followersCount; }
        }

        public int FriendsCount
        {
            get { return this.friendsCount; }
        }

        public int StatusesCount
        {
            get { return this.statusesCount; }
        }

        public string Location
        {
            get { return this.location; }
        }

        public string Bio
        {
            get { return this.bio; }
        }

        public bool GetFollowersSuccess
        {
            get { return this.getFollowerResult; }
        }

        public bool GetNoRetweetSuccess
        {
            get { return this.getNoRetweetResult; }
        }

        public string AccessToken
        {
            get { return this.twitterConnection.AccessToken; }
        }

        public string AccessTokenSecret
        {
            get { return this.twitterConnection.AccessTokenSecret; }
        }

        public string TrackWord { get; set; }

        public bool AllAtReply { get; set; }

        public List<FormattedEvent> StoredEvent { get; set; }

        public bool IsUserstreamDataReceived
        {
            get { return DateTime.Now.Subtract(this.lastUserstreamDataReceived).TotalSeconds < 31; }
        }

        public bool UserStreamEnabled
        {
            get { return this.UserStream == null ? false : this.UserStream.Enabled; }
        }

        private TwitterUserstream UserStream
        {
            get { return this.withEventsFielduserStream; }
            set
            {
                if (this.withEventsFielduserStream != null)
                {
                    this.withEventsFielduserStream.StatusArrived -= this.UserStream_StatusArrived;
                    this.withEventsFielduserStream.Started -= this.UserStream_Started;
                    this.withEventsFielduserStream.Stopped -= this.UserStream_Stopped;
                }
                this.withEventsFielduserStream = value;
                if (this.withEventsFielduserStream != null)
                {
                    this.withEventsFielduserStream.StatusArrived += this.UserStream_StatusArrived;
                    this.withEventsFielduserStream.Started += this.UserStream_Started;
                    this.withEventsFielduserStream.Stopped += this.UserStream_Stopped;
                }
            }
        }

        public string Authenticate(string username, string password)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = this.twitterConnection.AuthUserAndPass(username, password, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    this.uname = username.ToLower();
                    if (AppendSettingDialog.Instance.UserstreamStartup)
                    {
                        this.ReconnectUserStream();
                    }
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = AccountState.Invalid;
                        string errMsg = this.GetErrorMessageJson(content);
                        if (string.IsNullOrEmpty(errMsg))
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
                        string errMsg = this.GetErrorMessageJson(content);
                        if (string.IsNullOrEmpty(errMsg))
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
            // OAuth PIN Flow
            bool res = false;
            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = this.twitterConnection.AuthGetRequestToken(ref pinPageUrl);
            }
            catch (Exception)
            {
                return "Err:" + "Failed to access auth server.";
            }

            return string.Empty;
        }

        public string Authenticate(string pinCode)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;

            MyCommon.TwitterApiInfo.Initialize();
            try
            {
                res = this.twitterConnection.AuthGetAccessToken(pinCode);
            }
            catch (Exception)
            {
                return "Err:" + "Failed to access auth acc server.";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    this.uname = this.Username.ToLower();
                    if (AppendSettingDialog.Instance.UserstreamStartup)
                    {
                        this.ReconnectUserStream();
                    }
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = AccountState.Invalid;
                        string errMsg = this.GetErrorMessageJson(content);
                        if (string.IsNullOrEmpty(errMsg))
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
                        string errMsg = this.GetErrorMessageJson(content);
                        if (string.IsNullOrEmpty(errMsg))
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
            Twitter.AccountState = AccountState.Invalid;
            MyCommon.TwitterApiInfo.Initialize();
            this.twitterConnection.ClearAuthInfo();
        }

        public void VerifyCredentials()
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.VerifyCredentials(ref content);
            }
            catch (Exception)
            {
                return;
            }

            if (res == HttpStatusCode.OK)
            {
                Twitter.AccountState = AccountState.Valid;
                User user = null;
                try
                {
                    user = D.CreateDataFromJson<User>(content);
                }
                catch (SerializationException)
                {
                    return;
                }
                this.twitterConnection.AuthenticatedUserId = user.Id;
            }
        }

        public void Initialize(string token, string tokenSecret, string username, long userId)
        {
            // OAuth認証
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret) || string.IsNullOrEmpty(username))
            {
                Twitter.AccountState = AccountState.Invalid;
            }

            MyCommon.TwitterApiInfo.Initialize();
            this.twitterConnection.Initialize(token, tokenSecret, username, userId);
            this.uname = username.ToLower();
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
                    string urlStr = string.Empty;
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

        public string PostStatus(string postStr, long replyToId)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            postStr = postStr.Trim();

            if (Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
            {
                return this.SendDirectMessage(postStr);
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.UpdateStatus(postStr, replyToId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    Status status = null;
                    try
                    {
                        status = D.CreateDataFromJson<Status>(content);
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

                    this.followersCount = status.User.FollowersCount;
                    this.friendsCount = status.User.FriendsCount;
                    this.statusesCount = status.User.StatusesCount;
                    this.location = status.User.Location;
                    this.bio = status.User.Description;

                    if (this.IsPostRestricted(status))
                    {
                        return "OK:Delaying?";
                    }

                    return this.outputz.Post(postStr.Length) ? string.Empty : "Outputz:Failed";
                case HttpStatusCode.NotFound:
                    return string.Empty;
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
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
                    // 仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = AccountState.Invalid;
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostStatusWithMedia(string postStr, long replyToId, FileInfo mediaFile)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            postStr = postStr.Trim();

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.UpdateStatusWithMedia(postStr, replyToId, mediaFile, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    Status status = null;
                    try
                    {
                        status = D.CreateDataFromJson<Status>(content);
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

                    this.followersCount = status.User.FollowersCount;
                    this.friendsCount = status.User.FriendsCount;
                    this.statusesCount = status.User.StatusesCount;
                    this.location = status.User.Location;
                    this.bio = status.User.Description;

                    if (this.IsPostRestricted(status))
                    {
                        return "OK:Delaying?";
                    }

                    return this.outputz.Post(postStr.Length) ? string.Empty : "Outputz:Failed";
                case HttpStatusCode.NotFound:
                    return string.Empty;
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
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
                    // 仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = AccountState.Invalid;
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string SendDirectMessage(string postStr)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
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
            string content = string.Empty;
            Match mc = Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            try
            {
                res = this.twitterConnection.SendDirectMessage(mc.Groups["body"].Value, mc.Groups["id"].Value, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    Directmessage status = null;
                    try
                    {
                        status = D.CreateDataFromJson<Directmessage>(content);
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

                    this.followersCount = status.Sender.FollowersCount;
                    this.friendsCount = status.Sender.FriendsCount;
                    this.statusesCount = status.Sender.StatusesCount;
                    this.location = status.Sender.Location;
                    this.bio = status.Sender.Description;
                    return this.outputz.Post(postStr.Length) ? string.Empty : "Outputz:Failed";
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.BadRequest:
                    {
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? "Warn:" + res.ToString() : "Warn:" + errMsg;
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
                    // 仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                    return "Warn:" + res.ToString();
                case HttpStatusCode.Unauthorized:
                    {
                        Twitter.AccountState = AccountState.Invalid;
                        string errMsg = this.GetErrorMessageJson(content);
                        return string.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string RemoveStatus(long id)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = this.twitterConnection.DestroyStatus(id);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.NotFound:
                    return string.Empty;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRetweet(long id, bool read)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            // データ部分の生成
            long target = id;
            PostClass post = TabInformations.GetInstance().Item(id);
            if (post == null)
            {
                return "Err:Target isn't found.";
            }
            if (TabInformations.GetInstance().Item(id).RetweetedId > 0)
            {
                // 再RTの場合は元発言をRT
                target = TabInformations.GetInstance().Item(id).RetweetedId;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.RetweetStatus(target, ref content);
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

            Twitter.AccountState = AccountState.Valid;
            Status status = null;
            try
            {
                status = D.CreateDataFromJson<Status>(content);
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

            // ReTweetしたものをTLに追加
            post = this.CreatePostsFromStatusData(status);
            if (post == null)
            {
                return "Invalid Json!";
            }

            // 二重取得回避
            lock (this.lockObj)
            {
                if (TabInformations.GetInstance().ContainsKey(post.StatusId))
                {
                    return string.Empty;
                }
            }
            // Retweet判定
            if (post.RetweetedId == 0)
            {
                return "Invalid Json!";
            }

            // ユーザー情報
            post.IsMe = true;
            post.IsRead = read;
            post.IsOwl = false;
            if (this.readOwnPost)
            {
                post.IsRead = true;
            }
            post.IsDm = false;

            TabInformations.GetInstance().AddPost(post);

            return string.Empty;
        }

        public string RemoveDirectMessage(long id, PostClass post)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
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
                res = this.twitterConnection.DestroyDirectMessage(id);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.NotFound:
                    return string.Empty;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostFollowCommand(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.CreateFriendships(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRemoveCommand(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.DestroyFriendships(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostCreateBlock(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.CreateBlock(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostDestroyBlock(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.DestroyBlock(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostReportSpam(string screenName)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.ReportSpam(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetFriendshipInfo(string screenName, ref bool isFollowing, ref bool isFollowed)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.ShowFriendships(this.uname, screenName, ref content);
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
                        var relation = D.CreateDataFromJson<Relationship>(content);
                        isFollowing = relation.Relationship_.Source.Following;
                        isFollowed = relation.Relationship_.Source.FollowedBy;
                        return string.Empty;
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
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetUserInfo(string screenName, ref User user)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            user = null;
            try
            {
                res = this.twitterConnection.ShowUserInfo(screenName, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    try
                    {
                        user = D.CreateDataFromJson<User>(content);
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
                    return string.Empty;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? Hoehoe.Properties.Resources.Unauthorized : "Auth err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string GetStatus_Retweeted_Count(long statusId, ref int retweetedCount)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            retweetedCount = 0;
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    res = this.twitterConnection.Statusid_retweeted_by_ids(statusId, 100, i, ref content);
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
                            long[] ids = D.CreateDataFromJson<long[]>(content);
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
                        Twitter.AccountState = AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    default:
                        retweetedCount = -1;
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }
            }
            return string.Empty;
        }

        public string PostFavAdd(long id)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.CreateFavorites(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    if (!this.restrictFavCheck)
                    {
                        return string.Empty;
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            // http://twitter.com/statuses/show/id.xml APIを発行して本文を取得
            content = string.Empty;
            try
            {
                res = this.twitterConnection.ShowStatuses(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    Status status = null;
                    try
                    {
                        status = D.CreateDataFromJson<Status>(content);
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
                    return status.Favorited ? string.Empty : "NG(Restricted?)";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
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
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.DestroyFavorites(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfile(string name, string url, string location, string description)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.UpdateProfile(name, url, location, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfileImage(string filename)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.UpdateProfileImage(new FileInfo(filename), ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    return string.Empty;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    string errMsg = this.GetErrorMessageJson(content);
                    return string.IsNullOrEmpty(errMsg) ? "Err:Forbidden(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")" : "Err:" + errMsg;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
        }

        public void SetGetIcon(bool value)
        {
            this.getIcon = value;
        }

        public void SetTinyUrlResolve(bool value)
        {
            this.tinyUrlResolve = value;
        }

        public void SetRestrictFavCheck(bool value)
        {
            this.restrictFavCheck = value;
        }

        public void SetIconSize(int value)
        {
            this.iconSz = value;
        }

        #region "TODO:バージョンアップ"

        public string GetVersionInfo()
        {
            string content = string.Empty;
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
                // 本体
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenNew.exe")))
                {
                    return "Err:Download failed";
                }
                // 英語リソース
                if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, "en")))
                {
                    Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, "en"));
                }
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenResEn" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, "en"), "Tween.resourcesNew.dll")))
                {
                    return "Err:Download failed";
                }
                // その他言語圏のリソース。取得失敗しても継続
                // UIの言語圏のリソース
                string curCul = string.Empty;
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
                if (!string.IsNullOrEmpty(curCul) && curCul != "en" && curCul != "ja")
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, curCul)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, curCul));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, curCul), "Tween.resourcesNew.dll")))
                    {
                        // Return "Err:Download failed"
                    }
                }
                // スレッドの言語圏のリソース
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
                if (!string.IsNullOrEmpty(curCul2) && curCul2 != "en" && curCul2 != curCul)
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.SettingPath, curCul2)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.SettingPath, curCul2));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul2 + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(Path.Combine(MyCommon.SettingPath, curCul2), "Tween.resourcesNew.dll")))
                    {
                        // Return "Err:Download failed"
                    }
                }

                // アップデータ
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenUp3.gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenUp3.exe")))
                {
                    return "Err:Download failed";
                }
                // シリアライザDLL
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Path.Combine(MyCommon.SettingPath, "TweenNew.XmlSerializers.dll")))
                {
                    return "Err:Download failed";
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return "Err:Download failed";
            }
        }

        #endregion "TODO:バージョンアップ"

        public void SetUseSsl(bool value)
        {
            HttpTwitter.SetUseSsl(value);
            this.protocol = value ? "https://" : "http://";
        }

        public string GetTimelineApi(bool read, WorkerType workerType, bool more, bool startup)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            int count = AppendSettingDialog.Instance.CountApi;
            if (workerType == WorkerType.Reply)
            {
                count = AppendSettingDialog.Instance.CountApiReply;
            }
            if (AppendSettingDialog.Instance.UseAdditionalCount)
            {
                if (more && AppendSettingDialog.Instance.MoreCountApi != 0)
                {
                    count = AppendSettingDialog.Instance.MoreCountApi;
                }
                else if (startup && AppendSettingDialog.Instance.FirstCountApi != 0 && workerType == WorkerType.Timeline)
                {
                    count = AppendSettingDialog.Instance.FirstCountApi;
                }
            }
            try
            {
                if (workerType == WorkerType.Timeline)
                {
                    if (more)
                    {
                        res = this.twitterConnection.HomeTimeline(count, this.minHomeTimeline, 0, ref content);
                    }
                    else
                    {
                        res = this.twitterConnection.HomeTimeline(count, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = this.twitterConnection.Mentions(count, this.minMentions, 0, ref content);
                    }
                    else
                    {
                        res = this.twitterConnection.Mentions(count, 0, 0, ref content);
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
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return workerType == WorkerType.Timeline ?
                this.CreatePostsFromJson(content, workerType, null, read, count, ref this.minHomeTimeline) :
                this.CreatePostsFromJson(content, workerType, null, read, count, ref this.minMentions);
        }

        public string GetUserTimelineApi(bool read, int count, string userName, TabClass tab, bool more)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            if (count == 0)
            {
                count = 20;
            }
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    string target = tab.User;
                    if (string.IsNullOrEmpty(target))
                    {
                        return string.Empty;
                    }
                    userName = target;
                    res = this.twitterConnection.UserTimeline(0, target, count, 0, 0, ref content);
                }
                else
                {
                    if (more)
                    {
                        res = this.twitterConnection.UserTimeline(0, userName, count, tab.OldestId, 0, ref content);
                    }
                    else
                    {
                        res = this.twitterConnection.UserTimeline(0, userName, count, 0, 0, ref content);
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
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Valid;
                    return "Err:@" + userName + "'s Tweets are protected.";
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            List<Status> items = null;
            try
            {
                items = D.CreateDataFromJson<List<Status>>(content);
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

            foreach (var status in items)
            {
                PostClass item = this.CreatePostsFromStatusData(status);
                if (item == null)
                {
                    continue;
                }
                if (item.StatusId < tab.OldestId)
                {
                    tab.OldestId = item.StatusId;
                }
                item.IsRead = read;
                if (item.IsMe && !read && this.readOwnPost)
                {
                    item.IsRead = true;
                }
                if (tab != null)
                {
                    item.RelTabName = tab.TabName;
                }
                // 非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(item);
            }

            return string.Empty;
        }

        public string GetStatusApi(bool read, long id, ref PostClass post)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.ShowStatuses(id, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Forbidden:
                    return "Err:Protected user's tweet";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            Status status = null;
            try
            {
                status = D.CreateDataFromJson<Status>(content);
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

            PostClass item = this.CreatePostsFromStatusData(status);
            if (item == null)
            {
                return "Err:Can't create post";
            }
            item.IsRead = read;
            if (item.IsMe && !read && this.readOwnPost)
            {
                item.IsRead = true;
            }
            post = item;
            return string.Empty;
        }

        public string GetStatusApi(bool read, long id, TabClass tab)
        {
            PostClass post = null;
            string r = this.GetStatusApi(read, id, ref post);
            if (string.IsNullOrEmpty(r))
            {
                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                // 非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }
            return r;
        }

        public string GetListStatus(bool read, TabClass tab, bool more, bool startup)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
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
                    res = this.twitterConnection.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, tab.OldestId, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
                else
                {
                    res = this.twitterConnection.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, 0, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            var t = tab.OldestId;
            var ret = this.CreatePostsFromJson(content, WorkerType.List, tab, read, count, ref t);
            tab.OldestId = t;
            return ret;
        }

        public string GetRelatedResult(bool read, TabClass tab)
        {
            string rslt = string.Empty;
            List<PostClass> relPosts = new List<PostClass>();
            if (tab.RelationTargetPost.TextFromApi.Contains("@") && tab.RelationTargetPost.InReplyToStatusId == 0)
            {
                // 検索結果対応
                PostClass p = TabInformations.GetInstance().Item(tab.RelationTargetPost.StatusId);
                if (p != null && p.InReplyToStatusId > 0)
                {
                    tab.RelationTargetPost = p;
                }
                else
                {
                    rslt = this.GetStatusApi(read, tab.RelationTargetPost.StatusId, ref p);
                    if (!string.IsNullOrEmpty(rslt))
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
                if (!string.IsNullOrEmpty(rslt))
                {
                    break;
                }
                tmpPost = this.CheckReplyToPost(relPosts);
            } while (tmpPost != null);
            relPosts.ForEach(p => TabInformations.GetInstance().AddPost(p));
            return rslt;
        }

        public string GetSearch(bool read, TabClass tab, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
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
                res = this.twitterConnection.Search(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
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
                    // API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab))
            {
                return string.Empty;
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
                    // 本文
                    post.TextFromApi = xentry["title"].InnerText;
                    // Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xentry["twitter:source"].InnerText;
                    post.InReplyToStatusId = 0;
                    post.InReplyToUser = string.Empty;
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

                    // 以下、ユーザー情報
                    XmlElement author = (XmlElement)xentry.SelectSingleNode("./search:author", nsmgr);
                    post.UserId = 0;
                    post.ScreenName = author["name"].InnerText.Split(' ')[0].Trim();
                    post.Nickname = author["name"].InnerText.Substring(post.ScreenName.Length).Trim();
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
                    post.IsMe = post.ScreenName.ToLower().Equals(this.uname);
                    // HTMLに整形
                    post.Text = this.CreateHtmlAnchor(HttpUtility.HtmlEncode(post.TextFromApi), post.ReplyToList, post.Media);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    // Source整形
                    this.CreateSource(ref post);
                    post.IsRead = read;
                    post.IsReply = post.ReplyToList.Contains(this.uname);
                    post.IsExcludeReply = false;
                    post.IsOwl = false;
                    if (post.IsMe && !read && this.readOwnPost)
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
            return string.Empty;
        }

        public string GetPhoenixSearch(bool read, TabClass tab, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            int page = 0;
            long sinceId = 0;
            int count = 100;
            string querystr = string.Empty;
            if (AppendSettingDialog.Instance.UseAdditionalCount && AppendSettingDialog.Instance.SearchCountApi != 0)
            {
                count = AppendSettingDialog.Instance.SearchCountApi;
            }
            if (more)
            {
                page = tab.GetSearchPage(count);
                if (!string.IsNullOrEmpty(tab.NextPageQuery))
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
                if (string.IsNullOrEmpty(querystr))
                {
                    res = this.twitterConnection.PhoenixSearch(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
                }
                else
                {
                    res = this.twitterConnection.PhoenixSearch(querystr, ref content);
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
                    // API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab))
            {
                return string.Empty;
            }

            var oid = tab.OldestId;
            var npq = tab.NextPageQuery;
            var ret = this.CreatePostsFromPhoenixSearch(content, WorkerType.PublicSearch, tab, read, count, ref oid, ref npq);
            tab.OldestId = oid;
            tab.NextPageQuery = npq;
            return ret;
        }

        public string GetDirectMessageApi(bool read, WorkerType workerType, bool more)
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.TwitterApiInfo.AccessLevel != ApiAccessLevel.None)
            {
                if (!MyCommon.TwitterApiInfo.IsDirectMessagePermission)
                {
                    return "Auth Err:try to re-authorization.";
                }
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                if (workerType == WorkerType.DirectMessegeRcv)
                {
                    if (more)
                    {
                        res = this.twitterConnection.DirectMessages(20, this.minDirectmessage, 0, ref content);
                    }
                    else
                    {
                        res = this.twitterConnection.DirectMessages(20, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = this.twitterConnection.DirectMessagesSent(20, this.minDirectmessageSent, 0, ref content);
                    }
                    else
                    {
                        res = this.twitterConnection.DirectMessagesSent(20, 0, 0, ref content);
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
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return this.CreateDirectMessagesFromJson(content, workerType, read);
        }

        public string GetFavoritesApi(bool read, WorkerType workerType, bool more)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            int count = AppendSettingDialog.Instance.CountApi;
            if (AppendSettingDialog.Instance.UseAdditionalCount && AppendSettingDialog.Instance.FavoritesCountApi != 0)
            {
                count = AppendSettingDialog.Instance.FavoritesCountApi;
            }

            // 前ページ取得の場合はページカウンタをインクリメント、それ以外の場合はページカウンタリセット
            if (more)
            {
                this.prevFavPage += 1;
            }
            else
            {
                this.prevFavPage = 1;
            }

            try
            {
                res = this.twitterConnection.Favorites(count, this.prevFavPage, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Status>));
            List<Status> item = null;
            try
            {
                item = D.CreateDataFromJson<List<Status>>(content);
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

            foreach (var status in item)
            {
                PostClass post = new PostClass();
                Entities entities = null;

                try
                {
                    post.StatusId = status.Id;
                    // 二重取得回避
                    lock (this.lockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).Contains(post.StatusId))
                        {
                            continue;
                        }
                    }
                    // Retweet判定
                    if (status.RetweetedStatus != null)
                    {
                        var retweeted = status.RetweetedStatus;
                        post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);
                        // Id
                        post.RetweetedId = post.StatusId;
                        // 本文
                        post.TextFromApi = retweeted.Text;
                        entities = retweeted.Entities;
                        // Source取得（htmlの場合は、中身を取り出し）
                        post.Source = retweeted.Source;
                        // Reply先
                        {
                            long t;
                            long.TryParse(retweeted.InReplyToStatusId, out t);
                            post.InReplyToStatusId = t;
                        }
                        post.InReplyToUser = retweeted.InReplyToScreenName;
                        {
                            long t;
                            long.TryParse(retweeted.InReplyToUserId, out t);
                            post.InReplyToUserId = t;
                        }
                        post.IsFav = true;
                        // 以下、ユーザー情報
                        var user = retweeted.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrl;
                        post.IsProtect = user.Protected;
                        // Retweetした人
                        post.RetweetedBy = status.User.ScreenName;
                        post.IsMe = post.RetweetedBy.ToLower().Equals(this.uname);
                    }
                    else
                    {
                        post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                        // 本文
                        post.TextFromApi = status.Text;
                        entities = status.Entities;
                        // Source取得（htmlの場合は、中身を取り出し）
                        post.Source = status.Source;
                        {
                            long t;
                            long.TryParse(status.InReplyToStatusId, out t);
                            post.InReplyToStatusId = t;
                        }
                        post.InReplyToUser = status.InReplyToScreenName;
                        {
                            long t;
                            long.TryParse(status.InReplyToUserId, out t);
                            post.InReplyToUserId = t;
                        }

                        post.IsFav = true;
                        // 以下、ユーザー情報
                        var user = status.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrl;
                        post.IsProtect = user.Protected;
                        post.IsMe = post.ScreenName.ToLower().Equals(this.uname);
                    }
                    // HTMLに整形
                    {
                        var t = post.TextFromApi;
                        post.Text = this.CreateHtmlAnchor(ref t, post.ReplyToList, entities, post.Media);
                        post.TextFromApi = t;
                    }
                    post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
                    // Source整形
                    this.CreateSource(ref post);
                    post.IsRead = read;
                    post.IsReply = post.ReplyToList.Contains(this.uname);
                    post.IsExcludeReply = false;

                    if (post.IsMe)
                    {
                        post.IsOwl = false;
                    }
                    else
                    {
                        if (this.followerIds.Count > 0)
                        {
                            post.IsOwl = !this.followerIds.Contains(post.UserId);
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

            return string.Empty;
        }
        public string GetFollowersApi()
        {
            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            long cursor = -1;
            List<long> tmpFollower = new List<long>(this.followerIds);
            this.followerIds.Clear();
            do
            {
                string ret = this.FollowerApi(ref cursor);
                if (!string.IsNullOrEmpty(ret))
                {
                    this.followerIds.Clear();
                    this.followerIds.AddRange(tmpFollower);
                    this.getFollowerResult = false;
                    return ret;
                }
            } while (cursor > 0);

            TabInformations.GetInstance().RefreshOwl(this.followerIds);

            this.getFollowerResult = true;
            return string.Empty;
        }

        public string GetNoRetweetIdsApi()
        {
            string newVariable = string.Empty;
            if (MyCommon.IsEnding)
            {
                return newVariable;
            }

            long cursor = -1;
            List<long> tmpIds = new List<long>(this.noRetweetIds);
            this.noRetweetIds.Clear();
            do
            {
                string ret = this.NoRetweetApi(ref cursor);
                if (!string.IsNullOrEmpty(ret))
                {
                    this.noRetweetIds.Clear();
                    this.noRetweetIds.AddRange(tmpIds);
                    this.getNoRetweetResult = false;
                    return ret;
                }
            } while (cursor > 0);

            this.getNoRetweetResult = true;
            return newVariable;
        }

        public string ConfigurationApi()
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.GetConfiguration(ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                AppendSettingDialog.Instance.TwitterConfiguration = D.CreateDataFromJson<Configuration>(content);
                return string.Empty;
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
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            long cursor = -1;
            List<ListElement> lists = new List<ListElement>();
            do
            {
                try
                {
                    res = this.twitterConnection.GetLists(this.Username, cursor, ref content);
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = AccountState.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = D.CreateDataFromJson<Lists>(content);
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
            content = string.Empty;
            do
            {
                try
                {
                    res = this.twitterConnection.GetListsSubscriptions(this.Username, cursor, ref content);
                }
                catch (Exception ex)
                {
                    return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = AccountState.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = AccountState.Invalid;
                        return Hoehoe.Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = D.CreateDataFromJson<Lists>(content);
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
            return string.Empty;
        }

        public string DeleteList(string listId)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;

            try
            {
                res = this.twitterConnection.DeleteListID(this.Username, listId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return string.Empty;
        }

        public string EditList(string listId, string newName, bool isPrivate, string description, ref ListElement list)
        {
            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;

            try
            {
                res = this.twitterConnection.UpdateListID(this.Username, listId, newName, isPrivate, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = D.CreateDataFromJson<ListElementData>(content);
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
                return string.Empty;
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
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.GetListMembers(this.Username, listId, cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var users = D.CreateDataFromJson<Users>(content);
                Array.ForEach<User>(users.users, new Action<User>(u => { lists.Add(new UserInfo(u)); }));
                cursor = users.NextCursor;
                return string.Empty;
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
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.CreateLists(listName, isPrivate, description, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = D.CreateDataFromJson<ListElementData>(content);
                TabInformations.GetInstance().SubscribableLists.Add(new ListElement(le, this));
                return string.Empty;
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

            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.ShowListMember(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.NotFound:
                    value = false;
                    return string.Empty;
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var u = D.CreateDataFromJson<User>(content);
                value = true;
                return string.Empty;
            }
            catch (Exception)
            {
                value = false;
                return string.Empty;
            }
        }

        public string AddUserToList(string listId, string user)
        {
            string content = string.Empty;
            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = this.twitterConnection.CreateListMembers(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + this.GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            return string.Empty;
        }

        public string RemoveUserToList(string listId, string user)
        {
            string content = string.Empty;
            HttpStatusCode res = default(HttpStatusCode);
            try
            {
                res = this.twitterConnection.DeleteListMembers(listId, user, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + this.GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }
            return string.Empty;
        }

        public string CreateHtmlAnchor(string text, List<string> replyToIds, Dictionary<string, string> media)
        {
            if (text == null)
            {
                return null;
            }
            string retStr = text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>");
            // 絶対パス表現のUriをリンクに置換
            MatchEvaluator mev = mu =>
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
            };
            retStr = Regex.Replace(retStr, UrlRegexPattern, mev, RegexOptions.IgnoreCase);

            // @先をリンクに置換（リスト）
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\\p{IsLatin-1Supplement}\\-]{0,79})", "$1$2<a href=\"/$3\">$3</a>");
            Match m = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})");
            while (m.Success)
            {
                if (!replyToIds.Contains(m.Result("$2").ToLower()))
                {
                    replyToIds.Add(m.Result("$2").ToLower());
                }
                m = m.NextMatch();
            }
            // @先をリンクに置換
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})", "$1$2<a href=\"/$3\">$3</a>");

            // ハッシュタグを抽出し、リンクに置換
            List<Range> anchorRange = new List<Range>();
            for (int i = 0; i < retStr.Length; i++)
            {
                int index = retStr.IndexOf("<a ", i);
                if (index > -1 && index < retStr.Length)
                {
                    i = index;
                    int toIndex = retStr.IndexOf("</a>", index);
                    if (toIndex > -1)
                    {
                        anchorRange.Add(new Range(index, toIndex + 3));
                        i = toIndex;
                    }
                }
            }
            MatchEvaluator hashReplace = mh =>
            {
                foreach (Range rng in anchorRange)
                {
                    if (mh.Index >= rng.FromIndex && mh.Index <= rng.ToIndex)
                    {
                        return mh.Result("$0");
                    }
                }
                lock (lockObj)
                {
                    hashList.Add("#" + mh.Result("$3"));
                }
                return mh.Result("$1") + "<a href=\"" + protocol + "twitter.com/search?q=%23" + mh.Result("$3") + "\">" + mh.Result("$2$3") + "</a>";
            };
            retStr = Regex.Replace(retStr, HashtagRegexPattern, hashReplace, RegexOptions.IgnoreCase);
            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;");
            retStr = this.AdjustHtml(this.PreProcessUrl(retStr)); // IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return retStr;
        }

        public string CreateHtmlAnchor(ref string text, List<string> replyToIds, Entities entities, Dictionary<string, string> media)
        {
            string ret = text;

            if (entities != null)
            {
                var entityInfos = new SortedList<int, EntityInfo>();
                // URL
                if (entities.Urls != null)
                {
                    foreach (var ent in entities.Urls)
                    {
                        if (string.IsNullOrEmpty(ent.DisplayUrl))
                        {
                            var tmpEntity = new EntityInfo()
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = string.Format("<a href=\"{0}\">{0}</a>", ent.Url)
                            };
                            entityInfos.Add(ent.Indices[0], tmpEntity);
                        }
                        else
                        {
                            string expanded = ShortUrl.ResolveMedia(ent.ExpandedUrl, false);
                            var tmp = new EntityInfo()
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = string.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", ent.Url, expanded, ent.DisplayUrl),
                                Display = ent.DisplayUrl
                            };
                            entityInfos.Add(ent.Indices[0], tmp);
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
                        var tmp = new EntityInfo()
                        {
                            StartIndex = ent.Indices[0],
                            EndIndex = ent.Indices[1],
                            Text = hash,
                            Html = string.Format("<a href=\"{0}twitter.com/search?q=%23{1}\">{2}</a>", this.protocol, ent.Text, hash)
                        };
                        entityInfos.Add(ent.Indices[0], tmp);
                        lock (this.lockObj)
                        {
                            this.hashList.Add("#" + ent.Text);
                        }
                    }
                }

                if (entities.UserMentions != null)
                {
                    foreach (var ent in entities.UserMentions)
                    {
                        string screenName = text.Substring(ent.Indices[0] + 1, ent.Indices[1] - ent.Indices[0] - 1);
                        var tmp = new EntityInfo()
                        {
                            StartIndex = ent.Indices[0] + 1,
                            EndIndex = ent.Indices[1],
                            Text = ent.ScreenName,
                            Html = string.Format("<a href=\"/{0}\">{1}</a>", ent.ScreenName, screenName)
                        };
                        entityInfos.Add(ent.Indices[0] + 1, tmp);
                        if (!replyToIds.Contains(ent.ScreenName.ToLower()))
                        {
                            replyToIds.Add(ent.ScreenName.ToLower());
                        }
                    }
                }

                if (entities.Media != null)
                {
                    foreach (var ent in entities.Media)
                    {
                        if (ent.Type == "photo")
                        {
                            var tmp = new EntityInfo()
                            {
                                StartIndex = ent.Indices[0],
                                EndIndex = ent.Indices[1],
                                Text = ent.Url,
                                Html = string.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", ent.Url, ent.ExpandedUrl, ent.DisplayUrl),
                                Display = ent.DisplayUrl
                            };
                            entityInfos.Add(ent.Indices[0], tmp);
                            if (media != null && !media.ContainsKey(ent.Url))
                            {
                                media.Add(ent.Url, ent.MediaUrl);
                            }
                        }
                    }
                }

                if (entityInfos.Count > 0)
                {
                    try
                    {
                        int idx = 0;
                        ret = string.Empty;
                        foreach (var et in entityInfos)
                        {
                            ret += text.Substring(idx, et.Key - idx) + et.Value.Html;
                            idx = et.Value.EndIndex;
                        }
                        ret += text.Substring(idx);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Twitterのバグで不正なエンティティ（Index指定範囲が重なっている）が返ってくる場合の対応
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
            ret = this.AdjustHtml(ShortUrl.Resolve(this.PreProcessUrl(ret), false)); // IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return ret;
        }

        public bool GetInfoApi(ApiInfo info)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return true;
            }

            if (MyCommon.IsEnding)
            {
                return true;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.RateLimitStatus(ref content);
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
                var limit = D.CreateDataFromJson<RateLimitStatus>(content);
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

                if (this.ApiInformationChanged != null)
                {
                    this.ApiInformationChanged(this, arg);
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
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.GetBlockUserIds(ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var ids = D.CreateDataFromJson<List<long>>(content);
                if (ids.Contains(this.UserId))
                {
                    ids.Remove(this.UserId);
                }
                TabInformations.GetInstance().BlockIds.AddRange(ids);
                return string.Empty;
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
            lock (this.lockObj)
            {
                hashArray = this.hashList.ToArray();
                this.hashList.Clear();
            }
            return hashArray;
        }

        public EventType EventNameToEventType(string eventName)
        {
            return (from tbl in this.eventsTable where tbl.Name.Equals(eventName) select tbl.Type).FirstOrDefault();
        }

        public void StartUserStream()
        {
            if (this.UserStream != null)
            {
                this.StopUserStream();
            }
            this.UserStream = new TwitterUserstream(this.twitterConnection);
            this.UserStream.Start(this.AllAtReply, this.TrackWord);
        }

        public void StopUserStream()
        {
            if (this.UserStream != null)
            {
                this.UserStream.Dispose();
            }
            this.UserStream = null;
            if (!MyCommon.IsEnding)
            {
                if (this.UserStreamStopped != null)
                {
                    this.UserStreamStopped();
                }
            }
        }

        public void ReconnectUserStream()
        {
            if (this.UserStream != null)
            {
                this.StartUserStream();
            }
        }

        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        private string GetErrorMessageJson(string content)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(content), XmlDictionaryReaderQuotas.Max))
                    {
                        XElement elm = XElement.Load(jsonReader);
                        return elm.Element("error") != null ? elm.Element("error").Value : string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetPlainText(string orgData)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(orgData, "(?<tagStart><a [^>]+>)(?<text>[^<]+)(?<tagEnd></a>)", "${text}"));
        }

        // htmlの簡易サニタイズ(詳細表示に不要なタグの除去)
        private string SanitizeHtml(string orgdata)
        {
            string retdata = orgdata;
            retdata = Regex.Replace(retdata, "<(script|object|applet|image|frameset|fieldset|legend|style).*" + "</(script|object|applet|image|frameset|fieldset|legend|style)>", string.Empty, RegexOptions.IgnoreCase);
            retdata = Regex.Replace(retdata, "<(frame|link|iframe|img)>", string.Empty, RegexOptions.IgnoreCase);
            return retdata;
        }

        private string AdjustHtml(string orgData)
        {
            string retStr = orgData;
            retStr = Regex.Replace(retStr, "<a [^>]*href=\"/", "<a href=\"" + this.protocol + "twitter.com/");
            retStr = retStr.Replace("<a href=", "<a target=\"_self\" href=");
            retStr = retStr.Replace("\r\n", "<br>");

            // 半角スペースを置換(Thanks @anis774)
            bool ret = false;
            do
            {
                ret = this.EscapeSpace(ref retStr);
            }
            while (!ret);

            return this.SanitizeHtml(retStr);
        }

        private bool EscapeSpace(ref string html)
        {
            // 半角スペースを置換(Thanks @anis774)
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

        private bool IsPostRestricted(Status status)
        {
            PostInfo currentPost = new PostInfo(string.Empty, string.Empty, string.Empty, string.Empty);
            currentPost.CreatedAt = status.CreatedAt;
            currentPost.Id = status.IdStr;
            if (status.Text == null)
            {
                currentPost.Text = string.Empty;
            }
            else
            {
                currentPost.Text = status.Text;
            }
            currentPost.UserId = status.User.IdStr;

            if (currentPost.Equals(this.prevPostInfo))
            {
                return true;
            }
            this.prevPostInfo.CreatedAt = currentPost.CreatedAt;
            this.prevPostInfo.Id = currentPost.Id;
            this.prevPostInfo.Text = currentPost.Text;
            this.prevPostInfo.UserId = currentPost.UserId;

            return false;
        }

        private PostClass CreatePostsFromStatusData(Status status)
        {
            PostClass post = new PostClass();
            Entities entities = null;
            post.StatusId = status.Id;
            if (status.RetweetedStatus != null)
            {
                RetweetedStatus retweeted = status.RetweetedStatus;
                post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);
                // Id
                post.RetweetedId = retweeted.Id;
                // 本文
                post.TextFromApi = retweeted.Text;
                entities = retweeted.Entities;
                // Source取得（htmlの場合は、中身を取り出し）
                post.Source = retweeted.Source;
                // Reply先
                {
                    long t;
                    long.TryParse(retweeted.InReplyToStatusId, out t);
                    post.InReplyToStatusId = t;
                }
                post.InReplyToUser = retweeted.InReplyToScreenName;
                {
                    long t;
                    long.TryParse(status.InReplyToUserId, out t);
                    post.InReplyToUserId = t;
                }

                // 幻覚fav対策
                TabClass tc = TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.RetweetedId);

                if (retweeted.Geo != null)
                {
                    post.PostGeo = new PostClass.StatusGeo
                    {
                        Lat = retweeted.Geo.Coordinates[0],
                        Lng = retweeted.Geo.Coordinates[1]
                    };
                }

                // 以下、ユーザー情報
                var user = retweeted.User;
                if (user.ScreenName == null || status.User.ScreenName == null)
                {
                    return null;
                }
                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrl;
                post.IsProtect = user.Protected;
                // Retweetした人
                post.RetweetedBy = status.User.ScreenName;
                post.RetweetedByUserId = status.User.Id;
                post.IsMe = post.RetweetedBy.ToLower().Equals(this.uname);
            }
            else
            {
                post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                // 本文
                post.TextFromApi = status.Text;
                entities = status.Entities;
                // Source取得（htmlの場合は、中身を取り出し）
                post.Source = status.Source;
                {
                    long t;
                    long.TryParse(status.InReplyToStatusId, out t);
                    post.InReplyToStatusId = t;
                }
                post.InReplyToUser = status.InReplyToScreenName;
                {
                    long t;
                    long.TryParse(status.InReplyToUserId, out t);
                    post.InReplyToUserId = t;
                }

                if (status.Geo != null)
                {
                    post.PostGeo = new PostClass.StatusGeo
                    {
                        Lat = status.Geo.Coordinates[0],
                        Lng = status.Geo.Coordinates[1]
                    };
                }

                // 以下、ユーザー情報
                var user = status.User;
                if (user.ScreenName == null)
                {
                    return null;
                }
                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrl;
                post.IsProtect = user.Protected;
                post.IsMe = post.ScreenName.ToLower().Equals(this.uname);

                // 幻覚fav対策
                TabClass tc = TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.StatusId) && TabInformations.GetInstance().Item(post.StatusId).IsFav;
            }
            // HTMLに整形
            {
                var t = post.TextFromApi;
                post.Text = this.CreateHtmlAnchor(ref t, post.ReplyToList, entities, post.Media);
                post.TextFromApi = t;
            }
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
            post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
            // Source整形
            this.CreateSource(ref post);
            post.IsReply = post.ReplyToList.Contains(this.uname);
            post.IsExcludeReply = false;
            if (post.IsMe)
            {
                post.IsOwl = false;
            }
            else
            {
                if (this.followerIds.Count > 0)
                {
                    post.IsOwl = !this.followerIds.Contains(post.UserId);
                }
            }
            post.IsDm = false;
            return post;
        }

        private string CreatePostsFromJson(string content, WorkerType workerType, TabClass tab, bool read, int count, ref long minimumId)
        {
            List<Status> items = null;
            try
            {
                items = D.CreateDataFromJson<List<Status>>(content);
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

            foreach (var status in items)
            {
                PostClass post = null;
                post = this.CreatePostsFromStatusData(status);
                if (post == null)
                {
                    continue;
                }

                if (minimumId > post.StatusId)
                {
                    minimumId = post.StatusId;
                }
                // 二重取得回避
                lock (this.lockObj)
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

                // RT禁止ユーザーによるもの
                if (post.RetweetedId > 0 && this.noRetweetIds.Contains(post.RetweetedByUserId))
                {
                    continue;
                }

                post.IsRead = read;
                if (post.IsMe && !read && this.readOwnPost)
                {
                    post.IsRead = true;
                }

                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                // 非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }
            return string.Empty;
        }

        private string CreatePostsFromPhoenixSearch(string content, WorkerType workerType, TabClass tab, bool read, int count, ref long minimumId, ref string nextPageQuery)
        {
            SearchResult items = null;
            try
            {
                items = D.CreateDataFromJson<SearchResult>(content);
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

            foreach (var status in items.Statuses)
            {
                PostClass post = this.CreatePostsFromStatusData(status);
                if (post == null)
                {
                    continue;
                }

                if (minimumId > post.StatusId)
                {
                    minimumId = post.StatusId;
                }
                // 二重取得回避
                lock (this.lockObj)
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
                if (post.IsMe && !read && this.readOwnPost)
                {
                    post.IsRead = true;
                }

                if (tab != null)
                {
                    post.RelTabName = tab.TabName;
                }
                // 非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return string.IsNullOrEmpty(items.ErrMsg) ? string.Empty : "Err:" + items.ErrMsg;
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

        private string GetRelatedResultsApi(bool read, PostClass post, TabClass tab, List<PostClass> relatedPosts)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            if (MyCommon.IsEnding)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                if (post.RetweetedId > 0)
                {
                    res = this.twitterConnection.GetRelatedResults(post.RetweetedId, ref content);
                }
                else
                {
                    res = this.twitterConnection.GetRelatedResults(post.StatusId, ref content);
                }
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            List<RelatedResult> items = null;
            try
            {
                items = D.CreateDataFromJson<List<RelatedResult>>(content);
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
                return string.Empty;
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
                if (replyToItem.IsMe && !read && this.readOwnPost)
                {
                    replyToItem.IsRead = true;
                }
                replyToItem.RelTabName = tab.TabName;
            }

            bool replyAdded = false;
            foreach (var relatedData in items)
            {
                foreach (var result in relatedData.Results)
                {
                    PostClass item = this.CreatePostsFromStatusData(result.Status);
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
                    if (item.IsMe && !read && this.readOwnPost)
                    {
                        item.IsRead = true;
                    }
                    if (tab != null)
                    {
                        item.RelTabName = tab.TabName;
                    }
                    // 非同期アイコン取得＆StatusDictionaryに追加
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
                string rslt = this.GetStatusApi(read, targetItem.InReplyToStatusId, ref p);
                if (string.IsNullOrEmpty(rslt))
                {
                    p.IsRead = read;
                    p.RelTabName = tab.TabName;
                    relatedPosts.Add(p);
                }
                return rslt;
            }

            // MRTとかに対応のためツイート内にあるツイートを指すURLを取り込む
            MatchCollection ma = Regex.Matches(tab.RelationTargetPost.Text, "title=\"https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/(?<StatusId>[0-9]+))\"");
            foreach (Match m in ma)
            {
                long statusId;
                if (long.TryParse(m.Groups["StatusId"].Value, out statusId))
                {
                    PostClass p = null;
                    PostClass p2 = TabInformations.GetInstance().Item(statusId);
                    if (p2 == null)
                    {
                        this.GetStatusApi(read, statusId, ref p);
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
            return string.Empty;
        }

        private string CreateDirectMessagesFromJson(string content, WorkerType workerType, bool read)
        {
            List<Directmessage> item = null;
            try
            {
                if (workerType == WorkerType.UserStream)
                {
                    List<DirectmessageEvent> itm = D.CreateDataFromJson<List<DirectmessageEvent>>(content);
                    item = new List<Directmessage>();
                    foreach (var dat in itm)
                    {
                        item.Add(dat.Directmessage);
                    }
                }
                else
                {
                    item = D.CreateDataFromJson<List<Directmessage>>(content);
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

            foreach (var message in item)
            {
                PostClass post = new PostClass();
                try
                {
                    post.StatusId = message.Id;
                    if (workerType != WorkerType.UserStream)
                    {
                        if (workerType == WorkerType.DirectMessegeRcv)
                        {
                            if (this.minDirectmessage > post.StatusId)
                            {
                                this.minDirectmessage = post.StatusId;
                            }
                        }
                        else
                        {
                            if (this.minDirectmessageSent > post.StatusId)
                            {
                                this.minDirectmessageSent = post.StatusId;
                            }
                        }
                    }

                    // 二重取得回避
                    lock (this.lockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(TabUsageType.DirectMessage).Contains(post.StatusId))
                        {
                            continue;
                        }
                    }
                    // sender_id
                    // recipient_id
                    post.CreatedAt = MyCommon.DateTimeParse(message.CreatedAt);
                    // 本文
                    post.TextFromApi = message.Text;
                    // HTMLに整形
                    post.Text = this.CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, post.Media);
                    post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "♡");
                    post.IsFav = false;

                    // 以下、ユーザー情報
                    User user = null;
                    if (workerType == WorkerType.UserStream)
                    {
                        if (this.twitterConnection.AuthenticatedUsername.Equals(message.Recipient.ScreenName, StringComparison.CurrentCultureIgnoreCase))
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
                        if (workerType == WorkerType.DirectMessegeRcv)
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
                if (post.IsMe && !read && this.readOwnPost)
                {
                    post.IsRead = true;
                }
                post.IsReply = false;
                post.IsExcludeReply = false;
                post.IsDm = true;

                TabInformations.GetInstance().AddPost(post);
            }

            return string.Empty;
        }

        private string ReplaceTextFromApi(string text, Entities entities)
        {
            if (entities != null)
            {
                if (entities.Urls != null)
                {
                    foreach (var m in entities.Urls)
                    {
                        if (!string.IsNullOrEmpty(m.DisplayUrl))
                        {
                            text = text.Replace(m.Url, m.DisplayUrl);
                        }
                    }
                }
                if (entities.Media != null)
                {
                    foreach (var m in entities.Media)
                    {
                        if (!string.IsNullOrEmpty(m.DisplayUrl))
                        {
                            text = text.Replace(m.Url, m.DisplayUrl);
                        }
                    }
                }
            }
            return text;
        }

        private string FollowerApi(ref long cursor)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.FollowerIds(cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var followers = D.CreateDataFromJson<Ids>(content);
                this.followerIds.AddRange(followers.Id);
                cursor = followers.NextCursor;
                return string.Empty;
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

        private string NoRetweetApi(ref long cursor)
        {
            if (Twitter.AccountState != AccountState.Valid)
            {
                return string.Empty;
            }

            HttpStatusCode res = default(HttpStatusCode);
            string content = string.Empty;
            try
            {
                res = this.twitterConnection.NoRetweetIds(cursor, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = AccountState.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = AccountState.Invalid;
                    return Hoehoe.Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + System.Reflection.MethodInfo.GetCurrentMethod().Name + ")";
            }

            try
            {
                var ids = D.CreateDataFromJson<long[]>(content);
                this.noRetweetIds.AddRange(ids);
                cursor = 0; // 0より小さければ何でも良い。
                return string.Empty;
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

        /// <summary>
        /// Source整形
        /// </summary>
        /// <param name="post"></param>
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
                    post.SourceHtml = string.Copy(ShortUrl.Resolve(this.PreProcessUrl(post.Source), false));
                    post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"));
                }
                else
                {
                    post.Source = string.Empty;
                    post.SourceHtml = string.Empty;
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

        private void Twitter_ApiInformationChanged(object sender, ApiInformationChangedEventArgs e)
        {
        }

        #region "UserStream"

        private void UserStream_StatusArrived(string line)
        {
            this.lastUserstreamDataReceived = DateTime.Now;
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            bool isDm = false;
            try
            {
                using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(line), XmlDictionaryReaderQuotas.Max))
                {
                    XElement elm = XElement.Load(jsonReader);
                    if (elm.Element("friends") != null)
                    {
                        Debug.Print("friends");
                        return;
                    }
                    if (elm.Element("delete") != null)
                    {
                        Debug.Print("delete");
                        long id = default(long);
                        if (elm.Element("delete").Element("direct_message") != null && elm.Element("delete").Element("direct_message").Element("id") != null)
                        {
                            id = Convert.ToInt64(elm.Element("delete").Element("direct_message").Element("id").Value);
                            if (this.PostDeleted != null)
                            {
                                this.PostDeleted(id);
                            }
                        }
                        else if (elm.Element("delete").Element("status") != null && elm.Element("delete").Element("status").Element("id") != null)
                        {
                            id = Convert.ToInt64(elm.Element("delete").Element("status").Element("id").Value);
                            if (this.PostDeleted != null)
                            {
                                this.PostDeleted(id);
                            }
                        }
                        else
                        {
                            MyCommon.TraceOut("delete:" + line);
                            return;
                        }
                        for (int i = this.StoredEvent.Count - 1; i >= 0; i--)
                        {
                            var stored = this.StoredEvent[i];
                            if (stored.Id == id && (stored.Event == "favorite" || stored.Event == "unfavorite"))
                            {
                                this.StoredEvent.RemoveAt(i);
                            }
                        }
                        return;
                    }
                    if (elm.Element("limit") != null)
                    {
                        Debug.Print(line);
                        return;
                    }
                    if (elm.Element("event") != null)
                    {
                        Debug.Print("event: " + elm.Element("event").Value);
                        this.CreateEventFromJson(line);
                        return;
                    }
                    if (elm.Element("direct_message") != null)
                    {
                        Debug.Print("direct_message");
                        isDm = true;
                    }
                    else if (elm.Element("scrub_geo") != null)
                    {
                        try
                        {
                            TabInformations.GetInstance().ScrubGeoReserve(long.Parse(elm.Element("scrub_geo").Element("user_id").Value), long.Parse(elm.Element("scrub_geo").Element("up_to_status_id").Value));
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
                    this.CreateDirectMessagesFromJson(res.ToString(), WorkerType.UserStream, false);
                }
                else
                {
                    long t = -1;
                    this.CreatePostsFromJson(res.ToString(), WorkerType.Timeline, null, false, 0, ref t);
                }
            }
            catch (NullReferenceException)
            {
                MyCommon.TraceOut("NullRef StatusArrived: " + line);
            }

            if (this.NewPostFromStream != null)
            {
                this.NewPostFromStream();
            }
        }

        private void CreateEventFromJson(string content)
        {
            EventData eventData = null;
            try
            {
                eventData = D.CreateDataFromJson<EventData>(content);
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
            evt.Eventtype = this.EventNameToEventType(evt.Event);
            switch (eventData.Event)
            {
                case "access_revoked":
                    return;
                case "follow":
                    if (eventData.Target.ScreenName.ToLower().Equals(this.uname))
                    {
                        if (!this.followerIds.Contains(eventData.Source.Id))
                        {
                            this.followerIds.Add(eventData.Source.Id);
                        }
                    }
                    else
                    {
                        // Block後のUndoをすると、SourceとTargetが逆転したfollowイベントが帰ってくるため。
                        return;
                    }
                    evt.Target = string.Empty;
                    break;
                case "favorite":
                case "unfavorite":
                    evt.Target = "@" + eventData.TargetObject.User.ScreenName + ":" + HttpUtility.HtmlDecode(eventData.TargetObject.Text);
                    evt.Id = eventData.TargetObject.Id;
                    if (AppendSettingDialog.Instance.IsRemoveSameEvent)
                    {
                        if (this.StoredEvent.Any((FormattedEvent ev) => { return ev.Username == evt.Username && ev.Eventtype == evt.Eventtype && ev.Target == evt.Target; }))
                        {
                            return;
                        }
                    }
                    if (TabInformations.GetInstance().ContainsKey(eventData.TargetObject.Id))
                    {
                        PostClass post = TabInformations.GetInstance().Item(eventData.TargetObject.Id);
                        if (eventData.Event == "favorite")
                        {
                            if (evt.Username.ToLower().Equals(this.uname))
                            {
                                post.IsFav = true;
                                TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                            }
                            else
                            {
                                post.FavoritedCount += 1;
                                if (!TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).Contains(post.StatusId))
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread && post.IsRead)
                                    {
                                        post.IsRead = false;
                                    }
                                    TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                }
                                else
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread)
                                    {
                                        TabInformations.GetInstance().SetRead(false, TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).TabName, TabInformations.GetInstance().GetTabByType(TabUsageType.Favorites).IndexOf(post.StatusId));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (evt.Username.ToLower().Equals(this.uname))
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
                    evt.Target = string.Empty;
                    break;
                case "unblock":
                    if (TabInformations.GetInstance().BlockIds.Contains(eventData.Target.Id))
                    {
                        TabInformations.GetInstance().BlockIds.Remove(eventData.Target.Id);
                    }
                    evt.Target = string.Empty;
                    break;
                case "user_update":
                    evt.Target = string.Empty;
                    break;
                case "list_created":
                    evt.Target = string.Empty;
                    break;
                default:
                    MyCommon.TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content);
                    break;
            }
            this.StoredEvent.Insert(0, evt);
            if (this.UserStreamEventReceived != null)
            {
                this.UserStreamEventReceived(evt);
            }
        }

        private void UserStream_Started()
        {
            if (this.UserStreamStarted != null)
            {
                this.UserStreamStarted();
            }
        }

        private void UserStream_Stopped()
        {
            if (this.UserStreamStopped != null)
            {
                this.UserStreamStopped();
            }
        }

        #endregion "UserStream"

        public class FormattedEvent
        {
            public EventType Eventtype { get; set; }

            public DateTime CreatedAt { get; set; }

            public string Event { get; set; }

            public string Username { get; set; }

            public string Target { get; set; }

            public long Id { get; set; }

            public bool IsMe { get; set; }
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
        
        private class Range
        {
            public Range(int fromIndex, int toIndex)
            {
                this.FromIndex = fromIndex;
                this.ToIndex = toIndex;
            }

            public int FromIndex { get; set; }

            public int ToIndex { get; set; }
        }

        private class EntityInfo
        {
            public int StartIndex { get; set; }

            public int EndIndex { get; set; }

            public string Text { get; set; }

            public string Html { get; set; }

            public string Display { get; set; }
        }

        private class EventTypeTableElement
        {
            public string Name;

            public EventType Type;

            public EventTypeTableElement(string name, EventType type)
            {
                this.Name = name;
                this.Type = type;
            }
        }
        private class TwitterUserstream : IDisposable
        {
            private HttpTwitter twitterConnection;
            private Thread streamThread;
            private bool streamActive;
            private bool allAtreplies;
            private string trackwords = string.Empty;

            // 重複する呼び出しを検出するには
            private bool disposedValue;

            public TwitterUserstream(HttpTwitter twitterConnection)
            {
                this.twitterConnection = (HttpTwitter)twitterConnection.Clone();
            }
            public delegate void StatusArrivedEventHandler(string status);
            public delegate void StoppedEventHandler();
            public delegate void StartedEventHandler();

            public event StatusArrivedEventHandler StatusArrived;
            public event StoppedEventHandler Stopped;
            public event StartedEventHandler Started;

            public bool Enabled
            {
                get { return this.streamActive; }
            }

            public bool AllAtReplies
            {
                get { return this.allAtreplies; }
                set { this.allAtreplies = value; }
            }

            public string TrackWords
            {
                get { return this.trackwords; }
                set { this.trackwords = value; }
            }

            public void Start(bool allAtReplies, string trackwords)
            {
                this.AllAtReplies = allAtReplies;
                this.TrackWords = trackwords;
                this.streamActive = true;
                if (this.streamThread != null && this.streamThread.IsAlive)
                {
                    return;
                }
                this.streamThread = new Thread(this.UserStreamLoop);
                this.streamThread.Name = "UserStreamReceiver";
                this.streamThread.IsBackground = true;
                this.streamThread.Start();
            }

            // TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
            // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                        this.streamActive = false;
                        if (this.streamThread != null && this.streamThread.IsAlive)
                        {
                            this.streamThread.Abort();
                        }
                    }

                    // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                    // TODO: 大きなフィールドを null に設定します。
                }
                this.disposedValue = true;
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

                        if (this.Started != null)
                        {
                            this.Started();
                        }
                        HttpStatusCode res = this.twitterConnection.UserStream(ref st, this.allAtreplies, this.trackwords, MyCommon.GetUserAgentString());
                        switch (res)
                        {
                            case HttpStatusCode.OK:
                                Twitter.AccountState = AccountState.Valid;
                                break;
                            case HttpStatusCode.Unauthorized:
                                Twitter.AccountState = AccountState.Invalid;
                                sleepSec = 120;
                                continue;
                        }

                        if (st == null)
                        {
                            sleepSec = 30;
                            continue;
                        }

                        sr = new StreamReader(st);

                        while (this.streamActive && !sr.EndOfStream && Twitter.AccountState == AccountState.Valid)
                        {
                            if (this.StatusArrived != null)
                            {
                                this.StatusArrived(sr.ReadLine());
                            }
                        }

                        if (sr.EndOfStream || Twitter.AccountState == AccountState.Invalid)
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
                        // System.ArgumentException: ストリームを読み取れませんでした。
                        // サーバー側もしくは通信経路上で切断された場合？タイムアウト頻発後発生
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
                        if (this.streamActive)
                        {
                            if (this.Stopped != null)
                            {
                                this.Stopped();
                            }
                        }
                        this.twitterConnection.RequestAbort();
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
                            while (this.streamActive && ms < sleepSec * 1000)
                            {
                                Thread.Sleep(500);
                                ms += 500;
                            }
                        }
                        sleepSec = 0;
                    }
                } while (this.streamActive);

                if (this.streamActive)
                {
                    if (this.Stopped != null)
                    {
                        this.Stopped();
                    }
                }
                MyCommon.TraceOut("Stop:EndLoop");
            }
        }
    }
}