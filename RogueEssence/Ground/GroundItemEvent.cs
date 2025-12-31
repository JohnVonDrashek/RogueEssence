using System;
using System.Collections.Generic;
using System.Linq;
using NLua;
using RogueEssence.Data;
using RogueEssence.Dev;
using RogueEssence.Dungeon;
using RogueEssence.Menu;
using RogueEssence.Script;

namespace RogueEssence.Ground
{
    /// <summary>
    /// Specifies the target selection type for item usage.
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        /// apply to user
        /// </summary>
        Self,
        /// <summary>
        /// apply to other party members
        /// </summary>
        Others,
    }

    /// <summary>
    /// Abstract base class for events triggered when items are used in ground mode.
    /// Defines how items can be used outside of dungeon combat.
    /// </summary>
    public abstract class GroundItemEvent : GameEvent
    {
        /// <summary>
        /// The type of usage this item supports in ground mode.
        /// </summary>
        public ItemData.UseType GroundUsageType;

        /// <summary>
        /// The target selection type for this item use.
        /// </summary>
        public SelectionType Selection;

        /// <summary>
        /// Applies the item event effect to the given context.
        /// </summary>
        /// <param name="context">The ground context containing item, owner, and target information.</param>
        /// <returns>A coroutine enumerator for the item effect.</returns>
        public abstract IEnumerator<YieldInstruction> Apply(GroundContext context);
    }

    /// <summary>
    /// An item event that executes a Lua script when the item is used.
    /// </summary>
    [Serializable]
    public class ScriptItemEvent : GroundItemEvent
    {
        [Dev.Sanitize(0)]
        public string Script;
        [Dev.Multiline(0)]
        public string ArgTable;

        public ScriptItemEvent() { Script = ""; ArgTable = "{}"; }
        public ScriptItemEvent(string script) { Script = script; ArgTable = "{}"; }
        public ScriptItemEvent(string script, string argTable) { Script = script; ArgTable = argTable; }
        protected ScriptItemEvent(ScriptItemEvent other)
        {
            Script = other.Script;
            ArgTable = other.ArgTable;
        }
        public override GameEvent Clone() { return new ScriptItemEvent(this); }

        public override IEnumerator<YieldInstruction> Apply(GroundContext context)
        {
            LuaTable args = LuaEngine.Instance.RunString("return " + ArgTable).First() as LuaTable;
            object[] parameters = new object[] { context, args };
            string name = LuaEngine.EVENT_GROUNDITEM_NAME + "." + Script;
            LuaFunction func_iter = LuaEngine.Instance.CreateCoroutineIterator(name, parameters);

            yield return CoroutineManager.Instance.StartCoroutine(ScriptEvent.ApplyFunc(name, func_iter));
        }
    }
    
    /// <summary>
    /// An item event that teaches a skill to a character when the item is used.
    /// </summary>
    [Serializable]
    public class LearnItemEvent : GroundItemEvent
    {
        [DataType(0, DataManager.DataType.Skill, false)] 
        public string Skill;
        public LearnItemEvent() { Skill = ""; }
        public LearnItemEvent(string skill) { Skill = skill; }
        protected LearnItemEvent(LearnItemEvent other) { Skill = other.Skill; }

        public override GameEvent Clone() { return new LearnItemEvent(this); }
        public override IEnumerator<YieldInstruction> Apply(GroundContext context)
        {
            Character target = context.User;
            
            int learn = -1;
            
            yield return CoroutineManager.Instance.StartCoroutine(DungeonScene.TryLearnSkill(target, Skill, (int slot) => { learn = slot; }, () => { context.CancelState.Cancel = true; }));
            if (context.CancelState.Cancel) yield break;
            
            yield return CoroutineManager.Instance.StartCoroutine(
                DungeonScene.LearnSkillWithFanfare(target, Skill, learn));
        }
    }
}