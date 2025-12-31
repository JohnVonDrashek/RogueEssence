using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for menus that present selectable choices to the user.
    /// Provides cursor management, choice navigation, and selection handling.
    /// </summary>
    public abstract class ChoiceMenu : InteractableMenu
    {
        /// <summary>
        /// Gets the collection of non-selectable elements in this menu.
        /// </summary>
        public List<IMenuElement> NonChoices { get { return Elements; } }

        /// <summary>
        /// The collection of selectable choices in this menu.
        /// </summary>
        public List<IChoosable> Choices;

        /// <summary>
        /// Gets the currently hovered choice, or null if no choice is hovered.
        /// </summary>
        public IChoosable Hovered
        {
            get
            {
                foreach (IChoosable choice in Choices)
                {
                    if (choice.Hovered)
                        return choice;
                }
                return null;
            }
        }

        /// <summary>
        /// The cursor element that indicates the current selection.
        /// </summary>
        protected MenuCursor cursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceMenu"/> class.
        /// </summary>
        public ChoiceMenu()
        {
            cursor = new MenuCursor(MenuLabel.CURSOR, this, Dir4.Right);
            Choices = new List<IChoosable>();
        }

        /// <inheritdoc/>
        protected override IEnumerable<IMenuElement> GetDrawElements()
        {
            yield return cursor;
            foreach (IChoosable choice in Choices)
                yield return choice;
            foreach (IMenuElement nonChoice in NonChoices)
                yield return nonChoice;
        }

        /// <summary>
        /// Exports a copy of the current choices list.
        /// </summary>
        /// <returns>A new list containing all current choices.</returns>
        public virtual List<IChoosable> ExportChoices() => new(Choices);

        /// <summary>
        /// Imports a list of choices to replace the current choices.
        /// </summary>
        /// <param name="choices">The list of choices to import.</param>
        public void ImportChoices(List<IChoosable> choices)
        {
            ImportChoices(choices.ToArray());
        }

        /// <summary>
        /// Imports an array of choices to replace the current choices.
        /// </summary>
        /// <param name="choices">The choices to import.</param>
        public abstract void ImportChoices(params IChoosable[] choices);

        /// <summary>
        /// Gets the index of a choice by its label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The index of the choice with the specified label, or -1 if not found.</returns>
        public int GetChoiceIndexByLabel(string label)
        {
            return GetChoiceIndicesByLabel(label)[label];
        }

        /// <summary>
        /// Gets the indices of multiple choices by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their choice indices, with -1 for labels not found.</returns>
        public virtual Dictionary<string, int> GetChoiceIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, Choices);
        }

        /// <summary>
        /// Gets the index of a non-choice element by its label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The index of the non-choice element with the specified label, or -1 if not found.</returns>
        public int GetNonChoiceIndexByLabel(string label)
        {
            return GetNonChoiceIndicesByLabel(label)[label];
        }

        /// <summary>
        /// Gets the indices of multiple non-choice elements by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their non-choice element indices, with -1 for labels not found.</returns>
        public virtual Dictionary<string, int> GetNonChoiceIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, NonChoices);
        }
    }
}
