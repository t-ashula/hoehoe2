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

    public class ApiInformationChangedEventArgs : EventArgs
    {
        public ApiInfo ApiInfo = new ApiInfo();
    }

    public abstract class ApiInfoBase
    {
        protected static int _MaxCount = -1;
        protected static int _RemainCount = -1;
        protected static DateTime _ResetTime = new DateTime();
        protected static int _ResetTimeInSeconds = -1;
        protected static int _UsingCount = -1;
        protected static ApiAccessLevel _AccessLevel = ApiAccessLevel.None;
        protected static int _MediaMaxCount = -1;
        protected static DateTime _MediaResetTime = new DateTime();
        protected static int _MediaRemainCount = -1;
    }

    public class ApiInfo : ApiInfoBase
    {
        public int MaxCount;
        public int RemainCount;
        public DateTime ResetTime;
        public int ResetTimeInSeconds;
        public int UsingCount;
        public ApiAccessLevel AccessLevel;
        public int MediaMaxCount;
        public int MediaRemainCount;

        public DateTime MediaResetTime;

        public ApiInfo()
        {
            this.MaxCount = ApiInfo._MaxCount;
            this.RemainCount = ApiInfo._RemainCount;
            this.ResetTime = ApiInfo._ResetTime;
            this.ResetTimeInSeconds = ApiInfo._ResetTimeInSeconds;
            this.UsingCount = ApiInfo._UsingCount;
            this.AccessLevel = ApiInfo._AccessLevel;
            this.MediaMaxCount = ApiInfo._MediaMaxCount;
            this.MediaRemainCount = ApiInfo._MediaRemainCount;
            this.MediaResetTime = ApiInfo._MediaResetTime;
        }
    }

    public class ApiInformation : ApiInfoBase
    {
        public Dictionary<string, string> HttpHeaders = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

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

            ApiInformation._MaxCount = -1;
            ApiInformation._RemainCount = -1;
            ApiInformation._ResetTime = new DateTime();
            ApiInformation._ResetTimeInSeconds = -1;
            ApiInformation._MediaMaxCount = -1;
            ApiInformation._MediaRemainCount = -1;
            ApiInformation._MediaResetTime = new DateTime();
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

        public event ChangedEventHandler Changed;

        public delegate void ChangedEventHandler(object sender, ApiInformationChangedEventArgs e);

        private void Raise_Changed()
        {
            ApiInformationChangedEventArgs arg = new ApiInformationChangedEventArgs();
            if (this.Changed != null)
            {
                this.Changed(this, arg);
            }

            ApiInformation._MaxCount = arg.ApiInfo.MaxCount;
            ApiInformation._RemainCount = arg.ApiInfo.RemainCount;
            ApiInformation._ResetTime = arg.ApiInfo.ResetTime;
            ApiInformation._ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
        }

        public int MaxCount
        {
            get
            {
                return ApiInformation._MaxCount;
            }

            set
            {
                if (ApiInformation._MaxCount != value)
                {
                    ApiInformation._MaxCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public int RemainCount
        {
            get
            {
                return ApiInformation._RemainCount;
            }

            set
            {
                if (ApiInformation._RemainCount != value)
                {
                    ApiInformation._RemainCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public DateTime ResetTime
        {
            get
            {
                return ApiInformation._ResetTime;
            }

            set
            {
                if (ApiInformation._ResetTime != value)
                {
                    ApiInformation._ResetTime = value;
                    this.Raise_Changed();
                }
            }
        }

        public int MediaMaxCount
        {
            get
            {
                return ApiInformation._MediaMaxCount;
            }

            set
            {
                if (ApiInformation._MediaMaxCount != value)
                {
                    ApiInformation._MediaMaxCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public int MediaRemainCount
        {
            get
            {
                return ApiInformation._MediaRemainCount;
            }

            set
            {
                if (ApiInformation._MediaRemainCount != value)
                {
                    ApiInformation._MediaRemainCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public DateTime MediaResetTime
        {
            get
            {
                return ApiInformation._MediaResetTime;
            }

            set
            {
                if (ApiInformation._MediaResetTime != value)
                {
                    ApiInformation._MediaResetTime = value;
                    this.Raise_Changed();
                }
            }
        }

        public int ResetTimeInSeconds
        {
            get
            {
                return ApiInformation._ResetTimeInSeconds;
            }

            set
            {
                if (ApiInformation._ResetTimeInSeconds != value)
                {
                    ApiInformation._ResetTimeInSeconds = value;
                    this.Raise_Changed();
                }
            }
        }

        public int UsingCount
        {
            get
            {
                return ApiInformation._UsingCount;
            }

            set
            {
                if (ApiInformation._UsingCount != value)
                {
                    ApiInformation._UsingCount = value;
                    this.Raise_Changed();
                }
            }
        }

        public ApiAccessLevel AccessLevel
        {
            get
            {
                return ApiInformation._AccessLevel;
            }

            private set
            {
                if (ApiInformation._AccessLevel != value)
                {
                    ApiInformation._AccessLevel = value;
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
                    MyCommon.TraceOut("Unknown ApiAccessLevel:" + this.HttpHeaders["X-Access-Level"]);
                    // 未知のアクセスレベルの場合Read/Write/Dmと仮定して処理継続
                    return ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        public void ParseHttpHeaders(Dictionary<string, string> headers)
        {
            int tmp = 0;
            DateTime tmpd = default(DateTime);
            tmp = this.GetMaxCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation._MaxCount = tmp;
            }
            tmp = this.GetRemainCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation._RemainCount = tmp;
            }
            tmpd = this.GetResetTimeFromHttpHeader();
            if (tmpd != new DateTime())
            {
                ApiInformation._ResetTime = tmpd;
            }

            tmp = this.GetMediaMaxCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation._MediaMaxCount = tmp;
            }
            tmp = this.GetMediaRemainCountFromHttpHeader();
            if (tmp != -1)
            {
                ApiInformation._MediaRemainCount = tmp;
            }
            tmpd = this.GetMediaResetTimeFromHttpHeader();
            if (tmpd != new DateTime())
            {
                ApiInformation._MediaResetTime = tmpd;
            }

            this.AccessLevel = this.GetApiAccessLevelFromHttpHeader();
            this.Raise_Changed();
        }

        public void WriteBackEventArgs(ApiInformationChangedEventArgs arg)
        {
            ApiInformation._MaxCount = arg.ApiInfo.MaxCount;
            ApiInformation._RemainCount = arg.ApiInfo.RemainCount;
            ApiInformation._ResetTime = arg.ApiInfo.ResetTime;
            ApiInformation._ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
            ApiInformation._UsingCount = arg.ApiInfo.UsingCount;
            this.Raise_Changed();
        }
    }
}