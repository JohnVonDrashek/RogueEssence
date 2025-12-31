using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// A horizontal divider line used to separate sections within a menu.
    /// </summary>
    public class MenuDivider : BaseMenuElement
    {
        /// <summary>
        /// Gets or sets the length of the divider in pixels.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the position of the divider relative to the parent menu.
        /// </summary>
        public Loc Loc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuDivider"/> class.
        /// </summary>
        /// <param name="loc">The position of the divider.</param>
        /// <param name="length">The length of the divider in pixels.</param>
        public MenuDivider(Loc loc, int length) : this("", loc, length) { }

        /// <summary>
        /// Initializes a new labeled instance of the <see cref="MenuDivider"/> class.
        /// </summary>
        /// <param name="label">The identifier label for this divider.</param>
        /// <param name="loc">The position of the divider.</param>
        /// <param name="length">The length of the divider in pixels.</param>
        public MenuDivider(string label, Loc loc, int length)
        {
            Label = label;
            Length = length;
            Loc = loc;
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            GraphicsManager.DivTex.Draw(spriteBatch, new Rectangle(Loc.X + offset.X, Loc.Y + offset.Y, Length, 2), null, Color.White);
        }
    }
}
