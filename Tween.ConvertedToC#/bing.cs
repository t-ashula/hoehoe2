using System.Collections.Generic;

namespace Tween
{
    public class Bing
    {
        private const string AppId = "8DFACAC0C4891D0F75F5728391C9D30664B797A1";

        #region "言語テーブル定義"

        private static readonly List<string> LanguageTable = new List<string> {
			"af",
			"sq",
			"ar-sa",
			"ar-iq",
			"ar-eg",
			"ar-ly",
			"ar-dz",
			"ar-ma",
			"ar-tn",
			"ar-om",
			"ar-ye",
			"ar-sy",
			"ar-jo",
			"ar-lb",
			"ar-kw",
			"ar-ae",
			"ar-bh",
			"ar-qa",
			"eu",
			"bg",
			"be",
			"ca",
			"zh-tw",
			"zh-cn",
			"zh-hk",
			"zh-sg",
			"hr",
			"cs",
			"da",
			"nl",
			"nl-be",
			"en",
			"en-us",
			"en-gb",
			"en-au",
			"en-ca",
			"en-nz",
			"en-ie",
			"en-za",
			"en-jm",
			"en",
			"en-bz",
			"en-tt",
			"et",
			"fo",
			"fa",
			"fi",
			"fr",
			"fr-be",
			"fr-ca",
			"fr-ch",
			"fr-lu",
			"gd",
			"ga",
			"de",
			"de-ch",
			"de-at",
			"de-lu",
			"de-li",
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
			"pt-br",
			"pt",
			"rm",
			"ro",
			"ro-mo",
			"ru",
			"ru-mo",
			"sz",
			"sr",
			"sr",
			"sk",
			"sl",
			"sb",
			"es",
			"es-mx",
			"es-gt",
			"es-cr",
			"es-pa",
			"es-do",
			"es-ve",
			"es-co",
			"es-pe",
			"es-ar",
			"es-ec",
			"es-cl",
			"es-uy",
			"es-py",
			"es-bo",
			"es-sv",
			"es-hn",
			"es-ni",
			"es-pr",
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

			#endregion "言語テーブル定義"
		};

        #region "Translation"

        private const string TranslateUri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + AppId;

        public bool Translate(string _from, string _to, string _text, ref string buf)
        {
            HttpVarious http = new HttpVarious();
            string apiurl = TranslateUri + "&text=" + System.Web.HttpUtility.UrlEncode(_text) + "&to=" + _to;
            string content = "";
            if (http.GetData(apiurl, null, ref content))
            {
                buf = string.Copy(content);
                return true;
            }
            return false;
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
    }
}