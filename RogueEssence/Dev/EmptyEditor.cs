using Microsoft.Xna.Framework;
using RogueEssence.Data;

namespace RogueEssence.Dev
{
    /// <summary>
    /// A no-op implementation of IRootEditor used when no editor functionality is needed.
    /// All methods perform no action and all editor properties return null or false.
    /// </summary>
    public class EmptyEditor : IRootEditor
    {
        /// <inheritdoc/>
        public bool LoadComplete => true;

        /// <inheritdoc/>
        public IGroundEditor GroundEditor => null;

        /// <inheritdoc/>
        public IMapEditor MapEditor => null;

        /// <inheritdoc/>
        public bool AteMouse { get { return false; } }

        /// <inheritdoc/>
        public bool AteKeyboard { get { return false; } }

        /// <inheritdoc/>
        public void ReloadData(DataManager.DataType dataType) { }

        /// <inheritdoc/>
        public void Load(GameBase game) { }

        /// <inheritdoc/>
        public void Update(GameTime gameTime) { }

        /// <inheritdoc/>
        public void Draw() { }

        /// <inheritdoc/>
        public void OpenGround() { }

        /// <inheritdoc/>
        public void CloseGround() { }

        /// <inheritdoc/>
        public void OpenMap() { }

        /// <inheritdoc/>
        public void CloseMap() { }
    }
}