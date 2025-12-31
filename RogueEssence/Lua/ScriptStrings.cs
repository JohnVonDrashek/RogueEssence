using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NLua;
using System.IO;

namespace RogueEssence.Script
{
    /// <summary>
    /// Class for handling loading localized string via xml, because lua xml libs sucks
    /// </summary>
    public class ScriptStrings : ILuaEngineComponent
    {
        /// <summary>
        /// Gets the currently loaded map strings as a Lua table.
        /// </summary>
        public LuaTable MapStrings { get; private set; }

        /// <summary>
        /// Creates a Lua table from a package string table file.
        /// </summary>
        /// <param name="packagefilepath">The relative path to the package.</param>
        /// <returns>The loaded string table as a Lua table.</returns>
        public LuaTable MakePackageStringTable(string packagefilepath)
        {
            return MapStrings;
        }

        private LuaTable makePackageStringTable(string packagefilepath)
        {
            try
            {
                Dictionary<string, string> xmlDict = Text.LoadScriptStringDict(LocaleCode(), LuaEngine.SCRIPT_PATH, packagefilepath);

                //Build a lua table as we go and return it
                LuaTable tbl = LuaEngine.Instance.RunString("return {}").First() as LuaTable;
                LuaFunction addfn = LuaEngine.Instance.RunString("return function(tbl, key, str) tbl[key] = str end").First() as LuaFunction;

                foreach (string name in xmlDict.Keys)
                    addfn.Call(tbl, name, xmlDict[name]);

                return tbl;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                return LuaEngine.Instance.RunString("return {}").First() as LuaTable;
            }
        }

        /// <summary>
        /// Loads a package string table and stores it in MapStrings.
        /// </summary>
        /// <param name="packagefilepath">The relative path to the package.</param>
        public void LoadPackageStringTable(string packagefilepath)
        {
            LuaTable strings = makePackageStringTable(packagefilepath);
            MapStrings = strings;
        }


        /// <summary>
        /// Gets the current language setting of the game.
        /// </summary>
        /// <returns>The current language, represented by a locale code.</returns>
        public string LocaleCode()
        {
            return DiagManager.Instance.CurSettings.Language;
        }


        /// <summary>
        /// Shifts all non-whitespace characters in a string by a specified amount.
        /// </summary>
        /// <param name="baseStr">The base string to shift.</param>
        /// <param name="shift">The amount to shift each character by.</param>
        /// <returns>The shifted string.</returns>
        public string ShiftString(string baseStr, int shift)
        {
            string resultStr = "";
            for (int ii = 0; ii < baseStr.Length; ii++)
            {
                if (!string.IsNullOrWhiteSpace(baseStr[ii].ToString()))
                {
                    int en = (int)baseStr[ii];
                    int un = en + shift;
                    resultStr = resultStr + (char)un;
                }
                else
                    resultStr += baseStr[ii];
            }
            return resultStr;
        }
        /// <summary>
        /// Formats a string.  Will unescape escaped characters and process grammar tags.
        /// </summary>
        /// <param name="fmt">String to format.</param>
        /// <param name="para">Arguments</param>
        /// <returns>The formatted string.</returns>
        public string Format( string fmt, params object[] para )
        {
            try
            {
                return Text.FormatGrammar(System.Text.RegularExpressions.Regex.Unescape(fmt), para);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return fmt;
        }

        /// <summary>
        /// Formats a string given a string key.  Will unescape escaped characters.
        /// </summary>
        /// <param name="fmt">The string key to format</param>
        /// <param name="para">string arguments</param>
        /// <returns></returns>
        public string FormatKey(string fmt, params object[] para)
        {
            try
            {
                return Text.FormatKey(fmt, para);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return fmt;
        }

        /// <summary>
        /// Gets the string representing a button or key.
        /// </summary>
        /// <param name="index">The input type of the input.</param>
        /// <returns>The string representing the button o key the input maps to.</returns>
        public string LocalKeyString(int index)
        {
            try
            {
                return DiagManager.Instance.GetControlString((FrameInput.InputType)index);//TODO: LUA 5.3: do regex unescape here
                //return System.Text.RegularExpressions.Regex.Unescape(DiagManager.Instance.CurSettings.ActionKeys[index].ToLocal());
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return ((FrameInput.InputType)index).ToString();
        }

        /// <summary>
        /// Builds a single string of comma separated elements.
        /// </summary>
        /// <param name="listStrings">LuaTable of strings containing the elements to build the list from.</param>
        /// <returns>The combined string containing all elements.</returns>
        public string CreateList(LuaTable listStrings)
        {
            try
            {
                List<string> para = new List<string>();
                foreach (object key in listStrings.Keys)
                {
                    string entry = (string)listStrings[key];
                    para.Add(entry);
                }
                return System.Text.RegularExpressions.Regex.Unescape(Text.BuildList(para.ToArray()));
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return "";
        }

        /// <summary>
        /// Sets up Lua function wrappers for string operations.
        /// </summary>
        /// <param name="state">The Lua engine state.</param>
        public override void SetupLuaFunctions(LuaEngine state)
        {
            //TODO
        }
    }
}
