using System;
using RogueElements;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Content
{
    //TODO: move this variable around so that NoDraw is -2?
    public enum DrawLayer
    {
        /// <summary>
        /// Draws on the floor, behind all entities and terrain.
        /// </summary>
        Under = -1,
        /// <summary>
        /// Draws on the floor, behind all entities but not terrain.
        /// </summary>
        Bottom = 0,
        /// <summary>
        /// Draws in front of entities if placed at a higher Y coordinate, but draws behind entities in a tie.
        /// </summary>
        Back = 1,
        /// <summary>
        /// Draws in behind of entities if placed at a lower Y coordinate, but draws in front of entities in a tie.
        /// </summary>
        Normal = 2,
        /// <summary>
        /// Draws in front of entities.
        /// </summary>
        Front = 3,
        /// <summary>
        /// Draws on top of everything else.  Often used for overlay.
        /// </summary>
        Top = 4,
        /// <summary>
        /// Does not draw.
        /// </summary>
        NoDraw = 5
    }

    /// <summary>
    /// Interface for sprites that can be drawn in the game world.
    /// Provides position, drawing, and size information.
    /// </summary>
    public interface IDrawableSprite
    {
        /// <summary>
        /// Gets the position on the map in pixels.
        /// </summary>
        Loc MapLoc { get; }

        /// <summary>
        /// Gets the height above the ground in pixels.
        /// </summary>
        int LocHeight { get; }

        /// <summary>
        /// Draws debug information for the sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        void DrawDebug(SpriteBatch spriteBatch, Loc offset);

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        void Draw(SpriteBatch spriteBatch, Loc offset);

        /// <summary>
        /// Gets the draw location adjusted for the camera offset.
        /// </summary>
        /// <param name="offset">The camera offset.</param>
        /// <returns>The screen position to draw at.</returns>
        Loc GetDrawLoc(Loc offset);

        /// <summary>
        /// Gets the offset within the sprite sheet.
        /// </summary>
        /// <returns>The sheet offset.</returns>
        Loc GetSheetOffset();

        /// <summary>
        /// Gets the size of the sprite for drawing.
        /// </summary>
        /// <returns>The sprite dimensions.</returns>
        Loc GetDrawSize();
    }

    /// <summary>
    /// Interface for background sprites that are drawn behind other sprites.
    /// </summary>
    public interface IBackgroundSprite : IDrawableSprite
    {

    }

    /// <summary>
    /// Interface for character sprites that can display different forms and animations.
    /// </summary>
    public interface ICharSprite : IDrawableSprite
    {
        /// <summary>
        /// Gets the current sprite state for rendering.
        /// </summary>
        /// <param name="currentForm">The current character form ID.</param>
        /// <param name="currentOffset">The current position offset.</param>
        /// <param name="currentHeight">The current height offset.</param>
        /// <param name="currentAnim">The current animation index.</param>
        /// <param name="currentTime">The current animation time.</param>
        /// <param name="currentFrame">The current frame index.</param>
        void GetCurrentSprite(out CharID currentForm, out Loc currentOffset, out int currentHeight, out int currentAnim, out int currentTime, out int currentFrame);
    }

    /// <summary>
    /// Interface for emittable objects that support particle physics (velocity and acceleration).
    /// </summary>
    public interface IParticleEmittable : IEmittable
    {
        /// <summary>
        /// Creates a particle with the specified physics parameters.
        /// </summary>
        /// <param name="startLoc">The starting position.</param>
        /// <param name="speed">The initial velocity in pixels per second.</param>
        /// <param name="acceleration">The acceleration in pixels per second squared.</param>
        /// <param name="startHeight">The initial height above ground.</param>
        /// <param name="heightSpeed">The initial vertical velocity in pixels per second.</param>
        /// <param name="heightAcceleration">The vertical acceleration in pixels per second squared.</param>
        /// <param name="dir">The direction the particle faces.</param>
        /// <returns>A new particle instance.</returns>
        IParticleEmittable CreateParticle(Loc startLoc, Loc speed, Loc acceleration, int startHeight, int heightSpeed, int heightAcceleration, Dir8 dir);

        /// <summary>
        /// Creates a particle with the specified physics parameters and custom duration.
        /// </summary>
        /// <param name="totalTime">The total duration in frames.</param>
        /// <param name="startLoc">The starting position.</param>
        /// <param name="speed">The initial velocity in pixels per second.</param>
        /// <param name="acceleration">The acceleration in pixels per second squared.</param>
        /// <param name="startHeight">The initial height above ground.</param>
        /// <param name="heightSpeed">The initial vertical velocity in pixels per second.</param>
        /// <param name="heightAcceleration">The vertical acceleration in pixels per second squared.</param>
        /// <param name="dir">The direction the particle faces.</param>
        /// <returns>A new particle instance.</returns>
        IParticleEmittable CreateParticle(int totalTime, Loc startLoc, Loc speed, Loc acceleration, int startHeight, int heightSpeed, int heightAcceleration, Dir8 dir);
    }

    /// <summary>
    /// Interface for objects that can be emitted by particle emitters.
    /// Supports cloning and positioning at emission time.
    /// </summary>
    public interface IEmittable : IFinishableSprite
    {
        /// <summary>
        /// Creates a clone of this emittable for emission.
        /// </summary>
        /// <returns>A new copy of this emittable.</returns>
        IEmittable CloneIEmittable();

        /// <summary>
        /// Creates a positioned instance at the specified location.
        /// </summary>
        /// <param name="mapLoc">The map position.</param>
        /// <param name="locHeight">The height above ground.</param>
        /// <param name="dir">The facing direction.</param>
        /// <returns>A positioned emittable instance.</returns>
        IEmittable CreateStatic(Loc mapLoc, int locHeight, Dir8 dir);
    }

    /// <summary>
    /// Interface for sprites that have a finite lifetime and can be updated.
    /// </summary>
    public interface IFinishableSprite : IDrawableSprite
    {
        /// <summary>
        /// Gets whether the sprite has finished its lifetime.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Updates the sprite state.
        /// </summary>
        /// <param name="scene">The current scene.</param>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        void Update(BaseScene scene, FrameTick elapsedTime);
    }

    /// <summary>
    /// Abstract base class for all animations in the game.
    /// Provides position, height, and finish state tracking.
    /// </summary>
    [Serializable]
    public abstract class BaseAnim : IFinishableSprite
    {
        [NonSerialized]
        protected Loc mapLoc;
        [NonSerialized]
        protected int locHeight;
        [NonSerialized]
        protected bool finished;

        public Loc MapLoc { get { return mapLoc; } }
        public int LocHeight { get { return locHeight; } }
        public bool Finished { get { return finished; } }

        public abstract void Update(BaseScene scene, FrameTick elapsedTime);

        public void DrawDebug(SpriteBatch spriteBatch, Loc offset) { }
        public abstract void Draw(SpriteBatch spriteBatch, Loc offset);

        public abstract Loc GetDrawLoc(Loc offset);
        public Loc GetSheetOffset() { return Loc.Zero; }
        public abstract Loc GetDrawSize();
    }

    /// <summary>
    /// Interface for objects that can draw a preview with transparency.
    /// </summary>
    public interface IPreviewable
    {
        /// <summary>
        /// Draws a preview of the object with the specified transparency.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        /// <param name="alpha">The transparency value (0.0 to 1.0).</param>
        void DrawPreview(SpriteBatch spriteBatch, Loc offset, float alpha);
    }
}
