using System;
using RogueElements;
using Microsoft.Xna.Framework.Input;

namespace RogueEssence
{
    /// <summary>
    /// Manages input state tracking, providing methods to detect button presses, releases, and held states.
    /// Maintains both current and previous frame input for edge detection.
    /// </summary>
    public class InputManager
    {
        private FrameInput PrevInput;
        private FrameInput CurrentInput;

        /// <summary>
        /// Gets the number of consecutive frames the current input state has been held.
        /// </summary>
        public long InputTime { get; private set; }

        /// <summary>
        /// Gets the time added since the last input change.
        /// </summary>
        public long AddedInputTime { get; private set; }

        /// <summary>
        /// Gets the difference in mouse wheel position between frames.
        /// Handles wrap-around at integer boundaries.
        /// </summary>
        public int MouseWheelDiff
        {
            get
            {
                if (!CurrentInput.Active || !PrevInput.Active)
                    return 0;
                int diff = CurrentInput.MouseWheel - PrevInput.MouseWheel;
                if (diff > Int32.MaxValue / 2)
                    diff = (PrevInput.MouseWheel - Int32.MinValue) + (Int32.MaxValue - CurrentInput.MouseWheel);
                else if (diff < Int32.MinValue / 2)
                    diff = (CurrentInput.MouseWheel - Int32.MinValue) + (Int32.MaxValue - PrevInput.MouseWheel);
                return diff;
            }
        }

        /// <summary>
        /// Gets the state of the specified input type for the current frame.
        /// </summary>
        /// <param name="i">The input type to check.</param>
        /// <returns>True if the input is active; otherwise, false.</returns>
        public bool this[FrameInput.InputType i] { get { return CurrentInput[i]; } }

        /// <summary>
        /// Gets the directional input from the previous frame.
        /// </summary>
        public Dir8 PrevDirection { get { return PrevInput.Direction; } }

        /// <summary>
        /// Gets the directional input from the current frame.
        /// </summary>
        public Dir8 Direction { get { return CurrentInput.Direction; } }

        /// <summary>
        /// Gets the mouse location from the previous frame.
        /// </summary>
        public Loc PrevMouseLoc { get { return PrevInput.MouseLoc; } }

        /// <summary>
        /// Gets the mouse location from the current frame.
        /// </summary>
        public Loc MouseLoc { get { return CurrentInput.MouseLoc; } }

        /// <summary>
        /// Initializes a new instance of the InputManager class.
        /// </summary>
        public InputManager()
        {
            PrevInput = new FrameInput();
            CurrentInput = new FrameInput();
        }

        /// <summary>
        /// Sets the current frame's input state, updating timing information.
        /// </summary>
        /// <param name="input">The new input state.</param>
        public void SetFrameInput(FrameInput input)
        {
            if (input == CurrentInput)
            {
                AddedInputTime = 1;
                InputTime++;
            }
            else
            {
                AddedInputTime = 1;
                InputTime = 1;
            }

            PrevInput = CurrentInput;
            CurrentInput = input;

        }

        public void RepeatFrameInput()
        {
            SetFrameInput(CurrentInput);
        }

        public bool OnlyPressed(FrameInput.InputType input)
        {
            //nonmeta input only
            for (int ii = 0; ii < (int)FrameInput.InputType.RightMouse; ii++)
            {
                if (ii != (int)input && CurrentInput[(FrameInput.InputType)ii])
                    return false;
            }
            return true;
        }

        public bool JustPressed(FrameInput.InputType input)
        {
            return !PrevInput[input] && CurrentInput[input];
        }

        public bool JustReleased(FrameInput.InputType input)
        {
            return PrevInput[input] && !CurrentInput[input];
        }

        public bool BaseKeyDown(Keys key)
        {
            return CurrentInput.BaseKeyState.IsKeyDown(key);
        }

        public bool BaseKeyPressed(Keys key)
        {
            return (CurrentInput.BaseKeyState.IsKeyDown(key) && !PrevInput.BaseKeyState.IsKeyDown(key));
        }

        public bool AnyKeyPressed()
        {
            if (PrevInput.BaseKeyState.GetPressedKeys().Length == 0)
            {
                foreach (Keys key in CurrentInput.BaseKeyState.GetPressedKeys())
                {
                    if (key < Keys.F1 || key > Keys.F24)
                        return true;
                }
            }
            return false;
        }

        public bool BaseButtonDown(Buttons button)
        {
            return CurrentInput.BaseGamepadState.IsButtonDown(button);
        }

        public bool BaseButtonPressed(Buttons button)
        {
            return (CurrentInput.BaseGamepadState.IsButtonDown(button) && !PrevInput.BaseGamepadState.IsButtonDown(button));
        }

        public bool AnyButtonPressed()
        {
            GamePadButtons untouchedButtons = new GamePadButtons();
            return (CurrentInput.BaseGamepadState.Buttons != untouchedButtons && PrevInput.BaseGamepadState.Buttons == untouchedButtons);
        }
    }
}
