using Microsoft.Xna.Framework.Graphics;
using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for menu elements providing label functionality.
    /// Inherit from this class to create custom menu elements.
    /// </summary>
    public abstract class BaseMenuElement : IMenuElement
    {
        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public bool HasLabel() => !string.IsNullOrEmpty(Label);

        /// <inheritdoc/>
        public bool LabelContains(string substr) => HasLabel() && Label.Contains(substr);

        /// <inheritdoc/>
        public abstract void Draw(SpriteBatch spriteBatch, Loc offset);
    }
}
