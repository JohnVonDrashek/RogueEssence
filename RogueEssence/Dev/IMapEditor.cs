using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Interface for dungeon map editing functionality.
    /// Provides access to edit modes, undo stack, and input processing.
    /// </summary>
    public interface IMapEditor
    {
        /// <summary>
        /// Defines the available tile editing modes.
        /// </summary>
        public enum TileEditMode
        {
            /// <summary>Draw individual tiles.</summary>
            Draw = 0,
            /// <summary>Fill connected areas with tiles.</summary>
            Fill = 1,
            /// <summary>Sample tile values from the map.</summary>
            Eyedrop = 2
        }

        /// <summary>
        /// Gets a value indicating whether the map editor is currently active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets the undo stack for tracking and reverting map edits.
        /// </summary>
        public UndoStack Edits { get; }

        /// <summary>
        /// Processes input for the map editor.
        /// </summary>
        /// <param name="input">The input manager containing current input state.</param>
        void ProcessInput(InputManager input);
    }
}