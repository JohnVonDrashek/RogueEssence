using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// A multi-page menu that supports left/right navigation between pages of choices.
    /// Extends <see cref="TitledStripMenu"/> to add pagination functionality.
    /// </summary>
    public abstract class MultiPageMenu : TitledStripMenu
    {
        /// <summary>
        /// Text element displaying the current page number (e.g., "(1/3)").
        /// </summary>
        public MenuText PageText;

        /// <summary>
        /// A 2D array of choices organized by page. Each inner array represents one page of choices.
        /// </summary>
        public IChoosable[][] TotalChoices;

        /// <summary>
        /// The index of the currently displayed page (0-based).
        /// </summary>
        public int CurrentPage;

        /// <summary>
        /// The maximum number of choices displayed per page.
        /// </summary>
        public int SpacesPerPage;

        /// <summary>
        /// Whether to show page numbers when there is only one page.
        /// </summary>
        public bool ShowPagesOnSingle;

        /// <summary>
        /// Gets the total index across all pages for the current selection.
        /// </summary>
        public int CurrentChoiceTotal { get => CurrentPage * SpacesPerPage + CurrentChoice; }

        /// <summary>
        /// Initializes the multi-page menu with the specified parameters.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="totalChoices">The 2D array of choices organized by page.</param>
        /// <param name="defaultChoice">The default choice index within the page.</param>
        /// <param name="defaultPage">The default page index.</param>
        /// <param name="spacesPerPage">The number of choices per page.</param>
        protected void Initialize(Loc start, int width, string title, IChoosable[][] totalChoices, int defaultChoice, int defaultPage, int spacesPerPage)
        {
            Initialize(start, width, title, totalChoices, defaultChoice, defaultPage, spacesPerPage, true, -1);
        }

        /// <summary>
        /// Initializes the multi-page menu with multi-select support.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="totalChoices">The 2D array of choices organized by page.</param>
        /// <param name="defaultChoice">The default choice index within the page.</param>
        /// <param name="defaultPage">The default page index.</param>
        /// <param name="spacesPerPage">The number of choices per page.</param>
        /// <param name="showPagesOnSingle">Whether to show page numbers when there is only one page.</param>
        /// <param name="multiSelect">The maximum number of simultaneous selections (-1 to disable).</param>
        protected void Initialize(Loc start, int width, string title, IChoosable[][] totalChoices, int defaultChoice, int defaultPage, int spacesPerPage, bool showPagesOnSingle, int multiSelect)
        {
            Initialize(start, width, title, totalChoices, defaultChoice, defaultPage, spacesPerPage, showPagesOnSingle, new IntRange(-1, multiSelect + 1));
        }

        /// <summary>
        /// Initializes the multi-page menu with full multi-select range configuration.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="title">The title text to display.</param>
        /// <param name="totalChoices">The 2D array of choices organized by page.</param>
        /// <param name="defaultChoice">The default choice index within the page.</param>
        /// <param name="defaultPage">The default page index.</param>
        /// <param name="spacesPerPage">The number of choices per page.</param>
        /// <param name="showPagesOnSingle">Whether to show page numbers when there is only one page.</param>
        /// <param name="multiSelect">The range defining minimum required and maximum allowed selections.</param>
        protected void Initialize(Loc start, int width, string title, IChoosable[][] totalChoices, int defaultChoice, int defaultPage, int spacesPerPage, bool showPagesOnSingle, IntRange multiSelect)
        {
            TotalChoices = totalChoices;
            SpacesPerPage = spacesPerPage;
            ShowPagesOnSingle = showPagesOnSingle;
            
            Bounds = new Rect(start, new Loc(width, spacesPerPage * VERT_SPACE + GraphicsManager.MenuBG.TileHeight * 2 + ContentOffset));
            MultiSelect = multiSelect;

            IncludeTitle(title);

            PageText = new MenuText("", new Loc(width - GraphicsManager.MenuBG.TileWidth, GraphicsManager.MenuBG.TileHeight), DirH.Right);
            NonChoices.Add(PageText);

            SetPage(defaultPage);
            CurrentChoice = defaultChoice;
        }

        /// <summary>
        /// Sorts a flat array of choices into pages of the specified size.
        /// </summary>
        /// <param name="choices">The flat array of all choices.</param>
        /// <param name="maxSlots">The maximum number of slots per page.</param>
        /// <returns>A 2D array of choices organized by page.</returns>
        protected static IChoosable[][] SortIntoPages(IChoosable[] choices, int maxSlots)
        {
            int pages = MathUtils.DivUp(choices.Length, maxSlots);
            int count = 0;
            List<IChoosable[]> box = new List<IChoosable[]>();
            for (int ii = 0; ii < pages; ii++)
            {
                box.Add(new IChoosable[Math.Min(choices.Length - maxSlots * ii, maxSlots)]);
                for (int jj = 0; jj < box[ii].Length; jj++)
                {
                    box[ii][jj] = choices[count];
                    count++;
                }
            }

            return box.ToArray();
        }

        /// <summary>
        /// Changes the currently displayed page and updates the page text.
        /// </summary>
        /// <param name="page">The page index to switch to.</param>
        protected virtual void SetPage(int page)
        {
            CurrentPage = page;
            if (TotalChoices.Length == 1 && !ShowPagesOnSingle)
                PageText.SetText("");
            else
                PageText.SetText("(" + (CurrentPage + 1) + "/" + TotalChoices.Length+ ")");
            IChoosable[] choices = new IChoosable[TotalChoices[CurrentPage].Length];
            for (int ii = 0; ii < choices.Length; ii++)
                choices[ii] = TotalChoices[CurrentPage][ii];
            SetChoices(choices);
            CurrentChoice = Math.Min(CurrentChoice, choices.Length - 1);
        }

        protected override void UpdateKeys(InputManager input)
        {
            bool moved = false;
            if (TotalChoices.Length > 1)
            {
                if (IsInputting(input, Dir8.Left))
                {
                    SetPage((CurrentPage + TotalChoices.Length - 1) % TotalChoices.Length);
                    moved = true;
                }
                else if (IsInputting(input, Dir8.Right))
                {
                    SetPage((CurrentPage + 1) % TotalChoices.Length);
                    moved = true;
                }
            }
            if (moved)
            {
                GameManager.Instance.SE("Menu/Skip");
                cursor.ResetTimeOffset();
            }
            else if (input.JustPressed(FrameInput.InputType.Confirm))
            {
                if (MultiSelect.Max > 0)
                {
                    List<int> slots = new List<int>();
                    for (int ii = 0; ii < TotalChoices.Length; ii++)
                    {
                        for (int jj = 0; jj < TotalChoices[ii].Length; jj++)
                        {
                            if (TotalChoices[ii][jj].Selected)
                                slots.Add(ii * SpacesPerPage + jj);
                        }
                    }

                    if (slots.Count >= MultiSelect.Min)
                    {
                        if (slots.Count > 0)
                        {
                            GameManager.Instance.SE("Menu/Confirm");
                            ChoseMultiIndex(slots);
                        }
                        else
                            Choices[CurrentChoice].OnConfirm();
                    }
                    else
                        GameManager.Instance.SE("Menu/Cancel");
                }
                else
                    Choices[CurrentChoice].OnConfirm();
            }
            else
                base.UpdateKeys(input);
        }

        /// <summary>
        /// Gets a choice by its total index across all pages.
        /// </summary>
        /// <param name="totalIndex">The total index across all pages.</param>
        /// <returns>The choice at the specified total index.</returns>
        public IChoosable GetTotalChoiceAtIndex(int totalIndex)
        {
            int page = totalIndex / SpacesPerPage;
            int index = totalIndex % SpacesPerPage;
            return TotalChoices[page][index];
        }

        public override List<IChoosable> ExportChoices()
        {
            List<IChoosable> allChoices = new List<IChoosable>();
            foreach (IChoosable[] page in TotalChoices)
                foreach (IChoosable choice in page)
                    allChoices.Add(choice);
            return allChoices;
        }
        public override void ImportChoices(params IChoosable[] choices)
        {
            TotalChoices = SortIntoPages(choices, SpacesPerPage);
            SetPage(CurrentPage);
        }

        public override Dictionary<string, int> GetChoiceIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, ExportChoices());
        }
    }
}
