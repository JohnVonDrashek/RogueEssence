
using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Dungeon;
using RogueEssence.LevelGen;
using RogueEssence.Dev;
using RogueEssence.Script;
using NLua;
using System.Linq;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Calls a lua script function that acts as the map gen step.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ScriptGenStep<T> : GenStep<T> where T : BaseMapGenContext
    {
        /// <summary>
        /// The name of the script.  The resulting function will be FLOOR_GEN_SCRIPT.[Script Name]
        /// </summary>
        [Dev.Sanitize(0)]
        public string Script;

        /// <summary>
        /// Additional arguments that will be passed into the script.
        /// </summary>
        [Multiline(0)]
        public string ArgTable;

        /// <summary>
        /// Initializes a new instance of the ScriptGenStep class.
        /// </summary>
        public ScriptGenStep() { Script = ""; ArgTable = "{}"; }

        /// <summary>
        /// Initializes a new instance of the ScriptGenStep class with the specified script.
        /// </summary>
        /// <param name="script">The script function name.</param>
        public ScriptGenStep(string script) { Script = script; ArgTable = "{}"; }

        /// <summary>
        /// Initializes a new instance of the ScriptGenStep class with the specified script and arguments.
        /// </summary>
        /// <param name="script">The script function name.</param>
        /// <param name="argTable">The Lua argument table string.</param>
        public ScriptGenStep(string script, string argTable) { Script = script; ArgTable = argTable; }

        /// <summary>
        /// Applies the script generation step by calling the specified Lua function.
        /// </summary>
        /// <param name="map">The map generation context to pass to the script.</param>
        public override void Apply(T map)
        {
            LuaFunction luafun = LuaEngine.Instance.LuaState.GetFunction(LuaEngine.EVENT_FLOORGEN_NAME + "." + Script);

            if (luafun != null)
            {
                LuaTable args = LuaEngine.Instance.RunString("return " + ArgTable).First() as LuaTable;
                luafun.Call(new object[] { map, args });
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), Script);
        }
    }
}
