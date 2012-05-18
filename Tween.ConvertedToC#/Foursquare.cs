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
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Hoehoe
{
    public class Foursquare : HttpConnection
    {
        private static Foursquare _instance = new Foursquare();

        public static Foursquare GetInstance
        {
            get { return _instance; }
        }

        private Dictionary<string, string> _authKey = new Dictionary<string, string> {
			{
				"client_id",
				"VWVC5NMXB1T5HKOYAKARCXKZDOHDNYSRLEMDDQYJNSJL2SUU"
			},
			{
				"client_secret",
				MyCommon.DecryptString("eXXMGYXZyuDxz/lJ9nLApihoUeEGXNLEO0ZDCAczvwdKgGRExZl1Xyac/ezNTwHFOLUZqaA8tbA=")
			}
		};

        private Dictionary<string, Google.GlobalLocation> CheckInUrlsVenueCollection = new Dictionary<string, Google.GlobalLocation>();

        private string GetVenueId(string url)
        {
            string content = "";
            try
            {
                HttpStatusCode res = GetContent("GET", new Uri(url), null, ref content);
                if (res != HttpStatusCode.OK)
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            Match mc = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase);
            return mc.Success ? mc.Result("${venueId}") : "";
        }

        private FourSquareDataModel.Venue GetVenueInfo(string venueId)
        {
            string content = "";
            try
            {
                HttpStatusCode res = GetContent("GET", new Uri("https://api.foursquare.com/v2/venues/" + venueId), _authKey, ref content);

                if (res == HttpStatusCode.OK)
                {
                    FourSquareDataModel.FourSquareData curData = null;
                    try
                    {
                        curData = MyCommon.CreateDataFromJson<FourSquareDataModel.FourSquareData>(content);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                    return curData.Response.Venue;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetMapsUri(string url, ref string refText)
        {
            if (!AppendSettingDialog.Instance.IsPreviewFoursquare)
            {
                return null;
            }

            string urlId = Regex.Replace(url, "https?://(4sq|foursquare)\\.com/", "");

            if (CheckInUrlsVenueCollection.ContainsKey(urlId))
            {
                refText = CheckInUrlsVenueCollection[urlId].LocateInfo;
                return (new Google()).CreateGoogleStaticMapsUri(CheckInUrlsVenueCollection[urlId]);
            }

            FourSquareDataModel.Venue curVenue = null;
            string venueId = GetVenueId(url);
            if (String.IsNullOrEmpty(venueId))
            {
                return null;
            }

            curVenue = GetVenueInfo(venueId);
            if (curVenue == null)
            {
                return null;
            }

            Google.GlobalLocation curLocation = new Google.GlobalLocation
            {
                Latitude = curVenue.Location.Latitude,
                Longitude = curVenue.Location.Longitude,
                LocateInfo = CreateVenueInfoText(curVenue)
            };
            //例外発生の場合があるため
            if (!CheckInUrlsVenueCollection.ContainsKey(urlId))
            {
                CheckInUrlsVenueCollection.Add(urlId, curLocation);
            }
            refText = curLocation.LocateInfo;
            return (new Google()).CreateGoogleStaticMapsUri(curLocation);
        }

        private string CreateVenueInfoText(FourSquareDataModel.Venue info)
        {
            return info.Name + Environment.NewLine
                + info.Stats.UsersCount.ToString() + "/" + info.Stats.CheckinsCount.ToString() + Environment.NewLine
                + info.Location.Address + Environment.NewLine
                + info.Location.City + info.Location.State + Environment.NewLine
                + info.Location.Latitude.ToString() + Environment.NewLine
                + info.Location.Longitude.ToString();
        }

        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref string content)
        {
            HttpWebRequest webReq = CreateRequest(method, requestUri, param, false);
            HttpStatusCode code = default(HttpStatusCode);
            code = GetResponse(webReq, ref content, null, false);
            return code;
        }

        #region "FourSquare DataModel"

        public class FourSquareDataModel
        {
            [DataContract]
            public class FourSquareData
            {
                [DataMember(Name = "meta", IsRequired = false)]
                public Meta Meta;

                [DataMember(Name = "response", IsRequired = false)]
                public Response Response;
            }

            [DataContract]
            public class Response
            {
                [DataMember(Name = "venue", IsRequired = false)]
                public Venue Venue;
            }

            [DataContract]
            public class Venue
            {
                [DataMember(Name = "id")]
                public string Id;

                [DataMember(Name = "name")]
                public string Name;

                [DataMember(Name = "location")]
                public Location Location;

                [DataMember(Name = "verified")]
                public bool Verified;

                [DataMember(Name = "stats")]
                public Stats Stats;

                [DataMember(Name = "mayor")]
                public Mayor Mayor;

                [DataMember(Name = "shortUrl")]
                public string ShortUrl;

                [DataMember(Name = "timeZone")]
                public string TimeZone;
            }

            [DataContract]
            public class Location
            {
                [DataMember(Name = "address")]
                public string Address;

                [DataMember(Name = "city")]
                public string City;

                [DataMember(Name = "state")]
                public string State;

                [DataMember(Name = "lat")]
                public double Latitude;

                [DataMember(Name = "lng")]
                public double Longitude;
            }

            [DataContract]
            public class Stats
            {
                [DataMember(Name = "checkinsCount")]
                public int CheckinsCount;

                [DataMember(Name = "usersCount")]
                public int UsersCount;
            }

            [DataContract]
            public class Mayor
            {
                [DataMember(Name = "count")]
                public int Count;

                [DataMember(Name = "user", IsRequired = false)]
                public FoursquareUser User;
            }

            [DataContract]
            public class FoursquareUser
            {
                [DataMember(Name = "id")]
                public int Id;

                [DataMember(Name = "firstName")]
                public string FirstName;

                [DataMember(Name = "photo")]
                public string Photo;

                [DataMember(Name = "gender")]
                public string Gender;

                [DataMember(Name = "homeCity")]
                public string HomeCity;
            }

            [DataContract]
            public class Meta
            {
                [DataMember(Name = "code")]
                public int Code;

                [DataMember(Name = "errorType", IsRequired = false)]
                public string ErrorType;

                [DataMember(Name = "errorDetail", IsRequired = false)]
                public string ErrorDetail;
            }
        }

        #endregion "FourSquare DataModel"
    }
}