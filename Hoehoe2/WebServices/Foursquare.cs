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
using System.Text.RegularExpressions;
using Hoehoe.DataModels;
using Hoehoe.DataModels.FourSquare;

namespace Hoehoe
{
    public class Foursquare : HttpConnection
    {
        private static readonly Foursquare Instance = new Foursquare();

        /// <summary>
        /// 4SQ Api Key : TODO
        /// </summary>
        private readonly Dictionary<string, string> _authKey = new Dictionary<string, string>
        {
            { "client_id", "VWVC5NMXB1T5HKOYAKARCXKZDOHDNYSRLEMDDQYJNSJL2SUU" },
            { "client_secret", CryptoUtils.DecryptString("eXXMGYXZyuDxz/lJ9nLApihoUeEGXNLEO0ZDCAczvwdKgGRExZl1Xyac/ezNTwHFOLUZqaA8tbA=") }
        };

        private readonly Dictionary<string, Google.GlobalLocation> _checkInUrlsVenueCollection = new Dictionary<string, Google.GlobalLocation>();

        public static Foursquare GetInstance
        {
            get { return Instance; }
        }

        public string GetMapsUri(string url, ref string refText)
        {
            if (!Configs.Instance.IsPreviewFoursquare)
            {
                return null;
            }

            var urlId = Regex.Replace(url, "https?://(4sq|foursquare)\\.com/", string.Empty);

            if (_checkInUrlsVenueCollection.ContainsKey(urlId))
            {
                refText = _checkInUrlsVenueCollection[urlId].LocateInfo;
                return (new Google()).CreateGoogleStaticMapsUri(_checkInUrlsVenueCollection[urlId]);
            }

            var venueId = GetVenueId(url);
            if (string.IsNullOrEmpty(venueId))
            {
                return null;
            }

            var curVenue = GetVenueInfo(venueId);
            if (curVenue == null)
            {
                return null;
            }

            var curLocation = new Google.GlobalLocation
            {
                Latitude = curVenue.Location.Latitude,
                Longitude = curVenue.Location.Longitude,
                LocateInfo = CreateVenueInfoText(curVenue)
            };

            // 例外発生の場合があるため
            if (!_checkInUrlsVenueCollection.ContainsKey(urlId))
            {
                _checkInUrlsVenueCollection.Add(urlId, curLocation);
            }

            refText = curLocation.LocateInfo;
            return new Google().CreateGoogleStaticMapsUri(curLocation);
        }

        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref string content)
        {
            var webReq = CreateRequest(method, requestUri, param, false);
            var code = GetResponse(webReq, ref content, null, false);
            return code;
        }

        private string GetVenueId(string url)
        {
            var content = string.Empty;
            try
            {
                var res = GetContent("GET", new Uri(url), null, ref content);
                if (res != HttpStatusCode.OK)
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            var mc = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase);
            return mc.Success ? mc.Result("${venueId}") : string.Empty;
        }

        private Venue GetVenueInfo(string venueId)
        {
            var content = string.Empty;
            try
            {
                var res = GetContent("GET", new Uri("https://api.foursquare.com/v2/venues/" + venueId), _authKey, ref content);

                if (res == HttpStatusCode.OK)
                {
                    FourSquareData curData;
                    try
                    {
                        curData = D.CreateDataFromJson<FourSquareData>(content);
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    return curData.Response.Venue;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string CreateVenueInfoText(Venue info)
        {
            return string.Format(
                "{1}{0}{2}/{3}{0}{4}{0}{5}{6}{0}{7}{0}{8}",
                Environment.NewLine,
                info.Name,
                info.Stats.UsersCount,
                info.Stats.CheckinsCount,
                info.Location.Address,
                info.Location.City,
                info.Location.State,
                info.Location.Latitude,
                info.Location.Longitude);
        }
    }
}