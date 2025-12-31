using NLua;
using RogueEssence.Script;
using System;
using System.Linq;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// An AI plan that executes Lua scripts to determine character behavior.
    /// Allows for flexible, script-driven AI logic without requiring compiled code changes.
    /// </summary>
    [Serializable]
    public class ScriptPlan : BasePlan
    {
        /// <summary>
        /// The name of the Lua function to call for determining the next action.
        /// </summary>
        [Dev.Sanitize(0)]
        public string ThinkScript;

        /// <summary>
        /// Arguments passed to the ThinkScript as a Lua table string.
        /// </summary>
        [Dev.Multiline(0)]
        public string ThinkArgTable;

        /// <summary>
        /// The name of the Lua function to call during initialization.
        /// </summary>
        [Dev.Sanitize(0)]
        public string InitializeScript;

        /// <summary>
        /// Arguments passed to the InitializeScript as a Lua table string.
        /// </summary>
        [Dev.Multiline(0)]
        public string InitializeArgTable;

        /// <summary>
        /// The name of the Lua function to call when this plan becomes active.
        /// </summary>
        [Dev.Sanitize(0)]
        public string SwitchedInScript;

        /// <summary>
        /// Arguments passed to the SwitchedInScript as a Lua table string.
        /// </summary>
        [Dev.Multiline(0)]
        public string SwitchedInArgTable;

        [NonSerialized]
        private LuaTable luaTable;

        /// <summary>
        /// Initializes a new instance of the ScriptPlan class with empty scripts.
        /// </summary>
        public ScriptPlan()
        {
            luaTable = LuaEngine.Instance.RunString("return {}").First() as LuaTable;
            ThinkScript = ""; ThinkArgTable = "{}";
            InitializeScript = ""; InitializeArgTable = "{}";
            SwitchedInScript = ""; SwitchedInArgTable = "{}";
        }

        /// <summary>
        /// Initializes a new instance of the ScriptPlan class with only a think script.
        /// </summary>
        /// <param name="script">The name of the think script function.</param>
        public ScriptPlan(string script)
        {
            luaTable = LuaEngine.Instance.RunString("return {}").First() as LuaTable;
            ThinkScript = script; ThinkArgTable = "{}";
            InitializeScript = ""; InitializeArgTable = "{}";
            SwitchedInScript = ""; SwitchedInArgTable = "{}";
        }

        /// <summary>
        /// Initializes a new instance of the ScriptPlan class with a think script and arguments.
        /// </summary>
        /// <param name="think">The name of the think script function.</param>
        /// <param name="thinkTable">Arguments for the think script as a Lua table string.</param>
        public ScriptPlan(string think, string thinkTable)
        {
            luaTable = LuaEngine.Instance.RunString("return {}").First() as LuaTable;
            ThinkScript = think; ThinkArgTable = thinkTable;
            InitializeScript = ""; InitializeArgTable = "{}";
            SwitchedInScript = ""; SwitchedInArgTable = "{}";
        }

        /// <summary>
        /// Initializes a new instance of the ScriptPlan class with all script handlers.
        /// </summary>
        /// <param name="think">The name of the think script function.</param>
        /// <param name="thinkTable">Arguments for the think script as a Lua table string.</param>
        /// <param name="init">The name of the initialization script function.</param>
        /// <param name="initTable">Arguments for the init script as a Lua table string.</param>
        /// <param name="switched">The name of the switched-in script function.</param>
        /// <param name="switchedTable">Arguments for the switched script as a Lua table string.</param>
        public ScriptPlan(string think, string thinkTable, string init, string initTable, string switched, string switchedTable)
        {
            luaTable = LuaEngine.Instance.RunString("return {}").First() as LuaTable;
            ThinkScript = think; ThinkArgTable = thinkTable;
            InitializeScript = init; InitializeArgTable = initTable;
            SwitchedInScript = switched; SwitchedInArgTable = switchedTable;
        }

        /// <summary>
        /// Creates a copy of another ScriptPlan.
        /// </summary>
        /// <param name="other">The ScriptPlan to copy from.</param>
        protected ScriptPlan(ScriptPlan other)
        {
            luaTable = other.luaTable;
            ThinkScript = other.ThinkScript;
            ThinkArgTable = other.ThinkArgTable;
            InitializeScript = other.InitializeScript;
            InitializeArgTable = other.InitializeArgTable;
            SwitchedInScript = other.SwitchedInScript;
            SwitchedInArgTable = other.SwitchedInArgTable;
        }

        /// <inheritdoc/>
        public override BasePlan CreateNew() { return new ScriptPlan(this); }

        /// <summary>
        /// Executes the initialization Lua script if one is defined.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI plan.</param>
        public override void Initialize(Character controlledChar)
        {
            if (string.IsNullOrEmpty(InitializeScript)) return;
            LuaTable args = LuaEngine.Instance.RunString("return " + InitializeArgTable).First() as LuaTable;
            string name = LuaEngine.EVENT_AI_INIT_NAME + "." + InitializeScript;
            LuaEngine.Instance.CallLuaFunctions(name, controlledChar, luaTable, args );
        }

        /// <summary>
        /// Executes the switched-in Lua script if one is defined.
        /// </summary>
        /// <param name="currentPlan">The plan that was previously active.</param>
        public override void SwitchedIn(BasePlan currentPlan)
        {
            if (string.IsNullOrEmpty(SwitchedInScript)) return;
            LuaTable args = LuaEngine.Instance.RunString("return " + SwitchedInArgTable).First() as LuaTable;
            string name = LuaEngine.EVENT_AI_SWITCH_NAME + "." + SwitchedInScript;
            LuaEngine.Instance.CallLuaFunctions(name, currentPlan, luaTable, args);
        }

        /// <summary>
        /// Executes the think Lua script to determine the next action.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI plan.</param>
        /// <param name="preThink">Whether this is a pre-think phase before the actual turn.</param>
        /// <param name="rand">Random number generator for decision making.</param>
        /// <returns>The GameAction returned by the script, or null if no action is determined.</returns>
        /// <exception cref="Exception">Thrown when the script returns an unexpected type.</exception>
        public override GameAction Think(Character controlledChar, bool preThink, IRandom rand)
        {
            LuaTable args = LuaEngine.Instance.RunString("return " + ThinkArgTable).First() as LuaTable;
            string name = LuaEngine.EVENT_AI_THINK_NAME + "." + ThinkScript;
            object result = null;
            object[] res = LuaEngine.Instance.CallLuaFunctions(name, controlledChar, preThink, rand, luaTable, args);
            if (res is not null && res.Length > 0) result = res.First();

            if (result is GameAction action) return action;
            if (result == null) return null;
            throw new Exception("Expected return types for \"" + LuaEngine.EVENT_AI_THINK_NAME + "\" functions are: \"RogueEssence.Dungeon.GameAction\", nil. \"" + name + "\" returned a " + result.GetType().ToString());
        }

    }
}