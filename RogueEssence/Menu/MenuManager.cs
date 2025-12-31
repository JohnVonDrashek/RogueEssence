using System;
using System.Collections.Generic;
using RogueEssence.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Dungeon;
using System.Text.RegularExpressions;
using RogueElements;
using RogueEssence.Script;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Singleton manager class that handles the menu stack, dialogue creation, and menu processing.
    /// Provides centralized control over all menu operations including adding, removing, and updating menus.
    /// </summary>
    public class MenuManager
    {
        /// <summary>
        /// Coroutine to execute after the current menu update. Used for chaining menu actions.
        /// </summary>
        public IEnumerator<YieldInstruction> NextAction;

        /// <summary>
        /// Coroutine to execute after all menus are closed.
        /// </summary>
        public IEnumerator<YieldInstruction> EndAction;

        private int menuModeDepth;

        private static MenuManager instance;

        /// <summary>
        /// Initializes the singleton instance of the MenuManager.
        /// </summary>
        public static void InitInstance()
        {
            instance = new MenuManager();
            MenuBase.Transparent = false;
        }

        /// <summary>
        /// Gets the singleton instance of the MenuManager.
        /// </summary>
        public static MenuManager Instance { get { return instance; } }

        private List<IInteractable> menus;

        /// <summary>
        /// Gets the number of menus currently on the stack.
        /// </summary>
        public int MenuCount { get { return menus.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuManager"/> class.
        /// </summary>
        public MenuManager()
        {
            menus = new List<IInteractable>();
        }

        /// <summary>
        /// Adds a menu to the top of the menu stack.
        /// </summary>
        /// <param name="menu">The menu to add.</param>
        /// <param name="stackOn">If true, menus below will still be visible; if false, they will be hidden.</param>
        /// <exception cref="Exception">Thrown if not in menu mode.</exception>
        public void AddMenu(IInteractable menu, bool stackOn)
        {
            if (menuModeDepth == 0)
                throw new Exception("Can't add menu while not in menu mode");

            LuaEngine.Instance.OnAddMenu(menu);
            if (menus.Count > 0)
                menus[menus.Count - 1].Inactive = true;
            menu.Inactive = false;
            menu.BlockPrevious = !stackOn;
            menus.Add(menu);
        }

        /// <summary>
        /// Replaces the top menu on the stack with a new menu.
        /// </summary>
        /// <param name="menu">The menu to replace the current top menu.</param>
        /// <exception cref="Exception">Thrown if not in menu mode.</exception>
        public void ReplaceMenu(IInteractable menu)
        {
            if (menuModeDepth == 0)
                throw new Exception("Can't replace menu while not in menu mode");

            LuaEngine.Instance.OnAddMenu(menu);
            menu.BlockPrevious = menus[menus.Count - 1].BlockPrevious;
            menus.RemoveAt(menus.Count - 1);
            menus.Add(menu);
        }

        /// <summary>
        /// Removes the top menu from the stack.
        /// </summary>
        /// <exception cref="Exception">Thrown if not in menu mode.</exception>
        public void RemoveMenu()
        {
            if (menuModeDepth == 0)
                throw new Exception("Can't remove menu while not in menu mode");

            menus[menus.Count - 1].Inactive = true;
            menus.RemoveAt(menus.Count - 1);
            if (menus.Count > 0)
                menus[menus.Count - 1].Inactive = false;
        }

        /// <summary>
        /// Clears all menus from the stack.
        /// </summary>
        public void ClearMenus()
        {
            menus.Clear();
        }

        /// <summary>
        /// Clears menus from the stack until reaching a checkpoint menu.
        /// </summary>
        public void ClearToCheckpoint()
        {
            menus.RemoveAt(menus.Count - 1);
            while (menus.Count > 0 && menus[menus.Count-1].IsCheckpoint)
                menus.RemoveAt(menus.Count - 1);
        }

        /// <summary>
        /// Processes a menu as a coroutine, blocking until the menu is closed.
        /// </summary>
        /// <param name="menu">The menu to process.</param>
        /// <returns>A coroutine that processes the menu.</returns>
        public IEnumerator<YieldInstruction> ProcessMenuCoroutine(IInteractable menu)
        {
            LuaEngine.Instance.OnAddMenu(menu);
            if (menus.Count > 0)
                menus[menus.Count - 1].Inactive = true;
            menu.Inactive = false;
            menu.BlockPrevious = true;
            menus.Add(menu);
            yield return CoroutineManager.Instance.StartCoroutine(ProcessMenuCoroutine());
        }

        /// <summary>
        /// Processes the current menu stack as a coroutine until all menus are closed.
        /// </summary>
        /// <returns>A coroutine that processes the menu stack.</returns>
        public IEnumerator<YieldInstruction> ProcessMenuCoroutine()
        {
            yield return CoroutineManager.Instance.StartCoroutine(processInternalCoroutine());
        }

        /// <summary>
        /// Saves the current menu state for later restoration.
        /// </summary>
        /// <returns>A list containing the current menu stack.</returns>
        public List<IInteractable> SaveMenuState()
        {
            List<IInteractable> save = new List<IInteractable>();
            save.AddRange(menus);
            return save;
        }

        /// <summary>
        /// Loads a previously saved menu state. The current menu stack must be empty.
        /// </summary>
        /// <param name="save">The saved menu state to load.</param>
        /// <exception cref="Exception">Thrown if the current menu stack is not empty.</exception>
        public void LoadMenuState(List<IInteractable> save)
        {
            if (menus.Count > 0)
                throw new Exception("Must load menus from empty.");
            menus.AddRange(save);
        }

        /// <summary>
        /// Draws all visible menus in the stack.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public void DrawMenus(SpriteBatch spriteBatch)
        {
            int startIndex = menus.Count;
            for (int ii = menus.Count - 1; ii >= 0; ii--)
            {
                startIndex = ii;
                if (menus[ii].BlockPrevious)
                    break;
            }
            for (int ii = startIndex; ii < menus.Count; ii++)
            {
                if (menus[ii].Visible)
                    menus[ii].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Gets the menu and relative location at the specified screen coordinates.
        /// </summary>
        /// <param name="screenLoc">The screen coordinates to check.</param>
        /// <param name="menu">The menu at the specified location, or null if none.</param>
        /// <param name="relativeLoc">The location relative to the menu's origin, or null.</param>
        public void GetMenuCoord(Loc screenLoc, out MenuBase menu, out Loc? relativeLoc)
        {
            menu = null;
            relativeLoc = null;

            for (int ii = menus.Count - 1; ii >= 0; ii--)
            {
                if (menus[ii].Visible)
                {
                    InteractableMenu interactable = menus[ii] as InteractableMenu;
                    if (interactable != null && interactable.GetRelativeMouseLoc(screenLoc, out menu, out relativeLoc))
                        return;

                }
                if (menus[ii].BlockPrevious)
                    break;
            }
        }

        /// <summary>
        /// Creates and displays a sign-style dialogue (auto-finishes text).
        /// </summary>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetSign(params string[] msgs)
        {
            return SetDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, () => { }, -1, true, false, false, msgs);
        }

        /// <summary>
        /// Creates and displays a simple dialogue box.
        /// </summary>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetDialogue(params string[] msgs)
        {
            return SetDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, msgs);
        }

        /// <summary>
        /// Creates and displays a dialogue with a finish action callback.
        /// </summary>
        /// <param name="finishAction">Action to execute when dialogue completes.</param>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetDialogue(Action finishAction, params string[] msgs)
        {
            return SetDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, finishAction, -1, false, false, false, msgs);
        }

        /// <summary>
        /// Creates and displays a dialogue with optional sound.
        /// </summary>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetDialogue(bool sound, params string[] msgs)
        {
            return SetDialogue(MonsterID.Invalid, null, new EmoteStyle(0), sound, msgs);
        }

        /// <summary>
        /// Creates and displays a dialogue with a speaker portrait and emotion.
        /// </summary>
        /// <param name="speaker">The monster ID of the speaker.</param>
        /// <param name="speakerName">The name to display for the speaker.</param>
        /// <param name="emotion">The emotion/emote style for the speaker portrait.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, bool sound, params string[] msgs)
        {
            return SetDialogue(speaker, speakerName, emotion, sound, () => { }, -1, false, false, false, msgs);
        }

        /// <summary>
        /// Creates and displays a fully customized dialogue.
        /// </summary>
        /// <param name="speaker">The monster ID of the speaker.</param>
        /// <param name="speakerName">The name to display for the speaker.</param>
        /// <param name="emotion">The emotion/emote style for the speaker portrait.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="finishAction">Action to execute when dialogue completes.</param>
        /// <param name="waitTime">Time in frames before auto-advance (-1 for click to advance).</param>
        /// <param name="autoFinish">Whether to auto-finish displaying text.</param>
        /// <param name="centerH">Whether to center text horizontally.</param>
        /// <param name="centerV">Whether to center text vertically.</param>
        /// <param name="msgs">The messages to display.</param>
        /// <returns>A coroutine that processes the dialogue.</returns>
        public IEnumerator<YieldInstruction> SetDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, bool sound, Action finishAction, int waitTime, bool autoFinish, bool centerH, bool centerV, params string[] msgs)
        {
            DialogueBox box = CreateDialogue(speaker, speakerName, emotion, sound, finishAction, waitTime, autoFinish, centerH, centerV, msgs);
            yield return CoroutineManager.Instance.StartCoroutine(ProcessMenuCoroutine(box));
        }
        
        public IEnumerator<YieldInstruction> SetDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, Loc speakerLoc, bool sound, string soundEffect, int speakTime, Action finishAction, int waitTime, bool autoFinish, bool centerH, bool centerV, Rect bounds, object[] scripts, params string[] msgs)
        {
            DialogueBox box = CreateDialogue(speaker, speakerName, emotion, speakerLoc, sound, soundEffect, speakTime, finishAction, waitTime, autoFinish, centerH, centerV, bounds, scripts, msgs);
            yield return CoroutineManager.Instance.StartCoroutine(ProcessMenuCoroutine(box));
        }

        public IEnumerator<YieldInstruction> SetTitleDialog(int holdTime, bool fadeIn, Rect bounds, object[] scripts, Action finishAction, params string[] msgs)
        {
            TitleDialog box = CreateTitleDialog(holdTime, fadeIn, bounds, scripts, finishAction, msgs);
            yield return CoroutineManager.Instance.StartCoroutine(ProcessMenuCoroutine(box));
        }

        public IEnumerator<YieldInstruction> SetWaitMenu(params FrameInput.InputType[] inputs)
        {
            WaitMenu box = new WaitMenu(inputs);
            yield return CoroutineManager.Instance.StartCoroutine(ProcessMenuCoroutine(box));
        }

        public DialogueBox CreateDialogue(params string[] msgs)
        {
            return CreateDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, msgs);
        }
        public DialogueBox CreateDialogue(Action finishAction, params string[] msgs)
        {
            return CreateDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, finishAction, -1, false, false, false, msgs);
        }
        public DialogueBox CreateDialogue(bool sound, params string[] msgs)
        {
            return CreateDialogue(MonsterID.Invalid, null, new EmoteStyle(0), sound, msgs);
        }
        public DialogueBox CreateDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, bool sound, params string[] msgs)
        {
            return CreateDialogue(speaker, speakerName, emotion, sound, () => { }, -1, false, false, false, msgs);
        }

        public DialogueBox CreateDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, bool sound, Action finishAction, int waitTime, bool autoFinish, bool centerH, bool centerV, params string[] msgs)
        {
            return CreateDialogue(speaker, speakerName, emotion, SpeakerPortrait.DefaultLoc, sound, DialogueBox.SOUND_EFFECT, DialogueBox.SPEAK_FRAMES, finishAction, waitTime, autoFinish, centerH,
                centerV, DialogueBox.DefaultBounds, new object[] { }, msgs);
        }

        public DialogueBox CreateDialogue(MonsterID speaker, string speakerName, EmoteStyle emotion, Loc speakerLoc, bool sound, string soundEffect, int speakTime, Action finishAction, int waitTime, bool autoFinish, bool centerH, bool centerV, Rect bounds, object[] scripts, params string[] msgs)
        {
            if (msgs.Length > 0)
            {
                List<string> sep_msgs = new List<string>();
                for (int ii = 0; ii < msgs.Length; ii++)
                {
                    string[] break_str = Regex.Split(msgs[ii], @"\[br\]", RegexOptions.IgnoreCase);
                    sep_msgs.AddRange(break_str);
                }
                DialogueBox box = null;
                for (int ii = sep_msgs.Count - 1; ii >= 0; ii--)
                {
                    DialogueBox prevBox = box;
                    box = CreateBox(speaker, speakerName, emotion, speakerLoc, sound, soundEffect, speakTime, (prevBox == null) ? finishAction : () => { AddMenu(prevBox, false); }, waitTime, autoFinish, centerH, centerV, bounds, scripts, sep_msgs[ii]);
                }
                return box;
            }
            return null;
        }
        
        public DialogueBox CreateBox(MonsterID speaker, string speakerName, EmoteStyle emotion, Loc speakerLoc, bool sound, string soundEffect, int speakTime,
             Action finishAction, int waitTime, bool autoFinish, bool centerH, bool centerV, Rect bounds, object[] scripts, string msg)
        {
            DialogueBox box = null;
            if (waitTime > -1)
                box = new TimedDialog(msg, sound, soundEffect, speakTime, centerH, centerV, bounds, scripts, waitTime, finishAction);
            else
                box = new ClickedDialog(msg, sound, soundEffect, speakTime, centerH, centerV, bounds, scripts, finishAction);
            box.SetSpeaker(speaker, speakerName, emotion, speakerLoc);
            if (autoFinish)
                box.FinishText();
            return box;
        }

        public TitleDialog CreateTitleDialog(int holdTime, bool fadeIn, Rect bounds, object[] scripts, Action finishAction, params string[] msgs)
        {
            if (msgs.Length > 0)
            {
                List<string> sep_msgs = new List<string>();
                for (int ii = 0; ii < msgs.Length; ii++)
                {
                    string[] break_str = Regex.Split(msgs[ii], @"\[br\]", RegexOptions.IgnoreCase);
                    sep_msgs.AddRange(break_str);
                }
                TitleDialog box = null;
                for (int ii = sep_msgs.Count - 1; ii >= 0; ii--)
                {
                    TitleDialog prevBox = box;
                    box = new TitleDialog(sep_msgs[ii], fadeIn, holdTime, bounds, scripts, (prevBox == null) ? finishAction : () => { AddMenu(prevBox, false); });
                }
                return box;
            }
            return null;
        }

        public InfoMenu CreateNotice(string title, string msg)
        {
            return CreateNotice(title, () => { }, msg);
        }

        public InfoMenu CreateNotice(string title, params string[] msgs)
        {
            return CreateNotice(title, () => { }, msgs);
        }

        public InfoMenu CreateNotice(string title, Action finishAction, params string[] msgs)
        {
            if (msgs.Length > 0)
            {
                List<string> sep_msgs = new List<string>();
                for (int ii = 0; ii < msgs.Length; ii++)
                {
                    string[] break_str = Regex.Split(msgs[ii], @"\[br\]", RegexOptions.IgnoreCase);
                    sep_msgs.AddRange(break_str);
                }
                InfoMenu box = null;
                for (int ii = sep_msgs.Count - 1; ii >= 0; ii--)
                {
                    InfoMenu prevBox = box;
                    box = new InfoMenu(title, sep_msgs[ii], (prevBox == null) ? finishAction : () => { AddMenu(prevBox, false); });
                }
                return box;
            }
            return null;
        }

        /// <summary>
        /// Creates a yes/no question dialogue.
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="yes">Action to execute if yes is selected.</param>
        /// <param name="no">Action to execute if no is selected.</param>
        /// <returns>The created dialogue box.</returns>
        public DialogueBox CreateQuestion(string message, Action yes, Action no)
        {
            return CreateQuestion(MonsterID.Invalid, null, new EmoteStyle(0), message, true, false, false, false, yes, no, false);
        }

        /// <summary>
        /// Creates a yes/no question dialogue with sound control.
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="yes">Action to execute if yes is selected.</param>
        /// <param name="no">Action to execute if no is selected.</param>
        /// <returns>The created dialogue box.</returns>
        public DialogueBox CreateQuestion(string message, bool sound, Action yes, Action no)
        {
            return CreateQuestion(MonsterID.Invalid, null, new EmoteStyle(0), message, sound, false, false, false, yes, no, false);
        }

        /// <summary>
        /// Creates a yes/no question dialogue with default selection control.
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="yes">Action to execute if yes is selected.</param>
        /// <param name="no">Action to execute if no is selected.</param>
        /// <param name="defaultNo">Whether No should be selected by default.</param>
        /// <returns>The created dialogue box.</returns>
        public DialogueBox CreateQuestion(string message, bool sound, Action yes, Action no, bool defaultNo)
        {
            return CreateQuestion(MonsterID.Invalid, null, new EmoteStyle(0), message, sound, false, false, false, yes, no, defaultNo);
        }
        
        public DialogueBox CreateQuestion(MonsterID speaker, string speakerName, EmoteStyle emotion, Loc speakerLoc,
            string msg, bool sound, string soundEffect, int speakTime, bool autoFinish, bool centerH, bool centerV, Rect bounds, object[] scripts, Loc menuLoc, Action yes, Action no, bool defaultNo)
        {
            string[] break_str = Regex.Split(msg, "\\[br\\]", RegexOptions.IgnoreCase);

            DialogueChoice[] choices = new DialogueChoice[2];
            choices[0] = new DialogueChoice(Text.FormatKey("DLG_CHOICE_YES"), yes);
            choices[1] = new DialogueChoice(Text.FormatKey("DLG_CHOICE_NO"), no);

            DialogueBox box = new QuestionDialog(break_str[break_str.Length - 1], sound, soundEffect, speakTime, centerH, centerV, bounds, scripts, choices, defaultNo ? 1 : 0, 1, menuLoc);
            box.SetSpeaker(speaker, speakerName, emotion, speakerLoc);
            if (autoFinish)
                box.FinishText();

            if (break_str.Length > 1)
            {
                string[] pre_str = new string[break_str.Length - 1];
                Array.Copy(break_str, pre_str, pre_str.Length);
                return CreateDialogue(speaker, speakerName, emotion, sound, () => { AddMenu(box, false); }, -1, autoFinish, centerH, centerV, pre_str);
            }
            else
                return box;
        }

        public DialogueBox CreateQuestion(MonsterID speaker, string speakerName, EmoteStyle emotion,
            string msg, bool sound, bool autoFinish, bool centerH, bool centerV, Action yes, Action no, bool defaultNo)
        {
            return CreateQuestion(speaker, speakerName, emotion, SpeakerPortrait.DefaultLoc, msg, sound, DialogueBox.SOUND_EFFECT, DialogueBox.SPEAK_FRAMES, autoFinish, centerH, centerV, DialogueBox.DefaultBounds, new object[] {}, DialogueChoiceMenu.DefaultLoc, yes, no, defaultNo);
        }

        /// <summary>
        /// Creates a multiple choice question dialogue from a list of choices.
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="choices">List of dialogue choices.</param>
        /// <param name="defaultChoice">The index of the default selection.</param>
        /// <param name="cancelChoice">The index to select when canceling.</param>
        /// <returns>The created dialogue box.</returns>
        public DialogueBox CreateMultiQuestion(string message, bool sound, List<DialogueChoice> choices, int defaultChoice, int cancelChoice)
        {
            return CreateMultiQuestion(MonsterID.Invalid, null, new EmoteStyle(0), message, sound, false, false, false, choices.ToArray(), defaultChoice, cancelChoice);
        }

        /// <summary>
        /// Creates a multiple choice question dialogue from an array of choices.
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="sound">Whether to play text sound effects.</param>
        /// <param name="choices">Array of dialogue choices.</param>
        /// <param name="defaultChoice">The index of the default selection.</param>
        /// <param name="cancelChoice">The index to select when canceling.</param>
        /// <returns>The created dialogue box.</returns>
        public DialogueBox CreateMultiQuestion(string message, bool sound, DialogueChoice[] choices, int defaultChoice, int cancelChoice)
        {
            return CreateMultiQuestion(MonsterID.Invalid, null, new EmoteStyle(0), message, sound, false, false, false, choices, defaultChoice, cancelChoice);
        }

        public DialogueBox CreateMultiQuestion(MonsterID speaker, string speakerName, EmoteStyle emotion, Loc speakerLoc,
            string msg, bool sound, string soundEffect, int speakTime, bool autoFinish, bool centerH, bool centerV, Rect bounds, object[] scripts, Loc menuLoc, DialogueChoice[] choices, int defaultChoice, int cancelChoice)
        {
            string[] break_str = Regex.Split(msg, "\\[br\\]", RegexOptions.IgnoreCase);

            // TODO fix MultiQuestion
            DialogueBox box = new QuestionDialog(break_str[break_str.Length - 1], sound, soundEffect, speakTime, centerH, centerV, bounds, scripts, choices, defaultChoice, cancelChoice, menuLoc);
            box.SetSpeaker(speaker, speakerName, emotion, speakerLoc);
            if (autoFinish)
                box.FinishText();

            if (break_str.Length > 1)
            {
                string[] pre_str = new string[break_str.Length - 1];
                Array.Copy(break_str, pre_str, pre_str.Length);
                return CreateDialogue(speaker, speakerName, emotion, sound, () => { AddMenu(box, false); }, -1, autoFinish, centerH, centerV, pre_str);
            }
            else
                return box;
        }

        public DialogueBox CreateMultiQuestion(MonsterID speaker, string speakerName, EmoteStyle emotion,
            string msg, bool sound, bool autoFinish, bool centerH, bool centerV, DialogueChoice[] choices,
            int defaultChoice, int cancelChoice)
        {
            return CreateMultiQuestion(speaker, speakerName, emotion, SpeakerPortrait.DefaultLoc, msg, sound, DialogueBox.SOUND_EFFECT, DialogueBox.SPEAK_FRAMES, autoFinish, centerH, centerV, DialogueBox.DefaultBounds, new object[] {}, DialogueChoiceMenu.DefaultLoc,  choices, defaultChoice, cancelChoice);
            
        }
        

        private IEnumerator<YieldInstruction> processInternalCoroutine()
        {
            menuModeDepth++;
            while (MenuCount > 0)
            {
                yield return new WaitForFrames(1);
                ProcessMenus(GameManager.Instance.InputManager);

                if (NextAction != null)
                {
                    IEnumerator<YieldInstruction> action = NextAction;
                    NextAction = null;
                    yield return CoroutineManager.Instance.StartCoroutine(action);
                }
            }
            menuModeDepth--;

            if (EndAction != null)
            {
                IEnumerator<YieldInstruction> action = EndAction;
                EndAction = null;
                yield return CoroutineManager.Instance.StartCoroutine(action);
            }
            GameManager.Instance.FrameProcessed = false;
        }


        private void ProcessMenus(InputManager input)
        {
            try
            {
                //process most recent menu
                if (menus.Count > 0)
                    menus[menus.Count - 1].Update(input);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Processes time-based actions for the topmost menu.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last frame.</param>
        public void ProcessActions(FrameTick elapsedTime)
        {
            //process most recent menu
            if (menus.Count > 0)
                menus[menus.Count - 1].ProcessActions(elapsedTime);
        }

    }
}
