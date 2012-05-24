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
    using System.Web;

    public class Bing
    {
        /// <summary>
        /// TODO:BingAppID 
        /// </summary>
        private const string AppId = "8DFACAC0C4891D0F75F5728391C9D30664B797A1";
        private const string TranslateUri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + AppId;

        #region "言語テーブル定義"
        private static readonly List<string> languages = new List<string> 
        {
          "af",
          "sq",
          "ar-sa", "ar-iq", "ar-eg", "ar-ly", "ar-dz", "ar-ma", "ar-tn", "ar-om", "ar-ye", "ar-sy", "ar-jo", "ar-lb", "ar-kw", "ar-ae", "ar-bh", "ar-qa",
          "eu",
          "bg",
          "be",
          "ca",
          "zh-tw",  "zh-cn", "zh-hk", "zh-sg",
          "hr",
          "cs",
          "da",
          "nl", "nl-be",
          "en", "en-us", "en-gb", "en-au", "en-ca", "en-nz", "en-ie", "en-za", "en-jm", "en", "en-bz", "en-tt", 
          "et",
          "fo",
          "fa",
          "fi",
          "fr", "fr-be", "fr-ca", "fr-ch", "fr-lu",
          "gd",
          "ga",
          "de", "de-ch", "de-at", "de-lu", "de-li",
          "el",
          "he",
          "hi",
          "hu",
          "is",
          "id",
          "it",
          "it-ch",
          "ja",
          "ko",
          "ko",
          "lv",
          "lt",
          "mk",
          "ms",
          "mt",
          "no",
          "no",
          "pl",
          "pt-br", "pt",
          "rm",
          "ro", "ro-mo",
          "ru", "ru-mo",
          "sz",
          "sr",
          "sr",
          "sk",
          "sl",
          "sb",
          "es", "es-mx", "es-gt", "es-cr", "es-pa", "es-do", "es-ve", "es-co", "es-pe", "es-ar", "es-ec", "es-cl", "es-uy", "es-py", "es-bo", "es-sv", "es-hn", "es-ni", "es-pr",
          "sx",
          "sv",
          "sv-fi",
          "th",
          "ts",
          "tn",
          "tr",
          "uk",
          "ur",
          "ve",
          "vi",
          "xh",
          "ji",
          "zu" 
        };

        #endregion "言語テーブル定義"

        #region "Translation"

        public bool Translate(string from, string to, string text, ref string buf)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = TranslateUri + "&text=" + HttpUtility.UrlEncode(text) + "&to=" + to;
            string content = string.Empty;
            if (http.GetData(apiurl, null, ref content))
            {
                buf = string.Copy(content);
                return true;
            }

            return false;
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
    }
}