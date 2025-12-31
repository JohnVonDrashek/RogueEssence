using Microsoft.Xna.Framework.Graphics;
using RogueElements;
using System.Collections.Generic;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for menus that can receive and respond to user input.
    /// Extends <see cref="MenuBase"/> with interactivity features including input handling
    /// and support for attached summary menus.
    /// </summary>
    public abstract class InteractableMenu : MenuBase, IInteractable
    {
        /// <summary>
        /// The number of frames to wait before starting input repeat.
        /// </summary>
        const int INPUT_WAIT = 30;

        /// <summary>
        /// The number of frames between input repeats.
        /// </summary>
        const int INPUT_GAP = 6;

        /// <inheritdoc/>
        public virtual bool IsCheckpoint { get { return false; } }

        /// <inheritdoc/>
        public bool Inactive { get; set; }

        /// <inheritdoc/>
        public bool BlockPrevious { get; set; }

        /// <summary>
        /// Gets or sets the collection of summary menus displayed above this menu.
        /// </summary>
        public List<SummaryMenu> SummaryMenus { get; set; }

        /// <summary>
        /// Gets or sets the collection of summary menus displayed below this menu.
        /// </summary>
        public List<SummaryMenu> LowerSummaryMenus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractableMenu"/> class.
        /// </summary>
        public InteractableMenu()
        {
            SummaryMenus = new List<SummaryMenu>();
            LowerSummaryMenus = new List<SummaryMenu>();
        }

        /// <inheritdoc/>
        public abstract void Update(InputManager input);

        /// <inheritdoc/>
        public void ProcessActions(FrameTick elapsedTime) { }

        /// <summary>
        /// Determines if directional input is being registered, accounting for input repeat timing.
        /// </summary>
        /// <param name="input">The input manager containing the current input state.</param>
        /// <param name="dirs">The directions to check for input.</param>
        /// <returns>True if a valid directional input is detected; otherwise, false.</returns>
        public static bool IsInputting(InputManager input, params Dir8[] dirs)
        {
            bool choseDir = false;
            bool prevDir = false;
            foreach (Dir8 allowedDir in dirs)
            {
                if (input.Direction == allowedDir)
                    choseDir = true;
                if (input.PrevDirection == allowedDir)
                    prevDir = true;
            }

            bool atAdd = false;
            if (input.InputTime >= INPUT_WAIT)
            {
                if ((input.InputTime - input.AddedInputTime) / INPUT_GAP < input.InputTime / INPUT_GAP)
                    atAdd = true;
            }
            return (choseDir && (!prevDir || atAdd));
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            foreach (SummaryMenu menu in LowerSummaryMenus)
                menu.Draw(spriteBatch);

            base.Draw(spriteBatch);

            foreach (SummaryMenu menu in SummaryMenus)
                menu.Draw(spriteBatch);
        }

        /// <inheritdoc/>
        public override bool GetRelativeMouseLoc(Loc screenLoc, out MenuBase menu, out Loc? relativeLoc)
        {
            menu = null;
            relativeLoc = null;

            if (!Visible)
                return false;


            if (base.GetRelativeMouseLoc(screenLoc, out menu, out relativeLoc))
                return true;

            foreach (SummaryMenu summary in SummaryMenus)
            {
                if (summary.GetRelativeMouseLoc(screenLoc, out menu, out relativeLoc))
                    return true;
            }

            foreach (SummaryMenu summary in LowerSummaryMenus)
            {
                if (summary.GetRelativeMouseLoc(screenLoc, out menu, out relativeLoc))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the index of a summary menu by its label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The index of the summary menu with the specified label, or -1 if not found.</returns>
        public int GetSummaryIndexByLabel(string label)
        {
            return GetSummaryIndicesByLabel(label)[label];
        }

        /// <summary>
        /// Gets the indices of multiple summary menus by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their summary menu indices, with -1 for labels not found.</returns>
        public virtual Dictionary<string, int> GetSummaryIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, SummaryMenus);
        }

        /// <summary>
        /// Gets the indices of multiple lower summary menus by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their lower summary menu indices, with -1 for labels not found.</returns>
        public virtual Dictionary<string, int> GetLowerSummaryIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, LowerSummaryMenus);
        }
    }
}
