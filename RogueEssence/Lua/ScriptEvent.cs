/*
 * ScriptCoroutines.cs
 * 2017/07/01
 * psycommando@gmail.com

 */
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RogueEssence.Script
{
    /// <summary>
    /// An event which calls a script function when triggered!
    /// </summary>
    [Serializable]
    public class ScriptEvent
    {
        protected string m_luapath;

        /// <summary>
        /// Gets the name of this event, which is the Lua function path.
        /// </summary>
        /// <returns>The Lua function path string.</returns>
        public virtual string EventName()
        {
            return m_luapath;
        }

        /// <summary>
        /// Protected blank constructor, for child classes
        /// </summary>
        protected ScriptEvent()
        {
            m_luapath = null;
        }

        /// <summary>
        /// Creates a new script event that calls the specified Lua function.
        /// </summary>
        /// <param name="luafunpath">The path to the Lua function to call.</param>
        public ScriptEvent(string luafunpath)
        {
            SetLuaFunctionPath(luafunpath);
        }

        /// <summary>
        /// Sets the Lua function path for this event.
        /// </summary>
        /// <param name="luafunpath">The path to the Lua function to call.</param>
        public void SetLuaFunctionPath(string luafunpath)
        {
            m_luapath = luafunpath;
            bool func_valid = LuaEngine.Instance.DoesFunctionExists(m_luapath); //Make an initial check for that, and keeps the event from running
            if (!func_valid && m_luapath != null)
                DiagManager.Instance.LogInfo(String.Format("ScriptEvent(): Lua function '{0}' does not exists. The event will not run!", m_luapath));
        }

        /// <summary>
        /// Creates a copy of this script event.
        /// </summary>
        /// <returns>A new ScriptEvent with the same Lua function path.</returns>
        public virtual ScriptEvent Clone()
        {
            return new ScriptEvent(m_luapath);
        }

        /// <summary>
        /// Called when the event is about to be removed from the context. Add everything that needs to be done before the event is removed in here.
        /// </summary>
        public virtual void DoCleanup()
        {
            DiagManager.Instance.LogInfo(String.Format("ScriptEvent.DoCleanup(): Doing cleanup on {0}!", m_luapath));
        }

        /// <summary>
        /// Executes this event by calling the Lua function as a coroutine.
        /// </summary>
        /// <param name="parameters">Parameters to pass to the Lua function.</param>
        /// <returns>A coroutine representing the event execution.</returns>
        public virtual Coroutine Apply(params object[] parameters)
        {
            LuaFunction func_iter = LuaEngine.Instance.CreateCoroutineIterator(m_luapath, parameters);
            return new LuaCoroutine(String.Format("{0}: {1}", typeof(ScriptEvent).Name, m_luapath), applyFunc(m_luapath, func_iter));
        }


        /// <summary>
        /// Creates a coroutine from a Lua iterator function.
        /// </summary>
        /// <param name="name">The name of the function for logging purposes.</param>
        /// <param name="func_iter">The Lua iterator function to execute.</param>
        /// <returns>A coroutine that executes the Lua function.</returns>
        public static Coroutine ApplyFunc(string name, LuaFunction func_iter)
        {
            return new LuaCoroutine(String.Format("{0}: {1}", typeof(ScriptEvent).Name, name), applyFunc(name, func_iter));
        }

        private static IEnumerator<YieldInstruction> applyFunc(string name, LuaFunction func_iter)
        {
            if (func_iter == null)
                yield break;

            //Then call it until it returns null!
            object[] allres = callInternal(name, func_iter);
            object res = allres.First();
            while (res != null)
            {
                if (res.GetType() == typeof(Coroutine)) //This handles waiting on coroutines
                    yield return CoroutineManager.Instance.StartCoroutine(res as Coroutine, false);
                else if (res.GetType().IsSubclassOf(typeof(YieldInstruction)))
                    yield return res as YieldInstruction;

                //In case we get the order to break out of this, do it! This will happen when the engine is reloaded, and we don't want
                // to be running a coroutine when this happens!
                if (LuaEngine.Instance.Breaking)
                {
                    DiagManager.Instance.LogInfo(String.Format("ScriptEvent.Apply(): Coroutine for event interrupted by break command!"));
                    break;
                }

                //Pick another yield from the lua coroutine
                allres = callInternal(name, func_iter);
                res = allres.First();
            }
        }

        /// <summary>
        /// Wrapper around the lua iterator to catch and print any possible script errors.
        /// </summary>
        /// <returns></returns>
        private static object[] callInternal(string name, LuaFunction func_internal)
        {
            try
            {
                return func_internal.Call();
            }
            catch (Exception e)
            {
                DiagManager.Instance.LogError(new Exception(String.Format("[SE]:ScriptEvent.CallInternal(): Error calling coroutine iterator in {0}:\n{1}", name, e.Message), e));
            }
            return new object[] { null }; //Stop the coroutine since we errored
        }

        /// <summary>
        /// Looks into the current lua state for the function corresponding to this event's stored luapath, and determines if it can
        /// be run or not.
        /// This should be called only after the corresponding map script has been loaded, otherwise it won't find its matching function.
        /// </summary>
        public virtual void ReloadEvent()
        {
            DiagManager.Instance.LogInfo(String.Format("ScriptEvent.ReloadEvent(): Reloading event {0}!", m_luapath));
            bool func_valid = LuaEngine.Instance.DoesFunctionExists(m_luapath); //Make an initial check for that, and keeps the event from running
            if (!func_valid)
                DiagManager.Instance.LogInfo(String.Format("ScriptEvent.ReloadEvent(): Lua function '{0}' does not exists. The event will not run!", m_luapath));
        }

        /// <summary>
        /// Called when the Lua engine is reloaded.
        /// Since all Lua references are invalidated, this re-validates the function path.
        /// </summary>
        public virtual void LuaEngineReload()
        {
            DiagManager.Instance.LogInfo(String.Format("ScriptEvent.OnLuaEngineReload(): Reloading event {0}, and interrupting current coroutine!", m_luapath));
            ReloadEvent();
        }

    }


    /// <summary>
    /// TransientScriptEvent is a script event that's not meant to be serialized,
    /// and runs for a short period of time.
    /// As a result, it can take directly a lua function as construction parameter!
    /// </summary>
    public class TransientScriptEvent : ScriptEvent
    {
        [NonSerialized] private LuaFunction m_luafun = null;

        /// <summary>
        /// Creates a transient script event from a Lua function reference.
        /// </summary>
        /// <param name="luafun">The Lua function to execute.</param>
        public TransientScriptEvent(LuaFunction luafun)
        {
            m_luafun = luafun;
            bool func_valid = luafun != null;
            if (!func_valid)
                DiagManager.Instance.LogInfo("TransientScriptEvent.TransientScriptEvent(): lua function passed as parameter is null!");
        }
        /// <summary>
        /// Creates a transient script event from a Lua function path string.
        /// </summary>
        /// <param name="luafun">The Lua code that returns a function.</param>
        public TransientScriptEvent(string luafun)
        {
            m_luafun = LuaEngine.Instance.RunString("return " + luafun).First() as LuaFunction;
            bool func_valid = luafun != null;
            if (!func_valid)
                DiagManager.Instance.LogInfo("TransientScriptEvent.TransientScriptEvent(): lua function path passed as parameter is invalid!");
        }

        /// <summary>
        /// Gets the name of this event.
        /// </summary>
        /// <returns>The string representation of the Lua function.</returns>
        public override string EventName()
        {
            return m_luafun.ToString();
        }


        /// <summary>
        /// Executes this transient event by calling the stored Lua function.
        /// </summary>
        /// <param name="parameters">Parameters to pass to the Lua function.</param>
        /// <returns>A coroutine representing the event execution.</returns>
        /// <exception cref="Exception">Thrown if the Lua function is null.</exception>
        public override Coroutine Apply(params object[] parameters)
        {
            if (m_luafun == null)
                throw new Exception("TransientScriptEvent.MakeIterator(): Function is null! Make sure the transientevent isn't being deserialized and run!");
            LuaFunction func_iter = LuaEngine.Instance.CreateCoroutineIterator(m_luafun, parameters);

            return ApplyFunc(m_luapath, func_iter);
        }
    }

}
