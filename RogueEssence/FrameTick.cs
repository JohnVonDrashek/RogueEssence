using System;

namespace RogueEssence
{
    /// <summary>
    /// Represents a time value with sub-frame precision for smooth animations and timing.
    /// Uses 120 ticks per frame for fine-grained time control.
    /// </summary>
    [Serializable]
    public struct FrameTick
    {
        /// <summary>
        /// The number of internal ticks per game frame.
        /// </summary>
        const int FRAME_TICKS_PER_FRAME = 120;

        /// <summary>
        /// The internal tick count.
        /// </summary>
        public long Ticks;

        /// <summary>
        /// Initializes a new instance of the FrameTick struct with the specified ticks.
        /// </summary>
        /// <param name="ticks">The internal tick count.</param>
        public FrameTick(long ticks)
        {
            Ticks = ticks;
        }

        /// <summary>
        /// Creates a FrameTick from a frame count.
        /// </summary>
        /// <param name="frames">The number of frames.</param>
        /// <returns>A FrameTick representing the specified number of frames.</returns>
        public static FrameTick FromFrames(long frames)
        {
            return new FrameTick(frames * FRAME_TICKS_PER_FRAME);
        }

        /// <summary>
        /// Gets a FrameTick representing zero time.
        /// </summary>
        public static FrameTick Zero
        {
            get { return new FrameTick(); }
        }

        /// <summary>
        /// Converts the tick count to whole frames.
        /// </summary>
        /// <returns>The number of complete frames.</returns>
        public int ToFrames()
        {
            return (int)(Ticks / FRAME_TICKS_PER_FRAME);
        }

        /// <summary>
        /// Converts ticks to frames.
        /// </summary>
        /// <param name="ticks">The tick count to convert.</param>
        /// <returns>The number of complete frames.</returns>
        public static ulong TickToFrames(ulong ticks)
        {
            return (ticks / FRAME_TICKS_PER_FRAME);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this FrameTick.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is FrameTick) && Equals((FrameTick)obj);
        }

        /// <summary>
        /// Determines whether the specified FrameTick is equal to this instance.
        /// </summary>
        /// <param name="other">The FrameTick to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public bool Equals(FrameTick other)
        {
            return Ticks == other.Ticks;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code based on the tick count.</returns>
        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }


        public static bool operator >(FrameTick value1, FrameTick value2)
        {
            return (value1.Ticks > value2.Ticks);
        }

        public static bool operator >=(FrameTick value1, FrameTick value2)
        {
            return (value1.Ticks >= value2.Ticks);
        }

        public static bool operator <(FrameTick value1, FrameTick value2)
        {
            return (value1.Ticks < value2.Ticks);
        }

        public static bool operator <=(FrameTick value1, FrameTick value2)
        {
            return (value1.Ticks <= value2.Ticks);
        }

        public static bool operator ==(FrameTick value1, FrameTick value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(FrameTick value1, FrameTick value2)
        {
            return !(value1 == value2);
        }

        public static FrameTick operator +(FrameTick value1, FrameTick value2)
        {
            return new FrameTick(value1.Ticks + value2.Ticks);
        }

        public static FrameTick operator -(FrameTick value1, FrameTick value2)
        {
            return new FrameTick(value1.Ticks - value2.Ticks);
        }

        public static FrameTick operator *(FrameTick value1, FrameTick value2)
        {
            return new FrameTick(value1.Ticks * value2.Ticks);
        }

        public static FrameTick operator /(FrameTick value1, FrameTick value2)
        {
            return new FrameTick(value1.Ticks / value2.Ticks);
        }

        public static FrameTick operator %(FrameTick value1, FrameTick value2)
        {
            return new FrameTick(value1.Ticks % value2.Ticks);
        }



        public static FrameTick operator +(FrameTick value1, long value2)
        {
            return new FrameTick(value1.Ticks + FrameToTick(value2));
        }

        public static FrameTick operator -(FrameTick value1, long value2)
        {
            return new FrameTick(value1.Ticks - FrameToTick(value2));
        }

        public static FrameTick operator *(FrameTick value1, long value2)
        {
            return new FrameTick(value1.Ticks * value2);
        }

        public static FrameTick operator /(FrameTick value1, long value2)
        {
            return new FrameTick(value1.Ticks / value2);
        }

        public static FrameTick operator %(FrameTick value1, long value2)
        {
            return new FrameTick(value1.Ticks % FrameToTick(value2));
        }


        /// <summary>
        /// Converts frames to internal ticks.
        /// </summary>
        /// <param name="frames">The number of frames to convert.</param>
        /// <returns>The equivalent number of internal ticks.</returns>
        public static long FrameToTick(long frames)
        {
            return frames * FRAME_TICKS_PER_FRAME;
        }

        /// <summary>
        /// Calculates how many times the specified time period fits into this tick count.
        /// </summary>
        /// <param name="time2">The time period in frames.</param>
        /// <returns>The number of complete time periods.</returns>
        public int DivOf(long time2)
        {
            return (int)(Ticks / FrameToTick(time2));
        }

        /// <summary>
        /// Calculates the fraction of this tick count relative to the specified time.
        /// </summary>
        /// <param name="time2">The time period in frames.</param>
        /// <returns>A value between 0 and 1 representing the fraction.</returns>
        public float FractionOf(int time2)
        {
            return (float)Ticks / FrameToTick(time2);
        }

        /// <summary>
        /// Calculates a scaled fraction of this tick count.
        /// </summary>
        /// <param name="frac">The numerator for scaling.</param>
        /// <param name="time2">The time period in frames.</param>
        /// <returns>The scaled fraction value.</returns>
        public long FractionOf(long frac, long time2)
        {
            return Ticks * frac / FrameToTick(time2);
        }

        /// <summary>
        /// Calculates a scaled fraction of this tick count relative to another FrameTick.
        /// </summary>
        /// <param name="frac">The numerator for scaling.</param>
        /// <param name="time2">The FrameTick to compare against.</param>
        /// <returns>The scaled fraction value.</returns>
        public long FractionOf(long frac, FrameTick time2)
        {
            return Ticks * frac / time2.Ticks;
        }


        public static bool operator >(FrameTick value1, long value2)
        {
            return (value1.Ticks > FrameToTick(value2));
        }

        public static bool operator >=(FrameTick value1, long value2)
        {
            return (value1.Ticks >= FrameToTick(value2));
        }

        public static bool operator <(FrameTick value1, long value2)
        {
            return (value1.Ticks < FrameToTick(value2));
        }

        public static bool operator <=(FrameTick value1, long value2)
        {
            return (value1.Ticks <= FrameToTick(value2));
        }

        public static bool operator ==(FrameTick value1, long value2)
        {
            return (value1.Ticks == FrameToTick(value2));
        }

        public static bool operator !=(FrameTick value1, long value2)
        {
            return !(value1 == value2);
        }
    }
}
