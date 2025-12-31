using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for menus with vertical choice navigation.
    /// Provides keyboard and mouse input handling for navigating up/down through choices,
    /// along with multi-select support.
    /// </summary>
    public abstract class VertChoiceMenu : ChoiceMenu
    {
        /// <summary>
        /// Delegate for handling single slot selection.
        /// </summary>
        /// <param name="slot">The index of the selected slot.</param>
        public delegate void OnChooseSlot(int slot);

        /// <summary>
        /// Delegate for handling multi-selection confirmation.
        /// </summary>
        /// <param name="slot">The list of selected slot indices.</param>
        public delegate void OnMultiChoice(List<int> slot);

        private int currentChoice;

        /// <summary>
        /// Gets or sets the currently selected choice index.
        /// Setting this property updates the cursor position and triggers the ChoiceChanged callback.
        /// </summary>
        public int CurrentChoice
        {
            get { return currentChoice; }
            protected set
            {
                currentChoice = value;
                cursor.Loc = new Loc(GraphicsManager.MenuBG.TileWidth * 2 - 7, GraphicsManager.MenuBG.TileHeight + CurrentChoice * VERT_SPACE + ContentOffset);
                ChoiceChanged();
            }
        }

        /// <summary>
        /// Gets the vertical offset for content positioning, used by subclasses to add title space.
        /// </summary>
        public virtual int ContentOffset { get { return 0; } }

        private int hoveredChoice;
        private bool clicking;

        /// <summary>
        /// Gets or sets the range of allowed multi-selections. Min is the minimum required, Max-1 is the maximum allowed.
        /// </summary>
        public IntRange MultiSelect { get; protected set; }

        private int selectedTotal;

        /// <summary>
        /// Gets a value indicating whether the menu button is enabled.
        /// </summary>
        public virtual bool CanMenu { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether canceling is enabled.
        /// </summary>
        public virtual bool CanCancel { get { return true; } }

        /// <summary>
        /// Initializes the menu with the specified parameters.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        protected void Initialize(Loc start, int width, IChoosable[] choices, int defaultChoice)
        {
            Initialize(start, width, choices, defaultChoice, choices.Length, -1);
        }

        /// <summary>
        /// Initializes the menu with the specified parameters and multi-select support.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        /// <param name="totalSpaces">The total number of spaces in the menu.</param>
        /// <param name="multiSelect">The maximum number of simultaneous selections (-1 to disable).</param>
        protected void Initialize(Loc start, int width, IChoosable[] choices, int defaultChoice, int totalSpaces, int multiSelect)
        {
            Initialize(start, width, choices, defaultChoice, totalSpaces, new IntRange(-1, multiSelect+1));
        }

        /// <summary>
        /// Initializes the menu with the specified parameters and multi-select range.
        /// </summary>
        /// <param name="start">The top-left position of the menu.</param>
        /// <param name="width">The width of the menu in pixels.</param>
        /// <param name="choices">The array of selectable choices.</param>
        /// <param name="defaultChoice">The index of the initially selected choice.</param>
        /// <param name="totalSpaces">The total number of spaces in the menu.</param>
        /// <param name="multiSelect">The range defining minimum required and maximum allowed selections.</param>
        protected void Initialize(Loc start, int width, IChoosable[] choices, int defaultChoice, int totalSpaces, IntRange multiSelect)
        {
            Bounds = new Rect(start, new Loc(width, choices.Length * VERT_SPACE + GraphicsManager.MenuBG.TileHeight * 2 + ContentOffset));

            MultiSelect = multiSelect;

            SetChoices(choices);
            CurrentChoice = defaultChoice;
        }

        /// <summary>
        /// Sets the choices in the menu and calculates their bounds.
        /// </summary>
        /// <param name="choices">The array of choices to set.</param>
        protected void SetChoices(IChoosable[] choices)
        {
            Choices.Clear();
            for (int ii = 0; ii < choices.Length; ii++)
            {
                Choices.Add(choices[ii]);
                choices[ii].Bounds = new Rect(new Loc(GraphicsManager.MenuBG.TileWidth + 16 - 5, GraphicsManager.MenuBG.TileHeight + ContentOffset + VERT_SPACE * ii - 1),
                    new Loc(Bounds.Width - GraphicsManager.MenuBG.TileWidth * 2 - 16 + 5 - 4, VERT_SPACE - 2));
            }
        }

        /// <summary>
        /// Calculates the required width for the menu based on the text length of choices.
        /// </summary>
        /// <param name="choices">The choices to measure.</param>
        /// <param name="minWidth">The minimum width to return.</param>
        /// <returns>The calculated width, rounded up to the nearest multiple of 4.</returns>
        protected int CalculateChoiceLength(IEnumerable<IChoosable> choices, int minWidth)
        {
            int maxWidth = minWidth;
            foreach (IChoosable choice in choices)
            {
                if (choice is MenuTextChoice)
                {
                    MenuTextChoice textChoice = (MenuTextChoice)choice;
                    maxWidth = Math.Max(textChoice.Text.GetTextLength() + 16 + GraphicsManager.MenuBG.TileWidth * 2, maxWidth);
                }
            }
            maxWidth = MathUtils.DivUp(maxWidth, 4) * 4;
            return maxWidth;
        }

        /// <summary>
        /// Called when the current choice changes. Override to handle selection changes.
        /// </summary>
        protected virtual void ChoiceChanged() { }

        /// <summary>
        /// Called when multi-select state changes. Override to handle selection state updates.
        /// </summary>
        protected virtual void MultiSelectChanged() { }

        /// <inheritdoc/>
        public override void Update(InputManager input)
        {
            UpdateMouse(input);

            if (!clicking)
                UpdateKeys(input);
        }

        /// <summary>
        /// Processes mouse input for hover and click detection.
        /// </summary>
        /// <param name="input">The input manager containing the current input state.</param>
        protected virtual void UpdateMouse(InputManager input)
        {
            //when moused down on a selection, change currentchoice to that choice
            //find the choice it's hovered over
            int newHover = FindHoveredMenuChoice(input);
            if (hoveredChoice != newHover)
            {
                if (hoveredChoice > -1 && hoveredChoice < Choices.Count)
                    Choices[hoveredChoice].OnHoverChanged(false);
                if (newHover > -1)
                    Choices[newHover].OnHoverChanged(true);
                hoveredChoice = newHover;
            }
            if (input.JustPressed(FrameInput.InputType.LeftMouse))
            {
                if (newHover > -1)
                {
                    CurrentChoice = newHover;
                    clicking = true;
                }

                foreach (IChoosable choice in Choices)
                    choice.OnMouseState(true);
            }
            else if (input.JustReleased(FrameInput.InputType.LeftMouse))
            {
                clicking = false;
                foreach (IChoosable choice in Choices)
                    choice.OnMouseState(false);
            }
        }


        /// <summary>
        /// Processes keyboard and gamepad input for navigation and selection.
        /// </summary>
        /// <param name="input">The input manager containing the current input state.</param>
        protected virtual void UpdateKeys(InputManager input)
        {
            if (input.JustPressed(FrameInput.InputType.Confirm))
            {
                if (MultiSelect.Max > 0)
                {
                    List<int> slots = new List<int>();
                    for (int ii = 0; ii < Choices.Count; ii++)
                    {
                        if (Choices[ii].Selected)
                            slots.Add(ii);
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
            else if (input.JustPressed(FrameInput.InputType.Menu))
            {
                if (CanMenu)
                {
                    GameManager.Instance.SE("Menu/Cancel");
                    MenuPressed();
                }
            }
            else if (input.JustPressed(FrameInput.InputType.Cancel))
            {
                if (CanCancel)
                {
                    GameManager.Instance.SE("Menu/Cancel");
                    Canceled();
                }
            }
            else if (MultiSelect.Max > 0 && input.JustPressed(FrameInput.InputType.SelectItems))
            {
                int spaceLeft = MultiSelect.Max - 1 - selectedTotal;
                if (spaceLeft > 0 || Choices[CurrentChoice].Selected)
                {
                    Choices[CurrentChoice].OnSelect(!Choices[CurrentChoice].Selected);
                    if (Choices[CurrentChoice].Selected)
                        selectedTotal++;
                    else
                        selectedTotal--;
                    MultiSelectChanged();
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
            else
            {
                bool moved = false;
                if (Choices.Count > 1)
                {
                    if (IsInputting(input, Dir8.Down, Dir8.DownLeft, Dir8.DownRight))
                    {
                        moved = true;
                        CurrentChoice = (CurrentChoice + 1) % Choices.Count;
                    }
                    else if (IsInputting(input, Dir8.Up, Dir8.UpLeft, Dir8.UpRight))
                    {
                        moved = true;
                        CurrentChoice = (CurrentChoice + Choices.Count - 1) % Choices.Count;
                    }
                    if (moved)
                    {
                        GameManager.Instance.SE("Menu/Select");
                        cursor.ResetTimeOffset();
                    }
                }
            }
        }

        private int FindHoveredMenuChoice(InputManager input)
        {
            for (int ii = Choices.Count - 1; ii >= 0; ii--)
            {
                if (Collision.InBounds(Choices[ii].Bounds, input.MouseLoc / GraphicsManager.WindowZoom - Bounds.Start))
                    return ii;
            }
            return -1;
        }

        /// <summary>
        /// Called when the menu button is pressed. Must be implemented by subclasses.
        /// </summary>
        protected abstract void MenuPressed();

        /// <summary>
        /// Called when the cancel button is pressed. Must be implemented by subclasses.
        /// </summary>
        protected abstract void Canceled();

        /// <summary>
        /// Called when multi-selection is confirmed. Must be implemented by subclasses.
        /// </summary>
        /// <param name="slots">The list of selected slot indices.</param>
        protected abstract void ChoseMultiIndex(List<int> slots);

        /// <inheritdoc/>
        public override void ImportChoices(params IChoosable[] choices)
        {
            Initialize(Bounds.Start, CalculateChoiceLength(choices, 72), choices, Math.Min(CurrentChoice, choices.Length));
        }
    }

    /// <summary>
    /// A simple vertical menu that clears to checkpoint on menu press and removes itself on cancel.
    /// This is the most common base class for single-column choice menus.
    /// </summary>
    public abstract class SingleStripMenu : VertChoiceMenu
    {
        /// <inheritdoc/>
        protected override void MenuPressed()
        {
            MenuManager.Instance.ClearToCheckpoint();
        }

        /// <inheritdoc/>
        protected override void Canceled()
        {
            MenuManager.Instance.RemoveMenu();
        }

        /// <inheritdoc/>
        protected override void ChoseMultiIndex(List<int> slots) { }
    }
}
