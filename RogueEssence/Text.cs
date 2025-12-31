using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using System.IO;
using System.Resources.NetStandard;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence
{
    /// <summary>
    /// Represents settings for a supported language, including its display name and fallback languages.
    /// </summary>
    public class LanguageSetting
    {
        /// <summary>
        /// The display name of the language (e.g., "English", "Japanese").
        /// </summary>
        public string Name;

        /// <summary>
        /// List of language codes to fall back to if a string is not found in this language.
        /// </summary>
        public List<string> Fallbacks;

        /// <summary>
        /// Initializes a new instance of the LanguageSetting class.
        /// </summary>
        /// <param name="name">The display name of the language.</param>
        /// <param name="fallbacks">The list of fallback language codes.</param>
        public LanguageSetting(string name, List<string> fallbacks)
        {
            Name = name;
            Fallbacks = new List<string>();
            Fallbacks.AddRange(fallbacks);
        }
    }

    /// <summary>
    /// Provides localization and text formatting utilities for the game.
    /// Handles loading, caching, and formatting of localized strings with support for
    /// grammar rules in multiple languages including English, Spanish, German, Italian, and Korean.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// The newline string used as a divider.
        /// </summary>
        public const string DIVIDER_STR = "\n";

        /// <summary>
        /// The base filename for localization string files.
        /// </summary>
        public const string STRINGS_FILE_NAME = "strings";

        /// <summary>
        /// The file extension for localization resource files.
        /// </summary>
        public const string STRINGS_FILE_EXT = ".resx";

        /// <summary>
        /// Dictionary of loaded localization strings keyed by string ID.
        /// </summary>
        public static Dictionary<string, string> Strings;

        /// <summary>
        /// Dictionary of extended localization strings for custom content.
        /// </summary>
        public static Dictionary<string, string> StringsEx;

        /// <summary>
        /// The current culture/language setting.
        /// </summary>
        public static CultureInfo Culture;

        /// <summary>
        /// Array of supported language codes.
        /// </summary>
        public static string[] SupportedLangs;

        /// <summary>
        /// Dictionary mapping language codes to their settings.
        /// </summary>
        public static Dictionary<string, LanguageSetting> LangNames;

        private static string subMsgRegex = @"(?<pause>\[pause=(?<pauseval>\d+)\])" +
                                                @"|(?<sound>\[sound=(?<soundval>[A-Za-z\/0-9\-_]*),?(?<speaktime>\d*)?\])" +
                                                @"|(?<colorstart>\[color=#(?<colorval>[0-9a-f]{6})\])|(?<colorend>\[color\])" +
                                                @"|(?<boxbreak>\[br\])" +
                                                @"|(?<scrollbreak>\[scroll\])" +
                                                @"|(?<script>\[script=(?<scriptval>\d+)\])" +
                                                @"|(?<speed>\[speed=(?<speedval>[+-]?\d+\.?\d*)\])" +
                                                @"|(?<emote>\[emote=(?<emoteval>[a-zA-Z0-9\-]*)\])";

        private static string subGenderRegex = @"(?<sex>\[male\]|\[female\]|\[neutral\])";

        public static Regex SubMsgTags = new Regex(subMsgRegex,
                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex MsgTags = new Regex(subMsgRegex + @"|" + subGenderRegex,
                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Compiled regex for grammar tag replacements supporting multiple languages.
        /// Handles articles and grammatical gender for English, Spanish, German, Italian, and Korean.
        /// </summary>
        public static Regex GrammarTags = new Regex(@"(?<a_an>\[a/an\]\W+(?<a_anval>\w))" + //en
                                                @"|(?<el_la>\[el/la\]\W+?(?<el_lasex>\[male\]|\[female\])?\w)" + //es
                                                @"|(?<los_las>\[los/las\]\W+?(?<los_lassex>\[male\]|\[female\])?\w)" + //es
                                                @"|(?<der_die_das>\[der/die/das\]\W+?(?<der_die_dassex>\[male\]|\[female\]|\[neutral\])?\w)" + //de
                                                @"|(?<ein_eine_einen>\[ein/eine/einen\]\W+?(?<ein_eine_einensex>\[male\]|\[female\]|\[neutral\])?\w)" + //de
                                                @"|(?<ein_eine_ein>\[ein/eine/ein\]\W+?(?<ein_eine_einsex>\[male\]|\[female\]|\[neutral\])?\w)" + //de
                                                @"|(?<il_la>\[il/la\]\W+?(?<il_lasex>\[male\]|\[female\])?(?<il_laval>\w\w?))" + //it
                                                @"|(?<i_le>\[i/le\]\W+?(?<i_lesex>\[male\]|\[female\])?(?<i_leval>\w\w?))" + //it
                                                @"|(?<uno_una>\[uno/una\]\W+?(?<uno_unasex>\[male\]|\[female\])?(?<uno_unaval>\w))" + //it
                                                @"|(?<eun_neun>(?<eun_neunval>\w)[^가-힣]+?\[은/는\])" + //ko
                                                @"|(?<eul_leul>(?<eul_leulval>\w)[^가-힣]+?\[을/를\])" + //ko
                                                @"|(?<i_ga>(?<i_gaval>\w)[^가-힣]+?\[이/가\])" + //ko
                                                @"|(?<wa_gwa>(?<wa_gwaval>\w)[^가-힣]+?\[와/과\])" + //ko
                                                @"|(?<eu_lo>(?<eu_loval>\w)[^가-힣]+?\[으/로\])" + //ko
                                                @"|(?<i_lamyeon>(?<i_lamyeonval>\w)[^가-힣]+?\[이/라면\])", //ko
                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes the Text system by registering encoding providers and loading language definitions.
        /// Must be called before using any localization features.
        /// </summary>
        public static void Init()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Strings = new Dictionary<string, string>();
            StringsEx = new Dictionary<string, string>();

            List<string> codes = new List<string>();
            Dictionary<string, LanguageSetting> translations = new Dictionary<string, LanguageSetting>();
            try
            {
                foreach (string path in PathMod.FallforthPaths("Strings/Languages.xml"))
                {
                    if (File.Exists(path))
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(path);
                        foreach (XmlNode xnode in xmldoc.DocumentElement.ChildNodes)
                        {
                            if (xnode.Name == "data")
                            {
                                string value = null;
                                string name = null;
                                var atname = xnode.Attributes["name"];
                                if (atname != null)
                                    name = atname.Value;

                                //Get value
                                XmlNode valnode = xnode.SelectSingleNode("value");
                                if (valnode != null)
                                    value = valnode.InnerText;

                                List<string> fallbacks = new List<string>();
                                foreach (XmlNode fallbacknode in xnode.SelectNodes("fallback"))
                                    fallbacks.Add(fallbacknode.InnerText);

                                if (!codes.Contains(name))
                                    codes.Add(name);
                                translations[name] = new LanguageSetting(value, fallbacks);
                            }
                        }
                    }
                }
                SupportedLangs = codes.ToArray();
                LangNames = translations;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                SupportedLangs = new string[1] { "en" };
                LangNames["en"] = new LanguageSetting("English", new List<string>());
            }
        }

        /// <summary>
        /// Converts a language code to its display name.
        /// </summary>
        /// <param name="lang">The language code.</param>
        /// <returns>The display name of the language.</returns>
        public static string ToName(this string lang)
        {
            return LangNames[lang].Name;
        }

        /// <summary>
        /// Loads localization strings from a RESX file.
        /// </summary>
        /// <param name="path">The path to the RESX file.</param>
        /// <returns>A dictionary of string key-value pairs.</returns>
        public static Dictionary<string, string> LoadStringResx(string path)
        {
            try
            {
                Dictionary<string, string> translations = new Dictionary<string, string>();
                if (File.Exists(path))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);
                    foreach (XmlNode xnode in xmldoc.DocumentElement.ChildNodes)
                    {
                        if (xnode.Name == "data")
                        {
                            string value = null;
                            string name = null;
                            var atname = xnode.Attributes["name"];
                            if (atname != null)
                                name = atname.Value;

                            //Get value
                            XmlNode valnode = xnode.SelectSingleNode("value");
                            if (valnode != null)
                                value = valnode.InnerText;

                            if (value != null && name != null)
                                translations[name] = value;
                        }
                    }
                }
                return translations;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Loads localization strings with comments from a RESX file for development use.
        /// </summary>
        /// <param name="path">The path to the RESX file.</param>
        /// <returns>A dictionary of string keys to value and comment tuples.</returns>
        public static Dictionary<string, (string val, string comment)> LoadDevStringResx(string path)
        {
            try
            {
                Dictionary<string, (string val, string comment)> translations = new Dictionary<string, (string val, string comment)>();
                if (File.Exists(path))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);
                    foreach (XmlNode xnode in xmldoc.DocumentElement.ChildNodes)
                    {
                        if (xnode.Name == "data")
                        {
                            string value = null;
                            string name = null;
                            string comment = "";
                            var atname = xnode.Attributes["name"];
                            if (atname != null)
                                name = atname.Value;

                            //Get value
                            XmlNode valnode = xnode.SelectSingleNode("value");
                            if (valnode != null)
                                value = valnode.InnerText;

                            //Get comment
                            XmlNode comnode = xnode.SelectSingleNode("comment");
                            if (comnode != null)
                                comment = comnode.InnerText;

                            if (value != null && name != null)
                                translations[name] = (value, comment);
                        }
                    }
                }
                return translations;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                return new Dictionary<string, (string, string)>();
            }
        }


        /// <summary>
        /// Loads script-specific localization strings with fallback support.
        /// </summary>
        /// <param name="code">The language code to load.</param>
        /// <param name="basePath">The base path for script files.</param>
        /// <param name="packagefilepath">The relative path within the package.</param>
        /// <returns>A dictionary of localized strings.</returns>
        public static Dictionary<string, string> LoadScriptStringDict(string code, string basePath, string packagefilepath)
        {
            Dictionary<string, string> xmlDict = new Dictionary<string, string>();

            //order of string fallbacks:
            //first go through all mods of the original language
            foreach (ModHeader mod in PathMod.FallbackScriptMods(basePath))
            {
                string modulePath = PathMod.HardMod(mod.Path, Path.Join(basePath, mod.Namespace, packagefilepath, STRINGS_FILE_NAME + "." + code + ".resx"));
                if (File.Exists(modulePath))
                {
                    Dictionary<string, string> dict = LoadStringResx(modulePath);
                    foreach (string key in dict.Keys)
                    {
                        if (!xmlDict.ContainsKey(key))
                            xmlDict.Add(key, dict[key]);
                    }
                }
            }

            //then go through all mods of the official fallbacks
            if (Text.LangNames.ContainsKey(code))
            {
                foreach (string fallback in Text.LangNames[code].Fallbacks)
                {
                    foreach (ModHeader mod in PathMod.FallbackScriptMods(basePath))
                    {
                        string modulePath = PathMod.HardMod(mod.Path, Path.Join(basePath, mod.Namespace, packagefilepath, STRINGS_FILE_NAME + "." + fallback + ".resx"));
                        if (File.Exists(modulePath))
                        {
                            Dictionary<string, string> dict = LoadStringResx(modulePath);
                            foreach (string key in dict.Keys)
                            {
                                if (!xmlDict.ContainsKey(key))
                                    xmlDict.Add(key, dict[key]);
                            }
                        }
                    }
                }
            }
            //then go through all mods of the default language
            foreach (ModHeader mod in PathMod.FallbackScriptMods(basePath))
            {
                string modulePath = PathMod.HardMod(mod.Path, Path.Join(basePath, mod.Namespace, packagefilepath, STRINGS_FILE_NAME + ".resx"));
                if (File.Exists(modulePath))
                {
                    Dictionary<string, string> dict = LoadStringResx(modulePath);
                    foreach (string key in dict.Keys)
                    {
                        if (!xmlDict.ContainsKey(key))
                            xmlDict.Add(key, dict[key]);
                    }
                }
            }
            return xmlDict;
        }

        /// <summary>
        /// Loads script-specific localization strings for all languages for development use.
        /// </summary>
        /// <param name="basePath">The base path for script files.</param>
        /// <param name="packagefilepath">The relative path within the package.</param>
        /// <param name="excludeEdit">Whether to exclude the current namespace from loading.</param>
        /// <returns>A nested dictionary of string keys to language code to value/comment pairs.</returns>
        public static Dictionary<string, Dictionary<string, (string val, string comment)>> LoadDevScriptStringDict(string basePath, string packagefilepath, bool excludeEdit)
        {
            Dictionary<string, Dictionary<string, (string val, string comment)>> rawStrings = new Dictionary<string, Dictionary<string, (string, string)>>();
            foreach (string code in Text.SupportedLangs)
            {
                //go through all mods of the original language
                foreach (ModHeader mod in PathMod.FallbackScriptMods(basePath))
                {
                    if (excludeEdit && mod.Namespace == PathMod.GetCurrentNamespace())
                        continue;

                    string modulePath = PathMod.HardMod(mod.Path, Path.Join(basePath, mod.Namespace, packagefilepath, STRINGS_FILE_NAME + (code == "en" ? "" : ("." + code)) + ".resx"));
                    if (File.Exists(modulePath))
                    {
                        Dictionary<string, (string, string)> xmlDict = LoadDevStringResx(modulePath);
                        foreach (string name in xmlDict.Keys)
                        {
                            if (!rawStrings.ContainsKey(name))
                                rawStrings.Add(name, new Dictionary<string, (string val, string comment)>());

                            if (!rawStrings[name].ContainsKey(code))
                                rawStrings[name].Add(code, xmlDict[name]);
                        }
                    }
                }
            }
            return rawStrings;
        }

        /// <summary>
        /// Saves localization strings to a RESX file.
        /// </summary>
        /// <param name="path">The path to save the RESX file.</param>
        /// <param name="stringDict">The dictionary of strings with comments to save.</param>
        public static void SaveStringResx(string path, Dictionary<string, (string val, string comment)> stringDict)
        {
            using (ResXResourceWriter resx = new ResXResourceWriter(path))
            {
                foreach (string key in stringDict.Keys)
                    resx.AddResource(new ResXDataNode(key, stringDict[key].val) { Comment = stringDict[key].comment });

                resx.Generate();
                resx.Close();
            }
        }

        /// <summary>
        /// Formats a string with arguments and applies language-specific grammar rules.
        /// Handles article selection (a/an), gender agreement, and Korean particles.
        /// </summary>
        /// <param name="input">The format string with grammar tags.</param>
        /// <param name="args">Arguments to insert into the format string.</param>
        /// <returns>The formatted and grammar-corrected string.</returns>
        public static string FormatGrammar(string input, params object[] args)
        {
            try
            {
                string output = String.Format(input, args);

                List<(int idx, string replace)> reInserts = new List<(int, string)>();
                int lag = 0;
                MatchCollection tagMatches = SubMsgTags.Matches(output);
                foreach (Match match in tagMatches)
                {
                    reInserts.Add((match.Index - lag, output.Substring(match.Index - lag, match.Length)));
                    output = output.Remove(match.Index - lag, match.Length);
                    lag += match.Length;
                }

                List<(int idx, int length, string replace)> replacements = new List<(int, int, string)>();
                MatchCollection matches = GrammarTags.Matches(output);
                foreach (Match match in matches)
                {
                    foreach (string key in match.Groups.Keys)
                    {
                        if (!match.Groups[key].Success)
                            continue;
                        switch (key)
                        {
                            case "a_an":
                                {
                                    string vowelcheck = match.Groups["a_anval"].Value;

                                    if (Regex.IsMatch(vowelcheck, "^[aeiou]", RegexOptions.IgnoreCase))
                                        replacements.Add(chooseIndefinite(match, "[a/an]", "an"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[a/an]", "a"));
                                }
                                break;
                            case "el_la":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["el_lasex"].Value, Gender.Male);

                                    if (gendercheck == Gender.Male)
                                        replacements.Add(chooseIndefinite(match, "[el/la]", "el"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[el/la]", "la"));

                                    if (match.Groups["el_lasex"].Success)
                                        replacements.Add((match.Groups["el_lasex"].Index, match.Groups["el_lasex"].Value.Length, ""));
                                }
                                break;
                            case "los_las":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["los_lassex"].Value, Gender.Male);

                                    if (gendercheck == Gender.Male)
                                        replacements.Add(chooseIndefinite(match, "[los/las]", "los"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[los/las]", "las"));

                                    if (match.Groups["los_lassex"].Success)
                                        replacements.Add((match.Groups["los_lassex"].Index, match.Groups["los_lassex"].Value.Length, ""));
                                }
                                break;
                            case "der_die_das":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["der_die_dassex"].Value, Gender.Male);

                                    if (gendercheck == Gender.Male)
                                        replacements.Add(chooseIndefinite(match, "[der/die/das]", "der"));
                                    else if (gendercheck == Gender.Female)
                                        replacements.Add(chooseIndefinite(match, "[der/die/das]", "die"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[der/die/das]", "das"));

                                    if (match.Groups["der_die_dassex"].Success)
                                        replacements.Add((match.Groups["der_die_dassex"].Index, match.Groups["der_die_dassex"].Value.Length, ""));
                                }
                                break;
                            case "ein_eine_einen":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["ein_eine_einensex"].Value, Gender.Male);

                                    if (gendercheck == Gender.Male)
                                        replacements.Add(chooseIndefinite(match, "[ein/eine/einen]", "einen"));
                                    else if (gendercheck == Gender.Female)
                                        replacements.Add(chooseIndefinite(match, "[ein/eine/einen]", "eine"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[ein/eine/einen]", "ein"));

                                    if (match.Groups["ein_eine_einensex"].Success)
                                        replacements.Add((match.Groups["ein_eine_einensex"].Index, match.Groups["ein_eine_einensex"].Value.Length, ""));
                                }
                                break;
                            case "ein_eine_ein":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["ein_eine_einsex"].Value, Gender.Male);

                                    if (gendercheck == Gender.Female)
                                        replacements.Add(chooseIndefinite(match, "[ein/eine/ein]", "eine"));
                                    else
                                        replacements.Add(chooseIndefinite(match, "[ein/eine/ein]", "ein"));

                                    if (match.Groups["ein_eine_einsex"].Success)
                                        replacements.Add((match.Groups["ein_eine_einsex"].Index, match.Groups["ein_eine_einsex"].Value.Length, ""));
                                }
                                break;
                            case "il_la":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["il_lasex"].Value, Gender.Male);
                                    string vowelcheck = match.Groups["il_laval"].Value;
                                    string postMatch = "";

                                    if (Regex.IsMatch(vowelcheck, "^([aeou]|i[bcdfghjklmnpqrstvwxyz])", RegexOptions.IgnoreCase))
                                    {
                                        int total_length = "[il/la]".Length;
                                        while (Regex.IsMatch(match.Value.Substring(total_length, 1), @"\s"))
                                            total_length++;
                                        replacements.Add((match.Index, total_length, ""));
                                        postMatch = "l'";
                                    }
                                    else
                                    {
                                        if (gendercheck == Gender.Male)
                                        {
                                            if (Regex.IsMatch(vowelcheck, "^(x|y|z|s[bcdfghjklmnpqrstvwxyz]|gn|ps|pn|i[aeiou])", RegexOptions.IgnoreCase))
                                                replacements.Add(chooseIndefinite(match, "[il/la]", "lo"));
                                            else
                                                replacements.Add(chooseIndefinite(match, "[il/la]", "il"));
                                        }
                                        else
                                        {
                                            replacements.Add(chooseIndefinite(match, "[il/la]", "la"));
                                        }
                                    }

                                    if (match.Groups["il_lasex"].Success)
                                        replacements.Add((match.Groups["il_lasex"].Index, match.Groups["il_lasex"].Value.Length, ""));

                                    if (!String.IsNullOrEmpty(postMatch))
                                        replacements.Add((match.Groups["il_laval"].Index, 0, capitalizeIndefinite(match, postMatch)));
                                }
                                break;
                            case "i_le":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["i_lesex"].Value, Gender.Male);
                                    string vowelcheck = match.Groups["i_leval"].Value;

                                    if (gendercheck == Gender.Male)
                                    {
                                        if (Regex.IsMatch(vowelcheck, "^(x|y|z|s[bcdfghjklmnpqrstvwxyz]|gn|ps|pn|[aeiou])", RegexOptions.IgnoreCase))
                                            replacements.Add(chooseIndefinite(match, "[i/le]", "gli"));
                                        else
                                            replacements.Add(chooseIndefinite(match, "[i/le]", "i"));
                                    }
                                    else
                                        replacements.Add(chooseIndefinite(match, "[i/le]", "le"));

                                    if (match.Groups["i_lesex"].Success)
                                        replacements.Add((match.Groups["i_lesex"].Index, match.Groups["i_lesex"].Value.Length, ""));
                                }
                                break;
                            case "uno_una":
                                {
                                    Gender gendercheck = extractGenderTag(match.Groups["uno_unasex"].Value, Gender.Male);
                                    string vowelcheck = match.Groups["uno_unaval"].Value;
                                    string postMatch = "";

                                    if (gendercheck == Gender.Male)
                                    {
                                        if (Regex.IsMatch(vowelcheck, "^[aeiou]", RegexOptions.IgnoreCase))
                                            replacements.Add(chooseIndefinite(match, "[uno/una]", "un"));
                                        else
                                            replacements.Add(chooseIndefinite(match, "[uno/una]", "uno"));
                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(vowelcheck, "^[aeiou]", RegexOptions.IgnoreCase))
                                        {
                                            int total_length = "[uno/una]".Length;
                                            while (Regex.IsMatch(match.Value.Substring(total_length, 1), @"\s"))
                                                total_length++;
                                            replacements.Add((match.Index, total_length, ""));
                                            postMatch = "un'";
                                        }
                                        else
                                            replacements.Add(chooseIndefinite(match, "[uno/una]", "una"));
                                    }

                                    if (match.Groups["uno_unasex"].Success)
                                        replacements.Add((match.Groups["uno_unasex"].Index, match.Groups["uno_unasex"].Value.Length, ""));

                                    if (!String.IsNullOrEmpty(postMatch))
                                        replacements.Add((match.Groups["uno_unaval"].Index, 0, capitalizeIndefinite(match, postMatch)));
                                }
                                break;
                            case "eun_neun":
                                {
                                    string vowelcheck = match.Groups["eun_neunval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[는/은]", "는"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[는/은]", "은"));
                                }
                                break;
                            case "eul_leul":
                                {
                                    string vowelcheck = match.Groups["eul_leulval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[를/을]", "를"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[를/을]", "을"));
                                }
                                break;
                            case "i_ga":
                                {
                                    string vowelcheck = match.Groups["i_gaval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[가/이]", "가"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[가/이]", "이"));
                                }
                                break;
                            case "wa_gwa":
                                {
                                    string vowelcheck = match.Groups["wa_gwaval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[과/와]", "과"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[과/와]", "와"));
                                }
                                break;
                            case "eu_lo":
                                {
                                    string vowelcheck = match.Groups["eu_loval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[로/으]", "로"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[로/으]", "으"));
                                }
                                break;
                            case "i_lamyeon":
                                {
                                    string vowelcheck = match.Groups["i_lamyeonval"].Value;
                                    char vowelchar = vowelcheck[0];
                                    if ((int)(vowelchar - '가') % 28 == 0)
                                        replacements.Add(chooseIndefiniteEnd(match, "[이/라면]", "라면"));
                                    else
                                        replacements.Add(chooseIndefiniteEnd(match, "[이/라면]", "이"));
                                }
                                break;
                        }
                    }
                }

                int reIdx = reInserts.Count - 1;
                for (int ii = replacements.Count - 1; ii >= 0; ii--)
                {
                    while (reIdx > -1)
                    {
                        if (reInserts[reIdx].Item1 < replacements[ii].idx + replacements[ii].length)
                            break;

                        output = output.Insert(reInserts[reIdx].idx, reInserts[reIdx].replace);
                        reIdx--;
                    }

                    output = output.Remove(replacements[ii].idx, replacements[ii].length);
                    output = output.Insert(replacements[ii].idx, replacements[ii].replace);
                }

                while (reIdx > -1)
                {
                    output = output.Insert(reInserts[reIdx].Item1, reInserts[reIdx].replace);
                    reIdx--;
                }

                return output;

            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return input;
        }

        private static (int, int, string) chooseIndefiniteEnd(Match match, string tag, string val)
        {
            return (match.Index + match.Length - tag.Length, tag.Length, val);
        }

        private static (int, int, string) chooseIndefinite(Match match, string tag, string val)
        {
            return (match.Index, tag.Length, capitalizeIndefinite(match, val));
        }

        private static string capitalizeIndefinite(Match match, string val)
        {
            if (char.IsUpper(match.Value[1]))
                return val.Substring(0, 1).ToUpper() + val.Substring(1);
            return val;
        }

        private static Gender extractGenderTag(string genderStr, Gender defaultGender)
        {
            switch (genderStr.ToLower())
            {
                case "[male]":
                    return Gender.Male;
                case "[female]":
                    return Gender.Female;
                case "[neutral]":
                    return Gender.Genderless;
            }

            return defaultGender;
        }

        /// <summary>
        /// Retrieves a localized string by key and formats it with the provided arguments.
        /// </summary>
        /// <param name="key">The localization string key.</param>
        /// <param name="args">Arguments to insert into the format string.</param>
        /// <returns>The formatted localized string.</returns>
        public static string FormatKey(string key, params object[] args)
        {
            try
            {
                //take a resource instead of a string, and return the localized string for it
                string text;
                if (Text.Strings.TryGetValue(key, out text))
                    return FormatGrammar(Regex.Unescape(text), args);
                throw new KeyNotFoundException(String.Format("Could not find value for {0}", key));
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return key;
        }
        /// <summary>
        /// Converts an enum value to its localized string representation.
        /// </summary>
        /// <param name="value">The enum value to localize.</param>
        /// <param name="extra">Additional suffix for the localization key, or null.</param>
        /// <returns>The localized string, or the enum's ToString() if not found.</returns>
        public static string ToLocal(this Enum value, string extra)
        {
            string key = "_ENUM_" + value.GetType().Name + "_" + value;
            if (extra != null)
                key += "_" + extra;

            string text;
            if (Text.Strings.TryGetValue(key, out text))
            {
                if (!String.IsNullOrEmpty(text))
                    return Regex.Unescape(text);
            }
    
            return value.ToString();
        }
        /// <summary>
        /// Converts an enum value to its localized string representation.
        /// </summary>
        /// <param name="value">The enum value to localize.</param>
        /// <returns>The localized string, or the enum's ToString() if not found.</returns>
        public static string ToLocal(this Enum value)
        {
            return value.ToLocal(null);
        }

        /// <summary>
        /// Converts a string to an escaped representation with Unicode escapes for non-ASCII characters.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string with Unicode escape sequences.</returns>
        public static string ToEscaped(this string str)
        {
            StringBuilder builder = new StringBuilder();
            for (int ii = 0; ii < str.Length; ii++)
            {
                if (str[ii] > 0xFF)
                    builder.Append("\\u" + ((int)str[ii]).ToString("X4"));
                else
                    builder.Append(str[ii]);

            }
            return builder.ToString();
        }

        /// <summary>
        /// Builds a grammatically correct list string from an array of items.
        /// Uses localized separators for commas and "and".
        /// </summary>
        /// <param name="input">The array of strings to join.</param>
        /// <returns>A formatted list string.</returns>
        public static string BuildList(string[] input)
        {
            StringBuilder totalString = new StringBuilder();
            for (int ii = 0; ii < input.Length; ii++)
            {
                if (ii > 0)
                {
                    if (ii == input.Length - 1)
                        totalString.Append(Text.FormatKey("ADD_END"));
                    else
                        totalString.Append(Text.FormatKey("ADD_SEPARATOR"));
                }
                totalString.Append(input[ii]);
            }
            return totalString.ToString();
        }

        /// <summary>
        /// Sets the current culture and loads localization strings for the specified language.
        /// </summary>
        /// <param name="code">The language code to set (e.g., "en", "jp").</param>
        public static void SetCultureCode(string code)
        {
            Culture = new CultureInfo(code);

            loadCulture(Strings, code, "strings");

            loadCulture(StringsEx, code, "stringsEx");
        }

        private static void loadCulture(Dictionary<string, string> strings, string code, string fileName)
        {
            strings.Clear();
            //order of string fallbacks:
            //first go through all mods of the original language
            foreach (string path in PathMod.FallbackPaths("Strings/" + fileName + "." + code + STRINGS_FILE_EXT))
            {
                Dictionary<string, string> dict = LoadStringResx(path);
                foreach (string key in dict.Keys)
                {
                    if (!strings.ContainsKey(key))
                        strings.Add(key, dict[key]);
                }
            }

            //then go through all mods of the official fallbacks
            if (LangNames.ContainsKey(code))
            {
                foreach (string fallback in LangNames[code].Fallbacks)
                {
                    foreach (string path in PathMod.FallbackPaths("Strings/" + fileName + "." + fallback + STRINGS_FILE_EXT))
                    {
                        Dictionary<string, string> dict = LoadStringResx(path);
                        foreach (string key in dict.Keys)
                        {
                            if (!strings.ContainsKey(key))
                                strings.Add(key, dict[key]);
                        }
                    }
                }
            }
            //then go through all mods of the default language
            foreach (string path in PathMod.FallbackPaths("Strings/" + fileName + STRINGS_FILE_EXT))
            {
                Dictionary<string, string> dict = LoadStringResx(path);
                foreach (string key in dict.Keys)
                {
                    if (!strings.ContainsKey(key))
                        strings.Add(key, dict[key]);
                }
            }
        }

        /// <summary>
        /// Gets the language-specific version of a file path.
        /// </summary>
        /// <param name="basePath">The base file path.</param>
        /// <param name="cultureCode">The culture code to append.</param>
        /// <returns>The path with the culture code inserted before the extension.</returns>
        public static string GetLanguagedPath(string basePath, string cultureCode)
        {
            if (String.IsNullOrEmpty(cultureCode))
                return basePath;

            string dir = Path.GetDirectoryName(basePath);
            string noExt = Path.GetFileNameWithoutExtension(basePath);
            string ext = Path.GetExtension(basePath);
            return Path.Join(dir, noExt + "." + cultureCode + ext);
        }

        /// <summary>
        /// Gets the best available language-specific path, falling back through the language chain.
        /// </summary>
        /// <param name="basePath">The base file path.</param>
        /// <returns>The best matching language path that exists, or the base path.</returns>
        public static string ModLangPath(string basePath)
        {
            string cultureCode = Culture.Name.ToLower();
            string langPath = GetLanguagedPath(basePath, cultureCode);
            if (File.Exists(langPath) || Directory.Exists(langPath))
                return langPath;
            foreach (string fallback in LangNames[cultureCode].Fallbacks)
            {
                langPath = GetLanguagedPath(basePath, cultureCode);
                if (File.Exists(langPath) || Directory.Exists(langPath))
                    return langPath;
            }

            return basePath;
        }

        /// <summary>
        /// Sanitizes a string for use as a filename by removing diacritics and special characters.
        /// </summary>
        /// <param name="input">The string to sanitize.</param>
        /// <returns>A sanitized string safe for use as a filename.</returns>
        public static string Sanitize(string input)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = input.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            string result = Regex.Replace(sbReturn.ToString(), "[':.]", "");
            result = Regex.Replace(result, "\\W", "_");
            return result;
        }

        /// <summary>
        /// Generates a unique name by appending a numeric suffix if needed.
        /// </summary>
        /// <param name="inputStr">The desired name.</param>
        /// <param name="getConflict">A function that returns true if the name conflicts.</param>
        /// <returns>A non-conflicting name, or null if no valid name could be found.</returns>
        public static string GetNonConflictingName(string inputStr, Func<string, bool> getConflict)
        {
            string prefix = inputStr;
            int origIndex;
            int lastUnderscore = inputStr.LastIndexOf('_');
            if (lastUnderscore > -1)
            {
                string substr = inputStr.Substring(lastUnderscore + 1);
                if (int.TryParse(substr, out origIndex))
                    prefix = inputStr.Substring(0, lastUnderscore);
            }

            if (!getConflict(inputStr))
                return inputStr;

            int copy_index = 1;
            while (copy_index < Int32.MaxValue)
            {
                if (!getConflict(prefix + "_" + copy_index.ToString()))
                    return prefix + "_" + copy_index.ToString();

                copy_index++;
            }

            return null;
        }

        /// <summary>
        /// Gets a non-conflicting save file path by appending a numeric suffix if needed.
        /// </summary>
        /// <param name="folderPath">The folder path for the save file.</param>
        /// <param name="fileName">The desired file name without extension.</param>
        /// <param name="fileExtension">The file extension including the dot.</param>
        /// <returns>A non-conflicting file name (without path or extension).</returns>
        public static string GetNonConflictingSavePath(string folderPath, string fileName, string fileExtension)
        {
            bool savePathExists(string name)
            {
                return File.Exists(folderPath + name + fileExtension);
            };

            return Text.GetNonConflictingName(fileName, savePathExists);
        }

        /// <summary>
        /// Converts a PascalCase member name to a human-readable title with spaces.
        /// </summary>
        /// <param name="name">The PascalCase name to convert.</param>
        /// <returns>The name with spaces inserted between word boundaries.</returns>
        public static string GetMemberTitle(string name)
        {
            StringBuilder separatedName = new StringBuilder();
            for (int ii = 0; ii < name.Length; ii++)
            {
                if (ii > 0)
                {
                    bool space = false;
                    if (char.IsDigit(name[ii]) && char.IsLetter(name[ii - 1]))
                        space = true;
                    if (char.IsUpper(name[ii]) && char.IsLower(name[ii - 1]) || char.IsUpper(name[ii]) && char.IsDigit(name[ii - 1]))
                        space = true;
                    if (char.IsUpper(name[ii]) && char.IsUpper(name[ii - 1]) && ii < name.Length - 1 && char.IsLower(name[ii + 1]))
                        space = true;
                    if (space)
                        separatedName.Append(' ');
                }
                separatedName.Append(name[ii]);
            }
            return separatedName.ToString();
        }

        /// <summary>
        /// Computes a deterministic hash code for a string that is consistent across runs.
        /// Unlike GetHashCode(), this produces the same value every time.
        /// </summary>
        /// <param name="str">The string to hash.</param>
        /// <returns>A deterministic 32-bit hash value.</returns>
        public static int DeterministicHash(string str)
        {
            //TODO: we don't know if this is consistent between 32bit and 64bit machines...
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }



    /// <summary>
    /// A serializable key for looking up extended localization strings.
    /// Provides methods to retrieve the localized value from StringsEx.
    /// </summary>
    [Serializable]
    public struct StringKey
    {
        /// <summary>
        /// The localization key string.
        /// </summary>
        public string Key;

        /// <summary>
        /// Initializes a new StringKey with the specified key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        public StringKey(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Retrieves the localized value for this key from StringsEx.
        /// </summary>
        /// <returns>The localized string, or the key itself if not found.</returns>
        public string ToLocal()
        {
            try
            {
                string val;
                if (Text.StringsEx.TryGetValue(Key, out val))
                    return Regex.Unescape(val);
                throw new KeyNotFoundException(String.Format("Could not find value for {0}", Key));
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return Key;
        }

        /// <summary>
        /// Returns the key as a string.
        /// </summary>
        /// <returns>The key, or an empty string if null.</returns>
        public override string ToString()
        {
            if (Key != null)
                return Key;
            return "";
        }

        /// <summary>
        /// Checks whether this key has a valid, non-empty value.
        /// </summary>
        /// <returns>True if the key is not null or whitespace.</returns>
        public bool IsValid()
        {
            return !String.IsNullOrWhiteSpace(Key);
        }

        /// <summary>
        /// Checks whether a localization value exists for the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists in StringsEx.</returns>
        public static bool HasValue(string key)
        {
            return Text.StringsEx.ContainsKey(key);
        }
    }


    /// <summary>
    /// Abstract base class for localized format strings with variable arguments.
    /// Subclasses define how arguments are provided and formatted.
    /// </summary>
    [Serializable]
    public abstract class LocalFormat
    {
        /// <summary>
        /// The localization key for the format string.
        /// </summary>
        public StringKey Key;

        /// <summary>
        /// Initializes a new instance with an empty key.
        /// </summary>
        public LocalFormat() { Key = new StringKey(""); }

        /// <summary>
        /// Initializes a new instance by copying from another LocalFormat.
        /// </summary>
        /// <param name="other">The LocalFormat to copy from.</param>
        public LocalFormat(LocalFormat other) { Key = other.Key; }

        /// <summary>
        /// Creates a deep copy of this LocalFormat.
        /// </summary>
        /// <returns>A new LocalFormat instance with the same values.</returns>
        public abstract LocalFormat Clone();

        /// <summary>
        /// Formats and returns the localized string with arguments inserted.
        /// </summary>
        /// <returns>The formatted localized string.</returns>
        public abstract string FormatLocal();
    }

    /// <summary>
    /// Provides localized text formatting with enum values as arguments.
    /// Each enum value is converted to its localized string representation.
    /// </summary>
    /// <typeparam name="T">The enum type for the arguments.</typeparam>
    [Serializable]
    public class LocalFormatEnum<T> : LocalFormat where T : Enum
    {
        /// <summary>
        /// The list of enum values to use as format arguments.
        /// </summary>
        public List<T> Enums;

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public LocalFormatEnum() { Enums = new List<T>(); }

        /// <summary>
        /// Initializes a new instance by copying from another LocalFormatEnum.
        /// </summary>
        /// <param name="other">The LocalFormatEnum to copy from.</param>
        public LocalFormatEnum(LocalFormatEnum<T> other) : base(other)
        {
            Enums = new List<T>();
            Enums.AddRange(other.Enums);
        }

        /// <summary>
        /// Creates a deep copy of this LocalFormatEnum.
        /// </summary>
        /// <returns>A new LocalFormatEnum instance with the same values.</returns>
        public override LocalFormat Clone() { return new LocalFormatEnum<T>(this); }

        /// <summary>
        /// Formats the localized string with the enum values converted to localized strings.
        /// </summary>
        /// <returns>The formatted localized string.</returns>
        public override string FormatLocal()
        {
            List<string> enumStrings = new List<string>();
            foreach (T t in Enums)
                enumStrings.Add(t.ToLocal());
            return Text.FormatGrammar(Key.ToLocal(), enumStrings.ToArray());
        }
    }

    /// <summary>
    /// Provides localized text formatting with StringKey arguments.
    /// Each argument key is resolved to its localized value before formatting.
    /// </summary>
    [Serializable]
    public class LocalFormatSimple : LocalFormat
    {
        /// <summary>
        /// The list of StringKey arguments to insert into the format string.
        /// </summary>
        public List<StringKey> Args;

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public LocalFormatSimple() { Args = new List<StringKey>(); }

        /// <summary>
        /// Initializes a new instance with the specified key and string arguments.
        /// </summary>
        /// <param name="keyString">The localization key for the format string.</param>
        /// <param name="args">The argument keys as strings.</param>
        public LocalFormatSimple(string keyString, params string[] args)
        {
            Key = new StringKey(keyString);
            Args = new List<StringKey>();
            foreach (string arg in args)
                Args.Add(new StringKey(arg));
        }
        /// <summary>
        /// Initializes a new instance with the specified key and StringKey arguments.
        /// </summary>
        /// <param name="key">The localization key for the format string.</param>
        /// <param name="args">The argument keys as StringKeys.</param>
        public LocalFormatSimple(StringKey key, params StringKey[] args)
        {
            Key = key;
            Args = new List<StringKey>();
            foreach (StringKey arg in args)
                Args.Add(arg);
        }

        /// <summary>
        /// Initializes a new instance by copying from another LocalFormatSimple.
        /// </summary>
        /// <param name="other">The LocalFormatSimple to copy from.</param>
        public LocalFormatSimple(LocalFormatSimple other) : base(other)
        {
            Args = new List<StringKey>();
            Args.AddRange(other.Args);
        }

        /// <summary>
        /// Creates a deep copy of this LocalFormatSimple.
        /// </summary>
        /// <returns>A new LocalFormatSimple instance with the same values.</returns>
        public override LocalFormat Clone() { return new LocalFormatSimple(this); }

        /// <summary>
        /// Formats the localized string with the argument keys resolved to their localized values.
        /// </summary>
        /// <returns>The formatted localized string.</returns>
        public override string FormatLocal()
        {
            object[] args = new object[Args.Count];
            for (int ii = 0; ii < args.Length; ii++)
                args[ii] = Args[ii].ToLocal();
            return Text.FormatGrammar(Key.ToLocal(), args);
        }
    }

    /// <summary>
    /// Represents text that can be localized into multiple languages.
    /// Stores a default text and a dictionary of translations keyed by language code.
    /// </summary>
    [Serializable]
    public class LocalText
    {
        /// <summary>
        /// The default text used when no translation is available.
        /// </summary>
        public string DefaultText;

        /// <summary>
        /// Dictionary mapping language codes to translated text.
        /// </summary>
        public Dictionary<string, string> LocalTexts;

        /// <summary>
        /// Initializes a new empty LocalText instance.
        /// </summary>
        public LocalText()
        {
            DefaultText = "";
            LocalTexts = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new LocalText with the specified default text.
        /// </summary>
        /// <param name="defaultText">The default text.</param>
        public LocalText(string defaultText)
        {
            DefaultText = defaultText;
            LocalTexts = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new LocalText by copying from another instance.
        /// </summary>
        /// <param name="other">The LocalText to copy from.</param>
        public LocalText(LocalText other)
        {
            DefaultText = other.DefaultText;
            LocalTexts = new Dictionary<string, string>();
            foreach (string key in other.LocalTexts.Keys)
                LocalTexts.Add(key, other.LocalTexts[key]);
        }
        /// <summary>
        /// Initializes a new LocalText by copying and formatting with string arguments.
        /// </summary>
        /// <param name="other">The LocalText format template to copy from.</param>
        /// <param name="args">The arguments to insert into the format string.</param>
        public LocalText(LocalText other, string[] args)
        {
            DefaultText = Text.FormatGrammar(other.DefaultText, args);
            LocalTexts = new Dictionary<string, string>();
            foreach (string key in other.LocalTexts.Keys)
                LocalTexts.Add(key, Text.FormatGrammar(other.LocalTexts[key], args));
        }
        /// <summary>
        /// Initializes a new LocalText by copying and formatting with LocalText arguments.
        /// Each language is formatted independently using the appropriate translation.
        /// </summary>
        /// <param name="other">The LocalText format template to copy from.</param>
        /// <param name="args">The LocalText arguments to insert.</param>
        public LocalText(LocalText other, LocalText[] args)
        {
            string[] defaultArgs = new string[args.Length];
            for (int ii = 0; ii < args.Length; ii++)
                defaultArgs[ii] = args[ii].DefaultText;
            DefaultText = Text.FormatGrammar(other.DefaultText, defaultArgs);
            LocalTexts = new Dictionary<string, string>();
            foreach (string key in other.LocalTexts.Keys)
            {
                string[] localArgs = new string[args.Length];
                for (int ii = 0; ii < args.Length; ii++)
                {
                    if (args[ii].LocalTexts.ContainsKey(key))
                        localArgs[ii] = args[ii].LocalTexts[key];
                    else//if there is no translation for this string argument, fall back on default text.
                        localArgs[ii] = args[ii].DefaultText;
                }
                LocalTexts.Add(key, Text.FormatGrammar(other.LocalTexts[key], localArgs));
            }
        }
        /// <summary>
        /// Creates a new LocalText by formatting the template with string arguments.
        /// </summary>
        /// <param name="format">The format template.</param>
        /// <param name="args">The string arguments to insert.</param>
        /// <returns>A new formatted LocalText.</returns>
        public static LocalText FormatLocalText(LocalText format, params string[] args)
        {
            return new LocalText(format, args);
        }

        /// <summary>
        /// Creates a new LocalText by formatting the template with LocalText arguments.
        /// </summary>
        /// <param name="format">The format template.</param>
        /// <param name="args">The LocalText arguments to insert.</param>
        /// <returns>A new formatted LocalText.</returns>
        public static LocalText FormatLocalText(LocalText format, params LocalText[] args)
        {
            return new LocalText(format, args);
        }

        /// <summary>
        /// Gets the localized text for the current culture, falling back through the language chain.
        /// </summary>
        /// <returns>The localized text, or the default text if no translation is found.</returns>
        public string ToLocal()
        {
            string text;
            if (LocalTexts.TryGetValue(Text.Culture.Name.ToLower(), out text))
                return Regex.Unescape(text);

            if (Text.LangNames.ContainsKey(Text.Culture.Name))
            {
                foreach (string fallback in Text.LangNames[Text.Culture.Name].Fallbacks)
                {
                    if (LocalTexts.TryGetValue(fallback, out text))
                        return Regex.Unescape(text);
                }
            }

            return Regex.Unescape(DefaultText);
        }

        /// <summary>
        /// Returns the default text as a string.
        /// </summary>
        /// <returns>The default text.</returns>
        public override string ToString()
        {
            return DefaultText;
        }
    }

}
