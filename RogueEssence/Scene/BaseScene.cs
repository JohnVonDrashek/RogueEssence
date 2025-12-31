using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence
{
    /// <summary>
    /// Abstract base class for all game scenes.
    /// Provides common functionality for scene management, animation handling, camera control, and sprite rendering.
    /// </summary>
    public abstract class BaseScene
    {
        /// <summary>
        /// Array of animation lists organized by draw layer priority.
        /// </summary>
        public List<IFinishableSprite>[] Anims;

        /// <summary>
        /// The current screen shake effect, if any.
        /// </summary>
        public ScreenMover ScreenShake;

        /// <summary>
        /// Gets the current window scale factor.
        /// </summary>
        public float WindowScale { get; protected set; }

        /// <summary>
        /// The matrix scale factor for rendering transformations.
        /// </summary>
        protected float matrixScale;

        /// <summary>
        /// The base scale factor.
        /// </summary>
        protected float scale;

        /// <summary>
        /// The scale factor used for drawing operations.
        /// </summary>
        protected float drawScale;

        /// <summary>
        /// Gets the current view rectangle representing the visible area.
        /// </summary>
        public Rect ViewRect { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the BaseScene class.
        /// Creates animation lists for each draw layer.
        /// </summary>
        public BaseScene()
        {
            Anims = new List<IFinishableSprite>[6];
            for (int n = (int)DrawLayer.Bottom; n <= (int)DrawLayer.NoDraw; n++)
                Anims[n] = new List<IFinishableSprite>();
        }

        /// <summary>
        /// Called when exiting the scene. Must be implemented by derived classes.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// Called when the scene begins. Must be implemented by derived classes.
        /// </summary>
        public abstract void Begin();

        /// <summary>
        /// Updates meta-information for the scene. Can be overridden by derived classes.
        /// </summary>
        public virtual void UpdateMeta() { }

        /// <summary>
        /// Processes input for the scene as a coroutine.
        /// </summary>
        /// <returns>An enumerator of yield instructions for coroutine execution.</returns>
        public abstract IEnumerator<YieldInstruction> ProcessInput();

        /// <summary>
        /// Creates and adds an animation to the specified draw layer.
        /// </summary>
        /// <param name="anim">The animation sprite to add.</param>
        /// <param name="priority">The draw layer priority for the animation.</param>
        public void CreateAnim(IFinishableSprite anim, DrawLayer priority)
        {
            Anims[(int)priority].Add(anim);
        }

        /// <summary>
        /// Clears all animations from all draw layers.
        /// </summary>
        public void ResetAnims()
        {
            for (int nn = (int)DrawLayer.Bottom; nn <= (int)DrawLayer.NoDraw; nn++)
                Anims[nn].Clear();
        }

        /// <summary>
        /// Sets a screen shake effect, replacing weaker shakes if necessary.
        /// </summary>
        /// <param name="shake">The screen shake effect to apply.</param>
        public void SetScreenShake(ScreenMover shake)
        {
            if (shake.MaxShake == 0)
                return;
            if (ScreenShake != null && shake.MaxShake < ScreenShake.MaxShake)
                return;

            ScreenShake = shake;
        }

        /// <summary>
        /// Updates camera modifications including screen shake effects.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        /// <param name="focusedLoc">Reference to the focused location, modified by camera effects.</param>
        public virtual void UpdateCamMod(FrameTick elapsedTime, ref Loc focusedLoc)
        {
            if (ScreenShake != null)
            {
                ScreenShake.Update(elapsedTime, ref focusedLoc);
                if (ScreenShake.Finished)
                    ScreenShake = null;
            }
        }

        /// <summary>
        /// Updates all animations in the scene.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        public virtual void Update(FrameTick elapsedTime)
        {
            for (int nn = (int)DrawLayer.Bottom; nn <= (int)DrawLayer.NoDraw; nn++)
            {
                for (int ii = Anims[nn].Count - 1; ii >= 0; ii--)
                {
                    Anims[nn][ii].Update(this, elapsedTime);
                    if (Anims[nn][ii].Finished)
                        Anims[nn].RemoveAt(ii);
                }
            }

        }

        /// <summary>
        /// Draws the scene. Must be implemented by derived classes.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Draws debug information for the scene. Can be overridden by derived classes.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public virtual void DrawDebug(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Determines if a sprite is visible within the specified view bounds.
        /// </summary>
        /// <param name="viewBounds">The view bounds to check against.</param>
        /// <param name="sprite">The sprite to check visibility for.</param>
        /// <returns>True if the sprite is visible within the bounds; otherwise, false.</returns>
        public bool CanSeeSprite(Rect viewBounds, IDrawableSprite sprite)
        {
            if (sprite == null)
                return false;

            Loc drawSize = sprite.GetDrawSize();
            if (drawSize == new Loc(-1))
                return true;
            if (drawSize == Loc.Zero)
                return false;

            Rect spriteRect = new Rect(sprite.GetDrawLoc(Loc.Zero) + sprite.GetSheetOffset(), drawSize);


            return Collision.Collides(spriteRect, viewBounds);
        }

        /// <summary>
        /// Adds a sprite to the draw list using the current view offset.
        /// </summary>
        /// <param name="sprites">The list of sprites to add to.</param>
        /// <param name="sprite">The sprite to add.</param>
        public void AddToDraw(List<(IDrawableSprite, Loc)> sprites, IDrawableSprite sprite)
        {
            AddToDraw(sprites, sprite, ViewRect.Start);
        }

        /// <summary>
        /// Adds a sprite to the draw list with a specified view offset.
        /// </summary>
        /// <param name="sprites">The list of sprites to add to.</param>
        /// <param name="sprite">The sprite to add.</param>
        /// <param name="viewOffset">The view offset for the sprite.</param>
        public void AddToDraw(List<(IDrawableSprite, Loc)> sprites, IDrawableSprite sprite, Loc viewOffset)
        {
            CollectionExt.AddToSortedList(sprites, (sprite, viewOffset), CompareSpriteCoords);
        }

        /// <summary>
        /// Adds a sprite to the draw list if it is relevant to the current view, handling wrapping if enabled.
        /// </summary>
        /// <param name="sprites">The list of sprites to add to.</param>
        /// <param name="wrapped">Whether the map uses wrapping.</param>
        /// <param name="wrapSize">The size of the wrapping area.</param>
        /// <param name="sprite">The sprite to add.</param>
        public void AddRelevantDraw(List<(IDrawableSprite, Loc)> sprites, bool wrapped, Loc wrapSize, IDrawableSprite sprite)
        {
            foreach (Loc viewOffset in IterateRelevantDraw(wrapped, wrapSize, sprite))
                AddToDraw(sprites, sprite, viewOffset);
        }

        /// <summary>
        /// Iterates over all relevant view offsets for drawing a sprite, handling wrapping.
        /// </summary>
        /// <param name="wrapped">Whether the map uses wrapping.</param>
        /// <param name="wrapSize">The size of the wrapping area.</param>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>An enumerable of view offsets where the sprite should be drawn.</returns>
        public IEnumerable<Loc> IterateRelevantDraw(bool wrapped, Loc wrapSize, IDrawableSprite sprite)
        {
            if (sprite == null)
                yield break;

            Loc drawSize = sprite.GetDrawSize();
            if (drawSize == new Loc(-1))
            {
                yield return ViewRect.Start;
                yield break;
            }
            if (drawSize == Loc.Zero)
                yield break;

            Loc baseDrawLoc = sprite.GetDrawLoc(Loc.Zero) + sprite.GetSheetOffset();
            if (!wrapped)
            {
                Rect spriteRect = new Rect(baseDrawLoc, drawSize);
                if (Collision.Collides(spriteRect, ViewRect))
                    yield return ViewRect.Start;
                yield break;
            }

            foreach (Rect spriteRect in WrappedCollision.IterateRegionsColliding(wrapSize, ViewRect, new Rect(baseDrawLoc, drawSize)))
            {
                //first compute a loc for which the addition to the original loc would result in this checked loc
                Loc diffLoc = spriteRect.Start - baseDrawLoc;
                //that difference is how much the viewRect needs to be shifted by
                yield return ViewRect.Start - diffLoc;
            }
        }

        /// <summary>
        /// Compares two sprites by their Y coordinate for sorting purposes.
        /// </summary>
        /// <param name="sprite1">The first sprite tuple to compare.</param>
        /// <param name="sprite2">The second sprite tuple to compare.</param>
        /// <returns>A negative value if sprite1 is above sprite2, positive if below, zero if equal.</returns>
        public int CompareSpriteCoords((IDrawableSprite sprite, Loc viewOffset) sprite1, (IDrawableSprite sprite, Loc viewOffset) sprite2)
        {
            return Math.Sign((sprite1.sprite.MapLoc.Y - sprite1.viewOffset.Y) - (sprite2.sprite.MapLoc.Y - sprite2.viewOffset.Y));
        }
    }
}
