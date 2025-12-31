// =============================================================================
// EXAMPLE: Custom Menu
// =============================================================================
// This file demonstrates how to create custom menus in RogueEssence.
// Menus are used for player interaction: choices, information display,
// item selection, team management, etc.
//
// The menu system is built on several base classes:
// - MenuBase: Base for all menus (positioning, visibility)
// - InteractableMenu: Adds input handling
// - SingleStripMenu: Single column of choices
// - MultiPageMenu: Scrollable multi-page menus
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Menu;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Examples
{
    // =========================================================================
    // EXAMPLE 1: Simple Choice Menu
    // =========================================================================
    /// <summary>
    /// A simple menu with text choices.
    /// Inherits from SingleStripMenu for vertical choice layout.
    /// </summary>
    public class SimpleChoiceMenu : SingleStripMenu
    {
        // ---------------------------------------------------------------------
        // STEP 1: Define callback for when a choice is selected
        // ---------------------------------------------------------------------
        // Callbacks let the menu communicate results back to the caller.

        private Action<int> OnChoiceMade;

        // ---------------------------------------------------------------------
        // STEP 2: Constructor
        // ---------------------------------------------------------------------

        /// <summary>
        /// Creates a simple choice menu.
        /// </summary>
        /// <param name="title">Title displayed at the top.</param>
        /// <param name="choices">Array of choice strings.</param>
        /// <param name="defaultChoice">Initially selected choice index.</param>
        /// <param name="onChoice">Callback when a choice is confirmed.</param>
        public SimpleChoiceMenu(
            string title,
            string[] choices,
            int defaultChoice,
            Action<int> onChoice)
        {
            OnChoiceMade = onChoice;

            // -----------------------------------------------------------------
            // STEP 3: Create menu text choices
            // -----------------------------------------------------------------
            // MenuTextChoice is the standard choice element.
            // Parameters: (text, action, enabled, color)

            List<MenuTextChoice> menuChoices = new List<MenuTextChoice>();

            for (int i = 0; i < choices.Length; i++)
            {
                int choiceIndex = i;  // Capture for closure

                menuChoices.Add(new MenuTextChoice(
                    choices[i],                           // Display text
                    () => Choose(choiceIndex),            // Click action
                    true,                                 // Enabled
                    Color.White                           // Text color
                ));
            }

            // -----------------------------------------------------------------
            // STEP 4: Calculate menu dimensions
            // -----------------------------------------------------------------
            // The menu needs to know its size for positioning.

            int menuWidth = 0;
            foreach (string choice in choices)
            {
                // Calculate text width
                int textWidth = GraphicsManager.TextFont.SubstringWidth(choice);
                if (textWidth > menuWidth)
                    menuWidth = textWidth;
            }

            // Add padding for border
            menuWidth += GraphicsManager.MenuBG.TileWidth * 2 + MenuTextChoice.CHOICE_WIDTH;

            // -----------------------------------------------------------------
            // STEP 5: Initialize the menu
            // -----------------------------------------------------------------
            // Initialize sets up the menu elements and layout.

            // Calculate position (center of screen)
            int menuX = (GraphicsManager.ScreenWidth - menuWidth) / 2;
            int menuY = (GraphicsManager.ScreenHeight - (choices.Length * VERT_SPACE + GraphicsManager.MenuBG.TileHeight * 2)) / 2;

            Initialize(
                new Loc(menuX, menuY),     // Position
                menuWidth,                  // Width
                title,                      // Title text
                menuChoices.ToArray(),      // Choices array
                defaultChoice               // Default selection
            );
        }

        // ---------------------------------------------------------------------
        // STEP 6: Implement choice handling
        // ---------------------------------------------------------------------

        private void Choose(int index)
        {
            // Remove this menu from the stack
            MenuManager.Instance.RemoveMenu();

            // Invoke the callback with the chosen index
            OnChoiceMade?.Invoke(index);
        }

        // ---------------------------------------------------------------------
        // STEP 7: Optional - Override input handling
        // ---------------------------------------------------------------------
        // Override Update to add custom input handling.

        public override void Update(InputManager input)
        {
            // Call base to handle standard navigation
            base.Update(input);

            // Add custom input handling
            if (input.JustPressed(FrameInput.InputType.Cancel))
            {
                // Play cancel sound
                GameManager.Instance.SE("Menu/Cancel");

                // Remove menu
                MenuManager.Instance.RemoveMenu();

                // Return -1 for cancel
                OnChoiceMade?.Invoke(-1);
            }
        }
    }

    // =========================================================================
    // EXAMPLE 2: Information Display Menu
    // =========================================================================
    /// <summary>
    /// A menu that displays information without choices.
    /// Good for tutorials, help screens, item descriptions, etc.
    /// </summary>
    public class InfoDisplayMenu : InteractableMenu
    {
        private string[] lines;
        private string title;

        public InfoDisplayMenu(string menuTitle, string[] textLines)
        {
            title = menuTitle;
            lines = textLines;

            // Calculate dimensions based on content
            int maxWidth = 0;
            foreach (string line in lines)
            {
                int lineWidth = GraphicsManager.TextFont.SubstringWidth(line);
                if (lineWidth > maxWidth)
                    maxWidth = lineWidth;
            }

            int width = maxWidth + GraphicsManager.MenuBG.TileWidth * 2 + 16;
            int height = lines.Length * GraphicsManager.TextFont.CharHeight +
                         GraphicsManager.MenuBG.TileHeight * 2 + 16;

            // Add space for title if present
            if (!string.IsNullOrEmpty(title))
                height += GraphicsManager.TextFont.CharHeight + 8;

            // Center on screen
            Bounds = new Rect(
                (GraphicsManager.ScreenWidth - width) / 2,
                (GraphicsManager.ScreenHeight - height) / 2,
                width,
                height
            );
        }

        // ---------------------------------------------------------------------
        // Drawing the menu
        // ---------------------------------------------------------------------

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the menu background
            if (!Visible)
                return;

            // Draw background box
            GraphicsManager.MenuBG.Draw(spriteBatch,
                new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height),
                Color.White);

            int textX = Bounds.X + GraphicsManager.MenuBG.TileWidth + 8;
            int textY = Bounds.Y + GraphicsManager.MenuBG.TileHeight + 8;

            // Draw title
            if (!string.IsNullOrEmpty(title))
            {
                GraphicsManager.TextFont.DrawText(spriteBatch,
                    textX, textY,
                    title, null, DirV.Up, DirH.Left,
                    Color.Yellow);
                textY += GraphicsManager.TextFont.CharHeight + 8;
            }

            // Draw each line
            foreach (string line in lines)
            {
                GraphicsManager.TextFont.DrawText(spriteBatch,
                    textX, textY,
                    line, null, DirV.Up, DirH.Left,
                    Color.White);
                textY += GraphicsManager.TextFont.CharHeight;
            }
        }

        // ---------------------------------------------------------------------
        // Input handling
        // ---------------------------------------------------------------------

        public override void Update(InputManager input)
        {
            // Any button press closes this menu
            if (input.JustPressed(FrameInput.InputType.Confirm) ||
                input.JustPressed(FrameInput.InputType.Cancel) ||
                input.JustPressed(FrameInput.InputType.Menu))
            {
                GameManager.Instance.SE("Menu/Confirm");
                MenuManager.Instance.RemoveMenu();
            }
        }
    }

    // =========================================================================
    // EXAMPLE 3: Team Selection Menu
    // =========================================================================
    /// <summary>
    /// A menu for selecting a team member.
    /// Demonstrates multi-page menu and character display.
    /// </summary>
    public class TeamSelectMenu : MultiPageMenu
    {
        private Action<int> OnTeamMemberSelected;

        // Summary widget to show selected character info
        private SummaryMenu summaryMenu;

        public TeamSelectMenu(Action<int> onSelect)
        {
            OnTeamMemberSelected = onSelect;

            // Get team members
            ExplorerTeam team = DataManager.Instance.Save.ActiveTeam;

            // Create choices for each team member
            List<MenuChoice> choices = new List<MenuChoice>();

            for (int i = 0; i < team.Players.Count; i++)
            {
                Character member = team.Players[i];
                int memberIndex = i;

                // Create a choice element with icon and name
                MenuTextChoice choice = new MenuTextChoice(
                    member.GetDisplayName(true),
                    () => SelectMember(memberIndex),
                    true,
                    Color.White
                );

                choices.Add(choice);
            }

            // Initialize multi-page menu
            int width = 160;
            int height = Math.Min(team.Players.Count * VERT_SPACE + GraphicsManager.MenuBG.TileHeight * 2,
                                  200);

            Initialize(
                new Loc(GraphicsManager.ScreenWidth / 2 - width / 2, 8),
                width,
                Text.FormatKey("MENU_TEAM_TITLE"),
                choices.ToArray(),
                0,    // Default selection
                8,    // Choices per page
                true  // Can cancel
            );

            // Create summary panel
            summaryMenu = new MemberSummaryMenu(
                new Rect(0, height + 16, GraphicsManager.ScreenWidth, 80)
            );

            // Initial summary update
            UpdateSummary();
        }

        private void SelectMember(int index)
        {
            MenuManager.Instance.RemoveMenu();
            OnTeamMemberSelected?.Invoke(index);
        }

        // Update summary when selection changes
        protected override void ChoiceChanged()
        {
            base.ChoiceChanged();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (summaryMenu is MemberSummaryMenu memberSummary)
            {
                ExplorerTeam team = DataManager.Instance.Save.ActiveTeam;
                if (CurrentChoice >= 0 && CurrentChoice < team.Players.Count)
                {
                    memberSummary.SetMember(team.Players[CurrentChoice]);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            summaryMenu?.Draw(spriteBatch);
        }
    }

    // =========================================================================
    // EXAMPLE 4: Custom Summary Panel
    // =========================================================================
    /// <summary>
    /// A summary panel that displays character information.
    /// Not interactive - just displays data.
    /// </summary>
    public class MemberSummaryMenu : SummaryMenu
    {
        private Character displayMember;

        public MemberSummaryMenu(Rect bounds)
            : base(bounds)
        {
        }

        public void SetMember(Character member)
        {
            displayMember = member;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || displayMember == null)
                return;

            // Draw background
            GraphicsManager.MenuBG.Draw(spriteBatch,
                new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height),
                Color.White);

            int textX = Bounds.X + 8;
            int textY = Bounds.Y + 8;

            // Draw name
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX, textY,
                displayMember.GetDisplayName(true),
                null, DirV.Up, DirH.Left, Color.White);

            textY += GraphicsManager.TextFont.CharHeight + 4;

            // Draw level
            string levelText = Text.FormatKey("MENU_TEAM_LEVEL", displayMember.Level);
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX, textY,
                levelText, null, DirV.Up, DirH.Left, Color.Yellow);

            // Draw HP
            textX += 80;
            string hpText = Text.FormatKey("MENU_TEAM_HP",
                displayMember.HP, displayMember.MaxHP);
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX, textY,
                hpText, null, DirV.Up, DirH.Left, Color.Lime);
        }
    }

    // =========================================================================
    // EXAMPLE 5: Dialog Question Menu
    // =========================================================================
    /// <summary>
    /// A yes/no question dialog that appears during gameplay.
    /// Demonstrates integration with the dialogue system.
    /// </summary>
    public class QuestionDialog : InteractableMenu
    {
        private string questionText;
        private Action onYes;
        private Action onNo;
        private bool cursorOnYes;

        public QuestionDialog(string question, Action yesAction, Action noAction)
        {
            questionText = question;
            onYes = yesAction;
            onNo = noAction;
            cursorOnYes = true;

            // Calculate size based on question text
            int textWidth = GraphicsManager.TextFont.SubstringWidth(question);
            int width = Math.Max(textWidth + 32, 120);
            int height = GraphicsManager.TextFont.CharHeight * 3 + 32;

            // Position at bottom center (like dialogue boxes)
            Bounds = new Rect(
                (GraphicsManager.ScreenWidth - width) / 2,
                GraphicsManager.ScreenHeight - height - 16,
                width,
                height
            );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            // Draw background
            GraphicsManager.MenuBG.Draw(spriteBatch,
                new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height),
                Color.White);

            int textX = Bounds.X + 16;
            int textY = Bounds.Y + 8;

            // Draw question
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX, textY,
                questionText, null, DirV.Up, DirH.Left, Color.White);

            textY += GraphicsManager.TextFont.CharHeight + 12;

            // Draw Yes option
            Color yesColor = cursorOnYes ? Color.Yellow : Color.White;
            string yesCursor = cursorOnYes ? "> " : "  ";
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX, textY,
                yesCursor + Text.FormatKey("DLG_CHOICE_YES"),
                null, DirV.Up, DirH.Left, yesColor);

            // Draw No option
            Color noColor = !cursorOnYes ? Color.Yellow : Color.White;
            string noCursor = !cursorOnYes ? "> " : "  ";
            GraphicsManager.TextFont.DrawText(spriteBatch,
                textX + 80, textY,
                noCursor + Text.FormatKey("DLG_CHOICE_NO"),
                null, DirV.Up, DirH.Left, noColor);
        }

        public override void Update(InputManager input)
        {
            // Left/Right to switch between Yes/No
            if (input.JustPressed(FrameInput.InputType.Left) ||
                input.JustPressed(FrameInput.InputType.Right))
            {
                cursorOnYes = !cursorOnYes;
                GameManager.Instance.SE("Menu/Select");
            }

            // Confirm selection
            if (input.JustPressed(FrameInput.InputType.Confirm))
            {
                GameManager.Instance.SE("Menu/Confirm");
                MenuManager.Instance.RemoveMenu();

                if (cursorOnYes)
                    onYes?.Invoke();
                else
                    onNo?.Invoke();
            }

            // Cancel = No
            if (input.JustPressed(FrameInput.InputType.Cancel))
            {
                GameManager.Instance.SE("Menu/Cancel");
                MenuManager.Instance.RemoveMenu();
                onNo?.Invoke();
            }
        }
    }

    // =========================================================================
    // USAGE EXAMPLES
    // =========================================================================

    public static class MenuUsageExamples
    {
        /// <summary>
        /// Example of showing a simple choice menu.
        /// </summary>
        public static void ShowSimpleChoiceExample()
        {
            string[] options = { "Option 1", "Option 2", "Option 3" };

            SimpleChoiceMenu menu = new SimpleChoiceMenu(
                "Choose an option",
                options,
                0,
                (choice) =>
                {
                    if (choice >= 0)
                    {
                        // Handle choice
                        DungeonScene.Instance.LogMsg(
                            string.Format("You chose: {0}", options[choice]));
                    }
                }
            );

            // Add menu to the menu stack
            MenuManager.Instance.AddMenu(menu, false);
        }

        /// <summary>
        /// Example of showing an info display.
        /// </summary>
        public static void ShowInfoExample()
        {
            string[] lines = {
                "Welcome to the dungeon!",
                "",
                "Use arrow keys to move.",
                "Press A to attack.",
                "Press B to open menu."
            };

            InfoDisplayMenu menu = new InfoDisplayMenu("Help", lines);
            MenuManager.Instance.AddMenu(menu, false);
        }

        /// <summary>
        /// Example of showing a yes/no question.
        /// </summary>
        public static void ShowQuestionExample()
        {
            QuestionDialog dialog = new QuestionDialog(
                "Save your progress?",
                () => {
                    // Yes action
                    DungeonScene.Instance.LogMsg("Saving...");
                },
                () => {
                    // No action
                    DungeonScene.Instance.LogMsg("Not saving.");
                }
            );

            MenuManager.Instance.AddMenu(dialog, false);
        }
    }

    // =========================================================================
    // MENU SYSTEM REFERENCE
    // =========================================================================
    //
    // Base Classes:
    // - MenuBase: Basic menu (bounds, visibility)
    // - InteractableMenu: Adds Update() for input
    // - SingleStripMenu: Vertical list of choices
    // - MultiPageMenu: Scrollable pages of choices
    // - SummaryMenu: Non-interactive display panel
    //
    // Common Menu Elements:
    // - MenuTextChoice: Text choice with action
    // - MenuDivider: Visual separator
    // - MenuTextScript: Text that triggers script on hover
    //
    // MenuManager Methods:
    // - AddMenu(menu, pauseGame): Show a menu
    // - RemoveMenu(): Close current menu
    // - ClearMenus(): Close all menus
    //
    // Input Handling:
    // - input.JustPressed(InputType): Single press
    // - input.Pressed(InputType): Held down
    // - input.Direction: Directional input
    //
    // =========================================================================
}
