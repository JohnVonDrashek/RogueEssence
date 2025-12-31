using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using System.Text;

namespace RogueEssence.Data
{
    /// <summary>
    /// Contains all data needed to replay a game session, including actions, states, and UI inputs.
    /// Supports verification of replays to detect desyncs.
    /// </summary>
    [Serializable]
    public class ReplayData
    {
        /// <summary>
        /// Types of log entries that can be recorded during replay.
        /// </summary>
        public enum ReplayLog
        {
            StateLog = 0,
            GameLog,
            UILog,
            QuicksaveLog,
            GroundsaveLog,
            OptionLog,
        }
        
        /// <summary>
        /// The directory where the record is stored. Not serialized.
        /// </summary>
        [NonSerialized]
        public string RecordDir;

        /// <summary>
        /// The version of the game when the record was created.
        /// </summary>
        public Version RecordVersion;

        /// <summary>
        /// The language setting when the record was created.
        /// </summary>
        public string RecordLang;

        /// <summary>
        /// Total session time accumulated before quicksave.
        /// </summary>
        public long SessionTime;

        /// <summary>
        /// The time when the current session started.
        /// </summary>
        public long SessionStartTime;

        /// <summary>
        /// The position in the replay for quicksave state.
        /// </summary>
        public long QuicksavePos;

        /// <summary>
        /// The position in the replay for ground save state.
        /// </summary>
        public long GroundsavePos;

        /// <summary>
        /// Current index in the States list during playback.
        /// </summary>
        public int CurrentState;

        /// <summary>
        /// Current index in the Actions list during playback.
        /// </summary>
        public int CurrentAction;

        /// <summary>
        /// Current index in the UICodes list during playback.
        /// </summary>
        public int CurrentUI;

        /// <summary>
        /// Whether replay playback is paused.
        /// </summary>
        public bool Paused;

        /// <summary>
        /// Whether the menu is open during playback.
        /// </summary>
        public bool OpenMenu;

        /// <summary>
        /// The playback speed of the replay.
        /// </summary>
        public GameManager.GameSpeed ReplaySpeed;

        /// <summary>
        /// List of game states recorded at key points.
        /// </summary>
        public List<GameState> States;

        /// <summary>
        /// List of player actions recorded during gameplay.
        /// </summary>
        public List<GameAction> Actions;

        /// <summary>
        /// List of UI input codes recorded during gameplay.
        /// </summary>
        public List<int> UICodes;

        /// <summary>
        /// Count of desyncs detected during verification.
        /// </summary>
        public int Desyncs;

        /// <summary>
        /// Whether to suppress desync messages during verification.
        /// </summary>
        public bool SilentVerify;

        /// <summary>
        /// Initializes a new instance of the ReplayData class with default values.
        /// </summary>
        public ReplayData()
        {
            RecordDir = "";
            States = new List<GameState>();
            Actions = new List<GameAction>();
            UICodes = new List<int>();
            RecordVersion = new Version();
            ReplaySpeed = GameManager.GameSpeed.Normal;
        }

        /// <summary>
        /// Reads the next game state from the replay.
        /// </summary>
        /// <returns>The next GameState in the sequence.</returns>
        public GameState ReadState()
        {
            GameState save = States[CurrentState];
            CurrentState++;
            return save;
        }

        /// <summary>
        /// Reads the next action command from the replay.
        /// </summary>
        /// <returns>The next GameAction in the sequence.</returns>
        public GameAction ReadCommand()
        {
            GameAction cmd = Actions[CurrentAction];
            CurrentAction++;
            return cmd;
        }

        /// <summary>
        /// Reads the next UI code from the replay.
        /// </summary>
        /// <returns>The next UI code integer.</returns>
        public int ReadUI()
        {
            int cmd = UICodes[CurrentUI];
            CurrentUI++;
            return cmd;
        }

        /// <summary>
        /// Reads a string from UI codes, where the first code is the length.
        /// </summary>
        /// <returns>The reconstructed string from UI codes.</returns>
        public string ReadUIString()
        {
            int count = ReadUI();
            StringBuilder str = new StringBuilder();
            for (int ii = 0; ii < count; ii++)
                str.Append((char)ReadUI());
            return str.ToString();
        }
    }

    /// <summary>
    /// Represents a saved game state at a specific point in the replay.
    /// Contains the game progress and zone data.
    /// </summary>
    [Serializable]
    public class GameState
    {
        /// <summary>
        /// The saved game progress data.
        /// </summary>
        public GameProgress Save;

        /// <summary>
        /// The zone manager state at this point.
        /// </summary>
        public ZoneManager Zone;
    }
}
