using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RogueEssence.Dev.ViewModels;
using RogueEssence.Dungeon;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Collections;
using Avalonia.Styling;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A combo box that supports type-ahead search by letter keys.
    /// Users can type letters to quickly jump to matching items.
    /// </summary>
    public class SearchComboBox : ComboBox, IStyleable
    {
        /// <summary>
        /// Gets the style key for this control, returning ComboBox for styling inheritance.
        /// </summary>
        Type IStyleable.StyleKey => typeof(ComboBox);

        private string workingSearch;
        private bool[] processedKey;

        /// <summary>
        /// Initializes a new instance of the SearchComboBox class.
        /// </summary>
        public SearchComboBox() : base()
        {
            workingSearch = "";
            processedKey = new bool[26];
        }

        /// <summary>
        /// Handles key down events to build search string and jump to matching items.
        /// </summary>
        /// <param name="e">The key event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                int idx = e.Key - Key.A;
                if (!processedKey[idx])
                {
                    processedKey[idx] = true;
                    char letter = (char)(idx + 'A');
                    workingSearch = workingSearch + letter.ToString();
                    int letterIndex = 0;
                    foreach (string obj in Items)
                    {
                        if (obj.StartsWith(workingSearch, StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.ScrollIntoView(letterIndex);
                            break;
                        }
                        letterIndex++;
                    }
                }
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handles key up events to reset key tracking state.
        /// </summary>
        /// <param name="e">The key event arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                int idx = e.Key - Key.A;
                processedKey[idx] = false;
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Handles pointer moved events to clear the search string.
        /// </summary>
        /// <param name="e">The pointer event arguments.</param>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            workingSearch = "";
            base.OnPointerMoved(e);
        }
    }
}
