using System;
using System.Collections.Generic;
using System.Linq;
using NLua;
using System.Collections;
/*
* LuaCoroutineIterator.cs
* 2017/08/18
* psycommando@gmail.com
* Description:
*/

namespace RogueEssence.Script
{
    /// <summary>
    /// This class is meant to be used to iterate over a lua iterator/coroutine in C# in a conscise way.
    /// </summary>
    public class LuaCoroutineIterator : IEnumerator<YieldInstruction>
    {
        /// <summary>
        /// Gets the current yield instruction from the iterator.
        /// </summary>
        public YieldInstruction Current { get; internal set; }

        object IEnumerator.Current { get { return this.Current; } }

        private LuaFunction m_iterator;
        private LuaFunction m_origfun;
        private object[] m_origargs;

        /// <summary>
        /// Creates a new Lua coroutine iterator from a Lua function.
        /// </summary>
        /// <param name="fun">The Lua function to wrap as a coroutine.</param>
        /// <param name="args">Arguments to pass to the Lua function.</param>
        public LuaCoroutineIterator( LuaFunction fun, params object[] args )
        {
            m_origfun = fun;
            m_origargs = args;
            m_iterator = LuaEngine.Instance.CreateCoroutineIterator(fun, args);
        }

        private object CallInternal()
        {
            try
            {
                return m_iterator.Call().First();
            }
            catch(Exception ex)
            {
                DiagManager.Instance.LogInfo(String.Format("LuaCoroutineIterator.CallInternal(): Caught exception :\n", ex.Message));
            }
            return null;
        }

        /// <summary>
        /// Advances the iterator to the next yield instruction.
        /// </summary>
        /// <returns>True if there is a next instruction, false if the coroutine has completed.</returns>
        public bool MoveNext()
        {
            object res = CallInternal();

            if (res == null)
                Current = null;
            else
            {
                //This handles waiting on coroutines
                if (res.GetType() == typeof(Coroutine))
                    Current = CoroutineManager.Instance.StartCoroutine(res as Coroutine, false);
                else if (res.GetType().IsSubclassOf(typeof(YieldInstruction)))
                    Current = res as YieldInstruction;
            }
            return res != null;
        }

        /// <summary>
        /// Resets the iterator to the beginning, recreating the coroutine.
        /// </summary>
        public void Reset()
        {
            Current = null;
            m_iterator = LuaEngine.Instance.CreateCoroutineIterator(m_origfun, m_origargs);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LuaCoroutineIterator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Disposes of the iterator and releases any resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }

}
