// Hoehoe - Client of Twitter
// Copyright (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
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

using System.Runtime.Serialization;

namespace Hoehoe.DataModels.Twitter
{
    // XXX: have to use TT?

    [DataContract]
    public class RateLimitStatus
    {
        [DataMember(Name = "rate_limit_context")]
        public RateLimitCtxt RateLimitContext;

        [DataMember(Name = "resources")]
        public Resources ResourcesLimit;

        [DataContract]
        public class RateLimitCtxt
        {
            [DataMember(Name = "access_token")]
            public string AccessToken;
        }

        [DataContract]
        public class Resources
        {
            [DataMember(Name = "users")]
            public UsersApiLimit Users;

            [DataMember(Name = "statuses")]
            public StatusesApiLimit Statuses;

            [DataMember(Name = "help")]
            public HelpApiLimit Help;

            [DataMember(Name = "search")]
            public SearchApiLimit Search;

            [DataMember(Name = "lists")]
            public ListsApiLimit Lists;
        }

        [DataContract]
        public class ListsApiLimit
        {
            [DataMember(Name = "/lists/subscribers")]
            public ApiLimitInfo Subscribers;

            [DataMember(Name = "/lists/list")]
            public ApiLimitInfo List;

            [DataMember(Name = "/lists/memberships")]
            public ApiLimitInfo Memberships;

            [DataMember(Name = "/lists/")]
            public ApiLimitInfo Subscriptions;

            [DataMember(Name = "/lists/members")]
            public ApiLimitInfo Members;

            [DataMember(Name = "/lists/subscribers/show")]
            public ApiLimitInfo SubscribersShow;

            [DataMember(Name = "/lists/statuses")]
            public ApiLimitInfo Statuses;

            [DataMember(Name = "/lists/show")]
            public ApiLimitInfo Show;

            [DataMember(Name = "/lists/members/show")]
            public ApiLimitInfo MembersShow;
        }

        [DataContract]
        public class AppApiLimit
        {
            [DataMember(Name = "/application/rate_limit_status")]
            public ApiLimitInfo RateLimiStatus;
        }

        [DataContract]
        public class FriendshipApiLimit
        {
            [DataMember(Name = "/friendships/lookup")]
            public ApiLimitInfo Lookup;

            [DataMember(Name = "/friendships/incoming")]
            public ApiLimitInfo Incoming;

            [DataMember(Name = "/friendships/outgoing")]
            public ApiLimitInfo Outgoing;

            [DataMember(Name = "/friendships/")]
            public ApiLimitInfo NoRetweetsIds;

            [DataMember(Name = "/friendships/")]
            public ApiLimitInfo Show;
        }

        [DataContract]
        public class BlokcsApiLimit
        {
            [DataMember(Name = "/blocks/ids")]
            public ApiLimitInfo Ids;

            [DataMember(Name = "/blocks/list")]
            public ApiLimitInfo List;
        }

        [DataContract]
        public class GeoApiLimit
        {
            [DataMember(Name = "/geo/similar_places")]
            public ApiLimitInfo SimilarPlaces;

            [DataMember(Name = "/geo/search")]
            public ApiLimitInfo Search;

            [DataMember(Name = "/geo/reverse_geocode")]
            public ApiLimitInfo ReverseGeocode;

            [DataMember(Name = "/geo/id/:place_id")]
            public ApiLimitInfo IDPlaceID;
        }

        [DataContract]
        public class UsersApiLimit
        {
            [DataMember(Name = "/users/profile_banner")]
            public ApiLimitInfo ProfileBanner;

            [DataMember(Name = "/users/show/:id")]
            public ApiLimitInfo ShowId;

            [DataMember(Name = "/users/lookup")]
            public ApiLimitInfo Lookup;

            [DataMember(Name = "/users/search")]
            public ApiLimitInfo Search;

            [DataMember(Name = "/users/contributors")]
            public ApiLimitInfo Contributors;

            [DataMember(Name = "/users/contributees")]
            public ApiLimitInfo Contirbutees;

            [DataMember(Name = "/users/suggestions")]
            public ApiLimitInfo Suggestions;

            [DataMember(Name = "/users/suggestions/:slug")]
            public ApiLimitInfo SuggestionsSlug;

            [DataMember(Name = "/users/suggestions/:slug/members")]
            public ApiLimitInfo SuggestionsSlugMembers;
        }

        [DataContract]
        public class FollowersApiLimit
        {
            [DataMember(Name = "/followers/list")]
            public ApiLimitInfo List;

            [DataMember(Name = "/followers/ids")]
            public ApiLimitInfo Ids;
        }

        [DataContract]
        public class StatusesApiLimit
        {
            [DataMember(Name = "/statuses/mentions_timeline")]
            public ApiLimitInfo MentionsTimeline;

            [DataMember(Name = "/statuses/show/:id")]
            public ApiLimitInfo ShowId;

            [DataMember(Name = "/statuses/oembed")]
            public ApiLimitInfo Oembed;

            [DataMember(Name = "/statuses/home_timeline")]
            public ApiLimitInfo HomeTimelime;

            [DataMember(Name = "/statuses/user_timeline")]
            public ApiLimitInfo UserTimeline;

            [DataMember(Name = "/statuses/retweets_of_me")]
            public ApiLimitInfo RetweetsOfMe;

            [DataMember(Name = "/statuses/retweets/:id")]
            public ApiLimitInfo RetweetsId;
        }

        [DataContract]
        public class HelpApiLimit
        {
            [DataMember(Name = "/help/privacy")]
            public ApiLimitInfo Privacy;

            [DataMember(Name = "/help/tos")]
            public ApiLimitInfo Tos;

            [DataMember(Name = "/help/configiration")]
            public ApiLimitInfo Configuration;

            [DataMember(Name = "/help/languages")]
            public ApiLimitInfo Languages;
        }

        [DataContract]
        public class FriendsApiLimit
        {
            [DataMember(Name = "/friends/ids")]
            public ApiLimitInfo Ids;

            [DataMember(Name = "/friends/list")]
            public ApiLimitInfo List;
        }

        [DataContract]
        public class DirectMessageApiLimit
        {
            [DataMember(Name = "/direct_messages/show")]
            public ApiLimitInfo Show;

            [DataMember(Name = "/direct_messages/sent_and_received")]
            public ApiLimitInfo SentAndReceived;

            [DataMember(Name = "/direct_messages/sent")]
            public ApiLimitInfo Sent;

            [DataMember(Name = "/direct_messages")]
            public ApiLimitInfo DirectMessages;
        }

        [DataContract]
        public class AccountApiLimit
        {
            [DataMember(Name = "/account/verify_credentials")]
            public ApiLimitInfo VerifyCredentials;

            [DataMember(Name = "/account/settings")]
            public ApiLimitInfo Settings;
        }

        [DataContract]
        public class FavoritesApiLimit
        {
            [DataMember(Name = "/favorites/list")]
            public ApiLimitInfo List;
        }

        [DataContract]
        public class SavedSearchesApiLimit
        {
            [DataMember(Name = "/saved_searches/destroy/:id")]
            public ApiLimitInfo DestroyId;

            [DataMember(Name = "/saved_searches/list")]
            public ApiLimitInfo List;

            [DataMember(Name = "/saved_searches/show/:id")]
            public ApiLimitInfo ShowId;
        }

        [DataContract]
        public class SearchApiLimit
        {
            [DataMember(Name = "/search/tweets")]
            public ApiLimitInfo Tweets;
        }

        [DataContract]
        public class TrendsApiLimit
        {
            [DataMember(Name = "/trends/available")]
            public ApiLimitInfo Available;

            [DataMember(Name = "/trends/place")]
            public ApiLimitInfo Place;

            [DataMember(Name = "/trends/closest")]
            public ApiLimitInfo Closest;
        }

        [DataContract]
        public class ApiLimitInfo
        {
            [DataMember(Name = "limit")]
            public int Limit;

            [DataMember(Name = "remaining")]
            public int Remaining;

            [DataMember(Name = "reset")]
            public long Reset;
        }
    }
}