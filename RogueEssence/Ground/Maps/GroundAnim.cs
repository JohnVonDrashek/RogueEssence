using System;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * GroundAnim.cs
 * 2017/07/03
 * idk
 * Description: An animated prop to be put on a GroundMap.  Unlike GroundObject, it cannot be collided or interacted with
 */

namespace RogueEssence.Ground
{
    /// <summary>
    /// Represents an animated decoration prop that can be placed on a ground map.
    /// Unlike <see cref="GroundObject"/>, this entity cannot be collided with or interacted with.
    /// It is purely visual decoration that plays an animation at a fixed location.
    /// </summary>
    [Serializable]
    public class GroundAnim : IDrawableSprite, IPreviewable
    {
        /// <summary>
        /// The animation data for this ground animation, including sprite sheet and frame information.
        /// </summary>
        public IPlaceableAnimData ObjectAnim;

        /// <summary>
        /// Gets or sets the position of this animation on the map in pixels.
        /// </summary>
        public Loc MapLoc { get; set; }

        /// <summary>
        /// Gets the height offset for drawing. Always returns 0 for ground animations.
        /// </summary>
        public int LocHeight { get { return 0; } }

        /// <summary>
        /// Creates a new ground animation with default settings.
        /// Initializes with an empty animation facing down.
        /// </summary>
        public GroundAnim()
        {
            ObjectAnim = new ObjAnimData();
            ObjectAnim.AnimDir = Dir8.Down;
        }

        /// <summary>
        /// Creates a new ground animation with the specified animation data and location.
        /// </summary>
        /// <param name="anim">The animation data to use for this decoration.</param>
        /// <param name="loc">The position on the map in pixels.</param>
        public GroundAnim(IPlaceableAnimData anim, Loc loc)
        {
            ObjectAnim = anim;
            MapLoc = loc;
        }

        /// <summary>
        /// Creates a copy of another ground animation.
        /// </summary>
        /// <param name="other">The ground animation to copy.</param>
        public GroundAnim(GroundAnim other)
        {
            ObjectAnim = (IPlaceableAnimData)other.ObjectAnim.Clone();
            MapLoc = other.MapLoc;
        }

        /// <summary>
        /// Draws debug information for this animation. Currently does nothing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        public void DrawDebug(SpriteBatch spriteBatch, Loc offset) { }

        /// <summary>
        /// Draws this animation at full opacity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            DrawPreview(spriteBatch, offset, 1f);
        }

        /// <summary>
        /// Draws this animation with the specified transparency level.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        /// <param name="alpha">The transparency level (0.0 to 1.0).</param>
        public void DrawPreview(SpriteBatch spriteBatch, Loc offset, float alpha)
        {
            if (ObjectAnim.AnimIndex != "")
            {
                Loc drawLoc = GetDrawLoc(offset);

                DirSheet sheet = GraphicsManager.GetDirSheet(ObjectAnim.AssetType, ObjectAnim.AnimIndex);
                sheet.DrawDir(spriteBatch, drawLoc.ToVector2(), ObjectAnim.GetCurrentFrame(GraphicsManager.TotalFrameTick, sheet.TotalFrames), ObjectAnim.GetDrawDir(Dir8.None), Color.White * ((float)ObjectAnim.Alpha * alpha / 255), ObjectAnim.AnimFlip);
            }
        }

        /// <summary>
        /// Gets the screen position for drawing this animation.
        /// </summary>
        /// <param name="offset">The camera offset to subtract.</param>
        /// <returns>The screen position to draw at.</returns>
        public Loc GetDrawLoc(Loc offset)
        {
            return MapLoc - offset;
        }

        /// <summary>
        /// Gets the offset within the sprite sheet. Always returns zero for ground animations.
        /// </summary>
        /// <returns>A zero location.</returns>
        public Loc GetSheetOffset() { return Loc.Zero; }

        /// <summary>
        /// Gets the size of this animation's sprite in pixels.
        /// </summary>
        /// <returns>The width and height of the sprite.</returns>
        public Loc GetDrawSize()
        {
            DirSheet sheet = GraphicsManager.GetObject(ObjectAnim.AnimIndex);

            return new Loc(sheet.TileWidth, sheet.TileHeight);
        }

        /// <summary>
        /// Gets the bounding rectangle for this animation.
        /// The bounds are at least as large as one texture unit.
        /// </summary>
        /// <returns>The bounding rectangle in map coordinates.</returns>
        public Rect GetBounds()
        {
            Loc drawSize = GetDrawSize();
            return new Rect(MapLoc, new Loc(Math.Max(drawSize.X, GraphicsManager.TEX_SIZE), Math.Max(drawSize.Y, GraphicsManager.TEX_SIZE)));
        }
    }
}
