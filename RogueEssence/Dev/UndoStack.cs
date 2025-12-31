using RogueEssence.Ground;
using RogueEssence.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Abstract base class for operations that can be undone and redone.
    /// </summary>
    public abstract class Undoable
    {
        /// <summary>
        /// Applies this operation for the first time. By default, calls Redo.
        /// Override to add custom first-time application logic.
        /// </summary>
        public virtual void Apply()
        {
            Redo();
        }

        /// <summary>
        /// Undoes this operation, reverting to the previous state.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// Redoes this operation, reapplying the change.
        /// </summary>
        public abstract void Redo();
    }

    /// <summary>
    /// An undoable operation where undo and redo perform the same action.
    /// Useful for toggle-style operations.
    /// </summary>
    public abstract class SymmetricUndo : Undoable
    {
        /// <inheritdoc/>
        public override void Undo()
        {
            Redo();
        }
    }

    /// <summary>
    /// An undoable operation that can be created in either forward or reversed direction.
    /// Provides Forward and Backward methods that are swapped based on the reversed flag.
    /// </summary>
    public abstract class ReversibleUndo : Undoable
    {
        private bool reversed;

        /// <summary>
        /// Initializes a new instance of the ReversibleUndo class.
        /// </summary>
        /// <param name="reversed">If true, Forward and Backward are swapped for undo/redo.</param>
        public ReversibleUndo(bool reversed)
        {
            this.reversed = reversed;
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            if (reversed)
                Forward();
            else
                Backward();
        }

        /// <inheritdoc/>
        public override void Redo()
        {
            if (reversed)
                Backward();
            else
                Forward();
        }

        /// <summary>
        /// Performs the forward operation.
        /// </summary>
        public abstract void Forward();

        /// <summary>
        /// Performs the backward operation.
        /// </summary>
        public abstract void Backward();
    }

    /// <summary>
    /// An undoable operation that captures and restores complete state snapshots.
    /// Uses serialization to create deep copies of state.
    /// </summary>
    /// <typeparam name="T">The type of state being captured.</typeparam>
    public abstract class StateUndo<T> : Undoable
    {
        private T past;
        private T result;

        /// <inheritdoc/>
        public override void Apply()
        {
            T curState = GetState();
            //serialize, deserialize, assign
            past = ReflectionExt.SerializeCopy(curState);
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            result = GetState();
            SetState(past);
        }

        /// <inheritdoc/>
        public override void Redo()
        {
            SetState(result);
        }

        /// <summary>
        /// Gets the current state to be captured.
        /// </summary>
        /// <returns>The current state.</returns>
        public abstract T GetState();

        /// <summary>
        /// Sets the state to the specified value.
        /// </summary>
        /// <param name="state">The state to restore.</param>
        public abstract void SetState(T state);
    }

    /// <summary>
    /// Manages a stack of undoable operations, supporting undo and redo functionality.
    /// </summary>
    public class UndoStack
    {
        private Stack<Undoable> undos;
        private Stack<Undoable> redos;

        /// <summary>
        /// Initializes a new instance of the UndoStack class.
        /// </summary>
        public UndoStack()
        {
            undos = new Stack<Undoable>();
            redos = new Stack<Undoable>();
        }

        /// <summary>
        /// Gets a value indicating whether there are operations that can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return undos.Count > 0; }
            set { }
        }

        /// <summary>
        /// Gets a value indicating whether there are operations that can be redone.
        /// </summary>
        public bool CanRedo
        {
            get { return redos.Count > 0; }
            set { }
        }

        /// <summary>
        /// Undoes the most recent operation and moves it to the redo stack.
        /// </summary>
        public void Undo()
        {
            Undoable step = undos.Pop();
            step.Undo();
            redos.Push(step);
            DiagManager.Instance.LogInfo(String.Format("Undo {0}", step));
        }

        /// <summary>
        /// Redoes the most recently undone operation and moves it to the undo stack.
        /// </summary>
        public void Redo()
        {
            Undoable step = redos.Pop();
            step.Redo();
            undos.Push(step);
            DiagManager.Instance.LogInfo(String.Format("Redo {0}", step));
        }

        /// <summary>
        /// Applies a new operation, clearing the redo stack and adding to the undo stack.
        /// </summary>
        /// <param name="step">The operation to apply.</param>
        public void Apply(Undoable step)
        {
            redos.Clear();
            step.Apply();
            undos.Push(step);
            DiagManager.Instance.LogInfo(String.Format("Apply {0}", step));
        }

        /// <summary>
        /// Clears both the undo and redo stacks.
        /// </summary>
        public void Clear()
        {
            undos.Clear();
            redos.Clear();
        }
    }
}
