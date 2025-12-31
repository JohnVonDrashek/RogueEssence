using System;
using System.Collections.Generic;
using RogueEssence.Content;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Abstract base class for canvas stroke operations used in map editing.
    /// Represents a brush stroke that can be applied to tiles on a canvas.
    /// </summary>
    /// <typeparam name="T">The type of brush value applied by the stroke.</typeparam>
    public abstract class CanvasStroke<T>
    {
        /// <summary>
        /// Gets the brush value to apply at the specified location.
        /// </summary>
        /// <param name="loc">The location to get the brush for.</param>
        /// <returns>The brush value for the specified location.</returns>
        public abstract T GetBrush(Loc loc);

        /// <summary>
        /// Gets the rectangular area covered by this stroke.
        /// </summary>
        public abstract Rect CoveredRect { get; }

        /// <summary>
        /// Sets the end point of the stroke, defining its extent.
        /// </summary>
        /// <param name="loc">The end location of the stroke.</param>
        public abstract void SetEnd(Loc loc);

        /// <summary>
        /// Determines whether the specified location is included in this stroke.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the location is part of this stroke; otherwise, false.</returns>
        public abstract bool IncludesLoc(Loc loc);

        /// <summary>
        /// Gets all locations affected by this stroke.
        /// </summary>
        /// <returns>An enumerable of all locations in this stroke.</returns>
        public abstract IEnumerable<Loc> GetLocs();



        /// <summary>
        /// Delegate for creating a new canvas stroke.
        /// </summary>
        /// <returns>A new canvas stroke instance.</returns>
        public delegate CanvasStroke<T> StrokeCreator();

        /// <summary>
        /// Delegate for executing an action on a canvas stroke.
        /// </summary>
        /// <param name="stroke">The stroke to act upon.</param>
        public delegate void StrokeAction(CanvasStroke<T> stroke);

        /// <summary>
        /// Processes canvas input for drawing operations, handling mouse button states and stroke creation.
        /// </summary>
        /// <param name="input">The input manager containing mouse state.</param>
        /// <param name="tileCoords">The current tile coordinates under the cursor.</param>
        /// <param name="inWindow">Whether the cursor is within the canvas window.</param>
        /// <param name="createStroke">Factory method for creating a draw stroke.</param>
        /// <param name="deleteStroke">Factory method for creating a delete stroke.</param>
        /// <param name="strokeAction">Action to execute when a stroke is completed.</param>
        /// <param name="pendingStroke">Reference to the current pending stroke being drawn.</param>
        public static void ProcessCanvasInput(InputManager input, Loc tileCoords, bool inWindow, StrokeCreator createStroke, StrokeCreator deleteStroke, StrokeAction strokeAction, ref CanvasStroke<T> pendingStroke)
        {
            if (input.JustPressed(FrameInput.InputType.LeftMouse) && inWindow)
                pendingStroke = createStroke();
            else if (pendingStroke != null && input[FrameInput.InputType.LeftMouse])
                pendingStroke.SetEnd(tileCoords);
            else if (pendingStroke != null && input.JustReleased(FrameInput.InputType.LeftMouse))
            {
                strokeAction(pendingStroke);
                pendingStroke = null;
            }
            else if (input.JustPressed(FrameInput.InputType.RightMouse) && inWindow)
                pendingStroke = deleteStroke();
            else if (pendingStroke != null && input[FrameInput.InputType.RightMouse])
                pendingStroke.SetEnd(tileCoords);
            else if (pendingStroke != null && input.JustReleased(FrameInput.InputType.RightMouse))
            {
                strokeAction(pendingStroke);
                pendingStroke = null;
            }
        }
    }

    /// <summary>
    /// A canvas stroke that fills a rectangular area with a uniform brush value.
    /// The rectangle is defined by a start point and an end point.
    /// </summary>
    /// <typeparam name="T">The type of brush value applied by the stroke.</typeparam>
    public class RectStroke<T> : CanvasStroke<T>
    {
        private T brush;
        private Loc start;
        private Loc end;

        /// <inheritdoc/>
        public override T GetBrush(Loc loc) { return brush; }

        private Rect coveredRect;

        /// <inheritdoc/>
        public override Rect CoveredRect { get { return coveredRect; } }

        /// <summary>
        /// Initializes a new instance of the RectStroke class with a starting location and brush value.
        /// </summary>
        /// <param name="start">The starting corner of the rectangle.</param>
        /// <param name="brush">The brush value to apply to the rectangle.</param>
        public RectStroke(Loc start, T brush)
        {
            this.brush = brush;
            this.start = start;
            SetEnd(start);
        }

        /// <inheritdoc/>
        public override bool IncludesLoc(Loc loc)
        {
            return coveredRect.Contains(loc);
        }

        /// <inheritdoc/>
        public override void SetEnd(Loc loc)
        {
            this.end = loc;
            Rect resultRect = Rect.FromPoints(start, end);
            if (resultRect.Size.X <= 0)
            {
                resultRect.Start = new Loc(resultRect.Start.X + resultRect.Size.X, resultRect.Start.Y);
                resultRect.Size = new Loc(-resultRect.Size.X + 1, resultRect.Size.Y);
            }
            else
                resultRect.Size = new Loc(resultRect.Size.X + 1, resultRect.Size.Y);

            if (resultRect.Size.Y <= 0)
            {
                resultRect.Start = new Loc(resultRect.Start.X, resultRect.Start.Y + resultRect.Size.Y);
                resultRect.Size = new Loc(resultRect.Size.X, -resultRect.Size.Y + 1);
            }
            else
                resultRect.Size = new Loc(resultRect.Size.X, resultRect.Size.Y + 1);
            coveredRect = resultRect;
        }

        /// <inheritdoc/>
        public override IEnumerable<Loc> GetLocs()
        {
            for (int yy = coveredRect.Y; yy < coveredRect.Bottom; yy++)
            {
                for (int xx = coveredRect.X; xx < coveredRect.Right; xx++)
                    yield return new Loc(xx, yy);
            }
        }
    }

    /// <summary>
    /// A canvas stroke that accumulates individual locations as they are drawn.
    /// Used for freehand drawing operations where the user paints over specific tiles.
    /// </summary>
    /// <typeparam name="T">The type of brush value applied by the stroke.</typeparam>
    public class DrawStroke<T> : CanvasStroke<T>
    {
        private T brush;
        private HashSet<Loc> locs;

        /// <inheritdoc/>
        public override T GetBrush(Loc loc) { return brush; }

        private Rect coveredRect;

        /// <inheritdoc/>
        public override Rect CoveredRect { get { return coveredRect; } }

        /// <summary>
        /// Initializes a new instance of the DrawStroke class with a starting location and brush value.
        /// </summary>
        /// <param name="start">The initial location of the stroke.</param>
        /// <param name="brush">The brush value to apply to painted locations.</param>
        public DrawStroke(Loc start, T brush)
        {
            this.brush = brush;
            locs = new HashSet<Loc>();
            coveredRect = new Rect(start, Loc.Zero);
            SetEnd(start);
        }

        /// <inheritdoc/>
        public override bool IncludesLoc(Loc loc)
        {
            return locs.Contains(loc);
        }

        /// <inheritdoc/>
        public override void SetEnd(Loc loc)
        {
            locs.Add(loc);
            coveredRect = Rect.IncludeLoc(coveredRect, loc);
        }

        /// <inheritdoc/>
        public override IEnumerable<Loc> GetLocs()
        {
            foreach (Loc loc in locs)
                yield return loc;
        }
    }

    /// <summary>
    /// A canvas stroke that applies a 2D array of brush values as a cluster/stamp.
    /// Used for placing multi-tile patterns at a specific location.
    /// </summary>
    /// <typeparam name="T">The type of brush value applied by the stroke.</typeparam>
    public class ClusterStroke<T> : CanvasStroke<T>
    {
        private T[][] brush;
        private Loc loc;

        /// <inheritdoc/>
        public override T GetBrush(Loc loc)
        {
            Loc checkLoc = loc - this.loc;
            return brush[checkLoc.X][checkLoc.Y];
        }

        private Rect coveredRect;

        /// <inheritdoc/>
        public override Rect CoveredRect { get { return coveredRect; } }

        /// <summary>
        /// Initializes a new instance of the ClusterStroke class with a starting location and 2D brush pattern.
        /// </summary>
        /// <param name="start">The top-left location where the cluster will be placed.</param>
        /// <param name="brush">A 2D array of brush values defining the cluster pattern.</param>
        public ClusterStroke(Loc start, T[][] brush)
        {
            this.brush = brush;
            SetEnd(start);
        }

        /// <inheritdoc/>
        public override bool IncludesLoc(Loc loc)
        {
            return coveredRect.Contains(loc);
        }

        /// <inheritdoc/>
        public override void SetEnd(Loc loc)
        {
            this.loc = loc;
            coveredRect = new Rect(loc, new Loc(brush.Length, brush[0].Length));
        }

        /// <inheritdoc/>
        public override IEnumerable<Loc> GetLocs()
        {
            for (int xx = 0; xx < brush.Length; xx++)
            {
                for (int yy = 0; yy < brush[0].Length; yy++)
                    yield return loc + new Loc(xx, yy);
            }
        }
    }

    /// <summary>
    /// A canvas stroke that represents a fill operation at a single location.
    /// Used for flood-fill style operations that start from a point.
    /// </summary>
    /// <typeparam name="T">The type of brush value applied by the stroke.</typeparam>
    public class FillStroke<T> : CanvasStroke<T>
    {
        private T brush;
        private Loc loc;

        /// <inheritdoc/>
        public override T GetBrush(Loc loc)
        {
            return brush;
        }

        private Rect coveredRect;

        /// <inheritdoc/>
        public override Rect CoveredRect { get { return coveredRect; } }

        /// <summary>
        /// Initializes a new instance of the FillStroke class with a starting location and brush value.
        /// </summary>
        /// <param name="start">The location where the fill operation starts.</param>
        /// <param name="brush">The brush value to apply during the fill.</param>
        public FillStroke(Loc start, T brush)
        {
            this.brush = brush;
            SetEnd(start);
        }

        /// <inheritdoc/>
        public override bool IncludesLoc(Loc loc)
        {
            return coveredRect.Contains(loc);
        }

        /// <inheritdoc/>
        public override void SetEnd(Loc loc)
        {
            this.loc = loc;
            coveredRect = new Rect(loc, Loc.One);
        }

        /// <inheritdoc/>
        public override IEnumerable<Loc> GetLocs()
        {
            yield return loc;
        }
    }

    /// <summary>
    /// Abstract base class for undoable drawing operations on a canvas.
    /// Stores the previous state of affected tiles to enable undo/redo functionality.
    /// </summary>
    /// <typeparam name="T">The type of values stored at each location.</typeparam>
    public abstract class DrawUndo<T> : Undoable
    {
        private Dictionary<Loc, T> brush;
        private Dictionary<Loc, T> prevStates;

        /// <summary>
        /// Initializes a new instance of the DrawUndo class with the specified brush data.
        /// </summary>
        /// <param name="brush">A dictionary mapping locations to their new values.</param>
        public DrawUndo(Dictionary<Loc, T> brush)
        {
            this.brush = brush;
        }

        /// <summary>
        /// Gets the current value at the specified location.
        /// </summary>
        /// <param name="loc">The location to retrieve the value from.</param>
        /// <returns>The current value at the location.</returns>
        protected abstract T GetValue(Loc loc);

        /// <summary>
        /// Sets the value at the specified location.
        /// </summary>
        /// <param name="loc">The location to set the value at.</param>
        /// <param name="val">The value to set.</param>
        protected abstract void SetValue(Loc loc, T val);

        /// <summary>
        /// Called after all values have been set during an undo or redo operation.
        /// Override to perform cleanup or notification after changes are applied.
        /// </summary>
        protected virtual void ValuesFinished() { }

        /// <inheritdoc/>
        public override void Apply()
        {
            prevStates = new Dictionary<Loc, T>();

            foreach (Loc loc in brush.Keys)
                prevStates[loc] = GetValue(loc);

            Redo();
        }

        /// <inheritdoc/>
        public override void Redo()
        {
            foreach (Loc loc in brush.Keys)
                SetValue(loc, brush[loc]);
            ValuesFinished();
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            foreach (Loc loc in prevStates.Keys)
                SetValue(loc, prevStates[loc]);
            ValuesFinished();
        }
    }
}
