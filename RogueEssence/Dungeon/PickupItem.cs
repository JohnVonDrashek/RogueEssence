using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents an item being picked up in the dungeon, including its visual representation and pickup message.
    /// Implements IDrawableSprite to render the item sprite during the pickup animation.
    /// </summary>
    public class PickupItem : IDrawableSprite
    {
        /// <summary>
        /// The message displayed when the item is picked up.
        /// </summary>
        public string PickupMessage;

        /// <summary>
        /// The sprite index for the item's visual representation.
        /// </summary>
        public string SpriteIndex;

        /// <summary>
        /// The sound effect played when the item is picked up.
        /// </summary>
        public string Sound;

        /// <summary>
        /// The tile location of the item in the dungeon.
        /// </summary>
        public Loc TileLoc;

        /// <summary>
        /// The character waiting to pick up this item.
        /// </summary>
        public Character WaitingChar;

        /// <summary>
        /// Whether the pickup message is displayed only locally.
        /// </summary>
        public bool LocalMsg;

        /// <summary>
        /// Gets the map location in pixels.
        /// </summary>
        public Loc MapLoc { get { return TileLoc * GraphicsManager.TileSize; } }

        /// <summary>
        /// Gets the height offset for rendering. Always returns 0 for items.
        /// </summary>
        public int LocHeight { get { return 0; } }

        /// <summary>
        /// Initializes a new PickupItem with all properties.
        /// </summary>
        /// <param name="msg">The pickup message to display.</param>
        /// <param name="sprite">The sprite index for the item.</param>
        /// <param name="sound">The sound effect to play.</param>
        /// <param name="loc">The tile location of the item.</param>
        /// <param name="waitChar">The character picking up the item.</param>
        /// <param name="localMsg">Whether the message is local only.</param>
        public PickupItem(string msg, string sprite, string sound, Loc loc, Character waitChar, bool localMsg)
        {
            PickupMessage = msg;
            SpriteIndex = sprite;
            Sound = sound;
            TileLoc = loc;
            WaitingChar = waitChar;
            LocalMsg = localMsg;
        }

        /// <summary>
        /// Draws debug information. This implementation does nothing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        public void DrawDebug(SpriteBatch spriteBatch, Loc offset) { }

        /// <summary>
        /// Draws the item sprite at its location with white color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        public void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            Draw(spriteBatch, offset, Color.White);
        }

        /// <summary>
        /// Draws the item sprite at its location with the specified color tint.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        /// <param name="color">The color tint to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Loc offset, Color color)
        {
            if (SpriteIndex != "")
            {
                Loc drawLoc = GetDrawLoc(offset);

                DirSheet sheet = GraphicsManager.GetItem(SpriteIndex);
                sheet.DrawDir(spriteBatch, new Vector2(drawLoc.X, drawLoc.Y), 0, Dir8.Down, color);
            }
        }

        /// <summary>
        /// Gets the drawing location accounting for centering and camera offset.
        /// </summary>
        /// <param name="offset">The camera offset.</param>
        /// <returns>The screen position to draw the item at.</returns>
        public Loc GetDrawLoc(Loc offset)
        {
            return new Loc(MapLoc.X + GraphicsManager.TileSize / 2 - GraphicsManager.GetItem(SpriteIndex).TileWidth / 2,
                MapLoc.Y + GraphicsManager.TileSize / 2 - GraphicsManager.GetItem(SpriteIndex).TileHeight / 2) - offset;
        }

        /// <summary>
        /// Gets the sheet offset. Always returns zero for items.
        /// </summary>
        /// <returns>A zero location.</returns>
        public Loc GetSheetOffset() { return Loc.Zero; }

        /// <summary>
        /// Gets the size of the item sprite.
        /// </summary>
        /// <returns>The dimensions of the item sprite.</returns>
        public Loc GetDrawSize()
        {
            return new Loc(GraphicsManager.GetItem(SpriteIndex).TileWidth,
                GraphicsManager.GetItem(SpriteIndex).TileHeight);
        }

    }
}
