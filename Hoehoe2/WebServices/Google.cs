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
    using System.Runtime.Serialization.Json;
    using System.Web;
    using Hoehoe.DataModels;
    using Hoehoe.DataModels.Google;

    public class Google
    {
        #region "Translation"

        /*
         * デベロッパー ガイド - Google AJAX Language_ API - Google Code
         * http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
         */

        private const string TranslateEndPoint = "http://ajax.googleapis.com/ajax/services/language/translate";
        private const string LanguageDetectEndPoint = "https://ajax.googleapis.com/ajax/services/language/detect";

        #region "言語テーブル定義"

        private static List<string> languages = new List<string>
        {
            "af",
            "sq",
            "am",
            "ar",
   "hy",
   "az",
   "eu",
   "be",
   "bn",
   "bh",
   "br",
   "bg",
   "my",
   "ca",
   "chr",
   "zh",
   "zh-CN",
   "zh-TW",
   "co",
   "hr",
   "cs",
   "da",
   "dv",
   "nl",
   "en",
   "eo",
   "et",
   "fo",
   "tl",
   "fi",
   "fr",
   "fy",
   "gl",
   "ka",
   "de",
   "el",
   "gu",
   "ht",
   "iw",
   "hi",
   "hu",
   "is",
   "id",
   "iu",
   "ga",
   "it",
   "ja",
   "jw",
   "kn",
   "kk",
   "km",
   "ko",
   "ku",
   "ky",
   "lo",
   "la",
   "lv",
   "lt",
   "lb",
   "mk",
   "ms",
   "ml",
   "mt",
   "mi",
   "mr",
   "mn",
   "ne",
   "no",
   "oc",
   "or",
   "ps",
   "fa",
   "pl",
   "pt",
   "pt-PT",
   "pa",
   "qu",
   "ro",
   "ru",
   "sa",
   "gd",
   "sr",
   "sd",
   "si",
   "sk",
   "sl",
   "es",
   "su",
   "sw",
   "sv",
   "syr",
   "tg",
   "ta",
   "tt",
   "te",
   "th",
   "bo",
   "to",
   "tr",
   "uk",
   "ur",
   "uz",
   "ug",
   "vi",
   "cy",
   "yi",
   "yo"
        };

        #endregion "言語テーブル定義"

        public bool Translate(string srclng, string dstlng, string source, ref string destination, ref string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(srclng) || string.IsNullOrEmpty(dstlng))
            {
                return false;
            }

            string apiurl = TranslateEndPoint;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("v", "1.0");
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("langpair", srclng + "|" + dstlng);
            headers.Add("q", source);

            string content = string.Empty;
            HttpVarious http = new HttpVarious();
            if (http.GetData(apiurl, headers, ref content))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TranslateResponse));
                TranslateResponse res = null;

                try
                {
                    res = D.CreateDataFromJson<TranslateResponse>(content);
                }
                catch (Exception)
                {
                    errorMessage = "Err:Invalid JSON";
                    return false;
                }

                if (res.ResponseData == null)
                {
                    errorMessage = "Err:" + res.ResponseDetails;
                    return false;
                }

                string body = res.ResponseData.TranslatedText;
                string buf = HttpUtility.UrlDecode(body);
                destination = string.Copy(buf);
                return true;
            }

            return false;
        }

        public string LanguageDetect(string source)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = LanguageDetectEndPoint;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("v", "1.0");
            headers.Add("q", source);
            string content = string.Empty;
            if (http.GetData(apiurl, headers, ref content))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LanguageDetectResponse));
                try
                {
                    LanguageDetectResponse res = D.CreateDataFromJson<LanguageDetectResponse>(content);
                    return res.ResponseData.Language;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public string GetLanguageEnumFromIndex(int index)
        {
            return languages[index];
        }

        public int GetIndexFromLanguageEnum(string lang)
        {
            return languages.IndexOf(lang);
        }

        #endregion "Translation"

        #region "UrlShortener"

        // Google URL Shortener API
        // http://code.google.com/intl/ja/apis/urlshortener/v1/getting_started.html
        public string Shorten(string source)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("Content-Type", "application/json");

            HttpVarious http = new HttpVarious();
            string apiurl = "https://www.googleapis.com/urlshortener/v1/url";
            http.PostData(apiurl, headers);
            return string.Empty;
        }

        #endregion "UrlShortener"

        #region "GoogleMaps"

        public string CreateGoogleStaticMapsUri(GlobalLocation locate)
        {
            return CreateGoogleStaticMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleStaticMapsUri(double lat, double lng)
        {
            return string.Format(
                "http://maps.google.com/maps/api/staticmap?center={0},{1}&size={2}x{3}&zoom={4}&markers={0},{1}&sensor=false",
                lat,
                lng,
                Configs.Instance.FoursquarePreviewWidth,
                Configs.Instance.FoursquarePreviewHeight,
                Configs.Instance.FoursquarePreviewZoom);
        }

        public string CreateGoogleMapsUri(GlobalLocation locate)
        {
            return CreateGoogleMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleMapsUri(double lat, double lng)
        {
            return string.Format("http://maps.google.com/maps?ll={0},{1}&z={2}&q={0},{1}", lat, lng, Configs.Instance.FoursquarePreviewZoom);
        }

        public class GlobalLocation
        {
            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public string LocateInfo { get; set; }
        }

        #endregion "GoogleMaps"
    }
}