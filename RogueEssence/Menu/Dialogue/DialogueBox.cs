using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using System.Text.RegularExpressions;
using NLua;
using RogueEssence.Script;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for dialogue boxes that display scrolling text with optional speaker portraits.
    /// Supports text tags for pauses, speed changes, scripts, emotes, and sound effects.
    /// </summary>
    public abstract class DialogueBox : MenuBase, IInteractable
    {
        /// <summary>
        /// Regex pattern for splitting text at [scroll] tags.
        /// </summary>
        public static Regex SplitTags = new Regex(@"\[scroll\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected const int HOLD_CANCEL_TIME = 30;
        private const int SCROLL_SPEED = 2;
        protected const int CURSOR_FLASH_TIME = 24;

        /// <summary>
        /// Global text speed multiplier for dialogue. Higher values display text faster.
        /// </summary>
        public static double TextSpeed;

        /// <summary>
        /// Number of frames between speech sound effects.
        /// </summary>
        public const int SPEAK_FRAMES = 2;

        /// <summary>
        /// Default sound effect to play for speech.
        /// </summary>
        public const string SOUND_EFFECT = "Menu/Speak";

        /// <summary>
        /// Horizontal buffer from screen edge in pixels.
        /// </summary>
        public const int SIDE_BUFFER = 8;

        /// <summary>
        /// Height of each text line in pixels.
        /// </summary>
        public const int TEXT_HEIGHT = 16;

        /// <summary>
        /// Vertical padding inside the dialogue box.
        /// </summary>
        public const int VERT_PAD = 2;

        /// <summary>
        /// Vertical offset adjustment for text positioning.
        /// </summary>
        public const int VERT_OFFSET = -2;

        /// <summary>
        /// Horizontal padding inside the dialogue box.
        /// </summary>
        public const int HORIZ_PAD = 4;

        /// <summary>
        /// Maximum number of text lines visible in the dialogue box.
        /// </summary>
        public const int MAX_LINES = 2;

        /// <summary>
        /// Gets the default bounds for a dialogue box positioned at the bottom of the screen.
        /// </summary>
        public static Rect DefaultBounds => Rect.FromPoints(
            new Loc(SIDE_BUFFER, GraphicsManager.ScreenHeight - (16 + TEXT_HEIGHT * MAX_LINES + VERT_PAD * 2)),
            new Loc(GraphicsManager.ScreenWidth - SIDE_BUFFER, GraphicsManager.ScreenHeight - 8)
        );

        /// <summary>
        /// Whether the dialogue can be skipped by holding cancel.
        /// </summary>
        public bool Skippable;

        /// <summary>
        /// Collection of text tags organized by text segment index.
        /// </summary>
        public List<List<TextTag>> Tags;

        /// <summary>
        /// Gets the tags for the current text segment.
        /// </summary>
        protected List<TextTag> CurrentTag { get { return Tags[curTextIndex]; } }

        /// <summary>
        /// Script callbacks that can be invoked from dialogue text tags.
        /// </summary>
        public object[] Scripts;
        
        //Dialogue Text needs to be able to set character index accurately
        protected List<DialogueText> Texts;
        private int curTextIndex;
        private bool scrolling;
        private bool centerH;
        private bool centerV;
        private int totalLines;
        private int nextTextIndex;

        /// <summary>
        /// Gets the currently displayed text segment.
        /// </summary>
        protected DialogueText CurrentText { get { return Texts[curTextIndex]; } }

        /// <summary>
        /// Gets the next text segment to display during scrolling, or null.
        /// </summary>
        protected DialogueText NextText { get { return nextTextIndex > -1 ? Texts[nextTextIndex] : null; } }

        /// <summary>
        /// Gets whether the current text box has finished displaying all text and tags.
        /// </summary>
        protected bool CurrentBoxFinished { get { return CurrentText.Finished && CurrentTag.Count == 0; } }

        /// <summary>
        /// Gets whether all dialogue text has been displayed.
        /// </summary>
        public bool Finished { get { return CurrentText.Finished && curTextIndex == Texts.Count-1; } }

        /// <summary>
        /// Whether to play sound effects during text display.
        /// </summary>
        public bool Sound;

        protected FrameTick TotalTextTime;
        protected FrameTick CurrentTextTime;
        protected FrameTick LastSpeakTime;
        protected FrameTick CurrentScrollTime;

        /// <summary>
        /// Called when all text has been displayed. Override to handle completion behavior.
        /// </summary>
        /// <param name="input">The input manager for handling user input.</param>
        public abstract void ProcessTextDone(InputManager input);

        //optional speaker box
        private SpeakerPortrait speakerPic;

        //the speakername, alone
        private string speakerName;

        //message with pauses, without speaker name
        private string message;

        private double currSpeed;

        private string currSe;
        private int currSpeakTime;
        
        private bool runningScript;
        private bool startedScript;

        /// <inheritdoc/>
        public bool IsCheckpoint { get { return false; } }

        /// <inheritdoc/>
        public bool Inactive { get; set; }

        /// <inheritdoc/>
        public bool BlockPrevious { get; set; }

        /// <summary>
        /// Creates an array of script callbacks from a Lua table.
        /// </summary>
        /// <param name="callbacks">The Lua table containing callbacks.</param>
        /// <returns>An array of script objects.</returns>
        public static object[] CreateScripts(LuaTable callbacks)
        {
            object[] scripts = new object[] {};
            if (callbacks != null)
            {
                int scriptIdx = 0;
                scripts = new object[callbacks.Values.Count];
                foreach (object val in callbacks.Values)
                {
                    scripts[scriptIdx] = val;
                    scriptIdx++;
                }
            }

            return scripts;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueBox"/> class.
        /// </summary>
        /// <param name="msg">The message to display.</param>
        /// <param name="sound">Whether to play sound effects.</param>
        /// <param name="soundEffect">The sound effect to play for speech.</param>
        /// <param name="speakTime">Frames between speech sounds.</param>
        /// <param name="centerH">Whether to center text horizontally.</param>
        /// <param name="centerV">Whether to center text vertically.</param>
        /// <param name="bounds">The bounds of the dialogue box.</param>
        /// <param name="scripts">Script callbacks for text tags.</param>
        public DialogueBox(string msg, bool sound, string soundEffect, int speakTime, bool centerH, bool centerV, Rect bounds, object[] scripts)
        {
            Bounds = bounds;
            Scripts = scripts;
            Tags = new List<List<TextTag>>();
            speakerName = "";
            currSe = soundEffect;
            currSpeakTime = speakTime;
            Sound = sound;
            message = msg;
            totalLines = Bounds.Height / TEXT_HEIGHT;
            
            Texts = new List<DialogueText>();
            this.centerH = centerH;
            this.centerV = centerV;
            updateMessage(0);
        }

        /// <inheritdoc/>
        public virtual void ProcessActions(FrameTick elapsedTime)
        {
            TotalTextTime += elapsedTime;
            if (!CurrentBoxFinished)
            {
                CurrentTextTime += elapsedTime;
                LastSpeakTime += elapsedTime;
            }
            if (scrolling)
                CurrentScrollTime += elapsedTime;
        }

        /// <inheritdoc/>
        public void Update(InputManager input)
        {
            if (!CurrentBoxFinished)
            {
                TextTag curTag = getCurrentTextTag();
                while (curTag != null)
                {
                    TextScript textScript = curTag as TextScript;
                    if (textScript != null)
                    {
                        if (!startedScript && textScript.Script < Scripts.Length && textScript.Script >= 0)
                        {
                            object script = Scripts[textScript.Script];
                            if (script is Coroutine)
                            {
                                MenuManager.Instance.NextAction = waitForTaskDone(CoroutineManager.Instance.StartCoroutine((Coroutine)script, true));
                                runningScript = true;
                                startedScript = true;
                            }
                            else if (script is LuaFunction)
                            {
                                LuaFunction luaFun = script as LuaFunction;
                                MenuManager.Instance.NextAction = waitForTaskDone(CoroutineManager.Instance.StartCoroutine(new Coroutine(LuaEngine.Instance.CallScriptFunction(luaFun)), true));
                                runningScript = true;
                                startedScript = true;
                            }
                        }

                        if (runningScript)
                            return;

                        startedScript = false;
                        runningScript = false;
                        CurrentTextTime = new FrameTick();
                    }

                    TextSpeed textSpeed = curTag as TextSpeed;
                    if (textSpeed != null)
                    {
                        currSpeed = textSpeed.Speed;
                    }

                    TextSoundEffect textSoundEffect = curTag as TextSoundEffect;
                    if (textSoundEffect != null)
                    {
                        currSe = textSoundEffect.Sound;
                        currSpeakTime = textSoundEffect.SpeakTime;
                    }

                    TextEmote textEmote = curTag as TextEmote;
                    if (textEmote != null)
                    {
                        SetPortraitEmote(textEmote.Emote);
                    }

                    TextPause textPause = curTag as TextPause;
                    if (textPause != null)
                    {
                        bool continueText;
                        if (textPause.Time > 0)
                            continueText = CurrentTextTime >= textPause.Time;
                        else
                            continueText = (input.JustPressed(FrameInput.InputType.Confirm) || input[FrameInput.InputType.Cancel]
                                || input.JustPressed(FrameInput.InputType.LeftMouse));

                        if (!continueText)
                            return;

                        CurrentTextTime = new FrameTick();
                    }

                    CurrentTag.RemoveAt(0);
                    curTag = getCurrentTextTag();
                }

                bool addedText = false;
                
                double speed = currSpeed > 0 ? currSpeed : DialogueBox.TextSpeed;
                FrameTick subTick = speed > 0 ? new FrameTick((long)(FrameTick.FrameToTick(1) / speed)) : FrameTick.FromFrames(1);
                while (true)
                {
                    TextTag blockingTag = getCurrentTextTag();
                    if (CurrentText.Finished || blockingTag != null && blockingTag.IsBlocking())
                    {
                        CurrentTextTime = new FrameTick();
                        break;
                    }

                    if (CurrentTextTime < subTick)
                        break;
                    CurrentTextTime -= subTick;
                    CurrentText.CurrentCharIndex++;
                    addedText = true;
                }


                if (addedText && Sound && LastSpeakTime > currSpeakTime)
                {
                    LastSpeakTime = new FrameTick();
                    GameManager.Instance.SE(currSe);
                }
            }
            else if (curTextIndex < Texts.Count - 1)
            {
                if (input.JustPressed(FrameInput.InputType.Confirm) || input[FrameInput.InputType.Cancel] && TotalTextTime >= HOLD_CANCEL_TIME
                                                                    || input.JustPressed(FrameInput.InputType.LeftMouse))
                {
                    scrolling = true;
                    nextTextIndex = curTextIndex + 1;
                    NextText.Rect.Start = CurrentText.Rect.Start + new Loc(0, TEXT_HEIGHT * totalLines);
                }

                int scrollFrames = TEXT_HEIGHT * totalLines / SCROLL_SPEED;
                if (CurrentScrollTime < FrameTick.FromFrames(scrollFrames))
                {
                    if (scrolling)
                    {
                        CurrentText.Rect.Start -= new Loc(0, SCROLL_SPEED);
                        NextText.Rect.Start -= new Loc(0, SCROLL_SPEED);
                    }
                }
                else
                {
                    nextTextIndex = -1;
                    scrolling = false;
                    curTextIndex++;
                    CurrentScrollTime = new FrameTick();
                }
            }
            else
                ProcessTextDone(input);

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            base.Draw(spriteBatch);
            
            //when text is paused and waiting for input, flash a tick at the end
            TextPause textPause = getCurrentTextTag() as TextPause;
            if (textPause != null && textPause.Time == 0)
            {
                //text needs a "GetTextProgress" method, which returns the end loc of the string as its currently shown.
                //the coordinates are relative to the string's position
                Loc loc = Bounds.Start + CurrentText.GetTextProgress() + CurrentText.Rect.Start;
                
                if ((GraphicsManager.TotalFrameTick / (ulong)FrameTick.FrameToTick(CURSOR_FLASH_TIME / 2)) % 2 == 0)
                    GraphicsManager.Cursor.DrawTile(spriteBatch, new Vector2(loc.X + 2, loc.Y), 0, 0);
            }

            if (speakerPic != null)
                speakerPic.Draw(spriteBatch, new Loc());

            //draw down-tick
            if (CurrentBoxFinished && !Finished && !scrolling)
            {
                if ((GraphicsManager.TotalFrameTick / (ulong)FrameTick.FrameToTick(CURSOR_FLASH_TIME / 2)) % 2 == 0)
                    GraphicsManager.Cursor.DrawTile(spriteBatch, new Vector2(Bounds.Center.X - 5, Bounds.End.Y - 6), 1, 0);
            }
        }

        protected override IEnumerable<IMenuElement> GetDrawElements()
        {
            yield return CurrentText;
            if (nextTextIndex > -1)
                yield return NextText;
        }

        protected TextTag getCurrentTextTag()
        {
            if (CurrentTag.Count > 0)
            {
                if (CurrentText.CurrentCharIndex < 0 || CurrentTag[0].LetterIndex <= CurrentText.CurrentCharIndex)
                    return CurrentTag[0];
            }
            return null;
        }

        /// <summary>
        /// Sets the speaker portrait for the dialogue box.
        /// </summary>
        /// <param name="speaker">The monster ID of the speaker.</param>
        /// <param name="emotion">The emotion style for the portrait.</param>
        /// <param name="speakerLoc">The location to display the portrait.</param>
        public void SetPortrait(MonsterID speaker, EmoteStyle emotion, Loc speakerLoc)
        {
            if (speaker.IsValid())
            {
                speakerPic = new SpeakerPortrait(speaker, emotion, speakerLoc, true);
            }
            else
                speakerPic = null;
        }
        
        /// <summary>
        /// Updates the speaker portrait's emotion.
        /// </summary>
        /// <param name="emote">The new emote index to display.</param>
        public void SetPortraitEmote(int emote)
        {
            if (speakerPic != null)
            {
                speakerPic = new SpeakerPortrait(speakerPic, emote);
            }
        }

        /// <summary>
        /// Sets the speaker with portrait and name.
        /// </summary>
        /// <param name="speaker">The monster ID of the speaker.</param>
        /// <param name="name">The display name for the speaker.</param>
        /// <param name="emotion">The emotion style for the portrait.</param>
        /// <param name="speakerLoc">The location to display the portrait.</param>
        public void SetSpeaker(MonsterID speaker, string name, EmoteStyle emotion, Loc speakerLoc)
        {
            SetPortrait(speaker, emotion, speakerLoc);

            if (!String.IsNullOrEmpty(name))
                speakerName = name;
            else
                speakerName = "";
            updateMessage(0);
        }

        /// <summary>
        /// Updates the dialogue message.
        /// </summary>
        /// <param name="msg">The new message to display.</param>
        /// <param name="sound">Whether to play sound effects.</param>
        public void SetMessage(string msg, bool sound)
        {
            message = msg;
            Sound = sound;
            updateMessage(0);
        }

        private IEnumerator<YieldInstruction> waitForTaskDone(Coroutine coroutine)
        {
            while (true)
            {
                if (coroutine.FinishedYield())
                {
                    runningScript = false;
                    yield break;
                }
                yield return new WaitForFrames(1);
            }
        }
        /// <summary>
        /// Immediately finishes displaying all text, skipping to the end.
        /// </summary>
        public void FinishText()
        {
            foreach(DialogueText text in Texts)
                text.FinishText();
            foreach (List<TextTag> tagList in Tags)
                tagList.Clear();
        }

        /// <summary>
        /// Sets the current character index for text display progress.
        /// </summary>
        /// <param name="curCharIndex">The character index to set.</param>
        public void SetTextProgress(int curCharIndex)
        {
            updateMessage(curCharIndex);
        }

        private void updateTagWithRange(List<TextTag> tags, List<IntRange> tagRanges, TextTag tag, Match match, int curCharIndex)
        {
            if (match.Index + match.Length <= curCharIndex)
                return;

            tags.Add(tag);
            tagRanges.Add(new IntRange(match.Index, match.Index + match.Length));
        }

        private void updateMessage(int curCharIndex)
        {
            //message will contain pauses, which get parsed here.
            //and colors, which will get parsed by the text renderer
            Texts.Clear();
            curTextIndex = 0;
            nextTextIndex = -1;
            Tags.Clear();

            string msg = message;
            if (speakerName != "")
            {
                msg = String.Format("{0}: {1}", speakerName, msg);
                curCharIndex += speakerName.Length + 2;
            }
            int startLag = 0;

            string[] scrolls = SplitTags.Split(msg);
            for (int nn = 0; nn < scrolls.Length; nn++)
            {
                List<TextTag> tags = new List<TextTag>();
                List<IntRange> tagRanges = new List<IntRange>();
                
                int lag = 0;
                MatchCollection matches = Text.MsgTags.Matches(scrolls[nn]);
                
                foreach (Match match in matches)
                {
                    foreach (string key in match.Groups.Keys)
                    {
                        if (!match.Groups[key].Success)
                            continue;
                        switch (key)
                        {
                            case "pause":
                                {
                                    TextPause pause = new TextPause();
                                    pause.LetterIndex = match.Index - lag;
                                    int param;
                                    if (Int32.TryParse(match.Groups["pauseval"].Value, out param))
                                        pause.Time = param;
                                    updateTagWithRange(tags, tagRanges, pause, match, curCharIndex);
                                }
                                break;
                            case "script":
                                {
                                    TextScript script = new TextScript();
                                    script.LetterIndex = match.Index - lag;
                                    int param;
                                    if (Int32.TryParse(match.Groups["scriptval"].Value, out param))
                                        script.Script = param;
                                    updateTagWithRange(tags, tagRanges, script, match, curCharIndex);
                                }
                                break;
                            case "speed":
                                {
                                    TextSpeed speed = new TextSpeed();
                                    speed.LetterIndex = match.Index - lag;
                                    double param;
                                    if (Double.TryParse(match.Groups["speedval"].Value, out param))
                                        speed.Speed = param;
                                    updateTagWithRange(tags, tagRanges, speed, match, curCharIndex);
                                }
                                break;
                            case "sound":
                                {
                                    TextSoundEffect sound = new TextSoundEffect();
                                    sound.LetterIndex = match.Index - lag;
                                    sound.Sound = match.Groups["soundval"].Value;
                                    sound.SpeakTime = currSpeakTime;
                                    int param;
                                    if (Int32.TryParse(match.Groups["speaktime"].Value, out param))
                                        sound.SpeakTime = param;
                                    updateTagWithRange(tags, tagRanges, sound, match, curCharIndex);
                                }
                                break;
                            case "emote":
                                {
                                    TextEmote emote = new TextEmote();
                                    emote.LetterIndex = match.Index - lag;
                                    emote.Emote = GraphicsManager.Emotions.FindIndex((EmotionType element) => element.Name.ToLower() == match.Groups["emoteval"].Value.ToLower());
                                    updateTagWithRange(tags, tagRanges, emote, match, curCharIndex);
                                }
                                break;
                            case "colorstart":
                            case "colorend":
                                break;
                        }
                    }

                    lag += match.Length;

                    if (nn == 0 && match.Index + match.Length <= curCharIndex)
                        startLag += match.Length;
                }

                //remove the tags, leaving pure text (except for the color tags)
                for (int ii = tagRanges.Count - 1; ii >= 0; ii--)
                    scrolls[nn] = scrolls[nn].Remove(tagRanges[ii].Min, tagRanges[ii].Length);

                //split the text, being mindful of color tags
                List<DialogueText> texts = DialogueText.SplitFormattedText(scrolls[nn], new Rect(GraphicsManager.MenuBG.TileWidth + HORIZ_PAD, GraphicsManager.MenuBG.TileHeight + VERT_PAD + VERT_OFFSET,
                    Bounds.Width - GraphicsManager.MenuBG.TileWidth * 2 - HORIZ_PAD * 2, Bounds.Height - GraphicsManager.MenuBG.TileHeight * 2 - VERT_PAD * 2 - VERT_OFFSET * 2), TEXT_HEIGHT, centerH, centerV, 0);

                int totalTrim = 0;
                int totalLength = 0;
                int curTag = 0;
                for (int kk = 0; kk < texts.Count; kk++)
                {
                    DialogueText text = texts[kk];

                    //tags need to be re-aligned to the removals done by breaking the text into lines
                    List<TextTag> subTags = new List<TextTag>();
                    int lineCount = text.GetLineCount();
                    for (int ii = 0; ii < lineCount; ii++)
                    {
                        totalTrim += text.GetLineTrim(ii);
                        int oldLength = totalLength;
                        totalLength += text.GetLineTrim(ii) + text.GetLineLength(ii);
                        for (; curTag < tags.Count; curTag++)
                        {
                            TextTag tag = tags[curTag];
                            if (tag.LetterIndex <= totalLength)
                            {
                                tag.LetterIndex -= totalTrim;
                                subTags.Add(tag);
                            }
                            else
                                break;
                        }
                    }
                    totalTrim = totalLength;

                    Tags.Add(subTags);
                    Texts.Add(text);
                }

            }
            CurrentText.CurrentCharIndex = curCharIndex - startLag;
        }
    }

    /// <summary>
    /// Abstract base class for tags that modify dialogue text behavior at specific character positions.
    /// </summary>
    public abstract class TextTag
    {
        /// <summary>
        /// The character index at which this tag takes effect.
        /// </summary>
        public int LetterIndex;

        /// <summary>
        /// Determines if this tag blocks text advancement until processed.
        /// </summary>
        /// <returns>True if blocking; otherwise, false.</returns>
        public virtual bool IsBlocking()
        {
            return false;
        }
    }

    /// <summary>
    /// A text tag that pauses dialogue display for a specified time or until user input.
    /// </summary>
    public class TextPause : TextTag
    {
        /// <summary>
        /// The time in frames to pause. Use 0 or negative to wait for button press.
        /// </summary>
        public int Time;

        /// <inheritdoc/>
        public override bool IsBlocking()
        {
            return true;
        }
    }

    /// <summary>
    /// A text tag that changes the text display speed.
    /// </summary>
    public class TextSpeed : TextTag
    {
        /// <summary>
        /// The new text speed multiplier.
        /// </summary>
        public double Speed;
    }

    /// <summary>
    /// A text tag that changes the speaker's emote/expression.
    /// </summary>
    public class TextEmote : TextTag
    {
        /// <summary>
        /// The emote index to display.
        /// </summary>
        public int Emote;
    }

    /// <summary>
    /// A text tag that changes the speech sound effect.
    /// </summary>
    public class TextSoundEffect : TextTag
    {
        /// <summary>
        /// The sound effect to play.
        /// </summary>
        public string Sound;

        /// <summary>
        /// Frames between speech sounds.
        /// </summary>
        public int SpeakTime;
    }

    /// <summary>
    /// A text tag that executes a script callback.
    /// </summary>
    public class TextScript : TextTag
    {
        /// <summary>
        /// The index of the script in the Scripts array to execute.
        /// </summary>
        public int Script;

        /// <inheritdoc/>
        public override bool IsBlocking()
        {
            return true;
        }
    }

}
