using RogueElements;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace RogueEssence
{
    /// <summary>
    /// Represents input state for a single frame, capturing keyboard, gamepad, and mouse states.
    /// </summary>
    public class FrameInput
    {
        /// <summary>
        /// Enumeration of all possible input types in the game.
        /// </summary>
        public enum InputType
        {
            /// <summary>Confirm/accept action.</summary>
            Confirm,
            /// <summary>Cancel/back action.</summary>
            Cancel,
            /// <summary>Attack action.</summary>
            Attack,
            /// <summary>Run/dash action.</summary>
            Run,
            /// <summary>Show skills action.</summary>
            Skills,
            /// <summary>Turn in place action.</summary>
            Turn,
            /// <summary>Enable diagonal movement.</summary>
            Diagonal,
            /// <summary>Toggle team mode.</summary>
            TeamMode,
            /// <summary>Show minimap.</summary>
            Minimap,
            /// <summary>Open main menu.</summary>
            Menu,
            /// <summary>Open message log.</summary>
            MsgLog,
            /// <summary>Open skill menu.</summary>
            SkillMenu,
            /// <summary>Open item menu.</summary>
            ItemMenu,
            /// <summary>Open tactic menu.</summary>
            TacticMenu,
            /// <summary>Open team menu.</summary>
            TeamMenu,
            /// <summary>Swap to leader 1.</summary>
            LeaderSwap1,
            /// <summary>Swap to leader 2.</summary>
            LeaderSwap2,
            /// <summary>Swap to leader 3.</summary>
            LeaderSwap3,
            /// <summary>Swap to leader 4.</summary>
            LeaderSwap4,
            /// <summary>Swap to previous leader.</summary>
            LeaderSwapBack,
            /// <summary>Swap to next leader.</summary>
            LeaderSwapForth,
            /// <summary>Use skill 1.</summary>
            Skill1,
            /// <summary>Use skill 2.</summary>
            Skill2,
            /// <summary>Use skill 3.</summary>
            Skill3,
            /// <summary>Use skill 4.</summary>
            Skill4,
            /// <summary>Sort items.</summary>
            SortItems,
            /// <summary>Select items.</summary>
            SelectItems,
            /// <summary>Preview skill.</summary>
            SkillPreview,
            /// <summary>Wait/pass turn.</summary>
            Wait,
            /// <summary>Left mouse button.</summary>
            LeftMouse,
            //meta input here
            /// <summary>Right mouse button.</summary>
            RightMouse,
            /// <summary>Mute/unmute music.</summary>
            MuteMusic,
            /// <summary>Show debug information.</summary>
            ShowDebug,
            /// <summary>Control key modifier.</summary>
            Ctrl,
            /// <summary>Pause game.</summary>
            Pause,
            /// <summary>Advance one frame (debug).</summary>
            AdvanceFrame,
            /// <summary>Take screenshot.</summary>
            Screenshot,
            /// <summary>Decrease game speed.</summary>
            SpeedDown,
            /// <summary>Increase game speed.</summary>
            SpeedUp,
            /// <summary>See all tiles (debug).</summary>
            SeeAll,
            /// <summary>Restart game (debug).</summary>
            Restart,
            /// <summary>Test action (debug).</summary>
            Test,
            /// <summary>Total count of input types.</summary>
            Count
        }

        private bool[] inputStates;

        /// <summary>
        /// Gets the state of the specified input type.
        /// </summary>
        /// <param name="i">The input type to check.</param>
        /// <returns>True if the input is active; otherwise, false.</returns>
        public bool this[InputType i]
        {
            get
            {
                return inputStates[(int)i];
            }
        }

        /// <summary>
        /// Gets the total number of input types.
        /// </summary>
        public int TotalInputs { get { return inputStates.Length; } }

        /// <summary>
        /// Gets the directional input for this frame.
        /// </summary>
        public Dir8 Direction { get; private set; }

        /// <summary>
        /// Gets the raw keyboard state for this frame.
        /// </summary>
        public KeyboardState BaseKeyState { get; private set; }

        /// <summary>
        /// Gets the raw gamepad state for this frame.
        /// </summary>
        public GamePadState BaseGamepadState { get; private set; }

        /// <summary>
        /// Gets the mouse location for this frame.
        /// </summary>
        public Loc MouseLoc { get; private set; }

        /// <summary>
        /// Gets the mouse wheel value for this frame.
        /// </summary>
        public int MouseWheel { get; private set; }

        /// <summary>
        /// Gets whether the game window is active.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets whether a gamepad is connected.
        /// </summary>
        public bool HasGamePad => BaseGamepadState.IsConnected;

        /// <summary>
        /// Initializes a new instance of the FrameInput class with no input.
        /// </summary>
        public FrameInput()
        {
            inputStates = new bool[(int)InputType.Count];
            Direction = Dir8.None;
        }

        /// <summary>
        /// Initializes a new instance of the FrameInput class with the current device states.
        /// </summary>
        /// <param name="gamePad">The current gamepad state.</param>
        /// <param name="keyboard">The current keyboard state.</param>
        /// <param name="mouse">The current mouse state.</param>
        /// <param name="keyActive">Whether keyboard input is active.</param>
        /// <param name="mouseActive">Whether mouse input is active.</param>
        /// <param name="screenActive">Whether the game window is active.</param>
        /// <param name="screenOffset">The offset of the screen for mouse position calculation.</param>
        public FrameInput(GamePadState gamePad, KeyboardState keyboard, MouseState mouse, bool keyActive, bool mouseActive, bool screenActive, Loc screenOffset)
        {
            Active = screenActive;
            BaseGamepadState = gamePad;
            BaseKeyState = keyboard;

            Loc dirLoc = new Loc();
            inputStates = new bool[(int)InputType.Count];
            MouseLoc = new Loc(mouse.X, mouse.Y) - screenOffset;

            if (Active)
                ReadDevInput(keyboard, mouse, keyActive, mouseActive);

            keyActive &= screenActive;
            mouseActive &= screenActive;
            bool controllerActive = gamePad.IsConnected;
            controllerActive &= (Active || DiagManager.Instance.CurSettings.InactiveInput);

            if (controllerActive)
            {
                if (gamePad.ThumbSticks.Left.Length() > 0.25f)
                    dirLoc = DirExt.ApproximateDir8(new Loc((int)(gamePad.ThumbSticks.Left.X * 100), (int)(-gamePad.ThumbSticks.Left.Y * 100))).GetLoc();

                //if (gamePad.ThumbSticks.Right.Length() > 0.25f)
                //    dirLoc = DirExt.ApproximateDir8(new Loc((int)(gamePad.ThumbSticks.Right.X * 100), (int)(-gamePad.ThumbSticks.Right.Y * 100))).GetLoc();


                if (gamePad.IsButtonDown(Buttons.DPadDown))
                    dirLoc = dirLoc + Dir4.Down.GetLoc();
                if (gamePad.IsButtonDown(Buttons.DPadLeft))
                    dirLoc = dirLoc + Dir4.Left.GetLoc();
                if (gamePad.IsButtonDown(Buttons.DPadUp))
                    dirLoc = dirLoc + Dir4.Up.GetLoc();
                if (gamePad.IsButtonDown(Buttons.DPadRight))
                    dirLoc = dirLoc + Dir4.Right.GetLoc();

                //if (DiagManager.Instance.CurSettings.ControllerDisablesKeyboard)
                //    keyActive = false;
            }

            if (keyActive)
            {
                if (dirLoc == Loc.Zero)
                {
                    for (int ii = 0; ii < DiagManager.Instance.CurSettings.DirKeys.Length; ii++)
                    {
                        if (keyboard.IsKeyDown(DiagManager.Instance.CurSettings.DirKeys[ii]))
                            dirLoc = dirLoc + ((Dir4)ii).GetLoc();
                    }
                }

                if (dirLoc == Loc.Zero && DiagManager.Instance.CurSettings.NumPad)
                {
                    if (keyboard.IsKeyDown(Keys.NumPad2))
                        dirLoc = dirLoc + Dir8.Down.GetLoc();
                    if (keyboard.IsKeyDown(Keys.NumPad4))
                        dirLoc = dirLoc + Dir8.Left.GetLoc();
                    if (keyboard.IsKeyDown(Keys.NumPad8))
                        dirLoc = dirLoc + Dir8.Up.GetLoc();
                    if (keyboard.IsKeyDown(Keys.NumPad6))
                        dirLoc = dirLoc + Dir8.Right.GetLoc();

                    if (dirLoc == Loc.Zero)
                    {
                        if (keyboard.IsKeyDown(Keys.NumPad3) || keyboard.IsKeyDown(Keys.NumPad1))
                            dirLoc = dirLoc + Dir8.Down.GetLoc();
                        if (keyboard.IsKeyDown(Keys.NumPad1) || keyboard.IsKeyDown(Keys.NumPad7))
                            dirLoc = dirLoc + Dir8.Left.GetLoc();
                        if (keyboard.IsKeyDown(Keys.NumPad7) || keyboard.IsKeyDown(Keys.NumPad9))
                            dirLoc = dirLoc + Dir8.Up.GetLoc();
                        if (keyboard.IsKeyDown(Keys.NumPad9) || keyboard.IsKeyDown(Keys.NumPad3))
                            dirLoc = dirLoc + Dir8.Right.GetLoc();
                    }
                }
            }

            Direction = dirLoc.GetDir();

            if (controllerActive)
            {
                for (int ii = 0; ii < DiagManager.Instance.CurActionButtons.Length; ii++)
                    inputStates[ii] |= Settings.UsedByGamepad((InputType)ii) && gamePad.IsButtonDown(DiagManager.Instance.CurActionButtons[ii]);
            }

            if (keyActive)
            {
                for (int ii = 0; ii < DiagManager.Instance.CurSettings.ActionKeys.Length; ii++)
                    inputStates[ii] |= Settings.UsedByKeyboard((InputType)ii) && keyboard.IsKeyDown(DiagManager.Instance.CurSettings.ActionKeys[ii]);

                if (DiagManager.Instance.CurSettings.Enter)
                    inputStates[(int)InputType.Confirm] |= keyboard.IsKeyDown(Keys.Enter);

                if (DiagManager.Instance.CurSettings.NumPad)
                    inputStates[(int)InputType.Wait] = keyboard.IsKeyDown(Keys.NumPad5);
            }

            if (mouseActive)
            {
                inputStates[(int)InputType.LeftMouse] = (mouse.LeftButton == ButtonState.Pressed);
                inputStates[(int)InputType.RightMouse] = (mouse.RightButton == ButtonState.Pressed);
            }
        }

        /// <summary>
        /// Reads developer/debug input from keyboard and mouse.
        /// </summary>
        /// <param name="keyboard">The current keyboard state.</param>
        /// <param name="mouse">The current mouse state.</param>
        /// <param name="keyActive">Whether keyboard input is active.</param>
        /// <param name="mouseActive">Whether mouse input is active.</param>
        public void ReadDevInput(KeyboardState keyboard, MouseState mouse, bool keyActive, bool mouseActive)
        {
            if (keyActive)
            {
                inputStates[(int)InputType.ShowDebug] = keyboard.IsKeyDown(Keys.F1);
                inputStates[(int)InputType.Pause] |= keyboard.IsKeyDown(Keys.F2);
                inputStates[(int)InputType.AdvanceFrame] |= keyboard.IsKeyDown(Keys.F3);
                inputStates[(int)InputType.SpeedDown] |= keyboard.IsKeyDown(Keys.F5);
                inputStates[(int)InputType.SpeedUp] |= keyboard.IsKeyDown(Keys.F6);
                inputStates[(int)InputType.MuteMusic] = keyboard.IsKeyDown(Keys.F8);
            }

            if (DiagManager.Instance.DevMode)
            {
                if (mouseActive)
                    MouseWheel = mouse.ScrollWheelValue;

                if (keyActive)
                {
                    inputStates[(int)InputType.Ctrl] |= (keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl));

                    inputStates[(int)InputType.Test] |= keyboard.IsKeyDown(Keys.F4);
                    //inputStates[(int)InputType.] |= keyboard.IsKeyDown(Keys.F7);
                    //inputStates[(int)InputType.] |= keyboard.IsKeyDown(Keys.F8);
                    inputStates[(int)InputType.SeeAll] |= keyboard.IsKeyDown(Keys.F9);
                    inputStates[(int)InputType.Screenshot] |= keyboard.IsKeyDown(Keys.F11);
                    inputStates[(int)InputType.Restart] |= keyboard.IsKeyDown(Keys.F12);
                }
            }
        }


        /// <summary>
        /// Determines whether the specified object is equal to this FrameInput.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is FrameInput) && Equals((FrameInput)obj);
        }

        /// <summary>
        /// Determines whether the specified FrameInput is equal to this instance.
        /// </summary>
        /// <param name="other">The FrameInput to compare.</param>
        /// <returns>True if the inputs are equal; otherwise, false.</returns>
        public bool Equals(FrameInput other)
        {
            if (Direction != other.Direction) return false;

            for (int ii = 0; ii < (int)InputType.Count; ii++)
            {
                if (inputStates[ii] != other.inputStates[ii]) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code based on direction and input states.</returns>
        public override int GetHashCode()
        {
            return Direction.GetHashCode() ^ inputStates.GetHashCode();
        }

        /// <summary>
        /// Determines whether two FrameInput instances are equal.
        /// </summary>
        /// <param name="input1">The first FrameInput.</param>
        /// <param name="input2">The second FrameInput.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public static bool operator ==(FrameInput input1, FrameInput input2)
        {
            return input1.Equals(input2);
        }

        /// <summary>
        /// Determines whether two FrameInput instances are not equal.
        /// </summary>
        /// <param name="input1">The first FrameInput.</param>
        /// <param name="input2">The second FrameInput.</param>
        /// <returns>True if not equal; otherwise, false.</returns>
        public static bool operator !=(FrameInput input1, FrameInput input2)
        {
            return !(input1 == input2);
        }

        /// <summary>
        /// Loads a FrameInput from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new FrameInput instance with the loaded data.</returns>
        public static FrameInput Load(BinaryReader reader)
        {
            FrameInput input = new FrameInput();

            input.Direction = (Dir8)((int)reader.ReadByte());
            for (int ii = 0; ii < (int)FrameInput.InputType.Ctrl; ii++)
                input.inputStates[ii] = reader.ReadBoolean();
            //for (int ii = 0; ii < FrameInput.TOTAL_CHARS; ii++)
            //    input.CharInput[ii] = reader.ReadBoolean();
            return input;
        }
    }
}
