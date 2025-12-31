using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using RogueElements;
using Newtonsoft.Json;
using RogueEssence.Dev;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Interface for spawn generators that create characters in a team from a map context.
    /// </summary>
    /// <typeparam name="T">The type of map context.</typeparam>
    //TODO: port this interface down to RogueElements
    //to address concerns that map gen context may need to be passed into the logic responsible for creating a spawnable entity
    public interface ISpawnGenerator<T>
    {
        /// <summary>
        /// Spawns a character and adds it to the specified team.
        /// </summary>
        /// <param name="team">The team to add the character to.</param>
        /// <param name="map">The map context for spawning.</param>
        /// <returns>The spawned character.</returns>
        Character Spawn(Team team, T map);
    }

    /// <summary>
    /// Represents a single mob spawn configuration including species, level, skills, and other properties.
    /// Used to define what monsters appear in dungeons and how they are configured.
    /// </summary>
    [Serializable]
    public class MobSpawn : ISpawnable, ISpawnGenerator<IMobSpawnMap>
    {
        /// <summary>
        /// The species, form, etc. of the mob spawned.
        /// </summary>
        [Dev.MonsterID(0, false, false, true, true)]
        public MonsterID BaseForm;

        /// <summary>
        /// The level of the monster spawned.
        /// </summary>
        [Dev.SubGroup]
        [Dev.RangeBorder(0, false, true)]
        public RandRange Level;

        /// <summary>
        /// The skills for the mob.  Empty spaces will be filled based on its current level.
        /// </summary>
        [JsonConverter(typeof(SkillListConverter))]
        [Dev.DataType(1, DataManager.DataType.Skill, false)]
        public List<string> SpecifiedSkills;

        /// <summary>
        /// The passive skill for the mob.
        /// </summary>
        [JsonConverter(typeof(Dev.IntrinsicConverter))]
        [Dev.DataType(0, DataManager.DataType.Intrinsic, true)]
        public string Intrinsic;

        /// <summary>
        /// The mob's AI.
        /// </summary>
        [JsonConverter(typeof(Dev.AIConverter))]
        [Dev.DataType(0, DataManager.DataType.AI, false)]
        public string Tactic;

        /// <summary>
        /// Conditions that must be met in order for the mob to spawn.
        /// </summary>
        public List<MobSpawnCheck> SpawnConditions;

        /// <summary>
        /// Additional alterations made to the mob after it is created but before it is spawned.
        /// </summary>
        public List<MobSpawnExtra> SpawnFeatures;
        
        /// <summary>
        /// Initializes a new instance of the MobSpawn class with default values.
        /// </summary>
        public MobSpawn()
        {
            BaseForm = new MonsterID("", 0, "", Gender.Unknown);
            SpecifiedSkills = new List<string>();
            Intrinsic = "";
            Tactic = "";
            SpawnConditions = new List<MobSpawnCheck>();
            SpawnFeatures = new List<MobSpawnExtra>();
        }

        /// <summary>
        /// Initializes a new instance of the MobSpawn class as a copy of another.
        /// </summary>
        /// <param name="other">The MobSpawn to copy.</param>
        protected MobSpawn(MobSpawn other) : this()
        {
            BaseForm = other.BaseForm;
            Tactic = other.Tactic;
            Level = other.Level;
            SpecifiedSkills.AddRange(other.SpecifiedSkills);
            Intrinsic = other.Intrinsic;
            foreach (MobSpawnCheck extra in other.SpawnConditions)
                SpawnConditions.Add(extra.Copy());
            foreach (MobSpawnExtra extra in other.SpawnFeatures)
                SpawnFeatures.Add(extra.Copy());
        }
        /// <summary>
        /// Creates a copy of this MobSpawn.
        /// </summary>
        /// <returns>A new MobSpawn with copied data.</returns>
        public MobSpawn Copy() { return new MobSpawn(this); }
        ISpawnable ISpawnable.Copy() { return Copy(); }

        /// <summary>
        /// Determines whether this mob can spawn based on all spawn conditions.
        /// </summary>
        /// <returns>True if all spawn conditions pass; otherwise, false.</returns>
        public bool CanSpawn()
        {
            foreach (MobSpawnCheck extra in SpawnConditions)
            {
                if (!extra.CanSpawn())
                    return false;
            }
            return true;
        }

        protected Character SpawnBase(Team team, IMobSpawnMap map)
        {
            MonsterID formData = BaseForm;
            MonsterData dex = DataManager.Instance.GetMonster(formData.Species);
            if (formData.Form == -1)
            {
                int form = map.Rand.Next(dex.Forms.Count);
                formData.Form = form;
            }

            BaseMonsterForm formEntry = dex.Forms[formData.Form];

            if (formData.Gender == Gender.Unknown)
                formData.Gender = formEntry.RollGender(map.Rand);

            if (String.IsNullOrEmpty(formData.Skin))
                formData.Skin = formEntry.RollSkin(map.Rand);

            CharData character = new CharData();
            character.BaseForm = formData;
            character.Level = Level.Pick(map.Rand);
            
            List<string> final_skills = formEntry.RollLatestSkills(character.Level, SpecifiedSkills);
            for (int ii = 0; ii < final_skills.Count; ii++)
                character.BaseSkills[ii] = new SlotSkill(final_skills[ii]);

            if (String.IsNullOrEmpty(Intrinsic))
                character.SetBaseIntrinsic(formEntry.RollIntrinsic(map.Rand, 2));
            else
                character.SetBaseIntrinsic(Intrinsic);

            character.Discriminator = map.Rand.Next();

            Character new_mob = new Character(character);
            team.Players.Add(new_mob);

            return new_mob;
        }

        /// <summary>
        /// Spawns the mob with its configured properties and applies all spawn features.
        /// </summary>
        /// <param name="team">The team to add the character to.</param>
        /// <param name="map">The map context for spawning.</param>
        /// <returns>The spawned character.</returns>
        public virtual Character Spawn(Team team, IMobSpawnMap map)
        {
            Character newChar = SpawnBase(team, map);

            AITactic tactic = DataManager.Instance.GetAITactic(Tactic);
            newChar.Tactic = new AITactic(tactic);

            foreach (MobSpawnExtra spawnExtra in SpawnFeatures)
                spawnExtra.ApplyFeature(map, newChar);

            return newChar;
        }

        public override string ToString()
        {
            MonsterData entry = DataManager.Instance.GetMonster(BaseForm.Species);
            return String.Format("{0} Lv.{1}", entry.Name.ToLocal(), Level);
        }
    }


}
