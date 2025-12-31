using System;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using RogueEssence.Content;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Abstract base class for character sheet operations.
    /// Defines operations that can be applied to character sprite sheets during processing.
    /// </summary>
    [Serializable]
    public abstract class CharSheetOp
    {
        /// <summary>
        /// Gets the animation indices that this operation applies to.
        /// </summary>
        public abstract int[] Anims { get; }

        /// <summary>
        /// Gets the display name of this operation.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Applies this operation to the specified character sheet for a given animation.
        /// </summary>
        /// <param name="sheet">The character sheet to modify.</param>
        /// <param name="anim">The animation index to apply the operation to.</param>
        public abstract void Apply(CharSheet sheet, int anim);
    }

}
