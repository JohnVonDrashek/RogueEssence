using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RogueEssence.Dev;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an evolution branch for a monster, including the result species
    /// and all requirements that must be met to evolve.
    /// </summary>
    [Serializable]
    public class PromoteBranch
    {
        /// <summary>
        /// The monster species ID that results from this evolution.
        /// </summary>
        [JsonConverter(typeof(MonsterConverter))]
        [Dev.DataType(0, DataManager.DataType.Monster, false)]
        public string Result;

        /// <summary>
        /// List of requirements and effects for this evolution.
        /// </summary>
        public List<PromoteDetail> Details;

        /// <summary>
        /// Initializes a new instance of the PromoteBranch class.
        /// </summary>
        public PromoteBranch()
        {
            Details = new List<PromoteDetail>();
        }

        /// <summary>
        /// Checks if a character meets all requirements for this evolution.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <param name="inDungeon">Whether the character is currently in a dungeon.</param>
        /// <returns>True if all requirements are met.</returns>
        public bool IsQualified(Character character, bool inDungeon)
        {
            foreach (PromoteDetail detail in Details)
            {
                if (!detail.GetReq(character, inDungeon))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Called before the evolution occurs to allow details to modify the result.
        /// </summary>
        /// <param name="character">The character being promoted.</param>
        /// <param name="inDungeon">Whether the promotion is happening in a dungeon.</param>
        /// <param name="result">The resulting monster ID, which can be modified.</param>
        public void BeforePromote(Character character, bool inDungeon, ref MonsterID result)
        {
            foreach (PromoteDetail detail in Details)
            {
                detail.BeforePromote(character, inDungeon, ref result);
            }
        }

        /// <summary>
        /// Called when the evolution occurs to consume items and apply effects.
        /// </summary>
        /// <param name="character">The character being promoted.</param>
        /// <param name="inDungeon">Whether the promotion is happening in a dungeon.</param>
        /// <param name="noGive">If true, skip consuming required items.</param>
        public void OnPromote(Character character, bool inDungeon, bool noGive)
        {
            foreach (PromoteDetail detail in Details)
            {
                if (noGive && !String.IsNullOrEmpty(detail.GetReqItem(character)))
                    continue;
                detail.OnPromote(character, inDungeon);
            }
        }

        /// <summary>
        /// Gets a human-readable string describing all non-hard requirements.
        /// </summary>
        /// <returns>A comma-separated list of requirement descriptions.</returns>
        public string GetReqString()
        {
            string reqList = "";
            foreach (PromoteDetail detail in Details)
            {
                if (!detail.IsHardReq() && detail.GetReqString() != "")
                {
                    if (reqList != "")
                        reqList += ", ";
                    reqList += detail.GetReqString();
                }
            }
            if (reqList != "")
                reqList = (reqList[0].ToString()).ToUpper() + reqList.Substring(1);
            return reqList;
        }
    }

    /// <summary>
    /// Abstract base class for evolution requirements and effects.
    /// Subclasses implement specific requirements like level, item, location, etc.
    /// </summary>
    [Serializable]
    public abstract class PromoteDetail
    {
        /// <summary>
        /// Gets the item required by this detail, if any.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns>The required item ID, or empty string if no item required.</returns>
        public virtual string GetReqItem(Character character) { return ""; }

        /// <summary>
        /// Gets a human-readable description of this requirement.
        /// </summary>
        /// <returns>The requirement description.</returns>
        public virtual string GetReqString() { return ""; }

        /// <summary>
        /// Indicates whether this is a hard requirement that cannot be shown.
        /// </summary>
        /// <returns>True if this is a hidden hard requirement.</returns>
        public virtual bool IsHardReq() { return false; }

        /// <summary>
        /// Checks if the character meets this requirement.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <param name="inDungeon">Whether the character is in a dungeon.</param>
        /// <returns>True if the requirement is met.</returns>
        public virtual bool GetReq(Character character, bool inDungeon) { return true; }

        /// <summary>
        /// Called before promotion to allow modifying the result.
        /// </summary>
        /// <param name="character">The character being promoted.</param>
        /// <param name="inDungeon">Whether in a dungeon.</param>
        /// <param name="result">The resulting monster ID to modify.</param>
        public virtual void BeforePromote(Character character, bool inDungeon, ref MonsterID result) { }

        /// <summary>
        /// Called when promotion occurs to apply effects like consuming items.
        /// </summary>
        /// <param name="character">The character being promoted.</param>
        /// <param name="inDungeon">Whether in a dungeon.</param>
        public virtual void OnPromote(Character character, bool inDungeon) { }
    }

}
