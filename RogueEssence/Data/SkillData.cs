using System;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Contains all data for a skill/move that can be used in battle.
    /// Includes targeting, power, effects, and visual data.
    /// </summary>
    [Serializable]
    public class SkillData : IDescribedData
    {
        /// <summary>
        /// Returns the localized name of the skill.
        /// </summary>
        /// <returns>The skill name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The name of the data
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// The description of the data
        /// </summary>
        [Dev.Multiline(0)]
        public LocalText Desc { get; set; }

        /// <summary>
        /// Is it released and allowed to show up in the game?
        /// </summary>
        public bool Released { get; set; }

        /// <summary>
        /// Comments visible to only developers
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Index number of the skill for sorting.  Must be unique
        /// </summary>
        public int IndexNum;

        /// <summary>
        /// Generates a summary of this skill for indexing and quick access.
        /// </summary>
        /// <returns>An EntrySummary containing the skill's key metadata.</returns>
        public EntrySummary GenerateEntrySummary()
        {
            BasePowerState powerState = Data.SkillStates.GetWithDefault<BasePowerState>();
            SkillDataSummary summary = new SkillDataSummary(Name, Released, Comment, IndexNum);
            summary.RangeDescription = HitboxAction.GetDescription();
            summary.BaseCharges = BaseCharges;
            summary.BasePower = powerState != null ? powerState.Power : -1;
            summary.HitRate = Data.HitRate;
            summary.Category = Data.Category;
            summary.Element = Data.Element;
            summary.Description = Desc;
            return summary;
        }

        /// <summary>
        /// The number of times the skill can be used.
        /// </summary>
        public int BaseCharges;

        /// <summary>
        /// How many times the skill attacks.
        /// </summary>
        [Dev.NumberRange(0, 1, Int32.MaxValue)]
        public int Strikes;

        /// <summary>
        /// Data on the hitbox of the attack.  Controls range and targeting.
        /// </summary>
        public CombatAction HitboxAction;

        /// <summary>
        /// Optional data to specify a splash effect on the tiles hit.
        /// </summary>
        public ExplosionData Explosion;

        /// <summary>
        /// Events that occur with this skill.
        /// Before it's used, when it hits, after it's used, etc.
        /// </summary>
        public BattleData Data;


        /// <summary>
        /// Initializes a new instance of the SkillData class with default values.
        /// </summary>
        public SkillData()
        {
            Name = new LocalText();
            Desc = new LocalText();
            Comment = "";

            Data = new BattleData();
            Explosion = new ExplosionData();

            Strikes = 1;
            HitboxAction = new AttackAction();
        }
        

        /// <summary>
        /// Gets the colored text string of the skill
        /// </summary>
        /// <returns></returns>
        public string GetColoredName()
        {
            return String.Format("[color=#00FF00]{0}[color]", Name.ToLocal());
        }

        /// <summary>
        /// Gets the colored text string of the skill, with icon included
        /// </summary>
        /// <returns></returns>
        public string GetIconName()
        {
            ElementData element = DataManager.Instance.GetElement(Data.Element);
            return String.Format("{0}\u2060{1}", element.Symbol, GetColoredName());
        }
    }
    
    /// <summary>
    /// Summary data for a skill entry, including combat stats and range info.
    /// Used for quick access without loading full skill data.
    /// </summary>
    [Serializable]
    public class SkillDataSummary : EntrySummary
    {
        /// <summary>
        /// The elemental type of the skill.
        /// </summary>
        public string Element;

        /// <summary>
        /// The category (physical, special, status) of the skill.
        /// </summary>
        public BattleData.SkillCategory Category;

        /// <summary>
        /// The base power of the skill.
        /// </summary>
        public int BasePower;

        /// <summary>
        /// The number of times this skill can be used.
        /// </summary>
        public int BaseCharges;

        /// <summary>
        /// The base accuracy/hit rate of the skill.
        /// </summary>
        public int HitRate;

        /// <summary>
        /// A description of the skill's targeting range.
        /// </summary>
        public string RangeDescription;

        /// <summary>
        /// The localized description of the skill.
        /// </summary>
        public LocalText Description;

        /// <summary>
        /// Initializes a new empty instance of the SkillDataSummary class.
        /// </summary>
        public SkillDataSummary() : base() { }

        /// <summary>
        /// Initializes a new instance of the SkillDataSummary class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the skill.</param>
        /// <param name="released">Whether the skill is released for gameplay.</param>
        /// <param name="comment">Developer comment for this skill.</param>
        /// <param name="sort">The sort order for this skill.</param>
        public SkillDataSummary(LocalText name, bool released, string comment, int sort)
            : base(name, released, comment, sort)
        { }

        /// <summary>
        /// Gets the display name with green color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public override string GetColoredName()
        {
            return String.Format("[color=#00FF00]{0}[color]", Name.ToLocal());
        }

        /// <summary>
        /// Gets the colored text string of the skill, with icon included
        /// </summary>
        /// <returns></returns>
        public string GetIconName()
        {
            ElementData element = DataManager.Instance.GetElement(Element);
            return String.Format("{0}\u2060{1}", element.Symbol, GetColoredName());
        }
    }
}
