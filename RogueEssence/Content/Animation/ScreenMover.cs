using System;
using RogueElements;

namespace RogueEssence.Content
{
    /// <summary>
    /// Controls screen shake effects by adding random offsets to the camera position.
    /// Shake intensity decreases over time from max to min.
    /// </summary>
    [Serializable]
    public class ScreenMover
    {
        public bool Finished { get { return (ShakeTime.ToFrames() >= MaxShakeTime); } }

        public int MinShake;
        [Dev.SharedRow]
        public int MaxShake;
        public int MaxShakeTime;

        [NonSerialized]
        public FrameTick ShakeTime;

        public ScreenMover()
        {

        }
        public ScreenMover(int minShake, int maxShake, int shakeTime)
        {
            MinShake = minShake;
            MaxShake = maxShake;
            MaxShakeTime = shakeTime;
        }
        public ScreenMover(ScreenMover other)
        {
            MaxShake = other.MaxShake;
            MinShake = other.MinShake;
            MaxShakeTime = other.MaxShakeTime;
        }

        public void Update(FrameTick elapsedTime, ref Loc offsetLoc)
        {
            ShakeTime += elapsedTime;
            if (ShakeTime < MaxShakeTime)
            {
                int divShake = MinShake + (int)ShakeTime.FractionOf(MaxShake, MaxShakeTime);
                offsetLoc += new Loc(MathUtils.Rand.Next(divShake), MathUtils.Rand.Next(divShake));
            }
            //else
            //    offsetLoc = new Loc();
        }
    }
}
