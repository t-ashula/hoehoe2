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
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Web;

namespace Tween
{
    public class Google
    {
        #region "Translation"

        // http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
        // デベロッパー ガイド - Google AJAX Language API - Google Code

        private const string TranslateEndPoint = "http://ajax.googleapis.com/ajax/services/language/translate";

        private const string LanguageDetectEndPoint = "https://ajax.googleapis.com/ajax/services/language/detect";

        #region "言語テーブル定義"

        private static List<string> LanguageTable = new List<string> {
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

			#endregion "言語テーブル定義"
		};

        [DataContract()]
        public class TranslateResponseData
        {
            [DataMember(Name = "translatedText")]
            public string TranslatedText;
        }

        [DataContract()]
        private class TranslateResponse
        {
            [DataMember(Name = "responseData")]
            public TranslateResponseData ResponseData;

            [DataMember(Name = "responseDetails")]
            public string ResponseDetails;

            [DataMember(Name = "responseStatus")]
            public HttpStatusCode ResponseStatus;
        }

        [DataContract()]
        public class LanguageDetectResponseData
        {
            [DataMember(Name = "language")]
            public string Language;

            [DataMember(Name = "isReliable")]
            public bool IsReliable;

            [DataMember(Name = "confidence")]
            public double Confidence;
        }

        [DataContract()]
        private class LanguageDetectResponse
        {
            [DataMember(Name = "responseData")]
            public LanguageDetectResponseData ResponseData;

            [DataMember(Name = "responseDetails")]
            public string ResponseDetails;

            [DataMember(Name = "responseStatus")]
            public HttpStatusCode ResponseStatus;
        }

        public bool Translate(string srclng, string dstlng, string source, ref string destination, ref string ErrMsg)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = TranslateEndPoint;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("v", "1.0");

            ErrMsg = "";
            if (string.IsNullOrEmpty(srclng) || string.IsNullOrEmpty(dstlng))
            {
                return false;
            }
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("langpair", srclng + "|" + dstlng);

            headers.Add("q", source);

            string content = "";
            if (http.GetData(apiurl, headers, ref content))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TranslateResponse));
                TranslateResponse res = null;

                try
                {
                    res = MyCommon.CreateDataFromJson<TranslateResponse>(content);
                }
                catch (Exception ex)
                {
                    ErrMsg = "Err:Invalid JSON";
                    return false;
                }

                if (res.ResponseData == null)
                {
                    ErrMsg = "Err:" + res.ResponseDetails;
                    return false;
                }
                string _body = res.ResponseData.TranslatedText;
                string buf = HttpUtility.UrlDecode(_body);

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
            string content = "";
            if (http.GetData(apiurl, headers, ref content))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LanguageDetectResponse));
                try
                {
                    LanguageDetectResponse res = MyCommon.CreateDataFromJson<LanguageDetectResponse>(content);
                    return res.ResponseData.Language;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            return "";
        }

        public string GetLanguageEnumFromIndex(int index)
        {
            return LanguageTable[index];
        }

        public int GetIndexFromLanguageEnum(string lang)
        {
            return LanguageTable.IndexOf(lang);
        }

        #endregion "Translation"

        #region "UrlShortener"

        // http://code.google.com/intl/ja/apis/urlshortener/v1/getting_started.html
        // Google URL Shortener API

        [DataContract()]
        private class UrlShortenerParameter
        {
            [DataMember(Name = "longUrl")]
            string LongUrl;
        }

        [DataContract()]
        private class UrlShortenerResponse
        {
        }

        public string Shorten(string source)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = "https://www.googleapis.com/urlshortener/v1/url";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("Content-Type", "application/json");

            http.PostData(apiurl, headers);
            return "";
        }

        #endregion "UrlShortener"

        #region "GoogleMaps"

        public string CreateGoogleStaticMapsUri(GlobalLocation locate)
        {
            return CreateGoogleStaticMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleStaticMapsUri(double lat, double lng)
        {
            return "http://maps.google.com/maps/api/staticmap?center=" + lat.ToString() + "," + lng.ToString() + "&size=" + AppendSettingDialog.Instance.FoursquarePreviewWidth.ToString() + "x" + AppendSettingDialog.Instance.FoursquarePreviewHeight.ToString() + "&zoom=" + AppendSettingDialog.Instance.FoursquarePreviewZoom.ToString() + "&markers=" + lat.ToString() + "," + lng.ToString() + "&sensor=false";
        }

        public string CreateGoogleMapsUri(GlobalLocation locate)
        {
            return CreateGoogleMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleMapsUri(double lat, double lng)
        {
            return "http://maps.google.com/maps?ll=" + lat.ToString() + "," + lng.ToString() + "&z=" + AppendSettingDialog.Instance.FoursquarePreviewZoom.ToString() + "&q=" + lat.ToString() + "," + lng.ToString();
        }

        public class GlobalLocation
        {
            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public string LocateInfo { get; set; }
        }

        #endregion "GoogleMaps"

        #region "Google Analytics"

        public class GASender : HttpConnection
        {
            private const string GA_ACCOUNT = "UA-4618605-5";
            // この hash を あとで みつける
            private const string GA_DOMAIN_HASH = "211246021";
            private const string GA_HOSTNAME = "apps.tweenapp.org";
            private const string GA_VERSION = "5.1.5";

            private const string GA_CHARACTER_SET = "shift_jis";
            //#define GA_COLOR_DEPTH                  @"24-bit" // とれるなら かんきょう から
            private const string GA_JAVA_ENABLED = "1";
            //"10.1 r102"をURLエンコード
            private const string GA_FLASH_VERSION = "10.0 r32";
            private const string GA_PAGE_TITLE = "Tween";

            private const string GA_GIF_URL = "http://www.google-analytics.com/__utm.gif";
            private DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

            private static Random rnd = new Random();
            private string _language;
            private string _screenResolution;
            private string _screenColorDepth;

            private int _sessionCount;

            public long SessionFirst { get; set; }

            public long SessionLast { get; set; }

            public event SentEventHandler Sent;

            public delegate void SentEventHandler();

            //Singleton
            private static GASender _me = new GASender();

            public static GASender GetInstance()
            {
                return _me;
            }

            private GASender()
            {
                this._language = System.Globalization.CultureInfo.CurrentCulture.Name.Replace('_', '-');
                //Me._language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName
                this._screenResolution = string.Format("{0}x{1}", System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                this._screenColorDepth = string.Format("{0}-bit", System.Windows.Forms.Screen.PrimaryScreen.BitsPerPixel);
                ThreadStart proc = null;
                proc = () =>
                {
                    System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    while (!MyCommon.IsEnding)
                    {
                        if (this.gaQueue.Count > 0)
                        {
                            Dictionary<string, string> param = null;
                            lock (this.syncObj)
                            {
                                param = this.gaQueue.Dequeue();
                            }
                            try
                            {
                                HttpWebRequest req = CreateRequest(GetMethod, new Uri(GA_GIF_URL), param, false);
                                req.AllowAutoRedirect = true;
                                req.Accept = "*/*";
                                req.Referer = "http://apps.tweenapp.org/foo.html";
                                req.Headers.Add("Accept-Language", "ja-JP");
                                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; MALC)";
                                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                                Bitmap img = null;
                                var res = this.GetResponse(req, ref img, null, false);
                            }
                            catch (Exception ex)
                            {
                                //nothing to do
                            }
                        }
                        Thread.Sleep(5000);
                    }
                };
                proc.BeginInvoke(null, null);
            }

            private void Init()
            {
                this.SessionFirst = Convert.ToInt64((DateTime.Now - UnixEpoch).TotalSeconds);
                this.SessionLast = this.SessionFirst;
            }

            private void SendRequest(Dictionary<string, string> info, long userId)
            {
                if (userId == 0)
                    return;
                if (this.SessionFirst == 0)
                    this.Init();

                this._sessionCount += 1;
                long sessionCurrent = Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                string utma = string.Format("{0}.{1}.{2}.{3}.{4}.{5}", GA_DOMAIN_HASH, userId, this.SessionFirst, this.SessionLast, sessionCurrent, this._sessionCount);
                string utmz = string.Format("{0}.{1}.{2}.{3}.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", GA_DOMAIN_HASH, this.SessionFirst, 1, 1);
                //Dim utmcc = String.Format("__utma={0};+__utmz={1};",
                //                        utma,
                //                        utmz)
                var utmcc = string.Format("__utma={0};", utma);
                this.SessionLast = sessionCurrent;

                Dictionary<string, string> @params = new Dictionary<string, string> {
					{						"utmwv",						GA_VERSION					},
					{						"utms",						"1"					},
					{						"utmn",						rnd.Next().ToString()					},
					{						"utmhn",						GA_HOSTNAME					},
					{						"utmcs",						GA_CHARACTER_SET					},
					{						"utmsr",						this._screenResolution					},
					{						"utmsc",						this._screenColorDepth					},
					{						"utmul",						this._language					},
					{						"utmje",						GA_JAVA_ENABLED					},
					{						"utmfl",						GA_FLASH_VERSION					},
					{						"utmhid",						rnd.Next().ToString()					},
					{						"utmr",						"-"					},
					{						"utmp",						"/"					},
					{						"utmac",						GA_ACCOUNT					},
					{						"utmcc",						utmcc					},
					{						"utmu",						"q~"					}
				};
                //                {"utmdt", GA_PAGE_TITLE},

                if (info.ContainsKey("page"))
                {
                    @params["utmp"] = info["page"];
                    if (info.ContainsKey("referer"))
                    {
                        @params["utmr"] = info["referer"];
                    }
                }
                if (info.ContainsKey("event"))
                {
                    @params.Add("utmt", "event");
                    @params.Add("utme", info["event"]);
                    @params["utmr"] = "0";
                }

                //Me.GetAsync(params, New Uri(GA_GIF_URL))
                lock (syncObj)
                {
                    this.gaQueue.Enqueue(@params);
                }
            }

            private object syncObj = new object();

            private Queue<Dictionary<string, string>> gaQueue = new Queue<Dictionary<string, string>>();

            public void TrackPage(string page, long userId)
            {
                this.SendRequest(new Dictionary<string, string> { {
					"page",
					page
				} }, userId);
            }

            public void TrackEventWithCategory(string category, string action, long userId)
            {
                this.TrackEventWithCategory(category, action, null, null, userId);
            }

            public void TrackEventWithCategory(string category, string action, string label, long userId)
            {
                this.TrackEventWithCategory(category, action, label, null, userId);
            }

            public void TrackEventWithCategory(string category, string action, string label, string value, long userId)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.AppendFormat("5({0}*{1}", category, action);
                if (!string.IsNullOrEmpty(label))
                {
                    builder.AppendFormat("*{0}", label);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    builder.AppendFormat(")({0}", value);
                }
                builder.Append(")");
                this.SendRequest(new Dictionary<string, string> { {
					"event",
					builder.ToString()
				} }, userId);
            }
        }

        #endregion "Google Analytics"
    }
}