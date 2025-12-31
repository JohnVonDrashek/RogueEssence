using RogueElements;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// A vertical menu with a title bar and divider at the top.
    /// Extends <see cref="SingleStripMenu"/> to add a title element above the choices.
    /// </summary>
    public abstract class TitledStripMenu : SingleStripMenu
    {
        /// <summary>
        /// The vertical offset in pixels for the title area.
        /// </summary>
        public const int TITLE_OFFSET = 16;

        /// <inheritdoc/>
        public override int ContentOffset { get { return TITLE_OFFSET; } }

        /// <summary>
        /// The title text element displayed at the top of the menu.
        /// </summary>
        public MenuText Title;

        /// <summary>
        /// Initializes the titled menu with specified parameters.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        protected virtual void Initialize(Loc start, int width, string title, IChoosable[] choices, int defaultChoice)
        {
            Initialize(start, width, title, choices, defaultChoice, choices.Length);
        }

        /// <summary>
        /// Initializes the titled menu with specified parameters and total spaces.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        /// <param name="totalSpaces">The total number of spaces in the menu.</param>
        protected virtual void Initialize(Loc start, int width, string title, IChoosable[] choices, int defaultChoice, int totalSpaces)
        {
            Initialize(start, width, title, choices, defaultChoice, choices.Length, -1);
        }

        /// <summary>
        /// Initializes the titled menu with specified parameters, total spaces, and multi-select support.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        /// <param name="totalSpaces">The total number of spaces in the menu.</param>
        /// <param name="multiSelect">The maximum number of simultaneous selections (-1 to disable).</param>
        protected virtual void Initialize(Loc start, int width, string title, IChoosable[] choices, int defaultChoice, int totalSpaces, int multiSelect)
        {
            base.Initialize(start, width, choices, defaultChoice, totalSpaces, multiSelect);
            IncludeTitle(title);
        }

        /// <summary>
        /// Adds the title text and divider elements to the menu.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        protected void IncludeTitle(string title)
        {
            Title = new MenuText(MenuLabel.TITLE, title, new Loc(GraphicsManager.MenuBG.TileWidth * 2, GraphicsManager.MenuBG.TileHeight));
            NonChoices.Add(Title);
            NonChoices.Add(new MenuDivider(MenuLabel.DIV, new Loc(GraphicsManager.MenuBG.TileWidth, GraphicsManager.MenuBG.TileHeight + LINE_HEIGHT), Bounds.Width - GraphicsManager.MenuBG.TileWidth * 2));
        }

        /// <inheritdoc/>
        public override void ImportChoices(params IChoosable[] choices)
        {
            base.ImportChoices(choices);
            int index = GetNonChoiceIndexByLabel(MenuLabel.DIV);
            if (index >= 0 && NonChoices[index] is MenuDivider divider)
                divider.Length = Bounds.Width - GraphicsManager.MenuBG.TileWidth * 2;
        }
    }
}
