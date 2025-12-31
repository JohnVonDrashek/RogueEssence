using System;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using RogueElements;
using NLua;
using RogueEssence.Script;
using System.Linq;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Abstract base class for extra modifications applied to a mob after it is created but before spawning.
    /// Implementations can add statuses, run scripts, or make other modifications.
    /// </summary>
    [Serializable]
    public abstract class MobSpawnExtra
    {
        /// <summary>
        /// Creates a copy of this spawn extra.
        /// </summary>
        /// <returns>A new MobSpawnExtra with copied data.</returns>
        public abstract MobSpawnExtra Copy();

        /// <summary>
        /// Applies the extra feature to the newly created character.
        /// </summary>
        /// <param name="map">The map context.</param>
        /// <param name="newChar">The character to modify.</param>
        public abstract void ApplyFeature(IMobSpawnMap map, Character newChar);
    }


    /// <summary>
    /// Spawns the mob with a status problem.
    /// </summary>
    [Serializable]
    public class MobSpawnStatus : MobSpawnExtra
    {
        /// <summary>
        /// The possible statuses.  Picks one.
        /// </summary>
        public SpawnList<StatusEffect> Statuses;

        /// <summary>
        /// Initializes a new instance of the MobSpawnStatus class.
        /// </summary>
        public MobSpawnStatus()
        {
            Statuses = new SpawnList<StatusEffect>();
        }

        /// <summary>
        /// Initializes a new instance of the MobSpawnStatus class as a copy of another.
        /// </summary>
        /// <param name="other">The MobSpawnStatus to copy.</param>
        public MobSpawnStatus(MobSpawnStatus other) : this()
        {
            for (int ii = 0; ii < other.Statuses.Count; ii++)
                Statuses.Add(other.Statuses.GetSpawn(ii).Clone(), other.Statuses.GetSpawnRate(ii));
        }

        /// <summary>
        /// Creates a copy of this spawn extra.
        /// </summary>
        /// <returns>A new MobSpawnStatus with copied data.</returns>
        public override MobSpawnExtra Copy() { return new MobSpawnStatus(this); }

        /// <summary>
        /// Applies a random status effect from the list to the character.
        /// </summary>
        /// <param name="map">The map context.</param>
        /// <param name="newChar">The character to apply the status to.</param>
        public override void ApplyFeature(IMobSpawnMap map, Character newChar)
        {
            StatusEffect status = Statuses.Pick(map.Rand).Clone();//Clone Use Case; convert to Instantiate?
            status.LoadFromData();
            StatusData entry = (StatusData)status.GetData();
            if (!entry.Targeted)//no targeted statuses allowed
            {
                //need to also add the additional status states
                newChar.StatusEffects.Add(status.ID, status);
            }
        }

        public override string ToString()
        {
            if (Statuses.Count != 1)
                return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), Statuses.Count.ToString());
            else
            {
                EntrySummary summary = DataManager.Instance.DataIndices[DataManager.DataType.Status].Get(Statuses.GetSpawn(0).ID);
                return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), summary.Name.ToLocal());
            }
        }
    }

    /// <summary>
    /// Runs a script to modify the spawning mob.
    /// </summary>
    [Serializable]
    public class MobSpawnScript : MobSpawnExtra
    {
        /// <summary>
        /// The name of the Lua script function to run.
        /// </summary>
        [Dev.Sanitize(0)]
        public string Script;

        /// <summary>
        /// A Lua table string containing arguments to pass to the script.
        /// </summary>
        [Dev.Multiline(0)]
        public string ArgTable;

        /// <summary>
        /// Initializes a new instance of the MobSpawnScript class.
        /// </summary>
        public MobSpawnScript() { Script = ""; ArgTable = ""; }

        /// <summary>
        /// Initializes a new instance of the MobSpawnScript class with the specified script.
        /// </summary>
        /// <param name="script">The script function name.</param>
        public MobSpawnScript(string script)  { Script = script; ArgTable = ""; }

        /// <summary>
        /// Initializes a new instance of the MobSpawnScript class with the specified script and arguments.
        /// </summary>
        /// <param name="script">The script function name.</param>
        /// <param name="argTable">The Lua argument table string.</param>
        public MobSpawnScript(string script, string argTable)  { Script = script; ArgTable = argTable; }

        /// <summary>
        /// Initializes a new instance of the MobSpawnScript class as a copy of another.
        /// </summary>
        /// <param name="other">The MobSpawnScript to copy.</param>
        public MobSpawnScript(MobSpawnScript other) : this()
        {
            Script = other.Script;
            ArgTable = other.ArgTable;
        }

        /// <summary>
        /// Creates a copy of this spawn extra.
        /// </summary>
        /// <returns>A new MobSpawnScript with copied data.</returns>
        public override MobSpawnExtra Copy() { return new MobSpawnScript(this); }

        /// <summary>
        /// Runs the configured Lua script to modify the character.
        /// </summary>
        /// <param name="map">The map context.</param>
        /// <param name="newChar">The character to modify.</param>
        public override void ApplyFeature(IMobSpawnMap map, Character newChar)
        {
            LuaTable args = LuaEngine.Instance.RunString("return " + ArgTable).First() as LuaTable;
            object[] parameters = new object[] { map, newChar, args };
            string name = LuaEngine.EVENT_SPAWN_NAME + "." + Script;

            LuaEngine.Instance.CallLuaFunctions(name, parameters);
        }

        public override string ToString()
        {
            string name = LuaEngine.EVENT_SPAWN_NAME + "." + Script;
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), name);
        }
    }

}
