using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using RogueEssence.Menu;
using Microsoft.Xna.Framework;

namespace RogueEssence
{
    /// <summary>
    /// Represents a gamepad button mapping configuration for a specific controller type.
    /// Maps game actions to physical gamepad buttons.
    /// </summary>
    [Serializable]
    public class GamePadMap
    {
        /// <summary>
        /// The display name of this gamepad configuration.
        /// </summary>
        public string Name;

        /// <summary>
        /// Array mapping InputType indices to physical gamepad buttons.
        /// </summary>
        public Buttons[] ActionButtons;

        /// <summary>
        /// Initializes a new instance of the GamePadMap class with default values.
        /// </summary>
        public GamePadMap()
        {
            Name = "";
            ActionButtons = new Buttons[(int)FrameInput.InputType.Wait];
        }

        /// <summary>
        /// Initializes a new instance of the GamePadMap class by copying from another instance.
        /// </summary>
        /// <param name="other">The GamePadMap to copy from.</param>
        public GamePadMap(GamePadMap other)
        {
            Name = other.Name;
            ActionButtons = new Buttons[(int)FrameInput.InputType.Wait];
            Array.Copy(other.ActionButtons, ActionButtons, ActionButtons.Length);
        }
    }

    /// <summary>
    /// Contains all user-configurable game settings including audio, controls, display, and network options.
    /// This class is serialized to persist settings between sessions.
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Defines the speed at which battle animations play.
        /// </summary>
        public enum BattleSpeed
        {
            VerySlow,
            Slow,
            Normal,
            Fast,
            VeryFast
        }
        /// <summary>
        /// Defines which skills are enabled by default when a character joins the team.
        /// </summary>
        public enum SkillDefault
        {
            None,
            Attacks,
            All
        }

        /// <summary>
        /// Defines the color scheme for the minimap display.
        /// </summary>
        public enum MinimapStyle
        {
            White,
            Black,
            Blue
        }

        /// <summary>
        /// Background music volume level (0-10).
        /// </summary>
        public int BGMBalance;

        /// <summary>
        /// Sound effects volume level (0-10).
        /// </summary>
        public int SEBalance;

        /// <summary>
        /// The speed at which battle animations play.
        /// </summary>
        public BattleSpeed BattleFlow;

        /// <summary>
        /// Which skills are enabled by default for new team members.
        /// </summary>
        public SkillDefault DefaultSkills;

        /// <summary>
        /// Minimap transparency/visibility level (0-100).
        /// </summary>
        public int Minimap;

        /// <summary>
        /// The color scheme used for the minimap.
        /// </summary>
        public MinimapStyle MinimapColor;

        private double textSpeed;

        /// <summary>
        /// Gets or sets the text display speed multiplier. Updates DialogueBox.TextSpeed when set.
        /// </summary>
        public double TextSpeed
        {
            get { return textSpeed; }
            set
            {
                textSpeed = value;
                DialogueBox.TextSpeed = textSpeed;
            }
        }

        private int border;

        /// <summary>
        /// Gets or sets the menu border style index. Updates MenuBase.BorderStyle when set.
        /// </summary>
        public int Border
        {
            get { return border; }
            set
            {
                border = value;
                MenuBase.BorderStyle = border;
            }
        }

        /// <summary>
        /// Window size/scale mode index.
        /// </summary>
        public int Window;

        /// <summary>
        /// The language code for localization (e.g., "en", "jp").
        /// </summary>
        public string Language;

        /// <summary>
        /// Keyboard keys mapped to directional input (Down, Left, Up, Right).
        /// </summary>
        public Keys[] DirKeys;

        /// <summary>
        /// Keyboard keys mapped to action inputs.
        /// </summary>
        public Keys[] ActionKeys;

        /// <summary>
        /// Dictionary of gamepad configurations keyed by controller GUID.
        /// </summary>
        public Dictionary<string, GamePadMap> GamepadMaps;

        /// <summary>
        /// Whether Enter key can be used for confirmation.
        /// </summary>
        public bool Enter;

        /// <summary>
        /// Whether numpad can be used for directional input.
        /// </summary>
        public bool NumPad;

        /// <summary>
        /// Whether to accept input when the game window is not focused.
        /// </summary>
        public bool InactiveInput;

        /// <summary>
        /// List of saved server connections for online play.
        /// </summary>
        public List<ServerInfo> ServerList;

        /// <summary>
        /// List of saved contacts for online play.
        /// </summary>
        public List<ContactInfo> ContactList;

        /// <summary>
        /// List of saved peer connections for direct P2P play.
        /// </summary>
        public List<PeerInfo> PeerList;

        /// <summary>
        /// Set of input types that may conflict in menu contexts.
        /// </summary>
        public static HashSet<FrameInput.InputType> MenuConflicts;

        /// <summary>
        /// Set of input types that may conflict in dungeon contexts.
        /// </summary>
        public static HashSet<FrameInput.InputType> DungeonConflicts;

        /// <summary>
        /// Set of input types that may conflict for action inputs.
        /// </summary>
        public static HashSet<FrameInput.InputType> ActionConflicts;

        /// <summary>
        /// Set of keyboard keys that cannot be bound to actions.
        /// </summary>
        public static HashSet<Keys> ForbiddenKeys;

        /// <summary>
        /// Set of gamepad buttons that cannot be bound to actions.
        /// </summary>
        public static HashSet<Buttons> ForbiddenButtons;

        /// <summary>
        /// List of color pairs for minimap tiles (explored, unexplored).
        /// </summary>
        public static List<(Color explored, Color unexplored)> MinimapColors;

        /// <summary>
        /// Initializes static conflict sets, forbidden inputs, and minimap colors.
        /// Must be called before using Settings.
        /// </summary>
        public static void InitStatic()
        {
            MenuConflicts = new HashSet<FrameInput.InputType>();
            MenuConflicts.Add(FrameInput.InputType.Confirm);
            MenuConflicts.Add(FrameInput.InputType.Cancel);
            MenuConflicts.Add(FrameInput.InputType.Menu);
            MenuConflicts.Add(FrameInput.InputType.MsgLog);
            MenuConflicts.Add(FrameInput.InputType.SkillMenu);
            MenuConflicts.Add(FrameInput.InputType.ItemMenu);
            MenuConflicts.Add(FrameInput.InputType.TacticMenu);
            MenuConflicts.Add(FrameInput.InputType.TeamMenu);
            MenuConflicts.Add(FrameInput.InputType.Minimap);
            MenuConflicts.Add(FrameInput.InputType.SortItems);
            MenuConflicts.Add(FrameInput.InputType.SelectItems);

            DungeonConflicts = new HashSet<FrameInput.InputType>();
            DungeonConflicts.Add(FrameInput.InputType.Attack);
            DungeonConflicts.Add(FrameInput.InputType.Run);
            DungeonConflicts.Add(FrameInput.InputType.Skills);
            DungeonConflicts.Add(FrameInput.InputType.Turn);
            DungeonConflicts.Add(FrameInput.InputType.Diagonal);
            DungeonConflicts.Add(FrameInput.InputType.TeamMode);
            DungeonConflicts.Add(FrameInput.InputType.Menu);
            DungeonConflicts.Add(FrameInput.InputType.Minimap);

            DungeonConflicts.Add(FrameInput.InputType.MsgLog);
            DungeonConflicts.Add(FrameInput.InputType.SkillMenu);
            DungeonConflicts.Add(FrameInput.InputType.ItemMenu);
            DungeonConflicts.Add(FrameInput.InputType.TacticMenu);
            DungeonConflicts.Add(FrameInput.InputType.TeamMenu);
            DungeonConflicts.Add(FrameInput.InputType.Minimap);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwap1);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwap2);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwap3);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwap4);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwapBack);
            DungeonConflicts.Add(FrameInput.InputType.LeaderSwapForth);

            ActionConflicts = new HashSet<FrameInput.InputType>();
            ActionConflicts.Add(FrameInput.InputType.Skills);
            ActionConflicts.Add(FrameInput.InputType.Skill1);
            ActionConflicts.Add(FrameInput.InputType.Skill2);
            ActionConflicts.Add(FrameInput.InputType.Skill3);
            ActionConflicts.Add(FrameInput.InputType.Skill4);
            ActionConflicts.Add(FrameInput.InputType.SkillPreview);

            ForbiddenKeys = new HashSet<Keys>();
            ForbiddenKeys.Add(Keys.None);
            ForbiddenKeys.Add(Keys.CapsLock);
            ForbiddenKeys.Add(Keys.PageUp);
            ForbiddenKeys.Add(Keys.PageDown);
            ForbiddenKeys.Add(Keys.End);
            ForbiddenKeys.Add(Keys.Home);
            ForbiddenKeys.Add(Keys.Select);
            ForbiddenKeys.Add(Keys.Print);
            ForbiddenKeys.Add(Keys.Execute);
            ForbiddenKeys.Add(Keys.PrintScreen);
            ForbiddenKeys.Add(Keys.Insert);
            ForbiddenKeys.Add(Keys.Delete);
            ForbiddenKeys.Add(Keys.Help);
            ForbiddenKeys.Add(Keys.LeftWindows);
            ForbiddenKeys.Add(Keys.RightWindows);
            ForbiddenKeys.Add(Keys.Sleep);

            for (int ii = 0; ii < 24; ii++)
                ForbiddenKeys.Add(Keys.F1 + ii);

            ForbiddenKeys.Add(Keys.NumLock);
            ForbiddenKeys.Add(Keys.Scroll);
            ForbiddenKeys.Add(Keys.LeftControl);
            ForbiddenKeys.Add(Keys.RightControl);
            ForbiddenKeys.Add(Keys.LeftAlt);
            ForbiddenKeys.Add(Keys.RightAlt);
            for (int ii = 0; ii < 20; ii++)
                ForbiddenKeys.Add(Keys.BrowserBack + ii);
            ForbiddenKeys.Add(Keys.Oem8);
            ForbiddenKeys.Add(Keys.ProcessKey);
            ForbiddenKeys.Add(Keys.Attn);
            ForbiddenKeys.Add(Keys.Crsel);
            ForbiddenKeys.Add(Keys.Exsel);
            ForbiddenKeys.Add(Keys.EraseEof);
            ForbiddenKeys.Add(Keys.Play);
            ForbiddenKeys.Add(Keys.Zoom);
            ForbiddenKeys.Add(Keys.Pa1);
            ForbiddenKeys.Add(Keys.OemClear);
            ForbiddenKeys.Add(Keys.ChatPadGreen);
            ForbiddenKeys.Add(Keys.ChatPadOrange);
            ForbiddenKeys.Add(Keys.Pause);
            ForbiddenKeys.Add(Keys.ImeConvert);
            ForbiddenKeys.Add(Keys.ImeNoConvert);
            ForbiddenKeys.Add(Keys.Kana);
            ForbiddenKeys.Add(Keys.Kanji);
            ForbiddenKeys.Add(Keys.OemAuto);
            ForbiddenKeys.Add(Keys.OemCopy);
            ForbiddenKeys.Add(Keys.OemEnlW);

            ForbiddenButtons = new HashSet<Buttons>();

            ForbiddenButtons.Add(Buttons.DPadDown);
            ForbiddenButtons.Add(Buttons.DPadLeft);
            ForbiddenButtons.Add(Buttons.DPadUp);
            ForbiddenButtons.Add(Buttons.DPadRight);

            ForbiddenButtons.Add(Buttons.LeftThumbstickDown);
            ForbiddenButtons.Add(Buttons.LeftThumbstickLeft);
            ForbiddenButtons.Add(Buttons.LeftThumbstickUp);
            ForbiddenButtons.Add(Buttons.LeftThumbstickRight);
            ForbiddenButtons.Add(Buttons.RightThumbstickDown);
            ForbiddenButtons.Add(Buttons.RightThumbstickLeft);
            ForbiddenButtons.Add(Buttons.RightThumbstickUp);
            ForbiddenButtons.Add(Buttons.RightThumbstickRight);

            MinimapColors = new List<(Color, Color)>();
            MinimapColors.Add((Color.White, Color.DarkGray));
            MinimapColors.Add((Color.DimGray, Color.DarkGray));
            MinimapColors.Add((Color.Blue, Color.DarkGray));
        }

        /// <summary>
        /// Initializes a new instance of the Settings class with default values.
        /// </summary>
        public Settings()
        {

            BGMBalance = 5;
            SEBalance = 5;
            BattleFlow = BattleSpeed.Normal;
            TextSpeed = 1.0;
            DefaultSkills = SkillDefault.Attacks;
            Language = "";

            Minimap = 100;
            Window = 2;

            DirKeys = new Keys[4];
            ActionKeys = new Keys[(int)FrameInput.InputType.Wait];
            GamepadMaps = new Dictionary<string, GamePadMap>();
            ServerList = new List<ServerInfo>();
            ContactList = new List<ContactInfo>();
            PeerList = new List<PeerInfo>();

            GamePadMap defaultMap = new GamePadMap();
            defaultMap.Name = "Unknown";
            DefaultControls(DirKeys, ActionKeys, defaultMap.ActionButtons);
            GamepadMaps["default"] = defaultMap;
            Enter = true;
            NumPad = true;
            InactiveInput = false;
        }

        /// <summary>
        /// Sets up default control bindings for keyboard and gamepad.
        /// </summary>
        /// <param name="dirKeys">Array to populate with default directional key bindings, or null to skip.</param>
        /// <param name="actionKeys">Array to populate with default action key bindings, or null to skip.</param>
        /// <param name="actionButtons">Array to populate with default gamepad button bindings, or null to skip.</param>
        public static void DefaultControls(Keys[] dirKeys, Keys[] actionKeys, Buttons[] actionButtons)
        {
            if (dirKeys != null)
            {
                dirKeys[0] = Keys.Down;
                dirKeys[1] = Keys.Left;
                dirKeys[2] = Keys.Up;
                dirKeys[3] = Keys.Right;
            }

            if (actionKeys != null)
            {
                actionKeys[(int)FrameInput.InputType.Confirm] = Keys.X;
                actionKeys[(int)FrameInput.InputType.Cancel] = Keys.Z;
                actionKeys[(int)FrameInput.InputType.Attack] = Keys.X;
                actionKeys[(int)FrameInput.InputType.Run] = Keys.Z;
                actionKeys[(int)FrameInput.InputType.Skills] = Keys.A;
                actionKeys[(int)FrameInput.InputType.Turn] = Keys.S;
                actionKeys[(int)FrameInput.InputType.Diagonal] = Keys.D;
                actionKeys[(int)FrameInput.InputType.Menu] = Keys.Escape;
                actionKeys[(int)FrameInput.InputType.MsgLog] = Keys.Tab;
                actionKeys[(int)FrameInput.InputType.SkillMenu] = Keys.Q;
                actionKeys[(int)FrameInput.InputType.ItemMenu] = Keys.W;
                actionKeys[(int)FrameInput.InputType.TacticMenu] = Keys.E;
                actionKeys[(int)FrameInput.InputType.TeamMenu] = Keys.R;
                actionKeys[(int)FrameInput.InputType.TeamMode] = Keys.C;
                actionKeys[(int)FrameInput.InputType.Minimap] = Keys.Back;
                actionKeys[(int)FrameInput.InputType.LeaderSwap1] = Keys.D1;
                actionKeys[(int)FrameInput.InputType.LeaderSwap2] = Keys.D2;
                actionKeys[(int)FrameInput.InputType.LeaderSwap3] = Keys.D3;
                actionKeys[(int)FrameInput.InputType.LeaderSwap4] = Keys.D4;
                actionKeys[(int)FrameInput.InputType.Skill1] = Keys.S;
                actionKeys[(int)FrameInput.InputType.Skill2] = Keys.D;
                actionKeys[(int)FrameInput.InputType.Skill3] = Keys.Z;
                actionKeys[(int)FrameInput.InputType.Skill4] = Keys.X;
                actionKeys[(int)FrameInput.InputType.SortItems] = Keys.S;
                actionKeys[(int)FrameInput.InputType.SelectItems] = Keys.A;
                actionKeys[(int)FrameInput.InputType.SkillPreview] = Keys.Back;
            }

            if (actionButtons != null)
            {
                actionButtons[(int)FrameInput.InputType.Confirm] = Buttons.A;
                actionButtons[(int)FrameInput.InputType.Cancel] = Buttons.B;
                actionButtons[(int)FrameInput.InputType.Attack] = Buttons.A;
                actionButtons[(int)FrameInput.InputType.Run] = Buttons.B;
                actionButtons[(int)FrameInput.InputType.Skills] = Buttons.LeftTrigger;
                actionButtons[(int)FrameInput.InputType.Turn] = Buttons.X;
                actionButtons[(int)FrameInput.InputType.Diagonal] = Buttons.RightTrigger;
                actionButtons[(int)FrameInput.InputType.Menu] = Buttons.Y;
                actionButtons[(int)FrameInput.InputType.TeamMode] = Buttons.Start;
                actionButtons[(int)FrameInput.InputType.Minimap] = Buttons.Back;
                actionButtons[(int)FrameInput.InputType.LeaderSwapBack] = Buttons.LeftShoulder;
                actionButtons[(int)FrameInput.InputType.LeaderSwapForth] = Buttons.RightShoulder;
                actionButtons[(int)FrameInput.InputType.Skill1] = Buttons.X;
                actionButtons[(int)FrameInput.InputType.Skill2] = Buttons.Y;
                actionButtons[(int)FrameInput.InputType.Skill3] = Buttons.A;
                actionButtons[(int)FrameInput.InputType.Skill4] = Buttons.B;
                actionButtons[(int)FrameInput.InputType.SortItems] = Buttons.X;
                actionButtons[(int)FrameInput.InputType.SelectItems] = Buttons.LeftTrigger;
                actionButtons[(int)FrameInput.InputType.SkillPreview] = Buttons.RightTrigger;
            }
        }

        /// <summary>
        /// Determines whether the specified input type can be bound to a keyboard key.
        /// </summary>
        /// <param name="input">The input type to check.</param>
        /// <returns>True if the input type supports keyboard binding; otherwise, false.</returns>
        public static bool UsedByKeyboard(FrameInput.InputType input)
        {
            switch (input)
            {
                case FrameInput.InputType.Confirm: return true;
                case FrameInput.InputType.Cancel: return true;
                case FrameInput.InputType.Attack: return true;
                case FrameInput.InputType.Run: return true;
                case FrameInput.InputType.Skills: return true;
                case FrameInput.InputType.Turn: return true;
                case FrameInput.InputType.Diagonal: return true;
                case FrameInput.InputType.TeamMode: return true;
                case FrameInput.InputType.Minimap: return true;
                case FrameInput.InputType.Menu: return true;
                case FrameInput.InputType.MsgLog: return true;
                case FrameInput.InputType.SkillMenu: return true;
                case FrameInput.InputType.ItemMenu: return true;
                case FrameInput.InputType.TacticMenu: return true;
                case FrameInput.InputType.TeamMenu: return true;
                case FrameInput.InputType.LeaderSwap1: return true;
                case FrameInput.InputType.LeaderSwap2: return true;
                case FrameInput.InputType.LeaderSwap3: return true;
                case FrameInput.InputType.LeaderSwap4: return true;
                case FrameInput.InputType.Skill1: return true;
                case FrameInput.InputType.Skill2: return true;
                case FrameInput.InputType.Skill3: return true;
                case FrameInput.InputType.Skill4: return true;
                case FrameInput.InputType.SortItems: return true;
                case FrameInput.InputType.SelectItems: return true;
                case FrameInput.InputType.SkillPreview: return true;
                default: return false;
            }
        }

        /// <summary>
        /// Determines whether the specified input type can be bound to a gamepad button.
        /// </summary>
        /// <param name="input">The input type to check.</param>
        /// <returns>True if the input type supports gamepad binding; otherwise, false.</returns>
        public static bool UsedByGamepad(FrameInput.InputType input)
        {
            switch (input)
            {
                case FrameInput.InputType.Confirm: return true;
                case FrameInput.InputType.Cancel: return true;
                case FrameInput.InputType.Attack: return true;
                case FrameInput.InputType.Run: return true;
                case FrameInput.InputType.Skills: return true;
                case FrameInput.InputType.Turn: return true;
                case FrameInput.InputType.Diagonal: return true;
                case FrameInput.InputType.TeamMode: return true;
                case FrameInput.InputType.Minimap: return true;
                case FrameInput.InputType.Menu: return true;
                case FrameInput.InputType.LeaderSwapBack: return true;
                case FrameInput.InputType.LeaderSwapForth: return true;
                case FrameInput.InputType.Skill1: return true;
                case FrameInput.InputType.Skill2: return true;
                case FrameInput.InputType.Skill3: return true;
                case FrameInput.InputType.Skill4: return true;
                case FrameInput.InputType.SortItems: return true;
                case FrameInput.InputType.SelectItems: return true;
                case FrameInput.InputType.SkillPreview: return true;
                default: return false;
            }
        }

    }

    /// <summary>
    /// Provides localized text formatting with control input placeholders.
    /// Replaces format arguments with the current control bindings for specified input types.
    /// </summary>
    [Serializable]
    public class LocalFormatControls : LocalFormat
    {
        /// <summary>
        /// The list of input types whose control bindings will be used as format arguments.
        /// </summary>
        public List<FrameInput.InputType> Enums;

        /// <summary>
        /// Initializes a new empty instance of the LocalFormatControls class.
        /// </summary>
        public LocalFormatControls() { Enums = new List<FrameInput.InputType>(); }

        /// <summary>
        /// Initializes a new instance of the LocalFormatControls class with the specified key and input types.
        /// </summary>
        /// <param name="keyString">The localization string key.</param>
        /// <param name="inputs">The input types whose control bindings will be inserted.</param>
        public LocalFormatControls(string keyString, params FrameInput.InputType[] inputs)
        {
            Key = new StringKey(keyString);
            Enums = new List<FrameInput.InputType>();
            Enums.AddRange(inputs);
        }
        /// <summary>
        /// Initializes a new instance of the LocalFormatControls class by copying from another instance.
        /// </summary>
        /// <param name="other">The LocalFormatControls instance to copy from.</param>
        public LocalFormatControls(LocalFormatControls other) : base(other)
        {
            Enums = new List<FrameInput.InputType>();
            Enums.AddRange(other.Enums);
        }

        /// <summary>
        /// Creates a deep copy of this LocalFormatControls instance.
        /// </summary>
        /// <returns>A new LocalFormatControls instance with the same values.</returns>
        public override LocalFormat Clone() { return new LocalFormatControls(this); }

        /// <summary>
        /// Formats the localized string, replacing placeholders with the current control bindings.
        /// </summary>
        /// <returns>The formatted localized string with control bindings inserted.</returns>
        public override string FormatLocal()
        {
            List<string> enumStrings = new List<string>();
            foreach (FrameInput.InputType t in Enums)
                enumStrings.Add(DiagManager.Instance.GetControlString(t));
            return Text.FormatGrammar(Key.ToLocal(), enumStrings.ToArray());
        }
    }
}
