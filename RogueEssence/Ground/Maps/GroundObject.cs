using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AABB;
using RogueEssence.Script;
using System.Runtime.Serialization;

namespace RogueEssence.Ground
{
    /// <summary>
    /// Represents an interactive object placed on a ground map.
    /// Objects can be collided with, triggered by touch or action, and display animations.
    /// Unlike <see cref="GroundAnim"/>, objects participate in the collision system
    /// and can run Lua script events when interacted with.
    /// </summary>
    [Serializable]
    public class GroundObject : BaseTaskUser, IDrawableSprite, IObstacle
    {
        /// <summary>
        /// The default animation data for this object.
        /// </summary>
        public IPlaceableAnimData ObjectAnim;

        /// <summary>
        /// If true, characters can pass through this object without collision.
        /// </summary>
        public bool Passable;

        /// <summary>
        /// The currently playing animation, if any.
        /// </summary>
        public IPlaceableAnimData CurrentAnim;

        /// <summary>
        /// The elapsed time for the current animation.
        /// </summary>
        public FrameTick AnimTime;

        /// <summary>
        /// The number of animation cycles to play.
        /// </summary>
        public int Cycles;

        /// <summary>
        /// Gets the collision tags for this object based on its state.
        /// Returns 0 if disabled, 3 for passable (cross), 2 for touch trigger, 1 for slide.
        /// </summary>
        public uint Tags
        {
            get
            {
                if (!EntEnabled)
                    return 0u;

                if (Passable)
                    return 3u; // cross response
                else
                {
                    if (TriggerType == EEntityTriggerTypes.Touch || TriggerType == EEntityTriggerTypes.TouchOnce)
                        return 2u; // touch response
                    else
                        return 1u; // slide response
                }
            }
        }

        /// <summary>
        /// The offset to apply when drawing this object relative to its position.
        /// </summary>
        public Loc DrawOffset;

        /// <summary>
        /// Gets the color used to display this entity in the editor.
        /// </summary>
        public override Color DevEntColoring => Color.Chartreuse;

        /// <summary>
        /// Gets the thinking behavior type. Objects never think autonomously.
        /// </summary>
        public override EThink ThinkType => EThink.Never;

        /// <summary>
        /// Creates a new ground object with default settings.
        /// </summary>
        public GroundObject()
        {
            ObjectAnim = new ObjAnimData();
            CurrentAnim = new ObjAnimData();
            EntName = "GroundObject" + ToString(); //!#FIXME : Give a default unique name please fix this when we have editor/template names!
            SetTriggerType(EEntityTriggerTypes.Action);
        }

        /// <summary>
        /// Creates a new ground object with full parameter specification.
        /// </summary>
        /// <param name="anim">The animation data for this object.</param>
        /// <param name="dir">The direction the object faces.</param>
        /// <param name="collider">The collision bounds rectangle.</param>
        /// <param name="drawOffset">The offset for drawing the sprite.</param>
        /// <param name="triggerty">The type of trigger for this object.</param>
        /// <param name="passable">Whether characters can pass through this object.</param>
        /// <param name="entname">The unique name identifier for this entity.</param>
        public GroundObject(IPlaceableAnimData anim, Dir8 dir, Rect collider, Loc drawOffset, EEntityTriggerTypes triggerty, bool passable, string entname)
        {
            ObjectAnim = anim;
            CurrentAnim = new ObjAnimData();
            Collider = collider;
            DrawOffset = drawOffset;
            Direction = dir;
            SetTriggerType(triggerty);
            Passable = passable;
            EntName = entname;
        }

        /// <summary>
        /// Creates a new ground object with animation, collider, trigger type, and name.
        /// </summary>
        /// <param name="anim">The animation data for this object.</param>
        /// <param name="collider">The collision bounds rectangle.</param>
        /// <param name="triggerty">The type of trigger for this object.</param>
        /// <param name="entname">The unique name identifier for this entity.</param>
        public GroundObject(IPlaceableAnimData anim, Rect collider, EEntityTriggerTypes triggerty, string entname)
            :this(anim, Dir8.Down, collider, new Loc(), triggerty, false, entname)
        {}

        /// <summary>
        /// Creates a new ground object with specified draw offset.
        /// </summary>
        /// <param name="anim">The animation data for this object.</param>
        /// <param name="collider">The collision bounds rectangle.</param>
        /// <param name="drawOffset">The offset for drawing the sprite.</param>
        /// <param name="contact">If true, triggered on touch; if false, triggered on action.</param>
        /// <param name="entname">The unique name identifier for this entity.</param>
        public GroundObject(IPlaceableAnimData anim, Rect collider, Loc drawOffset, bool contact, string entname)
            : this(anim, Dir8.Down, collider, drawOffset, contact ? EEntityTriggerTypes.Touch : EEntityTriggerTypes.Action, false, entname)
        { }

        /// <summary>
        /// Creates a new ground object with specified direction and draw offset.
        /// </summary>
        /// <param name="anim">The animation data for this object.</param>
        /// <param name="dir">The direction the object faces.</param>
        /// <param name="collider">The collision bounds rectangle.</param>
        /// <param name="drawOffset">The offset for drawing the sprite.</param>
        /// <param name="contact">If true, triggered on touch; if false, triggered on action.</param>
        /// <param name="entname">The unique name identifier for this entity.</param>
        public GroundObject(IPlaceableAnimData anim, Dir8 dir, Rect collider, Loc drawOffset, bool contact, string entname)
            : this(anim, dir, collider, drawOffset, contact ? EEntityTriggerTypes.Touch : EEntityTriggerTypes.Action, false, entname)
        { }

        /// <summary>
        /// Creates a new ground object with minimal parameters.
        /// </summary>
        /// <param name="anim">The animation data for this object.</param>
        /// <param name="collider">The collision bounds rectangle.</param>
        /// <param name="contact">If true, triggered on touch; if false, triggered on action.</param>
        /// <param name="entname">The unique name identifier for this entity.</param>
        public GroundObject(IPlaceableAnimData anim, Rect collider, bool contact, string entname)
            : this(anim, collider, new Loc(), contact, entname)
        { }

        /// <summary>
        /// Creates a copy of another ground object.
        /// </summary>
        /// <param name="other">The ground object to copy.</param>
        protected GroundObject(GroundObject other) : base(other)
        {
            ObjectAnim = other.ObjectAnim.Clone();
            CurrentAnim = new ObjAnimData();
            DrawOffset = other.DrawOffset;
            Passable = other.Passable;
        }

        /// <summary>
        /// Creates a clone of this ground object.
        /// </summary>
        /// <returns>A new ground object with the same properties.</returns>
        public override GroundEntity Clone() { return new GroundObject(this); }

        /// <summary>
        /// Handles interaction with this object when activated or touched by another entity.
        /// Runs the appropriate Lua script event based on the trigger type.
        /// </summary>
        /// <param name="activator">The entity that triggered the interaction.</param>
        /// <param name="result">The result of the trigger to be populated.</param>
        /// <returns>A coroutine enumerator for the interaction sequence.</returns>
        public override IEnumerator<YieldInstruction> Interact(GroundEntity activator, TriggerResult result) //PSY: Set this value to get the entity that touched us/activated us
        {
            if (!EntEnabled)
                yield break;

            //Run script events
            if (GetTriggerType() == EEntityTriggerTypes.Action)
                yield return CoroutineManager.Instance.StartCoroutine(RunEvent(LuaEngine.EEntLuaEventTypes.Action, result, activator));
            else if (GetTriggerType() == EEntityTriggerTypes.Touch || GetTriggerType() == EEntityTriggerTypes.TouchOnce)
                yield return CoroutineManager.Instance.StartCoroutine(RunEvent(LuaEngine.EEntLuaEventTypes.Touch, result, activator));

        }

        /// <summary>
        /// Starts playing a temporary animation on this object.
        /// </summary>
        /// <param name="anim">The animation to play.</param>
        /// <param name="cycles">The number of times to loop the animation.</param>
        public void StartAction(ObjAnimData anim, int cycles)
        {
            CurrentAnim = anim;
            Cycles = cycles;
            AnimTime = FrameTick.Zero;
        }

        /// <summary>
        /// Moves this object smoothly from one location to another.
        /// </summary>
        /// <param name="loc">The starting location.</param>
        /// <param name="moveRate">The speed of movement in pixels per frame.</param>
        /// <param name="destination">The target location to move to.</param>
        /// <returns>A coroutine enumerator for the movement animation.</returns>
        public IEnumerator<YieldInstruction> MoveToLoc(Loc loc, int moveRate, Loc destination)
        {
            this.Position = loc;
            Loc goalDiff = destination - this.Position;
            int framesPassed = 1;
            while (this.Position != destination)
            {

                bool vertical = Math.Abs(goalDiff.Y) > Math.Abs(goalDiff.X);
                int mainMove = moveRate * framesPassed;
                int subMove = (int)Math.Abs(Math.Round((double)moveRate * framesPassed * goalDiff.GetScalar(vertical ? Axis4.Horiz : Axis4.Vert) / goalDiff.GetScalar(vertical ? Axis4.Vert : Axis4.Horiz)));
                Loc newDiff = new Loc((vertical ? subMove : mainMove) * Math.Sign(goalDiff.X), (vertical ? mainMove : subMove) * Math.Sign(goalDiff.Y));
                if (mainMove >= Math.Abs(goalDiff.GetScalar(vertical ? Axis4.Vert : Axis4.Horiz)))
                    newDiff = goalDiff;

                this.Position = loc + newDiff;

                yield return new WaitForFrames(1);

                framesPassed++;
            }
        }

        /// <summary>
        /// Updates the object's animation state.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        public void Update(FrameTick elapsedTime)
        {
            if (CurrentAnim.AnimIndex != "")
            {
                AnimTime += elapsedTime;

                DirSheet sheet = GraphicsManager.GetObject(CurrentAnim.AnimIndex);
                int totalTime = CurrentAnim.GetTotalFrames(sheet.TotalFrames) * CurrentAnim.FrameTime * Cycles;
                //end animation if it is finished
                if (AnimTime.ToFrames() >= totalTime)
                {
                    AnimTime = FrameTick.Zero;
                    CurrentAnim = new ObjAnimData();
                    Cycles = 0;
                }
            }
        }

        /// <summary>
        /// Draws debug visualization for this object's collision bounds.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        public override void DrawDebug(SpriteBatch spriteBatch, Loc offset)
        {
            if (EntEnabled)
            {
                BaseSheet blank = GraphicsManager.Pixel;
                blank.Draw(spriteBatch, new Rectangle(Collider.X - offset.X, Collider.Y - offset.Y, Collider.Width, Collider.Height), null, Color.Cyan * 0.7f);
            }
        }

        /// <summary>
        /// Draws this object at full opacity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            DrawPreview(spriteBatch, offset, 1f);
        }

        /// <summary>
        /// Draws this object with the specified transparency level.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset to apply.</param>
        /// <param name="alpha">The transparency level (0.0 to 1.0).</param>
        public override void DrawPreview(SpriteBatch spriteBatch, Loc offset, float alpha)
        {
            if (CurrentAnim.AnimIndex != "")
            {
                Loc drawLoc = GetDrawLoc(offset);

                DirSheet sheet = GraphicsManager.GetDirSheet(CurrentAnim.AssetType, CurrentAnim.AnimIndex);
                sheet.DrawDir(spriteBatch, drawLoc.ToVector2(), CurrentAnim.GetCurrentFrame(AnimTime, sheet.TotalFrames), CurrentAnim.GetDrawDir(Direction), Color.White * ((float)CurrentAnim.Alpha * alpha / 255), CurrentAnim.AnimFlip);
            }
            else if (ObjectAnim.AnimIndex != "")
            {
                Loc drawLoc = GetDrawLoc(offset);

                DirSheet sheet = GraphicsManager.GetDirSheet(ObjectAnim.AssetType, ObjectAnim.AnimIndex);
                sheet.DrawDir(spriteBatch, drawLoc.ToVector2(), ObjectAnim.GetCurrentFrame(GraphicsManager.TotalFrameTick, sheet.TotalFrames), ObjectAnim.GetDrawDir(Direction), Color.White * ((float)ObjectAnim.Alpha * alpha / 255), ObjectAnim.AnimFlip);
            }
        }

        /// <summary>
        /// Gets the current frame index of the active animation.
        /// </summary>
        /// <returns>The current frame index, or -1 if no animation is playing.</returns>
        public int GetCurrentFrame()
        {
            if (CurrentAnim.AnimIndex != "")
            {
                DirSheet sheet = GraphicsManager.GetDirSheet(CurrentAnim.AssetType, CurrentAnim.AnimIndex);
                return CurrentAnim.GetCurrentFrame(AnimTime, sheet.TotalFrames);
            }
            else if (ObjectAnim.AnimIndex != "")
            {
                DirSheet sheet = GraphicsManager.GetDirSheet(ObjectAnim.AssetType, ObjectAnim.AnimIndex);
                return ObjectAnim.GetCurrentFrame(GraphicsManager.TotalFrameTick, sheet.TotalFrames);
            }
            return -1;
        }

        /// <summary>
        /// Gets the screen position for drawing this object.
        /// </summary>
        /// <param name="offset">The camera offset to subtract.</param>
        /// <returns>The screen position to draw at.</returns>
        public override Loc GetDrawLoc(Loc offset)
        {
            return MapLoc - offset - DrawOffset;
        }

        /// <summary>
        /// Gets the size of this object's sprite in pixels.
        /// </summary>
        /// <returns>The width and height of the sprite.</returns>
        public override Loc GetDrawSize()
        {
            DirSheet sheet = GraphicsManager.GetObject(ObjectAnim.AnimIndex);

            return new Loc(sheet.TileWidth, sheet.TileHeight);
        }

        /// <summary>
        /// Gets the entity type for this object.
        /// </summary>
        /// <returns>Always returns EEntTypes.Object.</returns>
        public override EEntTypes GetEntityType()
        {
            return EEntTypes.Object;
        }

        /// <summary>
        /// Determines if this object has graphics assigned.
        /// </summary>
        /// <returns>True if an animation is assigned, false otherwise.</returns>
        public override bool DevHasGraphics()
        {
            if (ObjectAnim != null && ObjectAnim.AnimIndex != "")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a specific Lua event type is supported by this object.
        /// Objects support all events except Invalid and Think.
        /// </summary>
        /// <param name="ev">The event type to check.</param>
        /// <returns>True if the event is supported, false otherwise.</returns>
        public override bool IsEventSupported(LuaEngine.EEntLuaEventTypes ev)
        {
            return ev != LuaEngine.EEntLuaEventTypes.Invalid && ev != LuaEngine.EEntLuaEventTypes.Think;
        }

        /// <summary>
        /// Called when the Lua engine is reloaded to refresh script events.
        /// </summary>
        public override void LuaEngineReload()
        {
            ReloadEvents();
        }
    }
}
