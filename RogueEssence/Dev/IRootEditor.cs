using Microsoft.Xna.Framework;
using RogueEssence.Data;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Interface for the root editor that manages ground and map editing modes.
    /// Handles loading, updating, and drawing of editor UI, as well as input consumption tracking.
    /// </summary>
    public interface IRootEditor
    {
        /// <summary>
        /// Gets a value indicating whether the editor has completed loading.
        /// </summary>
        bool LoadComplete { get; }

        /// <summary>
        /// Gets a value indicating whether the editor consumed mouse input this frame.
        /// </summary>
        bool AteMouse { get; }

        /// <summary>
        /// Gets a value indicating whether the editor consumed keyboard input this frame.
        /// </summary>
        bool AteKeyboard { get; }

        /// <summary>
        /// Gets the ground map editor interface.
        /// </summary>
        IGroundEditor GroundEditor { get; }

        /// <summary>
        /// Gets the dungeon map editor interface.
        /// </summary>
        IMapEditor MapEditor { get; }

        /// <summary>
        /// Reloads data of the specified type in the editor.
        /// </summary>
        /// <param name="dataType">The type of data to reload.</param>
        void ReloadData(DataManager.DataType dataType);

        /// <summary>
        /// Loads the editor with the specified game instance.
        /// </summary>
        /// <param name="game">The game instance to load the editor with.</param>
        void Load(GameBase game);

        /// <summary>
        /// Updates the editor state for the current frame.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws the editor UI and overlays.
        /// </summary>
        void Draw();

        /// <summary>
        /// Opens the ground map editor.
        /// </summary>
        void OpenGround();

        /// <summary>
        /// Closes the ground map editor.
        /// </summary>
        void CloseGround();

        /// <summary>
        /// Opens the dungeon map editor.
        /// </summary>
        void OpenMap();

        /// <summary>
        /// Closes the dungeon map editor.
        /// </summary>
        void CloseMap();
    }
}