using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for universal gameplay states that apply globally.
    /// Universal states affect game-wide mechanics rather than specific characters or items.
    /// </summary>
    [Serializable]
    public abstract class UniversalState : GameplayState
    {

    }
}
