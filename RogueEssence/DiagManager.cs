using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Input;
using System.Text;
using RogueEssence.Dev;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using RogueElements;

namespace RogueEssence
{
    /// <summary>
    /// Manages diagnostics, logging, settings persistence, input recording/playback, and control configuration.
    /// Provides a singleton instance accessible throughout the application.
    /// </summary>
    public class DiagManager
    {
        private static DiagManager instance;

        /// <summary>
        /// Initializes the singleton instance of DiagManager.
        /// </summary>
        public static void InitInstance()
        {
            instance = new DiagManager();
        }
        /// <summary>
        /// Gets the singleton instance of DiagManager.
        /// </summary>
        public static DiagManager Instance { get { return instance; } }

        /// <summary>
        /// Path to control button label graphics.
        /// </summary>
        public static string CONTROLS_LABEL_PATH { get => PathMod.ASSET_PATH + "Controls/Label/"; }

        /// <summary>
        /// Path to default control configuration files.
        /// </summary>
        public static string CONTROLS_DEFAULT_PATH { get => PathMod.ASSET_PATH + "Controls/Default/"; }

        /// <summary>
        /// Path to user configuration files.
        /// </summary>
        public static string CONFIG_PATH { get => PathMod.APP_PATH + "CONFIG/"; }

        /// <summary>
        /// Path to gamepad-specific configuration files.
        /// </summary>
        public static string CONFIG_GAMEPAD_PATH { get => CONFIG_PATH + "GAMEPAD/"; }

        /// <summary>
        /// Path to log files.
        /// </summary>
        public static string LOG_PATH { get => PathMod.APP_PATH + "LOG/"; }

        /// <summary>
        /// Windows registry path for application settings.
        /// </summary>
        public const string REG_PATH = "HKEY_CURRENT_USER\\Software\\RogueEssence";


        private object lockObj = new object();

        /// <summary>
        /// Delegate for handling log message events.
        /// </summary>
        /// <param name="message">The log message.</param>
        public delegate void LogAdded(string message);

        /// <summary>
        /// Delegate for getting additional error trace information.
        /// </summary>
        /// <returns>Additional trace information to append to error logs.</returns>
        public delegate string ErrorTrace();

        private bool inError;
        private LogAdded errorAddedEvent;
        private ErrorTrace errorTraceEvent;

        /// <summary>
        /// Gets whether input is currently being recorded.
        /// </summary>
        public bool RecordingInput { get { return (ActiveDebugReplay == null && inputWriter != null); } }
        private BinaryWriter inputWriter;

        /// <summary>
        /// The list of recorded inputs for debug replay, or null if not replaying.
        /// </summary>
        public List<FrameInput> ActiveDebugReplay;

        /// <summary>
        /// The current index in the debug replay sequence.
        /// </summary>
        public int DebugReplayIndex;

        /// <summary>
        /// Whether developer mode is enabled.
        /// </summary>
        public bool DevMode;

        /// <summary>
        /// Whether Lua debugging with listener is enabled.
        /// </summary>
        public bool DebugLua;

        /// <summary>
        /// The root editor interface for development tools.
        /// </summary>
        public IRootEditor DevEditor;

        /// <summary>
        /// Whether to listen and log map generation events.
        /// </summary>
        public bool ListenGen;

        /// <summary>
        /// Gets whether a gamepad is currently the active input device.
        /// </summary>
        public bool GamePadActive { get; private set; }

        private string gamePadID;

        /// <summary>
        /// Dictionary mapping gamepad GUIDs to their button label mappings.
        /// </summary>
        public Dictionary<string, Dictionary<Buttons, string>> ButtonToLabel { get; private set; }

        /// <summary>
        /// Dictionary mapping keyboard keys to their display labels.
        /// </summary>
        public Dictionary<Keys, string> KeyToLabel { get; private set; }

        /// <summary>
        /// Dictionary of default gamepad configurations by GUID.
        /// </summary>
        public Dictionary<string, GamePadMap> GamepadDefaults { get; set; }

        /// <summary>
        /// Gets the action button mappings for the current gamepad.
        /// </summary>
        public Buttons[] CurActionButtons { get { return CurSettings.GamepadMaps[gamePadID].ActionButtons; } }

        /// <summary>
        /// Gets the display name of the current gamepad.
        /// </summary>
        public string CurGamePadName { get { return CurSettings.GamepadMaps[gamePadID].Name; } }

        /// <summary>
        /// The current user settings.
        /// </summary>
        public Settings CurSettings;

        private string loadMessage;

        /// <summary>
        /// Gets or sets the current loading message displayed during initialization.
        /// Thread-safe property.
        /// </summary>
        public string LoadMsg
        {
            get
            {
                lock (lockObj)
                    return loadMessage;
            }
            set
            {
                lock (lockObj)
                    loadMessage = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DiagManager class.
        /// Creates required directories and initializes default settings.
        /// </summary>
        public DiagManager()
        {
            if (!Directory.Exists(LOG_PATH))
                Directory.CreateDirectory(LOG_PATH);
            if (!Directory.Exists(PathMod.MODS_PATH))
                Directory.CreateDirectory(PathMod.MODS_PATH);
            if (!Directory.Exists(CONFIG_PATH))
                Directory.CreateDirectory(CONFIG_PATH);
            if (!Directory.Exists(CONFIG_GAMEPAD_PATH))
                Directory.CreateDirectory(CONFIG_GAMEPAD_PATH);

            Settings.InitStatic();
            CurSettings = new Settings();
            gamePadID = "default";
            ButtonToLabel = new Dictionary<string, Dictionary<Buttons, string>>();
            KeyToLabel = new Dictionary<Keys, string>();
            GamepadDefaults = new Dictionary<string, GamePadMap>();
            FNALoggerEXT.LogInfo = LogInfo;
            FNALoggerEXT.LogWarn = LogInfo;
            FNALoggerEXT.LogError = LogInfo;
        }

        /// <summary>
        /// Sets the error event handlers for logging and tracing.
        /// </summary>
        /// <param name="errorAdded">Handler called when an error is logged.</param>
        /// <param name="errorTrace">Handler that provides additional trace information.</param>
        public void SetErrorListener(LogAdded errorAdded, ErrorTrace errorTrace)
        {
            errorAddedEvent = errorAdded;
            errorTraceEvent = errorTrace;
        }

        /// <summary>
        /// Subscribes to RogueElements map generation events for debugging.
        /// </summary>
        public void ListenToMapGen()
        {
            GenContextDebug.OnError += logRogueElementsError;
            GenContextDebug.OnStepIn += logRogueElements;
            GenContextDebug.OnStep += logRogueElements;
        }

        private List<string> logMsgs = new List<string>();
        private void logRogueElementsError(Exception ex)
        {
            LogError(ex);
            if (ListenGen)
            {
                logMsgs.Add(String.Format("[{0}] Mapgen: {1}: {2}", String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now), ex.GetType().ToString(), ex.Message));
            }
        }
        private void logRogueElements(string msg)
        {
            if (ListenGen)
            {
                logMsgs.Add(String.Format("[{1}] Mapgen: {0}", msg, String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now)));
            }
        }

        /// <summary>
        /// Writes all accumulated RogueElements log messages to the log file and clears the buffer.
        /// </summary>
        public void FlushRogueElements()
        {
            StringBuilder builder = new StringBuilder("Steps:\n");
            foreach (string msg in logMsgs)
            {
                builder.Append(msg);
                builder.Append("\n");
            }
            LogInfo(builder.ToString());
            logMsgs.Clear();
        }

        /// <summary>
        /// Cleans up resources, ending any active input recording.
        /// </summary>
        public void Unload()
        {
            EndInput();
        }


        /// <summary>
        /// Begins recording input to a timestamped log file.
        /// Records the random seed and all subsequent frame inputs.
        /// </summary>
        public void BeginInput()
        {
            try
            {
                inputWriter = new BinaryWriter(new FileStream(LOG_PATH + String.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now), FileMode.Create, FileAccess.Write, FileShare.None));

                inputWriter.Write(RogueElements.MathUtils.Rand.FirstSeed);

                inputWriter.Flush();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Records a single frame of input to the active recording.
        /// </summary>
        /// <param name="input">The frame input to record.</param>
        public void LogInput(FrameInput input)
        {
            if (inputWriter != null)
            {
                try
                {
                    inputWriter.Write((byte)((int)input.Direction));
                    for (int ii = 0; ii < (int)FrameInput.InputType.Ctrl; ii++)
                        inputWriter.Write(input[(FrameInput.InputType)ii]);
                    //for (int ii = 0; ii < FrameInput.TOTAL_CHARS; ii++)
                    //    inputWriter.Write(input.CharInput[ii]);

                    inputWriter.Flush();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }


        /// <summary>
        /// Ends the current input recording session and closes the file.
        /// </summary>
        public void EndInput()
        {
            try
            {
                if (inputWriter != null)
                {
                    inputWriter.Close();
                    inputWriter = null;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Loads a recorded input file for debug replay.
        /// Restores the random seed and populates ActiveDebugReplay with recorded inputs.
        /// </summary>
        /// <param name="path">The filename of the recording in the LOG_PATH directory.</param>
        public void LoadInputs(string path)
        {
            try
            {
                using (FileStream stream = File.OpenRead(LOG_PATH + path))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        //seed
                        RogueElements.MathUtils.ReSeedRand(reader.ReadUInt64());

                        //all inputs
                        ActiveDebugReplay = new List<FrameInput>();

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            FrameInput input = FrameInput.Load(reader);
                            ActiveDebugReplay.Add(input);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ActiveDebugReplay = null;
            }
        }


        /// <summary>
        /// Logs an error to console and output log with full stack trace.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogError(Exception exception)
        {
            LogError(exception, true);
        }

        /// <summary>
        /// Logs an error to console and output log. Outputs the entire stack trace including inner exceptions.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="signal">Triggers error event handlers if true. Logs silently if false.</param>
        public void LogError(Exception exception, bool signal)
        {
            lock (lockObj)
            {
                if (inError)
                    throw new InvalidOperationException("Attempted to log an error when logging an error.", exception);
                inError = true;
                try
                {
                    StringBuilder errorMsg = new StringBuilder();
                    errorMsg.Append(String.Format("[{0}] {1}", String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now), exception.Message));
                    errorMsg.Append("\n");
                    Exception innerException = exception;
                    int depth = 0;
                    while (innerException != null)
                    {
                        errorMsg.Append("Exception Depth: " + depth);
                        errorMsg.Append("\n");

                        errorMsg.Append(innerException.ToString());
                        errorMsg.Append("\n\n");

                        innerException = innerException.InnerException;
                        depth++;
                    }

                    if (errorTraceEvent != null)
                        errorMsg.Append(errorTraceEvent());

                    Console.WriteLine(errorMsg);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(errorMsg);
#endif

                    if (errorAddedEvent != null && signal)
                        errorAddedEvent(exception.Message);


                    string filePath = LOG_PATH + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                        writer.Write(errorMsg);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                inError = false;
            }
        }

        /// <summary>
        /// Logs an informational message to console and log file.
        /// </summary>
        /// <param name="diagInfo">The message to log.</param>
        public void LogInfo(string diagInfo)
        {
            lock (lockObj)
            {
                string fullMsg = String.Format("[{0}] {1}", String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now), diagInfo);
                if (DevMode)
                {
                    Console.WriteLine(fullMsg);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(fullMsg);
#endif
                }

                try
                {
                    string filePath = LOG_PATH + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(fullMsg);
                        writer.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Loads control label mappings and default gamepad configurations from XML files.
        /// Populates ButtonToLabel, KeyToLabel, and GamepadDefaults.
        /// </summary>
        public void SetupInputs()
        {
            ButtonToLabel = new Dictionary<string, Dictionary<Buttons, string>>();
            //try to load from file
            string[] filePaths = Directory.GetFiles(CONTROLS_LABEL_PATH, "*.xml");
            foreach (string filePath in filePaths)
            {
                Dictionary<Buttons, string> mapping = new Dictionary<Buttons, string>();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filePath);

                foreach (XmlNode button in xmldoc.SelectNodes("root/Button"))
                {
                    Buttons btn = Enum.Parse<Buttons>(button.Attributes["name"].Value);
                    mapping[btn] = button.InnerText;
                }

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                ButtonToLabel[fileName] = mapping;
            }

            KeyToLabel = new Dictionary<Keys, string>();
            {
                string filePath = CONTROLS_LABEL_PATH + "keyboard.xml";
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filePath);

                foreach (XmlNode button in xmldoc.SelectNodes("root/Key"))
                {
                    Keys btn = Enum.Parse<Keys>(button.Attributes["name"].Value);
                    KeyToLabel[btn] = button.InnerText;
                }
            }

            filePaths = Directory.GetFiles(CONTROLS_DEFAULT_PATH, "*.xml");
            foreach (string filePath in filePaths)
            {
                GamePadMap mapping = new GamePadMap();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filePath);

                mapping.Name = xmldoc.SelectSingleNode("Config/Name").InnerText;

                int index = 0;
                XmlNode keys = xmldoc.SelectSingleNode("Config/ActionButtons");
                foreach (XmlNode key in keys.SelectNodes("ActionButton"))
                {
                    while (!Settings.UsedByGamepad((FrameInput.InputType)index) && index < mapping.ActionButtons.Length)
                        index++;
                    mapping.ActionButtons[index] = Enum.Parse<Buttons>(key.InnerText);
                    index++;
                }

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                GamepadDefaults[fileName] = mapping;
            }
        }

        /// <summary>
        /// Loads user settings from XML configuration files.
        /// Returns default settings if files don't exist or fail to load.
        /// </summary>
        /// <returns>The loaded or default Settings object.</returns>
        public Settings LoadSettings()
        {
            //try to load from file

            Settings settings = new Settings();

            string path = CONFIG_PATH + "Config.xml";
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);

                    settings.BGMBalance = Int32.Parse(xmldoc.SelectSingleNode("Config/BGM").InnerText);
                    settings.SEBalance = Int32.Parse(xmldoc.SelectSingleNode("Config/SE").InnerText);

                    settings.BattleFlow = Enum.Parse<Settings.BattleSpeed>(xmldoc.SelectSingleNode("Config/BattleFlow").InnerText);
                    settings.DefaultSkills = Enum.Parse<Settings.SkillDefault>(xmldoc.SelectSingleNode("Config/DefaultSkills").InnerText);
                    settings.Minimap = Int32.Parse(xmldoc.SelectSingleNode("Config/Minimap").InnerText);
                    settings.MinimapColor = Enum.Parse<Settings.MinimapStyle>(xmldoc.SelectSingleNode("Config/MinimapColor").InnerText);

                    settings.TextSpeed = Double.Parse(xmldoc.SelectSingleNode("Config/TextSpeed").InnerText);
                    settings.Border = Int32.Parse(xmldoc.SelectSingleNode("Config/Border").InnerText);
                    settings.Window = Int32.Parse(xmldoc.SelectSingleNode("Config/Window").InnerText);
                    settings.Language = xmldoc.SelectSingleNode("Config/Language").InnerText;
                    settings.InactiveInput = Boolean.Parse(xmldoc.SelectSingleNode("Config/InactiveInput").InnerText);
                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }

            path = CONFIG_PATH + "Keyboard.xml";
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);

                    int index = 0;
                    XmlNode keys = xmldoc.SelectSingleNode("Config/DirKeys");
                    foreach (XmlNode key in keys.SelectNodes("DirKey"))
                    {
                        settings.DirKeys[index] = Enum.Parse<Keys>(key.InnerText);
                        index++;
                    }

                    index = 0;
                    keys = xmldoc.SelectSingleNode("Config/ActionKeys");
                    foreach (XmlNode key in keys.SelectNodes("ActionKey"))
                    {
                        while (!Settings.UsedByKeyboard((FrameInput.InputType)index) && index < settings.ActionKeys.Length)
                            index++;
                        settings.ActionKeys[index] = Enum.Parse<Keys>(key.InnerText);
                        index++;
                    }

                    settings.Enter = Boolean.Parse(xmldoc.SelectSingleNode("Config/Enter").InnerText);
                    settings.NumPad = Boolean.Parse(xmldoc.SelectSingleNode("Config/NumPad").InnerText);
                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }


            string[] filePaths = Directory.GetFiles(CONFIG_GAMEPAD_PATH, "*.xml");
            foreach (string filePath in filePaths)
            {
                try
                {
                    GamePadMap mapping = new GamePadMap();
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(filePath);

                    mapping.Name = xmldoc.SelectSingleNode("Config/Name").InnerText;

                    int index = 0;
                    XmlNode keys = xmldoc.SelectSingleNode("Config/ActionButtons");
                    foreach (XmlNode key in keys.SelectNodes("ActionButton"))
                    {
                        while (!Settings.UsedByGamepad((FrameInput.InputType)index) && index < mapping.ActionButtons.Length)
                            index++;
                        mapping.ActionButtons[index] = Enum.Parse<Buttons>(key.InnerText);
                        index++;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    settings.GamepadMaps[fileName] = mapping;
                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }

            path = CONFIG_PATH + "Contacts.xml";
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);

                    XmlNode contacts = xmldoc.SelectSingleNode("Config/Servers");
                    foreach (XmlNode contact in contacts.SelectNodes("Server"))
                    {
                        ServerInfo info = new ServerInfo();
                        info.ServerName = contact.SelectSingleNode("Name").InnerText;
                        info.IP = contact.SelectSingleNode("IP").InnerText;
                        info.Port = Int32.Parse(contact.SelectSingleNode("Port").InnerText);
                        settings.ServerList.Add(info);
                    }

                    contacts = xmldoc.SelectSingleNode("Config/Contacts");
                    foreach (XmlNode contact in contacts.SelectNodes("Contact"))
                    {
                        ContactInfo info = new ContactInfo();
                        info.UUID = contact.SelectSingleNode("UUID").InnerText;
                        info.LastContact = contact.SelectSingleNode("LastSeen").InnerText;
                        loadContactNode(contact, info);
                        settings.ContactList.Add(info);
                    }

                    contacts = xmldoc.SelectSingleNode("Config/Peers");
                    foreach (XmlNode contact in contacts.SelectNodes("Peer"))
                    {
                        PeerInfo info = new PeerInfo();
                        info.UUID = contact.SelectSingleNode("UUID").InnerText;
                        info.LastContact = contact.SelectSingleNode("LastSeen").InnerText;
                        loadContactNode(contact, info);
                        info.IP = contact.SelectSingleNode("IP").InnerText;
                        info.Port = Int32.Parse(contact.SelectSingleNode("Port").InnerText);
                        settings.PeerList.Add(info);
                    }

                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }
            return settings;
        }

        /// <summary>
        /// Saves user settings to XML configuration files.
        /// Creates separate files for general config, keyboard, gamepad, and contacts.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        public void SaveSettings(Settings settings)
        {
            {
                XmlDocument xmldoc = new XmlDocument();

                XmlNode docNode = xmldoc.CreateElement("Config");
                xmldoc.AppendChild(docNode);

                docNode.AppendInnerTextChild(xmldoc, "BGM", settings.BGMBalance.ToString());
                docNode.AppendInnerTextChild(xmldoc, "SE", settings.SEBalance.ToString());

                docNode.AppendInnerTextChild(xmldoc, "BattleFlow", settings.BattleFlow.ToString());
                docNode.AppendInnerTextChild(xmldoc, "DefaultSkills", settings.DefaultSkills.ToString());
                docNode.AppendInnerTextChild(xmldoc, "Minimap", settings.Minimap.ToString());
                docNode.AppendInnerTextChild(xmldoc, "MinimapColor", settings.MinimapColor.ToString());

                docNode.AppendInnerTextChild(xmldoc, "TextSpeed", settings.TextSpeed.ToString());
                docNode.AppendInnerTextChild(xmldoc, "Border", settings.Border.ToString());
                docNode.AppendInnerTextChild(xmldoc, "Window", settings.Window.ToString());
                docNode.AppendInnerTextChild(xmldoc, "Language", settings.Language.ToString());
                docNode.AppendInnerTextChild(xmldoc, "InactiveInput", settings.InactiveInput.ToString());

                xmldoc.Save(CONFIG_PATH + "Config.xml");
            }

            {
                XmlDocument xmldoc = new XmlDocument();

                XmlNode docNode = xmldoc.CreateElement("Config");
                xmldoc.AppendChild(docNode);

                XmlNode dirKeys = xmldoc.CreateElement("DirKeys");
                foreach (Keys key in settings.DirKeys)
                    dirKeys.AppendInnerTextChild(xmldoc, "DirKey", key.ToString());
                docNode.AppendChild(dirKeys);

                XmlNode actionKeys = xmldoc.CreateElement("ActionKeys");
                for (int ii = 0; ii < settings.ActionKeys.Length; ii++)
                {
                    if (!Settings.UsedByKeyboard((FrameInput.InputType)ii))
                        continue;
                    Keys key = settings.ActionKeys[ii];
                    actionKeys.AppendInnerTextChild(xmldoc, "ActionKey", key.ToString());
                }
                docNode.AppendChild(actionKeys);
                docNode.AppendInnerTextChild(xmldoc, "Enter", settings.Enter.ToString());
                docNode.AppendInnerTextChild(xmldoc, "NumPad", settings.NumPad.ToString());

                xmldoc.Save(CONFIG_PATH + "Keyboard.xml");
            }

            foreach(string guid in settings.GamepadMaps.Keys)
            {
                if (guid == "default")
                    continue;

                GamePadMap gamePadMap = settings.GamepadMaps[guid];

                XmlDocument xmldoc = new XmlDocument();

                XmlNode docNode = xmldoc.CreateElement("Config");
                xmldoc.AppendChild(docNode);

                docNode.AppendInnerTextChild(xmldoc, "Name", gamePadMap.Name);

                XmlNode actionButtons = xmldoc.CreateElement("ActionButtons");
                for (int ii = 0; ii < gamePadMap.ActionButtons.Length; ii++)
                {
                    if (!Settings.UsedByGamepad((FrameInput.InputType)ii))
                        continue;
                    Buttons button = gamePadMap.ActionButtons[ii];
                    actionButtons.AppendInnerTextChild(xmldoc, "ActionButton", button.ToString());
                }
                docNode.AppendChild(actionButtons);

                xmldoc.Save(CONFIG_GAMEPAD_PATH + guid + ".xml");
            }

            {
                XmlDocument xmldoc = new XmlDocument();

                XmlNode docNode = xmldoc.CreateElement("Config");
                xmldoc.AppendChild(docNode);

                XmlNode servers = xmldoc.CreateElement("Servers");
                foreach (ServerInfo contact in settings.ServerList)
                {
                    XmlNode node = xmldoc.CreateElement("Server");
                    node.AppendInnerTextChild(xmldoc, "Name", contact.ServerName);
                    node.AppendInnerTextChild(xmldoc, "IP", contact.IP);
                    node.AppendInnerTextChild(xmldoc, "Port", contact.Port.ToString());
                    servers.AppendChild(node);
                }
                docNode.AppendChild(servers);

                XmlNode contacts = xmldoc.CreateElement("Contacts");
                foreach (ContactInfo contact in settings.ContactList)
                {
                    XmlNode node = xmldoc.CreateElement("Contact");
                    node.AppendInnerTextChild(xmldoc, "UUID", contact.UUID);
                    node.AppendInnerTextChild(xmldoc, "LastSeen", contact.LastContact);
                    appendContactNode(xmldoc, node, contact);
                    contacts.AppendChild(node);
                }
                docNode.AppendChild(contacts);

                XmlNode peers = xmldoc.CreateElement("Peers");
                foreach (PeerInfo contact in settings.PeerList)
                {
                    XmlNode node = xmldoc.CreateElement("Peer");
                    node.AppendInnerTextChild(xmldoc, "UUID", contact.UUID);
                    node.AppendInnerTextChild(xmldoc, "LastSeen", contact.LastContact);
                    appendContactNode(xmldoc, node, contact);
                    node.AppendInnerTextChild(xmldoc, "IP", contact.IP);
                    node.AppendInnerTextChild(xmldoc, "Port", contact.Port.ToString());
                    peers.AppendChild(node);
                }
                docNode.AppendChild(peers);


                xmldoc.Save(CONFIG_PATH + "Contacts.xml");
            }
        }

        /// <summary>
        /// Loads mod configuration from ModConfig.xml.
        /// </summary>
        /// <returns>A tuple containing the quest mod header and array of enabled mod headers.</returns>
        public (ModHeader, ModHeader[]) LoadModSettings()
        {
            string path = CONFIG_PATH + "ModConfig.xml";
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);

                    string quest = xmldoc.SelectSingleNode("Config/Quest").InnerText;
                    ModHeader newQuest = ModHeader.Invalid;
                    if (!String.IsNullOrEmpty(quest))
                    {
                        ModHeader questHeader = PathMod.GetModDetails(PathMod.FromApp(quest));
                        if (questHeader.IsValid())
                            newQuest = questHeader;
                    }

                    List<ModHeader> modList = new List<ModHeader>();
                    XmlNode modsNode = xmldoc.SelectSingleNode("Config/Mods");
                    foreach (XmlNode modNode in modsNode.SelectNodes("Mod"))
                    {
                        string mod = modNode.InnerText;
                        if (!String.IsNullOrEmpty(mod))
                        {
                            ModHeader modHeader = PathMod.GetModDetails(PathMod.FromApp(mod));
                            if (modHeader.IsValid())
                                modList.Add(modHeader);
                        }
                    }

                    return (newQuest, modList.ToArray());
                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }
            return (ModHeader.Invalid, new ModHeader[0] { });
        }

        /// <summary>
        /// Logs the currently active quest and mods to the diagnostic log.
        /// </summary>
        public void PrintModSettings()
        {
            DiagManager.Instance.LogInfo("-----------------------------------------");
            if (PathMod.Quest.IsValid())
                DiagManager.Instance.LogInfo(String.Format("Quest: {0} {1} {2}", PathMod.Quest.Name, PathMod.Quest.Version, PathMod.Quest.UUID));
            foreach (ModHeader mod in PathMod.Mods)
                DiagManager.Instance.LogInfo(String.Format("Mod: {0} {1} {2}", mod.Name, mod.Version, mod.UUID));
            DiagManager.Instance.LogInfo("-----------------------------------------");
        }

        /// <summary>
        /// Saves the current mod configuration to ModConfig.xml.
        /// </summary>
        public void SaveModSettings()
        {
            XmlDocument xmldoc = new XmlDocument();

            XmlNode docNode = xmldoc.CreateElement("Config");
            xmldoc.AppendChild(docNode);

            docNode.AppendInnerTextChild(xmldoc, "Quest", PathMod.Quest.Path);

            XmlNode modsMode = xmldoc.CreateElement("Mods");
            foreach (ModHeader mod in PathMod.Mods)
                modsMode.AppendInnerTextChild(xmldoc, "Mod", mod.Path);
            docNode.AppendChild(modsMode);

            xmldoc.Save(CONFIG_PATH + "ModConfig.xml");
        }


        /// <summary>
        /// Updates the gamepad active state and loads appropriate gamepad configuration.
        /// </summary>
        /// <param name="active">Whether a gamepad should be considered active.</param>
        public void UpdateGamePadActive(bool active)
        {
            if (!GamePadActive && active)
            {
                string guid = GamePad.GetGUIDEXT(PlayerIndex.One);

                //if (guid.Equals("4c05c405") || guid.Equals("4c05cc09"))//PS4
                //else if (guid.Equals("4c05e60c"))//PS5
                //else if (guid.Equals("7e050920") || guid.Equals("7e053003"))//Nintendo
                //else if (guid.Equals("5e04ff02"))//Xbox
                gamePadID = guid;
                if (!CurSettings.GamepadMaps.ContainsKey(guid))
                {
                    GamePadMap defaultMap;
                    if (GamepadDefaults.TryGetValue(guid, out defaultMap))
                        CurSettings.GamepadMaps[guid] = new GamePadMap(defaultMap);
                    else
                        CurSettings.GamepadMaps[guid] = new GamePadMap(CurSettings.GamepadMaps["default"]);
                }
            }
            else if (GamePadActive && !active)
            {
                gamePadID = "default";
            }
            GamePadActive = active;
        }

        /// <summary>
        /// Gets the display string for a control input based on the current input device.
        /// Returns gamepad button or keyboard key label as appropriate.
        /// </summary>
        /// <param name="inputType">The input type to get the string for.</param>
        /// <returns>A localized control string (e.g., "[X]" or "(A)").</returns>
        public string GetControlString(FrameInput.InputType inputType)
        {
            if (GamePadActive)
            {
                GamePadMap gamePadMap = CurSettings.GamepadMaps[gamePadID];
                Buttons button = gamePadMap.ActionButtons[(int)inputType];
                if (button > 0)
                    return GetButtonString(button);
            }
            return GetKeyboardString(CurSettings.ActionKeys[(int)inputType]);
        }

        /// <summary>
        /// Gets the display string for a gamepad button.
        /// </summary>
        /// <param name="button">The button to get the string for.</param>
        /// <returns>A localized button string or the button name in parentheses.</returns>
        public string GetButtonString(Buttons button)
        {
            Dictionary<Buttons, string> dict;
            if (ButtonToLabel.TryGetValue(gamePadID, out dict) || ButtonToLabel.TryGetValue("default", out dict))
            {
                string mappedString;
                if (dict.TryGetValue(button, out mappedString))
                    return Regex.Unescape(mappedString);
            }

            return "(" + button.ToLocal() + ")";
        }

        /// <summary>
        /// Gets the display string for a keyboard key.
        /// </summary>
        /// <param name="key">The key to get the string for.</param>
        /// <returns>A localized key string or the key name in brackets.</returns>
        public string GetKeyboardString(Keys key)
        {
            string mappedString;
            if (KeyToLabel.TryGetValue(key, out mappedString))
                return Regex.Unescape(mappedString);

            return "[" + key.ToLocal() + "]";
        }


        private static void loadContactNode(XmlNode contact, ContactInfo info)
        {
            string hex = contact.SelectSingleNode("Data").InnerText;
            
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            info.DeserializeData(bytes);
        }

        private static void appendContactNode(XmlDocument xmldoc, XmlNode node, ContactInfo contact)
        {
            byte[] ba = contact.SerializeData();
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            hex.ToString();
            node.AppendInnerTextChild(xmldoc, "Data", hex.ToString());
        }

    }
}
