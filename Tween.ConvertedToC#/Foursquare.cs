using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Tween
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
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }
            Match mc = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                string vId = mc.Result("${venueId}");
                return vId;
            }
            else
            {
                return "";
            }
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
                        curData = MyCommon.CreateDataFromJson(content);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                    return curData.Response.Venue;
                }
                else
                {
                    //Dim curData As FourSquareDataModel.FourSquareData = Nothing
                    //Try
                    //    curData = CreateDataFromJson(Of FourSquareDataModel.FourSquareData)(content)
                    //Catch ex As Exception
                    //    Return Nothing
                    //End Try
                    //MessageBox.Show(res.ToString + Environment.NewLine + curData.Meta.ErrorType + Environment.NewLine + curData.Meta.ErrorDetail)
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetMapsUri(string url, ref string refText)
        {
            if (!AppendSettingDialog.Instance.IsPreviewFoursquare)
                return null;

            string urlId = Regex.Replace(url, "https?://(4sq|foursquare)\\.com/", "");

            if (CheckInUrlsVenueCollection.ContainsKey(urlId))
            {
                refText = CheckInUrlsVenueCollection[urlId].LocateInfo;
                return (new Google()).CreateGoogleStaticMapsUri(CheckInUrlsVenueCollection[urlId]);
            }

            FourSquareDataModel.Venue curVenue = null;
            string venueId = GetVenueId(url);
            if (string.IsNullOrEmpty(venueId))
                return null;

            curVenue = GetVenueInfo(venueId);
            if (curVenue == null)
                return null;

            Google.GlobalLocation curLocation = new Google.GlobalLocation
            {
                Latitude = curVenue.Location.Latitude,
                Longitude = curVenue.Location.Longitude,
                LocateInfo = CreateVenueInfoText[curVenue]
            };
            //例外発生の場合があるため
            if (!CheckInUrlsVenueCollection.ContainsKey(urlId))
                CheckInUrlsVenueCollection.Add(urlId, curLocation);
            refText = curLocation.LocateInfo;
            return (new Google()).CreateGoogleStaticMapsUri(curLocation);
        }

        private string CreateVenueInfoText
        {
            get { return info.Name + Environment.NewLine + info.Stats.UsersCount.ToString + "/" + info.Stats.CheckinsCount.ToString + Environment.NewLine + info.Location.Address + Environment.NewLine + info.Location.City + info.Location.State + Environment.NewLine + info.Location.Latitude.ToString + Environment.NewLine + info.Location.Longitude.ToString; }
        }

        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref string content)
        {
            HttpWebRequest webReq = CreateRequest(method, requestUri, param, false);
            HttpStatusCode code = default(HttpStatusCode);
            code = GetResponse(webReq, content, null, false);
            return code;
        }

        #region "FourSquare DataModel"

        public class FourSquareDataModel
        {
            [DataContract()]
            public class FourSquareData
            {
                [DataMember(Name = "meta", isRequired = false)]
                public Meta Meta;

                [DataMember(Name = "response", isRequired = false)]
                public Response Response;
            }

            [DataContract()]
            public class Response
            {
                [DataMember(Name = "venue", isRequired = false)]
                public Venue Venue;
            }

            [DataContract()]
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

            [DataContract()]
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

            [DataContract()]
            public class Stats
            {
                [DataMember(Name = "checkinsCount")]
                public int CheckinsCount;

                [DataMember(Name = "usersCount")]
                public int UsersCount;
            }

            [DataContract()]
            public class Mayor
            {
                [DataMember(Name = "count")]
                public int Count;

                [DataMember(Name = "user", isrequired = false)]
                public FoursquareUser User;
            }

            [DataContract()]
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

            [DataContract()]
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