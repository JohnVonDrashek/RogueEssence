using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Interface for ground map editing functionality.
    /// Provides access to active state, undo stack, and input processing.
    /// </summary>
    public interface IGroundEditor
    {
        /// <summary>
        /// Gets a value indicating whether the ground editor is currently active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets the undo stack for tracking and reverting ground map edits.
        /// </summary>
        public UndoStack Edits { get; }

        /// <summary>
        /// Processes input for the ground editor.
        /// </summary>
        /// <param name="input">The input manager containing current input state.</param>
        void ProcessInput(InputManager input);
    }
}