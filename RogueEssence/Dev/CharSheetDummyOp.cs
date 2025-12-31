using System;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using RogueEssence.Content;
using RogueEssence.Dev;

namespace RogueEssence.Dev
{
    /// <summary>
    /// A placeholder character sheet operation that performs no actual modifications.
    /// Used as a dummy entry in operation lists or for testing purposes.
    /// </summary>
    [Serializable]
    public class CharSheetDummyOp : CharSheetOp
    {
        /// <inheritdoc/>
        public override int[] Anims { get { return new int[0]; } }

        private string name;

        /// <inheritdoc/>
        public override string Name { get { return name; } }

        /// <summary>
        /// Initializes a new instance of the CharSheetDummyOp class with the specified name.
        /// </summary>
        /// <param name="name">The display name for this dummy operation.</param>
        public CharSheetDummyOp(string name)
        {
            this.name = name;
        }

        /// <inheritdoc/>
        public override void Apply(CharSheet sheet, int anim) { }
    }

}
