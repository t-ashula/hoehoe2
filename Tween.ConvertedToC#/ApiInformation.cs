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

    public class ApiInformation : ApiInfoBase
    {
        private Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        public delegate void ChangedEventHandler(object sender, ApiInformationChangedEventArgs e);

        public event ChangedEventHandler Changed;

        public Dictionary<string, string> HttpHeaders
        {
            get { return this.headers; }
            set { this.headers = value; }
        }

        public int MaxCount
        {
            get
            {
                return ApiInformation.maxCount;
            }

            set
            {
                if (ApiInformation.maxCount != value)
                {
                    ApiInformation.maxCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public int RemainCount
        {
            get
            {
                return ApiInformation.remainCount;
            }

            set
            {
                if (ApiInformation.remainCount != value)
                {
                    ApiInformation.remainCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public DateTime ResetTime
        {
            get
            {
                return ApiInformation.resetTime;
            }

            set
            {
                if (ApiInformation.resetTime != value)
                {
                    ApiInformation.resetTime = value;
                    this.Raise_Changed();
                }
            }
        }

        public int MediaMaxCount
        {
            get
            {
                return ApiInformation.mediaMaxCount;
            }

            set
            {
                if (ApiInformation.mediaMaxCount != value)
                {
                    ApiInformation.mediaMaxCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public int MediaRemainCount
        {
            get
            {
                return ApiInformation.mediaRemainCount;
            }

            set
            {
                if (ApiInformation.mediaRemainCount != value)
                {
                    ApiInformation.mediaRemainCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public DateTime MediaResetTime
        {
            get
            {
                return ApiInformation.mediaResetTime;
            }

            set
            {
                if (ApiInformation.mediaResetTime != value)
                {
                    ApiInformation.mediaResetTime = value;
                    this.Raise_Changed();
                }
            }
        }

        public int ResetTimeInSeconds
        {
            get
            {
                return ApiInformation.resetTimeInSeconds;
            }

            set
            {
                if (ApiInformation.resetTimeInSeconds != value)
                {
                    ApiInformation.resetTimeInSeconds = value;
                    this.Raise_Changed();
                }
            }
        }

        public int UsingCount
        {
            get
            {
                return ApiInformation.usingCount;
            }

            set
            {
                if (ApiInformation.usingCount != value)
                {
                    ApiInformation.usingCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public ApiAccessLevel AccessLevel
        {
            get
            {
                return ApiInformation.accessLevel;
            }

            private set
            {
                if (ApiInformation.accessLevel != value)
                {
                    ApiInformation.accessLevel = value;
                    this.Raise_Changed();
                }
            }
        }

        public bool IsReadPermission
        {
            get
            {
                return this.AccessLevel == ApiAccessLevel.Read
                    || this.AccessLevel == ApiAccessLevel.ReadWrite
                    || this.AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        public bool IsWritePermission
        {
            get
            {
                return this.AccessLevel == ApiAccessLevel.ReadWrite
                    || this.AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        public bool IsDirectMessagePermission
        {
            get { return this.AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage; }
        }

        public void Initialize()
        {
            if (this.HttpHeaders.ContainsKey("X-RateLimit-Remaining"))
            {
                this.HttpHeaders["X-RateLimit-Remaining"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-RateLimit-Remaining", "-1");
            }

            if (this.HttpHeaders.ContainsKey("X-RateLimit-Limit"))
            {
                this.HttpHeaders["X-RateLimit-Limit"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-RateLimit-Limit", "-1");
            }

            if (this.HttpHeaders.ContainsKey("X-RateLimit-Reset"))
            {
                this.HttpHeaders["X-RateLimit-Reset"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-RateLimit-Reset", "-1");
            }

            if (this.HttpHeaders.ContainsKey("X-Access-Level"))
            {
                this.HttpHeaders["X-Access-Level"] = "read-write-directmessages";
            }
            else
            {
                this.HttpHeaders.Add("X-Access-Level", "read-write-directmessages");
            }

            if (this.HttpHeaders.ContainsKey("X-MediaRateLimit-Remaining"))
            {
                this.HttpHeaders["X-MediaRateLimit-Remaining"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-MediaRateLimit-Remaining", "-1");
            }

            if (this.HttpHeaders.ContainsKey("X-MediaRateLimit-Limit"))
            {
                this.HttpHeaders["X-MediaRateLimit-Limit"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-MediaRateLimit-Limit", "-1");
            }

            if (this.HttpHeaders.ContainsKey("X-MediaRateLimit-Reset"))
            {
                this.HttpHeaders["X-MediaRateLimit-Reset"] = "-1";
            }
            else
            {
                this.HttpHeaders.Add("X-MediaRateLimit-Reset", "-1");
            }

            ApiInformation.maxCount = -1;
            ApiInformation.remainCount = -1;
            ApiInformation.resetTime = new DateTime();
            ApiInformation.resetTimeInSeconds = -1;
            ApiInformation.mediaMaxCount = -1;
            ApiInformation.mediaRemainCount = -1;
            ApiInformation.mediaResetTime = new DateTime();
            this.AccessLevel = ApiAccessLevel.None;

            // _UsingCount = -1
            if (this.Changed != null)
            {
                this.Changed(this, new ApiInformationChangedEventArgs());
            }
        }

        public DateTime ConvertResetTimeInSecondsToResetTime(int seconds)
        {
            if (seconds >= 0)
            {
                return TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(seconds));
            }
            else
            {
                return new DateTime();
            }
        }

        public void ParseHttpHeaders(Dictionary<string, string> headers)
        {
            int tmp = 0;
            DateTime tmpd = default(DateTime);
            tmp = this.GetMaxCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation.maxCount = tmp;
            }

            tmp = this.GetRemainCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation.remainCount = tmp;
            }
            
            tmpd = this.GetResetTimeFromHttpHeader();
            if (tmpd != new DateTime())
            {
                ApiInformation.resetTime = tmpd;
            }

            tmp = this.GetMediaMaxCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation.mediaMaxCount = tmp;
            }
            
            tmp = this.GetMediaRemainCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation.mediaRemainCount = tmp;
            }
            
            tmpd = this.GetMediaResetTimeFromHttpHeader();
            if (tmpd != new DateTime())
            {
                ApiInformation.mediaResetTime = tmpd;
            }

            this.AccessLevel = this.GetApiAccessLevelFromHttpHeader();
            this.Raise_Changed();
        }

        public void WriteBackEventArgs(ApiInformationChangedEventArgs arg)
        {
            ApiInformation.maxCount = arg.ApiInfo.MaxCount;
            ApiInformation.remainCount = arg.ApiInfo.RemainCount;
            ApiInformation.resetTime = arg.ApiInfo.ResetTime;
            ApiInformation.resetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
            ApiInformation.usingCount = arg.ApiInfo.UsingCount;
            this.Raise_Changed();
        }

        private void Raise_Changed()
        {
            ApiInformationChangedEventArgs arg = new ApiInformationChangedEventArgs();
            if (this.Changed != null)
            {
                this.Changed(this, arg);
            }

            ApiInformation.maxCount = arg.ApiInfo.MaxCount;
            ApiInformation.remainCount = arg.ApiInfo.RemainCount;
            ApiInformation.resetTime = arg.ApiInfo.ResetTime;
            ApiInformation.resetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
        }

        private int GetRemainCountFromHttpHeader()
        {
            int result = 0;
            if (string.IsNullOrEmpty(this.HttpHeaders["X-RateLimit-Remaining"]))
            {
                return -1;
            }

            if (int.TryParse(this.HttpHeaders["X-RateLimit-Remaining"], out result))
            {
                return result;
            }

            return -1;
        }

        private int GetMaxCountFromHttpHeader()
        {
            int result = 0;
            if (string.IsNullOrEmpty(this.HttpHeaders["X-RateLimit-Limit"]))
            {
                return -1;
            }

            if (int.TryParse(this.HttpHeaders["X-RateLimit-Limit"], out result))
            {
                return result;
            }
            
            return -1;
        }

        private DateTime GetResetTimeFromHttpHeader()
        {
            int i = 0;
            if (int.TryParse(this.HttpHeaders["X-RateLimit-Reset"], out i))
            {
                if (i >= 0)
                {
                    return TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i));
                }
                else
                {
                    return new DateTime();
                }
            }
            else
            {
                return new DateTime();
            }
        }

        private int GetMediaRemainCountFromHttpHeader()
        {
            int result = 0;
            if (string.IsNullOrEmpty(this.HttpHeaders["X-MediaRateLimit-Remaining"]))
            {
                return -1;
            }

            if (int.TryParse(this.HttpHeaders["X-MediaRateLimit-Remaining"], out result))
            {
                return result;
            }
            
            return -1;
        }

        private int GetMediaMaxCountFromHttpHeader()
        {
            int result = 0;
            if (string.IsNullOrEmpty(this.HttpHeaders["X-MediaRateLimit-Limit"]))
            {
                return -1;
            }
            
            if (int.TryParse(this.HttpHeaders["X-MediaRateLimit-Limit"], out result))
            {
                return result;
            }
            
            return -1;
        }

        private DateTime GetMediaResetTimeFromHttpHeader()
        {
            int i = 0;
            if (int.TryParse(this.HttpHeaders["X-MediaRateLimit-Reset"], out i))
            {
                return i >= 0 ? TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i)) : new DateTime();
            }
            
            return new DateTime();
        }

        private ApiAccessLevel GetApiAccessLevelFromHttpHeader()
        {
            switch (this.HttpHeaders["X-Access-Level"])
            {
                case "read":
                    return ApiAccessLevel.Read;
                case "read-write":
                    return ApiAccessLevel.ReadWrite;
                case "read-write-directmessages":
                case "read-write-privatemessages":
                    return ApiAccessLevel.ReadWriteAndDirectMessage;
                default:
                    // 未知のアクセスレベルの場合Read/Write/Dmと仮定して処理継続
                    MyCommon.TraceOut("Unknown ApiAccessLevel:" + this.HttpHeaders["X-Access-Level"]);
                    return ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }
    }
}