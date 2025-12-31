using System;
using System.Collections.Generic;
using System.Text;
using RogueEssence;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.Data;
using ReactiveUI;
using System.Collections.ObjectModel;
using RogueEssence.Content;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the animation choice dialog.
    /// Allows users to select an animation from a list of available animations.
    /// </summary>
    public class AnimChoiceViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the AnimChoiceViewModel class.
        /// </summary>
        /// <param name="anims">Array of animation indices to display as choices.</param>
        public AnimChoiceViewModel(int[] anims)
        {
            Anims = new ObservableCollection<string>();
            foreach (int anim in anims)
                Anims.Add(GraphicsManager.Actions[anim].Name);
        }

        public ObservableCollection<string> Anims { get; }

        private int chosenAnim;
        public int ChosenAnim
        {
            get => chosenAnim;
            set => this.SetIfChanged(ref chosenAnim, value);
        }

    }
}
