using System;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;
using System.Collections.Generic;

namespace RogueEssence.Menu
{
    /// <summary>
    /// A dialogue box that presents a question with multiple choice answers.
    /// Displays text and then shows a choice menu when the text is finished.
    /// </summary>
    public class QuestionDialog : DialogueBox
    {
        private DialogueChoiceMenu dialogueChoices;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionDialog"/> class with full customization.
        /// </summary>
        /// <param name="message">The question message to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="soundEffect">The sound effect to use.</param>
        /// <param name="speakTime">Frames between speech sounds.</param>
        /// <param name="centerH">Whether to center text horizontally.</param>
        /// <param name="centerV">Whether to center text vertically.</param>
        /// <param name="bounds">The bounds of the dialogue box.</param>
        /// <param name="scripts">Script callbacks for text tags.</param>
        /// <param name="choices">The available answer choices.</param>
        /// <param name="defaultChoice">Index of the default selected choice.</param>
        /// <param name="cancelChoice">Index of the choice to select on cancel.</param>
        /// <param name="menuLoc">Location of the choice menu.</param>
        public QuestionDialog(string message, bool sound, string soundEffect, int speakTime, bool centerH, bool centerV, Rect bounds, object[] scripts, DialogueChoice[] choices, int defaultChoice, int cancelChoice, Loc menuLoc)
            : base(message, sound, soundEffect, speakTime, centerH, centerV, bounds, scripts)
        {
            dialogueChoices = new DialogueChoiceMenu(choices, defaultChoice, cancelChoice, menuLoc);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionDialog"/> class with default settings.
        /// </summary>
        /// <param name="message">The question message to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="centerH">Whether to center text horizontally.</param>
        /// <param name="centerV">Whether to center text vertically.</param>
        /// <param name="choices">The available answer choices.</param>
        /// <param name="defaultChoice">Index of the default selected choice.</param>
        /// <param name="cancelChoice">Index of the choice to select on cancel.</param>
        public QuestionDialog(string message, bool sound, bool centerH, bool centerV, DialogueChoice[] choices, int defaultChoice, int cancelChoice) : this(message, sound, DialogueBox.SOUND_EFFECT, DialogueBox.SPEAK_FRAMES, centerH, centerV, DialogueBox.DefaultBounds, new object[] {}, choices, defaultChoice, cancelChoice, new Loc(-1, -1)) {}

        /// <inheritdoc/>
        public override void ProcessTextDone(InputManager input)
        {
            //choice menu needs a special setting for always making the cursor flash
            //make the singlestripmenu store callbacks for when a choice gets selected?
            //then, its initialize method must be public
            //or, just make a special menu window for dialogue questions.  DialogueChoiceMenu?
            dialogueChoices.Update(input);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            base.Draw(spriteBatch);

            if (Finished)
                dialogueChoices.Draw(spriteBatch);

        }
    }

    /// <summary>
    /// A vertical choice menu displayed alongside question dialogues.
    /// Provides the answer selection interface for <see cref="QuestionDialog"/>.
    /// </summary>
    public class DialogueChoiceMenu : VertChoiceMenu
    {
        private Action[] results;
        private int cancelChoice;

        /// <summary>
        /// Spacing constant for question menus.
        /// </summary>
        public const int QUESTION_SPACE = 8;

        /// <summary>
        /// Gets the default location for the choice menu (-1 indicates auto-position).
        /// </summary>
        public static Loc DefaultLoc => new Loc(-1);

        /// <inheritdoc/>
        public override bool CanMenu { get { return false; } }

        /// <inheritdoc/>
        public override bool CanCancel { get { return cancelChoice > -1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueChoiceMenu"/> class.
        /// </summary>
        /// <param name="choices">The available choices.</param>
        /// <param name="defaultChoice">Index of the default selected choice.</param>
        /// <param name="cancelChoice">Index of the choice to select on cancel, or -1 to disable canceling.</param>
        /// <param name="menuLoc">The location of the menu, or (-1,-1) for auto-position.</param>
        public DialogueChoiceMenu(DialogueChoice[] choices, int defaultChoice, int cancelChoice, Loc menuLoc)
        {
            MenuTextChoice[] menu_choices = new MenuTextChoice[choices.Length];
            results = new Action[choices.Length];
            for (int ii = 0; ii < choices.Length; ii++)
            {
                int index = ii;
                menu_choices[ii] = new MenuTextChoice(choices[ii].Choice, () => { choose(index); }, choices[ii].Enabled, choices[ii].Enabled ? Color.White : Color.Red);
                results[ii] = choices[ii].Result;
            }
            
            int choice_width = CalculateChoiceLength(menu_choices, 0);
            
            int x = menuLoc.X != -1 ? menuLoc.X : GraphicsManager.ScreenWidth - DialogueBox.SIDE_BUFFER - choice_width;
            int y = menuLoc.Y != -1 ? menuLoc.Y : 188 - (choices.Length * VERT_SPACE + GraphicsManager.MenuBG.TileHeight * 2);
            Loc loc = new Loc(x, y);
            Initialize(loc, choice_width, menu_choices, defaultChoice);

            this.cancelChoice = cancelChoice;
        }
        
        private void choose(int choice)
        {
            MenuManager.Instance.RemoveMenu();

            if (results[choice] != null)
                results[choice]();
        }
        protected override void MenuPressed() { }
        protected override void ChoseMultiIndex(List<int> slots) { }

        protected override void Canceled()
        {
            choose(cancelChoice);
        }
    }


    /// <summary>
    /// Represents a single choice option in a dialogue question.
    /// </summary>
    public class DialogueChoice
    {
        /// <summary>
        /// The display text for this choice.
        /// </summary>
        public string Choice;

        /// <summary>
        /// The action to execute when this choice is selected.
        /// </summary>
        public Action Result;

        /// <summary>
        /// Whether this choice is enabled and selectable.
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Initializes a new enabled <see cref="DialogueChoice"/>.
        /// </summary>
        /// <param name="choice">The display text for the choice.</param>
        /// <param name="result">The action to execute when selected.</param>
        public DialogueChoice(string choice, Action result)
        : this(choice, result, true) { }

        /// <summary>
        /// Initializes a new <see cref="DialogueChoice"/> with enabled state control.
        /// </summary>
        /// <param name="choice">The display text for the choice.</param>
        /// <param name="result">The action to execute when selected.</param>
        /// <param name="enable">Whether this choice is enabled.</param>
        public DialogueChoice(string choice, Action result, bool enable)
        {
            Choice = choice;
            Result = result;
            Enabled = enable;
        }
    }
}
