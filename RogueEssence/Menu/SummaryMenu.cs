using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// A non-interactive menu panel used to display supplementary information.
    /// Summary menus are typically attached to interactive menus to show details
    /// about the currently selected item, such as item descriptions or character stats.
    /// </summary>
    public class SummaryMenu : MenuBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryMenu"/> class with a label and bounds.
        /// </summary>
        /// <param name="label">The identifier label for this summary menu.</param>
        /// <param name="bounds">The rectangular bounds defining position and size.</param>
        public SummaryMenu(string label, Rect bounds) : this(bounds) { Label = label; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryMenu"/> class with specified bounds.
        /// </summary>
        /// <param name="bounds">The rectangular bounds defining position and size.</param>
        public SummaryMenu(Rect bounds) { Bounds = bounds; }
    }
}
