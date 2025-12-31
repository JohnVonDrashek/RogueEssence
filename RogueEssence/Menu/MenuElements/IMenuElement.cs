using Microsoft.Xna.Framework.Graphics;
using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Defines the contract for visual elements that can be displayed within a menu.
    /// Menu elements are drawn at a relative offset from their parent menu's position.
    /// </summary>
    public interface IMenuElement : ILabeled
    {
        /// <summary>
        /// Draws this element to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="offset">The offset from the parent menu's origin.</param>
        void Draw(SpriteBatch spriteBatch, Loc offset);
    }
}
