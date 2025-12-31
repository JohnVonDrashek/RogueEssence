using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueEssence.Ground;
using RogueEssence.Dev.Views;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the Game tab in the developer form.
    /// Provides functionality for spawning entities, managing items, skills, statuses, and intrinsics during gameplay.
    /// </summary>
    public class DevTabGameViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the DevTabGameViewModel class with empty collections.
        /// </summary>
        public DevTabGameViewModel()
        {
            Skills = new ObservableCollection<string>();
            SkillKeys = new List<string>();
            Intrinsics = new ObservableCollection<string>();
            IntrinsicKeys = new List<string>();

            Statuses = new ObservableCollection<string>();
            StatusKeys = new List<string>();
            Items = new ObservableCollection<string>();
            ItemKeys = new List<string>();
        }

        /// <summary>
        /// Reloads the skills list from the data manager.
        /// </summary>
        public void ReloadSkills()
        {
            Dictionary<string, string> entry_names = DataManager.Instance.DataIndices[DataManager.DataType.Skill].GetLocalStringArray(true);
            Skills.Clear();
            SkillKeys.Clear();
            foreach (string key in entry_names.Keys)
            {
                Skills.Add(key + ": " + entry_names[key]);
                SkillKeys.Add(key);
            }
            ChosenSkill = -1;
            ChosenSkill = Math.Min(Math.Max(DevForm.GetConfig("SkillChoice", 0), 0), Skills.Count - 1);
        }

        /// <summary>
        /// Reloads the intrinsics list from the data manager.
        /// </summary>
        public void ReloadIntrinsics()
        {
            Dictionary<string, string> entry_names = DataManager.Instance.DataIndices[DataManager.DataType.Intrinsic].GetLocalStringArray(true);
            Intrinsics.Clear();
            IntrinsicKeys.Clear();
            foreach (string key in entry_names.Keys)
            {
                Intrinsics.Add(key + ": " + entry_names[key]);
                IntrinsicKeys.Add(key);
            }
            ChosenIntrinsic = -1;
            ChosenIntrinsic = Math.Min(Math.Max(DevForm.GetConfig("IntrinsicChoice", 0), 0), Intrinsics.Count - 1);
        }

        /// <summary>
        /// Reloads the statuses list from the data manager.
        /// </summary>
        public void ReloadStatuses()
        {
            Dictionary<string, string> entry_names = DataManager.Instance.DataIndices[DataManager.DataType.Status].GetLocalStringArray(true);
            Statuses.Clear();
            StatusKeys.Clear();
            foreach (string key in entry_names.Keys)
            {
                Statuses.Add(key + ": " + entry_names[key]);
                StatusKeys.Add(key);
            }
            ChosenStatus = -1;
            ChosenStatus = Math.Min(Math.Max(DevForm.GetConfig("StatusChoice", 0), 0), Statuses.Count - 1);
        }

        /// <summary>
        /// Reloads the items list from the data manager.
        /// </summary>
        public void ReloadItems()
        {
            Dictionary<string, string> entry_names = DataManager.Instance.DataIndices[DataManager.DataType.Item].GetLocalStringArray(true);
            Items.Clear();
            ItemKeys.Clear();
            foreach (string key in entry_names.Keys)
            {
                Items.Add(key + ": " + entry_names[key]);
                ItemKeys.Add(key);
            }
            ChosenItem = -1;
            ChosenItem = Math.Min(Math.Max(DevForm.GetConfig("ItemChoice", 0), 0), Items.Count - 1);
        }

        public ObservableCollection<string> Skills { get; }

        public List<string> SkillKeys;

        private int chosenSkill;
        public int ChosenSkill
        {
            get { return chosenSkill; }
            set { this.SetIfChanged(ref chosenSkill, value); }
        }

        public ObservableCollection<string> Intrinsics { get; }

        public List<string> IntrinsicKeys;

        private int chosenIntrinsic;
        public int ChosenIntrinsic
        {
            get { return chosenIntrinsic; }
            set { this.SetIfChanged(ref chosenIntrinsic, value); }
        }

        public ObservableCollection<string> Statuses { get; }

        public List<string> StatusKeys;

        private int chosenStatus;
        public int ChosenStatus
        {
            get { return chosenStatus; }
            set { this.SetIfChanged(ref chosenStatus, value); }
        }

        public List<string> ItemKeys;
        public ObservableCollection<string> Items { get; }

        private int chosenItem;
        public int ChosenItem
        {
            get { return chosenItem; }
            set { this.SetIfChanged(ref chosenItem, value); }
        }

        private bool hideSprites;
        public bool HideSprites
        {
            get { return hideSprites; }
            set
            {
                this.SetIfChanged(ref hideSprites, value);
                DataManager.Instance.HideChars = value;
            }
        }

        private bool hideObjects;
        public bool HideObjects
        {
            get { return hideObjects; }
            set { this.SetIfChanged(ref hideObjects, value);
                DataManager.Instance.HideObjects = value;
            }
        }


        /// <summary>
        /// Spawns a clone of the focused character as a new monster on the map.
        /// </summary>
        public void btnSpawn_Click()
        {
            lock (GameBase.lockObj)
            {
                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    MonsterTeam team = new MonsterTeam();
                    Character new_mob = DungeonScene.Instance.FocusedCharacter.Clone(team);
                    ZoneManager.Instance.CurrentMap.MapTeams.Add(new_mob.MemberTeam);
                    new_mob.RefreshTraits();
                    DungeonScene.Instance.PendingDevEvent = new_mob.OnMapStart();
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Removes all ally and map teams from the current dungeon map.
        /// </summary>
        public void btnDespawn_Click()
        {
            lock (GameBase.lockObj)
            {
                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    ZoneManager.Instance.CurrentMap.AllyTeams.Clear();
                    ZoneManager.Instance.CurrentMap.MapTeams.Clear();
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Spawns the selected item on the map or adds it to the player's inventory.
        /// </summary>
        public void btnSpawnItem_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("ItemChoice", chosenItem);
                InvItem item = new InvItem(ItemKeys[chosenItem]);
                ItemData entry = (ItemData)item.GetData();
                if (entry.MaxStack > 1)
                    item.Amount = entry.MaxStack;
                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                    DungeonScene.Instance.PendingDevEvent = DungeonScene.Instance.DropItem(item, DungeonScene.Instance.FocusedCharacter.CharLoc);
                else if (GameManager.Instance.CurrentScene == GroundScene.Instance)
                {
                    if (DataManager.Instance.Save.ActiveTeam.GetInvCount() < DataManager.Instance.Save.ActiveTeam.GetMaxInvSlots(ZoneManager.Instance.CurrentZone))
                    {
                        GameManager.Instance.SE("Menu/Sort");
                        DataManager.Instance.Save.ActiveTeam.AddToInv(item);
                    }
                    else
                        GameManager.Instance.SE("Menu/Cancel");
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Toggles the selected status effect on the focused character.
        /// </summary>
        public void btnToggleStatus_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("StatusChoice", chosenStatus);
                StatusData entry = DataManager.Instance.GetStatus(StatusKeys[chosenStatus]);
                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
                    {
                        Character player = DungeonScene.Instance.FocusedCharacter;
                        if (entry.Targeted)
                            DungeonScene.Instance.LogMsg(String.Format("This is a targeted status."), false, true);
                        else
                        {
                            if (player.StatusEffects.ContainsKey(StatusKeys[chosenStatus]))
                                DungeonScene.Instance.PendingDevEvent = player.RemoveStatusEffect(StatusKeys[chosenStatus]);
                            else
                            {
                                StatusEffect status = new StatusEffect(StatusKeys[chosenStatus]);
                                status.LoadFromData();
                                DungeonScene.Instance.PendingDevEvent = player.AddStatusEffect(status);
                            }
                        }
                    }
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Teaches the selected skill to the focused character.
        /// </summary>
        public void btnLearnSkill_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("SkillChoice", chosenSkill);

                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
                    {
                        Character player = DungeonScene.Instance.FocusedCharacter;
                        if (!String.IsNullOrEmpty(player.BaseSkills[CharData.MAX_SKILL_SLOTS - 1].SkillNum))
                            player.DeleteSkill(0);
                        player.LearnSkill(SkillKeys[chosenSkill], true);
                        DungeonScene.Instance.LogMsg(String.Format("Taught {1} to {0}.", player.Name, DataManager.Instance.GetSkill(SkillKeys[chosenSkill]).Name.ToLocal()), false, true);
                    }
                }
                else if (GameManager.Instance.CurrentScene == GroundScene.Instance)
                {
                    if (DataManager.Instance.Save.ActiveTeam.Players.Count > 0)
                    {
                        Character player = DataManager.Instance.Save.ActiveTeam.Leader;
                        if (!String.IsNullOrEmpty(player.BaseSkills[CharData.MAX_SKILL_SLOTS - 1].SkillNum))
                            player.DeleteSkill(0);
                        player.LearnSkill(SkillKeys[chosenSkill], true);
                        GameManager.Instance.SE("Menu/Sort");
                    }
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Teaches the selected skill to all enemy characters on the map.
        /// </summary>
        public void btnGiveSkill_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("SkillChoice", chosenSkill);
                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    Character player = DungeonScene.Instance.FocusedCharacter;
                    foreach (Character character in ZoneManager.Instance.CurrentMap.IterateCharacters())
                    {
                        if (DungeonScene.Instance.GetMatchup(player, character) == Alignment.Foe)
                        {
                            if (!String.IsNullOrEmpty(character.BaseSkills[CharData.MAX_SKILL_SLOTS - 1].SkillNum))
                                character.DeleteSkill(0);
                            character.LearnSkill(SkillKeys[chosenSkill], true);
                        }
                    }
                    DungeonScene.Instance.LogMsg(String.Format("Taught {0} to all foes.", DataManager.Instance.GetSkill(SkillKeys[chosenSkill]).Name.ToLocal()), false, true);
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Sets the selected intrinsic ability on the focused character.
        /// </summary>
        public void btnSetIntrinsic_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("IntrinsicChoice", chosenIntrinsic);

                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
                    {
                        Character player = DungeonScene.Instance.FocusedCharacter;
                        player.LearnIntrinsic(IntrinsicKeys[chosenIntrinsic], 0);
                        DungeonScene.Instance.LogMsg(String.Format("Gave {1} to {0}.", player.Name, DataManager.Instance.GetIntrinsic(IntrinsicKeys[chosenIntrinsic]).Name.ToLocal()), false, true);
                    }
                }
                else if (GameManager.Instance.CurrentScene == GroundScene.Instance)
                {
                    if (DataManager.Instance.Save.ActiveTeam.Players.Count > 0)
                    {
                        Character player = DataManager.Instance.Save.ActiveTeam.Leader;
                        player.LearnIntrinsic(IntrinsicKeys[chosenIntrinsic], 0);
                        GameManager.Instance.SE("Menu/Sort");
                    }
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }

        /// <summary>
        /// Gives the selected intrinsic ability to all enemy characters on the map.
        /// </summary>
        public void btnGiveFoes_Click()
        {
            lock (GameBase.lockObj)
            {
                DevForm.SetConfig("IntrinsicChoice", chosenIntrinsic);

                if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                {
                    Character player = DungeonScene.Instance.FocusedCharacter;
                    foreach (Character character in ZoneManager.Instance.CurrentMap.IterateCharacters())
                    {
                        if (DungeonScene.Instance.GetMatchup(player, character) == Alignment.Foe)
                            character.LearnIntrinsic(IntrinsicKeys[chosenIntrinsic], 0);
                    }
                    DungeonScene.Instance.LogMsg(String.Format("Gave {0} to all foes.", DataManager.Instance.GetIntrinsic(IntrinsicKeys[chosenIntrinsic]).Name.ToLocal()), false, true);
                }
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }
        }
    }
}
