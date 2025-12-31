using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// An animated cursor element that indicates the current selection in a menu.
    /// The cursor flashes on/off to provide visual feedback.
    /// </summary>
    public class MenuCursor : BaseMenuElement
    {
        /// <summary>
        /// The total frames for one cursor flash cycle.
        /// </summary>
        protected const int CURSOR_FLASH_TIME = 24;

        /// <summary>
        /// The previous tick value for flash timing calculation.
        /// </summary>
        public ulong PrevTick;

        /// <summary>
        /// Gets or sets the position of the cursor relative to the parent menu.
        /// </summary>
        public Loc Loc { get; set; }

        /// <summary>
        /// The direction the cursor arrow points.
        /// </summary>
        public Dir4 Direction;

        private IInteractable baseMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCursor"/> class pointing right.
        /// </summary>
        /// <param name="baseMenu">The menu this cursor belongs to.</param>
        public MenuCursor(IInteractable baseMenu) : this("", baseMenu, Dir4.Right) { }

        /// <summary>
        /// Initializes a new labeled instance of the <see cref="MenuCursor"/> class pointing right.
        /// </summary>
        /// <param name="label">The identifier label for this cursor.</param>
        /// <param name="baseMenu">The menu this cursor belongs to.</param>
        public MenuCursor(string label, IInteractable baseMenu) : this(label, baseMenu, Dir4.Right) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCursor"/> class with custom direction.
        /// </summary>
        /// <param name="baseMenu">The menu this cursor belongs to.</param>
        /// <param name="dir">The direction the cursor points.</param>
        public MenuCursor(IInteractable baseMenu, Dir4 dir) : this("", baseMenu, dir) { }

        /// <summary>
        /// Initializes a new labeled instance of the <see cref="MenuCursor"/> class with custom direction.
        /// </summary>
        /// <param name="label">The identifier label for this cursor.</param>
        /// <param name="baseMenu">The menu this cursor belongs to.</param>
        /// <param name="dir">The direction the cursor points.</param>
        public MenuCursor(string label, IInteractable baseMenu, Dir4 dir)
        {
            this.Label = label;
            this.baseMenu = baseMenu;
            this.Direction = dir;
        }

        /// <summary>
        /// Resets the flash timing so the cursor starts visible.
        /// </summary>
        public void ResetTimeOffset()
        {
            PrevTick = GraphicsManager.TotalFrameTick % (ulong)FrameTick.FrameToTick(CURSOR_FLASH_TIME);
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            //draw cursor
            if (((GraphicsManager.TotalFrameTick - PrevTick) / (ulong)FrameTick.FrameToTick(CURSOR_FLASH_TIME / 2)) % 2 == 0 || baseMenu.Inactive)
            {
                switch (Direction)
                {
                    case Dir4.Down:
                        GraphicsManager.Cursor.DrawTile(spriteBatch, (Loc + offset).ToVector2(), 1, 0);
                        break;
                    case Dir4.Left:
                        GraphicsManager.Cursor.DrawTile(spriteBatch, (Loc + offset).ToVector2(), 0, 0, Color.White, SpriteEffects.FlipHorizontally);
                        break;
                    case Dir4.Up:
                        GraphicsManager.Cursor.DrawTile(spriteBatch, (Loc + offset).ToVector2(), 1, 0, Color.White, SpriteEffects.FlipVertically);
                        break;
                    case Dir4.Right:
                        GraphicsManager.Cursor.DrawTile(spriteBatch, (Loc + offset).ToVector2(), 0, 0);
                        break;
                }
            }
        }
    }
}
