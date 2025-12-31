using System;
using System.Collections.Generic;
using System.Text;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using System.Drawing;
using RogueElements;
using Avalonia.Controls;
using RogueEssence.Dev.Views;
using System.Collections;
using Avalonia;
using System.Reactive.Subjects;
using RogueEssence.LevelGen;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for IMultiRandPicker objects. Enables subgroup display for complex multi-random picker editing.
    /// </summary>
    public class MultiRandPickerEditor : Editor<IMultiRandPicker>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;
    }
}
