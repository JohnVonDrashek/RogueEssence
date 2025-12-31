using System;
using RogueEssence.Data;
using RogueEssence.Dev;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for item states that store mutable item data.
    /// Item states modify or enhance item behavior at runtime.
    /// </summary>
    [Serializable]
    public abstract class ItemState : GameplayState
    {

    }

    /// <summary>
    /// An item state that stores an index value.
    /// </summary>
    [Serializable]
    public class ItemIndexState : ItemState
    {
        /// <summary>
        /// The index value for this item state.
        /// </summary>
        public int Index;

        /// <summary>
        /// Initializes a new ItemIndexState with default values.
        /// </summary>
        public ItemIndexState() { }

        /// <summary>
        /// Initializes a new ItemIndexState with the specified index.
        /// </summary>
        /// <param name="idx">The index value.</param>
        public ItemIndexState(int idx) { Index = idx; }

        /// <summary>
        /// Creates a copy of another ItemIndexState.
        /// </summary>
        /// <param name="other">The ItemIndexState to copy.</param>
        protected ItemIndexState(ItemIndexState other) { Index = other.Index; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new ItemIndexState with the same values.</returns>
        public override GameplayState Clone() { return new ItemIndexState(this); }
    }


    /// <summary>
    /// An item state that stores a skill ID for skill-teaching items.
    /// </summary>
    // TODO: Rename this to ItemSkillIDState, it's only used for skills!!
    [Serializable]
    public class ItemIDState : ItemState
    {
        /// <summary>
        /// The skill ID this item teaches or contains.
        /// </summary>
        [DataType(0, DataManager.DataType.Skill, false)]
        public string ID;

        /// <summary>
        /// Initializes a new ItemIDState with an empty ID.
        /// </summary>
        public ItemIDState() { ID = ""; }

        /// <summary>
        /// Initializes a new ItemIDState with the specified skill ID.
        /// </summary>
        /// <param name="idx">The skill ID.</param>
        public ItemIDState(string idx) { ID = idx; }

        /// <summary>
        /// Creates a copy of another ItemIDState.
        /// </summary>
        /// <param name="other">The ItemIDState to copy.</param>
        protected ItemIDState(ItemIDState other) { ID = other.ID; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new ItemIDState with the same values.</returns>
        public override GameplayState Clone() { return new ItemIDState(this); }
    }

    /// <summary>
    /// An item state marking an item as a crafting material.
    /// </summary>
    [Serializable]
    public class MaterialState : ItemState
    {
        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MaterialState.</returns>
        public override GameplayState Clone() { return new MaterialState(); }
    }

}
