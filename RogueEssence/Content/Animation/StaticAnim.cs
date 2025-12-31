using System;
using RogueElements;

namespace RogueEssence.Content
{
    /// <summary>
    /// A static animation that plays at a fixed location.
    /// Can be emitted by particle systems or placed directly in the scene.
    /// </summary>
    [Serializable]
    public class StaticAnim : LoopingAnim, IEmittable
    {
        /// <summary>
        /// Creates a new empty StaticAnim.
        /// </summary>
        public StaticAnim() { }

        /// <summary>
        /// Creates a StaticAnim with the specified animation, playing once.
        /// </summary>
        /// <param name="anim">The animation data to use.</param>
        public StaticAnim(AnimData anim) : this(anim, 1, 0) { }

        /// <summary>
        /// Creates a StaticAnim with the specified animation and cycle count.
        /// </summary>
        /// <param name="anim">The animation data to use.</param>
        /// <param name="cycles">The number of times to loop the animation.</param>
        public StaticAnim(AnimData anim, int cycles) : this(anim, cycles, 0) { }

        /// <summary>
        /// Creates a StaticAnim with full control over playback.
        /// </summary>
        /// <param name="anim">The animation data to use.</param>
        /// <param name="cycles">The number of times to loop the animation.</param>
        /// <param name="totalTime">The total duration in frames, or 0 for cycle-based.</param>
        public StaticAnim(AnimData anim, int cycles, int totalTime)
            : base(anim, totalTime, cycles) { }


        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The StaticAnim to copy.</param>
        protected StaticAnim(StaticAnim other) : base(other) { }

        /// <summary>
        /// Creates a clone of this animation for emission.
        /// </summary>
        /// <returns>A new copy of this StaticAnim.</returns>
        public virtual IEmittable CloneIEmittable() { return new StaticAnim(this); }

        /// <summary>
        /// Creates a positioned instance of this animation at the specified location.
        /// </summary>
        /// <param name="startLoc">The starting position in map coordinates.</param>
        /// <param name="startHeight">The height above the ground.</param>
        /// <param name="dir">The direction the animation faces.</param>
        /// <returns>A new positioned animation instance.</returns>
        public IEmittable CreateStatic(Loc startLoc, int startHeight, Dir8 dir)
        {
            StaticAnim anim = (StaticAnim)CloneIEmittable();
            anim.SetupEmitted(startLoc, startHeight, dir);
            return anim;
        }

        /// <summary>
        /// Initializes the animation at the specified location and direction.
        /// </summary>
        /// <param name="startLoc">The starting position in map coordinates.</param>
        /// <param name="startHeight">The height above the ground.</param>
        /// <param name="dir">The direction the animation faces.</param>
        public virtual void SetupEmitted(Loc startLoc, int startHeight, Dir8 dir)
        {
            mapLoc = startLoc;
            locHeight = startHeight;
            Direction = dir;
            SetUp();
        }
    }
}
