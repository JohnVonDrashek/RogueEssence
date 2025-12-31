using System;
using System.Collections.Generic;

//This file contains all classes related to the coroutine system that governs the Dungeon portion of the game

namespace RogueEssence
{
    /// <summary>
    /// Abstract base class for yield instructions used in the coroutine system.
    /// Represents a pausable operation that can be waited on.
    /// </summary>
    public abstract class YieldInstruction
    {
        /// <summary>
        /// Determines whether this yield instruction has completed.
        /// </summary>
        /// <returns>True if the instruction has finished; otherwise, false.</returns>
        public abstract bool FinishedYield();

        /// <summary>
        /// Updates the yield instruction state. Called each frame while waiting.
        /// </summary>
        public virtual void Update() { }
    }

    /// <summary>
    /// Yield instruction that waits for a specified number of frames.
    /// </summary>
    public class WaitForFrames : YieldInstruction
    {
        private long frames;

        /// <summary>
        /// Initializes a new instance of the WaitForFrames class.
        /// </summary>
        /// <param name="frames">The number of frames to wait.</param>
        public WaitForFrames(long frames)
        {
            this.frames = frames;
        }

        /// <summary>
        /// Determines whether the wait has completed.
        /// </summary>
        /// <returns>True if the frame count has reached zero or below; otherwise, false.</returns>
        public override bool FinishedYield()
        {
            return frames <= 0;
        }

        /// <summary>
        /// Decrements the frame counter by one.
        /// </summary>
        public override void Update()
        {
            frames--;
        }
    }

    /// <summary>
    /// Coroutine wrapper for Lua scripts that provides a custom name for debugging.
    /// </summary>
    public class LuaCoroutine : Coroutine
    {
        string name;

        /// <summary>
        /// Initializes a new instance of the LuaCoroutine class.
        /// </summary>
        /// <param name="name">The display name for the coroutine.</param>
        /// <param name="enumerator">The enumerator containing the coroutine logic.</param>
        public LuaCoroutine(string name, IEnumerator<YieldInstruction> enumerator) : base(enumerator)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets a string representation of the enumerator for debugging.
        /// </summary>
        /// <returns>The name of this Lua coroutine.</returns>
        public override string GetEnumeratorString()
        {
            return name;
        }
    }

    /// <summary>
    /// Represents a coroutine that can be yielded and resumed.
    /// Wraps an enumerator of yield instructions for cooperative multitasking.
    /// </summary>
    public class Coroutine : YieldInstruction
    {
        IEnumerator<YieldInstruction> enumerator;
        bool finished;

        /// <summary>
        /// Initializes a new instance of the Coroutine class.
        /// </summary>
        /// <param name="enumerator">The enumerator containing the coroutine logic.</param>
        public Coroutine(IEnumerator<YieldInstruction> enumerator)
        {
            this.enumerator = enumerator;
        }

        /// <summary>
        /// Advances the coroutine to the next yield instruction.
        /// Continues advancing while yield instructions complete immediately.
        /// </summary>
        public void MoveNext()
        {
            bool wantsAnother;
            do
            {
                wantsAnother = false;

                bool hasAnother = false;
                try
                {
                    hasAnother = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }

                if (!hasAnother)
                    finished = true;
                else
                {
                    if (enumerator.Current.FinishedYield())
                        wantsAnother = true;
                }
            } while (wantsAnother);
        }

        /// <summary>
        /// Determines whether the coroutine has completed execution.
        /// </summary>
        /// <returns>True if the coroutine has finished; otherwise, false.</returns>
        public override bool FinishedYield()
        {
            return finished;
        }

        /// <summary>
        /// Updates the current yield instruction and advances if complete.
        /// </summary>
        public override void Update()
        {
            if (enumerator.Current != null)
            {
                enumerator.Current.Update();
                if (enumerator.Current.FinishedYield())
                    MoveNext();
            }
            else
                MoveNext();
        }

        /// <summary>
        /// Gets a string representation of the enumerator for debugging.
        /// </summary>
        /// <returns>A string representation of the underlying enumerator.</returns>
        public virtual string GetEnumeratorString()
        {
            return enumerator.ToString();
        }
    }

    /// <summary>
    /// Yield instruction that waits until a predicate returns true.
    /// </summary>
    public class WaitUntil : YieldInstruction
    {
        Func<bool> predicate;

        /// <summary>
        /// Initializes a new instance of the WaitUntil class.
        /// </summary>
        /// <param name="predicate">The function that determines when waiting should end.</param>
        public WaitUntil(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>
        /// Determines whether the wait condition has been met.
        /// </summary>
        /// <returns>True if the predicate returns true; otherwise, false.</returns>
        public override bool FinishedYield()
        {
            return predicate();
        }
    }

    /// <summary>
    /// Yield instruction that waits while a predicate returns true.
    /// </summary>
    public class WaitWhile : YieldInstruction
    {
        Func<bool> predicate;

        /// <summary>
        /// Initializes a new instance of the WaitWhile class.
        /// </summary>
        /// <param name="predicate">The function that determines when waiting should continue.</param>
        public WaitWhile(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>
        /// Determines whether the wait condition has ended.
        /// </summary>
        /// <returns>True if the predicate returns false; otherwise, false.</returns>
        public override bool FinishedYield()
        {
            return !predicate();
        }
    }
}
