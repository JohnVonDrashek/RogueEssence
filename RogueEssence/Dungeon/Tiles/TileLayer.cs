using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a layer of animated tile frames for rendering terrain or decorations.
    /// Supports multiple frames for animation and configurable frame timing.
    /// </summary>
    [Serializable]
    public class TileLayer
    {
        /// <summary>
        /// The list of frames in this tile animation.
        /// </summary>
        public List<TileFrame> Frames;

        /// <summary>
        /// The length of each frame in game ticks.
        /// </summary>
        public int FrameLength;

        /// <summary>
        /// Returns a string representation of this tile layer.
        /// </summary>
        /// <returns>A description of the layer and frame count.</returns>
        public override string ToString()
        {
            if (Frames.Count == 0)
                return String.Format("Empty Layer");
            else if (Frames.Count == 1)
                return Frames[0].ToString();
            else
                return String.Format("{0} ({1} Frames)", Frames[0].ToString(), Frames.Count);
        }

        /// <summary>
        /// Initializes a new empty TileLayer with default 60-tick frame length.
        /// </summary>
        public TileLayer()
        {
            Frames = new List<TileFrame>();
            FrameLength = 60;
        }

        /// <summary>
        /// Initializes a new empty TileLayer with the specified frame length.
        /// </summary>
        /// <param name="frameLength">The length of each frame in ticks.</param>
        public TileLayer(int frameLength)
        {
            Frames = new List<TileFrame>();
            FrameLength = frameLength;
        }

        /// <summary>
        /// Creates a copy of another TileLayer.
        /// </summary>
        /// <param name="other">The TileLayer to copy.</param>
        public TileLayer(TileLayer other)
        {
            Frames = new List<TileFrame>();
            for (int ii = 0; ii < other.Frames.Count; ii++)
                Frames.Add(other.Frames[ii]);
            FrameLength = other.FrameLength;
        }

        /// <summary>
        /// Initializes a new TileLayer with a single frame at the specified texture location.
        /// </summary>
        /// <param name="texture">The texture coordinates.</param>
        /// <param name="sheet">The sprite sheet name.</param>
        public TileLayer(Loc texture, string sheet)
            : this(new TileFrame(texture, sheet))
        { }

        /// <summary>
        /// Initializes a new TileLayer with a single frame.
        /// </summary>
        /// <param name="frame">The tile frame.</param>
        public TileLayer(TileFrame frame)
            : this()
        {
            Frames.Add(frame);
        }

        /// <summary>
        /// Draws this tile layer at the specified position with white color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The screen position to draw at.</param>
        /// <param name="totalTick">The total elapsed ticks for animation.</param>
        public void Draw(SpriteBatch spriteBatch, Loc pos, ulong totalTick)
        {
            Draw(spriteBatch, pos, totalTick, Color.White);
        }

        /// <summary>
        /// Draws this tile layer at the specified position with a color tint.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The screen position to draw at.</param>
        /// <param name="totalTick">The total elapsed ticks for animation.</param>
        /// <param name="color">The color tint to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Loc pos, ulong totalTick, Color color)
        {
            if (Frames.Count > 0)
            {
                int currentFrame = (int)(totalTick / (ulong)FrameTick.FrameToTick(FrameLength) % (ulong)Frames.Count);
                TileFrame frame = Frames[currentFrame];
                if (frame != TileFrame.Empty)
                {
                    BaseSheet texture = GraphicsManager.GetTile(frame);
                    texture.Draw(spriteBatch, pos.ToVector2(), null, color);
                }
            }
        }

        /// <summary>
        /// Determines whether this TileLayer equals another TileLayer.
        /// </summary>
        /// <param name="other">The TileLayer to compare.</param>
        /// <returns>True if frame length and all frames match; otherwise, false.</returns>
        public bool Equals(TileLayer other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (FrameLength != other.FrameLength)
                return false;
            if (Frames.Count != other.Frames.Count)
                return false;

            for (int ii = 0; ii < other.Frames.Count; ii++)
            {
                if (!Frames[0].Equals(other.Frames[0]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this TileLayer equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is an equal TileLayer; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj != null) && Equals(obj as TileLayer);
        }

        /// <summary>
        /// Tests equality between two TileLayer values.
        /// </summary>
        public static bool operator ==(TileLayer value1, TileLayer value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Tests inequality between two TileLayer values.
        /// </summary>
        public static bool operator !=(TileLayer value1, TileLayer value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Gets the hash code for this TileLayer.
        /// </summary>
        /// <returns>The base hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
